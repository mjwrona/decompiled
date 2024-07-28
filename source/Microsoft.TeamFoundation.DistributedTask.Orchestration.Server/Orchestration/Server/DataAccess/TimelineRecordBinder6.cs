// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.DataAccess.TimelineRecordBinder6
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.DataAccess
{
  internal class TimelineRecordBinder6 : ObjectBinder<TimelineRecord>
  {
    private SqlColumnBinder m_timelineId = new SqlColumnBinder("TimelineId");
    private SqlColumnBinder m_recordId = new SqlColumnBinder("RecordId");
    private SqlColumnBinder m_parentId = new SqlColumnBinder("ParentId");
    private SqlColumnBinder m_changeId = new SqlColumnBinder("ChangeId");
    private SqlColumnBinder m_type = new SqlColumnBinder("Type");
    private SqlColumnBinder m_name = new SqlColumnBinder("Name");
    private SqlColumnBinder m_startTime = new SqlColumnBinder("StartTime");
    private SqlColumnBinder m_finishTime = new SqlColumnBinder("FinishTime");
    private SqlColumnBinder m_currentOperation = new SqlColumnBinder("CurrentOperation");
    private SqlColumnBinder m_percentComplete = new SqlColumnBinder("PercentComplete");
    private SqlColumnBinder m_state = new SqlColumnBinder("State");
    private SqlColumnBinder m_result = new SqlColumnBinder("Result");
    private SqlColumnBinder m_resultCode = new SqlColumnBinder("ResultCode");
    private SqlColumnBinder m_workerName = new SqlColumnBinder("WorkerName");
    private SqlColumnBinder m_detailTimelineId = new SqlColumnBinder("DetailTimelineId");
    private SqlColumnBinder m_detailTimelineChangeId = new SqlColumnBinder("DetailTimelineChangeId");
    private SqlColumnBinder m_logId = new SqlColumnBinder("LogId");
    private SqlColumnBinder m_attempt = new SqlColumnBinder("Attempt");
    private SqlColumnBinder m_order = new SqlColumnBinder("Order");
    private SqlColumnBinder m_identifier = new SqlColumnBinder("Identifier");
    private SqlColumnBinder m_errorCount = new SqlColumnBinder("ErrorCount");
    private SqlColumnBinder m_warningCount = new SqlColumnBinder("WarningCount");
    private SqlColumnBinder m_issues = new SqlColumnBinder("Issues");
    private SqlColumnBinder m_taskId = new SqlColumnBinder("TaskId");
    private SqlColumnBinder m_taskName = new SqlColumnBinder("TaskName");
    private SqlColumnBinder m_taskVersion = new SqlColumnBinder("TaskVersion");
    private SqlColumnBinder m_refName = new SqlColumnBinder("RefName");

    protected override TimelineRecord Bind()
    {
      TimelineRecord timelineRecord1 = new TimelineRecord();
      timelineRecord1.ChangeId = this.m_changeId.GetInt32((IDataReader) this.Reader);
      timelineRecord1.Id = this.m_recordId.GetGuid((IDataReader) this.Reader);
      timelineRecord1.ParentId = this.m_parentId.GetNullableGuid((IDataReader) this.Reader);
      timelineRecord1.RecordType = this.m_type.GetString((IDataReader) this.Reader, false);
      timelineRecord1.Name = this.m_name.GetString((IDataReader) this.Reader, false);
      timelineRecord1.RefName = this.m_refName.GetString((IDataReader) this.Reader, true);
      timelineRecord1.StartTime = this.m_startTime.GetNullableDateTime((IDataReader) this.Reader);
      timelineRecord1.FinishTime = this.m_finishTime.GetNullableDateTime((IDataReader) this.Reader);
      timelineRecord1.CurrentOperation = this.m_currentOperation.GetString((IDataReader) this.Reader, true);
      timelineRecord1.PercentComplete = this.m_percentComplete.GetNullableInt32((IDataReader) this.Reader);
      timelineRecord1.State = new TimelineRecordState?((TimelineRecordState) this.m_state.GetByte((IDataReader) this.Reader));
      TimelineRecord timelineRecord2 = timelineRecord1;
      byte? nullableByte = this.m_result.GetNullableByte((IDataReader) this.Reader);
      TaskResult? nullable = nullableByte.HasValue ? new TaskResult?((TaskResult) nullableByte.GetValueOrDefault()) : new TaskResult?();
      timelineRecord2.Result = nullable;
      timelineRecord1.ResultCode = this.m_resultCode.GetString((IDataReader) this.Reader, true);
      timelineRecord1.WorkerName = this.m_workerName.GetString((IDataReader) this.Reader, true);
      Guid? nullableGuid1 = this.m_detailTimelineId.GetNullableGuid((IDataReader) this.Reader);
      if (nullableGuid1.HasValue)
        timelineRecord1.Details = new TimelineReference()
        {
          Id = nullableGuid1.Value,
          ChangeId = this.m_detailTimelineChangeId.GetInt32((IDataReader) this.Reader)
        };
      int? nullableInt32 = this.m_logId.GetNullableInt32((IDataReader) this.Reader);
      if (nullableInt32.HasValue)
        timelineRecord1.Log = new TaskLogReference()
        {
          Id = nullableInt32.Value
        };
      timelineRecord1.Attempt = this.m_attempt.GetInt32((IDataReader) this.Reader);
      timelineRecord1.Order = this.m_order.GetNullableInt32((IDataReader) this.Reader);
      timelineRecord1.Identifier = this.m_identifier.GetString((IDataReader) this.Reader, true);
      timelineRecord1.ErrorCount = new int?(this.m_errorCount.GetInt32((IDataReader) this.Reader));
      timelineRecord1.WarningCount = new int?(this.m_warningCount.GetInt32((IDataReader) this.Reader));
      if (!this.m_issues.IsNull((IDataReader) this.Reader))
      {
        List<Issue> collection = JsonUtility.Deserialize<List<Issue>>(this.m_issues.GetBytes((IDataReader) this.Reader, false));
        if (collection != null)
          timelineRecord1.Issues.AddRange((IEnumerable<Issue>) collection);
      }
      Guid? nullableGuid2 = this.m_taskId.GetNullableGuid((IDataReader) this.Reader);
      string str1 = this.m_taskName.GetString((IDataReader) this.Reader, true);
      string str2 = this.m_taskVersion.GetString((IDataReader) this.Reader, true);
      if (nullableGuid2.HasValue && !string.IsNullOrEmpty(str2))
        timelineRecord1.Task = new TaskReference()
        {
          Id = nullableGuid2.Value,
          Name = str1,
          Version = str2
        };
      return timelineRecord1;
    }
  }
}
