// Decompiled with JetBrains decompiler
// Type: Nest.AsyncSearchSubmitDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.AsyncSearchApi;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Nest
{
  public class AsyncSearchSubmitDescriptor<TInferDocument> : 
    RequestDescriptorBase<AsyncSearchSubmitDescriptor<TInferDocument>, AsyncSearchSubmitRequestParameters, IAsyncSearchSubmitRequest<TInferDocument>>,
    IAsyncSearchSubmitRequest<TInferDocument>,
    IAsyncSearchSubmitRequest,
    IRequest<AsyncSearchSubmitRequestParameters>,
    IRequest,
    ITypedSearchRequest
    where TInferDocument : class
  {
    internal override ApiUrls ApiUrls => ApiUrlsLookups.AsyncSearchSubmit;

    public AsyncSearchSubmitDescriptor()
      : this((Indices) typeof (TInferDocument))
    {
    }

    public AsyncSearchSubmitDescriptor(Indices index)
      : base((Func<RouteValues, RouteValues>) (r => r.Optional(nameof (index), (IUrlParameter) index)))
    {
    }

    Indices IAsyncSearchSubmitRequest.Index => this.Self.RouteValues.Get<Indices>("index");

    public AsyncSearchSubmitDescriptor<TInferDocument> Index(Indices index) => this.Assign<Indices>(index, (Action<IAsyncSearchSubmitRequest<TInferDocument>, Indices>) ((a, v) => a.RouteValues.Optional(nameof (index), (IUrlParameter) v)));

    public AsyncSearchSubmitDescriptor<TInferDocument> Index<TOther>() where TOther : class => this.Assign<Type>(typeof (TOther), (Action<IAsyncSearchSubmitRequest<TInferDocument>, Type>) ((a, v) => a.RouteValues.Optional("index", (IUrlParameter) (Indices) v)));

    public AsyncSearchSubmitDescriptor<TInferDocument> AllIndices() => this.Index(Indices.All);

    public AsyncSearchSubmitDescriptor<TInferDocument> AllowNoIndices(bool? allownoindices = true) => this.Qs("allow_no_indices", (object) allownoindices);

    public AsyncSearchSubmitDescriptor<TInferDocument> AllowPartialSearchResults(
      bool? allowpartialsearchresults = true)
    {
      return this.Qs("allow_partial_search_results", (object) allowpartialsearchresults);
    }

    public AsyncSearchSubmitDescriptor<TInferDocument> AnalyzeWildcard(bool? analyzewildcard = true) => this.Qs("analyze_wildcard", (object) analyzewildcard);

    public AsyncSearchSubmitDescriptor<TInferDocument> Analyzer(string analyzer) => this.Qs(nameof (analyzer), (object) analyzer);

    public AsyncSearchSubmitDescriptor<TInferDocument> BatchedReduceSize(long? batchedreducesize) => this.Qs("batched_reduce_size", (object) batchedreducesize);

    public AsyncSearchSubmitDescriptor<TInferDocument> DefaultOperator(
      Elasticsearch.Net.DefaultOperator? defaultoperator)
    {
      return this.Qs("default_operator", (object) defaultoperator);
    }

    public AsyncSearchSubmitDescriptor<TInferDocument> Df(string df) => this.Qs(nameof (df), (object) df);

    public AsyncSearchSubmitDescriptor<TInferDocument> ExpandWildcards(
      Elasticsearch.Net.ExpandWildcards? expandwildcards)
    {
      return this.Qs("expand_wildcards", (object) expandwildcards);
    }

    public AsyncSearchSubmitDescriptor<TInferDocument> IgnoreThrottled(bool? ignorethrottled = true) => this.Qs("ignore_throttled", (object) ignorethrottled);

    public AsyncSearchSubmitDescriptor<TInferDocument> IgnoreUnavailable(bool? ignoreunavailable = true) => this.Qs("ignore_unavailable", (object) ignoreunavailable);

    public AsyncSearchSubmitDescriptor<TInferDocument> KeepAlive(Time keepalive) => this.Qs("keep_alive", (object) keepalive);

    public AsyncSearchSubmitDescriptor<TInferDocument> KeepOnCompletion(bool? keeponcompletion = true) => this.Qs("keep_on_completion", (object) keeponcompletion);

    public AsyncSearchSubmitDescriptor<TInferDocument> Lenient(bool? lenient = true) => this.Qs(nameof (lenient), (object) lenient);

    public AsyncSearchSubmitDescriptor<TInferDocument> MaxConcurrentShardRequests(
      long? maxconcurrentshardrequests)
    {
      return this.Qs("max_concurrent_shard_requests", (object) maxconcurrentshardrequests);
    }

    public AsyncSearchSubmitDescriptor<TInferDocument> Preference(string preference) => this.Qs(nameof (preference), (object) preference);

    public AsyncSearchSubmitDescriptor<TInferDocument> QueryOnQueryString(string queryonquerystring) => this.Qs("q", (object) queryonquerystring);

    public AsyncSearchSubmitDescriptor<TInferDocument> RequestCache(bool? requestcache = true) => this.Qs("request_cache", (object) requestcache);

    public AsyncSearchSubmitDescriptor<TInferDocument> Routing(Nest.Routing routing) => this.Qs(nameof (routing), (object) routing);

    public AsyncSearchSubmitDescriptor<TInferDocument> SearchType(Elasticsearch.Net.SearchType? searchtype) => this.Qs("search_type", (object) searchtype);

    public AsyncSearchSubmitDescriptor<TInferDocument> SequenceNumberPrimaryTerm(
      bool? sequencenumberprimaryterm = true)
    {
      return this.Qs("seq_no_primary_term", (object) sequencenumberprimaryterm);
    }

    public AsyncSearchSubmitDescriptor<TInferDocument> Stats(params string[] stats) => this.Qs(nameof (stats), (object) stats);

    public AsyncSearchSubmitDescriptor<TInferDocument> SuggestField(Field suggestfield) => this.Qs("suggest_field", (object) suggestfield);

    public AsyncSearchSubmitDescriptor<TInferDocument> SuggestField(
      Expression<Func<TInferDocument, object>> field)
    {
      return this.Qs("suggest_field", (object) (Field) (Expression) field);
    }

    public AsyncSearchSubmitDescriptor<TInferDocument> SuggestMode(Elasticsearch.Net.SuggestMode? suggestmode) => this.Qs("suggest_mode", (object) suggestmode);

    public AsyncSearchSubmitDescriptor<TInferDocument> SuggestSize(long? suggestsize) => this.Qs("suggest_size", (object) suggestsize);

    public AsyncSearchSubmitDescriptor<TInferDocument> SuggestText(string suggesttext) => this.Qs("suggest_text", (object) suggesttext);

    public AsyncSearchSubmitDescriptor<TInferDocument> TypedKeys(bool? typedkeys = true) => this.Qs("typed_keys", (object) typedkeys);

    public AsyncSearchSubmitDescriptor<TInferDocument> WaitForCompletionTimeout(
      Time waitforcompletiontimeout)
    {
      return this.Qs("wait_for_completion_timeout", (object) waitforcompletiontimeout);
    }

    AggregationDictionary IAsyncSearchSubmitRequest.Aggregations { get; set; }

    Type ITypedSearchRequest.ClrType => typeof (TInferDocument);

    IFieldCollapse IAsyncSearchSubmitRequest.Collapse { get; set; }

    Nest.Fields IAsyncSearchSubmitRequest.DocValueFields { get; set; }

    bool? IAsyncSearchSubmitRequest.Explain { get; set; }

    Nest.Fields IAsyncSearchSubmitRequest.Fields { get; set; }

    int? IAsyncSearchSubmitRequest.From { get; set; }

    IHighlight IAsyncSearchSubmitRequest.Highlight { get; set; }

    IDictionary<IndexName, double> IAsyncSearchSubmitRequest.IndicesBoost { get; set; }

    double? IAsyncSearchSubmitRequest.MinScore { get; set; }

    QueryContainer IAsyncSearchSubmitRequest.PostFilter { get; set; }

    bool? IAsyncSearchSubmitRequest.Profile { get; set; }

    QueryContainer IAsyncSearchSubmitRequest.Query { get; set; }

    IList<IRescore> IAsyncSearchSubmitRequest.Rescore { get; set; }

    IScriptFields IAsyncSearchSubmitRequest.ScriptFields { get; set; }

    IList<object> IAsyncSearchSubmitRequest.SearchAfter { get; set; }

    int? IAsyncSearchSubmitRequest.Size { get; set; }

    IList<ISort> IAsyncSearchSubmitRequest.Sort { get; set; }

    Union<bool, ISourceFilter> IAsyncSearchSubmitRequest.Source { get; set; }

    Nest.Fields IAsyncSearchSubmitRequest.StoredFields { get; set; }

    ISuggestContainer IAsyncSearchSubmitRequest.Suggest { get; set; }

    long? IAsyncSearchSubmitRequest.TerminateAfter { get; set; }

    string IAsyncSearchSubmitRequest.Timeout { get; set; }

    bool? IAsyncSearchSubmitRequest.TrackScores { get; set; }

    bool? IAsyncSearchSubmitRequest.TrackTotalHits { get; set; }

    bool? IAsyncSearchSubmitRequest.Version { get; set; }

    IPointInTime IAsyncSearchSubmitRequest.PointInTime { get; set; }

    IRuntimeFields IAsyncSearchSubmitRequest.RuntimeFields { get; set; }

    protected override sealed void RequestDefaults(AsyncSearchSubmitRequestParameters parameters) => this.TypedKeys();

    public AsyncSearchSubmitDescriptor<TInferDocument> Aggregations(
      Func<AggregationContainerDescriptor<TInferDocument>, IAggregationContainer> aggregationsSelector)
    {
      return this.Assign<AggregationDictionary>(aggregationsSelector(new AggregationContainerDescriptor<TInferDocument>())?.Aggregations, (Action<IAsyncSearchSubmitRequest<TInferDocument>, AggregationDictionary>) ((a, v) => a.Aggregations = v));
    }

    public AsyncSearchSubmitDescriptor<TInferDocument> Aggregations(
      AggregationDictionary aggregations)
    {
      return this.Assign<AggregationDictionary>(aggregations, (Action<IAsyncSearchSubmitRequest<TInferDocument>, AggregationDictionary>) ((a, v) => a.Aggregations = v));
    }

    public AsyncSearchSubmitDescriptor<TInferDocument> Source(bool? enabled = true) => this.Assign<bool?>(enabled, (Action<IAsyncSearchSubmitRequest<TInferDocument>, bool?>) ((a, v) =>
    {
      IAsyncSearchSubmitRequest<TInferDocument> searchSubmitRequest = a;
      bool? nullable = v;
      Union<bool, ISourceFilter> valueOrDefault = nullable.HasValue ? (Union<bool, ISourceFilter>) nullable.GetValueOrDefault() : (Union<bool, ISourceFilter>) null;
      searchSubmitRequest.Source = valueOrDefault;
    }));

    public AsyncSearchSubmitDescriptor<TInferDocument> Source(
      Func<SourceFilterDescriptor<TInferDocument>, ISourceFilter> selector)
    {
      return this.Assign<Func<SourceFilterDescriptor<TInferDocument>, ISourceFilter>>(selector, (Action<IAsyncSearchSubmitRequest<TInferDocument>, Func<SourceFilterDescriptor<TInferDocument>, ISourceFilter>>) ((a, v) => a.Source = new Union<bool, ISourceFilter>(v != null ? v(new SourceFilterDescriptor<TInferDocument>()) : (ISourceFilter) null)));
    }

    public AsyncSearchSubmitDescriptor<TInferDocument> Size(int? size) => this.Assign<int?>(size, (Action<IAsyncSearchSubmitRequest<TInferDocument>, int?>) ((a, v) => a.Size = v));

    public AsyncSearchSubmitDescriptor<TInferDocument> Take(int? take) => this.Size(take);

    public AsyncSearchSubmitDescriptor<TInferDocument> Fields(
      Func<FieldsDescriptor<TInferDocument>, IPromise<Nest.Fields>> fields)
    {
      return this.Assign<Func<FieldsDescriptor<TInferDocument>, IPromise<Nest.Fields>>>(fields, (Action<IAsyncSearchSubmitRequest<TInferDocument>, Func<FieldsDescriptor<TInferDocument>, IPromise<Nest.Fields>>>) ((a, v) => a.Fields = v != null ? v(new FieldsDescriptor<TInferDocument>())?.Value : (Nest.Fields) null));
    }

    public AsyncSearchSubmitDescriptor<TInferDocument> Fields<TSource>(
      Func<FieldsDescriptor<TSource>, IPromise<Nest.Fields>> fields)
      where TSource : class
    {
      return this.Assign<Func<FieldsDescriptor<TSource>, IPromise<Nest.Fields>>>(fields, (Action<IAsyncSearchSubmitRequest<TInferDocument>, Func<FieldsDescriptor<TSource>, IPromise<Nest.Fields>>>) ((a, v) => a.Fields = v != null ? v(new FieldsDescriptor<TSource>())?.Value : (Nest.Fields) null));
    }

    public AsyncSearchSubmitDescriptor<TInferDocument> Fields(Nest.Fields fields) => this.Assign<Nest.Fields>(fields, (Action<IAsyncSearchSubmitRequest<TInferDocument>, Nest.Fields>) ((a, v) => a.DocValueFields = v));

    public AsyncSearchSubmitDescriptor<TInferDocument> From(int? from) => this.Assign<int?>(from, (Action<IAsyncSearchSubmitRequest<TInferDocument>, int?>) ((a, v) => a.From = v));

    public AsyncSearchSubmitDescriptor<TInferDocument> Skip(int? skip) => this.From(skip);

    public AsyncSearchSubmitDescriptor<TInferDocument> Timeout(string timeout) => this.Assign<string>(timeout, (Action<IAsyncSearchSubmitRequest<TInferDocument>, string>) ((a, v) => a.Timeout = v));

    public AsyncSearchSubmitDescriptor<TInferDocument> Explain(bool? explain = true) => this.Assign<bool?>(explain, (Action<IAsyncSearchSubmitRequest<TInferDocument>, bool?>) ((a, v) => a.Explain = v));

    public AsyncSearchSubmitDescriptor<TInferDocument> Version(bool? version = true) => this.Assign<bool?>(version, (Action<IAsyncSearchSubmitRequest<TInferDocument>, bool?>) ((a, v) => a.Version = v));

    public AsyncSearchSubmitDescriptor<TInferDocument> TrackScores(bool? trackscores = true) => this.Assign<bool?>(trackscores, (Action<IAsyncSearchSubmitRequest<TInferDocument>, bool?>) ((a, v) => a.TrackScores = v));

    public AsyncSearchSubmitDescriptor<TInferDocument> Profile(bool? profile = true) => this.Assign<bool?>(profile, (Action<IAsyncSearchSubmitRequest<TInferDocument>, bool?>) ((a, v) => a.Profile = v));

    public AsyncSearchSubmitDescriptor<TInferDocument> MinScore(double? minScore) => this.Assign<double?>(minScore, (Action<IAsyncSearchSubmitRequest<TInferDocument>, double?>) ((a, v) => a.MinScore = v));

    public AsyncSearchSubmitDescriptor<TInferDocument> TerminateAfter(long? terminateAfter) => this.Assign<long?>(terminateAfter, (Action<IAsyncSearchSubmitRequest<TInferDocument>, long?>) ((a, v) => a.TerminateAfter = v));

    public AsyncSearchSubmitDescriptor<TInferDocument> ExecuteOnLocalShard() => this.Preference("_local");

    public AsyncSearchSubmitDescriptor<TInferDocument> ExecuteOnNode(string node) => this.Preference("_only_node:" + node);

    public AsyncSearchSubmitDescriptor<TInferDocument> ExecuteOnPreferredNode(string node) => this.Preference(node.IsNullOrEmpty() ? (string) null : "_prefer_node:" + node);

    public AsyncSearchSubmitDescriptor<TInferDocument> IndicesBoost(
      Func<FluentDictionary<IndexName, double>, FluentDictionary<IndexName, double>> boost)
    {
      return this.Assign<Func<FluentDictionary<IndexName, double>, FluentDictionary<IndexName, double>>>(boost, (Action<IAsyncSearchSubmitRequest<TInferDocument>, Func<FluentDictionary<IndexName, double>, FluentDictionary<IndexName, double>>>) ((a, v) => a.IndicesBoost = v != null ? (IDictionary<IndexName, double>) v(new FluentDictionary<IndexName, double>()) : (IDictionary<IndexName, double>) null));
    }

    public AsyncSearchSubmitDescriptor<TInferDocument> StoredFields(
      Func<FieldsDescriptor<TInferDocument>, IPromise<Nest.Fields>> fields)
    {
      return this.Assign<Func<FieldsDescriptor<TInferDocument>, IPromise<Nest.Fields>>>(fields, (Action<IAsyncSearchSubmitRequest<TInferDocument>, Func<FieldsDescriptor<TInferDocument>, IPromise<Nest.Fields>>>) ((a, v) => a.StoredFields = v != null ? v(new FieldsDescriptor<TInferDocument>())?.Value : (Nest.Fields) null));
    }

    public AsyncSearchSubmitDescriptor<TInferDocument> StoredFields(Nest.Fields fields) => this.Assign<Nest.Fields>(fields, (Action<IAsyncSearchSubmitRequest<TInferDocument>, Nest.Fields>) ((a, v) => a.StoredFields = v));

    public AsyncSearchSubmitDescriptor<TInferDocument> ScriptFields(
      Func<ScriptFieldsDescriptor, IPromise<IScriptFields>> selector)
    {
      return this.Assign<Func<ScriptFieldsDescriptor, IPromise<IScriptFields>>>(selector, (Action<IAsyncSearchSubmitRequest<TInferDocument>, Func<ScriptFieldsDescriptor, IPromise<IScriptFields>>>) ((a, v) => a.ScriptFields = v != null ? v(new ScriptFieldsDescriptor())?.Value : (IScriptFields) null));
    }

    public AsyncSearchSubmitDescriptor<TInferDocument> DocValueFields(
      Func<FieldsDescriptor<TInferDocument>, IPromise<Nest.Fields>> fields)
    {
      return this.Assign<Func<FieldsDescriptor<TInferDocument>, IPromise<Nest.Fields>>>(fields, (Action<IAsyncSearchSubmitRequest<TInferDocument>, Func<FieldsDescriptor<TInferDocument>, IPromise<Nest.Fields>>>) ((a, v) => a.DocValueFields = v != null ? v(new FieldsDescriptor<TInferDocument>())?.Value : (Nest.Fields) null));
    }

    public AsyncSearchSubmitDescriptor<TInferDocument> DocValueFields(Nest.Fields fields) => this.Assign<Nest.Fields>(fields, (Action<IAsyncSearchSubmitRequest<TInferDocument>, Nest.Fields>) ((a, v) => a.DocValueFields = v));

    public AsyncSearchSubmitDescriptor<TInferDocument> Sort(
      Func<SortDescriptor<TInferDocument>, IPromise<IList<ISort>>> selector)
    {
      return this.Assign<Func<SortDescriptor<TInferDocument>, IPromise<IList<ISort>>>>(selector, (Action<IAsyncSearchSubmitRequest<TInferDocument>, Func<SortDescriptor<TInferDocument>, IPromise<IList<ISort>>>>) ((a, v) => a.Sort = v != null ? v(new SortDescriptor<TInferDocument>())?.Value : (IList<ISort>) null));
    }

    public AsyncSearchSubmitDescriptor<TInferDocument> SearchAfter(IList<object> searchAfter) => this.Assign<IList<object>>(searchAfter, (Action<IAsyncSearchSubmitRequest<TInferDocument>, IList<object>>) ((a, v) => a.SearchAfter = v));

    public AsyncSearchSubmitDescriptor<TInferDocument> SearchAfter(params object[] searchAfter) => this.Assign<object[]>(searchAfter, (Action<IAsyncSearchSubmitRequest<TInferDocument>, object[]>) ((a, v) => a.SearchAfter = (IList<object>) v));

    public AsyncSearchSubmitDescriptor<TInferDocument> Suggest(
      Func<SuggestContainerDescriptor<TInferDocument>, IPromise<ISuggestContainer>> selector)
    {
      return this.Assign<Func<SuggestContainerDescriptor<TInferDocument>, IPromise<ISuggestContainer>>>(selector, (Action<IAsyncSearchSubmitRequest<TInferDocument>, Func<SuggestContainerDescriptor<TInferDocument>, IPromise<ISuggestContainer>>>) ((a, v) => a.Suggest = v != null ? v(new SuggestContainerDescriptor<TInferDocument>())?.Value : (ISuggestContainer) null));
    }

    public AsyncSearchSubmitDescriptor<TInferDocument> Query(
      Func<QueryContainerDescriptor<TInferDocument>, QueryContainer> query)
    {
      return this.Assign<Func<QueryContainerDescriptor<TInferDocument>, QueryContainer>>(query, (Action<IAsyncSearchSubmitRequest<TInferDocument>, Func<QueryContainerDescriptor<TInferDocument>, QueryContainer>>) ((a, v) => a.Query = v != null ? v(new QueryContainerDescriptor<TInferDocument>()) : (QueryContainer) null));
    }

    public AsyncSearchSubmitDescriptor<TInferDocument> MatchAll(
      Func<MatchAllQueryDescriptor, IMatchAllQuery> selector = null)
    {
      return this.Query((Func<QueryContainerDescriptor<TInferDocument>, QueryContainer>) (q => q.MatchAll(selector)));
    }

    public AsyncSearchSubmitDescriptor<TInferDocument> PostFilter(
      Func<QueryContainerDescriptor<TInferDocument>, QueryContainer> filter)
    {
      return this.Assign<Func<QueryContainerDescriptor<TInferDocument>, QueryContainer>>(filter, (Action<IAsyncSearchSubmitRequest<TInferDocument>, Func<QueryContainerDescriptor<TInferDocument>, QueryContainer>>) ((a, v) => a.PostFilter = v != null ? v(new QueryContainerDescriptor<TInferDocument>()) : (QueryContainer) null));
    }

    public AsyncSearchSubmitDescriptor<TInferDocument> Highlight(
      Func<HighlightDescriptor<TInferDocument>, IHighlight> highlightSelector)
    {
      return this.Assign<Func<HighlightDescriptor<TInferDocument>, IHighlight>>(highlightSelector, (Action<IAsyncSearchSubmitRequest<TInferDocument>, Func<HighlightDescriptor<TInferDocument>, IHighlight>>) ((a, v) => a.Highlight = v != null ? v(new HighlightDescriptor<TInferDocument>()) : (IHighlight) null));
    }

    public AsyncSearchSubmitDescriptor<TInferDocument> Collapse(
      Func<FieldCollapseDescriptor<TInferDocument>, IFieldCollapse> collapseSelector)
    {
      return this.Assign<Func<FieldCollapseDescriptor<TInferDocument>, IFieldCollapse>>(collapseSelector, (Action<IAsyncSearchSubmitRequest<TInferDocument>, Func<FieldCollapseDescriptor<TInferDocument>, IFieldCollapse>>) ((a, v) => a.Collapse = v != null ? v(new FieldCollapseDescriptor<TInferDocument>()) : (IFieldCollapse) null));
    }

    public AsyncSearchSubmitDescriptor<TInferDocument> Rescore(
      Func<RescoringDescriptor<TInferDocument>, IPromise<IList<IRescore>>> rescoreSelector)
    {
      return this.Assign<Func<RescoringDescriptor<TInferDocument>, IPromise<IList<IRescore>>>>(rescoreSelector, (Action<IAsyncSearchSubmitRequest<TInferDocument>, Func<RescoringDescriptor<TInferDocument>, IPromise<IList<IRescore>>>>) ((a, v) => a.Rescore = v != null ? v(new RescoringDescriptor<TInferDocument>()).Value : (IList<IRescore>) null));
    }

    public AsyncSearchSubmitDescriptor<TInferDocument> TrackTotalHits(bool? trackTotalHits = true) => this.Assign<bool?>(trackTotalHits, (Action<IAsyncSearchSubmitRequest<TInferDocument>, bool?>) ((a, v) => a.TrackTotalHits = v));

    public AsyncSearchSubmitDescriptor<TInferDocument> PointInTime(string pitId)
    {
      this.Self.PointInTime = (IPointInTime) new Nest.PointInTime(pitId);
      return this;
    }

    public AsyncSearchSubmitDescriptor<TInferDocument> PointInTime(
      string pitId,
      Func<PointInTimeDescriptor, IPointInTime> pit)
    {
      return this.Assign<Func<PointInTimeDescriptor, IPointInTime>>(pit, (Action<IAsyncSearchSubmitRequest<TInferDocument>, Func<PointInTimeDescriptor, IPointInTime>>) ((a, v) => a.PointInTime = v != null ? v(new PointInTimeDescriptor(pitId)) : (IPointInTime) null));
    }

    public AsyncSearchSubmitDescriptor<TInferDocument> RuntimeFields(
      Func<RuntimeFieldsDescriptor<TInferDocument>, IPromise<IRuntimeFields>> runtimeFieldsSelector)
    {
      return this.Assign<Func<RuntimeFieldsDescriptor<TInferDocument>, IPromise<IRuntimeFields>>>(runtimeFieldsSelector, (Action<IAsyncSearchSubmitRequest<TInferDocument>, Func<RuntimeFieldsDescriptor<TInferDocument>, IPromise<IRuntimeFields>>>) ((a, v) => a.RuntimeFields = v != null ? v(new RuntimeFieldsDescriptor<TInferDocument>())?.Value : (IRuntimeFields) null));
    }

    public AsyncSearchSubmitDescriptor<TInferDocument> RuntimeFields<TSource>(
      Func<RuntimeFieldsDescriptor<TSource>, IPromise<IRuntimeFields>> runtimeFieldsSelector)
      where TSource : class
    {
      return this.Assign<Func<RuntimeFieldsDescriptor<TSource>, IPromise<IRuntimeFields>>>(runtimeFieldsSelector, (Action<IAsyncSearchSubmitRequest<TInferDocument>, Func<RuntimeFieldsDescriptor<TSource>, IPromise<IRuntimeFields>>>) ((a, v) => a.RuntimeFields = v != null ? v(new RuntimeFieldsDescriptor<TSource>())?.Value : (IRuntimeFields) null));
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
