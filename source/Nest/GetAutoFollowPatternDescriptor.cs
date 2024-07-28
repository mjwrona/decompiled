// Decompiled with JetBrains decompiler
// Type: Nest.GetAutoFollowPatternDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.CrossClusterReplicationApi;
using System;

namespace Nest
{
  public class GetAutoFollowPatternDescriptor : 
    RequestDescriptorBase<GetAutoFollowPatternDescriptor, GetAutoFollowPatternRequestParameters, IGetAutoFollowPatternRequest>,
    IGetAutoFollowPatternRequest,
    IRequest<GetAutoFollowPatternRequestParameters>,
    IRequest
  {
    internal override ApiUrls ApiUrls => ApiUrlsLookups.CrossClusterReplicationGetAutoFollowPattern;

    public GetAutoFollowPatternDescriptor()
    {
    }

    public GetAutoFollowPatternDescriptor(Nest.Name name)
      : base((Func<RouteValues, RouteValues>) (r => r.Optional(nameof (name), (IUrlParameter) name)))
    {
    }

    Nest.Name IGetAutoFollowPatternRequest.Name => this.Self.RouteValues.Get<Nest.Name>("name");

    public GetAutoFollowPatternDescriptor Name(Nest.Name name) => this.Assign<Nest.Name>(name, (Action<IGetAutoFollowPatternRequest, Nest.Name>) ((a, v) => a.RouteValues.Optional(nameof (name), (IUrlParameter) v)));
  }
}
