// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.Tasks.RunCheckpointInput
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: ACF674F2-B05D-403A-A061-F4792BD3317C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks.dll

using System;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines.Tasks
{
  public class RunCheckpointInput
  {
    public Guid ScopeId { get; set; }

    public Guid PlanId { get; set; }

    public int ActivityDispatcherShardsCount { get; set; }

    public PipelineActivityShardKey ShardKey { get; set; }

    public int PlanVersion { get; set; }

    public string NodeInstanceName { get; set; }

    public string NodeName { get; set; }

    public int NodeAttempt { get; set; }

    public int TimeoutInMinutes { get; set; } = 44640;
  }
}
