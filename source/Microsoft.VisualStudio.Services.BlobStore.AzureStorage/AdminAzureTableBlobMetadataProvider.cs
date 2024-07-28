// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.AzureStorage.AdminAzureTableBlobMetadataProvider
// Assembly: Microsoft.VisualStudio.Services.BlobStore.AzureStorage, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8BF1D977-E244-4825-BEA6-8BA4E1DDDB84
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.AzureStorage.dll

using BuildXL.Cache.ContentStore.Hashing;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.VisualStudio.Services.BlobStore.Common;
using Microsoft.VisualStudio.Services.BlobStore.Server.Common;
using Microsoft.VisualStudio.Services.BlobStore.Server.Common.Domains;
using Microsoft.VisualStudio.Services.BlobStore.Server.Common.Models;
using Microsoft.VisualStudio.Services.Content.Common;
using Microsoft.VisualStudio.Services.Content.Server.Azure;
using Microsoft.VisualStudio.Services.Content.Server.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace Microsoft.VisualStudio.Services.BlobStore.AzureStorage
{
  public class AdminAzureTableBlobMetadataProvider : 
    AzureTableBlobMetadataProvider,
    IAdminBlobMetadataProvider,
    IBlobMetadataProvider,
    IDisposable
  {
    public static readonly TimeSpan ClockSkew = TimeSpan.FromMinutes(5.0);
    private const int BoundedCapacityForBlobMetadata = 1000;

    public AdminAzureTableBlobMetadataProvider(
      ITableClientFactory tableClientFactory,
      AzureTableBlobMetadataProviderOptions options)
      : base(tableClientFactory, options)
    {
    }

    public Task PerformSweepOperationAsync(
      VssRequestPump.Processor processor,
      SweepOperation sweepOperation,
      BlobIdentifier firstBlobId,
      BlobIdentifier lastBlobId)
    {
      throw new ArgumentException(string.Format("Unknown SweepOperation: {0}", (object) sweepOperation), nameof (sweepOperation));
    }

    private async Task CheckForMetadataRow(
      VssRequestPump.Processor vssProcessor,
      ITable foundTable,
      ValidateMetadataAndCopyBlobsToCorrectShardsResults results,
      AzureTableBlobMetadataProvider.BlobMetaDataRowTableEntity metadataRow,
      Func<BlobIdentifier, Task<bool>> tryReuploadBlobFunc,
      Func<int, string, Task> traceFunc)
    {
      AdminAzureTableBlobMetadataProvider metadataProvider = this;
      Interlocked.Increment(ref results.MetadataChecked);
      List<BlobReference> references;
      if (metadataProvider.GetTableShard(metadataRow.BlobId).StorageAccountName == foundTable.StorageAccountName)
      {
        references = (List<BlobReference>) null;
      }
      else
      {
        Interlocked.Increment(ref results.MetadataOnWrongShard);
        references = new List<BlobReference>();
        AzureTableBlobMetadataProvider.KeepUntilBlobReferenceRowTableEntity referenceRowTableEntity = await metadataProvider.GetKeepUntilRowAsync(vssProcessor, foundTable, metadataRow.BlobId).ConfigureAwait(false);
        if (referenceRowTableEntity != null)
          references.Add(new BlobReference(referenceRowTableEntity.KeepUntil));
        foreach (IdBlobReference reference in await metadataProvider.GetIdReferencesInternalAsync(vssProcessor, foundTable, metadataRow.BlobId, new int?()).ConfigureAwait(false))
          references.Add(new BlobReference(reference));
        // ISSUE: explicit non-virtual call
        ConfiguredTaskAwaitable<IEnumerable<ReferenceResult>> configuredTaskAwaitable = __nonvirtual (metadataProvider.TryReferenceAsync(vssProcessor, metadataRow.BlobId, (IEnumerable<BlobReference>) references)).ConfigureAwait(false);
        if ((await configuredTaskAwaitable).All<ReferenceResult>((Func<ReferenceResult, bool>) (r => r.Success)))
        {
          Interlocked.Increment(ref results.MetadataCopiedToCorrectShard);
          references = (List<BlobReference>) null;
        }
        else if (await tryReuploadBlobFunc(metadataRow.BlobId).ConfigureAwait(false))
        {
          // ISSUE: explicit non-virtual call
          configuredTaskAwaitable = __nonvirtual (metadataProvider.TryReferenceAsync(vssProcessor, metadataRow.BlobId, (IEnumerable<BlobReference>) references)).ConfigureAwait(false);
          if ((await configuredTaskAwaitable).All<ReferenceResult>((Func<ReferenceResult, bool>) (r => r.Success)))
          {
            Interlocked.Increment(ref results.MetadataCopiedToCorrectShardAfterReupload);
            references = (List<BlobReference>) null;
          }
          else
          {
            await traceFunc(5701190, string.Format("Could not reference blob '{0}' even after successful reupload.", (object) metadataRow.BlobId)).ConfigureAwait(false);
            Interlocked.Increment(ref results.MetadataFailedToAddReferencesEvenAfterReupload);
            references = (List<BlobReference>) null;
          }
        }
        else
        {
          await traceFunc(5701200, string.Format("Could not reference blob '{0}' even after successful reupload.", (object) metadataRow.BlobId)).ConfigureAwait(false);
          Interlocked.Increment(ref results.MetadataFailedToAddReferencesReuploadFailed);
          references = (List<BlobReference>) null;
        }
      }
    }

    public async Task FixMetadataAfterDisasterAsync(
      VssRequestPump.Processor processor,
      FixMetadataAfterDisasterResults results,
      IBlobProviderDomain domain,
      BlobIdentifier startingBlobId,
      BlobIdentifier lastBlobId,
      DateTime lastSyncTime,
      bool skipDeletion,
      Func<int, TraceLevel, string, Task> traceFunc,
      CancellationToken cancellationToken)
    {
      AdminAzureTableBlobMetadataProvider metadataProvider = this;
      ExponentialRetry retryPolicy = new ExponentialRetry(TimeSpan.FromMilliseconds(10.0), 16);
      IEnumerable<ITable> allTables = metadataProvider.TableClientFactory.GetAllTables();
      Action<IList<TableOperationResult>> action;
      Func<ITable, Task> action1 = (Func<ITable, Task>) (async table =>
      {
        table.RetryPolicy = (IRetryPolicy) retryPolicy;
        DateTime date = lastSyncTime.ToUniversalTime() - AdminAzureTableBlobMetadataProvider.ClockSkew;
        Stopwatch tickTock = Stopwatch.StartNew();
        PartitionRangeExactRowQuery<AzureTableBlobMetadataProvider.BlobMetaDataRowTableEntity> scanQuery = PartitionRangeExactRowQuery<AzureTableBlobMetadataProvider.BlobMetaDataRowTableEntity>.Create<AzureTableBlobMetadataProvider.BlobMetaDataRowTableEntity>(new RangeFilter<PartitionKeyColumn>(new RangeMinimumBoundary<PartitionKeyColumn>((IColumnValue<PartitionKeyColumn>) new AzureTableBlobMetadataProvider.BlobIdPartitionKeyColumnValue(startingBlobId), RangeBoundaryType.Inclusive), new RangeMaximumBoundary<PartitionKeyColumn>((IColumnValue<PartitionKeyColumn>) new AzureTableBlobMetadataProvider.BlobIdPartitionKeyColumnValue(lastBlobId), RangeBoundaryType.Inclusive)), (IFilter<INonUserColumn>) new ComparisonFilter<TimestampColumn>((IColumnValue<TimestampColumn>) new TimestampColumnValue(date), ComparisonOperator.GreaterThanOrEqual), (IFilter<UserColumn>) new ComparisonFilter<UserColumn>((IColumnValue<UserColumn>) new AzureTableBlobMetadataProvider.BlobMetaDataRowTableEntity.ReferenceStateColumnValue(BlobReferenceState.AddedBlob), ComparisonOperator.Equal));
        tickTock.Stop();
        await traceFunc(5701170, TraceLevel.Verbose, string.Format("Range query TableName={0}, StorageAccName={1}, TimeWindow={2}, ScanRangeStart={3}, ScanRangeEnd={4}, TimeElapsed={5}", (object) table.Name, (object) table.StorageAccountName, (object) date, (object) startingBlobId, (object) lastBlobId, (object) tickTock.ElapsedMilliseconds));
        tickTock.Restart();
        await table.QueryConcurrentIterator<AzureTableBlobMetadataProvider.BlobMetaDataRowTableEntity>(processor, (Query<AzureTableBlobMetadataProvider.BlobMetaDataRowTableEntity>) scanQuery).ForEachAsyncNoContext<AzureTableBlobMetadataProvider.BlobMetaDataRowTableEntity>(cancellationToken, (Func<AzureTableBlobMetadataProvider.BlobMetaDataRowTableEntity, Task>) (async metadataRow =>
        {
          Interlocked.Increment(ref results.MetadataEnumerated);
          try
          {
            string str = await domain.FindProvider(metadataRow.BlobId).GetBlobEtagAsync(processor, metadataRow.BlobId).ConfigureAwait(false);
            if (str == null)
            {
              Interlocked.Increment(ref results.MissingBlob);
              await traceFunc(5701250, TraceLevel.Error, string.Format("Could not find blob '{0}'.", (object) metadataRow.BlobId)).ConfigureAwait(false);
              if (skipDeletion)
                return;
              Stopwatch tickTockInner = Stopwatch.StartNew();
              TableBatchOperationDescriptor batchOp = new TableBatchOperationDescriptor();
              PartitionScanQuery<DynamicTableEntity> partitionScanQuery = new PartitionScanQuery<DynamicTableEntity>((StringColumnValue<PartitionKeyColumn>) new AzureTableBlobMetadataProvider.BlobIdPartitionKeyColumnValue(metadataRow.BlobId));
              await table.QueryConcurrentIterator<DynamicTableEntity>(processor, (Query<DynamicTableEntity>) partitionScanQuery).ForEachAsyncNoContext<DynamicTableEntity>(cancellationToken, (Action<DynamicTableEntity>) (entity => batchOp.Add(TableOperationDescriptor.Delete((ITableEntity) entity))));
              (await table.ExecuteBatchAsync(processor, batchOp)).Match(action ?? (action = (Action<IList<TableOperationResult>>) (success => Interlocked.Increment(ref results.RemovedMetadata))), (Action<TableBatchOperationResult.Error>) (error =>
              {
                Interlocked.Increment(ref results.FailedToRemoveMetadata);
                traceFunc(5701230, TraceLevel.Error, string.Format("Could not remove metadata for '{0}'.", (object) metadataRow.BlobId)).Wait();
              }));
              tickTockInner.Stop();
              await traceFunc(5701170, TraceLevel.Verbose, string.Format("Blob={0}, TableName={1}, StorageAccName={2}, TimeElapsed={3}", (object) metadataRow.BlobId, (object) table.Name, (object) table.StorageAccountName, (object) tickTockInner.ElapsedMilliseconds));
              return;
            }
            if (str != metadataRow.BlobEtag)
            {
              Interlocked.Increment(ref results.BlobEtagMismatch);
              return;
            }
            Interlocked.Increment(ref results.BlobsPassed);
          }
          catch (Exception ex)
          {
            Interlocked.Increment(ref results.ExceptionsThrown);
            await traceFunc(5701240, TraceLevel.Error, string.Format("Failure on {0}: {1}", (object) metadataRow.BlobId, (object) ex.ToString())).ConfigureAwait(false);
          }
        }));
        tickTock.Stop();
        await traceFunc(5701170, TraceLevel.Verbose, string.Format("Completed range iteration on ScanRangeStart={0}, ScanRangeEnd={1}, TableName={2}, StorageAccName={3}, TimeElapsed={4}", (object) startingBlobId, (object) lastBlobId, (object) table.Name, (object) table.StorageAccountName, (object) tickTock.ElapsedMilliseconds));
        tickTock = (Stopwatch) null;
        scanQuery = (PartitionRangeExactRowQuery<AzureTableBlobMetadataProvider.BlobMetaDataRowTableEntity>) null;
      });
      ExecutionDataflowBlockOptions dataflowBlockOptions = new ExecutionDataflowBlockOptions();
      dataflowBlockOptions.MaxDegreeOfParallelism = allTables.Count<ITable>();
      dataflowBlockOptions.CancellationToken = cancellationToken;
      await NonSwallowingActionBlock.Create<ITable>(action1, dataflowBlockOptions).SendAllAndCompleteSingleBlockNetworkAsync<ITable>(allTables, cancellationToken).ConfigureAwait(false);
    }

    public IConcurrentIterator<KeyValuePair<BlobIdentifier, DateTime>> GetBlobIdentifiersWithExpiredKeepUntilConcurrentIterator(
      VssRequestPump.Processor processor,
      BlobIdentifier startingBlobId,
      BlobIdentifier endingBlobIdOrNull,
      IteratorPartition partition,
      DateTime expiredUntilDate)
    {
      ComparisonFilter<UserColumn> comparisonFilter = new ComparisonFilter<UserColumn>((IColumnValue<UserColumn>) new AzureTableBlobMetadataProvider.KeepUntilColumnValue(expiredUntilDate), ComparisonOperator.LessThan);
      return this.QueryAllShardsOrdered<AzureTableBlobMetadataProvider.KeepUntilBlobReferenceRowTableEntity>(processor, startingBlobId, endingBlobIdOrNull, partition, (IFilter<UserColumn>) comparisonFilter).Select<AzureTableBlobMetadataProvider.KeepUntilBlobReferenceRowTableEntity, KeyValuePair<BlobIdentifier, DateTime>>((Func<AzureTableBlobMetadataProvider.KeepUntilBlobReferenceRowTableEntity, KeyValuePair<BlobIdentifier, DateTime>>) (e => new KeyValuePair<BlobIdentifier, DateTime>(e.BlobId, e.KeepUntil)));
    }

    public IConcurrentIterator<IBlobMetadataSizeInfo> GetBlobMetadataSizeConcurrentIterator(
      VssRequestPump.Processor processor,
      BlobIdentifier startingBlobIdOrNull,
      BlobIdentifier endingBlobIdOrNull,
      IteratorPartition partition,
      bool excludeStartId,
      IEnumerable<IBlobIdReferenceRowVisitor> blobIdReferenceRowVisitors)
    {
      RangeMinimumBoundary<PartitionKeyColumn> minimum = (RangeMinimumBoundary<PartitionKeyColumn>) null;
      if (startingBlobIdOrNull != (BlobIdentifier) null && startingBlobIdOrNull != BlobIdentifier.MinValue)
      {
        RangeBoundaryType boundaryType = excludeStartId ? RangeBoundaryType.Exclusive : RangeBoundaryType.Inclusive;
        minimum = new RangeMinimumBoundary<PartitionKeyColumn>((IColumnValue<PartitionKeyColumn>) new PartitionKeyColumnValue(startingBlobIdOrNull.ValueString), boundaryType);
      }
      RangeMaximumBoundary<PartitionKeyColumn> maximum = (RangeMaximumBoundary<PartitionKeyColumn>) null;
      if (endingBlobIdOrNull != (BlobIdentifier) null && endingBlobIdOrNull != BlobIdentifier.MaxValue)
        maximum = new RangeMaximumBoundary<PartitionKeyColumn>((IColumnValue<PartitionKeyColumn>) new PartitionKeyColumnValue(endingBlobIdOrNull.ValueString), RangeBoundaryType.Exclusive);
      PartitionRangeSpecificRowsQuery<DynamicTableEntity> query = new PartitionRangeSpecificRowsQuery<DynamicTableEntity>(new RangeFilter<PartitionKeyColumn>(minimum, maximum), (StringColumnValue<RowKeyColumn>[]) new RowKeyColumnValue[2]
      {
        AzureTableBlobMetadataProvider.EmptyBlobMetaDataRowTableEntity.RowKeyColumnValueInstance,
        AzureTableBlobMetadataProvider.KeepUntilBlobReferenceRowTableEntity.RowKeyColumnValueInstance
      }, AzureTableBlobMetadataProvider.IdBlobReferenceRowTableEntity.RowRangeFilter);
      IEnumerable<ITable> allTables = this.TableClientFactory.GetAllTables();
      IConcurrentIterator<DynamicTableEntity> tableEnum = (partition?.SelectIterators<ITable>(allTables) ?? allTables).Select<ITable, IConcurrentIterator<DynamicTableEntity>>((Func<ITable, IConcurrentIterator<DynamicTableEntity>>) (table => table.QueryConcurrentIterator<DynamicTableEntity>(processor, (Query<DynamicTableEntity>) query))).CollectSortOrdered<DynamicTableEntity>(new int?(1000), (IComparer<DynamicTableEntity>) new AdminAzureTableBlobMetadataProvider.EntityComparer(), processor.CancellationToken);
      return this.GetBlobMetadataSizeInfoConcurrentIterator(processor, tableEnum, blobIdReferenceRowVisitors);
    }

    public IConcurrentIterator<IBlobMetadataProjectScopedSizeInfo> GetBlobMetadataProjectScopedSizeConcurrentIterator(
      VssRequestPump.Processor processor,
      BlobIdentifier startingBlobIdOrNull,
      BlobIdentifier endingBlobIdOrNull,
      IteratorPartition partition,
      bool excludeStartId,
      bool enableFeedInfoExport)
    {
      RangeMinimumBoundary<PartitionKeyColumn> minimum = (RangeMinimumBoundary<PartitionKeyColumn>) null;
      if (startingBlobIdOrNull != (BlobIdentifier) null && startingBlobIdOrNull != BlobIdentifier.MinValue)
      {
        RangeBoundaryType boundaryType = excludeStartId ? RangeBoundaryType.Exclusive : RangeBoundaryType.Inclusive;
        minimum = new RangeMinimumBoundary<PartitionKeyColumn>((IColumnValue<PartitionKeyColumn>) new PartitionKeyColumnValue(startingBlobIdOrNull.ValueString), boundaryType);
      }
      RangeMaximumBoundary<PartitionKeyColumn> maximum = (RangeMaximumBoundary<PartitionKeyColumn>) null;
      if (endingBlobIdOrNull != (BlobIdentifier) null && endingBlobIdOrNull != BlobIdentifier.MaxValue)
        maximum = new RangeMaximumBoundary<PartitionKeyColumn>((IColumnValue<PartitionKeyColumn>) new PartitionKeyColumnValue(endingBlobIdOrNull.ValueString), RangeBoundaryType.Exclusive);
      PartitionRangeSpecificRowsQuery<DynamicTableEntity> query = new PartitionRangeSpecificRowsQuery<DynamicTableEntity>(new RangeFilter<PartitionKeyColumn>(minimum, maximum), (StringColumnValue<RowKeyColumn>[]) new RowKeyColumnValue[2]
      {
        AzureTableBlobMetadataProvider.EmptyBlobMetaDataRowTableEntity.RowKeyColumnValueInstance,
        AzureTableBlobMetadataProvider.KeepUntilBlobReferenceRowTableEntity.RowKeyColumnValueInstance
      }, AzureTableBlobMetadataProvider.IdBlobReferenceRowTableEntity.RowRangeFilter);
      IEnumerable<ITable> allTables = this.TableClientFactory.GetAllTables();
      IConcurrentIterator<DynamicTableEntity> tableEnum = (partition?.SelectIterators<ITable>(allTables) ?? allTables).Select<ITable, IConcurrentIterator<DynamicTableEntity>>((Func<ITable, IConcurrentIterator<DynamicTableEntity>>) (table => table.QueryConcurrentIterator<DynamicTableEntity>(processor, (Query<DynamicTableEntity>) query))).CollectSortOrdered<DynamicTableEntity>(new int?(1000), (IComparer<DynamicTableEntity>) new AdminAzureTableBlobMetadataProvider.EntityComparer(), processor.CancellationToken);
      return this.GetBlobMetadataProjectScopedConcurrentIterator(processor, tableEnum, enableFeedInfoExport);
    }

    private IConcurrentIterator<IBlobMetadataSizeInfo> GetBlobMetadataSizeInfoConcurrentIterator(
      VssRequestPump.Processor processor,
      IConcurrentIterator<DynamicTableEntity> tableEnum,
      IEnumerable<IBlobIdReferenceRowVisitor> blobIdReferenceRowVisitors)
    {
      return (IConcurrentIterator<IBlobMetadataSizeInfo>) new ConcurrentIterator<IBlobMetadataSizeInfo>(new int?(1000), processor.CancellationToken, (Func<TryAddValueAsyncFunc<IBlobMetadataSizeInfo>, CancellationToken, Task>) (async (valueAdderAsync, cancellationToken) =>
      {
        string lastPartitionKey = (string) null;
        AzureTableBlobMetadataProvider.KeepUntilBlobReferenceRowTableEntity lastKeepUntilRow = (AzureTableBlobMetadataProvider.KeepUntilBlobReferenceRowTableEntity) null;
        AzureTableBlobMetadataProvider.BlobMetadataSizeInfo blobInfo = (AzureTableBlobMetadataProvider.BlobMetadataSizeInfo) null;
        string blobMetadataPK = (string) null;
        OperationContext context = new OperationContext();
        DynamicTableEntity row;
        while (true)
        {
          if (await tableEnum.MoveNextAsync(cancellationToken).ConfigureAwait(false))
          {
            row = tableEnum.Current;
            IDictionary<string, EntityProperty> properties = row.WriteEntity(context);
            if (!row.PartitionKey.Equals(lastPartitionKey, StringComparison.Ordinal))
            {
              if (blobInfo != null)
              {
                int num = await valueAdderAsync((IBlobMetadataSizeInfo) blobInfo).ConfigureAwait(false) ? 1 : 0;
                blobInfo = (AzureTableBlobMetadataProvider.BlobMetadataSizeInfo) null;
                blobMetadataPK = (string) null;
              }
              lastPartitionKey = row.PartitionKey;
            }
            if (row.RowKey.Equals("keepUntil", StringComparison.Ordinal))
            {
              lastKeepUntilRow = new AzureTableBlobMetadataProvider.KeepUntilBlobReferenceRowTableEntity()
              {
                PartitionKey = row.PartitionKey,
                RowKey = row.RowKey
              };
              lastKeepUntilRow.ReadEntity(properties, context);
            }
            else if (row.RowKey.Equals("metadata", StringComparison.Ordinal))
            {
              AzureTableBlobMetadataProvider.BlobMetaDataRowTableEntity metadataRow = new AzureTableBlobMetadataProvider.BlobMetaDataRowTableEntity()
              {
                PartitionKey = row.PartitionKey,
                RowKey = row.RowKey
              };
              metadataRow.ReadEntity(properties, context);
              blobInfo = new AzureTableBlobMetadataProvider.BlobMetadataSizeInfo(metadataRow);
              blobMetadataPK = metadataRow.PartitionKey;
              if (lastKeepUntilRow != null && row.PartitionKey.Equals(lastKeepUntilRow.PartitionKey, StringComparison.Ordinal))
                blobInfo.KeepUntilTime = new DateTimeOffset?(new DateTimeOffset(lastKeepUntilRow.KeepUntil, TimeSpan.Zero));
            }
            else if (row.RowKey.StartsWith("ref_", StringComparison.Ordinal))
            {
              if (blobInfo != null && row.PartitionKey.Equals(blobMetadataPK, StringComparison.Ordinal))
              {
                ++blobInfo.IdReferenceCount;
                AzureTableBlobMetadataProvider.IdBlobReferenceRowTableEntity referenceRowTableEntity = new AzureTableBlobMetadataProvider.IdBlobReferenceRowTableEntity()
                {
                  PartitionKey = row.PartitionKey,
                  RowKey = row.RowKey
                };
                referenceRowTableEntity.ReadEntity(properties, context);
                blobInfo.IdReferenceCountByScope.AddOrUpdate(ArtifactScopeHelper.GetScopeTypeFromScopeId(referenceRowTableEntity.ScopeId), 1L, (Func<ArtifactScopeType, long, long>) ((key, cur) => cur + 1L));
                if (referenceRowTableEntity.ReferenceId.StartsWith("feed"))
                {
                  string[] source = referenceRowTableEntity.ReferenceId.Split('/');
                  if (((IEnumerable<string>) source).Count<string>() >= 2 && source[0].Equals("feed"))
                  {
                    long num = (long) blobInfo.IdReferenceCountByFeed.AddOrUpdate(source[1], 1UL, (Func<string, ulong, ulong>) ((key, cur) => cur + 1UL));
                  }
                }
                IdBlobReferenceRow idBlobReferenceRow = new IdBlobReferenceRow()
                {
                  BlobId = referenceRowTableEntity.BlobId,
                  ReferenceId = referenceRowTableEntity.ReferenceId,
                  ScopeId = referenceRowTableEntity.ScopeId,
                  LastModified = referenceRowTableEntity.Timestamp
                };
                foreach (IBlobIdReferenceRowVisitor referenceRowVisitor in blobIdReferenceRowVisitors)
                {
                  try
                  {
                    referenceRowVisitor.VisitIdReference(idBlobReferenceRow);
                  }
                  catch
                  {
                  }
                }
              }
            }
            else
              break;
            row = (DynamicTableEntity) null;
            properties = (IDictionary<string, EntityProperty>) null;
          }
          else
            goto label_28;
        }
        throw new InvalidOperationException("Unknown row: " + row.RowKey);
label_28:
        if (blobInfo == null)
        {
          lastPartitionKey = (string) null;
          lastKeepUntilRow = (AzureTableBlobMetadataProvider.KeepUntilBlobReferenceRowTableEntity) null;
          blobInfo = (AzureTableBlobMetadataProvider.BlobMetadataSizeInfo) null;
          blobMetadataPK = (string) null;
          context = (OperationContext) null;
        }
        else
        {
          int num = await valueAdderAsync((IBlobMetadataSizeInfo) blobInfo).ConfigureAwait(false) ? 1 : 0;
          lastPartitionKey = (string) null;
          lastKeepUntilRow = (AzureTableBlobMetadataProvider.KeepUntilBlobReferenceRowTableEntity) null;
          blobInfo = (AzureTableBlobMetadataProvider.BlobMetadataSizeInfo) null;
          blobMetadataPK = (string) null;
          context = (OperationContext) null;
        }
      }));
    }

    private IConcurrentIterator<BlobReferenceDetailInfo> GetBlobBlobReferenceDetailInfoConcurrentIterator(
      VssRequestPump.Processor processor)
    {
      PartitionRangeSpecificRowsQuery<DynamicTableEntity> query = new PartitionRangeSpecificRowsQuery<DynamicTableEntity>(new RangeFilter<PartitionKeyColumn>((RangeMinimumBoundary<PartitionKeyColumn>) null, (RangeMaximumBoundary<PartitionKeyColumn>) null), (StringColumnValue<RowKeyColumn>[]) new RowKeyColumnValue[2]
      {
        AzureTableBlobMetadataProvider.EmptyBlobMetaDataRowTableEntity.RowKeyColumnValueInstance,
        AzureTableBlobMetadataProvider.KeepUntilBlobReferenceRowTableEntity.RowKeyColumnValueInstance
      }, AzureTableBlobMetadataProvider.IdBlobReferenceRowTableEntity.RowRangeFilter);
      IConcurrentIterator<DynamicTableEntity> sortedTableEnumerator = this.TableClientFactory.GetAllTables().Select<ITable, IConcurrentIterator<DynamicTableEntity>>((Func<ITable, IConcurrentIterator<DynamicTableEntity>>) (table => table.QueryConcurrentIterator<DynamicTableEntity>(processor, (Query<DynamicTableEntity>) query))).CollectSortOrdered<DynamicTableEntity>(new int?(1000), (IComparer<DynamicTableEntity>) new AdminAzureTableBlobMetadataProvider.EntityComparer(), processor.CancellationToken);
      return (IConcurrentIterator<BlobReferenceDetailInfo>) new ConcurrentIterator<BlobReferenceDetailInfo>(new int?(1000), processor.CancellationToken, (Func<TryAddValueAsyncFunc<BlobReferenceDetailInfo>, CancellationToken, Task>) (async (valueAdderAsync, cancellationToken) =>
      {
        string lastPartitionKey = (string) null;
        BlobReferenceDetailInfo blobInfoAccumulator = new BlobReferenceDetailInfo();
        OperationContext context = new OperationContext();
        DynamicTableEntity row;
        while (true)
        {
          if (await sortedTableEnumerator.MoveNextAsync(cancellationToken).ConfigureAwait(false))
          {
            row = sortedTableEnumerator.Current;
            IDictionary<string, EntityProperty> properties = row.WriteEntity(context);
            if (!row.PartitionKey.Equals(lastPartitionKey, StringComparison.Ordinal))
            {
              int num = await valueAdderAsync(blobInfoAccumulator).ConfigureAwait(false) ? 1 : 0;
              blobInfoAccumulator = new BlobReferenceDetailInfo();
              lastPartitionKey = row.PartitionKey;
            }
            if (row.RowKey.Equals("keepUntil", StringComparison.Ordinal))
            {
              AzureTableBlobMetadataProvider.KeepUntilBlobReferenceRowTableEntity referenceRowTableEntity = new AzureTableBlobMetadataProvider.KeepUntilBlobReferenceRowTableEntity()
              {
                PartitionKey = row.PartitionKey,
                RowKey = row.RowKey
              };
              referenceRowTableEntity.ReadEntity(properties, context);
              blobInfoAccumulator.References.Add(new BlobReference(new KeepUntilBlobReference(referenceRowTableEntity.KeepUntil)));
            }
            else if (row.RowKey.Equals("metadata", StringComparison.Ordinal))
            {
              AzureTableBlobMetadataProvider.BlobMetaDataRowTableEntity dataRowTableEntity = new AzureTableBlobMetadataProvider.BlobMetaDataRowTableEntity()
              {
                PartitionKey = row.PartitionKey,
                RowKey = row.RowKey
              };
              dataRowTableEntity.ReadEntity(properties, context);
              blobInfoAccumulator.BlobId = dataRowTableEntity.BlobId;
              blobInfoAccumulator.StoredReferenceState = dataRowTableEntity.ReferenceState;
              blobInfoAccumulator.BlobAddedTime = dataRowTableEntity.BlobAddedTime;
              blobInfoAccumulator.BlobLength = dataRowTableEntity.BlobLength;
            }
            else if (row.RowKey.StartsWith("ref_", StringComparison.Ordinal))
            {
              AzureTableBlobMetadataProvider.IdBlobReferenceRowTableEntity referenceRowTableEntity = new AzureTableBlobMetadataProvider.IdBlobReferenceRowTableEntity()
              {
                PartitionKey = row.PartitionKey,
                RowKey = row.RowKey
              };
              referenceRowTableEntity.ReadEntity(properties, context);
              blobInfoAccumulator.References.Add(new BlobReference(new IdBlobReference(referenceRowTableEntity.ReferenceId, referenceRowTableEntity.ScopeId)));
            }
            else
              break;
            row = (DynamicTableEntity) null;
            properties = (IDictionary<string, EntityProperty>) null;
          }
          else
            goto label_13;
        }
        throw new InvalidOperationException("Unknown row: " + row.RowKey);
label_13:
        int num1 = await valueAdderAsync(blobInfoAccumulator).ConfigureAwait(false) ? 1 : 0;
        lastPartitionKey = (string) null;
        blobInfoAccumulator = (BlobReferenceDetailInfo) null;
        context = (OperationContext) null;
      }));
    }

    private IConcurrentIterator<IBlobMetadataProjectScopedSizeInfo> GetBlobMetadataProjectScopedConcurrentIterator(
      VssRequestPump.Processor processor,
      IConcurrentIterator<DynamicTableEntity> tableEnum,
      bool enableFeedInfoExport = false)
    {
      return (IConcurrentIterator<IBlobMetadataProjectScopedSizeInfo>) new ConcurrentIterator<IBlobMetadataProjectScopedSizeInfo>(new int?(1000), processor.CancellationToken, (Func<TryAddValueAsyncFunc<IBlobMetadataProjectScopedSizeInfo>, CancellationToken, Task>) (async (valueAdderAsync, cancellationToken) =>
      {
        string lastPartitionKey = (string) null;
        AzureTableBlobMetadataProvider.BlobMetadataProjectScopedSizeInfo blobInfo = (AzureTableBlobMetadataProvider.BlobMetadataProjectScopedSizeInfo) null;
        string blobMetadataPK = (string) null;
        OperationContext context = new OperationContext();
        DynamicTableEntity row;
        while (true)
        {
          if (await tableEnum.MoveNextAsync(cancellationToken).ConfigureAwait(false))
          {
            row = tableEnum.Current;
            IDictionary<string, EntityProperty> properties = row.WriteEntity(context);
            if (!row.PartitionKey.Equals(lastPartitionKey, StringComparison.Ordinal))
            {
              if (blobInfo != null)
              {
                int num = await valueAdderAsync((IBlobMetadataProjectScopedSizeInfo) blobInfo).ConfigureAwait(false) ? 1 : 0;
                blobInfo = (AzureTableBlobMetadataProvider.BlobMetadataProjectScopedSizeInfo) null;
                blobMetadataPK = (string) null;
              }
              lastPartitionKey = row.PartitionKey;
            }
            if (row.RowKey.Equals("keepUntil", StringComparison.Ordinal))
              new AzureTableBlobMetadataProvider.KeepUntilBlobReferenceRowTableEntity()
              {
                PartitionKey = row.PartitionKey,
                RowKey = row.RowKey
              }.ReadEntity(properties, context);
            else if (row.RowKey.Equals("metadata", StringComparison.Ordinal))
            {
              AzureTableBlobMetadataProvider.BlobMetaDataRowTableEntity metadataRow = new AzureTableBlobMetadataProvider.BlobMetaDataRowTableEntity()
              {
                PartitionKey = row.PartitionKey,
                RowKey = row.RowKey
              };
              metadataRow.ReadEntity(properties, context);
              blobInfo = new AzureTableBlobMetadataProvider.BlobMetadataProjectScopedSizeInfo(metadataRow);
              blobMetadataPK = metadataRow.PartitionKey;
            }
            else if (row.RowKey.StartsWith("ref_", StringComparison.Ordinal))
            {
              if (blobInfo != null && row.PartitionKey.Equals(blobMetadataPK, StringComparison.Ordinal))
              {
                AzureTableBlobMetadataProvider.IdBlobReferenceRowTableEntity referenceRowTableEntity = new AzureTableBlobMetadataProvider.IdBlobReferenceRowTableEntity()
                {
                  PartitionKey = row.PartitionKey,
                  RowKey = row.RowKey
                };
                referenceRowTableEntity.ReadEntity(properties, context);
                if (referenceRowTableEntity.ReferenceId.StartsWith("feed"))
                {
                  string[] source = referenceRowTableEntity.ReferenceId.Split('/');
                  if (((IEnumerable<string>) source).Count<string>() >= 2 && source[0].Equals("feed"))
                  {
                    string str = source[1];
                    blobInfo.IdReferenceCountByProject.AddOrUpdate(str, 1L, (Func<string, long, long>) ((key, cur) => cur + 1L));
                    if (enableFeedInfoExport)
                    {
                      int startIndex = "feed".Length + str.Length + 2;
                      string referenceIdDetails = referenceRowTableEntity.ReferenceId.Substring(startIndex);
                      blobInfo.ExportFeedInfo.Add(new ExportFeedInfo(row.PartitionKey, str, referenceIdDetails, new DateTimeOffset?((DateTimeOffset) row.Timestamp.UtcDateTime)));
                    }
                  }
                }
              }
            }
            else
              break;
            row = (DynamicTableEntity) null;
            properties = (IDictionary<string, EntityProperty>) null;
          }
          else
            goto label_19;
        }
        throw new InvalidOperationException("Unknown row: " + row.RowKey);
label_19:
        if (blobInfo == null)
        {
          lastPartitionKey = (string) null;
          blobInfo = (AzureTableBlobMetadataProvider.BlobMetadataProjectScopedSizeInfo) null;
          blobMetadataPK = (string) null;
          context = (OperationContext) null;
        }
        else
        {
          int num = await valueAdderAsync((IBlobMetadataProjectScopedSizeInfo) blobInfo).ConfigureAwait(false) ? 1 : 0;
          lastPartitionKey = (string) null;
          blobInfo = (AzureTableBlobMetadataProvider.BlobMetadataProjectScopedSizeInfo) null;
          blobMetadataPK = (string) null;
          context = (OperationContext) null;
        }
      }));
    }

    public IConcurrentIterator<BlobIdentifier> GetAllBlobIdentifiersConcurrentIterator(
      VssRequestPump.Processor processor)
    {
      return this.QueryAllShardsOrdered<AzureTableBlobMetadataProvider.BlobMetaDataRowTableEntity>(processor, BlobIdentifier.MinValue, (BlobIdentifier) null, (IteratorPartition) null, (IFilter<UserColumn>) null).Select<AzureTableBlobMetadataProvider.BlobMetaDataRowTableEntity, BlobIdentifier>((Func<AzureTableBlobMetadataProvider.BlobMetaDataRowTableEntity, BlobIdentifier>) (e => e.BlobId));
    }

    public IConcurrentIterator<BlobReferenceDetailInfo> GetAllReferencesConcurrentIterator(
      VssRequestPump.Processor processor)
    {
      return this.GetBlobBlobReferenceDetailInfoConcurrentIterator(processor);
    }

    private IConcurrentIterator<T>[] QueryEachShard<T>(
      VssRequestPump.Processor processor,
      BlobIdentifier startingBlobId,
      BlobIdentifier endingBlobId,
      IteratorPartition partition,
      IFilter<UserColumn> filter)
      where T : AzureTableBlobMetadataProvider.EmptyBlobRowTableEntity, ITableEntityConstantRowKey, new()
    {
      PartitionRangeExactRowQuery<T> query = PartitionRangeExactRowQuery<T>.Create<T>(new RangeFilter<PartitionKeyColumn>(new RangeMinimumBoundary<PartitionKeyColumn>((IColumnValue<PartitionKeyColumn>) new PartitionKeyColumnValue(startingBlobId.ValueString), RangeBoundaryType.Inclusive), endingBlobId == (BlobIdentifier) null ? (RangeMaximumBoundary<PartitionKeyColumn>) null : new RangeMaximumBoundary<PartitionKeyColumn>((IColumnValue<PartitionKeyColumn>) new PartitionKeyColumnValue(endingBlobId.ValueString), RangeBoundaryType.Exclusive)), userColumnFilter: filter);
      IEnumerable<ITable> allTables = this.TableClientFactory.GetAllTables();
      return (partition == null ? allTables : partition.SelectIterators<ITable>(allTables)).Select<ITable, IConcurrentIterator<T>>((Func<ITable, IConcurrentIterator<T>>) (table => table.QueryConcurrentIterator<T>(processor, (Query<T>) query))).ToArray<IConcurrentIterator<T>>();
    }

    private IConcurrentIterator<T> QueryAllShardsOrdered<T>(
      VssRequestPump.Processor processor,
      BlobIdentifier startingBlobId,
      BlobIdentifier endingBlobId,
      IteratorPartition partition,
      IFilter<UserColumn> filter)
      where T : AzureTableBlobMetadataProvider.EmptyBlobRowTableEntity, ITableEntityConstantRowKey, new()
    {
      IConcurrentIterator<T>[] sourceEnumerators = this.QueryEachShard<T>(processor, startingBlobId, endingBlobId, partition, filter);
      return ((IEnumerable<IConcurrentIterator<T>>) sourceEnumerators).CollectSortOrdered<T>(new int?(sourceEnumerators.Length * 1000), (IComparer<T>) new AdminAzureTableBlobMetadataProvider.EntityComparer(), processor.CancellationToken);
    }

    private class EntityComparer : IComparer<ITableEntity>
    {
      public int Compare(ITableEntity x, ITableEntity y) => x.PartitionKey == y.PartitionKey ? string.CompareOrdinal(x.RowKey, y.RowKey) : string.CompareOrdinal(x.PartitionKey, y.PartitionKey);
    }
  }
}
