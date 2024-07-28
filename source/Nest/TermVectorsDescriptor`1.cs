// Decompiled with JetBrains decompiler
// Type: Nest.TermVectorsDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Nest
{
  public class TermVectorsDescriptor<TDocument> : 
    RequestDescriptorBase<TermVectorsDescriptor<TDocument>, TermVectorsRequestParameters, ITermVectorsRequest<TDocument>>,
    ITermVectorsRequest<TDocument>,
    IRequest<TermVectorsRequestParameters>,
    IRequest
    where TDocument : class
  {
    internal override ApiUrls ApiUrls => ApiUrlsLookups.NoNamespaceTermVectors;

    public TermVectorsDescriptor(IndexName index, Nest.Id id)
      : base((Func<RouteValues, RouteValues>) (r => r.Required(nameof (index), (IUrlParameter) index).Optional(nameof (id), (IUrlParameter) id)))
    {
    }

    public TermVectorsDescriptor(IndexName index)
      : base((Func<RouteValues, RouteValues>) (r => r.Required(nameof (index), (IUrlParameter) index)))
    {
    }

    public TermVectorsDescriptor(Nest.Id id)
      : this((IndexName) typeof (TDocument), id)
    {
    }

    public TermVectorsDescriptor()
      : this((IndexName) typeof (TDocument))
    {
    }

    public TermVectorsDescriptor(TDocument documentWithId, IndexName index = null, Nest.Id id = null)
    {
      IndexName index1 = index;
      if ((object) index1 == null)
        index1 = (IndexName) typeof (TDocument);
      Nest.Id id1 = id;
      if ((object) id1 == null)
        id1 = Nest.Id.From<TDocument>(documentWithId);
      // ISSUE: explicit constructor call
      this.\u002Ector(index1, id1);
    }

    IndexName ITermVectorsRequest<TDocument>.Index => this.Self.RouteValues.Get<IndexName>("index");

    Nest.Id ITermVectorsRequest<TDocument>.Id => this.Self.RouteValues.Get<Nest.Id>("id");

    public TermVectorsDescriptor<TDocument> Index(IndexName index) => this.Assign<IndexName>(index, (Action<ITermVectorsRequest<TDocument>, IndexName>) ((a, v) => a.RouteValues.Required(nameof (index), (IUrlParameter) v)));

    public TermVectorsDescriptor<TDocument> Index<TOther>() where TOther : class => this.Assign<Type>(typeof (TOther), (Action<ITermVectorsRequest<TDocument>, Type>) ((a, v) => a.RouteValues.Required("index", (IUrlParameter) (IndexName) v)));

    public TermVectorsDescriptor<TDocument> Id(Nest.Id id) => this.Assign<Nest.Id>(id, (Action<ITermVectorsRequest<TDocument>, Nest.Id>) ((a, v) => a.RouteValues.Optional(nameof (id), (IUrlParameter) v)));

    public TermVectorsDescriptor<TDocument> FieldStatistics(bool? fieldstatistics = true) => this.Qs("field_statistics", (object) fieldstatistics);

    public TermVectorsDescriptor<TDocument> Fields(Nest.Fields fields) => this.Qs(nameof (fields), (object) fields);

    public TermVectorsDescriptor<TDocument> Fields(
      params Expression<Func<TDocument, object>>[] fields)
    {
      return this.Qs(nameof (fields), fields != null ? (object) ((IEnumerable<Expression<Func<TDocument, object>>>) fields).Select<Expression<Func<TDocument, object>>, Field>((Func<Expression<Func<TDocument, object>>, Field>) (e => (Field) (Expression) e)) : (object) (IEnumerable<Field>) null);
    }

    public TermVectorsDescriptor<TDocument> Offsets(bool? offsets = true) => this.Qs(nameof (offsets), (object) offsets);

    public TermVectorsDescriptor<TDocument> Payloads(bool? payloads = true) => this.Qs(nameof (payloads), (object) payloads);

    public TermVectorsDescriptor<TDocument> Positions(bool? positions = true) => this.Qs(nameof (positions), (object) positions);

    public TermVectorsDescriptor<TDocument> Preference(string preference) => this.Qs(nameof (preference), (object) preference);

    public TermVectorsDescriptor<TDocument> Realtime(bool? realtime = true) => this.Qs(nameof (realtime), (object) realtime);

    public TermVectorsDescriptor<TDocument> Routing(Nest.Routing routing) => this.Qs(nameof (routing), (object) routing);

    public TermVectorsDescriptor<TDocument> TermStatistics(bool? termstatistics = true) => this.Qs("term_statistics", (object) termstatistics);

    public TermVectorsDescriptor<TDocument> Version(long? version) => this.Qs(nameof (version), (object) version);

    public TermVectorsDescriptor<TDocument> VersionType(Elasticsearch.Net.VersionType? versiontype) => this.Qs("version_type", (object) versiontype);

    TDocument ITermVectorsRequest<TDocument>.Document { get; set; }

    ITermVectorFilter ITermVectorsRequest<TDocument>.Filter { get; set; }

    HttpMethod IRequest.HttpMethod => (object) this.Self.Document == null && this.Self.Filter == null ? HttpMethod.GET : HttpMethod.POST;

    IPerFieldAnalyzer ITermVectorsRequest<TDocument>.PerFieldAnalyzer { get; set; }

    public TermVectorsDescriptor<TDocument> Document(TDocument document) => this.Assign<TDocument>(document, (Action<ITermVectorsRequest<TDocument>, TDocument>) ((a, v) => a.Document = v));

    public TermVectorsDescriptor<TDocument> PerFieldAnalyzer(
      Func<PerFieldAnalyzerDescriptor<TDocument>, IPromise<IPerFieldAnalyzer>> analyzerSelector)
    {
      return this.Assign<Func<PerFieldAnalyzerDescriptor<TDocument>, IPromise<IPerFieldAnalyzer>>>(analyzerSelector, (Action<ITermVectorsRequest<TDocument>, Func<PerFieldAnalyzerDescriptor<TDocument>, IPromise<IPerFieldAnalyzer>>>) ((a, v) => a.PerFieldAnalyzer = v != null ? v(new PerFieldAnalyzerDescriptor<TDocument>())?.Value : (IPerFieldAnalyzer) null));
    }

    public TermVectorsDescriptor<TDocument> Filter(
      Func<TermVectorFilterDescriptor, ITermVectorFilter> filterSelector)
    {
      return this.Assign<Func<TermVectorFilterDescriptor, ITermVectorFilter>>(filterSelector, (Action<ITermVectorsRequest<TDocument>, Func<TermVectorFilterDescriptor, ITermVectorFilter>>) ((a, v) => a.Filter = v != null ? v(new TermVectorFilterDescriptor()) : (ITermVectorFilter) null));
    }
  }
}
