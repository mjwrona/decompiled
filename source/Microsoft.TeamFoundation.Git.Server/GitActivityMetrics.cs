// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.GitActivityMetrics
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Git.Server
{
  public class GitActivityMetrics
  {
    public GitActivityMetrics(
      RepoScope repoScope,
      int commitsPushedCount,
      int pullRequestsCreatedCount,
      int pullRequestsCompletedCount,
      int authorsCount,
      List<CommitsTrendItem> commitsTrend)
    {
      this.RepoScope = repoScope;
      this.CommitsPushedCount = commitsPushedCount;
      this.PullRequestsCreatedCount = pullRequestsCreatedCount;
      this.PullRequestsCompletedCount = pullRequestsCompletedCount;
      this.AuthorsCount = authorsCount;
      this.CommitsTrend = commitsTrend;
    }

    public int CommitsPushedCount { get; }

    public int PullRequestsCreatedCount { get; }

    public int PullRequestsCompletedCount { get; }

    public int AuthorsCount { get; }

    public List<CommitsTrendItem> CommitsTrend { get; }

    public RepoScope RepoScope { get; }
  }
}
