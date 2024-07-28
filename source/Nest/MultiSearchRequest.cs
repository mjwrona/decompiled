// Decompiled with JetBrains decompiler
// Type: Nest.MultiSearchRequest
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Nest
{
  public class MultiSearchRequest : 
    PlainRequestBase<MultiSearchRequestParameters>,
    IMultiSearchRequest,
    IRequest<MultiSearchRequestParameters>,
    IRequest
  {
    protected IMultiSearchRequest Self => (IMultiSearchRequest) this;

    internal override ApiUrls ApiUrls => ApiUrlsLookups.NoNamespaceMultiSearch;

    public MultiSearchRequest()
    {
    }

    public MultiSearchRequest(Indices index)
      : base((Func<RouteValues, RouteValues>) (r => r.Optional(nameof (index), (IUrlParameter) index)))
    {
    }

    [IgnoreDataMember]
    Indices IMultiSearchRequest.Index => this.Self.RouteValues.Get<Indices>("index");

    public bool? CcsMinimizeRoundtrips
    {
      get => this.Q<bool?>("ccs_minimize_roundtrips");
      set => this.Q("ccs_minimize_roundtrips", (object) value);
    }

    public long? MaxConcurrentSearches
    {
      get => this.Q<long?>("max_concurrent_searches");
      set => this.Q("max_concurrent_searches", (object) value);
    }

    public long? MaxConcurrentShardRequests
    {
      get => this.Q<long?>("max_concurrent_shard_requests");
      set => this.Q("max_concurrent_shard_requests", (object) value);
    }

    public long? PreFilterShardSize
    {
      get => this.Q<long?>("pre_filter_shard_size");
      set => this.Q("pre_filter_shard_size", (object) value);
    }

    public Elasticsearch.Net.SearchType? SearchType
    {
      get => this.Q<Elasticsearch.Net.SearchType?>("search_type");
      set => this.Q("search_type", (object) value);
    }

    public bool? TotalHitsAsInteger
    {
      get => this.Q<bool?>("rest_total_hits_as_int");
      set => this.Q("rest_total_hits_as_int", (object) value);
    }

    public bool? TypedKeys
    {
      get => this.Q<bool?>("typed_keys");
      set => this.Q("typed_keys", (object) value);
    }

    protected override sealed void RequestDefaults(MultiSearchRequestParameters parameters)
    {
      this.TypedKeys = new bool?(true);
      parameters.CustomResponseBuilder = (CustomResponseBuilderBase) new MultiSearchResponseBuilder((IRequest) this);
    }

    public IDictionary<string, ISearchRequest> Operations { get; set; }
  }
}
