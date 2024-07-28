// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks.RunPlanBase`1
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: ACF674F2-B05D-403A-A061-F4792BD3317C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.VisualStudio.Services.Orchestration;
using System;
using System.Globalization;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks
{
  public abstract class RunPlanBase<TInput> : TaskOrchestration<TaskResult, TInput, object, string> where TInput : IRunPlanInput
  {
    public Func<Exception, bool> RetryException { get; set; }

    protected IJobControlExtension3 JobController { get; set; }

    protected IPlanTrackingExtension3 PlanTracker { get; set; }

    protected abstract TaskCompletionSource<CanceledEvent> CancellationSource { get; }

    protected RunState RunState { get; set; }

    protected abstract string RunAgentJobVersion { get; }

    protected abstract string RunServerJobVersion { get; }

    protected virtual async Task<TaskResult> ExecuteJobAsync(
      OrchestrationContext context,
      TInput input,
      TaskOrchestrationJob job,
      bool continueOnError)
    {
      string message = (string) null;
      bool jobExecuted = false;
      TaskResult result = TaskResult.Succeeded;
      string jobInstanceId = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0:N}_{1:N}", (object) input.PlanId, (object) job.InstanceId);
      try
      {
        switch (this.RunState)
        {
          case RunState.Canceled:
            result = TaskResult.Canceled;
            goto label_20;
          case RunState.Failed:
            if (!continueOnError)
            {
              result = TaskResult.Failed;
              goto label_20;
            }
            else
              break;
        }
        jobExecuted = true;
        if (!this.CanRunJob(context, input, job))
        {
          result = TaskResult.Skipped;
        }
        else
        {
          for (int attempt = 0; attempt < 2; ++attempt)
          {
            Task<JobResult> jobTask = this.CreateSubOrchestrationInstance(context, job, jobInstanceId, input, attempt);
            if (await Task.WhenAny((Task) this.CancellationSource.Task, (Task) jobTask) == this.CancellationSource.Task)
            {
              CanceledEvent canceledEvent = this.CancellationSource.Task.Result;
              // ISSUE: reference to a compiler-generated field
              // ISSUE: reference to a compiler-generated field
              await context.ExecuteAsync((Func<Task>) (() => this.CS\u0024\u003C\u003E8__locals1.\u003C\u003E4__this.JobController.CancelJob(jobInstanceId, canceledEvent.Timeout, canceledEvent.Reason)), canRetry: this.RetryException, traceException: closure_4 ?? (closure_4 = (Action<Exception>) (ex => context.TracePlanException(input.ScopeId, input.PlanId, ex))));
            }
            JobResult jobResult = await jobTask;
            result = jobResult.Result;
            message = jobResult.Message;
            if (jobResult.IsRetryable)
            {
              if (this.RunState != RunState.Canceled)
              {
                if (jobResult.Result != TaskResult.Failed)
                {
                  if (jobResult.Result != TaskResult.Abandoned)
                    break;
                }
              }
              else
                break;
            }
            else
              break;
          }
        }
      }
      catch (SubOrchestrationFailedException ex)
      {
        context.TracePlanException(input.ScopeId, input.PlanId, (Exception) ex);
        message = ex.InnerException == null ? ex.Message : ex.InnerException.Message;
        result = TaskResult.Failed;
      }
label_20:
      context.TraceJobCompleted(input.ScopeId, input.PlanId, job.InstanceId, result);
      if (!string.IsNullOrEmpty(message))
        await this.LogIssue(context, input, job, IssueType.Error, message);
      this.OnJobComplete(context, jobExecuted, result);
      return result;
    }

    public virtual string GetServerJobOrchestratorName() => "RunServerJob";

    private Task<JobResult> CreateSubOrchestrationInstance(
      OrchestrationContext context,
      TaskOrchestrationJob job,
      string jobInstanceId,
      TInput planInput,
      int attempt)
    {
      if (job.ExecutionMode == "Server")
      {
        RunServerJobInput serverJobInput = this.GetServerJobInput(context, planInput, job, attempt);
        return context.CreateSubOrchestrationInstance<JobResult>(this.GetServerJobOrchestratorName(), this.RunServerJobVersion, jobInstanceId, (object) serverJobInput);
      }
      RunJobInput4 agentJobInput = this.GetAgentJobInput(context, planInput, job);
      return context.CreateSubOrchestrationInstance<JobResult>("RunAgentJob", this.RunAgentJobVersion, jobInstanceId, (object) agentJobInput);
    }

    protected virtual bool CanRunJob(
      OrchestrationContext context,
      TInput input,
      TaskOrchestrationJob job)
    {
      return this.CanRunJob(context, input);
    }

    protected virtual bool CanRunJob(OrchestrationContext context, TInput input) => true;

    protected virtual RunJobInput4 GetAgentJobInput(
      OrchestrationContext context,
      TInput input,
      TaskOrchestrationJob job)
    {
      return new RunJobInput4()
      {
        PoolId = input.PoolId,
        ScopeId = input.ScopeId,
        PlanId = input.PlanId,
        Job = job,
        JobAttempt = (TaskOrchestrationJobAttempt) null,
        NotifyJobAssigned = false
      };
    }

    protected RunServerJobInput GetServerJobInput(
      OrchestrationContext context,
      TInput input,
      TaskOrchestrationJob job,
      int attempt)
    {
      return new RunServerJobInput()
      {
        ScopeId = input.ScopeId,
        PlanId = input.PlanId,
        Job = job
      };
    }

    protected virtual void OnJobComplete(
      OrchestrationContext context,
      bool jobExecuted,
      TaskResult result)
    {
    }

    protected Task<TResult> ExecuteAsync<TResult>(
      OrchestrationContext context,
      TInput input,
      Func<Task<TResult>> operation)
    {
      return context.ExecuteAsync<TResult>(operation, canRetry: this.RetryException, traceException: (Action<Exception>) (ex => context.TracePlanException(input.ScopeId, input.PlanId, ex)));
    }

    protected Task ExecuteAsync(OrchestrationContext context, TInput input, Func<Task> operation) => context.ExecuteAsync(operation, canRetry: this.RetryException, traceException: (Action<Exception>) (ex => context.TracePlanException(input.ScopeId, input.PlanId, ex)));

    protected async Task LogIssue(
      OrchestrationContext context,
      TInput input,
      TaskOrchestrationJob job,
      IssueType issueType,
      string message)
    {
      try
      {
        // ISSUE: reference to a compiler-generated field
        await this.ExecuteAsync(context, input, (Func<Task>) (() => this.\u003C\u003E4__this.PlanTracker.LogIssue(input.ScopeId, input.PlanId, job.InstanceId, context.CurrentUtcDateTime, issueType, message)));
      }
      catch (TaskFailedException ex)
      {
      }
    }
  }
}
