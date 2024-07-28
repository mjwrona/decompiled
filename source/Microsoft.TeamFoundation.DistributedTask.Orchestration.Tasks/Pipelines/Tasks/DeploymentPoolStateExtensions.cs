// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.Tasks.DeploymentPoolStateExtensions
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: ACF674F2-B05D-403A-A061-F4792BD3317C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines.Tasks
{
  internal static class DeploymentPoolStateExtensions
  {
    public static TaskResult GetDeploymentResult(this DeploymentPoolState poolState)
    {
      if (poolState == null)
        throw new ArgumentNullException(nameof (poolState));
      TaskResult? aggregatedResult = new TaskResult?();
      foreach (DeploymentMachineState deploymentMachineState in (IEnumerable<DeploymentMachineState>) poolState.Machines.Values)
      {
        if (!deploymentMachineState.DeploymentAttempted)
          return TaskResult.Failed;
        if (deploymentMachineState.DeploymentAttempted)
        {
          TaskResult jobResult = (TaskResult) ((int) deploymentMachineState.DeploymentResult ?? 2);
          aggregatedResult = new TaskResult?(DeploymentPoolStateExtensions.MergeResult(aggregatedResult, jobResult));
        }
      }
      return aggregatedResult.GetValueOrDefault();
    }

    private static TaskResult MergeResult(TaskResult? aggregatedResult, TaskResult jobResult)
    {
      TaskResult taskResult1 = jobResult == TaskResult.Succeeded || jobResult == TaskResult.SucceededWithIssues ? jobResult : TaskResult.Failed;
      if (!aggregatedResult.HasValue)
        return taskResult1;
      TaskResult? nullable = aggregatedResult;
      TaskResult taskResult2 = TaskResult.SucceededWithIssues;
      if (nullable.GetValueOrDefault() == taskResult2 & nullable.HasValue || taskResult1 == TaskResult.SucceededWithIssues)
        return TaskResult.SucceededWithIssues;
      nullable = aggregatedResult;
      TaskResult taskResult3 = TaskResult.Succeeded;
      if (nullable.GetValueOrDefault() == taskResult3 & nullable.HasValue && taskResult1 == TaskResult.Succeeded)
      {
        aggregatedResult = new TaskResult?(TaskResult.Succeeded);
      }
      else
      {
        nullable = aggregatedResult;
        TaskResult taskResult4 = TaskResult.Failed;
        aggregatedResult = !(nullable.GetValueOrDefault() == taskResult4 & nullable.HasValue) || taskResult1 != TaskResult.Failed ? new TaskResult?(TaskResult.SucceededWithIssues) : new TaskResult?(TaskResult.Failed);
      }
      return aggregatedResult.Value;
    }
  }
}
