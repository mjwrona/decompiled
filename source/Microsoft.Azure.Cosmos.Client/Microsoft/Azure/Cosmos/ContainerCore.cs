// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.ContainerCore
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.ChangeFeed;
using Microsoft.Azure.Cosmos.ChangeFeed.Configuration;
using Microsoft.Azure.Cosmos.ChangeFeed.FeedProcessing;
using Microsoft.Azure.Cosmos.ChangeFeed.LeaseManagement;
using Microsoft.Azure.Cosmos.ChangeFeed.Pagination;
using Microsoft.Azure.Cosmos.Common;
using Microsoft.Azure.Cosmos.Core.Utf8;
using Microsoft.Azure.Cosmos.CosmosElements;
using Microsoft.Azure.Cosmos.Diagnostics;
using Microsoft.Azure.Cosmos.Json;
using Microsoft.Azure.Cosmos.Linq;
using Microsoft.Azure.Cosmos.Pagination;
using Microsoft.Azure.Cosmos.Query;
using Microsoft.Azure.Cosmos.Query.Core;
using Microsoft.Azure.Cosmos.Query.Core.Monads;
using Microsoft.Azure.Cosmos.Query.Core.QueryClient;
using Microsoft.Azure.Cosmos.Query.Core.QueryPlan;
using Microsoft.Azure.Cosmos.ReadFeed;
using Microsoft.Azure.Cosmos.ReadFeed.Pagination;
using Microsoft.Azure.Cosmos.Resource.CosmosExceptions;
using Microsoft.Azure.Cosmos.Routing;
using Microsoft.Azure.Cosmos.Scripts;
using Microsoft.Azure.Cosmos.Tracing;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Routing;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Cosmos
{
  internal abstract class ContainerCore : ContainerInternal
  {
    private readonly Lazy<BatchAsyncContainerExecutor> lazyBatchExecutor;
    private static readonly Range<string> allRanges = new Range<string>(PartitionKeyInternal.MinimumInclusiveEffectivePartitionKey, PartitionKeyInternal.MaximumExclusiveEffectivePartitionKey, true, false);
    private readonly CosmosQueryClient queryClient;

    protected ContainerCore(
      CosmosClientContext clientContext,
      DatabaseInternal database,
      string containerId,
      CosmosQueryClient cosmosQueryClient = null)
    {
      this.Id = containerId;
      this.ClientContext = clientContext;
      this.LinkUri = clientContext.CreateLink(database.LinkUri, "colls", containerId);
      this.Database = (Database) database;
      this.Conflicts = (Conflicts) new ConflictsInlineCore(this.ClientContext, (ContainerInternal) this);
      this.Scripts = (Microsoft.Azure.Cosmos.Scripts.Scripts) new ScriptsInlineCore((ContainerInternal) this, this.ClientContext);
      this.cachedUriSegmentWithoutId = this.GetResourceSegmentUriWithoutId();
      this.queryClient = cosmosQueryClient ?? (CosmosQueryClient) new CosmosQueryClientCore(this.ClientContext, (ContainerInternal) this);
      this.lazyBatchExecutor = new Lazy<BatchAsyncContainerExecutor>((Func<BatchAsyncContainerExecutor>) (() => this.ClientContext.GetExecutorForContainer((ContainerInternal) this)));
    }

    public override string Id { get; }

    public override Database Database { get; }

    public override string LinkUri { get; }

    public override CosmosClientContext ClientContext { get; }

    public override BatchAsyncContainerExecutor BatchExecutor => this.lazyBatchExecutor.Value;

    public override Conflicts Conflicts { get; }

    public override Microsoft.Azure.Cosmos.Scripts.Scripts Scripts { get; }

    public async Task<ContainerResponse> ReadContainerAsync(
      ITrace trace,
      ContainerRequestOptions requestOptions = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      ContainerCore containerCore1 = this;
      ContainerCore containerCore2 = containerCore1;
      RequestOptions requestOptions1 = (RequestOptions) requestOptions;
      ITrace trace1 = trace;
      RequestOptions requestOptions2 = requestOptions1;
      CancellationToken cancellationToken1 = cancellationToken;
      ResponseMessage responseMessage = await containerCore2.ReadContainerStreamAsync(trace1, requestOptions2, cancellationToken1);
      return containerCore1.ClientContext.ResponseFactory.CreateContainerResponse((Container) containerCore1, responseMessage);
    }

    public async Task<ContainerResponse> ReplaceContainerAsync(
      ContainerProperties containerProperties,
      ITrace trace,
      ContainerRequestOptions requestOptions = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      ContainerCore containerCore1 = this;
      if (containerProperties == null)
        throw new ArgumentNullException(nameof (containerProperties));
      containerCore1.ClientContext.ValidateResource(containerProperties.Id);
      ContainerCore containerCore2 = containerCore1;
      Stream stream = containerCore1.ClientContext.SerializerCore.ToStream<ContainerProperties>(containerProperties);
      ContainerRequestOptions containerRequestOptions = requestOptions;
      ITrace trace1 = trace;
      ContainerRequestOptions requestOptions1 = containerRequestOptions;
      CancellationToken cancellationToken1 = cancellationToken;
      ResponseMessage responseMessage = await containerCore2.ReplaceStreamInternalAsync(stream, trace1, requestOptions1, cancellationToken1);
      return containerCore1.ClientContext.ResponseFactory.CreateContainerResponse((Container) containerCore1, responseMessage);
    }

    public async Task<ContainerResponse> DeleteContainerAsync(
      ITrace trace,
      ContainerRequestOptions requestOptions = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      ContainerCore containerCore1 = this;
      ContainerCore containerCore2 = containerCore1;
      ContainerRequestOptions containerRequestOptions = requestOptions;
      ITrace trace1 = trace;
      ContainerRequestOptions requestOptions1 = containerRequestOptions;
      CancellationToken cancellationToken1 = cancellationToken;
      ResponseMessage responseMessage = await containerCore2.DeleteContainerStreamAsync(trace1, requestOptions1, cancellationToken1);
      return containerCore1.ClientContext.ResponseFactory.CreateContainerResponse((Container) containerCore1, responseMessage);
    }

    public async Task<int?> ReadThroughputAsync(ITrace trace, CancellationToken cancellationToken = default (CancellationToken)) => (int?) (await this.ReadThroughputIfExistsAsync((RequestOptions) null, cancellationToken)).Resource?.Throughput;

    public async Task<ThroughputResponse> ReadThroughputAsync(
      RequestOptions requestOptions,
      ITrace trace,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      ContainerCore containerCore = this;
      ThroughputResponse throughputResponse = await containerCore.ReadThroughputIfExistsAsync(requestOptions, trace, cancellationToken);
      return throughputResponse.StatusCode != HttpStatusCode.NotFound ? throughputResponse : throw CosmosExceptionFactory.CreateNotFoundException("Throughput is not configured for " + containerCore.Id, throughputResponse.Headers, trace: trace);
    }

    public Task<ThroughputResponse> ReadThroughputIfExistsAsync(
      RequestOptions requestOptions,
      ITrace trace,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      CosmosOffers cosmosOffers = new CosmosOffers(this.ClientContext);
      return this.OfferRetryHelperForStaleRidCacheAsync((Func<string, Task<ThroughputResponse>>) (rid => cosmosOffers.ReadThroughputIfExistsAsync(rid, requestOptions, cancellationToken)), trace, cancellationToken);
    }

    public Task<ThroughputResponse> ReplaceThroughputAsync(
      int throughput,
      ITrace trace,
      RequestOptions requestOptions = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      ThroughputProperties manualThroughput = ThroughputProperties.CreateManualThroughput(throughput);
      RequestOptions requestOptions1 = requestOptions;
      ITrace trace1 = trace;
      RequestOptions requestOptions2 = requestOptions1;
      CancellationToken cancellationToken1 = cancellationToken;
      return this.ReplaceThroughputAsync(manualThroughput, trace1, requestOptions2, cancellationToken1);
    }

    public Task<ThroughputResponse> ReplaceThroughputIfExistsAsync(
      ThroughputProperties throughput,
      ITrace trace,
      RequestOptions requestOptions = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      CosmosOffers cosmosOffers = new CosmosOffers(this.ClientContext);
      return this.OfferRetryHelperForStaleRidCacheAsync((Func<string, Task<ThroughputResponse>>) (rid => cosmosOffers.ReplaceThroughputPropertiesIfExistsAsync(rid, throughput, requestOptions, cancellationToken)), trace, cancellationToken);
    }

    public async Task<ThroughputResponse> ReplaceThroughputAsync(
      ThroughputProperties throughputProperties,
      ITrace trace,
      RequestOptions requestOptions = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      ContainerCore containerCore = this;
      ThroughputResponse throughputResponse = await containerCore.ReplaceThroughputIfExistsAsync(throughputProperties, trace, requestOptions, cancellationToken);
      return throughputResponse.StatusCode != HttpStatusCode.NotFound ? throughputResponse : throw CosmosExceptionFactory.CreateNotFoundException("Throughput is not configured for " + containerCore.Id, throughputResponse.Headers);
    }

    public Task<ResponseMessage> DeleteContainerStreamAsync(
      ITrace trace,
      ContainerRequestOptions requestOptions = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.ProcessStreamAsync((Stream) null, Microsoft.Azure.Documents.OperationType.Delete, (RequestOptions) requestOptions, trace, cancellationToken);
    }

    public Task<ResponseMessage> ReadContainerStreamAsync(
      ITrace trace,
      RequestOptions requestOptions = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.ProcessStreamAsync((Stream) null, Microsoft.Azure.Documents.OperationType.Read, requestOptions, trace, cancellationToken);
    }

    public Task<ResponseMessage> ReplaceContainerStreamAsync(
      ContainerProperties containerProperties,
      ITrace trace,
      ContainerRequestOptions requestOptions = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      if (containerProperties == null)
        throw new ArgumentNullException(nameof (containerProperties));
      this.ClientContext.ValidateResource(containerProperties.Id);
      Stream stream = this.ClientContext.SerializerCore.ToStream<ContainerProperties>(containerProperties);
      ContainerRequestOptions containerRequestOptions = requestOptions;
      ITrace trace1 = trace;
      ContainerRequestOptions requestOptions1 = containerRequestOptions;
      CancellationToken cancellationToken1 = cancellationToken;
      return this.ReplaceStreamInternalAsync(stream, trace1, requestOptions1, cancellationToken1);
    }

    public async Task<IReadOnlyList<FeedRange>> GetFeedRangesAsync(
      ITrace trace,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      ContainerCore containerCore = this;
      PartitionKeyRangeCache partitionKeyRangeCache = await containerCore.ClientContext.DocumentClient.GetPartitionKeyRangeCacheAsync(trace);
      string containerRId = await containerCore.GetCachedRIDAsync(false, trace, cancellationToken);
      IReadOnlyList<PartitionKeyRange> overlappingRangesAsync = await partitionKeyRangeCache.TryGetOverlappingRangesAsync(containerRId, ContainerCore.allRanges, trace, true);
      if (overlappingRangesAsync == null)
      {
        string cachedRidAsync = await containerCore.GetCachedRIDAsync(true, trace, cancellationToken);
        if (string.Equals(containerRId, cachedRidAsync))
          throw CosmosExceptionFactory.CreateInternalServerErrorException("Container rid " + containerRId + " did not have a partition key range after refresh", new Headers(), trace: trace);
        overlappingRangesAsync = await partitionKeyRangeCache.TryGetOverlappingRangesAsync(cachedRidAsync, ContainerCore.allRanges, trace, true);
        if (overlappingRangesAsync == null)
          throw CosmosExceptionFactory.CreateInternalServerErrorException("Container rid " + containerRId + " returned partitionKeyRanges null after Container RID refresh", new Headers(), trace: trace);
      }
      List<FeedRange> feedRangeList = new List<FeedRange>(overlappingRangesAsync.Count);
      foreach (PartitionKeyRange partitionKeyRange in (IEnumerable<PartitionKeyRange>) overlappingRangesAsync)
        feedRangeList.Add((FeedRange) new FeedRangeEpk(partitionKeyRange.ToRange()));
      IReadOnlyList<FeedRange> feedRangesAsync = (IReadOnlyList<FeedRange>) feedRangeList;
      partitionKeyRangeCache = (PartitionKeyRangeCache) null;
      containerRId = (string) null;
      return feedRangesAsync;
    }

    public override FeedIterator GetChangeFeedStreamIterator(
      ChangeFeedStartFrom changeFeedStartFrom,
      ChangeFeedMode changeFeedMode,
      ChangeFeedRequestOptions changeFeedRequestOptions = null)
    {
      if (changeFeedStartFrom == null)
        throw new ArgumentNullException(nameof (changeFeedStartFrom));
      if (changeFeedMode == null)
        throw new ArgumentNullException(nameof (changeFeedMode));
      DocumentContainer documentContainer = new DocumentContainer((IMonadicDocumentContainer) new NetworkAttachedDocumentContainer((ContainerInternal) this, this.queryClient, Guid.NewGuid(), changeFeedRequestOptions: changeFeedRequestOptions));
      ChangeFeedStartFrom changeFeedStartFrom1 = changeFeedStartFrom;
      ChangeFeedMode changeFeedMode1 = changeFeedMode;
      ChangeFeedRequestOptions changeFeedRequestOptions1 = changeFeedRequestOptions;
      ChangeFeedStartFrom changeFeedStartFrom2 = changeFeedStartFrom1;
      CosmosClientContext clientContext = this.ClientContext;
      return (FeedIterator) new ChangeFeedIteratorCore((IDocumentContainer) documentContainer, changeFeedMode1, changeFeedRequestOptions1, changeFeedStartFrom2, clientContext, (ContainerInternal) this);
    }

    public override FeedIterator<T> GetChangeFeedIterator<T>(
      ChangeFeedStartFrom changeFeedStartFrom,
      ChangeFeedMode changeFeedMode,
      ChangeFeedRequestOptions changeFeedRequestOptions = null)
    {
      if (changeFeedStartFrom == null)
        throw new ArgumentNullException(nameof (changeFeedStartFrom));
      if (changeFeedMode == null)
        throw new ArgumentNullException(nameof (changeFeedMode));
      DocumentContainer documentContainer = new DocumentContainer((IMonadicDocumentContainer) new NetworkAttachedDocumentContainer((ContainerInternal) this, this.queryClient, Guid.NewGuid(), changeFeedRequestOptions: changeFeedRequestOptions));
      ChangeFeedStartFrom changeFeedStartFrom1 = changeFeedStartFrom;
      ChangeFeedMode changeFeedMode1 = changeFeedMode;
      ChangeFeedRequestOptions changeFeedRequestOptions1 = changeFeedRequestOptions;
      ChangeFeedStartFrom changeFeedStartFrom2 = changeFeedStartFrom1;
      ContainerInternal containerInternal = (ContainerInternal) this;
      CosmosClientContext clientContext = this.ClientContext;
      ContainerInternal container = containerInternal;
      return (FeedIterator<T>) new FeedIteratorCore<T>((FeedIteratorInternal) new ChangeFeedIteratorCore((IDocumentContainer) documentContainer, changeFeedMode1, changeFeedRequestOptions1, changeFeedStartFrom2, clientContext, container), new Func<ResponseMessage, FeedResponse<T>>(this.ClientContext.ResponseFactory.CreateChangeFeedUserTypeResponse<T>));
    }

    internal async Task<IEnumerable<string>> GetPartitionKeyRangesAsync(
      FeedRange feedRange,
      ITrace trace,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      ContainerCore containerCore = this;
      IRoutingMapProvider routingMapProvider = (IRoutingMapProvider) await containerCore.ClientContext.DocumentClient.GetPartitionKeyRangeCacheAsync(trace);
      string containerRid = await containerCore.GetCachedRIDAsync(false, trace, cancellationToken);
      PartitionKeyDefinition keyDefinitionAsync = await containerCore.GetPartitionKeyDefinitionAsync(cancellationToken);
      if (!(feedRange is FeedRangeInternal feedRangeInternal))
        throw new ArgumentException(nameof (feedRange), ClientResources.FeedToken_UnrecognizedFeedToken);
      IEnumerable<string> partitionKeyRangesAsync = await feedRangeInternal.GetPartitionKeyRangesAsync(routingMapProvider, containerRid, keyDefinitionAsync, cancellationToken, trace);
      routingMapProvider = (IRoutingMapProvider) null;
      containerRid = (string) null;
      return partitionKeyRangesAsync;
    }

    public override async Task<ContainerProperties> GetCachedContainerPropertiesAsync(
      bool forceRefresh,
      ITrace trace,
      CancellationToken cancellationToken)
    {
      ContainerCore containerCore = this;
      ContainerProperties containerPropertiesAsync;
      try
      {
        containerPropertiesAsync = await (await containerCore.ClientContext.DocumentClient.GetCollectionCacheAsync(trace)).ResolveByNameAsync(HttpConstants.Versions.CurrentVersion, containerCore.LinkUri, forceRefresh, trace, (IClientSideRequestStatistics) null, cancellationToken);
      }
      catch (DocumentClientException ex)
      {
        ITrace trace1 = trace;
        throw CosmosExceptionFactory.Create(ex, trace1);
      }
      return containerPropertiesAsync;
    }

    public override async Task<string> GetCachedRIDAsync(
      bool forceRefresh,
      ITrace trace,
      CancellationToken cancellationToken)
    {
      ContainerCore containerCore = this;
      string resourceId;
      using (trace.StartChild("Get RID", TraceComponent.Routing, TraceLevel.Info))
        resourceId = (await containerCore.GetCachedContainerPropertiesAsync(forceRefresh, trace, cancellationToken))?.ResourceId;
      return resourceId;
    }

    public override async Task<PartitionKeyDefinition> GetPartitionKeyDefinitionAsync(
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return (await this.GetCachedContainerPropertiesAsync(false, (ITrace) NoOpTrace.Singleton, cancellationToken))?.PartitionKey;
    }

    public override async Task<IReadOnlyList<IReadOnlyList<string>>> GetPartitionKeyPathTokensAsync(
      ITrace trace,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      ContainerCore containerCore = this;
      ContainerProperties containerPropertiesAsync = await containerCore.GetCachedContainerPropertiesAsync(false, trace, cancellationToken);
      if (containerPropertiesAsync == null)
        throw new ArgumentOutOfRangeException("Container " + containerCore.LinkUri + " not found");
      if (containerPropertiesAsync.PartitionKey?.Paths == null)
        throw new ArgumentOutOfRangeException("Partition key not defined for container " + containerCore.LinkUri);
      return containerPropertiesAsync.PartitionKeyPathTokens;
    }

    public override async Task<PartitionKeyInternal> GetNonePartitionKeyValueAsync(
      ITrace trace,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return (await this.GetCachedContainerPropertiesAsync(false, trace, cancellationToken)).GetNoneValue();
    }

    public override async Task<CollectionRoutingMap> GetRoutingMapAsync(
      CancellationToken cancellationToken)
    {
      ContainerCore containerCore = this;
      string collectionRid = await containerCore.GetCachedRIDAsync(false, (ITrace) NoOpTrace.Singleton, cancellationToken);
      PartitionKeyRangeCache partitionKeyRangeCache = await containerCore.ClientContext.Client.DocumentClient.GetPartitionKeyRangeCacheAsync((ITrace) NoOpTrace.Singleton);
      CollectionRoutingMap collectionRoutingMap = await partitionKeyRangeCache.TryLookupAsync(collectionRid, (CollectionRoutingMap) null, (DocumentServiceRequest) null, (ITrace) NoOpTrace.Singleton);
      if (collectionRoutingMap == null)
      {
        collectionRid = await containerCore.GetCachedRIDAsync(true, (ITrace) NoOpTrace.Singleton, cancellationToken);
        collectionRoutingMap = await partitionKeyRangeCache.TryLookupAsync(collectionRid, (CollectionRoutingMap) null, (DocumentServiceRequest) null, (ITrace) NoOpTrace.Singleton);
      }
      CollectionRoutingMap routingMapAsync = collectionRoutingMap;
      collectionRid = (string) null;
      partitionKeyRangeCache = (PartitionKeyRangeCache) null;
      return routingMapAsync;
    }

    private async Task<ThroughputResponse> OfferRetryHelperForStaleRidCacheAsync(
      Func<string, Task<ThroughputResponse>> executeOfferOperation,
      ITrace trace,
      CancellationToken cancellationToken)
    {
      ContainerCore containerCore = this;
      string rid = await containerCore.GetCachedRIDAsync(false, trace, cancellationToken);
      ThroughputResponse throughputResponse = await executeOfferOperation(rid);
      if (throughputResponse.StatusCode != HttpStatusCode.NotFound)
        return throughputResponse;
      ResponseMessage responseMessage = await containerCore.ReadContainerStreamAsync(trace, cancellationToken: cancellationToken);
      if (responseMessage.StatusCode == HttpStatusCode.NotFound)
        return new ThroughputResponse(responseMessage.StatusCode, responseMessage.Headers, (ThroughputProperties) null, (CosmosDiagnostics) new CosmosTraceDiagnostics(trace), responseMessage.RequestMessage);
      responseMessage.EnsureSuccessStatusCode();
      ContainerProperties containerProperties = containerCore.ClientContext.SerializerCore.FromStream<ContainerProperties>(responseMessage.Content);
      return string.Equals(rid, containerProperties.ResourceId) ? throughputResponse : await executeOfferOperation(containerProperties.ResourceId);
    }

    private Task<ResponseMessage> ReplaceStreamInternalAsync(
      Stream streamPayload,
      ITrace trace,
      ContainerRequestOptions requestOptions = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.ProcessStreamAsync(streamPayload, Microsoft.Azure.Documents.OperationType.Replace, (RequestOptions) requestOptions, trace, cancellationToken);
    }

    private Task<ResponseMessage> ProcessStreamAsync(
      Stream streamPayload,
      Microsoft.Azure.Documents.OperationType operationType,
      RequestOptions requestOptions,
      ITrace trace,
      CancellationToken cancellationToken)
    {
      Stream streamPayload1 = streamPayload;
      int num = (int) operationType;
      string linkUri = this.LinkUri;
      RequestOptions requestOptions1 = requestOptions;
      ITrace trace1 = trace;
      RequestOptions requestOptions2 = requestOptions1;
      CancellationToken cancellationToken1 = cancellationToken;
      return this.ProcessResourceOperationStreamAsync(streamPayload1, (Microsoft.Azure.Documents.OperationType) num, linkUri, ResourceType.Collection, trace1, requestOptions2, cancellationToken1);
    }

    private Task<ResponseMessage> ProcessResourceOperationStreamAsync(
      Stream streamPayload,
      Microsoft.Azure.Documents.OperationType operationType,
      string linkUri,
      ResourceType resourceType,
      ITrace trace,
      RequestOptions requestOptions = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      CosmosClientContext clientContext = this.ClientContext;
      string resourceUri = linkUri;
      int num1 = (int) resourceType;
      int num2 = (int) operationType;
      Stream stream = streamPayload;
      RequestOptions requestOptions1 = requestOptions;
      Stream streamPayload1 = stream;
      ITrace trace1 = trace;
      CancellationToken cancellationToken1 = cancellationToken;
      return clientContext.ProcessResourceOperationStreamAsync(resourceUri, (ResourceType) num1, (Microsoft.Azure.Documents.OperationType) num2, requestOptions1, (ContainerInternal) null, (FeedRange) null, streamPayload1, (Action<RequestMessage>) null, trace1, cancellationToken1);
    }

    public override FeedIterator GetChangeFeedStreamIteratorWithQuery(
      ChangeFeedStartFrom changeFeedStartFrom,
      ChangeFeedMode changeFeedMode,
      ChangeFeedQuerySpec changeFeedQuerySpec,
      ChangeFeedRequestOptions changeFeedRequestOptions = null)
    {
      if (changeFeedStartFrom == null)
        throw new ArgumentNullException(nameof (changeFeedStartFrom));
      if (changeFeedMode == null)
        throw new ArgumentNullException(nameof (changeFeedMode));
      DocumentContainer documentContainer = new DocumentContainer((IMonadicDocumentContainer) new NetworkAttachedDocumentContainer((ContainerInternal) this, this.queryClient, Guid.NewGuid(), changeFeedRequestOptions: changeFeedRequestOptions));
      ChangeFeedStartFrom changeFeedStartFrom1 = changeFeedStartFrom;
      ChangeFeedMode changeFeedMode1 = changeFeedMode;
      ChangeFeedRequestOptions changeFeedRequestOptions1 = changeFeedRequestOptions;
      ChangeFeedStartFrom changeFeedStartFrom2 = changeFeedStartFrom1;
      CosmosClientContext clientContext = this.ClientContext;
      ChangeFeedQuerySpec changeFeedQuerySpec1 = changeFeedQuerySpec;
      return (FeedIterator) new ChangeFeedIteratorCore((IDocumentContainer) documentContainer, changeFeedMode1, changeFeedRequestOptions1, changeFeedStartFrom2, clientContext, (ContainerInternal) this, changeFeedQuerySpec1);
    }

    public override FeedIterator<T> GetChangeFeedIteratorWithQuery<T>(
      ChangeFeedStartFrom changeFeedStartFrom,
      ChangeFeedMode changeFeedMode,
      ChangeFeedQuerySpec changeFeedQuerySpec,
      ChangeFeedRequestOptions changeFeedRequestOptions = null)
    {
      if (changeFeedStartFrom == null)
        throw new ArgumentNullException(nameof (changeFeedStartFrom));
      if (changeFeedMode == null)
        throw new ArgumentNullException(nameof (changeFeedMode));
      DocumentContainer documentContainer = new DocumentContainer((IMonadicDocumentContainer) new NetworkAttachedDocumentContainer((ContainerInternal) this, this.queryClient, Guid.NewGuid(), changeFeedRequestOptions: changeFeedRequestOptions));
      ChangeFeedStartFrom changeFeedStartFrom1 = changeFeedStartFrom;
      ChangeFeedMode changeFeedMode1 = changeFeedMode;
      ChangeFeedRequestOptions changeFeedRequestOptions1 = changeFeedRequestOptions;
      ChangeFeedStartFrom changeFeedStartFrom2 = changeFeedStartFrom1;
      CosmosClientContext clientContext = this.ClientContext;
      ChangeFeedQuerySpec changeFeedQuerySpec1 = changeFeedQuerySpec;
      return (FeedIterator<T>) new FeedIteratorCore<T>((FeedIteratorInternal) new ChangeFeedIteratorCore((IDocumentContainer) documentContainer, changeFeedMode1, changeFeedRequestOptions1, changeFeedStartFrom2, clientContext, (ContainerInternal) this, changeFeedQuerySpec1), new Func<ResponseMessage, FeedResponse<T>>(this.ClientContext.ResponseFactory.CreateChangeFeedUserTypeResponse<T>));
    }

    private string cachedUriSegmentWithoutId { get; }

    public async Task<ResponseMessage> CreateItemStreamAsync(
      Stream streamPayload,
      PartitionKey partitionKey,
      ITrace trace,
      ItemRequestOptions requestOptions = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return await this.ProcessItemStreamAsync(new PartitionKey?(partitionKey), (string) null, streamPayload, Microsoft.Azure.Documents.OperationType.Create, requestOptions, trace, cancellationToken);
    }

    public async Task<ItemResponse<T>> CreateItemAsync<T>(
      T item,
      ITrace trace,
      PartitionKey? partitionKey = null,
      ItemRequestOptions requestOptions = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      ContainerCore containerCore = this;
      if ((object) (T) item == null)
        throw new ArgumentNullException(nameof (item));
      ResponseMessage processItemStreamAsync = await containerCore.ExtractPartitionKeyAndProcessItemStreamAsync<T>(partitionKey, (string) null, item, Microsoft.Azure.Documents.OperationType.Create, requestOptions, trace, cancellationToken);
      return containerCore.ClientContext.ResponseFactory.CreateItemResponse<T>(processItemStreamAsync);
    }

    public async Task<ResponseMessage> ReadItemStreamAsync(
      string id,
      PartitionKey partitionKey,
      ITrace trace,
      ItemRequestOptions requestOptions = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return await this.ProcessItemStreamAsync(new PartitionKey?(partitionKey), id, (Stream) null, Microsoft.Azure.Documents.OperationType.Read, requestOptions, trace, cancellationToken);
    }

    public async Task<ItemResponse<T>> ReadItemAsync<T>(
      string id,
      PartitionKey partitionKey,
      ITrace trace,
      ItemRequestOptions requestOptions = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      ContainerCore containerCore = this;
      ResponseMessage responseMessage = await containerCore.ProcessItemStreamAsync(new PartitionKey?(partitionKey), id, (Stream) null, Microsoft.Azure.Documents.OperationType.Read, requestOptions, trace, cancellationToken);
      return containerCore.ClientContext.ResponseFactory.CreateItemResponse<T>(responseMessage);
    }

    public async Task<ResponseMessage> UpsertItemStreamAsync(
      Stream streamPayload,
      PartitionKey partitionKey,
      ITrace trace,
      ItemRequestOptions requestOptions = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return await this.ProcessItemStreamAsync(new PartitionKey?(partitionKey), (string) null, streamPayload, Microsoft.Azure.Documents.OperationType.Upsert, requestOptions, trace, cancellationToken);
    }

    public async Task<ItemResponse<T>> UpsertItemAsync<T>(
      T item,
      ITrace trace,
      PartitionKey? partitionKey = null,
      ItemRequestOptions requestOptions = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      ContainerCore containerCore = this;
      if ((object) (T) item == null)
        throw new ArgumentNullException(nameof (item));
      ResponseMessage processItemStreamAsync = await containerCore.ExtractPartitionKeyAndProcessItemStreamAsync<T>(partitionKey, (string) null, item, Microsoft.Azure.Documents.OperationType.Upsert, requestOptions, trace, cancellationToken);
      return containerCore.ClientContext.ResponseFactory.CreateItemResponse<T>(processItemStreamAsync);
    }

    public async Task<ResponseMessage> ReplaceItemStreamAsync(
      Stream streamPayload,
      string id,
      PartitionKey partitionKey,
      ITrace trace,
      ItemRequestOptions requestOptions = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return await this.ProcessItemStreamAsync(new PartitionKey?(partitionKey), id, streamPayload, Microsoft.Azure.Documents.OperationType.Replace, requestOptions, trace, cancellationToken);
    }

    public async Task<ItemResponse<T>> ReplaceItemAsync<T>(
      T item,
      string id,
      ITrace trace,
      PartitionKey? partitionKey = null,
      ItemRequestOptions requestOptions = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      ContainerCore containerCore = this;
      if (id == null)
        throw new ArgumentNullException(nameof (id));
      if ((object) (T) item == null)
        throw new ArgumentNullException(nameof (item));
      ResponseMessage processItemStreamAsync = await containerCore.ExtractPartitionKeyAndProcessItemStreamAsync<T>(partitionKey, id, item, Microsoft.Azure.Documents.OperationType.Replace, requestOptions, trace, cancellationToken);
      return containerCore.ClientContext.ResponseFactory.CreateItemResponse<T>(processItemStreamAsync);
    }

    public async Task<ResponseMessage> DeleteItemStreamAsync(
      string id,
      PartitionKey partitionKey,
      ITrace trace,
      ItemRequestOptions requestOptions = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return await this.ProcessItemStreamAsync(new PartitionKey?(partitionKey), id, (Stream) null, Microsoft.Azure.Documents.OperationType.Delete, requestOptions, trace, cancellationToken);
    }

    public async Task<ItemResponse<T>> DeleteItemAsync<T>(
      string id,
      PartitionKey partitionKey,
      ITrace trace,
      ItemRequestOptions requestOptions = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      ContainerCore containerCore = this;
      ResponseMessage responseMessage = await containerCore.ProcessItemStreamAsync(new PartitionKey?(partitionKey), id, (Stream) null, Microsoft.Azure.Documents.OperationType.Delete, requestOptions, trace, cancellationToken);
      return containerCore.ClientContext.ResponseFactory.CreateItemResponse<T>(responseMessage);
    }

    public override FeedIterator GetItemQueryStreamIterator(
      string queryText = null,
      string continuationToken = null,
      QueryRequestOptions requestOptions = null)
    {
      QueryDefinition queryDefinition = (QueryDefinition) null;
      if (queryText != null)
        queryDefinition = new QueryDefinition(queryText);
      return this.GetItemQueryStreamIterator(queryDefinition, continuationToken, requestOptions);
    }

    public override FeedIterator GetItemQueryStreamIterator(
      QueryDefinition queryDefinition,
      string continuationToken = null,
      QueryRequestOptions requestOptions = null)
    {
      return (FeedIterator) this.GetItemQueryStreamIteratorInternal(queryDefinition?.ToSqlQuerySpec(), true, continuationToken, (FeedRangeInternal) null, requestOptions);
    }

    public async Task<ResponseMessage> ReadManyItemsStreamAsync(
      IReadOnlyList<(string id, PartitionKey partitionKey)> items,
      ITrace trace,
      ReadManyRequestOptions readManyRequestOptions = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      ContainerCore container = this;
      if (items == null)
        throw new ArgumentNullException(nameof (items));
      if (trace == null)
        throw new ArgumentNullException(nameof (trace));
      PartitionKeyDefinition keyDefinitionAsync;
      try
      {
        keyDefinitionAsync = await container.GetPartitionKeyDefinitionAsync(new CancellationToken());
      }
      catch (CosmosException ex)
      {
        return ex.ToCosmosResponseMessage((RequestMessage) null);
      }
      return await new ReadManyQueryHelper(keyDefinitionAsync, container).ExecuteReadManyRequestAsync(items, readManyRequestOptions, trace, cancellationToken);
    }

    public async Task<FeedResponse<T>> ReadManyItemsAsync<T>(
      IReadOnlyList<(string id, PartitionKey partitionKey)> items,
      ITrace trace,
      ReadManyRequestOptions readManyRequestOptions = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      ContainerCore container = this;
      if (items == null)
        throw new ArgumentNullException(nameof (items));
      if (trace == null)
        throw new ArgumentNullException(nameof (trace));
      return await new ReadManyQueryHelper(await container.GetPartitionKeyDefinitionAsync(new CancellationToken()), container).ExecuteReadManyRequestAsync<T>(items, readManyRequestOptions, trace, cancellationToken);
    }

    public override async Task<ContainerInternal.TryExecuteQueryResult> TryExecuteQueryAsync(
      QueryFeatures supportedQueryFeatures,
      QueryDefinition queryDefinition,
      string continuationToken,
      FeedRangeInternal feedRangeInternal,
      QueryRequestOptions requestOptions,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      ContainerCore containerCore = this;
      if (queryDefinition == null)
        throw new ArgumentNullException(nameof (queryDefinition));
      if (requestOptions == null)
        throw new ArgumentNullException(nameof (requestOptions));
      if (feedRangeInternal != null)
        return (ContainerInternal.TryExecuteQueryResult) new ContainerInternal.QueryPlanIsSupportedResult(QueryIterator.Create(containerCore, containerCore.queryClient, containerCore.ClientContext, queryDefinition.ToSqlQuerySpec(), continuationToken, feedRangeInternal, requestOptions, containerCore.LinkUri, false, true, true, (Microsoft.Azure.Cosmos.Query.Core.QueryPlan.PartitionedQueryExecutionInfo) null));
      cancellationToken.ThrowIfCancellationRequested();
      object obj;
      PartitionKeyDefinition partitionKeyDefinition1;
      if (requestOptions.Properties != null && requestOptions.Properties.TryGetValue("x-ms-query-partitionkey-definition", out obj))
        partitionKeyDefinition1 = obj is PartitionKeyDefinition partitionKeyDefinition2 ? partitionKeyDefinition2 : throw new ArgumentException("partitionkeydefinition has invalid type", "partitionKeyDefinitionObject");
      else
        partitionKeyDefinition1 = (await containerCore.queryClient.GetCachedContainerQueryPropertiesAsync(containerCore.LinkUri, requestOptions.PartitionKey, (ITrace) NoOpTrace.Singleton, cancellationToken)).PartitionKeyDefinition;
      TryCatch<(Microsoft.Azure.Cosmos.Query.Core.QueryPlan.PartitionedQueryExecutionInfo, bool)> ifSupportedAsync = await new QueryPlanHandler(containerCore.queryClient).TryGetQueryInfoAndIfSupportedAsync(supportedQueryFeatures, queryDefinition.ToSqlQuerySpec(), ResourceType.Document, partitionKeyDefinition1, requestOptions.PartitionKey.HasValue, QueryIterator.IsSystemPrefixExpected(requestOptions), cancellationToken);
      if (ifSupportedAsync.Failed)
        return (ContainerInternal.TryExecuteQueryResult) new ContainerInternal.FailedToGetQueryPlanResult(ifSupportedAsync.Exception);
      (Microsoft.Azure.Cosmos.Query.Core.QueryPlan.PartitionedQueryExecutionInfo, bool) result = ifSupportedAsync.Result;
      Microsoft.Azure.Cosmos.Query.Core.QueryPlan.PartitionedQueryExecutionInfo partitionedQueryExecutionInfo = result.Item1;
      return !result.Item2 ? (ContainerInternal.TryExecuteQueryResult) new ContainerInternal.QueryPlanNotSupportedResult(partitionedQueryExecutionInfo) : (ContainerInternal.TryExecuteQueryResult) new ContainerInternal.QueryPlanIsSupportedResult(QueryIterator.Create(containerCore, containerCore.queryClient, containerCore.ClientContext, queryDefinition.ToSqlQuerySpec(), continuationToken, feedRangeInternal, requestOptions, containerCore.LinkUri, false, true, false, partitionedQueryExecutionInfo));
    }

    public override FeedIterator<T> GetItemQueryIterator<T>(
      string queryText = null,
      string continuationToken = null,
      QueryRequestOptions requestOptions = null)
    {
      QueryDefinition queryDefinition = (QueryDefinition) null;
      if (queryText != null)
        queryDefinition = new QueryDefinition(queryText);
      return this.GetItemQueryIterator<T>(queryDefinition, continuationToken, requestOptions);
    }

    public override FeedIterator<T> GetItemQueryIterator<T>(
      QueryDefinition queryDefinition,
      string continuationToken = null,
      QueryRequestOptions requestOptions = null)
    {
      if (requestOptions == null)
        requestOptions = new QueryRequestOptions();
      if (requestOptions.IsEffectivePartitionKeyRouting)
        requestOptions.PartitionKey = new PartitionKey?();
      if (!(this.GetItemQueryStreamIterator(queryDefinition, continuationToken, requestOptions) is FeedIteratorInternal queryStreamIterator))
        throw new InvalidOperationException("Expected a FeedIteratorInternal.");
      return (FeedIterator<T>) new FeedIteratorCore<T>(queryStreamIterator, new Func<ResponseMessage, FeedResponse<T>>(this.ClientContext.ResponseFactory.CreateQueryFeedUserTypeResponse<T>));
    }

    public override IOrderedQueryable<T> GetItemLinqQueryable<T>(
      bool allowSynchronousQueryExecution = false,
      string continuationToken = null,
      QueryRequestOptions requestOptions = null,
      CosmosLinqSerializerOptions linqSerializerOptions = null)
    {
      if (requestOptions == null)
        requestOptions = new QueryRequestOptions();
      if (linqSerializerOptions == null && this.ClientContext.ClientOptions.SerializerOptions != null)
        linqSerializerOptions = new CosmosLinqSerializerOptions()
        {
          PropertyNamingPolicy = this.ClientContext.ClientOptions.SerializerOptions.PropertyNamingPolicy
        };
      return (IOrderedQueryable<T>) new CosmosLinqQuery<T>((ContainerInternal) this, this.ClientContext.ResponseFactory, (CosmosQueryClientCore) this.queryClient, continuationToken, requestOptions, allowSynchronousQueryExecution, linqSerializerOptions);
    }

    public override FeedIterator<T> GetItemQueryIterator<T>(
      FeedRange feedRange,
      QueryDefinition queryDefinition,
      string continuationToken = null,
      QueryRequestOptions requestOptions = null)
    {
      if (requestOptions == null)
        requestOptions = new QueryRequestOptions();
      if (!(this.GetItemQueryStreamIterator(feedRange, queryDefinition, continuationToken, requestOptions) is FeedIteratorInternal queryStreamIterator))
        throw new InvalidOperationException("Expected a FeedIteratorInternal.");
      return (FeedIterator<T>) new FeedIteratorCore<T>(queryStreamIterator, new Func<ResponseMessage, FeedResponse<T>>(this.ClientContext.ResponseFactory.CreateQueryFeedUserTypeResponse<T>));
    }

    public override FeedIterator GetItemQueryStreamIterator(
      FeedRange feedRange,
      QueryDefinition queryDefinition,
      string continuationToken = null,
      QueryRequestOptions requestOptions = null)
    {
      FeedRangeInternal feedRange1 = feedRange as FeedRangeInternal;
      return (FeedIterator) this.GetItemQueryStreamIteratorInternal(queryDefinition?.ToSqlQuerySpec(), true, continuationToken, feedRange1, requestOptions);
    }

    public override ChangeFeedProcessorBuilder GetChangeFeedProcessorBuilder<T>(
      string processorName,
      Container.ChangesHandler<T> onChangesDelegate)
    {
      if (processorName == null)
        throw new ArgumentNullException(nameof (processorName));
      if (onChangesDelegate == null)
        throw new ArgumentNullException(nameof (onChangesDelegate));
      ChangeFeedObserverFactory observerFactory = (ChangeFeedObserverFactory) new CheckpointerObserverFactory((ChangeFeedObserverFactory) new ChangeFeedObserverFactoryCore<T>(onChangesDelegate, this.ClientContext.SerializerCore), false);
      return this.GetChangeFeedProcessorBuilderPrivate(processorName, observerFactory);
    }

    public override ChangeFeedProcessorBuilder GetChangeFeedProcessorBuilder<T>(
      string processorName,
      Container.ChangeFeedHandler<T> onChangesDelegate)
    {
      if (processorName == null)
        throw new ArgumentNullException(nameof (processorName));
      if (onChangesDelegate == null)
        throw new ArgumentNullException(nameof (onChangesDelegate));
      ChangeFeedObserverFactory observerFactory = (ChangeFeedObserverFactory) new CheckpointerObserverFactory((ChangeFeedObserverFactory) new ChangeFeedObserverFactoryCore<T>(onChangesDelegate, this.ClientContext.SerializerCore), false);
      return this.GetChangeFeedProcessorBuilderPrivate(processorName, observerFactory);
    }

    public override ChangeFeedProcessorBuilder GetChangeFeedProcessorBuilderWithManualCheckpoint<T>(
      string processorName,
      Container.ChangeFeedHandlerWithManualCheckpoint<T> onChangesDelegate)
    {
      if (processorName == null)
        throw new ArgumentNullException(nameof (processorName));
      if (onChangesDelegate == null)
        throw new ArgumentNullException(nameof (onChangesDelegate));
      ChangeFeedObserverFactory observerFactory = (ChangeFeedObserverFactory) new CheckpointerObserverFactory((ChangeFeedObserverFactory) new ChangeFeedObserverFactoryCore<T>(onChangesDelegate, this.ClientContext.SerializerCore), true);
      return this.GetChangeFeedProcessorBuilderPrivate(processorName, observerFactory);
    }

    public override ChangeFeedProcessorBuilder GetChangeFeedProcessorBuilder(
      string processorName,
      Container.ChangeFeedStreamHandler onChangesDelegate)
    {
      if (processorName == null)
        throw new ArgumentNullException(nameof (processorName));
      ChangeFeedObserverFactory observerFactory = onChangesDelegate != null ? (ChangeFeedObserverFactory) new CheckpointerObserverFactory((ChangeFeedObserverFactory) new ChangeFeedObserverFactoryCore(onChangesDelegate), false) : throw new ArgumentNullException(nameof (onChangesDelegate));
      return this.GetChangeFeedProcessorBuilderPrivate(processorName, observerFactory);
    }

    public override ChangeFeedProcessorBuilder GetChangeFeedProcessorBuilderWithManualCheckpoint(
      string processorName,
      Container.ChangeFeedStreamHandlerWithManualCheckpoint onChangesDelegate)
    {
      if (processorName == null)
        throw new ArgumentNullException(nameof (processorName));
      ChangeFeedObserverFactory observerFactory = onChangesDelegate != null ? (ChangeFeedObserverFactory) new CheckpointerObserverFactory((ChangeFeedObserverFactory) new ChangeFeedObserverFactoryCore(onChangesDelegate), true) : throw new ArgumentNullException(nameof (onChangesDelegate));
      return this.GetChangeFeedProcessorBuilderPrivate(processorName, observerFactory);
    }

    public override ChangeFeedProcessorBuilder GetChangeFeedEstimatorBuilder(
      string processorName,
      Container.ChangesEstimationHandler estimationDelegate,
      TimeSpan? estimationPeriod = null)
    {
      if (processorName == null)
        throw new ArgumentNullException(nameof (processorName));
      ChangeFeedEstimatorRunner feedEstimatorRunner = estimationDelegate != null ? new ChangeFeedEstimatorRunner(estimationDelegate, estimationPeriod) : throw new ArgumentNullException(nameof (estimationDelegate));
      return new ChangeFeedProcessorBuilder(processorName, (ContainerInternal) this, (ChangeFeedProcessor) feedEstimatorRunner, new Action<DocumentServiceLeaseStoreManager, ContainerInternal, string, ChangeFeedLeaseOptions, ChangeFeedProcessorOptions, ContainerInternal>(feedEstimatorRunner.ApplyBuildConfiguration));
    }

    public override ChangeFeedEstimator GetChangeFeedEstimator(
      string processorName,
      Container leaseContainer)
    {
      if (processorName == null)
        throw new ArgumentNullException(nameof (processorName));
      return leaseContainer != null ? (ChangeFeedEstimator) new ChangeFeedEstimatorCore(processorName, (ContainerInternal) this, (ContainerInternal) leaseContainer, (DocumentServiceLeaseContainer) null) : throw new ArgumentNullException(nameof (leaseContainer));
    }

    public override TransactionalBatch CreateTransactionalBatch(PartitionKey partitionKey) => (TransactionalBatch) new BatchCore((ContainerInternal) this, partitionKey);

    public override IAsyncEnumerable<TryCatch<Microsoft.Azure.Cosmos.ChangeFeed.ChangeFeedPage>> GetChangeFeedAsyncEnumerable(
      ChangeFeedCrossFeedRangeState state,
      ChangeFeedMode changeFeedMode,
      ChangeFeedRequestOptions changeFeedRequestOptions = null)
    {
      DocumentContainer documentContainer = new DocumentContainer((IMonadicDocumentContainer) new NetworkAttachedDocumentContainer((ContainerInternal) this, this.queryClient, Guid.NewGuid(), changeFeedRequestOptions: changeFeedRequestOptions));
      Dictionary<string, string> additionalHeaders;
      if (changeFeedRequestOptions?.Properties != null && changeFeedRequestOptions.Properties.Any<KeyValuePair<string, object>>())
      {
        Dictionary<string, object> dictionary = new Dictionary<string, object>();
        additionalHeaders = new Dictionary<string, string>();
        foreach (KeyValuePair<string, object> property in (IEnumerable<KeyValuePair<string, object>>) changeFeedRequestOptions.Properties)
        {
          if (property.Value is string str)
            additionalHeaders[property.Key] = str;
          else
            dictionary[property.Key] = property.Value;
        }
        changeFeedRequestOptions.Properties = (IReadOnlyDictionary<string, object>) dictionary;
      }
      else
        additionalHeaders = (Dictionary<string, string>) null;
      ChangeFeedPaginationOptions changeFeedPaginationOptions = new ChangeFeedPaginationOptions(changeFeedMode, (int?) changeFeedRequestOptions?.PageSizeHint, changeFeedRequestOptions?.JsonSerializationFormatOptions?.JsonSerializationFormat, additionalHeaders);
      return (IAsyncEnumerable<TryCatch<Microsoft.Azure.Cosmos.ChangeFeed.ChangeFeedPage>>) new ChangeFeedCrossFeedRangeAsyncEnumerable((IDocumentContainer) documentContainer, state, changeFeedPaginationOptions, changeFeedRequestOptions?.JsonSerializationFormatOptions);
    }

    public override FeedIterator GetStandByFeedIterator(
      string continuationToken = null,
      int? maxItemCount = null,
      StandByFeedIteratorRequestOptions requestOptions = null)
    {
      StandByFeedIteratorRequestOptions iteratorRequestOptions = requestOptions ?? new StandByFeedIteratorRequestOptions();
      CosmosClientContext clientContext = this.ClientContext;
      string str = continuationToken;
      int? nullable = maxItemCount;
      string continuationToken1 = str;
      int? maxItemCount1 = nullable;
      StandByFeedIteratorRequestOptions options = iteratorRequestOptions;
      return (FeedIterator) new StandByFeedIteratorCore(clientContext, (ContainerInternal) this, continuationToken1, maxItemCount1, options);
    }

    public override FeedIteratorInternal GetItemQueryStreamIteratorInternal(
      SqlQuerySpec sqlQuerySpec,
      bool isContinuationExcpected,
      string continuationToken,
      FeedRangeInternal feedRange,
      QueryRequestOptions requestOptions)
    {
      if (requestOptions == null)
        requestOptions = new QueryRequestOptions();
      if (requestOptions.IsEffectivePartitionKeyRouting)
      {
        if (feedRange != null)
          throw new ArgumentException(nameof (feedRange), ClientResources.FeedToken_EffectivePartitionKeyRouting);
        requestOptions.PartitionKey = new PartitionKey?();
      }
      if (sqlQuerySpec != null)
        return (FeedIteratorInternal) QueryIterator.Create(this, this.queryClient, this.ClientContext, sqlQuerySpec, continuationToken, feedRange, requestOptions, this.LinkUri, isContinuationExcpected, true, false, (Microsoft.Azure.Cosmos.Query.Core.QueryPlan.PartitionedQueryExecutionInfo) null);
      DocumentContainer documentContainer = new DocumentContainer((IMonadicDocumentContainer) new NetworkAttachedDocumentContainer((ContainerInternal) this, this.queryClient, Guid.NewGuid(), requestOptions));
      ReadFeedPaginationOptions.PaginationDirection? paginationDirection = new ReadFeedPaginationOptions.PaginationDirection?();
      object obj;
      if (requestOptions.Properties != null && requestOptions.Properties.TryGetValue("x-ms-enumeration-direction", out obj))
        paginationDirection = new ReadFeedPaginationOptions.PaginationDirection?((byte) obj == (byte) 2 ? ReadFeedPaginationOptions.PaginationDirection.Reverse : ReadFeedPaginationOptions.PaginationDirection.Forward);
      ReadFeedPaginationOptions paginationOptions = new ReadFeedPaginationOptions(paginationDirection, new int?(requestOptions.MaxItemCount ?? int.MaxValue));
      string continuationToken1 = continuationToken;
      ReadFeedPaginationOptions readFeedPaginationOptions = paginationOptions;
      QueryRequestOptions queryRequestOptions = requestOptions;
      CancellationToken cancellationToken = new CancellationToken();
      return (FeedIteratorInternal) new ReadFeedIteratorCore((IDocumentContainer) documentContainer, continuationToken1, readFeedPaginationOptions, queryRequestOptions, (ContainerInternal) this, cancellationToken);
    }

    public override FeedIteratorInternal GetReadFeedIterator(
      QueryDefinition queryDefinition,
      QueryRequestOptions queryRequestOptions,
      string resourceLink,
      ResourceType resourceType,
      string continuationToken,
      int pageSize)
    {
      if (queryRequestOptions == null)
        queryRequestOptions = new QueryRequestOptions();
      DocumentContainer documentContainer1 = new DocumentContainer((IMonadicDocumentContainer) new NetworkAttachedDocumentContainer((ContainerInternal) this, this.queryClient, Guid.NewGuid(), queryRequestOptions, resourceLink: resourceLink, resourceType: resourceType));
      FeedIteratorInternal readFeedIterator;
      if (queryDefinition != null)
      {
        readFeedIterator = (FeedIteratorInternal) QueryIterator.Create(this, this.queryClient, this.ClientContext, queryDefinition.ToSqlQuerySpec(), continuationToken, (FeedRangeInternal) FeedRangeEpk.FullRange, queryRequestOptions, resourceLink, false, true, false, (Microsoft.Azure.Cosmos.Query.Core.QueryPlan.PartitionedQueryExecutionInfo) null);
      }
      else
      {
        ReadFeedPaginationOptions.PaginationDirection? paginationDirection = new ReadFeedPaginationOptions.PaginationDirection?();
        object obj;
        if (queryRequestOptions.Properties != null && queryRequestOptions.Properties.TryGetValue("x-ms-enumeration-direction", out obj))
          paginationDirection = new ReadFeedPaginationOptions.PaginationDirection?((byte) obj == (byte) 2 ? ReadFeedPaginationOptions.PaginationDirection.Reverse : ReadFeedPaginationOptions.PaginationDirection.Forward);
        ReadFeedPaginationOptions paginationOptions = new ReadFeedPaginationOptions(paginationDirection, new int?(queryRequestOptions.MaxItemCount ?? int.MaxValue));
        DocumentContainer documentContainer2 = documentContainer1;
        QueryRequestOptions queryRequestOptions1 = queryRequestOptions;
        string continuationToken1 = continuationToken;
        ReadFeedPaginationOptions readFeedPaginationOptions = paginationOptions;
        QueryRequestOptions queryRequestOptions2 = queryRequestOptions1;
        CancellationToken cancellationToken = new CancellationToken();
        readFeedIterator = (FeedIteratorInternal) new ReadFeedIteratorCore((IDocumentContainer) documentContainer2, continuationToken1, readFeedPaginationOptions, queryRequestOptions2, (ContainerInternal) this, cancellationToken);
      }
      return readFeedIterator;
    }

    public override IAsyncEnumerable<TryCatch<Microsoft.Azure.Cosmos.ReadFeed.ReadFeedPage>> GetReadFeedAsyncEnumerable(
      ReadFeedCrossFeedRangeState state,
      QueryRequestOptions queryRequestOptions = null)
    {
      DocumentContainer documentContainer = new DocumentContainer((IMonadicDocumentContainer) new NetworkAttachedDocumentContainer((ContainerInternal) this, this.queryClient, Guid.NewGuid(), queryRequestOptions));
      ReadFeedPaginationOptions.PaginationDirection? paginationDirection = new ReadFeedPaginationOptions.PaginationDirection?();
      object obj;
      if (queryRequestOptions?.Properties != null && queryRequestOptions.Properties.TryGetValue("x-ms-enumeration-direction", out obj))
        paginationDirection = new ReadFeedPaginationOptions.PaginationDirection?((byte) obj == (byte) 2 ? ReadFeedPaginationOptions.PaginationDirection.Reverse : ReadFeedPaginationOptions.PaginationDirection.Forward);
      ReadFeedPaginationOptions paginationOptions = new ReadFeedPaginationOptions(paginationDirection, (int?) queryRequestOptions?.MaxItemCount);
      ReadFeedCrossFeedRangeState state1 = state;
      ReadFeedPaginationOptions readFeedPaginationOptions = paginationOptions;
      return (IAsyncEnumerable<TryCatch<Microsoft.Azure.Cosmos.ReadFeed.ReadFeedPage>>) new ReadFeedCrossFeedRangeAsyncEnumerable((IDocumentContainer) documentContainer, state1, readFeedPaginationOptions);
    }

    private async Task<ResponseMessage> ExtractPartitionKeyAndProcessItemStreamAsync<T>(
      PartitionKey? partitionKey,
      string itemId,
      T item,
      Microsoft.Azure.Documents.OperationType operationType,
      ItemRequestOptions requestOptions,
      ITrace trace,
      CancellationToken cancellationToken)
    {
      ContainerCore containerCore = this;
      if (trace == null)
        throw new ArgumentNullException(nameof (trace));
      Stream itemStream;
      using (trace.StartChild("ItemSerialize"))
        itemStream = containerCore.ClientContext.SerializerCore.ToStream<T>(item);
      if (partitionKey.HasValue)
      {
        PartitionKeyDefinition keyDefinitionAsync = await containerCore.GetPartitionKeyDefinitionAsync(new CancellationToken());
        if (partitionKey.HasValue && partitionKey.Value != PartitionKey.None && partitionKey.Value.InternalKey.Components.Count != keyDefinitionAsync.Paths.Count)
          throw new ArgumentException(RMResources.MissingPartitionKeyValue);
        return await containerCore.ProcessItemStreamAsync(partitionKey, itemId, itemStream, operationType, requestOptions, trace, cancellationToken);
      }
      PartitionKeyMismatchRetryPolicy requestRetryPolicy = (PartitionKeyMismatchRetryPolicy) null;
      ResponseMessage responseMessage;
      while (true)
      {
        partitionKey = new PartitionKey?(await containerCore.GetPartitionKeyValueFromStreamAsync(itemStream, trace, cancellationToken));
        responseMessage = await containerCore.ProcessItemStreamAsync(partitionKey, itemId, itemStream, operationType, requestOptions, trace, cancellationToken);
        if (!responseMessage.IsSuccessStatusCode)
        {
          if (requestRetryPolicy == null)
            requestRetryPolicy = new PartitionKeyMismatchRetryPolicy((CollectionCache) await containerCore.ClientContext.DocumentClient.GetCollectionCacheAsync(trace), (IDocumentClientRetryPolicy) requestRetryPolicy);
          if ((await requestRetryPolicy.ShouldRetryAsync(responseMessage, cancellationToken)).ShouldRetry)
            responseMessage = (ResponseMessage) null;
          else
            goto label_20;
        }
        else
          break;
      }
      return responseMessage;
label_20:
      return responseMessage;
    }

    private async Task<ResponseMessage> ProcessItemStreamAsync(
      PartitionKey? partitionKey,
      string itemId,
      Stream streamPayload,
      Microsoft.Azure.Documents.OperationType operationType,
      ItemRequestOptions requestOptions,
      ITrace trace,
      CancellationToken cancellationToken)
    {
      ContainerCore cosmosContainerCore = this;
      if (trace == null)
        throw new ArgumentNullException(nameof (trace));
      if (requestOptions != null && requestOptions.IsEffectivePartitionKeyRouting)
        partitionKey = new PartitionKey?();
      ContainerInternal.ValidatePartitionKey((object) partitionKey, (RequestOptions) requestOptions);
      string resourceUri = cosmosContainerCore.GetResourceUri((RequestOptions) requestOptions, operationType, itemId);
      return await cosmosContainerCore.ClientContext.ProcessResourceOperationStreamAsync(resourceUri, ResourceType.Document, operationType, (RequestOptions) requestOptions, (ContainerInternal) cosmosContainerCore, partitionKey, itemId, streamPayload, (Action<RequestMessage>) null, trace, cancellationToken);
    }

    public override async Task<PartitionKey> GetPartitionKeyValueFromStreamAsync(
      Stream stream,
      ITrace trace,
      CancellationToken cancellation = default (CancellationToken))
    {
      ContainerCore containerCore = this;
      if (!stream.CanSeek)
        throw new ArgumentException("Stream needs to be seekable", nameof (stream));
      PartitionKey partitionKeyObject;
      using (ITrace childTrace = trace.StartChild("Get PkValue From Stream", TraceComponent.Routing, TraceLevel.Info))
      {
        try
        {
          stream.Position = 0L;
          if (!(stream is MemoryStream destination))
          {
            destination = new MemoryStream();
            stream.CopyTo((Stream) destination);
          }
          IJsonNavigator jsonNavigator = JsonNavigator.Create((ReadOnlyMemory<byte>) destination.ToArray());
          IJsonNavigatorNode rootNode = jsonNavigator.GetRootNode();
          CosmosObject pathTraversal = CosmosObject.Create(jsonNavigator, rootNode);
          IReadOnlyList<IReadOnlyList<string>> keyPathTokensAsync = await containerCore.GetPartitionKeyPathTokensAsync(childTrace, cancellation);
          List<CosmosElement> cosmosElementList = new List<CosmosElement>(keyPathTokensAsync.Count);
          foreach (IReadOnlyList<string> tokens in (IEnumerable<IReadOnlyList<string>>) keyPathTokensAsync)
          {
            CosmosElement result;
            if (ContainerCore.TryParseTokenListForElement(pathTraversal, tokens, out result))
              cosmosElementList.Add(result);
            else
              cosmosElementList.Add((CosmosElement) null);
          }
          partitionKeyObject = ContainerCore.CosmosElementToPartitionKeyObject((IReadOnlyList<CosmosElement>) cosmosElementList);
        }
        finally
        {
          stream.Position = 0L;
        }
      }
      return partitionKeyObject;
    }

    public Task<ResponseMessage> DeleteAllItemsByPartitionKeyStreamAsync(
      PartitionKey partitionKey,
      ITrace trace,
      RequestOptions requestOptions = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      PartitionKey? partitionKey1 = requestOptions == null || !requestOptions.IsEffectivePartitionKeyRouting ? new PartitionKey?(partitionKey) : new PartitionKey?();
      ContainerInternal.ValidatePartitionKey((object) partitionKey1, requestOptions);
      return this.ClientContext.ProcessResourceOperationStreamAsync(this.LinkUri, ResourceType.PartitionKey, Microsoft.Azure.Documents.OperationType.Delete, requestOptions, (ContainerInternal) this, partitionKey1, (string) null, (Stream) null, (Action<RequestMessage>) null, trace, cancellationToken);
    }

    private static bool TryParseTokenListForElement(
      CosmosObject pathTraversal,
      IReadOnlyList<string> tokens,
      out CosmosElement result)
    {
      result = (CosmosElement) null;
      for (int index = 0; index < tokens.Count - 1; ++index)
      {
        if (!pathTraversal.TryGetValue<CosmosObject>(tokens[index], out pathTraversal))
          return false;
      }
      return pathTraversal.TryGetValue(tokens[tokens.Count - 1], out result);
    }

    private static PartitionKey CosmosElementToPartitionKeyObject(
      IReadOnlyList<CosmosElement> cosmosElementList)
    {
      PartitionKeyBuilder partitionKeyBuilder = new PartitionKeyBuilder();
      foreach (CosmosElement cosmosElement in (IEnumerable<CosmosElement>) cosmosElementList)
      {
        if (cosmosElement == (CosmosElement) null)
        {
          partitionKeyBuilder.AddNoneType();
        }
        else
        {
          switch (cosmosElement)
          {
            case CosmosString cosmosString:
              partitionKeyBuilder.Add(UtfAnyString.op_Implicit(cosmosString.Value));
              continue;
            case CosmosNumber cosmosNumber:
              partitionKeyBuilder.Add(Number64.ToDouble(cosmosNumber.Value));
              continue;
            case CosmosBoolean cosmosBoolean:
              partitionKeyBuilder.Add(cosmosBoolean.Value);
              continue;
            case CosmosNull _:
              partitionKeyBuilder.AddNullValue();
              continue;
            default:
              throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, RMResources.UnsupportedPartitionKeyComponentValue, (object) cosmosElement));
          }
        }
      }
      return partitionKeyBuilder.Build();
    }

    private string GetResourceUri(
      RequestOptions requestOptions,
      Microsoft.Azure.Documents.OperationType operationType,
      string itemId)
    {
      Uri resourceUri;
      if (requestOptions != null && requestOptions.TryGetResourceUri(out resourceUri))
        return resourceUri.OriginalString;
      return operationType == Microsoft.Azure.Documents.OperationType.Create || operationType == Microsoft.Azure.Documents.OperationType.Upsert ? this.LinkUri : this.ContcatCachedUriWithId(itemId);
    }

    private string GetResourceSegmentUriWithoutId()
    {
      StringBuilder stringBuilder = new StringBuilder(this.LinkUri.Length + "docs".Length + 2);
      stringBuilder.Append(this.LinkUri);
      stringBuilder.Append("/");
      stringBuilder.Append("docs");
      stringBuilder.Append("/");
      return stringBuilder.ToString();
    }

    private string ContcatCachedUriWithId(string resourceId) => this.cachedUriSegmentWithoutId + Uri.EscapeUriString(resourceId);

    public async Task<ItemResponse<T>> PatchItemAsync<T>(
      string id,
      PartitionKey partitionKey,
      IReadOnlyList<PatchOperation> patchOperations,
      ITrace trace,
      PatchItemRequestOptions requestOptions = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      ContainerCore containerCore = this;
      ResponseMessage responseMessage = await containerCore.PatchItemStreamAsync(id, partitionKey, patchOperations, trace, requestOptions, cancellationToken);
      return containerCore.ClientContext.ResponseFactory.CreateItemResponse<T>(responseMessage);
    }

    public Task<ResponseMessage> PatchItemStreamAsync(
      string id,
      PartitionKey partitionKey,
      IReadOnlyList<PatchOperation> patchOperations,
      ITrace trace,
      PatchItemRequestOptions requestOptions = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      if (trace == null)
        throw new ArgumentNullException(nameof (trace));
      if (string.IsNullOrWhiteSpace(id))
        throw new ArgumentNullException(nameof (id));
      if (patchOperations == null || !patchOperations.Any<PatchOperation>())
        throw new ArgumentNullException(nameof (patchOperations));
      if (trace == null)
        throw new ArgumentNullException(nameof (trace));
      Stream stream;
      using (trace.StartChild("Patch Operations Serialize"))
        stream = this.ClientContext.SerializerCore.ToStream<PatchSpec>(new PatchSpec(patchOperations, (Either<PatchItemRequestOptions, TransactionalBatchPatchItemRequestOptions>) requestOptions));
      return this.ClientContext.ProcessResourceOperationStreamAsync(this.GetResourceUri((RequestOptions) requestOptions, Microsoft.Azure.Documents.OperationType.Patch, id), ResourceType.Document, Microsoft.Azure.Documents.OperationType.Patch, (RequestOptions) requestOptions, (ContainerInternal) this, new PartitionKey?(partitionKey), id, stream, (Action<RequestMessage>) null, trace, cancellationToken);
    }

    public Task<ResponseMessage> PatchItemStreamAsync(
      string id,
      PartitionKey partitionKey,
      Stream streamPayload,
      ITrace trace,
      ItemRequestOptions requestOptions = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      if (trace == null)
        throw new ArgumentNullException(nameof (trace));
      if (id == null)
        throw new ArgumentNullException(nameof (id));
      if (streamPayload == null)
        throw new ArgumentNullException(nameof (streamPayload));
      if (trace == null)
        throw new ArgumentNullException(nameof (trace));
      return this.ProcessItemStreamAsync(new PartitionKey?(partitionKey), id, streamPayload, Microsoft.Azure.Documents.OperationType.Patch, requestOptions, trace, cancellationToken);
    }

    private ChangeFeedProcessorBuilder GetChangeFeedProcessorBuilderPrivate(
      string processorName,
      ChangeFeedObserverFactory observerFactory)
    {
      ChangeFeedProcessorCore feedProcessorCore = new ChangeFeedProcessorCore(observerFactory);
      return new ChangeFeedProcessorBuilder(processorName, (ContainerInternal) this, (ChangeFeedProcessor) feedProcessorCore, new Action<DocumentServiceLeaseStoreManager, ContainerInternal, string, ChangeFeedLeaseOptions, ChangeFeedProcessorOptions, ContainerInternal>(feedProcessorCore.ApplyBuildConfiguration));
    }
  }
}
