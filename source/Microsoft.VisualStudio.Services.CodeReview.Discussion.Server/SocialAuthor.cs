// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.CodeReview.Discussion.Server.SocialAuthor
// Assembly: Microsoft.VisualStudio.Services.CodeReview.Discussion.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FFC5EC6C-1B94-4299-8BA9-787264C21330
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.CodeReview.Discussion.Server.dll

using Microsoft.VisualStudio.Services.WebApi;
using System;

namespace Microsoft.VisualStudio.Services.CodeReview.Discussion.Server
{
  internal class SocialAuthor
  {
    internal int DiscussionId { get; private set; }

    internal short CommentId { get; private set; }

    internal IdentityRef Identity { get; private set; }

    internal SocialAuthor(int discussionId, short commentId, Guid authorId)
    {
      this.DiscussionId = discussionId;
      this.CommentId = commentId;
      this.Identity = new IdentityRef()
      {
        Id = authorId.ToString()
      };
    }

    internal Guid GetAuthorId()
    {
      Guid result;
      return this.Identity != null && Guid.TryParse(this.Identity.Id, out result) ? result : Guid.Empty;
    }

    internal void SetAuthor(IdentityRef userId) => this.Identity = userId;
  }
}
