// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.RequestLanguage
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Threading;
using System.Web;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class RequestLanguage : IDisposable
  {
    internal const string Market = "mkt";
    internal const string AcceptLanguage = "Accept-Language";
    internal const string CurrentCulture = "ApplyRequestLanguage.CurrentCulture";
    internal const string CurrentUICulture = "ApplyRequestLanguage.CurrentUICulture";
    private const string forceHeaderCultureFeatureFlag = "VisualStudio.Services.WebAccess.ForceHeaderCulture";

    public RequestLanguage(IVssRequestContext requestContext = null) => RequestLanguage.Apply(requestContext);

    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize((object) this);
    }

    protected void Dispose(bool disposing)
    {
      if (!disposing)
        return;
      RequestLanguage.Revert();
    }

    public static void Apply(IVssRequestContext requestContext = null)
    {
      HttpRequestBase currentRequest = RequestLanguage.GetCurrentRequest();
      if (currentRequest == null)
        return;
      IDictionary items = currentRequest.RequestContext == null || currentRequest.RequestContext.HttpContext == null ? (IDictionary) null : currentRequest.RequestContext.HttpContext.Items;
      RequestLanguage.Apply(requestContext, currentRequest.QueryString, currentRequest.UserLanguages, items);
    }

    public static void Apply(
      IVssRequestContext requestContext,
      NameValueCollection queryString,
      string[] userLanguages,
      IDictionary state)
    {
      CultureInfo cultureInfo = RequestLanguage.GetCulture(queryString == null ? (string) null : queryString["Accept-Language"]) ?? RequestLanguage.GetCulture(queryString == null ? (string) null : queryString["mkt"]);
      if (state != null && !state.IsReadOnly)
      {
        state[(object) "ApplyRequestLanguage.CurrentCulture"] = (object) Thread.CurrentThread.CurrentCulture;
        state[(object) "ApplyRequestLanguage.CurrentUICulture"] = (object) Thread.CurrentThread.CurrentUICulture;
      }
      if (cultureInfo != null)
      {
        Thread.CurrentThread.CurrentCulture = cultureInfo;
        Thread.CurrentThread.CurrentUICulture = cultureInfo;
      }
      else
      {
        bool forceHeaderCulture = false;
        if (requestContext != null)
          forceHeaderCulture = requestContext.IsFeatureEnabled("VisualStudio.Services.WebAccess.ForceHeaderCulture");
        TeamFoundationApplicationCore.SetPreferredCulture(userLanguages, requestContext, forceHeaderCulture);
      }
    }

    public static CultureInfo GetCulture(string acceptLanguage)
    {
      if (string.IsNullOrWhiteSpace(acceptLanguage))
        return (CultureInfo) null;
      try
      {
        return CultureInfo.GetCultureInfo(acceptLanguage);
      }
      catch
      {
      }
      return (CultureInfo) null;
    }

    public static CultureInfo GetThreadUICulture(IDictionary state) => state == null || !state.Contains((object) "ApplyRequestLanguage.CurrentUICulture") ? (CultureInfo) null : state[(object) "ApplyRequestLanguage.CurrentUICulture"] as CultureInfo;

    public static void Revert()
    {
      HttpRequestBase currentRequest = RequestLanguage.GetCurrentRequest();
      if (currentRequest == null || currentRequest.RequestContext == null || currentRequest.RequestContext.HttpContext == null)
        return;
      RequestLanguage.Revert(currentRequest.RequestContext.HttpContext.Items);
    }

    public static void Revert(IDictionary state)
    {
      if (state == null || state.IsReadOnly)
        return;
      if (state[(object) "ApplyRequestLanguage.CurrentCulture"] is CultureInfo cultureInfo1)
      {
        Thread.CurrentThread.CurrentCulture = cultureInfo1;
        state.Remove((object) "ApplyRequestLanguage.CurrentCulture");
      }
      if (!(state[(object) "ApplyRequestLanguage.CurrentUICulture"] is CultureInfo cultureInfo2))
        return;
      Thread.CurrentThread.CurrentUICulture = cultureInfo2;
      state.Remove((object) "ApplyRequestLanguage.CurrentUICulture");
    }

    public static List<CultureInfo> GetAcceptedCultures(IVssRequestContext requestContext)
    {
      CultureInfo cultureInfo;
      using (new RequestLanguage(requestContext))
        cultureInfo = Thread.CurrentThread.CurrentUICulture;
      List<CultureInfo> acceptedCultures = new List<CultureInfo>();
      for (; cultureInfo != null && cultureInfo.Parent != cultureInfo && !cultureInfo.Equals((object) CultureInfo.InvariantCulture); cultureInfo = cultureInfo.Parent)
        acceptedCultures.Add(cultureInfo);
      if (acceptedCultures.Count == 0)
        acceptedCultures.Add(cultureInfo);
      return acceptedCultures;
    }

    private static HttpRequestBase GetCurrentRequest()
    {
      HttpContextBase current = HttpContextFactory.Current;
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
      return (HttpRequestBase) null;
    }
  }
}
