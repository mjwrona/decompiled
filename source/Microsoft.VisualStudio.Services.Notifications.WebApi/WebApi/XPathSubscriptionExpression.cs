// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.WebApi.XPathSubscriptionExpression
// Assembly: Microsoft.VisualStudio.Services.Notifications.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FF217E0A-7730-437B-BE9F-877363CB7392
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.WebApi.dll

using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Microsoft.VisualStudio.Services.Notifications.WebApi
{
  public class XPathSubscriptionExpression : PathSubscriptionExpression
  {
    private static Dictionary<FieldFilterType, Regex> s_filterRegExs = new Dictionary<FieldFilterType, Regex>();

    public XPathSubscriptionExpression()
    {
      this.FilterName = (Token) new XPathToken();
      this.FilterValue = (Token) new XPathToken();
    }

    static XPathSubscriptionExpression()
    {
      XPathSubscriptionExpression.s_filterRegExs.Add(FieldFilterType.NotStartsWith, new Regex("not\\(starts-with\\((?<caseSensitive>translate\\()?(?<name>@?[^\\s,]+)(,\\s*[\\'\\\"][^\\'\\\"]*[\\'\\\"],\\s*[\\'\\\"][^\\']*[\\'\\\"]\\))?,\\s*[\\'\\\"](?<value>[^\\'\\\"]*)[\\'\\\"]\\)\\)", RegexOptions.Compiled));
      XPathSubscriptionExpression.s_filterRegExs.Add(FieldFilterType.NotEndsWith, new Regex("not\\(starts-with\\(substring\\((?<caseSensitive>translate\\()(?<name>@?[^\\s,]+),\\s*[\\'\\\"][^\\'\\\"]*[\\'\\\"],\\s*[\\'\\\"][^\\'\\\"]*[\\'\\\"]\\),\\s*1\\s*\\+\\s*string-length\\(@.*\\)\\s*-\\s*string-length\\([\\'\\\"][^\\'\\\"]*[\\'\\\"]\\)\\),\\s*[\\'\\\"](?<value>[^\\'\\\"]*)[\\'\\\"]\\)\\)", RegexOptions.Compiled));
      XPathSubscriptionExpression.s_filterRegExs.Add(FieldFilterType.StartsWith, new Regex("starts-with\\((?<caseSensitive>translate\\()?(?<name>@?[^\\s,]+)(,\\s*[\\'\\\"][^\\'\\\"]*[\\'\\\"],\\s*[\\'\\\"][^\\']*[\\'\\\"]\\))?,\\s*[\\'\\\"](?<value>[^\\'\\\"]*)[\\'\\\"]\\)", RegexOptions.Compiled));
      XPathSubscriptionExpression.s_filterRegExs.Add(FieldFilterType.EndsWith, new Regex("starts-with\\(substring\\((?<caseSensitive>translate\\()(?<name>@?[^\\s,]+),\\s*[\\'\\\"][^\\'\\\"]*[\\'\\\"],\\s*[\\'\\\"][^\\'\\\"]*[\\'\\\"]\\),\\s*1\\s*\\+\\s*string-length\\(@.*\\)\\s*-\\s*string-length\\([\\'\\\"][^\\'\\\"]*[\\'\\\"]\\)\\),\\s*[\\'\\\"](?<value>[^\\'\\\"]*)[\\'\\\"]\\)", RegexOptions.Compiled));
      XPathSubscriptionExpression.s_filterRegExs.Add(FieldFilterType.NotContains, new Regex("not\\(contains\\((?<caseSensitive>translate\\()?(?<name>@?[^\\s,]+)(,\\s*[\\'\\\"][^\\'\\\"]*[\\'\\\"],\\s*[\\'\\\"][^\\']*[\\'\\\"]\\))?,\\s*[\\'\\\"](?<value>[^\\'\\\"]*)[\\'\\\"]\\)\\)", RegexOptions.Compiled));
      XPathSubscriptionExpression.s_filterRegExs.Add(FieldFilterType.Contains, new Regex("contains\\((?<caseSensitive>translate\\()?(?<name>@?[^\\s,]+)(,\\s*[\\'\\\"][^\\'\\\"]*[\\'\\\"],\\s*[\\'\\\"][^\\']*[\\'\\\"]\\))?,\\s*[\\'\\\"](?<value>[^\\'\\\"]*)[\\'\\\"]\\)", RegexOptions.Compiled));
      XPathSubscriptionExpression.s_filterRegExs.Add(FieldFilterType.CountGT, new Regex("count\\((?<name>.+)\\)\\s*>\\s*(?<value>\\d+)", RegexOptions.Compiled));
      XPathSubscriptionExpression.s_filterRegExs.Add(FieldFilterType.CountEqualTo, new Regex("count\\((?<name>.+)\\)\\s*=\\s*(?<value>\\d+)", RegexOptions.Compiled));
      XPathSubscriptionExpression.s_filterRegExs.Add(FieldFilterType.NotEquals, new Regex("not\\((?<caseSensitive>translate\\()?(?<name>@?[^\\s,]+)(,\\s*[\\'\\\"][^\\'\\\"]*[\\'\\\"],\\s*[\\'\\\"][^\\']*[\\'\\\"]\\))?\\s*=\\s*[\\'\\\"](?<value>[^\\'\\\"]*)[\\'\\\"]\\)", RegexOptions.Compiled));
      XPathSubscriptionExpression.s_filterRegExs.Add(FieldFilterType.Equals, new Regex("(?<caseSensitive>translate\\()?(?<name>@?[^\\s,]+)(,\\s*[\\'\\\"][^\\'\\\"]*[\\'\\\"],\\s*[\\'\\\"][^\\']*[\\'\\\"]\\))?\\s*=\\s*[\\'\\\"](?<value>[^\\'\\\"]*)[\\'\\\"]", RegexOptions.Compiled));
    }

    public static XPathSubscriptionExpression Parse(string xpath)
    {
      XPathSubscriptionExpression subscriptionExpression = new XPathSubscriptionExpression();
      string path;
      string filter;
      string postFilterPath;
      if (!XPathSubscriptionExpression.ParseLastXPathFilter(xpath, out path, out filter, out postFilterPath))
      {
        subscriptionExpression.Path = xpath;
        subscriptionExpression.FilterType = FieldFilterType.None;
      }
      else
      {
        subscriptionExpression.Path = path;
        subscriptionExpression.PostFilterPath = postFilterPath;
        subscriptionExpression.FilterType = FieldFilterType.NoOperator;
        subscriptionExpression.FilterName = (Token) new XPathToken(filter);
        string pathFunctions = subscriptionExpression.FilterName.EvaluatePathFunctions();
        foreach (FieldFilterType key in XPathSubscriptionExpression.s_filterRegExs.Keys)
        {
          Match match = XPathSubscriptionExpression.s_filterRegExs[key].Match(pathFunctions);
          if (match.Success)
          {
            subscriptionExpression.FilterType = key;
            subscriptionExpression.FilterName = (Token) new XPathToken(match.Groups["name"].Value);
            subscriptionExpression.FilterValue = (Token) new XPathToken(match.Groups["value"].Value);
            subscriptionExpression.FilterNameIgnoreCase = match.Groups["caseSensitive"] != null && !string.IsNullOrEmpty(match.Groups["caseSensitive"].Value);
            break;
          }
        }
      }
      return subscriptionExpression;
    }

    public override PathSubscriptionExpression ParsePath(string xpath) => (PathSubscriptionExpression) XPathSubscriptionExpression.Parse(xpath);

    public override string ToPath()
    {
      string str1 = this.UseSingleQuoteChar ? "'" : "\"";
      string str2 = this.FilterValue.EscapeSpecialChatactersIfNeeded(this.UseSingleQuoteChar);
      string path1 = this.Path;
      string path2;
      switch (this.FilterType)
      {
        case FieldFilterType.NoOperator:
          path2 = string.Format("{0}[{1}]{2}", (object) this.Path, (object) this.FilterName, (object) this.PostFilterPath);
          break;
        case FieldFilterType.Equals:
          if (this.FilterNameIgnoreCase)
          {
            XPathSubscriptionExpression.Alphabet alphabet = XPathSubscriptionExpression.GetAlphabet(str2);
            path2 = string.Format("{1}[translate({2}, {0}{5}{0}, {0}{6}{0}) = {3}]{4}", (object) str1, (object) this.Path, (object) this.FilterName, (object) str2.ToLower(), (object) this.PostFilterPath, (object) alphabet.AlphabetUpperCase, (object) alphabet.AlphabetLowerCase);
            break;
          }
          path2 = string.Format("{1}[{2}={3}]{4}", (object) str1, (object) this.Path, (object) this.FilterName, (object) str2, (object) this.PostFilterPath);
          break;
        case FieldFilterType.NotEquals:
          if (this.FilterNameIgnoreCase)
          {
            XPathSubscriptionExpression.Alphabet alphabet = XPathSubscriptionExpression.GetAlphabet(str2);
            path2 = string.Format("{1}[not(translate({2}, {0}{5}{0}, {0}{6}{0}) = {3})]{4}", (object) str1, (object) this.Path, (object) this.FilterName, (object) str2.ToLower(), (object) this.PostFilterPath, (object) alphabet.AlphabetUpperCase, (object) alphabet.AlphabetLowerCase);
            break;
          }
          path2 = string.Format("{1}[not({2}={3})]{4}", (object) str1, (object) this.Path, (object) this.FilterName, (object) str2, (object) this.PostFilterPath);
          break;
        case FieldFilterType.StartsWith:
          if (this.FilterNameIgnoreCase)
          {
            XPathSubscriptionExpression.Alphabet alphabet = XPathSubscriptionExpression.GetAlphabet(str2);
            path2 = string.Format("{1}[starts-with(translate({2}, {0}{5}{0}, {0}{6}{0}), {3})]{4}", (object) str1, (object) this.Path, (object) this.FilterName, (object) str2.ToLower(), (object) this.PostFilterPath, (object) alphabet.AlphabetUpperCase, (object) alphabet.AlphabetLowerCase);
            break;
          }
          path2 = string.Format("{1}[starts-with({2}, {3})]{4}", (object) str1, (object) this.Path, (object) this.FilterName, (object) str2, (object) this.PostFilterPath);
          break;
        case FieldFilterType.NotStartsWith:
          if (this.FilterNameIgnoreCase)
          {
            XPathSubscriptionExpression.Alphabet alphabet = XPathSubscriptionExpression.GetAlphabet(str2);
            path2 = string.Format("{1}[not(starts-with(translate({2}, {0}{5}{0}, {0}{6}{0}), {3}))]{4}", (object) str1, (object) this.Path, (object) this.FilterName, (object) str2.ToLower(), (object) this.PostFilterPath, (object) alphabet.AlphabetUpperCase, (object) alphabet.AlphabetLowerCase);
            break;
          }
          path2 = string.Format("{1}[not(starts-with({2}, {3}))]{4}", (object) str1, (object) this.Path, (object) this.FilterName, (object) str2, (object) this.PostFilterPath);
          break;
        case FieldFilterType.EndsWith:
          if (this.FilterNameIgnoreCase)
          {
            XPathSubscriptionExpression.Alphabet alphabet = XPathSubscriptionExpression.GetAlphabet(str2);
            path2 = string.Format("{1}[starts-with(substring(translate({2}, {0}{5}{0}, {0}{6}{0}), 1 + string-length({2}) - string-length({3})), {3})]{4}", (object) str1, (object) this.Path, (object) this.FilterName, (object) str2.ToLower(), (object) this.PostFilterPath, (object) alphabet.AlphabetUpperCase, (object) alphabet.AlphabetLowerCase);
            break;
          }
          path2 = string.Format("{1}[starts-with(substring({2}, 1 + string-length({2}) - string-length({3})), {3})]{4}", (object) str1, (object) this.Path, (object) this.FilterName, (object) str2, (object) this.PostFilterPath);
          break;
        case FieldFilterType.NotEndsWith:
          if (this.FilterNameIgnoreCase)
          {
            XPathSubscriptionExpression.Alphabet alphabet = XPathSubscriptionExpression.GetAlphabet(str2);
            path2 = string.Format("{1}[not(starts-with(substring(translate({2}, {0}{5}{0}, {0}{6}{0}), 1 + string-length({2}) - string-length({3})), {3}))]{4}", (object) str1, (object) this.Path, (object) this.FilterName, (object) str2.ToLower(), (object) this.PostFilterPath, (object) alphabet.AlphabetUpperCase, (object) alphabet.AlphabetLowerCase);
            break;
          }
          path2 = string.Format("{1}[not(starts-with(substring({2}, 1 + string-length({2}) - string-length({3})), {3}))]{4}", (object) str1, (object) this.Path, (object) this.FilterName, (object) str2, (object) this.PostFilterPath);
          break;
        case FieldFilterType.Contains:
          if (this.FilterNameIgnoreCase)
          {
            XPathSubscriptionExpression.Alphabet alphabet = XPathSubscriptionExpression.GetAlphabet(str2);
            path2 = string.Format("{1}[contains(translate({2}, {0}{5}{0}, {0}{6}{0}), {3})]{4}", (object) str1, (object) this.Path, (object) this.FilterName, (object) str2.ToLower(), (object) this.PostFilterPath, (object) alphabet.AlphabetUpperCase, (object) alphabet.AlphabetLowerCase);
            break;
          }
          path2 = string.Format("{1}[contains({2}, {3})]{4}", (object) str1, (object) this.Path, (object) this.FilterName, (object) str2, (object) this.PostFilterPath);
          break;
        case FieldFilterType.NotContains:
          if (this.FilterNameIgnoreCase)
          {
            XPathSubscriptionExpression.Alphabet alphabet = XPathSubscriptionExpression.GetAlphabet(str2);
            path2 = string.Format("{1}[not(contains(translate({2}, {0}{5}{0}, {0}{6}{0}), {3}))]{4}", (object) str1, (object) this.Path, (object) this.FilterName, (object) str2.ToLower(), (object) this.PostFilterPath, (object) alphabet.AlphabetUpperCase, (object) alphabet.AlphabetLowerCase);
            break;
          }
          path2 = string.Format("{1}[not(contains({2}, {3}))]{4}", (object) str1, (object) this.Path, (object) this.FilterName, (object) str2, (object) this.PostFilterPath);
          break;
        case FieldFilterType.CountGT:
          path2 = string.Format("{1}[count({2}) > {3}]{4}", (object) str1, (object) this.Path, (object) this.FilterName, (object) this.FilterValue.Spelling, (object) this.PostFilterPath);
          break;
        case FieldFilterType.CountEqualTo:
          path2 = string.Format("{1}[count({2}) = {3}]{4}", (object) str1, (object) this.Path, (object) this.FilterName, (object) this.FilterValue.Spelling, (object) this.PostFilterPath);
          break;
        case FieldFilterType.AttributeEquals:
          path2 = string.Format("{1}[@{2}={3}]{4}", (object) str1, (object) this.Path, (object) this.FilterName, (object) str2, (object) this.PostFilterPath);
          break;
        default:
          path2 = this.Path;
          break;
      }
      return path2;
    }

    public static bool ParseLastXPathFilter(
      string xpath,
      out string path,
      out string filter,
      out string postFilterPath)
    {
      int num1 = xpath.LastIndexOf(']');
      if (num1 > 0)
      {
        int length = -1;
        int num2 = 0;
        for (int index = num1 - 1; index >= 0; --index)
        {
          if (xpath[index] == ']')
            ++num2;
          else if (xpath[index] == '[' && num2-- == 0)
          {
            length = index;
            break;
          }
        }
        if (length >= 0)
        {
          filter = xpath.Substring(length + 1, num1 - length - 1);
          path = xpath.Substring(0, length);
          postFilterPath = xpath.Substring(num1 + 1);
          return true;
        }
      }
      path = (string) null;
      filter = (string) null;
      postFilterPath = (string) null;
      return false;
    }

    private static XPathSubscriptionExpression.Alphabet GetAlphabet(string value)
    {
      HashSet<char> source = new HashSet<char>();
      foreach (char c in value)
      {
        if (char.IsLetter(c))
          source.Add(char.ToLower(c));
      }
      if (source.Any<char>())
      {
        XPathSubscriptionExpression.Alphabet alphabet = new XPathSubscriptionExpression.Alphabet()
        {
          AlphabetLowerCase = new string(source.ToArray<char>())
        };
        alphabet.AlphabetUpperCase = alphabet.AlphabetLowerCase.ToUpper();
        return alphabet;
      }
      return new XPathSubscriptionExpression.Alphabet()
      {
        AlphabetLowerCase = "abcdefghijklmnopqrstuvwxyz",
        AlphabetUpperCase = "ABCDEFGHIJKLMNOPQRSTUVWXYZ"
      };
    }

    private struct Alphabet
    {
      public string AlphabetLowerCase;
      public string AlphabetUpperCase;
    }
  }
}
