// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Query.Core.Pipeline.CrossPartition.OrderBy.OrderByCrossPartitionQueryPipelineStage
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.Core.Utf8;
using Microsoft.Azure.Cosmos.CosmosElements;
using Microsoft.Azure.Cosmos.Pagination;
using Microsoft.Azure.Cosmos.Query.Core.Collections;
using Microsoft.Azure.Cosmos.Query.Core.Exceptions;
using Microsoft.Azure.Cosmos.Query.Core.Monads;
using Microsoft.Azure.Cosmos.Query.Core.Pipeline.CrossPartition.Parallel;
using Microsoft.Azure.Cosmos.Query.Core.Pipeline.Pagination;
using Microsoft.Azure.Cosmos.Query.Core.QueryClient;
using Microsoft.Azure.Cosmos.Resource.CosmosExceptions;
using Microsoft.Azure.Cosmos.Tracing;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Cosmos.Query.Core.Pipeline.CrossPartition.OrderBy
{
  internal sealed class OrderByCrossPartitionQueryPipelineStage : 
    IQueryPipelineStage,
    ITracingAsyncEnumerator<TryCatch<QueryPage>>,
    IAsyncDisposable
  {
    private const string FormatPlaceHolder = "{documentdb-formattableorderbyquery-filter}";
    private const string TrueFilter = "true";
    private static readonly QueryState InitializingQueryState = new QueryState((CosmosElement) CosmosString.Create("ORDER BY NOT INITIALIZED YET!"));
    private static readonly IReadOnlyList<CosmosElement> EmptyPage = (IReadOnlyList<CosmosElement>) new List<CosmosElement>();
    private readonly IDocumentContainer documentContainer;
    private readonly IReadOnlyList<SortOrder> sortOrders;
    private readonly PriorityQueue<OrderByQueryPartitionRangePageAsyncEnumerator> enumerators;
    private readonly Queue<(OrderByQueryPartitionRangePageAsyncEnumerator enumerator, OrderByContinuationToken token)> uninitializedEnumeratorsAndTokens;
    private readonly QueryPaginationOptions queryPaginationOptions;
    private readonly int maxConcurrency;
    private CancellationToken cancellationToken;
    private QueryState state;
    private bool returnedFinalPage;

    private OrderByCrossPartitionQueryPipelineStage(
      IDocumentContainer documentContainer,
      IReadOnlyList<SortOrder> sortOrders,
      QueryPaginationOptions queryPaginationOptions,
      int maxConcurrency,
      IEnumerable<(OrderByQueryPartitionRangePageAsyncEnumerator, OrderByContinuationToken)> uninitializedEnumeratorsAndTokens,
      QueryState state,
      CancellationToken cancellationToken)
    {
      this.documentContainer = documentContainer ?? throw new ArgumentNullException(nameof (documentContainer));
      this.sortOrders = sortOrders ?? throw new ArgumentNullException(nameof (sortOrders));
      this.enumerators = new PriorityQueue<OrderByQueryPartitionRangePageAsyncEnumerator>((IComparer<OrderByQueryPartitionRangePageAsyncEnumerator>) new OrderByEnumeratorComparer(this.sortOrders));
      this.queryPaginationOptions = queryPaginationOptions ?? QueryPaginationOptions.Default;
      this.maxConcurrency = maxConcurrency >= 0 ? maxConcurrency : throw new ArgumentOutOfRangeException("maxConcurrency must be a non negative number.");
      this.uninitializedEnumeratorsAndTokens = new Queue<(OrderByQueryPartitionRangePageAsyncEnumerator, OrderByContinuationToken)>(uninitializedEnumeratorsAndTokens ?? throw new ArgumentNullException(nameof (uninitializedEnumeratorsAndTokens)));
      this.state = state ?? OrderByCrossPartitionQueryPipelineStage.InitializingQueryState;
      this.cancellationToken = cancellationToken;
    }

    public TryCatch<QueryPage> Current { get; private set; }

    public ValueTask DisposeAsync() => new ValueTask();

    private async ValueTask<bool> MoveNextAsync_Initialize_FromBeginningAsync(
      OrderByQueryPartitionRangePageAsyncEnumerator uninitializedEnumerator,
      ITrace trace)
    {
      this.cancellationToken.ThrowIfCancellationRequested();
      if (uninitializedEnumerator == null)
        throw new ArgumentNullException(nameof (uninitializedEnumerator));
      if (!await uninitializedEnumerator.MoveNextAsync(trace))
      {
        this.Current = TryCatch<QueryPage>.FromResult(new QueryPage(OrderByCrossPartitionQueryPipelineStage.EmptyPage, 0.0, string.Empty, 0L, (Lazy<CosmosQueryExecutionInfo>) null, (string) null, (IReadOnlyDictionary<string, string>) null, this.state));
        return true;
      }
      if (uninitializedEnumerator.Current.Failed)
      {
        if (OrderByCrossPartitionQueryPipelineStage.IsSplitException(uninitializedEnumerator.Current.Exception))
          return await this.MoveNextAsync_InitializeAsync_HandleSplitAsync(uninitializedEnumerator, (OrderByContinuationToken) null, trace);
        this.uninitializedEnumeratorsAndTokens.Enqueue((uninitializedEnumerator, (OrderByContinuationToken) null));
        this.Current = TryCatch<QueryPage>.FromException(uninitializedEnumerator.Current.Exception);
      }
      else
      {
        QueryPage page = uninitializedEnumerator.Current.Result.Page;
        if (!uninitializedEnumerator.Current.Result.Enumerator.MoveNext())
        {
          if (uninitializedEnumerator.FeedRangeState.State != null)
            this.uninitializedEnumeratorsAndTokens.Enqueue((uninitializedEnumerator, (OrderByContinuationToken) null));
          if (this.uninitializedEnumeratorsAndTokens.Count == 0 && this.enumerators.Count == 0)
          {
            this.Current = TryCatch<QueryPage>.FromResult(new QueryPage(OrderByCrossPartitionQueryPipelineStage.EmptyPage, page.RequestCharge, string.IsNullOrEmpty(page.ActivityId) ? Guid.NewGuid().ToString() : page.ActivityId, page.ResponseLengthInBytes, page.CosmosQueryExecutionInfo, page.DisallowContinuationTokenMessage, page.AdditionalHeaders, (QueryState) null));
            this.returnedFinalPage = true;
            return true;
          }
        }
        else
          this.enumerators.Enqueue(uninitializedEnumerator);
        this.Current = TryCatch<QueryPage>.FromResult(new QueryPage(OrderByCrossPartitionQueryPipelineStage.EmptyPage, page.RequestCharge, page.ActivityId, page.ResponseLengthInBytes, page.CosmosQueryExecutionInfo, page.DisallowContinuationTokenMessage, page.AdditionalHeaders, this.state));
      }
      return true;
    }

    private async ValueTask<bool> MoveNextAsync_Initialize_FilterAsync(
      OrderByQueryPartitionRangePageAsyncEnumerator uninitializedEnumerator,
      OrderByContinuationToken token,
      ITrace trace)
    {
      this.cancellationToken.ThrowIfCancellationRequested();
      if (uninitializedEnumerator == null)
        throw new ArgumentNullException(nameof (uninitializedEnumerator));
      if (token == null)
        throw new ArgumentNullException(nameof (token));
      TryCatch<(bool, int, TryCatch<OrderByQueryPage>)> tryCatch1 = await OrderByCrossPartitionQueryPipelineStage.FilterNextAsync(uninitializedEnumerator, this.sortOrders, token, trace, new CancellationToken());
      if (tryCatch1.Failed)
      {
        if (OrderByCrossPartitionQueryPipelineStage.IsSplitException(tryCatch1.Exception))
          return await this.MoveNextAsync_InitializeAsync_HandleSplitAsync(uninitializedEnumerator, token, trace);
        this.Current = TryCatch<QueryPage>.FromException(tryCatch1.Exception);
        return true;
      }
      (bool flag, int skipCount, TryCatch<OrderByQueryPage> tryCatch2) = tryCatch1.Result;
      QueryPage page = uninitializedEnumerator.Current.Result.Page;
      if (flag)
      {
        if (uninitializedEnumerator.Current.Result.Enumerator.Current != (CosmosElement) null)
          this.enumerators.Enqueue(uninitializedEnumerator);
        else if (this.uninitializedEnumeratorsAndTokens.Count == 0 && this.enumerators.Count == 0)
        {
          this.Current = TryCatch<QueryPage>.FromResult(new QueryPage(OrderByCrossPartitionQueryPipelineStage.EmptyPage, page.RequestCharge, string.IsNullOrEmpty(page.ActivityId) ? Guid.NewGuid().ToString() : page.ActivityId, page.ResponseLengthInBytes, page.CosmosQueryExecutionInfo, page.DisallowContinuationTokenMessage, page.AdditionalHeaders, (QueryState) null));
          this.returnedFinalPage = true;
          return true;
        }
      }
      else
      {
        if (tryCatch2.Failed && OrderByCrossPartitionQueryPipelineStage.IsSplitException(tryCatch1.Exception))
          return await this.MoveNextAsync_InitializeAsync_HandleSplitAsync(uninitializedEnumerator, token, trace);
        if (uninitializedEnumerator.FeedRangeState.State != null)
        {
          OrderByContinuationToken orderByContinuationToken = new OrderByContinuationToken(new ParallelContinuationToken(UtfAnyString.op_Implicit(((CosmosString) uninitializedEnumerator.FeedRangeState.State.Value).Value), ((FeedRangeEpk) uninitializedEnumerator.FeedRangeState.FeedRange).Range), token.OrderByItems, token.Rid, skipCount, token.Filter);
          this.uninitializedEnumeratorsAndTokens.Enqueue((uninitializedEnumerator, orderByContinuationToken));
          this.state = new QueryState((CosmosElement) CosmosArray.Create((IEnumerable<CosmosElement>) new List<CosmosElement>()
          {
            OrderByContinuationToken.ToCosmosElement(orderByContinuationToken)
          }));
        }
      }
      this.Current = TryCatch<QueryPage>.FromResult(new QueryPage(OrderByCrossPartitionQueryPipelineStage.EmptyPage, page.RequestCharge, page.ActivityId, page.ResponseLengthInBytes, page.CosmosQueryExecutionInfo, page.DisallowContinuationTokenMessage, page.AdditionalHeaders, OrderByCrossPartitionQueryPipelineStage.InitializingQueryState));
      return true;
    }

    private async ValueTask<bool> MoveNextAsync_InitializeAsync_HandleSplitAsync(
      OrderByQueryPartitionRangePageAsyncEnumerator uninitializedEnumerator,
      OrderByContinuationToken token,
      ITrace trace)
    {
      this.cancellationToken.ThrowIfCancellationRequested();
      IReadOnlyList<FeedRangeEpk> childRangeAsync = (IReadOnlyList<FeedRangeEpk>) await this.documentContainer.GetChildRangeAsync(uninitializedEnumerator.FeedRangeState.FeedRange, trace, this.cancellationToken);
      if (childRangeAsync.Count <= 1)
      {
        await this.documentContainer.RefreshProviderAsync(trace, this.cancellationToken);
        childRangeAsync = (IReadOnlyList<FeedRangeEpk>) await this.documentContainer.GetChildRangeAsync(uninitializedEnumerator.FeedRangeState.FeedRange, trace, this.cancellationToken);
      }
      if (childRangeAsync.Count < 1)
      {
        string message = "SDK invariant violated 82086B2D: Must have at least one EPK range in a cross partition enumerator";
        throw CosmosExceptionFactory.CreateInternalServerErrorException(message, (Microsoft.Azure.Cosmos.Headers) null, trace: trace, error: new Error()
        {
          Code = "SDK_invariant_violated_82086B2D",
          Message = message
        });
      }
      if (childRangeAsync.Count == 1)
      {
        this.uninitializedEnumeratorsAndTokens.Enqueue((new OrderByQueryPartitionRangePageAsyncEnumerator((IQueryDataSource) this.documentContainer, uninitializedEnumerator.SqlQuerySpec, new FeedRangeState<QueryState>(uninitializedEnumerator.FeedRangeState.FeedRange, uninitializedEnumerator.StartOfPageState), new Microsoft.Azure.Cosmos.PartitionKey?(), uninitializedEnumerator.QueryPaginationOptions, uninitializedEnumerator.Filter, this.cancellationToken), token));
      }
      else
      {
        foreach (FeedRangeInternal feedRange in (IEnumerable<FeedRangeEpk>) childRangeAsync)
        {
          this.cancellationToken.ThrowIfCancellationRequested();
          this.uninitializedEnumeratorsAndTokens.Enqueue((new OrderByQueryPartitionRangePageAsyncEnumerator((IQueryDataSource) this.documentContainer, uninitializedEnumerator.SqlQuerySpec, new FeedRangeState<QueryState>(feedRange, uninitializedEnumerator.StartOfPageState), new Microsoft.Azure.Cosmos.PartitionKey?(), uninitializedEnumerator.QueryPaginationOptions, uninitializedEnumerator.Filter, this.cancellationToken), token));
        }
      }
      return await this.MoveNextAsync(trace);
    }

    private async ValueTask<bool> MoveNextAsync_InitializeAsync(ITrace trace)
    {
      this.cancellationToken.ThrowIfCancellationRequested();
      await ParallelPrefetch.PrefetchInParallelAsync((IEnumerable<IPrefetcher>) this.uninitializedEnumeratorsAndTokens.Select<(OrderByQueryPartitionRangePageAsyncEnumerator, OrderByContinuationToken), OrderByQueryPartitionRangePageAsyncEnumerator>((Func<(OrderByQueryPartitionRangePageAsyncEnumerator, OrderByContinuationToken), OrderByQueryPartitionRangePageAsyncEnumerator>) (value => value.enumerator)), this.maxConcurrency, trace, this.cancellationToken);
      (OrderByQueryPartitionRangePageAsyncEnumerator pageAsyncEnumerator, OrderByContinuationToken token) = this.uninitializedEnumeratorsAndTokens.Dequeue();
      bool flag;
      if (token == null)
        flag = await this.MoveNextAsync_Initialize_FromBeginningAsync(pageAsyncEnumerator, trace);
      else
        flag = await this.MoveNextAsync_Initialize_FilterAsync(pageAsyncEnumerator, token, trace);
      return flag;
    }

    private ValueTask<bool> MoveNextAsync_DrainPageAsync(ITrace trace)
    {
      this.cancellationToken.ThrowIfCancellationRequested();
      if (trace == null)
        throw new ArgumentNullException(nameof (trace));
      OrderByQueryPartitionRangePageAsyncEnumerator pageAsyncEnumerator = (OrderByQueryPartitionRangePageAsyncEnumerator) null;
      OrderByQueryResult orderByQueryResult = new OrderByQueryResult();
      List<OrderByQueryResult> source = new List<OrderByQueryResult>();
      TryCatch<OrderByQueryPage> current;
      while (source.Count < this.queryPaginationOptions.PageSizeLimit.GetValueOrDefault(int.MaxValue))
      {
        pageAsyncEnumerator = this.enumerators.Dequeue();
        current = pageAsyncEnumerator.Current;
        orderByQueryResult = new OrderByQueryResult(current.Result.Enumerator.Current);
        source.Add(orderByQueryResult);
        current = pageAsyncEnumerator.Current;
        if (!current.Result.Enumerator.MoveNext())
        {
          if (pageAsyncEnumerator.FeedRangeState.State != null)
          {
            this.uninitializedEnumeratorsAndTokens.Enqueue((pageAsyncEnumerator, (OrderByContinuationToken) null));
            FeedRangeState<QueryState> feedRangeState = pageAsyncEnumerator.FeedRangeState;
            string token = UtfAnyString.op_Implicit(((CosmosString) feedRangeState.State.Value).Value);
            feedRangeState = pageAsyncEnumerator.FeedRangeState;
            Range<string> range = ((FeedRangeEpk) feedRangeState.FeedRange).Range;
            this.state = new QueryState((CosmosElement) CosmosArray.Create((IEnumerable<CosmosElement>) new List<CosmosElement>()
            {
              OrderByContinuationToken.ToCosmosElement(new OrderByContinuationToken(new ParallelContinuationToken(token, range), orderByQueryResult.OrderByItems, orderByQueryResult.Rid, 0, pageAsyncEnumerator.Filter))
            }));
            List<CosmosElement> list = source.Select<OrderByQueryResult, CosmosElement>((Func<OrderByQueryResult, CosmosElement>) (result => result.Payload)).ToList<CosmosElement>();
            current = pageAsyncEnumerator.Current;
            IReadOnlyDictionary<string, string> additionalHeaders = current.Result.Page.AdditionalHeaders;
            QueryState state = this.state;
            this.Current = TryCatch<QueryPage>.FromResult(new QueryPage((IReadOnlyList<CosmosElement>) list, 0.0, (string) null, 0L, (Lazy<CosmosQueryExecutionInfo>) null, (string) null, additionalHeaders, state));
            return new ValueTask<bool>(true);
          }
          break;
        }
        this.enumerators.Enqueue(pageAsyncEnumerator);
      }
      int skipCount = source.Where<OrderByQueryResult>((Func<OrderByQueryResult, bool>) (result => string.Equals(result.Rid, orderByQueryResult.Rid))).Count<OrderByQueryResult>();
      CosmosElement cosmosElement;
      if (this.enumerators.Count == 0 && this.uninitializedEnumeratorsAndTokens.Count == 0)
        cosmosElement = (CosmosElement) null;
      else
        cosmosElement = (CosmosElement) CosmosArray.Create((IEnumerable<CosmosElement>) new List<CosmosElement>()
        {
          OrderByContinuationToken.ToCosmosElement(new OrderByContinuationToken(new ParallelContinuationToken(UtfAnyString.op_Implicit(pageAsyncEnumerator.StartOfPageState != null ? ((CosmosString) pageAsyncEnumerator.StartOfPageState.Value).Value : UtfAnyString.op_Implicit((string) null)), ((FeedRangeEpk) pageAsyncEnumerator.FeedRangeState.FeedRange).Range), orderByQueryResult.OrderByItems, orderByQueryResult.Rid, skipCount, pageAsyncEnumerator.Filter))
        });
      this.state = cosmosElement != (CosmosElement) null ? new QueryState(cosmosElement) : (QueryState) null;
      List<CosmosElement> list1 = source.Select<OrderByQueryResult, CosmosElement>((Func<OrderByQueryResult, CosmosElement>) (result => result.Payload)).ToList<CosmosElement>();
      IReadOnlyDictionary<string, string> additionalHeaders1;
      if (pageAsyncEnumerator == null)
      {
        additionalHeaders1 = (IReadOnlyDictionary<string, string>) null;
      }
      else
      {
        current = pageAsyncEnumerator.Current;
        additionalHeaders1 = current.Result.Page.AdditionalHeaders;
      }
      QueryState state1 = this.state;
      this.Current = TryCatch<QueryPage>.FromResult(new QueryPage((IReadOnlyList<CosmosElement>) list1, 0.0, (string) null, 0L, (Lazy<CosmosQueryExecutionInfo>) null, (string) null, additionalHeaders1, state1));
      if (cosmosElement == (CosmosElement) null)
        this.returnedFinalPage = true;
      return new ValueTask<bool>(true);
    }

    public ValueTask<bool> MoveNextAsync(ITrace trace)
    {
      this.cancellationToken.ThrowIfCancellationRequested();
      if (trace == null)
        throw new ArgumentNullException(nameof (trace));
      if (this.uninitializedEnumeratorsAndTokens.Count != 0)
        return this.MoveNextAsync_InitializeAsync(trace);
      if (this.enumerators.Count != 0)
        return this.MoveNextAsync_DrainPageAsync(trace);
      if (this.returnedFinalPage)
        return new ValueTask<bool>(false);
      this.Current = TryCatch<QueryPage>.FromResult(new QueryPage(OrderByCrossPartitionQueryPipelineStage.EmptyPage, 0.0, Guid.NewGuid().ToString(), 0L, (Lazy<CosmosQueryExecutionInfo>) null, (string) null, (IReadOnlyDictionary<string, string>) null, (QueryState) null));
      this.returnedFinalPage = true;
      return new ValueTask<bool>(true);
    }

    public static TryCatch<IQueryPipelineStage> MonadicCreate(
      IDocumentContainer documentContainer,
      SqlQuerySpec sqlQuerySpec,
      IReadOnlyList<FeedRangeEpk> targetRanges,
      Microsoft.Azure.Cosmos.PartitionKey? partitionKey,
      IReadOnlyList<OrderByColumn> orderByColumns,
      QueryPaginationOptions queryPaginationOptions,
      int maxConcurrency,
      CosmosElement continuationToken,
      CancellationToken cancellationToken)
    {
      if (documentContainer == null)
        throw new ArgumentNullException(nameof (documentContainer));
      if (sqlQuerySpec == null)
        throw new ArgumentNullException(nameof (sqlQuerySpec));
      if (targetRanges == null)
        throw new ArgumentNullException(nameof (targetRanges));
      if (targetRanges.Count == 0)
        throw new ArgumentException("targetRanges must not be empty.");
      if (orderByColumns == null)
        throw new ArgumentNullException(nameof (orderByColumns));
      if (orderByColumns.Count == 0)
        throw new ArgumentException("orderByColumns must not be empty.");
      List<(OrderByQueryPartitionRangePageAsyncEnumerator, OrderByContinuationToken)> uninitializedEnumeratorsAndTokens;
      if (continuationToken == (CosmosElement) null)
      {
        SqlQuerySpec rewrittenQueryForOrderBy = new SqlQuerySpec(sqlQuerySpec.QueryText.Replace("{documentdb-formattableorderbyquery-filter}", "true"), sqlQuerySpec.Parameters);
        uninitializedEnumeratorsAndTokens = targetRanges.Select<FeedRangeEpk, (OrderByQueryPartitionRangePageAsyncEnumerator, OrderByContinuationToken)>((Func<FeedRangeEpk, (OrderByQueryPartitionRangePageAsyncEnumerator, OrderByContinuationToken)>) (range => (new OrderByQueryPartitionRangePageAsyncEnumerator((IQueryDataSource) documentContainer, rewrittenQueryForOrderBy, new FeedRangeState<QueryState>((FeedRangeInternal) range, (QueryState) null), partitionKey, queryPaginationOptions, "true", cancellationToken), (OrderByContinuationToken) null))).ToList<(OrderByQueryPartitionRangePageAsyncEnumerator, OrderByContinuationToken)>();
      }
      else
      {
        TryCatch<PartitionMapper.PartitionMapping<OrderByContinuationToken>> continuationTokenMapping = OrderByCrossPartitionQueryPipelineStage.MonadicGetOrderByContinuationTokenMapping(targetRanges, continuationToken, orderByColumns.Count);
        if (continuationTokenMapping.Failed)
          return TryCatch<IQueryPipelineStage>.FromException(continuationTokenMapping.Exception);
        PartitionMapper.PartitionMapping<OrderByContinuationToken> result = continuationTokenMapping.Result;
        IReadOnlyList<CosmosElement> list = (IReadOnlyList<CosmosElement>) result.TargetMapping.Values.First<OrderByContinuationToken>().OrderByItems.Select<OrderByItem, CosmosElement>((Func<OrderByItem, CosmosElement>) (x => x.Item)).ToList<CosmosElement>();
        if (list.Count != orderByColumns.Count)
          return TryCatch<IQueryPipelineStage>.FromException((Exception) new MalformedContinuationTokenException("Order By Items from continuation token did not match the query text. " + string.Format("Order by item count: {0} did not match column count {1}. ", (object) list.Count<CosmosElement>(), (object) orderByColumns.Count<OrderByColumn>()) + string.Format("Continuation token: {0}", (object) continuationToken)));
        (string leftFilter, string targetFilter, string rightFilter) = OrderByCrossPartitionQueryPipelineStage.GetFormattedFilters((ReadOnlyMemory<(OrderByColumn, CosmosElement)>) orderByColumns.Zip<OrderByColumn, CosmosElement, (OrderByColumn, CosmosElement)>((IEnumerable<CosmosElement>) list, (Func<OrderByColumn, CosmosElement, (OrderByColumn, CosmosElement)>) ((column, item) => (column, item))).ToArray<(OrderByColumn, CosmosElement)>());
        List<(IReadOnlyDictionary<FeedRangeEpk, OrderByContinuationToken>, string)> valueTupleList = new List<(IReadOnlyDictionary<FeedRangeEpk, OrderByContinuationToken>, string)>();
        valueTupleList.Add((result.MappingLeftOfTarget, leftFilter));
        valueTupleList.Add((result.TargetMapping, targetFilter));
        valueTupleList.Add((result.MappingRightOfTarget, rightFilter));
        uninitializedEnumeratorsAndTokens = new List<(OrderByQueryPartitionRangePageAsyncEnumerator, OrderByContinuationToken)>();
        foreach ((IReadOnlyDictionary<FeedRangeEpk, OrderByContinuationToken> readOnlyDictionary, string str) in valueTupleList)
        {
          SqlQuerySpec sqlQuerySpec1 = new SqlQuerySpec(sqlQuerySpec.QueryText.Replace("{documentdb-formattableorderbyquery-filter}", str), sqlQuerySpec.Parameters);
          foreach (KeyValuePair<FeedRangeEpk, OrderByContinuationToken> keyValuePair in (IEnumerable<KeyValuePair<FeedRangeEpk, OrderByContinuationToken>>) readOnlyDictionary)
          {
            FeedRangeEpk key = keyValuePair.Key;
            OrderByContinuationToken continuationToken1 = keyValuePair.Value;
            OrderByQueryPartitionRangePageAsyncEnumerator pageAsyncEnumerator = new OrderByQueryPartitionRangePageAsyncEnumerator((IQueryDataSource) documentContainer, sqlQuerySpec1, new FeedRangeState<QueryState>((FeedRangeInternal) key, continuationToken1 == null || continuationToken1.ParallelContinuationToken?.Token == null ? (QueryState) null : new QueryState((CosmosElement) CosmosString.Create(continuationToken1.ParallelContinuationToken.Token))), partitionKey, queryPaginationOptions, str, cancellationToken);
            uninitializedEnumeratorsAndTokens.Add((pageAsyncEnumerator, continuationToken1));
          }
        }
      }
      return TryCatch<IQueryPipelineStage>.FromResult((IQueryPipelineStage) new OrderByCrossPartitionQueryPipelineStage(documentContainer, (IReadOnlyList<SortOrder>) orderByColumns.Select<OrderByColumn, SortOrder>((Func<OrderByColumn, SortOrder>) (column => column.SortOrder)).ToList<SortOrder>(), queryPaginationOptions, maxConcurrency, (IEnumerable<(OrderByQueryPartitionRangePageAsyncEnumerator, OrderByContinuationToken)>) uninitializedEnumeratorsAndTokens, continuationToken == (CosmosElement) null ? (QueryState) null : new QueryState(continuationToken), cancellationToken));
    }

    private static TryCatch<PartitionMapper.PartitionMapping<OrderByContinuationToken>> MonadicGetOrderByContinuationTokenMapping(
      IReadOnlyList<FeedRangeEpk> partitionKeyRanges,
      CosmosElement continuationToken,
      int numOrderByItems)
    {
      if (partitionKeyRanges == null)
        throw new ArgumentOutOfRangeException(nameof (partitionKeyRanges));
      if (numOrderByItems < 0)
        throw new ArgumentOutOfRangeException(nameof (numOrderByItems));
      TryCatch<List<OrderByContinuationToken>> tryCatch = !(continuationToken == (CosmosElement) null) ? OrderByCrossPartitionQueryPipelineStage.MonadicExtractOrderByTokens(continuationToken, numOrderByItems) : throw new ArgumentNullException(nameof (continuationToken));
      return tryCatch.Failed ? TryCatch<PartitionMapper.PartitionMapping<OrderByContinuationToken>>.FromException(tryCatch.Exception) : PartitionMapper.MonadicGetPartitionMapping<OrderByContinuationToken>(partitionKeyRanges, (IReadOnlyList<OrderByContinuationToken>) tryCatch.Result);
    }

    private static TryCatch<List<OrderByContinuationToken>> MonadicExtractOrderByTokens(
      CosmosElement continuationToken,
      int numOrderByColumns)
    {
      if (continuationToken == (CosmosElement) null)
        return TryCatch<List<OrderByContinuationToken>>.FromResult((List<OrderByContinuationToken>) null);
      if (!(continuationToken is CosmosArray cosmosArray))
        return TryCatch<List<OrderByContinuationToken>>.FromException((Exception) new MalformedContinuationTokenException(string.Format("Order by continuation token must be an array: {0}.", (object) continuationToken)));
      if (cosmosArray.Count == 0)
        return TryCatch<List<OrderByContinuationToken>>.FromException((Exception) new MalformedContinuationTokenException(string.Format("Order by continuation token cannot be empty: {0}.", (object) continuationToken)));
      List<OrderByContinuationToken> result = new List<OrderByContinuationToken>();
      foreach (CosmosElement cosmosElement in cosmosArray)
      {
        TryCatch<OrderByContinuationToken> fromCosmosElement = OrderByContinuationToken.TryCreateFromCosmosElement(cosmosElement);
        if (!fromCosmosElement.Succeeded)
          return TryCatch<List<OrderByContinuationToken>>.FromException(fromCosmosElement.Exception);
        result.Add(fromCosmosElement.Result);
      }
      foreach (OrderByContinuationToken continuationToken1 in result)
      {
        if (continuationToken1.OrderByItems.Count != numOrderByColumns)
          return TryCatch<List<OrderByContinuationToken>>.FromException((Exception) new MalformedContinuationTokenException(string.Format("Invalid order-by items in continuation token {0} for OrderBy~Context.", (object) continuationToken)));
      }
      return TryCatch<List<OrderByContinuationToken>>.FromResult(result);
    }

    private static void AppendToBuilders(
      (StringBuilder leftFilter, StringBuilder targetFilter, StringBuilder rightFilter) builders,
      object str)
    {
      OrderByCrossPartitionQueryPipelineStage.AppendToBuilders(builders, str, str, str);
    }

    private static void AppendToBuilders(
      (StringBuilder leftFilter, StringBuilder targetFilter, StringBuilder rightFilter) builders,
      object left,
      object target,
      object right)
    {
      builders.leftFilter.Append(left);
      builders.targetFilter.Append(target);
      builders.rightFilter.Append(right);
    }

    private static (string leftFilter, string targetFilter, string rightFilter) GetFormattedFilters(
      ReadOnlyMemory<(OrderByColumn orderByColumn, CosmosElement orderByItem)> columnAndItems)
    {
      int length1 = columnAndItems.Length;
      int num = length1 == 1 ? 1 : 0;
      StringBuilder stringBuilder1 = new StringBuilder();
      StringBuilder stringBuilder2 = new StringBuilder();
      StringBuilder stringBuilder3 = new StringBuilder();
      (StringBuilder, StringBuilder, StringBuilder) builders = (stringBuilder1, stringBuilder2, stringBuilder3);
      if (num != 0)
      {
        (OrderByColumn orderByColumn, CosmosElement cosmosElement) = columnAndItems.Span[0];
        string expression1 = orderByColumn.Expression;
        int sortOrder1 = (int) orderByColumn.SortOrder;
        string expression2 = expression1;
        SortOrder sortOrder2 = (SortOrder) sortOrder1;
        OrderByCrossPartitionQueryPipelineStage.AppendToBuilders(builders, (object) "( ");
        if (!(cosmosElement is CosmosUndefined))
        {
          StringBuilder stringBuilder4 = new StringBuilder();
          CosmosElementToQueryLiteral elementToQueryLiteral = new CosmosElementToQueryLiteral(stringBuilder4);
          cosmosElement.Accept((ICosmosElementVisitor) elementToQueryLiteral);
          string str = stringBuilder4.ToString();
          stringBuilder1.Append(expression2 + " " + (sortOrder2 == SortOrder.Descending ? "<" : ">") + " " + str);
          stringBuilder2.Append(expression2 + " " + (sortOrder2 == SortOrder.Descending ? "<=" : ">=") + " " + str);
          stringBuilder3.Append(expression2 + " " + (sortOrder2 == SortOrder.Descending ? "<=" : ">=") + " " + str);
        }
        else
        {
          OrderByCrossPartitionQueryPipelineStage.ComparisionWithUndefinedFilters undefinedFilters = new OrderByCrossPartitionQueryPipelineStage.ComparisionWithUndefinedFilters(expression2);
          stringBuilder1.Append((sortOrder2 == SortOrder.Descending ? undefinedFilters.LessThan : undefinedFilters.GreaterThan) ?? "");
          stringBuilder2.Append((sortOrder2 == SortOrder.Descending ? undefinedFilters.LessThanOrEqualTo : undefinedFilters.GreaterThanOrEqualTo) ?? "");
          stringBuilder3.Append((sortOrder2 == SortOrder.Descending ? undefinedFilters.LessThanOrEqualTo : undefinedFilters.GreaterThanOrEqualTo) ?? "");
        }
        ReadOnlySpan<string> span = cosmosElement.Accept<bool, ReadOnlyMemory<string>>((ICosmosElementVisitor<bool, ReadOnlyMemory<string>>) OrderByCrossPartitionQueryPipelineStage.CosmosElementToIsSystemFunctionsVisitor.Singleton, sortOrder2 == SortOrder.Ascending).Span;
        for (int index = 0; index < span.Length; ++index)
        {
          string str = span[index];
          OrderByCrossPartitionQueryPipelineStage.AppendToBuilders(builders, (object) " OR ");
          OrderByCrossPartitionQueryPipelineStage.AppendToBuilders(builders, (object) (str + "(" + expression2 + ")"));
        }
        OrderByCrossPartitionQueryPipelineStage.AppendToBuilders(builders, (object) " )");
      }
      else
      {
        for (int length2 = 1; length2 <= length1; ++length2)
        {
          ReadOnlySpan<(OrderByColumn, CosmosElement)> readOnlySpan = columnAndItems.Span.Slice(0, length2);
          bool flag1 = length2 == length1;
          OrderByCrossPartitionQueryPipelineStage.AppendToBuilders(builders, (object) "(");
          for (int index1 = 0; index1 < length2; ++index1)
          {
            string expression = readOnlySpan[index1].Item1.Expression;
            SortOrder sortOrder = readOnlySpan[index1].Item1.SortOrder;
            CosmosElement cosmosElement = readOnlySpan[index1].Item2;
            bool flag2 = index1 == length2 - 1;
            OrderByCrossPartitionQueryPipelineStage.AppendToBuilders(builders, (object) "(");
            bool flag3;
            if (cosmosElement is CosmosUndefined)
            {
              OrderByCrossPartitionQueryPipelineStage.ComparisionWithUndefinedFilters undefinedFilters = new OrderByCrossPartitionQueryPipelineStage.ComparisionWithUndefinedFilters(expression);
              if (flag2)
              {
                if (flag1)
                {
                  if (sortOrder == SortOrder.Descending)
                    OrderByCrossPartitionQueryPipelineStage.AppendToBuilders(builders, (object) undefinedFilters.LessThan, (object) undefinedFilters.LessThanOrEqualTo, (object) undefinedFilters.LessThanOrEqualTo);
                  else
                    OrderByCrossPartitionQueryPipelineStage.AppendToBuilders(builders, (object) undefinedFilters.GreaterThan, (object) undefinedFilters.GreaterThanOrEqualTo, (object) undefinedFilters.GreaterThanOrEqualTo);
                }
                else if (sortOrder == SortOrder.Descending)
                  OrderByCrossPartitionQueryPipelineStage.AppendToBuilders(builders, (object) undefinedFilters.LessThan, (object) undefinedFilters.LessThan, (object) undefinedFilters.LessThan);
                else
                  OrderByCrossPartitionQueryPipelineStage.AppendToBuilders(builders, (object) undefinedFilters.GreaterThan, (object) undefinedFilters.GreaterThan, (object) undefinedFilters.GreaterThan);
                flag3 = true;
              }
              else
              {
                OrderByCrossPartitionQueryPipelineStage.AppendToBuilders(builders, (object) undefinedFilters.EqualTo);
                flag3 = false;
              }
            }
            else
            {
              OrderByCrossPartitionQueryPipelineStage.AppendToBuilders(builders, (object) expression);
              OrderByCrossPartitionQueryPipelineStage.AppendToBuilders(builders, (object) " ");
              if (flag2)
              {
                string str = sortOrder == SortOrder.Descending ? "<" : ">";
                OrderByCrossPartitionQueryPipelineStage.AppendToBuilders(builders, (object) str);
                if (flag1)
                  OrderByCrossPartitionQueryPipelineStage.AppendToBuilders(builders, (object) string.Empty, (object) "=", (object) "=");
                flag3 = true;
              }
              else
              {
                OrderByCrossPartitionQueryPipelineStage.AppendToBuilders(builders, (object) "=");
                flag3 = false;
              }
              StringBuilder stringBuilder5 = new StringBuilder();
              CosmosElementToQueryLiteral elementToQueryLiteral = new CosmosElementToQueryLiteral(stringBuilder5);
              cosmosElement.Accept((ICosmosElementVisitor) elementToQueryLiteral);
              string str1 = stringBuilder5.ToString();
              OrderByCrossPartitionQueryPipelineStage.AppendToBuilders(builders, (object) " ");
              OrderByCrossPartitionQueryPipelineStage.AppendToBuilders(builders, (object) str1);
              OrderByCrossPartitionQueryPipelineStage.AppendToBuilders(builders, (object) " ");
            }
            if (flag3)
            {
              ReadOnlySpan<string> span = cosmosElement.Accept<bool, ReadOnlyMemory<string>>((ICosmosElementVisitor<bool, ReadOnlyMemory<string>>) OrderByCrossPartitionQueryPipelineStage.CosmosElementToIsSystemFunctionsVisitor.Singleton, sortOrder == SortOrder.Ascending).Span;
              for (int index2 = 0; index2 < span.Length; ++index2)
              {
                string str = span[index2];
                OrderByCrossPartitionQueryPipelineStage.AppendToBuilders(builders, (object) " OR ");
                OrderByCrossPartitionQueryPipelineStage.AppendToBuilders(builders, (object) (str + "(" + expression + ") "));
              }
            }
            OrderByCrossPartitionQueryPipelineStage.AppendToBuilders(builders, (object) ")");
            if (!flag2)
              OrderByCrossPartitionQueryPipelineStage.AppendToBuilders(builders, (object) " AND ");
          }
          OrderByCrossPartitionQueryPipelineStage.AppendToBuilders(builders, (object) ")");
          if (!flag1)
            OrderByCrossPartitionQueryPipelineStage.AppendToBuilders(builders, (object) " OR ");
        }
      }
      return (stringBuilder1.ToString(), stringBuilder2.ToString(), stringBuilder3.ToString());
    }

    private static async Task<TryCatch<(bool doneFiltering, int itemsLeftToSkip, TryCatch<OrderByQueryPage> monadicQueryByPage)>> FilterNextAsync(
      OrderByQueryPartitionRangePageAsyncEnumerator enumerator,
      IReadOnlyList<SortOrder> sortOrders,
      OrderByContinuationToken continuationToken,
      ITrace trace,
      CancellationToken cancellationToken)
    {
      cancellationToken.ThrowIfCancellationRequested();
      int itemsToSkip = continuationToken.SkipCount;
      ResourceId continuationRid;
      if (!ResourceId.TryParse(continuationToken.Rid, out continuationRid))
        return TryCatch<(bool, int, TryCatch<OrderByQueryPage>)>.FromException((Exception) new MalformedContinuationTokenException("Invalid Rid in the continuation token " + continuationToken.ParallelContinuationToken.Token + " for OrderBy~Context."));
      if (!await enumerator.MoveNextAsync(trace))
        return TryCatch<(bool, int, TryCatch<OrderByQueryPage>)>.FromResult((true, 0, enumerator.Current));
      TryCatch<OrderByQueryPage> current = enumerator.Current;
      if (current.Failed)
        return TryCatch<(bool, int, TryCatch<OrderByQueryPage>)>.FromException(current.Exception);
      OrderByQueryPage result = current.Result;
      IEnumerator<CosmosElement> enumerator1 = result.Enumerator;
      while (enumerator1.MoveNext())
      {
        int num1 = 0;
        OrderByQueryResult orderByQueryResult = new OrderByQueryResult(enumerator1.Current);
        for (int index = 0; index < sortOrders.Count && num1 == 0; ++index)
        {
          ItemComparer instance = ItemComparer.Instance;
          OrderByItem orderByItem = continuationToken.OrderByItems[index];
          CosmosElement element1 = orderByItem.Item;
          orderByItem = orderByQueryResult.OrderByItems[index];
          CosmosElement element2 = orderByItem.Item;
          num1 = instance.Compare(element1, element2);
          if (num1 != 0)
            num1 = sortOrders[index] == SortOrder.Ascending ? num1 : -num1;
        }
        if (num1 < 0)
          return TryCatch<(bool, int, TryCatch<OrderByQueryPage>)>.FromResult((true, 0, enumerator.Current));
        if (num1 <= 0)
        {
          int num2 = continuationRid.Document.CompareTo(ResourceId.Parse(orderByQueryResult.Rid).Document);
          Lazy<CosmosQueryExecutionInfo> queryExecutionInfo = result.Page.CosmosQueryExecutionInfo;
          if (queryExecutionInfo == null || queryExecutionInfo.Value.ReverseRidEnabled)
          {
            if (sortOrders[0] == SortOrder.Descending)
              num2 = -num2;
          }
          else if (queryExecutionInfo.Value.ReverseIndexScan)
            num2 = -num2;
          if (num2 < 0)
            return TryCatch<(bool, int, TryCatch<OrderByQueryPage>)>.FromResult((true, 0, enumerator.Current));
          if (num2 <= 0 && --itemsToSkip < 0)
            return TryCatch<(bool, int, TryCatch<OrderByQueryPage>)>.FromResult((true, 0, enumerator.Current));
        }
      }
      return TryCatch<(bool, int, TryCatch<OrderByQueryPage>)>.FromResult((false, itemsToSkip, enumerator.Current));
    }

    private static bool IsSplitException(Exception exception)
    {
      while (exception.InnerException != null)
        exception = exception.InnerException;
      return exception is CosmosException cosmosException && cosmosException.StatusCode == HttpStatusCode.Gone && cosmosException.SubStatusCode == 1002;
    }

    public void SetCancellationToken(CancellationToken cancellationToken)
    {
      this.cancellationToken = cancellationToken;
      foreach (PartitionRangePageAsyncEnumerator<OrderByQueryPage, QueryState> enumerator in this.enumerators)
        enumerator.SetCancellationToken(cancellationToken);
      foreach ((OrderByQueryPartitionRangePageAsyncEnumerator enumerator, OrderByContinuationToken token) enumeratorsAndToken in this.uninitializedEnumeratorsAndTokens)
        enumeratorsAndToken.enumerator.SetCancellationToken(cancellationToken);
    }

    private static class Expressions
    {
      public const string LessThan = "<";
      public const string LessThanOrEqualTo = "<=";
      public const string EqualTo = "=";
      public const string GreaterThan = ">";
      public const string GreaterThanOrEqualTo = ">=";
      public const string True = "true";
      public const string False = "false";
    }

    private sealed class CosmosElementToIsSystemFunctionsVisitor : 
      ICosmosElementVisitor<bool, ReadOnlyMemory<string>>
    {
      public static readonly OrderByCrossPartitionQueryPipelineStage.CosmosElementToIsSystemFunctionsVisitor Singleton = new OrderByCrossPartitionQueryPipelineStage.CosmosElementToIsSystemFunctionsVisitor();
      private static readonly ReadOnlyMemory<string> SystemFunctionSortOrder = (ReadOnlyMemory<string>) new string[7]
      {
        "NOT IS_DEFINED",
        "IS_NULL",
        "IS_BOOLEAN",
        "IS_NUMBER",
        "IS_STRING",
        "IS_ARRAY",
        "IS_OBJECT"
      };
      private static readonly ReadOnlyMemory<string> ExtendedTypesSystemFunctionSortOrder = (ReadOnlyMemory<string>) new string[2]
      {
        "NOT IS_DEFINED",
        "IS_DEFINED"
      };

      private CosmosElementToIsSystemFunctionsVisitor()
      {
      }

      public ReadOnlyMemory<string> Visit(CosmosArray cosmosArray, bool isAscending) => OrderByCrossPartitionQueryPipelineStage.CosmosElementToIsSystemFunctionsVisitor.GetIsDefinedFunctions(5, isAscending);

      public ReadOnlyMemory<string> Visit(CosmosBinary cosmosBinary, bool isAscending) => OrderByCrossPartitionQueryPipelineStage.CosmosElementToIsSystemFunctionsVisitor.GetExtendedTypesIsDefinedFunctions(1, isAscending);

      public ReadOnlyMemory<string> Visit(CosmosBoolean cosmosBoolean, bool isAscending) => OrderByCrossPartitionQueryPipelineStage.CosmosElementToIsSystemFunctionsVisitor.GetIsDefinedFunctions(2, isAscending);

      public ReadOnlyMemory<string> Visit(CosmosGuid cosmosGuid, bool isAscending) => OrderByCrossPartitionQueryPipelineStage.CosmosElementToIsSystemFunctionsVisitor.GetExtendedTypesIsDefinedFunctions(1, isAscending);

      public ReadOnlyMemory<string> Visit(CosmosNull cosmosNull, bool isAscending) => OrderByCrossPartitionQueryPipelineStage.CosmosElementToIsSystemFunctionsVisitor.GetIsDefinedFunctions(1, isAscending);

      public ReadOnlyMemory<string> Visit(CosmosUndefined cosmosUndefined, bool isAscending) => !isAscending ? ReadOnlyMemory<string>.Empty : OrderByCrossPartitionQueryPipelineStage.CosmosElementToIsSystemFunctionsVisitor.SystemFunctionSortOrder.Slice(1);

      public ReadOnlyMemory<string> Visit(CosmosNumber cosmosNumber, bool isAscending) => OrderByCrossPartitionQueryPipelineStage.CosmosElementToIsSystemFunctionsVisitor.GetIsDefinedFunctions(3, isAscending);

      public ReadOnlyMemory<string> Visit(CosmosObject cosmosObject, bool isAscending) => OrderByCrossPartitionQueryPipelineStage.CosmosElementToIsSystemFunctionsVisitor.GetIsDefinedFunctions(6, isAscending);

      public ReadOnlyMemory<string> Visit(CosmosString cosmosString, bool isAscending) => OrderByCrossPartitionQueryPipelineStage.CosmosElementToIsSystemFunctionsVisitor.GetIsDefinedFunctions(4, isAscending);

      private static ReadOnlyMemory<string> GetIsDefinedFunctions(int index, bool isAscending) => !isAscending ? OrderByCrossPartitionQueryPipelineStage.CosmosElementToIsSystemFunctionsVisitor.SystemFunctionSortOrder.Slice(0, index) : OrderByCrossPartitionQueryPipelineStage.CosmosElementToIsSystemFunctionsVisitor.SystemFunctionSortOrder.Slice(index + 1);

      private static ReadOnlyMemory<string> GetExtendedTypesIsDefinedFunctions(
        int index,
        bool isAscending)
      {
        return !isAscending ? OrderByCrossPartitionQueryPipelineStage.CosmosElementToIsSystemFunctionsVisitor.ExtendedTypesSystemFunctionSortOrder.Slice(0, index) : OrderByCrossPartitionQueryPipelineStage.CosmosElementToIsSystemFunctionsVisitor.ExtendedTypesSystemFunctionSortOrder.Slice(index + 1);
      }

      private static class IsSystemFunctions
      {
        public const string Defined = "IS_DEFINED";
        public const string Undefined = "NOT IS_DEFINED";
        public const string Null = "IS_NULL";
        public const string Boolean = "IS_BOOLEAN";
        public const string Number = "IS_NUMBER";
        public const string String = "IS_STRING";
        public const string Array = "IS_ARRAY";
        public const string Object = "IS_OBJECT";
      }

      private static class SortOrder
      {
        public const int Undefined = 0;
        public const int Null = 1;
        public const int Boolean = 2;
        public const int Number = 3;
        public const int String = 4;
        public const int Array = 5;
        public const int Object = 6;
      }

      private static class ExtendedTypesSortOrder
      {
        public const int Undefined = 0;
        public const int Defined = 1;
      }
    }

    private readonly struct ComparisionWithUndefinedFilters
    {
      public ComparisionWithUndefinedFilters(string expression)
      {
        this.LessThan = "false";
        this.LessThanOrEqualTo = "NOT IS_DEFINED(" + expression + ")";
        this.EqualTo = "NOT IS_DEFINED(" + expression + ")";
        this.GreaterThan = "IS_DEFINED(" + expression + ")";
        this.GreaterThanOrEqualTo = "true";
      }

      public string LessThan { get; }

      public string LessThanOrEqualTo { get; }

      public string EqualTo { get; }

      public string GreaterThan { get; }

      public string GreaterThanOrEqualTo { get; }
    }
  }
}
