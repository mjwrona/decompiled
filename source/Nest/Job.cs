// Decompiled with JetBrains decompiler
// Type: Nest.Job
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using System;
using System.Runtime.Serialization;

namespace Nest
{
  [DataContract]
  public class Job
  {
    [DataMember(Name = "analysis_config")]
    public IAnalysisConfig AnalysisConfig { get; set; }

    [DataMember(Name = "analysis_limits")]
    public IAnalysisLimits AnalysisLimits { get; set; }

    [DataMember(Name = "background_persist_interval")]
    public Time BackgroundPersistInterval { get; set; }

    [DataMember(Name = "create_time")]
    [JsonFormatter(typeof (DateTimeOffsetEpochMillisecondsFormatter))]
    public DateTimeOffset CreateTime { get; set; }

    [DataMember(Name = "data_description")]
    public IDataDescription DataDescription { get; set; }

    [DataMember(Name = "description")]
    public string Description { get; set; }

    [DataMember(Name = "finished_time")]
    [JsonFormatter(typeof (NullableDateTimeOffsetEpochMillisecondsFormatter))]
    public DateTimeOffset? FinishedTime { get; set; }

    [DataMember(Name = "job_id")]
    public string JobId { get; set; }

    [DataMember(Name = "job_type")]
    public string JobType { get; set; }

    [DataMember(Name = "model_plot")]
    public IModelPlotConfig ModelPlotConfig { get; set; }

    [DataMember(Name = "model_snapshot_id")]
    public string ModelSnapshotId { get; set; }

    [DataMember(Name = "model_snapshot_retention_days")]
    public long? ModelSnapshotRetentionDays { get; set; }

    [DataMember(Name = "daily_model_snapshot_retention_after_days")]
    public long? DailyModelSnapshotRetentionAfterDays { get; set; }

    [DataMember(Name = "renormalization_window_days")]
    public long? RenormalizationWindowDays { get; set; }

    [DataMember(Name = "results_index_name")]
    public string ResultsIndexName { get; set; }

    [DataMember(Name = "results_retention_days")]
    public long? ResultsRetentionDays { get; set; }

    [DataMember(Name = "allow_lazy_open")]
    public bool? AllowLazyOpen { get; set; }
  }
}
