// Decompiled with JetBrains decompiler
// Type: Nest.IndexHealthStats
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Utf8Json;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Nest
{
  [DataContract]
  public class IndexHealthStats
  {
    [DataMember(Name = "active_primary_shards")]
    public int ActivePrimaryShards { get; internal set; }

    [DataMember(Name = "active_shards")]
    public int ActiveShards { get; internal set; }

    [DataMember(Name = "initializing_shards")]
    public int InitializingShards { get; internal set; }

    [DataMember(Name = "number_of_replicas")]
    public int NumberOfReplicas { get; internal set; }

    [DataMember(Name = "number_of_shards")]
    public int NumberOfShards { get; internal set; }

    [DataMember(Name = "relocating_shards")]
    public int RelocatingShards { get; internal set; }

    [DataMember(Name = "shards")]
    [JsonFormatter(typeof (VerbatimInterfaceReadOnlyDictionaryKeysFormatter<string, ShardHealthStats>))]
    public IReadOnlyDictionary<string, ShardHealthStats> Shards { get; internal set; } = EmptyReadOnly<string, ShardHealthStats>.Dictionary;

    [DataMember(Name = "status")]
    public Health Status { get; internal set; }

    [DataMember(Name = "unassigned_shards")]
    public int UnassignedShards { get; internal set; }
  }
}
