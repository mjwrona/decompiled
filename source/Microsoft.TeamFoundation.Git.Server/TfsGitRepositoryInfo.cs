// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.TfsGitRepositoryInfo
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using System;
using System.ComponentModel;

namespace Microsoft.TeamFoundation.Git.Server
{
  public class TfsGitRepositoryInfo
  {
    [EditorBrowsable(EditorBrowsableState.Never)]
    public TfsGitRepositoryInfo(
      string name,
      RepoKey key,
      string defaultBranch = null,
      bool isFork = false,
      long size = 0,
      DateTime createdDate = default (DateTime),
      DateTime lastMetadataUpdate = default (DateTime),
      bool isDisabled = false,
      bool isInMaintenance = false)
    {
      this.Name = name;
      this.Key = key;
      this.Size = size;
      this.DefaultBranch = defaultBranch;
      this.IsFork = isFork;
      this.CreatedDate = createdDate;
      this.LastMetadataUpdate = lastMetadataUpdate;
      this.IsDisabled = isDisabled;
      this.IsInMaintenance = isInMaintenance;
    }

    public string Name { get; }

    public RepoKey Key { get; }

    public long Size { get; }

    public string DefaultBranch { get; }

    public bool IsFork { get; }

    public DateTime CreatedDate { get; }

    public DateTime LastMetadataUpdate { get; }

    public bool IsDisabled { get; }

    public bool IsInMaintenance { get; }

    public override string ToString() => this.Name + " (Key: " + this.Key?.ToString() + ")";
  }
}
