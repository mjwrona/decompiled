// Decompiled with JetBrains decompiler
// Type: Nest.ExtendedStatsAggregationDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class ExtendedStatsAggregationDescriptor<T> : 
    FormattableMetricAggregationDescriptorBase<ExtendedStatsAggregationDescriptor<T>, IExtendedStatsAggregation, T>,
    IExtendedStatsAggregation,
    IFormattableMetricAggregation,
    IMetricAggregation,
    IAggregation
    where T : class
  {
    double? IExtendedStatsAggregation.Sigma { get; set; }

    public ExtendedStatsAggregationDescriptor<T> Sigma(double? sigma) => this.Assign<double?>(sigma, (Action<IExtendedStatsAggregation, double?>) ((a, v) => a.Sigma = v));
  }
}
