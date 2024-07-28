// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.ChunkDedupLogicalSizeJob
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D3AB5C9B-EB54-4477-A304-63BB297414A3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.BlobStore.Common;
using Microsoft.VisualStudio.Services.BlobStore.Server.Billing;
using Microsoft.VisualStudio.Services.BlobStore.Server.Billing.PackagingBreakdown;
using Microsoft.VisualStudio.Services.BlobStore.Server.Common;
using Microsoft.VisualStudio.Services.BlobStore.Server.Domain;
using Microsoft.VisualStudio.Services.Content.Common;
using Microsoft.VisualStudio.Services.Content.Server.Common;
using Microsoft.VisualStudio.Services.Content.Server.Common.JobFramework;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.BlobStore.Server
{
  public class ChunkDedupLogicalSizeJob : ArtifactsPartitionedWaitedJob
  {
    public static readonly Guid ChunkLogicalUsageInfoJobId = ArtifactReservedJobIds.ChunkDedupLogicalSizeJobId;
    private readonly TraceData traceData = new TraceData()
    {
      Area = "BlobStore",
      Layer = "Job"
    };
    private const int ChunkLogicalUsageTracepoint = 5701999;
    private static readonly IEnumerable<int> ValidNumPartitions = (IEnumerable<int>) JobFanoutSettings.ValidTotalPartitionsByJobId[ArtifactReservedJobIds.ChunkDedupLogicalSizeJobId];

    protected override string RegistryBasePath => "/Configuration/BlobStore/ChunkDedupLogicalSizeJob";

    protected override Guid ParentJobId => ChunkDedupLogicalSizeJob.ChunkLogicalUsageInfoJobId;

    public override bool IsValidPartitionSize(
      IVssRequestContext rc,
      int partitionSize,
      int numStorageAccounts)
    {
      return partitionSize <= BlobStoreUtils.PrefixX3.Length;
    }

    protected override string JobNamePrefix => "Chunk dedup logical size accounting child job - ";

    protected override (TraceData traceData, int tracePoint) TraceInfo => (this.traceData, 5701999);

    protected override bool IsCPUThrottlingDisabled(IVssRequestContext requestContext) => true;

    protected override bool ShouldReuseJobResultForInactiveHost => true;

    protected override async Task<string> RunJobAsync(
      IVssRequestContext requestContext,
      JobParameters jobParameters,
      Microsoft.VisualStudio.Services.Content.Server.Common.Tracer tracer)
    {
      ChunkDedupLogicalSizeJob dedupLogicalSizeJob = this;
      DedupLogicalDataJobPartitionedInfo jobInfo = new DedupLogicalDataJobPartitionedInfo()
      {
        CpuThreshold = dedupLogicalSizeJob.Settings.CpuThreshold,
        PartitionId = jobParameters.PartitionId,
        TotalPartitions = jobParameters.TotalPartitions,
        RunId = jobParameters.RunId,
        DomainId = jobParameters.DomainId,
        MaxParallelism = 1
      };
      AdminDedupStoreService service = requestContext.GetService<AdminDedupStoreService>();
      ChunkPackagingStorageBreakdownService chunkPackagingStorageBreakdownService = requestContext.GetService<ChunkPackagingStorageBreakdownService>();
      IDomainId domainId = DomainIdFactory.Create(jobParameters.DomainId);
      if (dedupLogicalSizeJob.IsRunningForDefaultDomain(jobInfo) && jobInfo.PartitionId == 0)
        chunkPackagingStorageBreakdownService.ResetResultsIfNeeded(requestContext, jobParameters);
      await dedupLogicalSizeJob.CollectDedupLogicalSizeAcrossPartitions(requestContext, domainId, (IAdminDedupStore) service, jobInfo, jobParameters, tracer);
      if (dedupLogicalSizeJob.IsRunningForDefaultDomain(jobInfo))
      {
        ConcurrentDictionary<string, ulong> logicalSizeByFeed = UsageInfoServiceExtensions.GetTopLogicalSizeByFeed(jobInfo.Result.LogicalSizeByFeed);
        chunkPackagingStorageBreakdownService.SavePartitionResult(requestContext, new ChunkVolumeByFeedResult()
        {
          LogicalSizeByFeed = logicalSizeByFeed
        }, jobParameters);
        if (jobInfo.TotalPartitions == 1)
        {
          chunkPackagingStorageBreakdownService.SaveRunResult(requestContext, jobParameters);
          chunkPackagingStorageBreakdownService.QueueAggregationJob(requestContext);
        }
      }
      string str = JsonSerializer.Serialize<DedupLogicalDataJobPartitionedInfo>(jobInfo);
      jobInfo = (DedupLogicalDataJobPartitionedInfo) null;
      chunkPackagingStorageBreakdownService = (ChunkPackagingStorageBreakdownService) null;
      return str;
    }

    private async Task CollectDedupLogicalSizeAcrossPartitions(
      IVssRequestContext requestContext,
      IDomainId domainId,
      IAdminDedupStore dedupStore,
      DedupLogicalDataJobPartitionedInfo jobInfo,
      JobParameters jobParameters,
      Microsoft.VisualStudio.Services.Content.Server.Common.Tracer tracer)
    {
      ChunkDedupLogicalSizeJob dedupLogicalSizeJob = this;
      IEnumerable<string> prefixRange = BlobStoreUtils.GeneratePrefixRange(jobInfo.TotalPartitions, jobInfo.PartitionId);
      ChunkLogicalSizeJobHelper chunkLogicalSizeJobHelper = new ChunkLogicalSizeJobHelper();
      foreach (string prefix in prefixRange)
        await chunkLogicalSizeJobHelper.PerformChunkLogicalSizeAccounting(requestContext, domainId, dedupStore, (IDedupLogicalDataJobResult) jobInfo.Result, jobParameters, prefix, dedupLogicalSizeJob.Settings.CpuThreshold, dedupLogicalSizeJob.IsCPUThrottlingDisabled(requestContext), tracer).ConfigureAwait(true);
      chunkLogicalSizeJobHelper = (ChunkLogicalSizeJobHelper) null;
    }

    protected override VssJobResult AggregateChildJobResults(
      IVssRequestContext requestContext,
      IEnumerable<TeamFoundationJobHistoryEntry> successfulChildJobs,
      JobParameters jobParameters)
    {
      TeamFoundationJobExecutionResult result = TeamFoundationJobExecutionResult.Succeeded;
      int num1 = successfulChildJobs.Count<TeamFoundationJobHistoryEntry>();
      if (num1 == 0)
        return new VssJobResult(TeamFoundationJobExecutionResult.Failed, "None of the child jobs succeeded.");
      if (num1 < jobParameters.TotalPartitions)
        result = TeamFoundationJobExecutionResult.PartiallySucceeded;
      IEnumerable<DedupLogicalDataJobResult> source = successfulChildJobs.Select<TeamFoundationJobHistoryEntry, DedupLogicalDataJobResult>((Func<TeamFoundationJobHistoryEntry, DedupLogicalDataJobResult>) (r => JsonSerializer.Deserialize<DedupLogicalDataJobPartitionedInfo>(r.ResultMessage).Result));
      DedupLogicalDataJobAggregatedInfo jobAggregatedInfo1 = new DedupLogicalDataJobAggregatedInfo();
      jobAggregatedInfo1.CpuThreshold = this.Settings.CpuThreshold;
      jobAggregatedInfo1.TotalPartitions = jobParameters.TotalPartitions;
      jobAggregatedInfo1.RunId = jobParameters.RunId;
      jobAggregatedInfo1.DomainId = jobParameters.DomainId;
      jobAggregatedInfo1.TotalSucceededChildJobs = num1;
      jobAggregatedInfo1.Result = new DedupLogicalDataJobResult()
      {
        TotalRootsEvaluated = source.Sum<DedupLogicalDataJobResult>((Func<DedupLogicalDataJobResult, ulong>) (tre => tre.TotalRootsEvaluated)),
        TotalRootsDiscovered = source.Sum<DedupLogicalDataJobResult>((Func<DedupLogicalDataJobResult, ulong>) (trd => trd.TotalRootsDiscovered)),
        TotalThrottleDuration = new TimeSpan(source.Sum<DedupLogicalDataJobResult>((Func<DedupLogicalDataJobResult, long>) (ttd => ttd.TotalThrottleDuration.Ticks))),
        TotalBytes = (ulong) (source.Average<DedupLogicalDataJobResult>((Func<DedupLogicalDataJobResult, double>) (tb => (double) tb.TotalBytes)) * (double) jobParameters.TotalPartitions),
        TotalBytesOutOfScope = (ulong) (source.Average<DedupLogicalDataJobResult>((Func<DedupLogicalDataJobResult, double>) (tbos => (double) tbos.TotalBytesOutOfScope)) * (double) jobParameters.TotalPartitions)
      };
      DedupLogicalDataJobAggregatedInfo jobAggregatedInfo2 = jobAggregatedInfo1;
      foreach (DedupLogicalDataJobResult logicalDataJobResult in source)
      {
        foreach (KeyValuePair<ArtifactScopeType, ulong> keyValuePair in logicalDataJobResult.SizeByScope)
        {
          KeyValuePair<ArtifactScopeType, ulong> kvp = keyValuePair;
          long num2 = (long) jobAggregatedInfo2.Result.SizeByScope.AddOrUpdate(kvp.Key, kvp.Value, (Func<ArtifactScopeType, ulong, ulong>) ((k, v) => v + kvp.Value));
        }
      }
      if (this.IsRunningForDefaultDomain((DedupLogicalDataJobPartitionedInfo) jobAggregatedInfo2))
      {
        ChunkPackagingStorageBreakdownService service = requestContext.GetService<ChunkPackagingStorageBreakdownService>();
        service.SaveRunResult(requestContext, jobParameters);
        service.QueueAggregationJob(requestContext);
      }
      return new VssJobResult(result, JsonSerializer.Serialize<DedupLogicalDataJobAggregatedInfo>(jobAggregatedInfo2));
    }

    protected override async Task<IEnumerable<PhysicalDomainInfo>> GetDomains(
      IVssRequestContext requestContext)
    {
      return !requestContext.AllowHostDomainAdminOperations() ? (IEnumerable<PhysicalDomainInfo>) null : await requestContext.GetService<AdminHostDomainStoreService>().GetPhysicalDomainsForOrganizationForAdminAsync(requestContext).ConfigureAwait(true);
    }

    private bool IsRunningForDefaultDomain(DedupLogicalDataJobPartitionedInfo jobInfo) => jobInfo.DomainId == WellKnownDomainIds.DefaultDomainId.Serialize();
  }
}
