// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.TfsGitCommitChangeWithId
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.VisualStudio.Services.Common;

namespace Microsoft.TeamFoundation.Git.Server
{
  public class TfsGitCommitChangeWithId : TfsGitCommitChange
  {
    internal TfsGitCommitChangeWithId(Sha1Id commitId) => this.CommitId = commitId;

    public TfsGitCommitChangeWithId(Sha1Id commitId, TfsGitDiffEntry entry)
      : base(entry)
    {
      this.CommitId = commitId;
    }

    internal TfsGitCommitChangeWithId(Sha1Id commitId, TfsGitCommitChange change)
      : base(change)
    {
      this.CommitId = commitId;
    }

    public Sha1Id CommitId { get; set; }

    internal override TfsGitCommitChangeWithId WithId(Sha1Id commitId)
    {
      ArgumentUtility.CheckForOutOfRange<Sha1Id>(commitId, nameof (commitId), this.CommitId, this.CommitId);
      return this;
    }
  }
}
