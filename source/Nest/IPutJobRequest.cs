// Decompiled with JetBrains decompiler
// Type: Nest.IPutJobRequest
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Specification.MachineLearningApi;
using Elasticsearch.Net.Utf8Json;
using System.Runtime.Serialization;

namespace Nest
{
  [InterfaceDataContract]
  [MapsApi("ml.put_job.json")]
  public interface IPutJobRequest : IRequest<PutJobRequestParameters>, IRequest
  {
    [IgnoreDataMember]
    Id JobId { get; }

    [DataMember(Name = "analysis_config")]
    IAnalysisConfig AnalysisConfig { get; set; }

    [DataMember(Name = "analysis_limits")]
    IAnalysisLimits AnalysisLimits { get; set; }

    [DataMember(Name = "data_description")]
    IDataDescription DataDescription { get; set; }

    [DataMember(Name = "description")]
    string Description { get; set; }

    [DataMember(Name = "model_plot")]
    IModelPlotConfig ModelPlotConfig { get; set; }

    [DataMember(Name = "model_snapshot_retention_days")]
    long? ModelSnapshotRetentionDays { get; set; }

    [DataMember(Name = "daily_model_snapshot_retention_after_days")]
    long? DailyModelSnapshotRetentionAfterDays { get; set; }

    [DataMember(Name = "results_index_name")]
    IndexName ResultsIndexName { get; set; }

    [DataMember(Name = "allow_lazy_open")]
    bool? AllowLazyOpen { get; set; }
  }
}
