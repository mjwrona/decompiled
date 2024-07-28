// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.DataAccess.TaskResourceComponent
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Analytics;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.DistributedTask.Server.DataAccess
{
  internal class TaskResourceComponent : TaskSqlComponentBase
  {
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[80]
    {
      (IComponentCreator) new ComponentCreator<TaskResourceComponent>(1),
      (IComponentCreator) new ComponentCreator<TaskResourceComponent2>(2),
      (IComponentCreator) new ComponentCreator<TaskResourceComponent3>(3),
      (IComponentCreator) new ComponentCreator<TaskResourceComponent4>(4),
      (IComponentCreator) new ComponentCreator<TaskResourceComponent5>(5),
      (IComponentCreator) new ComponentCreator<TaskResourceComponent6>(6),
      (IComponentCreator) new ComponentCreator<TaskResourceComponent7>(7),
      (IComponentCreator) new ComponentCreator<TaskResourceComponent8>(8),
      (IComponentCreator) new ComponentCreator<TaskResourceComponent9>(9),
      (IComponentCreator) new ComponentCreator<TaskResourceComponent10>(10),
      (IComponentCreator) new ComponentCreator<TaskResourceComponent11>(11),
      (IComponentCreator) new ComponentCreator<TaskResourceComponent12>(12),
      (IComponentCreator) new ComponentCreator<TaskResourceComponent13>(13),
      (IComponentCreator) new ComponentCreator<TaskResourceComponent14>(14),
      (IComponentCreator) new ComponentCreator<TaskResourceComponent15>(15),
      (IComponentCreator) new ComponentCreator<TaskResourceComponent16>(16),
      (IComponentCreator) new ComponentCreator<TaskResourceComponent17>(17),
      (IComponentCreator) new ComponentCreator<TaskResourceComponent18>(18),
      (IComponentCreator) new ComponentCreator<TaskResourceComponent19>(19),
      (IComponentCreator) new ComponentCreator<TaskResourceComponent20>(20),
      (IComponentCreator) new ComponentCreator<TaskResourceComponent21>(21),
      (IComponentCreator) new ComponentCreator<TaskResourceComponent22>(22),
      (IComponentCreator) new ComponentCreator<TaskResourceComponent22>(23),
      (IComponentCreator) new ComponentCreator<TaskResourceComponent24>(24),
      (IComponentCreator) new ComponentCreator<TaskResourceComponent25>(25),
      (IComponentCreator) new ComponentCreator<TaskResourceComponent26>(26),
      (IComponentCreator) new ComponentCreator<TaskResourceComponent27>(27),
      (IComponentCreator) new ComponentCreator<TaskResourceComponent28>(28),
      (IComponentCreator) new ComponentCreator<TaskResourceComponent29>(29),
      (IComponentCreator) new ComponentCreator<TaskResourceComponent30>(30),
      (IComponentCreator) new ComponentCreator<TaskResourceComponent31>(31),
      (IComponentCreator) new ComponentCreator<TaskResourceComponent32>(32),
      (IComponentCreator) new ComponentCreator<TaskResourceComponent33>(33),
      (IComponentCreator) new ComponentCreator<TaskResourceComponent34>(34),
      (IComponentCreator) new ComponentCreator<TaskResourceComponent35>(35),
      (IComponentCreator) new ComponentCreator<TaskResourceComponent36>(36),
      (IComponentCreator) new ComponentCreator<TaskResourceComponent37>(37),
      (IComponentCreator) new ComponentCreator<TaskResourceComponent38>(38),
      (IComponentCreator) new ComponentCreator<TaskResourceComponent39>(39),
      (IComponentCreator) new ComponentCreator<TaskResourceComponent40>(40),
      (IComponentCreator) new ComponentCreator<TaskResourceComponent41>(41),
      (IComponentCreator) new ComponentCreator<TaskResourceComponent42>(42),
      (IComponentCreator) new ComponentCreator<TaskResourceComponent43>(43),
      (IComponentCreator) new ComponentCreator<TaskResourceComponent44>(44),
      (IComponentCreator) new ComponentCreator<TaskResourceComponent45>(45),
      (IComponentCreator) new ComponentCreator<TaskResourceComponent46>(46),
      (IComponentCreator) new ComponentCreator<TaskResourceComponent47>(47),
      (IComponentCreator) new ComponentCreator<TaskResourceComponent48>(48),
      (IComponentCreator) new ComponentCreator<TaskResourceComponent49>(49),
      (IComponentCreator) new ComponentCreator<TaskResourceComponent50>(50),
      (IComponentCreator) new ComponentCreator<TaskResourceComponent51>(51),
      (IComponentCreator) new ComponentCreator<TaskResourceComponent52>(52),
      (IComponentCreator) new ComponentCreator<TaskResourceComponent53>(53),
      (IComponentCreator) new ComponentCreator<TaskResourceComponent54>(54),
      (IComponentCreator) new ComponentCreator<TaskResourceComponent55>(55),
      (IComponentCreator) new ComponentCreator<TaskResourceComponent56>(56),
      (IComponentCreator) new ComponentCreator<TaskResourceComponent57>(57),
      (IComponentCreator) new ComponentCreator<TaskResourceComponent58>(58),
      (IComponentCreator) new ComponentCreator<TaskResourceComponent59>(59),
      (IComponentCreator) new ComponentCreator<TaskResourceComponent60>(60),
      (IComponentCreator) new ComponentCreator<TaskResourceComponent61>(61),
      (IComponentCreator) new ComponentCreator<TaskResourceComponent62>(62),
      (IComponentCreator) new ComponentCreator<TaskResourceComponent63>(63),
      (IComponentCreator) new ComponentCreator<TaskResourceComponent64>(64),
      (IComponentCreator) new ComponentCreator<TaskResourceComponent65>(65),
      (IComponentCreator) new ComponentCreator<TaskResourceComponent66>(66),
      (IComponentCreator) new ComponentCreator<TaskResourceComponent67>(67),
      (IComponentCreator) new ComponentCreator<TaskResourceComponent68>(68),
      (IComponentCreator) new ComponentCreator<TaskResourceComponent69>(69),
      (IComponentCreator) new ComponentCreator<TaskResourceComponent70>(70),
      (IComponentCreator) new ComponentCreator<TaskResourceComponent71>(71),
      (IComponentCreator) new ComponentCreator<TaskResourceComponent72>(72),
      (IComponentCreator) new ComponentCreator<TaskResourceComponent73>(73),
      (IComponentCreator) new ComponentCreator<TaskResourceComponent74>(74),
      (IComponentCreator) new ComponentCreator<TaskResourceComponent75>(75),
      (IComponentCreator) new ComponentCreator<TaskResourceComponent76>(76),
      (IComponentCreator) new ComponentCreator<TaskResourceComponent77>(77),
      (IComponentCreator) new ComponentCreator<TaskResourceComponent78>(78),
      (IComponentCreator) new ComponentCreator<TaskResourceComponent79>(79),
      (IComponentCreator) new ComponentCreator<TaskResourceComponent80>(80)
    }, "DistributedTaskResource", "DistributedTask");

    public TaskResourceComponent() => this.SelectedFeatures |= SqlResourceComponentFeatures.BindPartitionIdByDefault;

    public virtual CreateAgentResult AddAgent(int poolId, TaskAgent agent, bool createEnabled = true)
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (AddAgent)))
      {
        this.PrepareStoredProcedure("prc_AddTaskAgent");
        this.BindInt("@poolId", poolId);
        this.BindString("@name", agent.Name, 128, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
        this.BindNullableInt("@maxParallelism", agent.MaxParallelism);
        this.BindKeyValuePairStringTable("@systemCapabilities", (IEnumerable<KeyValuePair<string, string>>) agent.SystemCapabilities);
        this.BindKeyValuePairStringTable("@userCapabilities", (IEnumerable<KeyValuePair<string, string>>) agent.UserCapabilities);
        CreateAgentResult createAgentResult = new CreateAgentResult();
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<TaskAgentPoolData>((ObjectBinder<TaskAgentPoolData>) new TaskAgentPoolBinder(this.RequestContext));
          resultCollection.AddBinder<TaskAgent>((ObjectBinder<TaskAgent>) new TaskAgentBinder());
          createAgentResult.PoolData = resultCollection.GetCurrent<TaskAgentPoolData>().FirstOrDefault<TaskAgentPoolData>();
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

    public virtual TaskAgentPoolData AddAgentPool(
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
        this.PrepareStoredProcedure("prc_AddTaskAgentPool");
        this.BindString("@name", name, 128, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<TaskAgentPoolData>((ObjectBinder<TaskAgentPoolData>) new TaskAgentPoolBinder(this.RequestContext));
          return resultCollection.GetCurrent<TaskAgentPoolData>().First<TaskAgentPoolData>();
        }
      }
    }

    public virtual Task<TaskAgentCloud> AddAgentCloudAsync(
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
      return (Task<TaskAgentCloud>) null;
    }

    public virtual Task<TaskAgentCloudRequest> AddAgentCloudRequestAsync(
      int agentCloudId,
      Guid requestIdentifier,
      int poolId,
      int agentId,
      JObject agentSpecification)
    {
      return (Task<TaskAgentCloudRequest>) null;
    }

    public virtual AgentQueueOrMachineGroup AddAgentQueue(
      Guid projectId,
      string name,
      int poolId,
      TaskAgentQueueType queueType = TaskAgentQueueType.Automation,
      string description = null)
    {
      if (queueType == TaskAgentQueueType.Deployment)
        throw new ServiceVersionNotSupportedException("DistributedTaskResource", this.Version, 25);
      throw new ServiceVersionNotSupportedException("DistributedTaskResource", this.Version, 10);
    }

    public virtual DeploymentMachine AddDeploymentMachine(
      Guid projectId,
      int deploymentGroupId,
      DeploymentMachine deploymentMachine)
    {
      throw new ServiceVersionNotSupportedException("DistributedTaskResource", this.Version, 36);
    }

    public virtual IEnumerable<DeploymentMachineData> GetDeploymentMachinesByAgentId(int agentId) => (IEnumerable<DeploymentMachineData>) new List<DeploymentMachineData>();

    public virtual IEnumerable<DeploymentGroupMetrics> GetDeploymentGroupsMetrics(
      IDictionary<int, DeploymentGroup> deploymentGroups,
      Guid hostId = default (Guid),
      Guid scopeId = default (Guid))
    {
      return (IEnumerable<DeploymentGroupMetrics>) new List<DeploymentGroupMetrics>();
    }

    public virtual IEnumerable<DeploymentPoolSummary> GetDeploymentPoolsSummary(string poolName = null) => (IEnumerable<DeploymentPoolSummary>) new List<DeploymentPoolSummary>();

    public virtual CreateAgentSessionResult CreateAgentSession(
      int poolId,
      TaskAgentReference agent,
      string ownerName,
      IDictionary<string, string> systemCapabilities,
      bool transitionAgentState)
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (CreateAgentSession)))
      {
        this.PrepareStoredProcedure("prc_CreateTaskAgentSession");
        this.BindInt("@poolId", poolId);
        this.BindInt("@agentId", agent.Id);
        this.BindString("@ownerName", ownerName, 512, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
        this.BindGuid("@writerId", this.Author);
        CreateAgentSessionResult agentSession = new CreateAgentSessionResult();
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<TaskAgentSessionData>((ObjectBinder<TaskAgentSessionData>) new TaskAgentSessionDataBinder());
          resultCollection.AddBinder<TaskAgentSessionData>((ObjectBinder<TaskAgentSessionData>) new TaskAgentSessionDataBinder());
          agentSession.OldSession = resultCollection.GetCurrent<TaskAgentSessionData>().FirstOrDefault<TaskAgentSessionData>();
          resultCollection.NextResult();
          agentSession.NewSession = resultCollection.GetCurrent<TaskAgentSessionData>().FirstOrDefault<TaskAgentSessionData>();
          return agentSession;
        }
      }
    }

    public virtual TaskAgentPoolMaintenanceDefinition CreateMaintenanceDefinition(
      int poolId,
      TaskAgentPoolMaintenanceDefinition definition)
    {
      return (TaskAgentPoolMaintenanceDefinition) null;
    }

    public virtual GetTaskAgentPoolMaintenanceDefinitionResult GetAgentPoolMaintenanceDefinition(
      int poolId,
      int definitionId)
    {
      return (GetTaskAgentPoolMaintenanceDefinitionResult) null;
    }

    public virtual IList<TaskAgentPoolMaintenanceDefinition> GetAgentPoolMaintenanceDefinitions(
      int poolId)
    {
      return (IList<TaskAgentPoolMaintenanceDefinition>) Array.Empty<TaskAgentPoolMaintenanceDefinition>();
    }

    public virtual Task<TaskAgentJobRequest> GetAgentRequestForAgentCloudRequestAsync(
      int agentCloudId,
      Guid agentCloudRequestId)
    {
      return Task.FromResult<TaskAgentJobRequest>((TaskAgentJobRequest) null);
    }

    public virtual IList<TaskAgentPoolStatus> GetAgentPoolStatusByIds(IEnumerable<int> poolIds) => (IList<TaskAgentPoolStatus>) new List<TaskAgentPoolStatus>();

    public virtual TaskAgentPoolMaintenanceDefinition UpdateAgentPoolMaintenanceDefinition(
      int poolId,
      int definitionId,
      TaskAgentPoolMaintenanceDefinition definition)
    {
      return (TaskAgentPoolMaintenanceDefinition) null;
    }

    public virtual DeletePoolMaintenanceDefinitionResult DeleteAgentPoolMaintenanceDefinitions(
      int poolId,
      IEnumerable<int> definitionIds)
    {
      return new DeletePoolMaintenanceDefinitionResult();
    }

    public virtual TaskAgentPoolMaintenanceJob QueueAgentPoolMaintenanceJob(
      int poolId,
      int definitionId,
      Guid requestedBy)
    {
      return (TaskAgentPoolMaintenanceJob) null;
    }

    public virtual IList<TaskAgentPoolMaintenanceJob> GetAgentPoolMaintenanceJobs(
      int poolId,
      int? defintionId = null)
    {
      return (IList<TaskAgentPoolMaintenanceJob>) Array.Empty<TaskAgentPoolMaintenanceJob>();
    }

    public virtual Task<IList<TaskAgentCloudRequest>> GetActiveAgentCloudRequestsAsync() => Task.FromResult<IList<TaskAgentCloudRequest>>((IList<TaskAgentCloudRequest>) Array.Empty<TaskAgentCloudRequest>());

    public virtual TaskAgentPoolMaintenanceJob GetAgentPoolMaintenanceJob(int poolId, int jobId) => (TaskAgentPoolMaintenanceJob) null;

    public virtual int GetResourceUsage(
      Guid hostId,
      string resourceType,
      bool poolIsHosted,
      bool includeRunningRequests,
      int maxRequestsCount,
      out List<TaskAgentJobRequest> runningRequests)
    {
      runningRequests = new List<TaskAgentJobRequest>();
      return 0;
    }

    public virtual IList<TaskAgentPoolMaintenanceJob> DeleteAgentPoolMaintenanceJobs(
      int poolId,
      IEnumerable<int> jobIds)
    {
      return (IList<TaskAgentPoolMaintenanceJob>) Array.Empty<TaskAgentPoolMaintenanceJob>();
    }

    public virtual UpdateTaskAgentPoolMaintenanceJobResult UpdateAgentPoolMaintenanceJob(
      int poolId,
      int jobId,
      TaskAgentPoolMaintenanceJobStatus? status = null,
      DateTime? startTime = null,
      DateTime? finishTime = null,
      TaskAgentPoolMaintenanceJobResult? result = null,
      IList<TaskAgentPoolMaintenanceJobTargetAgent> targetAgents = null,
      bool updateTargetAgents = false)
    {
      return (UpdateTaskAgentPoolMaintenanceJobResult) null;
    }

    public virtual DeleteAgentResult DeleteAgent(int poolId, int agentId)
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (DeleteAgent)))
      {
        this.PrepareStoredProcedure("prc_DeleteTaskAgent");
        this.BindInt("@poolId", poolId);
        this.BindInt("@agentId", agentId);
        this.ExecuteNonQuery();
        return new DeleteAgentResult();
      }
    }

    public virtual DeleteAgentResult DeleteAgents(int poolId, IEnumerable<int> agentIds)
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (DeleteAgents)))
      {
        foreach (int agentId in agentIds)
          this.DeleteAgent(poolId, agentId);
        return new DeleteAgentResult();
      }
    }

    public virtual DeleteAgentPoolResult DeleteAgentPool(int poolId) => throw new ServiceVersionNotSupportedException(TaskResources.AgentPoolCannotBeDeletedDuringUpgrade());

    public virtual DeleteAgentQueueResult DeleteAgentQueue(
      Guid projectId,
      int queueId,
      TaskAgentQueueType queueType = TaskAgentQueueType.Automation)
    {
      if (queueType == TaskAgentQueueType.Deployment)
        throw new ServiceVersionNotSupportedException("DistributedTaskResource", this.Version, 25);
      throw new ServiceVersionNotSupportedException("DistributedTaskResource", this.Version, 10);
    }

    public virtual DeploymentMachine DeleteDeploymentMachine(
      Guid projectId,
      int deploymentGroupId,
      int deploymentMachineId)
    {
      throw new ServiceVersionNotSupportedException("DistributedTaskResource", this.Version, 36);
    }

    public virtual GetAgentQueuesResult DeleteTeamProject(Guid projectId) => new GetAgentQueuesResult();

    public virtual TaskAgentSessionData DeleteAgentSession(int poolId, Guid sessionId)
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (DeleteAgentSession)))
      {
        this.PrepareStoredProcedure("prc_DeleteTaskAgentSession");
        this.BindInt("@poolId", poolId);
        this.BindGuid("@sessionId", sessionId);
        this.BindGuid("@writerId", this.Author);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<TaskAgentSessionData>((ObjectBinder<TaskAgentSessionData>) new TaskAgentSessionDataBinder());
          return resultCollection.GetCurrent<TaskAgentSessionData>().FirstOrDefault<TaskAgentSessionData>();
        }
      }
    }

    public virtual TaskAgentSessionData DeleteAgentSession(int poolId, int agentId) => (TaskAgentSessionData) null;

    public virtual Task<TaskAgentSessionData> DeleteAgentSessionAsync(int poolId, int agentId) => Task.FromResult<TaskAgentSessionData>(this.DeleteAgentSession(poolId, agentId));

    protected virtual TaskAgent GetAgent(
      int poolId,
      int agentId,
      bool includeCapabilities = false,
      bool includeAssignedRequest = false)
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (GetAgent)))
      {
        this.PrepareStoredProcedure("prc_GetTaskAgent");
        this.BindInt("@poolId", poolId);
        this.BindInt("@agentId", agentId);
        this.BindBoolean("@includeCapabilities", includeCapabilities);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<TaskAgent>((ObjectBinder<TaskAgent>) new TaskAgentBinder());
          resultCollection.AddBinder<TaskAgentCapability>((ObjectBinder<TaskAgentCapability>) new TaskAgentCapabilityBinder());
          TaskAgent agent = resultCollection.GetCurrent<TaskAgent>().FirstOrDefault<TaskAgent>();
          if (includeCapabilities && resultCollection.TryNextResult())
          {
            foreach (TaskAgentCapability taskAgentCapability in resultCollection.GetCurrent<TaskAgentCapability>().Items)
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
          return agent;
        }
      }
    }

    public virtual IList<TaskAgent> GetAgentsById(
      int poolId,
      IEnumerable<int> agentIds,
      bool includeCapabilities = false,
      bool includeAssignedRequest = false,
      bool includeLastCompletedRequest = false)
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (GetAgentsById)))
      {
        if (agentIds.Count<int>() == 1)
          return (IList<TaskAgent>) new TaskAgent[1]
          {
            this.GetAgent(poolId, agentIds.First<int>(), includeCapabilities, includeAssignedRequest)
          };
        ILookup<int, int> agentIdLookUp = agentIds.ToLookup<int, int>((System.Func<int, int>) (x => x));
        return (IList<TaskAgent>) this.GetAgents(poolId, includeCapabilities: includeCapabilities, includeAssignedRequest: includeAssignedRequest).Agents.Where<TaskAgent>((System.Func<TaskAgent, bool>) (a => agentIdLookUp.Contains(a.Id))).ToList<TaskAgent>();
      }
    }

    public virtual Task<IList<TaskAgent>> GetAgentsByIdAsync(
      int poolId,
      IEnumerable<int> agentIds,
      bool includeCapabilities = false,
      bool includeAssignedRequest = false,
      bool includeLastCompletedRequest = false)
    {
      return Task.FromResult<IList<TaskAgent>>(this.GetAgentsById(poolId, agentIds, includeCapabilities, includeAssignedRequest, includeLastCompletedRequest));
    }

    public virtual Task<IEnumerable<TaskAgent>> GetAgentsByFilterAsync(
      int poolId,
      Guid hostId,
      Guid scopeId,
      string agentName = null,
      bool partialNameMatch = false,
      bool includeCapabilities = false,
      bool includeAssignedRequest = false,
      bool includeLastCompletedRequest = false,
      IEnumerable<int> agentIds = null,
      int? agentStatusFilter = null,
      IEnumerable<byte> agentLastJobStatusFilters = null,
      string continuationToken = null,
      bool isNeverDeployedFilter = false,
      int top = 1000,
      bool? enabled = null)
    {
      return Task.FromResult<IEnumerable<TaskAgent>>((IEnumerable<TaskAgent>) new List<TaskAgent>());
    }

    public virtual TaskAgentList GetAgents(
      int poolId,
      string agentName = null,
      bool includeCapabilities = false,
      bool includeAssignedRequest = false,
      bool includeAgentCloudRequest = false,
      bool includeLastCompletedRequest = false,
      IEnumerable<string> capabilityFilters = null)
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (GetAgents)))
      {
        this.PrepareStoredProcedure("prc_GetTaskAgents");
        this.BindInt("@poolId", poolId);
        this.BindString("@agentName", agentName, 128, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
        this.BindBoolean("@includeCapabilities", includeCapabilities);
        this.BindStringTable("@capabilityFilters", capabilityFilters == null ? (IEnumerable<string>) null : capabilityFilters.Distinct<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase));
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<TaskAgent>((ObjectBinder<TaskAgent>) new TaskAgentBinder());
          resultCollection.AddBinder<TaskAgentCapability>((ObjectBinder<TaskAgentCapability>) new TaskAgentCapabilityBinder());
          List<TaskAgent> items = resultCollection.GetCurrent<TaskAgent>().Items;
          if (includeCapabilities && resultCollection.TryNextResult())
          {
            Dictionary<int, TaskAgent> dictionary = items.ToDictionary<TaskAgent, int>((System.Func<TaskAgent, int>) (x => x.Id));
            foreach (IGrouping<int, TaskAgentCapability> grouping in resultCollection.GetCurrent<TaskAgentCapability>().Items.GroupBy<TaskAgentCapability, int>((System.Func<TaskAgentCapability, int>) (x => x.AgentId)))
            {
              TaskAgent taskAgent;
              if (dictionary.TryGetValue(grouping.Key, out taskAgent))
              {
                foreach (TaskAgentCapability taskAgentCapability in (IEnumerable<TaskAgentCapability>) grouping)
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
          return new TaskAgentList(new bool?(), (IList<TaskAgent>) items);
        }
      }
    }

    public virtual Task<TaskAgentList> GetAgentsAsync(
      int poolId,
      string agentName = null,
      bool includeCapabilities = false,
      bool includeAssignedRequest = false,
      bool includeAgentCloudRequest = false,
      bool includeLastCompletedRequest = false,
      IEnumerable<string> capabilityFilters = null)
    {
      return Task.FromResult<TaskAgentList>(this.GetAgents(poolId, agentName, includeCapabilities, includeAssignedRequest, includeAgentCloudRequest, includeLastCompletedRequest, capabilityFilters));
    }

    public virtual IList<TaskAgentMachine> GetAgentMachines(IEnumerable<string> machineNames) => (IList<TaskAgentMachine>) Array.Empty<TaskAgentMachine>();

    public virtual TaskAgentPoolData GetAgentPool(int poolId)
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (GetAgentPool)))
      {
        this.PrepareStoredProcedure("prc_GetTaskAgentPool");
        this.BindInt("@poolId", poolId);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<TaskAgentPoolData>((ObjectBinder<TaskAgentPoolData>) new TaskAgentPoolBinder(this.RequestContext));
          return resultCollection.GetCurrent<TaskAgentPoolData>().FirstOrDefault<TaskAgentPoolData>();
        }
      }
    }

    public virtual Task<TaskAgentPoolData> GetAgentPoolAsync(int poolId) => Task.FromResult<TaskAgentPoolData>(this.GetAgentPool(poolId));

    public virtual IList<TaskAgentPoolData> GetAgentPoolsById(HashSet<int> poolIds)
    {
      List<TaskAgentPoolData> agentPoolsById = new List<TaskAgentPoolData>();
      if (poolIds.Any<int>())
        agentPoolsById.Add(this.GetAgentPool(poolIds.FirstOrDefault<int>()));
      return (IList<TaskAgentPoolData>) agentPoolsById;
    }

    public virtual Task<IList<TaskAgentPoolData>> GetAgentPoolsByIdAsync(HashSet<int> poolIds) => Task.FromResult<IList<TaskAgentPoolData>>(this.GetAgentPoolsById(poolIds));

    public virtual Task<IEnumerable<TaskAgentPoolData>> GetAgentPoolsAsync(
      string poolName,
      TaskAgentPoolType poolType = TaskAgentPoolType.Automation)
    {
      return Task.FromResult<IEnumerable<TaskAgentPoolData>>(this.GetAgentPools(poolName, poolType));
    }

    public virtual IEnumerable<TaskAgentPoolData> GetAgentPools(
      string poolName,
      TaskAgentPoolType poolType = TaskAgentPoolType.Automation)
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (GetAgentPools)))
      {
        this.PrepareStoredProcedure("prc_GetTaskAgentPools");
        this.BindString("@poolName", poolName, 128, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<TaskAgentPoolData>((ObjectBinder<TaskAgentPoolData>) new TaskAgentPoolBinder(this.RequestContext));
          return (IEnumerable<TaskAgentPoolData>) resultCollection.GetCurrent<TaskAgentPoolData>().Items;
        }
      }
    }

    public virtual List<AgentPoolData> GetAgentPoolsByLastModifiedDate(
      int batchSize,
      DateTime? fromDate)
    {
      return new List<AgentPoolData>();
    }

    public virtual TaskAgentRequestQueryResult GetAgentRequests(
      int poolId,
      int maxRequestCount,
      DateTime? lastRunningAssignTime,
      long? lastQueuedRequestId,
      DateTime? lastFinishedFinishTime)
    {
      return new TaskAgentRequestQueryResult();
    }

    public virtual Task<TaskAgentRequestQueryResult> GetAgentRequestsAsync(
      int poolId,
      int maxRequestCount,
      DateTime? lastRunningAssignTime,
      long? lastQueuedRequestId,
      DateTime? lastFinishedFinishTime)
    {
      return Task.FromResult<TaskAgentRequestQueryResult>(new TaskAgentRequestQueryResult());
    }

    public virtual Task<TaskAgentCloud> DeleteAgentCloudAsync(int agentCloudId) => (Task<TaskAgentCloud>) null;

    public virtual IList<TaskAgentCloud> GetAgentClouds(int? cloudId) => (IList<TaskAgentCloud>) Array.Empty<TaskAgentCloud>();

    public virtual Task<IList<TaskAgentCloud>> GetAgentCloudsAsync(int? cloudId) => Task.FromResult<IList<TaskAgentCloud>>((IList<TaskAgentCloud>) Array.Empty<TaskAgentCloud>());

    public virtual Task<TaskAgentCloudRequest> GetAgentCloudRequestAsync(
      int agentCloudId,
      Guid requestId)
    {
      return Task.FromResult<TaskAgentCloudRequest>((TaskAgentCloudRequest) null);
    }

    public virtual Task<TaskAgentCloudRequest> GetAgentCloudRequestForAgentAsync(
      int poolId,
      int agentId)
    {
      return Task.FromResult<TaskAgentCloudRequest>((TaskAgentCloudRequest) null);
    }

    public virtual Task<IList<TaskAgentCloudRequest>> GetAgentCloudRequestsAsync(int? agentCloudId) => Task.FromResult<IList<TaskAgentCloudRequest>>((IList<TaskAgentCloudRequest>) Array.Empty<TaskAgentCloudRequest>());

    public virtual TaskAgentCloud GetAgentCloudForPool(int poolId) => (TaskAgentCloud) null;

    public virtual Task<TaskAgentCloud> GetAgentCloudForPoolAsync(int poolId) => Task.FromResult<TaskAgentCloud>((TaskAgentCloud) null);

    public virtual AgentQueueOrMachineGroup GetAgentQueue(
      Guid projectId,
      int queueId,
      TaskAgentQueueType queueType = TaskAgentQueueType.Automation,
      bool includeMachines = true,
      bool includeTags = false)
    {
      return new AgentQueueOrMachineGroup();
    }

    public virtual GetAgentQueuesResult GetAgentQueuesById(
      Guid projectId,
      IEnumerable<int> queueIds,
      TaskAgentQueueType queueType = TaskAgentQueueType.Automation,
      bool includeMachines = false,
      bool includeTags = false)
    {
      return new GetAgentQueuesResult();
    }

    public virtual GetAgentQueuesResult GetAgentQueuesByName(
      Guid projectId,
      IEnumerable<string> queueNames,
      TaskAgentQueueType queueType = TaskAgentQueueType.Automation,
      bool includeMachines = false,
      bool includeTags = false)
    {
      return new GetAgentQueuesResult();
    }

    public virtual GetAgentQueuesResult GetAgentQueuesByPoolId(
      Guid projectId,
      IEnumerable<int> poolIds,
      TaskAgentQueueType queueType = TaskAgentQueueType.Automation,
      bool includeMachines = false,
      bool includeTags = false)
    {
      return new GetAgentQueuesResult();
    }

    public virtual Task<AgentQueueOrMachineGroup> GetAgentQueueAsync(
      Guid projectId,
      int queueId,
      TaskAgentQueueType queueType = TaskAgentQueueType.Automation,
      bool includeMachines = true,
      bool includeTags = false)
    {
      return Task.FromResult<AgentQueueOrMachineGroup>(new AgentQueueOrMachineGroup());
    }

    public virtual GetAgentQueuesResult GetAgentQueues(
      Guid projectId,
      string queueName,
      TaskAgentQueueType queueType = TaskAgentQueueType.Automation,
      bool includeMachines = false,
      bool includeTags = true,
      string lastQueueName = null,
      int maxQueuesCount = 10000)
    {
      return new GetAgentQueuesResult();
    }

    public virtual GetAgentQueuesResult GetAgentQueuesByPoolIds(
      IEnumerable<int> poolIds,
      TaskAgentQueueType queueType = TaskAgentQueueType.Automation)
    {
      return poolIds.Count<int>() == 1 ? this.GetAgentQueuesByPoolId(poolIds.First<int>(), queueType) : new GetAgentQueuesResult();
    }

    public virtual GetAgentQueuesResult GetHostedAgentQueues(Guid projectId) => new GetAgentQueuesResult();

    public virtual Task<IList<TaskAgentPoolData>> GetAgentPoolsWithCapabilityChangesAsync() => Task.FromResult<IList<TaskAgentPoolData>>((IList<TaskAgentPoolData>) Array.Empty<TaskAgentPoolData>());

    public virtual IList<TaskAgentSessionData> GetAgentSessions(int? poolId = null, Guid? sessionId = null)
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (GetAgentSessions)))
      {
        this.PrepareStoredProcedure("prc_GetTaskAgentSessions");
        this.BindNullableInt("@poolId", poolId);
        this.BindNullableGuid("@sessionId", sessionId);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<TaskAgentSessionData>((ObjectBinder<TaskAgentSessionData>) new TaskAgentSessionDataBinder());
          return (IList<TaskAgentSessionData>) resultCollection.GetCurrent<TaskAgentSessionData>().ToList<TaskAgentSessionData>();
        }
      }
    }

    public virtual TaskAgentRequestData GetAgentRequest(
      int poolId,
      long requestId,
      bool includeStatus = false)
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (GetAgentRequest)))
      {
        TaskAgentRequestData agentRequest1 = (TaskAgentRequestData) null;
        this.PrepareStoredProcedure("prc_GetTaskAgentJobRequest");
        this.BindInt("@poolId", poolId);
        this.BindLong("@requestId", requestId);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<TaskAgentJobRequest>((ObjectBinder<TaskAgentJobRequest>) new TaskAgentRequestBinder());
          TaskAgentJobRequest agentRequest2 = resultCollection.GetCurrent<TaskAgentJobRequest>().FirstOrDefault<TaskAgentJobRequest>();
          if (agentRequest2 != null)
            agentRequest1 = new TaskAgentRequestData(agentRequest2);
        }
        return agentRequest1;
      }
    }

    public virtual Task<TaskAgentRequestData> GetAgentRequestAsync(
      int poolId,
      long requestId,
      bool includeStatus = false)
    {
      return Task.FromResult<TaskAgentRequestData>(this.GetAgentRequest(poolId, requestId, includeStatus));
    }

    public virtual IList<TaskAgentJobRequest> GetAgentRequestsForAgent(
      int poolId,
      int agentId,
      int completedRequestCount)
    {
      return (IList<TaskAgentJobRequest>) Array.Empty<TaskAgentJobRequest>();
    }

    public virtual IList<TaskAgentJobRequest> GetAgentRequestsForAgents(
      int poolId,
      IList<int> agentIds,
      int completedRequestCount)
    {
      return (IList<TaskAgentJobRequest>) Array.Empty<TaskAgentJobRequest>();
    }

    public virtual IList<TaskAgentJobRequest> GetAgentRequestsForAgents(
      int poolId,
      IList<int> agentIds,
      Guid hostId,
      Guid scopeId,
      int completedRequestCount,
      int? ownerId = null,
      DateTime? completedOn = null)
    {
      return (IList<TaskAgentJobRequest>) Array.Empty<TaskAgentJobRequest>();
    }

    public virtual Task<IList<TaskAgentJobRequest>> GetAgentRequestsForAgentsAsync(
      int poolId,
      IList<int> agentIds,
      int completedRequestCount)
    {
      return Task.FromResult<IList<TaskAgentJobRequest>>(this.GetAgentRequestsForAgents(poolId, agentIds, completedRequestCount));
    }

    public virtual IList<TaskAgentJobRequest> GetAgentRequestsForPlan(
      int poolId,
      Guid planId,
      Guid? jobId)
    {
      return (IList<TaskAgentJobRequest>) Array.Empty<TaskAgentJobRequest>();
    }

    public virtual IList<TaskAgentJobRequest> GetAgentRequestsForPlan(Guid planId) => (IList<TaskAgentJobRequest>) Array.Empty<TaskAgentJobRequest>();

    public virtual Task<ExpiredAgentRequestsResult> GetExpiredAgentRequestsAsync(int poolId) => Task.FromResult<ExpiredAgentRequestsResult>(new ExpiredAgentRequestsResult(true, (IList<TaskAgentJobRequest>) Array.Empty<TaskAgentJobRequest>()));

    public virtual Task<IList<TaskAgentJobRequest>> GetUnassigableAgentRequestsAsync(
      DateTime expirationDate,
      int batchSize)
    {
      return Task.FromResult<IList<TaskAgentJobRequest>>((IList<TaskAgentJobRequest>) Array.Empty<TaskAgentJobRequest>());
    }

    public virtual Task<IList<TaskAgentJobRequest>> GetUnassignedAgentRequestsAsync(int poolId) => Task.FromResult<IList<TaskAgentJobRequest>>((IList<TaskAgentJobRequest>) Array.Empty<TaskAgentJobRequest>());

    public virtual async Task<TaskAgentJobRequest> QueueAgentRequestAsync(
      int poolId,
      int? agentCloudId,
      TaskAgentJobRequest request,
      string requestResourceType)
    {
      TaskResourceComponent component = this;
      TaskAgentJobRequest taskAgentJobRequest;
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) component, nameof (QueueAgentRequestAsync)))
      {
        component.PrepareStoredProcedure("prc_QueueTaskAgentJobRequest");
        component.BindInt("@poolId", poolId);
        component.BindInt32Table("@matchedAgentIds", (IEnumerable<int>) request.MatchedAgents.Select<TaskAgentReference, int>((System.Func<TaskAgentReference, int>) (x => x.Id)).Distinct<int>().ToList<int>());
        component.BindGuid("@hostId", request.HostId);
        component.BindGuid("@planId", request.PlanId);
        component.BindGuid("@jobId", request.JobId);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) await component.ExecuteReaderAsync(), component.ProcedureName, component.RequestContext))
        {
          resultCollection.AddBinder<TaskAgentJobRequest>((ObjectBinder<TaskAgentJobRequest>) new TaskAgentRequestBinder());
          taskAgentJobRequest = resultCollection.GetCurrent<TaskAgentJobRequest>().FirstOrDefault<TaskAgentJobRequest>();
        }
      }
      return taskAgentJobRequest;
    }

    public virtual async Task<FinishAgentRequestResult> FinishAgentRequestAsync(
      int poolId,
      long requestId,
      bool deprovisionAgent,
      bool transitionAgentState,
      TaskResult? jobResult = null,
      bool agentShuttingDown = false)
    {
      TaskResourceComponent component = this;
      FinishAgentRequestResult agentRequestResult;
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) component, nameof (FinishAgentRequestAsync)))
      {
        component.PrepareStoredProcedure("prc_FinishTaskAgentJobRequest");
        component.BindInt("@poolId", poolId);
        component.BindLong("@requestId", requestId);
        using (new ResultCollection((IDataReader) await component.ExecuteReaderAsync(), component.ProcedureName, component.RequestContext))
          agentRequestResult = new FinishAgentRequestResult();
      }
      return agentRequestResult;
    }

    public virtual Task ResetAgentPoolCapabilityChangesAsync(IEnumerable<int> poolIds) => Task.CompletedTask;

    public virtual AgentConnectivityResult SetAgentOffline(
      int poolId,
      int agentId,
      int sequenceId = 0,
      bool force = false)
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (SetAgentOffline)))
      {
        this.PrepareStoredProcedure("prc_SetTaskAgentOffline");
        this.BindInt("@poolId", poolId);
        this.BindInt("@agentId", agentId);
        this.BindInt("@sequenceId", sequenceId);
        this.ExecuteNonQuery();
        AgentConnectivityResult connectivityResult = new AgentConnectivityResult();
        connectivityResult.Agent = (TaskAgent) null;
        connectivityResult.HandledEvent = true;
        connectivityResult = connectivityResult;
        return connectivityResult;
      }
    }

    public virtual AgentConnectivityResult SetAgentOnline(int poolId, int agentId, int sequenceId)
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (SetAgentOnline)))
      {
        this.PrepareStoredProcedure("prc_SetTaskAgentOnline");
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

    public virtual UpdateAgentResult UpdateAgent(
      int poolId,
      TaskAgent agent,
      TaskAgentCapabilityType capabilityUpdate)
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (UpdateAgent)))
      {
        this.PrepareStoredProcedure("prc_UpdateTaskAgent");
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
          resultCollection.AddBinder<TaskAgentPoolData>((ObjectBinder<TaskAgentPoolData>) new TaskAgentPoolBinder(this.RequestContext));
          resultCollection.AddBinder<TaskAgent>((ObjectBinder<TaskAgent>) new TaskAgentBinder());
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

    public virtual Task<UpdateAgentResult> UpdateAgentAsync(
      int poolId,
      TaskAgent agent,
      TaskAgentCapabilityType capabilityUpdate)
    {
      return Task.FromResult<UpdateAgentResult>(this.UpdateAgent(poolId, agent, capabilityUpdate));
    }

    public virtual Task<TaskAgentCloudRequest> UpdateAgentCloudRequestAsync(
      int agentCloudId,
      Guid requestId,
      JObject agentSpecification,
      JObject agentData,
      DateTime? provisionRequestTime,
      DateTime? provisionedTime,
      DateTime? agentConnectedTime,
      DateTime? releaseRequestTime)
    {
      return (Task<TaskAgentCloudRequest>) null;
    }

    public virtual Task<TaskAgentCloud> UpdateAgentCloudAsync(
      int agentCloudId,
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
      return (Task<TaskAgentCloud>) null;
    }

    public virtual UpdateAgentPoolResult UpdateAgentPool(
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
        this.PrepareStoredProcedure("prc_UpdateTaskAgentPool");
        this.BindInt("@poolId", poolId);
        this.BindString("@name", name, 256, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
        this.BindNullableGuid("@administratorsGroupId", administratorsGroupId);
        this.BindNullableGuid("@serviceAccountsGroupId", serviceAccountsGroupId);
        this.BindNullableGuid("@serviceIdentityId", serviceIdentityId);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          UpdateAgentPoolResult updateAgentPoolResult = new UpdateAgentPoolResult();
          resultCollection.AddBinder<TaskAgentPoolData>((ObjectBinder<TaskAgentPoolData>) new TaskAgentPoolBinder(this.RequestContext));
          updateAgentPoolResult.UpdatedPoolData = resultCollection.GetCurrent<TaskAgentPoolData>().FirstOrDefault<TaskAgentPoolData>();
          return updateAgentPoolResult;
        }
      }
    }

    public virtual AgentQueueOrMachineGroup UpdateAgentQueue(
      Guid projectId,
      int queueId,
      string name,
      Guid? groupScopeId = null,
      bool? groupScopeProvisioned = null,
      int? poolId = null,
      TaskAgentQueueType queueType = TaskAgentQueueType.Automation,
      string description = null)
    {
      if (queueType == TaskAgentQueueType.Deployment)
        throw new ServiceVersionNotSupportedException("DistributedTaskResource", this.Version, 26);
      throw new ServiceVersionNotSupportedException("DistributedTaskResource", 1, 11);
    }

    public virtual Task<TaskAgentJobRequest> UpdateAgentRequestMatchesAsync(
      int poolId,
      long requestId,
      IList<int> agentIds,
      bool matchaseAllAgents)
    {
      return Task.FromResult<TaskAgentJobRequest>((TaskAgentJobRequest) null);
    }

    public virtual TaskAgentJobRequest AbandonAgentRequest(
      int poolId,
      long requestId,
      DateTime expirationTime)
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (AbandonAgentRequest)))
      {
        this.PrepareStoredProcedure("prc_AbandonTaskAgentJobRequest");
        this.BindInt("@poolId", poolId);
        this.BindLong("@requestId", requestId);
        this.BindDateTime2("@expirationTime", expirationTime);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<TaskAgentJobRequest>((ObjectBinder<TaskAgentJobRequest>) new TaskAgentRequestBinder());
          return resultCollection.GetCurrent<TaskAgentJobRequest>().FirstOrDefault<TaskAgentJobRequest>();
        }
      }
    }

    public virtual UpdateAgentRequestResult UpdateAgentRequest(
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
        this.PrepareStoredProcedure("prc_UpdateTaskAgentJobRequest");
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
          resultCollection.AddBinder<TaskAgentJobRequest>((ObjectBinder<TaskAgentJobRequest>) new TaskAgentRequestBinder());
          agentRequestResult.After = resultCollection.GetCurrent<TaskAgentJobRequest>().FirstOrDefault<TaskAgentJobRequest>();
          return agentRequestResult;
        }
      }
    }

    public virtual Task<UpdateAgentRequestResult> UpdateAgentRequestAsync(
      int poolId,
      long requestId,
      TimeSpan leaseRenewalTimeout,
      DateTime? expirationTime = null,
      DateTime? startTime = null,
      DateTime? finishTime = null,
      TaskResult? result = null)
    {
      return Task.FromResult<UpdateAgentRequestResult>(this.UpdateAgentRequest(poolId, requestId, leaseRenewalTimeout, expirationTime, startTime, finishTime, result));
    }

    public virtual GetDeploymentMachinesResult GetDeploymentMachines(
      Guid projectId,
      int machineGroupId,
      IEnumerable<string> tagFilters)
    {
      return new GetDeploymentMachinesResult();
    }

    public virtual Task<GetDeploymentMachinesResult> GetDeploymentMachinesAsync(
      Guid projectId,
      int machineGroupId,
      IEnumerable<string> tagFilters)
    {
      return Task.FromResult<GetDeploymentMachinesResult>(this.GetDeploymentMachines(projectId, machineGroupId, tagFilters));
    }

    public virtual Task<GetDeploymentMachinesResult> GetDeploymentTargetsAsync(
      Guid projectId,
      int DeploymentGroupId,
      IEnumerable<string> tagFilters)
    {
      return Task.FromResult<GetDeploymentMachinesResult>(new GetDeploymentMachinesResult());
    }

    public virtual UpdateDeploymentMachinesResult UpdateDeploymentMachines(
      Guid projectId,
      int machineGroupId,
      IEnumerable<DeploymentMachine> machines)
    {
      throw new ServiceVersionNotSupportedException("DistributedTaskResource", this.Version, 25);
    }

    public virtual DeploymentMachine AddDeploymentTarget(
      Guid projectId,
      int deploymentGroupId,
      DeploymentMachine deploymentMachine)
    {
      throw new ServiceVersionNotSupportedException("DistributedTaskResource", this.Version, 39);
    }

    public virtual DeploymentMachine DeleteDeploymentTarget(
      Guid projectId,
      int deploymentGroupId,
      int deploymentMachineId)
    {
      throw new ServiceVersionNotSupportedException("DistributedTaskResource", this.Version, 39);
    }

    public virtual UpdateDeploymentMachinesResult UpdateDeploymentTargets(
      Guid projectId,
      int machineGroupId,
      IEnumerable<DeploymentMachine> machines)
    {
      throw new ServiceVersionNotSupportedException("DistributedTaskResource", this.Version, 39);
    }

    public virtual Task<RequestAgentsUpdateResult> RequestAgentsUpdateAsync(
      int poolId,
      IEnumerable<int> agentIds,
      string targetVersion,
      Guid requestedBy,
      string currentState,
      TaskAgentUpdateReasonData reasonData)
    {
      return Task.FromResult<RequestAgentsUpdateResult>(new RequestAgentsUpdateResult());
    }

    public virtual TaskAgent UpdateAgentUpdateState(int poolId, int agentId, string currentState) => (TaskAgent) null;

    public virtual TaskAgent FinishAgentUpdate(
      int poolId,
      int agentId,
      TaskResult updateResult,
      string currentState)
    {
      return (TaskAgent) null;
    }

    public virtual Task<AssignAgentRequestsResult> AssignAgentRequestsAsync(
      TimeSpan privateLeaseTimeout,
      TimeSpan hostedLeaseTimeout,
      TimeSpan defaultLeaseTimeout,
      IList<ResourceLimit> resourceLimits,
      int maxRequestsCount,
      bool transitionAgentState)
    {
      return Task.FromResult<AssignAgentRequestsResult>(new AssignAgentRequestsResult());
    }

    public virtual IEnumerable<DeploymentPoolSummary> GetDeploymentPoolsSummaryById(
      IList<int> poolIds)
    {
      return (IEnumerable<DeploymentPoolSummary>) new List<DeploymentPoolSummary>();
    }

    public virtual IEnumerable<TaskAgentPoolData> GetActiveAgentPools(DateTime activeSince) => Enumerable.Empty<TaskAgentPoolData>();

    public virtual List<AgentRequestData> GetAgentRequestData(
      List<TaskTimelineRecord> timelineRecords)
    {
      return new List<AgentRequestData>();
    }

    public virtual List<AgentRequestData> GetAgentRequestDataFromDate(
      int batchSize,
      DateTime? fromTime)
    {
      return new List<AgentRequestData>();
    }

    public virtual List<TaskAgentPoolSizeData> GetAgentPoolSize() => new List<TaskAgentPoolSizeData>();

    public virtual ResourceLockRequest QueueResourceLockRequest(ResourceLockRequest request) => new ResourceLockRequest();

    public virtual ResourceLockRequest UpdateResourceLockRequest(
      long requestId,
      ResourceLockStatus status,
      DateTime? finishTime = null)
    {
      return new ResourceLockRequest();
    }

    public virtual List<ResourceLockRequest> FreeResourceLocks(
      Guid planId,
      string nodeName,
      int nodeAttempt)
    {
      return new List<ResourceLockRequest>();
    }

    public virtual void CleanupResourceLockTable(int daysToKeep)
    {
    }

    public virtual Task<TaskAgentJobRequest> BumpAgentRequestPriorityAsync(
      int poolId,
      long requestId)
    {
      return Task.FromResult<TaskAgentJobRequest>(new TaskAgentJobRequest());
    }

    protected override SqlParameter BindNullableGuid(string parameterName, Guid? value) => !value.HasValue ? this.BindNullValue(parameterName, SqlDbType.UniqueIdentifier) : this.BindGuid(parameterName, value.Value);

    protected virtual GetAgentQueuesResult GetAgentQueuesByPoolId(
      int poolId,
      TaskAgentQueueType queueType = TaskAgentQueueType.Automation)
    {
      return new GetAgentQueuesResult();
    }

    public virtual Task<AssignAgentRequestsResult> AssignAgentRequestsAsyncV2(
      TimeSpan privateLeaseTimeout,
      TimeSpan hostedLeaseTimeout,
      TimeSpan defaultLeaseTimeout,
      IList<ResourceLimit> resourceLimits,
      int maxRequestsCount,
      bool transitionAgentState)
    {
      throw new ServiceVersionNotSupportedException("DistributedTaskResource", this.Version, 78);
    }
  }
}
