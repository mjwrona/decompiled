// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Implementations.ElasticSearch.EntityProviders.ProjectEntityIndexProvider`1
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
  internal sealed class ProjectEntityIndexProvider<T> : ProjectRepoEntityIndexProviderBase<T> where T : class
  {
    [StaticSafe]
    private static readonly IDictionary<string, IList<string>> s_fieldPreferencesOrderListMapping = (IDictionary<string, IList<string>>) new Dictionary<string, IList<string>>()
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
          "collectionNameAnalyzed",
          "collectionNameAnalyzed.casechangeanalyzed"
        }
      },
      {
        "tags",
        (IList<string>) new List<string>()
        {
          "tags",
          "tags.casechangeanalyzed"
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
      funcFilter = new Func<QueryContainerDescriptor<T>, QueryContainer>(new ProjectRepoFilterBuilder(request.SearchFilters).Filters<T>);
      ProjectSearchPlatformRequest searchPlatformRequest = request as ProjectSearchPlatformRequest;
      funcAggregations = new Func<AggregationContainerDescriptor<T>, AggregationContainerDescriptor<T>>(new ProjectRepoAggregationBuilder(searchPlatformRequest.SearchFilters, request.Options.HasFlag((Enum) SearchOptions.Faceting), searchPlatformRequest.CustomAggregations).Aggregates<T>);
      funcHighlight = new ProjectHighlightBuilder((IEnumerable<string>) this.GetHighlightFields(), request.Options.HasFlag((Enum) SearchOptions.Highlighting)).Highlight<T>();
      funcSort = new ProjectRepoSortBuilder(searchPlatformRequest.SortOptions).Sort<T>();
    }

    public override EntitySearchPlatformResponse DefaultPlatformResponse(
      EntitySearchPlatformRequest searchRequest)
    {
      return (EntitySearchPlatformResponse) new ProjectSearchPlatformResponse(0, (IList<SearchHit>) new List<SearchHit>(), false, (IDictionary<string, IEnumerable<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Filter>>) new Dictionary<string, IEnumerable<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Filter>>());
    }

    public override EntitySearchPlatformResponse PreparePlatformResponse(
      int responseCount,
      bool isTimedOut,
      List<SearchHit> searchResults,
      IDictionary<string, IEnumerable<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Filter>> searchFacets)
    {
      int totalMatches = responseCount;
      bool flag = isTimedOut;
      List<SearchHit> results = searchResults;
      int num = flag ? 1 : 0;
      IDictionary<string, IEnumerable<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Filter>> facets = searchFacets;
      return (EntitySearchPlatformResponse) new ProjectSearchPlatformResponse(totalMatches, (IList<SearchHit>) results, num != 0, facets);
    }

    public override EntitySearchPlatformResponse PreparePlatformResponse(
      AbstractSearchDocumentContract docContract,
      int responseCount,
      bool isTimedOut,
      List<SearchHit> searchResults,
      IDictionary<string, IEnumerable<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Filter>> searchFacets,
      string scrollId)
    {
      int totalMatches = responseCount;
      bool flag = isTimedOut;
      List<SearchHit> results = searchResults;
      int num = flag ? 1 : 0;
      IDictionary<string, IEnumerable<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Filter>> facets = searchFacets;
      return (EntitySearchPlatformResponse) new ProjectSearchPlatformResponse(totalMatches, (IList<SearchHit>) results, num != 0, facets);
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
        searchFacets.Add(this.GetTermFacets(aggregations, request, Microsoft.VisualStudio.Services.Search.WebApi.Legacy.Contracts.WorkItem.Constants.FilterCategories.Tags, new string[2]
        {
          "filtered_project_tags_aggs",
          "project_tags_aggs"
        }));
        searchFacets.Add(this.GetTermFacets(aggregations, request, Microsoft.VisualStudio.Services.Search.WebApi.Legacy.Contracts.WorkItem.Constants.FilterCategories.Languages, new string[2]
        {
          "filtered_languages_aggs",
          "languages_aggs"
        }));
        searchFacets.Add(this.GetTermFacets(aggregations, request, Microsoft.VisualStudio.Services.Search.WebApi.Legacy.Contracts.WorkItem.Constants.FilterCategories.Projects, new string[2]
        {
          "filtered_project_aggs",
          "project_aggs"
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
        return (SearchHit) new ProjectSearchHit();
      if (hit.Highlight == null)
      {
        exceptions.Add((Exception) new SearchPlatformException("ES Response: hit.highlight cannot be null"));
        isOperationSuccessful = false;
        return (SearchHit) new ProjectSearchHit();
      }
      Dictionary<string, IReadOnlyCollection<string>> highlights = this.PruneHighlightsBasedOnPreferenceOrderForBaseField(hit.Highlight, ProjectEntityIndexProvider<T>.s_fieldPreferencesOrderListMapping);
      List<ProjectSearchInnerHit> innerRepositoryHits = new List<ProjectSearchInnerHit>();
      if (hit.InnerHits != null)
      {
        foreach (InnerHitsResult innerHitsResult in hit.InnerHits.Values)
        {
          foreach (Hit<ILazyDocument> hit1 in innerHitsResult.Hits.Hits)
          {
            RepositoryContract repositoryContract = hit1.Source.As<RepositoryContract>();
            innerRepositoryHits.Add(new ProjectSearchInnerHit((AbstractSearchDocumentContract) repositoryContract, hit1.Score.GetValueOrDefault(), this.ExtractInnerHitHighlights(hit1.Highlight, (AbstractSearchDocumentContract) repositoryContract)));
          }
        }
      }
      return (SearchHit) new ProjectSearchHit((IEnumerable<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Contracts.Highlight>) this.CalculateHighlightSnippets(highlights, (IList<string>) this.GetHighlightFields(), request.ContractType), (IEnumerable<ProjectSearchInnerHit>) innerRepositoryHits, (object) hit.Source as ProjectContract);
    }

    protected override List<string> GetHighlightFields() => new List<string>()
    {
      "name",
      "tags",
      "collectionNameAnalyzed",
      "description",
      "name.casechangeanalyzed",
      "tags.casechangeanalyzed",
      "collectionNameAnalyzed.casechangeanalyzed"
    };

    private static List<string> GetHighlightFieldsForRepositoryChildDocument() => new List<string>()
    {
      "name",
      "name.casechangeanalyzed",
      "readme",
      "languages.analysed"
    };

    private List<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Contracts.Highlight> ExtractInnerHitHighlights(
      IReadOnlyDictionary<string, IReadOnlyCollection<string>> highlights,
      AbstractSearchDocumentContract abstractSearchDocument)
    {
      this.PruneHighlightsBasedOnPreferenceOrderForBaseField(highlights, RepositoryEntityIndexProvider<RepositoryContract>.FieldPreferencesOrderListMapping);
      List<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Contracts.Highlight> innerHitHighlights = new List<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Contracts.Highlight>();
      foreach (string str in ProjectEntityIndexProvider<T>.GetHighlightFieldsForRepositoryChildDocument())
      {
        if (highlights.ContainsKey(str))
        {
          IReadOnlyCollection<string> highlights1;
          highlights.TryGetValue(str, out highlights1);
          innerHitHighlights.Add(new Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Contracts.Highlight(abstractSearchDocument.GetStoredFieldForFieldName(str), (IEnumerable<string>) highlights1));
        }
      }
      return innerHitHighlights;
    }

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
