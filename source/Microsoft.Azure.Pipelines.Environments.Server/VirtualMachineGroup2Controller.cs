// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Environments.Server.Controllers.VirtualMachineGroup2Controller
// Assembly: Microsoft.Azure.Pipelines.Environments.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4A0C9A0D-816B-442F-8D21-CE0F4EA438AE
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.Environments.Server.dll

using Microsoft.TeamFoundation.DistributedTask.Server;
using Microsoft.TeamFoundation.DistributedTask.Server.Converters;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System.Threading.Tasks;
using System.Web.Http;

namespace Microsoft.Azure.Pipelines.Environments.Server.Controllers
{
  [ControllerApiVersion(5.2)]
  [ClientInternalUseOnly(false)]
  [VersionedApiControllerCustomName(Area = "environments", ResourceName = "virtualmachinegroups")]
  public sealed class VirtualMachineGroup2Controller : EnvironmentsProjectApiController
  {
    [HttpPost]
    public async Task<VirtualMachineGroup> AddVirtualMachineGroup(
      int environmentId,
      [FromBody] VirtualMachineGroupCreateParameters createParameters)
    {
      VirtualMachineGroup2Controller group2Controller = this;
      return await group2Controller.TfsRequestContext.GetService<IVirtualMachineGroupService>().AddEnvironmentResourceAsync(group2Controller.TfsRequestContext, group2Controller.ProjectId, createParameters.ToResource(environmentId));
    }

    [HttpGet]
    public VirtualMachineGroup GetVirtualMachineGroup(int environmentId, int resourceId) => this.TfsRequestContext.GetService<IVirtualMachineGroupService>().GetEnvironmentResource(this.TfsRequestContext, this.ProjectId, environmentId, resourceId);

    [HttpPatch]
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
    public void DeleteVirtualMachineGroup(int environmentId, int resourceId) => this.TfsRequestContext.GetService<IVirtualMachineGroupService>().DeleteEnvironmentResource(this.TfsRequestContext, this.ProjectId, environmentId, resourceId);
  }
}
