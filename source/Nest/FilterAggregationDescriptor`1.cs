// Decompiled with JetBrains decompiler
// Type: Nest.FilterAggregationDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class FilterAggregationDescriptor<T> : 
    BucketAggregationDescriptorBase<FilterAggregationDescriptor<T>, IFilterAggregation, T>,
    IFilterAggregation,
    IBucketAggregation,
    IAggregation
    where T : class
  {
    QueryContainer IFilterAggregation.Filter { get; set; }

    public FilterAggregationDescriptor<T> Filter(
      Func<QueryContainerDescriptor<T>, QueryContainer> selector)
    {
      return this.Assign<Func<QueryContainerDescriptor<T>, QueryContainer>>(selector, (Action<IFilterAggregation, Func<QueryContainerDescriptor<T>, QueryContainer>>) ((a, v) => a.Filter = v != null ? v(new QueryContainerDescriptor<T>()) : (QueryContainer) null));
    }
  }
}
