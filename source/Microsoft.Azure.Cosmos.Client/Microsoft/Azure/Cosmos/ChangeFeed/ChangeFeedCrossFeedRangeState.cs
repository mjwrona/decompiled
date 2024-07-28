// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.ChangeFeed.ChangeFeedCrossFeedRangeState
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.ChangeFeed.Pagination;
using Microsoft.Azure.Cosmos.CosmosElements;
using Microsoft.Azure.Cosmos.Json;
using Microsoft.Azure.Cosmos.Pagination;
using Microsoft.Azure.Cosmos.Query.Core.Monads;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;

namespace Microsoft.Azure.Cosmos.ChangeFeed
{
  internal readonly struct ChangeFeedCrossFeedRangeState
  {
    public ChangeFeedCrossFeedRangeState(
      IReadOnlyList<FeedRangeState<ChangeFeedState>> feedRangeStates)
      : this((ReadOnlyMemory<FeedRangeState<ChangeFeedState>>) feedRangeStates.ToArray<FeedRangeState<ChangeFeedState>>().AsMemory<FeedRangeState<ChangeFeedState>>())
    {
    }

    internal ChangeFeedCrossFeedRangeState(
      ReadOnlyMemory<FeedRangeState<ChangeFeedState>> feedRangeStates)
    {
      this.FeedRangeStates = !feedRangeStates.IsEmpty ? feedRangeStates : throw new ArgumentException("Expected feedRangeStates to be non empty.");
    }

    internal ReadOnlyMemory<FeedRangeState<ChangeFeedState>> FeedRangeStates { get; }

    public ChangeFeedCrossFeedRangeState Merge(ChangeFeedCrossFeedRangeState first) => new ChangeFeedCrossFeedRangeState((ReadOnlyMemory<FeedRangeState<ChangeFeedState>>) CrossFeedRangeStateSplitterAndMerger.Merge<ChangeFeedState>(this.FeedRangeStates, first.FeedRangeStates));

    public ChangeFeedCrossFeedRangeState Merge(
      ChangeFeedCrossFeedRangeState first,
      ChangeFeedCrossFeedRangeState second)
    {
      return new ChangeFeedCrossFeedRangeState((ReadOnlyMemory<FeedRangeState<ChangeFeedState>>) CrossFeedRangeStateSplitterAndMerger.Merge<ChangeFeedState>(this.FeedRangeStates, first.FeedRangeStates, second.FeedRangeStates));
    }

    public ChangeFeedCrossFeedRangeState Merge(
      params ChangeFeedCrossFeedRangeState[] changeFeedCrossFeedRangeStates)
    {
      return this.Merge((IReadOnlyList<ChangeFeedCrossFeedRangeState>) ((IEnumerable<ChangeFeedCrossFeedRangeState>) changeFeedCrossFeedRangeStates).ToList<ChangeFeedCrossFeedRangeState>());
    }

    public ChangeFeedCrossFeedRangeState Merge(
      IReadOnlyList<ChangeFeedCrossFeedRangeState> changeFeedCrossFeedRangeStates)
    {
      List<ReadOnlyMemory<FeedRangeState<ChangeFeedState>>> ranges = new List<ReadOnlyMemory<FeedRangeState<ChangeFeedState>>>(1 + changeFeedCrossFeedRangeStates.Count)
      {
        this.FeedRangeStates
      };
      foreach (ChangeFeedCrossFeedRangeState crossFeedRangeState in (IEnumerable<ChangeFeedCrossFeedRangeState>) changeFeedCrossFeedRangeStates)
        ranges.Add(crossFeedRangeState.FeedRangeStates);
      return new ChangeFeedCrossFeedRangeState((ReadOnlyMemory<FeedRangeState<ChangeFeedState>>) CrossFeedRangeStateSplitterAndMerger.Merge<ChangeFeedState>((IReadOnlyList<ReadOnlyMemory<FeedRangeState<ChangeFeedState>>>) ranges));
    }

    public bool TrySplit(
      out ChangeFeedCrossFeedRangeState first,
      out ChangeFeedCrossFeedRangeState second)
    {
      ReadOnlyMemory<FeedRangeState<ChangeFeedState>> first1;
      ReadOnlyMemory<FeedRangeState<ChangeFeedState>> second1;
      if (!CrossFeedRangeStateSplitterAndMerger.TrySplit<ChangeFeedState>(this.FeedRangeStates, out first1, out second1))
      {
        first = new ChangeFeedCrossFeedRangeState();
        second = new ChangeFeedCrossFeedRangeState();
        return false;
      }
      first = new ChangeFeedCrossFeedRangeState(first1);
      second = new ChangeFeedCrossFeedRangeState(second1);
      return true;
    }

    public bool TrySplit(
      out ChangeFeedCrossFeedRangeState first,
      out ChangeFeedCrossFeedRangeState second,
      out ChangeFeedCrossFeedRangeState third)
    {
      ReadOnlyMemory<FeedRangeState<ChangeFeedState>> first1;
      ReadOnlyMemory<FeedRangeState<ChangeFeedState>> second1;
      ReadOnlyMemory<FeedRangeState<ChangeFeedState>> third1;
      if (!CrossFeedRangeStateSplitterAndMerger.TrySplit<ChangeFeedState>(this.FeedRangeStates, out first1, out second1, out third1))
      {
        first = new ChangeFeedCrossFeedRangeState();
        second = new ChangeFeedCrossFeedRangeState();
        third = new ChangeFeedCrossFeedRangeState();
        return false;
      }
      first = new ChangeFeedCrossFeedRangeState(first1);
      second = new ChangeFeedCrossFeedRangeState(second1);
      third = new ChangeFeedCrossFeedRangeState(third1);
      return true;
    }

    public bool TrySplit(int numberOfPartitions, out List<ChangeFeedCrossFeedRangeState> partitions)
    {
      List<ReadOnlyMemory<FeedRangeState<ChangeFeedState>>> partitions1;
      if (!CrossFeedRangeStateSplitterAndMerger.TrySplit<ChangeFeedState>(this.FeedRangeStates, numberOfPartitions, out partitions1))
      {
        partitions = (List<ChangeFeedCrossFeedRangeState>) null;
        return false;
      }
      partitions = new List<ChangeFeedCrossFeedRangeState>(partitions1.Count);
      foreach (ReadOnlyMemory<FeedRangeState<ChangeFeedState>> feedRangeStates in partitions1)
        partitions.Add(new ChangeFeedCrossFeedRangeState(feedRangeStates));
      return true;
    }

    public CosmosElement ToCosmosElement()
    {
      List<CosmosElement> cosmosElementList = new List<CosmosElement>();
      ReadOnlySpan<FeedRangeState<ChangeFeedState>> span = this.FeedRangeStates.Span;
      for (int index = 0; index < span.Length; ++index)
      {
        FeedRangeState<ChangeFeedState> feedRangeState = span[index];
        cosmosElementList.Add(ChangeFeedFeedRangeStateSerializer.ToCosmosElement(feedRangeState));
      }
      return (CosmosElement) CosmosArray.Create((IEnumerable<CosmosElement>) cosmosElementList);
    }

    public override string ToString()
    {
      IJsonWriter jsonWriter = JsonWriter.Create(JsonSerializationFormat.Text);
      this.ToCosmosElement().WriteTo(jsonWriter);
      return Encoding.UTF8.GetString(jsonWriter.GetResult().Span);
    }

    public static ChangeFeedCrossFeedRangeState Parse(string text)
    {
      TryCatch<ChangeFeedCrossFeedRangeState> tryCatch = text != null ? ChangeFeedCrossFeedRangeState.Monadic.Parse(text) : throw new ArgumentNullException(nameof (text));
      tryCatch.ThrowIfFailed();
      return tryCatch.Result;
    }

    public static bool TryParse(string text, out ChangeFeedCrossFeedRangeState state)
    {
      TryCatch<ChangeFeedCrossFeedRangeState> tryCatch = text != null ? ChangeFeedCrossFeedRangeState.Monadic.Parse(text) : throw new ArgumentNullException(nameof (text));
      if (tryCatch.Failed)
      {
        state = new ChangeFeedCrossFeedRangeState();
        return false;
      }
      state = tryCatch.Result;
      return true;
    }

    public static ChangeFeedCrossFeedRangeState CreateFromBeginning() => ChangeFeedCrossFeedRangeState.CreateFromBeginning((FeedRange) FeedRangeEpk.FullRange);

    public static ChangeFeedCrossFeedRangeState CreateFromBeginning(FeedRange feedRange)
    {
      if (!(feedRange is FeedRangeInternal feedRange1))
        throw new ArgumentException("feedRange needs to be a FeedRangeInternal.");
      if (feedRange.Equals((object) FeedRangeEpk.FullRange))
        return ChangeFeedCrossFeedRangeState.FullRangeStatesSingletons.Beginning;
      return new ChangeFeedCrossFeedRangeState((IReadOnlyList<FeedRangeState<ChangeFeedState>>) new List<FeedRangeState<ChangeFeedState>>()
      {
        new FeedRangeState<ChangeFeedState>(feedRange1, ChangeFeedState.Beginning())
      });
    }

    public static ChangeFeedCrossFeedRangeState CreateFromNow() => ChangeFeedCrossFeedRangeState.CreateFromNow((FeedRange) FeedRangeEpk.FullRange);

    public static ChangeFeedCrossFeedRangeState CreateFromNow(FeedRange feedRange)
    {
      if (!(feedRange is FeedRangeInternal feedRange1))
        throw new ArgumentException("feedRange needs to be a FeedRangeInternal.");
      if (feedRange.Equals((object) FeedRangeEpk.FullRange))
        return ChangeFeedCrossFeedRangeState.FullRangeStatesSingletons.Now;
      return new ChangeFeedCrossFeedRangeState((IReadOnlyList<FeedRangeState<ChangeFeedState>>) new List<FeedRangeState<ChangeFeedState>>()
      {
        new FeedRangeState<ChangeFeedState>(feedRange1, ChangeFeedState.Now())
      });
    }

    public static ChangeFeedCrossFeedRangeState CreateFromTime(DateTime dateTimeUtc) => ChangeFeedCrossFeedRangeState.CreateFromTime(dateTimeUtc, (FeedRange) FeedRangeEpk.FullRange);

    public static ChangeFeedCrossFeedRangeState CreateFromTime(
      DateTime dateTimeUtc,
      FeedRange feedRange)
    {
      return feedRange is FeedRangeInternal feedRange1 ? new ChangeFeedCrossFeedRangeState((IReadOnlyList<FeedRangeState<ChangeFeedState>>) new List<FeedRangeState<ChangeFeedState>>()
      {
        new FeedRangeState<ChangeFeedState>(feedRange1, ChangeFeedState.Time(dateTimeUtc))
      }) : throw new ArgumentException("feedRange needs to be a FeedRangeInternal.");
    }

    public static ChangeFeedCrossFeedRangeState CreateFromContinuation(CosmosElement continuation) => ChangeFeedCrossFeedRangeState.CreateFromContinuation(continuation, (FeedRange) FeedRangeEpk.FullRange);

    public static ChangeFeedCrossFeedRangeState CreateFromContinuation(
      CosmosElement continuation,
      FeedRange feedRange)
    {
      return feedRange is FeedRangeInternal feedRange1 ? new ChangeFeedCrossFeedRangeState((IReadOnlyList<FeedRangeState<ChangeFeedState>>) new List<FeedRangeState<ChangeFeedState>>()
      {
        new FeedRangeState<ChangeFeedState>(feedRange1, ChangeFeedState.Continuation(continuation))
      }) : throw new ArgumentException("feedRange needs to be a FeedRangeInternal.");
    }

    private static class FullRangeStatesSingletons
    {
      public static readonly ChangeFeedCrossFeedRangeState Beginning = new ChangeFeedCrossFeedRangeState((IReadOnlyList<FeedRangeState<ChangeFeedState>>) new List<FeedRangeState<ChangeFeedState>>()
      {
        new FeedRangeState<ChangeFeedState>((FeedRangeInternal) FeedRangeEpk.FullRange, ChangeFeedState.Beginning())
      });
      public static readonly ChangeFeedCrossFeedRangeState Now = new ChangeFeedCrossFeedRangeState((IReadOnlyList<FeedRangeState<ChangeFeedState>>) new List<FeedRangeState<ChangeFeedState>>()
      {
        new FeedRangeState<ChangeFeedState>((FeedRangeInternal) FeedRangeEpk.FullRange, ChangeFeedState.Now())
      });
    }

    public static class Monadic
    {
      public static TryCatch<ChangeFeedCrossFeedRangeState> Parse(string text)
      {
        TryCatch<CosmosElement> tryCatch = text != null ? CosmosElement.Monadic.Parse(text) : throw new ArgumentNullException(nameof (text));
        return tryCatch.Failed ? TryCatch<ChangeFeedCrossFeedRangeState>.FromException(tryCatch.Exception) : ChangeFeedCrossFeedRangeState.Monadic.CreateFromCosmosElement(tryCatch.Result);
      }

      internal static TryCatch<ChangeFeedCrossFeedRangeState> CreateFromCosmosElement(
        CosmosElement cosmosElement)
      {
        if (cosmosElement == (CosmosElement) null)
          throw new ArgumentNullException(nameof (cosmosElement));
        if (!(cosmosElement is CosmosArray cosmosArray))
          return TryCatch<ChangeFeedCrossFeedRangeState>.FromException((Exception) new FormatException(string.Format("Expected array: {0}", (object) cosmosElement)));
        List<FeedRangeState<ChangeFeedState>> items = new List<FeedRangeState<ChangeFeedState>>(cosmosArray.Count);
        foreach (CosmosElement cosmosElement1 in cosmosArray)
        {
          TryCatch<FeedRangeState<ChangeFeedState>> fromCosmosElement = ChangeFeedFeedRangeStateSerializer.Monadic.CreateFromCosmosElement(cosmosElement1);
          if (fromCosmosElement.Failed)
            return TryCatch<ChangeFeedCrossFeedRangeState>.FromException(fromCosmosElement.Exception);
          items.Add(fromCosmosElement.Result);
        }
        return TryCatch<ChangeFeedCrossFeedRangeState>.FromResult(new ChangeFeedCrossFeedRangeState((IReadOnlyList<FeedRangeState<ChangeFeedState>>) items.ToImmutableArray<FeedRangeState<ChangeFeedState>>()));
      }
    }
  }
}
