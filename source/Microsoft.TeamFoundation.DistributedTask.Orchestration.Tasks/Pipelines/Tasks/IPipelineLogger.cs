// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.Tasks.IPipelineLogger
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: ACF674F2-B05D-403A-A061-F4792BD3317C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks.dll

using Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines.Tasks
{
  public interface IPipelineLogger
  {
    Task LogIssue(
      Guid scopeId,
      Guid planId,
      Guid jobId,
      DateTime timestamp,
      IssueType issueType,
      string message);

    Task PlanStarted(Guid scopeId, Guid planId, DateTime timestamp);

    Task PlanCompleted(
      Guid scopeId,
      Guid planId,
      DateTime timestamp,
      TaskResult result,
      string resultCode);

    Task StageCompleted(Guid scopeId, Guid planId, Guid stageId, TaskResult? result);

    Task JobCompleted(Guid scopeId, Guid planId, Guid jobId, TaskResult? result);

    Task UpdateTimeline(
      Guid scopeId,
      Guid planId,
      IList<TimelineRecord> records,
      bool updateChildrenStateAndResult = false);

    Task CreateRetryTimelines(Guid scopeId, Guid planId, RetryEvent retryEvent);

    Task<IList<IGraphNode>> CreateRetryTimelinesWithAttempts(
      Guid scopeId,
      Guid planId,
      RetryEvent retryEvent);
  }
}
