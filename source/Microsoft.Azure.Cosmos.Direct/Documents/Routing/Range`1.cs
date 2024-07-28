// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.Routing.Range`1
// Assembly: Microsoft.Azure.Cosmos.Direct, Version=3.29.4.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: FFE3C00D-4333-4294-8947-B1C93A852E2F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Direct.dll

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.Azure.Documents.Routing
{
  [JsonObject(MemberSerialization.OptIn)]
  internal sealed class Range<T> where T : IComparable<T>
  {
    public static readonly IComparer<T> TComparer = typeof (T) == typeof (string) ? (IComparer<T>) StringComparer.Ordinal : (IComparer<T>) Comparer<T>.Default;

    [JsonConstructor]
    public Range(T min, T max, bool isMinInclusive, bool isMaxInclusive)
    {
      if ((object) min == null)
        throw new ArgumentNullException(nameof (min));
      if ((object) max == null)
        throw new ArgumentNullException(nameof (max));
      this.Min = min;
      this.Max = max;
      this.IsMinInclusive = isMinInclusive;
      this.IsMaxInclusive = isMaxInclusive;
    }

    public static Range<T> GetPointRange(T value) => new Range<T>(value, value, true, true);

    [JsonProperty("min")]
    public T Min { get; private set; }

    [JsonProperty("max")]
    public T Max { get; private set; }

    [JsonProperty("isMinInclusive")]
    public bool IsMinInclusive { get; private set; }

    [JsonProperty("isMaxInclusive")]
    public bool IsMaxInclusive { get; private set; }

    public bool IsSingleValue => this.IsMinInclusive && this.IsMaxInclusive && Range<T>.TComparer.Compare(this.Min, this.Max) == 0;

    public bool IsEmpty
    {
      get
      {
        if (Range<T>.TComparer.Compare(this.Min, this.Max) != 0)
          return false;
        return !this.IsMinInclusive || !this.IsMaxInclusive;
      }
    }

    public static Range<T> GetEmptyRange(T value) => new Range<T>(value, value, true, false);

    public bool Contains(T value)
    {
      if ((object) value == null)
        throw new ArgumentNullException(nameof (value));
      int num1 = Range<T>.TComparer.Compare(this.Min, value);
      int num2 = Range<T>.TComparer.Compare(this.Max, value);
      if ((!this.IsMinInclusive || num1 > 0) && (this.IsMinInclusive || num1 >= 0))
        return false;
      if (this.IsMaxInclusive && num2 >= 0)
        return true;
      return !this.IsMaxInclusive && num2 > 0;
    }

    public static bool CheckOverlapping(Range<T> range1, Range<T> range2)
    {
      if (range1 == null || range2 == null || range1.IsEmpty || range2.IsEmpty)
        return false;
      int num1 = Range<T>.TComparer.Compare(range1.Min, range2.Max);
      int num2 = Range<T>.TComparer.Compare(range2.Min, range1.Max);
      return num1 <= 0 && num2 <= 0 && (num1 != 0 || range1.IsMinInclusive && range2.IsMaxInclusive) && (num2 != 0 || range2.IsMinInclusive && range1.IsMaxInclusive);
    }

    public override bool Equals(object obj) => this.Equals(obj as Range<T>);

    public override int GetHashCode() => (((0 * 397 ^ this.Min.GetHashCode()) * 397 ^ this.Max.GetHashCode()) * 397 ^ Convert.ToInt32(this.IsMinInclusive)) * 397 ^ Convert.ToInt32(this.IsMaxInclusive);

    public bool Equals(Range<T> other) => other != null && Range<T>.TComparer.Compare(this.Min, other.Min) == 0 && Range<T>.TComparer.Compare(this.Max, other.Max) == 0 && this.IsMinInclusive == other.IsMinInclusive && this.IsMaxInclusive == other.IsMaxInclusive;

    public override string ToString() => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}{1},{2}{3}", this.IsMinInclusive ? (object) "[" : (object) "(", (object) this.Min, (object) this.Max, this.IsMaxInclusive ? (object) "]" : (object) ")");

    public class MinComparer : IComparer<Range<T>>
    {
      public static readonly Range<T>.MinComparer Instance = new Range<T>.MinComparer(Range<T>.TComparer);
      private readonly IComparer<T> boundsComparer;

      private MinComparer(IComparer<T> boundsComparer) => this.boundsComparer = boundsComparer;

      public int Compare(Range<T> left, Range<T> right)
      {
        int num = this.boundsComparer.Compare(left.Min, right.Min);
        if (num != 0 || left.IsMinInclusive == right.IsMinInclusive)
          return num;
        return left.IsMinInclusive ? -1 : 1;
      }
    }

    public class MaxComparer : IComparer<Range<T>>
    {
      public static readonly Range<T>.MaxComparer Instance = new Range<T>.MaxComparer(Range<T>.TComparer);
      private readonly IComparer<T> boundsComparer;

      private MaxComparer(IComparer<T> boundsComparer) => this.boundsComparer = boundsComparer;

      public int Compare(Range<T> left, Range<T> right)
      {
        int num = this.boundsComparer.Compare(left.Max, right.Max);
        if (num != 0 || left.IsMaxInclusive == right.IsMaxInclusive)
          return num;
        return left.IsMaxInclusive ? 1 : -1;
      }
    }
  }
}
