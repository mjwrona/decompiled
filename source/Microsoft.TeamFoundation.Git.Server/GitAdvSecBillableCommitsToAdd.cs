// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.GitAdvSecBillableCommitsToAdd
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using System;

namespace Microsoft.TeamFoundation.Git.Server
{
  public class GitAdvSecBillableCommitsToAdd
  {
    public GitAdvSecBillableCommitsToAdd(
      Guid projectId,
      Guid repositoryId,
      int pushId,
      Sha1Id commitId,
      string commitNameAndEmail,
      DateTime commitTime)
    {
      this.ProjectId = projectId;
      this.RepositoryId = repositoryId;
      this.PushId = pushId;
      this.CommitId = commitId;
      this.CommitNameAndEmail = commitNameAndEmail;
      this.CommitTime = commitTime;
    }

    public Guid ProjectId { get; }

    public Guid RepositoryId { get; }

    public int PushId { get; }

    public Sha1Id CommitId { get; }

    public string CommitNameAndEmail { get; }

    public DateTime CommitTime { get; }
  }
}
