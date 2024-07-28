// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Pagination.BufferedPartitionRangePageAsyncEnumerator`2
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
  internal sealed class BufferedPartitionRangePageAsyncEnumerator<TPage, TState> : 
    BufferedPartitionRangePageAsyncEnumeratorBase<TPage, TState>
    where TPage : Page<TState>
    where TState : State
  {
    private readonly PartitionRangePageAsyncEnumerator<TPage, TState> enumerator;
    private TryCatch<TPage>? bufferedPage;

    public BufferedPartitionRangePageAsyncEnumerator(
      PartitionRangePageAsyncEnumerator<TPage, TState> enumerator,
      CancellationToken cancellationToken)
      : base(enumerator.FeedRangeState, cancellationToken)
    {
      this.enumerator = enumerator ?? throw new ArgumentNullException(nameof (enumerator));
    }

    public override ValueTask DisposeAsync() => this.enumerator.DisposeAsync();

    protected override async Task<TryCatch<TPage>> GetNextPageAsync(
      ITrace trace,
      CancellationToken cancellationToken)
    {
      BufferedPartitionRangePageAsyncEnumerator<TPage, TState> pageAsyncEnumerator = this;
      if (trace == null)
        throw new ArgumentNullException(nameof (trace));
      await pageAsyncEnumerator.PrefetchAsync(trace, cancellationToken);
      TryCatch<TPage> nextPageAsync = pageAsyncEnumerator.bufferedPage.Value;
      pageAsyncEnumerator.bufferedPage = new TryCatch<TPage>?();
      return nextPageAsync;
    }

    public override async ValueTask PrefetchAsync(ITrace trace, CancellationToken cancellationToken)
    {
      if (trace == null)
        throw new ArgumentNullException(nameof (trace));
      if (this.bufferedPage.HasValue)
        return;
      cancellationToken.ThrowIfCancellationRequested();
      using (ITrace prefetchTrace = trace.StartChild("Prefetch", TraceComponent.Pagination, TraceLevel.Info))
      {
        int num = await this.enumerator.MoveNextAsync(prefetchTrace) ? 1 : 0;
        this.bufferedPage = new TryCatch<TPage>?(this.enumerator.Current);
      }
    }

    public override void SetCancellationToken(CancellationToken cancellationToken)
    {
      base.SetCancellationToken(cancellationToken);
      this.enumerator.SetCancellationToken(cancellationToken);
    }
  }
}
