// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.CodeReview.Server.Common.DiffFinder`1
// Assembly: Microsoft.VisualStudio.Services.CodeReview.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F16CDF2D-8103-4EAE-A2A8-4FA5B1C1BE58
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.CodeReview.Server.Common.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.CodeReview.Server.Common
{
  public abstract class DiffFinder<T> : IDisposable
  {
    private IList<T> m_original;
    private IList<T> m_modified;
    private int[] m_originalIds;
    private int[] m_modifiedIds;
    private IEqualityComparer<T> m_elementComparer;
    private Microsoft.VisualStudio.Services.CodeReview.Server.Common.ContinueDifferencePredicate<T> m_predicate;

    protected IList<T> OriginalSequence => this.m_original;

    protected IList<T> ModifiedSequence => this.m_modified;

    protected IEqualityComparer<T> ElementComparer => this.m_elementComparer;

    public virtual void Dispose()
    {
      if (this.m_originalIds != null)
        this.m_originalIds = (int[]) null;
      if (this.m_modifiedIds != null)
        this.m_modifiedIds = (int[]) null;
      GC.SuppressFinalize((object) this);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected bool ElementsAreEqual(int originalIndex, int modifiedIndex)
    {
      int originalId = this.m_originalIds[originalIndex];
      int modifiedId = this.m_modifiedIds[modifiedIndex];
      return originalId != 0 && modifiedId != 0 ? originalId == modifiedId : this.ElementComparer.Equals(this.OriginalSequence[originalIndex], this.ModifiedSequence[modifiedIndex]);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected bool ElementsAreEqualWithoutBoudaryCheck(int originalIndex, int modifiedIndex) => this.m_originalIds[originalIndex] == this.m_modifiedIds[modifiedIndex];

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected bool OriginalElementsAreEqual(int firstIndex, int secondIndex) => this.ElementsAreEqual(firstIndex, true, secondIndex, true);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected bool ModifiedElementsAreEqual(int firstIndex, int secondIndex) => this.ElementsAreEqual(firstIndex, false, secondIndex, false);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private bool ElementsAreEqual(
      int firstIndex,
      bool firstIsOriginal,
      int secondIndex,
      bool secondIsOriginal)
    {
      int num1 = firstIsOriginal ? this.m_originalIds[firstIndex] : this.m_modifiedIds[firstIndex];
      int num2 = secondIsOriginal ? this.m_originalIds[secondIndex] : this.m_modifiedIds[secondIndex];
      return num1 != 0 && num2 != 0 ? num1 == num2 : this.ElementComparer.Equals(firstIsOriginal ? this.OriginalSequence[firstIndex] : this.ModifiedSequence[firstIndex], secondIsOriginal ? this.OriginalSequence[secondIndex] : this.ModifiedSequence[secondIndex]);
    }

    private void ComputeUniqueIdentifiers(
      int originalStart,
      int originalEnd,
      int modifiedStart,
      int modifiedEnd)
    {
      Dictionary<T, int> dictionary = new Dictionary<T, int>(this.OriginalSequence.Count + this.ModifiedSequence.Count, this.ElementComparer);
      int num = 1;
      for (int index = originalStart; index <= originalEnd; ++index)
      {
        T key = this.OriginalSequence[index];
        if (!dictionary.TryGetValue(key, out this.m_originalIds[index]))
        {
          this.m_originalIds[index] = num++;
          dictionary.Add(key, this.m_originalIds[index]);
        }
      }
      for (int index = modifiedStart; index <= modifiedEnd; ++index)
      {
        T key = this.ModifiedSequence[index];
        if (!dictionary.TryGetValue(key, out this.m_modifiedIds[index]))
        {
          this.m_modifiedIds[index] = num++;
          dictionary.Add(key, this.m_modifiedIds[index]);
        }
      }
    }

    public IDiffChange[] Diff(
      IList<T> original,
      IList<T> modified,
      IEqualityComparer<T> elementComparer)
    {
      return this.Diff(original, modified, elementComparer, (Microsoft.VisualStudio.Services.CodeReview.Server.Common.ContinueDifferencePredicate<T>) null);
    }

    private static WordPosition[] tokenizeWords(IList<T> file, int start, int end)
    {
      List<WordPosition> wordPositionList = new List<WordPosition>();
      char[] chArray = new char[5]
      {
        '\r',
        '\n',
        '\u2028',
        '\u2029',
        '\u0085'
      };
      int num1 = 0;
      for (int index = start; index < end; ++index)
      {
        string str = file[index].ToString().TrimEnd(chArray);
        int num2 = 0;
        int num3 = 0;
        while (num2 < str.Length && (str[num2] == ' ' || str[num2] == '\t'))
        {
          ++num2;
          ++num3;
        }
        for (; num3 < str.Length; ++num3)
        {
          if (num2 == num3 && " ;:().{}=\t[]<>".Contains<char>(str[num3]))
          {
            WordPosition wordPosition = new WordPosition(str[num3].ToString(), index, num2, 1, num1 + num2);
            wordPositionList.Add(wordPosition);
            ++num2;
          }
          else if (" ;:().{}=\t[]<>".Contains<char>(str[num3]))
          {
            int length = num3 - num2;
            WordPosition wordPosition1 = new WordPosition(str.Substring(num2, length), index, num2, length, num1 + num2);
            WordPosition wordPosition2 = new WordPosition(str[num3].ToString(), index, num3, 1, num1 + num3);
            wordPositionList.Add(wordPosition1);
            wordPositionList.Add(wordPosition2);
            num2 = num3 + 1;
          }
        }
        if (num2 != str.Length)
        {
          string word = str.Substring(num2);
          wordPositionList.Add(new WordPosition(word, index, num2, word.Length, num1 + num2));
        }
        num1 += str.Length;
      }
      return wordPositionList.ToArray();
    }

    private IList<IDiffChange> createDiffBlockPerLine(
      int startLine,
      int endLine,
      int wordStart,
      int wordCount,
      WordPosition[] wordPositions,
      bool isOriginal)
    {
      IList<IDiffChange> diffBlockPerLine = (IList<IDiffChange>) new List<IDiffChange>();
      WordPosition[] array = ((IEnumerable<WordPosition>) wordPositions).Skip<WordPosition>(wordStart).Take<WordPosition>(wordCount).ToArray<WordPosition>();
      if (array.Length == 0)
        return (IList<IDiffChange>) new List<IDiffChange>();
      int index = 0;
      for (int line = array[0].Line; line <= endLine; ++line)
      {
        if (array[index].Line == line)
        {
          WordPosition wordPosition1 = array[index];
          do
          {
            ++index;
          }
          while (index < wordCount && array[index].Line == line);
          WordPosition wordPosition2 = array[index - 1];
          int fullStart = wordPosition1.FullStart;
          int num = wordPosition2.FullStart + wordPosition2.Length;
          IDiffChange diffChange = isOriginal ? (IDiffChange) new DiffChange(fullStart, num - fullStart, 0, 0) : (IDiffChange) new DiffChange(0, 0, fullStart, num - fullStart);
          diffBlockPerLine.Add(diffChange);
        }
      }
      return diffBlockPerLine;
    }

    public IList<LineChanges> WordDiff(
      IDiffChange[] lineDiffs,
      IList<T> original,
      IList<T> modified)
    {
      IList<LineChanges> lineChangesList = (IList<LineChanges>) new List<LineChanges>();
      DiffFinder<WordPosition> lcsDiff = DiffFinder<WordPosition>.LcsDiff;
      foreach (IDiffChange lineDiff in lineDiffs)
      {
        if (lineDiff.ChangeType == DiffChangeType.Change)
        {
          WordPosition[] wordPositionArray1 = DiffFinder<T>.tokenizeWords(original, lineDiff.OriginalStart, lineDiff.OriginalEnd);
          WordPosition[] wordPositionArray2 = DiffFinder<T>.tokenizeWords(modified, lineDiff.ModifiedStart, lineDiff.ModifiedEnd);
          IDiffChange[] diffChangeArray = lcsDiff.Diff((IList<WordPosition>) wordPositionArray1, (IList<WordPosition>) wordPositionArray2, (IEqualityComparer<WordPosition>) new DiffFinder<T>.WordComparer());
          List<IDiffChange> diffChangeList = new List<IDiffChange>();
          foreach (IDiffChange diffChange1 in diffChangeArray)
          {
            int line1 = wordPositionArray1.Length == 0 || diffChange1.ChangeType == DiffChangeType.Insert ? 0 : wordPositionArray1[diffChange1.OriginalStart].Line;
            int endLine1 = diffChange1.OriginalLength == 0 ? line1 : wordPositionArray1[diffChange1.OriginalEnd - 1].Line;
            int line2 = wordPositionArray2.Length == 0 || diffChange1.ChangeType == DiffChangeType.Delete ? 0 : wordPositionArray2[diffChange1.ModifiedStart].Line;
            int endLine2 = diffChange1.ModifiedLength == 0 ? line2 : wordPositionArray2[diffChange1.ModifiedEnd - 1].Line;
            if (line1 != endLine1 || line2 != endLine2)
            {
              diffChangeList.AddRange((IEnumerable<IDiffChange>) this.createDiffBlockPerLine(line1, endLine1, diffChange1.OriginalStart, diffChange1.OriginalLength, wordPositionArray1, true));
              diffChangeList.AddRange((IEnumerable<IDiffChange>) this.createDiffBlockPerLine(line2, endLine2, diffChange1.ModifiedStart, diffChange1.ModifiedLength, wordPositionArray2, false));
            }
            else
            {
              int fullStart1 = wordPositionArray1.Length == 0 || diffChange1.ChangeType == DiffChangeType.Insert ? 0 : wordPositionArray1[diffChange1.OriginalStart].FullStart;
              int num1 = fullStart1;
              if (diffChange1.OriginalLength > 0)
                num1 = wordPositionArray1[diffChange1.OriginalStart + diffChange1.OriginalLength - 1].FullStart + wordPositionArray1[diffChange1.OriginalStart + diffChange1.OriginalLength - 1].Length;
              int fullStart2 = wordPositionArray2.Length == 0 || diffChange1.ChangeType == DiffChangeType.Delete ? 0 : wordPositionArray2[diffChange1.ModifiedStart].FullStart;
              int num2 = fullStart2;
              if (diffChange1.ModifiedLength > 0)
                num2 = wordPositionArray2[diffChange1.ModifiedStart + diffChange1.ModifiedLength - 1].FullStart + wordPositionArray2[diffChange1.ModifiedStart + diffChange1.ModifiedLength - 1].Length;
              int originalLength = num1 - fullStart1;
              int modifiedLength = num2 - fullStart2;
              IDiffChange diffChange2 = (IDiffChange) new DiffChange(fullStart1, originalLength, fullStart2, modifiedLength);
              diffChangeList.Add(diffChange2);
            }
          }
          lineChangesList.Add(new LineChanges(lineDiff, diffChangeList.ToArray()));
        }
        else
          lineChangesList.Add(new LineChanges(lineDiff, (IDiffChange[]) null));
      }
      return lineChangesList;
    }

    public IDiffChange[] Diff(
      IList<T> original,
      IList<T> modified,
      IEqualityComparer<T> elementComparer,
      Microsoft.VisualStudio.Services.CodeReview.Server.Common.ContinueDifferencePredicate<T> predicate)
    {
      this.m_original = original;
      this.m_modified = modified;
      this.m_elementComparer = elementComparer;
      this.m_predicate = predicate;
      this.m_originalIds = new int[this.OriginalSequence.Count];
      this.m_modifiedIds = new int[this.ModifiedSequence.Count];
      int num1 = 0;
      int num2 = this.OriginalSequence.Count - 1;
      int num3 = 0;
      int num4;
      for (num4 = this.ModifiedSequence.Count - 1; num1 <= num2 && num3 <= num4 && this.ElementsAreEqual(num1, num3); ++num3)
        ++num1;
      for (; num2 >= num1 && num4 >= num3 && this.ElementsAreEqual(num2, num4); --num4)
        --num2;
      if (num1 > num2 || num3 > num4)
      {
        IDiffChange[] diffChangeArray;
        if (num3 <= num4)
          diffChangeArray = new IDiffChange[1]
          {
            (IDiffChange) new DiffChange(num1, 0, num3, num4 - num3 + 1)
          };
        else if (num1 <= num2)
          diffChangeArray = new IDiffChange[1]
          {
            (IDiffChange) new DiffChange(num1, num2 - num1 + 1, num3, 0)
          };
        else
          diffChangeArray = Array.Empty<IDiffChange>();
        return diffChangeArray;
      }
      this.ComputeUniqueIdentifiers(num1, num2, num3, num4);
      return this.ComputeDiff(num1, num2, num3, num4);
    }

    protected abstract IDiffChange[] ComputeDiff(
      int originalStart,
      int originalEnd,
      int modifiedStart,
      int modifiedEnd);

    public static DiffFinder<T> LcsDiff => (DiffFinder<T>) new Microsoft.VisualStudio.Services.CodeReview.Server.Common.LcsDiff<T>();

    protected Microsoft.VisualStudio.Services.CodeReview.Server.Common.ContinueDifferencePredicate<T> ContinueDifferencePredicate => this.m_predicate;

    private void ScanEqualRegion(
      List<IDiffChange> listChanges,
      IEqualityComparer<T> secondaryElementComparer,
      int oBegin,
      int oLen,
      int mBegin,
      int mLen)
    {
      int num = -1;
      for (int index = 0; index < oLen; ++index)
      {
        if (secondaryElementComparer.Equals(this.OriginalSequence[oBegin + index], this.ModifiedSequence[mBegin + index]))
        {
          if (num != -1)
          {
            DiffChange diffChange = new DiffChange(oBegin + num, index - num, mBegin + num, index - num);
            listChanges.Add((IDiffChange) diffChange);
            num = -1;
          }
        }
        else if (num == -1)
          num = index;
      }
      if (num == -1)
        return;
      DiffChange diffChange1 = new DiffChange(oBegin + num, oLen - num, mBegin + num, mLen - num);
      listChanges.Add((IDiffChange) diffChange1);
    }

    public IDiffChange[] DiffEx(
      IList<T> original,
      IList<T> modified,
      IEqualityComparer<T> primaryElementComparer,
      IEqualityComparer<T> secondaryElementComparer)
    {
      return this.DiffEx(original, modified, primaryElementComparer, secondaryElementComparer, (Microsoft.VisualStudio.Services.CodeReview.Server.Common.ContinueDifferencePredicate<T>) null);
    }

    public IDiffChange[] DiffEx(
      IList<T> original,
      IList<T> modified,
      IEqualityComparer<T> primaryElementComparer,
      IEqualityComparer<T> secondaryElementComparer,
      Microsoft.VisualStudio.Services.CodeReview.Server.Common.ContinueDifferencePredicate<T> predicate)
    {
      IDiffChange[] diffChangeArray = this.Diff(original, modified, primaryElementComparer, predicate);
      if (secondaryElementComparer == null)
        return diffChangeArray;
      List<IDiffChange> listChanges = new List<IDiffChange>();
      if (diffChangeArray.Length == 0)
      {
        this.ScanEqualRegion(listChanges, secondaryElementComparer, 0, original.Count, 0, modified.Count);
      }
      else
      {
        IDiffChange diffChange1 = diffChangeArray[0];
        if (diffChange1.OriginalStart > 0)
          this.ScanEqualRegion(listChanges, secondaryElementComparer, 0, diffChange1.OriginalStart, 0, diffChange1.ModifiedStart);
        listChanges.Add(diffChange1);
        for (int index = 1; index < diffChangeArray.Length; ++index)
        {
          IDiffChange diffChange2 = diffChangeArray[index];
          this.ScanEqualRegion(listChanges, secondaryElementComparer, diffChange1.OriginalEnd, diffChange2.OriginalStart - diffChange1.OriginalEnd, diffChange1.ModifiedEnd, diffChange2.ModifiedStart - diffChange1.ModifiedEnd);
          diffChange1 = diffChange2;
          listChanges.Add(diffChange1);
        }
        if (diffChange1.OriginalEnd < original.Count)
          this.ScanEqualRegion(listChanges, secondaryElementComparer, diffChange1.OriginalEnd, original.Count - diffChange1.OriginalEnd, diffChange1.ModifiedEnd, modified.Count - diffChange1.ModifiedEnd);
      }
      IDiffChange[] array = listChanges.ToArray();
      if (array.Length < 2)
        return array;
      List<IDiffChange> diffChangeList = new List<IDiffChange>();
      IDiffChange diffChange3 = array[0];
      int originalStart = diffChange3.OriginalStart;
      int originalLength = diffChange3.OriginalLength;
      int modifiedStart = diffChange3.ModifiedStart;
      int modifiedLength = diffChange3.ModifiedLength;
      for (int index = 1; index < array.Length; ++index)
      {
        IDiffChange diffChange4 = array[index];
        if (diffChange4.OriginalStart == originalStart + originalLength && diffChange4.ModifiedStart == modifiedStart + modifiedLength)
        {
          originalLength += diffChange4.OriginalLength;
          modifiedLength += diffChange4.ModifiedLength;
        }
        else
        {
          diffChangeList.Add((IDiffChange) new DiffChange(originalStart, originalLength, modifiedStart, modifiedLength));
          originalStart = diffChange4.OriginalStart;
          originalLength = diffChange4.OriginalLength;
          modifiedStart = diffChange4.ModifiedStart;
          modifiedLength = diffChange4.ModifiedLength;
        }
      }
      diffChangeList.Add((IDiffChange) new DiffChange(originalStart, originalLength, modifiedStart, modifiedLength));
      return diffChangeList.ToArray();
    }

    private class WordComparer : IEqualityComparer<WordPosition>
    {
      public bool Equals(WordPosition w1, WordPosition w2) => string.Equals(w1.Word, w2.Word);

      public int GetHashCode(WordPosition w) => w.Word.GetHashCode();
    }
  }
}
