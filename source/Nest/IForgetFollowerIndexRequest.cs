// Decompiled with JetBrains decompiler
// Type: Nest.IForgetFollowerIndexRequest
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Specification.CrossClusterReplicationApi;
using Elasticsearch.Net.Utf8Json;
using System.Runtime.Serialization;

namespace Nest
{
  [InterfaceDataContract]
  [MapsApi("ccr.forget_follower.json")]
  [ReadAs(typeof (ForgetFollowerIndexRequest))]
  public interface IForgetFollowerIndexRequest : 
    IRequest<ForgetFollowerIndexRequestParameters>,
    IRequest
  {
    [IgnoreDataMember]
    IndexName Index { get; }

    [DataMember(Name = "follower_cluster")]
    string FollowerCluster { get; set; }

    [DataMember(Name = "follower_index")]
    IndexName FollowerIndex { get; set; }

    [DataMember(Name = "follower_index_uuid")]
    string FollowerIndexUUID { get; set; }

    [DataMember(Name = "leader_remote_cluster")]
    string LeaderRemoteCluster { get; set; }
  }
}
