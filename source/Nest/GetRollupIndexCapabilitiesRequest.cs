// Decompiled with JetBrains decompiler
// Type: Nest.GetRollupIndexCapabilitiesRequest
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.RollupApi;
using Elasticsearch.Net.Utf8Json;
using System;
using System.Runtime.Serialization;

namespace Nest
{
  public class GetRollupIndexCapabilitiesRequest : 
    PlainRequestBase<GetRollupIndexCapabilitiesRequestParameters>,
    IGetRollupIndexCapabilitiesRequest,
    IRequest<GetRollupIndexCapabilitiesRequestParameters>,
    IRequest
  {
    protected IGetRollupIndexCapabilitiesRequest Self => (IGetRollupIndexCapabilitiesRequest) this;

    internal override ApiUrls ApiUrls => ApiUrlsLookups.RollupGetIndexCapabilities;

    public GetRollupIndexCapabilitiesRequest(IndexName index)
      : base((Func<RouteValues, RouteValues>) (r => r.Required(nameof (index), (IUrlParameter) index)))
    {
    }

    [SerializationConstructor]
    protected GetRollupIndexCapabilitiesRequest()
    {
    }

    [IgnoreDataMember]
    IndexName IGetRollupIndexCapabilitiesRequest.Index => this.Self.RouteValues.Get<IndexName>("index");
  }
}
