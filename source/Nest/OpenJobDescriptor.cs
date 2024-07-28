// Decompiled with JetBrains decompiler
// Type: Nest.OpenJobDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.MachineLearningApi;
using Elasticsearch.Net.Utf8Json;
using System;

namespace Nest
{
  public class OpenJobDescriptor : 
    RequestDescriptorBase<OpenJobDescriptor, OpenJobRequestParameters, IOpenJobRequest>,
    IOpenJobRequest,
    IRequest<OpenJobRequestParameters>,
    IRequest
  {
    internal override ApiUrls ApiUrls => ApiUrlsLookups.MachineLearningOpenJob;

    public OpenJobDescriptor(Id jobId)
      : base((Func<RouteValues, RouteValues>) (r => r.Required("job_id", (IUrlParameter) jobId)))
    {
    }

    [SerializationConstructor]
    protected OpenJobDescriptor()
    {
    }

    Id IOpenJobRequest.JobId => this.Self.RouteValues.Get<Id>("job_id");

    Time IOpenJobRequest.Timeout { get; set; }

    public OpenJobDescriptor Timeout(Time timeout) => this.Assign<Time>(timeout, (Action<IOpenJobRequest, Time>) ((a, v) => a.Timeout = v));
  }
}
