// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.ConfigurationExtensions
// Assembly: Microsoft.TeamFoundation.Server.WebAccess, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A2CCA8C5-6910-48A5-82D9-BDC1350B5B4D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Core;
using Microsoft.TeamFoundation.Server.WebAccess.Configuration;
using Microsoft.TeamFoundation.Server.WebAccess.Routing;
using Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Routing;

namespace Microsoft.TeamFoundation.Server.WebAccess
{
  public static class ConfigurationExtensions
  {
    internal static string StaticContentFolder = "_content";
    internal static string ThemeContentFolder = "App_Themes";
    internal static string ScriptContentFolder = "_scripts";
    internal static string PluginsFolder = "_plugins";
    internal const string FragmentActionParamKey = "_a";
    private const int WebAccessExceptionEaten = 599999;
    private const string TrackingScriptUrlRegistryKey = "/WebAccess/TrackingScriptUrl";

    public static MvcHtmlString TfsWebContext(this HtmlHelper htmlHelper) => ConfigurationExtensions.TfsWebContext(htmlHelper, htmlHelper.ViewContext.TfsWebContext(), (IDictionary<string, object>) null);

    public static MvcHtmlString TfsWebContext(this HtmlHelper htmlHelper, object htmlAttributes) => ConfigurationExtensions.TfsWebContext(htmlHelper, htmlHelper.ViewContext.TfsWebContext(), (IDictionary<string, object>) new RouteValueDictionary(htmlAttributes));

    public static MvcHtmlString TfsWebContext(
      this HtmlHelper htmlHelper,
      IDictionary<string, object> htmlAttributes)
    {
      return ConfigurationExtensions.TfsWebContext(htmlHelper, htmlHelper.ViewContext.TfsWebContext(), htmlAttributes);
    }

    public static MvcHtmlString TfsWebContext(this HtmlHelper htmlHelper, Microsoft.TeamFoundation.Server.WebAccess.TfsWebContext model) => ConfigurationExtensions.TfsWebContext(htmlHelper, model, (IDictionary<string, object>) null);

    public static MvcHtmlString TfsWebContext(
      this HtmlHelper htmlHelper,
      Microsoft.TeamFoundation.Server.WebAccess.TfsWebContext model,
      object htmlAttributes)
    {
      return ConfigurationExtensions.TfsWebContext(htmlHelper, model, (IDictionary<string, object>) new RouteValueDictionary(htmlAttributes));
    }

    public static MvcHtmlString TfsWebContext(
      this HtmlHelper htmlHelper,
      Microsoft.TeamFoundation.Server.WebAccess.TfsWebContext model,
      IDictionary<string, object> htmlAttributes)
    {
      htmlHelper.TraceEnter(0, "WebAccess", TfsTraceLayers.Content, nameof (TfsWebContext));
      try
      {
        return PlatformHtmlExtensions.WebContext(htmlHelper, (WebContext) model, htmlAttributes);
      }
      finally
      {
        htmlHelper.TraceLeave(0, "WebAccess", TfsTraceLayers.Content, nameof (TfsWebContext));
      }
    }

    public static string IdentityImage(
      this Microsoft.TeamFoundation.Server.WebAccess.TfsWebContext webContext,
      TeamFoundationIdentity identity)
    {
      return webContext.IdentityImage(identity.TeamFoundationId, (object) null);
    }

    public static string IdentityImage(
      this Microsoft.TeamFoundation.Server.WebAccess.TfsWebContext webContext,
      TeamFoundationIdentity identity,
      object routeValues)
    {
      return webContext.IdentityImage(identity.TeamFoundationId, routeValues);
    }

    public static string IdentityImage(this Microsoft.TeamFoundation.Server.WebAccess.TfsWebContext webContext, Guid identityId) => webContext.IdentityImage(identityId, (object) null);

    public static string IdentityImage(
      this Microsoft.TeamFoundation.Server.WebAccess.TfsWebContext webContext,
      Guid identityId,
      object routeValues)
    {
      RouteValueDictionary routeValueDictionary = new RouteValueDictionary()
      {
        {
          "id",
          (object) identityId
        },
        {
          "team",
          (object) ""
        },
        {
          "project",
          (object) ""
        },
        {
          "__v",
          (object) webContext.WebApiVersionClient
        }
      };
      if (routeValues != null)
        routeValueDictionary.Merge(routeValues);
      routeValueDictionary["routeArea"] = (object) "Api";
      string empty = string.Empty;
      return !string.IsNullOrEmpty(routeValueDictionary["team"] as string) || !string.IsNullOrEmpty(routeValueDictionary["project"] as string) ? webContext.Url.Action("identityImage", "common", routeValueDictionary) : UrlHelperExtensions.RouteUrl(webContext.Url, "ApiServiceHostControllerAction", "identityImage", "common", routeValueDictionary);
    }

    public static string FragmentAction(this UrlHelper urlHelper, string action) => urlHelper.FragmentAction(action, (object) null);

    public static string FragmentAction(
      this UrlHelper urlHelper,
      string action,
      object routeValues)
    {
      return "#" + ConfigurationExtensions.GetActionBasedParameters(action, routeValues);
    }

    public static string GetQueryParameters(
      this UrlHelper urlHelper,
      string action,
      object routeValues)
    {
      return "?" + ConfigurationExtensions.GetActionBasedParameters(action, routeValues);
    }

    public static string QueryParameterAction(
      this UrlHelper urlHelper,
      string serverAction,
      RouteValueDictionary serverRouteValues,
      string clientAction,
      object clientRouteValues)
    {
      return urlHelper.Action(serverAction, serverRouteValues) + urlHelper.GetQueryParameters(clientAction, clientRouteValues);
    }

    public static string FragmentAction(
      this UrlHelper urlHelper,
      string serverAction,
      object serverRouteValues,
      string clientAction,
      object clientRouteValues)
    {
      return urlHelper.Action(serverAction, serverRouteValues) + urlHelper.FragmentAction(clientAction, clientRouteValues);
    }

    public static string FragmentAction(
      this UrlHelper urlHelper,
      string serverAction,
      RouteValueDictionary serverRouteValues,
      string clientAction,
      object clientRouteValues)
    {
      return urlHelper.Action(serverAction, serverRouteValues) + urlHelper.FragmentAction(clientAction, clientRouteValues);
    }

    public static string FragmentAction(
      this UrlHelper urlHelper,
      string serverAction,
      string serverControllerName,
      object serverRouteValues,
      string clientAction,
      object clientRouteValues)
    {
      return urlHelper.Action(serverAction, serverControllerName, serverRouteValues) + urlHelper.FragmentAction(clientAction, clientRouteValues);
    }

    public static string FragmentAction(
      this UrlHelper urlHelper,
      string serverAction,
      string serverControllerName,
      RouteValueDictionary serverRouteValues,
      string clientAction,
      object clientRouteValues)
    {
      return urlHelper.Action(serverAction, serverControllerName, serverRouteValues) + urlHelper.FragmentAction(clientAction, clientRouteValues);
    }

    public static MvcHtmlString Bootstrap(this HtmlHelper htmlHelper)
    {
      htmlHelper.TraceEnter(0, "WebAccess", TfsTraceLayers.Content, nameof (Bootstrap));
      try
      {
        return MvcHtmlString.Empty;
      }
      finally
      {
        htmlHelper.TraceLeave(0, "WebAccess", TfsTraceLayers.Content, nameof (Bootstrap));
      }
    }

    public static MvcHtmlString BuiltinPlugins(this HtmlHelper htmlHelper)
    {
      htmlHelper.TraceEnter(0, "WebAccess", TfsTraceLayers.Content, nameof (BuiltinPlugins));
      try
      {
        return BuiltinPluginManager.GetBootstrap(htmlHelper.ViewContext.TfsWebContext());
      }
      finally
      {
        htmlHelper.TraceLeave(0, "WebAccess", TfsTraceLayers.Content, nameof (BuiltinPlugins));
      }
    }

    public static MvcHtmlString EnsightenTrackingScriptUrl(this HtmlHelper htmlHelper) => MvcHtmlString.Create(ConfigurationExtensions.TrackingScriptUrl(htmlHelper.ViewContext.TfsWebContext().TfsRequestContext));

    public static MvcHtmlString EnsightenTrackingScript(this HtmlHelper htmlHelper)
    {
      htmlHelper.TraceEnter(0, "WebAccess", TfsTraceLayers.Content, nameof (EnsightenTrackingScript));
      Microsoft.TeamFoundation.Server.WebAccess.TfsWebContext tfsWebContext = htmlHelper.ViewContext.TfsWebContext();
      try
      {
        if (!tfsWebContext.IsHosted)
          return new MvcHtmlString("");
        StringBuilder stringBuilder = new StringBuilder();
        TagBuilder scriptTag = PlatformHtmlExtensions.CreateScriptTag(ConfigurationExtensions.TrackingScriptUrl(tfsWebContext.TfsRequestContext));
        scriptTag.AddNonceAttribute(htmlHelper.ViewContext.TfsRequestContext(false), htmlHelper.ViewContext.HttpContext);
        stringBuilder.AppendLine(scriptTag.ToString());
        return MvcHtmlString.Create(stringBuilder.ToString());
      }
      finally
      {
        htmlHelper.TraceLeave(0, "WebAccess", TfsTraceLayers.Content, nameof (EnsightenTrackingScript));
      }
    }

    private static string GetActionBasedParameters(string action, object routeValues)
    {
      IEnumerable<KeyValuePair<string, object>> keyValuePairs;
      if (routeValues != null)
      {
        RouteValueDictionary routeDictionary = PlatformRouteHelpers.ToRouteDictionary(routeValues);
        action = routeDictionary.GetValue<string>("_a", action);
        keyValuePairs = routeDictionary.Where<KeyValuePair<string, object>>((Func<KeyValuePair<string, object>, bool>) (pair => !StringComparer.OrdinalIgnoreCase.Equals("_a", pair.Key)));
      }
      else
        keyValuePairs = Enumerable.Empty<KeyValuePair<string, object>>();
      if (!string.IsNullOrEmpty(action))
        keyValuePairs = ((IEnumerable<KeyValuePair<string, object>>) new KeyValuePair<string, object>[1]
        {
          new KeyValuePair<string, object>("_a", (object) action)
        }).Concat<KeyValuePair<string, object>>(keyValuePairs);
      return string.Join("&", keyValuePairs.Select<KeyValuePair<string, object>, string>((Func<KeyValuePair<string, object>, string>) (pair => Uri.EscapeDataString(pair.Key) + "=" + Uri.EscapeDataString(Convert.ToString(pair.Value, (IFormatProvider) CultureInfo.InvariantCulture)))));
    }

    private static string TrackingScriptUrl(IVssRequestContext requestContext)
    {
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      return vssRequestContext.GetService<CachedRegistryService>().GetValue(vssRequestContext, (RegistryQuery) "/WebAccess/TrackingScriptUrl", "//nexus.ensighten.com/vso/dev/Bootstrap.js");
    }
  }
}
