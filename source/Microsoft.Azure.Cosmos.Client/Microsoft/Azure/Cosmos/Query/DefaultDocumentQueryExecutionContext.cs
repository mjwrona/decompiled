// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Query.DefaultDocumentQueryExecutionContext
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.Common;
using Microsoft.Azure.Cosmos.CosmosElements;
using Microsoft.Azure.Cosmos.Query.Core.Metrics;
using Microsoft.Azure.Cosmos.Query.Core.QueryPlan;
using Microsoft.Azure.Cosmos.Routing;
using Microsoft.Azure.Cosmos.Tracing;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Collections;
using Microsoft.Azure.Documents.Routing;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Cosmos.Query
{
  internal sealed class DefaultDocumentQueryExecutionContext : DocumentQueryExecutionContextBase
  {
    private const string InternalPartitionKeyDefinitionProperty = "x-ms-query-partitionkey-definition";
    private readonly bool isContinuationExpected;
    private readonly SchedulingStopwatch fetchSchedulingMetrics;
    private readonly FetchExecutionRangeAccumulator fetchExecutionRangeAccumulator;
    private readonly IDictionary<string, IReadOnlyList<Range<string>>> providedRangesCache;
    private readonly PartitionRoutingHelper partitionRoutingHelper;
    private long retries;

    public DefaultDocumentQueryExecutionContext(
      DocumentQueryExecutionContextBase.InitParams constructorParams,
      bool isContinuationExpected)
      : base(constructorParams)
    {
      this.isContinuationExpected = isContinuationExpected;
      this.fetchSchedulingMetrics = new SchedulingStopwatch();
      this.fetchSchedulingMetrics.Ready();
      this.fetchExecutionRangeAccumulator = new FetchExecutionRangeAccumulator();
      this.providedRangesCache = (IDictionary<string, IReadOnlyList<Range<string>>>) new Dictionary<string, IReadOnlyList<Range<string>>>();
      this.retries = -1L;
      this.partitionRoutingHelper = new PartitionRoutingHelper();
    }

    public static Task<DefaultDocumentQueryExecutionContext> CreateAsync(
      DocumentQueryExecutionContextBase.InitParams constructorParams,
      bool isContinuationExpected,
      CancellationToken token)
    {
      token.ThrowIfCancellationRequested();
      return Task.FromResult<DefaultDocumentQueryExecutionContext>(new DefaultDocumentQueryExecutionContext(constructorParams, isContinuationExpected));
    }

    public override void Dispose()
    {
    }

    protected override async Task<DocumentFeedResponse<CosmosElement>> ExecuteInternalAsync(
      CancellationToken token)
    {
      DefaultDocumentQueryExecutionContext executionContext = this;
      CollectionCache collectionCache = await executionContext.Client.GetCollectionCacheAsync();
      PartitionKeyRangeCache keyRangeCacheAsync = await executionContext.Client.GetPartitionKeyRangeCacheAsync();
      IDocumentClientRetryPolicy retryPolicyInstance = executionContext.Client.ResetSessionTokenRetryPolicy.GetRequestPolicy();
      retryPolicyInstance = (IDocumentClientRetryPolicy) new InvalidPartitionExceptionRetryPolicy(retryPolicyInstance);
      if (executionContext.ResourceTypeEnum.IsPartitioned())
        retryPolicyInstance = (IDocumentClientRetryPolicy) new PartitionKeyRangeGoneRetryPolicy(collectionCache, keyRangeCacheAsync, PathsHelper.GetCollectionPath(executionContext.ResourceLink), retryPolicyInstance);
      DocumentFeedResponse<CosmosElement> documentFeedResponse1 = await BackoffRetryUtility<DocumentFeedResponse<CosmosElement>>.ExecuteAsync((Func<Task<DocumentFeedResponse<CosmosElement>>>) (async () =>
      {
        this.fetchExecutionRangeAccumulator.BeginFetchRange();
        ++this.retries;
        Tuple<DocumentFeedResponse<CosmosElement>, string> tuple = await this.ExecuteOnceAsync(retryPolicyInstance, token);
        DocumentFeedResponse<CosmosElement> documentFeedResponse2 = tuple.Item1;
        string str = tuple.Item2;
        if (!string.IsNullOrEmpty(documentFeedResponse2.ResponseHeaders["x-ms-documentdb-query-metrics"]))
        {
          this.fetchExecutionRangeAccumulator.EndFetchRange(str, documentFeedResponse2.ActivityId, (long) documentFeedResponse2.Count, this.retries);
          DocumentFeedResponse<CosmosElement> result = documentFeedResponse2;
          int count = documentFeedResponse2.Count;
          INameValueCollection headers = documentFeedResponse2.Headers;
          int num = documentFeedResponse2.UseETagAsContinuation ? 1 : 0;
          Dictionary<string, QueryMetrics> queryMetrics = new Dictionary<string, QueryMetrics>();
          queryMetrics.Add(str, new QueryMetrics(documentFeedResponse2.ResponseHeaders["x-ms-documentdb-query-metrics"], IndexUtilizationInfo.CreateFromString(documentFeedResponse2.ResponseHeaders["x-ms-cosmos-index-utilization"], true), new ClientSideMetrics(this.retries, documentFeedResponse2.RequestCharge, this.fetchExecutionRangeAccumulator.GetExecutionRanges())));
          IClientSideRequestStatistics requestStatistics = documentFeedResponse2.RequestStatistics;
          string continuationTokenMessage = documentFeedResponse2.DisallowContinuationTokenMessage;
          long responseLengthBytes = documentFeedResponse2.ResponseLengthBytes;
          documentFeedResponse2 = new DocumentFeedResponse<CosmosElement>((IEnumerable<CosmosElement>) result, count, headers, num != 0, (IReadOnlyDictionary<string, QueryMetrics>) queryMetrics, requestStatistics, continuationTokenMessage, responseLengthBytes);
        }
        this.retries = -1L;
        return documentFeedResponse2;
      }), (IRetryPolicy) retryPolicyInstance, token);
      collectionCache = (CollectionCache) null;
      return documentFeedResponse1;
    }

    private async Task<Tuple<DocumentFeedResponse<CosmosElement>, string>> ExecuteOnceAsync(
      IDocumentClientRetryPolicy retryPolicyInstance,
      CancellationToken cancellationToken)
    {
      DefaultDocumentQueryExecutionContext context = this;
      DocumentServiceRequest request;
      int num;
      if ((uint) (num - 1) > 13U)
        request = await context.CreateRequestAsync();
      Tuple<DocumentFeedResponse<CosmosElement>, string> tuple;
      try
      {
        DocumentFeedResponse<CosmosElement> documentFeedResponse;
        string str;
        if (DefaultDocumentQueryExecutionContext.LogicalPartitionKeyProvided(request))
        {
          documentFeedResponse = await context.ExecuteRequestAsync(request, retryPolicyInstance, cancellationToken);
          str = "PKId(" + request.Headers["x-ms-documentdb-partitionkey"] + ")";
        }
        else if (DefaultDocumentQueryExecutionContext.PhysicalPartitionKeyRangeIdProvided(context))
        {
          request.RouteTo(new PartitionKeyRangeIdentity((await (await context.Client.GetCollectionCacheAsync()).ResolveCollectionAsync(request, CancellationToken.None, (ITrace) NoOpTrace.Singleton)).ResourceId, context.PartitionKeyRangeId));
          documentFeedResponse = await context.ExecuteRequestAsync(request, retryPolicyInstance, cancellationToken);
          str = context.PartitionKeyRangeId;
        }
        else if (DefaultDocumentQueryExecutionContext.ServiceInteropAvailable())
        {
          CollectionCache collectionCache = await context.Client.GetCollectionCacheAsync();
          ContainerProperties collection = await collectionCache.ResolveCollectionAsync(request, CancellationToken.None, (ITrace) NoOpTrace.Singleton);
          QueryPartitionProvider queryPartitionProvider = await context.Client.GetQueryPartitionProviderAsync();
          IRoutingMapProvider routingMapProvider = await context.Client.GetRoutingMapProviderAsync();
          List<CompositeContinuationToken> suppliedTokens;
          Range<string> rangeFromContinuationToken = context.partitionRoutingHelper.ExtractPartitionKeyRangeFromContinuationToken(request.Headers, out suppliedTokens);
          Tuple<PartitionRoutingHelper.ResolvedRangeInfo, IReadOnlyList<Range<string>>> queryRoutingInfo = await context.TryGetTargetPartitionKeyRangeAsync(request, collection, queryPartitionProvider, routingMapProvider, rangeFromContinuationToken, suppliedTokens);
          if (request.IsNameBased && queryRoutingInfo == null)
          {
            request.ForceNameCacheRefresh = true;
            collection = await collectionCache.ResolveCollectionAsync(request, CancellationToken.None, (ITrace) NoOpTrace.Singleton);
            queryRoutingInfo = await context.TryGetTargetPartitionKeyRangeAsync(request, collection, queryPartitionProvider, routingMapProvider, rangeFromContinuationToken, suppliedTokens);
          }
          if (queryRoutingInfo == null)
            throw new NotFoundException(DateTime.UtcNow.ToString("o", (IFormatProvider) CultureInfo.InvariantCulture) + ": Was not able to get queryRoutingInfo even after resolve collection async with force name cache refresh to the following collectionRid: " + collection.ResourceId + " with the supplied tokens: " + JsonConvert.SerializeObject((object) suppliedTokens));
          request.RouteTo(new PartitionKeyRangeIdentity(collection.ResourceId, queryRoutingInfo.Item1.ResolvedRange.Id));
          DocumentFeedResponse<CosmosElement> response = await context.ExecuteRequestAsync(request, retryPolicyInstance, cancellationToken);
          if (!await context.partitionRoutingHelper.TryAddPartitionKeyRangeToContinuationTokenAsync(response.Headers, queryRoutingInfo.Item2, routingMapProvider, collection.ResourceId, queryRoutingInfo.Item1, (ITrace) NoOpTrace.Singleton))
            throw new NotFoundException(DateTime.UtcNow.ToString("o", (IFormatProvider) CultureInfo.InvariantCulture) + ": Call to TryAddPartitionKeyRangeToContinuationTokenAsync failed to the following collectionRid: " + collection.ResourceId + " with the supplied tokens: " + JsonConvert.SerializeObject((object) suppliedTokens));
          documentFeedResponse = response;
          str = queryRoutingInfo.Item1.ResolvedRange.Id;
          collectionCache = (CollectionCache) null;
          collection = (ContainerProperties) null;
          queryPartitionProvider = (QueryPartitionProvider) null;
          routingMapProvider = (IRoutingMapProvider) null;
          suppliedTokens = (List<CompositeContinuationToken>) null;
          rangeFromContinuationToken = (Range<string>) null;
          queryRoutingInfo = (Tuple<PartitionRoutingHelper.ResolvedRangeInfo, IReadOnlyList<Range<string>>>) null;
          response = (DocumentFeedResponse<CosmosElement>) null;
        }
        else
        {
          request.UseGatewayMode = true;
          documentFeedResponse = await context.ExecuteRequestAsync(request, retryPolicyInstance, cancellationToken);
          str = "Gateway";
        }
        tuple = new Tuple<DocumentFeedResponse<CosmosElement>, string>(documentFeedResponse, str);
      }
      finally
      {
        request?.Dispose();
      }
      return tuple;
    }

    private static bool LogicalPartitionKeyProvided(DocumentServiceRequest request) => !string.IsNullOrEmpty(request.Headers["x-ms-documentdb-partitionkey"]) || !request.ResourceType.IsPartitioned();

    private static bool PhysicalPartitionKeyRangeIdProvided(
      DefaultDocumentQueryExecutionContext context)
    {
      return !string.IsNullOrEmpty(context.PartitionKeyRangeId);
    }

    private static bool ServiceInteropAvailable() => !CustomTypeExtensions.ByPassQueryParsing();

    private async Task<Tuple<PartitionRoutingHelper.ResolvedRangeInfo, IReadOnlyList<Range<string>>>> TryGetTargetPartitionKeyRangeAsync(
      DocumentServiceRequest request,
      ContainerProperties collection,
      QueryPartitionProvider queryPartitionProvider,
      IRoutingMapProvider routingMapProvider,
      Range<string> rangeFromContinuationToken,
      List<CompositeContinuationToken> suppliedTokens)
    {
      DefaultDocumentQueryExecutionContext executionContext = this;
      string header1 = request.Headers["x-ms-version"];
      string clientApiVersion = string.IsNullOrEmpty(header1) ? HttpConstants.Versions.CurrentVersion : header1;
      bool result = false;
      string header2 = request.Headers["x-ms-documentdb-query-enablecrosspartition"];
      if (header2 != null && !bool.TryParse(header2, out result))
        throw new BadRequestException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, RMResources.InvalidHeaderValue, (object) header2, (object) "x-ms-documentdb-query-enablecrosspartition"));
      IReadOnlyList<Range<string>> providedRanges;
      if (!executionContext.providedRangesCache.TryGetValue(collection.ResourceId, out providedRanges))
      {
        if (executionContext.ShouldExecuteQueryRequest)
        {
          FeedOptions feedOptions = executionContext.GetFeedOptions((string) null);
          object obj;
          PartitionKeyDefinition partitionKeyDefinition1;
          if (feedOptions.Properties != null && feedOptions.Properties.TryGetValue("x-ms-query-partitionkey-definition", out obj))
            partitionKeyDefinition1 = obj is PartitionKeyDefinition partitionKeyDefinition2 ? partitionKeyDefinition2 : throw new ArgumentException("partitionkeydefinition has invalid type", "partitionKeyDefinitionObject");
          else
            partitionKeyDefinition1 = collection.PartitionKey;
          providedRanges = PartitionRoutingHelper.GetProvidedPartitionKeyRanges(JsonConvert.SerializeObject((object) executionContext.QuerySpec), result, false, executionContext.isContinuationExpected, false, false, false, false, partitionKeyDefinition1, queryPartitionProvider, clientApiVersion, out QueryInfo _);
        }
        else
        {
          object obj;
          if (request.Properties != null && request.Properties.TryGetValue("x-ms-effective-partition-key-string", out obj))
            providedRanges = obj is string str ? (IReadOnlyList<Range<string>>) new List<Range<string>>()
            {
              Range<string>.GetPointRange(str)
            } : throw new ArgumentException("EffectivePartitionKey must be a string", "x-ms-effective-partition-key-string");
          else
            providedRanges = (IReadOnlyList<Range<string>>) new List<Range<string>>()
            {
              new Range<string>(PartitionKeyInternal.MinimumInclusiveEffectivePartitionKey, PartitionKeyInternal.MaximumExclusiveEffectivePartitionKey, true, false)
            };
        }
        executionContext.providedRangesCache[collection.ResourceId] = providedRanges;
      }
      PartitionRoutingHelper.ResolvedRangeInfo continuationTokenRangeAsync = await executionContext.partitionRoutingHelper.TryGetTargetRangeFromContinuationTokenRangeAsync(providedRanges, routingMapProvider, collection.ResourceId, rangeFromContinuationToken, suppliedTokens, (ITrace) NoOpTrace.Singleton);
      Tuple<PartitionRoutingHelper.ResolvedRangeInfo, IReadOnlyList<Range<string>>> partitionKeyRangeAsync = continuationTokenRangeAsync.ResolvedRange != null ? Tuple.Create<PartitionRoutingHelper.ResolvedRangeInfo, IReadOnlyList<Range<string>>>(continuationTokenRangeAsync, providedRanges) : (Tuple<PartitionRoutingHelper.ResolvedRangeInfo, IReadOnlyList<Range<string>>>) null;
      providedRanges = (IReadOnlyList<Range<string>>) null;
      return partitionKeyRangeAsync;
    }

    private async Task<DocumentServiceRequest> CreateRequestAsync()
    {
      DefaultDocumentQueryExecutionContext executionContext = this;
      INameValueCollection commonHeadersAsync = await executionContext.CreateCommonHeadersAsync(executionContext.GetFeedOptions(executionContext.ContinuationToken));
      commonHeadersAsync["x-ms-documentdb-query-iscontinuationexpected"] = executionContext.isContinuationExpected.ToString();
      return executionContext.CreateDocumentServiceRequest(commonHeadersAsync, executionContext.QuerySpec, executionContext.PartitionKeyInternal);
    }
  }
}
