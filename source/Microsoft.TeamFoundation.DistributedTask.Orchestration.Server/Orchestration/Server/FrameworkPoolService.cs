// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.FrameworkPoolService
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.DataAccess;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.DistributedTask.Orchestration.Server
{
  internal class FrameworkPoolService : IDistributedTaskPoolService, IVssFrameworkService
  {
    private const string c_layer = "FrameworkPoolService";
    private const string c_queueCacheMissesItemKey = "MS.TF.DistributedTask.QueueCacheMisses";

    public TaskAgentPool AddAgentPool(IVssRequestContext requestContext, TaskAgentPool pool)
    {
      using (new MethodScope(requestContext, nameof (FrameworkPoolService), nameof (AddAgentPool)))
        return requestContext.GetClient<TaskAgentHttpClient>().AddAgentPoolAsync(pool).SyncResult<TaskAgentPool>();
    }

    public TaskAgentQueue AddAgentQueue(
      IVssRequestContext requestContext,
      Guid projectId,
      TaskAgentQueue queue)
    {
      using (new MethodScope(requestContext, nameof (FrameworkPoolService), nameof (AddAgentQueue)))
      {
        queue = requestContext.GetClient<TaskAgentHttpClient>().AddAgentQueueAsync(projectId, queue).SyncResult<TaskAgentQueue>();
        requestContext.GetService<TaskAgentQueueCacheService>().Set(projectId, queue.Id, queue);
        return queue;
      }
    }

    public TaskAgentQueue CheckUsePermissionForQueue(
      IVssRequestContext requestContext,
      Guid projectId,
      int queueId)
    {
      using (new MethodScope(requestContext, nameof (FrameworkPoolService), nameof (CheckUsePermissionForQueue)))
        return requestContext.GetClient<TaskAgentHttpClient>().GetAgentQueueAsync(projectId, queueId, new TaskAgentQueueActionFilter?(TaskAgentQueueActionFilter.Use)).SyncResult<TaskAgentQueue>();
    }

    public DeploymentGroup CheckUsePermissionForDeploymentGroup(
      IVssRequestContext requestContext,
      Guid projectId,
      int machineGroupId)
    {
      using (new MethodScope(requestContext, nameof (FrameworkPoolService), nameof (CheckUsePermissionForDeploymentGroup)))
        return requestContext.GetClient<TaskAgentHttpClient>().GetDeploymentGroupAsync(projectId, machineGroupId, new DeploymentGroupActionFilter?(DeploymentGroupActionFilter.Use)).SyncResult<DeploymentGroup>();
    }

    public void DeleteAgentQueue(IVssRequestContext requestContext, Guid projectId, int queueId)
    {
      using (new MethodScope(requestContext, nameof (FrameworkPoolService), nameof (DeleteAgentQueue)))
      {
        requestContext.GetClient<TaskAgentHttpClient>().DeleteAgentQueueAsync(projectId, queueId).SyncResult();
        requestContext.GetService<TaskAgentQueueCacheService>().Remove(projectId, queueId);
      }
    }

    public Task DeleteAgentRequestAsync(
      IVssRequestContext requestContext,
      int poolId,
      long requestId,
      TaskResult? result = null,
      bool agentShuttingDown = false)
    {
      using (new MethodScope(requestContext, nameof (FrameworkPoolService), nameof (DeleteAgentRequestAsync)))
      {
        TaskAgentHttpClient client = requestContext.GetClient<TaskAgentHttpClient>();
        int poolId1 = poolId;
        long requestId1 = requestId;
        Guid empty = Guid.Empty;
        TaskResult? result1 = result;
        object obj = (object) requestContext;
        bool? agentShuttingDown1 = new bool?(agentShuttingDown);
        object userState = obj;
        CancellationToken cancellationToken = new CancellationToken();
        return client.DeleteAgentRequestAsync(poolId1, requestId1, empty, result1, agentShuttingDown1, userState, cancellationToken);
      }
    }

    public TaskAgentPool GetAgentPool(
      IVssRequestContext requestContext,
      int poolId,
      IList<string> properties = null)
    {
      using (new MethodScope(requestContext, nameof (FrameworkPoolService), nameof (GetAgentPool)))
      {
        TaskAgentHttpClient client = requestContext.GetClient<TaskAgentHttpClient>();
        int poolId1 = poolId;
        IList<string> properties1 = properties;
        object obj = (object) requestContext;
        TaskAgentPoolActionFilter? actionFilter = new TaskAgentPoolActionFilter?();
        object userState = obj;
        CancellationToken cancellationToken = new CancellationToken();
        return client.GetAgentPoolAsync(poolId1, (IEnumerable<string>) properties1, actionFilter, userState, cancellationToken).SyncResult<TaskAgentPool>();
      }
    }

    public TaskAgentPool GetAgentPool(
      IVssRequestContext requestContext,
      string poolName,
      IList<string> properties = null)
    {
      using (new MethodScope(requestContext, nameof (FrameworkPoolService), nameof (GetAgentPool)))
      {
        TaskAgentHttpClient client = requestContext.GetClient<TaskAgentHttpClient>();
        string poolName1 = poolName;
        IList<string> properties1 = properties;
        object obj = (object) requestContext;
        TaskAgentPoolType? poolType = new TaskAgentPoolType?();
        TaskAgentPoolActionFilter? actionFilter = new TaskAgentPoolActionFilter?();
        object userState = obj;
        CancellationToken cancellationToken = new CancellationToken();
        return client.GetAgentPoolsAsync(poolName1, (IEnumerable<string>) properties1, poolType, actionFilter, userState, cancellationToken).SyncResult<List<TaskAgentPool>>().FirstOrDefault<TaskAgentPool>();
      }
    }

    public IList<TaskAgentPool> GetAgentPools(
      IVssRequestContext requestContext,
      TaskAgentPoolType poolType = TaskAgentPoolType.Automation)
    {
      using (new MethodScope(requestContext, nameof (FrameworkPoolService), nameof (GetAgentPools)))
        return (IList<TaskAgentPool>) requestContext.GetClient<TaskAgentHttpClient>().GetAgentPoolsAsync(poolType: new TaskAgentPoolType?(poolType)).SyncResult<List<TaskAgentPool>>();
    }

    public IList<TaskAgentPool> GetAgentPoolsByIds(
      IVssRequestContext requestContext,
      IEnumerable<int> poolIds,
      TaskAgentPoolActionFilter actionFilter = TaskAgentPoolActionFilter.None)
    {
      using (new MethodScope(requestContext, nameof (FrameworkPoolService), nameof (GetAgentPoolsByIds)))
        return (IList<TaskAgentPool>) requestContext.GetClient<TaskAgentHttpClient>().GetAgentPoolsByIdsAsync(poolIds, new TaskAgentPoolActionFilter?(actionFilter)).SyncResult<List<TaskAgentPool>>();
    }

    public TaskAgentQueue GetAgentQueue(
      IVssRequestContext requestContext,
      Guid projectId,
      int queueId)
    {
      using (new MethodScope(requestContext, nameof (FrameworkPoolService), nameof (GetAgentQueue)))
      {
        TaskAgentQueueCacheService service = requestContext.GetService<TaskAgentQueueCacheService>();
        TaskAgentQueue queue1;
        if (service.TryGetValue(projectId, queueId, out queue1))
          return queue1;
        string keyToken = service.GetKeyToken(projectId, queueId);
        HashSet<string> stringSet;
        if (requestContext.TryGetItem<HashSet<string>>("MS.TF.DistributedTask.QueueCacheMisses", out stringSet) && stringSet.Contains(keyToken))
          return (TaskAgentQueue) null;
        TaskAgentQueue queue2 = requestContext.GetClient<TaskAgentHttpClient>().GetAgentQueueAsync(projectId, queueId).SyncResult<TaskAgentQueue>();
        if (queue2 != null)
        {
          service.Set(projectId, queue2.Id, queue2);
        }
        else
        {
          if (stringSet == null && !requestContext.TryGetItem<HashSet<string>>("MS.TF.DistributedTask.QueueCacheMisses", out stringSet))
          {
            stringSet = new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
            requestContext.Items["MS.TF.DistributedTask.QueueCacheMisses"] = (object) stringSet;
          }
          stringSet.Add(keyToken);
        }
        return queue2;
      }
    }

    public TaskAgentQueue GetAgentQueue(
      IVssRequestContext requestContext,
      Guid projectId,
      string queueName)
    {
      using (new MethodScope(requestContext, nameof (FrameworkPoolService), nameof (GetAgentQueue)))
      {
        ArgumentUtility.CheckStringForNullOrEmpty(queueName, nameof (queueName));
        return requestContext.GetClient<TaskAgentHttpClient>().GetAgentQueuesAsync(projectId, queueName).SyncResult<List<TaskAgentQueue>>().SingleOrDefault<TaskAgentQueue>();
      }
    }

    public TaskAgentQueue GetAgentQueueForPool(
      IVssRequestContext requestContext,
      Guid projectId,
      int poolId)
    {
      using (new MethodScope(requestContext, nameof (FrameworkPoolService), nameof (GetAgentQueueForPool)))
        return requestContext.GetClient<TaskAgentHttpClient>().GetAgentQueuesForPoolsAsync(projectId, (IEnumerable<int>) new int[1]
        {
          poolId
        }).SyncResult<List<TaskAgentQueue>>().SingleOrDefault<TaskAgentQueue>();
    }

    public IList<TaskAgentQueue> GetAgentQueues(
      IVssRequestContext requestContext,
      Guid projectId,
      string queueName = null,
      TaskAgentQueueActionFilter? actionFilter = null)
    {
      using (new MethodScope(requestContext, nameof (FrameworkPoolService), nameof (GetAgentQueues)))
        return (IList<TaskAgentQueue>) requestContext.GetClient<TaskAgentHttpClient>().GetAgentQueuesAsync(projectId, queueName, actionFilter).SyncResult<List<TaskAgentQueue>>();
    }

    public IList<TaskAgentQueue> GetAgentQueues(
      IVssRequestContext requestContext,
      Guid projectId,
      IEnumerable<int> ids,
      TaskAgentQueueActionFilter? actionFilter = null)
    {
      using (new MethodScope(requestContext, nameof (FrameworkPoolService), nameof (GetAgentQueues)))
        return (IList<TaskAgentQueue>) requestContext.GetClient<TaskAgentHttpClient>().GetAgentQueuesByIdsAsync(projectId, ids, actionFilter).SyncResult<List<TaskAgentQueue>>();
    }

    public IList<TaskAgentQueue> GetAgentQueues(
      IVssRequestContext requestContext,
      Guid projectId,
      IEnumerable<string> names,
      TaskAgentQueueActionFilter? actionFilter = null)
    {
      using (new MethodScope(requestContext, nameof (FrameworkPoolService), nameof (GetAgentQueues)))
        return (IList<TaskAgentQueue>) requestContext.GetClient<TaskAgentHttpClient>().GetAgentQueuesByNamesAsync(projectId, names, actionFilter).SyncResult<List<TaskAgentQueue>>();
    }

    public IList<TaskAgentQueue> GetHostedAgentQueues(
      IVssRequestContext requestContext,
      Guid projectId,
      TaskAgentQueueActionFilter? actionFilter = null)
    {
      return (IList<TaskAgentQueue>) new List<TaskAgentQueue>(0);
    }

    public TaskAgentJobRequest GetAgentRequest(
      IVssRequestContext requestContext,
      int poolId,
      long requestId)
    {
      using (new MethodScope(requestContext, nameof (FrameworkPoolService), nameof (GetAgentRequest)))
        return requestContext.GetClient<TaskAgentHttpClient>().GetAgentRequestAsync(poolId, requestId).SyncResult<TaskAgentJobRequest>();
    }

    public Task<TaskAgentJobRequest> GetAgentRequestAsync(
      IVssRequestContext requestContext,
      int poolId,
      long requestId)
    {
      using (new MethodScope(requestContext, nameof (FrameworkPoolService), nameof (GetAgentRequestAsync)))
        return requestContext.GetClient<TaskAgentHttpClient>().GetAgentRequestAsync(poolId, requestId);
    }

    public IList<TaskDefinition> GetTaskDefinitions(
      IVssRequestContext requestContext,
      Guid? taskId = null,
      TaskVersion version = null,
      IList<string> visibility = null,
      bool scopeLocal = false)
    {
      using (new MethodScope(requestContext, nameof (FrameworkPoolService), nameof (GetTaskDefinitions)))
      {
        TaskDefinitionResult definitionResult1 = (TaskDefinitionResult) null;
        IVssRequestContext vssRequestContext = (IVssRequestContext) null;
        Guid serviceAreaId = new Guid();
        if (!scopeLocal && requestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection))
        {
          vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
          serviceAreaId = vssRequestContext.GetService<IInstanceManagementService>().GetHostInstanceMapping(vssRequestContext, requestContext.ServiceHost.InstanceId, ServiceInstanceTypes.TFS).ServiceInstance.InstanceId;
          definitionResult1 = vssRequestContext.GetService<ITaskDefinitionCacheService>().GetTasks(vssRequestContext.Elevate(), new Guid?(serviceAreaId));
        }
        TaskDefinitionResult tasks = requestContext.GetService<ITaskDefinitionCacheService>().GetTasks(requestContext.Elevate(), new Guid?());
        TaskDefinitionResult definitionResult2;
        if (taskId.HasValue && version != (TaskVersion) null)
        {
          TaskDefinition definition = (definitionResult1 != null ? definitionResult1.Get(new Guid?(taskId.Value), version).FirstOrDefault<TaskDefinition>() : (TaskDefinition) null) ?? tasks.Get(new Guid?(taskId.Value), version).FirstOrDefault<TaskDefinition>();
          if (definition == null)
          {
            if (vssRequestContext != null)
            {
              TaskAgentHttpClient client = vssRequestContext.Elevate().GetClient<TaskAgentHttpClient>(serviceAreaId);
              try
              {
                TaskAgentHttpClient taskAgentHttpClient = client;
                bool? nullable = new bool?(true);
                Guid taskId1 = taskId.Value;
                string versionString = (string) version;
                bool? scopeLocal1 = nullable;
                CancellationToken cancellationToken = new CancellationToken();
                definition = taskAgentHttpClient.GetTaskDefinitionAsync(taskId1, versionString, scopeLocal: scopeLocal1, cancellationToken: cancellationToken).SyncResult<TaskDefinition>();
              }
              catch (TaskDefinitionNotFoundException ex)
              {
              }
              if (definition != null)
              {
                definitionResult1.Add(definition);
                vssRequestContext.TraceAlways(10015539, TraceLevel.Info, "DistributedTask", "PoolService", "Read through cache found task definition ID '{0}' version '{1}'", (object) definition.Id, (object) definition.Version);
              }
            }
            if (definition == null)
            {
              TaskAgentHttpClient client = requestContext.Elevate().GetClient<TaskAgentHttpClient>();
              try
              {
                TaskAgentHttpClient taskAgentHttpClient = client;
                bool? nullable = new bool?(true);
                Guid taskId2 = taskId.Value;
                string versionString = (string) version;
                bool? scopeLocal2 = nullable;
                CancellationToken cancellationToken = new CancellationToken();
                definition = taskAgentHttpClient.GetTaskDefinitionAsync(taskId2, versionString, scopeLocal: scopeLocal2, cancellationToken: cancellationToken).SyncResult<TaskDefinition>();
              }
              catch (TaskDefinitionNotFoundException ex)
              {
                requestContext.TraceAlways(10015538, TraceLevel.Info, "DistributedTask", "PoolService", "Read through cache did not find task definition ID '{0}' version '{1}'", (object) taskId, (object) version);
              }
              if (definition != null)
              {
                tasks.Add(definition);
                requestContext.TraceAlways(10015539, TraceLevel.Info, "DistributedTask", "PoolService", "Read through cache found task definition ID '{0}' version '{1}'", (object) definition.Id, (object) definition.Version);
              }
            }
          }
          definitionResult2 = new TaskDefinitionResult(new int?(1));
          if (definition != null)
            definitionResult2.Add(definition);
        }
        else
        {
          definitionResult2 = new TaskDefinitionResult(new int?(1));
          if (definitionResult1 != null)
            definitionResult2.Add(definitionResult1.Get(taskId, (TaskVersion) null));
          definitionResult2.Add(tasks.Get(taskId, (TaskVersion) null));
        }
        return (IList<TaskDefinition>) definitionResult2.Get(visibility != null ? visibility.ToArray<string>() : (string[]) null).ToList<TaskDefinition>();
      }
    }

    public IList<TaskAgent> GetAgents(
      IVssRequestContext requestContext,
      int poolId,
      IList<Demand> demands = null)
    {
      using (new MethodScope(requestContext, nameof (FrameworkPoolService), nameof (GetAgents)))
      {
        TaskAgentHttpClient client = requestContext.GetClient<TaskAgentHttpClient>();
        int poolId1 = poolId;
        IEnumerable<Demand> demands1 = (IEnumerable<Demand>) demands;
        bool? includeCapabilities = new bool?();
        bool? includeAssignedRequest = new bool?();
        bool? includeLastCompletedRequest = new bool?();
        IEnumerable<Demand> demands2 = demands1;
        CancellationToken cancellationToken = new CancellationToken();
        return (IList<TaskAgent>) client.GetAgentsAsync(poolId1, (string) null, includeCapabilities, includeAssignedRequest, includeLastCompletedRequest, (IEnumerable<string>) null, demands2, (object) null, cancellationToken).SyncResult<List<TaskAgent>>();
      }
    }

    public async Task<IList<TaskAgent>> GetAgentsAsync(
      IVssRequestContext requestContext,
      int poolId,
      IList<Demand> demands = null)
    {
      IList<TaskAgent> agentsAsync;
      using (new MethodScope(requestContext, nameof (FrameworkPoolService), nameof (GetAgentsAsync)))
      {
        TaskAgentHttpClient client = requestContext.GetClient<TaskAgentHttpClient>();
        int poolId1 = poolId;
        IEnumerable<Demand> demands1 = (IEnumerable<Demand>) demands;
        bool? includeCapabilities = new bool?();
        bool? includeAssignedRequest = new bool?();
        bool? includeLastCompletedRequest = new bool?();
        IEnumerable<Demand> demands2 = demands1;
        CancellationToken cancellationToken = new CancellationToken();
        agentsAsync = (IList<TaskAgent>) await client.GetAgentsAsync(poolId1, (string) null, includeCapabilities, includeAssignedRequest, includeLastCompletedRequest, (IEnumerable<string>) null, demands2, (object) null, cancellationToken);
      }
      return agentsAsync;
    }

    public async Task<TaskAgent> GetAgentAsync(
      IVssRequestContext requestContext,
      int poolId,
      int agentId)
    {
      TaskAgent agentAsync;
      using (new MethodScope(requestContext, nameof (FrameworkPoolService), nameof (GetAgentAsync)))
        agentAsync = await requestContext.GetClient<TaskAgentHttpClient>().GetAgentAsync(poolId, agentId);
      return agentAsync;
    }

    public async Task<IList<DeploymentMachine>> GetDeploymentMachinesAsync(
      IVssRequestContext requestContext,
      Guid projectId,
      int machineGroupId,
      IList<string> tagFilters = null)
    {
      IList<DeploymentMachine> deploymentMachinesAsync;
      using (new MethodScope(requestContext, nameof (FrameworkPoolService), nameof (GetDeploymentMachinesAsync)))
        deploymentMachinesAsync = (IList<DeploymentMachine>) await requestContext.GetClient<TaskAgentHttpClient>().GetDeploymentMachinesAsync(projectId, machineGroupId, (IEnumerable<string>) tagFilters);
      return deploymentMachinesAsync;
    }

    public async Task<IPagedList<DeploymentMachine>> GetDeploymentMachinesAsync(
      IVssRequestContext requestContext,
      Guid projectId,
      int deploymentGroupId,
      IList<string> tagFilters = null,
      string continuationToken = null,
      DeploymentTargetExpands expands = DeploymentTargetExpands.None,
      bool? enabled = null,
      IList<string> propertyFilters = null)
    {
      IPagedList<DeploymentMachine> continuationToken1;
      using (new MethodScope(requestContext, nameof (FrameworkPoolService), nameof (GetDeploymentMachinesAsync)))
      {
        TaskAgentHttpClient client = requestContext.GetClient<TaskAgentHttpClient>();
        Guid project = projectId;
        int deploymentGroupId1 = deploymentGroupId;
        IList<string> tags = tagFilters;
        DeploymentTargetExpands? nullable1 = new DeploymentTargetExpands?(expands);
        string str = continuationToken;
        bool? nullable2 = enabled;
        IEnumerable<string> strings = (IEnumerable<string>) propertyFilters;
        bool? partialNameMatch = new bool?();
        DeploymentTargetExpands? expand = nullable1;
        TaskAgentStatusFilter? agentStatus = new TaskAgentStatusFilter?();
        TaskAgentJobResultFilter? agentJobResult = new TaskAgentJobResultFilter?();
        string continuationToken2 = str;
        int? top = new int?();
        CancellationToken cancellationToken = new CancellationToken();
        bool? enabled1 = nullable2;
        IEnumerable<string> propertyFilters1 = strings;
        continuationToken1 = await client.GetDeploymentTargetsAsyncWithContinuationToken(project, deploymentGroupId1, (IEnumerable<string>) tags, partialNameMatch: partialNameMatch, expand: expand, agentStatus: agentStatus, agentJobResult: agentJobResult, continuationToken: continuationToken2, top: top, cancellationToken: cancellationToken, enabled: enabled1, propertyFilters: propertyFilters1);
      }
      return continuationToken1;
    }

    public DeploymentGroup GetDeploymentGroup(
      IVssRequestContext requestContext,
      Guid projectId,
      int machineGroupId,
      bool includeMachines = false)
    {
      using (new MethodScope(requestContext, nameof (FrameworkPoolService), nameof (GetDeploymentGroup)))
      {
        DeploymentGroupExpands deploymentGroupExpands = includeMachines ? DeploymentGroupExpands.Machines : DeploymentGroupExpands.None;
        TaskAgentHttpClient client = requestContext.GetClient<TaskAgentHttpClient>();
        Guid project = projectId;
        int deploymentGroupId = machineGroupId;
        DeploymentGroupExpands? nullable = new DeploymentGroupExpands?(deploymentGroupExpands);
        DeploymentGroupActionFilter? actionFilter = new DeploymentGroupActionFilter?();
        DeploymentGroupExpands? expand = nullable;
        CancellationToken cancellationToken = new CancellationToken();
        return client.GetDeploymentGroupAsync(project, deploymentGroupId, actionFilter, expand, cancellationToken: cancellationToken).SyncResult<DeploymentGroup>();
      }
    }

    public IList<DeploymentGroup> GetDeploymentGroups(
      IVssRequestContext requestContext,
      Guid projectId,
      string deploymentGroupName = null)
    {
      using (new MethodScope(requestContext, nameof (FrameworkPoolService), nameof (GetDeploymentGroups)))
        return (IList<DeploymentGroup>) requestContext.GetClient<TaskAgentHttpClient>().GetDeploymentGroupsAsync(projectId, deploymentGroupName).SyncResult<List<DeploymentGroup>>();
    }

    public IList<DeploymentGroup> GetDeploymentGroupsByIds(
      IVssRequestContext requestContext,
      Guid projectId,
      IList<int> deploymentGroupIds)
    {
      using (new MethodScope(requestContext, nameof (FrameworkPoolService), nameof (GetDeploymentGroupsByIds)))
        return (IList<DeploymentGroup>) requestContext.GetClient<TaskAgentHttpClient>().GetDeploymentGroupsAsync(projectId, ids: (IEnumerable<int>) deploymentGroupIds).SyncResult<List<DeploymentGroup>>();
    }

    public IList<DeploymentPoolSummary> GetDeploymentPoolsSummary(
      IVssRequestContext requestContext,
      string poolName = null,
      bool includeDeploymentGroupReferences = false,
      IList<int> poolIds = null)
    {
      using (new MethodScope(requestContext, nameof (FrameworkPoolService), nameof (GetDeploymentPoolsSummary)))
      {
        DeploymentPoolSummaryExpands poolSummaryExpands = includeDeploymentGroupReferences ? DeploymentPoolSummaryExpands.DeploymentGroups : DeploymentPoolSummaryExpands.None;
        return (IList<DeploymentPoolSummary>) requestContext.GetClient<TaskAgentHttpClient>().GetDeploymentPoolsSummaryAsync(poolName, new DeploymentPoolSummaryExpands?(poolSummaryExpands), (IEnumerable<int>) poolIds).SyncResult<List<DeploymentPoolSummary>>();
      }
    }

    public IList<TaskGroup> GetTaskGroups(
      IVssRequestContext requestContext,
      Guid project,
      Guid? taskGroupId = null,
      bool? expanded = null)
    {
      using (new MethodScope(requestContext, nameof (FrameworkPoolService), nameof (GetTaskGroups)))
        return (IList<TaskGroup>) requestContext.GetClient<TaskAgentHttpClient>().GetTaskGroupsAsync(project, taskGroupId, expanded, new Guid?(), new bool?(), new int?(), new DateTime?(), new TaskGroupQueryOrder?(), (object) null, new CancellationToken()).SyncResult<List<TaskGroup>>();
    }

    public TaskGroup GetTaskGroup(
      IVssRequestContext requestContext,
      Guid project,
      Guid taskGroupId,
      string versionSpec,
      bool? expanded = null)
    {
      using (new MethodScope(requestContext, nameof (FrameworkPoolService), nameof (GetTaskGroup)))
      {
        int num;
        if (expanded.HasValue)
        {
          bool? nullable = expanded;
          bool flag = true;
          if (nullable.GetValueOrDefault() == flag & nullable.HasValue)
          {
            num = 2;
            goto label_5;
          }
        }
        num = 0;
label_5:
        TaskGroupExpands taskGroupExpands = (TaskGroupExpands) num;
        return requestContext.GetClient<TaskAgentHttpClient>().GetTaskGroupAsync(project, taskGroupId, versionSpec, new TaskGroupExpands?(taskGroupExpands)).SyncResult<TaskGroup>();
      }
    }

    public async Task<IList<TaskAgentJobRequest>> GetAgentRequestsForAgentsAsync(
      IVssRequestContext requestContext,
      int poolId,
      IList<int> agentIds,
      int completedRequests)
    {
      IList<TaskAgentJobRequest> requestsForAgentsAsync;
      using (new MethodScope(requestContext, nameof (FrameworkPoolService), nameof (GetAgentRequestsForAgentsAsync)))
        requestsForAgentsAsync = (IList<TaskAgentJobRequest>) await requestContext.GetClient<TaskAgentHttpClient>().GetAgentRequestsForAgentsAsync(poolId, (IEnumerable<int>) agentIds, new int?(completedRequests));
      return requestsForAgentsAsync;
    }

    public async Task<TaskAgentJobRequest> QueueAgentRequestByPoolAsync(
      IVssRequestContext requestContext,
      int poolId,
      TaskAgentJobRequest request)
    {
      TaskAgentJobRequest taskAgentJobRequest;
      using (new MethodScope(requestContext, nameof (FrameworkPoolService), nameof (QueueAgentRequestByPoolAsync)))
      {
        KPIHelper.PublishDTAgentRequestSent(requestContext);
        taskAgentJobRequest = await requestContext.GetClient<TaskAgentHttpClient>().QueueAgentRequestByPoolAsync(poolId, request);
      }
      return taskAgentJobRequest;
    }

    public async Task SendAgentMessageAsync(
      IVssRequestContext requestContext,
      int poolId,
      long requestId,
      TaskAgentMessage message)
    {
      MethodScope methodScope = new MethodScope(requestContext, nameof (FrameworkPoolService), nameof (SendAgentMessageAsync));
      try
      {
        await requestContext.GetClient<TaskAgentHttpClient>().SendMessageAsync(poolId, requestId, message);
      }
      finally
      {
        methodScope.Dispose();
      }
      methodScope = new MethodScope();
    }

    public TaskAgent UpdateAgent(IVssRequestContext requestContext, int poolId, TaskAgent agent)
    {
      using (new MethodScope(requestContext, nameof (FrameworkPoolService), nameof (UpdateAgent)))
        return requestContext.GetClient<TaskAgentHttpClient>().ReplaceAgentAsync(poolId, agent).SyncResult<TaskAgent>();
    }

    public TaskAgentJobRequest UpdateAgentRequest(
      IVssRequestContext requestContext,
      int poolId,
      long requestId,
      TaskAgentJobRequest request)
    {
      using (new MethodScope(requestContext, nameof (FrameworkPoolService), nameof (UpdateAgentRequest)))
        return requestContext.GetClient<TaskAgentHttpClient>().UpdateAgentRequestAsync(poolId, requestId, Guid.Empty, request).SyncResult<TaskAgentJobRequest>();
    }

    public void CreateTeamProject(IVssRequestContext requestContext, Guid projectId) => throw new InvalidOperationException("CreateTeamProject can only be called by ProjectCreate event.");

    public void DeleteTeamProject(IVssRequestContext requestContext, Guid projectId) => throw new InvalidOperationException("DeleteTeamProject can only be called by ProjectDelete event.");

    private void OnTasksChanged(IVssRequestContext requestContext, NotificationEventArgs args)
    {
      requestContext.TraceInfo(10015513, "Notification", "Received TasksChanged event, Invalidating cache entries. Event Data {0}", (object) args.Data);
      requestContext.GetService<ITaskDefinitionCacheService>().Invalidate(requestContext);
    }

    void IVssFrameworkService.ServiceStart(IVssRequestContext requestContext) => requestContext.GetService<ITeamFoundationSqlNotificationService>().RegisterNotification(requestContext, "Default", SqlNotificationEventIds.TasksChanged, new SqlNotificationHandler(this.OnTasksChanged), false);

    void IVssFrameworkService.ServiceEnd(IVssRequestContext requestContext)
    {
    }
  }
}
