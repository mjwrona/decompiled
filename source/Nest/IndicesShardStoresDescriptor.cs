// Decompiled with JetBrains decompiler
// Type: Nest.IndicesShardStoresDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.IndicesApi;
using System;

namespace Nest
{
  public class IndicesShardStoresDescriptor : 
    RequestDescriptorBase<IndicesShardStoresDescriptor, IndicesShardStoresRequestParameters, IIndicesShardStoresRequest>,
    IIndicesShardStoresRequest,
    IRequest<IndicesShardStoresRequestParameters>,
    IRequest
  {
    internal override ApiUrls ApiUrls => ApiUrlsLookups.IndicesShardStores;

    public IndicesShardStoresDescriptor()
    {
    }

    public IndicesShardStoresDescriptor(Indices index)
      : base((Func<RouteValues, RouteValues>) (r => r.Optional(nameof (index), (IUrlParameter) index)))
    {
    }

    Indices IIndicesShardStoresRequest.Index => this.Self.RouteValues.Get<Indices>("index");

    public IndicesShardStoresDescriptor Index(Indices index) => this.Assign<Indices>(index, (Action<IIndicesShardStoresRequest, Indices>) ((a, v) => a.RouteValues.Optional(nameof (index), (IUrlParameter) v)));

    public IndicesShardStoresDescriptor Index<TOther>() where TOther : class => this.Assign<Type>(typeof (TOther), (Action<IIndicesShardStoresRequest, Type>) ((a, v) => a.RouteValues.Optional("index", (IUrlParameter) (Indices) v)));

    public IndicesShardStoresDescriptor AllIndices() => this.Index(Indices.All);

    public IndicesShardStoresDescriptor AllowNoIndices(bool? allownoindices = true) => this.Qs("allow_no_indices", (object) allownoindices);

    public IndicesShardStoresDescriptor ExpandWildcards(Elasticsearch.Net.ExpandWildcards? expandwildcards) => this.Qs("expand_wildcards", (object) expandwildcards);

    public IndicesShardStoresDescriptor IgnoreUnavailable(bool? ignoreunavailable = true) => this.Qs("ignore_unavailable", (object) ignoreunavailable);

    public IndicesShardStoresDescriptor Status(params string[] status) => this.Qs(nameof (status), (object) status);
  }
}
