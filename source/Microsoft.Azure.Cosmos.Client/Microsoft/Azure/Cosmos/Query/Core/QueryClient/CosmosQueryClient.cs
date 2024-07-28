// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Query.Core.QueryClient.CosmosQueryClient
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.Query.Core.Monads;
using Microsoft.Azure.Cosmos.Query.Core.Pipeline.Pagination;
using Microsoft.Azure.Cosmos.Tracing;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Cosmos.Query.Core.QueryClient
{
  internal abstract class CosmosQueryClient
  {
    public abstract Action<IQueryable> OnExecuteScalarQueryCallback { get; }

    public abstract Task<ContainerQueryProperties> GetCachedContainerQueryPropertiesAsync(
      string containerLink,
      Microsoft.Azure.Cosmos.PartitionKey? partitionKey,
      ITrace trace,
      CancellationToken cancellationToken);

    public abstract Task<IReadOnlyList<PartitionKeyRange>> TryGetOverlappingRangesAsync(
      string collectionResourceId,
      Range<string> range,
      bool forceRefresh = false);

    public abstract Task<TryCatch<Microsoft.Azure.Cosmos.Query.Core.QueryPlan.PartitionedQueryExecutionInfo>> TryGetPartitionedQueryExecutionInfoAsync(
      SqlQuerySpec sqlQuerySpec,
      ResourceType resourceType,
      PartitionKeyDefinition partitionKeyDefinition,
      bool requireFormattableOrderByQuery,
      bool isContinuationExpected,
      bool allowNonValueAggregateQuery,
      bool hasLogicalPartitionKey,
      bool allowDCount,
      bool useSystemPrefix,
      CancellationToken cancellationToken);

    public abstract Task<TryCatch<QueryPage>> ExecuteItemQueryAsync(
      string resourceUri,
      ResourceType resourceType,
      OperationType operationType,
      Guid clientQueryCorrelationId,
      FeedRange feedRange,
      QueryRequestOptions requestOptions,
      SqlQuerySpec sqlQuerySpec,
      string continuationToken,
      bool isContinuationExpected,
      int pageSize,
      ITrace trace,
      CancellationToken cancellationToken);

    public abstract Task<Microsoft.Azure.Cosmos.Query.Core.QueryPlan.PartitionedQueryExecutionInfo> ExecuteQueryPlanRequestAsync(
      string resourceUri,
      ResourceType resourceType,
      OperationType operationType,
      SqlQuerySpec sqlQuerySpec,
      Microsoft.Azure.Cosmos.PartitionKey? partitionKey,
      string supportedQueryFeatures,
      Guid clientQueryCorrelationId,
      ITrace trace,
      CancellationToken cancellationToken);

    public abstract void ClearSessionTokenCache(string collectionFullName);

    public abstract Task<List<PartitionKeyRange>> GetTargetPartitionKeyRangesByEpkStringAsync(
      string resourceLink,
      string collectionResourceId,
      string effectivePartitionKeyString,
      bool forceRefresh,
      ITrace trace);

    public abstract Task<List<PartitionKeyRange>> GetTargetPartitionKeyRangeByFeedRangeAsync(
      string resourceLink,
      string collectionResourceId,
      PartitionKeyDefinition partitionKeyDefinition,
      FeedRangeInternal feedRangeInternal,
      bool forceRefresh,
      ITrace trace);

    public abstract Task<List<PartitionKeyRange>> GetTargetPartitionKeyRangesAsync(
      string resourceLink,
      string collectionResourceId,
      List<Range<string>> providedRanges,
      bool forceRefresh,
      ITrace trace);

    public abstract bool ByPassQueryParsing();

    public abstract Task ForceRefreshCollectionCacheAsync(
      string collectionLink,
      CancellationToken cancellationToken);
  }
}
