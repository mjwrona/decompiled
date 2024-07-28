// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.Controllers.DeploymentMachineJobRequestsController
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System.Collections.Generic;
using System.Web.Http;

namespace Microsoft.TeamFoundation.DistributedTask.Server.Controllers
{
  [ControllerApiVersion(3.2)]
  [VersionedApiControllerCustomName(Area = "distributedtask", ResourceName = "deploymentmachinejobrequests")]
  [ClientInternalUseOnly(false)]
  public sealed class DeploymentMachineJobRequestsController : DeploymentGroupApiController
  {
    [HttpGet]
    public IList<TaskAgentJobRequest> GetAgentRequestsForDeploymentMachine(
      int deploymentGroupId,
      [ClientQueryParameter] int machineId,
      [ClientQueryParameter] int completedRequestCount = 50)
    {
      throw new RESTEndpointNotSupportedException(TaskResources.GetAgentRequestsForDeploymentMachineApiDeprecated());
    }

    [HttpGet]
    public IList<TaskAgentJobRequest> GetAgentRequestsForDeploymentMachines(
      int deploymentGroupId,
      [ClientParameterAsIEnumerable(typeof (int), ',')] string machineIds = null,
      [ClientQueryParameter] int completedRequestCount = 50)
    {
      throw new RESTEndpointNotSupportedException(TaskResources.GetAgentRequestsForDeploymentMachinesApiDeprecated());
    }
  }
}
