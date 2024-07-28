// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.DataAccess.TaskTrackingComponent
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
  internal class TaskTrackingComponent : TaskSqlComponentBase
  {
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[45]
    {
      (IComponentCreator) new ComponentCreator<TaskTrackingComponent>(1),
      (IComponentCreator) new ComponentCreator<TaskTrackingComponent2>(2),
      (IComponentCreator) new ComponentCreator<TaskTrackingComponent3>(3),
      (IComponentCreator) new ComponentCreator<TaskTrackingComponent4>(4),
      (IComponentCreator) new ComponentCreator<TaskTrackingComponent5>(5),
      (IComponentCreator) new ComponentCreator<TaskTrackingComponent6>(6),
      (IComponentCreator) new ComponentCreator<TaskTrackingComponent7>(7),
      (IComponentCreator) new ComponentCreator<TaskTrackingComponent8>(8),
      (IComponentCreator) new ComponentCreator<TaskTrackingComponent9>(9),
      (IComponentCreator) new ComponentCreator<TaskTrackingComponent10>(10),
      (IComponentCreator) new ComponentCreator<TaskTrackingComponent11>(11),
      (IComponentCreator) new ComponentCreator<TaskTrackingComponent12>(12),
      (IComponentCreator) new ComponentCreator<TaskTrackingComponent13>(13),
      (IComponentCreator) new ComponentCreator<TaskTrackingComponent14>(14),
      (IComponentCreator) new ComponentCreator<TaskTrackingComponent15>(15),
      (IComponentCreator) new ComponentCreator<TaskTrackingComponent16>(16),
      (IComponentCreator) new ComponentCreator<TaskTrackingComponent17>(17),
      (IComponentCreator) new ComponentCreator<TaskTrackingComponent18>(18),
      (IComponentCreator) new ComponentCreator<TaskTrackingComponent19>(19),
      (IComponentCreator) new ComponentCreator<TaskTrackingComponent20>(20),
      (IComponentCreator) new ComponentCreator<TaskTrackingComponent21>(21),
      (IComponentCreator) new ComponentCreator<TaskTrackingComponent21>(22),
      (IComponentCreator) new ComponentCreator<TaskTrackingComponent23>(23),
      (IComponentCreator) new ComponentCreator<TaskTrackingComponent24>(24),
      (IComponentCreator) new ComponentCreator<TaskTrackingComponent25>(25),
      (IComponentCreator) new ComponentCreator<TaskTrackingComponent26>(26),
      (IComponentCreator) new ComponentCreator<TaskTrackingComponent27>(27),
      (IComponentCreator) new ComponentCreator<TaskTrackingComponent28>(28),
      (IComponentCreator) new ComponentCreator<TaskTrackingComponent29>(29),
      (IComponentCreator) new ComponentCreator<TaskTrackingComponent30>(30),
      (IComponentCreator) new ComponentCreator<TaskTrackingComponent31>(31),
      (IComponentCreator) new ComponentCreator<TaskTrackingComponent32>(32),
      (IComponentCreator) new ComponentCreator<TaskTrackingComponent33>(33),
      (IComponentCreator) new ComponentCreator<TaskTrackingComponent34>(34),
      (IComponentCreator) new ComponentCreator<TaskTrackingComponent35>(35),
      (IComponentCreator) new ComponentCreator<TaskTrackingComponent36>(36),
      (IComponentCreator) new ComponentCreator<TaskTrackingComponent37>(37),
      (IComponentCreator) new ComponentCreator<TaskTrackingComponent38>(38),
      (IComponentCreator) new ComponentCreator<TaskTrackingComponent39>(39),
      (IComponentCreator) new ComponentCreator<TaskTrackingComponent40>(40),
      (IComponentCreator) new ComponentCreator<TaskTrackingComponent41>(41),
      (IComponentCreator) new ComponentCreator<TaskTrackingComponent42>(42),
      (IComponentCreator) new ComponentCreator<TaskTrackingComponent43>(43),
      (IComponentCreator) new ComponentCreator<TaskTrackingComponent44>(44),
      (IComponentCreator) new ComponentCreator<TaskTrackingComponent45>(45)
    }, "DistributedTaskTracking");
    protected static SqlMetaData[] typ_JobTable = new SqlMetaData[3]
    {
      new SqlMetaData("JobId", SqlDbType.UniqueIdentifier),
      new SqlMetaData("JobName", SqlDbType.NVarChar, 512L),
      new SqlMetaData("JobData", SqlDbType.VarBinary, -1L)
    };
    protected static SqlMetaData[] typ_TimelineRecordTable = new SqlMetaData[14]
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
      new SqlMetaData("LogId", SqlDbType.Int)
    };

    public TaskTrackingComponent() => this.SelectedFeatures |= SqlResourceComponentFeatures.BindPartitionIdByDefault;

    public virtual bool SupportStoreJobInstance => false;

    public virtual Task<Timeline> AddJobsAsync(
      Guid scopeIdentifier,
      Guid planId,
      Guid requestedBy,
      IList<TaskOrchestrationJob> jobs,
      IList<TaskReferenceData> tasks,
      IList<TimelineRecord> records,
      IEnumerable<TimelineAttempt> attempts = null)
    {
      return Task.FromResult<Timeline>((Timeline) null);
    }

    public virtual Task<TaskAttachmentData> CreateAttachmentAsync(
      Guid scopeIdentifier,
      Guid planId,
      Guid timelineId,
      Guid recordId,
      string type,
      string name,
      string attachmentPath,
      Guid requestedBy)
    {
      throw new ServiceVersionNotSupportedException("DistributedTaskTracking", this.Version, 7);
    }

    public virtual TaskHub CreateHub(string name, string dataspaceCategory) => throw new ServiceVersionNotSupportedException("DistributedTaskTracking", this.Version, 6);

    public virtual async Task<TimelineRecordReference> GetTimelineRecordReferenceAsync(
      Guid scopeId,
      Guid planId,
      Guid timeLineRecordId)
    {
      TimelineRecord timelineRecord = (await this.GetTimelineRecordsAsync(scopeId, planId, Guid.Empty, (IEnumerable<Guid>) new Guid[1]
      {
        timeLineRecordId
      })).FirstOrDefault<TimelineRecord>();
      if (timelineRecord == null)
        return (TimelineRecordReference) null;
      return new TimelineRecordReference()
      {
        Id = timelineRecord.Id,
        Identifier = timelineRecord.Identifier,
        State = timelineRecord.State
      };
    }

    public virtual TaskLog CreateLog(
      Guid scopeIdentifier,
      Guid planId,
      Guid requestedBy,
      string logPath)
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (CreateLog)))
      {
        this.PrepareStoredProcedure("prc_CreateTaskLog");
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

    public virtual Task<TaskLog> CreateLogAsync(
      Guid scopeIdentifier,
      Guid planId,
      Guid requestedBy,
      string logPath)
    {
      return Task.FromResult<TaskLog>(this.CreateLog(scopeIdentifier, planId, requestedBy, logPath));
    }

    public virtual Tuple<TaskLog, TaskLogPage> CreateLogPage(
      Guid scopeIdentifier,
      Guid planId,
      int logId)
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (CreateLogPage)))
      {
        this.PrepareStoredProcedure("prc_CreateTaskLogPage");
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

    public virtual Task<Tuple<TaskLog, TaskLogPage>> CreateLogPageAsync(
      Guid scopeIdentifier,
      Guid planId,
      int logId)
    {
      return Task.FromResult<Tuple<TaskLog, TaskLogPage>>(this.CreateLogPage(scopeIdentifier, planId, logId));
    }

    public virtual TaskOrchestrationPlan CreatePlan(
      TaskOrchestrationPlan plan,
      Timeline timeline,
      IEnumerable<TaskOrchestrationJob> jobs,
      IEnumerable<TaskReferenceData> tasks,
      IEnumerable<TimelineAttempt> attempts,
      bool createInitializationLog = false,
      bool createExpandedYaml = false)
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (CreatePlan)))
      {
        this.PrepareStoredProcedure("prc_CreateTaskOrchestrationPlan");
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

    public virtual Timeline CreateTimeline(
      Guid scopeIdentifier,
      Guid planId,
      Guid requestedBy,
      Guid timelineId,
      IEnumerable<TimelineRecord> records)
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (CreateTimeline)))
      {
        this.PrepareStoredProcedure("prc_CreateTimeline");
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

    public virtual Task<Timeline> CreatePipelineAttemptAsync(
      Guid scopeIdentifier,
      Guid planId,
      IList<TimelineAttempt> stages,
      Timeline newAttempt,
      Guid requestedBy)
    {
      throw new NotSupportedException();
    }

    public virtual Task<Timeline> GetTimelineAttemptAsync(
      Guid scopeIdentifier,
      Guid planId,
      string identifier,
      int attempt)
    {
      throw new NotSupportedException();
    }

    public virtual Task<IList<Timeline>> GetAllTimelineAttemptsAsync(
      Guid scopeIdentifier,
      Guid planId,
      string identifier,
      IList<string> includedPhases = null)
    {
      throw new NotSupportedException();
    }

    public virtual void DeleteLog(Guid scopeIdentifier, Guid planId, int logId)
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (DeleteLog)))
      {
        this.PrepareStoredProcedure("prc_DeleteTaskLog");
        this.BindGuid("@planId", planId);
        this.BindInt("@logId", logId);
        this.ExecuteNonQuery();
      }
    }

    public virtual void DeleteTeamProject(Guid scopeIdentifier)
    {
    }

    public virtual void DeleteTimeline(Guid scopeIdentifier, Guid planId, Guid timelineId)
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (DeleteTimeline)))
      {
        this.PrepareStoredProcedure("prc_DeleteTimeline");
        this.BindGuid("@planId", planId);
        this.BindGuid("@timelineId", timelineId);
        this.ExecuteNonQuery();
      }
    }

    public virtual Task<TaskAttachmentData> GetAttachmentAsync(
      Guid scopeIdentifier,
      Guid planId,
      Guid timelineId,
      Guid recordId,
      string type,
      string name)
    {
      return Task.FromResult<TaskAttachmentData>((TaskAttachmentData) null);
    }

    public virtual Task<IList<TaskAttachmentData>> GetAttachmentsAsync(
      Guid scopeIdentifier,
      Guid planId,
      Guid? timelineId,
      Guid? recordId,
      string type)
    {
      return Task.FromResult<IList<TaskAttachmentData>>((IList<TaskAttachmentData>) Array.Empty<TaskAttachmentData>());
    }

    public virtual IList<TaskHub> GetHubs() => (IList<TaskHub>) Array.Empty<TaskHub>();

    public virtual TaskOrchestrationPlanReference GetPlanData(Guid scopeIdentifier, Guid planId)
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (GetPlanData)))
      {
        this.PrepareStoredProcedure("prc_GetTaskOrchestrationPlan");
        this.BindGuid("@planId", planId);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<TaskOrchestrationPlanReference>((ObjectBinder<TaskOrchestrationPlanReference>) new TaskOrchestrationPlanReferenceBinder());
          return resultCollection.GetCurrent<TaskOrchestrationPlanReference>().FirstOrDefault<TaskOrchestrationPlanReference>();
        }
      }
    }

    public virtual async Task<GetTaskOrchestrationJobResult> GetJobAsync(
      Guid scopeIdentifier,
      Guid planId,
      Guid jobId)
    {
      TaskTrackingComponent component = this;
      GetTaskOrchestrationJobResult jobAsync;
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) component, nameof (GetJobAsync)))
      {
        component.PrepareStoredProcedure("prc_GetTaskOrchestrationJob");
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

    public virtual Task<T> AddPlanContextAsync<T>(
      Guid scopeIdentifier,
      Guid planId,
      string contextName,
      T contextData)
    {
      return Task.FromResult<T>(default (T));
    }

    public virtual T AddPlanContext<T>(
      Guid scopeIdentifier,
      Guid planId,
      string contextName,
      T contextData)
    {
      return default (T);
    }

    public virtual Task DeletePlanContextAsync(
      Guid scopeIdentifier,
      Guid planId,
      string contextName)
    {
      return Task.CompletedTask;
    }

    public virtual Task<T> GetPlanContextAsync<T>(
      Guid scopeIdentifier,
      Guid planId,
      string contextName)
    {
      return Task.FromResult<T>(default (T));
    }

    public virtual Task<T> UpdatePlanContextAsync<T>(
      Guid scopeIdentifier,
      Guid planId,
      string contextName,
      T contextData)
    {
      return Task.FromResult<T>(default (T));
    }

    public virtual TaskLog GetLog(Guid scopeIdentifier, Guid planId, int logId)
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (GetLog)))
      {
        this.PrepareStoredProcedure("prc_GetTaskLogById");
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

    public virtual TaskLog GetLog(
      Guid scopeIdentifier,
      Guid planId,
      int logId,
      out IList<TaskLogPage> pages)
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (GetLog)))
      {
        this.PrepareStoredProcedure("prc_GetTaskLogById");
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

    public virtual TaskLog GetLog(Guid scopeIdentifier, Guid planId, string logPath)
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (GetLog)))
      {
        this.PrepareStoredProcedure("prc_GetTaskLog");
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

    public virtual TaskLog GetLog(
      Guid scopeIdentifier,
      Guid planId,
      string logPath,
      out IList<TaskLogPage> pages)
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (GetLog)))
      {
        this.PrepareStoredProcedure("prc_GetTaskLog");
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

    public virtual GetLogsResult GetLogs(Guid scopeIdentifier, Guid planId, bool includePages = false)
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (GetLogs)))
      {
        this.PrepareStoredProcedure("prc_GetTaskLogs");
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

    public virtual Task<IList<TaskLog>> GetLogsAsync(
      Guid scopeIdentifier,
      Guid planId,
      IList<int> logIds)
    {
      HashSet<int> idSet = new HashSet<int>((IEnumerable<int>) logIds);
      return Task.FromResult<IList<TaskLog>>((IList<TaskLog>) this.GetLogs(scopeIdentifier, planId).Logs.Where<TaskLog>((System.Func<TaskLog, bool>) (x => idSet.Contains(x.Id))).ToList<TaskLog>());
    }

    public virtual async Task<TaskOrchestrationPlan> GetPlanAsync(Guid scopeIdentifier, Guid planId)
    {
      TaskTrackingComponent component = this;
      TaskOrchestrationPlan planAsync;
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) component, nameof (GetPlanAsync)))
      {
        component.PrepareStoredProcedure("prc_GetTaskOrchestrationPlan");
        component.BindGuid("@planId", planId);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) await component.ExecuteReaderAsync(), component.ProcedureName, component.RequestContext))
        {
          resultCollection.AddBinder<TaskOrchestrationPlan>((ObjectBinder<TaskOrchestrationPlan>) new TaskOrchestrationPlanBinder());
          planAsync = resultCollection.GetCurrent<TaskOrchestrationPlan>().FirstOrDefault<TaskOrchestrationPlan>();
        }
      }
      return planAsync;
    }

    public virtual IList<TaskOrchestrationPlan> GetPlans(
      Guid scopeIdentifier,
      IList<Guid> planIds,
      IList<string> timelineRecordTypes)
    {
      return (IList<TaskOrchestrationPlan>) Array.Empty<TaskOrchestrationPlan>();
    }

    public virtual IList<TaskOrchestrationPlan> GetRunningPlansByDefinition(
      Guid scopeIdentifier,
      int definitionId,
      IList<string> timelineRecordTypes,
      int maxPlans)
    {
      return (IList<TaskOrchestrationPlan>) Array.Empty<TaskOrchestrationPlan>();
    }

    public virtual IList<TaskOrchestrationPlan> GetPlansByPlanGroup(
      string hubName,
      Guid scopeIdentifier,
      string planGroup)
    {
      return (IList<TaskOrchestrationPlan>) Array.Empty<TaskOrchestrationPlan>();
    }

    public virtual IList<TaskOrchestrationPlan> GetPlansByState(
      string hubName,
      TaskOrchestrationPlanState state,
      int? maxCount)
    {
      return (IList<TaskOrchestrationPlan>) Array.Empty<TaskOrchestrationPlan>();
    }

    public virtual IList<TaskOrchestrationPlan> GetRunnablePlans(
      ICollection<PlanConcurrency> concurrencyLimits,
      int maxCount)
    {
      throw new ServiceVersionNotSupportedException("DistributedTaskTracking", this.Version, 41);
    }

    public virtual Task<PlanLogTable> CreateLogTableAsync(
      Guid scopeIdentifier,
      Guid planId,
      string storageKey,
      string tableName,
      DateTime startedOn,
      DateTime expiryOn)
    {
      return Task.FromResult<PlanLogTable>((PlanLogTable) null);
    }

    public virtual Task<PlanLogTable> UpdateLogTableAsync(
      Guid scopeIdentifier,
      Guid planId,
      PlanLogTable logTable)
    {
      return Task.FromResult<PlanLogTable>((PlanLogTable) null);
    }

    public virtual Task<PlanLogTable> GetLogTableAsync(Guid scopeIdentifier, Guid planId) => Task.FromResult<PlanLogTable>((PlanLogTable) null);

    public virtual Task<IList<PlanLogTable>> GetExpiredLogTablesAsync(int maxCount) => Task.FromResult<IList<PlanLogTable>>((IList<PlanLogTable>) Array.Empty<PlanLogTable>());

    public virtual Task DeleteLogTablesAsync(IList<string> tableNames) => Task.CompletedTask;

    public virtual async Task<Timeline> GetTimelineAsync(
      Guid scopeIdentifier,
      Guid planId,
      Guid timelineId,
      int changeId = 0,
      bool includeRecords = true,
      bool includePreviousAttempts = true)
    {
      TaskTrackingComponent component = this;
      Timeline timelineAsync;
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) component, nameof (GetTimelineAsync)))
      {
        component.PrepareStoredProcedure("prc_GetTimeline");
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

    public virtual IEnumerable<Timeline> GetTimelines(Guid scopeIdentifier, Guid planId)
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (GetTimelines)))
      {
        this.PrepareStoredProcedure("prc_GetTimelines");
        this.BindGuid("@planId", planId);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<Timeline>((ObjectBinder<Timeline>) new TimelineBinder());
          return (IEnumerable<Timeline>) resultCollection.GetCurrent<Timeline>().Items;
        }
      }
    }

    public virtual async Task<IList<TimelineRecord>> GetTimelineRecordsAsync(
      Guid scopeIdentifier,
      Guid planId,
      Guid timelineId,
      IEnumerable<Guid> records,
      bool includeOutputs = false)
    {
      Timeline timelineAsync = await this.GetTimelineAsync(scopeIdentifier, planId, timelineId);
      if (timelineAsync == null)
        return (IList<TimelineRecord>) Array.Empty<TimelineRecord>();
      HashSet<Guid> recordFilter = new HashSet<Guid>(records);
      return (IList<TimelineRecord>) timelineAsync.Records.Where<TimelineRecord>((System.Func<TimelineRecord, bool>) (x => recordFilter.Contains(x.Id))).ToList<TimelineRecord>();
    }

    public virtual void UpdateHub(string hubName, bool enableThrottling)
    {
    }

    public virtual TaskLog UpdateLogPage(
      Guid scopeIdentifier,
      Guid planId,
      int logId,
      int pageId,
      long lineCount,
      TaskLogPageState state)
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (UpdateLogPage)))
      {
        this.PrepareStoredProcedure("prc_UpdateTaskLogPage");
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

    public virtual Task<TaskLog> UpdateLogPageAsync(
      Guid scopeIdentifier,
      Guid planId,
      int logId,
      int pageId,
      long lineCount,
      TaskLogPageState state,
      string blobFileId = null)
    {
      return Task.FromResult<TaskLog>(this.UpdateLogPage(scopeIdentifier, planId, logId, pageId, lineCount, state));
    }

    public virtual async Task<UpdatePlanResult> UpdatePlanAsync(
      Guid scopeIdentifier,
      Guid planId,
      DateTime? startTime,
      DateTime? finishTime,
      TaskOrchestrationPlanState? state,
      TaskResult? result,
      string resultCode,
      IOrchestrationEnvironment environment)
    {
      TaskTrackingComponent component = this;
      UpdatePlanResult updatePlanResult;
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) component, nameof (UpdatePlanAsync)))
      {
        component.PrepareStoredProcedure("prc_UpdateTaskOrchestrationPlan");
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

    public virtual async Task<Timeline> UpdateTimelineAsync(
      Guid scopeIdentifier,
      Guid planId,
      Guid timelineId,
      Guid requestedBy,
      IList<TimelineRecord> records,
      int blockingPeriod = 0)
    {
      TaskTrackingComponent component = this;
      Timeline timeline1;
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) component, nameof (UpdateTimelineAsync)))
      {
        component.PrepareStoredProcedure("prc_UpdateTimeline");
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

    public virtual void DeletePlans(Guid scopeIdentifier, IEnumerable<Guid> planIds)
    {
    }

    public virtual int GetPlanStartedCount(DateTime startTime, DateTime endTime) => -1;

    public virtual Task DeletePlanContextsAsync(Guid scopeIdentifier, Guid planId) => Task.CompletedTask;

    protected virtual SqlParameter BindTimelineRecordTable(
      string parameterName,
      IEnumerable<TimelineRecord> rows)
    {
      return this.BindTable(parameterName, "typ_TimelineRecordTable", (rows ?? Enumerable.Empty<TimelineRecord>()).Select<TimelineRecord, SqlDataRecord>(new System.Func<TimelineRecord, SqlDataRecord>(this.ConvertToSqlDataRecord)));
    }

    protected virtual SqlParameter BindJobTable(
      string parameterName,
      IEnumerable<TaskOrchestrationJob> rows)
    {
      return this.BindTable(parameterName, "typ_TaskJobTable", (rows ?? Enumerable.Empty<TaskOrchestrationJob>()).Select<TaskOrchestrationJob, SqlDataRecord>(new System.Func<TaskOrchestrationJob, SqlDataRecord>(this.ConvertToSqlDataRecord)));
    }

    protected virtual SqlDataRecord ConvertToSqlDataRecord(TaskOrchestrationJob row)
    {
      SqlDataRecord sqlDataRecord = new SqlDataRecord(TaskTrackingComponent.typ_JobTable);
      sqlDataRecord.SetGuid(0, row.InstanceId);
      sqlDataRecord.SetString(1, row.Name);
      byte[] buffer = JsonUtility.Serialize((object) row);
      sqlDataRecord.SetBytes(2, 0L, buffer, 0, buffer.Length);
      return sqlDataRecord;
    }

    protected virtual SqlDataRecord ConvertToSqlDataRecord(TimelineRecord row)
    {
      SqlDataRecord record = new SqlDataRecord(TaskTrackingComponent.typ_TimelineRecordTable);
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
      return record;
    }

    protected virtual ObjectBinder<TimelineRecordVariableData> GetTimelineRecordVariableBinder() => (ObjectBinder<TimelineRecordVariableData>) new TimelineRecordVariableBinder();

    protected virtual ObjectBinder<TaskLogPage> GetTaskLogPageBinder() => (ObjectBinder<TaskLogPage>) new TaskLogPageBinder();
  }
}
