// Decompiled with JetBrains decompiler
// Type: Nest.FormattableMetricAggregationBase
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

namespace Nest
{
  public abstract class FormattableMetricAggregationBase : 
    MetricAggregationBase,
    IFormattableMetricAggregation,
    IMetricAggregation,
    IAggregation
  {
    internal FormattableMetricAggregationBase()
    {
    }

    protected FormattableMetricAggregationBase(string name, Field field)
      : base(name, field)
    {
    }

    public string Format { get; set; }
  }
}
