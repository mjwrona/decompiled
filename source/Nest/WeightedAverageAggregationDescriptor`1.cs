// Decompiled with JetBrains decompiler
// Type: Nest.WeightedAverageAggregationDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Collections.Generic;

namespace Nest
{
  public class WeightedAverageAggregationDescriptor<T> : 
    DescriptorBase<WeightedAverageAggregationDescriptor<T>, IWeightedAverageAggregation>,
    IWeightedAverageAggregation,
    IAggregation
    where T : class
  {
    string IWeightedAverageAggregation.Format { get; set; }

    IDictionary<string, object> IAggregation.Meta { get; set; }

    string IAggregation.Name { get; set; }

    IWeightedAverageValue IWeightedAverageAggregation.Value { get; set; }

    Nest.ValueType? IWeightedAverageAggregation.ValueType { get; set; }

    IWeightedAverageValue IWeightedAverageAggregation.Weight { get; set; }

    public WeightedAverageAggregationDescriptor<T> Meta(
      Func<FluentDictionary<string, object>, FluentDictionary<string, object>> selector)
    {
      return this.Assign<Func<FluentDictionary<string, object>, FluentDictionary<string, object>>>(selector, (Action<IWeightedAverageAggregation, Func<FluentDictionary<string, object>, FluentDictionary<string, object>>>) ((a, v) => a.Meta = v != null ? (IDictionary<string, object>) v(new FluentDictionary<string, object>()) : (IDictionary<string, object>) null));
    }

    public WeightedAverageAggregationDescriptor<T> Value(
      Func<WeightedAverageValueDescriptor<T>, IWeightedAverageValue> selector)
    {
      return this.Assign<Func<WeightedAverageValueDescriptor<T>, IWeightedAverageValue>>(selector, (Action<IWeightedAverageAggregation, Func<WeightedAverageValueDescriptor<T>, IWeightedAverageValue>>) ((a, v) => a.Value = v != null ? v(new WeightedAverageValueDescriptor<T>()) : (IWeightedAverageValue) null));
    }

    public WeightedAverageAggregationDescriptor<T> Weight(
      Func<WeightedAverageValueDescriptor<T>, IWeightedAverageValue> selector)
    {
      return this.Assign<Func<WeightedAverageValueDescriptor<T>, IWeightedAverageValue>>(selector, (Action<IWeightedAverageAggregation, Func<WeightedAverageValueDescriptor<T>, IWeightedAverageValue>>) ((a, v) => a.Weight = v != null ? v(new WeightedAverageValueDescriptor<T>()) : (IWeightedAverageValue) null));
    }

    public WeightedAverageAggregationDescriptor<T> Format(string format) => this.Assign<string>(format, (Action<IWeightedAverageAggregation, string>) ((a, v) => a.Format = v));

    public WeightedAverageAggregationDescriptor<T> ValueType(Nest.ValueType? valueType) => this.Assign<Nest.ValueType?>(valueType, (Action<IWeightedAverageAggregation, Nest.ValueType?>) ((a, v) => a.ValueType = v));
  }
}
