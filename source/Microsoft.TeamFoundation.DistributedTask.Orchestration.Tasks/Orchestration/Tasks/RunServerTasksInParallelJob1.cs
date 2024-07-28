// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks.RunServerTasksInParallelJob1
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
  internal sealed class RunServerTasksInParallelJob1 : 
    TaskOrchestration<JobResult, RunServerJobInput, object, string>
  {
    private RunServerJobInput m_input;
    private bool m_hasJobStarted;
    private bool m_hasOnlyOneTask;
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
          context.TraceJobError(10015551, this.m_input.ScopeId, this.m_input.PlanId, this.m_input.Job.InstanceId, "RunServerTasksInParallelJob1::OnEvent -- Unexpected JobCompleted event when more than one task exist.");
          break;
        case "JobStarted":
          this.m_jobStarted.TrySetResult(true);
          break;
        default:
          context.TraceJobError(10015551, this.m_input.ScopeId, this.m_input.PlanId, this.m_input.Job.InstanceId, "RunServerTasksInParallelJob1::OnEvent -- Unexpected eventName: " + name);
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
        TimeSpan jobTimeoutValue;
        Task<string> jobTimeoutTask = this.GetJobTimeoutTask(context, jobTimeoutCancellationTokenSource, out jobTimeoutValue);
        List<Task> waitingTasks = new List<Task>()
        {
          (Task) this.m_jobCanceled.Task
        };
        Dictionary<Task, string> userTasks = new Dictionary<Task, string>();
        if (jobTimeoutTask != null)
          waitingTasks.Add((Task) jobTimeoutTask);
        if (this.m_hasOnlyOneTask)
        {
          waitingTasks.Add((Task) this.m_jobStarted.Task);
          waitingTasks.Add((Task) this.m_jobAssigned.Task);
          waitingTasks.Add((Task) this.m_jobCompleted.Task);
        }
        await this.PerformStartJob(context);
        foreach (TaskInstance task in job.Tasks)
        {
          string orchestrationId;
          Task<JobResult> key = this.ExecuteSingleTask(context, jobResult, task, out orchestrationId);
          waitingTasks.Add((Task) key);
          if (!string.IsNullOrWhiteSpace(orchestrationId))
            userTasks.Add((Task) key, orchestrationId);
        }
        while (userTasks.Count > 0)
        {
          Task completedTask = await Task.WhenAny((IEnumerable<Task>) waitingTasks);
          waitingTasks.RemoveAt(waitingTasks.IndexOf(completedTask));
          JobResult taskResult = new JobResult()
          {
            Result = TaskResult.Succeeded
          };
          if (userTasks.ContainsKey(completedTask))
          {
            taskResult = (completedTask as Task<JobResult>).Result;
            userTasks.Remove(completedTask);
          }
          if (completedTask == jobTimeoutTask)
          {
            jobResult.Message = Resources.ServerJob4TimedOut((object) jobTimeoutValue.ToString());
            taskResult = new JobResult()
            {
              Result = TaskResult.Canceled
            };
            await this.CancelAllRunningTasks(context, userTasks, jobTimeoutValue, TaskCanceledReasonType.Timeout);
            userTasks.Clear();
          }
          if (completedTask == this.m_jobCanceled.Task)
          {
            jobResult.Message = this.m_jobCanceled.Task.Result.Reason;
            taskResult = new JobResult()
            {
              Result = TaskResult.Canceled
            };
            await this.CancelAllRunningTasks(context, userTasks, jobTimeoutValue, TaskCanceledReasonType.Other);
            userTasks.Clear();
          }
          if (this.m_hasOnlyOneTask && completedTask == this.m_jobStarted.Task)
          {
            TaskInstance task = job.Tasks[0];
            context.TraceServerJobStartTask(this.m_input.ScopeId, this.m_input.PlanId, this.m_input.Job.InstanceId, task.InstanceId);
            await this.ExecuteAsync(context, this.m_input, (Func<Task>) (() => this.ExecutionControl.StartTask(this.m_input.ScopeId, this.m_input.PlanId, this.m_input.Job.InstanceId, task.InstanceId)));
          }
          else if (this.m_hasOnlyOneTask && completedTask == this.m_jobAssigned.Task)
          {
            TaskInstance task = job.Tasks[0];
            context.TraceServerJobAssignTask(this.m_input.ScopeId, this.m_input.PlanId, this.m_input.Job.InstanceId, task.InstanceId);
            await this.ExecuteAsync(context, this.m_input, (Func<Task>) (() => this.ExecutionControl.AssignTask(this.m_input.ScopeId, this.m_input.PlanId, this.m_input.Job.InstanceId, task.InstanceId)));
          }
          else if (this.m_hasOnlyOneTask && completedTask == this.m_jobCompleted.Task)
          {
            TaskInstance task = job.Tasks[0];
            TaskCompletedEvent taskCompletedEventData = new TaskCompletedEvent(this.m_input.Job.InstanceId, task.InstanceId, this.m_jobCompleted.Task.Result);
            await this.ExecuteAsync(context, this.m_input, (Func<Task>) (() => this.ExecutionControl.CompleteTask(this.m_input.ScopeId, this.m_input.PlanId, this.m_input.Job.InstanceId, task.InstanceId, (TaskEvent) taskCompletedEventData)));
          }
          jobResult = RunServerTasksInParallelJob1.MergeResult(jobResult, taskResult);
          completedTask = (Task) null;
          taskResult = (JobResult) null;
        }
        jobTimeoutTask = (Task<string>) null;
        jobTimeoutValue = new TimeSpan();
        waitingTasks = (List<Task>) null;
        userTasks = (Dictionary<Task, string>) null;
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
      JobResult jobResult1 = jobResult;
      job = (TaskOrchestrationJob) null;
      jobTimeoutCancellationTokenSource = (CancellationTokenSource) null;
      return jobResult1;
    }

    private async Task CancelAllRunningTasks(
      OrchestrationContext context,
      Dictionary<Task, string> userTasks,
      TimeSpan jobTimeoutValue,
      TaskCanceledReasonType cancelReason)
    {
      List<Task> taskList = new List<Task>();
      List<Task> suborchestrationTasks = new List<Task>();
      foreach (KeyValuePair<Task, string> userTask in userTasks)
      {
        KeyValuePair<Task, string> kvp = userTask;
        suborchestrationTasks.Add(kvp.Key);
        Task task = this.ExecuteAsync(context, this.m_input, (Func<Task>) (() => this.ExecutionControl.CancelTask(kvp.Value, jobTimeoutValue, cancelReason.ToString())));
        taskList.Add(task);
      }
      await Task.WhenAll((IEnumerable<Task>) taskList);
      await Task.WhenAll((IEnumerable<Task>) suborchestrationTasks);
      suborchestrationTasks = (List<Task>) null;
    }

    private Task<JobResult> ExecuteSingleTask(
      OrchestrationContext context,
      JobResult jobResult,
      TaskInstance task,
      out string orchestrationId)
    {
      if (this.CanSkipExecution(context, task, jobResult.Result))
      {
        orchestrationId = (string) null;
        return this.RecordSkippedTask(context, task);
      }
      RunServerTaskInput serverTaskInput = this.GetServerTaskInput(task);
      orchestrationId = ServerTaskExtensions.GetServerTaskOrchestrationId(this.m_input.PlanId, this.m_input.Job.InstanceId, task.InstanceId);
      return RunServerTasksInParallelJob1.CreateSubOrchestrationInstance(context, serverTaskInput, orchestrationId);
    }

    private Task<string> GetJobTimeoutTask(
      OrchestrationContext context,
      CancellationTokenSource jobTimeoutCancellationTokenSource,
      out TimeSpan jobTimeoutValue)
    {
      Task<string> jobTimeoutTask = (Task<string>) null;
      jobTimeoutValue = TimeSpan.Zero;
      TimeSpan? executionTimeout = this.m_input.Job.ExecutionTimeout;
      ref TimeSpan? local1 = ref executionTimeout;
      if ((local1.HasValue ? (local1.GetValueOrDefault().TotalMinutes > 0.0 ? 1 : 0) : 0) != 0)
      {
        ref TimeSpan local2 = ref jobTimeoutValue;
        executionTimeout = this.m_input.Job.ExecutionTimeout;
        TimeSpan timeSpan = executionTimeout.Value;
        local2 = timeSpan;
        if (jobTimeoutValue > TimeSpan.Zero && jobTimeoutValue < TimeSpan.FromMilliseconds((double) int.MaxValue))
          jobTimeoutTask = context.CreateTimer<string>(context.CurrentUtcDateTime.Add(jobTimeoutValue), (string) null, jobTimeoutCancellationTokenSource.Token);
      }
      return jobTimeoutTask;
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
      if (this.m_hasJobStarted)
        return;
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
          context.TraceJobError(10015551, this.m_input.ScopeId, this.m_input.PlanId, this.m_input.Job.InstanceId, string.Format("RunServerTasksInParallelJob1::CanSkipExecution -- Unexpected jobResult: {0}, so skipping task: {1}", (object) jobResult, (object) taskInstance.InstanceId));
          goto case TaskResult.Succeeded;
      }
    }

    private async Task<JobResult> RecordSkippedTask(
      OrchestrationContext context,
      TaskInstance taskInstance)
    {
      string str = string.IsNullOrWhiteSpace(taskInstance.DisplayName) ? taskInstance.Name : taskInstance.DisplayName;
      JobResult skippedTaskResult = new JobResult()
      {
        Result = TaskResult.Skipped
      };
      TimelineRecord taskRecord = new TimelineRecord()
      {
        Id = taskInstance.InstanceId,
        Name = str,
        RecordType = "Task",
        ParentId = new Guid?(this.m_input.Job.InstanceId),
        StartTime = new DateTime?(DateTime.UtcNow),
        FinishTime = new DateTime?(DateTime.UtcNow),
        State = new TimelineRecordState?(TimelineRecordState.Completed),
        Result = new TaskResult?(skippedTaskResult.Result)
      };
      context.TraceServerTaskStarted(this.m_input.ScopeId, this.m_input.PlanId, this.m_input.Job.InstanceId, taskInstance.InstanceId);
      await this.ExecuteAsync(context, this.m_input, (Func<Task>) (() => this.ServerTaskTracker.UpdateTimeline(this.m_input.ScopeId, this.m_input.PlanId, (IList<TimelineRecord>) new TimelineRecord[1]
      {
        taskRecord
      })));
      context.TraceServerTaskCompleted(this.m_input.ScopeId, this.m_input.PlanId, this.m_input.Job.InstanceId, taskInstance.InstanceId, skippedTaskResult.Result);
      JobResult jobResult = skippedTaskResult;
      skippedTaskResult = (JobResult) null;
      return jobResult;
    }

    private RunServerTaskInput GetServerTaskInput(TaskInstance task) => new RunServerTaskInput()
    {
      ScopeId = this.m_input.ScopeId,
      PlanId = this.m_input.PlanId,
      JobId = this.m_input.Job.InstanceId,
      Task = task,
      NotifyTaskStarted = false
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
