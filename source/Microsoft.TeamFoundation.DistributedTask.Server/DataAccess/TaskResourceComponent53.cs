// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.DataAccess.TaskResourceComponent53
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.Pipelines;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.Threading;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.DistributedTask.Server.DataAccess
{
  internal class TaskResourceComponent53 : TaskResourceComponent52
  {
    public override TaskAgentList GetAgents(
      int poolId,
      string agentName = null,
      bool includeCapabilities = false,
      bool includeAssignedRequest = false,
      bool includeAgentCloudRequest = false,
      bool includeLastCompletedRequest = false,
      IEnumerable<string> capabilityFilters = null)
    {
      return this.RequestContext.RunSynchronously<TaskAgentList>((Func<Task<TaskAgentList>>) (async () => await this.GetAgentsAsync(poolId, agentName, includeCapabilities, includeAssignedRequest, includeAgentCloudRequest, includeLastCompletedRequest, capabilityFilters)));
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
      TaskResourceComponent53 component = this;
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
          agentsAsync = new TaskAgentList(new bool?(), (IList<TaskAgent>) items);
        }
      }
      return agentsAsync;
    }
  }
}
