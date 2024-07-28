// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.TfsGitPullRequestDelegatedVote
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using System;

namespace Microsoft.TeamFoundation.Git.Server
{
  internal class TfsGitPullRequestDelegatedVote
  {
    public Guid ReviewerId { get; private set; }

    public Guid VotedForId { get; private set; }

    public int? PullRequestId { get; private set; }

    public TfsGitPullRequestDelegatedVote(Guid reviewerId, Guid votedForId, int? pullRequestId = null)
    {
      this.ReviewerId = reviewerId;
      this.VotedForId = votedForId;
      this.PullRequestId = pullRequestId;
    }
  }
}
