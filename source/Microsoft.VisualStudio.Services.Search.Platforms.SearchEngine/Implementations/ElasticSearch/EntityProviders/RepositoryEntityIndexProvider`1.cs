// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Implementations.ElasticSearch.EntityProviders.RepositoryEntityIndexProvider`1
// Assembly: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EE1DF96-C85D-457F-AAA1-93619829BFD4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Arriba.Expressions;
using Microsoft.VisualStudio.Services.Search.Common.Enums;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities.ProjectRepo;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Implementations.ElasticSearch.QueryBuilders;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Implementations.ElasticSearch.QueryBuilders.Aggregation;
using Nest;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Implementations.ElasticSearch.EntityProviders
{
  internal sealed class RepositoryEntityIndexProvider<T> : ProjectRepoEntityIndexProviderBase<T> where T : class
  {
    [StaticSafe]
    public static readonly IDictionary<string, IList<string>> FieldPreferencesOrderListMapping = (IDictionary<string, IList<string>>) new Dictionary<string, IList<string>>()
    {
      {
        "name",
        (IList<string>) new List<string>()
        {
          "name",
          "name.casechangeanalyzed"
        }
      },
      {
        "collectionName",
        (IList<string>) new List<string>()
        {
          "collectionName",
          "collectionNameAnalyzed",
          "collectionNameAnalyzed.casechangeanalyzed"
        }
      },
      {
        "projectName",
        (IList<string>) new List<string>()
        {
          "projectName",
          "projectName.casechangeanalyzed"
        }
      },
      {
        "languages",
        (IList<string>) new List<string>()
        {
          "languages.analysed"
        }
      }
    };

    public override void BuildSearchComponents(
      IVssRequestContext requestContext,
      EntitySearchPlatformRequest request,
      string rawQueryString,
      string rawFilterString,
      out Func<QueryContainerDescriptor<T>, QueryContainer> funcQuery,
      out Func<QueryContainerDescriptor<T>, QueryContainer> funcFilter,
      out Func<AggregationContainerDescriptor<T>, AggregationContainerDescriptor<T>> funcAggregations,
      out Func<HighlightDescriptor<T>, IHighlight> funcHighlight,
      out Func<IVssRequestContext, SortDescriptor<T>> funcSort)
    {
      funcQuery = this.GetFilteredQuery(rawQueryString, rawFilterString);
      funcFilter = new Func<QueryContainerDescriptor<T>, QueryContainer>(new RepositoryFilterBuilder(request.SearchFilters).Filters<T>);
      RepositorySearchPlatformRequest searchPlatformRequest = request as RepositorySearchPlatformRequest;
      funcAggregations = new Func<AggregationContainerDescriptor<T>, AggregationContainerDescriptor<T>>(new RepositoryAggregationBuilder(request.SearchFilters, request.Options.HasFlag((Enum) SearchOptions.Faceting), searchPlatformRequest.CustomAggregations).Aggregates<T>);
      funcHighlight = new ProjectHighlightBuilder((IEnumerable<string>) this.GetHighlightFields(), request.Options.HasFlag((Enum) SearchOptions.Highlighting)).Highlight<T>();
      funcSort = new ProjectRepoSortBuilder(request.SortOptions).Sort<T>();
    }

    public override EntitySearchPlatformResponse DefaultPlatformResponse(
      EntitySearchPlatformRequest searchRequest)
    {
      return new EntitySearchPlatformResponse(0, (IList<SearchHit>) new List<SearchHit>(), facets: (IDictionary<string, IEnumerable<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Filter>>) new Dictionary<string, IEnumerable<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Filter>>());
    }

    public override EntitySearchPlatformResponse PreparePlatformResponse(
      int responseCount,
      bool isTimedOut,
      List<SearchHit> searchResults,
      IDictionary<string, IEnumerable<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Filter>> searchFacets)
    {
      return new EntitySearchPlatformResponse(responseCount, (IList<SearchHit>) searchResults, isTimedOut, searchFacets);
    }

    public override EntitySearchPlatformResponse PreparePlatformResponse(
      AbstractSearchDocumentContract docContract,
      int responseCount,
      bool isTimedOut,
      List<SearchHit> searchResults,
      IDictionary<string, IEnumerable<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Filter>> searchFacets,
      string scrollId)
    {
      return new EntitySearchPlatformResponse(responseCount, (IList<SearchHit>) searchResults, isTimedOut, searchFacets);
    }

    public override IDictionary<string, IEnumerable<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Filter>> GetSearchFacets(
      EntitySearchPlatformRequest request,
      ISearchResponse<T> elasticSearchResponse)
    {
      FriendlyDictionary<string, IEnumerable<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Filter>> searchFacets = new FriendlyDictionary<string, IEnumerable<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Filter>>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      AggregateDictionary aggregations = elasticSearchResponse.Aggregations;
      if (aggregations != null)
      {
        searchFacets.Add(this.GetTermFacets(aggregations, request, Microsoft.VisualStudio.Services.Search.WebApi.Legacy.Contracts.WorkItem.Constants.FilterCategories.Collections, new string[2]
        {
          "filtered_collection_aggs",
          "collection_aggs"
        }, true));
        searchFacets.Add(this.GetTermFacets(aggregations, request, Microsoft.VisualStudio.Services.Search.WebApi.Legacy.Contracts.WorkItem.Constants.FilterCategories.Languages, new string[2]
        {
          "filtered_languages_aggs",
          "languages_aggs"
        }));
        searchFacets.Add(this.GetTermFacets(aggregations, request, Microsoft.VisualStudio.Services.Search.WebApi.Legacy.Contracts.WorkItem.Constants.FilterCategories.Repositories, new string[2]
        {
          "filtered_repository_aggs",
          "repository_aggs"
        }));
        IEnumerable<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Filter> visibilityFilters = this.GetModifiedVisibilityFilters(this.GetTermFacets(aggregations, request, Microsoft.VisualStudio.Services.Search.WebApi.Legacy.Contracts.WorkItem.Constants.FilterCategories.Visibility, new string[2]
        {
          "filtered_visibility_aggs",
          "visibility_aggs"
        }).Value);
        searchFacets.Add(new KeyValuePair<string, IEnumerable<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Filter>>(Microsoft.VisualStudio.Services.Search.WebApi.Legacy.Contracts.WorkItem.Constants.FilterCategories.Visibility, visibilityFilters));
      }
      return (IDictionary<string, IEnumerable<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Filter>>) searchFacets;
    }

    public override SearchHit GetSearchHit(
      IHit<T> hit,
      EntitySearchPlatformRequest request,
      ConcurrentBag<Exception> exceptions,
      ref bool isOperationSuccessful)
    {
      if (!isOperationSuccessful)
        return (SearchHit) new RepositorySearchHit();
      if (hit.Highlight != null)
        return (SearchHit) new RepositorySearchHit((IEnumerable<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Contracts.Highlight>) this.CalculateHighlightSnippets(this.PruneHighlightsBasedOnPreferenceOrderForBaseField(hit.Highlight, RepositoryEntityIndexProvider<T>.FieldPreferencesOrderListMapping), (IList<string>) this.GetHighlightFields(), request.ContractType), (object) hit.Source as RepositoryContract);
      exceptions.Add((Exception) new SearchPlatformException("ES Response: hit.highlights cannot be null"));
      isOperationSuccessful = false;
      return (SearchHit) new RepositorySearchHit();
    }

    protected override List<string> GetHighlightFields() => new List<string>()
    {
      "readme",
      "name",
      "collectionNameAnalyzed",
      "projectName",
      "name.casechangeanalyzed",
      "collectionNameAnalyzed.casechangeanalyzed",
      "projectName.casechangeanalyzed"
    };

    public override void BuildCountComponents(
      IVssRequestContext requestContext,
      ResultsCountPlatformRequest request,
      string rawQueryString,
      string rawFilterString,
      out Func<QueryContainerDescriptor<T>, QueryContainer> query)
    {
      query = this.GetFilteredQuery(rawQueryString, rawFilterString);
    }

    public override IExpression BuildQueryFilterExpression(
      IVssRequestContext requestContext,
      IDictionary<string, IEnumerable<string>> searchFilters,
      IExpression queryParseTree,
      DocumentContractType contractType)
    {
      return new ProjectRepoFilterBuilder(searchFilters).GetQueryFilterExpression();
    }
  }
}
