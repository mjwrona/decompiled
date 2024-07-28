// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks.RunPlan4
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: ACF674F2-B05D-403A-A061-F4792BD3317C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.VisualStudio.Services.Orchestration;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks
{
  internal sealed class RunPlan4 : TaskOrchestration<TaskResult, RunPlanInput3, object, string>
  {
    private TimeSpan m_timeout;
    private bool m_hasFailedWhilePaused;
    private TaskCompletionSource<RunState> m_resumeHandle;
    private readonly TaskCompletionSource<CanceledEvent> m_cancellationSource;

    public RunPlan4()
    {
      this.m_timeout = TimeSpan.FromSeconds(60.0);
      this.m_resumeHandle = new TaskCompletionSource<RunState>();
      this.m_cancellationSource = new TaskCompletionSource<CanceledEvent>();
    }

    public IJobControlExtension3 JobController { get; private set; }

    public IPlanTrackingExtension3 PlanTracker { get; private set; }

    public Func<Exception, bool> RetryException { get; set; }

    public TaskCompletionSource<CanceledEvent> CancellationSource => this.m_cancellationSource;

    public RunState RunState { get; set; }

    public override void OnEvent(OrchestrationContext context, string name, object input)
    {
      switch (name)
      {
        case "Paused":
          if (this.RunState == RunState.InProgress)
            this.RunState = RunState.Paused;
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
      this.EnsureExtensions(context);
      context.TracePlanStarted(input.ScopeId, input.PlanId);
      await this.ExecuteAsync(context, input, (Func<Task>) (() => this.PlanTracker.PlanStarted(input.ScopeId, input.PlanId, context.CurrentUtcDateTime)));
      TaskResult result;
      string resultCode;
      try
      {
        Tuple<TaskResult, string> tuple = await this.ExecuteAsync(context, input, input.Implementation);
        result = tuple.Item1;
        resultCode = tuple.Item2;
      }
      catch (TaskFailedException ex)
      {
        context.TracePlanException(input.ScopeId, input.PlanId, (Exception) ex);
        result = TaskResult.Failed;
        resultCode = ex.GetType().Name;
      }
      await this.ExecuteAsync(context, input, (Func<Task>) (() => this.PlanTracker.PlanCompleted(input.ScopeId, input.PlanId, context.CurrentUtcDateTime, result, resultCode)));
      context.TracePlanCompleted(input.ScopeId, input.PlanId, result);
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
      TaskResult taskResult = await this.ExecuteContainerAsync(context, input, item);
      if (taskResult != TaskResult.Failed || item.Rollback == null)
        return new Tuple<TaskResult, string>(taskResult, (string) null);
      if (this.RunState == RunState.Failed)
        this.SetRunInProgressForRollback();
      this.m_hasFailedWhilePaused = false;
      return new Tuple<TaskResult, string>(await this.ExecuteItemAsync(context, input, (TaskOrchestrationItem) item.Rollback, item.ContinueOnError), "Rollback");
    }

    private async Task<TaskResult> ExecuteContainerAsync(
      OrchestrationContext context,
      RunPlanInput3 input,
      TaskOrchestrationContainer item)
    {
      TaskResult containerResult = TaskResult.Succeeded;
      if (item.Parallel)
      {
        List<Task<TaskResult>> allTasks = new List<Task<TaskResult>>();
        foreach (TaskOrchestrationItem child in item.Children)
          allTasks.Add(this.ExecuteItemAsync(context, input, child, item.ContinueOnError));
        while (allTasks.Count > 0)
        {
          int completedIndex = -1;
          TaskResult jobResult;
          try
          {
            Task<TaskResult> task = await Task.WhenAny<TaskResult>((IEnumerable<Task<TaskResult>>) allTasks);
            completedIndex = allTasks.IndexOf(task);
            allTasks.RemoveAt(completedIndex);
            jobResult = task.Result;
          }
          catch (TaskFailedException ex)
          {
            jobResult = TaskResult.Failed;
          }
          containerResult = RunPlan4.Merge(containerResult, jobResult);
          if (containerResult == TaskResult.Failed && !item.ContinueOnError)
          {
            string str = (string) null;
            if (item.Children[completedIndex] is TaskOrchestrationJob child)
              str = Resources.JobCancelFailFast((object) child.Name);
            this.m_cancellationSource.TrySetResult(new CanceledEvent()
            {
              Reason = str,
              Timeout = TimeSpan.FromSeconds(60.0)
            });
          }
        }
        allTasks = (List<Task<TaskResult>>) null;
      }
      else
      {
        foreach (TaskOrchestrationItem child in item.Children)
        {
          if (this.RunState == RunState.Canceled)
            return TaskResult.Canceled;
          TaskResult jobResult;
          try
          {
            jobResult = await this.ExecuteItemAsync(context, input, child, item.ContinueOnError);
          }
          catch (TaskFailedException ex)
          {
            jobResult = TaskResult.Failed;
          }
          containerResult = RunPlan4.Merge(containerResult, jobResult);
          if (containerResult == TaskResult.Failed)
          {
            if (!item.ContinueOnError)
              break;
          }
        }
      }
      return containerResult;
    }

    private async Task<TaskResult> ExecuteJobAsync(
      OrchestrationContext context,
      RunPlanInput3 input,
      TaskOrchestrationJob job,
      bool continueOnError)
    {
      string errorMessage = (string) null;
      bool jobExecuted = false;
      string jobInstanceId = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0:N}_{1:N}", (object) input.PlanId, (object) job.InstanceId);
      TaskResult result;
      try
      {
        switch (await this.EnsureRunIsNotPaused())
        {
          case RunState.Canceled:
            result = TaskResult.Canceled;
            goto label_13;
          case RunState.Failed:
            if (!continueOnError)
            {
              result = TaskResult.Failed;
              goto label_13;
            }
            else
              break;
        }
        jobExecuted = true;
        RunJobInput3 input1 = new RunJobInput3()
        {
          PoolId = input.PoolId,
          ScopeId = input.ScopeId,
          PlanId = input.PlanId,
          Job = job
        };
        context.TraceJobScheduled(input.ScopeId, input.PlanId, job.InstanceId);
        Task<TaskResult> jobTask = context.CreateSubOrchestrationInstance<TaskResult>("RunAgentJob", "4.0", jobInstanceId, (object) input1);
        if (await Task.WhenAny((Task) this.m_cancellationSource.Task, (Task) jobTask) == this.m_cancellationSource.Task)
        {
          CanceledEvent canceledEvent = this.m_cancellationSource.Task.Result;
          await context.ExecuteAsync((Func<Task>) (() => this.JobController.CancelJob(jobInstanceId, canceledEvent.Timeout, canceledEvent.Reason)), canRetry: this.RetryException, traceException: (Action<Exception>) (ex => context.TracePlanException(input.ScopeId, input.PlanId, ex)));
        }
        return await jobTask;
      }
      catch (SubOrchestrationFailedException ex)
      {
        context.TracePlanException(input.ScopeId, input.PlanId, (Exception) ex);
        errorMessage = ex.InnerException == null ? ex.Message : ex.InnerException.Message;
        result = TaskResult.Failed;
      }
label_13:
      context.TraceJobCompleted(input.ScopeId, input.PlanId, job.InstanceId, result);
      if (!string.IsNullOrEmpty(errorMessage))
      {
        try
        {
          await this.ExecuteAsync(context, input, (Func<Task>) (() => this.PlanTracker.LogIssue(input.ScopeId, input.PlanId, job.InstanceId, context.CurrentUtcDateTime, IssueType.Error, errorMessage)));
        }
        catch (TaskFailedException ex)
        {
        }
      }
      if (jobExecuted && result == TaskResult.Failed)
        this.SetRunAsFailed(context);
      return result;
    }

    private Task ExecuteAsync(
      OrchestrationContext context,
      RunPlanInput3 input,
      Func<Task> operation)
    {
      return context.ExecuteAsync(operation, canRetry: this.RetryException, traceException: (Action<Exception>) (ex => context.TracePlanException(input.ScopeId, input.PlanId, ex)));
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
          throw new InvalidOperationException("Do not know how to handle item type: " + item.ItemType.ToString());
      }
    }

    private async Task<RunState> EnsureRunIsNotPaused()
    {
      RunState runState = this.RunState;
      if (runState == RunState.Paused)
        runState = await this.WaitForUnpause();
      return runState;
    }

    private async Task<RunState> WaitForUnpause()
    {
      if (this.RunState != RunState.Paused)
        throw new InvalidOperationException("RunPlan is not paused");
      int task = (int) await this.m_resumeHandle.Task;
      this.m_resumeHandle = new TaskCompletionSource<RunState>();
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
  }
}
