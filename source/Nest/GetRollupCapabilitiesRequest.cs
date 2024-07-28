// Decompiled with JetBrains decompiler
// Type: Nest.GetRollupCapabilitiesRequest
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.RollupApi;
using System;
using System.Runtime.Serialization;

namespace Nest
{
  public class GetRollupCapabilitiesRequest : 
    PlainRequestBase<GetRollupCapabilitiesRequestParameters>,
    IGetRollupCapabilitiesRequest,
    IRequest<GetRollupCapabilitiesRequestParameters>,
    IRequest
  {
    protected IGetRollupCapabilitiesRequest Self => (IGetRollupCapabilitiesRequest) this;

    internal override ApiUrls ApiUrls => ApiUrlsLookups.RollupGetCapabilities;

    public GetRollupCapabilitiesRequest(Id id)
      : base((Func<RouteValues, RouteValues>) (r => r.Optional(nameof (id), (IUrlParameter) id)))
    {
    }

    public GetRollupCapabilitiesRequest()
    {
    }

    [IgnoreDataMember]
    Id IGetRollupCapabilitiesRequest.Id => this.Self.RouteValues.Get<Id>("id");
  }
}
