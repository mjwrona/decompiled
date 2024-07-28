// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Pagination.CrossFeedRangeStateSplitterAndMerger
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using System;
using System.Collections.Generic;

namespace Microsoft.Azure.Cosmos.Pagination
{
  internal static class CrossFeedRangeStateSplitterAndMerger
  {
    public static Memory<FeedRangeState<TState>> Merge<TState>(
      ReadOnlyMemory<FeedRangeState<TState>> first,
      ReadOnlyMemory<FeedRangeState<TState>> second)
      where TState : State
    {
      return CrossFeedRangeStateSplitterAndMerger.Merge<TState>(first, second, ReadOnlyMemory<FeedRangeState<TState>>.Empty);
    }

    public static Memory<FeedRangeState<TState>> Merge<TState>(
      ReadOnlyMemory<FeedRangeState<TState>> first,
      ReadOnlyMemory<FeedRangeState<TState>> second,
      ReadOnlyMemory<FeedRangeState<TState>> third)
      where TState : State
    {
      FeedRangeState<TState>[] array = new FeedRangeState<TState>[first.Length + second.Length + third.Length];
      Memory<FeedRangeState<TState>> destination = array.AsMemory<FeedRangeState<TState>>();
      first.CopyTo(destination);
      destination = destination.Slice(first.Length);
      second.CopyTo(destination);
      destination = destination.Slice(second.Length);
      third.CopyTo(destination);
      destination = destination.Slice(third.Length);
      return (Memory<FeedRangeState<TState>>) array;
    }

    public static Memory<FeedRangeState<TState>> Merge<TState>(
      IReadOnlyList<ReadOnlyMemory<FeedRangeState<TState>>> ranges)
      where TState : State
    {
      if (ranges == null)
        throw new ArgumentNullException(nameof (ranges));
      int length = 0;
      for (int index = 0; index < ranges.Count; ++index)
        length += ranges[index].Length;
      FeedRangeState<TState>[] array = new FeedRangeState<TState>[length];
      Memory<FeedRangeState<TState>> destination = array.AsMemory<FeedRangeState<TState>>();
      for (int index = 0; index < ranges.Count; ++index)
      {
        ReadOnlyMemory<FeedRangeState<TState>> range = ranges[index];
        range.CopyTo(destination);
        destination = destination.Slice(range.Length);
      }
      return (Memory<FeedRangeState<TState>>) array;
    }

    public static bool TrySplit<TState>(
      ReadOnlyMemory<FeedRangeState<TState>> rangeToSplit,
      out ReadOnlyMemory<FeedRangeState<TState>> first,
      out ReadOnlyMemory<FeedRangeState<TState>> second)
      where TState : State
    {
      if (rangeToSplit.Length <= 1)
      {
        first = new ReadOnlyMemory<FeedRangeState<TState>>();
        second = new ReadOnlyMemory<FeedRangeState<TState>>();
        return false;
      }
      ReadOnlyMemory<FeedRangeState<TState>> readOnlyMemory = rangeToSplit;
      first = readOnlyMemory.Slice(0, rangeToSplit.Length / 2);
      readOnlyMemory = readOnlyMemory.Slice(rangeToSplit.Length / 2);
      second = readOnlyMemory.Slice(0, rangeToSplit.Length / 2);
      readOnlyMemory = readOnlyMemory.Slice(rangeToSplit.Length / 2);
      return true;
    }

    public static bool TrySplit<TState>(
      ReadOnlyMemory<FeedRangeState<TState>> rangeToSplit,
      out ReadOnlyMemory<FeedRangeState<TState>> first,
      out ReadOnlyMemory<FeedRangeState<TState>> second,
      out ReadOnlyMemory<FeedRangeState<TState>> third)
      where TState : State
    {
      if (rangeToSplit.Length <= 1)
      {
        first = new ReadOnlyMemory<FeedRangeState<TState>>();
        second = new ReadOnlyMemory<FeedRangeState<TState>>();
        third = new ReadOnlyMemory<FeedRangeState<TState>>();
        return false;
      }
      ReadOnlyMemory<FeedRangeState<TState>> readOnlyMemory = rangeToSplit;
      first = readOnlyMemory.Slice(0, rangeToSplit.Length / 3);
      readOnlyMemory = readOnlyMemory.Slice(rangeToSplit.Length / 3);
      second = readOnlyMemory.Slice(0, rangeToSplit.Length / 3);
      readOnlyMemory = readOnlyMemory.Slice(rangeToSplit.Length / 3);
      third = readOnlyMemory.Slice(0, rangeToSplit.Length / 3);
      readOnlyMemory = readOnlyMemory.Slice(rangeToSplit.Length / 3);
      return true;
    }

    public static bool TrySplit<TState>(
      ReadOnlyMemory<FeedRangeState<TState>> rangeToSplit,
      int numPartitions,
      out List<ReadOnlyMemory<FeedRangeState<TState>>> partitions)
      where TState : State
    {
      if (rangeToSplit.Length <= 1)
      {
        partitions = (List<ReadOnlyMemory<FeedRangeState<TState>>>) null;
        return false;
      }
      ReadOnlyMemory<FeedRangeState<TState>> readOnlyMemory1 = rangeToSplit;
      partitions = new List<ReadOnlyMemory<FeedRangeState<TState>>>(numPartitions);
      int num = rangeToSplit.Length / numPartitions;
      for (int index = 0; index < numPartitions; ++index)
      {
        ReadOnlyMemory<FeedRangeState<TState>> readOnlyMemory2 = readOnlyMemory1.Slice(0, num);
        readOnlyMemory1 = readOnlyMemory1.Slice(num);
        partitions.Add(readOnlyMemory2);
      }
      return true;
    }
  }
}
