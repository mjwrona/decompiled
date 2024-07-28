// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Tasks.DeprovisionEvent
// Assembly: Microsoft.TeamFoundation.DistributedTask.Tasks, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 691D8169-F87B-47FC-8906-5680483E9D38
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Tasks.dll

using System;

namespace Microsoft.TeamFoundation.DistributedTask.Tasks
{
  public sealed class DeprovisionEvent : RunAgentEvent
  {
    public DeprovisionEvent()
    {
    }

    public DeprovisionEvent(
      int agentCloudId,
      Guid agentCloudRequestId,
      int poolId,
      int agentId,
      string reason)
      : base(agentCloudId, agentCloudRequestId, poolId, agentId)
    {
      this.Reason = reason;
    }

    public override string EventType => "Deprovision";

    public string Reason { get; set; }

    public override object GetEventData() => (object) this.Reason;
  }
}
