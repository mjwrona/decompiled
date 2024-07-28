// Decompiled with JetBrains decompiler
// Type: Nest.GetBucketsDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.MachineLearningApi;
using Elasticsearch.Net.Utf8Json;
using System;

namespace Nest
{
  public class GetBucketsDescriptor : 
    RequestDescriptorBase<GetBucketsDescriptor, GetBucketsRequestParameters, IGetBucketsRequest>,
    IGetBucketsRequest,
    IRequest<GetBucketsRequestParameters>,
    IRequest
  {
    internal override ApiUrls ApiUrls => ApiUrlsLookups.MachineLearningGetBuckets;

    public GetBucketsDescriptor(Id jobId, Nest.Timestamp timestamp)
      : base((Func<RouteValues, RouteValues>) (r => r.Required("job_id", (IUrlParameter) jobId).Optional(nameof (timestamp), (IUrlParameter) timestamp)))
    {
    }

    public GetBucketsDescriptor(Id jobId)
      : base((Func<RouteValues, RouteValues>) (r => r.Required("job_id", (IUrlParameter) jobId)))
    {
    }

    [SerializationConstructor]
    protected GetBucketsDescriptor()
    {
    }

    Id IGetBucketsRequest.JobId => this.Self.RouteValues.Get<Id>("job_id");

    Nest.Timestamp IGetBucketsRequest.Timestamp => this.Self.RouteValues.Get<Nest.Timestamp>("timestamp");

    public GetBucketsDescriptor Timestamp(Nest.Timestamp timestamp) => this.Assign<Nest.Timestamp>(timestamp, (Action<IGetBucketsRequest, Nest.Timestamp>) ((a, v) => a.RouteValues.Optional(nameof (timestamp), (IUrlParameter) v)));

    double? IGetBucketsRequest.AnomalyScore { get; set; }

    bool? IGetBucketsRequest.Descending { get; set; }

    DateTimeOffset? IGetBucketsRequest.End { get; set; }

    bool? IGetBucketsRequest.ExcludeInterim { get; set; }

    bool? IGetBucketsRequest.Expand { get; set; }

    IPage IGetBucketsRequest.Page { get; set; }

    Field IGetBucketsRequest.Sort { get; set; }

    DateTimeOffset? IGetBucketsRequest.Start { get; set; }

    public GetBucketsDescriptor AnomalyScore(double? anomalyScore) => this.Assign<double?>(anomalyScore, (Action<IGetBucketsRequest, double?>) ((a, v) => a.AnomalyScore = v));

    public GetBucketsDescriptor Descending(bool? descending = true) => this.Assign<bool?>(descending, (Action<IGetBucketsRequest, bool?>) ((a, v) => a.Descending = v));

    public GetBucketsDescriptor End(DateTimeOffset? end) => this.Assign<DateTimeOffset?>(end, (Action<IGetBucketsRequest, DateTimeOffset?>) ((a, v) => a.End = v));

    public GetBucketsDescriptor ExcludeInterim(bool? excludeInterim = true) => this.Assign<bool?>(excludeInterim, (Action<IGetBucketsRequest, bool?>) ((a, v) => a.ExcludeInterim = v));

    public GetBucketsDescriptor Expand(bool? expand = true) => this.Assign<bool?>(expand, (Action<IGetBucketsRequest, bool?>) ((a, v) => a.Expand = v));

    public GetBucketsDescriptor Page(Func<PageDescriptor, IPage> selector) => this.Assign<Func<PageDescriptor, IPage>>(selector, (Action<IGetBucketsRequest, Func<PageDescriptor, IPage>>) ((a, v) => a.Page = v != null ? v(new PageDescriptor()) : (IPage) null));

    public GetBucketsDescriptor Sort(Field field) => this.Assign<Field>(field, (Action<IGetBucketsRequest, Field>) ((a, v) => a.Sort = v));

    public GetBucketsDescriptor Start(DateTimeOffset? start) => this.Assign<DateTimeOffset?>(start, (Action<IGetBucketsRequest, DateTimeOffset?>) ((a, v) => a.Start = v));
  }
}
