// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Query.CodeSearchQueryForwarder
// Assembly: Microsoft.VisualStudio.Services.Search.Query, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 71E00698-03D3-4C67-B313-A550333DA80C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Query.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.BusinessIntelligence;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Arriba.Expressions;
using Microsoft.VisualStudio.Services.Search.Common.Entities;
using Microsoft.VisualStudio.Services.Search.Common.Enums;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities.Code;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Relevance.Rules;
using Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy;
using Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Contracts;
using Microsoft.VisualStudio.Services.Search.WebApi.Legacy;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Search.Query
{
  public class CodeSearchQueryForwarder : EntitySearchQueryForwarder<SearchQuery, CodeQueryResponse>
  {
    public CodeSearchQueryForwarder(
      string searchPlatformConnectionString,
      string searchPlatformSettings,
      SearchOptions searchOptions,
      bool isOnPrem)
      : base(searchPlatformConnectionString, searchPlatformSettings, searchOptions, isOnPrem)
    {
    }

    public CodeSearchQueryForwarder(
      string searchPlatformConnectionString,
      string searchPlatformSettings,
      bool isOnPrem)
      : this(searchPlatformConnectionString, searchPlatformSettings, SearchOptions.Highlighting | SearchOptions.Faceting | SearchOptions.Ranking | SearchOptions.Rescore, isOnPrem)
    {
    }

    protected CodeSearchQueryForwarder(ISearchPlatform searchPlatform)
      : base(searchPlatform)
    {
    }

    public override CodeQueryResponse ForwardSearchRequest(
      IVssRequestContext requestContext,
      SearchQuery searchQuery,
      IEnumerable<IndexInfo> indexInfo,
      IExpression scopeFiltersExpression,
      string requestId,
      DocumentContractType contractType)
    {
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceEnter(1081300, "Query Pipeline", "Query", nameof (ForwardSearchRequest));
      CodeQueryResponse searchResponse;
      try
      {
        if (searchQuery == null)
          throw new ArgumentNullException(nameof (searchQuery));
        if (scopeFiltersExpression == null)
          throw new ArgumentNullException(nameof (scopeFiltersExpression));
        if (requestContext.GetCurrentHostConfigValue<bool>("/Service/ALMSearch/Settings/EnableABTesting"))
        {
          ClientTraceData properties = new ClientTraceData();
          properties.Add("RequestIdForABTesting", (object) requestId);
          properties.Add("SearchText", (object) searchQuery.SearchText);
          Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishClientTrace("Query Pipeline", "Query", properties);
        }
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceVerboseConditionally(1083156, "Query Pipeline", "Query", (Func<string>) (() => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "SearchText : {0}", (object) searchQuery.SearchText)));
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceVerboseConditionally(1081302, "Query Pipeline", "Query", (Func<string>) (() => searchQuery.ToString()));
        bool isWildcardOnlySearch;
        IExpression correctedQueryParseTree;
        IDictionary<string, IEnumerable<string>> searchFilters;
        this.TransformQuery(requestContext, searchQuery, contractType, out isWildcardOnlySearch, out correctedQueryParseTree, out searchFilters);
        List<ErrorData> errorList;
        CodeSearchQueryTagger tagger;
        CodeForwarderHelpers.CheckIfCodeQuerySupported(requestContext, contractType, correctedQueryParseTree, searchFilters, isWildcardOnlySearch, out errorList, out tagger);
        if (requestContext.Items.ContainsKey("testQuery") && requestContext.GetCurrentHostConfigValue<bool>("/Service/ALMSearch/Settings/EnableABTesting") && errorList.Count == 1 && tagger.Tags.HasFlag((Enum) CodeSearchTag.CodeSubStringTerm) && errorList.ElementAt<ErrorData>(0).ErrorCode.ToString().Equals("PrefixWildcardQueryNotSupported"))
          errorList.RemoveAt(0);
        if (errorList.Count > 0)
        {
          searchResponse = this.GetZeroResultResponse(searchQuery);
          foreach (ErrorData errorData in errorList)
            searchResponse.AddError(errorData);
        }
        else if (indexInfo == null || !indexInfo.Any<IndexInfo>())
        {
          searchResponse = this.GetZeroResultResponse(searchQuery);
          searchResponse.AddError(new ErrorData()
          {
            ErrorCode = "IndexingNotStarted",
            ErrorType = ErrorType.Warning
          });
        }
        else
        {
          SearchOptions platformSearchRequest = this.GetSearchOptionsForPlatformSearchRequest((EntitySearchQuery) searchQuery);
          CodeSearchPlatformResponse platformSearchResponse = CodeSearchPlatformRequest.CreateCodeSearchPlatformRequest(this.SearchPlatform, contractType, platformSearchRequest, requestId, indexInfo, correctedQueryParseTree, searchFilters, scopeFiltersExpression, searchQuery).Search(requestContext);
          searchResponse = CodeSearchPlatformResponse.PrepareSearchResponse(platformSearchResponse, searchQuery);
          if (searchResponse.Results.Count > 0)
          {
            if (platformSearchResponse.SearchTimedOut)
              searchResponse.AddError(new ErrorData()
              {
                ErrorCode = "PartialResultsDueToSearchRequestTimeout",
                ErrorType = ErrorType.Warning
              });
            if (requestContext.IsFeatureEnabled("Search.Server.Code.WildCardPartialResults") && this.HasWildCardTerm(tagger.Tags))
              searchResponse.AddError(new ErrorData()
              {
                ErrorCode = "WildCardPartialResults",
                ErrorType = ErrorType.Warning
              });
          }
          if (searchResponse.Results.Count <= 0)
            searchResponse.AddError(new ErrorData()
            {
              ErrorCode = this.GetZeroResultsErrorCodeBasedOnWildcardAndFilterTags(tagger.Tags, searchQuery.SearchText, searchQuery.Filters).ToString(),
              ErrorType = ErrorType.Warning
            });
          this.SetWarningInReponseForFacetingAskWhenFacetingIsOffAtHostLevel((EntitySearchQuery) searchQuery, (EntitySearchResponse) searchResponse);
        }
      }
      finally
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceLeave(1081301, "Query Pipeline", "Query", nameof (ForwardSearchRequest));
      }
      return searchResponse;
    }

    private void TransformQuery(
      IVssRequestContext requestContext,
      SearchQuery searchQuery,
      DocumentContractType contractType,
      out bool isWildcardOnlySearch,
      out IExpression correctedQueryParseTree,
      out IDictionary<string, IEnumerable<string>> searchFilters)
    {
      ISearchQueryTransformer transformerInstance = SearchQueryTransformerFactory.GetTransformerInstance(requestContext, EntityPluginsFactory.GetEntityType(requestContext, "Code"));
      IExpression searchText = transformerInstance.ParseSearchText(searchQuery.SearchText);
      isWildcardOnlySearch = CodeForwarderHelpers.IsWildCardOnlySearch_Code(searchText);
      correctedQueryParseTree = transformerInstance.CorrectQuery(requestContext, searchText);
      searchFilters = QueryForwarderHelpers.ConvertToDictionary(searchQuery.Filters);
      this.UpdateRelevanceRules(requestContext, correctedQueryParseTree, EntityPluginsFactory.GetEntityType(requestContext, "Code"), contractType, new Func<IVssRequestContext, IEnumerable<RelevanceRule>, IEnumerable<RelevanceRule>>(transformerInstance.CorrectRelevanceRules));
    }

    public override CodeQueryResponse GetZeroResultResponse(SearchQuery searchRequest)
    {
      CodeQueryResponse zeroResultResponse = new CodeQueryResponse();
      zeroResultResponse.Query = searchRequest;
      zeroResultResponse.Results = new CodeResults(0, Enumerable.Empty<CodeResult>());
      zeroResultResponse.FilterCategories = searchRequest.Filters.Select<SearchFilter, FilterCategory>((Func<SearchFilter, FilterCategory>) (sqf => new FilterCategory()
      {
        Name = sqf.Name,
        Filters = sqf.Values.Select<string, Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Filter>((Func<string, Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Filter>) (v => new Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Filter(sqf.Name == "CodeElementFilters" ? CodeSearchFilters.DisplayableCEFilterIdToNameMap[v] : v, v, 0, true)))
      }));
      return zeroResultResponse;
    }

    private ErrorCode GetZeroResultsErrorCodeBasedOnWildcardAndFilterTags(
      CodeSearchTag tags,
      string searchText,
      IEnumerable<SearchFilter> searchFilter)
    {
      List<string> all = ((IEnumerable<string>) RegularExpressions.WhitespaceRegex.Split(searchText)).ToList<string>().FindAll((Predicate<string>) (s => !string.IsNullOrWhiteSpace(s)));
      bool flag1 = this.HasWildCardTerm(tags);
      bool flag2 = all.FindIndex((Predicate<string>) (x => x.StartsWith(CodeFileContract.CodeContractQueryableElement.FileName.InlineFilterName() + ":", StringComparison.OrdinalIgnoreCase) || x.StartsWith(CodeFileContract.CodeContractQueryableElement.FileName2.InlineFilterName() + ":", StringComparison.OrdinalIgnoreCase))) >= 0;
      bool flag3 = searchFilter.FirstOrDefault<SearchFilter>((Func<SearchFilter, bool>) (x => x.Name.Equals("BranchFilters", StringComparison.OrdinalIgnoreCase))) != null;
      bool flag4 = ((((tags.HasFlag((Enum) CodeSearchTag.ProjFilter) || tags.HasFlag((Enum) CodeSearchTag.ProjFacet) || tags.HasFlag((Enum) CodeSearchTag.RepoFilter) || tags.HasFlag((Enum) CodeSearchTag.RepoFacet) || tags.HasFlag((Enum) CodeSearchTag.PathFilter) ? 1 : (tags.HasFlag((Enum) CodeSearchTag.PathFacet) ? 1 : 0)) | (flag2 ? 1 : 0)) != 0 || tags.HasFlag((Enum) CodeSearchTag.ExtFilter) || tags.HasFlag((Enum) CodeSearchTag.CEFilter) || tags.HasFlag((Enum) CodeSearchTag.CEFacet) ? 1 : (tags.HasFlag((Enum) CodeSearchTag.BranchFilter) ? 1 : 0)) | (flag3 ? 1 : 0)) != 0;
      return !flag1 ? (!flag4 ? ErrorCode.ZeroResultsWithNoWildcardNoFilter : ErrorCode.ZeroResultsWithFilter) : (!flag4 ? ErrorCode.ZeroResultsWithWildcard : ErrorCode.ZeroResultsWithWildcardAndFilter);
    }

    private bool HasWildCardTerm(CodeSearchTag tags) => tags.HasFlag((Enum) CodeSearchTag.WildcardAsterisk) || tags.HasFlag((Enum) CodeSearchTag.WildcardQuestion);
  }
}
