// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.WebApi.ElasticNode
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.DistributedTask.WebApi
{
  [DataContract]
  public class ElasticNode
  {
    private static ElasticNodeState[] m_notStableFlexNodes = new ElasticNodeState[4]
    {
      ElasticNodeState.AssignedPendingDelete,
      ElasticNodeState.FailedToStartPendingDelete,
      ElasticNodeState.FailedToRestartPendingDelete,
      ElasticNodeState.FailedVMPendingDelete
    };
    private static ElasticNodeState[] m_notStableUniformNodes = new ElasticNodeState[6]
    {
      ElasticNodeState.DeletingCompute,
      ElasticNodeState.ReimagingCompute,
      ElasticNodeState.CreatingCompute,
      ElasticNodeState.StartingAgent,
      ElasticNodeState.RestartingAgent,
      ElasticNodeState.AssignedPendingDelete
    };

    public ElasticNode() => this.StateChangedOn = DateTime.UtcNow;

    private ElasticNode(ElasticNode nodeToBeCloned)
    {
      this.PoolId = nodeToBeCloned.PoolId;
      this.Id = nodeToBeCloned.Id;
      this.Name = nodeToBeCloned.Name;
      this.State = nodeToBeCloned.State;
      this.StateChangedOn = nodeToBeCloned.StateChangedOn;
      this.DesiredState = nodeToBeCloned.DesiredState;
      this.AgentId = nodeToBeCloned.AgentId;
      this.AgentState = nodeToBeCloned.AgentState;
      this.ComputeId = nodeToBeCloned.ComputeId;
      this.ComputeState = nodeToBeCloned.ComputeState;
      this.RequestId = nodeToBeCloned.RequestId;
      this.HasNewRequest = nodeToBeCloned.HasNewRequest;
    }

    public void MergeChanges(NullableElasticNode nen)
    {
      this.PoolId = nen.PoolId;
      this.Id = nen.Id;
      this.Name = nen.Name ?? this.Name;
      ElasticNodeState? nullable = nen.State;
      this.State = (ElasticNodeState) ((int) nullable ?? (int) this.State);
      this.StateChangedOn = nen.StateChangedOn ?? this.StateChangedOn;
      nullable = nen.DesiredState;
      this.DesiredState = (ElasticNodeState) ((int) nullable ?? (int) this.DesiredState);
      this.AgentId = nen.AgentId ?? this.AgentId;
      this.AgentState = (ElasticAgentState) ((int) nen.AgentState ?? (int) this.AgentState);
      this.ComputeId = nen.ComputeId ?? this.ComputeId;
      this.ComputeState = (ElasticComputeState) ((int) nen.ComputeState ?? (int) this.ComputeState);
      this.RequestId = nen.RequestId ?? this.RequestId;
    }

    public bool IsUniformNodeStable() => !((IEnumerable<ElasticNodeState>) ElasticNode.m_notStableUniformNodes).Contains<ElasticNodeState>(this.State);

    public bool IsFlexibleNodeStable() => !((IEnumerable<ElasticNodeState>) ElasticNode.m_notStableFlexNodes).Contains<ElasticNodeState>(this.State);

    public bool WasSizeUpSuccessful() => this.State == ElasticNodeState.Idle || this.State == ElasticNodeState.Assigned || this.State == ElasticNodeState.Offline || this.State == ElasticNodeState.PendingReimage || this.State == ElasticNodeState.Saved;

    public override string ToString() => string.Format("Node {0} {1} {2}, {3} {4}, {5} {6}", (object) this.Id, (object) this.Name, (object) this.State, (object) this.ComputeId, (object) this.ComputeState, (object) this.AgentId, (object) this.AgentState);

    [DataMember]
    public int PoolId { get; set; }

    [DataMember]
    public int Id { get; set; }

    [DataMember]
    public string Name { get; set; }

    [DataMember]
    public ElasticNodeState State { get; set; }

    [DataMember]
    public DateTime StateChangedOn { get; set; }

    [DataMember]
    public ElasticNodeState DesiredState { get; set; }

    [DataMember]
    public int? AgentId { get; set; }

    [DataMember]
    public ElasticAgentState AgentState { get; set; }

    [DataMember]
    public string ComputeId { get; set; }

    [DataMember]
    public ElasticComputeState ComputeState { get; set; }

    [DataMember]
    public long? RequestId { get; set; }

    public bool ComputeExists { get; set; }

    public bool AgentExists { get; set; }

    public bool IsNew { get; set; }

    public bool HasNewRequest { get; set; }

    public bool HasStateChanged { get; set; }

    public bool WasIdleResetted { get; set; }

    public string ComputeChangeLog { get; set; }

    public string AgentChangeLog { get; set; }

    public TaskResult LastAgentTaskResult { get; set; }

    public bool? NeedUpgrade { get; set; }

    public long? LastAgentRequestId { get; set; }

    public ElasticNode Clone() => new ElasticNode(this);
  }
}
