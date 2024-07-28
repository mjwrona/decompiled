// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.GalleryRequestLoggerModule
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;

namespace Microsoft.VisualStudio.Services.Gallery.Server
{
  internal class GalleryRequestLoggerModule : IHttpModule
  {
    public void Init(HttpApplication context)
    {
      context.PreRequestHandlerExecute += (EventHandler) ((sender, e) => this.AddAnonymousIdentifier((HttpContextBase) new HttpContextWrapper(((HttpApplication) sender).Context)));
      context.PreRequestHandlerExecute += (EventHandler) ((sender, e) => this.AddAcceptEncodingForVsIde((HttpContextBase) new HttpContextWrapper(((HttpApplication) sender).Context)));
    }

    public void Dispose()
    {
    }

    public void AddAnonymousIdentifier(HttpContextBase context)
    {
      if (!context.Items.Contains((object) HttpContextConstants.IVssRequestContext))
        return;
      IVssRequestContext vssRequestContext = (IVssRequestContext) context.Items[(object) HttpContextConstants.IVssRequestContext];
      if (vssRequestContext == null)
        return;
      NameValueCollection headers = context.Request.Headers;
      IEnumerable<string> source = (IEnumerable<string>) null;
      if (headers != null)
        source = (IEnumerable<string>) headers.GetValues("X-Market-User-Id");
      if (source != null && source.Count<string>() > 0)
      {
        vssRequestContext.Items[RequestContextItemsKeys.AnonymousIdentifier] = (object) source.FirstOrDefault<string>();
      }
      else
      {
        if (context.Request.Cookies == null)
          return;
        HttpCookie httpCookie = context.Request.Cookies.Get("Gallery-Service-UserIdentifier");
        if (httpCookie == null)
          return;
        string str = httpCookie.Value;
        if (string.IsNullOrEmpty(str))
          return;
        vssRequestContext.Items[RequestContextItemsKeys.AnonymousIdentifier] = (object) str;
      }
    }

    public void AddAcceptEncodingForVsIde(HttpContextBase context)
    {
      IVssRequestContext requestContext = (IVssRequestContext) context.Items[(object) HttpContextConstants.IVssRequestContext];
      if (requestContext == null || !requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.EnableForcefulGzippingForVs"))
        return;
      string[] values1 = context.Request.Headers.GetValues("User-Agent");
      if (values1 == null || values1.Length != 1 || values1[0] == null || !values1[0].StartsWith("VSIDE", StringComparison.OrdinalIgnoreCase))
        return;
      string[] values2 = context.Request.Headers.GetValues("Accept-Encoding");
      if (values2 != null && values2.Length != 0)
        return;
      context.Request.Headers.Add("Accept-Encoding", "gzip");
    }
  }
}
