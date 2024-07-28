// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.Controllers.DeploymentMachineGroupsController
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Microsoft.TeamFoundation.DistributedTask.Server.Controllers
{
  [ControllerApiVersion(3.1)]
  [VersionedApiControllerCustomName(Area = "distributedtask", ResourceName = "machinegroups")]
  [ClientInternalUseOnly(false)]
  public sealed class DeploymentMachineGroupsController : DeploymentGroupApiController
  {
    [HttpPost]
    public DeploymentMachineGroup AddDeploymentMachineGroup(DeploymentMachineGroup machineGroup) => DeploymentMachineGroupsController.ToDeploymentMachineGroup(this.DeploymentGroupService.AddDeploymentGroup(this.TfsRequestContext, this.ProjectId, DeploymentMachineGroupsController.ToDeploymentGroup(machineGroup)));

    [HttpDelete]
    public void DeleteDeploymentMachineGroup(int machineGroupId) => this.DeploymentGroupService.DeleteDeploymentGroup(this.TfsRequestContext, this.ProjectId, machineGroupId);

    [HttpGet]
    public DeploymentMachineGroup GetDeploymentMachineGroup(
      int machineGroupId,
      MachineGroupActionFilter actionFilter = MachineGroupActionFilter.None)
    {
      return DeploymentMachineGroupsController.ToDeploymentMachineGroup(this.DeploymentGroupService.GetDeploymentGroup(this.TfsRequestContext, this.ProjectId, machineGroupId, (DeploymentGroupActionFilter) actionFilter) ?? throw new DeploymentMachineGroupNotFoundException(TaskResources.DeploymentMachineGroupNotFound((object) machineGroupId)));
    }

    [HttpGet]
    public IList<DeploymentMachineGroup> GetDeploymentMachineGroups(
      [ClientQueryParameter] string machineGroupName = null,
      MachineGroupActionFilter actionFilter = MachineGroupActionFilter.None)
    {
      return (IList<DeploymentMachineGroup>) this.DeploymentGroupService.GetDeploymentGroups(this.TfsRequestContext, this.ProjectId, machineGroupName, (DeploymentGroupActionFilter) actionFilter, true).Select<DeploymentGroup, DeploymentMachineGroup>((Func<DeploymentGroup, DeploymentMachineGroup>) (deploymentMachineGroup => DeploymentMachineGroupsController.ToDeploymentMachineGroup(deploymentMachineGroup))).ToList<DeploymentMachineGroup>();
    }

    [HttpPatch]
    public DeploymentMachineGroup UpdateDeploymentMachineGroup(
      int machineGroupId,
      DeploymentMachineGroup machineGroup)
    {
      return DeploymentMachineGroupsController.ToDeploymentMachineGroup(this.DeploymentGroupService.UpdateDeploymentGroup(this.TfsRequestContext, this.ProjectId, machineGroupId, DeploymentMachineGroupsController.ToDeploymentGroup(machineGroup)));
    }

    private static DeploymentGroup ToDeploymentGroup(DeploymentMachineGroup machineGroup)
    {
      if (machineGroup == null)
        return (DeploymentGroup) null;
      DeploymentGroup deploymentGroup1 = new DeploymentGroup();
      deploymentGroup1.Id = machineGroup.Id;
      deploymentGroup1.Project = machineGroup.Project;
      deploymentGroup1.Name = machineGroup.Name;
      deploymentGroup1.Pool = machineGroup.Pool;
      deploymentGroup1.MachineCount = machineGroup.Size;
      DeploymentGroup deploymentGroup2 = deploymentGroup1;
      if (machineGroup.Machines.Count > 0)
        deploymentGroup2.Machines = machineGroup.Machines;
      return deploymentGroup2;
    }

    private static DeploymentMachineGroup ToDeploymentMachineGroup(DeploymentGroup deploymentGroup)
    {
      if (deploymentGroup == null)
        return (DeploymentMachineGroup) null;
      DeploymentMachineGroup deploymentMachineGroup = new DeploymentMachineGroup();
      deploymentMachineGroup.Id = deploymentGroup.Id;
      deploymentMachineGroup.Machines = deploymentGroup.Machines;
      deploymentMachineGroup.Name = deploymentGroup.Name;
      deploymentMachineGroup.Pool = deploymentGroup.Pool;
      deploymentMachineGroup.Project = deploymentGroup.Project;
      deploymentMachineGroup.Size = deploymentGroup.MachineCount;
      return deploymentMachineGroup;
    }
  }
}
