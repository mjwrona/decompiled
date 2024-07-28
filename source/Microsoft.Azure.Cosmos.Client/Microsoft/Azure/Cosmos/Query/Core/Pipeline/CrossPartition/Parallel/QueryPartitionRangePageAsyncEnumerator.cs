// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Query.Core.Pipeline.CrossPartition.Parallel.QueryPartitionRangePageAsyncEnumerator
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.Pagination;
using Microsoft.Azure.Cosmos.Query.Core.Monads;
using Microsoft.Azure.Cosmos.Query.Core.Pipeline.Pagination;
using Microsoft.Azure.Cosmos.Tracing;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Cosmos.Query.Core.Pipeline.CrossPartition.Parallel
{
  internal sealed class QueryPartitionRangePageAsyncEnumerator : 
    PartitionRangePageAsyncEnumerator<QueryPage, QueryState>
  {
    private readonly IQueryDataSource queryDataSource;
    private readonly SqlQuerySpec sqlQuerySpec;
    private readonly QueryPaginationOptions queryPaginationOptions;
    private readonly PartitionKey? partitionKey;

    public QueryPartitionRangePageAsyncEnumerator(
      IQueryDataSource queryDataSource,
      SqlQuerySpec sqlQuerySpec,
      Microsoft.Azure.Cosmos.Pagination.FeedRangeState<QueryState> feedRangeState,
      PartitionKey? partitionKey,
      QueryPaginationOptions queryPaginationOptions,
      CancellationToken cancellationToken)
      : base(feedRangeState, cancellationToken)
    {
      this.queryDataSource = queryDataSource ?? throw new ArgumentNullException(nameof (queryDataSource));
      this.sqlQuerySpec = sqlQuerySpec ?? throw new ArgumentNullException(nameof (sqlQuerySpec));
      this.queryPaginationOptions = queryPaginationOptions;
      this.partitionKey = partitionKey;
    }

    public override ValueTask DisposeAsync() => new ValueTask();

    protected override Task<TryCatch<QueryPage>> GetNextPageAsync(
      ITrace trace,
      CancellationToken cancellationToken)
    {
      if (trace == null)
        throw new ArgumentNullException(nameof (trace));
      return this.queryDataSource.MonadicQueryAsync(this.sqlQuerySpec, new Microsoft.Azure.Cosmos.Pagination.FeedRangeState<QueryState>(this.partitionKey.HasValue ? (FeedRangeInternal) new FeedRangePartitionKey(this.partitionKey.Value) : this.FeedRangeState.FeedRange, this.FeedRangeState.State), this.queryPaginationOptions, trace, cancellationToken);
    }
  }
}
