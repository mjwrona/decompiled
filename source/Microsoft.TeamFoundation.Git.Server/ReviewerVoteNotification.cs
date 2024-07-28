// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.ReviewerVoteNotification
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Server.Core;
using System;

namespace Microsoft.TeamFoundation.Git.Server
{
  public class ReviewerVoteNotification : PullRequestNotification
  {
    internal ReviewerVoteNotification(
      string teamProjectUri,
      Guid repositoryId,
      string repositoryName,
      int pullRequestId,
      TeamFoundationIdentity reviewer,
      short reviewerVote,
      bool createDiscussionMessage = true,
      TeamFoundationIdentity initiator = null,
      string reason = null)
      : base(teamProjectUri, repositoryId, repositoryName, pullRequestId)
    {
      this.Reviewer = reviewer;
      this.ReviewerVote = reviewerVote;
      this.CreateDiscussionMessage = createDiscussionMessage;
      this.Initiator = initiator ?? reviewer;
      this.Reason = reason;
    }

    public TeamFoundationIdentity Reviewer { get; }

    public short ReviewerVote { get; }

    public bool CreateDiscussionMessage { get; }

    public TeamFoundationIdentity Initiator { get; }

    public string Reason { get; }
  }
}
