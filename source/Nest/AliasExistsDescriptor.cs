// Decompiled with JetBrains decompiler
// Type: Nest.AliasExistsDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.IndicesApi;
using Elasticsearch.Net.Utf8Json;
using System;

namespace Nest
{
  public class AliasExistsDescriptor : 
    RequestDescriptorBase<AliasExistsDescriptor, AliasExistsRequestParameters, IAliasExistsRequest>,
    IAliasExistsRequest,
    IRequest<AliasExistsRequestParameters>,
    IRequest
  {
    internal override ApiUrls ApiUrls => ApiUrlsLookups.IndicesAliasExists;

    public AliasExistsDescriptor(Names name)
      : base((Func<RouteValues, RouteValues>) (r => r.Required(nameof (name), (IUrlParameter) name)))
    {
    }

    public AliasExistsDescriptor(Indices index, Names name)
      : base((Func<RouteValues, RouteValues>) (r => r.Optional(nameof (index), (IUrlParameter) index).Required(nameof (name), (IUrlParameter) name)))
    {
    }

    [SerializationConstructor]
    protected AliasExistsDescriptor()
    {
    }

    Names IAliasExistsRequest.Name => this.Self.RouteValues.Get<Names>("name");

    Indices IAliasExistsRequest.Index => this.Self.RouteValues.Get<Indices>("index");

    public AliasExistsDescriptor Index(Indices index) => this.Assign<Indices>(index, (Action<IAliasExistsRequest, Indices>) ((a, v) => a.RouteValues.Optional(nameof (index), (IUrlParameter) v)));

    public AliasExistsDescriptor Index<TOther>() where TOther : class => this.Assign<Type>(typeof (TOther), (Action<IAliasExistsRequest, Type>) ((a, v) => a.RouteValues.Optional("index", (IUrlParameter) (Indices) v)));

    public AliasExistsDescriptor AllIndices() => this.Index(Indices.All);

    public AliasExistsDescriptor AllowNoIndices(bool? allownoindices = true) => this.Qs("allow_no_indices", (object) allownoindices);

    public AliasExistsDescriptor ExpandWildcards(Elasticsearch.Net.ExpandWildcards? expandwildcards) => this.Qs("expand_wildcards", (object) expandwildcards);

    public AliasExistsDescriptor IgnoreUnavailable(bool? ignoreunavailable = true) => this.Qs("ignore_unavailable", (object) ignoreunavailable);

    public AliasExistsDescriptor Local(bool? local = true) => this.Qs(nameof (local), (object) local);
  }
}
