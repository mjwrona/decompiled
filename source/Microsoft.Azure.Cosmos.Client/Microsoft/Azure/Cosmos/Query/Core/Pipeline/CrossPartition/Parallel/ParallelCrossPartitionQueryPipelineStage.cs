// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Query.Core.Pipeline.CrossPartition.Parallel.ParallelCrossPartitionQueryPipelineStage
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.Core.Utf8;
using Microsoft.Azure.Cosmos.CosmosElements;
using Microsoft.Azure.Cosmos.Pagination;
using Microsoft.Azure.Cosmos.Query.Core.Exceptions;
using Microsoft.Azure.Cosmos.Query.Core.Monads;
using Microsoft.Azure.Cosmos.Query.Core.Pipeline.Pagination;
using Microsoft.Azure.Cosmos.Tracing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Cosmos.Query.Core.Pipeline.CrossPartition.Parallel
{
  internal sealed class ParallelCrossPartitionQueryPipelineStage : 
    IQueryPipelineStage,
    ITracingAsyncEnumerator<TryCatch<QueryPage>>,
    IAsyncDisposable
  {
    private readonly CrossPartitionRangePageAsyncEnumerator<QueryPage, QueryState> crossPartitionRangePageAsyncEnumerator;
    private CancellationToken cancellationToken;

    private ParallelCrossPartitionQueryPipelineStage(
      CrossPartitionRangePageAsyncEnumerator<QueryPage, QueryState> crossPartitionRangePageAsyncEnumerator,
      CancellationToken cancellationToken)
    {
      this.crossPartitionRangePageAsyncEnumerator = crossPartitionRangePageAsyncEnumerator ?? throw new ArgumentNullException(nameof (crossPartitionRangePageAsyncEnumerator));
      this.cancellationToken = cancellationToken;
    }

    public TryCatch<QueryPage> Current { get; private set; }

    public ValueTask DisposeAsync() => this.crossPartitionRangePageAsyncEnumerator.DisposeAsync();

    public async ValueTask<bool> MoveNextAsync(ITrace trace)
    {
      if (trace == null)
        throw new ArgumentNullException(nameof (trace));
      if (!await this.crossPartitionRangePageAsyncEnumerator.MoveNextAsync(trace))
      {
        this.Current = new TryCatch<QueryPage>();
        return false;
      }
      TryCatch<CrossFeedRangePage<QueryPage, QueryState>> current = this.crossPartitionRangePageAsyncEnumerator.Current;
      if (current.Failed)
      {
        this.Current = TryCatch<QueryPage>.FromException(current.Exception);
        return true;
      }
      CrossFeedRangePage<QueryPage, QueryState> result = current.Result;
      QueryPage page = result.Page;
      CrossFeedRangeState<QueryState> state1 = result.State;
      QueryState state2;
      if (state1 == null)
      {
        state2 = (QueryState) null;
      }
      else
      {
        IOrderedEnumerable<FeedRangeState<QueryState>> source1 = ((IEnumerable<FeedRangeState<QueryState>>) state1.Value.ToArray()).OrderBy<FeedRangeState<QueryState>, string>((Func<FeedRangeState<QueryState>, string>) (tuple => ((FeedRangeEpk) tuple.FeedRange).Range.Min));
        List<ParallelContinuationToken> source2 = new List<ParallelContinuationToken>();
        FeedRangeState<QueryState> feedRangeState1 = source1.First<FeedRangeState<QueryState>>();
        ParallelContinuationToken continuationToken1 = new ParallelContinuationToken(UtfAnyString.op_Implicit(feedRangeState1.State != null ? ((CosmosString) feedRangeState1.State.Value).Value : UtfAnyString.op_Implicit((string) null)), ((FeedRangeEpk) feedRangeState1.FeedRange).Range);
        source2.Add(continuationToken1);
        foreach (FeedRangeState<QueryState> feedRangeState2 in source1.Skip<FeedRangeState<QueryState>>(1))
        {
          this.cancellationToken.ThrowIfCancellationRequested();
          if (feedRangeState2.State != null)
          {
            ParallelContinuationToken continuationToken2 = new ParallelContinuationToken(UtfAnyString.op_Implicit(feedRangeState2.State != null ? ((CosmosString) feedRangeState2.State.Value).Value : UtfAnyString.op_Implicit((string) null)), ((FeedRangeEpk) feedRangeState2.FeedRange).Range);
            source2.Add(continuationToken2);
          }
        }
        state2 = new QueryState((CosmosElement) CosmosArray.Create(source2.Select<ParallelContinuationToken, CosmosElement>((Func<ParallelContinuationToken, CosmosElement>) (token => ParallelContinuationToken.ToCosmosElement(token)))));
      }
      this.Current = TryCatch<QueryPage>.FromResult(new QueryPage(page.Documents, page.RequestCharge, page.ActivityId, page.ResponseLengthInBytes, page.CosmosQueryExecutionInfo, page.DisallowContinuationTokenMessage, page.AdditionalHeaders, state2));
      return true;
    }

    public static TryCatch<IQueryPipelineStage> MonadicCreate(
      IDocumentContainer documentContainer,
      SqlQuerySpec sqlQuerySpec,
      IReadOnlyList<FeedRangeEpk> targetRanges,
      PartitionKey? partitionKey,
      QueryPaginationOptions queryPaginationOptions,
      int maxConcurrency,
      PrefetchPolicy prefetchPolicy,
      CosmosElement continuationToken,
      CancellationToken cancellationToken)
    {
      if (targetRanges == null)
        throw new ArgumentNullException(nameof (targetRanges));
      TryCatch<CrossFeedRangeState<QueryState>> tryCatch = targetRanges.Count != 0 ? ParallelCrossPartitionQueryPipelineStage.MonadicExtractState(continuationToken, targetRanges) : throw new ArgumentException("targetRanges must have some elements");
      if (tryCatch.Failed)
        return TryCatch<IQueryPipelineStage>.FromException(tryCatch.Exception);
      CrossFeedRangeState<QueryState> result = tryCatch.Result;
      IDocumentContainer documentContainer1 = documentContainer;
      CreatePartitionRangePageAsyncEnumerator<QueryPage, QueryState> function = ParallelCrossPartitionQueryPipelineStage.MakeCreateFunction((IQueryDataSource) documentContainer, sqlQuerySpec, queryPaginationOptions, partitionKey, cancellationToken);
      ParallelCrossPartitionQueryPipelineStage.Comparer singleton = ParallelCrossPartitionQueryPipelineStage.Comparer.Singleton;
      int? maxConcurrency1 = new int?(maxConcurrency);
      int num = (int) prefetchPolicy;
      CrossFeedRangeState<QueryState> crossFeedRangeState = result;
      CancellationToken cancellationToken1 = cancellationToken;
      CrossFeedRangeState<QueryState> state = crossFeedRangeState;
      return TryCatch<IQueryPipelineStage>.FromResult((IQueryPipelineStage) new ParallelCrossPartitionQueryPipelineStage(new CrossPartitionRangePageAsyncEnumerator<QueryPage, QueryState>((IFeedRangeProvider) documentContainer1, function, (IComparer<PartitionRangePageAsyncEnumerator<QueryPage, QueryState>>) singleton, maxConcurrency1, (PrefetchPolicy) num, cancellationToken1, state), cancellationToken));
    }

    private static TryCatch<CrossFeedRangeState<QueryState>> MonadicExtractState(
      CosmosElement continuationToken,
      IReadOnlyList<FeedRangeEpk> ranges)
    {
      if (continuationToken == (CosmosElement) null)
        return TryCatch<CrossFeedRangeState<QueryState>>.FromResult(new CrossFeedRangeState<QueryState>((ReadOnlyMemory<FeedRangeState<QueryState>>) ranges.Select<FeedRangeEpk, FeedRangeState<QueryState>>((Func<FeedRangeEpk, FeedRangeState<QueryState>>) (range => new FeedRangeState<QueryState>((FeedRangeInternal) range, (QueryState) null))).ToArray<FeedRangeState<QueryState>>()));
      if (!(continuationToken is CosmosArray cosmosArray))
        return TryCatch<CrossFeedRangeState<QueryState>>.FromException((Exception) new MalformedContinuationTokenException(string.Format("Invalid format for continuation token {0} for {1}", (object) continuationToken, (object) nameof (ParallelCrossPartitionQueryPipelineStage))));
      if (cosmosArray.Count == 0)
        return TryCatch<CrossFeedRangeState<QueryState>>.FromException((Exception) new MalformedContinuationTokenException(string.Format("Invalid format for continuation token {0} for {1}", (object) continuationToken, (object) nameof (ParallelCrossPartitionQueryPipelineStage))));
      List<ParallelContinuationToken> tokens = new List<ParallelContinuationToken>();
      foreach (CosmosElement cosmosElement in cosmosArray)
      {
        TryCatch<ParallelContinuationToken> fromCosmosElement = ParallelContinuationToken.TryCreateFromCosmosElement(cosmosElement);
        if (fromCosmosElement.Failed)
          return TryCatch<CrossFeedRangeState<QueryState>>.FromException(fromCosmosElement.Exception);
        tokens.Add(fromCosmosElement.Result);
      }
      TryCatch<PartitionMapper.PartitionMapping<ParallelContinuationToken>> partitionMapping = PartitionMapper.MonadicGetPartitionMapping<ParallelContinuationToken>(ranges, (IReadOnlyList<ParallelContinuationToken>) tokens);
      if (partitionMapping.Failed)
        return TryCatch<CrossFeedRangeState<QueryState>>.FromException(partitionMapping.Exception);
      PartitionMapper.PartitionMapping<ParallelContinuationToken> result = partitionMapping.Result;
      List<FeedRangeState<QueryState>> feedRangeStateList = new List<FeedRangeState<QueryState>>();
      foreach (IEnumerable<KeyValuePair<FeedRangeEpk, ParallelContinuationToken>> keyValuePairs in new List<IReadOnlyDictionary<FeedRangeEpk, ParallelContinuationToken>>()
      {
        result.TargetMapping,
        result.MappingRightOfTarget
      })
      {
        foreach (KeyValuePair<FeedRangeEpk, ParallelContinuationToken> keyValuePair in keyValuePairs)
        {
          FeedRangeState<QueryState> feedRangeState = new FeedRangeState<QueryState>((FeedRangeInternal) keyValuePair.Key, keyValuePair.Value?.Token != null ? new QueryState((CosmosElement) CosmosString.Create(keyValuePair.Value.Token)) : (QueryState) null);
          feedRangeStateList.Add(feedRangeState);
        }
      }
      return TryCatch<CrossFeedRangeState<QueryState>>.FromResult(new CrossFeedRangeState<QueryState>((ReadOnlyMemory<FeedRangeState<QueryState>>) feedRangeStateList.ToArray()));
    }

    private static CreatePartitionRangePageAsyncEnumerator<QueryPage, QueryState> MakeCreateFunction(
      IQueryDataSource queryDataSource,
      SqlQuerySpec sqlQuerySpec,
      QueryPaginationOptions queryPaginationOptions,
      PartitionKey? partitionKey,
      CancellationToken cancellationToken)
    {
      return (CreatePartitionRangePageAsyncEnumerator<QueryPage, QueryState>) (feedRangeState => (PartitionRangePageAsyncEnumerator<QueryPage, QueryState>) new QueryPartitionRangePageAsyncEnumerator(queryDataSource, sqlQuerySpec, feedRangeState, partitionKey, queryPaginationOptions, cancellationToken));
    }

    public void SetCancellationToken(CancellationToken cancellationToken)
    {
      this.cancellationToken = cancellationToken;
      this.crossPartitionRangePageAsyncEnumerator.SetCancellationToken(cancellationToken);
    }

    private sealed class Comparer : 
      IComparer<PartitionRangePageAsyncEnumerator<QueryPage, QueryState>>
    {
      public static readonly ParallelCrossPartitionQueryPipelineStage.Comparer Singleton = new ParallelCrossPartitionQueryPipelineStage.Comparer();

      public int Compare(
        PartitionRangePageAsyncEnumerator<QueryPage, QueryState> partitionRangePageEnumerator1,
        PartitionRangePageAsyncEnumerator<QueryPage, QueryState> partitionRangePageEnumerator2)
      {
        return partitionRangePageEnumerator1 == partitionRangePageEnumerator2 ? 0 : string.CompareOrdinal(((FeedRangeEpk) partitionRangePageEnumerator1.FeedRangeState.FeedRange).Range.Min, ((FeedRangeEpk) partitionRangePageEnumerator2.FeedRangeState.FeedRange).Range.Min);
      }
    }
  }
}
