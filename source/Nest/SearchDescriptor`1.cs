// Decompiled with JetBrains decompiler
// Type: Nest.SearchDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Runtime.Serialization;

namespace Nest
{
  [DataContract]
  public class SearchDescriptor<TInferDocument> : 
    RequestDescriptorBase<SearchDescriptor<TInferDocument>, SearchRequestParameters, ISearchRequest<TInferDocument>>,
    ISearchRequest<TInferDocument>,
    ISearchRequest,
    IRequest<SearchRequestParameters>,
    IRequest,
    ITypedSearchRequest
    where TInferDocument : class
  {
    internal override ApiUrls ApiUrls => ApiUrlsLookups.NoNamespaceSearch;

    public SearchDescriptor()
      : this((Indices) typeof (TInferDocument))
    {
    }

    public SearchDescriptor(Indices index)
      : base((Func<RouteValues, RouteValues>) (r => r.Optional(nameof (index), (IUrlParameter) index)))
    {
    }

    Indices ISearchRequest.Index => this.Self.RouteValues.Get<Indices>("index");

    public SearchDescriptor<TInferDocument> Index(Indices index) => this.Assign<Indices>(index, (Action<ISearchRequest<TInferDocument>, Indices>) ((a, v) => a.RouteValues.Optional(nameof (index), (IUrlParameter) v)));

    public SearchDescriptor<TInferDocument> Index<TOther>() where TOther : class => this.Assign<Type>(typeof (TOther), (Action<ISearchRequest<TInferDocument>, Type>) ((a, v) => a.RouteValues.Optional("index", (IUrlParameter) (Indices) v)));

    public SearchDescriptor<TInferDocument> AllIndices() => this.Index(Indices.All);

    public SearchDescriptor<TInferDocument> AllowNoIndices(bool? allownoindices = true) => this.Qs("allow_no_indices", (object) allownoindices);

    public SearchDescriptor<TInferDocument> AllowPartialSearchResults(
      bool? allowpartialsearchresults = true)
    {
      return this.Qs("allow_partial_search_results", (object) allowpartialsearchresults);
    }

    public SearchDescriptor<TInferDocument> AnalyzeWildcard(bool? analyzewildcard = true) => this.Qs("analyze_wildcard", (object) analyzewildcard);

    public SearchDescriptor<TInferDocument> Analyzer(string analyzer) => this.Qs(nameof (analyzer), (object) analyzer);

    public SearchDescriptor<TInferDocument> BatchedReduceSize(long? batchedreducesize) => this.Qs("batched_reduce_size", (object) batchedreducesize);

    public SearchDescriptor<TInferDocument> CcsMinimizeRoundtrips(bool? ccsminimizeroundtrips = true) => this.Qs("ccs_minimize_roundtrips", (object) ccsminimizeroundtrips);

    public SearchDescriptor<TInferDocument> DefaultOperator(Elasticsearch.Net.DefaultOperator? defaultoperator) => this.Qs("default_operator", (object) defaultoperator);

    public SearchDescriptor<TInferDocument> Df(string df) => this.Qs(nameof (df), (object) df);

    public SearchDescriptor<TInferDocument> ExpandWildcards(Elasticsearch.Net.ExpandWildcards? expandwildcards) => this.Qs("expand_wildcards", (object) expandwildcards);

    public SearchDescriptor<TInferDocument> IgnoreThrottled(bool? ignorethrottled = true) => this.Qs("ignore_throttled", (object) ignorethrottled);

    public SearchDescriptor<TInferDocument> IgnoreUnavailable(bool? ignoreunavailable = true) => this.Qs("ignore_unavailable", (object) ignoreunavailable);

    public SearchDescriptor<TInferDocument> Lenient(bool? lenient = true) => this.Qs(nameof (lenient), (object) lenient);

    public SearchDescriptor<TInferDocument> MaxConcurrentShardRequests(
      long? maxconcurrentshardrequests)
    {
      return this.Qs("max_concurrent_shard_requests", (object) maxconcurrentshardrequests);
    }

    public SearchDescriptor<TInferDocument> MinCompatibleShardNode(string mincompatibleshardnode) => this.Qs("min_compatible_shard_node", (object) mincompatibleshardnode);

    public SearchDescriptor<TInferDocument> PreFilterShardSize(long? prefiltershardsize) => this.Qs("pre_filter_shard_size", (object) prefiltershardsize);

    public SearchDescriptor<TInferDocument> Preference(string preference) => this.Qs(nameof (preference), (object) preference);

    public SearchDescriptor<TInferDocument> QueryOnQueryString(string queryonquerystring) => this.Qs("q", (object) queryonquerystring);

    public SearchDescriptor<TInferDocument> RequestCache(bool? requestcache = true) => this.Qs("request_cache", (object) requestcache);

    public SearchDescriptor<TInferDocument> Routing(Nest.Routing routing) => this.Qs(nameof (routing), (object) routing);

    public SearchDescriptor<TInferDocument> Scroll(Time scroll) => this.Qs(nameof (scroll), (object) scroll);

    public SearchDescriptor<TInferDocument> SearchType(Elasticsearch.Net.SearchType? searchtype) => this.Qs("search_type", (object) searchtype);

    public SearchDescriptor<TInferDocument> SequenceNumberPrimaryTerm(
      bool? sequencenumberprimaryterm = true)
    {
      return this.Qs("seq_no_primary_term", (object) sequencenumberprimaryterm);
    }

    public SearchDescriptor<TInferDocument> Stats(params string[] stats) => this.Qs(nameof (stats), (object) stats);

    public SearchDescriptor<TInferDocument> SuggestField(Field suggestfield) => this.Qs("suggest_field", (object) suggestfield);

    public SearchDescriptor<TInferDocument> SuggestField(
      Expression<Func<TInferDocument, object>> field)
    {
      return this.Qs("suggest_field", (object) (Field) (Expression) field);
    }

    public SearchDescriptor<TInferDocument> SuggestMode(Elasticsearch.Net.SuggestMode? suggestmode) => this.Qs("suggest_mode", (object) suggestmode);

    public SearchDescriptor<TInferDocument> SuggestSize(long? suggestsize) => this.Qs("suggest_size", (object) suggestsize);

    public SearchDescriptor<TInferDocument> SuggestText(string suggesttext) => this.Qs("suggest_text", (object) suggesttext);

    public SearchDescriptor<TInferDocument> TotalHitsAsInteger(bool? totalhitsasinteger = true) => this.Qs("rest_total_hits_as_int", (object) totalhitsasinteger);

    public SearchDescriptor<TInferDocument> TypedKeys(bool? typedkeys = true) => this.Qs("typed_keys", (object) typedkeys);

    protected override HttpMethod HttpMethod
    {
      get
      {
        SearchRequestParameters requestParameters = this.RequestState.RequestParameters;
        return (requestParameters != null ? (__nonvirtual (requestParameters.ContainsQueryString("source")) ? 1 : 0) : 0) == 0 ? HttpMethod.POST : HttpMethod.GET;
      }
    }

    AggregationDictionary ISearchRequest.Aggregations { get; set; }

    Type ITypedSearchRequest.ClrType => typeof (TInferDocument);

    IFieldCollapse ISearchRequest.Collapse { get; set; }

    Nest.Fields ISearchRequest.DocValueFields { get; set; }

    bool? ISearchRequest.Explain { get; set; }

    Nest.Fields ISearchRequest.Fields { get; set; }

    int? ISearchRequest.From { get; set; }

    IHighlight ISearchRequest.Highlight { get; set; }

    IDictionary<IndexName, double> ISearchRequest.IndicesBoost { get; set; }

    double? ISearchRequest.MinScore { get; set; }

    QueryContainer ISearchRequest.PostFilter { get; set; }

    bool? ISearchRequest.Profile { get; set; }

    QueryContainer ISearchRequest.Query { get; set; }

    IList<IRescore> ISearchRequest.Rescore { get; set; }

    IScriptFields ISearchRequest.ScriptFields { get; set; }

    IList<object> ISearchRequest.SearchAfter { get; set; }

    int? ISearchRequest.Size { get; set; }

    ISlicedScroll ISearchRequest.Slice { get; set; }

    IList<ISort> ISearchRequest.Sort { get; set; }

    Union<bool, ISourceFilter> ISearchRequest.Source { get; set; }

    Nest.Fields ISearchRequest.StoredFields { get; set; }

    ISuggestContainer ISearchRequest.Suggest { get; set; }

    long? ISearchRequest.TerminateAfter { get; set; }

    string ISearchRequest.Timeout { get; set; }

    bool? ISearchRequest.TrackScores { get; set; }

    bool? ISearchRequest.TrackTotalHits { get; set; }

    bool? ISearchRequest.Version { get; set; }

    IPointInTime ISearchRequest.PointInTime { get; set; }

    IRuntimeFields ISearchRequest.RuntimeFields { get; set; }

    protected override sealed void RequestDefaults(SearchRequestParameters parameters) => this.TypedKeys();

    public SearchDescriptor<TInferDocument> Aggregations(
      Func<AggregationContainerDescriptor<TInferDocument>, IAggregationContainer> aggregationsSelector)
    {
      return this.Assign<AggregationDictionary>(aggregationsSelector(new AggregationContainerDescriptor<TInferDocument>())?.Aggregations, (Action<ISearchRequest<TInferDocument>, AggregationDictionary>) ((a, v) => a.Aggregations = v));
    }

    public SearchDescriptor<TInferDocument> Aggregations(AggregationDictionary aggregations) => this.Assign<AggregationDictionary>(aggregations, (Action<ISearchRequest<TInferDocument>, AggregationDictionary>) ((a, v) => a.Aggregations = v));

    public SearchDescriptor<TInferDocument> Source(bool enabled = true) => this.Assign<bool>(enabled, (Action<ISearchRequest<TInferDocument>, bool>) ((a, v) => a.Source = (Union<bool, ISourceFilter>) v));

    public SearchDescriptor<TInferDocument> Source(
      Func<SourceFilterDescriptor<TInferDocument>, ISourceFilter> selector)
    {
      return this.Assign<ISourceFilter>(selector != null ? selector(new SourceFilterDescriptor<TInferDocument>()) : (ISourceFilter) null, (Action<ISearchRequest<TInferDocument>, ISourceFilter>) ((a, v) => a.Source = v == null ? (Union<bool, ISourceFilter>) null : new Union<bool, ISourceFilter>(v)));
    }

    public SearchDescriptor<TInferDocument> Size(int? size) => this.Assign<int?>(size, (Action<ISearchRequest<TInferDocument>, int?>) ((a, v) => a.Size = v));

    public SearchDescriptor<TInferDocument> Take(int? take) => this.Size(take);

    public SearchDescriptor<TInferDocument> Fields(
      Func<FieldsDescriptor<TInferDocument>, IPromise<Nest.Fields>> fields)
    {
      return this.Assign<Func<FieldsDescriptor<TInferDocument>, IPromise<Nest.Fields>>>(fields, (Action<ISearchRequest<TInferDocument>, Func<FieldsDescriptor<TInferDocument>, IPromise<Nest.Fields>>>) ((a, v) => a.Fields = v != null ? v(new FieldsDescriptor<TInferDocument>())?.Value : (Nest.Fields) null));
    }

    public SearchDescriptor<TInferDocument> Fields<TSource>(
      Func<FieldsDescriptor<TSource>, IPromise<Nest.Fields>> fields)
      where TSource : class
    {
      return this.Assign<Func<FieldsDescriptor<TSource>, IPromise<Nest.Fields>>>(fields, (Action<ISearchRequest<TInferDocument>, Func<FieldsDescriptor<TSource>, IPromise<Nest.Fields>>>) ((a, v) => a.Fields = v != null ? v(new FieldsDescriptor<TSource>())?.Value : (Nest.Fields) null));
    }

    public SearchDescriptor<TInferDocument> Fields(Nest.Fields fields) => this.Assign<Nest.Fields>(fields, (Action<ISearchRequest<TInferDocument>, Nest.Fields>) ((a, v) => a.DocValueFields = v));

    public SearchDescriptor<TInferDocument> From(int? from) => this.Assign<int?>(from, (Action<ISearchRequest<TInferDocument>, int?>) ((a, v) => a.From = v));

    public SearchDescriptor<TInferDocument> Skip(int? skip) => this.From(skip);

    public SearchDescriptor<TInferDocument> Timeout(string timeout) => this.Assign<string>(timeout, (Action<ISearchRequest<TInferDocument>, string>) ((a, v) => a.Timeout = v));

    public SearchDescriptor<TInferDocument> Explain(bool? explain = true) => this.Assign<bool?>(explain, (Action<ISearchRequest<TInferDocument>, bool?>) ((a, v) => a.Explain = v));

    public SearchDescriptor<TInferDocument> Version(bool? version = true) => this.Assign<bool?>(version, (Action<ISearchRequest<TInferDocument>, bool?>) ((a, v) => a.Version = v));

    public SearchDescriptor<TInferDocument> TrackScores(bool? trackscores = true) => this.Assign<bool?>(trackscores, (Action<ISearchRequest<TInferDocument>, bool?>) ((a, v) => a.TrackScores = v));

    public SearchDescriptor<TInferDocument> Profile(bool? profile = true) => this.Assign<bool?>(profile, (Action<ISearchRequest<TInferDocument>, bool?>) ((a, v) => a.Profile = v));

    public SearchDescriptor<TInferDocument> MinScore(double? minScore) => this.Assign<double?>(minScore, (Action<ISearchRequest<TInferDocument>, double?>) ((a, v) => a.MinScore = v));

    public SearchDescriptor<TInferDocument> TerminateAfter(long? terminateAfter) => this.Assign<long?>(terminateAfter, (Action<ISearchRequest<TInferDocument>, long?>) ((a, v) => a.TerminateAfter = v));

    public SearchDescriptor<TInferDocument> ExecuteOnLocalShard() => this.Preference("_local");

    public SearchDescriptor<TInferDocument> ExecuteOnNode(string node) => this.Preference("_only_node:" + node);

    public SearchDescriptor<TInferDocument> ExecuteOnPreferredNode(string node) => this.Preference(node.IsNullOrEmpty() ? (string) null : "_prefer_node:" + node);

    public SearchDescriptor<TInferDocument> IndicesBoost(
      Func<FluentDictionary<IndexName, double>, FluentDictionary<IndexName, double>> boost)
    {
      return this.Assign<Func<FluentDictionary<IndexName, double>, FluentDictionary<IndexName, double>>>(boost, (Action<ISearchRequest<TInferDocument>, Func<FluentDictionary<IndexName, double>, FluentDictionary<IndexName, double>>>) ((a, v) => a.IndicesBoost = v != null ? (IDictionary<IndexName, double>) v(new FluentDictionary<IndexName, double>()) : (IDictionary<IndexName, double>) null));
    }

    public SearchDescriptor<TInferDocument> StoredFields(
      Func<FieldsDescriptor<TInferDocument>, IPromise<Nest.Fields>> fields)
    {
      return this.Assign<Func<FieldsDescriptor<TInferDocument>, IPromise<Nest.Fields>>>(fields, (Action<ISearchRequest<TInferDocument>, Func<FieldsDescriptor<TInferDocument>, IPromise<Nest.Fields>>>) ((a, v) => a.StoredFields = v != null ? v(new FieldsDescriptor<TInferDocument>())?.Value : (Nest.Fields) null));
    }

    public SearchDescriptor<TInferDocument> StoredFields(Nest.Fields fields) => this.Assign<Nest.Fields>(fields, (Action<ISearchRequest<TInferDocument>, Nest.Fields>) ((a, v) => a.StoredFields = v));

    public SearchDescriptor<TInferDocument> ScriptFields(
      Func<ScriptFieldsDescriptor, IPromise<IScriptFields>> selector)
    {
      return this.Assign<Func<ScriptFieldsDescriptor, IPromise<IScriptFields>>>(selector, (Action<ISearchRequest<TInferDocument>, Func<ScriptFieldsDescriptor, IPromise<IScriptFields>>>) ((a, v) => a.ScriptFields = v != null ? v(new ScriptFieldsDescriptor())?.Value : (IScriptFields) null));
    }

    public SearchDescriptor<TInferDocument> DocValueFields(
      Func<FieldsDescriptor<TInferDocument>, IPromise<Nest.Fields>> fields)
    {
      return this.Assign<Func<FieldsDescriptor<TInferDocument>, IPromise<Nest.Fields>>>(fields, (Action<ISearchRequest<TInferDocument>, Func<FieldsDescriptor<TInferDocument>, IPromise<Nest.Fields>>>) ((a, v) => a.DocValueFields = v != null ? v(new FieldsDescriptor<TInferDocument>())?.Value : (Nest.Fields) null));
    }

    public SearchDescriptor<TInferDocument> DocValueFields(Nest.Fields fields) => this.Assign<Nest.Fields>(fields, (Action<ISearchRequest<TInferDocument>, Nest.Fields>) ((a, v) => a.DocValueFields = v));

    public SearchDescriptor<TInferDocument> Sort(
      Func<SortDescriptor<TInferDocument>, IPromise<IList<ISort>>> selector)
    {
      return this.Assign<Func<SortDescriptor<TInferDocument>, IPromise<IList<ISort>>>>(selector, (Action<ISearchRequest<TInferDocument>, Func<SortDescriptor<TInferDocument>, IPromise<IList<ISort>>>>) ((a, v) => a.Sort = v != null ? v(new SortDescriptor<TInferDocument>())?.Value : (IList<ISort>) null));
    }

    public SearchDescriptor<TInferDocument> SearchAfter(IEnumerable<object> searchAfter) => this.Assign<IEnumerable<object>>(searchAfter, (Action<ISearchRequest<TInferDocument>, IEnumerable<object>>) ((a, v) => a.SearchAfter = v != null ? (IList<object>) v.ToListOrNullIfEmpty<object>() : (IList<object>) null));

    public SearchDescriptor<TInferDocument> SearchAfter(IList<object> searchAfter) => this.Assign<IList<object>>(searchAfter, (Action<ISearchRequest<TInferDocument>, IList<object>>) ((a, v) => a.SearchAfter = v));

    public SearchDescriptor<TInferDocument> SearchAfter(params object[] searchAfter) => this.Assign<object[]>(searchAfter, (Action<ISearchRequest<TInferDocument>, object[]>) ((a, v) => a.SearchAfter = (IList<object>) v));

    public SearchDescriptor<TInferDocument> Suggest(
      Func<SuggestContainerDescriptor<TInferDocument>, IPromise<ISuggestContainer>> selector)
    {
      return this.Assign<Func<SuggestContainerDescriptor<TInferDocument>, IPromise<ISuggestContainer>>>(selector, (Action<ISearchRequest<TInferDocument>, Func<SuggestContainerDescriptor<TInferDocument>, IPromise<ISuggestContainer>>>) ((a, v) => a.Suggest = v != null ? v(new SuggestContainerDescriptor<TInferDocument>())?.Value : (ISuggestContainer) null));
    }

    public SearchDescriptor<TInferDocument> Query(
      Func<QueryContainerDescriptor<TInferDocument>, QueryContainer> query)
    {
      return this.Assign<Func<QueryContainerDescriptor<TInferDocument>, QueryContainer>>(query, (Action<ISearchRequest<TInferDocument>, Func<QueryContainerDescriptor<TInferDocument>, QueryContainer>>) ((a, v) => a.Query = v != null ? v(new QueryContainerDescriptor<TInferDocument>()) : (QueryContainer) null));
    }

    public SearchDescriptor<TInferDocument> Slice(
      Func<SlicedScrollDescriptor<TInferDocument>, ISlicedScroll> selector)
    {
      return this.Assign<Func<SlicedScrollDescriptor<TInferDocument>, ISlicedScroll>>(selector, (Action<ISearchRequest<TInferDocument>, Func<SlicedScrollDescriptor<TInferDocument>, ISlicedScroll>>) ((a, v) => a.Slice = v != null ? v(new SlicedScrollDescriptor<TInferDocument>()) : (ISlicedScroll) null));
    }

    public SearchDescriptor<TInferDocument> MatchAll(
      Func<MatchAllQueryDescriptor, IMatchAllQuery> selector = null)
    {
      return this.Query((Func<QueryContainerDescriptor<TInferDocument>, QueryContainer>) (q => q.MatchAll(selector)));
    }

    public SearchDescriptor<TInferDocument> PostFilter(
      Func<QueryContainerDescriptor<TInferDocument>, QueryContainer> filter)
    {
      return this.Assign<Func<QueryContainerDescriptor<TInferDocument>, QueryContainer>>(filter, (Action<ISearchRequest<TInferDocument>, Func<QueryContainerDescriptor<TInferDocument>, QueryContainer>>) ((a, v) => a.PostFilter = v != null ? v(new QueryContainerDescriptor<TInferDocument>()) : (QueryContainer) null));
    }

    public SearchDescriptor<TInferDocument> Highlight(
      Func<HighlightDescriptor<TInferDocument>, IHighlight> highlightSelector)
    {
      return this.Assign<Func<HighlightDescriptor<TInferDocument>, IHighlight>>(highlightSelector, (Action<ISearchRequest<TInferDocument>, Func<HighlightDescriptor<TInferDocument>, IHighlight>>) ((a, v) => a.Highlight = v != null ? v(new HighlightDescriptor<TInferDocument>()) : (IHighlight) null));
    }

    public SearchDescriptor<TInferDocument> Collapse(
      Func<FieldCollapseDescriptor<TInferDocument>, IFieldCollapse> collapseSelector)
    {
      return this.Assign<Func<FieldCollapseDescriptor<TInferDocument>, IFieldCollapse>>(collapseSelector, (Action<ISearchRequest<TInferDocument>, Func<FieldCollapseDescriptor<TInferDocument>, IFieldCollapse>>) ((a, v) => a.Collapse = v != null ? v(new FieldCollapseDescriptor<TInferDocument>()) : (IFieldCollapse) null));
    }

    public SearchDescriptor<TInferDocument> Rescore(
      Func<RescoringDescriptor<TInferDocument>, IPromise<IList<IRescore>>> rescoreSelector)
    {
      return this.Assign<Func<RescoringDescriptor<TInferDocument>, IPromise<IList<IRescore>>>>(rescoreSelector, (Action<ISearchRequest<TInferDocument>, Func<RescoringDescriptor<TInferDocument>, IPromise<IList<IRescore>>>>) ((a, v) => a.Rescore = v != null ? v(new RescoringDescriptor<TInferDocument>()).Value : (IList<IRescore>) null));
    }

    public SearchDescriptor<TInferDocument> TrackTotalHits(bool? trackTotalHits = true) => this.Assign<bool?>(trackTotalHits, (Action<ISearchRequest<TInferDocument>, bool?>) ((a, v) => a.TrackTotalHits = v));

    public SearchDescriptor<TInferDocument> PointInTime(string pitId)
    {
      this.Self.PointInTime = (IPointInTime) new Nest.PointInTime(pitId);
      return this;
    }

    public SearchDescriptor<TInferDocument> PointInTime(
      string pitId,
      Func<PointInTimeDescriptor, IPointInTime> pit)
    {
      return this.Assign<Func<PointInTimeDescriptor, IPointInTime>>(pit, (Action<ISearchRequest<TInferDocument>, Func<PointInTimeDescriptor, IPointInTime>>) ((a, v) => a.PointInTime = v != null ? v(new PointInTimeDescriptor(pitId)) : (IPointInTime) null));
    }

    public SearchDescriptor<TInferDocument> RuntimeFields(
      Func<RuntimeFieldsDescriptor<TInferDocument>, IPromise<IRuntimeFields>> runtimeFieldsSelector)
    {
      return this.Assign<Func<RuntimeFieldsDescriptor<TInferDocument>, IPromise<IRuntimeFields>>>(runtimeFieldsSelector, (Action<ISearchRequest<TInferDocument>, Func<RuntimeFieldsDescriptor<TInferDocument>, IPromise<IRuntimeFields>>>) ((a, v) => a.RuntimeFields = v != null ? v(new RuntimeFieldsDescriptor<TInferDocument>())?.Value : (IRuntimeFields) null));
    }

    public SearchDescriptor<TInferDocument> RuntimeFields<TSource>(
      Func<RuntimeFieldsDescriptor<TSource>, IPromise<IRuntimeFields>> runtimeFieldsSelector)
      where TSource : class
    {
      return this.Assign<Func<RuntimeFieldsDescriptor<TSource>, IPromise<IRuntimeFields>>>(runtimeFieldsSelector, (Action<ISearchRequest<TInferDocument>, Func<RuntimeFieldsDescriptor<TSource>, IPromise<IRuntimeFields>>>) ((a, v) => a.RuntimeFields = v != null ? v(new RuntimeFieldsDescriptor<TSource>())?.Value : (IRuntimeFields) null));
    }

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
