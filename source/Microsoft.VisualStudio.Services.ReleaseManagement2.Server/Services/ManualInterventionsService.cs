// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.Services.ManualInterventionsService
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Exceptions;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Properties;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.DistributedTask;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server.Services
{
  public class ManualInterventionsService : ReleaseManagement2ServiceBase
  {
    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Vssf needs it to be non-static")]
    public ManualIntervention GetManualIntervention(
      IVssRequestContext requestContext,
      Guid projectId,
      int releaseId,
      int manualInterventionId)
    {
      return new DataAccessLayer(requestContext, projectId).GetManualIntervention(releaseId, manualInterventionId);
    }

    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Vssf needs it to be non-static")]
    public virtual IList<ManualIntervention> GetManualInterventionsForRelease(
      IVssRequestContext requestContext,
      Guid projectId,
      int releaseId)
    {
      return new DataAccessLayer(requestContext, projectId).GetManualInterventionsForRelease(releaseId);
    }

    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Vssf needs it to be non-static")]
    public virtual ManualIntervention UpdateManualIntervention(
      IVssRequestContext requestContext,
      Guid projectId,
      int releaseId,
      int manualInterventionId,
      ManualInterventionStatus manualInterventionStatus,
      Guid approvedBy,
      string comment,
      string taskIssue)
    {
      if (requestContext == null)
        throw new ArgumentNullException(nameof (requestContext));
      ManualIntervention manualIntervention = new DataAccessLayer(requestContext, projectId).UpdateManualIntervention(releaseId, manualInterventionId, approvedBy, manualInterventionStatus, comment);
      TaskActivityData taskActivityData = manualIntervention.TaskActivityData;
      TaskResult result = manualInterventionStatus == ManualInterventionStatus.Approved ? TaskResult.Succeeded : TaskResult.Failed;
      this.ManualInterventionCompleted(requestContext.Elevate(), projectId, taskActivityData.PlanId, taskActivityData.TimelineId, taskActivityData.JobId, taskActivityData.TaskInstanceId, taskActivityData.TaskName, result, taskIssue);
      requestContext.SendReleaseEnvironmentUpdatedEvent(projectId, manualIntervention.ReleaseDefinitionId, manualIntervention.ReleaseId, manualIntervention.ReleaseEnvironmentId);
      return manualIntervention;
    }

    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Vssf needs it to be non-static")]
    public virtual ManualIntervention CreateManualIntervention(
      IVssRequestContext requestContext,
      Guid projectId,
      int releaseId,
      int releaseEnvironmentId,
      int releaseDeployPhaseId,
      Guid planId,
      Guid timelineId,
      Guid jobId,
      string jobName,
      Guid taskInstanceId,
      string taskName,
      string instructions)
    {
      DataAccessLayer dataAccessLayer = requestContext != null ? new DataAccessLayer(requestContext, projectId) : throw new ArgumentNullException(nameof (requestContext));
      TaskActivityData taskActivityData1 = new TaskActivityData(planId, timelineId, jobId, jobName, taskInstanceId, 0, taskName);
      int releaseId1 = releaseId;
      int releaseEnvironmentId1 = releaseEnvironmentId;
      int releaseDeployPhaseId1 = releaseDeployPhaseId;
      TaskActivityData taskActivityData2 = taskActivityData1;
      string instructions1 = instructions;
      ManualIntervention manualIntervention = dataAccessLayer.AddManualIntervention(releaseId1, releaseEnvironmentId1, releaseDeployPhaseId1, taskActivityData2, instructions1);
      this.ManualInterventionStarted(requestContext.Elevate(), projectId, planId, timelineId, jobId, taskInstanceId, taskName);
      requestContext.SendReleaseEnvironmentUpdatedEvent(projectId, manualIntervention.ReleaseDefinitionId, manualIntervention.ReleaseId, manualIntervention.ReleaseEnvironmentId);
      return manualIntervention;
    }

    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Vssf needs it to be non-static")]
    public virtual void HandleManualInterventionTimeout(
      IVssRequestContext requestContext,
      Guid projectId,
      int releaseId,
      int manualInterventionId,
      double timeoutInMinutes,
      bool resumeOnTimeout)
    {
      ManualIntervention manualIntervention = this.GetManualIntervention(requestContext, projectId, releaseId, manualInterventionId);
      if (manualIntervention.Status != ManualInterventionStatus.Pending)
      {
        string format = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "ManualInterventionTimeoutHandlerJob: ManualIntervention with ReleaseId {0}, ManualInterventionId: {1} not in pending state.", (object) releaseId, (object) manualIntervention.Id);
        ManualInterventionsService.TraceInfo(requestContext, format);
      }
      else
      {
        string taskIssue = string.Empty;
        ManualInterventionStatus manualInterventionStatus;
        string comment;
        if (resumeOnTimeout)
        {
          manualInterventionStatus = ManualInterventionStatus.Approved;
          comment = string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.ManualInterventionTimeoutMessage, (object) manualIntervention.TaskActivityData.TaskName, (object) Resources.ManualInterventionResumed, (object) timeoutInMinutes);
        }
        else
        {
          manualInterventionStatus = ManualInterventionStatus.Rejected;
          comment = string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.ManualInterventionTimeoutMessage, (object) manualIntervention.TaskActivityData.TaskName, (object) Resources.ManualInterventionRejected, (object) timeoutInMinutes);
          taskIssue = comment;
        }
        try
        {
          string format = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "ManualInterventionTimeoutHandlerJob: Updating ReleaseId {0}, ManualInterventionId: {1} manualintervention with status {2} and comments '{3}'.", (object) releaseId, (object) manualIntervention.Id, (object) manualInterventionStatus, (object) comment);
          ManualInterventionsService.TraceInfo(requestContext, format);
          this.UpdateManualIntervention(requestContext, projectId, releaseId, manualIntervention.Id, manualInterventionStatus, Guid.Empty, comment, taskIssue);
        }
        catch (ManualInterventionAlreadyUpdatedException ex)
        {
          string format = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "ManualInterventionTimeoutHandlerJob: ReleaseId {0}, ManualInterventionId: {1} manualintervention already updated.", (object) releaseId, (object) manualIntervention.Id);
          ManualInterventionsService.TraceInfo(requestContext, format);
        }
      }
    }

    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Vssf needs it to be non-static")]
    internal void ManualInterventionStarted(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid planId,
      Guid timelineId,
      Guid jobId,
      Guid taskId,
      string taskName)
    {
      TaskHub releaseTaskHub = requestContext.GetService<IDistributedTaskHubService>().GetReleaseTaskHub(requestContext);
      TimelineRecord timelineRecord = ManualInterventionsService.CreatedTimelineRecord(taskId, "Task", taskName, new Guid?(jobId), TimelineRecordState.Pending, new DateTime?(DateTime.UtcNow));
      IVssRequestContext requestContext1 = requestContext;
      Guid scopeIdentifier = projectId;
      Guid planId1 = planId;
      Guid timelineId1 = timelineId;
      releaseTaskHub.UpdateTimeline(requestContext1, scopeIdentifier, planId1, timelineId1, (IList<TimelineRecord>) new List<TimelineRecord>()
      {
        timelineRecord
      });
    }

    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Vssf needs it to be non-static")]
    internal void ManualInterventionCompleted(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid planId,
      Guid timelineId,
      Guid jobId,
      Guid taskId,
      string taskName,
      TaskResult result,
      string taskIssue)
    {
      TaskHub releaseTaskHub = requestContext.GetService<IDistributedTaskHubService>().GetReleaseTaskHub(requestContext);
      Guid recordId = taskId;
      string name = taskName;
      Guid? parentId = new Guid?(jobId);
      DateTime? nullable1 = new DateTime?(DateTime.UtcNow);
      TaskResult? nullable2 = new TaskResult?(result);
      DateTime? startTime = new DateTime?();
      DateTime? finishTime = nullable1;
      TaskResult? result1 = nullable2;
      TimelineRecord timelineRecord = ManualInterventionsService.CreatedTimelineRecord(recordId, "Task", name, parentId, TimelineRecordState.Completed, startTime, finishTime, result1);
      if (!string.IsNullOrWhiteSpace(taskIssue))
        timelineRecord.Issues.Add(new Microsoft.TeamFoundation.DistributedTask.WebApi.Issue()
        {
          Type = Microsoft.TeamFoundation.DistributedTask.WebApi.IssueType.Error,
          Message = taskIssue
        });
      releaseTaskHub.UpdateTimeline(requestContext, projectId, planId, timelineId, (IList<TimelineRecord>) new List<TimelineRecord>()
      {
        timelineRecord
      });
      releaseTaskHub.RaiseTaskCompletedEvent(requestContext, projectId, planId, jobId, taskId, new TaskCompletedEvent(jobId, taskId, result));
    }

    private static TimelineRecord CreatedTimelineRecord(
      Guid recordId,
      string type,
      string name,
      Guid? parentId,
      TimelineRecordState state,
      DateTime? startTime = null,
      DateTime? finishTime = null,
      TaskResult? result = null)
    {
      return new TimelineRecord()
      {
        Id = recordId,
        Name = name,
        RecordType = type,
        ParentId = parentId,
        StartTime = startTime,
        FinishTime = finishTime,
        State = new TimelineRecordState?(state),
        Result = result
      };
    }

    private static void TraceInfo(
      IVssRequestContext requestContext,
      string format,
      params object[] args)
    {
      VssRequestContextExtensions.Trace(requestContext, 1972118, TraceLevel.Info, "ReleaseManagementService", "JobLayer", format, args);
    }
  }
}
