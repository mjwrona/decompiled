// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.DataAccess.TaskResourceComponent64
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.Pipelines;
using Microsoft.TeamFoundation.DistributedTask.Tasks;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.DistributedTask.Server.DataAccess
{
  internal class TaskResourceComponent64 : TaskResourceComponent63
  {
    public ObjectBinder<ProvisionAgentEvent> CreateProvisionAgentEventBinder() => (ObjectBinder<ProvisionAgentEvent>) new ProvisionAgentEventBinder();

    public ObjectBinder<RequestAssignedEvent> CreateRequestAssignedEventBinder() => (ObjectBinder<RequestAssignedEvent>) new RequestAssignedEventBinder();

    public ObjectBinder<RequestFinishedEvent> CreateRequestFinishedEventBinder() => (ObjectBinder<RequestFinishedEvent>) new RequestFinishedEventBinder();

    public ObjectBinder<DeprovisionEvent> CreateDeprovisionEventBinder() => (ObjectBinder<DeprovisionEvent>) new DeprovisionEventBinder();

    public ObjectBinder<AgentConnectedEvent> CreateAgentConnectedEventBinder() => (ObjectBinder<AgentConnectedEvent>) new AgentConnecteEventBinder();

    public override async Task<AssignAgentRequestsResult> AssignAgentRequestsAsync(
      TimeSpan privateLeaseTimeout,
      TimeSpan hostedLeaseTimeout,
      TimeSpan defaultLeaseTimeout,
      IList<ResourceLimit> resourceLimits,
      int maxRequestsCount,
      bool transitionAgentState)
    {
      TaskResourceComponent64 component = this;
      AssignAgentRequestsResult agentRequestsResult;
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) component, nameof (AssignAgentRequestsAsync)))
      {
        AssignAgentRequestsResult result = new AssignAgentRequestsResult();
        component.PrepareStoredProcedure("Task.prc_AssignAgentRequests");
        component.BindInt("@privateLeaseTimeout", (int) privateLeaseTimeout.TotalSeconds);
        component.BindInt("@hostedLeaseTimeout", (int) hostedLeaseTimeout.TotalSeconds);
        component.BindInt("@defaultLeaseTimeout", (int) defaultLeaseTimeout.TotalSeconds);
        component.BindResourceLimitsTable("@resourceLimits", resourceLimits);
        component.BindInt("@maxRequestsCount", maxRequestsCount);
        component.BindBoolean("@transitionAgentState", transitionAgentState);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) await component.ExecuteReaderAsync(), component.ProcedureName, component.RequestContext))
        {
          resultCollection.AddBinder<AssignedAgentRequestResult>(component.CreateAssignedAgentRequestResultBinder());
          resultCollection.AddBinder<ResourceUsageData>((ObjectBinder<ResourceUsageData>) new ResourceUsageDataBinder());
          resultCollection.AddBinder<ProvisionAgentEvent>(component.CreateProvisionAgentEventBinder());
          resultCollection.AddBinder<RequestAssignedEvent>(component.CreateRequestAssignedEventBinder());
          resultCollection.AddBinder<DeprovisionEvent>(component.CreateDeprovisionEventBinder());
          result.AssignedRequestResults.AddRange((IEnumerable<AssignedAgentRequestResult>) resultCollection.GetCurrent<AssignedAgentRequestResult>().Items);
          resultCollection.NextResult();
          result.ResourceUsageDataCollection.AddRange((IEnumerable<ResourceUsageData>) resultCollection.GetCurrent<ResourceUsageData>().Items);
          resultCollection.NextResult();
          result.Events.AddRange((IEnumerable<RunAgentEvent>) resultCollection.GetCurrent<ProvisionAgentEvent>().Items);
          resultCollection.NextResult();
          result.Events.AddRange((IEnumerable<RunAgentEvent>) resultCollection.GetCurrent<RequestAssignedEvent>().Items);
          resultCollection.NextResult();
          result.Events.AddRange((IEnumerable<RunAgentEvent>) resultCollection.GetCurrent<DeprovisionEvent>().Items);
        }
        agentRequestsResult = result;
      }
      return agentRequestsResult;
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
        this.BindString("@osDescription", agent.OSDescription, 512, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
        this.BindString("@accessPoint", agent.AccessPoint, 512, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
        this.BindString("@ownerName", ownerName, 512, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
        this.BindGuid("@writerId", this.Author);
        this.BindBoolean("@transitionAgentState", transitionAgentState);
        if (systemCapabilities != null && systemCapabilities.Count > 0)
        {
          systemCapabilities.Remove(PipelineConstants.AgentName);
          systemCapabilities.Remove(PipelineConstants.AgentVersionDemandName);
        }
        this.BindKeyValuePairStringTable("@systemCapabilities", (IEnumerable<KeyValuePair<string, string>>) systemCapabilities);
        CreateAgentSessionResult agentSession = new CreateAgentSessionResult();
        agentSession.ComponentRetrunsAgentCloud = true;
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<bool>((ObjectBinder<bool>) new BooleanBinder());
          resultCollection.AddBinder<TaskAgent>(this.CreateTaskAgentBinder());
          resultCollection.AddBinder<TaskAgentSessionData>(this.CreateTaskAgentSessionBinder());
          resultCollection.AddBinder<TaskAgentSessionData>(this.CreateTaskAgentSessionBinder());
          resultCollection.AddBinder<AgentConnectedEvent>(this.CreateAgentConnectedEventBinder());
          resultCollection.AddBinder<TaskAgentJobRequest>(this.CreateTaskAgentRequestBinder());
          resultCollection.AddBinder<TaskAgentCloud>(this.CreateTaskAgentCloudBinder());
          agentSession.RecalculateRequestMatches = resultCollection.GetCurrent<bool>().First<bool>();
          resultCollection.NextResult();
          agentSession.Agent = resultCollection.GetCurrent<TaskAgent>().FirstOrDefault<TaskAgent>();
          resultCollection.NextResult();
          agentSession.OldSession = resultCollection.GetCurrent<TaskAgentSessionData>().FirstOrDefault<TaskAgentSessionData>();
          resultCollection.NextResult();
          agentSession.NewSession = resultCollection.GetCurrent<TaskAgentSessionData>().FirstOrDefault<TaskAgentSessionData>();
          resultCollection.NextResult();
          agentSession.OrchestrationEvent = resultCollection.GetCurrent<AgentConnectedEvent>().Items.FirstOrDefault<AgentConnectedEvent>();
          if (resultCollection.TryNextResult())
          {
            agentSession.AssignedRequest = resultCollection.GetCurrent<TaskAgentJobRequest>().FirstOrDefault<TaskAgentJobRequest>();
            resultCollection.NextResult();
            agentSession.AssignedRequestAgentCloud = resultCollection.GetCurrent<TaskAgentCloud>().FirstOrDefault<TaskAgentCloud>();
          }
          return agentSession;
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
      TaskResourceComponent64 component = this;
      FinishAgentRequestResult agentRequestResult;
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) component, nameof (FinishAgentRequestAsync)))
      {
        component.PrepareStoredProcedure("Task.prc_FinishAgentRequest");
        component.BindInt("@poolId", poolId);
        component.BindLong("@requestId", requestId);
        component.BindBoolean("@deprovisionAgent", deprovisionAgent);
        component.BindBoolean("@transitionAgentState", transitionAgentState);
        if (jobResult.HasValue)
          component.BindInt("@result", (int) (byte) jobResult.Value);
        FinishAgentRequestResult result = new FinishAgentRequestResult();
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) await component.ExecuteReaderAsync(), component.ProcedureName, component.RequestContext))
        {
          resultCollection.AddBinder<TaskAgentJobRequest>(component.CreateTaskAgentRequestBinder());
          resultCollection.AddBinder<TaskAgentCloud>(component.CreateTaskAgentCloudBinder());
          resultCollection.AddBinder<RequestFinishedEvent>(component.CreateRequestFinishedEventBinder());
          result.CompletedRequest = resultCollection.GetCurrent<TaskAgentJobRequest>().FirstOrDefault<TaskAgentJobRequest>();
          resultCollection.NextResult();
          result.CompletedRequestCloud = resultCollection.GetCurrent<TaskAgentCloud>().FirstOrDefault<TaskAgentCloud>();
          resultCollection.NextResult();
          result.OrchestrationEvent = resultCollection.GetCurrent<RequestFinishedEvent>().FirstOrDefault<RequestFinishedEvent>();
        }
        agentRequestResult = result;
      }
      return agentRequestResult;
    }

    public override async Task<IList<TaskAgentCloudRequest>> GetActiveAgentCloudRequestsAsync()
    {
      TaskResourceComponent64 component = this;
      IList<TaskAgentCloudRequest> items;
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) component, nameof (GetActiveAgentCloudRequestsAsync)))
      {
        component.PrepareStoredProcedure("Task.prc_GetActiveAgentCloudRequests");
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) await component.ExecuteReaderAsync(), component.ProcedureName, component.RequestContext))
        {
          resultCollection.AddBinder<TaskAgentCloudRequest>(component.CreateTaskAgentCloudRequestBinder());
          items = (IList<TaskAgentCloudRequest>) resultCollection.GetCurrent<TaskAgentCloudRequest>().Items;
        }
      }
      return items;
    }

    public override async Task<TaskAgentCloudRequest> UpdateAgentCloudRequestAsync(
      int agentCloudId,
      Guid requestId,
      JObject agentSpecification,
      JObject agentData,
      DateTime? provisionRequestTime,
      DateTime? provisionedTime,
      DateTime? agentConnectedTime,
      DateTime? releaseRequestTime)
    {
      TaskResourceComponent64 component = this;
      TaskAgentCloudRequest agentCloudRequest;
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) component, nameof (UpdateAgentCloudRequestAsync)))
      {
        component.PrepareStoredProcedure("Task.prc_UpdateAgentCloudRequest");
        component.BindInt("@agentCloudId", agentCloudId);
        component.BindGuid("@requestId", requestId);
        component.BindBinary("@agentSpecification", JsonUtility.Serialize((object) agentSpecification, false), SqlDbType.VarBinary);
        component.BindBinary("@agentData", JsonUtility.Serialize((object) agentData, false), SqlDbType.VarBinary);
        component.BindNullableDateTime2("@provisionRequestTime", provisionRequestTime);
        component.BindNullableDateTime2("@provisionedTime", provisionedTime);
        component.BindNullableDateTime2("@agentConnectedTime", agentConnectedTime);
        component.BindNullableDateTime2("@releaseRequestTime", releaseRequestTime);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) await component.ExecuteReaderAsync(), component.ProcedureName, component.RequestContext))
        {
          resultCollection.AddBinder<TaskAgentCloudRequest>(component.CreateTaskAgentCloudRequestBinder());
          agentCloudRequest = resultCollection.GetCurrent<TaskAgentCloudRequest>().First<TaskAgentCloudRequest>();
        }
      }
      return agentCloudRequest;
    }
  }
}
