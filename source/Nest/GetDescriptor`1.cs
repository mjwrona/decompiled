// Decompiled with JetBrains decompiler
// Type: Nest.GetDescriptor`1
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
  public class GetDescriptor<TDocument> : 
    RequestDescriptorBase<GetDescriptor<TDocument>, GetRequestParameters, IGetRequest<TDocument>>,
    IGetRequest<TDocument>,
    IGetRequest,
    IRequest<GetRequestParameters>,
    IRequest
    where TDocument : class
  {
    internal override ApiUrls ApiUrls => ApiUrlsLookups.NoNamespaceGet;

    public GetDescriptor(IndexName index, Id id)
      : base((Func<RouteValues, RouteValues>) (r => r.Required(nameof (index), (IUrlParameter) index).Required(nameof (id), (IUrlParameter) id)))
    {
    }

    public GetDescriptor(Id id)
      : this((IndexName) typeof (TDocument), id)
    {
    }

    public GetDescriptor(TDocument documentWithId, IndexName index = null, Id id = null)
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
    protected GetDescriptor()
    {
    }

    IndexName IGetRequest.Index => this.Self.RouteValues.Get<IndexName>("index");

    Id IGetRequest.Id => this.Self.RouteValues.Get<Id>("id");

    public GetDescriptor<TDocument> Index(IndexName index) => this.Assign<IndexName>(index, (Action<IGetRequest<TDocument>, IndexName>) ((a, v) => a.RouteValues.Required(nameof (index), (IUrlParameter) v)));

    public GetDescriptor<TDocument> Index<TOther>() where TOther : class => this.Assign<Type>(typeof (TOther), (Action<IGetRequest<TDocument>, Type>) ((a, v) => a.RouteValues.Required("index", (IUrlParameter) (IndexName) v)));

    public GetDescriptor<TDocument> Preference(string preference) => this.Qs(nameof (preference), (object) preference);

    public GetDescriptor<TDocument> Realtime(bool? realtime = true) => this.Qs(nameof (realtime), (object) realtime);

    public GetDescriptor<TDocument> Refresh(bool? refresh = true) => this.Qs(nameof (refresh), (object) refresh);

    public GetDescriptor<TDocument> Routing(Nest.Routing routing) => this.Qs(nameof (routing), (object) routing);

    public GetDescriptor<TDocument> SourceEnabled(bool? sourceenabled = true) => this.Qs("_source", (object) sourceenabled);

    public GetDescriptor<TDocument> SourceExcludes(Fields sourceexcludes) => this.Qs("_source_excludes", (object) sourceexcludes);

    public GetDescriptor<TDocument> SourceExcludes(
      params Expression<Func<TDocument, object>>[] fields)
    {
      return this.Qs("_source_excludes", fields != null ? (object) ((IEnumerable<Expression<Func<TDocument, object>>>) fields).Select<Expression<Func<TDocument, object>>, Field>((Func<Expression<Func<TDocument, object>>, Field>) (e => (Field) (Expression) e)) : (object) (IEnumerable<Field>) null);
    }

    public GetDescriptor<TDocument> SourceIncludes(Fields sourceincludes) => this.Qs("_source_includes", (object) sourceincludes);

    public GetDescriptor<TDocument> SourceIncludes(
      params Expression<Func<TDocument, object>>[] fields)
    {
      return this.Qs("_source_includes", fields != null ? (object) ((IEnumerable<Expression<Func<TDocument, object>>>) fields).Select<Expression<Func<TDocument, object>>, Field>((Func<Expression<Func<TDocument, object>>, Field>) (e => (Field) (Expression) e)) : (object) (IEnumerable<Field>) null);
    }

    public GetDescriptor<TDocument> StoredFields(Fields storedfields) => this.Qs("stored_fields", (object) storedfields);

    public GetDescriptor<TDocument> StoredFields(
      params Expression<Func<TDocument, object>>[] fields)
    {
      return this.Qs("stored_fields", fields != null ? (object) ((IEnumerable<Expression<Func<TDocument, object>>>) fields).Select<Expression<Func<TDocument, object>>, Field>((Func<Expression<Func<TDocument, object>>, Field>) (e => (Field) (Expression) e)) : (object) (IEnumerable<Field>) null);
    }

    public GetDescriptor<TDocument> Version(long? version) => this.Qs(nameof (version), (object) version);

    public GetDescriptor<TDocument> VersionType(Elasticsearch.Net.VersionType? versiontype) => this.Qs("version_type", (object) versiontype);

    public GetDescriptor<TDocument> ExecuteOnLocalShard() => this.Preference("_local");
  }
}
