// Decompiled with JetBrains decompiler
// Type: Nest.FollowInfoDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.CrossClusterReplicationApi;
using Elasticsearch.Net.Utf8Json;
using System;

namespace Nest
{
  public class FollowInfoDescriptor : 
    RequestDescriptorBase<FollowInfoDescriptor, FollowInfoRequestParameters, IFollowInfoRequest>,
    IFollowInfoRequest,
    IRequest<FollowInfoRequestParameters>,
    IRequest
  {
    internal override ApiUrls ApiUrls => ApiUrlsLookups.CrossClusterReplicationFollowInfo;

    public FollowInfoDescriptor(Indices index)
      : base((Func<RouteValues, RouteValues>) (r => r.Required(nameof (index), (IUrlParameter) index)))
    {
    }

    [SerializationConstructor]
    protected FollowInfoDescriptor()
    {
    }

    Indices IFollowInfoRequest.Index => this.Self.RouteValues.Get<Indices>("index");

    public FollowInfoDescriptor Index(Indices index) => this.Assign<Indices>(index, (Action<IFollowInfoRequest, Indices>) ((a, v) => a.RouteValues.Required(nameof (index), (IUrlParameter) v)));

    public FollowInfoDescriptor Index<TOther>() where TOther : class => this.Assign<Type>(typeof (TOther), (Action<IFollowInfoRequest, Type>) ((a, v) => a.RouteValues.Required("index", (IUrlParameter) (Indices) v)));

    public FollowInfoDescriptor AllIndices() => this.Index(Indices.All);
  }
}
