// Decompiled with JetBrains decompiler
// Type: Nest.DeleteDatafeedDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.MachineLearningApi;
using Elasticsearch.Net.Utf8Json;
using System;

namespace Nest
{
  public class DeleteDatafeedDescriptor : 
    RequestDescriptorBase<DeleteDatafeedDescriptor, DeleteDatafeedRequestParameters, IDeleteDatafeedRequest>,
    IDeleteDatafeedRequest,
    IRequest<DeleteDatafeedRequestParameters>,
    IRequest
  {
    internal override ApiUrls ApiUrls => ApiUrlsLookups.MachineLearningDeleteDatafeed;

    public DeleteDatafeedDescriptor(Id datafeedId)
      : base((Func<RouteValues, RouteValues>) (r => r.Required("datafeed_id", (IUrlParameter) datafeedId)))
    {
    }

    [SerializationConstructor]
    protected DeleteDatafeedDescriptor()
    {
    }

    Id IDeleteDatafeedRequest.DatafeedId => this.Self.RouteValues.Get<Id>("datafeed_id");

    public DeleteDatafeedDescriptor Force(bool? force = true) => this.Qs(nameof (force), (object) force);
  }
}
