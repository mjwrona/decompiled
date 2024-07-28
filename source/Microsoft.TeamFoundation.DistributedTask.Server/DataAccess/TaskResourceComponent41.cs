// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.DataAccess.TaskResourceComponent41
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.DistributedTask.Server.DataAccess
{
  internal class TaskResourceComponent41 : TaskResourceComponent40
  {
    protected override ObjectBinder<TaskAgentJobRequest> CreateTaskAgentRequestBinder(int poolId) => (ObjectBinder<TaskAgentJobRequest>) new TaskAgentRequestBinder12();

    protected override ObjectBinder<TaskAgentJobRequest> CreateTaskAgentRequestBinder() => (ObjectBinder<TaskAgentJobRequest>) new TaskAgentRequestBinder12();

    public override IList<TaskAgentJobRequest> GetAgentRequestsForAgents(
      int poolId,
      IList<int> agentIds,
      Guid hostId,
      Guid scopeId,
      int completedRequestCount,
      int? ownerId = null,
      DateTime? completedOn = null)
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (GetAgentRequestsForAgents)))
      {
        this.PrepareStoredProcedure("Task.prc_GetAgentRequestsByFilter");
        this.BindInt("@poolId", poolId);
        this.BindInt32Table("@agentIds", (IEnumerable<int>) agentIds);
        this.BindGuid("@hostId", hostId);
        this.BindGuid("@scopeId", scopeId);
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

    public override async Task<TaskAgentJobRequest> QueueAgentRequestAsync(
      int poolId,
      int? agentCloudId,
      TaskAgentJobRequest request,
      string requestResourceType)
    {
      TaskResourceComponent41 component = this;
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
        TaskResourceComponent41 resourceComponent41_1 = component;
        TaskOrchestrationOwner definition = request.Definition;
        int? parameterValue1 = new int?(definition != null ? definition.Id : 0);
        resourceComponent41_1.BindNullableInt("@definitionId", parameterValue1);
        component.BindBinary("@definitionReference", JsonUtility.Serialize((object) request.Definition, false), int.MaxValue, SqlDbType.VarBinary);
        TaskResourceComponent41 resourceComponent41_2 = component;
        TaskOrchestrationOwner owner = request.Owner;
        int? parameterValue2 = new int?(owner != null ? owner.Id : 0);
        resourceComponent41_2.BindNullableInt("@ownerId", parameterValue2);
        component.BindBinary("@ownerReference", JsonUtility.Serialize((object) request.Owner, false), int.MaxValue, SqlDbType.VarBinary);
        component.BindInt("@leaseTimeoutInSeconds", (int) TimeSpan.FromMinutes(5.0).TotalSeconds);
        component.BindBinary("@data", JsonUtility.Serialize((object) request.Data, false), 1024, SqlDbType.VarBinary);
        component.BindString("@planGroup", (string) null, 160, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
        component.BindString("@resourceType", requestResourceType, 160, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
        component.BindBoolean("@assignRequests", false);
        component.BindNullableInt("@queueId", request.QueueId);
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

    public override GetAgentQueuesResult GetAgentQueuesByPoolIds(
      IEnumerable<int> poolIds,
      TaskAgentQueueType queueType = TaskAgentQueueType.Automation)
    {
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (GetAgentQueuesByPoolIds)))
      {
        this.PrepareStoredProcedure("Task.prc_GetQueuesByPoolIds");
        this.BindByte("@queueType", (byte) queueType);
        this.BindInt32Table("@poolIds", poolIds);
        GetAgentQueuesResult agentQueuesByPoolIds = new GetAgentQueuesResult();
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          if (queueType == TaskAgentQueueType.Automation)
          {
            resultCollection.AddBinder<TaskAgentQueue>((ObjectBinder<TaskAgentQueue>) new TaskAgentQueueBinder3((TaskResourceComponent) this));
            agentQueuesByPoolIds.Queues.AddRange<TaskAgentQueue, IList<TaskAgentQueue>>((IEnumerable<TaskAgentQueue>) resultCollection.GetCurrent<TaskAgentQueue>().Items);
          }
          else
          {
            resultCollection.AddBinder<DeploymentGroup>((ObjectBinder<DeploymentGroup>) new DeploymentGroupBinder((TaskResourceComponent) this));
            agentQueuesByPoolIds.MachineGroups.AddRange<DeploymentGroup, IList<DeploymentGroup>>((IEnumerable<DeploymentGroup>) resultCollection.GetCurrent<DeploymentGroup>().Items);
          }
          return agentQueuesByPoolIds;
        }
      }
    }

    public override IEnumerable<DeploymentPoolSummary> GetDeploymentPoolsSummary(string poolName = null)
    {
      List<DeploymentPoolSummary> deploymentPoolsSummary = new List<DeploymentPoolSummary>();
      using (new TaskSqlComponentBase.SqlMethodScope((TaskSqlComponentBase) this, nameof (GetDeploymentPoolsSummary)))
      {
        this.PrepareStoredProcedure("Task.prc_GetDeploymentPoolsMetrics");
        this.BindString("@poolName", poolName, 128, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<DeploymentPoolSummary>((ObjectBinder<DeploymentPoolSummary>) new DeploymentPoolSummaryBinder(this.RequestContext));
          deploymentPoolsSummary.AddRange((IEnumerable<DeploymentPoolSummary>) resultCollection.GetCurrent<DeploymentPoolSummary>().Items);
        }
      }
      return (IEnumerable<DeploymentPoolSummary>) deploymentPoolsSummary;
    }
  }
}
