// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.ConnectedService.Server.OAuth2CallbackAreaRegistration
// Assembly: Microsoft.TeamFoundation.ConnectedService.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 5BEB400C-7A81-4197-B897-D0116BC50257
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.ConnectedService.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess.Routing;
using System;
using System.Web.Mvc;
using System.Web.Routing;

namespace Microsoft.TeamFoundation.ConnectedService.Server
{
  public class OAuth2CallbackAreaRegistration : AreaRegistration
  {
    public override string AreaName => "OAuth2";

    public override void RegisterArea(AreaRegistrationContext context)
    {
      if (context == null)
        throw new ArgumentNullException(nameof (context));
      OAuth2CallbackAreaRegistration.RegisterRoutes(context.Routes);
    }

    private static void RegisterRoutes(RouteCollection routes) => OAuth2CallbackAreaRegistration.AddRoute(routes, "Callback", "Callback");

    private static void AddRoute(
      RouteCollection routes,
      string routeTemplate,
      string action,
      TeamFoundationHostType hostType = TeamFoundationHostType.ProjectCollection)
    {
      string name = "Endpoint" + action;
      routeTemplate = "_admin/oauth2/" + routeTemplate;
      routes.MapTfsRoute(name, hostType, routeTemplate, (object) new RouteValueDictionary()
      {
        {
          "controller",
          (object) "OAuth2Endpoint"
        },
        {
          nameof (action),
          (object) action
        }
      });
    }
  }
}
