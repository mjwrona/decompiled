// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Platform.SessionTokenCookie
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Platform, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A6A2C403-5081-466C-A570-9B50BFA8E213
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Platform.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Location.Server;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Microsoft.TeamFoundation.Server.WebAccess.Platform
{
  internal static class SessionTokenCookie
  {
    private static string s_cookieName = "WebPlatform-SessionTokens";

    private static string GetDomain(TfsApiController controller)
    {
      if (!controller.TfsRequestContext.ExecutionEnvironment.IsHostedDeployment)
        return (string) null;
      string domain = LocationServiceHelper.GetServiceBaseUri(controller.TfsRequestContext.To(TeamFoundationHostType.Deployment)).Host;
      if (domain.Contains("."))
        domain = string.Format((IFormatProvider) CultureInfo.InvariantCulture, ".{0}", (object) domain);
      return domain;
    }

    public static SessionTokens GetTokens(TfsApiController controller)
    {
      CookieState cookieState = controller.Request.Headers.GetCookies().Select<CookieHeaderValue, CookieState>((Func<CookieHeaderValue, CookieState>) (c => c[SessionTokenCookie.s_cookieName])).FirstOrDefault<CookieState>();
      SessionTokens sessionTokens = (SessionTokens) null;
      if (cookieState != null && !string.IsNullOrEmpty(cookieState.Value))
        JsonUtilities.TryDeserialize<SessionTokens>(cookieState.Value, out sessionTokens);
      return sessionTokens ?? new SessionTokens();
    }

    public static void SetTokens(
      TfsApiController controller,
      HttpResponseMessage response,
      SessionTokens sessionTokens,
      DateTime validTo)
    {
      response.Headers.AddCookies((IEnumerable<CookieHeaderValue>) new CookieHeaderValue[1]
      {
        new CookieHeaderValue(SessionTokenCookie.s_cookieName, sessionTokens.Serialize<SessionTokens>())
        {
          Expires = new DateTimeOffset?((DateTimeOffset) validTo),
          HttpOnly = true,
          Secure = controller.TfsRequestContext.ExecutionEnvironment.IsSslOnly,
          Domain = SessionTokenCookie.GetDomain(controller)
        }
      });
    }
  }
}
