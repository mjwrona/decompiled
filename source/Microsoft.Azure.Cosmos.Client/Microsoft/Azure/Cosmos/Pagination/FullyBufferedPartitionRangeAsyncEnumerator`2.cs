// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Pagination.FullyBufferedPartitionRangeAsyncEnumerator`2
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.Query.Core.Monads;
using Microsoft.Azure.Cosmos.Tracing;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Cosmos.Pagination
{
  internal sealed class FullyBufferedPartitionRangeAsyncEnumerator<TPage, TState> : 
    BufferedPartitionRangePageAsyncEnumeratorBase<TPage, TState>
    where TPage : Page<TState>
    where TState : State
  {
    private readonly PartitionRangePageAsyncEnumerator<TPage, TState> enumerator;
    private readonly List<TPage> bufferedPages;
    private int currentIndex;
    private Exception exception;

    private bool HasPrefetched => this.exception != null || this.bufferedPages.Count > 0;

    public FullyBufferedPartitionRangeAsyncEnumerator(
      PartitionRangePageAsyncEnumerator<TPage, TState> enumerator,
      CancellationToken cancellationToken)
      : base(enumerator.FeedRangeState, cancellationToken)
    {
      this.enumerator = enumerator ?? throw new ArgumentNullException(nameof (enumerator));
      this.bufferedPages = new List<TPage>();
    }

    public override ValueTask DisposeAsync() => this.enumerator.DisposeAsync();

    public override async ValueTask PrefetchAsync(ITrace trace, CancellationToken cancellationToken)
    {
      if (trace == null)
        throw new ArgumentNullException(nameof (trace));
      if (this.HasPrefetched)
        return;
      cancellationToken.ThrowIfCancellationRequested();
      ITrace prefetchTrace = trace.StartChild("Prefetch", TraceComponent.Pagination, TraceLevel.Info);
      try
      {
        TryCatch<TPage> current;
        while (true)
        {
          if (await this.enumerator.MoveNextAsync(prefetchTrace))
          {
            cancellationToken.ThrowIfCancellationRequested();
            current = this.enumerator.Current;
            if (current.Succeeded)
              this.bufferedPages.Add(current.Result);
            else
              break;
          }
          else
            goto label_13;
        }
        this.exception = current.Exception;
      }
      finally
      {
        prefetchTrace?.Dispose();
      }
label_13:
      prefetchTrace = (ITrace) null;
    }

    protected override async Task<TryCatch<TPage>> GetNextPageAsync(
      ITrace trace,
      CancellationToken cancellationToken)
    {
      TryCatch<TPage> nextPageAsync;
      if (this.currentIndex < this.bufferedPages.Count)
        nextPageAsync = TryCatch<TPage>.FromResult(this.bufferedPages[this.currentIndex]);
      else if (this.currentIndex == this.bufferedPages.Count && this.exception != null)
      {
        nextPageAsync = TryCatch<TPage>.FromException(this.exception);
      }
      else
      {
        int num = await this.enumerator.MoveNextAsync(trace) ? 1 : 0;
        nextPageAsync = this.enumerator.Current;
      }
      ++this.currentIndex;
      return nextPageAsync;
    }

    public override void SetCancellationToken(CancellationToken cancellationToken)
    {
      base.SetCancellationToken(cancellationToken);
      this.enumerator.SetCancellationToken(cancellationToken);
    }
  }
}
