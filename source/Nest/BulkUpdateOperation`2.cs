// Decompiled with JetBrains decompiler
// Type: Nest.BulkUpdateOperation`2
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Nest
{
  [DataContract]
  public class BulkUpdateOperation<TDocument, TPartialDocument> : 
    BulkOperationBase,
    IBulkUpdateOperation<TDocument, TPartialDocument>,
    IBulkOperation
    where TDocument : class
    where TPartialDocument : class
  {
    public BulkUpdateOperation(Id id) => this.Id = id;

    public BulkUpdateOperation(TDocument idFrom, bool useIdFromAsUpsert = false)
    {
      this.IdFrom = idFrom;
      if (!useIdFromAsUpsert)
        return;
      this.Upsert = idFrom;
    }

    public BulkUpdateOperation(TDocument idFrom, TPartialDocument update, bool useIdFromAsUpsert = false)
    {
      this.IdFrom = idFrom;
      if (useIdFromAsUpsert)
        this.Upsert = idFrom;
      this.Doc = update;
    }

    public TPartialDocument Doc { get; set; }

    public bool? DocAsUpsert { get; set; }

    public TDocument IdFrom { get; set; }

    public IScript Script { get; set; }

    public bool? ScriptedUpsert { get; set; }

    public TDocument Upsert { get; set; }

    public long? IfSequenceNumber { get; set; }

    public long? IfPrimaryTerm { get; set; }

    public Union<bool, ISourceFilter> Source { get; set; }

    protected override Type ClrType => typeof (TDocument);

    protected override string Operation => "update";

    protected override Id GetIdForOperation(Inferrer inferrer)
    {
      Id id = this.Id;
      if ((object) id != null)
        return id;
      return new Id((object) ((IEnumerable<TDocument>) new TDocument[2]
      {
        this.IdFrom,
        this.Upsert
      }).FirstOrDefault<TDocument>((Func<TDocument, bool>) (o => (object) o != null)));
    }

    protected override Routing GetRoutingForOperation(Inferrer inferrer)
    {
      if (this.Routing != (Routing) null)
        return this.Routing;
      if ((object) this.IdFrom != null)
        return new Routing((object) this.IdFrom);
      return (object) this.Upsert != null ? new Routing((object) this.Upsert) : (Routing) null;
    }

    protected override object GetBody() => (object) new BulkUpdateBody<TDocument, TPartialDocument>()
    {
      PartialUpdate = this.Doc,
      Script = this.Script,
      Upsert = this.Upsert,
      DocAsUpsert = this.DocAsUpsert,
      ScriptedUpsert = this.ScriptedUpsert,
      IfPrimaryTerm = this.IfPrimaryTerm,
      IfSequenceNumber = this.IfSequenceNumber,
      Source = this.Source
    };
  }
}
