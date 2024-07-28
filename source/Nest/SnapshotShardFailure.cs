// Decompiled with JetBrains decompiler
// Type: Nest.SnapshotShardFailure
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using System.Runtime.Serialization;

namespace Nest
{
  [DataContract]
  public class SnapshotShardFailure
  {
    [DataMember(Name = "index")]
    public string Index { get; set; }

    [DataMember(Name = "node_id")]
    public string NodeId { get; set; }

    [DataMember(Name = "reason")]
    public string Reason { get; set; }

    [DataMember(Name = "shard_id")]
    [JsonFormatter(typeof (IntStringFormatter))]
    public string ShardId { get; set; }

    [DataMember(Name = "status")]
    public string Status { get; set; }
  }
}
