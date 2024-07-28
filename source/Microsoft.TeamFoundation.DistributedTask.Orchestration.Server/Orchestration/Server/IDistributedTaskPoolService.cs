// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.IDistributedTaskPoolService
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.DistributedTask.Orchestration.Server
{
  [DefaultServiceImplementation(typeof (FrameworkPoolService))]
  public interface IDistributedTaskPoolService : IVssFrameworkService
  {
    TaskAgentPool AddAgentPool(IVssRequestContext requestContext, TaskAgentPool pool);

    TaskAgentQueue AddAgentQueue(
      IVssRequestContext requestContext,
      Guid projectId,
      TaskAgentQueue queue);

    TaskAgentQueue CheckUsePermissionForQueue(
      IVssRequestContext requestContext,
      Guid projectId,
      int queueId);

    DeploymentGroup CheckUsePermissionForDeploymentGroup(
      IVssRequestContext requestContext,
      Guid projectId,
      int machineGroupId);

    void DeleteAgentQueue(IVssRequestContext requestContext, Guid projectId, int queueId);

    Task DeleteAgentRequestAsync(
      IVssRequestContext requestContext,
      int poolId,
      long requestId,
      TaskResult? result = null,
      bool agentShuttingDown = false);

    TaskAgentPool GetAgentPool(
      IVssRequestContext requestContext,
      int poolId,
      IList<string> properties = null);

    TaskAgentPool GetAgentPool(
      IVssRequestContext requestContext,
      string poolName,
      IList<string> properties = null);

    IList<TaskAgentPool> GetAgentPools(
      IVssRequestContext requestContext,
      TaskAgentPoolType poolType = TaskAgentPoolType.Automation);

    IList<TaskAgentPool> GetAgentPoolsByIds(
      IVssRequestContext requestContext,
      IEnumerable<int> poolIds,
      TaskAgentPoolActionFilter actionFilter = TaskAgentPoolActionFilter.None);

    TaskAgentQueue GetAgentQueue(IVssRequestContext requestContext, Guid projectId, int queueId);

    TaskAgentQueue GetAgentQueue(
      IVssRequestContext requestContext,
      Guid projectId,
      string queueName);

    TaskAgentQueue GetAgentQueueForPool(
      IVssRequestContext requestContext,
      Guid projectId,
      int poolId);

    IList<TaskAgentQueue> GetAgentQueues(
      IVssRequestContext requestContext,
      Guid projectId,
      string queueName = null,
      TaskAgentQueueActionFilter? actionFilter = null);

    IList<TaskAgentQueue> GetAgentQueues(
      IVssRequestContext requestContext,
      Guid projectId,
      IEnumerable<int> ids,
      TaskAgentQueueActionFilter? actionFilter = null);

    IList<TaskAgentQueue> GetAgentQueues(
      IVssRequestContext requestContext,
      Guid projectId,
      IEnumerable<string> names,
      TaskAgentQueueActionFilter? actionFilter = null);

    IList<TaskAgentQueue> GetHostedAgentQueues(
      IVssRequestContext requestContext,
      Guid projectId,
      TaskAgentQueueActionFilter? actionFilter = null);

    TaskAgentJobRequest GetAgentRequest(
      IVssRequestContext requestContext,
      int poolId,
      long requestId);

    Task<TaskAgentJobRequest> GetAgentRequestAsync(
      IVssRequestContext requestContext,
      int poolId,
      long requestId);

    IList<TaskAgent> GetAgents(
      IVssRequestContext requestContext,
      int poolId,
      IList<Demand> demands = null);

    Task<IList<TaskAgent>> GetAgentsAsync(
      IVssRequestContext requestContext,
      int poolId,
      IList<Demand> demands = null);

    Task<TaskAgent> GetAgentAsync(IVssRequestContext requestContext, int poolId, int agentId);

    Task<IList<DeploymentMachine>> GetDeploymentMachinesAsync(
      IVssRequestContext requestContext,
      Guid projectId,
      int machineGroupId,
      IList<string> tagFilters = null);

    Task<IPagedList<DeploymentMachine>> GetDeploymentMachinesAsync(
      IVssRequestContext requestContext,
      Guid projectId,
      int deploymentGroupId,
      IList<string> tagFilters = null,
      string continuationToken = null,
      DeploymentTargetExpands expands = DeploymentTargetExpands.None,
      bool? enabled = null,
      IList<string> propertyFilters = null);

    DeploymentGroup GetDeploymentGroup(
      IVssRequestContext requestContext,
      Guid projectId,
      int machineGroupId,
      bool includeMachines = false);

    IList<DeploymentGroup> GetDeploymentGroups(
      IVssRequestContext requestContext,
      Guid projectId,
      string deploymentGroupName = null);

    IList<DeploymentGroup> GetDeploymentGroupsByIds(
      IVssRequestContext requestContext,
      Guid projectId,
      IList<int> deploymentGroupIds);

    IList<DeploymentPoolSummary> GetDeploymentPoolsSummary(
      IVssRequestContext requestContext,
      string poolName = null,
      bool includeDeploymentGroupReferences = false,
      IList<int> poolIds = null);

    Task<IList<TaskAgentJobRequest>> GetAgentRequestsForAgentsAsync(
      IVssRequestContext requestContext,
      int poolId,
      IList<int> agentIds,
      int completedRequests);

    IList<TaskDefinition> GetTaskDefinitions(
      IVssRequestContext requestContext,
      Guid? taskId = null,
      TaskVersion version = null,
      IList<string> visibility = null,
      bool scopeLocal = false);

    IList<TaskGroup> GetTaskGroups(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid? taskGroupId = null,
      bool? expanded = null);

    TaskGroup GetTaskGroup(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid taskGroupId,
      string versionSpec,
      bool? expanded = null);

    Task<TaskAgentJobRequest> QueueAgentRequestByPoolAsync(
      IVssRequestContext requestContext,
      int poolId,
      TaskAgentJobRequest request);

    Task SendAgentMessageAsync(
      IVssRequestContext requestContext,
      int poolId,
      long requestId,
      TaskAgentMessage message);

    TaskAgent UpdateAgent(IVssRequestContext requestContext, int poolId, TaskAgent agent);

    TaskAgentJobRequest UpdateAgentRequest(
      IVssRequestContext requestContext,
      int poolId,
      long requestId,
      TaskAgentJobRequest request);

    void CreateTeamProject(IVssRequestContext requestContext, Guid projectId);

    void DeleteTeamProject(IVssRequestContext requestContext, Guid projectId);
  }
}
