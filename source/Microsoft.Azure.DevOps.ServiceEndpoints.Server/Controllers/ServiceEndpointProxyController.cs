// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.DevOps.ServiceEndpoints.Server.Controllers.ServiceEndpointProxyController
// Assembly: Microsoft.Azure.DevOps.ServiceEndpoints.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B7D66E3F-07ED-4CF3-859D-36958D465656
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DevOps.ServiceEndpoints.Server.dll

using Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.WebApiProxy;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi;
using System;
using System.Collections.Generic;
using System.Net;
using System.ServiceModel;
using System.Web.Http;

namespace Microsoft.Azure.DevOps.ServiceEndpoints.Server.Controllers
{
  [ControllerApiVersion(4.1)]
  [VersionedApiControllerCustomName(Area = "serviceendpoint", ResourceName = "endpointproxy")]
  public class ServiceEndpointProxyController : ServiceEndpointsProjectApiController
  {
    [Obsolete("Use ExecuteServiceEndpointRequest API Instead")]
    [HttpPost]
    [ClientLocationId("CC63BB57-2A5F-4A7A-B79C-C142D308657E")]
    public IList<string> QueryServiceEndpoint(DataSourceBinding binding) => this.TfsRequestContext.GetService<PlatformServiceEndpointProxyService>().QueryServiceEndpoint(this.TfsRequestContext, this.ProjectId, binding, (HttpRequestProxy) null);

    [HttpPost]
    [ClientExample("POST__ExecuteServiceEndpointRequestAsync.json", "Execute service endpoint request", null, null)]
    [ClientLocationId("CC63BB57-2A5F-4A7A-B79C-C142D308657E")]
    public ServiceEndpointRequestResult ExecuteServiceEndpointRequest(
      string endpointId,
      [FromBody] ServiceEndpointRequest serviceEndpointRequest)
    {
      return this.TfsRequestContext.GetService<PlatformServiceEndpointProxyService>().ExecuteServiceEndpointRequest(this.TfsRequestContext, this.ProjectId, endpointId, serviceEndpointRequest, (HttpRequestProxy) null);
    }

    protected override void InitializeExceptionMap(ApiExceptionMapping exceptionMap)
    {
      base.InitializeExceptionMap(exceptionMap);
      exceptionMap.AddStatusCode<EndpointNotFoundException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<ServiceEndpointUntrustedHostException>(HttpStatusCode.Forbidden);
    }

    public override string ActivityLogArea => "ServiceEndpoints";
  }
}
