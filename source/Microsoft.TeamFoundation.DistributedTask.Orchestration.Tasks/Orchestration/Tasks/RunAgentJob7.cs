// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks.RunAgentJob7
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: ACF674F2-B05D-403A-A061-F4792BD3317C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.VisualStudio.Services.Orchestration;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks
{
  internal sealed class RunAgentJob7 : TaskOrchestration<JobResult, RunJobInput4, object, string>
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
    private RunJobInput4 m_input;
    private TaskAgentJobRequest m_agentRequest;
    private TimeSpan m_cancelTimeout = TimeSpan.FromSeconds(60.0);
    private CancellationTokenSource m_jobTimeoutCancel = new CancellationTokenSource();
    private TaskCompletionSource<TaskResult> m_jobCompleted = new TaskCompletionSource<TaskResult>();
    private TaskCompletionSource<CanceledEvent> m_jobCanceled = new TaskCompletionSource<CanceledEvent>();
    private TaskCompletionSource<TaskAgentJobRequest> m_jobAssigned = new TaskCompletionSource<TaskAgentJobRequest>();
    private const string c_teamName = "CIPlatform";

    public IAgentPoolExtension7 Pool { get; private set; }

    public IPlanTrackingExtension3 Tracker { get; private set; }

    public IPlanControlExtension PlanController { get; private set; }

    public Func<Exception, bool> RetryException { get; set; }

    public override void OnEvent(OrchestrationContext context, string name, object input)
    {
      switch (name)
      {
        case "JobAssigned":
          JobAssignedEvent jobAssignedEvent = (JobAssignedEvent) input;
          if (this.m_agentRequest == null || this.m_agentRequest.RequestId == jobAssignedEvent.Request.RequestId)
          {
            this.m_jobAssigned.TrySetResult(jobAssignedEvent.Request);
            break;
          }
          context.TraceJobError(10015506, this.m_input.ScopeId, this.m_input.PlanId, this.m_input.Job.InstanceId, "Received an assigned event for the wrong request identifier. Expected {0} but got {1}", (object) (this.m_agentRequest != null ? this.m_agentRequest.RequestId : 0L), (object) jobAssignedEvent.Request.RequestId);
          break;
        case "JobCompleted":
          JobCompletedEvent jobCompletedEvent = (JobCompletedEvent) input;
          if (jobCompletedEvent.RequestId == 0L || this.m_agentRequest == null || this.m_agentRequest.RequestId == jobCompletedEvent.RequestId)
          {
            context.TraceInfo(string.Format("Received event {0}. RequestId: {1}. JobId: {2}. AgentShuttingDown: {3}", (object) name, (object) jobCompletedEvent?.RequestId, (object) jobCompletedEvent?.JobId, (object) jobCompletedEvent?.AgentShuttingDown));
            this.m_jobCompleted.TrySetResult(jobCompletedEvent.Result);
            break;
          }
          context.TraceJobError(10015507, this.m_input.ScopeId, this.m_input.PlanId, this.m_input.Job.InstanceId, "Received a completed event for the wrong request identifier. Expected {0} but got {1}", (object) (this.m_agentRequest != null ? this.m_agentRequest.RequestId : 0L), (object) jobCompletedEvent.RequestId);
          break;
        case "JobCanceled":
          this.m_jobCanceled.TrySetResult((CanceledEvent) input);
          break;
      }
    }

    public override async Task<JobResult> RunTask(OrchestrationContext context, RunJobInput4 input)
    {
      this.EnsureExtensions(context);
      context.TraceStartLinearOrchestration();
      context.TraceStartLinearPhase("CIPlatform", "QueueRequest");
      this.m_input = input;
      string errorCode = (string) null;
      bool errorIsExpected = false;
      bool jobStarted = false;
      Task<string> jobExecutionTimerTask = (Task<string>) null;
      CancellationTokenSource jobExecutionTimerCancel = (CancellationTokenSource) null;
      JobResult jobResult = new JobResult()
      {
        Result = TaskResult.Succeeded
      };
      try
      {
        context.TraceAgentRequesting(input.ScopeId, input.PlanId, input.Job.InstanceId, input.PoolId, (IList<Demand>) input.Job.Demands);
        this.m_agentRequest = await this.ExecuteAsync<TaskAgentJobRequest>(context, input, (Func<Task<TaskAgentJobRequest>>) (() => this.Pool.QueueAgentRequest(input.PoolId, (IList<int>) input.AgentIds, (IList<Demand>) input.Job.Demands, input.ScopeId, input.PlanId, input.Job.InstanceId)));
        if (this.m_agentRequest.ReservedAgent == null)
        {
          context.TraceStartLinearPhase("CIPlatform", "WaitForAssignment");
          context.TraceAgentWaiting(input.ScopeId, input.PlanId, input.Job.InstanceId, input.PoolId, this.m_agentRequest.RequestId);
          Task task = await Task.WhenAny((Task) this.m_jobAssigned.Task, (Task) this.m_jobCompleted.Task, (Task) this.m_jobCanceled.Task);
          if (task == this.m_jobAssigned.Task)
            this.m_agentRequest = this.m_jobAssigned.Task.Result;
          else if (task == this.m_jobCompleted.Task)
          {
            jobResult.Result = this.m_jobCompleted.Task.Result;
            if (jobResult.Result == TaskResult.Abandoned)
            {
              context.TraceStartLinearPhase("CIPlatform", "GetAgentRequest");
              jobResult.IsRetryable = !(await this.ExecuteAsync<TaskAgentJobRequest>(context, input, (Func<Task<TaskAgentJobRequest>>) (() => this.Pool.GetAgentRequest(input.PoolId, this.m_agentRequest.RequestId)))).ReceiveTime.HasValue;
              errorIsExpected = true;
              errorCode = "Abandoned";
              jobResult.Message = Resources.JobAbandonedNotStarted((object) this.m_agentRequest.RequestId);
            }
            context.TraceJobError(10015508, this.m_input.ScopeId, this.m_input.PlanId, this.m_input.Job.InstanceId, "Received job completed event for request {0} with result {1} without receiving the assigned event", (object) this.m_agentRequest.RequestId, (object) jobResult.Result);
          }
          else
          {
            jobResult.Result = TaskResult.Canceled;
            CanceledEvent canceledEvent = this.m_jobCanceled.Task.Result;
            if (!string.IsNullOrEmpty(canceledEvent.Reason))
            {
              context.Trace(10015564, TraceLevel.Warning, "JobResult := Canceled. Reason: " + canceledEvent.Reason);
              await this.ExecuteAsync(context, input, (Func<Task>) (() => this.Tracker.LogIssue(input.ScopeId, input.PlanId, input.Job.InstanceId, context.CurrentUtcDateTime, IssueType.Error, canceledEvent.Reason)));
            }
          }
        }
        if (this.m_agentRequest.ReservedAgent != null)
        {
          context.TraceAgentAssigned(input.ScopeId, input.PlanId, input.Job.InstanceId, input.PoolId, this.m_agentRequest.RequestId, this.m_agentRequest.ReservedAgent);
          context.TraceAgentJobSending(input.ScopeId, input.PlanId, input.Job.InstanceId, input.PoolId, this.m_agentRequest.RequestId, this.m_agentRequest.ReservedAgent);
          if (this.m_input.NotifyJobAssigned)
          {
            context.TraceStartLinearPhase("CIPlatform", "NotifyJobAssigned");
            await this.PlanController.JobAssigned(this.m_agentRequest);
          }
          context.TraceStartLinearPhase("CIPlatform", "SendJob");
          await this.ExecuteAsync(context, input, (Func<Task>) (() => this.Pool.StartJob(input.PoolId, this.m_agentRequest.RequestId, this.m_agentRequest.ReservedAgent, input.ScopeId, input.PlanId, input.Job.InstanceId, input.JobAttempt)));
          jobStarted = true;
          context.TraceStartLinearPhase("CIPlatform", "WaitForJobComplete");
          context.TraceAgentJobSent(input.ScopeId, input.PlanId, input.Job.InstanceId, input.PoolId, this.m_agentRequest.RequestId, this.m_agentRequest.ReservedAgent);
          if (input.Job.ExecutionTimeout.HasValue && input.Job.ExecutionTimeout.Value > TimeSpan.Zero && input.Job.ExecutionTimeout.Value < TimeSpan.FromMilliseconds((double) int.MaxValue))
          {
            jobExecutionTimerCancel = new CancellationTokenSource();
            jobExecutionTimerTask = context.CreateTimer<string>(context.CurrentUtcDateTime.Add(input.Job.ExecutionTimeout.Value), (string) null, jobExecutionTimerCancel.Token);
          }
          CancellationTokenSource timerCancel;
          TaskResult? requestResult;
          while (true)
          {
            Task<string> timerTask = (Task<string>) null;
            timerCancel = new CancellationTokenSource();
            timerTask = !this.m_canceled ? context.CreateTimer<string>(context.CurrentUtcDateTime.Add(TimeSpan.FromMinutes(15.0)), (string) null, timerCancel.Token) : context.CreateTimer<string>(context.CurrentUtcDateTime.Add(this.m_cancelTimeout), (string) null, timerCancel.Token);
            List<Task> taskList = new List<Task>()
            {
              (Task) timerTask,
              (Task) this.m_jobCompleted.Task
            };
            if (!this.m_canceled)
            {
              taskList.Add((Task) this.m_jobCanceled.Task);
              if (jobExecutionTimerTask != null)
                taskList.Add((Task) jobExecutionTimerTask);
            }
            Task task = await Task.WhenAny((IEnumerable<Task>) taskList);
            if (task == this.m_jobCanceled.Task)
            {
              this.m_canceled = true;
              timerCancel.Cancel();
              CanceledEvent canceledEvent = this.m_jobCanceled.Task.Result;
              this.m_cancelTimeout = canceledEvent.Timeout;
              await this.ExecuteAsync(context, input, (Func<Task>) (() => this.Pool.CancelJob(input.PoolId, this.m_agentRequest.RequestId, this.m_agentRequest.ReservedAgent, input.ScopeId, input.PlanId, input.Job.InstanceId, canceledEvent.Timeout)));
              context.TraceAgentJobCancellationSent(input.ScopeId, input.PlanId, input.Job.InstanceId, input.PoolId, this.m_agentRequest.RequestId, this.m_agentRequest.ReservedAgent);
              if (!string.IsNullOrEmpty(canceledEvent.Reason))
              {
                context.Trace(10015564, TraceLevel.Warning, "JobResult := Canceled. Reason: " + canceledEvent.Reason);
                await this.ExecuteAsync(context, input, (Func<Task>) (() => this.Tracker.LogIssue(input.ScopeId, input.PlanId, input.Job.InstanceId, context.CurrentUtcDateTime, IssueType.Error, canceledEvent.Reason)));
              }
            }
            else if (task == jobExecutionTimerTask)
            {
              this.m_timedOut = true;
              this.m_canceled = true;
              timerCancel.Cancel();
              await this.ExecuteAsync(context, input, (Func<Task>) (() => this.Pool.CancelJob(input.PoolId, this.m_agentRequest.RequestId, this.m_agentRequest.ReservedAgent, input.ScopeId, input.PlanId, input.Job.InstanceId, this.m_cancelTimeout)));
              await this.ExecuteAsync(context, input, (Func<Task>) (() => this.Tracker.LogIssue(input.ScopeId, input.PlanId, input.Job.InstanceId, context.CurrentUtcDateTime, IssueType.Error, Resources.JobTimedOut((object) this.m_agentRequest.ReservedAgent.Name, (object) input.Job.ExecutionTimeout))));
            }
            else if (task != this.m_jobCompleted.Task)
            {
              if (task == timerTask)
              {
                requestResult = await this.ExecuteAsync<TaskResult?>(context, input, (Func<Task<TaskResult?>>) (() => this.Pool.GetJobResult(input.PoolId, this.m_agentRequest.RequestId, input.ScopeId, input.PlanId, input.Job.InstanceId)));
                if (this.m_canceled && !requestResult.HasValue)
                {
                  requestResult = new TaskResult?(this.m_timedOut ? TaskResult.Failed : TaskResult.Canceled);
                  await this.ExecuteAsync(context, input, (Func<Task>) (() => this.Tracker.LogIssue(input.ScopeId, input.PlanId, input.Job.InstanceId, context.CurrentUtcDateTime, IssueType.Warning, Resources.JobCancelTimeout((object) this.m_agentRequest.ReservedAgent.Name, (object) this.m_cancelTimeout))));
                }
                if (!requestResult.HasValue)
                  requestResult = new TaskResult?();
                else
                  goto label_42;
              }
            }
            else
              break;
            timerTask = (Task<string>) null;
            timerCancel = (CancellationTokenSource) null;
          }
          timerCancel.Cancel();
          jobResult.Result = this.m_timedOut ? TaskResult.Failed : this.m_jobCompleted.Task.Result;
          goto label_54;
label_42:
          jobResult.Result = requestResult.Value;
        }
      }
      catch (TaskFailedException ex)
      {
        context.TraceJobException(input.ScopeId, input.PlanId, input.Job.InstanceId, (Exception) ex);
        if (!jobStarted)
          jobResult.IsRetryable = ex.InnerException is TaskAgentJobNotFoundException || ex.InnerException is TaskAgentJobTokenExpiredException;
        Type type;
        if (ex.InnerException is AggregateException innerException)
        {
          type = innerException.InnerException?.GetType();
          jobResult.Message = innerException.Flatten().InnerExceptions.Aggregate<Exception, string>(string.Empty, (Func<string, Exception, string>) ((current, e) => current + e.Message + " "));
        }
        else
        {
          type = ex.InnerException?.GetType();
          jobResult.Message = ex.Message;
        }
        errorIsExpected = RunAgentJob7.s_exceptionTypesCausedByUser.Contains(type);
        errorCode = type?.Name;
        jobResult.Result = TaskResult.Failed;
      }
      finally
      {
        if (jobExecutionTimerCancel != null)
        {
          jobExecutionTimerCancel.Cancel();
          jobExecutionTimerCancel = (CancellationTokenSource) null;
        }
      }
label_54:
      if (this.m_agentRequest != null && this.m_agentRequest.ReservedAgent != null)
        context.TraceAgentJobComplete(input.ScopeId, input.PlanId, input.Job.InstanceId, input.PoolId, this.m_agentRequest.RequestId, this.m_agentRequest.ReservedAgent, jobResult.Result);
      if (jobResult.Result == TaskResult.Abandoned && string.IsNullOrEmpty(jobResult.Message) && this.m_agentRequest.ReservedAgent != null)
      {
        errorIsExpected = true;
        errorCode = "Abandoned";
        jobResult.Message = Resources.JobAbandoned((object) this.m_agentRequest.ReservedAgent.Name);
      }
      if (this.m_agentRequest != null)
      {
        context.TraceStartLinearPhase("CIPlatform", "ReleaseAgent");
        try
        {
          await this.ExecuteAsync(context, input, (Func<Task>) (() => this.Pool.DeleteAgentRequest(input.PoolId, this.m_agentRequest.RequestId, input.ScopeId, input.PlanId, input.Job.InstanceId, jobResult.Result)), 10);
          if (this.m_agentRequest.ReservedAgent != null)
            context.TraceAgentReleased(input.ScopeId, input.PlanId, input.Job.InstanceId, input.PoolId, this.m_agentRequest.RequestId, this.m_agentRequest.ReservedAgent);
        }
        catch (Exception ex)
        {
          if (this.m_agentRequest.ReservedAgent != null)
            context.TraceJobError(10015548, this.m_input.ScopeId, this.m_input.PlanId, this.m_input.Job.InstanceId, "Exception while deleting agent request  {0} ({1}) of pool {2}", (object) this.m_agentRequest.ReservedAgent.Name, (object) this.m_agentRequest.ReservedAgent.Id, (object) input.PoolId);
          context.TraceJobException(input.ScopeId, input.PlanId, input.Job.InstanceId, ex);
        }
      }
      if (errorCode == null)
        context.TraceCompleteLinearOrchestration("CIPlatform", "JobCompleted");
      else
        context.TraceCompleteLinearOrchestrationWithError("CIPlatform", "JobCompleted", errorCode, jobResult.Message, errorIsExpected);
      JobResult jobResult1 = jobResult;
      errorCode = (string) null;
      jobExecutionTimerTask = (Task<string>) null;
      jobExecutionTimerCancel = (CancellationTokenSource) null;
      return jobResult1;
    }

    private void EnsureExtensions(OrchestrationContext context)
    {
      this.Pool = context.CreateClient<IAgentPoolExtension7>(true);
      this.Tracker = context.CreateClient<IPlanTrackingExtension3>(true);
      this.PlanController = context.CreateClient<IPlanControlExtension>(true);
    }

    private Task ExecuteAsync(
      OrchestrationContext context,
      RunJobInput4 input,
      Func<Task> operation,
      int maxAttempts = 5)
    {
      return context.ExecuteAsync(operation, maxAttempts, canRetry: this.RetryException, traceException: (Action<Exception>) (ex => context.TraceJobException(input.ScopeId, input.PlanId, input.Job.InstanceId, ex)));
    }

    private Task<T> ExecuteAsync<T>(
      OrchestrationContext context,
      RunJobInput4 input,
      Func<Task<T>> operation,
      int maxAttempts = 5)
    {
      return context.ExecuteAsync<T>(operation, maxAttempts, canRetry: this.RetryException, traceException: (Action<Exception>) (ex => context.TraceJobException(input.ScopeId, input.PlanId, input.Job.InstanceId, ex)));
    }
  }
}
