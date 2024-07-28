// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.DataAccess.TaskTrackingComponent4
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.DataAccess
{
  internal class TaskTrackingComponent4 : TaskTrackingComponent3
  {
    protected static SqlMetaData[] typ_TimelineRecordTableV3 = new SqlMetaData[18]
    {
      new SqlMetaData("RecordId", SqlDbType.UniqueIdentifier),
      new SqlMetaData("ParentId", SqlDbType.UniqueIdentifier),
      new SqlMetaData("Type", SqlDbType.NVarChar, 512L),
      new SqlMetaData("Name", SqlDbType.NVarChar, 512L),
      new SqlMetaData("StartTime", SqlDbType.DateTime2),
      new SqlMetaData("FinishTime", SqlDbType.DateTime2),
      new SqlMetaData("CurrentOperation", SqlDbType.NVarChar, 512L),
      new SqlMetaData("PercentComplete", SqlDbType.Int),
      new SqlMetaData("State", SqlDbType.TinyInt),
      new SqlMetaData("Result", SqlDbType.TinyInt),
      new SqlMetaData("ResultCode", SqlDbType.NVarChar, 512L),
      new SqlMetaData("WorkerName", SqlDbType.NVarChar, 512L),
      new SqlMetaData("DetailTimelineId", SqlDbType.UniqueIdentifier),
      new SqlMetaData("LogId", SqlDbType.Int),
      new SqlMetaData("Order", SqlDbType.Int),
      new SqlMetaData("ErrorCount", SqlDbType.Int),
      new SqlMetaData("WarningCount", SqlDbType.Int),
      new SqlMetaData("Issues", SqlDbType.VarBinary, SqlMetaData.Max)
    };

    public override Timeline CreateTimeline(
      Guid scopeIdentifier,
      Guid planId,
      Guid requestedBy,
      Guid timelineId,
      IEnumerable<TimelineRecord> records)
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (CreateTimeline)))
      {
        this.PrepareStoredProcedure("Task.prc_CreateTimeline");
        this.BindGuid("@planId", planId);
        this.BindGuid("@requestedBy", requestedBy);
        this.BindGuid("@timelineId", timelineId);
        this.BindTimelineRecordTable("@records", records);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<Timeline>((ObjectBinder<Timeline>) new TimelineBinder());
          resultCollection.AddBinder<TimelineRecord>((ObjectBinder<TimelineRecord>) new TimelineRecordBinder3());
          Timeline timeline = resultCollection.GetCurrent<Timeline>().FirstOrDefault<Timeline>();
          if (timeline != null && resultCollection.TryNextResult())
            timeline.Records.AddRange((IEnumerable<TimelineRecord>) resultCollection.GetCurrent<TimelineRecord>().Items);
          return timeline;
        }
      }
    }

    public override async Task<Timeline> GetTimelineAsync(
      Guid scopeIdentifier,
      Guid planId,
      Guid timelineId,
      int changeId = 0,
      bool includeRecords = true,
      bool includePreviousAttempts = true)
    {
      TaskTrackingComponent4 component = this;
      Timeline timelineAsync;
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) component, nameof (GetTimelineAsync)))
      {
        component.PrepareStoredProcedure("Task.prc_GetTimeline");
        component.BindGuid("@planId", planId);
        component.BindNullableGuid("@timelineId", new Guid?(timelineId), true);
        component.BindInt("@changeId", changeId);
        component.BindBoolean("@includeRecords", includeRecords);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) await component.ExecuteReaderAsync(), component.ProcedureName, component.RequestContext))
        {
          resultCollection.AddBinder<Timeline>((ObjectBinder<Timeline>) new TimelineBinder());
          resultCollection.AddBinder<TimelineRecord>((ObjectBinder<TimelineRecord>) new TimelineRecordBinder3());
          Timeline timeline = resultCollection.GetCurrent<Timeline>().FirstOrDefault<Timeline>();
          if (timeline != null && resultCollection.TryNextResult())
            timeline.Records.AddRange((IEnumerable<TimelineRecord>) resultCollection.GetCurrent<TimelineRecord>().Items);
          timelineAsync = timeline;
        }
      }
      return timelineAsync;
    }

    public override async Task<Timeline> UpdateTimelineAsync(
      Guid scopeIdentifier,
      Guid planId,
      Guid timelineId,
      Guid requestedBy,
      IList<TimelineRecord> records,
      int blockingPeriod = 0)
    {
      TaskTrackingComponent4 component = this;
      Timeline timeline1;
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) component, nameof (UpdateTimelineAsync)))
      {
        component.PrepareStoredProcedure("Task.prc_UpdateTimeline");
        component.BindGuid("@planId", planId);
        component.BindGuid("@timelineId", timelineId);
        component.BindGuid("@requestedBy", requestedBy);
        component.BindTimelineRecordTable("@records", (IEnumerable<TimelineRecord>) records);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) await component.ExecuteReaderAsync(), component.ProcedureName, component.RequestContext))
        {
          resultCollection.AddBinder<Timeline>((ObjectBinder<Timeline>) new TimelineBinder());
          resultCollection.AddBinder<TimelineRecord>((ObjectBinder<TimelineRecord>) new TimelineRecordBinder3());
          Timeline timeline2 = resultCollection.GetCurrent<Timeline>().FirstOrDefault<Timeline>();
          if (timeline2 != null && resultCollection.TryNextResult())
            timeline2.Records.AddRange((IEnumerable<TimelineRecord>) resultCollection.GetCurrent<TimelineRecord>().Items);
          timeline1 = timeline2;
        }
      }
      return timeline1;
    }

    protected override SqlParameter BindTimelineRecordTable(
      string parameterName,
      IEnumerable<TimelineRecord> rows)
    {
      return this.BindTable(parameterName, "Task.typ_TimelineRecordTableV3", (rows ?? Enumerable.Empty<TimelineRecord>()).Select<TimelineRecord, SqlDataRecord>(new System.Func<TimelineRecord, SqlDataRecord>(((TaskTrackingComponent) this).ConvertToSqlDataRecord)));
    }

    protected override SqlDataRecord ConvertToSqlDataRecord(TimelineRecord row)
    {
      SqlDataRecord record = new SqlDataRecord(TaskTrackingComponent4.typ_TimelineRecordTableV3);
      record.SetGuid(0, row.Id);
      record.SetNullableGuid(1, row.ParentId);
      record.SetString(2, row.RecordType, BindStringBehavior.EmptyStringToNull);
      record.SetString(3, row.Name, BindStringBehavior.Unchanged);
      record.SetNullableDateTime(4, row.StartTime);
      record.SetNullableDateTime(5, row.FinishTime);
      record.SetString(6, row.CurrentOperation, BindStringBehavior.EmptyStringToNull);
      record.SetNullableInt32(7, row.PercentComplete);
      TimelineRecordState? state = row.State;
      record.SetNullableByte(8, state.HasValue ? new byte?((byte) state.GetValueOrDefault()) : new byte?());
      TaskResult? result = row.Result;
      record.SetNullableByte(9, result.HasValue ? new byte?((byte) result.GetValueOrDefault()) : new byte?());
      record.SetString(10, row.ResultCode, BindStringBehavior.EmptyStringToNull);
      record.SetString(11, row.WorkerName, BindStringBehavior.EmptyStringToNull);
      record.SetNullableGuid(12, row.Details == null ? new Guid?() : new Guid?(row.Details.Id));
      record.SetNullableInt32(13, row.Log == null ? new int?() : new int?(row.Log.Id));
      record.SetNullableInt32(14, row.Order);
      record.SetNullableInt32(15, row.ErrorCount);
      record.SetNullableInt32(16, row.WarningCount);
      byte[] numArray = (byte[]) null;
      if (row.Issues.Count > 0)
        numArray = JsonUtility.Serialize((object) row.Issues);
      record.SetNullableBinary(17, numArray);
      return record;
    }
  }
}
