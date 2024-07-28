// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.Controllers.DeploymentMachineGroupMachinesController
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;

namespace Microsoft.TeamFoundation.DistributedTask.Server.Controllers
{
  [ControllerApiVersion(3.1)]
  [VersionedApiControllerCustomName(Area = "distributedtask", ResourceName = "machines")]
  [ClientInternalUseOnly(false)]
  public class DeploymentMachineGroupMachinesController : DeploymentGroupApiController
  {
    [HttpGet]
    [ClientLocationId("966C3874-C347-4B18-A90C-D509116717FD")]
    [ClientResponseType(typeof (IList<DeploymentMachine>), null, null)]
    public Task<IList<DeploymentMachine>> GetDeploymentMachineGroupMachines(
      int machineGroupId,
      [ClientParameterAsIEnumerable(typeof (string), ',')] string tagFilters = null)
    {
      IDeploymentGroupService deploymentGroupService = this.DeploymentGroupService;
      IVssRequestContext tfsRequestContext = this.TfsRequestContext;
      Guid projectId = this.ProjectId;
      int machineGroupId1 = machineGroupId;
      List<string> tagFilters1;
      if (tagFilters == null)
        tagFilters1 = (List<string>) null;
      else
        tagFilters1 = ((IEnumerable<string>) tagFilters.Split(new char[1]
        {
          ','
        }, StringSplitOptions.RemoveEmptyEntries)).ToList<string>();
      return deploymentGroupService.GetDeploymentMachinesAsync(tfsRequestContext, projectId, machineGroupId1, (IList<string>) tagFilters1);
    }

    [HttpPatch]
    [ClientLocationId("966C3874-C347-4B18-A90C-D509116717FD")]
    public IList<DeploymentMachine> UpdateDeploymentMachineGroupMachines(
      int machineGroupId,
      IList<DeploymentMachine> deploymentMachines)
    {
      return this.DeploymentGroupService.UpdateDeploymentMachines(this.TfsRequestContext, this.ProjectId, machineGroupId, deploymentMachines);
    }
  }
}
