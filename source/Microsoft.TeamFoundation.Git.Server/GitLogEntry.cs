// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.GitLogEntry
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Git.Server
{
  public class GitLogEntry
  {
    internal GitLogEntry(ITfsGitRepository repo, TfsGitCommit commit, TfsGitCommitChange change)
      : this(repo, commit, change, (IEnumerable<Sha1Id>) null)
    {
    }

    internal GitLogEntry(
      ITfsGitRepository repo,
      TfsGitCommit commit,
      TfsGitCommitChange change,
      IEnumerable<Sha1Id> outEdges)
    {
      this.Commit = commit;
      this.Change = change;
      this.OutEdges = outEdges != null ? (IReadOnlyList<Sha1Id>) outEdges.ToList<Sha1Id>() : (IReadOnlyList<Sha1Id>) null;
    }

    public TfsGitCommit Commit { get; }

    public TfsGitCommitChange Change { get; }

    public IReadOnlyList<Sha1Id> OutEdges { get; }
  }
}
