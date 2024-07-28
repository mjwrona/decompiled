// Decompiled with JetBrains decompiler
// Type: Nest.DeleteFilterDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.MachineLearningApi;
using Elasticsearch.Net.Utf8Json;
using System;

namespace Nest
{
  public class DeleteFilterDescriptor : 
    RequestDescriptorBase<DeleteFilterDescriptor, DeleteFilterRequestParameters, IDeleteFilterRequest>,
    IDeleteFilterRequest,
    IRequest<DeleteFilterRequestParameters>,
    IRequest
  {
    internal override ApiUrls ApiUrls => ApiUrlsLookups.MachineLearningDeleteFilter;

    public DeleteFilterDescriptor(Id filterId)
      : base((Func<RouteValues, RouteValues>) (r => r.Required("filter_id", (IUrlParameter) filterId)))
    {
    }

    [SerializationConstructor]
    protected DeleteFilterDescriptor()
    {
    }

    Id IDeleteFilterRequest.FilterId => this.Self.RouteValues.Get<Id>("filter_id");
  }
}
