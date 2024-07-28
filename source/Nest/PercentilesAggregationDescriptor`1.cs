// Decompiled with JetBrains decompiler
// Type: Nest.PercentilesAggregationDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Collections.Generic;

namespace Nest
{
  public class PercentilesAggregationDescriptor<T> : 
    FormattableMetricAggregationDescriptorBase<PercentilesAggregationDescriptor<T>, IPercentilesAggregation, T>,
    IPercentilesAggregation,
    IFormattableMetricAggregation,
    IMetricAggregation,
    IAggregation
    where T : class
  {
    IPercentilesMethod IPercentilesAggregation.Method { get; set; }

    IEnumerable<double> IPercentilesAggregation.Percents { get; set; }

    bool? IPercentilesAggregation.Keyed { get; set; }

    public PercentilesAggregationDescriptor<T> Percents(IEnumerable<double> percentages) => this.Assign<IEnumerable<double>>(percentages, (Action<IPercentilesAggregation, IEnumerable<double>>) ((a, v) => a.Percents = v));

    public PercentilesAggregationDescriptor<T> Percents(params double[] percentages) => this.Assign<double[]>(percentages, (Action<IPercentilesAggregation, double[]>) ((a, v) => a.Percents = (IEnumerable<double>) v));

    public PercentilesAggregationDescriptor<T> Method(
      Func<PercentilesMethodDescriptor, IPercentilesMethod> methodSelector)
    {
      return this.Assign<Func<PercentilesMethodDescriptor, IPercentilesMethod>>(methodSelector, (Action<IPercentilesAggregation, Func<PercentilesMethodDescriptor, IPercentilesMethod>>) ((a, v) => a.Method = v != null ? v(new PercentilesMethodDescriptor()) : (IPercentilesMethod) null));
    }

    public PercentilesAggregationDescriptor<T> Keyed(bool? keyed = true) => this.Assign<bool?>(keyed, (Action<IPercentilesAggregation, bool?>) ((a, v) => a.Keyed = v));
  }
}
