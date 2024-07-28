// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Comments.Server.ICommentReactionService
// Assembly: Microsoft.TeamFoundation.Comments.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CBA40CC5-9694-4582-97B5-1660FA9D4307
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Comments.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Social.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Comments.Server
{
  [DefaultServiceImplementation(typeof (CommentReactionService))]
  public interface ICommentReactionService : IVssFrameworkService
  {
    CommentReaction CreateCommentReaction(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid artifactKind,
      string artifactId,
      int commentId,
      SocialEngagementType reactionType);

    CommentReaction DeleteCommentReaction(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid artifactKind,
      string artifactId,
      int commentId,
      SocialEngagementType reactionType);

    IList<CommentReaction> GetCommentReactions(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid artifactKind,
      string artifactId,
      ISet<int> commentIds);

    IEnumerable<IdentityRef> GetEngagedUsers(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid artifactKind,
      string artifactId,
      int commentId,
      SocialEngagementType reactionType,
      int top = 20,
      int skip = 0);
  }
}
