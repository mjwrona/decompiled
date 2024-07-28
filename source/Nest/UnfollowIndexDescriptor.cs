// Decompiled with JetBrains decompiler
// Type: Nest.UnfollowIndexDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.CrossClusterReplicationApi;
using Elasticsearch.Net.Utf8Json;
using System;

namespace Nest
{
  public class UnfollowIndexDescriptor : 
    RequestDescriptorBase<UnfollowIndexDescriptor, UnfollowIndexRequestParameters, IUnfollowIndexRequest>,
    IUnfollowIndexRequest,
    IRequest<UnfollowIndexRequestParameters>,
    IRequest
  {
    internal override ApiUrls ApiUrls => ApiUrlsLookups.CrossClusterReplicationUnfollowIndex;

    public UnfollowIndexDescriptor(IndexName index)
      : base((Func<RouteValues, RouteValues>) (r => r.Required(nameof (index), (IUrlParameter) index)))
    {
    }

    [SerializationConstructor]
    protected UnfollowIndexDescriptor()
    {
    }

    IndexName IUnfollowIndexRequest.Index => this.Self.RouteValues.Get<IndexName>("index");

    public UnfollowIndexDescriptor Index(IndexName index) => this.Assign<IndexName>(index, (Action<IUnfollowIndexRequest, IndexName>) ((a, v) => a.RouteValues.Required(nameof (index), (IUrlParameter) v)));

    public UnfollowIndexDescriptor Index<TOther>() where TOther : class => this.Assign<Type>(typeof (TOther), (Action<IUnfollowIndexRequest, Type>) ((a, v) => a.RouteValues.Required("index", (IUrlParameter) (IndexName) v)));
  }
}
