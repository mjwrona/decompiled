// Decompiled with JetBrains decompiler
// Type: Nest.FlushDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.IndicesApi;
using System;

namespace Nest
{
  public class FlushDescriptor : 
    RequestDescriptorBase<FlushDescriptor, FlushRequestParameters, IFlushRequest>,
    IFlushRequest,
    IRequest<FlushRequestParameters>,
    IRequest
  {
    internal override ApiUrls ApiUrls => ApiUrlsLookups.IndicesFlush;

    public FlushDescriptor()
    {
    }

    public FlushDescriptor(Indices index)
      : base((Func<RouteValues, RouteValues>) (r => r.Optional(nameof (index), (IUrlParameter) index)))
    {
    }

    Indices IFlushRequest.Index => this.Self.RouteValues.Get<Indices>("index");

    public FlushDescriptor Index(Indices index) => this.Assign<Indices>(index, (Action<IFlushRequest, Indices>) ((a, v) => a.RouteValues.Optional(nameof (index), (IUrlParameter) v)));

    public FlushDescriptor Index<TOther>() where TOther : class => this.Assign<Type>(typeof (TOther), (Action<IFlushRequest, Type>) ((a, v) => a.RouteValues.Optional("index", (IUrlParameter) (Indices) v)));

    public FlushDescriptor AllIndices() => this.Index(Indices.All);

    public FlushDescriptor AllowNoIndices(bool? allownoindices = true) => this.Qs("allow_no_indices", (object) allownoindices);

    public FlushDescriptor ExpandWildcards(Elasticsearch.Net.ExpandWildcards? expandwildcards) => this.Qs("expand_wildcards", (object) expandwildcards);

    public FlushDescriptor Force(bool? force = true) => this.Qs(nameof (force), (object) force);

    public FlushDescriptor IgnoreUnavailable(bool? ignoreunavailable = true) => this.Qs("ignore_unavailable", (object) ignoreunavailable);

    public FlushDescriptor WaitIfOngoing(bool? waitifongoing = true) => this.Qs("wait_if_ongoing", (object) waitifongoing);
  }
}
