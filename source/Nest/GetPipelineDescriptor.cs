// Decompiled with JetBrains decompiler
// Type: Nest.GetPipelineDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.IngestApi;
using System;

namespace Nest
{
  public class GetPipelineDescriptor : 
    RequestDescriptorBase<GetPipelineDescriptor, GetPipelineRequestParameters, IGetPipelineRequest>,
    IGetPipelineRequest,
    IRequest<GetPipelineRequestParameters>,
    IRequest
  {
    internal override ApiUrls ApiUrls => ApiUrlsLookups.IngestGetPipeline;

    public GetPipelineDescriptor()
    {
    }

    public GetPipelineDescriptor(Nest.Id id)
      : base((Func<RouteValues, RouteValues>) (r => r.Optional(nameof (id), (IUrlParameter) id)))
    {
    }

    Nest.Id IGetPipelineRequest.Id => this.Self.RouteValues.Get<Nest.Id>("id");

    public GetPipelineDescriptor Id(Nest.Id id) => this.Assign<Nest.Id>(id, (Action<IGetPipelineRequest, Nest.Id>) ((a, v) => a.RouteValues.Optional(nameof (id), (IUrlParameter) v)));

    public GetPipelineDescriptor MasterTimeout(Time mastertimeout) => this.Qs("master_timeout", (object) mastertimeout);

    public GetPipelineDescriptor Summary(bool? summary = true) => this.Qs(nameof (summary), (object) summary);
  }
}
