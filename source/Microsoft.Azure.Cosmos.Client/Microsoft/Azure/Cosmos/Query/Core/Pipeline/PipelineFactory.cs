// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Query.Core.Pipeline.PipelineFactory
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.CosmosElements;
using Microsoft.Azure.Cosmos.Pagination;
using Microsoft.Azure.Cosmos.Query.Core.Monads;
using Microsoft.Azure.Cosmos.Query.Core.Pipeline.Aggregate;
using Microsoft.Azure.Cosmos.Query.Core.Pipeline.CrossPartition.OrderBy;
using Microsoft.Azure.Cosmos.Query.Core.Pipeline.CrossPartition.Parallel;
using Microsoft.Azure.Cosmos.Query.Core.Pipeline.DCount;
using Microsoft.Azure.Cosmos.Query.Core.Pipeline.Distinct;
using Microsoft.Azure.Cosmos.Query.Core.Pipeline.GroupBy;
using Microsoft.Azure.Cosmos.Query.Core.Pipeline.Pagination;
using Microsoft.Azure.Cosmos.Query.Core.Pipeline.Skip;
using Microsoft.Azure.Cosmos.Query.Core.Pipeline.Take;
using Microsoft.Azure.Cosmos.Query.Core.QueryPlan;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Microsoft.Azure.Cosmos.Query.Core.Pipeline
{
  internal static class PipelineFactory
  {
    public static TryCatch<IQueryPipelineStage> MonadicCreate(
      ExecutionEnvironment executionEnvironment,
      IDocumentContainer documentContainer,
      SqlQuerySpec sqlQuerySpec,
      IReadOnlyList<FeedRangeEpk> targetRanges,
      PartitionKey? partitionKey,
      QueryInfo queryInfo,
      QueryPaginationOptions queryPaginationOptions,
      int maxConcurrency,
      CosmosElement requestContinuationToken,
      CancellationToken requestCancellationToken)
    {
      if (documentContainer == null)
        throw new ArgumentNullException(nameof (documentContainer));
      if (sqlQuerySpec == null)
        throw new ArgumentNullException(nameof (sqlQuerySpec));
      if (targetRanges == null)
        throw new ArgumentNullException(nameof (targetRanges));
      if (targetRanges.Count == 0)
        throw new ArgumentException("targetRanges must not be empty.");
      if (queryInfo == null)
        throw new ArgumentNullException(nameof (queryInfo));
      sqlQuerySpec = !string.IsNullOrEmpty(queryInfo.RewrittenQuery) ? new SqlQuerySpec(queryInfo.RewrittenQuery, sqlQuerySpec.Parameters) : sqlQuerySpec;
      PrefetchPolicy prefetchPolicy = PipelineFactory.DeterminePrefetchPolicy(queryInfo);
      MonadicCreatePipelineStage createPipelineStage = !queryInfo.HasOrderBy ? (MonadicCreatePipelineStage) ((continuationToken, cancellationToken) =>
      {
        IDocumentContainer documentContainer1 = documentContainer;
        SqlQuerySpec sqlQuerySpec1 = sqlQuerySpec;
        IReadOnlyList<FeedRangeEpk> targetRanges1 = targetRanges;
        QueryPaginationOptions paginationOptions = queryPaginationOptions;
        PartitionKey? partitionKey1 = partitionKey;
        QueryPaginationOptions queryPaginationOptions1 = paginationOptions;
        PrefetchPolicy prefetchPolicy1 = prefetchPolicy;
        int maxConcurrency1 = maxConcurrency;
        int num = (int) prefetchPolicy1;
        CosmosElement continuationToken1 = continuationToken;
        CancellationToken cancellationToken1 = cancellationToken;
        return ParallelCrossPartitionQueryPipelineStage.MonadicCreate(documentContainer1, sqlQuerySpec1, targetRanges1, partitionKey1, queryPaginationOptions1, maxConcurrency1, (PrefetchPolicy) num, continuationToken1, cancellationToken1);
      }) : (MonadicCreatePipelineStage) ((continuationToken, cancellationToken) => OrderByCrossPartitionQueryPipelineStage.MonadicCreate(documentContainer, sqlQuerySpec, targetRanges, partitionKey, (IReadOnlyList<OrderByColumn>) queryInfo.OrderByExpressions.Zip<string, SortOrder, OrderByColumn>((IEnumerable<SortOrder>) queryInfo.OrderBy, (Func<string, SortOrder, OrderByColumn>) ((expression, sortOrder) => new OrderByColumn(expression, sortOrder))).ToList<OrderByColumn>(), queryPaginationOptions, maxConcurrency, continuationToken, cancellationToken));
      if (queryInfo.HasAggregates && !queryInfo.HasGroupBy)
      {
        MonadicCreatePipelineStage monadicCreateSourceStage = createPipelineStage;
        createPipelineStage = (MonadicCreatePipelineStage) ((continuationToken, cancellationToken) => AggregateQueryPipelineStage.MonadicCreate(executionEnvironment, queryInfo.Aggregates, queryInfo.GroupByAliasToAggregateType, queryInfo.GroupByAliases, queryInfo.HasSelectValue, continuationToken, cancellationToken, monadicCreateSourceStage));
      }
      if (queryInfo.HasDistinct)
      {
        MonadicCreatePipelineStage monadicCreateSourceStage = createPipelineStage;
        createPipelineStage = (MonadicCreatePipelineStage) ((continuationToken, cancellationToken) => DistinctQueryPipelineStage.MonadicCreate(executionEnvironment, continuationToken, cancellationToken, monadicCreateSourceStage, queryInfo.DistinctType));
      }
      if (queryInfo.HasGroupBy)
      {
        MonadicCreatePipelineStage monadicCreateSourceStage = createPipelineStage;
        createPipelineStage = (MonadicCreatePipelineStage) ((continuationToken, cancellationToken) => GroupByQueryPipelineStage.MonadicCreate(executionEnvironment, continuationToken, cancellationToken, monadicCreateSourceStage, queryInfo.GroupByAliasToAggregateType, queryInfo.GroupByAliases, queryInfo.HasSelectValue, (queryPaginationOptions ?? QueryPaginationOptions.Default).PageSizeLimit.GetValueOrDefault(int.MaxValue)));
      }
      if (queryInfo.HasOffset)
      {
        MonadicCreatePipelineStage monadicCreateSourceStage = createPipelineStage;
        createPipelineStage = (MonadicCreatePipelineStage) ((continuationToken, cancellationToken) => SkipQueryPipelineStage.MonadicCreate(executionEnvironment, queryInfo.Offset.Value, continuationToken, cancellationToken, monadicCreateSourceStage));
      }
      if (queryInfo.HasLimit)
      {
        MonadicCreatePipelineStage monadicCreateSourceStage = createPipelineStage;
        createPipelineStage = (MonadicCreatePipelineStage) ((continuationToken, cancellationToken) => TakeQueryPipelineStage.MonadicCreateLimitStage(executionEnvironment, queryInfo.Limit.Value, continuationToken, cancellationToken, monadicCreateSourceStage));
      }
      if (queryInfo.HasTop)
      {
        MonadicCreatePipelineStage monadicCreateSourceStage = createPipelineStage;
        createPipelineStage = (MonadicCreatePipelineStage) ((continuationToken, cancellationToken) => TakeQueryPipelineStage.MonadicCreateTopStage(executionEnvironment, queryInfo.Top.Value, continuationToken, cancellationToken, monadicCreateSourceStage));
      }
      if (queryInfo.HasDCount)
      {
        MonadicCreatePipelineStage monadicCreateSourceStage = createPipelineStage;
        createPipelineStage = (MonadicCreatePipelineStage) ((continuationToken, cancellationToken) => DCountQueryPipelineStage.MonadicCreate(executionEnvironment, queryInfo.DCountInfo, continuationToken, cancellationToken, monadicCreateSourceStage));
      }
      return createPipelineStage(requestContinuationToken, requestCancellationToken).Try<IQueryPipelineStage>((Func<IQueryPipelineStage, IQueryPipelineStage>) (stage => (IQueryPipelineStage) new SkipEmptyPageQueryPipelineStage(stage, requestCancellationToken)));
    }

    private static PrefetchPolicy DeterminePrefetchPolicy(QueryInfo queryInfo) => queryInfo.HasDCount || queryInfo.HasAggregates || queryInfo.HasGroupBy ? PrefetchPolicy.PrefetchAll : PrefetchPolicy.PrefetchSinglePage;
  }
}
