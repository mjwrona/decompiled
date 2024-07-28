// Decompiled with JetBrains decompiler
// Type: Nest.DeletePipelineDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.IngestApi;
using Elasticsearch.Net.Utf8Json;
using System;

namespace Nest
{
  public class DeletePipelineDescriptor : 
    RequestDescriptorBase<DeletePipelineDescriptor, DeletePipelineRequestParameters, IDeletePipelineRequest>,
    IDeletePipelineRequest,
    IRequest<DeletePipelineRequestParameters>,
    IRequest
  {
    internal override ApiUrls ApiUrls => ApiUrlsLookups.IngestDeletePipeline;

    public DeletePipelineDescriptor(Id id)
      : base((Func<RouteValues, RouteValues>) (r => r.Required(nameof (id), (IUrlParameter) id)))
    {
    }

    [SerializationConstructor]
    protected DeletePipelineDescriptor()
    {
    }

    Id IDeletePipelineRequest.Id => this.Self.RouteValues.Get<Id>("id");

    public DeletePipelineDescriptor MasterTimeout(Time mastertimeout) => this.Qs("master_timeout", (object) mastertimeout);

    public DeletePipelineDescriptor Timeout(Time timeout) => this.Qs(nameof (timeout), (object) timeout);
  }
}
