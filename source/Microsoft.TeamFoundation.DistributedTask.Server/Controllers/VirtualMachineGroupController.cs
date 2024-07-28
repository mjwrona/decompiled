// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.Controllers.VirtualMachineGroupController
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.Server.Converters;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Threading.Tasks;
using System.Web.Http;

namespace Microsoft.TeamFoundation.DistributedTask.Server.Controllers
{
  [ControllerApiVersion(5.0)]
  [VersionedApiControllerCustomName(Area = "distributedtask", ResourceName = "virtualmachinegroups")]
  [ClientInternalUseOnly(true)]
  public sealed class VirtualMachineGroupController : DistributedTaskProjectApiController
  {
    [HttpPost]
    [Obsolete]
    public async Task<VirtualMachineGroup> AddVirtualMachineGroup(
      int environmentId,
      [FromBody] VirtualMachineGroupCreateParameters createParameters)
    {
      VirtualMachineGroupController machineGroupController = this;
      return await machineGroupController.TfsRequestContext.GetService<IVirtualMachineGroupService>().AddEnvironmentResourceAsync(machineGroupController.TfsRequestContext, machineGroupController.ProjectId, createParameters.ToResource(environmentId));
    }

    [HttpGet]
    [Obsolete]
    public VirtualMachineGroup GetVirtualMachineGroup(int environmentId, int resourceId) => this.TfsRequestContext.GetService<IVirtualMachineGroupService>().GetEnvironmentResource(this.TfsRequestContext, this.ProjectId, environmentId, resourceId);

    [HttpPatch]
    [Obsolete]
    public VirtualMachineGroup UpdateVirtualMachineGroup(
      int environmentId,
      [FromBody] VirtualMachineGroup resource)
    {
      resource.EnvironmentReference = new EnvironmentReference()
      {
        Id = environmentId
      };
      return this.TfsRequestContext.GetService<IVirtualMachineGroupService>().UpdateEnvironmentResource(this.TfsRequestContext, this.ProjectId, resource);
    }

    [HttpDelete]
    [Obsolete]
    public void DeleteVirtualMachineGroup(int environmentId, int resourceId) => this.TfsRequestContext.GetService<IVirtualMachineGroupService>().DeleteEnvironmentResource(this.TfsRequestContext, this.ProjectId, environmentId, resourceId);
  }
}
