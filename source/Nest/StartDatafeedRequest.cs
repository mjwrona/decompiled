// Decompiled with JetBrains decompiler
// Type: Nest.StartDatafeedRequest
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.MachineLearningApi;
using Elasticsearch.Net.Utf8Json;
using System;
using System.Runtime.Serialization;

namespace Nest
{
  public class StartDatafeedRequest : 
    PlainRequestBase<StartDatafeedRequestParameters>,
    IStartDatafeedRequest,
    IRequest<StartDatafeedRequestParameters>,
    IRequest
  {
    protected IStartDatafeedRequest Self => (IStartDatafeedRequest) this;

    internal override ApiUrls ApiUrls => ApiUrlsLookups.MachineLearningStartDatafeed;

    public StartDatafeedRequest(Id datafeedId)
      : base((Func<RouteValues, RouteValues>) (r => r.Required("datafeed_id", (IUrlParameter) datafeedId)))
    {
    }

    [SerializationConstructor]
    protected StartDatafeedRequest()
    {
    }

    [IgnoreDataMember]
    Id IStartDatafeedRequest.DatafeedId => this.Self.RouteValues.Get<Id>("datafeed_id");

    public DateTimeOffset? End { get; set; }

    public DateTimeOffset? Start { get; set; }

    public Time Timeout { get; set; }
  }
}
