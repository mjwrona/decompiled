// Decompiled with JetBrains decompiler
// Type: Nest.DiversifiedSamplerAggregation
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

namespace Nest
{
  public class DiversifiedSamplerAggregation : 
    BucketAggregationBase,
    IDiversifiedSamplerAggregation,
    IBucketAggregation,
    IAggregation
  {
    internal DiversifiedSamplerAggregation()
    {
    }

    public DiversifiedSamplerAggregation(string name)
      : base(name)
    {
    }

    public DiversifiedSamplerAggregationExecutionHint? ExecutionHint { get; set; }

    public Field Field { get; set; }

    public int? MaxDocsPerValue { get; set; }

    public IScript Script { get; set; }

    public int? ShardSize { get; set; }

    internal override void WrapInContainer(AggregationContainer c) => c.DiversifiedSampler = (IDiversifiedSamplerAggregation) this;
  }
}
