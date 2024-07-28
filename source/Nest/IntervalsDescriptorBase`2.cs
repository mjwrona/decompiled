// Decompiled with JetBrains decompiler
// Type: Nest.IntervalsDescriptorBase`2
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public abstract class IntervalsDescriptorBase<TDescriptor, TInterface> : 
    DescriptorBase<TDescriptor, TInterface>,
    IIntervals
    where TDescriptor : DescriptorBase<TDescriptor, TInterface>, TInterface
    where TInterface : class, IIntervals
  {
    IIntervalsFilter IIntervals.Filter { get; set; }

    public TDescriptor Filter(
      Func<IntervalsFilterDescriptor, IIntervalsFilter> selector)
    {
      return this.Assign<Func<IntervalsFilterDescriptor, IIntervalsFilter>>(selector, (Action<TInterface, Func<IntervalsFilterDescriptor, IIntervalsFilter>>) ((a, v) => a.Filter = v != null ? v(new IntervalsFilterDescriptor()) : (IIntervalsFilter) null));
    }
  }
}
