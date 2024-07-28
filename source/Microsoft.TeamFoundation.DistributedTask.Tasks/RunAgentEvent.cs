// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Tasks.RunAgentEvent
// Assembly: Microsoft.TeamFoundation.DistributedTask.Tasks, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 691D8169-F87B-47FC-8906-5680483E9D38
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Tasks.dll

using System;

namespace Microsoft.TeamFoundation.DistributedTask.Tasks
{
  public abstract class RunAgentEvent
  {
    public RunAgentEvent()
    {
    }

    public RunAgentEvent(int agentCloudId, Guid agentCloudRequestId, int poolId, int agentId)
    {
      this.AgentCloudId = agentCloudId;
      this.AgentCloudRequestId = agentCloudRequestId;
      this.PoolId = poolId;
      this.AgentId = agentId;
    }

    public abstract string EventType { get; }

    public int AgentCloudId { get; set; }

    public Guid AgentCloudRequestId { get; set; }

    public int PoolId { get; set; }

    public int AgentId { get; set; }

    public abstract object GetEventData();
  }
}
