// Decompiled with JetBrains decompiler
// Type: Nest.BulkOperationBase
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public abstract class BulkOperationBase : IBulkOperation
  {
    public Id Id { get; set; }

    public IndexName Index { get; set; }

    public int? RetriesOnConflict { get; set; }

    public Routing Routing { get; set; }

    public long? Version { get; set; }

    public Elasticsearch.Net.VersionType? VersionType { get; set; }

    protected abstract Type ClrType { get; }

    protected abstract string Operation { get; }

    Type IBulkOperation.ClrType => this.ClrType;

    string IBulkOperation.Operation => this.Operation;

    object IBulkOperation.GetBody() => this.GetBody();

    Id IBulkOperation.GetIdForOperation(Inferrer inferrer) => this.GetIdForOperation(inferrer);

    Routing IBulkOperation.GetRoutingForOperation(Inferrer inferrer) => this.GetRoutingForOperation(inferrer);

    protected abstract object GetBody();

    protected virtual Id GetIdForOperation(Inferrer inferrer)
    {
      Id id = this.Id;
      return (object) id != null ? id : new Id(this.GetBody());
    }

    protected virtual Routing GetRoutingForOperation(Inferrer inferrer)
    {
      Routing routing = this.Routing;
      return (object) routing != null ? routing : new Routing(this.GetBody());
    }
  }
}
