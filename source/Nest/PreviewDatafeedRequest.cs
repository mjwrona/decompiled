// Decompiled with JetBrains decompiler
// Type: Nest.PreviewDatafeedRequest
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.MachineLearningApi;
using System;
using System.Runtime.Serialization;

namespace Nest
{
  public class PreviewDatafeedRequest : 
    PlainRequestBase<PreviewDatafeedRequestParameters>,
    IPreviewDatafeedRequest,
    IRequest<PreviewDatafeedRequestParameters>,
    IRequest
  {
    protected IPreviewDatafeedRequest Self => (IPreviewDatafeedRequest) this;

    internal override ApiUrls ApiUrls => ApiUrlsLookups.MachineLearningPreviewDatafeed;

    public PreviewDatafeedRequest(Id datafeedId)
      : base((Func<RouteValues, RouteValues>) (r => r.Optional("datafeed_id", (IUrlParameter) datafeedId)))
    {
    }

    public PreviewDatafeedRequest()
    {
    }

    [IgnoreDataMember]
    Id IPreviewDatafeedRequest.DatafeedId => this.Self.RouteValues.Get<Id>("datafeed_id");
  }
}
