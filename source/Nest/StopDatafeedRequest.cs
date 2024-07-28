// Decompiled with JetBrains decompiler
// Type: Nest.StopDatafeedRequest
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
  public class StopDatafeedRequest : 
    PlainRequestBase<StopDatafeedRequestParameters>,
    IStopDatafeedRequest,
    IRequest<StopDatafeedRequestParameters>,
    IRequest
  {
    protected IStopDatafeedRequest Self => (IStopDatafeedRequest) this;

    internal override ApiUrls ApiUrls => ApiUrlsLookups.MachineLearningStopDatafeed;

    public StopDatafeedRequest(Id datafeedId)
      : base((Func<RouteValues, RouteValues>) (r => r.Required("datafeed_id", (IUrlParameter) datafeedId)))
    {
    }

    [SerializationConstructor]
    protected StopDatafeedRequest()
    {
    }

    [IgnoreDataMember]
    Id IStopDatafeedRequest.DatafeedId => this.Self.RouteValues.Get<Id>("datafeed_id");

    [Obsolete("Scheduled to be removed in 8.0, deprecated")]
    public bool? AllowNoDatafeeds
    {
      get => this.Q<bool?>("allow_no_datafeeds");
      set => this.Q("allow_no_datafeeds", (object) value);
    }

    public bool? AllowNoMatch
    {
      get => this.Q<bool?>("allow_no_match");
      set => this.Q("allow_no_match", (object) value);
    }

    public bool? Force { get; set; }

    public Time Timeout { get; set; }
  }
}
