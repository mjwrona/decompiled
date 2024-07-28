// Decompiled with JetBrains decompiler
// Type: Nest.SamplerAggregation
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

namespace Nest
{
  public class SamplerAggregation : 
    BucketAggregationBase,
    ISamplerAggregation,
    IBucketAggregation,
    IAggregation
  {
    internal SamplerAggregation()
    {
    }

    public SamplerAggregation(string name)
      : base(name)
    {
    }

    public SamplerAggregationExecutionHint? ExecutionHint { get; set; }

    public int? MaxDocsPerValue { get; set; }

    public IScript Script { get; set; }

    public int? ShardSize { get; set; }

    internal override void WrapInContainer(AggregationContainer c) => c.Sampler = (ISamplerAggregation) this;
  }
}
