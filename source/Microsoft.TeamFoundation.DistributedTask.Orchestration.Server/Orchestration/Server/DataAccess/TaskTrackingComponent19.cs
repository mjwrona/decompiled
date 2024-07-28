// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.DataAccess.TaskTrackingComponent19
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
  internal class TaskTrackingComponent19 : TaskTrackingComponent18
  {
    protected static SqlMetaData[] typ_TimelineRecordVariableTable = new SqlMetaData[4]
    {
      new SqlMetaData("RecordId", SqlDbType.UniqueIdentifier),
      new SqlMetaData("Name", SqlDbType.NVarChar, 512L),
      new SqlMetaData("IsSecret", SqlDbType.Bit),
      new SqlMetaData("Value", SqlDbType.NVarChar, SqlMetaData.Max)
    };
    protected static SqlMetaData[] typ_TimelineRecordTableV5 = new SqlMetaData[19]
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
      new SqlMetaData("WorkerName", SqlDbType.NVarChar, 512L),
      new SqlMetaData("DetailTimelineId", SqlDbType.UniqueIdentifier),
      new SqlMetaData("LogId", SqlDbType.Int),
      new SqlMetaData("Order", SqlDbType.Int),
      new SqlMetaData("ErrorCount", SqlDbType.Int),
      new SqlMetaData("WarningCount", SqlDbType.Int),
      new SqlMetaData("Issues", SqlDbType.VarBinary, SqlMetaData.Max)
    };

    protected override ObjectBinder<TimelineRecord> GetTimelineRecordBinder() => (ObjectBinder<TimelineRecord>) new TimelineRecordBinder5();

    protected SqlParameter BindTimelineRecordVariableTable(
      string parameterName,
      IEnumerable<TimelineRecordVariableData> rows)
    {
      return this.BindTable(parameterName, "Task.typ_TimelineRecordVariableTable", (rows ?? Enumerable.Empty<TimelineRecordVariableData>()).Select<TimelineRecordVariableData, SqlDataRecord>(new System.Func<TimelineRecordVariableData, SqlDataRecord>(this.ConvertToSqlDataRecord)));
    }

    protected SqlDataRecord ConvertToSqlDataRecord(TimelineRecordVariableData row)
    {
      SqlDataRecord record = new SqlDataRecord(TaskTrackingComponent19.typ_TimelineRecordVariableTable);
      record.SetGuid(0, row.RecordId);
      record.SetString(1, row.Name, BindStringBehavior.Unchanged);
      record.SetBoolean(2, row.IsSecret);
      record.SetString(3, row.Value, BindStringBehavior.Unchanged);
      return record;
    }

    protected override SqlParameter BindTimelineRecordTable(
      string parameterName,
      IEnumerable<TimelineRecord> rows)
    {
      return this.BindTable(parameterName, "Task.typ_TimelineRecordTableV5", (rows ?? Enumerable.Empty<TimelineRecord>()).Select<TimelineRecord, SqlDataRecord>(new System.Func<TimelineRecord, SqlDataRecord>(((TaskTrackingComponent) this).ConvertToSqlDataRecord)));
    }

    protected override SqlDataRecord ConvertToSqlDataRecord(TimelineRecord row)
    {
      SqlDataRecord record = new SqlDataRecord(TaskTrackingComponent19.typ_TimelineRecordTableV5);
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
      record.SetString(12, row.WorkerName, BindStringBehavior.EmptyStringToNull);
      record.SetNullableGuid(13, row.Details == null ? new Guid?() : new Guid?(row.Details.Id));
      record.SetNullableInt32(14, row.Log == null ? new int?() : new int?(row.Log.Id));
      record.SetNullableInt32(15, row.Order);
      record.SetNullableInt32(16, row.ErrorCount);
      record.SetNullableInt32(17, row.WarningCount);
      byte[] numArray = (byte[]) null;
      if (row.Issues.Count > 0)
        numArray = JsonUtility.Serialize((object) row.Issues);
      record.SetNullableBinary(18, numArray);
      return record;
    }

    public override async Task<Timeline> AddJobsAsync(
      Guid scopeIdentifier,
      Guid planId,
      Guid requestedBy,
      IList<TaskOrchestrationJob> jobs,
      IList<TaskReferenceData> tasks,
      IList<TimelineRecord> records,
      IEnumerable<TimelineAttempt> attempts = null)
    {
      TaskTrackingComponent19 component = this;
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) component, nameof (AddJobsAsync)))
      {
        component.PrepareStoredProcedure("Task.prc_AddJobs");
        component.BindDataspaceId(scopeIdentifier);
        component.BindGuid("@planId", planId);
        component.BindGuid("@requestedBy", requestedBy);
        component.BindJobTable("@jobs", (IEnumerable<TaskOrchestrationJob>) jobs);
        component.BindTaskReferenceTable("@tasks", (IEnumerable<TaskReferenceData>) tasks);
        component.BindTimelineRecordTable("@records", (IEnumerable<TimelineRecord>) records);
        int num = await component.ExecuteNonQueryAsync();
      }
      Timeline timeline;
      return timeline;
    }

    public override TaskOrchestrationPlan CreatePlan(
      TaskOrchestrationPlan plan,
      Timeline timeline,
      IEnumerable<TaskOrchestrationJob> jobs,
      IEnumerable<TaskReferenceData> tasks,
      IEnumerable<TimelineAttempt> attempts,
      bool createInitializationLog,
      bool createExpandedYaml)
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (CreatePlan)))
      {
        this.PrepareStoredProcedure("Task.prc_CreatePlan");
        this.BindDataspaceId(plan.ScopeIdentifier);
        this.BindString("@planType", plan.PlanType, 260, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
        this.BindInt("@planVersion", plan.Version);
        this.BindGuid("@planId", plan.PlanId);
        this.BindString("@planGroup", plan.PlanGroup, 128, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
        this.BindString("@artifactUri", plan.ArtifactUri.ToString(), 128, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
        this.BindLong("@containerId", plan.ContainerId);
        this.BindBinary("@environment", JsonUtility.Serialize((object) plan.ProcessEnvironment), SqlDbType.VarBinary);
        this.BindBinary("@implementation", JsonUtility.Serialize((object) plan.Process), SqlDbType.VarBinary);
        this.BindJobTable("@jobs", jobs);
        this.BindTaskReferenceTable("@tasks", tasks);
        this.BindGuid("@requestedBy", plan.RequestedById);
        this.BindGuid("@requestedFor", plan.RequestedForId);
        this.BindGuid("@timelineId", timeline.Id);
        this.BindTimelineRecordTable("@timelineRecords", (IEnumerable<TimelineRecord>) timeline.Records);
        this.BindNullableInt("@definitionId", plan.Definition?.Id);
        this.BindString("@definitionName", plan.Definition?.Name ?? string.Empty, 260, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
        this.BindBinary("@definitionReference", JsonUtility.Serialize((object) plan.Definition, false), int.MaxValue, SqlDbType.VarBinary);
        this.BindNullableInt("@ownerId", plan.Owner?.Id);
        this.BindString("@ownerName", plan.Owner?.Name ?? string.Empty, 260, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
        this.BindBinary("@ownerReference", JsonUtility.Serialize((object) plan.Owner, false), int.MaxValue, SqlDbType.VarBinary);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<TaskOrchestrationPlan>(this.GetPlanBinder());
          return resultCollection.GetCurrent<TaskOrchestrationPlan>().FirstOrDefault<TaskOrchestrationPlan>();
        }
      }
    }

    public override async Task<UpdatePlanResult> UpdatePlanAsync(
      Guid scopeIdentifier,
      Guid planId,
      DateTime? startTime,
      DateTime? finishTime,
      TaskOrchestrationPlanState? state,
      TaskResult? result,
      string resultCode,
      IOrchestrationEnvironment environment)
    {
      TaskTrackingComponent19 component = this;
      UpdatePlanResult updatePlanResult;
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) component, nameof (UpdatePlanAsync)))
      {
        component.PrepareStoredProcedure("Task.prc_UpdatePlan");
        component.BindDataspaceId(scopeIdentifier);
        component.BindGuid("@planId", planId);
        if (startTime.HasValue)
          component.BindDateTime2("@startTime", startTime.Value);
        if (finishTime.HasValue)
          component.BindDateTime2("@finishTime", finishTime.Value);
        if (state.HasValue)
          component.BindByte("@state", (byte) state.Value);
        if (result.HasValue)
          component.BindByte("@result", (byte) result.Value);
        if (resultCode != null)
          component.BindString("@resultCode", resultCode, 512, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
        UpdatePlanResult dbResult = new UpdatePlanResult();
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) await component.ExecuteReaderAsync(), component.ProcedureName, component.RequestContext))
        {
          resultCollection.AddBinder<TaskOrchestrationPlan>(component.GetPlanBinder());
          resultCollection.AddBinder<Timeline>((ObjectBinder<Timeline>) new TimelineBinder());
          resultCollection.AddBinder<TimelineRecord>(component.GetTimelineRecordBinder());
          dbResult.Plan = resultCollection.GetCurrent<TaskOrchestrationPlan>().FirstOrDefault<TaskOrchestrationPlan>();
          resultCollection.NextResult();
          dbResult.Timeline = resultCollection.GetCurrent<Timeline>().FirstOrDefault<Timeline>();
          if (dbResult.Timeline != null)
          {
            resultCollection.NextResult();
            dbResult.Timeline.Records.AddRange((IEnumerable<TimelineRecord>) resultCollection.GetCurrent<TimelineRecord>().Items);
          }
          updatePlanResult = dbResult;
        }
      }
      return updatePlanResult;
    }

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
        this.BindDataspaceId(scopeIdentifier);
        this.BindGuid("@planId", planId);
        this.BindGuid("@requestedBy", requestedBy);
        this.BindGuid("@timelineId", timelineId);
        this.BindTimelineRecordTable("@records", records);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<Timeline>((ObjectBinder<Timeline>) new TimelineBinder());
          resultCollection.AddBinder<TimelineRecord>(this.GetTimelineRecordBinder());
          Timeline timeline = resultCollection.GetCurrent<Timeline>().FirstOrDefault<Timeline>();
          if (timeline != null && resultCollection.TryNextResult())
            timeline.Records.AddRange((IEnumerable<TimelineRecord>) resultCollection.GetCurrent<TimelineRecord>().Items);
          return timeline;
        }
      }
    }

    public override async Task<Timeline> UpdateTimelineAsync(
      Guid scopeIdentifier,
      Guid planId,
      Guid timelineId,
      Guid requestedBy,
      IList<TimelineRecord> records,
      int blockingPeriod = 0)
    {
      TaskTrackingComponent19 component = this;
      Timeline timeline1;
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) component, nameof (UpdateTimelineAsync)))
      {
        component.PrepareStoredProcedure("Task.prc_UpdateTimeline");
        component.BindDataspaceId(scopeIdentifier);
        component.BindGuid("@planId", planId);
        component.BindGuid("@timelineId", timelineId);
        component.BindGuid("@requestedBy", requestedBy);
        component.BindTimelineRecordTable("@records", (IEnumerable<TimelineRecord>) records);
        List<TimelineRecordVariableData> rows = new List<TimelineRecordVariableData>();
        foreach (TimelineRecord timelineRecord in (IEnumerable<TimelineRecord>) ((object) records ?? (object) Array.Empty<TimelineRecord>()))
        {
          if (timelineRecord.Variables.Count > 0)
          {
            foreach (KeyValuePair<string, VariableValue> variable in (IEnumerable<KeyValuePair<string, VariableValue>>) timelineRecord.Variables)
            {
              TimelineRecordVariableData recordVariableData = new TimelineRecordVariableData()
              {
                RecordId = timelineRecord.Id,
                Name = variable.Key,
                IsSecret = variable.Value.IsSecret,
                Value = variable.Value.Value
              };
              rows.Add(recordVariableData);
            }
          }
        }
        component.BindTimelineRecordVariableTable("@recordVariables", (IEnumerable<TimelineRecordVariableData>) rows);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) await component.ExecuteReaderAsync(), component.ProcedureName, component.RequestContext))
        {
          resultCollection.AddBinder<Timeline>((ObjectBinder<Timeline>) new TimelineBinder());
          resultCollection.AddBinder<TimelineRecord>(component.GetTimelineRecordBinder());
          resultCollection.AddBinder<TimelineRecordVariableData>(component.GetTimelineRecordVariableBinder());
          Timeline timeline2 = resultCollection.GetCurrent<Timeline>().FirstOrDefault<Timeline>();
          if (timeline2 != null && resultCollection.TryNextResult())
          {
            Dictionary<Guid, TimelineRecord> dictionary = resultCollection.GetCurrent<TimelineRecord>().Items.ToDictionary<TimelineRecord, Guid, TimelineRecord>((System.Func<TimelineRecord, Guid>) (k => k.Id), (System.Func<TimelineRecord, TimelineRecord>) (v => v));
            if (resultCollection.TryNextResult())
            {
              foreach (TimelineRecordVariableData recordVariableData in resultCollection.GetCurrent<TimelineRecordVariableData>().Items ?? new List<TimelineRecordVariableData>())
                dictionary[recordVariableData.RecordId].Variables[recordVariableData.Name] = new VariableValue(recordVariableData.Value, recordVariableData.IsSecret);
            }
            timeline2.Records.AddRange((IEnumerable<TimelineRecord>) dictionary.Values);
          }
          timeline1 = timeline2;
        }
      }
      return timeline1;
    }

    public override async Task<Timeline> GetTimelineAsync(
      Guid scopeIdentifier,
      Guid planId,
      Guid timelineId,
      int changeId = 0,
      bool includeRecords = true,
      bool includePreviousAttempts = true)
    {
      TaskTrackingComponent19 component = this;
      Timeline timelineAsync;
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) component, nameof (GetTimelineAsync)))
      {
        component.PrepareStoredProcedure("Task.prc_GetTimeline");
        component.BindDataspaceId(scopeIdentifier);
        component.BindGuid("@planId", planId);
        component.BindNullableGuid("@timelineId", new Guid?(timelineId), true);
        component.BindInt("@changeId", changeId);
        component.BindBoolean("@includeRecords", includeRecords);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) await component.ExecuteReaderAsync(), component.ProcedureName, component.RequestContext))
        {
          resultCollection.AddBinder<Timeline>((ObjectBinder<Timeline>) new TimelineBinder());
          resultCollection.AddBinder<TimelineRecord>(component.GetTimelineRecordBinder());
          resultCollection.AddBinder<TimelineRecordVariableData>(component.GetTimelineRecordVariableBinder());
          Timeline timeline = resultCollection.GetCurrent<Timeline>().FirstOrDefault<Timeline>();
          if (timeline != null && resultCollection.TryNextResult())
          {
            Dictionary<Guid, TimelineRecord> dictionary = resultCollection.GetCurrent<TimelineRecord>().Items.ToDictionary<TimelineRecord, Guid, TimelineRecord>((System.Func<TimelineRecord, Guid>) (k => k.Id), (System.Func<TimelineRecord, TimelineRecord>) (v => v));
            if (resultCollection.TryNextResult())
            {
              foreach (TimelineRecordVariableData recordVariableData in resultCollection.GetCurrent<TimelineRecordVariableData>().Items ?? new List<TimelineRecordVariableData>())
                dictionary[recordVariableData.RecordId].Variables[recordVariableData.Name] = new VariableValue(recordVariableData.Value, recordVariableData.IsSecret);
            }
            timeline.Records.AddRange((IEnumerable<TimelineRecord>) dictionary.Values);
          }
          timelineAsync = timeline;
        }
      }
      return timelineAsync;
    }
  }
}
