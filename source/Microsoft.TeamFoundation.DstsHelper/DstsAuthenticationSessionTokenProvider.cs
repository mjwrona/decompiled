// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DstsHelper.DstsAuthenticationSessionTokenProvider
// Assembly: Microsoft.TeamFoundation.DstsHelper, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08D47267-3A15-4307-BBA0-1792E9C6BDF1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DstsHelper.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.Authentication;
using Microsoft.TeamFoundation.Framework.Server.Authentication.UserAuthentication;
using Microsoft.TeamFoundation.Framework.Server.ConfigFramework;
using Microsoft.TeamFoundation.Framework.Server.OAuth2;
using Microsoft.VisualStudio.Services.DelegatedAuthorization;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.Location.Server;
using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.VisualStudio.Services.WebApi.Jwt;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Web;

namespace Microsoft.TeamFoundation.DstsHelper
{
  internal class DstsAuthenticationSessionTokenProvider
  {
    private static readonly string s_area = "Authentication";
    private static readonly string s_layer = nameof (DstsAuthenticationSessionTokenProvider);
    internal const string RequestContextKeyName = "UserAuthenticationToken";
    public const string FedAuthShimIdentifier = "UserAuthenticationToken";
    private const string confPropertyPath = "Identity.UserAuthenticationSessionToken.PreserveValidFromOnRefresh";
    private static readonly IConfigPrototype<bool> preserveValidFromConfigPrototype = ConfigPrototype.Create<bool>("Identity.UserAuthenticationSessionToken.PreserveValidFromOnRefresh", false);
    private readonly IConfigQueryable<bool> preserveValidFromConfig;

    public DstsAuthenticationSessionTokenProvider()
      : this(ConfigProxy.Create<bool>(DstsAuthenticationSessionTokenProvider.preserveValidFromConfigPrototype))
    {
    }

    public DstsAuthenticationSessionTokenProvider(IConfigQueryable<bool> config) => this.preserveValidFromConfig = config;

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
        if (authenticationSessionToken2.AuthenticationMechanism == AuthenticationMechanism.UserAuthToken)
          requestContext.GetService<ITeamFoundationAuthenticationService>().AuthenticationServiceInternal().ReissueFedAuthToken(requestContext);
        if (this.preserveValidFromConfig.QueryByCtx<bool>(requestContext))
        {
          requestContext.Trace(17002, TraceLevel.Info, DstsAuthenticationSessionTokenProvider.s_area, DstsAuthenticationSessionTokenProvider.s_layer, "Preserving ValidFrom property between UserAuth token renewals");
          authenticationSessionToken2 = this.IssueSessionToken(requestContext, context, authenticationSessionToken2.User, authenticationSessionToken2.AuthenticationMechanism, currentToken: authenticationSessionToken2);
        }
        else
        {
          requestContext.Trace(17003, TraceLevel.Info, DstsAuthenticationSessionTokenProvider.s_area, DstsAuthenticationSessionTokenProvider.s_layer, "Issuing UserAuth session token from current timestamp");
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
      Microsoft.IdentityModel.Tokens.SecurityToken fedAuthToken,
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
      VssSigningCredentials signingCredentials = vssRequestContext.GetService<IDelegatedAuthorizationConfigurationService>().GetSigningCredentials(vssRequestContext, true);
      string issuer = configuration.Issuer ?? this.GetIssuerFallback(requestContext);
      Dictionary<string, string> dictionary1 = new Dictionary<string, string>();
      Dictionary<string, string> dictionary2 = new Dictionary<string, string>();
      dictionary1.TryAdd<string, string>("ver", configuration.CurrentVersion);
      dictionary1.TryAdd<string, string>("jti", Guid.NewGuid().ToString("D"));
      foreach (Claim mergeClaim in AuthenticationHelpers.MergeClaims(claimsPrincipal.Claims, additionalClaims))
      {
        if (!(mergeClaim.Type == "prov_data") || ProvDataClaimHelper.HasGitHubClaim(ProvDataClaimHelper.ToProviderDataStub(requestContext, mergeClaim)))
        {
          string validJwtClaimType = DstsAuthenticationSessionTokenProvider.ClaimTypeMapping.GetValidJwtClaimType(mergeClaim.Type);
          if (validJwtClaimType != null)
            dictionary1.TryAdd<string, string>(validJwtClaimType, mergeClaim.Value);
          else if (DstsAuthenticationSessionTokenProvider.ClaimTypeMapping.GetValidLongClaimType(mergeClaim.Type) != null)
            dictionary2.TryAdd<string, string>(mergeClaim.Type, mergeClaim.Value);
        }
      }
      dictionary1.TryAddRange<string, string, Dictionary<string, string>>((IEnumerable<KeyValuePair<string, string>>) dictionary2);
      JsonWebToken jsonWebToken = JsonWebToken.Create(issuer, configuration.Audience, tokenValidFrom ?? configuration.TimeProvider.Now, configuration.TimeProvider.Now.Add(configuration.ExpireTimeSpan), dictionary1.Select<KeyValuePair<string, string>, Claim>((Func<KeyValuePair<string, string>, Claim>) (item => new Claim(item.Key, item.Value))), signingCredentials);
      return new UserAuthenticationSessionToken(claimsPrincipal, new JwtSecurityToken(jsonWebToken.EncodedToken), authenticationMechanism);
    }

    public void WriteTokenToCookie(
      IVssRequestContext requestContext,
      HttpContextBase httpContext,
      UserAuthenticationSessionToken token)
    {
      UserAuthenticationConfiguration configuration = this.GetConfiguration(requestContext);
      DateTime validTo = token.SecurityToken.ValidTo;
      string domain = (string) null;
      if (token.AuthenticationMechanism == AuthenticationMechanism.UserAuthToken)
      {
        HttpCookie secureCookie = this.CreateSecureCookie(configuration.CookieName, token.SecurityToken.RawData, validTo, domain);
        httpContext.Response.Cookies.Set(secureCookie);
      }
      else if (token.AuthenticationMechanism == AuthenticationMechanism.UserAuthToken_VS2012)
      {
        HttpCookie secureCookie1 = this.CreateSecureCookie("FedAuth", "UserAuthenticationToken", validTo, domain);
        HttpCookie secureCookie2 = this.CreateSecureCookie("FedAuth1", token.SecurityToken.RawData, validTo, domain);
        httpContext.Response.Cookies.Set(secureCookie1);
        httpContext.Response.Cookies.Set(secureCookie2);
      }
      httpContext.Response.Cache.SetCacheability(HttpCacheability.NoCache);
    }

    public bool DeleteSessionToken(IVssRequestContext requestContext, HttpContextBase httpContext)
    {
      UserAuthenticationConfiguration configuration = this.GetConfiguration(requestContext);
      DateTime expires = DateTime.UtcNow.AddDays(-1.0);
      string domain = (string) null;
      bool flag = false;
      if (httpContext.Request.Cookies[configuration.CookieName] != null)
      {
        HttpCookie secureCookie = this.CreateSecureCookie(configuration.CookieName, (string) null, expires, domain);
        httpContext.Response.Cookies.Set(secureCookie);
        requestContext.RootContext.Items.Remove("UserAuthenticationToken");
        flag = true;
      }
      if (httpContext.Request.Cookies["FedAuth"]?.Value == "UserAuthenticationToken")
      {
        HttpCookie secureCookie1 = this.CreateSecureCookie("FedAuth", (string) null, expires, domain);
        HttpCookie secureCookie2 = this.CreateSecureCookie("FedAuth1", (string) null, expires, domain);
        httpContext.Response.Cookies.Set(secureCookie1);
        httpContext.Response.Cookies.Set(secureCookie2);
        flag = true;
      }
      return flag;
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
          requestContext.Trace(828620, TraceLevel.Info, DstsAuthenticationSessionTokenProvider.s_area, DstsAuthenticationSessionTokenProvider.s_layer, "UserAuthenticationCookie present in VS2012 sign in request.");
        }
      }
      if (cookie1 != null)
        return cookie1.Value;
      requestContext.Trace(828619, TraceLevel.Info, DstsAuthenticationSessionTokenProvider.s_area, DstsAuthenticationSessionTokenProvider.s_layer, "AuthenticationCookie: Cookie was not present in request.");
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
      JwtSecurityToken jwtToken;
      bool impersonating;
      bool validIdentity;
      ClaimsPrincipal user;
      try
      {
        user = service.ValidateToken(vssRequestContext, encodedToken, OAuth2TokenValidators.UserAuthentication, out jwtToken, out impersonating, out validIdentity);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(248428, DstsAuthenticationSessionTokenProvider.s_area, DstsAuthenticationSessionTokenProvider.s_layer, ex);
        return (UserAuthenticationSessionToken) null;
      }
      if (!validIdentity)
      {
        requestContext.Trace(288525, TraceLevel.Error, DstsAuthenticationSessionTokenProvider.s_area, DstsAuthenticationSessionTokenProvider.s_layer, "AuthenticationCookie: JWT token was not valid.");
        return (UserAuthenticationSessionToken) null;
      }
      if (!impersonating)
        return new UserAuthenticationSessionToken(user, jwtToken, authenticationMechanism);
      requestContext.Trace(920898, TraceLevel.Error, DstsAuthenticationSessionTokenProvider.s_area, DstsAuthenticationSessionTokenProvider.s_layer, "AuthenticationCookie: Impersonation not permitted with cookie auth.");
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
            requestContext.Trace(17004, TraceLevel.Info, DstsAuthenticationSessionTokenProvider.s_area, DstsAuthenticationSessionTokenProvider.s_layer, "Determining ShouldRefresh based on ValidTo");
            dateTime = token.SecurityToken.ValidTo.Subtract(configuration.ExpireTimeSpan);
          }
          else
          {
            requestContext.Trace(17005, TraceLevel.Info, DstsAuthenticationSessionTokenProvider.s_area, DstsAuthenticationSessionTokenProvider.s_layer, "Determining ShouldRefresh based on ValidFrom");
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
        },
        {
          "http://schemas.microsoft.com/ws/2008/06/identity/claims/windowsaccountname",
          "winaccountname"
        },
        {
          "onprem_sid",
          "onprem_sid"
        },
        {
          "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/upn",
          "upn"
        }
      };
      private static Dictionary<string, string> s_shortToLongClaimTypeMapping = new Dictionary<string, string>();

      static ClaimTypeMapping()
      {
        foreach (KeyValuePair<string, string> keyValuePair in DstsAuthenticationSessionTokenProvider.ClaimTypeMapping.s_longToShortClaimTypeMapping)
        {
          if (!DstsAuthenticationSessionTokenProvider.ClaimTypeMapping.s_shortToLongClaimTypeMapping.ContainsKey(keyValuePair.Value))
            DstsAuthenticationSessionTokenProvider.ClaimTypeMapping.s_shortToLongClaimTypeMapping.Add(keyValuePair.Value, keyValuePair.Key);
        }
      }

      public static string GetValidJwtClaimType(string claimType)
      {
        string str;
        return !DstsAuthenticationSessionTokenProvider.ClaimTypeMapping.s_longToShortClaimTypeMapping.TryGetValue(claimType, out str) ? (string) null : str;
      }

      public static string GetValidLongClaimType(string jwtClaimType)
      {
        string str;
        return !DstsAuthenticationSessionTokenProvider.ClaimTypeMapping.s_shortToLongClaimTypeMapping.TryGetValue(jwtClaimType, out str) ? (string) null : str;
      }
    }
  }
}
