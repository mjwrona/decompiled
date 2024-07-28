// Decompiled with JetBrains decompiler
// Type: Nest.IndexDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using System;
using System.IO;

namespace Nest
{
  public class IndexDescriptor<TDocument> : 
    RequestDescriptorBase<IndexDescriptor<TDocument>, IndexRequestParameters, IIndexRequest<TDocument>>,
    IIndexRequest<TDocument>,
    IProxyRequest,
    IRequest,
    IDocumentRequest,
    IRequest<IndexRequestParameters>
    where TDocument : class
  {
    internal override ApiUrls ApiUrls => ApiUrlsLookups.NoNamespaceIndex;

    public IndexDescriptor(IndexName index, Nest.Id id)
      : base((Func<RouteValues, RouteValues>) (r => r.Required(nameof (index), (IUrlParameter) index).Optional(nameof (id), (IUrlParameter) id)))
    {
    }

    public IndexDescriptor(IndexName index)
      : base((Func<RouteValues, RouteValues>) (r => r.Required(nameof (index), (IUrlParameter) index)))
    {
    }

    public IndexDescriptor(Nest.Id id)
      : this((IndexName) typeof (TDocument), id)
    {
    }

    public IndexDescriptor()
      : this((IndexName) typeof (TDocument))
    {
    }

    public IndexDescriptor(TDocument documentWithId, IndexName index = null, Nest.Id id = null)
    {
      IndexName index1 = index;
      if ((object) index1 == null)
        index1 = (IndexName) typeof (TDocument);
      Nest.Id id1 = id;
      if ((object) id1 == null)
        id1 = Nest.Id.From<TDocument>(documentWithId);
      // ISSUE: explicit constructor call
      this.\u002Ector(index1, id1);
      this.DocumentFromPath(documentWithId);
    }

    private void DocumentFromPath(TDocument document) => this.Assign<TDocument>(document, (Action<IIndexRequest<TDocument>, TDocument>) ((a, v) => a.Document = v));

    IndexName IIndexRequest<TDocument>.Index => this.Self.RouteValues.Get<IndexName>("index");

    Nest.Id IIndexRequest<TDocument>.Id => this.Self.RouteValues.Get<Nest.Id>("id");

    public IndexDescriptor<TDocument> Index(IndexName index) => this.Assign<IndexName>(index, (Action<IIndexRequest<TDocument>, IndexName>) ((a, v) => a.RouteValues.Required(nameof (index), (IUrlParameter) v)));

    public IndexDescriptor<TDocument> Index<TOther>() where TOther : class => this.Assign<Type>(typeof (TOther), (Action<IIndexRequest<TDocument>, Type>) ((a, v) => a.RouteValues.Required("index", (IUrlParameter) (IndexName) v)));

    public IndexDescriptor<TDocument> Id(Nest.Id id) => this.Assign<Nest.Id>(id, (Action<IIndexRequest<TDocument>, Nest.Id>) ((a, v) => a.RouteValues.Optional(nameof (id), (IUrlParameter) v)));

    public IndexDescriptor<TDocument> IfPrimaryTerm(long? ifprimaryterm) => this.Qs("if_primary_term", (object) ifprimaryterm);

    public IndexDescriptor<TDocument> IfSequenceNumber(long? ifsequencenumber) => this.Qs("if_seq_no", (object) ifsequencenumber);

    public IndexDescriptor<TDocument> OpType(Elasticsearch.Net.OpType? optype) => this.Qs("op_type", (object) optype);

    public IndexDescriptor<TDocument> Pipeline(string pipeline) => this.Qs(nameof (pipeline), (object) pipeline);

    public IndexDescriptor<TDocument> Refresh(Elasticsearch.Net.Refresh? refresh) => this.Qs(nameof (refresh), (object) refresh);

    public IndexDescriptor<TDocument> RequireAlias(bool? requirealias = true) => this.Qs("require_alias", (object) requirealias);

    public IndexDescriptor<TDocument> Routing(Nest.Routing routing) => this.Qs(nameof (routing), (object) routing);

    public IndexDescriptor<TDocument> Timeout(Time timeout) => this.Qs(nameof (timeout), (object) timeout);

    public IndexDescriptor<TDocument> Version(long? version) => this.Qs(nameof (version), (object) version);

    public IndexDescriptor<TDocument> VersionType(Elasticsearch.Net.VersionType? versiontype) => this.Qs("version_type", (object) versiontype);

    public IndexDescriptor<TDocument> WaitForActiveShards(string waitforactiveshards) => this.Qs("wait_for_active_shards", (object) waitforactiveshards);

    protected override HttpMethod HttpMethod => IndexRequest<TDocument>.GetHttpMethod((IIndexRequest<TDocument>) this);

    TDocument IIndexRequest<TDocument>.Document { get; set; }

    void IProxyRequest.WriteJson(
      IElasticsearchSerializer sourceSerializer,
      Stream stream,
      SerializationFormatting formatting)
    {
      sourceSerializer.Serialize<TDocument>(this.Self.Document, stream, formatting);
    }
  }
}
