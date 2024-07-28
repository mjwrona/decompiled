// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.FeedRangeCompositeContinuation
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.Query.Core.Monads;
using Microsoft.Azure.Cosmos.Routing;
using Microsoft.Azure.Cosmos.Tracing;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Routing;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Cosmos
{
  [JsonConverter(typeof (FeedRangeCompositeContinuationConverter))]
  internal sealed class FeedRangeCompositeContinuation : FeedRangeContinuation
  {
    private static readonly ShouldRetryResult Retry = ShouldRetryResult.RetryAfter(TimeSpan.Zero);
    private static readonly ShouldRetryResult NoRetry = ShouldRetryResult.NoRetry();
    private string initialNoResultsRange;

    public Queue<CompositeContinuationToken> CompositeContinuationTokens { get; }

    public CompositeContinuationToken CurrentToken { get; private set; }

    private FeedRangeCompositeContinuation(string containerRid, FeedRangeInternal feedRange)
      : base(containerRid, feedRange)
    {
      this.CompositeContinuationTokens = new Queue<CompositeContinuationToken>();
    }

    public FeedRangeCompositeContinuation(
      string containerRid,
      FeedRangeInternal feedRange,
      IReadOnlyList<Range<string>> ranges,
      string continuation = null)
      : this(containerRid, feedRange)
    {
      if (ranges == null)
        throw new ArgumentNullException(nameof (ranges));
      if (ranges.Count == 0)
        throw new ArgumentOutOfRangeException(nameof (ranges));
      foreach (Range<string> range in (IEnumerable<Range<string>>) ranges)
        this.CompositeContinuationTokens.Enqueue(FeedRangeCompositeContinuation.CreateCompositeContinuationTokenForRange(range.Min, range.Max, continuation));
      this.CurrentToken = this.CompositeContinuationTokens.Peek();
    }

    public FeedRangeCompositeContinuation(
      string containerRid,
      FeedRangeInternal feedRange,
      IReadOnlyList<CompositeContinuationToken> deserializedTokens)
      : this(containerRid, feedRange)
    {
      if (deserializedTokens == null)
        throw new ArgumentNullException(nameof (deserializedTokens));
      if (deserializedTokens.Count == 0)
        throw new ArgumentOutOfRangeException(nameof (deserializedTokens));
      foreach (CompositeContinuationToken deserializedToken in (IEnumerable<CompositeContinuationToken>) deserializedTokens)
        this.CompositeContinuationTokens.Enqueue(deserializedToken);
      this.CurrentToken = this.CompositeContinuationTokens.Peek();
    }

    public override string GetContinuation() => this.CurrentToken?.Token;

    public override Microsoft.Azure.Cosmos.FeedRange GetFeedRange()
    {
      if (!(this.FeedRange is FeedRangeEpk))
        return (Microsoft.Azure.Cosmos.FeedRange) this.FeedRange;
      return this.CurrentToken != null ? (Microsoft.Azure.Cosmos.FeedRange) new FeedRangeEpk(this.CurrentToken.Range) : (Microsoft.Azure.Cosmos.FeedRange) null;
    }

    public override string ToString() => JsonConvert.SerializeObject((object) this);

    public override void ReplaceContinuation(string continuationToken)
    {
      this.CurrentToken.Token = continuationToken;
      this.MoveToNextToken();
    }

    public override TryCatch ValidateContainer(string containerRid) => !string.IsNullOrEmpty(this.ContainerRid) && !this.ContainerRid.Equals(containerRid, StringComparison.Ordinal) ? TryCatch.FromException((Exception) new ArgumentException(string.Format(ClientResources.FeedToken_InvalidFeedTokenForContainer, (object) this.ContainerRid, (object) containerRid))) : TryCatch.FromResult();

    public override bool IsDone => this.CompositeContinuationTokens.Count == 0;

    public override ShouldRetryResult HandleChangeFeedNotModified(ResponseMessage responseMessage)
    {
      if (responseMessage.IsSuccessStatusCode)
      {
        this.initialNoResultsRange = (string) null;
        return FeedRangeCompositeContinuation.NoRetry;
      }
      if (responseMessage.StatusCode == HttpStatusCode.NotModified && this.CompositeContinuationTokens.Count > 1)
      {
        if (this.initialNoResultsRange == null)
        {
          this.initialNoResultsRange = this.CurrentToken.Range.Min;
          this.ReplaceContinuation(responseMessage.Headers.ETag);
          return FeedRangeCompositeContinuation.Retry;
        }
        if (!this.initialNoResultsRange.Equals(this.CurrentToken.Range.Min, StringComparison.OrdinalIgnoreCase))
        {
          this.ReplaceContinuation(responseMessage.Headers.ETag);
          return FeedRangeCompositeContinuation.Retry;
        }
      }
      return FeedRangeCompositeContinuation.NoRetry;
    }

    public override async Task<ShouldRetryResult> HandleSplitAsync(
      ContainerInternal containerCore,
      ResponseMessage responseMessage,
      CancellationToken cancellationToken)
    {
      if ((responseMessage.StatusCode != HttpStatusCode.Gone ? 0 : (responseMessage.Headers.SubStatusCode == SubStatusCodes.PartitionKeyRangeGone ? 1 : (responseMessage.Headers.SubStatusCode == SubStatusCodes.CompletingSplit ? 1 : 0))) == 0)
        return FeedRangeCompositeContinuation.NoRetry;
      IReadOnlyList<PartitionKeyRange> overlappingRangesAsync = await this.TryGetOverlappingRangesAsync(await containerCore.ClientContext.DocumentClient.GetPartitionKeyRangeCacheAsync((ITrace) NoOpTrace.Singleton), this.CurrentToken.Range.Min, this.CurrentToken.Range.Max, true);
      if (overlappingRangesAsync.Count > 0)
        this.CreateChildRanges(overlappingRangesAsync);
      return FeedRangeCompositeContinuation.Retry;
    }

    public new static bool TryParse(string toStringValue, out FeedRangeContinuation feedToken)
    {
      try
      {
        feedToken = (FeedRangeContinuation) JsonConvert.DeserializeObject<FeedRangeCompositeContinuation>(toStringValue);
        return true;
      }
      catch (JsonException ex)
      {
        feedToken = (FeedRangeContinuation) null;
        return false;
      }
    }

    private static bool TryParseAsCompositeContinuationToken(
      string providedContinuation,
      out CompositeContinuationToken compositeContinuationToken)
    {
      compositeContinuationToken = (CompositeContinuationToken) null;
      try
      {
        if (providedContinuation.Trim().StartsWith("[", StringComparison.Ordinal))
        {
          List<CompositeContinuationToken> continuationTokenList = JsonConvert.DeserializeObject<List<CompositeContinuationToken>>(providedContinuation);
          if (continuationTokenList != null && continuationTokenList.Count > 0)
            compositeContinuationToken = continuationTokenList[0];
          return compositeContinuationToken != null;
        }
        if (!providedContinuation.Trim().StartsWith("{", StringComparison.Ordinal))
          return false;
        compositeContinuationToken = JsonConvert.DeserializeObject<CompositeContinuationToken>(providedContinuation);
        return compositeContinuationToken != null;
      }
      catch (JsonException ex)
      {
        return false;
      }
    }

    private static CompositeContinuationToken CreateCompositeContinuationTokenForRange(
      string minInclusive,
      string maxExclusive,
      string token)
    {
      return new CompositeContinuationToken()
      {
        Range = new Range<string>(minInclusive, maxExclusive, true, false),
        Token = token
      };
    }

    private void MoveToNextToken()
    {
      CompositeContinuationToken continuationToken = this.CompositeContinuationTokens.Dequeue();
      if (continuationToken.Token != null)
        this.CompositeContinuationTokens.Enqueue(continuationToken);
      this.CurrentToken = this.CompositeContinuationTokens.Count > 0 ? this.CompositeContinuationTokens.Peek() : (CompositeContinuationToken) null;
    }

    private void CreateChildRanges(IReadOnlyList<PartitionKeyRange> keyRanges)
    {
      PartitionKeyRange partitionKeyRange1 = keyRanges != null ? keyRanges[0] : throw new ArgumentNullException(nameof (keyRanges));
      this.CurrentToken.Range = new Range<string>(partitionKeyRange1.MinInclusive, partitionKeyRange1.MaxExclusive, true, false);
      CompositeContinuationToken compositeContinuationToken;
      if (FeedRangeCompositeContinuation.TryParseAsCompositeContinuationToken(this.CurrentToken.Token, out compositeContinuationToken))
      {
        compositeContinuationToken.Range = this.CurrentToken.Range;
        this.CurrentToken.Token = JsonConvert.SerializeObject((object) compositeContinuationToken);
        foreach (PartitionKeyRange partitionKeyRange2 in keyRanges.Skip<PartitionKeyRange>(1))
        {
          compositeContinuationToken.Range = partitionKeyRange2.ToRange();
          this.CompositeContinuationTokens.Enqueue(FeedRangeCompositeContinuation.CreateCompositeContinuationTokenForRange(partitionKeyRange2.MinInclusive, partitionKeyRange2.MaxExclusive, JsonConvert.SerializeObject((object) compositeContinuationToken)));
        }
      }
      else
      {
        foreach (PartitionKeyRange partitionKeyRange3 in keyRanges.Skip<PartitionKeyRange>(1))
          this.CompositeContinuationTokens.Enqueue(FeedRangeCompositeContinuation.CreateCompositeContinuationTokenForRange(partitionKeyRange3.MinInclusive, partitionKeyRange3.MaxExclusive, this.CurrentToken.Token));
      }
    }

    private async Task<IReadOnlyList<PartitionKeyRange>> TryGetOverlappingRangesAsync(
      PartitionKeyRangeCache partitionKeyRangeCache,
      string min,
      string max,
      bool forceRefresh = false)
    {
      FeedRangeCompositeContinuation compositeContinuation = this;
      IReadOnlyList<PartitionKeyRange> overlappingRangesAsync = await partitionKeyRangeCache.TryGetOverlappingRangesAsync(compositeContinuation.ContainerRid, new Range<string>(min, max, true, false), (ITrace) NoOpTrace.Singleton, forceRefresh);
      return overlappingRangesAsync.Count != 0 ? overlappingRangesAsync : throw new ArgumentOutOfRangeException("RequestContinuation", "Token contains invalid range " + min + "-" + max);
    }

    public override void Accept(IFeedRangeContinuationVisitor visitor) => visitor.Visit(this);
  }
}
