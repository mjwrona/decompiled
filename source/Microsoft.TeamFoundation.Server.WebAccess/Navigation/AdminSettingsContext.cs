// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Navigation.AdminSettingsContext
// Assembly: Microsoft.TeamFoundation.Server.WebAccess, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A2CCA8C5-6910-48A5-82D9-BDC1350B5B4D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.TeamFoundation.Server.Types.Team;
using Microsoft.TeamFoundation.Server.WebAccess.Routing;
using Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server.Contributions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web.Routing;

namespace Microsoft.TeamFoundation.Server.WebAccess.Navigation
{
  [DataContract]
  public class AdminSettingsContext : HeaderItemContext
  {
    public AdminSettingsContext(IVssRequestContext requestContext)
      : base(50)
    {
      this.Available = AdminSettingsContext.AdminActionsAllowed(requestContext);
    }

    public override void AddActions(IVssRequestContext requestContext)
    {
      foreach (HeaderAction allAction in AdminSettingsContext.GetAllActions(requestContext))
        this.AddAction(allAction.Id, allAction);
    }

    public static IEnumerable<HeaderAction> GetAllActions(IVssRequestContext requestContext)
    {
      List<HeaderAction> source = new List<HeaderAction>();
      if (!AdminSettingsContext.AdminActionsAllowed(requestContext))
        return (IEnumerable<HeaderAction>) source;
      IContributionRoutingService service1 = requestContext.GetService<IContributionRoutingService>();
      string routeValue = service1.GetRouteValue<string>(requestContext, "project");
      bool flag = false;
      if (routeValue != null)
      {
        flag = true;
        if (service1.GetRouteValue<string>(requestContext, "team") != null)
        {
          source.Add(AdminSettingsContext.GetProjectAction(requestContext));
        }
        else
        {
          IProjectService service2 = requestContext.GetService<IProjectService>();
          if (!Guid.TryParse(routeValue, out Guid _))
          {
            try
            {
              ProjectInfo project = service2.GetProject(requestContext, routeValue);
              if (project != null)
              {
                WebApiTeam defaultTeam = requestContext.GetService<ITeamService>().GetDefaultTeam(requestContext, project.Id);
                if (defaultTeam != null)
                  source.Add(AdminSettingsContext.GetAdminAction(requestContext, "navigate-to-team-settings", NavigationContextLevels.Team, new RouteValueDictionary()
                  {
                    {
                      "team",
                      (object) defaultTeam.Name
                    }
                  }));
              }
            }
            catch (ProjectDoesNotExistWithNameException ex)
            {
            }
          }
        }
      }
      if (flag)
        source.Add(AdminSettingsContext.GetCollectionAction(requestContext));
      if (!requestContext.ExecutionEnvironment.IsHostedDeployment && requestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection))
        source.Add(AdminSettingsContext.GetServerAction(requestContext));
      return source.Where<HeaderAction>((Func<HeaderAction, bool>) (a => a != null));
    }

    private static HeaderAction GetAdminAction(
      IVssRequestContext requestContext,
      string actionId,
      NavigationContextLevels level,
      RouteValueDictionary routeValues = null)
    {
      HeaderAction adminAction = new HeaderAction()
      {
        Id = actionId
      };
      switch (level)
      {
        case NavigationContextLevels.Collection:
          adminAction.Text = requestContext.ExecutionEnvironment.IsHostedDeployment ? WACommonResources.AccountSettings : WACommonResources.CollectionSettings;
          break;
        case NavigationContextLevels.Project:
          adminAction.Text = WACommonResources.ProjectSettings;
          break;
        case NavigationContextLevels.Team:
          adminAction.Text = WACommonResources.TeamSettings;
          break;
        default:
          adminAction.Text = requestContext.ExecutionEnvironment.IsHostedDeployment ? WACommonResources.AccountSettings : WACommonResources.ServerSettings;
          break;
      }
      if (routeValues == null)
        routeValues = new RouteValueDictionary();
      routeValues["routeArea"] = (object) "Admin";
      routeValues["serviceHost"] = level == NavigationContextLevels.Deployment ? (object) requestContext.ServiceHost.DeploymentServiceHost : (object) requestContext.ServiceHost;
      if (level < NavigationContextLevels.Project)
        routeValues["project"] = (object) null;
      if (level < NavigationContextLevels.Team)
        routeValues["team"] = (object) null;
      IContributionRoutingService service = requestContext.GetService<IContributionRoutingService>();
      string levelRouteName = TfsRouteHelpers.GetLevelRouteName(level, "Admin");
      adminAction.Url = service.RouteUrl(requestContext, levelRouteName, "index", "home", routeValues);
      return adminAction;
    }

    private static HeaderAction GetServerAction(IVssRequestContext requestContext) => AdminSettingsContext.GetAdminAction(requestContext, "navigate-to-server-settings", requestContext.ExecutionEnvironment.IsHostedDeployment ? NavigationContextLevels.Application : NavigationContextLevels.Deployment);

    private static HeaderAction GetCollectionAction(IVssRequestContext requestContext) => AdminSettingsContext.GetAdminAction(requestContext, "navigate-to-collection-settings", NavigationContextLevels.Collection);

    private static HeaderAction GetProjectAction(IVssRequestContext requestContext) => AdminSettingsContext.GetAdminAction(requestContext, "navigate-to-project-settings", NavigationContextLevels.Project);

    public static bool AdminActionsAllowed(IVssRequestContext requestContext) => HeaderActionHelpers.HasClaim(requestContext, "member") && HeaderPermissionChecks.UserHasAdminSettingsPermissions(requestContext);
  }
}
