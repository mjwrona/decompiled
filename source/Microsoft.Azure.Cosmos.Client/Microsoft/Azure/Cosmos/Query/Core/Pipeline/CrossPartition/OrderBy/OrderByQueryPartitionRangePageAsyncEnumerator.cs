// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Query.Core.Pipeline.CrossPartition.OrderBy.OrderByQueryPartitionRangePageAsyncEnumerator
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

namespace Microsoft.Azure.Cosmos.Query.Core.Pipeline.CrossPartition.OrderBy
{
  internal sealed class OrderByQueryPartitionRangePageAsyncEnumerator : 
    PartitionRangePageAsyncEnumerator<OrderByQueryPage, QueryState>,
    IPrefetcher
  {
    private readonly OrderByQueryPartitionRangePageAsyncEnumerator.InnerEnumerator innerEnumerator;
    private readonly BufferedPartitionRangePageAsyncEnumerator<OrderByQueryPage, QueryState> bufferedEnumerator;

    public OrderByQueryPartitionRangePageAsyncEnumerator(
      IQueryDataSource queryDataSource,
      SqlQuerySpec sqlQuerySpec,
      Microsoft.Azure.Cosmos.Pagination.FeedRangeState<QueryState> feedRangeState,
      PartitionKey? partitionKey,
      QueryPaginationOptions queryPaginationOptions,
      string filter,
      CancellationToken cancellationToken)
      : base(feedRangeState, cancellationToken)
    {
      this.StartOfPageState = feedRangeState.State;
      this.innerEnumerator = new OrderByQueryPartitionRangePageAsyncEnumerator.InnerEnumerator(queryDataSource, sqlQuerySpec, feedRangeState, partitionKey, queryPaginationOptions, filter, cancellationToken);
      this.bufferedEnumerator = new BufferedPartitionRangePageAsyncEnumerator<OrderByQueryPage, QueryState>((PartitionRangePageAsyncEnumerator<OrderByQueryPage, QueryState>) this.innerEnumerator, cancellationToken);
    }

    public SqlQuerySpec SqlQuerySpec => this.innerEnumerator.SqlQuerySpec;

    public QueryPaginationOptions QueryPaginationOptions => this.innerEnumerator.QueryPaginationOptions;

    public string Filter => this.innerEnumerator.Filter;

    public QueryState StartOfPageState { get; private set; }

    public override ValueTask DisposeAsync() => new ValueTask();

    protected override async Task<TryCatch<OrderByQueryPage>> GetNextPageAsync(
      ITrace trace,
      CancellationToken cancellationToken)
    {
      OrderByQueryPartitionRangePageAsyncEnumerator pageAsyncEnumerator = this;
      pageAsyncEnumerator.StartOfPageState = pageAsyncEnumerator.FeedRangeState.State;
      int num = await pageAsyncEnumerator.bufferedEnumerator.MoveNextAsync(trace) ? 1 : 0;
      return pageAsyncEnumerator.bufferedEnumerator.Current;
    }

    public ValueTask PrefetchAsync(ITrace trace, CancellationToken cancellationToken) => this.bufferedEnumerator.PrefetchAsync(trace, cancellationToken);

    private sealed class InnerEnumerator : 
      PartitionRangePageAsyncEnumerator<OrderByQueryPage, QueryState>
    {
      private readonly IQueryDataSource queryDataSource;

      public InnerEnumerator(
        IQueryDataSource queryDataSource,
        SqlQuerySpec sqlQuerySpec,
        Microsoft.Azure.Cosmos.Pagination.FeedRangeState<QueryState> feedRangeState,
        PartitionKey? partitionKey,
        QueryPaginationOptions queryPaginationOptions,
        string filter,
        CancellationToken cancellationToken)
        : base(feedRangeState, cancellationToken)
      {
        this.queryDataSource = queryDataSource ?? throw new ArgumentNullException(nameof (queryDataSource));
        this.SqlQuerySpec = sqlQuerySpec ?? throw new ArgumentNullException(nameof (sqlQuerySpec));
        this.PartitionKey = partitionKey;
        this.QueryPaginationOptions = queryPaginationOptions ?? QueryPaginationOptions.Default;
        this.Filter = filter;
      }

      public SqlQuerySpec SqlQuerySpec { get; }

      public PartitionKey? PartitionKey { get; }

      public QueryPaginationOptions QueryPaginationOptions { get; }

      public string Filter { get; }

      public override ValueTask DisposeAsync() => new ValueTask();

      protected override async Task<TryCatch<OrderByQueryPage>> GetNextPageAsync(
        ITrace trace,
        CancellationToken cancellationToken)
      {
        OrderByQueryPartitionRangePageAsyncEnumerator.InnerEnumerator innerEnumerator = this;
        FeedRangeInternal feedRange = innerEnumerator.PartitionKey.HasValue ? (FeedRangeInternal) new FeedRangePartitionKey(innerEnumerator.PartitionKey.Value) : innerEnumerator.FeedRangeState.FeedRange;
        TryCatch<QueryPage> tryCatch = await innerEnumerator.queryDataSource.MonadicQueryAsync(innerEnumerator.SqlQuerySpec, new Microsoft.Azure.Cosmos.Pagination.FeedRangeState<QueryState>(feedRange, innerEnumerator.FeedRangeState.State), innerEnumerator.QueryPaginationOptions, trace, cancellationToken);
        return !tryCatch.Failed ? TryCatch<OrderByQueryPage>.FromResult(new OrderByQueryPage(tryCatch.Result)) : TryCatch<OrderByQueryPage>.FromException(tryCatch.Exception);
      }
    }
  }
}
