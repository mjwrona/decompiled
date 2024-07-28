// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.Controllers.DeploymentMachinesController
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;

namespace Microsoft.TeamFoundation.DistributedTask.Server.Controllers
{
  [ControllerApiVersion(3.2)]
  [VersionedApiControllerCustomName(Area = "distributedtask", ResourceName = "machines")]
  public sealed class DeploymentMachinesController : DeploymentMachineGroupMachinesController
  {
    [HttpGet]
    [ClientLocationId("6F6D406F-CFE6-409C-9327-7009928077E7")]
    [ClientResponseType(typeof (IList<DeploymentMachine>), null, null)]
    public Task<IList<DeploymentMachine>> GetDeploymentMachines(
      int deploymentGroupId,
      [ClientParameterAsIEnumerable(typeof (string), ',')] string tags = null,
      [ClientQueryParameter] string name = null,
      [FromUri(Name = "$expand")] DeploymentMachineExpands expands = DeploymentMachineExpands.None)
    {
      throw new RESTEndpointNotSupportedException(TaskResources.GetDeploymentMachinesApiDeprecated());
    }

    [HttpPatch]
    [ClientLocationId("6F6D406F-CFE6-409C-9327-7009928077E7")]
    public IList<DeploymentMachine> UpdateDeploymentMachines(
      int deploymentGroupId,
      IList<DeploymentMachine> machines)
    {
      throw new RESTEndpointNotSupportedException(TaskResources.UpdateDeploymentMachinesApiDeprecated());
    }

    [HttpPost]
    [ClientLocationId("6F6D406F-CFE6-409C-9327-7009928077E7")]
    public DeploymentMachine AddDeploymentMachine(int deploymentGroupId, DeploymentMachine machine) => throw new RESTEndpointNotSupportedException(TaskResources.AddDeploymentMachineApiDeprecated());

    [HttpDelete]
    [ClientLocationId("6F6D406F-CFE6-409C-9327-7009928077E7")]
    public void DeleteDeploymentMachine(int deploymentGroupId, int machineId) => throw new RESTEndpointNotSupportedException(TaskResources.DeleteDeploymentMachineApiDeprecated());

    [HttpGet]
    [ClientLocationId("6F6D406F-CFE6-409C-9327-7009928077E7")]
    public DeploymentMachine GetDeploymentMachine(
      int deploymentGroupId,
      int machineId,
      [FromUri(Name = "$expand")] DeploymentMachineExpands expands = DeploymentMachineExpands.None)
    {
      throw new RESTEndpointNotSupportedException(TaskResources.GetDeploymentMachineApiDeprecated());
    }

    [HttpPut]
    [ClientLocationId("6F6D406F-CFE6-409C-9327-7009928077E7")]
    public DeploymentMachine ReplaceDeploymentMachine(
      int deploymentGroupId,
      int machineId,
      DeploymentMachine machine)
    {
      throw new RESTEndpointNotSupportedException(TaskResources.ReplaceDeploymentMachineApiDeprecated());
    }

    [HttpPatch]
    [ClientLocationId("6F6D406F-CFE6-409C-9327-7009928077E7")]
    public DeploymentMachine UpdateDeploymentMachine(
      int deploymentGroupId,
      int machineId,
      DeploymentMachine machine)
    {
      throw new RESTEndpointNotSupportedException(TaskResources.UpdateDeploymentMachineApiDeprecated());
    }
  }
}
