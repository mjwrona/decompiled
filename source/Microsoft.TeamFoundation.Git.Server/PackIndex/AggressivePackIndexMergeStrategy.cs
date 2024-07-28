// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.PackIndex.AggressivePackIndexMergeStrategy
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Framework.Server;

namespace Microsoft.TeamFoundation.Git.Server.PackIndex
{
  internal class AggressivePackIndexMergeStrategy : IPackIndexMergeStrategy
  {
    private readonly long m_targetTipIndexSize;
    private readonly long m_targetStagingIndexSize;

    public AggressivePackIndexMergeStrategy(long targetTipIndexSize = 65536, long targetStagingIndexSize = 8388608)
    {
      this.m_targetTipIndexSize = targetTipIndexSize;
      this.m_targetStagingIndexSize = targetStagingIndexSize;
    }

    public ConcatGitPackIndex Merge(
      IVssRequestContext rc,
      ConcatGitPackIndex unmerged,
      PackIndexSizeEstimator sizeEstimator)
    {
      PackIndexMerger packIndexMerger = new PackIndexMerger(rc, unmerged, sizeEstimator);
      packIndexMerger.MergeUntilSize(this.m_targetTipIndexSize);
      if (packIndexMerger.CurrentSubIndexCount - packIndexMerger.LockedBases <= 3)
        return packIndexMerger.GetCurrentMergedIndex();
      packIndexMerger.MergeUntilSize(this.m_targetStagingIndexSize);
      if (packIndexMerger.CurrentSubIndexCount - packIndexMerger.LockedBases <= 2)
        return packIndexMerger.GetCurrentMergedIndex();
      packIndexMerger.MergeUntilSize(long.MaxValue);
      return packIndexMerger.GetCurrentMergedIndex();
    }
  }
}
