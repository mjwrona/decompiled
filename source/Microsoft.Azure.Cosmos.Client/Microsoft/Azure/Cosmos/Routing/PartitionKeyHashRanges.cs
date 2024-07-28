// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Routing.PartitionKeyHashRanges
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.Query.Core.Monads;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Azure.Cosmos.Routing
{
  internal sealed class PartitionKeyHashRanges : 
    IOrderedEnumerable<PartitionKeyHashRange>,
    IEnumerable<PartitionKeyHashRange>,
    IEnumerable
  {
    private readonly SortedSet<PartitionKeyHashRange> partitionKeyHashRanges;

    private PartitionKeyHashRanges(
      SortedSet<PartitionKeyHashRange> partitionKeyHashRanges)
    {
      this.partitionKeyHashRanges = partitionKeyHashRanges;
    }

    public IOrderedEnumerable<PartitionKeyHashRange> CreateOrderedEnumerable<TKey>(
      Func<PartitionKeyHashRange, TKey> keySelector,
      IComparer<TKey> comparer,
      bool descending)
    {
      return !descending ? this.partitionKeyHashRanges.OrderBy<PartitionKeyHashRange, PartitionKeyHash>((Func<PartitionKeyHashRange, PartitionKeyHash>) (range => range.StartInclusive.Value)).ThenBy<PartitionKeyHashRange, TKey>(keySelector, comparer) : this.partitionKeyHashRanges.OrderByDescending<PartitionKeyHashRange, PartitionKeyHash>((Func<PartitionKeyHashRange, PartitionKeyHash>) (range => range.StartInclusive.Value)).ThenByDescending<PartitionKeyHashRange, TKey>(keySelector, comparer);
    }

    public IEnumerator<PartitionKeyHashRange> GetEnumerator() => (IEnumerator<PartitionKeyHashRange>) this.partitionKeyHashRanges.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.partitionKeyHashRanges.GetEnumerator();

    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append("[");
      foreach (PartitionKeyHashRange partitionKeyHashRange in this.partitionKeyHashRanges)
      {
        stringBuilder.Append(partitionKeyHashRange.ToString());
        stringBuilder.Append(",");
      }
      stringBuilder.Append("]");
      return stringBuilder.ToString();
    }

    public static PartitionKeyHashRanges Create(
      IEnumerable<PartitionKeyHashRange> partitionKeyHashRanges)
    {
      TryCatch<PartitionKeyHashRanges> tryCatch = PartitionKeyHashRanges.Monadic.Create(partitionKeyHashRanges);
      tryCatch.ThrowIfFailed();
      return tryCatch.Result;
    }

    public static PartitionKeyHashRanges.CreateOutcome TryCreate(
      IEnumerable<PartitionKeyHashRange> partitionKeyHashRanges,
      out PartitionKeyHashRanges partitionedSortedEffectiveRanges)
    {
      if (partitionKeyHashRanges == null)
      {
        partitionedSortedEffectiveRanges = (PartitionKeyHashRanges) null;
        return PartitionKeyHashRanges.CreateOutcome.NullPartitionKeyRanges;
      }
      if (partitionKeyHashRanges.Count<PartitionKeyHashRange>() == 0)
      {
        partitionedSortedEffectiveRanges = (PartitionKeyHashRanges) null;
        return PartitionKeyHashRanges.CreateOutcome.NoPartitionKeyRanges;
      }
      SortedSet<PartitionKeyHashRange> partitionKeyHashRanges1 = new SortedSet<PartitionKeyHashRange>();
      foreach (PartitionKeyHashRange partitionKeyHashRange in partitionKeyHashRanges)
      {
        PartitionKeyHash? nullable = partitionKeyHashRange.StartInclusive;
        if (nullable.Equals((object) partitionKeyHashRange.EndExclusive))
        {
          nullable = partitionKeyHashRange.StartInclusive;
          if (nullable.HasValue)
          {
            nullable = partitionKeyHashRange.EndExclusive;
            if (nullable.HasValue)
            {
              partitionedSortedEffectiveRanges = (PartitionKeyHashRanges) null;
              return PartitionKeyHashRanges.CreateOutcome.EmptyPartitionKeyRange;
            }
          }
        }
        if (!partitionKeyHashRanges1.Add(partitionKeyHashRange))
        {
          partitionedSortedEffectiveRanges = (PartitionKeyHashRanges) null;
          return PartitionKeyHashRanges.CreateOutcome.DuplicatePartitionKeyRange;
        }
      }
      UInt128 uint128_1 = UInt128.MaxValue;
      UInt128 uint128_2 = UInt128.MinValue;
      UInt128 uint128_3 = (UInt128) 0;
      bool flag = false;
      foreach (PartitionKeyHashRange partitionKeyHashRange in partitionKeyHashRanges1)
      {
        PartitionKeyHash? nullable = partitionKeyHashRange.StartInclusive;
        PartitionKeyHash valueOrDefault;
        if (nullable.HasValue)
        {
          nullable = partitionKeyHashRange.StartInclusive;
          valueOrDefault = nullable.Value;
          if (valueOrDefault.Value < uint128_1)
          {
            nullable = partitionKeyHashRange.StartInclusive;
            valueOrDefault = nullable.Value;
            uint128_1 = valueOrDefault.Value;
          }
        }
        else
          uint128_1 = UInt128.MinValue;
        nullable = partitionKeyHashRange.EndExclusive;
        if (nullable.HasValue)
        {
          nullable = partitionKeyHashRange.EndExclusive;
          valueOrDefault = nullable.Value;
          if (valueOrDefault.Value > uint128_2)
          {
            nullable = partitionKeyHashRange.EndExclusive;
            valueOrDefault = nullable.Value;
            uint128_2 = valueOrDefault.Value;
          }
        }
        else
          uint128_2 = UInt128.MaxValue;
        nullable = partitionKeyHashRange.EndExclusive;
        valueOrDefault = nullable.GetValueOrDefault(new PartitionKeyHash(UInt128.MaxValue));
        UInt128 uint128_4 = valueOrDefault.Value;
        nullable = partitionKeyHashRange.StartInclusive;
        valueOrDefault = nullable.GetValueOrDefault(new PartitionKeyHash(UInt128.MinValue));
        UInt128 uint128_5 = valueOrDefault.Value;
        UInt128 uint128_6 = uint128_4 - uint128_5;
        uint128_3 += uint128_6;
        if (uint128_3 < uint128_6)
          flag = true;
      }
      UInt128 uint128_7 = uint128_2 - uint128_1;
      PartitionKeyHashRanges.CreateOutcome createOutcome;
      if (uint128_7 < uint128_3 | flag)
      {
        partitionedSortedEffectiveRanges = (PartitionKeyHashRanges) null;
        createOutcome = PartitionKeyHashRanges.CreateOutcome.RangesOverlap;
      }
      else if (uint128_7 > uint128_3)
      {
        partitionedSortedEffectiveRanges = (PartitionKeyHashRanges) null;
        createOutcome = PartitionKeyHashRanges.CreateOutcome.RangesAreNotContiguous;
      }
      else
      {
        partitionedSortedEffectiveRanges = new PartitionKeyHashRanges(partitionKeyHashRanges1);
        createOutcome = PartitionKeyHashRanges.CreateOutcome.Success;
      }
      return createOutcome;
    }

    public static class Monadic
    {
      public static TryCatch<PartitionKeyHashRanges> Create(
        IEnumerable<PartitionKeyHashRange> partitionKeyHashRanges)
      {
        PartitionKeyHashRanges partitionedSortedEffectiveRanges;
        PartitionKeyHashRanges.CreateOutcome createOutcome = PartitionKeyHashRanges.TryCreate(partitionKeyHashRanges, out partitionedSortedEffectiveRanges);
        switch (createOutcome)
        {
          case PartitionKeyHashRanges.CreateOutcome.DuplicatePartitionKeyRange:
            return TryCatch<PartitionKeyHashRanges>.FromException((Exception) new ArgumentException("partitionKeyHashRanges must not have duplicate values."));
          case PartitionKeyHashRanges.CreateOutcome.EmptyPartitionKeyRange:
            return TryCatch<PartitionKeyHashRanges>.FromException((Exception) new ArgumentException("partitionKeyHashRanges must not have an empty range."));
          case PartitionKeyHashRanges.CreateOutcome.NoPartitionKeyRanges:
            return TryCatch<PartitionKeyHashRanges>.FromException((Exception) new ArgumentException("partitionKeyHashRanges must not be empty."));
          case PartitionKeyHashRanges.CreateOutcome.NullPartitionKeyRanges:
            return TryCatch<PartitionKeyHashRanges>.FromException((Exception) new ArgumentNullException(nameof (partitionKeyHashRanges)));
          case PartitionKeyHashRanges.CreateOutcome.RangesAreNotContiguous:
            return TryCatch<PartitionKeyHashRanges>.FromException((Exception) new ArgumentException("partitionKeyHashRanges must have contiguous ranges."));
          case PartitionKeyHashRanges.CreateOutcome.RangesOverlap:
            return TryCatch<PartitionKeyHashRanges>.FromException((Exception) new ArgumentException("partitionKeyHashRanges must not overlapping ranges."));
          case PartitionKeyHashRanges.CreateOutcome.Success:
            return TryCatch<PartitionKeyHashRanges>.FromResult(partitionedSortedEffectiveRanges);
          default:
            throw new ArgumentOutOfRangeException(string.Format("Unknown {0}: {1}.", (object) "CreateOutcome", (object) createOutcome));
        }
      }
    }

    public enum CreateOutcome
    {
      DuplicatePartitionKeyRange,
      EmptyPartitionKeyRange,
      NoPartitionKeyRanges,
      NullPartitionKeyRanges,
      RangesAreNotContiguous,
      RangesOverlap,
      Success,
    }
  }
}
