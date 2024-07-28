// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.ChunkDedupPhysicalSizeJob
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D3AB5C9B-EB54-4477-A304-63BB297414A3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.BlobStore.Common;
using Microsoft.VisualStudio.Services.BlobStore.Server.Common;
using Microsoft.VisualStudio.Services.BlobStore.Server.Domain;
using Microsoft.VisualStudio.Services.Content.Common;
using Microsoft.VisualStudio.Services.Content.Server.Common;
using Microsoft.VisualStudio.Services.Content.Server.Common.JobFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.BlobStore.Server
{
  public class ChunkDedupPhysicalSizeJob : ArtifactsPartitionedWaitedJob
  {
    public static readonly Guid ChunkPhysicalSizeInfoJobId = ArtifactReservedJobIds.ChunkDedupPhysicalSizeJobId;
    private readonly TraceData traceData = new TraceData()
    {
      Area = "BlobStore",
      Layer = "Job"
    };
    private const int ChunkPhysicalUsageTracepoint = 5701111;
    private readonly string EnablePrefixScopedScan = "/Configuration/BlobStore/ChunkDedupPhysicalSizeJob/EnablePrefixScopedScan";
    private static readonly IEnumerable<int> ValidNumPartitions = (IEnumerable<int>) JobFanoutSettings.ValidTotalPartitionsByJobId[ArtifactReservedJobIds.ChunkDedupPhysicalSizeJobId];

    protected override string RegistryBasePath => "/Configuration/BlobStore/ChunkDedupPhysicalSizeJob";

    protected override Guid ParentJobId => ChunkDedupPhysicalSizeJob.ChunkPhysicalSizeInfoJobId;

    public override bool IsValidPartitionSize(
      IVssRequestContext rc,
      int partitionSize,
      int numStorageAccounts)
    {
      return ChunkDedupPhysicalSizeJob.ValidNumPartitions.Contains<int>(partitionSize) || numStorageAccounts % partitionSize == 0;
    }

    protected override string JobNamePrefix => "Chunk dedup physical size accounting child job - ";

    protected override (TraceData traceData, int tracePoint) TraceInfo => (this.traceData, 5701111);

    protected override bool ShouldReuseJobResultForInactiveHost => true;

    protected override async Task<string> RunJobAsync(
      IVssRequestContext requestContext,
      JobParameters jobParameters,
      Microsoft.VisualStudio.Services.Content.Server.Common.Tracer tracer)
    {
      ChunkDedupPhysicalSizeJob dedupPhysicalSizeJob = this;
      bool flag = requestContext.GetService<IVssRegistryService>().GetValue<bool>(requestContext, (RegistryQuery) dedupPhysicalSizeJob.EnablePrefixScopedScan, true, false);
      tracer.TraceAlways(string.Format("Job will execute in prefix scoped mode: {0}", (object) flag));
      AdminDedupStoreService service = requestContext.GetService<AdminDedupStoreService>();
      DedupDataJobInfo jobInfo = dedupPhysicalSizeJob.CreateJobInfo(jobParameters);
      IVssRequestContext requestContext1 = requestContext;
      DedupDataJobInfo jobInfo1 = jobInfo;
      IClock instance = UtcClock.Instance;
      int num1 = flag ? 1 : 0;
      int num2 = dedupPhysicalSizeJob.IsCPUThrottlingDisabled(requestContext) ? 1 : 0;
      await service.CollectStorageDataFromContainersAsync(requestContext1, jobInfo1, instance, num1 != 0, num2 != 0).ConfigureAwait(true);
      string str = JsonSerializer.Serialize<DedupDataJobInfo>(jobInfo);
      jobInfo = (DedupDataJobInfo) null;
      return str;
    }

    protected override VssJobResult AggregateChildJobResults(
      IVssRequestContext requestContext,
      IEnumerable<TeamFoundationJobHistoryEntry> successfulChildJobs,
      JobParameters jobParameters)
    {
      int num = successfulChildJobs.Count<TeamFoundationJobHistoryEntry>();
      TeamFoundationJobExecutionResult result = TeamFoundationJobExecutionResult.Succeeded;
      if (num == 0)
        return new VssJobResult(TeamFoundationJobExecutionResult.Failed, "No chunk dedup physical size child job succeeded.");
      if (num < jobParameters.TotalPartitions)
        result = TeamFoundationJobExecutionResult.PartiallySucceeded;
      IEnumerable<DedupDataJobInfo> source = successfulChildJobs.Select<TeamFoundationJobHistoryEntry, DedupDataJobInfo>((Func<TeamFoundationJobHistoryEntry, DedupDataJobInfo>) (r => JsonSerializer.Deserialize<DedupDataJobInfo>(r.ResultMessage))).Where<DedupDataJobInfo>((Func<DedupDataJobInfo, bool>) (r => r.IsCompleteResult));
      ParentDedupDataJobInfo dedupDataJobInfo = new ParentDedupDataJobInfo();
      dedupDataJobInfo.CpuThreshold = this.Settings.CpuThreshold;
      dedupDataJobInfo.TotalPartitions = jobParameters.TotalPartitions;
      dedupDataJobInfo.RunId = jobParameters.RunId;
      dedupDataJobInfo.DomainId = jobParameters.DomainId;
      dedupDataJobInfo.TotalSucceededChildJobs = num;
      ParentDedupDataJobInfo dataContractObject = dedupDataJobInfo;
      dataContractObject.Result.AggregateDedupStorageData(source.Select<DedupDataJobInfo, DedupPhysicalStorageData>((Func<DedupDataJobInfo, DedupPhysicalStorageData>) (r => r.Result)), jobParameters.TotalPartitions, num != jobParameters.TotalPartitions);
      return new VssJobResult(result, JsonSerializer.Serialize<ParentDedupDataJobInfo>(dataContractObject));
    }

    protected override async Task<IEnumerable<PhysicalDomainInfo>> GetDomains(
      IVssRequestContext requestContext)
    {
      return !requestContext.AllowHostDomainAdminOperations() ? (IEnumerable<PhysicalDomainInfo>) null : await requestContext.GetService<AdminHostDomainStoreService>().GetPhysicalDomainsForOrganizationForAdminAsync(requestContext).ConfigureAwait(true);
    }

    private DedupDataJobInfo CreateJobInfo(JobParameters jobParameters)
    {
      DedupDataJobInfo jobInfo = new DedupDataJobInfo();
      jobInfo.CpuThreshold = this.Settings.CpuThreshold;
      jobInfo.PartitionId = jobParameters.PartitionId;
      jobInfo.TotalPartitions = jobParameters.TotalPartitions;
      jobInfo.RunId = jobParameters.RunId;
      jobInfo.DomainId = jobParameters.DomainId;
      jobInfo.MaxParallelism = this.Settings.MaxParallelism;
      jobInfo.FirstJobStartTime = new DateTimeOffset?(DateTimeOffset.Now);
      return jobInfo;
    }
  }
}
