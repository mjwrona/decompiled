// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.PatternUtility
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

namespace Microsoft.TeamFoundation.Framework.Server
{
  public static class PatternUtility
  {
    public static bool Match(string toMatch, string pattern)
    {
      if (string.IsNullOrEmpty(toMatch))
        return false;
      int index1 = 0;
      int index2 = 0;
      bool flag = false;
      while (index1 < pattern.Length)
      {
        if ((pattern[index1] == '<' || pattern[index1] == '>') && index1 + 1 < pattern.Length)
        {
          char ch = pattern[index1++];
          int startIndex1 = index1;
          while (index1 < pattern.Length && char.IsDigit(pattern[index1]) && index1 - startIndex1 <= 4)
            ++index1;
          if (index1 > startIndex1)
          {
            int result1;
            if (int.TryParse(pattern.Substring(startIndex1, index1 - startIndex1), out result1))
            {
              int startIndex2 = index2;
              while (index2 < toMatch.Length && char.IsDigit(toMatch[index2]) && index2 - startIndex2 <= 4)
                ++index2;
              int result2;
              if (index2 > startIndex2 && int.TryParse(toMatch.Substring(startIndex2, index2 - startIndex2), out result2) && (ch == '<' && result2 >= result1 || ch == '>' && result2 <= result1))
                return false;
              continue;
            }
            continue;
          }
          if (ch == '<' && pattern[index1] != '<' || ch == '>' && pattern[index1] != '>')
            --index1;
        }
        if (pattern[index1] == '*')
        {
          ++index1;
          flag = true;
        }
        else
        {
          if (index2 >= toMatch.Length)
            return false;
          if ((int) pattern[index1] == (int) toMatch[index2] || pattern[index1] == '?')
          {
            ++index1;
            flag = false;
          }
          else if (!flag)
            return false;
          ++index2;
        }
      }
      return toMatch.Length == index2 | flag;
    }
  }
}
