// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.ReleaseManagement2.ReleaseAreaRegistration
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.ReleaseManagement2, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B374520F-138F-4DCB-BCF6-50FBC8C65346
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.ReleaseManagement2.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess.Routing;
using System;
using System.Web.Mvc;
using System.Web.Routing;

namespace Microsoft.TeamFoundation.Server.WebAccess.ReleaseManagement2
{
  public class ReleaseAreaRegistration : AreaRegistration
  {
    public override string AreaName => "ReleaseManagement";

    public override void RegisterArea(AreaRegistrationContext context)
    {
      if (context == null)
        throw new ArgumentNullException(nameof (context));
      ReleaseAreaRegistration.RegisterRoutes(context.Routes);
    }

    private static void RegisterRoutes(RouteCollection routes) => ReleaseAreaRegistration.AddRoute(routes, "AadOauthCallback", "AadOauthCallback");

    private static void AddRoute(
      RouteCollection routes,
      string routeTemplate,
      string action,
      TeamFoundationHostType hostType = TeamFoundationHostType.ProjectCollection)
    {
      string name = "Release" + action;
      routeTemplate = "_release/" + routeTemplate;
      routes.MapTfsRoute(name, hostType, routeTemplate, (object) new RouteValueDictionary()
      {
        {
          "controller",
          (object) "Release"
        },
        {
          nameof (action),
          (object) action
        }
      });
    }
  }
}
