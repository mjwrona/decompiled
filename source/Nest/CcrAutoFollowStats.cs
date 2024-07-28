// Decompiled with JetBrains decompiler
// Type: Nest.CcrAutoFollowStats
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Nest
{
  public class CcrAutoFollowStats
  {
    [DataMember(Name = "number_of_failed_follow_indices")]
    public long NumberOfFailedFollowIndices { get; internal set; }

    [DataMember(Name = "number_of_failed_remote_cluster_state_requests")]
    public long NumberOfFailedRemoteClusterStateRequests { get; internal set; }

    [DataMember(Name = "number_of_successful_follow_indices")]
    public long NumberOfSuccessfulFollowIndices { get; internal set; }

    [DataMember(Name = "recent_auto_follow_errors")]
    public IReadOnlyCollection<ErrorCause> RecentAutoFollowErrors { get; internal set; } = EmptyReadOnly<ErrorCause>.Collection;

    [DataMember(Name = "auto_followed_clusters")]
    public IReadOnlyCollection<AutoFollowedCluster> AutoFollowedClusters { get; internal set; } = EmptyReadOnly<AutoFollowedCluster>.Collection;
  }
}
