// Decompiled with JetBrains decompiler
// Type: Nest.PostJobDataResponse
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using System;
using System.Runtime.Serialization;

namespace Nest
{
  public class PostJobDataResponse : ResponseBase
  {
    [DataMember(Name = "bucket_count")]
    public long BucketCount { get; internal set; }

    [DataMember(Name = "earliest_record_timestamp")]
    [JsonFormatter(typeof (NullableDateTimeOffsetEpochMillisecondsFormatter))]
    public DateTimeOffset? EarliestRecordTimestamp { get; internal set; }

    [DataMember(Name = "empty_bucket_count")]
    public long EmptyBucketCount { get; internal set; }

    [DataMember(Name = "input_bytes")]
    public long InputBytes { get; internal set; }

    [DataMember(Name = "input_field_count")]
    public long InputFieldCount { get; internal set; }

    [DataMember(Name = "input_record_count")]
    public long InputRecordCount { get; internal set; }

    [DataMember(Name = "invalid_date_count")]
    public long InvalidDateCount { get; internal set; }

    [DataMember(Name = "job_id")]
    public string JobId { get; internal set; }

    [DataMember(Name = "last_data_time")]
    [JsonFormatter(typeof (DateTimeOffsetEpochMillisecondsFormatter))]
    public DateTimeOffset LastDataTime { get; internal set; }

    [DataMember(Name = "latest_record_timestamp")]
    [JsonFormatter(typeof (NullableDateTimeOffsetEpochMillisecondsFormatter))]
    public DateTimeOffset? LatestRecordTimestamp { get; internal set; }

    [DataMember(Name = "missing_field_count")]
    public long MissingFieldCount { get; internal set; }

    [DataMember(Name = "out_of_order_timestamp_count")]
    public long OutOfOrderTimestampCount { get; internal set; }

    [DataMember(Name = "processed_field_count")]
    public long ProcessedFieldCount { get; internal set; }

    [DataMember(Name = "processed_record_count")]
    public long ProcessedRecordCount { get; internal set; }

    [DataMember(Name = "sparse_bucket_count")]
    public long SparseBucketCount { get; internal set; }
  }
}
