// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Platform.WebPlatformModule
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Platform, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A6A2C403-5081-466C-A570-9B50BFA8E213
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Platform.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess.Routing;
using System.Collections.Generic;
using System.Web.Routing;

namespace Microsoft.TeamFoundation.Server.WebAccess.Platform
{
  public class WebPlatformModule : WebModule<VisualStudioServicesApplication>
  {
    protected virtual IEnumerable<string> RouteAreas => (IEnumerable<string>) new string[2]
    {
      "",
      "Public"
    };

    protected override void RegisterDefaultRoutes(RouteCollection routes)
    {
      base.RegisterDefaultRoutes(routes);
      routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
      routes.MapViewRoute(TeamFoundationHostType.Unknown, "ScriptResource", "_static/tfs/{staticContentVersion}/" + PlatformHtmlExtensions.ScriptContentFolder + "/TFS/{flavor}/{__loc}/{modulePrefix}.Resources.{resourceName}.js", (object) new
      {
        controller = "ScriptResource",
        flavor = "debug",
        action = "Index"
      }, (object) new
      {
        resourceName = "[a-zA-z_]([.]?[a-zA-Z_]+)*"
      });
      this.RegisterRouteAreas(routes);
      this.RegisterWebModuleRoutes(routes);
      routes.MapViewRoute(TeamFoundationHostType.All, "FallbackRoute", "{*params}", (object) new
      {
        controller = "error",
        action = "notFound"
      });
    }

    protected virtual void RegisterWebModuleRoutes(RouteCollection routes)
    {
    }

    protected virtual void RegisterRouteAreas(RouteCollection routes)
    {
      if (this.RouteAreas == null)
        return;
      foreach (string routeArea in this.RouteAreas)
        routes.RegisterRouteArea(routeArea);
    }
  }
}
