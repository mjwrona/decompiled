// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.DataAccess.TaskTrackingComponent17
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.DataAccess
{
  internal class TaskTrackingComponent17 : TaskTrackingComponent16
  {
    public override async Task<GetTaskOrchestrationJobResult> GetJobAsync(
      Guid scopeIdentifier,
      Guid planId,
      Guid jobId)
    {
      TaskTrackingComponent17 component = this;
      GetTaskOrchestrationJobResult jobAsync;
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) component, nameof (GetJobAsync)))
      {
        component.PrepareStoredProcedure("Task.prc_GetJob");
        component.BindDataspaceId(scopeIdentifier);
        component.BindGuid("@planId", planId);
        component.BindGuid("@jobId", jobId);
        GetTaskOrchestrationJobResult result = new GetTaskOrchestrationJobResult();
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) await component.ExecuteReaderAsync(), component.ProcedureName, component.RequestContext))
        {
          resultCollection.AddBinder<TaskOrchestrationPlan>(component.GetPlanBinder());
          resultCollection.AddBinder<TaskOrchestrationJob>((ObjectBinder<TaskOrchestrationJob>) new TaskOrchestrationJobBinder());
          result.Plan = resultCollection.GetCurrent<TaskOrchestrationPlan>().FirstOrDefault<TaskOrchestrationPlan>();
          resultCollection.NextResult();
          result.Job = resultCollection.GetCurrent<TaskOrchestrationJob>().FirstOrDefault<TaskOrchestrationJob>();
          jobAsync = result;
        }
      }
      return jobAsync;
    }

    public override TaskOrchestrationPlanReference GetPlanData(Guid scopeIdentifier, Guid planId)
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (GetPlanData)))
      {
        this.PrepareStoredProcedure("Task.prc_GetPlan");
        if (scopeIdentifier != Guid.Empty)
          this.BindDataspaceId(scopeIdentifier);
        else
          this.BindInt("@dataspaceId", 0);
        this.BindGuid("@planId", planId);
        this.BindBoolean("@includeSubscriptions", true);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<TaskOrchestrationPlanReference>(this.GetPlanReferenceBinder());
          return resultCollection.GetCurrent<TaskOrchestrationPlanReference>().FirstOrDefault<TaskOrchestrationPlanReference>();
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
      TaskTrackingComponent17 component = this;
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
  }
}
