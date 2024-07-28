// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.Tasks.RunServerJob
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: ACF674F2-B05D-403A-A061-F4792BD3317C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks.dll

using Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks;
using Microsoft.TeamFoundation.DistributedTask.Pipelines.Runtime;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Orchestration;
using Microsoft.VisualStudio.Services.Orchestration.TaskActivityDispatcherSharding;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines.Tasks
{
  internal sealed class RunServerJob : RunJob<RunServerJobInput>
  {
    private Guid m_jobRecordId;
    private string m_jobIdentifier;
    private RunServerJobInput m_input;
    private DateTime? m_cancelExpirationTime;
    private IPipelineIdGenerator m_idGenerator;
    private TaskCompletionSource<bool> m_jobStarted;
    private TaskCompletionSource<bool> m_jobAssigned;
    private TaskCompletionSource<TaskResult> m_jobCompleted;
    private readonly TaskCompletionSource<CanceledEvent> m_jobCanceled = new TaskCompletionSource<CanceledEvent>();

    public IServerTaskConditionEvaluator ConditionEvaluator { get; private set; }

    public IServerJobLogger JobLogger { get; private set; }

    public IServerJobManager JobManager { get; private set; }

    public IServerTaskController TaskController { get; private set; }

    public override void OnEvent(OrchestrationContext context, string name, object input)
    {
      context.TraceEvent(name);
      switch (name)
      {
        case "JobAssigned":
          this.m_jobAssigned.TrySetResult(true);
          break;
        case "JobStarted":
          this.m_jobStarted.TrySetResult(true);
          break;
        case "JobCompleted":
          this.m_jobCompleted.TrySetResult(((JobCompletedEvent) input).Result);
          break;
        case "JobCanceled":
          this.m_jobCanceled.TrySetResult((CanceledEvent) input);
          break;
      }
    }

    public override JobExecutionState OnGetStatus() => this.m_executionState;

    public override async Task<JobExecutionState> RunTask(
      OrchestrationContext context,
      RunServerJobInput input)
    {
      RunServerJob runServerJob = this;
      runServerJob.EnsureExtensions(context, input.ShardKey, input.ActivityDispatcherShardsCount);
      runServerJob.m_idGenerator = (IPipelineIdGenerator) new PipelineIdGenerator(input.PlanVersion < 4);
      runServerJob.m_input = input;
      runServerJob.m_executionState = input.Job;
      runServerJob.m_executionState.State = PipelineState.InProgress;
      runServerJob.m_executionState.StartTime = new DateTime?(context.CurrentUtcDateTime);
      runServerJob.m_executionState.Result = new TaskResult?(TaskResult.Succeeded);
      runServerJob.m_jobIdentifier = runServerJob.m_idGenerator.GetJobInstanceName(input.StageName, input.PhaseName, input.Job.Name, input.Job.Attempt, input.Job.CheckRerunAttempt);
      runServerJob.m_jobRecordId = runServerJob.m_idGenerator.GetInstanceId(runServerJob.m_jobIdentifier);
      JobParameters parameters = new JobParameters()
      {
        StageName = runServerJob.m_input.StageName,
        StageAttempt = runServerJob.m_input.StageAttempt,
        PhaseName = runServerJob.m_input.PhaseName,
        PhaseAttempt = runServerJob.m_input.PhaseAttempt,
        Name = runServerJob.m_input.Job.Name,
        Attempt = runServerJob.m_input.Job.Attempt,
        CheckRerunAttempt = runServerJob.m_input.Job.CheckRerunAttempt
      };
      if (!await runServerJob.EnforcePolicyForJobResources(context, input.ScopeId, input.PlanId, input.PlanVersion, parameters, input.ActivityDispatcherShardsCount, input.ShardKey))
        return runServerJob.m_executionState;
      if (input.NotifyProviderJobStarted)
        await runServerJob.ExecuteAsync(context, runServerJob.m_input, (Func<Task>) (() => this.JobManager.JobStarted(this.m_input.ScopeId, this.m_input.PlanId, parameters, new JobStartedEventData()
        {
          JobType = PhaseTargetType.Server,
          JobId = this.m_jobRecordId
        })));
      if (runServerJob.m_executionState.Tasks.Count == 0)
      {
        IList<TaskExecutionState> values = await runServerJob.ExecuteAsync<IList<TaskExecutionState>>(context, runServerJob.m_input, (Func<Task<IList<TaskExecutionState>>>) (() => this.JobManager.Expand(this.m_input.ScopeId, this.m_input.PlanId, parameters)));
        runServerJob.m_executionState.Tasks.AddRange<TaskExecutionState, IList<TaskExecutionState>>((IEnumerable<TaskExecutionState>) values);
      }
      Task timeoutTask = (Task) null;
      CancellationTokenSource timeoutToken = (CancellationTokenSource) null;
      if (runServerJob.m_executionState.TimeoutInMinutes > 0 && runServerJob.m_executionState.TimeoutInMinutes < int.MaxValue)
      {
        timeoutToken = new CancellationTokenSource();
        timeoutTask = (Task) context.CreateTimer<string>(context.CurrentUtcDateTime.AddMinutes((double) runServerJob.m_executionState.TimeoutInMinutes), (string) null, timeoutToken.Token);
      }
      try
      {
        string str = timeoutTask == null ? "no timeout" : string.Format("{0} minutes timeout", (object) runServerJob.m_executionState.TimeoutInMinutes);
        context.TraceInfo(string.Format("Start executing {0} server tasks in server job with {1}.", (object) runServerJob.m_executionState.Tasks.Count, (object) str));
        foreach (TaskExecutionState task in (IEnumerable<TaskExecutionState>) runServerJob.m_executionState.Tasks)
        {
          runServerJob.m_jobStarted = new TaskCompletionSource<bool>();
          runServerJob.m_jobAssigned = new TaskCompletionSource<bool>();
          runServerJob.m_jobCompleted = new TaskCompletionSource<TaskResult>();
          ServerTaskResult serverTaskResult = await runServerJob.RunServerTask(context, task, (Task) runServerJob.m_jobAssigned.Task, (Task) runServerJob.m_jobStarted.Task, runServerJob.m_jobCompleted.Task, (Task) runServerJob.m_jobCanceled.Task, timeoutTask);
          runServerJob.m_executionState.Result = new TaskResult?(serverTaskResult.Result);
          runServerJob.m_executionState.DeliveryFailed = serverTaskResult.DeliveryFailed;
          context.TraceInfo(string.Format("Results after executing {0}: Result: '{1}', DeliveryFailed: '{2}'.", (object) task.Name, (object) runServerJob.m_executionState.Result.Value, (object) runServerJob.m_executionState.DeliveryFailed));
        }
      }
      finally
      {
        timeoutToken?.Cancel();
        timeoutToken?.Dispose();
      }
      runServerJob.m_executionState.FinishTime = new DateTime?(context.CurrentUtcDateTime);
      runServerJob.m_executionState.State = PipelineState.Completed;
      context.TraceInfo(string.Format("Received job completed notification with result {0} and deliveryFailed {1}", (object) runServerJob.m_executionState.Result.Value, (object) runServerJob.m_executionState.DeliveryFailed));
      await runServerJob.ExecuteAsync(context, runServerJob.m_input, (Func<Task>) (() => this.JobLogger.JobCompleted(this.m_input.ScopeId, this.m_input.PlanId, this.m_jobRecordId, context.CurrentUtcDateTime, this.m_executionState.Result.Value)));
      return runServerJob.m_executionState;
    }

    private async Task<bool> ShouldExecute(OrchestrationContext context, TaskExecutionState task)
    {
      RunServerJob runServerJob = this;
      if (runServerJob.m_executionState.State == PipelineState.Canceling)
      {
        DateTime currentUtcDateTime = context.CurrentUtcDateTime;
        DateTime? cancelExpirationTime = runServerJob.m_cancelExpirationTime;
        if ((cancelExpirationTime.HasValue ? (currentUtcDateTime >= cancelExpirationTime.GetValueOrDefault() ? 1 : 0) : 0) != 0)
          return false;
      }
      TaskConditionContext context1 = new TaskConditionContext()
      {
        JobName = runServerJob.m_input.Job.Name,
        JobAttempt = runServerJob.m_input.Job.Attempt,
        StageName = runServerJob.m_input.StageName,
        StageAttempt = runServerJob.m_input.StageAttempt,
        PhaseName = runServerJob.m_input.PhaseName,
        PhaseAttempt = runServerJob.m_input.PhaseAttempt,
        TaskName = task.Name,
        State = runServerJob.m_executionState.State,
        Result = runServerJob.m_executionState.Result.Value
      };
      return await runServerJob.ConditionEvaluator.EvaluateCondition(runServerJob.m_input.ScopeId, runServerJob.m_input.PlanId, task.Condition, context1);
    }

    private async Task<ServerTaskResult> RunServerTask(
      OrchestrationContext context,
      TaskExecutionState task,
      Task assignedTask,
      Task startedTask,
      Task<TaskResult> completedTask,
      Task canceledTask,
      Task timeoutTask)
    {
      RunServerJob runServerJob = this;
      Guid taskId = runServerJob.m_idGenerator.GetTaskInstanceId(runServerJob.m_input.StageName, runServerJob.m_input.PhaseName, runServerJob.m_input.Job.Name, runServerJob.m_input.Job.Attempt, task.Name, runServerJob.m_input.Job.CheckRerunAttempt);
      if (!await runServerJob.ShouldExecute(context, task))
      {
        context.TraceInfo(string.Format("Skip server task {0} ({1}) base on condition.", (object) task.Name, (object) taskId));
        task.StartTime = new DateTime?(context.CurrentUtcDateTime);
        task.FinishTime = new DateTime?(context.CurrentUtcDateTime);
        task.State = PipelineState.Completed;
        task.Result = new TaskResult?(TaskResult.Skipped);
        await runServerJob.JobLogger.TaskCompleted(runServerJob.m_input.ScopeId, runServerJob.m_input.PlanId, runServerJob.m_jobRecordId, taskId, context.CurrentUtcDateTime, task.Result.Value);
        return new ServerTaskResult(PipelineUtilities.MergeResult(runServerJob.m_executionState.Result.Value, TaskResult.Skipped), runServerJob.m_executionState.DeliveryFailed);
      }
      List<Task> allTasks = new List<Task>(6);
      allTasks.Add(assignedTask);
      allTasks.Add(startedTask);
      allTasks.Add(canceledTask);
      allTasks.Add((Task) completedTask);
      if (timeoutTask != null)
        allTasks.Add(timeoutTask);
      RunServerTaskInput input = new RunServerTaskInput()
      {
        ShardKey = runServerJob.m_input.ShardKey,
        ActivityDispatcherShardsCount = runServerJob.m_input.ActivityDispatcherShardsCount,
        ScopeId = runServerJob.m_input.ScopeId,
        PlanId = runServerJob.m_input.PlanId,
        PlanVersion = runServerJob.m_input.PlanVersion,
        JobName = runServerJob.m_input.Job.Name,
        JobAttempt = runServerJob.m_input.Job.Attempt,
        StageName = runServerJob.m_input.StageName,
        StageAttempt = runServerJob.m_input.StageAttempt,
        PhaseName = runServerJob.m_input.PhaseName,
        PhaseAttempt = runServerJob.m_input.PhaseAttempt,
        Task = task,
        CheckRerunAttempt = runServerJob.m_input.Job.CheckRerunAttempt
      };
      Task cancelTimeoutTask = (Task) null;
      CancellationTokenSource cancelTimeoutToken = (CancellationTokenSource) null;
      string instanceId = runServerJob.GetInstanceId(input);
      Task<TaskExecutionState> executeTask = context.CreateSubOrchestrationInstance<TaskExecutionState>("RunPipelineServerTask", "1.0", instanceId, (object) input);
      allTasks.Add((Task) executeTask);
      try
      {
        while (allTasks.Count > 0)
        {
          Task task1 = await Task.WhenAny((IEnumerable<Task>) allTasks);
          if (task1 != executeTask)
          {
            if (task1 == canceledTask)
            {
              allTasks.Remove(canceledTask);
              if (!runServerJob.m_cancelExpirationTime.HasValue)
              {
                runServerJob.m_executionState.State = PipelineState.Canceling;
                if (!runServerJob.m_executionState.Result.HasValue)
                  runServerJob.m_executionState.Result = new TaskResult?(TaskResult.Canceled);
                runServerJob.m_cancelExpirationTime = new DateTime?(context.CurrentUtcDateTime.AddMinutes((double) runServerJob.m_executionState.CancelTimeoutInMinutes));
              }
              if (!await runServerJob.ShouldExecute(context, task))
              {
                context.TraceInfo("Cancel running server task " + task.Name + ".");
                await runServerJob.TaskController.Cancel(instanceId, new CanceledEvent()
                {
                  Reason = runServerJob.m_jobCanceled.Task.Result.Reason,
                  Timeout = TimeSpan.FromSeconds(15.0)
                });
              }
              else if (runServerJob.m_executionState.CancelTimeoutInMinutes > 0)
              {
                cancelTimeoutToken = new CancellationTokenSource();
                cancelTimeoutTask = (Task) context.CreateTimer<string>(runServerJob.m_cancelExpirationTime.Value, (string) null, cancelTimeoutToken.Token);
                allTasks.Add(cancelTimeoutTask);
                context.TraceInfo(string.Format("Server task {0} will continue run for at most {1} minutes on cancellation.", (object) task.Name, (object) runServerJob.m_executionState.CancelTimeoutInMinutes));
              }
            }
            else if (task1 == cancelTimeoutTask)
            {
              allTasks.Remove(cancelTimeoutTask);
              context.TraceInfo("Server task " + task.Name + " exceed cancellation timeout.");
              await runServerJob.TaskController.Cancel(instanceId, new CanceledEvent()
              {
                Reason = Resources.ServerTaskCancellationTimedOut((object) runServerJob.m_executionState.CancelTimeoutInMinutes),
                Timeout = TimeSpan.FromSeconds(15.0)
              });
            }
            else if (task1 == timeoutTask)
            {
              allTasks.Remove(timeoutTask);
              context.TraceInfo("Server job exceed job timeout while running task " + task.Name + ".");
              runServerJob.m_executionState.Result = new TaskResult?(TaskResult.Failed);
              runServerJob.m_jobCanceled.TrySetResult(new CanceledEvent()
              {
                Reason = Resources.ServerJobExecutionTimedOut((object) runServerJob.m_executionState.TimeoutInMinutes)
              });
            }
            else
            {
              allTasks.Remove(task1);
              if (task1 == assignedTask)
              {
                TaskAssignedEvent assignedEvent = new TaskAssignedEvent(runServerJob.m_jobRecordId, taskId);
                await runServerJob.ExecuteAsync(context, runServerJob.m_input, (Func<Task>) (() => this.TaskController.Notify(instanceId, (TaskEvent) assignedEvent)));
              }
              else if (task1 == startedTask)
              {
                TaskStartedEvent startedEvent = new TaskStartedEvent(runServerJob.m_jobRecordId, taskId);
                await runServerJob.ExecuteAsync(context, runServerJob.m_input, (Func<Task>) (() => this.TaskController.Notify(instanceId, (TaskEvent) startedEvent)));
              }
              else if (task1 == completedTask)
              {
                TaskCompletedEvent completedEvent = new TaskCompletedEvent(runServerJob.m_jobRecordId, taskId, completedTask.Result);
                await runServerJob.ExecuteAsync(context, runServerJob.m_input, (Func<Task>) (() => this.TaskController.Notify(instanceId, (TaskEvent) completedEvent)));
              }
            }
          }
          else
            break;
        }
      }
      finally
      {
        cancelTimeoutToken?.Cancel();
        cancelTimeoutToken?.Dispose();
      }
      TaskExecutionState source = await executeTask;
      task.CopyFrom(source);
      return new ServerTaskResult(PipelineUtilities.MergeResult(runServerJob.m_executionState.Result.Value, task.Result.Value), runServerJob.m_executionState.DeliveryFailed || source.DeliveryFailed);
    }

    private string GetInstanceId(RunServerTaskInput input)
    {
      string taskInstanceName = this.m_idGenerator.GetTaskInstanceName(input.StageName, input.PhaseName, input.JobName, input.JobAttempt, input.Task.Name, input.CheckRerunAttempt);
      return input.PlanVersion < 4 ? string.Format("{0:D}_{1:D}_{2:D}", (object) input.PlanId, (object) this.m_jobRecordId, (object) TimelineRecordIdGenerator.GetId(taskInstanceName)) : string.Format("{0:D}.{1}", (object) input.PlanId, (object) taskInstanceName.ToLowerInvariant());
    }

    private Task ExecuteAsync(
      OrchestrationContext context,
      RunServerJobInput input,
      Func<Task> operation,
      int maxAttempts = 5)
    {
      return context.ExecuteAsync(operation, maxAttempts, canRetry: this.RetryException, traceException: (Action<Exception>) (ex => context.TraceException(ex)));
    }

    private Task<T> ExecuteAsync<T>(
      OrchestrationContext context,
      RunServerJobInput input,
      Func<Task<T>> operation,
      int maxAttempts = 5)
    {
      return context.ExecuteAsync<T>(operation, maxAttempts, canRetry: this.RetryException, traceException: (Action<Exception>) (ex => context.TraceException(ex)));
    }

    private void EnsureExtensions(
      OrchestrationContext context,
      PipelineActivityShardKey shardKey,
      int activityDispatcherShardsCount)
    {
      this.ConditionEvaluator = context.CreateShardedClient<IServerTaskConditionEvaluator>(true, activityDispatcherShardsCount, (IActivityShardKey) shardKey, "Server");
      this.JobManager = context.CreateShardedClient<IServerJobManager>(true, activityDispatcherShardsCount, (IActivityShardKey) shardKey, "Server");
      this.JobManager = context.CreateShardedClient<IServerJobManager>(true, activityDispatcherShardsCount, (IActivityShardKey) shardKey, "Server");
      this.JobLogger = context.CreateShardedClient<IServerJobLogger>(true, activityDispatcherShardsCount, (IActivityShardKey) shardKey, "Server");
      this.JobManager = context.CreateShardedClient<IServerJobManager>(true, activityDispatcherShardsCount, (IActivityShardKey) shardKey, "Server");
      this.TaskController = context.CreateShardedClient<IServerTaskController>(true, activityDispatcherShardsCount, (IActivityShardKey) shardKey, "Server");
    }
  }
}
