// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.DataAccess.TaskResourceComponent5
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.DistributedTask.Server.DataAccess
{
  internal class TaskResourceComponent5 : TaskResourceComponent4
  {
    public override TaskAgentJobRequest AbandonAgentRequest(
      int poolId,
      long requestId,
      DateTime expirationTime)
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (AbandonAgentRequest)))
      {
        this.PrepareStoredProcedure("Task.prc_AbandonAgentJobRequest");
        this.BindInt("@poolId", poolId);
        this.BindLong("@requestId", requestId);
        this.BindDateTime2("@expirationTime", expirationTime);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<TaskAgentJobRequest>((ObjectBinder<TaskAgentJobRequest>) new TaskAgentRequestBinder4());
          return resultCollection.GetCurrent<TaskAgentJobRequest>().FirstOrDefault<TaskAgentJobRequest>();
        }
      }
    }

    public override async Task<FinishAgentRequestResult> FinishAgentRequestAsync(
      int poolId,
      long requestId,
      bool deprovisionAgent,
      bool transitionAgentState,
      TaskResult? jobResult = null,
      bool agentShuttingDown = false)
    {
      TaskResourceComponent5 component = this;
      FinishAgentRequestResult agentRequestResult;
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) component, nameof (FinishAgentRequestAsync)))
      {
        component.PrepareStoredProcedure("Task.prc_FinishAgentJobRequest");
        component.BindInt("@poolId", poolId);
        component.BindLong("@requestId", requestId);
        using (new ResultCollection((IDataReader) await component.ExecuteReaderAsync(), component.ProcedureName, component.RequestContext))
          agentRequestResult = new FinishAgentRequestResult();
      }
      return agentRequestResult;
    }

    public override TaskAgentRequestData GetAgentRequest(
      int poolId,
      long requestId,
      bool includeStatus = false)
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (GetAgentRequest)))
      {
        TaskAgentRequestData agentRequest1 = (TaskAgentRequestData) null;
        this.PrepareStoredProcedure("Task.prc_GetAgentJobRequest");
        this.BindInt("@poolId", poolId);
        this.BindLong("@requestId", requestId);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<TaskAgentJobRequest>((ObjectBinder<TaskAgentJobRequest>) new TaskAgentRequestBinder4());
          TaskAgentJobRequest agentRequest2 = resultCollection.GetCurrent<TaskAgentJobRequest>().FirstOrDefault<TaskAgentJobRequest>();
          if (agentRequest2 != null)
            agentRequest1 = new TaskAgentRequestData(agentRequest2);
        }
        return agentRequest1;
      }
    }

    public override async Task<ExpiredAgentRequestsResult> GetExpiredAgentRequestsAsync(int poolId)
    {
      TaskResourceComponent5 component = this;
      ExpiredAgentRequestsResult agentRequestsAsync;
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) component, nameof (GetExpiredAgentRequestsAsync)))
      {
        component.PrepareStoredProcedure("Task.prc_GetExpiredAgentJobRequests");
        component.BindInt("@poolId", poolId);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) await component.ExecuteReaderAsync(), component.ProcedureName, component.RequestContext))
        {
          resultCollection.AddBinder<TaskAgentJobRequest>((ObjectBinder<TaskAgentJobRequest>) new TaskAgentRequestBinder4());
          agentRequestsAsync = new ExpiredAgentRequestsResult(true, (IList<TaskAgentJobRequest>) resultCollection.GetCurrent<TaskAgentJobRequest>().Items);
        }
      }
      return agentRequestsAsync;
    }

    public override async Task<IList<TaskAgentJobRequest>> GetUnassignedAgentRequestsAsync(
      int poolId)
    {
      TaskResourceComponent5 component = this;
      IList<TaskAgentJobRequest> items;
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) component, nameof (GetUnassignedAgentRequestsAsync)))
      {
        component.PrepareStoredProcedure("Task.prc_GetUnassignedAgentJobRequests");
        component.BindInt("@poolId", poolId);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) await component.ExecuteReaderAsync(), component.ProcedureName, component.RequestContext))
        {
          resultCollection.AddBinder<TaskAgentJobRequest>((ObjectBinder<TaskAgentJobRequest>) new TaskAgentRequestBinder4());
          items = (IList<TaskAgentJobRequest>) resultCollection.GetCurrent<TaskAgentJobRequest>().Items;
        }
      }
      return items;
    }

    public override async Task<TaskAgentJobRequest> QueueAgentRequestAsync(
      int poolId,
      int? agentCloudId,
      TaskAgentJobRequest request,
      string requestResourceType)
    {
      TaskResourceComponent5 component = this;
      TaskAgentJobRequest taskAgentJobRequest;
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) component, nameof (QueueAgentRequestAsync)))
      {
        component.PrepareStoredProcedure("prc_QueueTaskAgentJobRequest");
        component.BindInt("@poolId", poolId);
        component.BindInt32Table("@matchedAgentIds", (IEnumerable<int>) request.MatchedAgents.Select<TaskAgentReference, int>((System.Func<TaskAgentReference, int>) (x => x.Id)).Distinct<int>().ToList<int>());
        component.BindGuid("@serviceOwner", request.ServiceOwner);
        component.BindGuid("@hostId", request.HostId);
        component.BindGuid("@planId", request.PlanId);
        component.BindGuid("@jobId", request.JobId);
        component.BindBinary("@demands", JsonUtility.Serialize((object) request.Demands, false), int.MaxValue, SqlDbType.VarBinary);
        component.BindString("@orchestrationId", string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0:N}_{1:N}", (object) request.PlanId, (object) request.JobId), 256, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) await component.ExecuteReaderAsync(), component.ProcedureName, component.RequestContext))
        {
          resultCollection.AddBinder<TaskAgentJobRequest>((ObjectBinder<TaskAgentJobRequest>) new TaskAgentRequestBinder4());
          taskAgentJobRequest = resultCollection.GetCurrent<TaskAgentJobRequest>().FirstOrDefault<TaskAgentJobRequest>();
        }
      }
      return taskAgentJobRequest;
    }

    public override AgentConnectivityResult SetAgentOnline(int poolId, int agentId, int sequenceId)
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (SetAgentOnline)))
      {
        this.PrepareStoredProcedure("Task.prc_SetAgentOnline");
        this.BindInt("@poolId", poolId);
        this.BindInt("@agentId", agentId);
        this.BindInt("@sequenceId", sequenceId);
        this.ExecuteNonQuery();
        return new AgentConnectivityResult()
        {
          HandledEvent = true
        };
      }
    }

    public override UpdateAgentRequestResult UpdateAgentRequest(
      int poolId,
      long requestId,
      TimeSpan leaseRenewalTimeout,
      DateTime? expirationTime = null,
      DateTime? startTime = null,
      DateTime? finishTime = null,
      TaskResult? result = null)
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (UpdateAgentRequest)))
      {
        this.PrepareStoredProcedure("Task.prc_UpdateAgentJobRequest");
        this.BindInt("@poolId", poolId);
        this.BindLong("@requestId", requestId);
        this.BindGuid("@lockToken", Guid.Empty);
        if (expirationTime.HasValue)
          this.BindDateTime2("@expirationTime", expirationTime.Value);
        if (finishTime.HasValue)
          this.BindDateTime2("@finishTime", finishTime.Value);
        if (result.HasValue)
          this.BindByte("@result", (byte) result.Value);
        UpdateAgentRequestResult agentRequestResult = new UpdateAgentRequestResult();
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<TaskAgentJobRequest>((ObjectBinder<TaskAgentJobRequest>) new TaskAgentRequestBinder4());
          agentRequestResult.After = resultCollection.GetCurrent<TaskAgentJobRequest>().FirstOrDefault<TaskAgentJobRequest>();
          return agentRequestResult;
        }
      }
    }

    public override async Task<TaskAgentJobRequest> UpdateAgentRequestMatchesAsync(
      int poolId,
      long requestId,
      IList<int> matchingAgents,
      bool matchaseAllAgents)
    {
      TaskResourceComponent5 component = this;
      TaskAgentJobRequest taskAgentJobRequest;
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) component, nameof (UpdateAgentRequestMatchesAsync)))
      {
        component.PrepareStoredProcedure("Task.prc_UpdateAgentJobRequestMatches");
        component.BindInt("@poolId", poolId);
        component.BindLong("@requestId", requestId);
        component.BindInt32Table("@matchedAgentIds", (IEnumerable<int>) matchingAgents);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) await component.ExecuteReaderAsync(), component.ProcedureName, component.RequestContext))
        {
          resultCollection.AddBinder<TaskAgentJobRequest>((ObjectBinder<TaskAgentJobRequest>) new TaskAgentRequestBinder4());
          taskAgentJobRequest = resultCollection.GetCurrent<TaskAgentJobRequest>().FirstOrDefault<TaskAgentJobRequest>();
        }
      }
      return taskAgentJobRequest;
    }
  }
}
