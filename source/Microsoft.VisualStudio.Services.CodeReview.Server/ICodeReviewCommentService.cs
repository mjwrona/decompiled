// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.CodeReview.Server.ICodeReviewCommentService
// Assembly: Microsoft.VisualStudio.Services.CodeReview.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2BCB2866-BDCB-4FDE-9EA3-48DFA660C131
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.CodeReview.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.CodeReview.Discussion.WebApi;
using Microsoft.VisualStudio.Services.CodeReview.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.CodeReview.Server
{
  [DefaultServiceImplementation(typeof (CodeReviewCommentService))]
  public interface ICodeReviewCommentService : IVssFrameworkService
  {
    IEnumerable<CommentThread> SaveCommentThreads(
      IVssRequestContext requestContext,
      Guid projectId,
      int reviewId,
      IEnumerable<CommentThread> reviewCommentThreads,
      bool addReferenceLinks = true);

    CommentThread GetCommentThread(
      IVssRequestContext requestContext,
      Guid projectId,
      int reviewId,
      int threadId,
      CommentTrackingCriteria trackingCriteria = null,
      bool addReferenceLinks = true,
      bool includeExtendedProperties = true);

    List<CommentThread> GetCommentThreads(
      IVssRequestContext requestContext,
      Guid projectId,
      int reviewId,
      DateTime? modifiedSince = null,
      CommentTrackingCriteria trackingCriteria = null,
      bool addReferenceLinks = true,
      bool includeExtendedProperties = true);

    DiscussionComment SaveComment(
      IVssRequestContext requestContext,
      Guid projectId,
      int reviewId,
      int threadId,
      DiscussionComment newComment,
      bool addReferenceLinks = true);

    IList<DiscussionComment> SaveComments(
      IVssRequestContext requestContext,
      Guid projectId,
      int reviewId,
      int threadId,
      IEnumerable<DiscussionComment> newComments,
      bool addReferenceLinks = true);

    DiscussionComment GetComment(
      IVssRequestContext requestContext,
      Guid projectId,
      int reviewId,
      int threadId,
      short commentId,
      bool addReferenceLinks = true);

    void DeleteComment(
      IVssRequestContext requestContext,
      Guid projectId,
      int reviewId,
      int threadId,
      short commentId);

    List<IdentityRef> LikeComment(
      IVssRequestContext requestContext,
      Guid projectId,
      int reviewId,
      int threadId,
      short commentId,
      List<IdentityRef> authors = null);

    List<IdentityRef> GetLikes(
      IVssRequestContext requestContext,
      Guid projectId,
      int reviewId,
      int threadId,
      short commentId);

    void WithdrawLikeComment(
      IVssRequestContext requestContext,
      Guid projectId,
      int reviewId,
      int threadId,
      short commentId,
      IdentityRef author);
  }
}
