// Decompiled with JetBrains decompiler
// Type: Nest.MultiTermVectorsDescriptor
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
  public class MultiTermVectorsDescriptor : 
    RequestDescriptorBase<MultiTermVectorsDescriptor, MultiTermVectorsRequestParameters, IMultiTermVectorsRequest>,
    IMultiTermVectorsRequest,
    IRequest<MultiTermVectorsRequestParameters>,
    IRequest
  {
    private List<IMultiTermVectorOperation> _operations;

    internal override ApiUrls ApiUrls => ApiUrlsLookups.NoNamespaceMultiTermVectors;

    public MultiTermVectorsDescriptor()
    {
    }

    public MultiTermVectorsDescriptor(IndexName index)
      : base((Func<RouteValues, RouteValues>) (r => r.Optional(nameof (index), (IUrlParameter) index)))
    {
    }

    IndexName IMultiTermVectorsRequest.Index => this.Self.RouteValues.Get<IndexName>("index");

    public MultiTermVectorsDescriptor Index(IndexName index) => this.Assign<IndexName>(index, (Action<IMultiTermVectorsRequest, IndexName>) ((a, v) => a.RouteValues.Optional(nameof (index), (IUrlParameter) v)));

    public MultiTermVectorsDescriptor Index<TOther>() where TOther : class => this.Assign<Type>(typeof (TOther), (Action<IMultiTermVectorsRequest, Type>) ((a, v) => a.RouteValues.Optional("index", (IUrlParameter) (IndexName) v)));

    public MultiTermVectorsDescriptor FieldStatistics(bool? fieldstatistics = true) => this.Qs("field_statistics", (object) fieldstatistics);

    public MultiTermVectorsDescriptor Fields(Nest.Fields fields) => this.Qs(nameof (fields), (object) fields);

    public MultiTermVectorsDescriptor Fields<T>(params Expression<Func<T, object>>[] fields) where T : class => this.Qs(nameof (fields), fields != null ? (object) ((IEnumerable<Expression<Func<T, object>>>) fields).Select<Expression<Func<T, object>>, Field>((Func<Expression<Func<T, object>>, Field>) (e => (Field) (Expression) e)) : (object) (IEnumerable<Field>) null);

    public MultiTermVectorsDescriptor Offsets(bool? offsets = true) => this.Qs(nameof (offsets), (object) offsets);

    public MultiTermVectorsDescriptor Payloads(bool? payloads = true) => this.Qs(nameof (payloads), (object) payloads);

    public MultiTermVectorsDescriptor Positions(bool? positions = true) => this.Qs(nameof (positions), (object) positions);

    public MultiTermVectorsDescriptor Preference(string preference) => this.Qs(nameof (preference), (object) preference);

    public MultiTermVectorsDescriptor Realtime(bool? realtime = true) => this.Qs(nameof (realtime), (object) realtime);

    public MultiTermVectorsDescriptor Routing(Nest.Routing routing) => this.Qs(nameof (routing), (object) routing);

    public MultiTermVectorsDescriptor TermStatistics(bool? termstatistics = true) => this.Qs("term_statistics", (object) termstatistics);

    public MultiTermVectorsDescriptor Version(long? version) => this.Qs(nameof (version), (object) version);

    public MultiTermVectorsDescriptor VersionType(Elasticsearch.Net.VersionType? versiontype) => this.Qs("version_type", (object) versiontype);

    IEnumerable<IMultiTermVectorOperation> IMultiTermVectorsRequest.Documents
    {
      get => (IEnumerable<IMultiTermVectorOperation>) this._operations;
      set => this._operations = value != null ? value.ToList<IMultiTermVectorOperation>() : (List<IMultiTermVectorOperation>) null;
    }

    IEnumerable<Id> IMultiTermVectorsRequest.Ids { get; set; }

    private List<IMultiTermVectorOperation> Operations => this._operations ?? (this._operations = new List<IMultiTermVectorOperation>());

    public MultiTermVectorsDescriptor Documents<T>(
      Func<MultiTermVectorOperationDescriptor<T>, IMultiTermVectorOperation> selector)
      where T : class
    {
      this.Operations.AddIfNotNull<IMultiTermVectorOperation>(selector != null ? selector(new MultiTermVectorOperationDescriptor<T>()) : (IMultiTermVectorOperation) null);
      return this;
    }

    public MultiTermVectorsDescriptor Documents<T>(
      IEnumerable<long> ids,
      Func<MultiTermVectorOperationDescriptor<T>, long, IMultiTermVectorOperation> selector = null)
      where T : class
    {
      foreach (long id in ids)
        this.Operations.Add(selector.InvokeOrDefault<MultiTermVectorOperationDescriptor<T>, long, IMultiTermVectorOperation>(new MultiTermVectorOperationDescriptor<T>().Id((Id) id), id));
      return this;
    }

    public MultiTermVectorsDescriptor Documents<T>(
      IEnumerable<string> ids,
      Func<MultiTermVectorOperationDescriptor<T>, string, IMultiTermVectorOperation> getSelector = null)
      where T : class
    {
      foreach (string id in ids)
        this.Operations.Add(getSelector.InvokeOrDefault<MultiTermVectorOperationDescriptor<T>, string, IMultiTermVectorOperation>(new MultiTermVectorOperationDescriptor<T>().Id((Id) id), id));
      return this;
    }

    public MultiTermVectorsDescriptor Documents<T>(
      IEnumerable<Id> ids,
      Func<MultiTermVectorOperationDescriptor<T>, Id, IMultiTermVectorOperation> getSelector = null)
      where T : class
    {
      foreach (Id id in ids)
        this.Operations.Add(getSelector.InvokeOrDefault<MultiTermVectorOperationDescriptor<T>, Id, IMultiTermVectorOperation>(new MultiTermVectorOperationDescriptor<T>().Id(id), id));
      return this;
    }

    public MultiTermVectorsDescriptor Ids(IEnumerable<Id> ids) => this.Assign<IEnumerable<Id>>(ids, (Action<IMultiTermVectorsRequest, IEnumerable<Id>>) ((a, v) => a.Ids = v));

    public MultiTermVectorsDescriptor Ids(params Id[] ids) => this.Assign<Id[]>(ids, (Action<IMultiTermVectorsRequest, Id[]>) ((a, v) => a.Ids = (IEnumerable<Id>) v));
  }
}
