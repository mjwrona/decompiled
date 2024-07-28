// Decompiled with JetBrains decompiler
// Type: Nest.SearchShardsDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using System;

namespace Nest
{
  public class SearchShardsDescriptor<TDocument> : 
    RequestDescriptorBase<SearchShardsDescriptor<TDocument>, SearchShardsRequestParameters, ISearchShardsRequest<TDocument>>,
    ISearchShardsRequest<TDocument>,
    ISearchShardsRequest,
    IRequest<SearchShardsRequestParameters>,
    IRequest
    where TDocument : class
  {
    internal override ApiUrls ApiUrls => ApiUrlsLookups.NoNamespaceSearchShards;

    public SearchShardsDescriptor()
      : this((Indices) typeof (TDocument))
    {
    }

    public SearchShardsDescriptor(Indices index)
      : base((Func<RouteValues, RouteValues>) (r => r.Optional(nameof (index), (IUrlParameter) index)))
    {
    }

    Indices ISearchShardsRequest.Index => this.Self.RouteValues.Get<Indices>("index");

    public SearchShardsDescriptor<TDocument> Index(Indices index) => this.Assign<Indices>(index, (Action<ISearchShardsRequest<TDocument>, Indices>) ((a, v) => a.RouteValues.Optional(nameof (index), (IUrlParameter) v)));

    public SearchShardsDescriptor<TDocument> Index<TOther>() where TOther : class => this.Assign<Type>(typeof (TOther), (Action<ISearchShardsRequest<TDocument>, Type>) ((a, v) => a.RouteValues.Optional("index", (IUrlParameter) (Indices) v)));

    public SearchShardsDescriptor<TDocument> AllIndices() => this.Index(Indices.All);

    public SearchShardsDescriptor<TDocument> AllowNoIndices(bool? allownoindices = true) => this.Qs("allow_no_indices", (object) allownoindices);

    public SearchShardsDescriptor<TDocument> ExpandWildcards(Elasticsearch.Net.ExpandWildcards? expandwildcards) => this.Qs("expand_wildcards", (object) expandwildcards);

    public SearchShardsDescriptor<TDocument> IgnoreUnavailable(bool? ignoreunavailable = true) => this.Qs("ignore_unavailable", (object) ignoreunavailable);

    public SearchShardsDescriptor<TDocument> Local(bool? local = true) => this.Qs(nameof (local), (object) local);

    public SearchShardsDescriptor<TDocument> Preference(string preference) => this.Qs(nameof (preference), (object) preference);

    public SearchShardsDescriptor<TDocument> Routing(Nest.Routing routing) => this.Qs(nameof (routing), (object) routing);
  }
}
