// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.DataAccess.TaskResourceComponent58
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.Pipelines;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.DistributedTask.Server.DataAccess
{
  internal class TaskResourceComponent58 : TaskResourceComponent57
  {
    protected override ObjectBinder<TaskAgentCloud> CreateTaskAgentCloudBinder() => (ObjectBinder<TaskAgentCloud>) new TaskAgentCloudBinder4();

    protected override ObjectBinder<AssignedAgentRequestResult> CreateAssignedAgentRequestResultBinder() => (ObjectBinder<AssignedAgentRequestResult>) new AssignedAgentRequestResultBinder3();

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
          resultCollection.AddBinder<TaskAgentJobRequest>(this.CreateTaskAgentRequestBinder());
          resultCollection.AddBinder<TaskAgentCloud>(this.CreateTaskAgentCloudBinder());
          agentSession.RecalculateRequestMatches = resultCollection.GetCurrent<bool>().First<bool>();
          resultCollection.NextResult();
          agentSession.Agent = resultCollection.GetCurrent<TaskAgent>().FirstOrDefault<TaskAgent>();
          resultCollection.NextResult();
          agentSession.OldSession = resultCollection.GetCurrent<TaskAgentSessionData>().FirstOrDefault<TaskAgentSessionData>();
          resultCollection.NextResult();
          agentSession.NewSession = resultCollection.GetCurrent<TaskAgentSessionData>().FirstOrDefault<TaskAgentSessionData>();
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

    public override TaskAgentRequestQueryResult GetAgentRequests(
      int poolId,
      int maxRequestCount,
      DateTime? lastRunningAssignTime,
      long? lastQueuedRequestId,
      DateTime? lastFinishedFinishTime)
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (GetAgentRequests)))
      {
        this.PrepareStoredProcedure("Task.prc_GetAgentRequestsForPool");
        this.BindInt("@poolId", poolId);
        this.BindInt("@maxRequestCount", maxRequestCount);
        this.BindNullableDateTime2("@lastRunningAssignTime", lastRunningAssignTime);
        this.BindNullableLong("@lastQueuedRequestId", lastQueuedRequestId);
        this.BindNullableDateTime2("@lastFinishedFinishTime", lastFinishedFinishTime);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<TaskAgentJobRequest>(this.CreateTaskAgentRequestBinder());
          resultCollection.AddBinder<TaskAgentJobRequest>(this.CreateTaskAgentRequestBinder());
          resultCollection.AddBinder<TaskAgentJobRequest>(this.CreateTaskAgentRequestBinder());
          TaskAgentRequestQueryResult requestQueryResult = new TaskAgentRequestQueryResult();
          List<TaskAgentJobRequest> items1 = resultCollection.GetCurrent<TaskAgentJobRequest>().Items;
          resultCollection.NextResult();
          IList<TaskAgentJobRequest> items2 = (IList<TaskAgentJobRequest>) resultCollection.GetCurrent<TaskAgentJobRequest>().Items;
          resultCollection.NextResult();
          IList<TaskAgentJobRequest> items3 = (IList<TaskAgentJobRequest>) resultCollection.GetCurrent<TaskAgentJobRequest>().Items;
          IList<TaskAgentJobRequest> queuedRequests = items2;
          IList<TaskAgentJobRequest> finishedRequests = items3;
          return new TaskAgentRequestQueryResult((IList<TaskAgentJobRequest>) items1, queuedRequests, finishedRequests);
        }
      }
    }

    public override async Task<TaskAgentRequestQueryResult> GetAgentRequestsAsync(
      int poolId,
      int maxRequestCount,
      DateTime? lastRunningAssignTime,
      long? lastQueuedRequestId,
      DateTime? lastFinishedFinishTime)
    {
      TaskResourceComponent58 component = this;
      TaskAgentRequestQueryResult agentRequestsAsync;
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) component, nameof (GetAgentRequestsAsync)))
      {
        component.PrepareStoredProcedure("Task.prc_GetAgentRequestsForPool");
        component.BindInt("@poolId", poolId);
        component.BindInt("@maxRequestCount", maxRequestCount);
        component.BindNullableDateTime2("@lastRunningAssignTime", lastRunningAssignTime);
        component.BindNullableLong("@lastQueuedRequestId", lastQueuedRequestId);
        component.BindNullableDateTime2("@lastFinishedFinishTime", lastFinishedFinishTime);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) await component.ExecuteReaderAsync(), component.ProcedureName, component.RequestContext))
        {
          resultCollection.AddBinder<TaskAgentJobRequest>(component.CreateTaskAgentRequestBinder());
          resultCollection.AddBinder<TaskAgentJobRequest>(component.CreateTaskAgentRequestBinder());
          resultCollection.AddBinder<TaskAgentJobRequest>(component.CreateTaskAgentRequestBinder());
          TaskAgentRequestQueryResult requestQueryResult = new TaskAgentRequestQueryResult();
          List<TaskAgentJobRequest> items1 = resultCollection.GetCurrent<TaskAgentJobRequest>().Items;
          resultCollection.NextResult();
          IList<TaskAgentJobRequest> items2 = (IList<TaskAgentJobRequest>) resultCollection.GetCurrent<TaskAgentJobRequest>().Items;
          resultCollection.NextResult();
          IList<TaskAgentJobRequest> items3 = (IList<TaskAgentJobRequest>) resultCollection.GetCurrent<TaskAgentJobRequest>().Items;
          IList<TaskAgentJobRequest> queuedRequests = items2;
          IList<TaskAgentJobRequest> finishedRequests = items3;
          agentRequestsAsync = new TaskAgentRequestQueryResult((IList<TaskAgentJobRequest>) items1, queuedRequests, finishedRequests);
        }
      }
      return agentRequestsAsync;
    }

    public override async Task<TaskAgentJobRequest> GetAgentRequestForAgentCloudRequestAsync(
      int agentCloudId,
      Guid agentCloudRequestId)
    {
      TaskResourceComponent58 component = this;
      TaskAgentJobRequest cloudRequestAsync;
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) component, nameof (GetAgentRequestForAgentCloudRequestAsync)))
      {
        component.PrepareStoredProcedure("Task.prc_GetAgentRequestForAgentCloudRequest");
        component.BindInt("@agentCloudId", agentCloudId);
        component.BindGuid("@agentCloudRequestId", agentCloudRequestId);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) await component.ExecuteReaderAsync(), component.ProcedureName, component.RequestContext))
        {
          resultCollection.AddBinder<TaskAgentJobRequest>(component.CreateTaskAgentRequestBinder());
          cloudRequestAsync = resultCollection.GetCurrent<TaskAgentJobRequest>().FirstOrDefault<TaskAgentJobRequest>();
        }
      }
      return cloudRequestAsync;
    }

    public override async Task<AssignAgentRequestsResult> AssignAgentRequestsAsync(
      TimeSpan privateLeaseTimeout,
      TimeSpan hostedLeaseTimeout,
      TimeSpan defaultLeaseTimeout,
      IList<ResourceLimit> resourceLimits,
      int maxRequestsCount,
      bool transitionAgentState)
    {
      TaskResourceComponent58 component = this;
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
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) await component.ExecuteReaderAsync(), component.ProcedureName, component.RequestContext))
        {
          resultCollection.AddBinder<AssignedAgentRequestResult>(component.CreateAssignedAgentRequestResultBinder());
          resultCollection.AddBinder<ResourceUsageData>((ObjectBinder<ResourceUsageData>) new ResourceUsageDataBinder());
          result.AssignedRequestResults.AddRange((IEnumerable<AssignedAgentRequestResult>) resultCollection.GetCurrent<AssignedAgentRequestResult>().Items);
          resultCollection.NextResult();
          result.ResourceUsageDataCollection.AddRange((IEnumerable<ResourceUsageData>) resultCollection.GetCurrent<ResourceUsageData>().Items);
        }
        agentRequestsResult = result;
      }
      return agentRequestsResult;
    }

    public override async Task<TaskAgentCloud> AddAgentCloudAsync(
      Guid id,
      string name,
      string type,
      string getAgentDefinitionEndpoint,
      string acquireAgentEndpoint,
      string getAgentRequestStatusEndpoint,
      string releaseAgentEndpoint,
      string getAccountParallelismEndpoint,
      int? maxParallelism,
      int? acquisitionTimeout,
      bool internalAgentCloud = false)
    {
      TaskResourceComponent58 component = this;
      TaskAgentCloud taskAgentCloud;
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) component, nameof (AddAgentCloudAsync)))
      {
        component.PrepareStoredProcedure("Task.prc_AddAgentCloud");
        component.BindString("@name", name, 128, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
        component.BindString("@type", type, 128, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
        component.BindString("@getAgentDefinitionEndpoint", getAgentDefinitionEndpoint, 128, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
        component.BindString("@acquireAgentEndpoint", acquireAgentEndpoint, 128, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
        component.BindString("@getAgentRequestStatusEndpoint", getAgentRequestStatusEndpoint, 128, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
        component.BindString("@releaseAgentEndpoint", releaseAgentEndpoint, 128, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
        component.BindNullableInt("@acquisitionTimeout", acquisitionTimeout);
        component.BindBoolean("@internal", internalAgentCloud);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) await component.ExecuteReaderAsync(), component.ProcedureName, component.RequestContext))
        {
          resultCollection.AddBinder<TaskAgentCloud>(component.CreateTaskAgentCloudBinder());
          taskAgentCloud = resultCollection.GetCurrent<TaskAgentCloud>().First<TaskAgentCloud>();
        }
      }
      return taskAgentCloud;
    }
  }
}
