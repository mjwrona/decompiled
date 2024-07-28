// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebServer.GitUrlGenerator
// Assembly: Microsoft.TeamFoundation.SourceControl.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 47C05AF5-4412-4EF0-BF63-D475D8EECD03
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.SourceControl.WebServer.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Git.Server.Routing;
using Microsoft.TeamFoundation.Server.WebAccess;
using Microsoft.TeamFoundation.WebPlatform.Server;
using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Microsoft.TeamFoundation.SourceControl.WebServer
{
  public static class GitUrlGenerator
  {
    internal const string GitActionParametersRouteName = "Action_Parameters";
    internal const string GitActionRouteName = "Action";
    internal const string GitDefaultRouteName = "DefaultAction";

    public static string GetActionUrl(
      TfsWebContext tfsWebContext,
      string repositoryName,
      string action)
    {
      return GitUrlGenerator.GetActionUrl(tfsWebContext, tfsWebContext.NavigationContext.Project, repositoryName, action, (RouteValueDictionary) null);
    }

    public static string GetActionUrl(
      TfsWebContext tfsWebContext,
      string projectName,
      string repositoryName,
      string action,
      object routeValues)
    {
      RouteValueDictionary routeValues1 = (RouteValueDictionary) null;
      if (routeValues != null)
        routeValues1 = routeValues is RouteValueDictionary ? (RouteValueDictionary) routeValues : new RouteValueDictionary(routeValues);
      return GitUrlGenerator.GetActionUrl(tfsWebContext, projectName, repositoryName, action, routeValues1);
    }

    public static string GetActionUrl(
      TfsWebContext tfsWebContext,
      string projectName,
      string repositoryName,
      string action,
      RouteValueDictionary routeValues)
    {
      return GitUrlGenerator.GetActionUrl(tfsWebContext.NavigationContext, projectName, repositoryName, action, routeValues);
    }

    public static string GetActionUrl(
      IVssRequestContext requestContext,
      string repositoryName,
      string action)
    {
      return GitUrlGenerator.GetActionUrl(new NavigationContext(requestContext, HttpContext.Current.Request.RequestContext), requestContext.GetService<IRequestProjectService>().GetProject(requestContext).Name, repositoryName, action, (RouteValueDictionary) null);
    }

    public static string GetActionUrl(
      NavigationContext navigationContext,
      string projectName,
      string repositoryName,
      string action,
      RouteValueDictionary routeValues)
    {
      string controllerActionRouteName = GitUrlGenerator.GetGitControllerActionRouteName(navigationContext.TopMostLevel, routeValues);
      if (navigationContext.TopMostLevel == NavigationContextLevels.Project && string.Equals(projectName, repositoryName))
        controllerActionRouteName = GitUrlGenerator.GetGitControllerActionRouteName(NavigationContextLevels.Collection, routeValues);
      RouteValueDictionary routeValues1 = GitUrlGenerator.PrepareRouteValues(navigationContext, projectName, repositoryName, routeValues);
      return Microsoft.TeamFoundation.Server.WebAccess.UrlHelperExtensions.RouteUrl(new UrlHelper(HttpContext.Current.Request.RequestContext), controllerActionRouteName, action, "git", routeValues1);
    }

    private static string GetGitControllerActionRouteName(
      NavigationContextLevels navigationLevel,
      RouteValueDictionary routeValues)
    {
      string str1;
      switch (navigationLevel)
      {
        case NavigationContextLevels.Project:
          str1 = "VersionControl_Project_Git_Repo_";
          break;
        case NavigationContextLevels.Team:
          str1 = "VersionControl_Project_Team_Git_Repo_";
          break;
        default:
          str1 = "VersionControl_Git_Repo_";
          break;
      }
      string str2 = routeValues == null || routeValues["parameters"] == null ? "Action" : "Action_Parameters";
      return str1 + str2 + "_git";
    }

    private static RouteValueDictionary PrepareRouteValues(
      NavigationContext navigationContext,
      string projectName,
      string repositoryName,
      RouteValueDictionary routeValues)
    {
      RouteValueDictionary routeValueDictionary = new RouteValueDictionary();
      routeValueDictionary["GitRouteType"] = (object) GitRouteType.WebAccess;
      routeValueDictionary["GitRepositoryName"] = (object) repositoryName;
      routeValueDictionary["routeArea"] = (object) "";
      routeValueDictionary["parameters"] = (object) null;
      if (string.IsNullOrEmpty(navigationContext.Team) || !string.Equals(projectName, navigationContext.Project, StringComparison.OrdinalIgnoreCase))
      {
        routeValueDictionary["team"] = (object) null;
        routeValueDictionary["project"] = string.IsNullOrEmpty(projectName) || string.Equals(projectName, repositoryName) ? (object) null : (object) projectName;
      }
      else
      {
        routeValueDictionary["project"] = (object) projectName;
        routeValueDictionary["team"] = (object) navigationContext.Team;
      }
      if (routeValues != null)
      {
        foreach (KeyValuePair<string, object> routeValue in routeValues)
          routeValueDictionary[routeValue.Key] = routeValue.Value;
      }
      return routeValueDictionary;
    }
  }
}
