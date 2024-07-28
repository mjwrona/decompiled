// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.PackIndex.PackIndexMerger
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Diagnostics;

namespace Microsoft.TeamFoundation.Git.Server.PackIndex
{
  internal class PackIndexMerger
  {
    private readonly IVssRequestContext m_rc;
    private readonly ConcatGitPackIndex m_eligibleUnmerged;
    private readonly PackIndexSizeEstimator m_sizeEstimator;
    private int m_iFirstToMerge;
    private long m_nextMergedSize;
    private long m_nextMergedPackCount;
    private long m_maxSize;
    private long m_maxPackCount;
    private const long c_defaultMaxSize = 4085252096;
    private const long c_defaultMaxPackCount = 64536;

    public PackIndexMerger(
      IVssRequestContext rc,
      ConcatGitPackIndex unmerged,
      PackIndexSizeEstimator sizeEstimator)
    {
      this.m_rc = rc;
      this.m_maxSize = 4085252096L;
      this.m_maxPackCount = 4085252096L;
      IVssRegistryService service = rc.GetService<IVssRegistryService>();
      this.m_maxSize = service.GetValue<long>(rc, (RegistryQuery) "/Service/Git/Configuration/PackIndex/MaxSize", true, 4085252096L);
      this.m_maxPackCount = service.GetValue<long>(rc, (RegistryQuery) "/Service/Git/Configuration/PackIndex/MaxPackCount", true, 64536L);
      this.m_sizeEstimator = sizeEstimator;
      this.m_eligibleUnmerged = unmerged;
      this.LockedBases = 0;
      long num1 = 0;
      long num2 = 0;
      for (int index = unmerged.Subindexes.Count - 1; index >= 0; --index)
      {
        num1 += this.m_sizeEstimator(unmerged.Subindexes[index]);
        num2 += (long) unmerged.Subindexes[index].PackIds.Count;
        if (num1 >= this.m_maxSize || num2 >= this.m_maxPackCount)
        {
          this.LockedBases = index + 1;
          break;
        }
      }
      this.m_iFirstToMerge = this.m_eligibleUnmerged.Subindexes.Count + 1;
      this.MergeNextIfRemainingUnderHardLimits();
      this.MergeNextIfRemainingUnderHardLimits();
    }

    public int CurrentSubIndexCount => this.m_iFirstToMerge + 1;

    public void MergeUntilSize(long targetSize)
    {
      do
        ;
      while (this.m_nextMergedSize <= targetSize && this.MergeNextIfRemainingUnderHardLimits());
    }

    public bool MergeNextIfRemainingUnderHardLimits()
    {
      if (this.m_iFirstToMerge == 0)
        return false;
      if (this.m_nextMergedSize >= this.m_maxSize || this.m_nextMergedPackCount >= this.m_maxPackCount)
      {
        this.m_rc.TraceAlways(1013885, TraceLevel.Verbose, "PackIndexMergeStrategy", nameof (MergeNextIfRemainingUnderHardLimits), new
        {
          FirstToMerge = this.m_iFirstToMerge,
          MergedSize = this.m_nextMergedSize,
          MergedPackCount = this.m_nextMergedPackCount,
          MaxSize = this.m_maxSize,
          MaxPackCount = this.m_maxPackCount
        }.ToString());
        return false;
      }
      --this.m_iFirstToMerge;
      if (this.m_iFirstToMerge != 0)
      {
        this.m_nextMergedSize += this.m_sizeEstimator(this.m_eligibleUnmerged.Subindexes[this.m_iFirstToMerge - 1]);
        this.m_nextMergedPackCount += (long) this.m_eligibleUnmerged.Subindexes[this.m_iFirstToMerge - 1].PackIds.Count;
      }
      return true;
    }

    public ConcatGitPackIndex GetCurrentMergedIndex() => this.m_eligibleUnmerged.GetRange(this.m_iFirstToMerge, this.m_eligibleUnmerged.Subindexes.Count - this.m_iFirstToMerge);

    public int LockedBases { get; private set; }
  }
}
