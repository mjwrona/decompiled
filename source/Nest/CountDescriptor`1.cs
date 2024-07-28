// Decompiled with JetBrains decompiler
// Type: Nest.CountDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using System;

namespace Nest
{
  public class CountDescriptor<TDocument> : 
    RequestDescriptorBase<CountDescriptor<TDocument>, CountRequestParameters, ICountRequest<TDocument>>,
    ICountRequest<TDocument>,
    ICountRequest,
    IRequest<CountRequestParameters>,
    IRequest
    where TDocument : class
  {
    internal override ApiUrls ApiUrls => ApiUrlsLookups.NoNamespaceCount;

    public CountDescriptor()
      : this((Indices) typeof (TDocument))
    {
    }

    public CountDescriptor(Indices index)
      : base((Func<RouteValues, RouteValues>) (r => r.Optional(nameof (index), (IUrlParameter) index)))
    {
    }

    Indices ICountRequest.Index => this.Self.RouteValues.Get<Indices>("index");

    public CountDescriptor<TDocument> Index(Indices index) => this.Assign<Indices>(index, (Action<ICountRequest<TDocument>, Indices>) ((a, v) => a.RouteValues.Optional(nameof (index), (IUrlParameter) v)));

    public CountDescriptor<TDocument> Index<TOther>() where TOther : class => this.Assign<Type>(typeof (TOther), (Action<ICountRequest<TDocument>, Type>) ((a, v) => a.RouteValues.Optional("index", (IUrlParameter) (Indices) v)));

    public CountDescriptor<TDocument> AllIndices() => this.Index(Indices.All);

    public CountDescriptor<TDocument> AllowNoIndices(bool? allownoindices = true) => this.Qs("allow_no_indices", (object) allownoindices);

    public CountDescriptor<TDocument> AnalyzeWildcard(bool? analyzewildcard = true) => this.Qs("analyze_wildcard", (object) analyzewildcard);

    public CountDescriptor<TDocument> Analyzer(string analyzer) => this.Qs(nameof (analyzer), (object) analyzer);

    public CountDescriptor<TDocument> DefaultOperator(Elasticsearch.Net.DefaultOperator? defaultoperator) => this.Qs("default_operator", (object) defaultoperator);

    public CountDescriptor<TDocument> Df(string df) => this.Qs(nameof (df), (object) df);

    public CountDescriptor<TDocument> ExpandWildcards(Elasticsearch.Net.ExpandWildcards? expandwildcards) => this.Qs("expand_wildcards", (object) expandwildcards);

    public CountDescriptor<TDocument> IgnoreThrottled(bool? ignorethrottled = true) => this.Qs("ignore_throttled", (object) ignorethrottled);

    public CountDescriptor<TDocument> IgnoreUnavailable(bool? ignoreunavailable = true) => this.Qs("ignore_unavailable", (object) ignoreunavailable);

    public CountDescriptor<TDocument> Lenient(bool? lenient = true) => this.Qs(nameof (lenient), (object) lenient);

    public CountDescriptor<TDocument> MinScore(double? minscore) => this.Qs("min_score", (object) minscore);

    public CountDescriptor<TDocument> Preference(string preference) => this.Qs(nameof (preference), (object) preference);

    public CountDescriptor<TDocument> QueryOnQueryString(string queryonquerystring) => this.Qs("q", (object) queryonquerystring);

    public CountDescriptor<TDocument> Routing(Nest.Routing routing) => this.Qs(nameof (routing), (object) routing);

    public CountDescriptor<TDocument> TerminateAfter(long? terminateafter) => this.Qs("terminate_after", (object) terminateafter);

    protected override HttpMethod HttpMethod => !this.Self.RequestParameters.ContainsQueryString("source") && !this.Self.RequestParameters.ContainsQueryString("q") && this.Self.Query != null && !this.Self.Query.IsConditionless() ? HttpMethod.POST : HttpMethod.GET;

    QueryContainer ICountRequest.Query { get; set; }

    public CountDescriptor<TDocument> Query(
      Func<QueryContainerDescriptor<TDocument>, QueryContainer> querySelector)
    {
      return this.Assign<Func<QueryContainerDescriptor<TDocument>, QueryContainer>>(querySelector, (Action<ICountRequest<TDocument>, Func<QueryContainerDescriptor<TDocument>, QueryContainer>>) ((a, v) => a.Query = v != null ? v(new QueryContainerDescriptor<TDocument>()) : (QueryContainer) null));
    }
  }
}
