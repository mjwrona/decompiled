// Decompiled with JetBrains decompiler
// Type: Nest.OpenJobRequest
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
  public class OpenJobRequest : 
    PlainRequestBase<OpenJobRequestParameters>,
    IOpenJobRequest,
    IRequest<OpenJobRequestParameters>,
    IRequest
  {
    protected IOpenJobRequest Self => (IOpenJobRequest) this;

    internal override ApiUrls ApiUrls => ApiUrlsLookups.MachineLearningOpenJob;

    public OpenJobRequest(Id jobId)
      : base((Func<RouteValues, RouteValues>) (r => r.Required("job_id", (IUrlParameter) jobId)))
    {
    }

    [SerializationConstructor]
    protected OpenJobRequest()
    {
    }

    [IgnoreDataMember]
    Id IOpenJobRequest.JobId => this.Self.RouteValues.Get<Id>("job_id");

    public Time Timeout { get; set; }
  }
}
