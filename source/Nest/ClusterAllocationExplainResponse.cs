// Decompiled with JetBrains decompiler
// Type: Nest.ClusterAllocationExplainResponse
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Nest
{
  [DataContract]
  public class ClusterAllocationExplainResponse : ResponseBase
  {
    [DataMember(Name = "allocate_explanation")]
    public string AllocateExplanation { get; internal set; }

    [DataMember(Name = "allocation_delay")]
    public string AllocationDelay { get; internal set; }

    [DataMember(Name = "allocation_delay_in_millis")]
    public long AllocationDelayInMilliseconds { get; internal set; }

    [DataMember(Name = "can_allocate")]
    public Decision? CanAllocate { get; internal set; }

    [DataMember(Name = "can_move_to_other_node")]
    public Decision? CanMoveToOtherNode { get; internal set; }

    [DataMember(Name = "can_rebalance_cluster")]
    public Decision? CanRebalanceCluster { get; internal set; }

    [DataMember(Name = "can_rebalance_cluster_decisions")]
    public IReadOnlyCollection<AllocationDecision> CanRebalanceClusterDecisions { get; internal set; } = EmptyReadOnly<AllocationDecision>.Collection;

    [DataMember(Name = "can_rebalance_to_other_node")]
    public Decision? CanRebalanceToOtherNode { get; internal set; }

    [DataMember(Name = "can_remain_decisions")]
    public IReadOnlyCollection<AllocationDecision> CanRemainDecisions { get; internal set; } = EmptyReadOnly<AllocationDecision>.Collection;

    [DataMember(Name = "can_remain_on_current_node")]
    public Decision? CanRemainOnCurrentNode { get; internal set; }

    [DataMember(Name = "configured_delay")]
    public string ConfiguredDelay { get; internal set; }

    [DataMember(Name = "configured_delay_in_mills")]
    public long ConfiguredDelayInMilliseconds { get; internal set; }

    [DataMember(Name = "current_node")]
    public CurrentNode CurrentNode { get; internal set; }

    [DataMember(Name = "current_state")]
    public string CurrentState { get; internal set; }

    [DataMember(Name = "index")]
    public string Index { get; internal set; }

    [DataMember(Name = "move_explanation")]
    public string MoveExplanation { get; internal set; }

    [DataMember(Name = "node_allocation_decisions")]
    public IReadOnlyCollection<NodeAllocationExplanation> NodeAllocationDecisions { get; internal set; } = EmptyReadOnly<NodeAllocationExplanation>.Collection;

    [DataMember(Name = "primary")]
    public bool Primary { get; internal set; }

    [DataMember(Name = "rebalance_explanation")]
    public string RebalanceExplanation { get; internal set; }

    [DataMember(Name = "remaining_delay")]
    public string RemainingDelay { get; internal set; }

    [DataMember(Name = "remaining_delay_in_millis")]
    public long RemainingDelayInMilliseconds { get; internal set; }

    [DataMember(Name = "shard")]
    public int Shard { get; internal set; }

    [DataMember(Name = "unassigned_info")]
    public UnassignedInformation UnassignedInformation { get; internal set; }
  }
}
