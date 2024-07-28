// Decompiled with JetBrains decompiler
// Type: Nest.MultiGetOperationDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class MultiGetOperationDescriptor<T> : 
    DescriptorBase<MultiGetOperationDescriptor<T>, IMultiGetOperation>,
    IMultiGetOperation
    where T : class
  {
    public MultiGetOperationDescriptor() => this.Self.Index = (IndexName) this.Self.ClrType;

    public MultiGetOperationDescriptor(bool allowExplicitIndex)
      : this()
    {
      if (allowExplicitIndex)
        return;
      this.Self.Index = (IndexName) null;
    }

    bool IMultiGetOperation.CanBeFlattened => this.Self.Index == (IndexName) null && this.Self.Routing == null && this.Self.Source == null && this.Self.StoredFields == (Fields) null;

    Type IMultiGetOperation.ClrType => typeof (T);

    Nest.Id IMultiGetOperation.Id { get; set; }

    IndexName IMultiGetOperation.Index { get; set; }

    string IMultiGetOperation.Routing { get; set; }

    Union<bool, ISourceFilter> IMultiGetOperation.Source { get; set; }

    Fields IMultiGetOperation.StoredFields { get; set; }

    long? IMultiGetOperation.Version { get; set; }

    Elasticsearch.Net.VersionType? IMultiGetOperation.VersionType { get; set; }

    public MultiGetOperationDescriptor<T> Index(IndexName index) => this.Assign<IndexName>(index, (Action<IMultiGetOperation, IndexName>) ((a, v) => a.Index = v));

    public MultiGetOperationDescriptor<T> Id(Nest.Id id) => this.Assign<Nest.Id>(id, (Action<IMultiGetOperation, Nest.Id>) ((a, v) => a.Id = v));

    public MultiGetOperationDescriptor<T> Source(bool? sourceEnabled = true) => this.Assign<bool?>(sourceEnabled, (Action<IMultiGetOperation, bool?>) ((a, v) =>
    {
      IMultiGetOperation multiGetOperation = a;
      bool? nullable = v;
      Union<bool, ISourceFilter> valueOrDefault = nullable.HasValue ? (Union<bool, ISourceFilter>) nullable.GetValueOrDefault() : (Union<bool, ISourceFilter>) null;
      multiGetOperation.Source = valueOrDefault;
    }));

    public MultiGetOperationDescriptor<T> Source(
      Func<SourceFilterDescriptor<T>, ISourceFilter> source)
    {
      return this.Assign<Union<bool, ISourceFilter>>(new Union<bool, ISourceFilter>(source(new SourceFilterDescriptor<T>())), (Action<IMultiGetOperation, Union<bool, ISourceFilter>>) ((a, v) => a.Source = v));
    }

    public MultiGetOperationDescriptor<T> Routing(string routing) => this.Assign<string>(routing, (Action<IMultiGetOperation, string>) ((a, v) => a.Routing = v));

    public MultiGetOperationDescriptor<T> StoredFields(
      Func<FieldsDescriptor<T>, IPromise<Fields>> fields)
    {
      return this.Assign<Func<FieldsDescriptor<T>, IPromise<Fields>>>(fields, (Action<IMultiGetOperation, Func<FieldsDescriptor<T>, IPromise<Fields>>>) ((a, v) => a.StoredFields = v != null ? v(new FieldsDescriptor<T>())?.Value : (Fields) null));
    }

    public MultiGetOperationDescriptor<T> StoredFields(Fields fields) => this.Assign<Fields>(fields, (Action<IMultiGetOperation, Fields>) ((a, v) => a.StoredFields = v));

    public MultiGetOperationDescriptor<T> Version(long? version) => this.Assign<long?>(version, (Action<IMultiGetOperation, long?>) ((a, v) => a.Version = v));

    public MultiGetOperationDescriptor<T> VersionType(Elasticsearch.Net.VersionType? versionType) => this.Assign<Elasticsearch.Net.VersionType?>(versionType, (Action<IMultiGetOperation, Elasticsearch.Net.VersionType?>) ((a, v) => a.VersionType = v));
  }
}
