// Decompiled with JetBrains decompiler
// Type: Nest.ForgetFollowerIndexDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Specification.CrossClusterReplicationApi;
using Elasticsearch.Net.Utf8Json;
using System;

namespace Nest
{
  public class ForgetFollowerIndexDescriptor : 
    RequestDescriptorBase<ForgetFollowerIndexDescriptor, ForgetFollowerIndexRequestParameters, IForgetFollowerIndexRequest>,
    IForgetFollowerIndexRequest,
    IRequest<ForgetFollowerIndexRequestParameters>,
    IRequest
  {
    internal override ApiUrls ApiUrls => ApiUrlsLookups.CrossClusterReplicationForgetFollowerIndex;

    public ForgetFollowerIndexDescriptor(IndexName index)
      : base((Func<RouteValues, RouteValues>) (r => r.Required(nameof (index), (IUrlParameter) index)))
    {
    }

    [SerializationConstructor]
    protected ForgetFollowerIndexDescriptor()
    {
    }

    IndexName IForgetFollowerIndexRequest.Index => this.Self.RouteValues.Get<IndexName>("index");

    public ForgetFollowerIndexDescriptor Index(IndexName index) => this.Assign<IndexName>(index, (Action<IForgetFollowerIndexRequest, IndexName>) ((a, v) => a.RouteValues.Required(nameof (index), (IUrlParameter) v)));

    public ForgetFollowerIndexDescriptor Index<TOther>() where TOther : class => this.Assign<Type>(typeof (TOther), (Action<IForgetFollowerIndexRequest, Type>) ((a, v) => a.RouteValues.Required("index", (IUrlParameter) (IndexName) v)));

    string IForgetFollowerIndexRequest.FollowerCluster { get; set; }

    IndexName IForgetFollowerIndexRequest.FollowerIndex { get; set; }

    string IForgetFollowerIndexRequest.FollowerIndexUUID { get; set; }

    string IForgetFollowerIndexRequest.LeaderRemoteCluster { get; set; }

    public ForgetFollowerIndexDescriptor FollowerCluster(string followerCluster) => this.Assign<string>(followerCluster, (Action<IForgetFollowerIndexRequest, string>) ((a, v) => a.FollowerCluster = v));

    public ForgetFollowerIndexDescriptor FollowerIndex(IndexName followerIndex) => this.Assign<IndexName>(followerIndex, (Action<IForgetFollowerIndexRequest, IndexName>) ((a, v) => a.FollowerIndex = v));

    public ForgetFollowerIndexDescriptor FollowerIndexUUID(string followerIndexUUID) => this.Assign<string>(followerIndexUUID, (Action<IForgetFollowerIndexRequest, string>) ((a, v) => a.FollowerIndexUUID = v));

    public ForgetFollowerIndexDescriptor LeaderRemoteCluster(string leaderRemoteCluster) => this.Assign<string>(leaderRemoteCluster, (Action<IForgetFollowerIndexRequest, string>) ((a, v) => a.LeaderRemoteCluster = v));
  }
}
