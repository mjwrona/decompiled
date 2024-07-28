// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.ContainerInternal
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.ChangeFeed;
using Microsoft.Azure.Cosmos.Query;
using Microsoft.Azure.Cosmos.Query.Core;
using Microsoft.Azure.Cosmos.Query.Core.Monads;
using Microsoft.Azure.Cosmos.Query.Core.QueryPlan;
using Microsoft.Azure.Cosmos.ReadFeed;
using Microsoft.Azure.Cosmos.Routing;
using Microsoft.Azure.Cosmos.Tracing;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Routing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Cosmos
{
  internal abstract class ContainerInternal : Container
  {
    public abstract string LinkUri { get; }

    public abstract CosmosClientContext ClientContext { get; }

    public abstract BatchAsyncContainerExecutor BatchExecutor { get; }

    public abstract Task<ThroughputResponse> ReadThroughputIfExistsAsync(
      RequestOptions requestOptions,
      CancellationToken cancellationToken);

    public abstract Task<ThroughputResponse> ReplaceThroughputIfExistsAsync(
      ThroughputProperties throughput,
      RequestOptions requestOptions,
      CancellationToken cancellationToken);

    public Task<string> GetCachedRIDAsync(CancellationToken cancellationToken) => this.GetCachedRIDAsync(false, (ITrace) NoOpTrace.Singleton, cancellationToken);

    public abstract Task<string> GetCachedRIDAsync(
      bool forceRefresh,
      ITrace trace,
      CancellationToken cancellationToken);

    public abstract Task<PartitionKeyDefinition> GetPartitionKeyDefinitionAsync(
      CancellationToken cancellationToken);

    public abstract Task<ContainerProperties> GetCachedContainerPropertiesAsync(
      bool forceRefresh,
      ITrace trace,
      CancellationToken cancellationToken);

    public abstract Task<IReadOnlyList<IReadOnlyList<string>>> GetPartitionKeyPathTokensAsync(
      ITrace trace,
      CancellationToken cancellationToken = default (CancellationToken));

    public abstract Task<PartitionKeyInternal> GetNonePartitionKeyValueAsync(
      ITrace trace,
      CancellationToken cancellationToken);

    public abstract Task<CollectionRoutingMap> GetRoutingMapAsync(
      CancellationToken cancellationToken);

    public abstract Task<ContainerInternal.TryExecuteQueryResult> TryExecuteQueryAsync(
      QueryFeatures supportedQueryFeatures,
      QueryDefinition queryDefinition,
      string continuationToken,
      FeedRangeInternal feedRangeInternal,
      QueryRequestOptions requestOptions,
      CancellationToken cancellationToken = default (CancellationToken));

    public abstract FeedIterator GetStandByFeedIterator(
      string continuationToken = null,
      int? maxItemCount = null,
      StandByFeedIteratorRequestOptions requestOptions = null);

    public abstract FeedIteratorInternal GetItemQueryStreamIteratorInternal(
      SqlQuerySpec sqlQuerySpec,
      bool isContinuationExcpected,
      string continuationToken,
      FeedRangeInternal feedRange,
      QueryRequestOptions requestOptions);

    public abstract FeedIteratorInternal GetReadFeedIterator(
      QueryDefinition queryDefinition,
      QueryRequestOptions queryRequestOptions,
      string resourceLink,
      ResourceType resourceType,
      string continuationToken,
      int pageSize);

    public abstract Task<PartitionKey> GetPartitionKeyValueFromStreamAsync(
      Stream stream,
      ITrace trace,
      CancellationToken cancellation);

    public abstract IAsyncEnumerable<TryCatch<ChangeFeedPage>> GetChangeFeedAsyncEnumerable(
      ChangeFeedCrossFeedRangeState state,
      ChangeFeedMode changeFeedMode,
      ChangeFeedRequestOptions changeFeedRequestOptions = null);

    public abstract IAsyncEnumerable<TryCatch<ReadFeedPage>> GetReadFeedAsyncEnumerable(
      ReadFeedCrossFeedRangeState state,
      QueryRequestOptions requestOptions = null);

    public static void ValidatePartitionKey(object partitionKey, RequestOptions requestOptions)
    {
      if (partitionKey == null && (requestOptions == null || !requestOptions.IsEffectivePartitionKeyRouting))
        throw new ArgumentNullException(nameof (partitionKey));
    }

    public abstract Task<ResponseMessage> PatchItemStreamAsync(
      string id,
      PartitionKey partitionKey,
      Stream streamPayload,
      ItemRequestOptions requestOptions = null,
      CancellationToken cancellationToken = default (CancellationToken));

    public abstract Task<ResponseMessage> DeleteAllItemsByPartitionKeyStreamAsync(
      PartitionKey partitionKey,
      RequestOptions requestOptions = null,
      CancellationToken cancellationToken = default (CancellationToken));

    public abstract Task<IEnumerable<string>> GetPartitionKeyRangesAsync(
      FeedRange feedRange,
      CancellationToken cancellationToken = default (CancellationToken));

    public abstract FeedIterator GetChangeFeedStreamIteratorWithQuery(
      ChangeFeedStartFrom changeFeedStartFrom,
      ChangeFeedMode changeFeedMode,
      ChangeFeedQuerySpec changeFeedQuerySpec,
      ChangeFeedRequestOptions changeFeedRequestOptions = null);

    public abstract FeedIterator<T> GetChangeFeedIteratorWithQuery<T>(
      ChangeFeedStartFrom changeFeedStartFrom,
      ChangeFeedMode changeFeedMode,
      ChangeFeedQuerySpec changeFeedQuerySpec,
      ChangeFeedRequestOptions changeFeedRequestOptions = null);

    public abstract class TryExecuteQueryResult
    {
    }

    public sealed class FailedToGetQueryPlanResult : ContainerInternal.TryExecuteQueryResult
    {
      public FailedToGetQueryPlanResult(Exception exception) => this.Exception = exception ?? throw new ArgumentNullException(nameof (exception));

      public Exception Exception { get; }
    }

    public sealed class QueryPlanNotSupportedResult : ContainerInternal.TryExecuteQueryResult
    {
      public QueryPlanNotSupportedResult(
        Microsoft.Azure.Cosmos.Query.Core.QueryPlan.PartitionedQueryExecutionInfo partitionedQueryExecutionInfo)
      {
        this.QueryPlan = partitionedQueryExecutionInfo ?? throw new ArgumentNullException(nameof (partitionedQueryExecutionInfo));
      }

      public Microsoft.Azure.Cosmos.Query.Core.QueryPlan.PartitionedQueryExecutionInfo QueryPlan { get; }
    }

    public sealed class QueryPlanIsSupportedResult : ContainerInternal.TryExecuteQueryResult
    {
      public QueryPlanIsSupportedResult(QueryIterator queryIterator) => this.QueryIterator = queryIterator ?? throw new ArgumentNullException(nameof (queryIterator));

      public QueryIterator QueryIterator { get; }
    }
  }
}
