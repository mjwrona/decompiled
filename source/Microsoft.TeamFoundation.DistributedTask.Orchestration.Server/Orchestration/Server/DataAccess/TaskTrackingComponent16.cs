// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.DataAccess.TaskTrackingComponent16
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
  internal class TaskTrackingComponent16 : TaskTrackingComponent15
  {
    protected static SqlMetaData[] typ_TaskReferenceTable = new SqlMetaData[4]
    {
      new SqlMetaData("InstanceId", SqlDbType.UniqueIdentifier),
      new SqlMetaData("TaskId", SqlDbType.UniqueIdentifier),
      new SqlMetaData("TaskName", SqlDbType.NVarChar, 128L),
      new SqlMetaData("TaskVersion", SqlDbType.NVarChar, 64L)
    };

    protected virtual ObjectBinder<TaskOrchestrationPlan> GetPlanBinder() => (ObjectBinder<TaskOrchestrationPlan>) new TaskOrchestrationPlanBinder5((TaskTrackingComponent) this);

    protected virtual ObjectBinder<TaskOrchestrationPlanReference> GetPlanReferenceBinder() => (ObjectBinder<TaskOrchestrationPlanReference>) new TaskOrchestrationPlanReferenceBinder3((TaskTrackingComponent) this);

    protected virtual ObjectBinder<TimelineRecord> GetTimelineRecordBinder() => (ObjectBinder<TimelineRecord>) new TimelineRecordBinder4();

    public override async Task<Timeline> AddJobsAsync(
      Guid scopeIdentifier,
      Guid planId,
      Guid requestedBy,
      IList<TaskOrchestrationJob> jobs,
      IList<TaskReferenceData> tasks,
      IList<TimelineRecord> records,
      IEnumerable<TimelineAttempt> attempts = null)
    {
      TaskTrackingComponent16 component = this;
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

    public override async Task<Timeline> GetTimelineAsync(
      Guid scopeIdentifier,
      Guid planId,
      Guid timelineId,
      int changeId = 0,
      bool includeRecords = true,
      bool includePreviousAttempts = true)
    {
      TaskTrackingComponent16 component = this;
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
          Timeline timeline = resultCollection.GetCurrent<Timeline>().FirstOrDefault<Timeline>();
          if (timeline != null && resultCollection.TryNextResult())
            timeline.Records.AddRange((IEnumerable<TimelineRecord>) resultCollection.GetCurrent<TimelineRecord>().Items);
          timelineAsync = timeline;
        }
      }
      return timelineAsync;
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
      TaskTrackingComponent16 component = this;
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

    public override async Task<Timeline> UpdateTimelineAsync(
      Guid scopeIdentifier,
      Guid planId,
      Guid timelineId,
      Guid requestedBy,
      IList<TimelineRecord> records,
      int blockingPeriod = 0)
    {
      TaskTrackingComponent16 component = this;
      Timeline timeline1;
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) component, nameof (UpdateTimelineAsync)))
      {
        component.PrepareStoredProcedure("Task.prc_UpdateTimeline");
        component.BindDataspaceId(scopeIdentifier);
        component.BindGuid("@planId", planId);
        component.BindGuid("@timelineId", timelineId);
        component.BindGuid("@requestedBy", requestedBy);
        component.BindTimelineRecordTable("@records", (IEnumerable<TimelineRecord>) records);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) await component.ExecuteReaderAsync(), component.ProcedureName, component.RequestContext))
        {
          resultCollection.AddBinder<Timeline>((ObjectBinder<Timeline>) new TimelineBinder());
          resultCollection.AddBinder<TimelineRecord>(component.GetTimelineRecordBinder());
          Timeline timeline2 = resultCollection.GetCurrent<Timeline>().FirstOrDefault<Timeline>();
          if (timeline2 != null && resultCollection.TryNextResult())
            timeline2.Records.AddRange((IEnumerable<TimelineRecord>) resultCollection.GetCurrent<TimelineRecord>().Items);
          timeline1 = timeline2;
        }
      }
      return timeline1;
    }

    protected virtual SqlParameter BindTaskReferenceTable(
      string parameterName,
      IEnumerable<TaskReferenceData> rows)
    {
      return this.BindTable(parameterName, "Task.typ_TaskReferenceTable", (rows ?? Enumerable.Empty<TaskReferenceData>()).Select<TaskReferenceData, SqlDataRecord>(new System.Func<TaskReferenceData, SqlDataRecord>(this.ConvertToSqlDataRecord)));
    }

    protected virtual SqlDataRecord ConvertToSqlDataRecord(TaskReferenceData row)
    {
      SqlDataRecord record = new SqlDataRecord(TaskTrackingComponent16.typ_TaskReferenceTable);
      record.SetGuid(0, row.InstanceId);
      record.SetGuid(1, row.Task.Id);
      record.SetString(2, row.Task.Name, BindStringBehavior.Unchanged);
      record.SetString(3, row.Task.Version, BindStringBehavior.Unchanged);
      return record;
    }

    public override async Task<TaskOrchestrationPlan> GetPlanAsync(
      Guid scopeIdentifier,
      Guid planId)
    {
      TaskTrackingComponent16 component = this;
      TaskOrchestrationPlan planAsync;
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) component, nameof (GetPlanAsync)))
      {
        component.PrepareStoredProcedure("Task.prc_GetPlan");
        if (scopeIdentifier != Guid.Empty)
          component.BindDataspaceId(scopeIdentifier);
        else
          component.BindInt("@dataspaceId", 0);
        component.BindGuid("@planId", planId);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) await component.ExecuteReaderAsync(), component.ProcedureName, component.RequestContext))
        {
          resultCollection.AddBinder<TaskOrchestrationPlan>(component.GetPlanBinder());
          planAsync = resultCollection.GetCurrent<TaskOrchestrationPlan>().FirstOrDefault<TaskOrchestrationPlan>();
        }
      }
      return planAsync;
    }
  }
}
