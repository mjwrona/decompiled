// Decompiled with JetBrains decompiler
// Type: Nest.ISamplerAggregation
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using System;
using System.Runtime.Serialization;

namespace Nest
{
  [InterfaceDataContract]
  [ReadAs(typeof (SamplerAggregation))]
  public interface ISamplerAggregation : IBucketAggregation, IAggregation
  {
    [Obsolete("This option is not valid for the sampler aggregation and only applies to IDiversifiedSamplerAggregation")]
    [DataMember(Name = "execution_hint")]
    SamplerAggregationExecutionHint? ExecutionHint { get; set; }

    [Obsolete("This option is not valid for the sampler aggregation and only applies to IDiversifiedSamplerAggregation")]
    [DataMember(Name = "max_docs_per_value")]
    int? MaxDocsPerValue { get; set; }

    [Obsolete("This option is not valid for the sampler aggregation and only applies to IDiversifiedSamplerAggregation")]
    [DataMember(Name = "script")]
    IScript Script { get; set; }

    [DataMember(Name = "shard_size")]
    int? ShardSize { get; set; }
  }
}
