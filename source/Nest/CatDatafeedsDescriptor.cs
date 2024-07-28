// Decompiled with JetBrains decompiler
// Type: Nest.CatDatafeedsDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.CatApi;
using System;

namespace Nest
{
  public class CatDatafeedsDescriptor : 
    RequestDescriptorBase<CatDatafeedsDescriptor, CatDatafeedsRequestParameters, ICatDatafeedsRequest>,
    ICatDatafeedsRequest,
    IRequest<CatDatafeedsRequestParameters>,
    IRequest
  {
    internal override ApiUrls ApiUrls => ApiUrlsLookups.CatDatafeeds;

    public CatDatafeedsDescriptor()
    {
    }

    public CatDatafeedsDescriptor(Id datafeedId)
      : base((Func<RouteValues, RouteValues>) (r => r.Optional("datafeed_id", (IUrlParameter) datafeedId)))
    {
    }

    Id ICatDatafeedsRequest.DatafeedId => this.Self.RouteValues.Get<Id>("datafeed_id");

    public CatDatafeedsDescriptor DatafeedId(Id datafeedId) => this.Assign<Id>(datafeedId, (Action<ICatDatafeedsRequest, Id>) ((a, v) => a.RouteValues.Optional("datafeed_id", (IUrlParameter) v)));

    [Obsolete("Scheduled to be removed in 8.0, deprecated")]
    public CatDatafeedsDescriptor AllowNoDatafeeds(bool? allownodatafeeds = true) => this.Qs("allow_no_datafeeds", (object) allownodatafeeds);

    public CatDatafeedsDescriptor AllowNoMatch(bool? allownomatch = true) => this.Qs("allow_no_match", (object) allownomatch);

    public CatDatafeedsDescriptor Format(string format) => this.Qs(nameof (format), (object) format);

    public CatDatafeedsDescriptor Headers(params string[] headers) => this.Qs("h", (object) headers);

    public CatDatafeedsDescriptor Help(bool? help = true) => this.Qs(nameof (help), (object) help);

    public CatDatafeedsDescriptor SortByColumns(params string[] sortbycolumns) => this.Qs("s", (object) sortbycolumns);

    public CatDatafeedsDescriptor Verbose(bool? verbose = true) => this.Qs("v", (object) verbose);
  }
}
