// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Query.ScrollCodeSearchQueryForwarder
// Assembly: Microsoft.VisualStudio.Services.Search.Query, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 71E00698-03D3-4C67-B313-A550333DA80C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Query.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Arriba.Expressions;
using Microsoft.VisualStudio.Services.Search.Common.Entities;
using Microsoft.VisualStudio.Services.Search.Common.Enums;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities.Code;
using Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts;
using Microsoft.VisualStudio.Services.Search.WebApi.Contracts.Code;
using Microsoft.VisualStudio.Services.Search.WebApi.Legacy;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Search.Query
{
  public class ScrollCodeSearchQueryForwarder : 
    ScrollEntitySearchQueryForwarder<ScrollSearchRequest, ScrollSearchResponse>
  {
    protected ScrollCodeSearchQueryForwarder(ISearchPlatform searchPlatform)
      : base(searchPlatform)
    {
    }

    public ScrollCodeSearchQueryForwarder(
      string searchPlatformConnectionString,
      string searchPlatformSettings,
      SearchOptions searchOptions,
      bool isOnPrem)
      : base(searchPlatformConnectionString, searchPlatformSettings, searchOptions, isOnPrem)
    {
    }

    public ScrollCodeSearchQueryForwarder(
      string searchPlatformConnectionString,
      string searchPlatformSettings,
      bool isOnPrem)
      : this(searchPlatformConnectionString, searchPlatformSettings, SearchOptions.None, isOnPrem)
    {
    }

    public override ScrollSearchResponse ForwardSearchRequest(
      IVssRequestContext requestContext,
      ScrollSearchRequest searchQuery,
      IEnumerable<IndexInfo> indexInfo,
      IExpression scopeFiltersExpression,
      string requestId,
      DocumentContractType contractType)
    {
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceEnter(1081300, "Query Pipeline", "Query", nameof (ForwardSearchRequest));
      ScrollSearchResponse scrollSearchResponse = (ScrollSearchResponse) null;
      try
      {
        if (searchQuery == null)
          throw new ArgumentNullException(nameof (searchQuery));
        if (scopeFiltersExpression == null)
          throw new ArgumentNullException(nameof (scopeFiltersExpression));
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceVerboseConditionally(1081302, "Query Pipeline", "Query", (Func<string>) (() => searchQuery.ToString()));
        ISearchQueryTransformer transformerInstance = SearchQueryTransformerFactory.GetTransformerInstance(requestContext, (IEntityType) CodeEntityType.GetInstance());
        if (!string.IsNullOrEmpty(searchQuery.ScrollId))
          searchQuery.SearchText = string.Empty;
        IExpression searchText = transformerInstance.ParseSearchText(searchQuery.SearchText);
        bool flag1 = this.IsWildCardOnlySearch(searchText);
        IExpression expression = transformerInstance.CorrectQuery(requestContext, searchText);
        IDictionary<string, IEnumerable<string>> dictionary = CodeSearchRequestConvertor.UpdateFilterMap(searchQuery.Filters);
        CodeSearchQueryTagger searchQueryTagger = new CodeSearchQueryTagger(expression, dictionary, requestContext);
        searchQueryTagger.Compute();
        searchQueryTagger.Publish();
        bool flag2 = requestContext.GetCurrentHostConfigValue<bool>("/Service/ALMSearch/Settings/EnableCodePrefixWildcardSearch", true) && contractType.IsDedupeFileContract();
        EmptyExpression emptyExpression = expression as EmptyExpression;
        int num = -1;
        if (searchQueryTagger.Tags.HasFlag((Enum) CodeSearchTag.CodeElementPrefixWildcard) || !flag2 && searchQueryTagger.Tags.HasFlag((Enum) CodeSearchTag.CodePrefixWildcard) || flag2 && searchQueryTagger.Tags.HasFlag((Enum) CodeSearchTag.CodePrefixWildcard) && searchQueryTagger.Tags.HasFlag((Enum) CodeSearchTag.CEFacet) || flag2 && searchQueryTagger.Tags.HasFlag((Enum) CodeSearchTag.CodePrefixSuffixWildcardTerm))
          num = 4;
        else if (flag1 && emptyExpression != null)
          num = 13;
        else if (emptyExpression != null && string.IsNullOrEmpty(searchQuery.ScrollId))
          num = 12;
        if (searchQueryTagger.Tags.HasFlag((Enum) CodeSearchTag.MultiWord) && searchQueryTagger.Tags.HasFlag((Enum) CodeSearchTag.CEFacet))
          num = 5;
        if (num != -1)
        {
          scrollSearchResponse = this.GetZeroResultResponse(searchQuery);
          scrollSearchResponse.InfoCode = num;
        }
        else if (indexInfo == null || !indexInfo.Any<IndexInfo>())
        {
          scrollSearchResponse = this.GetZeroResultResponse(searchQuery);
          scrollSearchResponse.InfoCode = 2;
        }
        else
        {
          CodeSearchPlatformResponse platformSearchResponse = CodeSearchPlatformRequest.CreateCodeSearchPlatformScrollRequest(contractType, requestId, this.SearchPlatform, indexInfo, expression, dictionary, scopeFiltersExpression, searchQuery.ScrollSize, searchQuery.ScrollId).Search(requestContext);
          scrollSearchResponse = CodeSearchPlatformResponse.PrepareSearchResponse(platformSearchResponse);
          if (platformSearchResponse.SearchTimedOut && scrollSearchResponse.Count > 0)
            scrollSearchResponse.InfoCode = 18;
          if (scrollSearchResponse.Count <= 0)
            scrollSearchResponse.InfoCode = this.GetZeroResultsErrorCodeBasedOnWildcardAndFilterTags(searchQueryTagger.Tags, searchQuery.SearchText, searchQuery.Filters);
          if (!string.IsNullOrEmpty(searchQuery.ScrollId) && scrollSearchResponse.ScrollId == null)
            scrollSearchResponse.InfoCode = 21;
          if (!string.IsNullOrEmpty(searchQuery.ScrollId))
          {
            if (scrollSearchResponse.ScrollId != null)
            {
              if (string.IsNullOrEmpty(scrollSearchResponse.ScrollId))
                scrollSearchResponse.InfoCode = 22;
            }
          }
        }
      }
      finally
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceLeave(1081301, "Query Pipeline", "Query", nameof (ForwardSearchRequest));
      }
      return scrollSearchResponse;
    }

    protected override ScrollSearchResponse GetZeroResultResponse(ScrollSearchRequest searchQuery)
    {
      ScrollSearchResponse zeroResultResponse = new ScrollSearchResponse();
      zeroResultResponse.Count = 0;
      zeroResultResponse.Facets = (IDictionary<string, IEnumerable<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts.Filter>>) searchQuery.Filters.ToDictionary<KeyValuePair<string, IEnumerable<string>>, string, IEnumerable<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts.Filter>>((Func<KeyValuePair<string, IEnumerable<string>>, string>) (fc => fc.Key), (Func<KeyValuePair<string, IEnumerable<string>>, IEnumerable<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts.Filter>>) (fc => fc.Value.Select<string, Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts.Filter>((Func<string, Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts.Filter>) (f => new Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts.Filter(f, string.Empty, 0)))));
      return zeroResultResponse;
    }

    private int GetZeroResultsErrorCodeBasedOnWildcardAndFilterTags(
      CodeSearchTag tags,
      string searchText,
      IDictionary<string, IEnumerable<string>> searchFilter)
    {
      List<string> all = ((IEnumerable<string>) RegularExpressions.WhitespaceRegex.Split(searchText)).ToList<string>().FindAll((Predicate<string>) (s => !string.IsNullOrWhiteSpace(s)));
      bool flag1 = tags.HasFlag((Enum) CodeSearchTag.WildcardAsterisk) || tags.HasFlag((Enum) CodeSearchTag.WildcardQuestion);
      bool flag2 = all.FindIndex((Predicate<string>) (x => x.StartsWith(CodeFileContract.CodeContractQueryableElement.FileName.InlineFilterName() + ":", StringComparison.OrdinalIgnoreCase) || x.StartsWith(CodeFileContract.CodeContractQueryableElement.FileName2.InlineFilterName() + ":", StringComparison.OrdinalIgnoreCase))) >= 0;
      bool flag3 = searchFilter.FirstOrDefault<KeyValuePair<string, IEnumerable<string>>>((Func<KeyValuePair<string, IEnumerable<string>>, bool>) (x => x.Key.Equals("BranchFilters", StringComparison.OrdinalIgnoreCase))).Equals((object) new KeyValuePair<string, IEnumerable<string>>());
      bool flag4 = ((((tags.HasFlag((Enum) CodeSearchTag.ProjFilter) || tags.HasFlag((Enum) CodeSearchTag.ProjFacet) || tags.HasFlag((Enum) CodeSearchTag.RepoFilter) || tags.HasFlag((Enum) CodeSearchTag.RepoFacet) || tags.HasFlag((Enum) CodeSearchTag.PathFilter) ? 1 : (tags.HasFlag((Enum) CodeSearchTag.PathFacet) ? 1 : 0)) | (flag2 ? 1 : 0)) != 0 || tags.HasFlag((Enum) CodeSearchTag.ExtFilter) || tags.HasFlag((Enum) CodeSearchTag.CEFilter) || tags.HasFlag((Enum) CodeSearchTag.CEFacet) ? 1 : (tags.HasFlag((Enum) CodeSearchTag.BranchFilter) ? 1 : 0)) | (flag3 ? 1 : 0)) != 0;
      return !flag1 ? (!flag4 ? 17 : 15) : (!flag4 ? 14 : 16);
    }

    private bool IsWildCardOnlySearch(IExpression expression)
    {
      bool flag = false;
      if (!(expression is EmptyExpression))
      {
        foreach (IExpression expression1 in (IEnumerable<IExpression>) expression)
        {
          if (expression1 is TermExpression termExpression && !string.IsNullOrWhiteSpace(termExpression.Value))
          {
            if (!RegularExpressions.WildcardOnlyRegex.IsMatch(termExpression.Value))
              return false;
            flag = true;
          }
        }
        if (flag)
          return true;
      }
      return false;
    }
  }
}
