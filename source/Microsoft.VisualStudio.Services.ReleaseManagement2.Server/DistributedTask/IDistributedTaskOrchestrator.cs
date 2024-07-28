// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.DistributedTask.IDistributedTaskOrchestrator
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server.DistributedTask
{
  public interface IDistributedTaskOrchestrator
  {
    IEnumerable<TimelineRecord> GetTimelineRecords(Guid runPlanId);

    IEnumerable<TaskLog> GetLogs(Guid runPlanId);

    TaskLog GetLog(Guid runPlanId, int logId);

    [SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "4#", Justification = "Have to mantain same signature as DistributedTaskOrchestrationService.")]
    [SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "5#", Justification = "Have to mantain same signature as DistributedTaskOrchestrationService.")]
    [SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "4#", Justification = "Have to mantain same signature as DistributedTaskOrchestrationService.")]
    [SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "2#", Justification = "Have to mantain same signature as DistributedTaskOrchestrationService.")]
    [SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "3#", Justification = "Have to mantain same signature as DistributedTaskOrchestrationService.")]
    TeamFoundationDataReader GetLoglines(
      Guid planId,
      int logId,
      ref long startLine,
      ref long endLine,
      out long totalLines);

    IEnumerable<TaskAttachment> GetAttachments(Guid planId, string type);

    Stream GetAttachment(Guid planId, TaskAttachment attachment);

    Guid StartDeployment(AutomationEngineInput input);

    bool CancelDeployment(Guid planId, TimeSpan jobCancelTimeout);

    Dictionary<Guid, string> GetJobIdNameMap(IEnumerable<TimelineRecord> timelineRecords);

    IList<OutputVariableScope> GetOutputVariables(IList<Guid> planIds);

    IDictionary<string, string> GetSystemVariables(AutomationEngineInput input);

    ITimelineRecordContext StartJob(
      IVssRequestContext requestContext,
      Guid jobId,
      string jobName,
      Guid projectId,
      Guid planId);

    ITimelineRecordContext StartTask(
      IVssRequestContext requestContext,
      ITimelineRecordContext parentContext,
      Guid taskId,
      string taskName,
      Guid projectId,
      Guid planId);

    string GetLogRelativeFilePath(TimelineRecord timelineRecord, string folderName);
  }
}
