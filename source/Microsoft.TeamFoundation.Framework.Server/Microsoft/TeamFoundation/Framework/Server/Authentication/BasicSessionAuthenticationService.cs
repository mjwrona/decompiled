// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.Authentication.BasicSessionAuthenticationService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.Location;
using Microsoft.VisualStudio.Services.Location.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.IdentityModel.Services;
using System.IdentityModel.Services.Configuration;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web;

namespace Microsoft.TeamFoundation.Framework.Server.Authentication
{
  public class BasicSessionAuthenticationService : 
    ITeamFoundationAuthenticationService,
    IVssFrameworkService,
    ITeamFoundationAuthenticationServiceInternal
  {
    private CookieHandlerSettings m_cookieHandlerSettings;
    private const string s_area = "UserAuthenticationService";
    private const string s_layer = "IVssFrameworkService";
    private const string STSServiceType = "STS";
    private static readonly Guid WebSignInUrl = new Guid("6e33c2d4-dcb3-4bc0-a63b-4dbbcae9ba78");
    private static readonly HashSet<string> s_redirectionSuppressedHttpMethods = new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase)
    {
      "OPTIONS",
      "HEAD",
      "PROPFIND",
      "PATCH"
    };

    public void ServiceStart(IVssRequestContext systemRequestContext) => this.m_cookieHandlerSettings = new CookieHandlerSettings(systemRequestContext.GetService<IVssRegistryService>().ReadEntriesFallThru(systemRequestContext, (RegistryQuery) (FederatedAuthRegistryConstants.CookieHandler + "/**")));

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public void ConfigureBasic(IVssRequestContext requestContext, Uri realm)
    {
      CachedRegistryService service = requestContext.GetService<CachedRegistryService>();
      service.SetValue<string>(requestContext, FederatedAuthRegistryConstants.DefaultRealm, realm.AbsoluteUri);
      if (!requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
        return;
      service.SetValue<string>(requestContext, FederatedAuthRegistryConstants.Issuer, "https://www.visualstudio.com/");
      service.SetValue<bool>(requestContext, FederatedAuthRegistryConstants.RequireSsl, realm.Scheme.Equals(Uri.UriSchemeHttps, StringComparison.OrdinalIgnoreCase));
      service.SetValue<bool>(requestContext, FederatedAuthRegistryConstants.Enabled, true);
    }

    public void ConfigureRequest(IVssRequestContext requestContext)
    {
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      FederationConfiguration federationConfiguration = vssRequestContext.GetService<FederationConfigurationService>().GetFederationConfiguration(vssRequestContext);
      SessionAuthenticationModule authenticationModule = FederatedAuthentication.SessionAuthenticationModule;
      authenticationModule.FederationConfiguration = federationConfiguration;
      CookieHandler cookieHandler1 = authenticationModule.CookieHandler;
      if (cookieHandler1 == FederatedAuthentication.FederationConfiguration.CookieHandler)
      {
        ChunkedCookieHandler chunkedCookieHandler = new ChunkedCookieHandler(((ChunkedCookieHandler) cookieHandler1).ChunkSize);
        chunkedCookieHandler.Domain = cookieHandler1.Domain;
        chunkedCookieHandler.HideFromClientScript = cookieHandler1.HideFromClientScript;
        chunkedCookieHandler.Name = cookieHandler1.Name;
        chunkedCookieHandler.Path = cookieHandler1.Path;
        chunkedCookieHandler.PersistentSessionLifetime = cookieHandler1.PersistentSessionLifetime;
        chunkedCookieHandler.RequireSsl = cookieHandler1.RequireSsl;
        CookieHandler cookieHandler2 = (CookieHandler) chunkedCookieHandler;
        FederatedAuthentication.SessionAuthenticationModule.CookieHandler = cookieHandler2;
        cookieHandler1 = cookieHandler2;
      }
      cookieHandler1.Domain = this.GetSessionCookieDomain(requestContext);
      bool? fromClientScript = this.m_cookieHandlerSettings.HideFromClientScript;
      if (fromClientScript.HasValue)
      {
        CookieHandler cookieHandler3 = cookieHandler1;
        fromClientScript = this.m_cookieHandlerSettings.HideFromClientScript;
        int num = fromClientScript.Value ? 1 : 0;
        cookieHandler3.HideFromClientScript = num != 0;
      }
      cookieHandler1.RequireSsl = this.m_cookieHandlerSettings.RequireSsl;
    }

    public List<SessionSecurityTokenData> GetSessionSecurityTokenDataFromCookies(
      IVssRequestContext requestContext)
    {
      return requestContext.GetService<TeamFoundationAuthenticationService>().GetSessionSecurityTokenDataFromCookies(requestContext);
    }

    public string GetSignInRedirectLocation(IVssRequestContext requestContext)
    {
      if (requestContext.IsFeatureEnabled("Dreamlifter.RedirectToGitHubWhenAuthIsRequired"))
        return "https://github.com";
      ILocationService service = requestContext.GetService<ILocationService>();
      string str = service.LocationForAccessMapping(requestContext, "STS", BasicSessionAuthenticationService.WebSignInUrl, service.GetPublicAccessMapping(requestContext));
      Uri url = HttpContext.Current.Request.Url;
      string host = url.Host;
      return str.Replace("{realm}", host).Replace("{reply_to}", Uri.EscapeDataString(url.AbsoluteUri)).Replace("{context}", SignInContextFactory.Deconstruct(SignInContextFactory.Construct(requestContext)));
    }

    public void AddFederatedAuthHeaders(
      IVssRequestContext requestContext,
      HttpResponseBase response)
    {
      if (string.IsNullOrWhiteSpace(response.Headers.Get("X-TFS-FedAuthRealm")))
      {
        string str = requestContext.GetService<IVssRegistryService>().GetValue<string>(requestContext, (RegistryQuery) FederatedAuthRegistryConstants.DefaultRealm, false, (string) null);
        response.AddHeader("X-TFS-FedAuthRealm", str);
      }
      if (string.IsNullOrWhiteSpace(response.Headers.Get("X-TFS-FedAuthIssuer")))
      {
        string locationServiceUrl = requestContext.GetService<ILocationService>().GetLocationServiceUrl(requestContext, Guid.Empty, AccessMappingConstants.ClientAccessMappingMoniker);
        response.AddHeader("X-TFS-FedAuthIssuer", locationServiceUrl);
      }
      if (!string.IsNullOrWhiteSpace(response.Headers.Get("X-VSS-AuthorizationEndpoint")))
        return;
      string locationServiceUrl1 = requestContext.GetService<ILocationService>().GetLocationServiceUrl(requestContext, ServiceInstanceTypes.SPS, AccessMappingConstants.PublicAccessMappingMoniker);
      if (string.IsNullOrEmpty(locationServiceUrl1))
        return;
      response.AddHeader("X-VSS-AuthorizationEndpoint", locationServiceUrl1);
    }

    public void RedirectToIdentityProvider(
      IVssRequestContext requestContext,
      bool hasInvalidToken = false,
      bool force = false)
    {
      HttpContextBase current = HttpContextFactory.Current;
      if (current.Items.Contains((object) HttpContextConstants.ArrRequestRouted))
        return;
      if (hasInvalidToken)
        current.Response.AddHeader("X-VSS-AuthenticateError", "InvalidToken");
      this.AddFederatedAuthHeaders(requestContext, current.Response);
      string redirectLocation = this.GetSignInRedirectLocation(requestContext);
      AuthenticationMechanisms mechanismsToAdvertise = requestContext.RequestRestrictions().MechanismsToAdvertise;
      if ((current.Request.Headers["X-TFS-FedAuthRedirect"] == "Suppress" ? 1 : (mechanismsToAdvertise.HasFlag((System.Enum) AuthenticationMechanisms.FederatedRedirect) ? 0 : (mechanismsToAdvertise.HasFlag((System.Enum) AuthenticationMechanisms.Federated) ? 1 : 0))) != 0 || BasicSessionAuthenticationService.s_redirectionSuppressedHttpMethods.Contains(current.Request.HttpMethod))
      {
        if (!string.IsNullOrWhiteSpace(HttpContext.Current.Response.Headers.Get("X-TFS-FedAuthRedirect")))
          return;
        current.Response.AddHeader("WWW-Authenticate", "TFS-Federated");
        current.Response.AddHeader("X-TFS-FedAuthRedirect", redirectLocation);
      }
      else
      {
        current.Server.ClearError();
        current.Response.Redirect(redirectLocation, true);
      }
    }

    bool ITeamFoundationAuthenticationServiceInternal.IssueSessionSecurityToken(
      IVssRequestContext requestContext,
      ClaimsPrincipal principal,
      IEnumerable<Claim> additionalClaims)
    {
      return AuthenticationHelpers.IssueSessionSecurityToken(requestContext, principal, additionalClaims);
    }

    void ITeamFoundationAuthenticationServiceInternal.ReissueFedAuthToken(
      IVssRequestContext requestContext)
    {
    }

    IdentityValidationResult ITeamFoundationAuthenticationServiceInternal.CompleteUnauthorizedRequest(
      IVssRequestContext requestContext,
      HttpResponseBase response,
      IdentityValidationResult identityValidationResult,
      Uri redirectUrl)
    {
      this.AddFederatedAuthHeaders(requestContext, response);
      if (identityValidationResult.HttpStatusCode == HttpStatusCode.Unauthorized && requestContext.UserContext != (IdentityDescriptor) null)
      {
        if (string.IsNullOrWhiteSpace(response.Headers.Get("X-TFS-FedAuthRedirect")))
        {
          string redirectLocation = this.GetSignInRedirectLocation(requestContext);
          response.AddHeader("WWW-Authenticate", "TFS-Federated");
          response.AddHeader("X-TFS-FedAuthRedirect", redirectLocation);
        }
        response.AddHeader("X-VSS-AuthenticateError", "Unauthorized");
      }
      return IdentityValidationResult.Unauthorized(FrameworkResources.AuthenticationRequiredError());
    }

    public IAuthCredential GetAuthCredential() => (IAuthCredential) null;

    public string GetSignInRedirectLocation(
      IVssRequestContext requestContext,
      bool force = false,
      IDictionary<string, string> parameters = null,
      Uri replyToOverride = null,
      SwitchHintParameter switchHintParameter = null)
    {
      throw new NotImplementedException();
    }

    public void AddFederatedAuthHeaders(
      IVssRequestContext requestContext,
      HttpResponseMessage message)
    {
      throw new NotImplementedException();
    }

    public void SignOutFromSessionModule(IVssRequestContext requestContext) => throw new NotImplementedException();

    public string LocationForRealm(IVssRequestContext requestContext, string relativePath) => throw new NotImplementedException();

    public string DetermineRealm(IVssRequestContext requestContext) => throw new NotImplementedException();

    public void ProcessSignOutCookie(IVssRequestContext requestContext, Uri realmUri) => throw new NotImplementedException();

    public string BuildHostedSignOutUrl(IVssRequestContext requestContext) => throw new NotImplementedException();

    public string BuildAADSignOutUrl(IVssRequestContext requestContext, string callBackUrl = null) => throw new NotImplementedException();

    public SignOutUris ReadSignOutCookie(IVssRequestContext requestContext) => throw new NotImplementedException();

    public void ClearSignOutCookie(IVssRequestContext requestContext) => throw new NotImplementedException();

    bool ITeamFoundationAuthenticationServiceInternal.IsRequestAuthenticationValid(
      IVssWebRequestContext requestContext,
      bool isSshRequest)
    {
      throw new NotImplementedException();
    }

    FederatedAuthenticationSettings ITeamFoundationAuthenticationServiceInternal.GetFederatedAuthenticationSettings(
      IVssRequestContext requestContext)
    {
      throw new NotImplementedException();
    }

    void ITeamFoundationAuthenticationServiceInternal.SetAuthenticationCredential(
      IAuthCredential credential)
    {
    }

    string ITeamFoundationAuthenticationServiceInternal.GetCookieRootDomain(
      IVssRequestContext requestContext)
    {
      throw new NotImplementedException();
    }

    string ITeamFoundationAuthenticationServiceInternal.GetSessionCookieDomain(
      IVssRequestContext requestContext)
    {
      return this.GetSessionCookieDomain(requestContext);
    }

    internal string GetSessionCookieDomain(IVssRequestContext requestContext) => this.m_cookieHandlerSettings.GetCookieDomain(requestContext.RequestUri()?.Host);

    public void AddTenantInfoResponseHeader(
      IVssRequestContext tfsRequestContext,
      HttpResponseMessage response)
    {
      throw new NotImplementedException();
    }
  }
}
