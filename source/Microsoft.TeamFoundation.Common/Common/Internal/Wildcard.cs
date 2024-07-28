// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Common.Internal.Wildcard
// Assembly: Microsoft.TeamFoundation.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E643B42-FE11-4FC2-A9D6-79417E26CF92
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Common.dll

using System;
using System.ComponentModel;

namespace Microsoft.TeamFoundation.Common.Internal
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public static class Wildcard
  {
    private static readonly char[] wildcardChars = new char[2]
    {
      '*',
      '?'
    };

    public static bool Match(string str, string pattern) => Wildcard.Match(str, 0, pattern, 0);

    public static bool Match(string str, string pattern, StringComparison comparer) => Wildcard.Match(str, 0, pattern, 0, comparer);

    public static bool IsWildcard(string str) => str.IndexOfAny(Wildcard.wildcardChars) >= 0;

    public static bool IsWildcard(string str, int index, int count) => str.IndexOfAny(Wildcard.wildcardChars, index, count) >= 0;

    public static bool IsWildcard(char c) => c == '*' || c == '?';

    public static bool Match(string itemName, int itemIndex, string matchPattern) => Wildcard.Match(itemName, itemIndex, matchPattern, 0);

    private static bool Match(
      string itemName,
      int itemIndex,
      string matchPattern,
      int matchIndex,
      StringComparison comparer = StringComparison.OrdinalIgnoreCase)
    {
      for (; matchIndex < matchPattern.Length; ++matchIndex)
      {
        if (matchPattern[matchIndex] == '*')
        {
          do
          {
            ++matchIndex;
          }
          while (matchIndex < matchPattern.Length && matchPattern[matchIndex] == '*');
          for (; !Wildcard.Match(itemName, itemIndex, matchPattern, matchIndex, comparer); ++itemIndex)
          {
            if (itemIndex == itemName.Length || itemName[itemIndex] == '.' && itemName.LastIndexOf('.', itemName.Length - 1) == itemIndex && matchPattern.Length == matchIndex + 1 && matchPattern[matchIndex] == '.')
              return false;
          }
          return true;
        }
        if (itemIndex == itemName.Length)
          return matchPattern[matchIndex] == '.' && matchPattern.LastIndexOf('.', matchPattern.Length - 1) == matchIndex && Wildcard.Match(itemName, itemIndex, matchPattern, matchIndex + 1, comparer);
        if (string.Compare(itemName, itemIndex, matchPattern, matchIndex, 1, comparer) != 0 && matchPattern[matchIndex] != '?')
          return false;
        ++itemIndex;
      }
      if (itemIndex == itemName.Length)
        return true;
      return itemName[itemIndex] == '.' && itemIndex + 1 == itemName.Length;
    }
  }
}
