// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.SearchQueryParser
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Microsoft.VisualStudio.Services.Gallery.Server
{
  internal class SearchQueryParser
  {
    private const string DoubleQuotes = "\"";
    private const string Space = " ";
    private const int WordCountLimitInPhrase = 40;
    private const string SearchQuerySplitPattern = "(\\w+:)?([' '\\t])*(\"[^\"]+\"|\"\"|[\\w\\+\\#\\-]+)\\s*";
    private const string SearchQuerySplitPattern2 = "(\\w+:)?([' '\\t])*(\"[^\"]+\"|\"\"|[\\w\\+\\#\\-\\.]+)\\s*";
    [StaticSafe]
    private static readonly IDictionary<SearchFilterType, int> SearchFilterPriorityMap = (IDictionary<SearchFilterType, int>) new Dictionary<SearchFilterType, int>()
    {
      {
        SearchFilterType.SearchPhrase,
        1
      },
      {
        SearchFilterType.SearchWord,
        2
      },
      {
        SearchFilterType.Name,
        3
      },
      {
        SearchFilterType.Publisher,
        4
      },
      {
        SearchFilterType.Tag,
        5
      },
      {
        SearchFilterType.TagName,
        6
      },
      {
        SearchFilterType.InstallationTarget,
        7
      },
      {
        SearchFilterType.Category,
        8
      }
    };
    [StaticSafe]
    private static readonly IDictionary<SearchFilterType, SearchFilterOperatorType> ExcludeToIncludeMap = (IDictionary<SearchFilterType, SearchFilterOperatorType>) new Dictionary<SearchFilterType, SearchFilterOperatorType>()
    {
      {
        SearchFilterType.Name,
        SearchFilterOperatorType.Or
      },
      {
        SearchFilterType.Category,
        SearchFilterOperatorType.Or
      },
      {
        SearchFilterType.TagName,
        SearchFilterOperatorType.Or
      },
      {
        SearchFilterType.Publisher,
        SearchFilterOperatorType.Or
      },
      {
        SearchFilterType.InstallationTarget,
        SearchFilterOperatorType.Or
      }
    };

    public virtual List<SearchCriteria> Parse(IVssRequestContext requestContext, string searchQuery)
    {
      List<SearchCriteria> criteriaList = new List<SearchCriteria>();
      if (string.IsNullOrEmpty(searchQuery))
        return criteriaList;
      searchQuery = searchQuery.Trim();
      string pattern = "(\\w+:)?([' '\\t])*(\"[^\"]+\"|\"\"|[\\w\\+\\#\\-]+)\\s*";
      if (requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.DoNotTreatDotAsDelimiterInSearchText"))
        pattern = "(\\w+:)?([' '\\t])*(\"[^\"]+\"|\"\"|[\\w\\+\\#\\-\\.]+)\\s*";
      MatchCollection matchCollection = Regex.Matches(searchQuery, pattern);
      EqlParamsHelper eqlParamsHelper = new EqlParamsHelper();
      StringBuilder phraseBuilder = new StringBuilder();
      int num = 0;
      string filterValue1 = (string) null;
      foreach (Match match in matchCollection)
      {
        bool flag1 = false;
        string str = match.Groups[0].Value.Trim();
        bool flag2;
        string searchQuery1;
        SearchFilterType filterType;
        SearchFilterOperatorType operatorType;
        if (this.IsQuotedTerm(str))
        {
          flag2 = true;
          searchQuery1 = str;
          filterType = SearchFilterType.SearchPhrase;
          operatorType = SearchFilterOperatorType.Or;
        }
        else
        {
          string applicableEqlParam = eqlParamsHelper.GetApplicableEqlParam(str);
          if (string.IsNullOrEmpty(applicableEqlParam))
          {
            flag1 = true;
            flag2 = false;
            filterType = SearchFilterType.SearchWord;
            searchQuery1 = str;
            operatorType = SearchFilterOperatorType.Or;
          }
          else
          {
            string searchQuery2 = eqlParamsHelper.GetValueFromQueryForEqlParam(str, applicableEqlParam);
            filterType = eqlParamsHelper.GetFilterTypeForEqlParam(applicableEqlParam);
            operatorType = SearchFilterOperatorType.And;
            if (filterType == SearchFilterType.Category || filterType == SearchFilterType.InstallationTarget)
              searchQuery2 = searchQuery2.Replace("\"", string.Empty);
            searchQuery1 = this.SanitizeQuery(searchQuery2);
            flag2 = true;
          }
        }
        string filterValue2 = this.SanitizeQuery(searchQuery1);
        if (flag2)
        {
          this.ExtractPhraseAndClearBuilder(phraseBuilder, (IList<SearchCriteria>) criteriaList);
          num = 0;
        }
        if (num == 0 && filterType == SearchFilterType.SearchWord)
          filterValue1 = filterValue2;
        else if (num == 1 && filterType == SearchFilterType.SearchWord)
        {
          this.AddSearchCriteriaToList(filterType, filterValue1, operatorType, (IList<SearchCriteria>) criteriaList);
          this.AddSearchCriteriaToList(filterType, filterValue2, operatorType, (IList<SearchCriteria>) criteriaList);
        }
        else if (num < 40)
          this.AddSearchCriteriaToList(filterType, filterValue2, operatorType, (IList<SearchCriteria>) criteriaList);
        if (flag1 && num < 40)
        {
          phraseBuilder.Append(filterValue2);
          phraseBuilder.Append(" ");
          ++num;
        }
      }
      this.ExtractPhraseAndClearBuilder(phraseBuilder, (IList<SearchCriteria>) criteriaList);
      this.FixCriteriaAsPerPriority((IList<SearchCriteria>) criteriaList);
      if (criteriaList.Count > 0)
        this.AddSearchCriteriaToList(SearchFilterType.UserQuery, this.SanitizeQuery(searchQuery), SearchFilterOperatorType.Or, (IList<SearchCriteria>) criteriaList);
      return criteriaList;
    }

    private void ExtractPhraseAndClearBuilder(
      StringBuilder phraseBuilder,
      IList<SearchCriteria> criteriaList)
    {
      this.AddSearchCriteriaToList(SearchFilterType.SearchPhrase, phraseBuilder.ToString().Trim(), SearchFilterOperatorType.Or, criteriaList);
      phraseBuilder.Clear();
    }

    private void AddSearchCriteriaToList(
      SearchFilterType filterType,
      string filterValue,
      SearchFilterOperatorType operatorType,
      IList<SearchCriteria> criteriaList)
    {
      if (string.IsNullOrEmpty(filterValue))
        return;
      criteriaList.Add(this.GetSearchCriteria(filterType, filterValue, operatorType));
    }

    private string SanitizeQuery(string searchQuery)
    {
      if (string.IsNullOrEmpty(searchQuery))
        return string.Empty;
      string searchQuery1 = searchQuery.Trim();
      bool flag = this.IsQuotedTerm(searchQuery1);
      string str = searchQuery1.Replace("\"", " ").Trim().Replace(";", " ").Trim().Replace("--", " ").Trim().Replace("*", " ").Trim();
      if (!string.IsNullOrEmpty(str) & flag)
        str = "\"" + str + "\"";
      return str;
    }

    private bool IsQuotedTerm(string searchQuery) => searchQuery.StartsWith("\"") && searchQuery.EndsWith("\"");

    private SearchCriteria GetSearchCriteria(
      SearchFilterType type,
      string value,
      SearchFilterOperatorType operatorType)
    {
      return new SearchCriteria()
      {
        FilterType = type,
        FilterValue = value,
        OperatorType = operatorType
      };
    }

    private int GetSearchFilterPriority(SearchFilterType filter)
    {
      int num;
      return !SearchQueryParser.SearchFilterPriorityMap.TryGetValue(filter, out num) ? int.MaxValue : num;
    }

    private void FixCriteriaAsPerPriority(IList<SearchCriteria> criteriaList)
    {
      int num = int.MaxValue;
      foreach (SearchCriteria criteria in (IEnumerable<SearchCriteria>) criteriaList)
      {
        int searchFilterPriority = this.GetSearchFilterPriority(criteria.FilterType);
        num = num > searchFilterPriority ? searchFilterPriority : num;
      }
      foreach (SearchCriteria criteria in (IEnumerable<SearchCriteria>) criteriaList)
      {
        if (SearchQueryParser.ExcludeToIncludeMap.ContainsKey(criteria.FilterType))
        {
          int searchFilterPriority = this.GetSearchFilterPriority(criteria.FilterType);
          if (num == searchFilterPriority)
            criteria.OperatorType = SearchQueryParser.ExcludeToIncludeMap[criteria.FilterType];
        }
      }
    }
  }
}
