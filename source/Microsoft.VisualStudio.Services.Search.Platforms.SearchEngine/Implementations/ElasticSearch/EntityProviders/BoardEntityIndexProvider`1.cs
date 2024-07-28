// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Implementations.ElasticSearch.EntityProviders.BoardEntityIndexProvider`1
// Assembly: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EE1DF96-C85D-457F-AAA1-93619829BFD4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common.Arriba.Expressions;
using Microsoft.VisualStudio.Services.Search.Common.Enums;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities.Board;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Implementations.ElasticSearch.QueryBuilders;
using Nest;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Implementations.ElasticSearch.EntityProviders
{
  internal class BoardEntityIndexProvider<T> : EntityIndexProvider<T> where T : class
  {
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
      BoardSearchPlatformRequest searchPlatformRequest = request as BoardSearchPlatformRequest;
      query = this.GetFilteredQuery(rawQueryString, rawFilterString);
      filter = new Func<QueryContainerDescriptor<T>, QueryContainer>(new BoardFilterBuilder(searchPlatformRequest.SearchFilters).Filters<T>);
      aggregations = (Func<AggregationContainerDescriptor<T>, AggregationContainerDescriptor<T>>) null;
      highlight = (Func<HighlightDescriptor<T>, IHighlight>) null;
      sort = (Func<IVssRequestContext, SortDescriptor<T>>) null;
    }

    public override EntitySearchPlatformResponse DefaultPlatformResponse(
      EntitySearchPlatformRequest searchRequest)
    {
      return (EntitySearchPlatformResponse) new BoardSearchPlatformResponse(0, (IList<SearchHit>) new List<SearchHit>(), false, (IDictionary<string, IEnumerable<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Filter>>) new Dictionary<string, IEnumerable<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Filter>>());
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
      return (IDictionary<string, IEnumerable<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Filter>>) null;
    }

    public override SearchHit GetSearchHit(
      IHit<T> hit,
      EntitySearchPlatformRequest request,
      ConcurrentBag<Exception> exceptions,
      ref bool isOperationSuccessful)
    {
      if (!isOperationSuccessful)
        return (SearchHit) new BoardSearchHit();
      BoardVersionContract source = (object) hit.Source as BoardVersionContract;
      return (SearchHit) new BoardSearchHit(new BoardVersionContract(), new BoardRecord(source.CollectionName, source.ProjectName, source.ProjectId, source.TeamName, source.TeamId, source.BoardType));
    }

    public override int GetTotalResultCount(
      EntitySearchPlatformRequest request,
      ISearchResponse<T> elasticSearchResponse)
    {
      return (int) elasticSearchResponse.Total;
    }

    public override string GetStoredFieldValue(
      string field,
      string fieldValue,
      DocumentContractType contractType)
    {
      return AbstractSearchDocumentContract.CreateContract(contractType).GetStoredFieldValue(field, fieldValue);
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
      return (EntitySearchPlatformResponse) new BoardSearchPlatformResponse(totalMatches, (IList<SearchHit>) results, num != 0, facets);
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
      return (EntitySearchPlatformResponse) new BoardSearchPlatformResponse(totalMatches, (IList<SearchHit>) results, num != 0, facets);
    }

    public override List<SearchHit> GetSearchHits(
      EntitySearchPlatformRequest request,
      ISearchResponse<T> elasticSearchResponse)
    {
      IList<IHit<T>> hits = elasticSearchResponse.Hits as IList<IHit<T>>;
      List<SearchHit> results = new List<SearchHit>((IEnumerable<SearchHit>) new BoardSearchHit[hits.Count]);
      bool isOperationSuccessful = true;
      ConcurrentBag<Exception> exceptions = new ConcurrentBag<Exception>();
      Parallel.For(0, hits.Count, (Action<int>) (i => results[i] = this.GetSearchHit(hits[i], request, exceptions, ref isOperationSuccessful)));
      if (!isOperationSuccessful)
        this.LogUnsuccessfulOperation(elasticSearchResponse, exceptions);
      return results;
    }

    protected override Dictionary<string, IEnumerable<string>> GetSearchHitSources(
      T sources,
      EntitySearchPlatformRequest request,
      ConcurrentBag<Exception> exceptions,
      ref bool isOperationSuccessful)
    {
      Dictionary<string, IEnumerable<string>> searchHitSources = new Dictionary<string, IEnumerable<string>>();
      if (sources is BoardVersionContract boardVersionContract && !string.IsNullOrWhiteSpace(boardVersionContract.CollectionName))
        searchHitSources.Add("collectionName", (IEnumerable<string>) new List<string>()
        {
          boardVersionContract.CollectionName
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
      throw new NotImplementedException();
    }

    public override string GetStoredFieldNameForElasticsearchName(
      string field,
      DocumentContractType contractType)
    {
      throw new NotImplementedException();
    }
  }
}
