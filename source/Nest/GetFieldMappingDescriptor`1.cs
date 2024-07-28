// Decompiled with JetBrains decompiler
// Type: Nest.GetFieldMappingDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.IndicesApi;
using Elasticsearch.Net.Utf8Json;
using System;

namespace Nest
{
  public class GetFieldMappingDescriptor<TDocument> : 
    RequestDescriptorBase<GetFieldMappingDescriptor<TDocument>, GetFieldMappingRequestParameters, IGetFieldMappingRequest>,
    IGetFieldMappingRequest,
    IRequest<GetFieldMappingRequestParameters>,
    IRequest
    where TDocument : class
  {
    internal override ApiUrls ApiUrls => ApiUrlsLookups.IndicesGetFieldMapping;

    public GetFieldMappingDescriptor(Fields fields)
      : this((Indices) typeof (TDocument), fields)
    {
    }

    public GetFieldMappingDescriptor(Indices index, Fields fields)
      : base((Func<RouteValues, RouteValues>) (r => r.Optional(nameof (index), (IUrlParameter) index).Required(nameof (fields), (IUrlParameter) fields)))
    {
    }

    [SerializationConstructor]
    protected GetFieldMappingDescriptor()
    {
    }

    Fields IGetFieldMappingRequest.Fields => this.Self.RouteValues.Get<Fields>("fields");

    Indices IGetFieldMappingRequest.Index => this.Self.RouteValues.Get<Indices>("index");

    public GetFieldMappingDescriptor<TDocument> Index(Indices index) => this.Assign<Indices>(index, (Action<IGetFieldMappingRequest, Indices>) ((a, v) => a.RouteValues.Optional(nameof (index), (IUrlParameter) v)));

    public GetFieldMappingDescriptor<TDocument> Index<TOther>() where TOther : class => this.Assign<Type>(typeof (TOther), (Action<IGetFieldMappingRequest, Type>) ((a, v) => a.RouteValues.Optional("index", (IUrlParameter) (Indices) v)));

    public GetFieldMappingDescriptor<TDocument> AllIndices() => this.Index(Indices.All);

    public GetFieldMappingDescriptor<TDocument> AllowNoIndices(bool? allownoindices = true) => this.Qs("allow_no_indices", (object) allownoindices);

    public GetFieldMappingDescriptor<TDocument> ExpandWildcards(Elasticsearch.Net.ExpandWildcards? expandwildcards) => this.Qs("expand_wildcards", (object) expandwildcards);

    public GetFieldMappingDescriptor<TDocument> IgnoreUnavailable(bool? ignoreunavailable = true) => this.Qs("ignore_unavailable", (object) ignoreunavailable);

    public GetFieldMappingDescriptor<TDocument> IncludeDefaults(bool? includedefaults = true) => this.Qs("include_defaults", (object) includedefaults);

    public GetFieldMappingDescriptor<TDocument> IncludeTypeName(bool? includetypename = true) => this.Qs("include_type_name", (object) includetypename);

    public GetFieldMappingDescriptor<TDocument> Local(bool? local = true) => this.Qs(nameof (local), (object) local);
  }
}
