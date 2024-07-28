// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.PackIndex.FastPackIndexMergeStrategy
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Framework.Server;

namespace Microsoft.TeamFoundation.Git.Server.PackIndex
{
  internal class FastPackIndexMergeStrategy : IPackIndexMergeStrategy
  {
    private readonly long m_targetTipIndexSize;

    public FastPackIndexMergeStrategy(long targetTipIndexSize = 65536) => this.m_targetTipIndexSize = targetTipIndexSize;

    public ConcatGitPackIndex Merge(
      IVssRequestContext rc,
      ConcatGitPackIndex unmerged,
      PackIndexSizeEstimator sizeEstimator)
    {
      PackIndexMerger packIndexMerger = new PackIndexMerger(rc, unmerged, sizeEstimator);
      packIndexMerger.MergeUntilSize(this.m_targetTipIndexSize);
      return packIndexMerger.GetCurrentMergedIndex();
    }
  }
}
