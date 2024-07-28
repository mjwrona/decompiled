// Decompiled with JetBrains decompiler
// Type: Nest.BulkIndexDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Runtime.Serialization;

namespace Nest
{
  [DataContract]
  public class BulkIndexDescriptor<T> : 
    BulkOperationDescriptorBase<BulkIndexDescriptor<T>, IBulkIndexOperation<T>>,
    IBulkIndexOperation<T>,
    IBulkOperation
    where T : class
  {
    protected override Type BulkOperationClrType => typeof (T);

    protected override string BulkOperationType => "index";

    T IBulkIndexOperation<T>.Document { get; set; }

    string IBulkIndexOperation<T>.Percolate { get; set; }

    string IBulkIndexOperation<T>.Pipeline { get; set; }

    long? IBulkIndexOperation<T>.IfSequenceNumber { get; set; }

    long? IBulkIndexOperation<T>.IfPrimaryTerm { get; set; }

    protected override object GetBulkOperationBody() => (object) this.Self.Document;

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

    public BulkIndexDescriptor<T> Document(T @object) => this.Assign<T>(@object, (Action<IBulkIndexOperation<T>, T>) ((a, v) => a.Document = v));

    public BulkIndexDescriptor<T> Pipeline(string pipeline) => this.Assign<string>(pipeline, (Action<IBulkIndexOperation<T>, string>) ((a, v) => a.Pipeline = v));

    public BulkIndexDescriptor<T> Percolate(string percolate) => this.Assign<string>(percolate, (Action<IBulkIndexOperation<T>, string>) ((a, v) => a.Percolate = v));

    public BulkIndexDescriptor<T> IfSequenceNumber(long? seqNo) => this.Assign<long?>(seqNo, (Action<IBulkIndexOperation<T>, long?>) ((a, v) => a.IfSequenceNumber = v));

    public BulkIndexDescriptor<T> IfPrimaryTerm(long? primaryTerm) => this.Assign<long?>(primaryTerm, (Action<IBulkIndexOperation<T>, long?>) ((a, v) => a.IfPrimaryTerm = v));
  }
}
