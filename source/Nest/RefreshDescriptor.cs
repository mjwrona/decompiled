// Decompiled with JetBrains decompiler
// Type: Nest.RefreshDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.IndicesApi;
using System;

namespace Nest
{
  public class RefreshDescriptor : 
    RequestDescriptorBase<RefreshDescriptor, RefreshRequestParameters, IRefreshRequest>,
    IRefreshRequest,
    IRequest<RefreshRequestParameters>,
    IRequest
  {
    internal override ApiUrls ApiUrls => ApiUrlsLookups.IndicesRefresh;

    public RefreshDescriptor()
    {
    }

    public RefreshDescriptor(Indices index)
      : base((Func<RouteValues, RouteValues>) (r => r.Optional(nameof (index), (IUrlParameter) index)))
    {
    }

    Indices IRefreshRequest.Index => this.Self.RouteValues.Get<Indices>("index");

    public RefreshDescriptor Index(Indices index) => this.Assign<Indices>(index, (Action<IRefreshRequest, Indices>) ((a, v) => a.RouteValues.Optional(nameof (index), (IUrlParameter) v)));

    public RefreshDescriptor Index<TOther>() where TOther : class => this.Assign<Type>(typeof (TOther), (Action<IRefreshRequest, Type>) ((a, v) => a.RouteValues.Optional("index", (IUrlParameter) (Indices) v)));

    public RefreshDescriptor AllIndices() => this.Index(Indices.All);

    public RefreshDescriptor AllowNoIndices(bool? allownoindices = true) => this.Qs("allow_no_indices", (object) allownoindices);

    public RefreshDescriptor ExpandWildcards(Elasticsearch.Net.ExpandWildcards? expandwildcards) => this.Qs("expand_wildcards", (object) expandwildcards);

    public RefreshDescriptor IgnoreUnavailable(bool? ignoreunavailable = true) => this.Qs("ignore_unavailable", (object) ignoreunavailable);
  }
}
