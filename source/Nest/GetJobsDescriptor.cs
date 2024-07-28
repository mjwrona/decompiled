// Decompiled with JetBrains decompiler
// Type: Nest.GetJobsDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.MachineLearningApi;
using System;

namespace Nest
{
  public class GetJobsDescriptor : 
    RequestDescriptorBase<GetJobsDescriptor, GetJobsRequestParameters, IGetJobsRequest>,
    IGetJobsRequest,
    IRequest<GetJobsRequestParameters>,
    IRequest
  {
    internal override ApiUrls ApiUrls => ApiUrlsLookups.MachineLearningGetJobs;

    public GetJobsDescriptor(Id jobId)
      : base((Func<RouteValues, RouteValues>) (r => r.Optional("job_id", (IUrlParameter) jobId)))
    {
    }

    public GetJobsDescriptor()
    {
    }

    Id IGetJobsRequest.JobId => this.Self.RouteValues.Get<Id>("job_id");

    public GetJobsDescriptor JobId(Id jobId) => this.Assign<Id>(jobId, (Action<IGetJobsRequest, Id>) ((a, v) => a.RouteValues.Optional("job_id", (IUrlParameter) v)));

    [Obsolete("Scheduled to be removed in 8.0, deprecated")]
    public GetJobsDescriptor AllowNoJobs(bool? allownojobs = true) => this.Qs("allow_no_jobs", (object) allownojobs);

    public GetJobsDescriptor AllowNoMatch(bool? allownomatch = true) => this.Qs("allow_no_match", (object) allownomatch);

    public GetJobsDescriptor ExcludeGenerated(bool? excludegenerated = true) => this.Qs("exclude_generated", (object) excludegenerated);
  }
}
