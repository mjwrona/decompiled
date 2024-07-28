// Decompiled with JetBrains decompiler
// Type: Nest.BulkDeleteOperation`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Runtime.Serialization;

namespace Nest
{
  [DataContract]
  public class BulkDeleteOperation<T> : BulkOperationBase, IBulkDeleteOperation<T>, IBulkOperation where T : class
  {
    public BulkDeleteOperation(T document) => this.Document = document;

    public BulkDeleteOperation(Id id) => this.Id = id;

    public T Document { get; set; }

    public long? IfSequenceNumber { get; set; }

    public long? IfPrimaryTerm { get; set; }

    protected override Type ClrType => typeof (T);

    protected override string Operation => "delete";

    protected override object GetBody() => (object) null;

    protected override Id GetIdForOperation(Inferrer inferrer)
    {
      Id id = this.Id;
      return (object) id != null ? id : new Id((object) this.Document);
    }

    protected override Routing GetRoutingForOperation(Inferrer inferrer)
    {
      Routing routing = this.Routing;
      return (object) routing != null ? routing : new Routing((object) this.Document);
    }
  }
}
