// Decompiled with JetBrains decompiler
// Type: Nest.AdjacencyMatrixAggregationDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class AdjacencyMatrixAggregationDescriptor<T> : 
    BucketAggregationDescriptorBase<AdjacencyMatrixAggregationDescriptor<T>, IAdjacencyMatrixAggregation, T>,
    IAdjacencyMatrixAggregation,
    IBucketAggregation,
    IAggregation
    where T : class
  {
    INamedFiltersContainer IAdjacencyMatrixAggregation.Filters { get; set; }

    public AdjacencyMatrixAggregationDescriptor<T> Filters(
      Func<NamedFiltersContainerDescriptor<T>, IPromise<INamedFiltersContainer>> selector)
    {
      return this.Assign<Func<NamedFiltersContainerDescriptor<T>, IPromise<INamedFiltersContainer>>>(selector, (Action<IAdjacencyMatrixAggregation, Func<NamedFiltersContainerDescriptor<T>, IPromise<INamedFiltersContainer>>>) ((a, v) => a.Filters = v != null ? v(new NamedFiltersContainerDescriptor<T>())?.Value : (INamedFiltersContainer) null));
    }
  }
}
