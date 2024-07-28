// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.AzureStorage.AzureBlobTableDedupProvider
// Assembly: Microsoft.VisualStudio.Services.BlobStore.AzureStorage, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8BF1D977-E244-4825-BEA6-8BA4E1DDDB84
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.AzureStorage.dll

using BuildXL.Cache.ContentStore.Hashing;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.BlobStore.Common;
using Microsoft.VisualStudio.Services.BlobStore.Server;
using Microsoft.VisualStudio.Services.BlobStore.Server.Common;
using Microsoft.VisualStudio.Services.BlobStore.Server.Common.DedupGC;
using Microsoft.VisualStudio.Services.BlobStore.Server.Common.DedupGC.KeepUntil;
using Microsoft.VisualStudio.Services.BlobStore.Server.Common.Domains;
using Microsoft.VisualStudio.Services.BlobStore.WebApi;
using Microsoft.VisualStudio.Services.Content.Common;
using Microsoft.VisualStudio.Services.Content.Server.Azure;
using Microsoft.VisualStudio.Services.Content.Server.Common;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace Microsoft.VisualStudio.Services.BlobStore.AzureStorage
{
  public class AzureBlobTableDedupProvider : IDedupProvider
  {
    private readonly IClock clock;
    private readonly LocationMode? locationMode;
    internal const string c_keepUntilMetadataKey = "keepUntil";
    private const int MaxCompareSwapRetries = 5;
    private static readonly TimeSpan ChunkKeepUntilBonus = TimeSpan.FromDays(1.0);
    internal readonly IDomainProvider<IBlobContainerDomain> domainProvider;
    internal readonly ITableClientFactory tableClientFactory;
    private bool? blobRequiresVss;
    private readonly ContainerStorageAccessStatistics statistics;
    private bool isStatisticsEnabled;
    private bool isBlobOperationTimeoutEnabled;
    private List<ICloudBlobContainer> containers = new List<ICloudBlobContainer>();
    private const int MaxUndeleteAttempt = 5;

    private static TimeSpan GetKeepUntilBonus(DedupIdentifier id) => !ChunkerHelper.IsChunk(id.AlgorithmId) ? TimeSpan.Zero : AzureBlobTableDedupProvider.ChunkKeepUntilBonus;

    public List<ICloudBlobContainer> Containers => this.containers;

    public bool ProviderRequireVss => this.tableClientFactory.RequiresVssRequestContext || this.blobRequiresVss.Value;

    public AzureBlobTableDedupProvider(
      IEnumerable<ICloudBlobContainer> containers,
      LocationMode? locationMode,
      ITableClientFactory tableClientFactory,
      IClock clock,
      IDomainId domainId)
      : this(containers, tableClientFactory, locationMode, clock, domainId)
    {
      this.containers.AddRange(containers);
    }

    public AzureBlobTableDedupProvider(
      IDomainProvider<IBlobContainerDomain> domainProvider,
      bool blobRequiresVss,
      ITableClientFactory tableClientFactory,
      LocationMode? locationMode,
      IClock clock)
    {
      this.domainProvider = domainProvider;
      this.tableClientFactory = tableClientFactory;
      this.clock = clock;
      this.locationMode = new LocationMode?(locationMode.GetValueOrDefault());
      this.blobRequiresVss = new bool?(blobRequiresVss);
      this.statistics = new ContainerStorageAccessStatistics();
      this.isStatisticsEnabled = false;
      this.isBlobOperationTimeoutEnabled = false;
    }

    public AzureBlobTableDedupProvider(
      IEnumerable<ICloudBlobContainer> containers,
      ITableClientFactory tableClientFactory,
      LocationMode? locationMode,
      IClock clock,
      IDomainId domainId)
    {
      IEnumerable<BlobContainerNode> nodes = containers.Select<ICloudBlobContainer, BlobContainerNode>((Func<ICloudBlobContainer, BlobContainerNode>) (c => new BlobContainerNode(c)));
      this.domainProvider = (IDomainProvider<IBlobContainerDomain>) new DomainProvider<IBlobContainerDomain>(domainId, (IEnumerable<IBlobContainerDomain>) new ShardedBlobContainerDomain[1]
      {
        new ShardedBlobContainerDomain(domainId, (IShardManager<BlobContainerNode>) new ConsistentHashShardManager<BlobContainerNode>(nodes, 128))
      });
      this.tableClientFactory = tableClientFactory;
      this.clock = clock;
      this.locationMode = new LocationMode?(locationMode.GetValueOrDefault());
      this.blobRequiresVss = new bool?(containers.Any<ICloudBlobContainer>((Func<ICloudBlobContainer, bool>) (c => c.RequiresVssRequestContext)));
      this.statistics = new ContainerStorageAccessStatistics();
      this.isStatisticsEnabled = false;
      this.isBlobOperationTimeoutEnabled = false;
    }

    public async Task InitializeAsync(
      VssRequestPump.Processor processor,
      IDomainSecurityValidator validator)
    {
      LocationMode? locationMode1 = this.locationMode;
      LocationMode locationMode2 = LocationMode.SecondaryOnly;
      if (locationMode1.GetValueOrDefault() == locationMode2 & locationMode1.HasValue)
        return;
      ExecutionDataflowBlockOptions dataflowBlockOptions1 = new ExecutionDataflowBlockOptions();
      dataflowBlockOptions1.MaxDegreeOfParallelism = Environment.ProcessorCount;
      dataflowBlockOptions1.BoundedCapacity = 4 * Environment.ProcessorCount;
      dataflowBlockOptions1.CancellationToken = processor.CancellationToken;
      dataflowBlockOptions1.EnsureOrdered = false;
      ExecutionDataflowBlockOptions dataflowBlockOptions2 = dataflowBlockOptions1;
      List<ICloudBlobContainer> inputs = new List<ICloudBlobContainer>();
      foreach (IDomainId listDomain in this.domainProvider.ListDomains())
        inputs.AddRange(this.domainProvider.GetDomain((ISecuredDomainRequest) new SecuredDomainRequest(validator, listDomain)).GetAllContainers());
      int num1;
      ActionBlock<ICloudBlobContainer> actionBlock1 = NonSwallowingActionBlock.Create<ICloudBlobContainer>((Func<ICloudBlobContainer, Task>) (async container => num1 = await container.CreateIfNotExistsAsync(processor).ConfigureAwait(false) ? 1 : 0), dataflowBlockOptions2);
      int num2;
      ActionBlock<ITable> actionBlock2 = NonSwallowingActionBlock.Create<ITable>((Func<ITable, Task>) (async table => num2 = await table.CreateIfNotExistsAsync(processor).ConfigureAwait(false) ? 1 : 0), dataflowBlockOptions2);
      await Task.WhenAll(actionBlock1.SendAllAndCompleteSingleBlockNetworkAsync<ICloudBlobContainer>((IEnumerable<ICloudBlobContainer>) inputs, processor.CancellationToken), actionBlock2.SendAllAndCompleteSingleBlockNetworkAsync<ITable>(this.tableClientFactory.GetAllTables(), processor.CancellationToken)).ConfigureAwait(false);
    }

    public async Task<DedupCompressedBuffer> GetDedupAsync(
      VssRequestPump.SecuredDomainProcessor processor,
      DedupIdentifier dedupId)
    {
      ICloudBlockBlob blob = this.GetBlockBlobReference(processor.SecuredDomainRequest, dedupId);
      try
      {
        try
        {
          byte[] numArray = await blob.DownloadAsByteArrayAsync((VssRequestPump.Processor) processor).ConfigureAwait(false);
          return blob.IsCompressed() ? DedupCompressedBuffer.FromCompressed(numArray) : DedupCompressedBuffer.FromUncompressed(numArray);
        }
        catch (Microsoft.Azure.Storage.StorageException ex) when (
        {
          // ISSUE: unable to correctly present filter
          Microsoft.Azure.Storage.RequestResult requestInformation = ex.RequestInformation;
          if (requestInformation != null && requestInformation.HttpStatusCode == 404)
          {
            SuccessfulFiltering;
          }
          else
            throw;
        }
        )
        {
          return (DedupCompressedBuffer) null;
        }
      }
      catch (Microsoft.Azure.Storage.StorageException ex)
      {
        ICloudBlockBlob blob1 = blob;
        throw new ExpandedStorageException(ex, blob1);
      }
    }

    public async Task<bool> RestoreIfNotExists(
      VssRequestPump.SecuredDomainProcessor processor,
      DedupIdentifier dedupId)
    {
      ICloudBlockBlob blob = this.GetBlockBlobReference(processor.SecuredDomainRequest, dedupId);
      if (await blob.ExistsAsync((VssRequestPump.Processor) processor).ConfigureAwait(false))
        return true;
      for (int attempt = 0; attempt < 5 && !processor.CancellationToken.IsCancellationRequested; ++attempt)
      {
        if (await blob.UndeleteAsync((VssRequestPump.Processor) processor).ConfigureAwait(false))
        {
          if (await blob.ExistsAsync((VssRequestPump.Processor) processor).ConfigureAwait(false))
            return true;
        }
        await Task.Delay(TimeSpan.FromSeconds((double) (1 >> attempt))).ConfigureAwait(false);
      }
      return false;
    }

    public IEnumerable<Uri> GetContainerUrls(IDomainSecurityValidator validator)
    {
      List<Uri> containerUrls = new List<Uri>();
      IEnumerable<ICloudBlobContainer> cloudBlobContainers = (IEnumerable<ICloudBlobContainer>) new List<ICloudBlobContainer>();
      foreach (IDomainId listDomain in this.domainProvider.ListDomains())
        cloudBlobContainers = this.domainProvider.GetDomain((ISecuredDomainRequest) new SecuredDomainRequest(validator, listDomain)).GetAllContainers();
      foreach (ICloudBlobContainer cloudBlobContainer in cloudBlobContainers)
        containerUrls.Add(cloudBlobContainer.StorageUri.PrimaryUri);
      return (IEnumerable<Uri>) containerUrls;
    }

    public Task<IDictionary<DedupIdentifier, PreauthenticatedUri>> GetDownloadUrlsAsync(
      VssRequestPump.SecuredDomainProcessor processor,
      ISet<DedupIdentifier> dedupIds,
      SASUriExpiry expiry,
      (string, Guid)[] sasTracing)
    {
      return Task.FromResult<IDictionary<DedupIdentifier, PreauthenticatedUri>>((IDictionary<DedupIdentifier, PreauthenticatedUri>) dedupIds.ToDictionary<DedupIdentifier, DedupIdentifier, PreauthenticatedUri>((Func<DedupIdentifier, DedupIdentifier>) (dedupId => dedupId), (Func<DedupIdentifier, PreauthenticatedUri>) (dedupId =>
      {
        DateTimeOffset blobExpiryTime = expiry.RoundSASTimeRange(dedupId.ToBlobIdentifier());
        ICloudBlockBlob blockBlobReference = this.GetBlockBlobReference(processor.SecuredDomainRequest, dedupId);
        LocationMode? locationMode1 = this.locationMode;
        LocationMode locationMode2 = LocationMode.SecondaryOnly;
        Uri uri = locationMode1.GetValueOrDefault() == locationMode2 & locationMode1.HasValue ? blockBlobReference.StorageUri.SecondaryUri : blockBlobReference.StorageUri.PrimaryUri;
        try
        {
          string compatibleSasToken = AzureBlobBlobProvider.GetAzureFrontDoorCompatibleSASToken(blockBlobReference, blobExpiryTime, expiry, (string) null, (SharedAccessBlobHeaders) null, sasTracing);
          return new PreauthenticatedUri(new Uri(uri?.ToString() + compatibleSasToken), EdgeType.NotEdge);
        }
        catch (Microsoft.Azure.Storage.StorageException ex)
        {
          ICloudBlockBlob blob = blockBlobReference;
          throw new ExpandedStorageException(ex, blob);
        }
      })));
    }

    public async Task<DateTime> PutDedupAndKeepUntilReferenceAsync(
      VssRequestPump.SecuredDomainProcessor processor,
      DedupIdentifier dedupId,
      DedupCompressedBuffer dedup,
      DateTime keepUntil,
      bool useHttpClient)
    {
      IKeepUntil newKu = (IKeepUntil) new ConstantFutureKeepUntil(false, (DateTimeOffset) keepUntil, (DateTimeOffset) (keepUntil + AzureBlobTableDedupProvider.GetKeepUntilBonus(dedupId)), (DateTimeOffset) keepUntil);
      DateTime dedupKu = newKu.GetMarkingDate();
      ICloudBlockBlob blob = this.GetBlockBlobReference(processor.SecuredDomainRequest, dedupId);
      try
      {
        ArraySegment<byte>? compressedBytes;
        bool isCompressed = dedup.TryGetAlreadyCompressed(out compressedBytes);
        ArraySegment<byte> bytes = isCompressed ? compressedBytes.Value : dedup.Uncompressed;
        blob.Properties.ContentEncoding = isCompressed ? "xpress" : "none";
        AzureBlobTableDedupProvider.WriteKeepUntil(blob, newKu.GetMarkingDate());
        bool flag1;
        try
        {
          AzureBlobTableDedupProvider.ValidateBlobMetadata(blob);
          await blob.UploadFromByteArrayAsync((VssRequestPump.Processor) processor, bytes, useHttpClient, AccessCondition.GenerateIfNotExistsCondition()).ConfigureAwait(false);
          flag1 = false;
        }
        catch (Microsoft.Azure.Storage.StorageException ex) when (
        {
          // ISSUE: unable to correctly present filter
          Microsoft.Azure.Storage.RequestResult requestInformation = ex.RequestInformation;
          if (requestInformation != null && requestInformation.HttpStatusCode == 409)
          {
            SuccessfulFiltering;
          }
          else
            throw;
        }
        )
        {
          flag1 = true;
        }
        int attempts = 10;
        while (flag1)
        {
          --attempts;
          if (!await blob.ExistsAsync((VssRequestPump.Processor) processor).ConfigureAwait(false))
            return await this.PutDedupAndKeepUntilReferenceAsync(processor, dedupId, dedup, keepUntil, useHttpClient).ConfigureAwait(false);
          bool flag2 = isCompressed && blob.Properties.ContentEncoding != "xpress";
          if (AzureBlobTableDedupProvider.TryUpdateLocalKeepUntil(blob, newKu, out dedupKu) || flag2)
          {
            AccessCondition ifMatchCondition = AccessCondition.GenerateIfMatchCondition(blob.Properties.ETag);
            try
            {
              if (flag2)
              {
                blob.Properties.ContentEncoding = "xpress";
                AzureBlobTableDedupProvider.ValidateBlobMetadata(blob);
                await blob.UploadFromByteArrayAsync((VssRequestPump.Processor) processor, bytes, useHttpClient, ifMatchCondition).ConfigureAwait(false);
              }
              else
              {
                AzureBlobTableDedupProvider.ValidateBlobMetadata(blob);
                await blob.SetMetadataAsync((VssRequestPump.Processor) processor, ifMatchCondition).ConfigureAwait(false);
              }
              flag1 = false;
            }
            catch (Microsoft.Azure.Storage.StorageException ex) when (
            {
              // ISSUE: unable to correctly present filter
              Microsoft.Azure.Storage.RequestResult requestInformation = ex.RequestInformation;
              if (requestInformation != null && requestInformation.HttpStatusCode == 412)
              {
                SuccessfulFiltering;
              }
              else
                throw;
            }
            )
            {
              flag1 = true;
              if (attempts == 0)
                throw new TimeoutException("Too many compare-swap failures. (DedupId: " + dedupId.ValueString + ")", (Exception) ex);
            }
          }
          else
            break;
        }
        return dedupKu;
      }
      catch (Microsoft.Azure.Storage.StorageException ex)
      {
        ICloudBlockBlob blob1 = blob;
        throw new ExpandedStorageException(ex, blob1);
      }
    }

    public async Task<IDictionary<DedupIdentifier, DateTime?>> TryAddKeepUntilReferencesAsync(
      VssRequestPump.SecuredDomainProcessor processor,
      ISet<DedupIdentifier> dedupIds,
      DateTime keepUntil)
    {
      ConcurrentDictionary<DedupIdentifier, DateTime?> results = new ConcurrentDictionary<DedupIdentifier, DateTime?>();
      ExecutionDataflowBlockOptions dataflowBlockOptions = new ExecutionDataflowBlockOptions();
      dataflowBlockOptions.MaxDegreeOfParallelism = Environment.ProcessorCount;
      dataflowBlockOptions.BoundedCapacity = 4 * Environment.ProcessorCount;
      dataflowBlockOptions.CancellationToken = processor.CancellationToken;
      dataflowBlockOptions.EnsureOrdered = false;
      await NonSwallowingActionBlock.Create<DedupIdentifier>((Func<DedupIdentifier, Task>) (async dedupId =>
      {
        TimeSpan keepUntilBonus = AzureBlobTableDedupProvider.GetKeepUntilBonus(dedupId);
        int num = (int) await this.TryExtendSingleKeepUntilReferenceAsync(processor, dedupId, (IKeepUntil) new ConstantFutureKeepUntil(false, (DateTimeOffset) keepUntil, (DateTimeOffset) (keepUntil + keepUntilBonus), (DateTimeOffset) keepUntil), results).ConfigureAwait(false);
      }), dataflowBlockOptions).SendAllAndCompleteSingleBlockNetworkAsync<DedupIdentifier>((IEnumerable<DedupIdentifier>) dedupIds, processor.CancellationToken).ConfigureAwait(false);
      return (IDictionary<DedupIdentifier, DateTime?>) results;
    }

    public async Task<MetadataOperationResult> TryExtendKeepUntilReferenceAsync(
      VssRequestPump.SecuredDomainProcessor processor,
      DedupIdentifier dedupId,
      IKeepUntil keepUntil)
    {
      return await this.TryExtendSingleKeepUntilReferenceAsync(processor, dedupId, keepUntil, (ConcurrentDictionary<DedupIdentifier, DateTime?>) null).ConfigureAwait(false);
    }

    private async Task<MetadataOperationResult> TryExtendSingleKeepUntilReferenceAsync(
      VssRequestPump.SecuredDomainProcessor processor,
      DedupIdentifier dedupId,
      IKeepUntil keepUntil,
      ConcurrentDictionary<DedupIdentifier, DateTime?> results)
    {
      ICloudBlockBlob blob = this.GetBlockBlobReference(processor.SecuredDomainRequest, dedupId);
      try
      {
        int attempts = 10;
        DateTime blobKu;
        while (true)
        {
          --attempts;
          if (attempts >= 0)
          {
            if (await blob.ExistsAsync((VssRequestPump.Processor) processor).ConfigureAwait(false))
            {
              if (AzureBlobTableDedupProvider.TryUpdateLocalKeepUntil(blob, keepUntil, out blobKu))
              {
                try
                {
                  AzureBlobTableDedupProvider.ValidateBlobMetadata(blob);
                  await blob.SetMetadataAsync((VssRequestPump.Processor) processor, AccessCondition.GenerateIfMatchCondition(blob.Properties.ETag)).ConfigureAwait(false);
                  results?.TryAdd(dedupId, new DateTime?(blobKu));
                  return MetadataOperationResult.Changed;
                }
                catch (Microsoft.Azure.Storage.StorageException ex) when (
                {
                  // ISSUE: unable to correctly present filter
                  Microsoft.Azure.Storage.RequestResult requestInformation = ex.RequestInformation;
                  if (requestInformation != null && requestInformation.HttpStatusCode == 412)
                  {
                    SuccessfulFiltering;
                  }
                  else
                    throw;
                }
                )
                {
                }
              }
              else
                goto label_9;
            }
            else
              goto label_5;
          }
          else
            break;
        }
        throw new TimeoutException("Too many compare-swap failures. (DedupId: " + dedupId.ValueString + ")");
label_5:
        results?.TryAdd(dedupId, new DateTime?());
        return MetadataOperationResult.Missing;
label_9:
        results?.TryAdd(dedupId, new DateTime?(blobKu));
        return MetadataOperationResult.Unchanged;
      }
      catch (Microsoft.Azure.Storage.StorageException ex) when (!(ex is ExpandedStorageException))
      {
        throw new ExpandedStorageException(ex, blob);
      }
    }

    public async Task<IDictionary<DedupIdentifier, KeepUntilResult?>> ValidateKeepUntilReferencesAsync(
      VssRequestPump.SecuredDomainProcessor processor,
      ISet<DedupIdentifier> dedupIds,
      DateTime keepUntil)
    {
      ConcurrentDictionary<DedupIdentifier, KeepUntilResult?> results = new ConcurrentDictionary<DedupIdentifier, KeepUntilResult?>();
      ExecutionDataflowBlockOptions dataflowBlockOptions = new ExecutionDataflowBlockOptions();
      dataflowBlockOptions.MaxDegreeOfParallelism = Environment.ProcessorCount;
      dataflowBlockOptions.BoundedCapacity = 4 * Environment.ProcessorCount;
      dataflowBlockOptions.CancellationToken = processor.CancellationToken;
      dataflowBlockOptions.EnsureOrdered = false;
      await NonSwallowingActionBlock.Create<DedupIdentifier>((Func<DedupIdentifier, Task>) (async dedupId =>
      {
        ICloudBlockBlob blob = this.GetBlockBlobReference(processor.SecuredDomainRequest, dedupId);
        try
        {
          if (await blob.ExistsAsync((VssRequestPump.Processor) processor).ConfigureAwait(false))
          {
            DateTime bestAvailableKeepUntil = AzureBlobTableDedupProvider.ReadKeepUntil(blob);
            results.TryAdd(dedupId, new KeepUntilResult?(new KeepUntilResult(keepUntil <= bestAvailableKeepUntil, bestAvailableKeepUntil)));
            blob = (ICloudBlockBlob) null;
          }
          else
          {
            results.TryAdd(dedupId, new KeepUntilResult?());
            blob = (ICloudBlockBlob) null;
          }
        }
        catch (Microsoft.Azure.Storage.StorageException ex)
        {
          ICloudBlockBlob blob1 = blob;
          throw new ExpandedStorageException(ex, blob1);
        }
      }), dataflowBlockOptions).SendAllAndCompleteSingleBlockNetworkAsync<DedupIdentifier>((IEnumerable<DedupIdentifier>) dedupIds, processor.CancellationToken).ConfigureAwait(false);
      return (IDictionary<DedupIdentifier, KeepUntilResult?>) results;
    }

    public async Task<DateTime> GetKeepUntilAsync(
      VssRequestPump.SecuredDomainProcessor processor,
      DedupIdentifier dedupId)
    {
      ICloudBlockBlob blob = this.GetBlockBlobReference(processor.SecuredDomainRequest, dedupId);
      DateTime keepUntilAsync;
      try
      {
        keepUntilAsync = await blob.ExistsAsync((VssRequestPump.Processor) processor).ConfigureAwait(false) ? AzureBlobTableDedupProvider.ReadKeepUntil(blob) : throw new BlobNotFoundException("Could not find blob with name: " + blob.Name);
      }
      catch (Microsoft.Azure.Storage.StorageException ex)
      {
        ICloudBlockBlob blob1 = blob;
        throw new ExpandedStorageException(ex, blob1);
      }
      blob = (ICloudBlockBlob) null;
      return keepUntilAsync;
    }

    public Task AddRootAsync(
      VssRequestPump.SecuredDomainProcessor processor,
      DedupIdentifier dedupId,
      IdBlobReference rootRef,
      long size)
    {
      return this.AddReferenceInternalAsync(processor, dedupId, rootRef, size);
    }

    public Task RemoveRootAsync(
      VssRequestPump.SecuredDomainProcessor processor,
      DedupIdentifier dedupId,
      IdBlobReference rootRef)
    {
      return this.RemoveReferenceInternalAsync(processor, dedupId, rootRef);
    }

    public async Task RestoreRootAsync(
      VssRequestPump.SecuredDomainProcessor processor,
      DedupIdentifier dedupId,
      IdBlobReference rootRef)
    {
      await this.PerformUpdateOrDeleteOperationAsync(processor, dedupId, rootRef, new long?(), (Func<ITable, ReferenceRowTableEntity, Task<HttpStatusCode>>) (async (table, entity) =>
      {
        if (entity.State != ReferenceState.SoftDeleted)
          return entity.State != ReferenceState.Active ? HttpStatusCode.NotFound : HttpStatusCode.OK;
        entity.State = ReferenceState.Active;
        entity.StateChangeTime = new DateTimeOffset?(this.clock.Now);
        return (await table.ExecuteAsync((VssRequestPump.Processor) processor, TableOperationDescriptor.Merge((ITableEntity) entity)).ConfigureAwait(false)).HttpStatusCode;
      })).ConfigureAwait(false);
    }

    public Task UpdateRootSizeAsync(
      VssRequestPump.SecuredDomainProcessor processor,
      DedupMetadataEntry entry)
    {
      return this.UpdateReferenceSizeInternalAsync(processor, entry);
    }

    public IConcurrentIterator<IEnumerable<DedupMetadataEntry>> GetRootPagesAsync(
      VssRequestPump.Processor processor,
      DedupMetadataPageRetrievalOption option)
    {
      DateTimeOffset? nullable;
      DateTimeOffset dateTimeOffset;
      ComparisonFilter<TimestampColumn> comparisonFilter1;
      if (!option.Start.HasValue)
      {
        comparisonFilter1 = (ComparisonFilter<TimestampColumn>) null;
      }
      else
      {
        nullable = option.Start;
        dateTimeOffset = nullable.Value;
        comparisonFilter1 = new ComparisonFilter<TimestampColumn>((IColumnValue<TimestampColumn>) new TimestampColumnValue(dateTimeOffset.UtcDateTime), ComparisonOperator.GreaterThanOrEqual);
      }
      ComparisonFilter<TimestampColumn> comparisonFilter2 = comparisonFilter1;
      nullable = option.End;
      ComparisonFilter<TimestampColumn> comparisonFilter3;
      if (!nullable.HasValue)
      {
        comparisonFilter3 = (ComparisonFilter<TimestampColumn>) null;
      }
      else
      {
        nullable = option.End;
        dateTimeOffset = nullable.Value;
        comparisonFilter3 = new ComparisonFilter<TimestampColumn>((IColumnValue<TimestampColumn>) new TimestampColumnValue(dateTimeOffset.UtcDateTime), ComparisonOperator.LessThanOrEqual);
      }
      ComparisonFilter<TimestampColumn> comparisonFilter4 = comparisonFilter3;
      IFilter<TimestampColumn> filter = (IFilter<TimestampColumn>) null;
      if (comparisonFilter2 != null && comparisonFilter4 != null)
        filter = (IFilter<TimestampColumn>) new BooleanFilter<TimestampColumn>(BooleanOperator.And, new IFilter<TimestampColumn>[2]
        {
          (IFilter<TimestampColumn>) comparisonFilter2,
          (IFilter<TimestampColumn>) comparisonFilter4
        });
      else if (comparisonFilter2 != null)
        filter = (IFilter<TimestampColumn>) comparisonFilter2;
      else if (comparisonFilter4 != null)
        filter = (IFilter<TimestampColumn>) comparisonFilter4;
      IFilter<UserColumn> userColumnFilter = (IFilter<UserColumn>) null;
      if (option.StateFilter == StateFilter.SoftDeleted)
        userColumnFilter = (IFilter<UserColumn>) new BooleanFilter<UserColumn>(BooleanOperator.And, new IFilter<UserColumn>[1]
        {
          (IFilter<UserColumn>) new ComparisonFilter<UserColumn>((IColumnValue<UserColumn>) new ReferenceRowTableEntity.StateColumnValue(ReferenceState.SoftDeleted), ComparisonOperator.Equal)
        });
      ArtifactScopeType? scopeFilter = option.ScopeFilter;
      if (scopeFilter.HasValue)
      {
        scopeFilter = option.ScopeFilter;
        ArtifactScopeType artifactScopeType = scopeFilter.Value;
        if (!string.IsNullOrWhiteSpace(artifactScopeType.ToString()))
        {
          BooleanOperator and = BooleanOperator.And;
          IFilter<UserColumn>[] filterArray = new IFilter<UserColumn>[1];
          scopeFilter = option.ScopeFilter;
          artifactScopeType = scopeFilter.Value;
          filterArray[0] = (IFilter<UserColumn>) new ComparisonFilter<UserColumn>((IColumnValue<UserColumn>) new ReferenceRowTableEntity.ScopeColumnValue(artifactScopeType.ToString().ToLower()), ComparisonOperator.Equal);
          userColumnFilter = (IFilter<UserColumn>) new BooleanFilter<UserColumn>(and, filterArray);
        }
      }
      ComparisonFilter<DomainColumn> domainColumnFilter = option.Domain != (IDomainId) null ? new ComparisonFilter<DomainColumn>((IColumnValue<DomainColumn>) new DomainColumnValue(option.Domain), ComparisonOperator.Equal) : (ComparisonFilter<DomainColumn>) null;
      string startingKey = option.StartingKey;
      Query<ReferenceRowTableEntity> query;
      if (string.IsNullOrWhiteSpace(startingKey))
        query = (Query<ReferenceRowTableEntity>) new PartitionScanRowRangeQuery<ReferenceRowTableEntity>(ReferenceRowTableEntity.AllRowsFilter, filter, (IFilter<DomainColumn>) domainColumnFilter, userColumnFilter);
      else
        query = (Query<ReferenceRowTableEntity>) new PartitionRangeRowRangeQuery<ReferenceRowTableEntity>(new RangeFilter<PartitionKeyColumn>(new RangeMinimumBoundary<PartitionKeyColumn>((IColumnValue<PartitionKeyColumn>) new PartitionKeyColumnValue(startingKey), RangeBoundaryType.Inclusive), new RangeMaximumBoundary<PartitionKeyColumn>((IColumnValue<PartitionKeyColumn>) new PartitionKeyColumnValue(option.Prefix + "~"), RangeBoundaryType.Exclusive)), ReferenceRowTableEntity.AllRowsFilter, (IFilter<INonUserColumn>) filter, (IFilter<DomainColumn>) domainColumnFilter, userColumnFilter);
      ResultArrangement arrangement = option.Arrangement;
      if (arrangement != ResultArrangement.AllOrdered)
        return (IConcurrentIterator<IEnumerable<DedupMetadataEntry>>) new ConcurrentIterator<ITable, IEnumerable<DedupMetadataEntry>>(this.tableClientFactory.GetAllTables(), new int?(Environment.ProcessorCount), new int?(4 * Environment.ProcessorCount), processor.CancellationToken, (Func<ITable, TryAddValueAsyncFunc<IEnumerable<DedupMetadataEntry>>, CancellationToken, Task>) (async (table, valueAdderAsync, cancelToken) => await table.QueryPagesConcurrentIterator<ReferenceRowTableEntity>(processor, query).ForEachAsyncNoContext<IReadOnlyList<ReferenceRowTableEntity>>(cancelToken, (Func<IReadOnlyList<ReferenceRowTableEntity>, Task>) (async page =>
        {
          IEnumerable<ReferenceRowTableEntity> source = (IEnumerable<ReferenceRowTableEntity>) page;
          if (option.StateFilter == StateFilter.Active)
            source = source.Where<ReferenceRowTableEntity>((Func<ReferenceRowTableEntity, bool>) (e => !e.IsSoftDeleted));
          if (!source.Any<ReferenceRowTableEntity>())
            return;
          IEnumerable<DedupMetadataEntry> dedupMetadataEntries = source.Select<ReferenceRowTableEntity, DedupMetadataEntry>((Func<ReferenceRowTableEntity, DedupMetadataEntry>) (entity => new DedupMetadataEntry(DedupIdentifier.Create(entity.PartitionKey), entity.Scope, entity.ReferenceId, entity.IsSoftDeleted, entity.StateChangeTime, entity.Size)));
          if (arrangement == ResultArrangement.DistinctUnordered)
            dedupMetadataEntries = dedupMetadataEntries.Distinct<DedupMetadataEntry>((IEqualityComparer<DedupMetadataEntry>) DedupOnlyComparer.Instance);
          int num = await valueAdderAsync(dedupMetadataEntries).ConfigureAwait(false) ? 1 : 0;
        })).ConfigureAwait(false)));
      IConcurrentIterator<ReferenceRowTableEntity> enumerator = this.tableClientFactory.GetAllTables().Select<ITable, IConcurrentIterator<ReferenceRowTableEntity>>((Func<ITable, IConcurrentIterator<ReferenceRowTableEntity>>) (table => table.QueryConcurrentIterator<ReferenceRowTableEntity>(processor, query, option.BoundedCapacity))).CollectSortOrdered<ReferenceRowTableEntity>(new int?(1000), (IComparer<ReferenceRowTableEntity>) AzureBlobTableDedupProvider.PKComparer.Instance, processor.CancellationToken);
      if (option.StateFilter == StateFilter.Active)
        enumerator = enumerator.Where<ReferenceRowTableEntity>((Func<ReferenceRowTableEntity, bool>) (e => !e.IsSoftDeleted));
      return (IConcurrentIterator<IEnumerable<DedupMetadataEntry>>) enumerator.Select<ReferenceRowTableEntity, DedupMetadataEntry>((Func<ReferenceRowTableEntity, DedupMetadataEntry>) (entity => new DedupMetadataEntry(DedupIdentifier.Create(entity.PartitionKey), entity.Scope, entity.ReferenceId, entity.IsSoftDeleted, entity.StateChangeTime, entity.Size))).GetPages<DedupMetadataEntry>(option.PageSize, processor.CancellationToken);
    }

    private Task<IBlobResultSegment> GetBlobListSegmentedAsync(
      VssRequestPump.Processor processor,
      ICloudBlobContainer container,
      BlobContinuationToken continuation,
      string prefix,
      bool includeDeletedBlobs,
      int? pageSize = null)
    {
      BlobListingDetails blobListingDetails1 = BlobListingDetails.Metadata;
      if (includeDeletedBlobs)
        blobListingDetails1 |= BlobListingDetails.Deleted;
      ICloudBlobContainer cloudBlobContainer = container;
      VssRequestPump.Processor processor1 = processor;
      string prefix1 = prefix;
      BlobContinuationToken continuationToken = continuation;
      int blobListingDetails2 = (int) blobListingDetails1;
      int? maxResults = new int?(pageSize ?? 5000);
      BlobContinuationToken currentToken = continuationToken;
      return cloudBlobContainer.ListBlobsSegmentedAsync(processor1, prefix1, blobListingDetails: (BlobListingDetails) blobListingDetails2, maxResults: maxResults, currentToken: currentToken);
    }

    private IConcurrentIterator<ICloudBlockBlob> GetDedupBlobsAsync(
      VssRequestPump.SecuredDomainProcessor processor,
      string prefix,
      bool sorted,
      bool includeDeletedBlobs,
      int boundedCapacity = 0)
    {
      int effectiveBoundedCapacity = boundedCapacity == 0 ? 10000 : boundedCapacity;
      IEnumerable<ConcurrentIterator<ICloudBlockBlob>> concurrentIterators = this.domainProvider.GetDomain(processor.SecuredDomainRequest).GetAllContainers().Select<ICloudBlobContainer, ConcurrentIterator<ICloudBlockBlob>>((Func<ICloudBlobContainer, ConcurrentIterator<ICloudBlockBlob>>) (cont => new ConcurrentIterator<ICloudBlockBlob>(new int?(effectiveBoundedCapacity), processor.CancellationToken, (Func<TryAddValueAsyncFunc<ICloudBlockBlob>, CancellationToken, Task>) (async (tryAddAsync, cancellationToken) =>
      {
        BlobContinuationToken continuation = (BlobContinuationToken) null;
        do
        {
          IBlobResultSegment blobResultSegment = await this.GetBlobListSegmentedAsync((VssRequestPump.Processor) processor, cont, continuation, prefix, includeDeletedBlobs).ConfigureAwait(false);
          continuation = blobResultSegment.ContinuationToken;
          foreach (ICloudBlockBlob valueToAdd in blobResultSegment.Results.OfType<ICloudBlockBlob>())
          {
            if (!await tryAddAsync(valueToAdd))
            {
              continuation = (BlobContinuationToken) null;
              return;
            }
          }
        }
        while (continuation != null);
        continuation = (BlobContinuationToken) null;
      }))));
      return sorted ? ((IEnumerable<IConcurrentIterator<ICloudBlockBlob>>) concurrentIterators).CollectSortOrdered<ICloudBlockBlob>(new int?(effectiveBoundedCapacity), (IComparer<ICloudBlockBlob>) AzureBlobTableDedupProvider.BlobIdComparer.Instance, processor.CancellationToken) : ((IEnumerable<IConcurrentIterator<ICloudBlockBlob>>) concurrentIterators).CollectUnordered<ICloudBlockBlob>(new int?(effectiveBoundedCapacity), processor.CancellationToken);
    }

    public IConcurrentIterator<IEnumerable<KeyValuePair<DedupIdentifier, DateTime>>> GetDedupPagesAsync(
      VssRequestPump.SecuredDomainProcessor processor)
    {
      return this.GetDedupBlobsAsync(processor, string.Empty, false, false).GetPages<ICloudBlockBlob>(5000, processor.CancellationToken).Select<IReadOnlyCollection<ICloudBlockBlob>, IEnumerable<KeyValuePair<DedupIdentifier, DateTime>>>((Func<IReadOnlyCollection<ICloudBlockBlob>, IEnumerable<KeyValuePair<DedupIdentifier, DateTime>>>) (blobPage => blobPage.Select<ICloudBlockBlob, KeyValuePair<DedupIdentifier, DateTime>>((Func<ICloudBlockBlob, KeyValuePair<DedupIdentifier, DateTime>>) (blob => new KeyValuePair<DedupIdentifier, DateTime>(DedupIdentifier.Create(blob.Name), AzureBlobTableDedupProvider.ReadKeepUntil(blob))))));
    }

    public async Task<DeleteResult> DeleteExpiredDedupsAsync(
      VssRequestPump.SecuredDomainProcessor processor,
      ChunkDedupGCCheckpoint checkpointData,
      DateTimeOffset expiryKeepUntil,
      int boundedCapacity = 0)
    {
      DeleteResult deleteResult;
      using (await processor.ExecuteWorkAsync<IDisposable>((Func<IVssRequestContext, IDisposable>) (requestContext => requestContext.Enter(ContentTracePoints.DedupStoreService.DeleteExpiredDedupsAsyncCall, nameof (DeleteExpiredDedupsAsync)))))
      {
        int totalDeleted = 0;
        int totalScanned = 0;
        int totalEligibleForDelete = 0;
        ulong totalBytesDeleted = 0;
        int telemetryBatchSize = await processor.ExecuteWorkAsync<int>((Func<IVssRequestContext, int>) (requestContext => requestContext.GetService<IVssRegistryService>().GetValue<int>(requestContext, (RegistryQuery) ServiceRegistryConstants.ChunkDedupGarbageCollectionDeleteTelemetryBatchSizePath, true, 500)));
        List<string> deletedBlobNames = new List<string>();
        bool skipPage = false;
        int pageSize = boundedCapacity == 0 ? 10000 : boundedCapacity;
        using (IConcurrentIterator<IReadOnlyCollection<ICloudBlockBlob>> dedupPages = this.GetDedupBlobsAsync(processor, string.Empty, true, false, boundedCapacity).GetPages<ICloudBlockBlob>(pageSize, processor.CancellationToken))
        {
          while (true)
          {
            if (await dedupPages.MoveNextAsync(processor.CancellationToken))
            {
              try
              {
                if (checkpointData != null)
                  skipPage = dedupPages.Current.Last<ICloudBlockBlob>().Name.CompareTo(checkpointData.CurrentDedupIdentifier) < 0;
                if (!skipPage)
                {
                  foreach (ICloudBlockBlob blob in (IEnumerable<ICloudBlockBlob>) dedupPages.Current)
                  {
                    ++totalScanned;
                    DateTimeOffset dateTimeOffset = DateTimeOffset.FromFileTime(AzureBlobTableDedupProvider.ReadKeepUntilFileTime(blob));
                    if (dateTimeOffset != new DateTimeOffset() && dateTimeOffset < expiryKeepUntil)
                    {
                      ++totalEligibleForDelete;
                      long lengthOfBlob = blob.Properties.Length;
                      if (await blob.DeleteIfExistsAsync((VssRequestPump.Processor) processor, accessCondition: AccessCondition.GenerateIfMatchCondition(blob.Properties.ETag)).ConfigureAwait(false))
                      {
                        deletedBlobNames.Add(blob.Name);
                        ++totalDeleted;
                        totalBytesDeleted += (ulong) lengthOfBlob;
                        if (deletedBlobNames.Count == telemetryBatchSize)
                        {
                          await processor.ExecuteWorkAsync(closure_10 ?? (closure_10 = (Action<IVssRequestContext>) (requestContext => requestContext.TraceAlways(ContentTracePoints.DedupStoreService.DeleteExpiredDedupsAsyncInfo, string.Format("Initial MarkTime: {0}. BlobsDeleted Range:{1} to {2}. Total Number Deleted: {3}. Cummulative Total Physical Bytes Deleted: {4}.", (object) checkpointData?.MarkTime, (object) deletedBlobNames.FirstOrDefault<string>(), (object) deletedBlobNames.LastOrDefault<string>(), (object) deletedBlobNames.Count<string>(), (object) totalBytesDeleted)))));
                          deletedBlobNames.Clear();
                        }
                      }
                    }
                  }
                }
                await processor.ExecuteWorkAsync((Action<IVssRequestContext>) (requestContext => checkpointData?.SaveCheckpoint(requestContext, dedupPages.Current.LastOrDefault<ICloudBlockBlob>()?.Name.ToString())));
              }
              catch (Microsoft.Azure.Storage.StorageException ex) when (ex.RequestInformation.HttpStatusCode == 412)
              {
                await processor.ExecuteWorkAsync((Action<IVssRequestContext>) (requestContext => requestContext.TraceException(ContentTracePoints.DedupStoreService.DeleteExpiredDedupsAsyncInfo, (Exception) ex)));
              }
            }
            else
              break;
          }
          if (deletedBlobNames.Count > 0)
            await processor.ExecuteWorkAsync((Action<IVssRequestContext>) (requestContext => requestContext.TraceAlways(ContentTracePoints.DedupStoreService.DeleteExpiredDedupsAsyncInfo, string.Format("Initial MarkTime: {0}. BlobsDeleted Range:{1} to {2}. Total Number Deleted: {3}. Cummulative Total Physical Bytes Deleted: {4}", (object) checkpointData?.MarkTime, (object) deletedBlobNames.FirstOrDefault<string>(), (object) deletedBlobNames.LastOrDefault<string>(), (object) deletedBlobNames.Count<string>(), (object) totalBytesDeleted))));
        }
        await processor.ExecuteWorkAsync((Action<IVssRequestContext>) (requestContext => requestContext.TraceAlways(ContentTracePoints.DedupStoreService.DeleteExpiredDedupsAsyncCall.LeaveTracePoint, TraceLevel.Info, "DedupStore", "DedupStoreService", string.Format("Initial MarkTime: {0}. TotalScanned:{1}, TotalEligibleForDelete:{2}, TotalDeleted: {3}, Total Physical Bytes Deleted: {4}", (object) checkpointData?.MarkTime, (object) totalScanned, (object) totalEligibleForDelete, (object) totalDeleted, (object) totalBytesDeleted))));
        deleteResult = new DeleteResult(true, (long) totalScanned, (long) totalEligibleForDelete, (long) totalDeleted, totalBytesDeleted);
      }
      return deleteResult;
    }

    internal static List<string> ListAllPrefixesForBlobDeletion(string prefix, string checkpoint)
    {
      checkpoint = checkpoint.Substring(0, Math.Min(checkpoint.Length, prefix.Length + 10));
      prefix += new string('F', checkpoint.Length - prefix.Length);
      return ShardingRangePrefixLister.ListAllPrefixes(checkpoint, prefix);
    }

    public async Task<IDedupInfo> GetDedupInfoAsync(
      VssRequestPump.SecuredDomainProcessor processor,
      DedupIdentifier dedupId)
    {
      ICloudBlockBlob blob = this.GetBlockBlobReference(processor.SecuredDomainRequest, dedupId);
      AzureDedupInfo azureDedupInfo;
      try
      {
        await blob.FetchAttributesAsync((VssRequestPump.Processor) processor).ConfigureAwait(false);
        try
        {
          azureDedupInfo = new AzureDedupInfo(dedupId, AzureBlobTableDedupProvider.ReadKeepUntil(blob));
        }
        catch (FormatException ex)
        {
          azureDedupInfo = new AzureDedupInfo(dedupId, HealthStatus.MissingMetadata);
        }
      }
      catch (Microsoft.Azure.Storage.StorageException ex)
      {
        azureDedupInfo = !ex.HasHttpStatus(HttpStatusCode.NotFound) ? new AzureDedupInfo(dedupId, HealthStatus.Undetermined) : new AzureDedupInfo(dedupId, HealthStatus.Absent);
      }
      IDedupInfo dedupInfoAsync = (IDedupInfo) azureDedupInfo;
      blob = (ICloudBlockBlob) null;
      return dedupInfoAsync;
    }

    private static bool TryUpdateLocalKeepUntil(
      ICloudBlockBlob blob,
      IKeepUntil keepUntil,
      out DateTime blobKu)
    {
      long fileTime = AzureBlobTableDedupProvider.ReadKeepUntilFileTime(blob);
      if (keepUntil.ShouldMark(DateTimeOffset.FromFileTime(fileTime)))
      {
        blobKu = keepUntil.GetMarkingDate();
        long fileTimeUtc = blobKu.ToFileTimeUtc();
        if (fileTimeUtc < fileTime)
          throw new InvalidOperationException(Resources.CannotMarkNewerKeepUntilInProduction());
        AzureBlobTableDedupProvider.WriteKeepUntilFileTime(blob, fileTimeUtc);
        return true;
      }
      blobKu = DateTime.FromFileTimeUtc(fileTime);
      return false;
    }

    private static DateTime ReadKeepUntil(ICloudBlockBlob blob) => DateTime.FromFileTimeUtc(AzureBlobTableDedupProvider.ReadKeepUntilFileTime(blob));

    private static long ReadKeepUntilFileTime(ICloudBlockBlob blob) => long.Parse(blob.Metadata["keepUntil"]);

    private static void ValidateBlobMetadata(ICloudBlockBlob blob)
    {
      string s;
      if (!blob.Metadata.TryGetValue("keepUntil", out s))
        throw new InvalidOperationException("Cannot set blob without a keepUntil.");
      if (!long.TryParse(s, out long _))
        throw new InvalidOperationException("KeepUntil must be a long.");
      if (string.IsNullOrEmpty(blob.Properties.ContentEncoding))
        throw new InvalidOperationException("Cannot set blob without a ContentEncoding.");
      switch (blob.Properties.ContentEncoding)
      {
        case "xpress":
          break;
        case "none":
          break;
        default:
          throw new InvalidOperationException("Unknown Content-Encoding:" + blob.Properties.ContentEncoding);
      }
    }

    private static void WriteKeepUntil(ICloudBlockBlob blob, DateTime keepUntil) => AzureBlobTableDedupProvider.WriteKeepUntilFileTime(blob, keepUntil.ToFileTimeUtc());

    private static void WriteKeepUntilFileTime(ICloudBlockBlob blob, long keepUntil) => blob.Metadata[nameof (keepUntil)] = keepUntil.ToString();

    private static string GetBlobDeletionError(ICloudBlockBlob wrapper, Exception e) => "sa=" + wrapper?.StorageUri?.PrimaryUri?.Host + "/container=" + wrapper?.GetContainer()?.Name + "/dedup=" + wrapper?.Name + ", " + e?.GetType().Name + ": " + e?.Message;

    private async Task AddReferenceInternalAsync(
      VssRequestPump.SecuredDomainProcessor processor,
      DedupIdentifier dedupId,
      IdBlobReference reference,
      long size)
    {
      TableOperationResult tableOperationResult = await this.GetTable(dedupId).ExecuteAsync((VssRequestPump.Processor) processor, TableOperationDescriptor.InsertOrMerge((ITableEntity) new ReferenceRowTableEntity(dedupId, reference, "*", processor.SecuredDomainRequest.DomainId, new long?(size))
      {
        StateChangeTime = new DateTimeOffset?(this.clock.Now)
      })).ConfigureAwait(false);
      if (tableOperationResult.HttpStatusCode != HttpStatusCode.NoContent)
        throw new InvalidOperationException(string.Format("{0}: {1}", (object) nameof (AddReferenceInternalAsync), (object) tableOperationResult.HttpStatusCode));
    }

    private ICloudBlockBlob GetBlockBlobReference(
      ISecuredDomainRequest domainRequest,
      DedupIdentifier dedupId)
    {
      return this.GetContainer(domainRequest, dedupId).GetBlockBlobReference(dedupId.ValueString);
    }

    private ICloudBlobContainer GetContainer(
      ISecuredDomainRequest domainRequest,
      DedupIdentifier dedupId)
    {
      return !this.isStatisticsEnabled ? this.domainProvider.GetDomain(domainRequest).Find(dedupId) : (ICloudBlobContainer) new TracingCloudBlobContainer(this.domainProvider.GetDomain(domainRequest).Find(dedupId), this.statistics, this.isBlobOperationTimeoutEnabled);
    }

    private ITable GetTable(DedupIdentifier dedupId) => this.tableClientFactory.GetTable(dedupId.ValueString);

    private async Task PerformUpdateOrDeleteOperationAsync(
      VssRequestPump.SecuredDomainProcessor processor,
      DedupIdentifier dedupId,
      IdBlobReference reference,
      long? size,
      Func<ITable, ReferenceRowTableEntity, Task<HttpStatusCode>> operation)
    {
      ReferenceRowTableEntity refRowToQuery = new ReferenceRowTableEntity(dedupId, reference.Scope, reference.Name, "*", processor.SecuredDomainRequest.DomainId, size);
      PointQuery<ReferenceRowTableEntity> query = new PointQuery<ReferenceRowTableEntity>((StringColumnValue<PartitionKeyColumn>) new PartitionKeyColumnValue(refRowToQuery.PartitionKey), (StringColumnValue<RowKeyColumn>) new RowKeyColumnValue(refRowToQuery.RowKey));
      ITable table = this.GetTable(dedupId);
      int attempt = 0;
      bool flag;
      do
      {
        ReferenceRowTableEntity referenceRowTableEntity = await table.QuerySingleOrDefaultAsync<ReferenceRowTableEntity>((VssRequestPump.Processor) processor, (Query<ReferenceRowTableEntity>) query).ConfigureAwait(false);
        if (referenceRowTableEntity != null)
        {
          if (referenceRowTableEntity.PartitionKey != refRowToQuery.PartitionKey || referenceRowTableEntity.RowKey != refRowToQuery.RowKey)
            throw new InvalidOperationException("The query returned invalid results. PK: " + referenceRowTableEntity.PartitionKey + ", RK: " + referenceRowTableEntity.RowKey);
          HttpStatusCode httpStatusCode = await operation(table, referenceRowTableEntity).ConfigureAwait(false);
          int num;
          switch (httpStatusCode)
          {
            case HttpStatusCode.NoContent:
              goto label_14;
            case HttpStatusCode.NotFound:
              goto label_5;
            case HttpStatusCode.PreconditionFailed:
              num = ++attempt < 5 ? 1 : 0;
              break;
            default:
              num = 0;
              break;
          }
          flag = num != 0;
          if (!flag)
            throw new InvalidOperationException(string.Format("{0}: {1}", (object) nameof (PerformUpdateOrDeleteOperationAsync), (object) httpStatusCode));
        }
        else
          goto label_2;
      }
      while (flag);
      goto label_7;
label_2:
      refRowToQuery = (ReferenceRowTableEntity) null;
      query = (PointQuery<ReferenceRowTableEntity>) null;
      table = (ITable) null;
      return;
label_14:
      refRowToQuery = (ReferenceRowTableEntity) null;
      query = (PointQuery<ReferenceRowTableEntity>) null;
      table = (ITable) null;
      return;
label_5:
      refRowToQuery = (ReferenceRowTableEntity) null;
      query = (PointQuery<ReferenceRowTableEntity>) null;
      table = (ITable) null;
      return;
label_7:
      refRowToQuery = (ReferenceRowTableEntity) null;
      query = (PointQuery<ReferenceRowTableEntity>) null;
      table = (ITable) null;
    }

    private async Task RemoveReferenceInternalAsync(
      VssRequestPump.SecuredDomainProcessor processor,
      DedupIdentifier dedupId,
      IdBlobReference reference)
    {
      await this.PerformUpdateOrDeleteOperationAsync(processor, dedupId, reference, new long?(), (Func<ITable, ReferenceRowTableEntity, Task<HttpStatusCode>>) (async (table, entity) =>
      {
        if (entity.State != ReferenceState.Active)
          return HttpStatusCode.NotFound;
        entity.State = ReferenceState.SoftDeleted;
        entity.StateChangeTime = new DateTimeOffset?(this.clock.Now);
        return (await table.ExecuteAsync((VssRequestPump.Processor) processor, TableOperationDescriptor.Merge((ITableEntity) entity)).ConfigureAwait(false)).HttpStatusCode;
      })).ConfigureAwait(false);
    }

    private async Task UpdateReferenceSizeInternalAsync(
      VssRequestPump.SecuredDomainProcessor processor,
      DedupMetadataEntry entry)
    {
      await this.PerformUpdateOrDeleteOperationAsync(processor, entry.DedupId, new IdBlobReference(entry.ReferenceId, entry.Scope), entry.Size, (Func<ITable, ReferenceRowTableEntity, Task<HttpStatusCode>>) (async (table, entity) =>
      {
        entity.Size = entry.Size;
        entity.StateChangeTime = new DateTimeOffset?(this.clock.Now);
        return (await table.ExecuteAsync((VssRequestPump.Processor) processor, TableOperationDescriptor.Replace((ITableEntity) entity)).ConfigureAwait(false)).HttpStatusCode;
      })).ConfigureAwait(false);
    }

    public async Task HardDeleteRootAsync(
      VssRequestPump.SecuredDomainProcessor processor,
      DedupIdentifier dedupId,
      IdBlobReference reference)
    {
      await this.PerformUpdateOrDeleteOperationAsync(processor, dedupId, reference, new long?(), (Func<ITable, ReferenceRowTableEntity, Task<HttpStatusCode>>) (async (table, entity) => entity.State == ReferenceState.SoftDeleted ? (await table.ExecuteAsync((VssRequestPump.Processor) processor, TableOperationDescriptor.Delete((ITableEntity) entity)).ConfigureAwait(false)).HttpStatusCode : HttpStatusCode.NotFound)).ConfigureAwait(false);
    }

    private async Task AddBasicBlobMetadata(
      VssRequestPump.Processor processor,
      ICloudBlobContainer container,
      string prefix,
      TryAddValueAsyncFunc<BasicBlobMetadata> valueAdderAsync)
    {
      BlobContinuationToken continuationToken = (BlobContinuationToken) null;
      do
      {
        IBlobResultSegment blobResultSegment;
        try
        {
          blobResultSegment = await container.ListBlobsSegmentedAsync(processor, prefix, true, BlobListingDetails.Metadata, new int?(5000), continuationToken).ConfigureAwait(false);
        }
        catch (Microsoft.Azure.Storage.StorageException ex) when (ex.HasHttpStatus(HttpStatusCode.NotFound))
        {
          continuationToken = (BlobContinuationToken) null;
          return;
        }
        List<BasicBlobMetadata> basicBlobMetadataList = new List<BasicBlobMetadata>(blobResultSegment.Results.Count);
        foreach (IListBlobItem result in (IEnumerable<IListBlobItem>) blobResultSegment.Results)
        {
          if (result is CloudBlockBlobWrapper blockBlobWrapper)
            basicBlobMetadataList.Add(new BasicBlobMetadata(blockBlobWrapper.Name, blockBlobWrapper.Properties.Length, blockBlobWrapper.Properties.LastModified));
        }
        continuationToken = blobResultSegment.ContinuationToken;
        foreach (BasicBlobMetadata valueToAdd in basicBlobMetadataList)
        {
          if (!await valueAdderAsync(valueToAdd).ConfigureAwait(false))
            break;
        }
      }
      while (continuationToken != null);
      continuationToken = (BlobContinuationToken) null;
    }

    public IConcurrentIterator<BasicBlobMetadata> GetBasicBlobMetadataConcurrentIterator(
      VssRequestPump.SecuredDomainProcessor processor,
      IteratorPartition partition,
      string prefix)
    {
      IEnumerable<ICloudBlobContainer> allContainers = this.domainProvider.GetDomain(processor.SecuredDomainRequest).GetAllContainers();
      return ((IEnumerable<IConcurrentIterator<BasicBlobMetadata>>) partition.SelectIterators<ICloudBlobContainer>(allContainers).Select<ICloudBlobContainer, ConcurrentIterator<BasicBlobMetadata>>((Func<ICloudBlobContainer, ConcurrentIterator<BasicBlobMetadata>>) (container => new ConcurrentIterator<BasicBlobMetadata>(new int?(1000), processor.CancellationToken, (Func<TryAddValueAsyncFunc<BasicBlobMetadata>, CancellationToken, Task>) (async (valueAdderAsync, cancellationToken) => await this.AddBasicBlobMetadata((VssRequestPump.Processor) processor, container, prefix, valueAdderAsync).ConfigureAwait(false)))))).CollectSortOrdered<BasicBlobMetadata>(new int?(1000), (IComparer<BasicBlobMetadata>) Comparer<BasicBlobMetadata>.Create((Comparison<BasicBlobMetadata>) ((bm1, bm2) => string.Compare(bm1.Name, bm2.Name, StringComparison.Ordinal))), processor.CancellationToken);
    }

    public void ResetStorageStatistics(ITraceRequest tracer)
    {
      if (!this.isStatisticsEnabled || tracer == null)
        return;
      tracer.TraceAlways(5702099, TraceLevel.Info, Microsoft.VisualStudio.Services.BlobStore.Server.Common.Telemetry.TracePoints.StorageStatistics.TraceData.Area, Microsoft.VisualStudio.Services.BlobStore.Server.Common.Telemetry.TracePoints.StorageStatistics.TraceData.Layer, this.statistics.Reset().Serialize<IContainerStorageAccessStatistics>());
    }

    public void EnableStatistics(bool isEnabled) => this.isStatisticsEnabled = isEnabled;

    public void EnableBlobOperationTimeout(bool isEnabled) => this.isBlobOperationTimeoutEnabled = isEnabled;

    private class PKComparer : IComparer<ITableEntity>
    {
      internal static readonly AzureBlobTableDedupProvider.PKComparer Instance = new AzureBlobTableDedupProvider.PKComparer();

      private PKComparer()
      {
      }

      public int Compare(ITableEntity x, ITableEntity y) => string.CompareOrdinal(x.PartitionKey, y.PartitionKey);
    }

    private class BlobIdComparer : IComparer<ICloudBlockBlob>
    {
      internal static readonly AzureBlobTableDedupProvider.BlobIdComparer Instance = new AzureBlobTableDedupProvider.BlobIdComparer();

      private BlobIdComparer()
      {
      }

      public int Compare(ICloudBlockBlob x, ICloudBlockBlob y) => string.CompareOrdinal(x.Name, y.Name);
    }

    private class DedupIdPartitionKeyColumnValue : PartitionKeyColumnValue
    {
      public DedupIdPartitionKeyColumnValue(DedupIdentifier dedupId)
        : base(dedupId.ValueString)
      {
      }
    }
  }
}
