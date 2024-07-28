// Decompiled with JetBrains decompiler
// Type: Nest.ModelSizeStats
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using System;
using System.Runtime.Serialization;

namespace Nest
{
  [DataContract]
  public class ModelSizeStats
  {
    [DataMember(Name = "bucket_allocation_failures_count")]
    public long BucketAllocationFailuresCount { get; internal set; }

    [DataMember(Name = "categorized_doc_count")]
    public long CategorizedDocCount { get; internal set; }

    [DataMember(Name = "categorization_status")]
    public ModelCategorizationStatus CategorizationStatus { get; internal set; }

    [DataMember(Name = "dead_category_count")]
    public long DeadCategoryCount { get; internal set; }

    [DataMember(Name = "failed_category_count")]
    public long FailedCategoryCount { get; internal set; }

    [DataMember(Name = "frequent_category_count")]
    public long FrequentCategoryCount { get; internal set; }

    [DataMember(Name = "job_id")]
    public string JobId { get; internal set; }

    [DataMember(Name = "log_time")]
    [JsonFormatter(typeof (DateTimeOffsetEpochMillisecondsFormatter))]
    public DateTimeOffset LogTime { get; internal set; }

    [DataMember(Name = "memory_status")]
    public MemoryStatus MemoryStatus { get; internal set; }

    [DataMember(Name = "model_bytes")]
    public long ModelBytes { get; internal set; }

    [DataMember(Name = "model_bytes_exceeded")]
    public long ModelBytesExceeded { get; internal set; }

    [DataMember(Name = "model_bytes_memory_limit")]
    public long ModelBytesMemoryLimit { get; internal set; }

    [DataMember(Name = "rare_category_count")]
    public long RareCategoryCount { get; internal set; }

    [DataMember(Name = "result_type")]
    public string ResultType { get; internal set; }

    [DataMember(Name = "timestamp")]
    [JsonFormatter(typeof (NullableDateTimeOffsetEpochMillisecondsFormatter))]
    public DateTimeOffset? Timestamp { get; internal set; }

    [DataMember(Name = "total_by_field_count")]
    public long TotalByFieldCount { get; internal set; }

    [DataMember(Name = "total_category_count")]
    public long TotalCategoryCount { get; internal set; }

    [DataMember(Name = "total_over_field_count")]
    public long TotalOverFieldCount { get; internal set; }

    [DataMember(Name = "total_partition_field_count")]
    public long TotalPartitionFieldCount { get; internal set; }
  }
}
