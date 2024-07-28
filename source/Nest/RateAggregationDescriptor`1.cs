// Decompiled with JetBrains decompiler
// Type: Nest.RateAggregationDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class RateAggregationDescriptor<T> : 
    MetricAggregationDescriptorBase<RateAggregationDescriptor<T>, IRateAggregation, T>,
    IRateAggregation,
    IMetricAggregation,
    IAggregation
    where T : class
  {
    DateInterval? IRateAggregation.Unit { get; set; }

    RateMode? IRateAggregation.Mode { get; set; }

    public RateAggregationDescriptor<T> Unit(DateInterval? dateInterval) => this.Assign<DateInterval?>(dateInterval, (Action<IRateAggregation, DateInterval?>) ((a, v) => a.Unit = v));

    public RateAggregationDescriptor<T> Mode(RateMode? mode) => this.Assign<RateMode?>(mode, (Action<IRateAggregation, RateMode?>) ((a, v) => a.Mode = v));
  }
}
