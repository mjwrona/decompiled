// Decompiled with JetBrains decompiler
// Type: Nest.SimulatePipelineRequest
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.IngestApi;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Nest
{
  public class SimulatePipelineRequest : 
    PlainRequestBase<SimulatePipelineRequestParameters>,
    ISimulatePipelineRequest,
    IRequest<SimulatePipelineRequestParameters>,
    IRequest
  {
    public IEnumerable<ISimulatePipelineDocument> Documents { get; set; }

    public IPipeline Pipeline { get; set; }

    protected ISimulatePipelineRequest Self => (ISimulatePipelineRequest) this;

    internal override ApiUrls ApiUrls => ApiUrlsLookups.IngestSimulatePipeline;

    public SimulatePipelineRequest()
    {
    }

    public SimulatePipelineRequest(Id id)
      : base((Func<RouteValues, RouteValues>) (r => r.Optional(nameof (id), (IUrlParameter) id)))
    {
    }

    [IgnoreDataMember]
    Id ISimulatePipelineRequest.Id => this.Self.RouteValues.Get<Id>("id");

    public bool? Verbose
    {
      get => this.Q<bool?>("verbose");
      set => this.Q("verbose", (object) value);
    }
  }
}
