// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.PullRequestToCommentConverter
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.CodeReview.Discussion.WebApi;
using Microsoft.VisualStudio.Services.CodeReview.WebApi;

namespace Microsoft.TeamFoundation.Git.Server
{
  public static class PullRequestToCommentConverter
  {
    public static DiscussionStatus ToDiscussionCommentStatus(this CommentThreadStatus commentStatus)
    {
      switch (commentStatus)
      {
        case CommentThreadStatus.Unknown:
          return DiscussionStatus.Unknown;
        case CommentThreadStatus.Active:
          return DiscussionStatus.Active;
        case CommentThreadStatus.Fixed:
          return DiscussionStatus.Fixed;
        case CommentThreadStatus.WontFix:
          return DiscussionStatus.WontFix;
        case CommentThreadStatus.Closed:
          return DiscussionStatus.Closed;
        case CommentThreadStatus.ByDesign:
          return DiscussionStatus.ByDesign;
        case CommentThreadStatus.Pending:
          return DiscussionStatus.Pending;
        default:
          return DiscussionStatus.Unknown;
      }
    }

    public static Microsoft.VisualStudio.Services.CodeReview.Discussion.WebApi.CommentType ToCRCommentType(
      this Microsoft.TeamFoundation.SourceControl.WebApi.CommentType commentType)
    {
      switch (commentType)
      {
        case Microsoft.TeamFoundation.SourceControl.WebApi.CommentType.Unknown:
          return Microsoft.VisualStudio.Services.CodeReview.Discussion.WebApi.CommentType.Unknown;
        case Microsoft.TeamFoundation.SourceControl.WebApi.CommentType.Text:
          return Microsoft.VisualStudio.Services.CodeReview.Discussion.WebApi.CommentType.Text;
        case Microsoft.TeamFoundation.SourceControl.WebApi.CommentType.CodeChange:
          return Microsoft.VisualStudio.Services.CodeReview.Discussion.WebApi.CommentType.CodeChange;
        case Microsoft.TeamFoundation.SourceControl.WebApi.CommentType.System:
          return Microsoft.VisualStudio.Services.CodeReview.Discussion.WebApi.CommentType.System;
        default:
          return Microsoft.VisualStudio.Services.CodeReview.Discussion.WebApi.CommentType.Unknown;
      }
    }

    public static Position ToCRCommentPosition(this CommentPosition prPosition)
    {
      if (prPosition == null)
        return (Position) null;
      return new Position()
      {
        Line = prPosition.Line,
        Offset = prPosition.Offset - 1
      };
    }
  }
}
