// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.Tasks.DeploymentMachineState
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: ACF674F2-B05D-403A-A061-F4792BD3317C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using System;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines.Tasks
{
  internal class DeploymentMachineState
  {
    public int Id { get; set; }

    public string Name { get; set; }

    public bool DemandsMatched { get; set; }

    public TaskAgentStatus AgentStatus { get; set; }

    public bool Excluded { get; set; }

    public bool DeploymentInQueue { get; set; }

    public bool DeploymentAttempted { get; set; }

    public Guid? JobId { get; set; }

    public TaskResult? DeploymentResult { get; set; }
  }
}
