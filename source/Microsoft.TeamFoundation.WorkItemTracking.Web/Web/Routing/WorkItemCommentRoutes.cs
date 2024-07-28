// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Web.Routing.WorkItemCommentRoutes
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CCDEBCB9-DB6B-41A2-8797-78C2455AE321
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Web.dll

using Microsoft.Azure.Boards.WebApi.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Core;
using System.Web.Http;

namespace Microsoft.TeamFoundation.WorkItemTracking.Web.Routing
{
  public class WorkItemCommentRoutes : IVssApiResourceProvider
  {
    public void RegisterResources(
      IVssRequestContext requestContext,
      ResourceAreaCollection areas,
      HttpRouteCollection routes)
    {
      routes.MapResourceRoute(TfsApiResourceScope.Project, WorkItemTrackingLocationIds.Comments2, "wit", "comments", "wit/workItems/{workItemId}/comments/{commentId}", VssRestApiVersion.v5_1, maxResourceVersion: 4, defaults: (object) new
      {
        resource = "comments",
        commentId = RouteParameter.Optional
      }, constraints: (object) new
      {
        workItemId = "\\0*[1-9][0-9]*",
        commentId = "(\\0*[1-9][0-9]*)*"
      }, routeName: "comments2");
      routes.MapResourceRoute(TfsApiResourceScope.Project, WorkItemTrackingLocationIds.CommentVersions, "wit", "commentsVersions", "wit/workItems/{workItemId}/comments/{commentId}/versions/{version}", VssRestApiVersion.v5_1, defaults: (object) new
      {
        resource = "commentsVersions",
        version = RouteParameter.Optional
      }, constraints: (object) new
      {
        workItemId = "\\0*[1-9][0-9]*",
        commentId = "\\0*[1-9][0-9]*",
        version = "(\\0*[1-9][0-9]*)*"
      }, routeName: "commentsVersions");
      routes.MapResourceRoute(TfsApiResourceScope.Project, WorkItemTrackingLocationIds.CommentReactions, "wit", "commentsReactions", "wit/workItems/{workItemId}/comments/{commentId}/reactions/{reactionType}", VssRestApiVersion.v5_1, defaults: (object) new
      {
        resource = "commentsReactions",
        reactionType = RouteParameter.Optional
      }, constraints: (object) new
      {
        workItemId = "\\0*[1-9][0-9]*",
        commentId = "\\0*[1-9][0-9]*"
      }, routeName: "commentsReactions");
      routes.MapResourceRoute(TfsApiResourceScope.Project, WorkItemTrackingLocationIds.CommentReactionsEngagedUsers, "wit", "commentReactionsEngagedUsers", "wit/workItems/{workItemId}/comments/{commentId}/reactions/{reactionType}/users", VssRestApiVersion.v5_1, defaults: (object) new
      {
        resource = "commentReactionsEngagedUsers"
      }, constraints: (object) new
      {
        workItemId = "\\0*[1-9][0-9]*",
        commentId = "\\0*[1-9][0-9]*"
      }, routeName: "commentReactionsEngagedUsers");
    }
  }
}
