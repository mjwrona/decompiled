// Decompiled with JetBrains decompiler
// Type: Nest.AliasExistsRequest
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
  public class AliasExistsRequest : 
    PlainRequestBase<AliasExistsRequestParameters>,
    IAliasExistsRequest,
    IRequest<AliasExistsRequestParameters>,
    IRequest
  {
    protected IAliasExistsRequest Self => (IAliasExistsRequest) this;

    internal override ApiUrls ApiUrls => ApiUrlsLookups.IndicesAliasExists;

    public AliasExistsRequest(Names name)
      : base((Func<RouteValues, RouteValues>) (r => r.Required(nameof (name), (IUrlParameter) name)))
    {
    }

    public AliasExistsRequest(Indices index, Names name)
      : base((Func<RouteValues, RouteValues>) (r => r.Optional(nameof (index), (IUrlParameter) index).Required(nameof (name), (IUrlParameter) name)))
    {
    }

    [SerializationConstructor]
    protected AliasExistsRequest()
    {
    }

    [IgnoreDataMember]
    Names IAliasExistsRequest.Name => this.Self.RouteValues.Get<Names>("name");

    [IgnoreDataMember]
    Indices IAliasExistsRequest.Index => this.Self.RouteValues.Get<Indices>("index");

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

    public bool? Local
    {
      get => this.Q<bool?>("local");
      set => this.Q("local", (object) value);
    }
  }
}
