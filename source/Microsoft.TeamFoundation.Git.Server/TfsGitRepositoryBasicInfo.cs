// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.TfsGitRepositoryBasicInfo
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

namespace Microsoft.TeamFoundation.Git.Server
{
  public class TfsGitRepositoryBasicInfo
  {
    public TfsGitRepositoryBasicInfo(
      string name,
      RepoKey key,
      bool isFork = false,
      long size = 0,
      bool isDisabled = false,
      bool isInMaintenance = false)
    {
      this.Name = name;
      this.Key = key;
      this.Size = size;
      this.IsFork = isFork;
      this.IsDisabled = isDisabled;
      this.IsInMaintenance = isInMaintenance;
    }

    public string Name { get; }

    public RepoKey Key { get; }

    public long Size { get; }

    public bool IsFork { get; }

    public bool IsDisabled { get; }

    public bool IsInMaintenance { get; }
  }
}
