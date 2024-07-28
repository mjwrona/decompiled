// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.DataAccess.TaskTrackingComponent32
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

namespace Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.DataAccess
{
  internal class TaskTrackingComponent32 : TaskTrackingComponent31
  {
    private static SqlMetaData[] typ_TimelineRecordTableV7 = new SqlMetaData[23]
    {
      new SqlMetaData("RecordId", SqlDbType.UniqueIdentifier),
      new SqlMetaData("ParentId", SqlDbType.UniqueIdentifier),
      new SqlMetaData("Type", SqlDbType.NVarChar, 512L),
      new SqlMetaData("Name", SqlDbType.NVarChar, 512L),
      new SqlMetaData("RefName", SqlDbType.NVarChar, 512L),
      new SqlMetaData("StartTime", SqlDbType.DateTime2),
      new SqlMetaData("FinishTime", SqlDbType.DateTime2),
      new SqlMetaData("CurrentOperation", SqlDbType.NVarChar, 512L),
      new SqlMetaData("PercentComplete", SqlDbType.Int),
      new SqlMetaData("State", SqlDbType.TinyInt),
      new SqlMetaData("Result", SqlDbType.TinyInt),
      new SqlMetaData("ResultCode", SqlDbType.NVarChar, 512L),
      new SqlMetaData("QueueId", SqlDbType.Int),
      new SqlMetaData("AgentSpecification", SqlDbType.VarBinary, SqlMetaData.Max),
      new SqlMetaData("WorkerName", SqlDbType.NVarChar, 512L),
      new SqlMetaData("DetailTimelineId", SqlDbType.UniqueIdentifier),
      new SqlMetaData("LogId", SqlDbType.Int),
      new SqlMetaData("Attempt", SqlDbType.Int),
      new SqlMetaData("Order", SqlDbType.Int),
      new SqlMetaData("ErrorCount", SqlDbType.Int),
      new SqlMetaData("WarningCount", SqlDbType.Int),
      new SqlMetaData("Issues", SqlDbType.VarBinary, SqlMetaData.Max),
      new SqlMetaData("Identifier", SqlDbType.NVarChar, 256L)
    };

    protected override ObjectBinder<TimelineRecord> GetTimelineRecordBinder() => (ObjectBinder<TimelineRecord>) new TimelineRecordBinder8();

    protected override SqlParameter BindTimelineRecordTable(
      string parameterName,
      IEnumerable<TimelineRecord> rows)
    {
      return this.BindTable(parameterName, "Task.typ_TimelineRecordTableV7", (rows ?? Enumerable.Empty<TimelineRecord>()).Select<TimelineRecord, SqlDataRecord>(new System.Func<TimelineRecord, SqlDataRecord>(((TaskTrackingComponent) this).ConvertToSqlDataRecord)));
    }

    protected override SqlDataRecord ConvertToSqlDataRecord(TimelineRecord row)
    {
      SqlDataRecord record = new SqlDataRecord(TaskTrackingComponent32.typ_TimelineRecordTableV7);
      record.SetGuid(0, row.Id);
      record.SetNullableGuid(1, row.ParentId);
      record.SetString(2, row.RecordType, BindStringBehavior.EmptyStringToNull);
      record.SetString(3, row.Name, BindStringBehavior.Unchanged);
      record.SetString(4, row.RefName, BindStringBehavior.EmptyStringToNull);
      record.SetNullableDateTime(5, row.StartTime);
      record.SetNullableDateTime(6, row.FinishTime);
      record.SetString(7, row.CurrentOperation, BindStringBehavior.EmptyStringToNull);
      record.SetNullableInt32(8, row.PercentComplete);
      TimelineRecordState? state = row.State;
      record.SetNullableByte(9, state.HasValue ? new byte?((byte) state.GetValueOrDefault()) : new byte?());
      TaskResult? result = row.Result;
      record.SetNullableByte(10, result.HasValue ? new byte?((byte) result.GetValueOrDefault()) : new byte?());
      record.SetString(11, row.ResultCode, BindStringBehavior.EmptyStringToNull);
      int? nullable1 = row.QueueId;
      int? nullable2;
      if (nullable1.HasValue)
      {
        nullable2 = row.QueueId;
      }
      else
      {
        nullable1 = new int?();
        nullable2 = nullable1;
      }
      record.SetNullableInt32(12, nullable2);
      record.SetNullableBinary(13, JsonUtility.Serialize((object) row.AgentSpecification, false));
      record.SetString(14, row.WorkerName, BindStringBehavior.EmptyStringToNull);
      record.SetNullableGuid(15, row.Details == null ? new Guid?() : new Guid?(row.Details.Id));
      int? nullable3;
      if (row.Log != null)
      {
        nullable3 = new int?(row.Log.Id);
      }
      else
      {
        nullable1 = new int?();
        nullable3 = nullable1;
      }
      record.SetNullableInt32(16, nullable3);
      int? nullable4;
      if (row.Attempt != 0)
      {
        nullable4 = new int?(row.Attempt);
      }
      else
      {
        nullable1 = new int?();
        nullable4 = nullable1;
      }
      record.SetNullableInt32(17, nullable4);
      record.SetNullableInt32(18, row.Order);
      record.SetNullableInt32(19, row.ErrorCount);
      record.SetNullableInt32(20, row.WarningCount);
      byte[] numArray = (byte[]) null;
      if (row.Issues.Count > 0)
        numArray = JsonUtility.Serialize((object) row.Issues);
      record.SetNullableBinary(21, numArray);
      record.SetString(22, row.Identifier, BindStringBehavior.EmptyStringToNull);
      return record;
    }
  }
}
