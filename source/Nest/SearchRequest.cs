// Decompiled with JetBrains decompiler
// Type: Nest.SearchRequest
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Utf8Json;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Nest
{
  [DataContract]
  public class SearchRequest : 
    PlainRequestBase<SearchRequestParameters>,
    ISearchRequest,
    IRequest<SearchRequestParameters>,
    IRequest,
    ITypedSearchRequest
  {
    private AggregationDictionary _aggs;

    protected ISearchRequest Self => (ISearchRequest) this;

    internal override ApiUrls ApiUrls => ApiUrlsLookups.NoNamespaceSearch;

    public SearchRequest()
    {
    }

    public SearchRequest(Indices index)
      : base((Func<RouteValues, RouteValues>) (r => r.Optional(nameof (index), (IUrlParameter) index)))
    {
    }

    [IgnoreDataMember]
    Indices ISearchRequest.Index => this.Self.RouteValues.Get<Indices>("index");

    public bool? AllowNoIndices
    {
      get => this.Q<bool?>("allow_no_indices");
      set => this.Q("allow_no_indices", (object) value);
    }

    public bool? AllowPartialSearchResults
    {
      get => this.Q<bool?>("allow_partial_search_results");
      set => this.Q("allow_partial_search_results", (object) value);
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

    public long? BatchedReduceSize
    {
      get => this.Q<long?>("batched_reduce_size");
      set => this.Q("batched_reduce_size", (object) value);
    }

    public bool? CcsMinimizeRoundtrips
    {
      get => this.Q<bool?>("ccs_minimize_roundtrips");
      set => this.Q("ccs_minimize_roundtrips", (object) value);
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

    public bool? Lenient
    {
      get => this.Q<bool?>("lenient");
      set => this.Q("lenient", (object) value);
    }

    public long? MaxConcurrentShardRequests
    {
      get => this.Q<long?>("max_concurrent_shard_requests");
      set => this.Q("max_concurrent_shard_requests", (object) value);
    }

    public string MinCompatibleShardNode
    {
      get => this.Q<string>("min_compatible_shard_node");
      set => this.Q("min_compatible_shard_node", (object) value);
    }

    public long? PreFilterShardSize
    {
      get => this.Q<long?>("pre_filter_shard_size");
      set => this.Q("pre_filter_shard_size", (object) value);
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

    public bool? RequestCache
    {
      get => this.Q<bool?>("request_cache");
      set => this.Q("request_cache", (object) value);
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

    public bool? SequenceNumberPrimaryTerm
    {
      get => this.Q<bool?>("seq_no_primary_term");
      set => this.Q("seq_no_primary_term", (object) value);
    }

    public string[] Stats
    {
      get => this.Q<string[]>("stats");
      set => this.Q("stats", (object) value);
    }

    public Field SuggestField
    {
      get => this.Q<Field>("suggest_field");
      set => this.Q("suggest_field", (object) value);
    }

    public Elasticsearch.Net.SuggestMode? SuggestMode
    {
      get => this.Q<Elasticsearch.Net.SuggestMode?>("suggest_mode");
      set => this.Q("suggest_mode", (object) value);
    }

    public long? SuggestSize
    {
      get => this.Q<long?>("suggest_size");
      set => this.Q("suggest_size", (object) value);
    }

    public string SuggestText
    {
      get => this.Q<string>("suggest_text");
      set => this.Q("suggest_text", (object) value);
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

    [DataMember(Name = "aggregations")]
    private AggregationDictionary AggregationsProxy
    {
      set => this._aggs = value;
    }

    public AggregationDictionary Aggregations
    {
      get => this._aggs;
      set => this._aggs = value;
    }

    public IFieldCollapse Collapse { get; set; }

    public Fields DocValueFields { get; set; }

    public bool? Explain { get; set; }

    public Fields Fields { get; set; }

    public int? From { get; set; }

    public IHighlight Highlight { get; set; }

    [JsonFormatter(typeof (IndicesBoostFormatter))]
    public IDictionary<IndexName, double> IndicesBoost { get; set; }

    public double? MinScore { get; set; }

    public QueryContainer PostFilter { get; set; }

    public bool? Profile { get; set; }

    public QueryContainer Query { get; set; }

    public IList<IRescore> Rescore { get; set; }

    public IScriptFields ScriptFields { get; set; }

    public IList<object> SearchAfter { get; set; }

    public int? Size { get; set; }

    public ISlicedScroll Slice { get; set; }

    public IList<ISort> Sort { get; set; }

    public Union<bool, ISourceFilter> Source { get; set; }

    public Fields StoredFields { get; set; }

    public ISuggestContainer Suggest { get; set; }

    public long? TerminateAfter { get; set; }

    public string Timeout { get; set; }

    public bool? TrackScores { get; set; }

    public bool? TrackTotalHits { get; set; }

    public bool? Version { get; set; }

    public IPointInTime PointInTime { get; set; }

    public IRuntimeFields RuntimeFields { get; set; }

    protected override HttpMethod HttpMethod
    {
      get
      {
        SearchRequestParameters requestParameters = this.RequestState.RequestParameters;
        return (requestParameters != null ? (__nonvirtual (requestParameters.ContainsQueryString("source")) ? 1 : 0) : 0) == 0 ? HttpMethod.POST : HttpMethod.GET;
      }
    }

    Type ITypedSearchRequest.ClrType => (Type) null;

    protected override sealed void RequestDefaults(SearchRequestParameters parameters) => this.TypedKeys = new bool?(true);

    protected override string ResolveUrl(
      RouteValues routeValues,
      IConnectionSettingsValues settings)
    {
      if (this.Self.PointInTime != null && !string.IsNullOrEmpty(this.Self.PointInTime.Id) && routeValues.ContainsKey("index"))
        routeValues.Remove("index");
      return base.ResolveUrl(routeValues, settings);
    }
  }
}
