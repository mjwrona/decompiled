// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Pagination.PartitionRangePageAsyncEnumerator`2
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.Query.Core.Monads;
using Microsoft.Azure.Cosmos.Tracing;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Cosmos.Pagination
{
  internal abstract class PartitionRangePageAsyncEnumerator<TPage, TState> : 
    ITracingAsyncEnumerator<TryCatch<TPage>>,
    IAsyncDisposable
    where TPage : Page<TState>
    where TState : State
  {
    private CancellationToken cancellationToken;

    protected PartitionRangePageAsyncEnumerator(
      Microsoft.Azure.Cosmos.Pagination.FeedRangeState<TState> feedRangeState,
      CancellationToken cancellationToken)
    {
      this.FeedRangeState = feedRangeState;
      this.cancellationToken = cancellationToken;
    }

    public Microsoft.Azure.Cosmos.Pagination.FeedRangeState<TState> FeedRangeState { get; private set; }

    public TryCatch<TPage> Current { get; private set; }

    public bool HasStarted { get; private set; }

    private bool HasMoreResults => !this.HasStarted || (object) this.FeedRangeState.State != null;

    public async ValueTask<bool> MoveNextAsync(ITrace trace)
    {
      PartitionRangePageAsyncEnumerator<TPage, TState> pageAsyncEnumerator1 = this;
      ITrace trace1 = trace != null ? trace : throw new ArgumentNullException(nameof (trace));
      Microsoft.Azure.Cosmos.Pagination.FeedRangeState<TState> feedRangeState1 = pageAsyncEnumerator1.FeedRangeState;
      string name = string.Format("{0} move next", (object) feedRangeState1.FeedRange);
      using (ITrace childTrace = trace1.StartChild(name, TraceComponent.Pagination, TraceLevel.Info))
      {
        if (!pageAsyncEnumerator1.HasMoreResults)
          return false;
        TryCatch<TPage> nextPageAsync = await pageAsyncEnumerator1.GetNextPageAsync(childTrace, pageAsyncEnumerator1.cancellationToken);
        pageAsyncEnumerator1.Current = nextPageAsync;
        // ISSUE: explicit non-virtual call
        if (__nonvirtual (pageAsyncEnumerator1.Current).Succeeded)
        {
          PartitionRangePageAsyncEnumerator<TPage, TState> pageAsyncEnumerator2 = pageAsyncEnumerator1;
          feedRangeState1 = pageAsyncEnumerator1.FeedRangeState;
          // ISSUE: explicit non-virtual call
          Microsoft.Azure.Cosmos.Pagination.FeedRangeState<TState> feedRangeState2 = new Microsoft.Azure.Cosmos.Pagination.FeedRangeState<TState>(feedRangeState1.FeedRange, __nonvirtual (pageAsyncEnumerator1.Current).Result.State);
          pageAsyncEnumerator2.FeedRangeState = feedRangeState2;
          pageAsyncEnumerator1.HasStarted = true;
        }
        return true;
      }
    }

    protected abstract Task<TryCatch<TPage>> GetNextPageAsync(
      ITrace trace,
      CancellationToken cancellationToken);

    public abstract ValueTask DisposeAsync();

    public virtual void SetCancellationToken(CancellationToken cancellationToken) => this.cancellationToken = cancellationToken;
  }
}
