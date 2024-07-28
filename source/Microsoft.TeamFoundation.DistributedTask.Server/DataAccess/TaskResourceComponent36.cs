// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.DataAccess.TaskResourceComponent36
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

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
  internal class TaskResourceComponent36 : TaskResourceComponent35
  {
    public override DeploymentMachine AddDeploymentMachine(
      Guid projectId,
      int deploymentGroupId,
      DeploymentMachine deploymentMachine)
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (AddDeploymentMachine)))
      {
        this.PrepareStoredProcedure("Task.prc_AddDeploymentMachine");
        this.BindInt("@dataspaceId", this.GetDataspaceId(projectId, "DistributedTask", true));
        this.BindInt("@deploymentGroupId", deploymentGroupId);
        this.BindInt("@agentId", deploymentMachine.Agent.Id);
        this.BindStringTable("@tags", (IEnumerable<string>) deploymentMachine.Tags);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<QueueAgent>((ObjectBinder<QueueAgent>) new QueueAgentBinder((TaskResourceComponent) this));
          resultCollection.AddBinder<QueueAgentTag>((ObjectBinder<QueueAgentTag>) new QueueAgentTagBinder());
          QueueAgent queueAgent = resultCollection.GetCurrent<QueueAgent>().First<QueueAgent>();
          resultCollection.NextResult();
          ILookup<int, string> lookup = resultCollection.GetCurrent<QueueAgentTag>().Items.ToLookup<QueueAgentTag, int, string>((System.Func<QueueAgentTag, int>) (t => t.MappingId), (System.Func<QueueAgentTag, string>) (t => t.Tag));
          return this.CreateDeploymentMachinesWithTags((IEnumerable<QueueAgent>) new QueueAgent[1]
          {
            queueAgent
          }, lookup, deploymentGroupId).First<DeploymentMachine>();
        }
      }
    }

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
        this.BindBoolean("@includeAgents", includeMachines);
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
              resultCollection.AddBinder<QueueAgent>((ObjectBinder<QueueAgent>) new QueueAgentBinder((TaskResourceComponent) this));
              resultCollection.AddBinder<QueueAgentTag>((ObjectBinder<QueueAgentTag>) new QueueAgentTagBinder());
              resultCollection.NextResult();
              List<QueueAgent> items = resultCollection.GetCurrent<QueueAgent>().Items;
              resultCollection.NextResult();
              ILookup<int, string> lookup = resultCollection.GetCurrent<QueueAgentTag>().Items.ToLookup<QueueAgentTag, int, string>((System.Func<QueueAgentTag, int>) (t => t.MappingId), (System.Func<QueueAgentTag, string>) (t => t.Tag));
              agentQueue.MachineGroup.Machines.AddRange<DeploymentMachine, IList<DeploymentMachine>>(this.CreateDeploymentMachinesWithTags((IEnumerable<QueueAgent>) items, lookup, queueId));
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
      TaskResourceComponent36 component = this;
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
        component.BindBoolean("@includeAgents", includeMachines);
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
              resultCollection.AddBinder<QueueAgent>((ObjectBinder<QueueAgent>) new QueueAgentBinder((TaskResourceComponent) component));
              resultCollection.AddBinder<QueueAgentTag>((ObjectBinder<QueueAgentTag>) new QueueAgentTagBinder());
              resultCollection.NextResult();
              List<QueueAgent> items = resultCollection.GetCurrent<QueueAgent>().Items;
              resultCollection.NextResult();
              ILookup<int, string> lookup = resultCollection.GetCurrent<QueueAgentTag>().Items.ToLookup<QueueAgentTag, int, string>((System.Func<QueueAgentTag, int>) (t => t.MappingId), (System.Func<QueueAgentTag, string>) (t => t.Tag));
              result.MachineGroup.Machines.AddRange<DeploymentMachine, IList<DeploymentMachine>>(component.CreateDeploymentMachinesWithTags((IEnumerable<QueueAgent>) items, lookup, queueId));
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
        this.BindBoolean("@includeAgents", includeMachines);
        this.BindString("@lastQueueName", lastQueueName, 260, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
        this.BindInt("@maxQueuesCount", maxQueuesCount);
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
              resultCollection.AddBinder<QueueAgent>((ObjectBinder<QueueAgent>) new QueueAgentBinder((TaskResourceComponent) this));
              resultCollection.AddBinder<QueueAgentTag>((ObjectBinder<QueueAgentTag>) new QueueAgentTagBinder());
              resultCollection.NextResult();
              List<QueueAgent> items = resultCollection.GetCurrent<QueueAgent>().Items;
              resultCollection.NextResult();
              ILookup<int, string> lookup = resultCollection.GetCurrent<QueueAgentTag>().Items.ToLookup<QueueAgentTag, int, string>((System.Func<QueueAgentTag, int>) (t => t.MappingId), (System.Func<QueueAgentTag, string>) (t => t.Tag));
              foreach (DeploymentGroup machineGroup in (IEnumerable<DeploymentGroup>) agentQueues.MachineGroups)
                machineGroup.Machines.AddRange<DeploymentMachine, IList<DeploymentMachine>>(this.CreateDeploymentMachinesWithTags((IEnumerable<QueueAgent>) items, lookup, machineGroup.Id));
            }
          }
          return agentQueues;
        }
      }
    }

    public override GetDeploymentMachinesResult GetDeploymentMachines(
      Guid projectId,
      int machineGroupId,
      IEnumerable<string> tagFilters)
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (GetDeploymentMachines)))
      {
        this.PrepareStoredProcedure("Task.prc_GetDeploymentMachines");
        this.BindInt("@machineGroupId", machineGroupId);
        this.BindInt("@dataspaceId", this.GetDataspaceId(projectId, "DistributedTask", true));
        this.BindStringTable("@tagFilters", tagFilters != null ? tagFilters.Distinct<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) : (IEnumerable<string>) null);
        GetDeploymentMachinesResult deploymentMachines = new GetDeploymentMachinesResult();
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<QueueAgent>((ObjectBinder<QueueAgent>) new QueueAgentBinder((TaskResourceComponent) this));
          resultCollection.AddBinder<QueueAgentTag>((ObjectBinder<QueueAgentTag>) new QueueAgentTagBinder());
          resultCollection.AddBinder<DeploymentGroup>((ObjectBinder<DeploymentGroup>) new DeploymentGroupBinder((TaskResourceComponent) this));
          List<QueueAgent> items = resultCollection.GetCurrent<QueueAgent>().Items;
          resultCollection.NextResult();
          ILookup<int, string> lookup = resultCollection.GetCurrent<QueueAgentTag>().Items.ToLookup<QueueAgentTag, int, string>((System.Func<QueueAgentTag, int>) (t => t.MappingId), (System.Func<QueueAgentTag, string>) (t => t.Tag));
          deploymentMachines.Machines = this.CreateDeploymentMachinesWithTags((IEnumerable<QueueAgent>) items, lookup, machineGroupId);
          resultCollection.NextResult();
          deploymentMachines.MachineGroup = resultCollection.GetCurrent<DeploymentGroup>().FirstOrDefault<DeploymentGroup>((System.Func<DeploymentGroup, bool>) (d => d.Id == machineGroupId));
        }
        return deploymentMachines;
      }
    }

    public override async Task<GetDeploymentMachinesResult> GetDeploymentMachinesAsync(
      Guid projectId,
      int machineGroupId,
      IEnumerable<string> tagFilters)
    {
      TaskResourceComponent36 component1 = this;
      GetDeploymentMachinesResult deploymentMachinesAsync;
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) component1, nameof (GetDeploymentMachinesAsync)))
      {
        component1.PrepareStoredProcedure("Task.prc_GetDeploymentMachines");
        component1.BindInt("@machineGroupId", machineGroupId);
        component1.BindInt("@dataspaceId", component1.GetDataspaceId(projectId, "DistributedTask", true));
        TaskResourceComponent36 component2 = component1;
        IEnumerable<string> source = tagFilters;
        IEnumerable<string> rows = source != null ? source.Distinct<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) : (IEnumerable<string>) null;
        component2.BindStringTable("@tagFilters", rows);
        GetDeploymentMachinesResult result = new GetDeploymentMachinesResult();
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) await component1.ExecuteReaderAsync(), component1.ProcedureName, component1.RequestContext))
        {
          resultCollection.AddBinder<QueueAgent>((ObjectBinder<QueueAgent>) new QueueAgentBinder((TaskResourceComponent) component1));
          resultCollection.AddBinder<QueueAgentTag>((ObjectBinder<QueueAgentTag>) new QueueAgentTagBinder());
          resultCollection.AddBinder<DeploymentGroup>((ObjectBinder<DeploymentGroup>) new DeploymentGroupBinder((TaskResourceComponent) component1));
          List<QueueAgent> items = resultCollection.GetCurrent<QueueAgent>().Items;
          resultCollection.NextResult();
          ILookup<int, string> lookup = resultCollection.GetCurrent<QueueAgentTag>().Items.ToLookup<QueueAgentTag, int, string>((System.Func<QueueAgentTag, int>) (t => t.MappingId), (System.Func<QueueAgentTag, string>) (t => t.Tag));
          result.Machines = component1.CreateDeploymentMachinesWithTags((IEnumerable<QueueAgent>) items, lookup, machineGroupId);
          resultCollection.NextResult();
          result.MachineGroup = resultCollection.GetCurrent<DeploymentGroup>().FirstOrDefault<DeploymentGroup>((System.Func<DeploymentGroup, bool>) (d => d.Id == machineGroupId));
        }
        deploymentMachinesAsync = result;
      }
      return deploymentMachinesAsync;
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
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          UpdateAgentPoolResult updateAgentPoolResult = new UpdateAgentPoolResult();
          resultCollection.AddBinder<TaskAgentPoolData>(this.CreateTaskAgentPoolBinder());
          updateAgentPoolResult.UpdatedPoolData = resultCollection.GetCurrent<TaskAgentPoolData>().FirstOrDefault<TaskAgentPoolData>();
          return updateAgentPoolResult;
        }
      }
    }

    protected override ObjectBinder<TaskAgentPoolData> CreateTaskAgentPoolBinder() => (ObjectBinder<TaskAgentPoolData>) new TaskAgentPoolBinder7(this.RequestContext);

    public override DeploymentMachine DeleteDeploymentMachine(
      Guid projectId,
      int deploymentGroupId,
      int deploymentMachineId)
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (DeleteDeploymentMachine)))
      {
        this.PrepareStoredProcedure("Task.prc_DeleteDeploymentMachine");
        this.BindInt("@dataspaceId", this.GetDataspaceId(projectId, "DistributedTask", true));
        this.BindInt("@deploymentGroupId", deploymentGroupId);
        this.BindInt("@machineId", deploymentMachineId);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<QueueAgent>((ObjectBinder<QueueAgent>) new QueueAgentBinder((TaskResourceComponent) this));
          resultCollection.AddBinder<QueueAgentTag>((ObjectBinder<QueueAgentTag>) new QueueAgentTagBinder());
          QueueAgent queueAgent = resultCollection.GetCurrent<QueueAgent>().FirstOrDefault<QueueAgent>();
          if (queueAgent == null)
            return (DeploymentMachine) null;
          resultCollection.NextResult();
          ILookup<int, string> lookup = resultCollection.GetCurrent<QueueAgentTag>().Items.ToLookup<QueueAgentTag, int, string>((System.Func<QueueAgentTag, int>) (t => t.MappingId), (System.Func<QueueAgentTag, string>) (t => t.Tag));
          return this.CreateDeploymentMachinesWithTags((IEnumerable<QueueAgent>) new QueueAgent[1]
          {
            queueAgent
          }, lookup, deploymentGroupId).FirstOrDefault<DeploymentMachine>();
        }
      }
    }

    public override UpdateDeploymentMachinesResult UpdateDeploymentMachines(
      Guid projectId,
      int machineGroupId,
      IEnumerable<DeploymentMachine> machines)
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (UpdateDeploymentMachines)))
      {
        IList<int> machineIdsForTagDeletion = (IList<int>) null;
        UpdateDeploymentMachinesResult deploymentMachinesResult = new UpdateDeploymentMachinesResult();
        IList<KeyValuePair<int, string>> machinesTagsTable = this.GetDeploymentMachinesTagsTable(machines, out machineIdsForTagDeletion);
        this.PrepareStoredProcedure("Task.prc_UpdateDeploymentMachines");
        this.BindInt("@dataspaceId", this.GetDataspaceId(projectId, "DistributedTask", true));
        this.BindInt("@deploymentGroupId", machineGroupId);
        this.BindKeyValuePairInt32StringTable("@tagsTable", (IEnumerable<KeyValuePair<int, string>>) machinesTagsTable);
        this.BindUniqueInt32Table("@machineIdsForTagDeletion", (IEnumerable<int>) machineIdsForTagDeletion);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<QueueAgent>((ObjectBinder<QueueAgent>) new QueueAgentBinder((TaskResourceComponent) this));
          resultCollection.AddBinder<QueueAgentTag>((ObjectBinder<QueueAgentTag>) new QueueAgentTagBinder());
          resultCollection.AddBinder<DeploymentGroup>((ObjectBinder<DeploymentGroup>) new DeploymentGroupBinder((TaskResourceComponent) this));
          List<QueueAgent> items = resultCollection.GetCurrent<QueueAgent>().Items;
          resultCollection.NextResult();
          ILookup<int, string> lookup = resultCollection.GetCurrent<QueueAgentTag>().Items.ToLookup<QueueAgentTag, int, string>((System.Func<QueueAgentTag, int>) (t => t.MappingId), (System.Func<QueueAgentTag, string>) (t => t.Tag));
          resultCollection.NextResult();
          deploymentMachinesResult.MachineGroup = resultCollection.GetCurrent<DeploymentGroup>().First<DeploymentGroup>((System.Func<DeploymentGroup, bool>) (d => d.Id == machineGroupId));
          deploymentMachinesResult.Machines = this.CreateDeploymentMachinesChangedDataWithTags((IEnumerable<QueueAgent>) items, lookup, machineGroupId);
        }
        return deploymentMachinesResult;
      }
    }

    protected virtual IEnumerable<DeploymentMachine> CreateDeploymentMachinesWithTags(
      IEnumerable<QueueAgent> mappings,
      ILookup<int, string> tagsWithMappingIdLookup,
      int queueId)
    {
      return mappings.Where<QueueAgent>((System.Func<QueueAgent, bool>) (m => m.QueueId == queueId)).Select<QueueAgent, DeploymentMachine>((System.Func<QueueAgent, DeploymentMachine>) (m =>
      {
        return new DeploymentMachine()
        {
          Id = m.QueueAgentId,
          Agent = new TaskAgent() { Id = m.AgentId },
          Tags = (IList<string>) tagsWithMappingIdLookup[m.QueueAgentId].ToList<string>()
        };
      }));
    }

    protected virtual IEnumerable<DeploymentMachineChangedData> CreateDeploymentMachinesChangedDataWithTags(
      IEnumerable<QueueAgent> mappings,
      ILookup<int, string> tagsWithMappingIdLookup,
      int queueId)
    {
      return mappings.Where<QueueAgent>((System.Func<QueueAgent, bool>) (m => m.QueueId == queueId)).Select<QueueAgent, DeploymentMachineChangedData>((System.Func<QueueAgent, DeploymentMachineChangedData>) (m =>
      {
        return new DeploymentMachineChangedData()
        {
          Id = m.QueueAgentId,
          Agent = new TaskAgent() { Id = m.AgentId },
          Tags = (IList<string>) tagsWithMappingIdLookup[m.QueueAgentId].ToList<string>()
        };
      }));
    }

    protected virtual IList<KeyValuePair<int, string>> GetDeploymentMachinesTagsTable(
      IEnumerable<DeploymentMachine> deploymentMachines,
      out IList<int> machineIdsForTagDeletion)
    {
      List<KeyValuePair<int, string>> machinesTagsTable = new List<KeyValuePair<int, string>>();
      machineIdsForTagDeletion = (IList<int>) new List<int>();
      if (deploymentMachines != null)
      {
        foreach (DeploymentMachine deploymentMachine in deploymentMachines)
        {
          DeploymentMachine machine = deploymentMachine;
          if (machine != null && machine.Tags != null)
          {
            if (machine.Tags.Count > 0)
              machinesTagsTable.AddRange(machine.Tags.Select<string, KeyValuePair<int, string>>((System.Func<string, KeyValuePair<int, string>>) (t => new KeyValuePair<int, string>(machine.Id, t))));
            else
              machineIdsForTagDeletion.Add(machine.Id);
          }
        }
      }
      return (IList<KeyValuePair<int, string>>) machinesTagsTable;
    }
  }
}
