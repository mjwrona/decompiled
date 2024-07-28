// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks.RunPlan
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: ACF674F2-B05D-403A-A061-F4792BD3317C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.VisualStudio.Services.Orchestration;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks
{
  internal class RunPlan : TaskOrchestration<TaskResult, RunPlanInput, object, string>
  {
    protected TimeSpan m_timeout;
    protected TaskCompletionSource<RunState> m_resumeHandle;
    protected bool m_hasFailedWhilePaused;

    public IJobControlExtension JobController { get; private set; }

    public IPlanTrackingExtension PlanTracker { get; private set; }

    public Func<Exception, bool> RetryException { get; set; }

    public TaskCompletionSource<object> JobCancellationSource { get; set; }

    public RunState RunState { get; set; }

    public RunPlan() => this.m_resumeHandle = new TaskCompletionSource<RunState>();

    public override async Task<TaskResult> RunTask(OrchestrationContext context, RunPlanInput input)
    {
      this.EnsureExtensions(context);
      context.TracePlanStarted(input.HostId, input.PlanId);
      await this.ExecuteAsync(context, input, (Func<Task>) (() => this.PlanTracker.PlanStarted(input.HostId, input.PlanId, context.CurrentUtcDateTime)));
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
        context.TracePlanException(input.HostId, input.PlanId, (Exception) ex);
        result = TaskResult.Failed;
        resultCode = ex.GetType().Name;
      }
      await this.ExecuteAsync(context, input, (Func<Task>) (() => this.PlanTracker.PlanCompleted(input.HostId, input.PlanId, context.CurrentUtcDateTime, result, resultCode)));
      context.TracePlanCompleted(input.HostId, input.PlanId, result);
      return result;
    }

    public override void OnEvent(OrchestrationContext context, string name, object input)
    {
      if (name.Equals("Canceled"))
      {
        TimeSpan result;
        this.m_timeout = TimeSpan.TryParse(input.ToString(), out result) ? result : TimeSpan.FromSeconds(60.0);
      }
      this.HandleEvent(context, name);
    }

    protected static TaskResult Merge(TaskResult result, TaskResult jobResult)
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
      this.PlanTracker = context.CreateClient<IPlanTrackingExtension>(true);
      this.JobController = context.CreateClient<IJobControlExtension>(true);
      this.JobCancellationSource = new TaskCompletionSource<object>();
    }

    protected async Task<Tuple<TaskResult, string>> ExecuteAsync(
      OrchestrationContext context,
      RunPlanInput input,
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

    protected virtual async Task<TaskResult> ExecuteContainerAsync(
      OrchestrationContext context,
      RunPlanInput input,
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
          TaskResult jobResult;
          try
          {
            Task<TaskResult> task = await Task.WhenAny<TaskResult>((IEnumerable<Task<TaskResult>>) allTasks);
            allTasks.Remove(task);
            jobResult = task.Result;
          }
          catch (TaskFailedException ex)
          {
            jobResult = TaskResult.Failed;
          }
          containerResult = RunPlan.Merge(containerResult, jobResult);
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
          containerResult = RunPlan.Merge(containerResult, jobResult);
          if (!item.ContinueOnError)
          {
            if (containerResult == TaskResult.Failed)
              break;
          }
        }
      }
      return containerResult;
    }

    protected async Task<TaskResult> ExecuteJobAsync(
      OrchestrationContext context,
      RunPlanInput input,
      TaskOrchestrationJob job,
      bool continueOnError)
    {
      string errorMessage = (string) null;
      string jobInstanceId = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0:N}_{1:N}", (object) input.PlanId, (object) job.InstanceId);
      bool jobExecuted = false;
      TaskResult result;
      try
      {
        switch (await this.EnsureRunIsNotPaused())
        {
          case RunState.Canceled:
            result = TaskResult.Canceled;
            goto label_9;
          case RunState.Failed:
            if (!continueOnError)
            {
              result = TaskResult.Failed;
              goto label_9;
            }
            else
              break;
        }
        jobExecuted = true;
        result = await this.ExecuteJob(context, input, job, jobInstanceId);
      }
      catch (SubOrchestrationFailedException ex)
      {
        context.TracePlanException(input.HostId, input.PlanId, (Exception) ex);
        errorMessage = ex.InnerException == null ? ex.Message : ex.InnerException.Message;
        result = TaskResult.Failed;
      }
label_9:
      context.TraceJobCompleted(input.HostId, input.PlanId, job.InstanceId, result);
      if (!string.IsNullOrEmpty(errorMessage))
      {
        try
        {
          await this.ExecuteAsync(context, input, (Func<Task>) (() => this.PlanTracker.LogMessage(input.HostId, input.PlanId, job.InstanceId, context.CurrentUtcDateTime, TraceLevel.Error, errorMessage)));
        }
        catch (TaskFailedException ex)
        {
        }
      }
      if (jobExecuted && result == TaskResult.Failed)
        this.SetRunAsFailed(context);
      TaskResult taskResult = result;
      jobInstanceId = (string) null;
      return taskResult;
    }

    private Task ExecuteAsync(
      OrchestrationContext context,
      RunPlanInput input,
      Func<Task> operation)
    {
      return context.ExecuteAsync(operation, canRetry: this.RetryException, traceException: (Action<Exception>) (ex => context.TracePlanException(input.HostId, input.PlanId, ex)));
    }

    protected Task<TaskResult> ExecuteItemAsync(
      OrchestrationContext context,
      RunPlanInput input,
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

    protected virtual async Task<TaskResult> ExecuteJob(
      OrchestrationContext context,
      RunPlanInput input,
      TaskOrchestrationJob job,
      string jobInstanceId)
    {
      RunJobInput input1 = new RunJobInput()
      {
        PoolId = input.PoolId,
        HostId = input.HostId,
        PlanId = input.PlanId,
        Environment = input.Environment,
        Job = job
      };
      context.TraceJobScheduled(input.HostId, input.PlanId, job.InstanceId);
      Task<TaskResult> jobTask = context.CreateSubOrchestrationInstance<TaskResult>("RunAgentJob", "2.0", jobInstanceId, (object) input1);
      if (await Task.WhenAny((Task) this.JobCancellationSource.Task, (Task) jobTask) == this.JobCancellationSource.Task)
        await context.ExecuteAsync((Func<Task>) (() => this.JobController.CancelJob(input.PlanId, jobInstanceId, this.m_timeout)), canRetry: this.RetryException, traceException: (Action<Exception>) (ex => context.TracePlanException(input.HostId, input.PlanId, ex)));
      TaskResult taskResult = await jobTask;
      jobTask = (Task<TaskResult>) null;
      return taskResult;
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

    private void HandleEvent(OrchestrationContext context, string eventName)
    {
      switch (eventName)
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
          this.JobCancellationSource.TrySetResult((object) null);
          break;
      }
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
