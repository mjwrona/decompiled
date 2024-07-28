// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.CodeReview.Discussion.Server.ITeamFoundationDiscussionService
// Assembly: Microsoft.VisualStudio.Services.CodeReview.Discussion.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FFC5EC6C-1B94-4299-8BA9-787264C21330
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.CodeReview.Discussion.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.CodeReview.Discussion.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.CodeReview.Discussion.Server
{
  [DefaultServiceImplementation(typeof (TeamFoundationDiscussionService))]
  public interface ITeamFoundationDiscussionService : IVssFrameworkService
  {
    List<int> PublishDiscussions(
      IVssRequestContext requestContext,
      DiscussionThread[] discussions,
      DiscussionComment[] comments,
      CommentId[] deletedComments,
      out List<short> commentIds,
      out DateTime lastUpdatedDate);

    List<DiscussionThread> PublishDiscussions(
      IVssRequestContext requestContext,
      DiscussionThread[] discussionsToSave,
      DiscussionComment[] commentsToSave,
      CommentId[] deletedComments);

    Dictionary<string, IEnumerable<DiscussionThread>> QueryDiscussionsByArtifactUris(
      IVssRequestContext requestContext,
      string[] artifactUris);

    List<DiscussionThread> QueryDiscussionsByArtifactUri(
      IVssRequestContext requestContext,
      string artifactUri,
      out List<DiscussionComment> comments,
      out IdentityRef[] authors);

    List<DiscussionThread> QueryDiscussionsByArtifactUri(
      IVssRequestContext requestContext,
      string artifactUri,
      out List<DiscussionComment> comments,
      DateTime? modifiedSince = null,
      bool includeExtendedProperties = true);

    List<DiscussionThread> QueryDiscussionsByCodeReviewRequest(
      IVssRequestContext requestContext,
      int workItemId,
      out List<DiscussionComment> comments,
      out IdentityRef[] authors);

    List<DiscussionThread> QueryDiscussionsByCodeReviewRequest(
      IVssRequestContext requestContext,
      int workItemId,
      out List<DiscussionComment> comments);

    DiscussionThread QueryDiscussionsById(
      IVssRequestContext requestContext,
      int discussionId,
      out List<DiscussionComment> comments,
      bool includeExtendedProperties = true);

    List<DiscussionThread> GetDiscussionThreadsChangedSinceLastWatermark(
      IVssRequestContext requestContext,
      DateTime lastUpdatedDate,
      int discussionId,
      int batchSize);

    List<DiscussionComment> GetDiscussionCommentsChangedSinceLastWatermark(
      IVssRequestContext requestContext,
      DateTime lastUpdatedDate,
      int discussionId,
      int commentId,
      int batchSize);

    DiscussionComment UpdateDiscussionComment(
      IVssRequestContext requestContext,
      DiscussionThread thread,
      DiscussionComment newComment);

    IEnumerable<IDiscussionArtifactPlugin> ArtifactPlugins { get; }

    bool CleanupDiscussions(
      IVssRequestContext requestContext,
      List<int> destroyedWorkItems,
      long highWaterMark,
      long newHighWaterMark);

    long QueryReplicationState(IVssRequestContext requestContext);

    List<IdentityRef> LikeComment(
      IVssRequestContext requestContext,
      DiscussionThread thread,
      short commentId,
      List<IdentityRef> authors = null);

    List<IdentityRef> QueryLikes(
      IVssRequestContext requestContext,
      DiscussionThread thread,
      short commentId);

    void WithdrawLikeComment(
      IVssRequestContext requestContext,
      DiscussionThread thread,
      short commentId,
      IdentityRef author);
  }
}
