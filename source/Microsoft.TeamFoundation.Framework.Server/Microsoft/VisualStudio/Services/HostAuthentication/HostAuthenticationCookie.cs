// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.HostAuthentication.HostAuthenticationCookie
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.Authentication.UserAuthentication;
using Microsoft.VisualStudio.Services.DelegatedAuthorization;
using Microsoft.VisualStudio.Services.Location;
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

namespace Microsoft.VisualStudio.Services.HostAuthentication
{
  internal static class HostAuthenticationCookie
  {
    private static readonly string s_area = "HostAuthentication";
    private static readonly string s_layer = nameof (HostAuthenticationCookie);

    internal static void DeleteHostAuthenticationCookie(IVssRequestContext requestContext)
    {
      string cookieRootDomain = requestContext.GetService<ITeamFoundationAuthenticationService>().AuthenticationServiceInternal().GetCookieRootDomain(requestContext);
      HttpCookie cookie = new HttpCookie("HostAuthentication", (string) null);
      cookie.Expires = DateTime.UtcNow.AddDays(-1.0);
      cookie.Domain = cookieRootDomain;
      cookie.HttpOnly = true;
      cookie.Secure = true;
      CookieModifier.AddSameSiteNoneToCookie(requestContext, cookie);
      HttpContextFactory.Current.Response.Cookies.Set(cookie);
    }

    internal static HostAuthenticationToken GetHostAuthenticationToken(
      IVssRequestContext requestContext)
    {
      HostAuthenticationToken authenticationToken;
      if (!requestContext.RootContext.TryGetItem<HostAuthenticationToken>("$HostAuthentication", out authenticationToken))
      {
        authenticationToken = HostAuthenticationCookie.ReadHostAuthenticationToken(requestContext);
        if (authenticationToken != null)
          requestContext.RootContext.Items["$HostAuthentication"] = (object) authenticationToken;
      }
      return authenticationToken;
    }

    internal static void SetHostAuthenticationToken(
      IVssRequestContext requestContext,
      HostAuthenticationToken token)
    {
      HostAuthenticationCookie.WriteHostAuthenticationToken(requestContext, token);
      requestContext.RootContext.Items["$HostAuthentication"] = (object) token;
    }

    private static HostAuthenticationToken ReadHostAuthenticationToken(
      IVssRequestContext requestContext)
    {
      HttpContextBase current = HttpContextFactory.Current;
      if (current == null)
        return (HostAuthenticationToken) null;
      HttpCookie cookie = current.Request.Cookies["HostAuthentication"];
      if (cookie == null)
      {
        requestContext.Trace(828619, TraceLevel.Info, HostAuthenticationCookie.s_area, HostAuthenticationCookie.s_layer, "HostAuthentication cookie was not present in request.");
        return (HostAuthenticationToken) null;
      }
      JwtSecurityToken jwtSecurityToken;
      try
      {
        jwtSecurityToken = new JwtSecurityToken(cookie.Value);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(248428, HostAuthenticationCookie.s_area, HostAuthenticationCookie.s_layer, ex);
        return (HostAuthenticationToken) null;
      }
      Claim claim = jwtSecurityToken.Claims.Where<Claim>((Func<Claim, bool>) (s => s.Type.Equals("host_auth", StringComparison.OrdinalIgnoreCase))).FirstOrDefault<Claim>();
      if (claim == null)
      {
        requestContext.Trace(340032, TraceLevel.Error, HostAuthenticationCookie.s_area, HostAuthenticationCookie.s_layer, "Missing host_auth claim in HostAuthentication JWT token.");
        return (HostAuthenticationToken) null;
      }
      HostAuthenticationToken authenticationToken;
      if (JsonUtilities.TryDeserialize<HostAuthenticationToken>(claim.Value, out authenticationToken))
        return authenticationToken;
      requestContext.Trace(990863, TraceLevel.Error, HostAuthenticationCookie.s_area, HostAuthenticationCookie.s_layer, "Error deserializing HostAuthenticationToken.");
      return (HostAuthenticationToken) null;
    }

    private static string GetConfigurationIssuer(IVssRequestContext requestContext)
    {
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      UserAuthenticationConfiguration configuration = vssRequestContext.GetService<IUserAuthenticationConfigurationService>().GetConfiguration(vssRequestContext);
      ILocationService service = vssRequestContext.GetService<ILocationService>();
      return configuration.Issuer ?? new Uri(service.GetLocationServiceUrl(vssRequestContext, ServiceInstanceTypes.SPS, AccessMappingConstants.PublicAccessMappingMoniker)).Host;
    }

    private static void WriteHostAuthenticationToken(
      IVssRequestContext requestContext,
      HostAuthenticationToken token)
    {
      HttpContextBase current = HttpContextFactory.Current;
      if (current == null)
        return;
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      IDelegatedAuthorizationConfigurationService service = vssRequestContext.GetService<IDelegatedAuthorizationConfigurationService>();
      ITeamFoundationAuthenticationServiceInternal authenticationServiceInternal = vssRequestContext.GetService<ITeamFoundationAuthenticationService>().AuthenticationServiceInternal();
      string configurationIssuer = HostAuthenticationCookie.GetConfigurationIssuer(requestContext);
      Claim[] additionalClaims1 = new Claim[2]
      {
        new Claim("nameid", "HostAuthentication"),
        new Claim("host_auth", token.Serialize<HostAuthenticationToken>())
      };
      JsonWebToken jsonWebToken;
      if (requestContext.IsFeatureEnabled("AzureDevOps.Services.Framework.DoNotUseDelegatedAuthorizationCertificate.M168"))
      {
        string issuer = configurationIssuer;
        string audience = configurationIssuer;
        DateTime utcNow = DateTime.UtcNow;
        DateTime dateTime = DateTime.UtcNow;
        DateTime validTo = dateTime.AddHours(8.0);
        dateTime = new DateTime();
        DateTime issuedAt = dateTime;
        Claim[] additionalClaims2 = additionalClaims1;
        jsonWebToken = JsonWebToken.Create(issuer, audience, utcNow, validTo, issuedAt, (IEnumerable<Claim>) additionalClaims2);
      }
      else
      {
        VssSigningCredentials signingCredentials = service.GetSigningCredentials(vssRequestContext, true);
        jsonWebToken = JsonWebToken.Create(configurationIssuer, configurationIssuer, DateTime.UtcNow, DateTime.UtcNow.AddHours(8.0), (IEnumerable<Claim>) additionalClaims1, signingCredentials, true);
      }
      HttpCookie cookie = new HttpCookie("HostAuthentication", jsonWebToken.EncodedToken)
      {
        Domain = authenticationServiceInternal.GetCookieRootDomain(vssRequestContext),
        HttpOnly = true,
        Secure = true
      };
      CookieModifier.AddSameSiteNoneToCookie(requestContext, cookie);
      current.Response.Cookies.Set(cookie);
    }
  }
}
