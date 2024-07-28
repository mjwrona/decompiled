// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.CosmosOffers
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.Handlers;
using Microsoft.Azure.Cosmos.Resource.CosmosExceptions;
using Microsoft.Azure.Cosmos.Tracing;
using Microsoft.Azure.Documents;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Cosmos
{
  internal class CosmosOffers
  {
    private readonly CosmosClientContext ClientContext;
    private readonly string OfferRootUri = "//offers/";

    public CosmosOffers(CosmosClientContext clientContext) => this.ClientContext = clientContext;

    internal async Task<ThroughputResponse> ReadThroughputAsync(
      string targetRID,
      RequestOptions requestOptions,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      (OfferV2, double) offerV2Async = await this.GetOfferV2Async<OfferV2>(targetRID, true, cancellationToken);
      return await this.GetThroughputResponseAsync((Stream) null, OperationType.Read, new Uri(offerV2Async.Item1.SelfLink, UriKind.Relative), ResourceType.Offer, offerV2Async.Item2, requestOptions, cancellationToken);
    }

    internal async Task<ThroughputResponse> ReadThroughputIfExistsAsync(
      string targetRID,
      RequestOptions requestOptions,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      (OfferV2 offerV2, double currentRequestCharge) = await this.GetOfferV2Async<OfferV2>(targetRID, false, cancellationToken);
      return offerV2 == null ? new ThroughputResponse(HttpStatusCode.NotFound, (Headers) null, (ThroughputProperties) null, (CosmosDiagnostics) null, (RequestMessage) null) : await this.GetThroughputResponseAsync((Stream) null, OperationType.Read, new Uri(offerV2.SelfLink, UriKind.Relative), ResourceType.Offer, currentRequestCharge, requestOptions, cancellationToken);
    }

    internal async Task<ThroughputResponse> ReplaceThroughputPropertiesAsync(
      string targetRID,
      ThroughputProperties throughputProperties,
      RequestOptions requestOptions,
      CancellationToken cancellationToken)
    {
      (ThroughputProperties input, double currentRequestCharge) = await this.GetOfferV2Async<ThroughputProperties>(targetRID, true, cancellationToken);
      input.Content = throughputProperties.Content;
      return await this.GetThroughputResponseAsync(this.ClientContext.SerializerCore.ToStream<ThroughputProperties>(input), OperationType.Replace, new Uri(input.SelfLink, UriKind.Relative), ResourceType.Offer, currentRequestCharge, requestOptions, cancellationToken);
    }

    internal async Task<ThroughputResponse> ReplaceThroughputPropertiesIfExistsAsync(
      string targetRID,
      ThroughputProperties throughputProperties,
      RequestOptions requestOptions,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      try
      {
        (ThroughputProperties input, double currentRequestCharge) = await this.GetOfferV2Async<ThroughputProperties>(targetRID, false, cancellationToken);
        if (input == null)
        {
          CosmosException notFoundException = CosmosExceptionFactory.CreateNotFoundException("Throughput is not configured for " + targetRID, new Headers()
          {
            RequestCharge = currentRequestCharge
          });
          return new ThroughputResponse(notFoundException.StatusCode, notFoundException.Headers, (ThroughputProperties) null, notFoundException.Diagnostics, (RequestMessage) null);
        }
        input.Content = throughputProperties.Content;
        return await this.GetThroughputResponseAsync(this.ClientContext.SerializerCore.ToStream<ThroughputProperties>(input), OperationType.Replace, new Uri(input.SelfLink, UriKind.Relative), ResourceType.Offer, currentRequestCharge, requestOptions, cancellationToken);
      }
      catch (DocumentClientException ex)
      {
        ResponseMessage cosmosResponseMessage = ex.ToCosmosResponseMessage((RequestMessage) null);
        return new ThroughputResponse(cosmosResponseMessage.StatusCode, cosmosResponseMessage.Headers, (ThroughputProperties) null, cosmosResponseMessage.Diagnostics, cosmosResponseMessage.RequestMessage);
      }
      catch (AggregateException ex)
      {
        ResponseMessage responseMessage = TransportHandler.AggregateExceptionConverter(ex, (RequestMessage) null);
        return new ThroughputResponse(responseMessage.StatusCode, responseMessage.Headers, (ThroughputProperties) null, responseMessage.Diagnostics, responseMessage.RequestMessage);
      }
    }

    internal Task<ThroughputResponse> ReplaceThroughputAsync(
      string targetRID,
      int throughput,
      RequestOptions requestOptions,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.ReplaceThroughputPropertiesAsync(targetRID, ThroughputProperties.CreateManualThroughput(throughput), requestOptions, cancellationToken);
    }

    internal Task<ThroughputResponse> ReplaceThroughputIfExistsAsync(
      string targetRID,
      int throughput,
      RequestOptions requestOptions,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.ReplaceThroughputPropertiesIfExistsAsync(targetRID, ThroughputProperties.CreateManualThroughput(throughput), requestOptions, cancellationToken);
    }

    private async Task<(T offer, double requestCharge)> GetOfferV2Async<T>(
      string targetRID,
      bool failIfNotConfigured,
      CancellationToken cancellationToken)
    {
      if (string.IsNullOrWhiteSpace(targetRID))
        throw new ArgumentNullException(targetRID);
      QueryDefinition queryDefinition = new QueryDefinition("select * from root r where r.offerResourceId= @targetRID");
      queryDefinition.WithParameter("@targetRID", (object) targetRID);
      (T, double) offerV2Async;
      using (FeedIterator<T> databaseStreamIterator = this.GetOfferQueryIterator<T>(queryDefinition, (string) null, (QueryRequestOptions) null, cancellationToken))
      {
        (T, double) valueTuple = await this.SingleOrDefaultAsync<T>(databaseStreamIterator);
        if ((object) valueTuple.Item1 == null & failIfNotConfigured)
          throw CosmosExceptionFactory.CreateNotFoundException("Throughput is not configured for " + targetRID, new Headers()
          {
            RequestCharge = valueTuple.Item2
          });
        offerV2Async = valueTuple;
      }
      return offerV2Async;
    }

    internal virtual FeedIterator<T> GetOfferQueryIterator<T>(
      QueryDefinition queryDefinition,
      string continuationToken,
      QueryRequestOptions requestOptions,
      CancellationToken cancellationToken)
    {
      if (!(this.GetOfferQueryStreamIterator(queryDefinition, continuationToken, requestOptions, cancellationToken) is FeedIteratorInternal queryStreamIterator))
        throw new InvalidOperationException("Expected a FeedIteratorInternal.");
      return (FeedIterator<T>) new FeedIteratorCore<T>(queryStreamIterator, (Func<ResponseMessage, FeedResponse<T>>) (response => this.ClientContext.ResponseFactory.CreateQueryFeedResponse<T>(response, ResourceType.Offer)));
    }

    internal virtual FeedIterator GetOfferQueryStreamIterator(
      QueryDefinition queryDefinition,
      string continuationToken = null,
      QueryRequestOptions requestOptions = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return (FeedIterator) new FeedIteratorCore(this.ClientContext, this.OfferRootUri, ResourceType.Offer, queryDefinition, continuationToken, requestOptions, (ContainerInternal) null);
    }

    private async Task<(T item, double requestCharge)> SingleOrDefaultAsync<T>(
      FeedIterator<T> offerQuery,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      double totalRequestCharge = 0.0;
      while (offerQuery.HasMoreResults)
      {
        FeedResponse<T> source = await offerQuery.ReadNextAsync(cancellationToken);
        totalRequestCharge += source.Headers.RequestCharge;
        if (source.Any<T>())
          return (source.Single<T>(), totalRequestCharge);
      }
      return ();
    }

    private async Task<ThroughputResponse> GetThroughputResponseAsync(
      Stream streamPayload,
      OperationType operationType,
      Uri linkUri,
      ResourceType resourceType,
      double currentRequestCharge,
      RequestOptions requestOptions = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      CosmosClientContext clientContext = this.ClientContext;
      string originalString = linkUri.OriginalString;
      int num1 = (int) resourceType;
      int num2 = (int) operationType;
      Stream stream = streamPayload;
      RequestOptions requestOptions1 = requestOptions;
      Stream streamPayload1 = stream;
      NoOpTrace singleton = NoOpTrace.Singleton;
      CancellationToken cancellationToken1 = cancellationToken;
      ThroughputResponse throughputResponse;
      using (ResponseMessage responseMessage = await clientContext.ProcessResourceOperationStreamAsync(originalString, (ResourceType) num1, (OperationType) num2, requestOptions1, (ContainerInternal) null, (FeedRange) null, streamPayload1, (Action<RequestMessage>) null, (ITrace) singleton, cancellationToken1))
      {
        responseMessage.Headers.RequestCharge += currentRequestCharge;
        throughputResponse = this.ClientContext.ResponseFactory.CreateThroughputResponse(responseMessage);
      }
      return throughputResponse;
    }
  }
}
