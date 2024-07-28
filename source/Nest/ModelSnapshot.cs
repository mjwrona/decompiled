// Decompiled with JetBrains decompiler
// Type: Nest.ModelSnapshot
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using System;
using System.Runtime.Serialization;

namespace Nest
{
  [DataContract]
  public class ModelSnapshot
  {
    [DataMember(Name = "description")]
    public string Description { get; internal set; }

    [DataMember(Name = "job_id")]
    public string JobId { get; internal set; }

    [DataMember(Name = "latest_record_time_stamp")]
    [JsonFormatter(typeof (NullableDateTimeOffsetEpochMillisecondsFormatter))]
    public DateTimeOffset? LatestRecordTimeStamp { get; internal set; }

    [DataMember(Name = "latest_result_time_stamp")]
    [JsonFormatter(typeof (NullableDateTimeOffsetEpochMillisecondsFormatter))]
    public DateTimeOffset? LatestResultTimeStamp { get; internal set; }

    [DataMember(Name = "model_size_stats")]
    public ModelSizeStats ModelSizeStats { get; internal set; }

    [DataMember(Name = "retain")]
    public bool Retain { get; internal set; }

    [DataMember(Name = "snapshot_doc_count")]
    public long SnapshotDocCount { get; internal set; }

    [DataMember(Name = "snapshot_id")]
    public string SnapshotId { get; internal set; }

    [DataMember(Name = "timestamp")]
    [JsonFormatter(typeof (DateTimeOffsetEpochMillisecondsFormatter))]
    public DateTimeOffset Timestamp { get; internal set; }
  }
}
