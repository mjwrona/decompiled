// Decompiled with JetBrains decompiler
// Type: Nest.CreateDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Utf8Json;
using System;
using System.IO;

namespace Nest
{
  public class CreateDescriptor<TDocument> : 
    RequestDescriptorBase<CreateDescriptor<TDocument>, CreateRequestParameters, ICreateRequest<TDocument>>,
    ICreateRequest<TDocument>,
    IProxyRequest,
    IRequest,
    IDocumentRequest,
    IRequest<CreateRequestParameters>
    where TDocument : class
  {
    internal override ApiUrls ApiUrls => ApiUrlsLookups.NoNamespaceCreate;

    public CreateDescriptor(IndexName index, Nest.Id id)
      : base((Func<RouteValues, RouteValues>) (r => r.Required(nameof (index), (IUrlParameter) index).Required(nameof (id), (IUrlParameter) id)))
    {
    }

    public CreateDescriptor(Nest.Id id)
      : this((IndexName) typeof (TDocument), id)
    {
    }

    public CreateDescriptor(TDocument documentWithId, IndexName index = null, Nest.Id id = null)
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

    private void DocumentFromPath(TDocument document) => this.Assign<TDocument>(document, (Action<ICreateRequest<TDocument>, TDocument>) ((a, v) => a.Document = v));

    [SerializationConstructor]
    protected CreateDescriptor()
    {
    }

    IndexName ICreateRequest<TDocument>.Index => this.Self.RouteValues.Get<IndexName>("index");

    Nest.Id ICreateRequest<TDocument>.Id => this.Self.RouteValues.Get<Nest.Id>("id");

    public CreateDescriptor<TDocument> Index(IndexName index) => this.Assign<IndexName>(index, (Action<ICreateRequest<TDocument>, IndexName>) ((a, v) => a.RouteValues.Required(nameof (index), (IUrlParameter) v)));

    public CreateDescriptor<TDocument> Index<TOther>() where TOther : class => this.Assign<Type>(typeof (TOther), (Action<ICreateRequest<TDocument>, Type>) ((a, v) => a.RouteValues.Required("index", (IUrlParameter) (IndexName) v)));

    public CreateDescriptor<TDocument> Pipeline(string pipeline) => this.Qs(nameof (pipeline), (object) pipeline);

    public CreateDescriptor<TDocument> Refresh(Elasticsearch.Net.Refresh? refresh) => this.Qs(nameof (refresh), (object) refresh);

    public CreateDescriptor<TDocument> Routing(Nest.Routing routing) => this.Qs(nameof (routing), (object) routing);

    public CreateDescriptor<TDocument> Timeout(Time timeout) => this.Qs(nameof (timeout), (object) timeout);

    public CreateDescriptor<TDocument> Version(long? version) => this.Qs(nameof (version), (object) version);

    public CreateDescriptor<TDocument> VersionType(Elasticsearch.Net.VersionType? versiontype) => this.Qs("version_type", (object) versiontype);

    public CreateDescriptor<TDocument> WaitForActiveShards(string waitforactiveshards) => this.Qs("wait_for_active_shards", (object) waitforactiveshards);

    TDocument ICreateRequest<TDocument>.Document { get; set; }

    void IProxyRequest.WriteJson(
      IElasticsearchSerializer sourceSerializer,
      Stream stream,
      SerializationFormatting formatting)
    {
      sourceSerializer.Serialize<TDocument>(this.Self.Document, stream, formatting);
    }

    public CreateDescriptor<TDocument> Id(Nest.Id id) => this.Assign<Nest.Id>(id, (Action<ICreateRequest<TDocument>, Nest.Id>) ((a, v) => a.RouteValues.Required(nameof (id), (IUrlParameter) v)));
  }
}
