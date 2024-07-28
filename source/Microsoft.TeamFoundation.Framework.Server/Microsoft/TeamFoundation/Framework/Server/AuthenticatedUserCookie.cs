// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.AuthenticatedUserCookie
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Globalization;
using System.Web;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public static class AuthenticatedUserCookie
  {
    private static string SpsAuthenticatedUserCookieName = "SpsAuthenticatedUser";
    private static string SpsAuthenticatedUserDisplayName = "DisplayName";
    private static string SpsAuthenticatedUserOrgId = "aad";

    public static void Set(IVssRequestContext requestContext, string displayName = null, bool? aad = null)
    {
      aad = new bool?(false);
      HttpResponse currentResponse = AuthenticatedUserCookie.GetCurrentResponse();
      if (currentResponse == null)
        return;
      if (string.IsNullOrEmpty(displayName))
      {
        displayName = requestContext.GetUserIdentity()?.DisplayName;
        displayName = displayName ?? string.Empty;
      }
      string cookieRootDomain = requestContext.GetService<ITeamFoundationAuthenticationService>().AuthenticationServiceInternal().GetCookieRootDomain(requestContext);
      HttpCookie cookie = new HttpCookie(AuthenticatedUserCookie.SpsAuthenticatedUserCookieName)
      {
        HttpOnly = true,
        Domain = cookieRootDomain,
        Expires = DateTime.UtcNow.AddDays(7.0),
        Secure = true
      };
      cookie.Values.Add(AuthenticatedUserCookie.SpsAuthenticatedUserDisplayName, Uri.EscapeDataString(displayName));
      cookie.Values.Add(AuthenticatedUserCookie.SpsAuthenticatedUserOrgId, aad.GetValueOrDefault().ToString((IFormatProvider) CultureInfo.InvariantCulture));
      CookieModifier.AddSameSiteNoneToCookie(requestContext, cookie);
      currentResponse.Cookies.Set(cookie);
    }

    public static void Clear(IVssRequestContext requestContext)
    {
      HttpRequest currentRequest = AuthenticatedUserCookie.GetCurrentRequest();
      HttpResponse currentResponse = AuthenticatedUserCookie.GetCurrentResponse();
      if (currentRequest == null || currentResponse == null)
        return;
      HttpCookie cookie = currentRequest.Cookies[AuthenticatedUserCookie.SpsAuthenticatedUserCookieName];
      if (cookie == null)
        return;
      string cookieRootDomain = requestContext.GetService<ITeamFoundationAuthenticationService>().AuthenticationServiceInternal().GetCookieRootDomain(requestContext);
      cookie.Value = string.Empty;
      cookie.Values.Clear();
      cookie.Expires = DateTime.Now.AddDays(-1.0);
      cookie.Domain = cookieRootDomain;
      cookie.HttpOnly = true;
      cookie.Secure = true;
      currentResponse.Cookies.Set(cookie);
    }

    private static HttpResponse GetCurrentResponse()
    {
      HttpContext current = HttpContext.Current;
      if (current != null)
      {
        try
        {
          return current.Response;
        }
        catch
        {
        }
      }
      return (HttpResponse) null;
    }

    private static HttpRequest GetCurrentRequest()
    {
      HttpContext current = HttpContext.Current;
      if (current != null)
      {
        try
        {
          return current.Request;
        }
        catch
        {
        }
      }
      return (HttpRequest) null;
    }
  }
}
