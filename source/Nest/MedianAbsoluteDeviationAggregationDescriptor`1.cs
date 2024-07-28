// Decompiled with JetBrains decompiler
// Type: Nest.MedianAbsoluteDeviationAggregationDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class MedianAbsoluteDeviationAggregationDescriptor<T> : 
    FormattableMetricAggregationDescriptorBase<MedianAbsoluteDeviationAggregationDescriptor<T>, IMedianAbsoluteDeviationAggregation, T>,
    IMedianAbsoluteDeviationAggregation,
    IFormattableMetricAggregation,
    IMetricAggregation,
    IAggregation
    where T : class
  {
    double? IMedianAbsoluteDeviationAggregation.Compression { get; set; }

    public MedianAbsoluteDeviationAggregationDescriptor<T> Compression(double? compression) => this.Assign<double?>(compression, (Action<IMedianAbsoluteDeviationAggregation, double?>) ((a, v) => a.Compression = v));
  }
}
