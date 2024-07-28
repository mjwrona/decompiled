// Decompiled with JetBrains decompiler
// Type: Nest.IUpdateJobRequest
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Specification.MachineLearningApi;
using Elasticsearch.Net.Utf8Json;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Nest
{
  [InterfaceDataContract]
  [MapsApi("ml.update_job.json")]
  public interface IUpdateJobRequest : IRequest<UpdateJobRequestParameters>, IRequest
  {
    [IgnoreDataMember]
    Id JobId { get; }

    [DataMember(Name = "analysis_limits")]
    IAnalysisMemoryLimit AnalysisLimits { get; set; }

    [DataMember(Name = "background_persist_interval")]
    Time BackgroundPersistInterval { get; set; }

    [DataMember(Name = "custom_settings")]
    [JsonFormatter(typeof (VerbatimDictionaryKeysFormatter<string, object>))]
    Dictionary<string, object> CustomSettings { get; set; }

    [DataMember(Name = "description")]
    string Description { get; set; }

    [DataMember(Name = "model_plot_config")]
    IModelPlotConfigEnabled ModelPlotConfig { get; set; }

    [DataMember(Name = "model_snapshot_retention_days")]
    long? ModelSnapshotRetentionDays { get; set; }

    [DataMember(Name = "daily_model_snapshot_retention_after_days")]
    long? DailyModelSnapshotRetentionAfterDays { get; set; }

    [DataMember(Name = "renormalization_window_days")]
    long? RenormalizationWindowDays { get; set; }

    [DataMember(Name = "results_retention_days")]
    long? ResultsRetentionDays { get; set; }

    [DataMember(Name = "allow_lazy_open")]
    bool? AllowLazyOpen { get; set; }
  }
}
