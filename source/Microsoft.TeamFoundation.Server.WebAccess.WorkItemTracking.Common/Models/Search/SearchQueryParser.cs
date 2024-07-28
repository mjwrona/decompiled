// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.Search.SearchQueryParser
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using Microsoft.TeamFoundation.WorkItemTracking.Common.Search;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.Search
{
  internal class SearchQueryParser
  {
    public static bool IsQuote(char ch, UnicodeCategory category)
    {
      if (ch == '"' || category == UnicodeCategory.InitialQuotePunctuation || category == UnicodeCategory.FinalQuotePunctuation || ch == '＂' || ch == '＇' || ch >= '‘' && ch <= '‟' || ch >= '「' && ch <= '』')
        return true;
      return ch >= '﹁' && ch <= '﹄';
    }

    public static bool IsSpace(char ch, UnicodeCategory category) => ch == ' ' || category == UnicodeCategory.SpaceSeparator;

    public static bool IsEscape(char ch) => ch == '\\';

    public static bool IsExcludeFilter(char ch) => ch == '-';

    public static bool IsFilterSeparator(char ch) => ch == ':' || ch == '=';

    public static IEnumerable<SearchToken> ParseSearchString(string searchString)
    {
      int tokenStart = -1;
      bool quoteMode = false;
      int length = searchString.Length;
      StringBuilder stringBuilder = new StringBuilder(length);
      List<SearchToken> searchString1 = new List<SearchToken>();
      for (int index = 0; index < length; ++index)
      {
        char ch = searchString[index];
        UnicodeCategory unicodeCategory = char.GetUnicodeCategory(ch);
        if (SearchQueryParser.IsSpace(ch, unicodeCategory))
        {
          if (quoteMode)
            stringBuilder.Append(ch);
          else if (tokenStart != -1)
          {
            searchString1.Add(SearchQueryParser.ParseTokenString(searchString, tokenStart, index - tokenStart, stringBuilder.ToString(), quoteMode));
            tokenStart = -1;
            stringBuilder.Clear();
          }
        }
        else
        {
          if (tokenStart == -1)
            tokenStart = index;
          if (SearchQueryParser.IsQuote(ch, unicodeCategory))
          {
            quoteMode = !quoteMode;
          }
          else
          {
            stringBuilder.Append(ch);
            if (SearchQueryParser.IsEscape(ch) && index + 1 < length)
              stringBuilder.Append(searchString[++index]);
          }
        }
      }
      if (tokenStart != -1)
        searchString1.Add(SearchQueryParser.ParseTokenString(searchString, tokenStart, length - tokenStart, stringBuilder.ToString(), quoteMode));
      return (IEnumerable<SearchToken>) searchString1;
    }

    private static SearchToken ParseTokenString(
      string searchString,
      int tokenStart,
      int tokenLength,
      string unquotedToken,
      bool quoteMode)
    {
      string str = searchString.Substring(tokenStart, tokenLength);
      SearchParseError parseError = SearchParseError.SPE_NONE;
      if (quoteMode)
        parseError |= SearchParseError.SPE_UNMATCHEDQUOTES;
      bool invalidEscape1 = false;
      string parsedTokenText = SearchQueryParser.UnescapeString(unquotedToken, out invalidEscape1);
      if (invalidEscape1)
        parseError |= SearchParseError.SPE_INVALIDESCAPE;
      int filterSeparator1 = SearchQueryParser.FindFilterSeparator(unquotedToken);
      SearchToken tokenString;
      if (filterSeparator1 == -1)
      {
        tokenString = new SearchToken(str, (uint) tokenStart, parsedTokenText, parseError);
      }
      else
      {
        SearchFilterTokenType filterTokenType = SearchFilterTokenType.SFTT_DEFAULT;
        if (unquotedToken[filterSeparator1] == '=')
          filterTokenType |= SearchFilterTokenType.SFTT_EXACTMATCH;
        bool invalidEscape2 = false;
        string filterField = SearchQueryParser.UnescapeString(unquotedToken.Substring(0, filterSeparator1), out invalidEscape2);
        bool invalidEscape3 = false;
        string filterValue = SearchQueryParser.UnescapeString(unquotedToken.Substring(filterSeparator1 + 1), out invalidEscape3);
        if (invalidEscape2 | invalidEscape3)
          parseError |= SearchParseError.SPE_INVALIDESCAPE;
        if (string.IsNullOrWhiteSpace(filterField))
          parseError |= SearchParseError.SPE_EMPTYFILTERFIELD;
        else if (SearchQueryParser.IsExcludeFilter(filterField[0]))
        {
          filterTokenType |= SearchFilterTokenType.SFTT_EXCLUDE;
          filterField = filterField.Substring(1);
          if (string.IsNullOrWhiteSpace(filterField))
            parseError |= SearchParseError.SPE_EMPTYFILTERFIELD;
        }
        if (string.IsNullOrEmpty(filterValue))
          parseError |= SearchParseError.SPE_EMPTYFILTERVALUE;
        int filterSeparator2 = SearchQueryParser.FindFilterSeparator(str);
        tokenString = (SearchToken) new SearchFilterToken(str, (uint) tokenStart, parsedTokenText, filterField, filterValue, filterTokenType, (uint) filterSeparator2, parseError);
      }
      return tokenString;
    }

    public static int FindFilterSeparator(string tokenString)
    {
      int length = tokenString.Length;
      for (int index = 0; index < length; ++index)
      {
        char ch = tokenString[index];
        if (SearchQueryParser.IsEscape(ch))
          ++index;
        else if (SearchQueryParser.IsFilterSeparator(ch))
          return index;
      }
      return -1;
    }

    public static string UnescapeString(string tokenString, out bool invalidEscape)
    {
      invalidEscape = false;
      int length = tokenString.Length;
      StringBuilder stringBuilder = new StringBuilder(length);
      for (int index = 0; index < length; ++index)
      {
        char ch1 = tokenString[index];
        if (!SearchQueryParser.IsEscape(ch1))
          stringBuilder.Append(ch1);
        else if (index + 1 < length)
        {
          char ch2 = tokenString[++index];
          UnicodeCategory unicodeCategory = char.GetUnicodeCategory(ch2);
          if (SearchQueryParser.IsEscape(ch2) || SearchQueryParser.IsFilterSeparator(ch2) || SearchQueryParser.IsQuote(ch2, unicodeCategory))
          {
            stringBuilder.Append(ch2);
          }
          else
          {
            invalidEscape = true;
            stringBuilder.Append(ch1);
            stringBuilder.Append(ch2);
          }
        }
        else
        {
          invalidEscape = true;
          stringBuilder.Append(ch1);
        }
      }
      return stringBuilder.ToString();
    }

    public static string EscapeString(
      string tokenString,
      SearchQueryParser.AddQuotesMode addQuotesMode,
      bool isFilterFieldString = false)
    {
      bool flag = addQuotesMode == SearchQueryParser.AddQuotesMode.Always;
      int length = tokenString.Length;
      StringBuilder stringBuilder = new StringBuilder(2 * length);
      if (length == 0)
      {
        flag = addQuotesMode != 0;
      }
      else
      {
        for (int index = 0; index < length; ++index)
        {
          char ch = tokenString[index];
          UnicodeCategory unicodeCategory = char.GetUnicodeCategory(ch);
          if (SearchQueryParser.IsSpace(ch, unicodeCategory))
            flag = true;
          if (SearchQueryParser.IsEscape(ch) || SearchQueryParser.IsFilterSeparator(ch) || SearchQueryParser.IsQuote(ch, unicodeCategory) || isFilterFieldString && index == 0 && SearchQueryParser.IsExcludeFilter(ch))
            stringBuilder.Append('\\');
          stringBuilder.Append(ch);
        }
      }
      if (flag)
      {
        stringBuilder.Insert(0, '"');
        stringBuilder.Append('"');
      }
      return stringBuilder.ToString();
    }

    private static string BuildTokenText(SearchToken token)
    {
      if (!(token is SearchFilterToken searchFilterToken))
        return SearchQueryParser.EscapeString(token.ParsedTokenText, string.IsNullOrEmpty(token.OriginalTokenText) ? SearchQueryParser.AddQuotesMode.Default : SearchQueryParser.AddQuotesMode.IfEmpty);
      string str = searchFilterToken.OriginalTokenText.Substring(0, (int) searchFilterToken.FilterSeparatorPosition).TrimStart();
      if (str.Length > 0 && SearchQueryParser.IsExcludeFilter(str[0]))
        str = str.Substring(1);
      bool flag1 = string.IsNullOrWhiteSpace(str);
      bool flag2 = string.IsNullOrWhiteSpace(searchFilterToken.OriginalTokenText.Substring((int) searchFilterToken.FilterSeparatorPosition + 1));
      return SearchQueryParser.BuildFilterTokenText(searchFilterToken.FilterField, flag1 ? SearchQueryParser.AddQuotesMode.Default : SearchQueryParser.AddQuotesMode.IfEmpty, searchFilterToken.FilterValue, flag2 ? SearchQueryParser.AddQuotesMode.Default : SearchQueryParser.AddQuotesMode.IfEmpty, searchFilterToken.FilterTokenType);
    }

    private static string BuildFilterTokenText(
      string unescapedFilterField,
      SearchQueryParser.AddQuotesMode addQuotesModeField,
      string unescapedFilterValue,
      SearchQueryParser.AddQuotesMode addQuotesModeValue,
      uint filterTokenType)
    {
      string str1 = SearchQueryParser.EscapeString(unescapedFilterField, addQuotesModeField, true);
      string str2 = SearchQueryParser.EscapeString(unescapedFilterValue, addQuotesModeValue);
      if (((int) filterTokenType & 1) != 0)
        str1 = "-" + str1;
      return string.Join((((int) filterTokenType & 2) != 0 ? '=' : ':').ToString(), new string[2]
      {
        str1,
        str2
      });
    }

    private static class SyntaxCharacters
    {
      public const char SpaceSeparator = ' ';
      public const char Quote = '"';
      public const char Escape = '\\';
      public const char PartialMatchFilterSeparator = ':';
      public const char ExactMatchFilterSeparator = '=';
      public const char ExcludeFilter = '-';
    }

    internal enum AddQuotesMode
    {
      Default,
      IfEmpty,
      Always,
    }
  }
}
