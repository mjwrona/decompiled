// Decompiled with JetBrains decompiler
// Type: Nest.SearchTemplateRequest
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Nest
{
  public class SearchTemplateRequest : 
    PlainRequestBase<SearchTemplateRequestParameters>,
    ISearchTemplateRequest,
    IRequest<SearchTemplateRequestParameters>,
    IRequest,
    ITypedSearchRequest
  {
    protected ISearchTemplateRequest Self => (ISearchTemplateRequest) this;

    internal override ApiUrls ApiUrls => ApiUrlsLookups.NoNamespaceSearchTemplate;

    public SearchTemplateRequest()
    {
    }

    public SearchTemplateRequest(Indices index)
      : base((Func<RouteValues, RouteValues>) (r => r.Optional(nameof (index), (IUrlParameter) index)))
    {
    }

    [IgnoreDataMember]
    Indices ISearchTemplateRequest.Index => this.Self.RouteValues.Get<Indices>("index");

    public bool? AllowNoIndices
    {
      get => this.Q<bool?>("allow_no_indices");
      set => this.Q("allow_no_indices", (object) value);
    }

    public bool? CcsMinimizeRoundtrips
    {
      get => this.Q<bool?>("ccs_minimize_roundtrips");
      set => this.Q("ccs_minimize_roundtrips", (object) value);
    }

    public Elasticsearch.Net.ExpandWildcards? ExpandWildcards
    {
      get => this.Q<Elasticsearch.Net.ExpandWildcards?>("expand_wildcards");
      set => this.Q("expand_wildcards", (object) value);
    }

    public bool? IgnoreThrottled
    {
      get => this.Q<bool?>("ignore_throttled");
      set => this.Q("ignore_throttled", (object) value);
    }

    public bool? IgnoreUnavailable
    {
      get => this.Q<bool?>("ignore_unavailable");
      set => this.Q("ignore_unavailable", (object) value);
    }

    public string Preference
    {
      get => this.Q<string>("preference");
      set => this.Q("preference", (object) value);
    }

    public bool? Profile
    {
      get => this.Q<bool?>("profile");
      set => this.Q("profile", (object) value);
    }

    public Routing Routing
    {
      get => this.Q<Routing>("routing");
      set => this.Q("routing", (object) value);
    }

    public Time Scroll
    {
      get => this.Q<Time>("scroll");
      set => this.Q("scroll", (object) value);
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

    public string Id { get; set; }

    public IDictionary<string, object> Params { get; set; }

    public string Source { get; set; }

    public bool? Explain { get; set; }

    protected Type ClrType { get; set; }

    Type ITypedSearchRequest.ClrType => this.ClrType;

    protected override sealed void RequestDefaults(SearchTemplateRequestParameters parameters) => this.TypedKeys = new bool?(true);
  }
}
