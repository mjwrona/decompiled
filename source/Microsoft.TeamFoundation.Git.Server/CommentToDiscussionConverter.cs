// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.CommentToDiscussionConverter
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.CodeReview.Discussion.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Git.Server
{
  public static class CommentToDiscussionConverter
  {
    public static DiscussionComment[] ToDiscussionCommentList(this IList<Comment> comments)
    {
      if (comments == null)
        return (DiscussionComment[]) null;
      List<DiscussionComment> discussionCommentList = new List<DiscussionComment>();
      foreach (Comment comment in (IEnumerable<Comment>) comments)
        discussionCommentList.Add(new DiscussionComment()
        {
          CommentId = comment.Id,
          ParentCommentId = comment.ParentCommentId,
          DiscussionId = comment.ThreadId,
          Author = comment.Author,
          CommentType = comment.CommentType.ToCRCommentType(),
          Content = comment.Content,
          IsDeleted = comment.IsDeleted,
          LastUpdatedDate = comment.LastUpdatedDate,
          PublishedDate = comment.PublishedDate,
          UsersLiked = comment.UsersLiked != null ? comment.UsersLiked.ToList<IdentityRef>() : (List<IdentityRef>) null
        });
      return discussionCommentList.ToArray();
    }
  }
}
