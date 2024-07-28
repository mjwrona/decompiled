// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.CommitMetadataAndChanges
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Git.Server
{
  public sealed class CommitMetadataAndChanges
  {
    private CommitMetadataAndChanges(
      TfsGitCommitMetadata metadata,
      List<TfsGitCommitChangeWithId> changes,
      ChangeCounts changeCounts,
      int? pushId,
      bool tooManyChanges = false)
    {
      this.Metadata = metadata;
      this.Changes = changes;
      this.ChangeCounts = changeCounts;
      this.PushId = pushId;
      this.TooManyChanges = tooManyChanges;
    }

    internal static CommitMetadataAndChanges ComputeFromKey(
      ITfsGitRepository objectDB,
      CommitMetadataKey key,
      bool preventGitTreeOverflow = false)
    {
      TfsGitCommit commit = objectDB.LookupObject<TfsGitCommit>(key.CommitId);
      TfsGitCommitMetadata metadata = new TfsGitCommitMetadata(commit);
      List<TfsGitCommitChangeWithId> changes = new List<TfsGitCommitChangeWithId>();
      ChangeCounts.Builder builder = new ChangeCounts.Builder();
      try
      {
        foreach (TfsGitDiffEntry entry in commit.GetManifest(objectDB, true, preventGitTreeOverflow: preventGitTreeOverflow))
        {
          changes.Add(new TfsGitCommitChangeWithId(commit.ObjectId, entry));
          builder.Add(entry);
        }
      }
      catch (DiffChangesLimitReachedException ex)
      {
        return new CommitMetadataAndChanges(metadata, changes, builder.Create(), key.PushId, true);
      }
      return new CommitMetadataAndChanges(metadata, changes, builder.Create(), key.PushId);
    }

    internal static CommitMetadataAndChanges TEST_Create(
      TfsGitCommitMetadata metadata,
      List<TfsGitCommitChangeWithId> changes,
      int pushId)
    {
      ChangeCounts.Builder builder = new ChangeCounts.Builder();
      foreach (TfsGitCommitChangeWithId change in changes)
        builder.Add((TfsGitCommitChange) change);
      return new CommitMetadataAndChanges(metadata, changes, builder.Create(), new int?(pushId));
    }

    public TfsGitCommitMetadata Metadata { get; }

    public List<TfsGitCommitChangeWithId> Changes { get; }

    public ChangeCounts ChangeCounts { get; }

    public int? PushId { get; }

    public bool TooManyChanges { get; }
  }
}
