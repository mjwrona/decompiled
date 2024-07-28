// Decompiled with JetBrains decompiler
// Type: Nest.MultiGetDescriptor
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
  public class MultiGetDescriptor : 
    RequestDescriptorBase<MultiGetDescriptor, MultiGetRequestParameters, IMultiGetRequest>,
    IMultiGetRequest,
    IRequest<MultiGetRequestParameters>,
    IRequest
  {
    private List<IMultiGetOperation> _operations = new List<IMultiGetOperation>();

    internal override ApiUrls ApiUrls => ApiUrlsLookups.NoNamespaceMultiGet;

    public MultiGetDescriptor()
    {
    }

    public MultiGetDescriptor(IndexName index)
      : base((Func<RouteValues, RouteValues>) (r => r.Optional(nameof (index), (IUrlParameter) index)))
    {
    }

    IndexName IMultiGetRequest.Index => this.Self.RouteValues.Get<IndexName>("index");

    public MultiGetDescriptor Index(IndexName index) => this.Assign<IndexName>(index, (Action<IMultiGetRequest, IndexName>) ((a, v) => a.RouteValues.Optional(nameof (index), (IUrlParameter) v)));

    public MultiGetDescriptor Index<TOther>() where TOther : class => this.Assign<Type>(typeof (TOther), (Action<IMultiGetRequest, Type>) ((a, v) => a.RouteValues.Optional("index", (IUrlParameter) (IndexName) v)));

    public MultiGetDescriptor Preference(string preference) => this.Qs(nameof (preference), (object) preference);

    public MultiGetDescriptor Realtime(bool? realtime = true) => this.Qs(nameof (realtime), (object) realtime);

    public MultiGetDescriptor Refresh(bool? refresh = true) => this.Qs(nameof (refresh), (object) refresh);

    public MultiGetDescriptor Routing(Nest.Routing routing) => this.Qs(nameof (routing), (object) routing);

    public MultiGetDescriptor SourceEnabled(bool? sourceenabled = true) => this.Qs("_source", (object) sourceenabled);

    public MultiGetDescriptor SourceExcludes(Fields sourceexcludes) => this.Qs("_source_excludes", (object) sourceexcludes);

    public MultiGetDescriptor SourceExcludes<T>(params Expression<Func<T, object>>[] fields) where T : class => this.Qs("_source_excludes", fields != null ? (object) ((IEnumerable<Expression<Func<T, object>>>) fields).Select<Expression<Func<T, object>>, Field>((Func<Expression<Func<T, object>>, Field>) (e => (Field) (Expression) e)) : (object) (IEnumerable<Field>) null);

    public MultiGetDescriptor SourceIncludes(Fields sourceincludes) => this.Qs("_source_includes", (object) sourceincludes);

    public MultiGetDescriptor SourceIncludes<T>(params Expression<Func<T, object>>[] fields) where T : class => this.Qs("_source_includes", fields != null ? (object) ((IEnumerable<Expression<Func<T, object>>>) fields).Select<Expression<Func<T, object>>, Field>((Func<Expression<Func<T, object>>, Field>) (e => (Field) (Expression) e)) : (object) (IEnumerable<Field>) null);

    protected override sealed void RequestDefaults(MultiGetRequestParameters parameters) => parameters.CustomResponseBuilder = (CustomResponseBuilderBase) new MultiGetResponseBuilder((IMultiGetRequest) this);

    IEnumerable<IMultiGetOperation> IMultiGetRequest.Documents
    {
      get => (IEnumerable<IMultiGetOperation>) this._operations;
      set => this._operations = value != null ? value.ToList<IMultiGetOperation>() : (List<IMultiGetOperation>) null;
    }

    Fields IMultiGetRequest.StoredFields
    {
      get => this.Q<Fields>("stored_fields");
      set => this.Q("stored_fields", (object) value);
    }

    public MultiGetDescriptor Get<T>(
      Func<MultiGetOperationDescriptor<T>, IMultiGetOperation> getSelector)
      where T : class
    {
      this._operations.AddIfNotNull<IMultiGetOperation>(getSelector != null ? getSelector(new MultiGetOperationDescriptor<T>()) : (IMultiGetOperation) null);
      return this;
    }

    public MultiGetDescriptor GetMany<T>(
      IEnumerable<long> ids,
      Func<MultiGetOperationDescriptor<T>, long, IMultiGetOperation> getSelector = null)
      where T : class
    {
      foreach (long id in ids)
        this._operations.Add(getSelector.InvokeOrDefault<MultiGetOperationDescriptor<T>, long, IMultiGetOperation>(new MultiGetOperationDescriptor<T>().Id((Id) id), id));
      return this;
    }

    public MultiGetDescriptor GetMany<T>(
      IEnumerable<string> ids,
      Func<MultiGetOperationDescriptor<T>, string, IMultiGetOperation> getSelector = null)
      where T : class
    {
      foreach (string id in ids)
        this._operations.Add(getSelector.InvokeOrDefault<MultiGetOperationDescriptor<T>, string, IMultiGetOperation>(new MultiGetOperationDescriptor<T>().Id((Id) id), id));
      return this;
    }

    public MultiGetDescriptor GetMany<T>(
      IEnumerable<Id> ids,
      Func<MultiGetOperationDescriptor<T>, Id, IMultiGetOperation> getSelector = null)
      where T : class
    {
      foreach (Id id in ids)
        this._operations.Add(getSelector.InvokeOrDefault<MultiGetOperationDescriptor<T>, Id, IMultiGetOperation>(new MultiGetOperationDescriptor<T>().Id(id), id));
      return this;
    }

    public MultiGetDescriptor StoredFields<T>(Func<FieldsDescriptor<T>, IPromise<Fields>> fields) where T : class => this.Assign<Func<FieldsDescriptor<T>, IPromise<Fields>>>(fields, (Action<IMultiGetRequest, Func<FieldsDescriptor<T>, IPromise<Fields>>>) ((a, v) => a.StoredFields = v != null ? v(new FieldsDescriptor<T>())?.Value : (Fields) null));

    public MultiGetDescriptor StoredFields(Fields fields) => this.Assign<Fields>(fields, (Action<IMultiGetRequest, Fields>) ((a, v) => a.StoredFields = v));
  }
}
