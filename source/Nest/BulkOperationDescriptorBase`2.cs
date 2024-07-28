// Decompiled with JetBrains decompiler
// Type: Nest.BulkOperationDescriptorBase`2
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public abstract class BulkOperationDescriptorBase<TDescriptor, TInterface> : 
    DescriptorBase<TDescriptor, TInterface>,
    IBulkOperation
    where TDescriptor : BulkOperationDescriptorBase<TDescriptor, TInterface>, TInterface, IBulkOperation
    where TInterface : class, IBulkOperation
  {
    protected abstract Type BulkOperationClrType { get; }

    protected abstract string BulkOperationType { get; }

    Type IBulkOperation.ClrType => this.BulkOperationClrType;

    Nest.Id IBulkOperation.Id { get; set; }

    IndexName IBulkOperation.Index { get; set; }

    string IBulkOperation.Operation => this.BulkOperationType;

    int? IBulkOperation.RetriesOnConflict { get; set; }

    Nest.Routing IBulkOperation.Routing { get; set; }

    long? IBulkOperation.Version { get; set; }

    Elasticsearch.Net.VersionType? IBulkOperation.VersionType { get; set; }

    object IBulkOperation.GetBody() => this.GetBulkOperationBody();

    Nest.Id IBulkOperation.GetIdForOperation(Inferrer inferrer) => this.GetIdForOperation(inferrer);

    Nest.Routing IBulkOperation.GetRoutingForOperation(Inferrer inferrer) => this.GetRoutingForOperation(inferrer);

    protected abstract object GetBulkOperationBody();

    protected virtual Nest.Id GetIdForOperation(Inferrer inferrer)
    {
      Nest.Id id = this.Self.Id;
      return (object) id != null ? id : new Nest.Id(this.GetBulkOperationBody());
    }

    protected virtual Nest.Routing GetRoutingForOperation(Inferrer inferrer)
    {
      Nest.Routing routing = this.Self.Routing;
      return (object) routing != null ? routing : new Nest.Routing(this.GetBulkOperationBody());
    }

    public TDescriptor Index(IndexName index) => this.Assign<IndexName>(index, (Action<TInterface, IndexName>) ((a, v) => a.Index = v));

    public TDescriptor Index<T>() => this.Assign<Type>(typeof (T), (Action<TInterface, Type>) ((a, v) => a.Index = (IndexName) v));

    public TDescriptor Id(Nest.Id id) => this.Assign<Nest.Id>(id, (Action<TInterface, Nest.Id>) ((a, v) => a.Id = v));

    public TDescriptor Version(long? version) => this.Assign<long?>(version, (Action<TInterface, long?>) ((a, v) => a.Version = v));

    public TDescriptor VersionType(Elasticsearch.Net.VersionType? versionType) => this.Assign<Elasticsearch.Net.VersionType?>(versionType, (Action<TInterface, Elasticsearch.Net.VersionType?>) ((a, v) => a.VersionType = v));

    public TDescriptor Routing(Nest.Routing routing) => this.Assign<Nest.Routing>(routing, (Action<TInterface, Nest.Routing>) ((a, v) => a.Routing = v));
  }
}
