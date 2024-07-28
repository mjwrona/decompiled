// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks.IServerExecutionControlExtension
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: ACF674F2-B05D-403A-A061-F4792BD3317C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using System;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks
{
  public interface IServerExecutionControlExtension
  {
    Task AssignTask(Guid scopeId, Guid planId, Guid jobId, Guid taskId);

    Task CancelTask(string serverTaskInstanceId, TimeSpan timeout, string reason);

    Task CompleteTask(Guid scopeId, Guid planId, Guid jobId, Guid taskId, TaskEvent eventData);

    Task<TaskOrchestrationJob> GetJob(Guid scopeId, Guid planId, Guid jobId);

    Task NotifyTaskStarted(Guid scopeId, Guid planId, Guid jobId);

    Task StartTask(Guid scopeId, Guid planId, Guid jobId, Guid taskId);
  }
}
