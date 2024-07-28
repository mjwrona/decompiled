// Decompiled with JetBrains decompiler
// Type: Nest.UpdateDescriptor`2
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Utf8Json;
using System;

namespace Nest
{
  public class UpdateDescriptor<TDocument, TPartialDocument> : 
    RequestDescriptorBase<UpdateDescriptor<TDocument, TPartialDocument>, UpdateRequestParameters, IUpdateRequest<TDocument, TPartialDocument>>,
    IUpdateRequest<TDocument, TPartialDocument>,
    IRequest<UpdateRequestParameters>,
    IRequest
    where TDocument : class
    where TPartialDocument : class
  {
    internal override ApiUrls ApiUrls => ApiUrlsLookups.NoNamespaceUpdate;

    public UpdateDescriptor(IndexName index, Id id)
      : base((Func<RouteValues, RouteValues>) (r => r.Required(nameof (index), (IUrlParameter) index).Required(nameof (id), (IUrlParameter) id)))
    {
    }

    public UpdateDescriptor(Id id)
      : this((IndexName) typeof (TDocument), id)
    {
    }

    public UpdateDescriptor(TDocument documentWithId, IndexName index = null, Id id = null)
    {
      IndexName index1 = index;
      if ((object) index1 == null)
        index1 = (IndexName) typeof (TDocument);
      Id id1 = id;
      if ((object) id1 == null)
        id1 = Id.From<TDocument>(documentWithId);
      // ISSUE: explicit constructor call
      this.\u002Ector(index1, id1);
    }

    [SerializationConstructor]
    protected UpdateDescriptor()
    {
    }

    IndexName IUpdateRequest<TDocument, TPartialDocument>.Index => this.Self.RouteValues.Get<IndexName>("index");

    Id IUpdateRequest<TDocument, TPartialDocument>.Id => this.Self.RouteValues.Get<Id>("id");

    public UpdateDescriptor<TDocument, TPartialDocument> Index(IndexName index) => this.Assign<IndexName>(index, (Action<IUpdateRequest<TDocument, TPartialDocument>, IndexName>) ((a, v) => a.RouteValues.Required(nameof (index), (IUrlParameter) v)));

    public UpdateDescriptor<TDocument, TPartialDocument> Index<TOther>() where TOther : class => this.Assign<Type>(typeof (TOther), (Action<IUpdateRequest<TDocument, TPartialDocument>, Type>) ((a, v) => a.RouteValues.Required("index", (IUrlParameter) (IndexName) v)));

    public UpdateDescriptor<TDocument, TPartialDocument> IfPrimaryTerm(long? ifprimaryterm) => this.Qs("if_primary_term", (object) ifprimaryterm);

    public UpdateDescriptor<TDocument, TPartialDocument> IfSequenceNumber(long? ifsequencenumber) => this.Qs("if_seq_no", (object) ifsequencenumber);

    public UpdateDescriptor<TDocument, TPartialDocument> Lang(string lang) => this.Qs(nameof (lang), (object) lang);

    public UpdateDescriptor<TDocument, TPartialDocument> Refresh(Elasticsearch.Net.Refresh? refresh) => this.Qs(nameof (refresh), (object) refresh);

    public UpdateDescriptor<TDocument, TPartialDocument> RequireAlias(bool? requirealias = true) => this.Qs("require_alias", (object) requirealias);

    public UpdateDescriptor<TDocument, TPartialDocument> RetryOnConflict(long? retryonconflict) => this.Qs("retry_on_conflict", (object) retryonconflict);

    public UpdateDescriptor<TDocument, TPartialDocument> Routing(Nest.Routing routing) => this.Qs(nameof (routing), (object) routing);

    public UpdateDescriptor<TDocument, TPartialDocument> SourceEnabled(bool? sourceenabled = true) => this.Qs("_source", (object) sourceenabled);

    public UpdateDescriptor<TDocument, TPartialDocument> Timeout(Time timeout) => this.Qs(nameof (timeout), (object) timeout);

    public UpdateDescriptor<TDocument, TPartialDocument> WaitForActiveShards(
      string waitforactiveshards)
    {
      return this.Qs("wait_for_active_shards", (object) waitforactiveshards);
    }

    bool? IUpdateRequest<TDocument, TPartialDocument>.DetectNoop { get; set; }

    TPartialDocument IUpdateRequest<TDocument, TPartialDocument>.Doc { get; set; }

    bool? IUpdateRequest<TDocument, TPartialDocument>.DocAsUpsert { get; set; }

    IScript IUpdateRequest<TDocument, TPartialDocument>.Script { get; set; }

    bool? IUpdateRequest<TDocument, TPartialDocument>.ScriptedUpsert { get; set; }

    Union<bool, ISourceFilter> IUpdateRequest<TDocument, TPartialDocument>.Source { get; set; }

    TDocument IUpdateRequest<TDocument, TPartialDocument>.Upsert { get; set; }

    public UpdateDescriptor<TDocument, TPartialDocument> Upsert(TDocument upsertObject) => this.Assign<TDocument>(upsertObject, (Action<IUpdateRequest<TDocument, TPartialDocument>, TDocument>) ((a, v) => a.Upsert = v));

    public UpdateDescriptor<TDocument, TPartialDocument> Doc(TPartialDocument @object) => this.Assign<TPartialDocument>(@object, (Action<IUpdateRequest<TDocument, TPartialDocument>, TPartialDocument>) ((a, v) => a.Doc = v));

    public UpdateDescriptor<TDocument, TPartialDocument> DocAsUpsert(bool? docAsUpsert = true) => this.Assign<bool?>(docAsUpsert, (Action<IUpdateRequest<TDocument, TPartialDocument>, bool?>) ((a, v) => a.DocAsUpsert = v));

    public UpdateDescriptor<TDocument, TPartialDocument> DetectNoop(bool? detectNoop = true) => this.Assign<bool?>(detectNoop, (Action<IUpdateRequest<TDocument, TPartialDocument>, bool?>) ((a, v) => a.DetectNoop = v));

    public UpdateDescriptor<TDocument, TPartialDocument> ScriptedUpsert(bool? scriptedUpsert = true) => this.Assign<bool?>(scriptedUpsert, (Action<IUpdateRequest<TDocument, TPartialDocument>, bool?>) ((a, v) => a.ScriptedUpsert = v));

    public UpdateDescriptor<TDocument, TPartialDocument> Script(
      Func<ScriptDescriptor, IScript> scriptSelector)
    {
      return this.Assign<Func<ScriptDescriptor, IScript>>(scriptSelector, (Action<IUpdateRequest<TDocument, TPartialDocument>, Func<ScriptDescriptor, IScript>>) ((a, v) => a.Script = v != null ? v(new ScriptDescriptor()) : (IScript) null));
    }

    public UpdateDescriptor<TDocument, TPartialDocument> Source(bool? enabled = true) => this.Assign<bool?>(enabled, (Action<IUpdateRequest<TDocument, TPartialDocument>, bool?>) ((a, v) =>
    {
      IUpdateRequest<TDocument, TPartialDocument> updateRequest = a;
      bool? nullable = v;
      Union<bool, ISourceFilter> valueOrDefault = nullable.HasValue ? (Union<bool, ISourceFilter>) nullable.GetValueOrDefault() : (Union<bool, ISourceFilter>) null;
      updateRequest.Source = valueOrDefault;
    }));

    public UpdateDescriptor<TDocument, TPartialDocument> Source(
      Func<SourceFilterDescriptor<TDocument>, ISourceFilter> selector)
    {
      return this.Assign<Func<SourceFilterDescriptor<TDocument>, ISourceFilter>>(selector, (Action<IUpdateRequest<TDocument, TPartialDocument>, Func<SourceFilterDescriptor<TDocument>, ISourceFilter>>) ((a, v) => a.Source = new Union<bool, ISourceFilter>(v != null ? v(new SourceFilterDescriptor<TDocument>()) : (ISourceFilter) null)));
    }
  }
}
