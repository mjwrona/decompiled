// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.Common.ShardingRangePrefixLister
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CB48D0BF-32A2-483C-A1D4-2F10DEBB3D56
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.Server.Common.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.BlobStore.Server.Common
{
  public static class ShardingRangePrefixLister
  {
    private static readonly char[] chars = "0123456789ABCDEF".ToArray<char>();

    public static List<string> ListAllPrefixes(string start, string end, int? totalLength = null) => ShardingRangePrefixLister.ListAllPrefixesAndCorePrefixes(start, end).Prefixes;

    public static PrefixResult ListAllPrefixesAndCorePrefixes(
      string start,
      string end,
      int? totalLength = null)
    {
      if (string.CompareOrdinal(start, end) > 0)
        throw new ArgumentException("Couldn't list prefixes within the given range, which has a starting boundary (" + start + ") > ending boundary (" + end + ").");
      List<string> stringList1 = new List<string>();
      int length1 = start.Length;
      int length2 = end.Length;
      int num1 = Math.Min(length1, length2);
      int num2 = totalLength.HasValue ? totalLength.Value : Math.Max(length1, length2);
      int lastCommonIndex = -1;
      for (int index = 0; index < num1 && (int) start[index] == (int) end[index]; ++index)
        lastCommonIndex = index;
      ShardingRangePrefixLister.PopulatePrefixes(stringList1, lastCommonIndex, start, length1, true);
      int num3 = lastCommonIndex + 1;
      string str1 = start.Substring(0, num3);
      if (num3 < num2)
      {
        int startChar = num3 < length1 ? (int) start[num3] : 0;
        char ch = num3 < length2 ? end[num3] : '~';
        bool flag = lastCommonIndex == length1 - 2;
        int num4 = flag ? 1 : 0;
        foreach (char charSequence in ShardingRangePrefixLister.GetCharSequences((char) startChar, true, num4 != 0))
        {
          if ((int) charSequence < (int) ch || flag && (int) charSequence == (int) ch)
          {
            string str2 = str1 + charSequence.ToString();
            stringList1.Add(str2);
          }
        }
      }
      else
        stringList1.Add(str1);
      List<string> stringList2 = new List<string>();
      ShardingRangePrefixLister.PopulatePrefixes(stringList2, lastCommonIndex, end, length2, false);
      stringList2.Reverse();
      stringList1.AddRange((IEnumerable<string>) stringList2);
      List<string> corePrefixes = new List<string>();
      List<string> stringList3 = new List<string>((IEnumerable<string>) stringList1);
      stringList3.Sort((IComparer<string>) LengthComparer.Instance);
      int num5 = 0;
      int num6 = 0;
      foreach (string str3 in stringList3)
      {
        int length3 = str3.Length;
        if (length3 > num6)
        {
          ++num5;
          if (num5 <= 2)
            num6 = length3;
          else
            break;
        }
        corePrefixes.Add(str3);
      }
      corePrefixes.Sort((IComparer<string>) StringComparer.InvariantCultureIgnoreCase);
      return new PrefixResult(stringList1, corePrefixes, start, end);
    }

    private static void PopulatePrefixes(
      List<string> list,
      int lastCommonIndex,
      string rangeBorderString,
      int rangeBorderStringLength,
      bool higherOrLower)
    {
      char ch = higherOrLower ? '0' : 'F';
      bool flag = true;
      int num = rangeBorderStringLength - 2;
      for (int index = num; index > lastCommonIndex; --index)
      {
        string str1 = rangeBorderString.Substring(0, index + 1);
        char startChar = rangeBorderString[index + 1];
        if (flag && (int) startChar == (int) ch)
        {
          --num;
          if (num == lastCommonIndex)
            list.Add(str1);
        }
        else
        {
          flag = false;
          foreach (char charSequence in ShardingRangePrefixLister.GetCharSequences(startChar, higherOrLower, index == num))
          {
            string str2 = str1 + charSequence.ToString();
            list.Add(str2);
          }
        }
      }
    }

    private static IEnumerable<char> GetCharSequences(
      char startChar,
      bool higherOrLower,
      bool inclusive)
    {
      IEnumerable<char> source = (IEnumerable<char>) ShardingRangePrefixLister.chars;
      if (!higherOrLower)
        source = source.Reverse<char>();
      foreach (char charSequence in source)
      {
        if (higherOrLower && (int) charSequence > (int) startChar)
          yield return charSequence;
        else if (!higherOrLower && (int) charSequence < (int) startChar)
          yield return charSequence;
        else if (inclusive && (int) charSequence == (int) startChar)
          yield return charSequence;
      }
    }
  }
}
