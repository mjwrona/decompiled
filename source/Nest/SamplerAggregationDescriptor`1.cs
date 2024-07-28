// Decompiled with JetBrains decompiler
// Type: Nest.SamplerAggregationDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class SamplerAggregationDescriptor<T> : 
    BucketAggregationDescriptorBase<SamplerAggregationDescriptor<T>, ISamplerAggregation, T>,
    ISamplerAggregation,
    IBucketAggregation,
    IAggregation
    where T : class
  {
    SamplerAggregationExecutionHint? ISamplerAggregation.ExecutionHint { get; set; }

    int? ISamplerAggregation.MaxDocsPerValue { get; set; }

    IScript ISamplerAggregation.Script { get; set; }

    int? ISamplerAggregation.ShardSize { get; set; }

    public SamplerAggregationDescriptor<T> ExecutionHint(
      SamplerAggregationExecutionHint? executionHint)
    {
      return this.Assign<SamplerAggregationExecutionHint?>(executionHint, (Action<ISamplerAggregation, SamplerAggregationExecutionHint?>) ((a, v) => a.ExecutionHint = v));
    }

    public SamplerAggregationDescriptor<T> MaxDocsPerValue(int? maxDocs) => this.Assign<int?>(maxDocs, (Action<ISamplerAggregation, int?>) ((a, v) => a.MaxDocsPerValue = v));

    public SamplerAggregationDescriptor<T> Script(string script) => this.Assign<InlineScript>((InlineScript) script, (Action<ISamplerAggregation, InlineScript>) ((a, v) => a.Script = (IScript) v));

    public SamplerAggregationDescriptor<T> Script(Func<ScriptDescriptor, IScript> scriptSelector) => this.Assign<Func<ScriptDescriptor, IScript>>(scriptSelector, (Action<ISamplerAggregation, Func<ScriptDescriptor, IScript>>) ((a, v) => a.Script = v != null ? v(new ScriptDescriptor()) : (IScript) null));

    public SamplerAggregationDescriptor<T> ShardSize(int? shardSize) => this.Assign<int?>(shardSize, (Action<ISamplerAggregation, int?>) ((a, v) => a.ShardSize = v));
  }
}
