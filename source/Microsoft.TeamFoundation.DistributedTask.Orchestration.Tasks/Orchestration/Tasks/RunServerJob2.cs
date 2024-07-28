// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks.RunServerJob2
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: ACF674F2-B05D-403A-A061-F4792BD3317C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.VisualStudio.Services.Orchestration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks
{
  internal sealed class RunServerJob2 : 
    TaskOrchestration<JobResult, RunServerJobInput, object, string>
  {
    private RunServerJobInput m_input;
    private Dictionary<string, TaskEventConfig> m_TaskEventsConfiguration;
    private TaskCompletionSource<CanceledEvent> m_jobCanceled = new TaskCompletionSource<CanceledEvent>();
    private TaskCompletionSource<bool> m_localCancellationCompleted = new TaskCompletionSource<bool>();
    private TaskCompletionSource<bool> m_localExecutionCompleted = new TaskCompletionSource<bool>();
    private TaskCompletionSource<bool> m_taskAssigned = new TaskCompletionSource<bool>();
    private TaskCompletionSource<bool> m_taskStarted = new TaskCompletionSource<bool>();
    private TaskCompletionSource<TaskResult> m_taskCompleted = new TaskCompletionSource<TaskResult>();

    public IServerJobTrackingExtension2 Tracker { get; private set; }

    public IServerJobHandlerExtension JobExecutionHandler { get; private set; }

    public Func<Exception, bool> RetryException { get; set; }

    public override void OnEvent(OrchestrationContext context, string name, object input)
    {
      if (name == null)
        return;
      switch (name.Length)
      {
        case 10:
          if (!(name == "JobStarted"))
            return;
          goto label_27;
        case 11:
          switch (name[3])
          {
            case 'A':
              if (!(name == "JobAssigned"))
                return;
              break;
            case 'C':
              if (!(name == "JobCanceled"))
                return;
              this.m_jobCanceled.TrySetResult((CanceledEvent) input);
              return;
            case 'k':
              if (!(name == "TaskStarted"))
                return;
              goto label_27;
            default:
              return;
          }
          break;
        case 12:
          switch (name[0])
          {
            case 'J':
              if (!(name == "JobCompleted"))
                return;
              this.m_taskCompleted.TrySetResult(((JobCompletedEvent) input).Result);
              context.TraceJobInfo(10015553, this.m_input.ScopeId, this.m_input.PlanId, this.m_input.Job.InstanceId, "RunServerJob2::OnEvent -- JobCompleted is invoked.");
              return;
            case 'T':
              if (!(name == "TaskAssigned"))
                return;
              break;
            default:
              return;
          }
          break;
        case 13:
          if (!(name == "TaskCompleted"))
            return;
          this.m_taskCompleted.TrySetResult(((TaskCompletedEvent) input).Result);
          return;
        case 23:
          if (!(name == "LocalExecutionCompleted"))
            return;
          this.m_localExecutionCompleted.TrySetResult(true);
          return;
        case 26:
          if (!(name == "LocalCancellationCompleted"))
            return;
          this.m_localCancellationCompleted.TrySetResult(true);
          return;
        default:
          return;
      }
      this.m_taskAssigned.TrySetResult(true);
      return;
label_27:
      this.m_taskStarted.TrySetResult(true);
    }

    public override async Task<JobResult> RunTask(
      OrchestrationContext context,
      RunServerJobInput input)
    {
      this.EnsureExtensions(context);
      this.m_input = input;
      TaskOrchestrationJob orchestrationJob = await this.ExecuteAsync<TaskOrchestrationJob>(context, this.m_input, (Func<Task<TaskOrchestrationJob>>) (() => this.JobExecutionHandler.GetJob(this.m_input.ScopeId, this.m_input.PlanId, this.m_input.Job.InstanceId)));
      if (orchestrationJob.Tasks.Count != 1)
      {
        await this.ExecuteAsync(context, this.m_input, (Func<Task>) (() => this.Tracker.LogIssue(this.m_input.ScopeId, this.m_input.PlanId, this.m_input.Job.InstanceId, context.CurrentUtcDateTime, IssueType.Error, Resources.InvalidServerJob())));
        return new JobResult()
        {
          Result = TaskResult.Failed
        };
      }
      TaskInstance taskInstance = orchestrationJob.Tasks.Single<TaskInstance>();
      JobResult jobResult = new JobResult()
      {
        Result = TaskResult.Succeeded
      };
      if (taskInstance.Enabled)
        jobResult = await this.RunServerTask(context, taskInstance);
      context.TraceServerJobCompleted(this.m_input.ScopeId, this.m_input.PlanId, this.m_input.Job.InstanceId, jobResult.Result);
      await this.ExecuteAsync(context, this.m_input, (Func<Task>) (() => this.Tracker.JobCompleted(this.m_input.ScopeId, this.m_input.PlanId, this.m_input.Job.InstanceId, context.CurrentUtcDateTime, jobResult.Result)));
      return jobResult;
    }

    private Task<string> CreateTimeoutTask(
      OrchestrationContext context,
      TimeSpan executionTimeout,
      CancellationTokenSource cancellationTokenSource)
    {
      Task<string> timeoutTask = (Task<string>) null;
      if (executionTimeout > TimeSpan.Zero && executionTimeout < TimeSpan.FromMilliseconds((double) int.MaxValue))
        timeoutTask = context.CreateTimer<string>(context.CurrentUtcDateTime.Add(executionTimeout), (string) null, cancellationTokenSource.Token);
      return timeoutTask;
    }

    private async Task<JobResult> RunServerTask(
      OrchestrationContext context,
      TaskInstance taskInstance)
    {
      JobResult taskResult = new JobResult()
      {
        Result = TaskResult.Succeeded
      };
      bool taskAssigned = false;
      Task<string> taskExecutionTimerTask = (Task<string>) null;
      Task<string> JobExecutionTimerTask = (Task<string>) null;
      CancellationTokenSource taskExecutionTimerCancelTokenSource = new CancellationTokenSource();
      CancellationTokenSource jobExecutionTimerCancelTokenSource = new CancellationTokenSource();
      try
      {
        context.TraceServerJobRequesting(this.m_input.ScopeId, this.m_input.PlanId, this.m_input.Job.InstanceId);
        ExecuteTaskResponse executeTaskResponse = await this.ExecuteAsync<ExecuteTaskResponse>(context, this.m_input, (Func<Task<ExecuteTaskResponse>>) (() => this.JobExecutionHandler.ExecuteTask(this.m_input.ScopeId, this.m_input.PlanId, this.m_input.Job.InstanceId, taskInstance.InstanceId)));
        context.TraceServerJobRequestSent(this.m_input.ScopeId, this.m_input.PlanId, this.m_input.Job.InstanceId);
        this.m_TaskEventsConfiguration = executeTaskResponse.TaskEvents.All;
        Queue<string> waitQueue = new Queue<string>();
        if (executeTaskResponse.WaitForLocalExecutionComplete)
        {
          waitQueue.Enqueue("LocalExecutionCompleted");
          this.m_TaskEventsConfiguration["LocalExecutionCompleted"] = new TaskEventConfig(TimeSpan.FromMilliseconds((double) int.MaxValue).ToString());
        }
        waitQueue.Enqueue("TaskAssigned");
        waitQueue.Enqueue("TaskStarted");
        waitQueue.Enqueue("TaskCompleted");
        TimeSpan jobExecutionTimeoutValue = TimeSpan.Zero;
        TimeSpan? executionTimeout = this.m_input.Job.ExecutionTimeout;
        ref TimeSpan? local = ref executionTimeout;
        if ((local.HasValue ? (local.GetValueOrDefault().TotalMinutes > 0.0 ? 1 : 0) : 0) != 0)
        {
          executionTimeout = this.m_input.Job.ExecutionTimeout;
          jobExecutionTimeoutValue = executionTimeout.Value;
          JobExecutionTimerTask = this.CreateTimeoutTask(context, jobExecutionTimeoutValue, jobExecutionTimerCancelTokenSource);
        }
        TimeSpan taskExecutionTimeoutValue = TimeSpan.FromMinutes((double) taskInstance.TimeoutInMinutes);
        taskExecutionTimerTask = this.CreateTimeoutTask(context, taskExecutionTimeoutValue, taskExecutionTimerCancelTokenSource);
        while (waitQueue.Count >= 1)
        {
          List<Task> taskList = new List<Task>();
          taskList.Add((Task) this.m_taskCompleted.Task);
          taskList.Add((Task) this.m_jobCanceled.Task);
          if (taskExecutionTimerTask != null)
            taskList.Add((Task) taskExecutionTimerTask);
          if (JobExecutionTimerTask != null)
            taskList.Add((Task) JobExecutionTimerTask);
          string eventName = waitQueue.Dequeue();
          RunServerJob2.WaitEventData waitEventDetails = this.GetWaitEventDetails(context, eventName);
          taskList.Add(waitEventDetails.WaitTask);
          CancellationTokenSource eventsTimerCancel = new CancellationTokenSource();
          Task<string> eventsTimertask = (Task<string>) null;
          TimeSpan? timeSpan = waitEventDetails.Timeout;
          if (timeSpan.HasValue)
          {
            eventsTimertask = context.CreateTimer<string>(context.CurrentUtcDateTime.Add(timeSpan.Value), (string) null, eventsTimerCancel.Token);
            taskList.Add((Task) eventsTimertask);
          }
          context.TraceServerJobWaiting(this.m_input.ScopeId, this.m_input.PlanId, this.m_input.Job.InstanceId, eventName, timeSpan.HasValue ? timeSpan.ToString() : "Nil");
          Task task = await Task.WhenAny((IEnumerable<Task>) taskList);
          if (task == this.m_taskCompleted.Task)
          {
            if (!taskAssigned)
              context.TraceJobError(10015550, this.m_input.ScopeId, this.m_input.PlanId, this.m_input.Job.InstanceId, "Received job completed event with result {0} without receiving the assigned event", (object) taskResult.Result);
            eventsTimerCancel.Cancel();
            taskResult.Result = this.m_taskCompleted.Task.Result;
            goto label_40;
          }
          else if (task == this.m_jobCanceled.Task)
          {
            eventsTimerCancel.Cancel();
            CanceledEvent result = this.m_jobCanceled.Task.Result;
            taskResult = await this.OnCancel(context, taskInstance, TaskCanceledReasonType.Other, result.Reason);
            goto label_40;
          }
          else if (task == JobExecutionTimerTask)
          {
            eventsTimerCancel.Cancel();
            taskResult = await this.OnCancel(context, taskInstance, TaskCanceledReasonType.Timeout, Resources.ServerJobTimedOut((object) jobExecutionTimeoutValue.ToString()));
            goto label_40;
          }
          else if (task == taskExecutionTimerTask)
          {
            eventsTimerCancel.Cancel();
            taskResult = await this.OnCancel(context, taskInstance, TaskCanceledReasonType.Timeout, Resources.ServerTaskTimedOut((object) taskExecutionTimeoutValue.ToString()));
            goto label_40;
          }
          else if (task == eventsTimertask)
          {
            context.TraceTimedOutWaitingForEvent(this.m_input.ScopeId, this.m_input.PlanId, this.m_input.Job.InstanceId, eventName, timeSpan.ToString());
            taskResult = await this.OnCancel(context, taskInstance, TaskCanceledReasonType.Timeout, Resources.JobTimedOutWaitingForEvent((object) timeSpan.ToString(), (object) eventName));
            goto label_40;
          }
          else
          {
            if (task == this.m_localExecutionCompleted.Task)
            {
              eventsTimerCancel.Cancel();
              context.TraceServerJobLocalExecutionCompletedEventReceived(this.m_input.ScopeId, this.m_input.PlanId, this.m_input.Job.InstanceId);
            }
            else if (task == this.m_taskAssigned.Task)
            {
              eventsTimerCancel.Cancel();
              taskAssigned = true;
              context.TraceServerJobAssigned(this.m_input.ScopeId, this.m_input.PlanId, this.m_input.Job.InstanceId);
            }
            else if (task == this.m_taskStarted.Task)
            {
              eventsTimerCancel.Cancel();
              await this.ExecuteAsync(context, this.m_input, (Func<Task>) (() => this.Tracker.TaskStarted(this.m_input.ScopeId, this.m_input.PlanId, this.m_input.Job.InstanceId, taskInstance.InstanceId, context.CurrentUtcDateTime)));
              context.TraceServerJobStarted(this.m_input.ScopeId, this.m_input.PlanId, this.m_input.Job.InstanceId);
            }
            eventName = (string) null;
            eventsTimerCancel = (CancellationTokenSource) null;
            eventsTimertask = (Task<string>) null;
            timeSpan = new TimeSpan?();
          }
        }
        context.TraceJobError(10015551, this.m_input.ScopeId, this.m_input.PlanId, this.m_input.Job.InstanceId, "Wait queue is empty and orchestration is not terminated");
label_40:
        waitQueue = (Queue<string>) null;
        jobExecutionTimeoutValue = new TimeSpan();
        taskExecutionTimeoutValue = new TimeSpan();
      }
      catch (TaskFailedException ex)
      {
        context.TraceJobException(this.m_input.ScopeId, this.m_input.PlanId, this.m_input.Job.InstanceId, (Exception) ex);
        taskResult.Result = TaskResult.Failed;
        if (!taskAssigned && ex.InnerException is ServerJobFailureException)
          taskResult.IsRetryable = true;
        taskResult.Message = !(ex.InnerException is AggregateException) ? ex.Message : (ex.InnerException as AggregateException).Flatten().InnerExceptions.Aggregate<Exception, string>(string.Empty, (Func<string, Exception, string>) ((current, e) => current + e.Message + " "));
      }
      finally
      {
        this.ReleaseCancellationToken(taskExecutionTimerCancelTokenSource);
        this.ReleaseCancellationToken(jobExecutionTimerCancelTokenSource);
      }
      if (taskResult.Result == TaskResult.Failed && !taskResult.IsRetryable && taskInstance.ContinueOnError)
        taskResult.Result = TaskResult.SucceededWithIssues;
      await this.ExecuteAsync(context, this.m_input, (Func<Task>) (() => this.Tracker.TaskCompleted(this.m_input.ScopeId, this.m_input.PlanId, this.m_input.Job.InstanceId, taskInstance.InstanceId, context.CurrentUtcDateTime, taskResult.Result)));
      JobResult jobResult = taskResult;
      taskExecutionTimerTask = (Task<string>) null;
      JobExecutionTimerTask = (Task<string>) null;
      taskExecutionTimerCancelTokenSource = (CancellationTokenSource) null;
      jobExecutionTimerCancelTokenSource = (CancellationTokenSource) null;
      return jobResult;
    }

    private void ReleaseCancellationToken(CancellationTokenSource cancellationTokenSource)
    {
      if (cancellationTokenSource == null)
        return;
      cancellationTokenSource.Cancel();
      cancellationTokenSource = (CancellationTokenSource) null;
    }

    private RunServerJob2.WaitEventData GetWaitEventDetails(
      OrchestrationContext context,
      string eventName)
    {
      TimeSpan? nullable = new TimeSpan?();
      TaskEventConfig taskEventConfig = (TaskEventConfig) null;
      bool isConfigured = this.m_TaskEventsConfiguration.TryGetValue(eventName, out taskEventConfig) && taskEventConfig.IsEnabled();
      if (isConfigured)
      {
        TimeSpan result;
        if (!TimeSpan.TryParse(taskEventConfig.Timeout, out result))
          context.TraceJobError(10015550, this.m_input.ScopeId, this.m_input.PlanId, this.m_input.Job.InstanceId, "Unable to parse timeout: {0} for event: {1}", (object) taskEventConfig.Timeout, (object) eventName);
        else if (result > TimeSpan.Zero && result < TimeSpan.FromMilliseconds((double) int.MaxValue))
          nullable = new TimeSpan?(result);
      }
      Task task;
      switch (eventName)
      {
        case "LocalExecutionCompleted":
          if (!isConfigured)
            this.m_localExecutionCompleted.TrySetResult(true);
          task = (Task) this.m_localExecutionCompleted.Task;
          break;
        case "TaskAssigned":
          if (!isConfigured)
            this.m_taskAssigned.TrySetResult(true);
          task = (Task) this.m_taskAssigned.Task;
          break;
        case "TaskStarted":
          if (!isConfigured)
            this.m_taskStarted.TrySetResult(true);
          task = (Task) this.m_taskStarted.Task;
          break;
        case "TaskCompleted":
          if (!isConfigured)
            this.m_taskCompleted.TrySetResult(TaskResult.Succeeded);
          task = (Task) this.m_taskCompleted.Task;
          break;
        default:
          throw new InvalidOperationException("Not known job event type");
      }
      context.TraceEventConfig(this.m_input.ScopeId, this.m_input.PlanId, this.m_input.Job.InstanceId, eventName, nullable.HasValue ? nullable.ToString() : "Nil", isConfigured);
      return new RunServerJob2.WaitEventData()
      {
        WaitTask = task,
        Timeout = nullable
      };
    }

    private async Task<JobResult> OnCancel(
      OrchestrationContext context,
      TaskInstance taskInstance,
      TaskCanceledReasonType cancelType,
      string message)
    {
      JobResult result = new JobResult()
      {
        Result = TaskResult.Canceled,
        Message = message
      };
      context.TraceServerJobCancellationSending(this.m_input.ScopeId, this.m_input.PlanId, this.m_input.Job.InstanceId);
      CancelTaskResponse cancelTaskResponse = await this.ExecuteAsync<CancelTaskResponse>(context, this.m_input, (Func<Task<CancelTaskResponse>>) (() => this.JobExecutionHandler.CancelTask(this.m_input.ScopeId, this.m_input.PlanId, this.m_input.Job.InstanceId, taskInstance.InstanceId, cancelType)));
      context.TraceServerJobCancellationSent(this.m_input.ScopeId, this.m_input.PlanId, this.m_input.Job.InstanceId);
      if (this.m_taskCompleted.Task.IsCompleted)
      {
        result.Result = this.m_taskCompleted.Task.Result;
        if (this.m_taskCompleted.Task.Result == TaskResult.Succeeded)
          result.Message = string.Empty;
        return result;
      }
      if (cancelType == TaskCanceledReasonType.Timeout)
        result.Result = TaskResult.Failed;
      if (cancelTaskResponse.WaitForLocalCancellationComplete)
      {
        context.TraceServerJobWaiting(this.m_input.ScopeId, this.m_input.PlanId, this.m_input.Job.InstanceId, "LocalCancellationCompleted", "Nil");
        int num = await this.m_localCancellationCompleted.Task ? 1 : 0;
        context.TraceServerJobLocalCancellationCompletedEventReceived(this.m_input.ScopeId, this.m_input.PlanId, this.m_input.Job.InstanceId);
      }
      return result;
    }

    private Task ExecuteAsync(
      OrchestrationContext context,
      RunServerJobInput input,
      Func<Task> operation,
      int maxAttempts = 5)
    {
      return context.ExecuteAsync(operation, maxAttempts, canRetry: this.RetryException, traceException: (Action<Exception>) (ex => context.TraceJobException(input.ScopeId, input.PlanId, input.Job.InstanceId, ex)));
    }

    private Task<T> ExecuteAsync<T>(
      OrchestrationContext context,
      RunServerJobInput input,
      Func<Task<T>> operation,
      int maxAttempts = 5)
    {
      return context.ExecuteAsync<T>(operation, maxAttempts, canRetry: this.RetryException, traceException: (Action<Exception>) (ex => context.TraceJobException(input.ScopeId, input.PlanId, input.Job.InstanceId, ex)));
    }

    private void EnsureExtensions(OrchestrationContext context)
    {
      this.JobExecutionHandler = context.CreateClient<IServerJobHandlerExtension>(true);
      this.Tracker = context.CreateClient<IServerJobTrackingExtension2>(true);
    }

    private struct WaitEventData
    {
      internal Task WaitTask { get; set; }

      internal TimeSpan? Timeout { get; set; }
    }
  }
}
