// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.PullRequestCommentConverter
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.CodeReview.Discussion.WebApi;
using Microsoft.VisualStudio.Services.CodeReview.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Git.Server
{
  public static class PullRequestCommentConverter
  {
    public static IList<Comment> ToCommentList(
      this IEnumerable<DiscussionComment> discussionComments)
    {
      if (discussionComments == null)
        return (IList<Comment>) null;
      List<Comment> commentList = new List<Comment>();
      foreach (DiscussionComment discussionComment in discussionComments)
        commentList.Add(discussionComment.ToSourceControlItem());
      return (IList<Comment>) commentList;
    }

    public static Comment ToSourceControlItem(this DiscussionComment comment) => new Comment()
    {
      Id = comment.CommentId,
      ParentCommentId = comment.ParentCommentId,
      Author = comment.Author,
      CommentType = comment.CommentType.ToCommentType(),
      Content = comment.Content,
      IsDeleted = comment.IsDeleted,
      LastUpdatedDate = comment.LastUpdatedDate,
      LastContentUpdatedDate = comment.LastContentUpdatedDate,
      PublishedDate = comment.PublishedDate,
      UsersLiked = (IList<IdentityRef>) comment.UsersLiked
    };

    public static CommentThreadStatus ToCommentStatus(this DiscussionStatus discussionStatus)
    {
      switch (discussionStatus)
      {
        case DiscussionStatus.Unknown:
          return CommentThreadStatus.Unknown;
        case DiscussionStatus.Active:
          return CommentThreadStatus.Active;
        case DiscussionStatus.Fixed:
          return CommentThreadStatus.Fixed;
        case DiscussionStatus.WontFix:
          return CommentThreadStatus.WontFix;
        case DiscussionStatus.Closed:
          return CommentThreadStatus.Closed;
        case DiscussionStatus.ByDesign:
          return CommentThreadStatus.ByDesign;
        case DiscussionStatus.Pending:
          return CommentThreadStatus.Pending;
        default:
          return CommentThreadStatus.Unknown;
      }
    }

    public static Microsoft.TeamFoundation.SourceControl.WebApi.CommentType ToCommentType(
      this Microsoft.VisualStudio.Services.CodeReview.Discussion.WebApi.CommentType commentType)
    {
      switch (commentType)
      {
        case Microsoft.VisualStudio.Services.CodeReview.Discussion.WebApi.CommentType.Unknown:
          return Microsoft.TeamFoundation.SourceControl.WebApi.CommentType.Unknown;
        case Microsoft.VisualStudio.Services.CodeReview.Discussion.WebApi.CommentType.Text:
          return Microsoft.TeamFoundation.SourceControl.WebApi.CommentType.Text;
        case Microsoft.VisualStudio.Services.CodeReview.Discussion.WebApi.CommentType.CodeChange:
          return Microsoft.TeamFoundation.SourceControl.WebApi.CommentType.CodeChange;
        case Microsoft.VisualStudio.Services.CodeReview.Discussion.WebApi.CommentType.System:
          return Microsoft.TeamFoundation.SourceControl.WebApi.CommentType.System;
        default:
          return Microsoft.TeamFoundation.SourceControl.WebApi.CommentType.Unknown;
      }
    }

    public static Microsoft.TeamFoundation.SourceControl.WebApi.CommentThreadContext ToThreadContext(
      this Microsoft.VisualStudio.Services.CodeReview.WebApi.CommentThreadContext crThreadContext)
    {
      if (crThreadContext == null)
        return (Microsoft.TeamFoundation.SourceControl.WebApi.CommentThreadContext) null;
      return new Microsoft.TeamFoundation.SourceControl.WebApi.CommentThreadContext()
      {
        LeftFileStart = crThreadContext.LeftFileStart.ToCommentPosition(),
        RightFileStart = crThreadContext.RightFileStart.ToCommentPosition(),
        LeftFileEnd = crThreadContext.LeftFileEnd.ToCommentPosition(),
        RightFileEnd = crThreadContext.RightFileEnd.ToCommentPosition(),
        FilePath = crThreadContext.FilePath
      };
    }

    public static CommentPosition ToCommentPosition(this Position position)
    {
      if (position == null)
        return (CommentPosition) null;
      return new CommentPosition()
      {
        Line = position.Line,
        Offset = position.Offset + 1
      };
    }
  }
}
