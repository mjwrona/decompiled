// Decompiled with JetBrains decompiler
// Type: Nest.BulkUpdateDescriptor`2
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
  public class BulkUpdateDescriptor<TDocument, TPartialDocument> : 
    BulkOperationDescriptorBase<BulkUpdateDescriptor<TDocument, TPartialDocument>, IBulkUpdateOperation<TDocument, TPartialDocument>>,
    IBulkUpdateOperation<TDocument, TPartialDocument>,
    IBulkOperation
    where TDocument : class
    where TPartialDocument : class
  {
    protected override Type BulkOperationClrType => typeof (TDocument);

    protected override string BulkOperationType => "update";

    TPartialDocument IBulkUpdateOperation<TDocument, TPartialDocument>.Doc { get; set; }

    bool? IBulkUpdateOperation<TDocument, TPartialDocument>.DocAsUpsert { get; set; }

    TDocument IBulkUpdateOperation<TDocument, TPartialDocument>.IdFrom { get; set; }

    IScript IBulkUpdateOperation<TDocument, TPartialDocument>.Script { get; set; }

    bool? IBulkUpdateOperation<TDocument, TPartialDocument>.ScriptedUpsert { get; set; }

    TDocument IBulkUpdateOperation<TDocument, TPartialDocument>.Upsert { get; set; }

    long? IBulkUpdateOperation<TDocument, TPartialDocument>.IfSequenceNumber { get; set; }

    long? IBulkUpdateOperation<TDocument, TPartialDocument>.IfPrimaryTerm { get; set; }

    Union<bool, ISourceFilter> IBulkUpdateOperation<TDocument, TPartialDocument>.Source { get; set; }

    protected override object GetBulkOperationBody() => (object) new BulkUpdateBody<TDocument, TPartialDocument>()
    {
      PartialUpdate = this.Self.Doc,
      Script = this.Self.Script,
      Upsert = this.Self.Upsert,
      DocAsUpsert = this.Self.DocAsUpsert,
      ScriptedUpsert = this.Self.ScriptedUpsert,
      IfPrimaryTerm = this.Self.IfPrimaryTerm,
      IfSequenceNumber = this.Self.IfSequenceNumber,
      Source = this.Self.Source
    };

    protected override Nest.Id GetIdForOperation(Inferrer inferrer)
    {
      Nest.Id id = this.Self.Id;
      if ((object) id != null)
        return id;
      return new Nest.Id((object) ((IEnumerable<TDocument>) new TDocument[2]
      {
        this.Self.IdFrom,
        this.Self.Upsert
      }).FirstOrDefault<TDocument>((Func<TDocument, bool>) (o => (object) o != null)));
    }

    protected override Nest.Routing GetRoutingForOperation(Inferrer inferrer)
    {
      if (this.Self.Routing != (Nest.Routing) null)
        return this.Self.Routing;
      if ((object) this.Self.IdFrom != null)
        return new Nest.Routing((object) this.Self.IdFrom);
      return (object) this.Self.Upsert != null ? new Nest.Routing((object) this.Self.Upsert) : (Nest.Routing) null;
    }

    public BulkUpdateDescriptor<TDocument, TPartialDocument> IdFrom(
      TDocument @object,
      bool useAsUpsert = false)
    {
      this.Self.IdFrom = @object;
      return !useAsUpsert ? this : this.Upsert(@object);
    }

    public BulkUpdateDescriptor<TDocument, TPartialDocument> Upsert(TDocument @object) => this.Assign<TDocument>(@object, (Action<IBulkUpdateOperation<TDocument, TPartialDocument>, TDocument>) ((a, v) => a.Upsert = v));

    public BulkUpdateDescriptor<TDocument, TPartialDocument> Doc(TPartialDocument @object) => this.Assign<TPartialDocument>(@object, (Action<IBulkUpdateOperation<TDocument, TPartialDocument>, TPartialDocument>) ((a, v) => a.Doc = v));

    public BulkUpdateDescriptor<TDocument, TPartialDocument> DocAsUpsert(
      bool? partialDocumentAsUpsert = true)
    {
      return this.Assign<bool?>(partialDocumentAsUpsert, (Action<IBulkUpdateOperation<TDocument, TPartialDocument>, bool?>) ((a, v) => a.DocAsUpsert = v));
    }

    public BulkUpdateDescriptor<TDocument, TPartialDocument> ScriptedUpsert(bool? scriptedUpsert = true) => this.Assign<bool?>(scriptedUpsert, (Action<IBulkUpdateOperation<TDocument, TPartialDocument>, bool?>) ((a, v) => a.ScriptedUpsert = v));

    public BulkUpdateDescriptor<TDocument, TPartialDocument> Script(
      Func<ScriptDescriptor, IScript> scriptSelector)
    {
      return this.Assign<Func<ScriptDescriptor, IScript>>(scriptSelector, (Action<IBulkUpdateOperation<TDocument, TPartialDocument>, Func<ScriptDescriptor, IScript>>) ((a, v) => a.Script = v != null ? v(new ScriptDescriptor()) : (IScript) null));
    }

    public BulkUpdateDescriptor<TDocument, TPartialDocument> RetriesOnConflict(
      int? retriesOnConflict)
    {
      return this.Assign<int?>(retriesOnConflict, (Action<IBulkUpdateOperation<TDocument, TPartialDocument>, int?>) ((a, v) => a.RetriesOnConflict = v));
    }

    public BulkUpdateDescriptor<TDocument, TPartialDocument> IfSequenceNumber(long? seqNo) => this.Assign<long?>(seqNo, (Action<IBulkUpdateOperation<TDocument, TPartialDocument>, long?>) ((a, v) => a.IfSequenceNumber = v));

    public BulkUpdateDescriptor<TDocument, TPartialDocument> IfPrimaryTerm(long? primaryTerm) => this.Assign<long?>(primaryTerm, (Action<IBulkUpdateOperation<TDocument, TPartialDocument>, long?>) ((a, v) => a.IfPrimaryTerm = v));

    public BulkUpdateDescriptor<TDocument, TPartialDocument> Source(
      Union<bool, ISourceFilter> source)
    {
      return this.Assign<Union<bool, ISourceFilter>>(source, (Action<IBulkUpdateOperation<TDocument, TPartialDocument>, Union<bool, ISourceFilter>>) ((a, v) => a.Source = v));
    }
  }
}
