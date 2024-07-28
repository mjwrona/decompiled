// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.DataAccess.TaskResourceComponent48
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.DistributedTask.Server.DataAccess
{
  internal class TaskResourceComponent48 : TaskResourceComponent47
  {
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
      TaskResourceComponent48 component = this;
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
        component.BindNullableBoolean("@enabled", enabled);
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
  }
}
