// Decompiled with JetBrains decompiler
// Type: Nest.GetInfluencersRequest
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
  public class GetInfluencersRequest : 
    PlainRequestBase<GetInfluencersRequestParameters>,
    IGetInfluencersRequest,
    IRequest<GetInfluencersRequestParameters>,
    IRequest
  {
    protected IGetInfluencersRequest Self => (IGetInfluencersRequest) this;

    internal override ApiUrls ApiUrls => ApiUrlsLookups.MachineLearningGetInfluencers;

    public GetInfluencersRequest(Id jobId)
      : base((Func<RouteValues, RouteValues>) (r => r.Required("job_id", (IUrlParameter) jobId)))
    {
    }

    [SerializationConstructor]
    protected GetInfluencersRequest()
    {
    }

    [IgnoreDataMember]
    Id IGetInfluencersRequest.JobId => this.Self.RouteValues.Get<Id>("job_id");

    public bool? Descending { get; set; }

    public DateTimeOffset? End { get; set; }

    public bool? ExcludeInterim { get; set; }

    public double? InfluencerScore { get; set; }

    public IPage Page { get; set; }

    public Field Sort { get; set; }

    public DateTimeOffset? Start { get; set; }
  }
}
