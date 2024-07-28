// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Container
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Cosmos
{
  public abstract class Container
  {
    public abstract string Id { get; }

    public abstract Database Database { get; }

    public abstract Conflicts Conflicts { get; }

    public abstract Microsoft.Azure.Cosmos.Scripts.Scripts Scripts { get; }

    public abstract Task<ContainerResponse> ReadContainerAsync(
      ContainerRequestOptions requestOptions = null,
      CancellationToken cancellationToken = default (CancellationToken));

    public abstract Task<ResponseMessage> ReadContainerStreamAsync(
      ContainerRequestOptions requestOptions = null,
      CancellationToken cancellationToken = default (CancellationToken));

    public abstract Task<ContainerResponse> ReplaceContainerAsync(
      ContainerProperties containerProperties,
      ContainerRequestOptions requestOptions = null,
      CancellationToken cancellationToken = default (CancellationToken));

    public abstract Task<ResponseMessage> ReplaceContainerStreamAsync(
      ContainerProperties containerProperties,
      ContainerRequestOptions requestOptions = null,
      CancellationToken cancellationToken = default (CancellationToken));

    public abstract Task<ContainerResponse> DeleteContainerAsync(
      ContainerRequestOptions requestOptions = null,
      CancellationToken cancellationToken = default (CancellationToken));

    public abstract Task<ResponseMessage> DeleteContainerStreamAsync(
      ContainerRequestOptions requestOptions = null,
      CancellationToken cancellationToken = default (CancellationToken));

    public abstract Task<int?> ReadThroughputAsync(CancellationToken cancellationToken = default (CancellationToken));

    public abstract Task<ThroughputResponse> ReadThroughputAsync(
      RequestOptions requestOptions,
      CancellationToken cancellationToken = default (CancellationToken));

    public abstract Task<ThroughputResponse> ReplaceThroughputAsync(
      int throughput,
      RequestOptions requestOptions = null,
      CancellationToken cancellationToken = default (CancellationToken));

    public abstract Task<ThroughputResponse> ReplaceThroughputAsync(
      ThroughputProperties throughputProperties,
      RequestOptions requestOptions = null,
      CancellationToken cancellationToken = default (CancellationToken));

    public abstract Task<ResponseMessage> CreateItemStreamAsync(
      Stream streamPayload,
      PartitionKey partitionKey,
      ItemRequestOptions requestOptions = null,
      CancellationToken cancellationToken = default (CancellationToken));

    public abstract Task<ItemResponse<T>> CreateItemAsync<T>(
      T item,
      PartitionKey? partitionKey = null,
      ItemRequestOptions requestOptions = null,
      CancellationToken cancellationToken = default (CancellationToken));

    public abstract Task<ResponseMessage> ReadItemStreamAsync(
      string id,
      PartitionKey partitionKey,
      ItemRequestOptions requestOptions = null,
      CancellationToken cancellationToken = default (CancellationToken));

    public abstract Task<ItemResponse<T>> ReadItemAsync<T>(
      string id,
      PartitionKey partitionKey,
      ItemRequestOptions requestOptions = null,
      CancellationToken cancellationToken = default (CancellationToken));

    public abstract Task<ResponseMessage> UpsertItemStreamAsync(
      Stream streamPayload,
      PartitionKey partitionKey,
      ItemRequestOptions requestOptions = null,
      CancellationToken cancellationToken = default (CancellationToken));

    public abstract Task<ItemResponse<T>> UpsertItemAsync<T>(
      T item,
      PartitionKey? partitionKey = null,
      ItemRequestOptions requestOptions = null,
      CancellationToken cancellationToken = default (CancellationToken));

    public abstract Task<ResponseMessage> ReplaceItemStreamAsync(
      Stream streamPayload,
      string id,
      PartitionKey partitionKey,
      ItemRequestOptions requestOptions = null,
      CancellationToken cancellationToken = default (CancellationToken));

    public abstract Task<ItemResponse<T>> ReplaceItemAsync<T>(
      T item,
      string id,
      PartitionKey? partitionKey = null,
      ItemRequestOptions requestOptions = null,
      CancellationToken cancellationToken = default (CancellationToken));

    public abstract Task<ResponseMessage> ReadManyItemsStreamAsync(
      IReadOnlyList<(string id, PartitionKey partitionKey)> items,
      ReadManyRequestOptions readManyRequestOptions = null,
      CancellationToken cancellationToken = default (CancellationToken));

    public abstract Task<FeedResponse<T>> ReadManyItemsAsync<T>(
      IReadOnlyList<(string id, PartitionKey partitionKey)> items,
      ReadManyRequestOptions readManyRequestOptions = null,
      CancellationToken cancellationToken = default (CancellationToken));

    public abstract Task<ItemResponse<T>> PatchItemAsync<T>(
      string id,
      PartitionKey partitionKey,
      IReadOnlyList<PatchOperation> patchOperations,
      PatchItemRequestOptions requestOptions = null,
      CancellationToken cancellationToken = default (CancellationToken));

    public abstract Task<ResponseMessage> PatchItemStreamAsync(
      string id,
      PartitionKey partitionKey,
      IReadOnlyList<PatchOperation> patchOperations,
      PatchItemRequestOptions requestOptions = null,
      CancellationToken cancellationToken = default (CancellationToken));

    public abstract Task<ResponseMessage> DeleteItemStreamAsync(
      string id,
      PartitionKey partitionKey,
      ItemRequestOptions requestOptions = null,
      CancellationToken cancellationToken = default (CancellationToken));

    public abstract Task<ItemResponse<T>> DeleteItemAsync<T>(
      string id,
      PartitionKey partitionKey,
      ItemRequestOptions requestOptions = null,
      CancellationToken cancellationToken = default (CancellationToken));

    public abstract FeedIterator GetItemQueryStreamIterator(
      QueryDefinition queryDefinition,
      string continuationToken = null,
      QueryRequestOptions requestOptions = null);

    public abstract FeedIterator<T> GetItemQueryIterator<T>(
      QueryDefinition queryDefinition,
      string continuationToken = null,
      QueryRequestOptions requestOptions = null);

    public abstract FeedIterator GetItemQueryStreamIterator(
      string queryText = null,
      string continuationToken = null,
      QueryRequestOptions requestOptions = null);

    public abstract FeedIterator<T> GetItemQueryIterator<T>(
      string queryText = null,
      string continuationToken = null,
      QueryRequestOptions requestOptions = null);

    public abstract FeedIterator GetItemQueryStreamIterator(
      FeedRange feedRange,
      QueryDefinition queryDefinition,
      string continuationToken,
      QueryRequestOptions requestOptions = null);

    public abstract FeedIterator<T> GetItemQueryIterator<T>(
      FeedRange feedRange,
      QueryDefinition queryDefinition,
      string continuationToken = null,
      QueryRequestOptions requestOptions = null);

    public abstract IOrderedQueryable<T> GetItemLinqQueryable<T>(
      bool allowSynchronousQueryExecution = false,
      string continuationToken = null,
      QueryRequestOptions requestOptions = null,
      CosmosLinqSerializerOptions linqSerializerOptions = null);

    public abstract ChangeFeedProcessorBuilder GetChangeFeedProcessorBuilder<T>(
      string processorName,
      Container.ChangesHandler<T> onChangesDelegate);

    public abstract ChangeFeedProcessorBuilder GetChangeFeedEstimatorBuilder(
      string processorName,
      Container.ChangesEstimationHandler estimationDelegate,
      TimeSpan? estimationPeriod = null);

    public abstract ChangeFeedEstimator GetChangeFeedEstimator(
      string processorName,
      Container leaseContainer);

    public abstract TransactionalBatch CreateTransactionalBatch(PartitionKey partitionKey);

    public abstract Task<IReadOnlyList<FeedRange>> GetFeedRangesAsync(
      CancellationToken cancellationToken = default (CancellationToken));

    public abstract FeedIterator GetChangeFeedStreamIterator(
      ChangeFeedStartFrom changeFeedStartFrom,
      ChangeFeedMode changeFeedMode,
      ChangeFeedRequestOptions changeFeedRequestOptions = null);

    public abstract FeedIterator<T> GetChangeFeedIterator<T>(
      ChangeFeedStartFrom changeFeedStartFrom,
      ChangeFeedMode changeFeedMode,
      ChangeFeedRequestOptions changeFeedRequestOptions = null);

    public abstract ChangeFeedProcessorBuilder GetChangeFeedProcessorBuilder<T>(
      string processorName,
      Container.ChangeFeedHandler<T> onChangesDelegate);

    public abstract ChangeFeedProcessorBuilder GetChangeFeedProcessorBuilderWithManualCheckpoint<T>(
      string processorName,
      Container.ChangeFeedHandlerWithManualCheckpoint<T> onChangesDelegate);

    public abstract ChangeFeedProcessorBuilder GetChangeFeedProcessorBuilder(
      string processorName,
      Container.ChangeFeedStreamHandler onChangesDelegate);

    public abstract ChangeFeedProcessorBuilder GetChangeFeedProcessorBuilderWithManualCheckpoint(
      string processorName,
      Container.ChangeFeedStreamHandlerWithManualCheckpoint onChangesDelegate);

    public delegate Task ChangesHandler<T>(
      IReadOnlyCollection<T> changes,
      CancellationToken cancellationToken);

    public delegate Task ChangesEstimationHandler(
      long estimatedPendingChanges,
      CancellationToken cancellationToken);

    public delegate Task ChangeFeedHandler<T>(
      ChangeFeedProcessorContext context,
      IReadOnlyCollection<T> changes,
      CancellationToken cancellationToken);

    public delegate Task ChangeFeedHandlerWithManualCheckpoint<T>(
      ChangeFeedProcessorContext context,
      IReadOnlyCollection<T> changes,
      Func<Task> checkpointAsync,
      CancellationToken cancellationToken);

    public delegate Task ChangeFeedStreamHandler(
      ChangeFeedProcessorContext context,
      Stream changes,
      CancellationToken cancellationToken);

    public delegate Task ChangeFeedStreamHandlerWithManualCheckpoint(
      ChangeFeedProcessorContext context,
      Stream changes,
      Func<Task> checkpointAsync,
      CancellationToken cancellationToken);

    public delegate Task ChangeFeedMonitorErrorDelegate(string leaseToken, Exception exception);

    public delegate Task ChangeFeedMonitorLeaseAcquireDelegate(string leaseToken);

    public delegate Task ChangeFeedMonitorLeaseReleaseDelegate(string leaseToken);
  }
}
