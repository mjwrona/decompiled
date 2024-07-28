// Decompiled with JetBrains decompiler
// Type: Nest.GetOverallBucketsRequest
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
  public class GetOverallBucketsRequest : 
    PlainRequestBase<GetOverallBucketsRequestParameters>,
    IGetOverallBucketsRequest,
    IRequest<GetOverallBucketsRequestParameters>,
    IRequest
  {
    protected IGetOverallBucketsRequest Self => (IGetOverallBucketsRequest) this;

    internal override ApiUrls ApiUrls => ApiUrlsLookups.MachineLearningGetOverallBuckets;

    public GetOverallBucketsRequest(Id jobId)
      : base((Func<RouteValues, RouteValues>) (r => r.Required("job_id", (IUrlParameter) jobId)))
    {
    }

    [SerializationConstructor]
    protected GetOverallBucketsRequest()
    {
    }

    [IgnoreDataMember]
    Id IGetOverallBucketsRequest.JobId => this.Self.RouteValues.Get<Id>("job_id");

    public bool? AllowNoMatch
    {
      get => this.Q<bool?>("allow_no_match");
      set => this.Q("allow_no_match", (object) value);
    }

    public bool? AllowNoJobs { get; set; }

    public Time BucketSpan { get; set; }

    public DateTimeOffset? End { get; set; }

    public bool? ExcludeInterim { get; set; }

    public double? OverallScore { get; set; }

    public DateTimeOffset? Start { get; set; }

    public int? TopN { get; set; }
  }
}
