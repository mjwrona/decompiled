// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.Authentication.AuthenticationHelpers
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IdentityModel.Services;
using System.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Web;

namespace Microsoft.TeamFoundation.Framework.Server.Authentication
{
  public static class AuthenticationHelpers
  {
    private const string s_area = "Authentication";
    private const string s_layer = "AuthenticationHelpers";
    private static int s_maxContentSize = 1024;
    private const string AuthStartString = "{\"__authToken\"";
    private const string BearerScheme = "Bearer";
    private static readonly HashSet<string> s_cookieBasedAuthenticationMechanisms = new HashSet<string>((IEqualityComparer<string>) StringComparer.Ordinal)
    {
      "AAD_Cookie",
      "FedAuth",
      "UserAuthToken",
      "UserAuthToken_VS2012"
    };
    private static readonly IReadOnlyCollection<string> s_aadAuthenticationMechanism = (IReadOnlyCollection<string>) new HashSet<string>()
    {
      "AAD",
      "AAD_ARM",
      "AAD_Ibiza",
      "AAD_Scoped",
      "AAD_Web"
    };
    public const string AuthenticationApplicationName = "Authentication";

    public static bool IsSignedInRequest(HttpRequest request) => AuthenticationHelpers.IsSignedInRequest((HttpRequestBase) new HttpRequestWrapper(request));

    public static bool IsSignedInRequest(HttpRequestBase request)
    {
      if (!string.Equals(request.HttpMethod, "POST", StringComparison.OrdinalIgnoreCase) || !string.Equals(request.ContentType ?? string.Empty, "application/x-www-form-urlencoded", StringComparison.OrdinalIgnoreCase))
        return false;
      return request.Url.AbsolutePath.EndsWith("/_signin", StringComparison.OrdinalIgnoreCase) || request.Url.AbsolutePath.EndsWith("/_signedin", StringComparison.OrdinalIgnoreCase);
    }

    public static bool IsPostSignOutRequest(HttpRequestBase request) => string.Equals(request.HttpMethod, "POST", StringComparison.OrdinalIgnoreCase) && string.Equals(request.ContentType ?? string.Empty, "application/x-www-form-urlencoded", StringComparison.OrdinalIgnoreCase) && request.Url.AbsolutePath.EndsWith("/_signout", StringComparison.OrdinalIgnoreCase);

    public static bool IsSignOutRequest(HttpRequest request) => request.Url.AbsolutePath.EndsWith("/_signout", StringComparison.OrdinalIgnoreCase) && request.Url.Query.StartsWith("?wa=wsignoutcleanup1.0", StringComparison.OrdinalIgnoreCase);

    public static void EnterMethodIfNull(
      IVssRequestContext requestContext,
      string applicationName,
      string methodName,
      bool lightWeight = false)
    {
      if (requestContext == null || requestContext.Method != null)
        return;
      MethodInformation methodInformation = lightWeight ? new MethodInformation(methodName, MethodType.LightWeight, EstimatedMethodCost.Low) : new MethodInformation(methodName, MethodType.Normal, EstimatedMethodCost.Moderate);
      requestContext.RootContext.EnterMethod(methodInformation);
      if (!string.IsNullOrEmpty(requestContext.RootContext.ServiceName))
        return;
      requestContext.RootContext.ServiceName = applicationName;
    }

    public static void SetAuthenticationMechanism(
      IVssRequestContext requestContext,
      AuthenticationMechanism authenticationMechanism,
      string additionalData = null)
    {
      if (requestContext == null)
        return;
      string str = EnumUtility.ToString<AuthenticationMechanism>(authenticationMechanism);
      if (additionalData != null)
        str = str + "!!" + additionalData;
      requestContext.RootContext.Items[RequestContextItemsKeys.AuthenticationMechanism] = (object) str;
    }

    public static void SetOAuthAppId(IVssRequestContext requestContext, JwtSecurityToken token)
    {
      Guid result;
      if (requestContext == null || token == null || !Guid.TryParse(token.GetClaimAsString("appid"), out result) || !(result != Guid.Empty))
        return;
      requestContext.RootContext.Items[RequestContextItemsKeys.OAuthAppId] = (object) result;
    }

    public static bool IsRequestUsingAADAuthentication(IVssRequestContext requestContext) => AuthenticationHelpers.s_aadAuthenticationMechanism.Contains<string>(requestContext.GetAuthenticationMechanism());

    public static bool IsRequestUsingCookieBasedAuthentication(IVssRequestContext requestContext)
    {
      string str;
      return requestContext.RootContext.TryGetItem<string>(RequestContextItemsKeys.AuthenticationMechanism, out str) && AuthenticationHelpers.s_cookieBasedAuthenticationMechanisms.Contains(str);
    }

    public static void SetCanIssueUserAuthenticationToken(
      IVssRequestContext requestContext,
      bool canIssueUserAuthenticationToken)
    {
      if (requestContext == null)
        return;
      requestContext.RootContext.Items[RequestContextItemsKeys.CanIssueUserAuthenticationToken] = (object) canIssueUserAuthenticationToken;
    }

    public static bool CanIssueUserAuthenticationToken(IVssRequestContext requestContext) => requestContext != null && requestContext.RootContext.Items.GetCastedValueOrDefault<string, bool>(RequestContextItemsKeys.CanIssueUserAuthenticationToken);

    public static void SetCanIssueFedAuthToken(
      IVssRequestContext requestContext,
      bool canIssueFedAuthToken)
    {
      if (requestContext == null)
        return;
      requestContext.RootContext.Items[RequestContextItemsKeys.CanIssueFedAuthToken] = (object) canIssueFedAuthToken;
    }

    public static bool CanIssueFedAuthToken(IVssRequestContext requestContext) => requestContext != null && requestContext.RootContext.Items.GetCastedValueOrDefault<string, bool>(RequestContextItemsKeys.CanIssueFedAuthToken);

    public static bool IsUnscopedToken(JwtSecurityToken token)
    {
      Claim claim = token.Claims.FirstOrDefault<Claim>((Func<Claim, bool>) (x => x.Type.Equals("scp")));
      return claim != null && claim.Value != null && claim.Value.Contains("app_token");
    }

    public static bool IsGlobalToken(JwtSecurityToken token) => token.Audiences == null || !token.Audiences.Any<string>((Func<string, bool>) (x => x.Contains("vso:")));

    public static void SetWWWAuthenticateHeaderIfNotPresent(
      HttpContextBase httpContext,
      string headerValue)
    {
      HttpResponseBase response = httpContext?.Response;
      if (response == null || response.Headers == null || response.HeadersWritten)
        return;
      string str = httpContext.Response.Headers.Get("WWW-Authenticate");
      if (!string.IsNullOrEmpty(str) && str.Contains(headerValue))
        return;
      httpContext.Response.AddHeader("WWW-Authenticate", headerValue);
    }

    public static string ReadAuthTokenFromBody(HttpRequestBase request)
    {
      string str1 = (string) null;
      if (HttpContext.Current.Request.InputStream == null)
        return str1;
      StreamReader streamReader = new StreamReader(request.InputStream);
      streamReader.BaseStream.Seek(0L, SeekOrigin.Begin);
      char[] buffer = new char[AuthenticationHelpers.s_maxContentSize];
      int num1 = 0;
      StringBuilder stringBuilder = new StringBuilder();
      int charCount;
      do
      {
        charCount = streamReader.Read(buffer, 0, buffer.Length);
        num1 += charCount;
        if (charCount != 0)
        {
          stringBuilder.Append(buffer, 0, charCount);
          string str2 = stringBuilder.ToString();
          if (str2.IndexOf("{\"__authToken\"") < 0 && charCount >= AuthenticationHelpers.s_maxContentSize)
          {
            streamReader.BaseStream.Seek(0L, SeekOrigin.Begin);
            return (string) null;
          }
          int startIndex = str2.IndexOf("Bearer", "{\"__authToken\"".Length);
          if (startIndex > 0)
          {
            int num2 = str2.IndexOf("\"", startIndex);
            if (num2 > -1 && str2.Length > num2)
            {
              str1 = str2.Substring(startIndex, num2 - startIndex);
              break;
            }
          }
        }
      }
      while (charCount > 0 && num1 < 4096);
      streamReader.BaseStream.Seek(0L, SeekOrigin.Begin);
      return str1;
    }

    public static IdentityDescriptor GetAadIdentityDescriptorFromAadToken(
      IVssRequestContext requestContext,
      JwtSecurityToken aadToken)
    {
      Claim claim1;
      if (aadToken == null)
      {
        claim1 = (Claim) null;
      }
      else
      {
        IEnumerable<Claim> claims = aadToken.Claims;
        claim1 = claims != null ? claims.FirstOrDefault<Claim>((Func<Claim, bool>) (x => x.Type == "tid")) : (Claim) null;
      }
      Claim claim2 = claim1;
      if (claim2 == null)
        return (IdentityDescriptor) null;
      if (!aadToken.Claims.Any<Claim>((Func<Claim, bool>) (x => x.Type == "oid")))
        return (IdentityDescriptor) null;
      string str = aadToken.Claims.FirstOrDefault<Claim>((Func<Claim, bool>) (x => x.Type == "upn"))?.Value;
      if (string.IsNullOrEmpty(str))
      {
        str = aadToken.Claims.FirstOrDefault<Claim>((Func<Claim, bool>) (x => x.Type == "unique_name"))?.Value;
        if (string.IsNullOrEmpty(str))
          return (IdentityDescriptor) null;
        if (str.StartsWith("live.com#", StringComparison.OrdinalIgnoreCase))
          str = str.Substring("live.com#".Length);
        else if (str.StartsWith("google.com#", StringComparison.OrdinalIgnoreCase))
          str = str.Substring("google.com#".Length);
        else if (str.StartsWith("mail#", StringComparison.OrdinalIgnoreCase))
          str = str.Substring("mail#".Length);
      }
      return new IdentityDescriptor("Microsoft.IdentityModel.Claims.ClaimsIdentity", string.Format("{0}\\{1}", (object) claim2.Value, (object) str));
    }

    public static bool ShouldSkipReissuingAuthToken(IVssRequestContext requestContext)
    {
      bool flag1 = false;
      if (!requestContext.IsFeatureEnabled("VisualStudio.Services.Authentication.SkipReissuingAuthToken"))
        return false;
      bool flag2 = requestContext.IsVstsDomainRequest();
      if (flag2)
        flag1 = new UserAgentDetails(requestContext.UserAgent).IsBrowser;
      requestContext.Trace(802165, TraceLevel.Info, "Authentication", nameof (ShouldSkipReissuingAuthToken), string.Format("isOldDomain: {0}, isBrowser: {1}", (object) flag2, (object) flag1));
      return flag2 & flag1;
    }

    internal static bool IssueSessionSecurityToken(
      IVssRequestContext requestContext,
      ClaimsPrincipal principal,
      IEnumerable<Claim> additionalClaims,
      Action<SessionSecurityToken> onTokenIssued = null)
    {
      bool flag = false;
      UserAuthenticationSessionToken token = (UserAuthenticationSessionToken) null;
      SessionSecurityToken sessionToken = (SessionSecurityToken) null;
      if (AuthenticationHelpers.CanIssueUserAuthenticationToken(requestContext))
      {
        requestContext.TraceDataConditionally(802166, TraceLevel.Info, "Authentication", nameof (IssueSessionSecurityToken), "Generating UserAuthenticationToken.", (Func<object>) (() => (object) new
        {
          principal = principal
        }), nameof (IssueSessionSecurityToken));
        token = UserAuthenticationSessionTokenHandler.CreateSessionToken(requestContext, (HttpContextBase) new HttpContextWrapper(HttpContext.Current), principal, AuthenticationMechanism.UserAuthToken, additionalClaims);
      }
      else
        requestContext.TraceDataConditionally(802066, TraceLevel.Info, "Authentication", nameof (IssueSessionSecurityToken), "Not Issuing UserAuthenticationToken.", (Func<object>) (() => (object) new
        {
          principal = principal
        }), nameof (IssueSessionSecurityToken));
      if (AuthenticationHelpers.CanIssueFedAuthToken(requestContext) && !requestContext.IsIssueFedAuthTokenDisabled())
      {
        requestContext.GetService<ITeamFoundationAuthenticationService>().ConfigureRequest(requestContext);
        if (additionalClaims != null)
        {
          IEnumerable<Claim> claims = AuthenticationHelpers.MergeClaims(principal.Claims, additionalClaims);
          ClaimsIdentity identity = principal.Identity as ClaimsIdentity;
          string authenticationType = identity.AuthenticationType;
          string nameClaimType = identity.NameClaimType;
          string roleClaimType = identity.RoleClaimType;
          principal = new ClaimsPrincipal((IIdentity) new ClaimsIdentity(claims, authenticationType, nameClaimType, roleClaimType));
        }
        sessionToken = FederatedAuthentication.SessionAuthenticationModule.CreateSessionSecurityToken(principal, Guid.NewGuid().ToString(), DateTime.UtcNow, DateTime.UtcNow.AddDays(7.0), true);
      }
      if (token != null)
      {
        requestContext.TraceDataConditionally(802166, TraceLevel.Info, "Authentication", nameof (IssueSessionSecurityToken), "Issuing UserAuthenticationToken.", (Func<object>) (() => (object) new
        {
          principal = principal
        }), nameof (IssueSessionSecurityToken));
        UserAuthenticationSessionTokenHandler.WriteTokenToCookie(requestContext, (HttpContextBase) new HttpContextWrapper(HttpContext.Current), token);
        requestContext.RootContext.Items["UserAuthenticationToken"] = (object) token;
        flag = true;
      }
      if (sessionToken != null)
      {
        FederatedAuthentication.SessionAuthenticationModule.WriteSessionTokenToCookie(sessionToken);
        if (onTokenIssued != null)
          onTokenIssued(sessionToken);
        flag = true;
      }
      return flag;
    }

    internal static byte[] WriteSessionTokenAsCookieForm(SessionSecurityToken token)
    {
      if (!(FederatedAuthentication.SessionAuthenticationModule.FederationConfiguration.IdentityConfiguration.SecurityTokenHandlers[typeof (SessionSecurityToken)] is SessionSecurityTokenHandler securityTokenHandler))
        throw new InvalidOperationException(string.Format("Can't get {0}", (object) securityTokenHandler));
      return securityTokenHandler.WriteToken(token);
    }

    public static IEnumerable<Claim> MergeClaims(
      IEnumerable<Claim> originalClaims,
      IEnumerable<Claim> additionalClaims)
    {
      foreach (Claim originalClaim in originalClaims)
        yield return originalClaim;
      if (additionalClaims != null)
      {
        foreach (Claim additionalClaim in additionalClaims)
        {
          Claim claim = additionalClaim;
          if (!originalClaims.Any<Claim>((Func<Claim, bool>) (x => x.Type.Equals(claim.Type, StringComparison.OrdinalIgnoreCase))))
            yield return claim;
        }
      }
    }

    public static bool ShouldTreatIdentityProviderAsMsa(
      IVssRequestContext requestContext,
      string claimValue)
    {
      int num1 = string.Equals(claimValue, "Live.com", StringComparison.OrdinalIgnoreCase) ? 1 : 0;
      bool flag1 = string.Equals(claimValue, "google.com", StringComparison.OrdinalIgnoreCase);
      bool flag2 = string.Equals(claimValue, "mail", StringComparison.OrdinalIgnoreCase);
      int num2 = flag1 ? 1 : 0;
      return (num1 | num2 | (flag2 ? 1 : 0)) != 0;
    }
  }
}
