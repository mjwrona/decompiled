// Decompiled with JetBrains decompiler
// Type: Nest.MovingAverageAggregation
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

namespace Nest
{
  public class MovingAverageAggregation : 
    PipelineAggregationBase,
    IMovingAverageAggregation,
    IPipelineAggregation,
    IAggregation
  {
    internal MovingAverageAggregation()
    {
    }

    public MovingAverageAggregation(string name, SingleBucketsPath bucketsPath)
      : base(name, (IBucketsPath) bucketsPath)
    {
    }

    public bool? Minimize { get; set; }

    public IMovingAverageModel Model { get; set; }

    public int? Predict { get; set; }

    public int? Window { get; set; }

    internal override void WrapInContainer(AggregationContainer c) => c.MovingAverage = (IMovingAverageAggregation) this;
  }
}
