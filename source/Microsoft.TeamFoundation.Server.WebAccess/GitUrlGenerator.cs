// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.GitUrlGenerator
// Assembly: Microsoft.TeamFoundation.Server.WebAccess, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A2CCA8C5-6910-48A5-82D9-BDC1350B5B4D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.dll

using Microsoft.TeamFoundation.Git.Server.Routing;
using System;
using System.Collections.Generic;
using System.Web.Routing;

namespace Microsoft.TeamFoundation.Server.WebAccess
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
      string controllerActionRouteName = GitUrlGenerator.GetGitControllerActionRouteName(tfsWebContext.NavigationContext.TopMostLevel, routeValues);
      if (tfsWebContext.NavigationContext.TopMostLevel == NavigationContextLevels.Project && string.Equals(projectName, repositoryName))
        controllerActionRouteName = GitUrlGenerator.GetGitControllerActionRouteName(NavigationContextLevels.Collection, routeValues);
      RouteValueDictionary routeValues1 = GitUrlGenerator.PrepareRouteValues(tfsWebContext, projectName, repositoryName, routeValues);
      return UrlHelperExtensions.RouteUrl(tfsWebContext.Url, controllerActionRouteName, action, "git", routeValues1);
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
      TfsWebContext tfsWebContext,
      string projectName,
      string repositoryName,
      RouteValueDictionary routeValues)
    {
      RouteValueDictionary routeValueDictionary = new RouteValueDictionary();
      routeValueDictionary["GitRouteType"] = (object) GitRouteType.WebAccess;
      routeValueDictionary["GitRepositoryName"] = (object) repositoryName;
      routeValueDictionary["routeArea"] = (object) "";
      routeValueDictionary["parameters"] = (object) null;
      if (string.IsNullOrEmpty(tfsWebContext.NavigationContext.Team) || !string.Equals(projectName, tfsWebContext.NavigationContext.Project, StringComparison.OrdinalIgnoreCase))
      {
        routeValueDictionary["team"] = (object) null;
        routeValueDictionary["project"] = string.IsNullOrEmpty(projectName) || string.Equals(projectName, repositoryName) ? (object) null : (object) projectName;
      }
      else
      {
        routeValueDictionary["project"] = (object) projectName;
        routeValueDictionary["team"] = (object) tfsWebContext.NavigationContext.Team;
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
