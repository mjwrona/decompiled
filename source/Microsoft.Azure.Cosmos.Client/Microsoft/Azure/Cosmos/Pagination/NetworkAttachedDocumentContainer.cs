// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Pagination.NetworkAttachedDocumentContainer
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.ChangeFeed;
using Microsoft.Azure.Cosmos.ChangeFeed.Pagination;
using Microsoft.Azure.Cosmos.Core.Utf8;
using Microsoft.Azure.Cosmos.CosmosElements;
using Microsoft.Azure.Cosmos.Json;
using Microsoft.Azure.Cosmos.Query.Core;
using Microsoft.Azure.Cosmos.Query.Core.Monads;
using Microsoft.Azure.Cosmos.Query.Core.Pipeline.Pagination;
using Microsoft.Azure.Cosmos.Query.Core.QueryClient;
using Microsoft.Azure.Cosmos.ReadFeed.Pagination;
using Microsoft.Azure.Cosmos.Resource.CosmosExceptions;
using Microsoft.Azure.Cosmos.Tracing;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Routing;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Cosmos.Pagination
{
  internal sealed class NetworkAttachedDocumentContainer : 
    IMonadicDocumentContainer,
    IMonadicFeedRangeProvider,
    IMonadicQueryDataSource,
    IMonadicReadFeedDataSource,
    IMonadicChangeFeedDataSource
  {
    private readonly ContainerInternal container;
    private readonly CosmosQueryClient cosmosQueryClient;
    private readonly QueryRequestOptions queryRequestOptions;
    private readonly ChangeFeedRequestOptions changeFeedRequestOptions;
    private readonly string resourceLink;
    private readonly ResourceType resourceType;
    private readonly Guid correlatedActivityId;

    public NetworkAttachedDocumentContainer(
      ContainerInternal container,
      CosmosQueryClient cosmosQueryClient,
      Guid correlatedActivityId,
      QueryRequestOptions queryRequestOptions = null,
      ChangeFeedRequestOptions changeFeedRequestOptions = null,
      string resourceLink = null,
      ResourceType resourceType = ResourceType.Document)
    {
      this.container = container ?? throw new ArgumentNullException(nameof (container));
      this.cosmosQueryClient = cosmosQueryClient ?? throw new ArgumentNullException(nameof (cosmosQueryClient));
      this.queryRequestOptions = queryRequestOptions;
      this.changeFeedRequestOptions = changeFeedRequestOptions;
      this.resourceLink = resourceLink ?? this.container.LinkUri;
      this.resourceType = resourceType;
      this.correlatedActivityId = correlatedActivityId;
    }

    public Task<TryCatch> MonadicSplitAsync(
      FeedRangeInternal feedRange,
      CancellationToken cancellationToken)
    {
      return Task.FromResult<TryCatch>(TryCatch.FromException((Exception) new NotSupportedException()));
    }

    public Task<TryCatch> MonadicMergeAsync(
      FeedRangeInternal feedRange1,
      FeedRangeInternal feedRange2,
      CancellationToken cancellationToken)
    {
      return Task.FromResult<TryCatch>(TryCatch.FromException((Exception) new NotSupportedException()));
    }

    public async Task<TryCatch<Record>> MonadicCreateItemAsync(
      CosmosObject payload,
      CancellationToken cancellationToken)
    {
      ContainerInternal container = this.container;
      CosmosObject cosmosObject = payload;
      CancellationToken cancellationToken1 = cancellationToken;
      Microsoft.Azure.Cosmos.PartitionKey? partitionKey = new Microsoft.Azure.Cosmos.PartitionKey?();
      CancellationToken cancellationToken2 = cancellationToken1;
      ItemResponse<CosmosObject> itemAsync = await container.CreateItemAsync<CosmosObject>(cosmosObject, partitionKey, cancellationToken: cancellationToken2);
      if (itemAsync.StatusCode != HttpStatusCode.Created)
        return TryCatch<Record>.FromException((Exception) new CosmosException("Failed to insert document", itemAsync.StatusCode, 0, itemAsync.ActivityId, itemAsync.RequestCharge));
      CosmosObject resource = itemAsync.Resource;
      string identifier = UtfAnyString.op_Implicit(((CosmosString) resource["id"]).Value);
      return TryCatch<Record>.FromResult(new Record(ResourceId.Parse(UtfAnyString.op_Implicit(((CosmosString) resource["_rid"]).Value)), new DateTime(Number64.ToLong(((CosmosNumber) resource["_ts"]).Value), DateTimeKind.Utc), identifier, resource));
    }

    public Task<TryCatch<Record>> MonadicReadItemAsync(
      CosmosElement partitionKey,
      string identifer,
      CancellationToken cancellationToken)
    {
      throw new NotImplementedException();
    }

    public Task<TryCatch<List<FeedRangeEpk>>> MonadicGetFeedRangesAsync(
      ITrace trace,
      CancellationToken cancellationToken)
    {
      return this.MonadicGetChildRangeAsync((FeedRangeInternal) FeedRangeEpk.FullRange, trace, cancellationToken);
    }

    public async Task<TryCatch<List<FeedRangeEpk>>> MonadicGetChildRangeAsync(
      FeedRangeInternal feedRange,
      ITrace trace,
      CancellationToken cancellationToken)
    {
      try
      {
        ContainerProperties containerProperties = await this.container.ClientContext.GetCachedContainerPropertiesAsync(this.container.LinkUri, trace, cancellationToken);
        CosmosQueryClient cosmosQueryClient = this.cosmosQueryClient;
        string resourceLink = this.container.LinkUri;
        List<PartitionKeyRange> byFeedRangeAsync = await cosmosQueryClient.GetTargetPartitionKeyRangeByFeedRangeAsync(resourceLink, await this.container.GetCachedRIDAsync(false, trace, cancellationToken), containerProperties.PartitionKey, feedRange, false, trace);
        cosmosQueryClient = (CosmosQueryClient) null;
        resourceLink = (string) null;
        return TryCatch<List<FeedRangeEpk>>.FromResult(byFeedRangeAsync.Select<PartitionKeyRange, FeedRangeEpk>((Func<PartitionKeyRange, FeedRangeEpk>) (range => new FeedRangeEpk(new Range<string>(range.MinInclusive, range.MaxExclusive, true, false)))).ToList<FeedRangeEpk>());
      }
      catch (Exception ex)
      {
        return TryCatch<List<FeedRangeEpk>>.FromException(ex);
      }
    }

    public async Task<TryCatch> MonadicRefreshProviderAsync(
      ITrace trace,
      CancellationToken cancellationToken)
    {
      cancellationToken.ThrowIfCancellationRequested();
      using (ITrace refreshTrace = trace.StartChild("Refresh FeedRangeProvider", TraceComponent.Routing, TraceLevel.Info))
      {
        try
        {
          IReadOnlyList<PartitionKeyRange> overlappingRangesAsync = await this.cosmosQueryClient.TryGetOverlappingRangesAsync(await this.container.GetCachedRIDAsync(false, refreshTrace, cancellationToken), FeedRangeEpk.FullRange.Range, true);
          return TryCatch.FromResult();
        }
        catch (Exception ex)
        {
          return TryCatch.FromException(ex);
        }
      }
    }

    public async Task<TryCatch<ReadFeedPage>> MonadicReadFeedAsync(
      FeedRangeState<ReadFeedState> feedRangeState,
      ReadFeedPaginationOptions readFeedPaginationOptions,
      ITrace trace,
      CancellationToken cancellationToken)
    {
      if (readFeedPaginationOptions == null)
        readFeedPaginationOptions = ReadFeedPaginationOptions.Default;
      ResponseMessage responseMessage = await this.container.ClientContext.ProcessResourceOperationStreamAsync(this.resourceLink, this.resourceType, OperationType.ReadFeed, (RequestOptions) this.queryRequestOptions, this.container, (FeedRange) feedRangeState.FeedRange, (Stream) null, (Action<RequestMessage>) (request =>
      {
        if (feedRangeState.State is ReadFeedContinuationState state2)
          request.Headers.ContinuationToken = UtfAnyString.op_Implicit(((CosmosString) state2.ContinuationToken).Value);
        JsonSerializationFormat? serializationFormat = readFeedPaginationOptions.JsonSerializationFormat;
        if (serializationFormat.HasValue)
        {
          Microsoft.Azure.Cosmos.Headers headers = request.Headers;
          serializationFormat = readFeedPaginationOptions.JsonSerializationFormat;
          string serializationFormatString = serializationFormat.Value.ToContentSerializationFormatString();
          headers.ContentSerializationFormat = serializationFormatString;
        }
        foreach (KeyValuePair<string, string> additionalHeader in readFeedPaginationOptions.AdditionalHeaders)
          request.Headers[additionalHeader.Key] = additionalHeader.Value;
      }), trace, cancellationToken);
      TryCatch<ReadFeedPage> tryCatch;
      if (responseMessage.StatusCode == HttpStatusCode.OK)
      {
        double requestCharge = responseMessage.Headers.RequestCharge;
        string activityId = responseMessage.Headers.ActivityId;
        ReadFeedState state3 = responseMessage.Headers.ContinuationToken != null ? ReadFeedState.Continuation((CosmosElement) CosmosString.Create(responseMessage.Headers.ContinuationToken)) : (ReadFeedState) null;
        Dictionary<string, string> additionalHeaders = NetworkAttachedDocumentContainer.GetAdditionalHeaders(responseMessage.Headers.CosmosMessageHeaders, ReadFeedPage.BannedHeaders);
        tryCatch = TryCatch<ReadFeedPage>.FromResult(new ReadFeedPage(responseMessage.Content, requestCharge, activityId, (IReadOnlyDictionary<string, string>) additionalHeaders, state3));
      }
      else
        tryCatch = TryCatch<ReadFeedPage>.FromException((Exception) CosmosExceptionFactory.Create(responseMessage));
      return tryCatch;
    }

    public async Task<TryCatch<QueryPage>> MonadicQueryAsync(
      SqlQuerySpec sqlQuerySpec,
      FeedRangeState<QueryState> feedRangeState,
      QueryPaginationOptions queryPaginationOptions,
      ITrace trace,
      CancellationToken cancellationToken)
    {
      if (sqlQuerySpec == null)
        throw new ArgumentNullException(nameof (sqlQuerySpec));
      if (queryPaginationOptions == null)
        throw new ArgumentNullException(nameof (queryPaginationOptions));
      if (trace == null)
        throw new ArgumentNullException(nameof (trace));
      QueryRequestOptions requestOptions = this.queryRequestOptions == null ? new QueryRequestOptions() : this.queryRequestOptions;
      return await this.cosmosQueryClient.ExecuteItemQueryAsync(this.resourceLink, this.resourceType, OperationType.Query, this.correlatedActivityId, (FeedRange) feedRangeState.FeedRange, requestOptions, sqlQuerySpec, UtfAnyString.op_Implicit(feedRangeState.State == null ? UtfAnyString.op_Implicit((string) null) : ((CosmosString) feedRangeState.State.Value).Value), false, queryPaginationOptions.PageSizeLimit ?? int.MaxValue, trace, cancellationToken);
    }

    public async Task<TryCatch<Microsoft.Azure.Cosmos.ChangeFeed.Pagination.ChangeFeedPage>> MonadicChangeFeedAsync(
      FeedRangeState<ChangeFeedState> feedRangeState,
      ChangeFeedPaginationOptions changeFeedPaginationOptions,
      ITrace trace,
      CancellationToken cancellationToken)
    {
      cancellationToken.ThrowIfCancellationRequested();
      if (changeFeedPaginationOptions == null)
        throw new ArgumentNullException(nameof (changeFeedPaginationOptions));
      Stream stream = (Stream) null;
      if (changeFeedPaginationOptions.ChangeFeedQuerySpec != null && changeFeedPaginationOptions.ChangeFeedQuerySpec.ShouldSerializeQueryText())
        stream = this.container.ClientContext.SerializerCore.ToStream<ChangeFeedQuerySpec>(changeFeedPaginationOptions.ChangeFeedQuerySpec);
      ResponseMessage responseMessage = await this.container.ClientContext.ProcessResourceOperationStreamAsync(this.container.LinkUri, ResourceType.Document, OperationType.ReadFeed, (RequestOptions) this.changeFeedRequestOptions, this.container, (FeedRange) feedRangeState.FeedRange, stream ?? (Stream) null, (Action<RequestMessage>) (request =>
      {
        if (changeFeedPaginationOptions.PageSizeLimit.HasValue)
          request.Headers.PageSize = changeFeedPaginationOptions.PageSizeLimit.Value.ToString();
        feedRangeState.State.Accept<RequestMessage>((IChangeFeedStateVisitor<RequestMessage>) NetworkAttachedDocumentContainer.ChangeFeedStateRequestMessagePopulator.Singleton, request);
        changeFeedPaginationOptions.Mode.Accept(request);
        JsonSerializationFormat? serializationFormat = changeFeedPaginationOptions.JsonSerializationFormat;
        if (serializationFormat.HasValue)
        {
          Microsoft.Azure.Cosmos.Headers headers = request.Headers;
          serializationFormat = changeFeedPaginationOptions.JsonSerializationFormat;
          string serializationFormatString = serializationFormat.Value.ToContentSerializationFormatString();
          headers.ContentSerializationFormat = serializationFormatString;
        }
        foreach (KeyValuePair<string, string> additionalHeader in changeFeedPaginationOptions.AdditionalHeaders)
          request.Headers[additionalHeader.Key] = additionalHeader.Value;
      }), trace, cancellationToken);
      TryCatch<Microsoft.Azure.Cosmos.ChangeFeed.Pagination.ChangeFeedPage> tryCatch;
      if ((responseMessage.StatusCode == HttpStatusCode.OK ? 1 : (responseMessage.StatusCode == HttpStatusCode.NotModified ? 1 : 0)) != 0)
      {
        double requestCharge = responseMessage.Headers.RequestCharge;
        string activityId = responseMessage.Headers.ActivityId;
        ChangeFeedState state = ChangeFeedState.Continuation((CosmosElement) CosmosString.Create(responseMessage.Headers.ETag));
        Dictionary<string, string> additionalHeaders = NetworkAttachedDocumentContainer.GetAdditionalHeaders(responseMessage.Headers.CosmosMessageHeaders, Microsoft.Azure.Cosmos.ChangeFeed.Pagination.ChangeFeedPage.BannedHeaders);
        tryCatch = TryCatch<Microsoft.Azure.Cosmos.ChangeFeed.Pagination.ChangeFeedPage>.FromResult(responseMessage.StatusCode != HttpStatusCode.OK ? (Microsoft.Azure.Cosmos.ChangeFeed.Pagination.ChangeFeedPage) new ChangeFeedNotModifiedPage(requestCharge, activityId, (IReadOnlyDictionary<string, string>) additionalHeaders, state) : (Microsoft.Azure.Cosmos.ChangeFeed.Pagination.ChangeFeedPage) new ChangeFeedSuccessPage(responseMessage.Content, requestCharge, activityId, (IReadOnlyDictionary<string, string>) additionalHeaders, state));
      }
      else
        tryCatch = TryCatch<Microsoft.Azure.Cosmos.ChangeFeed.Pagination.ChangeFeedPage>.FromException((Exception) CosmosExceptionFactory.Create(responseMessage));
      return tryCatch;
    }

    public async Task<TryCatch<string>> MonadicGetResourceIdentifierAsync(
      ITrace trace,
      CancellationToken cancellationToken)
    {
      try
      {
        return TryCatch<string>.FromResult(await this.container.GetCachedRIDAsync(false, trace, cancellationToken));
      }
      catch (Exception ex)
      {
        return TryCatch<string>.FromException(ex);
      }
    }

    private static Dictionary<string, string> GetAdditionalHeaders(
      CosmosMessageHeadersInternal headers,
      ImmutableHashSet<string> bannedHeaders)
    {
      Dictionary<string, string> additionalHeaders = new Dictionary<string, string>(headers.Count());
      foreach (string header in headers)
      {
        if (!bannedHeaders.Contains(header))
          additionalHeaders[header] = headers[header];
      }
      return additionalHeaders;
    }

    private sealed class ChangeFeedStateRequestMessagePopulator : 
      IChangeFeedStateVisitor<RequestMessage>
    {
      public static readonly NetworkAttachedDocumentContainer.ChangeFeedStateRequestMessagePopulator Singleton = new NetworkAttachedDocumentContainer.ChangeFeedStateRequestMessagePopulator();
      private const string IfNoneMatchAllHeaderValue = "*";
      private static readonly DateTime StartFromBeginningTime = DateTime.MinValue.ToUniversalTime();

      private ChangeFeedStateRequestMessagePopulator()
      {
      }

      public void Visit(ChangeFeedStateBeginning changeFeedStateBeginning, RequestMessage message)
      {
      }

      public void Visit(ChangeFeedStateTime changeFeedStateTime, RequestMessage message)
      {
        if (!(changeFeedStateTime.StartTime != NetworkAttachedDocumentContainer.ChangeFeedStateRequestMessagePopulator.StartFromBeginningTime))
          return;
        message.Headers.Add("If-Modified-Since", changeFeedStateTime.StartTime.ToString("r", (IFormatProvider) CultureInfo.InvariantCulture));
      }

      public void Visit(
        ChangeFeedStateContinuation changeFeedStateContinuation,
        RequestMessage message)
      {
        message.Headers.IfNoneMatch = UtfAnyString.op_Implicit((changeFeedStateContinuation.ContinuationToken as CosmosString).Value);
      }

      public void Visit(ChangeFeedStateNow changeFeedStateNow, RequestMessage message) => message.Headers.IfNoneMatch = "*";
    }
  }
}
