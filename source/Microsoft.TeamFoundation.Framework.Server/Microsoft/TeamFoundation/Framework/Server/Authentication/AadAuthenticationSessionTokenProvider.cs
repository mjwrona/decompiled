// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.Authentication.AadAuthenticationSessionTokenProvider
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server.OAuth2;
using Microsoft.VisualStudio.Services.Organization;
using System;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Web;

namespace Microsoft.TeamFoundation.Framework.Server.Authentication
{
  public class AadAuthenticationSessionTokenProvider : IAadAuthenticationSessionTokenProvider
  {
    private static readonly string TraceArea = "Authentication";
    private static readonly string TraceLayer = nameof (AadAuthenticationSessionTokenProvider);
    internal const string RequestContextKeyName = "AadAuthenticationToken";
    internal const string CookieName = "AadAuthentication";
    private readonly IAadAuthenticationSessionTokenConfiguration configuration;

    public AadAuthenticationSessionTokenProvider()
      : this((IAadAuthenticationSessionTokenConfiguration) new AadAuthenticationSessionTokenConfiguration())
    {
    }

    public AadAuthenticationSessionTokenProvider(
      IAadAuthenticationSessionTokenConfiguration configuration)
    {
      this.configuration = configuration ?? (IAadAuthenticationSessionTokenConfiguration) new AadAuthenticationSessionTokenConfiguration();
    }

    public IAadAuthenticationSessionTokenConfiguration Configuration => this.configuration;

    public bool TryGetAadAuthenticationSessionCookie(
      IVssRequestContext requestContext,
      HttpContextBase httpContext,
      out UserAuthenticationSessionToken aadAccessToken)
    {
      aadAccessToken = (UserAuthenticationSessionToken) null;
      UserAuthenticationSessionToken authenticationSessionToken = (UserAuthenticationSessionToken) null;
      try
      {
        authenticationSessionToken = this.ReadSessionToken(requestContext, httpContext);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(734926, AadAuthenticationSessionTokenProvider.TraceArea, AadAuthenticationSessionTokenProvider.TraceLayer, ex);
      }
      if (authenticationSessionToken == null || !this.configuration.AcceptAadAuthenticationCookieEnabled(requestContext, authenticationSessionToken.SecurityToken))
        return false;
      aadAccessToken = authenticationSessionToken;
      return true;
    }

    public UserAuthenticationSessionToken ReadSessionToken(
      IVssRequestContext requestContext,
      HttpContextBase context)
    {
      UserAuthenticationSessionToken authenticationSessionToken1;
      if (requestContext.RootContext.TryGetItem<UserAuthenticationSessionToken>("AadAuthenticationToken", out authenticationSessionToken1))
        return authenticationSessionToken1;
      string encodedToken = this.ReadTokenFromCookie(requestContext, context);
      if (encodedToken == null)
        return (UserAuthenticationSessionToken) null;
      UserAuthenticationSessionToken authenticationSessionToken2 = this.ValidateToken(requestContext, encodedToken);
      if (authenticationSessionToken2 == null)
        return (UserAuthenticationSessionToken) null;
      requestContext.RootContext.Items["AadAuthenticationToken"] = (object) authenticationSessionToken2;
      return authenticationSessionToken2;
    }

    public void IssueSessionToken(IVssRequestContext requestContext, HttpContextBase httpContext)
    {
      JwtSecurityToken jwtSecurityToken;
      if (!requestContext.TryGetItem<JwtSecurityToken>("AadCookieTokenValidator_ValidatedToken", out jwtSecurityToken) || jwtSecurityToken == null)
        return;
      this.WriteTokenToCookie(requestContext, httpContext, jwtSecurityToken);
      UserAuthenticationSessionToken authenticationSessionToken = new UserAuthenticationSessionToken(httpContext.User as ClaimsPrincipal, jwtSecurityToken, AuthenticationMechanism.AAD_Cookie);
      requestContext.RootContext.Items["AadAuthenticationToken"] = (object) authenticationSessionToken;
    }

    public void WriteTokenToCookie(
      IVssRequestContext requestContext,
      HttpContextBase httpContext,
      JwtSecurityToken token)
    {
      IVssRequestContext context = requestContext.To(TeamFoundationHostType.Deployment);
      ITeamFoundationAuthenticationServiceInternal authenticationServiceInternal = context.GetService<ITeamFoundationAuthenticationService>().AuthenticationServiceInternal();
      DateTime validTo = token.ValidTo;
      IVssRequestContext requestContext1 = context;
      string sessionCookieDomain = authenticationServiceInternal.GetSessionCookieDomain(requestContext1);
      HttpCookie secureCookie = this.CreateSecureCookie("AadAuthentication", token.RawData, validTo, sessionCookieDomain);
      CookieModifier.AddSameSiteNoneToCookie(requestContext, secureCookie);
      httpContext.Response.Cookies.Set(secureCookie);
      httpContext.Response.Cache.SetCacheability(HttpCacheability.NoCache);
    }

    public void DeleteSessionToken(IVssRequestContext requestContext, HttpContextBase httpContext)
    {
      IVssRequestContext context = requestContext.To(TeamFoundationHostType.Deployment);
      ITeamFoundationAuthenticationServiceInternal authenticationServiceInternal = context.GetService<ITeamFoundationAuthenticationService>().AuthenticationServiceInternal();
      DateTime expires = DateTime.UtcNow.AddDays(-1.0);
      IVssRequestContext requestContext1 = context;
      string sessionCookieDomain = authenticationServiceInternal.GetSessionCookieDomain(requestContext1);
      if (httpContext.Request.Cookies["AadAuthentication"] == null)
        return;
      HttpCookie secureCookie = this.CreateSecureCookie("AadAuthentication", (string) null, expires, sessionCookieDomain);
      CookieModifier.AddSameSiteNoneToCookie(requestContext, secureCookie);
      httpContext.Response.Cookies.Remove("AadAuthentication");
      httpContext.Response.Cookies.Set(secureCookie);
      requestContext.RootContext.Items.Remove("AadAuthenticationToken");
    }

    public void UpdateSessionToken(
      IVssRequestContext requestContext,
      HttpContextBase httpContext,
      JwtSecurityToken token)
    {
      if (!(httpContext.Request.Headers["X-VSS-ClientAuthProvider"] == "MsalTokenProvider") || !this.configuration.IssueAadAuthenticationCookieEnabled(requestContext, token))
        return;
      string b = this.ReadTokenFromCookie(requestContext, httpContext);
      if (string.Equals(token.RawData, b, StringComparison.Ordinal))
        return;
      requestContext.TraceAlways(15142005, TraceLevel.Info, AadAuthenticationSessionTokenProvider.TraceArea, AadAuthenticationSessionTokenProvider.TraceLayer, "Updating AadAuthentication cookie");
      this.WriteTokenToCookie(requestContext, httpContext, token);
    }

    public AuthOptions GetAuthOptions(IVssRequestContext requestContext)
    {
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      IVssRegistryService service = vssRequestContext.GetService<IVssRegistryService>();
      string str1 = service.GetValue(vssRequestContext, (RegistryQuery) "/Service/Aad/AuthResource", "499b84ac-1321-427f-aa17-267ca6975798");
      string str2 = service.GetValue(vssRequestContext, (RegistryQuery) "/Service/Aad/AuthAuthority", false, (string) null);
      string str3 = service.GetValue(vssRequestContext, (RegistryQuery) "/Service/Aad/AuthClientId", false, (string) null);
      string tenantId = this.GetTenantId(requestContext);
      return new AuthOptions()
      {
        Authority = "https://" + str2 + "/" + tenantId + "/",
        ClientId = str3,
        Scopes = new string[1]
        {
          str1 + "/user_impersonation"
        }
      };
    }

    internal string GetTenantId(IVssRequestContext requestContext)
    {
      string tenantId1 = string.Empty;
      Guid tenantId2 = requestContext.GetTenantId();
      if (tenantId2 != Guid.Empty)
      {
        tenantId1 = tenantId2.ToString("D");
      }
      else
      {
        UserAuthenticationSessionToken authenticationSessionToken;
        if (requestContext.Items.TryGetValue<UserAuthenticationSessionToken>("AadAuthenticationToken", out authenticationSessionToken))
        {
          Claim claim = authenticationSessionToken.SecurityToken.Claims.FirstOrDefault<Claim>((Func<Claim, bool>) (c => c.Type == "tid"));
          if (claim != null)
            tenantId1 = claim.Value;
        }
      }
      if (string.IsNullOrEmpty(tenantId1))
      {
        IVssRequestContext context = requestContext.To(TeamFoundationHostType.Application);
        if (context.ServiceHost.IsOnly(TeamFoundationHostType.Application))
        {
          Microsoft.VisualStudio.Services.Organization.Organization organization = context.GetService<IOrganizationService>().GetOrganization(context, Enumerable.Empty<string>());
          if (organization != null && organization.TenantId != Guid.Empty)
            tenantId1 = organization.TenantId.ToString("D");
        }
      }
      if (string.IsNullOrEmpty(tenantId1))
      {
        requestContext.Trace(100136286, TraceLevel.Info, AadAuthenticationSessionTokenProvider.TraceArea, AadAuthenticationSessionTokenProvider.TraceLayer, "Tenant id not found, using the \"organizations\" authority.");
        tenantId1 = "organizations";
      }
      return tenantId1;
    }

    private string ReadTokenFromCookie(
      IVssRequestContext requestContext,
      HttpContextBase httpContext)
    {
      HttpCookie cookie = httpContext.Request.Cookies["AadAuthentication"];
      if (cookie != null)
        return cookie.Value;
      requestContext.Trace(15142000, TraceLevel.Info, AadAuthenticationSessionTokenProvider.TraceArea, AadAuthenticationSessionTokenProvider.TraceLayer, "AadAuthenticationCookie: Cookie {0} was not present in request.", (object) "AadAuthentication");
      return (string) null;
    }

    private UserAuthenticationSessionToken ValidateToken(
      IVssRequestContext requestContext,
      string encodedToken)
    {
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      IOAuth2AuthenticationService service = vssRequestContext.GetService<IOAuth2AuthenticationService>();
      JwtSecurityToken jwtToken;
      bool impersonating;
      bool validIdentity;
      ClaimsPrincipal user;
      try
      {
        user = service.ValidateToken(vssRequestContext, encodedToken, OAuth2TokenValidators.AADCookie, out jwtToken, out impersonating, out validIdentity);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(15142001, AadAuthenticationSessionTokenProvider.TraceArea, AadAuthenticationSessionTokenProvider.TraceLayer, ex);
        return (UserAuthenticationSessionToken) null;
      }
      if (!validIdentity)
      {
        requestContext.Trace(15142002, TraceLevel.Error, AadAuthenticationSessionTokenProvider.TraceArea, AadAuthenticationSessionTokenProvider.TraceLayer, "AuthenticationCookie: JWT token was not valid.");
        return (UserAuthenticationSessionToken) null;
      }
      if (!impersonating)
        return new UserAuthenticationSessionToken(user, jwtToken, AuthenticationMechanism.AAD_Cookie);
      requestContext.Trace(15142003, TraceLevel.Error, AadAuthenticationSessionTokenProvider.TraceArea, AadAuthenticationSessionTokenProvider.TraceLayer, "AuthenticationCookie: Impersonation not permitted with cookie auth.");
      return (UserAuthenticationSessionToken) null;
    }

    private HttpCookie CreateSecureCookie(
      string name,
      string value,
      DateTime expires,
      string domain)
    {
      return new HttpCookie(name, value)
      {
        Expires = expires,
        Domain = domain,
        HttpOnly = true,
        Secure = true
      };
    }
  }
}
