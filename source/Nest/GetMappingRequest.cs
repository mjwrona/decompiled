// Decompiled with JetBrains decompiler
// Type: Nest.GetMappingRequest
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.IndicesApi;
using System;
using System.Runtime.Serialization;

namespace Nest
{
  public class GetMappingRequest : 
    PlainRequestBase<GetMappingRequestParameters>,
    IGetMappingRequest,
    IRequest<GetMappingRequestParameters>,
    IRequest
  {
    protected IGetMappingRequest Self => (IGetMappingRequest) this;

    internal override ApiUrls ApiUrls => ApiUrlsLookups.IndicesGetMapping;

    public GetMappingRequest()
    {
    }

    public GetMappingRequest(Indices index)
      : base((Func<RouteValues, RouteValues>) (r => r.Optional(nameof (index), (IUrlParameter) index)))
    {
    }

    [IgnoreDataMember]
    Indices IGetMappingRequest.Index => this.Self.RouteValues.Get<Indices>("index");

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

    public bool? IncludeTypeName
    {
      get => this.Q<bool?>("include_type_name");
      set => this.Q("include_type_name", (object) value);
    }

    [Obsolete("Scheduled to be removed in 8.0, Deprecated as of: 7.8.0, reason: This parameter is a no-op and field mappings are always retrieved locally.")]
    public bool? Local
    {
      get => this.Q<bool?>("local");
      set => this.Q("local", (object) value);
    }

    public Time MasterTimeout
    {
      get => this.Q<Time>("master_timeout");
      set => this.Q("master_timeout", (object) value);
    }
  }
}
