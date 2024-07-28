// Decompiled with JetBrains decompiler
// Type: Nest.GetDatafeedStatsDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.MachineLearningApi;
using System;

namespace Nest
{
  public class GetDatafeedStatsDescriptor : 
    RequestDescriptorBase<GetDatafeedStatsDescriptor, GetDatafeedStatsRequestParameters, IGetDatafeedStatsRequest>,
    IGetDatafeedStatsRequest,
    IRequest<GetDatafeedStatsRequestParameters>,
    IRequest
  {
    internal override ApiUrls ApiUrls => ApiUrlsLookups.MachineLearningGetDatafeedStats;

    public GetDatafeedStatsDescriptor(Id datafeedId)
      : base((Func<RouteValues, RouteValues>) (r => r.Optional("datafeed_id", (IUrlParameter) datafeedId)))
    {
    }

    public GetDatafeedStatsDescriptor()
    {
    }

    Id IGetDatafeedStatsRequest.DatafeedId => this.Self.RouteValues.Get<Id>("datafeed_id");

    public GetDatafeedStatsDescriptor DatafeedId(Id datafeedId) => this.Assign<Id>(datafeedId, (Action<IGetDatafeedStatsRequest, Id>) ((a, v) => a.RouteValues.Optional("datafeed_id", (IUrlParameter) v)));

    [Obsolete("Scheduled to be removed in 8.0, deprecated")]
    public GetDatafeedStatsDescriptor AllowNoDatafeeds(bool? allownodatafeeds = true) => this.Qs("allow_no_datafeeds", (object) allownodatafeeds);

    public GetDatafeedStatsDescriptor AllowNoMatch(bool? allownomatch = true) => this.Qs("allow_no_match", (object) allownomatch);
  }
}
