// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.GitLogRevisionRange
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Git.Server.Graph;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Git.Server
{
  public class GitLogRevisionRange : IRevisionRange
  {
    public GitLogRevisionRange(Sha1Id commitId)
    {
      this.CommitId = commitId;
      this.CompareToCommitId = new Sha1Id?();
    }

    public GitLogRevisionRange(Sha1Id commitId, Sha1Id compareToCommitId)
    {
      this.CommitId = commitId;
      this.CompareToCommitId = new Sha1Id?(compareToCommitId);
    }

    public Sha1Id CommitId { get; }

    public Sha1Id? CompareToCommitId { get; }

    public IEnumerable<Sha1Id> GetReachableFromCommits(ITfsGitRepository repo)
    {
      yield return this.CommitId;
    }

    public IEnumerable<Sha1Id> GetRequiredCommits(ITfsGitRepository repo)
    {
      yield return this.CommitId;
      if (this.CompareToCommitId.HasValue)
        yield return this.CompareToCommitId.Value;
    }

    public IReadOnlySet<int> GetRestrictedLabels(IGitCommitGraph graph)
    {
      if (!this.CompareToCommitId.HasValue || !graph.HasVertex(this.CompareToCommitId.Value))
        return (IReadOnlySet<int>) ReadOnlyUniversalSet<int>.Instance;
      return (IReadOnlySet<int>) new NotReachableLabels<int, Sha1Id>((IDirectedGraph<int, Sha1Id>) graph, (IEnumerable<int>) new int[1]
      {
        graph.GetLabel(this.CompareToCommitId.Value)
      });
    }
  }
}
