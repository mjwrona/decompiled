// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.TfsGitCommitParent
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

namespace Microsoft.TeamFoundation.Git.Server
{
  internal class TfsGitCommitParent
  {
    public TfsGitCommitParent(Sha1Id commitId, Sha1Id parentCommitId)
    {
      this.CommitId = commitId;
      this.ParentCommitId = parentCommitId;
    }

    public Sha1Id CommitId { get; private set; }

    public Sha1Id ParentCommitId { get; private set; }
  }
}
