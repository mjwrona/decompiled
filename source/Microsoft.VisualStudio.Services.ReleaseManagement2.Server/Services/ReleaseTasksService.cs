// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.Services.ReleaseTasksService
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Common.Helpers;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Exceptions;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.DistributedTask;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Extensions;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Properties;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server.Services
{
  public class ReleaseTasksService : ReleaseManagement2ServiceBase
  {
    private readonly IReleaseTasksServiceWireUp wireUp;

    public ReleaseTasksService()
      : this((IReleaseTasksServiceWireUp) new ReleaseTasksServiceWireup())
    {
    }

    protected ReleaseTasksService(IReleaseTasksServiceWireUp wireUp) => this.wireUp = wireUp;

    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Vssf needs it to be non-static")]
    public IEnumerable<ReleaseTask> GetTasks(
      IVssRequestContext requestContext,
      Guid projectId,
      int releaseId,
      int environmentId,
      int releaseDeployPhaseId,
      int attemptId)
    {
      if (requestContext == null)
        throw new ArgumentNullException(nameof (requestContext));
      List<ReleaseTask> tasks = new List<ReleaseTask>();
      ReleaseTasksService.TraceMessage(requestContext, 1973117, "Getting plan ids for release {0}, environment {1} and attempt {2}", (object) releaseId, (object) environmentId, (object) attemptId);
      IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseDeployPhase> deployPhases = this.wireUp.GetDeployPhases(requestContext, projectId, releaseId, environmentId, releaseDeployPhaseId, attemptId);
      ReleaseTasksService.TraceMessage(requestContext, 1973118, "Got plan ids for release {0}, environment {1} and attempt {2}", (object) releaseId, (object) environmentId, (object) attemptId);
      foreach (Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseDeployPhase releaseDeployPhase in deployPhases)
      {
        if (releaseDeployPhase.Status != Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.DeployPhaseStatus.Skipped)
        {
          if (!releaseDeployPhase.RunPlanId.HasValue || releaseDeployPhase.RunPlanId.Equals((object) Guid.Empty))
            throw new ReleaseManagementObjectNotFoundException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.RunPlanIdNotFound, (object) releaseId, (object) environmentId, (object) attemptId));
          IDistributedTaskOrchestrator taskOrchestrator = DistributedTaskOrchestrator.CreateDistributedTaskOrchestrator(requestContext, projectId, releaseDeployPhase.PhaseType);
          string phaseErrorLog;
          IList<ReleaseTask> tasksFromPlan = this.GetTasksFromPlan(requestContext, taskOrchestrator, projectId, releaseDeployPhase.RunPlanId.Value, out phaseErrorLog);
          releaseDeployPhase.Logs = phaseErrorLog;
          foreach (ReleaseTask releaseTask in (IEnumerable<ReleaseTask>) tasksFromPlan)
            releaseTask.LogUrl = WebAccessUrlBuilder.GetReleaseTaskLogUrl(requestContext, projectId, releaseId, environmentId, releaseDeployPhase.Id, releaseTask.Id);
          tasks.AddRange((IEnumerable<ReleaseTask>) tasksFromPlan);
        }
      }
      return (IEnumerable<ReleaseTask>) tasks;
    }

    [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "to be refactored.")]
    public IEnumerable<ReleaseTask> GetTasks(
      IVssRequestContext requestContext,
      Guid projectId,
      int releaseId,
      int environmentId,
      Guid timelineId)
    {
      List<ReleaseTask> enumerable = new List<ReleaseTask>();
      if (requestContext == null)
        throw new ArgumentNullException(nameof (requestContext));
      ReleaseLogContainers logContainer = this.wireUp.GetLogContainer(requestContext, projectId, releaseId, environmentId);
      string phaseErrorLog;
      if (logContainer.DeployPhases.Any<ReleaseDeployPhaseRef>())
      {
        ReleaseDeployPhaseRef releaseDeployPhaseRef = logContainer.DeployPhases.FirstOrDefault<ReleaseDeployPhaseRef>((Func<ReleaseDeployPhaseRef, bool>) (phase => phase.PlanId == timelineId));
        if (releaseDeployPhaseRef != null)
        {
          Guid planId = releaseDeployPhaseRef.PlanId;
          IDistributedTaskOrchestrator taskOrchestrator = DistributedTaskOrchestrator.CreateDistributedTaskOrchestrator(requestContext, projectId, releaseDeployPhaseRef.PhaseType);
          IList<ReleaseTask> tasksFromPlan = this.GetTasksFromPlan(requestContext, taskOrchestrator, projectId, releaseDeployPhaseRef.PlanId, out phaseErrorLog);
          foreach (ReleaseTask releaseTask in (IEnumerable<ReleaseTask>) tasksFromPlan)
            releaseTask.LogUrl = WebAccessUrlBuilder.GetReleaseTaskLogUrl(requestContext, projectId, releaseId, environmentId, releaseDeployPhaseRef.PhaseId, releaseTask.Id);
          enumerable.AddRange((IEnumerable<ReleaseTask>) tasksFromPlan);
        }
      }
      if (enumerable.IsNullOrEmpty<ReleaseTask>() && logContainer.DeploySteps.Any<ReleaseEnvironmentStep>())
      {
        ReleaseEnvironmentStep releaseEnvironmentStep = logContainer.DeploySteps.FirstOrDefault<ReleaseEnvironmentStep>((Func<ReleaseEnvironmentStep, bool>) (step =>
        {
          if (step.ReleaseEnvironmentId != environmentId)
            return false;
          Guid? runPlanId = step.RunPlanId;
          Guid guid = timelineId;
          if (!runPlanId.HasValue)
            return false;
          return !runPlanId.HasValue || runPlanId.GetValueOrDefault() == guid;
        }));
        if (releaseEnvironmentStep != null && releaseEnvironmentStep.RunPlanId.HasValue)
        {
          Guid valueOrDefault = releaseEnvironmentStep.RunPlanId.GetValueOrDefault();
          IList<ReleaseTask> tasksFromPlan = this.GetTasksFromPlan(requestContext, (IDistributedTaskOrchestrator) new PipelineOrchestrator(requestContext, projectId), projectId, valueOrDefault, out phaseErrorLog);
          foreach (ReleaseTask releaseTask in (IEnumerable<ReleaseTask>) tasksFromPlan)
            releaseTask.LogUrl = WebAccessUrlBuilder.GetPipelineTaskLogUrl(requestContext, projectId, releaseId, environmentId, releaseEnvironmentStep.TrialNumber, releaseTask.TimelineRecordId);
          enumerable.AddRange((IEnumerable<ReleaseTask>) tasksFromPlan);
        }
      }
      if (enumerable.IsNullOrEmpty<ReleaseTask>() && logContainer.Gates.Any<DeploymentGateRef>())
      {
        DeploymentGateRef deploymentGateRef = logContainer.Gates.FirstOrDefault<DeploymentGateRef>((Func<DeploymentGateRef, bool>) (gate =>
        {
          Guid? runPlanId = gate.RunPlanId;
          Guid guid = timelineId;
          if (!runPlanId.HasValue)
            return false;
          return !runPlanId.HasValue || runPlanId.GetValueOrDefault() == guid;
        }));
        if (deploymentGateRef != null && deploymentGateRef.RunPlanId.HasValue)
        {
          Guid valueOrDefault = deploymentGateRef.RunPlanId.GetValueOrDefault();
          IList<ReleaseTask> tasksFromPlan = this.GetTasksFromPlan(requestContext, (IDistributedTaskOrchestrator) new PipelineOrchestrator(requestContext, projectId), projectId, valueOrDefault, out phaseErrorLog);
          foreach (ReleaseTask releaseTask in (IEnumerable<ReleaseTask>) tasksFromPlan)
            releaseTask.LogUrl = WebAccessUrlBuilder.GetDeploymentGateLogUrl(requestContext, projectId, releaseId, environmentId, deploymentGateRef.Id, releaseTask.Id);
          enumerable.AddRange((IEnumerable<ReleaseTask>) tasksFromPlan);
        }
      }
      return !enumerable.IsNullOrEmpty<ReleaseTask>() ? (IEnumerable<ReleaseTask>) enumerable : throw new ReleaseManagementObjectNotFoundException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.NoTimeLineExistsForTasks, (object) releaseId, (object) environmentId, (object) timelineId));
    }

    private static void TraceMessage(
      IVssRequestContext requestContext,
      int tracePoint,
      string format,
      params object[] args)
    {
      VssRequestContextExtensions.Trace(requestContext, tracePoint, TraceLevel.Verbose, "ReleaseManagementService", "Service", format, args);
    }

    private IList<ReleaseTask> GetTasksFromPlan(
      IVssRequestContext requestContext,
      IDistributedTaskOrchestrator distributedTaskOrchestrator,
      Guid projectId,
      Guid planId,
      out string phaseErrorLog)
    {
      List<ReleaseTask> tasksFromPlan = new List<ReleaseTask>();
      ReleaseTasksService.TraceMessage(requestContext, 1973119, "Getting timeline records for plan id {0}", (object) planId);
      IEnumerable<TimelineRecord> timelineRecords = this.wireUp.GetTimelineRecords(distributedTaskOrchestrator, projectId, planId);
      ReleaseTasksService.TraceMessage(requestContext, 1973120, "Got timeline records for plan id {0}", (object) planId);
      if (timelineRecords == null || !timelineRecords.Any<TimelineRecord>())
      {
        phaseErrorLog = (string) null;
        return (IList<ReleaseTask>) tasksFromPlan;
      }
      ReleaseTasksService.TraceMessage(requestContext, 1973121, "Getting log records for plan id {0}", (object) planId);
      IEnumerable<TaskLog> logs = this.wireUp.GetLogs(distributedTaskOrchestrator, projectId, planId);
      ReleaseTasksService.TraceMessage(requestContext, 1973122, "Got log records for plan id {0}", (object) planId);
      List<DeploymentJob> list = timelineRecords.ToDeployStepRecords(out phaseErrorLog).ToList<DeploymentJob>();
      foreach (ReleaseTask releaseTask1 in list.GetAllTasks().Concat<ReleaseTask>(list.GetAllJobs()))
      {
        ReleaseTask releaseTask = releaseTask1;
        TaskLog taskLog = logs.SingleOrDefault<TaskLog>((Func<TaskLog, bool>) (t => t.Id == releaseTask.Id));
        releaseTask.LineCount = taskLog == null ? 0L : taskLog.LineCount;
        tasksFromPlan.Add(releaseTask);
      }
      return (IList<ReleaseTask>) tasksFromPlan;
    }
  }
}
