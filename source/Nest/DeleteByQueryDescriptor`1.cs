// Decompiled with JetBrains decompiler
// Type: Nest.DeleteByQueryDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using System;

namespace Nest
{
  public class DeleteByQueryDescriptor<TDocument> : 
    RequestDescriptorBase<DeleteByQueryDescriptor<TDocument>, DeleteByQueryRequestParameters, IDeleteByQueryRequest<TDocument>>,
    IDeleteByQueryRequest<TDocument>,
    IDeleteByQueryRequest,
    IRequest<DeleteByQueryRequestParameters>,
    IRequest
    where TDocument : class
  {
    internal override ApiUrls ApiUrls => ApiUrlsLookups.NoNamespaceDeleteByQuery;

    public DeleteByQueryDescriptor(Indices index)
      : base((Func<RouteValues, RouteValues>) (r => r.Required(nameof (index), (IUrlParameter) index)))
    {
    }

    public DeleteByQueryDescriptor()
      : this((Indices) typeof (TDocument))
    {
    }

    Indices IDeleteByQueryRequest.Index => this.Self.RouteValues.Get<Indices>("index");

    public DeleteByQueryDescriptor<TDocument> Index(Indices index) => this.Assign<Indices>(index, (Action<IDeleteByQueryRequest<TDocument>, Indices>) ((a, v) => a.RouteValues.Required(nameof (index), (IUrlParameter) v)));

    public DeleteByQueryDescriptor<TDocument> Index<TOther>() where TOther : class => this.Assign<Type>(typeof (TOther), (Action<IDeleteByQueryRequest<TDocument>, Type>) ((a, v) => a.RouteValues.Required("index", (IUrlParameter) (Indices) v)));

    public DeleteByQueryDescriptor<TDocument> AllIndices() => this.Index(Indices.All);

    public DeleteByQueryDescriptor<TDocument> AllowNoIndices(bool? allownoindices = true) => this.Qs("allow_no_indices", (object) allownoindices);

    public DeleteByQueryDescriptor<TDocument> AnalyzeWildcard(bool? analyzewildcard = true) => this.Qs("analyze_wildcard", (object) analyzewildcard);

    public DeleteByQueryDescriptor<TDocument> Analyzer(string analyzer) => this.Qs(nameof (analyzer), (object) analyzer);

    public DeleteByQueryDescriptor<TDocument> Conflicts(Elasticsearch.Net.Conflicts? conflicts) => this.Qs(nameof (conflicts), (object) conflicts);

    public DeleteByQueryDescriptor<TDocument> DefaultOperator(Elasticsearch.Net.DefaultOperator? defaultoperator) => this.Qs("default_operator", (object) defaultoperator);

    public DeleteByQueryDescriptor<TDocument> Df(string df) => this.Qs(nameof (df), (object) df);

    public DeleteByQueryDescriptor<TDocument> ExpandWildcards(Elasticsearch.Net.ExpandWildcards? expandwildcards) => this.Qs("expand_wildcards", (object) expandwildcards);

    public DeleteByQueryDescriptor<TDocument> From(long? from) => this.Qs(nameof (from), (object) from);

    public DeleteByQueryDescriptor<TDocument> IgnoreUnavailable(bool? ignoreunavailable = true) => this.Qs("ignore_unavailable", (object) ignoreunavailable);

    public DeleteByQueryDescriptor<TDocument> Lenient(bool? lenient = true) => this.Qs(nameof (lenient), (object) lenient);

    public DeleteByQueryDescriptor<TDocument> Preference(string preference) => this.Qs(nameof (preference), (object) preference);

    public DeleteByQueryDescriptor<TDocument> QueryOnQueryString(string queryonquerystring) => this.Qs("q", (object) queryonquerystring);

    public DeleteByQueryDescriptor<TDocument> Refresh(bool? refresh = true) => this.Qs(nameof (refresh), (object) refresh);

    public DeleteByQueryDescriptor<TDocument> RequestCache(bool? requestcache = true) => this.Qs("request_cache", (object) requestcache);

    public DeleteByQueryDescriptor<TDocument> RequestsPerSecond(long? requestspersecond) => this.Qs("requests_per_second", (object) requestspersecond);

    public DeleteByQueryDescriptor<TDocument> Routing(Nest.Routing routing) => this.Qs(nameof (routing), (object) routing);

    public DeleteByQueryDescriptor<TDocument> Scroll(Time scroll) => this.Qs(nameof (scroll), (object) scroll);

    public DeleteByQueryDescriptor<TDocument> ScrollSize(long? scrollsize) => this.Qs("scroll_size", (object) scrollsize);

    public DeleteByQueryDescriptor<TDocument> SearchTimeout(Time searchtimeout) => this.Qs("search_timeout", (object) searchtimeout);

    public DeleteByQueryDescriptor<TDocument> SearchType(Elasticsearch.Net.SearchType? searchtype) => this.Qs("search_type", (object) searchtype);

    public DeleteByQueryDescriptor<TDocument> Size(long? size) => this.Qs(nameof (size), (object) size);

    public DeleteByQueryDescriptor<TDocument> Slices(long? slices) => this.Qs(nameof (slices), (object) slices);

    public DeleteByQueryDescriptor<TDocument> Sort(params string[] sort) => this.Qs(nameof (sort), (object) sort);

    public DeleteByQueryDescriptor<TDocument> Stats(params string[] stats) => this.Qs(nameof (stats), (object) stats);

    public DeleteByQueryDescriptor<TDocument> TerminateAfter(long? terminateafter) => this.Qs("terminate_after", (object) terminateafter);

    public DeleteByQueryDescriptor<TDocument> Timeout(Time timeout) => this.Qs(nameof (timeout), (object) timeout);

    public DeleteByQueryDescriptor<TDocument> Version(bool? version = true) => this.Qs(nameof (version), (object) version);

    public DeleteByQueryDescriptor<TDocument> WaitForActiveShards(string waitforactiveshards) => this.Qs("wait_for_active_shards", (object) waitforactiveshards);

    public DeleteByQueryDescriptor<TDocument> WaitForCompletion(bool? waitforcompletion = true) => this.Qs("wait_for_completion", (object) waitforcompletion);

    QueryContainer IDeleteByQueryRequest.Query { get; set; }

    ISlicedScroll IDeleteByQueryRequest.Slice { get; set; }

    long? IDeleteByQueryRequest.MaximumDocuments { get; set; }

    public DeleteByQueryDescriptor<TDocument> MatchAll() => this.Assign<QueryContainer>(new QueryContainerDescriptor<TDocument>().MatchAll(), (Action<IDeleteByQueryRequest<TDocument>, QueryContainer>) ((a, v) => a.Query = v));

    public DeleteByQueryDescriptor<TDocument> Query(
      Func<QueryContainerDescriptor<TDocument>, QueryContainer> querySelector)
    {
      return this.Assign<Func<QueryContainerDescriptor<TDocument>, QueryContainer>>(querySelector, (Action<IDeleteByQueryRequest<TDocument>, Func<QueryContainerDescriptor<TDocument>, QueryContainer>>) ((a, v) => a.Query = v != null ? v(new QueryContainerDescriptor<TDocument>()) : (QueryContainer) null));
    }

    public DeleteByQueryDescriptor<TDocument> Slice(
      Func<SlicedScrollDescriptor<TDocument>, ISlicedScroll> selector)
    {
      return this.Assign<Func<SlicedScrollDescriptor<TDocument>, ISlicedScroll>>(selector, (Action<IDeleteByQueryRequest<TDocument>, Func<SlicedScrollDescriptor<TDocument>, ISlicedScroll>>) ((a, v) => a.Slice = v != null ? v(new SlicedScrollDescriptor<TDocument>()) : (ISlicedScroll) null));
    }

    public DeleteByQueryDescriptor<TDocument> MaximumDocuments(long? maximumDocuments) => this.Assign<long?>(maximumDocuments, (Action<IDeleteByQueryRequest<TDocument>, long?>) ((a, v) => a.MaximumDocuments = v));
  }
}
