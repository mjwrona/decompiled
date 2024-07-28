// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.FeedIteratorInlineCore
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
  internal sealed class FeedIteratorInlineCore : FeedIteratorInternal
  {
    private readonly FeedIteratorInternal feedIteratorInternal;
    private readonly CosmosClientContext clientContext;

    internal FeedIteratorInlineCore(FeedIterator feedIterator, CosmosClientContext clientContext)
    {
      this.feedIteratorInternal = feedIterator is FeedIteratorInternal iteratorInternal ? iteratorInternal : throw new ArgumentNullException(nameof (feedIterator));
      this.clientContext = clientContext;
      this.container = iteratorInternal.container;
      this.databaseName = iteratorInternal.databaseName;
    }

    internal FeedIteratorInlineCore(
      FeedIteratorInternal feedIteratorInternal,
      CosmosClientContext clientContext)
    {
      this.feedIteratorInternal = feedIteratorInternal ?? throw new ArgumentNullException(nameof (feedIteratorInternal));
      this.clientContext = clientContext;
      this.container = feedIteratorInternal.container;
      this.databaseName = feedIteratorInternal.databaseName;
    }

    public override bool HasMoreResults => this.feedIteratorInternal.HasMoreResults;

    public override CosmosElement GetCosmosElementContinuationToken() => this.feedIteratorInternal.GetCosmosElementContinuationToken();

    public override Task<ResponseMessage> ReadNextAsync(CancellationToken cancellationToken = default (CancellationToken)) => this.clientContext.OperationHelperAsync<ResponseMessage>("FeedIterator Read Next Async", (RequestOptions) null, (Func<ITrace, Task<ResponseMessage>>) (trace => this.feedIteratorInternal.ReadNextAsync(trace, cancellationToken)), (Func<ResponseMessage, OpenTelemetryAttributes>) (response => this.container == null ? (OpenTelemetryAttributes) new OpenTelemetryResponse(response, databaseName: this.databaseName) : (OpenTelemetryAttributes) new OpenTelemetryResponse(response, this.container?.Id, this.container?.Database?.Id ?? this.databaseName)));

    public override Task<ResponseMessage> ReadNextAsync(
      ITrace trace,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return TaskHelper.RunInlineIfNeededAsync<ResponseMessage>((Func<Task<ResponseMessage>>) (() => this.feedIteratorInternal.ReadNextAsync(trace, cancellationToken)));
    }

    protected override void Dispose(bool disposing)
    {
      this.feedIteratorInternal.Dispose();
      base.Dispose(disposing);
    }
  }
}
