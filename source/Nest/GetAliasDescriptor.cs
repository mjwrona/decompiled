// Decompiled with JetBrains decompiler
// Type: Nest.GetAliasDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.IndicesApi;
using System;

namespace Nest
{
  public class GetAliasDescriptor : 
    RequestDescriptorBase<GetAliasDescriptor, GetAliasRequestParameters, IGetAliasRequest>,
    IGetAliasRequest,
    IRequest<GetAliasRequestParameters>,
    IRequest
  {
    internal override ApiUrls ApiUrls => ApiUrlsLookups.IndicesGetAlias;

    public GetAliasDescriptor()
    {
    }

    public GetAliasDescriptor(Names name)
      : base((Func<RouteValues, RouteValues>) (r => r.Optional(nameof (name), (IUrlParameter) name)))
    {
    }

    public GetAliasDescriptor(Indices index, Names name)
      : base((Func<RouteValues, RouteValues>) (r => r.Optional(nameof (index), (IUrlParameter) index).Optional(nameof (name), (IUrlParameter) name)))
    {
    }

    public GetAliasDescriptor(Indices index)
      : base((Func<RouteValues, RouteValues>) (r => r.Optional(nameof (index), (IUrlParameter) index)))
    {
    }

    Names IGetAliasRequest.Name => this.Self.RouteValues.Get<Names>("name");

    Indices IGetAliasRequest.Index => this.Self.RouteValues.Get<Indices>("index");

    public GetAliasDescriptor Name(Names name) => this.Assign<Names>(name, (Action<IGetAliasRequest, Names>) ((a, v) => a.RouteValues.Optional(nameof (name), (IUrlParameter) v)));

    public GetAliasDescriptor Index(Indices index) => this.Assign<Indices>(index, (Action<IGetAliasRequest, Indices>) ((a, v) => a.RouteValues.Optional(nameof (index), (IUrlParameter) v)));

    public GetAliasDescriptor Index<TOther>() where TOther : class => this.Assign<Type>(typeof (TOther), (Action<IGetAliasRequest, Type>) ((a, v) => a.RouteValues.Optional("index", (IUrlParameter) (Indices) v)));

    public GetAliasDescriptor AllIndices() => this.Index(Indices.All);

    public GetAliasDescriptor AllowNoIndices(bool? allownoindices = true) => this.Qs("allow_no_indices", (object) allownoindices);

    public GetAliasDescriptor ExpandWildcards(Elasticsearch.Net.ExpandWildcards? expandwildcards) => this.Qs("expand_wildcards", (object) expandwildcards);

    public GetAliasDescriptor IgnoreUnavailable(bool? ignoreunavailable = true) => this.Qs("ignore_unavailable", (object) ignoreunavailable);

    public GetAliasDescriptor Local(bool? local = true) => this.Qs(nameof (local), (object) local);
  }
}
