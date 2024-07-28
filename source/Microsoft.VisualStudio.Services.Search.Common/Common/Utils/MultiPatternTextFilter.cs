// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Common.Utils.MultiPatternTextFilter
// Assembly: Microsoft.VisualStudio.Services.Search.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8E09DCBA-148E-4EB7-BB73-B53B030BE93E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Common.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Microsoft.VisualStudio.Services.Search.Common.Utils
{
  public class MultiPatternTextFilter
  {
    private readonly List<MultiPatternTextFilter.FilterCriterion> m_filterCriteria;

    public MultiPatternTextFilter(string inclusionExclusionList, char separator = ',', bool ignoreCase = true)
    {
      if (inclusionExclusionList == null)
        throw new ArgumentNullException(nameof (inclusionExclusionList));
      IEnumerable<string> strings = ((IEnumerable<string>) inclusionExclusionList.Split(new char[1]
      {
        separator
      }, StringSplitOptions.RemoveEmptyEntries)).Select<string, string>((Func<string, string>) (text => text.Trim()));
      this.m_filterCriteria = new List<MultiPatternTextFilter.FilterCriterion>();
      RegexOptions options = RegexOptions.Compiled;
      if (ignoreCase)
        options |= RegexOptions.IgnoreCase;
      foreach (string str1 in strings)
      {
        bool flag;
        string str2;
        if (str1[0] == '-')
        {
          flag = true;
          str2 = str1.Substring(1).Trim();
        }
        else if (str1[0] == '+')
        {
          flag = false;
          str2 = str1.Substring(1).Trim();
        }
        else
        {
          flag = false;
          str2 = str1;
        }
        this.m_filterCriteria.Add(new MultiPatternTextFilter.FilterCriterion(new Regex(("^" + str2 + "$").Replace(".", "\\.").Replace("*", ".*").Replace("?", "."), options), !flag));
      }
    }

    public bool IsMatch(string text)
    {
      foreach (MultiPatternTextFilter.FilterCriterion filterCriterion in this.m_filterCriteria)
      {
        if (!filterCriterion.IsMatch(text))
          return false;
      }
      return true;
    }

    private class FilterCriterion
    {
      private readonly bool m_shouldMatch;
      private readonly Regex m_pattern;

      public FilterCriterion(Regex pattern, bool shouldMatch)
      {
        this.m_pattern = pattern;
        this.m_shouldMatch = shouldMatch;
      }

      public bool IsMatch(string text) => this.m_shouldMatch ? this.m_pattern.IsMatch(text) : !this.m_pattern.IsMatch(text);
    }
  }
}
