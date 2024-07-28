// Decompiled with JetBrains decompiler
// Type: Nest.IntervalsDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class IntervalsDescriptor : IntervalsContainer
  {
    private IntervalsDescriptor Assign<TValue>(
      TValue value,
      Action<IIntervalsContainer, TValue> assigner)
    {
      return Fluent.Assign<IntervalsDescriptor, IIntervalsContainer, TValue>(this, value, assigner);
    }

    public IntervalsDescriptor Fuzzy(
      Func<IntervalsFuzzyDescriptor, IIntervalsFuzzy> selector)
    {
      return this.Assign<Func<IntervalsFuzzyDescriptor, IIntervalsFuzzy>>(selector, (Action<IIntervalsContainer, Func<IntervalsFuzzyDescriptor, IIntervalsFuzzy>>) ((a, v) => a.Fuzzy = v != null ? v(new IntervalsFuzzyDescriptor()) : (IIntervalsFuzzy) null));
    }

    public IntervalsDescriptor Match(
      Func<IntervalsMatchDescriptor, IIntervalsMatch> selector)
    {
      return this.Assign<Func<IntervalsMatchDescriptor, IIntervalsMatch>>(selector, (Action<IIntervalsContainer, Func<IntervalsMatchDescriptor, IIntervalsMatch>>) ((a, v) => a.Match = v != null ? v(new IntervalsMatchDescriptor()) : (IIntervalsMatch) null));
    }

    public IntervalsDescriptor Prefix(
      Func<IntervalsPrefixDescriptor, IIntervalsPrefix> selector)
    {
      return this.Assign<Func<IntervalsPrefixDescriptor, IIntervalsPrefix>>(selector, (Action<IIntervalsContainer, Func<IntervalsPrefixDescriptor, IIntervalsPrefix>>) ((a, v) => a.Prefix = v != null ? v(new IntervalsPrefixDescriptor()) : (IIntervalsPrefix) null));
    }

    public IntervalsDescriptor Wildcard(
      Func<IntervalsWildcardDescriptor, IIntervalsWildcard> selector)
    {
      return this.Assign<Func<IntervalsWildcardDescriptor, IIntervalsWildcard>>(selector, (Action<IIntervalsContainer, Func<IntervalsWildcardDescriptor, IIntervalsWildcard>>) ((a, v) => a.Wildcard = v != null ? v(new IntervalsWildcardDescriptor()) : (IIntervalsWildcard) null));
    }

    public IntervalsDescriptor AnyOf(
      Func<IntervalsAnyOfDescriptor, IIntervalsAnyOf> selector)
    {
      return this.Assign<Func<IntervalsAnyOfDescriptor, IIntervalsAnyOf>>(selector, (Action<IIntervalsContainer, Func<IntervalsAnyOfDescriptor, IIntervalsAnyOf>>) ((a, v) => a.AnyOf = v != null ? v(new IntervalsAnyOfDescriptor()) : (IIntervalsAnyOf) null));
    }

    public IntervalsDescriptor AllOf(
      Func<IntervalsAllOfDescriptor, IIntervalsAllOf> selector)
    {
      return this.Assign<Func<IntervalsAllOfDescriptor, IIntervalsAllOf>>(selector, (Action<IIntervalsContainer, Func<IntervalsAllOfDescriptor, IIntervalsAllOf>>) ((a, v) => a.AllOf = v != null ? v(new IntervalsAllOfDescriptor()) : (IIntervalsAllOf) null));
    }
  }
}
