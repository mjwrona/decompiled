// Decompiled with JetBrains decompiler
// Type: Nest.CatSnapshotsRecord
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using System.Runtime.Serialization;

namespace Nest
{
  [DataContract]
  public class CatSnapshotsRecord : ICatRecord
  {
    [DataMember(Name = "duration")]
    public Time Duration { get; set; }

    [DataMember(Name = "end_epoch")]
    [JsonFormatter(typeof (StringLongFormatter))]
    public long EndEpoch { get; set; }

    [DataMember(Name = "end_time")]
    public string EndTime { get; set; }

    [DataMember(Name = "failed_shards")]
    [JsonFormatter(typeof (StringLongFormatter))]
    public long FailedShards { get; set; }

    [DataMember(Name = "id")]
    public string Id { get; set; }

    [DataMember(Name = "indices")]
    [JsonFormatter(typeof (StringLongFormatter))]
    public long Indices { get; set; }

    [DataMember(Name = "start_epoch")]
    [JsonFormatter(typeof (StringLongFormatter))]
    public long StartEpoch { get; set; }

    [DataMember(Name = "start_time")]
    public string StartTime { get; set; }

    [DataMember(Name = "status")]
    public string Status { get; set; }

    [DataMember(Name = "successful_shards")]
    [JsonFormatter(typeof (StringLongFormatter))]
    public long SuccessfulShards { get; set; }

    [DataMember(Name = "total_shards")]
    [JsonFormatter(typeof (StringLongFormatter))]
    public long TotalShards { get; set; }
  }
}
