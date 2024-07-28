// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Web.SetUserTokenFilterAttribute
// Assembly: Microsoft.VisualStudio.Services.Gallery.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 17D36576-2EF3-4ABC-94BA-AF7891D15A3A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Web.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess.Platform;
using Microsoft.VisualStudio.Services.Gallery.Web.Utility;
using Microsoft.VisualStudio.Services.Location.Server;
using System;
using System.Globalization;
using System.Web;
using System.Web.Mvc;

namespace Microsoft.VisualStudio.Services.Gallery.Web
{
  public class SetUserTokenFilterAttribute : ActionFilterAttribute
  {
    public override void OnActionExecuting(ActionExecutingContext filterContext)
    {
      base.OnActionExecuting(filterContext);
      if (!(filterContext.Controller is WebPlatformAreaController controller) || !controller.TfsRequestContext.ExecutionEnvironment.IsHostedDeployment)
        return;
      HttpCookie cookie1 = filterContext.HttpContext.Request.Cookies.Get(Cookies.GalleryUserIdentity);
      DateTime expires = DateTime.UtcNow.AddYears(1);
      bool isSslOnly = controller.TfsRequestContext.ExecutionEnvironment.IsSslOnly;
      string domain = this.GetDomain(controller.TfsRequestContext);
      if (cookie1 == null)
      {
        if (filterContext.HttpContext.Response.HeadersWritten)
          return;
        string str = Guid.NewGuid().ToString();
        HttpCookie cookie2 = new HttpCookie(Cookies.GalleryUserIdentity, str);
        this.SetCookieAttributes(cookie2, expires, isSslOnly, domain);
        filterContext.HttpContext.Response.Cookies.Add(cookie2);
        IVssRequestContext vssRequestContext = (IVssRequestContext) filterContext.HttpContext.Items[(object) HttpContextConstants.IVssRequestContext];
        if (vssRequestContext == null || vssRequestContext.Items == null || vssRequestContext.Items.ContainsKey(RequestContextItemsKeys.AnonymousIdentifier))
          return;
        vssRequestContext.Items[RequestContextItemsKeys.AnonymousIdentifier] = (object) cookie2.Value;
      }
      else
      {
        this.SetCookieAttributes(cookie1, expires, isSslOnly, domain);
        filterContext.HttpContext.Response.SetCookie(cookie1);
      }
    }

    private void SetCookieAttributes(
      HttpCookie cookie,
      DateTime expires,
      bool isSecure,
      string domain)
    {
      cookie.Expires = expires;
      cookie.Secure = isSecure;
      cookie.Domain = domain;
    }

    protected virtual string GetDomain(IVssRequestContext requestContext)
    {
      string domain = LocationServiceHelper.GetServiceBaseUri(requestContext).Host;
      if (domain.Contains("."))
        domain = string.Format((IFormatProvider) CultureInfo.InvariantCulture, ".{0}", (object) domain);
      return domain;
    }
  }
}
