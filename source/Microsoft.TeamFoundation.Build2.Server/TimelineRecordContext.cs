// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.TimelineRecordContext
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace Microsoft.TeamFoundation.Build2.Server
{
  internal class TimelineRecordContext : ITimelineRecordContext, IDisposable
  {
    private TaskHub m_hub;
    private Guid m_projectId;
    private Guid m_planId;
    private Guid m_timelineId;
    private Guid m_jobRecordId;
    private TaskResult m_result;
    private TimelineRecord m_record;
    private StringBuilder m_logLines;
    private IVssRequestContext m_requestContext;
    private ITimelineRecordContext m_parentContext;

    public TimelineRecordContext(
      IVssRequestContext requestContext,
      TaskHub taskHub,
      Guid projectId,
      Guid planId,
      Guid timelineId,
      Guid jobRecordId,
      TimelineRecord record,
      ITimelineRecordContext parentContext)
    {
      this.m_requestContext = requestContext;
      this.m_hub = taskHub;
      this.m_projectId = projectId;
      this.m_planId = planId;
      this.m_timelineId = timelineId;
      this.m_jobRecordId = jobRecordId;
      this.m_record = record;
      this.m_parentContext = parentContext;
    }

    public Guid Id => this.m_record.Id;

    public void Dispose()
    {
      if (this.m_requestContext == null)
        return;
      if (this.m_logLines != null && this.m_logLines.Length > 0)
      {
        TaskLog log = this.m_hub.CreateLog(this.m_requestContext, this.m_projectId, this.m_planId, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "logs\\{0:D}", (object) this.m_record.Id));
        using (MemoryStream content = new MemoryStream())
        {
          byte[] bytes = Encoding.UTF8.GetBytes(this.m_logLines.ToString());
          content.Write(bytes, 0, bytes.Length);
          content.Position = 0L;
          this.m_hub.AppendLog(this.m_requestContext, this.m_projectId, this.m_planId, log.Id, (Stream) content);
        }
        this.m_record.Log = (TaskLogReference) log;
      }
      TimelineRecordState? state = this.m_record.State;
      TimelineRecordState timelineRecordState = TimelineRecordState.Completed;
      if (!(state.GetValueOrDefault() == timelineRecordState & state.HasValue))
      {
        this.m_record.FinishTime = new DateTime?(DateTime.UtcNow);
        this.m_record.State = new TimelineRecordState?(TimelineRecordState.Completed);
        this.m_record.Result = new TaskResult?(this.m_result);
      }
      List<TimelineRecord> records = new List<TimelineRecord>((IEnumerable<TimelineRecord>) new TimelineRecord[1]
      {
        this.m_record
      });
      TaskResult? result = this.m_record.Result;
      TaskResult taskResult1 = TaskResult.Failed;
      if (result.GetValueOrDefault() == taskResult1 & result.HasValue)
      {
        if (this.m_parentContext != null)
        {
          this.m_parentContext.PropagateError();
        }
        else
        {
          Timeline timeline = this.m_hub.GetTimeline(this.m_requestContext, this.m_projectId, this.m_planId, this.m_timelineId, includeRecords: true);
          Guid? parentId = this.m_record.ParentId;
          while (parentId.HasValue)
          {
            TimelineRecord timelineRecord = timeline?.Records.Find((Predicate<TimelineRecord>) (record =>
            {
              Guid id = record.Id;
              Guid? nullable = parentId;
              return nullable.HasValue && id == nullable.GetValueOrDefault();
            }));
            if (timelineRecord != null)
            {
              result = timelineRecord.Result;
              TaskResult taskResult2 = TaskResult.Failed;
              if (!(result.GetValueOrDefault() == taskResult2 & result.HasValue))
              {
                timelineRecord.Result = new TaskResult?(TaskResult.Failed);
                parentId = timelineRecord.ParentId;
                records.Add(timelineRecord);
                continue;
              }
            }
            parentId = new Guid?();
          }
        }
      }
      this.m_hub.UpdateTimeline(this.m_requestContext, this.m_projectId, this.m_planId, this.m_timelineId, (IList<TimelineRecord>) records);
      this.m_requestContext = (IVssRequestContext) null;
    }

    public void PropagateError()
    {
      this.m_result = TaskResult.Failed;
      TimelineRecordState? state = this.m_record.State;
      TimelineRecordState timelineRecordState = TimelineRecordState.Completed;
      if (!(state.GetValueOrDefault() == timelineRecordState & state.HasValue))
        return;
      this.m_record.Result = new TaskResult?(this.m_result);
    }

    public void WriteError(string format, params object[] arguments)
    {
      string message = TimelineRecordContext.GetMessage(format, arguments);
      this.m_record.Issues.Add(new Issue()
      {
        Type = IssueType.Error,
        Message = message
      });
      this.m_result = TaskResult.Failed;
      this.WriteLine("[Error]{0}", new object[1]
      {
        (object) message
      });
    }

    public void WriteWarning(string format, params object[] arguments)
    {
      string message = TimelineRecordContext.GetMessage(format, arguments);
      this.m_record.Issues.Add(new Issue()
      {
        Type = IssueType.Warning,
        Message = message
      });
      if (this.m_result != TaskResult.Failed)
        this.m_result = TaskResult.SucceededWithIssues;
      this.WriteLine("[Warning]{0}", new object[1]
      {
        (object) message
      });
    }

    public void WriteLine(string format, params object[] arguments)
    {
      if (this.m_logLines == null)
        this.m_logLines = new StringBuilder();
      string message = TimelineRecordContext.GetMessage(format, arguments);
      this.m_logLines.AppendLine(message);
      this.m_hub.FeedReceived(this.m_requestContext, this.m_projectId, this.m_planId, this.m_timelineId, this.m_jobRecordId, this.m_record.Id, (IList<string>) new string[1]
      {
        message
      });
    }

    private static string GetMessage(string format, params object[] arguments) => arguments == null || arguments.Length == 0 ? format : string.Format((IFormatProvider) CultureInfo.InvariantCulture, format, arguments);
  }
}
