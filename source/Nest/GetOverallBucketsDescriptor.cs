// Decompiled with JetBrains decompiler
// Type: Nest.GetOverallBucketsDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.MachineLearningApi;
using Elasticsearch.Net.Utf8Json;
using System;

namespace Nest
{
  public class GetOverallBucketsDescriptor : 
    RequestDescriptorBase<GetOverallBucketsDescriptor, GetOverallBucketsRequestParameters, IGetOverallBucketsRequest>,
    IGetOverallBucketsRequest,
    IRequest<GetOverallBucketsRequestParameters>,
    IRequest
  {
    internal override ApiUrls ApiUrls => ApiUrlsLookups.MachineLearningGetOverallBuckets;

    public GetOverallBucketsDescriptor(Id jobId)
      : base((Func<RouteValues, RouteValues>) (r => r.Required("job_id", (IUrlParameter) jobId)))
    {
    }

    [SerializationConstructor]
    protected GetOverallBucketsDescriptor()
    {
    }

    Id IGetOverallBucketsRequest.JobId => this.Self.RouteValues.Get<Id>("job_id");

    public GetOverallBucketsDescriptor AllowNoMatch(bool? allownomatch = true) => this.Qs("allow_no_match", (object) allownomatch);

    bool? IGetOverallBucketsRequest.AllowNoJobs { get; set; }

    Time IGetOverallBucketsRequest.BucketSpan { get; set; }

    DateTimeOffset? IGetOverallBucketsRequest.End { get; set; }

    bool? IGetOverallBucketsRequest.ExcludeInterim { get; set; }

    double? IGetOverallBucketsRequest.OverallScore { get; set; }

    DateTimeOffset? IGetOverallBucketsRequest.Start { get; set; }

    int? IGetOverallBucketsRequest.TopN { get; set; }

    public GetOverallBucketsDescriptor AllowNoJobs(bool? allowNoJobs = true) => this.Assign<bool?>(allowNoJobs, (Action<IGetOverallBucketsRequest, bool?>) ((a, v) => a.AllowNoJobs = v));

    public GetOverallBucketsDescriptor BucketSpan(Time bucketSpan) => this.Assign<Time>(bucketSpan, (Action<IGetOverallBucketsRequest, Time>) ((a, v) => a.BucketSpan = v));

    public GetOverallBucketsDescriptor End(DateTimeOffset? end) => this.Assign<DateTimeOffset?>(end, (Action<IGetOverallBucketsRequest, DateTimeOffset?>) ((a, v) => a.End = v));

    public GetOverallBucketsDescriptor ExcludeInterim(bool? excludeInterim = true) => this.Assign<bool?>(excludeInterim, (Action<IGetOverallBucketsRequest, bool?>) ((a, v) => a.ExcludeInterim = v));

    public GetOverallBucketsDescriptor OverallScore(double? overallScore) => this.Assign<double?>(overallScore, (Action<IGetOverallBucketsRequest, double?>) ((a, v) => a.OverallScore = v));

    public GetOverallBucketsDescriptor Start(DateTimeOffset? start) => this.Assign<DateTimeOffset?>(start, (Action<IGetOverallBucketsRequest, DateTimeOffset?>) ((a, v) => a.Start = v));

    public GetOverallBucketsDescriptor TopN(int? topN) => this.Assign<int?>(topN, (Action<IGetOverallBucketsRequest, int?>) ((a, v) => a.TopN = v));
  }
}
