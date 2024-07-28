// Decompiled with JetBrains decompiler
// Type: Nest.CloseJobDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.MachineLearningApi;
using Elasticsearch.Net.Utf8Json;
using System;

namespace Nest
{
  public class CloseJobDescriptor : 
    RequestDescriptorBase<CloseJobDescriptor, CloseJobRequestParameters, ICloseJobRequest>,
    ICloseJobRequest,
    IRequest<CloseJobRequestParameters>,
    IRequest
  {
    internal override ApiUrls ApiUrls => ApiUrlsLookups.MachineLearningCloseJob;

    public CloseJobDescriptor(Id jobId)
      : base((Func<RouteValues, RouteValues>) (r => r.Required("job_id", (IUrlParameter) jobId)))
    {
    }

    [SerializationConstructor]
    protected CloseJobDescriptor()
    {
    }

    Id ICloseJobRequest.JobId => this.Self.RouteValues.Get<Id>("job_id");

    [Obsolete("Scheduled to be removed in 8.0, deprecated")]
    public CloseJobDescriptor AllowNoJobs(bool? allownojobs = true) => this.Qs("allow_no_jobs", (object) allownojobs);

    public CloseJobDescriptor AllowNoMatch(bool? allownomatch = true) => this.Qs("allow_no_match", (object) allownomatch);

    public CloseJobDescriptor Force(bool? force = true) => this.Qs(nameof (force), (object) force);

    public CloseJobDescriptor Timeout(Time timeout) => this.Qs(nameof (timeout), (object) timeout);
  }
}
