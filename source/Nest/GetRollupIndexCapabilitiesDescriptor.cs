// Decompiled with JetBrains decompiler
// Type: Nest.GetRollupIndexCapabilitiesDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.RollupApi;
using Elasticsearch.Net.Utf8Json;
using System;

namespace Nest
{
  public class GetRollupIndexCapabilitiesDescriptor : 
    RequestDescriptorBase<GetRollupIndexCapabilitiesDescriptor, GetRollupIndexCapabilitiesRequestParameters, IGetRollupIndexCapabilitiesRequest>,
    IGetRollupIndexCapabilitiesRequest,
    IRequest<GetRollupIndexCapabilitiesRequestParameters>,
    IRequest
  {
    internal override ApiUrls ApiUrls => ApiUrlsLookups.RollupGetIndexCapabilities;

    public GetRollupIndexCapabilitiesDescriptor(IndexName index)
      : base((Func<RouteValues, RouteValues>) (r => r.Required(nameof (index), (IUrlParameter) index)))
    {
    }

    [SerializationConstructor]
    protected GetRollupIndexCapabilitiesDescriptor()
    {
    }

    IndexName IGetRollupIndexCapabilitiesRequest.Index => this.Self.RouteValues.Get<IndexName>("index");

    public GetRollupIndexCapabilitiesDescriptor Index(IndexName index) => this.Assign<IndexName>(index, (Action<IGetRollupIndexCapabilitiesRequest, IndexName>) ((a, v) => a.RouteValues.Required(nameof (index), (IUrlParameter) v)));

    public GetRollupIndexCapabilitiesDescriptor Index<TOther>() where TOther : class => this.Assign<Type>(typeof (TOther), (Action<IGetRollupIndexCapabilitiesRequest, Type>) ((a, v) => a.RouteValues.Required("index", (IUrlParameter) (IndexName) v)));
  }
}
