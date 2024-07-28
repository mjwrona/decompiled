// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.Query.DefaultDocumentQueryExecutionContext
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Collections;
using Microsoft.Azure.Documents.Common;
using Microsoft.Azure.Documents.Routing;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Documents.Query
{
  internal sealed class DefaultDocumentQueryExecutionContext : DocumentQueryExecutionContextBase
  {
    private readonly bool isContinuationExpected;
    private readonly SchedulingStopwatch fetchSchedulingMetrics;
    private readonly FetchExecutionRangeAccumulator fetchExecutionRangeAccumulator;
    private readonly IDictionary<string, IReadOnlyList<Range<string>>> providedRangesCache;
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

    protected override async Task<FeedResponse<object>> ExecuteInternalAsync(CancellationToken token)
    {
      DefaultDocumentQueryExecutionContext executionContext = this;
      CollectionCache collectionCache = await executionContext.Client.GetCollectionCacheAsync();
      PartitionKeyRangeCache partitionKeyRangeCache = await executionContext.Client.GetPartitionKeyRangeCache();
      IDocumentClientRetryPolicy retryPolicyInstance = executionContext.Client.ResetSessionTokenRetryPolicy.GetRequestPolicy();
      retryPolicyInstance = (IDocumentClientRetryPolicy) new InvalidPartitionExceptionRetryPolicy(collectionCache, retryPolicyInstance);
      if (executionContext.ResourceTypeEnum.IsPartitioned())
        retryPolicyInstance = (IDocumentClientRetryPolicy) new PartitionKeyRangeGoneRetryPolicy(collectionCache, partitionKeyRangeCache, PathsHelper.GetCollectionPath(executionContext.ResourceLink), retryPolicyInstance);
      return await BackoffRetryUtility<FeedResponse<object>>.ExecuteAsync((Func<Task<FeedResponse<object>>>) (async () =>
      {
        this.fetchExecutionRangeAccumulator.BeginFetchRange();
        ++this.retries;
        Tuple<FeedResponse<object>, string> tuple = await this.ExecuteOnceAsync(retryPolicyInstance, token);
        FeedResponse<object> result = tuple.Item1;
        string str = tuple.Item2;
        if (!string.IsNullOrEmpty(result.ResponseHeaders["x-ms-documentdb-query-metrics"]))
        {
          this.fetchExecutionRangeAccumulator.EndFetchRange(str, result.ActivityId, (long) result.Count, this.retries);
          string responseHeader1 = result.ResponseHeaders["x-ms-documentdb-query-metrics"];
          string responseHeader2 = result.ResponseHeaders["x-ms-cosmos-index-utilization"];
          long retries = this.retries;
          double requestCharge = result.RequestCharge;
          IEnumerable<FetchExecutionRange> executionRanges = this.fetchExecutionRangeAccumulator.GetExecutionRanges();
          List<Tuple<string, SchedulingTimeSpan>> partitionSchedulingTimeSpans;
          if (!string.IsNullOrEmpty(result.ResponseContinuation))
          {
            partitionSchedulingTimeSpans = new List<Tuple<string, SchedulingTimeSpan>>();
          }
          else
          {
            partitionSchedulingTimeSpans = new List<Tuple<string, SchedulingTimeSpan>>();
            partitionSchedulingTimeSpans.Add(new Tuple<string, SchedulingTimeSpan>(str, this.fetchSchedulingMetrics.Elapsed));
          }
          ClientSideMetrics clientSideMetrics1 = new ClientSideMetrics(retries, requestCharge, executionRanges, (IEnumerable<Tuple<string, SchedulingTimeSpan>>) partitionSchedulingTimeSpans);
          QueryMetrics clientSideMetrics2 = QueryMetrics.CreateFromDelimitedStringAndClientSideMetrics(responseHeader1, responseHeader2, clientSideMetrics1);
          PartitionedQueryMetrics queryMetrics = new PartitionedQueryMetrics((IReadOnlyDictionary<string, QueryMetrics>) new Dictionary<string, QueryMetrics>()
          {
            {
              str,
              clientSideMetrics2
            }
          });
          PartitionedClientSideRequestStatistics empty = PartitionedClientSideRequestStatistics.CreateEmpty();
          if (result.PartitionedClientSideRequestStatistics != null)
          {
            foreach (IEnumerable<IClientSideRequestStatistics> requestStatisticses in result.PartitionedClientSideRequestStatistics.Values)
            {
              foreach (IClientSideRequestStatistics clientSideRequestStatistics in requestStatisticses)
                empty.AddClientSideRequestStatisticsToPartition(str, clientSideRequestStatistics);
            }
          }
          result = new FeedResponse<object>((IEnumerable<object>) result, result.Count, result.Headers, result.UseETagAsContinuation, (IReadOnlyDictionary<string, QueryMetrics>) queryMetrics, empty, result.DisallowContinuationTokenMessage, result.ResponseLengthBytes);
        }
        this.retries = -1L;
        return result;
      }), (IRetryPolicy) retryPolicyInstance, token);
    }

    private async Task<Tuple<FeedResponse<object>, string>> ExecuteOnceAsync(
      IDocumentClientRetryPolicy retryPolicyInstance,
      CancellationToken cancellationToken)
    {
      DefaultDocumentQueryExecutionContext context = this;
      DocumentServiceRequest request;
      int num;
      if ((uint) (num - 1) > 13U)
        request = await context.CreateRequestAsync();
      Tuple<FeedResponse<object>, string> tuple;
      try
      {
        FeedResponse<object> feedResponse;
        string str;
        if (DefaultDocumentQueryExecutionContext.LogicalPartitionKeyProvided(request))
        {
          feedResponse = await context.ExecuteRequestAsync(request, retryPolicyInstance, cancellationToken);
          str = "PKId(" + request.Headers["x-ms-documentdb-partitionkey"] + ")";
        }
        else if (DefaultDocumentQueryExecutionContext.PhysicalPartitionKeyRangeIdProvided(context))
        {
          request.RouteTo(new PartitionKeyRangeIdentity((await (await context.Client.GetCollectionCacheAsync()).ResolveCollectionAsync(request, CancellationToken.None)).ResourceId, context.PartitionKeyRangeId));
          feedResponse = await context.ExecuteRequestAsync(request, retryPolicyInstance, cancellationToken);
          str = context.PartitionKeyRangeId;
        }
        else if (DefaultDocumentQueryExecutionContext.ServiceInteropAvailable())
        {
          CollectionCache collectionCache = await context.Client.GetCollectionCacheAsync();
          DocumentCollection collection = await collectionCache.ResolveCollectionAsync(request, CancellationToken.None);
          QueryPartitionProvider queryPartitionProvider = await context.Client.GetQueryPartitionProviderAsync(cancellationToken);
          IRoutingMapProvider routingMapProvider = await context.Client.GetRoutingMapProviderAsync();
          List<CompositeContinuationToken> suppliedTokens;
          Range<string> rangeFromContinuationToken = PartitionRoutingHelper.ExtractPartitionKeyRangeFromContinuationToken(request.Headers, out suppliedTokens);
          Tuple<PartitionRoutingHelper.ResolvedRangeInfo, IReadOnlyList<Range<string>>> queryRoutingInfo = await context.TryGetTargetPartitionKeyRangeAsync(request, collection, queryPartitionProvider, routingMapProvider, rangeFromContinuationToken, suppliedTokens);
          if (request.IsNameBased && queryRoutingInfo == null)
          {
            request.ForceNameCacheRefresh = true;
            collection = await collectionCache.ResolveCollectionAsync(request, CancellationToken.None);
            queryRoutingInfo = await context.TryGetTargetPartitionKeyRangeAsync(request, collection, queryPartitionProvider, routingMapProvider, rangeFromContinuationToken, suppliedTokens);
          }
          if (queryRoutingInfo == null)
            throw new NotFoundException(DateTime.UtcNow.ToString("o", (IFormatProvider) CultureInfo.InvariantCulture) + ": Was not able to get queryRoutingInfo even after resolve collection async with force name cache refresh to the following collectionRid: " + collection.ResourceId + " with the supplied tokens: " + JsonConvert.SerializeObject((object) suppliedTokens));
          request.RouteTo(new PartitionKeyRangeIdentity(collection.ResourceId, queryRoutingInfo.Item1.ResolvedRange.Id));
          FeedResponse<object> response = await context.ExecuteRequestAsync(request, retryPolicyInstance, cancellationToken);
          if (!await PartitionRoutingHelper.TryAddPartitionKeyRangeToContinuationTokenAsync(response.Headers, queryRoutingInfo.Item2, routingMapProvider, collection.ResourceId, queryRoutingInfo.Item1))
            throw new NotFoundException(DateTime.UtcNow.ToString("o", (IFormatProvider) CultureInfo.InvariantCulture) + ": Call to TryAddPartitionKeyRangeToContinuationTokenAsync failed to the following collectionRid: " + collection.ResourceId + " with the supplied tokens: " + JsonConvert.SerializeObject((object) suppliedTokens));
          feedResponse = response;
          str = queryRoutingInfo.Item1.ResolvedRange.Id;
          collectionCache = (CollectionCache) null;
          collection = (DocumentCollection) null;
          queryPartitionProvider = (QueryPartitionProvider) null;
          routingMapProvider = (IRoutingMapProvider) null;
          suppliedTokens = (List<CompositeContinuationToken>) null;
          rangeFromContinuationToken = (Range<string>) null;
          queryRoutingInfo = (Tuple<PartitionRoutingHelper.ResolvedRangeInfo, IReadOnlyList<Range<string>>>) null;
          response = (FeedResponse<object>) null;
        }
        else
        {
          request.UseGatewayMode = true;
          feedResponse = await context.ExecuteRequestAsync(request, retryPolicyInstance, cancellationToken);
          str = "Gateway";
        }
        tuple = new Tuple<FeedResponse<object>, string>(feedResponse, str);
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
      DocumentCollection collection,
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
          providedRanges = PartitionRoutingHelper.GetProvidedPartitionKeyRanges(executionContext.QuerySpec, result, false, executionContext.isContinuationExpected, false, collection.PartitionKey, queryPartitionProvider, clientApiVersion, out QueryInfo _);
        else
          providedRanges = (IReadOnlyList<Range<string>>) new List<Range<string>>()
          {
            new Range<string>(PartitionKeyInternal.MinimumInclusiveEffectivePartitionKey, PartitionKeyInternal.MaximumExclusiveEffectivePartitionKey, true, false)
          };
        executionContext.providedRangesCache[collection.ResourceId] = providedRanges;
      }
      PartitionRoutingHelper.ResolvedRangeInfo continuationTokenRange = await PartitionRoutingHelper.TryGetTargetRangeFromContinuationTokenRange(providedRanges, routingMapProvider, collection.ResourceId, rangeFromContinuationToken, suppliedTokens);
      return continuationTokenRange.ResolvedRange != null ? Tuple.Create<PartitionRoutingHelper.ResolvedRangeInfo, IReadOnlyList<Range<string>>>(continuationTokenRange, providedRanges) : (Tuple<PartitionRoutingHelper.ResolvedRangeInfo, IReadOnlyList<Range<string>>>) null;
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
