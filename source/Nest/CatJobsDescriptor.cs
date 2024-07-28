// Decompiled with JetBrains decompiler
// Type: Nest.CatJobsDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.CatApi;
using System;

namespace Nest
{
  public class CatJobsDescriptor : 
    RequestDescriptorBase<CatJobsDescriptor, CatJobsRequestParameters, ICatJobsRequest>,
    ICatJobsRequest,
    IRequest<CatJobsRequestParameters>,
    IRequest
  {
    internal override ApiUrls ApiUrls => ApiUrlsLookups.CatJobs;

    public CatJobsDescriptor()
    {
    }

    public CatJobsDescriptor(Id jobId)
      : base((Func<RouteValues, RouteValues>) (r => r.Optional("job_id", (IUrlParameter) jobId)))
    {
    }

    Id ICatJobsRequest.JobId => this.Self.RouteValues.Get<Id>("job_id");

    public CatJobsDescriptor JobId(Id jobId) => this.Assign<Id>(jobId, (Action<ICatJobsRequest, Id>) ((a, v) => a.RouteValues.Optional("job_id", (IUrlParameter) v)));

    [Obsolete("Scheduled to be removed in 8.0, deprecated")]
    public CatJobsDescriptor AllowNoJobs(bool? allownojobs = true) => this.Qs("allow_no_jobs", (object) allownojobs);

    public CatJobsDescriptor AllowNoMatch(bool? allownomatch = true) => this.Qs("allow_no_match", (object) allownomatch);

    public CatJobsDescriptor Bytes(Elasticsearch.Net.Bytes? bytes) => this.Qs(nameof (bytes), (object) bytes);

    public CatJobsDescriptor Format(string format) => this.Qs(nameof (format), (object) format);

    public CatJobsDescriptor Headers(params string[] headers) => this.Qs("h", (object) headers);

    public CatJobsDescriptor Help(bool? help = true) => this.Qs(nameof (help), (object) help);

    public CatJobsDescriptor SortByColumns(params string[] sortbycolumns) => this.Qs("s", (object) sortbycolumns);

    public CatJobsDescriptor Verbose(bool? verbose = true) => this.Qs("v", (object) verbose);
  }
}
