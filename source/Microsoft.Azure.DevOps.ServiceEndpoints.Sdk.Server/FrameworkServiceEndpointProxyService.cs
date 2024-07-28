// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.FrameworkServiceEndpointProxyService
// Assembly: Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 002C83BC-B53E-470A-8038-76E47B5E5BF3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.dll

using Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.WebApiProxy;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;

namespace Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server
{
  internal class FrameworkServiceEndpointProxyService : 
    IServiceEndpointProxyService2,
    IVssFrameworkService
  {
    private const string c_layer = "FrameworkServiceEndpointProxyService";

    public IList<string> QueryServiceEndpoint(
      IVssRequestContext requestContext,
      Guid scopeIdentifier,
      DataSourceBinding binding,
      HttpRequestProxy proxy = null)
    {
      using (new MethodScope(requestContext, nameof (FrameworkServiceEndpointProxyService), nameof (QueryServiceEndpoint)))
        return (IList<string>) requestContext.GetClient<ServiceEndpointHttpClient>().QueryServiceEndpointAsync(scopeIdentifier, binding).SyncResult<List<string>>();
    }

    public ServiceEndpointRequestResult ExecuteServiceEndpointRequest(
      IVssRequestContext requestContext,
      Guid scopeIdentifier,
      string endpointId,
      ServiceEndpointRequest serviceEndpointRequest,
      HttpRequestProxy proxy = null)
    {
      using (new MethodScope(requestContext, nameof (FrameworkServiceEndpointProxyService), nameof (ExecuteServiceEndpointRequest)))
        return requestContext.GetClient<ServiceEndpointHttpClient>().ExecuteServiceEndpointRequestAsync(scopeIdentifier, endpointId, serviceEndpointRequest).SyncResult<ServiceEndpointRequestResult>();
    }

    void IVssFrameworkService.ServiceStart(IVssRequestContext requestContext)
    {
    }

    void IVssFrameworkService.ServiceEnd(IVssRequestContext requestContext)
    {
    }
  }
}
