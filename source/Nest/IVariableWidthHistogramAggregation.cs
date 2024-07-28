// Decompiled with JetBrains decompiler
// Type: Nest.IVariableWidthHistogramAggregation
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using System.Runtime.Serialization;

namespace Nest
{
  [InterfaceDataContract]
  [ReadAs(typeof (VariableWidthHistogramAggregation))]
  public interface IVariableWidthHistogramAggregation : IBucketAggregation, IAggregation
  {
    [DataMember(Name = "field")]
    Field Field { get; set; }

    [DataMember(Name = "buckets")]
    int? Buckets { get; set; }

    [DataMember(Name = "initial_buffer")]
    int? InitialBuffer { get; set; }

    [DataMember(Name = "shard_size")]
    int? ShardSize { get; set; }
  }
}
