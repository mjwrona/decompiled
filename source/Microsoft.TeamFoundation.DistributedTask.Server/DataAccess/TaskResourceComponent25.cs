// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.DataAccess.TaskResourceComponent25
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
  internal class TaskResourceComponent25 : TaskResourceComponent24
  {
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
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<TaskAgentPoolData>(this.CreateTaskAgentPoolBinder());
          return resultCollection.GetCurrent<TaskAgentPoolData>().First<TaskAgentPoolData>();
        }
      }
    }

    public override async Task<IEnumerable<TaskAgentPoolData>> GetAgentPoolsAsync(
      string poolName,
      TaskAgentPoolType poolType = TaskAgentPoolType.Automation)
    {
      TaskResourceComponent25 component = this;
      IEnumerable<TaskAgentPoolData> items;
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) component, nameof (GetAgentPoolsAsync)))
      {
        component.PrepareStoredProcedure("Task.prc_GetAgentPools");
        component.BindString("@poolName", poolName, 128, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
        component.BindByte("@poolType", (byte) poolType);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) await component.ExecuteReaderAsync(), component.ProcedureName, component.RequestContext))
        {
          resultCollection.AddBinder<TaskAgentPoolData>(component.CreateTaskAgentPoolBinder());
          items = (IEnumerable<TaskAgentPoolData>) resultCollection.GetCurrent<TaskAgentPoolData>().Items;
        }
      }
      return items;
    }

    public override IEnumerable<TaskAgentPoolData> GetAgentPools(
      string poolName,
      TaskAgentPoolType poolType = TaskAgentPoolType.Automation)
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (GetAgentPools)))
      {
        this.PrepareStoredProcedure("Task.prc_GetAgentPools");
        this.BindString("@poolName", poolName, 128, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
        this.BindByte("@poolType", (byte) poolType);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<TaskAgentPoolData>(this.CreateTaskAgentPoolBinder());
          return (IEnumerable<TaskAgentPoolData>) resultCollection.GetCurrent<TaskAgentPoolData>().Items;
        }
      }
    }

    public override AgentQueueOrMachineGroup AddAgentQueue(
      Guid projectId,
      string name,
      int poolId,
      TaskAgentQueueType queueType = TaskAgentQueueType.Automation,
      string description = null)
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (AddAgentQueue)))
      {
        this.PrepareStoredProcedure("Task.prc_AddQueue");
        this.BindInt("@dataspaceId", this.GetDataspaceId(projectId, "DistributedTask", true));
        this.BindString("@queueName", name, 260, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
        this.BindByte("@queueType", (byte) queueType);
        this.BindInt("@poolId", poolId);
        this.BindGuid("@writerId", this.Author);
        AgentQueueOrMachineGroup queueOrMachineGroup = new AgentQueueOrMachineGroup();
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          if (queueType == TaskAgentQueueType.Automation)
          {
            resultCollection.AddBinder<TaskAgentQueue>((ObjectBinder<TaskAgentQueue>) new TaskAgentQueueBinder3((TaskResourceComponent) this));
            queueOrMachineGroup.Queue = resultCollection.GetCurrent<TaskAgentQueue>().FirstOrDefault<TaskAgentQueue>();
          }
          else
          {
            resultCollection.AddBinder<DeploymentGroup>((ObjectBinder<DeploymentGroup>) new DeploymentGroupBinder((TaskResourceComponent) this));
            queueOrMachineGroup.MachineGroup = resultCollection.GetCurrent<DeploymentGroup>().FirstOrDefault<DeploymentGroup>();
          }
          return queueOrMachineGroup;
        }
      }
    }

    public override DeleteAgentQueueResult DeleteAgentQueue(
      Guid projectId,
      int queueId,
      TaskAgentQueueType queueType = TaskAgentQueueType.Automation)
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (DeleteAgentQueue)))
      {
        this.PrepareStoredProcedure("Task.prc_DeleteQueue");
        this.BindInt("@dataspaceId", this.GetDataspaceId(projectId, "DistributedTask", true));
        this.BindInt("@queueId", queueId);
        this.BindByte("@queueType", (byte) queueType);
        this.BindGuid("@writerId", this.Author);
        DeleteAgentQueueResult agentQueueResult = new DeleteAgentQueueResult();
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          if (queueType == TaskAgentQueueType.Automation)
          {
            resultCollection.AddBinder<TaskAgentQueue>((ObjectBinder<TaskAgentQueue>) new TaskAgentQueueBinder3((TaskResourceComponent) this));
            resultCollection.AddBinder<bool>((ObjectBinder<bool>) new BooleanBinder());
            agentQueueResult.Queue = resultCollection.GetCurrent<TaskAgentQueue>().FirstOrDefault<TaskAgentQueue>();
          }
          else
          {
            resultCollection.AddBinder<DeploymentGroup>((ObjectBinder<DeploymentGroup>) new DeploymentGroupBinder((TaskResourceComponent) this));
            resultCollection.AddBinder<bool>((ObjectBinder<bool>) new BooleanBinder());
            agentQueueResult.MachineGroup = resultCollection.GetCurrent<DeploymentGroup>().FirstOrDefault<DeploymentGroup>();
          }
          resultCollection.NextResult();
          agentQueueResult.PoolUnreferenced = resultCollection.GetCurrent<bool>().FirstOrDefault<bool>();
          return agentQueueResult;
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
            resultCollection.AddBinder<TaskAgentTag>((ObjectBinder<TaskAgentTag>) new TaskAgentTagBinder());
            agentQueue.MachineGroup = resultCollection.GetCurrent<DeploymentGroup>().FirstOrDefault<DeploymentGroup>();
            if (agentQueue.MachineGroup != null)
            {
              resultCollection.NextResult();
              List<TaskAgentTag> items = resultCollection.GetCurrent<TaskAgentTag>().Items;
              agentQueue.MachineGroup.Machines.AddRange<DeploymentMachine, IList<DeploymentMachine>>(TaskResourceComponent25.CreateDeploymentMachinesWithTags((IEnumerable<TaskAgentTag>) items));
            }
          }
          return agentQueue;
        }
      }
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
            resultCollection.AddBinder<TaskAgentTag>((ObjectBinder<TaskAgentTag>) new TaskAgentTagBinder());
            agentQueues.MachineGroups.AddRange<DeploymentGroup, IList<DeploymentGroup>>((IEnumerable<DeploymentGroup>) resultCollection.GetCurrent<DeploymentGroup>().Items);
            if (agentQueues.MachineGroups.Count > 0)
            {
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

    public override GetDeploymentMachinesResult GetDeploymentMachines(
      Guid projectId,
      int machineGroupId,
      IEnumerable<string> tagFilters)
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (GetDeploymentMachines)))
      {
        GetDeploymentMachinesResult deploymentMachines = new GetDeploymentMachinesResult();
        this.PrepareStoredProcedure("Task.prc_GetDeploymentMachines");
        this.BindInt("@machineGroupId", machineGroupId);
        this.BindInt("@dataspaceId", this.GetDataspaceId(projectId, "DistributedTask", true));
        this.BindStringTable("@tagFilters", tagFilters != null ? tagFilters.Distinct<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) : (IEnumerable<string>) null);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<TaskAgentTag>((ObjectBinder<TaskAgentTag>) new TaskAgentTagBinder());
          resultCollection.AddBinder<DeploymentGroup>((ObjectBinder<DeploymentGroup>) new DeploymentGroupBinder((TaskResourceComponent) this));
          List<TaskAgentTag> items = resultCollection.GetCurrent<TaskAgentTag>().Items;
          deploymentMachines.Machines = TaskResourceComponent25.CreateDeploymentMachinesWithTags((IEnumerable<TaskAgentTag>) items);
          resultCollection.NextResult();
          deploymentMachines.MachineGroup = resultCollection.GetCurrent<DeploymentGroup>().FirstOrDefault<DeploymentGroup>();
        }
        return deploymentMachines;
      }
    }

    public override async Task<GetDeploymentMachinesResult> GetDeploymentMachinesAsync(
      Guid projectId,
      int machineGroupId,
      IEnumerable<string> tagFilters)
    {
      TaskResourceComponent25 component1 = this;
      GetDeploymentMachinesResult deploymentMachinesAsync;
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) component1, nameof (GetDeploymentMachinesAsync)))
      {
        component1.PrepareStoredProcedure("Task.prc_GetDeploymentMachines");
        component1.BindInt("@machineGroupId", machineGroupId);
        component1.BindInt("@dataspaceId", component1.GetDataspaceId(projectId, "DistributedTask", true));
        TaskResourceComponent25 component2 = component1;
        IEnumerable<string> source = tagFilters;
        IEnumerable<string> rows = source != null ? source.Distinct<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) : (IEnumerable<string>) null;
        component2.BindStringTable("@tagFilters", rows);
        GetDeploymentMachinesResult result = new GetDeploymentMachinesResult();
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) await component1.ExecuteReaderAsync(), component1.ProcedureName, component1.RequestContext))
        {
          resultCollection.AddBinder<TaskAgentTag>((ObjectBinder<TaskAgentTag>) new TaskAgentTagBinder());
          resultCollection.AddBinder<DeploymentGroup>((ObjectBinder<DeploymentGroup>) new DeploymentGroupBinder((TaskResourceComponent) component1));
          result.Machines = TaskResourceComponent25.CreateDeploymentMachinesWithTags((IEnumerable<TaskAgentTag>) resultCollection.GetCurrent<TaskAgentTag>().Items);
          resultCollection.NextResult();
          result.MachineGroup = resultCollection.GetCurrent<DeploymentGroup>().FirstOrDefault<DeploymentGroup>();
        }
        deploymentMachinesAsync = result;
      }
      return deploymentMachinesAsync;
    }

    public override UpdateDeploymentMachinesResult UpdateDeploymentMachines(
      Guid projectId,
      int machineGroupId,
      IEnumerable<DeploymentMachine> machines)
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (UpdateDeploymentMachines)))
      {
        IList<int> agentIdsForTagDeletion = (IList<int>) null;
        IList<TaskAgentTag> tagsTable = (IList<TaskAgentTag>) null;
        IList<KeyValuePair<int, string>> taskAgentTagsTable = this.GetTaskAgentTagsTable(machines, out agentIdsForTagDeletion);
        this.PrepareStoredProcedure("Task.prc_UpdateDeploymentMachines");
        this.BindInt("@machineGroupId", machineGroupId);
        this.BindInt("@dataspaceId", this.GetDataspaceId(projectId, "DistributedTask", true));
        this.BindKeyValuePairInt32StringTable("@tagsTable", (IEnumerable<KeyValuePair<int, string>>) taskAgentTagsTable);
        this.BindUniqueInt32Table("@agentIdsForTagDeletion", (IEnumerable<int>) agentIdsForTagDeletion);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<TaskAgentTag>((ObjectBinder<TaskAgentTag>) new TaskAgentTagBinder());
          tagsTable = (IList<TaskAgentTag>) resultCollection.GetCurrent<TaskAgentTag>().Items;
        }
        DeploymentGroup machineGroup = this.GetAgentQueue(projectId, machineGroupId, TaskAgentQueueType.Deployment, true, false).MachineGroup;
        UpdateDeploymentMachinesResult deploymentMachinesResult = new UpdateDeploymentMachinesResult();
        deploymentMachinesResult.Machines = TaskResourceComponent25.CreateDeploymentMachinesChangedDataWithTags((IEnumerable<TaskAgentTag>) tagsTable);
        deploymentMachinesResult.MachineGroup = machineGroup;
        deploymentMachinesResult = deploymentMachinesResult;
        return deploymentMachinesResult;
      }
    }

    protected IList<KeyValuePair<int, string>> GetTaskAgentTagsTable(
      IEnumerable<DeploymentMachine> deploymentMachines,
      out IList<int> agentIdsForTagDeletion)
    {
      List<KeyValuePair<int, string>> taskAgentTagsTable = new List<KeyValuePair<int, string>>();
      agentIdsForTagDeletion = (IList<int>) new List<int>();
      if (deploymentMachines != null)
      {
        foreach (DeploymentMachine deploymentMachine in deploymentMachines)
        {
          DeploymentMachine machine = deploymentMachine;
          if (machine.Agent != null && machine.Tags != null)
          {
            if (machine.Tags.Count > 0)
              taskAgentTagsTable.AddRange(machine.Tags.Select<string, KeyValuePair<int, string>>((System.Func<string, KeyValuePair<int, string>>) (t => new KeyValuePair<int, string>(machine.Agent.Id, t))));
            else
              agentIdsForTagDeletion.Add(machine.Agent.Id);
          }
        }
      }
      return (IList<KeyValuePair<int, string>>) taskAgentTagsTable;
    }

    protected static IEnumerable<DeploymentMachine> CreateDeploymentMachinesWithTags(
      IEnumerable<TaskAgentTag> tagsTable)
    {
      return tagsTable.GroupBy<TaskAgentTag, int, string>((System.Func<TaskAgentTag, int>) (t => t.AgentId), (System.Func<TaskAgentTag, string>) (t => t.Tag)).Select<IGrouping<int, string>, DeploymentMachine>((System.Func<IGrouping<int, string>, DeploymentMachine>) (agentTags =>
      {
        return new DeploymentMachine()
        {
          Id = agentTags.Key,
          Agent = new TaskAgent() { Id = agentTags.Key },
          Tags = (IList<string>) agentTags.ToList<string>()
        };
      }));
    }

    protected static IEnumerable<DeploymentMachineChangedData> CreateDeploymentMachinesChangedDataWithTags(
      IEnumerable<TaskAgentTag> tagsTable)
    {
      return tagsTable.GroupBy<TaskAgentTag, int, string>((System.Func<TaskAgentTag, int>) (t => t.AgentId), (System.Func<TaskAgentTag, string>) (t => t.Tag)).Select<IGrouping<int, string>, DeploymentMachineChangedData>((System.Func<IGrouping<int, string>, DeploymentMachineChangedData>) (agentTags =>
      {
        return new DeploymentMachineChangedData()
        {
          Id = agentTags.Key,
          Agent = new TaskAgent() { Id = agentTags.Key },
          Tags = (IList<string>) agentTags.ToList<string>()
        };
      }));
    }
  }
}
