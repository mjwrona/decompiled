// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Routing.PartitionKeyHashRange
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using System;
using System.Text;

namespace Microsoft.Azure.Cosmos.Routing
{
  internal readonly struct PartitionKeyHashRange : 
    IComparable<PartitionKeyHashRange>,
    IEquatable<PartitionKeyHashRange>
  {
    public PartitionKeyHashRange(PartitionKeyHash? startInclusive, PartitionKeyHash? endExclusive)
    {
      if (endExclusive.HasValue && startInclusive.HasValue && endExclusive.Value.CompareTo(startInclusive.Value) < 0)
        throw new ArgumentOutOfRangeException("startInclusive must be less than or equal to endExclusive.");
      this.StartInclusive = startInclusive;
      this.EndExclusive = endExclusive;
    }

    public PartitionKeyHash? StartInclusive { get; }

    public PartitionKeyHash? EndExclusive { get; }

    public bool Contains(PartitionKeyHash partitionKeyHash) => ((!this.StartInclusive.HasValue ? 1 : (this.StartInclusive.Value <= partitionKeyHash ? 1 : 0)) & (!this.EndExclusive.HasValue ? (true ? 1 : 0) : (partitionKeyHash <= this.EndExclusive.Value ? 1 : 0))) != 0;

    public bool Contains(PartitionKeyHashRange partitionKeyHashRange)
    {
      PartitionKeyHash? nullable;
      int num1;
      if (this.StartInclusive.HasValue)
      {
        if (partitionKeyHashRange.StartInclusive.HasValue)
        {
          PartitionKeyHash partitionKeyHash1 = this.StartInclusive.Value;
          nullable = partitionKeyHashRange.StartInclusive;
          PartitionKeyHash partitionKeyHash2 = nullable.Value;
          num1 = partitionKeyHash1 <= partitionKeyHash2 ? 1 : 0;
        }
        else
          num1 = 0;
      }
      else
        num1 = 1;
      nullable = this.EndExclusive;
      int num2;
      if (nullable.HasValue)
      {
        nullable = partitionKeyHashRange.EndExclusive;
        if (nullable.HasValue)
        {
          nullable = partitionKeyHashRange.EndExclusive;
          PartitionKeyHash partitionKeyHash3 = nullable.Value;
          nullable = this.EndExclusive;
          PartitionKeyHash partitionKeyHash4 = nullable.Value;
          num2 = partitionKeyHash3 <= partitionKeyHash4 ? 1 : 0;
        }
        else
          num2 = 0;
      }
      else
        num2 = 1;
      int num3 = num2 != 0 ? 1 : 0;
      return (num1 & num3) != 0;
    }

    public bool TryGetOverlappingRange(
      PartitionKeyHashRange rangeToOverlapWith,
      out PartitionKeyHashRange overlappingRange)
    {
      PartitionKeyHash? startInclusive;
      PartitionKeyHash? nullable1;
      if (this.StartInclusive.HasValue && rangeToOverlapWith.StartInclusive.HasValue)
      {
        ref PartitionKeyHash? local = ref startInclusive;
        PartitionKeyHash partitionKeyHash;
        if (!(this.StartInclusive.Value > rangeToOverlapWith.StartInclusive.Value))
        {
          partitionKeyHash = rangeToOverlapWith.StartInclusive.Value;
        }
        else
        {
          nullable1 = this.StartInclusive;
          partitionKeyHash = nullable1.Value;
        }
        local = new PartitionKeyHash?(partitionKeyHash);
      }
      else
        startInclusive = !this.StartInclusive.HasValue || rangeToOverlapWith.StartInclusive.HasValue ? (this.StartInclusive.HasValue || !rangeToOverlapWith.StartInclusive.HasValue ? new PartitionKeyHash?() : new PartitionKeyHash?(rangeToOverlapWith.StartInclusive.Value)) : new PartitionKeyHash?(this.StartInclusive.Value);
      nullable1 = this.EndExclusive;
      PartitionKeyHash? endExclusive;
      if (nullable1.HasValue)
      {
        nullable1 = rangeToOverlapWith.EndExclusive;
        if (nullable1.HasValue)
        {
          ref PartitionKeyHash? local = ref endExclusive;
          nullable1 = this.EndExclusive;
          PartitionKeyHash partitionKeyHash1 = nullable1.Value;
          nullable1 = rangeToOverlapWith.EndExclusive;
          PartitionKeyHash partitionKeyHash2 = nullable1.Value;
          PartitionKeyHash partitionKeyHash3;
          if (!(partitionKeyHash1 < partitionKeyHash2))
          {
            nullable1 = rangeToOverlapWith.EndExclusive;
            partitionKeyHash3 = nullable1.Value;
          }
          else
          {
            nullable1 = this.EndExclusive;
            partitionKeyHash3 = nullable1.Value;
          }
          local = new PartitionKeyHash?(partitionKeyHash3);
          goto label_19;
        }
      }
      nullable1 = this.EndExclusive;
      if (nullable1.HasValue)
      {
        nullable1 = rangeToOverlapWith.EndExclusive;
        if (!nullable1.HasValue)
        {
          ref PartitionKeyHash? local = ref endExclusive;
          nullable1 = this.EndExclusive;
          PartitionKeyHash partitionKeyHash = nullable1.Value;
          local = new PartitionKeyHash?(partitionKeyHash);
          goto label_19;
        }
      }
      nullable1 = this.EndExclusive;
      if (!nullable1.HasValue)
      {
        nullable1 = rangeToOverlapWith.EndExclusive;
        if (nullable1.HasValue)
        {
          ref PartitionKeyHash? local = ref endExclusive;
          nullable1 = rangeToOverlapWith.EndExclusive;
          PartitionKeyHash partitionKeyHash = nullable1.Value;
          local = new PartitionKeyHash?(partitionKeyHash);
          goto label_19;
        }
      }
      endExclusive = new PartitionKeyHash?();
label_19:
      if (startInclusive.HasValue && endExclusive.HasValue)
      {
        nullable1 = startInclusive;
        PartitionKeyHash? nullable2 = endExclusive;
        if ((nullable1.HasValue & nullable2.HasValue ? (nullable1.GetValueOrDefault() >= nullable2.GetValueOrDefault() ? 1 : 0) : 0) != 0)
        {
          overlappingRange = new PartitionKeyHashRange();
          return false;
        }
      }
      overlappingRange = new PartitionKeyHashRange(startInclusive, endExclusive);
      return true;
    }

    public int CompareTo(PartitionKeyHashRange other)
    {
      int num = !this.StartInclusive.HasValue || !other.StartInclusive.HasValue ? (!this.StartInclusive.HasValue || other.StartInclusive.HasValue ? (this.StartInclusive.HasValue || !other.StartInclusive.HasValue ? 0 : -1) : 1) : this.StartInclusive.Value.CompareTo(other.StartInclusive.Value);
      return num != 0 ? num : (!this.EndExclusive.HasValue || !other.EndExclusive.HasValue ? (!this.EndExclusive.HasValue || other.EndExclusive.HasValue ? (this.EndExclusive.HasValue || !other.EndExclusive.HasValue ? 0 : 1) : -1) : this.EndExclusive.Value.CompareTo(other.EndExclusive.Value));
    }

    public override bool Equals(object obj) => obj is PartitionKeyHashRange other && this.Equals(other);

    public bool Equals(PartitionKeyHashRange other) => this.StartInclusive.Equals((object) other.StartInclusive) && this.EndExclusive.Equals((object) other.EndExclusive);

    public override int GetHashCode()
    {
      PartitionKeyHash? nullable = this.StartInclusive;
      ref PartitionKeyHash? local1 = ref nullable;
      PartitionKeyHash valueOrDefault;
      int num1;
      if (!local1.HasValue)
      {
        num1 = 0;
      }
      else
      {
        valueOrDefault = local1.GetValueOrDefault();
        num1 = valueOrDefault.GetHashCode();
      }
      nullable = this.EndExclusive;
      ref PartitionKeyHash? local2 = ref nullable;
      int num2;
      if (!local2.HasValue)
      {
        num2 = 0;
      }
      else
      {
        valueOrDefault = local2.GetValueOrDefault();
        num2 = valueOrDefault.GetHashCode();
      }
      int num3 = num2;
      return num1 ^ num3;
    }

    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append("[");
      if (this.StartInclusive.HasValue)
        stringBuilder.Append((object) this.StartInclusive.Value.Value);
      stringBuilder.Append(",");
      if (this.EndExclusive.HasValue)
        stringBuilder.Append((object) this.EndExclusive.Value.Value);
      stringBuilder.Append("]");
      return stringBuilder.ToString();
    }
  }
}
