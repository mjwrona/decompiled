// Decompiled with JetBrains decompiler
// Type: Nest.FiltersAggregationDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace Nest
{
  public class FiltersAggregationDescriptor<T> : 
    BucketAggregationDescriptorBase<FiltersAggregationDescriptor<T>, IFiltersAggregation, T>,
    IFiltersAggregation,
    IBucketAggregation,
    IAggregation
    where T : class
  {
    Union<INamedFiltersContainer, IEnumerable<QueryContainer>> IFiltersAggregation.Filters { get; set; }

    bool? IFiltersAggregation.OtherBucket { get; set; }

    string IFiltersAggregation.OtherBucketKey { get; set; }

    public FiltersAggregationDescriptor<T> OtherBucket(bool? otherBucket = true) => this.Assign<bool?>(otherBucket, (Action<IFiltersAggregation, bool?>) ((a, v) => a.OtherBucket = v));

    public FiltersAggregationDescriptor<T> OtherBucketKey(string otherBucketKey) => this.Assign<string>(otherBucketKey, (Action<IFiltersAggregation, string>) ((a, v) => a.OtherBucketKey = v));

    public FiltersAggregationDescriptor<T> NamedFilters(
      Func<NamedFiltersContainerDescriptor<T>, IPromise<INamedFiltersContainer>> selector)
    {
      return this.Assign<Func<NamedFiltersContainerDescriptor<T>, IPromise<INamedFiltersContainer>>>(selector, (Action<IFiltersAggregation, Func<NamedFiltersContainerDescriptor<T>, IPromise<INamedFiltersContainer>>>) ((a, v) => a.Filters = new Union<INamedFiltersContainer, IEnumerable<QueryContainer>>(v != null ? v(new NamedFiltersContainerDescriptor<T>())?.Value : (INamedFiltersContainer) null)));
    }

    public FiltersAggregationDescriptor<T> AnonymousFilters(
      params Func<QueryContainerDescriptor<T>, QueryContainer>[] selectors)
    {
      return this.Assign<Func<QueryContainerDescriptor<T>, QueryContainer>[]>(selectors, (Action<IFiltersAggregation, Func<QueryContainerDescriptor<T>, QueryContainer>[]>) ((a, v) => a.Filters = (Union<INamedFiltersContainer, IEnumerable<QueryContainer>>) (IEnumerable<QueryContainer>) ((IEnumerable<Func<QueryContainerDescriptor<T>, QueryContainer>>) v).Select<Func<QueryContainerDescriptor<T>, QueryContainer>, QueryContainer>((Func<Func<QueryContainerDescriptor<T>, QueryContainer>, QueryContainer>) (vv => vv == null ? (QueryContainer) null : vv(new QueryContainerDescriptor<T>()))).ToList<QueryContainer>()));
    }

    public FiltersAggregationDescriptor<T> AnonymousFilters(
      IEnumerable<Func<QueryContainerDescriptor<T>, QueryContainer>> selectors)
    {
      return this.Assign<IEnumerable<Func<QueryContainerDescriptor<T>, QueryContainer>>>(selectors, (Action<IFiltersAggregation, IEnumerable<Func<QueryContainerDescriptor<T>, QueryContainer>>>) ((a, v) => a.Filters = (Union<INamedFiltersContainer, IEnumerable<QueryContainer>>) (IEnumerable<QueryContainer>) v.Select<Func<QueryContainerDescriptor<T>, QueryContainer>, QueryContainer>((Func<Func<QueryContainerDescriptor<T>, QueryContainer>, QueryContainer>) (vv => vv == null ? (QueryContainer) null : vv(new QueryContainerDescriptor<T>()))).ToList<QueryContainer>()));
    }
  }
}
