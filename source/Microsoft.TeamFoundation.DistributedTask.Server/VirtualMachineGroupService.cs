// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.VirtualMachineGroupService
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.DistributedTask.Core.Server.Plugins;
using Microsoft.TeamFoundation.DistributedTask.Server.DataAccess;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.DistributedTask.Server
{
  internal class VirtualMachineGroupService : 
    EnvironmentResourceService<VirtualMachineGroup>,
    IVirtualMachineGroupService,
    IEnvironmentResourceService<VirtualMachineGroup>,
    IVssFrameworkService
  {
    public VirtualMachineGroupService()
      : this((Func<IVssRequestContext, Microsoft.VisualStudio.Services.Identity.Identity>) (rc => rc.GetUserIdentity()), (Func<IVssRequestContext, Microsoft.VisualStudio.Services.Identity.Identity, IdentityRef>) ((rc, id) => id.ToIdentityRef(rc)))
    {
    }

    protected VirtualMachineGroupService(
      Func<IVssRequestContext, Microsoft.VisualStudio.Services.Identity.Identity> getCurrentUserIdentity,
      Func<IVssRequestContext, Microsoft.VisualStudio.Services.Identity.Identity, IdentityRef> toIdentityRef)
      : base(getCurrentUserIdentity, toIdentityRef)
    {
    }

    public async Task<IPagedList<VirtualMachine>> GetVirtualMachines(
      IVssRequestContext requestContext,
      Guid projectId,
      int environmentId,
      int resourceId,
      string continuationToken = null,
      int top = 1000,
      string name = null,
      bool partialNameMatch = false,
      IList<string> tagFilters = null)
    {
      VirtualMachineGroupService machineGroupService = this;
      string returnContinuationToken = (string) null;
      using (new MethodScope(requestContext, machineGroupService.c_layer, nameof (GetVirtualMachines)))
      {
        Guid instanceId = requestContext.ServiceHost.InstanceId;
        int poolId = machineGroupService.GetEnvironmentResource(requestContext, projectId, environmentId, resourceId).PoolId;
        GetVirtualMachinesResult result;
        using (VirtualMachineGroupComponent component = requestContext.CreateComponent<VirtualMachineGroupComponent>())
          result = component.GetVirtualMachines(projectId, environmentId, resourceId, (IEnumerable<string>) tagFilters);
        if (result.VirtualMachineGroup == null)
          return (IPagedList<VirtualMachine>) new PagedList<VirtualMachine>((IEnumerable<VirtualMachine>) Array.Empty<VirtualMachine>(), returnContinuationToken);
        bool isMachineTagsFilterPresent = tagFilters != null && tagFilters.Count > 0;
        List<int> list1 = isMachineTagsFilterPresent ? result.VirtualMachines.Select<VirtualMachine, int>((Func<VirtualMachine, int>) (machine => machine.Agent.Id)).ToList<int>() : (List<int>) null;
        List<VirtualMachine> list2 = VirtualMachineGroupHelper.MapAgentsToMachines(result.VirtualMachines, (IEnumerable<TaskAgent>) await requestContext.GetService<DistributedTaskResourceService>().GetAgentsByFilterPagedAsync(requestContext.Elevate(), poolId, instanceId, projectId, name, partialNameMatch, includeAssignedRequest: true, includeLastCompletedRequest: true, agentIds: (IList<int>) list1, continuationToken: continuationToken, top: top + 1), !isMachineTagsFilterPresent).ToList<VirtualMachine>();
        list2.Sort((Comparison<VirtualMachine>) ((vm1, vm2) => string.Compare(vm1.Agent.Name, vm2.Agent.Name, true)));
        if (list2.Count > top)
        {
          returnContinuationToken = list2[top].Agent.Name;
          list2 = list2.Take<VirtualMachine>(top).ToList<VirtualMachine>();
        }
        VirtualMachineGroupService.TrimResponse((IList<VirtualMachine>) list2);
        return (IPagedList<VirtualMachine>) new PagedList<VirtualMachine>((IEnumerable<VirtualMachine>) list2, returnContinuationToken);
      }
    }

    public IList<VirtualMachine> UpdateVirtualMachines(
      IVssRequestContext requestContext,
      Guid projectId,
      int environmentId,
      int resourceId,
      IList<VirtualMachine> machinesToUpdate)
    {
      using (new MethodScope(requestContext, this.c_layer, nameof (UpdateVirtualMachines)))
      {
        ArgumentUtility.CheckForNull<IList<VirtualMachine>>(machinesToUpdate, nameof (machinesToUpdate));
        VirtualMachineGroup environmentResource = this.GetEnvironmentResource(requestContext, projectId, environmentId, resourceId);
        foreach (VirtualMachine var in (IEnumerable<VirtualMachine>) machinesToUpdate)
        {
          ArgumentUtility.CheckForNull<VirtualMachine>(var, "machine");
          ArgumentValidation.CheckDeploymentTargetTags(var.Tags);
        }
        this.GetDeploymentPoolSecurity(requestContext).CheckPoolPermission(requestContext, environmentResource.PoolId, 2);
        ILookup<int, TaskAgent> lookup = requestContext.GetService<IDistributedTaskResourceService>().GetAgents(requestContext, environmentResource.PoolId, machinesToUpdate.Select<VirtualMachine, int>((Func<VirtualMachine, int>) (x => x.Id))).ToLookup<TaskAgent, int>((Func<TaskAgent, int>) (x => x.Id));
        foreach (VirtualMachine virtualMachine in (IEnumerable<VirtualMachine>) machinesToUpdate)
        {
          if (!lookup.Contains(virtualMachine.Id))
            throw new TaskAgentNotFoundException(TaskResources.VirtualMachineNotFound((object) virtualMachine.Id));
        }
        UpdateVirtualMachinesResult virtualMachinesResult;
        using (VirtualMachineGroupComponent component = requestContext.CreateComponent<VirtualMachineGroupComponent>())
          virtualMachinesResult = component.UpdateVirtualMachines(projectId, environmentId, resourceId, (IEnumerable<VirtualMachine>) machinesToUpdate);
        return (IList<VirtualMachine>) virtualMachinesResult.VirtualMachines.ToList<VirtualMachine>();
      }
    }

    public IList<VirtualMachineGroup> GetVirtualMachineGroupsByPoolIds(
      IVssRequestContext requestContext,
      IEnumerable<int> poolIds)
    {
      using (VirtualMachineGroupComponent component = requestContext.CreateComponent<VirtualMachineGroupComponent>())
        return component.GetVirtualMachineGroupsByPoolIds(poolIds);
    }

    protected override IEnvironmentResourceComponent<VirtualMachineGroup> GetResourceComponent(
      IVssRequestContext requestContext)
    {
      return (IEnvironmentResourceComponent<VirtualMachineGroup>) requestContext.CreateComponent<VirtualMachineGroupComponent>();
    }

    protected override VirtualMachineGroup AddResourceEntry(
      IVssRequestContext requestContext,
      Guid projectId,
      VirtualMachineGroup resource)
    {
      using (new MethodScope(requestContext, this.c_layer, nameof (AddResourceEntry)))
      {
        this.EnsureDeploymentPool(requestContext, projectId, resource);
        return base.AddResourceEntry(requestContext, projectId, resource);
      }
    }

    protected override void DeleteResourceEntry(
      IVssRequestContext requestContext,
      Guid projectId,
      int environmentId,
      int resourceId,
      Guid deletedBy)
    {
      IDistributedTaskResourceService service = requestContext.GetService<IDistributedTaskResourceService>();
      VirtualMachineGroup resourceInternal = this.GetResourceInternal(requestContext, projectId, environmentId, resourceId);
      if (resourceInternal == null)
        return;
      service.DeleteAgentPool(requestContext.Elevate(), resourceInternal.PoolId);
      base.DeleteResourceEntry(requestContext, projectId, environmentId, resourceId, deletedBy);
    }

    protected override void DeleteResourceEntries(
      IVssRequestContext requestContext,
      Guid projectId,
      int environmentId,
      Guid deletedBy)
    {
      IDistributedTaskResourceService service = requestContext.GetService<IDistributedTaskResourceService>();
      List<VirtualMachineGroup> resourcesInternal = this.GetResourcesInternal(requestContext, projectId, environmentId);
      if (resourcesInternal.IsNullOrEmpty<VirtualMachineGroup>())
        return;
      foreach (VirtualMachineGroup virtualMachineGroup in resourcesInternal)
        service.DeleteAgentPool(requestContext.Elevate(), virtualMachineGroup.PoolId);
      base.DeleteResourceEntries(requestContext, projectId, environmentId, deletedBy);
    }

    protected override async Task AuthorizePipelineResourceAsync(
      IVssRequestContext requestContext,
      Guid projectId,
      VirtualMachineGroup resource)
    {
      await requestContext.GetService<IPipelineResourceAuthorizationProxyService>().InheritAuthorizationPolicyFromEnvironmentAsync(requestContext, projectId, resource.EnvironmentReference.Id, resource.PoolId.ToString(), typeof (TaskAgentPool).ToString());
    }

    private List<VirtualMachineGroup> GetResourcesInternal(
      IVssRequestContext requestContext,
      Guid projectId,
      int environmentId)
    {
      using (VirtualMachineGroupComponent component = requestContext.CreateComponent<VirtualMachineGroupComponent>())
        return component.GetEnvironmentResources(projectId, environmentId);
    }

    private void EnsureDeploymentPool(
      IVssRequestContext requestContext,
      Guid projectId,
      VirtualMachineGroup resource)
    {
      IDistributedTaskResourceService service = requestContext.GetService<IDistributedTaskResourceService>();
      IVssRequestContext elevatedRequestContext = requestContext.Elevate();
      Microsoft.VisualStudio.Services.Identity.Identity identity1 = this.m_getCurrentUserIdentity(requestContext);
      string poolName = this.GeneratePoolName(elevatedRequestContext, projectId, resource);
      IVssRequestContext requestContext1 = elevatedRequestContext;
      TaskAgentPool pool = new TaskAgentPool();
      pool.Name = poolName;
      pool.PoolType = TaskAgentPoolType.Deployment;
      pool.AutoProvision = new bool?(false);
      pool.CreatedBy = identity1.ToIdentityRef(requestContext);
      TaskAgentPool taskAgentPool = service.AddAgentPool(requestContext1, pool);
      if (taskAgentPool == null)
        return;
      Microsoft.VisualStudio.Services.Identity.Identity identity2 = requestContext.GetService<IdentityService>().GetGroups(requestContext, projectId, (IList<string>) new string[1]
      {
        TaskResources.ProjectContributorsGroupName()
      }).FirstOrDefault<Microsoft.VisualStudio.Services.Identity.Identity>();
      if (identity2 != null)
        this.GetDeploymentPoolSecurity(requestContext).GrantReadPermissionToPool(requestContext.ToPoolRequestContext(), taskAgentPool.Id, identity2);
      else
        requestContext.TraceError(10015055, "ResourceService", "Couldn't find contributors group on the project: {0}. Could not grant read permissions on deployment pool: {1}", (object) projectId, (object) taskAgentPool.Id);
      this.GetDeploymentPoolSecurity(requestContext).GrantAdministratorPermissionToPool(requestContext, taskAgentPool.Id, identity1);
      resource.PoolId = taskAgentPool.Id;
    }

    private string GeneratePoolName(
      IVssRequestContext elevatedRequestContext,
      Guid projectId,
      VirtualMachineGroup resource)
    {
      TaskAgentPool taskAgentPool = (TaskAgentPool) null;
      IDistributedTaskResourceService service = elevatedRequestContext.GetService<IDistributedTaskResourceService>();
      string poolName = this.SanitizePoolName(this.GetAutoProvisionedPoolName(elevatedRequestContext, projectId, resource.Name));
      if (!poolName.IsNullOrEmpty<char>())
        taskAgentPool = service.GetAgentPools(elevatedRequestContext, poolName, poolType: TaskAgentPoolType.Deployment).FirstOrDefault<TaskAgentPool>();
      if (poolName.IsNullOrEmpty<char>() || taskAgentPool != null)
        poolName = this.SanitizePoolName(resource.Name + "-" + Guid.NewGuid().ToString());
      return poolName;
    }

    private string SanitizePoolName(string poolName)
    {
      int num = 0;
      while (num < poolName.Length && poolName[num].Equals('-'))
        ++num;
      return num >= poolName.Length ? string.Empty : poolName.Substring(num);
    }

    private IAgentPoolSecurityProvider GetDeploymentPoolSecurity(IVssRequestContext requestContext) => requestContext.GetService<DistributedTaskResourceService>().GetAgentPoolSecurity(requestContext, TaskAgentPoolType.Deployment);

    private string GetAutoProvisionedPoolName(
      IVssRequestContext requestContext,
      Guid projectId,
      string resourceName)
    {
      string poolName = requestContext.GetService<IProjectService>().GetProjectName(requestContext, projectId) + "-" + resourceName;
      try
      {
        ArgumentValidation.CheckPoolName(ref poolName, "pool name", false);
      }
      catch (ArgumentException ex)
      {
        return resourceName;
      }
      return poolName;
    }

    private static void TrimResponse(IList<VirtualMachine> virtualMachines)
    {
      foreach (VirtualMachine virtualMachine in (IEnumerable<VirtualMachine>) virtualMachines)
        virtualMachine.Agent.Authorization = (TaskAgentAuthorization) null;
    }

    protected override string c_layer => nameof (VirtualMachineGroupService);
  }
}
