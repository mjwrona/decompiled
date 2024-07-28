// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Implementations.ElasticSearch.EntityProviders.CollectionPackageEntityIndexProvider`1
// Assembly: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EE1DF96-C85D-457F-AAA1-93619829BFD4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.dll

using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities.Package;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Implementations.ElasticSearch.QueryBuilders.Aggregation;
using Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts.Package;
using Nest;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Implementations.ElasticSearch.EntityProviders
{
  internal class CollectionPackageEntityIndexProvider<T> : PackageEntityIndexProvider<T> where T : class
  {
    public override IDictionary<string, IEnumerable<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Filter>> GetSearchFacets(
      EntitySearchPlatformRequest request,
      ISearchResponse<T> elasticSearchResponse)
    {
      FriendlyDictionary<string, IEnumerable<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Filter>> searchFacets = new FriendlyDictionary<string, IEnumerable<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Filter>>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      AggregateDictionary aggregations = elasticSearchResponse.Aggregations;
      if (aggregations != null)
      {
        searchFacets.Add(this.GetTermFacets(aggregations, request, PackageSearchFilterCategories.ProtocolType, new string[2]
        {
          "filtered_protocol_tags_aggs",
          "protocol_aggs"
        }, aggregationBucketToFilter: new Func<KeyedBucket<string>, Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Filter>(Selector)));
        searchFacets.Add(this.GetTermFacets(aggregations, request, PackageSearchFilterCategories.Feeds, new string[2]
        {
          "filtered_feed_aggs",
          "feed_aggs"
        }, aggregationBucketToFilter: new Func<KeyedBucket<string>, Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Filter>(Selector)));
        if ((!request.SearchFilters.ContainsKey(PackageSearchFilterCategories.Feeds) ? 0 : (request.SearchFilters[PackageSearchFilterCategories.Feeds].ToList<string>().Count == 1 ? 1 : 0)) != 0)
          searchFacets.Add(this.GetTermFacets(aggregations, request, PackageSearchFilterCategories.View, new string[2]
          {
            "filtered_view_aggs",
            "view_aggs"
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

    protected override Func<AggregationContainerDescriptor<T>, AggregationContainerDescriptor<T>> GetAggregationBuilder(
      PackageSearchPlatformRequest request)
    {
      return new Func<AggregationContainerDescriptor<T>, AggregationContainerDescriptor<T>>(new CollectionPackageAggregationBuilder(request.SearchFilters, request.Options.HasFlag((Enum) SearchOptions.Faceting), request.TakeAggResults, request.Options.HasFlag((Enum) SearchOptions.Highlighting)).Aggregates<T>);
    }

    public override List<SearchHit> GetSearchHits(
      EntitySearchPlatformRequest request,
      ISearchResponse<T> elasticSearchResponse)
    {
      IAggregate aggregate;
      IList<IBucket> bucketList = !elasticSearchResponse.Aggregations.TryGetValue("Filtered_Results_Aggs", out aggregate) ? ((BucketAggregate) elasticSearchResponse.Aggregations["Package_Aggs"]).Items as IList<IBucket> : ((BucketAggregate) ((IsAReadOnlyDictionaryBase<string, IAggregate>) aggregate)["Package_Aggs"]).Items as IList<IBucket>;
      List<SearchHit> searchHits = new List<SearchHit>();
      bool isOperationSuccessful = true;
      ConcurrentBag<Exception> exceptions = new ConcurrentBag<Exception>();
      foreach (IBucket bucket in (IEnumerable<IBucket>) bucketList)
        searchHits.AddRange((IEnumerable<SearchHit>) this.GetSearchHit(bucket, request, ref exceptions, ref isOperationSuccessful));
      if (!isOperationSuccessful)
        this.LogUnsuccessfulOperation(elasticSearchResponse, exceptions);
      return searchHits;
    }

    private List<PackageSearchHit> GetSearchHit(
      IBucket bucket,
      EntitySearchPlatformRequest request,
      ref ConcurrentBag<Exception> exceptions,
      ref bool isOperationSuccessful)
    {
      if (!isOperationSuccessful)
        return new List<PackageSearchHit>();
      List<IHit<PackageVersionContract>> list = ((BucketAggregate) (bucket as KeyedBucket<object>)["Feed_Aggs"]).Items.Select<IBucket, IHit<PackageVersionContract>>(new Func<IBucket, IHit<PackageVersionContract>>(((PackageEntityIndexProvider<T>) this).ExtractHit)).ToList<IHit<PackageVersionContract>>();
      list.Sort((IComparer<IHit<PackageVersionContract>>) new PackageVersionContractComparer());
      Hit<PackageVersionContract> hit1 = (Hit<PackageVersionContract>) null;
      List<PackageSearchHit> searchHit = new List<PackageSearchHit>();
      foreach (Hit<PackageVersionContract> hit2 in list)
      {
        List<PackageFeed> feeds = new List<PackageFeed>();
        PackageVersionContract source = hit2.Source;
        feeds.Add(new PackageFeed(source.CollectionId, source.CollectionName, source.CollectionUrl, source.FeedId, source.FeedName, source.PackageId, source.Version, (IEnumerable<PackageViewInfo>) source.Views));
        if (hit1 == null)
          hit1 = hit2;
        if (hit1.Highlight == null)
        {
          exceptions.Add((Exception) new SearchPlatformException("ES Response: hit.highlights cannot be null"));
          isOperationSuccessful = false;
          return new List<PackageSearchHit>();
        }
        ICollection<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Contracts.Highlight> highlightSnippets = this.CalculateHighlightSnippets((IHit<PackageVersionContract>) hit1, this.GetHighlightFields(), request.ContractType);
        searchHit.Add(new PackageSearchHit((IEnumerable<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Contracts.Highlight>) highlightSnippets, hit1.Source, feeds));
      }
      return searchHit;
    }
  }
}
