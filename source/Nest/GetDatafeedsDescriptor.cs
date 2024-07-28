// Decompiled with JetBrains decompiler
// Type: Nest.GetDatafeedsDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.MachineLearningApi;
using System;

namespace Nest
{
  public class GetDatafeedsDescriptor : 
    RequestDescriptorBase<GetDatafeedsDescriptor, GetDatafeedsRequestParameters, IGetDatafeedsRequest>,
    IGetDatafeedsRequest,
    IRequest<GetDatafeedsRequestParameters>,
    IRequest
  {
    internal override ApiUrls ApiUrls => ApiUrlsLookups.MachineLearningGetDatafeeds;

    public GetDatafeedsDescriptor(Id datafeedId)
      : base((Func<RouteValues, RouteValues>) (r => r.Optional("datafeed_id", (IUrlParameter) datafeedId)))
    {
    }

    public GetDatafeedsDescriptor()
    {
    }

    Id IGetDatafeedsRequest.DatafeedId => this.Self.RouteValues.Get<Id>("datafeed_id");

    public GetDatafeedsDescriptor DatafeedId(Id datafeedId) => this.Assign<Id>(datafeedId, (Action<IGetDatafeedsRequest, Id>) ((a, v) => a.RouteValues.Optional("datafeed_id", (IUrlParameter) v)));

    [Obsolete("Scheduled to be removed in 8.0, deprecated")]
    public GetDatafeedsDescriptor AllowNoDatafeeds(bool? allownodatafeeds = true) => this.Qs("allow_no_datafeeds", (object) allownodatafeeds);

    public GetDatafeedsDescriptor AllowNoMatch(bool? allownomatch = true) => this.Qs("allow_no_match", (object) allownomatch);

    public GetDatafeedsDescriptor ExcludeGenerated(bool? excludegenerated = true) => this.Qs("exclude_generated", (object) excludegenerated);
  }
}
