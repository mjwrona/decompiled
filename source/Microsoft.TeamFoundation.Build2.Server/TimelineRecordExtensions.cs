// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.TimelineRecordExtensions
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.TeamFoundation.Build.WebApi;
using Microsoft.TeamFoundation.Build2.Routes;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Build2.Server
{
  public static class TimelineRecordExtensions
  {
    public static Microsoft.TeamFoundation.Build.WebApi.TimelineRecord ToBuildTimelineRecord(
      this Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecord taskTimelineRecord,
      IVssRequestContext requestContext,
      Guid projectId,
      int buildId,
      ISecuredObject securedObject)
    {
      using (PerformanceTimer.StartMeasure(requestContext, "TimelineRecordExtensions.ToBuildTimelineRecord"))
      {
        ArgumentUtility.CheckForNull<ISecuredObject>(securedObject, nameof (securedObject));
        if (taskTimelineRecord == null)
          return (Microsoft.TeamFoundation.Build.WebApi.TimelineRecord) null;
        Microsoft.TeamFoundation.Build.WebApi.TimelineRecord buildTimelineRecord = new Microsoft.TeamFoundation.Build.WebApi.TimelineRecord(securedObject)
        {
          Attempt = taskTimelineRecord.Attempt,
          Identifier = taskTimelineRecord.Identifier,
          ChangeId = taskTimelineRecord.ChangeId,
          CurrentOperation = taskTimelineRecord.CurrentOperation,
          ErrorCount = taskTimelineRecord.ErrorCount,
          FinishTime = taskTimelineRecord.FinishTime,
          Id = taskTimelineRecord.Id,
          LastModified = taskTimelineRecord.LastModified,
          Url = taskTimelineRecord.Location,
          Name = taskTimelineRecord.Name,
          Order = taskTimelineRecord.Order,
          ParentId = taskTimelineRecord.ParentId,
          PercentComplete = taskTimelineRecord.PercentComplete,
          RecordType = taskTimelineRecord.RecordType,
          Result = taskTimelineRecord.Result.ToBuildTaskResult(),
          ResultCode = taskTimelineRecord.ResultCode,
          StartTime = taskTimelineRecord.StartTime,
          State = taskTimelineRecord.State.ToBuildTimelineRecordState(),
          WarningCount = taskTimelineRecord.WarningCount,
          WorkerName = taskTimelineRecord.WorkerName,
          QueueId = taskTimelineRecord.QueueId
        };
        if (taskTimelineRecord.Issues.Count > 0)
          buildTimelineRecord.Issues.AddRange(taskTimelineRecord.Issues.Select<Microsoft.TeamFoundation.DistributedTask.WebApi.Issue, Microsoft.TeamFoundation.Build.WebApi.Issue>((Func<Microsoft.TeamFoundation.DistributedTask.WebApi.Issue, Microsoft.TeamFoundation.Build.WebApi.Issue>) (issue => issue.ToBuildIssue(securedObject))));
        IBuildRouteService service = requestContext.GetService<IBuildRouteService>();
        if (taskTimelineRecord.Log != null)
          buildTimelineRecord.Log = new BuildLogReference(securedObject)
          {
            Id = taskTimelineRecord.Log.Id,
            Type = "Container",
            Url = service.GetBuildLogRestUrl(requestContext, projectId, buildId, taskTimelineRecord.Log.Id)
          };
        if (taskTimelineRecord.Task != null)
          buildTimelineRecord.Task = new Microsoft.TeamFoundation.Build.WebApi.TaskReference(securedObject)
          {
            Id = taskTimelineRecord.Task.Id,
            Name = taskTimelineRecord.Task.Name,
            Version = taskTimelineRecord.Task.Version
          };
        if (taskTimelineRecord.Details != null)
          buildTimelineRecord.Details = new Microsoft.TeamFoundation.Build.WebApi.TimelineReference(securedObject)
          {
            Id = taskTimelineRecord.Details.Id,
            ChangeId = taskTimelineRecord.Details.ChangeId,
            Url = service.GetTimelineRestUrl(requestContext, projectId, buildId, new Guid?(taskTimelineRecord.Details.Id))
          };
        buildTimelineRecord.PreviousAttempts.AddRange<Microsoft.TeamFoundation.Build.WebApi.TimelineAttempt, IList<Microsoft.TeamFoundation.Build.WebApi.TimelineAttempt>>(taskTimelineRecord.PreviousAttempts.Select<Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineAttempt, Microsoft.TeamFoundation.Build.WebApi.TimelineAttempt>((Func<Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineAttempt, Microsoft.TeamFoundation.Build.WebApi.TimelineAttempt>) (attempt => attempt.ToBuildTimelineAttempt(securedObject))));
        return buildTimelineRecord;
      }
    }

    public static Microsoft.TeamFoundation.Build.WebApi.TaskResult? ToBuildTaskResult(
      this Microsoft.TeamFoundation.DistributedTask.WebApi.TaskResult? taskResult)
    {
      if (!taskResult.HasValue)
        return new Microsoft.TeamFoundation.Build.WebApi.TaskResult?();
      switch (taskResult.Value)
      {
        case Microsoft.TeamFoundation.DistributedTask.WebApi.TaskResult.Succeeded:
          return new Microsoft.TeamFoundation.Build.WebApi.TaskResult?(Microsoft.TeamFoundation.Build.WebApi.TaskResult.Succeeded);
        case Microsoft.TeamFoundation.DistributedTask.WebApi.TaskResult.SucceededWithIssues:
          return new Microsoft.TeamFoundation.Build.WebApi.TaskResult?(Microsoft.TeamFoundation.Build.WebApi.TaskResult.SucceededWithIssues);
        case Microsoft.TeamFoundation.DistributedTask.WebApi.TaskResult.Canceled:
          return new Microsoft.TeamFoundation.Build.WebApi.TaskResult?(Microsoft.TeamFoundation.Build.WebApi.TaskResult.Canceled);
        case Microsoft.TeamFoundation.DistributedTask.WebApi.TaskResult.Skipped:
          return new Microsoft.TeamFoundation.Build.WebApi.TaskResult?(Microsoft.TeamFoundation.Build.WebApi.TaskResult.Skipped);
        case Microsoft.TeamFoundation.DistributedTask.WebApi.TaskResult.Abandoned:
          return new Microsoft.TeamFoundation.Build.WebApi.TaskResult?(Microsoft.TeamFoundation.Build.WebApi.TaskResult.Abandoned);
        default:
          return new Microsoft.TeamFoundation.Build.WebApi.TaskResult?(Microsoft.TeamFoundation.Build.WebApi.TaskResult.Failed);
      }
    }

    public static Microsoft.TeamFoundation.Build.WebApi.TimelineRecordState? ToBuildTimelineRecordState(
      this Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecordState? timelineRecordState)
    {
      if (!timelineRecordState.HasValue)
        return new Microsoft.TeamFoundation.Build.WebApi.TimelineRecordState?();
      switch (timelineRecordState.Value)
      {
        case Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecordState.InProgress:
          return new Microsoft.TeamFoundation.Build.WebApi.TimelineRecordState?(Microsoft.TeamFoundation.Build.WebApi.TimelineRecordState.InProgress);
        case Microsoft.TeamFoundation.DistributedTask.WebApi.TimelineRecordState.Completed:
          return new Microsoft.TeamFoundation.Build.WebApi.TimelineRecordState?(Microsoft.TeamFoundation.Build.WebApi.TimelineRecordState.Completed);
        default:
          return new Microsoft.TeamFoundation.Build.WebApi.TimelineRecordState?(Microsoft.TeamFoundation.Build.WebApi.TimelineRecordState.Pending);
      }
    }
  }
}
