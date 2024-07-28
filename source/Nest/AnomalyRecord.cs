// Decompiled with JetBrains decompiler
// Type: Nest.AnomalyRecord
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
  [DataContract]
  public class AnomalyRecord
  {
    [DataMember(Name = "actual")]
    public IReadOnlyCollection<double> Actual { get; internal set; } = EmptyReadOnly<double>.Collection;

    [DataMember(Name = "bucket_span")]
    public Time BucketSpan { get; internal set; }

    [DataMember(Name = "by_field_name")]
    public string ByFieldName { get; internal set; }

    [DataMember(Name = "by_field_value")]
    public string ByFieldValue { get; internal set; }

    [DataMember(Name = "causes")]
    public IReadOnlyCollection<AnomalyCause> Causes { get; internal set; } = EmptyReadOnly<AnomalyCause>.Collection;

    [DataMember(Name = "detector_index")]
    public int DetectorIndex { get; internal set; }

    [DataMember(Name = "field_name")]
    public string FieldName { get; internal set; }

    [DataMember(Name = "function")]
    public string Function { get; internal set; }

    [DataMember(Name = "function_description")]
    public string FunctionDescription { get; internal set; }

    [DataMember(Name = "influencers")]
    public IReadOnlyCollection<Influence> Influencers { get; internal set; } = EmptyReadOnly<Influence>.Collection;

    [DataMember(Name = "initial_record_score")]
    public double InitialRecordScore { get; internal set; }

    [DataMember(Name = "is_interim")]
    public bool IsInterim { get; internal set; }

    [DataMember(Name = "job_id")]
    public string JobId { get; internal set; }

    [DataMember(Name = "over_field_name")]
    public string OverFieldName { get; internal set; }

    [DataMember(Name = "over_field_value")]
    public string OverFieldValue { get; internal set; }

    [DataMember(Name = "partition_field_name")]
    public string PartitionFieldName { get; internal set; }

    [DataMember(Name = "partition_field_value")]
    public string PartitionFieldValue { get; internal set; }

    [DataMember(Name = "probability")]
    public double Probability { get; internal set; }

    [DataMember(Name = "record_score")]
    public double RecordScore { get; internal set; }

    [DataMember(Name = "result_type")]
    public string ResultType { get; internal set; }

    [DataMember(Name = "timestamp")]
    [JsonFormatter(typeof (DateTimeOffsetEpochMillisecondsFormatter))]
    public DateTimeOffset Timestamp { get; internal set; }

    [DataMember(Name = "typical")]
    public IReadOnlyCollection<double> Typical { get; internal set; } = EmptyReadOnly<double>.Collection;
  }
}
