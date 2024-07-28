// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.DevOps.ServiceEndpoints.Server.Controllers.AzureRmManagementGroupsController
// Assembly: Microsoft.Azure.DevOps.ServiceEndpoints.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B7D66E3F-07ED-4CF3-859D-36958D465656
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DevOps.ServiceEndpoints.Server.dll

using Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System.Web.Http;

namespace Microsoft.Azure.DevOps.ServiceEndpoints.Server.Controllers
{
  [ControllerApiVersion(5.0)]
  [ClientInternalUseOnly(false)]
  [VersionedApiControllerCustomName(Area = "serviceendpoint", ResourceName = "azurermmanagementgroups")]
  public class AzureRmManagementGroupsController : ServiceEndpointsApiController
  {
    [HttpGet]
    public AzureManagementGroupQueryResult GetAzureManagementGroups() => this.TfsRequestContext.GetService<IAzureRmSubscriptionService2>().GetAzureManagementGroups(this.TfsRequestContext);

    public override string ActivityLogArea => "ServiceEndpoints";
  }
}
