// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.AzureEndpointServicePrincipalAuthorizer
// Assembly: Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 002C83BC-B53E-470A-8038-76E47B5E5BF3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi;
using System;
using System.Net;

namespace Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server
{
  public class AzureEndpointServicePrincipalAuthorizer : AzureEndpointAuthorizer
  {
    private readonly IAuthorizationTokenProvider _tokenProvider;

    public AzureEndpointServicePrincipalAuthorizer(
      IVssRequestContext requestContext,
      ServiceEndpoint serviceEndpoint)
      : base(requestContext, serviceEndpoint)
    {
      this._tokenProvider = AzureServicePrincipalTokenProviderFactory.CreateProvider(serviceEndpoint, requestContext);
    }

    public override void AuthorizeRequest(HttpWebRequest request, string resourceUrl)
    {
      this._requestContext.TraceEnter("WebApiProxy", nameof (AuthorizeRequest));
      if (!this.ServiceEndpoint.Authorization.Scheme.Equals("ServicePrincipal", StringComparison.OrdinalIgnoreCase))
        throw new InvalidOperationException(ServiceEndpointSdkResources.NoAzureServicePrincipal());
      string token = this._tokenProvider.GetToken(request, resourceUrl);
      if (string.IsNullOrEmpty(token))
      {
        string str;
        this.ServiceEndpoint.Authorization.Parameters.TryGetValue("ServicePrincipalId", out str);
        this._requestContext.TraceWarning("WebApiProxy", "Unable to acquire JWT token for Serviceprincipal Id : " + str + ", resource url: " + resourceUrl);
        throw new InvalidAuthorizationDetailsException(ServiceEndpointSdkResources.FailedToObtainToken());
      }
      string str1 = "Bearer " + token;
      request.Headers.Add(HttpRequestHeader.Authorization, str1);
    }
  }
}
