// Decompiled with JetBrains decompiler
// Type: Nest.GraphExploreRequest
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.GraphApi;
using Elasticsearch.Net.Utf8Json;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Nest
{
  public class GraphExploreRequest : 
    PlainRequestBase<GraphExploreRequestParameters>,
    IGraphExploreRequest,
    IRequest<GraphExploreRequestParameters>,
    IRequest,
    IHop
  {
    protected IGraphExploreRequest Self => (IGraphExploreRequest) this;

    internal override ApiUrls ApiUrls => ApiUrlsLookups.GraphExplore;

    public GraphExploreRequest(Indices index)
      : base((Func<RouteValues, RouteValues>) (r => r.Required(nameof (index), (IUrlParameter) index)))
    {
    }

    [SerializationConstructor]
    protected GraphExploreRequest()
    {
    }

    [IgnoreDataMember]
    Indices IGraphExploreRequest.Index => this.Self.RouteValues.Get<Indices>("index");

    public Routing Routing
    {
      get => this.Q<Routing>("routing");
      set => this.Q("routing", (object) value);
    }

    public Time Timeout
    {
      get => this.Q<Time>("timeout");
      set => this.Q("timeout", (object) value);
    }

    public IHop Connections { get; set; }

    public IGraphExploreControls Controls { get; set; }

    public QueryContainer Query { get; set; }

    public IEnumerable<IGraphVertexDefinition> Vertices { get; set; }
  }
}
