// Decompiled with JetBrains decompiler
// Type: Nest.PreviewDatafeedDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.MachineLearningApi;
using System;

namespace Nest
{
  public class PreviewDatafeedDescriptor : 
    RequestDescriptorBase<PreviewDatafeedDescriptor, PreviewDatafeedRequestParameters, IPreviewDatafeedRequest>,
    IPreviewDatafeedRequest,
    IRequest<PreviewDatafeedRequestParameters>,
    IRequest
  {
    internal override ApiUrls ApiUrls => ApiUrlsLookups.MachineLearningPreviewDatafeed;

    public PreviewDatafeedDescriptor(Id datafeedId)
      : base((Func<RouteValues, RouteValues>) (r => r.Optional("datafeed_id", (IUrlParameter) datafeedId)))
    {
    }

    public PreviewDatafeedDescriptor()
    {
    }

    Id IPreviewDatafeedRequest.DatafeedId => this.Self.RouteValues.Get<Id>("datafeed_id");

    public PreviewDatafeedDescriptor DatafeedId(Id datafeedId) => this.Assign<Id>(datafeedId, (Action<IPreviewDatafeedRequest, Id>) ((a, v) => a.RouteValues.Optional("datafeed_id", (IUrlParameter) v)));
  }
}
