// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Query.PackageSearchRequestForwarder
// Assembly: Microsoft.VisualStudio.Services.Search.Query, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 71E00698-03D3-4C67-B313-A550333DA80C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Query.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Arriba;
using Microsoft.VisualStudio.Services.Search.Common.Arriba.Expressions;
using Microsoft.VisualStudio.Services.Search.Common.Entities;
using Microsoft.VisualStudio.Services.Search.Common.Enums;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities.Package;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Implementations.ElasticSearch.EntityProviders;
using Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts;
using Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts.Package;
using Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Contracts;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Search.Query
{
  public class PackageSearchRequestForwarder : 
    EntitySearchRequestForwarder<PackageSearchRequest, PackageSearchResponseContent>
  {
    protected const string packagePhraseSuggesterName = "package_phrase_suggester_query";

    public PackageSearchRequestForwarder(
      string searchPlatformConnectionString,
      string searchPlatformSettings,
      SearchOptions searchOptions,
      bool isOnPrem)
      : base(searchPlatformConnectionString, searchPlatformSettings, searchOptions, isOnPrem)
    {
    }

    public PackageSearchRequestForwarder(ISearchPlatform searchPlatform)
      : base(searchPlatform)
    {
    }

    public override PackageSearchResponseContent ForwardSearchRequest(
      IVssRequestContext requestContext,
      PackageSearchRequest searchRequest,
      IEnumerable<IndexInfo> indexInfo,
      IExpression scopeFiltersExpression,
      string requestId,
      DocumentContractType contractType,
      bool includeSuggestions,
      out IEnumerable<string> suggestions)
    {
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceEnter(1080077, "Query Pipeline", "Query", nameof (ForwardSearchRequest));
      PackageSearchResponseContent searchResponse = (PackageSearchResponseContent) null;
      suggestions = (IEnumerable<string>) null;
      try
      {
        this.ValidateInputParameters(searchRequest, scopeFiltersExpression);
        if (indexInfo == null || !indexInfo.Any<IndexInfo>())
        {
          searchResponse = this.GetZeroResultResponseWithError(searchRequest, InfoCodes.IndexingNotStarted, out suggestions);
        }
        else
        {
          IDictionary<string, IEnumerable<string>> searchFilters;
          PackageSearchQueryTagger tagger;
          PackageSearchPlatformRequest searchQueryRequest1 = this.PreProcessSearchRequestAndFormPlatformRequest(requestContext, searchRequest, indexInfo, scopeFiltersExpression, requestId, out searchFilters, out tagger);
          if (tagger.Tags.Contains(EntitySearchQueryTagger.UnfilteredPrefixWildcard))
          {
            searchResponse = this.GetZeroResultResponseWithError(searchRequest, InfoCodes.PrefixWildcardQueryNotSupported, out suggestions);
          }
          else
          {
            EntitySearchSuggestPlatformResponse platformResponse = (EntitySearchSuggestPlatformResponse) null;
            if (searchQueryRequest1 != null)
            {
              Stopwatch stopwatch1 = Stopwatch.StartNew();
              PackageSearchPlatformResponse platformSearchResponse = this.SearchPlatform.Search<PackageVersionContract>(requestContext, (EntitySearchPlatformRequest) searchQueryRequest1, EntityPluginsFactory.GetEntityType(requestContext, "Package"), this.GetIndexProvider()) as PackageSearchPlatformResponse;
              Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishKpiAndCi("PackageQueryE2EQueryTime", "Query Pipeline", (double) stopwatch1.ElapsedMilliseconds);
              if (!string.IsNullOrWhiteSpace(searchRequest.SearchText) && this.IncludeSuggestions(includeSuggestions, (EntitySearchPlatformResponse) platformSearchResponse) && SuggesterSearchTextHelper.IsSimpleSearchText(searchRequest.SearchText))
              {
                Stopwatch stopwatch2 = Stopwatch.StartNew();
                EntitySearchSuggestPlatformRequest searchSuggestRequest = this.CreateSearchSuggestRequest(requestContext, searchRequest.SearchText);
                platformResponse = this.SearchPlatform.Suggest<PackageVersionContract>(requestContext, (EntitySearchPlatformRequest) searchQueryRequest1, searchSuggestRequest, EntityPluginsFactory.GetEntityType(requestContext, "Package"), (EntityIndexProvider<PackageVersionContract>) new PackageEntityIndexProvider<PackageVersionContract>());
                EntitySearchPhraseSuggesterHelper phraseSuggesterHelper = new EntitySearchPhraseSuggesterHelper(platformResponse);
                if (phraseSuggesterHelper.ReceivedValidSuggestions())
                {
                  searchRequest.SearchText = phraseSuggesterHelper.GetTopSuggestion();
                  PackageSearchPlatformRequest searchQueryRequest2 = this.PreProcessSearchRequestAndFormPlatformRequest(requestContext, searchRequest, indexInfo, scopeFiltersExpression, requestId, out IDictionary<string, IEnumerable<string>> _, out PackageSearchQueryTagger _);
                  platformSearchResponse = this.SearchPlatform.Search<PackageVersionContract>(requestContext, (EntitySearchPlatformRequest) searchQueryRequest2, EntityPluginsFactory.GetEntityType(requestContext, "Package"), (EntityIndexProvider<PackageVersionContract>) new PackageEntityIndexProvider<PackageVersionContract>()) as PackageSearchPlatformResponse;
                }
                Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishKpiAndCi("PackageE2EPlatformSuggestQueryTime", "Query Pipeline", (double) stopwatch2.ElapsedMilliseconds);
              }
              if (platformResponse == null)
                platformResponse = new PackageEntityIndexProvider<PackageVersionContract>().DefaultSuggestPlatformResponse(searchRequest.SearchText);
              PackageSearchPlatformResponse platformSearchLatestVersionResponse = new PackageSearchPlatformResponse(0, (IList<SearchHit>) new List<SearchHit>(), false);
              if (platformSearchResponse.Results.Count > 0)
              {
                List<string> packageIdFilters = new List<string>();
                platformSearchResponse.Results.ForEach<SearchHit>((Action<SearchHit>) (hit => packageIdFilters.AddRange(((PackageSearchHit) hit).Feeds.Select<PackageFeed, string>((Func<PackageFeed, string>) (s => s.PackageId)))));
                IExpression expression = (IExpression) new AndExpression(new IExpression[2]
                {
                  scopeFiltersExpression,
                  (IExpression) new TermsExpression("packageId", Operator.In, (IEnumerable<string>) packageIdFilters)
                });
                SearchOptions platformSearchRequest = this.GetSearchOptionsForPlatformSearchRequest((EntitySearchRequest) searchRequest);
                PackageSearchPlatformRequest searchPlatformRequest = new PackageSearchPlatformRequest();
                searchPlatformRequest.Options = platformSearchRequest;
                searchPlatformRequest.RequestId = requestId;
                searchPlatformRequest.IndexInfo = indexInfo;
                searchPlatformRequest.QueryParseTree = (IExpression) new EmptyExpression();
                searchPlatformRequest.ContinueOnEmptyQuery = true;
                searchPlatformRequest.SearchFilters = searchFilters;
                searchPlatformRequest.ScopeFiltersExpression = expression;
                searchPlatformRequest.SkipResults = 0;
                searchPlatformRequest.TakeResults = 0;
                searchPlatformRequest.TakeAggResults = platformSearchResponse.Results.Count;
                searchPlatformRequest.Fields = (IEnumerable<string>) new string[1]
                {
                  "name"
                };
                searchPlatformRequest.ContractType = DocumentContractType.PackageVersionContract;
                searchPlatformRequest.SortOptions = (IEnumerable<EntitySortOption>) (searchRequest.OrderBy ?? (IEnumerable<SortOption>) new List<SortOption>()).Select<SortOption, EntitySortOption>((Func<SortOption, EntitySortOption>) (i => new EntitySortOption()
                {
                  Field = i.Field,
                  SortOrder = (Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Contracts.SortOrder) i.SortOrder,
                  SortOrderStr = i.SortOrderStr
                })).ToList<EntitySortOption>();
                PackageSearchPlatformRequest searchQueryRequest3 = searchPlatformRequest;
                Stopwatch stopwatch3 = Stopwatch.StartNew();
                platformSearchLatestVersionResponse = this.SearchPlatform.Search<PackageVersionContract>(requestContext, (EntitySearchPlatformRequest) searchQueryRequest3, EntityPluginsFactory.GetEntityType(requestContext, "Package"), (EntityIndexProvider<PackageVersionContract>) new PackageEntityIndexProvider<PackageVersionContract>()) as PackageSearchPlatformResponse;
                Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishKpiAndCi("PackageLatestVersionE2EQueryTime", "Query Pipeline", (double) stopwatch3.ElapsedMilliseconds);
              }
              Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishKpiAndCi("E2EPlatformQueryTime", "Query Pipeline", (double) stopwatch1.ElapsedMilliseconds);
              searchResponse = PackageSearchPlatformResponse.PrepareSearchResponse(platformSearchResponse, platformSearchLatestVersionResponse, searchRequest);
              suggestions = PackageSearchRequestForwarder.GetSuggestResults(platformResponse);
              this.SetWarningInReponseForFacetingAskWhenFacetingIsOffAtHostLevel((EntitySearchRequest) searchRequest, (Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts.EntitySearchResponse) searchResponse);
            }
          }
        }
      }
      finally
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceLeave(1080075, "Query Pipeline", "Query", nameof (ForwardSearchRequest));
      }
      return searchResponse;
    }

    public override PackageSearchResponseContent GetZeroResultResponse(
      PackageSearchRequest searchRequest,
      out IEnumerable<string> suggestions)
    {
      suggestions = (IEnumerable<string>) new List<string>();
      PackageSearchResponseContent zeroResultResponse = new PackageSearchResponseContent();
      zeroResultResponse.Count = 0;
      zeroResultResponse.Results = Enumerable.Empty<PackageResult>();
      IDictionary<string, IEnumerable<string>> filters = searchRequest.Filters;
      zeroResultResponse.Facets = filters != null ? (IDictionary<string, IEnumerable<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts.Filter>>) filters.ToDictionary<KeyValuePair<string, IEnumerable<string>>, string, IEnumerable<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts.Filter>>((Func<KeyValuePair<string, IEnumerable<string>>, string>) (fc => fc.Key), (Func<KeyValuePair<string, IEnumerable<string>>, IEnumerable<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts.Filter>>) (fc =>
      {
        IEnumerable<string> source = fc.Value;
        return source == null ? (IEnumerable<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts.Filter>) null : source.Select<string, Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts.Filter>((Func<string, Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts.Filter>) (f => new Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts.Filter(f, f, 0)));
      })) : (IDictionary<string, IEnumerable<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts.Filter>>) null;
      return zeroResultResponse;
    }

    protected virtual EntityIndexProvider<PackageVersionContract> GetIndexProvider() => (EntityIndexProvider<PackageVersionContract>) new PackageEntityIndexProvider<PackageVersionContract>();

    protected void ValidateInputParameters(
      PackageSearchRequest searchRequest,
      IExpression scopeFiltersExpression)
    {
      if (searchRequest == null)
        throw new ArgumentNullException(nameof (searchRequest));
      if (scopeFiltersExpression == null)
        throw new ArgumentNullException(nameof (scopeFiltersExpression));
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceVerboseConditionally(1080076, "Query Pipeline", "Query", (Func<string>) (() => searchRequest.ToString()));
    }

    internal PackageSearchPlatformRequest PreProcessSearchRequestAndFormPlatformRequest(
      IVssRequestContext requestContext,
      PackageSearchRequest searchRequest,
      IEnumerable<IndexInfo> indexInfo,
      IExpression scopeFiltersExpression,
      string requestId,
      out IDictionary<string, IEnumerable<string>> searchFilters,
      out PackageSearchQueryTagger tagger)
    {
      PackageSearchPlatformRequest searchPlatformRequest1 = (PackageSearchPlatformRequest) null;
      IExpression expression = PackageSearchQueryTransformer.Correct(requestContext, PackageSearchQueryTransformer.Parse(searchRequest.SearchText));
      searchFilters = searchRequest.Filters ?? (IDictionary<string, IEnumerable<string>>) new Dictionary<string, IEnumerable<string>>();
      tagger = new PackageSearchQueryTagger(expression, searchFilters);
      tagger.Compute();
      tagger.Publish();
      if (!tagger.Tags.Contains(EntitySearchQueryTagger.UnfilteredPrefixWildcard))
      {
        SearchOptions platformSearchRequest = this.GetSearchOptionsForPlatformSearchRequest((EntitySearchRequest) searchRequest);
        PackageSearchPlatformRequest searchPlatformRequest2 = new PackageSearchPlatformRequest();
        searchPlatformRequest2.Options = platformSearchRequest;
        searchPlatformRequest2.RequestId = requestId;
        searchPlatformRequest2.IndexInfo = indexInfo;
        searchPlatformRequest2.QueryParseTree = expression;
        searchPlatformRequest2.SearchFilters = searchFilters;
        searchPlatformRequest2.ScopeFiltersExpression = scopeFiltersExpression;
        searchPlatformRequest2.SkipResults = searchRequest.Skip;
        searchPlatformRequest2.TakeResults = 0;
        searchPlatformRequest2.TakeAggResults = searchRequest.Top;
        searchPlatformRequest2.Fields = (IEnumerable<string>) new string[3]
        {
          "name",
          "description",
          "version"
        };
        searchPlatformRequest2.ContractType = DocumentContractType.PackageVersionContract;
        searchPlatformRequest2.SortOptions = (IEnumerable<EntitySortOption>) (searchRequest.OrderBy ?? (IEnumerable<SortOption>) new List<SortOption>()).Select<SortOption, EntitySortOption>((Func<SortOption, EntitySortOption>) (i => new EntitySortOption()
        {
          Field = i.Field,
          SortOrder = (Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Contracts.SortOrder) i.SortOrder,
          SortOrderStr = i.SortOrderStr
        })).ToList<EntitySortOption>();
        searchPlatformRequest1 = searchPlatformRequest2;
      }
      return searchPlatformRequest1;
    }

    protected EntitySearchSuggestPlatformRequest CreateSearchSuggestRequest(
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
        SuggestField = "name",
        SuggestQueryName = "package_phrase_suggester_query",
        Fields = (IEnumerable<string>) new string[1]
        {
          "name"
        }
      };
    }

    protected PackageSearchResponseContent GetZeroResultResponseWithError(
      PackageSearchRequest searchRequest,
      InfoCodes infoCode,
      out IEnumerable<string> suggestions)
    {
      PackageSearchResponseContent zeroResultResponse = this.GetZeroResultResponse(searchRequest, out suggestions);
      zeroResultResponse.InfoCode = (int) infoCode;
      return zeroResultResponse;
    }

    private static IEnumerable<string> GetSuggestResults(
      EntitySearchSuggestPlatformResponse platformSuggestResponse)
    {
      List<string> suggestResults = new List<string>();
      foreach (SuggestOption suggestion in platformSuggestResponse.Suggestions)
        suggestResults.Add(suggestion.Text);
      return (IEnumerable<string>) suggestResults;
    }

    protected bool IncludeSuggestions(
      bool shouldIncludeSuggestions,
      EntitySearchPlatformResponse platformSearchResponse)
    {
      return shouldIncludeSuggestions && platformSearchResponse.Results.Count == 0;
    }
  }
}
