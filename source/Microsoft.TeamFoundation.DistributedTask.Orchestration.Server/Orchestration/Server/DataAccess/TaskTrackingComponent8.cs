// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.DataAccess.TaskTrackingComponent8
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.DataAccess
{
  internal class TaskTrackingComponent8 : TaskTrackingComponent7
  {
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
        this.BindString("@artifactUri", plan.ArtifactUri.ToString(), 128, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
        this.BindLong("@containerId", plan.ContainerId);
        this.BindBinary("@environment", JsonUtility.Serialize((object) plan.ProcessEnvironment), SqlDbType.VarBinary);
        this.BindBinary("@implementation", JsonUtility.Serialize((object) plan.Process), SqlDbType.VarBinary);
        this.BindJobTable("@jobs", jobs);
        this.BindGuid("@requestedBy", plan.RequestedById);
        this.BindGuid("@requestedFor", plan.RequestedForId);
        this.BindGuid("@timelineId", timeline.Id);
        this.BindTimelineRecordTable("@timelineRecords", (IEnumerable<TimelineRecord>) timeline.Records);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<TaskOrchestrationPlan>((ObjectBinder<TaskOrchestrationPlan>) new TaskOrchestrationPlanBinder3((TaskTrackingComponent) this));
          return resultCollection.GetCurrent<TaskOrchestrationPlan>().FirstOrDefault<TaskOrchestrationPlan>();
        }
      }
    }

    public override async Task<TaskOrchestrationPlan> GetPlanAsync(
      Guid scopeIdentifier,
      Guid planId)
    {
      TaskTrackingComponent8 component = this;
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
          resultCollection.AddBinder<TaskOrchestrationPlan>((ObjectBinder<TaskOrchestrationPlan>) new TaskOrchestrationPlanBinder3((TaskTrackingComponent) component));
          planAsync = resultCollection.GetCurrent<TaskOrchestrationPlan>().FirstOrDefault<TaskOrchestrationPlan>();
        }
      }
      return planAsync;
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
      TaskTrackingComponent8 component = this;
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
        UpdatePlanResult updateResult = new UpdatePlanResult();
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) await component.ExecuteReaderAsync(), component.ProcedureName, component.RequestContext))
        {
          resultCollection.AddBinder<TaskOrchestrationPlan>((ObjectBinder<TaskOrchestrationPlan>) new TaskOrchestrationPlanBinder3((TaskTrackingComponent) component));
          resultCollection.AddBinder<Timeline>((ObjectBinder<Timeline>) new TimelineBinder());
          resultCollection.AddBinder<TimelineRecord>((ObjectBinder<TimelineRecord>) new TimelineRecordBinder3());
          updateResult.Plan = resultCollection.GetCurrent<TaskOrchestrationPlan>().FirstOrDefault<TaskOrchestrationPlan>();
          resultCollection.NextResult();
          updateResult.Timeline = resultCollection.GetCurrent<Timeline>().FirstOrDefault<Timeline>();
          if (updateResult.Timeline != null)
          {
            resultCollection.NextResult();
            updateResult.Timeline.Records.AddRange((IEnumerable<TimelineRecord>) resultCollection.GetCurrent<TimelineRecord>().Items);
          }
          updatePlanResult = updateResult;
        }
      }
      return updatePlanResult;
    }
  }
}
