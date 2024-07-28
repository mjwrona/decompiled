// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.Partitioning.Range`1
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using System;

namespace Microsoft.Azure.Documents.Partitioning
{
  [Obsolete("Support for classes used with IPartitionResolver is now obsolete. It's recommended that you use partitioned collections for higher storage and throughput.")]
  public class Range<T> : IEquatable<Range<T>>, IComparable<Range<T>> where T : IComparable<T>, IEquatable<T>
  {
    public T Low { get; private set; }

    public T High { get; private set; }

    public Range(T low, T high)
    {
      this.Low = low.CompareTo(high) <= 0 ? low : throw new ArgumentException(ClientResources.InvalidRangeError);
      this.High = high;
    }

    public Range(T point) => this.Low = this.High = point;

    public bool Equals(Range<T> other) => this.Low.Equals(other.Low) && this.High.Equals(other.High);

    public int CompareTo(Range<T> other)
    {
      if (this.Equals(other))
        return 0;
      T obj = this.Low;
      if (obj.CompareTo(other.Low) >= 0)
      {
        obj = this.High;
        if (obj.CompareTo(other.High) >= 0)
          return 1;
      }
      return -1;
    }

    public bool Contains(T point) => point.CompareTo(this.Low) >= 0 && point.CompareTo(this.High) <= 0;

    public bool Contains(Range<T> other) => other.Low.CompareTo(this.Low) >= 0 && other.High.CompareTo(this.High) <= 0;

    public override int GetHashCode()
    {
      T obj = this.Low;
      int hashCode1 = obj.GetHashCode();
      obj = this.High;
      int hashCode2 = obj.GetHashCode();
      return hashCode1 + hashCode2;
    }

    public bool Intersect(Range<T> other) => (this.Low.CompareTo(other.Low) >= 0 ? this.Low : other.Low).CompareTo(this.High.CompareTo(other.High) <= 0 ? this.High : other.High) <= 0;

    public override string ToString() => string.Join(",", new string[2]
    {
      this.Low.ToString(),
      this.High.ToString()
    });
  }
}
