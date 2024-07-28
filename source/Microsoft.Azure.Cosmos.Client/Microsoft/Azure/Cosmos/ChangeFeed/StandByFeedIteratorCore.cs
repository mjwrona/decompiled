// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.ChangeFeed.StandByFeedIteratorCore
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.CosmosElements;
using Microsoft.Azure.Cosmos.Routing;
using Microsoft.Azure.Cosmos.Tracing;
using Microsoft.Azure.Documents;
using System;
using System.Globalization;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Cosmos.ChangeFeed
{
  internal class StandByFeedIteratorCore : FeedIteratorInternal
  {
    internal StandByFeedContinuationToken compositeContinuationToken;
    private readonly CosmosClientContext clientContext;
    private string containerRid;
    private string continuationToken;
    private int? maxItemCount;
    protected readonly StandByFeedIteratorRequestOptions changeFeedOptions;

    internal StandByFeedIteratorCore(
      CosmosClientContext clientContext,
      ContainerInternal container,
      string continuationToken,
      int? maxItemCount,
      StandByFeedIteratorRequestOptions options)
    {
      if (container == null)
        throw new ArgumentNullException(nameof (container));
      this.clientContext = clientContext;
      this.container = container;
      this.changeFeedOptions = options;
      this.maxItemCount = maxItemCount;
      this.continuationToken = continuationToken;
    }

    public override bool HasMoreResults => true;

    public override Task<ResponseMessage> ReadNextAsync(CancellationToken cancellationToken = default (CancellationToken)) => this.ReadNextAsync((ITrace) NoOpTrace.Singleton, cancellationToken);

    public override async Task<ResponseMessage> ReadNextAsync(
      ITrace trace,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      cancellationToken.ThrowIfCancellationRequested();
      if (trace == null)
        throw new ArgumentNullException(nameof (trace));
      string firstNotModifiedKeyRangeId = (string) null;
      string currentKeyRangeId;
      ResponseMessage response;
      string str1;
      do
      {
        string str2;
        ResponseMessage responseMessage;
        (await this.ReadNextInternalAsync(trace, cancellationToken)).Deconstruct<string, ResponseMessage>(out str2, out responseMessage);
        currentKeyRangeId = str2;
        response = responseMessage;
        this.compositeContinuationToken.MoveToNextToken();
        (await this.compositeContinuationToken.GetCurrentTokenAsync()).Deconstruct<CompositeContinuationToken, string>(out CompositeContinuationToken _, out str2);
        str1 = str2;
        if (response.StatusCode == HttpStatusCode.NotModified)
        {
          if (string.IsNullOrEmpty(firstNotModifiedKeyRangeId))
            firstNotModifiedKeyRangeId = currentKeyRangeId;
        }
        else
          break;
      }
      while (!firstNotModifiedKeyRangeId.Equals(str1, StringComparison.InvariantCultureIgnoreCase));
      response.Headers.ContinuationToken = this.compositeContinuationToken.ToString();
      ResponseMessage responseMessage1 = response;
      firstNotModifiedKeyRangeId = (string) null;
      currentKeyRangeId = (string) null;
      response = (ResponseMessage) null;
      return responseMessage1;
    }

    internal async Task<Tuple<string, ResponseMessage>> ReadNextInternalAsync(
      ITrace trace,
      CancellationToken cancellationToken)
    {
      StandByFeedIteratorCore feedIteratorCore = this;
      cancellationToken.ThrowIfCancellationRequested();
      if (trace == null)
        throw new ArgumentNullException(nameof (trace));
      string cachedRidAsync;
      if (feedIteratorCore.compositeContinuationToken == null)
      {
        PartitionKeyRangeCache pkRangeCache = await feedIteratorCore.clientContext.DocumentClient.GetPartitionKeyRangeCacheAsync(trace);
        cachedRidAsync = await feedIteratorCore.container.GetCachedRIDAsync(false, trace, cancellationToken);
        feedIteratorCore.containerRid = cachedRidAsync;
        StandByFeedContinuationToken async = await StandByFeedContinuationToken.CreateAsync(feedIteratorCore.containerRid, feedIteratorCore.continuationToken, new StandByFeedContinuationToken.PartitionKeyRangeCacheDelegate(pkRangeCache.TryGetOverlappingRangesAsync));
        feedIteratorCore.compositeContinuationToken = async;
        pkRangeCache = (PartitionKeyRangeCache) null;
      }
      CompositeContinuationToken continuationToken;
      (await feedIteratorCore.compositeContinuationToken.GetCurrentTokenAsync()).Deconstruct<CompositeContinuationToken, string>(out continuationToken, out cachedRidAsync);
      CompositeContinuationToken currentRangeToken = continuationToken;
      string partitionKeyRangeId = cachedRidAsync;
      feedIteratorCore.continuationToken = currentRangeToken.Token;
      ResponseMessage response = await feedIteratorCore.NextResultSetDelegateAsync(feedIteratorCore.continuationToken, partitionKeyRangeId, feedIteratorCore.maxItemCount, feedIteratorCore.changeFeedOptions, trace, cancellationToken);
      if (await feedIteratorCore.ShouldRetryFailureAsync(response, cancellationToken))
        return await feedIteratorCore.ReadNextInternalAsync(trace, cancellationToken);
      if (response.IsSuccessStatusCode || response.StatusCode == HttpStatusCode.NotModified)
        currentRangeToken.Token = response.Headers.ETag;
      return new Tuple<string, ResponseMessage>(partitionKeyRangeId, response);
    }

    internal async Task<bool> ShouldRetryFailureAsync(
      ResponseMessage response,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      if (response.IsSuccessStatusCode || response.StatusCode == HttpStatusCode.NotModified || (response.StatusCode != HttpStatusCode.Gone ? 0 : (response.Headers.SubStatusCode == SubStatusCodes.PartitionKeyRangeGone ? 1 : (response.Headers.SubStatusCode == SubStatusCodes.CompletingSplit ? 1 : 0))) == 0)
        return false;
      Tuple<CompositeContinuationToken, string> currentTokenAsync = await this.compositeContinuationToken.GetCurrentTokenAsync(true);
      return true;
    }

    internal virtual Task<ResponseMessage> NextResultSetDelegateAsync(
      string continuationToken,
      string partitionKeyRangeId,
      int? maxItemCount,
      StandByFeedIteratorRequestOptions options,
      ITrace trace,
      CancellationToken cancellationToken)
    {
      return this.clientContext.ProcessResourceOperationAsync<ResponseMessage>(this.container.LinkUri, ResourceType.Document, Microsoft.Azure.Documents.OperationType.ReadFeed, (RequestOptions) options, this.container, (FeedRange) null, (Stream) null, (Action<RequestMessage>) (request =>
      {
        if (!string.IsNullOrWhiteSpace(continuationToken))
          request.Headers.IfNoneMatch = continuationToken;
        if (maxItemCount.HasValue)
          request.Headers.PageSize = maxItemCount.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        if (string.IsNullOrEmpty(partitionKeyRangeId))
          return;
        request.PartitionKeyRangeId = new PartitionKeyRangeIdentity(partitionKeyRangeId);
      }), (Func<ResponseMessage, ResponseMessage>) (response => response), trace, cancellationToken);
    }

    public override CosmosElement GetCosmosElementContinuationToken() => throw new NotImplementedException();
  }
}
