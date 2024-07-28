// Decompiled with JetBrains decompiler
// Type: Nest.GetPipelineRequest
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.IngestApi;
using System;
using System.Runtime.Serialization;

namespace Nest
{
  public class GetPipelineRequest : 
    PlainRequestBase<GetPipelineRequestParameters>,
    IGetPipelineRequest,
    IRequest<GetPipelineRequestParameters>,
    IRequest
  {
    protected IGetPipelineRequest Self => (IGetPipelineRequest) this;

    internal override ApiUrls ApiUrls => ApiUrlsLookups.IngestGetPipeline;

    public GetPipelineRequest()
    {
    }

    public GetPipelineRequest(Id id)
      : base((Func<RouteValues, RouteValues>) (r => r.Optional(nameof (id), (IUrlParameter) id)))
    {
    }

    [IgnoreDataMember]
    Id IGetPipelineRequest.Id => this.Self.RouteValues.Get<Id>("id");

    public Time MasterTimeout
    {
      get => this.Q<Time>("master_timeout");
      set => this.Q("master_timeout", (object) value);
    }

    public bool? Summary
    {
      get => this.Q<bool?>("summary");
      set => this.Q("summary", (object) value);
    }
  }
}
