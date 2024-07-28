// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.DataAccess.TaskResourceComponent39
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.DistributedTask.Pipelines;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.DistributedTask.Server.DataAccess
{
  internal class TaskResourceComponent39 : TaskResourceComponent38
  {
    protected static SqlMetaData[] typ_QueuePoolMapTable = new SqlMetaData[2]
    {
      new SqlMetaData("@QueueId", SqlDbType.Int),
      new SqlMetaData("@PoolId", SqlDbType.Int)
    };

    public override CreateAgentResult AddAgent(int poolId, TaskAgent agent, bool createEnabled = true)
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (AddAgent)))
      {
        this.PrepareStoredProcedure("Task.prc_AddAgent");
        this.BindInt("@poolId", poolId);
        this.BindString("@name", agent.Name, 128, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
        this.BindString("@version", agent.Version, 128, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
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
          resultCollection.AddBinder<TaskAgent>((ObjectBinder<TaskAgent>) new TaskAgentBinder5());
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

    public override async Task<GetDeploymentMachinesResult> GetDeploymentTargetsAsync(
      Guid projectId,
      int deploymentGroupId,
      IEnumerable<string> tagFilters)
    {
      TaskResourceComponent39 component1 = this;
      GetDeploymentMachinesResult deploymentTargetsAsync;
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) component1, nameof (GetDeploymentTargetsAsync)))
      {
        component1.PrepareStoredProcedure("Task.prc_GetDeploymentMachines");
        component1.BindInt("@machineGroupId", deploymentGroupId);
        component1.BindInt("@dataspaceId", component1.GetDataspaceId(projectId, "DistributedTask", true));
        TaskResourceComponent39 component2 = component1;
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
          result.Machines = component1.CreateDeploymentTargetsWithTags((IEnumerable<QueueAgent>) items, lookup, deploymentGroupId);
          resultCollection.NextResult();
          result.MachineGroup = resultCollection.GetCurrent<DeploymentGroup>().FirstOrDefault<DeploymentGroup>((System.Func<DeploymentGroup, bool>) (d => d.Id == deploymentGroupId));
        }
        deploymentTargetsAsync = result;
      }
      return deploymentTargetsAsync;
    }

    public override async Task<IEnumerable<TaskAgent>> GetAgentsByFilterAsync(
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
      TaskResourceComponent39 component = this;
      IEnumerable<TaskAgent> agentsByFilterAsync;
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) component, nameof (GetAgentsByFilterAsync)))
      {
        component.PrepareStoredProcedure("Task.prc_GetAgentsByFilter");
        component.BindInt("@poolId", poolId);
        component.BindGuid("@hostId", hostId);
        component.BindGuid("@scopeId", scopeId);
        component.BindString("@agentName", agentName, 128, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
        component.BindBoolean("@partialNameMatch", partialNameMatch);
        component.BindBoolean("@includeCapabilities", includeCapabilities);
        component.BindBoolean("@includeAssignedRequest", includeAssignedRequest);
        component.BindBoolean("@includeLastCompletedRequest", includeLastCompletedRequest);
        component.BindBoolean("@isNeverDeployedFilter", isNeverDeployedFilter);
        component.BindNullableInt("@agentStatus", agentStatusFilter);
        component.BindTinyIntTable("@agentLastJobStatus", agentLastJobStatusFilters);
        component.BindUniqueInt32Table("@agentIds", agentIds == null ? (IEnumerable<int>) null : (IEnumerable<int>) agentIds.Distinct<int>().ToList<int>());
        component.BindString("@continuationToken", continuationToken, 128, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
        component.BindInt("@top", top);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) await component.ExecuteReaderAsync(), component.ProcedureName, component.RequestContext))
        {
          resultCollection.AddBinder<TaskAgent>(component.CreateTaskAgentBinder());
          List<TaskAgent> items = resultCollection.GetCurrent<TaskAgent>().Items;
          if (includeLastCompletedRequest)
          {
            resultCollection.AddBinder<TaskAgentJobRequest>((ObjectBinder<TaskAgentJobRequest>) new TaskAgentRequestBinder10());
            resultCollection.NextResult();
            Dictionary<int, TaskAgentJobRequest> dictionary = resultCollection.GetCurrent<TaskAgentJobRequest>().Items.ToDictionary<TaskAgentJobRequest, int>((System.Func<TaskAgentJobRequest, int>) (x => x.ReservedAgent.Id));
            foreach (TaskAgent taskAgent in items)
            {
              TaskAgentJobRequest taskAgentJobRequest;
              if (dictionary.TryGetValue(taskAgent.Id, out taskAgentJobRequest))
                taskAgent.LastCompletedRequest = taskAgentJobRequest;
            }
          }
          if (includeCapabilities)
            resultCollection.AddBinder<TaskAgentCapability>((ObjectBinder<TaskAgentCapability>) new TaskAgentCapabilityBinder());
          if (includeAssignedRequest)
            resultCollection.AddBinder<TaskAgentJobRequest>((ObjectBinder<TaskAgentJobRequest>) new TaskAgentRequestBinder10());
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
          agentsByFilterAsync = (IEnumerable<TaskAgent>) items;
        }
      }
      return agentsByFilterAsync;
    }

    public override TaskAgentPoolData GetAgentPool(int poolId) => this.GetAgentPoolsById(new HashSet<int>()
    {
      poolId
    }).FirstOrDefault<TaskAgentPoolData>();

    public override async Task<TaskAgentPoolData> GetAgentPoolAsync(int poolId)
    {
      TaskResourceComponent39 component = this;
      TaskAgentPoolData agentPoolAsync;
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) component, nameof (GetAgentPoolAsync)))
      {
        component.PrepareStoredProcedure("Task.prc_GetAgentPoolsById");
        component.BindUniqueInt32Table("@poolIds", (IEnumerable<int>) new int[1]
        {
          poolId
        });
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) await component.ExecuteReaderAsync(), component.ProcedureName, component.RequestContext))
        {
          resultCollection.AddBinder<TaskAgentPoolData>(component.CreateTaskAgentPoolBinder());
          agentPoolAsync = resultCollection.GetCurrent<TaskAgentPoolData>().Items.FirstOrDefault<TaskAgentPoolData>();
        }
      }
      return agentPoolAsync;
    }

    public override IList<TaskAgentPoolData> GetAgentPoolsById(HashSet<int> poolIds)
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (GetAgentPoolsById)))
      {
        this.PrepareStoredProcedure("Task.prc_GetAgentPoolsById");
        this.BindUniqueInt32Table("@poolIds", (IEnumerable<int>) poolIds);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<TaskAgentPoolData>(this.CreateTaskAgentPoolBinder());
          return (IList<TaskAgentPoolData>) resultCollection.GetCurrent<TaskAgentPoolData>().Items;
        }
      }
    }

    public override async Task<IList<TaskAgentPoolData>> GetAgentPoolsByIdAsync(HashSet<int> poolIds)
    {
      TaskResourceComponent39 component = this;
      IList<TaskAgentPoolData> items;
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) component, nameof (GetAgentPoolsByIdAsync)))
      {
        component.PrepareStoredProcedure("Task.prc_GetAgentPoolsById");
        component.BindUniqueInt32Table("@poolIds", (IEnumerable<int>) poolIds);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) await component.ExecuteReaderAsync(), component.ProcedureName, component.RequestContext))
        {
          resultCollection.AddBinder<TaskAgentPoolData>(component.CreateTaskAgentPoolBinder());
          items = (IList<TaskAgentPoolData>) resultCollection.GetCurrent<TaskAgentPoolData>().Items;
        }
      }
      return items;
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
        this.BindDeploymentGroupPoolMappingTable("@queuePoolMap", deploymentGroups);
        this.BindGuid("@hostId", hostId);
        this.BindGuid("@scopeId", scopeId);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<DeploymentGroupMetrics>((ObjectBinder<DeploymentGroupMetrics>) new DeploymentGroupMetricsBinder(deploymentGroups));
          deploymentGroupsMetrics.AddRange((IEnumerable<DeploymentGroupMetrics>) resultCollection.GetCurrent<DeploymentGroupMetrics>().Items);
        }
      }
      return (IEnumerable<DeploymentGroupMetrics>) deploymentGroupsMetrics;
    }

    protected SqlParameter BindDeploymentGroupPoolMappingTable(
      string parameterName,
      IDictionary<int, DeploymentGroup> rows)
    {
      IList<SqlDataRecord> rows1 = (IList<SqlDataRecord>) new List<SqlDataRecord>();
      foreach (KeyValuePair<int, DeploymentGroup> row in (IEnumerable<KeyValuePair<int, DeploymentGroup>>) rows)
        rows1.Add(this.ConvertToQueuePoolSqlDataRecord(row.Value.Id, row.Value.Pool.Id));
      return this.BindTable(parameterName, "Task.typ_QueuePoolMapTable", (IEnumerable<SqlDataRecord>) rows1);
    }

    public override DeploymentMachine AddDeploymentTarget(
      Guid projectId,
      int deploymentGroupId,
      DeploymentMachine machine)
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (AddDeploymentTarget)))
      {
        this.PrepareStoredProcedure("Task.prc_AddDeploymentMachine");
        this.BindInt("@dataspaceId", this.GetDataspaceId(projectId, "DistributedTask", true));
        this.BindInt("@deploymentGroupId", deploymentGroupId);
        this.BindInt("@agentId", machine.Agent.Id);
        this.BindStringTable("@tags", (IEnumerable<string>) machine.Tags);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<QueueAgent>((ObjectBinder<QueueAgent>) new QueueAgentBinder((TaskResourceComponent) this));
          resultCollection.AddBinder<QueueAgentTag>((ObjectBinder<QueueAgentTag>) new QueueAgentTagBinder());
          QueueAgent queueAgent = resultCollection.GetCurrent<QueueAgent>().First<QueueAgent>();
          resultCollection.NextResult();
          ILookup<int, string> lookup = resultCollection.GetCurrent<QueueAgentTag>().Items.ToLookup<QueueAgentTag, int, string>((System.Func<QueueAgentTag, int>) (t => t.MappingId), (System.Func<QueueAgentTag, string>) (t => t.Tag));
          return this.CreateDeploymentTargetsWithTags((IEnumerable<QueueAgent>) new QueueAgent[1]
          {
            queueAgent
          }, lookup, deploymentGroupId).First<DeploymentMachine>();
        }
      }
    }

    public override DeploymentMachine DeleteDeploymentTarget(
      Guid projectId,
      int deploymentGroupId,
      int machineId)
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (DeleteDeploymentTarget)))
      {
        this.PrepareStoredProcedure("Task.prc_DeleteDeploymentTarget");
        this.BindInt("@dataspaceId", this.GetDataspaceId(projectId, "DistributedTask", true));
        this.BindInt("@deploymentGroupId", deploymentGroupId);
        this.BindInt("@agentId", machineId);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<QueueAgent>((ObjectBinder<QueueAgent>) new QueueAgentBinder((TaskResourceComponent) this));
          resultCollection.AddBinder<QueueAgentTag>((ObjectBinder<QueueAgentTag>) new QueueAgentTagBinder());
          QueueAgent queueAgent = resultCollection.GetCurrent<QueueAgent>().FirstOrDefault<QueueAgent>();
          if (queueAgent == null)
            return (DeploymentMachine) null;
          resultCollection.NextResult();
          ILookup<int, string> lookup = resultCollection.GetCurrent<QueueAgentTag>().Items.ToLookup<QueueAgentTag, int, string>((System.Func<QueueAgentTag, int>) (t => t.MappingId), (System.Func<QueueAgentTag, string>) (t => t.Tag));
          return this.CreateDeploymentTargetsWithTags((IEnumerable<QueueAgent>) new QueueAgent[1]
          {
            queueAgent
          }, lookup, deploymentGroupId).FirstOrDefault<DeploymentMachine>();
        }
      }
    }

    public override UpdateDeploymentMachinesResult UpdateDeploymentTargets(
      Guid projectId,
      int machineGroupId,
      IEnumerable<DeploymentMachine> machines)
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (UpdateDeploymentTargets)))
      {
        IList<int> machineIdsForTagDeletion = (IList<int>) null;
        UpdateDeploymentMachinesResult deploymentMachinesResult = new UpdateDeploymentMachinesResult();
        IList<KeyValuePair<int, string>> machinesTagsTable = this.GetDeploymentMachinesTagsTable(machines, out machineIdsForTagDeletion);
        this.PrepareStoredProcedure("Task.prc_UpdateDeploymentTargets");
        this.BindInt("@dataspaceId", this.GetDataspaceId(projectId, "DistributedTask", true));
        this.BindInt("@deploymentGroupId", machineGroupId);
        this.BindKeyValuePairInt32StringTable("@tagsTable", (IEnumerable<KeyValuePair<int, string>>) machinesTagsTable);
        this.BindUniqueInt32Table("@agentIdsForTagDeletion", (IEnumerable<int>) machineIdsForTagDeletion);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<QueueAgent>((ObjectBinder<QueueAgent>) new QueueAgentBinder((TaskResourceComponent) this));
          resultCollection.AddBinder<QueueAgentTag>((ObjectBinder<QueueAgentTag>) new QueueAgentTagBinder());
          resultCollection.AddBinder<DeploymentGroup>((ObjectBinder<DeploymentGroup>) new DeploymentGroupBinder((TaskResourceComponent) this));
          List<QueueAgent> items = resultCollection.GetCurrent<QueueAgent>().Items;
          resultCollection.NextResult();
          ILookup<int, string> lookup1 = resultCollection.GetCurrent<QueueAgentTag>().Items.ToLookup<QueueAgentTag, int, string>((System.Func<QueueAgentTag, int>) (t => t.MappingId), (System.Func<QueueAgentTag, string>) (t => t.Tag));
          resultCollection.NextResult();
          deploymentMachinesResult.MachineGroup = resultCollection.GetCurrent<DeploymentGroup>().First<DeploymentGroup>((System.Func<DeploymentGroup, bool>) (d => d.Id == machineGroupId));
          ILookup<int, string> lookup2 = Enumerable.Empty<string>().ToLookup<string, int>((System.Func<string, int>) (x => 0));
          deploymentMachinesResult.Machines = this.CreateDeploymentTargetsChangedDataWithTags((IEnumerable<QueueAgent>) items, lookup1, lookup2, machineGroupId);
        }
        return deploymentMachinesResult;
      }
    }

    protected override IEnumerable<DeploymentMachineData> CreateDeploymentMachines(
      IEnumerable<QueueAgent> mappings)
    {
      return mappings.Select<QueueAgent, DeploymentMachineData>((System.Func<QueueAgent, DeploymentMachineData>) (m =>
      {
        return new DeploymentMachineData()
        {
          QueueId = m.QueueId,
          Project = m.Project,
          Machine = new DeploymentMachine()
          {
            Id = m.AgentId,
            Agent = new TaskAgent() { Id = m.AgentId }
          }
        };
      }));
    }

    protected virtual IEnumerable<DeploymentMachine> CreateDeploymentTargetsWithTags(
      IEnumerable<QueueAgent> mappings,
      ILookup<int, string> tagsWithMappingIdLookup,
      int queueId)
    {
      return mappings.Where<QueueAgent>((System.Func<QueueAgent, bool>) (m => m.QueueId == queueId)).Select<QueueAgent, DeploymentMachine>((System.Func<QueueAgent, DeploymentMachine>) (m =>
      {
        return new DeploymentMachine()
        {
          Id = m.AgentId,
          Agent = new TaskAgent() { Id = m.AgentId },
          Tags = (IList<string>) tagsWithMappingIdLookup[m.QueueAgentId].ToList<string>()
        };
      }));
    }

    protected virtual IEnumerable<DeploymentMachineChangedData> CreateDeploymentTargetsChangedDataWithTags(
      IEnumerable<QueueAgent> mappings,
      ILookup<int, string> tagsWithMappingIdLookup,
      ILookup<int, string> existingTagsWithMappingIdLookup,
      int queueId)
    {
      return mappings.Where<QueueAgent>((System.Func<QueueAgent, bool>) (m => m.QueueId == queueId)).Select<QueueAgent, DeploymentMachineChangedData>((System.Func<QueueAgent, DeploymentMachineChangedData>) (m =>
      {
        return new DeploymentMachineChangedData()
        {
          Id = m.AgentId,
          Agent = new TaskAgent() { Id = m.AgentId },
          Tags = (IList<string>) tagsWithMappingIdLookup[m.QueueAgentId].ToList<string>(),
          TagsAdded = (IList<string>) tagsWithMappingIdLookup[m.QueueAgentId].ToList<string>().Except<string>((IEnumerable<string>) existingTagsWithMappingIdLookup[m.QueueAgentId].ToList<string>()).ToList<string>(),
          TagsDeleted = (IList<string>) existingTagsWithMappingIdLookup[m.QueueAgentId].ToList<string>().Except<string>((IEnumerable<string>) tagsWithMappingIdLookup[m.QueueAgentId].ToList<string>()).ToList<string>()
        };
      }));
    }

    protected SqlDataRecord ConvertToQueuePoolSqlDataRecord(int queueId, int poolId)
    {
      SqlDataRecord record = new SqlDataRecord(TaskResourceComponent39.typ_QueuePoolMapTable);
      record.SetNullableInt32(0, new int?(queueId));
      record.SetNullableInt32(1, new int?(poolId));
      return record;
    }
  }
}
