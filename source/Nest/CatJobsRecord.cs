﻿// Decompiled with JetBrains decompiler
// Type: Nest.CatJobsRecord
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Runtime.Serialization;

namespace Nest
{
  [DataContract]
  public class CatJobsRecord : ICatRecord
  {
    [DataMember(Name = "assignment_explanation")]
    public string AssignmentExplanation { get; set; }

    [DataMember(Name = "buckets.count")]
    public string BucketsCount { get; set; }

    [DataMember(Name = "buckets.time.exp_avg")]
    public string BucketsTimeExpAvg { get; set; }

    [DataMember(Name = "buckets.time.exp_avg_hour")]
    public string BucketsTimeExpAvgHour { get; set; }

    [DataMember(Name = "buckets.time.max")]
    public string BucketsTimeMax { get; set; }

    [DataMember(Name = "buckets.time.min")]
    public string BucketsTimeMin { get; set; }

    [DataMember(Name = "buckets.time.total")]
    public string BucketsTimeTotal { get; set; }

    [DataMember(Name = "data.buckets")]
    public string DataBuckets { get; set; }

    [DataMember(Name = "data.earliest_record")]
    public string DataEarliestRecord { get; set; }

    [DataMember(Name = "data.empty_buckets")]
    public string DataEmptyBuckets { get; set; }

    [DataMember(Name = "data.input_bytes")]
    public string DataInputBytes { get; set; }

    [DataMember(Name = "data.input_fields")]
    public string DataInputFields { get; set; }

    [DataMember(Name = "data.input_records")]
    public string DataInputRecords { get; set; }

    [DataMember(Name = "data.invalid_dates")]
    public string DataInvalidDates { get; set; }

    [DataMember(Name = "data.last")]
    public string DataLast { get; set; }

    [DataMember(Name = "data.last_empty_bucket")]
    public string DataLastEmptyBucket { get; set; }

    [DataMember(Name = "data.last_sparse_bucket")]
    public string DataLastSparseBucket { get; set; }

    [DataMember(Name = "data.latest_record")]
    public string DataLatestRecord { get; set; }

    [DataMember(Name = "data.missing_fields")]
    public string DataMissingFields { get; set; }

    [DataMember(Name = "data.out_of_order_timestamps")]
    public string DataOutOfOrderTimestamps { get; set; }

    [DataMember(Name = "data.processed_fields")]
    public string DataProcessedFields { get; set; }

    [DataMember(Name = "data.processed_records")]
    public string DataProcessedRecords { get; set; }

    [DataMember(Name = "data.sparse_buckets")]
    public string DataSparseBuckets { get; set; }

    [DataMember(Name = "forecasts.memory.avg")]
    public string ForecastsMemoryAvg { get; internal set; }

    [DataMember(Name = "forecasts.memory.min")]
    public string ForecastsMemoryMin { get; internal set; }

    [DataMember(Name = "forecasts.memory.total")]
    public string ForecastsMemoryTotal { get; internal set; }

    [DataMember(Name = "forecasts.records.avg")]
    public string ForecastsRecordsAvg { get; internal set; }

    [DataMember(Name = "forecasts.records.max")]
    public string ForecastsRecordsMax { get; internal set; }

    [DataMember(Name = "forecasts.records.min")]
    public string ForecastsRecordsMin { get; internal set; }

    [DataMember(Name = "forecasts.records.total")]
    public string ForecastsRecordsTotal { get; internal set; }

    [DataMember(Name = "forecasts.time.avg")]
    public string ForecastsTimeAvg { get; internal set; }

    [DataMember(Name = "forecasts.time.max")]
    public string ForecastsTimeMax { get; internal set; }

    [DataMember(Name = "forecasts.time.min")]
    public string ForecastsTimeMin { get; internal set; }

    [DataMember(Name = "forecasts.total")]
    public string ForecastsTotal { get; internal set; }

    [DataMember(Name = "id")]
    public string Id { get; internal set; }

    [DataMember(Name = "model.bucket_allocation_failures")]
    public string ModelBucketAllocationFailures { get; internal set; }

    [DataMember(Name = "model.by_fields")]
    public string ModelByFields { get; internal set; }

    [DataMember(Name = "model.bytes")]
    public string ModelBytes { get; internal set; }

    [DataMember(Name = "model.bytes")]
    public string ModelBytesExceeded { get; internal set; }

    [DataMember(Name = "model.categorization_status")]
    public ModelCategorizationStatus ModelCategorizationStatus { get; internal set; }

    [DataMember(Name = "model.categorized_doc_count")]
    public string ModelCategorizedDocCount { get; internal set; }

    [DataMember(Name = "model.dead_category_count")]
    public string ModelDeadCategoryCount { get; internal set; }

    [DataMember(Name = "model.frequent_category_count")]
    public string ModelFrequentCategoryCount { get; internal set; }

    [DataMember(Name = "model.log_time")]
    public string ModelLogTime { get; internal set; }

    [DataMember(Name = "model.memory_limit")]
    public string ModelMemoryLimit { get; internal set; }

    [DataMember(Name = "model.memory_status")]
    public ModelMemoryStatus ModelMemoryStatus { get; internal set; }

    [DataMember(Name = "model.over_fields")]
    public string ModelOverFields { get; internal set; }

    [DataMember(Name = "model.partition_fields")]
    public string ModelPartitionFields { get; internal set; }

    [DataMember(Name = "model.rare_category_count")]
    public string ModelRareCategoryCount { get; internal set; }

    [DataMember(Name = "model.timestamp")]
    public string ModelTimestamp { get; internal set; }

    [DataMember(Name = "node.address")]
    public string NodeAddress { get; internal set; }

    [DataMember(Name = "node.ephemeral_id")]
    public string NodeEphemeralId { get; internal set; }

    [DataMember(Name = "node.id")]
    public string NodeId { get; internal set; }

    [DataMember(Name = "node.name")]
    public string NodeName { get; internal set; }

    [DataMember(Name = "opened_time")]
    public string OpenedTime { get; internal set; }

    [DataMember(Name = "state")]
    public JobState State { get; internal set; }
  }
}
