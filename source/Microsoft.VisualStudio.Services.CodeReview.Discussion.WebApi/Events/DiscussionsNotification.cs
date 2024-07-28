// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.CodeReview.Discussion.WebApi.Events.DiscussionsNotification
// Assembly: Microsoft.VisualStudio.Services.CodeReview.Discussion.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 443E2621-CB19-4319-96B1-AE621A0F5B5B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.CodeReview.Discussion.WebApi.dll

namespace Microsoft.VisualStudio.Services.CodeReview.Discussion.WebApi.Events
{
  public class DiscussionsNotification
  {
    internal DiscussionsNotification(
      DiscussionThread[] discussions,
      DiscussionComment[] newOrUpdatedComments,
      DiscussionComment[] likedComments)
    {
      this.Threads = discussions;
      this.NewOrUpdatedComments = newOrUpdatedComments;
      this.LikedComments = likedComments;
      this.HasUpdatedCommentsOnly = false;
    }

    public DiscussionThread[] Threads { get; private set; }

    public DiscussionComment[] NewOrUpdatedComments { get; private set; }

    public DiscussionComment[] LikedComments { get; private set; }

    public bool LikeCommentOnly { get; set; }

    public bool WithdrawLikeCommentOnly { get; set; }

    public bool HasUpdatedCommentsOnly { get; set; }

    public bool HasDeletedCommentsOnly { get; set; }
  }
}
