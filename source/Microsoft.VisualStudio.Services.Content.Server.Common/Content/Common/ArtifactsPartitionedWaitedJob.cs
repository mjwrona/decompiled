// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Common.ArtifactsPartitionedWaitedJob
// Assembly: Microsoft.VisualStudio.Services.Content.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 203E0171-FB50-4FDE-9B1F-EFC6366423BC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Server.Common.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Content.Common
{
  public abstract class ArtifactsPartitionedWaitedJob : ArtifactsPartitionedJobBase
  {
    protected abstract VssJobResult AggregateChildJobResults(
      IVssRequestContext requestContext,
      IEnumerable<TeamFoundationJobHistoryEntry> successfulChildJobs,
      JobParameters jobParameters);

    protected override async Task<VssJobResult> HandleChildJobsAsync(
      IVssRequestContext requestContext,
      IEnumerable<Guid> childJobIds,
      JobParameters jobParameters,
      Microsoft.VisualStudio.Services.Content.Server.Common.Tracer tracer)
    {
      IEnumerable<TeamFoundationJobHistoryEntry> foundationJobHistoryEntries = (await this.WaitForChildJobsToComplete(requestContext, childJobIds, jobParameters, tracer).ConfigureAwait(true)).Where<TeamFoundationJobHistoryEntry>((Func<TeamFoundationJobHistoryEntry, bool>) (r => r.Result == TeamFoundationJobResult.Succeeded));
      int num = foundationJobHistoryEntries.Count<TeamFoundationJobHistoryEntry>();
      return num != 0 ? this.AggregateChildJobResults(requestContext, foundationJobHistoryEntries, jobParameters) : new VssJobResult(TeamFoundationJobExecutionResult.Failed, string.Format("Error: All of the child jobs failed. Successful Job Count: {0}, TotalPartitions: {1}", (object) num, (object) jobParameters.TotalPartitions));
    }

    protected async Task<IEnumerable<TeamFoundationJobHistoryEntry>> WaitForChildJobsToComplete(
      IVssRequestContext requestContext,
      IEnumerable<Guid> childJobIds,
      JobParameters jobParameters,
      Microsoft.VisualStudio.Services.Content.Server.Common.Tracer tracer)
    {
      ArtifactsPartitionedWaitedJob partitionedWaitedJob = this;
      IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
      TimeSpan maxParentJobWaitTime = partitionedWaitedJob.Settings.ParentAggregationTimeout;
      int childJobPollIntervalInSeconds = service.GetValue<int>(requestContext, (RegistryQuery) (partitionedWaitedJob.RegistryBasePath + "/ChildJobPollInterval"), true, 60);
      tracer.TraceAlways(string.Format("{0}: Waiting for child jobs for a max of {1}s polling every {2}s.", (object) nameof (WaitForChildJobsToComplete), (object) maxParentJobWaitTime, (object) childJobPollIntervalInSeconds));
      while (partitionedWaitedJob.GetRunningChildJobs(requestContext, childJobIds, tracer).Any<TeamFoundationJobQueueEntry>())
      {
        await Task.Delay(TimeSpan.FromSeconds((double) childJobPollIntervalInSeconds), requestContext.CancellationToken).ConfigureAwait(true);
        if (requestContext.CancellationToken.IsCancellationRequested || !(partitionedWaitedJob.JobStopwatch.Elapsed < maxParentJobWaitTime))
          break;
      }
      tracer.TraceAlways(string.Format("{0}: Parent job concluding sync wait for child jobs with CancellationRequested: {1} MaxWaitTimeMetOrExceeded: {2}", (object) nameof (WaitForChildJobsToComplete), (object) requestContext.CancellationToken.IsCancellationRequested, (object) (partitionedWaitedJob.JobStopwatch.Elapsed >= maxParentJobWaitTime)));
      requestContext.CancellationToken.ThrowIfCancellationRequested();
      return partitionedWaitedJob.GetChildJobsHistory(requestContext, childJobIds, partitionedWaitedJob.ParentJobStartTime);
    }

    private IEnumerable<TeamFoundationJobQueueEntry> GetRunningChildJobs(
      IVssRequestContext requestContext,
      IEnumerable<Guid> childJobIds,
      Microsoft.VisualStudio.Services.Content.Server.Common.Tracer tracer)
    {
      return requestContext.GetService<ITeamFoundationJobService>().QueryJobQueue(requestContext, childJobIds).Where<TeamFoundationJobQueueEntry>((Func<TeamFoundationJobQueueEntry, bool>) (entry =>
      {
        if (entry == null)
          return false;
        if (entry.State != TeamFoundationJobState.Dormant)
          return true;
        tracer.TraceError(string.Format("Unexpected dormant child job found. JobSource: {0}, JobID: {1}", (object) entry.JobSource, (object) entry.JobId));
        return false;
      }));
    }

    private IEnumerable<TeamFoundationJobHistoryEntry> GetChildJobsHistory(
      IVssRequestContext requestContext,
      IEnumerable<Guid> childJobIds,
      DateTimeOffset parentJobStart)
    {
      return (requestContext.GetService<ITeamFoundationJobService>().QueryLatestJobHistory(requestContext, childJobIds) ?? new List<TeamFoundationJobHistoryEntry>()).Where<TeamFoundationJobHistoryEntry>((Func<TeamFoundationJobHistoryEntry, bool>) (entry => entry != null && (DateTimeOffset) entry.ExecutionStartTime > parentJobStart));
    }
  }
}
