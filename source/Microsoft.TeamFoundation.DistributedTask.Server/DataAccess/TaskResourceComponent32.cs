// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.DataAccess.TaskResourceComponent32
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.Pipelines;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.DistributedTask.Server.DataAccess
{
  internal class TaskResourceComponent32 : TaskResourceComponent31
  {
    protected virtual ObjectBinder<TaskAgent> CreateTaskAgentBinder() => (ObjectBinder<TaskAgent>) new TaskAgentBinder6();

    public override AgentQueueOrMachineGroup GetAgentQueue(
      Guid projectId,
      int queueId,
      TaskAgentQueueType queueType = TaskAgentQueueType.Automation,
      bool includeMachines = true,
      bool includeTags = false)
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (GetAgentQueue)))
      {
        this.PrepareStoredProcedure("Task.prc_GetQueuesById");
        this.BindInt("@dataspaceId", this.GetDataspaceId(projectId, "DistributedTask", true));
        this.BindByte("@queueType", (byte) queueType);
        this.BindUniqueInt32Table("@queueIds", (IEnumerable<int>) new int[1]
        {
          queueId
        });
        this.BindBoolean("@includeAgentTags", includeMachines);
        AgentQueueOrMachineGroup agentQueue = new AgentQueueOrMachineGroup();
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          if (queueType == TaskAgentQueueType.Automation)
          {
            resultCollection.AddBinder<TaskAgentQueue>((ObjectBinder<TaskAgentQueue>) new TaskAgentQueueBinder3((TaskResourceComponent) this));
            agentQueue.Queue = resultCollection.GetCurrent<TaskAgentQueue>().FirstOrDefault<TaskAgentQueue>();
          }
          else
          {
            resultCollection.AddBinder<DeploymentGroup>((ObjectBinder<DeploymentGroup>) new DeploymentGroupBinder((TaskResourceComponent) this));
            agentQueue.MachineGroup = resultCollection.GetCurrent<DeploymentGroup>().FirstOrDefault<DeploymentGroup>();
            if (includeMachines && agentQueue.MachineGroup != null)
            {
              resultCollection.AddBinder<TaskAgentTag>((ObjectBinder<TaskAgentTag>) new TaskAgentTagBinder());
              resultCollection.NextResult();
              List<TaskAgentTag> items = resultCollection.GetCurrent<TaskAgentTag>().Items;
              agentQueue.MachineGroup.Machines.AddRange<DeploymentMachine, IList<DeploymentMachine>>(TaskResourceComponent25.CreateDeploymentMachinesWithTags((IEnumerable<TaskAgentTag>) items));
            }
          }
          return agentQueue;
        }
      }
    }

    public override async Task<AgentQueueOrMachineGroup> GetAgentQueueAsync(
      Guid projectId,
      int queueId,
      TaskAgentQueueType queueType = TaskAgentQueueType.Automation,
      bool includeMachines = true,
      bool includeTags = false)
    {
      TaskResourceComponent32 component = this;
      AgentQueueOrMachineGroup agentQueueAsync;
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) component, nameof (GetAgentQueueAsync)))
      {
        component.PrepareStoredProcedure("Task.prc_GetQueuesById");
        component.BindInt("@dataspaceId", component.GetDataspaceId(projectId, "DistributedTask", true));
        component.BindByte("@queueType", (byte) queueType);
        component.BindUniqueInt32Table("@queueIds", (IEnumerable<int>) new int[1]
        {
          queueId
        });
        component.BindBoolean("@includeAgentTags", includeMachines);
        AgentQueueOrMachineGroup result = new AgentQueueOrMachineGroup();
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) await component.ExecuteReaderAsync(), component.ProcedureName, component.RequestContext))
        {
          if (queueType == TaskAgentQueueType.Automation)
          {
            resultCollection.AddBinder<TaskAgentQueue>((ObjectBinder<TaskAgentQueue>) new TaskAgentQueueBinder3((TaskResourceComponent) component));
            result.Queue = resultCollection.GetCurrent<TaskAgentQueue>().FirstOrDefault<TaskAgentQueue>();
          }
          else
          {
            resultCollection.AddBinder<DeploymentGroup>((ObjectBinder<DeploymentGroup>) new DeploymentGroupBinder((TaskResourceComponent) component));
            result.MachineGroup = resultCollection.GetCurrent<DeploymentGroup>().FirstOrDefault<DeploymentGroup>();
            if (includeMachines && result.MachineGroup != null)
            {
              resultCollection.AddBinder<TaskAgentTag>((ObjectBinder<TaskAgentTag>) new TaskAgentTagBinder());
              resultCollection.NextResult();
              result.MachineGroup.Machines.AddRange<DeploymentMachine, IList<DeploymentMachine>>(TaskResourceComponent25.CreateDeploymentMachinesWithTags((IEnumerable<TaskAgentTag>) resultCollection.GetCurrent<TaskAgentTag>().Items));
            }
          }
          agentQueueAsync = result;
        }
      }
      return agentQueueAsync;
    }

    public override GetAgentQueuesResult GetAgentQueues(
      Guid projectId,
      string queueName,
      TaskAgentQueueType queueType = TaskAgentQueueType.Automation,
      bool includeMachines = false,
      bool includeTags = true,
      string lastQueueName = null,
      int maxQueuesCount = 10000)
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (GetAgentQueues)))
      {
        this.PrepareStoredProcedure("Task.prc_GetQueues");
        this.BindInt("@dataspaceId", this.GetDataspaceId(projectId, "DistributedTask", true));
        this.BindString("@queueName", queueName, 260, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
        this.BindByte("@queueType", (byte) queueType);
        this.BindBoolean("@includeAgentTags", includeMachines);
        GetAgentQueuesResult agentQueues = new GetAgentQueuesResult();
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          if (queueType == TaskAgentQueueType.Automation)
          {
            resultCollection.AddBinder<TaskAgentQueue>((ObjectBinder<TaskAgentQueue>) new TaskAgentQueueBinder3((TaskResourceComponent) this));
            agentQueues.Queues.AddRange<TaskAgentQueue, IList<TaskAgentQueue>>((IEnumerable<TaskAgentQueue>) resultCollection.GetCurrent<TaskAgentQueue>().Items);
          }
          else
          {
            resultCollection.AddBinder<DeploymentGroup>((ObjectBinder<DeploymentGroup>) new DeploymentGroupBinder((TaskResourceComponent) this));
            agentQueues.MachineGroups.AddRange<DeploymentGroup, IList<DeploymentGroup>>((IEnumerable<DeploymentGroup>) resultCollection.GetCurrent<DeploymentGroup>().Items);
            if (includeMachines && agentQueues.MachineGroups.Count > 0)
            {
              resultCollection.AddBinder<TaskAgentTag>((ObjectBinder<TaskAgentTag>) new TaskAgentTagBinder());
              resultCollection.NextResult();
              ILookup<int, TaskAgentTag> lookup = resultCollection.GetCurrent<TaskAgentTag>().Items.ToLookup<TaskAgentTag, int>((System.Func<TaskAgentTag, int>) (t => t.QueueId));
              foreach (DeploymentGroup machineGroup in (IEnumerable<DeploymentGroup>) agentQueues.MachineGroups)
                machineGroup.Machines.AddRange<DeploymentMachine, IList<DeploymentMachine>>(TaskResourceComponent25.CreateDeploymentMachinesWithTags(lookup[machineGroup.Id]));
            }
          }
          return agentQueues;
        }
      }
    }

    public override async Task<RequestAgentsUpdateResult> RequestAgentsUpdateAsync(
      int poolId,
      IEnumerable<int> agentIds,
      string targetVersion,
      Guid requestedBy,
      string currentState,
      TaskAgentUpdateReasonData reasonData)
    {
      TaskResourceComponent32 component = this;
      RequestAgentsUpdateResult agentsUpdateResult;
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) component, nameof (RequestAgentsUpdateAsync)))
      {
        component.PrepareStoredProcedure("Task.prc_RequestAgentsUpdate");
        component.BindInt("@poolId", poolId);
        component.BindInt32Table("@agentIds", agentIds);
        component.BindString("@targetVersion", targetVersion, 128, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
        component.BindGuid("@requestedBy", requestedBy);
        component.BindString("@currentState", currentState, 512, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
        RequestAgentsUpdateResult result = new RequestAgentsUpdateResult();
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) await component.ExecuteReaderAsync(), component.ProcedureName, component.RequestContext))
        {
          resultCollection.AddBinder<TaskAgent>(component.CreateTaskAgentBinder());
          result.NewUpdates.AddRange((IEnumerable<TaskAgent>) resultCollection.GetCurrent<TaskAgent>().Items);
          agentsUpdateResult = result;
        }
      }
      return agentsUpdateResult;
    }

    public override TaskAgent UpdateAgentUpdateState(int poolId, int agentId, string currentState)
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (UpdateAgentUpdateState)))
      {
        this.PrepareStoredProcedure("Task.prc_UpdateAgentUpdateState");
        this.BindInt("@poolId", poolId);
        this.BindInt("@agentId", agentId);
        this.BindString("@currentState", currentState, 512, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<TaskAgent>(this.CreateTaskAgentBinder());
          return resultCollection.GetCurrent<TaskAgent>().First<TaskAgent>();
        }
      }
    }

    public override TaskAgent FinishAgentUpdate(
      int poolId,
      int agentId,
      TaskResult updateResult,
      string currentState)
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (FinishAgentUpdate)))
      {
        this.PrepareStoredProcedure("Task.prc_FinishAgentUpdate");
        this.BindInt("@poolId", poolId);
        this.BindInt("@agentId", agentId);
        this.BindInt("@result", (int) (byte) updateResult);
        this.BindString("@currentState", currentState, 512, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<TaskAgent>(this.CreateTaskAgentBinder());
          return resultCollection.GetCurrent<TaskAgent>().First<TaskAgent>();
        }
      }
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
          agentSession.RecalculateRequestMatches = resultCollection.GetCurrent<bool>().First<bool>();
          resultCollection.NextResult();
          agentSession.Agent = resultCollection.GetCurrent<TaskAgent>().FirstOrDefault<TaskAgent>();
          resultCollection.NextResult();
          agentSession.OldSession = resultCollection.GetCurrent<TaskAgentSessionData>().FirstOrDefault<TaskAgentSessionData>();
          resultCollection.NextResult();
          agentSession.NewSession = resultCollection.GetCurrent<TaskAgentSessionData>().FirstOrDefault<TaskAgentSessionData>();
          return agentSession;
        }
      }
    }

    public override TaskAgentList GetAgents(
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
        this.PrepareStoredProcedure("Task.prc_GetAgents");
        this.BindInt("@poolId", poolId);
        this.BindString("@agentName", agentName, 128, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
        this.BindBoolean("@includeCapabilities", includeCapabilities);
        this.BindBoolean("@includeAssignedRequest", includeAssignedRequest);
        if (capabilityFilters != null)
        {
          HashSet<string> rows = new HashSet<string>(capabilityFilters, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
          rows.Remove(PipelineConstants.AgentName);
          rows.Remove(PipelineConstants.AgentVersionDemandName);
          this.BindStringTable("@capabilityFilters", (IEnumerable<string>) rows);
        }
        else
          this.BindStringTable("@capabilityFilters", (IEnumerable<string>) null);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<TaskAgent>(this.CreateTaskAgentBinder());
          if (includeCapabilities)
            resultCollection.AddBinder<TaskAgentCapability>((ObjectBinder<TaskAgentCapability>) new TaskAgentCapabilityBinder());
          if (includeAssignedRequest)
            resultCollection.AddBinder<TaskAgentJobRequest>(this.CreateTaskAgentRequestBinder(poolId));
          List<TaskAgent> items = resultCollection.GetCurrent<TaskAgent>().Items;
          if (includeAssignedRequest | includeCapabilities)
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
          }
          return new TaskAgentList(new bool?(), (IList<TaskAgent>) items);
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
      TaskResourceComponent32 component = this;
      TaskAgentList agentsAsync;
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) component, nameof (GetAgentsAsync)))
      {
        component.PrepareStoredProcedure("Task.prc_GetAgents");
        component.BindInt("@poolId", poolId);
        component.BindString("@agentName", agentName, 128, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
        component.BindBoolean("@includeCapabilities", includeCapabilities);
        component.BindBoolean("@includeAssignedRequest", includeAssignedRequest);
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
          List<TaskAgent> items = resultCollection.GetCurrent<TaskAgent>().Items;
          if (includeAssignedRequest | includeCapabilities)
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
          }
          agentsAsync = new TaskAgentList(new bool?(), (IList<TaskAgent>) items);
        }
      }
      return agentsAsync;
    }
  }
}
