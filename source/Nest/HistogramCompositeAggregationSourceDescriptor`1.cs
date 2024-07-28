// Decompiled with JetBrains decompiler
// Type: Nest.HistogramCompositeAggregationSourceDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class HistogramCompositeAggregationSourceDescriptor<T> : 
    CompositeAggregationSourceDescriptorBase<HistogramCompositeAggregationSourceDescriptor<T>, IHistogramCompositeAggregationSource, T>,
    IHistogramCompositeAggregationSource,
    ICompositeAggregationSource
  {
    public HistogramCompositeAggregationSourceDescriptor(string name)
      : base(name, "histogram")
    {
    }

    double? IHistogramCompositeAggregationSource.Interval { get; set; }

    IScript IHistogramCompositeAggregationSource.Script { get; set; }

    public HistogramCompositeAggregationSourceDescriptor<T> Interval(double? interval) => this.Assign<double?>(interval, (Action<IHistogramCompositeAggregationSource, double?>) ((a, v) => a.Interval = v));

    public HistogramCompositeAggregationSourceDescriptor<T> Script(
      Func<ScriptDescriptor, IScript> selector)
    {
      return this.Assign<Func<ScriptDescriptor, IScript>>(selector, (Action<IHistogramCompositeAggregationSource, Func<ScriptDescriptor, IScript>>) ((a, v) => a.Script = v != null ? v(new ScriptDescriptor()) : (IScript) null));
    }
  }
}
