// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.Controllers.LegacyServiceEndpointProxyControllerBase
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server;
using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Net;
using System.Web.Http;

namespace Microsoft.TeamFoundation.DistributedTask.Server.Controllers
{
  [ClientInternalUseOnly(false)]
  public abstract class LegacyServiceEndpointProxyControllerBase : 
    DistributedTaskProjectApiController
  {
    [Obsolete("Use ExecuteServiceEndpointRequest API Instead")]
    [HttpPost]
    public virtual IList<string> QueryServiceEndpoint(DataSourceBinding binding) => this.TfsRequestContext.GetService<IServiceEndpointProxyService2>().QueryServiceEndpoint(this.TfsRequestContext, this.ProjectId, binding.ToDataSourceBinding());

    [Obsolete("ServiceEndpoint APIs under distributedtask area is deprecated. Use the APIs under serviceendpoint area instead.")]
    [HttpPost]
    public virtual ServiceEndpointRequestResult ExecuteServiceEndpointRequest(
      string endpointId,
      [FromBody] ServiceEndpointRequest serviceEndpointRequest)
    {
      return this.TfsRequestContext.GetService<IServiceEndpointProxyService2>().ExecuteServiceEndpointRequest(this.TfsRequestContext, this.ProjectId, endpointId, serviceEndpointRequest.ToServiceEndpointRequest()).ToLegacyServiceEndpointRequestResult();
    }

    protected override void InitializeExceptionMap(ApiExceptionMapping exceptionMap)
    {
      base.InitializeExceptionMap(exceptionMap);
      exceptionMap.AddStatusCode<EndpointNotFoundException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<ServiceEndpointUntrustedHostException>(HttpStatusCode.Forbidden);
    }

    public override string ActivityLogArea => "ServiceEndpoint";
  }
}
