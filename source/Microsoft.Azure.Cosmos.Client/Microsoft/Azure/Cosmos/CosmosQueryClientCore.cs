// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.CosmosQueryClientCore
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.Common;
using Microsoft.Azure.Cosmos.CosmosElements;
using Microsoft.Azure.Cosmos.Json;
using Microsoft.Azure.Cosmos.Query.Core;
using Microsoft.Azure.Cosmos.Query.Core.Metrics;
using Microsoft.Azure.Cosmos.Query.Core.Monads;
using Microsoft.Azure.Cosmos.Query.Core.Pipeline.Pagination;
using Microsoft.Azure.Cosmos.Query.Core.QueryClient;
using Microsoft.Azure.Cosmos.Routing;
using Microsoft.Azure.Cosmos.Tracing;
using Microsoft.Azure.Cosmos.Tracing.TraceData;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Routing;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Cosmos
{
  internal class CosmosQueryClientCore : CosmosQueryClient
  {
    private const string QueryExecutionInfoHeader = "x-ms-cosmos-query-execution-info";
    private readonly CosmosClientContext clientContext;
    private readonly ContainerInternal cosmosContainerCore;
    private readonly DocumentClient documentClient;
    private readonly SemaphoreSlim semaphore;

    public CosmosQueryClientCore(
      CosmosClientContext clientContext,
      ContainerInternal cosmosContainerCore)
    {
      this.clientContext = clientContext ?? throw new ArgumentException(nameof (clientContext));
      this.cosmosContainerCore = cosmosContainerCore;
      this.documentClient = this.clientContext.DocumentClient;
      this.semaphore = new SemaphoreSlim(1, 1);
    }

    public override Action<IQueryable> OnExecuteScalarQueryCallback => this.documentClient.OnExecuteScalarQueryCallback;

    public override async Task<ContainerQueryProperties> GetCachedContainerQueryPropertiesAsync(
      string containerLink,
      PartitionKey? partitionKey,
      ITrace trace,
      CancellationToken cancellationToken)
    {
      ContainerProperties containerPropertiesAsync = await this.clientContext.GetCachedContainerPropertiesAsync(containerLink, trace, cancellationToken);
      string partitionKeyString;
      if (partitionKey.HasValue)
        partitionKeyString = (partitionKey.Value.IsNone ? containerPropertiesAsync.GetNoneValue() : partitionKey.Value.InternalKey).GetEffectivePartitionKeyString(containerPropertiesAsync.PartitionKey);
      return new ContainerQueryProperties(containerPropertiesAsync.ResourceId, partitionKeyString, containerPropertiesAsync.PartitionKey);
    }

    public override async Task<TryCatch<Microsoft.Azure.Cosmos.Query.Core.QueryPlan.PartitionedQueryExecutionInfo>> TryGetPartitionedQueryExecutionInfoAsync(
      SqlQuerySpec sqlQuerySpec,
      ResourceType resourceType,
      PartitionKeyDefinition partitionKeyDefinition,
      bool requireFormattableOrderByQuery,
      bool isContinuationExpected,
      bool allowNonValueAggregateQuery,
      bool hasLogicalPartitionKey,
      bool allowDCount,
      bool useSystemPrefix,
      CancellationToken cancellationToken)
    {
      string queryString = (string) null;
      if (sqlQuerySpec != null)
      {
        using (Stream streamSqlQuerySpec = this.clientContext.SerializerCore.ToStreamSqlQuerySpec(sqlQuerySpec, resourceType))
        {
          using (StreamReader streamReader = new StreamReader(streamSqlQuerySpec))
            queryString = streamReader.ReadToEnd();
        }
      }
      TryCatch<Microsoft.Azure.Cosmos.Query.Core.QueryPlan.PartitionedQueryExecutionInfo> queryExecutionInfo = (await this.documentClient.QueryPartitionProvider).TryGetPartitionedQueryExecutionInfo(queryString, partitionKeyDefinition, requireFormattableOrderByQuery, isContinuationExpected, allowNonValueAggregateQuery, hasLogicalPartitionKey, allowDCount, useSystemPrefix);
      queryString = (string) null;
      return queryExecutionInfo;
    }

    public override async Task<TryCatch<QueryPage>> ExecuteItemQueryAsync(
      string resourceUri,
      ResourceType resourceType,
      Microsoft.Azure.Documents.OperationType operationType,
      Guid clientQueryCorrelationId,
      FeedRange feedRange,
      QueryRequestOptions requestOptions,
      SqlQuerySpec sqlQuerySpec,
      string continuationToken,
      bool isContinuationExpected,
      int pageSize,
      ITrace trace,
      CancellationToken cancellationToken)
    {
      requestOptions.MaxItemCount = new int?(pageSize);
      CosmosClientContext clientContext = this.clientContext;
      string resourceUri1 = resourceUri;
      int num1 = (int) resourceType;
      int num2 = (int) operationType;
      QueryRequestOptions queryRequestOptions = requestOptions;
      FeedRange feedRange1 = feedRange;
      ContainerInternal cosmosContainerCore = this.cosmosContainerCore;
      FeedRange feedRange2 = feedRange1;
      Stream streamSqlQuerySpec = this.clientContext.SerializerCore.ToStreamSqlQuerySpec(sqlQuerySpec, resourceType);
      Action<RequestMessage> requestEnricher = (Action<RequestMessage>) (cosmosRequestMessage =>
      {
        cosmosRequestMessage.Headers.Add("x-ms-documentdb-query-iscontinuationexpected", isContinuationExpected.ToString());
        QueryRequestOptions.FillContinuationToken(cosmosRequestMessage, continuationToken);
        cosmosRequestMessage.Headers.Add("Content-Type", "application/query+json");
        cosmosRequestMessage.Headers.Add("x-ms-documentdb-isquery", bool.TrueString);
        cosmosRequestMessage.Headers.Add("x-ms-cosmos-correlated-activityid", clientQueryCorrelationId.ToString());
      });
      ITrace trace1 = trace;
      CancellationToken cancellationToken1 = cancellationToken;
      return CosmosQueryClientCore.GetCosmosElementResponse(requestOptions, resourceType, await clientContext.ProcessResourceOperationStreamAsync(resourceUri1, (ResourceType) num1, (Microsoft.Azure.Documents.OperationType) num2, (RequestOptions) queryRequestOptions, cosmosContainerCore, feedRange2, streamSqlQuerySpec, requestEnricher, trace1, cancellationToken1), trace);
    }

    public override async Task<Microsoft.Azure.Cosmos.Query.Core.QueryPlan.PartitionedQueryExecutionInfo> ExecuteQueryPlanRequestAsync(
      string resourceUri,
      ResourceType resourceType,
      Microsoft.Azure.Documents.OperationType operationType,
      SqlQuerySpec sqlQuerySpec,
      PartitionKey? partitionKey,
      string supportedQueryFeatures,
      Guid clientQueryCorrelationId,
      ITrace trace,
      CancellationToken cancellationToken)
    {
      CosmosClientContext clientContext = this.clientContext;
      string resourceUri1 = resourceUri;
      int num1 = (int) resourceType;
      int num2 = (int) operationType;
      FeedRange feedRange1 = partitionKey.HasValue ? (FeedRange) new FeedRangePartitionKey(partitionKey.Value) : (FeedRange) null;
      ContainerInternal cosmosContainerCore = this.cosmosContainerCore;
      FeedRange feedRange2 = feedRange1;
      Stream streamSqlQuerySpec = this.clientContext.SerializerCore.ToStreamSqlQuerySpec(sqlQuerySpec, resourceType);
      Action<RequestMessage> requestEnricher = (Action<RequestMessage>) (requestMessage =>
      {
        requestMessage.Headers.Add("Content-Type", "application/query+json");
        requestMessage.Headers.Add("x-ms-cosmos-is-query-plan-request", bool.TrueString);
        requestMessage.Headers.Add("x-ms-cosmos-supported-query-features", supportedQueryFeatures);
        requestMessage.Headers.Add("x-ms-cosmos-query-version", new System.Version(1, 0).ToString());
        requestMessage.Headers.Add("x-ms-cosmos-correlated-activityid", clientQueryCorrelationId.ToString());
        requestMessage.UseGatewayMode = new bool?(true);
      });
      ITrace trace1 = trace;
      CancellationToken cancellationToken1 = cancellationToken;
      Microsoft.Azure.Cosmos.Query.Core.QueryPlan.PartitionedQueryExecutionInfo queryExecutionInfo;
      using (ResponseMessage responseMessage = await clientContext.ProcessResourceOperationStreamAsync(resourceUri1, (ResourceType) num1, (Microsoft.Azure.Documents.OperationType) num2, (RequestOptions) null, cosmosContainerCore, feedRange2, streamSqlQuerySpec, requestEnricher, trace1, cancellationToken1))
      {
        responseMessage.EnsureSuccessStatusCode();
        queryExecutionInfo = this.clientContext.SerializerCore.FromStream<Microsoft.Azure.Cosmos.Query.Core.QueryPlan.PartitionedQueryExecutionInfo>(responseMessage.Content);
      }
      return queryExecutionInfo;
    }

    public override Task<List<PartitionKeyRange>> GetTargetPartitionKeyRangesByEpkStringAsync(
      string resourceLink,
      string collectionResourceId,
      string effectivePartitionKeyString,
      bool forceRefresh,
      ITrace trace)
    {
      string resourceLink1 = resourceLink;
      string collectionResourceId1 = collectionResourceId;
      List<Range<string>> providedRanges = new List<Range<string>>();
      providedRanges.Add(Range<string>.GetPointRange(effectivePartitionKeyString));
      int num = forceRefresh ? 1 : 0;
      ITrace trace1 = trace;
      return this.GetTargetPartitionKeyRangesAsync(resourceLink1, collectionResourceId1, providedRanges, num != 0, trace1);
    }

    public override async Task<List<PartitionKeyRange>> GetTargetPartitionKeyRangeByFeedRangeAsync(
      string resourceLink,
      string collectionResourceId,
      PartitionKeyDefinition partitionKeyDefinition,
      FeedRangeInternal feedRangeInternal,
      bool forceRefresh,
      ITrace trace)
    {
      CosmosQueryClientCore cosmosQueryClientCore = this;
      List<PartitionKeyRange> partitionKeyRangesAsync;
      using (ITrace childTrace = trace.StartChild("Get Overlapping Feed Ranges", TraceComponent.Routing, TraceLevel.Info))
      {
        List<Range<string>> effectiveRangesAsync = await feedRangeInternal.GetEffectiveRangesAsync((IRoutingMapProvider) await cosmosQueryClientCore.GetRoutingMapProviderAsync(), collectionResourceId, partitionKeyDefinition, trace);
        partitionKeyRangesAsync = await cosmosQueryClientCore.GetTargetPartitionKeyRangesAsync(resourceLink, collectionResourceId, effectiveRangesAsync, forceRefresh, childTrace);
      }
      return partitionKeyRangesAsync;
    }

    public override async Task<List<PartitionKeyRange>> GetTargetPartitionKeyRangesAsync(
      string resourceLink,
      string collectionResourceId,
      List<Range<string>> providedRanges,
      bool forceRefresh,
      ITrace trace)
    {
      if (string.IsNullOrEmpty(collectionResourceId))
        throw new ArgumentNullException(nameof (collectionResourceId));
      if (providedRanges == null || !providedRanges.Any<Range<string>>() || providedRanges.Any<Range<string>>((Func<Range<string>, bool>) (x => x == null)))
        throw new ArgumentNullException(nameof (providedRanges));
      List<PartitionKeyRange> partitionKeyRangesAsync;
      using (ITrace getPKRangesTrace = trace.StartChild("Get Partition Key Ranges", TraceComponent.Routing, TraceLevel.Info))
      {
        List<PartitionKeyRange> ranges = await (await this.GetRoutingMapProviderAsync()).TryGetOverlappingRangesAsync(collectionResourceId, (IEnumerable<Range<string>>) providedRanges, getPKRangesTrace);
        if (ranges == null && PathsHelper.IsNameBased(resourceLink))
          (await this.documentClient.GetCollectionCacheAsync(getPKRangesTrace)).Refresh(resourceLink);
        partitionKeyRangesAsync = ranges != null ? ranges : throw new NotFoundException(DateTime.UtcNow.ToString("o", (IFormatProvider) CultureInfo.InvariantCulture) + ": GetTargetPartitionKeyRanges(collectionResourceId:" + collectionResourceId + ", providedRanges: " + string.Join<Range<string>>(",", (IEnumerable<Range<string>>) providedRanges) + " failed due to stale cache");
      }
      return partitionKeyRangesAsync;
    }

    public override bool ByPassQueryParsing() => CustomTypeExtensions.ByPassQueryParsing();

    public override void ClearSessionTokenCache(string collectionFullName) => this.clientContext.DocumentClient.sessionContainer.ClearTokenByCollectionFullname(collectionFullName);

    private static TryCatch<QueryPage> GetCosmosElementResponse(
      QueryRequestOptions requestOptions,
      ResourceType resourceType,
      ResponseMessage cosmosResponseMessage,
      ITrace trace)
    {
      using (trace.StartChild("Get Cosmos Element Response", TraceComponent.Json, TraceLevel.Info))
      {
        using (cosmosResponseMessage)
        {
          if (cosmosResponseMessage.Headers.QueryMetricsText != null)
          {
            QueryMetricsTraceDatum metricsTraceDatum = new QueryMetricsTraceDatum(new Lazy<QueryMetrics>((Func<QueryMetrics>) (() => new QueryMetrics(cosmosResponseMessage.Headers.QueryMetricsText, IndexUtilizationInfo.Empty, ClientSideMetrics.Empty))));
            trace.AddDatum("Query Metrics", (TraceDatum) metricsTraceDatum);
          }
          if (!cosmosResponseMessage.IsSuccessStatusCode)
            return TryCatch<QueryPage>.FromException((Exception) (cosmosResponseMessage.CosmosException ?? new CosmosException(cosmosResponseMessage.ErrorMessage, cosmosResponseMessage.StatusCode, (int) cosmosResponseMessage.Headers.SubStatusCode, cosmosResponseMessage.Headers.ActivityId, cosmosResponseMessage.Headers.RequestCharge)));
          if (!(cosmosResponseMessage.Content is MemoryStream destination))
          {
            destination = new MemoryStream();
            cosmosResponseMessage.Content.CopyTo((Stream) destination);
          }
          long length = destination.Length;
          CosmosArray elementsFromRestStream = CosmosQueryClientCore.ParseElementsFromRestStream((Stream) destination, resourceType, requestOptions.CosmosSerializationFormatOptions);
          QueryState state = cosmosResponseMessage.Headers.ContinuationToken == null ? (QueryState) null : new QueryState((CosmosElement) CosmosString.Create(cosmosResponseMessage.Headers.ContinuationToken));
          Dictionary<string, string> additionalHeaders = new Dictionary<string, string>();
          foreach (string header in cosmosResponseMessage.Headers)
          {
            if (!QueryPage.BannedHeaders.Contains(header))
              additionalHeaders[header] = cosmosResponseMessage.Headers[header];
          }
          Lazy<CosmosQueryExecutionInfo> cosmosQueryExecutionInfo = (Lazy<CosmosQueryExecutionInfo>) null;
          string queryExecutionInfoString;
          if (cosmosResponseMessage.Headers.TryGetValue("x-ms-cosmos-query-execution-info", out queryExecutionInfoString))
            cosmosQueryExecutionInfo = new Lazy<CosmosQueryExecutionInfo>((Func<CosmosQueryExecutionInfo>) (() => JsonConvert.DeserializeObject<CosmosQueryExecutionInfo>(queryExecutionInfoString)));
          return TryCatch<QueryPage>.FromResult(new QueryPage((IReadOnlyList<CosmosElement>) elementsFromRestStream, cosmosResponseMessage.Headers.RequestCharge, cosmosResponseMessage.Headers.ActivityId, length, cosmosQueryExecutionInfo, (string) null, (IReadOnlyDictionary<string, string>) additionalHeaders, state));
        }
      }
    }

    private void PopulatePartitionKeyRangeInfo(
      RequestMessage request,
      PartitionKeyRangeIdentity partitionKeyRangeIdentity)
    {
      if (request == null)
        throw new ArgumentNullException(nameof (request));
      if (!request.ResourceType.IsPartitioned() || request.Headers.PartitionKey != null)
        return;
      request.ToDocumentServiceRequest().RouteTo(partitionKeyRangeIdentity);
    }

    public override async Task ForceRefreshCollectionCacheAsync(
      string collectionLink,
      CancellationToken cancellationToken)
    {
      CosmosQueryClientCore cosmosQueryClientCore = this;
      cosmosQueryClientCore.ClearSessionTokenCache(collectionLink);
      CollectionCache collectionCacheAsync = (CollectionCache) await cosmosQueryClientCore.documentClient.GetCollectionCacheAsync((ITrace) NoOpTrace.Singleton);
      using (DocumentServiceRequest request = DocumentServiceRequest.Create(Microsoft.Azure.Documents.OperationType.Query, ResourceType.Collection, collectionLink, AuthorizationTokenType.Invalid))
      {
        request.ForceNameCacheRefresh = true;
        ContainerProperties containerProperties = await collectionCacheAsync.ResolveCollectionAsync(request, cancellationToken, (ITrace) NoOpTrace.Singleton);
      }
    }

    public override async Task<IReadOnlyList<PartitionKeyRange>> TryGetOverlappingRangesAsync(
      string collectionResourceId,
      Range<string> range,
      bool forceRefresh = false)
    {
      return await (await this.GetRoutingMapProviderAsync()).TryGetOverlappingRangesAsync(collectionResourceId, range, (ITrace) NoOpTrace.Singleton, forceRefresh);
    }

    private Task<PartitionKeyRangeCache> GetRoutingMapProviderAsync() => this.documentClient.GetPartitionKeyRangeCacheAsync((ITrace) NoOpTrace.Singleton);

    public static CosmosArray ParseElementsFromRestStream(
      Stream stream,
      ResourceType resourceType,
      CosmosSerializationFormatOptions cosmosSerializationOptions)
    {
      if (!(stream is MemoryStream destination))
      {
        destination = new MemoryStream();
        stream.CopyTo((Stream) destination);
      }
      if (!destination.CanRead)
        throw new InvalidDataException("Stream can not be read");
      ArraySegment<byte> buffer;
      ReadOnlyMemory<byte> readOnlyMemory = destination.TryGetBuffer(out buffer) ? (ReadOnlyMemory<byte>) buffer : (ReadOnlyMemory<byte>) destination.ToArray();
      IJsonNavigator jsonNavigator;
      if (cosmosSerializationOptions != null)
      {
        jsonNavigator = cosmosSerializationOptions.CreateCustomNavigatorCallback(readOnlyMemory);
        if (jsonNavigator == null)
          throw new InvalidOperationException("The CosmosSerializationOptions did not return a JSON navigator.");
      }
      else
        jsonNavigator = JsonNavigator.Create(readOnlyMemory);
      string propertyName = resourceType != ResourceType.Collection ? resourceType.ToResourceTypeString() + "s" : "DocumentCollections";
      ObjectProperty objectProperty;
      if (!jsonNavigator.TryGetObjectProperty(jsonNavigator.GetRootNode(), propertyName, out objectProperty))
        throw new InvalidOperationException("Response Body Contract was violated. QueryResponse did not have property: " + propertyName);
      if (CosmosElement.Dispatch(jsonNavigator, objectProperty.ValueNode) is CosmosArray elementsFromRestStream)
        return elementsFromRestStream;
      throw new InvalidOperationException("QueryResponse did not have an array of : " + propertyName);
    }
  }
}
