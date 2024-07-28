// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.ContainerInlineCore
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.ChangeFeed;
using Microsoft.Azure.Cosmos.Query.Core.Monads;
using Microsoft.Azure.Cosmos.Query.Core.QueryClient;
using Microsoft.Azure.Cosmos.ReadFeed;
using Microsoft.Azure.Cosmos.Telemetry;
using Microsoft.Azure.Cosmos.Tracing;
using Microsoft.Azure.Documents;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Cosmos
{
  internal sealed class ContainerInlineCore : ContainerCore
  {
    internal ContainerInlineCore(
      CosmosClientContext clientContext,
      DatabaseInternal database,
      string containerId,
      CosmosQueryClient cosmosQueryClient = null)
      : base(clientContext, database, containerId, cosmosQueryClient)
    {
    }

    public override Task<ContainerResponse> ReadContainerAsync(
      ContainerRequestOptions requestOptions = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.ClientContext.OperationHelperAsync<ContainerResponse>(nameof (ReadContainerAsync), (RequestOptions) requestOptions, (Func<ITrace, Task<ContainerResponse>>) (trace => this.ReadContainerAsync(trace, requestOptions, cancellationToken)), (Func<ContainerResponse, OpenTelemetryAttributes>) (response => (OpenTelemetryAttributes) new OpenTelemetryResponse<ContainerProperties>((Response<ContainerProperties>) response)));
    }

    public override Task<ResponseMessage> ReadContainerStreamAsync(
      ContainerRequestOptions requestOptions = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.ClientContext.OperationHelperAsync<ResponseMessage>(nameof (ReadContainerStreamAsync), (RequestOptions) requestOptions, (Func<ITrace, Task<ResponseMessage>>) (trace => this.ReadContainerStreamAsync(trace, (RequestOptions) requestOptions, cancellationToken)), (Func<ResponseMessage, OpenTelemetryAttributes>) (response => (OpenTelemetryAttributes) new OpenTelemetryResponse(response)));
    }

    public override Task<ContainerResponse> ReplaceContainerAsync(
      ContainerProperties containerProperties,
      ContainerRequestOptions requestOptions = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.ClientContext.OperationHelperAsync<ContainerResponse>(nameof (ReplaceContainerAsync), (RequestOptions) requestOptions, (Func<ITrace, Task<ContainerResponse>>) (trace => this.ReplaceContainerAsync(containerProperties, trace, requestOptions, cancellationToken)), (Func<ContainerResponse, OpenTelemetryAttributes>) (response => (OpenTelemetryAttributes) new OpenTelemetryResponse<ContainerProperties>((Response<ContainerProperties>) response)));
    }

    public override Task<ResponseMessage> ReplaceContainerStreamAsync(
      ContainerProperties containerProperties,
      ContainerRequestOptions requestOptions = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.ClientContext.OperationHelperAsync<ResponseMessage>(nameof (ReplaceContainerStreamAsync), (RequestOptions) requestOptions, (Func<ITrace, Task<ResponseMessage>>) (trace => this.ReplaceContainerStreamAsync(containerProperties, trace, requestOptions, cancellationToken)), (Func<ResponseMessage, OpenTelemetryAttributes>) (response => (OpenTelemetryAttributes) new OpenTelemetryResponse(response)));
    }

    public override Task<ContainerResponse> DeleteContainerAsync(
      ContainerRequestOptions requestOptions = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.ClientContext.OperationHelperAsync<ContainerResponse>(nameof (DeleteContainerAsync), (RequestOptions) requestOptions, (Func<ITrace, Task<ContainerResponse>>) (trace => this.DeleteContainerAsync(trace, requestOptions, cancellationToken)), (Func<ContainerResponse, OpenTelemetryAttributes>) (response => (OpenTelemetryAttributes) new OpenTelemetryResponse<ContainerProperties>((Response<ContainerProperties>) response)));
    }

    public override Task<ResponseMessage> DeleteContainerStreamAsync(
      ContainerRequestOptions requestOptions = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.ClientContext.OperationHelperAsync<ResponseMessage>(nameof (DeleteContainerStreamAsync), (RequestOptions) requestOptions, (Func<ITrace, Task<ResponseMessage>>) (trace => this.DeleteContainerStreamAsync(trace, requestOptions, cancellationToken)), (Func<ResponseMessage, OpenTelemetryAttributes>) (response => (OpenTelemetryAttributes) new OpenTelemetryResponse(response)));
    }

    public override Task<int?> ReadThroughputAsync(CancellationToken cancellationToken = default (CancellationToken)) => this.ClientContext.OperationHelperAsync<int?>(nameof (ReadThroughputAsync), (RequestOptions) null, (Func<ITrace, Task<int?>>) (trace => this.ReadThroughputAsync(trace, cancellationToken)));

    public override Task<ThroughputResponse> ReadThroughputAsync(
      RequestOptions requestOptions,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.ClientContext.OperationHelperAsync<ThroughputResponse>(nameof (ReadThroughputAsync), requestOptions, (Func<ITrace, Task<ThroughputResponse>>) (trace => this.ReadThroughputAsync(requestOptions, trace, cancellationToken)), (Func<ThroughputResponse, OpenTelemetryAttributes>) (response => (OpenTelemetryAttributes) new OpenTelemetryResponse<ThroughputProperties>((Response<ThroughputProperties>) response)));
    }

    public override Task<ThroughputResponse> ReplaceThroughputAsync(
      int throughput,
      RequestOptions requestOptions = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.ClientContext.OperationHelperAsync<ThroughputResponse>(nameof (ReplaceThroughputAsync), requestOptions, (Func<ITrace, Task<ThroughputResponse>>) (trace => this.ReplaceThroughputAsync(throughput, trace, requestOptions, cancellationToken)), (Func<ThroughputResponse, OpenTelemetryAttributes>) (response => (OpenTelemetryAttributes) new OpenTelemetryResponse<ThroughputProperties>((Response<ThroughputProperties>) response)));
    }

    public override Task<ThroughputResponse> ReplaceThroughputAsync(
      ThroughputProperties throughputProperties,
      RequestOptions requestOptions = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.ClientContext.OperationHelperAsync<ThroughputResponse>(nameof (ReplaceThroughputAsync), requestOptions, (Func<ITrace, Task<ThroughputResponse>>) (trace => this.ReplaceThroughputAsync(throughputProperties, trace, requestOptions, cancellationToken)), (Func<ThroughputResponse, OpenTelemetryAttributes>) (response => (OpenTelemetryAttributes) new OpenTelemetryResponse<ThroughputProperties>((Response<ThroughputProperties>) response)));
    }

    public override Task<ThroughputResponse> ReadThroughputIfExistsAsync(
      RequestOptions requestOptions,
      CancellationToken cancellationToken)
    {
      return this.ClientContext.OperationHelperAsync<ThroughputResponse>(nameof (ReadThroughputIfExistsAsync), requestOptions, (Func<ITrace, Task<ThroughputResponse>>) (trace => this.ReadThroughputIfExistsAsync(requestOptions, trace, cancellationToken)), (Func<ThroughputResponse, OpenTelemetryAttributes>) (response => (OpenTelemetryAttributes) new OpenTelemetryResponse<ThroughputProperties>((Response<ThroughputProperties>) response)));
    }

    public override Task<ThroughputResponse> ReplaceThroughputIfExistsAsync(
      ThroughputProperties throughput,
      RequestOptions requestOptions,
      CancellationToken cancellationToken)
    {
      return this.ClientContext.OperationHelperAsync<ThroughputResponse>(nameof (ReplaceThroughputIfExistsAsync), requestOptions, (Func<ITrace, Task<ThroughputResponse>>) (trace => this.ReplaceThroughputIfExistsAsync(throughput, trace, requestOptions, cancellationToken)), (Func<ThroughputResponse, OpenTelemetryAttributes>) (response => (OpenTelemetryAttributes) new OpenTelemetryResponse<ThroughputProperties>((Response<ThroughputProperties>) response)));
    }

    public override Task<ResponseMessage> CreateItemStreamAsync(
      Stream streamPayload,
      PartitionKey partitionKey,
      ItemRequestOptions requestOptions = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.ClientContext.OperationHelperAsync<ResponseMessage>(nameof (CreateItemStreamAsync), (RequestOptions) requestOptions, new Func<ITrace, Task<ResponseMessage>>(func), (Func<ResponseMessage, OpenTelemetryAttributes>) (response => (OpenTelemetryAttributes) new OpenTelemetryResponse(response)));

      Task<ResponseMessage> func(ITrace trace) => this.CreateItemStreamAsync(streamPayload, partitionKey, trace, requestOptions, cancellationToken);
    }

    public override Task<ItemResponse<T>> CreateItemAsync<T>(
      T item,
      PartitionKey? partitionKey = null,
      ItemRequestOptions requestOptions = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.ClientContext.OperationHelperAsync<ItemResponse<T>>(nameof (CreateItemAsync), (RequestOptions) requestOptions, (Func<ITrace, Task<ItemResponse<T>>>) (trace => this.CreateItemAsync<T>(item, trace, partitionKey, requestOptions, cancellationToken)), (Func<ItemResponse<T>, OpenTelemetryAttributes>) (response => (OpenTelemetryAttributes) new OpenTelemetryResponse<T>((Response<T>) response)));
    }

    public override Task<ResponseMessage> ReadItemStreamAsync(
      string id,
      PartitionKey partitionKey,
      ItemRequestOptions requestOptions = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.ClientContext.OperationHelperAsync<ResponseMessage>(nameof (ReadItemStreamAsync), (RequestOptions) requestOptions, (Func<ITrace, Task<ResponseMessage>>) (trace => this.ReadItemStreamAsync(id, partitionKey, trace, requestOptions, cancellationToken)), (Func<ResponseMessage, OpenTelemetryAttributes>) (response => (OpenTelemetryAttributes) new OpenTelemetryResponse(response)));
    }

    public override Task<ItemResponse<T>> ReadItemAsync<T>(
      string id,
      PartitionKey partitionKey,
      ItemRequestOptions requestOptions = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.ClientContext.OperationHelperAsync<ItemResponse<T>>(nameof (ReadItemAsync), (RequestOptions) requestOptions, (Func<ITrace, Task<ItemResponse<T>>>) (trace => this.ReadItemAsync<T>(id, partitionKey, trace, requestOptions, cancellationToken)), (Func<ItemResponse<T>, OpenTelemetryAttributes>) (response => (OpenTelemetryAttributes) new OpenTelemetryResponse<T>((Response<T>) response)));
    }

    public override Task<ResponseMessage> UpsertItemStreamAsync(
      Stream streamPayload,
      PartitionKey partitionKey,
      ItemRequestOptions requestOptions = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.ClientContext.OperationHelperAsync<ResponseMessage>(nameof (UpsertItemStreamAsync), (RequestOptions) requestOptions, (Func<ITrace, Task<ResponseMessage>>) (trace => this.UpsertItemStreamAsync(streamPayload, partitionKey, trace, requestOptions, cancellationToken)), (Func<ResponseMessage, OpenTelemetryAttributes>) (response => (OpenTelemetryAttributes) new OpenTelemetryResponse(response)));
    }

    public override Task<ItemResponse<T>> UpsertItemAsync<T>(
      T item,
      PartitionKey? partitionKey = null,
      ItemRequestOptions requestOptions = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.ClientContext.OperationHelperAsync<ItemResponse<T>>(nameof (UpsertItemAsync), (RequestOptions) requestOptions, (Func<ITrace, Task<ItemResponse<T>>>) (trace => this.UpsertItemAsync<T>(item, trace, partitionKey, requestOptions, cancellationToken)), (Func<ItemResponse<T>, OpenTelemetryAttributes>) (response => (OpenTelemetryAttributes) new OpenTelemetryResponse<T>((Response<T>) response)));
    }

    public override Task<ResponseMessage> ReplaceItemStreamAsync(
      Stream streamPayload,
      string id,
      PartitionKey partitionKey,
      ItemRequestOptions requestOptions = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.ClientContext.OperationHelperAsync<ResponseMessage>(nameof (ReplaceItemStreamAsync), (RequestOptions) requestOptions, (Func<ITrace, Task<ResponseMessage>>) (trace => this.ReplaceItemStreamAsync(streamPayload, id, partitionKey, trace, requestOptions, cancellationToken)), (Func<ResponseMessage, OpenTelemetryAttributes>) (response => (OpenTelemetryAttributes) new OpenTelemetryResponse(response)));
    }

    public override Task<ItemResponse<T>> ReplaceItemAsync<T>(
      T item,
      string id,
      PartitionKey? partitionKey = null,
      ItemRequestOptions requestOptions = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.ClientContext.OperationHelperAsync<ItemResponse<T>>(nameof (ReplaceItemAsync), (RequestOptions) requestOptions, (Func<ITrace, Task<ItemResponse<T>>>) (trace => this.ReplaceItemAsync<T>(item, id, trace, partitionKey, requestOptions, cancellationToken)), (Func<ItemResponse<T>, OpenTelemetryAttributes>) (response => (OpenTelemetryAttributes) new OpenTelemetryResponse<T>((Response<T>) response)));
    }

    public override Task<ResponseMessage> DeleteItemStreamAsync(
      string id,
      PartitionKey partitionKey,
      ItemRequestOptions requestOptions = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.ClientContext.OperationHelperAsync<ResponseMessage>(nameof (DeleteItemStreamAsync), (RequestOptions) requestOptions, (Func<ITrace, Task<ResponseMessage>>) (trace => this.DeleteItemStreamAsync(id, partitionKey, trace, requestOptions, cancellationToken)), (Func<ResponseMessage, OpenTelemetryAttributes>) (response => (OpenTelemetryAttributes) new OpenTelemetryResponse(response)));
    }

    public override Task<ItemResponse<T>> DeleteItemAsync<T>(
      string id,
      PartitionKey partitionKey,
      ItemRequestOptions requestOptions = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.ClientContext.OperationHelperAsync<ItemResponse<T>>(nameof (DeleteItemAsync), (RequestOptions) requestOptions, (Func<ITrace, Task<ItemResponse<T>>>) (trace => this.DeleteItemAsync<T>(id, partitionKey, trace, requestOptions, cancellationToken)), (Func<ItemResponse<T>, OpenTelemetryAttributes>) (response => (OpenTelemetryAttributes) new OpenTelemetryResponse<T>((Response<T>) response)));
    }

    public override Task<ResponseMessage> PatchItemStreamAsync(
      string id,
      PartitionKey partitionKey,
      IReadOnlyList<PatchOperation> patchOperations,
      PatchItemRequestOptions requestOptions = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.ClientContext.OperationHelperAsync<ResponseMessage>(nameof (PatchItemStreamAsync), (RequestOptions) requestOptions, (Func<ITrace, Task<ResponseMessage>>) (trace => this.PatchItemStreamAsync(id, partitionKey, patchOperations, trace, requestOptions, cancellationToken)), (Func<ResponseMessage, OpenTelemetryAttributes>) (response => (OpenTelemetryAttributes) new OpenTelemetryResponse(response)));
    }

    public override Task<ResponseMessage> PatchItemStreamAsync(
      string id,
      PartitionKey partitionKey,
      Stream streamPayload,
      ItemRequestOptions requestOptions = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.ClientContext.OperationHelperAsync<ResponseMessage>(nameof (PatchItemStreamAsync), (RequestOptions) requestOptions, (Func<ITrace, Task<ResponseMessage>>) (trace => this.PatchItemStreamAsync(id, partitionKey, streamPayload, trace, requestOptions, cancellationToken)), (Func<ResponseMessage, OpenTelemetryAttributes>) (response => (OpenTelemetryAttributes) new OpenTelemetryResponse(response)));
    }

    public override Task<ItemResponse<T>> PatchItemAsync<T>(
      string id,
      PartitionKey partitionKey,
      IReadOnlyList<PatchOperation> patchOperations,
      PatchItemRequestOptions requestOptions = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.ClientContext.OperationHelperAsync<ItemResponse<T>>(nameof (PatchItemAsync), (RequestOptions) requestOptions, (Func<ITrace, Task<ItemResponse<T>>>) (trace => this.PatchItemAsync<T>(id, partitionKey, patchOperations, trace, requestOptions, cancellationToken)), (Func<ItemResponse<T>, OpenTelemetryAttributes>) (response => (OpenTelemetryAttributes) new OpenTelemetryResponse<T>((Response<T>) response)));
    }

    public override Task<ResponseMessage> ReadManyItemsStreamAsync(
      IReadOnlyList<(string id, PartitionKey partitionKey)> items,
      ReadManyRequestOptions readManyRequestOptions = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.ClientContext.OperationHelperAsync<ResponseMessage>(nameof (ReadManyItemsStreamAsync), (RequestOptions) null, (Func<ITrace, Task<ResponseMessage>>) (trace => this.ReadManyItemsStreamAsync(items, trace, readManyRequestOptions, cancellationToken)), (Func<ResponseMessage, OpenTelemetryAttributes>) (response => (OpenTelemetryAttributes) new OpenTelemetryResponse(response, this.Id, this.Database?.Id)));
    }

    public override Task<FeedResponse<T>> ReadManyItemsAsync<T>(
      IReadOnlyList<(string id, PartitionKey partitionKey)> items,
      ReadManyRequestOptions readManyRequestOptions = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.ClientContext.OperationHelperAsync<FeedResponse<T>>(nameof (ReadManyItemsAsync), (RequestOptions) null, (Func<ITrace, Task<FeedResponse<T>>>) (trace => this.ReadManyItemsAsync<T>(items, trace, readManyRequestOptions, cancellationToken)), (Func<FeedResponse<T>, OpenTelemetryAttributes>) (response => (OpenTelemetryAttributes) new OpenTelemetryResponse<T>(response, this.Id, this.Database?.Id)));
    }

    public override FeedIterator GetItemQueryStreamIterator(
      QueryDefinition queryDefinition,
      string continuationToken = null,
      QueryRequestOptions requestOptions = null)
    {
      return (FeedIterator) new FeedIteratorInlineCore(base.GetItemQueryStreamIterator(queryDefinition, continuationToken, requestOptions), this.ClientContext);
    }

    public override FeedIterator<T> GetItemQueryIterator<T>(
      QueryDefinition queryDefinition,
      string continuationToken = null,
      QueryRequestOptions requestOptions = null)
    {
      return (FeedIterator<T>) new FeedIteratorInlineCore<T>(base.GetItemQueryIterator<T>(queryDefinition, continuationToken, requestOptions), this.ClientContext);
    }

    public override FeedIterator GetItemQueryStreamIterator(
      string queryText = null,
      string continuationToken = null,
      QueryRequestOptions requestOptions = null)
    {
      return (FeedIterator) new FeedIteratorInlineCore(base.GetItemQueryStreamIterator(queryText, continuationToken, requestOptions), this.ClientContext);
    }

    public override FeedIterator<T> GetItemQueryIterator<T>(
      string queryText = null,
      string continuationToken = null,
      QueryRequestOptions requestOptions = null)
    {
      return (FeedIterator<T>) new FeedIteratorInlineCore<T>(base.GetItemQueryIterator<T>(queryText, continuationToken, requestOptions), this.ClientContext);
    }

    public override IOrderedQueryable<T> GetItemLinqQueryable<T>(
      bool allowSynchronousQueryExecution = false,
      string continuationToken = null,
      QueryRequestOptions requestOptions = null,
      CosmosLinqSerializerOptions linqSerializerOptions = null)
    {
      return base.GetItemLinqQueryable<T>(allowSynchronousQueryExecution, continuationToken, requestOptions, linqSerializerOptions);
    }

    public override ChangeFeedProcessorBuilder GetChangeFeedProcessorBuilder<T>(
      string processorName,
      Container.ChangesHandler<T> onChangesDelegate)
    {
      return base.GetChangeFeedProcessorBuilder<T>(processorName, onChangesDelegate);
    }

    public override ChangeFeedProcessorBuilder GetChangeFeedProcessorBuilder<T>(
      string processorName,
      Container.ChangeFeedHandler<T> onChangesDelegate)
    {
      return base.GetChangeFeedProcessorBuilder<T>(processorName, onChangesDelegate);
    }

    public override ChangeFeedProcessorBuilder GetChangeFeedProcessorBuilderWithManualCheckpoint<T>(
      string processorName,
      Container.ChangeFeedHandlerWithManualCheckpoint<T> onChangesDelegate)
    {
      return base.GetChangeFeedProcessorBuilderWithManualCheckpoint<T>(processorName, onChangesDelegate);
    }

    public override ChangeFeedProcessorBuilder GetChangeFeedProcessorBuilder(
      string processorName,
      Container.ChangeFeedStreamHandler onChangesDelegate)
    {
      return base.GetChangeFeedProcessorBuilder(processorName, onChangesDelegate);
    }

    public override ChangeFeedProcessorBuilder GetChangeFeedProcessorBuilderWithManualCheckpoint(
      string processorName,
      Container.ChangeFeedStreamHandlerWithManualCheckpoint onChangesDelegate)
    {
      return base.GetChangeFeedProcessorBuilderWithManualCheckpoint(processorName, onChangesDelegate);
    }

    public override ChangeFeedProcessorBuilder GetChangeFeedEstimatorBuilder(
      string processorName,
      Container.ChangesEstimationHandler estimationDelegate,
      TimeSpan? estimationPeriod = null)
    {
      return base.GetChangeFeedEstimatorBuilder(processorName, estimationDelegate, estimationPeriod);
    }

    public override ChangeFeedEstimator GetChangeFeedEstimator(
      string processorName,
      Container leaseContainer)
    {
      return base.GetChangeFeedEstimator(processorName, leaseContainer);
    }

    public override TransactionalBatch CreateTransactionalBatch(PartitionKey partitionKey) => base.CreateTransactionalBatch(partitionKey);

    public override Task<IReadOnlyList<FeedRange>> GetFeedRangesAsync(
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.ClientContext.OperationHelperAsync<IReadOnlyList<FeedRange>>(nameof (GetFeedRangesAsync), (RequestOptions) null, (Func<ITrace, Task<IReadOnlyList<FeedRange>>>) (trace => this.GetFeedRangesAsync(trace, cancellationToken)));
    }

    public override FeedIterator GetChangeFeedStreamIterator(
      ChangeFeedStartFrom changeFeedStartFrom,
      ChangeFeedMode changeFeedMode,
      ChangeFeedRequestOptions changeFeedRequestOptions = null)
    {
      return base.GetChangeFeedStreamIterator(changeFeedStartFrom, changeFeedMode, changeFeedRequestOptions);
    }

    public override FeedIterator<T> GetChangeFeedIterator<T>(
      ChangeFeedStartFrom changeFeedStartFrom,
      ChangeFeedMode changeFeedMode,
      ChangeFeedRequestOptions changeFeedRequestOptions = null)
    {
      return (FeedIterator<T>) new FeedIteratorInlineCore<T>(base.GetChangeFeedIterator<T>(changeFeedStartFrom, changeFeedMode, changeFeedRequestOptions), this.ClientContext);
    }

    public override Task<IEnumerable<string>> GetPartitionKeyRangesAsync(
      FeedRange feedRange,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.ClientContext.OperationHelperAsync<IEnumerable<string>>(nameof (GetPartitionKeyRangesAsync), (RequestOptions) null, (Func<ITrace, Task<IEnumerable<string>>>) (trace => this.GetPartitionKeyRangesAsync(feedRange, trace, cancellationToken)));
    }

    public override FeedIterator GetItemQueryStreamIterator(
      FeedRange feedRange,
      QueryDefinition queryDefinition,
      string continuationToken = null,
      QueryRequestOptions requestOptions = null)
    {
      return (FeedIterator) new FeedIteratorInlineCore(base.GetItemQueryStreamIterator(feedRange, queryDefinition, continuationToken, requestOptions), this.ClientContext);
    }

    public override FeedIterator<T> GetItemQueryIterator<T>(
      FeedRange feedRange,
      QueryDefinition queryDefinition,
      string continuationToken = null,
      QueryRequestOptions requestOptions = null)
    {
      return (FeedIterator<T>) new FeedIteratorInlineCore<T>(base.GetItemQueryIterator<T>(feedRange, queryDefinition, continuationToken, requestOptions), this.ClientContext);
    }

    public override FeedIteratorInternal GetReadFeedIterator(
      QueryDefinition queryDefinition,
      QueryRequestOptions queryRequestOptions,
      string resourceLink,
      ResourceType resourceType,
      string continuationToken,
      int pageSize)
    {
      return base.GetReadFeedIterator(queryDefinition, queryRequestOptions, resourceLink, resourceType, continuationToken, pageSize);
    }

    public override IAsyncEnumerable<TryCatch<ChangeFeedPage>> GetChangeFeedAsyncEnumerable(
      ChangeFeedCrossFeedRangeState state,
      ChangeFeedMode changeFeedMode,
      ChangeFeedRequestOptions changeFeedRequestOptions = null)
    {
      return base.GetChangeFeedAsyncEnumerable(state, changeFeedMode, changeFeedRequestOptions);
    }

    public override IAsyncEnumerable<TryCatch<ReadFeedPage>> GetReadFeedAsyncEnumerable(
      ReadFeedCrossFeedRangeState state,
      QueryRequestOptions requestOptions = null)
    {
      return base.GetReadFeedAsyncEnumerable(state, requestOptions);
    }

    public override Task<ResponseMessage> DeleteAllItemsByPartitionKeyStreamAsync(
      PartitionKey partitionKey,
      RequestOptions requestOptions = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.ClientContext.OperationHelperAsync<ResponseMessage>(nameof (DeleteAllItemsByPartitionKeyStreamAsync), requestOptions, (Func<ITrace, Task<ResponseMessage>>) (trace => this.DeleteAllItemsByPartitionKeyStreamAsync(partitionKey, trace, requestOptions, cancellationToken)), (Func<ResponseMessage, OpenTelemetryAttributes>) (response => (OpenTelemetryAttributes) new OpenTelemetryResponse(response)));
    }
  }
}
