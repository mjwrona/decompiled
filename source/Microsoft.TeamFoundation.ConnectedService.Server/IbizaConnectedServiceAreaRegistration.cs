// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.ConnectedService.Server.IbizaConnectedServiceAreaRegistration
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
  public sealed class IbizaConnectedServiceAreaRegistration : AreaRegistration
  {
    public override string AreaName => "connectedService";

    public override void RegisterArea(AreaRegistrationContext context)
    {
      if (context == null)
        throw new ArgumentNullException(nameof (context));
      this.RegisterRoutes(context.Routes);
    }

    private void RegisterRoutes(RouteCollection routes)
    {
      this.AddRoute(routes, "authRequest", "authRequest");
      this.AddRoute(routes, "callback", "callback");
    }

    private void AddRoute(
      RouteCollection routes,
      string routeTemplate,
      string action,
      TeamFoundationHostType hostType = TeamFoundationHostType.Deployment)
    {
      string name = "IbizaConnectedServiceProviderEndpoint_" + action;
      routeTemplate = "_admin/OAuthProvider/{providerType}/" + routeTemplate;
      routes.MapTfsRoute(name, hostType, routeTemplate, (object) new RouteValueDictionary()
      {
        {
          "controller",
          (object) "IbizaConnectedServiceProvider"
        },
        {
          nameof (action),
          (object) action
        }
      });
    }
  }
}
