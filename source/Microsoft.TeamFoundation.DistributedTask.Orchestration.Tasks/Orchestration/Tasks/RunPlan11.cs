// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks.RunPlan11
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: ACF674F2-B05D-403A-A061-F4792BD3317C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.VisualStudio.Services.Orchestration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks
{
  internal sealed class RunPlan11 : RunPlanBase<RunPlanInput3>
  {
    private bool m_hasFailedWhilePaused;
    private TaskCompletionSource<RunState> m_resumeHandle;
    private readonly TaskCompletionSource<CanceledEvent> m_cancellationSource;

    public RunPlan11()
    {
      this.m_resumeHandle = new TaskCompletionSource<RunState>();
      this.m_cancellationSource = new TaskCompletionSource<CanceledEvent>();
    }

    protected override TaskCompletionSource<CanceledEvent> CancellationSource => this.m_cancellationSource;

    protected override string RunAgentJobVersion => "7.0";

    protected override string RunServerJobVersion => "5.0";

    public override void OnEvent(OrchestrationContext context, string name, object input)
    {
      switch (name)
      {
        case "Paused":
          if (this.RunState == RunState.InProgress)
          {
            this.RunState = RunState.Paused;
            break;
          }
          if (this.RunState != RunState.Failed)
            break;
          this.m_hasFailedWhilePaused = true;
          this.RunState = RunState.Paused;
          break;
        case "Resumed":
          if (this.RunState != RunState.Paused)
            break;
          this.RunState = this.m_hasFailedWhilePaused ? RunState.Failed : RunState.InProgress;
          this.m_resumeHandle.TrySetResult(this.RunState);
          break;
        case "Canceled":
          this.RunState = RunState.Canceled;
          this.m_resumeHandle.TrySetResult(this.RunState);
          this.CancellationSource.TrySetResult((CanceledEvent) input);
          break;
      }
    }

    public override async Task<TaskResult> RunTask(
      OrchestrationContext context,
      RunPlanInput3 input)
    {
      RunPlan11 runPlan11 = this;
      runPlan11.EnsureExtensions(context);
      context.TracePlanStarted(input.ScopeId, input.PlanId);
      string resultCode = string.Empty;
      TaskResult result = TaskResult.Failed;
      try
      {
        await runPlan11.ExecuteAsync(context, input, (Func<Task>) (() => this.PlanTracker.PlanStarted(input.ScopeId, input.PlanId, context.CurrentUtcDateTime)));
        try
        {
          Tuple<TaskResult, string> tuple = await runPlan11.ExecuteAsync(context, input, input.Implementation);
          result = tuple.Item1;
          resultCode = tuple.Item2;
        }
        catch (TaskFailedException ex)
        {
          context.TracePlanException(input.ScopeId, input.PlanId, (Exception) ex);
          result = TaskResult.Failed;
          resultCode = ex.GetType().Name;
        }
      }
      finally
      {
        await runPlan11.ExecuteAsync(context, input, (Func<Task>) (() => this.PlanTracker.PlanCompleted(input.ScopeId, input.PlanId, context.CurrentUtcDateTime, result, resultCode)));
        context.TracePlanCompleted(input.ScopeId, input.PlanId, result);
      }
      return result;
    }

    private static TaskResult Merge(TaskResult result, TaskResult jobResult)
    {
      if (result == TaskResult.Canceled || result == TaskResult.Failed)
        return result;
      switch (jobResult)
      {
        case TaskResult.SucceededWithIssues:
          if (result == TaskResult.Succeeded)
          {
            result = TaskResult.SucceededWithIssues;
            break;
          }
          break;
        case TaskResult.Failed:
          result = TaskResult.Failed;
          break;
        case TaskResult.Canceled:
          result = TaskResult.Canceled;
          break;
        case TaskResult.Abandoned:
          result = TaskResult.Abandoned;
          break;
      }
      return result;
    }

    private void EnsureExtensions(OrchestrationContext context)
    {
      this.PlanTracker = context.CreateClient<IPlanTrackingExtension3>(true);
      this.JobController = context.CreateClient<IJobControlExtension3>(true);
    }

    private async Task<Tuple<TaskResult, string>> ExecuteAsync(
      OrchestrationContext context,
      RunPlanInput3 input,
      TaskOrchestrationContainer item)
    {
      RunPlan11 runPlan11 = this;
      TaskResult taskResult = await runPlan11.ExecuteContainerAsync(context, input, item);
      if (taskResult != TaskResult.Failed || item.Rollback == null)
        return new Tuple<TaskResult, string>(taskResult, (string) null);
      if (runPlan11.RunState == RunState.Failed)
        runPlan11.SetRunInProgressForRollback();
      runPlan11.m_hasFailedWhilePaused = false;
      return new Tuple<TaskResult, string>(await runPlan11.ExecuteItemAsync(context, input, (TaskOrchestrationItem) item.Rollback, item.ContinueOnError), "Rollback");
    }

    private async Task<TaskResult> ExecuteContainerAsync(
      OrchestrationContext context,
      RunPlanInput3 input,
      TaskOrchestrationContainer item)
    {
      RunPlan11 runPlan11 = this;
      TaskResult containerResult = TaskResult.Succeeded;
      List<Task<TaskResult>> currentlyRunningTasks = new List<Task<TaskResult>>();
      int count = item.Parallel ? Math.Max(1, item.MaxConcurrency) : 1;
      Queue<TaskOrchestrationItem> pendingExecution = new Queue<TaskOrchestrationItem>(item.Children.Skip<TaskOrchestrationItem>(count));
      foreach (TaskOrchestrationItem orchestrationItem in item.Children.Take<TaskOrchestrationItem>(count))
        currentlyRunningTasks.Add(runPlan11.ExecuteItemAsync(context, input, orchestrationItem, item.ContinueOnError));
      while (currentlyRunningTasks.Count > 0)
      {
        int completedIndex = -1;
        TaskResult jobResult;
        try
        {
          Task<TaskResult> task = await Task.WhenAny<TaskResult>((IEnumerable<Task<TaskResult>>) currentlyRunningTasks);
          completedIndex = currentlyRunningTasks.IndexOf(task);
          currentlyRunningTasks.RemoveAt(completedIndex);
          jobResult = task.Result;
        }
        catch (TaskFailedException ex)
        {
          jobResult = TaskResult.Failed;
        }
        containerResult = RunPlan11.Merge(containerResult, jobResult);
        if (containerResult == TaskResult.Failed && !item.ContinueOnError)
        {
          runPlan11.TryCancelSource(item.Children[completedIndex]);
          pendingExecution.Clear();
        }
        if (runPlan11.RunState != RunState.Canceled && pendingExecution.Count > 0)
          currentlyRunningTasks.Add(runPlan11.ExecuteItemAsync(context, input, pendingExecution.Dequeue(), item.ContinueOnError));
      }
      TaskResult taskResult = runPlan11.RunState != RunState.Canceled ? containerResult : TaskResult.Canceled;
      currentlyRunningTasks = (List<Task<TaskResult>>) null;
      pendingExecution = (Queue<TaskOrchestrationItem>) null;
      return taskResult;
    }

    protected override async Task<TaskResult> ExecuteJobAsync(
      OrchestrationContext context,
      RunPlanInput3 input,
      TaskOrchestrationJob job,
      bool continueOnError)
    {
      int num = (int) await this.EnsureRunIsNotPaused();
      return await base.ExecuteJobAsync(context, input, job, continueOnError);
    }

    protected override void OnJobComplete(
      OrchestrationContext context,
      bool jobExecuted,
      TaskResult result)
    {
      if (!jobExecuted || result != TaskResult.Failed)
        return;
      this.SetRunAsFailed(context);
    }

    private Task<TaskResult> ExecuteItemAsync(
      OrchestrationContext context,
      RunPlanInput3 input,
      TaskOrchestrationItem item,
      bool continueOnError)
    {
      switch (item.ItemType)
      {
        case TaskOrchestrationItemType.Container:
          return this.ExecuteContainerAsync(context, input, (TaskOrchestrationContainer) item);
        case TaskOrchestrationItemType.Job:
          return this.ExecuteJobAsync(context, input, (TaskOrchestrationJob) item, continueOnError);
        default:
          throw new InvalidOperationException(string.Format("Do not know how to handle item type: {0}", (object) item.ItemType));
      }
    }

    private async Task<RunState> EnsureRunIsNotPaused()
    {
      RunPlan11 runPlan11 = this;
      RunState runState = runPlan11.RunState;
      if (runState == RunState.Paused)
        runState = await runPlan11.WaitForUnpause();
      return runState;
    }

    private async Task<RunState> WaitForUnpause()
    {
      RunPlan11 runPlan11 = this;
      if (runPlan11.RunState != RunState.Paused)
        throw new InvalidOperationException("RunPlan is not paused");
      int task = (int) await runPlan11.m_resumeHandle.Task;
      runPlan11.m_resumeHandle = new TaskCompletionSource<RunState>();
      return (RunState) task;
    }

    private void SetRunAsFailed(OrchestrationContext context)
    {
      switch (this.RunState)
      {
        case RunState.InProgress:
          this.RunState = RunState.Failed;
          break;
        case RunState.Paused:
          this.m_hasFailedWhilePaused = true;
          break;
      }
    }

    private void SetRunInProgressForRollback() => this.RunState = RunState.InProgress;

    private void TryCancelSource(TaskOrchestrationItem failedItem)
    {
      string str = (string) null;
      if (failedItem is TaskOrchestrationJob orchestrationJob)
        str = Resources.JobCancelFailFast((object) orchestrationJob.Name);
      this.m_cancellationSource.TrySetResult(new CanceledEvent()
      {
        Reason = str,
        Timeout = TimeSpan.FromSeconds(60.0)
      });
    }
  }
}
