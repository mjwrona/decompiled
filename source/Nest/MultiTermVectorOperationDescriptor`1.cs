// Decompiled with JetBrains decompiler
// Type: Nest.MultiTermVectorOperationDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class MultiTermVectorOperationDescriptor<T> : 
    DescriptorBase<MultiTermVectorOperationDescriptor<T>, IMultiTermVectorOperation>,
    IMultiTermVectorOperation
    where T : class
  {
    private Nest.Routing _routing;

    object IMultiTermVectorOperation.Document { get; set; }

    bool? IMultiTermVectorOperation.FieldStatistics { get; set; }

    ITermVectorFilter IMultiTermVectorOperation.Filter { get; set; }

    Nest.Id IMultiTermVectorOperation.Id { get; set; }

    IndexName IMultiTermVectorOperation.Index { get; set; } = (IndexName) typeof (T);

    bool? IMultiTermVectorOperation.Offsets { get; set; }

    bool? IMultiTermVectorOperation.Payloads { get; set; }

    bool? IMultiTermVectorOperation.Positions { get; set; }

    Nest.Routing IMultiTermVectorOperation.Routing
    {
      get
      {
        Nest.Routing routing = this._routing;
        if ((object) routing != null)
          return routing;
        return this.Self.Document != null ? new Nest.Routing(this.Self.Document) : (Nest.Routing) null;
      }
      set => this._routing = value;
    }

    Nest.Fields IMultiTermVectorOperation.Fields { get; set; }

    bool? IMultiTermVectorOperation.TermStatistics { get; set; }

    long? IMultiTermVectorOperation.Version { get; set; }

    Elasticsearch.Net.VersionType? IMultiTermVectorOperation.VersionType { get; set; }

    public MultiTermVectorOperationDescriptor<T> Fields(
      Func<FieldsDescriptor<T>, IPromise<Nest.Fields>> fields)
    {
      return this.Assign<Func<FieldsDescriptor<T>, IPromise<Nest.Fields>>>(fields, (Action<IMultiTermVectorOperation, Func<FieldsDescriptor<T>, IPromise<Nest.Fields>>>) ((a, v) => a.Fields = v != null ? v(new FieldsDescriptor<T>())?.Value : (Nest.Fields) null));
    }

    public MultiTermVectorOperationDescriptor<T> Fields(Nest.Fields fields) => this.Assign<Nest.Fields>(fields, (Action<IMultiTermVectorOperation, Nest.Fields>) ((a, v) => a.Fields = v));

    public MultiTermVectorOperationDescriptor<T> Id(Nest.Id id) => this.Assign<Nest.Id>(id, (Action<IMultiTermVectorOperation, Nest.Id>) ((a, v) => a.Id = v));

    public MultiTermVectorOperationDescriptor<T> Index(IndexName index) => this.Assign<IndexName>(index, (Action<IMultiTermVectorOperation, IndexName>) ((a, v) => a.Index = v));

    public MultiTermVectorOperationDescriptor<T> Document(T document) => this.Assign<T>(document, (Action<IMultiTermVectorOperation, T>) ((a, v) => a.Document = (object) v));

    public MultiTermVectorOperationDescriptor<T> Offsets(bool? offsets = true) => this.Assign<bool?>(offsets, (Action<IMultiTermVectorOperation, bool?>) ((a, v) => a.Offsets = v));

    public MultiTermVectorOperationDescriptor<T> Payloads(bool? payloads = true) => this.Assign<bool?>(payloads, (Action<IMultiTermVectorOperation, bool?>) ((a, v) => a.Payloads = v));

    public MultiTermVectorOperationDescriptor<T> Positions(bool? positions = true) => this.Assign<bool?>(positions, (Action<IMultiTermVectorOperation, bool?>) ((a, v) => a.Positions = v));

    public MultiTermVectorOperationDescriptor<T> TermStatistics(bool? termStatistics = true) => this.Assign<bool?>(termStatistics, (Action<IMultiTermVectorOperation, bool?>) ((a, v) => a.TermStatistics = v));

    public MultiTermVectorOperationDescriptor<T> FieldStatistics(bool? fieldStatistics = true) => this.Assign<bool?>(fieldStatistics, (Action<IMultiTermVectorOperation, bool?>) ((a, v) => a.FieldStatistics = v));

    public MultiTermVectorOperationDescriptor<T> Filter(
      Func<TermVectorFilterDescriptor, ITermVectorFilter> filterSelector)
    {
      return this.Assign<Func<TermVectorFilterDescriptor, ITermVectorFilter>>(filterSelector, (Action<IMultiTermVectorOperation, Func<TermVectorFilterDescriptor, ITermVectorFilter>>) ((a, v) => a.Filter = v != null ? v(new TermVectorFilterDescriptor()) : (ITermVectorFilter) null));
    }

    public MultiTermVectorOperationDescriptor<T> Version(long? version) => this.Assign<long?>(version, (Action<IMultiTermVectorOperation, long?>) ((a, v) => a.Version = v));

    public MultiTermVectorOperationDescriptor<T> VersionType(Elasticsearch.Net.VersionType? versionType) => this.Assign<Elasticsearch.Net.VersionType?>(versionType, (Action<IMultiTermVectorOperation, Elasticsearch.Net.VersionType?>) ((a, v) => a.VersionType = v));

    public MultiTermVectorOperationDescriptor<T> Routing(Nest.Routing routing) => this.Assign<Nest.Routing>(routing, (Action<IMultiTermVectorOperation, Nest.Routing>) ((a, v) => a.Routing = v));
  }
}
