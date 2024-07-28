// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.DataAccess.TaskResourceComponent52
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

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
  internal class TaskResourceComponent52 : TaskResourceComponent51
  {
    protected override ObjectBinder<TaskAgentCloud> CreateTaskAgentCloudBinder() => (ObjectBinder<TaskAgentCloud>) new TaskAgentCloudBinder3();

    protected virtual ObjectBinder<AssignedAgentRequestResult> CreateAssignedAgentRequestResultBinder() => (ObjectBinder<AssignedAgentRequestResult>) new AssignedAgentRequestResultBinder();

    protected override ObjectBinder<TaskAgentJobRequest> CreateTaskAgentRequestBinder() => (ObjectBinder<TaskAgentJobRequest>) new TaskAgentRequestBinder14();

    protected override ObjectBinder<TaskAgentJobRequest> CreateTaskAgentRequestBinder(int poolId) => (ObjectBinder<TaskAgentJobRequest>) new TaskAgentRequestBinder14();

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
      TaskResourceComponent52 component = this;
      if (internalAgentCloud && getAgentDefinitionEndpoint == null && acquireAgentEndpoint == null && getAgentRequestStatusEndpoint == null && releaseAgentEndpoint == null)
      {
        string str = "https://azure.microsoft.com/services/devops/pipelines/";
        getAgentDefinitionEndpoint = str;
        acquireAgentEndpoint = str;
        getAgentRequestStatusEndpoint = str;
        releaseAgentEndpoint = str;
      }
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
        component.BindBoolean("@internal", internalAgentCloud);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) await component.ExecuteReaderAsync(), component.ProcedureName, component.RequestContext))
        {
          resultCollection.AddBinder<TaskAgentCloud>(component.CreateTaskAgentCloudBinder());
          taskAgentCloud = resultCollection.GetCurrent<TaskAgentCloud>().First<TaskAgentCloud>();
        }
      }
      return taskAgentCloud;
    }

    public override async Task<AssignAgentRequestsResult> AssignAgentRequestsAsync(
      TimeSpan privateLeaseTimeout,
      TimeSpan hostedLeaseTimeout,
      TimeSpan defaultLeaseTimeout,
      IList<ResourceLimit> resourceLimits,
      int maxRequestsCount,
      bool transitionAgentState)
    {
      TaskResourceComponent52 component = this;
      AssignAgentRequestsResult agentRequestsResult;
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) component, nameof (AssignAgentRequestsAsync)))
      {
        AssignAgentRequestsResult result = new AssignAgentRequestsResult();
        component.PrepareStoredProcedure("Task.prc_AssignAgentRequests");
        component.BindInt("@leaseTimeoutInSeconds", (int) privateLeaseTimeout.TotalSeconds);
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

    public override async Task<FinishAgentRequestResult> FinishAgentRequestAsync(
      int poolId,
      long requestId,
      bool deprovisionAgent,
      bool transitionAgentState,
      TaskResult? jobResult = null,
      bool agentShuttingDown = false)
    {
      TaskResourceComponent52 component = this;
      FinishAgentRequestResult agentRequestResult;
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) component, nameof (FinishAgentRequestAsync)))
      {
        component.PrepareStoredProcedure("Task.prc_FinishAgentRequest");
        component.BindInt("@poolId", poolId);
        component.BindNullableInt("@agentCloudId", new int?());
        component.BindLong("@requestId", requestId);
        component.BindInt("@leaseTimeoutInSeconds", (int) TimeSpan.FromMinutes(5.0).TotalSeconds);
        component.BindInt("@provisioningTimeoutInSeconds", (int) TimeSpan.FromMinutes(5.0).TotalSeconds);
        component.BindBoolean("@assignRequests", false);
        component.BindBoolean("@deprovisionAgent", deprovisionAgent);
        if (jobResult.HasValue)
          component.BindInt("@result", (int) (byte) jobResult.Value);
        FinishAgentRequestResult result = new FinishAgentRequestResult();
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) await component.ExecuteReaderAsync(), component.ProcedureName, component.RequestContext))
        {
          resultCollection.AddBinder<TaskAgentJobRequest>(component.CreateTaskAgentRequestBinder());
          resultCollection.AddBinder<TaskAgentJobRequest>(component.CreateTaskAgentRequestBinder());
          resultCollection.AddBinder<TaskAgentCloud>(component.CreateTaskAgentCloudBinder());
          result.CompletedRequest = resultCollection.GetCurrent<TaskAgentJobRequest>().FirstOrDefault<TaskAgentJobRequest>();
          resultCollection.NextResult();
          resultCollection.NextResult();
          result.CompletedRequestCloud = resultCollection.GetCurrent<TaskAgentCloud>().FirstOrDefault<TaskAgentCloud>();
        }
        agentRequestResult = result;
      }
      return agentRequestResult;
    }

    public override async Task<TaskAgentJobRequest> QueueAgentRequestAsync(
      int poolId,
      int? agentCloudId,
      TaskAgentJobRequest request,
      string requestResourceType)
    {
      TaskResourceComponent52 component = this;
      TaskAgentJobRequest taskAgentJobRequest;
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) component, nameof (QueueAgentRequestAsync)))
      {
        component.PrepareStoredProcedure("Task.prc_QueueAgentRequest");
        component.BindInt("@poolId", poolId);
        component.BindNullableInt("@agentCloudId", agentCloudId);
        component.BindInt32Table("@matchedAgentIds", (IEnumerable<int>) request.MatchedAgents.Select<TaskAgentReference, int>((System.Func<TaskAgentReference, int>) (x => x.Id)).Distinct<int>().ToList<int>());
        component.BindGuid("@serviceOwner", request.ServiceOwner);
        component.BindGuid("@hostId", request.HostId);
        component.BindGuid("@scopeId", request.ScopeId);
        component.BindString("@planType", request.PlanType, 260, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
        component.BindGuid("@planId", request.PlanId);
        component.BindGuid("@jobId", request.JobId);
        component.BindString("@jobName", request.JobName, 512, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
        component.BindBinary("@demands", JsonUtility.Serialize((object) request.Demands, false), int.MaxValue, SqlDbType.VarBinary);
        component.BindBinary("@agentSpecification", JsonUtility.Serialize((object) request.AgentSpecification, false), int.MaxValue, SqlDbType.VarBinary);
        component.BindNullableInt("@definitionId", request.Definition?.Id);
        component.BindBinary("@definitionReference", JsonUtility.Serialize((object) request.Definition, false), int.MaxValue, SqlDbType.VarBinary);
        component.BindNullableInt("@ownerId", request.Owner?.Id);
        component.BindBinary("@ownerReference", JsonUtility.Serialize((object) request.Owner, false), int.MaxValue, SqlDbType.VarBinary);
        component.BindInt("@leaseTimeoutInSeconds", (int) TimeSpan.FromMinutes(5.0).TotalSeconds);
        component.BindInt("@provisioningTimeoutInSeconds", (int) TimeSpan.FromMinutes(5.0).TotalSeconds);
        component.BindBinary("@data", JsonUtility.Serialize((object) request.Data, false), 1024, SqlDbType.VarBinary);
        component.BindString("@resourceType", requestResourceType, 160, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
        component.BindBoolean("@assignRequests", false);
        component.BindNullableInt("@queueId", request.QueueId);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) await component.ExecuteReaderAsync(), component.ProcedureName, component.RequestContext))
        {
          resultCollection.AddBinder<TaskAgentJobRequest>(component.CreateTaskAgentRequestBinder());
          resultCollection.AddBinder<MatchedAgent>(component.CreateMatchedAgentBinder());
          resultCollection.AddBinder<TaskAgentCloud>(component.CreateTaskAgentCloudBinder());
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

    public override async Task<TaskAgentJobRequest> UpdateAgentRequestMatchesAsync(
      int poolId,
      long requestId,
      IList<int> matchingAgents,
      bool matchesAllAgents)
    {
      TaskResourceComponent52 component = this;
      TaskAgentJobRequest taskAgentJobRequest;
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) component, nameof (UpdateAgentRequestMatchesAsync)))
      {
        component.PrepareStoredProcedure("Task.prc_UpdateAgentRequestMatches");
        component.BindInt("@poolId", poolId);
        component.BindNullableInt("@agentCloudId", new int?());
        component.BindLong("@requestId", requestId);
        component.BindInt32Table("@matchedAgentIds", (IEnumerable<int>) matchingAgents);
        component.BindInt("@leaseTimeoutInSeconds", (int) TimeSpan.FromMinutes(5.0).TotalSeconds);
        component.BindInt("@provisioningTimeoutInSeconds", (int) TimeSpan.FromMinutes(5.0).TotalSeconds);
        component.BindBoolean("@assignRequests", false);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) await component.ExecuteReaderAsync(), component.ProcedureName, component.RequestContext))
        {
          resultCollection.AddBinder<TaskAgentJobRequest>(component.CreateTaskAgentRequestBinder());
          taskAgentJobRequest = resultCollection.GetCurrent<TaskAgentJobRequest>().FirstOrDefault<TaskAgentJobRequest>();
        }
      }
      return taskAgentJobRequest;
    }
  }
}
