// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.DistributedTask.TimelineRecordContext
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server.DistributedTask
{
  public sealed class TimelineRecordContext : ITimelineRecordContext, IDisposable
  {
    private readonly TaskHub hub;
    private readonly Guid projectId;
    private readonly Guid planId;
    private readonly Guid timelineId;
    private readonly Guid jobRecordId;
    private readonly TimelineRecord record;
    private TaskResult result;
    private StringBuilder logLines;
    private IVssRequestContext requestContext;

    public TimelineRecordContext(
      IVssRequestContext requestContext,
      TaskHub taskHub,
      Guid projectId,
      Guid planId,
      Guid timelineId,
      Guid jobRecordId,
      TimelineRecord record)
    {
      this.requestContext = requestContext;
      this.hub = taskHub;
      this.projectId = projectId;
      this.planId = planId;
      this.timelineId = timelineId;
      this.jobRecordId = jobRecordId;
      this.record = record;
    }

    public Guid Id => this.record.Id;

    public void Dispose()
    {
      if (this.requestContext == null)
        return;
      if (this.logLines != null && this.logLines.Length > 0)
      {
        TaskLog log = this.hub.CreateLog(this.requestContext, this.projectId, this.planId, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "logs\\{0:D}", (object) this.record.Id));
        using (MemoryStream content = new MemoryStream())
        {
          byte[] bytes = Encoding.UTF8.GetBytes(this.logLines.ToString());
          content.Write(bytes, 0, bytes.Length);
          content.Position = 0L;
          this.hub.AppendLog(this.requestContext, this.projectId, this.planId, log.Id, (Stream) content);
        }
        this.record.Log = (TaskLogReference) log;
      }
      this.record.FinishTime = new DateTime?(DateTime.UtcNow);
      this.record.State = new TimelineRecordState?(TimelineRecordState.Completed);
      this.record.Result = new TaskResult?(this.result);
      this.hub.UpdateTimeline(this.requestContext, this.projectId, this.planId, this.timelineId, (IList<TimelineRecord>) new TimelineRecord[1]
      {
        this.record
      });
      this.requestContext = (IVssRequestContext) null;
    }

    public void WriteError(string format, params object[] arguments)
    {
      string message = TimelineRecordContext.GetMessage(format, arguments);
      this.record.Issues.Add(new Issue()
      {
        Type = IssueType.Error,
        Message = message
      });
      this.result = TaskResult.Failed;
      this.WriteLine("[Error]{0}", new object[1]
      {
        (object) message
      });
    }

    public void WriteWarning(string format, params object[] arguments)
    {
      string message = TimelineRecordContext.GetMessage(format, arguments);
      this.record.Issues.Add(new Issue()
      {
        Type = IssueType.Warning,
        Message = message
      });
      if (this.result != TaskResult.Failed)
        this.result = TaskResult.SucceededWithIssues;
      this.WriteLine("[Warning]{0}", new object[1]
      {
        (object) message
      });
    }

    public void WriteLine(string format, params object[] arguments)
    {
      if (this.logLines == null)
        this.logLines = new StringBuilder();
      string message = TimelineRecordContext.GetMessage(format, arguments);
      this.logLines.AppendLine(message);
      this.hub.FeedReceived(this.requestContext, this.projectId, this.planId, this.timelineId, this.jobRecordId, this.record.Id, (IList<string>) new string[1]
      {
        message
      });
    }

    private static string GetMessage(string format, params object[] arguments) => arguments == null || arguments.Length == 0 ? format : string.Format((IFormatProvider) CultureInfo.InvariantCulture, format, arguments);
  }
}
