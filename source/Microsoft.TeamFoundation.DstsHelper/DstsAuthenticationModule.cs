// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DstsHelper.DstsAuthenticationModule
// Assembly: Microsoft.TeamFoundation.DstsHelper, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08D47267-3A15-4307-BBA0-1792E9C6BDF1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DstsHelper.dll

using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.Authentication;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Security.Principal;
using System.Web;

namespace Microsoft.TeamFoundation.DstsHelper
{
  internal class DstsAuthenticationModule : VssfAuthenticationHttpModuleBase
  {
    private const string PendingSessionIdCookieName = "dstsAuth-PendingSessionId";
    private const string ModuleAndOidcConfigItemName = "dstsAuth-ModuleAndOidcConfig";
    private const string SignInPath = "/_signin";
    private const string SignedInPath = "/_signedin";
    private const string SignOutPath = "/_signout";
    private static readonly OpenIdConnectConfigurationRetriever OidcConfigRetriever = new OpenIdConnectConfigurationRetriever();
    private static readonly ConcurrentDictionary<string, ConfigurationManager<OpenIdConnectConfiguration>> OidcConfigManagers = new ConcurrentDictionary<string, ConfigurationManager<OpenIdConnectConfiguration>>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    private static readonly DstsAuthenticationSessionTokenProvider AuthTokenProvider = new DstsAuthenticationSessionTokenProvider();
    private static readonly string HiddenPIIString = string.Format((IFormatProvider) CultureInfo.InvariantCulture, IdentityModelEventSource.HiddenPIIString, (object) typeof (string).ToString());
    private const string _SecurityToken = "SecurityToken";
    private const string _Exception = "Exception";
    private const string BearerWithSpace = "Bearer ";
    private const string s_area = "DstsAuthenticationModule";
    private const string s_layer = "Module";

    private static ClaimsPrincipal ValidateBearerToken(
      IVssRequestContext requestContext,
      OpenIdConnectConfiguration oidcConfig,
      string bearerToken,
      string validAudience,
      out Microsoft.IdentityModel.Tokens.SecurityToken validatedToken,
      out string errorDescription)
    {
      JwtSecurityTokenHandler securityTokenHandler = new JwtSecurityTokenHandler()
      {
        MapInboundClaims = false
      };
      TokenValidationParameters validationParameters = new TokenValidationParameters()
      {
        ValidateLifetime = true,
        ValidateIssuer = true,
        ValidIssuer = oidcConfig.Issuer,
        ValidateAudience = !string.IsNullOrEmpty(validAudience),
        ValidAudience = validAudience,
        ValidateIssuerSigningKey = true,
        IssuerSigningKeys = (IEnumerable<SecurityKey>) oidcConfig.SigningKeys
      };
      errorDescription = (string) null;
      try
      {
        if (!(new JwtSecurityToken(bearerToken).Issuer != oidcConfig.Issuer))
          return securityTokenHandler.ValidateToken(bearerToken, validationParameters, out validatedToken);
        validatedToken = (Microsoft.IdentityModel.Tokens.SecurityToken) null;
        return (ClaimsPrincipal) null;
      }
      catch (ArgumentException ex)
      {
        string str = DstsAuthenticationModule.ReplaceHiddenPIIString(ex.Message);
        requestContext.Trace(0, TraceLevel.Info, nameof (DstsAuthenticationModule), "Module", "Bearer Token is in unexpected format. {0}", (object) str);
        errorDescription = str;
        validatedToken = (Microsoft.IdentityModel.Tokens.SecurityToken) null;
        return (ClaimsPrincipal) null;
      }
      catch (SecurityTokenValidationException ex)
      {
        string str = DstsAuthenticationModule.ReplaceHiddenPIIString(ex.Message);
        requestContext.Trace(0, TraceLevel.Info, nameof (DstsAuthenticationModule), "Module", "Bearer Token is invalid. {0}", (object) str);
        errorDescription = !(ex.GetType() == typeof (SecurityTokenValidationException)) ? DstsAuthenticationModule.GetErrorCodeFromSecurityTokenExceptionType(ex.GetType()) ?? str : str;
        validatedToken = (Microsoft.IdentityModel.Tokens.SecurityToken) null;
        return (ClaimsPrincipal) null;
      }
      catch (SecurityTokenException ex)
      {
        string str = DstsAuthenticationModule.ReplaceHiddenPIIString(ex.Message);
        requestContext.Trace(0, TraceLevel.Info, nameof (DstsAuthenticationModule), "Module", "Failed to handle Bearer Token. {0}", (object) str);
        errorDescription = !(ex.GetType() == typeof (SecurityTokenException)) ? DstsAuthenticationModule.GetErrorCodeFromSecurityTokenExceptionType(ex.GetType()) ?? str : str;
        validatedToken = (Microsoft.IdentityModel.Tokens.SecurityToken) null;
        return (ClaimsPrincipal) null;
      }
    }

    private static string GetErrorCodeFromExceptionType(Type type)
    {
      string name = type.Name;
      return name.Length > "Exception".Length && name.EndsWith("Exception", StringComparison.OrdinalIgnoreCase) ? name.Remove(name.Length - "Exception".Length) : (string) null;
    }

    private static string GetErrorCodeFromSecurityTokenExceptionType(Type type)
    {
      string name = type.Name;
      return name.Length > "SecurityToken".Length + "Exception".Length && name.StartsWith("SecurityToken", StringComparison.OrdinalIgnoreCase) && name.EndsWith("Exception", StringComparison.OrdinalIgnoreCase) ? name.Substring("SecurityToken".Length, name.Length - "SecurityToken".Length - "Exception".Length) : (string) null;
    }

    private static string ReplaceHiddenPIIString(string str) => IdentityModelEventSource.ShowPII ? str : str.Replace(DstsAuthenticationModule.HiddenPIIString, "[HiddenPII]");

    private static string GetBearerToken(HttpRequestBase request)
    {
      string header = request.Headers["Authorization"];
      if (header == null)
        return (string) null;
      return !header.StartsWith("Bearer ") ? (string) null : header.Substring("Bearer ".Length);
    }

    private static OpenIdConnectConfiguration GetOpenIdConnectConfig(string authority) => DstsAuthenticationModule.OidcConfigManagers.GetOrAdd(authority, (Func<string, ConfigurationManager<OpenIdConnectConfiguration>>) (au => new ConfigurationManager<OpenIdConnectConfiguration>(au.TrimEnd('/') + "/.well-known/openid-configuration", (IConfigurationRetriever<OpenIdConnectConfiguration>) DstsAuthenticationModule.OidcConfigRetriever))).GetConfigurationAsync().GetAwaiter().GetResult();

    private static bool TryGetModuleAndOidcConfig(
      IVssRequestContext requestContext,
      out ModuleConfig moduleConfig,
      out OpenIdConnectConfiguration oidcConfig)
    {
      moduleConfig = (ModuleConfig) null;
      oidcConfig = (OpenIdConnectConfiguration) null;
      object obj;
      if (requestContext.Items.TryGetValue("dstsAuth-ModuleAndOidcConfig", out obj))
      {
        if (obj == null || !(obj is DstsAuthenticationModule.ModuleAndOidcConfig moduleAndOidcConfig))
          return false;
        moduleConfig = moduleAndOidcConfig.ModuleConfig;
        oidcConfig = moduleAndOidcConfig.OidcConfig;
        return oidcConfig != null;
      }
      try
      {
        moduleConfig = ModuleConfig.Load(requestContext.To(TeamFoundationHostType.Deployment));
      }
      catch (Exception ex)
      {
        TeamFoundationTracingService.TraceExceptionRaw(0, TraceLevel.Error, nameof (DstsAuthenticationModule), "Module", ex, "Failed to load dSTS Auth Module Config.");
        requestContext.Items["dstsAuth-ModuleAndOidcConfig"] = (object) null;
        return false;
      }
      try
      {
        oidcConfig = DstsAuthenticationModule.GetOpenIdConnectConfig(moduleConfig.Authority);
      }
      catch (Exception ex)
      {
        TeamFoundationTracingService.TraceExceptionRaw(0, TraceLevel.Error, nameof (DstsAuthenticationModule), "Module", ex, "Failed to retrieve dSTS Auth OpenId Connect Config.");
      }
      DstsAuthenticationModule.ModuleAndOidcConfig moduleAndOidcConfig1 = new DstsAuthenticationModule.ModuleAndOidcConfig()
      {
        ModuleConfig = moduleConfig,
        OidcConfig = oidcConfig
      };
      requestContext.Items["dstsAuth-ModuleAndOidcConfig"] = (object) moduleAndOidcConfig1;
      return oidcConfig != null;
    }

    private static void RedirectToDstsAuthorizationEndpoint(
      IVssRequestContext requestContext,
      ModuleConfig moduleConfig,
      OpenIdConnectConfiguration oidcConfig,
      HttpRequestBase request,
      HttpResponseBase response,
      string reply_to,
      bool selectAccount)
    {
      string str1 = Uri.EscapeDataString(moduleConfig.ServerId);
      string str2 = Uri.EscapeDataString(new Uri(request.Url, "/_signedin").ToString());
      HttpCookie cookie1 = request.Cookies["dstsAuth-PendingSessionId"];
      Guid result;
      if (cookie1 == null || !Guid.TryParse(cookie1.Value, out result))
        result = Guid.NewGuid();
      string str3 = Uri.EscapeDataString("reply_to=" + Uri.EscapeDataString(reply_to));
      string url = string.Format("{0}?client_id={1}&response_mode=form_post&response_type=code+id_token&redirect_uri={2}&nonce={3}&state={4}&resource={5}", (object) oidcConfig.AuthorizationEndpoint, (object) str1, (object) str2, (object) result, (object) str3, (object) str1);
      if (selectAccount)
        url += "&prompt=select_account";
      HttpCookie cookie2 = new HttpCookie("dstsAuth-PendingSessionId", result.ToString())
      {
        Expires = DateTime.UtcNow.AddDays(1.0),
        HttpOnly = true,
        Secure = true
      };
      CookieModifier.AddSameSiteNoneToCookie(requestContext, cookie2);
      response.Cookies.Set(cookie2);
      response.Redirect(url, false);
    }

    private static void RedirectToDstsEndSessionEndpoint(
      OpenIdConnectConfiguration oidcConfig,
      HttpRequestBase request,
      HttpResponseBase response,
      string reply_to)
    {
      string endSessionEndpoint = oidcConfig.EndSessionEndpoint;
      response.Redirect(endSessionEndpoint, false);
    }

    private static string NormalizeReplyTo(string reply_to)
    {
      if (string.IsNullOrEmpty(reply_to))
        return "/";
      reply_to = reply_to.TrimStart('/', '\\');
      if (reply_to == string.Empty)
        return "/";
      if (reply_to[0] != '/')
        reply_to = "/" + reply_to;
      return reply_to.Equals("/_signin", StringComparison.OrdinalIgnoreCase) || reply_to.Equals("/_signout", StringComparison.OrdinalIgnoreCase) ? "/" : reply_to;
    }

    private static string GetIdentityName(ClaimsPrincipal claims)
    {
      Claim claim1 = claims.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name") ?? claims.FindFirst("unique_name");
      Claim claim2 = claims.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/upn") ?? claims.FindFirst("upn");
      if (claim2 == null || string.IsNullOrEmpty(claim2.Value))
        return claim1?.Value;
      if (claim1 == null || string.IsNullOrEmpty(claim1.Value))
        return claim2.Value;
      return claim1.Value == claim2.Value ? claim1.Value : claim1.Value + " (" + claim2.Value + ")";
    }

    private static void SetAlternateAuthFlagByScope(
      IVssRequestContext requestContext,
      ClaimsPrincipal claims)
    {
      Claim claim = claims.FindFirst("http://schemas.microsoft.com/identity/claims/scope") ?? claims.FindFirst("scp");
      if (claim == null || !claim.Value.Equals("user_impersonation", StringComparison.OrdinalIgnoreCase))
        return;
      requestContext.To(TeamFoundationHostType.Deployment).Items[RequestContextItemsKeys.AlternateAuthCredentialsContextKey] = (object) true;
    }

    private static void HandleAuthedSignInRequest(
      HttpContextBase httpContext,
      ModuleConfig moduleConfig)
    {
      HttpRequestBase request = httpContext.Request;
      if (!DstsAuthenticationModule.CanRedirectToDsts(request.Url, moduleConfig.RedirectAuthorities) && moduleConfig.BaseUrlForOtherAuthority == (Uri) null)
        return;
      HttpResponseBase response = httpContext.Response;
      response.Cache.SetCacheability(HttpCacheability.NoCache);
      response.Redirect(new Uri(request.Url, DstsAuthenticationModule.NormalizeReplyTo(request.QueryString["reply_to"])).ToString(), false);
    }

    private static void HandleUnauthedSignInRequest(
      IVssRequestContext requestContext,
      HttpContextBase httpContext,
      ModuleConfig moduleConfig,
      OpenIdConnectConfiguration oidcConfig)
    {
      HttpRequestBase request = httpContext.Request;
      bool dsts = DstsAuthenticationModule.CanRedirectToDsts(request.Url, moduleConfig.RedirectAuthorities);
      if (!dsts && moduleConfig.BaseUrlForOtherAuthority == (Uri) null)
        return;
      HttpResponseBase response = httpContext.Response;
      response.Cache.SetCacheability(HttpCacheability.NoCache);
      if (!dsts)
        response.Redirect(new Uri(moduleConfig.BaseUrlForOtherAuthority, request.Url.PathAndQuery).ToString(), false);
      else
        DstsAuthenticationModule.RedirectToDstsAuthorizationEndpoint(requestContext, moduleConfig, oidcConfig, request, response, DstsAuthenticationModule.NormalizeReplyTo(request.QueryString["reply_to"]), false);
    }

    private static void HandleUnauthedRegularRequest(
      IVssRequestContext requestContext,
      HttpContextBase httpContext,
      ModuleConfig moduleConfig,
      OpenIdConnectConfiguration oidcConfig)
    {
      HttpRequestBase request = httpContext.Request;
      bool dsts = DstsAuthenticationModule.CanRedirectToDsts(request.Url, moduleConfig.RedirectAuthorities);
      if (!dsts && moduleConfig.BaseUrlForOtherAuthority == (Uri) null || !DstsAuthenticationModule.ShouldRedirectToDsts(request))
        return;
      HttpResponseBase response = httpContext.Response;
      response.Cache.SetCacheability(HttpCacheability.NoCache);
      if (!dsts)
        response.Redirect(new Uri(moduleConfig.BaseUrlForOtherAuthority, request.Url.PathAndQuery).ToString(), false);
      else
        response.Redirect(new Uri(request.Url, "/_signin?reply_to=" + Uri.EscapeDataString(request.Url.PathAndQuery)).ToString(), false);
    }

    private static void HandleSignedInRequest(
      IVssRequestContext requestContext,
      HttpContextBase httpContext,
      ModuleConfig moduleConfig,
      OpenIdConnectConfiguration oidcConfig)
    {
      HttpRequestBase request = httpContext.Request;
      string bearerToken = request.Form["id_token"];
      if (string.IsNullOrEmpty(bearerToken))
        return;
      ClaimsPrincipal claimsPrincipal;
      try
      {
        string errorDescription;
        claimsPrincipal = DstsAuthenticationModule.ValidateBearerToken(requestContext, oidcConfig, bearerToken, moduleConfig.ServerId, out Microsoft.IdentityModel.Tokens.SecurityToken _, out errorDescription);
        if (errorDescription != null)
        {
          DstsAuthenticationModule.CompleteRequest(HttpStatusCode.BadRequest, errorDescription);
          return;
        }
        if (claimsPrincipal == null)
          return;
        Claim first = claimsPrincipal.FindFirst("nonce");
        if (first == null || string.IsNullOrEmpty(first.Value))
        {
          DstsAuthenticationModule.CompleteRequest(HttpStatusCode.BadRequest, "Id token has no claim nonce.");
          return;
        }
        HttpCookie cookie = request.Cookies["dstsAuth-PendingSessionId"];
        if (cookie != null)
        {
          if (first.Value.Equals(cookie.Value, StringComparison.OrdinalIgnoreCase))
            goto label_11;
        }
        DstsAuthenticationModule.CompleteRequest(HttpStatusCode.BadRequest, "Found mismatch between token nonce and session cookie nonce.");
        return;
      }
      catch (Exception ex)
      {
        TeamFoundationTracingService.TraceExceptionRaw(0, TraceLevel.Error, nameof (DstsAuthenticationModule), "Module", ex, "Failed to validate Id Token.");
        string fromExceptionType = DstsAuthenticationModule.GetErrorCodeFromExceptionType(ex.GetType());
        DstsAuthenticationModule.CompleteRequest(HttpStatusCode.InternalServerError, fromExceptionType == null ? "Internal Server Error" : "Internal Server Error: " + fromExceptionType);
        return;
      }
label_11:
      HttpResponseBase response = httpContext.Response;
      HttpCookie cookie1 = new HttpCookie("dstsAuth-PendingSessionId")
      {
        Expires = DateTime.UtcNow.AddDays(-1.0),
        HttpOnly = true,
        Secure = true
      };
      CookieModifier.AddSameSiteNoneToCookie(requestContext, cookie1);
      response.Cookies.Set(cookie1);
      DstsAuthenticationModule.AuthTokenProvider.IssueSessionToken(requestContext, httpContext, claimsPrincipal);
      string relativeUri = "/";
      string query = request.Form["state"];
      if (!string.IsNullOrEmpty(query))
        relativeUri = DstsAuthenticationModule.NormalizeReplyTo(HttpUtility.ParseQueryString(query)["reply_to"]);
      response.Cache.SetCacheability(HttpCacheability.NoCache);
      response.Redirect(new Uri(request.Url, relativeUri).ToString(), false);
      AuthenticationHelpers.SetAuthenticationMechanism(requestContext, AuthenticationMechanism.DSTS_Web);
      Microsoft.TeamFoundation.Framework.Server.HttpApplicationFactory.Current.CompleteRequest();
    }

    private static void HandleSignOutRequest(
      IVssRequestContext requestContext,
      HttpContextBase httpContext,
      ModuleConfig moduleConfig,
      OpenIdConnectConfiguration oidcConfig)
    {
      bool flag1 = DstsAuthenticationModule.AuthTokenProvider.DeleteSessionToken(requestContext, httpContext);
      if (!moduleConfig.PassiveAuthEnabled)
        return;
      HttpRequestBase request = httpContext.Request;
      bool dsts = DstsAuthenticationModule.CanRedirectToDsts(request.Url, moduleConfig.RedirectAuthorities);
      if (!dsts && moduleConfig.BaseUrlForOtherAuthority == (Uri) null)
        return;
      bool flag2 = string.Equals(request.QueryString["mode"], "SignInAsDifferentUser", StringComparison.OrdinalIgnoreCase);
      HttpResponseBase response = httpContext.Response;
      response.Cache.SetCacheability(HttpCacheability.NoCache);
      if (!dsts)
        response.Redirect(new Uri(moduleConfig.BaseUrlForOtherAuthority, request.Url.PathAndQuery).ToString(), false);
      else if (!flag1 && !flag2)
      {
        response.StatusCode = 204;
      }
      else
      {
        string reply_to = DstsAuthenticationModule.NormalizeReplyTo(request.QueryString["redirectUrl"]);
        if (flag2)
          DstsAuthenticationModule.RedirectToDstsAuthorizationEndpoint(requestContext, moduleConfig, oidcConfig, request, response, reply_to, true);
        else
          DstsAuthenticationModule.RedirectToDstsEndSessionEndpoint(oidcConfig, request, response, reply_to);
      }
      Microsoft.TeamFoundation.Framework.Server.HttpApplicationFactory.Current.CompleteRequest();
    }

    private static bool IsSignInRequest(HttpRequestBase request) => string.Equals(request.HttpMethod, "GET", StringComparison.OrdinalIgnoreCase) && request.Url.AbsolutePath.Equals("/_signin", StringComparison.OrdinalIgnoreCase);

    private static bool IsSignedInRequest(HttpRequestBase request) => string.Equals(request.HttpMethod, "POST", StringComparison.OrdinalIgnoreCase) && string.Equals(request.ContentType ?? string.Empty, "application/x-www-form-urlencoded", StringComparison.OrdinalIgnoreCase) && request.Url.AbsolutePath.Equals("/_signedin", StringComparison.OrdinalIgnoreCase);

    private static bool IsSignOutRequest(HttpRequestBase request) => string.Equals(request.HttpMethod, "GET", StringComparison.OrdinalIgnoreCase) && request.Url.AbsolutePath.Equals("/_signout", StringComparison.OrdinalIgnoreCase);

    public override void OnAuthenticateRequest(
      IVssRequestContext requestContext,
      HttpContextBase httpContext,
      EventArgs eventArgs)
    {
      requestContext.TraceEnter(908756, nameof (DstsAuthenticationModule), "Module", "DstsAuthenticationModule.OnAuthenticateRequest");
      try
      {
        ModuleConfig moduleConfig;
        OpenIdConnectConfiguration oidcConfig;
        if (!DstsAuthenticationModule.TryGetModuleAndOidcConfig(requestContext, out moduleConfig, out oidcConfig))
          return;
        HttpRequestBase request = httpContext.Request;
        if (DstsAuthenticationModule.IsSignedInRequest(request))
          DstsAuthenticationModule.HandleSignedInRequest(requestContext, httpContext, moduleConfig, oidcConfig);
        else if (DstsAuthenticationModule.IsSignOutRequest(request))
        {
          DstsAuthenticationModule.HandleSignOutRequest(requestContext, httpContext, moduleConfig, oidcConfig);
        }
        else
        {
          if (requestContext.RequestRestrictions().RequiredAuthentication == RequiredAuthentication.Anonymous)
            return;
          string bearerToken = DstsAuthenticationModule.GetBearerToken(request);
          ClaimsPrincipal claims;
          AuthenticationMechanism authenticationMechanism;
          if (bearerToken == null)
          {
            try
            {
              UserAuthenticationSessionToken authenticationSessionToken = DstsAuthenticationModule.AuthTokenProvider.ReadSessionToken(requestContext, httpContext);
              if (authenticationSessionToken == null)
                return;
              claims = authenticationSessionToken.User;
              authenticationMechanism = authenticationSessionToken.AuthenticationMechanism;
            }
            catch (Exception ex)
            {
              TeamFoundationTracingService.TraceExceptionRaw(0, TraceLevel.Error, nameof (DstsAuthenticationModule), "Module", ex, "Failed to validate Session Cookie.");
              string fromExceptionType = DstsAuthenticationModule.GetErrorCodeFromExceptionType(ex.GetType());
              DstsAuthenticationModule.CompleteRequest(HttpStatusCode.InternalServerError, fromExceptionType == null ? "Internal Server Error" : "Internal Server Error: " + fromExceptionType);
              return;
            }
          }
          else
          {
            try
            {
              string errorDescription;
              claims = DstsAuthenticationModule.ValidateBearerToken(requestContext, oidcConfig, bearerToken, moduleConfig.ServerId, out Microsoft.IdentityModel.Tokens.SecurityToken _, out errorDescription);
              if (errorDescription != null)
              {
                DstsAuthenticationModule.CompleteInvalidRequest(requestContext, httpContext, moduleConfig, oidcConfig, errorDescription);
                return;
              }
              if (claims == null)
                return;
              authenticationMechanism = AuthenticationMechanism.DSTS;
            }
            catch (Exception ex)
            {
              TeamFoundationTracingService.TraceExceptionRaw(0, TraceLevel.Error, nameof (DstsAuthenticationModule), "Module", ex, "Failed to validate Bearer Token.");
              string fromExceptionType = DstsAuthenticationModule.GetErrorCodeFromExceptionType(ex.GetType());
              DstsAuthenticationModule.CompleteRequest(HttpStatusCode.InternalServerError, fromExceptionType == null ? "Internal Server Error" : "Internal Server Error: " + fromExceptionType);
              return;
            }
          }
          IVssRequestContext vssRequestContext = requestContext.Elevate();
          Claim first = claims.FindFirst("onprem_sid");
          Microsoft.VisualStudio.Services.Identity.Identity identity;
          string additionalData;
          if (first != null && !string.IsNullOrEmpty(first.Value))
          {
            identity = DstsAuthenticationModule.ResolveEligibleActor(requestContext, vssRequestContext, IdentitySearchFilter.Identifier, first.Value);
            if (identity == null)
            {
              DstsAuthenticationModule.CompleteIdentityNotFoundRequest(requestContext, DstsAuthenticationModule.GetIdentityName(claims), "onprem_sid", first.Value);
              return;
            }
            additionalData = "onprem_sid:" + first.Value;
            DstsAuthenticationModule.SetAlternateAuthFlagByScope(requestContext, claims);
          }
          else
          {
            Claim claim1 = claims.FindFirst("http://schemas.microsoft.com/ws/2008/06/identity/claims/windowsaccountname") ?? claims.FindFirst("winaccountname");
            if (claim1 != null && !string.IsNullOrEmpty(claim1.Value))
            {
              identity = DstsAuthenticationModule.ResolveEligibleActor(requestContext, vssRequestContext, IdentitySearchFilter.AccountName, claim1.Value);
              if (identity == null)
              {
                DstsAuthenticationModule.CompleteIdentityNotFoundRequest(requestContext, DstsAuthenticationModule.GetIdentityName(claims), "winaccountname", claim1.Value);
                return;
              }
              additionalData = "winaccountname:" + claim1.Value;
              DstsAuthenticationModule.SetAlternateAuthFlagByScope(requestContext, claims);
            }
            else
            {
              Claim claim2 = claims.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier") ?? claims.FindFirst("oid");
              if (claim2 == null || string.IsNullOrEmpty(claim2.Value))
              {
                DstsAuthenticationModule.CompleteRequest(HttpStatusCode.Forbidden, "Required claims are missing.");
                return;
              }
              string accountNameByOid = ModuleConfig.GetAccountNameByOid(requestContext.To(TeamFoundationHostType.Deployment), claim2.Value);
              if (string.IsNullOrEmpty(accountNameByOid))
              {
                DstsAuthenticationModule.CompleteRequest(HttpStatusCode.Forbidden, "No identity is mapped by the claim oid.");
                return;
              }
              identity = DstsAuthenticationModule.ResolveEligibleActor(requestContext, vssRequestContext, IdentitySearchFilter.AccountName, accountNameByOid);
              if (identity == null)
              {
                DstsAuthenticationModule.CompleteIdentityNotFoundRequest(requestContext, accountNameByOid, "oid", claim2.Value);
                return;
              }
              additionalData = "oid:" + claim2.Value;
              requestContext.To(TeamFoundationHostType.Deployment).Items[RequestContextItemsKeys.AlternateAuthCredentialsContextKey] = (object) true;
            }
          }
          IdentityService service = vssRequestContext.GetService<IdentityService>();
          if (service.SyncAgents != null && !service.SyncAgents.ContainsKey(typeof (AlternateLoginIdentity).FullName))
            service.SyncAgents.Add(typeof (AlternateLoginIdentity).FullName, (IIdentityProvider) new AlternateLoginProvider());
          httpContext.User = (IPrincipal) new AlternateLoginPrincipal("Bearer", identity);
          AuthenticationHelpers.SetAuthenticationMechanism(requestContext, authenticationMechanism, additionalData);
        }
      }
      finally
      {
        requestContext.TraceLeave(213215, nameof (DstsAuthenticationModule), "Module", "DstsAuthenticationModule.OnAuthenticateRequest");
      }
    }

    public override void OnEndRequest(
      IVssRequestContext requestContext,
      HttpContextBase httpContext,
      EventArgs eventArgs)
    {
      requestContext.TraceEnter(311835, nameof (DstsAuthenticationModule), "Module", "DstsAuthenticationModule.OnEndRequest");
      try
      {
        if (httpContext.Response.StatusCode != 401)
        {
          ModuleConfig moduleConfig;
          if (!DstsAuthenticationModule.IsSignInRequest(httpContext.Request) || !DstsAuthenticationModule.TryGetModuleAndOidcConfig(requestContext, out moduleConfig, out OpenIdConnectConfiguration _) || !moduleConfig.PassiveAuthEnabled)
            return;
          DstsAuthenticationModule.HandleAuthedSignInRequest(httpContext, moduleConfig);
        }
        else
        {
          ModuleConfig moduleConfig;
          OpenIdConnectConfiguration oidcConfig;
          if (!DstsAuthenticationModule.TryGetModuleAndOidcConfig(requestContext, out moduleConfig, out oidcConfig))
            return;
          AuthenticationHelpers.SetWWWAuthenticateHeaderIfNotPresent(httpContext, "Bearer " + new AuthenticationMetadata(oidcConfig.Issuer, moduleConfig.Authority, moduleConfig.ServerId)?.ToString());
          if (!moduleConfig.PassiveAuthEnabled)
            return;
          if (DstsAuthenticationModule.IsSignInRequest(httpContext.Request))
            DstsAuthenticationModule.HandleUnauthedSignInRequest(requestContext, httpContext, moduleConfig, oidcConfig);
          else
            DstsAuthenticationModule.HandleUnauthedRegularRequest(requestContext, httpContext, moduleConfig, oidcConfig);
        }
      }
      finally
      {
        requestContext.TraceLeave(236250, nameof (DstsAuthenticationModule), "Module", "DstsAuthenticationModule.OnEndRequest");
      }
    }

    private static bool ShouldRedirectToDsts(HttpRequestBase request)
    {
      if (request.HttpMethod != "GET" || !string.IsNullOrEmpty(request.Headers["Authorization"]))
        return false;
      string absolutePath = request.Url.AbsolutePath;
      if (absolutePath.Equals("/_signin", StringComparison.OrdinalIgnoreCase) || absolutePath.Equals("/_signout", StringComparison.OrdinalIgnoreCase) || absolutePath.IndexOf("/_apis/", StringComparison.OrdinalIgnoreCase) >= 0)
        return false;
      string header = request.Headers["Accept"];
      return string.IsNullOrEmpty(header) || header.IndexOf("text/html", StringComparison.OrdinalIgnoreCase) >= 0 || header.IndexOf("text/*", StringComparison.OrdinalIgnoreCase) >= 0 || header.IndexOf("*/*", StringComparison.OrdinalIgnoreCase) >= 0;
    }

    private static bool CanRedirectToDsts(Uri requestUrl, ISet<string> redirectAuthorities) => redirectAuthorities == null || redirectAuthorities.Count == 0 || redirectAuthorities.Contains(requestUrl.Authority);

    private static void CompleteInvalidRequest(
      IVssRequestContext requestContext,
      HttpContextBase httpContext,
      ModuleConfig moduleConfig,
      OpenIdConnectConfiguration oidcConfig,
      string errorDescription)
    {
      AuthenticationHelpers.EnterMethodIfNull(requestContext, "Authentication", "DstsAuthenticationModule.CompleteInvalidRequest");
      HttpResponseBase response = httpContext.Response;
      response.Clear();
      response.StatusCode = 401;
      response.TrySkipIisCustomErrors = true;
      string str = "Bearer ";
      if (oidcConfig != null)
        str = str + new AuthenticationMetadata(oidcConfig.Issuer, moduleConfig.Authority, moduleConfig.ServerId)?.ToString() + ", ";
      string headerValue = str + new AuthenticationError("invalid_token", errorDescription)?.ToString();
      AuthenticationHelpers.SetWWWAuthenticateHeaderIfNotPresent(httpContext, headerValue);
      Microsoft.TeamFoundation.Framework.Server.HttpApplicationFactory.Current.CompleteRequest();
    }

    private static void CompleteRequest(HttpStatusCode statusCode, string errorDescription) => TeamFoundationApplicationCore.CompleteRequest(Microsoft.TeamFoundation.Framework.Server.HttpApplicationFactory.Current, statusCode, (string) null, (IEnumerable<KeyValuePair<string, string>>) null, (Exception) null, errorDescription, (string) null);

    private static void CompleteIdentityNotFoundRequest(
      IVssRequestContext requestContext,
      string identityName,
      string claimType,
      string claimValue)
    {
      requestContext.Trace(0, TraceLevel.Info, nameof (DstsAuthenticationModule), "Module", "The user " + identityName + " is either unauthorized or inactive. (Source Claim " + claimType + ": " + claimValue + ")");
      DstsAuthenticationModule.CompleteRequest(HttpStatusCode.Forbidden, "The user " + identityName + " is either unauthorized or inactive. Please grant permission or sign in as a different user.");
    }

    private static Microsoft.VisualStudio.Services.Identity.Identity ResolveEligibleActor(
      IVssRequestContext requestContext,
      IVssRequestContext elevatedContext,
      IdentitySearchFilter searchFactor,
      string factorValue)
    {
      IVssRequestContext vssRequestContext = elevatedContext.To(TeamFoundationHostType.Deployment);
      IList<Microsoft.VisualStudio.Services.Identity.Identity> source = vssRequestContext.GetService<IdentityService>().ReadIdentities(vssRequestContext, searchFactor, factorValue, QueryMembership.None, (IEnumerable<string>) null);
      if (source.Count == 0)
      {
        requestContext.Trace(1007064, TraceLevel.Info, nameof (DstsAuthenticationModule), "Module", "Couldn't find any identities with provided {0} '{1}'", (object) searchFactor, (object) factorValue);
        return (Microsoft.VisualStudio.Services.Identity.Identity) null;
      }
      try
      {
        source = (IList<Microsoft.VisualStudio.Services.Identity.Identity>) requestContext.GetService<IVssIdentityRetrievalService>().GetActiveUsers(elevatedContext, (IEnumerable<IdentityDescriptor>) source.Select<Microsoft.VisualStudio.Services.Identity.Identity, IdentityDescriptor>((Func<Microsoft.VisualStudio.Services.Identity.Identity, IdentityDescriptor>) (x => x.Descriptor)).ToList<IdentityDescriptor>()).Values.Where<Microsoft.VisualStudio.Services.Identity.Identity>((Func<Microsoft.VisualStudio.Services.Identity.Identity, bool>) (x => x != null)).ToList<Microsoft.VisualStudio.Services.Identity.Identity>();
      }
      catch (UnexpectedHostTypeException ex)
      {
      }
      Microsoft.VisualStudio.Services.Identity.Identity identity = (Microsoft.VisualStudio.Services.Identity.Identity) null;
      if (source.Count == 1)
        identity = source[0];
      else
        requestContext.Trace(1007061, TraceLevel.Error, nameof (DstsAuthenticationModule), "Module", "Find {0} identities with provided {1} '{2}'", (object) source.Count, (object) searchFactor, (object) factorValue);
      return identity;
    }

    private class ModuleAndOidcConfig
    {
      public ModuleConfig ModuleConfig;
      public OpenIdConnectConfiguration OidcConfig;
    }
  }
}
