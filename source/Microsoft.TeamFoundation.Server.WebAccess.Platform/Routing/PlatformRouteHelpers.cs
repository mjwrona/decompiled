// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Routing.PlatformRouteHelpers
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Platform, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A6A2C403-5081-466C-A570-9B50BFA8E213
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Platform.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;

namespace Microsoft.TeamFoundation.Server.WebAccess.Routing
{
  public static class PlatformRouteHelpers
  {
    private static RouteValueDictionary sm_navigationConstraints;
    private static object sm_defaults = (object) new
    {
      controller = "home",
      action = "index"
    };

    static PlatformRouteHelpers()
    {
      PlatformRouteHelpers.sm_navigationConstraints = new RouteValueDictionary();
      PlatformRouteHelpers.sm_navigationConstraints["navigation"] = (object) new TfsRouteAreaConstraint();
      PlatformRouteHelpers.sm_navigationConstraints["serviceHost"] = (object) "((?![_\\.]).*)?";
      PlatformRouteHelpers.sm_navigationConstraints["project"] = (object) "((?![_\\.])[^/:\\\\~&%;@'\"?<>=|#$*}{,+=\\[\\]]*)?";
      PlatformRouteHelpers.sm_navigationConstraints["team"] = (object) "((?![_\\.])[^/:\\\\~&%;@'\"?<>=|#$*}{,+=\\[\\]]*)?";
    }

    public static TfsRoute MapTfsRoute(
      this RouteCollection routes,
      string name,
      TeamFoundationHostType hostType,
      string address)
    {
      return PlatformRouteHelpers.MapTfsRoute(routes, name, hostType, address, (object) null, (string[]) null);
    }

    public static TfsRoute MapTfsRoute(
      this RouteCollection routes,
      string name,
      TeamFoundationHostType hostType,
      string address,
      object defaults)
    {
      return PlatformRouteHelpers.MapTfsRoute(routes, name, hostType, address, defaults, (string[]) null);
    }

    public static TfsRoute MapTfsRoute(
      this RouteCollection routes,
      string name,
      TeamFoundationHostType hostType,
      string address,
      string[] namespaces)
    {
      return routes.MapTfsRoute(name, hostType, address, (object) null, (object) null, namespaces);
    }

    public static TfsRoute MapTfsRoute(
      this RouteCollection routes,
      string name,
      TeamFoundationHostType hostType,
      string address,
      object defaults,
      object constraints)
    {
      return routes.MapTfsRoute(name, hostType, address, defaults, constraints, (string[]) null);
    }

    public static TfsRoute MapTfsRoute(
      this RouteCollection routes,
      string name,
      TeamFoundationHostType hostType,
      string address,
      object defaults,
      string[] namespaces)
    {
      return routes.MapTfsRoute(name, hostType, address, defaults, (object) null, namespaces);
    }

    public static TfsRoute MapTfsRoute(
      this RouteCollection routes,
      string name,
      TeamFoundationHostType hostType,
      string address,
      object defaults,
      object constraints,
      string[] namespaces)
    {
      if (routes == null)
        throw new ArgumentNullException(nameof (routes));
      if (address == null)
        throw new ArgumentNullException(nameof (address));
      TfsRoute tfsRoute = new TfsRoute(hostType, address, PlatformRouteHelpers.ToRouteDictionary(defaults), PlatformRouteHelpers.ToRouteDictionary(constraints), new RouteValueDictionary(), (IRouteHandler) new MvcRouteHandler());
      if (namespaces != null && namespaces.Length != 0)
        tfsRoute.DataTokens["Namespaces"] = (object) namespaces;
      routes.Add(name, (RouteBase) tfsRoute);
      return tfsRoute;
    }

    private static TfsRoute[] MapTfsNavRoutes(
      this RouteCollection routes,
      string name,
      TeamFoundationHostType hostType,
      string routeUrl,
      string routeArea)
    {
      string str = "";
      if (!string.IsNullOrEmpty(routeArea))
        str = "_" + routeArea.ToLowerInvariant() + "/";
      TfsRoute route1 = routes.MapTfsRoute(name, hostType, routeUrl + str, PlatformRouteHelpers.sm_defaults, (object) PlatformRouteHelpers.sm_navigationConstraints);
      TfsRoute route2 = routes.MapTfsRoute(name + "ControllerAction", hostType, routeUrl + str + string.Format((IFormatProvider) CultureInfo.InvariantCulture, "_{{{0}}}/{{{1}}}", (object) "controller", (object) "action"), PlatformRouteHelpers.sm_defaults, (object) PlatformRouteHelpers.sm_navigationConstraints);
      TfsRoute route3 = routes.MapTfsRoute(name + "ControllerActionWithParameters", hostType, routeUrl + str + string.Format((IFormatProvider) CultureInfo.InvariantCulture, "_{{{0}}}/{{{1}}}/{{*{2}}}", (object) "controller", (object) "action", (object) "parameters"), PlatformRouteHelpers.sm_defaults, (object) PlatformRouteHelpers.sm_navigationConstraints);
      route1.SetRouteArea(routeArea);
      route2.SetRouteArea(routeArea);
      route3.SetRouteArea(routeArea);
      return new TfsRoute[3]{ route1, route2, route3 };
    }

    public static void RegisterRouteArea(this RouteCollection routes, string routeArea)
    {
      ArgumentUtility.CheckForNull<RouteCollection>(routes, nameof (routes));
      ArgumentUtility.CheckForNull<string>(routeArea, nameof (routeArea));
      TfsRouteArea.RegisterRouteArea(routeArea);
      routes.MapTfsNavRoutes(routeArea + "Team", TeamFoundationHostType.ProjectCollection, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{{{0}}}/{{{1}}}/", (object) "project", (object) "team"), routeArea);
      routes.MapTfsNavRoutes(routeArea + "Project", TeamFoundationHostType.ProjectCollection, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{{{0}}}/", (object) "project"), routeArea);
      routes.MapTfsNavRoutes(routeArea + "ServiceHost", TeamFoundationHostType.All, "", routeArea);
    }

    public static TfsRoute MapTfsAreaRoute(
      this AreaRegistrationContext context,
      string name,
      TeamFoundationHostType hostType,
      string address)
    {
      return PlatformRouteHelpers.MapTfsAreaRoute(context, name, hostType, address, (string[]) null);
    }

    public static TfsRoute MapTfsAreaRoute(
      this AreaRegistrationContext context,
      string name,
      TeamFoundationHostType hostType,
      string address,
      object defaults)
    {
      return PlatformRouteHelpers.MapTfsAreaRoute(context, name, hostType, address, defaults, (string[]) null);
    }

    public static TfsRoute MapTfsAreaRoute(
      this AreaRegistrationContext context,
      string name,
      TeamFoundationHostType hostType,
      string address,
      string[] namespaces)
    {
      return PlatformRouteHelpers.MapTfsAreaRoute(context, name, hostType, address, (object) null, namespaces);
    }

    public static TfsRoute MapTfsAreaRoute(
      this AreaRegistrationContext context,
      string name,
      TeamFoundationHostType hostType,
      string address,
      object defaults,
      object constraints)
    {
      return context.MapTfsAreaRoute(name, hostType, address, defaults, constraints, (string[]) null);
    }

    public static TfsRoute MapTfsAreaRoute(
      this AreaRegistrationContext context,
      string name,
      TeamFoundationHostType hostType,
      string address,
      object defaults,
      string[] namespaces)
    {
      return context.MapTfsAreaRoute(name, hostType, address, defaults, (object) null, namespaces);
    }

    public static TfsRoute MapTfsAreaRoute(
      this AreaRegistrationContext context,
      string name,
      TeamFoundationHostType hostType,
      string address,
      object defaults,
      object constraints,
      string[] namespaces)
    {
      if (context == null)
        throw new ArgumentNullException(nameof (context));
      if (namespaces == null && context.Namespaces != null)
        namespaces = context.Namespaces.ToArray<string>();
      TfsRoute tfsRoute = context.Routes.MapTfsRoute(name, hostType, address, defaults, constraints, namespaces);
      tfsRoute.DataTokens["area"] = (object) context.AreaName;
      tfsRoute.DataTokens["UseNamespaceFallback"] = (object) (bool) (namespaces == null ? 1 : (namespaces.Length == 0 ? 1 : 0));
      return tfsRoute;
    }

    public static T GetRouteValue<T>(this RouteData routeData, string key) => routeData.GetRouteValue<T>(key, default (T));

    public static T GetRouteValue<T>(this RouteData routeData, string key, T defaultValue)
    {
      object obj = (object) null;
      if (!routeData.Values.TryGetValue(key, out obj))
        routeData.DataTokens.TryGetValue(key, out obj);
      return obj == null ? defaultValue : (T) obj;
    }

    public static T GetRouteValueLazy<T>(
      this RouteData routeData,
      string key,
      Lazy<T> lazyDefaultValue)
    {
      object obj = (object) null;
      if (!routeData.Values.TryGetValue(key, out obj))
        routeData.DataTokens.TryGetValue(key, out obj);
      return obj == null ? lazyDefaultValue.Value : (T) obj;
    }

    public static T GetValue<T>(this RouteValueDictionary values, string key) => values.GetValue<T>(key, default (T));

    public static T GetValue<T>(this RouteValueDictionary values, string key, T defaultValue)
    {
      object obj = (object) null;
      return !values.TryGetValue(key, out obj) ? defaultValue : (T) obj;
    }

    public static RouteValueDictionary ToRouteDictionary(object values) => values is RouteValueDictionary ? (RouteValueDictionary) values : new RouteValueDictionary(values);

    public static RouteData Clone(this RouteData routeData)
    {
      RouteData routeData1 = new RouteData()
      {
        Route = routeData.Route,
        RouteHandler = routeData.RouteHandler
      };
      foreach (KeyValuePair<string, object> dataToken in routeData.DataTokens)
        routeData1.DataTokens.Add(dataToken.Key, dataToken.Value);
      foreach (KeyValuePair<string, object> keyValuePair in routeData.Values)
        routeData1.Values.Add(keyValuePair.Key, keyValuePair.Value);
      return routeData1;
    }

    public static string ExtractTargetRouteArea(
      RequestContext requestContext,
      RouteValueDictionary values)
    {
      string targetRouteArea = (string) null;
      if (values != null)
        targetRouteArea = values.GetValue<string>("routeArea", (string) null);
      if (targetRouteArea == null && requestContext.RouteData != null)
        targetRouteArea = requestContext.RouteData.GetRouteArea();
      return targetRouteArea;
    }

    public static string GetControllerActionRouteName(RouteValueDictionary routeValues, string area = "")
    {
      string controllerActionRouteName = string.Empty;
      if (routeValues != null)
      {
        object obj;
        controllerActionRouteName = !routeValues.TryGetValue("team", out obj) || string.IsNullOrEmpty(obj as string) ? (!routeValues.TryGetValue("project", out obj) || string.IsNullOrEmpty(obj as string) ? "ServiceHostControllerAction" : "ProjectControllerAction") : "TeamControllerAction";
        if (!string.IsNullOrEmpty(area))
          controllerActionRouteName = area + controllerActionRouteName;
      }
      return controllerActionRouteName;
    }
  }
}
