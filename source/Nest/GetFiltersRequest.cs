// Decompiled with JetBrains decompiler
// Type: Nest.GetFiltersRequest
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.MachineLearningApi;
using System;
using System.Runtime.Serialization;

namespace Nest
{
  public class GetFiltersRequest : 
    PlainRequestBase<GetFiltersRequestParameters>,
    IGetFiltersRequest,
    IRequest<GetFiltersRequestParameters>,
    IRequest
  {
    protected IGetFiltersRequest Self => (IGetFiltersRequest) this;

    internal override ApiUrls ApiUrls => ApiUrlsLookups.MachineLearningGetFilters;

    public GetFiltersRequest()
    {
    }

    public GetFiltersRequest(Id filterId)
      : base((Func<RouteValues, RouteValues>) (r => r.Optional("filter_id", (IUrlParameter) filterId)))
    {
    }

    [IgnoreDataMember]
    Id IGetFiltersRequest.FilterId => this.Self.RouteValues.Get<Id>("filter_id");

    public int? From
    {
      get => this.Q<int?>("from");
      set => this.Q("from", (object) value);
    }

    public int? Size
    {
      get => this.Q<int?>("size");
      set => this.Q("size", (object) value);
    }
  }
}
