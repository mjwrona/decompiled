// Decompiled with JetBrains decompiler
// Type: Nest.Bucket
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Utf8Json;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Nest
{
  public class Bucket
  {
    [DataMember(Name = "anomaly_score")]
    public double AnomalyScore { get; internal set; }

    [DataMember(Name = "bucket_influencers")]
    public IReadOnlyCollection<BucketInfluencer> BucketInfluencers { get; internal set; } = EmptyReadOnly<BucketInfluencer>.Collection;

    [DataMember(Name = "bucket_span")]
    public Time BucketSpan { get; internal set; }

    [DataMember(Name = "event_count")]
    public long EventCount { get; internal set; }

    [DataMember(Name = "initial_anomaly_score")]
    public double InitialAnomalyScore { get; internal set; }

    [DataMember(Name = "is_interim")]
    public bool IsInterim { get; internal set; }

    [DataMember(Name = "job_id")]
    public string JobId { get; internal set; }

    [DataMember(Name = "partition_scores")]
    public IReadOnlyCollection<PartitionScore> PartitionScores { get; internal set; } = EmptyReadOnly<PartitionScore>.Collection;

    [DataMember(Name = "processing_time_ms")]
    public double ProcessingTimeMilliseconds { get; internal set; }

    [DataMember(Name = "result_type")]
    public string ResultType { get; internal set; }

    [DataMember(Name = "timestamp")]
    [JsonFormatter(typeof (DateTimeOffsetEpochMillisecondsFormatter))]
    public DateTimeOffset Timestamp { get; internal set; }
  }
}
