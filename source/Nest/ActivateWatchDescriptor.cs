// Decompiled with JetBrains decompiler
// Type: Nest.ActivateWatchDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.WatcherApi;
using Elasticsearch.Net.Utf8Json;
using System;

namespace Nest
{
  public class ActivateWatchDescriptor : 
    RequestDescriptorBase<ActivateWatchDescriptor, ActivateWatchRequestParameters, IActivateWatchRequest>,
    IActivateWatchRequest,
    IRequest<ActivateWatchRequestParameters>,
    IRequest
  {
    internal override ApiUrls ApiUrls => ApiUrlsLookups.WatcherActivate;

    public ActivateWatchDescriptor(Id watchId)
      : base((Func<RouteValues, RouteValues>) (r => r.Required("watch_id", (IUrlParameter) watchId)))
    {
    }

    [SerializationConstructor]
    protected ActivateWatchDescriptor()
    {
    }

    Id IActivateWatchRequest.WatchId => this.Self.RouteValues.Get<Id>("watch_id");
  }
}
