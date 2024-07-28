// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.DevOps.Comments.WebApi.Controllers.CommentReactionsEngagedUsersBaseController
// Assembly: Microsoft.Azure.DevOps.Comments.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A6538262-E3F2-45F5-B799-587642D68EAC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DevOps.Comments.Web.dll

using Microsoft.TeamFoundation.Comments.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.VisualStudio.Services.Social.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;

namespace Microsoft.Azure.DevOps.Comments.WebApi.Controllers
{
  [ApiTelemetry(true, false)]
  public abstract class CommentReactionsEngagedUsersBaseController : TfsProjectApiController
  {
    protected virtual ICommentReactionService CommentReactionService => this.TfsRequestContext.GetService<ICommentReactionService>();

    protected virtual IEnumerable<IdentityRef> GetEngagedUsers(
      Guid artifactKind,
      string artifactId,
      int commentId,
      CommentReactionType reactionType,
      int? top = 20,
      int? skip = 0)
    {
      Microsoft.Azure.DevOps.Comments.Web.Controllers.ValidationHelper.ValidateCommentId(commentId);
      Microsoft.Azure.DevOps.Comments.Web.Controllers.ValidationHelper.ValidateTop(top);
      return this.CommentReactionService.GetEngagedUsers(this.TfsRequestContext, this.ProjectId, artifactKind, artifactId, commentId, (SocialEngagementType) reactionType, top.Value, skip.Value);
    }
  }
}
