// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.CodeReview.Server.CodeReviewCommentService
// Assembly: Microsoft.VisualStudio.Services.CodeReview.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2BCB2866-BDCB-4FDE-9EA3-48DFA660C131
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.CodeReview.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.BusinessIntelligence;
using Microsoft.VisualStudio.Services.CodeReview.Discussion.Server;
using Microsoft.VisualStudio.Services.CodeReview.Discussion.WebApi;
using Microsoft.VisualStudio.Services.CodeReview.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Microsoft.VisualStudio.Services.CodeReview.Server
{
  internal class CodeReviewCommentService : 
    CodeReviewServiceBase,
    ICodeReviewCommentService,
    IVssFrameworkService
  {
    public virtual IEnumerable<CommentThread> SaveCommentThreads(
      IVssRequestContext requestContext,
      Guid projectId,
      int reviewId,
      IEnumerable<CommentThread> reviewCommentThreadsToSave,
      bool addReferenceLinks = true)
    {
      List<CommentThread> reviewCommentThreads = new List<CommentThread>();
      this.ExecuteAndTrace(requestContext, 1380801, 1380802, 1380803, (Action) (() =>
      {
        this.ValidateInput(requestContext, projectId, reviewId);
        this.TraceCommentThreadsInfo(requestContext, projectId, reviewId, reviewCommentThreadsToSave, 1380804, "Saving comment thread");
        List<DiscussionComment> discussionCommentList = new List<DiscussionComment>();
        List<DiscussionThread> discussionThreadList = (List<DiscussionThread>) new DiscussionThreadCollection();
        bool commentPositionValidationEnabled = requestContext.IsFeatureEnabled("CodeReview.Thread.ContextCheckCommentLineOffsetPosition");
        foreach (CommentThread thread in reviewCommentThreadsToSave)
        {
          DiscussionThread discussionThread = this.GetValidatedDiscussionThread(projectId, reviewId, thread, commentPositionValidationEnabled);
          discussionThreadList.Add(discussionThread);
          if (thread.Comments != null)
          {
            foreach (DiscussionComment comment in discussionThread.Comments)
              discussionCommentList.Add(this.GetValidatedComment(discussionThread.DiscussionId, comment));
          }
        }
        reviewCommentThreads.AddRange(requestContext.GetService<ITeamFoundationDiscussionService>().PublishDiscussions(requestContext, discussionThreadList.ToArray(), discussionCommentList.ToArray(), (CommentId[]) null).Select<DiscussionThread, CommentThread>((Func<DiscussionThread, CommentThread>) (t => t.ToReviewCommentThread())));
        if (!addReferenceLinks)
          return;
        foreach (CommentThread thread in reviewCommentThreads)
          thread.AddReferenceLinks(requestContext, projectId, reviewId, thread.DiscussionId);
      }), nameof (SaveCommentThreads));
      return (IEnumerable<CommentThread>) reviewCommentThreads;
    }

    public CommentThread GetCommentThread(
      IVssRequestContext requestContext,
      Guid projectId,
      int reviewId,
      int threadId,
      CommentTrackingCriteria trackingCriteria = null,
      bool addReferenceLinks = true,
      bool includeExtendedProperties = true)
    {
      CommentThread reviewCommentThread = (CommentThread) null;
      this.ExecuteAndTrace(requestContext, 1380811, 1380812, 1380813, (Action) (() =>
      {
        this.ValidateInput(requestContext, projectId, reviewId, threadId);
        requestContext.Trace(1380814, TraceLevel.Verbose, this.Area, this.Layer, "Getting a comment thread: review id: '{0}', thread id: '{1}', project id: '{2}'", (object) reviewId, (object) threadId, (object) projectId);
        reviewCommentThread = (requestContext.GetService<ITeamFoundationDiscussionService>().QueryDiscussionsById(requestContext, threadId, out List<DiscussionComment> _, includeExtendedProperties) ?? throw new CommentThreadNotFoundException(threadId)).ToReviewCommentThread();
        if (!addReferenceLinks)
          return;
        reviewCommentThread.AddReferenceLinks(requestContext, projectId, reviewId, threadId);
      }), nameof (GetCommentThread));
      if (trackingCriteria != null && trackingCriteria.SecondComparingIteration > 0)
      {
        IVssRequestContext requestContext1 = requestContext;
        Guid projectId1 = projectId;
        int reviewId1 = reviewId;
        List<CommentThread> threadList = new List<CommentThread>();
        threadList.Add(reviewCommentThread);
        CommentTrackingCriteria trackingCriteria1 = trackingCriteria;
        this.TrackCommentThreads(requestContext1, projectId1, reviewId1, threadList, trackingCriteria1);
      }
      return reviewCommentThread;
    }

    public List<CommentThread> GetCommentThreads(
      IVssRequestContext requestContext,
      Guid projectId,
      int reviewId,
      DateTime? modifiedSince = null,
      CommentTrackingCriteria trackingCriteria = null,
      bool addReferenceLinks = true,
      bool includeExtendedProperties = true)
    {
      List<CommentThread> reviewCommentThreads = (List<CommentThread>) null;
      this.ExecuteAndTrace(requestContext, 1380821, 1380822, 1380823, (Action) (() =>
      {
        this.ValidateInput(requestContext, projectId, reviewId);
        requestContext.Trace(1380824, TraceLevel.Verbose, this.Area, this.Layer, "Getting comment threads: review id: '{0}', project id: '{1}'", (object) reviewId, (object) projectId);
        string artifactUri = CodeReviewSdkArtifactId.GetArtifactUri(projectId, reviewId);
        reviewCommentThreads = requestContext.GetService<ITeamFoundationDiscussionService>().QueryDiscussionsByArtifactUri(requestContext, artifactUri, out List<DiscussionComment> _, modifiedSince, includeExtendedProperties).Select<DiscussionThread, CommentThread>((Func<DiscussionThread, CommentThread>) (thread => thread.ToReviewCommentThread())).ToList<CommentThread>();
        if (!addReferenceLinks)
          return;
        foreach (CommentThread thread in reviewCommentThreads)
          thread.AddReferenceLinks(requestContext, projectId, reviewId, thread.DiscussionId);
      }), nameof (GetCommentThreads));
      if (trackingCriteria != null && trackingCriteria.SecondComparingIteration > 0)
        this.TrackCommentThreads(requestContext, projectId, reviewId, reviewCommentThreads, trackingCriteria);
      return reviewCommentThreads;
    }

    public DiscussionComment SaveComment(
      IVssRequestContext requestContext,
      Guid projectId,
      int reviewId,
      int threadId,
      DiscussionComment newComment,
      bool addReferenceLinks = true)
    {
      DiscussionComment savedComment = (DiscussionComment) null;
      this.ExecuteAndTrace(requestContext, 1380831, 1380832, 1380833, (Action) (() =>
      {
        ArgumentUtility.CheckForNull<DiscussionComment>(newComment, nameof (newComment));
        requestContext.Trace(1380834, TraceLevel.Verbose, this.Area, this.Layer, "Saving a comment: review id: '{0}', thread id: '{1}', comment id: '{2}', project id: '{3}'", (object) reviewId, (object) threadId, (object) newComment.CommentId, (object) projectId);
        savedComment = this.SaveCommentsInternal(requestContext, projectId, reviewId, threadId, (IEnumerable<DiscussionComment>) new List<DiscussionComment>()
        {
          newComment
        }).FirstOrDefault<DiscussionComment>();
        if (!addReferenceLinks)
          return;
        savedComment.AddReferenceLinks(requestContext, projectId, reviewId, threadId, savedComment.CommentId);
      }), nameof (SaveComment));
      return savedComment;
    }

    public IList<DiscussionComment> SaveComments(
      IVssRequestContext requestContext,
      Guid projectId,
      int reviewId,
      int threadId,
      IEnumerable<DiscussionComment> newComments,
      bool addReferenceLinks = true)
    {
      IList<DiscussionComment> savedComments = (IList<DiscussionComment>) null;
      this.ExecuteAndTrace(requestContext, 1380841, 1380842, 1380843, (Action) (() =>
      {
        ArgumentUtility.CheckForNull<IEnumerable<DiscussionComment>>(newComments, nameof (newComments));
        this.TraceCommentsInfo(requestContext, projectId, reviewId, threadId, newComments, 1380844, "Saving comment");
        savedComments = this.SaveCommentsInternal(requestContext, projectId, reviewId, threadId, newComments);
        if (!addReferenceLinks)
          return;
        foreach (DiscussionComment comment in (IEnumerable<DiscussionComment>) savedComments)
          comment.AddReferenceLinks(requestContext, projectId, reviewId, threadId, comment.CommentId);
      }), nameof (SaveComments));
      return savedComments;
    }

    public DiscussionComment GetComment(
      IVssRequestContext requestContext,
      Guid projectId,
      int reviewId,
      int threadId,
      short commentId,
      bool addReferenceLinks = true)
    {
      DiscussionComment comment = (DiscussionComment) null;
      this.ExecuteAndTrace(requestContext, 1380851, 1380852, 1380853, (Action) (() =>
      {
        this.ValidateInput(requestContext, projectId, reviewId, threadId, commentId);
        requestContext.Trace(1380854, TraceLevel.Verbose, this.Area, this.Layer, "Get a comment: review id: '{0}', thread id: '{1}', comment id: '{2}', project id: '{3}'", (object) reviewId, (object) threadId, (object) commentId, (object) projectId);
        List<DiscussionComment> comments;
        if (requestContext.GetService<ITeamFoundationDiscussionService>().QueryDiscussionsById(requestContext, threadId, out comments) == null)
          throw new CommentThreadNotFoundException(threadId);
        comment = comments.Single<DiscussionComment>((Func<DiscussionComment, bool>) (x => (int) x.CommentId == (int) commentId));
        if (!addReferenceLinks)
          return;
        comment.AddReferenceLinks(requestContext, projectId, reviewId, threadId, comment.CommentId);
      }), nameof (GetComment));
      return comment;
    }

    public void DeleteComment(
      IVssRequestContext requestContext,
      Guid projectId,
      int reviewId,
      int threadId,
      short commentId)
    {
      this.ExecuteAndTrace(requestContext, 1380861, 1380862, 1380863, (Action) (() =>
      {
        this.ValidateInput(requestContext, projectId, reviewId, threadId, commentId);
        requestContext.Trace(1380864, TraceLevel.Verbose, this.Area, this.Layer, "Delete a comment: review id: '{0}', thread id: '{1}', comment id: '{2}', project id: '{3}'", (object) reviewId, (object) threadId, (object) commentId, (object) projectId);
        ITeamFoundationDiscussionService service = requestContext.GetService<ITeamFoundationDiscussionService>();
        List<DiscussionComment> comments;
        DiscussionThread discussionThread = service.QueryDiscussionsById(requestContext, threadId, out comments);
        if (discussionThread == null)
          throw new CommentThreadNotFoundException(threadId);
        DiscussionComment discussionComment = comments != null ? comments.Where<DiscussionComment>((Func<DiscussionComment, bool>) (c => (int) c.CommentId == (int) commentId)).FirstOrDefault<DiscussionComment>() : (DiscussionComment) null;
        if (discussionComment != null && !IdentityHelper.CompareRequesterIdentity(requestContext, discussionComment.Author.Id) && !this.IsUserProjectAdmin(requestContext, projectId))
          throw new ArgumentException(CodeReviewResources.CommentDeleteInvalidAuthor());
        CommentId commentId1 = new CommentId()
        {
          Id = commentId,
          DiscussionId = threadId
        };
        service.PublishDiscussions(requestContext, new DiscussionThread[1]
        {
          discussionThread
        }, (DiscussionComment[]) null, new CommentId[1]
        {
          commentId1
        }, out List<short> _, out DateTime _);
      }), nameof (DeleteComment));
    }

    public List<IdentityRef> LikeComment(
      IVssRequestContext requestContext,
      Guid projectId,
      int reviewId,
      int threadId,
      short commentId,
      List<IdentityRef> authors = null)
    {
      List<IdentityRef> likedUser = (List<IdentityRef>) null;
      this.ExecuteAndTrace(requestContext, 1380871, 1380872, 1380873, (Action) (() =>
      {
        this.ValidateInput(requestContext, projectId, reviewId, threadId);
        requestContext.Trace(1380874, TraceLevel.Verbose, this.Area, this.Layer, "Like a comment: review id: '{0}', thread id: '{1}', comment id: '{2}', project id: '{3}'", (object) reviewId, (object) threadId, (object) commentId, (object) projectId);
        ITeamFoundationDiscussionService service = requestContext.GetService<ITeamFoundationDiscussionService>();
        CommentThread thread = new CommentThread()
        {
          DiscussionId = threadId,
          ArtifactUri = CodeReviewSdkArtifactId.GetArtifactUri(projectId, reviewId)
        };
        likedUser = service.LikeComment(requestContext, (DiscussionThread) thread, commentId, authors);
      }), nameof (LikeComment));
      return likedUser;
    }

    public List<IdentityRef> GetLikes(
      IVssRequestContext requestContext,
      Guid projectId,
      int reviewId,
      int threadId,
      short commentId)
    {
      List<IdentityRef> likedUsers = (List<IdentityRef>) null;
      this.ExecuteAndTrace(requestContext, 1380881, 1380882, 1380883, (Action) (() =>
      {
        this.ValidateInput(requestContext, projectId, reviewId, threadId);
        requestContext.Trace(1380884, TraceLevel.Verbose, this.Area, this.Layer, "Getting likes: review id: '{0}', thread id: '{1}', comment id: '{2}', project id: '{3}'", (object) reviewId, (object) threadId, (object) commentId, (object) projectId);
        ITeamFoundationDiscussionService service = requestContext.GetService<ITeamFoundationDiscussionService>();
        CommentThread thread = new CommentThread()
        {
          DiscussionId = threadId,
          ArtifactUri = CodeReviewSdkArtifactId.GetArtifactUri(projectId, reviewId)
        };
        likedUsers = service.QueryLikes(requestContext, (DiscussionThread) thread, commentId);
      }), nameof (GetLikes));
      return likedUsers;
    }

    public void WithdrawLikeComment(
      IVssRequestContext requestContext,
      Guid projectId,
      int reviewId,
      int threadId,
      short commentId,
      IdentityRef author)
    {
      this.ExecuteAndTrace(requestContext, 1380891, 1380892, 1380893, (Action) (() =>
      {
        this.ValidateInput(requestContext, projectId, reviewId, threadId);
        requestContext.Trace(1380894, TraceLevel.Verbose, this.Area, this.Layer, "Withdrawing a like on comment: review id: '{0}', thread id: '{1}', comment id: '{2}', project id: '{3}', author: '{4}'", (object) reviewId, (object) threadId, (object) commentId, (object) projectId, (object) author.Id);
        ITeamFoundationDiscussionService service = requestContext.GetService<ITeamFoundationDiscussionService>();
        CommentThread commentThread = new CommentThread()
        {
          DiscussionId = threadId,
          ArtifactUri = CodeReviewSdkArtifactId.GetArtifactUri(projectId, reviewId)
        };
        IVssRequestContext requestContext1 = requestContext;
        CommentThread thread = commentThread;
        int commentId1 = (int) commentId;
        IdentityRef author1 = author;
        service.WithdrawLikeComment(requestContext1, (DiscussionThread) thread, (short) commentId1, author1);
      }), nameof (WithdrawLikeComment));
    }

    private IList<DiscussionComment> SaveCommentsInternal(
      IVssRequestContext requestContext,
      Guid projectId,
      int reviewId,
      int threadId,
      IEnumerable<DiscussionComment> commentsToSave)
    {
      this.ValidateInput(requestContext, projectId, reviewId, threadId);
      ITeamFoundationDiscussionService service = requestContext.GetService<ITeamFoundationDiscussionService>();
      DiscussionThread thread = service.QueryDiscussionsById(requestContext, threadId, out List<DiscussionComment> _);
      if (thread == null)
        throw new CommentThreadNotFoundException(threadId);
      int valueOrDefault = thread?.Comments?.Length.GetValueOrDefault();
      int num = commentsToSave != null ? commentsToSave.Count<DiscussionComment>() : 0;
      if (valueOrDefault + num > 500)
        throw new CodeReviewMaxCommentCountException(500);
      requestContext.GetService<ClientTraceService>().Publish(requestContext, this.Area, this.Layer, new ClientTraceData((IDictionary<string, object>) new Dictionary<string, object>()
      {
        {
          "SaveCommentReviewId",
          (object) reviewId
        },
        {
          "SaveCommentProjectId",
          (object) projectId
        },
        {
          "SaveCommentThreadId",
          (object) threadId
        },
        {
          "SaveCommentCurrentCount",
          (object) valueOrDefault
        },
        {
          "SaveCommentAdditionalCount",
          (object) num
        }
      }));
      List<DiscussionComment> discussionCommentList1 = new List<DiscussionComment>();
      List<DiscussionComment> discussionCommentList2 = new List<DiscussionComment>();
      List<DiscussionComment> discussionCommentList3 = new List<DiscussionComment>();
      foreach (DiscussionComment comment in commentsToSave)
      {
        DiscussionComment validatedComment = this.GetValidatedComment(threadId, comment);
        if (comment.CommentId == (short) 0)
          discussionCommentList1.Add(validatedComment);
        else
          discussionCommentList2.Add(validatedComment);
      }
      if (discussionCommentList1.Count > 0)
      {
        List<short> commentIds;
        DateTime lastUpdatedDate;
        service.PublishDiscussions(requestContext, new DiscussionThread[1]
        {
          thread
        }, discussionCommentList1.ToArray(), (CommentId[]) null, out commentIds, out lastUpdatedDate);
        for (int index = 0; index < commentIds.Count; ++index)
          discussionCommentList3.Add(this.GetUpdatedComment(discussionCommentList1[index], commentIds[index], lastUpdatedDate));
      }
      if (discussionCommentList2.Count > 0)
      {
        foreach (DiscussionComment newComment in discussionCommentList2)
        {
          DiscussionComment discussionComment = service.UpdateDiscussionComment(requestContext, thread, newComment);
          discussionCommentList3.Add(discussionComment);
        }
      }
      return (IList<DiscussionComment>) discussionCommentList3;
    }

    private void TrackCommentThreads(
      IVssRequestContext requestContext,
      Guid projectId,
      int reviewId,
      List<CommentThread> threadList,
      CommentTrackingCriteria trackingCriteria)
    {
      this.ExecuteAndTrace(requestContext, 1380121, 1380122, 1380123, (Action) (() =>
      {
        ArgumentUtility.CheckForNull<CommentTrackingCriteria>(trackingCriteria, nameof (trackingCriteria));
        ArgumentUtility.CheckForOutOfRange(trackingCriteria.FirstComparingIteration, "firstComparingIteration", 0);
        ArgumentUtility.CheckForOutOfRange(trackingCriteria.SecondComparingIteration, "secondComparingIteration", 1);
        this.ValidateInput(requestContext, projectId, reviewId);
        this.TraceTrackCommentThreads(requestContext, projectId, reviewId, threadList, trackingCriteria, 1380124, "Tracking comments:");
        CommentTrackingUtility.TrackCommentThreads(requestContext, projectId, reviewId, threadList, trackingCriteria);
      }), nameof (TrackCommentThreads));
    }

    private DiscussionComment GetUpdatedComment(
      DiscussionComment commentToUpdate,
      short commentId,
      DateTime lastUpdatedDate)
    {
      commentToUpdate.CommentId = commentId;
      commentToUpdate.PublishedDate = lastUpdatedDate;
      commentToUpdate.LastUpdatedDate = lastUpdatedDate;
      commentToUpdate.LastContentUpdatedDate = lastUpdatedDate;
      commentToUpdate.CanDelete = true;
      return commentToUpdate;
    }

    private void ValidateInput(IVssRequestContext requestContext, Guid projectId, int reviewId)
    {
      DataspaceHelper.EnsureDataspaceExists(requestContext, projectId);
      ArgumentUtility.CheckForOutOfRange(reviewId, nameof (reviewId), 1);
    }

    private void ValidateInput(
      IVssRequestContext requestContext,
      Guid projectId,
      int reviewId,
      int threadId)
    {
      ArgumentUtility.CheckForOutOfRange(threadId, nameof (threadId), 1);
      this.ValidateInput(requestContext, projectId, reviewId);
    }

    private void ValidateInput(
      IVssRequestContext requestContext,
      Guid projectId,
      int reviewId,
      int threadId,
      short commentId)
    {
      this.ValidateInput(requestContext, projectId, reviewId, threadId);
      ArgumentUtility.CheckForOutOfRange((int) commentId, nameof (commentId), 1);
    }

    private DiscussionThread GetValidatedDiscussionThread(
      Guid projectId,
      int reviewId,
      CommentThread thread,
      bool commentPositionValidationEnabled)
    {
      CommentThreadContext threadContext = thread.ThreadContext;
      if (thread.DiscussionId < 1 && !thread.IsReviewLevel)
      {
        if (string.IsNullOrEmpty(thread.ThreadContext.FilePath))
          throw new ArgumentNullException("FilePath", CodeReviewResources.CommentFilePathCannotBeNullWhenIsReviewLevelIsFalse());
        if (threadContext.IterationContext != null)
        {
          ArgumentUtility.CheckForOutOfRange((int) threadContext.IterationContext.FirstComparingIteration, "FirstComparingIteration", 1);
          ArgumentUtility.CheckForOutOfRange((int) threadContext.IterationContext.SecondComparingIteration, "SecondComparingIteration", 1);
        }
      }
      if (thread.DiscussionId >= 1)
      {
        if (thread.Properties != null)
          throw new ArgumentException(CodeReviewResources.CommentThreadPropertiesCannotBeUpdated(), "Properties");
        if (thread.ThreadContext != null)
          throw new ArgumentException(CodeReviewResources.CommentThreadContextCannotBeUpdated(), "ThreadContext");
        thread.IsDirty = true;
      }
      if (commentPositionValidationEnabled && threadContext != null)
      {
        this.ValidateCommentPosition(threadContext.LeftFileStart, "LeftFileStart");
        this.ValidateCommentPosition(threadContext.LeftFileEnd, "LeftFileEnd");
        this.ValidateCommentPosition(threadContext.RightFileStart, "RightFileStart");
        this.ValidateCommentPosition(threadContext.RightFileEnd, "RightFileEnd");
      }
      thread.ProjectId = projectId;
      thread.ReviewId = reviewId;
      return thread.ToDiscussionThread();
    }

    private void ValidateCommentPosition(Microsoft.VisualStudio.Services.CodeReview.WebApi.Position position, string positionName)
    {
      if (position == null)
        return;
      ArgumentUtility.CheckForOutOfRange(position.Line, positionName + ".Line", 1);
      ArgumentUtility.CheckForOutOfRange(position.Offset + 1, positionName + ".Offset", 1);
    }

    private DiscussionComment GetValidatedComment(int threadId, DiscussionComment comment)
    {
      ArgumentUtility.CheckForNull<DiscussionComment>(comment, nameof (comment));
      if (comment.DiscussionId != threadId)
        comment.DiscussionId = comment.DiscussionId < 1 ? threadId : throw new ArgumentException(CodeReviewResources.InvalidCommentDiscussionId((object) comment.DiscussionId.ToString()), "DiscussionId");
      if (comment.CommentId < (short) 1)
      {
        if (string.IsNullOrEmpty(comment.Content))
          throw new ArgumentNullException("content", CodeReviewResources.CannotAddCommentWithoutContent());
      }
      else if (comment.Author != null)
        throw new ArgumentException(CodeReviewResources.CommentAuthorCannotBeUpdated(), "Author");
      return comment;
    }

    private void TraceCommentThreadsInfo(
      IVssRequestContext requestContext,
      Guid projectId,
      int reviewId,
      IEnumerable<CommentThread> commentThreads,
      int tracePoint,
      string description)
    {
      if (!requestContext.IsTracing(tracePoint, TraceLevel.Verbose, this.Area, this.Layer))
        return;
      StringBuilder stringBuilder = new StringBuilder();
      if (commentThreads == null)
        return;
      foreach (CommentThread commentThread in commentThreads)
        stringBuilder.Append(string.Format("'{0}', ", (object) commentThread.DiscussionId));
      requestContext.Trace(tracePoint, TraceLevel.Verbose, this.Area, this.Layer, "{0}: review id: '{1}', project id: '{2}', comment thread ids: {3}", (object) description, (object) reviewId, (object) projectId, (object) stringBuilder.ToString());
    }

    private void TraceCommentsInfo(
      IVssRequestContext requestContext,
      Guid projectId,
      int reviewId,
      int threadId,
      IEnumerable<DiscussionComment> comments,
      int tracePoint,
      string description)
    {
      if (!requestContext.IsTracing(tracePoint, TraceLevel.Verbose, this.Area, this.Layer))
        return;
      StringBuilder stringBuilder = new StringBuilder();
      if (comments == null)
        return;
      foreach (DiscussionComment comment in comments)
        stringBuilder.Append(string.Format("'{0}', ", (object) comment.CommentId));
      requestContext.Trace(tracePoint, TraceLevel.Verbose, this.Area, this.Layer, "{0}: review id: '{1}', thread id: '{2}', project id: '{3}', comment ids: {4}", (object) description, (object) reviewId, (object) threadId, (object) projectId, (object) stringBuilder.ToString());
    }

    private void TraceTrackCommentThreads(
      IVssRequestContext requestContext,
      Guid projectId,
      int reviewId,
      List<CommentThread> threadList,
      CommentTrackingCriteria trackingCriteria,
      int tracePoint,
      string description)
    {
      if (!requestContext.IsTracing(tracePoint, TraceLevel.Verbose, this.Area, this.Layer) || threadList == null)
        return;
      StringBuilder threadData = new StringBuilder();
      threadList.Where<CommentThread>((Func<CommentThread, bool>) (t => !t.IsReviewLevel && !t.IsDeleted && t.ThreadContext.IterationContext != null && t.ThreadContext.ChangeTrackingId != 0)).Select<CommentThread, CommentThreadContext>((Func<CommentThread, CommentThreadContext>) (t => t.ThreadContext)).ToList<CommentThreadContext>().ForEach((Action<CommentThreadContext>) (threadContext => threadData.Append(string.Format("({0}: {1}->{2}), ", (object) threadContext.ChangeTrackingId, (object) threadContext.IterationContext.FirstComparingIteration, (object) threadContext.IterationContext.SecondComparingIteration))));
      requestContext.Trace(tracePoint, TraceLevel.Verbose, this.Area, this.Layer, "{0}: review id: '{1}', project id: '{2}', num threads: '{3}', iters: '{4}+{5}', thread data: {6}", (object) description, (object) reviewId, (object) projectId, (object) threadList.Count, (object) trackingCriteria.FirstComparingIteration, (object) trackingCriteria.SecondComparingIteration, (object) threadData.ToString());
    }
  }
}
