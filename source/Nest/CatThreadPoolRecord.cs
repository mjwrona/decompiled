// Decompiled with JetBrains decompiler
// Type: Nest.CatThreadPoolRecord
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using System.Runtime.Serialization;

namespace Nest
{
  [DataContract]
  public class CatThreadPoolRecord : ICatRecord
  {
    [DataMember(Name = "active")]
    [JsonFormatter(typeof (StringIntFormatter))]
    public int Active { get; set; }

    [DataMember(Name = "completed")]
    [JsonFormatter(typeof (NullableStringLongFormatter))]
    public long? Completed { get; set; }

    [DataMember(Name = "core")]
    [JsonFormatter(typeof (NullableStringIntFormatter))]
    public int? Core { get; set; }

    [DataMember(Name = "ephemeral_node_id")]
    public string EphemeralNodeId { get; set; }

    [DataMember(Name = "host")]
    public string Host { get; set; }

    [DataMember(Name = "ip")]
    public string Ip { get; set; }

    [DataMember(Name = "keep_alive")]
    public Time KeepAlive { get; set; }

    [DataMember(Name = "largest")]
    [JsonFormatter(typeof (NullableStringIntFormatter))]
    public int? Largest { get; set; }

    [DataMember(Name = "max")]
    [JsonFormatter(typeof (NullableStringIntFormatter))]
    public int? Maximum { get; set; }

    [DataMember(Name = "name")]
    public string Name { get; set; }

    [DataMember(Name = "node_id")]
    public string NodeId { get; set; }

    [DataMember(Name = "node_name")]
    public string NodeName { get; set; }

    [DataMember(Name = "pool_size")]
    [JsonFormatter(typeof (NullableStringIntFormatter))]
    public int? PoolSize { get; set; }

    [DataMember(Name = "port")]
    [JsonFormatter(typeof (NullableStringIntFormatter))]
    public int? Port { get; set; }

    [DataMember(Name = "pid")]
    [JsonFormatter(typeof (NullableStringIntFormatter))]
    public int? ProcessId { get; set; }

    [DataMember(Name = "queue")]
    [JsonFormatter(typeof (StringIntFormatter))]
    public int Queue { get; set; }

    [DataMember(Name = "queue_size")]
    [JsonFormatter(typeof (NullableStringIntFormatter))]
    public int? QueueSize { get; set; }

    [DataMember(Name = "rejected")]
    [JsonFormatter(typeof (StringLongFormatter))]
    public long Rejected { get; set; }

    [DataMember(Name = "size")]
    [JsonFormatter(typeof (NullableStringIntFormatter))]
    public int? Size { get; set; }

    [DataMember(Name = "type")]
    public string Type { get; set; }
  }
}
