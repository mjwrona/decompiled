// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Query.Core.Pipeline.OptimisticDirectExecutionQuery.OptimisticDirectExecutionQueryPipelineStage
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.Core.Utf8;
using Microsoft.Azure.Cosmos.CosmosElements;
using Microsoft.Azure.Cosmos.Pagination;
using Microsoft.Azure.Cosmos.Query.Core.Monads;
using Microsoft.Azure.Cosmos.Query.Core.Pipeline.CrossPartition;
using Microsoft.Azure.Cosmos.Query.Core.Pipeline.CrossPartition.Parallel;
using Microsoft.Azure.Cosmos.Query.Core.Pipeline.Pagination;
using Microsoft.Azure.Cosmos.Tracing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Cosmos.Query.Core.Pipeline.OptimisticDirectExecutionQuery
{
  internal sealed class OptimisticDirectExecutionQueryPipelineStage : 
    IQueryPipelineStage,
    ITracingAsyncEnumerator<TryCatch<QueryPage>>,
    IAsyncDisposable
  {
    private readonly QueryPartitionRangePageAsyncEnumerator queryPartitionRangePageAsyncEnumerator;

    private OptimisticDirectExecutionQueryPipelineStage(
      QueryPartitionRangePageAsyncEnumerator queryPartitionRangePageAsyncEnumerator)
    {
      this.queryPartitionRangePageAsyncEnumerator = queryPartitionRangePageAsyncEnumerator ?? throw new ArgumentNullException(nameof (queryPartitionRangePageAsyncEnumerator));
    }

    public TryCatch<QueryPage> Current { get; private set; }

    public ValueTask DisposeAsync() => this.queryPartitionRangePageAsyncEnumerator.DisposeAsync();

    public void SetCancellationToken(CancellationToken cancellationToken) => this.queryPartitionRangePageAsyncEnumerator.SetCancellationToken(cancellationToken);

    public async ValueTask<bool> MoveNextAsync(ITrace trace)
    {
      if (trace == null)
        throw new ArgumentNullException(nameof (trace));
      if (!await this.queryPartitionRangePageAsyncEnumerator.MoveNextAsync(trace))
      {
        this.Current = new TryCatch<QueryPage>();
        return false;
      }
      TryCatch<QueryPage> current = this.queryPartitionRangePageAsyncEnumerator.Current;
      if (current.Failed)
      {
        this.Current = current;
        return true;
      }
      QueryPage result = current.Result;
      QueryState state;
      if (result.State == null)
      {
        state = (QueryState) null;
      }
      else
      {
        UtfAnyString? nullable = result.State?.Value is CosmosString cosmosString ? new UtfAnyString?(cosmosString.Value) : new UtfAnyString?();
        state = new QueryState(OptimisticDirectExecutionContinuationToken.ToCosmosElement(new OptimisticDirectExecutionContinuationToken(new ParallelContinuationToken(nullable.HasValue ? UtfAnyString.op_Implicit(nullable.GetValueOrDefault()) : (string) null, ((FeedRangeEpk) this.queryPartitionRangePageAsyncEnumerator.FeedRangeState.FeedRange).Range))));
      }
      this.Current = TryCatch<QueryPage>.FromResult(new QueryPage(result.Documents, result.RequestCharge, result.ActivityId, result.ResponseLengthInBytes, result.CosmosQueryExecutionInfo, (string) null, result.AdditionalHeaders, state));
      return true;
    }

    public static TryCatch<IQueryPipelineStage> MonadicCreate(
      IDocumentContainer documentContainer,
      SqlQuerySpec sqlQuerySpec,
      FeedRangeEpk targetRange,
      PartitionKey? partitionKey,
      QueryPaginationOptions queryPaginationOptions,
      CosmosElement continuationToken,
      CancellationToken cancellationToken)
    {
      if (targetRange == null)
        throw new ArgumentNullException(nameof (targetRange));
      TryCatch<FeedRangeState<QueryState>> tryCatch = !(continuationToken == (CosmosElement) null) ? OptimisticDirectExecutionQueryPipelineStage.MonadicExtractState(continuationToken, targetRange) : TryCatch<FeedRangeState<QueryState>>.FromResult(new FeedRangeState<QueryState>((FeedRangeInternal) targetRange, (QueryState) null));
      if (tryCatch.Failed)
        return TryCatch<IQueryPipelineStage>.FromException(tryCatch.Exception);
      FeedRangeState<QueryState> result = tryCatch.Result;
      return TryCatch<IQueryPipelineStage>.FromResult((IQueryPipelineStage) new OptimisticDirectExecutionQueryPipelineStage(new QueryPartitionRangePageAsyncEnumerator((IQueryDataSource) documentContainer, sqlQuerySpec, result, partitionKey, queryPaginationOptions, cancellationToken)));
    }

    private static TryCatch<FeedRangeState<QueryState>> MonadicExtractState(
      CosmosElement continuationToken,
      FeedRangeEpk range)
    {
      TryCatch<OptimisticDirectExecutionContinuationToken> tryCatch = !(continuationToken == (CosmosElement) null) ? OptimisticDirectExecutionContinuationToken.TryCreateFromCosmosElement(continuationToken) : throw new ArgumentNullException(nameof (continuationToken));
      if (tryCatch.Failed)
        return TryCatch<FeedRangeState<QueryState>>.FromException(tryCatch.Exception);
      TryCatch<PartitionMapper.PartitionMapping<OptimisticDirectExecutionContinuationToken>> partitionMapping = PartitionMapper.MonadicGetPartitionMapping<OptimisticDirectExecutionContinuationToken>(range, tryCatch.Result);
      if (partitionMapping.Failed)
        return TryCatch<FeedRangeState<QueryState>>.FromException(partitionMapping.Exception);
      PartitionMapper.PartitionMapping<OptimisticDirectExecutionContinuationToken> result = partitionMapping.Result;
      KeyValuePair<FeedRangeEpk, OptimisticDirectExecutionContinuationToken> keyValuePair = new KeyValuePair<FeedRangeEpk, OptimisticDirectExecutionContinuationToken>(result.TargetMapping.Keys.First<FeedRangeEpk>(), result.TargetMapping.Values.First<OptimisticDirectExecutionContinuationToken>());
      return TryCatch<FeedRangeState<QueryState>>.FromResult(new FeedRangeState<QueryState>((FeedRangeInternal) keyValuePair.Key, keyValuePair.Value?.Token != null ? new QueryState((CosmosElement) CosmosString.Create(keyValuePair.Value.Token.Token)) : (QueryState) null));
    }
  }
}
