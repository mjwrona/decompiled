// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.FrameworkServiceEndpointTypesService
// Assembly: Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 002C83BC-B53E-470A-8038-76E47B5E5BF3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System.Collections.Generic;

namespace Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server
{
  internal class FrameworkServiceEndpointTypesService : 
    IServiceEndpointTypesService2,
    IVssFrameworkService
  {
    private const string c_layer = "FrameworkServiceEndpointTypesService";

    public IEnumerable<ServiceEndpointType> GetServiceEndpointTypes(
      IVssRequestContext requestContext,
      string type,
      string scheme)
    {
      using (new MethodScope(requestContext, nameof (FrameworkServiceEndpointTypesService), nameof (GetServiceEndpointTypes)))
        return (IEnumerable<ServiceEndpointType>) requestContext.GetClient<ServiceEndpointHttpClient>().GetServiceEndpointTypesAsync(type, scheme, (object) requestContext).SyncResult<List<ServiceEndpointType>>();
    }

    void IVssFrameworkService.ServiceStart(IVssRequestContext requestContext)
    {
    }

    void IVssFrameworkService.ServiceEnd(IVssRequestContext requestContext)
    {
    }
  }
}
