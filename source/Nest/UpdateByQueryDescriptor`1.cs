// Decompiled with JetBrains decompiler
// Type: Nest.UpdateByQueryDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using System;

namespace Nest
{
  public class UpdateByQueryDescriptor<TDocument> : 
    RequestDescriptorBase<UpdateByQueryDescriptor<TDocument>, UpdateByQueryRequestParameters, IUpdateByQueryRequest<TDocument>>,
    IUpdateByQueryRequest<TDocument>,
    IUpdateByQueryRequest,
    IRequest<UpdateByQueryRequestParameters>,
    IRequest
    where TDocument : class
  {
    internal override ApiUrls ApiUrls => ApiUrlsLookups.NoNamespaceUpdateByQuery;

    public UpdateByQueryDescriptor(Indices index)
      : base((Func<RouteValues, RouteValues>) (r => r.Required(nameof (index), (IUrlParameter) index)))
    {
    }

    public UpdateByQueryDescriptor()
      : this((Indices) typeof (TDocument))
    {
    }

    Indices IUpdateByQueryRequest.Index => this.Self.RouteValues.Get<Indices>("index");

    public UpdateByQueryDescriptor<TDocument> Index(Indices index) => this.Assign<Indices>(index, (Action<IUpdateByQueryRequest<TDocument>, Indices>) ((a, v) => a.RouteValues.Required(nameof (index), (IUrlParameter) v)));

    public UpdateByQueryDescriptor<TDocument> Index<TOther>() where TOther : class => this.Assign<Type>(typeof (TOther), (Action<IUpdateByQueryRequest<TDocument>, Type>) ((a, v) => a.RouteValues.Required("index", (IUrlParameter) (Indices) v)));

    public UpdateByQueryDescriptor<TDocument> AllIndices() => this.Index(Indices.All);

    public UpdateByQueryDescriptor<TDocument> AllowNoIndices(bool? allownoindices = true) => this.Qs("allow_no_indices", (object) allownoindices);

    public UpdateByQueryDescriptor<TDocument> AnalyzeWildcard(bool? analyzewildcard = true) => this.Qs("analyze_wildcard", (object) analyzewildcard);

    public UpdateByQueryDescriptor<TDocument> Analyzer(string analyzer) => this.Qs(nameof (analyzer), (object) analyzer);

    public UpdateByQueryDescriptor<TDocument> Conflicts(Elasticsearch.Net.Conflicts? conflicts) => this.Qs(nameof (conflicts), (object) conflicts);

    public UpdateByQueryDescriptor<TDocument> DefaultOperator(Elasticsearch.Net.DefaultOperator? defaultoperator) => this.Qs("default_operator", (object) defaultoperator);

    public UpdateByQueryDescriptor<TDocument> Df(string df) => this.Qs(nameof (df), (object) df);

    public UpdateByQueryDescriptor<TDocument> ExpandWildcards(Elasticsearch.Net.ExpandWildcards? expandwildcards) => this.Qs("expand_wildcards", (object) expandwildcards);

    public UpdateByQueryDescriptor<TDocument> From(long? from) => this.Qs(nameof (from), (object) from);

    public UpdateByQueryDescriptor<TDocument> IgnoreUnavailable(bool? ignoreunavailable = true) => this.Qs("ignore_unavailable", (object) ignoreunavailable);

    public UpdateByQueryDescriptor<TDocument> Lenient(bool? lenient = true) => this.Qs(nameof (lenient), (object) lenient);

    public UpdateByQueryDescriptor<TDocument> Pipeline(string pipeline) => this.Qs(nameof (pipeline), (object) pipeline);

    public UpdateByQueryDescriptor<TDocument> Preference(string preference) => this.Qs(nameof (preference), (object) preference);

    public UpdateByQueryDescriptor<TDocument> QueryOnQueryString(string queryonquerystring) => this.Qs("q", (object) queryonquerystring);

    public UpdateByQueryDescriptor<TDocument> Refresh(bool? refresh = true) => this.Qs(nameof (refresh), (object) refresh);

    public UpdateByQueryDescriptor<TDocument> RequestCache(bool? requestcache = true) => this.Qs("request_cache", (object) requestcache);

    public UpdateByQueryDescriptor<TDocument> RequestsPerSecond(long? requestspersecond) => this.Qs("requests_per_second", (object) requestspersecond);

    public UpdateByQueryDescriptor<TDocument> Routing(Nest.Routing routing) => this.Qs(nameof (routing), (object) routing);

    public UpdateByQueryDescriptor<TDocument> Scroll(Time scroll) => this.Qs(nameof (scroll), (object) scroll);

    public UpdateByQueryDescriptor<TDocument> ScrollSize(long? scrollsize) => this.Qs("scroll_size", (object) scrollsize);

    public UpdateByQueryDescriptor<TDocument> SearchTimeout(Time searchtimeout) => this.Qs("search_timeout", (object) searchtimeout);

    public UpdateByQueryDescriptor<TDocument> SearchType(Elasticsearch.Net.SearchType? searchtype) => this.Qs("search_type", (object) searchtype);

    public UpdateByQueryDescriptor<TDocument> Size(long? size) => this.Qs(nameof (size), (object) size);

    public UpdateByQueryDescriptor<TDocument> Slices(long? slices) => this.Qs(nameof (slices), (object) slices);

    public UpdateByQueryDescriptor<TDocument> Sort(params string[] sort) => this.Qs(nameof (sort), (object) sort);

    public UpdateByQueryDescriptor<TDocument> Stats(params string[] stats) => this.Qs(nameof (stats), (object) stats);

    public UpdateByQueryDescriptor<TDocument> TerminateAfter(long? terminateafter) => this.Qs("terminate_after", (object) terminateafter);

    public UpdateByQueryDescriptor<TDocument> Timeout(Time timeout) => this.Qs(nameof (timeout), (object) timeout);

    public UpdateByQueryDescriptor<TDocument> Version(bool? version = true) => this.Qs(nameof (version), (object) version);

    public UpdateByQueryDescriptor<TDocument> VersionType(bool? versiontype = true) => this.Qs("version_type", (object) versiontype);

    public UpdateByQueryDescriptor<TDocument> WaitForActiveShards(string waitforactiveshards) => this.Qs("wait_for_active_shards", (object) waitforactiveshards);

    public UpdateByQueryDescriptor<TDocument> WaitForCompletion(bool? waitforcompletion = true) => this.Qs("wait_for_completion", (object) waitforcompletion);

    QueryContainer IUpdateByQueryRequest.Query { get; set; }

    IScript IUpdateByQueryRequest.Script { get; set; }

    long? IUpdateByQueryRequest.MaximumDocuments { get; set; }

    ISlicedScroll IUpdateByQueryRequest.Slice { get; set; }

    public UpdateByQueryDescriptor<TDocument> MatchAll() => this.Assign<QueryContainer>(new QueryContainerDescriptor<TDocument>().MatchAll(), (Action<IUpdateByQueryRequest<TDocument>, QueryContainer>) ((a, v) => a.Query = v));

    public UpdateByQueryDescriptor<TDocument> Query(
      Func<QueryContainerDescriptor<TDocument>, QueryContainer> querySelector)
    {
      return this.Assign<Func<QueryContainerDescriptor<TDocument>, QueryContainer>>(querySelector, (Action<IUpdateByQueryRequest<TDocument>, Func<QueryContainerDescriptor<TDocument>, QueryContainer>>) ((a, v) => a.Query = v != null ? v(new QueryContainerDescriptor<TDocument>()) : (QueryContainer) null));
    }

    public UpdateByQueryDescriptor<TDocument> Script(string script) => this.Assign<InlineScript>((InlineScript) script, (Action<IUpdateByQueryRequest<TDocument>, InlineScript>) ((a, v) => a.Script = (IScript) v));

    public UpdateByQueryDescriptor<TDocument> Script(Func<ScriptDescriptor, IScript> scriptSelector) => this.Assign<Func<ScriptDescriptor, IScript>>(scriptSelector, (Action<IUpdateByQueryRequest<TDocument>, Func<ScriptDescriptor, IScript>>) ((a, v) => a.Script = v != null ? v(new ScriptDescriptor()) : (IScript) null));

    public UpdateByQueryDescriptor<TDocument> MaximumDocuments(long? maximumDocuments) => this.Assign<long?>(maximumDocuments, (Action<IUpdateByQueryRequest<TDocument>, long?>) ((a, v) => a.MaximumDocuments = v));

    public UpdateByQueryDescriptor<TDocument> Slice(
      Func<SlicedScrollDescriptor<TDocument>, ISlicedScroll> selector)
    {
      return this.Assign<Func<SlicedScrollDescriptor<TDocument>, ISlicedScroll>>(selector, (Action<IUpdateByQueryRequest<TDocument>, Func<SlicedScrollDescriptor<TDocument>, ISlicedScroll>>) ((a, v) => a.Slice = v != null ? v(new SlicedScrollDescriptor<TDocument>()) : (ISlicedScroll) null));
    }
  }
}
