// Decompiled with JetBrains decompiler
// Type: Antlr4.Runtime.Misc.Interval
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using System;

namespace Antlr4.Runtime.Misc
{
  internal struct Interval
  {
    public static readonly Interval Invalid = new Interval(-1, -2);
    public readonly int a;
    public readonly int b;

    public Interval(int a, int b)
    {
      this.a = a;
      this.b = b;
    }

    public static Interval Of(int a, int b) => new Interval(a, b);

    public int Length => this.b < this.a ? 0 : this.b - this.a + 1;

    public override bool Equals(object o) => o is Interval interval && this.a == interval.a && this.b == interval.b;

    public override int GetHashCode() => (23 * 31 + this.a) * 31 + this.b;

    public bool StartsBeforeDisjoint(Interval other) => this.a < other.a && this.b < other.a;

    public bool StartsBeforeNonDisjoint(Interval other) => this.a <= other.a && this.b >= other.a;

    public bool StartsAfter(Interval other) => this.a > other.a;

    public bool StartsAfterDisjoint(Interval other) => this.a > other.b;

    public bool StartsAfterNonDisjoint(Interval other) => this.a > other.a && this.a <= other.b;

    public bool Disjoint(Interval other) => this.StartsBeforeDisjoint(other) || this.StartsAfterDisjoint(other);

    public bool Adjacent(Interval other) => this.a == other.b + 1 || this.b == other.a - 1;

    public bool ProperlyContains(Interval other) => other.a >= this.a && other.b <= this.b;

    public Interval Union(Interval other) => Interval.Of(Math.Min(this.a, other.a), Math.Max(this.b, other.b));

    public Interval Intersection(Interval other) => Interval.Of(Math.Max(this.a, other.a), Math.Min(this.b, other.b));

    public Interval? DifferenceNotProperlyContained(Interval other)
    {
      Interval? nullable = new Interval?();
      if (other.StartsBeforeNonDisjoint(this))
        nullable = new Interval?(Interval.Of(Math.Max(this.a, other.b + 1), this.b));
      else if (other.StartsAfterNonDisjoint(this))
        nullable = new Interval?(Interval.Of(this.a, other.a - 1));
      return nullable;
    }

    public override string ToString()
    {
      int num = this.a;
      string str1 = num.ToString();
      num = this.b;
      string str2 = num.ToString();
      return str1 + ".." + str2;
    }
  }
}
