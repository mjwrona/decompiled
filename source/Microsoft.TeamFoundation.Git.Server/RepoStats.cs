// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.RepoStats
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

namespace Microsoft.TeamFoundation.Git.Server
{
  public class RepoStats
  {
    public RepoStats(
      RepoKey repoKey,
      int commitsCount,
      int branchesCount,
      int activePullRequestsCount)
    {
      this.RepoKey = repoKey;
      this.CommitsCount = commitsCount;
      this.BranchesCount = branchesCount;
      this.ActivePullRequestsCount = activePullRequestsCount;
    }

    public RepoKey RepoKey { get; }

    public int CommitsCount { get; }

    public int BranchesCount { get; }

    public int ActivePullRequestsCount { get; }
  }
}
