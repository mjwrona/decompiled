// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.Tasks.RunPhase
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: ACF674F2-B05D-403A-A061-F4792BD3317C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks.dll

using Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Orchestration;
using Microsoft.VisualStudio.Services.Orchestration.TaskActivityDispatcherSharding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines.Tasks
{
  public abstract class RunPhase : 
    TaskOrchestration<PhaseExecutionState, RunPhaseInput, object, PhaseExecutionState>
  {
    private const int MaxJobAttempts = 2;
    protected IPipelineIdGenerator m_idGenerator;
    protected PhaseExecutionState m_executionState;
    protected readonly TaskCompletionSource<CanceledEvent> m_cancellationSource;

    public RunPhase() => this.m_cancellationSource = new TaskCompletionSource<CanceledEvent>();

    public TaskCompletionSource<CanceledEvent> CancellationSource => this.m_cancellationSource;

    public IJobController JobController { get; private set; }

    public IPhaseController PhaseController { get; private set; }

    public IPipelineLogger Logger { get; private set; }

    public Func<Exception, bool> RetryException { get; set; }

    public override void OnEvent(OrchestrationContext context, string name, object input)
    {
      context.TraceEvent(name);
      if (!(name == "Canceled"))
        return;
      this.m_executionState.State = PipelineState.Canceling;
      this.m_cancellationSource.TrySetResult((CanceledEvent) input);
    }

    public override PhaseExecutionState OnGetStatus() => this.m_executionState;

    public override async Task<PhaseExecutionState> RunTask(
      OrchestrationContext context,
      RunPhaseInput input)
    {
      this.EnsureExtensions(context, input.ActivityDispatcherShardsCount, (IActivityShardKey) input.ShardKey);
      this.m_idGenerator = (IPipelineIdGenerator) new PipelineIdGenerator(input.PlanVersion < 4);
      this.m_executionState = input.Phase;
      this.m_executionState.State = PipelineState.InProgress;
      this.m_executionState.StartTime = new DateTime?(context.CurrentUtcDateTime);
      PhaseExecutionState expandedPhase;
      try
      {
        if (input.PlanVersion < 3)
        {
          expandedPhase = await this.PhaseController.ExpandTemplate(input.ScopeId, input.PlanId, input.Phase.Name, (IList<PhaseExecutionState>) input.DependsOn.Values.ToList<PhaseExecutionState>());
        }
        else
        {
          List<string> list = input.Phase.Jobs.Select<JobExecutionState, string>((Func<JobExecutionState, string>) (x => x.Name)).ToList<string>();
          expandedPhase = await this.PhaseController.Expand(input.ScopeId, input.PlanId, input.StageName, input.Phase.Name, input.DependsOn, input.StageAttempt, input.Phase.Attempt, (IList<string>) list);
        }
      }
      catch (Exception ex) when (ex is TaskFailedException taskFailedException && taskFailedException.InnerException is PipelineValidationException)
      {
        this.m_executionState.State = PipelineState.Completed;
        this.m_executionState.Result = new TaskResult?(TaskResult.Failed);
        this.m_executionState.FinishTime = new DateTime?(context.CurrentUtcDateTime);
        List<TimelineRecord> phaseRecord = new List<TimelineRecord>()
        {
          new TimelineRecord()
          {
            Id = this.m_idGenerator.GetPhaseInstanceId(input.StageName, input.Phase.Name, input.Phase.Attempt),
            State = new TimelineRecordState?(TimelineRecordState.Completed),
            FinishTime = this.m_executionState.FinishTime,
            Result = this.m_executionState.Result
          }
        };
        await this.ExecuteAsync(context, input, (Func<Task>) (() => this.Logger.UpdateTimeline(input.ScopeId, input.PlanId, (IList<TimelineRecord>) phaseRecord)));
        return this.TrimState(this.m_executionState);
      }
      this.m_executionState.ContinueOnError = expandedPhase.ContinueOnError;
      this.m_executionState.MaxConcurrency = expandedPhase.MaxConcurrency;
      this.m_executionState.FailFast = expandedPhase.FailFast;
      this.m_executionState.Jobs.Clear();
      this.m_executionState.Jobs.AddRange<JobExecutionState, IList<JobExecutionState>>((IEnumerable<JobExecutionState>) expandedPhase.Jobs);
      TaskResult phaseResult = TaskResult.Succeeded;
      List<Task<TaskResult>> currentlyRunningTasks = new List<Task<TaskResult>>();
      List<string> currentlyRunningJobNames = new List<string>();
      int count = Math.Max(1, this.m_executionState.MaxConcurrency);
      Queue<JobExecutionState> pendingExecution = new Queue<JobExecutionState>(this.m_executionState.Jobs.Skip<JobExecutionState>(count));
      foreach (JobExecutionState job in this.m_executionState.Jobs.Take<JobExecutionState>(count))
      {
        currentlyRunningJobNames.Add(job.Name);
        currentlyRunningTasks.Add(this.ExecuteJobAsync(context, input, input.Phase, job));
      }
      List<JobExecutionState> notRunJobs = new List<JobExecutionState>();
      while (currentlyRunningTasks.Count > 0)
      {
        Task<TaskResult> task = await Task.WhenAny<TaskResult>((IEnumerable<Task<TaskResult>>) currentlyRunningTasks);
        int index = currentlyRunningTasks.IndexOf(task);
        currentlyRunningTasks.RemoveAt(index);
        string str1 = currentlyRunningJobNames[index];
        currentlyRunningJobNames.RemoveAt(index);
        TaskResult childResult;
        if (task.IsFaulted)
        {
          childResult = TaskResult.Failed;
          context.TraceException((Exception) task.Exception);
        }
        else
          childResult = task.Result;
        phaseResult = PipelineUtilities.MergeResult(phaseResult, childResult);
        if (phaseResult == TaskResult.Failed && this.m_executionState.FailFast)
        {
          string str2 = (string) null;
          if (!string.IsNullOrEmpty(str1))
            str2 = Resources.PipelineJobCancelFailFast((object) str1);
          this.m_cancellationSource.TrySetResult(new CanceledEvent()
          {
            Reason = str2
          });
          pendingExecution.Clear();
          foreach (JobExecutionState job in (IEnumerable<JobExecutionState>) this.m_executionState.Jobs)
          {
            if (job.State == PipelineState.NotStarted)
            {
              if (input.PlanVersion != 8)
                notRunJobs.Add(job);
              job.State = PipelineState.Completed;
              job.Result = new TaskResult?(input.PlanVersion >= 8 ? TaskResult.Canceled : TaskResult.Skipped);
            }
          }
        }
        if (this.m_executionState.State == PipelineState.InProgress && pendingExecution.Count > 0)
        {
          JobExecutionState job = pendingExecution.Dequeue();
          currentlyRunningJobNames.Add(job.Name);
          currentlyRunningTasks.Add(this.ExecuteJobAsync(context, input, input.Phase, job));
        }
      }
      if (this.m_executionState.State == PipelineState.Canceling)
      {
        foreach (JobExecutionState job in (IEnumerable<JobExecutionState>) this.m_executionState.Jobs)
        {
          if (job.State == PipelineState.NotStarted)
          {
            job.State = PipelineState.Completed;
            job.Result = new TaskResult?(input.PlanVersion >= 8 ? TaskResult.Canceled : TaskResult.Skipped);
            if (input.PlanVersion != 8)
              notRunJobs.Add(job);
          }
          else if (input.PlanVersion < 8 && job.State == PipelineState.Completed)
          {
            TaskResult? result = job.Result;
            TaskResult taskResult = TaskResult.Skipped;
            if (result.GetValueOrDefault() == taskResult & result.HasValue)
              notRunJobs.Add(job);
          }
        }
        phaseResult = input.PlanVersion >= 8 || notRunJobs.Count != this.m_executionState.Jobs.Count ? TaskResult.Canceled : TaskResult.Skipped;
      }
      if (phaseResult == TaskResult.Failed && this.m_executionState.ContinueOnError)
        phaseResult = TaskResult.SucceededWithIssues;
      this.m_executionState.Result = new TaskResult?(phaseResult);
      this.m_executionState.State = PipelineState.Completed;
      this.m_executionState.FinishTime = new DateTime?(context.CurrentUtcDateTime);
      List<TimelineRecord> recordsToUpdate = new List<TimelineRecord>();
      List<TimelineRecord> timelineRecordList = recordsToUpdate;
      TimelineRecord timelineRecord = new TimelineRecord();
      timelineRecord.Id = this.m_idGenerator.GetPhaseInstanceId(input.StageName, input.Phase.Name, input.Phase.Attempt);
      timelineRecord.State = new TimelineRecordState?(TimelineRecordState.Completed);
      TaskResult? result1 = this.m_executionState.Result;
      TaskResult taskResult1 = TaskResult.Skipped;
      timelineRecord.FinishTime = result1.GetValueOrDefault() == taskResult1 & result1.HasValue ? new DateTime?() : this.m_executionState.FinishTime;
      timelineRecord.Result = this.m_executionState.Result;
      timelineRecordList.Add(timelineRecord);
      if (notRunJobs.Count > 0)
        recordsToUpdate.AddRange(notRunJobs.Select<JobExecutionState, TimelineRecord>((Func<JobExecutionState, TimelineRecord>) (x => new TimelineRecord()
        {
          Id = this.m_idGenerator.GetJobInstanceId(input.StageName, input.Phase.Name, x.Name, x.Attempt),
          State = new TimelineRecordState?(TimelineRecordState.Completed),
          Result = x.Result
        })));
      await this.ExecuteAsync(context, input, (Func<Task>) (() => this.Logger.UpdateTimeline(input.ScopeId, input.PlanId, (IList<TimelineRecord>) recordsToUpdate)));
      return this.TrimState(this.m_executionState);
    }

    protected abstract Task<JobExecutionState> RunJob(
      OrchestrationContext context,
      PhaseExecutionState phase,
      JobExecutionState job,
      string instanceId,
      RunPhaseInput input);

    protected virtual void EnsureExtensions(
      OrchestrationContext context,
      int activityDispatcherShardsCount,
      IActivityShardKey shardKey)
    {
      this.Logger = context.CreateShardedClient<IPipelineLogger>(true, activityDispatcherShardsCount, shardKey);
      this.JobController = context.CreateShardedClient<IJobController>(true, activityDispatcherShardsCount, shardKey);
      this.PhaseController = context.CreateShardedClient<IPhaseController>(true, activityDispatcherShardsCount, shardKey);
    }

    protected PhaseExecutionState TrimState(PhaseExecutionState phase)
    {
      PhaseExecutionState phaseExecutionState = new PhaseExecutionState();
      phaseExecutionState.StartTime = phase.StartTime;
      phaseExecutionState.FinishTime = phase.FinishTime;
      phaseExecutionState.State = phase.State;
      phaseExecutionState.Result = phase.Result;
      phaseExecutionState.Jobs.AddRange<JobExecutionState, IList<JobExecutionState>>((IEnumerable<JobExecutionState>) phase.Jobs);
      return phaseExecutionState;
    }

    private string GetOrchestrationId(RunPhaseInput input, JobExecutionState job)
    {
      string jobInstanceName = this.m_idGenerator.GetJobInstanceName(input.StageName, input.Phase.Name, job.Name, job.Attempt);
      return input.PlanVersion < 4 ? string.Format("{0:D}_{1:D}", (object) input.PlanId, (object) TimelineRecordIdGenerator.GetId(jobInstanceName)) : string.Format("{0:D}.{1}", (object) input.PlanId, (object) jobInstanceName.ToLowerInvariant());
    }

    protected async Task<TaskResult> ExecuteJobAsync(
      OrchestrationContext context,
      RunPhaseInput input,
      PhaseExecutionState phase,
      JobExecutionState job)
    {
      string jobInstanceId = this.GetOrchestrationId(input, job);
      Guid jobRecordId = this.m_idGenerator.GetJobInstanceId(input.StageName, input.Phase.Name, job.Name, job.Attempt);
      try
      {
        if (this.m_executionState.State == PipelineState.Canceling)
        {
          job.State = PipelineState.Completed;
          job.Result = new TaskResult?(input.PlanVersion >= 8 ? TaskResult.Canceled : TaskResult.Skipped);
        }
        else
        {
          for (int attempt = 0; attempt < 2; ++attempt)
          {
            job.State = PipelineState.InProgress;
            job.StartTime = new DateTime?(context.CurrentUtcDateTime);
            Task<JobExecutionState> jobTask = this.RunJob(context, phase, job, jobInstanceId, input);
            if (await Task.WhenAny((Task) this.CancellationSource.Task, (Task) jobTask) == this.CancellationSource.Task)
            {
              CanceledEvent canceledEvent = this.CancellationSource.Task.Result;
              TimeSpan cancelTimeout = job.CancelTimeoutInMinutes <= 0 ? TimeSpan.FromMinutes(1.0) : TimeSpan.FromMinutes((double) job.CancelTimeoutInMinutes);
              if (canceledEvent.Timeout != new TimeSpan())
                cancelTimeout = canceledEvent.Timeout;
              context.TraceInfo(string.Format("Cancelling job '{0}' with a cancellation timeout of {1} minutes and reason {2}", (object) job.Name, (object) cancelTimeout.TotalMinutes, (object) canceledEvent.Reason));
              await context.ExecuteAsync((Func<Task>) (() => this.JobController.CancelJob(jobInstanceId, cancelTimeout, canceledEvent.Reason)), canRetry: this.RetryException, traceException: closure_4 ?? (closure_4 = (Action<Exception>) (ex => context.TraceException(ex))));
            }
            JobExecutionState jobExecutionState = await jobTask;
            if (attempt < 1)
            {
              JobError error = jobExecutionState.Error;
              if ((error != null ? (error.CanRetry ? 1 : 0) : 0) != 0 && this.m_executionState.State != PipelineState.Canceling)
              {
                TaskResult? result = jobExecutionState.Result;
                TaskResult taskResult1 = TaskResult.Failed;
                if (!(result.GetValueOrDefault() == taskResult1 & result.HasValue))
                {
                  result = jobExecutionState.Result;
                  TaskResult taskResult2 = TaskResult.Abandoned;
                  if (!(result.GetValueOrDefault() == taskResult2 & result.HasValue))
                    goto label_18;
                }
                context.TraceInfo("Retry job '" + job.Name + "' one more time.");
                if (input.PlanVersion > 4)
                {
                  ++job.Attempt;
                  await this.PhaseController.CreateJobAttempt(input.ScopeId, input.PlanId, input.StageName, input.Phase.Name, job.Name, job.Attempt, job.Attempt - 1);
                  jobInstanceId = this.GetOrchestrationId(input, job);
                  jobRecordId = this.m_idGenerator.GetJobInstanceId(input.StageName, input.Phase.Name, job.Name, job.Attempt);
                  continue;
                }
                continue;
              }
            }
label_18:
            job.CopyFrom(jobTask.Result);
            break;
          }
        }
      }
      catch (SubOrchestrationFailedException ex)
      {
        context.TraceException((Exception) ex);
        job.FinishTime = new DateTime?(context.CurrentUtcDateTime);
        job.State = PipelineState.Completed;
        job.Result = new TaskResult?(TaskResult.Failed);
        job.Error = new JobError()
        {
          Message = ex.InnerException == null ? ex.Message : ex.InnerException.Message
        };
      }
      TaskResult? result1 = job.Result;
      TaskResult taskResult3 = TaskResult.Failed;
      int num = result1.GetValueOrDefault() == taskResult3 & result1.HasValue ? 1 : 0;
      result1 = job.Result;
      TaskResult taskResult4 = TaskResult.Failed;
      if (result1.GetValueOrDefault() == taskResult4 & result1.HasValue && job.ContinueOnError)
        job.Result = new TaskResult?(TaskResult.SucceededWithIssues);
      OrchestrationContext context1 = context;
      string name = job.Name;
      result1 = job.Result;
      // ISSUE: variable of a boxed type
      __Boxed<TaskResult> local = (Enum) result1.Value;
      string message = string.Format("Completed orchestration for job '{0}' with result {1}", (object) name, (object) local);
      context1.TraceInfo(message);
      if (num != 0 && !job.StartTime.HasValue)
      {
        TimelineRecord jobRecord = new TimelineRecord()
        {
          Id = jobRecordId,
          State = new TimelineRecordState?(TimelineRecordState.Completed),
          Result = job.Result
        };
        if (job.Error != null)
          jobRecord.Issues.Add(new Issue()
          {
            Type = IssueType.Error,
            Message = job.Error.Message
          });
        await this.ExecuteAsync(context, input, (Func<Task>) (() => this.Logger.UpdateTimeline(input.ScopeId, input.PlanId, (IList<TimelineRecord>) new TimelineRecord[1]
        {
          jobRecord
        })));
      }
      else if (job.Error != null)
        await this.LogIssue(context, input, jobRecordId, IssueType.Error, job.Error.Message);
      result1 = job.Result;
      return result1.Value;
    }

    protected Task<TResult> ExecuteAsync<TResult>(
      OrchestrationContext context,
      RunPhaseInput input,
      Func<Task<TResult>> operation)
    {
      return context.ExecuteAsync<TResult>(operation, canRetry: this.RetryException, traceException: (Action<Exception>) (ex => context.TraceException(ex)));
    }

    protected Task ExecuteAsync(
      OrchestrationContext context,
      RunPhaseInput input,
      Func<Task> operation)
    {
      return context.ExecuteAsync(operation, canRetry: this.RetryException, traceException: (Action<Exception>) (ex => context.TraceException(ex)));
    }

    protected async Task LogIssue(
      OrchestrationContext context,
      RunPhaseInput input,
      Guid instanceId,
      IssueType issueType,
      string message)
    {
      try
      {
        await this.ExecuteAsync(context, input, (Func<Task>) (() => this.Logger.LogIssue(input.ScopeId, input.PlanId, instanceId, context.CurrentUtcDateTime, issueType, message)));
      }
      catch (TaskFailedException ex)
      {
      }
    }
  }
}
