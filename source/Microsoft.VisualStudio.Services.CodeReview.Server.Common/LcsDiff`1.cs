// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.CodeReview.Server.Common.LcsDiff`1
// Assembly: Microsoft.VisualStudio.Services.CodeReview.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F16CDF2D-8103-4EAE-A2A8-4FA5B1C1BE58
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.CodeReview.Server.Common.dll

using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.CodeReview.Server.Common
{
  internal class LcsDiff<T> : DiffFinder<T>
  {
    private List<int[]> m_forwardHistory;
    private List<int[]> m_reverseHistory;
    private const int MaxDifferencesHistory = 1447;

    public LcsDiff()
    {
      this.m_forwardHistory = new List<int[]>(1447);
      this.m_reverseHistory = new List<int[]>(1447);
    }

    public override void Dispose()
    {
      base.Dispose();
      if (this.m_forwardHistory != null)
        this.m_forwardHistory = (List<int[]>) null;
      if (this.m_reverseHistory != null)
        this.m_reverseHistory = (List<int[]>) null;
      GC.SuppressFinalize((object) this);
    }

    protected override IDiffChange[] ComputeDiff(
      int originalStart,
      int originalEnd,
      int modifiedStart,
      int modifiedEnd)
    {
      return this.ShiftChanges(this.ComputeDiffRecursive(originalStart, originalEnd, modifiedStart, modifiedEnd, out bool _));
    }

    private IDiffChange[] ComputeDiffRecursive(
      int originalStart,
      int originalEnd,
      int modifiedStart,
      int modifiedEnd,
      out bool quitEarly)
    {
      quitEarly = false;
      for (; originalStart <= originalEnd && modifiedStart <= modifiedEnd && this.ElementsAreEqual(originalStart, modifiedStart); ++modifiedStart)
        ++originalStart;
      for (; originalEnd >= originalStart && modifiedEnd >= modifiedStart && this.ElementsAreEqual(originalEnd, modifiedEnd); --modifiedEnd)
        --originalEnd;
      if (originalStart > originalEnd || modifiedStart > modifiedEnd)
      {
        IDiffChange[] diffRecursive;
        if (modifiedStart <= modifiedEnd)
          diffRecursive = new IDiffChange[1]
          {
            (IDiffChange) new DiffChange(originalStart, 0, modifiedStart, modifiedEnd - modifiedStart + 1)
          };
        else if (originalStart <= originalEnd)
          diffRecursive = new IDiffChange[1]
          {
            (IDiffChange) new DiffChange(originalStart, originalEnd - originalStart + 1, modifiedStart, 0)
          };
        else
          diffRecursive = Array.Empty<IDiffChange>();
        return diffRecursive;
      }
      int midOriginal;
      int midModified;
      IDiffChange[] recursionPoint = this.ComputeRecursionPoint(originalStart, originalEnd, modifiedStart, modifiedEnd, out midOriginal, out midModified, out quitEarly);
      if (recursionPoint != null)
        return recursionPoint;
      if (!quitEarly)
      {
        IDiffChange[] diffRecursive = this.ComputeDiffRecursive(originalStart, midOriginal, modifiedStart, midModified, out quitEarly);
        Array.Empty<IDiffChange>();
        IDiffChange[] right;
        if (!quitEarly)
          right = this.ComputeDiffRecursive(midOriginal + 1, originalEnd, midModified + 1, modifiedEnd, out quitEarly);
        else
          right = (IDiffChange[]) new DiffChange[1]
          {
            new DiffChange(midOriginal + 1, originalEnd - (midOriginal + 1) + 1, midModified + 1, modifiedEnd - (midModified + 1) + 1)
          };
        return this.ConcatenateChanges(diffRecursive, right);
      }
      return (IDiffChange[]) new DiffChange[1]
      {
        new DiffChange(originalStart, originalEnd - originalStart + 1, modifiedStart, modifiedEnd - modifiedStart + 1)
      };
    }

    private IDiffChange[] ComputeRecursionPoint(
      int originalStart,
      int originalEnd,
      int modifiedStart,
      int modifiedEnd,
      out int midOriginal,
      out int midModified,
      out bool quitEarly)
    {
      int sourceIndex1 = 0;
      int num1 = 0;
      int sourceIndex2 = 0;
      int num2 = 0;
      --originalStart;
      --modifiedStart;
      midOriginal = 0;
      midModified = 0;
      this.m_forwardHistory.Clear();
      this.m_reverseHistory.Clear();
      int num3 = originalEnd - originalStart + (modifiedEnd - modifiedStart);
      int numDiagonals = num3 + 1;
      int[] sourceArray1 = new int[numDiagonals];
      int[] sourceArray2 = new int[numDiagonals];
      int diagonalBaseIndex1 = modifiedEnd - modifiedStart;
      int diagonalBaseIndex2 = originalEnd - originalStart;
      int num4 = originalStart - modifiedStart;
      int num5 = originalEnd - modifiedEnd;
      bool flag = (diagonalBaseIndex2 - diagonalBaseIndex1) % 2 == 0;
      sourceArray1[diagonalBaseIndex1] = originalStart;
      sourceArray2[diagonalBaseIndex2] = originalEnd;
      quitEarly = false;
      for (int numDifferences = 1; numDifferences <= num3 / 2 + 1; ++numDifferences)
      {
        int originalIndex1 = 0;
        int num6 = 0;
        sourceIndex1 = this.ClipDiagonalBound(diagonalBaseIndex1 - numDifferences, numDifferences, diagonalBaseIndex1, numDiagonals);
        num1 = this.ClipDiagonalBound(diagonalBaseIndex1 + numDifferences, numDifferences, diagonalBaseIndex1, numDiagonals);
        for (int index = sourceIndex1; index <= num1; index += 2)
        {
          int num7 = index == sourceIndex1 || index < num1 && sourceArray1[index - 1] < sourceArray1[index + 1] ? sourceArray1[index + 1] : sourceArray1[index - 1] + 1;
          int num8 = num7 - (index - diagonalBaseIndex1) - num4;
          int num9 = num7;
          for (; num7 < originalEnd && num8 < modifiedEnd && this.ElementsAreEqualWithoutBoudaryCheck(num7 + 1, num8 + 1); ++num8)
            ++num7;
          sourceArray1[index] = num7;
          if (num7 + num8 > originalIndex1 + num6)
          {
            originalIndex1 = num7;
            num6 = num8;
          }
          if (!flag && Math.Abs(index - diagonalBaseIndex2) <= numDifferences - 1 && num7 >= sourceArray2[index])
          {
            midOriginal = num7;
            midModified = num8;
            if (num9 > sourceArray2[index] || numDifferences > 1448)
              return (IDiffChange[]) null;
            goto label_28;
          }
        }
        int longestMatchSoFar = (originalIndex1 - originalStart + (num6 - modifiedStart) - numDifferences) / 2;
        if (this.ContinueDifferencePredicate != null && !this.ContinueDifferencePredicate(originalIndex1, this.OriginalSequence, longestMatchSoFar))
        {
          quitEarly = true;
          midOriginal = originalIndex1;
          midModified = num6;
          if (longestMatchSoFar <= 0 || numDifferences > 1448)
          {
            ++originalStart;
            ++modifiedStart;
            return (IDiffChange[]) new DiffChange[1]
            {
              new DiffChange(originalStart, originalEnd - originalStart + 1, modifiedStart, modifiedEnd - modifiedStart + 1)
            };
          }
          break;
        }
        sourceIndex2 = this.ClipDiagonalBound(diagonalBaseIndex2 - numDifferences, numDifferences, diagonalBaseIndex2, numDiagonals);
        num2 = this.ClipDiagonalBound(diagonalBaseIndex2 + numDifferences, numDifferences, diagonalBaseIndex2, numDiagonals);
        for (int index = sourceIndex2; index <= num2; index += 2)
        {
          int originalIndex2 = index == sourceIndex2 || index < num2 && sourceArray2[index - 1] >= sourceArray2[index + 1] ? sourceArray2[index + 1] - 1 : sourceArray2[index - 1];
          int modifiedIndex = originalIndex2 - (index - diagonalBaseIndex2) - num5;
          int num10 = originalIndex2;
          for (; originalIndex2 > originalStart && modifiedIndex > modifiedStart && this.ElementsAreEqualWithoutBoudaryCheck(originalIndex2, modifiedIndex); --modifiedIndex)
            --originalIndex2;
          sourceArray2[index] = originalIndex2;
          if (flag && Math.Abs(index - diagonalBaseIndex1) <= numDifferences && originalIndex2 <= sourceArray1[index])
          {
            midOriginal = originalIndex2;
            midModified = modifiedIndex;
            if (num10 < sourceArray1[index] || numDifferences > 1448)
              return (IDiffChange[]) null;
            goto label_28;
          }
        }
        if (numDifferences <= 1447)
        {
          int[] destinationArray1 = new int[num1 - sourceIndex1 + 2];
          destinationArray1[0] = diagonalBaseIndex1 - sourceIndex1 + 1;
          Array.Copy((Array) sourceArray1, sourceIndex1, (Array) destinationArray1, 1, num1 - sourceIndex1 + 1);
          this.m_forwardHistory.Add(destinationArray1);
          int[] destinationArray2 = new int[num2 - sourceIndex2 + 2];
          destinationArray2[0] = diagonalBaseIndex2 - sourceIndex2 + 1;
          Array.Copy((Array) sourceArray2, sourceIndex2, (Array) destinationArray2, 1, num2 - sourceIndex2 + 1);
          this.m_reverseHistory.Add(destinationArray2);
        }
      }
label_28:
      IDiffChange[] reverseChanges;
      using (DiffChangeHelper diffChangeHelper = new DiffChangeHelper())
      {
        int num11 = sourceIndex1;
        int num12 = num1;
        int num13 = midOriginal - midModified - num4;
        int num14 = int.MinValue;
        int index = this.m_forwardHistory.Count - 1;
        do
        {
          int num15 = num13 + diagonalBaseIndex1;
          if (num15 == num11 || num15 < num12 && sourceArray1[num15 - 1] < sourceArray1[num15 + 1])
          {
            int num16 = sourceArray1[num15 + 1];
            int modifiedIndex = num16 - num13 - num4;
            if (num16 < num14)
              diffChangeHelper.MarkNextChange();
            num14 = num16;
            diffChangeHelper.AddModifiedElement(num16 + 1, modifiedIndex);
            num13 = num15 + 1 - diagonalBaseIndex1;
          }
          else
          {
            int originalIndex = sourceArray1[num15 - 1] + 1;
            int num17 = originalIndex - num13 - num4;
            if (originalIndex < num14)
              diffChangeHelper.MarkNextChange();
            num14 = originalIndex - 1;
            diffChangeHelper.AddOriginalElement(originalIndex, num17 + 1);
            num13 = num15 - 1 - diagonalBaseIndex1;
          }
          if (index >= 0)
          {
            sourceArray1 = this.m_forwardHistory[index];
            diagonalBaseIndex1 = sourceArray1[0];
            num11 = 1;
            num12 = sourceArray1.Length - 1;
          }
        }
        while (--index >= -1);
        reverseChanges = diffChangeHelper.ReverseChanges;
      }
      IDiffChange[] right;
      if (quitEarly)
      {
        int num18 = midOriginal + 1;
        int num19 = midModified + 1;
        if (reverseChanges != null && reverseChanges.Length != 0)
        {
          IDiffChange diffChange = reverseChanges[reverseChanges.Length - 1];
          num18 = Math.Max(num18, diffChange.OriginalEnd);
          num19 = Math.Max(num19, diffChange.ModifiedEnd);
        }
        right = (IDiffChange[]) new DiffChange[1]
        {
          new DiffChange(num18, originalEnd - num18 + 1, num19, modifiedEnd - num19 + 1)
        };
      }
      else
      {
        using (DiffChangeHelper diffChangeHelper = new DiffChangeHelper())
        {
          int num20 = sourceIndex2;
          int num21 = num2;
          int num22 = midOriginal - midModified - num5;
          int num23 = int.MaxValue;
          int index = flag ? this.m_reverseHistory.Count - 1 : this.m_reverseHistory.Count - 2;
          do
          {
            int num24 = num22 + diagonalBaseIndex2;
            if (num24 == num20 || num24 < num21 && sourceArray2[num24 - 1] >= sourceArray2[num24 + 1])
            {
              int num25 = sourceArray2[num24 + 1] - 1;
              int num26 = num25 - num22 - num5;
              if (num25 > num23)
                diffChangeHelper.MarkNextChange();
              num23 = num25 + 1;
              diffChangeHelper.AddOriginalElement(num25 + 1, num26 + 1);
              num22 = num24 + 1 - diagonalBaseIndex2;
            }
            else
            {
              int num27 = sourceArray2[num24 - 1];
              int num28 = num27 - num22 - num5;
              if (num27 > num23)
                diffChangeHelper.MarkNextChange();
              num23 = num27;
              diffChangeHelper.AddModifiedElement(num27 + 1, num28 + 1);
              num22 = num24 - 1 - diagonalBaseIndex2;
            }
            if (index >= 0)
            {
              sourceArray2 = this.m_reverseHistory[index];
              diagonalBaseIndex2 = sourceArray2[0];
              num20 = 1;
              num21 = sourceArray2.Length - 1;
            }
          }
          while (--index >= -1);
          right = diffChangeHelper.Changes;
        }
      }
      return this.ConcatenateChanges(reverseChanges, right);
    }

    private IDiffChange[] ShiftChanges(IDiffChange[] changes)
    {
      for (int index = 0; index < changes.Length; ++index)
      {
        DiffChange change = changes[index] as DiffChange;
        int num1 = index < changes.Length - 1 ? changes[index + 1].OriginalStart : this.OriginalSequence.Count;
        int num2 = index < changes.Length - 1 ? changes[index + 1].ModifiedStart : this.ModifiedSequence.Count;
        bool flag1 = change.OriginalLength > 0;
        for (bool flag2 = change.ModifiedLength > 0; change.OriginalStart + change.OriginalLength < num1 && change.ModifiedStart + change.ModifiedLength < num2 && (!flag1 || this.OriginalElementsAreEqual(change.OriginalStart, change.OriginalStart + change.OriginalLength)) && (!flag2 || this.ModifiedElementsAreEqual(change.ModifiedStart, change.ModifiedStart + change.ModifiedLength)); ++change.ModifiedStart)
          ++change.OriginalStart;
      }
      List<IDiffChange> diffChangeList = new List<IDiffChange>(changes.Length);
      for (int index = 0; index < changes.Length; ++index)
      {
        IDiffChange mergedChange;
        if (index < changes.Length - 1 && this.ChangesOverlap(changes[index], changes[index + 1], out mergedChange))
        {
          diffChangeList.Add(mergedChange);
          ++index;
        }
        else
          diffChangeList.Add(changes[index]);
      }
      return diffChangeList.ToArray();
    }

    private IDiffChange[] ConcatenateChanges(IDiffChange[] left, IDiffChange[] right)
    {
      if (left.Length == 0 || right.Length == 0)
        return right.Length == 0 ? left : right;
      IDiffChange mergedChange;
      if (this.ChangesOverlap(left[left.Length - 1], right[0], out mergedChange))
      {
        IDiffChange[] destinationArray = new IDiffChange[left.Length + right.Length - 1];
        Array.Copy((Array) left, 0, (Array) destinationArray, 0, left.Length - 1);
        destinationArray[left.Length - 1] = mergedChange;
        Array.Copy((Array) right, 1, (Array) destinationArray, left.Length, right.Length - 1);
        return destinationArray;
      }
      IDiffChange[] destinationArray1 = new IDiffChange[left.Length + right.Length];
      Array.Copy((Array) left, 0, (Array) destinationArray1, 0, left.Length);
      Array.Copy((Array) right, 0, (Array) destinationArray1, left.Length, right.Length);
      return destinationArray1;
    }

    private bool ChangesOverlap(IDiffChange left, IDiffChange right, out IDiffChange mergedChange)
    {
      if (left.OriginalStart + left.OriginalLength >= right.OriginalStart || left.ModifiedStart + left.ModifiedLength >= right.ModifiedStart)
      {
        int originalStart = left.OriginalStart;
        int originalLength = left.OriginalLength;
        int modifiedStart = left.ModifiedStart;
        int modifiedLength = left.ModifiedLength;
        if (left.OriginalStart + left.OriginalLength >= right.OriginalStart)
          originalLength = right.OriginalStart + right.OriginalLength - left.OriginalStart;
        if (left.ModifiedStart + left.ModifiedLength >= right.ModifiedStart)
          modifiedLength = right.ModifiedStart + right.ModifiedLength - left.ModifiedStart;
        mergedChange = (IDiffChange) new DiffChange(originalStart, originalLength, modifiedStart, modifiedLength);
        return true;
      }
      mergedChange = (IDiffChange) null;
      return false;
    }

    private int ClipDiagonalBound(
      int diagonal,
      int numDifferences,
      int diagonalBaseIndex,
      int numDiagonals)
    {
      if (diagonal >= 0 && diagonal < numDiagonals)
        return diagonal;
      int num1 = diagonalBaseIndex;
      int num2 = numDiagonals - diagonalBaseIndex - 1;
      bool flag1 = numDifferences % 2 == 0;
      if (diagonal < 0)
      {
        bool flag2 = num1 % 2 == 0;
        return flag1 != flag2 ? 1 : 0;
      }
      bool flag3 = num2 % 2 == 0;
      return flag1 != flag3 ? numDiagonals - 2 : numDiagonals - 1;
    }
  }
}
