// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Web.Controllers.Comments.WorkItemCommentReactionsEngagedUsersController
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CCDEBCB9-DB6B-41A2-8797-78C2455AE321
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Web.dll

using Microsoft.Azure.DevOps.Comments.WebApi.Controllers;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.Comments;
using Microsoft.VisualStudio.Services.WebApi;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Microsoft.TeamFoundation.WorkItemTracking.Web.Controllers.Comments
{
  [VersionedApiControllerCustomName(Area = "wit", ResourceName = "commentReactionsEngagedUsers", ResourceVersion = 1)]
  [ControllerApiVersion(5.1)]
  [FeatureEnabled("WorkItemTracking.Server.CommentService.EnableCommentReactions")]
  public class WorkItemCommentReactionsEngagedUsersController : 
    CommentReactionsEngagedUsersBaseController
  {
    public override string TraceArea => "commentsReactions";

    protected virtual IWorkItemCommentReactionService WorkItemCommentReactionService => this.TfsRequestContext.GetService<IWorkItemCommentReactionService>();

    [HttpGet]
    public IEnumerable<IdentityRef> GetEngagedUsers(
      int workItemId,
      int commentId,
      Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.CommentReactionType reactionType,
      [FromUri(Name = "$top")] int top = 100,
      [FromUri(Name = "$skip")] int skip = 0)
    {
      ValidationHelper.ValidateWorkItemId(workItemId);
      IEnumerable<IdentityRef> engagedUsers = this.GetEngagedUsers(WorkItemArtifactKinds.WorkItem, workItemId.ToString(), commentId, (Microsoft.Azure.DevOps.Comments.WebApi.CommentReactionType) reactionType, new int?(top), new int?(skip));
      return engagedUsers == null ? (IEnumerable<IdentityRef>) null : (IEnumerable<IdentityRef>) engagedUsers.ToList<IdentityRef>();
    }
  }
}
