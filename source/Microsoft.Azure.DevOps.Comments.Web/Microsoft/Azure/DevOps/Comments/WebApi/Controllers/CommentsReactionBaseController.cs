// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.DevOps.Comments.WebApi.Controllers.CommentsReactionBaseController
// Assembly: Microsoft.Azure.DevOps.Comments.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A6538262-E3F2-45F5-B799-587642D68EAC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DevOps.Comments.Web.dll

using Microsoft.Azure.DevOps.Comments.Web.Factories;
using Microsoft.TeamFoundation.Comments.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Social.WebApi;
using System;

namespace Microsoft.Azure.DevOps.Comments.WebApi.Controllers
{
  [ApiTelemetry(true, false)]
  public abstract class CommentsReactionBaseController : TfsProjectApiController
  {
    protected virtual ICommentReactionService CommentReactionService => this.TfsRequestContext.GetService<ICommentReactionService>();

    protected virtual Microsoft.Azure.DevOps.Comments.WebApi.CommentReaction CreateCommentReaction(
      Guid artifactKind,
      string artifactId,
      int commentId,
      CommentReactionType reactionType)
    {
      Microsoft.Azure.DevOps.Comments.Web.Controllers.ValidationHelper.ValidateCommentId(commentId);
      ArgumentUtility.CheckStringForNullOrEmpty(artifactId, nameof (artifactId));
      return CommentReactionFactory.CreateReaction(this.CommentReactionService.CreateCommentReaction(this.TfsRequestContext, this.ProjectId, artifactKind, artifactId, commentId, (SocialEngagementType) reactionType));
    }

    protected virtual Microsoft.Azure.DevOps.Comments.WebApi.CommentReaction DeleteCommentReaction(
      Guid artifactKind,
      string artifactId,
      int commentId,
      CommentReactionType reactionType)
    {
      Microsoft.Azure.DevOps.Comments.Web.Controllers.ValidationHelper.ValidateCommentId(commentId);
      return CommentReactionFactory.CreateReaction(this.CommentReactionService.DeleteCommentReaction(this.TfsRequestContext, this.ProjectId, artifactKind, artifactId, commentId, (SocialEngagementType) reactionType));
    }
  }
}
