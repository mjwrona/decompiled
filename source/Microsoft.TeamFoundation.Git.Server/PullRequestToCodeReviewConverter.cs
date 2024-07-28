// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.PullRequestToCodeReviewConverter
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.CodeReview.WebApi;

namespace Microsoft.TeamFoundation.Git.Server
{
  public static class PullRequestToCodeReviewConverter
  {
    public static Microsoft.VisualStudio.Services.CodeReview.WebApi.CommentThread ToCodeReviewItem(
      this GitPullRequestCommentThread prCommentThread,
      IVssRequestContext rc)
    {
      if (prCommentThread == null)
        return (Microsoft.VisualStudio.Services.CodeReview.WebApi.CommentThread) null;
      Microsoft.VisualStudio.Services.CodeReview.WebApi.CommentThread codeReviewItem = new Microsoft.VisualStudio.Services.CodeReview.WebApi.CommentThread();
      codeReviewItem.DiscussionId = prCommentThread.Id;
      codeReviewItem.ArtifactUri = prCommentThread.ArtifactUri;
      codeReviewItem.LastUpdatedDate = prCommentThread.LastUpdatedDate;
      codeReviewItem.PublishedDate = prCommentThread.PublishedDate;
      codeReviewItem.Properties = ThreadPropertiesConverter.GetCRThreadProperties(rc, prCommentThread);
      codeReviewItem.Status = prCommentThread.Status.ToDiscussionCommentStatus();
      codeReviewItem.Comments = prCommentThread.Comments.ToDiscussionCommentList();
      codeReviewItem.ThreadContext = prCommentThread.ToCRThreadContext();
      codeReviewItem.IsDirty = prCommentThread.MarkForUpdate;
      return codeReviewItem;
    }

    public static Microsoft.VisualStudio.Services.CodeReview.WebApi.CommentThreadContext ToCRThreadContext(
      this GitPullRequestCommentThread prThread)
    {
      if (prThread == null || prThread.ThreadContext == null || prThread.PullRequestThreadContext == null)
        return (Microsoft.VisualStudio.Services.CodeReview.WebApi.CommentThreadContext) null;
      return new Microsoft.VisualStudio.Services.CodeReview.WebApi.CommentThreadContext()
      {
        LeftFileStart = prThread.ThreadContext.LeftFileStart.ToCRCommentPosition(),
        RightFileStart = prThread.ThreadContext.RightFileStart.ToCRCommentPosition(),
        LeftFileEnd = prThread.ThreadContext.LeftFileEnd.ToCRCommentPosition(),
        RightFileEnd = prThread.ThreadContext.RightFileEnd.ToCRCommentPosition(),
        ChangeTrackingId = prThread.PullRequestThreadContext.ChangeTrackingId,
        FilePath = prThread.ThreadContext.FilePath,
        IterationContext = prThread.PullRequestThreadContext.IterationContext.ToCRIterationContext(),
        TrackingCriteria = prThread.PullRequestThreadContext.TrackingCriteria.ToCRCommentTrackingCriteria()
      };
    }

    public static IterationContext ToCRIterationContext(
      this CommentIterationContext prIterationContext)
    {
      if (prIterationContext == null)
        return (IterationContext) null;
      return new IterationContext()
      {
        FirstComparingIteration = prIterationContext.FirstComparingIteration,
        SecondComparingIteration = prIterationContext.SecondComparingIteration
      };
    }

    public static Microsoft.VisualStudio.Services.CodeReview.WebApi.CommentTrackingCriteria ToCRCommentTrackingCriteria(
      this Microsoft.TeamFoundation.SourceControl.WebApi.CommentTrackingCriteria prCommentTrackingCriteria)
    {
      if (prCommentTrackingCriteria == null)
        return (Microsoft.VisualStudio.Services.CodeReview.WebApi.CommentTrackingCriteria) null;
      return new Microsoft.VisualStudio.Services.CodeReview.WebApi.CommentTrackingCriteria()
      {
        FirstComparingIteration = prCommentTrackingCriteria.FirstComparingIteration,
        SecondComparingIteration = prCommentTrackingCriteria.SecondComparingIteration
      };
    }

    public static Status ToCodeReviewItem(this GitPullRequestStatus prStatus)
    {
      if (prStatus == null)
        return (Status) null;
      StatusContext statusContext1;
      if (prStatus.Context == null)
      {
        statusContext1 = (StatusContext) null;
      }
      else
      {
        statusContext1 = new StatusContext();
        statusContext1.Name = prStatus.Context.Name;
        statusContext1.Genre = prStatus.Context.Genre;
      }
      StatusContext statusContext2 = statusContext1;
      return new Status()
      {
        Id = prStatus.Id,
        State = prStatus.State.ToMetaState(),
        Context = statusContext2,
        Description = prStatus.Description,
        TargetUrl = prStatus.TargetUrl,
        IterationId = prStatus.IterationId,
        Properties = prStatus.Properties
      };
    }

    private static MetaState ToMetaState(this GitStatusState prState)
    {
      switch (prState)
      {
        case GitStatusState.NotSet:
          return MetaState.NotSet;
        case GitStatusState.Pending:
          return MetaState.Pending;
        case GitStatusState.Succeeded:
          return MetaState.Succeeded;
        case GitStatusState.Failed:
        case GitStatusState.PartiallySucceeded:
          return MetaState.Failed;
        case GitStatusState.Error:
          return MetaState.Error;
        case GitStatusState.NotApplicable:
          return MetaState.NotApplicable;
        default:
          return MetaState.NotSet;
      }
    }
  }
}
