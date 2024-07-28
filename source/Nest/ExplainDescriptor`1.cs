// Decompiled with JetBrains decompiler
// Type: Nest.ExplainDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Utf8Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Nest
{
  public class ExplainDescriptor<TDocument> : 
    RequestDescriptorBase<ExplainDescriptor<TDocument>, ExplainRequestParameters, IExplainRequest<TDocument>>,
    IExplainRequest<TDocument>,
    IExplainRequest,
    IRequest<ExplainRequestParameters>,
    IRequest
    where TDocument : class
  {
    internal override ApiUrls ApiUrls => ApiUrlsLookups.NoNamespaceExplain;

    public ExplainDescriptor(IndexName index, Id id)
      : base((Func<RouteValues, RouteValues>) (r => r.Required(nameof (index), (IUrlParameter) index).Required(nameof (id), (IUrlParameter) id)))
    {
    }

    public ExplainDescriptor(Id id)
      : this((IndexName) typeof (TDocument), id)
    {
    }

    public ExplainDescriptor(TDocument documentWithId, IndexName index = null, Id id = null)
    {
      IndexName index1 = index;
      if ((object) index1 == null)
        index1 = (IndexName) typeof (TDocument);
      Id id1 = id;
      if ((object) id1 == null)
        id1 = Id.From<TDocument>(documentWithId);
      // ISSUE: explicit constructor call
      this.\u002Ector(index1, id1);
    }

    [SerializationConstructor]
    protected ExplainDescriptor()
    {
    }

    IndexName IExplainRequest.Index => this.Self.RouteValues.Get<IndexName>("index");

    Id IExplainRequest.Id => this.Self.RouteValues.Get<Id>("id");

    public ExplainDescriptor<TDocument> Index(IndexName index) => this.Assign<IndexName>(index, (Action<IExplainRequest<TDocument>, IndexName>) ((a, v) => a.RouteValues.Required(nameof (index), (IUrlParameter) v)));

    public ExplainDescriptor<TDocument> Index<TOther>() where TOther : class => this.Assign<Type>(typeof (TOther), (Action<IExplainRequest<TDocument>, Type>) ((a, v) => a.RouteValues.Required("index", (IUrlParameter) (IndexName) v)));

    public ExplainDescriptor<TDocument> AnalyzeWildcard(bool? analyzewildcard = true) => this.Qs("analyze_wildcard", (object) analyzewildcard);

    public ExplainDescriptor<TDocument> Analyzer(string analyzer) => this.Qs(nameof (analyzer), (object) analyzer);

    public ExplainDescriptor<TDocument> DefaultOperator(Elasticsearch.Net.DefaultOperator? defaultoperator) => this.Qs("default_operator", (object) defaultoperator);

    public ExplainDescriptor<TDocument> Df(string df) => this.Qs(nameof (df), (object) df);

    public ExplainDescriptor<TDocument> Lenient(bool? lenient = true) => this.Qs(nameof (lenient), (object) lenient);

    public ExplainDescriptor<TDocument> Preference(string preference) => this.Qs(nameof (preference), (object) preference);

    public ExplainDescriptor<TDocument> QueryOnQueryString(string queryonquerystring) => this.Qs("q", (object) queryonquerystring);

    public ExplainDescriptor<TDocument> Routing(Nest.Routing routing) => this.Qs(nameof (routing), (object) routing);

    public ExplainDescriptor<TDocument> SourceEnabled(bool? sourceenabled = true) => this.Qs("_source", (object) sourceenabled);

    public ExplainDescriptor<TDocument> SourceExcludes(Fields sourceexcludes) => this.Qs("_source_excludes", (object) sourceexcludes);

    public ExplainDescriptor<TDocument> SourceExcludes(
      params Expression<Func<TDocument, object>>[] fields)
    {
      return this.Qs("_source_excludes", fields != null ? (object) ((IEnumerable<Expression<Func<TDocument, object>>>) fields).Select<Expression<Func<TDocument, object>>, Field>((Func<Expression<Func<TDocument, object>>, Field>) (e => (Field) (Expression) e)) : (object) (IEnumerable<Field>) null);
    }

    public ExplainDescriptor<TDocument> SourceIncludes(Fields sourceincludes) => this.Qs("_source_includes", (object) sourceincludes);

    public ExplainDescriptor<TDocument> SourceIncludes(
      params Expression<Func<TDocument, object>>[] fields)
    {
      return this.Qs("_source_includes", fields != null ? (object) ((IEnumerable<Expression<Func<TDocument, object>>>) fields).Select<Expression<Func<TDocument, object>>, Field>((Func<Expression<Func<TDocument, object>>, Field>) (e => (Field) (Expression) e)) : (object) (IEnumerable<Field>) null);
    }

    protected override HttpMethod HttpMethod
    {
      get
      {
        ExplainRequestParameters requestParameters1 = this.RequestState.RequestParameters;
        if ((requestParameters1 != null ? (__nonvirtual (requestParameters1.ContainsQueryString("source")) ? 1 : 0) : 0) == 0)
        {
          ExplainRequestParameters requestParameters2 = this.RequestState.RequestParameters;
          if ((requestParameters2 != null ? (__nonvirtual (requestParameters2.ContainsQueryString("q")) ? 1 : 0) : 0) == 0)
            return HttpMethod.POST;
        }
        return HttpMethod.GET;
      }
    }

    Fields IExplainRequest.StoredFields { get; set; }

    QueryContainer IExplainRequest.Query { get; set; }

    public ExplainDescriptor<TDocument> Query(
      Func<QueryContainerDescriptor<TDocument>, QueryContainer> querySelector)
    {
      return this.Assign<Func<QueryContainerDescriptor<TDocument>, QueryContainer>>(querySelector, (Action<IExplainRequest<TDocument>, Func<QueryContainerDescriptor<TDocument>, QueryContainer>>) ((a, v) => a.Query = v != null ? v(new QueryContainerDescriptor<TDocument>()) : (QueryContainer) null));
    }

    public ExplainDescriptor<TDocument> StoredFields(
      Func<FieldsDescriptor<TDocument>, IPromise<Fields>> fields)
    {
      return this.Assign<Func<FieldsDescriptor<TDocument>, IPromise<Fields>>>(fields, (Action<IExplainRequest<TDocument>, Func<FieldsDescriptor<TDocument>, IPromise<Fields>>>) ((a, v) => a.StoredFields = v != null ? v(new FieldsDescriptor<TDocument>())?.Value : (Fields) null));
    }

    public ExplainDescriptor<TDocument> StoredFields(Fields fields) => this.Assign<Fields>(fields, (Action<IExplainRequest<TDocument>, Fields>) ((a, v) => a.StoredFields = v));
  }
}
