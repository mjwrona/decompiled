// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.Authentication.FederatedAuthCookieHelper
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.IdentityModel.Services;
using System.Web;

namespace Microsoft.TeamFoundation.Framework.Server.Authentication
{
  internal static class FederatedAuthCookieHelper
  {
    internal static void DeleteSessionTokenCookie(IVssRequestContext requestContext)
    {
      if (requestContext.IsDevOpsDomainRequestUsingRootContext())
      {
        string cookieRootDomain = requestContext.GetService<ITeamFoundationAuthenticationService>().AuthenticationServiceInternal().GetCookieRootDomain(requestContext);
        HttpCookie cookie1 = new HttpCookie("FedAuth");
        cookie1.Expires = DateTime.UtcNow.AddDays(-1.0);
        cookie1.Domain = cookieRootDomain;
        cookie1.HttpOnly = true;
        cookie1.Secure = true;
        HttpCookie cookie2 = new HttpCookie("FedAuth1");
        cookie2.Expires = DateTime.UtcNow.AddDays(-1.0);
        cookie2.Domain = cookieRootDomain;
        cookie2.HttpOnly = true;
        cookie2.Secure = true;
        CookieModifier.AddSameSiteNoneToCookie(requestContext, cookie1);
        CookieModifier.AddSameSiteNoneToCookie(requestContext, cookie2);
        HttpResponseBase response = HttpContextFactory.Current.Response;
        response.Cookies.Set(cookie1);
        response.Cookies.Set(cookie2);
      }
      else
        FederatedAuthentication.SessionAuthenticationModule.DeleteSessionTokenCookie();
    }
  }
}
