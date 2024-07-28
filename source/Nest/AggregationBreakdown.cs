// Decompiled with JetBrains decompiler
// Type: Nest.AggregationBreakdown
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Runtime.Serialization;

namespace Nest
{
  public class AggregationBreakdown
  {
    [DataMember(Name = "build_aggregation")]
    public long BuildAggregation { get; internal set; }

    [DataMember(Name = "build_aggregation_count")]
    public long BuildAggregationCount { get; internal set; }

    [DataMember(Name = "collect")]
    public long Collect { get; internal set; }

    [DataMember(Name = "collect_count")]
    public long CollectCount { get; internal set; }

    [DataMember(Name = "initialize")]
    public long Initialize { get; internal set; }

    [DataMember(Name = "intialize_count")]
    public long InitializeCount { get; internal set; }

    [DataMember(Name = "reduce")]
    public long Reduce { get; internal set; }

    [DataMember(Name = "reduce_count")]
    public long ReduceCount { get; internal set; }
  }
}
