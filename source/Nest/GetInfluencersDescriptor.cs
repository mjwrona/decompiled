// Decompiled with JetBrains decompiler
// Type: Nest.GetInfluencersDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.MachineLearningApi;
using Elasticsearch.Net.Utf8Json;
using System;

namespace Nest
{
  public class GetInfluencersDescriptor : 
    RequestDescriptorBase<GetInfluencersDescriptor, GetInfluencersRequestParameters, IGetInfluencersRequest>,
    IGetInfluencersRequest,
    IRequest<GetInfluencersRequestParameters>,
    IRequest
  {
    internal override ApiUrls ApiUrls => ApiUrlsLookups.MachineLearningGetInfluencers;

    public GetInfluencersDescriptor(Id jobId)
      : base((Func<RouteValues, RouteValues>) (r => r.Required("job_id", (IUrlParameter) jobId)))
    {
    }

    [SerializationConstructor]
    protected GetInfluencersDescriptor()
    {
    }

    Id IGetInfluencersRequest.JobId => this.Self.RouteValues.Get<Id>("job_id");

    bool? IGetInfluencersRequest.Descending { get; set; }

    DateTimeOffset? IGetInfluencersRequest.End { get; set; }

    bool? IGetInfluencersRequest.ExcludeInterim { get; set; }

    double? IGetInfluencersRequest.InfluencerScore { get; set; }

    IPage IGetInfluencersRequest.Page { get; set; }

    Field IGetInfluencersRequest.Sort { get; set; }

    DateTimeOffset? IGetInfluencersRequest.Start { get; set; }

    public GetInfluencersDescriptor InfluencerScore(double? influencerScore) => this.Assign<double?>(influencerScore, (Action<IGetInfluencersRequest, double?>) ((a, v) => a.InfluencerScore = v));

    public GetInfluencersDescriptor Desc(bool? descending = true) => this.Assign<bool?>(descending, (Action<IGetInfluencersRequest, bool?>) ((a, v) => a.Descending = v));

    public GetInfluencersDescriptor End(DateTimeOffset? end) => this.Assign<DateTimeOffset?>(end, (Action<IGetInfluencersRequest, DateTimeOffset?>) ((a, v) => a.End = v));

    public GetInfluencersDescriptor ExcludeInterim(bool? excludeInterim = true) => this.Assign<bool?>(excludeInterim, (Action<IGetInfluencersRequest, bool?>) ((a, v) => a.ExcludeInterim = v));

    public GetInfluencersDescriptor Page(Func<PageDescriptor, IPage> selector) => this.Assign<Func<PageDescriptor, IPage>>(selector, (Action<IGetInfluencersRequest, Func<PageDescriptor, IPage>>) ((a, v) => a.Page = v != null ? v(new PageDescriptor()) : (IPage) null));

    public GetInfluencersDescriptor Sort(Field field) => this.Assign<Field>(field, (Action<IGetInfluencersRequest, Field>) ((a, v) => a.Sort = v));

    public GetInfluencersDescriptor Start(DateTimeOffset? end) => this.Assign<DateTimeOffset?>(end, (Action<IGetInfluencersRequest, DateTimeOffset?>) ((a, v) => a.Start = v));
  }
}
