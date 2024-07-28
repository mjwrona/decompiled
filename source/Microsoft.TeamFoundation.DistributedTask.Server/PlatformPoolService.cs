// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.PlatformPoolService
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.DistributedTask.Server
{
  internal class PlatformPoolService : IDistributedTaskPoolService, IVssFrameworkService
  {
    private const string c_layer = "PlatformPoolService";

    public TaskAgentPool AddAgentPool(IVssRequestContext requestContext, TaskAgentPool pool)
    {
      using (new MethodScope(requestContext, nameof (PlatformPoolService), nameof (AddAgentPool)))
        return requestContext.GetService<DistributedTaskResourceService>().AddAgentPool(requestContext, pool, (Stream) null);
    }

    public TaskAgentQueue AddAgentQueue(
      IVssRequestContext requestContext,
      Guid projectId,
      TaskAgentQueue queue)
    {
      using (new MethodScope(requestContext, nameof (PlatformPoolService), nameof (AddAgentQueue)))
        return requestContext.GetService<DistributedTaskResourceService>().AddAgentQueue(requestContext, projectId, queue, false);
    }

    public TaskAgentQueue CheckUsePermissionForQueue(
      IVssRequestContext requestContext,
      Guid projectId,
      int queueId)
    {
      using (new MethodScope(requestContext, nameof (PlatformPoolService), nameof (CheckUsePermissionForQueue)))
        return requestContext.GetService<DistributedTaskResourceService>().GetAgentQueue(requestContext, projectId, queueId, TaskAgentQueueActionFilter.Use) ?? throw new TaskAgentQueueNotFoundException(TaskResources.QueueNotFound((object) queueId));
    }

    public DeploymentGroup CheckUsePermissionForDeploymentGroup(
      IVssRequestContext requestContext,
      Guid projectId,
      int machineGroupId)
    {
      using (new MethodScope(requestContext, nameof (PlatformPoolService), nameof (CheckUsePermissionForDeploymentGroup)))
        return requestContext.GetService<DeploymentGroupService>().GetDeploymentGroup(requestContext, projectId, machineGroupId, DeploymentGroupActionFilter.Use, false, false) ?? throw new DeploymentMachineGroupNotFoundException(TaskResources.DeploymentMachineGroupNotFound((object) machineGroupId));
    }

    public void CreateTeamProject(IVssRequestContext requestContext, Guid projectId)
    {
      using (new MethodScope(requestContext, nameof (PlatformPoolService), nameof (CreateTeamProject)))
        requestContext.GetService<DistributedTaskResourceService>().CreateTeamProject(requestContext, projectId);
    }

    public void DeleteTeamProject(IVssRequestContext requestContext, Guid projectId) => requestContext.GetService<DistributedTaskResourceService>().DeleteTeamProject(requestContext, projectId);

    public void DeleteAgentQueue(IVssRequestContext requestContext, Guid projectId, int queueId)
    {
      using (new MethodScope(requestContext, nameof (PlatformPoolService), nameof (DeleteAgentQueue)))
        requestContext.GetService<DistributedTaskResourceService>().DeleteAgentQueue(requestContext, projectId, queueId);
    }

    public void DeleteAgentRequest(
      IVssRequestContext requestContext,
      int poolId,
      long requestId,
      TaskResult? result = null,
      bool agentShuttingDown = false)
    {
      using (new MethodScope(requestContext, nameof (PlatformPoolService), nameof (DeleteAgentRequest)))
        requestContext.GetService<DistributedTaskResourceService>().FinishAgentRequest(requestContext, poolId, requestId, result, agentShuttingDown);
    }

    public Task DeleteAgentRequestAsync(
      IVssRequestContext requestContext,
      int poolId,
      long requestId,
      TaskResult? result = null,
      bool agentShuttingDown = false)
    {
      using (new MethodScope(requestContext, nameof (PlatformPoolService), nameof (DeleteAgentRequestAsync)))
        return requestContext.GetService<DistributedTaskResourceService>().FinishAgentRequestAsync(requestContext, poolId, requestId, result, agentShuttingDown, false);
    }

    public TaskAgentPool GetAgentPool(
      IVssRequestContext requestContext,
      int poolId,
      IList<string> properties = null)
    {
      using (new MethodScope(requestContext, nameof (PlatformPoolService), nameof (GetAgentPool)))
        return requestContext.GetService<DistributedTaskResourceService>().GetAgentPool(requestContext, poolId, properties, TaskAgentPoolActionFilter.None);
    }

    public TaskAgentPool GetAgentPool(
      IVssRequestContext requestContext,
      string poolName,
      IList<string> properties = null)
    {
      using (new MethodScope(requestContext, nameof (PlatformPoolService), nameof (GetAgentPool)))
        return requestContext.GetService<DistributedTaskResourceService>().GetAgentPools(requestContext, poolName, properties, TaskAgentPoolType.Automation, TaskAgentPoolActionFilter.None).FirstOrDefault<TaskAgentPool>();
    }

    public IList<TaskAgentPool> GetAgentPools(
      IVssRequestContext requestContext,
      TaskAgentPoolType poolType = TaskAgentPoolType.Automation)
    {
      using (new MethodScope(requestContext, nameof (PlatformPoolService), nameof (GetAgentPools)))
        return (IList<TaskAgentPool>) requestContext.GetService<DistributedTaskResourceService>().GetAgentPools(requestContext, (string) null, (IList<string>) null, poolType, TaskAgentPoolActionFilter.None);
    }

    public IList<TaskAgentPool> GetAgentPoolsByIds(
      IVssRequestContext requestContext,
      IEnumerable<int> poolIds,
      TaskAgentPoolActionFilter actionFilter = TaskAgentPoolActionFilter.None)
    {
      using (new MethodScope(requestContext, nameof (PlatformPoolService), nameof (GetAgentPoolsByIds)))
        return (IList<TaskAgentPool>) requestContext.GetService<DistributedTaskResourceService>().GetAgentPoolsByIdsAsync(requestContext, (IList<int>) poolIds.ToList<int>(), (IList<string>) null, actionFilter).SyncResult<List<TaskAgentPool>>();
    }

    public TaskAgentQueue GetAgentQueue(
      IVssRequestContext requestContext,
      Guid projectId,
      int queueId)
    {
      using (new MethodScope(requestContext, nameof (PlatformPoolService), nameof (GetAgentQueue)))
        return requestContext.GetService<DistributedTaskResourceService>().GetAgentQueue(requestContext, projectId, queueId, TaskAgentQueueActionFilter.None);
    }

    public TaskAgentQueue GetAgentQueue(
      IVssRequestContext requestContext,
      Guid projectId,
      string queueName)
    {
      using (new MethodScope(requestContext, nameof (PlatformPoolService), nameof (GetAgentQueue)))
        return requestContext.GetService<DistributedTaskResourceService>().GetAgentQueues(requestContext, projectId, queueName, TaskAgentQueueActionFilter.None).SingleOrDefault<TaskAgentQueue>();
    }

    public TaskAgentQueue GetAgentQueueForPool(
      IVssRequestContext requestContext,
      Guid projectId,
      int poolId)
    {
      using (new MethodScope(requestContext, nameof (PlatformPoolService), nameof (GetAgentQueueForPool)))
        return requestContext.GetService<DistributedTaskResourceService>().GetAgentQueuesForPools(requestContext, projectId, (IEnumerable<int>) new int[1]
        {
          poolId
        }, TaskAgentQueueActionFilter.None).FirstOrDefault<TaskAgentQueue>();
    }

    public IList<TaskAgentQueue> GetAgentQueues(
      IVssRequestContext requestContext,
      Guid projectId,
      string queueName = null,
      TaskAgentQueueActionFilter? actionFilter = null)
    {
      using (new MethodScope(requestContext, nameof (PlatformPoolService), nameof (GetAgentQueues)))
        return requestContext.GetService<DistributedTaskResourceService>().GetAgentQueues(requestContext, projectId, queueName, actionFilter.GetValueOrDefault());
    }

    public IList<TaskAgentQueue> GetAgentQueues(
      IVssRequestContext requestContext,
      Guid projectId,
      IEnumerable<int> ids,
      TaskAgentQueueActionFilter? actionFilter = null)
    {
      using (new MethodScope(requestContext, nameof (PlatformPoolService), nameof (GetAgentQueues)))
        return requestContext.GetService<DistributedTaskResourceService>().GetAgentQueues(requestContext, projectId, ids, actionFilter.GetValueOrDefault());
    }

    public IList<TaskAgentQueue> GetAgentQueues(
      IVssRequestContext requestContext,
      Guid projectId,
      IEnumerable<string> names,
      TaskAgentQueueActionFilter? actionFilter = null)
    {
      using (new MethodScope(requestContext, nameof (PlatformPoolService), nameof (GetAgentQueues)))
        return requestContext.GetService<DistributedTaskResourceService>().GetAgentQueues(requestContext, projectId, names, actionFilter.GetValueOrDefault());
    }

    public IList<TaskAgentQueue> GetHostedAgentQueues(
      IVssRequestContext requestContext,
      Guid projectId,
      TaskAgentQueueActionFilter? actionFilter = null)
    {
      using (new MethodScope(requestContext, nameof (PlatformPoolService), nameof (GetHostedAgentQueues)))
        return requestContext.GetService<DistributedTaskResourceService>().GetHostedAgentQueues(requestContext, projectId, actionFilter.GetValueOrDefault());
    }

    public TaskAgentJobRequest GetAgentRequest(
      IVssRequestContext requestContext,
      int poolId,
      long requestId)
    {
      using (new MethodScope(requestContext, nameof (PlatformPoolService), nameof (GetAgentRequest)))
        return requestContext.GetService<DistributedTaskResourceService>().GetAgentRequest(requestContext, poolId, requestId, false);
    }

    public async Task<TaskAgentJobRequest> GetAgentRequestAsync(
      IVssRequestContext requestContext,
      int poolId,
      long requestId)
    {
      TaskAgentJobRequest agentRequestAsync;
      using (new MethodScope(requestContext, nameof (PlatformPoolService), nameof (GetAgentRequestAsync)))
        agentRequestAsync = await requestContext.GetService<DistributedTaskResourceService>().GetAgentRequestAsync(requestContext, poolId, requestId, false);
      return agentRequestAsync;
    }

    public IList<TaskGroup> GetTaskGroups(
      IVssRequestContext requestContext,
      Guid project,
      Guid? taskGroupId = null,
      bool? expanded = null)
    {
      using (new MethodScope(requestContext, nameof (PlatformPoolService), nameof (GetTaskGroups)))
        return (IList<TaskGroup>) requestContext.GetService<IMetaTaskService>().GetTaskGroups(requestContext, project, taskGroupId, expanded);
    }

    public TaskGroup GetTaskGroup(
      IVssRequestContext requestContext,
      Guid project,
      Guid taskGroupId,
      string versionSpec,
      bool? expanded = null)
    {
      using (new MethodScope(requestContext, nameof (PlatformPoolService), nameof (GetTaskGroup)))
        return requestContext.GetService<IMetaTaskService>().GetTaskGroup(requestContext, project, taskGroupId, versionSpec, expanded);
    }

    public IList<TaskDefinition> GetTaskDefinitions(
      IVssRequestContext requestContext,
      Guid? taskId = null,
      TaskVersion version = null,
      IList<string> visibility = null,
      bool scopeLocal = false)
    {
      using (new MethodScope(requestContext, nameof (PlatformPoolService), nameof (GetTaskDefinitions)))
        return requestContext.GetService<DistributedTaskService>().GetTaskDefinitions(requestContext, taskId, version, (IEnumerable<string>) visibility, scopeLocal, false);
    }

    public IList<TaskAgent> GetAgents(
      IVssRequestContext requestContext,
      int poolId,
      IList<Demand> demands = null)
    {
      using (new MethodScope(requestContext, nameof (PlatformPoolService), nameof (GetAgents)))
      {
        DistributedTaskResourceService service = requestContext.GetService<DistributedTaskResourceService>();
        return demands == null || demands.Count == 0 ? service.GetAgents(requestContext, poolId, (string) null, false, false, false, false, (IList<string>) null) : service.GetAgents(requestContext, poolId, demands, (IList<string>) null).MatchedAgents;
      }
    }

    public async Task<IList<TaskAgent>> GetAgentsAsync(
      IVssRequestContext requestContext,
      int poolId,
      IList<Demand> demands = null)
    {
      using (new MethodScope(requestContext, nameof (PlatformPoolService), nameof (GetAgentsAsync)))
      {
        DistributedTaskResourceService service = requestContext.GetService<DistributedTaskResourceService>();
        return demands == null || demands.Count == 0 ? await service.GetAgentsAsync(requestContext, poolId) : (await service.GetAgentsAsync(requestContext, poolId, demands)).MatchedAgents;
      }
    }

    public async Task<TaskAgent> GetAgentAsync(
      IVssRequestContext requestContext,
      int poolId,
      int agentId)
    {
      TaskAgent agentAsync;
      using (new MethodScope(requestContext, nameof (PlatformPoolService), nameof (GetAgentAsync)))
        agentAsync = await requestContext.GetService<DistributedTaskResourceService>().GetAgentAsync(requestContext, poolId, agentId, false, false, false, (IList<string>) null);
      return agentAsync;
    }

    public Task<IList<DeploymentMachine>> GetDeploymentMachinesAsync(
      IVssRequestContext requestContext,
      Guid projectId,
      int machineGroupId,
      IList<string> tagFilters = null)
    {
      using (new MethodScope(requestContext, nameof (PlatformPoolService), nameof (GetDeploymentMachinesAsync)))
        return requestContext.GetService<DeploymentGroupService>().GetDeploymentMachinesAsync(requestContext, projectId, machineGroupId, tagFilters, (string) null, false, false);
    }

    public Task<IPagedList<DeploymentMachine>> GetDeploymentMachinesAsync(
      IVssRequestContext requestContext,
      Guid projectId,
      int deploymentGroupId,
      IList<string> tagFilters = null,
      string continuationToken = null,
      DeploymentTargetExpands expands = DeploymentTargetExpands.None,
      bool? enabled = null,
      IList<string> propertyFilters = null)
    {
      using (new MethodScope(requestContext, nameof (PlatformPoolService), nameof (GetDeploymentMachinesAsync)))
      {
        bool includeCapabilities = (expands & DeploymentTargetExpands.Capabilities) == DeploymentTargetExpands.Capabilities;
        bool includeAssignedRequest = (expands & DeploymentTargetExpands.AssignedRequest) == DeploymentTargetExpands.AssignedRequest;
        bool includeLastCompletedRequest = (expands & DeploymentTargetExpands.LastCompletedRequest) == DeploymentTargetExpands.LastCompletedRequest;
        return requestContext.GetService<DeploymentGroupService>().GetDeploymentTargetsPagedAsync(requestContext, projectId, deploymentGroupId, tagFilters, (string) null, false, includeCapabilities, includeAssignedRequest, includeLastCompletedRequest, TaskAgentStatusFilter.All, TaskAgentJobResultFilter.All, continuationToken, 1000, enabled, propertyFilters);
      }
    }

    public DeploymentGroup GetDeploymentGroup(
      IVssRequestContext requestContext,
      Guid projectId,
      int machineGroupId,
      bool includeMachines = false)
    {
      using (new MethodScope(requestContext, nameof (PlatformPoolService), nameof (GetDeploymentGroup)))
        return requestContext.GetService<DeploymentGroupService>().GetDeploymentGroup(requestContext, projectId, machineGroupId, DeploymentGroupActionFilter.None, includeMachines, false);
    }

    public IList<DeploymentGroup> GetDeploymentGroups(
      IVssRequestContext requestContext,
      Guid projectId,
      string deploymentGroupName = null)
    {
      using (new MethodScope(requestContext, nameof (PlatformPoolService), nameof (GetDeploymentGroups)))
        return requestContext.GetService<DeploymentGroupService>().GetDeploymentGroups(requestContext, projectId, deploymentGroupName, DeploymentGroupActionFilter.None, false);
    }

    public IList<DeploymentGroup> GetDeploymentGroupsByIds(
      IVssRequestContext requestContext,
      Guid projectId,
      IList<int> deploymentGroupIds)
    {
      using (new MethodScope(requestContext, nameof (PlatformPoolService), nameof (GetDeploymentGroupsByIds)))
        return requestContext.GetService<DeploymentGroupService>().GetDeploymentGroupsByIds(requestContext, projectId, deploymentGroupIds, DeploymentGroupActionFilter.None, false);
    }

    public IList<DeploymentPoolSummary> GetDeploymentPoolsSummary(
      IVssRequestContext requestContext,
      string poolName = null,
      bool includeDeploymentGroupReferences = false,
      IList<int> poolIds = null)
    {
      using (new MethodScope(requestContext, nameof (PlatformPoolService), nameof (GetDeploymentPoolsSummary)))
        return requestContext.GetService<DistributedTaskResourceService>().GetDeploymentPoolsSummary(requestContext, poolName, poolIds, includeDeploymentGroupReferences, false);
    }

    public Task<TaskAgentJobRequest> QueueAgentRequestByPoolAsync(
      IVssRequestContext requestContext,
      int poolId,
      TaskAgentJobRequest request)
    {
      using (new MethodScope(requestContext, nameof (PlatformPoolService), nameof (QueueAgentRequestByPoolAsync)))
      {
        KPIHelper.PublishDTAgentRequestSent(requestContext);
        return requestContext.GetService<DistributedTaskResourceService>().QueueAgentRequestByPoolAsync(requestContext, poolId, request);
      }
    }

    public IList<TaskAgentJobRequest> GetAgentRequestsForAgents(
      IVssRequestContext requestContext,
      int poolId,
      IList<int> agentIds,
      int completedRequests)
    {
      using (new MethodScope(requestContext, nameof (PlatformPoolService), nameof (GetAgentRequestsForAgents)))
        return requestContext.GetService<DistributedTaskResourceService>().GetAgentRequestsForAgents(requestContext, poolId, agentIds, completedRequests);
    }

    public async Task<IList<TaskAgentJobRequest>> GetAgentRequestsForAgentsAsync(
      IVssRequestContext requestContext,
      int poolId,
      IList<int> agentIds,
      int completedRequests)
    {
      IList<TaskAgentJobRequest> requestsForAgentsAsync;
      using (new MethodScope(requestContext, nameof (PlatformPoolService), nameof (GetAgentRequestsForAgentsAsync)))
        requestsForAgentsAsync = await requestContext.GetService<DistributedTaskResourceService>().GetAgentRequestsForAgentsAsync(requestContext, poolId, agentIds, completedRequests);
      return requestsForAgentsAsync;
    }

    public void SendAgentMessage(
      IVssRequestContext requestContext,
      int poolId,
      long requestId,
      TaskAgentMessage message)
    {
      using (new MethodScope(requestContext, nameof (PlatformPoolService), nameof (SendAgentMessage)))
        requestContext.GetService<DistributedTaskResourceService>().SendJobMessageToAgent(requestContext, poolId, requestId, message);
    }

    public async Task SendAgentMessageAsync(
      IVssRequestContext requestContext,
      int poolId,
      long requestId,
      TaskAgentMessage message)
    {
      MethodScope methodScope = new MethodScope(requestContext, nameof (PlatformPoolService), nameof (SendAgentMessageAsync));
      try
      {
        await requestContext.GetService<DistributedTaskResourceService>().SendJobMessageToAgentAsync(requestContext, poolId, requestId, message);
      }
      finally
      {
        methodScope.Dispose();
      }
      methodScope = new MethodScope();
    }

    public TaskAgent UpdateAgent(IVssRequestContext requestContext, int poolId, TaskAgent agent)
    {
      using (new MethodScope(requestContext, nameof (PlatformPoolService), nameof (UpdateAgent)))
        return requestContext.GetService<DistributedTaskResourceService>().UpdateAgent(requestContext, poolId, agent, TaskAgentCapabilityType.System | TaskAgentCapabilityType.User);
    }

    public TaskAgentJobRequest UpdateAgentRequest(
      IVssRequestContext requestContext,
      int poolId,
      long requestId,
      TaskAgentJobRequest request)
    {
      using (new MethodScope(requestContext, nameof (PlatformPoolService), nameof (UpdateAgentRequest)))
        return requestContext.GetService<DistributedTaskResourceService>().UpdateAgentRequest(requestContext, poolId, requestId, request.LockedUntil, request.ReceiveTime, request.FinishTime, request.Result);
    }

    void IVssFrameworkService.ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    void IVssFrameworkService.ServiceStart(IVssRequestContext systemRequestContext)
    {
    }
  }
}
