// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.DataAccess.TaskResourceComponent38
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.DistributedTask.Server.DataAccess
{
  internal class TaskResourceComponent38 : TaskResourceComponent37
  {
    protected static SqlMetaData[] typ_QueueAgentMapTable = new SqlMetaData[2]
    {
      new SqlMetaData("@QueueId", SqlDbType.Int),
      new SqlMetaData("@AgentId", SqlDbType.Int)
    };

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
        this.BindBoolean("@includeTags", includeTags);
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
            if (agentQueue.MachineGroup != null && includeMachines | includeTags)
            {
              resultCollection.AddBinder<QueueAgentTag>((ObjectBinder<QueueAgentTag>) new QueueAgentTagBinder());
              resultCollection.NextResult();
              List<QueueAgentTag> items1 = resultCollection.GetCurrent<QueueAgentTag>().Items;
              if (includeTags)
              {
                List<string> list = items1.Select<QueueAgentTag, string>((System.Func<QueueAgentTag, string>) (t => t.Tag)).Distinct<string>((IEqualityComparer<string>) StringComparer.CurrentCultureIgnoreCase).ToList<string>();
                agentQueue.MachineGroup.MachineTags = (IList<string>) list;
              }
              if (includeMachines)
              {
                resultCollection.AddBinder<QueueAgent>((ObjectBinder<QueueAgent>) new QueueAgentBinder((TaskResourceComponent) this));
                resultCollection.NextResult();
                List<QueueAgent> items2 = resultCollection.GetCurrent<QueueAgent>().Items;
                ILookup<int, string> lookup = items1.ToLookup<QueueAgentTag, int, string>((System.Func<QueueAgentTag, int>) (t => t.MappingId), (System.Func<QueueAgentTag, string>) (t => t.Tag));
                agentQueue.MachineGroup.Machines.AddRange<DeploymentMachine, IList<DeploymentMachine>>(this.CreateDeploymentMachinesWithTags((IEnumerable<QueueAgent>) items2, lookup));
              }
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
      TaskResourceComponent38 component = this;
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
        component.BindBoolean("@includeTags", includeTags);
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
            if (result.MachineGroup != null && includeMachines | includeTags)
            {
              resultCollection.AddBinder<QueueAgentTag>((ObjectBinder<QueueAgentTag>) new QueueAgentTagBinder());
              resultCollection.NextResult();
              List<QueueAgentTag> items1 = resultCollection.GetCurrent<QueueAgentTag>().Items;
              if (includeTags)
                result.MachineGroup.MachineTags = (IList<string>) items1.Select<QueueAgentTag, string>((System.Func<QueueAgentTag, string>) (t => t.Tag)).Distinct<string>().ToList<string>();
              if (includeMachines)
              {
                resultCollection.AddBinder<QueueAgent>((ObjectBinder<QueueAgent>) new QueueAgentBinder((TaskResourceComponent) component));
                resultCollection.NextResult();
                List<QueueAgent> items2 = resultCollection.GetCurrent<QueueAgent>().Items;
                ILookup<int, string> lookup = items1.ToLookup<QueueAgentTag, int, string>((System.Func<QueueAgentTag, int>) (t => t.MappingId), (System.Func<QueueAgentTag, string>) (t => t.Tag));
                result.MachineGroup.Machines.AddRange<DeploymentMachine, IList<DeploymentMachine>>(component.CreateDeploymentMachinesWithTags((IEnumerable<QueueAgent>) items2, lookup));
              }
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
        this.BindBoolean("@includeTags", includeTags);
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
              resultCollection.NextResult();
              IList<QueueAgent> queueAgentList = (IList<QueueAgent>) new List<QueueAgent>();
              queueAgentList.AddRange<QueueAgent, IList<QueueAgent>>((IEnumerable<QueueAgent>) resultCollection.GetCurrent<QueueAgent>().Items);
              IDictionary<int, IList<QueueAgent>> mappingsPerQueue = this.GetQueueAgentMappingsPerQueue(queueAgentList);
              ILookup<int, string> tagsWithMappingIdLookup = (ILookup<int, string>) null;
              if (includeTags)
              {
                resultCollection.AddBinder<QueueAgentTag>((ObjectBinder<QueueAgentTag>) new QueueAgentTagBinder());
                resultCollection.NextResult();
                tagsWithMappingIdLookup = resultCollection.GetCurrent<QueueAgentTag>().Items.ToLookup<QueueAgentTag, int, string>((System.Func<QueueAgentTag, int>) (t => t.MappingId), (System.Func<QueueAgentTag, string>) (t => t.Tag));
              }
              foreach (DeploymentGroup machineGroup in (IEnumerable<DeploymentGroup>) agentQueues.MachineGroups)
              {
                if (mappingsPerQueue.ContainsKey(machineGroup.Id))
                  machineGroup.Machines.AddRange<DeploymentMachine, IList<DeploymentMachine>>(this.CreateDeploymentMachinesWithTags((IEnumerable<QueueAgent>) mappingsPerQueue[machineGroup.Id], tagsWithMappingIdLookup));
              }
            }
          }
          return agentQueues;
        }
      }
    }

    public override GetAgentQueuesResult GetAgentQueuesById(
      Guid projectId,
      IEnumerable<int> deploymentGroupIds,
      TaskAgentQueueType queueType = TaskAgentQueueType.Automation,
      bool includeMachines = false,
      bool includeTags = false)
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (GetAgentQueuesById)))
      {
        this.PrepareStoredProcedure("Task.prc_GetQueuesById");
        this.BindInt("@dataspaceId", this.GetDataspaceId(projectId, "DistributedTask", true));
        this.BindByte("@queueType", (byte) queueType);
        this.BindUniqueInt32Table("@queueIds", deploymentGroupIds == null ? (IEnumerable<int>) null : (IEnumerable<int>) deploymentGroupIds.Distinct<int>().ToList<int>());
        this.BindBoolean("@includeAgents", includeMachines);
        this.BindBoolean("@includeTags", includeTags);
        GetAgentQueuesResult agentQueuesById = new GetAgentQueuesResult();
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          if (queueType == TaskAgentQueueType.Automation)
          {
            resultCollection.AddBinder<TaskAgentQueue>((ObjectBinder<TaskAgentQueue>) new TaskAgentQueueBinder3((TaskResourceComponent) this));
            agentQueuesById.Queues.AddRange<TaskAgentQueue, IList<TaskAgentQueue>>((IEnumerable<TaskAgentQueue>) resultCollection.GetCurrent<TaskAgentQueue>().Items);
          }
          else
          {
            resultCollection.AddBinder<DeploymentGroup>((ObjectBinder<DeploymentGroup>) new DeploymentGroupBinder((TaskResourceComponent) this));
            agentQueuesById.MachineGroups.AddRange<DeploymentGroup, IList<DeploymentGroup>>((IEnumerable<DeploymentGroup>) resultCollection.GetCurrent<DeploymentGroup>().Items);
            if (includeMachines && agentQueuesById.MachineGroups != null)
            {
              resultCollection.AddBinder<QueueAgentTag>((ObjectBinder<QueueAgentTag>) new QueueAgentTagBinder());
              resultCollection.NextResult();
              ILookup<int, string> lookup = resultCollection.GetCurrent<QueueAgentTag>().Items.ToLookup<QueueAgentTag, int, string>((System.Func<QueueAgentTag, int>) (t => t.MappingId), (System.Func<QueueAgentTag, string>) (t => t.Tag));
              resultCollection.AddBinder<QueueAgent>((ObjectBinder<QueueAgent>) new QueueAgentBinder((TaskResourceComponent) this));
              resultCollection.NextResult();
              IDictionary<int, IList<QueueAgent>> mappingsPerQueue = this.GetQueueAgentMappingsPerQueue((IList<QueueAgent>) resultCollection.GetCurrent<QueueAgent>().Items);
              foreach (DeploymentGroup machineGroup in (IEnumerable<DeploymentGroup>) agentQueuesById.MachineGroups)
              {
                if (mappingsPerQueue.ContainsKey(machineGroup.Id))
                  machineGroup.Machines.AddRange<DeploymentMachine, IList<DeploymentMachine>>(this.CreateDeploymentMachinesWithTags((IEnumerable<QueueAgent>) mappingsPerQueue[machineGroup.Id], lookup));
              }
            }
          }
          return agentQueuesById;
        }
      }
    }

    public override IEnumerable<DeploymentGroupMetrics> GetDeploymentGroupsMetrics(
      IDictionary<int, DeploymentGroup> deploymentGroups,
      Guid hostId = default (Guid),
      Guid scopeId = default (Guid))
    {
      List<DeploymentGroupMetrics> deploymentGroupsMetrics = new List<DeploymentGroupMetrics>();
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (GetDeploymentGroupsMetrics)))
      {
        this.PrepareStoredProcedure("Task.prc_GetDeploymentGroupsMetrics");
        this.BindDeploymentGroupAgentMappingTable("@queueAgents", deploymentGroups);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<DeploymentGroupMetrics>((ObjectBinder<DeploymentGroupMetrics>) new DeploymentGroupMetricsBinder(deploymentGroups));
          deploymentGroupsMetrics.AddRange((IEnumerable<DeploymentGroupMetrics>) resultCollection.GetCurrent<DeploymentGroupMetrics>().Items);
        }
      }
      return (IEnumerable<DeploymentGroupMetrics>) deploymentGroupsMetrics;
    }

    protected virtual IDictionary<int, IList<QueueAgent>> GetQueueAgentMappingsPerQueue(
      IList<QueueAgent> mappings)
    {
      Dictionary<int, IList<QueueAgent>> mappingsPerQueue = new Dictionary<int, IList<QueueAgent>>();
      foreach (QueueAgent mapping in (IEnumerable<QueueAgent>) mappings)
      {
        if (!mappingsPerQueue.ContainsKey(mapping.QueueId))
          mappingsPerQueue[mapping.QueueId] = (IList<QueueAgent>) new List<QueueAgent>();
        mappingsPerQueue[mapping.QueueId].Add(mapping);
      }
      return (IDictionary<int, IList<QueueAgent>>) mappingsPerQueue;
    }

    protected IEnumerable<DeploymentMachine> CreateDeploymentMachinesWithTags(
      IEnumerable<QueueAgent> mappings,
      ILookup<int, string> tagsWithMappingIdLookup)
    {
      return mappings.Select<QueueAgent, DeploymentMachine>((System.Func<QueueAgent, DeploymentMachine>) (m =>
      {
        return new DeploymentMachine()
        {
          Id = m.QueueAgentId,
          Agent = new TaskAgent() { Id = m.AgentId },
          Tags = tagsWithMappingIdLookup != null ? (IList<string>) tagsWithMappingIdLookup[m.QueueAgentId].ToList<string>() : (IList<string>) null
        };
      }));
    }

    protected SqlParameter BindDeploymentGroupAgentMappingTable(
      string parameterName,
      IDictionary<int, DeploymentGroup> rows)
    {
      return this.BindTable(parameterName, "Task.typ_QueueAgentMapTable", rows.SelectMany<KeyValuePair<int, DeploymentGroup>, int, SqlDataRecord>((System.Func<KeyValuePair<int, DeploymentGroup>, IEnumerable<int>>) (x =>
      {
        if (x.Value.Machines.Count > 0)
          return x.Value.Machines.Select<DeploymentMachine, int>((System.Func<DeploymentMachine, int>) (y => y.Agent.Id));
        return (IEnumerable<int>) new int[1]{ -1 };
      }), (Func<KeyValuePair<int, DeploymentGroup>, int, SqlDataRecord>) ((x, y) => this.ConvertToSqlDataRecord(x.Key, y))));
    }

    protected SqlDataRecord ConvertToSqlDataRecord(int queueId, int agentId)
    {
      SqlDataRecord record = new SqlDataRecord(TaskResourceComponent38.typ_QueueAgentMapTable);
      record.SetNullableInt32(0, new int?(queueId));
      record.SetNullableInt32(1, agentId != -1 ? new int?(agentId) : new int?());
      return record;
    }
  }
}
