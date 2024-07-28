// Decompiled with JetBrains decompiler
// Type: Nest.BulkDeleteDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Runtime.Serialization;

namespace Nest
{
  [DataContract]
  public class BulkDeleteDescriptor<T> : 
    BulkOperationDescriptorBase<BulkDeleteDescriptor<T>, IBulkDeleteOperation<T>>,
    IBulkDeleteOperation<T>,
    IBulkOperation
    where T : class
  {
    protected override Type BulkOperationClrType => typeof (T);

    protected override string BulkOperationType => "delete";

    long? IBulkDeleteOperation<T>.IfSequenceNumber { get; set; }

    long? IBulkDeleteOperation<T>.IfPrimaryTerm { get; set; }

    T IBulkDeleteOperation<T>.Document { get; set; }

    protected override object GetBulkOperationBody() => (object) null;

    protected override Nest.Id GetIdForOperation(Inferrer inferrer)
    {
      Nest.Id id = this.Self.Id;
      return (object) id != null ? id : new Nest.Id((object) this.Self.Document);
    }

    protected override Nest.Routing GetRoutingForOperation(Inferrer inferrer)
    {
      Nest.Routing routing = this.Self.Routing;
      return (object) routing != null ? routing : new Nest.Routing((object) this.Self.Document);
    }

    public BulkDeleteDescriptor<T> Document(T @object) => this.Assign<T>(@object, (Action<IBulkDeleteOperation<T>, T>) ((a, v) => a.Document = v));

    public BulkDeleteDescriptor<T> IfSequenceNumber(long? sequenceNumber) => this.Assign<long?>(sequenceNumber, (Action<IBulkDeleteOperation<T>, long?>) ((a, v) => a.IfSequenceNumber = v));

    public BulkDeleteDescriptor<T> IfPrimaryTerm(long? primaryTerm) => this.Assign<long?>(primaryTerm, (Action<IBulkDeleteOperation<T>, long?>) ((a, v) => a.IfPrimaryTerm = v));
  }
}
