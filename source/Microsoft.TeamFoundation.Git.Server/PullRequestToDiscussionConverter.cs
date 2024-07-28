// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.PullRequestToDiscussionConverter
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.CodeReview.Discussion.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Git.Server
{
  public static class PullRequestToDiscussionConverter
  {
    public static DiscussionThread ToDiscussionItem(this GitPullRequestCommentThread prCommentThread)
    {
      if (prCommentThread == null)
        return (DiscussionThread) null;
      Microsoft.VisualStudio.Services.CodeReview.WebApi.CommentThread discussionItem = new Microsoft.VisualStudio.Services.CodeReview.WebApi.CommentThread();
      discussionItem.DiscussionId = prCommentThread.Id;
      discussionItem.ArtifactUri = prCommentThread.ArtifactUri;
      discussionItem.LastUpdatedDate = prCommentThread.LastUpdatedDate;
      discussionItem.PublishedDate = prCommentThread.PublishedDate;
      discussionItem.Properties = prCommentThread.Properties.ToDiscussionProperties(prCommentThread.ThreadContext);
      discussionItem.Status = prCommentThread.Status.ToDiscussionCommentStatus();
      discussionItem.Comments = prCommentThread.Comments.ToDiscussionCommentList();
      discussionItem.IsDirty = prCommentThread.MarkForUpdate;
      return (DiscussionThread) discussionItem;
    }

    public static PropertiesCollection ToDiscussionProperties(
      this PropertiesCollection prProperties,
      Microsoft.TeamFoundation.SourceControl.WebApi.CommentThreadContext threadContext)
    {
      if (prProperties == null && threadContext == null)
        return (PropertiesCollection) null;
      PropertiesCollection discussionProperties = prProperties == null ? new PropertiesCollection() : new PropertiesCollection((IDictionary<string, object>) prProperties);
      if (threadContext != null)
      {
        if (threadContext.FilePath != null)
          discussionProperties.Add("Microsoft.TeamFoundation.Discussion.ItemPath", (object) threadContext.FilePath);
        if (threadContext.RightFileStart != null && threadContext.RightFileEnd != null)
        {
          discussionProperties.Add("Microsoft.TeamFoundation.Discussion.Position.PositionContext", (object) "RightBuffer");
          discussionProperties.Add("Microsoft.TeamFoundation.Discussion.Position.StartLine", (object) threadContext.RightFileStart.Line);
          discussionProperties.Add("Microsoft.TeamFoundation.Discussion.Position.StartColumn", (object) (threadContext.RightFileStart.Offset + 1));
          discussionProperties.Add("Microsoft.TeamFoundation.Discussion.Position.EndLine", (object) threadContext.RightFileEnd.Line);
          discussionProperties.Add("Microsoft.TeamFoundation.Discussion.Position.EndColumn", (object) (threadContext.RightFileEnd.Offset + 1));
        }
        else if (threadContext.LeftFileStart != null && threadContext.LeftFileEnd != null)
        {
          discussionProperties.Add("Microsoft.TeamFoundation.Discussion.Position.PositionContext", (object) "LeftBuffer");
          discussionProperties.Add("Microsoft.TeamFoundation.Discussion.Position.StartLine", (object) threadContext.LeftFileStart.Line);
          discussionProperties.Add("Microsoft.TeamFoundation.Discussion.Position.StartColumn", (object) (threadContext.LeftFileStart.Offset + 1));
          discussionProperties.Add("Microsoft.TeamFoundation.Discussion.Position.EndLine", (object) threadContext.LeftFileEnd.Line);
          discussionProperties.Add("Microsoft.TeamFoundation.Discussion.Position.EndColumn", (object) (threadContext.LeftFileEnd.Offset + 1));
        }
      }
      return discussionProperties;
    }
  }
}
