// Decompiled with JetBrains decompiler
// Type: Nest.AcknowledgeWatchDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.WatcherApi;
using Elasticsearch.Net.Utf8Json;
using System;

namespace Nest
{
  public class AcknowledgeWatchDescriptor : 
    RequestDescriptorBase<AcknowledgeWatchDescriptor, AcknowledgeWatchRequestParameters, IAcknowledgeWatchRequest>,
    IAcknowledgeWatchRequest,
    IRequest<AcknowledgeWatchRequestParameters>,
    IRequest
  {
    internal override ApiUrls ApiUrls => ApiUrlsLookups.WatcherAcknowledge;

    public AcknowledgeWatchDescriptor(Id watchId)
      : base((Func<RouteValues, RouteValues>) (r => r.Required("watch_id", (IUrlParameter) watchId)))
    {
    }

    public AcknowledgeWatchDescriptor(Id watchId, Ids actionId)
      : base((Func<RouteValues, RouteValues>) (r => r.Required("watch_id", (IUrlParameter) watchId).Optional("action_id", (IUrlParameter) actionId)))
    {
    }

    [SerializationConstructor]
    protected AcknowledgeWatchDescriptor()
    {
    }

    Id IAcknowledgeWatchRequest.WatchId => this.Self.RouteValues.Get<Id>("watch_id");

    Ids IAcknowledgeWatchRequest.ActionId => this.Self.RouteValues.Get<Ids>("action_id");

    public AcknowledgeWatchDescriptor ActionId(Ids actionId) => this.Assign<Ids>(actionId, (Action<IAcknowledgeWatchRequest, Ids>) ((a, v) => a.RouteValues.Optional("action_id", (IUrlParameter) v)));
  }
}
