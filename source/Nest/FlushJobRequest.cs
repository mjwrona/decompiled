// Decompiled with JetBrains decompiler
// Type: Nest.FlushJobRequest
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
  public class FlushJobRequest : 
    PlainRequestBase<FlushJobRequestParameters>,
    IFlushJobRequest,
    IRequest<FlushJobRequestParameters>,
    IRequest
  {
    protected IFlushJobRequest Self => (IFlushJobRequest) this;

    internal override ApiUrls ApiUrls => ApiUrlsLookups.MachineLearningFlushJob;

    public FlushJobRequest(Id jobId)
      : base((Func<RouteValues, RouteValues>) (r => r.Required("job_id", (IUrlParameter) jobId)))
    {
    }

    [SerializationConstructor]
    protected FlushJobRequest()
    {
    }

    [IgnoreDataMember]
    Id IFlushJobRequest.JobId => this.Self.RouteValues.Get<Id>("job_id");

    public string SkipTime
    {
      get => this.Q<string>("skip_time");
      set => this.Q("skip_time", (object) value);
    }

    public DateTimeOffset? AdvanceTime { get; set; }

    public bool? CalculateInterim { get; set; }

    public DateTimeOffset? End { get; set; }

    public DateTimeOffset? Start { get; set; }
  }
}
