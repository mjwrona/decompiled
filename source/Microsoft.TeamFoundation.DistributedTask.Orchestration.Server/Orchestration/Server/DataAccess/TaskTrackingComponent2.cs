// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.DataAccess.TaskTrackingComponent2
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
  internal class TaskTrackingComponent2 : TaskTrackingComponent
  {
    public override TaskLog CreateLog(
      Guid scopeIdentifier,
      Guid planId,
      Guid requestedBy,
      string logPath)
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (CreateLog)))
      {
        this.PrepareStoredProcedure("Task.prc_CreateLog");
        this.BindGuid("@planId", planId);
        this.BindGuid("@requestedBy", requestedBy);
        this.BindString("@logPath", logPath, 400, BindStringBehavior.NullToEmptyString, SqlDbType.NVarChar);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<TaskLog>((ObjectBinder<TaskLog>) new TaskLogBinder());
          return resultCollection.GetCurrent<TaskLog>().FirstOrDefault<TaskLog>();
        }
      }
    }

    public override Tuple<TaskLog, TaskLogPage> CreateLogPage(
      Guid scopeIdentifier,
      Guid planId,
      int logId)
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (CreateLogPage)))
      {
        this.PrepareStoredProcedure("Task.prc_CreateLogPage");
        this.BindGuid("@planId", planId);
        this.BindInt("@logId", logId);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<TaskLog>((ObjectBinder<TaskLog>) new TaskLogBinder());
          resultCollection.AddBinder<TaskLogPage>(this.GetTaskLogPageBinder());
          return new Tuple<TaskLog, TaskLogPage>(resultCollection.GetCurrent<TaskLog>().FirstOrDefault<TaskLog>(), !resultCollection.TryNextResult() ? (TaskLogPage) null : resultCollection.GetCurrent<TaskLogPage>().FirstOrDefault<TaskLogPage>());
        }
      }
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
        this.BindGuid("@planId", plan.PlanId);
        this.BindString("@artifactUri", plan.ArtifactUri.ToString(), 128, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
        this.BindLong("@containerId", plan.ContainerId);
        this.BindBinary("@environment", JsonUtility.Serialize((object) plan.ProcessEnvironment), SqlDbType.VarBinary);
        this.BindBinary("@implementation", JsonUtility.Serialize((object) plan.Process), SqlDbType.VarBinary);
        this.BindJobTable("@jobs", jobs);
        this.BindGuid("@requestedBy", plan.RequestedById);
        this.BindGuid("@timelineId", timeline.Id);
        this.BindTimelineRecordTable("@timelineRecords", (IEnumerable<TimelineRecord>) timeline.Records);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<TaskOrchestrationPlan>((ObjectBinder<TaskOrchestrationPlan>) new TaskOrchestrationPlanBinder());
          return resultCollection.GetCurrent<TaskOrchestrationPlan>().FirstOrDefault<TaskOrchestrationPlan>();
        }
      }
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
        this.BindGuid("@planId", planId);
        this.BindGuid("@requestedBy", requestedBy);
        this.BindGuid("@timelineId", timelineId);
        this.BindTimelineRecordTable("@records", records);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<Timeline>((ObjectBinder<Timeline>) new TimelineBinder());
          resultCollection.AddBinder<TimelineRecord>((ObjectBinder<TimelineRecord>) new TimelineRecordBinder());
          Timeline timeline = resultCollection.GetCurrent<Timeline>().FirstOrDefault<Timeline>();
          if (timeline != null && resultCollection.TryNextResult())
            timeline.Records.AddRange((IEnumerable<TimelineRecord>) resultCollection.GetCurrent<TimelineRecord>().Items);
          return timeline;
        }
      }
    }

    public override void DeleteLog(Guid scopeIdentifier, Guid planId, int logId)
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (DeleteLog)))
      {
        this.PrepareStoredProcedure("Task.prc_DeleteLog");
        this.BindGuid("@planId", planId);
        this.BindInt("@logId", logId);
        this.ExecuteNonQuery();
      }
    }

    public override void DeleteTimeline(Guid scopeIdentifier, Guid planId, Guid timelineId)
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (DeleteTimeline)))
      {
        this.PrepareStoredProcedure("Task.prc_DeleteTimeline");
        this.BindGuid("@planId", planId);
        this.BindGuid("@timelineId", timelineId);
        this.ExecuteNonQuery();
      }
    }

    public override async Task<GetTaskOrchestrationJobResult> GetJobAsync(
      Guid scopeIdentifier,
      Guid planId,
      Guid jobId)
    {
      TaskTrackingComponent2 component = this;
      GetTaskOrchestrationJobResult jobAsync;
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) component, nameof (GetJobAsync)))
      {
        component.PrepareStoredProcedure("Task.prc_GetJob");
        component.BindGuid("@planId", planId);
        component.BindGuid("@jobId", jobId);
        GetTaskOrchestrationJobResult result = new GetTaskOrchestrationJobResult();
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) await component.ExecuteReaderAsync(), component.ProcedureName, component.RequestContext))
        {
          resultCollection.AddBinder<TaskOrchestrationPlan>((ObjectBinder<TaskOrchestrationPlan>) new TaskOrchestrationPlanBinder());
          resultCollection.AddBinder<TaskOrchestrationJob>((ObjectBinder<TaskOrchestrationJob>) new TaskOrchestrationJobBinder());
          result.Plan = resultCollection.GetCurrent<TaskOrchestrationPlan>().FirstOrDefault<TaskOrchestrationPlan>();
          resultCollection.NextResult();
          result.Job = resultCollection.GetCurrent<TaskOrchestrationJob>().FirstOrDefault<TaskOrchestrationJob>();
        }
        jobAsync = result;
      }
      return jobAsync;
    }

    public override TaskLog GetLog(Guid scopeIdentifier, Guid planId, int logId)
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (GetLog)))
      {
        this.PrepareStoredProcedure("Task.prc_GetLogById");
        this.BindGuid("@planId", planId);
        this.BindInt("@logId", logId);
        this.BindBoolean("@includePages", false);
        this.BindBoolean("@includeIndex", false);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<TaskLog>((ObjectBinder<TaskLog>) new TaskLogBinder());
          return resultCollection.GetCurrent<TaskLog>().FirstOrDefault<TaskLog>();
        }
      }
    }

    public override TaskLog GetLog(
      Guid scopeIdentifier,
      Guid planId,
      int logId,
      out IList<TaskLogPage> pages)
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (GetLog)))
      {
        this.PrepareStoredProcedure("Task.prc_GetLogById");
        this.BindGuid("@planId", planId);
        this.BindInt("@logId", logId);
        this.BindBoolean("@includePages", true);
        this.BindBoolean("@includeIndex", false);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<TaskLog>((ObjectBinder<TaskLog>) new TaskLogBinder());
          resultCollection.AddBinder<TaskLogPage>(this.GetTaskLogPageBinder());
          TaskLog log = resultCollection.GetCurrent<TaskLog>().FirstOrDefault<TaskLog>();
          pages = !resultCollection.TryNextResult() ? (IList<TaskLogPage>) Array.Empty<TaskLogPage>() : (IList<TaskLogPage>) resultCollection.GetCurrent<TaskLogPage>().Items;
          return log;
        }
      }
    }

    public override TaskLog GetLog(Guid scopeIdentifier, Guid planId, string logPath)
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (GetLog)))
      {
        this.PrepareStoredProcedure("Task.prc_GetLog");
        this.BindGuid("@planId", planId);
        this.BindString("@logPath", logPath, 400, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
        this.BindBoolean("@includePages", false);
        this.BindBoolean("@includeIndex", false);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<TaskLog>((ObjectBinder<TaskLog>) new TaskLogBinder());
          return resultCollection.GetCurrent<TaskLog>().FirstOrDefault<TaskLog>();
        }
      }
    }

    public override TaskLog GetLog(
      Guid scopeIdentifier,
      Guid planId,
      string logPath,
      out IList<TaskLogPage> pages)
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (GetLog)))
      {
        this.PrepareStoredProcedure("Task.prc_GetLog");
        this.BindGuid("@planId", planId);
        this.BindString("@logPath", logPath, 400, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
        this.BindBoolean("@includePages", true);
        this.BindBoolean("@includeIndex", false);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<TaskLog>((ObjectBinder<TaskLog>) new TaskLogBinder());
          resultCollection.AddBinder<TaskLogPage>(this.GetTaskLogPageBinder());
          TaskLog log = resultCollection.GetCurrent<TaskLog>().FirstOrDefault<TaskLog>();
          pages = !resultCollection.TryNextResult() ? (IList<TaskLogPage>) Array.Empty<TaskLogPage>() : (IList<TaskLogPage>) resultCollection.GetCurrent<TaskLogPage>().Items;
          return log;
        }
      }
    }

    public override GetLogsResult GetLogs(Guid scopeIdentifier, Guid planId, bool includePages = false)
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (GetLogs)))
      {
        this.PrepareStoredProcedure("Task.prc_GetLogs");
        this.BindGuid("@planId", planId);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<TaskLog>((ObjectBinder<TaskLog>) new TaskLogBinder());
          return new GetLogsResult()
          {
            Logs = (IEnumerable<TaskLog>) resultCollection.GetCurrent<TaskLog>().Items
          };
        }
      }
    }

    public override async Task<TaskOrchestrationPlan> GetPlanAsync(
      Guid scopeIdentifier,
      Guid planId)
    {
      TaskTrackingComponent2 component = this;
      TaskOrchestrationPlan planAsync;
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) component, nameof (GetPlanAsync)))
      {
        component.PrepareStoredProcedure("Task.prc_GetPlan");
        component.BindGuid("@planId", planId);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) await component.ExecuteReaderAsync(), component.ProcedureName, component.RequestContext))
        {
          resultCollection.AddBinder<TaskOrchestrationPlan>((ObjectBinder<TaskOrchestrationPlan>) new TaskOrchestrationPlanBinder());
          planAsync = resultCollection.GetCurrent<TaskOrchestrationPlan>().FirstOrDefault<TaskOrchestrationPlan>();
        }
      }
      return planAsync;
    }

    public override TaskOrchestrationPlanReference GetPlanData(Guid scopeIdentifier, Guid planId)
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (GetPlanData)))
      {
        this.PrepareStoredProcedure("Task.prc_GetPlan");
        this.BindGuid("@planId", planId);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<TaskOrchestrationPlanReference>((ObjectBinder<TaskOrchestrationPlanReference>) new TaskOrchestrationPlanReferenceBinder());
          return resultCollection.GetCurrent<TaskOrchestrationPlanReference>().FirstOrDefault<TaskOrchestrationPlanReference>();
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
      TaskTrackingComponent2 component = this;
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
          resultCollection.AddBinder<TimelineRecord>((ObjectBinder<TimelineRecord>) new TimelineRecordBinder());
          Timeline timeline = resultCollection.GetCurrent<Timeline>().FirstOrDefault<Timeline>();
          if (timeline != null && resultCollection.TryNextResult())
            timeline.Records.AddRange((IEnumerable<TimelineRecord>) resultCollection.GetCurrent<TimelineRecord>().Items);
          timelineAsync = timeline;
        }
      }
      return timelineAsync;
    }

    public override IEnumerable<Timeline> GetTimelines(Guid scopeIdentifier, Guid planId)
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (GetTimelines)))
      {
        this.PrepareStoredProcedure("Task.prc_GetTimelines");
        this.BindGuid("@planId", planId);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<Timeline>((ObjectBinder<Timeline>) new TimelineBinder());
          return (IEnumerable<Timeline>) resultCollection.GetCurrent<Timeline>().Items;
        }
      }
    }

    public override TaskLog UpdateLogPage(
      Guid scopeIdentifier,
      Guid planId,
      int logId,
      int pageId,
      long lineCount,
      TaskLogPageState state)
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (UpdateLogPage)))
      {
        this.PrepareStoredProcedure("Task.prc_UpdateLogPage");
        this.BindGuid("@planId", planId);
        this.BindInt("@logId", logId);
        this.BindInt("@pageId", pageId);
        this.BindInt("@state", (int) state);
        this.BindLong("@lineCount", lineCount);
        this.BindNullValue("@logIndex", SqlDbType.VarBinary);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<TaskLog>((ObjectBinder<TaskLog>) new TaskLogBinder());
          return resultCollection.GetCurrent<TaskLog>().FirstOrDefault<TaskLog>();
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
      TaskTrackingComponent2 component = this;
      UpdatePlanResult updatePlanResult;
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) component, nameof (UpdatePlanAsync)))
      {
        component.PrepareStoredProcedure("Task.prc_UpdatePlan");
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
        UpdatePlanResult updateResult = new UpdatePlanResult();
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) await component.ExecuteReaderAsync(), component.ProcedureName, component.RequestContext))
        {
          resultCollection.AddBinder<TaskOrchestrationPlan>((ObjectBinder<TaskOrchestrationPlan>) new TaskOrchestrationPlanBinder());
          updateResult.Plan = resultCollection.GetCurrent<TaskOrchestrationPlan>().FirstOrDefault<TaskOrchestrationPlan>();
          updatePlanResult = updateResult;
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
      TaskTrackingComponent2 component = this;
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
          resultCollection.AddBinder<TimelineRecord>((ObjectBinder<TimelineRecord>) new TimelineRecordBinder());
          Timeline timeline2 = resultCollection.GetCurrent<Timeline>().FirstOrDefault<Timeline>();
          if (timeline2 != null && resultCollection.TryNextResult())
            timeline2.Records.AddRange((IEnumerable<TimelineRecord>) resultCollection.GetCurrent<TimelineRecord>().Items);
          timeline1 = timeline2;
        }
      }
      return timeline1;
    }

    protected override SqlParameter BindJobTable(
      string parameterName,
      IEnumerable<TaskOrchestrationJob> rows)
    {
      return this.BindTable(parameterName, "Task.typ_JobTable", (rows ?? Enumerable.Empty<TaskOrchestrationJob>()).Select<TaskOrchestrationJob, SqlDataRecord>(new System.Func<TaskOrchestrationJob, SqlDataRecord>(((TaskTrackingComponent) this).ConvertToSqlDataRecord)));
    }

    protected override SqlParameter BindTimelineRecordTable(
      string parameterName,
      IEnumerable<TimelineRecord> rows)
    {
      return this.BindTable(parameterName, "Task.typ_TimelineRecordTable", (rows ?? Enumerable.Empty<TimelineRecord>()).Select<TimelineRecord, SqlDataRecord>(new System.Func<TimelineRecord, SqlDataRecord>(((TaskTrackingComponent) this).ConvertToSqlDataRecord)));
    }
  }
}
