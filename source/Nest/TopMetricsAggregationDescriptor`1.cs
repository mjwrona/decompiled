// Decompiled with JetBrains decompiler
// Type: Nest.TopMetricsAggregationDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Collections.Generic;

namespace Nest
{
  public class TopMetricsAggregationDescriptor<T> : 
    MetricAggregationDescriptorBase<TopMetricsAggregationDescriptor<T>, ITopMetricsAggregation, T>,
    ITopMetricsAggregation,
    IMetricAggregation,
    IAggregation
    where T : class
  {
    int? ITopMetricsAggregation.Size { get; set; }

    IList<ISort> ITopMetricsAggregation.Sort { get; set; }

    IList<ITopMetricsValue> ITopMetricsAggregation.Metrics { get; set; }

    public TopMetricsAggregationDescriptor<T> Size(int? size) => this.Assign<int?>(size, (Action<ITopMetricsAggregation, int?>) ((a, v) => a.Size = v));

    public TopMetricsAggregationDescriptor<T> Sort(
      Func<SortDescriptor<T>, IPromise<IList<ISort>>> sortSelector)
    {
      return this.Assign<Func<SortDescriptor<T>, IPromise<IList<ISort>>>>(sortSelector, (Action<ITopMetricsAggregation, Func<SortDescriptor<T>, IPromise<IList<ISort>>>>) ((a, v) => a.Sort = v != null ? v(new SortDescriptor<T>())?.Value : (IList<ISort>) null));
    }

    public TopMetricsAggregationDescriptor<T> Metrics(
      Func<TopMetricsValuesDescriptor<T>, IPromise<IList<ITopMetricsValue>>> TopMetricsValueSelector)
    {
      return this.Assign<Func<TopMetricsValuesDescriptor<T>, IPromise<IList<ITopMetricsValue>>>>(TopMetricsValueSelector, (Action<ITopMetricsAggregation, Func<TopMetricsValuesDescriptor<T>, IPromise<IList<ITopMetricsValue>>>>) ((a, v) => a.Metrics = v != null ? v(new TopMetricsValuesDescriptor<T>())?.Value : (IList<ITopMetricsValue>) null));
    }
  }
}
