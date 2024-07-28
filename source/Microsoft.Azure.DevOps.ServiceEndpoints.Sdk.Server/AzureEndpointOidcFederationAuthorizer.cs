// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.AzureEndpointOidcFederationAuthorizer
// Assembly: Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 002C83BC-B53E-470A-8038-76E47B5E5BF3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.dll

using Microsoft.Identity.Client;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.Threading;
using Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server
{
  internal class AzureEndpointOidcFederationAuthorizer : AzureEndpointAuthorizer
  {
    private const int c_maxAadMessageLength = 300;
    private readonly IAzureOidcFederationTokenAssertionProvider _oidcTokenProvider;
    private readonly IAzureOidcFederationAadTokenProvider _aadTokenProvider;

    public AzureEndpointOidcFederationAuthorizer(
      IVssRequestContext requestContext,
      ServiceEndpoint serviceEndpoint,
      IAzureOidcFederationTokenAssertionProvider oidcTokenProvider,
      IAzureOidcFederationAadTokenProvider aadTokenProvider)
      : base(requestContext, serviceEndpoint)
    {
      this._oidcTokenProvider = oidcTokenProvider;
      this._aadTokenProvider = aadTokenProvider;
    }

    public static async Task<AzureEndpointOidcFederationAuthorizer> CreateAzureEndpointOidcFederationAuthorizer(
      IVssRequestContext requestContext,
      Guid scopeIdentifier,
      ServiceEndpoint serviceEndpoint)
    {
      return AzureEndpointOidcFederationAuthorizer.CreateAzureEndpointOidcFederationAuthorizer(requestContext, serviceEndpoint, await OidcFederationClaims.CreateOidcFederationClaims(requestContext, scopeIdentifier, serviceEndpoint.Id));
    }

    public static AzureEndpointOidcFederationAuthorizer CreateAzureEndpointOidcFederationAuthorizer(
      IVssRequestContext requestContext,
      ServiceEndpoint serviceEndpoint,
      OidcFederationClaims federationClaims,
      Guid scopeIdentifier = default (Guid))
    {
      string authorityUrl = serviceEndpoint.GetAuthorityUrl(requestContext);
      bool validateAuthority = !serviceEndpoint.IsAdfsAuthenticationEnabled();
      TimeSpan tokenLifetime = AzureEndpointOidcFederationAuthorizer.GetTokenLifetime(requestContext);
      return new AzureEndpointOidcFederationAuthorizer(requestContext, serviceEndpoint, (IAzureOidcFederationTokenAssertionProvider) new AzureOidcFederationTokenAssertionProvider(scopeIdentifier, (IOidcFederationClaims) federationClaims, tokenLifetime), (IAzureOidcFederationAadTokenProvider) new AzureOidcFederationAadTokenProvider(authorityUrl, validateAuthority));
    }

    private static TimeSpan GetTokenLifetime(IVssRequestContext requestContext)
    {
      IVssRequestContext vssRequestContext = requestContext.Elevate();
      return TimeSpan.FromMinutes((double) vssRequestContext.GetService<IVssRegistryService>().GetValue<int>(vssRequestContext, (RegistryQuery) "/Service/ServiceEndpoints/Settings/WebApiProxy/OidcTokenMaxValidTime", true, 5));
    }

    public override void AuthorizeRequest(HttpWebRequest request, string resourceUrl)
    {
      string str = this.IssueToken(resourceUrl);
      request.Headers.Add(HttpRequestHeader.Authorization, "Bearer " + str);
    }

    internal string IssueToken(string resourceUrl = null)
    {
      this._requestContext.TraceEnter("WebApiProxy", nameof (IssueToken));
      if (!this._requestContext.RunSynchronously<bool>((Func<Task<bool>>) (() => GlobalContributedFeatureStateResolver.IsFeatureEnabled(this._requestContext, "ms.vss-distributedtask-web.workload-identity-federation", ServiceInstanceTypes.TFS))))
        throw new InvalidOperationException(ServiceEndpointSdkResources.WorkloadIdentityFederationDisabled());
      if (!this.ServiceEndpoint.Authorization.Scheme.Equals("WorkloadIdentityFederation", StringComparison.OrdinalIgnoreCase))
        throw new InvalidOperationException(ServiceEndpointSdkResources.NoAzureOidcFederation());
      string servicePrincipalId;
      if (!this.ServiceEndpoint.Authorization.Parameters.TryGetValue("ServicePrincipalId", out servicePrincipalId))
        throw new InvalidOperationException(ServiceEndpointSdkResources.NoAzureServicePrincipal());
      string oidcToken = this._oidcTokenProvider.IssueOidcToken(this._requestContext);
      if (string.IsNullOrEmpty(oidcToken))
      {
        this._requestContext.TraceWarning("WebApiProxy", "Unable to acquire OpenIdConnect token for Service Principal Id : " + servicePrincipalId);
        throw new InvalidAuthorizationDetailsException(ServiceEndpointSdkResources.FailedToGenerateOidcToken((object) servicePrincipalId));
      }
      string resolvedResourceUrl = string.IsNullOrEmpty(resourceUrl) ? this.ServiceEndpoint.GetResourceUrl(this._requestContext) : resourceUrl;
      string str = this.LoginIntoAadUsingClientCredentialsGrantWithTokenAssertion(resourceUrl, servicePrincipalId, oidcToken, resolvedResourceUrl);
      if (string.IsNullOrEmpty(str))
      {
        this._requestContext.TraceWarning("WebApiProxy", string.Format("Msal returned an empty access token. ServicePrincipalId: {0}, resource url: {1}", (object) 0, (object) 1), (object) servicePrincipalId, (object) resourceUrl);
        throw new InvalidAuthorizationDetailsException(ServiceEndpointSdkResources.FailedToObtainToken());
      }
      return str;
    }

    private string LoginIntoAadUsingClientCredentialsGrantWithTokenAssertion(
      string resourceUrl,
      string servicePrincipalId,
      string oidcToken,
      string resolvedResourceUrl)
    {
      try
      {
        return this._aadTokenProvider.IssueAadAccessToken(this._requestContext, resolvedResourceUrl, servicePrincipalId, oidcToken);
      }
      catch (MsalServiceException ex)
      {
        this._requestContext.TraceWarning("WebApiProxy", string.Format("Unable to acquire JWT token for Serviceprincipal Id using client assertion: {0}, resource url: {1}, msalException: {2}", (object) 0, (object) 1, (object) 2), (object) servicePrincipalId, (object) resourceUrl, (object) ex);
        AzureEndpointOidcFederationAuthorizer.MsalExceptionDetails exceptionDetails = JsonConvert.DeserializeObject<AzureEndpointOidcFederationAuthorizer.MsalExceptionDetails>(ex.ResponseBody);
        int val1 = exceptionDetails != null && !string.IsNullOrEmpty(exceptionDetails.ErrorDescription) ? exceptionDetails.ErrorDescription.IndexOf(".") : throw new InvalidAuthorizationDetailsException(ServiceEndpointSdkResources.FailedToObtainToken());
        throw new InvalidAuthorizationDetailsException(ServiceEndpointSdkResources.FailedToSignIntoAadWithWorkloadIdentityFederation(val1 < 0 ? (object) exceptionDetails.ErrorDescription.Substring(0, 300) : (object) exceptionDetails.ErrorDescription.Substring(0, Math.Min(val1, 300))));
      }
      catch (Exception ex)
      {
        this._requestContext.TraceException("WebApiProxy", ex);
        throw new InvalidAuthorizationDetailsException(ServiceEndpointSdkResources.FailedToObtainToken());
      }
    }

    private class MsalExceptionDetails
    {
      [JsonProperty("error_description")]
      public string ErrorDescription { get; set; }
    }
  }
}
