// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.SoapService.Galleries.FilterTranslator
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Microsoft.VisualStudio.Services.Gallery.Server.SoapService.Galleries
{
  public class FilterTranslator : IFilterTranslator
  {
    private Dictionary<string, string> filterNameTranslations;
    private Dictionary<Regex, Func<string[], string>> filterNameRegexTranslations;
    private Dictionary<string, Func<FilterExpression, string>> filterExpressionTranslations;
    private Dictionary<Regex, Func<FilterExpression, string[], string>> filterExpressionRegexTranslations;
    private static readonly Regex filterExpressionRegex = new Regex("([a-z0-9\\.]+|[a-z0-9\\.]+metadata\\[\\'[^']+\\'\\])(\\s*<=\\s*|\\s*=\\s*|\\s*!=\\s*|\\s+LIKE\\s+|\\s+IS\\s+)(?:\\'([^']*)\\'|([0-9]+|NULL))", RegexOptions.IgnoreCase | RegexOptions.Compiled);
    private static readonly Regex isFilterSafeRegex = new Regex("(?ix)\r\n(?:\r\n  [a-z]+[a-z0-9.]*(?:\\['[a-z0-9.]*'\\])?            #identifier\r\n  (?:  \\s*(?:<?!?=|like)\\s*(?:'[\\S ]*'|[0-9]+) #operator and value\r\n  |    \\s*is\\snull)                                #is null\r\n|\r\n  \\s|\\(|\\)|and|or                                  #ignored\r\n)*");

    public FilterTranslator()
    {
      this.filterNameTranslations = new Dictionary<string, string>();
      this.filterNameRegexTranslations = new Dictionary<Regex, Func<string[], string>>();
      this.filterExpressionTranslations = new Dictionary<string, Func<FilterExpression, string>>();
      this.filterExpressionRegexTranslations = new Dictionary<Regex, Func<FilterExpression, string[], string>>();
    }

    public void AddFilterNameTranslation(string filterName, string translation) => this.FilterNameTranslations.Add(filterName, translation);

    public void AddFilterNameTranslation(Regex filterNameRegex, Func<string[], string> translation) => this.FilterNameRegexTranslations.Add(filterNameRegex, translation);

    public void AddFilterExpressionTranslation(
      string filterName,
      Func<FilterExpression, string> translation)
    {
      this.FilterExpressionTranslations.Add(filterName, translation);
    }

    public void AddFilterExpressionTranslation(
      Regex filterNameRegex,
      Func<FilterExpression, string[], string> translation)
    {
      this.FilterExpressionRegexTranslations.Add(filterNameRegex, translation);
    }

    public IDictionary<string, string> FilterNameTranslations => (IDictionary<string, string>) this.filterNameTranslations;

    public IDictionary<Regex, Func<string[], string>> FilterNameRegexTranslations => (IDictionary<Regex, Func<string[], string>>) this.filterNameRegexTranslations;

    public IDictionary<string, Func<FilterExpression, string>> FilterExpressionTranslations => (IDictionary<string, Func<FilterExpression, string>>) this.filterExpressionTranslations;

    public IDictionary<Regex, Func<FilterExpression, string[], string>> FilterExpressionRegexTranslations => (IDictionary<Regex, Func<FilterExpression, string[], string>>) this.filterExpressionRegexTranslations;

    public string Translate(string filter)
    {
      string input = FilterTranslator.IsFilterSafe(filter) ? filter : throw new ArgumentException(string.Format("The filter {0} is invalid.", (object) filter), nameof (filter));
      int startat;
      for (Match match1 = FilterTranslator.filterExpressionRegex.Match(input); match1.Success; match1 = FilterTranslator.filterExpressionRegex.Match(input, startat))
      {
        startat = match1.Index + match1.Length;
        string filterName = match1.Groups[1].Value;
        string comparison = match1.Groups[2].Value.Trim();
        string str1 = match1.Groups[3].Value;
        if (!string.IsNullOrEmpty(match1.Groups[4].Value))
          str1 = match1.Groups[4].Value;
        KeyValuePair<string, Func<FilterExpression, string>> keyValuePair1 = this.filterExpressionTranslations.SingleOrDefault<KeyValuePair<string, Func<FilterExpression, string>>>((Func<KeyValuePair<string, Func<FilterExpression, string>>, bool>) (x => x.Key.Equals(filterName, StringComparison.OrdinalIgnoreCase)));
        KeyValuePair<string, string> keyValuePair2 = this.filterNameTranslations.SingleOrDefault<KeyValuePair<string, string>>((Func<KeyValuePair<string, string>, bool>) (x => x.Key.Equals(filterName, StringComparison.OrdinalIgnoreCase)));
        KeyValuePair<Regex, Func<string[], string>> keyValuePair3 = this.filterNameRegexTranslations.SingleOrDefault<KeyValuePair<Regex, Func<string[], string>>>((Func<KeyValuePair<Regex, Func<string[], string>>, bool>) (x => x.Key.IsMatch(filterName)));
        KeyValuePair<Regex, Func<FilterExpression, string[], string>> keyValuePair4 = this.filterExpressionRegexTranslations.SingleOrDefault<KeyValuePair<Regex, Func<FilterExpression, string[], string>>>((Func<KeyValuePair<Regex, Func<FilterExpression, string[], string>>, bool>) (x => x.Key.IsMatch(filterName)));
        if (keyValuePair1.Key != null)
        {
          string str2 = input.Remove(match1.Index, match1.Value.Length);
          string str3 = keyValuePair1.Value(new FilterExpression(filterName, comparison, str1));
          input = str2.Insert(match1.Index, str3);
          startat += str3.Length - match1.Length;
        }
        else if (keyValuePair2.Key != null)
        {
          Group group = match1.Groups[1];
          string str4 = input.Remove(group.Index, group.Value.Length);
          string str5 = keyValuePair2.Value;
          input = str4.Insert(group.Index, str5);
          startat += str5.Length - filterName.Length;
        }
        else if (keyValuePair4.Key != null)
        {
          Match match2 = keyValuePair4.Key.Match(filterName);
          List<string> stringList = new List<string>();
          for (int groupnum = 0; groupnum < match2.Groups.Count; ++groupnum)
            stringList.Add(match2.Groups[groupnum].Value);
          string str6 = input.Remove(match1.Index, match1.Value.Length);
          string str7 = keyValuePair4.Value(new FilterExpression(filterName, comparison, str1), stringList.ToArray());
          input = str6.Insert(match1.Index, str7);
          startat += str7.Length - match1.Length;
        }
        else if (keyValuePair3.Key != null)
        {
          Match match3 = keyValuePair3.Key.Match(filterName);
          List<string> stringList = new List<string>();
          for (int groupnum = 0; groupnum < match3.Groups.Count; ++groupnum)
            stringList.Add(match3.Groups[groupnum].Value);
          Group group = match1.Groups[1];
          string str8 = input.Remove(group.Index, group.Value.Length);
          string str9 = keyValuePair3.Value(stringList.ToArray());
          input = str8.Insert(group.Index, str9);
          startat += str9.Length - filterName.Length;
        }
      }
      return input;
    }

    private static bool IsFilterSafe(string filter)
    {
      Match match = FilterTranslator.isFilterSafeRegex.Match(filter);
      return match.Success && match.Length == filter.Length;
    }
  }
}
