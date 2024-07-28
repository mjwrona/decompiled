// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.Tasks.IServerJobManager
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: ACF674F2-B05D-403A-A061-F4792BD3317C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks.dll

using Microsoft.TeamFoundation.DistributedTask.Pipelines.Runtime;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines.Tasks
{
  public interface IServerJobManager
  {
    Task<IList<ServerTaskReference>> GetTasks(Guid scopeId, Guid planId, JobParameters parameters);

    Task<IList<TaskExecutionState>> Expand(Guid scopeId, Guid planId, JobParameters parameters);

    Task<ExecuteTaskResponse> ExecuteTask(Guid scopeId, Guid planId, TaskParameters parameters);

    Task<CancelTaskResponse> CancelTask(
      Guid scopeId,
      Guid planId,
      TaskParameters parameters,
      TaskCanceledReasonType reasonType);

    Task JobStarted(
      Guid scopeId,
      Guid planId,
      JobParameters parameters,
      JobStartedEventData eventData);
  }
}
