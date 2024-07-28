// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Analytics.TaskTimelineRecordBinder
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.DataAccess;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Analytics
{
  internal class TaskTimelineRecordBinder : TaskAnalyticsDataBinderBase<TaskTimelineRecord>
  {
    private SqlColumnBinder dataspaceId = new SqlColumnBinder("DataspaceId");
    private SqlColumnBinder timelineId = new SqlColumnBinder("TimelineId");
    private SqlColumnBinder planId = new SqlColumnBinder("PlanId");
    private SqlColumnBinder changeId = new SqlColumnBinder("ChangeId");
    private SqlColumnBinder recordId = new SqlColumnBinder("RecordId");
    private SqlColumnBinder parentId = new SqlColumnBinder("ParentId");
    private SqlColumnBinder type = new SqlColumnBinder("Type");
    private SqlColumnBinder name = new SqlColumnBinder("Name");
    private SqlColumnBinder refName = new SqlColumnBinder("RefName");
    private SqlColumnBinder startTime = new SqlColumnBinder("StartTime");
    private SqlColumnBinder finishTime = new SqlColumnBinder("FinishTime");
    private SqlColumnBinder state = new SqlColumnBinder("State");
    private SqlColumnBinder result = new SqlColumnBinder("Result");
    private SqlColumnBinder resultCode = new SqlColumnBinder("ResultCode");
    private SqlColumnBinder workerName = new SqlColumnBinder("WorkerName");
    private SqlColumnBinder order = new SqlColumnBinder("Order");
    private SqlColumnBinder logPath = new SqlColumnBinder("LogPath");
    private SqlColumnBinder lineCount = new SqlColumnBinder("LineCount");
    private SqlColumnBinder planLastUpdated = new SqlColumnBinder("PlanLastUpdated");
    private SqlColumnBinder taskDefinitionReferenceId = new SqlColumnBinder("TaskDefinitionReferenceId");
    private SqlColumnBinder identifier = new SqlColumnBinder("Identifier");
    private SqlColumnBinder attempt = new SqlColumnBinder("Attempt");
    private SqlColumnBinder issues = new SqlColumnBinder("Issues");
    private SqlColumnBinder planGuidId = new SqlColumnBinder("PlanGuidId");
    private Guid m_projectGuid;

    public TaskTimelineRecordBinder(TaskSqlComponentBase sqlComponent, Guid projectGuid)
      : base(sqlComponent)
    {
      this.m_projectGuid = projectGuid;
    }

    protected override TaskTimelineRecord Bind() => new TaskTimelineRecord()
    {
      ProjectGuid = this.m_projectGuid,
      TimelineRecordGuid = this.recordId.GetGuid((IDataReader) this.Reader),
      ParentRecordGuid = new Guid?(this.parentId.ColumnExists((IDataReader) this.Reader) ? this.parentId.GetGuid((IDataReader) this.Reader, true) : new Guid()),
      TimelineId = this.timelineId.GetInt32((IDataReader) this.Reader),
      PlanId = this.planId.GetInt32((IDataReader) this.Reader),
      Type = this.type.ColumnExists((IDataReader) this.Reader) ? this.type.GetString((IDataReader) this.Reader, true) : (string) null,
      Name = this.name.ColumnExists((IDataReader) this.Reader) ? this.name.GetString((IDataReader) this.Reader, true) : (string) null,
      RefName = this.refName.ColumnExists((IDataReader) this.Reader) ? this.refName.GetString((IDataReader) this.Reader, true) : (string) null,
      ChangeId = this.changeId.ColumnExists((IDataReader) this.Reader) ? this.changeId.GetInt32((IDataReader) this.Reader, 0) : 0,
      Order = new int?(this.order.ColumnExists((IDataReader) this.Reader) ? this.order.GetInt32((IDataReader) this.Reader, 0) : 0),
      State = this.state.ColumnExists((IDataReader) this.Reader) ? (int) this.state.GetByte((IDataReader) this.Reader) : 0,
      Result = new int?(this.result.ColumnExists((IDataReader) this.Reader) ? (int) this.result.GetByte((IDataReader) this.Reader, (byte) 3) : 0),
      ResultCode = this.resultCode.ColumnExists((IDataReader) this.Reader) ? this.resultCode.GetString((IDataReader) this.Reader, true) : (string) null,
      WorkerName = this.workerName.ColumnExists((IDataReader) this.Reader) ? this.workerName.GetString((IDataReader) this.Reader, true) : (string) null,
      LogPath = this.logPath.ColumnExists((IDataReader) this.Reader) ? this.logPath.GetString((IDataReader) this.Reader, true) : (string) null,
      LogLineCount = new int?(this.planId.ColumnExists((IDataReader) this.Reader) ? this.planId.GetInt32((IDataReader) this.Reader, 0) : 0),
      StartTime = this.startTime.ColumnExists((IDataReader) this.Reader) ? this.startTime.GetNullableDateTime((IDataReader) this.Reader) : new DateTime?(),
      FinishTime = this.finishTime.ColumnExists((IDataReader) this.Reader) ? this.finishTime.GetNullableDateTime((IDataReader) this.Reader) : new DateTime?(),
      PlanLastUpdated = this.planLastUpdated.GetDateTime((IDataReader) this.Reader),
      TaskDefinitionReferenceId = this.taskDefinitionReferenceId.ColumnExists((IDataReader) this.Reader) ? this.taskDefinitionReferenceId.GetNullableInt32((IDataReader) this.Reader) : new int?(),
      Identifier = this.identifier.ColumnExists((IDataReader) this.Reader) ? this.identifier.GetString((IDataReader) this.Reader, true) : (string) null,
      Attempt = this.attempt.ColumnExists((IDataReader) this.Reader) ? this.attempt.GetNullableInt32((IDataReader) this.Reader) : new int?(),
      Issues = this.issues.ColumnExists((IDataReader) this.Reader) ? JsonUtility.Deserialize<List<Issue>>(this.issues.GetBytes((IDataReader) this.Reader, true)) : (List<Issue>) null,
      PlanGuidId = this.planGuidId.ColumnExists((IDataReader) this.Reader) ? this.planGuidId.GetGuid((IDataReader) this.Reader) : new Guid()
    };
  }
}
