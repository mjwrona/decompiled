// Decompiled with JetBrains decompiler
// Type: Nest.UpdateRequest`2
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Utf8Json;
using System;
using System.Runtime.Serialization;

namespace Nest
{
  public class UpdateRequest<TDocument, TPartialDocument> : 
    PlainRequestBase<UpdateRequestParameters>,
    IUpdateRequest<TDocument, TPartialDocument>,
    IRequest<UpdateRequestParameters>,
    IRequest
    where TDocument : class
    where TPartialDocument : class
  {
    public bool? DetectNoop { get; set; }

    public TPartialDocument Doc { get; set; }

    public bool? DocAsUpsert { get; set; }

    public IScript Script { get; set; }

    public bool? ScriptedUpsert { get; set; }

    public Union<bool, ISourceFilter> Source { get; set; }

    public TDocument Upsert { get; set; }

    protected IUpdateRequest<TDocument, TPartialDocument> Self => (IUpdateRequest<TDocument, TPartialDocument>) this;

    internal override ApiUrls ApiUrls => ApiUrlsLookups.NoNamespaceUpdate;

    public UpdateRequest(IndexName index, Id id)
      : base((Func<RouteValues, RouteValues>) (r => r.Required(nameof (index), (IUrlParameter) index).Required(nameof (id), (IUrlParameter) id)))
    {
    }

    public UpdateRequest(Id id)
      : this((IndexName) typeof (TDocument), id)
    {
    }

    public UpdateRequest(TDocument documentWithId, IndexName index = null, Id id = null)
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
    protected UpdateRequest()
    {
    }

    [IgnoreDataMember]
    IndexName IUpdateRequest<TDocument, TPartialDocument>.Index => this.Self.RouteValues.Get<IndexName>("index");

    [IgnoreDataMember]
    Id IUpdateRequest<TDocument, TPartialDocument>.Id => this.Self.RouteValues.Get<Id>("id");

    public long? IfPrimaryTerm
    {
      get => this.Q<long?>("if_primary_term");
      set => this.Q("if_primary_term", (object) value);
    }

    public long? IfSequenceNumber
    {
      get => this.Q<long?>("if_seq_no");
      set => this.Q("if_seq_no", (object) value);
    }

    public string Lang
    {
      get => this.Q<string>("lang");
      set => this.Q("lang", (object) value);
    }

    public Elasticsearch.Net.Refresh? Refresh
    {
      get => this.Q<Elasticsearch.Net.Refresh?>("refresh");
      set => this.Q("refresh", (object) value);
    }

    public bool? RequireAlias
    {
      get => this.Q<bool?>("require_alias");
      set => this.Q("require_alias", (object) value);
    }

    public long? RetryOnConflict
    {
      get => this.Q<long?>("retry_on_conflict");
      set => this.Q("retry_on_conflict", (object) value);
    }

    public Routing Routing
    {
      get => this.Q<Routing>("routing");
      set => this.Q("routing", (object) value);
    }

    public bool? SourceEnabled
    {
      get => this.Q<bool?>("_source");
      set => this.Q("_source", (object) value);
    }

    public Time Timeout
    {
      get => this.Q<Time>("timeout");
      set => this.Q("timeout", (object) value);
    }

    public string WaitForActiveShards
    {
      get => this.Q<string>("wait_for_active_shards");
      set => this.Q("wait_for_active_shards", (object) value);
    }
  }
}
