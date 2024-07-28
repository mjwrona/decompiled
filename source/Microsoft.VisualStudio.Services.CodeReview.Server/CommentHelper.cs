// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.CodeReview.Server.CommentHelper
// Assembly: Microsoft.VisualStudio.Services.CodeReview.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2BCB2866-BDCB-4FDE-9EA3-48DFA660C131
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.CodeReview.Server.dll

using Microsoft.VisualStudio.Services.CodeReview.Discussion.WebApi;
using Microsoft.VisualStudio.Services.CodeReview.Server.Common;
using Microsoft.VisualStudio.Services.CodeReview.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.CodeReview.Server
{
  public static class CommentHelper
  {
    internal static DiscussionThread ToDiscussionThread(this CommentThread reviewCommentThread)
    {
      DiscussionThread discussionThread = (DiscussionThread) reviewCommentThread;
      discussionThread.ArtifactUri = CodeReviewSdkArtifactId.GetArtifactUri(reviewCommentThread);
      discussionThread.CommentsCount = reviewCommentThread.Comments != null ? reviewCommentThread.Comments.Length : 0;
      if (reviewCommentThread.ThreadContext != null)
      {
        discussionThread.Properties = discussionThread.Properties ?? new PropertiesCollection();
        foreach (KeyValuePair<string, object> property in (IEnumerable<KeyValuePair<string, object>>) reviewCommentThread.ThreadContext.ToProperties())
          discussionThread.Properties.Add(property.Key, property.Value);
      }
      return discussionThread;
    }

    internal static CommentThread ToReviewCommentThread(this DiscussionThread discussionThread)
    {
      CommentThread subclass = discussionThread.ToSubclass<CommentThread>();
      Guid projectId;
      int reviewId;
      DiscussionExtensions.ExtractMetadata(discussionThread.ArtifactUri, out projectId, out reviewId);
      subclass.ProjectId = projectId;
      subclass.ReviewId = reviewId;
      subclass.ThreadContext = discussionThread.Properties.ToCommentThreadContext();
      subclass.Properties = CommentExtension.RemovedMappedProperties(subclass.Properties);
      return subclass;
    }

    internal static PropertiesCollection ToProperties(this CommentThreadContext threadContext)
    {
      PropertiesCollection properties = new PropertiesCollection();
      if (threadContext == null)
        return properties;
      if (threadContext.LeftFileStart != null)
      {
        properties.Add("Microsoft.VisualStudio.Services.CodeReview.Left.StartLine", (object) threadContext.LeftFileStart.Line);
        properties.Add("Microsoft.VisualStudio.Services.CodeReview.Left.StartOffset", (object) threadContext.LeftFileStart.Offset);
      }
      if (threadContext.LeftFileEnd != null)
      {
        properties.Add("Microsoft.VisualStudio.Services.CodeReview.Left.EndLine", (object) threadContext.LeftFileEnd.Line);
        properties.Add("Microsoft.VisualStudio.Services.CodeReview.Left.EndOffset", (object) threadContext.LeftFileEnd.Offset);
      }
      if (threadContext.RightFileStart != null)
      {
        properties.Add("Microsoft.VisualStudio.Services.CodeReview.Right.StartLine", (object) threadContext.RightFileStart.Line);
        properties.Add("Microsoft.VisualStudio.Services.CodeReview.Right.StartOffset", (object) threadContext.RightFileStart.Offset);
      }
      if (threadContext.RightFileEnd != null)
      {
        properties.Add("Microsoft.VisualStudio.Services.CodeReview.Right.EndLine", (object) threadContext.RightFileEnd.Line);
        properties.Add("Microsoft.VisualStudio.Services.CodeReview.Right.EndOffset", (object) threadContext.RightFileEnd.Offset);
      }
      if (threadContext.IterationContext != null)
      {
        properties.Add("Microsoft.VisualStudio.Services.CodeReview.FirstComparingIteration", (object) threadContext.IterationContext.FirstComparingIteration);
        properties.Add("Microsoft.VisualStudio.Services.CodeReview.SecondComparingIteration", (object) threadContext.IterationContext.SecondComparingIteration);
      }
      if (threadContext.ChangeTrackingId > 0)
        properties.Add("Microsoft.VisualStudio.Services.CodeReview.ChangeTrackingId", (object) threadContext.ChangeTrackingId);
      if (!string.IsNullOrEmpty(threadContext.FilePath))
        properties.Add("Microsoft.VisualStudio.Services.CodeReview.ItemPath", (object) threadContext.FilePath);
      return properties;
    }
  }
}
