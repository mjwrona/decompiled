// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.IDistributedTaskResourceService
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.Azure.Pipelines.PoolProvider.Contracts;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.DistributedTask.Server
{
  [DefaultServiceImplementation(typeof (DistributedTaskResourceService))]
  public interface IDistributedTaskResourceService : IVssFrameworkService
  {
    Task<AbandonedAgentRequestsResult> AbandonExpiredAgentRequestsAsync(
      IVssRequestContext requestContext,
      int poolId);

    TaskAgent AddAgent(IVssRequestContext requestContext, int poolId, TaskAgent agent);

    TaskAgentPool AddAgentPool(
      IVssRequestContext requestContext,
      TaskAgentPool pool,
      Stream poolMetadata = null);

    TaskAgentQueue AddAgentQueue(
      IVssRequestContext requestContext,
      Guid projectId,
      TaskAgentQueue queue,
      bool authorizePipelines = false);

    Task<AgentRequestsAssignmentResult> AssignAgentRequestsAsync(IVssRequestContext requestContext);

    Task<IList<TaskAgentJobRequest>> CancelAgentRequestsForPoolAsync(
      IVssRequestContext requestContext,
      int poolId,
      string reason);

    void ClearAgentInformation(IVssRequestContext requestContext, int poolId, int agentId);

    TaskAgentSession CreateSession(
      IVssRequestContext requestContext,
      int poolId,
      TaskAgentSession session);

    int DeleteAgents(IVssRequestContext requestContext, int poolId, IEnumerable<int> agentIds);

    void DeleteAgentPool(IVssRequestContext requestContext, int poolId);

    void DeleteAgentQueue(IVssRequestContext requestContext, Guid projectId, int queueId);

    Task DeleteMessageAsync(
      IVssRequestContext requestContext,
      int poolId,
      Guid sessionId,
      long messageId);

    void DeleteSession(IVssRequestContext requestContext, int poolId, Guid sessionId);

    Task FinishAgentRequestAsync(
      IVssRequestContext requestContext,
      int poolId,
      long requestId,
      TaskResult? jobResult = null,
      bool agentShuttingDown = false,
      bool callSprocEvenIfPoolNotFound = false);

    TaskAgent GetAgent(
      IVssRequestContext requestContext,
      int poolId,
      int agentId,
      bool includeCapabilities = false,
      bool includeAssignedRequest = false,
      bool includeLastCompletedRequest = false,
      IList<string> propertyFilters = null);

    Task<TaskAgent> GetAgentAsync(
      IVssRequestContext requestContext,
      int poolId,
      int agentId,
      bool includeCapabilities = false,
      bool includeAssignedRequest = false,
      bool includeLastCompletedRequest = false,
      IList<string> propertyFilters = null);

    TaskAgentPool GetAgentPool(
      IVssRequestContext requestContext,
      int poolId,
      IList<string> propertyFilters = null,
      TaskAgentPoolActionFilter actionFilter = TaskAgentPoolActionFilter.None);

    Task<TaskAgentPool> GetAgentPoolAsync(
      IVssRequestContext requestContext,
      int poolId,
      IList<string> propertyFilters = null,
      TaskAgentPoolActionFilter actionFilter = TaskAgentPoolActionFilter.None);

    List<TaskAgentPool> GetAgentPools(
      IVssRequestContext requestContext,
      string poolName = null,
      IList<string> propertyFilters = null,
      TaskAgentPoolType poolType = TaskAgentPoolType.Automation,
      TaskAgentPoolActionFilter actionFilter = TaskAgentPoolActionFilter.None);

    Task<List<TaskAgentPool>> GetAgentPoolsAsync(
      IVssRequestContext requestContext,
      string poolName = null,
      IList<string> propertyFilters = null,
      TaskAgentPoolType poolType = TaskAgentPoolType.Automation,
      TaskAgentPoolActionFilter actionFilter = TaskAgentPoolActionFilter.None);

    Task<List<TaskAgentPool>> GetAgentPoolsByIdsAsync(
      IVssRequestContext requestContext,
      IList<int> poolIds,
      IList<string> propertyFilters = null,
      TaskAgentPoolActionFilter actionFilter = TaskAgentPoolActionFilter.None);

    Task<Stream> GetAgentPoolMetadataAsync(IVssRequestContext requestContext, int poolId);

    IList<TaskAgentPoolStatus> GetAgentPoolStatusByIds(
      IVssRequestContext requestContext,
      IList<int> poolIds);

    TaskAgentQueue GetAgentQueue(
      IVssRequestContext requestContext,
      Guid projectId,
      int queueId,
      TaskAgentQueueActionFilter actionFilter = TaskAgentQueueActionFilter.None);

    Task<TaskAgentQueue> GetAgentQueueAsync(
      IVssRequestContext requestContext,
      Guid projectId,
      int queueId,
      TaskAgentQueueActionFilter actionFilter = TaskAgentQueueActionFilter.None);

    IList<TaskAgentQueue> GetAgentQueues(
      IVssRequestContext requestContext,
      Guid projectId,
      string queueName = null,
      TaskAgentQueueActionFilter actionFilter = TaskAgentQueueActionFilter.None);

    IList<TaskAgentQueue> GetAgentQueues(
      IVssRequestContext requestContext,
      Guid projectId,
      IEnumerable<int> ids,
      TaskAgentQueueActionFilter actionFilter = TaskAgentQueueActionFilter.None);

    IList<TaskAgentQueue> GetAgentQueues(
      IVssRequestContext requestContext,
      Guid projectId,
      IEnumerable<string> names,
      TaskAgentQueueActionFilter actionFilter = TaskAgentQueueActionFilter.None);

    IList<TaskAgentQueue> GetAgentQueuesForPools(
      IVssRequestContext requestContext,
      Guid projectId,
      IEnumerable<int> poolIds,
      TaskAgentQueueActionFilter actionFilter = TaskAgentQueueActionFilter.None);

    TaskAgentJobRequest GetAgentRequest(
      IVssRequestContext requestContext,
      int poolId,
      long requestId,
      bool includeStatus = false);

    Task<TaskAgentJobRequest> GetAgentRequestAsync(
      IVssRequestContext requestContext,
      int poolId,
      long requestId,
      bool includeStatus = false);

    IPagedList<TaskAgentJobRequest> GetAgentRequests(
      IVssRequestContext requestContext,
      int poolId,
      string continuationToken = null,
      int? maxRequestCount = null);

    Task<IPagedList<TaskAgentJobRequest>> GetAgentRequestsAsync(
      IVssRequestContext requestContext,
      int poolId,
      string continuationToken = null,
      int? maxRequestCount = null);

    Task<IPagedList<TaskAgentJobRequest>> GetAgentRequestsForQueueAsync(
      IVssRequestContext requestContext,
      Guid projectId,
      int queueId,
      string continuationToken = null,
      int? maxRequestCount = null);

    IList<TaskAgentJobRequest> GetAgentRequestsForPlan(
      IVssRequestContext requestContext,
      int poolId,
      Guid planId,
      Guid? jobId = null);

    IList<TaskAgentJobRequest> GetAgentRequestsForPlan(
      IVssRequestContext requestContext,
      Guid planId);

    IList<TaskAgentJobRequest> GetAgentRequestsForAgent(
      IVssRequestContext requestContext,
      int poolId,
      int agentId,
      int completedRequestCount = 50);

    IList<TaskAgentJobRequest> GetAgentRequestsForAgents(
      IVssRequestContext requestContext,
      int poolId,
      IList<int> agentIds,
      int completedRequestCount = 0);

    IList<TaskAgentJobRequest> GetAgentRequestsForAgents(
      IVssRequestContext requestContext,
      int poolId,
      IList<int> agentIds,
      Guid hostId,
      Guid projectId,
      int completedRequestCount = 50,
      int? ownerId = null,
      DateTime? completedOn = null);

    IList<TaskAgent> GetAgents(
      IVssRequestContext requestContext,
      int poolId,
      string agentName = null,
      bool includeCapabilities = false,
      bool includeAssignedRequest = false,
      bool includeAgentCloudRequest = false,
      bool includeLastCompletedRequest = false,
      IList<string> propertyFilters = null);

    IList<TaskAgent> GetAgents(
      IVssRequestContext requestContext,
      int poolId,
      IEnumerable<int> agentIds,
      bool includeCapabilities = false,
      bool includeAssignedRequest = false,
      bool includeLastCompletedRequest = false,
      IList<string> propertyFilters = null);

    TaskAgentQueryResult GetAgents(
      IVssRequestContext requestContext,
      int poolId,
      IList<Demand> demands,
      IList<string> propertyFilters = null);

    IList<TaskAgentQueryResult> GetAgents(
      IVssRequestContext requestContext,
      int poolId,
      IList<Demand>[] demandSets,
      IList<string> propertyFilters = null);

    Task<TaskAgentMessage> GetMessageAsync(
      IVssRequestContext requestContext,
      int poolId,
      Guid sessionId,
      TimeSpan timeout,
      long? lastMessageId = null);

    TaskPackageMetadata GetPackage(
      IVssRequestContext requestContext,
      string packageType,
      bool includeUrl = true,
      string version = null);

    IList<TaskPackageMetadata> GetPackages(IVssRequestContext requestContext, string packageType = null);

    void WritePackageFile(IVssRequestContext requestContext, string package, Stream stream);

    TaskAgentPoolMaintenanceDefinition CreateAgentPoolMaintenanceDefinition(
      IVssRequestContext requestContext,
      int poolId,
      TaskAgentPoolMaintenanceDefinition definition);

    IList<TaskAgentPoolMaintenanceDefinition> GetAgentPoolMaintenanceDefinitions(
      IVssRequestContext requestContext,
      int poolId);

    TaskAgentPoolMaintenanceDefinition GetAgentPoolMaintenanceDefinition(
      IVssRequestContext requestContext,
      int poolId,
      int definitionId);

    ResourceUsage GetResourceUsage(
      IVssRequestContext poolRequestContext,
      string parallelismTag,
      bool poolIsHosted,
      bool includeUsedCount,
      bool includeRunningRequests,
      int maxRequestsCount);

    TaskAgentPoolMaintenanceDefinition UpdateAgentPoolMaintenanceDefinition(
      IVssRequestContext requestContext,
      int poolId,
      int definitionId,
      TaskAgentPoolMaintenanceDefinition definition);

    void DeleteAgentPoolMaintenanceDefinition(
      IVssRequestContext requestContext,
      int poolId,
      int definitionId);

    TaskAgentPoolMaintenanceJob QueueAgentPoolMaintenanceJob(
      IVssRequestContext requestContext,
      int poolId,
      int definitionId);

    IList<TaskAgentPoolMaintenanceJob> GetAgentPoolMaintenanceJobs(
      IVssRequestContext requestContext,
      int poolId,
      int? defintionId = null);

    TaskAgentPoolMaintenanceJob GetAgentPoolMaintenanceJob(
      IVssRequestContext requestContext,
      int poolId,
      int jobId);

    void GetAgentPoolMaintenanceJobLogs(
      IVssRequestContext requestContext,
      int poolId,
      int jobId,
      Stream outputStream);

    IList<DeploymentPoolSummary> GetDeploymentPoolsSummary(
      IVssRequestContext requestContext,
      string poolName = null,
      IList<int> deploymentPoolIds = null,
      bool includeDeploymentGroupReferences = false,
      bool includeResource = false);

    TaskAgentPoolMaintenanceJob UpdateAgentPoolMaintenanceJob(
      IVssRequestContext requestContext,
      int poolId,
      int jobId,
      TaskAgentPoolMaintenanceJob maintenanceJob);

    TaskAgentPoolMaintenanceJob UpdateAgentPoolMaintenanceJobTargetAgents(
      IVssRequestContext requestContext,
      int poolId,
      int jobId,
      List<TaskAgentPoolMaintenanceJobTargetAgent> targetAgents);

    void DeleteAgentPoolMaintenanceJobs(
      IVssRequestContext requestContext,
      int poolId,
      IEnumerable<int> jobIds);

    Task<TaskAgentJobRequest> QueueAgentRequestAsync(
      IVssRequestContext requestContext,
      Guid projectId,
      int queueId,
      TaskAgentJobRequest request);

    Task<TaskAgentJobRequest> QueueAgentRequestByPoolAsync(
      IVssRequestContext requestContext,
      int poolId,
      TaskAgentJobRequest request);

    void SendJobMessageToAgent(
      IVssRequestContext requestContext,
      int poolId,
      long requestId,
      TaskAgentMessage message);

    Task SendRefreshMessageToAgentAsync(IVssRequestContext requestContext, int poolId, int agentId);

    Task SendRefreshMessageToAgentsAsync(IVssRequestContext requestContext, int poolId);

    TaskAgent UpdateAgent(
      IVssRequestContext requestContext,
      int poolId,
      TaskAgent agent,
      TaskAgentCapabilityType capabilityUpdate = TaskAgentCapabilityType.System | TaskAgentCapabilityType.User);

    TaskAgentJobRequest UpdateAgentRequest(
      IVssRequestContext requestContext,
      int poolId,
      long requestId,
      DateTime? expirationTime = null,
      DateTime? startTime = null,
      DateTime? finishTime = null,
      TaskResult? result = null);

    Task<TaskAgentJobRequest> UpdateAgentRequestLockedUntilAsync(
      IVssRequestContext requestContext,
      int poolId,
      long requestId,
      DateTime? expirationTime = null,
      DateTime? startTime = null);

    Task<TaskAgentJobRequest> UpdateAgentRequestAsync(
      IVssRequestContext requestContext,
      int poolId,
      long requestId,
      DateTime? expirationTime = null,
      DateTime? startTime = null,
      DateTime? finishTime = null,
      TaskResult? result = null);

    Task<TaskAgentJobRequest> BumpAgentRequestPriorityAsync(
      IVssRequestContext requestContext,
      int poolId,
      long requestId);

    TaskAgentPool UpdateAgentPool(
      IVssRequestContext requestContext,
      int poolId,
      TaskAgentPool pool,
      bool removePoolMetadata = false,
      Stream poolMetadataStream = null);

    void CreateTeamProject(IVssRequestContext requestContext, Guid projectId);

    TaskAgent UpdateAgentUpdateState(
      IVssRequestContext requestContext,
      int poolId,
      int agentId,
      string currentState);

    void FinishAgentUpdate(
      IVssRequestContext requestContext,
      int poolId,
      int agentId,
      TaskResult result,
      string currentState,
      TaskAgentUpdate agentUpdate);

    Task<IList<AgentDefinition>> GetAgentPoolDefinitions(
      IVssRequestContext requestContext,
      int poolId);

    Task<IList<AgentDefinition>> GetAgentQueueDefinitions(
      IVssRequestContext requestContext,
      Guid projectId,
      int queueId);

    List<TaskAgentPool> GetActiveAgentPools(
      IVssRequestContext requestContext,
      DateTime activeSince,
      IList<string> propertyFilters = null,
      TaskAgentPoolActionFilter actionFilter = TaskAgentPoolActionFilter.None);

    void RemovePoisonedOrchestrations(
      IVssRequestContext requestContext,
      string hubName,
      IList<string> orchestrationIds,
      TimeSpan? timeout = null);
  }
}
