// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.PullRequestCommentNotification
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using System;

namespace Microsoft.TeamFoundation.Git.Server
{
  public class PullRequestCommentNotification : PullRequestNotification
  {
    public PullRequestCommentNotification()
      : base((string) null, Guid.Empty, (string) null, 0)
    {
    }

    internal PullRequestCommentNotification(
      string teamProjectUri,
      Guid repositoryId,
      string repositoryName,
      int pullRequestId,
      int threadId,
      int commentId,
      Guid commenter)
      : base(teamProjectUri, repositoryId, repositoryName, pullRequestId)
    {
      this.ThreadId = threadId;
      this.CommentId = commentId;
      this.Commenter = commenter;
    }

    public Guid Commenter { get; private set; }

    public int ThreadId { get; private set; }

    public int CommentId { get; private set; }
  }
}
