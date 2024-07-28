﻿// Decompiled with JetBrains decompiler
// Type: Nest.IndexStats
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Runtime.Serialization;

namespace Nest
{
  [DataContract]
  public class IndexStats
  {
    [DataMember(Name = "completion")]
    public CompletionStats Completion { get; set; }

    [DataMember(Name = "docs")]
    public DocStats Documents { get; set; }

    [DataMember(Name = "fielddata")]
    public FielddataStats Fielddata { get; set; }

    [DataMember(Name = "flush")]
    public FlushStats Flush { get; set; }

    [DataMember(Name = "get")]
    public GetStats Get { get; set; }

    [DataMember(Name = "indexing")]
    public IndexingStats Indexing { get; set; }

    [DataMember(Name = "merges")]
    public MergesStats Merges { get; set; }

    [DataMember(Name = "query_cache")]
    public QueryCacheStats QueryCache { get; set; }

    [DataMember(Name = "recovery")]
    public RecoveryStats Recovery { get; set; }

    [DataMember(Name = "refresh")]
    public RefreshStats Refresh { get; set; }

    [DataMember(Name = "request_cache")]
    public RequestCacheStats RequestCache { get; set; }

    [DataMember(Name = "search")]
    public SearchStats Search { get; set; }

    [DataMember(Name = "segments")]
    public SegmentsStats Segments { get; set; }

    [DataMember(Name = "shard_stats")]
    public ShardTotalStats ShardStats { get; set; }

    [DataMember(Name = "store")]
    public StoreStats Store { get; set; }

    [DataMember(Name = "translog")]
    public TranslogStats Translog { get; set; }

    [DataMember(Name = "warmer")]
    public WarmerStats Warmer { get; set; }
  }
}
