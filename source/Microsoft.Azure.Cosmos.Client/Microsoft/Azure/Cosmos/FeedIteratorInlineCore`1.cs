// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.FeedIteratorInlineCore`1
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.CosmosElements;
using Microsoft.Azure.Cosmos.Telemetry;
using Microsoft.Azure.Cosmos.Tracing;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Cosmos
{
  internal sealed class FeedIteratorInlineCore<T> : FeedIteratorInternal<T>
  {
    private readonly FeedIteratorInternal<T> feedIteratorInternal;
    private readonly CosmosClientContext clientContext;

    internal FeedIteratorInlineCore(FeedIterator<T> feedIterator, CosmosClientContext clientContext)
    {
      this.feedIteratorInternal = feedIterator is FeedIteratorInternal<T> iteratorInternal ? iteratorInternal : throw new ArgumentNullException(nameof (feedIterator));
      this.clientContext = clientContext;
      this.container = iteratorInternal.container;
      this.databaseName = iteratorInternal.databaseName;
    }

    internal FeedIteratorInlineCore(
      FeedIteratorInternal<T> feedIteratorInternal,
      CosmosClientContext clientContext)
    {
      this.feedIteratorInternal = feedIteratorInternal ?? throw new ArgumentNullException(nameof (feedIteratorInternal));
      this.clientContext = clientContext;
      this.container = feedIteratorInternal.container;
      this.databaseName = feedIteratorInternal.databaseName;
    }

    public override bool HasMoreResults => this.feedIteratorInternal.HasMoreResults;

    public override Task<FeedResponse<T>> ReadNextAsync(CancellationToken cancellationToken = default (CancellationToken)) => this.clientContext.OperationHelperAsync<FeedResponse<T>>("Typed FeedIterator ReadNextAsync", (RequestOptions) null, (Func<ITrace, Task<FeedResponse<T>>>) (trace => this.feedIteratorInternal.ReadNextAsync(trace, cancellationToken)), (Func<FeedResponse<T>, OpenTelemetryAttributes>) (response => this.container == null ? (OpenTelemetryAttributes) new OpenTelemetryResponse<T>(response, databaseName: this.databaseName) : (OpenTelemetryAttributes) new OpenTelemetryResponse<T>(response, this.container?.Id, this.container?.Database?.Id ?? this.databaseName)));

    public override Task<FeedResponse<T>> ReadNextAsync(
      ITrace trace,
      CancellationToken cancellationToken)
    {
      return TaskHelper.RunInlineIfNeededAsync<FeedResponse<T>>((Func<Task<FeedResponse<T>>>) (() => this.feedIteratorInternal.ReadNextAsync(trace, cancellationToken)));
    }

    public override CosmosElement GetCosmosElementContinuationToken() => this.feedIteratorInternal.GetCosmosElementContinuationToken();

    protected override void Dispose(bool disposing)
    {
      this.feedIteratorInternal.Dispose();
      base.Dispose(disposing);
    }
  }
}
