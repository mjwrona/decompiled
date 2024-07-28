// Decompiled with JetBrains decompiler
// Type: Nest.DeleteDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Utf8Json;
using System;

namespace Nest
{
  public class DeleteDescriptor<TDocument> : 
    RequestDescriptorBase<DeleteDescriptor<TDocument>, DeleteRequestParameters, IDeleteRequest<TDocument>>,
    IDeleteRequest<TDocument>,
    IDeleteRequest,
    IRequest<DeleteRequestParameters>,
    IRequest
    where TDocument : class
  {
    internal override ApiUrls ApiUrls => ApiUrlsLookups.NoNamespaceDelete;

    public DeleteDescriptor(IndexName index, Id id)
      : base((Func<RouteValues, RouteValues>) (r => r.Required(nameof (index), (IUrlParameter) index).Required(nameof (id), (IUrlParameter) id)))
    {
    }

    public DeleteDescriptor(Id id)
      : this((IndexName) typeof (TDocument), id)
    {
    }

    public DeleteDescriptor(TDocument documentWithId, IndexName index = null, Id id = null)
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
    protected DeleteDescriptor()
    {
    }

    IndexName IDeleteRequest.Index => this.Self.RouteValues.Get<IndexName>("index");

    Id IDeleteRequest.Id => this.Self.RouteValues.Get<Id>("id");

    public DeleteDescriptor<TDocument> Index(IndexName index) => this.Assign<IndexName>(index, (Action<IDeleteRequest<TDocument>, IndexName>) ((a, v) => a.RouteValues.Required(nameof (index), (IUrlParameter) v)));

    public DeleteDescriptor<TDocument> Index<TOther>() where TOther : class => this.Assign<Type>(typeof (TOther), (Action<IDeleteRequest<TDocument>, Type>) ((a, v) => a.RouteValues.Required("index", (IUrlParameter) (IndexName) v)));

    public DeleteDescriptor<TDocument> IfPrimaryTerm(long? ifprimaryterm) => this.Qs("if_primary_term", (object) ifprimaryterm);

    public DeleteDescriptor<TDocument> IfSequenceNumber(long? ifsequencenumber) => this.Qs("if_seq_no", (object) ifsequencenumber);

    public DeleteDescriptor<TDocument> Refresh(Elasticsearch.Net.Refresh? refresh) => this.Qs(nameof (refresh), (object) refresh);

    public DeleteDescriptor<TDocument> Routing(Nest.Routing routing) => this.Qs(nameof (routing), (object) routing);

    public DeleteDescriptor<TDocument> Timeout(Time timeout) => this.Qs(nameof (timeout), (object) timeout);

    public DeleteDescriptor<TDocument> Version(long? version) => this.Qs(nameof (version), (object) version);

    public DeleteDescriptor<TDocument> VersionType(Elasticsearch.Net.VersionType? versiontype) => this.Qs("version_type", (object) versiontype);

    public DeleteDescriptor<TDocument> WaitForActiveShards(string waitforactiveshards) => this.Qs("wait_for_active_shards", (object) waitforactiveshards);
  }
}
