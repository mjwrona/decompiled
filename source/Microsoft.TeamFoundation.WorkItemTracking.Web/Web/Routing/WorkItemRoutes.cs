// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Web.Routing.WorkItemRoutes
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CCDEBCB9-DB6B-41A2-8797-78C2455AE321
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Web.dll

using Microsoft.Azure.Boards.WebApi.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Core;
using System;
using System.Web.Http;

namespace Microsoft.TeamFoundation.WorkItemTracking.Web.Routing
{
  public class WorkItemRoutes : IVssApiResourceProvider
  {
    public void RegisterResources(
      IVssRequestContext requestContext,
      ResourceAreaCollection areas,
      HttpRouteCollection routes)
    {
      areas.RegisterArea("wit", WorkItemTrackingWebConstants.RestAreaId);
      routes.MapResourceRoute(TfsApiResourceScope.Collection | TfsApiResourceScope.Project, WorkItemTrackingLocationIds.Revisions, "wit", "revisions", "{area}/workItems/{id}/revisions/{revisionNumber}", VssRestApiVersion.v1_0, VssRestApiReleaseState.Released, 3, (object) new
      {
        resource = "revisions",
        revisionNumber = RouteParameter.Optional
      }, (object) new
      {
        id = "\\0*[1-9][0-9]*",
        revisionNumber = "(\\0*[1-9][0-9]*)*"
      }, "revisions");
      HttpRouteCollection routes1 = routes;
      Guid updates = WorkItemTrackingLocationIds.Updates;
      var defaults1 = new
      {
        resource = "updates",
        updateNumber = RouteParameter.Optional
      };
      var constraints1 = new
      {
        id = "\\0*[1-9][0-9]*",
        updateNumber = "(\\0*[1-9][0-9]*)*"
      };
      VssRestApiVersion? nullable = new VssRestApiVersion?();
      VssRestApiVersion? deprecatedAtVersion1 = nullable;
      routes1.MapResourceRoute(TfsApiResourceScope.Collection | TfsApiResourceScope.Project, updates, "wit", "updates", "{area}/workItems/{id}/updates/{updateNumber}", VssRestApiVersion.v1_0, VssRestApiReleaseState.Released, 4, (object) defaults1, (object) constraints1, "updates", deprecatedAtVersion: deprecatedAtVersion1);
      HttpRouteCollection routes2 = routes;
      Guid history = WorkItemTrackingLocationIds.History;
      nullable = new VssRestApiVersion?(VssRestApiVersion.v3_0);
      var defaults2 = new
      {
        resource = "history",
        revisionNumber = RouteParameter.Optional
      };
      var constraints2 = new
      {
        id = "\\0*[1-9][0-9]*",
        revisionNumber = "(\\0*[1-9][0-9]*)*"
      };
      VssRestApiVersion? deprecatedAtVersion2 = nullable;
      routes2.MapResourceRoute(TeamFoundationHostType.ProjectCollection, history, "wit", "history", "{area}/workItems/{id}/history/{revisionNumber}", VssRestApiVersion.v1_0, VssRestApiReleaseState.Released, 2, (object) defaults2, (object) constraints2, "history", deprecatedAtVersion: deprecatedAtVersion2);
      HttpRouteCollection routes3 = routes;
      Guid comments = WorkItemTrackingLocationIds.Comments;
      nullable = new VssRestApiVersion?(VssRestApiVersion.v5_0);
      var defaults3 = new
      {
        resource = "comments",
        revision = RouteParameter.Optional
      };
      var constraints3 = new
      {
        id = "\\0*[1-9][0-9]*",
        revision = "(\\0*[1-9][0-9]*)*"
      };
      VssRestApiVersion? deprecatedAtVersion3 = nullable;
      routes3.MapResourceRoute(TfsApiResourceScope.Collection | TfsApiResourceScope.Project, comments, "wit", "comments", "{area}/workItems/{id}/comments/{revision}", VssRestApiVersion.v3_0, maxResourceVersion: 2, defaults: (object) defaults3, constraints: (object) constraints3, routeName: "comments", deprecatedAtVersion: deprecatedAtVersion3);
      routes.MapResourceRoute(TfsApiResourceScope.Collection | TfsApiResourceScope.Project, WorkItemTrackingLocationIds.WorkItemsBatch, "wit", "workItemsBatch", "{area}/{resource}", VssRestApiVersion.v5_0, VssRestApiReleaseState.Released);
      routes.MapResourceRoute(TfsApiResourceScope.Collection | TfsApiResourceScope.Project, WorkItemTrackingLocationIds.WorkItems, "wit", "workItems", "{area}/{resource}/{id}", VssRestApiVersion.v1_0, VssRestApiReleaseState.Released, 3, (object) new
      {
        id = RouteParameter.Optional
      }, (object) new{ id = "(\\0*[1-9][0-9]*)*" });
      routes.MapResourceRoute(TfsApiResourceScope.Project, WorkItemTrackingLocationIds.WorkItemTemplate, "wit", "workItems", "{area}/{resource}/${type}", VssRestApiVersion.v1_0, VssRestApiReleaseState.Released, 3, routeName: "workItemTemplate");
    }
  }
}
