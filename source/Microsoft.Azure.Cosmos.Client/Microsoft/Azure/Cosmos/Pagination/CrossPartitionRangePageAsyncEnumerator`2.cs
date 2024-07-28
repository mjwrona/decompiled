// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Pagination.CrossPartitionRangePageAsyncEnumerator`2
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.Query.Core;
using Microsoft.Azure.Cosmos.Query.Core.Collections;
using Microsoft.Azure.Cosmos.Query.Core.Monads;
using Microsoft.Azure.Cosmos.Resource.CosmosExceptions;
using Microsoft.Azure.Cosmos.Tracing;
using Microsoft.Azure.Documents;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Cosmos.Pagination
{
  internal sealed class CrossPartitionRangePageAsyncEnumerator<TPage, TState> : 
    ITracingAsyncEnumerator<TryCatch<CrossFeedRangePage<TPage, TState>>>,
    IAsyncDisposable
    where TPage : Page<TState>
    where TState : State
  {
    private readonly IFeedRangeProvider feedRangeProvider;
    private readonly CreatePartitionRangePageAsyncEnumerator<TPage, TState> createPartitionRangeEnumerator;
    private readonly AsyncLazy<CrossPartitionRangePageAsyncEnumerator<TPage, TState>.IQueue<PartitionRangePageAsyncEnumerator<TPage, TState>>> lazyEnumerators;
    private CancellationToken cancellationToken;
    private FeedRangeState<TState>? nextState;

    public CrossPartitionRangePageAsyncEnumerator(
      IFeedRangeProvider feedRangeProvider,
      CreatePartitionRangePageAsyncEnumerator<TPage, TState> createPartitionRangeEnumerator,
      IComparer<PartitionRangePageAsyncEnumerator<TPage, TState>> comparer,
      int? maxConcurrency,
      PrefetchPolicy prefetchPolicy,
      CancellationToken cancellationToken,
      CrossFeedRangeState<TState> state = null)
    {
      this.feedRangeProvider = feedRangeProvider ?? throw new ArgumentNullException(nameof (feedRangeProvider));
      this.createPartitionRangeEnumerator = createPartitionRangeEnumerator ?? throw new ArgumentNullException(nameof (createPartitionRangeEnumerator));
      this.cancellationToken = cancellationToken;
      this.lazyEnumerators = new AsyncLazy<CrossPartitionRangePageAsyncEnumerator<TPage, TState>.IQueue<PartitionRangePageAsyncEnumerator<TPage, TState>>>((Func<ITrace, CancellationToken, Task<CrossPartitionRangePageAsyncEnumerator<TPage, TState>.IQueue<PartitionRangePageAsyncEnumerator<TPage, TState>>>>) ((trace, token) => CrossPartitionRangePageAsyncEnumerator<TPage, TState>.InitializeEnumeratorsAsync(feedRangeProvider, createPartitionRangeEnumerator, comparer, maxConcurrency, prefetchPolicy, state, trace, token)));
    }

    public TryCatch<CrossFeedRangePage<TPage, TState>> Current { get; private set; }

    public FeedRangeInternal CurrentRange { get; private set; }

    public async ValueTask<bool> MoveNextAsync(ITrace trace)
    {
      if (trace == null)
        throw new ArgumentNullException(nameof (trace));
      using (ITrace childTrace = trace.StartChild(nameof (MoveNextAsync), TraceComponent.Pagination, TraceLevel.Info))
      {
        CrossPartitionRangePageAsyncEnumerator<TPage, TState>.IQueue<PartitionRangePageAsyncEnumerator<TPage, TState>> enumerators = await this.lazyEnumerators.GetValueAsync(childTrace, this.cancellationToken);
        if (enumerators.Count == 0)
        {
          this.Current = new TryCatch<CrossFeedRangePage<TPage, TState>>();
          this.CurrentRange = (FeedRangeInternal) null;
          this.nextState = new FeedRangeState<TState>?();
          return false;
        }
        PartitionRangePageAsyncEnumerator<TPage, TState> currentPaginator = enumerators.Dequeue();
        currentPaginator.SetCancellationToken(this.cancellationToken);
        bool flag;
        try
        {
          flag = await currentPaginator.MoveNextAsync(childTrace);
        }
        catch
        {
          enumerators.Enqueue(currentPaginator);
          throw;
        }
        if (!flag)
          return await this.MoveNextAsync(childTrace);
        if (currentPaginator.Current.Failed)
        {
          Exception exeception = currentPaginator.Current.Exception;
          while (exeception.InnerException != null)
            exeception = exeception.InnerException;
          if (CrossPartitionRangePageAsyncEnumerator<TPage, TState>.IsSplitException(exeception))
          {
            List<FeedRangeEpk> childRangeAsync = await this.feedRangeProvider.GetChildRangeAsync(currentPaginator.FeedRangeState.FeedRange, childTrace, this.cancellationToken);
            FeedRangeState<TState> feedRangeState1;
            if (childRangeAsync.Count <= 1)
            {
              await this.feedRangeProvider.RefreshProviderAsync(childTrace, this.cancellationToken);
              IFeedRangeProvider feedRangeProvider = this.feedRangeProvider;
              feedRangeState1 = currentPaginator.FeedRangeState;
              FeedRangeInternal feedRange = feedRangeState1.FeedRange;
              ITrace trace1 = childTrace;
              CancellationToken cancellationToken = this.cancellationToken;
              childRangeAsync = await feedRangeProvider.GetChildRangeAsync(feedRange, trace1, cancellationToken);
            }
            if (childRangeAsync.Count < 1)
            {
              string message = "SDK invariant violated 4795CC37: Must have at least one EPK range in a cross partition enumerator";
              throw CosmosExceptionFactory.CreateInternalServerErrorException(message, (Microsoft.Azure.Cosmos.Headers) null, trace: childTrace, error: new Error()
              {
                Code = "SDK_invariant_violated_4795CC37",
                Message = message
              });
            }
            if (childRangeAsync.Count == 1)
            {
              enumerators.Enqueue(currentPaginator);
            }
            else
            {
              foreach (FeedRangeInternal feedRangeInternal in childRangeAsync)
              {
                CreatePartitionRangePageAsyncEnumerator<TPage, TState> partitionRangeEnumerator = this.createPartitionRangeEnumerator;
                FeedRangeInternal feedRange = feedRangeInternal;
                feedRangeState1 = currentPaginator.FeedRangeState;
                TState state = feedRangeState1.State;
                FeedRangeState<TState> feedRangeState2 = new FeedRangeState<TState>(feedRange, state);
                enumerators.Enqueue(partitionRangeEnumerator(feedRangeState2));
              }
            }
            return await this.MoveNextAsync(childTrace);
          }
          enumerators.Enqueue(currentPaginator);
          this.Current = TryCatch<CrossFeedRangePage<TPage, TState>>.FromException(currentPaginator.Current.Exception);
          this.CurrentRange = currentPaginator.FeedRangeState.FeedRange;
          this.nextState = CrossPartitionRangePageAsyncEnumerator<TPage, TState>.GetNextRange(enumerators);
          return true;
        }
        if ((object) currentPaginator.FeedRangeState.State != null)
          enumerators.Enqueue(currentPaginator);
        CrossFeedRangeState<TState> state1;
        if (enumerators.Count == 0)
        {
          state1 = (CrossFeedRangeState<TState>) null;
        }
        else
        {
          FeedRangeState<TState>[] feedRangeStateArray = new FeedRangeState<TState>[enumerators.Count];
          int num = 0;
          foreach (PartitionRangePageAsyncEnumerator<TPage, TState> pageAsyncEnumerator in (IEnumerable<PartitionRangePageAsyncEnumerator<TPage, TState>>) enumerators)
            feedRangeStateArray[num++] = pageAsyncEnumerator.FeedRangeState;
          state1 = new CrossFeedRangeState<TState>((ReadOnlyMemory<FeedRangeState<TState>>) feedRangeStateArray);
        }
        this.Current = TryCatch<CrossFeedRangePage<TPage, TState>>.FromResult(new CrossFeedRangePage<TPage, TState>(currentPaginator.Current.Result, state1));
        this.CurrentRange = currentPaginator.FeedRangeState.FeedRange;
        this.nextState = CrossPartitionRangePageAsyncEnumerator<TPage, TState>.GetNextRange(enumerators);
        return true;
      }
    }

    public ValueTask DisposeAsync() => new ValueTask();

    public bool TryPeekNext(out FeedRangeState<TState> nextState)
    {
      if (this.nextState.HasValue)
      {
        nextState = this.nextState.Value;
        return true;
      }
      nextState = new FeedRangeState<TState>();
      return false;
    }

    public void SetCancellationToken(CancellationToken cancellationToken) => this.cancellationToken = cancellationToken;

    private static bool IsSplitException(Exception exeception) => exeception is CosmosException cosmosException && cosmosException.StatusCode == HttpStatusCode.Gone && cosmosException.SubStatusCode == 1002;

    private static FeedRangeState<TState>? GetNextRange(
      CrossPartitionRangePageAsyncEnumerator<TPage, TState>.IQueue<PartitionRangePageAsyncEnumerator<TPage, TState>> enumerators)
    {
      if (enumerators == null || enumerators.Count == 0)
        return new FeedRangeState<TState>?();
      return enumerators.Peek()?.FeedRangeState;
    }

    private static async Task<CrossPartitionRangePageAsyncEnumerator<TPage, TState>.IQueue<PartitionRangePageAsyncEnumerator<TPage, TState>>> InitializeEnumeratorsAsync(
      IFeedRangeProvider feedRangeProvider,
      CreatePartitionRangePageAsyncEnumerator<TPage, TState> createPartitionRangeEnumerator,
      IComparer<PartitionRangePageAsyncEnumerator<TPage, TState>> comparer,
      int? maxConcurrency,
      PrefetchPolicy prefetchPolicy,
      CrossFeedRangeState<TState> state,
      ITrace trace,
      CancellationToken token)
    {
      ReadOnlyMemory<FeedRangeState<TState>> array;
      if (state != null)
      {
        array = state.Value;
      }
      else
      {
        List<FeedRangeEpk> feedRangesAsync = await feedRangeProvider.GetFeedRangesAsync(trace, token);
        List<FeedRangeState<TState>> feedRangeStateList = new List<FeedRangeState<TState>>(feedRangesAsync.Count);
        foreach (FeedRangeInternal feedRange in feedRangesAsync)
          feedRangeStateList.Add(new FeedRangeState<TState>(feedRange, default (TState)));
        array = (ReadOnlyMemory<FeedRangeState<TState>>) feedRangeStateList.ToArray();
      }
      IReadOnlyList<BufferedPartitionRangePageAsyncEnumeratorBase<TPage, TState>> bufferedEnumerators = CrossPartitionRangePageAsyncEnumerator<TPage, TState>.CreateBufferedEnumerators(prefetchPolicy, createPartitionRangeEnumerator, array, token);
      if (maxConcurrency.HasValue)
        await ParallelPrefetch.PrefetchInParallelAsync((IEnumerable<IPrefetcher>) bufferedEnumerators, maxConcurrency.Value, trace, token);
      CrossPartitionRangePageAsyncEnumerator<TPage, TState>.IQueue<PartitionRangePageAsyncEnumerator<TPage, TState>> queue = comparer == null ? (CrossPartitionRangePageAsyncEnumerator<TPage, TState>.IQueue<PartitionRangePageAsyncEnumerator<TPage, TState>>) new CrossPartitionRangePageAsyncEnumerator<TPage, TState>.QueueWrapper<PartitionRangePageAsyncEnumerator<TPage, TState>>(new Queue<PartitionRangePageAsyncEnumerator<TPage, TState>>((IEnumerable<PartitionRangePageAsyncEnumerator<TPage, TState>>) bufferedEnumerators)) : (CrossPartitionRangePageAsyncEnumerator<TPage, TState>.IQueue<PartitionRangePageAsyncEnumerator<TPage, TState>>) new CrossPartitionRangePageAsyncEnumerator<TPage, TState>.PriorityQueueWrapper<PartitionRangePageAsyncEnumerator<TPage, TState>>(new PriorityQueue<PartitionRangePageAsyncEnumerator<TPage, TState>>((IEnumerable<PartitionRangePageAsyncEnumerator<TPage, TState>>) bufferedEnumerators, comparer));
      bufferedEnumerators = (IReadOnlyList<BufferedPartitionRangePageAsyncEnumeratorBase<TPage, TState>>) null;
      return queue;
    }

    private static IReadOnlyList<BufferedPartitionRangePageAsyncEnumeratorBase<TPage, TState>> CreateBufferedEnumerators(
      PrefetchPolicy policy,
      CreatePartitionRangePageAsyncEnumerator<TPage, TState> createPartitionRangeEnumerator,
      ReadOnlyMemory<FeedRangeState<TState>> rangeAndStates,
      CancellationToken cancellationToken)
    {
      List<BufferedPartitionRangePageAsyncEnumeratorBase<TPage, TState>> bufferedEnumerators = new List<BufferedPartitionRangePageAsyncEnumeratorBase<TPage, TState>>(rangeAndStates.Length);
      for (int index = 0; index < rangeAndStates.Length; ++index)
      {
        FeedRangeState<TState> feedRangeState = rangeAndStates.Span[index];
        PartitionRangePageAsyncEnumerator<TPage, TState> enumerator = createPartitionRangeEnumerator(feedRangeState);
        BufferedPartitionRangePageAsyncEnumeratorBase<TPage, TState> asyncEnumeratorBase1;
        if (policy != PrefetchPolicy.PrefetchSinglePage)
        {
          if (policy != PrefetchPolicy.PrefetchAll)
            throw new ArgumentOutOfRangeException(nameof (policy));
          asyncEnumeratorBase1 = (BufferedPartitionRangePageAsyncEnumeratorBase<TPage, TState>) new FullyBufferedPartitionRangeAsyncEnumerator<TPage, TState>(enumerator, cancellationToken);
        }
        else
          asyncEnumeratorBase1 = (BufferedPartitionRangePageAsyncEnumeratorBase<TPage, TState>) new BufferedPartitionRangePageAsyncEnumerator<TPage, TState>(enumerator, cancellationToken);
        BufferedPartitionRangePageAsyncEnumeratorBase<TPage, TState> asyncEnumeratorBase2 = asyncEnumeratorBase1;
        bufferedEnumerators.Add(asyncEnumeratorBase2);
      }
      return (IReadOnlyList<BufferedPartitionRangePageAsyncEnumeratorBase<TPage, TState>>) bufferedEnumerators;
    }

    private interface IQueue<T> : IEnumerable<T>, IEnumerable
    {
      T Peek();

      void Enqueue(T item);

      T Dequeue();

      int Count { get; }
    }

    private sealed class PriorityQueueWrapper<T> : 
      CrossPartitionRangePageAsyncEnumerator<TPage, TState>.IQueue<T>,
      IEnumerable<T>,
      IEnumerable
    {
      private readonly PriorityQueue<T> implementation;

      public PriorityQueueWrapper(PriorityQueue<T> implementation) => this.implementation = implementation ?? throw new ArgumentNullException(nameof (implementation));

      public int Count => this.implementation.Count;

      public T Dequeue() => this.implementation.Dequeue();

      public void Enqueue(T item) => this.implementation.Enqueue(item);

      public T Peek() => this.implementation.Peek();

      public IEnumerator<T> GetEnumerator() => this.implementation.GetEnumerator();

      IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.implementation.GetEnumerator();
    }

    private sealed class QueueWrapper<T> : 
      CrossPartitionRangePageAsyncEnumerator<TPage, TState>.IQueue<T>,
      IEnumerable<T>,
      IEnumerable
    {
      private readonly Queue<T> implementation;

      public QueueWrapper(Queue<T> implementation) => this.implementation = implementation ?? throw new ArgumentNullException(nameof (implementation));

      public int Count => this.implementation.Count;

      public T Dequeue() => this.implementation.Dequeue();

      public void Enqueue(T item) => this.implementation.Enqueue(item);

      public T Peek() => this.implementation.Peek();

      public IEnumerator<T> GetEnumerator() => (IEnumerator<T>) this.implementation.GetEnumerator();

      IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.implementation.GetEnumerator();
    }
  }
}
