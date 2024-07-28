// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.CodeReview.WebApi.CommentExtension
// Assembly: Microsoft.VisualStudio.Services.CodeReview.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 84DE81C5-ABF4-4E22-A82B-21BA09D9141E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.CodeReview.WebApi.dll

using Microsoft.VisualStudio.Services.CodeReview.Discussion.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.CodeReview.WebApi
{
  public static class CommentExtension
  {
    public static DiscussionComment ShallowClone(this DiscussionComment toClone) => new DiscussionComment()
    {
      CommentId = toClone.CommentId,
      Author = toClone.Author,
      CanDelete = toClone.CanDelete,
      CommentType = toClone.CommentType,
      Content = toClone.Content,
      DiscussionId = toClone.DiscussionId,
      IsDeleted = toClone.IsDeleted,
      LastUpdatedDate = toClone.LastUpdatedDate,
      ParentCommentId = toClone.ParentCommentId,
      PublishedDate = toClone.PublishedDate,
      UsersLiked = toClone.UsersLiked != null ? new List<IdentityRef>((IEnumerable<IdentityRef>) toClone.UsersLiked) : (List<IdentityRef>) null
    };

    public static CommentThread ShallowClone(this DiscussionThread toClone)
    {
      CommentThread commentThread = new CommentThread();
      commentThread.ArtifactUri = toClone.ArtifactUri;
      commentThread.Comments = toClone.Comments == null ? (DiscussionComment[]) null : ((IEnumerable<DiscussionComment>) toClone.Comments).Select<DiscussionComment, DiscussionComment>((Func<DiscussionComment, DiscussionComment>) (x => x.ShallowClone())).ToArray<DiscussionComment>();
      commentThread.CommentsCount = toClone.CommentsCount;
      commentThread.DiscussionId = toClone.DiscussionId;
      commentThread.IsDirty = toClone.IsDirty;
      commentThread.LastUpdatedDate = toClone.LastUpdatedDate;
      commentThread.PropertyId = toClone.PropertyId;
      commentThread.PublishedDate = toClone.PublishedDate;
      commentThread.Revision = toClone.Revision;
      commentThread.VersionId = toClone.VersionId;
      commentThread.Status = toClone.Status;
      commentThread.Properties = toClone.Properties;
      return commentThread;
    }

    public static CommentThreadContext ToCommentThreadContext(this PropertiesCollection properties)
    {
      CommentThreadContext commentThreadContext = (CommentThreadContext) null;
      if (properties != null && properties.Count > 0 && (properties.ContainsKey("Microsoft.VisualStudio.Services.CodeReview.Left.StartLine") || properties.ContainsKey("Microsoft.VisualStudio.Services.CodeReview.Right.StartLine") || properties.ContainsKey("Microsoft.VisualStudio.Services.CodeReview.ChangeTrackingId") || properties.ContainsKey("Microsoft.VisualStudio.Services.CodeReview.ItemPath")))
      {
        commentThreadContext = new CommentThreadContext();
        if (properties.ContainsKey("Microsoft.VisualStudio.Services.CodeReview.Left.StartLine") && properties.ContainsKey("Microsoft.VisualStudio.Services.CodeReview.Left.StartOffset"))
          commentThreadContext.LeftFileStart = new Position()
          {
            Line = (int) properties["Microsoft.VisualStudio.Services.CodeReview.Left.StartLine"],
            Offset = (int) properties["Microsoft.VisualStudio.Services.CodeReview.Left.StartOffset"]
          };
        if (properties.ContainsKey("Microsoft.VisualStudio.Services.CodeReview.Left.EndLine") && properties.ContainsKey("Microsoft.VisualStudio.Services.CodeReview.Left.EndOffset"))
          commentThreadContext.LeftFileEnd = new Position()
          {
            Line = (int) properties["Microsoft.VisualStudio.Services.CodeReview.Left.EndLine"],
            Offset = (int) properties["Microsoft.VisualStudio.Services.CodeReview.Left.EndOffset"]
          };
        if (properties.ContainsKey("Microsoft.VisualStudio.Services.CodeReview.Right.StartLine") && properties.ContainsKey("Microsoft.VisualStudio.Services.CodeReview.Right.StartOffset"))
          commentThreadContext.RightFileStart = new Position()
          {
            Line = (int) properties["Microsoft.VisualStudio.Services.CodeReview.Right.StartLine"],
            Offset = (int) properties["Microsoft.VisualStudio.Services.CodeReview.Right.StartOffset"]
          };
        if (properties.ContainsKey("Microsoft.VisualStudio.Services.CodeReview.Right.EndLine") && properties.ContainsKey("Microsoft.VisualStudio.Services.CodeReview.Right.EndOffset"))
          commentThreadContext.RightFileEnd = new Position()
          {
            Line = (int) properties["Microsoft.VisualStudio.Services.CodeReview.Right.EndLine"],
            Offset = (int) properties["Microsoft.VisualStudio.Services.CodeReview.Right.EndOffset"]
          };
        if (properties.ContainsKey("Microsoft.VisualStudio.Services.CodeReview.ChangeTrackingId"))
          commentThreadContext.ChangeTrackingId = (int) properties["Microsoft.VisualStudio.Services.CodeReview.ChangeTrackingId"];
        if (properties.ContainsKey("Microsoft.VisualStudio.Services.CodeReview.ItemPath"))
          commentThreadContext.FilePath = (string) properties["Microsoft.VisualStudio.Services.CodeReview.ItemPath"];
        commentThreadContext.IterationContext = properties.ToIterationContext();
      }
      return commentThreadContext;
    }

    public static IterationContext ToIterationContext(this PropertiesCollection properties)
    {
      IterationContext iterationContext = (IterationContext) null;
      if (properties != null && properties.Count > 0 && properties.ContainsKey("Microsoft.VisualStudio.Services.CodeReview.FirstComparingIteration") && properties.ContainsKey("Microsoft.VisualStudio.Services.CodeReview.SecondComparingIteration"))
        iterationContext = new IterationContext()
        {
          FirstComparingIteration = Convert.ToInt16(properties["Microsoft.VisualStudio.Services.CodeReview.FirstComparingIteration"]),
          SecondComparingIteration = Convert.ToInt16(properties["Microsoft.VisualStudio.Services.CodeReview.SecondComparingIteration"])
        };
      return iterationContext;
    }

    public static PropertiesCollection RemovedMappedProperties(PropertiesCollection properties)
    {
      if (properties == null)
        return (PropertiesCollection) null;
      PropertiesCollection propertiesCollection = new PropertiesCollection();
      foreach (KeyValuePair<string, object> property in (IEnumerable<KeyValuePair<string, object>>) properties)
      {
        if (property.Key != "Microsoft.VisualStudio.Services.CodeReview.Left.StartLine" && property.Key != "Microsoft.VisualStudio.Services.CodeReview.Left.StartOffset" && property.Key != "Microsoft.VisualStudio.Services.CodeReview.Left.EndLine" && property.Key != "Microsoft.VisualStudio.Services.CodeReview.Left.EndOffset" && property.Key != "Microsoft.VisualStudio.Services.CodeReview.Right.StartLine" && property.Key != "Microsoft.VisualStudio.Services.CodeReview.Right.StartOffset" && property.Key != "Microsoft.VisualStudio.Services.CodeReview.Right.EndLine" && property.Key != "Microsoft.VisualStudio.Services.CodeReview.Right.EndOffset" && property.Key != "Microsoft.VisualStudio.Services.CodeReview.ItemPath" && property.Key != "Microsoft.VisualStudio.Services.CodeReview.ChangeTrackingId" && property.Key != "Microsoft.VisualStudio.Services.CodeReview.FirstComparingIteration" && property.Key != "Microsoft.VisualStudio.Services.CodeReview.SecondComparingIteration")
          propertiesCollection.Add(property.Key, property.Value);
      }
      return propertiesCollection;
    }

    public static CommentThread ToReviewCommentThread(
      this DiscussionThread discussionThread,
      Guid projectId,
      int reviewId)
    {
      CommentThread subclass = discussionThread.ToSubclass<CommentThread>();
      subclass.ProjectId = projectId;
      subclass.ReviewId = reviewId;
      subclass.ThreadContext = discussionThread.Properties.ToCommentThreadContext();
      subclass.Properties = CommentExtension.RemovedMappedProperties(subclass.Properties);
      return subclass;
    }
  }
}
