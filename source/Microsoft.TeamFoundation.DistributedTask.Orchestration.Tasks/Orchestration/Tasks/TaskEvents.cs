// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks.TaskEvents
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: ACF674F2-B05D-403A-A061-F4792BD3317C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks.dll

namespace Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks
{
  internal static class TaskEvents
  {
    public const string LocalExecutionCompleted = "LocalExecutionCompleted";
    public const string LocalCancellationCompleted = "LocalCancellationCompleted";
    public const string TaskCanceled = "TaskCanceled";
    public const string TaskAssigned = "TaskAssigned";
    public const string TaskStarted = "TaskStarted";
    public const string TaskCompleted = "TaskCompleted";
    public const string FetchedExternalVariables = "FetchedExternalVariables";
  }
}
