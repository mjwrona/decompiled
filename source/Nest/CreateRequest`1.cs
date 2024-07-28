// Decompiled with JetBrains decompiler
// Type: Nest.CreateRequest`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Utf8Json;
using System;
using System.IO;
using System.Runtime.Serialization;

namespace Nest
{
  public class CreateRequest<TDocument> : 
    PlainRequestBase<CreateRequestParameters>,
    ICreateRequest<TDocument>,
    IProxyRequest,
    IRequest,
    IDocumentRequest,
    IRequest<CreateRequestParameters>
    where TDocument : class
  {
    public TDocument Document { get; set; }

    void IProxyRequest.WriteJson(
      IElasticsearchSerializer sourceSerializer,
      Stream stream,
      SerializationFormatting formatting)
    {
      sourceSerializer.Serialize<TDocument>(this.Document, stream, formatting);
    }

    protected ICreateRequest<TDocument> Self => (ICreateRequest<TDocument>) this;

    internal override ApiUrls ApiUrls => ApiUrlsLookups.NoNamespaceCreate;

    public CreateRequest(IndexName index, Id id)
      : base((Func<RouteValues, RouteValues>) (r => r.Required(nameof (index), (IUrlParameter) index).Required(nameof (id), (IUrlParameter) id)))
    {
    }

    public CreateRequest(Id id)
      : this((IndexName) typeof (TDocument), id)
    {
    }

    public CreateRequest(TDocument documentWithId, IndexName index = null, Id id = null)
    {
      IndexName index1 = index;
      if ((object) index1 == null)
        index1 = (IndexName) typeof (TDocument);
      Id id1 = id;
      if ((object) id1 == null)
        id1 = Id.From<TDocument>(documentWithId);
      // ISSUE: explicit constructor call
      this.\u002Ector(index1, id1);
      this.DocumentFromPath(documentWithId);
    }

    private void DocumentFromPath(TDocument document) => this.Document = document;

    [SerializationConstructor]
    protected CreateRequest()
    {
    }

    [IgnoreDataMember]
    IndexName ICreateRequest<TDocument>.Index => this.Self.RouteValues.Get<IndexName>("index");

    [IgnoreDataMember]
    Id ICreateRequest<TDocument>.Id => this.Self.RouteValues.Get<Id>("id");

    public string Pipeline
    {
      get => this.Q<string>("pipeline");
      set => this.Q("pipeline", (object) value);
    }

    public Elasticsearch.Net.Refresh? Refresh
    {
      get => this.Q<Elasticsearch.Net.Refresh?>("refresh");
      set => this.Q("refresh", (object) value);
    }

    public Routing Routing
    {
      get => this.Q<Routing>("routing");
      set => this.Q("routing", (object) value);
    }

    public Time Timeout
    {
      get => this.Q<Time>("timeout");
      set => this.Q("timeout", (object) value);
    }

    public long? Version
    {
      get => this.Q<long?>("version");
      set => this.Q("version", (object) value);
    }

    public Elasticsearch.Net.VersionType? VersionType
    {
      get => this.Q<Elasticsearch.Net.VersionType?>("version_type");
      set => this.Q("version_type", (object) value);
    }

    public string WaitForActiveShards
    {
      get => this.Q<string>("wait_for_active_shards");
      set => this.Q("wait_for_active_shards", (object) value);
    }
  }
}
