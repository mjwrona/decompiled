// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.DeleteDanglingPipelineArtifactRootsJob
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D3AB5C9B-EB54-4477-A304-63BB297414A3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.Server.dll

using BuildXL.Cache.ContentStore.Hashing;
using Microsoft.TeamFoundation.Build.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.BlobStore.Common;
using Microsoft.VisualStudio.Services.BlobStore.Server.Common;
using Microsoft.VisualStudio.Services.BlobStore.Server.Common.Domains;
using Microsoft.VisualStudio.Services.Content.Common;
using Microsoft.VisualStudio.Services.Content.Server.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace Microsoft.VisualStudio.Services.BlobStore.Server
{
  public class DeleteDanglingPipelineArtifactRootsJob : ArtifactsPartitionedWaitedJob
  {
    private const char ReferenceIdSeparator = '/';
    private const string TfsDropScopeName = "tfsdrop";
    private const string PipelineArtifactScopeName = "pipelineartifact";
    private const string RegistryDaysBeforeWhichRequestsAreValidated = "/DaysBeforeWhichRequestsAreValidated";
    private static readonly IEnumerable<int> ValidNumPartitions = (IEnumerable<int>) new int[4]
    {
      1,
      16,
      256,
      4096
    };
    private int daysBeforeWhichRequestsAreValidated = 7;

    protected override string RegistryBasePath => "/Configuration/BlobStore/DeleteDanglingPARootsJobPath";

    protected override Guid ParentJobId => Guid.Parse("0240D6D5-FA86-476B-B172-85D087D6F0F3");

    protected override string JobNamePrefix => "Delete dangling pipeline artifact roots child job - ";

    protected override (TraceData traceData, int tracePoint) TraceInfo => (new TraceData()
    {
      Area = "BlobStore",
      Layer = "Job"
    }, 1100118);

    public override bool IsValidPartitionSize(
      IVssRequestContext rc,
      int partitionSize,
      int numStorageAccounts)
    {
      return DeleteDanglingPipelineArtifactRootsJob.ValidNumPartitions.Contains<int>(partitionSize);
    }

    protected override async Task<string> RunJobAsync(
      IVssRequestContext requestContext,
      JobParameters jobParameters,
      Microsoft.VisualStudio.Services.Content.Server.Common.Tracer tracer)
    {
      return JsonSerializer.Serialize<DeleteDanglingPipelineArtifactRootsInfo>(await this.DeleteDanglingRootsAsync(requestContext, jobParameters, tracer).ConfigureAwait(true));
    }

    protected override VssJobResult AggregateChildJobResults(
      IVssRequestContext requestContext,
      IEnumerable<TeamFoundationJobHistoryEntry> successfulChildJobs,
      JobParameters jobParameters)
    {
      TeamFoundationJobExecutionResult result = TeamFoundationJobExecutionResult.Succeeded;
      int num = successfulChildJobs.Count<TeamFoundationJobHistoryEntry>();
      if (num == 0)
        return new VssJobResult(TeamFoundationJobExecutionResult.Failed, "None of the child jobs succeeded.");
      if (num < jobParameters.TotalPartitions)
        result = TeamFoundationJobExecutionResult.PartiallySucceeded;
      IEnumerable<DeleteDanglingPipelineArtifactRootsInfo> source1 = successfulChildJobs.Select<TeamFoundationJobHistoryEntry, DeleteDanglingPipelineArtifactRootsInfo>((Func<TeamFoundationJobHistoryEntry, DeleteDanglingPipelineArtifactRootsInfo>) (r => JsonSerializer.Deserialize<DeleteDanglingPipelineArtifactRootsInfo>(r.ResultMessage)));
      IEnumerable<DeleteDanglingPipelineArtifactRootsJobResult> source2 = source1.Select<DeleteDanglingPipelineArtifactRootsInfo, DeleteDanglingPipelineArtifactRootsJobResult>((Func<DeleteDanglingPipelineArtifactRootsInfo, DeleteDanglingPipelineArtifactRootsJobResult>) (r => r.Result));
      DeleteDanglingPipelineArtifactRootsAggregateInfo rootsAggregateInfo = new DeleteDanglingPipelineArtifactRootsAggregateInfo();
      rootsAggregateInfo.CpuThreshold = this.Settings.CpuThreshold;
      rootsAggregateInfo.TotalPartitions = jobParameters.TotalPartitions;
      rootsAggregateInfo.RunId = jobParameters.RunId;
      rootsAggregateInfo.TotalSucceededChildJobs = num;
      DeleteDanglingPipelineArtifactRootsInfo artifactRootsInfo = source1.FirstOrDefault<DeleteDanglingPipelineArtifactRootsInfo>();
      rootsAggregateInfo.AllowDeletionOfDanglingPARoots = artifactRootsInfo != null && artifactRootsInfo.AllowDeletionOfDanglingPARoots;
      rootsAggregateInfo.Result = new DeleteDanglingPipelineArtifactRootsJobResult()
      {
        TotalRootsDiscovered = source2.Sum<DeleteDanglingPipelineArtifactRootsJobResult>((Func<DeleteDanglingPipelineArtifactRootsJobResult, ulong>) (trd => trd.TotalRootsDiscovered)),
        TotalRootsEvaluated = source2.Sum<DeleteDanglingPipelineArtifactRootsJobResult>((Func<DeleteDanglingPipelineArtifactRootsJobResult, ulong>) (tre => tre.TotalRootsEvaluated)),
        TotalRootsDeleted = source2.Sum<DeleteDanglingPipelineArtifactRootsJobResult>((Func<DeleteDanglingPipelineArtifactRootsJobResult, long>) (trd => trd.TotalRootsDeleted)),
        TotalRootsDeletedSize = source2.Sum<DeleteDanglingPipelineArtifactRootsJobResult>((Func<DeleteDanglingPipelineArtifactRootsJobResult, long>) (trds => trds.TotalRootsDeletedSize)),
        TotalRootsFailedToDelete = source2.Sum<DeleteDanglingPipelineArtifactRootsJobResult>((Func<DeleteDanglingPipelineArtifactRootsJobResult, long>) (trfd => trfd.TotalRootsFailedToDelete)),
        TotalThrottleDuration = new TimeSpan(source2.Sum<DeleteDanglingPipelineArtifactRootsJobResult>((Func<DeleteDanglingPipelineArtifactRootsJobResult, long>) (ttd => ttd.TotalThrottleDuration.Ticks)))
      };
      DeleteDanglingPipelineArtifactRootsAggregateInfo dataContractObject = rootsAggregateInfo;
      return new VssJobResult(result, JsonSerializer.Serialize<DeleteDanglingPipelineArtifactRootsAggregateInfo>(dataContractObject));
    }

    private async Task<DeleteDanglingPipelineArtifactRootsInfo> DeleteDanglingRootsAsync(
      IVssRequestContext requestContext,
      JobParameters jobParameters,
      Microsoft.VisualStudio.Services.Content.Server.Common.Tracer tracer)
    {
      DeleteDanglingPipelineArtifactRootsJob artifactRootsJob = this;
      bool flag = requestContext.IsFeatureEnabled("BlobStore.Features.AllowDeletionOfDanglingPARoots");
      if (!flag)
        tracer.TraceInfo("Feature flag: AllowDeletionOfDanglingPARoots is not enabled. Running in What-If mode.");
      DeleteDanglingPipelineArtifactRootsInfo jobInfo = new DeleteDanglingPipelineArtifactRootsInfo()
      {
        MaxParallelism = artifactRootsJob.Settings.MaxParallelism,
        CpuThreshold = artifactRootsJob.Settings.CpuThreshold,
        TotalPartitions = jobParameters.TotalPartitions,
        PartitionId = jobParameters.PartitionId,
        DomainId = jobParameters.DomainId,
        RunId = jobParameters.RunId,
        AllowDeletionOfDanglingPARoots = flag
      };
      AdminDedupStoreService adminDedupStore = requestContext.GetService<AdminDedupStoreService>();
      IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
      artifactRootsJob.daysBeforeWhichRequestsAreValidated = service.GetValue<int>(requestContext, (RegistryQuery) (artifactRootsJob.RegistryBasePath + "/DaysBeforeWhichRequestsAreValidated"), false, 7);
      tracer.TraceInfo(string.Format("Validating all the entries retrieved from before: {0} days.", (object) artifactRootsJob.daysBeforeWhichRequestsAreValidated));
      bool CPUThrottlingDisabled = artifactRootsJob.IsCPUThrottlingDisabled(requestContext);
      bool isRetryRequired;
      do
      {
        isRetryRequired = false;
        try
        {
          string prefix = BlobStoreUtils.GeneratePrefix(jobParameters.TotalPartitions, jobParameters.PartitionId);
          await requestContext.PumpFromAsync((ISecuredDomainRequest) new SecuredDomainRequest(requestContext, DomainIdFactory.Create(jobParameters.DomainId)), (Func<VssRequestPump.SecuredDomainProcessor, Task>) (async processor => await this.DeleteDanglingRootsAsync(processor, (IAdminDedupStore) adminDedupStore, UtcClock.Instance, prefix, jobInfo, tracer, CPUThrottlingDisabled).ConfigureAwait(false)));
        }
        catch (Exception ex)
        {
          isRetryRequired = AsyncHttpRetryHelper.IsTransientException(ex, requestContext.CancellationToken) && ++jobInfo.JobRetryCount < 10 && !requestContext.CancellationToken.IsCancellationRequested;
          if (!isRetryRequired)
            throw;
          tracer.TraceException(ex);
          jobInfo.ErrorDetails = JobHelper.GetNestedExceptionMessage(ex);
          await Task.Delay(JobHelper.RetryInterval).ConfigureAwait(true);
        }
      }
      while (isRetryRequired);
      return jobInfo;
    }

    private async Task DeleteDanglingRootsAsync(
      VssRequestPump.SecuredDomainProcessor processor,
      IAdminDedupStore adminDedupStore,
      IClock clock,
      string prefix,
      DeleteDanglingPipelineArtifactRootsInfo jobInfo,
      Microsoft.VisualStudio.Services.Content.Server.Common.Tracer tracer,
      bool CPUThrottlingDisabled)
    {
      Func<DedupMetadataEntry, Task> action = (Func<DedupMetadataEntry, Task>) (async entry => await this.CheckAndDeleteRootAsync(processor, entry, jobInfo, tracer).ConfigureAwait(true));
      ExecutionDataflowBlockOptions dataflowBlockOptions = new ExecutionDataflowBlockOptions();
      dataflowBlockOptions.BoundedCapacity = Environment.ProcessorCount * 4;
      dataflowBlockOptions.MaxDegreeOfParallelism = Environment.ProcessorCount;
      dataflowBlockOptions.CancellationToken = processor.CancellationToken;
      dataflowBlockOptions.EnsureOrdered = false;
      ActionBlock<DedupMetadataEntry> pipeline = NonSwallowingActionBlock.Create<DedupMetadataEntry>(action, dataflowBlockOptions);
      DomainIdFactory.Create(jobInfo.DomainId);
      using (IConcurrentIterator<IEnumerable<DedupMetadataEntry>> rootPages = adminDedupStore.GetRootPages(processor, new DedupMetadataPageRetrievalOption(prefix, new DateTimeOffset?(), new DateTimeOffset?(clock.Now.AddDays((double) -this.daysBeforeWhichRequestsAreValidated)), ResultArrangement.AllUnordered, 0, StateFilter.All), tracer))
      {
        await rootPages.ForEachAsyncNoContext<IEnumerable<DedupMetadataEntry>>(processor.CancellationToken, (Func<IEnumerable<DedupMetadataEntry>, Task>) (async page =>
        {
          if (!CPUThrottlingDisabled)
          {
            int num = await CpuThrottleHelper.Instance.Yield(this.Settings.CpuThreshold, processor.CancellationToken);
            jobInfo.Result.TotalThrottleDuration += TimeSpan.FromSeconds((double) num);
          }
          jobInfo.Result.TotalRootsDiscovered += (ulong) page.LongCount<DedupMetadataEntry>();
          foreach (DedupMetadataEntry input in page.Where<DedupMetadataEntry>((Func<DedupMetadataEntry, bool>) (entry => entry != null)))
          {
            if (!input.IsSoftDeleted && (input.Scope.Equals("pipelineartifact") || input.Scope.Equals("tfsdrop")))
              await pipeline.SendOrThrowSingleBlockNetworkAsync<DedupMetadataEntry>(input, processor.CancellationToken);
            ++jobInfo.Result.TotalRootsEvaluated;
          }
        }));
        pipeline.Complete();
        await pipeline.Completion.ConfigureAwait(true);
      }
    }

    private async Task CheckAndDeleteRootAsync(
      VssRequestPump.SecuredDomainProcessor processor,
      DedupMetadataEntry entry,
      DeleteDanglingPipelineArtifactRootsInfo jobInfo,
      Microsoft.VisualStudio.Services.Content.Server.Common.Tracer tracer)
    {
      string[] source = entry.ReferenceId.Split('/');
      if (((IEnumerable<string>) source).Count<string>() != 3)
      {
        tracer.TraceError(string.Format("Invalid referenceId: {0} found for dedupId: {1}", (object) entry.ReferenceId, (object) entry.DedupId));
      }
      else
      {
        string projectId = source[0];
        int buildId;
        if (!int.TryParse(source[1], out buildId))
        {
          tracer.TraceError(string.Format("Invalid buildId: {0} found for dedupId: {1} with referenceId: {2}", (object) buildId, (object) entry.DedupId, (object) entry.ReferenceId));
        }
        else
        {
          int num = 0;
          try
          {
            Microsoft.TeamFoundation.Build.WebApi.Build result;
            await processor.ExecuteWorkAsync((Action<IVssRequestContext>) (requestContext => result = requestContext.GetClient<BuildHttpClient>().GetBuildAsync(projectId, buildId).Result));
          }
          catch (Exception ex) when (ex.InnerException is BuildNotFoundException)
          {
            num = 1;
          }
          catch (Exception ex)
          {
            tracer.TraceException(ex);
          }
          if (num != 1)
            ;
          else
            await processor.ExecuteWorkAsync<Task>((Func<IVssRequestContext, Task>) (async requestContext =>
            {
              DedupStoreService dedupStoreService = requestContext.GetService<DedupStoreService>();
              try
              {
                IDomainId domainId = DomainIdFactory.Create(jobInfo.DomainId);
                DedupNode? dedupNode = await dedupStoreService.GetDedupNodeAsync(processor, entry.DedupId.CastToNodeDedupIdentifier()).ConfigureAwait(true);
                if (jobInfo.AllowDeletionOfDanglingPARoots)
                {
                  await DeleteDanglingPipelineArtifactRootsJob.DeleteRootAsync(requestContext, domainId, (IDedupStore) dedupStoreService, entry.DedupId, entry.ReferenceId);
                  tracer.TraceInfo(string.Format("Deleted pipeline artifact on domainId: {0} dedupId: {1} with referenceId: {2} ", (object) jobInfo.DomainId, (object) entry.DedupId, (object) entry.ReferenceId) + string.Format("of size: {0} for non-existent corresponding buildId: {1}", (object) dedupNode.Value.TransitiveContentBytes, (object) buildId));
                }
                else
                  tracer.TraceInfo(string.Format("What-If: Deleted pipeline artifact on domainId: {0} dedupId: {1} with referenceId: {2} ", (object) jobInfo.DomainId, (object) entry.DedupId, (object) entry.ReferenceId) + string.Format("of size: {0} for non-existent corresponding buildId: {1}", (object) dedupNode.Value.TransitiveContentBytes, (object) buildId));
                jobInfo.Result.LogDeletionDetails(dedupNode.Value.TransitiveContentBytes);
                domainId = (IDomainId) null;
                dedupNode = new DedupNode?();
                dedupStoreService = (DedupStoreService) null;
              }
              catch (Exception ex)
              {
                tracer.TraceException(ex);
                ++jobInfo.Result.TotalRootsFailedToDelete;
                dedupStoreService = (DedupStoreService) null;
              }
            })).Unwrap();
        }
      }
    }

    private static async Task DeleteRootAsync(
      IVssRequestContext requestContext,
      IDomainId domainId,
      IDedupStore dedupStoreService,
      DedupIdentifier dedupId,
      string blobReferenceName)
    {
      IdBlobReference rootRef = new IdBlobReference(blobReferenceName, "pipelineartifact");
      IdBlobReference referenceWithDeprecatedScope = new IdBlobReference(blobReferenceName, "tfsdrop");
      await dedupStoreService.DeleteRootAsync(requestContext, domainId, dedupId, rootRef).ConfigureAwait(true);
      await dedupStoreService.DeleteRootAsync(requestContext, domainId, dedupId, referenceWithDeprecatedScope).ConfigureAwait(true);
      referenceWithDeprecatedScope = new IdBlobReference();
    }

    protected override Task<IEnumerable<PhysicalDomainInfo>> GetDomains(
      IVssRequestContext requestContext)
    {
      throw new NotImplementedException();
    }
  }
}
