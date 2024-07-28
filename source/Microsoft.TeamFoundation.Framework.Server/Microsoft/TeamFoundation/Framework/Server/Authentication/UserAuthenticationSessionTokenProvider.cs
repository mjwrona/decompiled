// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.Authentication.UserAuthenticationSessionTokenProvider
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server.Authentication.UserAuthentication;
using Microsoft.TeamFoundation.Framework.Server.ConfigFramework;
using Microsoft.TeamFoundation.Framework.Server.OAuth2;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.Location.Server;
using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.VisualStudio.Services.WebApi.Jwt;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Web;

namespace Microsoft.TeamFoundation.Framework.Server.Authentication
{
  internal class UserAuthenticationSessionTokenProvider
  {
    private static readonly string s_area = "Authentication";
    private static readonly string s_layer = nameof (UserAuthenticationSessionTokenProvider);
    internal const string RequestContextKeyName = "UserAuthenticationToken";
    public const string FedAuthShimIdentifier = "UserAuthenticationToken";
    private const string PreserveValidFromConfigPath = "Identity.UserAuthenticationSessionToken.PreserveValidFromOnRefresh";
    private const string DisableUserAuthConfigPath = "Identity.UserAuthenticationSessionToken.Disable";
    private static readonly IConfigPrototype<bool> preserveValidFromConfigPrototype = ConfigPrototype.Create<bool>("Identity.UserAuthenticationSessionToken.PreserveValidFromOnRefresh", false);
    private readonly IConfigQueryable<bool> preserveValidFromConfig;
    private static readonly IConfigPrototype<bool> disableUserAuthConfigPrototype = ConfigPrototype.Create<bool>("Identity.UserAuthenticationSessionToken.Disable", false);
    private readonly IConfigQueryable<bool> disableUserAuthConfig;
    public const string UserAuthenticationSkipped = "$UserAuthenticationSkipped";

    public UserAuthenticationSessionTokenProvider()
      : this(ConfigProxy.Create<bool>(UserAuthenticationSessionTokenProvider.preserveValidFromConfigPrototype), ConfigProxy.Create<bool>(UserAuthenticationSessionTokenProvider.disableUserAuthConfigPrototype))
    {
    }

    public UserAuthenticationSessionTokenProvider(
      IConfigQueryable<bool> preserveValidFromConfig,
      IConfigQueryable<bool> disableUserAuthConfig)
    {
      this.preserveValidFromConfig = preserveValidFromConfig;
      this.disableUserAuthConfig = disableUserAuthConfig;
    }

    public UserAuthenticationSessionToken ReadSessionToken(
      IVssRequestContext requestContext,
      HttpContextBase context)
    {
      UserAuthenticationSessionToken authenticationSessionToken1;
      if (requestContext.RootContext.TryGetItem<UserAuthenticationSessionToken>("UserAuthenticationToken", out authenticationSessionToken1))
        return authenticationSessionToken1;
      AuthenticationMechanism authenticationMechanism;
      string encodedToken = this.ReadTokenFromCookie(requestContext, context, out authenticationMechanism);
      if (encodedToken == null)
        return (UserAuthenticationSessionToken) null;
      UserAuthenticationSessionToken authenticationSessionToken2 = this.ValidateToken(requestContext, encodedToken, authenticationMechanism);
      if (authenticationSessionToken2 == null)
        return (UserAuthenticationSessionToken) null;
      if (!AuthenticationHelpers.ShouldSkipReissuingAuthToken(requestContext) && this.ShouldRefreshToken(requestContext, context, authenticationSessionToken2))
      {
        if (authenticationSessionToken2.AuthenticationMechanism == AuthenticationMechanism.UserAuthToken && !requestContext.IsIssueFedAuthTokenDisabled())
          requestContext.GetService<ITeamFoundationAuthenticationService>().AuthenticationServiceInternal().ReissueFedAuthToken(requestContext);
        if (this.preserveValidFromConfig.QueryByCtx<bool>(requestContext))
        {
          requestContext.Trace(17002, TraceLevel.Info, UserAuthenticationSessionTokenProvider.s_area, UserAuthenticationSessionTokenProvider.s_layer, "Preserving ValidFrom property between UserAuth token renewals");
          authenticationSessionToken2 = this.IssueSessionToken(requestContext, context, authenticationSessionToken2.User, authenticationSessionToken2.AuthenticationMechanism, currentToken: authenticationSessionToken2);
        }
        else
        {
          requestContext.Trace(17003, TraceLevel.Info, UserAuthenticationSessionTokenProvider.s_area, UserAuthenticationSessionTokenProvider.s_layer, "Issuing UserAuth session token from current timestamp");
          authenticationSessionToken2 = this.IssueSessionToken(requestContext, context, authenticationSessionToken2.User, authenticationSessionToken2.AuthenticationMechanism);
        }
      }
      requestContext.RootContext.Items["UserAuthenticationToken"] = (object) authenticationSessionToken2;
      return authenticationSessionToken2;
    }

    public UserAuthenticationSessionToken IssueSessionToken(
      IVssRequestContext requestContext,
      HttpContextBase httpContext,
      ClaimsPrincipal claimsPrincipal,
      AuthenticationMechanism authenticationMechanism = AuthenticationMechanism.UserAuthToken,
      IEnumerable<Claim> additionalClaims = null,
      UserAuthenticationSessionToken currentToken = null)
    {
      return this.IssueSessionToken(requestContext, httpContext, claimsPrincipal, currentToken?.SecurityToken.ValidFrom);
    }

    private UserAuthenticationSessionToken IssueSessionToken(
      IVssRequestContext requestContext,
      HttpContextBase httpContext,
      ClaimsPrincipal claimsPrincipal,
      DateTime? validFrom,
      AuthenticationMechanism authenticationMechanism = AuthenticationMechanism.UserAuthToken,
      IEnumerable<Claim> additionalClaims = null)
    {
      UserAuthenticationSessionToken sessionToken = this.CreateSessionToken(requestContext, httpContext, claimsPrincipal, authenticationMechanism, additionalClaims, validFrom);
      this.WriteTokenToCookie(requestContext, httpContext, sessionToken);
      requestContext.RootContext.Items["UserAuthenticationToken"] = (object) sessionToken;
      return sessionToken;
    }

    public UserAuthenticationSessionToken UpgradeFromFedAuthToUserAuthToken(
      IVssRequestContext requestContext,
      HttpContextBase httpContext,
      ClaimsPrincipal claimsPrincipal,
      System.IdentityModel.Tokens.SecurityToken fedAuthToken,
      AuthenticationMechanism authenticationMechanism = AuthenticationMechanism.UserAuthToken,
      IEnumerable<Claim> additionalClaims = null)
    {
      return this.IssueSessionToken(requestContext, httpContext, claimsPrincipal, new DateTime?(fedAuthToken.ValidFrom), authenticationMechanism, additionalClaims);
    }

    public UserAuthenticationSessionToken CreateSessionToken(
      IVssRequestContext requestContext,
      HttpContextBase httpContext,
      ClaimsPrincipal claimsPrincipal,
      AuthenticationMechanism authenticationMechanism,
      IEnumerable<Claim> additionalClaims = null,
      DateTime? tokenValidFrom = null)
    {
      UserAuthenticationConfiguration configuration = this.GetConfiguration(requestContext);
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      VssSigningCredentials signingCredentials = vssRequestContext.GetService<IUserAuthenticationConfigurationService>().GetSigningCredentials(vssRequestContext);
      string issuer = configuration.Issuer ?? this.GetIssuerFallback(requestContext);
      List<Claim> additionalClaims1 = new List<Claim>();
      foreach (Claim mergeClaim in AuthenticationHelpers.MergeClaims(claimsPrincipal.Claims, additionalClaims))
      {
        string validJwtClaimType = UserAuthenticationSessionTokenProvider.ClaimTypeMapping.GetValidJwtClaimType(mergeClaim.Type);
        if ((!(mergeClaim.Type == "prov_data") || ProvDataClaimHelper.HasGitHubClaim(ProvDataClaimHelper.ToProviderDataStub(requestContext, mergeClaim))) && validJwtClaimType != null)
          additionalClaims1.Add(new Claim(validJwtClaimType, mergeClaim.Value));
      }
      additionalClaims1.Add(new Claim("ver", configuration.CurrentVersion));
      additionalClaims1.Add(new Claim("jti", Guid.NewGuid().ToString("D")));
      JsonWebToken jsonWebToken = JsonWebToken.Create(issuer, configuration.Audience, tokenValidFrom ?? configuration.TimeProvider.Now, configuration.TimeProvider.Now.Add(configuration.ExpireTimeSpan), (IEnumerable<Claim>) additionalClaims1, signingCredentials);
      return new UserAuthenticationSessionToken(claimsPrincipal, new JwtSecurityToken(jsonWebToken.EncodedToken), authenticationMechanism);
    }

    public void WriteTokenToCookie(
      IVssRequestContext requestContext,
      HttpContextBase httpContext,
      UserAuthenticationSessionToken token)
    {
      if (this.IsUserAuthenticationDisabled(requestContext, httpContext, token))
        return;
      UserAuthenticationConfiguration configuration = this.GetConfiguration(requestContext);
      IVssRequestContext context = requestContext.To(TeamFoundationHostType.Deployment);
      ITeamFoundationAuthenticationServiceInternal authenticationServiceInternal = context.GetService<ITeamFoundationAuthenticationService>().AuthenticationServiceInternal();
      DateTime validTo = token.SecurityToken.ValidTo;
      IVssRequestContext requestContext1 = context;
      string sessionCookieDomain = authenticationServiceInternal.GetSessionCookieDomain(requestContext1);
      if (token.AuthenticationMechanism == AuthenticationMechanism.UserAuthToken)
      {
        HttpCookie secureCookie = this.CreateSecureCookie(configuration.CookieName, token.SecurityToken.RawData, validTo, sessionCookieDomain);
        CookieModifier.AddSameSiteNoneToCookie(requestContext, secureCookie);
        httpContext.Response.Cookies.Set(secureCookie);
      }
      else if (token.AuthenticationMechanism == AuthenticationMechanism.UserAuthToken_VS2012)
      {
        HttpCookie secureCookie1 = this.CreateSecureCookie("FedAuth", "UserAuthenticationToken", validTo, sessionCookieDomain);
        HttpCookie secureCookie2 = this.CreateSecureCookie("FedAuth1", token.SecurityToken.RawData, validTo, sessionCookieDomain);
        CookieModifier.AddSameSiteNoneToCookie(requestContext, secureCookie1);
        CookieModifier.AddSameSiteNoneToCookie(requestContext, secureCookie2);
        httpContext.Response.Cookies.Set(secureCookie1);
        httpContext.Response.Cookies.Set(secureCookie2);
      }
      httpContext.Response.Cache.SetCacheability(HttpCacheability.NoCache);
    }

    public void DeleteSessionToken(IVssRequestContext requestContext, HttpContextBase httpContext)
    {
      UserAuthenticationConfiguration configuration = this.GetConfiguration(requestContext);
      IVssRequestContext context = requestContext.To(TeamFoundationHostType.Deployment);
      ITeamFoundationAuthenticationServiceInternal authenticationServiceInternal = context.GetService<ITeamFoundationAuthenticationService>().AuthenticationServiceInternal();
      DateTime expires = DateTime.UtcNow.AddDays(-1.0);
      IVssRequestContext requestContext1 = context;
      string sessionCookieDomain = authenticationServiceInternal.GetSessionCookieDomain(requestContext1);
      if (httpContext.Request.Cookies[configuration.CookieName] != null)
      {
        HttpCookie secureCookie = this.CreateSecureCookie(configuration.CookieName, (string) null, expires, sessionCookieDomain);
        CookieModifier.AddSameSiteNoneToCookie(requestContext, secureCookie);
        httpContext.Response.Cookies.Set(secureCookie);
        requestContext.RootContext.Items.Remove("UserAuthenticationToken");
      }
      if (!(httpContext.Request.Cookies["FedAuth"]?.Value == "UserAuthenticationToken"))
        return;
      HttpCookie secureCookie1 = this.CreateSecureCookie("FedAuth", (string) null, expires, sessionCookieDomain);
      HttpCookie secureCookie2 = this.CreateSecureCookie("FedAuth1", (string) null, expires, sessionCookieDomain);
      CookieModifier.AddSameSiteNoneToCookie(requestContext, secureCookie1);
      CookieModifier.AddSameSiteNoneToCookie(requestContext, secureCookie2);
      httpContext.Response.Cookies.Set(secureCookie1);
      httpContext.Response.Cookies.Set(secureCookie2);
    }

    private string ReadTokenFromCookie(
      IVssRequestContext requestContext,
      HttpContextBase httpContext,
      out AuthenticationMechanism authenticationMechanism)
    {
      authenticationMechanism = AuthenticationMechanism.UserAuthToken;
      UserAuthenticationConfiguration configuration = this.GetConfiguration(requestContext);
      HttpCookie cookie1 = httpContext.Request.Cookies[configuration.CookieName];
      if (cookie1 == null)
      {
        HttpCookie cookie2 = httpContext.Request.Cookies["FedAuth"];
        if (cookie2 != null && cookie2.Value == "UserAuthenticationToken")
        {
          cookie1 = httpContext.Request.Cookies["FedAuth1"];
          authenticationMechanism = AuthenticationMechanism.UserAuthToken_VS2012;
          requestContext.Trace(828620, TraceLevel.Info, UserAuthenticationSessionTokenProvider.s_area, UserAuthenticationSessionTokenProvider.s_layer, "UserAuthenticationCookie present in VS2012 sign in request.");
        }
      }
      if (cookie1 != null)
        return cookie1.Value;
      requestContext.Trace(828619, TraceLevel.Info, UserAuthenticationSessionTokenProvider.s_area, UserAuthenticationSessionTokenProvider.s_layer, "AuthenticationCookie: Cookie was not present in request.");
      return (string) null;
    }

    private UserAuthenticationSessionToken ValidateToken(
      IVssRequestContext requestContext,
      string encodedToken,
      AuthenticationMechanism authenticationMechanism)
    {
      this.GetConfiguration(requestContext);
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      IOAuth2AuthenticationService service = vssRequestContext.GetService<IOAuth2AuthenticationService>();
      OAuth2TokenValidators allowedValidators = authenticationMechanism == AuthenticationMechanism.UserAuthToken_VS2012 ? OAuth2TokenValidators.UserAuthentication_VS2012 : OAuth2TokenValidators.UserAuthentication;
      JwtSecurityToken jwtToken;
      bool impersonating;
      bool validIdentity;
      ClaimsPrincipal user;
      try
      {
        user = service.ValidateToken(vssRequestContext, encodedToken, allowedValidators, out jwtToken, out impersonating, out validIdentity);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(248428, UserAuthenticationSessionTokenProvider.s_area, UserAuthenticationSessionTokenProvider.s_layer, ex);
        return (UserAuthenticationSessionToken) null;
      }
      if (!validIdentity)
      {
        requestContext.Trace(288525, TraceLevel.Error, UserAuthenticationSessionTokenProvider.s_area, UserAuthenticationSessionTokenProvider.s_layer, "AuthenticationCookie: JWT token was not valid.");
        return (UserAuthenticationSessionToken) null;
      }
      if (!impersonating)
        return new UserAuthenticationSessionToken(user, jwtToken, authenticationMechanism);
      requestContext.Trace(920898, TraceLevel.Error, UserAuthenticationSessionTokenProvider.s_area, UserAuthenticationSessionTokenProvider.s_layer, "AuthenticationCookie: Impersonation not permitted with cookie auth.");
      return (UserAuthenticationSessionToken) null;
    }

    private string GetIssuerFallback(IVssRequestContext requestContext) => new Uri(requestContext.GetService<ILocationService>().GetLocationServiceUrl(requestContext, ServiceInstanceTypes.SPS, "PublicAccessMapping") ?? requestContext.GetService<IVssRegistryService>().GetValue<string>(requestContext, (RegistryQuery) "/Configuration/SharedService/Realm", true, (string) null)).Host;

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

    internal bool ShouldRefreshToken(
      IVssRequestContext requestContext,
      HttpContextBase httpContext,
      UserAuthenticationSessionToken token)
    {
      UserAuthenticationConfiguration configuration = this.GetConfiguration(requestContext);
      if (configuration.UseSlidingExpiration)
      {
        TimeSpan? reIssueTimeSpan = configuration.ReIssueTimeSpan;
        if (reIssueTimeSpan.HasValue)
        {
          DateTime now = configuration.TimeProvider.Now;
          DateTime dateTime;
          if (this.preserveValidFromConfig.QueryByCtx<bool>(requestContext))
          {
            requestContext.Trace(17004, TraceLevel.Info, UserAuthenticationSessionTokenProvider.s_area, UserAuthenticationSessionTokenProvider.s_layer, "Determining ShouldRefresh based on ValidTo");
            dateTime = token.SecurityToken.ValidTo.Subtract(configuration.ExpireTimeSpan);
          }
          else
          {
            requestContext.Trace(17005, TraceLevel.Info, UserAuthenticationSessionTokenProvider.s_area, UserAuthenticationSessionTokenProvider.s_layer, "Determining ShouldRefresh based on ValidFrom");
            dateTime = token.SecurityToken.ValidFrom;
          }
          TimeSpan timeSpan = now.Subtract(dateTime);
          reIssueTimeSpan = configuration.ReIssueTimeSpan;
          return reIssueTimeSpan.HasValue && timeSpan >= reIssueTimeSpan.GetValueOrDefault();
        }
      }
      return false;
    }

    private UserAuthenticationConfiguration GetConfiguration(IVssRequestContext requestContext)
    {
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      return vssRequestContext.GetService<IUserAuthenticationConfigurationService>().GetConfiguration(vssRequestContext);
    }

    internal bool IsUserAuthenticationDisabled(
      IVssRequestContext requestContext,
      HttpContextBase httpContext,
      UserAuthenticationSessionToken token = null)
    {
      if (requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment) && requestContext.ServiceInstanceType() == ServiceInstanceTypes.SPS)
      {
        Guid hostId;
        if (this.TryGetHostIdFromSignInRequest(httpContext.Request, out hostId))
          return this.disableUserAuthConfig.QueryByToken<bool>(requestContext, hostId, token?.SecurityToken);
        if (this.TryGetHostIdFromSignedInRequest(requestContext, httpContext.Request, out hostId))
          return this.disableUserAuthConfig.QueryByToken<bool>(requestContext, hostId, token?.SecurityToken);
      }
      return this.disableUserAuthConfig.QueryByToken<bool>(requestContext, token?.SecurityToken);
    }

    internal bool IsUserAuthenticationDisabled(
      IVssRequestContext requestContext,
      JwtSecurityToken token)
    {
      return this.disableUserAuthConfig.QueryByToken<bool>(requestContext, token);
    }

    private bool TryGetHostIdFromSignInRequest(HttpRequestBase httpRequest, out Guid hostId)
    {
      hostId = Guid.Empty;
      return string.Equals(httpRequest.HttpMethod, "GET", StringComparison.OrdinalIgnoreCase) && httpRequest.Url.AbsolutePath.EndsWith("/_signin", StringComparison.OrdinalIgnoreCase) && Guid.TryParse(httpRequest.QueryString["hid"], out hostId);
    }

    private bool TryGetHostIdFromSignedInRequest(
      IVssRequestContext requestContext,
      HttpRequestBase httpRequest,
      out Guid hostId)
    {
      hostId = Guid.Empty;
      string empty = string.Empty;
      if (AuthenticationHelpers.IsSignedInRequest(httpRequest))
        empty = AadAuthUrlUtility.ParseState(requestContext)["hid"];
      return Guid.TryParse(empty, out hostId);
    }

    internal static class ClaimTypeMapping
    {
      private static Dictionary<string, string> s_longToShortClaimTypeMapping = new Dictionary<string, string>()
      {
        {
          "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress",
          "email"
        },
        {
          "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/surname",
          "family_name"
        },
        {
          "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/givenname",
          "given_name"
        },
        {
          "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier",
          "sub"
        },
        {
          "http://schemas.microsoft.com/identity/claims/tenantid",
          "tid"
        },
        {
          "http://schemas.microsoft.com/identity/claims/objectidentifier",
          "oid"
        },
        {
          "PUID",
          "puid"
        },
        {
          "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name",
          "unique_name"
        },
        {
          "IsClient",
          "is_client"
        },
        {
          "CspPartner",
          "CspPartner"
        },
        {
          "prov_data",
          "prov_data"
        }
      };
      private static Dictionary<string, string> s_shortToLongClaimTypeMapping = new Dictionary<string, string>();

      static ClaimTypeMapping()
      {
        foreach (KeyValuePair<string, string> keyValuePair in UserAuthenticationSessionTokenProvider.ClaimTypeMapping.s_longToShortClaimTypeMapping)
        {
          if (!UserAuthenticationSessionTokenProvider.ClaimTypeMapping.s_shortToLongClaimTypeMapping.ContainsKey(keyValuePair.Value))
            UserAuthenticationSessionTokenProvider.ClaimTypeMapping.s_shortToLongClaimTypeMapping.Add(keyValuePair.Value, keyValuePair.Key);
        }
      }

      public static string GetValidJwtClaimType(string claimType)
      {
        string str;
        return !UserAuthenticationSessionTokenProvider.ClaimTypeMapping.s_longToShortClaimTypeMapping.TryGetValue(claimType, out str) ? (string) null : str;
      }

      public static string GetValidLongClaimType(string jwtClaimType)
      {
        string str;
        return !UserAuthenticationSessionTokenProvider.ClaimTypeMapping.s_shortToLongClaimTypeMapping.TryGetValue(jwtClaimType, out str) ? (string) null : str;
      }
    }
  }
}
