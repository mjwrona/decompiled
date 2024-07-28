// Decompiled with JetBrains decompiler
// Type: Nest.IntervalsFilterDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class IntervalsFilterDescriptor : 
    DescriptorBase<IntervalsFilterDescriptor, IIntervalsFilter>,
    IIntervalsFilter
  {
    IntervalsContainer IIntervalsFilter.After { get; set; }

    IntervalsContainer IIntervalsFilter.Before { get; set; }

    IntervalsContainer IIntervalsFilter.ContainedBy { get; set; }

    IntervalsContainer IIntervalsFilter.Containing { get; set; }

    IntervalsContainer IIntervalsFilter.NotContainedBy { get; set; }

    IntervalsContainer IIntervalsFilter.NotContaining { get; set; }

    IntervalsContainer IIntervalsFilter.NotOverlapping { get; set; }

    IntervalsContainer IIntervalsFilter.Overlapping { get; set; }

    IScript IIntervalsFilter.Script { get; set; }

    public IntervalsFilterDescriptor Containing(
      Func<IntervalsDescriptor, IntervalsContainer> selector)
    {
      return this.Assign<IntervalsContainer>(selector != null ? selector(new IntervalsDescriptor()) : (IntervalsContainer) null, (Action<IIntervalsFilter, IntervalsContainer>) ((a, v) => a.Containing = v));
    }

    public IntervalsFilterDescriptor ContainedBy(
      Func<IntervalsDescriptor, IntervalsContainer> selector)
    {
      return this.Assign<IntervalsContainer>(selector != null ? selector(new IntervalsDescriptor()) : (IntervalsContainer) null, (Action<IIntervalsFilter, IntervalsContainer>) ((a, v) => a.ContainedBy = v));
    }

    public IntervalsFilterDescriptor NotContaining(
      Func<IntervalsDescriptor, IntervalsContainer> selector)
    {
      return this.Assign<IntervalsContainer>(selector != null ? selector(new IntervalsDescriptor()) : (IntervalsContainer) null, (Action<IIntervalsFilter, IntervalsContainer>) ((a, v) => a.NotContaining = v));
    }

    public IntervalsFilterDescriptor NotContainedBy(
      Func<IntervalsDescriptor, IntervalsContainer> selector)
    {
      return this.Assign<IntervalsContainer>(selector != null ? selector(new IntervalsDescriptor()) : (IntervalsContainer) null, (Action<IIntervalsFilter, IntervalsContainer>) ((a, v) => a.NotContainedBy = v));
    }

    public IntervalsFilterDescriptor Overlapping(
      Func<IntervalsDescriptor, IntervalsContainer> selector)
    {
      return this.Assign<IntervalsContainer>(selector != null ? selector(new IntervalsDescriptor()) : (IntervalsContainer) null, (Action<IIntervalsFilter, IntervalsContainer>) ((a, v) => a.Overlapping = v));
    }

    public IntervalsFilterDescriptor NotOverlapping(
      Func<IntervalsDescriptor, IntervalsContainer> selector)
    {
      return this.Assign<IntervalsContainer>(selector != null ? selector(new IntervalsDescriptor()) : (IntervalsContainer) null, (Action<IIntervalsFilter, IntervalsContainer>) ((a, v) => a.NotOverlapping = v));
    }

    public IntervalsFilterDescriptor Before(
      Func<IntervalsDescriptor, IntervalsContainer> selector)
    {
      return this.Assign<IntervalsContainer>(selector != null ? selector(new IntervalsDescriptor()) : (IntervalsContainer) null, (Action<IIntervalsFilter, IntervalsContainer>) ((a, v) => a.Before = v));
    }

    public IntervalsFilterDescriptor After(
      Func<IntervalsDescriptor, IntervalsContainer> selector)
    {
      return this.Assign<IntervalsContainer>(selector != null ? selector(new IntervalsDescriptor()) : (IntervalsContainer) null, (Action<IIntervalsFilter, IntervalsContainer>) ((a, v) => a.After = v));
    }

    public IntervalsFilterDescriptor Script(Func<ScriptDescriptor, IScript> selector) => this.Assign<IScript>(selector != null ? selector(new ScriptDescriptor()) : (IScript) null, (Action<IIntervalsFilter, IScript>) ((a, v) => a.Script = v));
  }
}
