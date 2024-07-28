// Decompiled with JetBrains decompiler
// Type: Nest.GetJobStatsDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.MachineLearningApi;
using System;

namespace Nest
{
  public class GetJobStatsDescriptor : 
    RequestDescriptorBase<GetJobStatsDescriptor, GetJobStatsRequestParameters, IGetJobStatsRequest>,
    IGetJobStatsRequest,
    IRequest<GetJobStatsRequestParameters>,
    IRequest
  {
    internal override ApiUrls ApiUrls => ApiUrlsLookups.MachineLearningGetJobStats;

    public GetJobStatsDescriptor()
    {
    }

    public GetJobStatsDescriptor(Id jobId)
      : base((Func<RouteValues, RouteValues>) (r => r.Optional("job_id", (IUrlParameter) jobId)))
    {
    }

    Id IGetJobStatsRequest.JobId => this.Self.RouteValues.Get<Id>("job_id");

    public GetJobStatsDescriptor JobId(Id jobId) => this.Assign<Id>(jobId, (Action<IGetJobStatsRequest, Id>) ((a, v) => a.RouteValues.Optional("job_id", (IUrlParameter) v)));

    [Obsolete("Scheduled to be removed in 8.0, deprecated")]
    public GetJobStatsDescriptor AllowNoJobs(bool? allownojobs = true) => this.Qs("allow_no_jobs", (object) allownojobs);

    public GetJobStatsDescriptor AllowNoMatch(bool? allownomatch = true) => this.Qs("allow_no_match", (object) allownomatch);
  }
}
