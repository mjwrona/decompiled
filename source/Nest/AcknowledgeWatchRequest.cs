// Decompiled with JetBrains decompiler
// Type: Nest.AcknowledgeWatchRequest
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.WatcherApi;
using Elasticsearch.Net.Utf8Json;
using System;
using System.Runtime.Serialization;

namespace Nest
{
  public class AcknowledgeWatchRequest : 
    PlainRequestBase<AcknowledgeWatchRequestParameters>,
    IAcknowledgeWatchRequest,
    IRequest<AcknowledgeWatchRequestParameters>,
    IRequest
  {
    protected IAcknowledgeWatchRequest Self => (IAcknowledgeWatchRequest) this;

    internal override ApiUrls ApiUrls => ApiUrlsLookups.WatcherAcknowledge;

    public AcknowledgeWatchRequest(Id watchId)
      : base((Func<RouteValues, RouteValues>) (r => r.Required("watch_id", (IUrlParameter) watchId)))
    {
    }

    public AcknowledgeWatchRequest(Id watchId, Ids actionId)
      : base((Func<RouteValues, RouteValues>) (r => r.Required("watch_id", (IUrlParameter) watchId).Optional("action_id", (IUrlParameter) actionId)))
    {
    }

    [SerializationConstructor]
    protected AcknowledgeWatchRequest()
    {
    }

    [IgnoreDataMember]
    Id IAcknowledgeWatchRequest.WatchId => this.Self.RouteValues.Get<Id>("watch_id");

    [IgnoreDataMember]
    Ids IAcknowledgeWatchRequest.ActionId => this.Self.RouteValues.Get<Ids>("action_id");
  }
}
