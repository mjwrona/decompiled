// Decompiled with JetBrains decompiler
// Type: Nest.WeightedAverageAggregation
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

namespace Nest
{
  public class WeightedAverageAggregation : 
    AggregationBase,
    IWeightedAverageAggregation,
    IAggregation
  {
    internal WeightedAverageAggregation()
    {
    }

    public WeightedAverageAggregation(string name)
      : base(name)
    {
    }

    public string Format { get; set; }

    public IWeightedAverageValue Value { get; set; }

    public Nest.ValueType? ValueType { get; set; }

    public IWeightedAverageValue Weight { get; set; }

    internal override void WrapInContainer(AggregationContainer c) => c.WeightedAverage = (IWeightedAverageAggregation) this;
  }
}
