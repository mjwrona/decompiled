// Decompiled with JetBrains decompiler
// Type: Nest.IndicesShardStoresRequest
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.IndicesApi;
using System;
using System.Runtime.Serialization;

namespace Nest
{
  public class IndicesShardStoresRequest : 
    PlainRequestBase<IndicesShardStoresRequestParameters>,
    IIndicesShardStoresRequest,
    IRequest<IndicesShardStoresRequestParameters>,
    IRequest
  {
    protected IIndicesShardStoresRequest Self => (IIndicesShardStoresRequest) this;

    internal override ApiUrls ApiUrls => ApiUrlsLookups.IndicesShardStores;

    public IndicesShardStoresRequest()
    {
    }

    public IndicesShardStoresRequest(Indices index)
      : base((Func<RouteValues, RouteValues>) (r => r.Optional(nameof (index), (IUrlParameter) index)))
    {
    }

    [IgnoreDataMember]
    Indices IIndicesShardStoresRequest.Index => this.Self.RouteValues.Get<Indices>("index");

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

    public string[] Status
    {
      get => this.Q<string[]>("status");
      set => this.Q("status", (object) value);
    }
  }
}
