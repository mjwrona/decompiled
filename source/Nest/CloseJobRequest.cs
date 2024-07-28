// Decompiled with JetBrains decompiler
// Type: Nest.CloseJobRequest
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
  public class CloseJobRequest : 
    PlainRequestBase<CloseJobRequestParameters>,
    ICloseJobRequest,
    IRequest<CloseJobRequestParameters>,
    IRequest
  {
    protected ICloseJobRequest Self => (ICloseJobRequest) this;

    internal override ApiUrls ApiUrls => ApiUrlsLookups.MachineLearningCloseJob;

    public CloseJobRequest(Id jobId)
      : base((Func<RouteValues, RouteValues>) (r => r.Required("job_id", (IUrlParameter) jobId)))
    {
    }

    [SerializationConstructor]
    protected CloseJobRequest()
    {
    }

    [IgnoreDataMember]
    Id ICloseJobRequest.JobId => this.Self.RouteValues.Get<Id>("job_id");

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

    public bool? Force
    {
      get => this.Q<bool?>("force");
      set => this.Q("force", (object) value);
    }

    public Time Timeout
    {
      get => this.Q<Time>("timeout");
      set => this.Q("timeout", (object) value);
    }
  }
}
