// Decompiled with JetBrains decompiler
// Type: Nest.BucketSortAggregationDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Collections.Generic;

namespace Nest
{
  public class BucketSortAggregationDescriptor<T> : 
    DescriptorBase<BucketSortAggregationDescriptor<T>, IBucketSortAggregation>,
    IBucketSortAggregation,
    IAggregation
    where T : class
  {
    int? IBucketSortAggregation.From { get; set; }

    Nest.GapPolicy? IBucketSortAggregation.GapPolicy { get; set; }

    IDictionary<string, object> IAggregation.Meta { get; set; }

    string IAggregation.Name { get; set; }

    int? IBucketSortAggregation.Size { get; set; }

    IList<ISort> IBucketSortAggregation.Sort { get; set; }

    public BucketSortAggregationDescriptor<T> Sort(
      Func<SortDescriptor<T>, IPromise<IList<ISort>>> selector)
    {
      return this.Assign<Func<SortDescriptor<T>, IPromise<IList<ISort>>>>(selector, (Action<IBucketSortAggregation, Func<SortDescriptor<T>, IPromise<IList<ISort>>>>) ((a, v) => a.Sort = v != null ? v(new SortDescriptor<T>())?.Value : (IList<ISort>) null));
    }

    public BucketSortAggregationDescriptor<T> From(int? from) => this.Assign<int?>(from, (Action<IBucketSortAggregation, int?>) ((a, v) => a.From = v));

    public BucketSortAggregationDescriptor<T> Size(int? size) => this.Assign<int?>(size, (Action<IBucketSortAggregation, int?>) ((a, v) => a.Size = v));

    public BucketSortAggregationDescriptor<T> GapPolicy(Nest.GapPolicy? gapPolicy) => this.Assign<Nest.GapPolicy?>(gapPolicy, (Action<IBucketSortAggregation, Nest.GapPolicy?>) ((a, v) => a.GapPolicy = v));

    public BucketSortAggregationDescriptor<T> Meta(
      Func<FluentDictionary<string, object>, FluentDictionary<string, object>> selector)
    {
      return this.Assign<Func<FluentDictionary<string, object>, FluentDictionary<string, object>>>(selector, (Action<IBucketSortAggregation, Func<FluentDictionary<string, object>, FluentDictionary<string, object>>>) ((a, v) => a.Meta = v != null ? (IDictionary<string, object>) v(new FluentDictionary<string, object>()) : (IDictionary<string, object>) null));
    }
  }
}
