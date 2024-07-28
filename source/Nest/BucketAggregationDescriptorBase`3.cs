// Decompiled with JetBrains decompiler
// Type: Nest.BucketAggregationDescriptorBase`3
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Collections.Generic;

namespace Nest
{
  public abstract class BucketAggregationDescriptorBase<TBucketAggregation, TBucketAggregationInterface, T> : 
    IBucketAggregation,
    IAggregation,
    IDescriptor
    where TBucketAggregation : BucketAggregationDescriptorBase<TBucketAggregation, TBucketAggregationInterface, T>, TBucketAggregationInterface, IBucketAggregation
    where TBucketAggregationInterface : class, IBucketAggregation
    where T : class
  {
    protected TBucketAggregationInterface Self => (TBucketAggregationInterface) this;

    AggregationDictionary IBucketAggregation.Aggregations { get; set; }

    IDictionary<string, object> IAggregation.Meta { get; set; }

    string IAggregation.Name { get; set; }

    protected TBucketAggregation Assign<TValue>(
      TValue value,
      Action<TBucketAggregationInterface, TValue> assigner)
    {
      return Fluent.Assign<TBucketAggregation, TBucketAggregationInterface, TValue>((TBucketAggregation) this, value, assigner);
    }

    public TBucketAggregation Aggregations(
      Func<AggregationContainerDescriptor<T>, IAggregationContainer> selector)
    {
      return this.Assign<Func<AggregationContainerDescriptor<T>, IAggregationContainer>>(selector, (Action<TBucketAggregationInterface, Func<AggregationContainerDescriptor<T>, IAggregationContainer>>) ((a, v) => a.Aggregations = v != null ? v(new AggregationContainerDescriptor<T>())?.Aggregations : (AggregationDictionary) null));
    }

    public TBucketAggregation Aggregations(AggregationDictionary aggregations) => this.Assign<AggregationDictionary>(aggregations, (Action<TBucketAggregationInterface, AggregationDictionary>) ((a, v) => a.Aggregations = v));

    public TBucketAggregation Meta(
      Func<FluentDictionary<string, object>, FluentDictionary<string, object>> selector)
    {
      return this.Assign<Func<FluentDictionary<string, object>, FluentDictionary<string, object>>>(selector, (Action<TBucketAggregationInterface, Func<FluentDictionary<string, object>, FluentDictionary<string, object>>>) ((a, v) => a.Meta = v != null ? (IDictionary<string, object>) v(new FluentDictionary<string, object>()) : (IDictionary<string, object>) null));
    }
  }
}
