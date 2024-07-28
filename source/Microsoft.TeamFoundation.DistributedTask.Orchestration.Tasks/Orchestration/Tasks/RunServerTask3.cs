// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks.RunServerTask3
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
  internal sealed class RunServerTask3 : 
    TaskOrchestration<JobResult, RunServerTaskInput, object, string>
  {
    private RunServerTaskInput m_input;
    private bool m_hasRaisedTaskStartedNotification;
    private Dictionary<string, TaskEventConfig> m_taskEventsConfiguration;
    private TaskCompletionSource<bool> m_localCancellationCompleted = new TaskCompletionSource<bool>();
    private TaskCompletionSource<object> m_localExecutionCompleted = new TaskCompletionSource<object>();
    private TaskCompletionSource<bool> m_taskAssigned = new TaskCompletionSource<bool>();
    private TaskCompletionSource<bool> m_taskStarted = new TaskCompletionSource<bool>();
    private TaskCompletionSource<TaskResult> m_taskCompleted = new TaskCompletionSource<TaskResult>();
    private TaskCompletionSource<CanceledEvent> m_taskCanceled = new TaskCompletionSource<CanceledEvent>();
    private TaskCompletionSource<ExternalVariablesDownloadEvent> m_fetchedExternalVariables = new TaskCompletionSource<ExternalVariablesDownloadEvent>();
    private const int c_fetchExternalVariablesJobTimeout = 180000;

    public IServerTaskTrackingExtension Tracker { get; private set; }

    public IServerTaskHandlerExtension2 ExecutionHandler { get; private set; }

    public IServerExecutionControlExtension ControlExtension { get; private set; }

    public Func<Exception, bool> RetryException { get; set; }

    public override void OnEvent(OrchestrationContext context, string name, object input)
    {
      if (name != null)
      {
        switch (name.Length)
        {
          case 11:
            if (name == "TaskStarted")
            {
              this.m_taskStarted.TrySetResult(true);
              return;
            }
            break;
          case 12:
            switch (name[4])
            {
              case 'A':
                if (name == "TaskAssigned")
                {
                  this.m_taskAssigned.TrySetResult(true);
                  return;
                }
                break;
              case 'C':
                if (name == "TaskCanceled")
                {
                  this.m_taskCanceled.TrySetResult((CanceledEvent) input);
                  return;
                }
                break;
            }
            break;
          case 13:
            if (name == "TaskCompleted")
            {
              this.m_taskCompleted.TrySetResult(((TaskCompletedEvent) input).Result);
              return;
            }
            break;
          case 23:
            if (name == "LocalExecutionCompleted")
            {
              this.m_localExecutionCompleted.TrySetResult(input);
              return;
            }
            break;
          case 24:
            if (name == "FetchedExternalVariables")
            {
              this.m_fetchedExternalVariables.TrySetResult((ExternalVariablesDownloadEvent) input);
              return;
            }
            break;
          case 26:
            if (name == "LocalCancellationCompleted")
            {
              this.m_localCancellationCompleted.TrySetResult(true);
              return;
            }
            break;
        }
      }
      context.TraceServerTaskError(10015550, this.m_input.ScopeId, this.m_input.PlanId, this.m_input.JobId, this.m_input.Task.InstanceId, "RunServerTask::OnEvent -- Unexpected eventName: " + name);
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
      int sectionIndex = 0;
      bool isPreviousSectionSuccessful = true;
      Task<string> taskTimeoutTimerTask = (Task<string>) null;
      RunServerTask3.InvokeTaskHandlerResponse taskHandlerResponse = new RunServerTask3.InvokeTaskHandlerResponse();
      taskHandlerResponse.IsCompleted = true;
      taskHandlerResponse.JobResult = new JobResult()
      {
        Result = TaskResult.Failed
      };
      RunServerTask3.InvokeTaskHandlerResponse invokeTaskHandlerResponse = taskHandlerResponse;
      CancellationTokenSource taskExecutionTimerCancelTokenSource = new CancellationTokenSource();
      CancellationTokenSource retryTaskToken = new CancellationTokenSource();
      TimeSpan timeSpan = TimeSpan.FromMinutes((double) taskInstance.TimeoutInMinutes);
      if (timeSpan > TimeSpan.Zero && timeSpan < TimeSpan.FromMilliseconds((double) int.MaxValue))
        taskTimeoutTimerTask = context.CreateTimer<string>(context.CurrentUtcDateTime.Add(timeSpan), (string) null, taskExecutionTimerCancelTokenSource.Token);
      try
      {
        IList<ServerTaskSectionExecutionOptions> sectionOptions = await this.ExecutionHandler.GetTaskSectionExecutionOptions(this.m_input.ScopeId, this.m_input.PlanId, this.m_input.JobId, taskInstance.InstanceId);
        await this.ExecuteAsync(context, this.m_input, (Func<Task>) (() => this.ExecutionHandler.QueueExternalVariablesDownloadJob(this.m_input.ScopeId, this.m_input.PlanId, this.m_input.JobId, taskInstance.InstanceId)));
        Task task = await Task.WhenAny((Task) this.m_fetchedExternalVariables.Task, Task.Delay(180000));
        if (this.m_fetchedExternalVariables.Task.Result.Result == TaskResult.Succeeded)
          sectionOptions = this.m_fetchedExternalVariables.Task.Result.ExecutionOptions;
        int handlerInvocationCount = 1;
        while (sectionOptions.Count > sectionIndex & isPreviousSectionSuccessful)
        {
          bool flag = sectionOptions.Count > sectionIndex + 1;
          ServerTaskExecutionContext serverTaskExecutionContext = new ServerTaskExecutionContext()
          {
            CurrentSectionIndex = sectionIndex,
            UseExistingTimelineRecord = sectionIndex > 0,
            HasMoreSections = flag,
            UseNewOrchestrationIdentifierForGates = true,
            HandlerInvocationCount = handlerInvocationCount
          };
          taskHandlerResponse = await this.InvokeTaskHandler(context, taskInstance, taskTimeoutTimerTask, serverTaskExecutionContext);
          invokeTaskHandlerResponse = taskHandlerResponse;
          ServerTaskSectionExecutionOptions currentSectionOptions = sectionOptions[sectionIndex];
          if (currentSectionOptions.IsRetryable && !invokeTaskHandlerResponse.IsCompleted)
          {
            for (; !invokeTaskHandlerResponse.IsCompleted; invokeTaskHandlerResponse = taskHandlerResponse)
            {
              if (await this.CreateCancellableTimerTask(context, currentSectionOptions.DelayBetweenRetries.Value, retryTaskToken) == TaskResult.Succeeded)
              {
                this.ResetEventsForNextInvocation();
                serverTaskExecutionContext.UseExistingTimelineRecord = true;
                ++serverTaskExecutionContext.HandlerInvocationCount;
                taskHandlerResponse = await this.InvokeTaskHandler(context, taskInstance, taskTimeoutTimerTask, serverTaskExecutionContext);
              }
              else
              {
                JobResult jobResult = await this.OnCancel(context, taskInstance, serverTaskExecutionContext, TaskCanceledReasonType.Other, string.Empty);
                break;
              }
            }
          }
          this.ResetEventsForNextInvocation();
          isPreviousSectionSuccessful = invokeTaskHandlerResponse.JobResult.Result == TaskResult.Succeeded;
          ++sectionIndex;
          serverTaskExecutionContext = (ServerTaskExecutionContext) null;
          currentSectionOptions = (ServerTaskSectionExecutionOptions) null;
        }
        sectionOptions = (IList<ServerTaskSectionExecutionOptions>) null;
      }
      finally
      {
        RunServerTask3.DisposeCancellationToken(retryTaskToken);
        RunServerTask3.DisposeCancellationToken(taskExecutionTimerCancelTokenSource);
      }
      await this.NotifyTaskStarted(context, taskInstance.InstanceId);
      JobResult jobResult1 = invokeTaskHandlerResponse.JobResult;
      if (jobResult1.Result == TaskResult.Failed && !jobResult1.IsRetryable && taskInstance.ContinueOnError)
        jobResult1.Result = TaskResult.SucceededWithIssues;
      string taskInstanceDisplayName = string.IsNullOrWhiteSpace(taskInstance.DisplayName) ? taskInstance.Name : taskInstance.DisplayName;
      await this.ExecuteAsync(context, this.m_input, (Func<Task>) (() => this.Tracker.TaskCompleted(this.m_input.ScopeId, this.m_input.PlanId, this.m_input.JobId, taskInstance.InstanceId, context.CurrentUtcDateTime, invokeTaskHandlerResponse.JobResult, taskInstanceDisplayName)));
      context.TraceServerTaskCompleted(this.m_input.ScopeId, this.m_input.PlanId, this.m_input.JobId, taskInstance.InstanceId, invokeTaskHandlerResponse.JobResult.Result);
      JobResult jobResult2 = invokeTaskHandlerResponse.JobResult;
      taskTimeoutTimerTask = (Task<string>) null;
      taskExecutionTimerCancelTokenSource = (CancellationTokenSource) null;
      retryTaskToken = (CancellationTokenSource) null;
      return jobResult2;
    }

    private async Task<RunServerTask3.InvokeTaskHandlerResponse> InvokeTaskHandler(
      OrchestrationContext context,
      TaskInstance taskInstance,
      Task<string> taskTimeoutTimerTask,
      ServerTaskExecutionContext executionContext)
    {
      JobResult taskResult = new JobResult()
      {
        Result = TaskResult.Succeeded
      };
      bool taskAssigned = false;
      CancellationTokenSource eventsTimerCancel = (CancellationTokenSource) null;
      ServerTaskSectionExecutionOutput serverTaskSectionExecutionOutput = (ServerTaskSectionExecutionOutput) null;
      try
      {
        context.TraceServerTaskRequesting(this.m_input.ScopeId, this.m_input.PlanId, this.m_input.JobId, taskInstance.InstanceId);
        ExecuteTaskResponse executeTaskResponse = await this.ExecuteAsync<ExecuteTaskResponse>(context, this.m_input, (Func<Task<ExecuteTaskResponse>>) (() => this.ExecutionHandler.ExecuteTask(executionContext, this.m_input.ScopeId, this.m_input.PlanId, this.m_input.JobId, taskInstance.InstanceId)));
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
          RunServerTask3.WaitEventData waitEventDetails = this.GetWaitEventDetails(context, eventName);
          taskList.Add(waitEventDetails.WaitTask);
          RunServerTask3.DisposeCancellationToken(eventsTimerCancel);
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
            goto label_37;
          }
          else if (task == taskTimeoutTimerTask)
          {
            TimeSpan taskExecutionTimeoutValue = TimeSpan.FromMinutes((double) taskInstance.TimeoutInMinutes);
            eventsTimerCancel.Cancel();
            taskResult = await this.OnCancel(context, taskInstance, executionContext, TaskCanceledReasonType.Timeout, Resources.ServerTaskTimedOut((object) taskExecutionTimeoutValue.ToString()));
            taskResult.Message = taskResult.Result == TaskResult.Succeeded ? string.Empty : Resources.ServerTaskTimedOut((object) taskExecutionTimeoutValue.ToString());
            goto label_37;
          }
          else if (task == this.m_taskCanceled.Task)
          {
            eventsTimerCancel.Cancel();
            TaskCanceledReasonType cancelType = TaskCanceledReasonType.Other;
            if (this.m_taskCanceled.Task.Result.Reason.Equals("Timeout", StringComparison.OrdinalIgnoreCase))
              cancelType = TaskCanceledReasonType.Timeout;
            taskResult = await this.OnCancel(context, taskInstance, executionContext, cancelType, string.Empty);
            if (taskResult.Result == TaskResult.Succeeded)
            {
              taskResult.Message = string.Empty;
              goto label_37;
            }
            else
              goto label_37;
          }
          else if (task == eventsTimertask)
          {
            context.TraceTimedOutWaitingForEvent(this.m_input.ScopeId, this.m_input.PlanId, this.m_input.JobId, eventName, timeSpan.ToString());
            taskResult = await this.OnCancel(context, taskInstance, executionContext, TaskCanceledReasonType.Timeout, Resources.ServerTaskTimedOutWaitingForEvent((object) timeSpan.ToString(), (object) eventName));
            goto label_37;
          }
          else
          {
            if (task == this.m_localExecutionCompleted.Task)
            {
              eventsTimerCancel.Cancel();
              context.TraceServerTaskLocalExecutionCompletedEventReceived(this.m_input.ScopeId, this.m_input.PlanId, this.m_input.JobId, taskInstance.InstanceId);
              if (this.m_localExecutionCompleted.Task.Result is TaskLocalExecutionCompletedEvent result && result?.EventData != null)
                serverTaskSectionExecutionOutput = result.EventData;
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
              await this.NotifyTaskStarted(context, taskInstance.InstanceId);
            }
            eventName = (string) null;
            eventsTimertask = (Task<string>) null;
            timeSpan = new TimeSpan?();
          }
        }
        context.TraceServerTaskError(10015550, this.m_input.ScopeId, this.m_input.PlanId, this.m_input.JobId, taskInstance.InstanceId, "Wait queue is empty and orchestration is not terminated");
label_37:
        waitQueue = (Queue<string>) null;
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
        RunServerTask3.DisposeCancellationToken(eventsTimerCancel);
      }
      RunServerTask3.InvokeTaskHandlerResponse taskHandlerResponse = new RunServerTask3.InvokeTaskHandlerResponse()
      {
        JobResult = taskResult,
        IsCompleted = serverTaskSectionExecutionOutput == null || !serverTaskSectionExecutionOutput.IsCompleted.HasValue || serverTaskSectionExecutionOutput.IsCompleted.Value
      };
      taskResult = (JobResult) null;
      eventsTimerCancel = (CancellationTokenSource) null;
      serverTaskSectionExecutionOutput = (ServerTaskSectionExecutionOutput) null;
      return taskHandlerResponse;
    }

    private async Task NotifyTaskStarted(OrchestrationContext context, Guid taskInstanceId)
    {
      if (this.m_hasRaisedTaskStartedNotification)
        ;
      else
      {
        this.m_hasRaisedTaskStartedNotification = true;
        await this.ExecuteAsync(context, this.m_input, (Func<Task>) (() => this.Tracker.TaskStarted(this.m_input.ScopeId, this.m_input.PlanId, this.m_input.JobId, taskInstanceId, context.CurrentUtcDateTime)));
        context.TraceServerTaskStarted(this.m_input.ScopeId, this.m_input.PlanId, this.m_input.JobId, taskInstanceId);
        if (!this.m_input.NotifyTaskStarted)
          ;
        else
          await this.ExecuteAsync(context, this.m_input, (Func<Task>) (() => this.ControlExtension.NotifyTaskStarted(this.m_input.ScopeId, this.m_input.PlanId, this.m_input.JobId)));
      }
    }

    private void ResetEventsForNextInvocation()
    {
      this.m_localCancellationCompleted = new TaskCompletionSource<bool>();
      this.m_localExecutionCompleted = new TaskCompletionSource<object>();
      this.m_taskAssigned = new TaskCompletionSource<bool>();
      this.m_taskStarted = new TaskCompletionSource<bool>();
      this.m_taskCompleted = new TaskCompletionSource<TaskResult>();
    }

    private RunServerTask3.WaitEventData GetWaitEventDetails(
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
            this.m_localExecutionCompleted.TrySetResult((object) true);
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
      return new RunServerTask3.WaitEventData()
      {
        WaitTask = task,
        Timeout = nullable
      };
    }

    private async Task<JobResult> OnCancel(
      OrchestrationContext context,
      TaskInstance taskInstance,
      ServerTaskExecutionContext executionContext,
      TaskCanceledReasonType cancelType,
      string message)
    {
      JobResult result = new JobResult()
      {
        Result = TaskResult.Canceled
      };
      context.TraceServerJobCancellationSending(this.m_input.ScopeId, this.m_input.PlanId, this.m_input.JobId);
      CancelTaskResponse cancelTaskResponse = await this.ExecuteAsync<CancelTaskResponse>(context, this.m_input, (Func<Task<CancelTaskResponse>>) (() => this.ExecutionHandler.CancelTask(executionContext, this.m_input.ScopeId, this.m_input.PlanId, this.m_input.JobId, taskInstance.InstanceId, cancelType)));
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

    private async Task<TaskResult> CreateCancellableTimerTask(
      OrchestrationContext context,
      TimeSpan timespan,
      CancellationTokenSource cancellationTokenSource)
    {
      Task<TaskResult> timerTask = context.CreateTimer<TaskResult>(context.CurrentUtcDateTime.Add(timespan), TaskResult.Succeeded, cancellationTokenSource.Token);
      TaskResult cancellableTimerTask = await Task.WhenAny((Task) this.m_taskCanceled.Task, (Task) timerTask) != this.m_taskCanceled.Task ? timerTask.Result : TaskResult.Canceled;
      timerTask = (Task<TaskResult>) null;
      return cancellableTimerTask;
    }

    private void EnsureExtensions(OrchestrationContext context)
    {
      this.ExecutionHandler = context.CreateClient<IServerTaskHandlerExtension2>(true);
      this.Tracker = context.CreateClient<IServerTaskTrackingExtension>(true);
      this.ControlExtension = context.CreateClient<IServerExecutionControlExtension>(true);
    }

    private struct WaitEventData
    {
      internal Task WaitTask { get; set; }

      internal TimeSpan? Timeout { get; set; }
    }

    private struct InvokeTaskHandlerResponse
    {
      internal JobResult JobResult { get; set; }

      internal bool IsCompleted { get; set; }
    }
  }
}
