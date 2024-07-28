// Decompiled with JetBrains decompiler
// Type: Nest.TTestAggregationDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Collections.Generic;

namespace Nest
{
  public class TTestAggregationDescriptor<T> : 
    DescriptorBase<TTestAggregationDescriptor<T>, ITTestAggregation>,
    ITTestAggregation,
    IAggregation
    where T : class
  {
    IDictionary<string, object> IAggregation.Meta { get; set; }

    string IAggregation.Name { get; set; }

    ITTestPopulation ITTestAggregation.A { get; set; }

    ITTestPopulation ITTestAggregation.B { get; set; }

    TTestType? ITTestAggregation.Type { get; set; }

    public TTestAggregationDescriptor<T> A(
      Func<TTestPopulationDescriptor<T>, ITTestPopulation> selector)
    {
      return this.Assign<Func<TTestPopulationDescriptor<T>, ITTestPopulation>>(selector, (Action<ITTestAggregation, Func<TTestPopulationDescriptor<T>, ITTestPopulation>>) ((a, v) => a.A = v != null ? v(new TTestPopulationDescriptor<T>()) : (ITTestPopulation) null));
    }

    public TTestAggregationDescriptor<T> A<TOther>(
      Func<TTestPopulationDescriptor<TOther>, ITTestPopulation> selector)
      where TOther : class
    {
      return this.Assign<Func<TTestPopulationDescriptor<TOther>, ITTestPopulation>>(selector, (Action<ITTestAggregation, Func<TTestPopulationDescriptor<TOther>, ITTestPopulation>>) ((a, v) => a.A = v != null ? v(new TTestPopulationDescriptor<TOther>()) : (ITTestPopulation) null));
    }

    public TTestAggregationDescriptor<T> B(
      Func<TTestPopulationDescriptor<T>, ITTestPopulation> selector)
    {
      return this.Assign<Func<TTestPopulationDescriptor<T>, ITTestPopulation>>(selector, (Action<ITTestAggregation, Func<TTestPopulationDescriptor<T>, ITTestPopulation>>) ((a, v) => a.B = v != null ? v(new TTestPopulationDescriptor<T>()) : (ITTestPopulation) null));
    }

    public TTestAggregationDescriptor<T> B<TOther>(
      Func<TTestPopulationDescriptor<TOther>, ITTestPopulation> selector)
      where TOther : class
    {
      return this.Assign<Func<TTestPopulationDescriptor<TOther>, ITTestPopulation>>(selector, (Action<ITTestAggregation, Func<TTestPopulationDescriptor<TOther>, ITTestPopulation>>) ((a, v) => a.B = v != null ? v(new TTestPopulationDescriptor<TOther>()) : (ITTestPopulation) null));
    }

    public TTestAggregationDescriptor<T> Type(TTestType? type) => this.Assign<TTestType?>(type, (Action<ITTestAggregation, TTestType?>) ((a, v) => a.Type = v));

    public TTestAggregationDescriptor<T> Meta(
      Func<FluentDictionary<string, object>, FluentDictionary<string, object>> selector)
    {
      return this.Assign<Func<FluentDictionary<string, object>, FluentDictionary<string, object>>>(selector, (Action<ITTestAggregation, Func<FluentDictionary<string, object>, FluentDictionary<string, object>>>) ((a, v) => a.Meta = v != null ? (IDictionary<string, object>) v(new FluentDictionary<string, object>()) : (IDictionary<string, object>) null));
    }
  }
}
