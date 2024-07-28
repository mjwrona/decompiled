// Decompiled with JetBrains decompiler
// Type: Nest.BulkCreateDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Runtime.Serialization;

namespace Nest
{
  [DataContract]
  public class BulkCreateDescriptor<T> : 
    BulkOperationDescriptorBase<BulkCreateDescriptor<T>, IBulkCreateOperation<T>>,
    IBulkCreateOperation<T>,
    IBulkOperation
    where T : class
  {
    protected override Type BulkOperationClrType => typeof (T);

    protected override string BulkOperationType => "create";

    T IBulkCreateOperation<T>.Document { get; set; }

    string IBulkCreateOperation<T>.Pipeline { get; set; }

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

    public BulkCreateDescriptor<T> Document(T @object) => this.Assign<T>(@object, (Action<IBulkCreateOperation<T>, T>) ((a, v) => a.Document = v));

    public BulkCreateDescriptor<T> Pipeline(string pipeline) => this.Assign<string>(pipeline, (Action<IBulkCreateOperation<T>, string>) ((a, v) => a.Pipeline = v));
  }
}
