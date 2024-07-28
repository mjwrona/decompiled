// Decompiled with JetBrains decompiler
// Type: Nest.IntervalsListDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Collections.Generic;

namespace Nest
{
  public class IntervalsListDescriptor : 
    DescriptorPromiseBase<IntervalsListDescriptor, List<IntervalsContainer>>
  {
    public IntervalsListDescriptor()
      : base(new List<IntervalsContainer>())
    {
    }

    public IntervalsListDescriptor Fuzzy(
      Func<IntervalsFuzzyDescriptor, IIntervalsFuzzy> selector)
    {
      return this.Assign<Func<IntervalsFuzzyDescriptor, IIntervalsFuzzy>>(selector, (Action<List<IntervalsContainer>, Func<IntervalsFuzzyDescriptor, IIntervalsFuzzy>>) ((a, v) => a.AddIfNotNull<IntervalsContainer>((IntervalsContainer) new IntervalsDescriptor().Fuzzy(v))));
    }

    public IntervalsListDescriptor Match(
      Func<IntervalsMatchDescriptor, IIntervalsMatch> selector)
    {
      return this.Assign<Func<IntervalsMatchDescriptor, IIntervalsMatch>>(selector, (Action<List<IntervalsContainer>, Func<IntervalsMatchDescriptor, IIntervalsMatch>>) ((a, v) => a.AddIfNotNull<IntervalsContainer>((IntervalsContainer) new IntervalsDescriptor().Match(v))));
    }

    public IntervalsListDescriptor Prefix(
      Func<IntervalsPrefixDescriptor, IIntervalsPrefix> selector)
    {
      return this.Assign<Func<IntervalsPrefixDescriptor, IIntervalsPrefix>>(selector, (Action<List<IntervalsContainer>, Func<IntervalsPrefixDescriptor, IIntervalsPrefix>>) ((a, v) => a.AddIfNotNull<IntervalsContainer>((IntervalsContainer) new IntervalsDescriptor().Prefix(v))));
    }

    public IntervalsListDescriptor Wildcard(
      Func<IntervalsWildcardDescriptor, IIntervalsWildcard> selector)
    {
      return this.Assign<Func<IntervalsWildcardDescriptor, IIntervalsWildcard>>(selector, (Action<List<IntervalsContainer>, Func<IntervalsWildcardDescriptor, IIntervalsWildcard>>) ((a, v) => a.AddIfNotNull<IntervalsContainer>((IntervalsContainer) new IntervalsDescriptor().Wildcard(v))));
    }

    public IntervalsListDescriptor AnyOf(
      Func<IntervalsAnyOfDescriptor, IIntervalsAnyOf> selector)
    {
      return this.Assign<Func<IntervalsAnyOfDescriptor, IIntervalsAnyOf>>(selector, (Action<List<IntervalsContainer>, Func<IntervalsAnyOfDescriptor, IIntervalsAnyOf>>) ((a, v) => a.AddIfNotNull<IntervalsContainer>((IntervalsContainer) new IntervalsDescriptor().AnyOf(v))));
    }

    public IntervalsListDescriptor AllOf(
      Func<IntervalsAllOfDescriptor, IIntervalsAllOf> selector)
    {
      return this.Assign<Func<IntervalsAllOfDescriptor, IIntervalsAllOf>>(selector, (Action<List<IntervalsContainer>, Func<IntervalsAllOfDescriptor, IIntervalsAllOf>>) ((a, v) => a.AddIfNotNull<IntervalsContainer>((IntervalsContainer) new IntervalsDescriptor().AllOf(v))));
    }
  }
}
