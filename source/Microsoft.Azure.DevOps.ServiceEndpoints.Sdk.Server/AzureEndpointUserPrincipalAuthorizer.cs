// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.AzureEndpointUserPrincipalAuthorizer
// Assembly: Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 002C83BC-B53E-470A-8038-76E47B5E5BF3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi;
using System;
using System.Net;

namespace Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server
{
  public class AzureEndpointUserPrincipalAuthorizer : AzureEndpointAuthorizer
  {
    public AzureEndpointUserPrincipalAuthorizer(
      IVssRequestContext requestContext,
      ServiceEndpoint serviceEndpoint)
      : base(requestContext, serviceEndpoint)
    {
    }

    public override void AuthorizeRequest(HttpWebRequest request, string resourceUrl)
    {
      this._requestContext.TraceEnter("WebApiProxy", nameof (AuthorizeRequest));
      if (!this.ServiceEndpoint.Authorization.Scheme.Equals("ManagedServiceIdentity", StringComparison.OrdinalIgnoreCase))
        throw new InvalidOperationException(ServiceEndpointSdkResources.InvalidAzureRmEndpointAuthorizer((object) this.ServiceEndpoint.Authorization.Scheme));
      string str = "Bearer " + AzureEndpointUserPrincipalAuthorizer.GetAuthorizationToken(this._requestContext, this.ServiceEndpoint, resourceUrl);
      request.Headers.Add(HttpRequestHeader.Authorization, str);
    }

    public static string GetAuthorizationToken(
      IVssRequestContext requestContext,
      ServiceEndpoint serviceEndpoint,
      string resourceUrl)
    {
      if (string.IsNullOrEmpty(resourceUrl))
        resourceUrl = serviceEndpoint.Url.OriginalString;
      string usingVstsAppToken = AzureEndpointUserPrincipalAuthorizer.GetAuthorizationTokenUsingVstsAppToken(requestContext, serviceEndpoint, resourceUrl);
      if (string.IsNullOrEmpty(usingVstsAppToken))
      {
        requestContext.TraceWarning("WebApiProxy", "Unable to acquire JWT token for resource '{0}'.", (object) resourceUrl);
        throw new InvalidAuthorizationDetailsException(ServiceEndpointSdkResources.FailedToObtainTokenUsingUserPrincipal());
      }
      return usingVstsAppToken;
    }

    public static string GetAuthorizationTokenUsingVstsAppToken(
      IVssRequestContext requestContext,
      ServiceEndpoint serviceEndpoint,
      string resourceUrl)
    {
      requestContext.TraceEnter("AzureAccessTokenHelperSdk", nameof (GetAuthorizationTokenUsingVstsAppToken));
      string tenantId;
      serviceEndpoint.Authorization.Parameters.TryGetValue("tenantid", out tenantId);
      string vstsToken = (string) null;
      if (serviceEndpoint.Authorization.Parameters.ContainsKey("AccessToken"))
      {
        serviceEndpoint.Authorization.Parameters.Add("AccessTokenFetchingMethod", "Oauth");
        string errorMessage;
        if (!VstsAccessTokenHelper.TryGetVstsAccessToken(requestContext, serviceEndpoint, out vstsToken, out string _, out string _, out string _, out errorMessage))
          throw new InvalidOperationException(errorMessage);
      }
      return MsalAzureAccessTokenHelper.GetResourceAccessToken(requestContext, tenantId, resourceUrl, vstsToken);
    }
  }
}
