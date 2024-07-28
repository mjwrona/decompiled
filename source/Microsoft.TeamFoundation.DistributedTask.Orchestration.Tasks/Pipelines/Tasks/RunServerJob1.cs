// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.Tasks.RunServerJob1
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
  internal sealed class RunServerJob1 : 
    TaskOrchestration<JobExecutionState, RunServerJobInput, object, JobExecutionState>
  {
    private Guid m_jobRecordId;
    private string m_jobIdentifier;
    private RunServerJobInput m_input;
    private JobExecutionState m_executionState;
    private Dictionary<string, TaskEventConfig> m_eventConfiguration;
    private TaskCompletionSource<CanceledEvent> m_jobCanceled = new TaskCompletionSource<CanceledEvent>();
    private TaskCompletionSource<bool> m_localCancellationCompleted = new TaskCompletionSource<bool>();
    private TaskCompletionSource<bool> m_localExecutionCompleted = new TaskCompletionSource<bool>();
    private TaskCompletionSource<bool> m_taskAssigned = new TaskCompletionSource<bool>();
    private TaskCompletionSource<bool> m_taskStarted = new TaskCompletionSource<bool>();
    private TaskCompletionSource<TaskResult> m_taskCompleted = new TaskCompletionSource<TaskResult>();

    public IServerJobLogger JobLogger { get; private set; }

    public IServerJobManager JobManager { get; private set; }

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

    public override JobExecutionState OnGetStatus() => this.m_executionState;

    public override async Task<JobExecutionState> RunTask(
      OrchestrationContext context,
      RunServerJobInput input)
    {
      this.EnsureExtensions(context, input.ActivityDispatcherShardsCount, (IActivityShardKey) input.ShardKey);
      this.m_input = input;
      this.m_executionState = input.Job;
      this.m_executionState.State = PipelineState.InProgress;
      this.m_executionState.StartTime = new DateTime?(context.CurrentUtcDateTime);
      this.m_jobIdentifier = PipelineUtilities.GetJobInstanceName(input.StageName, input.PhaseName, input.Job.Name);
      this.m_jobRecordId = TimelineRecordIdGenerator.GetId(this.m_jobIdentifier);
      JobParameters parameters = new JobParameters()
      {
        StageName = this.m_input.StageName,
        PhaseName = this.m_input.PhaseName,
        Name = this.m_input.Job.Name
      };
      IList<ServerTaskReference> source = await this.ExecuteAsync<IList<ServerTaskReference>>(context, this.m_input, (Func<Task<IList<ServerTaskReference>>>) (() => this.JobManager.GetTasks(this.m_input.ScopeId, this.m_input.PlanId, parameters)));
      if (source.Count != 1)
      {
        await this.ExecuteAsync(context, this.m_input, (Func<Task>) (() => this.JobLogger.LogIssue(this.m_input.ScopeId, this.m_input.PlanId, this.m_jobRecordId, context.CurrentUtcDateTime, IssueType.Error, Resources.InvalidServerJob())));
        this.m_executionState.State = PipelineState.Completed;
        this.m_executionState.Result = new TaskResult?(TaskResult.Failed);
        this.m_executionState.FinishTime = new DateTime?(context.CurrentUtcDateTime);
        return this.m_executionState;
      }
      ServerTaskReference task = source.Single<ServerTaskReference>();
      if (task.Enabled)
      {
        JobExecutionState jobExecutionState = this.m_executionState;
        jobExecutionState.Result = new TaskResult?(await this.RunServerTask(context, task));
        jobExecutionState = (JobExecutionState) null;
      }
      else
        this.m_executionState.Result = new TaskResult?(TaskResult.Succeeded);
      this.m_executionState.State = PipelineState.Completed;
      this.m_executionState.FinishTime = new DateTime?(context.CurrentUtcDateTime);
      context.TraceServerJobCompleted(this.m_input.ScopeId, this.m_input.PlanId, this.m_jobIdentifier, this.m_executionState.Result.Value);
      await this.ExecuteAsync(context, this.m_input, (Func<Task>) (() => this.JobLogger.JobCompleted(this.m_input.ScopeId, this.m_input.PlanId, this.m_jobRecordId, context.CurrentUtcDateTime, this.m_executionState.Result.Value)));
      return this.m_executionState;
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

    private async Task<TaskResult> RunServerTask(
      OrchestrationContext context,
      ServerTaskReference task)
    {
      TaskResult executionResult = TaskResult.Succeeded;
      bool taskAssigned = false;
      Task<string> taskExecutionTimerTask = (Task<string>) null;
      Task<string> JobExecutionTimerTask = (Task<string>) null;
      CancellationTokenSource taskExecutionTimerCancelTokenSource = new CancellationTokenSource();
      CancellationTokenSource jobExecutionTimerCancelTokenSource = new CancellationTokenSource();
      try
      {
        context.TraceServerJobRequesting(this.m_input.ScopeId, this.m_input.PlanId, this.m_jobIdentifier);
        TaskParameters parameters = new TaskParameters()
        {
          Name = task.Name,
          JobName = this.m_input.Job.Name,
          PhaseName = this.m_input.PhaseName
        };
        ExecuteTaskResponse executeTaskResponse = await this.ExecuteAsync<ExecuteTaskResponse>(context, this.m_input, (Func<Task<ExecuteTaskResponse>>) (() => this.JobManager.ExecuteTask(this.m_input.ScopeId, this.m_input.PlanId, parameters)));
        context.TraceServerJobRequestSent(this.m_input.ScopeId, this.m_input.PlanId, this.m_jobIdentifier);
        this.m_eventConfiguration = executeTaskResponse.TaskEvents.All;
        Queue<string> waitQueue = new Queue<string>();
        if (executeTaskResponse.WaitForLocalExecutionComplete)
        {
          waitQueue.Enqueue("LocalExecutionCompleted");
          this.m_eventConfiguration["LocalExecutionCompleted"] = new TaskEventConfig(TimeSpan.FromMilliseconds((double) int.MaxValue).ToString());
        }
        waitQueue.Enqueue("TaskAssigned");
        waitQueue.Enqueue("TaskStarted");
        waitQueue.Enqueue("TaskCompleted");
        TimeSpan jobExecutionTimeoutValue = TimeSpan.Zero;
        if (this.m_input.Job.TimeoutInMinutes > 0)
        {
          jobExecutionTimeoutValue = TimeSpan.FromMinutes((double) this.m_input.Job.TimeoutInMinutes);
          JobExecutionTimerTask = this.CreateTimeoutTask(context, jobExecutionTimeoutValue, jobExecutionTimerCancelTokenSource);
        }
        TimeSpan taskExecutionTimeoutValue = TimeSpan.FromMinutes((double) task.TimeoutInMinutes);
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
          RunServerJob1.WaitEventData waitEventDetails = this.GetWaitEventDetails(context, eventName);
          taskList.Add(waitEventDetails.WaitTask);
          CancellationTokenSource eventsTimerCancel = new CancellationTokenSource();
          Task<string> eventsTimertask = (Task<string>) null;
          TimeSpan? timeSpan = waitEventDetails.Timeout;
          if (timeSpan.HasValue)
          {
            eventsTimertask = context.CreateTimer<string>(context.CurrentUtcDateTime.Add(timeSpan.Value), (string) null, eventsTimerCancel.Token);
            taskList.Add((Task) eventsTimertask);
          }
          context.TraceServerJobWaiting(this.m_input.ScopeId, this.m_input.PlanId, this.m_jobIdentifier, eventName, timeSpan.HasValue ? timeSpan.ToString() : "Nil");
          Task task1 = await Task.WhenAny((IEnumerable<Task>) taskList);
          if (task1 == this.m_taskCompleted.Task)
          {
            if (!taskAssigned)
              context.TraceJobError(10015550, this.m_input.ScopeId, this.m_input.PlanId, this.m_jobIdentifier, "Received job completed event with result {0} without receiving the assigned event", (object) executionResult);
            eventsTimerCancel.Cancel();
            executionResult = this.m_taskCompleted.Task.Result;
            goto label_40;
          }
          else if (task1 == this.m_jobCanceled.Task)
          {
            eventsTimerCancel.Cancel();
            CanceledEvent result = this.m_jobCanceled.Task.Result;
            executionResult = await this.OnCancel(context, task, TaskCanceledReasonType.Other, result.Reason);
            goto label_40;
          }
          else if (task1 == JobExecutionTimerTask)
          {
            eventsTimerCancel.Cancel();
            executionResult = await this.OnCancel(context, task, TaskCanceledReasonType.Timeout, Resources.ServerJobTimedOut((object) jobExecutionTimeoutValue.ToString()));
            goto label_40;
          }
          else if (task1 == taskExecutionTimerTask)
          {
            eventsTimerCancel.Cancel();
            executionResult = await this.OnCancel(context, task, TaskCanceledReasonType.Timeout, Resources.ServerTaskTimedOut((object) taskExecutionTimeoutValue.ToString()));
            goto label_40;
          }
          else if (task1 == eventsTimertask)
          {
            context.TraceTimedOutWaitingForEvent(this.m_input.ScopeId, this.m_input.PlanId, this.m_jobIdentifier, eventName, timeSpan.ToString());
            executionResult = await this.OnCancel(context, task, TaskCanceledReasonType.Timeout, Resources.JobTimedOutWaitingForEvent((object) timeSpan.ToString(), (object) eventName));
            goto label_40;
          }
          else
          {
            if (task1 == this.m_localExecutionCompleted.Task)
            {
              eventsTimerCancel.Cancel();
              context.TraceServerJobLocalExecutionCompletedEventReceived(this.m_input.ScopeId, this.m_input.PlanId, this.m_jobIdentifier);
            }
            else if (task1 == this.m_taskAssigned.Task)
            {
              eventsTimerCancel.Cancel();
              taskAssigned = true;
              context.TraceServerJobAssigned(this.m_input.ScopeId, this.m_input.PlanId, this.m_jobIdentifier);
            }
            else if (task1 == this.m_taskStarted.Task)
            {
              eventsTimerCancel.Cancel();
              await this.ExecuteAsync(context, this.m_input, closure_6 ?? (closure_6 = (Func<Task>) (() => this.JobLogger.TaskStarted(this.m_input.ScopeId, this.m_input.PlanId, this.m_jobRecordId, task.Id, context.CurrentUtcDateTime))));
              context.TraceServerJobStarted(this.m_input.ScopeId, this.m_input.PlanId, this.m_jobIdentifier);
            }
            eventName = (string) null;
            eventsTimerCancel = (CancellationTokenSource) null;
            eventsTimertask = (Task<string>) null;
            timeSpan = new TimeSpan?();
          }
        }
        context.TraceJobError(10015551, this.m_input.ScopeId, this.m_input.PlanId, this.m_jobIdentifier, "Wait queue is empty and orchestration is not terminated");
label_40:
        waitQueue = (Queue<string>) null;
        jobExecutionTimeoutValue = new TimeSpan();
        taskExecutionTimeoutValue = new TimeSpan();
      }
      catch (TaskFailedException ex)
      {
        context.TraceJobException(this.m_input.ScopeId, this.m_input.PlanId, this.m_jobIdentifier, (Exception) ex);
        this.m_executionState.Result = new TaskResult?(TaskResult.Failed);
        this.m_executionState.Error = new JobError()
        {
          CanRetry = !taskAssigned && ex.InnerException is ServerJobFailureException
        };
        this.m_executionState.Error.Message = !(ex.InnerException is AggregateException) ? ex.Message : (ex.InnerException as AggregateException).Flatten().InnerExceptions.Aggregate<Exception, string>(string.Empty, (Func<string, Exception, string>) ((current, e) => current + e.Message + " "));
      }
      finally
      {
        this.ReleaseCancellationToken(taskExecutionTimerCancelTokenSource);
        this.ReleaseCancellationToken(jobExecutionTimerCancelTokenSource);
      }
      if (!this.m_executionState.Result.HasValue)
        this.m_executionState.Result = new TaskResult?(executionResult);
      JobError error = this.m_executionState.Error;
      bool flag = error != null && error.CanRetry;
      TaskResult? result1 = this.m_executionState.Result;
      TaskResult taskResult1 = TaskResult.Failed;
      if (result1.GetValueOrDefault() == taskResult1 & result1.HasValue && !flag && task.ContinueOnError)
        this.m_executionState.Result = new TaskResult?(executionResult = TaskResult.SucceededWithIssues);
      await this.ExecuteAsync(context, this.m_input, (Func<Task>) (() => this.JobLogger.TaskCompleted(this.m_input.ScopeId, this.m_input.PlanId, this.m_jobRecordId, task.Id, context.CurrentUtcDateTime, this.m_executionState.Result.Value)));
      TaskResult taskResult2 = executionResult;
      taskExecutionTimerTask = (Task<string>) null;
      JobExecutionTimerTask = (Task<string>) null;
      taskExecutionTimerCancelTokenSource = (CancellationTokenSource) null;
      jobExecutionTimerCancelTokenSource = (CancellationTokenSource) null;
      return taskResult2;
    }

    private void ReleaseCancellationToken(CancellationTokenSource cancellationTokenSource)
    {
      if (cancellationTokenSource == null)
        return;
      cancellationTokenSource.Cancel();
      cancellationTokenSource = (CancellationTokenSource) null;
    }

    private RunServerJob1.WaitEventData GetWaitEventDetails(
      OrchestrationContext context,
      string eventName)
    {
      TimeSpan? nullable = new TimeSpan?();
      TaskEventConfig taskEventConfig = (TaskEventConfig) null;
      bool isConfigured = this.m_eventConfiguration.TryGetValue(eventName, out taskEventConfig) && taskEventConfig.IsEnabled();
      if (isConfigured)
      {
        TimeSpan result;
        if (!TimeSpan.TryParse(taskEventConfig.Timeout, out result))
          context.TraceJobError(10015550, this.m_input.ScopeId, this.m_input.PlanId, this.m_jobIdentifier, "Unable to parse timeout: {0} for event: {1}", (object) taskEventConfig.Timeout, (object) eventName);
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
      context.TraceEventConfig(this.m_input.ScopeId, this.m_input.PlanId, this.m_jobIdentifier, eventName, nullable.HasValue ? nullable.ToString() : "Nil", isConfigured);
      return new RunServerJob1.WaitEventData()
      {
        WaitTask = task,
        Timeout = nullable
      };
    }

    private async Task<TaskResult> OnCancel(
      OrchestrationContext context,
      ServerTaskReference task,
      TaskCanceledReasonType cancelType,
      string message)
    {
      TaskResult result = TaskResult.Canceled;
      this.m_executionState.State = PipelineState.Canceling;
      TaskParameters taskParameters = new TaskParameters()
      {
        JobName = this.m_input.Job.Name,
        PhaseName = this.m_input.PhaseName,
        Name = task.Name
      };
      context.TraceServerJobCancellationSending(this.m_input.ScopeId, this.m_input.PlanId, this.m_jobIdentifier);
      CancelTaskResponse cancelTaskResponse = await this.ExecuteAsync<CancelTaskResponse>(context, this.m_input, (Func<Task<CancelTaskResponse>>) (() => this.JobManager.CancelTask(this.m_input.ScopeId, this.m_input.PlanId, taskParameters, cancelType)));
      context.TraceServerJobCancellationSent(this.m_input.ScopeId, this.m_input.PlanId, this.m_jobIdentifier);
      if (this.m_taskCompleted.Task.IsCompleted)
      {
        result = this.m_taskCompleted.Task.Result;
        return result;
      }
      if (cancelType == TaskCanceledReasonType.Timeout)
        result = TaskResult.Failed;
      if (cancelTaskResponse.WaitForLocalCancellationComplete)
      {
        context.TraceServerJobWaiting(this.m_input.ScopeId, this.m_input.PlanId, this.m_jobIdentifier, "LocalCancellationCompleted", "Nil");
        int num = await this.m_localCancellationCompleted.Task ? 1 : 0;
        context.TraceServerJobLocalCancellationCompletedEventReceived(this.m_input.ScopeId, this.m_input.PlanId, this.m_jobIdentifier);
      }
      this.m_executionState.State = PipelineState.Completed;
      this.m_executionState.Result = new TaskResult?(result);
      return result;
    }

    private Task ExecuteAsync(
      OrchestrationContext context,
      RunServerJobInput input,
      Func<Task> operation,
      int maxAttempts = 5)
    {
      return context.ExecuteAsync(operation, maxAttempts, canRetry: this.RetryException, traceException: (Action<Exception>) (ex => context.TraceJobException(input.ScopeId, input.PlanId, this.m_input.Job.Name, ex)));
    }

    private Task<T> ExecuteAsync<T>(
      OrchestrationContext context,
      RunServerJobInput input,
      Func<Task<T>> operation,
      int maxAttempts = 5)
    {
      return context.ExecuteAsync<T>(operation, maxAttempts, canRetry: this.RetryException, traceException: (Action<Exception>) (ex => context.TraceJobException(input.ScopeId, input.PlanId, this.m_input.Job.Name, ex)));
    }

    private void EnsureExtensions(
      OrchestrationContext context,
      int activityDispatcherShardsCount,
      IActivityShardKey shardKey)
    {
      this.JobManager = context.CreateShardedClient<IServerJobManager>(true, activityDispatcherShardsCount, shardKey, "Server");
      this.JobLogger = context.CreateShardedClient<IServerJobLogger>(true, activityDispatcherShardsCount, shardKey, "Server");
    }

    private struct WaitEventData
    {
      internal Task WaitTask { get; set; }

      internal TimeSpan? Timeout { get; set; }
    }
  }
}
