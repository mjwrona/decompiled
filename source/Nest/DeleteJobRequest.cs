// Decompiled with JetBrains decompiler
// Type: Nest.DeleteJobRequest
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
  public class DeleteJobRequest : 
    PlainRequestBase<DeleteJobRequestParameters>,
    IDeleteJobRequest,
    IRequest<DeleteJobRequestParameters>,
    IRequest
  {
    protected IDeleteJobRequest Self => (IDeleteJobRequest) this;

    internal override ApiUrls ApiUrls => ApiUrlsLookups.MachineLearningDeleteJob;

    public DeleteJobRequest(Id jobId)
      : base((Func<RouteValues, RouteValues>) (r => r.Required("job_id", (IUrlParameter) jobId)))
    {
    }

    [SerializationConstructor]
    protected DeleteJobRequest()
    {
    }

    [IgnoreDataMember]
    Id IDeleteJobRequest.JobId => this.Self.RouteValues.Get<Id>("job_id");

    public bool? Force
    {
      get => this.Q<bool?>("force");
      set => this.Q("force", (object) value);
    }

    public bool? WaitForCompletion
    {
      get => this.Q<bool?>("wait_for_completion");
      set => this.Q("wait_for_completion", (object) value);
    }
  }
}
