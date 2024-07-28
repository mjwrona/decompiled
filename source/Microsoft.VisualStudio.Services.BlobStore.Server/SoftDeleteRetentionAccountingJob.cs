// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.SoftDeleteRetentionAccountingJob
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D3AB5C9B-EB54-4477-A304-63BB297414A3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.BlobStore.Common;
using Microsoft.VisualStudio.Services.BlobStore.Server.Common;
using Microsoft.VisualStudio.Services.BlobStore.Server.Domain;
using Microsoft.VisualStudio.Services.Content.Common;
using Microsoft.VisualStudio.Services.Content.Server.Azure;
using Microsoft.VisualStudio.Services.Content.Server.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.BlobStore.Server
{
  public class SoftDeleteRetentionAccountingJob : ArtifactsPartitionedWaitedJob
  {
    public static readonly Guid SoftDeleteRetentionAccountingJobId = Guid.Parse("7A9A7B6F-6018-4090-9962-9E3FEFC38CAE");
    private readonly TraceData traceData = new TraceData()
    {
      Area = "BlobStore",
      Layer = "Job"
    };
    private const int SoftDeleteRetentionAccountingTracepoint = 5701992;

    protected override string RegistryBasePath => "/Configuration/BlobStore/SoftDeleteRetentionAccountingJob";

    protected override Guid ParentJobId => SoftDeleteRetentionAccountingJob.SoftDeleteRetentionAccountingJobId;

    protected override string JobNamePrefix => "Soft delete retention accounting child job - ";

    protected override (TraceData traceData, int tracePoint) TraceInfo => (this.traceData, 5701992);

    public override bool IsValidPartitionSize(
      IVssRequestContext rc,
      int partitionSize,
      int numStorageAccounts)
    {
      return numStorageAccounts % partitionSize == 0;
    }

    protected override async Task<string> RunJobAsync(
      IVssRequestContext requestContext,
      JobParameters jobParameters,
      Microsoft.VisualStudio.Services.Content.Server.Common.Tracer tracer)
    {
      SoftDeleteRetentionAccountingJob retentionAccountingJob = this;
      AdminPlatformBlobStore pbs = requestContext.GetService<AdminPlatformBlobStore>();
      IEnumerable<PhysicalDomainInfo> source = await retentionAccountingJob.GetDomains(requestContext).ConfigureAwait(false);
      PhysicalDomainInfo physicalDomainInfo;
      if (source != null)
      {
        physicalDomainInfo = source.SingleOrDefault<PhysicalDomainInfo>((Func<PhysicalDomainInfo, bool>) (dom => dom.DomainId.ToString().Equals(jobParameters.DomainId, StringComparison.OrdinalIgnoreCase)));
        if (physicalDomainInfo == null)
          throw new Exception("Job is scheduled for domain " + jobParameters.DomainId + " but the domain doesn't exist. Domains discovered: " + string.Join<IDomainId>(",", source.Select<PhysicalDomainInfo, IDomainId>((Func<PhysicalDomainInfo, IDomainId>) (dom => dom.DomainId))));
      }
      IEnumerable<string> shardConnectionStrings = new IteratorPartition(jobParameters.PartitionId, jobParameters.TotalPartitions).SelectIterators<string>(StorageAccountConfigurationFacade.ReadAllStorageAccounts(requestContext.To(TeamFoundationHostType.Deployment), physicalDomainInfo).Select<StrongBoxConnectionString, string>((Func<StrongBoxConnectionString, string>) (s => s.ConnectionString)));
      SoftDeletedRetentionJobPartitionedInfo jobInfo = new SoftDeletedRetentionJobPartitionedInfo()
      {
        DomainId = jobParameters.DomainId,
        RunId = jobParameters.RunId,
        PartitionId = jobParameters.PartitionId,
        TotalPartitions = jobParameters.TotalPartitions,
        MaxParallelism = retentionAccountingJob.Settings.MaxParallelism,
        CpuThreshold = retentionAccountingJob.Settings.CpuThreshold
      };
      await pbs.AccountForSoftDeletedBytesFromContainersAsync(requestContext, jobInfo, shardConnectionStrings, retentionAccountingJob.Settings.CpuThreshold, retentionAccountingJob.IsCPUThrottlingDisabled(requestContext));
      string str = JsonSerializer.Serialize<SoftDeletedRetentionJobPartitionedInfo>(jobInfo);
      pbs = (AdminPlatformBlobStore) null;
      jobInfo = (SoftDeletedRetentionJobPartitionedInfo) null;
      return str;
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
      IEnumerable<SoftDeletedRetentionJobResult> source = successfulChildJobs.Select<TeamFoundationJobHistoryEntry, SoftDeletedRetentionJobResult>((Func<TeamFoundationJobHistoryEntry, SoftDeletedRetentionJobResult>) (r => JsonSerializer.Deserialize<SoftDeletedRetentionJobPartitionedInfo>(r.ResultMessage).Result));
      SoftDeletedRetentionJobAggregatedInfo jobAggregatedInfo = new SoftDeletedRetentionJobAggregatedInfo();
      jobAggregatedInfo.CpuThreshold = this.Settings.CpuThreshold;
      jobAggregatedInfo.TotalPartitions = jobParameters.TotalPartitions;
      jobAggregatedInfo.RunId = jobParameters.RunId;
      jobAggregatedInfo.DomainId = jobParameters.DomainId;
      jobAggregatedInfo.TotalSucceededChildJobs = num;
      jobAggregatedInfo.Result = new SoftDeletedRetentionJobResult()
      {
        TotalFileDedupSoftDeletedBlobs = source.Sum<SoftDeletedRetentionJobResult>((Func<SoftDeletedRetentionJobResult, long>) (fsd => fsd.TotalFileDedupSoftDeletedBlobs)),
        TotalFileDedupSoftDeletedBytes = source.Sum<SoftDeletedRetentionJobResult>((Func<SoftDeletedRetentionJobResult, long>) (fsd => fsd.TotalFileDedupSoftDeletedBytes)),
        TotalChunkDedupSoftDeletedBlobs = source.Sum<SoftDeletedRetentionJobResult>((Func<SoftDeletedRetentionJobResult, long>) (csd => csd.TotalChunkDedupSoftDeletedBlobs)),
        TotalChunkDedupSoftDeletedBytes = source.Sum<SoftDeletedRetentionJobResult>((Func<SoftDeletedRetentionJobResult, long>) (csd => csd.TotalChunkDedupSoftDeletedBytes)),
        TotalThrottleDuration = new TimeSpan(source.Sum<SoftDeletedRetentionJobResult>((Func<SoftDeletedRetentionJobResult, long>) (ttd => ttd.TotalThrottleDuration.Ticks))),
        TotalShards = source.Sum<SoftDeletedRetentionJobResult>((Func<SoftDeletedRetentionJobResult, int>) (sh => sh.TotalShards))
      };
      SoftDeletedRetentionJobAggregatedInfo dataContractObject = jobAggregatedInfo;
      foreach (SoftDeletedRetentionJobResult retentionJobResult in source)
      {
        dataContractObject.Result.FileSoftDeletedExpirationInDays.MergeFrom((Histogram) retentionJobResult.FileSoftDeletedExpirationInDays);
        dataContractObject.Result.ChunkSoftDeletedExpirationInDays.MergeFrom((Histogram) retentionJobResult.ChunkSoftDeletedExpirationInDays);
      }
      return new VssJobResult(result, JsonSerializer.Serialize<SoftDeletedRetentionJobAggregatedInfo>(dataContractObject));
    }

    protected override async Task<IEnumerable<PhysicalDomainInfo>> GetDomains(
      IVssRequestContext requestContext)
    {
      return !requestContext.AllowHostDomainAdminOperations() ? (IEnumerable<PhysicalDomainInfo>) null : await requestContext.GetService<AdminHostDomainStoreService>().GetPhysicalDomainsForOrganizationForAdminAsync(requestContext).ConfigureAwait(true);
    }
  }
}
