// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.CookieModifier
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Web;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public static class CookieModifier
  {
    private static readonly string Area = nameof (CookieModifier);
    private static Regex IOsNoSameSiteSuport = new Regex("CPU\\siPhone\\sOS\\s([1-9]|[1][0-2])([_]|\\s)", RegexOptions.IgnoreCase | RegexOptions.Compiled);
    private static Regex IPadOsNoSameSiteSuport = new Regex("iPad;\\sCPU\\sOS\\s([1-9]|[1][0-2])([_]|\\s)", RegexOptions.IgnoreCase | RegexOptions.Compiled);
    private static Regex EdgeNoSameSiteSuport = new Regex("(Edge|Edg)/([1-9]|[1-6][0-9])\\.", RegexOptions.IgnoreCase | RegexOptions.Compiled);
    private static Regex ChromeNoSameSiteSuport = new Regex("Chrome/([1-9]|[1-6][0-9])\\.", RegexOptions.IgnoreCase | RegexOptions.Compiled);
    private static Regex FireFoxNoSameSiteSuport = new Regex("Firefox/([1-9]|[1-5][0-9])\\.", RegexOptions.IgnoreCase | RegexOptions.Compiled);
    private static Regex MacOsNoSameSiteSuport = new Regex("Macintosh;\\sIntel\\sMac\\sOS\\sX\\s10_([1-9]|[1][0-4])(\\)|_)", RegexOptions.IgnoreCase | RegexOptions.Compiled);

    public static void AddSameSiteNoneToCookie(IVssRequestContext requestContext, HttpCookie cookie)
    {
      if (requestContext == null)
        return;
      if (cookie == null)
      {
        requestContext.Trace(1511200, TraceLevel.Info, CookieModifier.Area, nameof (AddSameSiteNoneToCookie), "Null cookie");
      }
      else
      {
        requestContext.Trace(1511201, TraceLevel.Info, CookieModifier.Area, nameof (AddSameSiteNoneToCookie), "Attempting to set SameSite=None for " + cookie.Name);
        if (!cookie.Secure)
        {
          requestContext.Trace(1511205, TraceLevel.Info, CookieModifier.Area, nameof (AddSameSiteNoneToCookie), "Setting cookie " + cookie.Name + " as Secure");
          cookie.Secure = true;
        }
        string userAgent = CookieModifier.GetUserAgent(requestContext);
        if (CookieModifier.SupportsSameSiteNone(requestContext, userAgent))
        {
          requestContext.Trace(1511203, TraceLevel.Info, CookieModifier.Area, nameof (AddSameSiteNoneToCookie), "Successfully set SameSite=None for " + cookie.Name + " with UserAgent " + userAgent);
          cookie.Value += ";SameSite=None";
        }
        else
          requestContext.Trace(1511204, TraceLevel.Info, CookieModifier.Area, nameof (AddSameSiteNoneToCookie), "Failed to set SameSite=None for " + cookie.Name + " due to unsupportive UserAgent " + userAgent);
      }
    }

    private static string GetUserAgent(IVssRequestContext requestContext)
    {
      try
      {
        return requestContext.RootContext.WebRequestContextInternal().HttpContext.Request.Headers["X-VSS-OriginUserAgent"] ?? requestContext.UserAgent;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1511214, TraceLevel.Error, CookieModifier.Area, "AddSameSiteNoneToCookie", ex);
        return string.Empty;
      }
    }

    internal static bool SupportsSameSiteNone(IVssRequestContext requestContext, string userAgent)
    {
      if (string.IsNullOrWhiteSpace(userAgent))
      {
        requestContext.Trace(1511207, TraceLevel.Info, CookieModifier.Area, nameof (SupportsSameSiteNone), "Empty UserAgent");
        return false;
      }
      if (CookieModifier.IOsNoSameSiteSuport.IsMatch(userAgent) || CookieModifier.IPadOsNoSameSiteSuport.IsMatch(userAgent))
      {
        requestContext.Trace(1511208, TraceLevel.Info, CookieModifier.Area, nameof (SupportsSameSiteNone), "Unsupported iOS/iPadOS UserAgent: " + userAgent);
        return false;
      }
      if (CookieModifier.EdgeNoSameSiteSuport.IsMatch(userAgent))
      {
        requestContext.Trace(1511214, TraceLevel.Info, CookieModifier.Area, nameof (SupportsSameSiteNone), "Unsupported Edge UserAgent: " + userAgent);
        return false;
      }
      if (CookieModifier.FireFoxNoSameSiteSuport.IsMatch(userAgent))
      {
        requestContext.Trace(1511209, TraceLevel.Info, CookieModifier.Area, nameof (SupportsSameSiteNone), "Unsupported FireFox UserAgent: " + userAgent);
        return false;
      }
      if (CookieModifier.ChromeNoSameSiteSuport.IsMatch(userAgent))
      {
        requestContext.Trace(1511210, TraceLevel.Info, CookieModifier.Area, nameof (SupportsSameSiteNone), "Unsupported Chrome UserAgent: " + userAgent);
        return false;
      }
      if (userAgent.IndexOf("Safari", StringComparison.InvariantCultureIgnoreCase) != -1 && userAgent.IndexOf("Version/", StringComparison.InvariantCultureIgnoreCase) != -1 && CookieModifier.MacOsNoSameSiteSuport.IsMatch(userAgent))
      {
        requestContext.Trace(1511211, TraceLevel.Info, CookieModifier.Area, nameof (SupportsSameSiteNone), "Unsupported Safari UserAgent: " + userAgent);
        return false;
      }
      if (userAgent.IndexOf("MSIE", StringComparison.InvariantCultureIgnoreCase) != -1 || userAgent.IndexOf("Trident/", StringComparison.InvariantCultureIgnoreCase) != -1 && userAgent.IndexOf("rv:11", StringComparison.InvariantCultureIgnoreCase) != -1)
      {
        requestContext.Trace(1511212, TraceLevel.Info, CookieModifier.Area, nameof (SupportsSameSiteNone), "Unsupported IE UserAgent: " + userAgent);
        return false;
      }
      requestContext.Trace(1511213, TraceLevel.Info, CookieModifier.Area, nameof (SupportsSameSiteNone), "UserAgent supports SameSite=None: " + userAgent);
      return true;
    }
  }
}
