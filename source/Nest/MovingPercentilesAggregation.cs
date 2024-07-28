// Decompiled with JetBrains decompiler
// Type: Nest.MovingPercentilesAggregation
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

namespace Nest
{
  public class MovingPercentilesAggregation : 
    PipelineAggregationBase,
    IMovingPercentilesAggregation,
    IPipelineAggregation,
    IAggregation
  {
    internal MovingPercentilesAggregation()
    {
    }

    public MovingPercentilesAggregation(string name, SingleBucketsPath bucketsPath)
      : base(name, (IBucketsPath) bucketsPath)
    {
    }

    public int? Window { get; set; }

    public int? Shift { get; set; }

    internal override void WrapInContainer(AggregationContainer c) => c.MovingPercentiles = (IMovingPercentilesAggregation) this;
  }
}
