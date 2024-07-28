// Decompiled with JetBrains decompiler
// Type: Nest.GetMappingDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.IndicesApi;
using System;

namespace Nest
{
  public class GetMappingDescriptor<TDocument> : 
    RequestDescriptorBase<GetMappingDescriptor<TDocument>, GetMappingRequestParameters, IGetMappingRequest>,
    IGetMappingRequest,
    IRequest<GetMappingRequestParameters>,
    IRequest
    where TDocument : class
  {
    internal override ApiUrls ApiUrls => ApiUrlsLookups.IndicesGetMapping;

    public GetMappingDescriptor()
      : this((Indices) typeof (TDocument))
    {
    }

    public GetMappingDescriptor(Indices index)
      : base((Func<RouteValues, RouteValues>) (r => r.Optional(nameof (index), (IUrlParameter) index)))
    {
    }

    Indices IGetMappingRequest.Index => this.Self.RouteValues.Get<Indices>("index");

    public GetMappingDescriptor<TDocument> Index(Indices index) => this.Assign<Indices>(index, (Action<IGetMappingRequest, Indices>) ((a, v) => a.RouteValues.Optional(nameof (index), (IUrlParameter) v)));

    public GetMappingDescriptor<TDocument> Index<TOther>() where TOther : class => this.Assign<Type>(typeof (TOther), (Action<IGetMappingRequest, Type>) ((a, v) => a.RouteValues.Optional("index", (IUrlParameter) (Indices) v)));

    public GetMappingDescriptor<TDocument> AllIndices() => this.Index(Indices.All);

    public GetMappingDescriptor<TDocument> AllowNoIndices(bool? allownoindices = true) => this.Qs("allow_no_indices", (object) allownoindices);

    public GetMappingDescriptor<TDocument> ExpandWildcards(Elasticsearch.Net.ExpandWildcards? expandwildcards) => this.Qs("expand_wildcards", (object) expandwildcards);

    public GetMappingDescriptor<TDocument> IgnoreUnavailable(bool? ignoreunavailable = true) => this.Qs("ignore_unavailable", (object) ignoreunavailable);

    public GetMappingDescriptor<TDocument> IncludeTypeName(bool? includetypename = true) => this.Qs("include_type_name", (object) includetypename);

    [Obsolete("Scheduled to be removed in 8.0, Deprecated as of: 7.8.0, reason: This parameter is a no-op and field mappings are always retrieved locally.")]
    public GetMappingDescriptor<TDocument> Local(bool? local = true) => this.Qs(nameof (local), (object) local);

    public GetMappingDescriptor<TDocument> MasterTimeout(Time mastertimeout) => this.Qs("master_timeout", (object) mastertimeout);
  }
}
