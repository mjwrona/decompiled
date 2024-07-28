// Decompiled with JetBrains decompiler
// Type: Nest.PutJobResponse
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using System;
using System.Runtime.Serialization;

namespace Nest
{
  public class PutJobResponse : ResponseBase
  {
    [DataMember(Name = "analysis_config")]
    public IAnalysisConfig AnalysisConfig { get; internal set; }

    [DataMember(Name = "analysis_limits")]
    public IAnalysisLimits AnalysisLimits { get; internal set; }

    [DataMember(Name = "background_persist_interval")]
    public Time BackgroundPersistInterval { get; internal set; }

    [JsonFormatter(typeof (DateTimeOffsetEpochMillisecondsFormatter))]
    [DataMember(Name = "create_time")]
    public DateTimeOffset CreateTime { get; internal set; }

    [DataMember(Name = "data_description")]
    public IDataDescription DataDescription { get; internal set; }

    [DataMember(Name = "description")]
    public string Description { get; internal set; }

    [DataMember(Name = "job_id")]
    public string JobId { get; internal set; }

    [DataMember(Name = "job_type")]
    public string JobType { get; internal set; }

    [DataMember(Name = "model_plot")]
    public IModelPlotConfig ModelPlotConfig { get; internal set; }

    [DataMember(Name = "model_snapshot_id")]
    public string ModelSnapshotId { get; internal set; }

    [DataMember(Name = "model_snapshot_retention_days")]
    public long? ModelSnapshotRetentionDays { get; internal set; }

    [DataMember(Name = "renormalization_window_days")]
    public long? RenormalizationWindowDays { get; internal set; }

    [DataMember(Name = "results_index_name")]
    public string ResultsIndexName { get; internal set; }

    [DataMember(Name = "results_retention_days")]
    public long? ResultsRetentionDays { get; internal set; }

    [DataMember(Name = "allow_lazy_open")]
    public bool? AllowLazyOpen { get; set; }
  }
}
