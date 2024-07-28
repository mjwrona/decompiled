// Decompiled with JetBrains decompiler
// Type: Nest.FieldCapabilitiesRequest
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using System;
using System.Runtime.Serialization;

namespace Nest
{
  public class FieldCapabilitiesRequest : 
    PlainRequestBase<FieldCapabilitiesRequestParameters>,
    IFieldCapabilitiesRequest,
    IRequest<FieldCapabilitiesRequestParameters>,
    IRequest
  {
    protected IFieldCapabilitiesRequest Self => (IFieldCapabilitiesRequest) this;

    internal override ApiUrls ApiUrls => ApiUrlsLookups.NoNamespaceFieldCapabilities;

    public FieldCapabilitiesRequest()
    {
    }

    public FieldCapabilitiesRequest(Indices index)
      : base((Func<RouteValues, RouteValues>) (r => r.Optional(nameof (index), (IUrlParameter) index)))
    {
    }

    [IgnoreDataMember]
    Indices IFieldCapabilitiesRequest.Index => this.Self.RouteValues.Get<Indices>("index");

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

    public Fields Fields
    {
      get => this.Q<Fields>("fields");
      set => this.Q("fields", (object) value);
    }

    public bool? IgnoreUnavailable
    {
      get => this.Q<bool?>("ignore_unavailable");
      set => this.Q("ignore_unavailable", (object) value);
    }

    public bool? IncludeUnmapped
    {
      get => this.Q<bool?>("include_unmapped");
      set => this.Q("include_unmapped", (object) value);
    }

    protected override HttpMethod HttpMethod => this.IndexFilter == null ? HttpMethod.GET : HttpMethod.POST;

    public QueryContainer IndexFilter { get; set; }
  }
}
