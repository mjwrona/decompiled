// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Implementations.ElasticSearch.EntityProviders.EntityIndexProvider`1
// Assembly: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EE1DF96-C85D-457F-AAA1-93619829BFD4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Arriba.Expressions;
using Microsoft.VisualStudio.Services.Search.Common.Enums;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Implementations.ElasticSearch.QueryBuilders;
using Nest;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Implementations.ElasticSearch.EntityProviders
{
  public abstract class EntityIndexProvider<T> : 
    EntityProvider,
    ISearchEntityQueryBuilder<T>,
    ISearchEntityResponseInterpreter<T>,
    ISearchEntityResponseInterpreter
    where T : class
  {
    public abstract void BuildSearchComponents(
      IVssRequestContext requestContext,
      EntitySearchPlatformRequest request,
      string rawQueryString,
      string rawFilterString,
      out Func<QueryContainerDescriptor<T>, QueryContainer> query,
      out Func<QueryContainerDescriptor<T>, QueryContainer> filter,
      out Func<AggregationContainerDescriptor<T>, AggregationContainerDescriptor<T>> aggregations,
      out Func<HighlightDescriptor<T>, IHighlight> highlight,
      out Func<IVssRequestContext, SortDescriptor<T>> sort);

    public abstract void BuildCountComponents(
      IVssRequestContext requestContext,
      ResultsCountPlatformRequest request,
      string rawQueryString,
      string rawFilterString,
      out Func<QueryContainerDescriptor<T>, QueryContainer> query);

    public abstract void BuildSuggestComponents(
      IVssRequestContext requestContext,
      EntitySearchSuggestPlatformRequest suggestRequest,
      string rawFilterString,
      out Func<SuggestContainerDescriptor<T>, SuggestContainerDescriptor<T>> suggest);

    public abstract IExpression BuildQueryFilterExpression(
      IVssRequestContext requestContext,
      IDictionary<string, IEnumerable<string>> searchFilters,
      IExpression queryParseTree,
      DocumentContractType contractType);

    public abstract EntitySearchPlatformResponse DefaultPlatformResponse(
      EntitySearchPlatformRequest request);

    public abstract EntitySearchPlatformResponse PreparePlatformResponse(
      int responseCount,
      bool isTimedOut,
      List<SearchHit> searchResults,
      IDictionary<string, IEnumerable<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Filter>> searchFacets);

    public abstract IEnumerable<string> GetFieldNames(
      IEnumerable<string> storedFields,
      DocumentContractType contractType);

    public abstract string GetStoredFieldNameForElasticsearchName(
      string field,
      DocumentContractType contractType);

    public abstract string GetStoredFieldValue(
      string field,
      string fieldValue,
      DocumentContractType contractType);

    public abstract IDictionary<string, IEnumerable<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Filter>> GetSearchFacets(
      EntitySearchPlatformRequest request,
      ISearchResponse<T> elasticSearchResponse);

    public abstract int GetTotalResultCount(
      EntitySearchPlatformRequest request,
      ISearchResponse<T> elasticSearchResponse);

    [SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "3#", Justification = "Passing isOperationSuccessful by reference is by design")]
    public abstract SearchHit GetSearchHit(
      IHit<T> hit,
      EntitySearchPlatformRequest request,
      ConcurrentBag<Exception> exceptions,
      ref bool isOperationSuccessful);

    public virtual List<SearchHit> GetSearchHits(
      EntitySearchPlatformRequest request,
      ISearchResponse<T> elasticSearchResponse)
    {
      IList<IHit<T>> hits = elasticSearchResponse.Hits as IList<IHit<T>>;
      List<SearchHit> results = new List<SearchHit>((IEnumerable<SearchHit>) new SearchHit[hits.Count]);
      bool isOperationSuccessful = true;
      ConcurrentBag<Exception> exceptions = new ConcurrentBag<Exception>();
      Parallel.For(0, hits.Count, (Action<int>) (i => results[i] = this.GetSearchHit(hits[i], request, exceptions, ref isOperationSuccessful)));
      if (!isOperationSuccessful)
        this.LogUnsuccessfulOperation(elasticSearchResponse, exceptions);
      return results;
    }

    public virtual EntitySearchPlatformResponse PreparePlatformResponse(
      AbstractSearchDocumentContract docContract,
      int responseCount,
      bool isTimedOut,
      List<SearchHit> searchResults,
      IDictionary<string, IEnumerable<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Filter>> searchFacets,
      string scrollId)
    {
      throw new NotImplementedException();
    }

    public List<SuggestOption> GetSuggestOptions(Suggest<T> suggest)
    {
      IReadOnlyCollection<ISuggestOption<T>> options = suggest.Options;
      List<SuggestOption> suggestOptions = new List<SuggestOption>();
      foreach (SuggestOption<T> suggestOption in (IEnumerable<ISuggestOption<T>>) options)
      {
        if (suggestOption.CollateMatch)
          suggestOptions.Add(new SuggestOption(suggestOption.Text, suggestOption.Score, suggestOption.CollateMatch));
      }
      return suggestOptions;
    }

    public virtual EntitySearchSuggestPlatformResponse DefaultSuggestPlatformResponse(
      string suggestSearchText)
    {
      return new EntitySearchSuggestPlatformResponse(0, (IEnumerable<SuggestOption>) new List<SuggestOption>(), false, suggestSearchText);
    }

    public EntitySearchSuggestPlatformResponse PrepareSuggestPlatformResponse(
      int responseCount,
      bool isTimedOut,
      List<SuggestOption> suggestions,
      string suggestText)
    {
      int count = responseCount;
      bool flag = isTimedOut;
      List<SuggestOption> suggestions1 = suggestions;
      int num = flag ? 1 : 0;
      string suggestText1 = suggestText;
      return new EntitySearchSuggestPlatformResponse(count, (IEnumerable<SuggestOption>) suggestions1, num != 0, suggestText1);
    }

    public virtual string GetQueryRequestTimeout(IVssRequestContext requestContext) => "62s";

    protected void LogUnsuccessfulOperation(
      ISearchResponse<T> elasticSearchResponse,
      ConcurrentBag<Exception> exceptions)
    {
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceWarningConditionally(1082217, "Search Engine", "Search Engine", new Func<string>(((IResponseExtensions) elasticSearchResponse).SerializeRequestAndResponse));
      Exception e = (Exception) null;
      if (exceptions.Count > 1)
        e = (Exception) new AggregateException("Exceptions caught while processing search hits.", (IEnumerable<Exception>) exceptions);
      else if (exceptions.Count == 1)
        e = exceptions.First<Exception>();
      if (e == null)
        return;
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceException(1082222, "Search Engine", "Search Engine", e);
    }

    protected Func<QueryContainerDescriptor<T>, QueryContainer> GetFilteredQuery(
      string rawQueryString,
      string rawFilterString)
    {
      return new Func<QueryContainerDescriptor<T>, QueryContainer>(new BoolQueryBuilder(rawQueryString, rawFilterString).BoolQuery<T>);
    }

    [SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "3#", Justification = "Passing isOperationSuccessful by reference is by design")]
    protected Dictionary<string, IEnumerable<string>> GetSearchHitFields(
      IHit<T> hit,
      EntitySearchPlatformRequest request,
      ConcurrentBag<Exception> exceptions,
      ref bool isOperationSuccessful)
    {
      if (request.ContractType.IsSourceOnAndApplicable())
        return this.GetSearchHitSources(hit.Source, request, exceptions, ref isOperationSuccessful);
      Dictionary<string, IEnumerable<string>> searchHitFields = new Dictionary<string, IEnumerable<string>>();
      if (hit.Fields == null || !hit.Fields.Any<KeyValuePair<string, LazyDocument>>())
      {
        exceptions.Add((Exception) new SearchPlatformException("ES Response: hit.fields cannot be null or empty"));
        isOperationSuccessful = false;
        return searchHitFields;
      }
      foreach (KeyValuePair<string, LazyDocument> field1 in (IEnumerable<KeyValuePair<string, LazyDocument>>) hit.Fields)
      {
        KeyValuePair<string, LazyDocument> field = field1;
        List<string> source = field.Value.As<List<string>>();
        if (source == null || source.Count == 0)
        {
          exceptions.Add((Exception) new SearchPlatformException("ES Response: Field Value cannot be null for field : " + field.Key));
          isOperationSuccessful = false;
          return searchHitFields;
        }
        searchHitFields.Add(this.GetStoredFieldNameForElasticsearchName(field.Key, request.ContractType), source.Select<string, string>((Func<string, string>) (x => this.GetStoredFieldValue(field.Key, x.ToString(), request.ContractType))));
      }
      return searchHitFields;
    }

    [SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "3#", Justification = "Passing isOperationSuccessful by reference is by design")]
    protected abstract Dictionary<string, IEnumerable<string>> GetSearchHitSources(
      T sources,
      EntitySearchPlatformRequest request,
      ConcurrentBag<Exception> exceptions,
      ref bool isOperationSuccessful);

    protected IDictionary<string, IEnumerable<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Filter>> PostFacetCleanup(
      EntitySearchPlatformRequest request,
      IDictionary<string, IEnumerable<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Filter>> searchFacets,
      IReadOnlyDictionary<string, string> parentOf)
    {
      foreach (string key in parentOf.Keys)
      {
        if (request.SearchFilters.ContainsKey(key))
        {
          string parentFilter = parentOf[key];
          if (searchFacets.ContainsKey(parentFilter))
            searchFacets[parentFilter] = (IEnumerable<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Filter>) new List<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Filter>()
            {
              searchFacets[parentFilter].First<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Filter>((Func<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Filter, bool>) (f => f.Name.Equals(request.SearchFilters[parentFilter].First<string>(), StringComparison.OrdinalIgnoreCase)))
            };
          else
            Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceError(1082278, "Search Engine", "Search Engine", FormattableString.Invariant(FormattableStringFactory.Create("Facet(s) for [{0}] returned but facet for [{1}] not returned.", (object) key, (object) parentFilter)));
        }
      }
      return searchFacets;
    }

    protected Dictionary<string, IReadOnlyCollection<string>> PruneHighlightsBasedOnPreferenceOrderForBaseField(
      IReadOnlyDictionary<string, IReadOnlyCollection<string>> highlights,
      IDictionary<string, IList<string>> baseFieldPreferenceOrderListMapping)
    {
      Dictionary<string, IReadOnlyCollection<string>> dictionary = highlights.ToDictionary<KeyValuePair<string, IReadOnlyCollection<string>>, string, IReadOnlyCollection<string>>((Func<KeyValuePair<string, IReadOnlyCollection<string>>, string>) (val => val.Key), (Func<KeyValuePair<string, IReadOnlyCollection<string>>, IReadOnlyCollection<string>>) (val => val.Value));
      if (baseFieldPreferenceOrderListMapping != null && baseFieldPreferenceOrderListMapping.Any<KeyValuePair<string, IList<string>>>())
      {
        List<string> stringList = new List<string>();
        foreach (KeyValuePair<string, IList<string>> keyValuePair in (IEnumerable<KeyValuePair<string, IList<string>>>) baseFieldPreferenceOrderListMapping)
        {
          KeyValuePair<string, IList<string>> preferenceOrderMapping = keyValuePair;
          if (dictionary.Keys.Any<string>((Func<string, bool>) (key => key.Contains(preferenceOrderMapping.Key))))
          {
            bool flag = false;
            foreach (string key in (IEnumerable<string>) preferenceOrderMapping.Value)
            {
              if (dictionary.ContainsKey(key) & flag)
                stringList.Add(key);
              else if (dictionary.Keys.Contains<string>(key, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) && !flag)
                flag = true;
            }
          }
        }
        foreach (string key in stringList)
          dictionary.Remove(key);
      }
      return dictionary;
    }

    protected KeyValuePair<string, IEnumerable<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Filter>> GetTermFacets(
      AggregateDictionary aggHelper,
      EntitySearchPlatformRequest request,
      string filterCategoryId,
      string[] termAggregationPath,
      bool logErrorForDuplicateFacets = false,
      Func<string, string> filterIdToDisplayName = null,
      Func<KeyedBucket<string>, Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Filter> aggregationBucketToFilter = null)
    {
      if (termAggregationPath == null || termAggregationPath.Length == 0)
        throw new ArgumentException(FormattableString.Invariant(FormattableStringFactory.Create("Terms aggregation path must contain at least one element representing the leaf terms aggregation name")), nameof (termAggregationPath));
      if (filterIdToDisplayName == null)
        filterIdToDisplayName = (Func<string, string>) (name => name);
      if (aggregationBucketToFilter == null)
        aggregationBucketToFilter = (Func<KeyedBucket<string>, Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Filter>) (bucket =>
        {
          string key = bucket.Key;
          return new Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Filter(bucket.Key, key, (int) bucket.DocCount.Value, false);
        });
      TermsAggregate<string> termsAggregate = new TermsAggregate<string>();
      AggregateDictionary aggregateDictionary = aggHelper;
      int num = termAggregationPath.Length - 1;
      for (int index = 0; index < num; ++index)
      {
        string key = termAggregationPath[index];
        if (aggregateDictionary.ContainsKey(key))
          aggregateDictionary = (AggregateDictionary) aggregateDictionary.Nested(key);
      }
      string key1 = termAggregationPath[termAggregationPath.Length - 1];
      if (aggregateDictionary.ContainsKey(key1))
        termsAggregate = aggregateDictionary.Terms(key1);
      IEnumerable<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Filter> filters = termsAggregate == null || termsAggregate.Buckets.Count <= 0 ? Enumerable.Empty<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Filter>() : termsAggregate.Buckets.Select<KeyedBucket<string>, Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Filter>(aggregationBucketToFilter);
      FriendlyDictionary<string, Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Filter> facetMap = new FriendlyDictionary<string, Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Filter>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      foreach (Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Filter filter in filters)
      {
        if (!facetMap.ContainsKey(filter.Name))
        {
          facetMap.Add(filter.Name, filter);
        }
        else
        {
          if (logErrorForDuplicateFacets)
            Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceError(1082276, "Search Engine", "Search Engine", FormattableString.Invariant(FormattableStringFactory.Create("Duplicate aggregation values found for filter category [{0}].", (object) filterCategoryId)));
          facetMap[filter.Name].ResultCount += filter.ResultCount;
        }
      }
      return EntityIndexProvider<T>.PreserveFilterSelectionStateAndSort(request.SearchFilters, filterCategoryId, (IDictionary<string, Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Filter>) facetMap, filterIdToDisplayName);
    }

    internal static KeyValuePair<string, IEnumerable<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Filter>> PreserveFilterSelectionStateAndSort(
      IDictionary<string, IEnumerable<string>> searchFilters,
      string filterCategoryId,
      IDictionary<string, Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Filter> facetMap,
      Func<string, string> filterIdToDisplayName)
    {
      if (searchFilters.ContainsKey(filterCategoryId))
      {
        foreach (string key1 in searchFilters[filterCategoryId])
        {
          if (facetMap.ContainsKey(key1))
          {
            facetMap[key1].Selected = true;
          }
          else
          {
            IDictionary<string, Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Filter> dictionary = facetMap;
            string key2 = key1;
            string id = key1;
            Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Filter filter = new Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Filter(filterIdToDisplayName(key1), id, 0, true);
            dictionary.Add(key2, filter);
          }
        }
      }
      return new KeyValuePair<string, IEnumerable<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Filter>>(filterCategoryId, (IEnumerable<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Filter>) facetMap.Values.OrderBy<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Filter, string>((Func<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Filter, string>) (filter => filter.Name), (IComparer<string>) StringComparer.OrdinalIgnoreCase));
    }
  }
}
