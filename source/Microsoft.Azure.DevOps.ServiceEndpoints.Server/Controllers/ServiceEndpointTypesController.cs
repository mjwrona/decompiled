// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.DevOps.ServiceEndpoints.Server.Controllers.ServiceEndpointTypesController
// Assembly: Microsoft.Azure.DevOps.ServiceEndpoints.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B7D66E3F-07ED-4CF3-859D-36958D465656
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DevOps.ServiceEndpoints.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi;
using System.Collections.Generic;
using System.Web.Http;

namespace Microsoft.Azure.DevOps.ServiceEndpoints.Server.Controllers
{
  [ControllerApiVersion(4.1)]
  [VersionedApiControllerCustomName(Area = "serviceendpoint", ResourceName = "types")]
  public class ServiceEndpointTypesController : ServiceEndpointsApiController
  {
    [HttpGet]
    [ClientExample("GET__ServiceEndpointTypes.json", "Get service endpoint types", null, null)]
    public IEnumerable<ServiceEndpointType> GetServiceEndpointTypes(string type = null, string scheme = null) => this.TfsRequestContext.GetService<PlatformServiceEndpointTypesService>().GetServiceEndpointTypes(this.TfsRequestContext, type, scheme);

    public override string ActivityLogArea => "ServiceEndpoints";
  }
}
