// Decompiled with JetBrains decompiler
// Type: Nest.GraphExploreDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.GraphApi;
using System;
using System.Collections.Generic;

namespace Nest
{
  public class GraphExploreDescriptor<TDocument> : 
    RequestDescriptorBase<GraphExploreDescriptor<TDocument>, GraphExploreRequestParameters, IGraphExploreRequest<TDocument>>,
    IGraphExploreRequest<TDocument>,
    IGraphExploreRequest,
    IRequest<GraphExploreRequestParameters>,
    IRequest,
    IHop
    where TDocument : class
  {
    internal override ApiUrls ApiUrls => ApiUrlsLookups.GraphExplore;

    public GraphExploreDescriptor(Indices index)
      : base((Func<RouteValues, RouteValues>) (r => r.Required(nameof (index), (IUrlParameter) index)))
    {
    }

    public GraphExploreDescriptor()
      : this((Indices) typeof (TDocument))
    {
    }

    Indices IGraphExploreRequest.Index => this.Self.RouteValues.Get<Indices>("index");

    public GraphExploreDescriptor<TDocument> Index(Indices index) => this.Assign<Indices>(index, (Action<IGraphExploreRequest<TDocument>, Indices>) ((a, v) => a.RouteValues.Required(nameof (index), (IUrlParameter) v)));

    public GraphExploreDescriptor<TDocument> Index<TOther>() where TOther : class => this.Assign<Type>(typeof (TOther), (Action<IGraphExploreRequest<TDocument>, Type>) ((a, v) => a.RouteValues.Required("index", (IUrlParameter) (Indices) v)));

    public GraphExploreDescriptor<TDocument> AllIndices() => this.Index(Indices.All);

    public GraphExploreDescriptor<TDocument> Routing(Nest.Routing routing) => this.Qs(nameof (routing), (object) routing);

    public GraphExploreDescriptor<TDocument> Timeout(Time timeout) => this.Qs(nameof (timeout), (object) timeout);

    IHop IHop.Connections { get; set; }

    IGraphExploreControls IGraphExploreRequest.Controls { get; set; }

    QueryContainer IHop.Query { get; set; }

    IEnumerable<IGraphVertexDefinition> IHop.Vertices { get; set; }

    public GraphExploreDescriptor<TDocument> Query(
      Func<QueryContainerDescriptor<TDocument>, QueryContainer> querySelector)
    {
      return this.Assign<Func<QueryContainerDescriptor<TDocument>, QueryContainer>>(querySelector, (Action<IGraphExploreRequest<TDocument>, Func<QueryContainerDescriptor<TDocument>, QueryContainer>>) ((a, v) => a.Query = v != null ? v(new QueryContainerDescriptor<TDocument>()) : (QueryContainer) null));
    }

    public GraphExploreDescriptor<TDocument> Vertices(
      Func<GraphVerticesDescriptor<TDocument>, IPromise<IList<IGraphVertexDefinition>>> selector)
    {
      return this.Assign<Func<GraphVerticesDescriptor<TDocument>, IPromise<IList<IGraphVertexDefinition>>>>(selector, (Action<IGraphExploreRequest<TDocument>, Func<GraphVerticesDescriptor<TDocument>, IPromise<IList<IGraphVertexDefinition>>>>) ((a, v) => a.Vertices = v != null ? (IEnumerable<IGraphVertexDefinition>) v(new GraphVerticesDescriptor<TDocument>())?.Value : (IEnumerable<IGraphVertexDefinition>) null));
    }

    public GraphExploreDescriptor<TDocument> Connections(
      Func<HopDescriptor<TDocument>, IHop> selector)
    {
      return this.Assign<Func<HopDescriptor<TDocument>, IHop>>(selector, (Action<IGraphExploreRequest<TDocument>, Func<HopDescriptor<TDocument>, IHop>>) ((a, v) => a.Connections = v != null ? v(new HopDescriptor<TDocument>()) : (IHop) null));
    }

    public GraphExploreDescriptor<TDocument> Controls(
      Func<GraphExploreControlsDescriptor<TDocument>, IGraphExploreControls> selector)
    {
      return this.Assign<Func<GraphExploreControlsDescriptor<TDocument>, IGraphExploreControls>>(selector, (Action<IGraphExploreRequest<TDocument>, Func<GraphExploreControlsDescriptor<TDocument>, IGraphExploreControls>>) ((a, v) => a.Controls = v != null ? v(new GraphExploreControlsDescriptor<TDocument>()) : (IGraphExploreControls) null));
    }
  }
}
