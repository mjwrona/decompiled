// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Implementations.ElasticSearchQueryClient
// Assembly: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EE1DF96-C85D-457F-AAA1-93619829BFD4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.dll

using Elasticsearch.Net;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.BusinessIntelligence;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Arriba.Expressions;
using Microsoft.VisualStudio.Services.Search.Common.Entities;
using Microsoft.VisualStudio.Services.Search.Common.Enums;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.ElasticSearch.QueryBuilders;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Implementations.ElasticSearch.EntityProviders;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Implementations.ElasticSearch.Profilers;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Implementations.ElasticSearch.QueryBuilders;
using Nest;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Implementations
{
  internal class ElasticSearchQueryClient : ISearchQueryClient
  {
    private IElasticClient ElasticClient { get; set; }

    public ElasticSearchQueryClient(IElasticClient elasticClient) => this.ElasticClient = elasticClient;

    public EntitySearchSuggestPlatformResponse Suggest<T>(
      IVssRequestContext requestContext,
      EntitySearchPlatformRequest request,
      EntitySearchSuggestPlatformRequest searchSuggestRequest,
      IEntityType entityType,
      EntityIndexProvider<T> entityProvider)
      where T : AbstractSearchDocumentContract
    {
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceEnter(1082303, "Search Engine", "Search Engine", nameof (Suggest));
      try
      {
        entityProvider.ValidateQueryRequest(request);
        entityProvider.ValidateSuggestRequest(searchSuggestRequest);
        bool flag = false;
        SearchDescriptor<T> searchDescriptorSuggest = (SearchDescriptor<T>) null;
        string elasticSearchQuery = request.ScopeFiltersExpression.ToElasticSearchQuery(requestContext, entityType, request.ContractType, false, false, request.RequestId, (ResultsCountPlatformRequest) request);
        Func<SuggestContainerDescriptor<T>, SuggestContainerDescriptor<T>> suggest;
        entityProvider.BuildSuggestComponents(requestContext, searchSuggestRequest, elasticSearchQuery, out suggest);
        Stopwatch stopwatch = Stopwatch.StartNew();
        ISearchResponse<T> searchResponse;
        try
        {
          searchDescriptorSuggest = new SearchDescriptor<T>().Index((Indices) Indices.Index(request.Indices)).Size(new int?(0)).Suggest((Func<SuggestContainerDescriptor<T>, IPromise<ISuggestContainer>>) suggest);
          if (request.Routing != null && ((IEnumerable<string>) request.Routing).All<string>((Func<string, bool>) (r => !string.IsNullOrWhiteSpace(r))))
            searchDescriptorSuggest = searchDescriptorSuggest.Routing((Routing) request.Routing);
          searchResponse = this.ElasticClient.Search<T>((Func<SearchDescriptor<T>, ISearchRequest>) (s => (ISearchRequest) searchDescriptorSuggest));
          stopwatch.Stop();
          flag = ElasticSearchQueryClient.ValidateQueryResponse<T>(requestContext, searchResponse);
        }
        catch (Exception ex) when (!(ex is SearchPlatformException))
        {
          throw new SearchPlatformException("Phrase Suggester query to ElasticSearch failed", ex);
        }
        finally
        {
          if (!flag && searchDescriptorSuggest != null)
            Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishClientTraceMessage("Search Engine", "Search Engine", Microsoft.VisualStudio.Services.WebPlatform.Level.Error, FormattableString.Invariant(FormattableStringFactory.Create("Elasticsearch phrase suggester request returned invalid response:{0}", (object) Environment.NewLine)) + FormattableString.Invariant(FormattableStringFactory.Create("Indices={0}{1}", request.Indices != null ? (object) string.Join<IndexName>(",", request.Indices) : (object) string.Empty, (object) Environment.NewLine)) + FormattableString.Invariant(FormattableStringFactory.Create("Type={0}{1}", (object) request.ContractType.GetMappingName(), (object) Environment.NewLine)) + FormattableString.Invariant(FormattableStringFactory.Create("Routing={0}{1}", request.Routing != null ? (object) string.Join(",", request.Routing) : (object) string.Empty, (object) Environment.NewLine)) + FormattableString.Invariant(FormattableStringFactory.Create("RequestBody={0}", (object) Encoding.UTF8.GetString(this.ElasticClient.SourceSerializer.SerializeToBytes<SearchDescriptor<T>>(searchDescriptorSuggest, SerializationFormatting.None)).CompactJson())));
        }
        long took = searchResponse.Took;
        long elapsedMilliseconds = stopwatch.ElapsedMilliseconds;
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishKpiAndCi("ESReportedQueryTime", "Query Pipeline", (double) took, true);
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishKpiAndCi("E2EPlatformQueryTime", "Query Pipeline", (double) elapsedMilliseconds, true);
        return ElasticSearchQueryClient.PrepareSuggestQueryResponse<T>(searchSuggestRequest, searchResponse, entityProvider);
      }
      finally
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceLeave(1082304, "Search Engine", "Search Engine", "ElasticSearchIndex.Suggest");
      }
    }

    public EntitySearchPlatformResponse Search<T>(
      IVssRequestContext requestContext,
      EntitySearchPlatformRequest request,
      IEnumerable<EntitySearchField> storedFields,
      IEntityType entityType,
      EntityIndexProvider<T> entityProvider)
      where T : AbstractSearchDocumentContract
    {
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceEnter(1082218, "Search Engine", "Search Engine", nameof (Search));
      try
      {
        bool flag1 = false;
        if (request == null)
          throw new ArgumentNullException(nameof (request));
        if (!string.IsNullOrEmpty(request.ScrollId) && requestContext.IsFeatureEnabled("Search.Server.ScrollSearchQuery"))
        {
          request.ContinueOnEmptyQuery = true;
          Stopwatch stopwatch1 = Stopwatch.StartNew();
          ISearchResponse<T> searchResponse = this.ElasticClient.Scroll<T>((Time) requestContext.GetCurrentHostConfigValue<string>("/Service/ALMSearch/Settings/Controller/CodeSearchScrollTime", defaultValue: "1m"), request.ScrollId);
          stopwatch1.Stop();
          EntitySearchPlatformResponse entitySearchPlatformResponse;
          try
          {
            bool flag2 = ElasticSearchQueryClient.ValidateQueryResponse<T>(requestContext, searchResponse);
            entitySearchPlatformResponse = ElasticSearchQueryClient.PrepareSearchQueryResponse<T>(request, searchResponse, entityProvider, searchResponse.ScrollId);
            if (searchResponse.Hits.Count == 0)
            {
              this.ElasticClient.ClearScroll((IClearScrollRequest) new ClearScrollRequest(request.ScrollId));
              entitySearchPlatformResponse = ElasticSearchQueryClient.PrepareSearchQueryResponse<T>(request, searchResponse, entityProvider, (string) null);
            }
            if (!flag2)
              this.LogForInvalidSearchResponse<T>(request, new SearchDescriptor<T>(), requestContext);
            ElasticSearchQueryClient.TraceMeasureSearchResponse<T>(requestContext, searchResponse, entitySearchPlatformResponse, stopwatch1);
          }
          catch
          {
            this.LogForInvalidSearchResponse<T>(request, new SearchDescriptor<T>(), requestContext);
            entitySearchPlatformResponse = ElasticSearchQueryClient.PrepareSearchQueryResponse<T>(request, searchResponse, entityProvider, string.Empty);
          }
          return entitySearchPlatformResponse;
        }
        entityProvider.ValidateQueryRequest(request);
        string rawQueryString;
        string rawFilterString;
        ElasticSearchQueryClient.ConvertQueryAndFilterToElasticSearchQuery(requestContext, entityType, (ResultsCountPlatformRequest) request, request.ScopeFiltersExpression, request.Options.HasFlag((Enum) SearchOptions.Ranking), request.Options.HasFlag((Enum) SearchOptions.AllowSpellingErrors), out rawQueryString, out rawFilterString);
        if (string.IsNullOrWhiteSpace(rawQueryString) && !request.ContinueOnEmptyQuery)
        {
          Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceWarning(1082223, "Search Engine", "Search Engine", "Query string is empty. Empty results with selected facets will be returned.");
          return entityProvider.DefaultPlatformResponse(request);
        }
        Func<QueryContainerDescriptor<T>, QueryContainer> query;
        Func<QueryContainerDescriptor<T>, QueryContainer> filter;
        Func<AggregationContainerDescriptor<T>, AggregationContainerDescriptor<T>> aggregations;
        Func<HighlightDescriptor<T>, IHighlight> highlight;
        Func<IVssRequestContext, SortDescriptor<T>> sort;
        entityProvider.BuildSearchComponents(requestContext, request, rawQueryString, rawFilterString, out query, out filter, out aggregations, out highlight, out sort);
        string[] storedFieldsActual = storedFields.Select<EntitySearchField, string>((Func<EntitySearchField, string>) (x => x.ElasticsearchFieldName)).ToArray<string>();
        SearchDescriptor<T> searchDescriptor = (SearchDescriptor<T>) null;
        bool flag3 = false;
        Stopwatch stopwatch = Stopwatch.StartNew();
        ISearchResponse<T> searchResponse1;
        try
        {
          searchDescriptor = new SearchDescriptor<T>().Timeout(entityProvider.GetQueryRequestTimeout(requestContext)).Index((Indices) Indices.Index(request.Indices)).Query(query).Highlight(highlight);
          if (request.ScrollSize > 0 && request.ScrollSize > requestContext.GetCurrentHostConfigValueOrDefault("/Service/ALMSearch/Settings/Controller/CodeSearchTakeResultsLimit", 200) && requestContext.IsFeatureEnabled("Search.Server.ScrollSearchQuery"))
          {
            searchDescriptor.Scroll((Time) requestContext.GetCurrentHostConfigValue<string>("/Service/ALMSearch/Settings/Controller/CodeSearchScrollTime", defaultValue: "1m")).Size(new int?(request.ScrollSize)).PostFilter(filter).Sort((Func<SortDescriptor<T>, IPromise<IList<ISort>>>) (x => (IPromise<IList<ISort>>) x.Ascending(SortSpecialField.DocumentIndexOrder)));
            flag3 = true;
          }
          else
          {
            searchDescriptor.Skip(new int?(request.SkipResults)).PostFilter(filter).Take(new int?(request.TakeResults));
            if (aggregations != null)
              searchDescriptor.Aggregations((Func<AggregationContainerDescriptor<T>, IAggregationContainer>) aggregations);
            SortDescriptor<T> sortDescriptor;
            if (sort != null && (sortDescriptor = sort(requestContext)) != null)
            {
              searchDescriptor.Sort((Func<SortDescriptor<T>, IPromise<IList<ISort>>>) (s => (IPromise<IList<ISort>>) sortDescriptor));
            }
            else
            {
              Func<RescoringDescriptor<T>, IPromise<IList<IRescore>>> rescoreSelector = EntityRescoreProvider.GetProvider(entityType, request.ContractType, request.QueryParseTree, request.Options.HasFlag((Enum) SearchOptions.Rescore)).Rescore<T>(requestContext);
              searchDescriptor.Rescore(rescoreSelector);
            }
          }
          if (request.ContractType.IsSourceOnAndApplicable())
          {
            searchDescriptor = searchDescriptor.Source((Func<SourceFilterDescriptor<T>, ISourceFilter>) (s => (ISourceFilter) s.Includes((Func<FieldsDescriptor<T>, IPromise<Fields>>) (i => (IPromise<Fields>) i.Fields(storedFieldsActual)))));
          }
          else
          {
            searchDescriptor.Source(false);
            searchDescriptor = searchDescriptor.StoredFields((Fields) storedFieldsActual);
          }
          if (request.Routing != null && ((IEnumerable<string>) request.Routing).All<string>((Func<string, bool>) (r => !string.IsNullOrWhiteSpace(r))))
            searchDescriptor = searchDescriptor.Routing((Routing) request.Routing);
          bool traceForProfiling = false;
          string configValue = requestContext.GetConfigValue<string>("/Service/ALMSearch/Settings/ESQueryProfiler", TeamFoundationHostType.Deployment, true, "");
          if (!string.IsNullOrWhiteSpace(configValue))
          {
            ProfilerFactory profilerFactory = new ProfilerFactory(requestContext.GetConfigValue<int>("/Service/ALMSearch/Settings/ProfileESQueriesProbability", TeamFoundationHostType.Deployment, true), entityType);
            ProfilerType result;
            if (Enum.TryParse<ProfilerType>(configValue, out result) && profilerFactory.GetProfiler(result).ShouldProfile(requestContext, request))
            {
              searchDescriptor.Profile();
              traceForProfiling = true;
            }
          }
          searchResponse1 = this.ElasticClient.Search<T>((Func<SearchDescriptor<T>, ISearchRequest>) (s => (ISearchRequest) searchDescriptor));
          stopwatch.Stop();
          flag1 = ElasticSearchQueryClient.ValidateQueryResponse<T>(requestContext, searchResponse1, traceForProfiling);
        }
        catch (Exception ex) when (!(ex is SearchPlatformException))
        {
          throw new SearchPlatformException("Query to ElasticSearch failed", ex);
        }
        finally
        {
          if (!flag1)
            this.LogForInvalidSearchResponse<T>(request, searchDescriptor, requestContext);
          else
            Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceVerboseConditionally(1083163, "Query Pipeline", "Search Engine", (Func<string>) (() => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Query ran on Indices: {0}, Time taken: {1} ms", request.Indices != null ? (object) string.Join<IndexName>(",", request.Indices) : (object) string.Empty, (object) stopwatch.ElapsedMilliseconds)));
        }
        EntitySearchPlatformResponse entitySearchPlatformResponse1;
        if (flag3 && searchResponse1.Total <= (long) request.ScrollSize)
        {
          this.ElasticClient.ClearScroll((IClearScrollRequest) new ClearScrollRequest(searchResponse1.ScrollId));
          entitySearchPlatformResponse1 = ElasticSearchQueryClient.PrepareSearchQueryResponse<T>(request, searchResponse1, entityProvider, (string) null);
        }
        else
          entitySearchPlatformResponse1 = ElasticSearchQueryClient.PrepareSearchQueryResponse<T>(request, searchResponse1, entityProvider, searchResponse1.ScrollId);
        ElasticSearchQueryClient.TraceMeasureSearchResponse<T>(requestContext, searchResponse1, entitySearchPlatformResponse1, stopwatch);
        return entitySearchPlatformResponse1;
      }
      finally
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceLeave(1082219, "Search Engine", "Search Engine", "ElasticSearchIndex.Search");
      }
    }

    public ResultsCountPlatformResponse Count<T>(
      IVssRequestContext requestContext,
      ResultsCountPlatformRequest request,
      IEntityType entityType,
      EntityIndexProvider<T> entityProvider)
      where T : AbstractSearchDocumentContract
    {
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceEnter(1080023, "Search Engine", "Search Engine", nameof (Count));
      try
      {
        entityProvider.ValidateCountRequest(request);
        IExpression expression = entityProvider.BuildQueryFilterExpression(requestContext, request.SearchFilters, request.QueryParseTree, request.ContractType);
        IExpression filters = (IExpression) new AndExpression(new IExpression[2]
        {
          request.ScopeFiltersExpression,
          expression
        });
        string rawQueryString;
        string rawFilterString;
        ElasticSearchQueryClient.ConvertQueryAndFilterToElasticSearchQuery(requestContext, entityType, request, filters, false, false, out rawQueryString, out rawFilterString);
        if (string.IsNullOrWhiteSpace(rawQueryString))
        {
          Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceWarning(1080028, "Search Engine", "Search Engine", "Query string is empty. Results with count equals 0 will be returned.");
          return ResultsCountPlatformResponse.DefaultResultsCountPlatformResponse();
        }
        Func<QueryContainerDescriptor<T>, QueryContainer> query;
        entityProvider.BuildCountComponents(requestContext, request, rawQueryString, rawFilterString, out query);
        bool flag = false;
        CountDescriptor<T> countDescriptor = (CountDescriptor<T>) null;
        Stopwatch stopwatch = Stopwatch.StartNew();
        CountResponse countResponse;
        try
        {
          countDescriptor = new CountDescriptor<T>().Index((Indices) Indices.Index(request.Indices)).Query(query);
          if (request.Routing != null && ((IEnumerable<string>) request.Routing).All<string>((Func<string, bool>) (r => !string.IsNullOrWhiteSpace(r))))
          {
            string str = string.Join(",", request.Routing);
            if (!string.IsNullOrWhiteSpace(str))
              countDescriptor = countDescriptor.Routing((Routing) str);
          }
          countDescriptor.TerminateAfter(new long?((long) request.TerminateAfter));
          countResponse = this.ElasticClient.Count((ICountRequest) countDescriptor);
          stopwatch.Stop();
          flag = ElasticSearchQueryClient.ValidateCountResponse(countResponse);
        }
        catch (Exception ex) when (!(ex is SearchPlatformException))
        {
          throw new SearchPlatformException("Count request to ElasticSearch failed", ex);
        }
        finally
        {
          if (!flag)
          {
            if (countDescriptor != null)
              Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishClientTraceMessage("Search Engine", "Search Engine", Microsoft.VisualStudio.Services.WebPlatform.Level.Error, FormattableString.Invariant(FormattableStringFactory.Create("Elasticsearch search request returned invalid response:{0}", (object) Environment.NewLine)) + FormattableString.Invariant(FormattableStringFactory.Create("Indices={0}{1}", request.Indices != null ? (object) string.Join<IndexName>(",", request.Indices) : (object) string.Empty, (object) Environment.NewLine)) + FormattableString.Invariant(FormattableStringFactory.Create("Type={0}{1}", (object) request.ContractType.GetMappingName(), (object) Environment.NewLine)) + FormattableString.Invariant(FormattableStringFactory.Create("Routing={0}{1}", request.Routing != null ? (object) string.Join(",", request.Routing) : (object) string.Empty, (object) Environment.NewLine)) + FormattableString.Invariant(FormattableStringFactory.Create("RequestBody={0}", (object) Encoding.UTF8.GetString(this.ElasticClient.SourceSerializer.SerializeToBytes<CountDescriptor<T>>(countDescriptor)).CompactJson())));
          }
          else
            Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceVerboseConditionally(1083160, "Query Pipeline", "Search Engine", (Func<string>) (() => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Query ran on Indices: {0}, Time taken: {1} ms", request.Indices != null ? (object) string.Join<IndexName>(",", request.Indices) : (object) string.Empty, (object) stopwatch.ElapsedMilliseconds)));
        }
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishKpiAndCi("E2EPlatformCountRequestTime", "Query Pipeline", (double) stopwatch.ElapsedMilliseconds, true);
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishKpiAndCi("NoOfFileHitsForCountRequest", "Query Pipeline", (double) countResponse.Count);
        return ElasticSearchQueryClient.PrepareCountRequestResponse(countResponse);
      }
      finally
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceLeave(1080024, "Search Engine", "Search Engine", nameof (Count));
      }
    }

    private static void ConvertQueryAndFilterToElasticSearchQuery(
      IVssRequestContext requestContext,
      IEntityType entityType,
      ResultsCountPlatformRequest request,
      IExpression filters,
      bool enableRanking,
      bool allowSpellingErrors,
      out string rawQueryString,
      out string rawFilterString)
    {
      IExpression expression1;
      if (!(request.QueryParseTree is EmptyExpression))
        expression1 = (IExpression) new AndExpression(new IExpression[2]
        {
          request.QueryParseTree,
          ElasticSearchQueryClient.GetContractTypeFilter(request.IndexInfo, request.ContractType)
        });
      else
        expression1 = request.QueryParseTree;
      IExpression expression2 = expression1;
      rawQueryString = expression2.ToElasticSearchQuery(requestContext, entityType, request.ContractType, enableRanking, allowSpellingErrors, request.RequestId, request);
      DocumentContractType childContractType = request.ContractType.GetChildContractType();
      if (childContractType != DocumentContractType.Unsupported)
      {
        IExpression expression3;
        if (!(request.QueryParseTree is EmptyExpression))
          expression3 = (IExpression) new AndExpression(new IExpression[2]
          {
            request.QueryParseTree,
            ElasticSearchQueryClient.GetContractTypeFilter(request.IndexInfo, childContractType)
          });
        else
          expression3 = request.QueryParseTree;
        IVssRequestContext requestContext1 = requestContext;
        IEntityType entityType1 = entityType;
        int contractType = (int) childContractType;
        int num1 = enableRanking ? 1 : 0;
        int num2 = allowSpellingErrors ? 1 : 0;
        string requestId = request.RequestId;
        ResultsCountPlatformRequest request1 = request;
        string elasticSearchQuery = expression3.ToElasticSearchQuery(requestContext1, entityType1, (DocumentContractType) contractType, num1 != 0, num2 != 0, requestId, request1);
        ParentChildQueryBuilder childQueryBuilder = new ParentChildQueryBuilder(rawQueryString, elasticSearchQuery, childContractType, request.MaxInnerHits);
        rawQueryString = childQueryBuilder.GetParentChildQueryRawString();
      }
      rawFilterString = filters.ToElasticSearchQuery(requestContext, entityType, request.ContractType, false, false, request.RequestId, request);
    }

    internal static bool ValidateQueryResponse<T>(
      IVssRequestContext requestContext,
      ISearchResponse<T> queryResponse,
      bool traceForProfiling = false)
      where T : class
    {
      if (requestContext.GetConfigValue<bool>("/Service/ALMSearch/Settings/LogNestAuditTrail"))
        ElasticSearchQueryClient.LogNestAuditTrail<T>(requestContext, queryResponse);
      if (traceForProfiling)
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceAlways(1083071, TraceLevel.Verbose, "Search Engine", "Search Engine", queryResponse.SerializeResponseProfile<T>());
      ElasticSearchQueryClient.HandleNullOrInvalidResponse((IResponse) queryResponse);
      if (queryResponse.Hits == null)
        throw new SearchPlatformException("ElasticSearch Response Hits is null", queryResponse.OriginalException);
      if (queryResponse.Shards.Failed <= 0)
        return true;
      ElasticSearchQueryClient.HandleESShardFailure(queryResponse.Shards);
      return false;
    }

    internal static bool ValidateCountResponse(CountResponse countResponse)
    {
      ElasticSearchQueryClient.HandleNullOrInvalidResponse((IResponse) countResponse);
      if (countResponse.Shards.Failed <= 0)
        return true;
      ElasticSearchQueryClient.HandleESShardFailure(countResponse.Shards);
      return false;
    }

    private static void HandleESShardFailure(ShardStatistics shards)
    {
      StringBuilder stringBuilder = new StringBuilder("Elasticsearch query failed on " + shards.Failed.ToString((IFormatProvider) CultureInfo.InvariantCulture) + " shard(s):" + Environment.NewLine);
      foreach (ShardFailure failure in (IEnumerable<ShardFailure>) shards.Failures)
        stringBuilder.AppendLine(failure.Serialize<ShardFailure>(true));
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishClientTraceMessage("Search Engine", "Search Engine", Microsoft.VisualStudio.Services.WebPlatform.Level.Error, stringBuilder.ToString());
    }

    private static void HandleNullOrInvalidResponse(IResponse response)
    {
      if (response == null)
        throw new SearchPlatformException("ElasticSearch Response is null", SearchServiceErrorCode.ElasticSearch_ResponseNull_Error);
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishClientTrace("Search Engine", "Search Engine", new ClientTraceData((IDictionary<string, object>) new FriendlyDictionary<string, object>()
      {
        ["ElasticsearchSearchRequestUrl"] = (object) response.ApiCall?.Uri
      }));
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceVerboseConditionally(1082217, "Search Engine", "Search Engine", new Func<string>(((IResponseExtensions) response).SerializeRequestAndResponse));
      response.ThrowOnInvalidOrFailedResponse();
    }

    private static EntitySearchPlatformResponse PrepareSearchQueryResponse<T>(
      EntitySearchPlatformRequest request,
      ISearchResponse<T> elasticSearchResponse,
      EntityIndexProvider<T> entityProvider,
      string scrollId)
      where T : class
    {
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceEnter(1082220, "Search Engine", "Search Engine", nameof (PrepareSearchQueryResponse));
      try
      {
        List<SearchHit> searchHits = entityProvider.GetSearchHits(request, elasticSearchResponse);
        IDictionary<string, IEnumerable<Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Filter>> searchFacets = entityProvider.GetSearchFacets(request, elasticSearchResponse);
        int totalResultCount = entityProvider.GetTotalResultCount(request, elasticSearchResponse);
        return entityProvider.PreparePlatformResponse(request.DocumentContract, totalResultCount, elasticSearchResponse.TimedOut, searchHits, searchFacets, scrollId);
      }
      finally
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceLeave(1082221, "Search Engine", "Search Engine", nameof (PrepareSearchQueryResponse));
      }
    }

    private static EntitySearchSuggestPlatformResponse PrepareSuggestQueryResponse<T>(
      EntitySearchSuggestPlatformRequest request,
      ISearchResponse<T> elasticSearchResponse,
      EntityIndexProvider<T> entityProvider)
      where T : class
    {
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceEnter(1082305, "Search Engine", "Search Engine", nameof (PrepareSuggestQueryResponse));
      try
      {
        if (!elasticSearchResponse.Suggest.ContainsKey(request.SuggestQueryName))
          throw new SearchPlatformException(FormattableString.Invariant(FormattableStringFactory.Create("Phrase suggester [{0}] not found.", (object) request.SuggestQueryName)));
        ISuggest<T> suggest = ((IEnumerable<ISuggest<T>>) (elasticSearchResponse.Suggest[request.SuggestQueryName] ?? throw new SearchPlatformException("Phrase suggester object is null"))).First<ISuggest<T>>();
        List<SuggestOption> suggestOptions = entityProvider.GetSuggestOptions((Nest.Suggest<T>) suggest);
        return entityProvider.PrepareSuggestPlatformResponse(suggestOptions.Count, elasticSearchResponse.TimedOut, suggestOptions, suggest.Text);
      }
      finally
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceLeave(1082306, "Search Engine", "Search Engine", nameof (PrepareSuggestQueryResponse));
      }
    }

    private static ResultsCountPlatformResponse PrepareCountRequestResponse(
      CountResponse elasticSearchResponse)
    {
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceEnter(1080026, "Search Engine", "Search Engine", nameof (PrepareCountRequestResponse));
      ResultsCountPlatformResponse platformResponse = new ResultsCountPlatformResponse((int) elasticSearchResponse.Count);
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceLeave(1080027, "Search Engine", "Search Engine", nameof (PrepareCountRequestResponse));
      return platformResponse;
    }

    private void LogForInvalidSearchResponse<T>(
      EntitySearchPlatformRequest request,
      SearchDescriptor<T> searchDescriptor,
      IVssRequestContext requestContext)
      where T : class
    {
      string message = FormattableString.Invariant(FormattableStringFactory.Create("Elasticsearch search request returned invalid response:{0}", (object) Environment.NewLine)) + FormattableString.Invariant(FormattableStringFactory.Create("Indices={0}{1}", request.Indices != null ? (object) string.Join<IndexName>(",", request.Indices) : (object) string.Empty, (object) Environment.NewLine)) + FormattableString.Invariant(FormattableStringFactory.Create("Type={0}{1}", (object) request.ContractType.GetMappingName(), (object) Environment.NewLine)) + FormattableString.Invariant(FormattableStringFactory.Create("Routing={0}{1}", request.Routing != null ? (object) string.Join(",", request.Routing) : (object) string.Empty, (object) Environment.NewLine));
      if (searchDescriptor != null)
        message += FormattableString.Invariant(FormattableStringFactory.Create("RequestBody={0}", (object) Encoding.UTF8.GetString(this.ElasticClient.SourceSerializer.SerializeToBytes<SearchDescriptor<T>>(searchDescriptor)).CompactJson()));
      else if (requestContext.IsFeatureEnabled("Search.Server.ScrollSearchQuery"))
        message += FormattableString.Invariant(FormattableStringFactory.Create("SearchScrollId={0}", (object) request.ScrollId));
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishClientTraceMessage("Search Engine", "Search Engine", Microsoft.VisualStudio.Services.WebPlatform.Level.Error, message);
    }

    internal static void TraceMeasureSearchResponse<T>(
      IVssRequestContext requestContext,
      ISearchResponse<T> queryResponse,
      EntitySearchPlatformResponse entitySearchPlatformResponse,
      Stopwatch stopwatch)
      where T : class
    {
      long took = queryResponse.Took;
      long elapsedMilliseconds = stopwatch.ElapsedMilliseconds;
      int totalHighlights = entitySearchPlatformResponse.GetTotalHighlights();
      if (elapsedMilliseconds >= 10000L && elapsedMilliseconds - took >= 5000L)
        ElasticSearchQueryClient.LogNestAuditTrail<T>(requestContext, queryResponse);
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishKpiAndCi("ESReportedQueryTime", "Query Pipeline", (double) took, true);
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishKpiAndCi("E2EPlatformQueryTime", "Query Pipeline", (double) elapsedMilliseconds, true);
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishKpiAndCi("NoOfTotalFileHits", "Query Pipeline", (double) queryResponse.Total);
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishKpiAndCi("NoOfFileHitsReturned", "Query Pipeline", (double) queryResponse.Hits.Count<IHit<T>>());
      if (totalHighlights < 0)
        return;
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishKpiAndCi("NoOfTotalHighlights", "Query Pipeline", (double) totalHighlights);
    }

    public long GetNumberOfHitsCount(
      IExpression filter,
      DocumentContractType fileContractType,
      IEnumerable<IndexInfo> indexInfo = null)
    {
      if (filter == null || filter is EmptyExpression)
        throw new ArgumentException("Filter is either null or EmptyExpression", nameof (filter));
      long numberOfHitsCount = -1;
      filter = (IExpression) new AndExpression(new IExpression[2]
      {
        filter,
        ElasticSearchQueryClient.GetContractTypeFilter(indexInfo, fileContractType)
      });
      CountDescriptor<object> countDescriptor = new CountDescriptor<object>().Query((Func<QueryContainerDescriptor<object>, QueryContainer>) (q => filter.ToTermLevelQuery()));
      CountResponse response = this.ElasticClient.Count(indexInfo == null ? (ICountRequest) countDescriptor.Index(Indices.All) : (ICountRequest) countDescriptor.Index((Indices) Indices.Index(ElasticSearchBasePlatform.GetIndices(indexInfo))));
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceVerboseConditionally(1082297, "Search Engine", "Search Engine", new Func<string>(((IResponseExtensions) response).SerializeRequestAndResponse));
      if (response != null && response.IsValid)
        numberOfHitsCount = response.Count;
      else
        response.ThrowOnInvalidOrFailedResponse();
      return numberOfHitsCount;
    }

    public IEnumerable<Dictionary<string, string>> GetDocMetadata(
      IVssRequestContext requestContext,
      IEnumerable<IndexInfo> indexInfo,
      List<string> fields,
      IExpression filter,
      DocumentContractType fileContractType,
      int numberOfHitsInOneGo)
    {
      if (filter == null || filter is EmptyExpression)
        throw new ArgumentException("Filter is either null or EmptyExpression.", nameof (filter));
      numberOfHitsInOneGo = numberOfHitsInOneGo <= 0 ? 1 : numberOfHitsInOneGo;
      string scrollTimeAsString = FormattableString.Invariant(FormattableStringFactory.Create("{0}s", (object) (int) TimeSpan.FromSeconds((double) (500 + numberOfHitsInOneGo / 1000 * 30)).RoundUpTimeSpanToHighestUnit().TotalSeconds));
      SearchDescriptor<string> searchDescriptor = new SearchDescriptor<string>().Query((Func<QueryContainerDescriptor<string>, QueryContainer>) (q => filter.ToTermLevelQuery())).Scroll((Time) scrollTimeAsString).Size(new int?(numberOfHitsInOneGo)).StoredFields((Fields) fields.ToArray());
      searchDescriptor = indexInfo == null ? searchDescriptor.Index(Indices.All) : searchDescriptor.Index((Indices) Indices.Index(ElasticSearchBasePlatform.GetIndices(indexInfo)));
      string[] routing = ElasticSearchPlatform.GetRouting(indexInfo);
      if (routing != null && ((IEnumerable<string>) routing).All<string>((Func<string, bool>) (r => !string.IsNullOrWhiteSpace(r))))
        searchDescriptor = searchDescriptor.Routing((Routing) routing);
      ISearchResponse<string> result = this.ElasticClient.Search<string>((Func<SearchDescriptor<string>, ISearchRequest>) (s => (ISearchRequest) searchDescriptor));
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceVerboseConditionally(1082296, "Search Engine", "Search Engine", new Func<string>(((IResponseExtensions) result).SerializeRequestAndResponse));
      while (result.Hits.Any<IHit<string>>())
      {
        foreach (IHit<string> hit in (IEnumerable<IHit<string>>) result.Hits)
        {
          Dictionary<string, string> dictionary = new Dictionary<string, string>();
          foreach (string field in fields)
          {
            object obj = hit.Fields.Value<object>((Field) field);
            if (obj != null)
              dictionary.Add(field, (string) obj);
          }
          yield return dictionary;
        }
        result = this.ElasticClient.Scroll<string>((Time) scrollTimeAsString, result.ScrollId);
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceVerboseConditionally(1082296, "Search Engine", "Search Engine", new Func<string>(((IResponseExtensions) result).SerializeRequestAndResponse));
      }
    }

    public List<Dictionary<string, List<string>>> GetDocMetadata(
      IEnumerable<IndexInfo> indexInfo,
      List<string> fields,
      IExpression filter,
      DocumentContractType fileContractType,
      int numberOfHitsInOneGo,
      out string nextScrollId,
      string scrollId)
    {
      if (filter == null || filter is EmptyExpression)
        throw new ArgumentException("Filter is either null or EmptyExpression.", nameof (filter));
      numberOfHitsInOneGo = numberOfHitsInOneGo <= 0 ? 1 : numberOfHitsInOneGo;
      string scroll = FormattableString.Invariant(FormattableStringFactory.Create("{0}s", (object) (int) TimeSpan.FromSeconds((double) (500 + numberOfHitsInOneGo / 1000 * 30)).RoundUpTimeSpanToHighestUnit().TotalSeconds));
      List<Dictionary<string, List<string>>> docMetadata = new List<Dictionary<string, List<string>>>();
      ISearchResponse<string> response = (ISearchResponse<string>) null;
      nextScrollId = "";
      try
      {
        if (scrollId == null)
        {
          SearchDescriptor<string> searchDescriptor = new SearchDescriptor<string>().Index((Indices) Indices.Index(ElasticSearchBasePlatform.GetIndices(indexInfo))).Query((Func<QueryContainerDescriptor<string>, QueryContainer>) (q => filter.ToTermLevelQuery())).Scroll((Time) scroll).Size(new int?(numberOfHitsInOneGo));
          searchDescriptor = !fileContractType.IsSourceOnAndApplicable() ? searchDescriptor.StoredFields((Fields) fields.ToArray()) : searchDescriptor.Source((Func<SourceFilterDescriptor<string>, ISourceFilter>) (s => (ISourceFilter) s.Includes((Func<FieldsDescriptor<string>, IPromise<Fields>>) (i => (IPromise<Fields>) i.Fields(fields.ToArray())))));
          string[] routing = ElasticSearchPlatform.GetRouting(indexInfo);
          if (routing != null && ((IEnumerable<string>) routing).All<string>((Func<string, bool>) (r => !string.IsNullOrWhiteSpace(r))))
            searchDescriptor = searchDescriptor.Routing((Routing) routing);
          response = this.ElasticClient.Search<string>((Func<SearchDescriptor<string>, ISearchRequest>) (s => (ISearchRequest) searchDescriptor));
        }
        else
          response = this.ElasticClient.Scroll<string>((Time) scroll, scrollId);
        response.ThrowOnInvalidOrFailedResponse();
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceVerboseConditionally(1082296, "Search Engine", "Search Engine", new Func<string>(((IResponseExtensions) response).SerializeRequestAndResponse));
        if (response.Hits == null)
          throw new SearchPlatformException("ElasticSearch Response Hits is null for GetDocMetaData.");
      }
      catch (Exception ex)
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceException(1082287, "Search Engine", "Search Engine", ex);
        this.ElasticClient.WrapAndThrowException(ex);
      }
      foreach (IHit<string> hit in (IEnumerable<IHit<string>>) response.Hits)
      {
        Dictionary<string, List<string>> dictionary = new Dictionary<string, List<string>>();
        foreach (string field in fields)
        {
          string[] source = hit.Fields.ValuesOf<string>((Field) field);
          if (source != null)
            dictionary.Add(field, ((IEnumerable<string>) source).ToList<string>());
        }
        docMetadata.Add(dictionary);
      }
      if (response.Hits.Any<IHit<string>>())
        nextScrollId = response.ScrollId;
      return docMetadata;
    }

    internal static IExpression GetContractTypeFilter(
      IEnumerable<IndexInfo> indices,
      DocumentContractType contractType)
    {
      if (contractType == DocumentContractType.Unsupported)
        throw new NotSupportedException(FormattableString.Invariant(FormattableStringFactory.Create("Contract type [{0}] is not supported for creating contract type filter.", (object) contractType)));
      if (contractType.CoExistsWithOtherContractTypes())
      {
        int num = -1;
        if (indices != null && indices.Any<IndexInfo>())
          num = indices.Select<IndexInfo, int>((Func<IndexInfo, int>) (idx => idx.Version ?? -1)).Min();
        if (num >= 1421)
          return (IExpression) new TermExpression(nameof (contractType), Microsoft.VisualStudio.Services.Search.Common.Arriba.Operator.Equals, contractType.ToString());
      }
      return (IExpression) new EmptyExpression();
    }

    private static void LogNestAuditTrail<T>(
      IVssRequestContext requestContext,
      ISearchResponse<T> queryResponse)
      where T : class
    {
      if (queryResponse.ApiCall?.AuditTrail == null)
        return;
      StringBuilder stringBuilder = new StringBuilder(FormattableString.Invariant(FormattableStringFactory.Create("IsValid: [{0}], ", (object) queryResponse.IsValid)));
      foreach (Audit audit in queryResponse.ApiCall.AuditTrail)
        stringBuilder.AppendLine(FormattableString.Invariant(FormattableStringFactory.Create("Started: [{0:o}], Took: [{1}], Node: [{2}], Event: [{3}], NodeAlive: [{4}], Exception: [{5}]", (object) audit.Started, (object) (audit.Ended - audit.Started), (object) audit.Node?.Uri, (object) audit.Event, (object) audit.Node?.IsAlive, (object) audit.Exception?.Message)));
      requestContext.TraceAlways(1080025, TraceLevel.Warning, "Search Engine", "Search Engine", stringBuilder.ToString());
    }
  }
}
