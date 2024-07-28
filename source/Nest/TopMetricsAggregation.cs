// Decompiled with JetBrains decompiler
// Type: Nest.TopMetricsAggregation
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Collections.Generic;

namespace Nest
{
  public class TopMetricsAggregation : 
    MetricAggregationBase,
    ITopMetricsAggregation,
    IMetricAggregation,
    IAggregation
  {
    internal TopMetricsAggregation()
    {
    }

    public TopMetricsAggregation(string name)
      : base(name, (Field) null)
    {
    }

    public IList<ITopMetricsValue> Metrics { get; set; }

    public int? Size { get; set; }

    public IList<ISort> Sort { get; set; }

    internal override void WrapInContainer(AggregationContainer c) => c.TopMetrics = (ITopMetricsAggregation) this;
  }
}
