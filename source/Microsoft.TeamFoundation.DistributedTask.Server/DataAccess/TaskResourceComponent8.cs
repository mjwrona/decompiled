// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.DataAccess.TaskResourceComponent8
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
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.DistributedTask.Server.DataAccess
{
  internal class TaskResourceComponent8 : TaskResourceComponent7
  {
    protected virtual ObjectBinder<TaskAgentSessionData> CreateTaskAgentSessionBinder() => (ObjectBinder<TaskAgentSessionData>) new TaskAgentSessionDataBinder2();

    public override TaskAgentJobRequest AbandonAgentRequest(
      int poolId,
      long requestId,
      DateTime expirationTime)
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (AbandonAgentRequest)))
      {
        this.PrepareStoredProcedure("Task.prc_AbandonAgentJobRequest");
        this.BindInt("@poolId", poolId);
        this.BindLong("@requestId", requestId);
        this.BindDateTime2("@expirationTime", expirationTime);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<TaskAgentJobRequest>((ObjectBinder<TaskAgentJobRequest>) new TaskAgentRequestBinder6());
          return resultCollection.GetCurrent<TaskAgentJobRequest>().FirstOrDefault<TaskAgentJobRequest>();
        }
      }
    }

    public override CreateAgentResult AddAgent(int poolId, TaskAgent agent, bool createEnabled = true)
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (AddAgent)))
      {
        this.PrepareStoredProcedure("Task.prc_AddAgent");
        this.BindInt("@poolId", poolId);
        this.BindString("@name", agent.Name, 128, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
        this.BindString("@version", agent.Version, 128, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
        this.BindNullableInt("@maxParallelism", agent.MaxParallelism);
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
          resultCollection.AddBinder<TaskAgentPoolData>((ObjectBinder<TaskAgentPoolData>) new TaskAgentPoolBinder4(this.RequestContext));
          resultCollection.AddBinder<TaskAgent>((ObjectBinder<TaskAgent>) new TaskAgentBinder4());
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
        this.BindGuid("@createdBy", createdBy);
        this.BindBoolean("@isHosted", isHosted);
        this.BindBoolean("@autoProvision", autoProvision);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<TaskAgentPoolData>((ObjectBinder<TaskAgentPoolData>) new TaskAgentPoolBinder4(this.RequestContext));
          return resultCollection.GetCurrent<TaskAgentPoolData>().First<TaskAgentPoolData>();
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
          resultCollection.AddBinder<TaskAgentSessionData>(this.CreateTaskAgentSessionBinder());
          resultCollection.AddBinder<TaskAgentSessionData>(this.CreateTaskAgentSessionBinder());
          agentSession.RecalculateRequestMatches = resultCollection.GetCurrent<bool>().First<bool>();
          resultCollection.NextResult();
          agentSession.OldSession = resultCollection.GetCurrent<TaskAgentSessionData>().FirstOrDefault<TaskAgentSessionData>();
          resultCollection.NextResult();
          agentSession.NewSession = resultCollection.GetCurrent<TaskAgentSessionData>().FirstOrDefault<TaskAgentSessionData>();
          return agentSession;
        }
      }
    }

    public override DeleteAgentResult DeleteAgents(int poolId, IEnumerable<int> agentIds)
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (DeleteAgents)))
      {
        this.PrepareStoredProcedure("Task.prc_DeleteAgents");
        this.BindInt("@poolId", poolId);
        this.BindInt32Table("@agentIds", agentIds);
        this.BindGuid("@writerId", this.Author);
        DeleteAgentResult deleteAgentResult = new DeleteAgentResult();
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<TaskAgent>((ObjectBinder<TaskAgent>) new TaskAgentBinder4());
          resultCollection.AddBinder<TaskAgentSessionData>((ObjectBinder<TaskAgentSessionData>) new TaskAgentSessionDataBinder2());
          deleteAgentResult.DeletedAgents = (IList<TaskAgent>) resultCollection.GetCurrent<TaskAgent>().Items;
          resultCollection.NextResult();
          deleteAgentResult.DeletedSessions = (IList<TaskAgentSessionData>) resultCollection.GetCurrent<TaskAgentSessionData>().Items;
        }
        deleteAgentResult.PoolData = this.GetTaskAgentPoolData(poolId);
        return deleteAgentResult;
      }
    }

    public override DeleteAgentPoolResult DeleteAgentPool(int poolId)
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (DeleteAgentPool)))
      {
        this.PrepareStoredProcedure("Task.prc_DeleteAgentPool");
        this.BindInt("@poolId", poolId);
        this.BindGuid("@writerId", this.Author);
        DeleteAgentPoolResult deleteAgentPoolResult = new DeleteAgentPoolResult();
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<TaskAgent>((ObjectBinder<TaskAgent>) new TaskAgentBinder4());
          resultCollection.AddBinder<TaskAgentSessionData>((ObjectBinder<TaskAgentSessionData>) new TaskAgentSessionDataBinder2());
          resultCollection.AddBinder<TaskAgentPoolData>((ObjectBinder<TaskAgentPoolData>) new TaskAgentPoolBinder4(this.RequestContext));
          deleteAgentPoolResult.DeletedAgents = (IList<TaskAgent>) resultCollection.GetCurrent<TaskAgent>().Items;
          resultCollection.NextResult();
          deleteAgentPoolResult.DeletedSessions = (IList<TaskAgentSessionData>) resultCollection.GetCurrent<TaskAgentSessionData>().Items;
          resultCollection.NextResult();
          deleteAgentPoolResult.PoolData = resultCollection.GetCurrent<TaskAgentPoolData>().FirstOrDefault<TaskAgentPoolData>();
          return deleteAgentPoolResult;
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
      TaskResourceComponent8 component = this;
      FinishAgentRequestResult agentRequestResult;
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) component, nameof (FinishAgentRequestAsync)))
      {
        component.PrepareStoredProcedure("Task.prc_FinishAgentJobRequest");
        component.BindInt("@poolId", poolId);
        component.BindLong("@requestId", requestId);
        using (new ResultCollection((IDataReader) await component.ExecuteReaderAsync(), component.ProcedureName, component.RequestContext))
          agentRequestResult = new FinishAgentRequestResult();
      }
      return agentRequestResult;
    }

    public override TaskAgentRequestData GetAgentRequest(
      int poolId,
      long requestId,
      bool includeStatus = false)
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (GetAgentRequest)))
      {
        TaskAgentRequestData agentRequest1 = (TaskAgentRequestData) null;
        this.PrepareStoredProcedure("Task.prc_GetAgentJobRequest");
        this.BindInt("@poolId", poolId);
        this.BindLong("@requestId", requestId);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<TaskAgentJobRequest>((ObjectBinder<TaskAgentJobRequest>) new TaskAgentRequestBinder6());
          TaskAgentJobRequest agentRequest2 = resultCollection.GetCurrent<TaskAgentJobRequest>().FirstOrDefault<TaskAgentJobRequest>();
          if (agentRequest2 != null)
            agentRequest1 = new TaskAgentRequestData(agentRequest2);
        }
        return agentRequest1;
      }
    }

    public override IList<TaskAgentSessionData> GetAgentSessions(int? poolId = null, Guid? sessionId = null)
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (GetAgentSessions)))
      {
        this.PrepareStoredProcedure("Task.prc_GetAgentSessions");
        this.BindNullableInt("@poolId", poolId);
        this.BindNullableGuid("@sessionId", sessionId);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<TaskAgentSessionData>(this.CreateTaskAgentSessionBinder());
          return (IList<TaskAgentSessionData>) resultCollection.GetCurrent<TaskAgentSessionData>().ToList<TaskAgentSessionData>();
        }
      }
    }

    public override async Task<ExpiredAgentRequestsResult> GetExpiredAgentRequestsAsync(int poolId)
    {
      TaskResourceComponent8 component = this;
      ExpiredAgentRequestsResult agentRequestsAsync;
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) component, nameof (GetExpiredAgentRequestsAsync)))
      {
        component.PrepareStoredProcedure("Task.prc_GetExpiredAgentJobRequests");
        component.BindInt("@poolId", poolId);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) await component.ExecuteReaderAsync(), component.ProcedureName, component.RequestContext))
        {
          resultCollection.AddBinder<TaskAgentJobRequest>((ObjectBinder<TaskAgentJobRequest>) new TaskAgentRequestBinder6());
          agentRequestsAsync = new ExpiredAgentRequestsResult(true, (IList<TaskAgentJobRequest>) resultCollection.GetCurrent<TaskAgentJobRequest>().Items);
        }
      }
      return agentRequestsAsync;
    }

    public override async Task<IList<TaskAgentJobRequest>> GetUnassignedAgentRequestsAsync(
      int poolId)
    {
      TaskResourceComponent8 component = this;
      IList<TaskAgentJobRequest> items;
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) component, nameof (GetUnassignedAgentRequestsAsync)))
      {
        component.PrepareStoredProcedure("Task.prc_GetUnassignedAgentJobRequests");
        component.BindInt("@poolId", poolId);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) await component.ExecuteReaderAsync(), component.ProcedureName, component.RequestContext))
        {
          resultCollection.AddBinder<TaskAgentJobRequest>((ObjectBinder<TaskAgentJobRequest>) new TaskAgentRequestBinder6());
          items = (IList<TaskAgentJobRequest>) resultCollection.GetCurrent<TaskAgentJobRequest>().Items;
        }
      }
      return items;
    }

    protected override TaskAgent GetAgent(
      int poolId,
      int agentId,
      bool includeCapabilities = false,
      bool includeAssignedRequest = false)
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (GetAgent)))
      {
        this.PrepareStoredProcedure("Task.prc_GetAgent");
        this.BindInt("@poolId", poolId);
        this.BindInt("@agentId", agentId);
        this.BindBoolean("@includeCapabilities", includeCapabilities);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<TaskAgent>((ObjectBinder<TaskAgent>) new TaskAgentBinder4());
          resultCollection.AddBinder<TaskAgentCapability>((ObjectBinder<TaskAgentCapability>) new TaskAgentCapabilityBinder());
          TaskAgent agent = resultCollection.GetCurrent<TaskAgent>().FirstOrDefault<TaskAgent>();
          if (includeCapabilities)
            agent = agent.PopulateImplicitCapabilities();
          if (includeCapabilities && resultCollection.TryNextResult())
          {
            foreach (TaskAgentCapability taskAgentCapability in resultCollection.GetCurrent<TaskAgentCapability>().Items.Where<TaskAgentCapability>((System.Func<TaskAgentCapability, bool>) (x => !x.IsWellKnownCapability())))
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
          resultCollection.AddBinder<TaskAgent>((ObjectBinder<TaskAgent>) new TaskAgentBinder4());
          resultCollection.AddBinder<TaskAgentCapability>((ObjectBinder<TaskAgentCapability>) new TaskAgentCapabilityBinder());
          List<TaskAgent> items = resultCollection.GetCurrent<TaskAgent>().Items;
          if (includeCapabilities && resultCollection.TryNextResult())
          {
            Dictionary<int, TaskAgent> dictionary = items.ToDictionary<TaskAgent, int, TaskAgent>((System.Func<TaskAgent, int>) (x => x.Id), (System.Func<TaskAgent, TaskAgent>) (x => !includeCapabilities ? x : x.PopulateImplicitCapabilities()));
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
          return new TaskAgentList(new bool?(), (IList<TaskAgent>) items);
        }
      }
    }

    public override TaskAgentPoolData GetAgentPool(int poolId)
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (GetAgentPool)))
      {
        this.PrepareStoredProcedure("Task.prc_GetAgentPool");
        this.BindInt("@poolId", poolId);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<TaskAgentPoolData>((ObjectBinder<TaskAgentPoolData>) new TaskAgentPoolBinder4(this.RequestContext));
          return resultCollection.GetCurrent<TaskAgentPoolData>().FirstOrDefault<TaskAgentPoolData>();
        }
      }
    }

    public override IEnumerable<TaskAgentPoolData> GetAgentPools(
      string poolName,
      TaskAgentPoolType poolType = TaskAgentPoolType.Automation)
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (GetAgentPools)))
      {
        this.PrepareStoredProcedure("Task.prc_GetAgentPools");
        this.BindString("@poolName", poolName, 128, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<TaskAgentPoolData>((ObjectBinder<TaskAgentPoolData>) new TaskAgentPoolBinder4(this.RequestContext));
          return (IEnumerable<TaskAgentPoolData>) resultCollection.GetCurrent<TaskAgentPoolData>().Items;
        }
      }
    }

    public override async Task<IList<TaskAgentPoolData>> GetAgentPoolsWithCapabilityChangesAsync()
    {
      TaskResourceComponent8 component = this;
      IList<TaskAgentPoolData> items;
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) component, nameof (GetAgentPoolsWithCapabilityChangesAsync)))
      {
        component.PrepareStoredProcedure("Task.prc_GetAgentPoolsWithCapabilityChanges");
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) await component.ExecuteReaderAsync(), component.ProcedureName, component.RequestContext))
        {
          resultCollection.AddBinder<TaskAgentPoolData>((ObjectBinder<TaskAgentPoolData>) new TaskAgentPoolBinder4(component.RequestContext));
          items = (IList<TaskAgentPoolData>) resultCollection.GetCurrent<TaskAgentPoolData>().Items;
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
      TaskResourceComponent8 component = this;
      TaskAgentJobRequest taskAgentJobRequest;
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) component, nameof (QueueAgentRequestAsync)))
      {
        component.PrepareStoredProcedure("prc_QueueTaskAgentJobRequest");
        component.BindInt("@poolId", poolId);
        component.BindInt32Table("@matchedAgentIds", (IEnumerable<int>) request.MatchedAgents.Select<TaskAgentReference, int>((System.Func<TaskAgentReference, int>) (x => x.Id)).Distinct<int>().ToList<int>());
        component.BindGuid("@serviceOwner", request.ServiceOwner);
        component.BindGuid("@hostId", request.HostId);
        component.BindGuid("@scopeId", request.ScopeId);
        component.BindString("@planType", request.PlanType, 260, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
        component.BindGuid("@planId", request.PlanId);
        component.BindGuid("@jobId", request.JobId);
        component.BindBinary("@demands", JsonUtility.Serialize((object) request.Demands, false), int.MaxValue, SqlDbType.VarBinary);
        component.BindString("@orchestrationId", string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0:N}_{1:N}", (object) request.PlanId, (object) request.JobId), 256, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) await component.ExecuteReaderAsync(), component.ProcedureName, component.RequestContext))
        {
          resultCollection.AddBinder<TaskAgentJobRequest>((ObjectBinder<TaskAgentJobRequest>) new TaskAgentRequestBinder6());
          taskAgentJobRequest = resultCollection.GetCurrent<TaskAgentJobRequest>().FirstOrDefault<TaskAgentJobRequest>();
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
        this.ExecuteNonQuery();
        return new AgentConnectivityResult()
        {
          HandledEvent = true
        };
      }
    }

    public override UpdateAgentResult UpdateAgent(
      int poolId,
      TaskAgent agent,
      TaskAgentCapabilityType capabilityUpdate)
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (UpdateAgent)))
      {
        this.PrepareStoredProcedure("Task.prc_UpdateAgent");
        this.BindInt("@poolId", poolId);
        this.BindInt("@agentId", agent.Id);
        this.BindString("@name", agent.Name, 128, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
        this.BindString("@version", agent.Version, 128, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
        this.BindNullableBoolean("@enabled", agent.Enabled);
        this.BindNullableInt("@maxParallelism", agent.MaxParallelism);
        this.BindByte("@capabilityUpdate", (byte) capabilityUpdate);
        if (agent.SystemCapabilities.Count > 0)
        {
          agent.SystemCapabilities.Remove(PipelineConstants.AgentName);
          agent.SystemCapabilities.Remove(PipelineConstants.AgentVersionDemandName);
        }
        if ((capabilityUpdate & TaskAgentCapabilityType.System) == TaskAgentCapabilityType.System)
          this.BindKeyValuePairStringTable("@systemCapabilities", (IEnumerable<KeyValuePair<string, string>>) agent.SystemCapabilities);
        else
          this.BindKeyValuePairStringTable("@systemCapabilities", (IEnumerable<KeyValuePair<string, string>>) new KeyValuePair<string, string>[0]);
        if ((capabilityUpdate & TaskAgentCapabilityType.User) == TaskAgentCapabilityType.User)
          this.BindKeyValuePairStringTable("@userCapabilities", (IEnumerable<KeyValuePair<string, string>>) agent.UserCapabilities);
        else
          this.BindKeyValuePairStringTable("@userCapabilities", (IEnumerable<KeyValuePair<string, string>>) new KeyValuePair<string, string>[0]);
        UpdateAgentResult updateAgentResult = new UpdateAgentResult();
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<bool>((ObjectBinder<bool>) new BooleanBinder());
          resultCollection.AddBinder<TaskAgentPoolData>((ObjectBinder<TaskAgentPoolData>) new TaskAgentPoolBinder4(this.RequestContext));
          resultCollection.AddBinder<TaskAgent>((ObjectBinder<TaskAgent>) new TaskAgentBinder4());
          updateAgentResult.RecalculateRequestMatches = resultCollection.GetCurrent<bool>().First<bool>();
          resultCollection.NextResult();
          updateAgentResult.PoolData = resultCollection.GetCurrent<TaskAgentPoolData>().FirstOrDefault<TaskAgentPoolData>();
          resultCollection.NextResult();
          updateAgentResult.Agent = resultCollection.GetCurrent<TaskAgent>().First<TaskAgent>().PopulateImplicitCapabilities();
          foreach (KeyValuePair<string, string> systemCapability in (IEnumerable<KeyValuePair<string, string>>) agent.SystemCapabilities)
            updateAgentResult.Agent.SystemCapabilities.Add(systemCapability.Key, systemCapability.Value);
          foreach (KeyValuePair<string, string> userCapability in (IEnumerable<KeyValuePair<string, string>>) agent.UserCapabilities)
            updateAgentResult.Agent.UserCapabilities.Add(userCapability.Key, userCapability.Value);
          return updateAgentResult;
        }
      }
    }

    public override UpdateAgentRequestResult UpdateAgentRequest(
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
        this.PrepareStoredProcedure("Task.prc_UpdateAgentJobRequest");
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
          resultCollection.AddBinder<TaskAgentJobRequest>((ObjectBinder<TaskAgentJobRequest>) new TaskAgentRequestBinder6());
          agentRequestResult.After = resultCollection.GetCurrent<TaskAgentJobRequest>().FirstOrDefault<TaskAgentJobRequest>();
          return agentRequestResult;
        }
      }
    }

    public override async Task<TaskAgentJobRequest> UpdateAgentRequestMatchesAsync(
      int poolId,
      long requestId,
      IList<int> matchingAgents,
      bool matchaseAllAgents)
    {
      TaskResourceComponent8 component = this;
      TaskAgentJobRequest taskAgentJobRequest;
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) component, nameof (UpdateAgentRequestMatchesAsync)))
      {
        component.PrepareStoredProcedure("Task.prc_UpdateAgentJobRequestMatches");
        component.BindInt("@poolId", poolId);
        component.BindLong("@requestId", requestId);
        component.BindInt32Table("@matchedAgentIds", (IEnumerable<int>) matchingAgents);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) await component.ExecuteReaderAsync(), component.ProcedureName, component.RequestContext))
        {
          resultCollection.AddBinder<TaskAgentJobRequest>((ObjectBinder<TaskAgentJobRequest>) new TaskAgentRequestBinder6());
          taskAgentJobRequest = resultCollection.GetCurrent<TaskAgentJobRequest>().FirstOrDefault<TaskAgentJobRequest>();
        }
      }
      return taskAgentJobRequest;
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
        this.BindNullableGuid("@groupScopeId", groupScopeId);
        this.BindNullableGuid("@administratorsGroupId", administratorsGroupId);
        this.BindNullableGuid("@serviceAccountsGroupId", serviceAccountsGroupId);
        this.BindNullableGuid("@serviceIdentityId", serviceIdentityId);
        this.BindNullableBoolean("@isHosted", isHosted);
        this.BindNullableBoolean("@autoProvision", autoProvision);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          UpdateAgentPoolResult updateAgentPoolResult = new UpdateAgentPoolResult();
          resultCollection.AddBinder<TaskAgentPoolData>((ObjectBinder<TaskAgentPoolData>) new TaskAgentPoolBinder4(this.RequestContext));
          updateAgentPoolResult.UpdatedPoolData = resultCollection.GetCurrent<TaskAgentPoolData>().FirstOrDefault<TaskAgentPoolData>();
          return updateAgentPoolResult;
        }
      }
    }

    protected virtual TaskAgentPoolData GetTaskAgentPoolData(int poolId) => throw new NotImplementedException();
  }
}
