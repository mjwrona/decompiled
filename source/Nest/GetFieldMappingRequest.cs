// Decompiled with JetBrains decompiler
// Type: Nest.GetFieldMappingRequest
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.IndicesApi;
using Elasticsearch.Net.Utf8Json;
using System;
using System.Runtime.Serialization;

namespace Nest
{
  public class GetFieldMappingRequest : 
    PlainRequestBase<GetFieldMappingRequestParameters>,
    IGetFieldMappingRequest,
    IRequest<GetFieldMappingRequestParameters>,
    IRequest
  {
    protected IGetFieldMappingRequest Self => (IGetFieldMappingRequest) this;

    internal override ApiUrls ApiUrls => ApiUrlsLookups.IndicesGetFieldMapping;

    public GetFieldMappingRequest(Fields fields)
      : base((Func<RouteValues, RouteValues>) (r => r.Required(nameof (fields), (IUrlParameter) fields)))
    {
    }

    public GetFieldMappingRequest(Indices index, Fields fields)
      : base((Func<RouteValues, RouteValues>) (r => r.Optional(nameof (index), (IUrlParameter) index).Required(nameof (fields), (IUrlParameter) fields)))
    {
    }

    [SerializationConstructor]
    protected GetFieldMappingRequest()
    {
    }

    [IgnoreDataMember]
    Fields IGetFieldMappingRequest.Fields => this.Self.RouteValues.Get<Fields>("fields");

    [IgnoreDataMember]
    Indices IGetFieldMappingRequest.Index => this.Self.RouteValues.Get<Indices>("index");

    public bool? AllowNoIndices
    {
      get => this.Q<bool?>("allow_no_indices");
      set => this.Q("allow_no_indices", (object) value);
    }

    public Elasticsearch.Net.ExpandWildcards? ExpandWildcards
    {
      get => this.Q<Elasticsearch.Net.ExpandWildcards?>("expand_wildcards");
      set => this.Q("expand_wildcards", (object) value);
    }

    public bool? IgnoreUnavailable
    {
      get => this.Q<bool?>("ignore_unavailable");
      set => this.Q("ignore_unavailable", (object) value);
    }

    public bool? IncludeDefaults
    {
      get => this.Q<bool?>("include_defaults");
      set => this.Q("include_defaults", (object) value);
    }

    public bool? IncludeTypeName
    {
      get => this.Q<bool?>("include_type_name");
      set => this.Q("include_type_name", (object) value);
    }

    public bool? Local
    {
      get => this.Q<bool?>("local");
      set => this.Q("local", (object) value);
    }
  }
}
