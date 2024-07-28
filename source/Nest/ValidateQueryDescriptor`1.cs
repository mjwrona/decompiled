// Decompiled with JetBrains decompiler
// Type: Nest.ValidateQueryDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.IndicesApi;
using System;

namespace Nest
{
  public class ValidateQueryDescriptor<TDocument> : 
    RequestDescriptorBase<ValidateQueryDescriptor<TDocument>, ValidateQueryRequestParameters, IValidateQueryRequest<TDocument>>,
    IValidateQueryRequest<TDocument>,
    IValidateQueryRequest,
    IRequest<ValidateQueryRequestParameters>,
    IRequest
    where TDocument : class
  {
    internal override ApiUrls ApiUrls => ApiUrlsLookups.IndicesValidateQuery;

    public ValidateQueryDescriptor()
      : this((Indices) typeof (TDocument))
    {
    }

    public ValidateQueryDescriptor(Indices index)
      : base((Func<RouteValues, RouteValues>) (r => r.Optional(nameof (index), (IUrlParameter) index)))
    {
    }

    Indices IValidateQueryRequest.Index => this.Self.RouteValues.Get<Indices>("index");

    public ValidateQueryDescriptor<TDocument> Index(Indices index) => this.Assign<Indices>(index, (Action<IValidateQueryRequest<TDocument>, Indices>) ((a, v) => a.RouteValues.Optional(nameof (index), (IUrlParameter) v)));

    public ValidateQueryDescriptor<TDocument> Index<TOther>() where TOther : class => this.Assign<Type>(typeof (TOther), (Action<IValidateQueryRequest<TDocument>, Type>) ((a, v) => a.RouteValues.Optional("index", (IUrlParameter) (Indices) v)));

    public ValidateQueryDescriptor<TDocument> AllIndices() => this.Index(Indices.All);

    public ValidateQueryDescriptor<TDocument> AllShards(bool? allshards = true) => this.Qs("all_shards", (object) allshards);

    public ValidateQueryDescriptor<TDocument> AllowNoIndices(bool? allownoindices = true) => this.Qs("allow_no_indices", (object) allownoindices);

    public ValidateQueryDescriptor<TDocument> AnalyzeWildcard(bool? analyzewildcard = true) => this.Qs("analyze_wildcard", (object) analyzewildcard);

    public ValidateQueryDescriptor<TDocument> Analyzer(string analyzer) => this.Qs(nameof (analyzer), (object) analyzer);

    public ValidateQueryDescriptor<TDocument> DefaultOperator(Elasticsearch.Net.DefaultOperator? defaultoperator) => this.Qs("default_operator", (object) defaultoperator);

    public ValidateQueryDescriptor<TDocument> Df(string df) => this.Qs(nameof (df), (object) df);

    public ValidateQueryDescriptor<TDocument> ExpandWildcards(Elasticsearch.Net.ExpandWildcards? expandwildcards) => this.Qs("expand_wildcards", (object) expandwildcards);

    public ValidateQueryDescriptor<TDocument> Explain(bool? explain = true) => this.Qs(nameof (explain), (object) explain);

    public ValidateQueryDescriptor<TDocument> IgnoreUnavailable(bool? ignoreunavailable = true) => this.Qs("ignore_unavailable", (object) ignoreunavailable);

    public ValidateQueryDescriptor<TDocument> Lenient(bool? lenient = true) => this.Qs(nameof (lenient), (object) lenient);

    public ValidateQueryDescriptor<TDocument> QueryOnQueryString(string queryonquerystring) => this.Qs("q", (object) queryonquerystring);

    public ValidateQueryDescriptor<TDocument> Rewrite(bool? rewrite = true) => this.Qs(nameof (rewrite), (object) rewrite);

    QueryContainer IValidateQueryRequest.Query { get; set; }

    public ValidateQueryDescriptor<TDocument> Query(
      Func<QueryContainerDescriptor<TDocument>, QueryContainer> querySelector)
    {
      return this.Assign<Func<QueryContainerDescriptor<TDocument>, QueryContainer>>(querySelector, (Action<IValidateQueryRequest<TDocument>, Func<QueryContainerDescriptor<TDocument>, QueryContainer>>) ((a, v) => a.Query = v != null ? v(new QueryContainerDescriptor<TDocument>()) : (QueryContainer) null));
    }
  }
}
