// Decompiled with JetBrains decompiler
// Type: Nest.FormattableMetricAggregationDescriptorBase`3
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public abstract class FormattableMetricAggregationDescriptorBase<TFormattableMetricAggregation, TFormattableMetricAggregationInterface, T> : 
    MetricAggregationDescriptorBase<TFormattableMetricAggregation, TFormattableMetricAggregationInterface, T>,
    IFormattableMetricAggregation,
    IMetricAggregation,
    IAggregation
    where TFormattableMetricAggregation : FormattableMetricAggregationDescriptorBase<TFormattableMetricAggregation, TFormattableMetricAggregationInterface, T>, TFormattableMetricAggregationInterface, IFormattableMetricAggregation
    where TFormattableMetricAggregationInterface : class, IFormattableMetricAggregation
    where T : class
  {
    string IFormattableMetricAggregation.Format { get; set; }

    public TFormattableMetricAggregation Format(string format) => this.Assign<string>(format, (Action<TFormattableMetricAggregationInterface, string>) ((a, v) => a.Format = v));
  }
}
