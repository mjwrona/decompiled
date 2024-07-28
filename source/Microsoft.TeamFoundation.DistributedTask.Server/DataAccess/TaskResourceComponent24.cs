// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.DataAccess.TaskResourceComponent24
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.DistributedTask.Server.DataAccess
{
  internal class TaskResourceComponent24 : TaskResourceComponent22
  {
    public override async Task<FinishAgentRequestResult> FinishAgentRequestAsync(
      int poolId,
      long requestId,
      bool deprovisionAgent,
      bool transitionAgentState,
      TaskResult? jobResult = null,
      bool agentShuttingDown = false)
    {
      TaskResourceComponent24 component = this;
      FinishAgentRequestResult agentRequestResult;
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) component, nameof (FinishAgentRequestAsync)))
      {
        component.PrepareStoredProcedure("Task.prc_FinishAgentRequest");
        component.BindInt("@poolId", poolId);
        component.BindLong("@requestId", requestId);
        component.BindInt("@leaseTimeoutInSeconds", (int) TimeSpan.FromMinutes(5.0).TotalSeconds);
        FinishAgentRequestResult result = new FinishAgentRequestResult();
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) await component.ExecuteReaderAsync(), component.ProcedureName, component.RequestContext))
        {
          resultCollection.AddBinder<TaskAgentJobRequest>((ObjectBinder<TaskAgentJobRequest>) new TaskAgentRequestBinder9());
          result.CompletedRequest = resultCollection.GetCurrent<TaskAgentJobRequest>().FirstOrDefault<TaskAgentJobRequest>();
          agentRequestResult = result;
        }
      }
      return agentRequestResult;
    }

    public override async Task<TaskAgentJobRequest> QueueAgentRequestAsync(
      int poolId,
      int? agentCloudId,
      TaskAgentJobRequest request,
      string requestResourceType)
    {
      TaskResourceComponent24 component = this;
      TaskAgentJobRequest taskAgentJobRequest;
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) component, nameof (QueueAgentRequestAsync)))
      {
        component.PrepareStoredProcedure("Task.prc_QueueAgentRequest");
        component.BindInt("@poolId", poolId);
        component.BindInt32Table("@matchedAgentIds", (IEnumerable<int>) request.MatchedAgents.Select<TaskAgentReference, int>((System.Func<TaskAgentReference, int>) (x => x.Id)).Distinct<int>().ToList<int>());
        component.BindGuid("@serviceOwner", request.ServiceOwner);
        component.BindGuid("@hostId", request.HostId);
        component.BindGuid("@scopeId", request.ScopeId);
        component.BindString("@planType", request.PlanType, 260, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
        component.BindGuid("@planId", request.PlanId);
        component.BindGuid("@jobId", request.JobId);
        component.BindString("@jobName", request.JobName, 512, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
        component.BindBinary("@demands", JsonUtility.Serialize((object) request.Demands, false), int.MaxValue, SqlDbType.VarBinary);
        TaskResourceComponent24 resourceComponent24_1 = component;
        TaskOrchestrationOwner definition = request.Definition;
        int? parameterValue1 = new int?(definition != null ? definition.Id : 0);
        resourceComponent24_1.BindNullableInt("@definitionId", parameterValue1);
        component.BindBinary("@definitionReference", JsonUtility.Serialize((object) request.Definition, false), int.MaxValue, SqlDbType.VarBinary);
        TaskResourceComponent24 resourceComponent24_2 = component;
        TaskOrchestrationOwner owner = request.Owner;
        int? parameterValue2 = new int?(owner != null ? owner.Id : 0);
        resourceComponent24_2.BindNullableInt("@ownerId", parameterValue2);
        component.BindBinary("@ownerReference", JsonUtility.Serialize((object) request.Owner, false), int.MaxValue, SqlDbType.VarBinary);
        component.BindInt("@leaseTimeoutInSeconds", (int) TimeSpan.FromMinutes(5.0).TotalSeconds);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) await component.ExecuteReaderAsync(), component.ProcedureName, component.RequestContext))
        {
          resultCollection.AddBinder<TaskAgentJobRequest>((ObjectBinder<TaskAgentJobRequest>) new TaskAgentRequestBinder9());
          resultCollection.AddBinder<MatchedAgent>((ObjectBinder<MatchedAgent>) new MatchedAgentBinder2());
          request = resultCollection.GetCurrent<TaskAgentJobRequest>().FirstOrDefault<TaskAgentJobRequest>();
          if (request != null)
          {
            resultCollection.NextResult();
            foreach (MatchedAgent matchedAgent in resultCollection.GetCurrent<MatchedAgent>())
              request.MatchedAgents.Add(matchedAgent.Agent);
          }
          taskAgentJobRequest = request;
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
        AgentConnectivityResult connectivityResult = new AgentConnectivityResult();
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<bool>((ObjectBinder<bool>) new BooleanBinder());
          resultCollection.AddBinder<TaskAgentJobRequest>((ObjectBinder<TaskAgentJobRequest>) new TaskAgentRequestBinder9());
          resultCollection.AddBinder<TaskAgent>((ObjectBinder<TaskAgent>) new TaskAgentBinder5());
          resultCollection.AddBinder<TaskAgentCapability>((ObjectBinder<TaskAgentCapability>) new TaskAgentCapabilityBinder());
          connectivityResult.HandledEvent = resultCollection.GetCurrent<bool>().First<bool>();
          resultCollection.NextResult();
          resultCollection.NextResult();
          connectivityResult.Agent = resultCollection.GetCurrent<TaskAgent>().FirstOrDefault<TaskAgent>();
          if (connectivityResult.Agent != null)
          {
            if (resultCollection.TryNextResult())
            {
              connectivityResult.Agent = connectivityResult.Agent.PopulateImplicitCapabilities();
              foreach (TaskAgentCapability taskAgentCapability in resultCollection.GetCurrent<TaskAgentCapability>().Items.Where<TaskAgentCapability>((System.Func<TaskAgentCapability, bool>) (x => !x.IsWellKnownCapability())))
              {
                switch (taskAgentCapability.CapabilityType)
                {
                  case TaskAgentCapabilityType.System:
                    connectivityResult.Agent.SystemCapabilities.Add(taskAgentCapability.Name, taskAgentCapability.Value);
                    continue;
                  case TaskAgentCapabilityType.User:
                    connectivityResult.Agent.UserCapabilities.Add(taskAgentCapability.Name, taskAgentCapability.Value);
                    continue;
                  default:
                    continue;
                }
              }
            }
          }
        }
        connectivityResult.PoolData = this.GetTaskAgentPoolData(poolId);
        return connectivityResult;
      }
    }

    public override UpdateAgentRequestResult UpdateAgentRequest(
      int poolId,
      long requestId,
      TimeSpan leaseTimeout,
      DateTime? expirationTime = null,
      DateTime? startTime = null,
      DateTime? finishTime = null,
      TaskResult? result = null)
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (UpdateAgentRequest)))
      {
        this.PrepareStoredProcedure("Task.prc_UpdateAgentRequest");
        this.BindInt("@poolId", poolId);
        this.BindLong("@requestId", requestId);
        this.BindInt("@leaseTimeoutInSeconds", (int) leaseTimeout.TotalSeconds);
        if (expirationTime.HasValue)
          this.BindDateTime2("@expirationTime", expirationTime.Value);
        if (startTime.HasValue)
          this.BindDateTime2("@startTime", startTime.Value);
        if (finishTime.HasValue)
          this.BindDateTime2("@finishTime", finishTime.Value);
        if (result.HasValue)
          this.BindByte("@result", (byte) result.Value);
        UpdateAgentRequestResult agentRequestResult = new UpdateAgentRequestResult();
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<TaskAgentJobRequest>((ObjectBinder<TaskAgentJobRequest>) new TaskAgentRequestBinder9());
          resultCollection.AddBinder<TaskAgentJobRequest>((ObjectBinder<TaskAgentJobRequest>) new TaskAgentRequestBinder9());
          agentRequestResult.Before = resultCollection.GetCurrent<TaskAgentJobRequest>().FirstOrDefault<TaskAgentJobRequest>();
          resultCollection.NextResult();
          agentRequestResult.After = resultCollection.GetCurrent<TaskAgentJobRequest>().FirstOrDefault<TaskAgentJobRequest>();
          return agentRequestResult;
        }
      }
    }

    public override async Task<UpdateAgentRequestResult> UpdateAgentRequestAsync(
      int poolId,
      long requestId,
      TimeSpan leaseTimeout,
      DateTime? expirationTime = null,
      DateTime? startTime = null,
      DateTime? finishTime = null,
      TaskResult? result = null)
    {
      TaskResourceComponent24 component = this;
      UpdateAgentRequestResult agentRequestResult;
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) component, nameof (UpdateAgentRequestAsync)))
      {
        component.PrepareStoredProcedure("Task.prc_UpdateAgentRequest");
        component.BindInt("@poolId", poolId);
        component.BindLong("@requestId", requestId);
        component.BindInt("@leaseTimeoutInSeconds", (int) leaseTimeout.TotalSeconds);
        if (expirationTime.HasValue)
          component.BindDateTime2("@expirationTime", expirationTime.Value);
        if (startTime.HasValue)
          component.BindDateTime2("@startTime", startTime.Value);
        if (finishTime.HasValue)
          component.BindDateTime2("@finishTime", finishTime.Value);
        if (result.HasValue)
          component.BindByte("@result", (byte) result.Value);
        UpdateAgentRequestResult updateResult = new UpdateAgentRequestResult();
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) await component.ExecuteReaderAsync(), component.ProcedureName, component.RequestContext))
        {
          resultCollection.AddBinder<TaskAgentJobRequest>((ObjectBinder<TaskAgentJobRequest>) new TaskAgentRequestBinder9());
          resultCollection.AddBinder<TaskAgentJobRequest>((ObjectBinder<TaskAgentJobRequest>) new TaskAgentRequestBinder9());
          updateResult.Before = resultCollection.GetCurrent<TaskAgentJobRequest>().FirstOrDefault<TaskAgentJobRequest>();
          resultCollection.NextResult();
          updateResult.After = resultCollection.GetCurrent<TaskAgentJobRequest>().FirstOrDefault<TaskAgentJobRequest>();
          agentRequestResult = updateResult;
        }
      }
      return agentRequestResult;
    }

    public override async Task<TaskAgentJobRequest> UpdateAgentRequestMatchesAsync(
      int poolId,
      long requestId,
      IList<int> matchingAgents,
      bool matchesAllAgents)
    {
      TaskResourceComponent24 component = this;
      TaskAgentJobRequest taskAgentJobRequest;
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) component, nameof (UpdateAgentRequestMatchesAsync)))
      {
        component.PrepareStoredProcedure("Task.prc_UpdateAgentRequestMatches");
        component.BindInt("@poolId", poolId);
        component.BindLong("@requestId", requestId);
        component.BindInt32Table("@matchedAgentIds", (IEnumerable<int>) matchingAgents);
        component.BindInt("@leaseTimeoutInSeconds", (int) TimeSpan.FromMinutes(5.0).TotalSeconds);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) await component.ExecuteReaderAsync(), component.ProcedureName, component.RequestContext))
        {
          resultCollection.AddBinder<TaskAgentJobRequest>((ObjectBinder<TaskAgentJobRequest>) new TaskAgentRequestBinder9());
          taskAgentJobRequest = resultCollection.GetCurrent<TaskAgentJobRequest>().FirstOrDefault<TaskAgentJobRequest>();
        }
      }
      return taskAgentJobRequest;
    }

    protected override TaskAgentPoolData GetTaskAgentPoolData(int poolId)
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (GetTaskAgentPoolData)))
      {
        TaskAgentPoolCacheService service = this.RequestContext.GetService<TaskAgentPoolCacheService>();
        TaskAgentPoolData agentPool;
        if (!service.TryGetValue(this.RequestContext, poolId, out agentPool))
        {
          agentPool = this.GetAgentPool(poolId);
          if (agentPool != null)
            service.Set(this.RequestContext, agentPool.Pool.Id, agentPool.Clone());
        }
        return agentPool;
      }
    }
  }
}
