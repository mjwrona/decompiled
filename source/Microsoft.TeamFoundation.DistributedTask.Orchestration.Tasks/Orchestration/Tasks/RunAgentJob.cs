// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks.RunAgentJob
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: ACF674F2-B05D-403A-A061-F4792BD3317C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.VisualStudio.Services.Orchestration;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks
{
  internal class RunAgentJob : TaskOrchestration<TaskResult, RunJobInput, object, string>
  {
    protected bool m_canceled;
    protected TimeSpan m_timeout;
    protected TaskCompletionSource<object> m_jobCanceled = new TaskCompletionSource<object>();
    protected TaskCompletionSource<TaskResult> m_jobCompleted = new TaskCompletionSource<TaskResult>();
    protected TaskCompletionSource<TaskAgentJobRequest> m_jobAssigned = new TaskCompletionSource<TaskAgentJobRequest>();

    public IAgentPoolExtension Pool { get; protected set; }

    public IPlanTrackingExtension Tracker { get; protected set; }

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
          this.m_canceled = true;
          TimeSpan result;
          this.m_timeout = !TimeSpan.TryParse(input.ToString(), out result) ? TimeSpan.FromSeconds(60.0) : result;
          this.m_jobCanceled.TrySetResult((object) null);
          break;
      }
    }

    public override async Task<TaskResult> RunTask(OrchestrationContext context, RunJobInput input)
    {
      this.EnsureExtensions(context);
      string errorMessage = (string) null;
      TaskAgentJobRequest reservation = (TaskAgentJobRequest) null;
      TaskResult result = TaskResult.Succeeded;
      try
      {
        context.TraceAgentRequesting(input.HostId, input.PlanId, input.Job.InstanceId, input.PoolId, (IList<Demand>) input.Job.Demands);
        reservation = await this.ExecuteAsync<TaskAgentJobRequest>(context, input, (Func<Task<TaskAgentJobRequest>>) (() => this.Pool.QueueJob(input.PoolId, (IList<Demand>) input.Job.Demands, input.HostId, input.PlanId, input.Job.InstanceId, context.OrchestrationInstance.InstanceId)));
        if (reservation.ReservedAgent == null)
        {
          context.TraceAgentWaiting(input.HostId, input.PlanId, input.Job.InstanceId, input.PoolId, reservation.RequestId);
          await this.Tracker.LogMessage(input.HostId, input.PlanId, input.Job.InstanceId, context.CurrentUtcDateTime, TraceLevel.Info, "Waiting for an available agent");
          if (await Task.WhenAny((Task) this.m_jobAssigned.Task, (Task) this.m_jobCanceled.Task) == this.m_jobAssigned.Task)
            reservation = this.m_jobAssigned.Task.Result;
          else
            result = TaskResult.Canceled;
        }
        if (reservation.ReservedAgent != null)
        {
          context.TraceAgentAssigned(input.HostId, input.PlanId, input.Job.InstanceId, input.PoolId, reservation.RequestId, reservation.ReservedAgent);
          context.TraceAgentJobSending(input.HostId, input.PlanId, input.Job.InstanceId, input.PoolId, reservation.RequestId, reservation.ReservedAgent);
          await this.ExecuteAsync(context, input, (Func<Task>) (() => this.Pool.StartJob(input.PoolId, reservation.RequestId, input.HostId, input.PlanId, input.Job.InstanceId, input.Environment)));
          context.TraceAgentJobSent(input.HostId, input.PlanId, input.Job.InstanceId, input.PoolId, reservation.RequestId, reservation.ReservedAgent);
          bool cancelMessageSent = false;
          CancellationTokenSource timerCancel;
          while (true)
          {
            Task<string> timerTask = (Task<string>) null;
            timerCancel = new CancellationTokenSource();
            timerTask = !cancelMessageSent ? context.CreateTimer<string>(context.CurrentUtcDateTime.Add(TimeSpan.FromMinutes(5.0)), (string) null, timerCancel.Token) : context.CreateTimer<string>(context.CurrentUtcDateTime.Add(this.m_timeout), (string) null, timerCancel.Token);
            List<Task> taskList = new List<Task>()
            {
              (Task) timerTask,
              (Task) this.m_jobCompleted.Task
            };
            if (!this.m_canceled)
              taskList.Add((Task) this.m_jobCanceled.Task);
            Task task = await Task.WhenAny((IEnumerable<Task>) taskList);
            if (task == this.m_jobCanceled.Task)
            {
              await this.ExecuteAsync(context, input, (Func<Task>) (() => this.Pool.CancelJob(input.PoolId, reservation.RequestId, input.HostId, input.PlanId, input.Job.InstanceId, this.m_timeout)));
              cancelMessageSent = true;
              timerCancel.Cancel();
            }
            else if (task != this.m_jobCompleted.Task)
            {
              if (task == timerTask)
              {
                AbandonJobResult abandonJobResult = await this.ExecuteAsync<AbandonJobResult>(context, input, (Func<Task<AbandonJobResult>>) (() => this.Pool.AbandonJob(input.PoolId, reservation.RequestId, DateTime.MinValue, input.HostId, input.PlanId)));
                if (!abandonJobResult.Abandoned)
                  reservation.LockedUntil = abandonJobResult.ExpirationTime;
                else
                  goto label_20;
              }
            }
            else
              break;
            timerTask = (Task<string>) null;
            timerCancel = (CancellationTokenSource) null;
          }
          timerCancel.Cancel();
          result = this.m_jobCompleted.Task.Result;
          goto label_24;
label_20:
          result = TaskResult.Abandoned;
          errorMessage = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "The job has been abandoned because agent {0} did not renew the lock.", (object) reservation.ReservedAgent.Name);
        }
      }
      catch (TaskFailedException ex)
      {
        context.TraceJobException(input.HostId, input.PlanId, input.Job.InstanceId, (Exception) ex);
        errorMessage = ex.Message;
        result = TaskResult.Failed;
      }
label_24:
      if (reservation != null && reservation.ReservedAgent != null)
        context.TraceAgentJobComplete(input.HostId, input.PlanId, input.Job.InstanceId, input.PoolId, reservation.RequestId, reservation.ReservedAgent, result);
      if (!string.IsNullOrEmpty(errorMessage))
      {
        try
        {
          await this.ExecuteAsync(context, input, (Func<Task>) (() => this.Tracker.LogMessage(input.HostId, input.PlanId, input.Job.InstanceId, context.CurrentUtcDateTime, TraceLevel.Error, errorMessage)), 3);
        }
        catch (TaskFailedException ex)
        {
        }
      }
      if (reservation != null)
      {
        await this.ExecuteAsync(context, input, (Func<Task>) (() => this.Pool.FinishJob(input.PoolId, reservation.RequestId, input.HostId, input.PlanId, input.Job.InstanceId)), 10);
        if (reservation.ReservedAgent != null)
          context.TraceAgentReleased(input.HostId, input.PlanId, input.Job.InstanceId, input.PoolId, reservation.RequestId, reservation.ReservedAgent);
      }
      return result;
    }

    public virtual void EnsureExtensions(OrchestrationContext context)
    {
      this.Pool = context.CreateClient<IAgentPoolExtension>(true);
      this.Tracker = context.CreateClient<IPlanTrackingExtension>(true);
    }

    protected Task ExecuteAsync(
      OrchestrationContext context,
      RunJobInput input,
      Func<Task> operation,
      int maxAttempts = 5)
    {
      return context.ExecuteAsync(operation, maxAttempts, canRetry: this.RetryException, traceException: (Action<Exception>) (ex => context.TraceJobException(input.HostId, input.PlanId, input.Job.InstanceId, ex)));
    }

    protected Task<T> ExecuteAsync<T>(
      OrchestrationContext context,
      RunJobInput input,
      Func<Task<T>> operation,
      int maxAttempts = 5)
    {
      return context.ExecuteAsync<T>(operation, maxAttempts, canRetry: this.RetryException, traceException: (Action<Exception>) (ex => context.TraceJobException(input.HostId, input.PlanId, input.Job.InstanceId, ex)));
    }
  }
}
