// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Query.WikiSearchQueryForwarder
// Assembly: Microsoft.VisualStudio.Services.Search.Query, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 71E00698-03D3-4C67-B313-A550333DA80C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Query.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Arriba.Expressions;
using Microsoft.VisualStudio.Services.Search.Common.Entities;
using Microsoft.VisualStudio.Services.Search.Common.Enums;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities.Wiki;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Relevance.Rules;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Implementations.ElasticSearch.EntityProviders;
using Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy;
using Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Contracts;
using Microsoft.VisualStudio.Services.Search.WebApi.Legacy;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.Search.Query
{
  public class WikiSearchQueryForwarder : 
    EntitySearchQueryForwarder<WikiSearchQuery, WikiQueryResponse>
  {
    private readonly WikiEntityIndexProvider<WikiContract> m_wikiEntityIndexProvider;
    private readonly WikiSearchPlatformRequest.Builder m_wikiSearchPlatformRequestBuilder;
    private const string WikiPhraseSuggesterName = "wiki_phrase_suggester_query";

    public WikiSearchQueryForwarder(
      string searchPlatformConnectionString,
      string searchPlatformSettings,
      SearchOptions searchOptions,
      bool isOnPrem,
      WikiEntityIndexProvider<WikiContract> wikiEntityIndexProvider,
      WikiSearchPlatformRequest.Builder wikiSearchPlatformRequestBuilder)
      : base(searchPlatformConnectionString, searchPlatformSettings, searchOptions, isOnPrem)
    {
      this.m_wikiEntityIndexProvider = wikiEntityIndexProvider;
      this.m_wikiSearchPlatformRequestBuilder = wikiSearchPlatformRequestBuilder ?? throw new SearchException(SearchWebApiResources.UnexpectedSearchErrorMessage);
    }

    public WikiSearchQueryForwarder(ISearchPlatform searchPlatform)
      : base(searchPlatform)
    {
      this.m_wikiEntityIndexProvider = new WikiEntityIndexProvider<WikiContract>();
      this.m_wikiSearchPlatformRequestBuilder = new WikiSearchPlatformRequest.Builder();
    }

    public override WikiQueryResponse ForwardSearchRequest(
      IVssRequestContext requestContext,
      WikiSearchQuery searchQuery,
      IEnumerable<IndexInfo> indexInfo,
      IExpression scopeFiltersExpression,
      string requestId,
      DocumentContractType contractType)
    {
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceEnter(1081450, "Query Pipeline", "Query", nameof (ForwardSearchRequest));
      WikiQueryResponse searchResponse = (WikiQueryResponse) null;
      try
      {
        if (searchQuery == null)
          throw new ArgumentNullException(nameof (searchQuery));
        if (scopeFiltersExpression == null)
          throw new ArgumentNullException(nameof (scopeFiltersExpression));
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceVerboseConditionally(1081451, "Query Pipeline", "Query", (Func<string>) (() => searchQuery.ToString()));
        if (indexInfo == null || !indexInfo.Any<IndexInfo>())
        {
          searchResponse = this.GetZeroResultResponseWithError(searchQuery, ErrorCode.IndexingNotStarted, ErrorType.Warning);
        }
        else
        {
          IDictionary<string, IEnumerable<string>> searchFilters;
          IExpression correctedQueryParseTree;
          WikiSearchQueryTagger tagger;
          WikiSearchPlatformRequest searchQueryRequest1 = this.PreProcessSearchQueryAndFormPlatformRequest(requestContext, searchQuery, indexInfo, scopeFiltersExpression, requestId, contractType, out searchFilters, out correctedQueryParseTree, out tagger);
          if (tagger.Tags.Contains(EntitySearchQueryTagger.UnfilteredPrefixWildcard))
          {
            searchResponse = this.GetZeroResultResponseWithError(searchQuery, ErrorCode.PrefixWildcardQueryNotSupported, ErrorType.Warning);
          }
          else
          {
            EntitySearchSuggestPlatformResponse platformResponse = (EntitySearchSuggestPlatformResponse) null;
            if (searchQueryRequest1 != null)
            {
              Stopwatch stopwatch1 = Stopwatch.StartNew();
              WikiSearchPlatformResponse platformSearchResponse = this.SearchPlatform.Search<WikiContract>(requestContext, (EntitySearchPlatformRequest) searchQueryRequest1, EntityPluginsFactory.GetEntityType(requestContext, "Wiki"), (EntityIndexProvider<WikiContract>) this.m_wikiEntityIndexProvider) as WikiSearchPlatformResponse;
              if (!string.IsNullOrWhiteSpace(searchQuery.SearchText) && this.IncludeSuggestions(searchQuery, (EntitySearchPlatformResponse) platformSearchResponse) && SuggesterSearchTextHelper.IsSimpleSearchText(searchQuery.SearchText))
              {
                EntitySearchSuggestPlatformRequest searchSuggestRequest = this.CreateSearchSuggestRequest(requestContext, searchQuery.SearchText);
                Stopwatch stopwatch2 = Stopwatch.StartNew();
                platformResponse = this.SearchPlatform.Suggest<WikiContract>(requestContext, (EntitySearchPlatformRequest) searchQueryRequest1, searchSuggestRequest, EntityPluginsFactory.GetEntityType(requestContext, "Wiki"), (EntityIndexProvider<WikiContract>) new WikiEntityIndexProvider<WikiContract>());
                EntitySearchPhraseSuggesterHelper phraseSuggesterHelper = new EntitySearchPhraseSuggesterHelper(platformResponse);
                if (phraseSuggesterHelper.ReceivedValidSuggestions())
                {
                  searchQuery.SearchText = phraseSuggesterHelper.GetTopSuggestion();
                  WikiSearchPlatformRequest searchQueryRequest2 = this.PreProcessSearchQueryAndFormPlatformRequest(requestContext, searchQuery, indexInfo, scopeFiltersExpression, requestId, contractType, out searchFilters, out correctedQueryParseTree, out tagger);
                  platformSearchResponse = this.SearchPlatform.Search<WikiContract>(requestContext, (EntitySearchPlatformRequest) searchQueryRequest2, EntityPluginsFactory.GetEntityType(requestContext, "Wiki"), (EntityIndexProvider<WikiContract>) this.m_wikiEntityIndexProvider) as WikiSearchPlatformResponse;
                }
                Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishKpiAndCi("WikiE2EPlatformSuggestQueryTime", "Query Pipeline", (double) stopwatch2.ElapsedMilliseconds);
              }
              if (platformResponse == null)
                platformResponse = new WikiEntityIndexProvider<WikiContract>().DefaultSuggestPlatformResponse(searchQuery.SearchText);
              Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishKpiAndCi("WikiE2EPlatformQueryTime", "Query Pipeline", (double) stopwatch1.ElapsedMilliseconds);
              searchResponse = WikiSearchPlatformResponse.PrepareSearchResponse(platformSearchResponse, platformResponse, searchQuery);
              this.SetWarningInReponseForFacetingAskWhenFacetingIsOffAtHostLevel((EntitySearchQuery) searchQuery, (EntitySearchResponse) searchResponse);
            }
          }
        }
      }
      finally
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceLeave(1081452, "Query Pipeline", "Query", nameof (ForwardSearchRequest));
      }
      return searchResponse;
    }

    public override WikiQueryResponse GetZeroResultResponse(WikiSearchQuery searchRequest)
    {
      IDictionary<string, IEnumerable<string>> source = searchRequest.SearchFilters ?? (IDictionary<string, IEnumerable<string>>) new Dictionary<string, IEnumerable<string>>();
      WikiQueryResponse zeroResultResponse = new WikiQueryResponse();
      zeroResultResponse.Query = searchRequest;
      zeroResultResponse.Results = new WikiResults(0, Enumerable.Empty<WikiResult>());
      zeroResultResponse.Suggestions = (IEnumerable<string>) new List<string>();
      zeroResultResponse.FilterCategories = source.Select<KeyValuePair<string, IEnumerable<string>>, FilterCategory>((Func<KeyValuePair<string, IEnumerable<string>>, FilterCategory>) (sqf => new FilterCategory()
      {
        Name = sqf.Key,
        Filters = sqf.Value.Select<string, Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Filter>((Func<string, Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Filter>) (v => new Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Filter(v, v, 0, true)))
      }));
      return zeroResultResponse;
    }

    private WikiQueryResponse GetZeroResultResponseWithError(
      WikiSearchQuery searchQuery,
      ErrorCode errorCode,
      ErrorType errorType)
    {
      WikiQueryResponse zeroResultResponse = this.GetZeroResultResponse(searchQuery);
      zeroResultResponse.AddError(new ErrorData()
      {
        ErrorCode = errorCode.ToString(),
        ErrorType = errorType
      });
      return zeroResultResponse;
    }

    private WikiSearchPlatformRequest PreProcessSearchQueryAndFormPlatformRequest(
      IVssRequestContext requestContext,
      WikiSearchQuery searchQuery,
      IEnumerable<IndexInfo> indexInfo,
      IExpression scopeFiltersExpression,
      string requestId,
      DocumentContractType contractType,
      out IDictionary<string, IEnumerable<string>> searchFilters,
      out IExpression correctedQueryParseTree,
      out WikiSearchQueryTagger tagger)
    {
      WikiSearchPlatformRequest searchPlatformRequest = (WikiSearchPlatformRequest) null;
      searchFilters = searchQuery.SearchFilters ?? (IDictionary<string, IEnumerable<string>>) new Dictionary<string, IEnumerable<string>>();
      ISearchQueryTransformer transformerInstance = SearchQueryTransformerFactory.GetTransformerInstance(requestContext, EntityPluginsFactory.GetEntityType(requestContext, "Wiki"));
      correctedQueryParseTree = transformerInstance.CorrectQuery(requestContext, transformerInstance.ParseSearchText(searchQuery.SearchText));
      tagger = new WikiSearchQueryTagger(correctedQueryParseTree, searchFilters);
      tagger.Compute();
      tagger.Publish();
      this.UpdateRelevanceRules(requestContext, correctedQueryParseTree, EntityPluginsFactory.GetEntityType(requestContext, "Wiki"), contractType, new Func<IVssRequestContext, IEnumerable<RelevanceRule>, IEnumerable<RelevanceRule>>(transformerInstance.CorrectRelevanceRules));
      if (!tagger.Tags.Contains(EntitySearchQueryTagger.UnfilteredPrefixWildcard))
      {
        SearchOptions platformSearchRequest = this.GetSearchOptionsForPlatformSearchRequest((EntitySearchQuery) searchQuery);
        searchPlatformRequest = this.m_wikiSearchPlatformRequestBuilder.Build(requestContext, platformSearchRequest, requestId, indexInfo, correctedQueryParseTree, searchFilters, scopeFiltersExpression, searchQuery);
      }
      return searchPlatformRequest;
    }

    private EntitySearchSuggestPlatformRequest CreateSearchSuggestRequest(
      IVssRequestContext requestContext,
      string originalSearchText)
    {
      double configValueOrDefault1 = requestContext.GetConfigValueOrDefault("/Service/OrgSearch/Settings/EntitySearchSuggestConfidenceValue", 0.0);
      double configValueOrDefault2 = requestContext.GetConfigValueOrDefault("/Service/OrgSearch/Settings/EntitySearchSuggestMaxErrorsValue", 2.0);
      int configValueOrDefault3 = requestContext.GetConfigValueOrDefault("/Service/OrgSearch/Settings/EntitySearchSuggestNumberOfSuggestionsRequired", 5);
      return new EntitySearchSuggestPlatformRequest()
      {
        Confidence = configValueOrDefault1,
        NumberOfSuggestions = configValueOrDefault3,
        MaxErrors = configValueOrDefault2,
        SuggestText = originalSearchText,
        SuggestField = FormattableString.Invariant(FormattableStringFactory.Create("{0}.{1}", (object) "fileNames", (object) "unstemmed")),
        SuggestQueryName = "wiki_phrase_suggester_query",
        Fields = (IEnumerable<string>) new string[1]
        {
          FormattableString.Invariant(FormattableStringFactory.Create("{0}.{1}", (object) "fileNames", (object) "unstemmed"))
        }
      };
    }

    private bool IncludeSuggestions(
      WikiSearchQuery wikiSearchQuery,
      EntitySearchPlatformResponse platformSearchResponse)
    {
      return wikiSearchQuery.IncludeSuggestions && platformSearchResponse.Results.Count == 0;
    }
  }
}
