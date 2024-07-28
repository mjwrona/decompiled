// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.DeploymentGroupService
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.DistributedTask.Server.DataAccess;
using Microsoft.TeamFoundation.DistributedTask.Server.Hubs;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.DelegatedAuthorization;
using Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server.FeatureManagement;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.VisualStudio.Services.WebPlatform;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.DistributedTask.Server
{
  internal class DeploymentGroupService : IDeploymentGroupService, IVssFrameworkService
  {
    private readonly ISecurityProvider m_security;
    private const int DefaultTop = 50;

    internal DeploymentGroupService()
      : this((ISecurityProvider) new DefaultSecurityProvider())
    {
    }

    public DeploymentGroupService(ISecurityProvider security) => this.m_security = security;

    public ISecurityProvider Security => this.m_security;

    public DeploymentGroup AddDeploymentGroup(
      IVssRequestContext requestContext,
      Guid projectId,
      DeploymentGroup deploymentGroup)
    {
      using (new MethodScope(requestContext, "ResourceService", nameof (AddDeploymentGroup)))
      {
        if (requestContext.GetService<IContributedFeatureService>().IsFeatureEnabled(requestContext, "ms.vss-build-web.disable-classic-build-pipeline-creation"))
          throw new InvalidOperationException(TaskResources.ClassicPipelinesDisabled());
        ArgumentValidation.CheckDeploymentGroup(deploymentGroup, nameof (deploymentGroup));
        this.CheckDeploymentGroupCreatePermissions(requestContext, projectId);
        if (this.GetDeploymentGroups(requestContext, projectId, deploymentGroup.Name, DeploymentGroupActionFilter.None, false).FirstOrDefault<DeploymentGroup>() != null)
          throw new DeploymentMachineGroupExistsException(TaskResources.DeploymentMachineGroupExists((object) deploymentGroup.Name));
        Microsoft.VisualStudio.Services.Identity.Identity userIdentity = requestContext.GetUserIdentity();
        IDistributedTaskResourceService service = requestContext.GetService<IDistributedTaskResourceService>();
        TaskAgentPool pool1;
        if (deploymentGroup.Pool != null)
        {
          pool1 = service.GetAgentPool(requestContext, deploymentGroup.Pool.Id);
          if (pool1 == null)
            throw new TaskAgentPoolNotFoundException(TaskResources.AgentPoolNotFound((object) deploymentGroup.Pool.Id));
          if (pool1.PoolType != TaskAgentPoolType.Deployment)
            throw new TaskAgentPoolTypeMismatchException(TaskResources.DeploymentGroupsShouldUseDeploymentPools());
          this.CheckViewAndOtherPermissionsForDeploymentPool(requestContext, deploymentGroup.Pool.Id, 16);
        }
        else
        {
          string poolName = this.GetAutoProvisionedPoolName(requestContext, projectId, deploymentGroup);
          if (service.GetAgentPools(requestContext.Elevate(), poolName, poolType: TaskAgentPoolType.Deployment).FirstOrDefault<TaskAgentPool>() != null)
            poolName = deploymentGroup.Name + Guid.NewGuid().ToString();
          IDistributedTaskResourceService taskResourceService = service;
          IVssRequestContext requestContext1 = requestContext.Elevate();
          TaskAgentPool pool2 = new TaskAgentPool();
          pool2.Name = poolName;
          pool2.PoolType = TaskAgentPoolType.Deployment;
          pool2.AutoProvision = new bool?(false);
          pool2.CreatedBy = userIdentity.ToIdentityRef(requestContext);
          pool1 = taskResourceService.AddAgentPool(requestContext1, pool2);
          deploymentGroup.Pool = pool1.AsReference();
        }
        using (TaskResourceComponent component = requestContext.CreateComponent<TaskResourceComponent>())
          deploymentGroup = component.AddAgentQueue(projectId, deploymentGroup.Name, deploymentGroup.Pool.Id, TaskAgentQueueType.Deployment, deploymentGroup.Description).MachineGroup;
        deploymentGroup.PopulateProjectName(requestContext, projectId);
        deploymentGroup.Pool = pool1.AsReference();
        if (deploymentGroup.Pool != null)
        {
          List<AccessControlEntry> accessControlEntryList = new List<AccessControlEntry>();
          if (userIdentity != null)
            accessControlEntryList.Add(new AccessControlEntry()
            {
              Descriptor = userIdentity.Descriptor,
              Allow = 27
            });
          if (accessControlEntryList.Count > 0)
            requestContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(requestContext, DefaultSecurityProvider.NamespaceId).SetAccessControlEntries(requestContext.Elevate(), DefaultSecurityProvider.GetDeploymentGroupToken(requestContext, projectId, deploymentGroup.Id), (IEnumerable<IAccessControlEntry>) accessControlEntryList, true);
          Microsoft.VisualStudio.Services.Identity.Identity identity = requestContext.GetService<IdentityService>().GetGroups(requestContext, projectId, (IList<string>) new string[1]
          {
            TaskResources.ProjectContributorsGroupName()
          }).FirstOrDefault<Microsoft.VisualStudio.Services.Identity.Identity>();
          if (identity != null)
            this.GetDeploymentPoolSecurity(requestContext).GrantReadPermissionToPool(requestContext.ToPoolRequestContext(), deploymentGroup.Pool.Id, identity);
          else
            requestContext.TraceError(10015055, "ResourceService", "Couldn't find contributors group on the project: {0}", (object) projectId, (object) deploymentGroup.Id, (object) deploymentGroup.Pool.Id);
        }
        return deploymentGroup;
      }
    }

    public void DeleteDeploymentGroup(
      IVssRequestContext requestContext,
      Guid projectId,
      int deploymentGroupId)
    {
      using (new MethodScope(requestContext, "ResourceService", nameof (DeleteDeploymentGroup)))
      {
        this.Security.CheckDeploymentGroupPermission(requestContext, projectId, deploymentGroupId, 2);
        DeploymentGroup machineGroup;
        using (TaskResourceComponent component = requestContext.CreateComponent<TaskResourceComponent>())
          machineGroup = component.DeleteAgentQueue(projectId, deploymentGroupId, TaskAgentQueueType.Deployment).MachineGroup;
        if (machineGroup == null)
          return;
        this.CleanupDeploymentGroupRolesAndSecurity(requestContext, projectId, machineGroup);
      }
    }

    public DeploymentGroup GetDeploymentGroup(
      IVssRequestContext requestContext,
      Guid projectId,
      int deploymentGroupId,
      DeploymentGroupActionFilter actionFilter = DeploymentGroupActionFilter.None,
      bool includeMachines = true,
      bool includeTags = false)
    {
      using (new MethodScope(requestContext, "ResourceService", nameof (GetDeploymentGroup)))
      {
        if (!this.Security.HasDeploymentGroupPermission(requestContext, projectId, deploymentGroupId, 1))
          return (DeploymentGroup) null;
        if (actionFilter != DeploymentGroupActionFilter.None)
        {
          int permissions = DeploymentGroupService.ConvertActionFilterToPermissions(actionFilter);
          this.Security.CheckDeploymentGroupPermission(requestContext, projectId, deploymentGroupId, permissions);
        }
        DeploymentGroup deploymentGroupInternal = DeploymentGroupService.GetDeploymentGroupInternal(requestContext, projectId, deploymentGroupId, includeMachines, includeTags);
        if (deploymentGroupInternal == null)
          return (DeploymentGroup) null;
        deploymentGroupInternal.PopulateProjectName(requestContext, projectId);
        deploymentGroupInternal.PopulatePoolReference(requestContext);
        if (includeMachines)
          deploymentGroupInternal.PopulateAgentReferences(requestContext);
        return deploymentGroupInternal;
      }
    }

    public IList<DeploymentGroup> GetDeploymentGroups(
      IVssRequestContext requestContext,
      Guid projectId,
      string deploymentGroupName = null,
      DeploymentGroupActionFilter actionFilter = DeploymentGroupActionFilter.None,
      bool includeMachines = false)
    {
      using (new MethodScope(requestContext, "ResourceService", nameof (GetDeploymentGroups)))
      {
        IList<DeploymentGroup> deploymentGroupsInternal = this.GetDeploymentGroupsInternal(requestContext, projectId, deploymentGroupName, includeMachines);
        List<DeploymentGroup> deploymentGroups = this.FilterDeploymentGroupByActionFilter(requestContext, projectId, deploymentGroupsInternal, actionFilter);
        return (IList<DeploymentGroup>) this.FillMachineDetailsForDeploymentGroups(requestContext, projectId, deploymentGroups, includeMachines);
      }
    }

    public IPagedList<DeploymentGroup> GetDeploymentGroupsPaged(
      IVssRequestContext requestContext,
      Guid projectId,
      string deploymentGroupName = null,
      DeploymentGroupActionFilter actionFilter = DeploymentGroupActionFilter.None,
      bool includeMachines = false,
      string lastDeploymentGroupName = null,
      int maxDeploymentGroupCount = 50)
    {
      string continuationToken = (string) null;
      using (new MethodScope(requestContext, "ResourceService", nameof (GetDeploymentGroupsPaged)))
      {
        IList<DeploymentGroup> deploymentGroupList = this.GetDeploymentGroupsInternal(requestContext, projectId, deploymentGroupName, includeMachines, lastDeploymentGroupName, maxDeploymentGroupCount + 1);
        if (deploymentGroupList.Count > maxDeploymentGroupCount)
        {
          continuationToken = deploymentGroupList[maxDeploymentGroupCount].Name;
          deploymentGroupList = (IList<DeploymentGroup>) deploymentGroupList.Take<DeploymentGroup>(maxDeploymentGroupCount).ToList<DeploymentGroup>();
        }
        List<DeploymentGroup> deploymentGroups = this.FilterDeploymentGroupByActionFilter(requestContext, projectId, deploymentGroupList, actionFilter);
        return (IPagedList<DeploymentGroup>) new PagedList<DeploymentGroup>((IEnumerable<DeploymentGroup>) this.FillMachineDetailsForDeploymentGroups(requestContext, projectId, deploymentGroups, includeMachines), continuationToken);
      }
    }

    public IList<DeploymentGroup> GetDeploymentGroupsByIds(
      IVssRequestContext requestContext,
      Guid projectId,
      IList<int> deploymentGroupIds,
      DeploymentGroupActionFilter actionFilter = DeploymentGroupActionFilter.None,
      bool includeMachines = false)
    {
      using (new MethodScope(requestContext, "ResourceService", nameof (GetDeploymentGroupsByIds)))
      {
        List<DeploymentGroup> groupsByIdInternal = DeploymentGroupService.GetDeploymentGroupsByIdInternal(requestContext, projectId, deploymentGroupIds, includeMachines);
        List<DeploymentGroup> deploymentGroups = this.FilterDeploymentGroupByActionFilter(requestContext, projectId, (IList<DeploymentGroup>) groupsByIdInternal, actionFilter);
        return (IList<DeploymentGroup>) this.FillMachineDetailsForDeploymentGroups(requestContext, projectId, deploymentGroups, includeMachines);
      }
    }

    public void MigrateDeploymentGroups(
      IVssRequestContext requestContext,
      string sourcePoolName,
      string destinationPoolName)
    {
      using (new MethodScope(requestContext, "ResourceService", nameof (MigrateDeploymentGroups)))
      {
        if (!requestContext.ExecutionEnvironment.IsOnPremisesDeployment)
          throw new NotSupportedException(TaskResources.MigrateDeploymentGroupsNotSupported());
        Dictionary<string, TaskAgentPool> dictionary = requestContext.GetService<IDistributedTaskResourceService>().GetAgentPools(requestContext, poolType: TaskAgentPoolType.Deployment).ToDictionary<TaskAgentPool, string>((Func<TaskAgentPool, string>) (x => x.Name.ToLower()));
        TaskAgentPool taskAgentPool1;
        if (!dictionary.TryGetValue(sourcePoolName, out taskAgentPool1))
          throw new TaskAgentPoolNotFoundException(TaskResources.DeploymentPoolDoesNotExist((object) sourcePoolName));
        TaskAgentPool taskAgentPool2;
        if (!dictionary.TryGetValue(destinationPoolName, out taskAgentPool2))
          throw new TaskAgentPoolNotFoundException(TaskResources.DeploymentPoolDoesNotExist((object) destinationPoolName));
        IAgentPoolSecurityProvider deploymentPoolSecurity = this.GetDeploymentPoolSecurity(requestContext);
        if (deploymentPoolSecurity.HasPoolPermission(requestContext, taskAgentPool1.Id, 2))
        {
          if (deploymentPoolSecurity.HasPoolPermission(requestContext, taskAgentPool2.Id, 2))
          {
            try
            {
              IVssRequestContext vssRequestContext1 = requestContext.To(TeamFoundationHostType.Deployment);
              TeamFoundationHostManagementService service = vssRequestContext1.GetService<TeamFoundationHostManagementService>();
              using (IEnumerator<TeamFoundationServiceHostProperties> enumerator = service.QueryServiceHostProperties(vssRequestContext1, requestContext.ServiceHost.InstanceId, ServiceHostFilterFlags.IncludeChildren).Children.Where<TeamFoundationServiceHostProperties>((Func<TeamFoundationServiceHostProperties, bool>) (x => x.HostType == TeamFoundationHostType.ProjectCollection)).GetEnumerator())
              {
                while (enumerator.MoveNext())
                {
                  TeamFoundationServiceHostProperties current = enumerator.Current;
                  using (IVssRequestContext vssRequestContext2 = service.BeginRequest(vssRequestContext1, current.Id, RequestContextType.SystemContext, true, true))
                  {
                    List<DeploymentGroup> deploymentGroupList = new List<DeploymentGroup>();
                    using (TaskResourceComponent component = vssRequestContext2.CreateComponent<TaskResourceComponent>())
                      deploymentGroupList.AddRange((IEnumerable<DeploymentGroup>) component.GetAgentQueuesByPoolIds((IEnumerable<int>) new int[1]
                      {
                        taskAgentPool1.Id
                      }, TaskAgentQueueType.Deployment).MachineGroups);
                    foreach (DeploymentGroup deploymentGroup in deploymentGroupList)
                    {
                      deploymentGroup.Pool.Id = taskAgentPool2.Id;
                      this.UpdateDeploymentGroup(vssRequestContext2, deploymentGroup.Project.Id, deploymentGroup.Id, deploymentGroup);
                    }
                  }
                }
                return;
              }
            }
            catch (Exception ex)
            {
              throw new DeploymentGroupException(TaskResources.FailedToMigrateDeploymentGroups((object) sourcePoolName, (object) destinationPoolName, (object) ex.Message));
            }
          }
        }
        throw new AccessDeniedException(TaskResources.InsufficientPoolPermission());
      }
    }

    public async Task<DeploymentMachine> GetDeploymentTargetAsync(
      IVssRequestContext requestContext,
      Guid projectId,
      int deploymentGroupId,
      int targetId,
      bool includeCapabilities = false,
      bool includeAssignedRequest = false,
      bool includeLastCompletedRequest = false)
    {
      using (new MethodScope(requestContext, "ResourceService", nameof (GetDeploymentTargetAsync)))
      {
        Guid hostId = requestContext.ServiceHost.InstanceId;
        if (!this.Security.HasDeploymentGroupPermission(requestContext, projectId, deploymentGroupId, 1))
          return (DeploymentMachine) null;
        GetDeploymentMachinesResult result;
        using (TaskResourceComponent rc = requestContext.CreateComponent<TaskResourceComponent>())
          result = await rc.GetDeploymentTargetsAsync(projectId, deploymentGroupId, (IEnumerable<string>) new List<string>());
        DeploymentMachine target = (DeploymentMachine) null;
        if (result.MachineGroup != null)
        {
          IList<TaskAgent> filterPagedAsync = await requestContext.GetService<DistributedTaskResourceService>().GetAgentsByFilterPagedAsync(requestContext.Elevate(), result.MachineGroup.Pool.Id, hostId, projectId, includeCapabilities: (includeCapabilities ? 1 : 0) != 0, includeAssignedRequest: (includeAssignedRequest ? 1 : 0) != 0, includeLastCompletedRequest: (includeLastCompletedRequest ? 1 : 0) != 0, agentIds: (IList<int>) new int[1]
          {
            targetId
          }, top: 1);
          if (filterPagedAsync != null && filterPagedAsync.Count > 0)
          {
            TaskAgent taskAgent = filterPagedAsync[0];
            DeploymentMachine deploymentMachine = result.Machines.FirstOrDefault<DeploymentMachine>((Func<DeploymentMachine, bool>) (x => x.Agent.Id == targetId));
            if (deploymentMachine == null)
              deploymentMachine = new DeploymentMachine()
              {
                Id = taskAgent.Id
              };
            target = deploymentMachine;
            target.Agent = taskAgent;
            DeploymentGroupService.TrimResponse(requestContext, target, result.MachineGroup.Pool.Id);
          }
        }
        return target;
      }
    }

    public async Task<IPagedList<DeploymentMachine>> GetDeploymentTargetsPagedAsync(
      IVssRequestContext requestContext,
      Guid projectId,
      int deploymentGroupId,
      IList<string> tagFilters = null,
      string machineName = null,
      bool partialNameMatch = false,
      bool includeCapabilities = false,
      bool includeAssignedRequest = false,
      bool includeLastCompletedRequest = false,
      TaskAgentStatusFilter agentStatusFilter = TaskAgentStatusFilter.All,
      TaskAgentJobResultFilter agentJobResultFilter = TaskAgentJobResultFilter.All,
      string continuationToken = null,
      int top = 1000,
      bool? enabled = null,
      IList<string> propertyFilters = null)
    {
      string returnContinuationToken = (string) null;
      using (new MethodScope(requestContext, "ResourceService", nameof (GetDeploymentTargetsPagedAsync)))
      {
        Guid hostId = requestContext.ServiceHost.InstanceId;
        if (!this.Security.HasDeploymentGroupPermission(requestContext, projectId, deploymentGroupId, 1))
          return (IPagedList<DeploymentMachine>) new PagedList<DeploymentMachine>((IEnumerable<DeploymentMachine>) Array.Empty<DeploymentMachine>(), returnContinuationToken);
        GetDeploymentMachinesResult result;
        using (TaskResourceComponent rc = requestContext.CreateComponent<TaskResourceComponent>())
          result = await rc.GetDeploymentTargetsAsync(projectId, deploymentGroupId, (IEnumerable<string>) tagFilters);
        bool isMachineTagsFilterPresent = tagFilters != null && tagFilters.Count > 0;
        if (result.MachineGroup == null)
          return (IPagedList<DeploymentMachine>) new PagedList<DeploymentMachine>((IEnumerable<DeploymentMachine>) Array.Empty<DeploymentMachine>(), returnContinuationToken);
        List<int> list1 = isMachineTagsFilterPresent ? result.Machines.Select<DeploymentMachine, int>((Func<DeploymentMachine, int>) (machine => machine.Agent.Id)).ToList<int>() : (List<int>) null;
        List<DeploymentMachine> list2 = DeploymentGroupHelper.MapAgentsToMachines(result.Machines, (IEnumerable<TaskAgent>) await requestContext.GetService<DistributedTaskResourceService>().GetAgentsByFilterPagedAsync(requestContext.Elevate(), result.MachineGroup.Pool.Id, hostId, projectId, machineName, partialNameMatch, includeCapabilities, includeAssignedRequest, includeLastCompletedRequest, (IList<int>) list1, agentStatusFilter, agentJobResultFilter, continuationToken, top + 1, enabled), !isMachineTagsFilterPresent).ToList<DeploymentMachine>();
        list2.Sort((Comparison<DeploymentMachine>) ((m1, m2) => string.Compare(m1.Agent.Name, m2.Agent.Name, true)));
        if (list2.Count > top)
        {
          returnContinuationToken = list2[top].Agent.Name;
          list2 = list2.Take<DeploymentMachine>(top).ToList<DeploymentMachine>();
        }
        DeploymentGroupService.TrimResponse((IList<DeploymentMachine>) list2);
        if (list2.Count > 0 && propertyFilters != null && propertyFilters.Count > 0)
        {
          using (TeamFoundationDataReader properties = requestContext.GetService<ITeamFoundationPropertyService>().GetProperties(requestContext, list2.Select<DeploymentMachine, ArtifactSpec>((Func<DeploymentMachine, ArtifactSpec>) (x => x.CreateSpec())), (IEnumerable<string>) propertyFilters))
            ArtifactPropertyKinds.MatchProperties<DeploymentMachine>(properties, (IList<DeploymentMachine>) list2, (Func<DeploymentMachine, int>) (x => x.Id), (Action<DeploymentMachine, PropertiesCollection>) ((x, y) => x.Properties = y));
        }
        return (IPagedList<DeploymentMachine>) new PagedList<DeploymentMachine>((IEnumerable<DeploymentMachine>) list2, returnContinuationToken);
      }
    }

    public DeploymentGroup UpdateDeploymentGroup(
      IVssRequestContext requestContext,
      Guid projectId,
      int deploymentGroupId,
      DeploymentGroup deploymentGroup)
    {
      using (new MethodScope(requestContext, "ResourceService", nameof (UpdateDeploymentGroup)))
      {
        ArgumentValidation.CheckDeploymentGroup(deploymentGroup, nameof (deploymentGroup));
        this.Security.CheckDeploymentGroupPermission(requestContext, projectId, deploymentGroupId, 2);
        using (TaskResourceComponent component = requestContext.CreateComponent<TaskResourceComponent>())
        {
          TaskResourceComponent resourceComponent = component;
          Guid projectId1 = projectId;
          int queueId = deploymentGroupId;
          string name = deploymentGroup.Name;
          string description1 = deploymentGroup.Description;
          int? id = deploymentGroup.Pool?.Id;
          Guid? groupScopeId = new Guid?();
          bool? groupScopeProvisioned = new bool?();
          int? poolId = id;
          string description2 = description1;
          deploymentGroup = resourceComponent.UpdateAgentQueue(projectId1, queueId, name, groupScopeId, groupScopeProvisioned, poolId, TaskAgentQueueType.Deployment, description2).MachineGroup;
        }
        deploymentGroup.PopulateProjectName(requestContext, projectId);
        return deploymentGroup;
      }
    }

    public IList<TaskAgentJobRequest> GetAgentRequestsForDeploymentTarget(
      IVssRequestContext requestContext,
      Guid projectId,
      int deploymentGroupId,
      int targetId,
      int completedRequestCount = 50)
    {
      return this.GetAgentRequestsForDeploymentTargets(requestContext, projectId, deploymentGroupId, (IList<int>) new int[1]
      {
        targetId
      }, completedRequestCount, new int?(), new DateTime?());
    }

    public IList<TaskAgentJobRequest> GetAgentRequestsForDeploymentTargets(
      IVssRequestContext requestContext,
      Guid projectId,
      int deploymentGroupId,
      IList<int> targetIds,
      int completedRequestCount = 50,
      int? ownerId = null,
      DateTime? completedOn = null)
    {
      using (new MethodScope(requestContext, "ResourceService", nameof (GetAgentRequestsForDeploymentTargets)))
      {
        if (!this.Security.HasDeploymentGroupPermission(requestContext, projectId, deploymentGroupId, 1))
          return (IList<TaskAgentJobRequest>) Array.Empty<TaskAgentJobRequest>();
        DeploymentGroup deploymentGroupInternal = DeploymentGroupService.GetDeploymentGroupInternal(requestContext, projectId, deploymentGroupId);
        if (deploymentGroupInternal == null)
          return (IList<TaskAgentJobRequest>) Array.Empty<TaskAgentJobRequest>();
        Guid instanceId = requestContext.ServiceHost.InstanceId;
        return requestContext.GetService<IDistributedTaskResourceService>().GetAgentRequestsForAgents(requestContext.Elevate(), deploymentGroupInternal.Pool.Id, targetIds, instanceId, projectId, completedRequestCount, ownerId, completedOn);
      }
    }

    public string GeneratePersonalAccessTokenWithDeploymentGroupScope(
      IVssRequestContext requestContext,
      Guid projectId,
      int deploymentGroupId)
    {
      this.Security.CheckDeploymentGroupPermission(requestContext, projectId, deploymentGroupId, 2);
      string tokenName = string.Format("Deployment Group-Personal Access Token {0}", (object) DateTime.UtcNow);
      return this.GeneratePersonalAccessTokenWithDeploymentGroupScope(requestContext, tokenName);
    }

    public string GeneratePersonalAccessTokenWithDeploymentPoolScope(
      IVssRequestContext requestContext,
      int poolId)
    {
      this.GetDeploymentPoolSecurity(requestContext).CheckPoolPermission(requestContext, poolId, 2);
      string tokenName = string.Format("Deployment Pool-Personal Access Token {0}", (object) DateTime.UtcNow);
      return this.GeneratePersonalAccessTokenWithDeploymentGroupScope(requestContext, tokenName);
    }

    public async Task SendRefreshMessageToDeploymentTargetsAsync(
      IVssRequestContext requestContext,
      Guid projectId,
      int deploymentGroupId)
    {
      MethodScope methodScope = new MethodScope(requestContext, "ResourceService", nameof (SendRefreshMessageToDeploymentTargetsAsync));
      try
      {
        this.Security.CheckDeploymentGroupPermission(requestContext, projectId, deploymentGroupId, 2);
        DeploymentGroup groupInternalAsync = await DeploymentGroupService.GetDeploymentGroupInternalAsync(requestContext, projectId, deploymentGroupId);
        if (groupInternalAsync == null)
          throw new DeploymentMachineGroupNotFoundException(TaskResources.DeploymentMachineGroupNotFound((object) deploymentGroupId));
        await requestContext.GetService<IDistributedTaskResourceService>().SendRefreshMessageToAgentsAsync(requestContext, groupInternalAsync.Pool.Id);
      }
      finally
      {
        methodScope.Dispose();
      }
      methodScope = new MethodScope();
    }

    public IPagedList<DeploymentGroupMetrics> GetDeploymentGroupsMetrics(
      IVssRequestContext requestContext,
      Guid projectId,
      string deploymentGroupName,
      string lastDeploymentGroupName = null,
      int maxDeploymentGroupsCount = 50)
    {
      if (maxDeploymentGroupsCount < 0 || maxDeploymentGroupsCount > 50)
        maxDeploymentGroupsCount = 50;
      string continuationToken = (string) null;
      IList<DeploymentGroup> deploymentGroupList;
      using (TaskResourceComponent component = requestContext.CreateComponent<TaskResourceComponent>())
        deploymentGroupList = component.GetAgentQueues(projectId, deploymentGroupName, TaskAgentQueueType.Deployment, includeTags: false, lastQueueName: lastDeploymentGroupName, maxQueuesCount: maxDeploymentGroupsCount + 1).MachineGroups;
      if (deploymentGroupList.Count > maxDeploymentGroupsCount)
      {
        continuationToken = deploymentGroupList[maxDeploymentGroupsCount].Name;
        deploymentGroupList = (IList<DeploymentGroup>) deploymentGroupList.Take<DeploymentGroup>(maxDeploymentGroupsCount).ToList<DeploymentGroup>();
      }
      List<DeploymentGroup> source = this.FilterDeploymentGroupByActionFilter(requestContext, projectId, deploymentGroupList);
      IEnumerable<DeploymentGroupMetrics> list = (IEnumerable<DeploymentGroupMetrics>) null;
      if (source.Any<DeploymentGroup>())
      {
        Dictionary<int, DeploymentGroup> deploymentGroups = new Dictionary<int, DeploymentGroup>();
        foreach (DeploymentGroup deploymentGroup in source)
          deploymentGroups.Add(deploymentGroup.Id, deploymentGroup.PopulatePoolReference(requestContext));
        using (TaskResourceComponent component = requestContext.ToPoolRequestContext().CreateComponent<TaskResourceComponent>())
          list = component.GetDeploymentGroupsMetrics((IDictionary<int, DeploymentGroup>) deploymentGroups, requestContext.ServiceHost.InstanceId, projectId);
      }
      if (list == null)
        list = (IEnumerable<DeploymentGroupMetrics>) new List<DeploymentGroupMetrics>();
      return (IPagedList<DeploymentGroupMetrics>) new PagedList<DeploymentGroupMetrics>(list, continuationToken);
    }

    public DeploymentMachine AddDeploymentTarget(
      IVssRequestContext requestContext,
      Guid projectId,
      int deploymentGroupId,
      DeploymentMachine machine)
    {
      using (new MethodScope(requestContext, "ResourceService", nameof (AddDeploymentTarget)))
      {
        ArgumentValidation.CheckDeploymentMachine(machine, nameof (machine));
        this.Security.CheckDeploymentGroupPermission(requestContext, projectId, deploymentGroupId, 2);
        DeploymentGroup deploymentGroupInternal = DeploymentGroupService.GetDeploymentGroupInternal(requestContext, projectId, deploymentGroupId);
        if (deploymentGroupInternal == null)
          throw new DeploymentMachineGroupNotFoundException(TaskResources.DeploymentMachineGroupNotFound((object) deploymentGroupId));
        TaskAgent taskAgent = requestContext.GetService<IDistributedTaskResourceService>().AddAgent(requestContext, deploymentGroupInternal.Pool.Id, machine.Agent);
        DeploymentMachine deploymentMachine1 = machine.Clone();
        deploymentMachine1.Agent = taskAgent;
        DeploymentMachine deploymentMachine2;
        using (TaskResourceComponent component = requestContext.CreateComponent<TaskResourceComponent>())
          deploymentMachine2 = component.AddDeploymentTarget(projectId, deploymentGroupId, deploymentMachine1);
        deploymentMachine2.Agent = taskAgent;
        DeploymentMachineChangedData machineChangedData = new DeploymentMachineChangedData(deploymentMachine2);
        if (!deploymentMachine2.Tags.IsNullOrEmpty<string>())
        {
          machineChangedData.TagsAdded = deploymentMachine2.Tags;
          DeploymentGroupService.RaiseDeploymentMachinesChangedEvent(requestContext, projectId, deploymentGroupInternal, (IList<DeploymentMachineChangedData>) new DeploymentMachineChangedData[1]
          {
            machineChangedData
          });
        }
        else
          requestContext.GetService<IDistributedTaskEventPublisherService>().NotifyMachinesChangedEvent(requestContext, deploymentGroupInternal, (IList<DeploymentMachineChangedData>) new List<DeploymentMachineChangedData>()
          {
            machineChangedData
          });
        if (machine.Properties != null && machine.Properties.Count > 0)
          requestContext.GetService<ITeamFoundationPropertyService>().SetProperties(requestContext, deploymentMachine2.CreateSpec(), machine.Properties.Convert());
        return deploymentMachine2;
      }
    }

    public DeploymentMachine UpdateDeploymentTarget(
      IVssRequestContext requestContext,
      Guid projectId,
      int deploymentGroupId,
      DeploymentMachine machine,
      TaskAgentCapabilityType capabilityUpdate = TaskAgentCapabilityType.System)
    {
      using (new MethodScope(requestContext, "ResourceService", nameof (UpdateDeploymentTarget)))
      {
        ArgumentValidation.CheckDeploymentMachine(machine, nameof (machine), false);
        this.Security.CheckDeploymentGroupPermission(requestContext, projectId, deploymentGroupId, 2);
        if (machine.Id != machine.Agent.Id)
          throw new InvalidDeploymentMachineException(TaskResources.DeploymentMachineIdDoesNotMatchWithAgentId((object) machine.Id, (object) machine.Agent.Id));
        DeploymentGroup deploymentGroupInternal = DeploymentGroupService.GetDeploymentGroupInternal(requestContext, projectId, deploymentGroupId);
        if (deploymentGroupInternal == null)
          throw new DeploymentMachineGroupNotFoundException(TaskResources.DeploymentMachineGroupNotFound((object) deploymentGroupId));
        TaskAgent taskAgent = requestContext.GetService<IDistributedTaskResourceService>().UpdateAgent(requestContext, deploymentGroupInternal.Pool.Id, machine.Agent, capabilityUpdate);
        UpdateDeploymentMachinesResult deploymentMachinesResult;
        using (TaskResourceComponent component = requestContext.CreateComponent<TaskResourceComponent>())
          deploymentMachinesResult = component.UpdateDeploymentTargets(projectId, deploymentGroupId, (IEnumerable<DeploymentMachine>) new DeploymentMachine[1]
          {
            machine
          });
        DeploymentMachineChangedData machineChangedData1 = deploymentMachinesResult.Machines.FirstOrDefault<DeploymentMachineChangedData>((Func<DeploymentMachineChangedData, bool>) (x => x.Agent.Id == machine.Agent.Id));
        if (machineChangedData1 == null)
        {
          DeploymentMachineChangedData machineChangedData2 = new DeploymentMachineChangedData();
          machineChangedData2.Id = taskAgent.Id;
          machineChangedData1 = machineChangedData2;
        }
        DeploymentMachineChangedData machineChangedData3 = machineChangedData1;
        machineChangedData3.Agent = taskAgent;
        DeploymentGroupService.RaiseDeploymentMachinesChangedEvent(requestContext, projectId, deploymentGroupInternal, DeploymentGroupService.FilterUpdatedMachinesWithoutNullTags((IList<DeploymentMachine>) new DeploymentMachine[1]
        {
          machine
        }, (IList<DeploymentMachineChangedData>) new DeploymentMachineChangedData[1]
        {
          machineChangedData3
        }));
        return (DeploymentMachine) machineChangedData3;
      }
    }

    public IList<DeploymentMachine> UpdateDeploymentTargets(
      IVssRequestContext requestContext,
      Guid projectId,
      int deploymentGroupId,
      IList<DeploymentMachine> machinesToUpdate)
    {
      using (new MethodScope(requestContext, "ResourceService", nameof (UpdateDeploymentTargets)))
      {
        ArgumentUtility.CheckForNull<IList<DeploymentMachine>>(machinesToUpdate, nameof (machinesToUpdate), "DistributedTask");
        this.Security.CheckDeploymentGroupPermission(requestContext, projectId, deploymentGroupId, 2);
        foreach (DeploymentMachine var in (IEnumerable<DeploymentMachine>) machinesToUpdate)
        {
          ArgumentUtility.CheckForNull<DeploymentMachine>(var, "machine", "DistributedTask");
          ArgumentValidation.CheckDeploymentTargetTags(var.Tags);
        }
        DeploymentGroup deploymentGroupInternal = DeploymentGroupService.GetDeploymentGroupInternal(requestContext, projectId, deploymentGroupId);
        if (deploymentGroupInternal == null)
          throw new DeploymentMachineGroupNotFoundException(TaskResources.DeploymentMachineGroupNotFound((object) deploymentGroupId));
        IList<TaskAgent> agents = requestContext.GetService<IDistributedTaskResourceService>().GetAgents(requestContext.Elevate(), deploymentGroupInternal.Pool.Id, machinesToUpdate.Select<DeploymentMachine, int>((Func<DeploymentMachine, int>) (x => x.Id)));
        ILookup<int, TaskAgent> lookup = agents.ToLookup<TaskAgent, int>((Func<TaskAgent, int>) (x => x.Id));
        foreach (DeploymentMachine deploymentMachine in (IEnumerable<DeploymentMachine>) machinesToUpdate)
        {
          if (!lookup.Contains(deploymentMachine.Id))
            throw new TaskAgentNotFoundException(TaskResources.MachineNotFound((object) deploymentGroupId, (object) deploymentMachine.Id));
        }
        UpdateDeploymentMachinesResult deploymentMachinesResult;
        using (TaskResourceComponent component = requestContext.CreateComponent<TaskResourceComponent>())
          deploymentMachinesResult = component.UpdateDeploymentTargets(projectId, deploymentGroupId, (IEnumerable<DeploymentMachine>) machinesToUpdate);
        IList<DeploymentMachineChangedData> machinesChangedData = DeploymentGroupHelper.MapAgentsToMachinesChangedData(deploymentMachinesResult.Machines, (IEnumerable<TaskAgent>) agents);
        IList<DeploymentMachineChangedData> changedMachines = DeploymentGroupService.FilterUpdatedMachinesWithoutNullTags(machinesToUpdate, machinesChangedData);
        DeploymentGroupService.RaiseDeploymentMachinesChangedEvent(requestContext, projectId, deploymentGroupInternal, changedMachines);
        IList<DeploymentMachine> deploymentMachines = DeploymentGroupHelper.convertToDeploymentMachines(machinesChangedData);
        DeploymentGroupService.TrimResponse(deploymentMachines);
        return deploymentMachines;
      }
    }

    public void DeleteDeploymentTarget(
      IVssRequestContext requestContext,
      Guid projectId,
      int deploymentGroupId,
      int machineId)
    {
      using (new MethodScope(requestContext, "ResourceService", nameof (DeleteDeploymentTarget)))
      {
        this.Security.CheckDeploymentGroupPermission(requestContext, projectId, deploymentGroupId, 2);
        DeploymentGroup deploymentGroupInternal = DeploymentGroupService.GetDeploymentGroupInternal(requestContext, projectId, deploymentGroupId);
        if (deploymentGroupInternal == null)
          return;
        requestContext.GetService<IDistributedTaskResourceService>().DeleteAgents(requestContext, deploymentGroupInternal.Pool.Id, (IEnumerable<int>) new int[1]
        {
          machineId
        });
        using (TaskResourceComponent component = requestContext.CreateComponent<TaskResourceComponent>())
          component.DeleteDeploymentTarget(projectId, deploymentGroupId, machineId);
      }
    }

    public async Task<IList<DeploymentMachine>> GetDeploymentMachinesAsync(
      IVssRequestContext requestContext,
      Guid projectId,
      int deploymentGroupId,
      IList<string> tagFilters = null,
      string machineName = null,
      bool includeCapabilities = false,
      bool includeAssignedRequest = false)
    {
      requestContext.AssertAsyncExecutionEnabled();
      using (new MethodScope(requestContext, "ResourceService", nameof (GetDeploymentMachinesAsync)))
      {
        if (!this.Security.HasDeploymentGroupPermission(requestContext, projectId, deploymentGroupId, 1))
          return (IList<DeploymentMachine>) Array.Empty<DeploymentMachine>();
        GetDeploymentMachinesResult result;
        using (TaskResourceComponent rc = requestContext.CreateComponent<TaskResourceComponent>())
          result = await rc.GetDeploymentMachinesAsync(projectId, deploymentGroupId, (IEnumerable<string>) tagFilters);
        return result.MachineGroup != null && result.Machines.Any<DeploymentMachine>() ? DeploymentGroupHelper.MapAgentsToMachines(result.Machines, (IEnumerable<TaskAgent>) await requestContext.GetService<DistributedTaskResourceService>().GetAgentsAsync(requestContext.Elevate(), result.MachineGroup.Pool.Id, machineName, includeCapabilities, includeAssignedRequest), false) : (IList<DeploymentMachine>) Array.Empty<DeploymentMachine>();
      }
    }

    public IList<DeploymentMachine> UpdateDeploymentMachines(
      IVssRequestContext requestContext,
      Guid projectId,
      int deploymentGroupId,
      IList<DeploymentMachine> machinesToUpdate)
    {
      using (new MethodScope(requestContext, "ResourceService", nameof (UpdateDeploymentMachines)))
      {
        ArgumentUtility.CheckForNull<IList<DeploymentMachine>>(machinesToUpdate, nameof (machinesToUpdate), "DistributedTask");
        foreach (DeploymentMachine machine in (IEnumerable<DeploymentMachine>) machinesToUpdate)
          ArgumentValidation.CheckDeploymentMachine(machine, "machine", false, false);
        this.Security.CheckDeploymentGroupPermission(requestContext, projectId, deploymentGroupId, 2);
        DeploymentGroup deploymentGroupInternal = DeploymentGroupService.GetDeploymentGroupInternal(requestContext, projectId, deploymentGroupId);
        if (deploymentGroupInternal == null)
          throw new DeploymentMachineGroupNotFoundException(TaskResources.DeploymentMachineGroupNotFound((object) deploymentGroupId));
        IList<TaskAgent> agents = requestContext.GetService<IDistributedTaskResourceService>().GetAgents(requestContext.Elevate(), deploymentGroupInternal.Pool.Id, machinesToUpdate.Select<DeploymentMachine, int>((Func<DeploymentMachine, int>) (x => x.Agent.Id)));
        UpdateDeploymentMachinesResult deploymentMachinesResult;
        using (TaskResourceComponent component = requestContext.CreateComponent<TaskResourceComponent>())
          deploymentMachinesResult = component.UpdateDeploymentMachines(projectId, deploymentGroupId, (IEnumerable<DeploymentMachine>) machinesToUpdate);
        IList<DeploymentMachineChangedData> machinesChangedData = DeploymentGroupHelper.MapAgentsToMachinesChangedData(deploymentMachinesResult.Machines, (IEnumerable<TaskAgent>) agents, false);
        DeploymentGroupService.RaiseDeploymentMachinesChangedEvent(requestContext, projectId, deploymentGroupInternal, machinesChangedData);
        return DeploymentGroupHelper.convertToDeploymentMachines(machinesChangedData);
      }
    }

    internal IAgentPoolSecurityProvider GetDeploymentPoolSecurity(IVssRequestContext requestContext) => requestContext.GetService<DistributedTaskResourceService>().GetAgentPoolSecurity(requestContext, TaskAgentPoolType.Deployment);

    private static void RaiseDeploymentMachinesChangedEvent(
      IVssRequestContext requestContext,
      Guid projectId,
      DeploymentGroup deploymentGroup,
      IList<DeploymentMachineChangedData> changedMachines)
    {
      try
      {
        requestContext.GetService<IDeploymentGroupHubDispatcher>().NotifyDeploymentMachinesUpdated(requestContext, projectId, deploymentGroup.Id, DeploymentGroupHelper.convertToDeploymentMachines(changedMachines));
      }
      catch (Exception ex)
      {
        requestContext.TraceException(10015125, "ResourceService", ex);
      }
      requestContext.GetService<IDistributedTaskEventPublisherService>().NotifyMachinesChangedEvent(requestContext, deploymentGroup, (IList<DeploymentMachineChangedData>) changedMachines.ToList<DeploymentMachineChangedData>());
    }

    private IList<DeploymentGroup> GetDeploymentGroupsInternal(
      IVssRequestContext requestContext,
      Guid projectId,
      string deploymentGroupName,
      bool includeMachines,
      string lastDeploymentGroupName = null,
      int maxDeploymentGroupCount = 50)
    {
      using (TaskResourceComponent component = requestContext.CreateComponent<TaskResourceComponent>())
        return component.GetAgentQueues(projectId, deploymentGroupName, TaskAgentQueueType.Deployment, includeMachines, includeMachines, lastDeploymentGroupName, maxDeploymentGroupCount).MachineGroups;
    }

    private static DeploymentGroup GetDeploymentGroupInternal(
      IVssRequestContext requestContext,
      Guid projectId,
      int deploymentGroupId,
      bool includeMachines = false,
      bool includeTags = false)
    {
      using (TaskResourceComponent component = requestContext.CreateComponent<TaskResourceComponent>())
        return component.GetAgentQueue(projectId, deploymentGroupId, TaskAgentQueueType.Deployment, includeMachines, includeTags).MachineGroup;
    }

    private static List<DeploymentGroup> GetDeploymentGroupsByIdInternal(
      IVssRequestContext requestContext,
      Guid projectId,
      IList<int> deploymentGroupIds,
      bool includeMachines = false)
    {
      List<DeploymentGroup> groupsByIdInternal = new List<DeploymentGroup>();
      using (TaskResourceComponent component = requestContext.CreateComponent<TaskResourceComponent>())
        groupsByIdInternal.AddRange((IEnumerable<DeploymentGroup>) component.GetAgentQueuesById(projectId, (IEnumerable<int>) deploymentGroupIds, TaskAgentQueueType.Deployment, includeMachines, includeMachines).MachineGroups);
      return groupsByIdInternal;
    }

    private List<DeploymentGroup> FilterDeploymentGroupByActionFilter(
      IVssRequestContext requestContext,
      Guid projectId,
      IList<DeploymentGroup> deploymentGroups,
      DeploymentGroupActionFilter actionFilter = DeploymentGroupActionFilter.None)
    {
      int additionalPermissions = 0;
      if (actionFilter != DeploymentGroupActionFilter.None)
        additionalPermissions = DeploymentGroupService.ConvertActionFilterToPermissions(actionFilter);
      return deploymentGroups.Where<DeploymentGroup>((Func<DeploymentGroup, bool>) (x =>
      {
        if (!this.Security.HasDeploymentGroupPermission(requestContext, projectId, x.Id, 1, true))
          return false;
        return additionalPermissions == 0 || this.Security.HasDeploymentGroupPermission(requestContext, projectId, x.Id, additionalPermissions);
      })).ToList<DeploymentGroup>();
    }

    private List<DeploymentGroup> FillMachineDetailsForDeploymentGroups(
      IVssRequestContext requestContext,
      Guid projectId,
      List<DeploymentGroup> deploymentGroups,
      bool includeMachines)
    {
      foreach (DeploymentGroup deploymentGroup in deploymentGroups)
      {
        deploymentGroup.PopulateProjectName(requestContext, projectId);
        deploymentGroup.PopulatePoolReference(requestContext);
        if (includeMachines)
          deploymentGroup.PopulateAgentReferences(requestContext, false);
      }
      return deploymentGroups;
    }

    private static async Task<DeploymentGroup> GetDeploymentGroupInternalAsync(
      IVssRequestContext requestContext,
      Guid projectId,
      int deploymentGroupId,
      bool includeMachines = false)
    {
      DeploymentGroup machineGroup;
      using (TaskResourceComponent rc = requestContext.CreateComponent<TaskResourceComponent>())
        machineGroup = (await rc.GetAgentQueueAsync(projectId, deploymentGroupId, TaskAgentQueueType.Deployment, includeMachines)).MachineGroup;
      return machineGroup;
    }

    public void InitializeDeploymentGroupSecurity(IVssRequestContext requestContext, Guid projectId)
    {
      IdentityService service = requestContext.GetService<IdentityService>();
      List<AccessControlEntry> accessControlEntryList = new List<AccessControlEntry>();
      Microsoft.VisualStudio.Services.Identity.Identity identity1 = this.Security.EnsureDeploymentGroupAdministratorsGroupIsProvisioned(requestContext, projectId);
      accessControlEntryList.Add(new AccessControlEntry()
      {
        Descriptor = identity1.Descriptor,
        Allow = 59
      });
      Microsoft.VisualStudio.Services.Identity.Identity identity2 = service.GetGroups(requestContext, projectId, (IEnumerable<IdentityDescriptor>) new IdentityDescriptor[1]
      {
        GroupWellKnownIdentityDescriptors.NamespaceAdministratorsGroup
      }).FirstOrDefault<Microsoft.VisualStudio.Services.Identity.Identity>();
      if (identity2 != null)
        accessControlEntryList.Add(new AccessControlEntry()
        {
          Descriptor = identity2.Descriptor,
          Allow = 59
        });
      Microsoft.VisualStudio.Services.Identity.Identity identity3 = service.GetGroups(requestContext, projectId, (IList<string>) new string[1]
      {
        TaskResources.ProjectReleaseAdminAccountName()
      }).FirstOrDefault<Microsoft.VisualStudio.Services.Identity.Identity>();
      if (identity3 != null)
        accessControlEntryList.Add(new AccessControlEntry()
        {
          Descriptor = identity3.Descriptor,
          Allow = 59
        });
      Microsoft.VisualStudio.Services.Identity.Identity identity4 = service.GetGroups(requestContext, projectId, (IList<string>) new string[1]
      {
        TaskResources.ProjectContributorsGroupName()
      }).FirstOrDefault<Microsoft.VisualStudio.Services.Identity.Identity>();
      if (identity4 != null)
        accessControlEntryList.Add(new AccessControlEntry()
        {
          Descriptor = identity4.Descriptor,
          Allow = 33
        });
      IVssRequestContext vssRequestContext = requestContext.Elevate();
      vssRequestContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(vssRequestContext, DefaultSecurityProvider.NamespaceId).SetAccessControlEntries(vssRequestContext, DefaultSecurityProvider.GetDeploymentGroupToken(vssRequestContext, projectId), (IEnumerable<IAccessControlEntry>) accessControlEntryList, true);
    }

    private void CheckDeploymentGroupCreatePermissions(
      IVssRequestContext requestContext,
      Guid projectId)
    {
      if (!this.GetDeploymentGroupsInternal(requestContext, projectId, (string) null, false).Any<DeploymentGroup>())
        this.InitializeDeploymentGroupSecurity(requestContext, projectId);
      this.Security.CheckDeploymentGroupPermission(requestContext, projectId, 32);
    }

    private void CleanupDeploymentGroupRolesAndSecurity(
      IVssRequestContext requestContext,
      Guid projectId,
      DeploymentGroup deploymentGroup)
    {
      try
      {
        this.Security.RemoveAccessControlLists(requestContext, projectId, deploymentGroup);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(10015013, "ResourceService", ex);
      }
    }

    private void CheckViewAndOtherPermissionsForDeploymentPool(
      IVssRequestContext requestContext,
      int poolId,
      int otherPermissions)
    {
      requestContext = requestContext.ToPoolRequestContext();
      IAgentPoolSecurityProvider deploymentPoolSecurity = this.GetDeploymentPoolSecurity(requestContext);
      if (!deploymentPoolSecurity.HasPoolPermission(requestContext, poolId, 1))
        throw new TaskAgentPoolNotFoundException(TaskResources.AgentPoolNotFound((object) poolId));
      if (deploymentPoolSecurity.HasPoolPermission(requestContext, poolId, otherPermissions))
        return;
      PoolSecurityProvider.ThrowPoolAccessDeniedException(requestContext, otherPermissions, poolId);
    }

    private string GetAutoProvisionedPoolName(
      IVssRequestContext requestContext,
      Guid projectId,
      DeploymentGroup deploymentGroup)
    {
      string poolName = requestContext.GetService<IProjectService>().GetProjectName(requestContext, projectId) + "-" + deploymentGroup.Name;
      try
      {
        ArgumentValidation.CheckPoolName(ref poolName, "pool name", false);
      }
      catch (ArgumentException ex)
      {
        return deploymentGroup.Name;
      }
      return poolName;
    }

    private static int ConvertActionFilterToPermissions(DeploymentGroupActionFilter actionFilter)
    {
      int permissions = 0;
      if ((actionFilter & DeploymentGroupActionFilter.Manage) == DeploymentGroupActionFilter.Manage)
        permissions |= 2;
      if ((actionFilter & DeploymentGroupActionFilter.Use) == DeploymentGroupActionFilter.Use)
        permissions |= 16;
      return permissions;
    }

    private string GeneratePersonalAccessTokenWithDeploymentGroupScope(
      IVssRequestContext requestContext,
      string tokenName)
    {
      IDelegatedAuthorizationService service = requestContext.GetService<IDelegatedAuthorizationService>();
      IVssRequestContext requestContext1 = requestContext;
      string str = tokenName;
      DateTime? nullable = new DateTime?(DateTime.UtcNow.AddHours(3.0));
      IList<Guid> guidList = (IList<Guid>) new Guid[1]
      {
        requestContext.ServiceHost.InstanceId
      };
      Guid? clientId = new Guid?();
      Guid? userId = new Guid?();
      string name = str;
      DateTime? validTo = nullable;
      IList<Guid> targetAccounts = guidList;
      Guid? authorizationId = new Guid?();
      Guid? accessId = new Guid?();
      SessionTokenResult sessionTokenResult = service.IssueSessionToken(requestContext1, clientId, userId, name, validTo, "vso.machinegroup_manage", targetAccounts, SessionTokenType.Compact, authorizationId: authorizationId, accessId: accessId);
      if (sessionTokenResult != null && sessionTokenResult.HasError)
        throw new SessionTokenException(sessionTokenResult.SessionTokenError);
      return sessionTokenResult?.SessionToken.Token;
    }

    private static void TrimResponse(
      IVssRequestContext requestContext,
      DeploymentMachine target,
      int poolId)
    {
      if (requestContext.GetService<DistributedTaskResourceService>().GetAgentPoolSecurity(requestContext, poolId).HasPoolPermission(requestContext, 2))
        return;
      target.Agent.Authorization = (TaskAgentAuthorization) null;
    }

    private static void TrimResponse(IList<DeploymentMachine> targets)
    {
      foreach (DeploymentMachine target in (IEnumerable<DeploymentMachine>) targets)
        target.Agent.Authorization = (TaskAgentAuthorization) null;
    }

    private static IList<DeploymentMachineChangedData> FilterUpdatedMachinesWithoutNullTags(
      IList<DeploymentMachine> machinesToUpdate,
      IList<DeploymentMachineChangedData> deploymentMachineChangedDataList)
    {
      Dictionary<int, DeploymentMachine> machinesToUpdateDictionary = machinesToUpdate.ToDictionary<DeploymentMachine, int>((Func<DeploymentMachine, int>) (x => x.Id));
      return (IList<DeploymentMachineChangedData>) deploymentMachineChangedDataList.Where<DeploymentMachineChangedData>((Func<DeploymentMachineChangedData, bool>) (x => machinesToUpdateDictionary.ContainsKey(x.Id) && machinesToUpdateDictionary[x.Id].Tags != null)).ToList<DeploymentMachineChangedData>();
    }

    void IVssFrameworkService.ServiceStart(IVssRequestContext requestContext)
    {
    }

    void IVssFrameworkService.ServiceEnd(IVssRequestContext requestContext)
    {
    }
  }
}
