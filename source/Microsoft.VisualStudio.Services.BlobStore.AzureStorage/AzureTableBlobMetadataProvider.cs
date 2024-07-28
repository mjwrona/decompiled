// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.AzureStorage.AzureTableBlobMetadataProvider
// Assembly: Microsoft.VisualStudio.Services.BlobStore.AzureStorage, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8BF1D977-E244-4825-BEA6-8BA4E1DDDB84
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.AzureStorage.dll

using BuildXL.Cache.ContentStore.Hashing;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.VisualStudio.Services.BlobStore.Common;
using Microsoft.VisualStudio.Services.BlobStore.Server.Common;
using Microsoft.VisualStudio.Services.Content.Common;
using Microsoft.VisualStudio.Services.Content.Server.Azure;
using Microsoft.VisualStudio.Services.Content.Server.Common;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace Microsoft.VisualStudio.Services.BlobStore.AzureStorage
{
  public class AzureTableBlobMetadataProvider : IBlobMetadataProvider, IDisposable
  {
    protected internal readonly ITableClientFactory TableClientFactory;
    private readonly int ExistsRows;
    private readonly int MaxBatchSize;

    public AzureTableBlobMetadataProvider(
      ITableClientFactory tableClientFactory,
      AzureTableBlobMetadataProviderOptions options)
    {
      this.TableClientFactory = tableClientFactory;
      this.ExistsRows = options.TotalExistsRows;
      this.MaxBatchSize = options.MaxBatchSize;
    }

    public TimeSpan MaxExpectedClockSkew => UpdateKeepUntilReferenceHelper.ClockSkewBuffer;

    public async Task<IBlobMetadata> GetAsync(
      VssRequestPump.Processor processor,
      BlobIdentifier blobId)
    {
      ITable tableShard = this.GetTableShard(blobId);
      PartitionScanQuery<DynamicTableEntity> partitionScanQuery1 = new PartitionScanQuery<DynamicTableEntity>((StringColumnValue<PartitionKeyColumn>) new AzureTableBlobMetadataProvider.BlobIdPartitionKeyColumnValue(blobId), maxRowsToTake: new int?(1 + this.ExistsRows + 1 + 1));
      bool hasIdReferences = false;
      AzureTableBlobMetadataProvider.BlobMetaDataRowTableEntity metadataRow = (AzureTableBlobMetadataProvider.BlobMetaDataRowTableEntity) null;
      AzureTableBlobMetadataProvider.KeepUntilBlobReferenceRowTableEntity keepUntilRow = (AzureTableBlobMetadataProvider.KeepUntilBlobReferenceRowTableEntity) null;
      List<AzureTableBlobMetadataProvider.EmptyBlobExistsRowTableEntity> existsRows = new List<AzureTableBlobMetadataProvider.EmptyBlobExistsRowTableEntity>();
      OperationContext context = new OperationContext();
      VssRequestPump.Processor processor1 = processor;
      PartitionScanQuery<DynamicTableEntity> partitionScanQuery2 = partitionScanQuery1;
      Func<IReadOnlyList<DynamicTableEntity>, bool> segmentCallback = (Func<IReadOnlyList<DynamicTableEntity>, bool>) (segment =>
      {
        foreach (DynamicTableEntity dynamicTableEntity in (IEnumerable<DynamicTableEntity>) segment)
        {
          IDictionary<string, EntityProperty> properties = dynamicTableEntity.WriteEntity(context);
          if (dynamicTableEntity.RowKey.Equals("metadata", StringComparison.Ordinal))
          {
            metadataRow = new AzureTableBlobMetadataProvider.BlobMetaDataRowTableEntity()
            {
              PartitionKey = dynamicTableEntity.PartitionKey,
              RowKey = dynamicTableEntity.RowKey,
              ETag = dynamicTableEntity.ETag,
              Timestamp = dynamicTableEntity.Timestamp
            };
            metadataRow.ReadEntity(properties, context);
          }
          else if (dynamicTableEntity.RowKey.Equals("keepUntil", StringComparison.Ordinal))
          {
            keepUntilRow = new AzureTableBlobMetadataProvider.KeepUntilBlobReferenceRowTableEntity()
            {
              PartitionKey = dynamicTableEntity.PartitionKey,
              RowKey = dynamicTableEntity.RowKey,
              ETag = dynamicTableEntity.ETag,
              Timestamp = dynamicTableEntity.Timestamp
            };
            keepUntilRow.ReadEntity(properties, context);
          }
          else if (dynamicTableEntity.RowKey.StartsWith("ref_", StringComparison.Ordinal))
          {
            hasIdReferences = true;
          }
          else
          {
            if (!dynamicTableEntity.RowKey.EndsWith("_exists", StringComparison.Ordinal))
              throw new InvalidOperationException("Unknown row: " + dynamicTableEntity.RowKey);
            AzureTableBlobMetadataProvider.EmptyBlobExistsRowTableEntity existsRowTableEntity = new AzureTableBlobMetadataProvider.EmptyBlobExistsRowTableEntity()
            {
              PartitionKey = dynamicTableEntity.PartitionKey,
              RowKey = dynamicTableEntity.RowKey,
              ETag = dynamicTableEntity.ETag,
              Timestamp = dynamicTableEntity.Timestamp
            };
            existsRowTableEntity.ReadEntity(properties, context);
            existsRows.Add(existsRowTableEntity);
          }
        }
        return true;
      });
      await tableShard.IterateOverQueryAsync<DynamicTableEntity>(processor1, (Query<DynamicTableEntity>) partitionScanQuery2, segmentCallback).ConfigureAwait(false);
      return metadataRow != null ? (IBlobMetadata) new AzureTableBlobMetadataProvider.BlobMetadata(metadataRow, existsRows, keepUntilRow, hasIdReferences) : (IBlobMetadata) new AzureTableBlobMetadataProvider.BlobMetadata(blobId);
    }

    public async Task<bool> TryDeleteAsync(
      VssRequestPump.Processor processor,
      IBlobMetadata metadataToDelete)
    {
      AzureTableBlobMetadataProvider.BlobMetaDataRowTableEntity entity = new AzureTableBlobMetadataProvider.BlobMetaDataRowTableEntity(metadataToDelete.BlobId, metadataToDelete.StoredReferenceState, metadataToDelete.BlobEtag, metadataToDelete.MetadataEtag, metadataToDelete.BlobLength, metadataToDelete.BlobAddedTime);
      if (entity.ReferenceState != BlobReferenceState.DeletingBlob)
        throw new ArgumentException("Metadata must be in state DeletingBlob");
      ITable table = this.GetTableShard(metadataToDelete.BlobId);
      HttpStatusCode httpStatusCode = (await table.ExecuteAsync(processor, TableOperationDescriptor.Delete((ITableEntity) entity)).ConfigureAwait(false)).HttpStatusCode;
      switch (httpStatusCode)
      {
        case HttpStatusCode.NoContent:
          return true;
        case HttpStatusCode.NotFound:
        case HttpStatusCode.Conflict:
        case HttpStatusCode.PreconditionFailed:
          return false;
        default:
          throw new ExpandedTableStorageException(string.Format("Failed to delete metadata row: {0}", (object) httpStatusCode), table?.StorageAccountName);
      }
    }

    public async Task<bool> TryUpdateAsync(
      VssRequestPump.Processor processor,
      IBlobMetadata updatedMetadata)
    {
      BlobReferenceStateHelper.AssertValidTransition(updatedMetadata);
      ITable table = this.GetTableShard(updatedMetadata.BlobId);
      AzureTableBlobMetadataProvider.BlobMetaDataRowTableEntity entity = new AzureTableBlobMetadataProvider.BlobMetaDataRowTableEntity(updatedMetadata.BlobId, updatedMetadata.DesiredReferenceState, updatedMetadata.BlobEtag, updatedMetadata.MetadataEtag, updatedMetadata.BlobLength, updatedMetadata.BlobAddedTime);
      TableBatchOperationDescriptor batchOperation = new TableBatchOperationDescriptor();
      if (updatedMetadata.StoredReferenceState == BlobReferenceState.Empty)
        batchOperation.Insert((ITableEntity) entity);
      else
        batchOperation.Merge((ITableEntity) entity);
      if (updatedMetadata.StoredReferenceState == BlobReferenceState.AddingBlob && updatedMetadata.DesiredReferenceState == BlobReferenceState.AddedBlob)
        throw new InvalidOperationException("Transition from Adding to Added should be via TryReferenceWithBlobAsync.");
      if (updatedMetadata.StoredReferenceState == BlobReferenceState.AddedBlob && updatedMetadata.DesiredReferenceState == BlobReferenceState.DeletingBlob)
      {
        AzureTableBlobMetadataProvider.BlobMetadata blobMetadata = (AzureTableBlobMetadataProvider.BlobMetadata) updatedMetadata;
        if (blobMetadata.ExistsRows == null)
          throw new InvalidOperationException("Cached metadata does not have any of the exists rows");
        if (blobMetadata.ExistsRows.Count != this.ExistsRows)
          return false;
        foreach (AzureTableBlobMetadataProvider.EmptyBlobExistsRowTableEntity existsRow in blobMetadata.ExistsRows)
          batchOperation.Delete((ITableEntity) existsRow);
        if (blobMetadata.KeepUntilRow != null)
          batchOperation.Delete((ITableEntity) blobMetadata.KeepUntilRow);
      }
      return (await this.RunBatchOperationAsync(processor, table, updatedMetadata.BlobId, batchOperation).ConfigureAwait(false)).Match<bool>((Func<IList<TableOperationResult>, bool>) (success => true), (Func<TableBatchOperationResult.Error, bool>) (error =>
      {
        switch (error.StatusCode)
        {
          case HttpStatusCode.NotFound:
          case HttpStatusCode.Conflict:
          case HttpStatusCode.PreconditionFailed:
            return false;
          default:
            throw new ExpandedTableStorageException(string.Format("Unexpected response from TryUpdate: {0}", (object) error.StatusCode), error.Exception, table?.StorageAccountName);
        }
      }));
    }

    public virtual bool HasReferences(IBlobMetadata metadata) => this.HasReferencesAtTime(metadata, DateTime.UtcNow);

    protected bool HasReferencesAtTime(IBlobMetadata metadata, DateTime timeReference)
    {
      AzureTableBlobMetadataProvider.BlobMetadata blobMetadata = (AzureTableBlobMetadataProvider.BlobMetadata) metadata;
      if (blobMetadata.HasIdReferences)
        return true;
      return blobMetadata.KeepUntilRow != null && blobMetadata.KeepUntilRow.KeepUntil > timeReference;
    }

    public Task<IEnumerable<ReferenceResult>> TryReferenceWithBlobAsync(
      VssRequestPump.Processor processor,
      IBlobMetadata metadata,
      IEnumerable<BlobReference> references)
    {
      BlobReferenceStateHelper.AssertValidTransition(metadata);
      if (metadata.StoredReferenceState == BlobReferenceState.AddedBlob)
        return this.TryReferenceAsyncInternal(processor, metadata.BlobId, references, (IBlobMetadata) null);
      if (metadata.StoredReferenceState == BlobReferenceState.AddingBlob)
        return this.TryReferenceAsyncInternal(processor, metadata.BlobId, references, metadata);
      throw new InvalidOperationException();
    }

    public Task<IEnumerable<ReferenceResult>> TryReferenceAsync(
      VssRequestPump.Processor processor,
      BlobIdentifier blobId,
      IEnumerable<BlobReference> references)
    {
      return this.TryReferenceAsyncInternal(processor, blobId, references, (IBlobMetadata) null);
    }

    public async Task<IEnumerable<RemoveReferenceResult>> RemoveReferencesAsync(
      VssRequestPump.Processor processor,
      BlobIdentifier blobId,
      IEnumerable<IdBlobReference> referencesEnumerable)
    {
      List<IdBlobReference> list1 = referencesEnumerable.ToList<IdBlobReference>();
      if (list1.Count == 0)
        return (IEnumerable<RemoveReferenceResult>) Array.Empty<RemoveReferenceResult>();
      ITable table = this.GetTableShard(blobId);
      List<IdBlobReference> referencesToDelete = new List<IdBlobReference>(list1.Count);
      Func<IReadOnlyList<AzureTableBlobMetadataProvider.IdBlobReferenceRowTableEntity>, bool> func;
      Func<IdBlobReference, Task> action1 = (Func<IdBlobReference, Task>) (async reference =>
      {
        PointQuery<AzureTableBlobMetadataProvider.IdBlobReferenceRowTableEntity> pointQuery = new PointQuery<AzureTableBlobMetadataProvider.IdBlobReferenceRowTableEntity>((StringColumnValue<PartitionKeyColumn>) new AzureTableBlobMetadataProvider.BlobIdPartitionKeyColumnValue(blobId), (StringColumnValue<RowKeyColumn>) new RowKeyColumnValue(AzureTableBlobMetadataProvider.IdBlobReferenceRowTableEntity.FormatRowKey(reference)));
        await table.IterateOverQueryAsync<AzureTableBlobMetadataProvider.IdBlobReferenceRowTableEntity>(processor, (Query<AzureTableBlobMetadataProvider.IdBlobReferenceRowTableEntity>) pointQuery, func ?? (func = (Func<IReadOnlyList<AzureTableBlobMetadataProvider.IdBlobReferenceRowTableEntity>, bool>) (segment =>
        {
          lock (referencesToDelete)
            referencesToDelete.AddRange(segment.Select<AzureTableBlobMetadataProvider.IdBlobReferenceRowTableEntity, IdBlobReference>((Func<AzureTableBlobMetadataProvider.IdBlobReferenceRowTableEntity, IdBlobReference>) (r => r.ReferenceIdentifier)));
          return true;
        }))).ConfigureAwait(false);
      });
      ExecutionDataflowBlockOptions dataflowBlockOptions1 = new ExecutionDataflowBlockOptions();
      dataflowBlockOptions1.MaxDegreeOfParallelism = Environment.ProcessorCount;
      dataflowBlockOptions1.CancellationToken = processor.CancellationToken;
      dataflowBlockOptions1.EnsureOrdered = false;
      await NonSwallowingActionBlock.Create<IdBlobReference>(action1, dataflowBlockOptions1).PostAllToUnboundedAndCompleteAsync<IdBlobReference>((IEnumerable<IdBlobReference>) list1, processor.CancellationToken).ConfigureAwait(false);
      List<IdBlobReference> source = referencesToDelete;
      List<RemoveReferenceResult> results = new List<RemoveReferenceResult>(source.Count);
      Func<List<IdBlobReference>, Task> action2 = (Func<List<IdBlobReference>, Task>) (async referencePage =>
      {
        TableBatchOperationDescriptor batchOperation = new TableBatchOperationDescriptor();
        foreach (IdBlobReference referenceId in referencePage)
          batchOperation.Add(TableOperationDescriptor.Delete((ITableEntity) new AzureTableBlobMetadataProvider.IdBlobReferenceRowTableEntity(blobId, referenceId, "*")));
        await (await this.RunBatchOperationAsync(processor, table, blobId, batchOperation).ConfigureAwait(false)).Match<Task>((Func<IList<TableOperationResult>, Task>) (_success =>
        {
          lock (results)
            results.AddRange(referencePage.Select<IdBlobReference, RemoveReferenceResult>((Func<IdBlobReference, RemoveReferenceResult>) (r => new RemoveReferenceResult(r, true))));
          return (Task) Task.FromResult<int>(0);
        }), (Func<TableBatchOperationResult.Error, Task>) (async error =>
        {
          if (error.StatusCode != HttpStatusCode.NotFound || !error.FailedOperationIndex.HasValue)
            throw new ExpandedTableStorageException(string.Format("Unexpected response from RemoveReference: {0}", (object) error.StatusCode), error.Exception, table?.StorageAccountName);
          int num = error.FailedOperationIndex.Value;
          lock (results)
            results.Add(new RemoveReferenceResult(referencePage[num], false));
          IList<IdBlobReference> list6 = (IList<IdBlobReference>) referencePage.Take<IdBlobReference>(num).ToList<IdBlobReference>();
          IList<IdBlobReference> list7 = (IList<IdBlobReference>) referencePage.Skip<IdBlobReference>(num + 1).ToList<IdBlobReference>();
          IList<IdBlobReference> list8 = (IList<IdBlobReference>) list7.Take<IdBlobReference>(list7.Count / 2).ToList<IdBlobReference>();
          IList<IdBlobReference> list9 = (IList<IdBlobReference>) list7.Skip<IdBlobReference>(list7.Count / 2).ToList<IdBlobReference>();
          Task<IEnumerable<RemoveReferenceResult>>[] taskArray = new Task<IEnumerable<RemoveReferenceResult>>[3]
          {
            this.RemoveReferencesAsync(processor, blobId, (IEnumerable<IdBlobReference>) list6),
            this.RemoveReferencesAsync(processor, blobId, (IEnumerable<IdBlobReference>) list8),
            this.RemoveReferencesAsync(processor, blobId, (IEnumerable<IdBlobReference>) list9)
          };
          foreach (IEnumerable<RemoveReferenceResult> collection in await Task.WhenAll<IEnumerable<RemoveReferenceResult>>(taskArray).ConfigureAwait(false))
          {
            lock (results)
              results.AddRange(collection);
          }
        })).ConfigureAwait(false);
      });
      ExecutionDataflowBlockOptions dataflowBlockOptions2 = new ExecutionDataflowBlockOptions();
      dataflowBlockOptions2.MaxDegreeOfParallelism = Environment.ProcessorCount;
      dataflowBlockOptions2.CancellationToken = processor.CancellationToken;
      dataflowBlockOptions2.EnsureOrdered = false;
      await NonSwallowingActionBlock.Create<List<IdBlobReference>>(action2, dataflowBlockOptions2).PostAllToUnboundedAndCompleteAsync<List<IdBlobReference>>(source.GetPages<IdBlobReference>(this.GetMaxReferenceChangesPerUpdate(0)), processor.CancellationToken).ConfigureAwait(false);
      return (IEnumerable<RemoveReferenceResult>) results;
    }

    protected ITable GetTableShard(BlobIdentifier blobId) => this.TableClientFactory.GetTable(blobId.ValueString);

    protected Task<IEnumerable<IdBlobReference>> GetIdReferencesInternalAsync(
      VssRequestPump.Processor processor,
      BlobIdentifier blobId,
      int? maxToTake)
    {
      ITable tableShard = this.GetTableShard(blobId);
      return this.GetIdReferencesInternalAsync(processor, tableShard, blobId, maxToTake);
    }

    protected async Task<IEnumerable<IdBlobReference>> GetIdReferencesInternalAsync(
      VssRequestPump.Processor processor,
      ITable table,
      BlobIdentifier blobId,
      int? maxToTake)
    {
      return (IEnumerable<IdBlobReference>) await table.QueryConcurrentIterator<AzureTableBlobMetadataProvider.IdBlobReferenceRowTableEntity>(processor, (Query<AzureTableBlobMetadataProvider.IdBlobReferenceRowTableEntity>) new RowRangeQuery<AzureTableBlobMetadataProvider.IdBlobReferenceRowTableEntity>((StringColumnValue<PartitionKeyColumn>) new AzureTableBlobMetadataProvider.BlobIdPartitionKeyColumnValue(blobId), AzureTableBlobMetadataProvider.IdBlobReferenceRowTableEntity.RowRangeFilter, maxRowsToTake: maxToTake)).Select<AzureTableBlobMetadataProvider.IdBlobReferenceRowTableEntity, IdBlobReference>((Func<AzureTableBlobMetadataProvider.IdBlobReferenceRowTableEntity, IdBlobReference>) (row => row.ReferenceIdentifier)).ToListAsync<IdBlobReference>(CancellationToken.None).ConfigureAwait(false);
    }

    private int GetMaxReferenceChangesPerUpdate(int existsRowsUsed) => this.MaxBatchSize - existsRowsUsed;

    private static async Task<TableBatchOperationResult> CreateTableWhileNotFoundExceptionAsync(
      VssRequestPump.Processor processor,
      ITable table,
      Func<ITable, Task<TableBatchOperationResult>> tableAction)
    {
      TableBatchOperationResult result = await tableAction(table).ConfigureAwait(false);
      await result.OnErrorAsync((Func<TableBatchOperationResult.Error, Task>) (async error =>
      {
        if (error.StatusCode != HttpStatusCode.NotFound || !(error.ErrorCode == "TableNotFound"))
          return;
        int num = await table.CreateIfNotExistsAsync(processor).ConfigureAwait(false) ? 1 : 0;
        result = await tableAction(table).ConfigureAwait(false);
      })).ConfigureAwait(false);
      return result;
    }

    private async Task<AzureTableBlobMetadataProvider.ReferenceBatchOperationResult> TryReferenceAsyncInternalRunBatch(
      VssRequestPump.Processor processor,
      BlobIdentifier blobId,
      IReadOnlyDictionary<BlobReference, TableOperationDescriptor> refsInBatch,
      IBlobMetadata metadataToAddBlob)
    {
      TableBatchOperationDescriptor batchOperation = new TableBatchOperationDescriptor();
      foreach (TableOperationDescriptor operationDescriptor in refsInBatch.Values)
        batchOperation.Add(operationDescriptor);
      ITable tableShard = this.GetTableShard(blobId);
      int indexOfMetadataOperation = batchOperation.Count - 1 + 1;
      if (metadataToAddBlob == null)
      {
        batchOperation.Merge((ITableEntity) new AzureTableBlobMetadataProvider.EmptyBlobExistsRowTableEntity(blobId, "*", ThreadLocalRandom.Generator.Next(this.ExistsRows), this.ExistsRows));
        return (await this.RunBatchOperationAsync(processor, tableShard, blobId, batchOperation).ConfigureAwait(false)).Match<AzureTableBlobMetadataProvider.ReferenceBatchOperationResult>((Func<IList<TableOperationResult>, AzureTableBlobMetadataProvider.ReferenceBatchOperationResult>) (success => new AzureTableBlobMetadataProvider.ReferenceBatchOperationResult(true)), (Func<TableBatchOperationResult.Error, AzureTableBlobMetadataProvider.ReferenceBatchOperationResult>) (error =>
        {
          if (!error.FailedOperationIndex.HasValue)
          {
            if (error.StatusCode == HttpStatusCode.NotFound)
              return new AzureTableBlobMetadataProvider.ReferenceBatchOperationResult(false);
          }
          else if (error.FailedOperationIndex.Value == indexOfMetadataOperation && error.StatusCode == HttpStatusCode.NotFound)
            return new AzureTableBlobMetadataProvider.ReferenceBatchOperationResult(false);
          return new AzureTableBlobMetadataProvider.ReferenceBatchOperationResult(error);
        }));
      }
      batchOperation.Merge((ITableEntity) new AzureTableBlobMetadataProvider.BlobMetaDataRowTableEntity(metadataToAddBlob.BlobId, BlobReferenceState.AddedBlob, metadataToAddBlob.BlobEtag, metadataToAddBlob.MetadataEtag, metadataToAddBlob.BlobLength, metadataToAddBlob.BlobAddedTime));
      for (int existsRowIndex = 0; existsRowIndex < this.ExistsRows; ++existsRowIndex)
        batchOperation.Insert((ITableEntity) new AzureTableBlobMetadataProvider.EmptyBlobExistsRowTableEntity(metadataToAddBlob.BlobId, (string) null, existsRowIndex, this.ExistsRows));
      return (await this.RunBatchOperationAsync(processor, tableShard, blobId, batchOperation).ConfigureAwait(false)).Match<AzureTableBlobMetadataProvider.ReferenceBatchOperationResult>((Func<IList<TableOperationResult>, AzureTableBlobMetadataProvider.ReferenceBatchOperationResult>) (success => new AzureTableBlobMetadataProvider.ReferenceBatchOperationResult(true)), (Func<TableBatchOperationResult.Error, AzureTableBlobMetadataProvider.ReferenceBatchOperationResult>) (error =>
      {
        switch (error.StatusCode)
        {
          case HttpStatusCode.NotFound:
            return new AzureTableBlobMetadataProvider.ReferenceBatchOperationResult(false);
          case HttpStatusCode.Conflict:
            int? failedOperationIndex1 = error.FailedOperationIndex;
            int num1 = indexOfMetadataOperation;
            if (failedOperationIndex1.GetValueOrDefault() > num1 & failedOperationIndex1.HasValue)
            {
              int? failedOperationIndex2 = error.FailedOperationIndex;
              int num2 = indexOfMetadataOperation + this.ExistsRows;
              if (failedOperationIndex2.GetValueOrDefault() <= num2 & failedOperationIndex2.HasValue)
                return new AzureTableBlobMetadataProvider.ReferenceBatchOperationResult(false);
              break;
            }
            break;
          case HttpStatusCode.PreconditionFailed:
            int? failedOperationIndex3 = error.FailedOperationIndex;
            int num3 = indexOfMetadataOperation;
            if (failedOperationIndex3.GetValueOrDefault() == num3 & failedOperationIndex3.HasValue)
              return new AzureTableBlobMetadataProvider.ReferenceBatchOperationResult(false);
            break;
        }
        return new AzureTableBlobMetadataProvider.ReferenceBatchOperationResult(error);
      }));
    }

    private Task<List<AzureTableBlobMetadataProvider.EmptyBlobExistsRowTableEntity>> GetExistsRowsAsync(
      VssRequestPump.Processor processor,
      BlobIdentifier blobId)
    {
      ITable tableShard = this.GetTableShard(blobId);
      RowRangeQuery<AzureTableBlobMetadataProvider.EmptyBlobExistsRowTableEntity> rowRangeQuery1 = new RowRangeQuery<AzureTableBlobMetadataProvider.EmptyBlobExistsRowTableEntity>((StringColumnValue<PartitionKeyColumn>) new AzureTableBlobMetadataProvider.BlobIdPartitionKeyColumnValue(blobId), AzureTableBlobMetadataProvider.EmptyBlobExistsRowTableEntity.RowRangeFilter);
      VssRequestPump.Processor processor1 = processor;
      RowRangeQuery<AzureTableBlobMetadataProvider.EmptyBlobExistsRowTableEntity> rowRangeQuery2 = rowRangeQuery1;
      return tableShard.QueryConcurrentIterator<AzureTableBlobMetadataProvider.EmptyBlobExistsRowTableEntity>(processor1, (Query<AzureTableBlobMetadataProvider.EmptyBlobExistsRowTableEntity>) rowRangeQuery2).ToListAsync<AzureTableBlobMetadataProvider.EmptyBlobExistsRowTableEntity>(CancellationToken.None);
    }

    private async Task<IEnumerable<ReferenceResult>> TryReferenceAsyncInternal(
      VssRequestPump.Processor processor,
      BlobIdentifier blobId,
      IEnumerable<BlobReference> references,
      IBlobMetadata metadataToAddBlob)
    {
      List<ReferenceResult> results = new List<ReferenceResult>();
      int maxExistsRowsUsed = metadataToAddBlob == null ? 1 : this.ExistsRows;
      Stopwatch totalLatencySw = Stopwatch.StartNew();
      int pages = 0;
      int refs = 0;
      KeepUntilBlobReference? nullable = ((IEnumerable<ITaggedUnion<IdBlobReference, KeepUntilBlobReference>>) references).SelectTwos<IdBlobReference, KeepUntilBlobReference>().NullableMaxOrNull<KeepUntilBlobReference>();
      if (nullable.HasValue)
      {
        ReferenceResult referenceResult = await this.TryReferenceAsyncInternalKeepUntil(processor, blobId, nullable.Value, metadataToAddBlob).ConfigureAwait(false);
        if (referenceResult.Success)
          metadataToAddBlob = (IBlobMetadata) null;
        results.Add(referenceResult);
      }
      foreach (IReadOnlyList<IdBlobReference> page in ((IEnumerable<ITaggedUnion<IdBlobReference, KeepUntilBlobReference>>) references).SelectOnes<IdBlobReference, KeepUntilBlobReference>().GetPages<IdBlobReference>(this.GetMaxReferenceChangesPerUpdate(maxExistsRowsUsed)))
      {
        IReadOnlyList<IdBlobReference> referencePage = page;
        if (results.Any<ReferenceResult>((Func<ReferenceResult, bool>) (r => !r.Success)))
        {
          results.AddRange(referencePage.Select<IdBlobReference, ReferenceResult>((Func<IdBlobReference, ReferenceResult>) (r => new ReferenceResult(new BlobReference(r), false, new DateTime?()))));
        }
        else
        {
          bool idRefSuccess = await this.TryReferenceAsyncInternalIdReferenceBatch(processor, blobId, (IEnumerable<IdBlobReference>) referencePage, metadataToAddBlob).ConfigureAwait(false);
          if (idRefSuccess)
            metadataToAddBlob = (IBlobMetadata) null;
          results.AddRange(referencePage.Select<IdBlobReference, ReferenceResult>((Func<IdBlobReference, ReferenceResult>) (r => new ReferenceResult(new BlobReference(r), idRefSuccess, new DateTime?()))));
          ++pages;
          refs += referencePage.Count;
          referencePage = (IReadOnlyList<IdBlobReference>) null;
        }
      }
      totalLatencySw.Stop();
      IEnumerable<ReferenceResult> referenceResults = (IEnumerable<ReferenceResult>) results;
      results = (List<ReferenceResult>) null;
      totalLatencySw = (Stopwatch) null;
      return referenceResults;
    }

    private async Task<ReferenceResult> TryReferenceAsyncInternalKeepUntil(
      VssRequestPump.Processor processor,
      BlobIdentifier blobId,
      KeepUntilBlobReference requestedKeepUntilRef,
      IBlobMetadata metadataToAddBlob)
    {
      bool? success = new bool?();
      int attempts = 0;
      DateTime keepUntilToSet;
      do
      {
        ++attempts;
        AzureTableBlobMetadataProvider.KeepUntilBlobReferenceRowTableEntity referenceRowTableEntity = await this.GetKeepUntilRowAsync(processor, blobId).ConfigureAwait(false);
        if (!UpdateKeepUntilReferenceHelper.TryGetNewKeepUntil(referenceRowTableEntity?.KeepUntil, requestedKeepUntilRef.KeepUntil, out keepUntilToSet))
        {
          success = new bool?(true);
          break;
        }
        KeepUntilBlobReference referenceId = new KeepUntilBlobReference(keepUntilToSet);
        Dictionary<BlobReference, TableOperationDescriptor> refsInBatch = new Dictionary<BlobReference, TableOperationDescriptor>();
        HttpStatusCode statusCodeToRetry;
        if (referenceRowTableEntity == null)
        {
          statusCodeToRetry = HttpStatusCode.Conflict;
          refsInBatch.Add(new BlobReference(requestedKeepUntilRef), TableOperationDescriptor.Insert((ITableEntity) new AzureTableBlobMetadataProvider.KeepUntilBlobReferenceRowTableEntity(blobId, referenceId, (string) null)));
        }
        else
        {
          statusCodeToRetry = HttpStatusCode.PreconditionFailed;
          refsInBatch.Add(new BlobReference(requestedKeepUntilRef), TableOperationDescriptor.Replace((ITableEntity) new AzureTableBlobMetadataProvider.KeepUntilBlobReferenceRowTableEntity(blobId, referenceId, referenceRowTableEntity.ETag)));
        }
        (await this.TryReferenceAsyncInternalRunBatch(processor, blobId, (IReadOnlyDictionary<BlobReference, TableOperationDescriptor>) refsInBatch, metadataToAddBlob).ConfigureAwait(false)).Match(closure_4 ?? (closure_4 = (Action<bool>) (referencesAdded => success = new bool?(referencesAdded))), (Action<TableBatchOperationResult.Error>) (error =>
        {
          int? failedOperationIndex = error.FailedOperationIndex;
          int num = 0;
          if (!(failedOperationIndex.GetValueOrDefault() == num & failedOperationIndex.HasValue) || error.StatusCode != statusCodeToRetry)
            throw new ExpandedStorageException(string.Format("Unexpected response from TryReferenceAsyncInternalKeepUntil: {0}", (object) error.StatusCode), (Exception) error.Exception, this.GetTableShard(blobId)?.StorageAccountName);
        }));
      }
      while (attempts < 10 && !success.HasValue);
      if (!success.HasValue)
        throw new TimeoutException("Exhausted retry on TryReferenceAsyncInternalKeepUntil");
      return new ReferenceResult(new BlobReference(requestedKeepUntilRef), success.Value, success.Value ? new DateTime?(keepUntilToSet) : new DateTime?());
    }

    private async Task<bool> TryReferenceAsyncInternalIdReferenceBatch(
      VssRequestPump.Processor processor,
      BlobIdentifier blobId,
      IEnumerable<IdBlobReference> referencePage,
      IBlobMetadata metadataToAddBlob)
    {
      Dictionary<BlobReference, TableOperationDescriptor> refsInBatch = new Dictionary<BlobReference, TableOperationDescriptor>();
      foreach (IdBlobReference idBlobReference in referencePage)
        refsInBatch.Add(new BlobReference(idBlobReference), TableOperationDescriptor.InsertOrMerge((ITableEntity) new AzureTableBlobMetadataProvider.IdBlobReferenceRowTableEntity(blobId, idBlobReference, (string) null)));
      return (await this.TryReferenceAsyncInternalRunBatch(processor, blobId, (IReadOnlyDictionary<BlobReference, TableOperationDescriptor>) refsInBatch, metadataToAddBlob).ConfigureAwait(false)).Match<bool>((Func<bool, bool>) (referencesAdded => referencesAdded), (Func<TableBatchOperationResult.Error, bool>) (error =>
      {
        throw new ExpandedStorageException(string.Format("Unexpected response from TryReference {0}", (object) error.StatusCode), (Exception) error.Exception, this.GetTableShard(blobId)?.StorageAccountName);
      }));
    }

    public async Task<KeepUntilBlobReference?> GetKeepUntilReferenceAsync(
      VssRequestPump.Processor processor,
      BlobIdentifier blobId)
    {
      AzureTableBlobMetadataProvider.KeepUntilBlobReferenceRowTableEntity referenceRowTableEntity = await this.GetKeepUntilRowAsync(processor, blobId).ConfigureAwait(false);
      return referenceRowTableEntity != null ? new KeepUntilBlobReference?(new KeepUntilBlobReference(referenceRowTableEntity.KeepUntil)) : new KeepUntilBlobReference?();
    }

    private async Task<AzureTableBlobMetadataProvider.KeepUntilBlobReferenceRowTableEntity> GetKeepUntilRowAsync(
      VssRequestPump.Processor processor,
      BlobIdentifier blobId)
    {
      ITable tableShard = this.GetTableShard(blobId);
      return await this.GetKeepUntilRowAsync(processor, tableShard, blobId).ConfigureAwait(false);
    }

    internal Task<AzureTableBlobMetadataProvider.KeepUntilBlobReferenceRowTableEntity> GetKeepUntilRowAsync(
      VssRequestPump.Processor processor,
      ITable table,
      BlobIdentifier blobId)
    {
      PointQuery<AzureTableBlobMetadataProvider.KeepUntilBlobReferenceRowTableEntity> pointQuery = new PointQuery<AzureTableBlobMetadataProvider.KeepUntilBlobReferenceRowTableEntity>((StringColumnValue<PartitionKeyColumn>) new AzureTableBlobMetadataProvider.BlobIdPartitionKeyColumnValue(blobId), (StringColumnValue<RowKeyColumn>) AzureTableBlobMetadataProvider.KeepUntilBlobReferenceRowTableEntity.RowKeyColumnValueInstance);
      return table.QuerySingleOrDefaultAsync<AzureTableBlobMetadataProvider.KeepUntilBlobReferenceRowTableEntity>(processor, (Query<AzureTableBlobMetadataProvider.KeepUntilBlobReferenceRowTableEntity>) pointQuery);
    }

    internal virtual Task<TableBatchOperationResult> RunBatchOperationAsync(
      VssRequestPump.Processor processor,
      ITable table,
      BlobIdentifier blobId,
      TableBatchOperationDescriptor batchOperation)
    {
      return AzureTableBlobMetadataProvider.CreateTableWhileNotFoundExceptionAsync(processor, table, (Func<ITable, Task<TableBatchOperationResult>>) (t => t.ExecuteBatchAsync(processor, batchOperation)));
    }

    protected virtual void Dispose(bool disposing) => this.TableClientFactory?.Dispose();

    public void Dispose() => this.Dispose(true);

    internal class ReferenceBatchOperationResult : Result<bool, TableBatchOperationResult.Error>
    {
      public ReferenceBatchOperationResult(bool referencesAdded)
        : base(referencesAdded)
      {
      }

      public ReferenceBatchOperationResult(TableBatchOperationResult.Error error)
        : base(error)
      {
      }
    }

    private class BlobMetadata : IBlobMetadata
    {
      public BlobMetadata(BlobIdentifier blobId)
      {
        this.BlobEtag = (string) null;
        this.BlobId = blobId;
        this.MetadataEtag = (string) null;
        this.StoredReferenceState = BlobReferenceState.Empty;
        this.DesiredReferenceState = this.StoredReferenceState;
        this.ExistsRows = (List<AzureTableBlobMetadataProvider.EmptyBlobExistsRowTableEntity>) null;
        this.KeepUntilRow = (AzureTableBlobMetadataProvider.KeepUntilBlobReferenceRowTableEntity) null;
        this.HasIdReferences = false;
      }

      public BlobMetadata(
        AzureTableBlobMetadataProvider.BlobMetaDataRowTableEntity metadataRow,
        List<AzureTableBlobMetadataProvider.EmptyBlobExistsRowTableEntity> existsRows,
        AzureTableBlobMetadataProvider.KeepUntilBlobReferenceRowTableEntity keepUntilRow,
        bool hasIdReferences)
      {
        this.BlobEtag = metadataRow.BlobEtag;
        this.BlobId = BlobIdentifier.Deserialize(metadataRow.PartitionKey);
        this.BlobLength = metadataRow.BlobLength;
        this.MetadataEtag = metadataRow.ETag;
        this.StoredReferenceState = metadataRow.ReferenceState;
        this.DesiredReferenceState = this.StoredReferenceState;
        this.ExistsRows = existsRows;
        this.KeepUntilRow = keepUntilRow;
        this.HasIdReferences = hasIdReferences;
      }

      public DateTimeOffset? BlobAddedTime { get; set; }

      public string BlobEtag { get; set; }

      public BlobIdentifier BlobId { get; private set; }

      public long? BlobLength { get; set; }

      public string MetadataEtag { get; private set; }

      public BlobReferenceState StoredReferenceState { get; set; }

      public BlobReferenceState DesiredReferenceState { get; set; }

      internal List<AzureTableBlobMetadataProvider.EmptyBlobExistsRowTableEntity> ExistsRows { get; set; }

      internal AzureTableBlobMetadataProvider.KeepUntilBlobReferenceRowTableEntity KeepUntilRow { get; set; }

      public bool HasIdReferences { get; set; }
    }

    public class BlobMetadataSizeInfo : IBlobMetadataSizeInfo
    {
      internal BlobMetadataSizeInfo(
        AzureTableBlobMetadataProvider.BlobMetaDataRowTableEntity metadataRow)
      {
        this.BlobId = metadataRow.BlobId;
        this.StoredReferenceState = metadataRow.ReferenceState;
        this.BlobAddedTime = metadataRow.BlobAddedTime;
        this.BlobLength = metadataRow.BlobLength;
        this.KeepUntilTime = new DateTimeOffset?();
        this.IdReferenceCount = 0L;
      }

      public BlobIdentifier BlobId { get; private set; }

      public BlobReferenceState StoredReferenceState { get; set; }

      public DateTimeOffset? BlobAddedTime { get; set; }

      public DateTimeOffset? KeepUntilTime { get; set; }

      public long? BlobLength { get; set; }

      public long IdReferenceCount { get; set; }

      public ConcurrentDictionary<ArtifactScopeType, long> IdReferenceCountByScope { get; set; } = new ConcurrentDictionary<ArtifactScopeType, long>();

      public ConcurrentDictionary<string, ulong> IdReferenceCountByFeed { get; set; } = new ConcurrentDictionary<string, ulong>();
    }

    public class BlobMetadataProjectScopedSizeInfo : IBlobMetadataProjectScopedSizeInfo
    {
      internal BlobMetadataProjectScopedSizeInfo(
        AzureTableBlobMetadataProvider.BlobMetaDataRowTableEntity metadataRow)
      {
        this.BlobId = metadataRow.BlobId;
        this.StoredReferenceState = metadataRow.ReferenceState;
        this.BlobLength = metadataRow.BlobLength;
      }

      public BlobIdentifier BlobId { get; set; }

      public BlobReferenceState StoredReferenceState { get; set; }

      public long? BlobLength { get; set; }

      public ConcurrentDictionary<string, long> IdReferenceCountByProject { get; set; } = new ConcurrentDictionary<string, long>();

      public ConcurrentBag<ExportFeedInfo> ExportFeedInfo { get; set; } = new ConcurrentBag<ExportFeedInfo>();
    }

    internal class BlobIdPartitionKeyColumnValue : PartitionKeyColumnValue
    {
      public BlobIdPartitionKeyColumnValue(BlobIdentifier blobId)
        : base(blobId.ValueString)
      {
      }
    }

    protected internal abstract class EmptyBlobRowTableEntity : TableEntity
    {
      protected EmptyBlobRowTableEntity()
      {
      }

      protected EmptyBlobRowTableEntity(BlobIdentifier blobId, string rowKey, string etagToMatch)
      {
        this.BlobId = blobId;
        this.PartitionKey = blobId.ValueString;
        this.RowKey = rowKey;
        this.ETag = etagToMatch;
      }

      [IgnoreProperty]
      public BlobIdentifier BlobId { get; private set; }

      public override void ReadEntity(
        IDictionary<string, EntityProperty> properties,
        OperationContext operationContext)
      {
        base.ReadEntity(properties, operationContext);
        this.BlobId = BlobIdentifier.Deserialize(this.PartitionKey);
      }

      protected static RangeFilter<RowKeyColumn> CreateHexRangeFilter(
        Func<string, RowKeyColumnValue> getColumnValue)
      {
        return new RangeFilter<RowKeyColumn>(new RangeMinimumBoundary<RowKeyColumn>((IColumnValue<RowKeyColumn>) getColumnValue("0"), RangeBoundaryType.Inclusive), new RangeMaximumBoundary<RowKeyColumn>((IColumnValue<RowKeyColumn>) getColumnValue("G"), RangeBoundaryType.Exclusive));
      }
    }

    internal class EmptyBlobMetaDataRowTableEntity : 
      AzureTableBlobMetadataProvider.EmptyBlobRowTableEntity
    {
      public const string RowKeyName = "metadata";
      public static readonly RowKeyColumnValue RowKeyColumnValueInstance = (RowKeyColumnValue) new AzureTableBlobMetadataProvider.EmptyBlobMetaDataRowTableEntity.MetadataRowKeyColumnValue();

      public EmptyBlobMetaDataRowTableEntity()
      {
      }

      public EmptyBlobMetaDataRowTableEntity(BlobIdentifier blobId, string etagToMatch)
        : base(blobId, "metadata", etagToMatch)
      {
      }

      private class MetadataRowKeyColumnValue : RowKeyColumnValue
      {
        public MetadataRowKeyColumnValue()
          : base("metadata")
        {
        }
      }
    }

    internal class EmptyBlobExistsRowTableEntity : 
      AzureTableBlobMetadataProvider.EmptyBlobRowTableEntity
    {
      public static readonly RangeFilter<RowKeyColumn> RowRangeFilter = AzureTableBlobMetadataProvider.EmptyBlobRowTableEntity.CreateHexRangeFilter((Func<string, RowKeyColumnValue>) (hexValue => (RowKeyColumnValue) new AzureTableBlobMetadataProvider.EmptyBlobExistsRowTableEntity.ExistsRowKeyColumnValue(hexValue)));

      public EmptyBlobExistsRowTableEntity()
      {
      }

      public EmptyBlobExistsRowTableEntity(
        BlobIdentifier blobId,
        string etagToMatch,
        int existsRowIndex,
        int existsRows)
        : base(blobId, AzureTableBlobMetadataProvider.EmptyBlobExistsRowTableEntity.FormatRowKey(existsRowIndex, existsRows), etagToMatch)
      {
      }

      private static string FormatRowKey(int existsRowIndex, int existsRows) => existsRowIndex >= 0 && existsRowIndex < existsRows ? string.Format("{0:X}_exists", (object) existsRowIndex) : throw new ArgumentException("Not a valid empty row index.");

      private class ExistsRowKeyColumnValue : RowKeyColumnValue
      {
        public ExistsRowKeyColumnValue(string hexValue)
          : base(string.Format("{0:X}_exists", (object) hexValue))
        {
        }
      }
    }

    internal class BlobMetaDataRowTableEntity : 
      AzureTableBlobMetadataProvider.EmptyBlobMetaDataRowTableEntity,
      ITableEntityConstantRowKey,
      ITableEntity
    {
      public BlobMetaDataRowTableEntity()
      {
        this.ReferenceState = BlobReferenceState.Unknown;
        this.BlobEtag = (string) null;
      }

      public BlobMetaDataRowTableEntity(
        BlobIdentifier blobId,
        BlobReferenceState referenceState,
        string blobEtag,
        string etagToMatch,
        long? blobLength,
        DateTimeOffset? blobAddedTime)
        : base(blobId, etagToMatch)
      {
        switch (referenceState)
        {
          case BlobReferenceState.AddedBlob:
          case BlobReferenceState.DeletingBlob:
            if (blobEtag == null)
              throw new ArgumentException("Added and Deleting states must have a blobEtag");
            break;
        }
        this.ReferenceState = referenceState;
        this.BlobEtag = blobEtag;
        this.BlobLength = blobLength;
        this.BlobAddedTime = blobAddedTime;
      }

      public string BlobEtag { get; set; }

      public long? BlobLength { get; set; }

      public DateTimeOffset? BlobAddedTime { get; set; }

      public RowKeyColumnValue DefinedRowKey => AzureTableBlobMetadataProvider.EmptyBlobMetaDataRowTableEntity.RowKeyColumnValueInstance;

      [IgnoreProperty]
      public BlobReferenceState ReferenceState { get; set; }

      public override void ReadEntity(
        IDictionary<string, EntityProperty> properties,
        OperationContext operationContext)
      {
        base.ReadEntity(properties, operationContext);
        BlobReferenceState result;
        if (!properties.ContainsKey("ReferenceState") || !System.Enum.TryParse<BlobReferenceState>(properties["ReferenceState"].StringValue, out result))
          throw new SerializationException("Could not deserialize ReferenceState");
        this.ReferenceState = result;
      }

      public override IDictionary<string, EntityProperty> WriteEntity(
        OperationContext operationContext)
      {
        IDictionary<string, EntityProperty> dictionary = base.WriteEntity(operationContext);
        dictionary.Add("ReferenceState", new EntityProperty(this.ReferenceState.ToString()));
        return dictionary;
      }

      public class ReferenceStateColumnValue : StringColumnValue<UserColumn>
      {
        public static readonly UserColumn ColumnInstance = new UserColumn("ReferenceState");

        public ReferenceStateColumnValue(BlobReferenceState value)
          : base(value.ToString())
        {
        }

        public override UserColumn Column => AzureTableBlobMetadataProvider.BlobMetaDataRowTableEntity.ReferenceStateColumnValue.ColumnInstance;
      }
    }

    internal class KeepUntilBlobReferenceRowTableEntity : 
      AzureTableBlobMetadataProvider.EmptyBlobRowTableEntity,
      ITableEntityConstantRowKey,
      ITableEntity
    {
      public static readonly RowKeyColumnValue RowKeyColumnValueInstance = (RowKeyColumnValue) new AzureTableBlobMetadataProvider.KeepUntilBlobReferenceRowTableEntity.KeepUntilRowKeyColumnValue();
      public const string RowKeyName = "keepUntil";

      public KeepUntilBlobReferenceRowTableEntity()
      {
      }

      public KeepUntilBlobReferenceRowTableEntity(
        BlobIdentifier blobId,
        KeepUntilBlobReference referenceId,
        string etagToMatch)
        : base(blobId, "keepUntil", etagToMatch)
      {
        this.KeepUntil = referenceId.KeepUntil;
      }

      public DateTime KeepUntil { get; set; }

      public RowKeyColumnValue DefinedRowKey => AzureTableBlobMetadataProvider.KeepUntilBlobReferenceRowTableEntity.RowKeyColumnValueInstance;

      private class KeepUntilRowKeyColumnValue : RowKeyColumnValue
      {
        public KeepUntilRowKeyColumnValue()
          : base("keepUntil")
        {
        }
      }
    }

    public class KeepUntilColumnValue : DateTimeColumnValue<UserColumn>
    {
      private static readonly UserColumn ColumnInstance = new UserColumn("KeepUntil");

      public KeepUntilColumnValue(DateTime date)
        : base(date)
      {
      }

      public override UserColumn Column => AzureTableBlobMetadataProvider.KeepUntilColumnValue.ColumnInstance;
    }

    internal class IdBlobReferenceRowTableEntity : 
      AzureTableBlobMetadataProvider.EmptyBlobRowTableEntity
    {
      public static readonly RangeFilter<RowKeyColumn> RowRangeFilter = AzureTableBlobMetadataProvider.EmptyBlobRowTableEntity.CreateHexRangeFilter((Func<string, RowKeyColumnValue>) (hexValue => (RowKeyColumnValue) new AzureTableBlobMetadataProvider.IdBlobReferenceRowTableEntity.ReferenceRowKeyColumnValue(hexValue)));
      public static RangeFilter<RowKeyColumn> AllRowsFilter = new RangeFilter<RowKeyColumn>(new RangeMinimumBoundary<RowKeyColumn>((IColumnValue<RowKeyColumn>) new RowKeyColumnValue("ref^"), RangeBoundaryType.Exclusive), new RangeMaximumBoundary<RowKeyColumn>((IColumnValue<RowKeyColumn>) new RowKeyColumnValue("ref`"), RangeBoundaryType.Exclusive));
      private const string RowKeyNameFormat = "ref_{0}";
      public const string RowKeyPrefix = "ref_";
      public const string ScopeIdColumnName = "ScopeId";

      public IdBlobReferenceRowTableEntity()
      {
      }

      public IdBlobReferenceRowTableEntity(
        BlobIdentifier blobId,
        IdBlobReference referenceId,
        string etagToMatch)
        : base(blobId, AzureTableBlobMetadataProvider.IdBlobReferenceRowTableEntity.FormatRowKey(referenceId), etagToMatch)
      {
        this.PartitionKey = blobId.ValueString;
        this.ReferenceIdentifier = referenceId;
      }

      [IgnoreProperty]
      public IdBlobReference ReferenceIdentifier
      {
        get => new IdBlobReference(this.ReferenceId, this.ScopeId);
        set
        {
          this.ReferenceId = value.Name;
          this.ScopeId = value.Scope;
        }
      }

      public string ReferenceId { get; set; }

      public string ScopeId { get; set; }

      public static string FormatRowKey(IdBlobReference referenceId) => string.Format("ref_{0}", (object) (referenceId.Name + referenceId.Scope).GetUTF8Bytes().CalculateBlockHash((IBlobHasher) Microsoft.VisualStudio.Services.BlobStore.Common.VsoHash.Instance).HashString);

      internal class ReferenceRowKeyColumnValue : RowKeyColumnValue
      {
        public ReferenceRowKeyColumnValue(string hexValue)
          : base(string.Format("ref_{0}", (object) hexValue))
        {
        }
      }

      internal class ScopeIdColumnValue : StringColumnValue<UserColumn>
      {
        private static readonly UserColumn ColumnInstance = new UserColumn("ScopeId");

        public ScopeIdColumnValue(string scope)
          : base(scope)
        {
        }

        public override UserColumn Column => AzureTableBlobMetadataProvider.IdBlobReferenceRowTableEntity.ScopeIdColumnValue.ColumnInstance;
      }
    }
  }
}
