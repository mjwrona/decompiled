// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.VirtualMachineResourceService
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.DistributedTask.Server.DataAccess;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.Threading;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.DistributedTask.Server
{
  internal class VirtualMachineResourceService : 
    EnvironmentResourceService<VirtualMachineResource>,
    IVirtualMachineResourceService,
    IEnvironmentResourceService<VirtualMachineResource>,
    IVssFrameworkService
  {
    public VirtualMachineResourceService()
      : this((Func<IVssRequestContext, Microsoft.VisualStudio.Services.Identity.Identity>) (rc => rc.GetUserIdentity()), (Func<IVssRequestContext, Microsoft.VisualStudio.Services.Identity.Identity, IdentityRef>) ((rc, id) => id.ToIdentityRef(rc)))
    {
    }

    protected VirtualMachineResourceService(
      Func<IVssRequestContext, Microsoft.VisualStudio.Services.Identity.Identity> getCurrentUserIdentity,
      Func<IVssRequestContext, Microsoft.VisualStudio.Services.Identity.Identity, IdentityRef> toIdentityRef)
      : base(getCurrentUserIdentity, toIdentityRef)
    {
    }

    public Task<IPagedList<VirtualMachineResource>> GetVirtualMachinesPagedAsync(
      IVssRequestContext requestContext,
      Guid projectId,
      int environmentId,
      string continuationToken = null,
      int top = 1000,
      string name = null,
      IList<string> tagFilters = null)
    {
      using (new MethodScope(requestContext, nameof (VirtualMachineResourceService), nameof (GetVirtualMachinesPagedAsync)))
      {
        string continuationToken1 = (string) null;
        if (!this.m_securityProvider.HasViewPermissions(requestContext, projectId, environmentId))
          return Task.FromResult<IPagedList<VirtualMachineResource>>((IPagedList<VirtualMachineResource>) new PagedList<VirtualMachineResource>((IEnumerable<VirtualMachineResource>) Array.Empty<VirtualMachineResource>(), continuationToken1));
        ArgumentValidation.CheckDeploymentTargetTags(tagFilters);
        IList<VirtualMachineResource> virtualMachineResourceList = (IList<VirtualMachineResource>) null;
        using (VirtualMachineResourceComponent component = requestContext.CreateComponent<VirtualMachineResourceComponent>())
          virtualMachineResourceList = (IList<VirtualMachineResource>) component.GetEnvironmentResources(projectId, environmentId, name, tagFilters, top + 1, continuationToken).ToList<VirtualMachineResource>();
        if (virtualMachineResourceList.IsNullOrEmpty<VirtualMachineResource>())
          return Task.FromResult<IPagedList<VirtualMachineResource>>((IPagedList<VirtualMachineResource>) new PagedList<VirtualMachineResource>((IEnumerable<VirtualMachineResource>) Array.Empty<VirtualMachineResource>(), continuationToken1));
        List<VirtualMachineResource> list1 = virtualMachineResourceList.ToList<VirtualMachineResource>();
        list1.Sort((Comparison<VirtualMachineResource>) ((vm1, vm2) => string.Compare(vm1.Name, vm2.Name, true)));
        if (list1.Count > top)
        {
          continuationToken1 = list1[top].Name;
          list1 = list1.Take<VirtualMachineResource>(top).ToList<VirtualMachineResource>();
        }
        List<int> list2 = virtualMachineResourceList.Select<VirtualMachineResource, int>((Func<VirtualMachineResource, int>) (machine => machine.Agent.Id)).ToList<int>();
        TaskAgentPoolReference environmentPool = requestContext.GetService<IEnvironmentService>().GetEnvironmentPool(requestContext, projectId, environmentId);
        if (environmentPool != null)
        {
          IList<TaskAgent> agents = requestContext.GetService<IDistributedTaskResourceService>().GetAgents(requestContext, environmentPool.Id, (IEnumerable<int>) list2);
          list1 = VirtualMachineResourceHelper.MapAgentsToVirtualMachines((IEnumerable<VirtualMachineResource>) list1, (IEnumerable<TaskAgent>) agents).ToList<VirtualMachineResource>();
        }
        return Task.FromResult<IPagedList<VirtualMachineResource>>((IPagedList<VirtualMachineResource>) new PagedList<VirtualMachineResource>((IEnumerable<VirtualMachineResource>) list1, continuationToken1));
      }
    }

    public void DeleteVirtualMachineResource(
      IVssRequestContext requestContext,
      Guid projectId,
      int environmentId,
      int resourceId)
    {
      using (new MethodScope(requestContext, nameof (VirtualMachineResourceService), nameof (DeleteVirtualMachineResource)))
      {
        this.m_securityProvider.CheckManagePermissions(requestContext, projectId, environmentId);
        VirtualMachineResource resourceInternal;
        try
        {
          resourceInternal = this.GetResourceInternal(requestContext, projectId, environmentId, resourceId);
        }
        catch (EnvironmentResourceNotFoundException ex)
        {
          requestContext.TraceError(10015190, nameof (VirtualMachineResourceService), "Resource Id: {0} in Environment Id: {0} not found", (object) environmentId, (object) resourceId, (object) ex.Message);
          return;
        }
        if (resourceInternal == null)
          return;
        TaskAgentPoolReference environmentPool = requestContext.GetService<IEnvironmentService>().GetEnvironmentPool(requestContext, projectId, environmentId);
        if (environmentPool != null)
          requestContext.GetService<IDistributedTaskResourceService>().DeleteAgents(requestContext, environmentPool.Id, (IEnumerable<int>) new int[1]
          {
            resourceInternal.Agent.Id
          });
        requestContext.TraceInfo(10015190, "EnvironmentService", "Delete virtual machine resource with Id: {0}.", (object) resourceInternal.Id);
        this.DeleteEnvironmentResource(requestContext, projectId, environmentId, resourceId);
      }
    }

    public VirtualMachineResource UpdateVirtualMachineResource(
      IVssRequestContext requestContext,
      Guid projectId,
      int environmentId,
      VirtualMachineResource resource,
      TaskAgentCapabilityType capabilityUpdate = TaskAgentCapabilityType.System)
    {
      using (new MethodScope(requestContext, nameof (VirtualMachineResourceService), nameof (UpdateVirtualMachineResource)))
      {
        ArgumentValidation.CheckVirtualMachineResource(resource, "virtual machine resource", false);
        this.m_securityProvider.CheckManagePermissions(requestContext, projectId, environmentId);
        requestContext.TraceInfo(10015220, "EnvironmentService", "Update virtual machine resource with Id: {0}.", (object) resource.Id);
        if (this.GetResourceInternal(requestContext, projectId, environmentId, resource.Id).Agent.Id != resource.Agent.Id)
          throw new InvalidVirtualMachineResourceException(TaskResources.VirtualMachineResourceAgentMismatch());
        TaskAgentPoolReference environmentPool = requestContext.GetService<IEnvironmentService>().GetEnvironmentPool(requestContext, projectId, environmentId);
        if (environmentPool == null || environmentPool.Id < 1)
          throw new VirtualMachineResourceLinkedPoolNullException(TaskResources.VirtualMachineResourceLinkedPoolNull());
        TaskAgent taskAgent = requestContext.GetService<IDistributedTaskResourceService>().UpdateAgent(requestContext, environmentPool.Id, resource.Agent, capabilityUpdate);
        VirtualMachineResource virtualMachineResource = this.UpdateEnvironmentResource(requestContext, projectId, resource);
        virtualMachineResource.Agent = taskAgent;
        return virtualMachineResource;
      }
    }

    protected override VirtualMachineResource AddResourceEntry(
      IVssRequestContext requestContext,
      Guid projectId,
      VirtualMachineResource resource)
    {
      using (new MethodScope(requestContext, nameof (VirtualMachineResourceService), nameof (AddResourceEntry)))
      {
        TaskAgentPoolReference agentPoolReference = this.EnsureEnvironmentPool(requestContext, projectId, resource.EnvironmentReference.Id);
        TaskAgent taskAgent = requestContext.GetService<IDistributedTaskResourceService>().AddAgent(requestContext, agentPoolReference.Id, resource.Agent);
        resource.Agent = taskAgent;
        VirtualMachineResource virtualMachineResource = base.AddResourceEntry(requestContext, projectId, resource);
        virtualMachineResource.Agent = taskAgent;
        requestContext.TraceInfo(10015219, "EnvironmentService", "Update virtual machine resource with Id: {0}.", (object) virtualMachineResource.Id);
        return virtualMachineResource;
      }
    }

    private TaskAgentPoolReference EnsureEnvironmentPool(
      IVssRequestContext requestContext,
      Guid projectId,
      int environmentId)
    {
      using (new MethodScope(requestContext, nameof (VirtualMachineResourceService), nameof (EnsureEnvironmentPool)))
      {
        IEnvironmentService environmentService = requestContext.GetService<IEnvironmentService>();
        TaskAgentPoolReference agentPoolReference;
        try
        {
          agentPoolReference = environmentService.GetEnvironmentPool(requestContext, projectId, environmentId);
        }
        catch (EnvironmentPoolNotFoundException ex)
        {
          requestContext.TraceError(10015189, nameof (VirtualMachineResourceService), "Environment '{0}' is not configured with a pool. Error Message: '{1}'. Configuring a new pool.", (object) environmentId, (object) ex.Message);
          agentPoolReference = requestContext.RunSynchronously<TaskAgentPoolReference>((Func<Task<TaskAgentPoolReference>>) (async () => await environmentService.ProvisionEnvironmentPoolAsync(requestContext, projectId, environmentId)));
        }
        return agentPoolReference != null && agentPoolReference.Id >= 1 ? agentPoolReference : throw new VirtualMachineResourceLinkedPoolNullException(TaskResources.VirtualMachineResourceLinkedPoolNull());
      }
    }

    protected override void DeleteResourceEntries(
      IVssRequestContext requestContext,
      Guid projectId,
      int environmentId,
      Guid deletedBy)
    {
      try
      {
        TaskAgentPoolReference environmentPool = requestContext.GetService<IEnvironmentService>().GetEnvironmentPool(requestContext, projectId, environmentId);
        if (environmentPool == null)
          return;
        IDistributedTaskResourceService service = requestContext.GetService<IDistributedTaskResourceService>();
        IList<TaskAgent> agents = service.GetAgents(requestContext, environmentPool.Id);
        List<int> list = agents != null ? agents.Select<TaskAgent, int>((Func<TaskAgent, int>) (taskAgent => taskAgent.Id)).ToList<int>() : (List<int>) null;
        service.DeleteAgents(requestContext, environmentPool.Id, (IEnumerable<int>) list);
        base.DeleteResourceEntries(requestContext, projectId, environmentId, deletedBy);
      }
      catch (EnvironmentPoolNotFoundException ex)
      {
      }
    }

    protected override IEnvironmentResourceComponent<VirtualMachineResource> GetResourceComponent(
      IVssRequestContext requestContext)
    {
      return (IEnvironmentResourceComponent<VirtualMachineResource>) requestContext.CreateComponent<VirtualMachineResourceComponent>();
    }

    protected override void ValidateCreateInputParameters(
      IVssRequestContext requestContext,
      Guid projectId,
      VirtualMachineResource resource)
    {
      ArgumentValidation.CheckVirtualMachineResource(resource, "virtual machine resource");
    }

    protected override string c_layer => nameof (VirtualMachineResourceService);
  }
}
