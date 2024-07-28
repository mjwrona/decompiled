// Decompiled with JetBrains decompiler
// Type: Nest.BucketInfluencer
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using System;
using System.Runtime.Serialization;

namespace Nest
{
  [DataContract]
  public class BucketInfluencer
  {
    [DataMember(Name = "bucket_span")]
    public long BucketSpan { get; internal set; }

    [DataMember(Name = "influencer_field_name")]
    public string InfluencerFieldName { get; internal set; }

    [DataMember(Name = "influencer_field_value")]
    public string InfluencerFieldValue { get; internal set; }

    [DataMember(Name = "influencer_score")]
    public double InfluencerScore { get; internal set; }

    [DataMember(Name = "initial_influencer_score")]
    public double InitialInfluencerScore { get; internal set; }

    [DataMember(Name = "is_interim")]
    public bool IsInterim { get; internal set; }

    [DataMember(Name = "job_id")]
    public string JobId { get; internal set; }

    [DataMember(Name = "probability")]
    public double Probability { get; internal set; }

    [DataMember(Name = "result_type")]
    public string ResultType { get; internal set; }

    [DataMember(Name = "timestamp")]
    [JsonFormatter(typeof (DateTimeOffsetEpochMillisecondsFormatter))]
    public DateTimeOffset Timestamp { get; internal set; }
  }
}
