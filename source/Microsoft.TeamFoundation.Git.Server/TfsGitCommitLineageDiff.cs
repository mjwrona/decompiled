// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.TfsGitCommitLineageDiff
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

namespace Microsoft.TeamFoundation.Git.Server
{
  public class TfsGitCommitLineageDiff
  {
    public TfsGitCommitLineageDiff(
      TfsGitCommitMetadata metadata,
      int behindCount,
      int aheadCount,
      string refName)
    {
      this.Metadata = metadata;
      this.BehindCount = behindCount;
      this.AheadCount = aheadCount;
      this.RefName = refName;
    }

    public TfsGitCommitMetadata Metadata { get; private set; }

    public int BehindCount { get; private set; }

    public int AheadCount { get; private set; }

    public string RefName { get; private set; }
  }
}
