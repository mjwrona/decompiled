// Decompiled with JetBrains decompiler
// Type: Nest.DataStreamsStatsResponse
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Nest
{
  public class DataStreamsStatsResponse : ResponseBase
  {
    [DataMember(Name = "_shards")]
    public ShardStatistics Shards { get; internal set; }

    [DataMember(Name = "data_stream_count")]
    public int DataStreamCount { get; internal set; }

    [DataMember(Name = "backing_indices")]
    public int BackingIndices { get; internal set; }

    [DataMember(Name = "total_store_size")]
    public string TotalStoreSize { get; internal set; }

    [DataMember(Name = "total_store_size_bytes")]
    public long TotalStoreSizeBytes { get; internal set; }

    [DataMember(Name = "data_streams")]
    public IReadOnlyCollection<DataStreamStats> DataStreams { get; internal set; } = EmptyReadOnly<DataStreamStats>.Collection;
  }
}
