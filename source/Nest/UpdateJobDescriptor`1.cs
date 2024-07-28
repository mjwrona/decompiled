// Decompiled with JetBrains decompiler
// Type: Nest.UpdateJobDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.MachineLearningApi;
using Elasticsearch.Net.Utf8Json;
using System;
using System.Collections.Generic;

namespace Nest
{
  public class UpdateJobDescriptor<TDocument> : 
    RequestDescriptorBase<UpdateJobDescriptor<TDocument>, UpdateJobRequestParameters, IUpdateJobRequest>,
    IUpdateJobRequest,
    IRequest<UpdateJobRequestParameters>,
    IRequest
    where TDocument : class
  {
    internal override ApiUrls ApiUrls => ApiUrlsLookups.MachineLearningUpdateJob;

    public UpdateJobDescriptor(Id jobId)
      : base((Func<RouteValues, RouteValues>) (r => r.Required("job_id", (IUrlParameter) jobId)))
    {
    }

    [SerializationConstructor]
    protected UpdateJobDescriptor()
    {
    }

    Id IUpdateJobRequest.JobId => this.Self.RouteValues.Get<Id>("job_id");

    IAnalysisMemoryLimit IUpdateJobRequest.AnalysisLimits { get; set; }

    Time IUpdateJobRequest.BackgroundPersistInterval { get; set; }

    Dictionary<string, object> IUpdateJobRequest.CustomSettings { get; set; }

    string IUpdateJobRequest.Description { get; set; }

    IModelPlotConfigEnabled IUpdateJobRequest.ModelPlotConfig { get; set; }

    long? IUpdateJobRequest.ModelSnapshotRetentionDays { get; set; }

    long? IUpdateJobRequest.DailyModelSnapshotRetentionAfterDays { get; set; }

    long? IUpdateJobRequest.RenormalizationWindowDays { get; set; }

    long? IUpdateJobRequest.ResultsRetentionDays { get; set; }

    bool? IUpdateJobRequest.AllowLazyOpen { get; set; }

    public UpdateJobDescriptor<TDocument> AnalysisLimits(
      Func<AnalysisMemoryLimitDescriptor, IAnalysisMemoryLimit> selector)
    {
      return this.Assign<Func<AnalysisMemoryLimitDescriptor, IAnalysisMemoryLimit>>(selector, (Action<IUpdateJobRequest, Func<AnalysisMemoryLimitDescriptor, IAnalysisMemoryLimit>>) ((a, v) => a.AnalysisLimits = v != null ? v(new AnalysisMemoryLimitDescriptor()) : (IAnalysisMemoryLimit) null));
    }

    public UpdateJobDescriptor<TDocument> BackgroundPersistInterval(Time backgroundPersistInterval) => this.Assign<Time>(backgroundPersistInterval, (Action<IUpdateJobRequest, Time>) ((a, v) => a.BackgroundPersistInterval = v));

    public UpdateJobDescriptor<TDocument> CustomSettings(
      Func<FluentDictionary<string, object>, FluentDictionary<string, object>> customSettingsDictionary)
    {
      return this.Assign<FluentDictionary<string, object>>(customSettingsDictionary(new FluentDictionary<string, object>()), (Action<IUpdateJobRequest, FluentDictionary<string, object>>) ((a, v) => a.CustomSettings = (Dictionary<string, object>) v));
    }

    public UpdateJobDescriptor<TDocument> Description(string description) => this.Assign<string>(description, (Action<IUpdateJobRequest, string>) ((a, v) => a.Description = v));

    public UpdateJobDescriptor<TDocument> ModelPlot(
      Func<ModelPlotConfigEnabledDescriptor<TDocument>, IModelPlotConfigEnabled> selector)
    {
      return this.Assign<Func<ModelPlotConfigEnabledDescriptor<TDocument>, IModelPlotConfigEnabled>>(selector, (Action<IUpdateJobRequest, Func<ModelPlotConfigEnabledDescriptor<TDocument>, IModelPlotConfigEnabled>>) ((a, v) => a.ModelPlotConfig = v != null ? v(new ModelPlotConfigEnabledDescriptor<TDocument>()) : (IModelPlotConfigEnabled) null));
    }

    public UpdateJobDescriptor<TDocument> ModelSnapshotRetentionDays(
      long? modelSnapshotRetentionDays)
    {
      return this.Assign<long?>(modelSnapshotRetentionDays, (Action<IUpdateJobRequest, long?>) ((a, v) => a.ModelSnapshotRetentionDays = v));
    }

    public UpdateJobDescriptor<TDocument> DailyModelSnapshotRetentionAfterDays(
      long? dailyModelSnapshotRetentionAfterDays)
    {
      return this.Assign<long?>(dailyModelSnapshotRetentionAfterDays, (Action<IUpdateJobRequest, long?>) ((a, v) => a.DailyModelSnapshotRetentionAfterDays = v));
    }

    public UpdateJobDescriptor<TDocument> RenormalizationWindowDays(long? renormalizationWindowDays) => this.Assign<long?>(renormalizationWindowDays, (Action<IUpdateJobRequest, long?>) ((a, v) => a.RenormalizationWindowDays = v));

    public UpdateJobDescriptor<TDocument> ResultsRetentionDays(long? resultsRetentionDays) => this.Assign<long?>(resultsRetentionDays, (Action<IUpdateJobRequest, long?>) ((a, v) => a.ResultsRetentionDays = v));

    public UpdateJobDescriptor<TDocument> AllowLazyOpen(bool? allowLazyOpen = true) => this.Assign<bool?>(allowLazyOpen, (Action<IUpdateJobRequest, bool?>) ((a, v) => a.AllowLazyOpen = v));
  }
}
