// Decompiled with JetBrains decompiler
// Type: Nest.GetJobStatsRequest
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.MachineLearningApi;
using System;
using System.Runtime.Serialization;

namespace Nest
{
  public class GetJobStatsRequest : 
    PlainRequestBase<GetJobStatsRequestParameters>,
    IGetJobStatsRequest,
    IRequest<GetJobStatsRequestParameters>,
    IRequest
  {
    protected IGetJobStatsRequest Self => (IGetJobStatsRequest) this;

    internal override ApiUrls ApiUrls => ApiUrlsLookups.MachineLearningGetJobStats;

    public GetJobStatsRequest()
    {
    }

    public GetJobStatsRequest(Id jobId)
      : base((Func<RouteValues, RouteValues>) (r => r.Optional("job_id", (IUrlParameter) jobId)))
    {
    }

    [IgnoreDataMember]
    Id IGetJobStatsRequest.JobId => this.Self.RouteValues.Get<Id>("job_id");

    [Obsolete("Scheduled to be removed in 8.0, deprecated")]
    public bool? AllowNoJobs
    {
      get => this.Q<bool?>("allow_no_jobs");
      set => this.Q("allow_no_jobs", (object) value);
    }

    public bool? AllowNoMatch
    {
      get => this.Q<bool?>("allow_no_match");
      set => this.Q("allow_no_match", (object) value);
    }
  }
}
