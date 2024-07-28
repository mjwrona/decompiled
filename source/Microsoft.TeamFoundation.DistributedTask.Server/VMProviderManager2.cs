// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.VMProviderManager2
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.Pipelines;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.DistributedTask.Server
{
  public sealed class VMProviderManager2 : IVMProviderManager2
  {
    private readonly IVssRequestContext requestContext;

    public VMProviderManager2(IVssRequestContext requestContext) => this.requestContext = requestContext;

    public Task<TaskAgentPoolReference> GetEnvironmentDeploymentPool(
      Guid projectId,
      int environmentId)
    {
      return Task.FromResult<TaskAgentPoolReference>(this.requestContext.GetService<IEnvironmentService>().GetEnvironmentPool(this.requestContext, projectId, environmentId));
    }

    public async Task<IList<VirtualMachineResource>> GetVirtualMachines(
      Guid projectId,
      int environmentId,
      EnvironmentResourceFilter resourceFilter)
    {
      IEnvironmentResourceReferenceService resourceReferenceService = this.requestContext.GetService<IEnvironmentResourceReferenceService>();
      IVirtualMachineResourceService vmService = this.requestContext.GetService<IVirtualMachineResourceService>();
      List<int> resourceIds = new List<int>();
      string continuationToken = "";
      while (true)
      {
        IPagedList<EnvironmentResourceReference> resourceReferencesAsync = await resourceReferenceService.GetEnvironmentResourceReferencesAsync(this.requestContext, projectId, environmentId, resourceFilter.Id, resourceFilter.Name, resourceFilter.Type, resourceFilter.Tags, continuationToken: continuationToken);
        resourceIds.AddRange((IEnumerable<int>) resourceReferencesAsync.Select<EnvironmentResourceReference, int>((Func<EnvironmentResourceReference, int>) (x => x.Id)).ToList<int>());
        if (!string.IsNullOrEmpty(resourceReferencesAsync.ContinuationToken))
          continuationToken = resourceReferencesAsync.ContinuationToken;
        else
          break;
      }
      if (!resourceIds.Any<int>())
        return (IList<VirtualMachineResource>) new List<VirtualMachineResource>();
      IList<VirtualMachineResource> virtualMachineResources = vmService.GetEnvironmentResourcesById(this.requestContext, projectId, environmentId, (IEnumerable<int>) resourceIds);
      List<int> agentIds = virtualMachineResources.Select<VirtualMachineResource, int>((Func<VirtualMachineResource, int>) (machine => machine.Agent.Id)).ToList<int>();
      TaskAgentPoolReference environmentDeploymentPool = await this.GetEnvironmentDeploymentPool(projectId, environmentId);
      IList<TaskAgent> agents = this.requestContext.GetService<DistributedTaskResourceService>().GetAgents(this.requestContext, environmentDeploymentPool.Id, (IEnumerable<int>) agentIds, false, false, false, (IList<string>) null);
      virtualMachineResources = this.MapAgentsToVirtualMachines(virtualMachineResources, agents);
      return virtualMachineResources;
    }

    private IList<VirtualMachineResource> MapAgentsToVirtualMachines(
      IList<VirtualMachineResource> virtualMachines,
      IList<TaskAgent> agents)
    {
      IList<VirtualMachineResource> virtualMachines1 = virtualMachines;
      Dictionary<int, TaskAgent> dictionary = agents.ToDictionary<TaskAgent, int, TaskAgent>((Func<TaskAgent, int>) (x => x.Id), (Func<TaskAgent, TaskAgent>) (x => x));
      foreach (VirtualMachineResource virtualMachineResource in (IEnumerable<VirtualMachineResource>) virtualMachines1)
      {
        int id = virtualMachineResource.Agent.Id;
        TaskAgent taskAgent;
        if (dictionary.TryGetValue(id, out taskAgent))
          virtualMachineResource.Agent = taskAgent;
      }
      return virtualMachines1;
    }
  }
}
