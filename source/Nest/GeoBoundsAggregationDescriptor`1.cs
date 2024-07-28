// Decompiled with JetBrains decompiler
// Type: Nest.GeoBoundsAggregationDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class GeoBoundsAggregationDescriptor<T> : 
    MetricAggregationDescriptorBase<GeoBoundsAggregationDescriptor<T>, IGeoBoundsAggregation, T>,
    IGeoBoundsAggregation,
    IMetricAggregation,
    IAggregation
    where T : class
  {
    bool? IGeoBoundsAggregation.WrapLongitude { get; set; }

    public GeoBoundsAggregationDescriptor<T> WrapLongitude(bool? wrapLongitude = true) => this.Assign<bool?>(wrapLongitude, (Action<IGeoBoundsAggregation, bool?>) ((a, v) => a.WrapLongitude = v));
  }
}
