// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks.PlanEvents
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: ACF674F2-B05D-403A-A061-F4792BD3317C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks.dll

namespace Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks
{
  public static class PlanEvents
  {
    public const string Paused = "Paused";
    public const string Resumed = "Resumed";
    public const string Canceled = "Canceled";
    public const string Completed = "Completed";
    public const string JobAssigned = "JobAssigned";
    public const string JobQueued = "JobQueued";
    public const string PipelineEvent = "PipelineEvent";
    public const string CheckpointEvent = "CheckpointEvent";
    public const string Retry = "Retry";
  }
}
