// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks.RunAgentJob3
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: ACF674F2-B05D-403A-A061-F4792BD3317C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.VisualStudio.Services.Orchestration;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks
{
  internal sealed class RunAgentJob3 : TaskOrchestration<TaskResult, RunJobInput3, object, string>
  {
    private bool m_timedOut;
    private bool m_canceled;
    private TimeSpan m_cancelTimeout = TimeSpan.FromSeconds(60.0);
    private CancellationTokenSource m_jobTimeoutCancel = new CancellationTokenSource();
    private TaskCompletionSource<TaskResult> m_jobCompleted = new TaskCompletionSource<TaskResult>();
    private TaskCompletionSource<CanceledEvent> m_jobCanceled = new TaskCompletionSource<CanceledEvent>();
    private TaskCompletionSource<TaskAgentJobRequest> m_jobAssigned = new TaskCompletionSource<TaskAgentJobRequest>();

    public IAgentPoolExtension3 Pool { get; private set; }

    public IPlanTrackingExtension3 Tracker { get; private set; }

    public Func<Exception, bool> RetryException { get; set; }

    public override void OnEvent(OrchestrationContext context, string name, object input)
    {
      switch (name)
      {
        case "JobAssigned":
          this.m_jobAssigned.TrySetResult(((JobAssignedEvent) input).Request);
          break;
        case "JobCompleted":
          this.m_jobCompleted.TrySetResult(((JobCompletedEvent) input).Result);
          break;
        case "JobCanceled":
          this.m_jobCanceled.TrySetResult((CanceledEvent) input);
          break;
      }
    }

    public override async Task<TaskResult> RunTask(OrchestrationContext context, RunJobInput3 input)
    {
      this.EnsureExtensions(context);
      string errorMessage = (string) null;
      TaskAgentJobRequest reservation = (TaskAgentJobRequest) null;
      TaskResult result = TaskResult.Succeeded;
      Task<string> jobExecutionTimerTask = (Task<string>) null;
      CancellationTokenSource jobExecutionTimerCancel = (CancellationTokenSource) null;
      try
      {
        context.TraceAgentRequesting(input.ScopeId, input.PlanId, input.Job.InstanceId, input.PoolId, (IList<Demand>) input.Job.Demands);
        reservation = await this.ExecuteAsync<TaskAgentJobRequest>(context, input, (Func<Task<TaskAgentJobRequest>>) (() => this.Pool.QueueAgentRequest(input.PoolId, (IList<Demand>) input.Job.Demands, input.ScopeId, input.PlanId, input.Job.InstanceId)));
        if (reservation.ReservedAgent == null)
        {
          context.TraceAgentWaiting(input.ScopeId, input.PlanId, input.Job.InstanceId, input.PoolId, reservation.RequestId);
          if (await Task.WhenAny((Task) this.m_jobAssigned.Task, (Task) this.m_jobCanceled.Task) == this.m_jobAssigned.Task)
          {
            reservation = this.m_jobAssigned.Task.Result;
          }
          else
          {
            result = TaskResult.Canceled;
            CanceledEvent canceledEvent = this.m_jobCanceled.Task.Result;
            if (!string.IsNullOrEmpty(canceledEvent.Reason))
              await this.ExecuteAsync(context, input, (Func<Task>) (() => this.Tracker.LogIssue(input.ScopeId, input.PlanId, input.Job.InstanceId, context.CurrentUtcDateTime, IssueType.Error, canceledEvent.Reason)));
          }
        }
        if (reservation.ReservedAgent != null)
        {
          context.TraceAgentAssigned(input.ScopeId, input.PlanId, input.Job.InstanceId, input.PoolId, reservation.RequestId, reservation.ReservedAgent);
          context.TraceAgentJobSending(input.ScopeId, input.PlanId, input.Job.InstanceId, input.PoolId, reservation.RequestId, reservation.ReservedAgent);
          await this.ExecuteAsync(context, input, (Func<Task>) (() => this.Pool.StartJob(input.PoolId, reservation.RequestId, input.ScopeId, input.PlanId, input.Job.InstanceId)));
          context.TraceAgentJobSent(input.ScopeId, input.PlanId, input.Job.InstanceId, input.PoolId, reservation.RequestId, reservation.ReservedAgent);
          if (input.Job.ExecutionTimeout.HasValue && input.Job.ExecutionTimeout.Value > TimeSpan.Zero && input.Job.ExecutionTimeout.Value < TimeSpan.FromMilliseconds((double) int.MaxValue))
          {
            jobExecutionTimerCancel = new CancellationTokenSource();
            jobExecutionTimerTask = context.CreateTimer<string>(context.CurrentUtcDateTime.Add(input.Job.ExecutionTimeout.Value), (string) null, jobExecutionTimerCancel.Token);
          }
          CancellationTokenSource timerCancel;
          TaskResult? jobResult;
          while (true)
          {
            Task<string> timerTask = (Task<string>) null;
            timerCancel = new CancellationTokenSource();
            timerTask = !this.m_canceled ? context.CreateTimer<string>(context.CurrentUtcDateTime.Add(TimeSpan.FromMinutes(5.0)), (string) null, timerCancel.Token) : context.CreateTimer<string>(context.CurrentUtcDateTime.Add(this.m_cancelTimeout), (string) null, timerCancel.Token);
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
              await this.ExecuteAsync(context, input, (Func<Task>) (() => this.Pool.CancelJob(input.PoolId, reservation.RequestId, input.ScopeId, input.PlanId, input.Job.InstanceId, canceledEvent.Timeout)));
              if (!string.IsNullOrEmpty(canceledEvent.Reason))
                await this.ExecuteAsync(context, input, (Func<Task>) (() => this.Tracker.LogIssue(input.ScopeId, input.PlanId, input.Job.InstanceId, context.CurrentUtcDateTime, IssueType.Error, canceledEvent.Reason)));
            }
            else if (task == jobExecutionTimerTask)
            {
              this.m_timedOut = true;
              this.m_canceled = true;
              timerCancel.Cancel();
              await this.ExecuteAsync(context, input, (Func<Task>) (() => this.Pool.CancelJob(input.PoolId, reservation.RequestId, input.ScopeId, input.PlanId, input.Job.InstanceId, this.m_cancelTimeout)));
              await this.ExecuteAsync(context, input, (Func<Task>) (() => this.Tracker.LogIssue(input.ScopeId, input.PlanId, input.Job.InstanceId, context.CurrentUtcDateTime, IssueType.Error, Resources.JobTimedOut((object) reservation.ReservedAgent.Name, (object) input.Job.ExecutionTimeout))));
            }
            else if (task != this.m_jobCompleted.Task)
            {
              if (task == timerTask)
              {
                jobResult = await this.ExecuteAsync<TaskResult?>(context, input, (Func<Task<TaskResult?>>) (() => this.Pool.GetJobResult(input.PoolId, reservation.RequestId, input.ScopeId, input.PlanId, input.Job.InstanceId)));
                if (this.m_canceled && !jobResult.HasValue)
                {
                  jobResult = new TaskResult?(this.m_timedOut ? TaskResult.Failed : TaskResult.Canceled);
                  await this.ExecuteAsync(context, input, (Func<Task>) (() => this.Tracker.LogIssue(input.ScopeId, input.PlanId, input.Job.InstanceId, context.CurrentUtcDateTime, IssueType.Warning, Resources.JobCancelTimeout((object) reservation.ReservedAgent.Name, (object) this.m_cancelTimeout))));
                }
                if (!jobResult.HasValue)
                  jobResult = new TaskResult?();
                else
                  goto label_32;
              }
            }
            else
              break;
            timerTask = (Task<string>) null;
            timerCancel = (CancellationTokenSource) null;
          }
          timerCancel.Cancel();
          result = this.m_timedOut ? TaskResult.Failed : this.m_jobCompleted.Task.Result;
          goto label_39;
label_32:
          result = jobResult.Value;
        }
      }
      catch (TaskFailedException ex)
      {
        context.TraceJobException(input.ScopeId, input.PlanId, input.Job.InstanceId, (Exception) ex);
        errorMessage = ex.Message;
        result = TaskResult.Failed;
      }
      finally
      {
        if (jobExecutionTimerCancel != null)
        {
          jobExecutionTimerCancel.Cancel();
          jobExecutionTimerCancel = (CancellationTokenSource) null;
        }
      }
label_39:
      if (reservation != null && reservation.ReservedAgent != null)
        context.TraceAgentJobComplete(input.ScopeId, input.PlanId, input.Job.InstanceId, input.PoolId, reservation.RequestId, reservation.ReservedAgent, result);
      if (result == TaskResult.Abandoned && string.IsNullOrEmpty(errorMessage) && reservation.ReservedAgent != null)
        errorMessage = Resources.JobAbandoned((object) reservation.ReservedAgent.Name);
      if (!string.IsNullOrEmpty(errorMessage))
      {
        try
        {
          await this.ExecuteAsync(context, input, (Func<Task>) (() => this.Tracker.LogIssue(input.ScopeId, input.PlanId, input.Job.InstanceId, context.CurrentUtcDateTime, IssueType.Error, errorMessage)), 3);
        }
        catch (TaskFailedException ex)
        {
        }
      }
      if (reservation != null)
      {
        await this.ExecuteAsync(context, input, (Func<Task>) (() => this.Pool.DeleteAgentRequest(input.PoolId, reservation.RequestId, input.ScopeId, input.PlanId, input.Job.InstanceId)), 10);
        if (reservation.ReservedAgent != null)
          context.TraceAgentReleased(input.ScopeId, input.PlanId, input.Job.InstanceId, input.PoolId, reservation.RequestId, reservation.ReservedAgent);
      }
      TaskResult taskResult = result;
      jobExecutionTimerTask = (Task<string>) null;
      jobExecutionTimerCancel = (CancellationTokenSource) null;
      return taskResult;
    }

    private void EnsureExtensions(OrchestrationContext context)
    {
      this.Pool = context.CreateClient<IAgentPoolExtension3>(true);
      this.Tracker = context.CreateClient<IPlanTrackingExtension3>(true);
    }

    private Task ExecuteAsync(
      OrchestrationContext context,
      RunJobInput3 input,
      Func<Task> operation,
      int maxAttempts = 5)
    {
      return context.ExecuteAsync(operation, maxAttempts, canRetry: this.RetryException, traceException: (Action<Exception>) (ex => context.TraceJobException(input.ScopeId, input.PlanId, input.Job.InstanceId, ex)));
    }

    private Task<T> ExecuteAsync<T>(
      OrchestrationContext context,
      RunJobInput3 input,
      Func<Task<T>> operation,
      int maxAttempts = 5)
    {
      return context.ExecuteAsync<T>(operation, maxAttempts, canRetry: this.RetryException, traceException: (Action<Exception>) (ex => context.TraceJobException(input.ScopeId, input.PlanId, input.Job.InstanceId, ex)));
    }
  }
}
