// Decompiled with JetBrains decompiler
// Type: Nest.StopDatafeedDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.MachineLearningApi;
using Elasticsearch.Net.Utf8Json;
using System;

namespace Nest
{
  public class StopDatafeedDescriptor : 
    RequestDescriptorBase<StopDatafeedDescriptor, StopDatafeedRequestParameters, IStopDatafeedRequest>,
    IStopDatafeedRequest,
    IRequest<StopDatafeedRequestParameters>,
    IRequest
  {
    internal override ApiUrls ApiUrls => ApiUrlsLookups.MachineLearningStopDatafeed;

    public StopDatafeedDescriptor(Id datafeedId)
      : base((Func<RouteValues, RouteValues>) (r => r.Required("datafeed_id", (IUrlParameter) datafeedId)))
    {
    }

    [SerializationConstructor]
    protected StopDatafeedDescriptor()
    {
    }

    Id IStopDatafeedRequest.DatafeedId => this.Self.RouteValues.Get<Id>("datafeed_id");

    [Obsolete("Scheduled to be removed in 8.0, deprecated")]
    public StopDatafeedDescriptor AllowNoDatafeeds(bool? allownodatafeeds = true) => this.Qs("allow_no_datafeeds", (object) allownodatafeeds);

    public StopDatafeedDescriptor AllowNoMatch(bool? allownomatch = true) => this.Qs("allow_no_match", (object) allownomatch);

    bool? IStopDatafeedRequest.Force { get; set; }

    Time IStopDatafeedRequest.Timeout { get; set; }

    public StopDatafeedDescriptor Timeout(Time timeout) => this.Assign<Time>(timeout, (Action<IStopDatafeedRequest, Time>) ((a, v) => a.Timeout = v));

    public StopDatafeedDescriptor Force(bool? force = true) => this.Assign<bool?>(force, (Action<IStopDatafeedRequest, bool?>) ((a, v) => a.Force = v));
  }
}
