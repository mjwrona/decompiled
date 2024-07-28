// Decompiled with JetBrains decompiler
// Type: Nest.SourceExistsDescriptor`1
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
  public class SourceExistsDescriptor<TDocument> : 
    RequestDescriptorBase<SourceExistsDescriptor<TDocument>, SourceExistsRequestParameters, ISourceExistsRequest<TDocument>>,
    ISourceExistsRequest<TDocument>,
    ISourceExistsRequest,
    IRequest<SourceExistsRequestParameters>,
    IRequest
    where TDocument : class
  {
    internal override ApiUrls ApiUrls => ApiUrlsLookups.NoNamespaceSourceExists;

    public SourceExistsDescriptor(IndexName index, Id id)
      : base((Func<RouteValues, RouteValues>) (r => r.Required(nameof (index), (IUrlParameter) index).Required(nameof (id), (IUrlParameter) id)))
    {
    }

    public SourceExistsDescriptor(Id id)
      : this((IndexName) typeof (TDocument), id)
    {
    }

    public SourceExistsDescriptor(TDocument documentWithId, IndexName index = null, Id id = null)
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
    protected SourceExistsDescriptor()
    {
    }

    IndexName ISourceExistsRequest.Index => this.Self.RouteValues.Get<IndexName>("index");

    Id ISourceExistsRequest.Id => this.Self.RouteValues.Get<Id>("id");

    public SourceExistsDescriptor<TDocument> Index(IndexName index) => this.Assign<IndexName>(index, (Action<ISourceExistsRequest<TDocument>, IndexName>) ((a, v) => a.RouteValues.Required(nameof (index), (IUrlParameter) v)));

    public SourceExistsDescriptor<TDocument> Index<TOther>() where TOther : class => this.Assign<Type>(typeof (TOther), (Action<ISourceExistsRequest<TDocument>, Type>) ((a, v) => a.RouteValues.Required("index", (IUrlParameter) (IndexName) v)));

    public SourceExistsDescriptor<TDocument> Preference(string preference) => this.Qs(nameof (preference), (object) preference);

    public SourceExistsDescriptor<TDocument> Realtime(bool? realtime = true) => this.Qs(nameof (realtime), (object) realtime);

    public SourceExistsDescriptor<TDocument> Refresh(bool? refresh = true) => this.Qs(nameof (refresh), (object) refresh);

    public SourceExistsDescriptor<TDocument> Routing(Nest.Routing routing) => this.Qs(nameof (routing), (object) routing);

    public SourceExistsDescriptor<TDocument> SourceEnabled(bool? sourceenabled = true) => this.Qs("_source", (object) sourceenabled);

    public SourceExistsDescriptor<TDocument> SourceExcludes(Fields sourceexcludes) => this.Qs("_source_excludes", (object) sourceexcludes);

    public SourceExistsDescriptor<TDocument> SourceExcludes(
      params Expression<Func<TDocument, object>>[] fields)
    {
      return this.Qs("_source_excludes", fields != null ? (object) ((IEnumerable<Expression<Func<TDocument, object>>>) fields).Select<Expression<Func<TDocument, object>>, Field>((Func<Expression<Func<TDocument, object>>, Field>) (e => (Field) (Expression) e)) : (object) (IEnumerable<Field>) null);
    }

    public SourceExistsDescriptor<TDocument> SourceIncludes(Fields sourceincludes) => this.Qs("_source_includes", (object) sourceincludes);

    public SourceExistsDescriptor<TDocument> SourceIncludes(
      params Expression<Func<TDocument, object>>[] fields)
    {
      return this.Qs("_source_includes", fields != null ? (object) ((IEnumerable<Expression<Func<TDocument, object>>>) fields).Select<Expression<Func<TDocument, object>>, Field>((Func<Expression<Func<TDocument, object>>, Field>) (e => (Field) (Expression) e)) : (object) (IEnumerable<Field>) null);
    }

    public SourceExistsDescriptor<TDocument> Version(long? version) => this.Qs(nameof (version), (object) version);

    public SourceExistsDescriptor<TDocument> VersionType(Elasticsearch.Net.VersionType? versiontype) => this.Qs("version_type", (object) versiontype);
  }
}
