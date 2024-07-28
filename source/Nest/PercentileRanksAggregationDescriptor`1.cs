// Decompiled with JetBrains decompiler
// Type: Nest.PercentileRanksAggregationDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Collections.Generic;

namespace Nest
{
  public class PercentileRanksAggregationDescriptor<T> : 
    FormattableMetricAggregationDescriptorBase<PercentileRanksAggregationDescriptor<T>, IPercentileRanksAggregation, T>,
    IPercentileRanksAggregation,
    IFormattableMetricAggregation,
    IMetricAggregation,
    IAggregation
    where T : class
  {
    IPercentilesMethod IPercentileRanksAggregation.Method { get; set; }

    IEnumerable<double> IPercentileRanksAggregation.Values { get; set; }

    bool? IPercentileRanksAggregation.Keyed { get; set; }

    public PercentileRanksAggregationDescriptor<T> Values(IEnumerable<double> values) => this.Assign<IEnumerable<double>>(values, (Action<IPercentileRanksAggregation, IEnumerable<double>>) ((a, v) => a.Values = v));

    public PercentileRanksAggregationDescriptor<T> Values(params double[] values) => this.Assign<double[]>(values, (Action<IPercentileRanksAggregation, double[]>) ((a, v) => a.Values = (IEnumerable<double>) v));

    public PercentileRanksAggregationDescriptor<T> Method(
      Func<PercentilesMethodDescriptor, IPercentilesMethod> methodSelctor)
    {
      return this.Assign<Func<PercentilesMethodDescriptor, IPercentilesMethod>>(methodSelctor, (Action<IPercentileRanksAggregation, Func<PercentilesMethodDescriptor, IPercentilesMethod>>) ((a, v) => a.Method = v != null ? v(new PercentilesMethodDescriptor()) : (IPercentilesMethod) null));
    }

    public PercentileRanksAggregationDescriptor<T> Keyed(bool? keyed = true) => this.Assign<bool?>(keyed, (Action<IPercentileRanksAggregation, bool?>) ((a, v) => a.Keyed = v));
  }
}
