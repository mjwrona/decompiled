// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.NavigationHelpers
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Platform, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A6A2C403-5081-466C-A570-9B50BFA8E213
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Platform.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess.Routing;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;
using Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server.Contributions;
using System;
using System.Web.Routing;

namespace Microsoft.TeamFoundation.Server.WebAccess
{
  public static class NavigationHelpers
  {
    private const string RequestProjectIdProperty = "ProjectId";
    private const string RequestTeamIdProperty = "TeamId";

    public static RouteValueDictionary GetCurrentNavigationLevelRouteValues(
      NavigationContext navigationContext)
    {
      RouteValueDictionary levelRouteValues = new RouteValueDictionary();
      if (!string.IsNullOrEmpty(navigationContext.Project))
        levelRouteValues["project"] = (object) navigationContext.Project;
      if (!string.IsNullOrEmpty(navigationContext.Team))
        levelRouteValues["team"] = (object) navigationContext.Team;
      return levelRouteValues;
    }

    public static void SetRequestNavigationProperties(WebContext webContext)
    {
      if (webContext.ProjectContext == null)
        return;
      webContext.TfsRequestContext.RootContext.Items["ProjectId"] = (object) webContext.ProjectContext.Id;
      if (webContext.TeamContext == null)
        return;
      webContext.TfsRequestContext.RootContext.Items["TeamId"] = (object) webContext.TeamContext.Id;
    }

    public static Guid GetRequestProjectId(IVssRequestContext requestContext)
    {
      object obj;
      return requestContext.RootContext.Items.TryGetValue("ProjectId", out obj) && obj is Guid guid ? guid : Guid.Empty;
    }

    public static Guid GetRequestTeamId(IVssRequestContext requestContext)
    {
      object obj;
      return requestContext.RootContext.Items.TryGetValue("TeamId", out obj) && obj is Guid guid ? guid : Guid.Empty;
    }

    public static string GetHubDefaultUrl(
      WebContext webContext,
      Contribution hubContribution,
      RouteValueDictionary routeValues)
    {
      webContext.TfsRequestContext.GetService<IContributionRoutingService>();
      string property = hubContribution.GetProperty<string>("defaultRoute");
      return NavigationHelpers.GetHubDefaultUrl(webContext, property, routeValues);
    }

    public static string GetHubDefaultUrl(
      WebContext webContext,
      string routeId,
      RouteValueDictionary routeValues)
    {
      IContributionRoutingService service = webContext.TfsRequestContext.GetService<IContributionRoutingService>();
      return !string.IsNullOrEmpty(routeId) ? service.RouteUrl(webContext.TfsRequestContext, routeId, routeValues) : (string) null;
    }

    public static NavigationContextLevels CalculateTopMostLevel(RequestContext requestContext)
    {
      ArgumentUtility.CheckForNull<RequestContext>(requestContext, nameof (requestContext));
      IVssRequestContext requestContext1 = requestContext.TfsRequestContext();
      if (requestContext1 != null)
      {
        switch (requestContext1.IntendedHostType())
        {
          case TeamFoundationHostType.Deployment:
            return NavigationContextLevels.Deployment;
          case TeamFoundationHostType.Application:
            return NavigationContextLevels.Application;
          case TeamFoundationHostType.ProjectCollection:
            RouteData routeData = requestContext.RouteData;
            if (routeData == null || string.IsNullOrEmpty(routeData.GetRouteValue<string>("project", (string) null)))
              return NavigationContextLevels.Collection;
            return !string.IsNullOrEmpty(routeData.GetRouteValue<string>("team", (string) null)) ? NavigationContextLevels.Team : NavigationContextLevels.Project;
        }
      }
      return NavigationContextLevels.None;
    }
  }
}
