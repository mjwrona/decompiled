// Decompiled with JetBrains decompiler
// Type: Nest.PutJobDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.MachineLearningApi;
using Elasticsearch.Net.Utf8Json;
using System;

namespace Nest
{
  public class PutJobDescriptor<TDocument> : 
    RequestDescriptorBase<PutJobDescriptor<TDocument>, PutJobRequestParameters, IPutJobRequest>,
    IPutJobRequest,
    IRequest<PutJobRequestParameters>,
    IRequest
    where TDocument : class
  {
    internal override ApiUrls ApiUrls => ApiUrlsLookups.MachineLearningPutJob;

    public PutJobDescriptor(Id jobId)
      : base((Func<RouteValues, RouteValues>) (r => r.Required("job_id", (IUrlParameter) jobId)))
    {
    }

    [SerializationConstructor]
    protected PutJobDescriptor()
    {
    }

    Id IPutJobRequest.JobId => this.Self.RouteValues.Get<Id>("job_id");

    public PutJobDescriptor<TDocument> AllowNoIndices(bool? allownoindices = true) => this.Qs("allow_no_indices", (object) allownoindices);

    public PutJobDescriptor<TDocument> ExpandWildcards(Elasticsearch.Net.ExpandWildcards? expandwildcards) => this.Qs("expand_wildcards", (object) expandwildcards);

    public PutJobDescriptor<TDocument> IgnoreThrottled(bool? ignorethrottled = true) => this.Qs("ignore_throttled", (object) ignorethrottled);

    public PutJobDescriptor<TDocument> IgnoreUnavailable(bool? ignoreunavailable = true) => this.Qs("ignore_unavailable", (object) ignoreunavailable);

    IAnalysisConfig IPutJobRequest.AnalysisConfig { get; set; }

    IAnalysisLimits IPutJobRequest.AnalysisLimits { get; set; }

    IDataDescription IPutJobRequest.DataDescription { get; set; }

    string IPutJobRequest.Description { get; set; }

    IModelPlotConfig IPutJobRequest.ModelPlotConfig { get; set; }

    long? IPutJobRequest.ModelSnapshotRetentionDays { get; set; }

    long? IPutJobRequest.DailyModelSnapshotRetentionAfterDays { get; set; }

    IndexName IPutJobRequest.ResultsIndexName { get; set; }

    bool? IPutJobRequest.AllowLazyOpen { get; set; }

    public PutJobDescriptor<TDocument> AnalysisConfig(
      Func<AnalysisConfigDescriptor<TDocument>, IAnalysisConfig> selector)
    {
      return this.Assign<Func<AnalysisConfigDescriptor<TDocument>, IAnalysisConfig>>(selector, (Action<IPutJobRequest, Func<AnalysisConfigDescriptor<TDocument>, IAnalysisConfig>>) ((a, v) => a.AnalysisConfig = v != null ? v(new AnalysisConfigDescriptor<TDocument>()) : (IAnalysisConfig) null));
    }

    public PutJobDescriptor<TDocument> AnalysisLimits(
      Func<AnalysisLimitsDescriptor, IAnalysisLimits> selector)
    {
      return this.Assign<Func<AnalysisLimitsDescriptor, IAnalysisLimits>>(selector, (Action<IPutJobRequest, Func<AnalysisLimitsDescriptor, IAnalysisLimits>>) ((a, v) => a.AnalysisLimits = v != null ? v(new AnalysisLimitsDescriptor()) : (IAnalysisLimits) null));
    }

    public PutJobDescriptor<TDocument> DataDescription(
      Func<DataDescriptionDescriptor<TDocument>, IDataDescription> selector)
    {
      return this.Assign<IDataDescription>(selector.InvokeOrDefault<DataDescriptionDescriptor<TDocument>, IDataDescription>(new DataDescriptionDescriptor<TDocument>()), (Action<IPutJobRequest, IDataDescription>) ((a, v) => a.DataDescription = v));
    }

    public PutJobDescriptor<TDocument> Description(string description) => this.Assign<string>(description, (Action<IPutJobRequest, string>) ((a, v) => a.Description = v));

    public PutJobDescriptor<TDocument> ModelPlot(
      Func<ModelPlotConfigDescriptor<TDocument>, IModelPlotConfig> selector)
    {
      return this.Assign<Func<ModelPlotConfigDescriptor<TDocument>, IModelPlotConfig>>(selector, (Action<IPutJobRequest, Func<ModelPlotConfigDescriptor<TDocument>, IModelPlotConfig>>) ((a, v) => a.ModelPlotConfig = v != null ? v(new ModelPlotConfigDescriptor<TDocument>()) : (IModelPlotConfig) null));
    }

    public PutJobDescriptor<TDocument> ModelSnapshotRetentionDays(long? modelSnapshotRetentionDays) => this.Assign<long?>(modelSnapshotRetentionDays, (Action<IPutJobRequest, long?>) ((a, v) => a.ModelSnapshotRetentionDays = v));

    public PutJobDescriptor<TDocument> DailyModelSnapshotRetentionAfterDays(
      long? dailyModelSnapshotRetentionAfterDays)
    {
      return this.Assign<long?>(dailyModelSnapshotRetentionAfterDays, (Action<IPutJobRequest, long?>) ((a, v) => a.DailyModelSnapshotRetentionAfterDays = v));
    }

    public PutJobDescriptor<TDocument> ResultsIndexName(IndexName indexName) => this.Assign<IndexName>(indexName, (Action<IPutJobRequest, IndexName>) ((a, v) => a.ResultsIndexName = v));

    public PutJobDescriptor<TDocument> ResultsIndexName<TIndex>() => this.Assign<Type>(typeof (TIndex), (Action<IPutJobRequest, Type>) ((a, v) => a.ResultsIndexName = (IndexName) v));

    public PutJobDescriptor<TDocument> AllowLazyOpen(bool? allowLazyOpen = true) => this.Assign<bool?>(allowLazyOpen, (Action<IPutJobRequest, bool?>) ((a, v) => a.AllowLazyOpen = v));
  }
}
