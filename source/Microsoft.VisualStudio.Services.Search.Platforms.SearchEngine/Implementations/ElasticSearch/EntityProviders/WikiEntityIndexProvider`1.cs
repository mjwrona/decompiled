// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Implementations.ElasticSearch.EntityProviders.WikiEntityIndexProvider`1
// Assembly: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EE1DF96-C85D-457F-AAA1-93619829BFD4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Arriba.Expressions;
using Microsoft.VisualStudio.Services.Search.Common.Enums;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities.Wiki;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Implementations.ElasticSearch.QueryBuilders;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Implementations.ElasticSearch.QueryBuilders.Aggregation;
using Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy;
using Nest;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Implementations.ElasticSearch.EntityProviders
{
  public class WikiEntityIndexProvider<T> : EntityIndexProvider<T> where T : class
  {
    [StaticSafe]
    private static readonly FriendlyDictionary<DocumentContractType, AbstractSearchDocumentContract> s_documentContractMapping = new FriendlyDictionary<DocumentContractType, AbstractSearchDocumentContract>()
    {
      [DocumentContractType.WikiContract] = (AbstractSearchDocumentContract) new WikiContract()
    };
    private readonly IDictionary<string, IList<string>> m_fieldPreferencesOrderListMapping = (IDictionary<string, IList<string>>) new Dictionary<string, IList<string>>()
    {
      {
        "projectName",
        (IList<string>) new List<string>()
        {
          "projectName.search",
          "projectName.searchwithcamelcasedelimiter"
        }
      },
      {
        "collectionName",
        (IList<string>) new List<string>()
        {
          "collectionName.search",
          "collectionName.searchwithcamelcasedelimiter"
        }
      },
      {
        "fileNames",
        (IList<string>) new List<string>()
        {
          "fileNames",
          "fileNames.lower"
        }
      }
    };
    private const string TenantIndexNamePrefix = "Tenant_Wiki@";

    public override void BuildSearchComponents(
      IVssRequestContext requestContext,
      EntitySearchPlatformRequest request,
      string rawQueryString,
      string rawFilterString,
      out Func<QueryContainerDescriptor<T>, QueryContainer> query,
      out Func<QueryContainerDescriptor<T>, QueryContainer> filter,
      out Func<AggregationContainerDescriptor<T>, AggregationContainerDescriptor<T>> aggregations,
      out Func<HighlightDescriptor<T>, IHighlight> highlight,
      out Func<IVssRequestContext, SortDescriptor<T>> sort)
    {
      WikiSearchPlatformRequest wikiSearchRequest = request as WikiSearchPlatformRequest;
      query = this.GetFilteredQuery(rawQueryString, rawFilterString);
      filter = new Func<QueryContainerDescriptor<T>, QueryContainer>(new WikiFilterBuilder(wikiSearchRequest.SearchFilters, requestContext).Filters<T>);
      aggregations = new Func<AggregationContainerDescriptor<T>, AggregationContainerDescriptor<T>>(this.GetWikiAggregationBuilder(wikiSearchRequest, requestContext).Aggregates<T>);
      highlight = new WikiHighlightBuilder(wikiSearchRequest.QueryParseTree, (IReadOnlyCollection<string>) wikiSearchRequest.HighlightFields, wikiSearchRequest.Options.HasFlag((Enum) SearchOptions.Highlighting)).Highlight<T>(requestContext);
      sort = new WikiSortBuilder(wikiSearchRequest.SortOptions).Sort<T>();
    }

    public override void BuildSuggestComponents(
      IVssRequestContext requestContext,
      EntitySearchSuggestPlatformRequest suggestRequest,
      string rawFilterString,
      out Func<SuggestContainerDescriptor<T>, SuggestContainerDescriptor<T>> suggest)
    {
      suggest = new EntitySearchPhraseSuggesterBuilder(suggestRequest, rawFilterString).Suggest<T>();
    }

    protected virtual WikiAggregationBuilder GetWikiAggregationBuilder(
      WikiSearchPlatformRequest wikiSearchRequest,
      IVssRequestContext requestContext)
    {
      bool addCollectionAggregation = (wikiSearchRequest.IndexInfo.Any<IndexInfo>() ? wikiSearchRequest.IndexInfo.First<IndexInfo>().IndexName : string.Empty).StartsWith("Tenant_Wiki@", StringComparison.Ordinal);
      return new WikiAggregationBuilder(wikiSearchRequest.SearchFilters, wikiSearchRequest.Options.HasFlag((Enum) SearchOptions.Faceting), requestContext, addCollectionAggregation);
    }

    public override EntitySearchPlatformResponse DefaultPlatformResponse(
      EntitySearchPlatformRequest request)
    {
      Dictionary<string, IEnumerable<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Filter>> facets = new Dictionary<string, IEnumerable<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Filter>>();
      if (request.SearchFilters != null)
      {
        foreach (KeyValuePair<string, IEnumerable<string>> searchFilter in (IEnumerable<KeyValuePair<string, IEnumerable<string>>>) request.SearchFilters)
          facets.Add(searchFilter.Key, searchFilter.Value.Select<string, Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Filter>((Func<string, Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Filter>) (f =>
          {
            string id = f;
            return new Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Filter(f, id, 0, true);
          })));
      }
      return (EntitySearchPlatformResponse) new WikiSearchPlatformResponse(0, (IList<SearchHit>) new List<SearchHit>(), false, (IDictionary<string, IEnumerable<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Filter>>) facets);
    }

    public override IEnumerable<string> GetFieldNames(
      IEnumerable<string> storedFields,
      DocumentContractType contractType)
    {
      return storedFields;
    }

    public override IDictionary<string, IEnumerable<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Filter>> GetSearchFacets(
      EntitySearchPlatformRequest request,
      ISearchResponse<T> elasticSearchResponse)
    {
      FriendlyDictionary<string, IEnumerable<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Filter>> searchFacets = new FriendlyDictionary<string, IEnumerable<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Filter>>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      AggregateDictionary aggregations = elasticSearchResponse.Aggregations;
      bool flag = (request.IndexInfo.Any<IndexInfo>() ? request.IndexInfo.First<IndexInfo>().IndexName : string.Empty).StartsWith("Tenant_Wiki@", StringComparison.Ordinal);
      if (aggregations != null)
      {
        if (flag)
          searchFacets.Add(this.GetTermFacets(aggregations, request, "CollectionFilters", new string[2]
          {
            "filtered_collection_aggs",
            "collection_aggs"
          }, true));
        searchFacets.Add(this.GetTermFacets(aggregations, request, "ProjectFilters", new string[2]
        {
          "filtered_project_aggs",
          "project_aggs"
        }, true));
        searchFacets.Add(this.GetTermFacets(aggregations, request, "Wiki", new string[2]
        {
          "filtered_wiki_aggs",
          "wiki_aggs"
        }));
        searchFacets.Add(this.GetTermFacets(aggregations, request, "TagFilters", new string[2]
        {
          "filtered_tags_aggs",
          "tags_aggs"
        }));
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
        return (SearchHit) new WikiSearchHit();
      if (hit.Highlight != null)
        return (SearchHit) new WikiSearchHit((IEnumerable<WikiHitSnippet>) this.GetHighlightSnippets(hit.Highlight), (object) hit.Source as WikiContract);
      exceptions.Add((Exception) new SearchPlatformException("ES Response: hit.highlight cannot be null"));
      isOperationSuccessful = false;
      return (SearchHit) new WikiSearchHit();
    }

    public override int GetTotalResultCount(
      EntitySearchPlatformRequest request,
      ISearchResponse<T> elasticSearchResponse)
    {
      return (int) elasticSearchResponse.Total;
    }

    public override string GetStoredFieldNameForElasticsearchName(
      string field,
      DocumentContractType contractType)
    {
      return WikiEntityIndexProvider<T>.s_documentContractMapping.ContainsKey(contractType) ? WikiEntityIndexProvider<T>.s_documentContractMapping[contractType].GetFieldNameForStoredField(field) : field;
    }

    public override string GetStoredFieldValue(
      string field,
      string fieldValue,
      DocumentContractType contractType)
    {
      return WikiEntityIndexProvider<T>.s_documentContractMapping.ContainsKey(contractType) ? WikiEntityIndexProvider<T>.s_documentContractMapping[contractType].GetStoredFieldValue(field, fieldValue) : fieldValue;
    }

    public override EntitySearchPlatformResponse PreparePlatformResponse(
      int responseCount,
      bool isTimedOut,
      List<SearchHit> searchResults,
      IDictionary<string, IEnumerable<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Filter>> searchFacets)
    {
      return (EntitySearchPlatformResponse) new WikiSearchPlatformResponse(responseCount, (IList<SearchHit>) searchResults, false, searchFacets);
    }

    public override EntitySearchPlatformResponse PreparePlatformResponse(
      AbstractSearchDocumentContract docContract,
      int responseCount,
      bool isTimedOut,
      List<SearchHit> searchResults,
      IDictionary<string, IEnumerable<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Filter>> searchFacets,
      string scrollId)
    {
      return (EntitySearchPlatformResponse) new WikiSearchPlatformResponse(responseCount, (IList<SearchHit>) searchResults, false, searchFacets);
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
      return new WikiFilterBuilder(searchFilters, requestContext).GetQueryFilterExpression();
    }

    protected override Dictionary<string, IEnumerable<string>> GetSearchHitSources(
      T sources,
      EntitySearchPlatformRequest request,
      ConcurrentBag<Exception> exceptions,
      ref bool isOperationSuccessful)
    {
      throw new NotImplementedException();
    }

    private ICollection<WikiHitSnippet> GetHighlightSnippets(
      IReadOnlyDictionary<string, IReadOnlyCollection<string>> highlights)
    {
      List<WikiHitSnippet> highlightSnippets = new List<WikiHitSnippet>();
      this.PruneHighlightsBasedOnPreferenceOrderForBaseField(highlights, this.m_fieldPreferencesOrderListMapping);
      foreach (KeyValuePair<string, IReadOnlyCollection<string>> highlight in (IEnumerable<KeyValuePair<string, IReadOnlyCollection<string>>>) highlights)
      {
        string nameForStoredField = WikiEntityIndexProvider<T>.s_documentContractMapping[DocumentContractType.WikiContract].GetFieldNameForStoredField(highlight.Key);
        highlightSnippets.Add(new WikiHitSnippet()
        {
          FieldReferenceName = nameForStoredField,
          Highlights = (IEnumerable<string>) highlight.Value
        });
      }
      return (ICollection<WikiHitSnippet>) highlightSnippets;
    }
  }
}
