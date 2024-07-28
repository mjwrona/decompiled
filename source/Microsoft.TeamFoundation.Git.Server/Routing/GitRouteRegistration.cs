// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.Routing.GitRouteRegistration
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.Routing;
using System.Collections.Generic;
using System.Web;
using System.Web.Routing;

namespace Microsoft.TeamFoundation.Git.Server.Routing
{
  public static class GitRouteRegistration
  {
    public static void RegisterGitRoutes(
      GitRouteType type,
      string name,
      string postRepositoryPath,
      RouteValueDictionary defaults,
      IRouteHandler routeHandler)
    {
      GitRouteRegistration.InstantiateRouteMethod createRouteMethod = (GitRouteRegistration.InstantiateRouteMethod) ((address, defaultValues, constraints, tokens, handler) => (Route) new VssfMVCRoute(address, defaultValues, constraints, tokens, handler));
      RouteBase routeBase = RouteTable.Routes["GitAggregateRoute"];
      if (routeBase == null)
      {
        routeBase = (RouteBase) new GroupedMVCRoute("_git");
        RouteTable.Routes.Add("GitAggregateRoute", routeBase);
      }
      RouteCollection routes = ((GroupedMVCRoute) routeBase).Routes;
      string[] strArray = new string[3]
      {
        "_git",
        "_git/_full",
        "_git/_optimized"
      };
      foreach (string gitArea in strArray)
        GitRouteRegistration.AddGitRoutesInArea(routes, type, name, postRepositoryPath, defaults, (RouteValueDictionary) null, routeHandler, createRouteMethod, gitArea);
    }

    public static void RegisterWebGitRoutes(
      GitRouteType type,
      string name,
      string postRepositoryPath,
      RouteValueDictionary defaults,
      RouteValueDictionary constraints,
      IRouteHandler routeHandler,
      GitRouteRegistration.InstantiateRouteMethod createRouteMethod)
    {
      string[] strArray = new string[3]
      {
        "_git/_full",
        "_git/_optimized",
        "_git"
      };
      foreach (string gitArea in strArray)
        GitRouteRegistration.AddGitRoutesInArea(RouteTable.Routes, type, name, postRepositoryPath, defaults, constraints, routeHandler, createRouteMethod, gitArea);
    }

    private static void AddGitRoutesInArea(
      RouteCollection routes,
      GitRouteType type,
      string name,
      string postRepositoryPath,
      RouteValueDictionary defaults,
      RouteValueDictionary constraints,
      IRouteHandler routeHandler,
      GitRouteRegistration.InstantiateRouteMethod createRouteMethod,
      string gitArea)
    {
      GitRouteRegistration.AddGitRoutes(routes, type, name + gitArea, postRepositoryPath, defaults, constraints, routeHandler, createRouteMethod, gitArea, new RouteValueDictionary()
      {
        {
          "GitArea",
          (object) gitArea
        }
      });
    }

    private static void AddGitRoutes(
      RouteCollection routes,
      GitRouteType type,
      string name,
      string postRepositoryPath,
      RouteValueDictionary defaults,
      RouteValueDictionary constraints,
      IRouteHandler routeHandler,
      GitRouteRegistration.InstantiateRouteMethod createRouteMethod,
      string gitArea,
      RouteValueDictionary dataTokens)
    {
      GitRouteRegistration.AddRoute(routes, type, "VersionControl_Project_Team_Git_Repo_" + name, TeamFoundationHostType.ProjectCollection, string.Format("{{{0}}}/{{{1}}}/{2}/{{{3}}}{4}", (object) "project", (object) "team", (object) gitArea, (object) "GitRepositoryName", (object) postRepositoryPath), defaults, constraints, routeHandler, createRouteMethod, dataTokens);
      GitRouteRegistration.AddRoute(routes, type, "VersionControl_Project_Git_Repo_" + name, TeamFoundationHostType.ProjectCollection, string.Format("{{{0}}}/{1}/{{{2}}}{3}", (object) "project", (object) gitArea, (object) "GitRepositoryName", (object) postRepositoryPath), defaults, constraints, routeHandler, createRouteMethod, dataTokens);
      GitRouteRegistration.AddRoute(routes, type, "VersionControl_Git_Repo_" + name, TeamFoundationHostType.ProjectCollection, string.Format("{0}/{{{1}}}{2}", (object) gitArea, (object) "GitRepositoryName", (object) postRepositoryPath), defaults, constraints, routeHandler, createRouteMethod, dataTokens);
    }

    private static void AddRoute(
      RouteCollection routes,
      GitRouteType type,
      string name,
      TeamFoundationHostType hostType,
      string address,
      RouteValueDictionary defaults,
      RouteValueDictionary additionalConstraints,
      IRouteHandler routeHandler,
      GitRouteRegistration.InstantiateRouteMethod createRouteMethod,
      RouteValueDictionary dataTokens)
    {
      RouteValueDictionary constraints = new RouteValueDictionary();
      constraints.Add("serviceHost", (object) new GitRouteRegistration.GitRouteServiceHostConstraint(hostType));
      constraints.Add("GitRouteType", (object) new GitRouteRegistration.GitRouteTypeConstraint(type));
      constraints.Add("GitRepositoryName", (object) new GitRouteRegistration.RepositoryNameHelperConstraint());
      if (additionalConstraints != null)
      {
        foreach (KeyValuePair<string, object> additionalConstraint in additionalConstraints)
          constraints[additionalConstraint.Key] = additionalConstraint.Value;
      }
      Route route = createRouteMethod(address, defaults, constraints, dataTokens ?? new RouteValueDictionary(), routeHandler);
      routes.Add(name, (RouteBase) route);
    }

    private class RepositoryNameHelperConstraint : IRouteConstraint
    {
      public bool Match(
        HttpContextBase httpContext,
        Route route,
        string parameterName,
        RouteValueDictionary values,
        RouteDirection routeDirection)
      {
        string str = values.GetValue<string>("GitRepositoryName", (string) null);
        if (string.IsNullOrEmpty(str))
          return false;
        if (string.IsNullOrEmpty(values.GetValue<string>("project", (string) null)))
          values["project"] = (object) str;
        return true;
      }
    }

    private class GitRouteTypeConstraint : IRouteConstraint
    {
      private readonly GitRouteType _routeType;

      public GitRouteTypeConstraint(GitRouteType routeType) => this._routeType = routeType;

      public bool Match(
        HttpContextBase httpContext,
        Route route,
        string parameterName,
        RouteValueDictionary values,
        RouteDirection routeDirection)
      {
        return routeDirection != RouteDirection.UrlGeneration || this._routeType == values.GetValue<GitRouteType>("GitRouteType", GitRouteType.None);
      }
    }

    private class GitRouteServiceHostConstraint : IRouteConstraint
    {
      private readonly TeamFoundationHostType _hostType;

      public GitRouteServiceHostConstraint(TeamFoundationHostType hostType) => this._hostType = hostType;

      public bool Match(
        HttpContextBase httpContext,
        Route route,
        string parameterName,
        RouteValueDictionary values,
        RouteDirection routeDirection)
      {
        return httpContext.Items[(object) "IVssRequestContext"] is IVssRequestContext vssRequestContext && (vssRequestContext.ServiceHost.HostType & this._hostType) != 0;
      }
    }

    public delegate Route InstantiateRouteMethod(
      string address,
      RouteValueDictionary defaults,
      RouteValueDictionary constraints,
      RouteValueDictionary tokens,
      IRouteHandler handler);
  }
}
