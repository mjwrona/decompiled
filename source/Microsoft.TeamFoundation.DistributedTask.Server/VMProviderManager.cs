// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.VMProviderManager
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.DistributedTask.Server
{
  public sealed class VMProviderManager : IVMProviderManager
  {
    private readonly IVssRequestContext requestContext;

    public VMProviderManager(IVssRequestContext requestContext) => this.requestContext = requestContext;

    public Task<List<VirtualMachine>> GetVirtualMachines(
      Guid projectId,
      int environmentId,
      int resourceId)
    {
      VirtualMachineGroup environmentResource = this.requestContext.GetService<IVirtualMachineGroupService>().GetEnvironmentResource(this.requestContext, projectId, environmentId, resourceId);
      return Task.FromResult<List<VirtualMachine>>(this.requestContext.GetService<IDistributedTaskResourceService>().GetAgents(this.requestContext, environmentResource.PoolId).Select<TaskAgent, VirtualMachine>((Func<TaskAgent, VirtualMachine>) (a => new VirtualMachine()
      {
        Id = a.Id,
        Agent = a
      })).ToList<VirtualMachine>());
    }

    public Task<VirtualMachineGroup> GetVirtualMachineGroup(
      Guid projectId,
      int environmentId,
      int resourceId)
    {
      return Task.FromResult<VirtualMachineGroup>(this.requestContext.GetService<IVirtualMachineGroupService>().GetEnvironmentResource(this.requestContext, projectId, environmentId, resourceId));
    }
  }
}
