// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.BatchAsyncContainerExecutor
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.Routing;
using Microsoft.Azure.Cosmos.Tracing;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Routing;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Cosmos
{
  internal class BatchAsyncContainerExecutor : IDisposable
  {
    private const int TimerWheelBucketCount = 20;
    private static readonly TimeSpan TimerWheelResolution = TimeSpan.FromMilliseconds(50.0);
    private readonly ContainerInternal cosmosContainer;
    private readonly CosmosClientContext cosmosClientContext;
    private readonly int maxServerRequestBodyLength;
    private readonly int maxServerRequestOperationCount;
    private readonly ConcurrentDictionary<string, BatchAsyncStreamer> streamersByPartitionKeyRange = new ConcurrentDictionary<string, BatchAsyncStreamer>();
    private readonly ConcurrentDictionary<string, SemaphoreSlim> limitersByPartitionkeyRange = new ConcurrentDictionary<string, SemaphoreSlim>();
    private readonly TimerWheel timerWheel;
    private readonly RetryOptions retryOptions;
    private readonly int defaultMaxDegreeOfConcurrency = 50;

    internal BatchAsyncContainerExecutor()
    {
    }

    public BatchAsyncContainerExecutor(
      ContainerInternal cosmosContainer,
      CosmosClientContext cosmosClientContext,
      int maxServerRequestOperationCount,
      int maxServerRequestBodyLength)
    {
      if (maxServerRequestOperationCount < 1)
        throw new ArgumentOutOfRangeException(nameof (maxServerRequestOperationCount));
      if (maxServerRequestBodyLength < 1)
        throw new ArgumentOutOfRangeException(nameof (maxServerRequestBodyLength));
      this.cosmosContainer = cosmosContainer ?? throw new ArgumentNullException(nameof (cosmosContainer));
      this.cosmosClientContext = cosmosClientContext;
      this.maxServerRequestBodyLength = maxServerRequestBodyLength;
      this.maxServerRequestOperationCount = maxServerRequestOperationCount;
      this.timerWheel = TimerWheel.CreateTimerWheel(BatchAsyncContainerExecutor.TimerWheelResolution, 20);
      this.retryOptions = cosmosClientContext.ClientOptions.GetConnectionPolicy(cosmosClientContext.Client.ClientId).RetryOptions;
    }

    public virtual async Task<TransactionalBatchOperationResult> AddAsync(
      ItemBatchOperation operation,
      ITrace trace,
      ItemRequestOptions itemRequestOptions = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      if (operation == null)
        throw new ArgumentNullException(nameof (operation));
      await this.ValidateOperationAsync(operation, itemRequestOptions, cancellationToken);
      string partitionKeyRangeId = await this.ResolvePartitionKeyRangeIdAsync(operation, trace, cancellationToken).ConfigureAwait(false);
      BatchAsyncStreamer partitionKeyRange = this.GetOrAddStreamerForPartitionKeyRange(partitionKeyRangeId);
      ItemBatchOperationContext context = new ItemBatchOperationContext(partitionKeyRangeId, trace, BatchAsyncContainerExecutor.GetRetryPolicy(this.cosmosContainer, operation.OperationType, this.retryOptions));
      if (itemRequestOptions != null && itemRequestOptions.AddRequestHeaders != null)
      {
        Headers headers = new Headers();
        Action<Headers> addRequestHeaders = itemRequestOptions.AddRequestHeaders;
        if (addRequestHeaders != null)
          addRequestHeaders(headers);
        string str1;
        if (headers.TryGetValue("x-ms-cosmos-is-client-encrypted", out str1))
        {
          context.IsClientEncrypted = bool.Parse(str1);
          string str2;
          if (context.IsClientEncrypted && headers.TryGetValue("x-ms-cosmos-intended-collection-rid", out str2))
            context.IntendedCollectionRidValue = str2;
        }
      }
      operation.AttachContext(context);
      ItemBatchOperation operation1 = operation;
      partitionKeyRange.Add(operation1);
      return await context.OperationTask;
    }

    public void Dispose()
    {
      foreach (KeyValuePair<string, BatchAsyncStreamer> keyValuePair in this.streamersByPartitionKeyRange)
        keyValuePair.Value.Dispose();
      foreach (KeyValuePair<string, SemaphoreSlim> keyValuePair in this.limitersByPartitionkeyRange)
        keyValuePair.Value.Dispose();
      this.timerWheel.Dispose();
    }

    internal virtual async Task ValidateOperationAsync(
      ItemBatchOperation operation,
      ItemRequestOptions itemRequestOptions = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      if (itemRequestOptions != null)
      {
        if (!itemRequestOptions.BaseConsistencyLevel.HasValue && itemRequestOptions.PreTriggers == null && itemRequestOptions.PostTriggers == null && itemRequestOptions.SessionToken == null && itemRequestOptions.Properties == null)
        {
          DedicatedGatewayRequestOptions gatewayRequestOptions = itemRequestOptions.DedicatedGatewayRequestOptions;
          if ((gatewayRequestOptions != null ? (gatewayRequestOptions.MaxIntegratedCacheStaleness.HasValue ? 1 : 0) : 0) == 0)
            goto label_4;
        }
        throw new InvalidOperationException(ClientResources.UnsupportedBulkRequestOptions);
      }
label_4:
      await operation.MaterializeResourceAsync(this.cosmosClientContext.SerializerCore, cancellationToken);
    }

    private static IDocumentClientRetryPolicy GetRetryPolicy(
      ContainerInternal containerInternal,
      Microsoft.Azure.Documents.OperationType operationType,
      RetryOptions retryOptions)
    {
      return (IDocumentClientRetryPolicy) new BulkExecutionRetryPolicy(containerInternal, operationType, (IDocumentClientRetryPolicy) new ResourceThrottleRetryPolicy(retryOptions.MaxRetryAttemptsOnThrottledRequests, retryOptions.MaxRetryWaitTimeInSeconds));
    }

    private static bool ValidateOperationEPK(
      ItemBatchOperation operation,
      ItemRequestOptions itemRequestOptions)
    {
      object obj1;
      object obj2;
      object obj3;
      if (itemRequestOptions.Properties != null && itemRequestOptions.Properties.TryGetValue("x-ms-effective-partition-key", out obj1) | itemRequestOptions.Properties.TryGetValue("x-ms-effective-partition-key-string", out obj2) | itemRequestOptions.Properties.TryGetValue("x-ms-documentdb-partitionkey", out obj3))
      {
        byte[] numArray = obj1 as byte[];
        string str = obj3 as string;
        if (numArray == null && str == null || !(obj2 is string))
          throw new InvalidOperationException(string.Format(ClientResources.EpkPropertiesPairingExpected, (object) "x-ms-effective-partition-key", (object) "x-ms-effective-partition-key-string"));
        if (operation.PartitionKey.HasValue)
          throw new InvalidOperationException(ClientResources.PKAndEpkSetTogether);
      }
      return true;
    }

    private static void AddHeadersToRequestMessage(
      RequestMessage requestMessage,
      PartitionKeyRangeServerBatchRequest partitionKeyRangeServerBatchRequest)
    {
      requestMessage.Headers.PartitionKeyRangeId = partitionKeyRangeServerBatchRequest.PartitionKeyRangeId;
      if (partitionKeyRangeServerBatchRequest.IsClientEncrypted)
      {
        requestMessage.Headers.Add("x-ms-cosmos-is-client-encrypted", partitionKeyRangeServerBatchRequest.IsClientEncrypted.ToString());
        requestMessage.Headers.Add("x-ms-cosmos-intended-collection-rid", partitionKeyRangeServerBatchRequest.IntendedCollectionRidValue);
      }
      requestMessage.Headers.Add("x-ms-cosmos-batch-continue-on-error", bool.TrueString);
      requestMessage.Headers.Add("x-ms-cosmos-batch-atomic", bool.FalseString);
      requestMessage.Headers.Add("x-ms-cosmos-is-batch-request", bool.TrueString);
    }

    private async Task ReBatchAsync(
      ItemBatchOperation operation,
      CancellationToken cancellationToken)
    {
      using (ITrace trace = (ITrace) Trace.GetRootTrace("Batch Retry Async", TraceComponent.Batch, TraceLevel.Verbose))
      {
        string str = await this.ResolvePartitionKeyRangeIdAsync(operation, trace, cancellationToken).ConfigureAwait(false);
        operation.Context.ReRouteOperation(str, trace);
        this.GetOrAddStreamerForPartitionKeyRange(str).Add(operation);
      }
    }

    private async Task<string> ResolvePartitionKeyRangeIdAsync(
      ItemBatchOperation operation,
      ITrace trace,
      CancellationToken cancellationToken)
    {
      cancellationToken.ThrowIfCancellationRequested();
      PartitionKeyDefinition partitionKeyDefinition = (await this.cosmosContainer.GetCachedContainerPropertiesAsync(false, trace, cancellationToken))?.PartitionKey;
      CollectionRoutingMap collectionRoutingMap = await this.cosmosContainer.GetRoutingMapAsync(cancellationToken);
      PartitionKeyInternal keyInternalAsync = await this.GetPartitionKeyInternalAsync(operation, cancellationToken);
      operation.PartitionKeyJson = keyInternalAsync.ToJsonString();
      string id = collectionRoutingMap.GetRangeByEffectivePartitionKey(keyInternalAsync.GetEffectivePartitionKeyString(partitionKeyDefinition)).Id;
      partitionKeyDefinition = (PartitionKeyDefinition) null;
      collectionRoutingMap = (CollectionRoutingMap) null;
      return id;
    }

    private async Task<PartitionKeyInternal> GetPartitionKeyInternalAsync(
      ItemBatchOperation operation,
      CancellationToken cancellationToken)
    {
      PartitionKey partitionKey = operation.PartitionKey.Value;
      if (partitionKey.IsNone)
        return await this.cosmosContainer.GetNonePartitionKeyValueAsync((ITrace) NoOpTrace.Singleton, cancellationToken).ConfigureAwait(false);
      partitionKey = operation.PartitionKey.Value;
      return partitionKey.InternalKey;
    }

    private async Task<PartitionKeyRangeBatchExecutionResult> ExecuteAsync(
      PartitionKeyRangeServerBatchRequest serverRequest,
      ITrace trace,
      CancellationToken cancellationToken)
    {
      PartitionKeyRangeBatchExecutionResult batchExecutionResult;
      using (await this.GetOrAddLimiterForPartitionKeyRange(serverRequest.PartitionKeyRangeId).UsingWaitAsync(trace, cancellationToken))
      {
        using (Stream serverRequestPayload = (Stream) serverRequest.TransferBodyStream())
          batchExecutionResult = new PartitionKeyRangeBatchExecutionResult(serverRequest.PartitionKeyRangeId, (IEnumerable<ItemBatchOperation>) serverRequest.Operations, await TransactionalBatchResponse.FromResponseMessageAsync(await this.cosmosClientContext.ProcessResourceOperationStreamAsync(this.cosmosContainer.LinkUri, ResourceType.Document, Microsoft.Azure.Documents.OperationType.Batch, new RequestOptions(), this.cosmosContainer, (FeedRange) null, serverRequestPayload, (Action<RequestMessage>) (requestMessage => BatchAsyncContainerExecutor.AddHeadersToRequestMessage(requestMessage, serverRequest)), trace, cancellationToken).ConfigureAwait(false), (ServerBatchRequest) serverRequest, this.cosmosClientContext.SerializerCore, true, trace, cancellationToken).ConfigureAwait(false));
      }
      return batchExecutionResult;
    }

    private BatchAsyncStreamer GetOrAddStreamerForPartitionKeyRange(string partitionKeyRangeId)
    {
      BatchAsyncStreamer partitionKeyRange;
      if (this.streamersByPartitionKeyRange.TryGetValue(partitionKeyRangeId, out partitionKeyRange))
        return partitionKeyRange;
      BatchAsyncStreamer batchAsyncStreamer = new BatchAsyncStreamer(this.maxServerRequestOperationCount, this.maxServerRequestBodyLength, this.timerWheel, this.GetOrAddLimiterForPartitionKeyRange(partitionKeyRangeId), this.defaultMaxDegreeOfConcurrency, this.cosmosClientContext.SerializerCore, new BatchAsyncBatcherExecuteDelegate(this.ExecuteAsync), new BatchAsyncBatcherRetryDelegate(this.ReBatchAsync), this.cosmosClientContext);
      if (!this.streamersByPartitionKeyRange.TryAdd(partitionKeyRangeId, batchAsyncStreamer))
        batchAsyncStreamer.Dispose();
      return this.streamersByPartitionKeyRange[partitionKeyRangeId];
    }

    private SemaphoreSlim GetOrAddLimiterForPartitionKeyRange(string partitionKeyRangeId)
    {
      SemaphoreSlim partitionKeyRange;
      if (this.limitersByPartitionkeyRange.TryGetValue(partitionKeyRangeId, out partitionKeyRange))
        return partitionKeyRange;
      SemaphoreSlim semaphoreSlim = new SemaphoreSlim(1, this.defaultMaxDegreeOfConcurrency);
      if (!this.limitersByPartitionkeyRange.TryAdd(partitionKeyRangeId, semaphoreSlim))
        semaphoreSlim.Dispose();
      return this.limitersByPartitionkeyRange[partitionKeyRangeId];
    }
  }
}
