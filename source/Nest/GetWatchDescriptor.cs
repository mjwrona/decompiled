// Decompiled with JetBrains decompiler
// Type: Nest.GetWatchDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.WatcherApi;
using Elasticsearch.Net.Utf8Json;
using System;

namespace Nest
{
  public class GetWatchDescriptor : 
    RequestDescriptorBase<GetWatchDescriptor, GetWatchRequestParameters, IGetWatchRequest>,
    IGetWatchRequest,
    IRequest<GetWatchRequestParameters>,
    IRequest
  {
    internal override ApiUrls ApiUrls => ApiUrlsLookups.WatcherGet;

    public GetWatchDescriptor(Id id)
      : base((Func<RouteValues, RouteValues>) (r => r.Required(nameof (id), (IUrlParameter) id)))
    {
    }

    [SerializationConstructor]
    protected GetWatchDescriptor()
    {
    }

    Id IGetWatchRequest.Id => this.Self.RouteValues.Get<Id>("id");
  }
}
