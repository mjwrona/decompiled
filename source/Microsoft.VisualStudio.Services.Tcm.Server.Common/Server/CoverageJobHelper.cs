// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.CoverageJobHelper
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.Coverage.WebApi;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.Threading;
using Microsoft.TeamFoundation.TestManagement.Server.Hub;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [CLSCompliant(false)]
  public class CoverageJobHelper
  {
    public static void SetPipelienCoverageSummaryStatus(
      TestManagementRequestContext tcmRequestContext,
      PipelineContext pipelineContext,
      Dictionary<string, object> ciData,
      string scopeName,
      CoverageEvaluationStatus evaluationStatus)
    {
      IPipelineCoverageService pipelineCoverageService = tcmRequestContext.RequestContext.GetService<IPipelineCoverageService>();
      PipelineCoverageSummary pipelineCoverageSummary = new PipelineCoverageSummary()
      {
        Scope = scopeName,
        EvaluationStatus = evaluationStatus.ToString()
      };
      if (tcmRequestContext.IsFeatureEnabled("TestManagement.Server.EnableRunSynchronousforCoverageJobs"))
        tcmRequestContext.RequestContext.RunSynchronously((Func<Task>) (() => pipelineCoverageService.UpdatePipelineCoverageSummary(tcmRequestContext, pipelineContext.ProjectId, pipelineContext.Id, pipelineCoverageSummary, new CoverageScope()
        {
          Name = scopeName
        })));
      else
        pipelineCoverageService.UpdatePipelineCoverageSummary(tcmRequestContext, pipelineContext.ProjectId, pipelineContext.Id, pipelineCoverageSummary, new CoverageScope()
        {
          Name = scopeName
        });
      ciData.Add("PipelineCoverageEvaluationStatusChange", (object) evaluationStatus);
    }

    public static void RequeueJob(
      TestManagementRequestContext tcmRequestContext,
      Guid currentJobId,
      Dictionary<string, object> ciData,
      string requeueReason)
    {
      ciData.Add("ReQueueReason", (object) requeueReason);
      ITeamFoundationJobService service = tcmRequestContext.RequestContext.GetService<ITeamFoundationJobService>();
      int sleepTimeInSeconds = new CoverageConfiguration().GetMonitorSleepTimeInSeconds(tcmRequestContext.RequestContext);
      IVssRequestContext requestContext = tcmRequestContext.RequestContext;
      Guid[] jobIds = new Guid[1]{ currentJobId };
      int maxDelaySeconds = sleepTimeInSeconds;
      service.QueueDelayedJobs(requestContext, (IEnumerable<Guid>) jobIds, maxDelaySeconds);
    }

    public static Guid QueueOneTimeJob<T>(TestManagementRequestContext tcmRequestContext, T jobData)
    {
      XmlNode jobData1 = CoverageSerializers.Serialize<T>(tcmRequestContext, jobData);
      string jobName;
      string extensionName;
      CoverageJobHelper.GetJobInfo<T>(jobData, out jobName, out extensionName);
      return tcmRequestContext.RequestContext.GetService<ITeamFoundationJobService>().QueueOneTimeJob(tcmRequestContext.RequestContext, jobName, extensionName, jobData1, JobPriorityLevel.Normal);
    }

    public static void NotifyUsingSignalR(
      TestManagementRequestContext tcmRequestContext,
      int buildId)
    {
      if (!tcmRequestContext.RequestContext.IsFeatureEnabled("TestManagement.Server.EnableAutoRefreshForCodeCoverageTab"))
        return;
      try
      {
        tcmRequestContext.RequestContext.GetService<IBuildCodeCoverageHubDispatcher>().SendCoverageStatsChanged(tcmRequestContext.RequestContext, buildId);
      }
      catch (Exception ex)
      {
        tcmRequestContext.TraceError("CodeCoverageJob", ex.Message);
      }
    }

    public static bool MonitorJobs(
      TestManagementRequestContext tcmRequestContext,
      PipelineContext pipelineContext,
      Guid sourceJobId,
      IEnumerable<Guid> prerequisiteJobs,
      Dictionary<string, object> ciData,
      DateTime monitorQueueTime,
      double monitorJobTimeoutInSeconds,
      out bool timeoutOccurred,
      out string resultMessage)
    {
      timeoutOccurred = false;
      resultMessage = string.Empty;
      TimeSpan timeSpan = DateTime.UtcNow - monitorQueueTime;
      if (timeSpan.TotalSeconds > monitorJobTimeoutInSeconds)
      {
        ciData.Add("piplineId", (object) pipelineContext.Id);
        ciData.Add("JobExecutionTimeSecs", (object) timeSpan.TotalSeconds);
        ciData.Add("MonitorJobTimedOutSecs", (object) monitorJobTimeoutInSeconds);
        tcmRequestContext.Logger.Error(1015799, string.Format("Timeout occurred while monitoring jobs. Project: {0}, PipelineRunId: {1}", (object) pipelineContext.ProjectId, (object) pipelineContext.Id));
        timeoutOccurred = true;
        resultMessage = "Timed out while waiting for prerequisite jobs";
        return true;
      }
      List<TeamFoundationJobHistoryEntry> source = tcmRequestContext.RequestContext.GetService<ITeamFoundationJobService>().QueryLatestJobHistory(tcmRequestContext.RequestContext, prerequisiteJobs);
      if (source == null || source.Count == 0)
      {
        resultMessage = "No JobHistory found.";
        CoverageJobHelper.RequeueJob(tcmRequestContext, sourceJobId, ciData, resultMessage);
        return false;
      }
      if (source.Where<TeamFoundationJobHistoryEntry>((Func<TeamFoundationJobHistoryEntry, bool>) (jobHistory => jobHistory == null)).Any<TeamFoundationJobHistoryEntry>())
      {
        resultMessage = "JobHistory is null.";
        CoverageJobHelper.RequeueJob(tcmRequestContext, sourceJobId, ciData, resultMessage);
        return false;
      }
      foreach (Guid prerequisiteJob in prerequisiteJobs)
      {
        Guid jobId = prerequisiteJob;
        if (!source.Where<TeamFoundationJobHistoryEntry>((Func<TeamFoundationJobHistoryEntry, bool>) (jobHistory => object.Equals((object) jobHistory.JobId, (object) jobId))).Any<TeamFoundationJobHistoryEntry>())
        {
          resultMessage = string.Format("Job {0} not found in history.", (object) jobId);
          CoverageJobHelper.RequeueJob(tcmRequestContext, sourceJobId, ciData, resultMessage);
          return false;
        }
      }
      return true;
    }

    private static void GetJobInfo<T>(
      T jobDataObject,
      out string jobName,
      out string extensionName)
    {
      jobName = (string) null;
      extensionName = (string) null;
      ArgumentUtility.CheckGenericForNull((object) jobDataObject, nameof (jobDataObject));
      if ((object) jobDataObject is MergeJobData)
      {
        jobName = "CoverageMergeJob";
        extensionName = "Microsoft.VisualStudio.Services.Tcm.Plugins.Jobs.CoverageMergeJob";
      }
      else if ((object) jobDataObject is CoverageMonitorJobData)
      {
        jobName = "CoverageMonitorJob";
        extensionName = "Microsoft.VisualStudio.Services.Tcm.Plugins.Jobs.CoverageMonitorJob";
      }
      else if ((object) jobDataObject is MergeInvokerJobData)
      {
        jobName = "CoverageMergeInvokerJob";
        extensionName = "Microsoft.VisualStudio.Services.Tcm.Jobs.CoverageMergeInvokerJob";
      }
      else if ((object) jobDataObject is FileCoverageEvaluationJobData)
      {
        jobName = "FileCoverageEvaluationJob";
        extensionName = "Microsoft.VisualStudio.Services.Tcm.Plugins.Jobs.FileCoverageEvaluationJob";
      }
      else if ((object) jobDataObject is PublishCoveragePRStatusJobData)
      {
        jobName = "PublishCoveragePRStatusJob";
        extensionName = "Microsoft.VisualStudio.Services.Tcm.Jobs.PublishCoveragePRStatusJob";
      }
      else if ((object) jobDataObject is PipelineCoverageEvaluationJobData)
      {
        jobName = "PipelineCoverageEvaluationJob";
        extensionName = "Microsoft.VisualStudio.Services.Tcm.Plugins.Jobs.PipelineCoverageEvaluationJob";
      }
      else if ((object) jobDataObject is PipelineScopeLevelCoverageAggregationJobData)
      {
        jobName = "PipelineScopeLevelFileCoverageAggregationJob";
        extensionName = "Microsoft.VisualStudio.Services.Tcm.Plugins.Jobs.PipelineScopeLevelFileCoverageAggregationJob";
      }
      else
      {
        if (!((object) jobDataObject is FolderViewGeneratorJobData))
          throw new ArgumentException(string.Format("Job data object type {0} is not supported", (object) jobDataObject.GetType()));
        jobName = "FolderViewGeneratorJob";
        extensionName = "Microsoft.VisualStudio.Services.Tcm.Plugins.Jobs.FolderViewGeneratorJob";
      }
    }

    public static ILogStoreContainerProvider GetLogStoreContainerProvider(
      TestManagementRequestContext tcmRequestContext,
      string coverageTool)
    {
      switch (coverageTool)
      {
        case "VstestCoverage":
          return (ILogStoreContainerProvider) new VsTestCoverageLogStoreContainerProvider();
        case "NativeCoverage":
          return (ILogStoreContainerProvider) new NativeCoverageLogStoreContainerProvider();
        case "VstestDotCoverage":
          return (ILogStoreContainerProvider) new VsTestDotCoverageLogStoreContainerProvider();
        default:
          tcmRequestContext.Logger.Error(1015425, "No tool found with Name " + coverageTool);
          return (ILogStoreContainerProvider) new VsTestDotCoverageLogStoreContainerProvider();
      }
    }
  }
}
