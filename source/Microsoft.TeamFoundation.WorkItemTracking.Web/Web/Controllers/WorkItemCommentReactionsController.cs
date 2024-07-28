// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Web.Controllers.WorkItemCommentReactionsController
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CCDEBCB9-DB6B-41A2-8797-78C2455AE321
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Web.dll

using Microsoft.Azure.DevOps.Comments.WebApi.Controllers;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.Comments;
using Microsoft.TeamFoundation.WorkItemTracking.Web.Models.Factories;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Microsoft.TeamFoundation.WorkItemTracking.Web.Controllers
{
  [VersionedApiControllerCustomName(Area = "wit", ResourceName = "commentsReactions", ResourceVersion = 1)]
  [ControllerApiVersion(5.1)]
  [FeatureEnabled("WorkItemTracking.Server.CommentService.EnableCommentReactions")]
  public class WorkItemCommentReactionsController : CommentsReactionBaseController
  {
    public override string TraceArea => "commentsReactions";

    protected virtual IWorkItemCommentReactionService WorkItemCommentReactionService => this.TfsRequestContext.GetService<IWorkItemCommentReactionService>();

    protected virtual WorkItemCommentService WorkItemCommentService => this.TfsRequestContext.GetService<WorkItemCommentService>();

    [HttpGet]
    public IList<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.CommentReaction> GetCommentReactions(
      int workItemId,
      int commentId)
    {
      Microsoft.TeamFoundation.WorkItemTracking.Web.Controllers.Comments.ValidationHelper.ValidateWorkItemId(workItemId);
      Microsoft.TeamFoundation.WorkItemTracking.Web.Controllers.Comments.ValidationHelper.ValidateCommentId(commentId);
      return (IList<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.CommentReaction>) WorkItemCommentFactory.CreateCommentReactions(this.WorkItemCommentService.GetComment(this.TfsRequestContext, this.ProjectId, workItemId, commentId, WorkItemCommentExpandOptions.Reactions)).ToList<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.CommentReaction>();
    }

    [HttpPut]
    public Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.CommentReaction CreateCommentReaction(
      [FromUri] int workItemId,
      [FromUri] int commentId,
      [FromUri(Name = "reactionType")] Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.CommentReactionType reactionType)
    {
      Microsoft.TeamFoundation.WorkItemTracking.Web.Controllers.Comments.ValidationHelper.ValidateWorkItemId(workItemId);
      Microsoft.TeamFoundation.WorkItemTracking.Web.Controllers.Comments.ValidationHelper.ValidateCommentId(commentId);
      return WorkItemCommentFactory.CreateCommentReaction(this.CreateCommentReaction(WorkItemArtifactKinds.WorkItem, workItemId.ToString(), commentId, (Microsoft.Azure.DevOps.Comments.WebApi.CommentReactionType) reactionType));
    }

    [HttpDelete]
    public Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.CommentReaction DeleteCommentReaction(
      [FromUri] int workItemId,
      [FromUri] int commentId,
      [FromUri(Name = "reactionType")] Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.CommentReactionType reactionType)
    {
      Microsoft.TeamFoundation.WorkItemTracking.Web.Controllers.Comments.ValidationHelper.ValidateWorkItemId(workItemId);
      Microsoft.TeamFoundation.WorkItemTracking.Web.Controllers.Comments.ValidationHelper.ValidateCommentId(commentId);
      return WorkItemCommentFactory.CreateCommentReaction(this.DeleteCommentReaction(WorkItemArtifactKinds.WorkItem, workItemId.ToString(), commentId, (Microsoft.Azure.DevOps.Comments.WebApi.CommentReactionType) reactionType));
    }
  }
}
