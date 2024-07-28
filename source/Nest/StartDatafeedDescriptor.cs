// Decompiled with JetBrains decompiler
// Type: Nest.StartDatafeedDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.MachineLearningApi;
using Elasticsearch.Net.Utf8Json;
using System;

namespace Nest
{
  public class StartDatafeedDescriptor : 
    RequestDescriptorBase<StartDatafeedDescriptor, StartDatafeedRequestParameters, IStartDatafeedRequest>,
    IStartDatafeedRequest,
    IRequest<StartDatafeedRequestParameters>,
    IRequest
  {
    internal override ApiUrls ApiUrls => ApiUrlsLookups.MachineLearningStartDatafeed;

    public StartDatafeedDescriptor(Id datafeedId)
      : base((Func<RouteValues, RouteValues>) (r => r.Required("datafeed_id", (IUrlParameter) datafeedId)))
    {
    }

    [SerializationConstructor]
    protected StartDatafeedDescriptor()
    {
    }

    Id IStartDatafeedRequest.DatafeedId => this.Self.RouteValues.Get<Id>("datafeed_id");

    DateTimeOffset? IStartDatafeedRequest.End { get; set; }

    DateTimeOffset? IStartDatafeedRequest.Start { get; set; }

    Time IStartDatafeedRequest.Timeout { get; set; }

    public StartDatafeedDescriptor Timeout(Time timeout) => this.Assign<Time>(timeout, (Action<IStartDatafeedRequest, Time>) ((a, v) => a.Timeout = v));

    public StartDatafeedDescriptor Start(DateTimeOffset? start) => this.Assign<DateTimeOffset?>(start, (Action<IStartDatafeedRequest, DateTimeOffset?>) ((a, v) => a.Start = v));

    public StartDatafeedDescriptor End(DateTimeOffset? end) => this.Assign<DateTimeOffset?>(end, (Action<IStartDatafeedRequest, DateTimeOffset?>) ((a, v) => a.End = v));
  }
}
