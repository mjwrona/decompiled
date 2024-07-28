// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks.IServerJobTrackingExtension2
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: ACF674F2-B05D-403A-A061-F4792BD3317C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using System;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks
{
  public interface IServerJobTrackingExtension2
  {
    Task JobStarted(Guid scopeId, Guid planId, Guid jobId, DateTime timestamp);

    Task JobCompleted(
      Guid scopeId,
      Guid planId,
      Guid jobId,
      DateTime timestamp,
      TaskResult jobResult);

    Task TaskStarted(Guid scopeId, Guid planId, Guid jobId, Guid taskId, DateTime timestamp);

    Task TaskCompleted(
      Guid scopeId,
      Guid planId,
      Guid jobId,
      Guid taskId,
      DateTime timestamp,
      TaskResult result);

    Task LogIssue(
      Guid scopeId,
      Guid planId,
      Guid jobId,
      DateTime timestamp,
      IssueType issueType,
      string message);
  }
}
