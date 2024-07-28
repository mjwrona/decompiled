// Decompiled with JetBrains decompiler
// Type: Nest.GetRollupJobDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.RollupApi;
using System;

namespace Nest
{
  public class GetRollupJobDescriptor : 
    RequestDescriptorBase<GetRollupJobDescriptor, GetRollupJobRequestParameters, IGetRollupJobRequest>,
    IGetRollupJobRequest,
    IRequest<GetRollupJobRequestParameters>,
    IRequest
  {
    internal override ApiUrls ApiUrls => ApiUrlsLookups.RollupGetJob;

    public GetRollupJobDescriptor(Nest.Id id)
      : base((Func<RouteValues, RouteValues>) (r => r.Optional(nameof (id), (IUrlParameter) id)))
    {
    }

    public GetRollupJobDescriptor()
    {
    }

    Nest.Id IGetRollupJobRequest.Id => this.Self.RouteValues.Get<Nest.Id>("id");

    public GetRollupJobDescriptor Id(Nest.Id id) => this.Assign<Nest.Id>(id, (Action<IGetRollupJobRequest, Nest.Id>) ((a, v) => a.RouteValues.Optional(nameof (id), (IUrlParameter) v)));
  }
}
