// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Platform.RouteCollectionExtensions
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Platform, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A6A2C403-5081-466C-A570-9B50BFA8E213
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Platform.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.Routing;
using Microsoft.VisualStudio.Services.Common;
using System.Web.Mvc;
using System.Web.Routing;

namespace Microsoft.TeamFoundation.Server.WebAccess.Platform
{
  public static class RouteCollectionExtensions
  {
    public static Route MapViewRoute(
      this RouteCollection routes,
      TeamFoundationHostType hostType,
      string name,
      string routeTemplate,
      object defaults)
    {
      return routes.MapViewRoute(hostType, name, routeTemplate, defaults, (object) null);
    }

    public static Route MapViewRoute(
      this RouteCollection routes,
      TeamFoundationHostType hostType,
      string name,
      string routeTemplate,
      object defaults,
      object constraints)
    {
      return routes.MapViewRouteInternal(hostType, name, routeTemplate, defaults, constraints, (string[]) null);
    }

    public static Route MapViewRoute(
      this RouteCollection routes,
      TeamFoundationHostType hostType,
      string name,
      string routeTemplate,
      object defaults,
      object constraints,
      string[] namespaces)
    {
      return routes.MapViewRouteInternal(hostType, name, routeTemplate, defaults, constraints, namespaces);
    }

    private static Route MapViewRouteInternal(
      this RouteCollection routes,
      TeamFoundationHostType hostType,
      string name,
      string routeTemplate,
      object defaults,
      object constraints,
      string[] namespaces)
    {
      ArgumentUtility.CheckForNull<RouteCollection>(routes, nameof (routes));
      ArgumentUtility.CheckForNull<string>(routeTemplate, nameof (routeTemplate));
      RouteValueDictionary defaults1 = new RouteValueDictionary(defaults);
      RouteValueDictionary constraints1 = new RouteValueDictionary(constraints);
      if (hostType != TeamFoundationHostType.Unknown)
        constraints1["TFS_HostType"] = (object) new TfsApiHostTypeConstraint(hostType);
      RouteValueDictionary dataTokens = new RouteValueDictionary();
      if (namespaces != null && namespaces.Length != 0)
        dataTokens["Namespaces"] = (object) namespaces;
      Route route = (Route) new VssfMVCRoute(routeTemplate, defaults1, constraints1, dataTokens, (IRouteHandler) new MvcRouteHandler());
      routes.Add(name, (RouteBase) route);
      return route;
    }
  }
}
