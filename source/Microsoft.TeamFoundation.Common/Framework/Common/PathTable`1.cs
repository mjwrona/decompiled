// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Common.PathTable`1
// Assembly: Microsoft.TeamFoundation.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E643B42-FE11-4FC2-A9D6-79417E26CF92
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Common.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.TeamFoundation.Framework.Common
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class PathTable<T>
  {
    private static readonly PathTable<T>.EntryCompare s_fullPathCompare = new PathTable<T>.EntryCompare(PathTable<T>.FullPathCompare);
    private static readonly PathTable<T>.EntryCompare s_parentPathCompare = new PathTable<T>.EntryCompare(PathTable<T>.ParentPathCompare);
    private readonly char m_tokenSeparator;
    private readonly string m_tokenSeparatorString;
    private readonly StringComparison m_comparison;
    private PathTable<T>.PathTableRow<T>[] m_list;
    private int m_size;
    private bool m_sorted;
    private static readonly PathTable<T>.PathTableRow<T>[] s_emptyArray = new PathTable<T>.PathTableRow<T>[0];

    public PathTable(char tokenSeparator, bool caseInsensitive)
    {
      this.m_tokenSeparator = tokenSeparator;
      this.m_tokenSeparatorString = new string(tokenSeparator, 1);
      this.m_list = PathTable<T>.s_emptyArray;
      this.m_size = 0;
      this.m_sorted = true;
      if (caseInsensitive)
        this.m_comparison = StringComparison.OrdinalIgnoreCase;
      else
        this.m_comparison = StringComparison.Ordinal;
    }

    public void Reserve(int capacity)
    {
      if (capacity <= this.m_list.Length)
        return;
      PathTable<T>.PathTableRow<T>[] destinationArray = new PathTable<T>.PathTableRow<T>[capacity];
      if (this.m_size > 0)
        Array.Copy((Array) this.m_list, 0, (Array) destinationArray, 0, this.m_size);
      this.m_list = destinationArray;
    }

    private void EnsureCapacity(int minimum)
    {
      if (this.m_list.Length >= minimum)
        return;
      int capacity = this.m_list.Length == 0 ? 4 : this.m_list.Length * 2;
      if (capacity < minimum)
        capacity = minimum;
      this.Reserve(capacity);
    }

    public void Add(string token, T referencedObject, bool overwrite = false)
    {
      ArgumentUtility.CheckForNull<string>(token, nameof (token));
      this.RequireSorted();
      token = this.CanonicalizeToken(token);
      int parentPathLength = this.GetParentPathLength(token);
      int index = this.Seek(PathTable<T>.s_fullPathCompare, token, parentPathLength, token.Length);
      if (index < 0)
      {
        this.Insert(~index, new PathTable<T>.PathTableRow<T>(token, parentPathLength, 0, referencedObject));
      }
      else
      {
        if (!overwrite)
          throw new ArgumentException("The token already exists in the PathTable.", nameof (token));
        this.m_list[index] = new PathTable<T>.PathTableRow<T>(token, parentPathLength, 0, referencedObject);
      }
    }

    private void Insert(int index, in PathTable<T>.PathTableRow<T> item)
    {
      if (this.m_size == this.m_list.Length)
        this.EnsureCapacity(this.m_size + 1);
      if (index < this.m_size)
        Array.Copy((Array) this.m_list, index, (Array) this.m_list, index + 1, this.m_size - index);
      this.m_list[index] = item;
      ++this.m_size;
    }

    public void ModifyInPlace(string token, PathTable<T>.ModifyInPlaceCallback callback)
    {
      ArgumentUtility.CheckForNull<string>(token, nameof (token));
      this.RequireSorted();
      token = this.CanonicalizeToken(token);
      int parentPathLength = this.GetParentPathLength(token);
      int index = this.Seek(PathTable<T>.s_fullPathCompare, token, parentPathLength, token.Length);
      PathTable<T>.PathTableRow<T> pathTableRow;
      T referencedObject1;
      if (index >= 0)
      {
        pathTableRow = this.m_list[index];
        referencedObject1 = pathTableRow.ReferencedObject;
      }
      else
      {
        pathTableRow = new PathTable<T>.PathTableRow<T>();
        referencedObject1 = default (T);
      }
      T referencedObject2 = callback(referencedObject1);
      if (index >= 0)
        this.m_list[index] = new PathTable<T>.PathTableRow<T>(pathTableRow.Token, pathTableRow.ParentPathLength, pathTableRow.OriginalIndex, referencedObject2);
      else
        this.Insert(~index, new PathTable<T>.PathTableRow<T>(token, parentPathLength, 0, referencedObject2));
    }

    public void AddUnsorted(string token, T referencedObject)
    {
      ArgumentUtility.CheckForNull<string>(token, nameof (token));
      token = this.CanonicalizeToken(token);
      int parentPathLength = this.GetParentPathLength(token);
      if (this.m_sorted && this.m_size > 0 && PathTable<T>.FullPathCompare(in this.m_list[this.m_size - 1], token, this.m_comparison, parentPathLength, token.Length) >= 0)
        this.m_sorted = false;
      if (this.m_size == this.m_list.Length)
        this.EnsureCapacity(this.m_size + 1);
      this.m_list[this.m_size] = new PathTable<T>.PathTableRow<T>(token, parentPathLength, this.m_size, referencedObject);
      ++this.m_size;
    }

    public void Sort(bool checkForDuplicates = false) => this.Sort(!checkForDuplicates ? (Func<string, T, T, bool>) null : PathTable<T>.\u003C\u003EO.\u003C0\u003E__DefaultDuplicateHandler ?? (PathTable<T>.\u003C\u003EO.\u003C0\u003E__DefaultDuplicateHandler = new Func<string, T, T, bool>(PathTable<T>.DefaultDuplicateHandler)));

    private static bool DefaultDuplicateHandler(string token, T value1, T value2) => false;

    public void Sort(Func<string, T, T, bool> duplicateHandler)
    {
      if (this.m_sorted)
        return;
      if (this.m_size > 0)
      {
        if (this.m_comparison == StringComparison.OrdinalIgnoreCase)
          Array.Sort<PathTable<T>.PathTableRow<T>>(this.m_list, 0, this.m_size, (IComparer<PathTable<T>.PathTableRow<T>>) PathTable<T>.RowComparer.OrdinalIgnoreCase);
        else
          Array.Sort<PathTable<T>.PathTableRow<T>>(this.m_list, 0, this.m_size, (IComparer<PathTable<T>.PathTableRow<T>>) PathTable<T>.RowComparer.Ordinal);
      }
      if (duplicateHandler != null)
      {
        for (int index = this.m_size - 1; index > 0; --index)
        {
          if (string.Equals(this.m_list[index - 1].Token, this.m_list[index].Token, this.m_comparison))
          {
            if (!duplicateHandler(this.m_list[index - 1].Token, this.m_list[index - 1].ReferencedObject, this.m_list[index].ReferencedObject))
              throw new ArgumentException("Duplicate tokens exist in the PathTable.");
            this.RemoveAt(index - 1);
          }
        }
      }
      this.m_sorted = true;
    }

    private void RemoveAt(int index)
    {
      --this.m_size;
      if (index < this.m_size)
        Array.Copy((Array) this.m_list, index + 1, (Array) this.m_list, index, this.m_size - index);
      this.m_list[this.m_size] = new PathTable<T>.PathTableRow<T>();
    }

    public bool TryGetByIndex(int index, out T referencedObject)
    {
      ArgumentUtility.CheckForOutOfRange(index, nameof (index), 0);
      this.RequireSorted();
      if (index < this.m_size)
      {
        referencedObject = this.m_list[index].ReferencedObject;
        return true;
      }
      referencedObject = default (T);
      return false;
    }

    public bool TryGetValue(string token, out T referencedObject) => this.TryGetValueAndIndex(token, out referencedObject, out int _);

    public bool TryGetValueAndIndex(string token, out T referencedObject, out int index)
    {
      ArgumentUtility.CheckForNull<string>(token, nameof (token));
      this.RequireSorted();
      token = this.CanonicalizeToken(token);
      index = this.Seek(PathTable<T>.s_fullPathCompare, token, this.GetParentPathLength(token), token.Length);
      if (index >= 0)
      {
        referencedObject = this.m_list[index].ReferencedObject;
        return true;
      }
      referencedObject = default (T);
      index = -1;
      return false;
    }

    public T this[string token]
    {
      get
      {
        T referencedObject;
        if (!this.TryGetValue(token, out referencedObject))
          throw new KeyNotFoundException();
        return referencedObject;
      }
      set => this.Add(token, value, true);
    }

    public void SetValueAtIndex(int index, string token, T value)
    {
      ArgumentUtility.CheckForOutOfRange(index, nameof (index), 0, this.m_size - 1);
      ArgumentUtility.CheckForNull<string>(token, nameof (token));
      this.RequireSorted();
      token = this.CanonicalizeToken(token);
      PathTable<T>.PathTableRow<T> pathTableRow = this.m_list[index];
      if (!string.Equals(token, pathTableRow.Token, this.m_comparison))
        throw new ArgumentOutOfRangeException(nameof (token));
      this.m_list[index] = new PathTable<T>.PathTableRow<T>(token, pathTableRow.ParentPathLength, pathTableRow.OriginalIndex, value);
    }

    public StringComparison Comparison => this.m_comparison;

    public int Count => this.m_size;

    public void Clear()
    {
      this.m_list = PathTable<T>.s_emptyArray;
      this.m_size = 0;
      this.m_sorted = true;
    }

    public bool Remove(string token, bool removeChildren)
    {
      ArgumentUtility.CheckForNull<string>(token, nameof (token));
      this.RequireSorted();
      token = this.CanonicalizeToken(token);
      bool flag = false;
      if (removeChildren)
      {
        string token1 = token + this.m_tokenSeparatorString;
        int index = ~this.Seek(PathTable<T>.s_parentPathCompare, token1, 1, 0);
        if (index < this.m_size && this.IsSubItem(this.m_list[index].Token, token))
        {
          int num = ~this.Seek(PathTable<T>.s_parentPathCompare, this.EndRange(token1), 1, 0, index);
          this.RemoveRange(index, num - index);
          flag = true;
        }
      }
      int index1 = this.Seek(PathTable<T>.s_fullPathCompare, token, this.GetParentPathLength(token), token.Length);
      if (index1 >= 0)
      {
        this.RemoveAt(index1);
        flag = true;
      }
      return flag;
    }

    private void RemoveRange(int index, int count)
    {
      if (count <= 0)
        return;
      this.m_size -= count;
      if (index < this.m_size)
        Array.Copy((Array) this.m_list, index + count, (Array) this.m_list, index, this.m_size - index);
      Array.Clear((Array) this.m_list, this.m_size, count);
    }

    public IEnumerable<PathTableTokenAndValue<T>> EnumRoots()
    {
      this.RequireSorted();
      PathTable<T>.PathTableRanges ranges = new PathTable<T>.PathTableRanges(new PathTable<T>.PathTableRange(0, this.m_size));
      for (int i = 0; i < ranges.Ranges.Count; ++i)
      {
        PathTable<T>.PathTableRange range = ranges.Ranges[i];
        for (int j = range.StartIndex; j < range.EndIndex; ++j)
        {
          PathTable<T>.PathTableRow<T> row = this.m_list[j];
          yield return new PathTableTokenAndValue<T>(row.Token, row.ReferencedObject);
          PathTable<T>.PathTableRange toExclude = this.RangeSeek(row.Token, PathTableRecursion.Full, j, ranges.EndIndex);
          if (toExclude.Length > 0)
          {
            ranges.Exclude(toExclude);
            range = ranges.Ranges[i];
          }
          row = new PathTable<T>.PathTableRow<T>();
        }
      }
    }

    public static IEnumerable<PathTableTokenAndValue<T>> EnumAllDifferences(
      PathTable<T> pt1,
      PathTable<T> pt2)
    {
      if (pt1 != null && pt2 != null && pt1.Comparison != pt2.Comparison)
        throw new InvalidOperationException();
      PathTable<T> pathTable1 = pt1;
      int num1;
      if (pathTable1 == null)
      {
        PathTable<T> pathTable2 = pt2;
        num1 = pathTable2 != null ? (int) pathTable2.Comparison : 4;
      }
      else
        num1 = (int) pathTable1.Comparison;
      StringComparison comparison = (StringComparison) num1;
      pt1?.RequireSorted();
      pt2?.RequireSorted();
      int pt1Size = pt1 != null ? pt1.m_size : 0;
      int pt2Size = pt2 != null ? pt2.m_size : 0;
      int i = 0;
      int j = 0;
      while (i < pt1Size && j < pt2Size)
      {
        int num2 = PathTable<T>.s_fullPathCompare(in pt1.m_list[i], pt2.m_list[j].Token, comparison, pt2.m_list[j].ParentPathLength, pt2.m_list[j].Token.Length);
        if (num2 == 0)
        {
          MoveNextNonDuplicate(pt1.m_list, comparison, pt1Size, ref i);
          MoveNextNonDuplicate(pt2.m_list, comparison, pt2Size, ref j);
        }
        else if (num2 < 0)
        {
          yield return new PathTableTokenAndValue<T>(pt1.m_list[i].Token, pt1.m_list[i].ReferencedObject);
          MoveNextNonDuplicate(pt1.m_list, comparison, pt1Size, ref i);
        }
        else
        {
          yield return new PathTableTokenAndValue<T>(pt2.m_list[j].Token, pt2.m_list[j].ReferencedObject);
          MoveNextNonDuplicate(pt2.m_list, comparison, pt2Size, ref j);
        }
      }
      while (i < pt1Size)
      {
        yield return new PathTableTokenAndValue<T>(pt1.m_list[i].Token, pt1.m_list[i].ReferencedObject);
        MoveNextNonDuplicate(pt1.m_list, comparison, pt1Size, ref i);
      }
      while (j < pt2Size)
      {
        yield return new PathTableTokenAndValue<T>(pt2.m_list[j].Token, pt2.m_list[j].ReferencedObject);
        MoveNextNonDuplicate(pt2.m_list, comparison, pt2Size, ref j);
      }

      static void MoveNextNonDuplicate(
        PathTable<T>.PathTableRow<T>[] list,
        StringComparison listComparison,
        int listSize,
        ref int k)
      {
        do
        {
          ++k;
        }
        while (k < listSize && string.Equals(list[k - 1].Token, list[k].Token, listComparison));
      }
    }

    public IEnumerable<T> EnumRootsReferencedObjects()
    {
      foreach (PathTableTokenAndValue<T> enumRoot in this.EnumRoots())
        yield return enumRoot.Value;
    }

    public IEnumerable<PathTableTokenAndValue<T>> EnumParents(string token)
    {
      ArgumentUtility.CheckForNull<string>(token, nameof (token));
      this.RequireSorted();
      token = this.CanonicalizeToken(token);
      int index = int.MaxValue;
      int parentPathLength = this.GetParentPathLength(token);
      int compareParam2 = token.Length;
      while (compareParam2 >= 0)
      {
        index = this.Seek(PathTable<T>.s_fullPathCompare, token, parentPathLength, compareParam2, endIndex: index);
        if (index >= 0)
          yield return new PathTableTokenAndValue<T>(this.m_list[index].Token, this.m_list[index].ReferencedObject);
        else
          index = ~index;
        if (index == 0)
          break;
        --index;
        compareParam2 = parentPathLength - 1;
        parentPathLength = this.GetParentPathLength(token, parentPathLength);
      }
    }

    public IEnumerable<T> EnumParentsReferencedObjects(string token)
    {
      foreach (PathTableTokenAndValue<T> enumParent in this.EnumParents(token))
        yield return enumParent.Value;
    }

    public PathTable<T>.SimpleSubTreeEnumerable EnumSubTree(
      string token,
      bool enumerateSubTreeRoot,
      PathTableRecursion depth)
    {
      if (token == null && PathTableRecursion.Full == depth)
        return new PathTable<T>.SimpleSubTreeEnumerable(this.m_list, -1, new PathTable<T>.PathTableRange(0, this.m_size));
      ArgumentUtility.CheckForNull<string>(token, nameof (token));
      this.RequireSorted();
      token = this.CanonicalizeToken(token);
      int rootIndex;
      PathTable<T>.PathTableRange children;
      this.SubTreeSeek(token, enumerateSubTreeRoot, depth, out rootIndex, out children);
      return new PathTable<T>.SimpleSubTreeEnumerable(this.m_list, rootIndex, in children);
    }

    public IEnumerable<PathTableTokenAndValue<T>> EnumSubTree(
      string token,
      bool enumerateSubTreeRoot,
      PathTableRecursion depth,
      IEnumerable<string> exclusions)
    {
      if (token == null && PathTableRecursion.Full == depth)
        return (IEnumerable<PathTableTokenAndValue<T>>) new PathTable<T>.SimpleSubTreeEnumerable(this.m_list, -1, new PathTable<T>.PathTableRange(0, this.m_size));
      ArgumentUtility.CheckForNull<string>(token, nameof (token));
      this.RequireSorted();
      token = this.CanonicalizeToken(token);
      int rootIndex;
      PathTable<T>.PathTableRange children;
      this.SubTreeSeek(token, enumerateSubTreeRoot, depth, out rootIndex, out children);
      if (exclusions == null || !exclusions.Any<string>())
        return (IEnumerable<PathTableTokenAndValue<T>>) new PathTable<T>.SimpleSubTreeEnumerable(this.m_list, rootIndex, in children);
      PathTable<T>.PathTableRanges ranges = new PathTable<T>.PathTableRanges(rootIndex, children);
      depth = PathTable<T>.DepthReduce(depth);
      foreach (string token1 in exclusions.Select<string, string>((Func<string, string>) (s => this.CanonicalizeToken(s))))
      {
        this.SubTreeSeek(token1, true, depth, out rootIndex, out children, ranges.StartIndex, ranges.EndIndex);
        ranges.Exclude(rootIndex, children);
      }
      return this.EnumRanges(ranges);
    }

    private IEnumerable<PathTableTokenAndValue<T>> EnumRanges(PathTable<T>.PathTableRanges ranges)
    {
      foreach (PathTable<T>.PathTableRange range in (IEnumerable<PathTable<T>.PathTableRange>) ranges.Ranges)
      {
        for (int i = range.StartIndex; i < range.EndIndex; ++i)
          yield return new PathTableTokenAndValue<T>(this.m_list[i].Token, this.m_list[i].ReferencedObject);
      }
    }

    public IEnumerable<T> EnumSubTreeReferencedObjects(
      string token,
      bool enumerateSubTreeRoot,
      PathTableRecursion depth,
      IEnumerable<string> exclusions = null)
    {
      foreach (PathTableTokenAndValue<T> tableTokenAndValue in this.EnumSubTree(token, enumerateSubTreeRoot, depth, exclusions))
        yield return tableTokenAndValue.Value;
    }

    private void SubTreeSeek(
      string token,
      bool enumerateSubTreeRoot,
      PathTableRecursion depth,
      out int rootIndex,
      out PathTable<T>.PathTableRange children,
      int startIndex = 0,
      int endIndex = 2147483647)
    {
      rootIndex = -1;
      children = new PathTable<T>.PathTableRange();
      if (enumerateSubTreeRoot)
      {
        rootIndex = this.Seek(PathTable<T>.s_fullPathCompare, token, this.GetParentPathLength(token), token.Length, startIndex, endIndex);
        if (rootIndex >= 0)
          startIndex = rootIndex;
      }
      if (depth == PathTableRecursion.None)
        return;
      children = this.RangeSeek(token, depth, startIndex, endIndex);
    }

    private IEnumerable<PathTable<T>.PathTableRange> SubTreeSeek(
      string token,
      bool enumerateSubTreeRoot,
      PathTableRecursion depth,
      int startIndex = 0,
      int endIndex = 2147483647)
    {
      if (enumerateSubTreeRoot)
      {
        int rootIndex = this.Seek(PathTable<T>.s_fullPathCompare, token, this.GetParentPathLength(token), token.Length, startIndex, endIndex);
        if (rootIndex >= 0)
        {
          yield return new PathTable<T>.PathTableRange(rootIndex, rootIndex + 1);
          startIndex = rootIndex;
        }
      }
      if (depth != PathTableRecursion.None)
        yield return this.RangeSeek(token, depth, startIndex, endIndex);
    }

    private PathTable<T>.PathTableRange RangeSeek(
      string token,
      PathTableRecursion depth,
      int startIndex = 0,
      int endIndex = 2147483647)
    {
      string token1 = token + this.m_tokenSeparatorString;
      int startIndex1 = ~this.Seek(PathTable<T>.s_parentPathCompare, token1, 1, 0, startIndex, endIndex);
      int endIndex1 = startIndex1;
      if (PathTableRecursion.OneLevel == depth)
      {
        if (startIndex1 < this.m_size && this.m_list[startIndex1].ParentPathLength == token1.Length && this.IsSubItem(this.m_list[startIndex1].Token, token))
          endIndex1 = ~this.Seek(PathTable<T>.s_parentPathCompare, token1, -1, 0, startIndex1, endIndex);
      }
      else if (startIndex1 < this.m_size && this.IsSubItem(this.m_list[startIndex1].Token, token))
        endIndex1 = ~this.Seek(PathTable<T>.s_parentPathCompare, this.EndRange(token1), 1, 0, startIndex1, endIndex);
      return new PathTable<T>.PathTableRange(startIndex1, endIndex1);
    }

    public bool HasSubItem(string token, int startIndex = 0)
    {
      ArgumentUtility.CheckForNull<string>(token, nameof (token));
      this.RequireSorted();
      token = this.CanonicalizeToken(token);
      int index = ~this.Seek(PathTable<T>.s_parentPathCompare, token + this.m_tokenSeparatorString, 1, 0, startIndex);
      return index < this.m_size && this.IsSubItem(this.m_list[index].Token, token);
    }

    private string CanonicalizeToken(string token)
    {
      int canonicalTokenLength = this.GetCanonicalTokenLength(token);
      return canonicalTokenLength < token.Length ? token.Substring(0, canonicalTokenLength) : token;
    }

    private int GetCanonicalTokenLength(string token)
    {
      int length = token.Length;
      while (length > 0 && (int) token[length - 1] == (int) this.m_tokenSeparator)
        --length;
      return length;
    }

    private void RequireSorted()
    {
      if (!this.m_sorted)
        throw new InvalidOperationException();
    }

    private int GetParentPathLength(string token) => token.LastIndexOf(this.m_tokenSeparator) + 1;

    private int GetParentPathLength(string token, int tokenLength) => tokenLength < 2 ? 0 : token.LastIndexOf(this.m_tokenSeparator, tokenLength - 2, tokenLength - 1) + 1;

    private unsafe string EndRange(string token)
    {
      string str = string.Copy(token);
      fixed (char* chPtr = str)
        chPtr[str.Length - 1] = (char) ((uint) this.m_tokenSeparator + 1U);
      return str;
    }

    private static PathTableRecursion DepthReduce(PathTableRecursion depth)
    {
      if (PathTableRecursion.OneLevel == depth)
        depth = PathTableRecursion.None;
      return depth;
    }

    private bool IsSubItem(string item, string parent) => item.StartsWith(parent, this.m_comparison) && (int) item[parent.Length] == (int) this.m_tokenSeparator;

    private int Seek(
      PathTable<T>.EntryCompare compare,
      string token,
      int compareParam,
      int compareParam2,
      int startIndex = 0,
      int endIndex = 2147483647)
    {
      int num1 = startIndex;
      int num2 = this.m_size - 1;
      if (endIndex < num2)
        num2 = endIndex;
      while (num1 <= num2)
      {
        int index = (num2 - num1) / 2 + num1;
        int num3 = compare(in this.m_list[index], token, this.m_comparison, compareParam, compareParam2);
        if (num3 < 0)
        {
          num1 = index + 1;
        }
        else
        {
          if (num3 <= 0)
            return index;
          num2 = index - 1;
        }
      }
      return ~num1;
    }

    private static int FullPathCompare(
      in PathTable<T>.PathTableRow<T> entry,
      string token,
      StringComparison comparison,
      int parentPathLength,
      int tokenLength)
    {
      int num1 = string.Compare(entry.Token, 0, token, 0, entry.ParentPathLength < parentPathLength ? entry.ParentPathLength : parentPathLength, comparison);
      if (num1 != 0)
        return num1;
      int num2 = entry.ParentPathLength - parentPathLength;
      if (num2 != 0)
        return num2;
      int num3 = tokenLength - parentPathLength;
      int num4 = string.Compare(entry.Token, parentPathLength, token, parentPathLength, entry.ChildItemLength < num3 ? entry.ChildItemLength : num3, comparison);
      return num4 != 0 ? num4 : entry.Token.Length - tokenLength;
    }

    private static int ParentPathCompare(
      in PathTable<T>.PathTableRow<T> entry,
      string token,
      StringComparison comparison,
      int equalityResult,
      int dummy)
    {
      int num1 = string.Compare(entry.Token, 0, token, 0, entry.ParentPathLength < token.Length ? entry.ParentPathLength : token.Length, comparison);
      if (num1 != 0)
        return num1;
      int num2 = entry.ParentPathLength - token.Length;
      return num2 != 0 ? num2 : equalityResult;
    }

    public delegate T ModifyInPlaceCallback(T referencedObject);

    private delegate int EntryCompare(
      in PathTable<T>.PathTableRow<T> entry,
      string token,
      StringComparison comparison,
      int compareParam,
      int compareParam2);

    private class RowComparer : IComparer<PathTable<T>.PathTableRow<T>>
    {
      private readonly StringComparison m_comparison;
      public static readonly PathTable<T>.RowComparer Ordinal = new PathTable<T>.RowComparer(StringComparison.Ordinal);
      public static readonly PathTable<T>.RowComparer OrdinalIgnoreCase = new PathTable<T>.RowComparer(StringComparison.OrdinalIgnoreCase);

      public RowComparer(StringComparison comparison) => this.m_comparison = comparison;

      public int Compare(PathTable<T>.PathTableRow<T> a, PathTable<T>.PathTableRow<T> b)
      {
        int num = PathTable<T>.FullPathCompare(in a, b.Token, this.m_comparison, b.ParentPathLength, b.Token.Length);
        if (num == 0)
          num = a.OriginalIndex - b.OriginalIndex;
        return num;
      }
    }

    [DebuggerDisplay("Token = {Token}")]
    public readonly struct PathTableRow<X>
    {
      public readonly string Token;
      public readonly int ParentPathLength;
      public readonly int OriginalIndex;
      public readonly X ReferencedObject;

      public PathTableRow(
        string token,
        int parentPathLength,
        int originalIndex,
        X referencedObject)
      {
        this.Token = token;
        this.ParentPathLength = parentPathLength;
        this.OriginalIndex = originalIndex;
        this.ReferencedObject = referencedObject;
      }

      public int ChildItemLength => this.Token.Length - this.ParentPathLength;
    }

    public readonly struct SimpleSubTreeEnumerable : 
      IEnumerable<PathTableTokenAndValue<T>>,
      IEnumerable
    {
      internal readonly PathTable<T>.PathTableRow<T>[] List;
      public readonly int RootIndex;
      public readonly PathTable<T>.PathTableRange Children;

      public SimpleSubTreeEnumerable(
        PathTable<T>.PathTableRow<T>[] list,
        int rootIndex,
        in PathTable<T>.PathTableRange children)
      {
        this.List = list;
        this.RootIndex = rootIndex;
        this.Children = children;
      }

      public PathTable<T>.SimpleSubTreeEnumerator GetEnumerator() => new PathTable<T>.SimpleSubTreeEnumerator(this);

      IEnumerator<PathTableTokenAndValue<T>> IEnumerable<PathTableTokenAndValue<T>>.GetEnumerator() => (IEnumerator<PathTableTokenAndValue<T>>) this.GetEnumerator();

      IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.GetEnumerator();
    }

    public struct SimpleSubTreeEnumerator : 
      IEnumerator<PathTableTokenAndValue<T>>,
      IDisposable,
      IEnumerator
    {
      private readonly PathTable<T>.SimpleSubTreeEnumerable m_enumerable;
      private int m_nextIndex;
      private PathTableTokenAndValue<T> m_current;

      internal SimpleSubTreeEnumerator(PathTable<T>.SimpleSubTreeEnumerable enumerable)
      {
        this.m_enumerable = enumerable;
        this.m_nextIndex = PathTable<T>.SimpleSubTreeEnumerator.GetInitialNextIndex(enumerable);
        this.m_current = new PathTableTokenAndValue<T>();
      }

      public void Dispose()
      {
      }

      public bool MoveNext()
      {
        if (this.m_nextIndex < 0)
        {
          this.m_current = new PathTableTokenAndValue<T>();
          return false;
        }
        PathTable<T>.PathTableRow<T>[] list = this.m_enumerable.List;
        this.m_current = new PathTableTokenAndValue<T>(list[this.m_nextIndex].Token, list[this.m_nextIndex].ReferencedObject);
        if (this.m_nextIndex == this.m_enumerable.RootIndex)
        {
          this.m_nextIndex = this.m_enumerable.Children.Length <= 0 ? -1 : this.m_enumerable.Children.StartIndex;
        }
        else
        {
          ++this.m_nextIndex;
          if (this.m_nextIndex >= this.m_enumerable.Children.EndIndex)
            this.m_nextIndex = -1;
        }
        return true;
      }

      private static int GetInitialNextIndex(PathTable<T>.SimpleSubTreeEnumerable enumerable)
      {
        if (enumerable.RootIndex >= 0)
          return enumerable.RootIndex;
        return enumerable.Children.Length > 0 ? enumerable.Children.StartIndex : -1;
      }

      object IEnumerator.Current
      {
        get
        {
          if (this.m_current.Token == null)
            throw new InvalidOperationException();
          return (object) this.Current;
        }
      }

      void IEnumerator.Reset()
      {
        this.m_nextIndex = PathTable<T>.SimpleSubTreeEnumerator.GetInitialNextIndex(this.m_enumerable);
        this.m_current = new PathTableTokenAndValue<T>();
      }

      public PathTableTokenAndValue<T> Current => this.m_current;
    }

    public class PathTableRanges
    {
      private readonly List<PathTable<T>.PathTableRange> m_ranges;

      public PathTableRanges(PathTable<T>.PathTableRange range)
      {
        this.m_ranges = new List<PathTable<T>.PathTableRange>();
        this.m_ranges.Add(range);
      }

      public PathTableRanges(IEnumerable<PathTable<T>.PathTableRange> ranges) => this.m_ranges = new List<PathTable<T>.PathTableRange>(ranges);

      public PathTableRanges(int rootIndex, PathTable<T>.PathTableRange children)
      {
        this.m_ranges = new List<PathTable<T>.PathTableRange>(2);
        if (rootIndex >= 0)
          this.m_ranges.Add(new PathTable<T>.PathTableRange(rootIndex, rootIndex + 1));
        if (children.Length <= 0)
          return;
        this.m_ranges.Add(children);
      }

      public IReadOnlyList<PathTable<T>.PathTableRange> Ranges => (IReadOnlyList<PathTable<T>.PathTableRange>) this.m_ranges;

      public int StartIndex
      {
        get
        {
          int startIndex = 0;
          if (this.m_ranges.Count > 0)
            startIndex = this.m_ranges[0].StartIndex;
          return startIndex;
        }
      }

      public int EndIndex
      {
        get
        {
          int endIndex = 0;
          if (this.m_ranges.Count > 0)
            endIndex = this.m_ranges[this.m_ranges.Count - 1].EndIndex;
          return endIndex;
        }
      }

      internal void Exclude(IEnumerable<PathTable<T>.PathTableRange> toExclude)
      {
        foreach (PathTable<T>.PathTableRange toExclude1 in toExclude)
          this.Exclude(toExclude1);
      }

      internal void Exclude(int rootIndex, PathTable<T>.PathTableRange children)
      {
        if (rootIndex >= 0)
          this.Exclude(new PathTable<T>.PathTableRange(rootIndex, rootIndex + 1));
        if (children.Length <= 0)
          return;
        this.Exclude(children);
      }

      internal void Exclude(PathTable<T>.PathTableRange toExclude)
      {
        for (int index = this.m_ranges.Count - 1; index >= 0; --index)
        {
          PathTable<T>.PathTableRange range1;
          PathTable<T>.PathTableRange range2;
          this.m_ranges[index].Abjunction(toExclude, out range1, out range2);
          if (range1.Length > 0)
          {
            if (range2.Length > 0)
            {
              this.m_ranges[index] = range1;
              this.m_ranges.Insert(index + 1, range2);
            }
            else
              this.m_ranges[index] = range1;
          }
          else if (range2.Length > 0)
            this.m_ranges[index] = range2;
          else
            this.m_ranges.RemoveAt(index);
        }
      }
    }

    public readonly struct PathTableRange
    {
      public readonly int StartIndex;
      public readonly int EndIndex;

      public PathTableRange(int startIndex, int endIndex)
      {
        this.StartIndex = startIndex;
        this.EndIndex = endIndex;
      }

      public PathTable<T>.PathTableRange And(PathTable<T>.PathTableRange range) => new PathTable<T>.PathTableRange(this.StartIndex > range.StartIndex ? this.StartIndex : range.StartIndex, this.EndIndex < range.EndIndex ? this.EndIndex : range.EndIndex);

      public void Not(
        out PathTable<T>.PathTableRange range1,
        out PathTable<T>.PathTableRange range2)
      {
        range1 = new PathTable<T>.PathTableRange(0, this.StartIndex);
        range2 = new PathTable<T>.PathTableRange(this.EndIndex, int.MaxValue);
      }

      public void Abjunction(
        PathTable<T>.PathTableRange range,
        out PathTable<T>.PathTableRange range1,
        out PathTable<T>.PathTableRange range2)
      {
        range.Not(out range1, out range2);
        range1 = this.And(range1);
        range2 = this.And(range2);
      }

      public int Length
      {
        get
        {
          int length = this.EndIndex - this.StartIndex;
          if (length < 0)
            length = 0;
          return length;
        }
      }
    }
  }
}
