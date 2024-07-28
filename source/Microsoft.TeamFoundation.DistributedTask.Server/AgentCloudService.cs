// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.AgentCloudService
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.Azure.Pipelines.PoolProvider.Contracts;
using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.DataAccess;
using Microsoft.TeamFoundation.DistributedTask.Server.DataAccess;
using Microsoft.TeamFoundation.DistributedTask.Tasks;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Orchestration;
using Microsoft.VisualStudio.Services.Orchestration.Server;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.DistributedTask.Server
{
  public class AgentCloudService : 
    IInternalAgentCloudService,
    IAgentCloudService,
    IVssFrameworkService
  {
    private IAgentCloudSecurityProvider m_security;
    private PrivateAgentPoolProvider m_privatePoolProvider = new PrivateAgentPoolProvider();
    private IDictionary<RemoteServerPoolProviderType, Func<TaskAgentCloud, TaskAgentPoolData, IServerPoolProvider>> m_remoteProviderFactories;
    private bool m_agentCloudOrchestrationHubExists;
    private static IDictionary<RemoteServerPoolProviderType, Func<TaskAgentCloud, TaskAgentPoolData, IServerPoolProvider>> s_defaultFactories = (IDictionary<RemoteServerPoolProviderType, Func<TaskAgentCloud, TaskAgentPoolData, IServerPoolProvider>>) new Dictionary<RemoteServerPoolProviderType, Func<TaskAgentCloud, TaskAgentPoolData, IServerPoolProvider>>()
    {
      {
        RemoteServerPoolProviderType.Internal,
        (Func<TaskAgentCloud, TaskAgentPoolData, IServerPoolProvider>) ((agentCloud, pool) => (IServerPoolProvider) new InternalPoolProvider(agentCloud, pool))
      },
      {
        RemoteServerPoolProviderType.Remote,
        (Func<TaskAgentCloud, TaskAgentPoolData, IServerPoolProvider>) ((agentCloud, pool) => (IServerPoolProvider) new RemoteAgentPoolProvider(agentCloud, pool))
      }
    };
    private const string c_initialLeaseTimeoutName = "InitialLeaseTimeout";
    private const string c_initialLeaseTimeoutPath = "/Service/DistributedTask/Settings/AgentRequest/InitialLeaseTimeout";
    private const string c_leaseRenewalTimeoutName = "LeaseRenewalTimeout";
    private const string c_leaseRenewalTimeoutPath = "/Service/DistributedTask/Settings/AgentRequest/LeaseRenewalTimeout";
    private const string c_agentCloudHubName = "AgentCloud";
    private const string c_agentCloudHubType = "AgentCloud";
    private const string c_runAgent = "RunAgent";

    public AgentCloudService()
      : this((IAgentCloudSecurityProvider) new DefaultAgentCloudSecurityProvider(), AgentCloudService.s_defaultFactories)
    {
    }

    internal AgentCloudService(
      IAgentCloudSecurityProvider security,
      IDictionary<RemoteServerPoolProviderType, Func<TaskAgentCloud, TaskAgentPoolData, IServerPoolProvider>> remoteProviderFactories)
    {
      this.m_security = security;
      this.m_remoteProviderFactories = remoteProviderFactories;
    }

    public static string HubName => "AgentCloud";

    private IAgentCloudSecurityProvider Security => this.m_security;

    public void ServiceStart(IVssRequestContext requestContext)
    {
      requestContext.GetService<IVssRegistryService>().RegisterNotification(requestContext, new RegistrySettingsChangedCallback(this.OnAgentRequestSettingsChanged), "/Service/DistributedTask/Settings/AgentRequest/**");
      this.OnAgentRequestSettingsChanged(requestContext, (RegistryEntryCollection) null);
      this.m_agentCloudOrchestrationHubExists = requestContext.GetService<OrchestrationService>().GetHubDescription(requestContext, "AgentCloud") != null;
    }

    public void ServiceEnd(IVssRequestContext requestContext) => requestContext.GetService<IVssRegistryService>().UnregisterNotification(requestContext, new RegistrySettingsChangedCallback(this.OnAgentRequestSettingsChanged));

    public async Task<TaskAgentCloud> AddAgentCloudAsync(
      IVssRequestContext requestContext,
      TaskAgentCloud agentCloud)
    {
      requestContext.AssertAsyncExecutionEnabled();
      requestContext = requestContext.ToPoolRequestContext();
      TaskAgentCloud taskAgentCloud1;
      using (new MethodScope(requestContext, nameof (AgentCloudService), nameof (AddAgentCloudAsync)))
      {
        agentCloud.Validate(requestContext);
        Guid id = agentCloud.Id;
        if (agentCloud.Id == Guid.Empty)
          agentCloud.Id = Guid.NewGuid();
        this.Security.CheckAgentCloudPermission(requestContext, 2);
        TaskAgentCloud createdCloud = (TaskAgentCloud) null;
        TaskResourceComponent rc = requestContext.CreateComponent<TaskResourceComponent>();
        try
        {
          createdCloud = await rc.AddAgentCloudAsync(agentCloud.Id, agentCloud.Name, agentCloud.Type, agentCloud.GetAgentDefinitionEndpoint, agentCloud.AcquireAgentEndpoint, agentCloud.GetAgentRequestStatusEndpoint, agentCloud.ReleaseAgentEndpoint, agentCloud.GetAccountParallelismEndpoint, agentCloud.MaxParallelism, agentCloud.AcquisitionTimeout, agentCloud.Internal.GetValueOrDefault());
        }
        finally
        {
          rc?.Dispose();
        }
        rc = (TaskResourceComponent) null;
        try
        {
          createdCloud.SharedSecret = agentCloud.SharedSecret;
          createdCloud.SaveSharedSecret(requestContext);
          createdCloud.SharedSecret = (string) null;
        }
        catch (Exception ex)
        {
          createdCloud.DeleteSharedSecret(requestContext);
          rc = requestContext.CreateComponent<TaskResourceComponent>();
          try
          {
            TaskAgentCloud taskAgentCloud2 = await rc.DeleteAgentCloudAsync(agentCloud.AgentCloudId);
          }
          finally
          {
            rc?.Dispose();
          }
          rc = (TaskResourceComponent) null;
          throw;
        }
        if (createdCloud.GetAccountParallelismEndpoint != null)
          requestContext.GetService<ITeamFoundationJobService>().QueueJobsNow(requestContext, (IEnumerable<Guid>) new Guid[1]
          {
            TaskConstants.UpdateAgentCloudParallelismJob
          });
        taskAgentCloud1 = createdCloud;
      }
      return taskAgentCloud1;
    }

    public async Task AddAgentCloudRequestMessageAsync(
      IVssRequestContext requestContext,
      int agentCloudId,
      Guid agentCloudRequestId,
      AgentRequestMessage requestMessage)
    {
      requestContext.AssertAsyncExecutionEnabled();
      requestContext = requestContext.ToPoolRequestContext();
      MethodScope methodScope = new MethodScope(requestContext, nameof (AgentCloudService), nameof (AddAgentCloudRequestMessageAsync));
      try
      {
        this.Security.CheckAgentCloudPermission(requestContext, agentCloudId, agentCloudRequestId, 4);
        TaskAgentJobRequest request = (TaskAgentJobRequest) null;
        using (TaskResourceComponent rc = requestContext.CreateComponent<TaskResourceComponent>())
          request = await rc.GetAgentRequestForAgentCloudRequestAsync(agentCloudId, agentCloudRequestId);
        if (request != null)
        {
          using (requestContext.CreateOrchestrationIdScope(request.OrchestrationId))
            await request.AddMessageAsync(requestContext.Elevate(), requestMessage);
        }
      }
      finally
      {
        methodScope.Dispose();
      }
      methodScope = new MethodScope();
    }

    public async Task<TaskAgentCloudRequest> CreateAgentCloudRequestAsync(
      IVssRequestContext requestContext,
      TaskAgentCloudRequest request)
    {
      requestContext.AssertAsyncExecutionEnabled();
      requestContext = requestContext.ToPoolRequestContext();
      TaskAgentCloudRequest cloudRequestAsync;
      using (new MethodScope(requestContext, nameof (AgentCloudService), nameof (CreateAgentCloudRequestAsync)))
      {
        using (TaskResourceComponent rc = requestContext.CreateComponent<TaskResourceComponent>())
          cloudRequestAsync = await rc.AddAgentCloudRequestAsync(request.AgentCloudId, request.RequestId, request.Pool.Id, request.Agent.Id, request.AgentSpecification);
      }
      return cloudRequestAsync;
    }

    public async Task<TaskAgentCloud> DeleteAgentCloudAsync(
      IVssRequestContext requestContext,
      int agentCloudId)
    {
      requestContext = requestContext.ToPoolRequestContext();
      using (new MethodScope(requestContext, nameof (AgentCloudService), nameof (DeleteAgentCloudAsync)))
      {
        if (!this.Security.HasAgentCloudPermission(requestContext, agentCloudId, 2, true))
          return (TaskAgentCloud) null;
        TaskAgentCloud cloud = await this.GetAgentCloudAsync(requestContext, agentCloudId);
        cloud.ValidateInternalPermissions(requestContext);
        using (TaskResourceComponent rc = requestContext.CreateComponent<TaskResourceComponent>())
          cloud = await rc.DeleteAgentCloudAsync(agentCloudId);
        this.InvalidateInMemCaches(requestContext, agentCloudId);
        return cloud;
      }
    }

    public bool DeliverEvent(
      IVssRequestContext requestContext,
      RunAgentEvent agentEvent,
      bool createOrchestration)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<RunAgentEvent>(agentEvent, nameof (agentEvent));
      requestContext = requestContext.ToPoolRequestContext();
      this.EnsureAgentCloudHubExists(requestContext);
      bool flag = false;
      OrchestrationService service = requestContext.GetService<OrchestrationService>();
      try
      {
        service.RaiseEvent(requestContext, "AgentCloud", agentEvent.AgentCloudRequestId.ToString(), agentEvent.EventType, agentEvent.GetEventData(), true);
        flag = true;
      }
      catch (OrchestrationSessionNotFoundException ex)
      {
        if (createOrchestration)
        {
          requestContext.TraceWarning(nameof (AgentCloudService), "Could not deliver event " + agentEvent.EventType + " to orchestration " + agentEvent.AgentCloudRequestId.ToString() + " as it does not exist!");
          this.EnsureRunAgentOrchestrationExists(requestContext, agentEvent.AgentCloudId, agentEvent.AgentCloudRequestId, agentEvent.PoolId, agentEvent.AgentId, out bool _);
          service.RaiseEvent(requestContext, "AgentCloud", agentEvent.AgentCloudRequestId.ToString(), agentEvent.EventType, agentEvent.GetEventData(), true);
          flag = true;
        }
      }
      if (flag)
        this.QueueAgentCloudRequestOrchestrationMonitorJob(requestContext);
      return flag;
    }

    public async Task<bool> DeliverEventAsync(
      IVssRequestContext requestContext,
      RunAgentEvent agentEvent,
      bool createOrchestration)
    {
      requestContext = requestContext.ToPoolRequestContext();
      this.EnsureAgentCloudHubExists(requestContext);
      bool deliveredEvent = false;
      OrchestrationService orchestrationService = requestContext.GetService<OrchestrationService>();
      int num = 0;
      try
      {
        await orchestrationService.RaiseEventAsync(requestContext, "AgentCloud", agentEvent.AgentCloudRequestId.ToString(), agentEvent.EventType, agentEvent.GetEventData(), true);
        deliveredEvent = true;
      }
      catch (OrchestrationSessionNotFoundException ex)
      {
        num = 1;
      }
      if (num == 1 && createOrchestration)
      {
        requestContext.TraceWarning(nameof (AgentCloudService), "Could not deliver event " + agentEvent.EventType + " to orchestration " + agentEvent.AgentCloudRequestId.ToString() + " as it does not exist!");
        this.EnsureRunAgentOrchestrationExists(requestContext, agentEvent.AgentCloudId, agentEvent.AgentCloudRequestId, agentEvent.PoolId, agentEvent.AgentId, out bool _);
        await orchestrationService.RaiseEventAsync(requestContext, "AgentCloud", agentEvent.AgentCloudRequestId.ToString(), agentEvent.EventType, agentEvent.GetEventData(), true);
        deliveredEvent = true;
      }
      if (deliveredEvent)
        this.QueueAgentCloudRequestOrchestrationMonitorJob(requestContext);
      bool flag = deliveredEvent;
      orchestrationService = (OrchestrationService) null;
      return flag;
    }

    public OrchestrationInstance EnsureRunAgentOrchestrationExists(
      IVssRequestContext requestContext,
      int agentCloudId,
      Guid agentCloudRequestId,
      int poolId,
      int agentId,
      out bool createdInstance)
    {
      OrchestrationService service = requestContext.GetService<OrchestrationService>();
      createdInstance = true;
      int num = requestContext.GetService<IVssRegistryService>().GetValue<int>(requestContext, (RegistryQuery) string.Format("/Service/Orchestration/Settings/ActivityDispatcher/{0}/CountShards", (object) "AgentCloud"), true, 1);
      bool flag = requestContext.IsFeatureEnabled("DistributedTask.ShardActivityDispatcherByPoolProvider");
      try
      {
        RunAgentInput input = new RunAgentInput()
        {
          ActivityDispatcherShardsCount = num,
          AgentCloudId = agentCloudId,
          AgentCloudRequestId = agentCloudRequestId,
          AgentPoolId = poolId,
          AgentId = agentId,
          ShardActivityDispatcherByPoolProvider = flag
        };
        return service.CreateOrchestrationInstance(requestContext, "AgentCloud", RunAgent.Name, RunAgent.Version, agentCloudRequestId.ToString(), (object) input);
      }
      catch (OrchestrationSessionExistsException ex)
      {
        createdInstance = false;
        requestContext.TraceWarning(nameof (AgentCloudService), "RunAgent orchestration with id " + agentCloudRequestId.ToString() + " already exists.");
        return new OrchestrationInstance()
        {
          InstanceId = agentCloudRequestId.ToString()
        };
      }
    }

    public async Task<IList<TaskAgentCloudRequest>> GetActiveAgentCloudRequestsAsync(
      IVssRequestContext requestContext)
    {
      requestContext = requestContext.ToPoolRequestContext();
      IList<TaskAgentCloudRequest> cloudRequestsAsync;
      using (new MethodScope(requestContext, nameof (AgentCloudService), nameof (GetActiveAgentCloudRequestsAsync)))
      {
        using (TaskResourceComponent trc = requestContext.CreateComponent<TaskResourceComponent>())
          cloudRequestsAsync = await trc.GetActiveAgentCloudRequestsAsync();
      }
      return cloudRequestsAsync;
    }

    public async Task<TaskAgentCloud> GetAgentCloudAsync(
      IVssRequestContext requestContext,
      int agentCloudId)
    {
      requestContext = requestContext.ToPoolRequestContext();
      using (new MethodScope(requestContext, nameof (AgentCloudService), nameof (GetAgentCloudAsync)))
        return !this.Security.HasAgentCloudPermission(requestContext, agentCloudId, 1, true) ? (TaskAgentCloud) null : await this.GetAgentCloudInternalAsync(requestContext, agentCloudId);
    }

    public TaskAgentCloud GetAgentCloud(IVssRequestContext requestContext, int agentCloudId) => this.GetAgentCloud(requestContext, agentCloudId, false);

    public TaskAgentCloud GetAgentCloud(
      IVssRequestContext requestContext,
      int agentCloudId,
      bool includeType = false)
    {
      requestContext = requestContext.ToPoolRequestContext();
      using (new MethodScope(requestContext, nameof (AgentCloudService), nameof (GetAgentCloud)))
        return !this.Security.HasAgentCloudPermission(requestContext, agentCloudId, 1, true) ? (TaskAgentCloud) null : this.GetAgentCloudInternal(requestContext, agentCloudId, false, includeType);
    }

    public async Task<IList<TaskAgentCloud>> GetAgentCloudsAsync(IVssRequestContext requestContext)
    {
      requestContext = requestContext.ToPoolRequestContext();
      IList<TaskAgentCloud> allAgentClouds = (IList<TaskAgentCloud>) null;
      MethodScope methodScope = new MethodScope(requestContext, nameof (AgentCloudService), nameof (GetAgentCloudsAsync));
      try
      {
        using (TaskResourceComponent rc = requestContext.CreateComponent<TaskResourceComponent>())
          allAgentClouds = await rc.GetAgentCloudsAsync(new int?());
      }
      finally
      {
        methodScope.Dispose();
      }
      methodScope = new MethodScope();
      return this.FilterAgentClouds(requestContext, allAgentClouds);
    }

    public async Task<IList<AgentDefinition>> GetAgentDefinitionsAsync(
      IVssRequestContext requestContext,
      int agentCloudId)
    {
      AgentCloudService agentCloudService = this;
      // ISSUE: explicit non-virtual call
      TaskAgentCloud cloudInternalAsync = await __nonvirtual (agentCloudService.GetAgentCloudInternalAsync(requestContext, agentCloudId, true, true));
      return await ((IInternalAgentCloudService) agentCloudService).GetPoolProviderForAgentCloud(requestContext, cloudInternalAsync, (TaskAgentPoolData) null).GetAgentDefinitionsAsync(requestContext);
    }

    public TaskAgentCloud GetAgentCloudInternal(
      IVssRequestContext requestContext,
      int agentCloudId,
      bool includeSharedSecret = false,
      bool includeType = false)
    {
      requestContext = requestContext.ToPoolRequestContext();
      using (new MethodScope(requestContext, nameof (AgentCloudService), nameof (GetAgentCloudInternal)))
      {
        TaskAgentCloudCacheService service = requestContext.GetService<TaskAgentCloudCacheService>();
        TaskAgentCloud cloud;
        if (!service.TryGetValue(requestContext, agentCloudId, out cloud))
        {
          using (TaskResourceComponent component = requestContext.CreateComponent<TaskResourceComponent>())
            cloud = component.GetAgentClouds(new int?(agentCloudId)).FirstOrDefault<TaskAgentCloud>();
          if (cloud == null)
          {
            requestContext.TraceError(nameof (AgentCloudService), string.Format("No TaskAgentCloud found with the id {0}!", (object) agentCloudId));
            throw new TaskAgentCloudNotFoundException(TaskResources.AgentCloudNotFound((object) agentCloudId));
          }
          cloud.LoadSharedSecret(requestContext);
          service.Set(requestContext, cloud.AgentCloudId, cloud);
        }
        if (cloud == null)
        {
          requestContext.TraceError(nameof (AgentCloudService), string.Format("Null TaskAgentCloud found in cache with id {0}!", (object) agentCloudId));
          throw new TaskAgentCloudNotFoundException(TaskResources.AgentCloudNotFound((object) agentCloudId));
        }
        TaskAgentCloud agentCloudInternal = cloud.Clone();
        if (!includeType)
          agentCloudInternal.Type = (string) null;
        if (!includeSharedSecret)
          agentCloudInternal.SharedSecret = (string) null;
        return agentCloudInternal;
      }
    }

    public async Task<TaskAgentCloud> GetAgentCloudInternalAsync(
      IVssRequestContext requestContext,
      int agentCloudId,
      bool includeSharedSecret = false,
      bool updateCache = true)
    {
      requestContext = requestContext.ToPoolRequestContext();
      TaskAgentCloud cloudInternalAsync;
      using (new MethodScope(requestContext, nameof (AgentCloudService), nameof (GetAgentCloudInternalAsync)))
      {
        TaskAgentCloudCacheService cacheService = requestContext.GetService<TaskAgentCloudCacheService>();
        TaskAgentCloud cloud;
        if (!cacheService.TryGetValue(requestContext, agentCloudId, out cloud))
        {
          using (TaskResourceComponent rc = requestContext.CreateComponent<TaskResourceComponent>())
            cloud = (await rc.GetAgentCloudsAsync(new int?(agentCloudId))).FirstOrDefault<TaskAgentCloud>();
          if (cloud == null)
          {
            requestContext.TraceError(nameof (AgentCloudService), string.Format("No TaskAgentCloud found with the id {0}!", (object) agentCloudId));
            throw new TaskAgentCloudNotFoundException(TaskResources.AgentCloudNotFound((object) agentCloudId));
          }
          if (includeSharedSecret | updateCache)
            cloud.LoadSharedSecret(requestContext);
          if (updateCache)
            cacheService.Set(requestContext, cloud.AgentCloudId, cloud);
        }
        if (cloud == null)
        {
          requestContext.TraceError(nameof (AgentCloudService), string.Format("Null TaskAgentCloud found in cache with id {0}!", (object) agentCloudId));
          throw new TaskAgentCloudNotFoundException(TaskResources.AgentCloudNotFound((object) agentCloudId));
        }
        cloud = cloud.Clone();
        if (!includeSharedSecret)
          cloud.SharedSecret = (string) null;
        cloudInternalAsync = cloud;
      }
      return cloudInternalAsync;
    }

    public async Task<TaskAgentCloudRequest> GetAgentCloudRequestForAgentAsync(
      IVssRequestContext requestContext,
      int poolId,
      int agentId)
    {
      requestContext.AssertAsyncExecutionEnabled();
      requestContext = requestContext.ToPoolRequestContext();
      TaskAgentCloudRequest requestForAgentAsync;
      using (new MethodScope(requestContext, nameof (AgentCloudService), nameof (GetAgentCloudRequestForAgentAsync)))
      {
        using (TaskResourceComponent rc = requestContext.CreateComponent<TaskResourceComponent>())
          requestForAgentAsync = await rc.GetAgentCloudRequestForAgentAsync(poolId, agentId);
      }
      return requestForAgentAsync;
    }

    public async Task<TaskAgentCloudRequest> GetAgentCloudRequestAsync(
      IVssRequestContext requestContext,
      int agentCloudId,
      Guid requestId)
    {
      requestContext = requestContext.ToPoolRequestContext();
      using (new MethodScope(requestContext, nameof (AgentCloudService), nameof (GetAgentCloudRequestAsync)))
      {
        if (!this.Security.HasAgentCloudPermission(requestContext, agentCloudId, requestId, 1, true))
          return (TaskAgentCloudRequest) null;
        using (TaskResourceComponent rc = requestContext.CreateComponent<TaskResourceComponent>())
          return await rc.GetAgentCloudRequestAsync(agentCloudId, requestId);
      }
    }

    public async Task<IList<TaskAgentCloudRequest>> GetAgentCloudRequestsAsync(
      IVssRequestContext requestContext,
      int agentCloudId)
    {
      requestContext = requestContext.ToPoolRequestContext();
      IList<TaskAgentCloudRequest> cloudRequestsAsync = (IList<TaskAgentCloudRequest>) null;
      MethodScope methodScope = new MethodScope(requestContext, nameof (AgentCloudService), nameof (GetAgentCloudRequestsAsync));
      try
      {
        if (!this.Security.HasAgentCloudPermission(requestContext, agentCloudId, 1, true))
          return (IList<TaskAgentCloudRequest>) null;
        using (TaskResourceComponent rc = requestContext.CreateComponent<TaskResourceComponent>())
          cloudRequestsAsync = await rc.GetAgentCloudRequestsAsync(new int?(agentCloudId));
      }
      finally
      {
        methodScope.Dispose();
      }
      methodScope = new MethodScope();
      return cloudRequestsAsync;
    }

    public async Task<string> GetAgentCloudRequestStatusAsync(
      IVssRequestContext requestContext,
      TaskAgentCloudRequest request)
    {
      AgentCloudService agentCloudService = this;
      requestContext.AssertAsyncExecutionEnabled();
      requestContext = requestContext.ToPoolRequestContext();
      string requestStatusAsync;
      using (new MethodScope(requestContext, nameof (AgentCloudService), nameof (GetAgentCloudRequestStatusAsync)))
      {
        TaskAgentCloud agentCloud = (TaskAgentCloud) null;
        if (request != null)
        {
          // ISSUE: explicit non-virtual call
          agentCloud = await __nonvirtual (agentCloudService.GetAgentCloudInternalAsync(requestContext, request.AgentCloudId, true, true));
        }
        string str;
        if (agentCloud != null && string.IsNullOrEmpty(agentCloud.GetAgentRequestStatusEndpoint))
        {
          str = TaskResources.WaitingForAgentCloudProvisioning((object) TaskResources.AgentCloudDoesNotSupportStatus());
        }
        else
        {
          IServerPoolProvider providerForAgentCloud = ((IInternalAgentCloudService) agentCloudService).GetPoolProviderForAgentCloud(requestContext, agentCloud, (TaskAgentPoolData) null);
          try
          {
            str = await providerForAgentCloud.GetAgentRequestStatusAsync(requestContext, request);
          }
          catch (Exception ex)
          {
            requestContext.TraceError(nameof (AgentCloudService), "Failure getting status for agent cloud request {0}!", (object) (request?.RequestId.ToString() ?? "none (private agent)"));
            requestContext.TraceException(nameof (AgentCloudService), ex);
            str = TaskResources.AgentCloudConnectionIssue();
          }
          if (string.IsNullOrEmpty(str))
            str = TaskResources.WaitingForAgentCloudProvisioning((object) TaskResources.AgentCloudDidNotReturnStatus());
          else if (agentCloud != null)
            str = TaskResources.WaitingForAgentCloudProvisioning((object) str);
        }
        requestStatusAsync = str;
      }
      return requestStatusAsync;
    }

    public async Task<AgentRequestJob> GetTaskAgentCloudRequestJobAsync(
      IVssRequestContext requestContext,
      int agentCloudId,
      Guid agentCloudRequestId)
    {
      requestContext.AssertAsyncExecutionEnabled();
      requestContext = requestContext.ToPoolRequestContext();
      AgentRequestJob cloudRequestJobAsync;
      using (new MethodScope(requestContext, nameof (AgentCloudService), nameof (GetTaskAgentCloudRequestJobAsync)))
      {
        this.Security.CheckAgentCloudPermission(requestContext, agentCloudId, agentCloudRequestId, 4);
        TaskAgentJobRequest request = (TaskAgentJobRequest) null;
        using (TaskResourceComponent rc = requestContext.CreateComponent<TaskResourceComponent>())
          request = await rc.GetAgentRequestForAgentCloudRequestAsync(agentCloudId, agentCloudRequestId);
        AgentRequestJob agentRequestJob = (AgentRequestJob) null;
        if (request != null)
          agentRequestJob = await request.GetAgentRequestJobAsync(requestContext.Elevate());
        cloudRequestJobAsync = agentRequestJob != null ? agentRequestJob : throw new TaskAgentJobNotFoundException(TaskResources.AgentRequestNotFound((object) agentCloudRequestId));
      }
      return cloudRequestJobAsync;
    }

    IServerPoolProvider IInternalAgentCloudService.GetPoolProviderForAgentCloud(
      IVssRequestContext requestContext,
      TaskAgentCloud agentCloud,
      TaskAgentPoolData agentPool)
    {
      if (agentCloud == null)
        return (IServerPoolProvider) this.m_privatePoolProvider;
      agentCloud.LoadSharedSecret(requestContext);
      return agentCloud.Internal.GetValueOrDefault() ? this.m_remoteProviderFactories[RemoteServerPoolProviderType.Internal](agentCloud, agentPool) : this.m_remoteProviderFactories[RemoteServerPoolProviderType.Remote](agentCloud, agentPool);
    }

    IServerPoolProvider IInternalAgentCloudService.GetPoolProviderForPool(
      IVssRequestContext requestContext,
      TaskAgentPoolData agentPool)
    {
      requestContext = requestContext.ToPoolRequestContext();
      using (new MethodScope(requestContext, nameof (AgentCloudService), "GetPoolProviderForPool"))
      {
        TaskAgentCloud agentCloud = (TaskAgentCloud) null;
        if (agentPool.Pool.AgentCloudId.HasValue)
          agentCloud = this.GetAgentCloudInternal(requestContext, agentPool.Pool.AgentCloudId.Value, true, false);
        return ((IInternalAgentCloudService) this).GetPoolProviderForAgentCloud(requestContext, agentCloud, agentPool);
      }
    }

    async Task<IServerPoolProvider> IInternalAgentCloudService.GetPoolProviderForPoolAsync(
      IVssRequestContext requestContext,
      TaskAgentPoolData agentPool)
    {
      return await ((IInternalAgentCloudService) this).GetPoolProviderForPoolAsync(requestContext, agentPool, true);
    }

    async Task<IServerPoolProvider> IInternalAgentCloudService.GetPoolProviderForPoolAsync(
      IVssRequestContext requestContext,
      TaskAgentPoolData agentPool,
      bool includeSharedSecret,
      bool updateCache)
    {
      AgentCloudService agentCloudService1 = this;
      requestContext.AssertAsyncExecutionEnabled();
      requestContext = requestContext.ToPoolRequestContext();
      IServerPoolProvider providerForAgentCloud;
      using (new MethodScope(requestContext, nameof (AgentCloudService), "GetPoolProviderForPoolAsync"))
      {
        TaskAgentCloud agentCloud = (TaskAgentCloud) null;
        TaskAgentPoolData taskAgentPoolData = agentPool;
        int? agentCloudId1;
        int num1;
        if (taskAgentPoolData == null)
        {
          num1 = 0;
        }
        else
        {
          TaskAgentPool pool = taskAgentPoolData.Pool;
          if (pool == null)
          {
            num1 = 0;
          }
          else
          {
            agentCloudId1 = pool.AgentCloudId;
            num1 = agentCloudId1.HasValue ? 1 : 0;
          }
        }
        if (num1 != 0)
        {
          AgentCloudService agentCloudService2 = agentCloudService1;
          IVssRequestContext requestContext1 = requestContext;
          agentCloudId1 = agentPool.Pool.AgentCloudId;
          int agentCloudId2 = agentCloudId1.Value;
          int num2 = includeSharedSecret ? 1 : 0;
          int num3 = updateCache ? 1 : 0;
          // ISSUE: explicit non-virtual call
          agentCloud = await __nonvirtual (agentCloudService2.GetAgentCloudInternalAsync(requestContext1, agentCloudId2, num2 != 0, num3 != 0));
        }
        providerForAgentCloud = ((IInternalAgentCloudService) agentCloudService1).GetPoolProviderForAgentCloud(requestContext, agentCloud, agentPool);
      }
      return providerForAgentCloud;
    }

    IServerPoolProvider IInternalAgentCloudService.GetPrivatePoolProvider() => this.m_privatePoolProvider.Clone() as IServerPoolProvider;

    public void SetServiceIdentityPermissions(IVssRequestContext requestContext, Microsoft.VisualStudio.Services.Identity.Identity identity)
    {
      using (new MethodScope(requestContext, nameof (AgentCloudService), nameof (SetServiceIdentityPermissions)))
        this.Security.GrantListenPermissionToAgentClouds(requestContext, identity);
    }

    public bool HasAgentCloudListenPermission(
      IVssRequestContext requestContext,
      int agentCloudId,
      Guid requestId)
    {
      requestContext = requestContext.ToPoolRequestContext();
      using (new MethodScope(requestContext, nameof (AgentCloudService), nameof (HasAgentCloudListenPermission)))
        return this.Security.HasAgentCloudPermission(requestContext, agentCloudId, requestId, 8);
    }

    public async Task<TaskAgentCloud> UpdateAgentCloudAsync(
      IVssRequestContext requestContext,
      TaskAgentCloud toUpdate)
    {
      requestContext.AssertAsyncExecutionEnabled();
      requestContext = requestContext.ToPoolRequestContext();
      TaskAgentCloud taskAgentCloud;
      using (new MethodScope(requestContext, "ResourceService", nameof (UpdateAgentCloudAsync)))
        taskAgentCloud = await this.UpdateAgentCloudAndInvalidateCachesAsync(requestContext, toUpdate);
      return taskAgentCloud;
    }

    public async Task<TaskAgentCloud> UpdateAgentCloudAsync(
      IVssRequestContext requestContext,
      int agentCloudId,
      TaskAgentCloud toUpdate)
    {
      requestContext.AssertAsyncExecutionEnabled();
      requestContext = requestContext.ToPoolRequestContext();
      using (new MethodScope(requestContext, "ResourceService", nameof (UpdateAgentCloudAsync)))
      {
        toUpdate.ValidatePartialCloud(requestContext);
        this.Security.CheckAgentCloudPermission(requestContext, 2);
        toUpdate.AgentCloudId = agentCloudId;
        TaskAgentCloud agentCloud = this.GetAgentCloud(requestContext, agentCloudId, true);
        if (agentCloud.Internal.GetValueOrDefault() && !requestContext.IsSystemContext)
          throw new InvalidTaskAgentCloudException(TaskResources.InvalidOperationOnInternalAgentCloud((object) agentCloud.Name));
        if (toUpdate.SharedSecret == null)
          return await this.UpdateAgentCloudAndInvalidateCachesAsync(requestContext, toUpdate);
        string sharedSecret = agentCloud.SharedSecret;
        agentCloud.SharedSecret = toUpdate.SharedSecret;
        agentCloud.SaveSharedSecret(requestContext);
        agentCloud.SharedSecret = (string) null;
        this.InvalidateInMemCaches(requestContext, agentCloud.AgentCloudId);
        return agentCloud;
      }
    }

    private async Task<TaskAgentCloud> UpdateAgentCloudAndInvalidateCachesAsync(
      IVssRequestContext requestContext,
      TaskAgentCloud toUpdate)
    {
      TaskAgentCloud taskAgentCloud;
      using (TaskResourceComponent rc = requestContext.CreateComponent<TaskResourceComponent>())
        taskAgentCloud = await rc.UpdateAgentCloudAsync(toUpdate.AgentCloudId, toUpdate.Name, toUpdate.Type, toUpdate.GetAgentDefinitionEndpoint, toUpdate.AcquireAgentEndpoint, toUpdate.GetAgentRequestStatusEndpoint, toUpdate.ReleaseAgentEndpoint, toUpdate.GetAccountParallelismEndpoint, toUpdate.MaxParallelism, toUpdate.AcquisitionTimeout, toUpdate.Internal.GetValueOrDefault());
      if (taskAgentCloud != null)
        this.InvalidateInMemCaches(requestContext, taskAgentCloud.AgentCloudId);
      return taskAgentCloud;
    }

    private void InvalidateInMemCaches(IVssRequestContext requestContext, int agentCloudId)
    {
      if (!requestContext.IsFeatureEnabled("DistributedTask.AgentCloudCacheInvalidation"))
        return;
      requestContext.GetService<ITeamFoundationSqlNotificationService>().SendNotification(requestContext, "DistributedTask", SqlNotificationEventIds.AgentCloudChanged, agentCloudId.ToString());
    }

    public async Task<TaskAgentCloudRequest> UpdateAgentCloudRequestAsync(
      IVssRequestContext requestContext,
      TaskAgentCloudRequest toUpdate)
    {
      requestContext.AssertAsyncExecutionEnabled();
      requestContext = requestContext.ToPoolRequestContext();
      TaskAgentCloudRequest agentCloudRequest1 = (TaskAgentCloudRequest) null;
      TaskAgentCloudRequest agentCloudRequest2;
      using (new MethodScope(requestContext, nameof (AgentCloudService), nameof (UpdateAgentCloudRequestAsync)))
      {
        using (TaskResourceComponent rc = requestContext.CreateComponent<TaskResourceComponent>())
          agentCloudRequest1 = await rc.UpdateAgentCloudRequestAsync(toUpdate.AgentCloudId, toUpdate.RequestId, toUpdate.AgentSpecification, toUpdate.AgentData, toUpdate.ProvisionRequestTime, toUpdate.ProvisionedTime, toUpdate.AgentConnectedTime, toUpdate.ReleaseRequestTime);
        if (agentCloudRequest1 != null && agentCloudRequest1.ReleaseRequestTime.HasValue)
          requestContext.GetService<IInternalDistributedTaskResourceService>().QueueRequestAssignmentJob(requestContext);
        agentCloudRequest2 = agentCloudRequest1;
      }
      return agentCloudRequest2;
    }

    public async Task UpdateAgentCloudRequestAsync(
      IVssRequestContext requestContext,
      int agentCloudId,
      Guid agentCloudRequestId,
      AgentRequestProvisioningResult provisioningResult)
    {
      requestContext.AssertAsyncExecutionEnabled();
      requestContext = requestContext.ToPoolRequestContext();
      MethodScope methodScope = new MethodScope(requestContext, nameof (AgentCloudService), nameof (UpdateAgentCloudRequestAsync));
      try
      {
        this.Security.CheckAgentCloudPermission(requestContext, agentCloudId, agentCloudRequestId, 4);
        TaskAgentCloudRequest agentCloudRequest = await this.GetAgentCloudRequestAsync(requestContext, agentCloudId, agentCloudRequestId);
        AgentCloudService.TraceIncomingAgentCloudEventsFromPoolProvider(requestContext, provisioningResult, agentCloudRequest, agentCloudId, agentCloudRequestId.ToString());
        if (agentCloudRequest == null)
          throw new TaskAgentCloudRequestNotFoundException(TaskResources.AgentCloudRequestNotFound((object) agentCloudId, (object) agentCloudRequestId));
        if (agentCloudRequest.ReleaseRequestTime.HasValue)
          throw new TaskAgentCloudRequestAlreadyCompleteException(TaskResources.AgentCloudRequestAlreadyCompleted((object) agentCloudId, (object) agentCloudRequestId));
        AgentRequestMessage resultMessage = provisioningResult.ResultMessage;
        provisioningResult.ResultMessage = (AgentRequestMessage) null;
        AgentProvisionedEvent agentEvent1 = new AgentProvisionedEvent(agentCloudId, agentCloudRequestId, agentCloudRequest.Pool.Id, agentCloudRequest.Agent.Id, provisioningResult);
        bool eventDelivered = await this.DeliverEventAsync(requestContext, (RunAgentEvent) agentEvent1, true);
        if (eventDelivered && provisioningResult.ProvisioningResult == RequestProvisioningResult.Failure)
        {
          DeprovisionEvent agentEvent2 = new DeprovisionEvent(agentCloudId, agentCloudRequestId, agentCloudRequest.Pool.Id, agentCloudRequest.Agent.Id, TaskResources.RequestWasCancelledByRemoteProvider());
          int num = await this.DeliverEventAsync(requestContext, (RunAgentEvent) agentEvent2, true) ? 1 : 0;
        }
        TaskAgentJobRequest request = (TaskAgentJobRequest) null;
        using (TaskResourceComponent rc = requestContext.CreateComponent<TaskResourceComponent>())
          request = await rc.GetAgentRequestForAgentCloudRequestAsync(agentCloudId, agentCloudRequestId);
        if (request != null)
        {
          if (provisioningResult.ProvisioningResult == RequestProvisioningResult.Failure)
            AgentCloudService.PropagateFailureReasonToCallingOrchestration(requestContext, request, agentCloudId, resultMessage?.Message);
          if (resultMessage != null)
            await request.AddMessageAsync(requestContext.Elevate(), resultMessage);
          if (!eventDelivered)
          {
            agentCloudRequest.ProvisionedTime = !provisioningResult.ProvisioningFinishTime.HasValue || !(provisioningResult.ProvisioningFinishTime.Value != DateTime.MinValue) ? new DateTime?(DateTime.UtcNow) : provisioningResult.ProvisioningFinishTime;
            TaskAgentCloudRequest agentCloudRequest1 = await this.UpdateAgentCloudRequestAsync(requestContext, agentCloudRequest);
            if (provisioningResult.ProvisioningResult == RequestProvisioningResult.Failure)
            {
              await request.AddIssueAsync(requestContext.Elevate(), IssueType.Error, TaskResources.JobCanceledByPoolProvider());
              IDistributedTaskResourceService service = requestContext.GetService<IDistributedTaskResourceService>();
              IVssRequestContext requestContext1 = requestContext.Elevate();
              int poolId = request.PoolId;
              long requestId = request.RequestId;
              DateTime? nullable1 = new DateTime?(DateTime.UtcNow);
              TaskResult? nullable2 = new TaskResult?(TaskResult.Canceled);
              DateTime? expirationTime = new DateTime?();
              DateTime? startTime = new DateTime?();
              DateTime? finishTime = nullable1;
              TaskResult? result = nullable2;
              TaskAgentJobRequest taskAgentJobRequest = await service.UpdateAgentRequestAsync(requestContext1, poolId, requestId, expirationTime, startTime, finishTime, result);
            }
          }
        }
        agentCloudRequest = (TaskAgentCloudRequest) null;
        resultMessage = (AgentRequestMessage) null;
        request = (TaskAgentJobRequest) null;
      }
      finally
      {
        methodScope.Dispose();
      }
      methodScope = new MethodScope();
    }

    private static void TraceIncomingAgentCloudEventsFromPoolProvider(
      IVssRequestContext requestContext,
      AgentRequestProvisioningResult provisioningResult,
      TaskAgentCloudRequest cloudRequest,
      int agentCloudId,
      string orchestrationIdForTheTrace)
    {
      if (!requestContext.IsFeatureEnabled("DistributedTask.TraceExternalAgentProvisioning"))
        return;
      try
      {
        string orchestrationId = requestContext.OrchestrationId;
        using (requestContext.CreateOrchestrationIdScope(orchestrationIdForTheTrace))
        {
          TraceLevel traceLevel = TraceLevel.Info;
          DateTime? nullable1;
          if (cloudRequest != null)
          {
            nullable1 = cloudRequest.ReleaseRequestTime;
            if (!nullable1.HasValue)
            {
              if (provisioningResult.ProvisioningResult == RequestProvisioningResult.Failure)
              {
                traceLevel = TraceLevel.Warning;
                goto label_7;
              }
              else
                goto label_7;
            }
          }
          traceLevel = TraceLevel.Error;
label_7:
          IVssRequestContext requestContext1 = requestContext;
          int level = (int) traceLevel;
          int provisioningResult1 = (int) provisioningResult.ProvisioningResult;
          AgentRequestMessage resultMessage = provisioningResult.ResultMessage;
          DateTime? provisioningFinishTime = provisioningResult.ProvisioningFinishTime;
          int num = agentCloudId;
          int? id = cloudRequest?.Pool?.Id;
          string name = cloudRequest?.Pool?.Name;
          DateTime? nullable2;
          if (cloudRequest == null)
          {
            nullable1 = new DateTime?();
            nullable2 = nullable1;
          }
          else
            nullable2 = cloudRequest.ProvisionRequestTime;
          DateTime? nullable3;
          if (cloudRequest == null)
          {
            nullable1 = new DateTime?();
            nullable3 = nullable1;
          }
          else
            nullable3 = cloudRequest.ProvisionedTime;
          DateTime? nullable4;
          if (cloudRequest == null)
          {
            nullable1 = new DateTime?();
            nullable4 = nullable1;
          }
          else
            nullable4 = cloudRequest.AgentConnectedTime;
          DateTime? nullable5;
          if (cloudRequest == null)
          {
            nullable1 = new DateTime?();
            nullable5 = nullable1;
          }
          else
            nullable5 = cloudRequest.ReleaseRequestTime;
          string authenticationMechanism = requestContext.GetAuthenticationMechanism();
          string str = orchestrationId;
          string format = new
          {
            Note = "ADO received AgentCloud Orchestration event from PoolProvider!",
            ProvisioningResult = ((RequestProvisioningResult) provisioningResult1),
            ResultMessage = resultMessage,
            ProvisioningFinishTime = provisioningFinishTime,
            AgentCloudId = num,
            PoolId = id,
            PoolName = name,
            ProvisionRequestTime = nullable2,
            ProvisionedTime = nullable3,
            AgentConnectedTime = nullable4,
            ReleaseRequestTime = nullable5,
            AuthenticationMechanism = authenticationMechanism,
            OrchestrationIdJwtClaim = str
          }.Serialize();
          object[] objArray = Array.Empty<object>();
          requestContext1.TraceAlways(10015244, (TraceLevel) level, "DistributedTask", nameof (AgentCloudService), format, objArray);
        }
      }
      catch (Exception ex)
      {
        requestContext.TraceError(10015244, nameof (AgentCloudService), "Failed to create trace message. Error: " + ex.Message);
      }
    }

    private static void PropagateFailureReasonToCallingOrchestration(
      IVssRequestContext requestContext,
      TaskAgentJobRequest request,
      int agentCloudId,
      string failureMsg)
    {
      if (!requestContext.IsFeatureEnabled("DistributedTask.TraceExternalAgentProvisioning"))
        return;
      try
      {
        using (requestContext.CreateOrchestrationIdScope(request.OrchestrationId))
          requestContext.TraceError(10015246, nameof (AgentCloudService), string.Format("External PoolProvider (AgentCloudId:{0}) has reported Failure for the AgentRequest {1}. Message: {2}", (object) agentCloudId, (object) request.RequestId, (object) failureMsg));
      }
      catch (Exception ex)
      {
        requestContext.TraceException(10015246, nameof (AgentCloudService), ex);
      }
    }

    private void EnsureAgentCloudHubExists(IVssRequestContext requestContext)
    {
      if (this.m_agentCloudOrchestrationHubExists)
        return;
      try
      {
        OrchestrationHubDescription description = new OrchestrationHubDescription()
        {
          CompressionSettings = new CompressionSettings()
          {
            Style = CompressionStyle.Threshold,
            ThresholdInBytes = 32768
          },
          HubName = "AgentCloud",
          HubType = "AgentCloud"
        };
        requestContext.GetService<OrchestrationService>().CreateHub(requestContext, description);
      }
      catch (OrchestrationHubExistsException ex)
      {
        requestContext.TraceWarning(nameof (AgentCloudService), "Could not create Task Hub with name AgentCloud as it already exists!");
      }
      this.m_agentCloudOrchestrationHubExists = true;
    }

    private IList<TaskAgentCloud> FilterAgentClouds(
      IVssRequestContext requestContext,
      IList<TaskAgentCloud> allAgentClouds)
    {
      using (new MethodScope(requestContext, nameof (AgentCloudService), nameof (FilterAgentClouds)))
      {
        List<TaskAgentCloud> taskAgentCloudList = new List<TaskAgentCloud>();
        foreach (TaskAgentCloud allAgentCloud in (IEnumerable<TaskAgentCloud>) allAgentClouds)
        {
          if (this.Security.HasAgentCloudPermission(requestContext, allAgentCloud.AgentCloudId, 1, true))
            taskAgentCloudList.Add(allAgentCloud);
        }
        return (IList<TaskAgentCloud>) taskAgentCloudList;
      }
    }

    private void OnAgentRequestSettingsChanged(
      IVssRequestContext requestContext,
      RegistryEntryCollection changedEntries)
    {
      RegistryEntryCollection registryEntryCollection = requestContext.GetService<IVssRegistryService>().ReadEntries(requestContext, (RegistryQuery) "/Service/DistributedTask/Settings/AgentRequest/**");
      this.m_privatePoolProvider.InitialLeaseTimeout = registryEntryCollection.GetValueFromPath<TimeSpan>("/Service/DistributedTask/Settings/AgentRequest/InitialLeaseTimeout", TimeSpan.FromMinutes(10.0));
      this.m_privatePoolProvider.ProvisioningLeaseTimeout = this.m_privatePoolProvider.InitialLeaseTimeout;
      this.m_privatePoolProvider.LeaseRenewalTimeout = registryEntryCollection.GetValueFromPath<TimeSpan>("/Service/DistributedTask/Settings/AgentRequest/LeaseRenewalTimeout", TimeSpan.FromMinutes(10.0));
    }

    private void QueueAgentCloudRequestOrchestrationMonitorJob(IVssRequestContext requestContext) => requestContext.GetService<ITeamFoundationJobService>().QueueDelayedJobs(requestContext, (IEnumerable<Guid>) new Guid[1]
    {
      TaskConstants.AgentCloudRequestOrchestrationMonitorJob
    }, (int) TimeSpan.FromMinutes(15.0).TotalSeconds);
  }
}
