// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.TfsGitCommitHistoryEntry
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

namespace Microsoft.TeamFoundation.Git.Server
{
  public sealed class TfsGitCommitHistoryEntry
  {
    private TfsGitCommitHistoryEntry()
    {
    }

    internal static TfsGitCommitHistoryEntry FromQueryCommitHistory(TfsGitCommitMetadata commit) => new TfsGitCommitHistoryEntry()
    {
      Commit = commit
    };

    internal static TfsGitCommitHistoryEntry FromQueryCommitItems(
      TfsGitCommitChangeWithId change,
      TfsGitCommitMetadata commit)
    {
      return new TfsGitCommitHistoryEntry()
      {
        Change = change,
        Commit = commit
      };
    }

    public TfsGitCommitChangeWithId Change { get; internal set; }

    public TfsGitCommitMetadata Commit { get; internal set; }

    public ChangeCounts ChangeCounts { get; internal set; }
  }
}
