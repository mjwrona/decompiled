// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Tasks.ProvisionAgentEvent
// Assembly: Microsoft.TeamFoundation.DistributedTask.Tasks, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 691D8169-F87B-47FC-8906-5680483E9D38
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Tasks.dll

using System;

namespace Microsoft.TeamFoundation.DistributedTask.Tasks
{
  public sealed class ProvisionAgentEvent : RunAgentEvent
  {
    public ProvisionAgentEvent() => this.AgentCloudRequestId = Guid.NewGuid();

    public override string EventType => "ProvisionAgent";

    public override object GetEventData() => (object) null;
  }
}
