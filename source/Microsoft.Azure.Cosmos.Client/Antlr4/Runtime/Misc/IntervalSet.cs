// Decompiled with JetBrains decompiler
// Type: Antlr4.Runtime.Misc.IntervalSet
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Antlr4.Runtime.Misc
{
  internal class IntervalSet : IIntSet
  {
    public static readonly IntervalSet CompleteCharSet = IntervalSet.Of(0, 1114111);
    public static readonly IntervalSet EmptySet = new IntervalSet(Array.Empty<int>());
    protected internal IList<Interval> intervals;
    protected internal bool @readonly;

    static IntervalSet()
    {
      IntervalSet.CompleteCharSet.SetReadonly(true);
      IntervalSet.EmptySet.SetReadonly(true);
    }

    public IntervalSet(IList<Interval> intervals) => this.intervals = intervals;

    public IntervalSet(IntervalSet set)
      : this()
    {
      this.AddAll((IIntSet) set);
    }

    public IntervalSet(params int[] els)
    {
      if (els == null)
      {
        this.intervals = (IList<Interval>) new ArrayList<Interval>(2);
      }
      else
      {
        this.intervals = (IList<Interval>) new ArrayList<Interval>(els.Length);
        foreach (int el in els)
          this.Add(el);
      }
    }

    [return: NotNull]
    public static IntervalSet Of(int a)
    {
      IntervalSet intervalSet = new IntervalSet(Array.Empty<int>());
      intervalSet.Add(a);
      return intervalSet;
    }

    public static IntervalSet Of(int a, int b)
    {
      IntervalSet intervalSet = new IntervalSet(Array.Empty<int>());
      intervalSet.Add(a, b);
      return intervalSet;
    }

    public virtual void Clear()
    {
      if (this.@readonly)
        throw new InvalidOperationException("can't alter readonly IntervalSet");
      this.intervals.Clear();
    }

    public virtual void Add(int el)
    {
      if (this.@readonly)
        throw new InvalidOperationException("can't alter readonly IntervalSet");
      this.Add(el, el);
    }

    public virtual void Add(int a, int b) => this.Add(Interval.Of(a, b));

    protected internal virtual void Add(Interval addition)
    {
      if (this.@readonly)
        throw new InvalidOperationException("can't alter readonly IntervalSet");
      if (addition.b < addition.a)
        return;
      for (int index1 = 0; index1 < this.intervals.Count; ++index1)
      {
        Interval interval1 = this.intervals[index1];
        if (addition.Equals((object) interval1))
          return;
        if (addition.Adjacent(interval1) || !addition.Disjoint(interval1))
        {
          Interval interval2 = addition.Union(interval1);
          this.intervals[index1] = interval2;
          while (index1 < this.intervals.Count - 1)
          {
            int index2 = index1 + 1;
            Interval interval3 = this.intervals[index2];
            if (!interval2.Adjacent(interval3) && interval2.Disjoint(interval3))
              break;
            this.intervals.RemoveAt(index2);
            index1 = index2 - 1;
            this.intervals[index1] = interval2.Union(interval3);
          }
          return;
        }
        if (addition.StartsBeforeDisjoint(interval1))
        {
          this.intervals.Insert(index1, addition);
          return;
        }
      }
      this.intervals.Add(addition);
    }

    public static IntervalSet Or(IntervalSet[] sets)
    {
      IntervalSet intervalSet = new IntervalSet(Array.Empty<int>());
      foreach (IntervalSet set in sets)
        intervalSet.AddAll((IIntSet) set);
      return intervalSet;
    }

    public virtual IntervalSet AddAll(IIntSet set)
    {
      if (set == null)
        return this;
      if (set is IntervalSet)
      {
        IntervalSet intervalSet = (IntervalSet) set;
        int count = intervalSet.intervals.Count;
        for (int index = 0; index < count; ++index)
        {
          Interval interval = intervalSet.intervals[index];
          this.Add(interval.a, interval.b);
        }
      }
      else
      {
        foreach (int el in (IEnumerable<int>) set.ToList())
          this.Add(el);
      }
      return this;
    }

    public virtual IntervalSet Complement(int minElement, int maxElement) => this.Complement((IIntSet) IntervalSet.Of(minElement, maxElement));

    public virtual IntervalSet Complement(IIntSet vocabulary)
    {
      if (vocabulary == null || vocabulary.IsNil)
        return (IntervalSet) null;
      IntervalSet intervalSet;
      if (vocabulary is IntervalSet)
      {
        intervalSet = (IntervalSet) vocabulary;
      }
      else
      {
        intervalSet = new IntervalSet(Array.Empty<int>());
        intervalSet.AddAll(vocabulary);
      }
      return intervalSet.Subtract((IIntSet) this);
    }

    public virtual IntervalSet Subtract(IIntSet a)
    {
      if (a == null || a.IsNil)
        return new IntervalSet(this);
      if (a is IntervalSet)
        return IntervalSet.Subtract(this, (IntervalSet) a);
      IntervalSet right = new IntervalSet(Array.Empty<int>());
      right.AddAll(a);
      return IntervalSet.Subtract(this, right);
    }

    [return: NotNull]
    public static IntervalSet Subtract(IntervalSet left, IntervalSet right)
    {
      if (left == null || left.IsNil)
        return new IntervalSet(Array.Empty<int>());
      IntervalSet intervalSet = new IntervalSet(left);
      if (right == null || right.IsNil)
        return intervalSet;
      int index1 = 0;
      int index2 = 0;
      while (index1 < intervalSet.intervals.Count && index2 < right.intervals.Count)
      {
        Interval interval1 = intervalSet.intervals[index1];
        Interval interval2 = right.intervals[index2];
        if (interval2.b < interval1.a)
          ++index2;
        else if (interval2.a > interval1.b)
        {
          ++index1;
        }
        else
        {
          Interval? nullable1 = new Interval?();
          Interval? nullable2 = new Interval?();
          if (interval2.a > interval1.a)
            nullable1 = new Interval?(new Interval(interval1.a, interval2.a - 1));
          if (interval2.b < interval1.b)
            nullable2 = new Interval?(new Interval(interval2.b + 1, interval1.b));
          if (nullable1.HasValue)
          {
            if (nullable2.HasValue)
            {
              intervalSet.intervals[index1] = nullable1.Value;
              intervalSet.intervals.Insert(index1 + 1, nullable2.Value);
              ++index1;
              ++index2;
            }
            else
            {
              intervalSet.intervals[index1] = nullable1.Value;
              ++index1;
            }
          }
          else if (nullable2.HasValue)
          {
            intervalSet.intervals[index1] = nullable2.Value;
            ++index2;
          }
          else
            intervalSet.intervals.RemoveAt(index1);
        }
      }
      return intervalSet;
    }

    public virtual IntervalSet Or(IIntSet a)
    {
      IntervalSet intervalSet = new IntervalSet(Array.Empty<int>());
      intervalSet.AddAll((IIntSet) this);
      intervalSet.AddAll(a);
      return intervalSet;
    }

    public virtual IntervalSet And(IIntSet other)
    {
      if (other == null)
        return (IntervalSet) null;
      IList<Interval> intervals1 = this.intervals;
      IList<Interval> intervals2 = ((IntervalSet) other).intervals;
      IntervalSet intervalSet = (IntervalSet) null;
      int count1 = intervals1.Count;
      int count2 = intervals2.Count;
      int index1 = 0;
      int index2 = 0;
      while (index1 < count1 && index2 < count2)
      {
        Interval other1 = intervals1[index1];
        Interval other2 = intervals2[index2];
        if (other1.StartsBeforeDisjoint(other2))
          ++index1;
        else if (other2.StartsBeforeDisjoint(other1))
          ++index2;
        else if (other1.ProperlyContains(other2))
        {
          if (intervalSet == null)
            intervalSet = new IntervalSet(Array.Empty<int>());
          intervalSet.Add(other1.Intersection(other2));
          ++index2;
        }
        else if (other2.ProperlyContains(other1))
        {
          if (intervalSet == null)
            intervalSet = new IntervalSet(Array.Empty<int>());
          intervalSet.Add(other1.Intersection(other2));
          ++index1;
        }
        else if (!other1.Disjoint(other2))
        {
          if (intervalSet == null)
            intervalSet = new IntervalSet(Array.Empty<int>());
          intervalSet.Add(other1.Intersection(other2));
          if (other1.StartsAfterNonDisjoint(other2))
            ++index2;
          else if (other2.StartsAfterNonDisjoint(other1))
            ++index1;
        }
      }
      return intervalSet ?? new IntervalSet(Array.Empty<int>());
    }

    public virtual bool Contains(int el)
    {
      int count = this.intervals.Count;
      for (int index = 0; index < count; ++index)
      {
        Interval interval = this.intervals[index];
        int a = interval.a;
        int b = interval.b;
        if (el >= a)
        {
          if (el >= a && el <= b)
            return true;
        }
        else
          break;
      }
      return false;
    }

    public virtual bool IsNil => this.intervals == null || this.intervals.Count == 0;

    public virtual int SingleElement
    {
      get
      {
        if (this.intervals != null && this.intervals.Count == 1)
        {
          Interval interval = this.intervals[0];
          if (interval.a == interval.b)
            return interval.a;
        }
        return 0;
      }
    }

    public virtual int MaxElement => this.IsNil ? 0 : this.intervals[this.intervals.Count - 1].b;

    public virtual int MinElement => this.IsNil ? 0 : this.intervals[0].a;

    public virtual IList<Interval> GetIntervals() => this.intervals;

    public override int GetHashCode()
    {
      int hash = MurmurHash.Initialize();
      foreach (Interval interval in (IEnumerable<Interval>) this.intervals)
      {
        hash = MurmurHash.Update(hash, interval.a);
        hash = MurmurHash.Update(hash, interval.b);
      }
      return MurmurHash.Finish(hash, this.intervals.Count * 2);
    }

    public override bool Equals(object obj) => obj != null && obj is IntervalSet && this.intervals.SequenceEqual<Interval>((IEnumerable<Interval>) ((IntervalSet) obj).intervals);

    public override string ToString() => this.ToString(false);

    public virtual string ToString(bool elemAreChar)
    {
      StringBuilder stringBuilder = new StringBuilder();
      if (this.intervals == null || this.intervals.Count == 0)
        return "{}";
      if (this.Count > 1)
        stringBuilder.Append("{");
      bool flag = true;
      foreach (Interval interval in (IEnumerable<Interval>) this.intervals)
      {
        if (!flag)
          stringBuilder.Append(", ");
        flag = false;
        int a = interval.a;
        int b = interval.b;
        if (a == b)
        {
          if (a == -1)
            stringBuilder.Append("<EOF>");
          else if (elemAreChar)
            stringBuilder.Append("'").Append((char) a).Append("'");
          else
            stringBuilder.Append(a);
        }
        else if (elemAreChar)
          stringBuilder.Append("'").Append((char) a).Append("'..'").Append((char) b).Append("'");
        else
          stringBuilder.Append(a).Append("..").Append(b);
      }
      if (this.Count > 1)
        stringBuilder.Append("}");
      return stringBuilder.ToString();
    }

    public virtual string ToString(IVocabulary vocabulary)
    {
      StringBuilder stringBuilder = new StringBuilder();
      if (this.intervals == null || this.intervals.Count == 0)
        return "{}";
      if (this.Count > 1)
        stringBuilder.Append("{");
      bool flag = true;
      foreach (Interval interval in (IEnumerable<Interval>) this.intervals)
      {
        if (!flag)
          stringBuilder.Append(", ");
        flag = false;
        int a1 = interval.a;
        int b = interval.b;
        if (a1 == b)
        {
          stringBuilder.Append(this.ElementName(vocabulary, a1));
        }
        else
        {
          for (int a2 = a1; a2 <= b; ++a2)
          {
            if (a2 > a1)
              stringBuilder.Append(", ");
            stringBuilder.Append(this.ElementName(vocabulary, a2));
          }
        }
      }
      if (this.Count > 1)
        stringBuilder.Append("}");
      return stringBuilder.ToString();
    }

    [return: NotNull]
    protected internal virtual string ElementName(IVocabulary vocabulary, int a)
    {
      if (a == -1)
        return "<EOF>";
      return a == -2 ? "<EPSILON>" : vocabulary.GetDisplayName(a);
    }

    public virtual int Count
    {
      get
      {
        int count1 = 0;
        int count2 = this.intervals.Count;
        if (count2 == 1)
        {
          Interval interval = this.intervals[0];
          return interval.b - interval.a + 1;
        }
        for (int index = 0; index < count2; ++index)
        {
          Interval interval = this.intervals[index];
          count1 += interval.b - interval.a + 1;
        }
        return count1;
      }
    }

    public virtual ArrayList<int> ToIntegerList()
    {
      ArrayList<int> integerList = new ArrayList<int>(this.Count);
      int count = this.intervals.Count;
      for (int index1 = 0; index1 < count; ++index1)
      {
        Interval interval = this.intervals[index1];
        int a = interval.a;
        int b = interval.b;
        for (int index2 = a; index2 <= b; ++index2)
          integerList.Add(index2);
      }
      return integerList;
    }

    public virtual IList<int> ToList()
    {
      IList<int> list = (IList<int>) new ArrayList<int>();
      int count = this.intervals.Count;
      for (int index1 = 0; index1 < count; ++index1)
      {
        Interval interval = this.intervals[index1];
        int a = interval.a;
        int b = interval.b;
        for (int index2 = a; index2 <= b; ++index2)
          list.Add(index2);
      }
      return list;
    }

    public virtual HashSet<int> ToSet()
    {
      HashSet<int> set = new HashSet<int>();
      foreach (Interval interval in (IEnumerable<Interval>) this.intervals)
      {
        int a = interval.a;
        int b = interval.b;
        for (int index = a; index <= b; ++index)
          set.Add(index);
      }
      return set;
    }

    public virtual int[] ToArray() => this.ToIntegerList().ToArray();

    public virtual void Remove(int el)
    {
      if (this.@readonly)
        throw new InvalidOperationException("can't alter readonly IntervalSet");
      int count = this.intervals.Count;
      for (int index = 0; index < count; ++index)
      {
        Interval interval = this.intervals[index];
        int a = interval.a;
        int b1 = interval.b;
        if (el < a)
          break;
        if (el == a && el == b1)
        {
          this.intervals.RemoveAt(index);
          break;
        }
        if (el == a)
        {
          this.intervals[index] = Interval.Of(interval.a + 1, interval.b);
          break;
        }
        if (el == b1)
        {
          this.intervals[index] = Interval.Of(interval.a, interval.b - 1);
          break;
        }
        if (el > a && el < b1)
        {
          int b2 = interval.b;
          this.intervals[index] = Interval.Of(interval.a, el - 1);
          this.Add(el + 1, b2);
        }
      }
    }

    public virtual bool IsReadOnly => this.@readonly;

    public virtual void SetReadonly(bool @readonly) => this.@readonly = !this.@readonly || @readonly ? @readonly : throw new InvalidOperationException("can't alter readonly IntervalSet");

    IIntSet IIntSet.AddAll(IIntSet set) => (IIntSet) this.AddAll(set);

    IIntSet IIntSet.And(IIntSet a) => (IIntSet) this.And(a);

    IIntSet IIntSet.Complement(IIntSet elements) => (IIntSet) this.Complement(elements);

    IIntSet IIntSet.Or(IIntSet a) => (IIntSet) this.Or(a);

    IIntSet IIntSet.Subtract(IIntSet a) => (IIntSet) this.Subtract(a);
  }
}
