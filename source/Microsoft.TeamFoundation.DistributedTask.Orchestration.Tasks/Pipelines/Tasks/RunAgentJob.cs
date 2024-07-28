// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.Tasks.RunAgentJob
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: ACF674F2-B05D-403A-A061-F4792BD3317C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks;
using Microsoft.TeamFoundation.DistributedTask.Pipelines.Runtime;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.VisualStudio.Services.Orchestration;
using Microsoft.VisualStudio.Services.Orchestration.TaskActivityDispatcherSharding;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines.Tasks
{
  public sealed class RunAgentJob : RunJob<RunAgentJobInput>
  {
    private static readonly HashSet<Type> s_exceptionTypesCausedByUser = new HashSet<Type>((IEnumerable<Type>) new Type[7]
    {
      typeof (TaskAgentJobFailedNotEnoughSubscriptionResourcesException),
      typeof (TaskAgentNotFoundException),
      typeof (ProjectDoesNotExistException),
      typeof (ProjectDoesNotExistWithNameException),
      typeof (TaskAgentPoolRemovedException),
      typeof (TaskAgentQueueNotFoundException),
      typeof (TaskDefinitionNotFoundException)
    });
    private bool m_timedOut;
    private bool m_canceled;
    private RunAgentJobInput m_input;
    private Guid m_jobId;
    private PhaseTargetType m_jobType;
    private AgentRequestData m_jobData;
    private IPipelineIdGenerator m_idGenerator;
    private CancellationTokenSource m_jobTimeoutCancel = new CancellationTokenSource();
    private TaskCompletionSource<JobCompletedEvent> m_jobCompleted = new TaskCompletionSource<JobCompletedEvent>();
    private TaskCompletionSource<CanceledEvent> m_jobCanceled = new TaskCompletionSource<CanceledEvent>();
    private TaskCompletionSource<TaskAgentJobRequest> m_jobAssigned = new TaskCompletionSource<TaskAgentJobRequest>();
    private TaskCompletionSource<JobMetadataEvent> m_jobMetadataUpdate = new TaskCompletionSource<JobMetadataEvent>();
    private TimeSpan m_cancellationTimeout;
    private const string c_teamName = "CIPlatform";

    public IPipelineLogger Logger { get; private set; }

    public IAgentPoolManager PoolManager { get; private set; }

    public IPhaseController PhaseController { get; private set; }

    public override void OnEvent(OrchestrationContext context, string name, object input)
    {
      context.TraceEvent(name);
      switch (name)
      {
        case "JobAssigned":
          JobAssignedEvent jobAssignedEvent = (JobAssignedEvent) input;
          if (this.m_jobData == null || this.m_jobData.RequestId == jobAssignedEvent.Request.RequestId)
          {
            this.m_jobAssigned.TrySetResult(jobAssignedEvent.Request);
            break;
          }
          OrchestrationContext context1 = context;
          AgentRequestData jobData = this.m_jobData;
          string message = string.Format("Received an assigned event for the wrong request identifier. Expected {0} but got {1}", (object) (jobData != null ? jobData.RequestId : 0L), (object) jobAssignedEvent.Request.RequestId);
          context1.TraceError(10015506, message);
          break;
        case "JobCompleted":
          JobCompletedEvent result = (JobCompletedEvent) input;
          if (result.RequestId == 0L || this.m_jobData == null || this.m_jobData.RequestId == result.RequestId)
          {
            context.TraceInfo(string.Format("Received event {0}. RequestId: {1}. JobId: {2}. AgentShuttingDown: {3}", (object) name, (object) result?.RequestId, (object) result?.JobId, (object) result?.AgentShuttingDown));
            this.m_jobCompleted.TrySetResult(result);
            break;
          }
          context.TraceError(10015507, string.Format("Received a completed event for the wrong request identifier. Expected {0} but got {1}", (object) this.m_jobData.RequestId, (object) result.RequestId));
          break;
        case "JobCanceled":
          this.m_executionState.State = PipelineState.Canceling;
          this.m_jobCanceled.TrySetResult((CanceledEvent) input);
          break;
        case "JobMetadataUpdate":
          this.m_jobMetadataUpdate.TrySetResult((JobMetadataEvent) input);
          break;
      }
    }

    public override JobExecutionState OnGetStatus() => this.m_executionState;

    public override async Task<JobExecutionState> RunTask(
      OrchestrationContext context,
      RunAgentJobInput input)
    {
      RunAgentJob runAgentJob = this;
      runAgentJob.EnsureExtensions(context, input.ActivityDispatcherShardsCount, (IActivityShardKey) input.ShardKey);
      context.TraceStartLinearOrchestration();
      context.TraceStartLinearPhase("CIPlatform", "QueueRequest");
      runAgentJob.m_input = input;
      runAgentJob.m_idGenerator = (IPipelineIdGenerator) new PipelineIdGenerator(input.PlanVersion < 4);
      runAgentJob.m_executionState = input.Job;
      runAgentJob.m_executionState.State = PipelineState.InProgress;
      runAgentJob.m_executionState.StartTime = new DateTime?(context.CurrentUtcDateTime);
      JobParameters jobParameters = new JobParameters()
      {
        Name = runAgentJob.m_input.Job.Name,
        Attempt = runAgentJob.m_input.Job.Attempt,
        PhaseName = runAgentJob.m_input.PhaseName,
        PhaseAttempt = runAgentJob.m_input.PhaseAttempt,
        StageName = runAgentJob.m_input.StageName,
        StageAttempt = runAgentJob.m_input.StageAttempt,
        CheckRerunAttempt = runAgentJob.m_input.Job.CheckRerunAttempt
      };
      if (!await runAgentJob.EnforcePolicyForJobResources(context, input.ScopeId, input.PlanId, input.PlanVersion, jobParameters, input.ActivityDispatcherShardsCount, input.ShardKey))
        return runAgentJob.m_executionState;
      jobParameters.CandidateAgentIds.AddRange((IEnumerable<int>) runAgentJob.m_input.AgentIds);
      runAgentJob.m_jobId = runAgentJob.m_idGenerator.GetJobInstanceId(jobParameters.StageName, jobParameters.PhaseName, jobParameters.Name, jobParameters.Attempt);
      string errorCode = (string) null;
      bool errorIsExpected = false;
      bool jobStarted = false;
      CancellationTokenSource jobExecutionTimerCancel = (CancellationTokenSource) null;
      bool agentShuttingDown = false;
      try
      {
        if (input.PoolId > 0)
        {
          context.TraceInfo(string.Format("Requesting an agent in pool {0}", (object) input.PoolId));
          AgentRequestData agentRequestData = await runAgentJob.ExecuteAsync<AgentRequestData>(context, input, (Func<Task<AgentRequestData>>) (() => this.PoolManager.QueueRequestToPool(input.PoolId, input.ScopeId, input.PlanId, jobParameters, input.AgentSpecification)));
          runAgentJob.m_jobData = agentRequestData;
          runAgentJob.m_jobType = PhaseTargetType.Pool;
        }
        else
        {
          context.TraceInfo(string.Format("Requesting an agent in queue {0}", (object) input.QueueId));
          AgentRequestData agentRequestData = await runAgentJob.ExecuteAsync<AgentRequestData>(context, input, (Func<Task<AgentRequestData>>) (() => this.PoolManager.QueueRequest(input.QueueId, input.ScopeId, input.PlanId, jobParameters, input.AgentSpecification)));
          runAgentJob.m_jobData = agentRequestData;
        }
        if (input.PlanVersion > 6)
          await runAgentJob.PhaseController.UpdateTimelineRecordPoolData(input.PlanId, runAgentJob.m_jobId, runAgentJob.m_jobData.PoolId, runAgentJob.m_jobData.AgentSpecification);
        TaskResult? nullable;
        if (runAgentJob.m_jobData.ReservedAgent == null)
        {
          context.TraceStartLinearPhase("CIPlatform", "WaitForAssignment");
          context.TraceInfo(string.Format("Waiting for an agent in pool {0} with request {1}", (object) runAgentJob.m_jobData.PoolId, (object) runAgentJob.m_jobData.RequestId));
          Task task = await Task.WhenAny((Task) runAgentJob.m_jobAssigned.Task, (Task) runAgentJob.m_jobCompleted.Task, (Task) runAgentJob.m_jobCanceled.Task);
          if (task == runAgentJob.m_jobAssigned.Task)
            runAgentJob.m_jobData.ReservedAgent = runAgentJob.m_jobAssigned.Task.Result.ReservedAgent;
          else if (task == runAgentJob.m_jobCompleted.Task)
          {
            runAgentJob.m_executionState.Result = new TaskResult?(runAgentJob.m_jobCompleted.Task.Result.Result);
            agentShuttingDown = runAgentJob.m_jobCompleted.Task.Result.AgentShuttingDown;
            nullable = runAgentJob.m_executionState.Result;
            TaskResult taskResult = TaskResult.Abandoned;
            if (nullable.GetValueOrDefault() == taskResult & nullable.HasValue)
            {
              context.TraceStartLinearPhase("CIPlatform", "GetAgentRequest");
              AgentRequestData agentRequestData = await runAgentJob.ExecuteAsync<AgentRequestData>(context, input, (Func<Task<AgentRequestData>>) (() => this.PoolManager.GetRequest(this.m_jobData.PoolId, this.m_jobData.RequestId)));
              errorIsExpected = true;
              errorCode = "Abandoned";
              runAgentJob.m_executionState.Error = new JobError()
              {
                CanRetry = !agentRequestData.ReceiveTime.HasValue,
                Message = Resources.JobAbandonedNotStarted((object) runAgentJob.m_jobData.RequestId)
              };
            }
            context.TraceError(10015508, string.Format("Received job completed event for request {0} with result {1} without receiving the assigned event", (object) runAgentJob.m_jobData.RequestId, (object) runAgentJob.m_executionState.Result));
          }
          else
          {
            runAgentJob.m_executionState.Result = new TaskResult?(TaskResult.Canceled);
            CanceledEvent canceledEvent = runAgentJob.m_jobCanceled.Task.Result;
            if (!string.IsNullOrEmpty(canceledEvent.Reason))
            {
              context.Trace(10015564, TraceLevel.Warning, "JobResult := Canceled. Reason: " + canceledEvent.Reason);
              await runAgentJob.ExecuteAsync(context, input, (Func<Task>) (() => this.Logger.LogIssue(input.ScopeId, input.PlanId, this.m_jobId, context.CurrentUtcDateTime, IssueType.Error, canceledEvent.Reason)));
            }
          }
        }
        if (runAgentJob.m_jobData.ReservedAgent != null)
        {
          context.TraceStartLinearPhase("CIPlatform", "SendJob");
          context.TraceInfo(string.Format("Reserved and sending job to agent {0} (Id:{1}, Version:{2}) ", (object) runAgentJob.m_jobData.ReservedAgent.Name, (object) runAgentJob.m_jobData.ReservedAgent.Id, (object) runAgentJob.m_jobData.ReservedAgent.Version) + string.Format("in pool {0} for request {1}", (object) runAgentJob.m_jobData.PoolId, (object) runAgentJob.m_jobData.RequestId));
          if (runAgentJob.m_input.NotifyJobAssigned)
          {
            string instanceId = string.Format("{0:D}.{1}", (object) input.PlanId, (object) runAgentJob.m_idGenerator.GetPhaseInstanceName(input.StageName, input.PhaseName, 1).ToLowerInvariant());
            await runAgentJob.PhaseController.JobAssigned(instanceId, new JobAssignedEventData()
            {
              JobId = runAgentJob.m_jobId,
              AgentRequest = runAgentJob.m_jobData
            });
          }
          if (runAgentJob.m_input.NotifyProviderJobStarted)
          {
            JobStartedEventData eventData = new JobStartedEventData()
            {
              JobType = runAgentJob.m_jobType,
              JobId = runAgentJob.m_jobId,
              Data = (object) new AgentJobStartedData()
              {
                ReservedAgent = runAgentJob.m_jobData.ReservedAgent
              }
            };
            await runAgentJob.PhaseController.JobStarted(input.ScopeId, input.PlanId, jobParameters, eventData);
          }
          await runAgentJob.ExecuteAsync(context, input, (Func<Task>) (() => this.PoolManager.StartJob(this.m_jobData.PoolId, this.m_jobData.RequestId, this.m_jobData.ReservedAgent, input.ScopeId, input.PlanId, jobParameters)));
          jobStarted = true;
          context.TraceStartLinearPhase("CIPlatform", "WaitForJobComplete");
          context.TraceInfo(string.Format("Successfully sent the job to agent {0} ({1}) in pool {2} for request {3}", (object) runAgentJob.m_jobData.ReservedAgent.Name, (object) runAgentJob.m_jobData.ReservedAgent.Id, (object) runAgentJob.m_jobData.PoolId, (object) runAgentJob.m_jobData.RequestId));
          Task<string> jobExecutionTimerTask = (Task<string>) null;
          if (input.Job.TimeoutInMinutes > 0 && input.Job.TimeoutInMinutes < int.MaxValue)
          {
            jobExecutionTimerCancel = new CancellationTokenSource();
            jobExecutionTimerTask = context.CreateTimer<string>(context.CurrentUtcDateTime.Add(TimeSpan.FromMinutes((double) input.Job.TimeoutInMinutes)), (string) null, jobExecutionTimerCancel.Token);
          }
          CancellationTokenSource pollingTimerCancel;
          TaskResult? requestResult;
          while (true)
          {
            TimeSpan timeSpan = runAgentJob.m_canceled ? runAgentJob.m_cancellationTimeout : TimeSpan.FromMinutes(15.0);
            pollingTimerCancel = new CancellationTokenSource();
            Task<string> pollingTimerTask = context.CreateTimer<string>(context.CurrentUtcDateTime.Add(timeSpan), (string) null, pollingTimerCancel.Token);
            List<Task> taskList = new List<Task>()
            {
              (Task) pollingTimerTask,
              (Task) runAgentJob.m_jobCompleted.Task,
              (Task) runAgentJob.m_jobMetadataUpdate.Task
            };
            if (!runAgentJob.m_canceled)
            {
              taskList.Add((Task) runAgentJob.m_jobCanceled.Task);
              if (jobExecutionTimerTask != null)
                taskList.Add((Task) jobExecutionTimerTask);
            }
            Task task = await Task.WhenAny((IEnumerable<Task>) taskList);
            if (task == runAgentJob.m_jobCanceled.Task)
            {
              runAgentJob.m_canceled = true;
              pollingTimerCancel.Cancel();
              CanceledEvent canceledEvent = runAgentJob.m_jobCanceled.Task.Result;
              runAgentJob.m_cancellationTimeout = canceledEvent.Timeout;
              if (runAgentJob.m_cancellationTimeout == new TimeSpan())
                runAgentJob.m_cancellationTimeout = runAgentJob.TransformToTimespan(runAgentJob.m_executionState.CancelTimeoutInMinutes);
              await runAgentJob.ExecuteAsync(context, input, closure_7 ?? (closure_7 = (Func<Task>) (() => this.PoolManager.CancelJob(this.m_jobData.PoolId, this.m_jobData.RequestId, this.m_jobData.ReservedAgent, input.ScopeId, input.PlanId, this.m_jobId, this.m_cancellationTimeout))));
              context.TraceInfo(string.Format("Sent cancellation to agent {0} ({1}) in pool {2} for request {3}. Reason: {4}", (object) runAgentJob.m_jobData.ReservedAgent.Name, (object) runAgentJob.m_jobData.ReservedAgent.Id, (object) runAgentJob.m_jobData.PoolId, (object) runAgentJob.m_jobData.RequestId, (object) canceledEvent.Reason));
              if (!string.IsNullOrEmpty(canceledEvent.Reason))
                await runAgentJob.ExecuteAsync(context, input, (Func<Task>) (() => this.Logger.LogIssue(input.ScopeId, input.PlanId, this.m_jobId, context.CurrentUtcDateTime, IssueType.Error, canceledEvent.Reason)));
            }
            else if (task == jobExecutionTimerTask)
            {
              context.TraceInfo(string.Format("Request {0} has exceeded the job execution timeout of {1} minutes.", (object) runAgentJob.m_jobData.RequestId, (object) input.Job.TimeoutInMinutes));
              runAgentJob.m_executionState.State = PipelineState.Canceling;
              runAgentJob.m_timedOut = true;
              runAgentJob.m_canceled = true;
              pollingTimerCancel.Cancel();
              runAgentJob.m_cancellationTimeout = runAgentJob.TransformToTimespan(runAgentJob.m_executionState.CancelTimeoutInMinutes);
              await runAgentJob.ExecuteAsync(context, input, (Func<Task>) (() => this.PoolManager.CancelJob(this.m_jobData.PoolId, this.m_jobData.RequestId, this.m_jobData.ReservedAgent, input.ScopeId, input.PlanId, this.m_jobId, this.m_cancellationTimeout)));
              await runAgentJob.ExecuteAsync(context, input, (Func<Task>) (() => this.Logger.LogIssue(input.ScopeId, input.PlanId, this.m_jobId, context.CurrentUtcDateTime, IssueType.Error, Resources.JobTimedOut((object) this.m_jobData.ReservedAgent.Name, (object) input.Job.TimeoutInMinutes))));
            }
            else if (task != runAgentJob.m_jobCompleted.Task)
            {
              if (task == pollingTimerTask)
              {
                nullable = await runAgentJob.ExecuteAsync<TaskResult?>(context, input, (Func<Task<TaskResult?>>) (() => this.PoolManager.GetJobResult(this.m_jobData.PoolId, this.m_jobData.RequestId, input.ScopeId, input.PlanId, this.m_jobId)));
                requestResult = nullable;
                if (runAgentJob.m_canceled && !requestResult.HasValue)
                {
                  context.TraceError(10015565, string.Format("The agent '{0}' ({1}) in pool '{2}' for request {3} did not respond within the cancellation timeout ({4})", (object) runAgentJob.m_jobData?.ReservedAgent?.Name, (object) runAgentJob.m_jobData?.ReservedAgent?.Id, (object) runAgentJob.m_jobData?.PoolId, (object) runAgentJob.m_jobData?.RequestId, (object) runAgentJob.m_cancellationTimeout));
                  requestResult = new TaskResult?(runAgentJob.m_timedOut ? TaskResult.Failed : TaskResult.Canceled);
                  await runAgentJob.ExecuteAsync(context, input, (Func<Task>) (() => this.Logger.LogIssue(input.ScopeId, input.PlanId, this.m_jobId, context.CurrentUtcDateTime, IssueType.Warning, Resources.JobCancelTimeout((object) this.m_jobData.ReservedAgent.Name, (object) this.m_cancellationTimeout))));
                }
                if (!requestResult.HasValue)
                  requestResult = new TaskResult?();
                else
                  goto label_55;
              }
              else if (task == runAgentJob.m_jobMetadataUpdate.Task)
              {
                pollingTimerCancel.Cancel();
                JobMetadataEvent metadataEvent = runAgentJob.m_jobMetadataUpdate.Task.Result;
                context.TraceInfo(string.Format("Sending metadata update to agent {0} ({1}) in pool {2} for request {3}", (object) runAgentJob.m_jobData.ReservedAgent.Name, (object) runAgentJob.m_jobData.ReservedAgent.Id, (object) runAgentJob.m_jobData.PoolId, (object) runAgentJob.m_jobData.RequestId));
                await runAgentJob.ExecuteAsync(context, input, (Func<Task>) (() => this.PoolManager.SendMetadataUpdate(this.m_jobData.PoolId, this.m_jobData.RequestId, metadataEvent.Message)));
                runAgentJob.m_jobMetadataUpdate = new TaskCompletionSource<JobMetadataEvent>();
              }
            }
            else
              break;
            pollingTimerCancel = (CancellationTokenSource) null;
            pollingTimerTask = (Task<string>) null;
          }
          pollingTimerCancel.Cancel();
          runAgentJob.m_executionState.Result = new TaskResult?(runAgentJob.m_timedOut ? TaskResult.Failed : runAgentJob.m_jobCompleted.Task.Result.Result);
          agentShuttingDown = runAgentJob.m_jobCompleted.Task.Result.AgentShuttingDown;
          goto label_61;
label_55:
          runAgentJob.m_executionState.Result = new TaskResult?(requestResult.Value);
label_61:
          jobExecutionTimerTask = (Task<string>) null;
        }
      }
      catch (TaskFailedException ex)
      {
        context.TraceException((Exception) ex);
        runAgentJob.m_executionState.Error = new JobError()
        {
          CanRetry = !jobStarted && (ex.InnerException is TaskAgentJobNotFoundException || ex.InnerException is TaskAgentJobTokenExpiredException)
        };
        Type type;
        if (ex.InnerException is AggregateException innerException)
        {
          type = innerException.InnerException?.GetType();
          runAgentJob.m_executionState.Error.Message = innerException.Flatten().InnerExceptions.Aggregate<Exception, string>(string.Empty, (Func<string, Exception, string>) ((current, e) => current + e.Message + " "));
        }
        else
        {
          type = ex.InnerException?.GetType();
          runAgentJob.m_executionState.Error.Message = ex.Message;
        }
        errorIsExpected = RunAgentJob.s_exceptionTypesCausedByUser.Contains(type);
        errorCode = type?.Name;
        runAgentJob.m_executionState.Result = new TaskResult?(TaskResult.Failed);
      }
      finally
      {
        if (jobExecutionTimerCancel != null)
        {
          jobExecutionTimerCancel.Cancel();
          jobExecutionTimerCancel = (CancellationTokenSource) null;
        }
      }
      if (runAgentJob.m_jobData != null && runAgentJob.m_jobData.ReservedAgent != null)
        context.TraceInfo(string.Format("Received a job complete notification from agent {0} ({1}) in pool {2} for request {3} with result {4}", (object) runAgentJob.m_jobData.ReservedAgent.Name, (object) runAgentJob.m_jobData.ReservedAgent.Id, (object) runAgentJob.m_jobData.PoolId, (object) runAgentJob.m_jobData.RequestId, (object) runAgentJob.m_executionState.Result.Value));
      TaskResult? result = runAgentJob.m_executionState.Result;
      TaskResult taskResult1 = TaskResult.Abandoned;
      if (result.GetValueOrDefault() == taskResult1 & result.HasValue && runAgentJob.m_executionState.Error == null && runAgentJob.m_jobData.ReservedAgent != null)
      {
        errorIsExpected = true;
        errorCode = "Abandoned";
        runAgentJob.m_executionState.Error = new JobError()
        {
          Message = Resources.JobAbandoned((object) runAgentJob.m_jobData.ReservedAgent.Name)
        };
      }
      if (runAgentJob.m_jobData != null)
      {
        context.TraceStartLinearPhase("CIPlatform", "ReleaseAgent");
        try
        {
          await runAgentJob.ExecuteAsync(context, input, (Func<Task>) (() => this.PoolManager.DeleteRequest(this.m_jobData.PoolId, this.m_jobData.RequestId, input.ScopeId, input.PlanId, this.m_jobId, this.m_executionState.Result.Value, this.m_jobData.ReservedAgent, agentShuttingDown)), 10);
          if (runAgentJob.m_jobData.ReservedAgent != null)
            context.TraceInfo(string.Format("Released agent {0} ({1}) back to pool {2} for request {3}", (object) runAgentJob.m_jobData.ReservedAgent.Name, (object) runAgentJob.m_jobData.ReservedAgent.Id, (object) runAgentJob.m_jobData.PoolId, (object) runAgentJob.m_jobData.RequestId));
        }
        catch (Exception ex)
        {
          if (runAgentJob.m_jobData.ReservedAgent != null)
            context.TraceError(10015548, string.Format("Exception while deleting agent request  {0} ({1}) of pool {2}", (object) runAgentJob.m_jobData.ReservedAgent.Name, (object) runAgentJob.m_jobData.ReservedAgent.Id, (object) runAgentJob.m_jobData.PoolId));
          context.TraceException(ex);
        }
      }
      if (!runAgentJob.m_timedOut && runAgentJob.m_executionState.State == PipelineState.Canceling)
      {
        if (jobStarted || input.PlanVersion >= 8)
          runAgentJob.m_executionState.Result = new TaskResult?(TaskResult.Canceled);
        else
          runAgentJob.m_executionState.Result = new TaskResult?(TaskResult.Skipped);
      }
      runAgentJob.m_executionState.State = PipelineState.Completed;
      runAgentJob.m_executionState.StartTime = jobStarted ? runAgentJob.m_executionState.StartTime : new DateTime?();
      JobExecutionState executionState = runAgentJob.m_executionState;
      DateTime? nullable1 = runAgentJob.m_executionState.StartTime;
      DateTime? nullable2;
      if (!nullable1.HasValue)
      {
        nullable1 = new DateTime?();
        nullable2 = nullable1;
      }
      else
        nullable2 = new DateTime?(context.CurrentUtcDateTime);
      executionState.FinishTime = nullable2;
      if (input.PlanVersion >= 9)
      {
        try
        {
          if (!jobStarted)
          {
            TimelineRecord record = new TimelineRecord()
            {
              Id = runAgentJob.m_idGenerator.GetJobInstanceId(input.StageName, input.PhaseName, input.Job.Name, input.Job.Attempt),
              State = new TimelineRecordState?(TimelineRecordState.Completed),
              Result = runAgentJob.m_executionState.Result
            };
            await runAgentJob.ExecuteAsync(context, input, (Func<Task>) (() => this.Logger.UpdateTimeline(input.ScopeId, input.PlanId, (IList<TimelineRecord>) new TimelineRecord[1]
            {
              record
            })));
          }
          await runAgentJob.ExecuteAsync(context, input, (Func<Task>) (() => this.Logger.JobCompleted(input.ScopeId, input.PlanId, this.m_jobId, this.m_executionState.Result)));
        }
        catch (Exception ex)
        {
          context.TraceException(ex);
        }
      }
      if (runAgentJob.m_executionState.Error == null)
      {
        context.TraceCompleteLinearOrchestration("CIPlatform", "JobCompleted");
      }
      else
      {
        if (errorCode == null)
        {
          result = runAgentJob.m_executionState.Result;
          errorCode = result.ToString();
        }
        context.TraceCompleteLinearOrchestrationWithError("CIPlatform", "JobCompleted", errorCode, runAgentJob.m_executionState.Error.Message, errorIsExpected);
      }
      return runAgentJob.TrimState(runAgentJob.m_executionState);
    }

    private void EnsureExtensions(
      OrchestrationContext context,
      int activityDispatcherShardsCount,
      IActivityShardKey shardKey)
    {
      this.PoolManager = context.CreateShardedClient<IAgentPoolManager>(true, activityDispatcherShardsCount, shardKey);
      this.Logger = context.CreateShardedClient<IPipelineLogger>(true, activityDispatcherShardsCount, shardKey);
      this.PhaseController = context.CreateShardedClient<IPhaseController>(true, activityDispatcherShardsCount, shardKey);
    }

    private JobExecutionState TrimState(JobExecutionState job) => new JobExecutionState()
    {
      Error = job.Error,
      StartTime = job.StartTime,
      FinishTime = job.FinishTime,
      State = job.State,
      Result = job.Result
    };

    private Task ExecuteAsync(
      OrchestrationContext context,
      RunAgentJobInput input,
      Func<Task> operation,
      int maxAttempts = 5)
    {
      return context.ExecuteAsync(operation, maxAttempts, canRetry: this.RetryException, traceException: (Action<Exception>) (ex => context.TraceException(ex)));
    }

    private Task<T> ExecuteAsync<T>(
      OrchestrationContext context,
      RunAgentJobInput input,
      Func<Task<T>> operation,
      int maxAttempts = 5)
    {
      return context.ExecuteAsync<T>(operation, maxAttempts, canRetry: this.RetryException, traceException: (Action<Exception>) (ex => context.TraceException(ex)));
    }

    private TimeSpan TransformToTimespan(int timeout)
    {
      if (timeout <= 0)
        return TimeSpan.FromMinutes(1.0);
      return timeout > 35790 ? TimeSpan.FromMinutes(35790.0) : TimeSpan.FromMinutes((double) timeout);
    }
  }
}
