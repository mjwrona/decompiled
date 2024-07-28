// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.HardDeleteRootsJob
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D3AB5C9B-EB54-4477-A304-63BB297414A3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.BlobStore.Common;
using Microsoft.VisualStudio.Services.BlobStore.Server.Common;
using Microsoft.VisualStudio.Services.BlobStore.Server.Common.Domains;
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
  public class HardDeleteRootsJob : ArtifactsPartitionedJobBase
  {
    private const string RegistryRetentionTime = "/RetentionTime";
    private const string RegistryNumDeleteFailuresThreshold = "/NumDeleteFailuresThreshold";
    private static readonly Guid HardDeleteRootsJobId = ArtifactReservedJobIds.HardDeleteRootsJobId;
    private static readonly TimeSpan DefaultSoftDeleteRetentionTime = TimeSpan.FromDays(60.0);
    private const int DefaultNumDeleteFailuresThreshold = 100;

    protected override Guid ParentJobId => HardDeleteRootsJob.HardDeleteRootsJobId;

    protected override string JobNamePrefix => "HardDeleteRoots child job - ";

    protected override string RegistryBasePath => "/Configuration/BlobStore/HardDeleteRootsJob";

    public override bool IsValidPartitionSize(
      IVssRequestContext rc,
      int partitionSize,
      int numStorageAccounts)
    {
      return partitionSize == 1;
    }

    protected override (TraceData traceData, int tracePoint) TraceInfo => (new TraceData()
    {
      Area = "BlobStore",
      Layer = "Job"
    }, 5701170);

    protected override async Task<string> RunJobAsync(
      IVssRequestContext requestContext,
      JobParameters jobParameters,
      Microsoft.VisualStudio.Services.Content.Server.Common.Tracer tracer)
    {
      HardDeleteRootsJob hardDeleteRootsJob = this;
      DeleteRootsJobInfo jobInfo = new DeleteRootsJobInfo()
      {
        CpuThreshold = hardDeleteRootsJob.Settings.CpuThreshold,
        PartitionId = jobParameters.PartitionId,
        DomainId = jobParameters.DomainId,
        TotalPartitions = jobParameters.TotalPartitions,
        RunId = jobParameters.RunId
      };
      IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
      jobInfo.SoftDeleteRetentionTime = service.GetValue<TimeSpan>(requestContext, (RegistryQuery) (hardDeleteRootsJob.RegistryBasePath + "/RetentionTime"), true, HardDeleteRootsJob.DefaultSoftDeleteRetentionTime);
      jobInfo.NumDeleteFailuresThreshold = (long) service.GetValue<int>(requestContext, (RegistryQuery) (hardDeleteRootsJob.RegistryBasePath + "/NumDeleteFailuresThreshold"), true, 100);
      jobInfo.ParallelismDegree = service.GetValue<int>(requestContext, (RegistryQuery) ServiceRegistryConstants.HardDeleteRootsJobParallelismPath, true, 5);
      IDomainId domainId = DomainIdFactory.Create(jobParameters.DomainId);
      await hardDeleteRootsJob.DeleteRootsAsync(requestContext, domainId, jobInfo, UtcClock.Instance, tracer).ConfigureAwait(true);
      string str = JsonSerializer.Serialize<DeleteRootsJobInfo>(jobInfo);
      jobInfo = (DeleteRootsJobInfo) null;
      return str;
    }

    private async Task CheckAndDeleteRoot(
      VssRequestPump.SecuredDomainProcessor processor,
      IAdminDedupStore dedupStore,
      DeleteRootsJobInfo jobInfo,
      IClock clock,
      DedupMetadataEntry rootEntry)
    {
      if (!rootEntry.IsSoftDeleted)
        throw new ArgumentException("Trying to delete a root which is not soft deleted");
      int totalRootsToBeDeleted = 0;
      DateTimeOffset? stateChangeTime = rootEntry.StateChangeTime;
      if (stateChangeTime.HasValue)
      {
        stateChangeTime = rootEntry.StateChangeTime;
        DateTimeOffset dateTimeOffset = clock.Now.Subtract(jobInfo.SoftDeleteRetentionTime);
        if ((stateChangeTime.HasValue ? (stateChangeTime.GetValueOrDefault() < dateTimeOffset ? 1 : 0) : 0) != 0)
        {
          ++totalRootsToBeDeleted;
          try
          {
            await dedupStore.HardDeleteRootAsync(processor, rootEntry).ConfigureAwait(false);
          }
          catch (Exception ex)
          {
            await processor.ExecuteWorkAsync((Action<IVssRequestContext>) (rq => rq.TraceException(this.TraceInfo.tracePoint, this.TraceInfo.traceData.Area, this.TraceInfo.traceData.Layer, ex))).ConfigureAwait(false);
            lock (jobInfo)
              ++jobInfo.Result.TotalRootsFailedDeletion;
            if (jobInfo.Result.TotalRootsFailedDeletion >= jobInfo.NumDeleteFailuresThreshold)
              throw;
          }
        }
      }
      lock (jobInfo)
      {
        ++jobInfo.Result.TotalRootsInSoftDeleteState;
        jobInfo.Result.TotalRootsToBeDeleted += (long) totalRootsToBeDeleted;
      }
    }

    private async Task DeleteRootsAsync(
      VssRequestPump.SecuredDomainProcessor processor,
      IAdminDedupStore dedupStore,
      DeleteRootsJobInfo jobInfo,
      IClock clock,
      bool CPUThrottlingDisabled,
      Microsoft.VisualStudio.Services.Content.Server.Common.Tracer tracer)
    {
      DedupMetadataPageRetrievalOption option = new DedupMetadataPageRetrievalOption(ResultArrangement.AllUnordered, 50, StateFilter.SoftDeleted);
      using (IConcurrentIterator<IEnumerable<DedupMetadataEntry>> rootPages = dedupStore.GetRootPages(processor, option, tracer))
      {
        Func<DedupMetadataEntry, Task> func;
        await rootPages.ForEachAsyncNoContext<IEnumerable<DedupMetadataEntry>>(processor.CancellationToken, (Func<IEnumerable<DedupMetadataEntry>, Task>) (async page =>
        {
          if (!CPUThrottlingDisabled)
          {
            int num = await CpuThrottleHelper.Instance.Yield(jobInfo.CpuThreshold, processor.CancellationToken).ConfigureAwait(false);
            jobInfo.TotalThrottleDuration += TimeSpan.FromSeconds((double) num);
          }
          jobInfo.Result.TotalRootsDiscovered += (long) page.Count<DedupMetadataEntry>();
          await Task.WhenAll((IEnumerable<Task>) page.AsParallel<DedupMetadataEntry>().WithDegreeOfParallelism<DedupMetadataEntry>(jobInfo.ParallelismDegree).Select<DedupMetadataEntry, Task>(func ?? (func = (Func<DedupMetadataEntry, Task>) (async entry => await this.CheckAndDeleteRoot(processor, dedupStore, jobInfo, clock, entry).ConfigureAwait(false)))));
        })).ConfigureAwait(false);
      }
    }

    private async Task DeleteRootsAsync(
      IVssRequestContext requestContext,
      IDomainId domainId,
      DeleteRootsJobInfo jobInfo,
      IClock clock,
      Microsoft.VisualStudio.Services.Content.Server.Common.Tracer tracer)
    {
      HardDeleteRootsJob hardDeleteRootsJob = this;
      AdminDedupStoreService dedupService = requestContext.GetService<AdminDedupStoreService>();
      bool CPUThrottlingDisabled = hardDeleteRootsJob.IsCPUThrottlingDisabled(requestContext);
      await requestContext.PumpFromAsync((ISecuredDomainRequest) new SecuredDomainRequest(requestContext, domainId), (Func<VssRequestPump.SecuredDomainProcessor, Task>) (async processor => await this.DeleteRootsAsync(processor, (IAdminDedupStore) dedupService, jobInfo, clock, CPUThrottlingDisabled, tracer).ConfigureAwait(false)));
    }

    protected override async Task<IEnumerable<PhysicalDomainInfo>> GetDomains(
      IVssRequestContext requestContext)
    {
      return !requestContext.AllowHostDomainAdminOperations() ? (IEnumerable<PhysicalDomainInfo>) null : await requestContext.GetService<AdminHostDomainStoreService>().GetPhysicalDomainsForOrganizationForAdminAsync(requestContext).ConfigureAwait(true);
    }
  }
}
