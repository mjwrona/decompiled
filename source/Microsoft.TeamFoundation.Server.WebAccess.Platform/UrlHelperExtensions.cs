// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.UrlHelperExtensions
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Platform, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A6A2C403-5081-466C-A570-9B50BFA8E213
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Platform.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Microsoft.TeamFoundation.Server.WebAccess
{
  public static class UrlHelperExtensions
  {
    public static string FragmentRouteUrl(
      this UrlHelper urlHelper,
      string routeName,
      string serverActionName,
      string serverControllerName,
      object serverRouteValues,
      string clientAction,
      object clientRouteValues)
    {
      return urlHelper.FragmentRouteUrl(routeName, serverActionName, serverControllerName, (object) new RouteValueDictionary(serverRouteValues), clientAction, clientRouteValues);
    }

    public static string LocAwareAction(
      this UrlHelper urlHelper,
      string actionName,
      string controllerName = null,
      RouteValueDictionary routeValues = null)
    {
      ArgumentUtility.CheckForNull<UrlHelper>(urlHelper, nameof (urlHelper));
      if (routeValues == null)
        routeValues = new RouteValueDictionary();
      if (!routeValues.ContainsKey("mkt"))
      {
        CultureInfo retainableCulture = UrlHelperExtensions.GetRetainableCulture(urlHelper);
        if (retainableCulture != null)
          routeValues.Add("mkt", (object) retainableCulture.ToString());
      }
      return !string.IsNullOrEmpty(controllerName) ? urlHelper.Action(actionName, controllerName, routeValues) : urlHelper.Action(actionName, routeValues);
    }

    public static string ForwardLink(this UrlHelper urlHelper, int linkId)
    {
      int? retainableLcid = UrlHelperExtensions.GetRetainableLCID(urlHelper);
      return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "https://go.microsoft.com/fwlink/?LinkID={0}{1}", (object) linkId, retainableLcid.HasValue ? (object) string.Format((IFormatProvider) CultureInfo.InvariantCulture, "&clcid={0}", (object) retainableLcid.Value.ToString("X")) : (object) string.Empty);
    }

    public static string ForwardLinkButton(
      this UrlHelper urlHelper,
      string buttonText,
      int linkId,
      string target = null,
      string @class = null,
      object htmlAttributes = null)
    {
      TagBuilder tagBuilder = new TagBuilder("a");
      tagBuilder.MergeAttribute("href", urlHelper.ForwardLink(linkId));
      tagBuilder.SetInnerText(buttonText);
      if (target != null)
        tagBuilder.MergeAttribute(nameof (target), target);
      if (@class != null)
        tagBuilder.AddClass(@class);
      if (htmlAttributes != null)
        tagBuilder.MergeAttributes<string, object>((IDictionary<string, object>) HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
      return tagBuilder.ToString();
    }

    private static int? GetRetainableLCID(UrlHelper urlHelper) => UrlHelperExtensions.GetRetainableCulture(urlHelper)?.LCID;

    private static CultureInfo GetRetainableCulture(UrlHelper urlHelper)
    {
      HttpContextBase httpContext = urlHelper.RequestContext.HttpContext;
      CultureInfo threadUiCulture = httpContext == null ? (CultureInfo) null : RequestLanguage.GetThreadUICulture(httpContext.Items);
      CultureInfo currentUiCulture = Thread.CurrentThread.CurrentUICulture;
      return threadUiCulture == null || threadUiCulture == currentUiCulture ? (CultureInfo) null : currentUiCulture;
    }

    public static void TraceEnter(
      this UrlHelper urlHelper,
      int tracePoint,
      string traceArea,
      string traceLayer,
      string methodName)
    {
      IVssRequestContext requestContext = urlHelper.RequestContext.TfsRequestContext();
      if (requestContext == null)
        return;
      requestContext.TraceEnter(tracePoint, traceArea, traceLayer, methodName);
    }

    public static void TraceLeave(
      this UrlHelper urlHelper,
      int tracePoint,
      string traceArea,
      string traceLayer,
      string methodName)
    {
      IVssRequestContext requestContext = urlHelper.RequestContext.TfsRequestContext();
      if (requestContext == null)
        return;
      requestContext.TraceLeave(tracePoint, traceArea, traceLayer, methodName);
    }
  }
}
