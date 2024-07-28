// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.DataAccess.TaskResourceComponent46
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.Pipelines;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.DistributedTask.Server.DataAccess
{
  internal class TaskResourceComponent46 : TaskResourceComponent45
  {
    protected override ObjectBinder<TaskAgentSessionData> CreateTaskAgentSessionBinder() => (ObjectBinder<TaskAgentSessionData>) new TaskAgentSessionDataBinder4();

    protected virtual ObjectBinder<TaskAgentCloud> CreateTaskAgentCloudBinder() => (ObjectBinder<TaskAgentCloud>) new TaskAgentCloudBinder();

    protected virtual ObjectBinder<TaskAgentCloudRequest> CreateTaskAgentCloudRequestBinder() => (ObjectBinder<TaskAgentCloudRequest>) new TaskAgentCloudRequestBinder();

    protected ObjectBinder<MatchedAgent> CreateMatchedAgentBinder() => (ObjectBinder<MatchedAgent>) new MatchedAgentBinder3();

    protected override ObjectBinder<TaskAgent> CreateTaskAgentBinder() => (ObjectBinder<TaskAgent>) new TaskAgentBinder9();

    protected override ObjectBinder<TaskAgentJobRequest> CreateTaskAgentRequestBinder(int poolId) => (ObjectBinder<TaskAgentJobRequest>) new TaskAgentRequestBinder13();

    protected override ObjectBinder<TaskAgentJobRequest> CreateTaskAgentRequestBinder() => (ObjectBinder<TaskAgentJobRequest>) new TaskAgentRequestBinder13();

    public override CreateAgentResult AddAgent(int poolId, TaskAgent agent, bool createEnabled = true)
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (AddAgent)))
      {
        this.PrepareStoredProcedure("Task.prc_AddAgent");
        this.BindInt("@poolId", poolId);
        this.BindString("@name", agent.Name, 128, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
        this.BindString("@version", agent.Version, 128, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
        this.BindString("@osDescription", agent.OSDescription, 512, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
        this.BindString("@provisioningState", agent.ProvisioningState, 64, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
        this.BindString("@accessPoint", agent.AccessPoint, 512, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
        this.BindNullableInt("@maxParallelism", agent.MaxParallelism);
        this.BindGuid("@writerId", this.Author);
        if (agent.Authorization != null)
          this.BindBinary("@publicKey", JsonUtility.Serialize((object) agent.Authorization.PublicKey, false), 512, SqlDbType.VarBinary);
        else
          this.BindNullValue("@publicKey", SqlDbType.VarBinary);
        if (agent.SystemCapabilities.Count > 0)
        {
          agent.SystemCapabilities.Remove(PipelineConstants.AgentName);
          agent.SystemCapabilities.Remove(PipelineConstants.AgentVersionDemandName);
        }
        this.BindKeyValuePairStringTable("@systemCapabilities", (IEnumerable<KeyValuePair<string, string>>) agent.SystemCapabilities);
        this.BindKeyValuePairStringTable("@userCapabilities", (IEnumerable<KeyValuePair<string, string>>) agent.UserCapabilities);
        CreateAgentResult createAgentResult = new CreateAgentResult();
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<TaskAgentPoolData>(this.CreateTaskAgentPoolBinder());
          resultCollection.AddBinder<TaskAgent>(this.CreateTaskAgentBinder());
          createAgentResult.PoolData = resultCollection.GetCurrent<TaskAgentPoolData>().First<TaskAgentPoolData>();
          resultCollection.NextResult();
          createAgentResult.Agent = resultCollection.GetCurrent<TaskAgent>().First<TaskAgent>().PopulateImplicitCapabilities();
          foreach (KeyValuePair<string, string> systemCapability in (IEnumerable<KeyValuePair<string, string>>) agent.SystemCapabilities)
            createAgentResult.Agent.SystemCapabilities.Add(systemCapability.Key, systemCapability.Value);
          foreach (KeyValuePair<string, string> userCapability in (IEnumerable<KeyValuePair<string, string>>) agent.UserCapabilities)
            createAgentResult.Agent.UserCapabilities.Add(userCapability.Key, userCapability.Value);
          return createAgentResult;
        }
      }
    }

    public override TaskAgentPoolData AddAgentPool(
      string name,
      Guid createdBy,
      bool isHosted = false,
      bool autoProvision = false,
      bool? autoSize = true,
      TaskAgentPoolType poolType = TaskAgentPoolType.Automation,
      int? poolMetadataFileId = null,
      Guid? ownerId = null,
      int? agentCloudId = null,
      int? targetSize = null,
      bool? isLegacy = null,
      TaskAgentPoolOptions options = TaskAgentPoolOptions.None)
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (AddAgentPool)))
      {
        this.PrepareStoredProcedure("Task.prc_AddAgentPool");
        this.BindString("@name", name, 128, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
        this.BindByte("@poolType", (byte) poolType);
        this.BindGuid("@createdBy", createdBy);
        this.BindBoolean("@isHosted", isHosted);
        this.BindBoolean("@autoProvision", autoProvision);
        this.BindNullableInt("@poolMetadataFileId", poolMetadataFileId);
        this.BindNullableGuid("@ownerId", ownerId);
        this.BindNullableInt("@agentCloudId", agentCloudId);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<TaskAgentPoolData>(this.CreateTaskAgentPoolBinder());
          return resultCollection.GetCurrent<TaskAgentPoolData>().First<TaskAgentPoolData>();
        }
      }
    }

    public override async Task<TaskAgentCloud> AddAgentCloudAsync(
      Guid id,
      string name,
      string type,
      string getAgentDefinitionEndpoint,
      string acquireAgentEndpoint,
      string getAgentRequestStatusEndpoint,
      string releaseAgentEndpoint,
      string getAccountParallelismEndpoint,
      int? maxParallelism,
      int? acquisitionTimeout,
      bool internalAgentCloud = false)
    {
      TaskResourceComponent46 component = this;
      TaskAgentCloud taskAgentCloud;
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) component, nameof (AddAgentCloudAsync)))
      {
        component.PrepareStoredProcedure("Task.prc_AddAgentCloud");
        component.BindString("@name", name, 128, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
        component.BindString("@getAgentDefinitionEndpoint", getAgentDefinitionEndpoint, 128, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
        component.BindString("@acquireAgentEndpoint", acquireAgentEndpoint, 128, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
        component.BindString("@getAgentRequestStatusEndpoint", getAgentRequestStatusEndpoint, 128, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
        component.BindString("@releaseAgentEndpoint", releaseAgentEndpoint, 128, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) await component.ExecuteReaderAsync(), component.ProcedureName, component.RequestContext))
        {
          resultCollection.AddBinder<TaskAgentCloud>(component.CreateTaskAgentCloudBinder());
          taskAgentCloud = resultCollection.GetCurrent<TaskAgentCloud>().First<TaskAgentCloud>();
        }
      }
      return taskAgentCloud;
    }

    public override async Task<TaskAgentCloudRequest> AddAgentCloudRequestAsync(
      int agentCloudId,
      Guid requestIdentifier,
      int poolId,
      int agentId,
      JObject agentSpecification)
    {
      TaskResourceComponent46 component = this;
      TaskAgentCloudRequest agentCloudRequest;
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) component, nameof (AddAgentCloudRequestAsync)))
      {
        component.PrepareStoredProcedure("Task.prc_AddAgentCloudRequest");
        component.BindInt("@agentCloudId", agentCloudId);
        component.BindGuid("@requestId", requestIdentifier);
        component.BindInt("@poolId", poolId);
        component.BindInt("@agentId", agentId);
        component.BindBinary("@agentSpecification", JsonUtility.Serialize((object) agentSpecification, false), SqlDbType.VarBinary);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) await component.ExecuteReaderAsync(), component.ProcedureName, component.RequestContext))
        {
          resultCollection.AddBinder<TaskAgentCloudRequest>(component.CreateTaskAgentCloudRequestBinder());
          agentCloudRequest = resultCollection.GetCurrent<TaskAgentCloudRequest>().First<TaskAgentCloudRequest>();
        }
      }
      return agentCloudRequest;
    }

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
        this.BindString("@agentVersion", agent.Version, 128, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
        this.BindString("@osDescription", agent.OSDescription, 512, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
        this.BindString("@accessPoint", agent.AccessPoint, 512, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
        this.BindString("@ownerName", ownerName, 512, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
        this.BindGuid("@writerId", this.Author);
        if (systemCapabilities != null && systemCapabilities.Count > 0)
        {
          systemCapabilities.Remove(PipelineConstants.AgentName);
          systemCapabilities.Remove(PipelineConstants.AgentVersionDemandName);
        }
        this.BindKeyValuePairStringTable("@systemCapabilities", (IEnumerable<KeyValuePair<string, string>>) systemCapabilities);
        CreateAgentSessionResult agentSession = new CreateAgentSessionResult();
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<bool>((ObjectBinder<bool>) new BooleanBinder());
          resultCollection.AddBinder<TaskAgent>(this.CreateTaskAgentBinder());
          resultCollection.AddBinder<TaskAgentSessionData>(this.CreateTaskAgentSessionBinder());
          resultCollection.AddBinder<TaskAgentSessionData>(this.CreateTaskAgentSessionBinder());
          resultCollection.AddBinder<TaskAgentJobRequest>(this.CreateTaskAgentRequestBinder());
          agentSession.RecalculateRequestMatches = resultCollection.GetCurrent<bool>().First<bool>();
          resultCollection.NextResult();
          agentSession.Agent = resultCollection.GetCurrent<TaskAgent>().FirstOrDefault<TaskAgent>();
          resultCollection.NextResult();
          agentSession.OldSession = resultCollection.GetCurrent<TaskAgentSessionData>().FirstOrDefault<TaskAgentSessionData>();
          resultCollection.NextResult();
          agentSession.NewSession = resultCollection.GetCurrent<TaskAgentSessionData>().FirstOrDefault<TaskAgentSessionData>();
          if (resultCollection.TryNextResult())
            agentSession.AssignedRequest = resultCollection.GetCurrent<TaskAgentJobRequest>().FirstOrDefault<TaskAgentJobRequest>();
          return agentSession;
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
      TaskResourceComponent46 component = this;
      FinishAgentRequestResult agentRequestResult;
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) component, nameof (FinishAgentRequestAsync)))
      {
        component.PrepareStoredProcedure("Task.prc_FinishAgentRequest");
        component.BindInt("@poolId", poolId);
        component.BindLong("@requestId", requestId);
        component.BindInt("@leaseTimeoutInSeconds", (int) TimeSpan.FromMinutes(5.0).TotalSeconds);
        component.BindInt("@provisioningTimeoutInSeconds", (int) TimeSpan.FromMinutes(5.0).TotalSeconds);
        component.BindBoolean("@assignRequests", false);
        component.BindBoolean("@deprovisionAgent", deprovisionAgent);
        if (jobResult.HasValue)
          component.BindInt("@result", (int) (byte) jobResult.Value);
        FinishAgentRequestResult result = new FinishAgentRequestResult();
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) await component.ExecuteReaderAsync(), component.ProcedureName, component.RequestContext))
        {
          resultCollection.AddBinder<TaskAgentJobRequest>(component.CreateTaskAgentRequestBinder());
          result.CompletedRequest = resultCollection.GetCurrent<TaskAgentJobRequest>().FirstOrDefault<TaskAgentJobRequest>();
        }
        agentRequestResult = result;
      }
      return agentRequestResult;
    }

    public override TaskAgentCloud GetAgentCloudForPool(int poolId)
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (GetAgentCloudForPool)))
      {
        this.PrepareStoredProcedure("Task.prc_GetAgentCloudForPool");
        this.BindInt("@poolId", poolId);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<TaskAgentCloud>(this.CreateTaskAgentCloudBinder());
          return resultCollection.GetCurrent<TaskAgentCloud>().FirstOrDefault<TaskAgentCloud>();
        }
      }
    }

    public override async Task<TaskAgentCloud> GetAgentCloudForPoolAsync(int poolId)
    {
      TaskResourceComponent46 component = this;
      TaskAgentCloud cloudForPoolAsync;
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) component, nameof (GetAgentCloudForPoolAsync)))
      {
        component.PrepareStoredProcedure("Task.prc_GetAgentCloudForPool");
        component.BindInt("@poolId", poolId);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) await component.ExecuteReaderAsync(), component.ProcedureName, component.RequestContext))
        {
          resultCollection.AddBinder<TaskAgentCloud>(component.CreateTaskAgentCloudBinder());
          cloudForPoolAsync = resultCollection.GetCurrent<TaskAgentCloud>().FirstOrDefault<TaskAgentCloud>();
        }
      }
      return cloudForPoolAsync;
    }

    public override IList<TaskAgentCloud> GetAgentClouds(int? providerId)
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (GetAgentClouds)))
      {
        this.PrepareStoredProcedure("Task.prc_GetAgentClouds");
        this.BindNullableInt("@agentCloudId", providerId);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<TaskAgentCloud>(this.CreateTaskAgentCloudBinder());
          return (IList<TaskAgentCloud>) resultCollection.GetCurrent<TaskAgentCloud>().Items;
        }
      }
    }

    public override async Task<IList<TaskAgentCloud>> GetAgentCloudsAsync(int? providerId)
    {
      TaskResourceComponent46 component = this;
      IList<TaskAgentCloud> items;
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) component, nameof (GetAgentCloudsAsync)))
      {
        component.PrepareStoredProcedure("Task.prc_GetAgentClouds");
        component.BindNullableInt("@agentCloudId", providerId);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) await component.ExecuteReaderAsync(), component.ProcedureName, component.RequestContext))
        {
          resultCollection.AddBinder<TaskAgentCloud>(component.CreateTaskAgentCloudBinder());
          items = (IList<TaskAgentCloud>) resultCollection.GetCurrent<TaskAgentCloud>().Items;
        }
      }
      return items;
    }

    public override async Task<TaskAgentCloudRequest> GetAgentCloudRequestForAgentAsync(
      int poolId,
      int agentId)
    {
      TaskResourceComponent46 component = this;
      TaskAgentCloudRequest requestForAgentAsync;
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) component, nameof (GetAgentCloudRequestForAgentAsync)))
      {
        component.PrepareStoredProcedure("Task.prc_GetAgentCloudRequestForAgent");
        component.BindInt("@poolId", poolId);
        component.BindInt("@agentId", agentId);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) await component.ExecuteReaderAsync(), component.ProcedureName, component.RequestContext))
        {
          resultCollection.AddBinder<TaskAgentCloudRequest>(component.CreateTaskAgentCloudRequestBinder());
          requestForAgentAsync = resultCollection.GetCurrent<TaskAgentCloudRequest>().FirstOrDefault<TaskAgentCloudRequest>();
        }
      }
      return requestForAgentAsync;
    }

    public override async Task<IList<TaskAgentCloudRequest>> GetAgentCloudRequestsAsync(
      int? agentCloudId)
    {
      TaskResourceComponent46 component = this;
      IList<TaskAgentCloudRequest> items;
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) component, nameof (GetAgentCloudRequestsAsync)))
      {
        component.PrepareStoredProcedure("Task.prc_GetAgentCloudRequests");
        component.BindNullableInt("@agentCloudId", agentCloudId);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) await component.ExecuteReaderAsync(), component.ProcedureName, component.RequestContext))
        {
          resultCollection.AddBinder<TaskAgentCloudRequest>(component.CreateTaskAgentCloudRequestBinder());
          items = (IList<TaskAgentCloudRequest>) resultCollection.GetCurrent<TaskAgentCloudRequest>().Items;
        }
      }
      return items;
    }

    public override TaskAgentRequestData GetAgentRequest(
      int poolId,
      long requestId,
      bool includeStatus = false)
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (GetAgentRequest)))
      {
        TaskAgentRequestData agentRequest1 = (TaskAgentRequestData) null;
        this.PrepareStoredProcedure("Task.prc_GetAgentRequest");
        this.BindInt("@poolId", poolId);
        this.BindLong("@requestId", requestId);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<TaskAgentJobRequest>(this.CreateTaskAgentRequestBinder());
          resultCollection.AddBinder<MatchedAgent>(this.CreateMatchedAgentBinder());
          TaskAgentJobRequest agentRequest2 = resultCollection.GetCurrent<TaskAgentJobRequest>().FirstOrDefault<TaskAgentJobRequest>();
          if (agentRequest2 != null)
          {
            agentRequest1 = new TaskAgentRequestData(agentRequest2);
            resultCollection.NextResult();
            foreach (MatchedAgent matchedAgent in resultCollection.GetCurrent<MatchedAgent>())
              agentRequest1.AgentRequest.MatchedAgents.Add(matchedAgent.Agent);
          }
        }
        return agentRequest1;
      }
    }

    public override IList<TaskAgentJobRequest> GetAgentRequestsForAgents(
      int poolId,
      IList<int> agentIds,
      Guid hostId,
      Guid scopeId,
      int completedRequestCount,
      int? ownerId = null,
      DateTime? completedOn = null)
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (GetAgentRequestsForAgents)))
      {
        this.PrepareStoredProcedure("Task.prc_GetAgentRequestsByFilter");
        this.BindInt("@poolId", poolId);
        this.BindInt32Table("@agentIds", (IEnumerable<int>) agentIds);
        this.BindGuid("@hostId", hostId);
        this.BindGuid("@scopeId", scopeId);
        this.BindInt("@completedRequestCount", completedRequestCount);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<TaskAgentJobRequest>(this.CreateTaskAgentRequestBinder());
          resultCollection.AddBinder<MatchedAgent>(this.CreateMatchedAgentBinder());
          resultCollection.AddBinder<TaskAgentJobRequest>(this.CreateTaskAgentRequestBinder());
          resultCollection.AddBinder<TaskAgentJobRequest>(this.CreateTaskAgentRequestBinder());
          List<TaskAgentJobRequest> items = resultCollection.GetCurrent<TaskAgentJobRequest>().Items;
          resultCollection.NextResult();
          if (items.Count > 0)
          {
            Dictionary<long, TaskAgentJobRequest> dictionary = items.ToDictionary<TaskAgentJobRequest, long>((System.Func<TaskAgentJobRequest, long>) (x => x.RequestId));
            foreach (MatchedAgent matchedAgent in resultCollection.GetCurrent<MatchedAgent>())
            {
              TaskAgentJobRequest taskAgentJobRequest;
              if (dictionary.TryGetValue(matchedAgent.RequestId, out taskAgentJobRequest))
                taskAgentJobRequest.MatchedAgents.Add(matchedAgent.Agent);
            }
          }
          resultCollection.NextResult();
          items.AddRange((IEnumerable<TaskAgentJobRequest>) resultCollection.GetCurrent<TaskAgentJobRequest>().Items);
          resultCollection.NextResult();
          items.AddRange((IEnumerable<TaskAgentJobRequest>) resultCollection.GetCurrent<TaskAgentJobRequest>().Items);
          return (IList<TaskAgentJobRequest>) items;
        }
      }
    }

    public override async Task<IList<TaskAgentJobRequest>> GetAgentRequestsForAgentsAsync(
      int poolId,
      IList<int> agentIds,
      int completedRequestCount)
    {
      TaskResourceComponent46 component = this;
      IList<TaskAgentJobRequest> requestsForAgentsAsync;
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) component, nameof (GetAgentRequestsForAgentsAsync)))
      {
        component.PrepareStoredProcedure("Task.prc_GetAgentRequests");
        component.BindInt("@poolId", poolId);
        component.BindInt32Table("@agentIds", (IEnumerable<int>) agentIds);
        component.BindInt("@completedRequestCount", completedRequestCount);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) await component.ExecuteReaderAsync(), component.ProcedureName, component.RequestContext))
        {
          resultCollection.AddBinder<TaskAgentJobRequest>(component.CreateTaskAgentRequestBinder());
          resultCollection.AddBinder<MatchedAgent>(component.CreateMatchedAgentBinder());
          resultCollection.AddBinder<TaskAgentJobRequest>(component.CreateTaskAgentRequestBinder());
          resultCollection.AddBinder<TaskAgentJobRequest>(component.CreateTaskAgentRequestBinder());
          List<TaskAgentJobRequest> items = resultCollection.GetCurrent<TaskAgentJobRequest>().Items;
          resultCollection.NextResult();
          if (items.Count > 0)
          {
            Dictionary<long, TaskAgentJobRequest> dictionary = items.ToDictionary<TaskAgentJobRequest, long>((System.Func<TaskAgentJobRequest, long>) (x => x.RequestId));
            foreach (MatchedAgent matchedAgent in resultCollection.GetCurrent<MatchedAgent>())
            {
              TaskAgentJobRequest taskAgentJobRequest;
              if (dictionary.TryGetValue(matchedAgent.RequestId, out taskAgentJobRequest))
                taskAgentJobRequest.MatchedAgents.Add(matchedAgent.Agent);
            }
          }
          resultCollection.NextResult();
          items.AddRange((IEnumerable<TaskAgentJobRequest>) resultCollection.GetCurrent<TaskAgentJobRequest>().Items);
          resultCollection.NextResult();
          items.AddRange((IEnumerable<TaskAgentJobRequest>) resultCollection.GetCurrent<TaskAgentJobRequest>().Items);
          requestsForAgentsAsync = (IList<TaskAgentJobRequest>) items;
        }
      }
      return requestsForAgentsAsync;
    }

    public override IList<TaskAgentJobRequest> GetAgentRequestsForPlan(
      int poolId,
      Guid planId,
      Guid? jobId)
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (GetAgentRequestsForPlan)))
      {
        this.PrepareStoredProcedure("Task.prc_GetAgentRequestsForPlan");
        this.BindInt("@poolId", poolId);
        this.BindGuid("@planId", planId);
        this.BindNullableGuid("@jobId", jobId);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<TaskAgentJobRequest>(this.CreateTaskAgentRequestBinder());
          resultCollection.AddBinder<MatchedAgent>(this.CreateMatchedAgentBinder());
          List<TaskAgentJobRequest> items = resultCollection.GetCurrent<TaskAgentJobRequest>().Items;
          resultCollection.NextResult();
          if (items.Count > 0)
          {
            Dictionary<long, TaskAgentJobRequest> dictionary = items.ToDictionary<TaskAgentJobRequest, long>((System.Func<TaskAgentJobRequest, long>) (x => x.RequestId));
            foreach (MatchedAgent matchedAgent in resultCollection.GetCurrent<MatchedAgent>())
            {
              TaskAgentJobRequest taskAgentJobRequest;
              if (dictionary.TryGetValue(matchedAgent.RequestId, out taskAgentJobRequest))
                taskAgentJobRequest.MatchedAgents.Add(matchedAgent.Agent);
            }
          }
          return (IList<TaskAgentJobRequest>) items;
        }
      }
    }

    public override async Task<TaskAgentJobRequest> QueueAgentRequestAsync(
      int poolId,
      int? agentCloudId,
      TaskAgentJobRequest request,
      string requestResourceType)
    {
      TaskResourceComponent46 component = this;
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
        TaskResourceComponent46 resourceComponent46_1 = component;
        TaskOrchestrationOwner definition = request.Definition;
        int? parameterValue1 = new int?(definition != null ? definition.Id : 0);
        resourceComponent46_1.BindNullableInt("@definitionId", parameterValue1);
        component.BindBinary("@definitionReference", JsonUtility.Serialize((object) request.Definition, false), int.MaxValue, SqlDbType.VarBinary);
        TaskResourceComponent46 resourceComponent46_2 = component;
        TaskOrchestrationOwner owner = request.Owner;
        int? parameterValue2 = new int?(owner != null ? owner.Id : 0);
        resourceComponent46_2.BindNullableInt("@ownerId", parameterValue2);
        component.BindBinary("@ownerReference", JsonUtility.Serialize((object) request.Owner, false), int.MaxValue, SqlDbType.VarBinary);
        TaskResourceComponent46 resourceComponent46_3 = component;
        TimeSpan timeSpan = TimeSpan.FromMinutes(5.0);
        int totalSeconds1 = (int) timeSpan.TotalSeconds;
        resourceComponent46_3.BindInt("@leaseTimeoutInSeconds", totalSeconds1);
        TaskResourceComponent46 resourceComponent46_4 = component;
        timeSpan = TimeSpan.FromMinutes(5.0);
        int totalSeconds2 = (int) timeSpan.TotalSeconds;
        resourceComponent46_4.BindInt("@provisioningTimeoutInSeconds", totalSeconds2);
        component.BindBinary("@data", JsonUtility.Serialize((object) request.Data, false), 1024, SqlDbType.VarBinary);
        component.BindString("@resourceType", requestResourceType, 160, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
        component.BindBoolean("@assignRequests", false);
        component.BindNullableInt("@queueId", request.QueueId);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) await component.ExecuteReaderAsync(), component.ProcedureName, component.RequestContext))
        {
          resultCollection.AddBinder<TaskAgentJobRequest>(component.CreateTaskAgentRequestBinder());
          resultCollection.AddBinder<MatchedAgent>(component.CreateMatchedAgentBinder());
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

    public override AgentConnectivityResult SetAgentOffline(
      int poolId,
      int agentId,
      int sequenceId = 0,
      bool force = false)
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (SetAgentOffline)))
      {
        this.PrepareStoredProcedure("Task.prc_SetAgentOffline");
        this.BindInt("@poolId", poolId);
        this.BindInt("@agentId", agentId);
        this.BindInt("@sequenceId", sequenceId);
        this.BindBoolean("@force", force);
        bool flag = false;
        TaskAgent agent = (TaskAgent) null;
        TaskAgentPoolData taskAgentPoolData = (TaskAgentPoolData) null;
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<bool>((ObjectBinder<bool>) new BooleanBinder());
          resultCollection.AddBinder<TaskAgentPoolData>(this.CreateTaskAgentPoolBinder());
          resultCollection.AddBinder<TaskAgent>(this.CreateTaskAgentBinder());
          resultCollection.AddBinder<TaskAgentCapability>((ObjectBinder<TaskAgentCapability>) new TaskAgentCapabilityBinder());
          flag = resultCollection.GetCurrent<bool>().First<bool>();
          resultCollection.NextResult();
          taskAgentPoolData = resultCollection.GetCurrent<TaskAgentPoolData>().FirstOrDefault<TaskAgentPoolData>();
          resultCollection.NextResult();
          agent = resultCollection.GetCurrent<TaskAgent>().FirstOrDefault<TaskAgent>();
          if (agent != null)
          {
            if (resultCollection.TryNextResult())
            {
              agent = agent.PopulateImplicitCapabilities();
              foreach (TaskAgentCapability taskAgentCapability in resultCollection.GetCurrent<TaskAgentCapability>().Items.Where<TaskAgentCapability>((System.Func<TaskAgentCapability, bool>) (x => !x.IsWellKnownCapability())))
              {
                switch (taskAgentCapability.CapabilityType)
                {
                  case TaskAgentCapabilityType.System:
                    agent.SystemCapabilities.Add(taskAgentCapability.Name, taskAgentCapability.Value);
                    continue;
                  case TaskAgentCapabilityType.User:
                    agent.UserCapabilities.Add(taskAgentCapability.Name, taskAgentCapability.Value);
                    continue;
                  default:
                    continue;
                }
              }
            }
          }
        }
        AgentConnectivityResult connectivityResult = new AgentConnectivityResult();
        connectivityResult.Agent = agent;
        connectivityResult.HandledEvent = flag;
        connectivityResult.PoolData = taskAgentPoolData;
        connectivityResult = connectivityResult;
        return connectivityResult;
      }
    }

    public override AgentConnectivityResult SetAgentOnline(int poolId, int agentId, int sequenceId)
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (SetAgentOnline)))
      {
        this.PrepareStoredProcedure("Task.prc_SetAgentOnline");
        this.BindInt("@poolId", poolId);
        this.BindInt("@agentId", agentId);
        this.BindInt("@sequenceId", sequenceId);
        this.BindInt("@leaseTimeoutInSeconds", (int) TimeSpan.FromMinutes(5.0).TotalSeconds);
        AgentConnectivityResult connectivityResult = new AgentConnectivityResult();
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<bool>((ObjectBinder<bool>) new BooleanBinder());
          resultCollection.AddBinder<TaskAgentJobRequest>(this.CreateTaskAgentRequestBinder());
          resultCollection.AddBinder<TaskAgentPoolData>(this.CreateTaskAgentPoolBinder());
          resultCollection.AddBinder<TaskAgent>(this.CreateTaskAgentBinder());
          resultCollection.AddBinder<TaskAgentCapability>((ObjectBinder<TaskAgentCapability>) new TaskAgentCapabilityBinder());
          connectivityResult.HandledEvent = resultCollection.GetCurrent<bool>().First<bool>();
          resultCollection.NextResult();
          resultCollection.NextResult();
          connectivityResult.PoolData = resultCollection.GetCurrent<TaskAgentPoolData>().FirstOrDefault<TaskAgentPoolData>();
          resultCollection.NextResult();
          ref AgentConnectivityResult local = ref connectivityResult;
          TaskAgent agent = resultCollection.GetCurrent<TaskAgent>().FirstOrDefault<TaskAgent>();
          TaskAgent taskAgent = agent != null ? agent.PopulateImplicitCapabilities() : (TaskAgent) null;
          local.Agent = taskAgent;
          if (connectivityResult.Agent != null)
          {
            resultCollection.NextResult();
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
        return connectivityResult;
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
        this.BindString("@version", agent.Version, 128, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
        this.BindString("@osDescription", agent.OSDescription, 512, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
        this.BindString("@provisioningState", agent.ProvisioningState, 64, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
        this.BindString("@accessPoint", agent.AccessPoint, 512, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
        this.BindNullableBoolean("@enabled", agent.Enabled);
        this.BindNullableInt("@maxParallelism", agent.MaxParallelism);
        if (agent.Authorization != null)
          this.BindBinary("@publicKey", JsonUtility.Serialize((object) agent.Authorization.PublicKey, false), 512, SqlDbType.VarBinary);
        else
          this.BindNullValue("@publicKey", SqlDbType.VarBinary);
        this.BindByte("@capabilityUpdate", (byte) capabilityUpdate);
        if (agent.SystemCapabilities.Count > 0)
        {
          agent.SystemCapabilities.Remove(PipelineConstants.AgentName);
          agent.SystemCapabilities.Remove(PipelineConstants.AgentVersionDemandName);
        }
        if ((capabilityUpdate & TaskAgentCapabilityType.System) == TaskAgentCapabilityType.System)
          this.BindKeyValuePairStringTable("@systemCapabilities", (IEnumerable<KeyValuePair<string, string>>) agent.SystemCapabilities);
        else
          this.BindKeyValuePairStringTable("@systemCapabilities", (IEnumerable<KeyValuePair<string, string>>) new KeyValuePair<string, string>[0]);
        if ((capabilityUpdate & TaskAgentCapabilityType.User) == TaskAgentCapabilityType.User)
          this.BindKeyValuePairStringTable("@userCapabilities", (IEnumerable<KeyValuePair<string, string>>) agent.UserCapabilities);
        else
          this.BindKeyValuePairStringTable("@userCapabilities", (IEnumerable<KeyValuePair<string, string>>) new KeyValuePair<string, string>[0]);
        UpdateAgentResult updateAgentResult = new UpdateAgentResult();
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<bool>((ObjectBinder<bool>) new BooleanBinder());
          resultCollection.AddBinder<TaskAgentPoolData>(this.CreateTaskAgentPoolBinder());
          resultCollection.AddBinder<TaskAgent>(this.CreateTaskAgentBinder());
          updateAgentResult.RecalculateRequestMatches = resultCollection.GetCurrent<bool>().First<bool>();
          resultCollection.NextResult();
          updateAgentResult.PoolData = resultCollection.GetCurrent<TaskAgentPoolData>().First<TaskAgentPoolData>();
          resultCollection.NextResult();
          updateAgentResult.Agent = resultCollection.GetCurrent<TaskAgent>().First<TaskAgent>().PopulateImplicitCapabilities();
          foreach (KeyValuePair<string, string> systemCapability in (IEnumerable<KeyValuePair<string, string>>) agent.SystemCapabilities)
            updateAgentResult.Agent.SystemCapabilities.Add(systemCapability.Key, systemCapability.Value);
          foreach (KeyValuePair<string, string> userCapability in (IEnumerable<KeyValuePair<string, string>>) agent.UserCapabilities)
            updateAgentResult.Agent.UserCapabilities.Add(userCapability.Key, userCapability.Value);
          return updateAgentResult;
        }
      }
    }

    public override async Task<UpdateAgentResult> UpdateAgentAsync(
      int poolId,
      TaskAgent agent,
      TaskAgentCapabilityType capabilityUpdate)
    {
      TaskResourceComponent46 component = this;
      UpdateAgentResult updateAgentResult;
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) component, nameof (UpdateAgentAsync)))
      {
        component.PrepareStoredProcedure("Task.prc_UpdateAgent");
        component.BindInt("@poolId", poolId);
        component.BindInt("@agentId", agent.Id);
        component.BindString("@name", agent.Name, 128, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
        component.BindString("@version", agent.Version, 128, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
        component.BindString("@osDescription", agent.OSDescription, 512, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
        component.BindString("@provisioningState", agent.ProvisioningState, 64, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
        component.BindString("@accessPoint", agent.AccessPoint, 512, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
        component.BindNullableBoolean("@enabled", agent.Enabled);
        component.BindNullableInt("@maxParallelism", agent.MaxParallelism);
        if (agent.Authorization != null)
          component.BindBinary("@publicKey", JsonUtility.Serialize((object) agent.Authorization.PublicKey, false), 512, SqlDbType.VarBinary);
        else
          component.BindNullValue("@publicKey", SqlDbType.VarBinary);
        component.BindByte("@capabilityUpdate", (byte) capabilityUpdate);
        if (agent.SystemCapabilities.Count > 0)
        {
          agent.SystemCapabilities.Remove(PipelineConstants.AgentName);
          agent.SystemCapabilities.Remove(PipelineConstants.AgentVersionDemandName);
        }
        if ((capabilityUpdate & TaskAgentCapabilityType.System) == TaskAgentCapabilityType.System)
          component.BindKeyValuePairStringTable("@systemCapabilities", (IEnumerable<KeyValuePair<string, string>>) agent.SystemCapabilities);
        else
          component.BindKeyValuePairStringTable("@systemCapabilities", (IEnumerable<KeyValuePair<string, string>>) new KeyValuePair<string, string>[0]);
        if ((capabilityUpdate & TaskAgentCapabilityType.User) == TaskAgentCapabilityType.User)
          component.BindKeyValuePairStringTable("@userCapabilities", (IEnumerable<KeyValuePair<string, string>>) agent.UserCapabilities);
        else
          component.BindKeyValuePairStringTable("@userCapabilities", (IEnumerable<KeyValuePair<string, string>>) new KeyValuePair<string, string>[0]);
        UpdateAgentResult result = new UpdateAgentResult();
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) await component.ExecuteReaderAsync(), component.ProcedureName, component.RequestContext))
        {
          resultCollection.AddBinder<bool>((ObjectBinder<bool>) new BooleanBinder());
          resultCollection.AddBinder<TaskAgentPoolData>(component.CreateTaskAgentPoolBinder());
          resultCollection.AddBinder<TaskAgent>(component.CreateTaskAgentBinder());
          result.RecalculateRequestMatches = resultCollection.GetCurrent<bool>().First<bool>();
          resultCollection.NextResult();
          result.PoolData = resultCollection.GetCurrent<TaskAgentPoolData>().First<TaskAgentPoolData>();
          resultCollection.NextResult();
          result.Agent = resultCollection.GetCurrent<TaskAgent>().First<TaskAgent>().PopulateImplicitCapabilities();
          foreach (KeyValuePair<string, string> systemCapability in (IEnumerable<KeyValuePair<string, string>>) agent.SystemCapabilities)
            result.Agent.SystemCapabilities.Add(systemCapability.Key, systemCapability.Value);
          foreach (KeyValuePair<string, string> userCapability in (IEnumerable<KeyValuePair<string, string>>) agent.UserCapabilities)
            result.Agent.UserCapabilities.Add(userCapability.Key, userCapability.Value);
        }
        updateAgentResult = result;
      }
      return updateAgentResult;
    }

    public override async Task<TaskAgentCloudRequest> UpdateAgentCloudRequestAsync(
      int agentCloudId,
      Guid requestId,
      JObject agentSpecification,
      JObject agentData,
      DateTime? provisionRequestTime,
      DateTime? provisionedTime,
      DateTime? agentConnectedTime,
      DateTime? releaseRequestTime)
    {
      TaskResourceComponent46 component = this;
      TaskAgentCloudRequest agentCloudRequest;
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) component, nameof (UpdateAgentCloudRequestAsync)))
      {
        component.PrepareStoredProcedure("Task.prc_UpdateAgentCloudRequest");
        component.BindInt("@agentCloudId", agentCloudId);
        component.BindGuid("@requestId", requestId);
        component.BindBinary("@agentData", JsonUtility.Serialize((object) agentData, false), SqlDbType.VarBinary);
        component.BindNullableDateTime2("@provisionRequestTime", provisionRequestTime);
        component.BindNullableDateTime2("@provisionedTime", provisionedTime);
        component.BindNullableDateTime2("@agentConnectedTime", agentConnectedTime);
        component.BindNullableDateTime2("@releaseRequestTime", releaseRequestTime);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) await component.ExecuteReaderAsync(), component.ProcedureName, component.RequestContext))
        {
          resultCollection.AddBinder<TaskAgentCloudRequest>(component.CreateTaskAgentCloudRequestBinder());
          agentCloudRequest = resultCollection.GetCurrent<TaskAgentCloudRequest>().First<TaskAgentCloudRequest>();
        }
      }
      return agentCloudRequest;
    }

    public override async Task<TaskAgentJobRequest> UpdateAgentRequestMatchesAsync(
      int poolId,
      long requestId,
      IList<int> matchingAgents,
      bool matchesAllAgents)
    {
      TaskResourceComponent46 component = this;
      TaskAgentJobRequest taskAgentJobRequest;
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) component, nameof (UpdateAgentRequestMatchesAsync)))
      {
        component.PrepareStoredProcedure("Task.prc_UpdateAgentRequestMatches");
        component.BindInt("@poolId", poolId);
        component.BindLong("@requestId", requestId);
        component.BindInt32Table("@matchedAgentIds", (IEnumerable<int>) matchingAgents);
        component.BindInt("@leaseTimeoutInSeconds", (int) TimeSpan.FromMinutes(5.0).TotalSeconds);
        component.BindInt("@provisioningTimeoutInSeconds", (int) TimeSpan.FromMinutes(5.0).TotalSeconds);
        component.BindBoolean("@assignRequests", false);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) await component.ExecuteReaderAsync(), component.ProcedureName, component.RequestContext))
        {
          resultCollection.AddBinder<TaskAgentJobRequest>(component.CreateTaskAgentRequestBinder());
          taskAgentJobRequest = resultCollection.GetCurrent<TaskAgentJobRequest>().FirstOrDefault<TaskAgentJobRequest>();
        }
      }
      return taskAgentJobRequest;
    }
  }
}
