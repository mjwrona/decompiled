// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.Controllers.DeploymentGroupMetricsController
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.TeamFoundation.DistributedTask.Server.Controllers
{
  [ControllerApiVersion(4.0)]
  [ClientInternalUseOnly(false)]
  [VersionedApiControllerCustomName(Area = "distributedtask", ResourceName = "deploymentgroupsmetrics")]
  public sealed class DeploymentGroupMetricsController : DeploymentGroupApiController
  {
    private const int DefaultTop = 50;

    [HttpGet]
    [ClientResponseType(typeof (IPagedList<DeploymentGroupMetrics>), null, null)]
    [ClientLocationId("281C6308-427A-49E1-B83A-DAC0F4862189")]
    public HttpResponseMessage GetDeploymentGroupsMetrics(
      [ClientQueryParameter] string deploymentGroupName = null,
      [ClientQueryParameter] string continuationToken = "",
      [FromUri(Name = "$top")] int top = 50)
    {
      if (top < 0 || top > 50)
        top = 50;
      IPagedList<DeploymentGroupMetrics> deploymentGroupsMetrics = this.DeploymentGroupService.GetDeploymentGroupsMetrics(this.TfsRequestContext, this.ProjectId, deploymentGroupName, continuationToken, top);
      HttpResponseMessage response = this.Request.CreateResponse<IPagedList<DeploymentGroupMetrics>>(HttpStatusCode.OK, deploymentGroupsMetrics);
      if (deploymentGroupsMetrics.ContinuationToken != null)
        DistributedTaskProjectApiController.SetContinuationToken(response, deploymentGroupsMetrics.ContinuationToken);
      return response;
    }
  }
}
