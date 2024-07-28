// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Web.Routing.GitHubConnectionRoutes
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CCDEBCB9-DB6B-41A2-8797-78C2455AE321
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Web.dll

using Microsoft.Azure.Boards.WebApi.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Core;
using System.Web.Http;
using System.Web.Http.Routing.Constraints;

namespace Microsoft.TeamFoundation.WorkItemTracking.Web.Routing
{
  public class GitHubConnectionRoutes : IVssApiResourceProvider
  {
    public void RegisterResources(
      IVssRequestContext requestContext,
      ResourceAreaCollection areas,
      HttpRouteCollection routes)
    {
      routes.MapResourceRoute(TfsApiResourceScope.Project, WorkItemTrackingLocationIds.GitHubConnectionReposBatch, "wit", "githubConnections", "githubconnections/{connectionId}/reposBatch", VssRestApiVersion.v7_1, constraints: (object) new
      {
        connectionId = new GuidRouteConstraint()
      }, routeName: "githubConnectionReposBatch");
      routes.MapResourceRoute(TfsApiResourceScope.Project, WorkItemTrackingLocationIds.GitHubConnectionRepos, "wit", "githubConnections", "githubconnections/{connectionId}/repos", VssRestApiVersion.v7_1, constraints: (object) new
      {
        connectionId = new GuidRouteConstraint()
      }, routeName: "githubConnectionRepos");
      routes.MapResourceRoute(TfsApiResourceScope.Project, WorkItemTrackingLocationIds.GitHubConnections, "wit", "githubConnections", "githubconnections", VssRestApiVersion.v7_1);
    }
  }
}
