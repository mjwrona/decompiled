// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.Controllers.DeploymentPoolsSummaryController
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.Server.Exceptions;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System.Collections.Generic;
using System.Web.Http;

namespace Microsoft.TeamFoundation.DistributedTask.Server.Controllers
{
  [ControllerApiVersion(4.1)]
  [ClientInternalUseOnly(false)]
  [VersionedApiControllerCustomName(Area = "distributedtask", ResourceName = "deploymentPoolsSummary")]
  public sealed class DeploymentPoolsSummaryController : DistributedTaskApiController
  {
    [HttpGet]
    [ClientResponseType(typeof (List<DeploymentPoolSummary>), null, null)]
    public IList<DeploymentPoolSummary> GetDeploymentPoolsSummary(
      [ClientQueryParameter] string poolName = null,
      [ClientQueryParameter] DeploymentPoolSummaryExpands expands = DeploymentPoolSummaryExpands.None,
      [ClientParameterAsIEnumerable(typeof (int), ',')] string poolIds = null)
    {
      if (!string.IsNullOrWhiteSpace(poolName) && !string.IsNullOrWhiteSpace(poolIds))
        throw new InvalidRequestException(TaskResources.GetDeploymentPoolsSummaryInvalidRequest());
      bool includeDeploymentGroupReferences = (expands & DeploymentPoolSummaryExpands.DeploymentGroups) == DeploymentPoolSummaryExpands.DeploymentGroups;
      bool includeResource = (expands & DeploymentPoolSummaryExpands.Resource) == DeploymentPoolSummaryExpands.Resource;
      IList<int> array = DistributedTaskApiControllerHelper.ParseArray(poolIds);
      return this.ResourceService.GetDeploymentPoolsSummary(this.TfsRequestContext, poolName, array, includeDeploymentGroupReferences, includeResource);
    }
  }
}
