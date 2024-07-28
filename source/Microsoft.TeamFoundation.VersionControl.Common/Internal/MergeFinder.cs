// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Common.Internal.MergeFinder
// Assembly: Microsoft.TeamFoundation.VersionControl.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 156CCB01-0A1F-468C-A332-06DB9F9B179E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Common.dll

using Microsoft.TeamFoundation.Diff;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Microsoft.TeamFoundation.VersionControl.Common.Internal
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public static class MergeFinder
  {
    public static IMergeChange[] Merge(
      IList<DiffLine> modified,
      IList<DiffLine> latest,
      IEqualityComparer<DiffLine> comparer,
      MergeOptions mergeOptions)
    {
      List<IMergeChange> mergeChangeList = new List<IMergeChange>();
      IDiffChange[] diffChangeArray;
      using (DiffFinder<DiffLine> lcsDiff = DiffFinder<DiffLine>.LcsDiff)
        diffChangeArray = lcsDiff.Diff(modified, latest, comparer);
      foreach (IDiffChange diffChange in diffChangeArray)
      {
        MergeChange mergeChange = new MergeChange(MergeChangeType.Conflict, diffChange, diffChange);
        mergeChangeList.Add((IMergeChange) mergeChange);
      }
      return mergeChangeList.ToArray();
    }

    public static IMergeChange[] Merge(
      IList<DiffLine> original,
      IList<DiffLine> modified,
      IList<DiffLine> latest,
      DiffLineComparer comparer,
      MergeOptions mergeOptions)
    {
      IDiffChange[] originalModified;
      IDiffChange[] originalLatest;
      using (DiffFinder<DiffLine> lcsDiff = DiffFinder<DiffLine>.LcsDiff)
      {
        originalModified = lcsDiff.Diff(original, modified, (IEqualityComparer<DiffLine>) comparer);
        originalLatest = lcsDiff.Diff(original, latest, (IEqualityComparer<DiffLine>) comparer);
      }
      return MergeFinder.JoinDiffs(originalModified, originalLatest, original, modified, latest, comparer, mergeOptions);
    }

    private static IMergeChange[] JoinDiffs(
      IDiffChange[] originalModified,
      IDiffChange[] originalLatest,
      IList<DiffLine> original,
      IList<DiffLine> modified,
      IList<DiffLine> latest,
      DiffLineComparer comparer,
      MergeOptions mergeOptions)
    {
      List<IMergeChange> mergeChangeList = new List<IMergeChange>();
      int index1 = 0;
      int index2 = 0;
      MergeChange mergeChange = (MergeChange) null;
      while (index1 < originalModified.Length && index2 < originalLatest.Length)
      {
        IDiffChange diffChange1 = originalModified[index1];
        IDiffChange diffChange2 = originalLatest[index2];
        int num1 = Math.Max(diffChange1.OriginalStart + diffChange1.OriginalLength - 1, diffChange1.OriginalStart);
        int num2 = Math.Max(diffChange2.OriginalStart + diffChange2.OriginalLength - 1, diffChange2.OriginalStart);
        if (num1 < diffChange2.OriginalStart && (!mergeOptions.AdjacentChangesConflict || diffChange1.ChangeType == DiffChangeType.Insert || num1 < diffChange2.OriginalStart - 1))
        {
          if (mergeChange == null)
            mergeChange = new MergeChange(MergeChangeType.Modified);
          mergeChange.AddModifiedChange(diffChange1);
          mergeChangeList.Add((IMergeChange) mergeChange);
          mergeChange = (MergeChange) null;
          ++index1;
        }
        else if (num2 < diffChange1.OriginalStart && (!mergeOptions.AdjacentChangesConflict || diffChange2.ChangeType == DiffChangeType.Insert || num2 < diffChange1.OriginalStart - 1))
        {
          if (mergeChange == null)
            mergeChange = new MergeChange(MergeChangeType.Latest);
          mergeChange.AddLatestChange(diffChange2);
          mergeChangeList.Add((IMergeChange) mergeChange);
          mergeChange = (MergeChange) null;
          ++index2;
        }
        else if (MergeFinder.ChangesIdentical(modified, diffChange1, latest, diffChange2, comparer))
        {
          mergeChangeList.Add((IMergeChange) new MergeChange(MergeChangeType.Both, diffChange1, diffChange2));
          ++index1;
          ++index2;
        }
        else
        {
          if (mergeChange == null)
            mergeChange = new MergeChange(MergeChangeType.Conflict);
          if (num1 < num2)
          {
            mergeChange.AddModifiedChange(diffChange1);
            ++index1;
          }
          else
          {
            mergeChange.AddLatestChange(diffChange2);
            ++index2;
          }
        }
      }
      if (index1 < originalModified.Length)
      {
        for (int index3 = index1; index3 < originalModified.Length; ++index3)
        {
          if (mergeChange != null && (mergeChange.LatestChange.OriginalEnd >= originalModified[index3].OriginalStart && mergeOptions.AdjacentChangesConflict || mergeChange.LatestChange.OriginalEnd > originalModified[index3].OriginalStart))
          {
            mergeChange.AddModifiedChange(originalModified[index3]);
          }
          else
          {
            if (mergeChange != null)
            {
              mergeChangeList.Add((IMergeChange) mergeChange);
              mergeChange = (MergeChange) null;
            }
            mergeChangeList.Add((IMergeChange) new MergeChange(MergeChangeType.Modified, originalModified[index3], (IDiffChange) null));
          }
        }
      }
      else if (index2 < originalLatest.Length)
      {
        for (int index4 = index2; index4 < originalLatest.Length; ++index4)
        {
          if (mergeChange != null && (mergeChange.ModifiedChange.OriginalEnd >= originalLatest[index4].OriginalStart && mergeOptions.AdjacentChangesConflict || mergeChange.ModifiedChange.OriginalEnd > originalLatest[index4].OriginalStart))
          {
            mergeChange.AddLatestChange(originalLatest[index4]);
          }
          else
          {
            if (mergeChange != null)
            {
              mergeChangeList.Add((IMergeChange) mergeChange);
              mergeChange = (MergeChange) null;
            }
            mergeChangeList.Add((IMergeChange) new MergeChange(MergeChangeType.Latest, (IDiffChange) null, originalLatest[index4]));
          }
        }
      }
      if (mergeChange != null)
        mergeChangeList.Add((IMergeChange) mergeChange);
      return mergeChangeList.ToArray();
    }

    public static IMergeChange[] MergeEx(
      IList<DiffLine> diffFileOriginal,
      IList<DiffLine> diffFileModified,
      IList<DiffLine> diffFileLatest,
      DiffLineComparer comparer1,
      DiffLineComparer comparer2,
      MergeOptions mergeOptions1,
      MergeOptions mergeOptions2)
    {
      IDiffChange[] originalModified;
      IDiffChange[] originalLatest;
      using (DiffFinder<DiffLine> lcsDiff = DiffFinder<DiffLine>.LcsDiff)
      {
        originalModified = lcsDiff.DiffEx(diffFileOriginal, diffFileModified, (IEqualityComparer<DiffLine>) comparer1, (IEqualityComparer<DiffLine>) comparer2);
        originalLatest = lcsDiff.DiffEx(diffFileOriginal, diffFileLatest, (IEqualityComparer<DiffLine>) comparer1, (IEqualityComparer<DiffLine>) comparer2);
      }
      return MergeFinder.JoinDiffs(originalModified, originalLatest, diffFileOriginal, diffFileModified, diffFileLatest, comparer2, mergeOptions2);
    }

    private static bool ChangesIdentical(
      IList<DiffLine> leftSequence,
      IDiffChange leftChange,
      IList<DiffLine> rightSequence,
      IDiffChange rightChange,
      DiffLineComparer comparer)
    {
      if (leftChange.OriginalStart != rightChange.OriginalStart || leftChange.OriginalLength != rightChange.OriginalLength || leftChange.ModifiedLength != rightChange.ModifiedLength)
        return false;
      int modifiedStart1 = leftChange.ModifiedStart;
      int modifiedStart2 = rightChange.ModifiedStart;
      while (modifiedStart1 < leftChange.ModifiedStart + leftChange.ModifiedLength)
      {
        if (!comparer.Equals(leftSequence[modifiedStart1], rightSequence[modifiedStart2]))
          return false;
        ++modifiedStart1;
        ++modifiedStart2;
      }
      return true;
    }

    public static MergeSummary GetSummary(IMergeChange[] mergedChanges)
    {
      MergeSummary summary = new MergeSummary();
      foreach (IMergeChange mergedChange in mergedChanges)
      {
        switch (mergedChange.ChangeType)
        {
          case MergeChangeType.Modified:
            ++summary.TotalModified;
            break;
          case MergeChangeType.Latest:
            ++summary.TotalLatest;
            break;
          case MergeChangeType.Both:
            ++summary.TotalBoth;
            break;
          case MergeChangeType.Conflict:
            ++summary.TotalConflicting;
            break;
        }
      }
      return summary;
    }
  }
}
