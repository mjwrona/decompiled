// Decompiled with JetBrains decompiler
// Type: Nest.UpdateJobRequest
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.MachineLearningApi;
using Elasticsearch.Net.Utf8Json;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Nest
{
  public class UpdateJobRequest : 
    PlainRequestBase<UpdateJobRequestParameters>,
    IUpdateJobRequest,
    IRequest<UpdateJobRequestParameters>,
    IRequest
  {
    protected IUpdateJobRequest Self => (IUpdateJobRequest) this;

    internal override ApiUrls ApiUrls => ApiUrlsLookups.MachineLearningUpdateJob;

    public UpdateJobRequest(Id jobId)
      : base((Func<RouteValues, RouteValues>) (r => r.Required("job_id", (IUrlParameter) jobId)))
    {
    }

    [SerializationConstructor]
    protected UpdateJobRequest()
    {
    }

    [IgnoreDataMember]
    Id IUpdateJobRequest.JobId => this.Self.RouteValues.Get<Id>("job_id");

    public IAnalysisMemoryLimit AnalysisLimits { get; set; }

    public Time BackgroundPersistInterval { get; set; }

    public Dictionary<string, object> CustomSettings { get; set; }

    public string Description { get; set; }

    public IModelPlotConfigEnabled ModelPlotConfig { get; set; }

    public long? ModelSnapshotRetentionDays { get; set; }

    public long? DailyModelSnapshotRetentionAfterDays { get; set; }

    public long? RenormalizationWindowDays { get; set; }

    public long? ResultsRetentionDays { get; set; }

    public bool? AllowLazyOpen { get; set; }
  }
}
