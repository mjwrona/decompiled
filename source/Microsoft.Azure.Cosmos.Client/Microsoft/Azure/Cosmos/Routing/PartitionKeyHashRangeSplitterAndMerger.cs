// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Routing.PartitionKeyHashRangeSplitterAndMerger
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Azure.Cosmos.Routing
{
  internal abstract class PartitionKeyHashRangeSplitterAndMerger
  {
    public abstract PartitionKeyHashRange FullRange { get; }

    public static PartitionKeyHashRanges SplitRange(
      PartitionKeyHashRange partitionKeyHashRange,
      int rangeCount)
    {
      PartitionKeyHashRanges splitRanges;
      PartitionKeyHashRangeSplitterAndMerger.SplitOutcome splitOutcome = PartitionKeyHashRangeSplitterAndMerger.TrySplitRange(partitionKeyHashRange, rangeCount, out splitRanges);
      switch (splitOutcome)
      {
        case PartitionKeyHashRangeSplitterAndMerger.SplitOutcome.Success:
          return splitRanges;
        case PartitionKeyHashRangeSplitterAndMerger.SplitOutcome.NumRangesNeedsToBeGreaterThanZero:
          throw new ArgumentOutOfRangeException("rangeCount must be a positive integer");
        case PartitionKeyHashRangeSplitterAndMerger.SplitOutcome.RangeNotWideEnough:
          throw new ArgumentOutOfRangeException(string.Format("{0} is not wide enough to split into {1} ranges.", (object) nameof (partitionKeyHashRange), (object) rangeCount));
        default:
          throw new ArgumentOutOfRangeException(string.Format("Unknown {0}: {1}.", (object) "SplitOutcome", (object) splitOutcome));
      }
    }

    public static PartitionKeyHashRangeSplitterAndMerger.SplitOutcome TrySplitRange(
      PartitionKeyHashRange partitionKeyHashRange,
      int rangeCount,
      out PartitionKeyHashRanges splitRanges)
    {
      if (rangeCount < 1)
      {
        splitRanges = (PartitionKeyHashRanges) null;
        return PartitionKeyHashRangeSplitterAndMerger.SplitOutcome.NumRangesNeedsToBeGreaterThanZero;
      }
      UInt128 uint128_1 = partitionKeyHashRange.EndExclusive.HasValue ? partitionKeyHashRange.EndExclusive.Value.Value : UInt128.MaxValue;
      UInt128 uint128_2 = partitionKeyHashRange.StartInclusive.HasValue ? partitionKeyHashRange.StartInclusive.Value.Value : UInt128.MinValue;
      UInt128 uint128_3 = uint128_2;
      UInt128 uint128_4 = uint128_1 - uint128_3;
      if (uint128_4 < (UInt128) rangeCount)
      {
        splitRanges = (PartitionKeyHashRanges) null;
        return PartitionKeyHashRangeSplitterAndMerger.SplitOutcome.RangeNotWideEnough;
      }
      if (rangeCount == 1)
      {
        splitRanges = PartitionKeyHashRanges.Create((IEnumerable<PartitionKeyHashRange>) new PartitionKeyHashRange[1]
        {
          partitionKeyHashRange
        });
        return PartitionKeyHashRangeSplitterAndMerger.SplitOutcome.Success;
      }
      List<PartitionKeyHashRange> partitionKeyHashRangeList = new List<PartitionKeyHashRange>();
      UInt128 uint128_5 = uint128_4 / (UInt128) rangeCount;
      PartitionKeyHash? startInclusive = partitionKeyHashRange.StartInclusive;
      PartitionKeyHash partitionKeyHash1 = new PartitionKeyHash(uint128_2 + uint128_5);
      partitionKeyHashRangeList.Add(new PartitionKeyHashRange(startInclusive, new PartitionKeyHash?(partitionKeyHash1)));
      for (int index = 1; index < rangeCount - 1; ++index)
      {
        PartitionKeyHash partitionKeyHash2 = new PartitionKeyHash(uint128_2 + uint128_5 * (UInt128) index);
        PartitionKeyHash partitionKeyHash3 = new PartitionKeyHash(partitionKeyHash2.Value + uint128_5);
        partitionKeyHashRangeList.Add(new PartitionKeyHashRange(new PartitionKeyHash?(partitionKeyHash2), new PartitionKeyHash?(partitionKeyHash3)));
      }
      PartitionKeyHash partitionKeyHash4 = new PartitionKeyHash(uint128_2 + uint128_5 * (UInt128) (rangeCount - 1));
      PartitionKeyHash? endExclusive = partitionKeyHashRange.EndExclusive;
      partitionKeyHashRangeList.Add(new PartitionKeyHashRange(new PartitionKeyHash?(partitionKeyHash4), endExclusive));
      splitRanges = PartitionKeyHashRanges.Create((IEnumerable<PartitionKeyHashRange>) partitionKeyHashRangeList);
      return PartitionKeyHashRangeSplitterAndMerger.SplitOutcome.Success;
    }

    public static PartitionKeyHashRange MergeRanges(
      PartitionKeyHashRanges partitionedSortedEffectiveRanges)
    {
      if (partitionedSortedEffectiveRanges == null)
        throw new ArgumentNullException(nameof (partitionedSortedEffectiveRanges));
      return new PartitionKeyHashRange(partitionedSortedEffectiveRanges.First<PartitionKeyHashRange>().StartInclusive, partitionedSortedEffectiveRanges.Last<PartitionKeyHashRange>().EndExclusive);
    }

    private sealed class V1 : PartitionKeyHashRangeSplitterAndMerger
    {
      private static readonly PartitionKeyHashRange fullRange = new PartitionKeyHashRange(new PartitionKeyHash?(new PartitionKeyHash((UInt128) 0)), new PartitionKeyHash?(new PartitionKeyHash((UInt128) uint.MaxValue)));

      public override PartitionKeyHashRange FullRange => PartitionKeyHashRangeSplitterAndMerger.V1.fullRange;
    }

    private sealed class V2 : PartitionKeyHashRangeSplitterAndMerger
    {
      private static readonly unsafe PartitionKeyHashRange fullRange = new PartitionKeyHashRange(new PartitionKeyHash?(new PartitionKeyHash((UInt128) 0)), new PartitionKeyHash?(new PartitionKeyHash(UInt128.FromByteArray(new ReadOnlySpan<byte>((void*) &\u003CPrivateImplementationDetails\u003E.\u003655A62D08198088F745220304782C7F02AE898BE2A644614D9AD891A7752266A, 16)))));

      public override PartitionKeyHashRange FullRange => PartitionKeyHashRangeSplitterAndMerger.V2.fullRange;
    }

    public enum SplitOutcome
    {
      Success,
      NumRangesNeedsToBeGreaterThanZero,
      RangeNotWideEnough,
    }
  }
}
