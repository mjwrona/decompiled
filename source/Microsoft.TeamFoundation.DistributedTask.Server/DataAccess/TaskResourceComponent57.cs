// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.DataAccess.TaskResourceComponent57
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.Pipelines;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.AuditLog;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.DistributedTask.Server.DataAccess
{
  internal class TaskResourceComponent57 : TaskResourceComponent56
  {
    protected override ObjectBinder<TaskAgentPoolData> CreateTaskAgentPoolBinder() => (ObjectBinder<TaskAgentPoolData>) new TaskAgentPoolBinder11(this.RequestContext);

    protected virtual ObjectBinder<DeprovisioningAgentResult> CreateDeprovisionedAgentResultBinder() => (ObjectBinder<DeprovisioningAgentResult>) new DeprovisionedAgentResultBinder();

    protected override ObjectBinder<TaskAgentCloudRequest> CreateTaskAgentCloudRequestBinder() => (ObjectBinder<TaskAgentCloudRequest>) new TaskAgentCloudRequestBinder3();

    protected override ObjectBinder<AssignedAgentRequestResult> CreateAssignedAgentRequestResultBinder() => (ObjectBinder<AssignedAgentRequestResult>) new AssignedAgentRequestResultBinder2();

    protected override ObjectBinder<TaskAgentJobRequest> CreateTaskAgentRequestBinder() => (ObjectBinder<TaskAgentJobRequest>) new TaskAgentRequestBinder15();

    protected override ObjectBinder<TaskAgentJobRequest> CreateTaskAgentRequestBinder(int poolId) => (ObjectBinder<TaskAgentJobRequest>) new TaskAgentRequestBinder15();

    public override CreateAgentResult AddAgent(int poolId, TaskAgent agent, bool createEnabled = true)
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (AddAgent)))
      {
        string agentAdded = AgentPoolAuditConstants.AgentAdded;
        IEnumerable<string> strings = (IEnumerable<string>) new string[2]
        {
          "@publicKey",
          "@systemCapabilities"
        };
        Guid projectId = new Guid();
        IEnumerable<string> excludeParameters = strings;
        this.PrepareForAuditingAction(agentAdded, projectId: projectId, excludeParameters: excludeParameters);
        this.PrepareStoredProcedure("Task.prc_AddAgent");
        this.BindInt("@poolId", poolId);
        this.BindString("@name", agent.Name, 128, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
        this.BindString("@version", agent.Version, 128, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
        this.BindString("@osDescription", agent.OSDescription, 512, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
        this.BindString("@provisioningState", agent.ProvisioningState, 64, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
        this.BindString("@accessPoint", agent.AccessPoint, 512, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
        this.BindNullableInt("@maxParallelism", agent.MaxParallelism);
        this.BindBoolean("@createEnabled", createEnabled);
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
        this.BindBoolean("@autoSize", !autoSize.HasValue || autoSize.Value);
        this.BindNullableInt("@poolMetadataFileId", poolMetadataFileId);
        this.BindNullableGuid("@ownerId", ownerId);
        this.BindNullableInt("@agentCloudId", agentCloudId);
        this.BindNullableInt("@targetSize", targetSize);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<TaskAgentPoolData>(this.CreateTaskAgentPoolBinder());
          return resultCollection.GetCurrent<TaskAgentPoolData>().First<TaskAgentPoolData>();
        }
      }
    }

    public override DeleteAgentResult DeleteAgents(int poolId, IEnumerable<int> agentIds)
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (DeleteAgents)))
      {
        if (agentIds.Count<int>() > 1)
          this.PrepareForAuditingAction(AgentPoolAuditConstants.AgentsDeleted);
        else
          this.PrepareForAuditingAction(AgentPoolAuditConstants.AgentDeleted);
        this.PrepareStoredProcedure("Task.prc_DeleteAgents");
        this.BindInt("@poolId", poolId);
        this.BindInt32Table("@agentIds", agentIds);
        this.BindGuid("@writerId", this.Author);
        DeleteAgentResult deleteAgentResult = new DeleteAgentResult();
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<TaskAgentPoolData>(this.CreateTaskAgentPoolBinder());
          resultCollection.AddBinder<TaskAgent>(this.CreateTaskAgentBinder());
          resultCollection.AddBinder<TaskAgentSessionData>(this.CreateTaskAgentSessionBinder());
          resultCollection.AddBinder<DeprovisioningAgentResult>(this.CreateDeprovisionedAgentResultBinder());
          deleteAgentResult.PoolData = resultCollection.GetCurrent<TaskAgentPoolData>().First<TaskAgentPoolData>();
          resultCollection.NextResult();
          deleteAgentResult.DeletedAgents = (IList<TaskAgent>) resultCollection.GetCurrent<TaskAgent>().Items;
          resultCollection.NextResult();
          deleteAgentResult.DeletedSessions = (IList<TaskAgentSessionData>) resultCollection.GetCurrent<TaskAgentSessionData>().Items;
          resultCollection.NextResult();
          deleteAgentResult.DeprovisioningAgents = (IList<DeprovisioningAgentResult>) resultCollection.GetCurrent<DeprovisioningAgentResult>().Items;
          return deleteAgentResult;
        }
      }
    }

    public override DeleteAgentPoolResult DeleteAgentPool(int poolId)
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (DeleteAgentPool)))
      {
        this.PrepareForAuditingAction(AgentPoolAuditConstants.AgentPoolDeleted);
        this.PrepareStoredProcedure("Task.prc_DeleteAgentPool");
        this.BindInt("@poolId", poolId);
        this.BindGuid("@writerId", this.Author);
        DeleteAgentPoolResult deleteAgentPoolResult = new DeleteAgentPoolResult();
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<TaskAgentPoolMaintenanceJob>((ObjectBinder<TaskAgentPoolMaintenanceJob>) new TaskAgentPoolMaintenanceJobBinder());
          resultCollection.AddBinder<TaskAgentPoolMaintenanceDefinition>((ObjectBinder<TaskAgentPoolMaintenanceDefinition>) new TaskAgentPoolMaintenanceDefinitionBinder());
          resultCollection.AddBinder<TaskAgent>(this.CreateTaskAgentBinder());
          resultCollection.AddBinder<TaskAgentSessionData>(this.CreateTaskAgentSessionBinder());
          resultCollection.AddBinder<TaskAgentPoolData>(this.CreateTaskAgentPoolBinder());
          resultCollection.AddBinder<DeprovisioningAgentResult>(this.CreateDeprovisionedAgentResultBinder());
          deleteAgentPoolResult.DeletedPoolMaintenanceJobs = (IList<TaskAgentPoolMaintenanceJob>) resultCollection.GetCurrent<TaskAgentPoolMaintenanceJob>().Items;
          resultCollection.NextResult();
          deleteAgentPoolResult.DeletedPoolMaintenanceDefinitions = (IList<TaskAgentPoolMaintenanceDefinition>) resultCollection.GetCurrent<TaskAgentPoolMaintenanceDefinition>().Items;
          resultCollection.NextResult();
          deleteAgentPoolResult.DeletedAgents = (IList<TaskAgent>) resultCollection.GetCurrent<TaskAgent>().Items;
          resultCollection.NextResult();
          deleteAgentPoolResult.DeletedSessions = (IList<TaskAgentSessionData>) resultCollection.GetCurrent<TaskAgentSessionData>().Items;
          resultCollection.NextResult();
          deleteAgentPoolResult.PoolData = resultCollection.GetCurrent<TaskAgentPoolData>().FirstOrDefault<TaskAgentPoolData>();
          resultCollection.NextResult();
          deleteAgentPoolResult.DeprovisioningAgents = (IList<DeprovisioningAgentResult>) resultCollection.GetCurrent<DeprovisioningAgentResult>().Items;
          return deleteAgentPoolResult;
        }
      }
    }

    public override async Task<TaskAgentList> GetAgentsAsync(
      int poolId,
      string agentName = null,
      bool includeCapabilities = false,
      bool includeAssignedRequest = false,
      bool includeAgentCloudRequest = false,
      bool includeLastCompletedRequest = false,
      IEnumerable<string> capabilityFilters = null)
    {
      TaskResourceComponent57 component = this;
      TaskAgentList agentsAsync;
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) component, nameof (GetAgentsAsync)))
      {
        component.PrepareStoredProcedure("Task.prc_GetAgents");
        component.BindInt("@poolId", poolId);
        component.BindString("@agentName", agentName, 128, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
        component.BindBoolean("@includeCapabilities", includeCapabilities);
        component.BindBoolean("@includeAssignedRequest", includeAssignedRequest);
        component.BindBoolean("@includeAgentCloudRequest", includeAgentCloudRequest);
        if (capabilityFilters != null)
        {
          HashSet<string> rows = new HashSet<string>(capabilityFilters, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
          rows.Remove(PipelineConstants.AgentName);
          rows.Remove(PipelineConstants.AgentVersionDemandName);
          component.BindStringTable("@capabilityFilters", (IEnumerable<string>) rows);
        }
        else
          component.BindStringTable("@capabilityFilters", (IEnumerable<string>) null);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) await component.ExecuteReaderAsync(), component.ProcedureName, component.RequestContext))
        {
          resultCollection.AddBinder<TaskAgent>(component.CreateTaskAgentBinder());
          if (includeCapabilities)
            resultCollection.AddBinder<TaskAgentCapability>((ObjectBinder<TaskAgentCapability>) new TaskAgentCapabilityBinder());
          if (includeAssignedRequest)
            resultCollection.AddBinder<TaskAgentJobRequest>(component.CreateTaskAgentRequestBinder(poolId));
          if (includeAgentCloudRequest)
            resultCollection.AddBinder<TaskAgentCloudRequest>(component.CreateTaskAgentCloudRequestBinder());
          resultCollection.AddBinder<bool>((ObjectBinder<bool>) new BooleanBinder());
          List<TaskAgent> items = resultCollection.GetCurrent<TaskAgent>().Items;
          if (includeAssignedRequest | includeCapabilities | includeAgentCloudRequest)
          {
            Dictionary<int, TaskAgent> dictionary = items.ToDictionary<TaskAgent, int, TaskAgent>((System.Func<TaskAgent, int>) (x => x.Id), (System.Func<TaskAgent, TaskAgent>) (x => !includeCapabilities ? x : x.PopulateImplicitCapabilities()));
            if (includeCapabilities && resultCollection.TryNextResult())
            {
              foreach (IGrouping<int, TaskAgentCapability> source in resultCollection.GetCurrent<TaskAgentCapability>().Items.GroupBy<TaskAgentCapability, int>((System.Func<TaskAgentCapability, int>) (x => x.AgentId)))
              {
                TaskAgent taskAgent;
                if (dictionary.TryGetValue(source.Key, out taskAgent))
                {
                  foreach (TaskAgentCapability taskAgentCapability in source.Where<TaskAgentCapability>((System.Func<TaskAgentCapability, bool>) (x => !x.IsWellKnownCapability())))
                  {
                    switch (taskAgentCapability.CapabilityType)
                    {
                      case TaskAgentCapabilityType.System:
                        taskAgent.SystemCapabilities.Add(taskAgentCapability.Name, taskAgentCapability.Value);
                        continue;
                      case TaskAgentCapabilityType.User:
                        taskAgent.UserCapabilities.Add(taskAgentCapability.Name, taskAgentCapability.Value);
                        continue;
                      default:
                        continue;
                    }
                  }
                }
              }
            }
            if (includeAssignedRequest && resultCollection.TryNextResult())
            {
              foreach (TaskAgentJobRequest taskAgentJobRequest in resultCollection.GetCurrent<TaskAgentJobRequest>())
              {
                TaskAgent taskAgent;
                if (dictionary.TryGetValue(taskAgentJobRequest.ReservedAgent.Id, out taskAgent))
                  taskAgent.AssignedRequest = taskAgentJobRequest;
              }
            }
            if (includeAgentCloudRequest && resultCollection.TryNextResult())
            {
              foreach (TaskAgentCloudRequest agentCloudRequest in resultCollection.GetCurrent<TaskAgentCloudRequest>())
              {
                TaskAgent taskAgent;
                if (dictionary.TryGetValue(agentCloudRequest.Agent.Id, out taskAgent))
                  taskAgent.AssignedAgentCloudRequest = agentCloudRequest;
              }
            }
          }
          resultCollection.NextResult();
          agentsAsync = new TaskAgentList(new bool?(resultCollection.GetCurrent<bool>().FirstOrDefault<bool>()), (IList<TaskAgent>) items);
        }
      }
      return agentsAsync;
    }

    public override async Task<FinishAgentRequestResult> FinishAgentRequestAsync(
      int poolId,
      long requestId,
      bool deprovisionAgent,
      bool transitionAgentState,
      TaskResult? jobResult = null,
      bool agentShuttingDown = false)
    {
      TaskResourceComponent57 component = this;
      FinishAgentRequestResult agentRequestResult;
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) component, nameof (FinishAgentRequestAsync)))
      {
        component.PrepareStoredProcedure("Task.prc_FinishAgentRequest");
        component.BindInt("@poolId", poolId);
        component.BindLong("@requestId", requestId);
        component.BindBoolean("@deprovisionAgent", deprovisionAgent);
        if (jobResult.HasValue)
          component.BindInt("@result", (int) (byte) jobResult.Value);
        FinishAgentRequestResult result = new FinishAgentRequestResult();
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) await component.ExecuteReaderAsync(), component.ProcedureName, component.RequestContext))
        {
          resultCollection.AddBinder<TaskAgentJobRequest>(component.CreateTaskAgentRequestBinder());
          resultCollection.AddBinder<TaskAgentCloud>(component.CreateTaskAgentCloudBinder());
          result.CompletedRequest = resultCollection.GetCurrent<TaskAgentJobRequest>().FirstOrDefault<TaskAgentJobRequest>();
          resultCollection.NextResult();
          result.CompletedRequestCloud = resultCollection.GetCurrent<TaskAgentCloud>().FirstOrDefault<TaskAgentCloud>();
        }
        agentRequestResult = result;
      }
      return agentRequestResult;
    }

    public override async Task<TaskAgentJobRequest> QueueAgentRequestAsync(
      int poolId,
      int? agentCloudId,
      TaskAgentJobRequest request,
      string requestResourceType)
    {
      TaskResourceComponent57 component = this;
      TaskAgentJobRequest taskAgentJobRequest;
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) component, nameof (QueueAgentRequestAsync)))
      {
        component.PrepareStoredProcedure("Task.prc_QueueAgentRequest");
        component.BindInt("@poolId", poolId);
        component.BindNullableInt("@agentCloudId", agentCloudId);
        component.BindGuid("@serviceOwner", request.ServiceOwner);
        component.BindGuid("@hostId", request.HostId);
        component.BindGuid("@scopeId", request.ScopeId);
        component.BindString("@planType", request.PlanType, 260, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
        component.BindGuid("@planId", request.PlanId);
        component.BindGuid("@jobId", request.JobId);
        component.BindString("@jobName", request.JobName, 512, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
        component.BindBinary("@demands", JsonUtility.Serialize((object) request.Demands, false), int.MaxValue, SqlDbType.VarBinary);
        component.BindBinary("@agentSpecification", JsonUtility.Serialize((object) request.AgentSpecification, false), int.MaxValue, SqlDbType.VarBinary);
        component.BindNullableInt("@definitionId", request.Definition?.Id);
        component.BindBinary("@definitionReference", JsonUtility.Serialize((object) request.Definition, false), int.MaxValue, SqlDbType.VarBinary);
        component.BindNullableInt("@ownerId", request.Owner?.Id);
        component.BindBinary("@ownerReference", JsonUtility.Serialize((object) request.Owner, false), int.MaxValue, SqlDbType.VarBinary);
        component.BindBinary("@data", JsonUtility.Serialize((object) request.Data, false), 1024, SqlDbType.VarBinary);
        component.BindString("@resourceType", requestResourceType, 160, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
        component.BindNullableInt("@queueId", request.QueueId);
        component.BindString("@orchestrationId", request.OrchestrationId, 512, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
        component.BindBoolean("@matchesAllAgents", request.MatchesAllAgentsInPool);
        component.BindInt32Table("@matchedAgentIds", (IEnumerable<int>) request.MatchedAgents.Select<TaskAgentReference, int>((System.Func<TaskAgentReference, int>) (x => x.Id)).Distinct<int>().ToList<int>());
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
          resultCollection.AddBinder<TaskAgentPoolData>(this.CreateTaskAgentPoolBinder());
          resultCollection.AddBinder<TaskAgent>(this.CreateTaskAgentBinder());
          resultCollection.AddBinder<TaskAgentCapability>((ObjectBinder<TaskAgentCapability>) new TaskAgentCapabilityBinder());
          connectivityResult.HandledEvent = resultCollection.GetCurrent<bool>().First<bool>();
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

    public override UpdateAgentPoolResult UpdateAgentPool(
      int poolId,
      string name = null,
      Guid? createdBy = null,
      Guid? groupScopeId = null,
      Guid? administratorsGroupId = null,
      Guid? serviceAccountsGroupId = null,
      Guid? serviceIdentityId = null,
      bool? isHosted = null,
      bool? autoProvision = null,
      bool? autoSize = null,
      bool? provisioned = null,
      bool removePoolMetadata = false,
      int? poolMetadataFileId = null,
      Guid? ownerId = null,
      int? agentCloudId = null,
      bool removeAgentCloud = false,
      int? targetSize = null,
      bool? isLegacy = null,
      bool? autoUpdate = null,
      TaskAgentPoolOptions? options = null)
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (UpdateAgentPool)))
      {
        this.PrepareStoredProcedure("Task.prc_UpdateAgentPool");
        this.BindInt("@poolId", poolId);
        this.BindString("@name", name, 256, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
        this.BindNullableGuid("@createdBy", createdBy);
        this.BindNullableGuid("@serviceIdentityId", serviceIdentityId);
        this.BindNullableBoolean("@isHosted", isHosted);
        this.BindNullableBoolean("@autoProvision", autoProvision);
        this.BindNullableBoolean("@autoSize", autoSize);
        this.BindBoolean("@removePoolMetadata", removePoolMetadata);
        this.BindNullableInt("@poolMetadataFileId", poolMetadataFileId);
        this.BindNullableGuid("@ownerId", ownerId);
        this.BindNullableInt("@agentCloudId", agentCloudId);
        this.BindBoolean("@removeAgentCloud", removeAgentCloud);
        this.BindGuid("@writerId", this.Author);
        this.BindNullableInt("@targetSize", targetSize);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<TaskAgentPoolData>(this.CreateTaskAgentPoolBinder());
          resultCollection.AddBinder<TaskAgentPoolData>(this.CreateTaskAgentPoolBinder());
          UpdateAgentPoolResult updateAgentPoolResult = new UpdateAgentPoolResult();
          updateAgentPoolResult.PreviousPoolData = resultCollection.GetCurrent<TaskAgentPoolData>().FirstOrDefault<TaskAgentPoolData>();
          resultCollection.NextResult();
          updateAgentPoolResult.UpdatedPoolData = resultCollection.GetCurrent<TaskAgentPoolData>().FirstOrDefault<TaskAgentPoolData>();
          return updateAgentPoolResult;
        }
      }
    }

    public override async Task<TaskAgentJobRequest> UpdateAgentRequestMatchesAsync(
      int poolId,
      long requestId,
      IList<int> matchingAgents,
      bool matchesAllAgents)
    {
      TaskResourceComponent57 component = this;
      TaskAgentJobRequest taskAgentJobRequest;
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) component, nameof (UpdateAgentRequestMatchesAsync)))
      {
        component.PrepareStoredProcedure("Task.prc_UpdateAgentRequestMatches");
        component.BindInt("@poolId", poolId);
        component.BindLong("@requestId", requestId);
        component.BindInt32Table("@matchedAgentIds", (IEnumerable<int>) matchingAgents);
        component.BindBoolean("@matchesAllAgents", matchesAllAgents);
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
