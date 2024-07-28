// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.DataAccess.TaskResourceComponent28
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.Pipelines;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.DistributedTask.Server.DataAccess
{
  internal class TaskResourceComponent28 : TaskResourceComponent27
  {
    public override TaskAgentJobRequest AbandonAgentRequest(
      int poolId,
      long requestId,
      DateTime expirationTime)
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (AbandonAgentRequest)))
      {
        this.PrepareStoredProcedure("Task.prc_AbandonAgentRequest");
        this.BindInt("@poolId", poolId);
        this.BindLong("@requestId", requestId);
        this.BindDateTime2("@expirationTime", expirationTime);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<TaskAgentJobRequest>(this.CreateTaskAgentRequestBinder(poolId));
          return resultCollection.GetCurrent<TaskAgentJobRequest>().FirstOrDefault<TaskAgentJobRequest>();
        }
      }
    }

    public override async Task<FinishAgentRequestResult> FinishAgentRequestAsync(
      int poolId,
      long requestId,
      bool deprovisionAgent,
      bool transitionAgentState,
      TaskResult? jobResult = null,
      bool agentShuttingDown = false)
    {
      TaskResourceComponent28 component = this;
      FinishAgentRequestResult agentRequestResult;
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) component, nameof (FinishAgentRequestAsync)))
      {
        component.PrepareStoredProcedure("Task.prc_FinishAgentRequest");
        component.BindInt("@poolId", poolId);
        component.BindLong("@requestId", requestId);
        component.BindInt("@leaseTimeoutInSeconds", (int) TimeSpan.FromMinutes(5.0).TotalSeconds);
        FinishAgentRequestResult result = new FinishAgentRequestResult();
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) await component.ExecuteReaderAsync(), component.ProcedureName, component.RequestContext))
        {
          resultCollection.AddBinder<TaskAgentJobRequest>(component.CreateTaskAgentRequestBinder(poolId));
          result.CompletedRequest = resultCollection.GetCurrent<TaskAgentJobRequest>().FirstOrDefault<TaskAgentJobRequest>();
          agentRequestResult = result;
        }
      }
      return agentRequestResult;
    }

    public override IList<TaskAgent> GetAgentsById(
      int poolId,
      IEnumerable<int> agentIds,
      bool includeCapabilities = false,
      bool includeAssignedRequest = false,
      bool includeLastCompletedRequest = false)
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (GetAgentsById)))
      {
        this.PrepareStoredProcedure("Task.prc_GetAgentsById");
        this.BindInt("@poolId", poolId);
        this.BindUniqueInt32Table("@agentIds", agentIds);
        this.BindBoolean("@includeCapabilities", includeCapabilities);
        this.BindBoolean("@includeAssignedRequest", includeAssignedRequest);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<TaskAgent>((ObjectBinder<TaskAgent>) new TaskAgentBinder5());
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
          return (IList<TaskAgent>) items;
        }
      }
    }

    public override async Task<IList<TaskAgent>> GetAgentsByIdAsync(
      int poolId,
      IEnumerable<int> agentIds,
      bool includeCapabilities = false,
      bool includeAssignedRequest = false,
      bool includeLastCompletedRequest = false)
    {
      TaskResourceComponent28 component = this;
      IList<TaskAgent> agentsByIdAsync;
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) component, nameof (GetAgentsByIdAsync)))
      {
        component.PrepareStoredProcedure("Task.prc_GetAgentsById");
        component.BindInt("@poolId", poolId);
        component.BindUniqueInt32Table("@agentIds", agentIds);
        component.BindBoolean("@includeCapabilities", includeCapabilities);
        component.BindBoolean("@includeAssignedRequest", includeAssignedRequest);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) await component.ExecuteReaderAsync(), component.ProcedureName, component.RequestContext))
        {
          resultCollection.AddBinder<TaskAgent>((ObjectBinder<TaskAgent>) new TaskAgentBinder5());
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
          agentsByIdAsync = (IList<TaskAgent>) items;
        }
      }
      return agentsByIdAsync;
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
          resultCollection.AddBinder<TaskAgent>((ObjectBinder<TaskAgent>) new TaskAgentBinder5());
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
      TaskResourceComponent28 component = this;
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
          resultCollection.AddBinder<TaskAgent>((ObjectBinder<TaskAgent>) new TaskAgentBinder5());
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

    public override TaskAgentRequestData GetAgentRequest(
      int poolId,
      long requestId,
      bool includeStatus = false)
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (GetAgentRequest)))
      {
        TaskAgentRequestData agentRequest1 = (TaskAgentRequestData) null;
        this.PrepareStoredProcedure("Task.prc_GetAgentRequest");
        this.BindInt("@poolId", poolId);
        this.BindLong("@requestId", requestId);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<TaskAgentJobRequest>(this.CreateTaskAgentRequestBinder(poolId));
          resultCollection.AddBinder<MatchedAgent>((ObjectBinder<MatchedAgent>) new MatchedAgentBinder2());
          TaskAgentJobRequest agentRequest2 = resultCollection.GetCurrent<TaskAgentJobRequest>().FirstOrDefault<TaskAgentJobRequest>();
          if (agentRequest2 != null)
          {
            agentRequest1 = new TaskAgentRequestData(agentRequest2);
            resultCollection.NextResult();
            foreach (MatchedAgent matchedAgent in resultCollection.GetCurrent<MatchedAgent>())
              agentRequest1.AgentRequest.MatchedAgents.Add(matchedAgent.Agent);
          }
        }
        return agentRequest1;
      }
    }

    public override async Task<TaskAgentRequestData> GetAgentRequestAsync(
      int poolId,
      long requestId,
      bool includeStatus = false)
    {
      TaskResourceComponent28 component = this;
      TaskAgentRequestData agentRequestAsync;
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) component, nameof (GetAgentRequestAsync)))
      {
        TaskAgentRequestData data = (TaskAgentRequestData) null;
        component.PrepareStoredProcedure("Task.prc_GetAgentRequest");
        component.BindInt("@poolId", poolId);
        component.BindLong("@requestId", requestId);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) await component.ExecuteReaderAsync(), component.ProcedureName, component.RequestContext))
        {
          resultCollection.AddBinder<TaskAgentJobRequest>(component.CreateTaskAgentRequestBinder(poolId));
          resultCollection.AddBinder<MatchedAgent>((ObjectBinder<MatchedAgent>) new MatchedAgentBinder2());
          TaskAgentJobRequest agentRequest = resultCollection.GetCurrent<TaskAgentJobRequest>().FirstOrDefault<TaskAgentJobRequest>();
          if (agentRequest != null)
          {
            data = new TaskAgentRequestData(agentRequest);
            resultCollection.NextResult();
            foreach (MatchedAgent matchedAgent in resultCollection.GetCurrent<MatchedAgent>())
              data.AgentRequest.MatchedAgents.Add(matchedAgent.Agent);
          }
        }
        agentRequestAsync = data;
      }
      return agentRequestAsync;
    }

    public override IList<TaskAgentJobRequest> GetAgentRequestsForAgents(
      int poolId,
      IList<int> agentIds,
      int completedRequestCount)
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (GetAgentRequestsForAgents)))
      {
        this.PrepareStoredProcedure("Task.prc_GetAgentRequests");
        this.BindInt("@poolId", poolId);
        this.BindInt32Table("@agentIds", (IEnumerable<int>) agentIds);
        this.BindInt("@completedRequestCount", completedRequestCount);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<TaskAgentJobRequest>(this.CreateTaskAgentRequestBinder(poolId));
          resultCollection.AddBinder<MatchedAgent>((ObjectBinder<MatchedAgent>) new MatchedAgentBinder2());
          resultCollection.AddBinder<TaskAgentJobRequest>(this.CreateTaskAgentRequestBinder(poolId));
          resultCollection.AddBinder<TaskAgentJobRequest>(this.CreateTaskAgentRequestBinder(poolId));
          List<TaskAgentJobRequest> items = resultCollection.GetCurrent<TaskAgentJobRequest>().Items;
          resultCollection.NextResult();
          if (items.Count > 0)
          {
            Dictionary<long, TaskAgentJobRequest> dictionary = items.ToDictionary<TaskAgentJobRequest, long>((System.Func<TaskAgentJobRequest, long>) (x => x.RequestId));
            foreach (MatchedAgent matchedAgent in resultCollection.GetCurrent<MatchedAgent>())
            {
              TaskAgentJobRequest taskAgentJobRequest;
              if (dictionary.TryGetValue(matchedAgent.RequestId, out taskAgentJobRequest))
                taskAgentJobRequest.MatchedAgents.Add(matchedAgent.Agent);
            }
          }
          resultCollection.NextResult();
          items.AddRange((IEnumerable<TaskAgentJobRequest>) resultCollection.GetCurrent<TaskAgentJobRequest>().Items);
          resultCollection.NextResult();
          items.AddRange((IEnumerable<TaskAgentJobRequest>) resultCollection.GetCurrent<TaskAgentJobRequest>().Items);
          return (IList<TaskAgentJobRequest>) items;
        }
      }
    }

    public override async Task<IList<TaskAgentJobRequest>> GetAgentRequestsForAgentsAsync(
      int poolId,
      IList<int> agentIds,
      int completedRequestCount)
    {
      TaskResourceComponent28 component = this;
      IList<TaskAgentJobRequest> requestsForAgentsAsync;
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) component, nameof (GetAgentRequestsForAgentsAsync)))
      {
        component.PrepareStoredProcedure("Task.prc_GetAgentRequests");
        component.BindInt("@poolId", poolId);
        component.BindInt32Table("@agentIds", (IEnumerable<int>) agentIds);
        component.BindInt("@completedRequestCount", completedRequestCount);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) await component.ExecuteReaderAsync(), component.ProcedureName, component.RequestContext))
        {
          resultCollection.AddBinder<TaskAgentJobRequest>(component.CreateTaskAgentRequestBinder(poolId));
          resultCollection.AddBinder<MatchedAgent>((ObjectBinder<MatchedAgent>) new MatchedAgentBinder2());
          resultCollection.AddBinder<TaskAgentJobRequest>(component.CreateTaskAgentRequestBinder(poolId));
          resultCollection.AddBinder<TaskAgentJobRequest>(component.CreateTaskAgentRequestBinder(poolId));
          List<TaskAgentJobRequest> items = resultCollection.GetCurrent<TaskAgentJobRequest>().Items;
          resultCollection.NextResult();
          if (items.Count > 0)
          {
            Dictionary<long, TaskAgentJobRequest> dictionary = items.ToDictionary<TaskAgentJobRequest, long>((System.Func<TaskAgentJobRequest, long>) (x => x.RequestId));
            foreach (MatchedAgent matchedAgent in resultCollection.GetCurrent<MatchedAgent>())
            {
              TaskAgentJobRequest taskAgentJobRequest;
              if (dictionary.TryGetValue(matchedAgent.RequestId, out taskAgentJobRequest))
                taskAgentJobRequest.MatchedAgents.Add(matchedAgent.Agent);
            }
          }
          resultCollection.NextResult();
          items.AddRange((IEnumerable<TaskAgentJobRequest>) resultCollection.GetCurrent<TaskAgentJobRequest>().Items);
          resultCollection.NextResult();
          items.AddRange((IEnumerable<TaskAgentJobRequest>) resultCollection.GetCurrent<TaskAgentJobRequest>().Items);
          requestsForAgentsAsync = (IList<TaskAgentJobRequest>) items;
        }
      }
      return requestsForAgentsAsync;
    }

    public override IList<TaskAgentJobRequest> GetAgentRequestsForPlan(
      int poolId,
      Guid planId,
      Guid? jobId)
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (GetAgentRequestsForPlan)))
      {
        this.PrepareStoredProcedure("Task.prc_GetAgentRequestsForPlan");
        this.BindInt("@poolId", poolId);
        this.BindGuid("@planId", planId);
        this.BindNullableGuid("@jobId", jobId);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<TaskAgentJobRequest>(this.CreateTaskAgentRequestBinder(poolId));
          resultCollection.AddBinder<MatchedAgent>((ObjectBinder<MatchedAgent>) new MatchedAgentBinder2());
          List<TaskAgentJobRequest> items = resultCollection.GetCurrent<TaskAgentJobRequest>().Items;
          resultCollection.NextResult();
          if (items.Count > 0)
          {
            Dictionary<long, TaskAgentJobRequest> dictionary = items.ToDictionary<TaskAgentJobRequest, long>((System.Func<TaskAgentJobRequest, long>) (x => x.RequestId));
            foreach (MatchedAgent matchedAgent in resultCollection.GetCurrent<MatchedAgent>())
            {
              TaskAgentJobRequest taskAgentJobRequest;
              if (dictionary.TryGetValue(matchedAgent.RequestId, out taskAgentJobRequest))
                taskAgentJobRequest.MatchedAgents.Add(matchedAgent.Agent);
            }
          }
          return (IList<TaskAgentJobRequest>) items;
        }
      }
    }

    public override async Task<ExpiredAgentRequestsResult> GetExpiredAgentRequestsAsync(int poolId)
    {
      TaskResourceComponent28 component = this;
      ExpiredAgentRequestsResult agentRequestsAsync;
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) component, nameof (GetExpiredAgentRequestsAsync)))
      {
        component.PrepareStoredProcedure("Task.prc_GetExpiredAgentRequests");
        component.BindInt("@poolId", poolId);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) await component.ExecuteReaderAsync(), component.ProcedureName, component.RequestContext))
        {
          resultCollection.AddBinder<TaskAgentJobRequest>(component.CreateTaskAgentRequestBinder(poolId));
          resultCollection.AddBinder<bool>((ObjectBinder<bool>) new BooleanBinder());
          IList<TaskAgentJobRequest> items = (IList<TaskAgentJobRequest>) resultCollection.GetCurrent<TaskAgentJobRequest>().Items;
          resultCollection.NextResult();
          agentRequestsAsync = new ExpiredAgentRequestsResult(resultCollection.GetCurrent<bool>().First<bool>(), items);
        }
      }
      return agentRequestsAsync;
    }

    public override async Task<IList<TaskAgentJobRequest>> GetUnassignedAgentRequestsAsync(
      int poolId)
    {
      TaskResourceComponent28 component = this;
      IList<TaskAgentJobRequest> items;
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) component, nameof (GetUnassignedAgentRequestsAsync)))
      {
        component.PrepareStoredProcedure("Task.prc_GetUnassignedAgentRequests");
        component.BindInt("@poolId", poolId);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) await component.ExecuteReaderAsync(), component.ProcedureName, component.RequestContext))
        {
          resultCollection.AddBinder<TaskAgentJobRequest>(component.CreateTaskAgentRequestBinder(poolId));
          items = (IList<TaskAgentJobRequest>) resultCollection.GetCurrent<TaskAgentJobRequest>().Items;
        }
      }
      return items;
    }

    public override async Task<TaskAgentJobRequest> QueueAgentRequestAsync(
      int poolId,
      int? agentCloudId,
      TaskAgentJobRequest request,
      string requestResourceType)
    {
      TaskResourceComponent28 component = this;
      TaskAgentJobRequest taskAgentJobRequest;
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) component, nameof (QueueAgentRequestAsync)))
      {
        component.PrepareStoredProcedure("Task.prc_QueueAgentRequest");
        component.BindInt("@poolId", poolId);
        component.BindInt32Table("@matchedAgentIds", (IEnumerable<int>) request.MatchedAgents.Select<TaskAgentReference, int>((System.Func<TaskAgentReference, int>) (x => x.Id)).Distinct<int>().ToList<int>());
        component.BindGuid("@serviceOwner", request.ServiceOwner);
        component.BindGuid("@hostId", request.HostId);
        component.BindGuid("@scopeId", request.ScopeId);
        component.BindString("@planType", request.PlanType, 260, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
        component.BindGuid("@planId", request.PlanId);
        component.BindGuid("@jobId", request.JobId);
        component.BindString("@jobName", request.JobName, 512, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
        component.BindBinary("@demands", JsonUtility.Serialize((object) request.Demands, false), int.MaxValue, SqlDbType.VarBinary);
        component.BindNullableInt("@definitionId", request.Definition?.Id);
        component.BindBinary("@definitionReference", JsonUtility.Serialize((object) request.Definition, false), int.MaxValue, SqlDbType.VarBinary);
        component.BindNullableInt("@ownerId", request.Owner?.Id);
        component.BindBinary("@ownerReference", JsonUtility.Serialize((object) request.Owner, false), int.MaxValue, SqlDbType.VarBinary);
        component.BindInt("@leaseTimeoutInSeconds", (int) TimeSpan.FromMinutes(5.0).TotalSeconds);
        component.BindBinary("@data", JsonUtility.Serialize((object) request.Data, false), 1024, SqlDbType.VarBinary);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) await component.ExecuteReaderAsync(), component.ProcedureName, component.RequestContext))
        {
          resultCollection.AddBinder<TaskAgentJobRequest>(component.CreateTaskAgentRequestBinder(poolId));
          resultCollection.AddBinder<MatchedAgent>((ObjectBinder<MatchedAgent>) new MatchedAgentBinder2());
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
        this.BindInt("@leaseTimeoutInSeconds", (int) TimeSpan.FromMinutes(5.0).TotalSeconds);
        AgentConnectivityResult connectivityResult = new AgentConnectivityResult();
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<bool>((ObjectBinder<bool>) new BooleanBinder());
          resultCollection.AddBinder<TaskAgentJobRequest>(this.CreateTaskAgentRequestBinder(poolId));
          resultCollection.AddBinder<TaskAgentPoolData>(this.CreateTaskAgentPoolBinder());
          resultCollection.AddBinder<TaskAgent>((ObjectBinder<TaskAgent>) new TaskAgentBinder5());
          resultCollection.AddBinder<TaskAgentCapability>((ObjectBinder<TaskAgentCapability>) new TaskAgentCapabilityBinder());
          connectivityResult.HandledEvent = resultCollection.GetCurrent<bool>().First<bool>();
          resultCollection.NextResult();
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

    public override UpdateAgentRequestResult UpdateAgentRequest(
      int poolId,
      long requestId,
      TimeSpan leaseTimeout,
      DateTime? expirationTime = null,
      DateTime? startTime = null,
      DateTime? finishTime = null,
      TaskResult? result = null)
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (UpdateAgentRequest)))
      {
        this.PrepareStoredProcedure("Task.prc_UpdateAgentRequest");
        this.BindInt("@poolId", poolId);
        this.BindLong("@requestId", requestId);
        this.BindInt("@leaseTimeoutInSeconds", (int) leaseTimeout.TotalSeconds);
        if (expirationTime.HasValue)
          this.BindDateTime2("@expirationTime", expirationTime.Value);
        if (startTime.HasValue)
          this.BindDateTime2("@startTime", startTime.Value);
        if (finishTime.HasValue)
          this.BindDateTime2("@finishTime", finishTime.Value);
        if (result.HasValue)
          this.BindByte("@result", (byte) result.Value);
        UpdateAgentRequestResult agentRequestResult = new UpdateAgentRequestResult();
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<TaskAgentJobRequest>(this.CreateTaskAgentRequestBinder(poolId));
          resultCollection.AddBinder<TaskAgentJobRequest>(this.CreateTaskAgentRequestBinder(poolId));
          agentRequestResult.Before = resultCollection.GetCurrent<TaskAgentJobRequest>().FirstOrDefault<TaskAgentJobRequest>();
          resultCollection.NextResult();
          agentRequestResult.After = resultCollection.GetCurrent<TaskAgentJobRequest>().FirstOrDefault<TaskAgentJobRequest>();
          return agentRequestResult;
        }
      }
    }

    public override async Task<UpdateAgentRequestResult> UpdateAgentRequestAsync(
      int poolId,
      long requestId,
      TimeSpan leaseTimeout,
      DateTime? expirationTime = null,
      DateTime? startTime = null,
      DateTime? finishTime = null,
      TaskResult? result = null)
    {
      TaskResourceComponent28 component = this;
      UpdateAgentRequestResult agentRequestResult;
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) component, nameof (UpdateAgentRequestAsync)))
      {
        component.PrepareStoredProcedure("Task.prc_UpdateAgentRequest");
        component.BindInt("@poolId", poolId);
        component.BindLong("@requestId", requestId);
        component.BindInt("@leaseTimeoutInSeconds", (int) leaseTimeout.TotalSeconds);
        if (expirationTime.HasValue)
          component.BindDateTime2("@expirationTime", expirationTime.Value);
        if (startTime.HasValue)
          component.BindDateTime2("@startTime", startTime.Value);
        if (finishTime.HasValue)
          component.BindDateTime2("@finishTime", finishTime.Value);
        if (result.HasValue)
          component.BindByte("@result", (byte) result.Value);
        UpdateAgentRequestResult updateResult = new UpdateAgentRequestResult();
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) await component.ExecuteReaderAsync(), component.ProcedureName, component.RequestContext))
        {
          resultCollection.AddBinder<TaskAgentJobRequest>(component.CreateTaskAgentRequestBinder(poolId));
          resultCollection.AddBinder<TaskAgentJobRequest>(component.CreateTaskAgentRequestBinder(poolId));
          updateResult.Before = resultCollection.GetCurrent<TaskAgentJobRequest>().FirstOrDefault<TaskAgentJobRequest>();
          resultCollection.NextResult();
          updateResult.After = resultCollection.GetCurrent<TaskAgentJobRequest>().FirstOrDefault<TaskAgentJobRequest>();
          agentRequestResult = updateResult;
        }
      }
      return agentRequestResult;
    }

    public override async Task<TaskAgentJobRequest> UpdateAgentRequestMatchesAsync(
      int poolId,
      long requestId,
      IList<int> matchingAgents,
      bool matchesAllAgents)
    {
      TaskResourceComponent28 component = this;
      TaskAgentJobRequest taskAgentJobRequest;
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) component, nameof (UpdateAgentRequestMatchesAsync)))
      {
        component.PrepareStoredProcedure("Task.prc_UpdateAgentRequestMatches");
        component.BindInt("@poolId", poolId);
        component.BindLong("@requestId", requestId);
        component.BindInt32Table("@matchedAgentIds", (IEnumerable<int>) matchingAgents);
        component.BindInt("@leaseTimeoutInSeconds", (int) TimeSpan.FromMinutes(5.0).TotalSeconds);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) await component.ExecuteReaderAsync(), component.ProcedureName, component.RequestContext))
        {
          resultCollection.AddBinder<TaskAgentJobRequest>(component.CreateTaskAgentRequestBinder(poolId));
          taskAgentJobRequest = resultCollection.GetCurrent<TaskAgentJobRequest>().FirstOrDefault<TaskAgentJobRequest>();
        }
      }
      return taskAgentJobRequest;
    }

    protected virtual ObjectBinder<TaskAgentJobRequest> CreateTaskAgentRequestBinder(int poolId) => (ObjectBinder<TaskAgentJobRequest>) new TaskAgentRequestBinder10(poolId);
  }
}
