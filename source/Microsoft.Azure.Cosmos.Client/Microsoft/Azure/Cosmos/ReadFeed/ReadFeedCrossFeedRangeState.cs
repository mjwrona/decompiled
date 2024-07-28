// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.ReadFeed.ReadFeedCrossFeedRangeState
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.CosmosElements;
using Microsoft.Azure.Cosmos.Json;
using Microsoft.Azure.Cosmos.Pagination;
using Microsoft.Azure.Cosmos.Query.Core.Monads;
using Microsoft.Azure.Cosmos.ReadFeed.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Azure.Cosmos.ReadFeed
{
  internal readonly struct ReadFeedCrossFeedRangeState
  {
    public ReadFeedCrossFeedRangeState(
      IReadOnlyList<FeedRangeState<ReadFeedState>> feedRangeStates)
      : this((ReadOnlyMemory<FeedRangeState<ReadFeedState>>) feedRangeStates.ToArray<FeedRangeState<ReadFeedState>>().AsMemory<FeedRangeState<ReadFeedState>>())
    {
    }

    internal ReadFeedCrossFeedRangeState(
      ReadOnlyMemory<FeedRangeState<ReadFeedState>> feedRangeStates)
    {
      this.FeedRangeStates = !feedRangeStates.IsEmpty ? feedRangeStates : throw new ArgumentException("Expected feedRangeStates to be non empty.");
    }

    internal ReadOnlyMemory<FeedRangeState<ReadFeedState>> FeedRangeStates { get; }

    public ReadFeedCrossFeedRangeState Merge(ReadFeedCrossFeedRangeState first) => new ReadFeedCrossFeedRangeState((ReadOnlyMemory<FeedRangeState<ReadFeedState>>) CrossFeedRangeStateSplitterAndMerger.Merge<ReadFeedState>(this.FeedRangeStates, first.FeedRangeStates));

    public ReadFeedCrossFeedRangeState Merge(
      ReadFeedCrossFeedRangeState first,
      ReadFeedCrossFeedRangeState second)
    {
      return new ReadFeedCrossFeedRangeState((ReadOnlyMemory<FeedRangeState<ReadFeedState>>) CrossFeedRangeStateSplitterAndMerger.Merge<ReadFeedState>(this.FeedRangeStates, first.FeedRangeStates, second.FeedRangeStates));
    }

    public ReadFeedCrossFeedRangeState Merge(
      params ReadFeedCrossFeedRangeState[] readFeedCrossFeedRangeStates)
    {
      return this.Merge((IReadOnlyList<ReadFeedCrossFeedRangeState>) ((IEnumerable<ReadFeedCrossFeedRangeState>) readFeedCrossFeedRangeStates).ToList<ReadFeedCrossFeedRangeState>());
    }

    public ReadFeedCrossFeedRangeState Merge(
      IReadOnlyList<ReadFeedCrossFeedRangeState> readFeedCrossFeedRangeStates)
    {
      List<ReadOnlyMemory<FeedRangeState<ReadFeedState>>> ranges = new List<ReadOnlyMemory<FeedRangeState<ReadFeedState>>>(1 + readFeedCrossFeedRangeStates.Count)
      {
        this.FeedRangeStates
      };
      foreach (ReadFeedCrossFeedRangeState crossFeedRangeState in (IEnumerable<ReadFeedCrossFeedRangeState>) readFeedCrossFeedRangeStates)
        ranges.Add(crossFeedRangeState.FeedRangeStates);
      return new ReadFeedCrossFeedRangeState((ReadOnlyMemory<FeedRangeState<ReadFeedState>>) CrossFeedRangeStateSplitterAndMerger.Merge<ReadFeedState>((IReadOnlyList<ReadOnlyMemory<FeedRangeState<ReadFeedState>>>) ranges));
    }

    public bool TrySplit(
      out ReadFeedCrossFeedRangeState first,
      out ReadFeedCrossFeedRangeState second)
    {
      ReadOnlyMemory<FeedRangeState<ReadFeedState>> first1;
      ReadOnlyMemory<FeedRangeState<ReadFeedState>> second1;
      if (!CrossFeedRangeStateSplitterAndMerger.TrySplit<ReadFeedState>(this.FeedRangeStates, out first1, out second1))
      {
        first = new ReadFeedCrossFeedRangeState();
        second = new ReadFeedCrossFeedRangeState();
        return false;
      }
      first = new ReadFeedCrossFeedRangeState(first1);
      second = new ReadFeedCrossFeedRangeState(second1);
      return true;
    }

    public bool TrySplit(
      out ReadFeedCrossFeedRangeState first,
      out ReadFeedCrossFeedRangeState second,
      out ReadFeedCrossFeedRangeState third)
    {
      ReadOnlyMemory<FeedRangeState<ReadFeedState>> first1;
      ReadOnlyMemory<FeedRangeState<ReadFeedState>> second1;
      ReadOnlyMemory<FeedRangeState<ReadFeedState>> third1;
      if (!CrossFeedRangeStateSplitterAndMerger.TrySplit<ReadFeedState>(this.FeedRangeStates, out first1, out second1, out third1))
      {
        first = new ReadFeedCrossFeedRangeState();
        second = new ReadFeedCrossFeedRangeState();
        third = new ReadFeedCrossFeedRangeState();
        return false;
      }
      first = new ReadFeedCrossFeedRangeState(first1);
      second = new ReadFeedCrossFeedRangeState(second1);
      third = new ReadFeedCrossFeedRangeState(third1);
      return true;
    }

    public bool TrySplit(int numberOfPartitions, out List<ReadFeedCrossFeedRangeState> partitions)
    {
      List<ReadOnlyMemory<FeedRangeState<ReadFeedState>>> partitions1;
      if (!CrossFeedRangeStateSplitterAndMerger.TrySplit<ReadFeedState>(this.FeedRangeStates, numberOfPartitions, out partitions1))
      {
        partitions = (List<ReadFeedCrossFeedRangeState>) null;
        return false;
      }
      partitions = new List<ReadFeedCrossFeedRangeState>(partitions1.Count);
      foreach (ReadOnlyMemory<FeedRangeState<ReadFeedState>> feedRangeStates in partitions1)
        partitions.Add(new ReadFeedCrossFeedRangeState(feedRangeStates));
      return true;
    }

    public CosmosElement ToCosmosElement()
    {
      List<CosmosElement> cosmosElementList = new List<CosmosElement>();
      ReadOnlySpan<FeedRangeState<ReadFeedState>> span = this.FeedRangeStates.Span;
      for (int index = 0; index < span.Length; ++index)
      {
        FeedRangeState<ReadFeedState> feedRangeState = span[index];
        cosmosElementList.Add(ReadFeedFeedRangeStateSerializer.ToCosmosElement(feedRangeState));
      }
      return (CosmosElement) CosmosArray.Create((IEnumerable<CosmosElement>) cosmosElementList);
    }

    public override string ToString()
    {
      IJsonWriter jsonWriter = JsonWriter.Create(JsonSerializationFormat.Text);
      this.ToCosmosElement().WriteTo(jsonWriter);
      return Encoding.UTF8.GetString(jsonWriter.GetResult().Span);
    }

    public static ReadFeedCrossFeedRangeState Parse(string text)
    {
      TryCatch<ReadFeedCrossFeedRangeState> tryCatch = text != null ? ReadFeedCrossFeedRangeState.Monadic.Parse(text) : throw new ArgumentNullException(nameof (text));
      tryCatch.ThrowIfFailed();
      return tryCatch.Result;
    }

    public static bool TryParse(string text, out ReadFeedCrossFeedRangeState state)
    {
      TryCatch<ReadFeedCrossFeedRangeState> tryCatch = text != null ? ReadFeedCrossFeedRangeState.Monadic.Parse(text) : throw new ArgumentNullException(nameof (text));
      if (tryCatch.Failed)
      {
        state = new ReadFeedCrossFeedRangeState();
        return false;
      }
      state = tryCatch.Result;
      return true;
    }

    public static ReadFeedCrossFeedRangeState CreateFromBeginning() => ReadFeedCrossFeedRangeState.CreateFromBeginning((FeedRange) FeedRangeEpk.FullRange);

    public static ReadFeedCrossFeedRangeState CreateFromBeginning(FeedRange feedRange)
    {
      if (!(feedRange is FeedRangeInternal feedRange1))
        throw new ArgumentException("feedRange needs to be a FeedRangeInternal.");
      if (feedRange.Equals((object) FeedRangeEpk.FullRange))
        return ReadFeedCrossFeedRangeState.FullRangeStatesSingletons.Beginning;
      return new ReadFeedCrossFeedRangeState((IReadOnlyList<FeedRangeState<ReadFeedState>>) new List<FeedRangeState<ReadFeedState>>()
      {
        new FeedRangeState<ReadFeedState>(feedRange1, ReadFeedState.Beginning())
      });
    }

    public static ReadFeedCrossFeedRangeState CreateFromContinuation(CosmosElement continuation) => ReadFeedCrossFeedRangeState.CreateFromContinuation(continuation, (FeedRange) FeedRangeEpk.FullRange);

    public static ReadFeedCrossFeedRangeState CreateFromContinuation(
      CosmosElement continuation,
      FeedRange feedRange)
    {
      return feedRange is FeedRangeInternal feedRange1 ? new ReadFeedCrossFeedRangeState((IReadOnlyList<FeedRangeState<ReadFeedState>>) new List<FeedRangeState<ReadFeedState>>()
      {
        new FeedRangeState<ReadFeedState>(feedRange1, ReadFeedState.Continuation(continuation))
      }) : throw new ArgumentException("feedRange needs to be a FeedRangeInternal.");
    }

    private static class FullRangeStatesSingletons
    {
      public static readonly ReadFeedCrossFeedRangeState Beginning = new ReadFeedCrossFeedRangeState((IReadOnlyList<FeedRangeState<ReadFeedState>>) new List<FeedRangeState<ReadFeedState>>()
      {
        new FeedRangeState<ReadFeedState>((FeedRangeInternal) FeedRangeEpk.FullRange, ReadFeedState.Beginning())
      });
    }

    public static class Monadic
    {
      public static TryCatch<ReadFeedCrossFeedRangeState> Parse(string text)
      {
        TryCatch<CosmosElement> tryCatch = text != null ? CosmosElement.Monadic.Parse(text) : throw new ArgumentNullException(nameof (text));
        return tryCatch.Failed ? TryCatch<ReadFeedCrossFeedRangeState>.FromException(tryCatch.Exception) : ReadFeedCrossFeedRangeState.Monadic.CreateFromCosmosElement(tryCatch.Result);
      }

      internal static TryCatch<ReadFeedCrossFeedRangeState> CreateFromCosmosElement(
        CosmosElement cosmosElement)
      {
        if (cosmosElement == (CosmosElement) null)
          throw new ArgumentNullException(nameof (cosmosElement));
        if (!(cosmosElement is CosmosArray cosmosArray))
          return TryCatch<ReadFeedCrossFeedRangeState>.FromException((Exception) new FormatException(string.Format("Expected array: {0}", (object) cosmosElement)));
        FeedRangeState<ReadFeedState>[] array = new FeedRangeState<ReadFeedState>[cosmosArray.Count];
        int num = 0;
        foreach (CosmosElement cosmosElement1 in cosmosArray)
        {
          TryCatch<FeedRangeState<ReadFeedState>> fromCosmosElement = ReadFeedFeedRangeStateSerializer.Monadic.CreateFromCosmosElement(cosmosElement1);
          if (fromCosmosElement.Failed)
            return TryCatch<ReadFeedCrossFeedRangeState>.FromException(fromCosmosElement.Exception);
          array[num++] = fromCosmosElement.Result;
        }
        return TryCatch<ReadFeedCrossFeedRangeState>.FromResult(new ReadFeedCrossFeedRangeState((ReadOnlyMemory<FeedRangeState<ReadFeedState>>) array.AsMemory<FeedRangeState<ReadFeedState>>()));
      }
    }
  }
}
