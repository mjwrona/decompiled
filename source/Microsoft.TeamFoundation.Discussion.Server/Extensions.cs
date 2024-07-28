// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Discussion.Server.Extensions
// Assembly: Microsoft.TeamFoundation.Discussion.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4DCA91C2-88ED-4792-BE4A-3104961AE8D8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Discussion.Server.dll

using Microsoft.VisualStudio.Services.CodeReview.Discussion.WebApi;

namespace Microsoft.TeamFoundation.Discussion.Server
{
  public static class Extensions
  {
    internal static LegacyComment ToLegacyComment(this DiscussionComment comment) => new LegacyComment()
    {
      CommentId = comment.CommentId,
      ParentCommentId = comment.ParentCommentId,
      DiscussionId = comment.DiscussionId,
      Author = comment.GetAuthorId(),
      CommentType = (byte) comment.CommentType,
      Content = comment.Content,
      PublishedDate = comment.PublishedDate,
      IsDeleted = comment.IsDeleted
    };

    internal static LegacyDiscussionThread ToLegacyDiscussionThread(this DiscussionThread thread)
    {
      LegacyDiscussionThread discussionThread = new LegacyDiscussionThread()
      {
        DiscussionId = thread.DiscussionId,
        Status = (byte) thread.Status,
        Severity = (byte) thread.Severity,
        WorkItemId = thread.WorkItemId,
        VersionUri = thread.ArtifactUri,
        PublishedDate = thread.PublishedDate,
        LastUpdatedDate = thread.LastUpdatedDate,
        Revision = thread.Revision,
        IsDirty = thread.IsDirty
      };
      if (thread.Properties != null)
      {
        string str1;
        if (thread.Properties.TryGetValue<string>("Microsoft.TeamFoundation.Discussion.ItemPath", out str1))
          discussionThread.ItemPath = str1;
        int num1;
        int num2;
        int num3;
        int num4;
        int num5;
        int num6;
        string str2;
        if ((0 | (thread.Properties.TryGetValue<int>("Microsoft.TeamFoundation.Discussion.Position.StartLine", out num1) ? 1 : 0) | (thread.Properties.TryGetValue<int>("Microsoft.TeamFoundation.Discussion.Position.EndLine", out num2) ? 1 : 0) | (thread.Properties.TryGetValue<int>("Microsoft.TeamFoundation.Discussion.Position.StartColumn", out num3) ? 1 : 0) | (thread.Properties.TryGetValue<int>("Microsoft.TeamFoundation.Discussion.Position.EndColumn", out num4) ? 1 : 0) | (thread.Properties.TryGetValue<int>("Microsoft.TeamFoundation.Discussion.Position.StartCharPosition", out num5) ? 1 : 0) | (thread.Properties.TryGetValue<int>("Microsoft.TeamFoundation.Discussion.Position.EndCharPosition", out num6) ? 1 : 0) | (thread.Properties.TryGetValue<string>("Microsoft.TeamFoundation.Discussion.Position.PositionContext", out str2) ? 1 : 0)) != 0)
          discussionThread.Position = new LegacyDiscussionPosition()
          {
            StartLine = num1,
            EndLine = num2,
            StartColumn = num3,
            EndColumn = num4,
            StartCharPosition = num5,
            EndCharPosition = num6,
            PositionContext = str2
          };
      }
      return discussionThread;
    }
  }
}
