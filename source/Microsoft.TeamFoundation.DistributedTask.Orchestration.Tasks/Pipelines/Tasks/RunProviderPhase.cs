// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.Tasks.RunProviderPhase
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
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines.Tasks
{
  internal sealed class RunProviderPhase : 
    TaskOrchestration<PhaseExecutionState, RunPhaseInput, object, PhaseExecutionState>
  {
    private IPipelineIdGenerator m_idGenerator;
    private PhaseExecutionState m_executionState;
    private TaskCompletionSource<CanceledEvent> m_phaseCancellationSource;
    private TaskCompletionSource<TaskResult> m_phaseCompletionSource;
    private TaskCompletionSource<bool> m_jobQueueCompletionSource;
    private Queue<JobExecutionState> m_pendingQueueJobs;

    public RunProviderPhase()
    {
      this.m_phaseCancellationSource = new TaskCompletionSource<CanceledEvent>();
      this.m_phaseCompletionSource = new TaskCompletionSource<TaskResult>();
      this.m_pendingQueueJobs = new Queue<JobExecutionState>();
      this.m_jobQueueCompletionSource = new TaskCompletionSource<bool>();
    }

    public IProviderPhaseController Controller { get; private set; }

    public IPipelineLogger Logger { get; private set; }

    public Func<Exception, bool> RetryException { get; set; }

    public override void OnEvent(OrchestrationContext context, string name, object input)
    {
      context.TraceEvent(name);
      switch (name)
      {
        case "Canceled":
          this.m_phaseCancellationSource.TrySetResult((CanceledEvent) input);
          break;
        case "Completed":
          TaskResult result;
          Enum.TryParse<TaskResult>(input.ToString(), out result);
          this.m_phaseCompletionSource.TrySetResult(result);
          break;
        case "JobQueued":
          this.m_pendingQueueJobs.Enqueue((JobExecutionState) input);
          this.m_jobQueueCompletionSource.TrySetResult(true);
          break;
      }
    }

    public override async Task<PhaseExecutionState> RunTask(
      OrchestrationContext context,
      RunPhaseInput input)
    {
      this.EnsureExtensions(context, input.ActivityDispatcherShardsCount, (IActivityShardKey) input.ShardKey);
      this.m_idGenerator = (IPipelineIdGenerator) new PipelineIdGenerator(input.PlanVersion < 4);
      this.m_executionState = input.Phase;
      this.m_executionState.State = PipelineState.InProgress;
      this.m_executionState.StartTime = new DateTime?(context.CurrentUtcDateTime);
      context.TraceInfo("Start '" + input.Phase.Provider + "' provider phase '" + input.Phase.Name + "'");
      await this.ExecuteAsync(context, input, (Func<Task>) (() => this.Controller.StartPhaseAsync(input.ScopeId, input.PlanId, input.StageName, input.StageAttempt, input.Phase.Name, input.Phase.Attempt, input.Phase.Provider)));
      context.TraceInfo("Waiting for '" + input.Phase.Provider + "' provider to orchestrate the phase");
      Dictionary<string, Task<JobExecutionState>> runningJobs = new Dictionary<string, Task<JobExecutionState>>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
label_2:
      while (true)
      {
        List<Task> taskList = new List<Task>();
        taskList.Add((Task) this.m_phaseCompletionSource.Task);
        taskList.Add((Task) this.m_jobQueueCompletionSource.Task);
        if (this.m_executionState.State != PipelineState.Canceling)
          taskList.Add((Task) this.m_phaseCancellationSource.Task);
        if (runningJobs.Values.Count > 0)
          taskList.AddRange((IEnumerable<Task>) runningJobs.Values);
        Task task = await Task.WhenAny((IEnumerable<Task>) taskList);
        if (task != this.m_jobQueueCompletionSource.Task)
        {
          if (task == this.m_phaseCancellationSource.Task)
          {
            CanceledEvent canceledEvent = this.m_phaseCancellationSource.Task.Result;
            context.TraceInfo("Provider phase has been cancelled.");
            this.m_executionState.State = PipelineState.Canceling;
            await this.ExecuteAsync(context, input, (Func<Task>) (() => this.Controller.CancelPhaseAsync(input.ScopeId, input.PlanId, input.StageName, input.StageAttempt, input.Phase.Name, input.Phase.Attempt, input.Phase.Provider, canceledEvent.Reason)));
          }
          else if (task != this.m_phaseCompletionSource.Task)
          {
            KeyValuePair<string, Task<JobExecutionState>> keyValuePair = runningJobs.First<KeyValuePair<string, Task<JobExecutionState>>>((Func<KeyValuePair<string, Task<JobExecutionState>>, bool>) (x => x.Value.IsCompleted));
            string jobInstanceName = keyValuePair.Key;
            JobExecutionState jobState = keyValuePair.Value.Result;
            runningJobs.Remove(jobInstanceName);
            context.TraceInfo(string.Format("Job '{0}' has finished with result {1}, {2} jobs remain running.", (object) jobState.Name, (object) jobState.Result.GetValueOrDefault(), (object) runningJobs.Values.Count));
            await this.ExecuteAsync(context, input, (Func<Task>) (() => this.Controller.JobCompleted(input.ScopeId, input.PlanId, input.StageName, input.StageAttempt, input.Phase.Name, input.Phase.Attempt, jobInstanceName, input.Phase.Provider, jobState.Result.Value)));
          }
          else
            goto label_15;
        }
        else
          break;
      }
      this.m_jobQueueCompletionSource = new TaskCompletionSource<bool>();
      while (this.m_pendingQueueJobs.Count > 0)
      {
        JobExecutionState jobState = this.m_pendingQueueJobs.Dequeue();
        context.TraceInfo("'" + input.Phase.Provider + "' provider request to execute job " + jobState.Name);
        string jobInstanceName = this.m_idGenerator.GetJobInstanceName(input.StageName, input.Phase.Name, jobState.Name, jobState.Attempt);
        runningJobs[jobInstanceName] = this.ExecuteJobAsync(context, input, input.Phase, jobState, jobInstanceName);
      }
      goto label_2;
label_15:
      this.m_executionState.Result = new TaskResult?(this.m_phaseCompletionSource.Task.Result);
      this.m_executionState.State = PipelineState.Completed;
      this.m_executionState.FinishTime = new DateTime?(context.CurrentUtcDateTime);
      context.TraceInfo(string.Format("'{0}' provider finish the phase with result: {1}.", (object) input.Phase.Provider, (object) this.m_executionState.Result));
      if (runningJobs.Count > 0)
        context.TraceError("'" + input.Phase.Provider + "' provider has following jobs still in running state. " + string.Join(", ", (IEnumerable<string>) runningJobs.Keys));
      if (input.PlanVersion >= 9)
      {
        List<TimelineRecord> recordsToUpdate = new List<TimelineRecord>();
        List<TimelineRecord> timelineRecordList = recordsToUpdate;
        TimelineRecord timelineRecord = new TimelineRecord();
        timelineRecord.Id = this.m_idGenerator.GetPhaseInstanceId(input.StageName, input.Phase.Name, input.Phase.Attempt);
        timelineRecord.State = new TimelineRecordState?(TimelineRecordState.Completed);
        TaskResult? result = this.m_executionState.Result;
        TaskResult taskResult = TaskResult.Skipped;
        timelineRecord.FinishTime = result.GetValueOrDefault() == taskResult & result.HasValue ? new DateTime?() : this.m_executionState.FinishTime;
        timelineRecord.Result = this.m_executionState.Result;
        timelineRecordList.Add(timelineRecord);
        await this.ExecuteAsync(context, input, (Func<Task>) (() => this.Logger.UpdateTimeline(input.ScopeId, input.PlanId, (IList<TimelineRecord>) recordsToUpdate)));
      }
      PhaseExecutionState executionState = this.m_executionState;
      runningJobs = (Dictionary<string, Task<JobExecutionState>>) null;
      return executionState;
    }

    private async Task<JobExecutionState> ExecuteJobAsync(
      OrchestrationContext context,
      RunPhaseInput input,
      PhaseExecutionState phase,
      JobExecutionState jobState,
      string jobInstanceName)
    {
      this.m_executionState.Jobs.Add(jobState);
      Guid jobRecordId = this.m_idGenerator.GetJobInstanceId(input.StageName, input.Phase.Name, jobState.Name, jobState.Attempt);
      try
      {
        jobState.State = PipelineState.InProgress;
        jobState.StartTime = new DateTime?(context.CurrentUtcDateTime);
        Task<JobExecutionState> orchestrationInstance;
        if (jobState.Target.Type == PhaseTargetType.Queue || jobState.Target.Type == PhaseTargetType.Pool)
        {
          AgentQueueTarget target1 = jobState.Target as AgentQueueTarget;
          AgentPoolTarget target2 = jobState.Target as AgentPoolTarget;
          RunAgentJobInput input1 = new RunAgentJobInput()
          {
            ActivityDispatcherShardsCount = input.ActivityDispatcherShardsCount,
            ShardKey = input.ShardKey,
            ScopeId = input.ScopeId,
            PlanId = input.PlanId,
            PlanVersion = input.PlanVersion,
            StageName = input.StageName,
            StageAttempt = input.StageAttempt,
            PhaseName = input.Phase.Name,
            PhaseAttempt = input.Phase.Attempt,
            QueueId = target1 != null ? target1.Queue.Id : 0,
            PoolId = target2 != null ? target2.Pool.Id : 0,
            Job = jobState,
            AgentSpecification = target2?.AgentSpecification ?? target1?.AgentSpecification,
            NotifyProviderJobStarted = true
          };
          if (target2 != null && target2.AgentIds.Count > 0)
            input1.AgentIds.AddRange((IEnumerable<int>) target2.AgentIds);
          orchestrationInstance = context.CreateSubOrchestrationInstance<JobExecutionState>("RunPipelineAgentJob", "1.0", string.Format("{0:D}.{1}", (object) input.PlanId, (object) jobInstanceName.ToLowerInvariant()), (object) input1);
        }
        else
        {
          RunServerJobInput input2 = new RunServerJobInput()
          {
            ActivityDispatcherShardsCount = input.ActivityDispatcherShardsCount,
            ShardKey = input.ShardKey,
            ScopeId = input.ScopeId,
            PlanId = input.PlanId,
            PlanVersion = input.PlanVersion,
            StageName = input.StageName,
            StageAttempt = input.StageAttempt,
            PhaseName = input.Phase.Name,
            PhaseAttempt = input.Phase.Attempt,
            Job = jobState,
            NotifyProviderJobStarted = true
          };
          orchestrationInstance = context.CreateSubOrchestrationInstance<JobExecutionState>("RunPipelineServerJob", "2.0", string.Format("{0:D}.{1}", (object) input.PlanId, (object) jobInstanceName.ToLowerInvariant()), (object) input2);
        }
        jobState.CopyFrom(await orchestrationInstance);
      }
      catch (SubOrchestrationFailedException ex)
      {
        context.TraceException((Exception) ex);
        jobState.FinishTime = new DateTime?(context.CurrentUtcDateTime);
        jobState.State = PipelineState.Completed;
        jobState.Result = new TaskResult?(TaskResult.Failed);
        jobState.Error = new JobError()
        {
          Message = ex.InnerException == null ? ex.Message : ex.InnerException.Message
        };
      }
      TaskResult? result = jobState.Result;
      TaskResult taskResult1 = TaskResult.Failed;
      int num = result.GetValueOrDefault() == taskResult1 & result.HasValue ? 1 : 0;
      result = jobState.Result;
      TaskResult taskResult2 = TaskResult.Failed;
      if (result.GetValueOrDefault() == taskResult2 & result.HasValue && jobState.ContinueOnError)
        jobState.Result = new TaskResult?(TaskResult.SucceededWithIssues);
      OrchestrationContext context1 = context;
      string name = jobState.Name;
      result = jobState.Result;
      // ISSUE: variable of a boxed type
      __Boxed<TaskResult> local = (Enum) result.Value;
      string message = string.Format("Completed orchestration for job '{0}' with result {1}", (object) name, (object) local);
      context1.TraceInfo(message);
      if (num != 0 && !jobState.StartTime.HasValue)
      {
        TimelineRecord jobRecord = new TimelineRecord()
        {
          Id = jobRecordId,
          State = new TimelineRecordState?(TimelineRecordState.Completed),
          Result = jobState.Result
        };
        if (jobState.Error != null)
          jobRecord.Issues.Add(new Issue()
          {
            Type = IssueType.Error,
            Message = jobState.Error.Message
          });
        await this.ExecuteAsync(context, input, (Func<Task>) (() => this.Logger.UpdateTimeline(input.ScopeId, input.PlanId, (IList<TimelineRecord>) new TimelineRecord[1]
        {
          jobRecord
        })));
      }
      else if (jobState.Error != null)
        await this.LogIssue(context, input, jobRecordId, IssueType.Error, jobState.Error.Message);
      return jobState;
    }

    private void EnsureExtensions(
      OrchestrationContext context,
      int activityDispatcherShardsCount,
      IActivityShardKey shardKey)
    {
      this.Logger = context.CreateShardedClient<IPipelineLogger>(true, activityDispatcherShardsCount, shardKey);
      this.Controller = context.CreateShardedClient<IProviderPhaseController>(true, activityDispatcherShardsCount, shardKey);
    }

    private async Task LogIssue(
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

    private Task ExecuteAsync(
      OrchestrationContext context,
      RunPhaseInput input,
      Func<Task> operation)
    {
      return context.ExecuteAsync(operation, canRetry: this.RetryException, traceException: (Action<Exception>) (ex => context.TraceException(ex)));
    }
  }
}
