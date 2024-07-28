// Decompiled with JetBrains decompiler
// Type: Nest.IntervalsQueryDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Runtime.Serialization;

namespace Nest
{
  [DataContract]
  public class IntervalsQueryDescriptor<T> : 
    FieldNameQueryDescriptorBase<IntervalsQueryDescriptor<T>, IIntervalsQuery, T>,
    IIntervalsQuery,
    IFieldNameQuery,
    IQuery,
    IIntervalsContainer
    where T : class
  {
    protected override bool Conditionless => IntervalsQuery.IsConditionless((IIntervalsQuery) this);

    IIntervalsAllOf IIntervalsContainer.AllOf { get; set; }

    IIntervalsAnyOf IIntervalsContainer.AnyOf { get; set; }

    IIntervalsFuzzy IIntervalsContainer.Fuzzy { get; set; }

    IIntervalsMatch IIntervalsContainer.Match { get; set; }

    IIntervalsPrefix IIntervalsContainer.Prefix { get; set; }

    IIntervalsWildcard IIntervalsContainer.Wildcard { get; set; }

    public IntervalsQueryDescriptor<T> Fuzzy(
      Func<IntervalsFuzzyDescriptor, IIntervalsFuzzy> selector)
    {
      return this.Assign<Func<IntervalsFuzzyDescriptor, IIntervalsFuzzy>>(selector, (Action<IIntervalsQuery, Func<IntervalsFuzzyDescriptor, IIntervalsFuzzy>>) ((a, v) => a.Fuzzy = v != null ? v(new IntervalsFuzzyDescriptor()) : (IIntervalsFuzzy) null));
    }

    public IntervalsQueryDescriptor<T> Match(
      Func<IntervalsMatchDescriptor, IIntervalsMatch> selector)
    {
      return this.Assign<Func<IntervalsMatchDescriptor, IIntervalsMatch>>(selector, (Action<IIntervalsQuery, Func<IntervalsMatchDescriptor, IIntervalsMatch>>) ((a, v) => a.Match = v != null ? v(new IntervalsMatchDescriptor()) : (IIntervalsMatch) null));
    }

    public IntervalsQueryDescriptor<T> Prefix(
      Func<IntervalsPrefixDescriptor, IIntervalsPrefix> selector)
    {
      return this.Assign<Func<IntervalsPrefixDescriptor, IIntervalsPrefix>>(selector, (Action<IIntervalsQuery, Func<IntervalsPrefixDescriptor, IIntervalsPrefix>>) ((a, v) => a.Prefix = v != null ? v(new IntervalsPrefixDescriptor()) : (IIntervalsPrefix) null));
    }

    public IntervalsQueryDescriptor<T> Wildcard(
      Func<IntervalsWildcardDescriptor, IIntervalsWildcard> selector)
    {
      return this.Assign<Func<IntervalsWildcardDescriptor, IIntervalsWildcard>>(selector, (Action<IIntervalsQuery, Func<IntervalsWildcardDescriptor, IIntervalsWildcard>>) ((a, v) => a.Wildcard = v != null ? v(new IntervalsWildcardDescriptor()) : (IIntervalsWildcard) null));
    }

    public IntervalsQueryDescriptor<T> AnyOf(
      Func<IntervalsAnyOfDescriptor, IIntervalsAnyOf> selector)
    {
      return this.Assign<Func<IntervalsAnyOfDescriptor, IIntervalsAnyOf>>(selector, (Action<IIntervalsQuery, Func<IntervalsAnyOfDescriptor, IIntervalsAnyOf>>) ((a, v) => a.AnyOf = v != null ? v(new IntervalsAnyOfDescriptor()) : (IIntervalsAnyOf) null));
    }

    public IntervalsQueryDescriptor<T> AllOf(
      Func<IntervalsAllOfDescriptor, IIntervalsAllOf> selector)
    {
      return this.Assign<Func<IntervalsAllOfDescriptor, IIntervalsAllOf>>(selector, (Action<IIntervalsQuery, Func<IntervalsAllOfDescriptor, IIntervalsAllOf>>) ((a, v) => a.AllOf = v != null ? v(new IntervalsAllOfDescriptor()) : (IIntervalsAllOf) null));
    }
  }
}
