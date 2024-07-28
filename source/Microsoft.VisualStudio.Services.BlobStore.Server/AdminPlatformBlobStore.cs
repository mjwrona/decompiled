// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.AdminPlatformBlobStore
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D3AB5C9B-EB54-4477-A304-63BB297414A3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.Server.dll

using BuildXL.Cache.ContentStore.Hashing;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using Microsoft.Azure.Storage.Shared.Protocol;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.BlobStore.AzureStorage;
using Microsoft.VisualStudio.Services.BlobStore.Common;
using Microsoft.VisualStudio.Services.BlobStore.Server.Common;
using Microsoft.VisualStudio.Services.BlobStore.Server.Common.Domains;
using Microsoft.VisualStudio.Services.BlobStore.Server.Common.Models;
using Microsoft.VisualStudio.Services.Cloud.BlobWrappers;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Content.Common;
using Microsoft.VisualStudio.Services.Content.Server.Azure;
using Microsoft.VisualStudio.Services.Content.Server.Common;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace Microsoft.VisualStudio.Services.BlobStore.Server
{
  public class AdminPlatformBlobStore : 
    PlatformBlobStore,
    IAdminBlobStore,
    IBlobStore,
    IVssFrameworkService
  {
    private const int MinTraceTimeInMs = 3600000;
    private const int DefaultParallelismForProviderQueries = 4;

    private AdminAzureTableBlobMetadataProviderWithTestHooks AdminMetadataProviderWithTestHooks { get; set; }

    protected override AdminAzureTableBlobMetadataProviderWithTestHooks ConstructTableMetadataProvider(
      ITableClientFactory tableClientFactory,
      AzureTableBlobMetadataProviderOptions options)
    {
      this.AdminMetadataProviderWithTestHooks = new AdminAzureTableBlobMetadataProviderWithTestHooks(tableClientFactory, options);
      this.MetadataProvider = (IAdminBlobMetadataProviderWithTestHooks) this.AdminMetadataProviderWithTestHooks;
      return this.AdminMetadataProviderWithTestHooks;
    }

    public async Task<FixMetadataAfterDisasterResults> FixMetadataAfterDisasterAsync(
      IVssRequestContext requestContext,
      IDomainId domainId,
      DateTime lastSyncTime,
      BlobIdentifier startingBlobId = null,
      bool skipDeletion = false)
    {
      Guid taskGuid = Guid.NewGuid();
      FixMetadataAfterDisasterResults result = new FixMetadataAfterDisasterResults();
      SecuredDomainRequest domainRequest = new SecuredDomainRequest(requestContext, domainId);
      FixMetadataAfterDisasterResults afterDisasterResults;
      using (requestContext.Enter(ContentTracePoints.AdminPlatformBlobstore.FixMetadataAfterDisasterAsyncCall, nameof (FixMetadataAfterDisasterAsync)))
      {
        try
        {
          int parallelism = requestContext.GetService<IVssRegistryService>().GetValue<int>(requestContext, (RegistryQuery) ServiceRegistryConstants.DRCleanupParallelismPerAccountPath, true, 32);
          await requestContext.PumpFromAsync((ISecuredDomainRequest) domainRequest, (Func<VssRequestPump.SecuredDomainProcessor, Task>) (async vssProcessor =>
          {
            await vssProcessor.TraceAlwaysAsync(ContentTracePoints.AdminPlatformBlobstore.FixMetadataAfterDisasterAsyncBegin, string.Format("Begin fixing metadata with {0}={1} threads per storage account. (Id={2})", (object) "DRCleanupParallelismPerAccountPath", (object) parallelism, (object) taskGuid));
            Func<int, TraceLevel, string, Task> func;
            Func<Tuple<BlobIdentifier, BlobIdentifier>, Task> action = (Func<Tuple<BlobIdentifier, BlobIdentifier>, Task>) (async range =>
            {
              await this.BlobMetadataDomainProvider.GetMetadataProvider(vssProcessor.SecuredDomainRequest).FixMetadataAfterDisasterAsync((VssRequestPump.Processor) vssProcessor, result, this.DomainProvider.GetDomain(vssProcessor.SecuredDomainRequest), range.Item1, range.Item2, lastSyncTime, skipDeletion, func ?? (func = (Func<int, TraceLevel, string, Task>) ((tracepoint, traceLevel, message) => (Task) vssProcessor.ExecuteWorkAsync<int>((Func<IVssRequestContext, int>) (requestCntxt =>
              {
                requestCntxt.TraceAlways(tracepoint, traceLevel, this.traceData.Area, this.traceData.Layer, message);
                return 0;
              })))), requestContext.CancellationToken);
              await vssProcessor.TraceAlwaysAsync(ContentTracePoints.AdminPlatformBlobstore.FixMetadataAfterDisasterAsyncRangeFinish, string.Format("Range {0} to {1} finished. Stats: {2}. (Id: {3})", (object) range.Item1, (object) range.Item2, (object) JsonSerializer.Serialize<FixMetadataAfterDisasterResults>(result), (object) taskGuid));
            });
            await NonSwallowingActionBlock.Create<Tuple<BlobIdentifier, BlobIdentifier>>(action, new ExecutionDataflowBlockOptions()
            {
              MaxDegreeOfParallelism = parallelism,
              CancellationToken = requestContext.CancellationToken
            }).SendAllAndCompleteSingleBlockNetworkAsync<Tuple<BlobIdentifier, BlobIdentifier>>(this.Get256BlobIdRanges().Where<Tuple<BlobIdentifier, BlobIdentifier>>(closure_4 ?? (closure_4 = (Func<Tuple<BlobIdentifier, BlobIdentifier>, bool>) (range => startingBlobId == (BlobIdentifier) null || string.Compare(range.Item2.AlgorithmResultString, startingBlobId.AlgorithmResultString, StringComparison.Ordinal) > 0))), requestContext.CancellationToken);
          }));
          afterDisasterResults = result;
        }
        catch (Exception ex)
        {
          requestContext.TraceException(ContentTracePoints.AdminPlatformBlobstore.FixMetadataAfterDisasterAsyncException, ex);
          string str = JsonSerializer.Serialize<FixMetadataAfterDisasterResults>(result);
          requestContext.TraceInfo(ContentTracePoints.AdminPlatformBlobstore.FixMetadataAfterDisasterAsyncResult, "result: " + str);
          throw new ApplicationException(string.Format("{0} failed. Partial Result: {1}. (Id: {2})", (object) nameof (FixMetadataAfterDisasterAsync), (object) str, (object) taskGuid), ex);
        }
      }
      return afterDisasterResults;
    }

    public async Task ServiceDeleteRetentionOnStorageAccounts(
      IVssRequestContext requestContext,
      DeleteRetentionConfigurationJobInfo jobInfo,
      PhysicalDomainInfo physicalDomainInfo = null)
    {
      string[] array = this.GetAzureConnectionStrings(requestContext, physicalDomainInfo).Select<StrongBoxConnectionString, string>((Func<StrongBoxConnectionString, string>) (c => c.ConnectionString)).ToArray<string>();
      DeleteRetentionOperationMode runMode = jobInfo.JobRunMode;
      int requestedDeleteRetentionInDays = jobInfo.DeleteRetentionRequestedInDays;
      jobInfo.TotalShardsDiscovered = array.Length;
      int totalShardsServiced = 0;
      await NonSwallowingActionBlock.Create<string>((Func<string, Task>) (async connectionString =>
      {
        CloudStorageAccount account;
        if (!CloudStorageAccount.TryParse(connectionString, out account))
          throw new Exception("Couldn't parse connection string from strong box. Aborting.");
        CloudBlobClient client = account.CreateCloudBlobClient();
        ServiceProperties servicePropertiesAsync = await client.GetServicePropertiesAsync();
        DeleteRetentionPolicy deleteRetentionPolicy1 = servicePropertiesAsync.DeleteRetentionPolicy;
        string storageAccount = account.BlobEndpoint.Host;
        bool initialRetentionState = deleteRetentionPolicy1.Enabled;
        int? initialDeleteRetentionInDays = deleteRetentionPolicy1.RetentionDays;
        switch (runMode)
        {
          case DeleteRetentionOperationMode.Query:
            requestContext.TraceAlways(ContentTracePoints.AdminPlatformBlobstore.ServiceDeleteRetentionOnStorageAccountsInfo, "Account: " + storageAccount + " " + string.Format("Queried retention state : {0} ", (object) initialRetentionState) + string.Format("Queried retention days: {0}", (object) initialDeleteRetentionInDays));
            break;
          case DeleteRetentionOperationMode.EnableOrUpdate:
            deleteRetentionPolicy1.Enabled = true;
            deleteRetentionPolicy1.RetentionDays = new int?(requestedDeleteRetentionInDays);
            await client.SetServicePropertiesAsync(servicePropertiesAsync);
            DeleteRetentionPolicy deleteRetentionPolicy2 = (await client.GetServicePropertiesAsync()).DeleteRetentionPolicy;
            requestContext.TraceAlways(ContentTracePoints.AdminPlatformBlobstore.ServiceDeleteRetentionOnStorageAccountsInfo, "Account: " + storageAccount + " " + string.Format("Transitioned retention state from: {0} to {1} ", (object) initialRetentionState, (object) deleteRetentionPolicy2.Enabled) + string.Format("Retention Days from: {0} to {1}.", (object) initialDeleteRetentionInDays, (object) deleteRetentionPolicy2.RetentionDays));
            break;
          case DeleteRetentionOperationMode.Disable:
            deleteRetentionPolicy1.Enabled = false;
            deleteRetentionPolicy1.RetentionDays = new int?(0);
            await client.SetServicePropertiesAsync(servicePropertiesAsync);
            DeleteRetentionPolicy deleteRetentionPolicy3 = (await client.GetServicePropertiesAsync()).DeleteRetentionPolicy;
            requestContext.TraceAlways(ContentTracePoints.AdminPlatformBlobstore.ServiceDeleteRetentionOnStorageAccountsInfo, "Account: " + storageAccount + " " + string.Format("Transitioned retention state from {0} to {1}", (object) initialRetentionState, (object) deleteRetentionPolicy3.Enabled) + string.Format("Queried retention days: {0}", (object) initialDeleteRetentionInDays));
            break;
          default:
            throw new InvalidEnumArgumentException(string.Format("Unknown retention run mode encountered {0}", (object) runMode));
        }
        Interlocked.Increment(ref totalShardsServiced);
        account = (CloudStorageAccount) null;
        client = (CloudBlobClient) null;
        storageAccount = (string) null;
      }), requestContext.CancellationToken).PostAllToUnboundedAndCompleteAsync<string>((IEnumerable<string>) array, requestContext.CancellationToken);
      jobInfo.TotalShardsServiced = totalShardsServiced;
    }

    public async Task AccountForSoftDeletedBytesFromContainersAsync(
      IVssRequestContext requestContext,
      SoftDeletedRetentionJobPartitionedInfo jobInfo,
      IEnumerable<string> shardConnectionStrings,
      int cpuThreshold,
      bool CPUThrottlingDisabled)
    {
      long totalFileDedupSoftDeletedBlobs = 0;
      long totalFileDedupSoftDeletedBytes = 0;
      long totalChunkDedupSoftDeletedBlobs = 0;
      long totalChunkDedupSoftDeletedBytes = 0;
      int totalShardCount = shardConnectionStrings.Count<string>();
      int totalShardsScanned = 0;
      int totalThrottlingInSecs = 0;
      Func<string, Task> action1 = (Func<string, Task>) (async shardConnection =>
      {
        if (!CPUThrottlingDisabled)
        {
          int num = await CpuThrottleHelper.Instance.Yield(cpuThreshold, requestContext.CancellationToken).ConfigureAwait(false);
          Interlocked.Add(ref totalThrottlingInSecs, num);
        }
        CloudStorageAccount account;
        if (!CloudStorageAccount.TryParse(shardConnection, out account))
          throw new Exception("Unable to determine the storage account: " + SecretUtility.ScrubSecrets(shardConnection));
        string host = account.BlobEndpoint.Host;
        jobInfo.Result.ShardNames.Add(host);
        Interlocked.Increment(ref totalShardsScanned);
        try
        {
          ICloudBlobClientWrapper cloudBlobClient = new CloudStorageAccountWrapper(shardConnection).CreateCloudBlobClient();
          HashSet<CloudBlobContainer> source = this.FetchBlobContainers(requestContext, cloudBlobClient, BlobStoreProviderConstants.BlobContainerPrefix);
          requestContext.TraceAlways(ContentTracePoints.AdminPlatformBlobstore.AccountForSoftDeletedBytesFromContainersAsyncInfo, string.Format("Shard: [{0}] listed {1} file dedup containers.", (object) host, (object) source.Count<CloudBlobContainer>()));
          HashSet<CloudBlobContainer> chunkDedupBlobContainers = this.FetchBlobContainers(requestContext, cloudBlobClient, BlobStoreProviderConstants.DedupContainerPrefix);
          requestContext.TraceAlways(ContentTracePoints.AdminPlatformBlobstore.AccountForSoftDeletedBytesFromContainersAsyncInfo, string.Format("Shard: [{0}] listed {1} chunk dedup containers.", (object) host, (object) chunkDedupBlobContainers.Count<CloudBlobContainer>()));
          long fileBlobCount = 0;
          long fileBlobDeletedSize = 0;
          int fileBlobContainersCount;
          Stopwatch stw;
          ConfiguredTaskAwaitable configuredTaskAwaitable;
          TimeSpan elapsed;
          if (source.Any<CloudBlobContainer>())
          {
            fileBlobContainersCount = source.Count;
            int num1 = Math.Min(fileBlobContainersCount, 4 * Environment.ProcessorCount);
            Func<CloudBlobContainer, Task> action2 = (Func<CloudBlobContainer, Task>) (async container =>
            {
              BlobContinuationToken currentToken = (BlobContinuationToken) null;
              do
              {
                Microsoft.Azure.Storage.Blob.BlobResultSegment blobResultSegment = await container.ListBlobsSegmentedAsync((string) null, true, BlobListingDetails.Deleted, new int?(), currentToken, (BlobRequestOptions) null, (OperationContext) null, requestContext.CancellationToken);
                currentToken = blobResultSegment.ContinuationToken;
                if (blobResultSegment.Results != null)
                {
                  foreach (IListBlobItem result in blobResultSegment.Results)
                  {
                    if (result is CloudBlockBlob cloudBlockBlob2 && cloudBlockBlob2.IsDeleted)
                    {
                      Interlocked.Increment(ref fileBlobCount);
                      Interlocked.Add(ref fileBlobDeletedSize, cloudBlockBlob2.Properties.Length);
                      int? beforePermanentDelete = cloudBlockBlob2.Properties.RemainingDaysBeforePermanentDelete;
                      if (beforePermanentDelete.HasValue)
                      {
                        LogHistogram expirationInDays = jobInfo.Result.FileSoftDeletedExpirationInDays;
                        beforePermanentDelete = cloudBlockBlob2.Properties.RemainingDaysBeforePermanentDelete;
                        double num2 = (double) beforePermanentDelete.Value;
                        expirationInDays.IncrementCount(num2);
                      }
                    }
                  }
                }
              }
              while (currentToken != null);
            });
            ActionBlock<CloudBlobContainer> targetBlock = NonSwallowingActionBlock.Create<CloudBlobContainer>(action2, new ExecutionDataflowBlockOptions()
            {
              MaxDegreeOfParallelism = num1,
              CancellationToken = requestContext.CancellationToken,
              EnsureOrdered = false
            });
            stw = Stopwatch.StartNew();
            HashSet<CloudBlobContainer> inputs = source;
            CancellationToken cancellationToken = requestContext.CancellationToken;
            configuredTaskAwaitable = targetBlock.PostAllToUnboundedAndCompleteAsync<CloudBlobContainer>((IEnumerable<CloudBlobContainer>) inputs, cancellationToken).ConfigureAwait(true);
            await configuredTaskAwaitable;
            stw.Stop();
            object[] objArray = new object[4]
            {
              (object) stw.Elapsed.Hours,
              (object) stw.Elapsed.Minutes,
              (object) stw.Elapsed.Seconds,
              null
            };
            elapsed = stw.Elapsed;
            objArray[3] = (object) elapsed.Milliseconds;
            string str = string.Format("{0:00}:{1:00}:{2:00}.{3}", objArray);
            requestContext.TraceInfo(ContentTracePoints.AdminPlatformBlobstore.AccountForSoftDeletedBytesFromContainersAsyncListed, string.Format("Listed delete retained blobs for [{0}] file dedup containers in {1}", (object) fileBlobContainersCount, (object) str));
            stw = (Stopwatch) null;
          }
          long chunkBlobCount = 0;
          long chunkBlobDeletedSize = 0;
          if (chunkDedupBlobContainers.Any<CloudBlobContainer>())
          {
            fileBlobContainersCount = chunkDedupBlobContainers.Count;
            int num3 = Math.Min(fileBlobContainersCount, 4 * Environment.ProcessorCount);
            Func<CloudBlobContainer, Task> action3 = (Func<CloudBlobContainer, Task>) (async container =>
            {
              BlobContinuationToken currentToken = (BlobContinuationToken) null;
              do
              {
                Microsoft.Azure.Storage.Blob.BlobResultSegment blobResultSegment = await container.ListBlobsSegmentedAsync((string) null, true, BlobListingDetails.Deleted, new int?(), currentToken, (BlobRequestOptions) null, (OperationContext) null, requestContext.CancellationToken);
                currentToken = blobResultSegment.ContinuationToken;
                if (blobResultSegment.Results != null)
                {
                  foreach (IListBlobItem result in blobResultSegment.Results)
                  {
                    if (result is CloudBlockBlob cloudBlockBlob4 && cloudBlockBlob4.IsDeleted)
                    {
                      Interlocked.Increment(ref chunkBlobCount);
                      Interlocked.Add(ref chunkBlobDeletedSize, cloudBlockBlob4.Properties.Length);
                      int? beforePermanentDelete = cloudBlockBlob4.Properties.RemainingDaysBeforePermanentDelete;
                      if (beforePermanentDelete.HasValue)
                      {
                        LogHistogram expirationInDays = jobInfo.Result.ChunkSoftDeletedExpirationInDays;
                        beforePermanentDelete = cloudBlockBlob4.Properties.RemainingDaysBeforePermanentDelete;
                        double num4 = (double) beforePermanentDelete.Value;
                        expirationInDays.IncrementCount(num4);
                      }
                    }
                  }
                }
              }
              while (currentToken != null);
            });
            ActionBlock<CloudBlobContainer> targetBlock = NonSwallowingActionBlock.Create<CloudBlobContainer>(action3, new ExecutionDataflowBlockOptions()
            {
              MaxDegreeOfParallelism = num3,
              CancellationToken = requestContext.CancellationToken,
              EnsureOrdered = false
            });
            stw = Stopwatch.StartNew();
            HashSet<CloudBlobContainer> inputs = chunkDedupBlobContainers;
            CancellationToken cancellationToken = requestContext.CancellationToken;
            configuredTaskAwaitable = targetBlock.PostAllToUnboundedAndCompleteAsync<CloudBlobContainer>((IEnumerable<CloudBlobContainer>) inputs, cancellationToken).ConfigureAwait(true);
            await configuredTaskAwaitable;
            stw.Stop();
            object[] objArray = new object[4];
            elapsed = stw.Elapsed;
            objArray[0] = (object) elapsed.Hours;
            elapsed = stw.Elapsed;
            objArray[1] = (object) elapsed.Minutes;
            elapsed = stw.Elapsed;
            objArray[2] = (object) elapsed.Seconds;
            elapsed = stw.Elapsed;
            objArray[3] = (object) elapsed.Milliseconds;
            string str = string.Format("{0:00}:{1:00}:{2:00}.{3}", objArray);
            requestContext.TraceInfo(ContentTracePoints.AdminPlatformBlobstore.AccountForSoftDeletedBytesFromContainersAsyncListed, string.Format("Listed delete retained blobs for [{0}] chunk dedup containers in {1}", (object) fileBlobContainersCount, (object) str));
            stw = (Stopwatch) null;
          }
          Interlocked.Add(ref totalFileDedupSoftDeletedBlobs, fileBlobCount);
          Interlocked.Add(ref totalFileDedupSoftDeletedBytes, fileBlobDeletedSize);
          Interlocked.Add(ref totalChunkDedupSoftDeletedBlobs, chunkBlobCount);
          Interlocked.Add(ref totalChunkDedupSoftDeletedBytes, chunkBlobDeletedSize);
          chunkDedupBlobContainers = (HashSet<CloudBlobContainer>) null;
        }
        catch (Exception ex)
        {
          requestContext.TraceException(ContentTracePoints.AdminPlatformBlobstore.AccountForSoftDeletedBytesFromContainersAsyncException, ex);
          throw;
        }
      });
      ExecutionDataflowBlockOptions dataflowBlockOptions = new ExecutionDataflowBlockOptions();
      dataflowBlockOptions.MaxDegreeOfParallelism = totalShardCount;
      dataflowBlockOptions.CancellationToken = requestContext.CancellationToken;
      await NonSwallowingActionBlock.Create<string>(action1, dataflowBlockOptions).PostAllToUnboundedAndCompleteAsync<string>(shardConnectionStrings, requestContext.CancellationToken).ConfigureAwait(true);
      jobInfo.Result.TotalShards = totalShardCount;
      jobInfo.Result.TotalShardsScanned = totalShardsScanned;
      jobInfo.Result.TotalThrottleDuration = TimeSpan.FromSeconds((double) totalThrottlingInSecs);
      jobInfo.Result.TotalFileDedupSoftDeletedBlobs = totalFileDedupSoftDeletedBlobs;
      jobInfo.Result.TotalFileDedupSoftDeletedBytes = totalFileDedupSoftDeletedBytes;
      jobInfo.Result.TotalChunkDedupSoftDeletedBlobs = totalChunkDedupSoftDeletedBlobs;
      jobInfo.Result.TotalChunkDedupSoftDeletedBytes = totalChunkDedupSoftDeletedBytes;
    }

    private HashSet<CloudBlobContainer> FetchBlobContainers(
      IVssRequestContext requestContext,
      ICloudBlobClientWrapper cloudBlobClientWrapper,
      string prefixFilter)
    {
      BlobContinuationToken currentToken = (BlobContinuationToken) null;
      HashSet<CloudBlobContainer> collection = new HashSet<CloudBlobContainer>();
      Stopwatch stopwatch = Stopwatch.StartNew();
      do
      {
        IContainerResultSegmentWrapper resultSegmentWrapper = cloudBlobClientWrapper.ListContainersSegmented(prefixFilter, ContainerListingDetails.None, new int?(), currentToken);
        currentToken = resultSegmentWrapper.ContinuationToken;
        if (resultSegmentWrapper.Results != null)
          collection.AddRange<CloudBlobContainer, HashSet<CloudBlobContainer>>(resultSegmentWrapper.Results);
      }
      while (currentToken != null);
      stopwatch.Stop();
      object[] objArray = new object[4]
      {
        (object) stopwatch.Elapsed.Hours,
        null,
        null,
        null
      };
      TimeSpan elapsed = stopwatch.Elapsed;
      objArray[1] = (object) elapsed.Minutes;
      elapsed = stopwatch.Elapsed;
      objArray[2] = (object) elapsed.Seconds;
      elapsed = stopwatch.Elapsed;
      objArray[3] = (object) elapsed.Milliseconds;
      string str = string.Format("{0:00}:{1:00}:{2:00}.{3}", objArray);
      requestContext.TraceInfo(ContentTracePoints.AdminPlatformBlobstore.FetchBlobContainersInfo, string.Format("Listed {0} for prefix {1} in {2}", (object) collection.Count, (object) prefixFilter, (object) str));
      return collection;
    }

    public async Task CollectStorageDataFromContainersAsync(
      IVssRequestContext requestContext,
      IClock clock,
      FileStorageDataJobInfo jobInfo)
    {
      AdminPlatformBlobStore platformBlobStore = this;
      ConcurrentBag<PhysicalDataJobResult> resultCollection = new ConcurrentBag<PhysicalDataJobResult>();
      using (requestContext.Enter(ContentTracePoints.AdminPlatformBlobstore.CollectStorageDataFromContainersAsyncCall, nameof (CollectStorageDataFromContainersAsync)))
      {
        try
        {
          SecuredDomainRequest domainRequest = new SecuredDomainRequest(requestContext, DomainIdFactory.Create(jobInfo.DomainId));
          List<Microsoft.VisualStudio.Services.BlobStore.Server.Common.IBlobProvider> providers = platformBlobStore.DomainProvider.GetDomain((ISecuredDomainRequest) domainRequest).GetAllProviders().ToList<Microsoft.VisualStudio.Services.BlobStore.Server.Common.IBlobProvider>();
          string prefix = BlobStoreUtils.GeneratePrefix(jobInfo.TotalPartitions, jobInfo.PartitionId);
          if (jobInfo.MaxParallelism == 0)
            jobInfo.MaxParallelism = 4;
          await platformBlobStore.PumpOrInlineFromAsync(requestContext, (ISecuredDomainRequest) domainRequest, (Func<VssRequestPump.SecuredDomainProcessor, Task>) (async processor =>
          {
            Func<Microsoft.VisualStudio.Services.BlobStore.Server.Common.IBlobProvider, Task> action = (Func<Microsoft.VisualStudio.Services.BlobStore.Server.Common.IBlobProvider, Task>) (async blobProvider =>
            {
              int numRetried = 0;
              bool isRetryRequired;
              PhysicalDataJobResult result;
              do
              {
                isRetryRequired = false;
                result = new PhysicalDataJobResult();
                try
                {
                  using (IConcurrentIterator<IEnumerable<BasicBlobMetadata>> pageEnumerator = blobProvider.GetBasicBlobMetadataConcurrentIterator((VssRequestPump.Processor) processor, prefix))
                  {
                    while (true)
                    {
                      if (await pageEnumerator.MoveNextAsync(processor.CancellationToken).ConfigureAwait(false))
                      {
                        foreach (BasicBlobMetadata blockBlob in pageEnumerator.Current)
                          result.AddBlockBlobMetadata(blockBlob, clock);
                        result.ThrottledTime += TimeSpan.FromSeconds((double) await CpuThrottleHelper.Instance.Yield(jobInfo.CpuThreshold, requestContext.CancellationToken).ConfigureAwait(false));
                      }
                      else
                        break;
                    }
                  }
                }
                catch (Exception ex)
                {
                  isRetryRequired = AsyncHttpRetryHelper.IsTransientException(ex, processor.CancellationToken) && ++numRetried < 10 && !processor.CancellationToken.IsCancellationRequested;
                  if (!isRetryRequired)
                    throw;
                  await Task.Delay(JobHelper.RetryInterval).ConfigureAwait(false);
                }
              }
              while (isRetryRequired);
              result.NumRetried = numRetried;
              resultCollection.Add(result);
              result = (PhysicalDataJobResult) null;
            });
            await NonSwallowingActionBlock.Create<Microsoft.VisualStudio.Services.BlobStore.Server.Common.IBlobProvider>(action, new ExecutionDataflowBlockOptions()
            {
              MaxDegreeOfParallelism = jobInfo.MaxParallelism,
              CancellationToken = processor.CancellationToken
            }).PostAllToUnboundedAndCompleteAsync<Microsoft.VisualStudio.Services.BlobStore.Server.Common.IBlobProvider>((IEnumerable<Microsoft.VisualStudio.Services.BlobStore.Server.Common.IBlobProvider>) providers, processor.CancellationToken).ConfigureAwait(true);
          })).ConfigureAwait(true);
          jobInfo.Result.AddStorageDataJobResult((IEnumerable<PhysicalDataJobResult>) resultCollection);
          jobInfo.Result.ThrottledTime = TimeSpan.FromTicks((long) resultCollection.Average<PhysicalDataJobResult>((Func<PhysicalDataJobResult, long>) (r => r.ThrottledTime.Ticks)));
        }
        catch (Exception ex)
        {
          requestContext.TraceException(ContentTracePoints.AdminPlatformBlobstore.CollectStorageDataFromContainersAsyncException, ex);
          throw;
        }
      }
    }

    public async Task<bool> UpdateBlobLengthForExistingMetadataAsync(
      VssRequestPump.SecuredDomainProcessor processor,
      IBlobMetadataSizeInfo blobMetadataSizeInfo)
    {
      AdminPlatformBlobStore platformBlobStore = this;
      BlobIdentifier blobId = blobMetadataSizeInfo.BlobId;
      bool isMetadataUpdated = false;
      long? blobLength = await platformBlobStore.DomainProvider.GetDomain(processor.SecuredDomainRequest).FindProvider(blobId).GetBlobLengthAsync((VssRequestPump.Processor) processor, blobId).ConfigureAwait(false);
      if (blobLength.HasValue)
      {
        for (int attempt = 0; attempt < 20; ++attempt)
        {
          IBlobMetadata blobMetadata = await platformBlobStore.BlobMetadataDomainProvider.GetMetadataProvider(processor.SecuredDomainRequest).GetAsync((VssRequestPump.Processor) processor, blobId).ConfigureAwait(false);
          if (blobMetadata.StoredReferenceState == BlobReferenceState.AddedBlob)
          {
            blobMetadata.BlobLength = blobLength;
            blobMetadata.DesiredReferenceState = blobMetadata.StoredReferenceState;
            isMetadataUpdated = await platformBlobStore.BlobMetadataDomainProvider.GetMetadataProvider(processor.SecuredDomainRequest).TryUpdateAsync((VssRequestPump.Processor) processor, blobMetadata).ConfigureAwait(false);
            if (!isMetadataUpdated)
            {
              await Task.Delay(100).ConfigureAwait(false);
            }
            else
            {
              blobMetadataSizeInfo.BlobLength = blobMetadata.BlobLength;
              break;
            }
          }
          else
            break;
        }
      }
      bool flag = isMetadataUpdated;
      blobId = (BlobIdentifier) null;
      return flag;
    }

    public IConcurrentIterator<IBlobMetadataSizeInfo> GetBlobMetadataSizeConcurrentIterator(
      VssRequestPump.SecuredDomainProcessor processor,
      BlobIdentifier startingBlobIdOrNull,
      BlobIdentifier endingBlobIdOrNull,
      IteratorPartition partition,
      bool excludeStartId,
      IEnumerable<IBlobIdReferenceRowVisitor> idBlobReferenceVisitors)
    {
      return this.BlobMetadataDomainProvider.GetMetadataProvider(processor.SecuredDomainRequest).GetBlobMetadataSizeConcurrentIterator((VssRequestPump.Processor) processor, startingBlobIdOrNull, endingBlobIdOrNull, partition, excludeStartId, idBlobReferenceVisitors);
    }

    public IConcurrentIterator<IBlobMetadataProjectScopedSizeInfo> GetBlobMetadataSizeProjectScopedConcurrentIterator(
      VssRequestPump.SecuredDomainProcessor processor,
      BlobIdentifier startingBlobIdOrNull,
      BlobIdentifier endingBlobIdOrNull,
      IteratorPartition partition,
      bool excludeStartId,
      bool enableFeedInfoExport)
    {
      return this.BlobMetadataDomainProvider.GetMetadataProvider(processor.SecuredDomainRequest).GetBlobMetadataProjectScopedSizeConcurrentIterator((VssRequestPump.Processor) processor, startingBlobIdOrNull, endingBlobIdOrNull, partition, true, enableFeedInfoExport);
    }

    public IConcurrentIterator<BlobReferenceDetailInfo> GetAllReferencesConcurrentIterator(
      VssRequestPump.SecuredDomainProcessor processor)
    {
      return this.BlobMetadataDomainProvider.GetMetadataProvider(processor.SecuredDomainRequest).GetAllReferencesConcurrentIterator((VssRequestPump.Processor) processor);
    }

    internal async Task<long?> CheckForDeleteAsync(
      IVssRequestContext requestContext,
      IDomainId domainId,
      BlobIdentifier blobId)
    {
      AdminPlatformBlobStore platformBlobStore = this;
      SecuredDomainRequest domainRequest = new SecuredDomainRequest(requestContext, domainId);
      long? result = new long?();
      await platformBlobStore.PumpOrInlineFromAsync(requestContext, (ISecuredDomainRequest) domainRequest, (Func<VssRequestPump.SecuredDomainProcessor, Task>) (async processor => result = await this.CheckForDeleteAsync(processor, this.DomainProvider.GetDomain(processor.SecuredDomainRequest).FindProvider(blobId), blobId)));
      return result;
    }

    public async Task CheckForDeletionAsync(
      IVssRequestContext requestContext,
      IDomainId domainId,
      DeletionJobInfo jobInfo,
      BlobIdsForPartition blobIds,
      IteratorPartition partition,
      bool CPUThrottlingDisabled)
    {
      jobInfo.Result.StartBlobId = blobIds.StartValue.ValueString;
      this.UpdateServicePointSettings(requestContext);
      await requestContext.PumpFromAsync((ISecuredDomainRequest) new SecuredDomainRequest(requestContext, domainId), (Func<VssRequestPump.SecuredDomainProcessor, Task>) (async vssProcessor =>
      {
        DateTime expiredUntil = this.Clock.Now.UtcDateTime - jobInfo.ClockBuffer;
        Stopwatch stopwatch = Stopwatch.StartNew();
        ConfiguredTaskAwaitable configuredTaskAwaitable = this.CheckForDeletionAsync(vssProcessor, blobIds, blobIds.StartValue, blobIds.MaxValue, partition, stopwatch, expiredUntil, jobInfo, CPUThrottlingDisabled).ConfigureAwait(false);
        await configuredTaskAwaitable;
        if (jobInfo.Result.Exception == null && stopwatch.Elapsed < jobInfo.TimeBudget)
        {
          configuredTaskAwaitable = this.CheckForDeletionAsync(vssProcessor, blobIds, blobIds.MinValue, blobIds.StartValue, partition, stopwatch, expiredUntil, jobInfo, CPUThrottlingDisabled).ConfigureAwait(false);
          await configuredTaskAwaitable;
        }
        jobInfo.Result.JobCompletePerMille = jobInfo.Result.Exception != null || !(stopwatch.Elapsed < jobInfo.TimeBudget) ? blobIds.CalculateJobPerMille(jobInfo.Result.CurrentBlobId) : 1000;
        stopwatch = (Stopwatch) null;
      })).ConfigureAwait(false);
    }

    public void UpdateServicePointSettings(IVssRequestContext requestContext)
    {
      if (requestContext.IsSystemContext && !requestContext.ExecutionEnvironment.IsOnPremisesDeployment)
      {
        IEnumerable<string> source = this.GetAzureConnectionStrings(requestContext, (PhysicalDomainInfo) null).Select<StrongBoxConnectionString, string>((Func<StrongBoxConnectionString, string>) (cs => cs.ConnectionString));
        foreach (string connectionString in source)
        {
          CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(connectionString);
          ServicePointManager.FindServicePoint(cloudStorageAccount.BlobStorageUri.PrimaryUri).UpdateServicePointSettings(ServicePointConstants.MaxConnectionsPerProc64);
          ServicePointManager.FindServicePoint(cloudStorageAccount.TableStorageUri.PrimaryUri).UpdateServicePointSettings(ServicePointConstants.MaxConnectionsPerProc64);
        }
        requestContext.TraceInfo(ContentTracePoints.AdminPlatformBlobstore.UpdateServicePointSettingsInfo, string.Format("Updated ServicePoint Settings for {0} accounts", (object) source.Count<string>()));
      }
      else
        requestContext.TraceInfo(ContentTracePoints.AdminPlatformBlobstore.UpdateServicePointSettingsInfo, string.Format("Skipping Update of ServicePoint Settings. IsOnPremises: {0}", (object) requestContext.ExecutionEnvironment.IsOnPremisesDeployment));
    }

    public async Task CheckForDeletionAsync(
      IVssRequestContext requestContext,
      IDomainId domainId,
      DeletionJobInfo jobInfo,
      BlobIdentifier startBlobIdOrNull = null,
      bool CPUThrottlingDisabled = false)
    {
      BlobIdsForPartition blobIds = BlobIdsForPartition.Create((byte) 0, 1, startBlobIdOrNull);
      using (requestContext.Enter(ContentTracePoints.AdminPlatformBlobstore.CheckForDeletionAsyncCall, nameof (CheckForDeletionAsync)))
        await this.CheckForDeletionAsync(requestContext, domainId, jobInfo, blobIds, (IteratorPartition) null, CPUThrottlingDisabled).ConfigureAwait(true);
    }

    private async Task CheckAndDeleteExpiredBlob(
      VssRequestPump.SecuredDomainProcessor vssProcessor,
      BlobIdentifier blobId,
      DateTime expiryTime,
      DeletionJobResult resultRef)
    {
      AdminPlatformBlobStore platformBlobStore = this;
      Interlocked.Increment(ref resultRef.NumConcurrentDeletes);
      Microsoft.VisualStudio.Services.BlobStore.Server.Common.IBlobProvider provider = platformBlobStore.DomainProvider.GetDomain(vssProcessor.SecuredDomainRequest).FindProvider(blobId);
      try
      {
        long? nullable = await platformBlobStore.CheckForDeleteAsync(vssProcessor, provider, blobId).ConfigureAwait(false);
        if (nullable.HasValue)
        {
          resultRef.LogDeletion(nullable.Value);
          resultRef.LogExpiredDeletion(platformBlobStore.Clock, expiryTime);
        }
        else
          resultRef.LogKept();
      }
      catch (Exception ex)
      {
        resultRef.LogFailure("BlobDeleteError: " + ex.GetType().Name + ": " + ex.Message);
      }
      Interlocked.Decrement(ref resultRef.NumConcurrentDeletes);
    }

    private async Task CheckForDeletionAsync(
      VssRequestPump.SecuredDomainProcessor vssProcessor,
      BlobIdsForPartition blobIds,
      BlobIdentifier startBlobId,
      BlobIdentifier endBlobId,
      IteratorPartition partition,
      Stopwatch stopwatch,
      DateTime expiredUntil,
      DeletionJobInfo jobInfo,
      bool CPUThrottlingDisabled)
    {
      AdminPlatformBlobStore platformBlobStore = this;
      DeletionJobResult resultRef = jobInfo.Result;
      ExecutionDataflowBlockOptions dataflowBlockOptions = new ExecutionDataflowBlockOptions();
      dataflowBlockOptions.BoundedCapacity = 10000;
      dataflowBlockOptions.MaxDegreeOfParallelism = jobInfo.MaxConcurrency;
      dataflowBlockOptions.CancellationToken = vssProcessor.CancellationToken;
      ExecutionDataflowBlockOptions expiredBlobDeletionDataflowOptions = dataflowBlockOptions;
      resultRef.CurrentBlobId = startBlobId;
      await vssProcessor.TraceInfoAsync(ContentTracePoints.AdminPlatformBlobstore.CheckForDeletionAsyncStartDelete, string.Format("P[{0}]: Starting Deletion Iteration ", (object) blobIds.Partition) + "from blobId: " + resultRef.CurrentBlobId.ValueString.Substring(0, 12) + ", " + string.Format("with TotalPartitons: {0}, ", (object) resultRef.TotalPartitions) + string.Format("and Concurrency: {0}", (object) jobInfo.MaxConcurrency)).ConfigureAwait(false);
      long lastCheckTime = 0;
      long queueLengthSum = 0;
      long queueLengthCount = 0;
      bool retryRequired;
      ConfiguredTaskAwaitable configuredTaskAwaitable;
      do
      {
        retryRequired = false;
        try
        {
          ActionBlock<KeyValuePair<BlobIdentifier, DateTime>> queue = NonSwallowingActionBlock.Create<KeyValuePair<BlobIdentifier, DateTime>>((Func<KeyValuePair<BlobIdentifier, DateTime>, Task>) (async blobKvp => await this.CheckAndDeleteExpiredBlob(vssProcessor, blobKvp.Key, blobKvp.Value, resultRef).ConfigureAwait(false)), expiredBlobDeletionDataflowOptions);
          IConcurrentIterator<KeyValuePair<BlobIdentifier, DateTime>> blobEnum = platformBlobStore.BlobMetadataDomainProvider.GetMetadataProvider(vssProcessor.SecuredDomainRequest).GetBlobIdentifiersWithExpiredKeepUntilConcurrentIterator((VssRequestPump.Processor) vssProcessor, resultRef.CurrentBlobId, endBlobId, partition, expiredUntil);
          try
          {
            while (true)
            {
              do
              {
                if (await blobEnum.MoveNextAsync(vssProcessor.CancellationToken).ConfigureAwait(false))
                {
                  if (stopwatch.Elapsed <= jobInfo.TimeBudget)
                  {
                    if (!CPUThrottlingDisabled)
                    {
                      DeletionJobResult deletionJobResult1 = resultRef;
                      DeletionJobResult deletionJobResult = deletionJobResult1;
                      int numJobThrottled = deletionJobResult1.NumJobThrottled;
                      deletionJobResult.NumJobThrottled = numJobThrottled + await CpuThrottleHelper.Instance.Yield(resultRef.CpuThreshold, vssProcessor.CancellationToken).ConfigureAwait(false);
                      deletionJobResult = (DeletionJobResult) null;
                    }
                    configuredTaskAwaitable = queue.SendOrThrowSingleBlockNetworkAsync<KeyValuePair<BlobIdentifier, DateTime>>(blobEnum.Current, vssProcessor.CancellationToken).ConfigureAwait(false);
                    await configuredTaskAwaitable;
                    resultRef.CurrentBlobId = blobEnum.Current.Key;
                  }
                  else
                    goto label_18;
                }
                else
                  goto label_18;
              }
              while (stopwatch.ElapsedMilliseconds - lastCheckTime <= 3600000L);
              queueLengthSum += Interlocked.Read(ref resultRef.NumConcurrentDeletes);
              ++queueLengthCount;
              lastCheckTime = stopwatch.ElapsedMilliseconds;
              int jobPerMille = blobIds.CalculateJobPerMille(resultRef.CurrentBlobId);
              configuredTaskAwaitable = vssProcessor.TraceInfoAsync(ContentTracePoints.AdminPlatformBlobstore.CheckForDeletionAsyncProcessingBlobId, string.Format("P[{0}]: Processing ", (object) blobIds.Partition) + "blobId: " + resultRef.CurrentBlobId.ValueString.Substring(0, 12) + ", " + string.Format("PerMille: {0}, ", (object) jobPerMille) + string.Format("BlobsDeleted: {0}, ", (object) resultRef.BlobsDeleted) + string.Format("BlobsKept: {0}, ", (object) resultRef.BlobsKept) + string.Format("AvgConcurrentDeletes: {0}", (object) (queueLengthSum / queueLengthCount))).ConfigureAwait(false);
              await configuredTaskAwaitable;
            }
          }
          finally
          {
            blobEnum?.Dispose();
          }
label_18:
          blobEnum = (IConcurrentIterator<KeyValuePair<BlobIdentifier, DateTime>>) null;
          queue.Complete();
          await queue.Completion.ConfigureAwait(false);
          queue = (ActionBlock<KeyValuePair<BlobIdentifier, DateTime>>) null;
        }
        catch (Exception ex)
        {
          retryRequired = (ex is ExpandedStorageException || ex is ExpandedTableStorageException) && !vssProcessor.CancellationToken.IsCancellationRequested && ++resultRef.NumJobRetried < 10 && stopwatch.Elapsed < jobInfo.TimeBudget;
          await Task.Delay(JobHelper.RetryInterval).ConfigureAwait(false);
          resultRef.Exception = retryRequired ? (Exception) null : ex;
          configuredTaskAwaitable = vssProcessor.TraceInfoAsync(ContentTracePoints.AdminPlatformBlobstore.CheckForDeletionAsyncException, "Exception: " + ex.GetType().Name + ", " + ex.Message).ConfigureAwait(false);
          await configuredTaskAwaitable;
        }
      }
      while (retryRequired);
      resultRef.EndBlobId = resultRef.CurrentBlobId.ValueString;
      resultRef.AvgNumConcurrentDeletes = queueLengthCount > 0L ? 1L + queueLengthSum / queueLengthCount : 0L;
      configuredTaskAwaitable = vssProcessor.TraceInfoAsync(ContentTracePoints.AdminPlatformBlobstore.CheckForDeletionAsyncCompleted, string.Format("P[{0}]: Completed Deletion Iteration from blobId: {1} to {2}", (object) blobIds.Partition, (object) startBlobId.ValueString.Substring(0, 12), (object) resultRef.EndBlobId.Substring(0, 12))).ConfigureAwait(false);
      await configuredTaskAwaitable;
      expiredBlobDeletionDataflowOptions = (ExecutionDataflowBlockOptions) null;
    }

    private IEnumerable<Tuple<BlobIdentifier, BlobIdentifier>> Get256BlobIdRanges()
    {
      List<Tuple<BlobIdentifier, BlobIdentifier>> tupleList = new List<Tuple<BlobIdentifier, BlobIdentifier>>();
      for (int index1 = 0; index1 <= (int) byte.MaxValue; ++index1)
      {
        byte[] algorithmResult1 = new byte[32];
        byte[] algorithmResult2 = new byte[32];
        algorithmResult1[0] = (byte) index1;
        algorithmResult2[0] = (byte) index1;
        for (int index2 = 1; index2 < 32; ++index2)
        {
          algorithmResult1[index2] = (byte) 0;
          algorithmResult2[index2] = byte.MaxValue;
        }
        tupleList.Add(new Tuple<BlobIdentifier, BlobIdentifier>(BlobIdentifier.CreateFromAlgorithmResult(algorithmResult1), BlobIdentifier.CreateFromAlgorithmResult(algorithmResult2)));
      }
      return (IEnumerable<Tuple<BlobIdentifier, BlobIdentifier>>) tupleList;
    }
  }
}
