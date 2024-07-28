// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.ReadFeed.ReadFeedIteratorCore
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.Core.Utf8;
using Microsoft.Azure.Cosmos.CosmosElements;
using Microsoft.Azure.Cosmos.Pagination;
using Microsoft.Azure.Cosmos.Query.Core;
using Microsoft.Azure.Cosmos.Query.Core.Monads;
using Microsoft.Azure.Cosmos.ReadFeed.Pagination;
using Microsoft.Azure.Cosmos.Routing;
using Microsoft.Azure.Cosmos.Tracing;
using Microsoft.Azure.Documents.Routing;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Cosmos.ReadFeed
{
  internal sealed class ReadFeedIteratorCore : FeedIteratorInternal
  {
    private readonly TryCatch<CrossPartitionReadFeedAsyncEnumerator> monadicEnumerator;
    private readonly QueryRequestOptions queryRequestOptions;
    private bool hasMoreResults;

    public ReadFeedIteratorCore(
      IDocumentContainer documentContainer,
      string continuationToken,
      ReadFeedPaginationOptions readFeedPaginationOptions,
      QueryRequestOptions queryRequestOptions,
      ContainerInternal container,
      CancellationToken cancellationToken)
    {
      this.container = container;
      this.queryRequestOptions = queryRequestOptions;
      if (readFeedPaginationOptions == null)
        readFeedPaginationOptions = ReadFeedPaginationOptions.Default;
      if (!string.IsNullOrEmpty(continuationToken) && (continuationToken.Length < 2 || continuationToken[0] != '[' ? 0 : (continuationToken[continuationToken.Length - 1] == ']' ? 1 : 0)) == 0)
      {
        FeedRangeContinuation parsedToken;
        if (!FeedRangeContinuation.TryParse(continuationToken, out parsedToken))
        {
          string empty = string.Empty;
          FeedRangeEpk fullRange = FeedRangeEpk.FullRange;
          List<Range<string>> ranges = new List<Range<string>>();
          ranges.Add(new Range<string>(PartitionKeyInternal.MinimumInclusiveEffectivePartitionKey, PartitionKeyInternal.MaximumExclusiveEffectivePartitionKey, true, false));
          string continuation = continuationToken;
          parsedToken = (FeedRangeContinuation) new FeedRangeCompositeContinuation(empty, (FeedRangeInternal) fullRange, (IReadOnlyList<Range<string>>) ranges, continuation);
        }
        List<CosmosElement> cosmosElementList = new List<CosmosElement>();
        string json = parsedToken.ToString();
        if (parsedToken.FeedRange is FeedRangePartitionKey feedRange)
        {
          CosmosArray cosmosArray = (CosmosArray) CosmosObject.Parse(json)["Continuation"];
          CosmosElement cosmosElement = cosmosArray.Count == 1 ? ((CosmosObject) cosmosArray[0])["token"] : throw new InvalidOperationException("Expected only one continuation for partition key queries");
          ReadFeedState state = !(cosmosElement is CosmosNull) ? ReadFeedState.Continuation(CosmosElement.Parse(UtfAnyString.op_Implicit(((CosmosString) cosmosElement).Value))) : ReadFeedState.Beginning();
          FeedRangeState<ReadFeedState> feedRangeState = new FeedRangeState<ReadFeedState>((FeedRangeInternal) feedRange, state);
          cosmosElementList.Add(ReadFeedFeedRangeStateSerializer.ToCosmosElement(feedRangeState));
        }
        else
        {
          foreach (CosmosObject cosmosObject1 in (CosmosArray) CosmosObject.Parse(json)["Continuation"])
          {
            CosmosObject cosmosObject2 = (CosmosObject) cosmosObject1["range"];
            string min = UtfAnyString.op_Implicit(((CosmosString) cosmosObject2["min"]).Value);
            string max = UtfAnyString.op_Implicit(((CosmosString) cosmosObject2["max"]).Value);
            CosmosElement cosmosElement = cosmosObject1["token"];
            FeedRangeState<ReadFeedState> feedRangeState = new FeedRangeState<ReadFeedState>((FeedRangeInternal) new FeedRangeEpk(new Range<string>(min, max, true, false)), !(cosmosElement is CosmosNull) ? ReadFeedState.Continuation(CosmosElement.Parse(UtfAnyString.op_Implicit(((CosmosString) cosmosElement).Value))) : ReadFeedState.Beginning());
            cosmosElementList.Add(ReadFeedFeedRangeStateSerializer.ToCosmosElement(feedRangeState));
          }
        }
        continuationToken = CosmosArray.Create((IEnumerable<CosmosElement>) cosmosElementList).ToString();
      }
      TryCatch<ReadFeedCrossFeedRangeState> tryCatch;
      if (continuationToken == null)
      {
        FeedRange feedRange;
        if (this.queryRequestOptions != null)
        {
          PartitionKey? partitionKey = this.queryRequestOptions.PartitionKey;
          if (partitionKey.HasValue)
          {
            partitionKey = this.queryRequestOptions.PartitionKey;
            feedRange = (FeedRange) new FeedRangePartitionKey(partitionKey.Value);
            goto label_20;
          }
        }
        feedRange = this.queryRequestOptions == null || this.queryRequestOptions.FeedRange == null ? (FeedRange) FeedRangeEpk.FullRange : this.queryRequestOptions.FeedRange;
label_20:
        tryCatch = TryCatch<ReadFeedCrossFeedRangeState>.FromResult(ReadFeedCrossFeedRangeState.CreateFromBeginning(feedRange));
      }
      else
        tryCatch = ReadFeedCrossFeedRangeState.Monadic.Parse(continuationToken);
      this.monadicEnumerator = !tryCatch.Failed ? TryCatch<CrossPartitionReadFeedAsyncEnumerator>.FromResult(CrossPartitionReadFeedAsyncEnumerator.Create(documentContainer, new CrossFeedRangeState<ReadFeedState>(tryCatch.Result.FeedRangeStates), readFeedPaginationOptions, cancellationToken)) : TryCatch<CrossPartitionReadFeedAsyncEnumerator>.FromException(tryCatch.Exception);
      this.hasMoreResults = true;
    }

    public override bool HasMoreResults => this.hasMoreResults;

    public override Task<ResponseMessage> ReadNextAsync(CancellationToken cancellationToken = default (CancellationToken)) => this.ReadNextAsync((ITrace) NoOpTrace.Singleton, cancellationToken);

    public override async Task<ResponseMessage> ReadNextAsync(
      ITrace trace,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      if (trace == null)
        throw new ArgumentNullException(nameof (trace));
      if (!this.hasMoreResults)
        throw new InvalidOperationException("Should not be calling FeedIterator that does not have any more results");
      if (this.monadicEnumerator.Failed)
      {
        this.hasMoreResults = false;
        CosmosException cosmosException;
        if (!ExceptionToCosmosException.TryCreateFromException(this.monadicEnumerator.Exception, trace, out cosmosException))
          throw this.monadicEnumerator.Exception;
        return new ResponseMessage(HttpStatusCode.BadRequest, (RequestMessage) null, cosmosException.Headers, cosmosException, trace);
      }
      CrossPartitionReadFeedAsyncEnumerator enumerator = this.monadicEnumerator.Result;
      enumerator.SetCancellationToken(cancellationToken);
      TryCatch<CrossFeedRangePage<Microsoft.Azure.Cosmos.ReadFeed.Pagination.ReadFeedPage, ReadFeedState>> tryCatch;
      try
      {
        tryCatch = await enumerator.MoveNextAsync(trace) ? enumerator.Current : throw new InvalidOperationException("Should not be calling enumerator that does not have any more results");
      }
      catch (OperationCanceledException ex) when (!(ex is CosmosOperationCanceledException))
      {
        throw new CosmosOperationCanceledException(ex, trace);
      }
      if (tryCatch.Failed)
      {
        CosmosException cosmosException;
        if (!ExceptionToCosmosException.TryCreateFromException(tryCatch.Exception, trace, out cosmosException))
          throw tryCatch.Exception;
        if (!FeedIteratorInternal.IsRetriableException(cosmosException))
          this.hasMoreResults = false;
        return new ResponseMessage(cosmosException.StatusCode, (RequestMessage) null, cosmosException.Headers, cosmosException, trace);
      }
      CrossFeedRangePage<Microsoft.Azure.Cosmos.ReadFeed.Pagination.ReadFeedPage, ReadFeedState> result = tryCatch.Result;
      if (result.State == null)
        this.hasMoreResults = false;
      string str;
      if (result.State != null)
      {
        List<CompositeContinuationToken> deserializedTokens = new List<CompositeContinuationToken>();
        CrossFeedRangeState<ReadFeedState> state1 = result.State;
        for (int index = 0; index < state1.Value.Length; ++index)
        {
          FeedRangeState<ReadFeedState> feedRangeState = state1.Value.Span[index];
          FeedRangeEpk feedRangeEpk = !(feedRangeState.FeedRange is FeedRangeEpk feedRange) ? FeedRangeEpk.FullRange : feedRange;
          ReadFeedState state2 = feedRangeState.State;
          CompositeContinuationToken continuationToken = new CompositeContinuationToken()
          {
            Range = feedRangeEpk.Range,
            Token = state2 is ReadFeedBeginningState ? (string) null : ((ReadFeedContinuationState) state2).ContinuationToken.ToString()
          };
          deserializedTokens.Add(continuationToken);
        }
        FeedRangeInternal feedRange1 = this.queryRequestOptions == null || !this.queryRequestOptions.PartitionKey.HasValue ? (this.queryRequestOptions == null || this.queryRequestOptions.FeedRange == null ? (FeedRangeInternal) FeedRangeEpk.FullRange : (FeedRangeInternal) this.queryRequestOptions.FeedRange) : (FeedRangeInternal) new FeedRangePartitionKey(this.queryRequestOptions.PartitionKey.Value);
        str = new FeedRangeCompositeContinuation(string.Empty, feedRange1, (IReadOnlyList<CompositeContinuationToken>) deserializedTokens).ToString();
      }
      else
        str = (string) null;
      Microsoft.Azure.Cosmos.ReadFeed.Pagination.ReadFeedPage page = result.Page;
      Microsoft.Azure.Cosmos.Headers headers = new Microsoft.Azure.Cosmos.Headers()
      {
        RequestCharge = page.RequestCharge,
        ActivityId = page.ActivityId,
        ContinuationToken = str
      };
      foreach (KeyValuePair<string, string> additionalHeader in (IEnumerable<KeyValuePair<string, string>>) page.AdditionalHeaders)
        headers[additionalHeader.Key] = additionalHeader.Value;
      return new ResponseMessage(HttpStatusCode.OK, (RequestMessage) null, headers, (CosmosException) null, trace)
      {
        Content = page.Content
      };
    }

    public override CosmosElement GetCosmosElementContinuationToken() => throw new NotSupportedException();
  }
}
