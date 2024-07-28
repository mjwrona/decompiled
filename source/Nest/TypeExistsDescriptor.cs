// Decompiled with JetBrains decompiler
// Type: Nest.TypeExistsDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.IndicesApi;
using Elasticsearch.Net.Utf8Json;
using System;

namespace Nest
{
  public class TypeExistsDescriptor : 
    RequestDescriptorBase<TypeExistsDescriptor, TypeExistsRequestParameters, ITypeExistsRequest>,
    ITypeExistsRequest,
    IRequest<TypeExistsRequestParameters>,
    IRequest
  {
    internal override ApiUrls ApiUrls => ApiUrlsLookups.IndicesTypeExists;

    public TypeExistsDescriptor(Indices index, Names type)
      : base((Func<RouteValues, RouteValues>) (r => r.Required(nameof (index), (IUrlParameter) index).Required(nameof (type), (IUrlParameter) type)))
    {
    }

    [SerializationConstructor]
    protected TypeExistsDescriptor()
    {
    }

    Indices ITypeExistsRequest.Index => this.Self.RouteValues.Get<Indices>("index");

    Names ITypeExistsRequest.Type => this.Self.RouteValues.Get<Names>("type");

    public TypeExistsDescriptor Index(Indices index) => this.Assign<Indices>(index, (Action<ITypeExistsRequest, Indices>) ((a, v) => a.RouteValues.Required(nameof (index), (IUrlParameter) v)));

    public TypeExistsDescriptor Index<TOther>() where TOther : class => this.Assign<Type>(typeof (TOther), (Action<ITypeExistsRequest, Type>) ((a, v) => a.RouteValues.Required("index", (IUrlParameter) (Indices) v)));

    public TypeExistsDescriptor AllIndices() => this.Index(Indices.All);

    public TypeExistsDescriptor AllowNoIndices(bool? allownoindices = true) => this.Qs("allow_no_indices", (object) allownoindices);

    public TypeExistsDescriptor ExpandWildcards(Elasticsearch.Net.ExpandWildcards? expandwildcards) => this.Qs("expand_wildcards", (object) expandwildcards);

    public TypeExistsDescriptor IgnoreUnavailable(bool? ignoreunavailable = true) => this.Qs("ignore_unavailable", (object) ignoreunavailable);

    public TypeExistsDescriptor Local(bool? local = true) => this.Qs(nameof (local), (object) local);
  }
}
