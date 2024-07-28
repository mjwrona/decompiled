// Decompiled with JetBrains decompiler
// Type: Nest.CatIndicesRequest
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.CatApi;
using System;
using System.Runtime.Serialization;

namespace Nest
{
  public class CatIndicesRequest : 
    PlainRequestBase<CatIndicesRequestParameters>,
    ICatIndicesRequest,
    IRequest<CatIndicesRequestParameters>,
    IRequest
  {
    protected ICatIndicesRequest Self => (ICatIndicesRequest) this;

    internal override ApiUrls ApiUrls => ApiUrlsLookups.CatIndices;

    public CatIndicesRequest()
    {
    }

    public CatIndicesRequest(Indices index)
      : base((Func<RouteValues, RouteValues>) (r => r.Optional(nameof (index), (IUrlParameter) index)))
    {
    }

    [IgnoreDataMember]
    Indices ICatIndicesRequest.Index => this.Self.RouteValues.Get<Indices>("index");

    public Elasticsearch.Net.Bytes? Bytes
    {
      get => this.Q<Elasticsearch.Net.Bytes?>("bytes");
      set => this.Q("bytes", (object) value);
    }

    public Elasticsearch.Net.ExpandWildcards? ExpandWildcards
    {
      get => this.Q<Elasticsearch.Net.ExpandWildcards?>("expand_wildcards");
      set => this.Q("expand_wildcards", (object) value);
    }

    public string Format
    {
      get => this.Q<string>("format");
      set
      {
        this.Q("format", (object) value);
        this.SetAcceptHeader(value);
      }
    }

    public string[] Headers
    {
      get => this.Q<string[]>("h");
      set => this.Q("h", (object) value);
    }

    public Elasticsearch.Net.Health? Health
    {
      get => this.Q<Elasticsearch.Net.Health?>("health");
      set => this.Q("health", (object) value);
    }

    public bool? Help
    {
      get => this.Q<bool?>("help");
      set => this.Q("help", (object) value);
    }

    public bool? IncludeUnloadedSegments
    {
      get => this.Q<bool?>("include_unloaded_segments");
      set => this.Q("include_unloaded_segments", (object) value);
    }

    [Obsolete("Scheduled to be removed in 8.0, Deprecated as of: 7.11.0, reason: This parameter does not affect the request. It will be removed in a future release.")]
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

    public bool? Pri
    {
      get => this.Q<bool?>("pri");
      set => this.Q("pri", (object) value);
    }

    public string[] SortByColumns
    {
      get => this.Q<string[]>("s");
      set => this.Q("s", (object) value);
    }

    public bool? Verbose
    {
      get => this.Q<bool?>("v");
      set => this.Q("v", (object) value);
    }
  }
}
