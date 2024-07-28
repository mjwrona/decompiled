// Decompiled with JetBrains decompiler
// Type: Nest.GetDatafeedsRequest
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.MachineLearningApi;
using System;
using System.Runtime.Serialization;

namespace Nest
{
  public class GetDatafeedsRequest : 
    PlainRequestBase<GetDatafeedsRequestParameters>,
    IGetDatafeedsRequest,
    IRequest<GetDatafeedsRequestParameters>,
    IRequest
  {
    protected IGetDatafeedsRequest Self => (IGetDatafeedsRequest) this;

    internal override ApiUrls ApiUrls => ApiUrlsLookups.MachineLearningGetDatafeeds;

    public GetDatafeedsRequest(Id datafeedId)
      : base((Func<RouteValues, RouteValues>) (r => r.Optional("datafeed_id", (IUrlParameter) datafeedId)))
    {
    }

    public GetDatafeedsRequest()
    {
    }

    [IgnoreDataMember]
    Id IGetDatafeedsRequest.DatafeedId => this.Self.RouteValues.Get<Id>("datafeed_id");

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

    public bool? ExcludeGenerated
    {
      get => this.Q<bool?>("exclude_generated");
      set => this.Q("exclude_generated", (object) value);
    }
  }
}
