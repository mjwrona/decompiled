// Decompiled with JetBrains decompiler
// Type: Nest.BoxplotAggregationDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class BoxplotAggregationDescriptor<T> : 
    MetricAggregationDescriptorBase<BoxplotAggregationDescriptor<T>, IBoxplotAggregation, T>,
    IBoxplotAggregation,
    IMetricAggregation,
    IAggregation
    where T : class
  {
    double? IBoxplotAggregation.Compression { get; set; }

    public BoxplotAggregationDescriptor<T> Compression(double? compression) => this.Assign<double?>(compression, (Action<IBoxplotAggregation, double?>) ((a, v) => a.Compression = v));
  }
}
