// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.TfsGitDeletedRepositoryInfo
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using System;

namespace Microsoft.TeamFoundation.Git.Server
{
  public class TfsGitDeletedRepositoryInfo
  {
    internal TfsGitDeletedRepositoryInfo(
      string name,
      RepoKey repoKey,
      Guid deletedBy,
      DateTime createdDate,
      DateTime deletedDate)
    {
      this.Name = name;
      this.Key = repoKey;
      this.DeletedBy = deletedBy;
      this.CreatedDate = createdDate;
      this.DeletedDate = deletedDate;
    }

    public string Name { get; }

    public RepoKey Key { get; }

    public Guid DeletedBy { get; }

    public DateTime CreatedDate { get; }

    public DateTime DeletedDate { get; }
  }
}
