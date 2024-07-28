// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.FeedIteratorInternal`1
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.CosmosElements;
using Microsoft.Azure.Cosmos.Tracing;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Cosmos
{
  internal abstract class FeedIteratorInternal<T> : FeedIterator<T>
  {
    public override Task<FeedResponse<T>> ReadNextAsync(CancellationToken cancellationToken = default (CancellationToken)) => TaskHelper.RunInlineIfNeededAsync<FeedResponse<T>>((Func<Task<FeedResponse<T>>>) (() => this.ReadNextWithRootTraceAsync(cancellationToken)));

    private async Task<FeedResponse<T>> ReadNextWithRootTraceAsync(
      CancellationToken cancellationToken = default (CancellationToken))
    {
      FeedResponse<T> feedResponse;
      using (ITrace trace = (ITrace) Trace.GetRootTrace("Typed FeedIterator ReadNextAsync", TraceComponent.Unknown, TraceLevel.Info))
        feedResponse = await this.ReadNextAsync(trace, cancellationToken);
      return feedResponse;
    }

    public abstract Task<FeedResponse<T>> ReadNextAsync(
      ITrace trace,
      CancellationToken cancellationToken);

    public abstract CosmosElement GetCosmosElementContinuationToken();
  }
}
