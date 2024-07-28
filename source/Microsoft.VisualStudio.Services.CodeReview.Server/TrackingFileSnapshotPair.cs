// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.CodeReview.Server.TrackingFileSnapshotPair
// Assembly: Microsoft.VisualStudio.Services.CodeReview.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2BCB2866-BDCB-4FDE-9EA3-48DFA660C131
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.CodeReview.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.VisualStudio.Services.CodeReview.Server.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.CodeReview.Server
{
  internal class TrackingFileSnapshotPair : IEquatable<TrackingFileSnapshotPair>
  {
    public TrackingFileSnapshot First;
    public TrackingFileSnapshot Second;
    public TrackingContext TrackingContext;
    private List<IDiffChange> DiffData;
    private int DiffCount;
    private bool DiffDataWasCached;

    public TrackingFileSnapshotPair(
      TrackingFileSnapshot first,
      TrackingFileSnapshot second,
      TrackingContext trackingContext = null)
    {
      this.First = first;
      this.Second = second;
      this.DiffCount = 0;
      this.DiffData = (List<IDiffChange>) null;
      this.DiffDataWasCached = false;
      this.TrackingContext = trackingContext;
    }

    public void DiffFiles()
    {
      using (DiffFinder<DiffLine> lcsDiff = DiffFinder<DiffLine>.LcsDiff)
      {
        DiffFile fileData1 = this.First.GetFileData();
        DiffFile fileData2 = this.Second.GetFileData();
        if (fileData1 == null || fileData2 == null)
          return;
        DiffOptions diffOptions = this.First.GetDiffOptions();
        diffOptions.SourceEncoding = this.First.GetFileEncoding();
        diffOptions.TargetEncoding = this.Second.GetFileEncoding();
        DiffLineComparer elementComparer = new DiffLineComparer(diffOptions);
        List<IDiffChange> list = ((IEnumerable<IDiffChange>) lcsDiff.Diff((IList<DiffLine>) fileData1, (IList<DiffLine>) fileData2, (IEqualityComparer<DiffLine>) elementComparer)).ToList<IDiffChange>();
        if (list.IsNullOrEmpty<IDiffChange>())
          return;
        this.DiffData = list;
      }
    }

    public List<IDiffChange> GetDiffData()
    {
      if (this.DiffData == null)
        return (List<IDiffChange>) null;
      List<IDiffChange> diffData = new List<IDiffChange>((IEnumerable<IDiffChange>) this.DiffData);
      if (--this.DiffCount != 0)
        return diffData;
      this.DiffData = (List<IDiffChange>) null;
      return diffData;
    }

    public void SetDiffData(List<IDiffChange> changes)
    {
      if (changes.IsNullOrEmpty<IDiffChange>())
        return;
      this.DiffData = changes;
    }

    public void PopulateCache(DiffCacheObject cache)
    {
      if (!this.HasDiffData() || this.HasCachedDiffData())
        return;
      string key = this.ToString();
      if (key == null)
        return;
      foreach (IDiffChange diffChange in this.DiffData)
        cache.AddEntry(key, new DiffCacheEntry()
        {
          ChangeType = diffChange.ChangeType,
          OrigStartLine = diffChange.OriginalStart,
          OrigStartOffset = 0,
          OrigEndLine = diffChange.OriginalEnd,
          OrigEndOffset = 0,
          ModStartLine = diffChange.ModifiedStart,
          ModStartOffset = 0,
          ModEndLine = diffChange.ModifiedEnd,
          ModEndOffset = 0
        });
    }

    public void PopulateDiffDataFromCache(List<DiffCacheEntry> cacheEntries)
    {
      if (cacheEntries.IsNullOrEmpty<DiffCacheEntry>())
        return;
      List<IDiffChange> diffChangeList = new List<IDiffChange>();
      foreach (DiffCacheEntry cacheEntry in cacheEntries)
      {
        int origStartLine = cacheEntry.OrigStartLine;
        int modStartLine = cacheEntry.ModStartLine;
        int originalLength = 0;
        int modifiedLength = 0;
        if (cacheEntry.ChangeType == DiffChangeType.Change || cacheEntry.ChangeType == DiffChangeType.Delete)
          originalLength = cacheEntry.OrigEndLine - cacheEntry.OrigStartLine;
        if (cacheEntry.ChangeType == DiffChangeType.Change || cacheEntry.ChangeType == DiffChangeType.Insert)
          modifiedLength = cacheEntry.ModEndLine - cacheEntry.ModStartLine;
        diffChangeList.Add((IDiffChange) new DiffChange(origStartLine, originalLength, modStartLine, modifiedLength));
      }
      this.DiffData = diffChangeList;
      this.DiffDataWasCached = true;
    }

    public void IncrementDiffCount() => ++this.DiffCount;

    public void DecrementDiffCount() => --this.DiffCount;

    public int GetDiffCount() => this.DiffCount;

    public bool HasDiffData() => this.DiffData != null;

    public bool HasEmptyDiffData() => this.HasDiffData() && this.DiffData.Count == 0;

    public bool HasCachedDiffData() => this.HasDiffData() && this.DiffDataWasCached;

    public bool IsPairValid() => this.First.IsValid() && this.Second.IsValid() && this.DiffCount > 0;

    private bool IsPairDuplicate() => this.First.FileHash == this.Second.FileHash;

    public bool ClearIfInvalid()
    {
      bool flag1 = this.IsPairValid();
      bool flag2 = this.IsPairDuplicate();
      if (!flag1)
      {
        this.DiffCount = 0;
        this.DiffData = (List<IDiffChange>) null;
        this.DiffDataWasCached = false;
      }
      if (flag2)
      {
        this.DiffData = new List<IDiffChange>(0);
        this.DiffDataWasCached = true;
      }
      if (!(!flag1 | flag2))
        return false;
      this.First.DecrementFileCount();
      this.First.ClearIfInvalid();
      this.Second.DecrementFileCount();
      this.Second.ClearIfInvalid();
      return !flag1;
    }

    public object FormatCI() => (object) new
    {
      First = this.First.FormatCI(),
      Second = this.Second.FormatCI(),
      TrackingContext = this.TrackingContext?.FormatCI(),
      DiffCount = this.DiffCount,
      DiffData = this.HasDiffData(),
      DiffDataEmpty = this.HasEmptyDiffData(),
      DiffCached = this.HasCachedDiffData(),
      IsValid = this.IsPairValid(),
      IsDuplicate = this.IsPairDuplicate()
    };

    public override int GetHashCode() => this.First.GetHashCode() ^ this.Second.GetHashCode();

    public bool Equals(TrackingFileSnapshotPair other)
    {
      if (other == null)
        return false;
      if (other.First.Equals(this.First) && other.Second.Equals(this.Second))
        return true;
      return other.Second.Equals(this.First) && other.First.Equals(this.Second);
    }

    public override string ToString() => !string.IsNullOrEmpty(this.First.FileHash) && !string.IsNullOrEmpty(this.Second.FileHash) ? this.First.FileHash + ":" + this.Second.FileHash : (string) null;
  }
}
