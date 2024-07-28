// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.WebApi.NullableElasticNode
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using System;

namespace Microsoft.TeamFoundation.DistributedTask.WebApi
{
  public class NullableElasticNode
  {
    public NullableElasticNode()
    {
    }

    public NullableElasticNode(int poolId, int id)
    {
      this.PoolId = poolId;
      this.Id = id;
    }

    public NullableElasticNode(ElasticNode en)
    {
      this.PoolId = en.PoolId;
      this.Id = en.Id;
      this.Name = en.Name;
      this.State = new ElasticNodeState?(en.State);
      this.StateChangedOn = new DateTime?(en.StateChangedOn);
      this.DesiredState = new ElasticNodeState?(en.DesiredState);
      this.AgentId = en.AgentId;
      this.AgentState = new ElasticAgentState?(en.AgentState);
      this.ComputeId = en.ComputeId;
      this.ComputeState = new ElasticComputeState?(en.ComputeState);
      this.RequestId = en.RequestId;
    }

    public NullableElasticNode(int poolId, int id, ElasticNodeSettings ens)
    {
      this.PoolId = poolId;
      this.Id = id;
      this.State = new ElasticNodeState?(ens.State);
    }

    public int PoolId { get; set; }

    public int Id { get; set; }

    public string Name { get; set; }

    public ElasticNodeState? State { get; set; }

    public DateTime? StateChangedOn { get; set; }

    public ElasticNodeState? DesiredState { get; set; }

    public int? AgentId { get; set; }

    public ElasticAgentState? AgentState { get; set; }

    public string ComputeId { get; set; }

    public ElasticComputeState? ComputeState { get; set; }

    public long? RequestId { get; set; }
  }
}
