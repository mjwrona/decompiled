// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.FeedIteratorCore`1
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
  internal sealed class FeedIteratorCore<T> : FeedIteratorInternal<T>
  {
    private readonly FeedIteratorInternal feedIterator;
    private readonly Func<ResponseMessage, FeedResponse<T>> responseCreator;

    internal FeedIteratorCore(
      FeedIteratorInternal feedIterator,
      Func<ResponseMessage, FeedResponse<T>> responseCreator)
    {
      this.responseCreator = responseCreator;
      this.feedIterator = feedIterator;
      this.databaseName = feedIterator.databaseName;
      this.container = feedIterator.container;
    }

    public override bool HasMoreResults => this.feedIterator.HasMoreResults;

    public override CosmosElement GetCosmosElementContinuationToken() => this.feedIterator.GetCosmosElementContinuationToken();

    public override Task<FeedResponse<T>> ReadNextAsync(CancellationToken cancellationToken = default (CancellationToken)) => TaskHelper.RunInlineIfNeededAsync<FeedResponse<T>>((Func<Task<FeedResponse<T>>>) (() => this.ReadNextWithRootTraceAsync(cancellationToken)));

    private async Task<FeedResponse<T>> ReadNextWithRootTraceAsync(
      CancellationToken cancellationToken = default (CancellationToken))
    {
      FeedResponse<T> feedResponse;
      using (ITrace trace = (ITrace) Trace.GetRootTrace("FeedIteratorCore ReadNextAsync", TraceComponent.Unknown, TraceLevel.Info))
        feedResponse = await this.ReadNextAsync(trace, cancellationToken);
      return feedResponse;
    }

    public override async Task<FeedResponse<T>> ReadNextAsync(
      ITrace trace,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      if (trace == null)
        throw new ArgumentNullException(nameof (trace));
      return this.responseCreator(await this.feedIterator.ReadNextAsync(trace, cancellationToken));
    }

    protected override void Dispose(bool disposing)
    {
      this.feedIterator.Dispose();
      base.Dispose(disposing);
    }
  }
}
