// Decompiled with JetBrains decompiler
// Type: Nest.CumulativeSumAggregation
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

namespace Nest
{
  public class CumulativeSumAggregation : 
    PipelineAggregationBase,
    ICumulativeSumAggregation,
    IPipelineAggregation,
    IAggregation
  {
    internal CumulativeSumAggregation()
    {
    }

    public CumulativeSumAggregation(string name, SingleBucketsPath bucketsPath)
      : base(name, (IBucketsPath) bucketsPath)
    {
    }

    internal override void WrapInContainer(AggregationContainer c) => c.CumulativeSum = (ICumulativeSumAggregation) this;
  }
}
