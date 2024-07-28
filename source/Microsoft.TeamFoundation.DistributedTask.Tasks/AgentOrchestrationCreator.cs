// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Tasks.AgentOrchestrationCreator
// Assembly: Microsoft.TeamFoundation.DistributedTask.Tasks, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 691D8169-F87B-47FC-8906-5680483E9D38
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Tasks.dll

using Microsoft.VisualStudio.Services.Orchestration;
using System;

namespace Microsoft.TeamFoundation.DistributedTask.Tasks
{
  public sealed class AgentOrchestrationCreator : ObjectCreator<TaskOrchestration>
  {
    public AgentOrchestrationCreator(
      string name,
      string version,
      Func<TaskOrchestration> createOrchestration)
    {
      this.Name = name;
      this.Version = version;
      this.CreateOrchestration = createOrchestration;
    }

    public Func<TaskOrchestration> CreateOrchestration { get; private set; }

    public override TaskOrchestration Create() => this.CreateOrchestration();
  }
}
