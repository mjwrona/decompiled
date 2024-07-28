// Decompiled with JetBrains decompiler
// Type: Nest.GetFiltersDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.MachineLearningApi;
using System;

namespace Nest
{
  public class GetFiltersDescriptor : 
    RequestDescriptorBase<GetFiltersDescriptor, GetFiltersRequestParameters, IGetFiltersRequest>,
    IGetFiltersRequest,
    IRequest<GetFiltersRequestParameters>,
    IRequest
  {
    internal override ApiUrls ApiUrls => ApiUrlsLookups.MachineLearningGetFilters;

    public GetFiltersDescriptor()
    {
    }

    public GetFiltersDescriptor(Id filterId)
      : base((Func<RouteValues, RouteValues>) (r => r.Optional("filter_id", (IUrlParameter) filterId)))
    {
    }

    Id IGetFiltersRequest.FilterId => this.Self.RouteValues.Get<Id>("filter_id");

    public GetFiltersDescriptor FilterId(Id filterId) => this.Assign<Id>(filterId, (Action<IGetFiltersRequest, Id>) ((a, v) => a.RouteValues.Optional("filter_id", (IUrlParameter) v)));

    public GetFiltersDescriptor From(int? from) => this.Qs(nameof (from), (object) from);

    public GetFiltersDescriptor Size(int? size) => this.Qs(nameof (size), (object) size);
  }
}
