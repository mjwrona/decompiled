// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks.RunServerJob5
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: ACF674F2-B05D-403A-A061-F4792BD3317C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.VisualStudio.Services.Orchestration;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks
{
  internal sealed class RunServerJob5 : 
    TaskOrchestration<JobResult, RunServerJobInput, object, string>
  {
    private RunServerJobInput m_input;
    private bool m_hasJobStarted;
    private bool m_hasJobAssigned;
    private bool m_hasJobCompleted;
    private bool m_hasOnlyOneTask;
    private bool m_hasReceivedTaskStartedNotification;
    private TaskCompletionSource<TaskResult> m_jobCompleted = new TaskCompletionSource<TaskResult>();
    private TaskCompletionSource<CanceledEvent> m_jobCanceled = new TaskCompletionSource<CanceledEvent>();
    private TaskCompletionSource<bool> m_jobStarted = new TaskCompletionSource<bool>();
    private TaskCompletionSource<bool> m_jobAssigned = new TaskCompletionSource<bool>();

    public IServerJobTrackingExtension3 Tracker { get; private set; }

    public IServerTaskTrackingExtension ServerTaskTracker { get; private set; }

    public IServerExecutionControlExtension ExecutionControl { get; private set; }

    public Func<Exception, bool> RetryException { get; set; }

    public override void OnEvent(OrchestrationContext context, string name, object input)
    {
      switch (name)
      {
        case "JobAssigned":
          this.m_jobAssigned.TrySetResult(true);
          break;
        case "JobCanceled":
          this.m_jobCanceled.TrySetResult((CanceledEvent) input);
          break;
        case "JobCompleted":
          if (this.m_hasOnlyOneTask)
          {
            this.m_jobCompleted.TrySetResult(((JobCompletedEvent) input).Result);
            break;
          }
          context.TraceJobError(10015551, this.m_input.ScopeId, this.m_input.PlanId, this.m_input.Job.InstanceId, "RunServerJob5::OnEvent -- Unexpected JobCompleted event when more than one task exist.");
          break;
        case "JobStarted":
          this.m_jobStarted.TrySetResult(true);
          break;
        case "TaskStarted":
          this.m_hasReceivedTaskStartedNotification = true;
          this.m_jobStarted.TrySetResult(true);
          break;
        default:
          context.TraceJobError(10015551, this.m_input.ScopeId, this.m_input.PlanId, this.m_input.Job.InstanceId, "RunServerJob5::OnEvent -- Unexpected eventName: " + name);
          break;
      }
    }

    public override async Task<JobResult> RunTask(
      OrchestrationContext context,
      RunServerJobInput input)
    {
      this.EnsureExtensions(context);
      this.m_input = input;
      TaskOrchestrationJob job = await this.ExecuteAsync<TaskOrchestrationJob>(context, this.m_input, (Func<Task<TaskOrchestrationJob>>) (() => this.ExecutionControl.GetJob(this.m_input.ScopeId, this.m_input.PlanId, this.m_input.Job.InstanceId)));
      JobResult jobResult = new JobResult()
      {
        Result = TaskResult.Succeeded
      };
      this.m_hasOnlyOneTask = job.Tasks != null && job.Tasks.Count == 1;
      CancellationTokenSource jobTimeoutCancellationTokenSource = new CancellationTokenSource();
      try
      {
        Task<string> jobTimeoutTask = (Task<string>) null;
        TimeSpan jobTimeoutValue = TimeSpan.Zero;
        TimeSpan? executionTimeout = this.m_input.Job.ExecutionTimeout;
        ref TimeSpan? local = ref executionTimeout;
        if ((local.HasValue ? (local.GetValueOrDefault().TotalMinutes > 0.0 ? 1 : 0) : 0) != 0)
        {
          executionTimeout = this.m_input.Job.ExecutionTimeout;
          jobTimeoutValue = executionTimeout.Value;
          if (jobTimeoutValue > TimeSpan.Zero && jobTimeoutValue < TimeSpan.FromMilliseconds((double) int.MaxValue))
            jobTimeoutTask = context.CreateTimer<string>(context.CurrentUtcDateTime.Add(jobTimeoutValue), (string) null, jobTimeoutCancellationTokenSource.Token);
        }
        int taskPosition = 0;
        foreach (TaskInstance task1 in job.Tasks)
        {
          TaskInstance task = task1;
          ++taskPosition;
          if (task == null)
            context.TraceJobError(10015551, this.m_input.ScopeId, this.m_input.PlanId, this.m_input.Job.InstanceId, string.Format("RunServerJob5::RunTask -- Task is null at position: {0}", (object) taskPosition));
          else if (this.CanSkipExecution(context, task, jobResult.Result))
          {
            if (!this.m_hasJobStarted)
              await this.PerformStartJob(context);
            JobResult jobResult1 = await this.RecordSkippedTask(context, task);
          }
          else
          {
            JobResult taskResult = new JobResult()
            {
              Result = TaskResult.Succeeded
            };
            RunServerTaskInput serverTaskInput = this.GetServerTaskInput(task);
            string orchestrationId = ServerTaskExtensions.GetServerTaskOrchestrationId(this.m_input.PlanId, this.m_input.Job.InstanceId, task.InstanceId);
            Task<JobResult> subOrchestrationTask = RunServerJob5.CreateSubOrchestrationInstance(context, serverTaskInput, orchestrationId);
            while (true)
            {
              Task task2;
              do
              {
                List<Task> taskList = new List<Task>()
                {
                  (Task) subOrchestrationTask,
                  (Task) this.m_jobCanceled.Task
                };
                if (jobTimeoutTask != null)
                  taskList.Add((Task) jobTimeoutTask);
                if (!this.m_hasJobStarted)
                  taskList.Add((Task) this.m_jobStarted.Task);
                if (this.m_hasOnlyOneTask && !this.m_hasJobAssigned)
                  taskList.Add((Task) this.m_jobAssigned.Task);
                if (this.m_hasOnlyOneTask && !this.m_hasJobCompleted)
                  taskList.Add((Task) this.m_jobCompleted.Task);
                task2 = await Task.WhenAny((IEnumerable<Task>) taskList);
                if (task2 == subOrchestrationTask)
                {
                  taskResult = subOrchestrationTask.Result;
                  goto label_46;
                }
                else if (task2 == jobTimeoutTask)
                {
                  jobResult.Message = Resources.ServerJob4TimedOut((object) jobTimeoutValue.ToString());
                  taskResult = new JobResult()
                  {
                    Result = TaskResult.Canceled
                  };
                  TaskCanceledReasonType cancelTypeReason = job.Tasks.Count == taskPosition ? TaskCanceledReasonType.Timeout : TaskCanceledReasonType.Other;
                  context.TraceServerJobCancelTask(this.m_input.ScopeId, this.m_input.PlanId, this.m_input.Job.InstanceId, task.InstanceId);
                  await this.ExecuteAsync(context, this.m_input, (Func<Task>) (() => this.ExecutionControl.CancelTask(orchestrationId, jobTimeoutValue, cancelTypeReason.ToString())));
                  JobResult jobResult2 = await subOrchestrationTask;
                  if ((jobResult.Result == TaskResult.Succeeded || jobResult.Result == TaskResult.SucceededWithIssues) && job.Tasks.Count == taskPosition)
                  {
                    taskResult.Result = jobResult2.Result;
                    if (jobResult2.Result == TaskResult.Succeeded || jobResult2.Result == TaskResult.SucceededWithIssues)
                    {
                      jobResult.Message = string.Empty;
                      goto label_46;
                    }
                    else
                      goto label_46;
                  }
                  else
                    goto label_46;
                }
                else if (task2 == this.m_jobCanceled.Task)
                {
                  jobResult.Message = this.m_jobCanceled.Task.Result.Reason;
                  taskResult = new JobResult()
                  {
                    Result = TaskResult.Canceled
                  };
                  context.TraceServerJobCancelTask(this.m_input.ScopeId, this.m_input.PlanId, this.m_input.Job.InstanceId, task.InstanceId);
                  await this.ExecuteAsync(context, this.m_input, (Func<Task>) (() => this.ExecutionControl.CancelTask(orchestrationId, jobTimeoutValue, "Other")));
                  JobResult jobResult3 = await subOrchestrationTask;
                  goto label_46;
                }
                else if (task2 == this.m_jobAssigned.Task)
                {
                  this.m_hasJobAssigned = true;
                  context.TraceServerJobAssignTask(this.m_input.ScopeId, this.m_input.PlanId, this.m_input.Job.InstanceId, task.InstanceId);
                  await this.ExecuteAsync(context, this.m_input, closure_13 ?? (closure_13 = (Func<Task>) (() => this.ExecutionControl.AssignTask(this.m_input.ScopeId, this.m_input.PlanId, this.m_input.Job.InstanceId, task.InstanceId))));
                }
                else if (task2 == this.m_jobStarted.Task)
                {
                  await this.PerformStartJob(context);
                  if (this.m_hasOnlyOneTask && !this.m_hasReceivedTaskStartedNotification)
                  {
                    context.TraceServerJobStartTask(this.m_input.ScopeId, this.m_input.PlanId, this.m_input.Job.InstanceId, task.InstanceId);
                    await this.ExecuteAsync(context, this.m_input, closure_14 ?? (closure_14 = (Func<Task>) (() => this.ExecutionControl.StartTask(this.m_input.ScopeId, this.m_input.PlanId, this.m_input.Job.InstanceId, task.InstanceId))));
                  }
                }
              }
              while (task2 != this.m_jobCompleted.Task);
              this.m_hasJobCompleted = true;
              context.TraceServerJobCompleteTask(this.m_input.ScopeId, this.m_input.PlanId, this.m_input.Job.InstanceId, task.InstanceId, this.m_jobCompleted.Task.Result);
              TaskCompletedEvent taskCompletedEventData = new TaskCompletedEvent(this.m_input.Job.InstanceId, task.InstanceId, this.m_jobCompleted.Task.Result);
              await this.ExecuteAsync(context, this.m_input, (Func<Task>) (() => this.ExecutionControl.CompleteTask(this.m_input.ScopeId, this.m_input.PlanId, this.m_input.Job.InstanceId, task.InstanceId, (TaskEvent) taskCompletedEventData)));
            }
label_46:
            jobResult = RunServerJob5.MergeResult(jobResult, taskResult);
            taskResult = (JobResult) null;
            subOrchestrationTask = (Task<JobResult>) null;
          }
        }
        jobTimeoutTask = (Task<string>) null;
      }
      finally
      {
        if (jobTimeoutCancellationTokenSource != null)
        {
          jobTimeoutCancellationTokenSource.Cancel();
          jobTimeoutCancellationTokenSource.Dispose();
          jobTimeoutCancellationTokenSource = (CancellationTokenSource) null;
        }
      }
      context.TraceServerJobCompleted(this.m_input.ScopeId, this.m_input.PlanId, this.m_input.Job.InstanceId, jobResult.Result);
      await this.ExecuteAsync(context, this.m_input, (Func<Task>) (() => this.Tracker.JobCompleted(this.m_input.ScopeId, this.m_input.PlanId, this.m_input.Job.InstanceId, context.CurrentUtcDateTime, jobResult.Result)));
      JobResult jobResult4 = jobResult;
      job = (TaskOrchestrationJob) null;
      jobTimeoutCancellationTokenSource = (CancellationTokenSource) null;
      return jobResult4;
    }

    private static JobResult MergeResult(JobResult jobResult, JobResult taskResult)
    {
      if (jobResult.Result > TaskResult.Failed || taskResult.Result < jobResult.Result)
        return jobResult;
      jobResult.Result = taskResult.Result;
      return jobResult;
    }

    private static Task<JobResult> CreateSubOrchestrationInstance(
      OrchestrationContext context,
      RunServerTaskInput input,
      string orchestrationId)
    {
      return context.CreateSubOrchestrationInstance<JobResult>("RunServerTask", "2.0", orchestrationId, (object) input);
    }

    private async Task PerformStartJob(OrchestrationContext context)
    {
      this.m_hasJobStarted = true;
      context.TraceServerJobStarted(this.m_input.ScopeId, this.m_input.PlanId, this.m_input.Job.InstanceId);
      await this.ExecuteAsync(context, this.m_input, (Func<Task>) (() => this.Tracker.JobStarted(this.m_input.ScopeId, this.m_input.PlanId, this.m_input.Job.InstanceId, context.CurrentUtcDateTime)));
    }

    private bool CanSkipExecution(
      OrchestrationContext context,
      TaskInstance taskInstance,
      TaskResult jobResult)
    {
      bool flag = taskInstance.Enabled;
      switch (jobResult)
      {
        case TaskResult.Succeeded:
        case TaskResult.SucceededWithIssues:
          return !flag;
        case TaskResult.Failed:
          flag = flag && taskInstance.AlwaysRun;
          goto case TaskResult.Succeeded;
        case TaskResult.Canceled:
          flag = false;
          goto case TaskResult.Succeeded;
        default:
          flag = false;
          context.TraceJobError(10015551, this.m_input.ScopeId, this.m_input.PlanId, this.m_input.Job.InstanceId, string.Format("RunServerJob5::CanSkipExecution -- Unexpected jobResult: {0}, so skipping task: {1}", (object) jobResult, (object) taskInstance.InstanceId));
          goto case TaskResult.Succeeded;
      }
    }

    private async Task<JobResult> RecordSkippedTask(
      OrchestrationContext context,
      TaskInstance taskInstance)
    {
      string taskInstanceDisplayName = string.IsNullOrWhiteSpace(taskInstance.DisplayName) ? taskInstance.Name : taskInstance.DisplayName;
      JobResult skippedTaskResult = new JobResult()
      {
        Result = TaskResult.Skipped
      };
      context.TraceServerTaskStarted(this.m_input.ScopeId, this.m_input.PlanId, this.m_input.Job.InstanceId, taskInstance.InstanceId);
      await this.ExecuteAsync(context, this.m_input, (Func<Task>) (() => this.ServerTaskTracker.TaskCompleted(this.m_input.ScopeId, this.m_input.PlanId, this.m_input.Job.InstanceId, taskInstance.InstanceId, context.CurrentUtcDateTime, skippedTaskResult, taskInstanceDisplayName)));
      context.TraceServerTaskCompleted(this.m_input.ScopeId, this.m_input.PlanId, this.m_input.Job.InstanceId, taskInstance.InstanceId, skippedTaskResult.Result);
      return skippedTaskResult;
    }

    private RunServerTaskInput GetServerTaskInput(TaskInstance task) => new RunServerTaskInput()
    {
      ScopeId = this.m_input.ScopeId,
      PlanId = this.m_input.PlanId,
      JobId = this.m_input.Job.InstanceId,
      Task = task,
      NotifyTaskStarted = !this.m_hasJobStarted
    };

    private Task ExecuteAsync(
      OrchestrationContext context,
      RunServerJobInput input,
      Func<Task> operation)
    {
      return context.ExecuteAsync(operation, canRetry: this.RetryException, traceException: (Action<Exception>) (ex => context.TraceJobException(input.ScopeId, input.PlanId, input.Job.InstanceId, ex)));
    }

    private Task<T> ExecuteAsync<T>(
      OrchestrationContext context,
      RunServerJobInput input,
      Func<Task<T>> operation)
    {
      return context.ExecuteAsync<T>(operation, canRetry: this.RetryException, traceException: (Action<Exception>) (ex => context.TraceJobException(input.ScopeId, input.PlanId, input.Job.InstanceId, ex)));
    }

    private void EnsureExtensions(OrchestrationContext context)
    {
      this.Tracker = context.CreateClient<IServerJobTrackingExtension3>(true);
      this.ServerTaskTracker = context.CreateClient<IServerTaskTrackingExtension>(true);
      this.ExecutionControl = context.CreateClient<IServerExecutionControlExtension>(true);
    }
  }
}
