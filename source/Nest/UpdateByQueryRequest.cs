// Decompiled with JetBrains decompiler
// Type: Nest.UpdateByQueryRequest
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Utf8Json;
using System;
using System.Runtime.Serialization;

namespace Nest
{
  public class UpdateByQueryRequest : 
    PlainRequestBase<UpdateByQueryRequestParameters>,
    IUpdateByQueryRequest,
    IRequest<UpdateByQueryRequestParameters>,
    IRequest
  {
    public ISlicedScroll Slice { get; set; }

    public QueryContainer Query { get; set; }

    public IScript Script { get; set; }

    public long? MaximumDocuments { get; set; }

    protected IUpdateByQueryRequest Self => (IUpdateByQueryRequest) this;

    internal override ApiUrls ApiUrls => ApiUrlsLookups.NoNamespaceUpdateByQuery;

    public UpdateByQueryRequest(Indices index)
      : base((Func<RouteValues, RouteValues>) (r => r.Required(nameof (index), (IUrlParameter) index)))
    {
    }

    [SerializationConstructor]
    protected UpdateByQueryRequest()
    {
    }

    [IgnoreDataMember]
    Indices IUpdateByQueryRequest.Index => this.Self.RouteValues.Get<Indices>("index");

    public bool? AllowNoIndices
    {
      get => this.Q<bool?>("allow_no_indices");
      set => this.Q("allow_no_indices", (object) value);
    }

    public bool? AnalyzeWildcard
    {
      get => this.Q<bool?>("analyze_wildcard");
      set => this.Q("analyze_wildcard", (object) value);
    }

    public string Analyzer
    {
      get => this.Q<string>("analyzer");
      set => this.Q("analyzer", (object) value);
    }

    public Elasticsearch.Net.Conflicts? Conflicts
    {
      get => this.Q<Elasticsearch.Net.Conflicts?>("conflicts");
      set => this.Q("conflicts", (object) value);
    }

    public Elasticsearch.Net.DefaultOperator? DefaultOperator
    {
      get => this.Q<Elasticsearch.Net.DefaultOperator?>("default_operator");
      set => this.Q("default_operator", (object) value);
    }

    public string Df
    {
      get => this.Q<string>("df");
      set => this.Q("df", (object) value);
    }

    public Elasticsearch.Net.ExpandWildcards? ExpandWildcards
    {
      get => this.Q<Elasticsearch.Net.ExpandWildcards?>("expand_wildcards");
      set => this.Q("expand_wildcards", (object) value);
    }

    public long? From
    {
      get => this.Q<long?>("from");
      set => this.Q("from", (object) value);
    }

    public bool? IgnoreUnavailable
    {
      get => this.Q<bool?>("ignore_unavailable");
      set => this.Q("ignore_unavailable", (object) value);
    }

    public bool? Lenient
    {
      get => this.Q<bool?>("lenient");
      set => this.Q("lenient", (object) value);
    }

    public string Pipeline
    {
      get => this.Q<string>("pipeline");
      set => this.Q("pipeline", (object) value);
    }

    public string Preference
    {
      get => this.Q<string>("preference");
      set => this.Q("preference", (object) value);
    }

    public string QueryOnQueryString
    {
      get => this.Q<string>("q");
      set => this.Q("q", (object) value);
    }

    public bool? Refresh
    {
      get => this.Q<bool?>("refresh");
      set => this.Q("refresh", (object) value);
    }

    public bool? RequestCache
    {
      get => this.Q<bool?>("request_cache");
      set => this.Q("request_cache", (object) value);
    }

    public long? RequestsPerSecond
    {
      get => this.Q<long?>("requests_per_second");
      set => this.Q("requests_per_second", (object) value);
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

    public long? ScrollSize
    {
      get => this.Q<long?>("scroll_size");
      set => this.Q("scroll_size", (object) value);
    }

    public Time SearchTimeout
    {
      get => this.Q<Time>("search_timeout");
      set => this.Q("search_timeout", (object) value);
    }

    public Elasticsearch.Net.SearchType? SearchType
    {
      get => this.Q<Elasticsearch.Net.SearchType?>("search_type");
      set => this.Q("search_type", (object) value);
    }

    public long? Size
    {
      get => this.Q<long?>("size");
      set => this.Q("size", (object) value);
    }

    public long? Slices
    {
      get => this.Q<long?>("slices");
      set => this.Q("slices", (object) value);
    }

    public string[] Sort
    {
      get => this.Q<string[]>("sort");
      set => this.Q("sort", (object) value);
    }

    public string[] Stats
    {
      get => this.Q<string[]>("stats");
      set => this.Q("stats", (object) value);
    }

    public long? TerminateAfter
    {
      get => this.Q<long?>("terminate_after");
      set => this.Q("terminate_after", (object) value);
    }

    public Time Timeout
    {
      get => this.Q<Time>("timeout");
      set => this.Q("timeout", (object) value);
    }

    public bool? Version
    {
      get => this.Q<bool?>("version");
      set => this.Q("version", (object) value);
    }

    public bool? VersionType
    {
      get => this.Q<bool?>("version_type");
      set => this.Q("version_type", (object) value);
    }

    public string WaitForActiveShards
    {
      get => this.Q<string>("wait_for_active_shards");
      set => this.Q("wait_for_active_shards", (object) value);
    }

    public bool? WaitForCompletion
    {
      get => this.Q<bool?>("wait_for_completion");
      set => this.Q("wait_for_completion", (object) value);
    }
  }
}
