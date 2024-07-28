// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.Tasks.RunServerTask
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: ACF674F2-B05D-403A-A061-F4792BD3317C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks.dll

using Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.VisualStudio.Services.Orchestration;
using Microsoft.VisualStudio.Services.Orchestration.TaskActivityDispatcherSharding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines.Tasks
{
  public class RunServerTask : 
    TaskOrchestration<TaskExecutionState, RunServerTaskInput, object, TaskExecutionState>
  {
    private Guid m_jobId;
    private Guid m_taskId;
    private RunServerTaskInput m_input;
    private IPipelineIdGenerator m_idGenerator;
    private TaskExecutionState m_executionState;
    private bool m_deliveryFailed;
    private TaskCompletionSource<bool> m_taskStarted = new TaskCompletionSource<bool>();
    private TaskCompletionSource<bool> m_taskAssigned = new TaskCompletionSource<bool>();
    private TaskCompletionSource<TaskResult> m_taskCompleted = new TaskCompletionSource<TaskResult>();
    private TaskCompletionSource<CanceledEvent> m_taskCanceled = new TaskCompletionSource<CanceledEvent>();
    private const string c_teamName = "ServerTasks";
    private static readonly string[] s_eventNames = new string[3]
    {
      "TaskAssigned",
      "TaskStarted",
      "TaskCompleted"
    };

    public IServerJobLogger JobLogger { get; private set; }

    public IServerJobManager JobManager { get; private set; }

    public Func<Exception, bool> RetryException { get; set; }

    public override TaskExecutionState OnGetStatus() => this.m_executionState;

    public override void OnEvent(OrchestrationContext context, string name, object input)
    {
      context.TraceEvent(name);
      switch (name)
      {
        case "TaskAssigned":
          this.m_taskAssigned.TrySetResult(true);
          break;
        case "TaskStarted":
          this.m_taskStarted.TrySetResult(true);
          break;
        case "TaskCompleted":
          TaskCompletedEvent taskCompletedEvent = (TaskCompletedEvent) input;
          this.m_taskCompleted.TrySetResult(taskCompletedEvent.Result);
          this.m_deliveryFailed = taskCompletedEvent.DeliveryFailed;
          break;
        case "TaskCanceled":
          this.m_taskCanceled.TrySetResult((CanceledEvent) input);
          break;
      }
    }

    public override async Task<TaskExecutionState> RunTask(
      OrchestrationContext context,
      RunServerTaskInput input)
    {
      this.EnsureExtensions(context, input.ActivityDispatcherShardsCount, (IActivityShardKey) input.ShardKey);
      context.TraceStartLinearOrchestration();
      this.m_input = input;
      this.m_executionState = input.Task;
      this.m_executionState.StartTime = new DateTime?(context.CurrentUtcDateTime);
      this.m_executionState.State = PipelineState.InProgress;
      this.m_idGenerator = (IPipelineIdGenerator) new PipelineIdGenerator(input.PlanVersion < 4);
      this.m_jobId = this.m_idGenerator.GetJobInstanceId(this.m_input.StageName, this.m_input.PhaseName, this.m_input.JobName, this.m_input.JobAttempt, this.m_input.CheckRerunAttempt);
      this.m_taskId = this.m_idGenerator.GetTaskInstanceId(this.m_input.StageName, this.m_input.PhaseName, this.m_input.JobName, this.m_input.JobAttempt, this.m_input.Task.Name, this.m_input.CheckRerunAttempt);
      Task eventTimeoutTask = (Task) null;
      Task<string> cancelTimeoutTask = (Task<string>) null;
      Task<string> executeTimeoutTask = (Task<string>) null;
      CancellationTokenSource eventTimeoutToken = (CancellationTokenSource) null;
      CancellationTokenSource cancelTimeoutToken = (CancellationTokenSource) null;
      CancellationTokenSource executeTimeoutToken = (CancellationTokenSource) null;
      try
      {
        TaskParameters parameters = this.CreateTaskParameters();
        context.TraceStartLinearPhase("ServerTasks", "SendTask");
        context.TraceInfo("Sending task request");
        ExecuteTaskResponse executeTaskResponse = await this.ExecuteAsync<ExecuteTaskResponse>(context, this.m_input, (Func<Task<ExecuteTaskResponse>>) (() => this.JobManager.ExecuteTask(this.m_input.ScopeId, this.m_input.PlanId, parameters)));
        context.TraceInfo("Successfully sent the task request");
        List<Task> waitingTasks = new List<Task>(5);
        Queue<RunServerTask.WaitEventData> eventQueue = this.GetEventQueue(context, (IDictionary<string, TaskEventConfig>) executeTaskResponse.TaskEvents.All);
        RunServerTask.WaitEventData currentEvent = new RunServerTask.WaitEventData();
        this.SetupNextEvent(context, eventQueue, waitingTasks, ref currentEvent, ref eventTimeoutTask, ref eventTimeoutToken);
        TimeSpan timeSpan1;
        DateTime currentUtcDateTime;
        if (this.m_input.Task.TimeoutInMinutes > 0)
        {
          double timeoutInMinutes = (double) this.m_input.Task.TimeoutInMinutes;
          timeSpan1 = TimeSpan.FromSeconds((double) int.MaxValue);
          double totalMinutes = timeSpan1.TotalMinutes;
          if (timeoutInMinutes < totalMinutes)
          {
            executeTimeoutToken = new CancellationTokenSource();
            OrchestrationContext orchestrationContext = context;
            currentUtcDateTime = context.CurrentUtcDateTime;
            DateTime fireAt = currentUtcDateTime.AddMinutes((double) this.m_input.Task.TimeoutInMinutes);
            CancellationToken token = executeTimeoutToken.Token;
            executeTimeoutTask = orchestrationContext.CreateTimer<string>(fireAt, (string) null, token);
            waitingTasks.Add((Task) executeTimeoutTask);
            context.TraceInfo(string.Format("Server task has {0} minutes timeout.", (object) this.m_input.Task.TimeoutInMinutes));
          }
        }
        waitingTasks.Add((Task) this.m_taskCanceled.Task);
        waitingTasks.Add((Task) this.m_taskCompleted.Task);
        while (waitingTasks.Count > 0)
        {
          Task task = await Task.WhenAny((IEnumerable<Task>) waitingTasks);
          if (task == this.m_taskCompleted.Task)
          {
            waitingTasks.Clear();
            if (!this.m_taskStarted.Task.IsCompleted)
              context.TraceError(10015550, string.Format("Server task received job completed event with result {0} without receiving the assigned event", (object) this.m_taskCompleted.Task.Result));
            if (!this.m_executionState.Result.HasValue)
              this.m_executionState.Result = new TaskResult?(this.m_taskCompleted.Task.Result);
            this.m_executionState.DeliveryFailed = this.m_deliveryFailed;
            break;
          }
          if (task == this.m_taskAssigned.Task)
            this.SetupNextEvent(context, eventQueue, waitingTasks, ref currentEvent, ref eventTimeoutTask, ref eventTimeoutToken);
          else if (task == this.m_taskStarted.Task)
          {
            await this.ExecuteAsync(context, this.m_input, closure_9 ?? (closure_9 = (Func<Task>) (() => this.JobLogger.TaskStarted(this.m_input.ScopeId, this.m_input.PlanId, this.m_jobId, this.m_taskId, context.CurrentUtcDateTime))));
            context.TraceInfo("Received task started notification");
            this.SetupNextEvent(context, eventQueue, waitingTasks, ref currentEvent, ref eventTimeoutTask, ref eventTimeoutToken);
          }
          else if (task == this.m_taskCanceled.Task)
          {
            eventTimeoutToken?.Cancel();
            executeTimeoutToken?.Cancel();
            waitingTasks.Remove(eventTimeoutTask);
            waitingTasks.Remove((Task) executeTimeoutTask);
            waitingTasks.Remove((Task) this.m_taskCanceled.Task);
            CanceledEvent result = this.m_taskCanceled.Task.Result;
            if (result.Timeout > TimeSpan.Zero && result.Timeout < TimeSpan.MaxValue)
            {
              cancelTimeoutToken = new CancellationTokenSource();
              OrchestrationContext orchestrationContext = context;
              currentUtcDateTime = context.CurrentUtcDateTime;
              DateTime fireAt = currentUtcDateTime.Add(result.Timeout);
              CancellationToken token = cancelTimeoutToken.Token;
              cancelTimeoutTask = orchestrationContext.CreateTimer<string>(fireAt, (string) null, token);
              waitingTasks.Add((Task) cancelTimeoutTask);
              OrchestrationContext context1 = context;
              timeSpan1 = result.Timeout;
              string message = string.Format("Server task needs to finish within {0} minutes on cancellation.", (object) timeSpan1.TotalMinutes);
              context1.TraceInfo(message);
            }
            if (!this.m_executionState.Result.HasValue)
              this.m_executionState.Result = new TaskResult?(TaskResult.Canceled);
            int num = (int) await this.OnCancel(context, TaskCanceledReasonType.Other, result.Reason);
          }
          else if (task == cancelTimeoutTask)
          {
            waitingTasks.Remove((Task) cancelTimeoutTask);
            context.TraceInfo("Server task exceeded cancellation timeout.");
            this.m_taskCompleted.TrySetResult(this.m_executionState.Result.Value);
          }
          else if (task == eventTimeoutTask)
          {
            waitingTasks.Remove(eventTimeoutTask);
            context.TraceInfo(string.Format("Server task timed out after {0} while waiting for event {1}", (object) currentEvent.Timeout, (object) currentEvent.Name));
            this.m_executionState.Result = new TaskResult?(TaskResult.Failed);
            this.m_taskCanceled.TrySetResult(new CanceledEvent()
            {
              Reason = Resources.ServerTaskTimedOutWaitingForEvent((object) currentEvent.Timeout, (object) currentEvent.Name),
              Timeout = TimeSpan.FromSeconds(15.0)
            });
          }
          else if (task == executeTimeoutTask)
          {
            waitingTasks.Remove((Task) executeTimeoutTask);
            if (this.m_input.PlanVersion >= 14)
            {
              TimeSpan timeSpan2 = TimeSpan.FromMinutes((double) this.m_input.Task.TimeoutInMinutes);
              TaskExecutionState taskExecutionState = this.m_executionState;
              taskExecutionState.Result = new TaskResult?(await this.OnCancel(context, TaskCanceledReasonType.Timeout, Resources.ServerTaskTimedOut((object) timeSpan2.ToString())));
              taskExecutionState = (TaskExecutionState) null;
              if (!this.m_executionState.Result.Equals((object) TaskResult.Succeeded))
              {
                this.m_executionState.Result = new TaskResult?(TaskResult.Failed);
                this.m_taskCanceled.TrySetResult(new CanceledEvent()
                {
                  Reason = Resources.ServerTaskTimedOut((object) this.m_input.Task.TimeoutInMinutes),
                  Timeout = TimeSpan.FromSeconds(15.0)
                });
              }
            }
            else
            {
              this.m_executionState.Result = new TaskResult?(TaskResult.Failed);
              this.m_taskCanceled.TrySetResult(new CanceledEvent()
              {
                Reason = Resources.ServerTaskTimedOut((object) this.m_input.Task.TimeoutInMinutes),
                Timeout = TimeSpan.FromSeconds(15.0)
              });
            }
          }
        }
        waitingTasks = (List<Task>) null;
        eventQueue = (Queue<RunServerTask.WaitEventData>) null;
        currentEvent = new RunServerTask.WaitEventData();
      }
      catch (TaskFailedException ex)
      {
        context.TraceException((Exception) ex);
        this.m_executionState.Result = new TaskResult?(TaskResult.Failed);
        this.m_executionState.Error = new TaskError();
        this.m_executionState.Error.Message = !(ex.InnerException is AggregateException) ? ex.Message : (ex.InnerException as AggregateException).Flatten().InnerExceptions.Aggregate<Exception, string>(string.Empty, (Func<string, Exception, string>) ((current, e) => current + e.Message + " "));
        await this.ExecuteAsync(context, input, (Func<Task>) (() => this.JobLogger.LogIssue(input.ScopeId, input.PlanId, this.m_taskId, context.CurrentUtcDateTime, IssueType.Error, this.m_executionState.Error.Message)));
      }
      finally
      {
        eventTimeoutToken?.Cancel();
        eventTimeoutToken?.Dispose();
        executeTimeoutToken?.Cancel();
        executeTimeoutToken?.Dispose();
        cancelTimeoutToken?.Cancel();
        cancelTimeoutToken?.Dispose();
      }
      this.m_executionState.State = PipelineState.Completed;
      TaskResult? result1 = this.m_executionState.Result;
      if (!result1.HasValue)
      {
        this.m_executionState.Result = new TaskResult?(TaskResult.Succeeded);
      }
      else
      {
        result1 = this.m_executionState.Result;
        TaskResult taskResult = TaskResult.Failed;
        if (result1.GetValueOrDefault() == taskResult & result1.HasValue && this.m_executionState.ContinueOnError)
          this.m_executionState.Result = new TaskResult?(TaskResult.SucceededWithIssues);
      }
      await this.ExecuteAsync(context, this.m_input, (Func<Task>) (() => this.JobLogger.TaskCompleted(this.m_input.ScopeId, this.m_input.PlanId, this.m_jobId, this.m_taskId, context.CurrentUtcDateTime, this.m_executionState.Result.Value)));
      if (this.m_executionState.Error == null)
      {
        context.TraceCompleteLinearOrchestration("ServerTasks", "TaskCompleted");
      }
      else
      {
        OrchestrationContext context2 = context;
        result1 = this.m_executionState.Result;
        string errorCode = result1.ToString();
        string message = this.m_executionState.Error.Message;
        context2.TraceCompleteLinearOrchestrationWithError("ServerTasks", "TaskCompleted", errorCode, message, false);
      }
      TaskExecutionState executionState = this.m_executionState;
      eventTimeoutTask = (Task) null;
      cancelTimeoutTask = (Task<string>) null;
      executeTimeoutTask = (Task<string>) null;
      eventTimeoutToken = (CancellationTokenSource) null;
      cancelTimeoutToken = (CancellationTokenSource) null;
      executeTimeoutToken = (CancellationTokenSource) null;
      return executionState;
    }

    private TaskParameters CreateTaskParameters() => new TaskParameters()
    {
      Name = this.m_input.Task.Name,
      JobName = this.m_input.JobName,
      JobAttempt = this.m_input.JobAttempt,
      StageName = this.m_input.StageName,
      StageAttempt = this.m_input.StageAttempt,
      PhaseName = this.m_input.PhaseName,
      PhaseAttempt = this.m_input.PhaseAttempt,
      CheckRerunAttempt = this.m_input.CheckRerunAttempt
    };

    private void SetupNextEvent(
      OrchestrationContext context,
      Queue<RunServerTask.WaitEventData> eventQueue,
      List<Task> waitingTasks,
      ref RunServerTask.WaitEventData currentEvent,
      ref Task eventTimeoutTask,
      ref CancellationTokenSource eventTimeoutToken)
    {
      eventTimeoutToken?.Cancel();
      if (eventTimeoutTask != null)
        waitingTasks.Remove(eventTimeoutTask);
      if (currentEvent.Task != null)
        waitingTasks.Remove(currentEvent.Task);
      currentEvent = eventQueue.Dequeue();
      waitingTasks.Insert(0, currentEvent.Task);
      if (currentEvent.Timeout.HasValue)
      {
        context.TraceInfo("Server task is waiting for event " + currentEvent.Name + " with timeout of " + currentEvent.Timeout.ToString());
        eventTimeoutToken = new CancellationTokenSource();
        eventTimeoutTask = (Task) context.CreateTimer<string>(context.CurrentUtcDateTime.Add(currentEvent.Timeout.Value), (string) null, eventTimeoutToken.Token);
        waitingTasks.Add(eventTimeoutTask);
      }
      else
      {
        Action setResult = currentEvent.SetResult;
        if (setResult != null)
          setResult();
      }
      context.TraceStartLinearPhase("ServerTasks", "WaitForEvent" + currentEvent.Name);
    }

    private Task ExecuteAsync(
      OrchestrationContext context,
      RunServerTaskInput input,
      Func<Task> operation,
      int maxAttempts = 5)
    {
      return context.ExecuteAsync(operation, maxAttempts, canRetry: this.RetryException, traceException: (Action<Exception>) (ex => context.TraceException(ex)));
    }

    private Task<T> ExecuteAsync<T>(
      OrchestrationContext context,
      RunServerTaskInput input,
      Func<Task<T>> operation,
      int maxAttempts = 5)
    {
      return context.ExecuteAsync<T>(operation, maxAttempts, canRetry: this.RetryException, traceException: (Action<Exception>) (ex => context.TraceException(ex)));
    }

    private void EnsureExtensions(
      OrchestrationContext context,
      int activityDispatcherShardsCount,
      IActivityShardKey shardKey)
    {
      this.JobManager = context.CreateShardedClient<IServerJobManager>(true, activityDispatcherShardsCount, shardKey, "Server");
      this.JobLogger = context.CreateShardedClient<IServerJobLogger>(true, activityDispatcherShardsCount, shardKey, "Server");
    }

    private Queue<RunServerTask.WaitEventData> GetEventQueue(
      OrchestrationContext context,
      IDictionary<string, TaskEventConfig> eventConfiguration)
    {
      Queue<RunServerTask.WaitEventData> eventQueue = new Queue<RunServerTask.WaitEventData>(RunServerTask.s_eventNames.Length);
      foreach (string eventName in RunServerTask.s_eventNames)
      {
        RunServerTask.WaitEventData waitEventData = new RunServerTask.WaitEventData()
        {
          Name = eventName
        };
        TaskEventConfig taskEventConfig;
        bool flag = eventConfiguration.TryGetValue(eventName, out taskEventConfig) && taskEventConfig.IsEnabled();
        if (flag)
        {
          TimeSpan result;
          if (!TimeSpan.TryParse(taskEventConfig.Timeout, out result))
            context.TraceError(10015550, "Server task unable to parse timeout " + taskEventConfig.Timeout + " for event " + eventName);
          else if (result > TimeSpan.Zero && result < TimeSpan.FromMilliseconds((double) int.MaxValue))
            waitEventData.Timeout = new TimeSpan?(result);
        }
        switch (eventName)
        {
          case "TaskAssigned":
            if (!flag)
              waitEventData.SetResult = (Action) (() => this.m_taskAssigned.TrySetResult(true));
            waitEventData.Task = (Task) this.m_taskAssigned.Task;
            break;
          case "TaskStarted":
            if (!flag)
              waitEventData.SetResult = (Action) (() => this.m_taskStarted.TrySetResult(true));
            waitEventData.Task = (Task) this.m_taskStarted.Task;
            break;
          case "TaskCompleted":
            if (!flag)
              waitEventData.SetResult = (Action) (() => this.m_taskCompleted.TrySetResult(TaskResult.Succeeded));
            waitEventData.Task = (Task) this.m_taskCompleted.Task;
            break;
          default:
            continue;
        }
        OrchestrationContext context1 = context;
        string name = waitEventData.Name;
        // ISSUE: variable of a boxed type
        __Boxed<bool> local1 = (ValueType) flag;
        TimeSpan? timeout = waitEventData.Timeout;
        ref TimeSpan? local2 = ref timeout;
        string str = (local2.HasValue ? local2.GetValueOrDefault().ToString() : (string) null) ?? "Nil";
        string message = string.Format("Event config : Name '{0}', isConfigured {1}, Timeout {2}", (object) name, (object) local1, (object) str);
        context1.TraceInfo(message);
        eventQueue.Enqueue(waitEventData);
      }
      return eventQueue;
    }

    private async Task<TaskResult> OnCancel(
      OrchestrationContext context,
      TaskCanceledReasonType cancelType,
      string message)
    {
      this.m_executionState.State = PipelineState.Canceling;
      context.TraceInfo("Sending job cancellation request");
      TaskParameters taskParameters = this.CreateTaskParameters();
      CancelTaskResponse cancelTaskResponse = await this.ExecuteAsync<CancelTaskResponse>(context, this.m_input, (Func<Task<CancelTaskResponse>>) (() => this.JobManager.CancelTask(this.m_input.ScopeId, this.m_input.PlanId, taskParameters, cancelType)));
      context.TraceInfo("Successfully sent job cancellation request");
      if (this.m_input.PlanVersion < 14 || !this.m_taskCompleted.Task.IsCompleted)
        return this.m_input.PlanVersion < 14 || cancelType != TaskCanceledReasonType.Timeout ? TaskResult.Canceled : TaskResult.Failed;
      this.m_executionState.State = PipelineState.InProgress;
      return this.m_taskCompleted.Task.Result;
    }

    private struct WaitEventData
    {
      public string Name { get; set; }

      public Task Task { get; set; }

      public TimeSpan? Timeout { get; set; }

      public Action SetResult { get; set; }
    }
  }
}
