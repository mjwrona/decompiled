// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.CommitMetadataUpdate
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Git.Server.Storage;
using System;

namespace Microsoft.TeamFoundation.Git.Server
{
  internal sealed class CommitMetadataUpdate
  {
    public CommitMetadataUpdate(
      Odb odb,
      TfsGitCommit commit,
      Action<TfsGitDiffEntry> onChange,
      bool preventGitTreeOverflow = false)
    {
      this.Metadata = new TfsGitCommitMetadata(commit);
      ChangeCounts.Builder builder = new ChangeCounts.Builder();
      try
      {
        foreach (TfsGitDiffEntry entry in commit.GetManifestFromOdb(odb, true, false, preventGitTreeOverflow: preventGitTreeOverflow))
        {
          if (onChange != null)
            onChange(entry);
          builder.Add(entry);
        }
      }
      catch (DiffChangesLimitReachedException ex)
      {
        this.ChangeCounts = new ChangeCounts.Builder().Create();
        return;
      }
      this.ChangeCounts = builder.Create();
    }

    public CommitMetadataUpdate(CommitMetadataAndChanges commit)
    {
      this.Metadata = commit.Metadata;
      this.ChangeCounts = commit.ChangeCounts;
    }

    public GitCommitChangeSummary ToChangeSummary() => new GitCommitChangeSummary(this.Metadata.CommitId, GitCommitMetadataStatus.ChangesCalculated, this.ChangeCounts);

    public TfsGitCommitMetadata Metadata { get; }

    public ChangeCounts ChangeCounts { get; }
  }
}
