// Decompiled with JetBrains decompiler
// Type: Nest.ActivateWatchRequest
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
  public class ActivateWatchRequest : 
    PlainRequestBase<ActivateWatchRequestParameters>,
    IActivateWatchRequest,
    IRequest<ActivateWatchRequestParameters>,
    IRequest
  {
    protected IActivateWatchRequest Self => (IActivateWatchRequest) this;

    internal override ApiUrls ApiUrls => ApiUrlsLookups.WatcherActivate;

    public ActivateWatchRequest(Id watchId)
      : base((Func<RouteValues, RouteValues>) (r => r.Required("watch_id", (IUrlParameter) watchId)))
    {
    }

    [SerializationConstructor]
    protected ActivateWatchRequest()
    {
    }

    [IgnoreDataMember]
    Id IActivateWatchRequest.WatchId => this.Self.RouteValues.Get<Id>("watch_id");
  }
}
