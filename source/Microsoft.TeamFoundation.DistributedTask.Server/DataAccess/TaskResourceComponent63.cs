// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.DataAccess.TaskResourceComponent63
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.Pipelines;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.AuditLog;
using Microsoft.TeamFoundation.Framework.Server.Threading;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.DistributedTask.Server.DataAccess
{
  internal class TaskResourceComponent63 : TaskResourceComponent62
  {
    public override async Task<TaskAgentList> GetAgentsAsync(
      int poolId,
      string agentName = null,
      bool includeCapabilities = false,
      bool includeAssignedRequest = false,
      bool includeAgentCloudRequest = false,
      bool includeLastCompletedRequest = false,
      IEnumerable<string> capabilityFilters = null)
    {
      TaskResourceComponent63 component = this;
      TaskAgentList agentsAsync;
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) component, nameof (GetAgentsAsync)))
      {
        component.PrepareStoredProcedure("Task.prc_GetAgents");
        component.BindInt("@poolId", poolId);
        component.BindString("@agentName", agentName, 128, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
        component.BindBoolean("@includeCapabilities", includeCapabilities);
        component.BindBoolean("@includeAssignedRequest", includeAssignedRequest);
        component.BindBoolean("@includeAgentCloudRequest", includeAgentCloudRequest);
        component.BindBoolean("@includeLastCompletedRequest", includeLastCompletedRequest);
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
          if (includeLastCompletedRequest)
            resultCollection.AddBinder<TaskAgentJobRequest>(component.CreateTaskAgentRequestBinder(poolId));
          if (includeAgentCloudRequest)
            resultCollection.AddBinder<TaskAgentCloudRequest>(component.CreateTaskAgentCloudRequestBinder());
          resultCollection.AddBinder<bool>((ObjectBinder<bool>) new BooleanBinder());
          List<TaskAgent> items = resultCollection.GetCurrent<TaskAgent>().Items;
          if (includeAssignedRequest | includeCapabilities | includeAgentCloudRequest | includeLastCompletedRequest)
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
            if (includeLastCompletedRequest && resultCollection.TryNextResult())
            {
              foreach (TaskAgentJobRequest taskAgentJobRequest in resultCollection.GetCurrent<TaskAgentJobRequest>())
              {
                TaskAgent taskAgent;
                if (dictionary.TryGetValue(taskAgentJobRequest.ReservedAgent.Id, out taskAgent))
                  taskAgent.LastCompletedRequest = taskAgentJobRequest;
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

    protected override ObjectBinder<TaskAgentPoolData> CreateTaskAgentPoolBinder() => (ObjectBinder<TaskAgentPoolData>) new TaskAgentPoolBinder12(this.RequestContext);

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
        this.PrepareForAuditingAction(AgentPoolAuditConstants.AgentPoolCreated);
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
        this.BindBoolean("@isLegacy", isLegacy.GetValueOrDefault());
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<TaskAgentPoolData>(this.CreateTaskAgentPoolBinder());
          return resultCollection.GetCurrent<TaskAgentPoolData>().First<TaskAgentPoolData>();
        }
      }
    }

    public override IList<TaskAgent> GetAgentsById(
      int poolId,
      IEnumerable<int> agentIds,
      bool includeCapabilities = false,
      bool includeAssignedRequest = false,
      bool includeLastCompletedRequest = false)
    {
      return this.RequestContext.RunSynchronously<IList<TaskAgent>>((Func<Task<IList<TaskAgent>>>) (async () => await this.GetAgentsByIdAsync(poolId, agentIds, includeCapabilities, includeAssignedRequest, includeLastCompletedRequest)));
    }

    public override async Task<IList<TaskAgent>> GetAgentsByIdAsync(
      int poolId,
      IEnumerable<int> agentIds,
      bool includeCapabilities = false,
      bool includeAssignedRequest = false,
      bool includeLastCompletedRequest = false)
    {
      TaskResourceComponent63 component = this;
      IList<TaskAgent> agentsByIdAsync;
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) component, nameof (GetAgentsByIdAsync)))
      {
        component.PrepareStoredProcedure("Task.prc_GetAgentsById");
        component.BindInt("@poolId", poolId);
        component.BindUniqueInt32Table("@agentIds", agentIds);
        component.BindBoolean("@includeCapabilities", includeCapabilities);
        component.BindBoolean("@includeAssignedRequest", includeAssignedRequest);
        component.BindBoolean("@includeLastCompletedRequest", includeLastCompletedRequest);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) await component.ExecuteReaderAsync(), component.ProcedureName, component.RequestContext))
        {
          resultCollection.AddBinder<TaskAgent>(component.CreateTaskAgentBinder());
          if (includeCapabilities)
            resultCollection.AddBinder<TaskAgentCapability>((ObjectBinder<TaskAgentCapability>) new TaskAgentCapabilityBinder());
          if (includeAssignedRequest)
            resultCollection.AddBinder<TaskAgentJobRequest>(component.CreateTaskAgentRequestBinder(poolId));
          if (includeLastCompletedRequest)
            resultCollection.AddBinder<TaskAgentJobRequest>(component.CreateTaskAgentRequestBinder(poolId));
          List<TaskAgent> items = resultCollection.GetCurrent<TaskAgent>().Items;
          if (includeAssignedRequest | includeCapabilities | includeLastCompletedRequest)
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
            if (includeLastCompletedRequest && resultCollection.TryNextResult())
            {
              foreach (TaskAgentJobRequest taskAgentJobRequest in resultCollection.GetCurrent<TaskAgentJobRequest>())
              {
                TaskAgent taskAgent;
                if (dictionary.TryGetValue(taskAgentJobRequest.ReservedAgent.Id, out taskAgent))
                  taskAgent.LastCompletedRequest = taskAgentJobRequest;
              }
            }
          }
          agentsByIdAsync = (IList<TaskAgent>) items;
        }
      }
      return agentsByIdAsync;
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
        this.BindNullableBoolean("@isLegacy", isLegacy);
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
  }
}
