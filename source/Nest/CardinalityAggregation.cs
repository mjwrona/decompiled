// Decompiled with JetBrains decompiler
// Type: Nest.CardinalityAggregation
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

namespace Nest
{
  public class CardinalityAggregation : 
    MetricAggregationBase,
    ICardinalityAggregation,
    IMetricAggregation,
    IAggregation
  {
    internal CardinalityAggregation()
    {
    }

    public CardinalityAggregation(string name, Field field)
      : base(name, field)
    {
    }

    public int? PrecisionThreshold { get; set; }

    public bool? Rehash { get; set; }

    internal override void WrapInContainer(AggregationContainer c) => c.Cardinality = (ICardinalityAggregation) this;
  }
}
