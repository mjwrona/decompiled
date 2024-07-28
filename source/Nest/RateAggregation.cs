// Decompiled with JetBrains decompiler
// Type: Nest.RateAggregation
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

namespace Nest
{
  public class RateAggregation : 
    MetricAggregationBase,
    IRateAggregation,
    IMetricAggregation,
    IAggregation
  {
    public RateAggregation(string name)
      : base(name, (Field) null)
    {
    }

    public RateAggregation(string name, Field field)
      : base(name, field)
    {
    }

    internal override void WrapInContainer(AggregationContainer c) => c.Rate = (IRateAggregation) this;

    public DateInterval? Unit { get; set; }

    public RateMode? Mode { get; set; }
  }
}
