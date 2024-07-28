// Decompiled with JetBrains decompiler
// Type: Nest.PercentilesAggregation
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Collections.Generic;

namespace Nest
{
  public class PercentilesAggregation : 
    FormattableMetricAggregationBase,
    IPercentilesAggregation,
    IFormattableMetricAggregation,
    IMetricAggregation,
    IAggregation
  {
    internal PercentilesAggregation()
    {
    }

    public PercentilesAggregation(string name, Field field)
      : base(name, field)
    {
    }

    public IPercentilesMethod Method { get; set; }

    public IEnumerable<double> Percents { get; set; }

    public bool? Keyed { get; set; }

    internal override void WrapInContainer(AggregationContainer c) => c.Percentiles = (IPercentilesAggregation) this;
  }
}
