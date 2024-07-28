// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Server.Common.JobFramework.AutoJobFanoutEngine
// Assembly: Microsoft.VisualStudio.Services.Content.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 203E0171-FB50-4FDE-9B1F-EFC6366423BC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Server.Common.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Content.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Content.Server.Common.JobFramework
{
  public class AutoJobFanoutEngine : IAutoJobFanoutEngine
  {
    private readonly string registryJobBasePath;
    private Microsoft.VisualStudio.Services.Content.Server.Common.Tracer tracer;
    private Guid jobId;
    private readonly ArtifactsPartitionedJobBase artifactsPartitionedJobBase;

    public AutoJobFanoutEngine(
      string registryJobBasePath,
      Guid jobId,
      ArtifactsPartitionedJobBase artifactsPartitionedJobBase,
      Microsoft.VisualStudio.Services.Content.Server.Common.Tracer tracer)
    {
      this.artifactsPartitionedJobBase = artifactsPartitionedJobBase;
      this.registryJobBasePath = registryJobBasePath;
      this.jobId = jobId;
      this.tracer = tracer;
    }

    public JobFanoutInfo ComputeRecommendation(
      IVssRequestContext requestContext,
      int totalPartitions,
      int shardsCount)
    {
      IEnumerable<Tuple<TeamFoundationJobHistoryEntry, TimeSpan>> inLookbackWindow = this.GetCountOfSuccessfulJobResultsInLookbackWindow(requestContext);
      JobFanoutInfo jobFanoutInfo = this.GetJobFanoutInfo(requestContext, inLookbackWindow, totalPartitions);
      bool flag1 = JobFanoutSettings.ValidTotalPartitionsByJobId.ContainsKey(this.jobId);
      if (jobFanoutInfo.SuccessfulCountExceedingTargetThresholdTime == 7 & flag1)
      {
        int currentTotalPartitions = jobFanoutInfo.CurrentTotalPartitions;
        bool flag2;
        do
        {
          ++currentTotalPartitions;
          if (currentTotalPartitions > 4100)
          {
            flag2 = false;
            break;
          }
          flag2 = this.artifactsPartitionedJobBase.IsValidPartitionSize(requestContext, currentTotalPartitions, shardsCount);
        }
        while (!flag2);
        if (!flag2)
          currentTotalPartitions = jobFanoutInfo.CurrentTotalPartitions;
        if (currentTotalPartitions > 0 & flag2)
          jobFanoutInfo.RecommendedTotalPartitions = currentTotalPartitions;
        this.tracer.TraceAlways("function:ComputeRecommendation," + string.Format("JobId: {0},", (object) jobFanoutInfo.JobId) + "Message: Recommending TotalPartition fanout out," + string.Format("Current: {0},", (object) jobFanoutInfo.CurrentTotalPartitions) + string.Format("Recommended: {0}", (object) jobFanoutInfo.RecommendedTotalPartitions));
      }
      return jobFanoutInfo;
    }

    private JobFanoutInfo GetJobFanoutInfo(
      IVssRequestContext requestContext,
      IEnumerable<Tuple<TeamFoundationJobHistoryEntry, TimeSpan>> pastJobResults,
      int totalPartitions)
    {
      return new JobFanoutInfo()
      {
        JobId = this.jobId,
        HostId = requestContext.ServiceHost.InstanceId,
        CurrentTotalPartitions = totalPartitions,
        TargetThreshold = JobConfig.DefaultTargetExecutionTimeThreshold,
        NumberOfSamples = pastJobResults.Count<Tuple<TeamFoundationJobHistoryEntry, TimeSpan>>(),
        SuccessfulCountExceedingTargetThresholdTime = pastJobResults.Where<Tuple<TeamFoundationJobHistoryEntry, TimeSpan>>((Func<Tuple<TeamFoundationJobHistoryEntry, TimeSpan>, bool>) (jobResult => jobResult.Item1.Result == TeamFoundationJobResult.Succeeded && jobResult.Item2 > JobConfig.DefaultTargetExecutionTimeThreshold)).Count<Tuple<TeamFoundationJobHistoryEntry, TimeSpan>>(),
        RecommendedTotalPartitions = totalPartitions
      };
    }

    private void RemoveJobRegistry(
      IVssRequestContext requestContext,
      string registryPath,
      Guid jobId)
    {
      ArtifactsPartitionedJobSettings partitionedJobSettings = new ArtifactsPartitionedJobSettings(requestContext, registryPath);
      IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
      try
      {
        this.tracer.TraceAlways("function: RemoveJobRegistry," + string.Format("JobId: {0},", (object) jobId) + "Message: Adjusting job fanout settings now...");
        this.tracer.TraceAlways("function: RemoveJobRegistry, " + string.Format("JobId: {0}, ", (object) jobId) + "Message: Removing Current Settings," + string.Format("TotalPartitions: {0}", (object) partitionedJobSettings.TotalPartitionSize));
        service.DeleteEntries(requestContext, registryPath + "/TotalPartitions");
        this.tracer.TraceAlways("function: RemoveJobRegistry," + string.Format("JobId: {0},", (object) jobId) + "Message: Removed Current Settings," + string.Format("TotalPartitions: {0}", (object) partitionedJobSettings.TotalPartitionSize));
      }
      catch (Exception ex)
      {
        this.tracer.TraceAlways(ex.Message);
        throw;
      }
    }

    private IEnumerable<Tuple<TeamFoundationJobHistoryEntry, TimeSpan>> GetCountOfSuccessfulJobResultsInLookbackWindow(
      IVssRequestContext requestContext)
    {
      List<Tuple<TeamFoundationJobHistoryEntry, TimeSpan>> results = new List<Tuple<TeamFoundationJobHistoryEntry, TimeSpan>>();
      List<TeamFoundationJobHistoryEntry> source = requestContext.GetService<ITeamFoundationJobService>().QueryJobHistory(requestContext, this.jobId);
      IEnumerable<TeamFoundationJobHistoryEntry> foundationJobHistoryEntries = source != null ? source.Where<TeamFoundationJobHistoryEntry>((Func<TeamFoundationJobHistoryEntry, bool>) (jobResult => jobResult.Result == TeamFoundationJobResult.Succeeded)).OrderByDescending<TeamFoundationJobHistoryEntry, DateTime>((Func<TeamFoundationJobHistoryEntry, DateTime>) (jobResult => jobResult.EndTime)).Take<TeamFoundationJobHistoryEntry>(7) : (IEnumerable<TeamFoundationJobHistoryEntry>) null;
      this.tracer.TraceAlways("function: GetCountOfSuccessfulJobResultsInLookbackWindow," + string.Format("JobId: {0},", (object) this.jobId) + string.Format("Message: Found {0} successful job results", (object) foundationJobHistoryEntries.Count<TeamFoundationJobHistoryEntry>()));
      if (foundationJobHistoryEntries.Count<TeamFoundationJobHistoryEntry>() > 0)
        foundationJobHistoryEntries.ForEach<TeamFoundationJobHistoryEntry>((Action<TeamFoundationJobHistoryEntry>) (successjob => results.Add(new Tuple<TeamFoundationJobHistoryEntry, TimeSpan>(successjob, successjob.EndTime - successjob.ExecutionStartTime))));
      return (IEnumerable<Tuple<TeamFoundationJobHistoryEntry, TimeSpan>>) results;
    }

    public bool SetRecommendation(IVssRequestContext requestContext, JobFanoutInfo fanoutInfo)
    {
      bool flag = false;
      ArtifactsPartitionedJobSettings partitionedJobSettings = new ArtifactsPartitionedJobSettings(requestContext, this.registryJobBasePath);
      IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
      try
      {
        this.tracer.TraceAlways("function: SetRecommendation, Message: Adjusting job fanout settings now...");
        this.tracer.TraceAlways("function: SetRecommendation, Message: Current Settings -" + string.Format("HostId: {0}", (object) fanoutInfo.HostId) + string.Format("JobId: {0}", (object) fanoutInfo.JobId) + string.Format("Total partitions: {0} ", (object) partitionedJobSettings.TotalPartitionSize));
        service.SetValue<int>(requestContext, this.registryJobBasePath + "/TotalPartitions", fanoutInfo.RecommendedTotalPartitions);
        this.tracer.TraceAlways("function: SetRecommendation, Message: Applied recommended settings -" + string.Format("HostId: {0}", (object) fanoutInfo.HostId) + string.Format("JobId: {0}", (object) fanoutInfo.JobId) + string.Format("Total partitions: {0}", (object) fanoutInfo.RecommendedTotalPartitions));
        flag = true;
        return flag;
      }
      catch (Exception ex)
      {
        this.tracer.TraceAlways("function: SetRecommendation], " + string.Format("JobId: {0},", (object) fanoutInfo.JobId) + "Exception: " + ex.Message + ")");
        throw;
      }
      finally
      {
        if (!flag)
        {
          service.SetValue<int>(requestContext, this.registryJobBasePath + "/TotalPartitions", partitionedJobSettings.TotalPartitionSize);
          this.tracer.TraceAlways("function: SetRecommendation," + string.Format("JobId: {0}, ", (object) fanoutInfo.JobId) + "Message: Restoring current settings because the recommended settings couldn't be applied," + string.Format("Total partitions: {0},", (object) partitionedJobSettings.TotalPartitionSize));
        }
      }
    }

    public void ClearRecommendation(IVssRequestContext requestContext, Guid jobId) => this.RemoveJobRegistry(requestContext, this.registryJobBasePath, jobId);
  }
}
