// Decompiled with JetBrains decompiler
// Type: Nest.PutJobRequest
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.MachineLearningApi;
using Elasticsearch.Net.Utf8Json;
using System;
using System.Runtime.Serialization;

namespace Nest
{
  public class PutJobRequest : 
    PlainRequestBase<PutJobRequestParameters>,
    IPutJobRequest,
    IRequest<PutJobRequestParameters>,
    IRequest
  {
    protected IPutJobRequest Self => (IPutJobRequest) this;

    internal override ApiUrls ApiUrls => ApiUrlsLookups.MachineLearningPutJob;

    public PutJobRequest(Id jobId)
      : base((Func<RouteValues, RouteValues>) (r => r.Required("job_id", (IUrlParameter) jobId)))
    {
    }

    [SerializationConstructor]
    protected PutJobRequest()
    {
    }

    [IgnoreDataMember]
    Id IPutJobRequest.JobId => this.Self.RouteValues.Get<Id>("job_id");

    public bool? AllowNoIndices
    {
      get => this.Q<bool?>("allow_no_indices");
      set => this.Q("allow_no_indices", (object) value);
    }

    public Elasticsearch.Net.ExpandWildcards? ExpandWildcards
    {
      get => this.Q<Elasticsearch.Net.ExpandWildcards?>("expand_wildcards");
      set => this.Q("expand_wildcards", (object) value);
    }

    public bool? IgnoreThrottled
    {
      get => this.Q<bool?>("ignore_throttled");
      set => this.Q("ignore_throttled", (object) value);
    }

    public bool? IgnoreUnavailable
    {
      get => this.Q<bool?>("ignore_unavailable");
      set => this.Q("ignore_unavailable", (object) value);
    }

    public IAnalysisConfig AnalysisConfig { get; set; }

    public IAnalysisLimits AnalysisLimits { get; set; }

    public IDataDescription DataDescription { get; set; }

    public string Description { get; set; }

    public IModelPlotConfig ModelPlotConfig { get; set; }

    public long? ModelSnapshotRetentionDays { get; set; }

    public long? DailyModelSnapshotRetentionAfterDays { get; set; }

    public IndexName ResultsIndexName { get; set; }

    public bool? AllowLazyOpen { get; set; }
  }
}
