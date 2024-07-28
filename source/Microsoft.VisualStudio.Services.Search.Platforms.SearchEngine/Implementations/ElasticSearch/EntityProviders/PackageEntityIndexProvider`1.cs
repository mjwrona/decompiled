// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Implementations.ElasticSearch.EntityProviders.PackageEntityIndexProvider`1
// Assembly: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EE1DF96-C85D-457F-AAA1-93619829BFD4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Arriba.Expressions;
using Microsoft.VisualStudio.Services.Search.Common.Enums;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities.Package;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Implementations.ElasticSearch.QueryBuilders;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Implementations.ElasticSearch.QueryBuilders.Aggregation;
using Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts.Package;
using Nest;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Implementations.ElasticSearch.EntityProviders
{
  internal class PackageEntityIndexProvider<T> : EntityIndexProvider<T> where T : class
  {
    [StaticSafe]
    private static readonly FriendlyDictionary<DocumentContractType, AbstractSearchDocumentContract> s_documentContractMapping = new FriendlyDictionary<DocumentContractType, AbstractSearchDocumentContract>()
    {
      [DocumentContractType.PackageVersionContract] = (AbstractSearchDocumentContract) new PackageVersionContract()
    };

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
      PackageSearchPlatformRequest request1 = request as PackageSearchPlatformRequest;
      query = this.GetFilteredQuery(rawQueryString, rawFilterString);
      filter = new Func<QueryContainerDescriptor<T>, QueryContainer>(new PackageFilterBuilder(request1.SearchFilters).Filters<T>);
      aggregations = this.GetAggregationBuilder(request1);
      highlight = (Func<HighlightDescriptor<T>, IHighlight>) null;
      sort = (Func<IVssRequestContext, SortDescriptor<T>>) null;
    }

    protected IReadOnlyCollection<string> GetHighlightFields() => (IReadOnlyCollection<string>) new List<string>()
    {
      "description",
      "version"
    };

    public override EntitySearchPlatformResponse DefaultPlatformResponse(
      EntitySearchPlatformRequest searchRequest)
    {
      return (EntitySearchPlatformResponse) new PackageSearchPlatformResponse(0, (IList<SearchHit>) new List<SearchHit>(), false, (IDictionary<string, IEnumerable<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Filter>>) new Dictionary<string, IEnumerable<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Filter>>());
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
      if (aggregations != null)
      {
        searchFacets.Add(this.GetTermFacets(aggregations, request, PackageSearchFilterCategories.Collections, new string[2]
        {
          "filtered_collection_aggs",
          "collection_aggs"
        }, true, aggregationBucketToFilter: new Func<KeyedBucket<string>, Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Filter>(Selector)));
        searchFacets.Add(this.GetTermFacets(aggregations, request, PackageSearchFilterCategories.ProtocolType, new string[2]
        {
          "filtered_protocol_tags_aggs",
          "protocol_aggs"
        }, aggregationBucketToFilter: new Func<KeyedBucket<string>, Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Filter>(Selector)));
      }
      return (IDictionary<string, IEnumerable<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Filter>>) searchFacets;

      static Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Filter Selector(
        KeyedBucket<string> bucket)
      {
        string key = bucket.Key;
        return new Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Filter(bucket.Key, key, (int) ((ValueAggregate) bucket["Package_Count_Aggs"]).Value.Value, false);
      }
    }

    public override List<SearchHit> GetSearchHits(
      EntitySearchPlatformRequest request,
      ISearchResponse<T> elasticSearchResponse)
    {
      IAggregate aggregate;
      IList<IBucket> hits = !elasticSearchResponse.Aggregations.TryGetValue("Filtered_Results_Aggs", out aggregate) ? ((BucketAggregate) elasticSearchResponse.Aggregations["Package_Aggs"]).Items as IList<IBucket> : ((BucketAggregate) ((IsAReadOnlyDictionaryBase<string, IAggregate>) aggregate)["Package_Aggs"]).Items as IList<IBucket>;
      List<SearchHit> results = new List<SearchHit>((IEnumerable<SearchHit>) new PackageSearchHit[hits.Count]);
      bool isOperationSuccessful = true;
      ConcurrentBag<Exception> exceptions = new ConcurrentBag<Exception>();
      Parallel.For(0, hits.Count, (Action<int>) (i => results[i] = (SearchHit) this.GetSearchHit(hits[i], request, ref exceptions, ref isOperationSuccessful)));
      if (!isOperationSuccessful)
        this.LogUnsuccessfulOperation(elasticSearchResponse, exceptions);
      return results;
    }

    public override int GetTotalResultCount(
      EntitySearchPlatformRequest request,
      ISearchResponse<T> elasticSearchResponse)
    {
      IAggregate aggregate;
      return elasticSearchResponse.Aggregations.TryGetValue("Filtered_Results_Aggs", out aggregate) ? (int) ((ValueAggregate) ((IsAReadOnlyDictionaryBase<string, IAggregate>) aggregate)["Package_Count_Aggs"]).Value.Value : (int) ((ValueAggregate) elasticSearchResponse.Aggregations["Package_Count_Aggs"]).Value.Value;
    }

    public override SearchHit GetSearchHit(
      IHit<T> hit,
      EntitySearchPlatformRequest request,
      ConcurrentBag<Exception> exceptions,
      ref bool isOperationSuccessful)
    {
      throw new NotImplementedException();
    }

    protected virtual Func<AggregationContainerDescriptor<T>, AggregationContainerDescriptor<T>> GetAggregationBuilder(
      PackageSearchPlatformRequest request)
    {
      return new Func<AggregationContainerDescriptor<T>, AggregationContainerDescriptor<T>>(new PackageAggregationBuilder(request.SearchFilters, request.Options.HasFlag((Enum) SearchOptions.Faceting), request.TakeAggResults, request.Options.HasFlag((Enum) SearchOptions.Highlighting)).Aggregates<T>);
    }

    private PackageSearchHit GetSearchHit(
      IBucket bucket,
      EntitySearchPlatformRequest request,
      ref ConcurrentBag<Exception> exceptions,
      ref bool isOperationSuccessful)
    {
      if (!isOperationSuccessful)
        return new PackageSearchHit();
      List<IHit<PackageVersionContract>> list = ((BucketAggregate) (bucket as KeyedBucket<object>)["Feed_Aggs"]).Items.Select<IBucket, IHit<PackageVersionContract>>(new Func<IBucket, IHit<PackageVersionContract>>(this.ExtractHit)).ToList<IHit<PackageVersionContract>>();
      list.Sort((IComparer<IHit<PackageVersionContract>>) new PackageVersionContractComparer());
      Hit<PackageVersionContract> hit1 = (Hit<PackageVersionContract>) null;
      List<PackageFeed> feeds = new List<PackageFeed>();
      foreach (Hit<PackageVersionContract> hit2 in list)
      {
        PackageVersionContract source = hit2.Source;
        feeds.Add(new PackageFeed(source.CollectionId, source.CollectionName, source.CollectionUrl, source.FeedId, source.FeedName, source.PackageId, source.Version, (IEnumerable<PackageViewInfo>) source.Views));
        if (hit1 == null)
          hit1 = hit2;
      }
      if (hit1.Highlight != null)
        return new PackageSearchHit((IEnumerable<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Contracts.Highlight>) this.CalculateHighlightSnippets((IHit<PackageVersionContract>) hit1, this.GetHighlightFields(), request.ContractType), hit1.Source, feeds);
      exceptions.Add((Exception) new SearchPlatformException("ES Response: hit.highlights cannot be null"));
      isOperationSuccessful = false;
      return new PackageSearchHit();
    }

    protected IHit<PackageVersionContract> ExtractHit(IBucket feed) => ((TopHitsAggregate) (feed as KeyedBucket<object>)["Top_Versions_Aggs"]).Hits<PackageVersionContract>().First<IHit<PackageVersionContract>>();

    protected ICollection<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Contracts.Highlight> CalculateHighlightSnippets(
      IHit<PackageVersionContract> hit,
      IReadOnlyCollection<string> highlightFields,
      DocumentContractType contractType)
    {
      List<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Contracts.Highlight> highlightSnippets = new List<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Contracts.Highlight>();
      highlightSnippets.AddRange((IEnumerable<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Contracts.Highlight>) this.CalculateCaseChangeFieldHighlights(hit, "name", "name.casechangeanalyzed", contractType));
      if (highlightFields != null)
      {
        foreach (string highlightField in (IEnumerable<string>) highlightFields)
        {
          if (hit.Highlight.ContainsKey(highlightField))
          {
            IReadOnlyCollection<string> highlights;
            hit.Highlight.TryGetValue(highlightField, out highlights);
            highlightSnippets.Add(new Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Contracts.Highlight(this.GetStoredFieldNameForElasticsearchName(highlightField, contractType), (IEnumerable<string>) highlights));
          }
        }
      }
      return (ICollection<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Contracts.Highlight>) highlightSnippets;
    }

    private ICollection<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Contracts.Highlight> CalculateCaseChangeFieldHighlights(
      IHit<PackageVersionContract> hit,
      string fieldName,
      string fieldNameCaseChange,
      DocumentContractType contractType)
    {
      List<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Contracts.Highlight> changeFieldHighlights = new List<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Contracts.Highlight>();
      if (hit.Highlight.ContainsKey(fieldName))
      {
        IReadOnlyCollection<string> highlights;
        hit.Highlight.TryGetValue(fieldName, out highlights);
        changeFieldHighlights.Add(new Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Contracts.Highlight(this.GetStoredFieldNameForElasticsearchName(fieldName, contractType), (IEnumerable<string>) highlights));
      }
      else if (hit.Highlight.ContainsKey(fieldNameCaseChange))
      {
        IReadOnlyCollection<string> highlights;
        hit.Highlight.TryGetValue(fieldNameCaseChange, out highlights);
        changeFieldHighlights.Add(new Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Contracts.Highlight(this.GetStoredFieldNameForElasticsearchName(fieldName, contractType), (IEnumerable<string>) highlights));
      }
      return (ICollection<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Contracts.Highlight>) changeFieldHighlights;
    }

    public override string GetStoredFieldNameForElasticsearchName(
      string field,
      DocumentContractType contractType)
    {
      return PackageEntityIndexProvider<T>.s_documentContractMapping.ContainsKey(contractType) ? PackageEntityIndexProvider<T>.s_documentContractMapping[contractType].GetFieldNameForStoredField(field) : field;
    }

    public override string GetStoredFieldValue(
      string field,
      string fieldValue,
      DocumentContractType contractType)
    {
      return PackageEntityIndexProvider<T>.s_documentContractMapping.ContainsKey(contractType) ? PackageEntityIndexProvider<T>.s_documentContractMapping[contractType].GetStoredFieldValue(field, fieldValue) : fieldValue;
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
      return (EntitySearchPlatformResponse) new PackageSearchPlatformResponse(totalMatches, (IList<SearchHit>) results, num != 0, facets);
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
      return (EntitySearchPlatformResponse) new PackageSearchPlatformResponse(totalMatches, (IList<SearchHit>) results, num != 0, facets);
    }

    protected override Dictionary<string, IEnumerable<string>> GetSearchHitSources(
      T sources,
      EntitySearchPlatformRequest request,
      ConcurrentBag<Exception> exceptions,
      ref bool isOperationSuccessful)
    {
      Dictionary<string, IEnumerable<string>> searchHitSources = new Dictionary<string, IEnumerable<string>>();
      if (sources is PackageVersionContract packageVersionContract && !string.IsNullOrWhiteSpace(packageVersionContract.CollectionName))
        searchHitSources.Add("collectionName", (IEnumerable<string>) new List<string>()
        {
          packageVersionContract.CollectionName
        });
      return searchHitSources;
    }

    public override void BuildCountComponents(
      IVssRequestContext requestContext,
      ResultsCountPlatformRequest request,
      string rawQueryString,
      string rawFilterString,
      out Func<QueryContainerDescriptor<T>, QueryContainer> query)
    {
      throw new NotImplementedException();
    }

    public override IExpression BuildQueryFilterExpression(
      IVssRequestContext requestContext,
      IDictionary<string, IEnumerable<string>> searchFilters,
      IExpression queryParseTree,
      DocumentContractType contractType)
    {
      throw new NotImplementedException();
    }

    public override void BuildSuggestComponents(
      IVssRequestContext requestContext,
      EntitySearchSuggestPlatformRequest suggestRequest,
      string rawFilterString,
      out Func<SuggestContainerDescriptor<T>, SuggestContainerDescriptor<T>> suggest)
    {
      suggest = new EntitySearchPhraseSuggesterBuilder(suggestRequest, rawFilterString).Suggest<T>();
    }
  }
}
