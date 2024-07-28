// Decompiled with JetBrains decompiler
// Type: Nest.DeleteWatchDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.WatcherApi;
using Elasticsearch.Net.Utf8Json;
using System;

namespace Nest
{
  public class DeleteWatchDescriptor : 
    RequestDescriptorBase<DeleteWatchDescriptor, DeleteWatchRequestParameters, IDeleteWatchRequest>,
    IDeleteWatchRequest,
    IRequest<DeleteWatchRequestParameters>,
    IRequest
  {
    internal override ApiUrls ApiUrls => ApiUrlsLookups.WatcherDelete;

    public DeleteWatchDescriptor(Id id)
      : base((Func<RouteValues, RouteValues>) (r => r.Required(nameof (id), (IUrlParameter) id)))
    {
    }

    [SerializationConstructor]
    protected DeleteWatchDescriptor()
    {
    }

    Id IDeleteWatchRequest.Id => this.Self.RouteValues.Get<Id>("id");
  }
}
