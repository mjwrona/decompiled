// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.RawRepoInfo
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using System;

namespace Microsoft.TeamFoundation.Git.Server
{
  internal class RawRepoInfo
  {
    public RawRepoInfo(
      int dataspaceId,
      Guid repoId,
      string name,
      Guid containerId,
      bool createdByForking,
      long compressedSize)
    {
      this.DataspaceId = dataspaceId;
      this.RepoId = repoId;
      this.Name = name;
      this.ContainerId = containerId;
      this.CreatedByForking = createdByForking;
      this.CompressedSize = compressedSize;
    }

    public RawRepoInfo(
      int dataspaceId,
      Guid repoId,
      string name,
      Guid containerId,
      bool createdByForking,
      long compressedSize,
      DateTime createdDate,
      DateTime lastMetadataUpdate)
    {
      this.DataspaceId = dataspaceId;
      this.RepoId = repoId;
      this.Name = name;
      this.ContainerId = containerId;
      this.CreatedByForking = createdByForking;
      this.CompressedSize = compressedSize;
      this.CreatedDate = createdDate;
      this.LastMetadataUpdate = lastMetadataUpdate;
    }

    public int DataspaceId { get; }

    public Guid RepoId { get; }

    public string Name { get; }

    public Guid ContainerId { get; }

    public bool CreatedByForking { get; }

    public long CompressedSize { get; }

    public DateTime CreatedDate { get; }

    public DateTime LastMetadataUpdate { get; }
  }
}
