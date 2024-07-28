// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.ChangeFeed.ChangeFeedIteratorCore
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.ChangeFeed.Pagination;
using Microsoft.Azure.Cosmos.Core.Utf8;
using Microsoft.Azure.Cosmos.CosmosElements;
using Microsoft.Azure.Cosmos.Pagination;
using Microsoft.Azure.Cosmos.Query.Core;
using Microsoft.Azure.Cosmos.Query.Core.Monads;
using Microsoft.Azure.Cosmos.Telemetry;
using Microsoft.Azure.Cosmos.Tracing;
using Microsoft.Azure.Documents.Routing;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Cosmos.ChangeFeed
{
  internal sealed class ChangeFeedIteratorCore : FeedIteratorInternal
  {
    private readonly IDocumentContainer documentContainer;
    private readonly ChangeFeedRequestOptions changeFeedRequestOptions;
    private readonly AsyncLazy<TryCatch<CrossPartitionChangeFeedAsyncEnumerator>> lazyMonadicEnumerator;
    private readonly CosmosClientContext clientContext;
    private readonly ChangeFeedQuerySpec changeFeedQuerySpec;
    private bool hasMoreResults;

    public ChangeFeedIteratorCore(
      IDocumentContainer documentContainer,
      ChangeFeedMode changeFeedMode,
      ChangeFeedRequestOptions changeFeedRequestOptions,
      ChangeFeedStartFrom changeFeedStartFrom,
      CosmosClientContext clientContext,
      ContainerInternal container,
      ChangeFeedQuerySpec changeFeedQuerySpec = null)
    {
      ChangeFeedIteratorCore feedIteratorCore = this;
      if (changeFeedStartFrom == null)
        throw new ArgumentNullException(nameof (changeFeedStartFrom));
      if (changeFeedMode == null)
        throw new ArgumentNullException(nameof (changeFeedMode));
      this.container = container;
      this.clientContext = clientContext;
      this.documentContainer = documentContainer ?? throw new ArgumentNullException(nameof (documentContainer));
      this.changeFeedRequestOptions = changeFeedRequestOptions ?? new ChangeFeedRequestOptions();
      this.changeFeedQuerySpec = changeFeedQuerySpec;
      this.lazyMonadicEnumerator = new AsyncLazy<TryCatch<CrossPartitionChangeFeedAsyncEnumerator>>((Func<ITrace, CancellationToken, Task<TryCatch<CrossPartitionChangeFeedAsyncEnumerator>>>) (async (trace, cancellationToken) =>
      {
        if (changeFeedStartFrom is ChangeFeedStartFromContinuation fromContinuation2)
        {
          TryCatch<CosmosElement> tryCatch = CosmosElement.Monadic.Parse(fromContinuation2.Continuation);
          if (tryCatch.Failed)
            return TryCatch<CrossPartitionChangeFeedAsyncEnumerator>.FromException((Exception) new MalformedChangeFeedContinuationTokenException("Failed to parse continuation token: " + fromContinuation2.Continuation + ".", tryCatch.Exception));
          TryCatch<VersionedAndRidCheckedCompositeToken> fromCosmosElement = VersionedAndRidCheckedCompositeToken.MonadicCreateFromCosmosElement(tryCatch.Result);
          if (fromCosmosElement.Failed)
            return TryCatch<CrossPartitionChangeFeedAsyncEnumerator>.FromException((Exception) new MalformedChangeFeedContinuationTokenException("Failed to parse continuation token: " + fromContinuation2.Continuation + ".", fromCosmosElement.Exception));
          VersionedAndRidCheckedCompositeToken versionedAndRidCheckedCompositeToken = fromCosmosElement.Result;
          if (versionedAndRidCheckedCompositeToken.VersionNumber == VersionedAndRidCheckedCompositeToken.Version.V1)
          {
            if (!(versionedAndRidCheckedCompositeToken.ContinuationToken is CosmosArray continuationToken2))
              return TryCatch<CrossPartitionChangeFeedAsyncEnumerator>.FromException((Exception) new MalformedChangeFeedContinuationTokenException("Failed to parse get array continuation token: " + fromContinuation2.Continuation + "."));
            List<CosmosElement> cosmosElementList = new List<CosmosElement>();
            foreach (CosmosElement cosmosElement1 in continuationToken2)
            {
              if (!(cosmosElement1 is CosmosObject cosmosObject3))
                return TryCatch<CrossPartitionChangeFeedAsyncEnumerator>.FromException((Exception) new MalformedChangeFeedContinuationTokenException("Failed to parse get object in composite continuation: " + fromContinuation2.Continuation + "."));
              CosmosElement cosmosElement2;
              if (!cosmosObject3.TryGetValue("range", out cosmosElement2))
                return TryCatch<CrossPartitionChangeFeedAsyncEnumerator>.FromException((Exception) new MalformedChangeFeedContinuationTokenException(string.Format("Failed to parse token: {0}.", (object) cosmosObject3)));
              if (!(cosmosElement2 is CosmosObject cosmosObject4))
                return TryCatch<CrossPartitionChangeFeedAsyncEnumerator>.FromException((Exception) new MalformedChangeFeedContinuationTokenException("Failed to parse get object in composite continuation: " + fromContinuation2.Continuation + "."));
              CosmosString typedCosmosElement1;
              if (!cosmosObject4.TryGetValue<CosmosString>("min", out typedCosmosElement1))
                return TryCatch<CrossPartitionChangeFeedAsyncEnumerator>.FromException((Exception) new MalformedChangeFeedContinuationTokenException(string.Format("Failed to parse start of range: {0}.", (object) cosmosObject3)));
              CosmosString typedCosmosElement2;
              if (!cosmosObject4.TryGetValue<CosmosString>("max", out typedCosmosElement2))
                return TryCatch<CrossPartitionChangeFeedAsyncEnumerator>.FromException((Exception) new MalformedChangeFeedContinuationTokenException(string.Format("Failed to parse end of range: {0}.", (object) cosmosObject3)));
              CosmosElement continuation;
              if (!cosmosObject3.TryGetValue("token", out continuation))
                return TryCatch<CrossPartitionChangeFeedAsyncEnumerator>.FromException((Exception) new MalformedChangeFeedContinuationTokenException(string.Format("Failed to parse token: {0}.", (object) cosmosObject3)));
              FeedRangeState<ChangeFeedState> feedRangeState = new FeedRangeState<ChangeFeedState>((FeedRangeInternal) new FeedRangeEpk(new Range<string>(UtfAnyString.op_Implicit(typedCosmosElement1.Value), UtfAnyString.op_Implicit(typedCosmosElement2.Value), true, false)), continuation is CosmosNull ? ChangeFeedState.Beginning() : ChangeFeedState.Continuation(continuation));
              cosmosElementList.Add(ChangeFeedFeedRangeStateSerializer.ToCosmosElement(feedRangeState));
            }
            versionedAndRidCheckedCompositeToken = new VersionedAndRidCheckedCompositeToken(VersionedAndRidCheckedCompositeToken.Version.V2, (CosmosElement) CosmosArray.Create((IEnumerable<CosmosElement>) cosmosElementList), versionedAndRidCheckedCompositeToken.Rid);
          }
          if (versionedAndRidCheckedCompositeToken.VersionNumber != VersionedAndRidCheckedCompositeToken.Version.V2)
            return TryCatch<CrossPartitionChangeFeedAsyncEnumerator>.FromException((Exception) new MalformedChangeFeedContinuationTokenException(string.Format("Wrong version number: {0}.", (object) versionedAndRidCheckedCompositeToken.VersionNumber)));
          string resourceIdentifierAsync = await documentContainer.GetResourceIdentifierAsync(trace, cancellationToken);
          if (versionedAndRidCheckedCompositeToken.Rid != resourceIdentifierAsync)
            return TryCatch<CrossPartitionChangeFeedAsyncEnumerator>.FromException((Exception) new MalformedChangeFeedContinuationTokenException("rids mismatched. Expected: " + resourceIdentifierAsync + " but got " + versionedAndRidCheckedCompositeToken.Rid + "."));
          changeFeedStartFrom = ChangeFeedStartFrom.ContinuationToken(versionedAndRidCheckedCompositeToken.ContinuationToken.ToString());
          versionedAndRidCheckedCompositeToken = new VersionedAndRidCheckedCompositeToken();
        }
        TryCatch<ChangeFeedCrossFeedRangeState> tryCatch1 = changeFeedStartFrom.Accept<TryCatch<ChangeFeedCrossFeedRangeState>>((ChangeFeedStartFromVisitor<TryCatch<ChangeFeedCrossFeedRangeState>>) ChangeFeedIteratorCore.ChangeFeedStateFromToChangeFeedCrossFeedRangeState.Singleton);
        if (tryCatch1.Failed)
          return TryCatch<CrossPartitionChangeFeedAsyncEnumerator>.FromException((Exception) new MalformedChangeFeedContinuationTokenException("Could not convert to ChangeFeedCrossFeedRangeState.", tryCatch1.Exception));
        Dictionary<string, string> additionalHeaders;
        if (changeFeedRequestOptions?.Properties != null)
        {
          additionalHeaders = new Dictionary<string, string>();
          Dictionary<string, object> dictionary = new Dictionary<string, object>();
          foreach (KeyValuePair<string, object> property in (IEnumerable<KeyValuePair<string, object>>) changeFeedRequestOptions.Properties)
          {
            if (property.Value is string str2)
              additionalHeaders[property.Key] = str2;
            else
              dictionary[property.Key] = property.Value;
          }
          changeFeedRequestOptions.Properties = (IReadOnlyDictionary<string, object>) dictionary;
        }
        else
          additionalHeaders = (Dictionary<string, string>) null;
        return TryCatch<CrossPartitionChangeFeedAsyncEnumerator>.FromResult(CrossPartitionChangeFeedAsyncEnumerator.Create(documentContainer, new CrossFeedRangeState<ChangeFeedState>(tryCatch1.Result.FeedRangeStates), new ChangeFeedPaginationOptions(changeFeedMode, (int?) changeFeedRequestOptions?.PageSizeHint, changeFeedRequestOptions?.JsonSerializationFormatOptions?.JsonSerializationFormat, additionalHeaders, feedIteratorCore.changeFeedQuerySpec), new CancellationToken()));
      }));
      this.hasMoreResults = true;
    }

    public override bool HasMoreResults => this.hasMoreResults;

    public override async Task<ResponseMessage> ReadNextAsync(CancellationToken cancellationToken = default (CancellationToken)) => await this.clientContext.OperationHelperAsync<ResponseMessage>("Change Feed Iterator Read Next Async", (RequestOptions) this.changeFeedRequestOptions, (Func<ITrace, Task<ResponseMessage>>) (trace => this.ReadNextInternalAsync(trace, cancellationToken)), (Func<ResponseMessage, OpenTelemetryAttributes>) (response => (OpenTelemetryAttributes) new OpenTelemetryResponse(response, this.container?.Id, this.container?.Database?.Id)), TraceComponent.ChangeFeed);

    public override async Task<ResponseMessage> ReadNextAsync(
      ITrace trace,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      ResponseMessage responseMessage;
      try
      {
        responseMessage = await this.ReadNextInternalAsync(trace, cancellationToken);
      }
      catch (OperationCanceledException ex) when (!(ex is CosmosOperationCanceledException))
      {
        throw new CosmosOperationCanceledException(ex, trace);
      }
      return responseMessage;
    }

    private async Task<ResponseMessage> ReadNextInternalAsync(
      ITrace trace,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      if (trace == null)
        throw new ArgumentNullException(nameof (trace));
      TryCatch<CrossPartitionChangeFeedAsyncEnumerator> valueAsync = await this.lazyMonadicEnumerator.GetValueAsync(trace, cancellationToken);
      if (valueAsync.Failed)
      {
        Exception exception = valueAsync.Exception;
        CosmosException cosmosException;
        if (!ExceptionToCosmosException.TryCreateFromException(exception, trace, out cosmosException))
        {
          this.hasMoreResults = false;
          throw exception;
        }
        return new ResponseMessage(cosmosException.StatusCode, (RequestMessage) null, cosmosException.Headers, cosmosException, trace);
      }
      CrossPartitionChangeFeedAsyncEnumerator enumerator = valueAsync.Result;
      enumerator.SetCancellationToken(cancellationToken);
      try
      {
        if (!await enumerator.MoveNextAsync(trace))
          throw new InvalidOperationException("ChangeFeed enumerator should always have a next continuation");
      }
      catch (OperationCanceledException ex) when (!(ex is CosmosOperationCanceledException))
      {
        throw new CosmosOperationCanceledException(ex, trace);
      }
      if (enumerator.Current.Failed)
      {
        CosmosException cosmosException;
        if (!ExceptionToCosmosException.TryCreateFromException(enumerator.Current.Exception, trace, out cosmosException))
          throw ExceptionWithStackTraceException.UnWrapMonadExcepion(enumerator.Current.Exception, trace);
        if (!FeedIteratorInternal.IsRetriableException(cosmosException))
          this.hasMoreResults = false;
        return new ResponseMessage(cosmosException.StatusCode, (RequestMessage) null, cosmosException.Headers, cosmosException, trace);
      }
      CrossFeedRangePage<Microsoft.Azure.Cosmos.ChangeFeed.Pagination.ChangeFeedPage, ChangeFeedState> result = enumerator.Current.Result;
      Microsoft.Azure.Cosmos.ChangeFeed.Pagination.ChangeFeedPage changeFeedPage = result.Page;
      ResponseMessage responseMessage;
      if (changeFeedPage is ChangeFeedSuccessPage changeFeedSuccessPage)
        responseMessage = new ResponseMessage(HttpStatusCode.OK)
        {
          Content = changeFeedSuccessPage.Content
        };
      else
        responseMessage = new ResponseMessage(HttpStatusCode.NotModified);
      CosmosElement continuationToken = new ChangeFeedCrossFeedRangeState(result.State.Value).ToCosmosElement();
      string str = VersionedAndRidCheckedCompositeToken.ToCosmosElement(new VersionedAndRidCheckedCompositeToken(VersionedAndRidCheckedCompositeToken.Version.V2, continuationToken, await this.documentContainer.GetResourceIdentifierAsync(trace, cancellationToken))).ToString();
      continuationToken = (CosmosElement) null;
      responseMessage.Headers.ContinuationToken = str;
      responseMessage.Headers.RequestCharge = changeFeedPage.RequestCharge;
      responseMessage.Headers.ActivityId = changeFeedPage.ActivityId;
      responseMessage.Trace = trace;
      return responseMessage;
    }

    public override CosmosElement GetCosmosElementContinuationToken() => throw new NotSupportedException();

    private sealed class ChangeFeedStateFromToChangeFeedCrossFeedRangeState : 
      ChangeFeedStartFromVisitor<TryCatch<ChangeFeedCrossFeedRangeState>>
    {
      public static readonly ChangeFeedIteratorCore.ChangeFeedStateFromToChangeFeedCrossFeedRangeState Singleton = new ChangeFeedIteratorCore.ChangeFeedStateFromToChangeFeedCrossFeedRangeState();

      public override TryCatch<ChangeFeedCrossFeedRangeState> Visit(
        ChangeFeedStartFromNow startFromNow)
      {
        return TryCatch<ChangeFeedCrossFeedRangeState>.FromResult(ChangeFeedCrossFeedRangeState.CreateFromNow(startFromNow.FeedRange));
      }

      public override TryCatch<ChangeFeedCrossFeedRangeState> Visit(
        ChangeFeedStartFromTime startFromTime)
      {
        return TryCatch<ChangeFeedCrossFeedRangeState>.FromResult(ChangeFeedCrossFeedRangeState.CreateFromTime(startFromTime.StartTime, startFromTime.FeedRange));
      }

      public override TryCatch<ChangeFeedCrossFeedRangeState> Visit(
        ChangeFeedStartFromContinuation startFromContinuation)
      {
        TryCatch<CosmosElement> tryCatch = CosmosElement.Monadic.Parse(startFromContinuation.Continuation);
        return tryCatch.Failed ? TryCatch<ChangeFeedCrossFeedRangeState>.FromException((Exception) new MalformedChangeFeedContinuationTokenException("Failed to parse continuation token: " + startFromContinuation.Continuation + ".", tryCatch.Exception)) : ChangeFeedCrossFeedRangeState.Monadic.CreateFromCosmosElement(tryCatch.Result);
      }

      public override TryCatch<ChangeFeedCrossFeedRangeState> Visit(
        ChangeFeedStartFromBeginning startFromBeginning)
      {
        return TryCatch<ChangeFeedCrossFeedRangeState>.FromResult(ChangeFeedCrossFeedRangeState.CreateFromBeginning(startFromBeginning.FeedRange));
      }

      public override TryCatch<ChangeFeedCrossFeedRangeState> Visit(
        ChangeFeedStartFromContinuationAndFeedRange startFromContinuationAndFeedRange)
      {
        throw new NotSupportedException();
      }
    }
  }
}
