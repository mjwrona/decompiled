// Decompiled with JetBrains decompiler
// Type: Nest.ClusterIndicesStats
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Runtime.Serialization;

namespace Nest
{
  [DataContract]
  public class ClusterIndicesStats
  {
    [DataMember(Name = "completion")]
    public CompletionStats Completion { get; internal set; }

    [DataMember(Name = "count")]
    public long Count { get; internal set; }

    [DataMember(Name = "docs")]
    public DocStats Documents { get; internal set; }

    [DataMember(Name = "fielddata")]
    public FielddataStats Fielddata { get; internal set; }

    [DataMember(Name = "query_cache")]
    public QueryCacheStats QueryCache { get; internal set; }

    [DataMember(Name = "segments")]
    public SegmentsStats Segments { get; internal set; }

    [DataMember(Name = "shards")]
    public ClusterIndicesShardsStats Shards { get; internal set; }

    [DataMember(Name = "store")]
    public StoreStats Store { get; internal set; }
  }
}
