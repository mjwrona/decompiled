// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.AdminDedupStoreService
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D3AB5C9B-EB54-4477-A304-63BB297414A3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.Server.dll

using BuildXL.Cache.ContentStore.Hashing;
using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.BlobStore.Common;
using Microsoft.VisualStudio.Services.BlobStore.Server.Common;
using Microsoft.VisualStudio.Services.BlobStore.Server.Common.DedupGC;
using Microsoft.VisualStudio.Services.BlobStore.Server.Common.DedupGC.KeepUntil;
using Microsoft.VisualStudio.Services.BlobStore.Server.Common.DedupUtility;
using Microsoft.VisualStudio.Services.BlobStore.Server.Common.Domains;
using Microsoft.VisualStudio.Services.BlobStore.Server.DedupUtility;
using Microsoft.VisualStudio.Services.Content.Common;
using Microsoft.VisualStudio.Services.Content.Server.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace Microsoft.VisualStudio.Services.BlobStore.Server
{
  public class AdminDedupStoreService : 
    DedupStoreService,
    IAdminDedupStore,
    IDedupStore,
    IVssFrameworkService
  {
    public const int DefaultPageSize = 50;

    public virtual IConcurrentIterator<IEnumerable<DedupMetadataEntry>> GetRootPages(
      VssRequestPump.SecuredDomainProcessor processor,
      DedupMetadataPageRetrievalOption option,
      Microsoft.VisualStudio.Services.Content.Server.Common.Tracer tracer)
    {
      return this.DomainDedupStoreProvider.GetProvider(processor.SecuredDomainRequest).GetRootPagesAsync((VssRequestPump.Processor) processor, option);
    }

    public async Task HardDeleteRootAsync(
      VssRequestPump.SecuredDomainProcessor processor,
      DedupMetadataEntry dedupMetadataEntry)
    {
      AdminDedupStoreService dedupStoreService = this;
      IdBlobReference rootRef = new IdBlobReference(dedupMetadataEntry.ReferenceId, dedupMetadataEntry.Scope);
      await dedupStoreService.DomainDedupStoreProvider.GetProvider(processor.SecuredDomainRequest).HardDeleteRootAsync(processor, dedupMetadataEntry.DedupId, rootRef).ConfigureAwait(false);
    }

    public async Task<long> RestoreDedupTree(
      VssRequestPump.SecuredDomainProcessor processor,
      DedupIdentifier dedupId,
      Microsoft.VisualStudio.Services.Content.Server.Common.Tracer tracer)
    {
      AdminDedupStoreService dedupStoreService = this;
      DedupTraversalConfig config = new DedupTraversalConfig()
      {
        NoCache = true,
        EnableDiagnostics = true
      };
      return await new DedupRestoringVisitor(processor, (IDedupInfoRetriever) dedupStoreService, (IDedupRestorer) dedupStoreService, config, new Func<TraceLevel, string, Task>(log)).RestoreAsync(dedupId).ConfigureAwait(false);

      async Task log(TraceLevel traceLevel, string message) => await processor.ExecuteWorkAsync((Action<IVssRequestContext>) (rc =>
      {
        Microsoft.VisualStudio.Services.Content.Server.Common.Tracer tracer1 = tracer;
        if (tracer1 == null)
          return;
        tracer1.ToTraceAction()(traceLevel, message);
      })).ConfigureAwait(false);
    }

    public async Task<DedupPhysicalStorageData> CollectStorageDataForPrefixAsync(
      VssRequestPump.SecuredDomainProcessor processor,
      IteratorPartition partition,
      string prefix,
      int cpuThreshold,
      IClock clock,
      bool CPUthrottlingDisabled)
    {
      AdminDedupStoreService dedupStoreService = this;
      int numRetried = 0;
      bool isRetryRequired;
      DedupPhysicalStorageData result;
      do
      {
        isRetryRequired = false;
        result = new DedupPhysicalStorageData();
        try
        {
          IConcurrentIterator<BasicBlobMetadata> blobEnumerator = dedupStoreService.DomainDedupStoreProvider.GetProvider(processor.SecuredDomainRequest).GetBasicBlobMetadataConcurrentIterator(processor, partition, prefix);
          try
          {
            while (true)
            {
              do
              {
                if (await blobEnumerator.MoveNextAsync(processor.CancellationToken).ConfigureAwait(false))
                  result.AddBlockBlobMetadata(blobEnumerator.Current, clock);
                else
                  goto label_11;
              }
              while (CPUthrottlingDisabled);
              int num = await CpuThrottleHelper.Instance.Yield(cpuThreshold, processor.CancellationToken).ConfigureAwait(false);
            }
          }
          finally
          {
            blobEnumerator?.Dispose();
          }
label_11:
          blobEnumerator = (IConcurrentIterator<BasicBlobMetadata>) null;
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
      DedupPhysicalStorageData physicalStorageData = result;
      result = (DedupPhysicalStorageData) null;
      return physicalStorageData;
    }

    public async Task CollectStorageDataFromContainersAsync(
      VssRequestPump.SecuredDomainProcessor processor,
      DedupDataJobInfo jobInfo,
      IClock clock,
      bool enablePrefixScopedScan,
      bool CPUthrottlingDisabled)
    {
      if (enablePrefixScopedScan)
      {
        string prefix = jobInfo.Prefix = BlobStoreUtils.GeneratePrefix(jobInfo.TotalPartitions, jobInfo.PartitionId);
        DedupPhysicalStorageData physicalStorageData = await this.CollectStorageDataForPrefixAsync(processor, new IteratorPartition(jobInfo.PartitionId, jobInfo.TotalPartitions, PartitionStrategy.All), prefix, jobInfo.CpuThreshold, clock, CPUthrottlingDisabled).ConfigureAwait(false);
        jobInfo.Result.AccumulateDedupStorageData((IEnumerable<DedupPhysicalStorageData>) new DedupPhysicalStorageData[1]
        {
          physicalStorageData
        });
      }
      else
      {
        IteratorPartition partition = new IteratorPartition(jobInfo.PartitionId, jobInfo.TotalPartitions);
        int numPrefixes = jobInfo.TotalPartitions > 1 ? 256 : 1;
        IEnumerable<int> inputs = Enumerable.Range(0, numPrefixes);
        Func<int, Task> action = (Func<int, Task>) (async prefixNumber =>
        {
          string prefix = BlobStoreUtils.GeneratePrefix(numPrefixes, prefixNumber);
          DedupPhysicalStorageData physicalStorageData = await this.CollectStorageDataForPrefixAsync(processor, partition, prefix, jobInfo.CpuThreshold, clock, CPUthrottlingDisabled).ConfigureAwait(false);
          jobInfo.Result.AccumulateDedupStorageData((IEnumerable<DedupPhysicalStorageData>) new DedupPhysicalStorageData[1]
          {
            physicalStorageData
          });
        });
        ExecutionDataflowBlockOptions dataflowBlockOptions = new ExecutionDataflowBlockOptions();
        dataflowBlockOptions.MaxDegreeOfParallelism = Math.Min(numPrefixes, 4 * Environment.ProcessorCount);
        dataflowBlockOptions.CancellationToken = processor.CancellationToken;
        await NonSwallowingActionBlock.Create<int>(action, dataflowBlockOptions).PostAllToUnboundedAndCompleteAsync<int>(inputs, processor.CancellationToken).ConfigureAwait(true);
      }
      jobInfo.IsCompleteResult = true;
    }

    public async Task CollectStorageDataFromContainersAsync(
      IVssRequestContext requestContext,
      DedupDataJobInfo jobInfo,
      IClock clock,
      bool enablePrefixScopedScan,
      bool CPUthrottlingDisabled)
    {
      AdminDedupStoreService dedupStoreService = this;
      using (Microsoft.VisualStudio.Services.Content.Server.Common.Tracer tracer = Microsoft.VisualStudio.Services.Content.Server.Common.Tracer.Enter(requestContext, dedupStoreService.traceData, 5701210, nameof (CollectStorageDataFromContainersAsync)))
      {
        try
        {
          await dedupStoreService.PumpOrInlineFromAsync(requestContext, DomainIdFactory.Create(jobInfo.DomainId), (Func<VssRequestPump.SecuredDomainProcessor, Task>) (async processor => await this.CollectStorageDataFromContainersAsync(processor, jobInfo, clock, enablePrefixScopedScan, CPUthrottlingDisabled).ConfigureAwait(true))).ConfigureAwait(true);
        }
        catch (Exception ex)
        {
          tracer.TraceException(ex);
          throw;
        }
      }
    }

    public virtual async Task<MarkResult> MarkRootsAsync(
      IVssRequestContext requestContext1,
      ChunkDedupGCCheckpoint checkpointData,
      IDomainId domainId,
      DateTimeOffset expiryKeepUntil)
    {
      MarkResult markResult1;
      using (requestContext1.Enter(ContentTracePoints.DedupStoreService.MarkRootsAsyncCall, nameof (MarkRootsAsync)))
      {
        if (checkpointData == null)
          throw new ArgumentNullException(nameof (checkpointData));
        SecuredDomainRequest domainRequest = new SecuredDomainRequest(requestContext1, domainId);
        IVssRegistryService service = requestContext1.GetService<IVssRegistryService>();
        int num = service.GetValue<int>(requestContext1, (RegistryQuery) "/Configuration/BlobStore/ConcurrentIteratorCapacity", true, 10);
        DedupMetadataPageRetrievalOption option = new DedupMetadataPageRetrievalOption(string.Empty, new DateTimeOffset?(), new DateTimeOffset?(), ResultArrangement.AllOrdered, 50, StateFilter.Active, (IDomainId) null);
        option.BoundedCapacity = num;
        option.StartingKey = checkpointData.CurrentDedupIdentifier ?? string.Empty;
        MarkResult markResult = new MarkResult();
        markResult.MarkTime = expiryKeepUntil;
        bool skipPage = false;
        int degreeOfParallelism = service.GetValue<int>(requestContext1, (RegistryQuery) ServiceRegistryConstants.ChunkDedupGarbageCollectionParallelismPath, true, 10);
        await requestContext1.PumpFromAsync((ISecuredDomainRequest) domainRequest, (Func<VssRequestPump.SecuredDomainProcessor, Task>) (async processor =>
        {
          IConcurrentIterator<IEnumerable<DedupMetadataEntry>> rootPagesAsync = this.DomainDedupStoreProvider.GetProvider(processor.SecuredDomainRequest).GetRootPagesAsync((VssRequestPump.Processor) processor, option);
          Stopwatch stopwatch = Stopwatch.StartNew();
          CancellationToken cancellationToken = processor.CancellationToken;
          Func<DedupMetadataEntry, Task> func;
          Func<IEnumerable<DedupMetadataEntry>, Task> loopBodyAsync = (Func<IEnumerable<DedupMetadataEntry>, Task>) (async page =>
          {
            markResult.ResumedFromDedupId = page.First<DedupMetadataEntry>().DedupId.ValueString;
            if (page.Last<DedupMetadataEntry>().DedupId.ValueString.CompareTo(checkpointData.CurrentDedupIdentifier) < 0)
            {
              markResult.TotalRoots += (long) page.Count<DedupMetadataEntry>();
              skipPage = true;
            }
            else
              skipPage = false;
            if (skipPage)
              ;
            else
            {
              await Task.WhenAll((IEnumerable<Task>) page.AsParallel<DedupMetadataEntry>().WithDegreeOfParallelism<DedupMetadataEntry>(degreeOfParallelism).Select<DedupMetadataEntry, Task>(func ?? (func = (Func<DedupMetadataEntry, Task>) (async entry =>
              {
                VisitDedupResult visitDedupResult = await this.VisitDedupsTopDownAsync(processor, entry, expiryKeepUntil);
                lock (markResult)
                {
                  markResult.NodesVisited += visitDedupResult.NodesVisited;
                  markResult.NonChunksVisited += visitDedupResult.NonChunksVisited;
                  markResult.CachedNodes += visitDedupResult.CachedNodes;
                  markResult.AlreadyMarkedNodes += visitDedupResult.AlreadyMarkedNodes;
                  if (visitDedupResult.MissingNodes.IsNullOrEmpty<DedupIdentifier>())
                    return;
                  markResult.MissingNodes += (long) visitDedupResult.MissingNodes.Count;
                  if (checkpointData == null)
                    return;
                  lock (checkpointData)
                    checkpointData.MissingDedups.UnionWith((IEnumerable<DedupIdentifier>) visitDedupResult.MissingNodes);
                }
              }))));
              markResult.TotalRoots += (long) page.Count<DedupMetadataEntry>();
              await processor.ExecuteWorkAsync((Action<IVssRequestContext>) (requestContext3 => checkpointData.SaveCheckpoint(requestContext3, page.LastOrDefault<DedupMetadataEntry>()?.DedupId.ToString())));
              if (stopwatch.ElapsedMilliseconds <= 1800000L)
                ;
              else
              {
                requestContext1.TraceAlways(ContentTracePoints.DedupStoreService.MarkRootsAsyncInfo, JsonSerializer.Serialize<MarkResult>(markResult));
                stopwatch.Restart();
              }
            }
          });
          await rootPagesAsync.ForEachAsyncCaptureContext<IEnumerable<DedupMetadataEntry>>(cancellationToken, loopBodyAsync);
          stopwatch.Stop();
        }));
        requestContext1.TraceAlways(ContentTracePoints.DedupStoreService.MarkRootsAsyncInfo, JsonSerializer.Serialize<MarkResult>(markResult));
        markResult1 = markResult.MissingNodes <= 0L ? markResult : throw new InvalidOperationException("Found missing dedup node(s). Aborting further validation or deletion.");
      }
      return markResult1;
    }

    private async Task<VisitDedupResult> VisitDedupsTopDownAsync(
      VssRequestPump.SecuredDomainProcessor processor,
      DedupMetadataEntry entry,
      DateTimeOffset expiryKeepUntil)
    {
      AdminDedupStoreService dedupStoreService = this;
      Dictionary<DedupIdentifier, DedupNode> nodeCache = new Dictionary<DedupIdentifier, DedupNode>();
      HashSet<DedupIdentifier> alreadyMarked = new HashSet<DedupIdentifier>();
      Stack<DedupIdentifier> dedupIdentifiers = new Stack<DedupIdentifier>();
      long nodesVisited = 0;
      long nonChunksVisited = 0;
      long cachedNodes = 0;
      long alreadyMarkedNodes = 0;
      DedupIdentifier rootId = entry.DedupId;
      dedupIdentifiers.Push(rootId);
      HashSet<DedupIdentifier> missingNodes = new HashSet<DedupIdentifier>();
      while (dedupIdentifiers.Any<DedupIdentifier>())
      {
        ++nodesVisited;
        DedupIdentifier currentDedupId = dedupIdentifiers.Pop();
        if (!alreadyMarked.Contains(currentDedupId))
        {
          // ISSUE: explicit non-virtual call
          if (await __nonvirtual (dedupStoreService.TryModifyKeepUntilAsync(processor, currentDedupId, (IKeepUntil) new ConstantKeepUntil(expiryKeepUntil), true)) == MetadataOperationResult.Missing)
          {
            string msg = "Cannot retrieve dedup node for " + currentDedupId.ValueString + " with rootId " + rootId.ValueString + ". The node is missing. DedupMetadataEntry: " + JsonSerializer.Serialize<DedupMetadataEntry>(entry);
            await processor.ExecuteWorkAsync((Action<IVssRequestContext>) (requestContext => requestContext.TraceAlways(ContentTracePoints.DedupStoreService.MissingNodeException, msg)));
            missingNodes.Add(currentDedupId);
            continue;
          }
          alreadyMarked.Add(currentDedupId);
        }
        else
          ++alreadyMarkedNodes;
        if (ChunkerHelper.IsNode(currentDedupId.AlgorithmId))
        {
          ++nonChunksVisited;
          DedupNode? nullable = new DedupNode?();
          DedupNode dedupNode;
          if (nodeCache.TryGetValue(currentDedupId, out dedupNode))
          {
            nullable = new DedupNode?(dedupNode);
            ++cachedNodes;
          }
          else
          {
            // ISSUE: explicit non-virtual call
            nullable = await __nonvirtual (dedupStoreService.GetDedupNodeAsync(processor, currentDedupId.CastToNodeDedupIdentifier()));
            nodeCache.Add(currentDedupId, nullable.Value);
          }
          // ISSUE: reference to a compiler-generated field
          // ISSUE: reference to a compiler-generated field
          foreach (DedupIdentifier dedupIdentifier in nullable.Value.ChildNodes.Select<DedupNode, DedupIdentifier>(AdminDedupStoreService.\u003C\u003EO.\u003C0\u003E__Create ?? (AdminDedupStoreService.\u003C\u003EO.\u003C0\u003E__Create = new Func<DedupNode, DedupIdentifier>(DedupIdentifier.Create))).ToList<DedupIdentifier>())
            dedupIdentifiers.Push(dedupIdentifier);
        }
        currentDedupId = (DedupIdentifier) null;
      }
      await processor.ExecuteWorkAsync((Action<IVssRequestContext>) (requestContext => requestContext.TraceInfo(ContentTracePoints.DedupStoreService.VisitDedupsTopDownAsyncInfo, string.Format("RootId: {0}, Nodes visited: {1}, Non-chunk visited: {2}, CachedNodes: {3}, AlreadyMarkedNodes: {4}", (object) rootId, (object) nodesVisited, (object) nonChunksVisited, (object) cachedNodes, (object) alreadyMarkedNodes))));
      VisitDedupResult visitDedupResult = new VisitDedupResult(rootId, nodesVisited, nonChunksVisited, cachedNodes, alreadyMarkedNodes, missingNodes);
      nodeCache = (Dictionary<DedupIdentifier, DedupNode>) null;
      alreadyMarked = (HashSet<DedupIdentifier>) null;
      dedupIdentifiers = (Stack<DedupIdentifier>) null;
      missingNodes = (HashSet<DedupIdentifier>) null;
      return visitDedupResult;
    }

    public async Task<DeleteResult> DeleteExpiredDedupsAsync(
      IVssRequestContext requestContext,
      ChunkDedupGCCheckpoint checkpointData,
      IDomainId domainId,
      DateTimeOffset expiryKeepUntil)
    {
      DeleteResult deleteResult = new DeleteResult();
      int boundedCapacity = requestContext.GetService<IVssRegistryService>().GetValue<int>(requestContext, (RegistryQuery) "/Configuration/BlobStore/ConcurrentIteratorCapacity", true, 10);
      await requestContext.PumpFromAsync((ISecuredDomainRequest) new SecuredDomainRequest(requestContext, domainId), (Func<VssRequestPump.SecuredDomainProcessor, Task>) (async processor => deleteResult = await this.DomainDedupStoreProvider.GetProvider(processor.SecuredDomainRequest).DeleteExpiredDedupsAsync(processor, checkpointData, expiryKeepUntil, boundedCapacity))).ConfigureAwait(false);
      return deleteResult;
    }

    public async Task<DedupsValidationResult> VerifyRootsAsync(
      IVssRequestContext requestContext1,
      ChunkDedupGCCheckpoint checkpointData,
      IDomainId domainId,
      DateTimeOffset keepUntil)
    {
      DedupsValidationResult validationResult;
      using (requestContext1.Enter(ContentTracePoints.DedupStoreService.VerifyRootsAsyncCall, nameof (VerifyRootsAsync)))
      {
        if (checkpointData == null)
          throw new ArgumentNullException(nameof (checkpointData));
        SecuredDomainRequest domainRequest = new SecuredDomainRequest(requestContext1, domainId);
        HashSet<DedupIdentifier> missingDedups = new HashSet<DedupIdentifier>();
        HashSet<DedupIdentifier> invalidKUDedups = new HashSet<DedupIdentifier>();
        long rootsProcessed = 0;
        int num = requestContext1.GetService<IVssRegistryService>().GetValue<int>(requestContext1, (RegistryQuery) "/Configuration/BlobStore/ConcurrentIteratorCapacity", true, 10);
        DedupMetadataPageRetrievalOption option = new DedupMetadataPageRetrievalOption(string.Empty, new DateTimeOffset?(), new DateTimeOffset?(), ResultArrangement.AllOrdered, 50, StateFilter.Active, (IDomainId) null);
        option.BoundedCapacity = num;
        option.StartingKey = checkpointData.CurrentDedupIdentifier ?? string.Empty;
        IVssRegistryService service = requestContext1.GetService<IVssRegistryService>();
        bool skipPage = false;
        int degreeOfParallelism = service.GetValue<int>(requestContext1, (RegistryQuery) ServiceRegistryConstants.ChunkDedupGarbageCollectionParallelismPath, true, 10);
        await requestContext1.PumpFromAsync((ISecuredDomainRequest) domainRequest, (Func<VssRequestPump.SecuredDomainProcessor, Task>) (async processor =>
        {
          IConcurrentIterator<IEnumerable<DedupMetadataEntry>> rootPagesAsync = this.DomainDedupStoreProvider.GetProvider(processor.SecuredDomainRequest).GetRootPagesAsync((VssRequestPump.Processor) processor, option);
          Stopwatch stopwatch = Stopwatch.StartNew();
          CancellationToken cancellationToken = processor.CancellationToken;
          Func<DedupMetadataEntry, Task> func;
          Func<IEnumerable<DedupMetadataEntry>, Task> loopBodyAsync = (Func<IEnumerable<DedupMetadataEntry>, Task>) (async page =>
          {
            if (page.Last<DedupMetadataEntry>().DedupId.ValueString.CompareTo(checkpointData.CurrentDedupIdentifier) < 0)
            {
              skipPage = true;
              rootsProcessed += (long) page.Count<DedupMetadataEntry>();
            }
            else
              skipPage = false;
            if (skipPage)
              ;
            else
            {
              await Task.WhenAll((IEnumerable<Task>) page.AsParallel<DedupMetadataEntry>().WithDegreeOfParallelism<DedupMetadataEntry>(degreeOfParallelism).Select<DedupMetadataEntry, Task>(func ?? (func = (Func<DedupMetadataEntry, Task>) (async entry =>
              {
                (HashSet<DedupIdentifier> other3, HashSet<DedupIdentifier> other4) = await this.ValidateKeepUntilAsync(processor, entry, keepUntil);
                lock (missingDedups)
                {
                  lock (invalidKUDedups)
                  {
                    missingDedups.UnionWith((IEnumerable<DedupIdentifier>) other3);
                    invalidKUDedups.UnionWith((IEnumerable<DedupIdentifier>) other4);
                  }
                }
              }))));
              if (checkpointData != null)
              {
                checkpointData.MissingDedups = missingDedups;
                checkpointData.InvalidKeepUntilDedupsCount = invalidKUDedups.Count;
              }
              rootsProcessed += (long) page.Count<DedupMetadataEntry>();
              await processor.ExecuteWorkAsync((Action<IVssRequestContext>) (requestContext3 => checkpointData.SaveCheckpoint(requestContext3, page.LastOrDefault<DedupMetadataEntry>()?.DedupId.ToString())));
              if (stopwatch.ElapsedMilliseconds <= 1800000L)
                ;
              else
              {
                IVssRequestContext context = requestContext1;
                SingleLocationTracePoint verifyRootsAsyncInfo = ContentTracePoints.DedupStoreService.VerifyRootsAsyncInfo;
                object[] objArray3 = new object[4]
                {
                  (object) keepUntil,
                  (object) rootsProcessed,
                  null,
                  null
                };
                ChunkDedupGCCheckpoint dedupGcCheckpoint3 = checkpointData;
                objArray3[2] = (object) (dedupGcCheckpoint3 != null ? dedupGcCheckpoint3.MissingDedups.Count : 0);
                ChunkDedupGCCheckpoint dedupGcCheckpoint4 = checkpointData;
                objArray3[3] = (object) (dedupGcCheckpoint4 != null ? dedupGcCheckpoint4.InvalidKeepUntilDedupsCount : 0);
                string messageFormat = string.Format("Initial MarkTime: {0}. Num Roots Verified: {1}, Missing Dedup Count: {2}, Dedups with Invalid KUs : {3}", objArray3);
                object[] objArray4 = Array.Empty<object>();
                context.TraceAlways(verifyRootsAsyncInfo, messageFormat, objArray4);
                stopwatch.Restart();
              }
            }
          });
          await rootPagesAsync.ForEachAsyncNoContext<IEnumerable<DedupMetadataEntry>>(cancellationToken, loopBodyAsync);
          stopwatch.Stop();
        }));
        IVssRequestContext context1 = requestContext1;
        SingleLocationTracePoint verifyRootsAsyncInfo1 = ContentTracePoints.DedupStoreService.VerifyRootsAsyncInfo;
        object[] objArray5 = new object[4]
        {
          (object) keepUntil,
          (object) rootsProcessed,
          null,
          null
        };
        ChunkDedupGCCheckpoint dedupGcCheckpoint5 = checkpointData;
        objArray5[2] = (object) (dedupGcCheckpoint5 != null ? dedupGcCheckpoint5.MissingDedups.Count : 0);
        ChunkDedupGCCheckpoint dedupGcCheckpoint6 = checkpointData;
        objArray5[3] = (object) (dedupGcCheckpoint6 != null ? dedupGcCheckpoint6.InvalidKeepUntilDedupsCount : 0);
        string messageFormat1 = string.Format("Initial MarkTime: {0}. Num Roots Verified: {1}, Missing Dedup Count: {2}, Dedups with Invalid KUs : {3}", objArray5);
        object[] objArray6 = Array.Empty<object>();
        context1.TraceAlways(verifyRootsAsyncInfo1, messageFormat1, objArray6);
        validationResult = checkpointData == null ? new DedupsValidationResult(missingDedups, invalidKUDedups) : new DedupsValidationResult(checkpointData.MissingDedups, invalidKUDedups);
      }
      return validationResult;
    }

    private async Task<(HashSet<DedupIdentifier>, HashSet<DedupIdentifier>)> ValidateKeepUntilAsync(
      VssRequestPump.SecuredDomainProcessor processor,
      DedupMetadataEntry entry,
      DateTimeOffset keepUntil)
    {
      AdminDedupStoreService dedupStoreService = this;
      Dictionary<DedupIdentifier, DedupNode> nodeCache = new Dictionary<DedupIdentifier, DedupNode>();
      HashSet<DedupIdentifier> alreadyVerified = new HashSet<DedupIdentifier>();
      Stack<DedupIdentifier> dedupIdentifiers = new Stack<DedupIdentifier>();
      HashSet<DedupIdentifier> missingDedups = new HashSet<DedupIdentifier>();
      HashSet<DedupIdentifier> invalidKUDedups = new HashSet<DedupIdentifier>();
      DedupIdentifier rootId = entry.DedupId;
      dedupIdentifiers.Push(rootId);
      while (dedupIdentifiers.Any<DedupIdentifier>())
      {
        DedupIdentifier currentDedupId = dedupIdentifiers.Pop();
        if (!alreadyVerified.Contains(currentDedupId))
        {
          // ISSUE: explicit non-virtual call
          IDedupInfo dedupInfo = await __nonvirtual (dedupStoreService.GetDedupInfoAsync(processor, currentDedupId));
          if (dedupInfo.Status == HealthStatus.Absent || dedupInfo.Status == HealthStatus.MissingMetadata)
          {
            // ISSUE: explicit non-virtual call
            if (await __nonvirtual (dedupStoreService.RestoreIfNotExists(processor, currentDedupId)))
            {
              string msg = string.Format("Initial MarkTime: {0}. Recovered missing node for {1} with rootId {2}. DedupMetadataEntry: {3}", (object) keepUntil, (object) currentDedupId.ValueString, (object) rootId.ValueString, (object) JsonSerializer.Serialize<DedupMetadataEntry>(entry));
              await processor.ExecuteWorkAsync((Action<IVssRequestContext>) (requestContext => requestContext.TraceAlways(ContentTracePoints.DedupStoreService.MissingNodeRecovered, msg)));
            }
            else
            {
              string msg = string.Format("Initial MarkTime: {0}. Cannot recover dedup node for {1} with rootId {2}. The node is missing. DedupMetadataEntry: {3}", (object) keepUntil, (object) currentDedupId.ValueString, (object) rootId.ValueString, (object) JsonSerializer.Serialize<DedupMetadataEntry>(entry));
              await processor.ExecuteWorkAsync((Action<IVssRequestContext>) (requestContext => requestContext.TraceAlways(ContentTracePoints.DedupStoreService.MissingNodeException, msg)));
              missingDedups.Add(currentDedupId);
              continue;
            }
          }
          DateTime? keepUntil1 = dedupInfo.KeepUntil;
          DateTimeOffset? nullable = keepUntil1.HasValue ? new DateTimeOffset?((DateTimeOffset) keepUntil1.GetValueOrDefault()) : new DateTimeOffset?();
          DateTimeOffset dateTimeOffset = keepUntil;
          if ((nullable.HasValue ? (nullable.GetValueOrDefault() < dateTimeOffset ? 1 : 0) : 0) != 0)
          {
            invalidKUDedups.Add(currentDedupId);
            string msg = string.Format("Initial MarkTime: {0}. Dedup node {1} with rootId {2} has an invalid KeepUntil value. Marking KeepUntil: {3}, Dedup KeepUntil: {4}", (object) keepUntil, (object) currentDedupId.ValueString, (object) rootId.ValueString, (object) keepUntil, (object) dedupInfo.KeepUntil);
            await processor.ExecuteWorkAsync((Action<IVssRequestContext>) (requestContext => requestContext.TraceAlways(ContentTracePoints.DedupStoreService.InvalidKUDedupFound, msg)));
          }
          alreadyVerified.Add(currentDedupId);
          dedupInfo = (IDedupInfo) null;
        }
        if (ChunkerHelper.IsNode(currentDedupId.AlgorithmId))
        {
          DedupNode? nullable = new DedupNode?();
          DedupNode dedupNode;
          if (nodeCache.TryGetValue(currentDedupId, out dedupNode))
          {
            nullable = new DedupNode?(dedupNode);
          }
          else
          {
            // ISSUE: explicit non-virtual call
            nullable = await __nonvirtual (dedupStoreService.GetDedupNodeAsync(processor, currentDedupId.CastToNodeDedupIdentifier()));
            nodeCache.Add(currentDedupId, nullable.Value);
          }
          // ISSUE: reference to a compiler-generated field
          // ISSUE: reference to a compiler-generated field
          foreach (DedupIdentifier dedupIdentifier in nullable.Value.ChildNodes.Select<DedupNode, DedupIdentifier>(AdminDedupStoreService.\u003C\u003EO.\u003C0\u003E__Create ?? (AdminDedupStoreService.\u003C\u003EO.\u003C0\u003E__Create = new Func<DedupNode, DedupIdentifier>(DedupIdentifier.Create))).ToList<DedupIdentifier>())
            dedupIdentifiers.Push(dedupIdentifier);
        }
        currentDedupId = (DedupIdentifier) null;
      }
      (HashSet<DedupIdentifier>, HashSet<DedupIdentifier>) valueTuple = (missingDedups, invalidKUDedups);
      nodeCache = (Dictionary<DedupIdentifier, DedupNode>) null;
      alreadyVerified = (HashSet<DedupIdentifier>) null;
      dedupIdentifiers = (Stack<DedupIdentifier>) null;
      missingDedups = (HashSet<DedupIdentifier>) null;
      invalidKUDedups = (HashSet<DedupIdentifier>) null;
      rootId = (DedupIdentifier) null;
      return valueTuple;
    }

    public async Task<bool> UpdateRootSizeAsync(
      VssRequestPump.SecuredDomainProcessor processor,
      DedupMetadataEntry rootEntry)
    {
      AdminDedupStoreService dedupStoreService = this;
      bool flag = false;
      if (!rootEntry.Size.HasValue)
      {
        DedupMetadataEntry entry = new DedupMetadataEntry(rootEntry.DedupId, rootEntry.Scope, rootEntry.ReferenceId, rootEntry.IsSoftDeleted, rootEntry.StateChangeTime, new long?(await processor.ExecuteAsyncWorkAsync<long>((Func<IVssRequestContext, Task<long>>) (rq => this.GetDedupSizeAsync(processor, rootEntry.DedupId)))));
        await dedupStoreService.DomainDedupStoreProvider.GetProvider(processor.SecuredDomainRequest).UpdateRootSizeAsync(processor, entry);
        flag = true;
      }
      return flag;
    }
  }
}
