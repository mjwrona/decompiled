// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks.TaskOrchestrationCreator
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: ACF674F2-B05D-403A-A061-F4792BD3317C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks.dll

using Microsoft.VisualStudio.Services.Orchestration;
using System;

namespace Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks
{
  public sealed class TaskOrchestrationCreator : ObjectCreator<TaskOrchestration>
  {
    public TaskOrchestrationCreator(
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
