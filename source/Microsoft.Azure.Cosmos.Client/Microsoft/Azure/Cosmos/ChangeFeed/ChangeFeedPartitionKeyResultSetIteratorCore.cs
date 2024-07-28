// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.ChangeFeed.ChangeFeedPartitionKeyResultSetIteratorCore
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.ChangeFeed.LeaseManagement;
using Microsoft.Azure.Cosmos.CosmosElements;
using Microsoft.Azure.Cosmos.Telemetry;
using Microsoft.Azure.Cosmos.Tracing;
using Microsoft.Azure.Documents;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Cosmos.ChangeFeed
{
  internal sealed class ChangeFeedPartitionKeyResultSetIteratorCore : FeedIteratorInternal
  {
    private readonly CosmosClientContext clientContext;
    private readonly ChangeFeedRequestOptions changeFeedOptions;
    private ChangeFeedStartFrom changeFeedStartFrom;
    private bool hasMoreResultsInternal;

    public static ChangeFeedPartitionKeyResultSetIteratorCore Create(
      DocumentServiceLease lease,
      string continuationToken,
      int? maxItemCount,
      ContainerInternal container,
      DateTime? startTime,
      bool startFromBeginning)
    {
      FeedRangeInternal feedRange = lease is DocumentServiceLeaseCoreEpk ? lease.FeedRange : (FeedRangeInternal) new FeedRangePartitionKeyRange(lease.CurrentLeaseToken);
      ChangeFeedStartFrom changeFeedStartFrom = continuationToken == null ? (!startTime.HasValue ? (!startFromBeginning ? ChangeFeedStartFrom.Now((FeedRange) feedRange) : ChangeFeedStartFrom.Beginning((FeedRange) feedRange)) : ChangeFeedStartFrom.Time(startTime.Value, (FeedRange) feedRange)) : (ChangeFeedStartFrom) new ChangeFeedStartFromContinuationAndFeedRange(continuationToken, feedRange);
      ChangeFeedRequestOptions options = new ChangeFeedRequestOptions()
      {
        PageSizeHint = maxItemCount
      };
      return new ChangeFeedPartitionKeyResultSetIteratorCore(container, changeFeedStartFrom, options);
    }

    private ChangeFeedPartitionKeyResultSetIteratorCore(
      ContainerInternal container,
      ChangeFeedStartFrom changeFeedStartFrom,
      ChangeFeedRequestOptions options)
    {
      this.container = container ?? throw new ArgumentNullException(nameof (container));
      this.changeFeedStartFrom = changeFeedStartFrom ?? throw new ArgumentNullException(nameof (changeFeedStartFrom));
      this.clientContext = this.container.ClientContext;
      this.changeFeedOptions = options;
    }

    public override bool HasMoreResults => this.hasMoreResultsInternal;

    public override CosmosElement GetCosmosElementContinuationToken() => throw new NotImplementedException();

    public override Task<ResponseMessage> ReadNextAsync(CancellationToken cancellationToken = default (CancellationToken)) => this.clientContext.OperationHelperAsync<ResponseMessage>("Change Feed Processor Read Next Async", (RequestOptions) this.changeFeedOptions, (Func<ITrace, Task<ResponseMessage>>) (trace => this.ReadNextAsync(trace, cancellationToken)), (Func<ResponseMessage, OpenTelemetryAttributes>) (response => (OpenTelemetryAttributes) new OpenTelemetryResponse(response, this.container?.Id, this.container?.Database?.Id)), TraceComponent.ChangeFeed);

    public override async Task<ResponseMessage> ReadNextAsync(
      ITrace trace,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      ChangeFeedPartitionKeyResultSetIteratorCore resultSetIteratorCore = this;
      CosmosClientContext clientContext = resultSetIteratorCore.clientContext;
      ContainerInternal container = resultSetIteratorCore.container;
      string linkUri = resultSetIteratorCore.container.LinkUri;
      ChangeFeedRequestOptions changeFeedOptions = resultSetIteratorCore.changeFeedOptions;
      ContainerInternal cosmosContainerCore = container;
      FeedRange feedRange = resultSetIteratorCore.changeFeedStartFrom.FeedRange;
      // ISSUE: reference to a compiler-generated method
      Action<RequestMessage> requestEnricher = new Action<RequestMessage>(resultSetIteratorCore.\u003CReadNextAsync\u003Eb__10_0);
      ITrace trace1 = trace;
      CancellationToken cancellationToken1 = cancellationToken;
      ResponseMessage responseMessage = await clientContext.ProcessResourceOperationStreamAsync(linkUri, ResourceType.Document, OperationType.ReadFeed, (RequestOptions) changeFeedOptions, cosmosContainerCore, feedRange, (Stream) null, requestEnricher, trace1, cancellationToken1);
      string etag = responseMessage.Headers.ETag;
      resultSetIteratorCore.hasMoreResultsInternal = responseMessage.IsSuccessStatusCode;
      responseMessage.Headers.ContinuationToken = etag;
      resultSetIteratorCore.changeFeedStartFrom = (ChangeFeedStartFrom) new ChangeFeedStartFromContinuationAndFeedRange(etag, (FeedRangeInternal) resultSetIteratorCore.changeFeedStartFrom.FeedRange);
      return responseMessage;
    }
  }
}
