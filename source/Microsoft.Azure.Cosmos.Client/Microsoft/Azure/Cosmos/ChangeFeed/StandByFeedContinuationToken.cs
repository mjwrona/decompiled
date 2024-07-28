// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.ChangeFeed.StandByFeedContinuationToken
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.Routing;
using Microsoft.Azure.Cosmos.Tracing;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Routing;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.Azure.Cosmos.ChangeFeed
{
  internal class StandByFeedContinuationToken
  {
    private readonly string containerRid;
    private readonly StandByFeedContinuationToken.PartitionKeyRangeCacheDelegate pkRangeCacheDelegate;
    private readonly string inputContinuationToken;
    private Queue<CompositeContinuationToken> compositeContinuationTokens;
    private CompositeContinuationToken currentToken;

    public static async Task<StandByFeedContinuationToken> CreateAsync(
      string containerRid,
      string initialStandByFeedContinuationToken,
      StandByFeedContinuationToken.PartitionKeyRangeCacheDelegate pkRangeCacheDelegate)
    {
      StandByFeedContinuationToken standByFeedContinuationToken = new StandByFeedContinuationToken(containerRid, initialStandByFeedContinuationToken, pkRangeCacheDelegate);
      await standByFeedContinuationToken.EnsureInitializedAsync();
      StandByFeedContinuationToken async = standByFeedContinuationToken;
      standByFeedContinuationToken = (StandByFeedContinuationToken) null;
      return async;
    }

    public static string CreateForRange(
      string containerRid,
      string minInclusive,
      string maxExclusive)
    {
      if (string.IsNullOrWhiteSpace(containerRid))
        throw new ArgumentNullException(nameof (containerRid));
      if (minInclusive == null)
        throw new ArgumentNullException(nameof (minInclusive));
      return !string.IsNullOrWhiteSpace(maxExclusive) ? StandByFeedContinuationToken.SerializeTokens((IEnumerable<CompositeContinuationToken>) new CompositeContinuationToken[1]
      {
        StandByFeedContinuationToken.CreateCompositeContinuationTokenForRange(minInclusive, maxExclusive, (string) null)
      }) : throw new ArgumentNullException(nameof (maxExclusive));
    }

    private static string SerializeTokens(
      IEnumerable<CompositeContinuationToken> compositeContinuationTokens)
    {
      return JsonConvert.SerializeObject((object) compositeContinuationTokens);
    }

    private static List<CompositeContinuationToken> DeserializeTokens(string continuationToken) => JsonConvert.DeserializeObject<List<CompositeContinuationToken>>(continuationToken);

    private StandByFeedContinuationToken(
      string containerRid,
      string initialStandByFeedContinuationToken,
      StandByFeedContinuationToken.PartitionKeyRangeCacheDelegate pkRangeCacheDelegate)
    {
      if (string.IsNullOrWhiteSpace(containerRid))
        throw new ArgumentNullException(nameof (containerRid));
      if (pkRangeCacheDelegate == null)
        throw new ArgumentNullException(nameof (pkRangeCacheDelegate));
      this.containerRid = containerRid;
      this.pkRangeCacheDelegate = pkRangeCacheDelegate;
      this.inputContinuationToken = initialStandByFeedContinuationToken;
    }

    public async Task<Tuple<CompositeContinuationToken, string>> GetCurrentTokenAsync(
      bool forceRefresh = false)
    {
      IReadOnlyList<PartitionKeyRange> overlappingRangesAsync = await this.TryGetOverlappingRangesAsync(this.currentToken.Range, forceRefresh);
      if (overlappingRangesAsync.Count > 1)
        this.HandleSplit(overlappingRangesAsync);
      return new Tuple<CompositeContinuationToken, string>(this.currentToken, overlappingRangesAsync[0].Id);
    }

    public void MoveToNextToken()
    {
      this.compositeContinuationTokens.Enqueue(this.compositeContinuationTokens.Dequeue());
      this.currentToken = this.compositeContinuationTokens.Peek();
    }

    public new string ToString() => this.compositeContinuationTokens == null ? (string) null : StandByFeedContinuationToken.SerializeTokens((IEnumerable<CompositeContinuationToken>) this.compositeContinuationTokens);

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

    private void HandleSplit(IReadOnlyList<PartitionKeyRange> keyRanges)
    {
      PartitionKeyRange partitionKeyRange1 = keyRanges != null ? keyRanges[0] : throw new ArgumentNullException(nameof (keyRanges));
      this.currentToken.Range = new Range<string>(partitionKeyRange1.MinInclusive, partitionKeyRange1.MaxExclusive, true, false);
      foreach (PartitionKeyRange partitionKeyRange2 in keyRanges.Skip<PartitionKeyRange>(1))
        this.compositeContinuationTokens.Enqueue(StandByFeedContinuationToken.CreateCompositeContinuationTokenForRange(partitionKeyRange2.MinInclusive, partitionKeyRange2.MaxExclusive, this.currentToken.Token));
    }

    private async Task EnsureInitializedAsync()
    {
      if (this.compositeContinuationTokens != null)
        return;
      this.InitializeCompositeTokens(await this.BuildCompositeTokensAsync(this.inputContinuationToken));
    }

    private async Task<IEnumerable<CompositeContinuationToken>> BuildCompositeTokensAsync(
      string initialContinuationToken)
    {
      if (string.IsNullOrEmpty(initialContinuationToken))
        return (await this.pkRangeCacheDelegate(this.containerRid, new Range<string>(PartitionKeyInternal.MinimumInclusiveEffectivePartitionKey, PartitionKeyInternal.MaximumExclusiveEffectivePartitionKey, true, false), (ITrace) NoOpTrace.Singleton, false)).Select<PartitionKeyRange, CompositeContinuationToken>((Func<PartitionKeyRange, CompositeContinuationToken>) (e => StandByFeedContinuationToken.CreateCompositeContinuationTokenForRange(e.MinInclusive, e.MaxExclusive, (string) null)));
      try
      {
        return (IEnumerable<CompositeContinuationToken>) StandByFeedContinuationToken.DeserializeTokens(initialContinuationToken);
      }
      catch (JsonReaderException ex)
      {
        throw new ArgumentOutOfRangeException("Provided token has an invalid format: " + initialContinuationToken, (Exception) ex);
      }
    }

    private void InitializeCompositeTokens(IEnumerable<CompositeContinuationToken> tokens)
    {
      this.compositeContinuationTokens = new Queue<CompositeContinuationToken>();
      foreach (CompositeContinuationToken token in tokens)
        this.compositeContinuationTokens.Enqueue(token);
      this.currentToken = this.compositeContinuationTokens.Peek();
    }

    private async Task<IReadOnlyList<PartitionKeyRange>> TryGetOverlappingRangesAsync(
      Range<string> targetRange,
      bool forceRefresh = false)
    {
      IReadOnlyList<PartitionKeyRange> partitionKeyRangeList = await this.pkRangeCacheDelegate(this.containerRid, new Range<string>(targetRange.Min, targetRange.Max, true, false), (ITrace) NoOpTrace.Singleton, forceRefresh);
      return partitionKeyRangeList.Count != 0 ? partitionKeyRangeList : throw new ArgumentOutOfRangeException("RequestContinuation", "Token contains invalid range " + targetRange.Min + "-" + targetRange.Max);
    }

    internal delegate Task<IReadOnlyList<PartitionKeyRange>> PartitionKeyRangeCacheDelegate(
      string containerRid,
      Range<string> ranges,
      ITrace trace,
      bool forceRefresh);
  }
}
