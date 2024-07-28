// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.Utility.TyposquattingDistanceCalculation
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Microsoft.VisualStudio.Services.Gallery.Server.Utility
{
  public static class TyposquattingDistanceCalculation
  {
    private const char PlaceholderForAlignment = '*';
    private static readonly HashSet<char> SpecialCharacters = new HashSet<char>()
    {
      '.',
      '_',
      '-'
    };
    private static readonly string SpecialCharactersToString = "[" + new string(TyposquattingDistanceCalculation.SpecialCharacters.ToArray<char>()) + "]";

    public static bool IsDistanceLessThanOrEqualToThreshold(
      string str1,
      string str2,
      int threshold)
    {
      if (str1 == null)
        throw new ArgumentNullException(nameof (str1));
      if (str2 == null)
        throw new ArgumentNullException(nameof (str2));
      return Math.Abs(RegexEx.ReplaceWithTimeout(str1, TyposquattingDistanceCalculation.SpecialCharactersToString, string.Empty, RegexOptions.None).Length - RegexEx.ReplaceWithTimeout(str2, TyposquattingDistanceCalculation.SpecialCharactersToString, string.Empty, RegexOptions.None).Length) <= threshold && TyposquattingDistanceCalculation.GetDistance(str1, str2, threshold) <= threshold;
    }

    private static int GetDistance(string str1, string str2, int threshold)
    {
      TyposquattingDistanceCalculation.BasicEditDistanceInfo distanceWithPath = TyposquattingDistanceCalculation.GetBasicEditDistanceWithPath(str1, str2);
      if (distanceWithPath.Distance <= threshold)
        return distanceWithPath.Distance;
      string[] strArray = TyposquattingDistanceCalculation.TraceBackAndAlignStrings(distanceWithPath.Path, str1, str2);
      return TyposquattingDistanceCalculation.RefreshEditDistance(strArray[0], strArray[1], distanceWithPath.Distance);
    }

    private static TyposquattingDistanceCalculation.BasicEditDistanceInfo GetBasicEditDistanceWithPath(
      string str1,
      string str2)
    {
      int[,] numArray = new int[str1.Length + 1, str2.Length + 1];
      TyposquattingDistanceCalculation.PathInfo[,] pathInfoArray = new TyposquattingDistanceCalculation.PathInfo[str1.Length + 1, str2.Length + 1];
      numArray[0, 0] = 0;
      pathInfoArray[0, 0] = TyposquattingDistanceCalculation.PathInfo.Match;
      for (int index = 1; index <= str1.Length; ++index)
      {
        numArray[index, 0] = index;
        pathInfoArray[index, 0] = TyposquattingDistanceCalculation.PathInfo.Delete;
      }
      for (int index = 1; index <= str2.Length; ++index)
      {
        numArray[0, index] = index;
        pathInfoArray[0, index] = TyposquattingDistanceCalculation.PathInfo.Insert;
      }
      for (int index1 = 1; index1 <= str1.Length; ++index1)
      {
        for (int index2 = 1; index2 <= str2.Length; ++index2)
        {
          if ((int) str1[index1 - 1] == (int) str2[index2 - 1])
          {
            numArray[index1, index2] = numArray[index1 - 1, index2 - 1];
            pathInfoArray[index1, index2] = TyposquattingDistanceCalculation.PathInfo.Match;
          }
          else
          {
            numArray[index1, index2] = numArray[index1 - 1, index2 - 1] + 1;
            pathInfoArray[index1, index2] = TyposquattingDistanceCalculation.PathInfo.Substitute;
            if (numArray[index1 - 1, index2] + 1 < numArray[index1, index2])
            {
              numArray[index1, index2] = numArray[index1 - 1, index2] + 1;
              pathInfoArray[index1, index2] = TyposquattingDistanceCalculation.PathInfo.Delete;
            }
            if (numArray[index1, index2 - 1] + 1 < numArray[index1, index2])
            {
              numArray[index1, index2] = numArray[index1, index2 - 1] + 1;
              pathInfoArray[index1, index2] = TyposquattingDistanceCalculation.PathInfo.Insert;
            }
          }
        }
      }
      return new TyposquattingDistanceCalculation.BasicEditDistanceInfo()
      {
        Distance = numArray[str1.Length, str2.Length],
        Path = pathInfoArray
      };
    }

    private static string[] TraceBackAndAlignStrings(
      TyposquattingDistanceCalculation.PathInfo[,] path,
      string str1,
      string str2)
    {
      StringBuilder stringBuilder1 = new StringBuilder(str1);
      StringBuilder stringBuilder2 = new StringBuilder(str2);
      string[] strArray = new string[2];
      int length1 = str1.Length;
      int length2 = str2.Length;
      while (length1 > 0 && length2 > 0)
      {
        switch (path[length1, length2])
        {
          case TyposquattingDistanceCalculation.PathInfo.Match:
            --length1;
            --length2;
            continue;
          case TyposquattingDistanceCalculation.PathInfo.Delete:
            stringBuilder2.Insert(length2, '*');
            --length1;
            continue;
          case TyposquattingDistanceCalculation.PathInfo.Substitute:
            --length1;
            --length2;
            continue;
          case TyposquattingDistanceCalculation.PathInfo.Insert:
            stringBuilder1.Insert(length1, '*');
            --length2;
            continue;
          default:
            throw new ArgumentException("Invalidate operation for edit distance trace back: " + path[length1, length2].ToString());
        }
      }
      for (int index = 0; index < length1; ++index)
        stringBuilder2.Insert(index, '*');
      for (int index = 0; index < length2; ++index)
        stringBuilder1.Insert(index, '*');
      strArray[0] = stringBuilder1.ToString();
      strArray[1] = stringBuilder2.ToString();
      return strArray;
    }

    private static int RefreshEditDistance(
      string alignedStr1,
      string alignedStr2,
      int basicEditDistance)
    {
      if (alignedStr1.Length != alignedStr2.Length)
        throw new ArgumentException("The lengths of two aligned strings are not same!");
      int num = 0;
      for (int index = 0; index < alignedStr2.Length; ++index)
      {
        if ((int) alignedStr1[index] != (int) alignedStr2[index])
        {
          if (alignedStr1[index] == '*' && TyposquattingDistanceCalculation.SpecialCharacters.Contains(alignedStr2[index]))
            ++num;
          else if (alignedStr2[index] == '*' && TyposquattingDistanceCalculation.SpecialCharacters.Contains(alignedStr1[index]))
            ++num;
        }
      }
      return basicEditDistance - num;
    }

    private class BasicEditDistanceInfo
    {
      public int Distance { get; set; }

      public TyposquattingDistanceCalculation.PathInfo[,] Path { get; set; }
    }

    private enum PathInfo
    {
      Match,
      Delete,
      Substitute,
      Insert,
    }
  }
}
