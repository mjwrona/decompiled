// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks.RunServerTask
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
  internal sealed class RunServerTask : 
    TaskOrchestration<JobResult, RunServerTaskInput, object, string>
  {
    private RunServerTaskInput m_input;
    private bool m_hasRaisedTaskStartedNotification;
    private Dictionary<string, TaskEventConfig> m_taskEventsConfiguration;
    private TaskCompletionSource<bool> m_localCancellationCompleted = new TaskCompletionSource<bool>();
    private TaskCompletionSource<bool> m_localExecutionCompleted = new TaskCompletionSource<bool>();
    private TaskCompletionSource<bool> m_taskAssigned = new TaskCompletionSource<bool>();
    private TaskCompletionSource<bool> m_taskStarted = new TaskCompletionSource<bool>();
    private TaskCompletionSource<TaskResult> m_taskCompleted = new TaskCompletionSource<TaskResult>();
    private TaskCompletionSource<CanceledEvent> m_taskCanceled = new TaskCompletionSource<CanceledEvent>();

    public IServerTaskTrackingExtension Tracker { get; private set; }

    public IServerTaskHandlerExtension ExecutionHandler { get; private set; }

    public IServerExecutionControlExtension ControlExtension { get; private set; }

    public Func<Exception, bool> RetryException { get; set; }

    public override void OnEvent(OrchestrationContext context, string name, object input)
    {
      switch (name)
      {
        case "LocalExecutionCompleted":
          this.m_localExecutionCompleted.TrySetResult(true);
          break;
        case "LocalCancellationCompleted":
          this.m_localCancellationCompleted.TrySetResult(true);
          break;
        case "TaskAssigned":
          this.m_taskAssigned.TrySetResult(true);
          break;
        case "TaskStarted":
          this.m_taskStarted.TrySetResult(true);
          break;
        case "TaskCompleted":
          this.m_taskCompleted.TrySetResult(((TaskCompletedEvent) input).Result);
          break;
        case "TaskCanceled":
          this.m_taskCanceled.TrySetResult((CanceledEvent) input);
          break;
        default:
          context.TraceServerTaskError(10015550, this.m_input.ScopeId, this.m_input.PlanId, this.m_input.JobId, this.m_input.Task.InstanceId, "RunServerTask::OnEvent -- Unexpected eventName: " + name);
          break;
      }
    }

    public override async Task<JobResult> RunTask(
      OrchestrationContext context,
      RunServerTaskInput input)
    {
      this.EnsureExtensions(context);
      this.m_input = input;
      if (this.m_input.Task.Enabled)
        return await this.ExecuteServerTask(context, this.m_input.Task);
      return new JobResult() { Result = TaskResult.Skipped };
    }

    private static void DisposeCancellationToken(CancellationTokenSource cancellationTokenSource)
    {
      if (cancellationTokenSource == null)
        return;
      cancellationTokenSource.Cancel();
      cancellationTokenSource.Dispose();
      cancellationTokenSource = (CancellationTokenSource) null;
    }

    private async Task<JobResult> ExecuteServerTask(
      OrchestrationContext context,
      TaskInstance taskInstance)
    {
      JobResult taskResult = new JobResult()
      {
        Result = TaskResult.Succeeded
      };
      bool taskAssigned = false;
      Task<string> taskTimeoutTimerTask = (Task<string>) null;
      CancellationTokenSource taskExecutionTimerCancelTokenSource = new CancellationTokenSource();
      CancellationTokenSource eventsTimerCancel = (CancellationTokenSource) null;
      try
      {
        context.TraceServerTaskRequesting(this.m_input.ScopeId, this.m_input.PlanId, this.m_input.JobId, taskInstance.InstanceId);
        ExecuteTaskResponse executeTaskResponse = await this.ExecuteAsync<ExecuteTaskResponse>(context, this.m_input, (Func<Task<ExecuteTaskResponse>>) (() => this.ExecutionHandler.ExecuteTask(this.m_input.ScopeId, this.m_input.PlanId, this.m_input.JobId, taskInstance.InstanceId)));
        context.TraceServerTaskRequestSent(this.m_input.ScopeId, this.m_input.PlanId, this.m_input.JobId, taskInstance.InstanceId);
        this.m_taskEventsConfiguration = executeTaskResponse.TaskEvents.All;
        Queue<string> waitQueue = new Queue<string>();
        if (executeTaskResponse.WaitForLocalExecutionComplete)
        {
          waitQueue.Enqueue("LocalExecutionCompleted");
          this.m_taskEventsConfiguration["LocalExecutionCompleted"] = new TaskEventConfig(TimeSpan.FromMilliseconds((double) int.MaxValue).ToString());
        }
        waitQueue.Enqueue("TaskAssigned");
        waitQueue.Enqueue("TaskStarted");
        waitQueue.Enqueue("TaskCompleted");
        TimeSpan taskExecutionTimeoutValue = TimeSpan.FromMinutes((double) taskInstance.TimeoutInMinutes);
        if (taskExecutionTimeoutValue > TimeSpan.Zero && taskExecutionTimeoutValue < TimeSpan.FromMilliseconds((double) int.MaxValue))
          taskTimeoutTimerTask = context.CreateTimer<string>(context.CurrentUtcDateTime.Add(taskExecutionTimeoutValue), (string) null, taskExecutionTimerCancelTokenSource.Token);
        while (waitQueue.Count >= 1)
        {
          List<Task> taskList = new List<Task>()
          {
            (Task) this.m_taskCompleted.Task,
            (Task) this.m_taskCanceled.Task
          };
          if (taskTimeoutTimerTask != null)
            taskList.Add((Task) taskTimeoutTimerTask);
          string eventName = waitQueue.Dequeue();
          RunServerTask.WaitEventData waitEventDetails = this.GetWaitEventDetails(context, eventName);
          taskList.Add(waitEventDetails.WaitTask);
          RunServerTask.DisposeCancellationToken(eventsTimerCancel);
          eventsTimerCancel = new CancellationTokenSource();
          Task<string> eventsTimertask = (Task<string>) null;
          TimeSpan? timeSpan = waitEventDetails.Timeout;
          if (timeSpan.HasValue)
          {
            eventsTimertask = context.CreateTimer<string>(context.CurrentUtcDateTime.Add(timeSpan.Value), (string) null, eventsTimerCancel.Token);
            taskList.Add((Task) eventsTimertask);
          }
          context.TraceServerTaskWaiting(this.m_input.ScopeId, this.m_input.PlanId, this.m_input.JobId, taskInstance.InstanceId, eventName, timeSpan.HasValue ? timeSpan.ToString() : "Nil");
          Task task = await Task.WhenAny((IEnumerable<Task>) taskList);
          if (task == this.m_taskCompleted.Task)
          {
            if (!taskAssigned)
              context.TraceServerTaskError(10015550, this.m_input.ScopeId, this.m_input.PlanId, this.m_input.JobId, taskInstance.InstanceId, "Received task completed event with result {0} without receiving the assigned event", (object) taskResult.Result);
            eventsTimerCancel.Cancel();
            taskResult.Result = this.m_taskCompleted.Task.Result;
            goto label_38;
          }
          else if (task == taskTimeoutTimerTask)
          {
            eventsTimerCancel.Cancel();
            taskResult = await this.OnCancel(context, taskInstance, TaskCanceledReasonType.Timeout, Resources.ServerTaskTimedOut((object) taskExecutionTimeoutValue.ToString()));
            taskResult.Message = taskResult.Result == TaskResult.Succeeded ? string.Empty : Resources.ServerTaskTimedOut((object) taskExecutionTimeoutValue.ToString());
            goto label_38;
          }
          else if (task == this.m_taskCanceled.Task)
          {
            eventsTimerCancel.Cancel();
            TaskCanceledReasonType cancelType = TaskCanceledReasonType.Other;
            if (this.m_taskCanceled.Task.Result.Reason.Equals("Timeout", StringComparison.OrdinalIgnoreCase))
              cancelType = TaskCanceledReasonType.Timeout;
            taskResult = await this.OnCancel(context, taskInstance, cancelType, string.Empty);
            if (taskResult.Result == TaskResult.Succeeded)
            {
              taskResult.Message = string.Empty;
              goto label_38;
            }
            else
              goto label_38;
          }
          else if (task == eventsTimertask)
          {
            context.TraceTimedOutWaitingForEvent(this.m_input.ScopeId, this.m_input.PlanId, this.m_input.JobId, eventName, timeSpan.ToString());
            taskResult = await this.OnCancel(context, taskInstance, TaskCanceledReasonType.Timeout, Resources.ServerTaskTimedOutWaitingForEvent((object) timeSpan.ToString(), (object) eventName));
            goto label_38;
          }
          else
          {
            if (task == this.m_localExecutionCompleted.Task)
            {
              eventsTimerCancel.Cancel();
              context.TraceServerTaskLocalExecutionCompletedEventReceived(this.m_input.ScopeId, this.m_input.PlanId, this.m_input.JobId, taskInstance.InstanceId);
            }
            else if (task == this.m_taskAssigned.Task)
            {
              eventsTimerCancel.Cancel();
              taskAssigned = true;
              context.TraceServerTaskAssigned(this.m_input.ScopeId, this.m_input.PlanId, this.m_input.JobId, taskInstance.InstanceId);
            }
            else if (task == this.m_taskStarted.Task)
            {
              eventsTimerCancel.Cancel();
              await this.NotifyTaskStarted(context);
              await this.ExecuteAsync(context, this.m_input, (Func<Task>) (() => this.Tracker.TaskStarted(this.m_input.ScopeId, this.m_input.PlanId, this.m_input.JobId, taskInstance.InstanceId, context.CurrentUtcDateTime)));
              context.TraceServerTaskStarted(this.m_input.ScopeId, this.m_input.PlanId, this.m_input.JobId, taskInstance.InstanceId);
            }
            eventName = (string) null;
            eventsTimertask = (Task<string>) null;
            timeSpan = new TimeSpan?();
          }
        }
        context.TraceServerTaskError(10015550, this.m_input.ScopeId, this.m_input.PlanId, this.m_input.JobId, taskInstance.InstanceId, "Wait queue is empty and orchestration is not terminated");
label_38:
        waitQueue = (Queue<string>) null;
        taskExecutionTimeoutValue = new TimeSpan();
      }
      catch (TaskFailedException ex)
      {
        context.TraceJobException(this.m_input.ScopeId, this.m_input.PlanId, this.m_input.JobId, (Exception) ex);
        taskResult.Result = TaskResult.Failed;
        if (!taskAssigned && ex.InnerException is ServerJobFailureException)
          context.TraceServerTaskError(10015550, this.m_input.ScopeId, this.m_input.PlanId, this.m_input.JobId, this.m_input.Task.InstanceId, "ServerJobFailureException while task is not assigned: {0}", (object) ex.InnerException.Message);
        taskResult.Message = !(ex.InnerException is AggregateException) ? ex.Message : (ex.InnerException as AggregateException).Flatten().InnerExceptions.Aggregate<Exception, string>(string.Empty, (Func<string, Exception, string>) ((current, e) => current + e.Message + " "));
      }
      finally
      {
        RunServerTask.DisposeCancellationToken(taskExecutionTimerCancelTokenSource);
        RunServerTask.DisposeCancellationToken(eventsTimerCancel);
      }
      if (taskResult.Result == TaskResult.Failed && !taskResult.IsRetryable && taskInstance.ContinueOnError)
        taskResult.Result = TaskResult.SucceededWithIssues;
      await this.NotifyTaskStarted(context);
      string taskInstanceDisplayName = string.IsNullOrWhiteSpace(taskInstance.DisplayName) ? taskInstance.Name : taskInstance.DisplayName;
      await this.ExecuteAsync(context, this.m_input, (Func<Task>) (() => this.Tracker.TaskCompleted(this.m_input.ScopeId, this.m_input.PlanId, this.m_input.JobId, taskInstance.InstanceId, context.CurrentUtcDateTime, taskResult, taskInstanceDisplayName)));
      context.TraceServerTaskCompleted(this.m_input.ScopeId, this.m_input.PlanId, this.m_input.JobId, taskInstance.InstanceId, taskResult.Result);
      JobResult jobResult = taskResult;
      taskTimeoutTimerTask = (Task<string>) null;
      taskExecutionTimerCancelTokenSource = (CancellationTokenSource) null;
      eventsTimerCancel = (CancellationTokenSource) null;
      return jobResult;
    }

    private async Task NotifyTaskStarted(OrchestrationContext context)
    {
      RunServerTask runServerTask = this;
      if (!runServerTask.m_input.NotifyTaskStarted || runServerTask.m_hasRaisedTaskStartedNotification)
        return;
      runServerTask.m_hasRaisedTaskStartedNotification = true;
      // ISSUE: reference to a compiler-generated method
      await runServerTask.ExecuteAsync(context, runServerTask.m_input, new Func<Task>(runServerTask.\u003CNotifyTaskStarted\u003Eb__20_0));
    }

    private RunServerTask.WaitEventData GetWaitEventDetails(
      OrchestrationContext context,
      string eventName)
    {
      TimeSpan? nullable = new TimeSpan?();
      TaskEventConfig taskEventConfig = (TaskEventConfig) null;
      bool isConfigured = this.m_taskEventsConfiguration.TryGetValue(eventName, out taskEventConfig) && taskEventConfig.IsEnabled();
      if (isConfigured)
      {
        TimeSpan result;
        if (!TimeSpan.TryParse(taskEventConfig.Timeout, out result))
          context.TraceServerTaskError(10015550, this.m_input.ScopeId, this.m_input.PlanId, this.m_input.JobId, this.m_input.Task.InstanceId, "Unable to parse timeout: {0} for event: {1}", (object) taskEventConfig.Timeout, (object) eventName);
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
          throw new InvalidOperationException("Not known task event type");
      }
      context.TraceEventConfig(this.m_input.ScopeId, this.m_input.PlanId, this.m_input.JobId, eventName, nullable.HasValue ? nullable.ToString() : "Nil", isConfigured);
      return new RunServerTask.WaitEventData()
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
        Result = TaskResult.Canceled
      };
      context.TraceServerJobCancellationSending(this.m_input.ScopeId, this.m_input.PlanId, this.m_input.JobId);
      CancelTaskResponse cancelTaskResponse = await this.ExecuteAsync<CancelTaskResponse>(context, this.m_input, (Func<Task<CancelTaskResponse>>) (() => this.ExecutionHandler.CancelTask(this.m_input.ScopeId, this.m_input.PlanId, this.m_input.JobId, taskInstance.InstanceId, cancelType)));
      context.TraceServerJobCancellationSent(this.m_input.ScopeId, this.m_input.PlanId, this.m_input.JobId);
      if (this.m_taskCompleted.Task.IsCompleted)
      {
        result.Result = this.m_taskCompleted.Task.Result;
        return result;
      }
      if (cancelType == TaskCanceledReasonType.Timeout)
        result.Result = TaskResult.Failed;
      if (cancelTaskResponse.WaitForLocalCancellationComplete)
      {
        context.TraceServerJobWaiting(this.m_input.ScopeId, this.m_input.PlanId, this.m_input.JobId, "LocalCancellationCompleted", "Nil");
        int num = await this.m_localCancellationCompleted.Task ? 1 : 0;
        context.TraceServerJobLocalCancellationCompletedEventReceived(this.m_input.ScopeId, this.m_input.PlanId, this.m_input.JobId);
      }
      result.Message = message;
      return result;
    }

    private Task ExecuteAsync(
      OrchestrationContext context,
      RunServerTaskInput input,
      Func<Task> operation)
    {
      return context.ExecuteAsync(operation, canRetry: this.RetryException, traceException: (Action<Exception>) (ex => context.TraceTaskException(input.ScopeId, input.PlanId, input.JobId, input.Task.InstanceId, ex)));
    }

    private Task<T> ExecuteAsync<T>(
      OrchestrationContext context,
      RunServerTaskInput input,
      Func<Task<T>> operation)
    {
      return context.ExecuteAsync<T>(operation, canRetry: this.RetryException, traceException: (Action<Exception>) (ex => context.TraceTaskException(input.ScopeId, input.PlanId, input.JobId, input.Task.InstanceId, ex)));
    }

    private void EnsureExtensions(OrchestrationContext context)
    {
      this.ExecutionHandler = context.CreateClient<IServerTaskHandlerExtension>(true);
      this.Tracker = context.CreateClient<IServerTaskTrackingExtension>(true);
      this.ControlExtension = context.CreateClient<IServerExecutionControlExtension>(true);
    }

    private struct WaitEventData
    {
      internal Task WaitTask { get; set; }

      internal TimeSpan? Timeout { get; set; }
    }
  }
}
