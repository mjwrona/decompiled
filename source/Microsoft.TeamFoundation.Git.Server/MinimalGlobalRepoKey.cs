// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.MinimalGlobalRepoKey
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using System;

namespace Microsoft.TeamFoundation.Git.Server
{
  internal struct MinimalGlobalRepoKey
  {
    public readonly Guid CollectionId;
    public readonly Guid RepositoryId;

    public MinimalGlobalRepoKey(Guid collectionId, Guid repositoryId)
    {
      this.CollectionId = !(collectionId != Guid.Empty) ? collectionId : throw new ArgumentException("Cross-collection forking is currently unsupported.");
      this.RepositoryId = repositoryId;
    }

    public override bool Equals(object obj) => obj is MinimalGlobalRepoKey minimalGlobalRepoKey && this.CollectionId == minimalGlobalRepoKey.CollectionId && this.RepositoryId == minimalGlobalRepoKey.RepositoryId;

    public override int GetHashCode() => HashCodeUtil.GetHashCode<Guid, Guid>(this.CollectionId, this.RepositoryId);
  }
}
