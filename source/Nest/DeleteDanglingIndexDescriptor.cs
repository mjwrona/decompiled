// Decompiled with JetBrains decompiler
// Type: Nest.DeleteDanglingIndexDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.DanglingIndicesApi;
using Elasticsearch.Net.Utf8Json;
using System;

namespace Nest
{
  public class DeleteDanglingIndexDescriptor : 
    RequestDescriptorBase<DeleteDanglingIndexDescriptor, DeleteDanglingIndexRequestParameters, IDeleteDanglingIndexRequest>,
    IDeleteDanglingIndexRequest,
    IRequest<DeleteDanglingIndexRequestParameters>,
    IRequest
  {
    internal override ApiUrls ApiUrls => ApiUrlsLookups.DanglingIndicesDeleteDanglingIndex;

    public DeleteDanglingIndexDescriptor(IndexUuid indexUuid)
      : base((Func<RouteValues, RouteValues>) (r => r.Required("index_uuid", (IUrlParameter) indexUuid)))
    {
    }

    [SerializationConstructor]
    protected DeleteDanglingIndexDescriptor()
    {
    }

    IndexUuid IDeleteDanglingIndexRequest.IndexUuid => this.Self.RouteValues.Get<IndexUuid>("index_uuid");

    public DeleteDanglingIndexDescriptor AcceptDataLoss(bool? acceptdataloss = true) => this.Qs("accept_data_loss", (object) acceptdataloss);

    public DeleteDanglingIndexDescriptor MasterTimeout(Time mastertimeout) => this.Qs("master_timeout", (object) mastertimeout);

    public DeleteDanglingIndexDescriptor Timeout(Time timeout) => this.Qs(nameof (timeout), (object) timeout);
  }
}
