// Decompiled with JetBrains decompiler
// Type: Nest.DeleteJobDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.MachineLearningApi;
using Elasticsearch.Net.Utf8Json;
using System;

namespace Nest
{
  public class DeleteJobDescriptor : 
    RequestDescriptorBase<DeleteJobDescriptor, DeleteJobRequestParameters, IDeleteJobRequest>,
    IDeleteJobRequest,
    IRequest<DeleteJobRequestParameters>,
    IRequest
  {
    internal override ApiUrls ApiUrls => ApiUrlsLookups.MachineLearningDeleteJob;

    public DeleteJobDescriptor(Id jobId)
      : base((Func<RouteValues, RouteValues>) (r => r.Required("job_id", (IUrlParameter) jobId)))
    {
    }

    [SerializationConstructor]
    protected DeleteJobDescriptor()
    {
    }

    Id IDeleteJobRequest.JobId => this.Self.RouteValues.Get<Id>("job_id");

    public DeleteJobDescriptor Force(bool? force = true) => this.Qs(nameof (force), (object) force);

    public DeleteJobDescriptor WaitForCompletion(bool? waitforcompletion = true) => this.Qs("wait_for_completion", (object) waitforcompletion);
  }
}
