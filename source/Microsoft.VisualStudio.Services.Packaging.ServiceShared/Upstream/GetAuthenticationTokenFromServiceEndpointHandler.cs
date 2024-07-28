// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Upstream.GetAuthenticationTokenFromServiceEndpointHandler
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Upstream
{
  public class GetAuthenticationTokenFromServiceEndpointHandler : 
    IHandler<UpstreamSource, VssCredentials>
  {
    private readonly IVssRequestContext requestContext;

    public GetAuthenticationTokenFromServiceEndpointHandler(IVssRequestContext requestContext) => this.requestContext = requestContext;

    public VssCredentials Handle(UpstreamSource upstreamSource)
    {
      if (!upstreamSource.ServiceEndpointId.HasValue)
        throw new InvalidOperationException("Provided upstream source does not have a service endpoint ID.");
      Guid guid = upstreamSource.ServiceEndpointId.Value;
      IVssRequestContext requestContext = this.requestContext.Elevate();
      ServiceEndpoint serviceEndpoint = this.requestContext.GetService<IServiceEndpointService2>().QueryServiceEndpoints(requestContext, upstreamSource.ServiceEndpointProjectId.Value, "artifacts-upstream", (IEnumerable<string>) null, (IEnumerable<Guid>) new List<Guid>()
      {
        guid
      }, (string) null, true, true).FirstOrDefault<ServiceEndpoint>();
      EndpointAuthorization endpointAuthorization = serviceEndpoint != null ? serviceEndpoint.Authorization : throw new NoTokenFoundFromServiceEndpointException(Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Error_NoServiceConnectionFound());
      if (endpointAuthorization == null)
        throw new NoTokenFoundFromServiceEndpointException(Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Error_NoAuthFoundInServiceConnection());
      string password = endpointAuthorization.Parameters.ContainsKey("apiToken") ? endpointAuthorization.Parameters["apiToken"] : throw new NoTokenFoundFromServiceEndpointException(Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Error_NoAuthTokenFoundInServiceConnection((object) guid.ToString()));
      if (password == null)
        throw new NoTokenFoundFromServiceEndpointException(Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Error_AuthTokenWasNull((object) guid.ToString()));
      if (serviceEndpoint.IsDisabled)
        throw new NoTokenFoundFromServiceEndpointException(Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Error_ServiceConnectionDisabled());
      return (VssCredentials) (FederatedCredential) new VssBasicCredential("userName", password);
    }
  }
}
