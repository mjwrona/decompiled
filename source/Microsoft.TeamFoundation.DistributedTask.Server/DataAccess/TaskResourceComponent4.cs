// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.DataAccess.TaskResourceComponent4
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
  internal class TaskResourceComponent4 : TaskResourceComponent3
  {
    public override CreateAgentSessionResult CreateAgentSession(
      int poolId,
      TaskAgentReference agent,
      string ownerName,
      IDictionary<string, string> systemCapabilities,
      bool transitionAgentState)
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (CreateAgentSession)))
      {
        this.PrepareStoredProcedure("Task.prc_CreateAgentSession");
        this.BindInt("@poolId", poolId);
        this.BindInt("@agentId", agent.Id);
        this.BindString("@ownerName", ownerName, 512, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
        this.BindGuid("@writerId", this.Author);
        this.BindKeyValuePairStringTable("@systemCapabilities", (IEnumerable<KeyValuePair<string, string>>) systemCapabilities);
        CreateAgentSessionResult agentSession = new CreateAgentSessionResult();
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<bool>((ObjectBinder<bool>) new BooleanBinder());
          resultCollection.AddBinder<TaskAgentSessionData>((ObjectBinder<TaskAgentSessionData>) new TaskAgentSessionDataBinder());
          resultCollection.AddBinder<TaskAgentSessionData>((ObjectBinder<TaskAgentSessionData>) new TaskAgentSessionDataBinder());
          agentSession.RecalculateRequestMatches = resultCollection.GetCurrent<bool>().First<bool>();
          resultCollection.NextResult();
          agentSession.OldSession = resultCollection.GetCurrent<TaskAgentSessionData>().FirstOrDefault<TaskAgentSessionData>();
          resultCollection.NextResult();
          agentSession.NewSession = resultCollection.GetCurrent<TaskAgentSessionData>().FirstOrDefault<TaskAgentSessionData>();
          return agentSession;
        }
      }
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
          resultCollection.AddBinder<TaskAgentJobRequest>((ObjectBinder<TaskAgentJobRequest>) new TaskAgentRequestBinder3());
          TaskAgentJobRequest agentRequest2 = resultCollection.GetCurrent<TaskAgentJobRequest>().FirstOrDefault<TaskAgentJobRequest>();
          if (agentRequest2 != null)
            agentRequest1 = new TaskAgentRequestData(agentRequest2);
        }
        return agentRequest1;
      }
    }

    public override async Task<ExpiredAgentRequestsResult> GetExpiredAgentRequestsAsync(int poolId)
    {
      TaskResourceComponent4 component = this;
      ExpiredAgentRequestsResult agentRequestsAsync;
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) component, nameof (GetExpiredAgentRequestsAsync)))
      {
        component.PrepareStoredProcedure("Task.prc_GetExpiredAgentJobRequests");
        component.BindInt("@poolId", poolId);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) await component.ExecuteReaderAsync(), component.ProcedureName, component.RequestContext))
        {
          resultCollection.AddBinder<TaskAgentJobRequest>((ObjectBinder<TaskAgentJobRequest>) new TaskAgentRequestBinder3());
          agentRequestsAsync = new ExpiredAgentRequestsResult(true, (IList<TaskAgentJobRequest>) resultCollection.GetCurrent<TaskAgentJobRequest>().Items);
        }
      }
      return agentRequestsAsync;
    }

    public override async Task<IList<TaskAgentJobRequest>> GetUnassignedAgentRequestsAsync(
      int poolId)
    {
      TaskResourceComponent4 component = this;
      IList<TaskAgentJobRequest> items;
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) component, nameof (GetUnassignedAgentRequestsAsync)))
      {
        component.PrepareStoredProcedure("Task.prc_GetUnassignedAgentJobRequests");
        component.BindInt("@poolId", poolId);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) await component.ExecuteReaderAsync(), component.ProcedureName, component.RequestContext))
        {
          resultCollection.AddBinder<TaskAgentJobRequest>((ObjectBinder<TaskAgentJobRequest>) new TaskAgentRequestBinder3());
          items = (IList<TaskAgentJobRequest>) resultCollection.GetCurrent<TaskAgentJobRequest>().Items;
        }
      }
      return items;
    }

    public override async Task<FinishAgentRequestResult> FinishAgentRequestAsync(
      int poolId,
      long requestId,
      bool deprovisionAgent,
      bool transitionAgentState,
      TaskResult? jobResult = null,
      bool agentShuttingDown = false)
    {
      TaskResourceComponent4 component = this;
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

    public override async Task<IList<TaskAgentPoolData>> GetAgentPoolsWithCapabilityChangesAsync()
    {
      TaskResourceComponent4 component = this;
      IList<TaskAgentPoolData> items;
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) component, nameof (GetAgentPoolsWithCapabilityChangesAsync)))
      {
        component.PrepareStoredProcedure("Task.prc_GetAgentPoolsWithCapabilityChanges");
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) await component.ExecuteReaderAsync(), component.ProcedureName, component.RequestContext))
        {
          resultCollection.AddBinder<TaskAgentPoolData>((ObjectBinder<TaskAgentPoolData>) new TaskAgentPoolBinder(component.RequestContext));
          items = (IList<TaskAgentPoolData>) resultCollection.GetCurrent<TaskAgentPoolData>().Items;
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
      TaskResourceComponent4 component = this;
      TaskAgentJobRequest taskAgentJobRequest;
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) component, nameof (QueueAgentRequestAsync)))
      {
        component.PrepareStoredProcedure("prc_QueueTaskAgentJobRequest");
        component.BindInt("@poolId", poolId);
        component.BindInt32Table("@matchedAgentIds", (IEnumerable<int>) request.MatchedAgents.Select<TaskAgentReference, int>((System.Func<TaskAgentReference, int>) (x => x.Id)).Distinct<int>().ToList<int>());
        component.BindGuid("@hostId", request.HostId);
        component.BindGuid("@planId", request.PlanId);
        component.BindGuid("@jobId", request.JobId);
        component.BindBinary("@demands", JsonUtility.Serialize((object) request.Demands, false), int.MaxValue, SqlDbType.VarBinary);
        component.BindString("@orchestrationId", string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0:N}_{1:N}", (object) request.PlanId, (object) request.JobId), 256, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) await component.ExecuteReaderAsync(), component.ProcedureName, component.RequestContext))
        {
          resultCollection.AddBinder<TaskAgentJobRequest>((ObjectBinder<TaskAgentJobRequest>) new TaskAgentRequestBinder3());
          taskAgentJobRequest = resultCollection.GetCurrent<TaskAgentJobRequest>().FirstOrDefault<TaskAgentJobRequest>();
        }
      }
      return taskAgentJobRequest;
    }

    public override async Task ResetAgentPoolCapabilityChangesAsync(IEnumerable<int> poolIds)
    {
      TaskResourceComponent4 component = this;
      TaskSqlComponentBase.SqlMethodScope sqlMethodScope = new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) component, nameof (ResetAgentPoolCapabilityChangesAsync));
      try
      {
        component.PrepareStoredProcedure("Task.prc_ResetAgentPoolCapabilityChanges");
        component.BindInt32Table("@poolIds", poolIds);
        int num = await component.ExecuteNonQueryAsync();
      }
      finally
      {
        sqlMethodScope.Dispose();
      }
      sqlMethodScope = new TaskSqlComponentBase.SqlMethodScope();
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

    public override UpdateAgentResult UpdateAgent(
      int poolId,
      TaskAgent agent,
      TaskAgentCapabilityType capabilityUpdate)
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (UpdateAgent)))
      {
        this.PrepareStoredProcedure("Task.prc_UpdateAgent");
        this.BindInt("@poolId", poolId);
        this.BindInt("@agentId", agent.Id);
        this.BindString("@name", agent.Name, 128, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
        this.BindNullableInt("@maxParallelism", agent.MaxParallelism);
        this.BindByte("@capabilityUpdate", (byte) capabilityUpdate);
        this.BindKeyValuePairStringTable("@systemCapabilities", (IEnumerable<KeyValuePair<string, string>>) agent.SystemCapabilities);
        this.BindKeyValuePairStringTable("@userCapabilities", (IEnumerable<KeyValuePair<string, string>>) agent.UserCapabilities);
        UpdateAgentResult updateAgentResult = new UpdateAgentResult();
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<bool>((ObjectBinder<bool>) new BooleanBinder());
          resultCollection.AddBinder<TaskAgentPoolData>((ObjectBinder<TaskAgentPoolData>) new TaskAgentPoolBinder(this.RequestContext));
          resultCollection.AddBinder<TaskAgent>((ObjectBinder<TaskAgent>) new TaskAgentBinder2());
          updateAgentResult.RecalculateRequestMatches = resultCollection.GetCurrent<bool>().First<bool>();
          resultCollection.NextResult();
          updateAgentResult.PoolData = resultCollection.GetCurrent<TaskAgentPoolData>().FirstOrDefault<TaskAgentPoolData>();
          resultCollection.NextResult();
          updateAgentResult.Agent = resultCollection.GetCurrent<TaskAgent>().First<TaskAgent>();
          foreach (KeyValuePair<string, string> systemCapability in (IEnumerable<KeyValuePair<string, string>>) agent.SystemCapabilities)
            updateAgentResult.Agent.SystemCapabilities.Add(systemCapability.Key, systemCapability.Value);
          foreach (KeyValuePair<string, string> userCapability in (IEnumerable<KeyValuePair<string, string>>) agent.UserCapabilities)
            updateAgentResult.Agent.UserCapabilities.Add(userCapability.Key, userCapability.Value);
          return updateAgentResult;
        }
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
          resultCollection.AddBinder<TaskAgentJobRequest>((ObjectBinder<TaskAgentJobRequest>) new TaskAgentRequestBinder3());
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
      TaskResourceComponent4 component = this;
      TaskAgentJobRequest taskAgentJobRequest;
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) component, nameof (UpdateAgentRequestMatchesAsync)))
      {
        component.PrepareStoredProcedure("Task.prc_UpdateAgentJobRequestMatches");
        component.BindInt("@poolId", poolId);
        component.BindLong("@requestId", requestId);
        component.BindInt32Table("@matchedAgentIds", (IEnumerable<int>) matchingAgents);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) await component.ExecuteReaderAsync(), component.ProcedureName, component.RequestContext))
        {
          resultCollection.AddBinder<TaskAgentJobRequest>((ObjectBinder<TaskAgentJobRequest>) new TaskAgentRequestBinder3());
          taskAgentJobRequest = resultCollection.GetCurrent<TaskAgentJobRequest>().FirstOrDefault<TaskAgentJobRequest>();
        }
      }
      return taskAgentJobRequest;
    }
  }
}
