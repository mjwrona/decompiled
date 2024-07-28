// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.DistributedTaskResourceService
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.Azure.Pipelines.PoolProvider.Contracts;
using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.DistributedTask.Core.Server.Plugins;
using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server;
using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.DataAccess;
using Microsoft.TeamFoundation.DistributedTask.Pipelines;
using Microsoft.TeamFoundation.DistributedTask.Server.Constants;
using Microsoft.TeamFoundation.DistributedTask.Server.DataAccess;
using Microsoft.TeamFoundation.DistributedTask.Tasks;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.Threading;
using Microsoft.TeamFoundation.Framework.Server.Tracing;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.DelegatedAuthorization;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.Location;
using Microsoft.VisualStudio.Services.Location.Server;
using Microsoft.VisualStudio.Services.MachineManagement.WebApi;
using Microsoft.VisualStudio.Services.OAuth;
using Microsoft.VisualStudio.Services.Orchestration.Server;
using Microsoft.VisualStudio.Services.Organization;
using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Microsoft.TeamFoundation.DistributedTask.Server
{
  internal class DistributedTaskResourceService : 
    IInternalDistributedTaskResourceService,
    IDistributedTaskResourceService,
    IVssFrameworkService
  {
    private readonly ISecurityProvider m_security;
    private IAgentPoolSecurityProvider m_automationPoolSecurity;
    private IAgentPoolSecurityProvider m_deploymentPoolSecurity;
    private readonly IJobEventSender EventSender;
    private readonly IAgentAssignmentNotificationSender m_assignmentNotificationSender;
    private bool m_messageQueueStarted;
    private DistributedTaskResourceService.AgentRequestSettings m_agentRequestSettings;
    private ITaskAgentExtension m_hostedAgentExtension = (ITaskAgentExtension) new HostedTaskAgentExtension();
    private ITaskAgentExtension m_defaultAgentExtension = (ITaskAgentExtension) new DefaultTaskAgentExtension();
    private ITaskAgentPoolExtension m_deploymentPoolExtension = (ITaskAgentPoolExtension) new DeploymentPoolExtension();
    private ITaskAgentPoolExtension m_defaultPoolExtension = (ITaskAgentPoolExtension) new DefaultPoolExtension();
    private const string c_assignmentBatchSizePath = "/Service/DistributedTask/Settings/AgentRequest/AssignmentBatchSize";
    private const string c_assignmentNotificationTimeoutPath = "/Service/DistributedTask/Settings/AgentRequest/AssignmentNotificationTimeout";
    private const string c_unassignedRequestTimeoutPath = "/Service/DistributedTask/Settings/AgentRequest/UnassignedRequestTimeout";
    private const string c_unassignedRequestBatchSizePath = "/Service/DistributedTask/Settings/AgentRequest/UnassignedRequestBatchSize";
    private const string c_hostedLeaseTimeoutPath = "/Service/DistributedTask/Settings/AgentRequest/HostedLeaseTimeout";
    private const string c_defaultLeaseTimeoutPath = "/Service/DistributedTask/Settings/AgentRequest/DefaultLeaseTimeout";
    private const string c_completedRequestTimeoutPath = "/Service/DistributedTask/Settings/AgentRequest/CompletedRequestTimeout";
    private const string c_agentSessionInfoKey = "MS.TF.DistributedTask.SessionInfo";
    private const string c_poolCacheMissesItemKey = "MS.TF.DistributedTask.PoolCacheMisses";
    private const string c_queueCacheMissesItemKey = "MS.TF.DistributedTask.QueueCacheMisses";
    private static readonly Version s_legacyXPlatAgentVersion = new Version("1.0.0");
    private static readonly Version s_legacyXPlatAgentVersion2 = new Version("1.999.0");
    private static readonly PackageVersion s_coreAgentEncryptionVersion = new PackageVersion("2.102.0");
    private static readonly PackageVersion s_coreAgentMaintenanceVersion = new PackageVersion("2.114.0");
    private static readonly PackageVersion s_coreAgentSelfUpdateVersion = new PackageVersion("2.103.0");
    private const string c_autoAuthorizeProperty = "System.AutoAuthorize";
    private static readonly string[] s_autoAuthorizePropertyFilters = new string[1]
    {
      "System.AutoAuthorize"
    };
    private static readonly int s_legacyWindowsAgentMajorVersion = 1;
    private const int c_defaultTop = 200;

    internal DistributedTaskResourceService()
      : this((ISecurityProvider) new DefaultSecurityProvider(), (IAgentPoolSecurityProvider) new AutomationPoolSecurityProvider(), (IAgentPoolSecurityProvider) new DeploymentPoolSecurityProvider(), (IJobEventSender) new DefaultJobEventSender(), (IAgentAssignmentNotificationSender) new AsyncAgentAssignmentNotifcationSender())
    {
    }

    public DistributedTaskResourceService(
      ISecurityProvider security,
      IAgentPoolSecurityProvider automationPoolSecurity,
      IAgentPoolSecurityProvider deploymentPoolSecurity,
      IJobEventSender eventSender,
      IAgentAssignmentNotificationSender assignmentNotificationSender)
    {
      this.m_security = security;
      this.m_automationPoolSecurity = automationPoolSecurity;
      this.m_deploymentPoolSecurity = deploymentPoolSecurity;
      this.EventSender = eventSender;
      this.m_assignmentNotificationSender = assignmentNotificationSender;
    }

    public ISecurityProvider Security => this.m_security;

    public async Task<AbandonedAgentRequestsResult> AbandonExpiredAgentRequestsAsync(
      IVssRequestContext requestContext,
      int poolId)
    {
      DistributedTaskResourceService taskResourceService = this;
      requestContext = requestContext.ToPoolRequestContext();
      taskResourceService.EnsureMessageQueueStarted(requestContext);
      List<TaskAgentJobRequest> abandonedRequests = new List<TaskAgentJobRequest>();
      AbandonedAgentRequestsResult agentRequestsResult;
      using (new MethodScope(requestContext, "ResourceService", nameof (AbandonExpiredAgentRequestsAsync)))
      {
        TaskAgentPoolData pool = (await taskResourceService.GetTaskAgentPoolInternalAsync(requestContext, (IList<int>) new int[1]
        {
          poolId
        })).FirstOrDefault<TaskAgentPoolData>();
        IInternalAgentCloudService agentCloudService = requestContext.GetService<IInternalAgentCloudService>();
        IServerPoolProvider currentPoolProvider = await agentCloudService.GetPoolProviderForPoolAsync(requestContext, pool, false, false);
        TaskResourceComponent rc = requestContext.CreateComponent<TaskResourceComponent>();
        ExpiredAgentRequestsResult expiredRequests;
        TaskAgentRequestQueryResult agentRequestsAsync;
        try
        {
          expiredRequests = await rc.GetExpiredAgentRequestsAsync(poolId);
          agentRequestsAsync = await rc.GetAgentRequestsAsync(poolId, 200, new DateTime?(), new long?(), new DateTime?());
        }
        finally
        {
          rc?.Dispose();
        }
        rc = (TaskResourceComponent) null;
        if (agentRequestsAsync.RunningRequests.Count > 0)
        {
          HashSet<long> longSet = new HashSet<long>(expiredRequests.ExpiredRequests.Select<TaskAgentJobRequest, long>((Func<TaskAgentJobRequest, long>) (r => r.RequestId)));
          foreach (TaskAgentJobRequest runningRequest in (IEnumerable<TaskAgentJobRequest>) agentRequestsAsync.RunningRequests)
          {
            DateTime? finishTime = runningRequest.FinishTime;
            if (finishTime.HasValue && runningRequest.Result.HasValue)
            {
              DateTime utcNow = DateTime.UtcNow;
              finishTime = runningRequest.FinishTime;
              TimeSpan? nullable = finishTime.HasValue ? new TimeSpan?(utcNow - finishTime.GetValueOrDefault()) : new TimeSpan?();
              TimeSpan completedRequestTimeout = taskResourceService.m_agentRequestSettings.CompletedRequestTimeout;
              if ((nullable.HasValue ? (nullable.GetValueOrDefault() > completedRequestTimeout ? 1 : 0) : 0) != 0 && !longSet.Contains(runningRequest.RequestId))
                expiredRequests.ExpiredRequests.Add(runningRequest);
            }
          }
        }
        List<TaskAgentJobRequest> requestsNeverReceived = new List<TaskAgentJobRequest>();
        foreach (TaskAgentJobRequest expiredRequest in (IEnumerable<TaskAgentJobRequest>) expiredRequests.ExpiredRequests)
        {
          RequestFinishedEvent orchestrationEvent = (RequestFinishedEvent) null;
          TaskAgentCloud completedRequestCloud = (TaskAgentCloud) null;
          TaskAgentJobRequest request = (TaskAgentJobRequest) null;
          TaskAgentJobRequest completedRequest = (TaskAgentJobRequest) null;
          using (requestContext.CreateOrchestrationIdScope(expiredRequest.OrchestrationId))
          {
            if (expiredRequest.FinishTime.HasValue && expiredRequest.Result.HasValue)
            {
              rc = requestContext.CreateComponent<TaskResourceComponent>();
              try
              {
                FinishAgentRequestResult agentRequestResult = await rc.FinishAgentRequestAsync(poolId, expiredRequest.RequestId, currentPoolProvider.SingleJobPerRequest, false, expiredRequest.Result);
                completedRequest = agentRequestResult.CompletedRequest ?? expiredRequest;
                completedRequestCloud = agentRequestResult.CompletedRequestCloud;
                orchestrationEvent = agentRequestResult.OrchestrationEvent;
              }
              finally
              {
                rc?.Dispose();
              }
              rc = (TaskResourceComponent) null;
              if (completedRequest != null)
                requestContext.TraceError(10015015, "ResourceService", "Deleted request {0} with expiration of {1} and finish time of {2} for agent {3} ({4}) on pool {5}", (object) completedRequest.RequestId, (object) expiredRequest.LockedUntil, (object) completedRequest.FinishTime, (object) completedRequest.ReservedAgent.Name, (object) completedRequest.ReservedAgent.Id, (object) poolId);
            }
            else
            {
              rc = requestContext.CreateComponent<TaskResourceComponent>();
              try
              {
                TaskAgentJobRequest abandonResult = rc.AbandonAgentRequest(poolId, expiredRequest.RequestId, expiredRequest.LockedUntil.Value);
                if (abandonResult != null)
                {
                  TaskResult? result = abandonResult.Result;
                  TaskResult taskResult = TaskResult.Abandoned;
                  if (result.GetValueOrDefault() == taskResult & result.HasValue)
                  {
                    FinishAgentRequestResult agentRequestResult = await rc.FinishAgentRequestAsync(poolId, abandonResult.RequestId, currentPoolProvider.SingleJobPerRequest, false, abandonResult.Result);
                    request = completedRequest = agentRequestResult.CompletedRequest ?? abandonResult;
                    completedRequestCloud = agentRequestResult.CompletedRequestCloud;
                    orchestrationEvent = agentRequestResult.OrchestrationEvent;
                  }
                }
                abandonResult = (TaskAgentJobRequest) null;
              }
              finally
              {
                rc?.Dispose();
              }
              rc = (TaskResourceComponent) null;
              if (request != null)
              {
                abandonedRequests.Add(request.PopulateReferenceLinks(requestContext, poolId));
                if (request.ReservedAgent != null)
                {
                  if (request.ReceiveTime.HasValue)
                  {
                    requestContext.TraceError(10015009, "ResourceService", "Abandoned job {0} with expiration of {1} for agent {2} ({3}) on pool {4} (agent stopped renewing lock)", (object) request.RequestId, (object) expiredRequest.LockedUntil, (object) request.ReservedAgent.Name, (object) request.ReservedAgent.Id, (object) poolId);
                  }
                  else
                  {
                    requestsNeverReceived.Add(request);
                    requestContext.TraceError(10015010, "ResourceService", "Abandoned job {0} with expiration of {1} from agent {2} ({3}) on pool {4} (agent never started the job)", (object) request.RequestId, (object) expiredRequest.LockedUntil, (object) request.ReservedAgent.Name, (object) request.ReservedAgent.Id, (object) poolId);
                  }
                }
              }
            }
            bool delivered = false;
            if (orchestrationEvent != null)
            {
              delivered = await agentCloudService.DeliverEventAsync(requestContext, (RunAgentEvent) orchestrationEvent, true);
              taskResourceService.TraceAgentCloudOrchestrationEventsFromSprocs(requestContext, "prc_FinishAgentRequest", (RunAgentEvent) orchestrationEvent, delivered, expiredRequest);
            }
            if (completedRequest != null)
            {
              KPIHelper.PublishDTJobCompleted(requestContext);
              if (requestContext.ExecutionEnvironment.IsHostedDeployment)
              {
                string projectName = (string) null;
                if (completedRequest.ScopeId != Guid.Empty)
                  requestContext.GetService<IProjectService>().TryGetProjectName(requestContext.Elevate(), completedRequest.ScopeId, out projectName);
                DistributedTaskEventSource.Log.PublishAgentPoolRequestHistory(requestContext.ServiceHost.InstanceId, pool.Pool.Name, projectName, completedRequest, completedRequestCloud);
              }
              try
              {
                JobCompletedEvent eventData = new JobCompletedEvent(completedRequest.RequestId, completedRequest.JobId, completedRequest.Result.Value);
                taskResourceService.RaiseEvent<JobCompletedEvent>(requestContext, completedRequest.ServiceOwner, completedRequest.HostId, completedRequest.ScopeId, completedRequest.PlanType, completedRequest.PlanId, eventData);
              }
              catch (TaskOrchestrationPlanNotFoundException ex)
              {
                requestContext.TraceError(10015132, "ResourceService", "Plan {0} not found when sending JobCompleted event for Job {1} running on agent {2} ({3}) on pool {4}", (object) completedRequest.PlanId, (object) completedRequest.RequestId, (object) completedRequest.ReservedAgent.Name, (object) completedRequest.ReservedAgent.Id, (object) poolId);
              }
              ITaskAgentExtension taskAgentExtension = taskResourceService.GetTaskAgentExtension(requestContext, poolId);
              IVssRequestContext requestContext1 = requestContext;
              int poolId1 = poolId;
              TaskAgentJobRequest jobRequest = completedRequest;
              TaskAgentCloud taskAgentCloud = completedRequestCloud;
              int num;
              if (taskAgentCloud == null)
              {
                num = 0;
              }
              else
              {
                int agentCloudId = taskAgentCloud.AgentCloudId;
                num = 1;
              }
              taskAgentExtension.JobCompleted(requestContext1, poolId1, jobRequest, num != 0);
              taskResourceService.GetTaskAgentPoolExtension(requestContext, poolId).AgentRequestCompleted(requestContext, poolId, completedRequest.PopulateReferenceLinks(requestContext, poolId));
              if (!delivered)
              {
                if (completedRequest.ReservedAgent != null)
                {
                  if (string.Equals(completedRequest.ReservedAgent.ProvisioningState, "Deprovisioning"))
                  {
                    IServerPoolProvider poolProvider = currentPoolProvider;
                    int? agentCloudId1 = poolProvider.AgentCloudId;
                    int? agentCloudId2 = completedRequestCloud?.AgentCloudId;
                    if (!(agentCloudId1.GetValueOrDefault() == agentCloudId2.GetValueOrDefault() & agentCloudId1.HasValue == agentCloudId2.HasValue))
                      poolProvider = agentCloudService.GetPoolProviderForAgentCloud(requestContext, completedRequestCloud, pool);
                    taskResourceService.QueueAgentDeprovision(requestContext, poolProvider, poolId, completedRequest.ReservedAgent);
                  }
                }
              }
            }
          }
          orchestrationEvent = (RequestFinishedEvent) null;
          completedRequestCloud = (TaskAgentCloud) null;
          completedRequest = (TaskAgentJobRequest) null;
        }
        foreach (TaskAgentJobRequest taskAgentJobRequest in requestsNeverReceived)
        {
          string queueName = MessageQueueHelpers.GetQueueName(poolId, taskAgentJobRequest.ReservedAgent.Id);
          TaskAgent taskAgent;
          using (TaskResourceComponent component = requestContext.CreateComponent<TaskResourceComponent>())
            taskAgent = component.GetAgentsById(poolId, (IEnumerable<int>) new int[1]
            {
              taskAgentJobRequest.ReservedAgent.Id
            }).FirstOrDefault<TaskAgent>();
          DateTime lastConnectedOn;
          if (taskAgent != null && requestContext.GetService<ITeamFoundationMessageQueueService>().GetQueueConnectionStatus(requestContext, queueName, out lastConnectedOn) == MessageQueueStatus.Offline && taskAgent.Status == TaskAgentStatus.Online)
          {
            requestContext.TraceError(10015011, "ResourceService", "Agent {0} ({1}) for pool {2} reports online but the message queue has been offline since {3}", (object) taskAgent.Name, (object) taskAgent.Id, (object) poolId, (object) lastConnectedOn);
            if (lastConnectedOn < DateTime.UtcNow.AddMinutes(-3.0))
            {
              AgentConnectivityResult connectivityResult;
              using (TaskResourceComponent component = requestContext.CreateComponent<TaskResourceComponent>())
              {
                requestContext.TraceInfo("ResourceService", "Pool {0}: Setting agent {1} offline forcibly due to unresponsiveness", (object) poolId, (object) taskAgent.Id);
                connectivityResult = component.SetAgentOffline(poolId, taskAgent.Id, force: true);
              }
              if (connectivityResult.HandledEvent)
              {
                taskResourceService.GetTaskAgentPoolExtension(requestContext, poolId).AgentDisconnected(requestContext, poolId, taskAgent.Id);
                if (connectivityResult.Agent != null && connectivityResult.PoolData != null)
                {
                  taskResourceService.GetTaskAgentExtension(requestContext, poolId).FilterCapabilities(requestContext, poolId, connectivityResult.Agent);
                  requestContext.GetService<IDistributedTaskEventPublisherService>().NotifyAgentChangeEvent(requestContext, "MS.TF.DistributedTask.AgentUpdated", connectivityResult.PoolData.Pool, connectivityResult.Agent);
                }
              }
            }
          }
        }
        agentRequestsResult = new AbandonedAgentRequestsResult(expiredRequests.ActiveRequestsExist, (IList<TaskAgentJobRequest>) abandonedRequests);
      }
      abandonedRequests = (List<TaskAgentJobRequest>) null;
      return agentRequestsResult;
    }

    public TaskAgent AddAgent(IVssRequestContext requestContext, int poolId, TaskAgent agent)
    {
      requestContext = requestContext.ToPoolRequestContext();
      this.EnsureMessageQueueStarted(requestContext);
      using (new MethodScope(requestContext, "ResourceService", nameof (AddAgent)))
      {
        ArgumentValidation.CheckAgent(agent, nameof (agent));
        this.CheckViewAndOtherPermissionsForPool(requestContext, poolId, 2);
        this.CheckHostedPoolPermissions(requestContext, poolId, agent);
        this.CheckAgentCloudPoolPermissions(requestContext, poolId, agent);
        this.CheckForSupportedAgentVersion(requestContext, poolId, agent.Version);
        this.ClearAgentCapabilitiesIfHosted(requestContext, poolId, agent);
        this.PopulateAgentAccessMapping(requestContext, (TaskAgentReference) agent);
        TaskAgentPoolData taskAgentPoolData = this.GetTaskAgentPoolInternal(requestContext, (IList<int>) new int[1]
        {
          poolId
        }).First<TaskAgentPoolData>();
        bool createEnabled = taskAgentPoolData.Pool.IsHosted || taskAgentPoolData.Pool.AgentCloudId.HasValue;
        CreateAgentResult createAgentResult;
        using (TaskResourceComponent component = requestContext.CreateComponent<TaskResourceComponent>())
        {
          createAgentResult = component.AddAgent(poolId, agent, createEnabled);
          createAgentResult.Agent.Properties = agent.Properties;
        }
        this.GetTaskAgentExtension(requestContext, poolId).FilterCapabilities(requestContext, poolId, createAgentResult.Agent);
        if (createAgentResult.Agent.Properties != null && createAgentResult.Agent.Properties.Count > 0)
          requestContext.GetService<ITeamFoundationPropertyService>().SetProperties(requestContext, createAgentResult.Agent.CreateSpec(), createAgentResult.Agent.Properties.Convert());
        if (agent.Authorization != null && agent.Authorization.PublicKey != null)
        {
          IVssRequestContext context = requestContext.To(TeamFoundationHostType.Application);
          IVssRequestContext vssRequestContext = requestContext.Elevate();
          Microsoft.VisualStudio.Services.Identity.Identity identity = this.ProvisionServiceIdentity(requestContext, createAgentResult.PoolData, AgentPoolServiceAccountRoles.AgentPoolService);
          Microsoft.VisualStudio.Services.DelegatedAuthorization.Registration registration1 = new Microsoft.VisualStudio.Services.DelegatedAuthorization.Registration()
          {
            ClientType = ClientType.MediumTrust,
            IdentityId = identity.Id,
            IsValid = true,
            PublicKey = agent.Authorization.PublicKey.ToXmlString(),
            RegistrationId = createAgentResult.Agent.Authorization.ClientId,
            RegistrationName = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Task Agent Pool {0} - Agent {1}", (object) poolId, (object) createAgentResult.Agent.Id),
            Scopes = "vso.agentpools_listen"
          };
          Microsoft.VisualStudio.Services.DelegatedAuthorization.Registration registration2 = context.GetService<IDelegatedAuthorizationRegistrationService>().Create(context.Elevate(), registration1);
          if (registration2 != null)
          {
            vssRequestContext.GetService<IDelegatedAuthorizationService>().AuthorizeHost(vssRequestContext, registration2.RegistrationId);
            ILocationDataProvider locationData = requestContext.GetService<ILocationService>().GetLocationData(requestContext, new Guid("585028FE-17D8-49E2-9A1B-EFB4D8502156"));
            createAgentResult.Agent.Authorization.AuthorizationUrl = locationData.GetResourceUri(requestContext, "oauth2", OAuth2ResourceIds.Token, (object) null, false);
          }
        }
        createAgentResult.Agent.PopulateReferenceLinks<TaskAgent>(requestContext, poolId);
        TaskAgent agent1 = createAgentResult.Agent.Clone();
        createAgentResult.Agent.PopulateProperties(requestContext, createAgentResult.PoolData);
        string queueName = MessageQueueHelpers.GetQueueName(poolId, createAgentResult.Agent.Id);
        ITeamFoundationMessageQueueService service = requestContext.GetService<ITeamFoundationMessageQueueService>();
        if (service.QueueExists(requestContext, queueName))
          service.DeleteQueue(requestContext, queueName);
        service.CreateQueue(requestContext, queueName, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Message queue for distributed task agent {0} in pool {1}", (object) createAgentResult.Agent.Name, (object) poolId));
        this.GetTaskAgentPoolExtension(requestContext, poolId).AgentAdded(requestContext, poolId, agent1);
        TaskAgentPool pool = createAgentResult.PoolData.Pool;
        requestContext.GetService<IDistributedTaskEventPublisherService>().NotifyAgentChangeEvent(requestContext, "MS.TF.DistributedTask.AgentAdded", pool, agent1);
        this.InValidatePoolCache(requestContext, pool.Id);
        if (!createEnabled)
          DistributedTaskResourceService.QueueAgentRematchJob(requestContext);
        return createAgentResult.Agent;
      }
    }

    public TaskAgentPool AddAgentPool(
      IVssRequestContext requestContext,
      TaskAgentPool pool,
      Stream poolMetadata = null)
    {
      requestContext = requestContext.ToPoolRequestContext();
      this.EnsureMessageQueueStarted(requestContext);
      using (new MethodScope(requestContext, "ResourceService", nameof (AddAgentPool)))
      {
        ArgumentValidation.CheckPool(pool, nameof (pool));
        pool.Validate();
        this.GetAgentPoolSecurity(requestContext, pool.PoolType).CheckPoolPermission(requestContext, 2);
        this.CheckHostedPoolPermissions(requestContext, pool);
        IdentityService service1 = requestContext.GetService<IdentityService>();
        Microsoft.VisualStudio.Services.Identity.Identity createdBy = requestContext.GetUserIdentity();
        if (pool.CreatedBy != null)
        {
          Microsoft.VisualStudio.Services.Identity.Identity identity = service1.GetIdentity(requestContext, pool.CreatedBy);
          if (identity != null)
            createdBy = identity;
        }
        Guid? nullable1 = new Guid?();
        if (pool.Owner != null)
        {
          Microsoft.VisualStudio.Services.Identity.Identity identity = service1.GetIdentity(requestContext, pool.Owner);
          if (identity != null)
            nullable1 = new Guid?(identity.Id);
        }
        int? nullable2 = new int?();
        ITeamFoundationFileService service2 = requestContext.GetService<ITeamFoundationFileService>();
        if (poolMetadata != null)
          nullable2 = new int?(service2.UploadFile(requestContext, poolMetadata, OwnerId.DistributedTask, Guid.Empty));
        ITaskAgentPoolExtension agentPoolExtension = this.GetTaskAgentPoolExtension(pool.PoolType);
        TaskAgentPool taskAgentPool = pool;
        bool? autoProvision = pool.AutoProvision;
        bool? nullable3 = new bool?(((int) autoProvision ?? (agentPoolExtension.DefaultAutoProvision ? 1 : 0)) != 0);
        taskAgentPool.AutoProvision = nullable3;
        int? nullable4 = pool.AgentCloudId;
        if (nullable4.HasValue)
        {
          nullable4 = pool.TargetSize;
          if (!nullable4.HasValue)
            pool.TargetSize = new int?(1);
        }
        pool.Options = new TaskAgentPoolOptions?(pool.Options.GetValueOrDefault());
        if (pool.IsHosted)
        {
          nullable4 = pool.AgentCloudId;
          if (!nullable4.HasValue)
            requestContext.TraceError(10015180, "ResourceService", "Creating hosted pool without AgentCloudId poolName=" + pool.Name + ", stackTrace=" + new StackTrace().ToString());
        }
        TaskAgentPoolData taskAgentPoolData;
        try
        {
          using (TaskResourceComponent component = requestContext.CreateComponent<TaskResourceComponent>())
          {
            TaskResourceComponent resourceComponent = component;
            string name = pool.Name;
            Guid id = createdBy.Id;
            int num1 = pool.IsHosted ? 1 : 0;
            autoProvision = pool.AutoProvision;
            int num2 = autoProvision.Value ? 1 : 0;
            bool? autoSize = pool.AutoSize;
            int poolType = (int) pool.PoolType;
            int? poolMetadataFileId = nullable2;
            Guid? ownerId = nullable1;
            int? agentCloudId = pool.AgentCloudId;
            int? targetSize = pool.TargetSize;
            bool? isLegacy = pool.IsLegacy;
            int options = (int) pool.Options.Value;
            taskAgentPoolData = resourceComponent.AddAgentPool(name, id, num1 != 0, num2 != 0, autoSize, (TaskAgentPoolType) poolType, poolMetadataFileId, ownerId, agentCloudId, targetSize, isLegacy, (TaskAgentPoolOptions) options);
            taskAgentPoolData.Pool.Properties = pool.Properties;
          }
        }
        catch (Exception ex)
        {
          requestContext.TraceException("ResourceService", ex);
          if (nullable2.HasValue)
            service2.DeleteFile(requestContext, (long) nullable2.Value);
          throw;
        }
        if (pool.AgentCloudId.HasValue)
          DistributedTaskResourceService.ResizeAgentCloudPool(requestContext, taskAgentPoolData.Pool);
        if (taskAgentPoolData.Pool.Properties != null && taskAgentPoolData.Pool.Properties.Count > 0)
          requestContext.GetService<ITeamFoundationPropertyService>().SetProperties(requestContext, taskAgentPoolData.Pool.CreateSpec(), taskAgentPoolData.Pool.Properties.Convert());
        if (pool.IsHosted)
          DistributedTaskHostedPoolHelper.SetCurrentPoolSize(requestContext, -1);
        taskAgentPoolData.Pool = this.ProvisionAgentPoolRoles(requestContext, taskAgentPoolData.Pool, createdBy);
        taskAgentPoolData.Pool = DistributedTaskResourceService.PopulateIdentityReferences(requestContext, taskAgentPoolData.Pool);
        requestContext.GetService<IDistributedTaskEventPublisherService>().NotifyAgentPoolEvent(requestContext, "MS.TF.DistributedTask.AgentPoolCreated", taskAgentPoolData.Pool);
        ITeamFoundationEventService service3 = requestContext.GetService<ITeamFoundationEventService>();
        requestContext.TraceInfo("ResourceService", "DistributedTask", (object) string.Format("Publishing AddAgentPoolEvent notification for pool {0}", (object) taskAgentPoolData.Pool.Id));
        service3.SyncPublishNotification(requestContext, (object) new AddAgentPoolEvent(taskAgentPoolData.Pool));
        requestContext.GetService<TaskAgentPoolCacheService>().Set(requestContext, taskAgentPoolData.Pool.Id, taskAgentPoolData.Clone());
        return taskAgentPoolData.Pool;
      }
    }

    internal static void ResizeAgentCloudPool(IVssRequestContext requestContext, TaskAgentPool pool)
    {
      try
      {
        requestContext.GetService<ITeamFoundationJobService>().QueueJobsNow(requestContext, (IEnumerable<Guid>) new Guid[1]
        {
          TaskConstants.AgentCloudPoolResizeJob
        });
      }
      catch (JobDefinitionNotFoundException ex)
      {
        DistributedTaskAgentCloudPoolsHelper.ResizeAgentCloudPools(requestContext, pool.Name);
      }
    }

    public TaskAgentQueue AddAgentQueue(
      IVssRequestContext requestContext,
      Guid projectId,
      TaskAgentQueue queue,
      bool authorizePipelines = false)
    {
      Microsoft.VisualStudio.Services.Identity.Identity userIdentity = requestContext.GetUserIdentity();
      return this.AddAgentQueue(requestContext, projectId, queue, userIdentity, authorizePipelines);
    }

    public TaskAgentQueue AddAgentQueue(
      IVssRequestContext requestContext,
      Guid projectId,
      TaskAgentQueue queue,
      Microsoft.VisualStudio.Services.Identity.Identity queueCreator,
      bool authorizePipelines)
    {
      IVssRequestContext requestContext1 = requestContext.Elevate();
      IdentityService service = requestContext.GetService<IdentityService>();
      Microsoft.VisualStudio.Services.Identity.Identity contributorsGroup = service.GetGroups(requestContext1, projectId, (IList<string>) new string[1]
      {
        TaskResources.ProjectContributorsGroupName()
      }).FirstOrDefault<Microsoft.VisualStudio.Services.Identity.Identity>();
      Microsoft.VisualStudio.Services.Identity.Identity projectValidUsers = service.GetGroups(requestContext, projectId, (IEnumerable<IdentityDescriptor>) new IdentityDescriptor[1]
      {
        GroupWellKnownIdentityDescriptors.EveryoneGroup
      }).FirstOrDefault<Microsoft.VisualStudio.Services.Identity.Identity>();
      return this.AddAgentQueue(requestContext, projectId, queue, queueCreator, authorizePipelines, contributorsGroup, projectValidUsers);
    }

    private TaskAgentQueue AddAgentQueue(
      IVssRequestContext requestContext,
      Guid projectId,
      TaskAgentQueue queue,
      Microsoft.VisualStudio.Services.Identity.Identity queueCreator,
      bool authorizePipelines,
      Microsoft.VisualStudio.Services.Identity.Identity contributorsGroup,
      Microsoft.VisualStudio.Services.Identity.Identity projectValidUsers)
    {
      using (new MethodScope(requestContext, "ResourceService", nameof (AddAgentQueue)))
      {
        ArgumentValidation.CheckQueue(queue, nameof (queue));
        this.Security.CheckQueuePermission(requestContext, projectId, 32);
        bool flag = requestContext.IsFeatureEnabled("DistributedTask.EnableGrantOrgLevelAccessPermissionToAllPipelinesInAgentPools");
        IVssRequestContext poolRequestContext = requestContext.ToPoolRequestContext();
        TaskAgentPool pool;
        if (queue.Pool != null)
        {
          pool = this.GetAgentPool(poolRequestContext, queue.Pool.Id, flag ? (IList<string>) DistributedTaskResourceService.s_autoAuthorizePropertyFilters : (IList<string>) (string[]) null, TaskAgentPoolActionFilter.None);
          if (pool == null)
            throw new TaskAgentPoolNotFoundException(TaskResources.AgentPoolNotFound((object) queue.Pool.Id));
          if (pool.PoolType != TaskAgentPoolType.Automation)
            throw new TaskAgentPoolTypeMismatchException(TaskResources.QueuesShouldUseAutomationPools());
          this.CheckViewAndOtherPermissionsForPool(poolRequestContext, queue.Pool.Id, 16, new int?(2));
        }
        else
        {
          pool = this.GetAgentPools(poolRequestContext.Elevate(), queue.Name, (IList<string>) null, TaskAgentPoolType.Automation, TaskAgentPoolActionFilter.None).FirstOrDefault<TaskAgentPool>();
          if (pool != null)
          {
            this.CheckViewAndOtherPermissionsForPool(poolRequestContext, pool.Id, 2);
            queue.Pool = pool.AsReference();
          }
          else
          {
            pool = this.AddAgentPool(poolRequestContext.Elevate(), new TaskAgentPool(queue.Name)
            {
              AutoProvision = new bool?(false),
              CreatedBy = queueCreator.ToIdentityRef(poolRequestContext)
            }, (Stream) null);
            queue.Pool = pool.AsReference();
          }
        }
        if (flag && !authorizePipelines && pool != null)
          pool.Properties.TryGetValue<bool>("System.AutoAuthorize", out authorizePipelines);
        using (TaskResourceComponent component = requestContext.CreateComponent<TaskResourceComponent>())
          queue = component.AddAgentQueue(projectId, queue.Name, queue.Pool.Id).Queue;
        List<AccessControlEntry> accessControlEntryList = new List<AccessControlEntry>();
        if (queueCreator != null)
          accessControlEntryList.Add(new AccessControlEntry()
          {
            Descriptor = queueCreator.Descriptor,
            Allow = 27
          });
        IVssRequestContext vssRequestContext = requestContext.Elevate();
        requestContext.GetService<IdentityService>();
        if (vssRequestContext.ExecutionEnvironment.IsHostedDeployment && queue.Pool != null && pool != null && pool.IsHosted)
        {
          if (contributorsGroup != null)
          {
            accessControlEntryList.Add(new AccessControlEntry()
            {
              Descriptor = contributorsGroup.Descriptor,
              Allow = 17
            });
            requestContext.TraceVerbose("ResourceService", "Adding contributors group to hosted queue users role for queue {0} on project {1})", (object) queue.Id, (object) projectId);
          }
          else
            requestContext.TraceVerbose("ResourceService", "Couldn't find contributors group on project: {0}", (object) projectId);
        }
        if (accessControlEntryList.Count > 0)
          requestContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(requestContext, DefaultSecurityProvider.NamespaceId).SetAccessControlEntries(requestContext.Elevate(), DefaultSecurityProvider.GetAgentQueueToken(requestContext, projectId, queue.Id), (IEnumerable<IAccessControlEntry>) accessControlEntryList, true);
        if (queue.Pool != null)
        {
          if (projectValidUsers != null)
            this.GetAgentPoolSecurity(requestContext, TaskAgentPoolType.Automation).GrantReadPermissionToPool(poolRequestContext, queue.Pool.Id, projectValidUsers);
          else
            requestContext.TraceError(10015055, "ResourceService", "Cannot find Project everyone group on the project {0}, Queue: {1}, Pool: {2}", (object) projectId, (object) queue.Id, (object) queue.Pool.Id);
          if (requestContext.IsFeatureEnabled("DistributedTask.ReturnTaskAgentPoolObjDuringQueueCreation"))
            queue.Pool = (TaskAgentPoolReference) pool;
        }
        requestContext.GetService<TaskAgentQueueCacheService>().Set(projectId, queue.Id, queue);
        IDistributedTaskEventPublisherService service = requestContext.GetService<IDistributedTaskEventPublisherService>();
        service.NotifyAgentQueueEvent(requestContext, "MS.TF.DistributedTask.AgentQueueCreated", queue);
        if (authorizePipelines)
          service.NotifyAgentQueueEvent(requestContext, "MS.TF.DistributedTask.AuthorizePipelines", queue);
        return queue;
      }
    }

    public async Task<IList<TaskAgentJobRequest>> CancelAgentRequestsForPoolAsync(
      IVssRequestContext requestContext,
      int poolId,
      string reason)
    {
      DistributedTaskResourceService taskResourceService = this;
      requestContext.AssertAsyncExecutionEnabled();
      requestContext = requestContext.ToPoolRequestContext();
      taskResourceService.EnsureMessageQueueStarted(requestContext);
      IList<TaskAgentJobRequest> taskAgentJobRequestList;
      using (new MethodScope(requestContext, "ResourceService", nameof (CancelAgentRequestsForPoolAsync)))
      {
        taskResourceService.CheckViewAndOtherPermissionsForPool(requestContext, poolId, 2);
        TaskAgentPool pool = ((await taskResourceService.GetTaskAgentPoolInternalAsync(requestContext, (IList<int>) new int[1]
        {
          poolId
        })).FirstOrDefault<TaskAgentPoolData>() ?? throw new TaskAgentPoolNotFoundException(TaskResources.AgentPoolNotFound((object) poolId))).Pool;
        List<TaskAgentJobRequest> canceledRequests = new List<TaskAgentJobRequest>();
        foreach (TaskAgentJobRequest taskAgentJobRequest in (IEnumerable<TaskAgentJobRequest>) await taskResourceService.GetAgentRequestsForAgentsAsync(requestContext, pool.Id, (IList<int>) new List<int>(), 0))
        {
          if (!taskAgentJobRequest.FinishTime.HasValue)
          {
            requestContext.TraceInfo("ResourceService", string.Format("Canceling request {0} for pool {1} via JobCanceledEvent.", (object) taskAgentJobRequest.RequestId, (object) pool.Id));
            JobCanceledEvent eventData = new JobCanceledEvent(taskAgentJobRequest.JobId, reason);
            taskResourceService.RaiseEvent<JobCanceledEvent>(requestContext, taskAgentJobRequest.ServiceOwner, taskAgentJobRequest.HostId, taskAgentJobRequest.ScopeId, taskAgentJobRequest.PlanType, taskAgentJobRequest.PlanId, eventData);
            canceledRequests.Add(taskAgentJobRequest);
          }
        }
        requestContext.TraceInfo("ResourceService", string.Format("{0} cancelations queued for pool {1}, '{2}'", (object) canceledRequests.Count, (object) pool.Id, (object) pool.Name));
        taskAgentJobRequestList = (IList<TaskAgentJobRequest>) canceledRequests;
      }
      return taskAgentJobRequestList;
    }

    public async Task<int> CleanupUnassignableRequestsAsync(IVssRequestContext requestContext)
    {
      requestContext = requestContext.ToPoolRequestContext();
      int count;
      using (new MethodScope(requestContext, "ResourceService", nameof (CleanupUnassignableRequestsAsync)))
      {
        IList<TaskAgentJobRequest> requests;
        using (TaskResourceComponent rc = requestContext.CreateComponent<TaskResourceComponent>())
          requests = await rc.GetUnassigableAgentRequestsAsync(DateTime.UtcNow.Subtract(this.m_agentRequestSettings.UnassignedRequestTimeout), this.m_agentRequestSettings.UnassignedRequestTimeoutBatchSize);
        foreach (TaskAgentJobRequest request in (IEnumerable<TaskAgentJobRequest>) requests)
        {
          using (requestContext.CreateOrchestrationIdScope(request.OrchestrationId))
          {
            if (requestContext.IsFeatureEnabled("DistributedTask.FixedUnassignableAgentRequestCleanup"))
            {
              try
              {
                await request.AddIssueAsync(requestContext, IssueType.Error, TaskResources.AgentRequestFailedDueToExpiration((object) this.m_agentRequestSettings.UnassignedRequestTimeout.GetPrettyString()));
                await this.FinishAgentRequestAsync(requestContext, request.PoolId, request.RequestId, new TaskResult?(TaskResult.Failed), callSprocEvenIfPoolNotFound: true);
                requestContext.TraceAlways(10015267, TraceLevel.Warning, "DistributedTask", "ResourceService", new
                {
                  Note = "The AgentJobRequest has been determined to be unassignable. Result := 'Failed'.",
                  RequestId = request.RequestId,
                  PoolId = request.PoolId,
                  AgentSpecification = request.AgentSpecification?.ToString(),
                  Demands = request.Demands,
                  PlanType = request.PlanType,
                  OwnerName = request.Owner?.Name,
                  QueueTime = request.QueueTime
                }.Serialize());
              }
              catch (Exception ex)
              {
                requestContext.TraceException(10015265, "ResourceService", ex);
              }
            }
            else
            {
              await request.AddIssueAsync(requestContext, IssueType.Error, TaskResources.AgentRequestFailedDueToExpiration((object) this.m_agentRequestSettings.UnassignedRequestTimeout.GetPrettyString()));
              await this.FinishAgentRequestAsync(requestContext, request.PoolId, request.RequestId, new TaskResult?(TaskResult.Failed));
            }
          }
        }
        count = requests.Count;
      }
      return count;
    }

    public int GetUnassignedRequestTimeoutBatchSize() => this.m_agentRequestSettings.UnassignedRequestTimeoutBatchSize;

    public void DeleteAgentPool(IVssRequestContext requestContext, int poolId)
    {
      requestContext = requestContext.ToPoolRequestContext();
      this.EnsureMessageQueueStarted(requestContext);
      using (new MethodScope(requestContext, "ResourceService", nameof (DeleteAgentPool)))
      {
        this.CheckViewAndOtherPermissionsForPool(requestContext, poolId, 2);
        this.CheckHostedPoolPermissions(requestContext, poolId);
        this.GetTaskAgentPoolExtension(requestContext, poolId).CheckIfPoolCanBeDeleted(requestContext.Elevate(), poolId);
        DeleteAgentPoolResult deleteAgentPoolResult = (DeleteAgentPoolResult) null;
        using (TaskResourceComponent component = requestContext.CreateComponent<TaskResourceComponent>())
          deleteAgentPoolResult = component.DeleteAgentPool(poolId);
        if (deleteAgentPoolResult.PoolData == null)
          return;
        requestContext.GetService<TaskAgentSessionCacheService>().Remove(requestContext, (IEnumerable<TaskAgentSessionData>) deleteAgentPoolResult.DeletedSessions);
        ITeamFoundationMessageQueueService service1 = requestContext.GetService<ITeamFoundationMessageQueueService>();
        foreach (TaskAgent deletedAgent in (IEnumerable<TaskAgent>) deleteAgentPoolResult.DeletedAgents)
        {
          try
          {
            service1.DeleteQueue(requestContext, MessageQueueHelpers.GetQueueName(poolId, deletedAgent.Id));
          }
          catch (Exception ex)
          {
            requestContext.TraceException(10015003, "ResourceService", ex);
          }
        }
        if (deleteAgentPoolResult.DeletedPoolMaintenanceJobs != null && deleteAgentPoolResult.DeletedPoolMaintenanceJobs.Count > 0)
          this.GetPoolMaintenanceTaskHub(requestContext).DeletePlans(requestContext.Elevate(), Guid.Empty, deleteAgentPoolResult.DeletedPoolMaintenanceJobs.Select<TaskAgentPoolMaintenanceJob, Guid>((Func<TaskAgentPoolMaintenanceJob, Guid>) (x => x.OrchestrationId)));
        if (deleteAgentPoolResult.DeletedPoolMaintenanceDefinitions != null && deleteAgentPoolResult.DeletedPoolMaintenanceDefinitions.Count > 0)
          this.DeleteScheduleMaintenanceJob(requestContext, deleteAgentPoolResult.DeletedPoolMaintenanceDefinitions);
        if (deleteAgentPoolResult.PoolData.ServiceIdentityId.HasValue)
        {
          try
          {
            requestContext.GetService<IAccessControlService>().DeleteServiceIdentity(requestContext.Elevate(), deleteAgentPoolResult.PoolData.ServiceIdentityId.Value);
          }
          catch (Exception ex)
          {
            requestContext.TraceException(10015001, "ResourceService", ex);
          }
        }
        try
        {
          this.GetAgentPoolSecurity(requestContext, deleteAgentPoolResult.PoolData.Pool.PoolType).RemoveAccessControlLists(requestContext, deleteAgentPoolResult.PoolData.Pool);
        }
        catch (Exception ex)
        {
          requestContext.TraceException(10015013, "ResourceService", ex);
        }
        try
        {
          requestContext.GetService<ITeamFoundationPropertyService>().DeleteArtifacts(requestContext, (IEnumerable<ArtifactSpec>) new ArtifactSpec[1]
          {
            deleteAgentPoolResult.PoolData.Pool.CreateSpec()
          });
        }
        catch (Exception ex)
        {
          requestContext.TraceException(10015012, "ResourceService", ex);
        }
        requestContext.GetService<TaskAgentPoolCacheService>().Remove(requestContext, poolId);
        int? poolMetadataFileId = deleteAgentPoolResult.PoolData.PoolMetadataFileId;
        if (poolMetadataFileId.HasValue)
        {
          ITeamFoundationFileService service2 = requestContext.GetService<ITeamFoundationFileService>();
          IVssRequestContext requestContext1 = requestContext;
          poolMetadataFileId = deleteAgentPoolResult.PoolData.PoolMetadataFileId;
          long fileId = (long) poolMetadataFileId.Value;
          service2.DeleteFile(requestContext1, fileId);
        }
        foreach (DeprovisioningAgentResult deprovisioningAgent in (IEnumerable<DeprovisioningAgentResult>) deleteAgentPoolResult.DeprovisioningAgents)
        {
          if (deprovisioningAgent.ProvisionedByAgentCloud != null)
          {
            IServerPoolProvider providerForAgentCloud = requestContext.GetService<IInternalAgentCloudService>().GetPoolProviderForAgentCloud(requestContext, deprovisioningAgent.ProvisionedByAgentCloud, deleteAgentPoolResult.PoolData);
            this.QueueAgentDeprovision(requestContext, providerForAgentCloud, poolId, (TaskAgentReference) deprovisioningAgent, true);
          }
        }
        requestContext.GetService<IDistributedTaskEventPublisherService>().NotifyAgentPoolEvent(requestContext, "MS.TF.DistributedTask.AgentPoolDeleted", deleteAgentPoolResult.PoolData.Pool);
        requestContext.TraceInfo("ResourceService", "DistributedTask", (object) string.Format("Publishing DeleteAgentPool notification for pool {0}", (object) poolId));
        requestContext.GetService<ITeamFoundationEventService>().SyncPublishNotification(requestContext, (object) new DeleteAgentPoolEvent(poolId));
      }
    }

    public void DeleteAgentQueue(IVssRequestContext requestContext, Guid projectId, int queueId)
    {
      using (new MethodScope(requestContext, "ResourceService", nameof (DeleteAgentQueue)))
      {
        if (this.GetAgentQueue(requestContext.Elevate(), projectId, queueId, TaskAgentQueueActionFilter.None) == null)
          throw new TaskAgentQueueNotFoundException(TaskResources.QueueNotFound((object) queueId));
        this.CheckViewAndOtherPermissionsForQueue(requestContext, projectId, queueId, 2);
        DeleteAgentQueueResult agentQueueResult;
        using (TaskResourceComponent component = requestContext.CreateComponent<TaskResourceComponent>())
          agentQueueResult = component.DeleteAgentQueue(projectId, queueId);
        if (agentQueueResult.Queue == null)
          return;
        this.CleanupQueueRolesAndSecurity(requestContext, projectId, agentQueueResult.Queue, agentQueueResult.PoolUnreferenced);
        requestContext.GetService<TaskAgentQueueCacheService>().Remove(projectId, queueId);
        requestContext.GetService<IDistributedTaskEventPublisherService>().NotifyAgentQueueEvent(requestContext, "MS.TF.DistributedTask.AgentQueueDeleted", agentQueueResult.Queue);
      }
    }

    public TaskAgentSession CreateSession(
      IVssRequestContext requestContext,
      int poolId,
      TaskAgentSession session)
    {
      requestContext = requestContext.ToPoolRequestContext();
      this.EnsureMessageQueueStarted(requestContext);
      using (new MethodScope(requestContext, "ResourceService", nameof (CreateSession)))
      {
        ArgumentValidation.CheckAgentSession(session, nameof (session));
        this.CheckViewAndOtherPermissionsForAgent(requestContext, poolId, new int?(session.Agent.Id), new long?(), 4);
        this.CheckForSupportedAgentVersion(requestContext, poolId, session.Agent.Version);
        this.PopulateAgentAccessMapping(requestContext, session.Agent);
        ITeamFoundationMessageQueueService service1 = requestContext.GetService<ITeamFoundationMessageQueueService>();
        CreateAgentSessionResult agentSession;
        try
        {
          using (TaskResourceComponent component = requestContext.CreateComponent<TaskResourceComponent>())
            agentSession = component.CreateAgentSession(poolId, session.Agent, session.OwnerName, session.SystemCapabilities, false);
        }
        catch (TaskAgentSessionConflictException ex)
        {
          DateTime lastConnectedOn = DateTime.MinValue;
          string queueName = MessageQueueHelpers.GetQueueName(poolId, session.Agent.Id);
          int connectionStatus = (int) service1.GetQueueConnectionStatus(requestContext, queueName, out lastConnectedOn);
          TimeSpan timeSpan = DateTime.UtcNow - lastConnectedOn;
          if (connectionStatus == 1 && timeSpan.TotalMinutes > 3.0)
          {
            TaskAgent taskAgent = (TaskAgent) null;
            using (TaskResourceComponent component = requestContext.CreateComponent<TaskResourceComponent>())
              taskAgent = component.GetAgentsById(poolId, (IEnumerable<int>) new int[1]
              {
                session.Agent.Id
              }).FirstOrDefault<TaskAgent>();
            if (taskAgent != null && taskAgent.Status == TaskAgentStatus.Online)
            {
              using (TaskResourceComponent component = requestContext.CreateComponent<TaskResourceComponent>())
              {
                requestContext.TraceInfo("ResourceService", "Pool {0}: Setting agent {1} offline forcibly due to session conflict", (object) poolId, (object) session.Agent.Id);
                component.SetAgentOffline(poolId, session.Agent.Id, force: true);
              }
            }
          }
          throw;
        }
        TaskAgentSessionCacheService service2 = requestContext.GetService<TaskAgentSessionCacheService>();
        if (agentSession.OldSession != null)
        {
          requestContext.TraceInfo("ResourceService", "Pool {0}: Deleted session {1} for agent {2}", (object) agentSession.OldSession.PoolId, (object) agentSession.OldSession.SessionId, (object) agentSession.OldSession.Agent.Id);
          service2.Remove(requestContext, agentSession.OldSession.SessionId);
        }
        session.SessionId = agentSession.NewSession.SessionId;
        service2.Set(requestContext, agentSession.NewSession.SessionId, agentSession.NewSession);
        requestContext.TraceAlways(10015211, "ResourceService", "Pool {0}: Created session {1} for agent {2}", (object) agentSession.NewSession.PoolId, (object) agentSession.NewSession.SessionId, (object) agentSession.NewSession.Agent.Id);
        service1.SetQueueOffline(requestContext, agentSession.NewSession.QueueName);
        if (agentSession.Agent.PendingUpdate != null)
        {
          TaskResult result;
          string currentState;
          if (new PackageVersion(agentSession.Agent.Version).Equals(agentSession.Agent.PendingUpdate.TargetVersion))
          {
            result = TaskResult.Succeeded;
            currentState = string.Format("Agent updated to {0}", (object) agentSession.Agent.PendingUpdate.TargetVersion);
          }
          else
          {
            result = TaskResult.Failed;
            currentState = "Agent still in to " + agentSession.Agent.Version;
          }
          this.FinishAgentUpdate(requestContext, poolId, session.Agent.Id, result, currentState, agentSession.Agent.PendingUpdate);
        }
        PackageVersion packageVersion = new PackageVersion(agentSession.NewSession.Agent.Version);
        if (agentSession.RecalculateRequestMatches)
          DistributedTaskResourceService.QueueAgentRematchJob(requestContext);
        byte[] numArray = DistributedTaskResourceService.GetAgentEncryptionKey(requestContext, poolId, session.Agent.Id, true);
        PackageVersion encryptionVersion = DistributedTaskResourceService.s_coreAgentEncryptionVersion;
        if (packageVersion.CompareTo(encryptionVersion) >= 0)
        {
          bool flag = false;
          if (agentSession.Agent != null && agentSession.Agent.Authorization != null && agentSession.Agent.Authorization.PublicKey != null)
          {
            flag = true;
            numArray = agentSession.Agent.Authorization.PublicKey.Encrypt(numArray);
          }
          session.EncryptionKey = new TaskAgentSessionKey()
          {
            Encrypted = flag,
            Value = numArray
          };
        }
        bool delivered = false;
        IInternalAgentCloudService service3 = requestContext.GetService<IInternalAgentCloudService>();
        if (agentSession.OrchestrationEvent != null)
        {
          delivered = service3.DeliverEvent(requestContext, (RunAgentEvent) agentSession.OrchestrationEvent, true);
          this.TraceAgentCloudOrchestrationEventsFromSprocs(requestContext, "prc_CreateAgentSession", (RunAgentEvent) agentSession.OrchestrationEvent, delivered, agentSession.AssignedRequest);
        }
        if (!delivered && agentSession.AssignedRequest != null)
        {
          using (requestContext.CreateOrchestrationIdScope(agentSession.AssignedRequest.OrchestrationId))
          {
            TaskAgentPoolData agentPool = this.GetTaskAgentPoolInternal(requestContext, (IList<int>) new int[1]
            {
              poolId
            }).FirstOrDefault<TaskAgentPoolData>();
            IServerPoolProvider serverPoolProvider = !agentSession.ComponentRetrunsAgentCloud ? service3.GetPoolProviderForPool(requestContext, agentPool) : service3.GetPoolProviderForAgentCloud(requestContext, agentSession.AssignedRequestAgentCloud, agentPool);
            this.QueueRequestAssignmentNotification(requestContext, serverPoolProvider, serverPoolProvider, agentSession.AssignedRequest);
          }
        }
        return session;
      }
    }

    public int DeleteAgents(
      IVssRequestContext requestContext,
      int poolId,
      IEnumerable<int> agentIds)
    {
      requestContext = requestContext.ToPoolRequestContext();
      this.EnsureMessageQueueStarted(requestContext);
      using (new MethodScope(requestContext, "ResourceService", nameof (DeleteAgents)))
      {
        this.CheckViewAndOtherPermissionsForPool(requestContext, poolId, 2);
        this.CheckHostedPoolPermissions(requestContext, poolId);
        this.CheckAgentCloudPoolPermissions(requestContext, poolId);
        DeleteAgentResult deleteAgentResult;
        using (TaskResourceComponent component = requestContext.CreateComponent<TaskResourceComponent>())
          deleteAgentResult = component.DeleteAgents(poolId, agentIds);
        requestContext.GetService<TaskAgentSessionCacheService>().Remove(requestContext, (IEnumerable<TaskAgentSessionData>) deleteAgentResult.DeletedSessions);
        foreach (TaskAgent deletedAgent in (IEnumerable<TaskAgent>) deleteAgentResult.DeletedAgents)
        {
          requestContext.GetService<ITeamFoundationMessageQueueService>().DeleteQueue(requestContext, MessageQueueHelpers.GetQueueName(poolId, deletedAgent.Id));
          if (deletedAgent.Authorization != null && deletedAgent.Authorization.ClientId != Guid.Empty && deletedAgent.Authorization.PublicKey != null)
            requestContext.GetService<IDelegatedAuthorizationRegistrationService>().Delete(requestContext.Elevate(), deletedAgent.Authorization.ClientId);
          requestContext.GetService<ITeamFoundationPropertyService>().DeleteArtifacts(requestContext.Elevate(), (IEnumerable<ArtifactSpec>) new ArtifactSpec[1]
          {
            deletedAgent.CreateSpec()
          });
          DistributedTaskResourceService.DeleteAgentEncryptionKeyStore(requestContext, poolId, deletedAgent.Id);
          this.GetTaskAgentPoolExtension(requestContext, poolId).AgentDeleted(requestContext, poolId, deletedAgent.Id);
          TaskAgentPool pool = deleteAgentResult.PoolData.Pool;
          requestContext.GetService<IDistributedTaskEventPublisherService>().NotifyAgentChangeEvent(requestContext, "MS.TF.DistributedTask.AgentDeleted", pool, deletedAgent);
        }
        foreach (DeprovisioningAgentResult deprovisioningAgent in (IEnumerable<DeprovisioningAgentResult>) deleteAgentResult.DeprovisioningAgents)
        {
          if (deprovisioningAgent.ProvisionedByAgentCloud != null)
          {
            IServerPoolProvider providerForAgentCloud = requestContext.GetService<IInternalAgentCloudService>().GetPoolProviderForAgentCloud(requestContext, deprovisioningAgent.ProvisionedByAgentCloud, deleteAgentResult.PoolData);
            this.QueueAgentDeprovision(requestContext, providerForAgentCloud, poolId, (TaskAgentReference) deprovisioningAgent);
          }
        }
        this.InValidatePoolCache(requestContext, poolId);
        return deleteAgentResult.DeletedAgents.Count;
      }
    }

    public async Task DeleteMessageAsync(
      IVssRequestContext requestContext,
      int poolId,
      Guid sessionId,
      long messageId)
    {
      requestContext = requestContext.ToPoolRequestContext();
      MethodScope methodScope = new MethodScope(requestContext, "ResourceService", nameof (DeleteMessageAsync));
      try
      {
        TaskAgentSessionData agentSessionData1 = this.EnsureSession(requestContext, poolId, sessionId, false);
        this.CheckViewAndOtherPermissionsForAgent(requestContext, poolId, agentSessionData1?.Agent?.Id, new long?(), 4);
        TaskAgentSessionData agentSessionData2 = this.EnsureSession(requestContext, poolId, sessionId);
        await requestContext.GetService<ITeamFoundationMessageQueueService>().DeleteMessagesAsync(requestContext, agentSessionData2.QueueName, sessionId, messageId, TimeSpan.FromSeconds(30.0));
      }
      finally
      {
        methodScope.Dispose();
      }
      methodScope = new MethodScope();
    }

    public void ClearAgentInformation(IVssRequestContext requestContext, int poolId, int agentId)
    {
      requestContext = requestContext.ToPoolRequestContext();
      using (new MethodScope(requestContext, "ResourceService", nameof (ClearAgentInformation)))
      {
        this.CheckViewAndOtherPermissionsForPool(requestContext, poolId, 8);
        TaskAgentSessionData agentSessionData = (TaskAgentSessionData) null;
        using (TaskResourceComponent component = requestContext.CreateComponent<TaskResourceComponent>())
          agentSessionData = component.DeleteAgentSession(poolId, agentId);
        if (agentSessionData != null)
        {
          requestContext.TraceInfo("ResourceService", "Pool {0}: Removed session {1} for agent {2}", (object) agentSessionData.PoolId, (object) agentSessionData.SessionId, (object) agentSessionData.Agent.Id);
          requestContext.GetService<TaskAgentSessionCacheService>().Remove(requestContext, agentSessionData.SessionId);
        }
        try
        {
          string queueName = MessageQueueHelpers.GetQueueName(poolId, agentId);
          ITeamFoundationMessageQueueService service = requestContext.GetService<ITeamFoundationMessageQueueService>();
          service.EmptyQueue(requestContext, queueName);
          service.SetQueueOffline(requestContext, queueName);
        }
        catch (Exception ex)
        {
          requestContext.TraceError("ResourceService", "Pool {0}: Failed to clear queue for agent {1}", (object) poolId, (object) agentId);
          requestContext.TraceException("ResourceService", ex);
        }
      }
    }

    public async Task ClearAgentSlotAsync(
      IVssRequestContext requestContext,
      int poolId,
      int agentId)
    {
      requestContext.AssertAsyncExecutionEnabled();
      requestContext = requestContext.ToPoolRequestContext();
      MethodScope methodScope = new MethodScope(requestContext, "ResourceService", nameof (ClearAgentSlotAsync));
      try
      {
        this.CheckViewAndOtherPermissionsForPool(requestContext, poolId, 8);
        TaskAgent agent = (TaskAgent) null;
        TaskAgentSessionData deletedSession = (TaskAgentSessionData) null;
        using (TaskResourceComponent trc = requestContext.CreateComponent<TaskResourceComponent>())
        {
          agent = (await trc.GetAgentsByIdAsync(poolId, (IEnumerable<int>) new int[1]
          {
            agentId
          }, includeAssignedRequest: true)).FirstOrDefault<TaskAgent>();
          deletedSession = await trc.DeleteAgentSessionAsync(poolId, agentId);
        }
        if (agent?.AssignedRequest != null)
        {
          TaskAgentJobRequest taskAgentJobRequest = await this.UpdateAgentRequestAsync(requestContext, poolId, agent.AssignedRequest.RequestId, finishTime: new DateTime?(DateTime.UtcNow), result: new TaskResult?(TaskResult.Canceled));
        }
        if (deletedSession != null)
        {
          requestContext.TraceInfo("ResourceService", "Pool {0}: Removed session {1} for agent {2}", (object) deletedSession.PoolId, (object) deletedSession.SessionId, (object) deletedSession.Agent.Id);
          requestContext.GetService<TaskAgentSessionCacheService>().Remove(requestContext, deletedSession.SessionId);
        }
        try
        {
          string queueName = MessageQueueHelpers.GetQueueName(poolId, agentId);
          ITeamFoundationMessageQueueService service = requestContext.GetService<ITeamFoundationMessageQueueService>();
          service.EmptyQueue(requestContext, queueName);
          service.SetQueueOffline(requestContext, queueName);
        }
        catch (Exception ex)
        {
          requestContext.TraceError("ResourceService", "Pool {0}: Failed to clear queue for agent {1}", (object) poolId, (object) agentId);
          requestContext.TraceException("ResourceService", ex);
        }
        agent = (TaskAgent) null;
        deletedSession = (TaskAgentSessionData) null;
      }
      finally
      {
        methodScope.Dispose();
      }
      methodScope = new MethodScope();
    }

    public void DeleteSession(IVssRequestContext requestContext, int poolId, Guid sessionId)
    {
      requestContext = requestContext.ToPoolRequestContext();
      using (new MethodScope(requestContext, "ResourceService", nameof (DeleteSession)))
      {
        TaskAgentSessionData agentSessionData1 = this.EnsureSession(requestContext, poolId, sessionId, false);
        this.CheckViewAndOtherPermissionsForAgent(requestContext, poolId, agentSessionData1?.Agent?.Id, new long?(), 4);
        TaskAgentSessionData agentSessionData2 = (TaskAgentSessionData) null;
        using (TaskResourceComponent component = requestContext.CreateComponent<TaskResourceComponent>())
          agentSessionData2 = component.DeleteAgentSession(poolId, sessionId);
        if (agentSessionData2 == null)
          return;
        requestContext.GetService<ITeamFoundationMessageQueueService>().SetQueueOffline(requestContext, agentSessionData2.QueueName);
        requestContext.TraceInfo("ResourceService", "Pool {0}: Removed session {1} for agent {2}", (object) agentSessionData2.PoolId, (object) agentSessionData2.SessionId, (object) agentSessionData2.Agent.Id);
        requestContext.GetService<TaskAgentSessionCacheService>().Remove(requestContext, agentSessionData2.SessionId);
      }
    }

    public TaskAgent GetAgent(
      IVssRequestContext requestContext,
      int poolId,
      int agentId,
      bool includeCapabilities = false,
      bool includeAssignedRequest = false,
      bool includeLastCompletedRequest = false,
      IList<string> propertyFilters = null)
    {
      return this.GetAgents(requestContext, poolId, (IEnumerable<int>) new int[1]
      {
        agentId
      }, (includeCapabilities ? 1 : 0) != 0, (includeAssignedRequest ? 1 : 0) != 0, (includeLastCompletedRequest ? 1 : 0) != 0, propertyFilters).FirstOrDefault<TaskAgent>();
    }

    public async Task<TaskAgent> GetAgentAsync(
      IVssRequestContext requestContext,
      int poolId,
      int agentId,
      bool includeCapabilities = false,
      bool includeAssignedRequest = false,
      bool includeLastCompletedRequest = false,
      IList<string> propertyFilters = null)
    {
      return (await this.GetAgentsAsync(requestContext, poolId, (IEnumerable<int>) new int[1]
      {
        agentId
      }, (includeCapabilities ? 1 : 0) != 0, (includeAssignedRequest ? 1 : 0) != 0, (includeLastCompletedRequest ? 1 : 0) != 0, propertyFilters)).FirstOrDefault<TaskAgent>();
    }

    public IList<TaskAgent> GetAgents(
      IVssRequestContext requestContext,
      int poolId,
      IEnumerable<int> agentIds,
      bool includeCapabilities = false,
      bool includeAssignedRequest = false,
      bool includeLastCompletedRequest = false,
      IList<string> propertyFilters = null)
    {
      requestContext = requestContext.ToPoolRequestContext();
      this.EnsureMessageQueueStarted(requestContext);
      using (new MethodScope(requestContext, "ResourceService", nameof (GetAgents)))
      {
        if (!this.GetAgentPoolSecurity(requestContext, poolId).HasPoolPermission(requestContext, poolId, 1))
          return (IList<TaskAgent>) Array.Empty<TaskAgent>();
        IList<TaskAgent> agentsById;
        using (TaskResourceComponent component = requestContext.CreateComponent<TaskResourceComponent>())
          agentsById = component.GetAgentsById(poolId, agentIds, includeCapabilities, includeAssignedRequest, includeLastCompletedRequest);
        if (includeCapabilities)
        {
          ITaskAgentExtension taskAgentExtension = this.GetTaskAgentExtension(requestContext, poolId);
          foreach (TaskAgent agent in (IEnumerable<TaskAgent>) agentsById)
            taskAgentExtension.FilterCapabilities(requestContext, poolId, agent);
        }
        if (agentsById.Count > 0 && propertyFilters != null && propertyFilters.Count > 0)
        {
          using (TeamFoundationDataReader properties = requestContext.GetService<ITeamFoundationPropertyService>().GetProperties(requestContext, agentsById.Select<TaskAgent, ArtifactSpec>((Func<TaskAgent, ArtifactSpec>) (x => x.CreateSpec())), (IEnumerable<string>) propertyFilters))
            ArtifactPropertyKinds.MatchProperties<TaskAgent>(properties, agentsById, (Func<TaskAgent, int>) (x => x.Id), (Action<TaskAgent, PropertiesCollection>) ((x, y) => x.Properties = y));
        }
        return (IList<TaskAgent>) agentsById.Select<TaskAgent, TaskAgent>((Func<TaskAgent, TaskAgent>) (x => x.PopulateReferenceLinks<TaskAgent>(requestContext, poolId))).ToList<TaskAgent>();
      }
    }

    public async Task<IList<TaskAgent>> GetAgentsAsync(
      IVssRequestContext requestContext,
      int poolId,
      IEnumerable<int> agentIds,
      bool includeCapabilities = false,
      bool includeAssignedRequest = false,
      bool includeLastCompletedRequest = false,
      IList<string> propertyFilters = null)
    {
      requestContext = requestContext.ToPoolRequestContext();
      this.EnsureMessageQueueStarted(requestContext);
      using (new MethodScope(requestContext, "ResourceService", nameof (GetAgentsAsync)))
      {
        if (!this.GetAgentPoolSecurity(requestContext, poolId).HasPoolPermission(requestContext, poolId, 1))
          return (IList<TaskAgent>) Array.Empty<TaskAgent>();
        IList<TaskAgent> agents;
        using (TaskResourceComponent thc = requestContext.CreateComponent<TaskResourceComponent>())
          agents = await thc.GetAgentsByIdAsync(poolId, agentIds, includeCapabilities, includeAssignedRequest, includeLastCompletedRequest);
        if (includeCapabilities)
        {
          ITaskAgentExtension agentExtension = this.GetTaskAgentExtension(requestContext, poolId);
          foreach (TaskAgent agent in (IEnumerable<TaskAgent>) agents)
            await agentExtension.FilterCapabilitiesAsync(requestContext, poolId, agent);
          agentExtension = (ITaskAgentExtension) null;
        }
        if (agents.Count > 0 && propertyFilters != null && propertyFilters.Count > 0)
        {
          using (TeamFoundationDataReader properties = requestContext.GetService<ITeamFoundationPropertyService>().GetProperties(requestContext, agents.Select<TaskAgent, ArtifactSpec>((Func<TaskAgent, ArtifactSpec>) (x => x.CreateSpec())), (IEnumerable<string>) propertyFilters))
            ArtifactPropertyKinds.MatchProperties<TaskAgent>(properties, agents, (Func<TaskAgent, int>) (x => x.Id), (Action<TaskAgent, PropertiesCollection>) ((x, y) => x.Properties = y));
        }
        return (IList<TaskAgent>) agents.Select<TaskAgent, TaskAgent>((Func<TaskAgent, TaskAgent>) (x => x.PopulateReferenceLinks<TaskAgent>(requestContext, poolId))).ToList<TaskAgent>();
      }
    }

    public IList<TaskAgent> GetAgents(
      IVssRequestContext requestContext,
      int poolId,
      string agentName = null,
      bool includeCapabilities = false,
      bool includeAssignedRequest = false,
      bool includeAgentCloudRequest = false,
      bool includeLastCompletedRequest = false,
      IList<string> propertyFilters = null)
    {
      requestContext = requestContext.ToPoolRequestContext();
      this.EnsureMessageQueueStarted(requestContext);
      using (new MethodScope(requestContext, "ResourceService", nameof (GetAgents)))
      {
        if (!this.GetAgentPoolSecurity(requestContext, poolId).HasPoolPermission(requestContext, poolId, 1))
          return (IList<TaskAgent>) new List<TaskAgent>(0);
        IList<TaskAgent> taskAgentList = (IList<TaskAgent>) new List<TaskAgent>();
        using (TaskResourceComponent component = requestContext.CreateComponent<TaskResourceComponent>())
          taskAgentList = component.GetAgents(poolId, agentName, includeCapabilities, includeAssignedRequest, includeAgentCloudRequest, includeLastCompletedRequest).Agents;
        if (includeCapabilities)
        {
          ITaskAgentExtension taskAgentExtension = this.GetTaskAgentExtension(requestContext, poolId);
          foreach (TaskAgent agent in (IEnumerable<TaskAgent>) taskAgentList)
            taskAgentExtension.FilterCapabilities(requestContext, poolId, agent);
        }
        if (taskAgentList.Count > 0 && propertyFilters != null && propertyFilters.Count > 0)
        {
          using (TeamFoundationDataReader properties = requestContext.GetService<ITeamFoundationPropertyService>().GetProperties(requestContext, taskAgentList.Select<TaskAgent, ArtifactSpec>((Func<TaskAgent, ArtifactSpec>) (x => x.CreateSpec())), (IEnumerable<string>) propertyFilters))
            ArtifactPropertyKinds.MatchProperties<TaskAgent>(properties, taskAgentList, (Func<TaskAgent, int>) (x => x.Id), (Action<TaskAgent, PropertiesCollection>) ((x, y) => x.Properties = y));
        }
        return (IList<TaskAgent>) taskAgentList.Select<TaskAgent, TaskAgent>((Func<TaskAgent, TaskAgent>) (x => x.PopulateReferenceLinks<TaskAgent>(requestContext, poolId))).ToList<TaskAgent>();
      }
    }

    public async Task<IList<TaskAgent>> GetAgentsAsync(
      IVssRequestContext requestContext,
      int poolId,
      string agentName = null,
      bool includeCapabilities = false,
      bool includeAssignedRequest = false,
      bool includeLastCompletedRequest = false,
      IList<string> propertyFilters = null)
    {
      requestContext.AssertAsyncExecutionEnabled();
      requestContext = requestContext.ToPoolRequestContext();
      this.EnsureMessageQueueStarted(requestContext);
      using (new MethodScope(requestContext, "ResourceService", nameof (GetAgentsAsync)))
      {
        if (!this.GetAgentPoolSecurity(requestContext, poolId).HasPoolPermission(requestContext, poolId, 1))
          return (IList<TaskAgent>) new List<TaskAgent>(0);
        IList<TaskAgent> allAgents;
        using (TaskResourceComponent resourceComponent = requestContext.CreateComponent<TaskResourceComponent>())
          allAgents = (await resourceComponent.GetAgentsAsync(poolId, agentName, includeCapabilities, includeAssignedRequest, includeLastCompletedRequest: includeLastCompletedRequest)).Agents;
        if (includeCapabilities)
        {
          ITaskAgentExtension agentExtension = this.GetTaskAgentExtension(requestContext, poolId);
          foreach (TaskAgent agent in (IEnumerable<TaskAgent>) allAgents)
          {
            await agentExtension.FilterCapabilitiesAsync(requestContext, poolId, agent);
            agent.PopulateReferenceLinks<TaskAgent>(requestContext, poolId);
          }
          agentExtension = (ITaskAgentExtension) null;
        }
        if (allAgents.Count > 0 && propertyFilters != null && propertyFilters.Count > 0)
        {
          using (TeamFoundationDataReader properties = requestContext.GetService<ITeamFoundationPropertyService>().GetProperties(requestContext, allAgents.Select<TaskAgent, ArtifactSpec>((Func<TaskAgent, ArtifactSpec>) (x => x.CreateSpec())), (IEnumerable<string>) propertyFilters))
            ArtifactPropertyKinds.MatchProperties<TaskAgent>(properties, allAgents, (Func<TaskAgent, int>) (x => x.Id), (Action<TaskAgent, PropertiesCollection>) ((x, y) => x.Properties = y));
        }
        return (IList<TaskAgent>) allAgents.Select<TaskAgent, TaskAgent>((Func<TaskAgent, TaskAgent>) (x => x.PopulateReferenceLinks<TaskAgent>(requestContext, poolId))).ToList<TaskAgent>();
      }
    }

    public async Task<IList<TaskAgent>> GetAgentsByFilterPagedAsync(
      IVssRequestContext requestContext,
      int poolId,
      Guid hostId,
      Guid projectId,
      string agentName = null,
      bool partialNameMatch = false,
      bool includeCapabilities = false,
      bool includeAssignedRequest = false,
      bool includeLastCompletedRequest = false,
      IList<int> agentIds = null,
      TaskAgentStatusFilter agentStatusFilter = TaskAgentStatusFilter.All,
      TaskAgentJobResultFilter agentJobResultFilter = TaskAgentJobResultFilter.All,
      string continuationToken = null,
      int top = 1000,
      bool? enabled = null)
    {
      requestContext.AssertAsyncExecutionEnabled();
      requestContext = requestContext.ToPoolRequestContext();
      this.EnsureMessageQueueStarted(requestContext);
      using (new MethodScope(requestContext, "ResourceService", nameof (GetAgentsByFilterPagedAsync)))
      {
        if (!this.GetAgentPoolSecurity(requestContext, poolId).HasPoolPermission(requestContext, poolId, 1))
          return (IList<TaskAgent>) new List<TaskAgent>(0);
        DistributedTaskResourceService.AgentFilter agentFilter = this.ConvertAgentFiltersForSQL(agentStatusFilter, agentJobResultFilter);
        IEnumerable<TaskAgent> allAgents;
        using (TaskResourceComponent resourceComponent = requestContext.CreateComponent<TaskResourceComponent>())
          allAgents = await resourceComponent.GetAgentsByFilterAsync(poolId, hostId, projectId, agentName, partialNameMatch, includeCapabilities, includeAssignedRequest, includeLastCompletedRequest, (IEnumerable<int>) agentIds, agentFilter.agentStatusFilter, (IEnumerable<byte>) agentFilter.agentlastJobStatusFilters, continuationToken, agentFilter.isNeverDeployedFilter, top, enabled);
        if (includeCapabilities)
        {
          ITaskAgentExtension agentExtension = this.GetTaskAgentExtension(requestContext, poolId);
          foreach (TaskAgent agent in allAgents)
          {
            await agentExtension.FilterCapabilitiesAsync(requestContext, poolId, agent);
            agent.PopulateReferenceLinks<TaskAgent>(requestContext, poolId);
          }
          agentExtension = (ITaskAgentExtension) null;
        }
        return (IList<TaskAgent>) allAgents.Select<TaskAgent, TaskAgent>((Func<TaskAgent, TaskAgent>) (x => x.PopulateReferenceLinks<TaskAgent>(requestContext, poolId))).ToList<TaskAgent>();
      }
    }

    public TaskAgentQueryResult GetAgents(
      IVssRequestContext requestContext,
      int poolId,
      IList<Demand> demands,
      IList<string> propertyFilters = null)
    {
      return this.GetAgents(requestContext, poolId, new IList<Demand>[1]
      {
        demands
      }, propertyFilters)[0];
    }

    public IList<TaskAgentQueryResult> GetAgents(
      IVssRequestContext requestContext,
      int poolId,
      IList<Demand>[] demandSets,
      IList<string> propertyFilters = null)
    {
      requestContext = requestContext.ToPoolRequestContext();
      this.EnsureMessageQueueStarted(requestContext);
      using (new MethodScope(requestContext, "ResourceService", nameof (GetAgents)))
      {
        if (demandSets == null || demandSets.Length == 0)
          return (IList<TaskAgentQueryResult>) Array.Empty<TaskAgentQueryResult>();
        if (!this.GetAgentPoolSecurity(requestContext, poolId).HasPoolPermission(requestContext, poolId, 1, true))
        {
          TaskAgentQueryResult[] agents = new TaskAgentQueryResult[demandSets.Length];
          for (int index = 0; index < demandSets.Length; ++index)
            agents[index] = new TaskAgentQueryResult((IEnumerable<Demand>) demandSets[index], new bool?());
          return (IList<TaskAgentQueryResult>) agents;
        }
        IList<string> capabilityFilters = (IList<string>) null;
        if (demandSets != null)
          capabilityFilters = (IList<string>) ((IEnumerable<IList<Demand>>) demandSets).SelectMany<IList<Demand>, Demand>((Func<IList<Demand>, IEnumerable<Demand>>) (x => (IEnumerable<Demand>) x)).Where<Demand>((Func<Demand, bool>) (x => x != null)).Select<Demand, string>((Func<Demand, string>) (x => x.Name)).Distinct<string>().ToList<string>();
        TaskAgentList agents1;
        using (TaskResourceComponent component = requestContext.CreateComponent<TaskResourceComponent>())
          agents1 = component.GetAgents(poolId, includeCapabilities: true, capabilityFilters: (IEnumerable<string>) capabilityFilters);
        ITaskAgentExtension taskAgentExtension = this.GetTaskAgentExtension(requestContext, poolId);
        foreach (TaskAgent agent in (IEnumerable<TaskAgent>) agents1.Agents)
          taskAgentExtension.FilterCapabilities(requestContext, poolId, agent);
        if (agents1.Agents.Count > 0 && propertyFilters != null && propertyFilters.Count > 0)
        {
          using (TeamFoundationDataReader properties = requestContext.GetService<TeamFoundationPropertyService>().GetProperties(requestContext, agents1.Agents.Select<TaskAgent, ArtifactSpec>((Func<TaskAgent, ArtifactSpec>) (x => x.CreateSpec())), (IEnumerable<string>) propertyFilters))
            ArtifactPropertyKinds.MatchProperties<TaskAgent>(properties, agents1.Agents, (Func<TaskAgent, int>) (x => x.Id), (Action<TaskAgent, PropertiesCollection>) ((x, y) => x.Properties = y));
        }
        TaskAgentQueryResult[] agents2 = new TaskAgentQueryResult[demandSets.Length];
        Dictionary<int, IDictionary<string, string>> capabilityCache = new Dictionary<int, IDictionary<string, string>>();
        agents1.Agents = (IList<TaskAgent>) agents1.Agents.Select<TaskAgent, TaskAgent>((Func<TaskAgent, TaskAgent>) (x => x.PopulateReferenceLinks<TaskAgent>(requestContext, poolId))).ToList<TaskAgent>();
        for (int index = 0; index < demandSets.Length; ++index)
          agents2[index] = demandSets[index] == null || demandSets[index].Count <= 0 ? new TaskAgentQueryResult((IEnumerable<Demand>) demandSets[index], (JObject) null, (IEnumerable<TaskAgent>) agents1.Agents, agents1.ReturnedAllAgentsInPool) : DistributedTaskResourceService.FilterAgents(agents1, demandSets[index], (IDictionary<int, IDictionary<string, string>>) capabilityCache);
        return (IList<TaskAgentQueryResult>) agents2;
      }
    }

    public async Task<TaskAgentQueryResult> GetAgentsAsync(
      IVssRequestContext requestContext,
      int poolId,
      IList<Demand> demands,
      IList<string> propertyFilters = null)
    {
      return (await this.GetAgentsAsync(requestContext, poolId, new IList<Demand>[1]
      {
        demands
      }, propertyFilters))[0];
    }

    public async Task<IList<TaskAgentQueryResult>> GetAgentsAsync(
      IVssRequestContext requestContext,
      int poolId,
      IList<Demand>[] demandSets,
      IList<string> propertyFilters = null)
    {
      requestContext.AssertAsyncExecutionEnabled();
      requestContext = requestContext.ToPoolRequestContext();
      this.EnsureMessageQueueStarted(requestContext);
      using (new MethodScope(requestContext, "ResourceService", nameof (GetAgentsAsync)))
      {
        if (demandSets == null || demandSets.Length == 0)
          return (IList<TaskAgentQueryResult>) Array.Empty<TaskAgentQueryResult>();
        if (!this.GetAgentPoolSecurity(requestContext, poolId).HasPoolPermission(requestContext, poolId, 1, true))
        {
          TaskAgentQueryResult[] agentsAsync = new TaskAgentQueryResult[demandSets.Length];
          for (int index = 0; index < demandSets.Length; ++index)
            agentsAsync[index] = new TaskAgentQueryResult((IEnumerable<Demand>) demandSets[index], new bool?());
          return (IList<TaskAgentQueryResult>) agentsAsync;
        }
        IList<string> capabilityFilters = (IList<string>) null;
        if (demandSets != null)
          capabilityFilters = (IList<string>) ((IEnumerable<IList<Demand>>) demandSets).SelectMany<IList<Demand>, Demand>((Func<IList<Demand>, IEnumerable<Demand>>) (x => (IEnumerable<Demand>) x)).Where<Demand>((Func<Demand, bool>) (x => x != null)).Select<Demand, string>((Func<Demand, string>) (x => x.Name)).Distinct<string>().ToList<string>();
        TaskAgentList agentList;
        using (TaskResourceComponent resourceComponent = requestContext.CreateComponent<TaskResourceComponent>())
          agentList = await resourceComponent.GetAgentsAsync(poolId, includeCapabilities: true, capabilityFilters: (IEnumerable<string>) capabilityFilters);
        ITaskAgentExtension agentExtension = this.GetTaskAgentExtension(requestContext, poolId);
        foreach (TaskAgent agent in (IEnumerable<TaskAgent>) agentList.Agents)
          await agentExtension.FilterCapabilitiesAsync(requestContext, poolId, agent);
        if (agentList.Agents.Count > 0 && propertyFilters != null && propertyFilters.Count > 0)
        {
          using (TeamFoundationDataReader properties = requestContext.GetService<TeamFoundationPropertyService>().GetProperties(requestContext, agentList.Agents.Select<TaskAgent, ArtifactSpec>((Func<TaskAgent, ArtifactSpec>) (x => x.CreateSpec())), (IEnumerable<string>) propertyFilters))
            ArtifactPropertyKinds.MatchProperties<TaskAgent>(properties, agentList.Agents, (Func<TaskAgent, int>) (x => x.Id), (Action<TaskAgent, PropertiesCollection>) ((x, y) => x.Properties = y));
        }
        TaskAgentQueryResult[] agentsAsync1 = new TaskAgentQueryResult[demandSets.Length];
        Dictionary<int, IDictionary<string, string>> capabilityCache = new Dictionary<int, IDictionary<string, string>>();
        agentList.Agents = (IList<TaskAgent>) agentList.Agents.Select<TaskAgent, TaskAgent>((Func<TaskAgent, TaskAgent>) (x => x.PopulateReferenceLinks<TaskAgent>(requestContext, poolId))).ToList<TaskAgent>();
        for (int index = 0; index < demandSets.Length; ++index)
          agentsAsync1[index] = demandSets[index] == null || demandSets[index].Count <= 0 ? new TaskAgentQueryResult((IEnumerable<Demand>) demandSets[index], (JObject) null, (IEnumerable<TaskAgent>) agentList.Agents, agentList.ReturnedAllAgentsInPool) : DistributedTaskResourceService.FilterAgents(agentList, demandSets[index], (IDictionary<int, IDictionary<string, string>>) capabilityCache);
        return (IList<TaskAgentQueryResult>) agentsAsync1;
      }
    }

    public async Task<TaskAgentQueryResult> GetAgentsAsync(
      IVssRequestContext requestContext,
      int poolId,
      JObject agentSpecification)
    {
      return (await this.GetAgentsAsync(requestContext, poolId, new JObject[1]
      {
        agentSpecification
      }))[0];
    }

    public async Task<IList<TaskAgentQueryResult>> GetAgentsAsync(
      IVssRequestContext requestContext,
      int poolId,
      JObject[] agentSpecifications)
    {
      requestContext.AssertAsyncExecutionEnabled();
      requestContext = requestContext.ToPoolRequestContext();
      this.EnsureMessageQueueStarted(requestContext);
      using (new MethodScope(requestContext, "ResourceService", nameof (GetAgentsAsync)))
      {
        if (!this.GetAgentPoolSecurity(requestContext, poolId).HasPoolPermission(requestContext, poolId, 1, true))
          return (IList<TaskAgentQueryResult>) Array.Empty<TaskAgentQueryResult>();
        IList<TaskAgent> agents = (IList<TaskAgent>) new List<TaskAgent>();
        IList<Tuple<TaskAgent, IList<Demand>>> nonProvisionedAgents = (IList<Tuple<TaskAgent, IList<Demand>>>) new List<Tuple<TaskAgent, IList<Demand>>>();
        TaskAgentList agentsAsync1;
        using (TaskResourceComponent resourceComponent = requestContext.CreateComponent<TaskResourceComponent>())
          agentsAsync1 = await resourceComponent.GetAgentsAsync(poolId, includeAgentCloudRequest: true);
        foreach (TaskAgent agent in (IEnumerable<TaskAgent>) agentsAsync1.Agents)
        {
          if (string.Equals(agent.ProvisioningState, "Provisioned", StringComparison.OrdinalIgnoreCase) && agent.AssignedAgentCloudRequest != null)
            agents.Add(agent);
          else
            nonProvisionedAgents.Add(new Tuple<TaskAgent, IList<Demand>>(agent, (IList<Demand>) null));
        }
        TaskAgentQueryResult[] agentsAsync2 = new TaskAgentQueryResult[agentSpecifications.Length];
        for (int index = 0; index < agentSpecifications.Length; ++index)
        {
          TaskAgentQueryResult agentQueryResult = new TaskAgentQueryResult(agentSpecifications[index]);
          if (nonProvisionedAgents.Count > 0)
            agentQueryResult.UnmatchedAgents.AddRange<Tuple<TaskAgent, IList<Demand>>, IList<Tuple<TaskAgent, IList<Demand>>>>((IEnumerable<Tuple<TaskAgent, IList<Demand>>>) nonProvisionedAgents);
          foreach (TaskAgent taskAgent in (IEnumerable<TaskAgent>) agents)
          {
            if (agentSpecifications[index] != null && agentSpecifications[index].Equals((object) taskAgent.AssignedAgentCloudRequest.AgentSpecification))
              agentQueryResult.MatchedAgents.Add(taskAgent);
            else if (agentSpecifications[index] == null && taskAgent.AssignedAgentCloudRequest.AgentSpecification == null)
              agentQueryResult.MatchedAgents.Add(taskAgent);
            else
              agentQueryResult.UnmatchedAgents.Add(new Tuple<TaskAgent, IList<Demand>>(taskAgent, (IList<Demand>) null));
          }
          agentsAsync2[index] = agentQueryResult;
        }
        return (IList<TaskAgentQueryResult>) agentsAsync2;
      }
    }

    public TaskAgentPool GetAgentPool(
      IVssRequestContext requestContext,
      int poolId,
      IList<string> propertyFilters = null,
      TaskAgentPoolActionFilter actionFilter = TaskAgentPoolActionFilter.None)
    {
      requestContext = requestContext.ToPoolRequestContext();
      this.EnsureMessageQueueStarted(requestContext);
      using (new MethodScope(requestContext, "ResourceService", nameof (GetAgentPool)))
      {
        IAgentPoolSecurityProvider agentPoolSecurity = this.GetAgentPoolSecurity(requestContext, poolId);
        if (!agentPoolSecurity.HasPoolPermission(requestContext, poolId, 1, true))
          return (TaskAgentPool) null;
        TaskAgentPool pool = this.GetTaskAgentPoolInternal(requestContext, (IList<int>) new List<int>()
        {
          poolId
        }).FirstOrDefault<TaskAgentPoolData>()?.Pool;
        if (pool == null || pool.ShouldHidePool(requestContext))
          return (TaskAgentPool) null;
        if (actionFilter != TaskAgentPoolActionFilter.None)
        {
          int requiredPermissions = 0;
          if ((actionFilter & TaskAgentPoolActionFilter.Manage) == TaskAgentPoolActionFilter.Manage)
            requiredPermissions |= 2;
          if ((actionFilter & TaskAgentPoolActionFilter.Use) == TaskAgentPoolActionFilter.Use)
            requiredPermissions |= 16;
          if (requiredPermissions != 0)
            agentPoolSecurity.CheckPoolPermission(requestContext, pool.Id, requiredPermissions);
        }
        if (propertyFilters != null && propertyFilters.Count > 0)
        {
          using (TeamFoundationDataReader properties = requestContext.GetService<ITeamFoundationPropertyService>().GetProperties(requestContext, pool.CreateSpec(), (IEnumerable<string>) propertyFilters))
          {
            foreach (ArtifactPropertyValue current in properties.CurrentEnumerable<ArtifactPropertyValue>())
              pool.Properties = current.PropertyValues.Convert();
          }
        }
        pool.SetPoolVisibility(requestContext);
        return DistributedTaskResourceService.PopulateIdentityReferences(requestContext, pool);
      }
    }

    public async Task<TaskAgentPool> GetAgentPoolAsync(
      IVssRequestContext requestContext,
      int poolId,
      IList<string> propertyFilters = null,
      TaskAgentPoolActionFilter actionFilter = TaskAgentPoolActionFilter.None)
    {
      requestContext = requestContext.ToPoolRequestContext();
      this.EnsureMessageQueueStarted(requestContext);
      using (new MethodScope(requestContext, "ResourceService", nameof (GetAgentPoolAsync)))
      {
        IAgentPoolSecurityProvider poolSecurity = this.GetAgentPoolSecurity(requestContext, poolId);
        if (!poolSecurity.HasPoolPermission(requestContext, poolId, 1, true))
          return (TaskAgentPool) null;
        TaskAgentPoolCacheService poolCache = requestContext.GetService<TaskAgentPoolCacheService>();
        TaskAgentPoolData agentPoolAsync;
        if (!poolCache.TryGetValue(requestContext, poolId, out agentPoolAsync))
        {
          using (TaskResourceComponent thc = requestContext.CreateComponent<TaskResourceComponent>())
            agentPoolAsync = await thc.GetAgentPoolAsync(poolId);
          if (agentPoolAsync != null)
            poolCache.Set(requestContext, agentPoolAsync.Pool.Id, agentPoolAsync.Clone());
        }
        if (agentPoolAsync == null || agentPoolAsync.Pool.ShouldHidePool(requestContext))
          return (TaskAgentPool) null;
        if (actionFilter != TaskAgentPoolActionFilter.None)
        {
          int requiredPermissions = 0;
          if ((actionFilter & TaskAgentPoolActionFilter.Manage) == TaskAgentPoolActionFilter.Manage)
            requiredPermissions |= 2;
          if ((actionFilter & TaskAgentPoolActionFilter.Use) == TaskAgentPoolActionFilter.Use)
            requiredPermissions |= 16;
          if (requiredPermissions != 0)
            poolSecurity.CheckPoolPermission(requestContext, agentPoolAsync.Pool.Id, requiredPermissions);
        }
        if (propertyFilters != null && propertyFilters.Count > 0)
        {
          using (TeamFoundationDataReader properties = requestContext.GetService<ITeamFoundationPropertyService>().GetProperties(requestContext, agentPoolAsync.Pool.CreateSpec(), (IEnumerable<string>) propertyFilters))
          {
            foreach (ArtifactPropertyValue current in properties.CurrentEnumerable<ArtifactPropertyValue>())
              agentPoolAsync.Pool.Properties = current.PropertyValues.Convert();
          }
        }
        agentPoolAsync.Pool.SetPoolVisibility(requestContext);
        return DistributedTaskResourceService.PopulateIdentityReferences(requestContext, agentPoolAsync.Pool);
      }
    }

    public async Task<List<TaskAgentPool>> GetAgentPoolsAsync(
      IVssRequestContext requestContext,
      string poolName = null,
      IList<string> propertyFilters = null,
      TaskAgentPoolType poolType = TaskAgentPoolType.Automation,
      TaskAgentPoolActionFilter actionFilter = TaskAgentPoolActionFilter.None)
    {
      requestContext = requestContext.ToPoolRequestContext();
      this.EnsureMessageQueueStarted(requestContext);
      List<TaskAgentPool> agentPoolsAsync;
      using (new MethodScope(requestContext, "ResourceService", nameof (GetAgentPoolsAsync)))
      {
        List<TaskAgentPool> allPools = new List<TaskAgentPool>();
        using (TaskResourceComponent thc = requestContext.CreateComponent<TaskResourceComponent>())
        {
          List<TaskAgentPool> taskAgentPoolList = allPools;
          taskAgentPoolList.AddRange((await thc.GetAgentPoolsAsync(poolName, poolType)).Select<TaskAgentPoolData, TaskAgentPool>((Func<TaskAgentPoolData, TaskAgentPool>) (x => x.Pool)));
          taskAgentPoolList = (List<TaskAgentPool>) null;
        }
        agentPoolsAsync = this.FilterAgentPools(requestContext, allPools, propertyFilters, actionFilter);
      }
      return agentPoolsAsync;
    }

    public List<TaskAgentPool> GetAgentPools(
      IVssRequestContext requestContext,
      string poolName = null,
      IList<string> propertyFilters = null,
      TaskAgentPoolType poolType = TaskAgentPoolType.Automation,
      TaskAgentPoolActionFilter actionFilter = TaskAgentPoolActionFilter.None)
    {
      requestContext = requestContext.ToPoolRequestContext();
      this.EnsureMessageQueueStarted(requestContext);
      using (new MethodScope(requestContext, "ResourceService", nameof (GetAgentPools)))
      {
        List<TaskAgentPool> allPools = new List<TaskAgentPool>();
        using (TaskResourceComponent component = requestContext.CreateComponent<TaskResourceComponent>())
          allPools.AddRange(component.GetAgentPools(poolName, poolType).Select<TaskAgentPoolData, TaskAgentPool>((Func<TaskAgentPoolData, TaskAgentPool>) (x => x.Pool)));
        return this.FilterAgentPools(requestContext, allPools, propertyFilters, actionFilter);
      }
    }

    public List<TaskAgentPool> GetActiveAgentPools(
      IVssRequestContext requestContext,
      DateTime activeSince,
      IList<string> propertyFilters = null,
      TaskAgentPoolActionFilter actionFilter = TaskAgentPoolActionFilter.None)
    {
      requestContext = requestContext.ToPoolRequestContext();
      this.EnsureMessageQueueStarted(requestContext);
      using (new MethodScope(requestContext, "ResourceService", nameof (GetActiveAgentPools)))
      {
        List<TaskAgentPool> allPools = new List<TaskAgentPool>();
        using (TaskResourceComponent component = requestContext.CreateComponent<TaskResourceComponent>())
          allPools.AddRange(component.GetActiveAgentPools(activeSince).Select<TaskAgentPoolData, TaskAgentPool>((Func<TaskAgentPoolData, TaskAgentPool>) (x => x.Pool)));
        return this.FilterAgentPools(requestContext, allPools, propertyFilters, actionFilter);
      }
    }

    public async Task<List<TaskAgentPool>> GetAgentPoolsByIdsAsync(
      IVssRequestContext requestContext,
      IList<int> poolIds,
      IList<string> propertyFilters = null,
      TaskAgentPoolActionFilter actionFilter = TaskAgentPoolActionFilter.None)
    {
      requestContext = requestContext.ToPoolRequestContext();
      this.EnsureMessageQueueStarted(requestContext);
      List<TaskAgentPool> agentPoolsByIdsAsync;
      using (new MethodScope(requestContext, "ResourceService", nameof (GetAgentPoolsByIdsAsync)))
      {
        List<TaskAgentPool> allPools = new List<TaskAgentPool>();
        TaskAgentPoolCacheService poolCache = requestContext.GetService<TaskAgentPoolCacheService>();
        HashSet<int> poolIdsToFetch = new HashSet<int>();
        HashSet<int> cacheMisses = (HashSet<int>) null;
        foreach (int key in poolIds.Distinct<int>())
        {
          TaskAgentPoolData taskAgentPoolData;
          if (poolCache.TryGetValue(requestContext, key, out taskAgentPoolData))
            allPools.Add(taskAgentPoolData.Pool);
          else if (!requestContext.TryGetItem<HashSet<int>>("MS.TF.DistributedTask.PoolCacheMisses", out cacheMisses) || !cacheMisses.Contains(key))
            poolIdsToFetch.Add(key);
        }
        if (poolIdsToFetch.Count > 0)
        {
          List<TaskAgentPoolData> fetchedPoolsData = new List<TaskAgentPoolData>();
          using (TaskResourceComponent thc = requestContext.CreateComponent<TaskResourceComponent>())
            fetchedPoolsData.AddRange((IEnumerable<TaskAgentPoolData>) await thc.GetAgentPoolsByIdAsync(poolIdsToFetch));
          foreach (TaskAgentPoolData taskAgentPoolData in fetchedPoolsData)
          {
            allPools.Add(taskAgentPoolData.Pool);
            poolCache.Set(requestContext, taskAgentPoolData.Pool.Id, taskAgentPoolData.Clone());
            poolIdsToFetch.Remove(taskAgentPoolData.Pool.Id);
          }
          if (poolIdsToFetch.Count > 0)
          {
            if (cacheMisses == null && !requestContext.TryGetItem<HashSet<int>>("MS.TF.DistributedTask.PoolCacheMisses", out cacheMisses))
            {
              cacheMisses = new HashSet<int>();
              requestContext.Items["MS.TF.DistributedTask.PoolCacheMisses"] = (object) cacheMisses;
            }
            cacheMisses.UnionWith((IEnumerable<int>) poolIdsToFetch);
          }
          fetchedPoolsData = (List<TaskAgentPoolData>) null;
        }
        agentPoolsByIdsAsync = this.FilterAgentPools(requestContext, allPools, propertyFilters, actionFilter);
      }
      return agentPoolsByIdsAsync;
    }

    public async Task<Stream> GetAgentPoolMetadataAsync(
      IVssRequestContext requestContext,
      int poolId)
    {
      requestContext.AssertAsyncExecutionEnabled();
      requestContext = requestContext.ToPoolRequestContext();
      this.EnsureMessageQueueStarted(requestContext);
      using (new MethodScope(requestContext, "ResourceService", nameof (GetAgentPoolMetadataAsync)))
      {
        if (!this.GetAgentPoolSecurity(requestContext, poolId).HasPoolPermission(requestContext, poolId, 1, true))
          return (Stream) null;
        TaskAgentPoolData agentPoolAsync;
        using (TaskResourceComponent thc = requestContext.CreateComponent<TaskResourceComponent>())
          agentPoolAsync = await thc.GetAgentPoolAsync(poolId);
        Stream poolMetadataAsync = (Stream) null;
        if (agentPoolAsync != null)
        {
          ITaskAgentExtension taskAgentExtension = this.GetTaskAgentExtension(requestContext, poolId);
          try
          {
            poolMetadataAsync = await taskAgentExtension.GetAgentPoolMetadataAsync(requestContext, agentPoolAsync.Pool.Name, agentPoolAsync.PoolMetadataFileId);
          }
          catch (VssServiceResponseException ex)
          {
            requestContext.TraceException("ResourceService", (Exception) ex);
            if (ex.HttpStatusCode != HttpStatusCode.NotFound)
              throw ex;
            poolMetadataAsync = (Stream) null;
          }
        }
        return poolMetadataAsync;
      }
    }

    public IList<TaskAgentPoolStatus> GetAgentPoolStatusByIds(
      IVssRequestContext requestContext,
      IList<int> poolIds)
    {
      requestContext = requestContext.ToPoolRequestContext();
      this.EnsureMessageQueueStarted(requestContext);
      using (new MethodScope(requestContext, "ResourceService", nameof (GetAgentPoolStatusByIds)))
      {
        List<int> poolIds1 = new List<int>();
        foreach (int poolId in (IEnumerable<int>) poolIds)
        {
          if (this.GetAgentPoolSecurity(requestContext, poolId).HasPoolPermission(requestContext, poolId, 1, true))
            poolIds1.Add(poolId);
        }
        if (poolIds1.Count == 0)
          return (IList<TaskAgentPoolStatus>) new List<TaskAgentPoolStatus>();
        using (TaskResourceComponent component = requestContext.CreateComponent<TaskResourceComponent>())
          return component.GetAgentPoolStatusByIds((IEnumerable<int>) poolIds1);
      }
    }

    public TaskAgentQueue GetAgentQueue(
      IVssRequestContext requestContext,
      Guid projectId,
      int queueId,
      TaskAgentQueueActionFilter actionFilter = TaskAgentQueueActionFilter.None)
    {
      using (new MethodScope(requestContext, "ResourceService", nameof (GetAgentQueue)))
      {
        TaskAgentQueueCacheService service = requestContext.GetService<TaskAgentQueueCacheService>();
        TaskAgentQueue queue;
        if (!service.TryGetValue(projectId, queueId, out queue))
        {
          string keyToken = service.GetKeyToken(projectId, queueId);
          HashSet<string> stringSet;
          if (requestContext.TryGetItem<HashSet<string>>("MS.TF.DistributedTask.QueueCacheMisses", out stringSet) && stringSet.Contains(keyToken))
            return (TaskAgentQueue) null;
          using (TaskResourceComponent component = requestContext.CreateComponent<TaskResourceComponent>())
            queue = component.GetAgentQueue(projectId, queueId).Queue;
          if (queue != null)
          {
            service.Set(projectId, queue.Id, queue.Clone());
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
        }
        if (queue == null || !this.Security.HasQueuePermission(requestContext, projectId, queueId, 1))
          return (TaskAgentQueue) null;
        if (actionFilter != TaskAgentQueueActionFilter.None)
        {
          int requiredPermissions = 0;
          if ((actionFilter & TaskAgentQueueActionFilter.Manage) == TaskAgentQueueActionFilter.Manage)
            requiredPermissions |= 2;
          if ((actionFilter & TaskAgentQueueActionFilter.Use) == TaskAgentQueueActionFilter.Use)
            requiredPermissions |= 16;
          this.Security.CheckQueuePermission(requestContext, projectId, queue.Id, requiredPermissions);
        }
        TaskAgentQueue taskAgentQueue = queue.PopulateReferences(requestContext);
        return taskAgentQueue.Pool.ShouldHidePool(requestContext) ? (TaskAgentQueue) null : taskAgentQueue;
      }
    }

    public async Task<TaskAgentQueue> GetAgentQueueAsync(
      IVssRequestContext requestContext,
      Guid projectId,
      int queueId,
      TaskAgentQueueActionFilter actionFilter = TaskAgentQueueActionFilter.None)
    {
      using (new MethodScope(requestContext, "ResourceService", nameof (GetAgentQueueAsync)))
      {
        TaskAgentQueueCacheService queueCache = requestContext.GetService<TaskAgentQueueCacheService>();
        TaskAgentQueue queue;
        if (!queueCache.TryGetValue(projectId, queueId, out queue))
        {
          string cacheKey = queueCache.GetKeyToken(projectId, queueId);
          HashSet<string> cacheMisses;
          if (requestContext.TryGetItem<HashSet<string>>("MS.TF.DistributedTask.QueueCacheMisses", out cacheMisses) && cacheMisses.Contains(cacheKey))
            return (TaskAgentQueue) null;
          using (TaskResourceComponent rc = requestContext.CreateComponent<TaskResourceComponent>())
            queue = (await rc.GetAgentQueueAsync(projectId, queueId)).Queue;
          if (queue != null)
          {
            queueCache.Set(projectId, queue.Id, queue.Clone());
          }
          else
          {
            if (cacheMisses == null && !requestContext.TryGetItem<HashSet<string>>("MS.TF.DistributedTask.QueueCacheMisses", out cacheMisses))
            {
              cacheMisses = new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
              requestContext.Items["MS.TF.DistributedTask.QueueCacheMisses"] = (object) cacheMisses;
            }
            cacheMisses.Add(cacheKey);
          }
          cacheMisses = (HashSet<string>) null;
          cacheKey = (string) null;
        }
        if (queue == null || !this.Security.HasQueuePermission(requestContext, projectId, queueId, 1))
          return (TaskAgentQueue) null;
        if (actionFilter != TaskAgentQueueActionFilter.None)
        {
          int requiredPermissions = 0;
          if ((actionFilter & TaskAgentQueueActionFilter.Manage) == TaskAgentQueueActionFilter.Manage)
            requiredPermissions |= 2;
          if ((actionFilter & TaskAgentQueueActionFilter.Use) == TaskAgentQueueActionFilter.Use)
            requiredPermissions |= 16;
          this.Security.CheckQueuePermission(requestContext, projectId, queue.Id, requiredPermissions);
        }
        TaskAgentQueue taskAgentQueue = queue.PopulateReferences(requestContext);
        return !taskAgentQueue.Pool.ShouldHidePool(requestContext) ? taskAgentQueue : (TaskAgentQueue) null;
      }
    }

    public IList<TaskAgentQueue> GetAgentQueues(
      IVssRequestContext requestContext,
      Guid projectId,
      string queueName = null,
      TaskAgentQueueActionFilter actionFilter = TaskAgentQueueActionFilter.None)
    {
      using (new MethodScope(requestContext, "ResourceService", nameof (GetAgentQueues)))
      {
        List<TaskAgentQueue> allQueues = new List<TaskAgentQueue>();
        using (TaskResourceComponent component = requestContext.CreateComponent<TaskResourceComponent>())
          allQueues.AddRange((IEnumerable<TaskAgentQueue>) component.GetAgentQueues(projectId, queueName).Queues);
        return (IList<TaskAgentQueue>) this.FilterQueues(requestContext, projectId, (IList<TaskAgentQueue>) allQueues, actionFilter).ToList<TaskAgentQueue>();
      }
    }

    public IList<TaskAgentQueue> GetAgentQueues(
      IVssRequestContext requestContext,
      Guid projectId,
      IEnumerable<string> names,
      TaskAgentQueueActionFilter actionFilter = TaskAgentQueueActionFilter.None)
    {
      using (new MethodScope(requestContext, "ResourceService", nameof (GetAgentQueues)))
      {
        ArgumentUtility.CheckEnumerableForEmpty((IEnumerable) names, nameof (names), "DistributedTask");
        List<TaskAgentQueue> allQueues = new List<TaskAgentQueue>();
        using (TaskResourceComponent component = requestContext.CreateComponent<TaskResourceComponent>())
          allQueues.AddRange((IEnumerable<TaskAgentQueue>) component.GetAgentQueuesByName(projectId, names).Queues);
        return (IList<TaskAgentQueue>) this.FilterQueues(requestContext, projectId, (IList<TaskAgentQueue>) allQueues, actionFilter).ToList<TaskAgentQueue>();
      }
    }

    public IList<TaskAgentQueue> GetAgentQueuesForPools(
      IVssRequestContext requestContext,
      Guid projectId,
      IEnumerable<int> poolIds,
      TaskAgentQueueActionFilter actionFilter = TaskAgentQueueActionFilter.None)
    {
      using (new MethodScope(requestContext, "ResourceService", nameof (GetAgentQueuesForPools)))
      {
        List<TaskAgentQueue> allQueues = new List<TaskAgentQueue>();
        using (TaskResourceComponent component = requestContext.CreateComponent<TaskResourceComponent>())
          allQueues.AddRange((IEnumerable<TaskAgentQueue>) component.GetAgentQueuesByPoolId(projectId, poolIds).Queues);
        return (IList<TaskAgentQueue>) this.FilterQueues(requestContext, projectId, (IList<TaskAgentQueue>) allQueues, actionFilter).ToList<TaskAgentQueue>();
      }
    }

    public IList<TaskAgentQueue> GetAgentQueues(
      IVssRequestContext requestContext,
      Guid projectId,
      IEnumerable<int> ids,
      TaskAgentQueueActionFilter actionFilter = TaskAgentQueueActionFilter.None)
    {
      using (new MethodScope(requestContext, "ResourceService", nameof (GetAgentQueues)))
      {
        ArgumentUtility.CheckEnumerableForEmpty((IEnumerable) ids, nameof (ids), "DistributedTask");
        List<TaskAgentQueue> allQueues = new List<TaskAgentQueue>();
        using (TaskResourceComponent component = requestContext.CreateComponent<TaskResourceComponent>())
          allQueues.AddRange((IEnumerable<TaskAgentQueue>) component.GetAgentQueuesById(projectId, ids).Queues);
        return (IList<TaskAgentQueue>) this.FilterQueues(requestContext, projectId, (IList<TaskAgentQueue>) allQueues, actionFilter).ToList<TaskAgentQueue>();
      }
    }

    public IList<TaskAgentQueue> GetHostedAgentQueues(
      IVssRequestContext requestContext,
      Guid projectId,
      TaskAgentQueueActionFilter actionFilter = TaskAgentQueueActionFilter.None)
    {
      using (new MethodScope(requestContext, "ResourceService", nameof (GetHostedAgentQueues)))
      {
        TaskAgentQueueCacheService service = requestContext.GetService<TaskAgentQueueCacheService>();
        IList<TaskAgentQueue> queues;
        if (!service.TryGetHostedQueues(projectId, out queues))
        {
          queues = (IList<TaskAgentQueue>) new List<TaskAgentQueue>();
          using (TaskResourceComponent component = requestContext.CreateComponent<TaskResourceComponent>())
            queues.AddRange<TaskAgentQueue, IList<TaskAgentQueue>>((IEnumerable<TaskAgentQueue>) component.GetHostedAgentQueues(projectId).Queues);
          service.SetHostedQueues(projectId, queues);
        }
        return (IList<TaskAgentQueue>) this.FilterQueues(requestContext, projectId, queues, actionFilter).ToList<TaskAgentQueue>();
      }
    }

    public async Task<IList<AgentDefinition>> GetAgentPoolDefinitions(
      IVssRequestContext requestContext,
      int poolId)
    {
      DistributedTaskResourceService taskResourceService = this;
      requestContext.AssertAsyncExecutionEnabled();
      requestContext = requestContext.ToPoolRequestContext();
      using (new MethodScope(requestContext, "ResourceService", nameof (GetAgentPoolDefinitions)))
      {
        // ISSUE: explicit non-virtual call
        if (!__nonvirtual (taskResourceService.GetAgentPoolSecurity(requestContext, poolId)).HasPoolPermission(requestContext, poolId, 1, true))
          return Task.FromResult<IList<AgentDefinition>>((IList<AgentDefinition>) new List<AgentDefinition>()).Result;
        TaskAgentPoolData taskAgentPoolData = taskResourceService.GetTaskAgentPoolInternal(requestContext, (IList<int>) new List<int>()
        {
          poolId
        }).FirstOrDefault<TaskAgentPoolData>();
        if (taskAgentPoolData == null)
          throw new TaskAgentPoolNotFoundException(TaskResources.AgentPoolNotFound((object) poolId));
        if (taskAgentPoolData != null)
        {
          TaskAgentPool pool = taskAgentPoolData.Pool;
          int? agentCloudId1;
          int num;
          if (pool == null)
          {
            num = 0;
          }
          else
          {
            agentCloudId1 = pool.AgentCloudId;
            num = agentCloudId1.HasValue ? 1 : 0;
          }
          if (num != 0)
          {
            IInternalAgentCloudService service = requestContext.GetService<IInternalAgentCloudService>();
            IVssRequestContext requestContext1 = requestContext;
            agentCloudId1 = taskAgentPoolData.Pool.AgentCloudId;
            int agentCloudId2 = agentCloudId1.Value;
            return await service.GetAgentDefinitionsAsync(requestContext1, agentCloudId2);
          }
        }
        return Task.FromResult<IList<AgentDefinition>>((IList<AgentDefinition>) new List<AgentDefinition>()).Result;
      }
    }

    public async Task<IList<AgentDefinition>> GetAgentQueueDefinitions(
      IVssRequestContext requestContext,
      Guid projectId,
      int queueId)
    {
      DistributedTaskResourceService taskResourceService = this;
      // ISSUE: explicit non-virtual call
      TaskAgentPoolData taskAgentPoolData = taskResourceService.GetTaskAgentPoolInternal(requestContext, (IList<int>) new int[1]
      {
        (__nonvirtual (taskResourceService.GetAgentQueue(requestContext, projectId, queueId, TaskAgentQueueActionFilter.None)) ?? throw new TaskAgentQueueNotFoundException(TaskResources.QueueNotFound((object) queueId))).Pool.Id
      }).FirstOrDefault<TaskAgentPoolData>();
      IInternalAgentCloudService service = requestContext.GetService<IInternalAgentCloudService>();
      if (taskAgentPoolData != null)
      {
        TaskAgentPool pool = taskAgentPoolData.Pool;
        int? agentCloudId1;
        int num;
        if (pool == null)
        {
          num = 0;
        }
        else
        {
          agentCloudId1 = pool.AgentCloudId;
          num = agentCloudId1.HasValue ? 1 : 0;
        }
        if (num != 0)
        {
          IInternalAgentCloudService agentCloudService = service;
          IVssRequestContext requestContext1 = requestContext;
          agentCloudId1 = taskAgentPoolData.Pool.AgentCloudId;
          int agentCloudId2 = agentCloudId1.Value;
          return await agentCloudService.GetAgentDefinitionsAsync(requestContext1, agentCloudId2);
        }
      }
      return Task.FromResult<IList<AgentDefinition>>((IList<AgentDefinition>) new List<AgentDefinition>()).Result;
    }

    public TaskAgentJobRequest GetAgentRequest(
      IVssRequestContext requestContext,
      int poolId,
      long requestId,
      bool includeStatus = false)
    {
      requestContext = requestContext.ToPoolRequestContext();
      this.EnsureMessageQueueStarted(requestContext);
      using (new MethodScope(requestContext, "ResourceService", nameof (GetAgentRequest)))
      {
        if (!this.GetAgentPoolSecurity(requestContext, poolId).HasPoolPermission(requestContext, poolId, 1, true))
          return (TaskAgentJobRequest) null;
        TaskAgentRequestData agentRequest1;
        using (TaskResourceComponent component = requestContext.CreateComponent<TaskResourceComponent>())
          agentRequest1 = component.GetAgentRequest(poolId, requestId, includeStatus);
        TaskAgentJobRequest agentRequest2 = agentRequest1?.AgentRequest;
        TaskAgentCloudRequest cloudRequest = agentRequest1?.CloudRequest;
        if (includeStatus && agentRequest2 != null)
        {
          agentRequest2.StatusMessage = TaskResources.AgentRequestRunningOrCompleted();
          DateTime? nullable1 = agentRequest2.ReceiveTime;
          if (!nullable1.HasValue)
          {
            TaskAgentReference reservedAgent = agentRequest2.ReservedAgent;
            int num1;
            if (reservedAgent == null)
            {
              num1 = 0;
            }
            else
            {
              int id = reservedAgent.Id;
              num1 = 1;
            }
            if (num1 != 0)
            {
              IAgentCloudService service = requestContext.GetService<IAgentCloudService>();
              agentRequest2.StatusMessage = service.GetAgentCloudRequestStatus(requestContext, agentRequest1.CloudRequest);
              if (cloudRequest != null)
              {
                nullable1 = cloudRequest.ProvisionRequestTime;
                if (nullable1.HasValue)
                {
                  DateTime now = DateTime.Now;
                  ref DateTime local = ref now;
                  nullable1 = cloudRequest.ProvisionRequestTime;
                  DateTime dateTime = nullable1.Value;
                  TimeSpan timeSpan = local.Subtract(dateTime);
                  if (timeSpan.TotalSeconds > (double) TaskConstants.ThresholdForWarrningRequestDealyedFromPPInSeconds)
                  {
                    TaskAgentJobRequest taskAgentJobRequest = agentRequest2;
                    string statusMessage = taskAgentJobRequest.StatusMessage;
                    nullable1 = cloudRequest.ProvisionRequestTime;
                    string str = TaskResources.AgentRequestPoolProviderSlowWarning((object) nullable1.Value, (object) timeSpan.TotalMinutes);
                    taskAgentJobRequest.StatusMessage = statusMessage + "/n" + str;
                  }
                }
              }
            }
            else
            {
              if (agentRequest1.AvailableAgentCount.HasValue)
              {
                int? nullable2 = agentRequest1.AvailableAgentCount;
                int num2 = 0;
                if (nullable2.GetValueOrDefault() > num2 & nullable2.HasValue)
                {
                  TaskAgentPoolData taskAgentPoolData = this.GetTaskAgentPoolInternal(requestContext, (IList<int>) new int[1]
                  {
                    poolId
                  }).FirstOrDefault<TaskAgentPoolData>();
                  ResourceUsage resourceUsage = this.GetResourceUsage(requestContext, agentRequest2.GetParallelismTag(), taskAgentPoolData.Pool.IsHosted, true, false, 0);
                  string resourceThrottlingType = ResourceLimitUtil.GetPrettyResourceThrottlingType(agentRequest2.GetParallelismTag(), taskAgentPoolData.Pool.IsHosted);
                  nullable2 = resourceUsage.UsedCount;
                  int? totalCount = resourceUsage.ResourceLimit.TotalCount;
                  if (nullable2.GetValueOrDefault() >= totalCount.GetValueOrDefault() & nullable2.HasValue & totalCount.HasValue)
                  {
                    agentRequest2.StatusMessage = TaskResources.AgentRequestBlockedByParallelismLimits((object) resourceThrottlingType, (object) agentRequest1.ThrottlingQueuePosition, (object) resourceUsage.UsedCount, (object) resourceUsage.ResourceLimit.TotalCount);
                    IDictionary<string, string> data = agentRequest2.Data;
                    string queuePosition = TaskAgentRequestConstants.QueuePosition;
                    int? throttlingQueuePosition = agentRequest1.ThrottlingQueuePosition;
                    ref int? local = ref throttlingQueuePosition;
                    string str = local.HasValue ? local.GetValueOrDefault().ToString() : (string) null;
                    data.Add(queuePosition, str);
                    goto label_28;
                  }
                  else
                  {
                    agentRequest2.StatusMessage = TaskResources.AgentRequestIsBeingProcessed((object) resourceThrottlingType, (object) agentRequest1.ThrottlingQueuePosition, (object) resourceUsage.UsedCount, (object) resourceUsage.ResourceLimit.TotalCount);
                    goto label_28;
                  }
                }
              }
              if (agentRequest1.PoolQueuePosition.HasValue)
              {
                TaskAgentPoolData taskAgentPoolData = this.GetTaskAgentPoolInternal(requestContext, (IList<int>) new int[1]
                {
                  poolId
                }).FirstOrDefault<TaskAgentPoolData>();
                agentRequest2.StatusMessage = TaskResources.AgentRequestBlockedByPoolUsage((object) agentRequest1.PoolQueuePosition);
                string requestQueueStatusInfo = this.GetAgentRequestQueueStatusInfo(requestContext, agentRequest2, taskAgentPoolData.Pool);
                if (!string.IsNullOrEmpty(requestQueueStatusInfo))
                {
                  TaskAgentJobRequest taskAgentJobRequest = agentRequest2;
                  taskAgentJobRequest.StatusMessage = taskAgentJobRequest.StatusMessage + "\n" + requestQueueStatusInfo;
                }
                IDictionary<string, string> data = agentRequest2.Data;
                string queuePosition = TaskAgentRequestConstants.QueuePosition;
                int? poolQueuePosition = agentRequest1.PoolQueuePosition;
                ref int? local = ref poolQueuePosition;
                string str = local.HasValue ? local.GetValueOrDefault().ToString() : (string) null;
                data.Add(queuePosition, str);
              }
              else
                agentRequest2.StatusMessage = (string) null;
            }
          }
        }
label_28:
        return agentRequest2.PopulateReferenceLinks(requestContext, poolId);
      }
    }

    internal string GetAgentRequestQueueStatusInfo(
      IVssRequestContext requestContext,
      TaskAgentJobRequest request,
      TaskAgentPool pool)
    {
      if (!request.MatchesAllAgentsInPool)
      {
        if (!pool.AutoUpdate.GetValueOrDefault(true))
        {
          DemandMinimumVersion versionDemand = request.Demands.Where<Demand>((Func<Demand, bool>) (x => x is DemandMinimumVersion)).Cast<DemandMinimumVersion>().FirstOrDefault<DemandMinimumVersion>();
          if (versionDemand != null)
          {
            TaskAgentQueryResult agents = this.GetAgents(requestContext, pool.Id, request.Demands, (IList<string>) null);
            if (agents.UnmatchedAgents.Select<Tuple<TaskAgent, IList<Demand>>, IList<Demand>>((Func<Tuple<TaskAgent, IList<Demand>>, IList<Demand>>) (x => x.Item2)).Where<IList<Demand>>((Func<IList<Demand>, bool>) (demands => demands.Count<Demand>() == 1 && demands[0] is DemandMinimumVersion)).Count<IList<Demand>>() > 0)
              return agents.MatchedAgents.Count<TaskAgent>() > 0 ? TaskResources.AgentRequestPoolNoUpdateStatus((object) pool.Name, (object) versionDemand.Value) : DistributedTaskResourceService.GetAgentNoUpdateErrorMessage(pool.Name, versionDemand);
          }
        }
        if (request.MatchedAgents.Count == 0)
          return TaskResources.AgentRequestPoolNoneMatched((object) pool.Name);
      }
      return (string) null;
    }

    public async Task<TaskAgentJobRequest> GetAgentRequestAsync(
      IVssRequestContext requestContext,
      int poolId,
      long requestId,
      bool includeStatus = false)
    {
      DistributedTaskResourceService taskResourceService = this;
      requestContext.AssertAsyncExecutionEnabled();
      requestContext = requestContext.ToPoolRequestContext();
      taskResourceService.EnsureMessageQueueStarted(requestContext);
      using (new MethodScope(requestContext, "ResourceService", nameof (GetAgentRequestAsync)))
      {
        // ISSUE: explicit non-virtual call
        if (!__nonvirtual (taskResourceService.GetAgentPoolSecurity(requestContext, poolId)).HasPoolPermission(requestContext, poolId, 1, true))
          return (TaskAgentJobRequest) null;
        TaskAgentRequestData data;
        using (TaskResourceComponent rc = requestContext.CreateComponent<TaskResourceComponent>())
          data = await rc.GetAgentRequestAsync(poolId, requestId, includeStatus);
        TaskAgentJobRequest request = data?.AgentRequest;
        TaskAgentCloudRequest taskAgentCloudRequest = data?.CloudRequest;
        if (includeStatus && request != null)
        {
          request.StatusMessage = TaskResources.AgentRequestRunningOrCompleted();
          DateTime? nullable = request.ReceiveTime;
          if (!nullable.HasValue)
          {
            TaskAgentReference reservedAgent = request.ReservedAgent;
            int num1;
            if (reservedAgent == null)
            {
              num1 = 0;
            }
            else
            {
              int id = reservedAgent.Id;
              num1 = 1;
            }
            if (num1 != 0)
            {
              IAgentCloudService service = requestContext.GetService<IAgentCloudService>();
              TaskAgentJobRequest taskAgentJobRequest = request;
              IVssRequestContext requestContext1 = requestContext;
              TaskAgentCloudRequest cloudRequest = data.CloudRequest;
              taskAgentJobRequest.StatusMessage = await service.GetAgentCloudRequestStatusAsync(requestContext1, cloudRequest);
              taskAgentJobRequest = (TaskAgentJobRequest) null;
              if (taskAgentCloudRequest != null)
              {
                nullable = taskAgentCloudRequest.ProvisionRequestTime;
                if (nullable.HasValue)
                {
                  DateTime now = DateTime.Now;
                  ref DateTime local = ref now;
                  nullable = taskAgentCloudRequest.ProvisionRequestTime;
                  DateTime dateTime = nullable.Value;
                  TimeSpan timeSpan = local.Subtract(dateTime);
                  if (timeSpan.TotalSeconds > (double) TaskConstants.ThresholdForWarrningRequestDealyedFromPPInSeconds)
                  {
                    TaskAgentJobRequest taskAgentJobRequest1 = request;
                    string statusMessage = taskAgentJobRequest1.StatusMessage;
                    nullable = taskAgentCloudRequest.ProvisionRequestTime;
                    string str = TaskResources.AgentRequestPoolProviderSlowWarning((object) nullable.Value, (object) timeSpan.TotalMinutes);
                    taskAgentJobRequest1.StatusMessage = statusMessage + "/n" + str;
                  }
                }
              }
            }
            else
            {
              int? availableAgentCount = data.AvailableAgentCount;
              int num2 = 0;
              if (availableAgentCount.GetValueOrDefault() > num2 & availableAgentCount.HasValue)
              {
                TaskAgentPoolData taskAgentPoolData = (await taskResourceService.GetTaskAgentPoolInternalAsync(requestContext, (IList<int>) new int[1]
                {
                  poolId
                })).FirstOrDefault<TaskAgentPoolData>();
                // ISSUE: explicit non-virtual call
                ResourceUsage resourceUsage = __nonvirtual (taskResourceService.GetResourceUsage(requestContext, request.GetParallelismTag(), taskAgentPoolData.Pool.IsHosted, true, false, 0));
                string resourceThrottlingType = ResourceLimitUtil.GetPrettyResourceThrottlingType(request.GetParallelismTag(), taskAgentPoolData.Pool.IsHosted);
                int? usedCount = resourceUsage.UsedCount;
                int? totalCount = resourceUsage.ResourceLimit.TotalCount;
                if (usedCount.GetValueOrDefault() >= totalCount.GetValueOrDefault() & usedCount.HasValue & totalCount.HasValue)
                {
                  request.StatusMessage = TaskResources.AgentRequestBlockedByParallelismLimits((object) resourceThrottlingType, (object) data.ThrottlingQueuePosition, (object) resourceUsage.UsedCount, (object) resourceUsage.ResourceLimit.TotalCount);
                  IDictionary<string, string> data1 = request.Data;
                  string queuePosition = TaskAgentRequestConstants.QueuePosition;
                  int? throttlingQueuePosition = data.ThrottlingQueuePosition;
                  ref int? local = ref throttlingQueuePosition;
                  string str = local.HasValue ? local.GetValueOrDefault().ToString() : (string) null;
                  data1.Add(queuePosition, str);
                }
                else
                  request.StatusMessage = TaskResources.AgentRequestIsBeingProcessed((object) resourceThrottlingType, (object) data.ThrottlingQueuePosition, (object) resourceUsage.UsedCount, (object) resourceUsage.ResourceLimit.TotalCount);
              }
              else if (data.PoolQueuePosition.HasValue)
              {
                TaskAgentPoolData taskAgentPoolData = (await taskResourceService.GetTaskAgentPoolInternalAsync(requestContext, (IList<int>) new int[1]
                {
                  poolId
                })).FirstOrDefault<TaskAgentPoolData>();
                request.StatusMessage = TaskResources.AgentRequestBlockedByPoolUsage((object) data.PoolQueuePosition);
                string requestQueueStatusInfo = taskResourceService.GetAgentRequestQueueStatusInfo(requestContext, request, taskAgentPoolData.Pool);
                if (!string.IsNullOrEmpty(requestQueueStatusInfo))
                {
                  TaskAgentJobRequest taskAgentJobRequest = request;
                  taskAgentJobRequest.StatusMessage = taskAgentJobRequest.StatusMessage + "\n" + requestQueueStatusInfo;
                }
              }
              else
                request.StatusMessage = (string) null;
            }
          }
        }
        return request.PopulateReferenceLinks(requestContext, poolId);
      }
    }

    public async Task<IPagedList<TaskAgentJobRequest>> GetAgentRequestsForQueueAsync(
      IVssRequestContext requestContext,
      Guid projectId,
      int queueId,
      string continuationToken,
      int? maxRequestCount)
    {
      TaskAgentQueue agentQueueAsync = await this.GetAgentQueueAsync(requestContext, projectId, queueId);
      return agentQueueAsync != null ? await this.GetAgentRequestsAsync(requestContext, agentQueueAsync.Pool.Id, continuationToken, maxRequestCount) : (IPagedList<TaskAgentJobRequest>) new PagedList<TaskAgentJobRequest>((IEnumerable<TaskAgentJobRequest>) Array.Empty<TaskAgentJobRequest>(), (string) null);
    }

    public async Task<IPagedList<TaskAgentJobRequest>> GetAgentRequestsAsync(
      IVssRequestContext requestContext,
      int poolId,
      string continuationToken,
      int? maxRequestCount)
    {
      requestContext = requestContext.ToPoolRequestContext();
      this.EnsureMessageQueueStarted(requestContext);
      if (maxRequestCount.HasValue)
      {
        int? nullable = maxRequestCount;
        int num1 = 0;
        if (!(nullable.GetValueOrDefault() < num1 & nullable.HasValue))
        {
          nullable = maxRequestCount;
          int num2 = 200;
          if (!(nullable.GetValueOrDefault() > num2 & nullable.HasValue))
            goto label_4;
        }
      }
      maxRequestCount = new int?(200);
label_4:
      using (new MethodScope(requestContext, "ResourceService", nameof (GetAgentRequestsAsync)))
      {
        if (!this.GetAgentPoolSecurity(requestContext, poolId).HasPoolPermission(requestContext, poolId, 1))
          return (IPagedList<TaskAgentJobRequest>) new PagedList<TaskAgentJobRequest>((IEnumerable<TaskAgentJobRequest>) Array.Empty<TaskAgentJobRequest>(), (string) null);
        DateTime? lastRunningAssignTime;
        long? lastQueuedRequestId;
        DateTime? lastFinishedFInishTime;
        if (!TaskAgentRequestQueryContinuationTokenHelper.TryParseContinuationToken(continuationToken, out lastRunningAssignTime, out lastQueuedRequestId, out lastFinishedFInishTime))
          throw new InvalidContinuationTokenException(TaskResources.InvalidContinuationToken((object) continuationToken));
        TaskAgentRequestQueryResult agentRequestsAsync;
        using (TaskResourceComponent rc = requestContext.CreateComponent<TaskResourceComponent>())
          agentRequestsAsync = await rc.GetAgentRequestsAsync(poolId, maxRequestCount.Value, lastRunningAssignTime, lastQueuedRequestId, lastFinishedFInishTime);
        string continuationToken1 = TaskAgentRequestQueryContinuationTokenHelper.GetContinuationToken(agentRequestsAsync);
        IList<TaskAgentJobRequest> runningRequests = agentRequestsAsync.RunningRequests;
        runningRequests.AddRange<TaskAgentJobRequest, IList<TaskAgentJobRequest>>((IEnumerable<TaskAgentJobRequest>) agentRequestsAsync.QueuedRequests);
        runningRequests.AddRange<TaskAgentJobRequest, IList<TaskAgentJobRequest>>((IEnumerable<TaskAgentJobRequest>) agentRequestsAsync.FinishedRequests);
        return (IPagedList<TaskAgentJobRequest>) new PagedList<TaskAgentJobRequest>((IEnumerable<TaskAgentJobRequest>) runningRequests.PopulateReferenceLinks(requestContext, poolId), continuationToken1);
      }
    }

    public IPagedList<TaskAgentJobRequest> GetAgentRequests(
      IVssRequestContext requestContext,
      int poolId,
      string continuationToken,
      int? maxRequestCount)
    {
      requestContext = requestContext.ToPoolRequestContext();
      this.EnsureMessageQueueStarted(requestContext);
      if (maxRequestCount.HasValue)
      {
        int? nullable = maxRequestCount;
        int num1 = 0;
        if (!(nullable.GetValueOrDefault() < num1 & nullable.HasValue))
        {
          nullable = maxRequestCount;
          int num2 = 200;
          if (!(nullable.GetValueOrDefault() > num2 & nullable.HasValue))
            goto label_4;
        }
      }
      maxRequestCount = new int?(200);
label_4:
      using (new MethodScope(requestContext, "ResourceService", nameof (GetAgentRequests)))
      {
        if (!this.GetAgentPoolSecurity(requestContext, poolId).HasPoolPermission(requestContext, poolId, 1))
          return (IPagedList<TaskAgentJobRequest>) new PagedList<TaskAgentJobRequest>((IEnumerable<TaskAgentJobRequest>) Array.Empty<TaskAgentJobRequest>(), (string) null);
        DateTime? lastRunningAssignTime;
        long? lastQueuedRequestId;
        DateTime? lastFinishedFInishTime;
        if (!TaskAgentRequestQueryContinuationTokenHelper.TryParseContinuationToken(continuationToken, out lastRunningAssignTime, out lastQueuedRequestId, out lastFinishedFInishTime))
          throw new InvalidContinuationTokenException(TaskResources.InvalidContinuationToken((object) continuationToken));
        TaskAgentRequestQueryResult agentRequests;
        using (TaskResourceComponent component = requestContext.CreateComponent<TaskResourceComponent>())
          agentRequests = component.GetAgentRequests(poolId, maxRequestCount.Value, lastRunningAssignTime, lastQueuedRequestId, lastFinishedFInishTime);
        string continuationToken1 = TaskAgentRequestQueryContinuationTokenHelper.GetContinuationToken(agentRequests);
        IList<TaskAgentJobRequest> runningRequests = agentRequests.RunningRequests;
        runningRequests.AddRange<TaskAgentJobRequest, IList<TaskAgentJobRequest>>((IEnumerable<TaskAgentJobRequest>) agentRequests.QueuedRequests);
        runningRequests.AddRange<TaskAgentJobRequest, IList<TaskAgentJobRequest>>((IEnumerable<TaskAgentJobRequest>) agentRequests.FinishedRequests);
        return (IPagedList<TaskAgentJobRequest>) new PagedList<TaskAgentJobRequest>((IEnumerable<TaskAgentJobRequest>) runningRequests.PopulateReferenceLinks(requestContext, poolId), continuationToken1);
      }
    }

    public IList<TaskAgentJobRequest> GetAgentRequestsForPlan(
      IVssRequestContext requestContext,
      int poolId,
      Guid planId,
      Guid? jobId = null)
    {
      requestContext = requestContext.ToPoolRequestContext();
      this.EnsureMessageQueueStarted(requestContext);
      using (new MethodScope(requestContext, "ResourceService", nameof (GetAgentRequestsForPlan)))
      {
        if (!this.GetAgentPoolSecurity(requestContext, poolId).HasPoolPermission(requestContext, poolId, 1))
          return (IList<TaskAgentJobRequest>) Array.Empty<TaskAgentJobRequest>();
        IList<TaskAgentJobRequest> agentRequestsForPlan;
        using (TaskResourceComponent component = requestContext.CreateComponent<TaskResourceComponent>())
          agentRequestsForPlan = component.GetAgentRequestsForPlan(poolId, planId, jobId);
        return agentRequestsForPlan.PopulateReferenceLinks(requestContext, poolId);
      }
    }

    public IList<TaskAgentJobRequest> GetAgentRequestsForPlan(
      IVssRequestContext requestContext,
      Guid planId)
    {
      requestContext = requestContext.ToPoolRequestContext();
      this.EnsureMessageQueueStarted(requestContext);
      using (new MethodScope(requestContext, "ResourceService", nameof (GetAgentRequestsForPlan)))
      {
        IList<TaskAgentJobRequest> agentRequestsForPlan;
        using (TaskResourceComponent component = requestContext.CreateComponent<TaskResourceComponent>())
          agentRequestsForPlan = component.GetAgentRequestsForPlan(planId);
        IEnumerable<IGrouping<int, TaskAgentJobRequest>> groupings = agentRequestsForPlan.GroupBy<TaskAgentJobRequest, int>((Func<TaskAgentJobRequest, int>) (request => request.PoolId));
        IList<TaskAgentJobRequest> collection = (IList<TaskAgentJobRequest>) new List<TaskAgentJobRequest>();
        foreach (IGrouping<int, TaskAgentJobRequest> source in groupings)
        {
          if (this.GetAgentPoolSecurity(requestContext, source.Key).HasPoolPermission(requestContext, source.Key, 1))
          {
            IList<TaskAgentJobRequest> values = source.ToList<TaskAgentJobRequest>().PopulateReferenceLinks(requestContext, source.Key);
            collection.AddRange<TaskAgentJobRequest, IList<TaskAgentJobRequest>>((IEnumerable<TaskAgentJobRequest>) values);
          }
        }
        return collection;
      }
    }

    public IList<TaskAgentJobRequest> GetAgentRequestsForAgent(
      IVssRequestContext requestContext,
      int poolId,
      int agentId,
      int completedRequestCount = 50)
    {
      return this.GetAgentRequestsForAgents(requestContext, poolId, (IList<int>) new int[1]
      {
        agentId
      }, completedRequestCount);
    }

    public IList<TaskAgentJobRequest> GetAgentRequestsForAgents(
      IVssRequestContext requestContext,
      int poolId,
      IList<int> agentIds,
      int completedRequestCount = 50)
    {
      requestContext = requestContext.ToPoolRequestContext();
      this.EnsureMessageQueueStarted(requestContext);
      using (new MethodScope(requestContext, "ResourceService", nameof (GetAgentRequestsForAgents)))
      {
        if (!this.GetAgentPoolSecurity(requestContext, poolId).HasPoolPermission(requestContext, poolId, 1))
          return (IList<TaskAgentJobRequest>) Array.Empty<TaskAgentJobRequest>();
        IList<TaskAgentJobRequest> requestsForAgents;
        using (TaskResourceComponent component = requestContext.CreateComponent<TaskResourceComponent>())
          requestsForAgents = component.GetAgentRequestsForAgents(poolId, agentIds, completedRequestCount);
        return requestsForAgents.PopulateReferenceLinks(requestContext, poolId);
      }
    }

    public IList<TaskAgentJobRequest> GetAgentRequestsForAgents(
      IVssRequestContext requestContext,
      int poolId,
      IList<int> agentIds,
      Guid hostId,
      Guid scopeId,
      int completedRequestCount = 50,
      int? ownerId = null,
      DateTime? completedOn = null)
    {
      requestContext = requestContext.ToPoolRequestContext();
      this.EnsureMessageQueueStarted(requestContext);
      using (new MethodScope(requestContext, "ResourceService", nameof (GetAgentRequestsForAgents)))
      {
        if (!this.GetAgentPoolSecurity(requestContext, poolId).HasPoolPermission(requestContext, poolId, 1))
          return (IList<TaskAgentJobRequest>) Array.Empty<TaskAgentJobRequest>();
        IList<TaskAgentJobRequest> requestsForAgents;
        using (TaskResourceComponent component = requestContext.CreateComponent<TaskResourceComponent>())
          requestsForAgents = component.GetAgentRequestsForAgents(poolId, agentIds, hostId, scopeId, completedRequestCount, ownerId, completedOn);
        return requestsForAgents.PopulateReferenceLinks(requestContext, poolId);
      }
    }

    internal async Task<IList<TaskAgentJobRequest>> GetAgentRequestsForAgentsAsync(
      IVssRequestContext requestContext,
      int poolId,
      IList<int> agentIds,
      int completedRequestCount = 50)
    {
      requestContext.AssertAsyncExecutionEnabled();
      requestContext = requestContext.ToPoolRequestContext();
      this.EnsureMessageQueueStarted(requestContext);
      using (new MethodScope(requestContext, "ResourceService", nameof (GetAgentRequestsForAgentsAsync)))
      {
        if (!this.GetAgentPoolSecurity(requestContext, poolId).HasPoolPermission(requestContext, poolId, 1))
          return (IList<TaskAgentJobRequest>) Array.Empty<TaskAgentJobRequest>();
        IList<TaskAgentJobRequest> requestsForAgentsAsync;
        using (TaskResourceComponent rc = requestContext.CreateComponent<TaskResourceComponent>())
          requestsForAgentsAsync = await rc.GetAgentRequestsForAgentsAsync(poolId, agentIds, completedRequestCount);
        return requestsForAgentsAsync.PopulateReferenceLinks(requestContext, poolId);
      }
    }

    public async Task<TaskAgentMessage> GetMessageAsync(
      IVssRequestContext requestContext,
      int poolId,
      Guid sessionId,
      TimeSpan timeout,
      long? lastMessageId = null)
    {
      requestContext = requestContext.ToPoolRequestContext();
      TaskAgentMessage messageAsync;
      using (new MethodScope(requestContext, "ResourceService", nameof (GetMessageAsync)))
      {
        TaskAgentSessionData session = this.EnsureSession(requestContext, poolId, sessionId, false);
        await this.CheckViewAndOtherPermissionsForAgentAsync(requestContext, poolId, session?.Agent?.Id, new long?(), 4);
        session = this.EnsureSession(requestContext, poolId, sessionId);
        if (requestContext.ExecutionEnvironment.IsHostedDeployment && string.IsNullOrEmpty(session.Agent.AccessPoint) && !string.IsNullOrEmpty(requestContext.UserAgent) && requestContext.UserAgent.IndexOf("vstsagentcore-", StringComparison.OrdinalIgnoreCase) >= 0)
        {
          requestContext.Trace(10015151, TraceLevel.Info, "DistributedTask", "ResourceService", string.Format("Agent '{0}' of pool '{1}' doesn't have access point moniker.", (object) session.Agent.Id, (object) poolId));
          ILocationService service = requestContext.GetService<ILocationService>();
          AccessMapping vstsAccessMapping = service.GetAccessMapping(requestContext, AccessMappingConstants.VstsAccessMapping);
          AccessMapping clientAccessMapping = service.DetermineAccessMapping(requestContext);
          if (vstsAccessMapping != null && clientAccessMapping != null && (string.Equals(clientAccessMapping.Moniker, AccessMappingConstants.VstsAccessMapping, StringComparison.OrdinalIgnoreCase) || string.Equals(clientAccessMapping.Moniker, AccessMappingConstants.DevOpsAccessMapping, StringComparison.OrdinalIgnoreCase) || string.Equals(clientAccessMapping.Moniker, vstsAccessMapping.AccessPoint, StringComparison.OrdinalIgnoreCase)))
          {
            bool invalidSessionCache = false;
            using (TaskResourceComponent trc = requestContext.CreateComponent<TaskResourceComponent>())
            {
              TaskAgent agent = (await trc.GetAgentsByIdAsync(poolId, (IEnumerable<int>) new int[1]
              {
                session.Agent.Id
              })).FirstOrDefault<TaskAgent>();
              if (agent != null)
              {
                if (string.Equals(clientAccessMapping.Moniker, vstsAccessMapping.AccessPoint, StringComparison.OrdinalIgnoreCase))
                  agent.AccessPoint = AccessMappingConstants.VstsAccessMapping;
                else
                  agent.AccessPoint = clientAccessMapping.Moniker;
                UpdateAgentResult updateAgentResult = await trc.UpdateAgentAsync(poolId, agent, TaskAgentCapabilityType.None);
                invalidSessionCache = true;
              }
            }
            if (invalidSessionCache)
              requestContext.GetService<ITeamFoundationSqlNotificationService>().SendNotification(requestContext, "DistributedTask", SqlNotificationEventIds.SessionDeleted, session.SessionId.ToString("D"));
          }
          else
            requestContext.TraceError(10015152, "ResourceService", "Expect agent access mapping moniker to be either '" + AccessMappingConstants.VstsAccessMapping + "' or '" + AccessMappingConstants.DevOpsAccessMapping + "' or '" + vstsAccessMapping?.AccessPoint + "', current moniker is '" + (clientAccessMapping?.Moniker ?? string.Empty) + "'.");
          vstsAccessMapping = (AccessMapping) null;
          clientAccessMapping = (AccessMapping) null;
        }
        if (requestContext.IsFeatureEnabled("DistributedTask.DeprecateLegacyAgent") && DemandMinimumVersion.ParseVersion(session.Agent.Version).Major < 2 && this.GetAgent(requestContext, poolId, session.Agent.Id, includeAssignedRequest: true).AssignedRequest == null)
          this.DeleteSession(requestContext, poolId, sessionId);
        TaskAgentMessage agentMessage = (await requestContext.GetService<ITeamFoundationMessageQueueService>().GetMessageAsync(requestContext, session.QueueName, sessionId, timeout, lastMessageId)).ToAgentMessage();
        if (agentMessage != null && new PackageVersion(session.Agent.Version).CompareTo(DistributedTaskResourceService.s_coreAgentEncryptionVersion) < 0 && agentMessage.IV != null)
        {
          byte[] agentEncryptionKey = DistributedTaskResourceService.GetAgentEncryptionKey(requestContext, poolId, session.Agent.Id);
          using (AesCryptoServiceProvider cryptoServiceProvider = new AesCryptoServiceProvider())
          {
            cryptoServiceProvider.Key = agentEncryptionKey;
            cryptoServiceProvider.IV = agentMessage.IV;
            cryptoServiceProvider.Mode = CipherMode.CBC;
            cryptoServiceProvider.Padding = PaddingMode.PKCS7;
            using (ICryptoTransform decryptor = cryptoServiceProvider.CreateDecryptor())
            {
              using (MemoryStream memoryStream = new MemoryStream(Convert.FromBase64String(agentMessage.Body)))
              {
                using (CryptoStream cryptoStream = new CryptoStream((Stream) memoryStream, decryptor, CryptoStreamMode.Read))
                {
                  using (StreamReader streamReader = new StreamReader((Stream) cryptoStream, Encoding.UTF8))
                    agentMessage.Body = streamReader.ReadToEnd();
                  agentMessage.IV = (byte[]) null;
                }
              }
            }
          }
        }
        messageAsync = agentMessage;
      }
      return messageAsync;
    }

    public TaskPackageMetadata GetPackage(
      IVssRequestContext requestContext,
      string packageType,
      bool includeUrl = true,
      string version = null)
    {
      this.CheckServerReadPermission(requestContext);
      return new TaskPackageMetadata(TaskAgentConstants.AgentPackageType, AgentConstants.Version);
    }

    public IList<TaskPackageMetadata> GetPackages(
      IVssRequestContext requestContext,
      string packageType = null)
    {
      this.CheckServerReadPermission(requestContext);
      return (IList<TaskPackageMetadata>) Array.Empty<TaskPackageMetadata>();
    }

    public void WritePackageFile(IVssRequestContext requestContext, string package, Stream stream)
    {
      using (new MethodScope(requestContext, "ResourceService", nameof (WritePackageFile)))
      {
        requestContext.CheckOnPremisesDeployment();
        string str = requestContext.GetService<IVssRegistryService>().GetValue<string>(requestContext, (RegistryQuery) "/Service/DistributedTask/Settings/PackageLocation", true, Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), TaskAgentConstants.OfflineAgentsDirectory));
        if (!Directory.Exists(str))
          throw new AgentFileNotFoundException(TaskResources.AgentFileNotFound((object) str));
        string fullPath;
        try
        {
          fullPath = Path.GetFullPath(Path.Combine(str, package));
        }
        catch (Exception ex)
        {
          throw new AgentFileNotFoundException(TaskResources.AgentFileNotFound((object) str));
        }
        if (FileSpec.GetRelativeStartIndex(fullPath, str) <= 0 || !System.IO.File.Exists(fullPath))
          throw new AgentFileNotFoundException(TaskResources.AgentFileNotFound((object) fullPath));
        using (SmartPushStreamContentStream destination = new SmartPushStreamContentStream(stream))
        {
          using (FileStream fileStream = new FileStream(fullPath, FileMode.Open, FileAccess.Read))
            fileStream.CopyTo((Stream) destination);
        }
      }
    }

    public void SendJobMessageToAgent(
      IVssRequestContext requestContext,
      int poolId,
      long requestId,
      TaskAgentMessage message)
    {
      requestContext = requestContext.ToPoolRequestContext();
      this.EnsureMessageQueueStarted(requestContext);
      using (new MethodScope(requestContext, "ResourceService", nameof (SendJobMessageToAgent)))
      {
        this.CheckViewAndOtherPermissionsForPool(requestContext, poolId, 2);
        TaskAgentJobRequest agentRequest;
        using (TaskResourceComponent component = requestContext.CreateComponent<TaskResourceComponent>())
          agentRequest = component.GetAgentRequest(poolId, requestId)?.AgentRequest;
        if (agentRequest == null)
          throw new TaskAgentJobNotFoundException(TaskResources.AgentRequestNotFound((object) requestId));
        if (agentRequest.FinishTime.HasValue)
          throw new TaskAgentJobTokenExpiredException(TaskResources.AgentRequestExpired((object) requestId));
        DistributedTaskResourceService.SendMessageToAgent(requestContext, poolId, agentRequest.ReservedAgent.Id, message);
      }
    }

    public async Task SendJobMessageToAgentAsync(
      IVssRequestContext requestContext,
      int poolId,
      long requestId,
      TaskAgentMessage message)
    {
      requestContext.AssertAsyncExecutionEnabled();
      requestContext = requestContext.ToPoolRequestContext();
      this.EnsureMessageQueueStarted(requestContext);
      MethodScope methodScope = new MethodScope(requestContext, "ResourceService", nameof (SendJobMessageToAgentAsync));
      try
      {
        this.CheckViewAndOtherPermissionsForPool(requestContext, poolId, 2);
        TaskAgentJobRequest agentRequest;
        using (TaskResourceComponent trc = requestContext.CreateComponent<TaskResourceComponent>())
          agentRequest = (await trc.GetAgentRequestAsync(poolId, requestId))?.AgentRequest;
        if (agentRequest == null)
          throw new TaskAgentJobNotFoundException(TaskResources.AgentRequestNotFound((object) requestId));
        if (agentRequest.FinishTime.HasValue)
          throw new TaskAgentJobTokenExpiredException(TaskResources.AgentRequestExpired((object) requestId));
        DistributedTaskResourceService.SendMessageToAgent(requestContext, poolId, agentRequest.ReservedAgent.Id, message);
      }
      finally
      {
        methodScope.Dispose();
      }
      methodScope = new MethodScope();
    }

    public async Task SendRefreshMessageToAgentAsync(
      IVssRequestContext requestContext,
      int poolId,
      int agentId)
    {
      DistributedTaskResourceService taskResourceService = this;
      requestContext = requestContext.ToPoolRequestContext();
      taskResourceService.EnsureMessageQueueStarted(requestContext);
      MethodScope methodScope = new MethodScope(requestContext, "ResourceService", nameof (SendRefreshMessageToAgentAsync));
      try
      {
        taskResourceService.CheckViewAndOtherPermissionsForPool(requestContext, poolId, 2);
        taskResourceService.ThrowIfHostedPool(requestContext, poolId);
        // ISSUE: explicit non-virtual call
        TaskAgent agent = await __nonvirtual (taskResourceService.GetAgentAsync(requestContext, poolId, agentId, false, false, false, (IList<string>) null));
        if (agent == null)
          throw new TaskAgentNotFoundException(TaskResources.AgentNotFound((object) poolId, (object) agentId));
        TaskAgentUpdateReasonData reason = new TaskAgentUpdateReasonData()
        {
          Reason = TaskAgentUpdateReasonType.Manual
        };
        RequestAgentsUpdateResult agentsInternalAsync = await taskResourceService.SendRefreshMessageToAgentsInternalAsync(requestContext, poolId, (IList<TaskAgent>) new TaskAgent[1]
        {
          agent
        }, reason);
        if (agentsInternalAsync.NewUpdates.Count == 0 && agentsInternalAsync.ExistingUpdates.Count == 1)
        {
          TaskAgent existingUpdate = agentsInternalAsync.ExistingUpdates[0];
          throw new TaskAgentPendingUpdateExistsException(TaskResources.AgentPendingUpdateExists((object) existingUpdate.PendingUpdate.RequestedBy.DisplayName, (object) agent.Name, (object) existingUpdate.PendingUpdate.RequestTime));
        }
        agent = (TaskAgent) null;
      }
      finally
      {
        methodScope.Dispose();
      }
      methodScope = new MethodScope();
    }

    public async Task SendRefreshMessageToAgentsAsync(IVssRequestContext requestContext, int poolId)
    {
      requestContext = requestContext.ToPoolRequestContext();
      this.EnsureMessageQueueStarted(requestContext);
      MethodScope methodScope = new MethodScope(requestContext, "ResourceService", nameof (SendRefreshMessageToAgentsAsync));
      try
      {
        this.CheckViewAndOtherPermissionsForPool(requestContext, poolId, 2);
        this.ThrowIfHostedPool(requestContext, poolId);
        TaskAgentUpdateReasonData reason = new TaskAgentUpdateReasonData()
        {
          Reason = TaskAgentUpdateReasonType.Manual
        };
        RequestAgentsUpdateResult agentsInternalAsync = await this.SendRefreshMessageToAgentsInternalAsync(requestContext, poolId, this.GetAgents(requestContext, poolId), reason);
      }
      finally
      {
        methodScope.Dispose();
      }
      methodScope = new MethodScope();
    }

    public TaskAgent UpdateAgent(
      IVssRequestContext requestContext,
      int poolId,
      TaskAgent agent,
      TaskAgentCapabilityType capabilityUpdate = TaskAgentCapabilityType.System | TaskAgentCapabilityType.User)
    {
      requestContext = requestContext.ToPoolRequestContext();
      this.EnsureMessageQueueStarted(requestContext);
      using (new MethodScope(requestContext, "ResourceService", nameof (UpdateAgent)))
      {
        ArgumentValidation.CheckAgent(agent, nameof (agent), false);
        this.CheckViewAndOtherPermissionsForPool(requestContext, poolId, 2);
        TaskAgentPool agentPool = this.GetAgentPool(requestContext, poolId, (IList<string>) null, TaskAgentPoolActionFilter.None);
        if (agentPool == null)
          throw new TaskAgentPoolNotFoundException(TaskResources.AgentPoolNotFound((object) poolId));
        TaskAgent agentBeforeUpdate = (TaskAgent) null;
        if (!agentPool.IsHosted && agentPool.PoolType != TaskAgentPoolType.Deployment)
        {
          bool? enabled = agent.Enabled;
          bool flag = true;
          if (!(enabled.GetValueOrDefault() == flag & enabled.HasValue))
            goto label_19;
        }
        agentBeforeUpdate = this.GetAgent(requestContext, poolId, agent.Id, false, false, false, (IList<string>) null);
        if (agentBeforeUpdate == null)
          throw new TaskAgentNotFoundException(TaskResources.AgentNotFound((object) poolId, (object) agent.Id));
        if (agentPool.IsHosted)
        {
          bool flag1 = false;
          if (agent.Authorization != null)
          {
            int num1 = flag1 | agent.Authorization.PublicKey != null ? 1 : 0;
            int num2;
            if (agent.Authorization.ClientId != Guid.Empty)
            {
              Guid clientId = agent.Authorization.ClientId;
              TaskAgentAuthorization authorization = agentBeforeUpdate.Authorization;
              Guid guid = authorization != null ? authorization.ClientId : Guid.Empty;
              num2 = clientId != guid ? 1 : 0;
            }
            else
              num2 = 0;
            flag1 = (num1 | num2) != 0;
          }
          bool flag2 = (capabilityUpdate & TaskAgentCapabilityType.System) == TaskAgentCapabilityType.System;
          bool flag3 = !string.IsNullOrEmpty(agent.Name) && !agent.Name.Equals(agentBeforeUpdate.Name, StringComparison.Ordinal);
          int num3;
          if (agent.MaxParallelism.HasValue)
          {
            int? maxParallelism1 = agent.MaxParallelism;
            int num4 = 0;
            if (!(maxParallelism1.GetValueOrDefault() == num4 & maxParallelism1.HasValue))
            {
              maxParallelism1 = agent.MaxParallelism;
              int? maxParallelism2 = agentBeforeUpdate.MaxParallelism;
              num3 = !(maxParallelism1.GetValueOrDefault() == maxParallelism2.GetValueOrDefault() & maxParallelism1.HasValue == maxParallelism2.HasValue) ? 1 : 0;
              goto label_17;
            }
          }
          num3 = 0;
label_17:
          bool flag4 = num3 != 0;
          bool flag5 = !string.IsNullOrEmpty(agent.Version) && !agent.Version.Equals(agentBeforeUpdate.Version, StringComparison.Ordinal);
          if (flag1 | flag2 | flag3 | flag4 | flag5)
            this.CheckHostedPoolPermissions(requestContext, agentPool, agent);
        }
label_19:
        this.ClearAgentCapabilitiesIfHosted(requestContext, poolId, agent);
        this.PopulateAgentAccessMapping(requestContext, (TaskAgentReference) agent);
        UpdateAgentResult updateAgentResult;
        using (TaskResourceComponent component = requestContext.CreateComponent<TaskResourceComponent>())
        {
          updateAgentResult = component.UpdateAgent(poolId, agent, capabilityUpdate);
          updateAgentResult.Agent.Properties = agent.Properties;
        }
        if (updateAgentResult.Agent.Properties != null && updateAgentResult.Agent.Properties.Count > 0)
          requestContext.GetService<ITeamFoundationPropertyService>().SetProperties(requestContext, updateAgentResult.Agent.CreateSpec(), updateAgentResult.Agent.Properties.Convert());
        updateAgentResult.Agent.PopulateReferenceLinks<TaskAgent>(requestContext, poolId);
        TaskAgent agent1 = updateAgentResult.Agent.Clone();
        updateAgentResult.Agent.PopulateProperties(requestContext, updateAgentResult.PoolData);
        if (agent.Authorization != null && agent.Authorization.PublicKey != null && updateAgentResult.Agent.Authorization != null && updateAgentResult.Agent.Authorization.PublicKey != null)
        {
          IVssRequestContext context = requestContext.To(TeamFoundationHostType.Application);
          IVssRequestContext vssRequestContext = requestContext.Elevate();
          Microsoft.VisualStudio.Services.Identity.Identity identity = this.ProvisionServiceIdentity(requestContext, updateAgentResult.PoolData, AgentPoolServiceAccountRoles.AgentPoolService);
          Microsoft.VisualStudio.Services.DelegatedAuthorization.Registration registration1 = new Microsoft.VisualStudio.Services.DelegatedAuthorization.Registration()
          {
            ClientType = ClientType.MediumTrust,
            IdentityId = identity.Id,
            IsValid = true,
            PublicKey = updateAgentResult.Agent.Authorization.PublicKey.ToXmlString(),
            RegistrationId = updateAgentResult.Agent.Authorization.ClientId,
            RegistrationName = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Task Agent Pool {0} - Agent {1}", (object) poolId, (object) updateAgentResult.Agent.Id),
            Scopes = "vso.agentpools_listen"
          };
          IDelegatedAuthorizationRegistrationService service = context.GetService<IDelegatedAuthorizationRegistrationService>();
          Microsoft.VisualStudio.Services.DelegatedAuthorization.Registration registration2;
          try
          {
            registration2 = service.Update(context.Elevate(), registration1);
          }
          catch (RegistrationNotFoundException ex)
          {
            registration2 = service.Create(context.Elevate(), registration1);
          }
          if (registration2 != null)
            vssRequestContext.GetService<IDelegatedAuthorizationService>().AuthorizeHost(vssRequestContext, registration2.RegistrationId);
          ILocationDataProvider locationData = requestContext.GetService<ILocationService>().GetLocationData(requestContext, new Guid("585028FE-17D8-49E2-9A1B-EFB4D8502156"));
          updateAgentResult.Agent.Authorization.AuthorizationUrl = locationData.GetResourceUri(requestContext, "oauth2", OAuth2ResourceIds.Token, (object) null, false);
        }
        string queueName = MessageQueueHelpers.GetQueueName(poolId, agent.Id);
        ITeamFoundationMessageQueueService service1 = requestContext.GetService<ITeamFoundationMessageQueueService>();
        if (!service1.QueueExists(requestContext, queueName))
        {
          try
          {
            service1.CreateQueue(requestContext, queueName, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Message queue for distributed task agent {0} in pool {1}", (object) updateAgentResult.Agent.Name, (object) poolId));
          }
          catch (MessageQueueAlreadyExistsException ex)
          {
          }
        }
        if (updateAgentResult.RecalculateRequestMatches)
          DistributedTaskResourceService.QueueAgentRematchJob(requestContext);
        if (string.Equals(agent.ProvisioningState, "Deallocated", StringComparison.OrdinalIgnoreCase))
          this.QueueRequestAssignmentJob(requestContext);
        if (!string.IsNullOrEmpty(agent.ProvisioningState))
          requestContext.TraceAlways(10015227, "ResourceService", string.Format("Updating provision state of agent={0}, poolId={1} to state={2}", (object) updateAgentResult.Agent.Id, (object) poolId, (object) agent.ProvisioningState));
        this.GetTaskAgentPoolExtension(requestContext, poolId).AgentUpdated(requestContext, poolId, agent1, agentBeforeUpdate);
        requestContext.GetService<IDistributedTaskEventPublisherService>().NotifyAgentChangeEvent(requestContext, "MS.TF.DistributedTask.AgentUpdated", agentPool, agent1);
        return updateAgentResult.Agent;
      }
    }

    public async Task FinishAgentRequestAsync(
      IVssRequestContext requestContext,
      int poolId,
      long requestId,
      TaskResult? jobResult = null,
      bool agentShuttingDown = false,
      bool callSprocEvenIfPoolNotFound = false)
    {
      DistributedTaskResourceService taskResourceService = this;
      requestContext.AssertAsyncExecutionEnabled();
      requestContext = requestContext.ToPoolRequestContext();
      taskResourceService.EnsureMessageQueueStarted(requestContext);
      MethodScope methodScope = new MethodScope(requestContext, "ResourceService", nameof (FinishAgentRequestAsync));
      try
      {
        taskResourceService.CheckViewAndOtherPermissionsForPool(requestContext, poolId, 2);
        TaskAgentPoolData pool = (await taskResourceService.GetTaskAgentPoolInternalAsync(requestContext, (IList<int>) new int[1]
        {
          poolId
        })).FirstOrDefault<TaskAgentPoolData>();
        if (pool == null && !callSprocEvenIfPoolNotFound)
          throw new TaskAgentPoolNotFoundException(TaskResources.AgentPoolNotFound((object) poolId));
        IInternalAgentCloudService agentCloudService = requestContext.GetService<IInternalAgentCloudService>();
        IServerPoolProvider serverPoolProvider1;
        if (pool != null)
          serverPoolProvider1 = await agentCloudService.GetPoolProviderForPoolAsync(requestContext, pool);
        else
          serverPoolProvider1 = (IServerPoolProvider) null;
        IServerPoolProvider currentPoolProvider = serverPoolProvider1;
        FinishAgentRequestResult result = (FinishAgentRequestResult) null;
        agentShuttingDown = agentShuttingDown && !requestContext.IsFeatureEnabled("DistributedTask.DisableAgentOfflineWhenShuttingDown");
        using (TaskResourceComponent resourceComponent1 = requestContext.CreateComponent<TaskResourceComponent>())
        {
          TaskResourceComponent resourceComponent2 = resourceComponent1;
          int poolId1 = poolId;
          long requestId1 = requestId;
          IServerPoolProvider serverPoolProvider2 = currentPoolProvider;
          int num1 = serverPoolProvider2 != null ? (serverPoolProvider2.SingleJobPerRequest ? 1 : 0) : 1;
          TaskResult? jobResult1 = jobResult;
          int num2 = agentShuttingDown ? 1 : 0;
          result = await resourceComponent2.FinishAgentRequestAsync(poolId1, requestId1, num1 != 0, false, jobResult1, num2 != 0);
        }
        bool delivered = false;
        if (result.OrchestrationEvent != null)
        {
          delivered = await agentCloudService.DeliverEventAsync(requestContext, (RunAgentEvent) result.OrchestrationEvent, true);
          taskResourceService.TraceAgentCloudOrchestrationEventsFromSprocs(requestContext, "prc_FinishAgentRequest", (RunAgentEvent) result.OrchestrationEvent, delivered, result.CompletedRequest);
        }
        if (pool == null)
        {
          requestContext.TraceAlways(10015266, "ResourceService", string.Format("Attempted to finish AgentRequest {0} whose pool ({1}) no longer exists. Finished={2}", (object) requestId, (object) poolId, (object) (result.CompletedRequest != null)));
          return;
        }
        if (result.CompletedRequest != null)
        {
          KPIHelper.PublishDTJobCompleted(requestContext);
          string projectName = (string) null;
          if (result.CompletedRequest.ScopeId != Guid.Empty && requestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection))
            requestContext.GetService<IProjectService>().TryGetProjectName(requestContext.Elevate(), result.CompletedRequest.ScopeId, out projectName);
          DistributedTaskEventSource.Log.PublishAgentPoolRequestHistory(requestContext.ServiceHost.InstanceId, pool.Pool.Name, projectName, result.CompletedRequest, result.CompletedRequestCloud);
          ITaskAgentExtension taskAgentExtension = taskResourceService.GetTaskAgentExtension(requestContext, poolId);
          IVssRequestContext requestContext1 = requestContext;
          int poolId2 = poolId;
          TaskAgentJobRequest completedRequest = result.CompletedRequest;
          TaskAgentCloud completedRequestCloud = result.CompletedRequestCloud;
          int num;
          if (completedRequestCloud == null)
          {
            num = 0;
          }
          else
          {
            int agentCloudId = completedRequestCloud.AgentCloudId;
            num = 1;
          }
          taskAgentExtension.JobCompleted(requestContext1, poolId2, completedRequest, num != 0);
          taskResourceService.GetTaskAgentPoolExtension(requestContext, poolId).AgentRequestCompleted(requestContext, poolId, result.CompletedRequest.PopulateReferenceLinks(requestContext, poolId));
          taskResourceService.QueueRequestBilling(requestContext, currentPoolProvider, result.CompletedRequest);
          if (!delivered && result.CompletedRequest != null && result.CompletedRequest.ReservedAgent != null && string.Equals(result.CompletedRequest.ReservedAgent.ProvisioningState, "Deprovisioning"))
          {
            IServerPoolProvider providerForAgentCloud = agentCloudService.GetPoolProviderForAgentCloud(requestContext, result.CompletedRequestCloud, pool);
            taskResourceService.QueueAgentDeprovision(requestContext, providerForAgentCloud, poolId, result.CompletedRequest.ReservedAgent);
          }
          if (result.CompletedRequest.ReservedAgent == null)
          {
            try
            {
              JobCompletedEvent eventData = new JobCompletedEvent(result.CompletedRequest.RequestId, result.CompletedRequest.JobId, result.CompletedRequest.Result.Value);
              taskResourceService.RaiseEvent<JobCompletedEvent>(requestContext, result.CompletedRequest.ServiceOwner, result.CompletedRequest.HostId, result.CompletedRequest.ScopeId, result.CompletedRequest.PlanType, result.CompletedRequest.PlanId, eventData);
            }
            catch (OrchestrationSessionNotFoundException ex)
            {
              requestContext.TraceAlways(10015270, TraceLevel.Warning, "DistributedTask", "ResourceService", "Failed to raise JobCompletedEvent because the orchestration no longer exists in DB: " + ex.Message);
            }
            catch (TaskOrchestrationPlanNotFoundException ex)
            {
              requestContext.TraceError(10015132, "ResourceService", "Plan {0} not found when sending JobCompleted event for the abandoned Job {1} on pool {2}", (object) result.CompletedRequest.PlanId, (object) result.CompletedRequest.RequestId, (object) poolId);
            }
          }
        }
        if (result.CompletedRequest != null)
        {
          // ISSUE: explicit non-virtual call
          __nonvirtual (taskResourceService.QueueRequestAssignmentJob(requestContext));
        }
        pool = (TaskAgentPoolData) null;
        agentCloudService = (IInternalAgentCloudService) null;
        currentPoolProvider = (IServerPoolProvider) null;
        result = (FinishAgentRequestResult) null;
      }
      finally
      {
        methodScope.Dispose();
      }
      methodScope = new MethodScope();
    }

    public TaskAgentPoolMaintenanceDefinition CreateAgentPoolMaintenanceDefinition(
      IVssRequestContext requestContext,
      int poolId,
      TaskAgentPoolMaintenanceDefinition definition)
    {
      requestContext = requestContext.ToPoolRequestContext();
      using (new MethodScope(requestContext, "ResourceService", nameof (CreateAgentPoolMaintenanceDefinition)))
      {
        this.CheckViewAndOtherPermissionsForPool(requestContext, poolId, 2);
        this.ThrowIfHostedPool(requestContext, poolId);
        TaskAgentPoolMaintenanceDefinition maintenanceDefinition;
        using (TaskResourceComponent component = requestContext.CreateComponent<TaskResourceComponent>())
          maintenanceDefinition = component.CreateMaintenanceDefinition(poolId, definition);
        if (maintenanceDefinition != null)
          this.AddScheduleMaintenanceJob(requestContext, maintenanceDefinition);
        return maintenanceDefinition;
      }
    }

    public IList<TaskAgentPoolMaintenanceDefinition> GetAgentPoolMaintenanceDefinitions(
      IVssRequestContext requestContext,
      int poolId)
    {
      requestContext = requestContext.ToPoolRequestContext();
      using (new MethodScope(requestContext, "ResourceService", nameof (GetAgentPoolMaintenanceDefinitions)))
      {
        TaskAgentPool agentPool = this.GetAgentPool(requestContext, poolId, (IList<string>) null, TaskAgentPoolActionFilter.None);
        if (agentPool == null || agentPool.IsHosted || agentPool.AgentCloudId.HasValue)
          return (IList<TaskAgentPoolMaintenanceDefinition>) Array.Empty<TaskAgentPoolMaintenanceDefinition>();
        using (TaskResourceComponent component = requestContext.CreateComponent<TaskResourceComponent>())
          return component.GetAgentPoolMaintenanceDefinitions(poolId);
      }
    }

    public TaskAgentPoolMaintenanceDefinition GetAgentPoolMaintenanceDefinition(
      IVssRequestContext requestContext,
      int poolId,
      int definitionId)
    {
      requestContext = requestContext.ToPoolRequestContext();
      using (new MethodScope(requestContext, "ResourceService", nameof (GetAgentPoolMaintenanceDefinition)))
      {
        this.CheckViewAndOtherPermissionsForPool(requestContext, poolId, 1);
        TaskAgentPool agentPool = this.GetAgentPool(requestContext, poolId, (IList<string>) null, TaskAgentPoolActionFilter.None);
        if (agentPool == null || agentPool.IsHosted || agentPool.AgentCloudId.HasValue)
          return (TaskAgentPoolMaintenanceDefinition) null;
        using (TaskResourceComponent component = requestContext.CreateComponent<TaskResourceComponent>())
          return component.GetAgentPoolMaintenanceDefinition(poolId, definitionId).Definition;
      }
    }

    public ResourceUsage GetResourceUsage(
      IVssRequestContext requestContext,
      string parallelismTag,
      bool poolIsHosted,
      bool includeUsedCount,
      bool includeRunningRequests,
      int maxRequestsCount)
    {
      using (new MethodScope(requestContext, "ResourceService", nameof (GetResourceUsage)))
      {
        requestContext = requestContext.ToPoolRequestContext();
        this.Security.CheckTaskHubLicensePermission(requestContext, 1);
        Guid hostId = requestContext.ServiceHost.InstanceId;
        ResourceUsage resourceUsage = new ResourceUsage();
        string resourceType = ResourceLimitUtil.GetResourceThrottlingType(parallelismTag, poolIsHosted);
        PlatformTaskHubLicenseService service = requestContext.GetService<PlatformTaskHubLicenseService>();
        ResourceLimit resourceLimit = service.GetResourceLimits(requestContext, true).FirstOrDefault<ResourceLimit>((Func<ResourceLimit, bool>) (x => x.HostId == hostId && x.GetResourceType() == resourceType));
        resourceUsage.ResourceLimit = resourceLimit;
        List<TaskAgentJobRequest> runningRequests = (List<TaskAgentJobRequest>) null;
        if (includeUsedCount | includeRunningRequests)
        {
          using (TaskResourceComponent component = requestContext.ToPoolRequestContext().CreateComponent<TaskResourceComponent>())
            resourceUsage.UsedCount = new int?(component.GetResourceUsage(hostId, resourceType, poolIsHosted, includeRunningRequests, maxRequestsCount, out runningRequests));
          resourceUsage.RunningRequests.AddRange<TaskAgentJobRequest, IList<TaskAgentJobRequest>>((IEnumerable<TaskAgentJobRequest>) runningRequests);
        }
        if (poolIsHosted && string.Equals(parallelismTag, "Private", StringComparison.OrdinalIgnoreCase))
          resourceUsage.UsedMinutes = service.GetUsedHostedMinutesForPrivateProjects(requestContext);
        return resourceUsage;
      }
    }

    public TaskAgentPoolMaintenanceDefinition UpdateAgentPoolMaintenanceDefinition(
      IVssRequestContext requestContext,
      int poolId,
      int definitionId,
      TaskAgentPoolMaintenanceDefinition definition)
    {
      requestContext = requestContext.ToPoolRequestContext();
      using (new MethodScope(requestContext, "ResourceService", nameof (UpdateAgentPoolMaintenanceDefinition)))
      {
        this.CheckViewAndOtherPermissionsForPool(requestContext, poolId, 2);
        this.ThrowIfHostedPool(requestContext, poolId);
        TaskAgentPoolMaintenanceDefinition maintenanceDefinition;
        using (TaskResourceComponent component = requestContext.CreateComponent<TaskResourceComponent>())
          maintenanceDefinition = component.UpdateAgentPoolMaintenanceDefinition(poolId, definitionId, definition);
        if (maintenanceDefinition != null)
          this.UpdateScheduleMaintenanceJob(requestContext, maintenanceDefinition);
        return maintenanceDefinition;
      }
    }

    public void DeleteAgentPoolMaintenanceDefinition(
      IVssRequestContext requestContext,
      int poolId,
      int definitionId)
    {
      requestContext = requestContext.ToPoolRequestContext();
      using (new MethodScope(requestContext, "ResourceService", nameof (DeleteAgentPoolMaintenanceDefinition)))
      {
        this.CheckViewAndOtherPermissionsForPool(requestContext, poolId, 2);
        this.ThrowIfHostedPool(requestContext, poolId);
        DeletePoolMaintenanceDefinitionResult definitionResult;
        using (TaskResourceComponent component = requestContext.CreateComponent<TaskResourceComponent>())
          definitionResult = component.DeleteAgentPoolMaintenanceDefinitions(poolId, (IEnumerable<int>) new int[1]
          {
            definitionId
          });
        if (definitionResult == null)
          return;
        this.DeleteAgentPoolMaintenanceJobs(requestContext, poolId, definitionResult.DeletedPoolMaintenanceJobs.Select<TaskAgentPoolMaintenanceJob, int>((Func<TaskAgentPoolMaintenanceJob, int>) (x => x.JobId)));
        this.DeleteScheduleMaintenanceJob(requestContext, definitionResult.DeletedPoolMaintenanceDefinitions);
      }
    }

    public void DeleteAgentPoolMaintenanceJobs(
      IVssRequestContext requestContext,
      int poolId,
      IEnumerable<int> jobIds)
    {
      requestContext = requestContext.ToPoolRequestContext();
      using (new MethodScope(requestContext, "ResourceService", nameof (DeleteAgentPoolMaintenanceJobs)))
      {
        this.CheckViewAndOtherPermissionsForPool(requestContext, poolId, 2);
        this.ThrowIfHostedPool(requestContext, poolId);
        IList<TaskAgentPoolMaintenanceJob> source;
        using (TaskResourceComponent component = requestContext.CreateComponent<TaskResourceComponent>())
          source = component.DeleteAgentPoolMaintenanceJobs(poolId, jobIds);
        if (source == null)
          return;
        this.GetPoolMaintenanceTaskHub(requestContext).DeletePlans(requestContext.Elevate(), Guid.Empty, source.Select<TaskAgentPoolMaintenanceJob, Guid>((Func<TaskAgentPoolMaintenanceJob, Guid>) (x => x.OrchestrationId)));
      }
    }

    public IList<TaskAgentPoolMaintenanceJob> GetAgentPoolMaintenanceJobs(
      IVssRequestContext requestContext,
      int poolId,
      int? defintionId = null)
    {
      requestContext = requestContext.ToPoolRequestContext();
      using (new MethodScope(requestContext, "ResourceService", nameof (GetAgentPoolMaintenanceJobs)))
      {
        this.CheckViewAndOtherPermissionsForPool(requestContext, poolId, 1);
        TaskAgentPool agentPool = this.GetAgentPool(requestContext, poolId, (IList<string>) null, TaskAgentPoolActionFilter.None);
        if (agentPool != null && !agentPool.IsHosted)
        {
          int? nullable = agentPool.AgentCloudId;
          if (!nullable.HasValue)
          {
            IList<TaskAgentPoolMaintenanceJob> poolMaintenanceJobs1;
            using (TaskResourceComponent component = requestContext.CreateComponent<TaskResourceComponent>())
            {
              TaskResourceComponent resourceComponent = component;
              int poolId1 = poolId;
              nullable = new int?();
              int? defintionId1 = nullable;
              poolMaintenanceJobs1 = resourceComponent.GetAgentPoolMaintenanceJobs(poolId1, defintionId1);
            }
            IList<TaskAgentPoolMaintenanceJob> poolMaintenanceJobs2 = (IList<TaskAgentPoolMaintenanceJob>) new List<TaskAgentPoolMaintenanceJob>();
            foreach (TaskAgentPoolMaintenanceJob maintenanceJob in (IEnumerable<TaskAgentPoolMaintenanceJob>) poolMaintenanceJobs1)
              poolMaintenanceJobs2.Add(this.PopulateMaintenanceJobData(requestContext, maintenanceJob));
            return poolMaintenanceJobs2;
          }
        }
        return (IList<TaskAgentPoolMaintenanceJob>) Array.Empty<TaskAgentPoolMaintenanceJob>();
      }
    }

    public TaskAgentPoolMaintenanceJob GetAgentPoolMaintenanceJob(
      IVssRequestContext requestContext,
      int poolId,
      int jobId)
    {
      requestContext = requestContext.ToPoolRequestContext();
      using (new MethodScope(requestContext, "ResourceService", nameof (GetAgentPoolMaintenanceJob)))
      {
        this.CheckViewAndOtherPermissionsForPool(requestContext, poolId, 1);
        TaskAgentPool agentPool = this.GetAgentPool(requestContext, poolId, (IList<string>) null, TaskAgentPoolActionFilter.None);
        if (agentPool == null || agentPool.IsHosted || agentPool.AgentCloudId.HasValue)
          return (TaskAgentPoolMaintenanceJob) null;
        TaskAgentPoolMaintenanceJob maintenanceJob = (TaskAgentPoolMaintenanceJob) null;
        using (TaskResourceComponent component = requestContext.CreateComponent<TaskResourceComponent>())
          maintenanceJob = component.GetAgentPoolMaintenanceJob(poolId, jobId);
        return this.PopulateMaintenanceJobData(requestContext, maintenanceJob);
      }
    }

    public void GetAgentPoolMaintenanceJobLogs(
      IVssRequestContext requestContext,
      int poolId,
      int jobId,
      Stream outputSteam)
    {
      requestContext = requestContext.ToPoolRequestContext();
      using (new MethodScope(requestContext, "ResourceService", nameof (GetAgentPoolMaintenanceJobLogs)))
      {
        this.CheckViewAndOtherPermissionsForPool(requestContext, poolId, 1);
        TaskAgentPool agentPool = this.GetAgentPool(requestContext, poolId, (IList<string>) null, TaskAgentPoolActionFilter.None);
        if (agentPool == null || agentPool.IsHosted || agentPool.AgentCloudId.HasValue)
          return;
        TaskAgentPoolMaintenanceJob poolMaintenanceJob = (TaskAgentPoolMaintenanceJob) null;
        using (TaskResourceComponent component = requestContext.CreateComponent<TaskResourceComponent>())
          poolMaintenanceJob = component.GetAgentPoolMaintenanceJob(poolId, jobId);
        if (poolMaintenanceJob == null)
          return;
        TaskHub maintenanceTaskHub = this.GetPoolMaintenanceTaskHub(requestContext);
        TaskOrchestrationPlan plan = maintenanceTaskHub.GetPlan(requestContext, Guid.Empty, poolMaintenanceJob.OrchestrationId);
        if (plan == null)
          throw new TaskOrchestrationPlanNotFoundException(Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.TaskResources.PlanNotFound((object) poolMaintenanceJob.OrchestrationId));
        Dictionary<int, TaskLog> dictionary1 = maintenanceTaskHub.GetLogs(requestContext, plan.ScopeIdentifier, plan.PlanId).ToDictionary<TaskLog, int>((Func<TaskLog, int>) (log => log.Id));
        IEnumerable<Timeline> timelines = maintenanceTaskHub.GetTimelines(requestContext, plan.ScopeIdentifier, plan.PlanId);
        if (dictionary1 == null || timelines == null)
          return;
        Dictionary<string, List<string>> dictionary2 = new Dictionary<string, List<string>>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        using (ZipArchive zipArchive = new ZipArchive((Stream) new PositionedStreamWrapper(outputSteam), ZipArchiveMode.Create))
        {
          zipArchive.CreateEntry("MaintenanceDetails/");
          foreach (Timeline timeline1 in timelines)
          {
            Timeline timeline2 = maintenanceTaskHub.GetTimeline(requestContext, plan.ScopeIdentifier, plan.PlanId, timeline1.Id, includeRecords: true);
            if (timeline2 != null)
            {
              foreach (TimelineRecord record in timeline2.Records)
              {
                if (string.IsNullOrEmpty(record.WorkerName))
                {
                  if (!dictionary2.ContainsKey(TaskResources.Issues()))
                    dictionary2[TaskResources.Issues()] = new List<string>();
                  if (record.Issues.Count > 0)
                  {
                    foreach (Issue issue in record.Issues)
                      dictionary2[TaskResources.Issues()].Add(issue.Message);
                  }
                }
                else
                {
                  if (!dictionary2.ContainsKey(record.WorkerName))
                    dictionary2[record.WorkerName] = new List<string>();
                  if (record.Issues.Count > 0)
                  {
                    foreach (Issue issue in record.Issues)
                      dictionary2[record.WorkerName].Add(issue.Message);
                  }
                  TaskLog taskLog;
                  if (string.Equals(record.RecordType, "Job", StringComparison.OrdinalIgnoreCase) && record.Log != null && dictionary1.TryGetValue(record.Log.Id, out taskLog) && taskLog.LineCount > 0L)
                  {
                    string str = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}.txt", (object) FileSpec.RemoveInvalidFileNameChars(record.WorkerName));
                    ZipArchiveEntry entry = zipArchive.CreateEntry("MaintenanceDetails/" + str);
                    long startLine = 0;
                    long lineCount = taskLog.LineCount;
                    long totalLines = 0;
                    using (TeamFoundationDataReader logLines = maintenanceTaskHub.GetLogLines(requestContext, plan.ScopeIdentifier, plan.PlanId, taskLog.Id, (ISecuredObject) null, ref startLine, ref lineCount, out totalLines))
                    {
                      using (Stream stream = entry.Open())
                      {
                        using (StreamWriter streamWriter = new StreamWriter(stream))
                        {
                          foreach (string current in logLines.CurrentEnumerable<string>())
                            streamWriter.WriteLine(current);
                        }
                      }
                    }
                  }
                }
              }
            }
          }
          using (Stream stream = zipArchive.CreateEntry("MaintenanceSummary.txt").Open())
          {
            using (StreamWriter streamWriter = new StreamWriter(stream))
            {
              foreach (KeyValuePair<string, List<string>> keyValuePair in dictionary2)
              {
                streamWriter.WriteLine(keyValuePair.Key + ": ");
                if (keyValuePair.Value.Count == 0)
                {
                  streamWriter.WriteLine(TaskResources.NoErrorAndWarning());
                }
                else
                {
                  foreach (string str in keyValuePair.Value)
                    streamWriter.WriteLine("   " + str);
                }
              }
            }
          }
        }
      }
    }

    public IList<DeploymentPoolSummary> GetDeploymentPoolsSummary(
      IVssRequestContext requestContext,
      string poolName = null,
      IList<int> deploymentPoolIds = null,
      bool includeDeploymentGroupReferences = false,
      bool includeResource = false)
    {
      IVssRequestContext poolContext = requestContext.ToPoolRequestContext();
      IEnumerable<DeploymentPoolSummary> source1;
      if (deploymentPoolIds != null && deploymentPoolIds.Any<int>())
      {
        using (TaskResourceComponent component = poolContext.CreateComponent<TaskResourceComponent>())
          source1 = component.GetDeploymentPoolsSummaryById(deploymentPoolIds);
      }
      else
      {
        using (TaskResourceComponent component = poolContext.CreateComponent<TaskResourceComponent>())
          source1 = component.GetDeploymentPoolsSummary(poolName);
      }
      IAgentPoolSecurityProvider deploymentPoolSecurity = this.GetAgentPoolSecurity(requestContext, TaskAgentPoolType.Deployment);
      List<DeploymentPoolSummary> list1 = source1.Where<DeploymentPoolSummary>((Func<DeploymentPoolSummary, bool>) (poolSummary => deploymentPoolSecurity.HasPoolPermission(poolContext, poolSummary.Pool.Id, 1, true))).ToList<DeploymentPoolSummary>();
      if (includeDeploymentGroupReferences && list1.Any<DeploymentPoolSummary>())
      {
        Dictionary<int, IList<DeploymentGroupReference>> dictionary1 = new Dictionary<int, IList<DeploymentGroupReference>>();
        IList<DeploymentGroup> machineGroups;
        using (TaskResourceComponent component = requestContext.CreateComponent<TaskResourceComponent>())
          machineGroups = component.GetAgentQueuesByPoolIds(list1.Select<DeploymentPoolSummary, int>((Func<DeploymentPoolSummary, int>) (x => x.Pool.Id)), TaskAgentQueueType.Deployment).MachineGroups;
        List<Guid> wellFormedProjectIds = requestContext.GetService<IProjectService>().GetProjects(requestContext, ProjectState.WellFormed).Select<ProjectInfo, Guid>((Func<ProjectInfo, Guid>) (p => p.Id)).ToList<Guid>();
        IList<DeploymentGroup> list2 = (IList<DeploymentGroup>) machineGroups.Where<DeploymentGroup>((Func<DeploymentGroup, bool>) (d => wellFormedProjectIds.Contains(d.Project.Id))).ToList<DeploymentGroup>();
        Dictionary<int, DeploymentPoolSummary> dictionary2 = list1.ToDictionary<DeploymentPoolSummary, int>((Func<DeploymentPoolSummary, int>) (x => x.Pool.Id));
        foreach (IGrouping<int, DeploymentGroup> source2 in list2.GroupBy<DeploymentGroup, int>((Func<DeploymentGroup, int>) (dg => dg.Pool.Id)))
        {
          DeploymentPoolSummary deploymentPoolSummary;
          if (dictionary2.TryGetValue(source2.Key, out deploymentPoolSummary))
            deploymentPoolSummary.DeploymentGroups = (IList<DeploymentGroupReference>) source2.Select<DeploymentGroup, DeploymentGroupReference>((Func<DeploymentGroup, DeploymentGroupReference>) (x => x.PopulateProjectName(requestContext, x.Project.Id).AsReference())).ToList<DeploymentGroupReference>();
        }
      }
      if (includeResource && list1.Any<DeploymentPoolSummary>())
      {
        IList<VirtualMachineGroup> machineGroupsByPoolIds = requestContext.GetService<IVirtualMachineGroupService>().GetVirtualMachineGroupsByPoolIds(requestContext, list1.Select<DeploymentPoolSummary, int>((Func<DeploymentPoolSummary, int>) (x => x.Pool.Id)));
        Dictionary<int, DeploymentPoolSummary> dictionary = list1.ToDictionary<DeploymentPoolSummary, int>((Func<DeploymentPoolSummary, int>) (x => x.Pool.Id));
        foreach (IGrouping<int, VirtualMachineGroup> source3 in machineGroupsByPoolIds.GroupBy<VirtualMachineGroup, int>((Func<VirtualMachineGroup, int>) (vmg => vmg.PoolId)))
        {
          DeploymentPoolSummary deploymentPoolSummary;
          if (dictionary.TryGetValue(source3.Key, out deploymentPoolSummary))
          {
            VirtualMachineGroup virtualMachineGroup = source3.ToList<VirtualMachineGroup>().FirstOrDefault<VirtualMachineGroup>();
            deploymentPoolSummary.Resource = new EnvironmentResourceReference()
            {
              Id = virtualMachineGroup.Id,
              Name = virtualMachineGroup.Name,
              Type = virtualMachineGroup.Type
            };
          }
        }
      }
      return (IList<DeploymentPoolSummary>) list1;
    }

    public TaskAgentPoolMaintenanceJob QueueAgentPoolMaintenanceJob(
      IVssRequestContext requestContext,
      int poolId,
      int definitionId)
    {
      requestContext = requestContext.ToPoolRequestContext();
      this.EnsureMessageQueueStarted(requestContext);
      using (new MethodScope(requestContext, "ResourceService", nameof (QueueAgentPoolMaintenanceJob)))
      {
        this.CheckViewAndOtherPermissionsForPool(requestContext, poolId, 2);
        this.ThrowIfHostedPool(requestContext, poolId);
        TaskAgentPoolMaintenanceDefinition definition;
        TaskAgentPoolData poolData;
        using (TaskResourceComponent component = requestContext.CreateComponent<TaskResourceComponent>())
        {
          GetTaskAgentPoolMaintenanceDefinitionResult maintenanceDefinition = component.GetAgentPoolMaintenanceDefinition(poolId, definitionId);
          definition = maintenanceDefinition.Definition;
          poolData = maintenanceDefinition.PoolData;
        }
        if (definition == null || !definition.Enabled)
          throw new TaskAgentPoolMaintenanceNotEnabledException(TaskResources.AgentPoolMaintenanceNotEnabled((object) poolId));
        Microsoft.VisualStudio.Services.Identity.Identity identity = this.ProvisionServiceIdentity(requestContext, poolData, AgentPoolServiceAccountRoles.AgentPoolService);
        Microsoft.VisualStudio.Services.Identity.Identity userIdentity = requestContext.GetUserIdentity();
        TaskAgentPoolMaintenanceJob maintenanceJob = (TaskAgentPoolMaintenanceJob) null;
        using (TaskResourceComponent component = requestContext.CreateComponent<TaskResourceComponent>())
          maintenanceJob = component.QueueAgentPoolMaintenanceJob(poolId, definition.Id, userIdentity.Id);
        if (maintenanceJob.TargetAgents.Count == 0)
        {
          using (TaskResourceComponent component = requestContext.CreateComponent<TaskResourceComponent>())
          {
            TaskResourceComponent resourceComponent = component;
            int poolId1 = poolId;
            int jobId = maintenanceJob.JobId;
            TaskAgentPoolMaintenanceJobStatus? status = new TaskAgentPoolMaintenanceJobStatus?(TaskAgentPoolMaintenanceJobStatus.Completed);
            TaskAgentPoolMaintenanceJobResult? nullable = new TaskAgentPoolMaintenanceJobResult?(TaskAgentPoolMaintenanceJobResult.Failed);
            DateTime? startTime = new DateTime?();
            DateTime? finishTime = new DateTime?();
            TaskAgentPoolMaintenanceJobResult? result = nullable;
            resourceComponent.UpdateAgentPoolMaintenanceJob(poolId1, jobId, status, startTime, finishTime, result);
          }
          throw new TaskAgentNotFoundException(TaskResources.AgentNotFoundNoOnlineEnable((object) poolData.Pool.Name));
        }
        TaskHub maintenanceTaskHub = this.GetPoolMaintenanceTaskHub(requestContext);
        PlanEnvironment environment = new PlanEnvironment();
        environment.Variables["maintenance.deleteworkingdirectory.daysthreshold"] = definition.Options.WorkingDirectoryExpirationInDays.ToString();
        environment.Variables["maintenance.jobtimeoutinminutes"] = definition.JobTimeoutInMinutes.ToString();
        TaskOrchestrationContainer process = new TaskOrchestrationContainer()
        {
          Parallel = true,
          ContinueOnError = true,
          MaxConcurrency = Math.Max(maintenanceJob.TargetAgents.Count * definition.MaxConcurrentAgentsPercentage / 100, 1)
        };
        List<int> validAgents = new List<int>();
        foreach (TaskAgentPoolMaintenanceJobTargetAgent targetAgent in maintenanceJob.TargetAgents)
        {
          Demand demand = (Demand) new DemandEquals(PipelineConstants.AgentName, targetAgent.Agent.Name);
          TaskHub hub = maintenanceTaskHub;
          IVssRequestContext requestContext1 = requestContext;
          List<TaskInstance> tasks = new List<TaskInstance>();
          List<TaskInstance> taskInstanceList;
          ref List<TaskInstance> local1 = ref taskInstanceList;
          TaskOrchestrationJob orchestrationJob;
          ref TaskOrchestrationJob local2 = ref orchestrationJob;
          List<Demand> jobDemands = new List<Demand>();
          jobDemands.Add(demand);
          int timeoutInMinutes = definition.JobTimeoutInMinutes;
          string minAgentVersion = DistributedTaskResourceService.s_coreAgentMaintenanceVersion.ToString();
          if (!hub.TryCreateJob(requestContext1, "Maintenance", "Maintenance", tasks, out local1, out local2, jobDemands, timeoutInMinutes, minAgentVersion))
          {
            requestContext.TraceError(10015115, "ResourceService", string.Format("Can't create maintenance job for agent '{0}' in pool '{1}' for maintenance job '{2}'.", (object) targetAgent.Agent.Id, (object) maintenanceJob.Pool.Id, (object) maintenanceJob.JobId));
          }
          else
          {
            orchestrationJob.Variables["System.TargetAgent"] = targetAgent.Agent.Id.ToString();
            orchestrationJob.ExecuteAs = identity.ToIdentityRef(requestContext);
            process.Children.Add((TaskOrchestrationItem) orchestrationJob);
            validAgents.Add(targetAgent.Agent.Id);
          }
        }
        maintenanceJob.TargetAgents.RemoveAll((Predicate<TaskAgentPoolMaintenanceJobTargetAgent>) (x => !validAgents.Contains(x.Agent.Id)));
        ArtifactId artifactId = new ArtifactId();
        int num = maintenanceJob.Pool.Id;
        string str1 = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        num = maintenanceJob.JobId;
        string str2 = num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        artifactId.ToolSpecificId = str1 + "/" + str2;
        artifactId.ArtifactType = "PoolMaintenance";
        artifactId.Tool = "DistributedTask";
        Uri artifactUri = new Uri(LinkingUtilities.EncodeUri(artifactId), UriKind.Absolute);
        using (TaskResourceComponent component = requestContext.CreateComponent<TaskResourceComponent>())
        {
          TaskResourceComponent resourceComponent = component;
          int poolId2 = poolId;
          int jobId = maintenanceJob.JobId;
          IList<TaskAgentPoolMaintenanceJobTargetAgent> targetAgents1 = (IList<TaskAgentPoolMaintenanceJobTargetAgent>) maintenanceJob.TargetAgents;
          TaskAgentPoolMaintenanceJobStatus? status = new TaskAgentPoolMaintenanceJobStatus?();
          DateTime? startTime = new DateTime?();
          DateTime? finishTime = new DateTime?();
          TaskAgentPoolMaintenanceJobResult? result = new TaskAgentPoolMaintenanceJobResult?();
          IList<TaskAgentPoolMaintenanceJobTargetAgent> targetAgents2 = targetAgents1;
          maintenanceJob = resourceComponent.UpdateAgentPoolMaintenanceJob(poolId2, jobId, status, startTime, finishTime, result, targetAgents2, true).UpdatedJob;
        }
        this.GetTaskAgentPoolExtension(requestContext, poolId).PoolMaintenanceQueued(requestContext, poolId, maintenanceJob);
        TaskOrchestrationOwner definitionReference = new TaskOrchestrationOwner()
        {
          Id = definition.Id,
          Name = "PoolMaintenance"
        };
        TaskOrchestrationOwner ownerReference = new TaskOrchestrationOwner()
        {
          Id = maintenanceJob.JobId,
          Name = TaskResources.RequestedBy((object) userIdentity.DisplayName)
        };
        ILocationService service = requestContext.GetService<ILocationService>();
        Uri resourceUri1 = service.GetResourceUri(requestContext, "distributedtask", TaskResourceIds.Pools, (object) new
        {
          poolId = poolId
        });
        Uri resourceUri2 = service.GetResourceUri(requestContext, "distributedtask", TaskResourceIds.PoolMaintenanceJobs, (object) new
        {
          poolId = poolId,
          jobId = maintenanceJob.JobId
        });
        string taskAgentPoolWebUrl1 = this.GetTaskAgentPoolWebUrl(requestContext, poolData.Pool.Id, "settings");
        string taskAgentPoolWebUrl2 = this.GetTaskAgentPoolWebUrl(requestContext, poolData.Pool.Id, "maintenancehistory");
        definitionReference.Links.AddLink("self", resourceUri1.AbsoluteUri);
        definitionReference.Links.AddLink("web", taskAgentPoolWebUrl1);
        ownerReference.Links.AddLink("self", resourceUri2.AbsoluteUri);
        ownerReference.Links.AddLink("web", taskAgentPoolWebUrl2);
        maintenanceTaskHub.RunPlan(requestContext, maintenanceJob.Pool, Guid.Empty, maintenanceJob.OrchestrationId, maintenanceJob.OrchestrationId.ToString(), PlanTemplateType.System, artifactUri, (IOrchestrationEnvironment) environment, (IOrchestrationProcess) process, (BuildOptions) null, userIdentity.Id, definitionReference, ownerReference);
        return maintenanceJob;
      }
    }

    public TaskAgentPoolMaintenanceJob UpdateAgentPoolMaintenanceJobTargetAgents(
      IVssRequestContext requestContext,
      int poolId,
      int jobId,
      List<TaskAgentPoolMaintenanceJobTargetAgent> targetAgents)
    {
      requestContext = requestContext.ToPoolRequestContext();
      using (new MethodScope(requestContext, "ResourceService", nameof (UpdateAgentPoolMaintenanceJobTargetAgents)))
      {
        this.GetAgentPoolSecurity(requestContext, poolId).CheckPoolPermission(requestContext, poolId, 2);
        UpdateTaskAgentPoolMaintenanceJobResult maintenanceJobResult;
        using (TaskResourceComponent component = requestContext.CreateComponent<TaskResourceComponent>())
        {
          TaskResourceComponent resourceComponent = component;
          int poolId1 = poolId;
          int jobId1 = jobId;
          IList<TaskAgentPoolMaintenanceJobTargetAgent> maintenanceJobTargetAgentList = (IList<TaskAgentPoolMaintenanceJobTargetAgent>) targetAgents;
          TaskAgentPoolMaintenanceJobStatus? status = new TaskAgentPoolMaintenanceJobStatus?();
          DateTime? startTime = new DateTime?();
          DateTime? finishTime = new DateTime?();
          TaskAgentPoolMaintenanceJobResult? result = new TaskAgentPoolMaintenanceJobResult?();
          IList<TaskAgentPoolMaintenanceJobTargetAgent> targetAgents1 = maintenanceJobTargetAgentList;
          maintenanceJobResult = resourceComponent.UpdateAgentPoolMaintenanceJob(poolId1, jobId1, status, startTime, finishTime, result, targetAgents1, true);
        }
        return maintenanceJobResult.UpdatedJob;
      }
    }

    public TaskAgentPoolMaintenanceJob UpdateAgentPoolMaintenanceJob(
      IVssRequestContext requestContext,
      int poolId,
      int jobId,
      TaskAgentPoolMaintenanceJob maintenanceJob)
    {
      requestContext = requestContext.ToPoolRequestContext();
      using (new MethodScope(requestContext, "ResourceService", nameof (UpdateAgentPoolMaintenanceJob)))
      {
        this.GetAgentPoolSecurity(requestContext, poolId).CheckPoolPermission(requestContext, maintenanceJob.Pool.Id, 2);
        UpdateTaskAgentPoolMaintenanceJobResult maintenanceJobResult;
        using (TaskResourceComponent component = requestContext.CreateComponent<TaskResourceComponent>())
          maintenanceJobResult = component.UpdateAgentPoolMaintenanceJob(poolId, jobId, new TaskAgentPoolMaintenanceJobStatus?(maintenanceJob.Status), maintenanceJob.StartTime, maintenanceJob.FinishTime, maintenanceJob.Result);
        if (maintenanceJobResult.OldJob.Status != TaskAgentPoolMaintenanceJobStatus.Cancelling && maintenanceJobResult.UpdatedJob.Status == TaskAgentPoolMaintenanceJobStatus.Cancelling)
        {
          TaskAgentPoolMaintenanceDefinition definition;
          using (TaskResourceComponent component = requestContext.CreateComponent<TaskResourceComponent>())
            definition = component.GetAgentPoolMaintenanceDefinition(poolId, maintenanceJobResult.OldJob.DefinitionId).Definition;
          this.GetPoolMaintenanceTaskHub(requestContext).CancelPlan(requestContext, Guid.Empty, maintenanceJobResult.OldJob.OrchestrationId, TimeSpan.FromMinutes((double) definition.JobTimeoutInMinutes), requestContext.GetUserIdentity().DisplayName);
        }
        if (maintenanceJobResult.UpdatedJob != null)
          maintenanceJobResult.UpdatedJob = this.PopulateMaintenanceJobData(requestContext, maintenanceJobResult.UpdatedJob);
        return maintenanceJobResult.UpdatedJob;
      }
    }

    public async Task<TaskAgentJobRequest> QueueAgentRequestAsync(
      IVssRequestContext requestContext,
      Guid projectId,
      int queueId,
      TaskAgentJobRequest request)
    {
      requestContext.AssertAsyncExecutionEnabled();
      TaskAgentJobRequest taskAgentJobRequest;
      using (new MethodScope(requestContext, "ResourceService", nameof (QueueAgentRequestAsync)))
      {
        TaskAgentQueue agentQueue = this.GetAgentQueue(requestContext, projectId, queueId);
        request.QueueId = agentQueue != null ? new int?(agentQueue.Id) : throw new TaskAgentQueueNotFoundException(TaskResources.QueueNotFound((object) queueId));
        taskAgentJobRequest = await this.QueueAgentRequestByPoolAsync(requestContext, agentQueue.Pool.Id, request);
      }
      return taskAgentJobRequest;
    }

    public async Task<TaskAgentJobRequest> QueueAgentRequestByPoolAsync(
      IVssRequestContext requestContext,
      int poolId,
      TaskAgentJobRequest request)
    {
      DistributedTaskResourceService taskResourceService = this;
      request.PoolId = poolId;
      requestContext.AssertAsyncExecutionEnabled();
      requestContext = requestContext.ToPoolRequestContext();
      taskResourceService.EnsureMessageQueueStarted(requestContext);
      TaskAgentJobRequest taskAgentJobRequest;
      using (new MethodScope(requestContext, "ResourceService", nameof (QueueAgentRequestByPoolAsync)))
      {
        KPIHelper.PublishDTAgentRequestReceived(requestContext);
        taskResourceService.CheckViewAndOtherPermissionsForPool(requestContext, poolId, 2);
        TaskAgentPoolData poolData = (await taskResourceService.GetTaskAgentPoolInternalAsync(requestContext, (IList<int>) new int[1]
        {
          poolId
        })).FirstOrDefault<TaskAgentPoolData>();
        if (poolData?.Pool == null)
          throw new TaskAgentPoolNotFoundException(TaskResources.AgentPoolNotFound((object) poolId));
        string parallelismTag = request.GetParallelismTag();
        if (requestContext.ExecutionEnvironment.IsHostedDeployment && poolData.Pool.IsHosted)
        {
          HostedParallelism parallelismAsync = await requestContext.GetService<IHostedParallelismService>().GetOrCreateHostedParallelismAsync(requestContext);
          HostedParallelismLevel parallelismLevel = parallelismAsync != null ? parallelismAsync.Level : HostedParallelismLevel.Public;
          if ((parallelismLevel < HostedParallelismLevel.Public && string.Equals(parallelismTag, "Public", StringComparison.OrdinalIgnoreCase) || parallelismLevel < HostedParallelismLevel.Private && string.Equals(parallelismTag, "Private", StringComparison.OrdinalIgnoreCase)) && requestContext.GetService<ITaskHubLicenseService>().GetResourceLimits(requestContext).Where<ResourceLimit>((Func<ResourceLimit, bool>) (x => x.IsHosted)).Where<ResourceLimit>((Func<ResourceLimit, bool>) (limit => string.Equals(limit.ParallelismTag, parallelismTag, StringComparison.OrdinalIgnoreCase) && limit.TotalCount.GetValueOrDefault() > 0)).Count<ResourceLimit>() == 0)
            throw new NoParallelismAvailableException(TaskResources.NoHostedParallelismAvailable());
        }
        if (requestContext.IsFeatureEnabled("DistributedTask.RemoveHostedPoolAccess"))
        {
          IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
          string str1 = vssRequestContext.GetService<IVssRegistryService>().GetValue(vssRequestContext, (RegistryQuery) "/Service/DistributedTask/Settings/HostedPoolRemovalData", (string) null);
          if (str1 != null)
          {
            string[] strArray = str1.Split(',');
            if (strArray != null && strArray.Length >= 2)
            {
              string str2 = strArray[0];
              string str3 = strArray[1];
              if (poolData.Pool.Name.Equals(str2) && poolData.Pool.IsHosted)
                throw new TaskAgentPoolRemovedException(TaskResources.AgentPoolRemoved((object) str2, (object) str3));
            }
          }
        }
        DemandMinimumVersion minAgentVersion = (DemandMinimumVersion) null;
        IList<Demand> demands = request.Demands;
        if ((demands != null ? (demands.Count > 0 ? 1 : 0) : 0) != 0)
        {
          HashSet<Demand> demandSet = new HashSet<Demand>((IEnumerable<Demand>) request.Demands);
          minAgentVersion = DemandMinimumVersion.MaxAndRemove((ISet<Demand>) demandSet);
          TaskAgentPoolOptions? options = poolData.Pool.Options;
          TaskAgentPoolOptions? nullable = options.HasValue ? new TaskAgentPoolOptions?(options.GetValueOrDefault() & TaskAgentPoolOptions.ElasticPool) : new TaskAgentPoolOptions?();
          TaskAgentPoolOptions agentPoolOptions = TaskAgentPoolOptions.ElasticPool;
          if (nullable.GetValueOrDefault() == agentPoolOptions & nullable.HasValue)
          {
            List<DemandEquals> list = request.Demands.OfType<DemandEquals>().Where<DemandEquals>((Func<DemandEquals, bool>) (x => x.Name.Equals(PipelineConstants.AgentName, StringComparison.OrdinalIgnoreCase))).ToList<DemandEquals>();
            request.Demands = (IList<Demand>) new List<Demand>();
            if (list.Count > 0)
              request.Demands.Add((Demand) list[0]);
            if (minAgentVersion != null)
              request.Demands.Add((Demand) minAgentVersion);
          }
          else
          {
            if (minAgentVersion != null)
              demandSet.Add((Demand) minAgentVersion);
            request.Demands = (IList<Demand>) new List<Demand>((IEnumerable<Demand>) demandSet);
          }
        }
        request = taskResourceService.RedirectJobRequestToSingleHostedPool(requestContext, request, poolData);
        if (request.PoolId != poolId)
        {
          poolData = (await taskResourceService.GetTaskAgentPoolInternalAsync(requestContext, (IList<int>) new int[1]
          {
            request.PoolId
          })).FirstOrDefault<TaskAgentPoolData>();
          if (poolData?.Pool == null)
            throw new TaskAgentPoolNotFoundException(TaskResources.AgentPoolNotFound((object) request.PoolId));
          requestContext.TraceInfo(10015177, "ResourceService", string.Format("Request redirected from original pool {0} to pool {1}", (object) poolId, (object) request.PoolId));
          poolId = request.PoolId;
        }
        List<TaskAgent> updateCandidates = new List<TaskAgent>();
        int? agentCloudId1 = poolData.Pool.AgentCloudId;
        bool isDemandMatching = !agentCloudId1.HasValue || poolData.Pool.IsHosted;
        TaskAgentQueryResult queryResult;
        if (isDemandMatching)
          queryResult = await taskResourceService.GetAgentsAsync(requestContext, poolId, request.Demands);
        else
          queryResult = await taskResourceService.GetAgentsAsync(requestContext, poolId, request.AgentSpecification);
        bool autoUpdateAllowed = poolData.Pool.AutoUpdate.GetValueOrDefault(true);
        IPackageMetadataService packageService = requestContext.GetService<IPackageMetadataService>();
        if (queryResult.UnmatchedAgents.Count > 0 & isDemandMatching)
        {
          List<Tuple<TaskAgent, IList<Demand>>> list = queryResult.UnmatchedAgents.Where<Tuple<TaskAgent, IList<Demand>>>((Func<Tuple<TaskAgent, IList<Demand>>, bool>) (x => DistributedTaskResourceService.IsMissingAgentVersionOnly(x.Item2))).ToList<Tuple<TaskAgent, IList<Demand>>>();
          if (list.Count > 0)
          {
            bool flag = requestContext.IsFeatureEnabled("DistributedTask.Agent.MajorUpgradeDisabled");
            foreach (Tuple<TaskAgent, IList<Demand>> tuple in list)
            {
              PackageMetadata compatiblePackage = packageService.GetLatestCompatiblePackage(requestContext, TaskAgentConstants.AgentPackageType, tuple.Item1);
              if (compatiblePackage != null)
              {
                Version version1 = DemandMinimumVersion.ParseVersion(compatiblePackage.Version.ToString());
                Version version2 = DemandMinimumVersion.ParseVersion(tuple.Item1.Version);
                Version version3 = DemandMinimumVersion.ParseVersion(tuple.Item2[0].Value);
                if (version2.Equals(DistributedTaskResourceService.s_legacyXPlatAgentVersion) || version2.Equals(DistributedTaskResourceService.s_legacyXPlatAgentVersion2))
                  queryResult.MatchedAgents.Add(tuple.Item1);
                else if (((version2.Major != 2 ? 0 : (version3.Major == 3 ? 1 : 0)) & (flag ? 1 : 0)) != 0)
                  queryResult.MatchedAgents.Add(tuple.Item1);
                else if (version2.Major != DistributedTaskResourceService.s_legacyWindowsAgentMajorVersion && version1 > version2 && version3 <= version1)
                  updateCandidates.Add(tuple.Item1);
              }
            }
          }
          if (((updateCandidates.Count <= 0 ? 0 : (minAgentVersion != null ? 1 : 0)) & (autoUpdateAllowed ? 1 : 0)) != 0)
          {
            TaskAgentUpdateReasonData reason = new TaskAgentUpdateReasonData()
            {
              Reason = TaskAgentUpdateReasonType.MinAgentVersionRequired,
              MinAgentVersion = (Demand) minAgentVersion,
              ServiceOwner = request.ServiceOwner,
              HostId = request.HostId,
              ScopeId = request.ScopeId,
              PlanType = request.PlanType,
              DefinitionReference = request.Definition?.Clone(),
              OwnerReference = request.Owner?.Clone()
            };
            RequestAgentsUpdateResult agentsInternalAsync = await taskResourceService.SendRefreshMessageToAgentsInternalAsync(requestContext, poolId, (IList<TaskAgent>) updateCandidates, reason);
          }
        }
        List<TaskAgent> agents = new List<TaskAgent>();
        if (autoUpdateAllowed && queryResult.MatchedAgents.Count > 0)
        {
          PackageVersion version = packageService.GetPackages(requestContext, TaskAgentConstants.AgentPackageType, top: new int?(1))[0].Version;
          foreach (TaskAgent matchedAgent in (IEnumerable<TaskAgent>) queryResult.MatchedAgents)
          {
            if (DemandMinimumVersion.CompareVersion(matchedAgent.Version, version.ToString()) > 0)
              agents.Add(matchedAgent);
          }
          if (agents.Count > 0)
          {
            TaskAgentUpdateReasonData reason = new TaskAgentUpdateReasonData()
            {
              Reason = TaskAgentUpdateReasonType.Downgrade
            };
            RequestAgentsUpdateResult agentsInternalAsync = await taskResourceService.SendRefreshMessageToAgentsInternalAsync(requestContext, poolId, (IList<TaskAgent>) agents, reason);
          }
        }
        List<TaskAgent> taskAgentList1 = new List<TaskAgent>();
        if (requestContext.IsFeatureEnabled("DistributedTask.DeprecateLegacyAgent"))
        {
          taskAgentList1 = queryResult.MatchedAgents.Where<TaskAgent>((Func<TaskAgent, bool>) (a => DemandMinimumVersion.ParseVersion(a.Version).Major < 2)).ToList<TaskAgent>();
          foreach (TaskAgent taskAgent in taskAgentList1)
            queryResult.MatchedAgents.Remove(taskAgent);
        }
        if (isDemandMatching && (updateCandidates.Count == 0 || !autoUpdateAllowed) && queryResult.MatchedAgents.Count == 0)
        {
          TaskAgentPoolOptions? options = poolData.Pool.Options;
          TaskAgentPoolOptions? nullable = options.HasValue ? new TaskAgentPoolOptions?(options.GetValueOrDefault() & TaskAgentPoolOptions.ElasticPool) : new TaskAgentPoolOptions?();
          TaskAgentPoolOptions agentPoolOptions = TaskAgentPoolOptions.ElasticPool;
          if (!(nullable.GetValueOrDefault() == agentPoolOptions & nullable.HasValue))
          {
            DemandMinimumVersion versionDemand = request.Demands.FirstOrDefault<Demand>((Func<Demand, bool>) (x => x is DemandMinimumVersion)) as DemandMinimumVersion;
            throw new TaskAgentNotFoundException(taskAgentList1.Count <= 0 ? (request.Demands == null || request.Demands.Count == 0 ? TaskResources.AgentNotFoundPoolEmpty((object) poolData.Pool.Name) : (updateCandidates.Count <= 0 || versionDemand == null ? DistributedTaskResourceServiceHelper.ConstructUnmatchedDemandsError(request.Demands, queryResult.UnmatchedAgents, poolData.Pool.Name) : DistributedTaskResourceService.GetAgentNoUpdateErrorMessage(poolData.Pool.Name, versionDemand))) : TaskResources.UsingDeprecatedAgents((object) poolData.Pool.Name));
          }
        }
        if (!isDemandMatching && queryResult.MatchedAgents.Count == 0 && queryResult.UnmatchedAgents.Count == 0)
          throw new TaskAgentNotFoundException(TaskResources.AgentNotFoundPoolEmpty((object) poolData.Pool.Name));
        (int, int) valueTuple = await taskResourceService.GetTaskAgentExtension(requestContext, poolId).CheckBillingResourcesAsync(requestContext, poolId, request.ScopeId, request.PlanId, parallelismTag);
        IList<TaskAgent> taskAgentList2 = queryResult.MatchedAgents;
        if (request.MatchedAgents.Count > 0)
        {
          taskAgentList2.AddRange<TaskAgent, IList<TaskAgent>>((IEnumerable<TaskAgent>) updateCandidates);
          taskAgentList2 = taskResourceService.GetTaskAgentPoolExtension(requestContext, poolId).GetFilteredAgents(taskAgentList2, request.MatchedAgents);
          if (taskAgentList2.Count == 0)
            throw new TaskAgentNotFoundException(TaskResources.AgentNotFoundMatchingDemands((object) poolData.Pool.Name, (object) string.Join(Environment.NewLine, request.Demands.Select<Demand, string>((Func<Demand, string>) (d => string.Format("     {0}", (object) d))))));
        }
        request.MatchesAllAgentsInPool = queryResult.MatchesAllAgents(requestContext) & isDemandMatching && updateCandidates.Count == 0 && request.MatchedAgents.Count == 0;
        request.MatchedAgents.Clear();
        request.MatchedAgents.AddRange((IEnumerable<TaskAgentReference>) taskAgentList2);
        if (string.IsNullOrEmpty(request.OrchestrationId))
          request.OrchestrationId = requestContext.OrchestrationId;
        TaskAgentCloud agentCloud = (TaskAgentCloud) null;
        TaskAgentPoolData taskAgentPoolData1 = poolData;
        int num1;
        if (taskAgentPoolData1 == null)
        {
          num1 = 0;
        }
        else
        {
          TaskAgentPool pool = taskAgentPoolData1.Pool;
          bool? nullable1;
          if (pool == null)
          {
            nullable1 = new bool?();
          }
          else
          {
            agentCloudId1 = pool.AgentCloudId;
            nullable1 = new bool?(agentCloudId1.HasValue);
          }
          bool? nullable2 = nullable1;
          bool flag = true;
          num1 = nullable2.GetValueOrDefault() == flag & nullable2.HasValue ? 1 : 0;
        }
        if (num1 != 0)
        {
          IInternalAgentCloudService service = requestContext.GetService<IInternalAgentCloudService>();
          IVssRequestContext requestContext1 = requestContext;
          agentCloudId1 = poolData.Pool.AgentCloudId;
          int agentCloudId2 = agentCloudId1.Value;
          agentCloud = await service.GetAgentCloudInternalAsync(requestContext1, agentCloudId2);
        }
        string resourceThrottlingType = requestContext.GetService<PlatformTaskHubLicenseService>().GetResourceThrottlingType(requestContext, (TaskAgentPoolReference) poolData.Pool, agentCloud, parallelismTag);
        TaskAgentJobRequest request1;
        using (TaskResourceComponent resourceComponent = requestContext.CreateComponent<TaskResourceComponent>())
          request1 = await resourceComponent.QueueAgentRequestAsync(poolId, poolData.Pool.AgentCloudId, request, resourceThrottlingType);
        if (request1 != null)
          taskResourceService.GetTaskAgentPoolExtension(requestContext, poolId).AgentRequestQueued(requestContext, poolId, request1.PopulateReferenceLinks(requestContext, poolId));
        // ISSUE: explicit non-virtual call
        __nonvirtual (taskResourceService.QueueRequestAssignmentJob(requestContext));
        TaskAgentPoolData taskAgentPoolData2 = poolData;
        int num2;
        if (taskAgentPoolData2 == null)
        {
          num2 = 0;
        }
        else
        {
          TaskAgentPoolOptions? options = (TaskAgentPoolOptions?) taskAgentPoolData2.Pool?.Options;
          TaskAgentPoolOptions? nullable = options.HasValue ? new TaskAgentPoolOptions?(options.GetValueOrDefault() & TaskAgentPoolOptions.ElasticPool) : new TaskAgentPoolOptions?();
          TaskAgentPoolOptions agentPoolOptions = TaskAgentPoolOptions.ElasticPool;
          num2 = nullable.GetValueOrDefault() == agentPoolOptions & nullable.HasValue ? 1 : 0;
        }
        if (num2 != 0)
        {
          ITeamFoundationJobService service = requestContext.GetService<ITeamFoundationJobService>();
          if (service.QueryJobDefinition(requestContext, DistributedTaskJobIds.ElasticPoolSizingJobId) != null)
          {
            try
            {
              service.QueueDelayedJobs(requestContext, (IEnumerable<Guid>) new Guid[1]
              {
                DistributedTaskJobIds.ElasticPoolSizingJobId
              }, 60);
            }
            catch (JobDefinitionNotFoundException ex)
            {
              requestContext.TraceException("ResourceService", (Exception) ex);
            }
          }
        }
        taskAgentJobRequest = request1;
      }
      return taskAgentJobRequest;
    }

    private TaskAgentJobRequest RedirectJobRequestToSingleHostedPool(
      IVssRequestContext requestContext,
      TaskAgentJobRequest request,
      TaskAgentPoolData poolData)
    {
      if (SingleHostedPoolMigration.GetSingleHostedPoolMigrationStage(requestContext) == SingleHostedPoolMigrationStage.RedirectToSinglePool && poolData.Pool.IsHosted)
      {
        bool? isLegacy1 = poolData.Pool.IsLegacy;
        bool flag1 = true;
        if (isLegacy1.GetValueOrDefault() == flag1 & isLegacy1.HasValue)
        {
          int num = SingleHostedPoolMigration.ShouldRunDemandsOnSingleHostedPool(requestContext) ? 1 : 0;
          IList<Demand> demands = request.Demands;
          Demand[] demandArray;
          if (demands == null)
          {
            demandArray = (Demand[]) null;
          }
          else
          {
            IEnumerable<Demand> source = demands.Where<Demand>((Func<Demand, bool>) (x => !x.Name.Equals(PipelineConstants.AgentVersionDemandName, StringComparison.OrdinalIgnoreCase)));
            demandArray = source != null ? source.ToArray<Demand>() : (Demand[]) null;
          }
          Demand[] source1 = demandArray;
          if (num == 0 && source1 != null && source1.Length != 0)
          {
            string str = ((IEnumerable<Demand>) source1).Aggregate<Demand, string>(string.Empty, (Func<string, Demand, string>) ((s, d) => s + d.Name + ","));
            requestContext.TraceAlways(10015175, "ResourceService", string.Format("Cannot redirect to single hosted pool. Request with custom demands, poolId={0}, customDemands={1}", (object) poolData.Pool.Id, (object) str));
            return request;
          }
          TaskAgentQueue[] array = this.GetHostedAgentQueues(requestContext, request.ScopeId).Where<TaskAgentQueue>((Func<TaskAgentQueue, bool>) (q =>
          {
            TaskAgentPoolReference pool1 = q.Pool;
            if ((pool1 != null ? (pool1.IsHosted ? 1 : 0) : 0) == 0)
              return false;
            TaskAgentPoolReference pool2 = q.Pool;
            if (pool2 == null)
              return false;
            bool? isLegacy2 = pool2.IsLegacy;
            bool flag2 = false;
            return isLegacy2.GetValueOrDefault() == flag2 & isLegacy2.HasValue;
          })).ToArray<TaskAgentQueue>();
          if (array.Length != 1)
          {
            string str = ((IEnumerable<TaskAgentQueue>) array).Aggregate<TaskAgentQueue, string>(string.Empty, (Func<string, TaskAgentQueue, string>) ((ids, queue) => ids + queue.Pool.Id.ToString() + ","));
            requestContext.TraceError(10015176, "ResourceService", "Failed to redirect to single hosted pool. There is none or more than one single hosted pool, nonLegacyHostedPools=" + str);
            return request;
          }
          int id = array[0].Pool.Id;
          IList<TaskAgent> agents = this.GetAgents(requestContext, id, (string) null, false, false, false, false, (IList<string>) null);
          int currentPoolSize = DistributedTaskHostedPoolHelper.GetCurrentPoolSize(requestContext);
          if (agents.Count < currentPoolSize)
            requestContext.TraceError(10015186, "ResourceService", string.Format("Unexpected single hosted pool size, singleHostedPoolId={0}, agentsCount={1}, currentPoolSize={2}", (object) id, (object) agents.Count, (object) currentPoolSize));
          bool flag3 = requestContext.RunSynchronously<bool>((Func<Task<bool>>) (() => request.TryUpdateAgentSpecificationForPoolAsync(requestContext, (TaskAgentPoolReference) poolData.Pool)));
          bool flag4 = requestContext.IsFeatureEnabled("DistributedTask.EnforceInternalAgentSpecifcation");
          requestContext.TraceAlways(10015269, TraceLevel.Info, nameof (DistributedTaskResourceService), nameof (RedirectJobRequestToSingleHostedPool), Array.Empty<string>(), string.Format("CodePath: {0}. PoolName: {1}. AgentSpecificationUpdated: {2}. EnforceInternalAgentSpecifcation: {3}.", (object) nameof (RedirectJobRequestToSingleHostedPool), (object) poolData.Pool.Name, (object) flag3, (object) flag4));
          if (flag4 && !flag3)
            throw new MachineImageLabelDoesNotExistException(TaskResources.ImageLabelNotFound((object) poolData.Pool.Name));
          request.PoolId = id;
        }
      }
      return request;
    }

    public TaskAgentPool UpdateAgentPool(
      IVssRequestContext requestContext,
      int poolId,
      TaskAgentPool pool,
      bool removePoolMetadata = false,
      Stream poolMetadata = null)
    {
      return this.UpdateAgentPool(requestContext, poolId, pool, removePoolMetadata, poolMetadata, false);
    }

    public TaskAgentPool UpdateAgentPool(
      IVssRequestContext requestContext,
      int poolId,
      TaskAgentPool pool,
      bool removePoolMetadata = false,
      Stream poolMetadata = null,
      bool removeAgentCloud = false)
    {
      requestContext = requestContext.ToPoolRequestContext();
      this.EnsureMessageQueueStarted(requestContext);
      using (new MethodScope(requestContext, "ResourceService", nameof (UpdateAgentPool)))
      {
        this.CheckViewAndOtherPermissionsForPool(requestContext, poolId, 2);
        this.CheckHostedPoolPermissions(requestContext, poolId);
        ArgumentValidation.CheckPool(pool, nameof (pool), false);
        pool.Validate();
        if (removePoolMetadata && poolMetadata != null)
          throw new Microsoft.TeamFoundation.DistributedTask.WebApi.CannotDeleteAndAddMetadataException(TaskResources.CannotDeleteAndAddMetadata());
        int? nullable1 = new int?();
        ITeamFoundationFileService service1 = requestContext.GetService<ITeamFoundationFileService>();
        if (poolMetadata != null)
          nullable1 = new int?(service1.UploadFile(requestContext, poolMetadata, OwnerId.DistributedTask, Guid.Empty));
        Guid? nullable2 = new Guid?();
        if (pool.Owner != null)
        {
          Microsoft.VisualStudio.Services.Identity.Identity identity = requestContext.GetService<IdentityService>().GetIdentity(requestContext, pool.Owner);
          if (identity != null)
            nullable2 = new Guid?(identity.Id);
        }
        if (pool.AgentCloudId.HasValue | removeAgentCloud && !requestContext.IsSystemContext)
          throw new Microsoft.TeamFoundation.DistributedTask.WebApi.AccessDeniedException(TaskResources.CannotEditAgentCloudOnPool());
        if (pool.IsLegacy.HasValue && !requestContext.IsSystemContext)
          throw new Microsoft.TeamFoundation.DistributedTask.WebApi.AccessDeniedException(TaskResources.CannotEditIsLegacyOnPool());
        UpdateAgentPoolResult updateAgentPoolResult;
        try
        {
          using (TaskResourceComponent component = requestContext.CreateComponent<TaskResourceComponent>())
          {
            TaskResourceComponent resourceComponent = component;
            int poolId1 = poolId;
            string name = pool.Name;
            bool? autoProvision1 = pool.AutoProvision;
            bool? autoSize1 = pool.AutoSize;
            bool flag1 = removePoolMetadata;
            int? nullable3 = nullable1;
            Guid? nullable4 = nullable2;
            int? agentCloudId1 = pool.AgentCloudId;
            bool flag2 = removeAgentCloud;
            int? targetSize1 = pool.TargetSize;
            bool? isLegacy1 = pool.IsLegacy;
            bool? autoUpdate1 = pool.AutoUpdate;
            TaskAgentPoolOptions? options1 = pool.Options;
            Guid? createdBy = new Guid?();
            Guid? groupScopeId = new Guid?();
            Guid? administratorsGroupId = new Guid?();
            Guid? serviceAccountsGroupId = new Guid?();
            Guid? serviceIdentityId = new Guid?();
            bool? isHosted = new bool?();
            bool? autoProvision2 = autoProvision1;
            bool? autoSize2 = autoSize1;
            bool? provisioned = new bool?();
            int num1 = flag1 ? 1 : 0;
            int? poolMetadataFileId = nullable3;
            Guid? ownerId = nullable4;
            int? agentCloudId2 = agentCloudId1;
            int num2 = flag2 ? 1 : 0;
            int? targetSize2 = targetSize1;
            bool? isLegacy2 = isLegacy1;
            bool? autoUpdate2 = autoUpdate1;
            TaskAgentPoolOptions? options2 = options1;
            updateAgentPoolResult = resourceComponent.UpdateAgentPool(poolId1, name, createdBy, groupScopeId, administratorsGroupId, serviceAccountsGroupId, serviceIdentityId, isHosted, autoProvision2, autoSize2, provisioned, num1 != 0, poolMetadataFileId, ownerId, agentCloudId2, num2 != 0, targetSize2, isLegacy2, autoUpdate2, options2);
          }
        }
        catch (Exception ex)
        {
          requestContext.TraceException("ResourceService", ex);
          if (nullable1.HasValue)
            service1.DeleteFile(requestContext, (long) nullable1.Value);
          throw;
        }
        ITeamFoundationEventService service2 = requestContext.GetService<ITeamFoundationEventService>();
        requestContext.TraceInfo("ResourceService", "DistributedTask", (object) string.Format("Publishing UpdateAgentPoolEvent notification for pool {0}", (object) pool.Id));
        service2.SyncPublishNotification(requestContext, (object) new UpdateAgentPoolEvent(pool));
        if (pool.TargetSize.HasValue || pool.AutoSize.HasValue)
          DistributedTaskResourceService.ResizeAgentCloudPool(requestContext, pool);
        if (updateAgentPoolResult.PreviousPoolData != null && updateAgentPoolResult.PreviousPoolData.PoolMetadataFileId.HasValue)
        {
          int? poolMetadataFileId1 = updateAgentPoolResult.PreviousPoolData.PoolMetadataFileId;
          int? poolMetadataFileId2 = updateAgentPoolResult.UpdatedPoolData.PoolMetadataFileId;
          if (!(poolMetadataFileId1.GetValueOrDefault() == poolMetadataFileId2.GetValueOrDefault() & poolMetadataFileId1.HasValue == poolMetadataFileId2.HasValue))
          {
            ITeamFoundationFileService foundationFileService = service1;
            IVssRequestContext requestContext1 = requestContext;
            poolMetadataFileId2 = updateAgentPoolResult.PreviousPoolData.PoolMetadataFileId;
            long fileId = (long) poolMetadataFileId2.Value;
            foundationFileService.DeleteFile(requestContext1, fileId);
          }
        }
        ITeamFoundationPropertyService service3 = requestContext.GetService<ITeamFoundationPropertyService>();
        if (pool.Properties != null && pool.Properties.Count > 0)
          service3.SetProperties(requestContext, pool.CreateSpec(), pool.Properties.Convert());
        using (TeamFoundationDataReader properties = service3.GetProperties(requestContext, updateAgentPoolResult.UpdatedPoolData.Pool.CreateSpec(), (IEnumerable<string>) new string[1]
        {
          "*"
        }))
        {
          foreach (ArtifactPropertyValue current in properties.CurrentEnumerable<ArtifactPropertyValue>())
            updateAgentPoolResult.UpdatedPoolData.Pool.Properties = current.PropertyValues.Convert();
        }
        updateAgentPoolResult.UpdatedPoolData.Pool = DistributedTaskResourceService.PopulateIdentityReferences(requestContext, updateAgentPoolResult.UpdatedPoolData.Pool);
        requestContext.GetService<TaskAgentPoolCacheService>().Set(requestContext, updateAgentPoolResult.UpdatedPoolData.Pool.Id, updateAgentPoolResult.UpdatedPoolData.Clone());
        return updateAgentPoolResult.UpdatedPoolData.Pool;
      }
    }

    public TaskAgentJobRequest UpdateAgentRequest(
      IVssRequestContext requestContext,
      int poolId,
      long requestId,
      DateTime? expirationTime = null,
      DateTime? startTime = null,
      DateTime? finishTime = null,
      TaskResult? result = null)
    {
      requestContext = requestContext.ToPoolRequestContext();
      this.EnsureMessageQueueStarted(requestContext);
      using (new MethodScope(requestContext, "ResourceService", nameof (UpdateAgentRequest)))
      {
        this.CheckViewAndOtherPermissionsForAgent(requestContext, poolId, new int?(), new long?(requestId), 4, new int?(2));
        TaskAgentPoolData agentPool = this.GetTaskAgentPoolInternal(requestContext, (IList<int>) new int[1]
        {
          poolId
        }).FirstOrDefault<TaskAgentPoolData>();
        IServerPoolProvider poolProviderForPool = requestContext.GetService<IInternalAgentCloudService>().GetPoolProviderForPool(requestContext, agentPool);
        UpdateAgentRequestResult agentRequestResult;
        using (TaskResourceComponent component = requestContext.CreateComponent<TaskResourceComponent>())
          agentRequestResult = component.UpdateAgentRequest(poolId, requestId, poolProviderForPool.LeaseRenewalTimeout, expirationTime, startTime, finishTime, result);
        agentRequestResult.After.PopulateReferenceLinks(requestContext, poolId);
        DateTime? nullable = agentRequestResult.Before.ReceiveTime;
        if (!nullable.HasValue)
        {
          nullable = agentRequestResult.After.ReceiveTime;
          if (nullable.HasValue)
          {
            KPIHelper.PublishDTJobStarted(requestContext);
            this.GetTaskAgentPoolExtension(requestContext, poolId).AgentRequestStarted(requestContext, poolId, agentRequestResult.After);
          }
        }
        nullable = agentRequestResult.Before.FinishTime;
        if (!nullable.HasValue)
        {
          nullable = agentRequestResult.After.FinishTime;
          if (nullable.HasValue)
          {
            try
            {
              JobCompletedEvent eventData = new JobCompletedEvent(agentRequestResult.After.RequestId, agentRequestResult.After.JobId, agentRequestResult.After.Result.Value);
              this.RaiseEvent<JobCompletedEvent>(requestContext, agentRequestResult.After.ServiceOwner, agentRequestResult.After.HostId, agentRequestResult.After.ScopeId, agentRequestResult.After.PlanType, agentRequestResult.After.PlanId, eventData);
            }
            catch (TaskOrchestrationPlanNotFoundException ex)
            {
              if (agentRequestResult.After.ReservedAgent?.Name != null)
                requestContext.TraceError(10015132, "ResourceService", "Plan {0} not found when sending JobCompleted event for Job {1} running on agent {2} ({3}) on pool {4}", (object) agentRequestResult.After.PlanId, (object) agentRequestResult.After.RequestId, (object) agentRequestResult.After.ReservedAgent.Name, (object) agentRequestResult.After.ReservedAgent.Id, (object) poolId);
              else
                requestContext.TraceError(10015132, "ResourceService", "Plan {0} not found when sending JobCompleted event for Job {1} for pool {2}", (object) agentRequestResult.After.PlanId, (object) agentRequestResult.After.RequestId, (object) poolId);
              requestContext.RunSynchronously((Func<Task>) (() => this.FinishAgentRequestAsync(requestContext, poolId, requestId, result, false, false)));
            }
          }
        }
        return agentRequestResult.After;
      }
    }

    public async Task<TaskAgentJobRequest> UpdateAgentRequestAsync(
      IVssRequestContext requestContext,
      int poolId,
      long requestId,
      DateTime? expirationTime = null,
      DateTime? startTime = null,
      DateTime? finishTime = null,
      TaskResult? result = null)
    {
      DistributedTaskResourceService taskResourceService = this;
      requestContext.AssertAsyncExecutionEnabled();
      requestContext = requestContext.ToPoolRequestContext();
      taskResourceService.EnsureMessageQueueStarted(requestContext);
      TaskAgentJobRequest after;
      using (new MethodScope(requestContext, "ResourceService", nameof (UpdateAgentRequestAsync)))
      {
        await taskResourceService.CheckViewAndOtherPermissionsForAgentAsync(requestContext, poolId, new int?(), new long?(requestId), 4, new int?(2));
        TaskAgentPoolData agentPool = (await taskResourceService.GetTaskAgentPoolInternalAsync(requestContext, (IList<int>) new int[1]
        {
          poolId
        })).FirstOrDefault<TaskAgentPoolData>();
        IServerPoolProvider providerForPoolAsync = await requestContext.GetService<IInternalAgentCloudService>().GetPoolProviderForPoolAsync(requestContext, agentPool);
        UpdateAgentRequestResult updateResult;
        using (TaskResourceComponent resourceComponent = requestContext.CreateComponent<TaskResourceComponent>())
          updateResult = await resourceComponent.UpdateAgentRequestAsync(poolId, requestId, providerForPoolAsync.LeaseRenewalTimeout, expirationTime, startTime, finishTime, result);
        updateResult.After.PopulateReferenceLinks(requestContext, poolId);
        DateTime? nullable;
        if (!updateResult.Before.ReceiveTime.HasValue)
        {
          nullable = updateResult.After.ReceiveTime;
          if (nullable.HasValue)
          {
            KPIHelper.PublishDTJobStarted(requestContext);
            taskResourceService.GetTaskAgentPoolExtension(requestContext, poolId).AgentRequestStarted(requestContext, poolId, updateResult.After);
          }
        }
        nullable = updateResult.Before.FinishTime;
        if (!nullable.HasValue)
        {
          nullable = updateResult.After.FinishTime;
          if (nullable.HasValue)
          {
            try
            {
              JobCompletedEvent eventData = new JobCompletedEvent(updateResult.After.RequestId, updateResult.After.JobId, updateResult.After.Result.Value);
              await taskResourceService.RaiseEventAsync<JobCompletedEvent>(requestContext, updateResult.After.ServiceOwner, updateResult.After.HostId, updateResult.After.ScopeId, updateResult.After.PlanType, updateResult.After.PlanId, eventData);
            }
            catch (TaskOrchestrationPlanNotFoundException ex)
            {
              if (updateResult.After.ReservedAgent?.Name != null)
                requestContext.TraceError(10015132, "ResourceService", "Plan {0} not found when sending JobCompleted event for Job {1} running on agent {2} ({3}) on pool {4}", (object) updateResult.After.PlanId, (object) updateResult.After.RequestId, (object) updateResult.After.ReservedAgent.Name, (object) updateResult.After.ReservedAgent.Id, (object) poolId);
              else
                requestContext.TraceError(10015132, "ResourceService", "Plan {0} not found when sending JobCompleted event for Job {1} for pool {2}", (object) updateResult.After.PlanId, (object) updateResult.After.RequestId, (object) poolId);
              // ISSUE: explicit non-virtual call
              await __nonvirtual (taskResourceService.FinishAgentRequestAsync(requestContext, poolId, requestId, result, false, false));
            }
          }
        }
        after = updateResult.After;
      }
      return after;
    }

    public async Task<TaskAgentJobRequest> BumpAgentRequestPriorityAsync(
      IVssRequestContext requestContext,
      int poolId,
      long requestId)
    {
      requestContext.AssertAsyncExecutionEnabled();
      requestContext = requestContext.ToPoolRequestContext();
      this.EnsureMessageQueueStarted(requestContext);
      using (new MethodScope(requestContext, "ResourceService", nameof (BumpAgentRequestPriorityAsync)))
      {
        if (!requestContext.IsFeatureEnabled("DistributedTask.BumpAgentRequestPriority"))
          return (TaskAgentJobRequest) null;
        this.CheckViewAndOtherPermissionsForPool(requestContext, poolId, 2);
        TaskAgentJobRequest taskAgentJobRequest;
        using (TaskResourceComponent resourceComponent = requestContext.CreateComponent<TaskResourceComponent>())
          taskAgentJobRequest = await resourceComponent.BumpAgentRequestPriorityAsync(poolId, requestId);
        if (taskAgentJobRequest != null)
        {
          using (requestContext.CreateOrchestrationIdScope(taskAgentJobRequest.OrchestrationId))
            requestContext.TraceAlways(10015212, TraceLevel.Info, "DistributedTask", "ResourceService", "Agent request {0} had its priority increased to {1}", (object) taskAgentJobRequest.RequestId, (object) taskAgentJobRequest.Priority);
        }
        return taskAgentJobRequest;
      }
    }

    public async Task<TaskAgentJobRequest> UpdateAgentRequestLockedUntilAsync(
      IVssRequestContext requestContext,
      int poolId,
      long requestId,
      DateTime? expirationTime,
      DateTime? startTime = null)
    {
      DistributedTaskResourceService taskResourceService = this;
      requestContext = requestContext.ToPoolRequestContext();
      taskResourceService.EnsureMessageQueueStarted(requestContext);
      TaskAgentJobRequest after;
      using (new MethodScope(requestContext, "ResourceService", nameof (UpdateAgentRequestLockedUntilAsync)))
      {
        await taskResourceService.CheckViewAndOtherPermissionsForAgentAsync(requestContext, poolId, new int?(), new long?(requestId), 4, new int?(2));
        TaskAgentPoolData agentPool = (await taskResourceService.GetTaskAgentPoolInternalAsync(requestContext, (IList<int>) new int[1]
        {
          poolId
        })).FirstOrDefault<TaskAgentPoolData>();
        IServerPoolProvider providerForPoolAsync = await requestContext.GetService<IInternalAgentCloudService>().GetPoolProviderForPoolAsync(requestContext, agentPool);
        using (TaskResourceComponent resourceComponent = requestContext.CreateComponent<TaskResourceComponent>())
          after = (await resourceComponent.UpdateAgentRequestAsync(poolId, requestId, providerForPoolAsync.LeaseRenewalTimeout, expirationTime, startTime)).After;
      }
      return after;
    }

    public void FinishAgentUpdate(
      IVssRequestContext requestContext,
      int poolId,
      int agentId,
      TaskResult result,
      string currentState,
      TaskAgentUpdate agentUpdate)
    {
      requestContext = requestContext.ToPoolRequestContext();
      using (new MethodScope(requestContext, "ResourceService", nameof (FinishAgentUpdate)))
      {
        this.CheckViewAndOtherPermissionsForPool(requestContext, poolId, 4);
        TaskAgent agent;
        using (TaskResourceComponent component = requestContext.CreateComponent<TaskResourceComponent>())
          agent = component.FinishAgentUpdate(poolId, agentId, result, currentState);
        if (agent != null)
          this.GetTaskAgentPoolExtension(requestContext, poolId).AgentUpdated(requestContext, poolId, agent);
        if (!requestContext.ExecutionEnvironment.IsHostedDeployment)
          return;
        CustomerIntelligenceData properties = new CustomerIntelligenceData();
        properties.Add("PoolId", (double) poolId);
        properties.Add("AgentId", (double) agentId);
        properties.Add("RequestTime", (object) agentUpdate.RequestTime);
        properties.Add("SourceVersion", (string) agentUpdate.SourceVersion);
        properties.Add("TargetVersion", (string) agentUpdate.TargetVersion);
        properties.Add("Reason", agentUpdate.Reason?.Code.ToString());
        properties.Add("Result", result.ToString());
        properties.Add("Detail", currentState);
        requestContext.GetService<CustomerIntelligenceService>().Publish(requestContext, "DistributedTask", "AgentUpdate", properties);
      }
    }

    public TaskAgent UpdateAgentUpdateState(
      IVssRequestContext requestContext,
      int poolId,
      int agentId,
      string currentState)
    {
      requestContext = requestContext.ToPoolRequestContext();
      using (new MethodScope(requestContext, "ResourceService", nameof (UpdateAgentUpdateState)))
      {
        this.CheckViewAndOtherPermissionsForPool(requestContext, poolId, 4);
        TaskAgent agent;
        using (TaskResourceComponent component = requestContext.CreateComponent<TaskResourceComponent>())
          agent = component.UpdateAgentUpdateState(poolId, agentId, currentState);
        if (agent != null)
          this.GetTaskAgentPoolExtension(requestContext, poolId).AgentUpdated(requestContext, poolId, agent);
        return agent;
      }
    }

    public void CreateTeamProject(IVssRequestContext requestContext, Guid projectId)
    {
      if (requestContext.ExecutionEnvironment.IsHostedDeployment)
        this.EnsureHostedMacPoolsAdded(requestContext);
      this.CreateQueuesForAgentPools(requestContext, projectId);
    }

    public void DeleteTeamProject(IVssRequestContext requestContext, Guid projectId)
    {
      using (new MethodScope(requestContext, "ResourceService", nameof (DeleteTeamProject)))
      {
        GetAgentQueuesResult agentQueuesResult;
        using (TaskResourceComponent component = requestContext.CreateComponent<TaskResourceComponent>())
          agentQueuesResult = component.DeleteTeamProject(projectId);
        IList<TaskAgentQueue> queues = agentQueuesResult.Queues;
        if (queues != null && queues.Count > 0)
        {
          TaskAgentQueueCacheService service = requestContext.GetService<TaskAgentQueueCacheService>();
          foreach (TaskAgentQueue taskAgentQueue in (IEnumerable<TaskAgentQueue>) queues)
            service.Remove(taskAgentQueue.ProjectId, taskAgentQueue.Id);
          Microsoft.VisualStudio.Services.Identity.Identity identity = requestContext.GetService<IdentityService>().GetDeletedGroups(requestContext, projectId, (IEnumerable<IdentityDescriptor>) new IdentityDescriptor[1]
          {
            GroupWellKnownIdentityDescriptors.EveryoneGroup
          }).FirstOrDefault<Microsoft.VisualStudio.Services.Identity.Identity>();
          if (identity != null)
          {
            IAgentPoolSecurityProvider agentPoolSecurity = this.GetAgentPoolSecurity(requestContext, TaskAgentPoolType.Automation);
            foreach (int poolId in queues.Where<TaskAgentQueue>((Func<TaskAgentQueue, bool>) (q => q.Pool != null)).Select<TaskAgentQueue, int>((Func<TaskAgentQueue, int>) (x => x.Pool.Id)).Distinct<int>())
              agentPoolSecurity.RevokeReadPermissionToPool(requestContext, poolId, identity);
          }
          requestContext.GetService<IDistributedTaskEventPublisherService>().NotifyAgentQueuesEvent(requestContext, "MS.TF.DistributedTask.AgentQueuesDeleted", (IEnumerable<TaskAgentQueue>) queues);
        }
        IList<DeploymentGroup> machineGroups = agentQueuesResult.MachineGroups;
        if (machineGroups == null || machineGroups.Count <= 0)
          return;
        IEnumerable<int> ints = machineGroups.Where<DeploymentGroup>((Func<DeploymentGroup, bool>) (m => m.Pool != null)).Select<DeploymentGroup, int>((Func<DeploymentGroup, int>) (x => x.Pool.Id));
        IAgentPoolSecurityProvider agentPoolSecurity1 = this.GetAgentPoolSecurity(requestContext, TaskAgentPoolType.Deployment);
        IdentityService service1 = requestContext.GetService<IdentityService>();
        IList<Microsoft.VisualStudio.Services.Identity.Identity> identityList = (IList<Microsoft.VisualStudio.Services.Identity.Identity>) new List<Microsoft.VisualStudio.Services.Identity.Identity>();
        Microsoft.VisualStudio.Services.Identity.Identity identity1 = service1.GetGroups(requestContext, projectId, (IEnumerable<IdentityDescriptor>) new IdentityDescriptor[1]
        {
          GroupWellKnownIdentityDescriptors.EveryoneGroup
        }).FirstOrDefault<Microsoft.VisualStudio.Services.Identity.Identity>();
        if (identity1 != null)
          identityList.Add(identity1);
        Microsoft.VisualStudio.Services.Identity.Identity identity2 = service1.GetGroups(requestContext, projectId, (IList<string>) new string[1]
        {
          TaskResources.ProjectContributorsGroupName()
        }).FirstOrDefault<Microsoft.VisualStudio.Services.Identity.Identity>();
        if (identity2 != null)
          identityList.Add(identity2);
        if (identityList.Any<Microsoft.VisualStudio.Services.Identity.Identity>())
        {
          foreach (int poolId in ints)
            agentPoolSecurity1.RevokeReadPermissionToPool(requestContext, poolId, identityList);
        }
        Microsoft.VisualStudio.Services.Identity.Identity identity3 = service1.GetGroups(requestContext, projectId, (IEnumerable<IdentityDescriptor>) new IdentityDescriptor[1]
        {
          TaskWellKnownIdentityDescriptors.DeploymentGroupAdministratorsGroup
        }).FirstOrDefault<Microsoft.VisualStudio.Services.Identity.Identity>();
        if (identity3 == null)
          return;
        foreach (int poolId in ints)
          agentPoolSecurity1.RevokeAdministratorPermissionToPool(requestContext, poolId, identity3);
      }
    }

    public void CreateQueuesForAgentPools(IVssRequestContext requestContext, Guid projectId)
    {
      using (requestContext.TraceScope("ResourceService", nameof (CreateQueuesForAgentPools)))
      {
        this.Security.CheckQueuePermission(requestContext, projectId, 32);
        IDataspaceService service1 = requestContext.GetService<IDataspaceService>();
        try
        {
          service1.QueryDataspace(requestContext, DefaultSecurityProvider.DataspaceCategory, projectId, true);
        }
        catch (DataspaceNotFoundException ex)
        {
          service1.CreateDataspace(requestContext, DefaultSecurityProvider.DataspaceCategory, projectId, DataspaceState.Active);
        }
        IdentityService service2 = requestContext.GetService<IdentityService>();
        IdentityDescriptor[] descriptors = new IdentityDescriptor[2]
        {
          GroupWellKnownIdentityDescriptors.NamespaceAdministratorsGroup,
          GroupWellKnownIdentityDescriptors.EveryoneGroup
        };
        List<Microsoft.VisualStudio.Services.Identity.Identity> list = service2.GetGroups(requestContext, projectId, (IEnumerable<IdentityDescriptor>) descriptors).ToList<Microsoft.VisualStudio.Services.Identity.Identity>();
        List<AccessControlEntry> accessControlEntryList = new List<AccessControlEntry>();
        Microsoft.VisualStudio.Services.Identity.Identity identity1 = list.FirstOrDefault<Microsoft.VisualStudio.Services.Identity.Identity>((Func<Microsoft.VisualStudio.Services.Identity.Identity, bool>) (x => IdentityHelper.IsWellKnownGroup(x.Descriptor, GroupWellKnownIdentityDescriptors.NamespaceAdministratorsGroup)));
        if (identity1 != null)
          accessControlEntryList.Add(new AccessControlEntry()
          {
            Descriptor = identity1.Descriptor,
            Allow = 59
          });
        Microsoft.VisualStudio.Services.Identity.Identity projectValidUsers = list.FirstOrDefault<Microsoft.VisualStudio.Services.Identity.Identity>((Func<Microsoft.VisualStudio.Services.Identity.Identity, bool>) (x => IdentityHelper.IsWellKnownGroup(x.Descriptor, GroupWellKnownIdentityDescriptors.EveryoneGroup)));
        if (projectValidUsers != null)
          accessControlEntryList.Add(new AccessControlEntry()
          {
            Descriptor = projectValidUsers.Descriptor,
            Allow = 1
          });
        string[] accountNames = new string[3]
        {
          TaskResources.ProjectBuildAdminAccountName(),
          TaskResources.ProjectReleaseAdminAccountName(),
          TaskResources.ProjectContributorsGroupName()
        };
        IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity> groups = service2.GetGroups(requestContext, projectId, (IList<string>) accountNames);
        foreach (Microsoft.VisualStudio.Services.Identity.Identity identity2 in groups.Where<Microsoft.VisualStudio.Services.Identity.Identity>((Func<Microsoft.VisualStudio.Services.Identity.Identity, bool>) (group => TaskResources.ProjectBuildAdminAccountName().Equals(group.GetProperty<string>("Account", string.Empty), StringComparison.OrdinalIgnoreCase) || TaskResources.ProjectReleaseAdminAccountName().Equals(group.GetProperty<string>("Account", string.Empty), StringComparison.OrdinalIgnoreCase))).ToList<Microsoft.VisualStudio.Services.Identity.Identity>())
          accessControlEntryList.Add(new AccessControlEntry()
          {
            Descriptor = identity2.Descriptor,
            Allow = 59
          });
        Microsoft.VisualStudio.Services.Identity.Identity contributorsGroup = groups.Where<Microsoft.VisualStudio.Services.Identity.Identity>((Func<Microsoft.VisualStudio.Services.Identity.Identity, bool>) (group => TaskResources.ProjectContributorsGroupName().Equals(group.GetProperty<string>("Account", string.Empty), StringComparison.OrdinalIgnoreCase))).FirstOrDefault<Microsoft.VisualStudio.Services.Identity.Identity>();
        IVssRequestContext vssRequestContext = requestContext.Elevate();
        vssRequestContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(vssRequestContext, DefaultSecurityProvider.NamespaceId).SetAccessControlEntries(vssRequestContext, DefaultSecurityProvider.GetAgentQueueToken(vssRequestContext, projectId), (IEnumerable<IAccessControlEntry>) accessControlEntryList, true);
        bool flag = requestContext.IsFeatureEnabled("DistributedTask.EnableGrantOrgLevelAccessPermissionToAllPipelinesInAgentPools");
        foreach (TaskAgentPool agentPool in this.GetAgentPools(vssRequestContext, (string) null, flag ? (IList<string>) DistributedTaskResourceService.s_autoAuthorizePropertyFilters : (IList<string>) (string[]) null, TaskAgentPoolType.Automation, TaskAgentPoolActionFilter.None))
        {
          bool? autoProvision = agentPool.AutoProvision;
          if (autoProvision.HasValue)
          {
            autoProvision = agentPool.AutoProvision;
            if (autoProvision.Value)
            {
              bool authorizePipelines = false;
              if (flag)
                agentPool.Properties.TryGetValue<bool>("System.AutoAuthorize", out authorizePipelines);
              TaskAgentQueue queue = new TaskAgentQueue()
              {
                Name = agentPool.Name,
                Pool = agentPool.AsReference()
              };
              try
              {
                TaskAgentQueue taskAgentQueue = this.AddAgentQueue(vssRequestContext, projectId, queue, (Microsoft.VisualStudio.Services.Identity.Identity) null, authorizePipelines, contributorsGroup, projectValidUsers);
                if (taskAgentQueue != null)
                  requestContext.TraceInfo(10015050, "DistributedTask", "Created queue {0} for agent pool {1} ({2})", (object) taskAgentQueue.Id, (object) agentPool.Name, (object) agentPool.Id);
              }
              catch (TaskAgentQueueExistsException ex)
              {
              }
            }
          }
        }
      }
    }

    public async Task<AgentRequestsAssignmentResult> AssignAgentRequestsAsync(
      IVssRequestContext requestContext)
    {
      DistributedTaskResourceService taskResourceService = this;
      requestContext.AssertAsyncExecutionEnabled();
      requestContext = requestContext.ToPoolRequestContext();
      taskResourceService.EnsureMessageQueueStarted(requestContext);
      AgentRequestsAssignmentResult assignmentResult1;
      using (new MethodScope(requestContext, "ResourceService", nameof (AssignAgentRequestsAsync)))
      {
        AgentRequestsAssignmentResult assignmentResult = new AgentRequestsAssignmentResult();
        if (requestContext.ExecutionEnvironment.IsHostedDeployment && !requestContext.IsFeatureEnabled("DistributedTask.PipelineBillingModel2.SelfHosted.InfiniteResourceLimits"))
        {
          IList<ResourceLimit> resourceLimits = requestContext.GetService<PlatformTaskHubLicenseService>().GetResourceLimits(requestContext, true);
          if (requestContext.IsFeatureEnabled("DistributedTask.LimitHostedMaxParallelism"))
          {
            ResourceLimit resourceLimit = resourceLimits.SingleOrDefault<ResourceLimit>((Func<ResourceLimit, bool>) (x => x.HostId == requestContext.ServiceHost.InstanceId && x.ParallelismTag == "Public" && x.IsHosted));
            if (resourceLimit != null)
              resourceLimits.Remove(resourceLimit);
            resourceLimits.Add(new ResourceLimit(requestContext.ServiceHost.InstanceId, "Public", true)
            {
              TotalCount = new int?(1)
            });
          }
          assignmentResult.ResourceLimits.AddRange((IEnumerable<ResourceLimit>) resourceLimits);
        }
        bool flag1 = requestContext.IsFeatureEnabled("DistributedTask.UseAssignAgentRequestsV3Sproc");
        bool flag2 = requestContext.IsFeatureEnabled("DistributedTask.UseAssignAgentRequestsV2Sproc");
        int num = requestContext.IsFeatureEnabled("DistributedTask.UseAssignAgentRequestsSprocTimeout") ? 1 : 0;
        int? timeoutSec = new int?();
        if (num != 0)
          timeoutSec = new int?(requestContext.GetService<IVssRegistryService>().GetValue<int>(requestContext, (RegistryQuery) "/Service/DistributedTask/Settings/AssignmentJobTimeoutInSeconds", true, 60));
        IInternalAgentCloudService agentCloudService = requestContext.GetService<IInternalAgentCloudService>();
        IServerPoolProvider privatePoolProvider = agentCloudService.GetPrivatePoolProvider();
        AssignAgentRequestsResult result;
        using (TaskResourceComponent rc = requestContext.CreateComponent<TaskResourceComponent>())
        {
          TaskResourceComponent79 resourceComponent79 = rc as TaskResourceComponent79;
          if (resourceComponent79 != null & flag1)
            result = await resourceComponent79.AssignAgentRequestsAsyncV3(privatePoolProvider.InitialLeaseTimeout, taskResourceService.m_agentRequestSettings.HostedLeaseTimeout, taskResourceService.m_agentRequestSettings.DefaultLeaseTimeout, (IList<ResourceLimit>) assignmentResult.ResourceLimits, taskResourceService.m_agentRequestSettings.AssignmentBatchSize, false, timeoutSec);
          else
            result = await (flag2 ? rc.AssignAgentRequestsAsyncV2(privatePoolProvider.InitialLeaseTimeout, taskResourceService.m_agentRequestSettings.HostedLeaseTimeout, taskResourceService.m_agentRequestSettings.DefaultLeaseTimeout, (IList<ResourceLimit>) assignmentResult.ResourceLimits, taskResourceService.m_agentRequestSettings.AssignmentBatchSize, false) : rc.AssignAgentRequestsAsync(privatePoolProvider.InitialLeaseTimeout, taskResourceService.m_agentRequestSettings.HostedLeaseTimeout, taskResourceService.m_agentRequestSettings.DefaultLeaseTimeout, (IList<ResourceLimit>) assignmentResult.ResourceLimits, taskResourceService.m_agentRequestSettings.AssignmentBatchSize, false));
        }
        assignmentResult.ResourceUsage.AddRange((IEnumerable<ResourceUsageData>) result.ResourceUsageDataCollection);
        List<RunAgentEvent> orchestrationEvents = result.Events;
        RunAgentEvent agentEvent;
        foreach (AssignedAgentRequestResult assignedRequestResult in result.AssignedRequestResults)
        {
          AssignedAgentRequestResult assignedResult = assignedRequestResult;
          bool delivered = false;
          if (assignedResult.Request.ReservedAgent != null)
          {
            taskResourceService.TraceAgentAssignmentResults(requestContext, assignedResult);
            agentEvent = orchestrationEvents.Where<RunAgentEvent>((Func<RunAgentEvent, bool>) (x => x.PoolId == assignedResult.Request.PoolId && x.AgentId == assignedResult.Request.ReservedAgent.Id)).FirstOrDefault<RunAgentEvent>();
            if (agentEvent != null)
            {
              delivered = await agentCloudService.DeliverEventAsync(requestContext, agentEvent, true);
              orchestrationEvents.Remove(agentEvent);
              taskResourceService.TraceAgentCloudOrchestrationEventsFromSprocs(requestContext, "prc_AssignAgentRequests", agentEvent, delivered, assignedResult.Request);
            }
            if (!delivered)
            {
              using (requestContext.CreateOrchestrationIdScope(assignedResult.Request.OrchestrationId))
              {
                try
                {
                  TaskAgentPoolData agentPool = (await taskResourceService.GetTaskAgentPoolInternalAsync(requestContext, (IList<int>) new int[1]
                  {
                    assignedResult.Request.PoolId
                  })).First<TaskAgentPoolData>();
                  IServerPoolProvider providerForAgentCloud = agentCloudService.GetPoolProviderForAgentCloud(requestContext, assignedResult.CurrentAgentCloud, agentPool);
                  IServerPoolProvider previousProvider = providerForAgentCloud;
                  int? agentCloudId1 = assignedResult.PreviousAgentCloud?.AgentCloudId;
                  int? agentCloudId2 = assignedResult.CurrentAgentCloud?.AgentCloudId;
                  if (!(agentCloudId1.GetValueOrDefault() == agentCloudId2.GetValueOrDefault() & agentCloudId1.HasValue == agentCloudId2.HasValue))
                    previousProvider = agentCloudService.GetPoolProviderForAgentCloud(requestContext, assignedResult.PreviousAgentCloud, agentPool);
                  taskResourceService.QueueRequestAssignmentNotification(requestContext, previousProvider, providerForAgentCloud, assignedResult.Request);
                }
                catch (Exception ex)
                {
                  requestContext.TraceException(10015154, "ResourceService", ex);
                }
              }
            }
            assignmentResult.AssignedRequests.Add(assignedResult.Request);
            agentEvent = (RunAgentEvent) null;
          }
        }
        foreach (RunAgentEvent runAgentEvent in orchestrationEvents)
        {
          agentEvent = runAgentEvent;
          bool delivered = await agentCloudService.DeliverEventAsync(requestContext, agentEvent, true);
          taskResourceService.TraceAgentCloudOrchestrationEventsFromSprocs(requestContext, "prc_AssignAgentRequests", agentEvent, delivered, (TaskAgentJobRequest) null);
          agentEvent = (RunAgentEvent) null;
        }
        if (requestContext.ExecutionEnvironment.IsHostedDeployment)
          taskResourceService.NotifyResourceUsageUpdated(requestContext, result.ResourceUsageDataCollection, (IList<ResourceLimit>) assignmentResult.ResourceLimits);
        if (result.AssignedRequestResults.Count > 0)
          requestContext.GetService<ITeamFoundationJobService>().QueueDelayedJobs(requestContext, (IEnumerable<Guid>) new Guid[1]
          {
            TaskConstants.AgentRequestMonitorJob
          }, (int) TimeSpan.FromMinutes(5.0).TotalSeconds);
        assignmentResult1 = assignmentResult;
      }
      return assignmentResult1;
    }

    private void TraceAgentAssignmentResults(
      IVssRequestContext requestContext,
      AssignedAgentRequestResult assignedResult)
    {
      if (!requestContext.IsFeatureEnabled("DistributedTask.TraceAgentAssignmentResults"))
        return;
      try
      {
        using (requestContext.CreateOrchestrationIdScope(assignedResult.Request.OrchestrationId))
          requestContext.TraceAlways(10015247, "ResourceService", new
          {
            Note = "prc_AssignAgentRequests returned a result",
            RequestId = assignedResult.Request.RequestId,
            AgentCloudType = assignedResult.CurrentAgentCloud?.Type,
            AgentCloudName = assignedResult.CurrentAgentCloud?.Name,
            PoolId = assignedResult.Request.PoolId,
            ProjectId = assignedResult.Request.ScopeId,
            QueueTime = assignedResult.Request.QueueTime,
            AssignTime = assignedResult.Request.AssignTime,
            MatchesAllAgentsInPool = assignedResult.Request.MatchesAllAgentsInPool,
            ReservedAgentId = assignedResult.Request.ReservedAgent.Id,
            ReservedAgentName = assignedResult.Request.ReservedAgent.Name,
            LockedUntil = assignedResult.Request.LockedUntil
          }.Serialize());
      }
      catch (Exception ex)
      {
        requestContext.TraceException(10015247, "ResourceService", ex);
      }
    }

    private void TraceAgentCloudOrchestrationEventsFromSprocs(
      IVssRequestContext requestContext,
      string sprocName,
      RunAgentEvent agentEvent,
      bool delivered,
      TaskAgentJobRequest agentRequest)
    {
      if (!requestContext.IsFeatureEnabled("DistributedTask.TraceExternalAgentProvisioning"))
        return;
      try
      {
        using (requestContext.CreateOrchestrationIdScope(agentEvent.AgentCloudRequestId.ToString()))
          requestContext.TraceAlways(10015245, TraceLevel.Info, "DistributedTask", "ResourceService", new
          {
            Note = "AgentCloud Orchestration event from a SProc has been raised",
            SProc = sprocName,
            AgentEvent = agentEvent.Serialize<RunAgentEvent>(),
            Delivered = delivered,
            AgentRequestId = agentRequest?.RequestId,
            AgentRequestQueueTime = agentRequest?.QueueTime,
            AgentRequestOrchestrationId = agentRequest?.OrchestrationId
          }.Serialize());
      }
      catch (Exception ex)
      {
        requestContext.TraceException(10015245, "ResourceService", ex);
      }
    }

    public void QueueRequestAssignmentJob(IVssRequestContext requestContext) => requestContext.GetService<ITeamFoundationJobService>().QueueJobsNow(requestContext, (IEnumerable<Guid>) new Guid[1]
    {
      TaskConstants.AgentRequestAssignmentJob
    });

    public async Task NotifyAgentReadyAsync(
      IVssRequestContext requestContext,
      int poolId,
      long requestId)
    {
      DistributedTaskResourceService taskResourceService = this;
      requestContext.AssertAsyncExecutionEnabled();
      requestContext = requestContext.ToPoolRequestContext();
      taskResourceService.EnsureMessageQueueStarted(requestContext);
      MethodScope methodScope = new MethodScope(requestContext, "ResourceService", nameof (NotifyAgentReadyAsync));
      try
      {
        List<TaskAgentPoolData> poolInternalAsync = await taskResourceService.GetTaskAgentPoolInternalAsync(requestContext, (IList<int>) new int[1]
        {
          poolId
        });
        // ISSUE: explicit non-virtual call
        TaskAgentJobRequest agentRequestAsync = await __nonvirtual (taskResourceService.GetAgentRequestAsync(requestContext, poolId, requestId, false));
        using (requestContext.CreateOrchestrationIdScope(agentRequestAsync.OrchestrationId))
        {
          requestContext.GetService<IOrchestrationLogTracingService>().TraceOrchestrationLogPhaseStarted(requestContext, agentRequestAsync.OrchestrationId, -1L, "Pipelines", agentRequestAsync.PlanType, "PoolService.NotifyAgentAssigned");
          using (requestContext.CreateAsyncTimeOutScope(taskResourceService.m_agentRequestSettings.AssignmentNotificationTimeout))
            await taskResourceService.GetTaskAgentExtension(requestContext, agentRequestAsync.PoolId).JobAssignedAsync(requestContext, agentRequestAsync.PoolId, agentRequestAsync.PopulateReferenceLinks(requestContext, agentRequestAsync.PoolId), true);
        }
      }
      finally
      {
        methodScope.Dispose();
      }
      methodScope = new MethodScope();
    }

    internal bool IsSessionValid(IVssRequestContext requestContext) => requestContext.TryGetItem<TaskAgentSessionData>("MS.TF.DistributedTask.SessionInfo", out TaskAgentSessionData _);

    internal void RaiseEvent<T>(
      IVssRequestContext requestContext,
      Guid serviceOwner,
      Guid hostId,
      Guid scopeId,
      string planType,
      Guid planId,
      T eventData)
      where T : JobEvent
    {
      this.EventSender.RaiseEvent<T>(requestContext, serviceOwner, hostId, scopeId, planType, planId, eventData);
    }

    internal Task RaiseEventAsync<T>(
      IVssRequestContext requestContext,
      Guid serviceOwner,
      Guid hostId,
      Guid scopeId,
      string planType,
      Guid planId,
      T eventData)
      where T : JobEvent
    {
      return this.EventSender.RaiseEventAsync<T>(requestContext, serviceOwner, hostId, scopeId, planType, planId, eventData);
    }

    public IAgentPoolSecurityProvider GetAgentPoolSecurity(
      IVssRequestContext requestContext,
      int poolId)
    {
      TaskAgentPoolData taskAgentPoolData = this.GetTaskAgentPoolInternal(requestContext.Elevate(), (IList<int>) new List<int>()
      {
        poolId
      }).FirstOrDefault<TaskAgentPoolData>();
      return taskAgentPoolData != null ? this.GetAgentPoolSecurity(requestContext, taskAgentPoolData.Pool.PoolType) : this.m_automationPoolSecurity;
    }

    internal IAgentPoolSecurityProvider GetAgentPoolSecurity(
      IVssRequestContext requestContext,
      TaskAgentPoolType poolType)
    {
      return poolType == TaskAgentPoolType.Automation ? this.m_automationPoolSecurity : this.m_deploymentPoolSecurity;
    }

    internal ITaskAgentExtension GetTaskAgentExtension(
      IVssRequestContext requestContext,
      int poolId)
    {
      TaskAgentPoolData taskAgentPoolData = this.GetTaskAgentPoolInternal(requestContext.Elevate(), (IList<int>) new List<int>()
      {
        poolId
      }).FirstOrDefault<TaskAgentPoolData>();
      if (taskAgentPoolData == null)
        throw new TaskAgentPoolNotFoundException(TaskResources.AgentPoolNotFound((object) poolId));
      return taskAgentPoolData.Pool.IsHosted ? this.m_hostedAgentExtension : this.m_defaultAgentExtension;
    }

    internal ITaskAgentPoolExtension GetTaskAgentPoolExtension(
      IVssRequestContext requestContext,
      int poolId)
    {
      TaskAgentPoolData taskAgentPoolData = this.GetTaskAgentPoolInternal(requestContext.Elevate(), (IList<int>) new List<int>()
      {
        poolId
      }).FirstOrDefault<TaskAgentPoolData>();
      return taskAgentPoolData != null ? this.GetTaskAgentPoolExtension(taskAgentPoolData.Pool.PoolType) : this.m_defaultPoolExtension;
    }

    internal void InValidatePoolCache(IVssRequestContext requestContext, int poolId) => requestContext.GetService<TaskAgentPoolCacheService>().Remove(requestContext, poolId);

    internal List<TaskAgentPoolData> GetTaskAgentPoolInternal(
      IVssRequestContext requestContext,
      IList<int> poolIds)
    {
      List<TaskAgentPoolData> agentPoolInternal = new List<TaskAgentPoolData>();
      TaskAgentPoolCacheService service = requestContext.GetService<TaskAgentPoolCacheService>();
      HashSet<int> intSet1 = new HashSet<int>();
      HashSet<int> intSet2 = (HashSet<int>) null;
      foreach (int poolId in (IEnumerable<int>) poolIds)
      {
        TaskAgentPoolData taskAgentPoolData;
        if (service.TryGetValue(requestContext, poolId, out taskAgentPoolData))
          agentPoolInternal.Add(taskAgentPoolData);
        else if (!requestContext.TryGetItem<HashSet<int>>("MS.TF.DistributedTask.PoolCacheMisses", out intSet2) || !intSet2.Contains(poolId))
          intSet1.Add(poolId);
      }
      if (intSet1.Count > 0)
      {
        List<TaskAgentPoolData> taskAgentPoolDataList = new List<TaskAgentPoolData>();
        using (TaskResourceComponent component = requestContext.CreateComponent<TaskResourceComponent>())
          taskAgentPoolDataList.AddRange((IEnumerable<TaskAgentPoolData>) component.GetAgentPoolsById(intSet1));
        foreach (TaskAgentPoolData taskAgentPoolData in taskAgentPoolDataList)
        {
          agentPoolInternal.Add(taskAgentPoolData);
          service.Set(requestContext, taskAgentPoolData.Pool.Id, taskAgentPoolData.Clone());
          intSet1.Remove(taskAgentPoolData.Pool.Id);
        }
        if (intSet1.Count > 0)
        {
          if (intSet2 == null && !requestContext.TryGetItem<HashSet<int>>("MS.TF.DistributedTask.PoolCacheMisses", out intSet2))
          {
            intSet2 = new HashSet<int>();
            requestContext.Items["MS.TF.DistributedTask.PoolCacheMisses"] = (object) intSet2;
          }
          intSet2.UnionWith((IEnumerable<int>) intSet1);
        }
      }
      return agentPoolInternal;
    }

    internal async Task<List<TaskAgentPoolData>> GetTaskAgentPoolInternalAsync(
      IVssRequestContext requestContext,
      IList<int> poolIds)
    {
      List<TaskAgentPoolData> pools = new List<TaskAgentPoolData>();
      TaskAgentPoolCacheService poolCache = requestContext.GetService<TaskAgentPoolCacheService>();
      HashSet<int> poolIdsToFetch = new HashSet<int>();
      HashSet<int> cacheMisses = (HashSet<int>) null;
      foreach (int poolId in (IEnumerable<int>) poolIds)
      {
        TaskAgentPoolData taskAgentPoolData;
        if (poolCache.TryGetValue(requestContext, poolId, out taskAgentPoolData))
          pools.Add(taskAgentPoolData);
        else if (!requestContext.TryGetItem<HashSet<int>>("MS.TF.DistributedTask.PoolCacheMisses", out cacheMisses) || !cacheMisses.Contains(poolId))
          poolIdsToFetch.Add(poolId);
      }
      if (poolIdsToFetch.Count > 0)
      {
        List<TaskAgentPoolData> fetchedPoolsData = new List<TaskAgentPoolData>();
        using (TaskResourceComponent thc = requestContext.CreateComponent<TaskResourceComponent>())
        {
          List<TaskAgentPoolData> taskAgentPoolDataList = fetchedPoolsData;
          taskAgentPoolDataList.AddRange((IEnumerable<TaskAgentPoolData>) await thc.GetAgentPoolsByIdAsync(poolIdsToFetch));
          taskAgentPoolDataList = (List<TaskAgentPoolData>) null;
        }
        foreach (TaskAgentPoolData taskAgentPoolData in fetchedPoolsData)
        {
          pools.Add(taskAgentPoolData);
          poolCache.Set(requestContext, taskAgentPoolData.Pool.Id, taskAgentPoolData.Clone());
          poolIdsToFetch.Remove(taskAgentPoolData.Pool.Id);
        }
        if (poolIdsToFetch.Count > 0)
        {
          if (cacheMisses == null && !requestContext.TryGetItem<HashSet<int>>("MS.TF.DistributedTask.PoolCacheMisses", out cacheMisses))
          {
            cacheMisses = new HashSet<int>();
            requestContext.Items["MS.TF.DistributedTask.PoolCacheMisses"] = (object) cacheMisses;
          }
          cacheMisses.UnionWith((IEnumerable<int>) poolIdsToFetch);
        }
        fetchedPoolsData = (List<TaskAgentPoolData>) null;
      }
      List<TaskAgentPoolData> poolInternalAsync = pools;
      pools = (List<TaskAgentPoolData>) null;
      poolCache = (TaskAgentPoolCacheService) null;
      poolIdsToFetch = (HashSet<int>) null;
      cacheMisses = (HashSet<int>) null;
      return poolInternalAsync;
    }

    internal void SetAgentOnline(
      IVssRequestContext requestContext,
      int poolId,
      int agentId,
      int sequenceId)
    {
      requestContext = requestContext.ToPoolRequestContext();
      AgentConnectivityResult connectivityResult;
      using (TaskResourceComponent component = requestContext.CreateComponent<TaskResourceComponent>())
      {
        requestContext.TraceInfo("ResourceService", "Pool {0}: Setting agent {1} online using sequence ID {2}", (object) poolId, (object) agentId, (object) sequenceId);
        connectivityResult = component.SetAgentOnline(poolId, agentId, sequenceId);
        if (connectivityResult.Agent != null)
        {
          if (connectivityResult.Agent.Status != TaskAgentStatus.Online)
            requestContext.TraceWarning("ResourceService", "Pool {0}: Agent {1} is still offline after update attempt.", (object) poolId, (object) agentId);
        }
      }
      if (!connectivityResult.HandledEvent)
        return;
      this.GetTaskAgentPoolExtension(requestContext, poolId).AgentConnected(requestContext, poolId, agentId);
      ITaskAgentExtension taskAgentExtension = this.GetTaskAgentExtension(requestContext, poolId);
      if (connectivityResult.Agent != null && connectivityResult.PoolData != null)
      {
        taskAgentExtension.FilterCapabilities(requestContext, connectivityResult.PoolData.Pool.Id, connectivityResult.Agent);
        requestContext.GetService<IDistributedTaskEventPublisherService>().NotifyAgentChangeEvent(requestContext, "MS.TF.DistributedTask.AgentUpdated", connectivityResult.PoolData.Pool, connectivityResult.Agent);
      }
      this.QueueRequestAssignmentJob(requestContext);
    }

    internal void SetAgentOffline(
      IVssRequestContext requestContext,
      int poolId,
      int agentId,
      int sequenceId)
    {
      requestContext = requestContext.ToPoolRequestContext();
      AgentConnectivityResult connectivityResult;
      using (TaskResourceComponent component = requestContext.CreateComponent<TaskResourceComponent>())
      {
        requestContext.TraceInfo("ResourceService", "Pool {0}: Setting agent {1} offline using sequence ID {2}", (object) poolId, (object) agentId, (object) sequenceId);
        connectivityResult = component.SetAgentOffline(poolId, agentId, sequenceId);
      }
      if (!connectivityResult.HandledEvent)
        return;
      requestContext.GetService<DistributedTaskResourceService>().GetTaskAgentPoolExtension(requestContext, poolId).AgentDisconnected(requestContext, poolId, agentId);
      if (connectivityResult.Agent == null || connectivityResult.PoolData == null)
        return;
      this.GetTaskAgentExtension(requestContext, poolId).FilterCapabilities(requestContext, connectivityResult.PoolData.Pool.Id, connectivityResult.Agent);
      requestContext.GetService<IDistributedTaskEventPublisherService>().NotifyAgentChangeEvent(requestContext, "MS.TF.DistributedTask.AgentUpdated", connectivityResult.PoolData.Pool, connectivityResult.Agent);
    }

    internal TaskAgentPool ProvisionAgentPoolRoles(
      IVssRequestContext requestContext,
      TaskAgentPool pool,
      Microsoft.VisualStudio.Services.Identity.Identity createdBy,
      IList<Microsoft.VisualStudio.Services.Identity.Identity> administrators = null,
      IList<Microsoft.VisualStudio.Services.Identity.Identity> serviceAccounts = null)
    {
      List<Microsoft.VisualStudio.Services.Identity.Identity> poolAdministrators = new List<Microsoft.VisualStudio.Services.Identity.Identity>();
      if (createdBy != null && !IdentityDescriptorComparer.Instance.Equals(createdBy.Descriptor, requestContext.ServiceHost.SystemDescriptor()))
        poolAdministrators.Add(createdBy);
      if (administrators != null)
        poolAdministrators.AddRange((IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) administrators);
      this.GetAgentPoolSecurity(requestContext, pool.PoolType).SetDefaultPermissions(requestContext, pool, (IList<Microsoft.VisualStudio.Services.Identity.Identity>) poolAdministrators);
      return pool;
    }

    internal async Task<TaskAgentJobRequest> UpdateAgentRequestMatchesAsync(
      IVssRequestContext requestContext,
      int poolId,
      long requestId,
      IList<int> matchedAgents,
      bool matchesAllAgents = false)
    {
      this.EnsureMessageQueueStarted(requestContext);
      TaskAgentJobRequest taskAgentJobRequest;
      using (new MethodScope(requestContext, "ResourceService", nameof (UpdateAgentRequestMatchesAsync)))
      {
        TaskAgentJobRequest request;
        using (TaskResourceComponent rc = requestContext.CreateComponent<TaskResourceComponent>())
          request = await rc.UpdateAgentRequestMatchesAsync(poolId, requestId, matchedAgents, matchesAllAgents);
        this.QueueRequestAssignmentJob(requestContext);
        taskAgentJobRequest = request.PopulateReferenceLinks(requestContext, poolId);
      }
      return taskAgentJobRequest;
    }

    private void QueueAgentDeprovision(
      IVssRequestContext requestContext,
      IServerPoolProvider poolProvider,
      int poolId,
      TaskAgentReference agent,
      bool isAgentDeleted = false)
    {
      TaskAgentReference agentCopy = agent.Clone();
      string taskLabel = string.Format("Notify remote provider that agent {0} is ready to be released", (object) agent.Id);
      this.m_assignmentNotificationSender.Notify(requestContext.Elevate(), taskLabel, (Func<IVssRequestContext, Task>) (async context => await this.DeprovisionAgentAsync(context, poolProvider, poolId, agentCopy, isAgentDeleted)));
    }

    private async Task DeprovisionAgentAsync(
      IVssRequestContext requestContext,
      IServerPoolProvider poolProvider,
      int poolId,
      TaskAgentReference agent,
      bool isAgentDeleted = false)
    {
      try
      {
        await poolProvider.DeprovisionAgentAsync(requestContext, poolId, agent);
      }
      catch (Exception ex)
      {
        requestContext.TraceError("ResourceService", "Failure occurred when attempting to notify pool provider of deprovision event");
        requestContext.TraceException("ResourceService", ex);
      }
      this.ClearAgentInformation(requestContext, poolId, agent.Id);
      if (isAgentDeleted)
        return;
      this.ClearAgentInformation(requestContext, poolId, agent.Id);
      agent.ProvisioningState = "Deallocated";
      TaskAgent agent1 = new TaskAgent(agent);
      this.UpdateAgent(requestContext, poolId, agent1, TaskAgentCapabilityType.None);
    }

    private void QueueRequestAssignmentNotification(
      IVssRequestContext requestContext,
      IServerPoolProvider previousProvider,
      IServerPoolProvider currentProvider,
      TaskAgentJobRequest request,
      bool clearReservedAgent = false)
    {
      TaskAgentJobRequest requestCopy = request.Clone();
      string taskLabel = string.Format("Notify agent assignment for plan : {0} request : {1}", (object) request.PlanId, (object) request.RequestId);
      if (request.ReservedAgent?.ProvisioningState == "Provisioned")
        requestContext.GetService<IOrchestrationLogTracingService>().TraceOrchestrationLogPhaseStarted(requestContext, request.OrchestrationId, -1L, "Pipelines", request.PlanType, "PoolService.QueueAssignmentNotification");
      this.m_assignmentNotificationSender.Notify(requestContext.Elevate(), taskLabel, (Func<IVssRequestContext, Task>) (async context => await this.NotifyRequestAssignmentAsync(context, previousProvider, currentProvider, requestCopy)));
      if (!clearReservedAgent)
        return;
      request.ReservedAgent = (TaskAgentReference) null;
    }

    private void NotifyResourceUsageUpdated(
      IVssRequestContext requestContext,
      List<ResourceUsageData> resourceUsageDataCollection,
      IList<ResourceLimit> resourceLimits)
    {
      foreach (IGrouping<string, ResourceUsageData> grouping in resourceUsageDataCollection.GroupBy<ResourceUsageData, string>((Func<ResourceUsageData, string>) (x => x.ResourceType)))
      {
        IGrouping<string, ResourceUsageData> usage = grouping;
        ResourceLimit resourceLimit = resourceLimits.FirstOrDefault<ResourceLimit>((Func<ResourceLimit, bool>) (x => string.Equals(usage.Key, ResourceLimitUtil.GetResourceThrottlingType(x.ParallelismTag, x.IsHosted), StringComparison.OrdinalIgnoreCase)));
        if (resourceLimit != null)
        {
          ResourceUsage usage1 = new ResourceUsage()
          {
            ResourceLimit = resourceLimit,
            UsedCount = new int?(usage.FirstOrDefault<ResourceUsageData>().RunningRequestsCount)
          };
          if (resourceLimit.IsHosted && string.Equals(resourceLimit.ParallelismTag, "Private", StringComparison.OrdinalIgnoreCase))
          {
            PlatformTaskHubLicenseService service = requestContext.GetService<PlatformTaskHubLicenseService>();
            usage1.UsedMinutes = service.GetUsedHostedMinutesForPrivateProjects(requestContext);
          }
          this.m_defaultPoolExtension.ResourceUsageUpdated(requestContext, usage1);
        }
      }
    }

    private async Task NotifyRequestAssignmentAsync(
      IVssRequestContext requestContext,
      IServerPoolProvider previousProvider,
      IServerPoolProvider currentProvider,
      TaskAgentJobRequest request)
    {
      TaskAgentReference reservedAgent = request.ReservedAgent;
      if (string.Equals(reservedAgent.ProvisioningState, "Deprovisioning"))
      {
        await this.DeprovisionAgentAsync(requestContext, previousProvider, request.PoolId, reservedAgent);
        reservedAgent.ProvisioningState = "Provisioning";
        TaskAgent agent = new TaskAgent(reservedAgent);
        this.UpdateAgent(requestContext, request.PoolId, agent, TaskAgentCapabilityType.None);
      }
      if (string.Equals(reservedAgent.ProvisioningState, "Provisioning"))
        await this.ProvisionAgentAsync(requestContext, currentProvider, request.PoolId, request);
      if (!string.Equals(reservedAgent.ProvisioningState, "Provisioned"))
      {
        reservedAgent = (TaskAgentReference) null;
      }
      else
      {
        requestContext.GetService<IOrchestrationLogTracingService>().TraceOrchestrationLogPhaseStarted(requestContext, request.OrchestrationId, -1L, "Pipelines", request.PlanType, "PoolService.NotifyAgentAssigned");
        using (requestContext.CreateAsyncTimeOutScope(this.m_agentRequestSettings.AssignmentNotificationTimeout))
          await this.GetTaskAgentExtension(requestContext, request.PoolId).JobAssignedAsync(requestContext, request.PoolId, request.PopulateReferenceLinks(requestContext, request.PoolId), currentProvider.AgentCloudId.HasValue);
        reservedAgent = (TaskAgentReference) null;
      }
    }

    private async Task ProvisionAgentAsync(
      IVssRequestContext requestContext,
      IServerPoolProvider poolProvider,
      int poolId,
      TaskAgentJobRequest request)
    {
      DistributedTaskResourceService taskResourceService1 = this;
      try
      {
        await poolProvider.ProvisionAgentAsync(requestContext, request);
      }
      catch (Exception ex)
      {
        requestContext.TraceError("ResourceService", "Failed to send provision request to pool provider, failing associated request");
        requestContext.TraceException("ResourceService", ex);
        await request.AddIssueAsync(requestContext, IssueType.Error, TaskResources.FailedToProvisionAgent());
        DistributedTaskResourceService taskResourceService2 = taskResourceService1;
        IVssRequestContext requestContext1 = requestContext;
        int poolId1 = poolId;
        long requestId = request.RequestId;
        DateTime? nullable1 = new DateTime?(DateTime.UtcNow);
        TaskResult? nullable2 = new TaskResult?(TaskResult.Failed);
        DateTime? expirationTime = new DateTime?();
        DateTime? startTime = new DateTime?();
        DateTime? finishTime = nullable1;
        TaskResult? result = nullable2;
        // ISSUE: explicit non-virtual call
        TaskAgentJobRequest taskAgentJobRequest = await __nonvirtual (taskResourceService2.UpdateAgentRequestAsync(requestContext1, poolId1, requestId, expirationTime, startTime, finishTime, result));
      }
    }

    private void QueueRequestBilling(
      IVssRequestContext requestContext,
      IServerPoolProvider provider,
      TaskAgentJobRequest request)
    {
      string taskLabel = string.Format("Bill for usage caused by Request {0}", (object) request.RequestId);
      this.m_assignmentNotificationSender.Notify(requestContext.Elevate(), taskLabel, (Func<IVssRequestContext, Task>) (async context =>
      {
        try
        {
          await provider.BillForResourcesAsync(context, request);
        }
        catch (Exception ex)
        {
          requestContext.TraceError(0, "ResourceService", "Failed to successfully bill for request {0}", (object) request.RequestId);
          requestContext.TraceException(0, "ResourceService", ex);
        }
      }));
    }

    private void ClearAgentCapabilitiesIfHosted(
      IVssRequestContext requestContext,
      int poolId,
      TaskAgent agent)
    {
      TaskAgentPool agentPool = this.GetAgentPool(requestContext, poolId, (IList<string>) null, TaskAgentPoolActionFilter.None);
      if (agentPool == null)
        throw new TaskAgentPoolNotFoundException(TaskResources.AgentPoolNotFound((object) poolId));
      if (!agentPool.IsHosted)
        return;
      agent.SystemCapabilities.Clear();
    }

    private ITaskAgentPoolExtension GetTaskAgentPoolExtension(TaskAgentPoolType poolType) => poolType == TaskAgentPoolType.Deployment ? this.m_deploymentPoolExtension : this.m_defaultPoolExtension;

    private static void SendMessageToAgent(
      IVssRequestContext requestContext,
      int poolId,
      int agentId,
      TaskAgentMessage message)
    {
      byte[] agentEncryptionKey = DistributedTaskResourceService.GetAgentEncryptionKey(requestContext, poolId, agentId);
      if (agentEncryptionKey != null)
      {
        using (AesCryptoServiceProvider cryptoServiceProvider = new AesCryptoServiceProvider())
        {
          cryptoServiceProvider.Key = agentEncryptionKey;
          cryptoServiceProvider.Mode = CipherMode.CBC;
          cryptoServiceProvider.Padding = PaddingMode.PKCS7;
          using (ICryptoTransform encryptor = cryptoServiceProvider.CreateEncryptor())
          {
            using (MemoryStream memoryStream = new MemoryStream())
            {
              using (CryptoStream cryptoStream = new CryptoStream((Stream) memoryStream, encryptor, CryptoStreamMode.Write))
              {
                using (StreamWriter streamWriter = new StreamWriter((Stream) cryptoStream, Encoding.UTF8, 1024, true))
                  streamWriter.Write(message.Body);
                cryptoStream.Flush();
                if (!cryptoStream.HasFlushedFinalBlock)
                  cryptoStream.FlushFinalBlock();
                message.IV = cryptoServiceProvider.IV;
                message.Body = Convert.ToBase64String(memoryStream.ToArray());
              }
            }
          }
        }
      }
      string queueName = MessageQueueHelpers.GetQueueName(poolId, agentId);
      requestContext.GetService<ITeamFoundationMessageQueueService>().EnqueueMessage(requestContext, queueName, typeof (TaskAgentMessage).Name, JsonUtility.ToString((object) message));
    }

    private List<TaskAgentPool> FilterAgentPools(
      IVssRequestContext requestContext,
      List<TaskAgentPool> allPools,
      IList<string> propertyFilters = null,
      TaskAgentPoolActionFilter actionFilter = TaskAgentPoolActionFilter.None)
    {
      using (new MethodScope(requestContext, "ResourceService", nameof (FilterAgentPools)))
      {
        int requiredPermissions = 0;
        if (actionFilter != TaskAgentPoolActionFilter.None)
        {
          if ((actionFilter & TaskAgentPoolActionFilter.Manage) == TaskAgentPoolActionFilter.Manage)
            requiredPermissions |= 2;
          if ((actionFilter & TaskAgentPoolActionFilter.Use) == TaskAgentPoolActionFilter.Use)
            requiredPermissions |= 16;
        }
        IAgentPoolSecurityProvider agentPoolSecurity1 = this.GetAgentPoolSecurity(requestContext, TaskAgentPoolType.Automation);
        IAgentPoolSecurityProvider agentPoolSecurity2 = this.GetAgentPoolSecurity(requestContext, TaskAgentPoolType.Deployment);
        List<TaskAgentPool> taskAgentPoolList = new List<TaskAgentPool>();
        foreach (TaskAgentPool allPool in allPools)
        {
          IAgentPoolSecurityProvider securityProvider = allPool.PoolType == TaskAgentPoolType.Deployment ? agentPoolSecurity2 : agentPoolSecurity1;
          if (securityProvider.HasPoolPermission(requestContext, allPool.Id, 1, true) && !allPool.ShouldHidePool(requestContext) && (requestContext.IsSystemContext || requiredPermissions == 0 || securityProvider.HasPoolPermission(requestContext, allPool.Id, requiredPermissions)))
          {
            allPool.SetPoolVisibility(requestContext);
            if (requestContext.IsFeatureEnabled("DistributedTask.BatchPoolIdentityRequests"))
              taskAgentPoolList.Add(allPool);
            else
              taskAgentPoolList.Add(DistributedTaskResourceService.PopulateIdentityReferences(requestContext, allPool));
          }
        }
        if (taskAgentPoolList.Count > 0)
        {
          if (requestContext.IsFeatureEnabled("DistributedTask.BatchPoolIdentityRequests"))
            taskAgentPoolList = DistributedTaskResourceService.PopulateIdentityReferences(requestContext, taskAgentPoolList);
          if (propertyFilters != null && propertyFilters.Count > 0)
          {
            using (TeamFoundationDataReader properties = requestContext.GetService<ITeamFoundationPropertyService>().GetProperties(requestContext, taskAgentPoolList.Select<TaskAgentPool, ArtifactSpec>((Func<TaskAgentPool, ArtifactSpec>) (x => x.CreateSpec())), (IEnumerable<string>) propertyFilters))
              ArtifactPropertyKinds.MatchProperties<TaskAgentPool>(properties, (IList<TaskAgentPool>) taskAgentPoolList, (Func<TaskAgentPool, int>) (x => x.Id), (Action<TaskAgentPool, PropertiesCollection>) ((x, y) => x.Properties = y));
          }
        }
        return taskAgentPoolList;
      }
    }

    private static Task SendMessageToAgentAsync(
      IVssRequestContext requestContext,
      int poolId,
      int agentId,
      TaskAgentMessage message)
    {
      byte[] agentEncryptionKey = DistributedTaskResourceService.GetAgentEncryptionKey(requestContext, poolId, agentId);
      if (agentEncryptionKey != null)
      {
        using (AesCryptoServiceProvider cryptoServiceProvider = new AesCryptoServiceProvider())
        {
          cryptoServiceProvider.Key = agentEncryptionKey;
          cryptoServiceProvider.Mode = CipherMode.CBC;
          cryptoServiceProvider.Padding = PaddingMode.PKCS7;
          using (ICryptoTransform encryptor = cryptoServiceProvider.CreateEncryptor())
          {
            using (MemoryStream memoryStream = new MemoryStream())
            {
              using (CryptoStream cryptoStream = new CryptoStream((Stream) memoryStream, encryptor, CryptoStreamMode.Write))
              {
                using (StreamWriter streamWriter = new StreamWriter((Stream) cryptoStream, Encoding.UTF8, 1024, true))
                  streamWriter.Write(message.Body);
                cryptoStream.Flush();
                if (!cryptoStream.HasFlushedFinalBlock)
                  cryptoStream.FlushFinalBlock();
                message.IV = cryptoServiceProvider.IV;
                message.Body = Convert.ToBase64String(memoryStream.ToArray());
              }
            }
          }
        }
      }
      string queueName = MessageQueueHelpers.GetQueueName(poolId, agentId);
      return (Task) requestContext.GetService<ITeamFoundationMessageQueueService>().EnqueueMessageAsync(requestContext, queueName, typeof (TaskAgentMessage).Name, JsonUtility.ToString((object) message));
    }

    private void CleanupQueueRolesAndSecurity(
      IVssRequestContext requestContext,
      Guid projectId,
      TaskAgentQueue deletedQueue,
      bool removePoolPermissions = false)
    {
      try
      {
        this.Security.RemoveAccessControlLists(requestContext, projectId, deletedQueue);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(10015013, "ResourceService", ex);
      }
      if (removePoolPermissions)
      {
        Microsoft.VisualStudio.Services.Identity.Identity identity = requestContext.GetService<IdentityService>().GetGroups(requestContext, projectId, (IEnumerable<IdentityDescriptor>) new IdentityDescriptor[1]
        {
          GroupWellKnownIdentityDescriptors.EveryoneGroup
        }).FirstOrDefault<Microsoft.VisualStudio.Services.Identity.Identity>();
        if (identity != null)
          this.GetAgentPoolSecurity(requestContext, TaskAgentPoolType.Automation).RevokeReadPermissionToPool(requestContext.ToPoolRequestContext(), deletedQueue.Pool.Id, identity);
        else
          requestContext.TraceError(10015056, "ResourceService", "Cannot find Project everyone group on the project {0}, Queue: {1}, Pool: {2}", (object) projectId, (object) deletedQueue.Id, (object) deletedQueue.Pool.Id);
      }
      try
      {
        IPipelineResourceAuthorizationProxyService authorizationProxyService = requestContext.GetService<IPipelineResourceAuthorizationProxyService>();
        requestContext.RunSynchronously((Func<Task>) (() => authorizationProxyService.DeletePipelinePermissionsForResource(requestContext.Elevate(), projectId, deletedQueue.Id.ToString(), "queue")));
      }
      catch (Exception ex)
      {
        string format = "Deleting pipeline permissions for agent queue with ID: {0} failed with the following error: {1}";
        requestContext.TraceError(10015178, "DistributedTask", format, (object) deletedQueue.Id.ToString(), (object) ex.Message);
      }
    }

    private void ThrowIfHostedPool(IVssRequestContext requestContext, int poolId)
    {
      TaskAgentPool agentPool = this.GetAgentPool(requestContext, poolId, (IList<string>) null, TaskAgentPoolActionFilter.None);
      if (agentPool != null && (agentPool.IsHosted || agentPool.AgentCloudId.HasValue))
        throw new Microsoft.TeamFoundation.DistributedTask.WebApi.AccessDeniedException(TaskResources.AccessDeniedForHostedPool());
    }

    private async Task<RequestAgentsUpdateResult> SendRefreshMessageToAgentsInternalAsync(
      IVssRequestContext requestContext,
      int poolId,
      IList<TaskAgent> agents,
      TaskAgentUpdateReasonData reason)
    {
      IPackageMetadataService service1 = requestContext.GetService<IPackageMetadataService>();
      Dictionary<string, List<TaskAgent>> dictionary = new Dictionary<string, List<TaskAgent>>();
      bool flag = requestContext.IsFeatureEnabled("DistributedTask.Agent.MajorUpgradeDisabled");
      foreach (TaskAgent agent in (IEnumerable<TaskAgent>) agents)
      {
        if (agent.Status != TaskAgentStatus.Offline && !(agent.Version == DistributedTaskResourceService.s_legacyXPlatAgentVersion.ToString()) && !(agent.Version == DistributedTaskResourceService.s_legacyXPlatAgentVersion2.ToString()) && new Version(agent.Version).Major != DistributedTaskResourceService.s_legacyWindowsAgentMajorVersion)
        {
          PackageMetadata compatiblePackage = service1.GetLatestCompatiblePackage(requestContext, TaskAgentConstants.AgentPackageType, agent);
          PackageVersion other = new PackageVersion(agent.Version);
          if (((other.Major != 2 ? 0 : (compatiblePackage.Version.Major == 3 ? 1 : 0)) & (flag ? 1 : 0)) == 0 && compatiblePackage.Version.CompareTo(other) != 0 && other.CompareTo(DistributedTaskResourceService.s_coreAgentSelfUpdateVersion) >= 0)
          {
            List<TaskAgent> taskAgentList;
            if (!dictionary.TryGetValue(compatiblePackage.Version.ToString(), out taskAgentList))
            {
              taskAgentList = new List<TaskAgent>();
              dictionary[compatiblePackage.Version.ToString()] = taskAgentList;
            }
            taskAgentList.Add(agent);
          }
        }
      }
      RequestAgentsUpdateResult updateResult = new RequestAgentsUpdateResult();
      Guid requestedBy = requestContext.GetUserIdentity().Id;
      using (TaskResourceComponent thc = requestContext.CreateComponent<TaskResourceComponent>())
      {
        foreach (KeyValuePair<string, List<TaskAgent>> keyValuePair in dictionary)
        {
          RequestAgentsUpdateResult agentsUpdateResult = await thc.RequestAgentsUpdateAsync(poolId, keyValuePair.Value.Select<TaskAgent, int>((Func<TaskAgent, int>) (a => a.Id)), keyValuePair.Key, requestedBy, TaskResources.SendAgentUpdateMessage(), reason);
          updateResult.NewUpdates.AddRange((IEnumerable<TaskAgent>) agentsUpdateResult.NewUpdates);
          updateResult.ExistingUpdates.AddRange((IEnumerable<TaskAgent>) agentsUpdateResult.ExistingUpdates);
        }
      }
      if (updateResult.NewUpdates.Count > 0)
      {
        ITeamFoundationJobService service2 = requestContext.GetService<ITeamFoundationJobService>();
        XmlDocument xmlDocument = new XmlDocument();
        XmlNode element1 = (XmlNode) xmlDocument.CreateElement("AgentUpdate");
        XmlNode element2 = (XmlNode) xmlDocument.CreateElement("PoolId");
        element2.AppendChild((XmlNode) xmlDocument.CreateTextNode(poolId.ToString()));
        element1.AppendChild(element2);
        TeamFoundationJobDefinition foundationJobDefinition = new TeamFoundationJobDefinition()
        {
          Data = element1,
          EnabledState = TeamFoundationJobEnabledState.Enabled,
          ExtensionName = "Microsoft.TeamFoundation.DistributedTask.Server.Extensions.AgentUpdateMonitorJob",
          JobId = Guid.NewGuid(),
          Name = "Agent update moniter job",
          IgnoreDormancy = service2.IsIgnoreDormancyPermitted,
          PriorityClass = JobPriorityClass.Normal
        };
        service2.UpdateJobDefinitions(requestContext.Elevate(), (IEnumerable<Guid>) null, (IEnumerable<TeamFoundationJobDefinition>) new TeamFoundationJobDefinition[1]
        {
          foundationJobDefinition
        });
        service2.QueueDelayedJobs(requestContext, (IEnumerable<Guid>) new Guid[1]
        {
          foundationJobDefinition.JobId
        }, (int) TimeSpan.FromMinutes(20.0).TotalSeconds);
      }
      foreach (TaskAgent newUpdate in updateResult.NewUpdates)
      {
        await DistributedTaskResourceService.SendMessageToAgentAsync(requestContext, poolId, newUpdate.Id, new TaskAgentMessage()
        {
          MessageType = AgentRefreshMessage.MessageType,
          Body = JsonUtility.ToString((object) new AgentRefreshMessage(newUpdate.Id, (string) newUpdate.PendingUpdate.TargetVersion))
        });
        this.GetTaskAgentPoolExtension(requestContext, poolId).AgentUpdated(requestContext, poolId, newUpdate);
      }
      RequestAgentsUpdateResult agentsInternalAsync = updateResult;
      updateResult = (RequestAgentsUpdateResult) null;
      return agentsInternalAsync;
    }

    private void CheckViewAndOtherPermissionsForAgent(
      IVssRequestContext requestContext,
      int poolId,
      int? agentId,
      long? requestId,
      int otherPermissions,
      int? fallbackPermissions = null)
    {
      requestContext.RunSynchronously((Func<Task>) (() => this.CheckViewAndOtherPermissionsForAgentAsync(requestContext, poolId, agentId, requestId, otherPermissions, fallbackPermissions)));
    }

    private async Task CheckViewAndOtherPermissionsForAgentAsync(
      IVssRequestContext requestContext,
      int poolId,
      int? agentId,
      long? requestId,
      int otherPermissions,
      int? fallbackPermissions = null)
    {
      DistributedTaskResourceService taskResourceService = this;
      // ISSUE: explicit non-virtual call
      IAgentPoolSecurityProvider poolSecurity = __nonvirtual (taskResourceService.GetAgentPoolSecurity(requestContext, poolId));
      string scopes = requestContext.GetScopeListString();
      bool usingAgentRequestScopes = scopes.Contains(poolSecurity.GetAgentPoolToken(poolId));
      bool usingAgentCloudRequestScopes = scopes.Contains(DefaultAgentCloudSecurityProvider.AgentCloudToken);
      TaskResourceComponent rc;
      if (usingAgentRequestScopes)
      {
        taskResourceService.GetTaskAgentPoolInternal(requestContext.Elevate(), (IList<int>) new int[1]
        {
          poolId
        }).FirstOrDefault<TaskAgentPoolData>();
        if (!agentId.HasValue && requestId.HasValue)
        {
          rc = requestContext.CreateComponent<TaskResourceComponent>();
          try
          {
            agentId = (await rc.GetAgentRequestAsync(poolId, requestId.Value)).AgentRequest?.ReservedAgent?.Id;
          }
          finally
          {
            rc?.Dispose();
          }
          rc = (TaskResourceComponent) null;
        }
        else if (!requestId.HasValue && agentId.HasValue)
        {
          rc = requestContext.CreateComponent<TaskResourceComponent>();
          try
          {
            requestId = (await rc.GetAgentsByIdAsync(poolId, (IEnumerable<int>) new int[1]
            {
              agentId.Value
            }, includeAssignedRequest: true)).FirstOrDefault<TaskAgent>()?.AssignedRequest?.RequestId;
          }
          finally
          {
            rc?.Dispose();
          }
          rc = (TaskResourceComponent) null;
        }
      }
      else if (usingAgentCloudRequestScopes && (agentId.HasValue || requestId.HasValue))
      {
        TaskAgentCloudRequest agentCloudRequest = (TaskAgentCloudRequest) null;
        IInternalAgentCloudService agentCloudService = requestContext.GetService<IInternalAgentCloudService>();
        if (agentId.HasValue)
        {
          agentCloudRequest = await agentCloudService.GetAgentCloudRequestForAgentAsync(requestContext, poolId, agentId.Value);
        }
        else
        {
          rc = requestContext.CreateComponent<TaskResourceComponent>();
          try
          {
            agentCloudRequest = (await rc.GetAgentRequestAsync(poolId, requestId.Value))?.CloudRequest;
          }
          finally
          {
            rc?.Dispose();
          }
          rc = (TaskResourceComponent) null;
        }
        if (agentCloudRequest != null && agentCloudService.HasAgentCloudListenPermission(requestContext, agentCloudRequest.AgentCloudId, agentCloudRequest.RequestId))
        {
          poolSecurity = (IAgentPoolSecurityProvider) null;
          scopes = (string) null;
          return;
        }
        agentCloudService = (IInternalAgentCloudService) null;
      }
      if (!poolSecurity.HasAgentPermission(requestContext, poolId, agentId, requestId, 1))
      {
        if (usingAgentRequestScopes | usingAgentCloudRequestScopes)
        {
          requestContext.TraceAlways(10015168, TraceLevel.Warning, "DistributedTask", "ResourceService", "Scoped agent token failed permission check on pool {0}, agent (1}, and request {2}\nScope List:\n{3}", (object) poolId, (object) agentId, (object) requestId, (object) scopes);
          throw new TaskAgentAccessTokenExpiredException(TaskResources.AgentAccessTokenExpired());
        }
        throw new TaskAgentPoolNotFoundException(TaskResources.AgentPoolNotFound((object) poolId));
      }
      if (poolSecurity.HasAgentPermission(requestContext, poolId, agentId, requestId, otherPermissions))
      {
        poolSecurity = (IAgentPoolSecurityProvider) null;
        scopes = (string) null;
      }
      else if (fallbackPermissions.HasValue && poolSecurity.HasAgentPermission(requestContext, poolId, agentId, requestId, fallbackPermissions.Value))
      {
        poolSecurity = (IAgentPoolSecurityProvider) null;
        scopes = (string) null;
      }
      else
      {
        PoolSecurityProvider.ThrowPoolAccessDeniedException(requestContext, otherPermissions, poolId);
        poolSecurity = (IAgentPoolSecurityProvider) null;
        scopes = (string) null;
      }
    }

    private void CheckViewAndOtherPermissionsForPool(
      IVssRequestContext requestContext,
      int poolId,
      int otherPermissions,
      int? fallbackPermissions = null)
    {
      IAgentPoolSecurityProvider agentPoolSecurity = this.GetAgentPoolSecurity(requestContext, poolId);
      if (!agentPoolSecurity.HasPoolPermission(requestContext, poolId, 1))
        throw new TaskAgentPoolNotEnoughPermissionsException(TaskResources.AgentPoolNotEnoughPermissions());
      if (agentPoolSecurity.HasPoolPermission(requestContext, poolId, otherPermissions) || fallbackPermissions.HasValue && agentPoolSecurity.HasPoolPermission(requestContext, poolId, fallbackPermissions.Value))
        return;
      PoolSecurityProvider.ThrowPoolAccessDeniedException(requestContext, otherPermissions, poolId);
    }

    private void CheckViewAndOtherPermissionsForQueue(
      IVssRequestContext requestContext,
      Guid projectId,
      int queueId,
      int otherPermissions,
      int? fallbackPermissions = null)
    {
      if (!this.Security.HasQueuePermission(requestContext, projectId, queueId, 1))
        throw new TaskAgentQueueNotFoundException(TaskResources.QueueNotFound((object) queueId));
      if (this.Security.HasQueuePermission(requestContext, projectId, queueId, otherPermissions) || fallbackPermissions.HasValue && this.Security.HasQueuePermission(requestContext, projectId, queueId, fallbackPermissions.Value))
        return;
      DefaultSecurityProvider.ThrowQueueAccessDeniedException(requestContext, projectId, otherPermissions, queueId);
    }

    private void CheckHostedPoolPermissions(
      IVssRequestContext requestContext,
      int poolId,
      TaskAgent agent = null)
    {
      if (requestContext.IsSystemContext)
        return;
      this.CheckHostedPoolPermissions(requestContext, this.GetAgentPool(requestContext, poolId, (IList<string>) null, TaskAgentPoolActionFilter.None) ?? throw new TaskAgentPoolNotFoundException(TaskResources.AgentPoolNotFound((object) poolId)), agent);
    }

    private void CheckHostedPoolPermissions(
      IVssRequestContext requestContext,
      TaskAgentPool pool,
      TaskAgent agent = null)
    {
      if (!pool.IsHosted || requestContext.IsSystemContext || this.IsServicePrincipalOrDeploymentAdmin(requestContext))
        return;
      if (agent == null)
        throw new Microsoft.TeamFoundation.DistributedTask.WebApi.AccessDeniedException(TaskResources.AccessDeniedForHostedPool());
      throw new Microsoft.TeamFoundation.DistributedTask.WebApi.AccessDeniedException(TaskResources.CannotModifyAgentsForHostedPool((object) pool.Name));
    }

    private void CheckAgentCloudPoolPermissions(
      IVssRequestContext requestContext,
      int poolId,
      TaskAgent agent = null)
    {
      if (requestContext.IsSystemContext)
        return;
      TaskAgentPool agentPool = this.GetAgentPool(requestContext, poolId, (IList<string>) null, TaskAgentPoolActionFilter.None);
      if (agentPool == null)
        throw new TaskAgentPoolNotFoundException(TaskResources.AgentPoolNotFound((object) poolId));
      if (!agentPool.AgentCloudId.HasValue || requestContext.IsSystemContext)
        return;
      if (agent == null)
        throw new Microsoft.TeamFoundation.DistributedTask.WebApi.AccessDeniedException(TaskResources.AccessDeniedForAgentCloudPool());
      throw new Microsoft.TeamFoundation.DistributedTask.WebApi.AccessDeniedException(TaskResources.CannotModifyAgentsForAgentCloudBackedPool((object) agentPool.Name));
    }

    private bool IsServicePrincipalOrDeploymentAdmin(IVssRequestContext requestContext)
    {
      if (requestContext.ExecutionEnvironment.IsDevFabricDeployment && requestContext.UserContext.Identifier.StartsWith("DEADBEEF-0000-8888-8000-000000000000"))
        return true;
      List<IdentityDescriptor> groupsToCheck = new List<IdentityDescriptor>()
      {
        GroupWellKnownIdentityDescriptors.NamespaceAdministratorsGroup,
        GroupWellKnownIdentityDescriptors.ServicePrincipalGroup
      };
      return DistributedTaskResourceService.IsMemberOfOneOfTheGroups(requestContext, groupsToCheck);
    }

    private static bool IsMemberOfOneOfTheGroups(
      IVssRequestContext requestContext,
      List<IdentityDescriptor> groupsToCheck)
    {
      try
      {
        if (!requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
          return DistributedTaskResourceService.IsMemberOfOneOfTheGroups(requestContext.To(TeamFoundationHostType.Deployment), groupsToCheck);
        IdentityService identityService = requestContext.GetService<IdentityService>();
        IdentityDescriptor authenticatedDescriptor = requestContext.GetAuthenticatedDescriptor();
        return groupsToCheck.Any<IdentityDescriptor>((Func<IdentityDescriptor, bool>) (groupDescriptor => identityService.IsMember(requestContext.Elevate(), groupDescriptor, authenticatedDescriptor)));
      }
      catch (Exception ex)
      {
        return false;
      }
    }

    private TaskAgentSessionData EnsureSession(
      IVssRequestContext requestContext,
      int poolId,
      Guid sessionId,
      bool throwException = true)
    {
      bool flag = false;
      TaskAgentSessionCacheService service = requestContext.GetService<TaskAgentSessionCacheService>();
      TaskAgentSessionData agentSessionData;
      if (!service.TryGetValue(requestContext, sessionId, out agentSessionData))
      {
        flag = true;
        using (TaskResourceComponent component = requestContext.CreateComponent<TaskResourceComponent>())
          agentSessionData = component.GetAgentSessions(new int?(poolId), new Guid?(sessionId)).FirstOrDefault<TaskAgentSessionData>();
      }
      if (throwException && (agentSessionData == null || agentSessionData.PoolId != poolId))
      {
        if (service.IsRevoked(requestContext, sessionId))
          throw new TaskAgentSessionDeletedException(TaskResources.AgentSessionRevoked((object) sessionId));
        throw new TaskAgentSessionExpiredException(TaskResources.AgentSessionExpired((object) sessionId));
      }
      if (!throwException && agentSessionData == null)
        requestContext.TraceAlways(10015167, TraceLevel.Warning, "DistributedTask", "ResourceService", "No session found for pool {0} and session id {1}!", (object) poolId, (object) sessionId);
      if (agentSessionData != null)
      {
        if (flag)
          service.Set(requestContext, sessionId, agentSessionData);
        requestContext.Items["MS.TF.DistributedTask.SessionInfo"] = (object) agentSessionData;
      }
      return agentSessionData;
    }

    private static TaskAgentQueryResult FilterAgents(
      TaskAgentList agentList,
      IList<Demand> demands,
      IDictionary<int, IDictionary<string, string>> capabilityCache = null)
    {
      if (demands == null || demands.Count == 0)
        return new TaskAgentQueryResult((IEnumerable<Demand>) demands, (JObject) null, (IEnumerable<TaskAgent>) agentList.Agents, agentList.ReturnedAllAgentsInPool);
      if (capabilityCache == null)
        capabilityCache = (IDictionary<int, IDictionary<string, string>>) new Dictionary<int, IDictionary<string, string>>();
      TaskAgentQueryResult agentQueryResult = new TaskAgentQueryResult((IEnumerable<Demand>) demands, agentList.ReturnedAllAgentsInPool);
      foreach (TaskAgent agent in (IEnumerable<TaskAgent>) agentList.Agents)
      {
        IDictionary<string, string> agentCapabilities;
        if (!capabilityCache.TryGetValue(agent.Id, out agentCapabilities))
        {
          agentCapabilities = agent.GetEffectiveCapabilities();
          capabilityCache.Add(agent.Id, agentCapabilities);
        }
        List<Demand> list = demands.Where<Demand>((Func<Demand, bool>) (x => !x.IsSatisfied(agentCapabilities))).ToList<Demand>();
        if (list.Count == 0)
          agentQueryResult.MatchedAgents.Add(agent);
        else
          agentQueryResult.UnmatchedAgents.Add(new Tuple<TaskAgent, IList<Demand>>(agent, (IList<Demand>) list));
      }
      return agentQueryResult;
    }

    private void CheckServerReadPermission(IVssRequestContext requestContext) => requestContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(requestContext, FrameworkSecurity.FrameworkNamespaceId).CheckPermission(requestContext, FrameworkSecurity.FrameworkNamespaceToken, 1);

    private void CheckForSupportedAgentVersion(
      IVssRequestContext requestContext,
      int poolId,
      string agentVersion)
    {
      if (!requestContext.IsFeatureEnabled("DistributedTask.DeprecateLegacyAgent"))
        return;
      TaskAgentPool agentPool = this.GetAgentPool(requestContext.Elevate(), poolId, (IList<string>) null, TaskAgentPoolActionFilter.None);
      if ((agentPool == null || !agentPool.IsHosted && !agentPool.AgentCloudId.HasValue) && DemandMinimumVersion.ParseVersion(agentVersion).Major < 2)
        throw new TaskAgentVersionNotSupportedException(TaskResources.UsingDeprecatedAgent((object) agentVersion));
    }

    private static void DeleteAgentEncryptionKeyStore(
      IVssRequestContext requestContext,
      int poolId,
      int agentId)
    {
      ITeamFoundationStrongBoxService service = requestContext.GetService<ITeamFoundationStrongBoxService>();
      string name = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "ms.vss.distributedtask.pools.{0}.agents.{1}", (object) poolId, (object) agentId);
      Guid drawerId = service.UnlockDrawer(requestContext.Elevate(), name, false);
      if (!(drawerId != Guid.Empty))
        return;
      service.DeleteDrawer(requestContext.Elevate(), drawerId);
    }

    private static byte[] GetAgentEncryptionKey(
      IVssRequestContext requestContext,
      int poolId,
      int agentId,
      bool createIfNotExists = false)
    {
      ITeamFoundationStrongBoxService service1 = requestContext.GetService<ITeamFoundationStrongBoxService>();
      string drawerName = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "ms.vss.distributedtask.pools.{0}.agents.{1}", (object) poolId, (object) agentId);
      StrongBoxItemInfo itemInfo1 = service1.GetItemInfo(requestContext.Elevate(), drawerName, "/keys/encryption", false);
      if (itemInfo1 != null)
      {
        string s = service1.GetString(requestContext.Elevate(), itemInfo1);
        if (!string.IsNullOrEmpty(s))
          return Convert.FromBase64String(s);
      }
      if (requestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection))
      {
        IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Application);
        if (!vssRequestContext.IsVirtualServiceHost())
        {
          ITeamFoundationStrongBoxService service2 = vssRequestContext.GetService<ITeamFoundationStrongBoxService>();
          StrongBoxItemInfo itemInfo2 = service2.GetItemInfo(vssRequestContext.Elevate(), drawerName, "/keys/encryption", false);
          if (itemInfo2 != null)
          {
            string s = service2.GetString(vssRequestContext.Elevate(), itemInfo2);
            if (!string.IsNullOrEmpty(s))
            {
              byte[] key = Convert.FromBase64String(s);
              DistributedTaskResourceService.SetAgentEncryptionKey(requestContext, poolId, agentId, key);
              return key;
            }
          }
        }
      }
      if (!createIfNotExists)
        return (byte[]) null;
      using (AesCryptoServiceProvider cryptoServiceProvider = new AesCryptoServiceProvider())
      {
        cryptoServiceProvider.KeySize = 256;
        cryptoServiceProvider.Mode = CipherMode.CBC;
        cryptoServiceProvider.Padding = PaddingMode.PKCS7;
        cryptoServiceProvider.GenerateKey();
        DistributedTaskResourceService.SetAgentEncryptionKey(requestContext, poolId, agentId, cryptoServiceProvider.Key);
        return cryptoServiceProvider.Key;
      }
    }

    internal static void SetAgentEncryptionKey(
      IVssRequestContext requestContext,
      int poolId,
      int agentId,
      byte[] key)
    {
      ITeamFoundationStrongBoxService service = requestContext.GetService<ITeamFoundationStrongBoxService>();
      string name = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "ms.vss.distributedtask.pools.{0}.agents.{1}", (object) poolId, (object) agentId);
      Guid drawerId = service.UnlockDrawer(requestContext.Elevate(), name, false);
      if (drawerId == Guid.Empty)
      {
        try
        {
          drawerId = service.CreateDrawer(requestContext.Elevate(), name);
        }
        catch (StrongBoxDrawerExistsException ex)
        {
          drawerId = service.UnlockDrawer(requestContext.Elevate(), name, true);
        }
      }
      service.AddString(requestContext.Elevate(), drawerId, "/keys/encryption", Convert.ToBase64String(key));
    }

    private static bool IsMissingAgentVersionOnly(IList<Demand> demands) => demands.Count == 1 && demands[0] is DemandMinimumVersion && demands[0].Name.Equals(PipelineConstants.AgentVersionDemandName, StringComparison.OrdinalIgnoreCase);

    private static TaskAgentPool PopulateIdentityReferences(
      IVssRequestContext requestContext,
      TaskAgentPool newPool)
    {
      if (newPool == null)
        return newPool;
      if (requestContext.IsFeatureEnabled("DistributedTask.BatchPoolIdentityRequests"))
        return DistributedTaskResourceService.PopulateIdentityReferences(requestContext, new List<TaskAgentPool>()
        {
          newPool
        }).FirstOrDefault<TaskAgentPool>();
      if (newPool.CreatedBy != null || newPool.Owner != null)
      {
        IdentityService service = requestContext.GetService<IdentityService>();
        if (newPool.CreatedBy != null)
        {
          requestContext.TraceVerbose("ResourceService", "Trying to resolve identity - Pool ID: {0}, scope: {1}, CreatedBy: {2}", (object) newPool.Id, (object) newPool.Scope, (object) newPool.CreatedBy.Serialize<IdentityRef>());
          newPool.CreatedBy = service.GetIdentity(requestContext, newPool.CreatedBy).ToIdentityRef(requestContext);
        }
        if (newPool.Owner != null)
        {
          requestContext.TraceVerbose("ResourceService", "Trying to resolve identity - Pool ID: {0}, scope: {1}, Owner: {2}", (object) newPool.Id, (object) newPool.Scope, (object) newPool.Owner.Serialize<IdentityRef>());
          newPool.Owner = service.GetIdentity(requestContext, newPool.Owner).ToIdentityRef(requestContext);
        }
        else
          newPool.Owner = newPool.CreatedBy;
        IVssRequestContext requestContext1 = requestContext;
        object[] objArray = new object[4]
        {
          (object) newPool.Id,
          (object) newPool.Scope,
          null,
          null
        };
        IdentityRef createdBy = newPool.CreatedBy;
        objArray[2] = (object) (createdBy != null ? createdBy.Serialize<IdentityRef>() : (string) null);
        IdentityRef owner = newPool.Owner;
        objArray[3] = (object) (owner != null ? owner.Serialize<IdentityRef>() : (string) null);
        requestContext1.TraceVerbose("ResourceService", "Resolved identities - Pool ID: {0}, scope: {1}, CreatedBy: {2}, Owner: {3}", objArray);
      }
      return newPool;
    }

    private static List<TaskAgentPool> PopulateIdentityReferences(
      IVssRequestContext requestContext,
      List<TaskAgentPool> newPools)
    {
      IdentityService service = requestContext.GetService<IdentityService>();
      HashSet<Guid> guidSet = new HashSet<Guid>();
      foreach (TaskAgentPool newPool in newPools)
      {
        if (newPool != null)
        {
          Guid result1;
          if (newPool.CreatedBy != null && Guid.TryParse(newPool.CreatedBy.Id, out result1))
            guidSet.Add(result1);
          Guid result2;
          if (newPool.Owner != null && Guid.TryParse(newPool.Owner.Id, out result2))
            guidSet.Add(result2);
        }
      }
      if (guidSet.Count > 0)
      {
        Dictionary<string, IdentityRef> dictionary = new Dictionary<string, IdentityRef>();
        foreach (Microsoft.VisualStudio.Services.Identity.Identity identity in service.GetIdentities(requestContext, (IEnumerable<Guid>) guidSet))
        {
          if (identity != null)
          {
            IdentityRef identityRef = identity.ToIdentityRef(requestContext);
            if (!string.IsNullOrEmpty(identityRef.Id))
              dictionary[identityRef.Id] = identityRef;
          }
        }
        if (dictionary.Count < guidSet.Count)
        {
          List<string> list = dictionary.Keys.Except<string>(guidSet.Select<Guid, string>((Func<Guid, string>) (id => id.ToString()))).ToList<string>();
          if (list.Count > 0)
            requestContext.TraceVerbose("ResourceService", "Unresolvable identities from batch call: {0}", (object) string.Join(";", (IEnumerable<string>) list));
        }
        foreach (TaskAgentPool newPool in newPools)
        {
          if (newPool != null)
          {
            newPool.CreatedBy = newPool.CreatedBy == null || !dictionary.ContainsKey(newPool.CreatedBy.Id) ? (IdentityRef) null : dictionary[newPool.CreatedBy.Id];
            newPool.Owner = newPool.Owner == null || !dictionary.ContainsKey(newPool.Owner.Id) ? newPool.CreatedBy : dictionary[newPool.Owner.Id];
          }
        }
      }
      return newPools;
    }

    internal static void QueueAgentRematchJob(IVssRequestContext requestContext)
    {
      ITeamFoundationJobService service = requestContext.GetService<ITeamFoundationJobService>();
      try
      {
        service.QueueJobsNow(requestContext, (IEnumerable<Guid>) new Guid[1]
        {
          TaskConstants.AgentCapabilityUpdateJob
        }, false);
      }
      catch (JobDefinitionNotFoundException ex)
      {
        TeamFoundationJobDefinition foundationJobDefinition = new TeamFoundationJobDefinition()
        {
          ExtensionName = "Microsoft.TeamFoundation.DistributedTask.Server.Extensions.AgentCapabilityUpdateJob",
          Name = "Agent Capability Update Job",
          JobId = TaskConstants.AgentCapabilityUpdateJob,
          PriorityClass = JobPriorityClass.Normal,
          Schedule = {
            new TeamFoundationJobSchedule()
            {
              Interval = (int) TimeSpan.FromHours(1.0).TotalSeconds,
              ScheduledTime = new DateTime(2011, 7, 3, 9, 0, 0, DateTimeKind.Utc)
            }
          }
        };
        service.UpdateJobDefinitions(requestContext.Elevate(), (IEnumerable<Guid>) null, (IEnumerable<TeamFoundationJobDefinition>) new TeamFoundationJobDefinition[1]
        {
          foundationJobDefinition
        });
        service.QueueJobsNow(requestContext, (IEnumerable<Guid>) new Guid[1]
        {
          TaskConstants.AgentCapabilityUpdateJob
        }, false);
      }
    }

    public Microsoft.VisualStudio.Services.Identity.Identity ProvisionServiceIdentity(
      IVssRequestContext poolRequestContext,
      TaskAgentPoolData poolData,
      AgentPoolServiceAccountRoles role)
    {
      string str1 = (string) null;
      switch (role)
      {
        case AgentPoolServiceAccountRoles.AgentPoolService:
          str1 = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Agent Pool Service ({0})", (object) poolData.Pool.Id);
          break;
        case AgentPoolServiceAccountRoles.AgentPoolAdmin:
          str1 = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Agent Pool Administrator ({0})", (object) poolData.Pool.Id);
          break;
      }
      string str2 = AgentPoolRoles.ToString(role);
      IVssRequestContext vssRequestContext = poolRequestContext.Elevate();
      IdentityService service = vssRequestContext.GetService<IdentityService>();
      IVssRequestContext requestContext1 = vssRequestContext;
      string role1 = str2;
      Guid serviceAccountId = poolData.ServiceAccountId;
      string identifier1 = serviceAccountId.ToString("D");
      Microsoft.VisualStudio.Services.Identity.Identity frameworkIdentity = IdentityHelper.GetFrameworkIdentity(requestContext1, FrameworkIdentityType.ServiceIdentity, role1, identifier1);
      if (frameworkIdentity != null)
      {
        vssRequestContext.TraceVerbose(0, "ResourceService", "Found service identity with ID {0}", (object) frameworkIdentity.Id);
        if (!frameworkIdentity.IsActive)
        {
          vssRequestContext.TraceError(10015032, "ResourceService", "Service identity with ID {0} is not active in scope {1}", (object) frameworkIdentity.Id, (object) vssRequestContext.ServiceHost.InstanceId);
          Microsoft.VisualStudio.Services.Identity.Identity identity = service.GetIdentity(vssRequestContext, GroupWellKnownIdentityDescriptors.SecurityServiceGroup);
          if (identity != null)
          {
            if (service.AddMemberToGroup(vssRequestContext, identity.Descriptor, frameworkIdentity))
              vssRequestContext.TraceError(10015033, "ResourceService", "Service identity with ID {0} has been successfully added to security service group {1}", (object) frameworkIdentity.Id, (object) identity.Id);
            else
              vssRequestContext.TraceError(10015034, "ResourceService", "Service identity with ID {0} is marked inactive but was not added to security service group {1}", (object) frameworkIdentity.Id, (object) identity.Id);
          }
        }
      }
      else
      {
        IVssRequestContext requestContext2 = vssRequestContext;
        object[] objArray1 = new object[1];
        serviceAccountId = poolData.ServiceAccountId;
        objArray1[0] = (object) serviceAccountId.ToString("D");
        requestContext2.TraceInfo(0, "ResourceService", "Service identity with ID {0} was not found. Provisioning a new service identity.", objArray1);
        IdentityService dentityService = service;
        IVssRequestContext requestContext3 = vssRequestContext;
        string role2 = str2;
        serviceAccountId = poolData.ServiceAccountId;
        string identifier2 = serviceAccountId.ToString("D");
        string displayName = str1;
        frameworkIdentity = dentityService.CreateFrameworkIdentity(requestContext3, FrameworkIdentityType.ServiceIdentity, role2, identifier2, displayName);
        if (frameworkIdentity != null)
        {
          vssRequestContext.TraceInfo(0, "ResourceService", "Successfully provisioned service identity {0} with VSID {1}", (object) frameworkIdentity.DisplayName, (object) frameworkIdentity.Id);
        }
        else
        {
          IVssRequestContext requestContext4 = vssRequestContext;
          object[] objArray2 = new object[3]
          {
            (object) str2,
            null,
            null
          };
          serviceAccountId = poolData.ServiceAccountId;
          objArray2[1] = (object) serviceAccountId.ToString("D");
          objArray2[2] = (object) str1;
          requestContext4.TraceError(10015117, "ResourceService", "Failed to provision service identity for  role {0}, id {1}, name {2}", objArray2);
        }
      }
      if (frameworkIdentity != null)
      {
        IAgentPoolSecurityProvider agentPoolSecurity = this.GetAgentPoolSecurity(poolRequestContext, poolData.Pool.PoolType);
        switch (role)
        {
          case AgentPoolServiceAccountRoles.AgentPoolService:
            agentPoolSecurity.GrantListenPermissionToPool(poolRequestContext, poolData.Pool.Id, frameworkIdentity);
            poolRequestContext.GetService<IInternalAgentCloudService>().SetServiceIdentityPermissions(poolRequestContext, frameworkIdentity);
            break;
          case AgentPoolServiceAccountRoles.AgentPoolAdmin:
            agentPoolSecurity.GrantAdministratorPermissionToPool(poolRequestContext, poolData.Pool.Id, frameworkIdentity);
            break;
        }
      }
      return frameworkIdentity;
    }

    private void EnsureMessageQueueStarted(IVssRequestContext requestContext)
    {
      if (this.m_messageQueueStarted)
        return;
      requestContext.GetService<ITeamFoundationMessageQueueService>();
      this.m_messageQueueStarted = true;
    }

    private void OnAgentRequestSettingsChanged(
      IVssRequestContext requestContext,
      RegistryEntryCollection changedEntries)
    {
      this.LoadRegistrySettings(requestContext);
    }

    private void ShutdownDispatcher(IVssRequestContext requestContext, object taskArgs) => ((IVssTaskDispatcher) taskArgs).Stop(TimeSpan.FromSeconds(100.0));

    private TaskAgentPoolMaintenanceJob PopulateMaintenanceJobData(
      IVssRequestContext requestContext,
      TaskAgentPoolMaintenanceJob maintenanceJob)
    {
      if (maintenanceJob == null)
        return (TaskAgentPoolMaintenanceJob) null;
      requestContext = requestContext.ToPoolRequestContext();
      if (maintenanceJob.OrchestrationId != new Guid())
      {
        TaskHub maintenanceTaskHub = this.GetPoolMaintenanceTaskHub(requestContext);
        TaskOrchestrationPlan plan = maintenanceTaskHub.GetPlan(requestContext, Guid.Empty, maintenanceJob.OrchestrationId);
        if (plan != null)
        {
          foreach (Timeline timeline1 in (IEnumerable<Timeline>) ((object) maintenanceTaskHub.GetTimelines(requestContext, plan.ScopeIdentifier, plan.PlanId) ?? (object) Array.Empty<Timeline>()))
          {
            Timeline timeline2 = maintenanceTaskHub.GetTimeline(requestContext, plan.ScopeIdentifier, plan.PlanId, timeline1.Id, includeRecords: true);
            if (timeline2 != null)
            {
              foreach (TimelineRecord record in timeline2.Records)
              {
                maintenanceJob.ErrorCount += record.ErrorCount.GetValueOrDefault();
                maintenanceJob.WarningCount += record.WarningCount.GetValueOrDefault();
              }
            }
          }
        }
      }
      if (maintenanceJob.Status != TaskAgentPoolMaintenanceJobStatus.Completed || maintenanceJob.StartTime.HasValue)
      {
        ILocationService service = requestContext.GetService<ILocationService>();
        maintenanceJob.LogsDownloadUrl = service.GetResourceUri(requestContext, "distributedtask", TaskResourceIds.PoolMaintenanceJobs, (object) new
        {
          poolId = maintenanceJob.Pool.Id,
          jobId = maintenanceJob.JobId
        }).ToString();
      }
      return maintenanceJob;
    }

    private void AddScheduleMaintenanceJob(
      IVssRequestContext requestContext,
      TaskAgentPoolMaintenanceDefinition maintenanceDefinition)
    {
      ITeamFoundationJobService service = requestContext.Elevate().GetService<ITeamFoundationJobService>();
      TeamFoundationJobDefinition maintenanceJobDefinition = this.GetPoolMaintenanceJobDefinition(requestContext, maintenanceDefinition, service.IsIgnoreDormancyPermitted);
      if (maintenanceJobDefinition == null)
        return;
      service.UpdateJobDefinitions(requestContext.Elevate(), (IEnumerable<Guid>) null, (IEnumerable<TeamFoundationJobDefinition>) new TeamFoundationJobDefinition[1]
      {
        maintenanceJobDefinition
      });
    }

    private void DeleteScheduleMaintenanceJob(
      IVssRequestContext requestContext,
      IList<TaskAgentPoolMaintenanceDefinition> maintenanceDefinitions)
    {
      if (maintenanceDefinitions == null)
        return;
      requestContext.Elevate().GetService<ITeamFoundationJobService>().UpdateJobDefinitions(requestContext.Elevate(), maintenanceDefinitions.Select<TaskAgentPoolMaintenanceDefinition, Guid>((Func<TaskAgentPoolMaintenanceDefinition, Guid>) (x => x.ScheduleSetting.ScheduleJobId)), (IEnumerable<TeamFoundationJobDefinition>) null);
    }

    private void UpdateScheduleMaintenanceJob(
      IVssRequestContext requestContext,
      TaskAgentPoolMaintenanceDefinition maintenanceDefinition)
    {
      ITeamFoundationJobService service = requestContext.Elevate().GetService<ITeamFoundationJobService>();
      if (!maintenanceDefinition.Enabled || maintenanceDefinition.ScheduleSetting.DaysToBuild == TaskAgentPoolMaintenanceScheduleDays.None)
      {
        service.UpdateJobDefinitions(requestContext.Elevate(), (IEnumerable<Guid>) new Guid[1]
        {
          maintenanceDefinition.ScheduleSetting.ScheduleJobId
        }, (IEnumerable<TeamFoundationJobDefinition>) null);
      }
      else
      {
        TeamFoundationJobDefinition maintenanceJobDefinition = this.GetPoolMaintenanceJobDefinition(requestContext, maintenanceDefinition, service.IsIgnoreDormancyPermitted);
        service.UpdateJobDefinitions(requestContext.Elevate(), (IEnumerable<Guid>) null, (IEnumerable<TeamFoundationJobDefinition>) new TeamFoundationJobDefinition[1]
        {
          maintenanceJobDefinition
        });
      }
    }

    private TeamFoundationJobDefinition GetPoolMaintenanceJobDefinition(
      IVssRequestContext requestContext,
      TaskAgentPoolMaintenanceDefinition maintenanceDefinition,
      bool isIgnoreDormancyPermitted = true)
    {
      if (!maintenanceDefinition.Enabled || maintenanceDefinition.ScheduleSetting.DaysToBuild == TaskAgentPoolMaintenanceScheduleDays.None)
        return (TeamFoundationJobDefinition) null;
      XmlDocument xmlDocument = new XmlDocument();
      XmlNode element1 = (XmlNode) xmlDocument.CreateElement("PoolMaintenance");
      XmlNode element2 = (XmlNode) xmlDocument.CreateElement("PoolId");
      element2.AppendChild((XmlNode) xmlDocument.CreateTextNode(maintenanceDefinition.Pool.Id.ToString()));
      XmlNode element3 = (XmlNode) xmlDocument.CreateElement("DefinitionId");
      element3.AppendChild((XmlNode) xmlDocument.CreateTextNode(maintenanceDefinition.Id.ToString()));
      XmlNode element4 = (XmlNode) xmlDocument.CreateElement("JobId");
      element4.AppendChild((XmlNode) xmlDocument.CreateTextNode(maintenanceDefinition.ScheduleSetting.ScheduleJobId.ToString("D")));
      element1.AppendChild(element2);
      element1.AppendChild(element3);
      element1.AppendChild(element4);
      TeamFoundationJobDefinition maintenanceJobDefinition = new TeamFoundationJobDefinition()
      {
        Data = element1,
        EnabledState = TeamFoundationJobEnabledState.Enabled,
        ExtensionName = "Microsoft.TeamFoundation.DistributedTask.Server.Extensions.PoolMaintenanceScheduleJobExtension",
        JobId = maintenanceDefinition.ScheduleSetting.ScheduleJobId,
        Name = "Pool maintenance schedule job",
        IgnoreDormancy = isIgnoreDormancyPermitted,
        PriorityClass = JobPriorityClass.High
      };
      DateTime dateTime1 = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.UtcNow, maintenanceDefinition.ScheduleSetting.TimeZoneId);
      DateTime dateTime2 = new DateTime(dateTime1.Year, dateTime1.Month, dateTime1.Day, maintenanceDefinition.ScheduleSetting.StartHours, maintenanceDefinition.ScheduleSetting.StartMinutes, 0, 0);
      DateTime utc = TimeZoneInfo.ConvertTimeToUtc(dateTime2, TimeZoneInfo.FindSystemTimeZoneById(maintenanceDefinition.ScheduleSetting.TimeZoneId));
      int num1 = 604800;
      DateTime dateTime3 = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day, 0, 0, 0, DateTimeKind.Utc);
      while (dateTime3.DayOfWeek != DayOfWeek.Sunday)
        dateTime3 = dateTime3.AddDays(-1.0);
      DateTime dateTime4 = dateTime3.Add(new TimeSpan(utc.Hour, utc.Minute, 0));
      int num2 = 0;
      if (utc.Day != dateTime2.Day)
        num2 = utc.Subtract(dateTime2).Ticks > 0L ? 1 : -1;
      if ((maintenanceDefinition.ScheduleSetting.DaysToBuild & TaskAgentPoolMaintenanceScheduleDays.Sunday) != TaskAgentPoolMaintenanceScheduleDays.None)
        maintenanceJobDefinition.Schedule.Add(new TeamFoundationJobSchedule()
        {
          Interval = num1,
          ScheduledTime = dateTime4.AddDays((double) num2),
          TimeZoneId = maintenanceDefinition.ScheduleSetting.TimeZoneId,
          PriorityLevel = JobPriorityLevel.Highest
        });
      if ((maintenanceDefinition.ScheduleSetting.DaysToBuild & TaskAgentPoolMaintenanceScheduleDays.Monday) != TaskAgentPoolMaintenanceScheduleDays.None)
        maintenanceJobDefinition.Schedule.Add(new TeamFoundationJobSchedule()
        {
          Interval = num1,
          ScheduledTime = dateTime4.AddDays((double) (1 + num2)),
          TimeZoneId = maintenanceDefinition.ScheduleSetting.TimeZoneId,
          PriorityLevel = JobPriorityLevel.Highest
        });
      if ((maintenanceDefinition.ScheduleSetting.DaysToBuild & TaskAgentPoolMaintenanceScheduleDays.Tuesday) != TaskAgentPoolMaintenanceScheduleDays.None)
        maintenanceJobDefinition.Schedule.Add(new TeamFoundationJobSchedule()
        {
          Interval = num1,
          ScheduledTime = dateTime4.AddDays((double) (2 + num2)),
          TimeZoneId = maintenanceDefinition.ScheduleSetting.TimeZoneId,
          PriorityLevel = JobPriorityLevel.Highest
        });
      if ((maintenanceDefinition.ScheduleSetting.DaysToBuild & TaskAgentPoolMaintenanceScheduleDays.Wednesday) != TaskAgentPoolMaintenanceScheduleDays.None)
        maintenanceJobDefinition.Schedule.Add(new TeamFoundationJobSchedule()
        {
          Interval = num1,
          ScheduledTime = dateTime4.AddDays((double) (3 + num2)),
          TimeZoneId = maintenanceDefinition.ScheduleSetting.TimeZoneId,
          PriorityLevel = JobPriorityLevel.Highest
        });
      if ((maintenanceDefinition.ScheduleSetting.DaysToBuild & TaskAgentPoolMaintenanceScheduleDays.Thursday) != TaskAgentPoolMaintenanceScheduleDays.None)
        maintenanceJobDefinition.Schedule.Add(new TeamFoundationJobSchedule()
        {
          Interval = num1,
          ScheduledTime = dateTime4.AddDays((double) (4 + num2)),
          TimeZoneId = maintenanceDefinition.ScheduleSetting.TimeZoneId,
          PriorityLevel = JobPriorityLevel.Highest
        });
      if ((maintenanceDefinition.ScheduleSetting.DaysToBuild & TaskAgentPoolMaintenanceScheduleDays.Friday) != TaskAgentPoolMaintenanceScheduleDays.None)
        maintenanceJobDefinition.Schedule.Add(new TeamFoundationJobSchedule()
        {
          Interval = num1,
          ScheduledTime = dateTime4.AddDays((double) (5 + num2)),
          TimeZoneId = maintenanceDefinition.ScheduleSetting.TimeZoneId,
          PriorityLevel = JobPriorityLevel.Highest
        });
      if ((maintenanceDefinition.ScheduleSetting.DaysToBuild & TaskAgentPoolMaintenanceScheduleDays.Saturday) != TaskAgentPoolMaintenanceScheduleDays.None)
        maintenanceJobDefinition.Schedule.Add(new TeamFoundationJobSchedule()
        {
          Interval = num1,
          ScheduledTime = dateTime4.AddDays((double) (6 + num2)),
          TimeZoneId = maintenanceDefinition.ScheduleSetting.TimeZoneId,
          PriorityLevel = JobPriorityLevel.Highest
        });
      return maintenanceJobDefinition;
    }

    private TaskHub GetPoolMaintenanceTaskHub(IVssRequestContext requestContext)
    {
      IDistributedTaskHubService service = requestContext.GetService<IDistributedTaskHubService>();
      TaskHub taskHub = service.GetTaskHub(requestContext, TaskAgentConstants.PoolMaintenanceHubName, false);
      if (taskHub == null)
      {
        try
        {
          taskHub = service.CreateTaskHub(requestContext, TaskAgentConstants.PoolMaintenanceHubName, "DistributedTask");
        }
        catch (TaskHubExistsException ex)
        {
          requestContext.TraceWarning("ResourceService", "Could not create Task Hub with name " + TaskAgentConstants.PoolMaintenanceHubName + " as it already exists!");
          taskHub = service.GetTaskHub(requestContext, TaskAgentConstants.PoolMaintenanceHubName, false);
        }
      }
      taskHub.CreateScope(requestContext, Guid.Empty);
      return taskHub;
    }

    private static string GetAgentNoUpdateErrorMessage(
      string poolName,
      DemandMinimumVersion versionDemand)
    {
      if (versionDemand.Source?.SourceName == null)
        return TaskResources.AgentNotFoundAutoUpdateDisabled((object) poolName, (object) versionDemand.Value);
      return versionDemand.Source.SourceType == DemandSourceType.Task ? TaskResources.AgentNotFoundAutoUpdateDisabledTask((object) poolName, (object) versionDemand.Source.SourceName, (object) versionDemand.Source.SourceVersion, (object) versionDemand.Value) : TaskResources.AgentNotFoundAutoUpdateDisabledFeature((object) poolName, (object) versionDemand.Source.SourceName, (object) versionDemand.Value);
    }

    private string GetTaskAgentPoolWebUrl(
      IVssRequestContext requestContext,
      int poolId,
      string tab)
    {
      ILocationService service = requestContext.RootContext.GetService<ILocationService>();
      AccessMapping accessMapping = service.DetermineAccessMapping(requestContext.RootContext);
      return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}_settings/agentpools?view={2}&poolId={1}", (object) service.GetSelfReferenceUrl(requestContext.RootContext, accessMapping), (object) poolId, (object) tab);
    }

    private void EnsureHostedMacPoolsAdded(IVssRequestContext requestContext)
    {
      string[] strArray = new string[2];
      IVssRequestContext context = requestContext.To(TeamFoundationHostType.Application);
      Microsoft.VisualStudio.Services.Organization.Organization organization = context.GetService<IOrganizationService>().GetOrganization(context, (IEnumerable<string>) null);
      if (organization == null)
      {
        requestContext.TraceWarning("ResourceService", "Could not determine if Mac pool should be added. Organization is null.");
      }
      else
      {
        if (object.Equals((object) organization.TenantId, (object) Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.TaskConstants.MobileCenterIntTenantId))
        {
          strArray[0] = "Hosted Mac Mobile Center INT";
          strArray[1] = "Hosted Mac Mobile Center High Sierra INT";
        }
        else if (object.Equals((object) organization.TenantId, (object) Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.TaskConstants.MobileCenterStagingTenantId))
        {
          strArray[0] = "Hosted Mac Mobile Center Staging";
          strArray[1] = "Hosted Mac Mobile Center High Sierra Staging";
        }
        else if (object.Equals((object) organization.TenantId, (object) Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.TaskConstants.MobileCenterProdTenantId) || this.CheckEntryPointForMobileCenter(requestContext))
        {
          strArray[0] = "Hosted Mac Mobile Center Prod";
          strArray[1] = "Hosted Mac Mobile Center High Sierra Prod";
        }
        TaskAgentCloud agentCloud = requestContext.GetService<IAgentCloudService>().GetAgentClouds(requestContext).SingleOrDefault<TaskAgentCloud>((Func<TaskAgentCloud, bool>) (ac => string.Equals(ac.Name, "Azure Pipelines", StringComparison.OrdinalIgnoreCase)));
        if (agentCloud == null)
          requestContext.TraceError("ResourceService", "Unable to find internal agent cloud for MobileCenter pools.");
        foreach (string poolName in strArray)
        {
          if (!string.IsNullOrEmpty(poolName))
            this.CreateHostedAgentPool(requestContext, poolName, true, 1, agentCloud);
        }
      }
    }

    private void CreateHostedAgentPool(
      IVssRequestContext requestContext,
      string poolName,
      bool autoProvision,
      int agentCount,
      TaskAgentCloud agentCloud)
    {
      TaskAgentPool pool = this.GetAgentPools(requestContext, poolName, (IList<string>) null, TaskAgentPoolType.Automation, TaskAgentPoolActionFilter.None).FirstOrDefault<TaskAgentPool>();
      if (pool == null)
      {
        Microsoft.VisualStudio.Services.Identity.Identity identity = requestContext.GetService<IdentityService>().GetIdentity(requestContext, TaskWellKnownScopeIds.PoolsRootScopeId, GroupWellKnownIdentityDescriptors.NamespaceAdministratorsGroup);
        TaskAgentPool taskAgentPool = new TaskAgentPool(poolName);
        taskAgentPool.AutoProvision = new bool?(autoProvision);
        taskAgentPool.IsHosted = true;
        taskAgentPool.IsLegacy = new bool?(true);
        pool = taskAgentPool;
        if (identity != null)
          pool.CreatedBy = new IdentityRef()
          {
            Id = identity.Id.ToString("D")
          };
        if (agentCloud != null)
          pool.AgentCloudId = new int?(agentCloud.AgentCloudId);
        pool.Properties.Add("PoolType", (object) "TaskAgent");
        pool.Properties.Add("PoolName", (object) "Mac");
        pool.Properties.Add("OrchestrationPoolType", (object) "TaskAgent");
        pool.Properties.Add("OrchestrationPoolName", (object) "Mac");
        try
        {
          pool = this.AddAgentPool(requestContext, pool, (Stream) null);
        }
        catch (TaskAgentPoolExistsException ex)
        {
        }
      }
      IList<TaskAgent> agents = this.GetAgents(requestContext, pool.Id, (string) null, false, false, false, false, (IList<string>) null);
      PackageVersion agentPackageVersion = requestContext.GetRecommendedAgentPackageVersion();
      for (int count = agents.Count; count < agentCount; ++count)
      {
        TaskAgent taskAgent = new TaskAgent(agentCount > 1 ? string.Format("Hosted Agent {0}", (object) (count + 1)) : "Hosted Agent");
        taskAgent.Enabled = new bool?(true);
        taskAgent.MaxParallelism = new int?(1);
        taskAgent.Version = agentPackageVersion.ToString();
        taskAgent.ProvisioningState = agentCloud != null ? "Deallocated" : "Provisioned";
        TaskAgent agent = taskAgent;
        try
        {
          this.AddAgent(requestContext, pool.Id, agent);
        }
        catch (TaskAgentExistsException ex)
        {
        }
      }
    }

    private DistributedTaskResourceService.AgentFilter ConvertAgentFiltersForSQL(
      TaskAgentStatusFilter agentStatusFilter,
      TaskAgentJobResultFilter agentJobResultFilter = TaskAgentJobResultFilter.All)
    {
      DistributedTaskResourceService.AgentFilter agentFilter = new DistributedTaskResourceService.AgentFilter();
      IList<byte> byteList = (IList<byte>) new List<byte>();
      agentFilter.isNeverDeployedFilter = false;
      switch (agentJobResultFilter)
      {
        case TaskAgentJobResultFilter.Failed:
          byteList.Add((byte) 5);
          byteList.Add((byte) 3);
          byteList.Add((byte) 2);
          byteList.Add((byte) 4);
          agentFilter.agentlastJobStatusFilters = byteList;
          break;
        case TaskAgentJobResultFilter.Passed:
          byteList.Add((byte) 0);
          byteList.Add((byte) 1);
          agentFilter.agentlastJobStatusFilters = byteList;
          break;
        case TaskAgentJobResultFilter.NeverDeployed:
          agentFilter.isNeverDeployedFilter = true;
          agentFilter.agentlastJobStatusFilters = (IList<byte>) null;
          break;
        case TaskAgentJobResultFilter.All:
          agentFilter.agentlastJobStatusFilters = (IList<byte>) null;
          break;
      }
      switch (agentStatusFilter)
      {
        case TaskAgentStatusFilter.Offline:
          agentFilter.agentStatusFilter = new int?(1);
          break;
        case TaskAgentStatusFilter.Online:
          agentFilter.agentStatusFilter = new int?(2);
          break;
        case TaskAgentStatusFilter.All:
          agentFilter.agentStatusFilter = new int?();
          break;
      }
      return agentFilter;
    }

    private IEnumerable<TaskAgentQueue> FilterQueues(
      IVssRequestContext requestContext,
      Guid projectId,
      IList<TaskAgentQueue> allQueues,
      TaskAgentQueueActionFilter actionFilter)
    {
      int requiredPermissions = 0;
      if (actionFilter != TaskAgentQueueActionFilter.None)
      {
        if ((actionFilter & TaskAgentQueueActionFilter.Manage) == TaskAgentQueueActionFilter.Manage)
          requiredPermissions |= 2;
        if ((actionFilter & TaskAgentQueueActionFilter.Use) == TaskAgentQueueActionFilter.Use)
          requiredPermissions |= 16;
      }
      if (!requestContext.IsSystemContext)
      {
        for (int index = allQueues.Count - 1; index >= 0; --index)
        {
          if (!this.Security.HasQueuePermission(requestContext, projectId, allQueues[index].Id, 1, true))
            allQueues.RemoveAt(index);
          else if (requiredPermissions != 0 && !this.Security.HasQueuePermission(requestContext, projectId, allQueues[index].Id, requiredPermissions))
            allQueues.RemoveAt(index);
        }
      }
      return allQueues.PopulateReferences(requestContext).Where<TaskAgentQueue>((Func<TaskAgentQueue, bool>) (x => requestContext.IsSystemContext || !x.Pool.ShouldHidePool(requestContext)));
    }

    private bool CheckEntryPointForMobileCenter(IVssRequestContext requestContext)
    {
      bool flag = false;
      string b;
      if (requestContext.GetService<ICollectionService>().GetCollection(requestContext, (IEnumerable<string>) new string[1]
      {
        "Microsoft.VisualStudio.Services.Account.SignupEntryPoint"
      }).Properties.TryGetValue<string>("Microsoft.VisualStudio.Services.Account.SignupEntryPoint", out b))
        flag = string.Equals("MobileCenter", b);
      return flag;
    }

    private void PopulateAgentAccessMapping(
      IVssRequestContext requestContext,
      TaskAgentReference agent)
    {
      ILocationService service = requestContext.GetService<ILocationService>();
      if (requestContext.ExecutionEnvironment.IsHostedDeployment && !string.IsNullOrEmpty(requestContext.UserAgent) && requestContext.UserAgent.IndexOf("vstsagentcore-", StringComparison.OrdinalIgnoreCase) >= 0)
      {
        AccessMapping accessMapping1 = service.GetAccessMapping(requestContext, AccessMappingConstants.VstsAccessMapping);
        if (accessMapping1 == null)
        {
          requestContext.TraceError(10015161, "ResourceService", "Fail to retrive access mapping for moniker " + AccessMappingConstants.VstsAccessMapping + ".");
        }
        else
        {
          AccessMapping accessMapping2 = service.DetermineAccessMapping(requestContext);
          if (accessMapping2 == null)
            requestContext.TraceError(10015161, "ResourceService", "Fail to retrive client access mapping.");
          else if (string.Equals(accessMapping2.Moniker, AccessMappingConstants.VstsAccessMapping, StringComparison.OrdinalIgnoreCase) || string.Equals(accessMapping2.Moniker, AccessMappingConstants.DevOpsAccessMapping, StringComparison.OrdinalIgnoreCase))
            agent.AccessPoint = accessMapping2.Moniker;
          else if (string.Equals(accessMapping2.Moniker, accessMapping1.AccessPoint, StringComparison.OrdinalIgnoreCase))
          {
            agent.AccessPoint = AccessMappingConstants.VstsAccessMapping;
          }
          else
          {
            agent.AccessPoint = (string) null;
            requestContext.TraceError(10015152, "ResourceService", "Expect agent access mapping moniker to be either '" + AccessMappingConstants.VstsAccessMapping + "' or '" + AccessMappingConstants.DevOpsAccessMapping + "' or '" + accessMapping1.AccessPoint + "', current moniker is '" + (accessMapping2.Moniker ?? string.Empty) + "'.");
          }
        }
      }
      else
        agent.AccessPoint = (string) null;
    }

    private void LoadRegistrySettings(IVssRequestContext requestContext)
    {
      RegistryEntryCollection registryEntryCollection = requestContext.GetService<IVssRegistryService>().ReadEntriesFallThru(requestContext, (RegistryQuery) "/Service/DistributedTask/Settings/AgentRequest/**");
      this.m_agentRequestSettings.AssignmentBatchSize = registryEntryCollection.GetValueFromPath<int>("/Service/DistributedTask/Settings/AgentRequest/AssignmentBatchSize", 20);
      this.m_agentRequestSettings.HostedLeaseTimeout = registryEntryCollection.GetValueFromPath<TimeSpan>("/Service/DistributedTask/Settings/AgentRequest/HostedLeaseTimeout", TimeSpan.FromMinutes(45.0));
      this.m_agentRequestSettings.DefaultLeaseTimeout = registryEntryCollection.GetValueFromPath<TimeSpan>("/Service/DistributedTask/Settings/AgentRequest/DefaultLeaseTimeout", TimeSpan.FromMinutes(20160.0));
      this.m_agentRequestSettings.AssignmentNotificationTimeout = registryEntryCollection.GetValueFromPath<TimeSpan>("/Service/DistributedTask/Settings/AgentRequest/AssignmentNotificationTimeout", TimeSpan.FromSeconds(30.0));
      this.m_agentRequestSettings.UnassignedRequestTimeout = registryEntryCollection.GetValueFromPath<TimeSpan>("/Service/DistributedTask/Settings/AgentRequest/UnassignedRequestTimeout", TimeSpan.FromDays(7.0));
      this.m_agentRequestSettings.UnassignedRequestTimeoutBatchSize = registryEntryCollection.GetValueFromPath<int>("/Service/DistributedTask/Settings/AgentRequest/UnassignedRequestBatchSize", 1000);
      this.m_agentRequestSettings.CompletedRequestTimeout = registryEntryCollection.GetValueFromPath<TimeSpan>("/Service/DistributedTask/Settings/AgentRequest/CompletedRequestTimeout", TimeSpan.FromDays(1.0));
    }

    void IVssFrameworkService.ServiceStart(IVssRequestContext requestContext)
    {
      requestContext.GetService<IVssRegistryService>().RegisterNotification(requestContext, new RegistrySettingsChangedCallback(this.OnAgentRequestSettingsChanged), true, (RegistryQuery) "/Service/DistributedTask/Settings/AgentRequest/**");
      this.LoadRegistrySettings(requestContext);
    }

    void IVssFrameworkService.ServiceEnd(IVssRequestContext requestContext) => requestContext.GetService<IVssRegistryService>().UnregisterNotification(requestContext, new RegistrySettingsChangedCallback(this.OnAgentRequestSettingsChanged));

    public void RemovePoisonedOrchestrations(
      IVssRequestContext requestContext,
      string hubName,
      IList<string> orchestrationIds,
      TimeSpan? timeout = null)
    {
      requestContext.GetService<IOrchestrationService>().RemovePoisonedOrchestrations(requestContext, hubName, orchestrationIds, timeout);
    }

    private struct AgentRequestSettings
    {
      public int AssignmentBatchSize { get; set; }

      public TimeSpan HostedLeaseTimeout { get; set; }

      public TimeSpan DefaultLeaseTimeout { get; set; }

      public TimeSpan AssignmentNotificationTimeout { get; set; }

      public TimeSpan UnassignedRequestTimeout { get; set; }

      public int UnassignedRequestTimeoutBatchSize { get; set; }

      public TimeSpan CompletedRequestTimeout { get; set; }
    }

    private struct AgentFilter
    {
      public int? agentStatusFilter { get; set; }

      public IList<byte> agentlastJobStatusFilters { get; set; }

      public bool isNeverDeployedFilter { get; set; }
    }
  }
}
