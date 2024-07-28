// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks.RunServerJob
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: ACF674F2-B05D-403A-A061-F4792BD3317C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.VisualStudio.Services.Orchestration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks
{
  internal sealed class RunServerJob : 
    TaskOrchestration<JobResult, RunServerJobInput, object, string>
  {
    private RunServerJobInput m_input;
    private Dictionary<string, JobEventConfig> m_jobEventsConfiguration;
    private TaskCompletionSource<CanceledEvent> m_jobCanceled = new TaskCompletionSource<CanceledEvent>();
    private TaskCompletionSource<bool> m_jobAssigned = new TaskCompletionSource<bool>();
    private TaskCompletionSource<bool> m_jobStarted = new TaskCompletionSource<bool>();
    private TaskCompletionSource<TaskResult> m_jobCompleted = new TaskCompletionSource<TaskResult>();

    public IServerJobTrackingExtension Tracker { get; private set; }

    public IServerJobHandlerExtension JobExecutionHandler { get; private set; }

    public Func<Exception, bool> RetryException { get; set; }

    public override void OnEvent(OrchestrationContext context, string name, object input)
    {
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

    public override async Task<JobResult> RunTask(
      OrchestrationContext context,
      RunServerJobInput input)
    {
      this.EnsureExtensions(context);
      this.m_input = input;
      JobResult jobResult = new JobResult()
      {
        Result = TaskResult.Succeeded
      };
      bool jobAssigned = false;
      try
      {
        context.TraceServerJobRequesting(input.ScopeId, input.PlanId, input.Job.InstanceId);
        SendJobResponse sendJobResponse = await this.ExecuteAsync<SendJobResponse>(context, this.m_input, (Func<Task<SendJobResponse>>) (() => this.JobExecutionHandler.SendJob(input.ScopeId, input.PlanId, input.Job.InstanceId)));
        context.TraceServerJobRequestSent(input.ScopeId, input.PlanId, input.Job.InstanceId);
        this.m_jobEventsConfiguration = sendJobResponse.Events.All;
        Queue<string> waitQueue = new Queue<string>();
        waitQueue.Enqueue("JobAssigned");
        waitQueue.Enqueue("JobStarted");
        waitQueue.Enqueue("JobCompleted");
        CancellationTokenSource timerCancel;
        TimeSpan? timeSpan;
        string eventName;
        while (true)
        {
          if (waitQueue.Count >= 1)
          {
            List<Task> taskList = new List<Task>();
            taskList.Add((Task) this.m_jobCompleted.Task);
            taskList.Add((Task) this.m_jobCanceled.Task);
            eventName = waitQueue.Dequeue();
            RunServerJob.WaitEventData waitEventDetails = this.GetWaitEventDetails(context, eventName);
            taskList.Add(waitEventDetails.WaitTask);
            timerCancel = new CancellationTokenSource();
            Task<string> timerTask = (Task<string>) null;
            timeSpan = waitEventDetails.Timeout;
            if (timeSpan.HasValue)
            {
              timerTask = context.CreateTimer<string>(context.CurrentUtcDateTime.Add(timeSpan.Value), (string) null, timerCancel.Token);
              taskList.Add((Task) timerTask);
            }
            context.TraceServerJobWaiting(input.ScopeId, input.PlanId, input.Job.InstanceId, eventName, timeSpan.HasValue ? timeSpan.ToString() : "Nil");
            Task task = await Task.WhenAny((IEnumerable<Task>) taskList);
            if (task != this.m_jobCompleted.Task)
            {
              if (task != this.m_jobCanceled.Task)
              {
                if (task != timerTask)
                {
                  if (task == this.m_jobAssigned.Task)
                  {
                    timerCancel.Cancel();
                    jobAssigned = true;
                    context.TraceServerJobAssigned(input.ScopeId, input.PlanId, input.Job.InstanceId);
                  }
                  else if (task == this.m_jobStarted.Task)
                  {
                    timerCancel.Cancel();
                    await this.ExecuteAsync(context, this.m_input, closure_7 ?? (closure_7 = (Func<Task>) (() => this.Tracker.JobStarted(input.ScopeId, input.PlanId, input.Job.InstanceId))));
                    context.TraceServerJobStarted(input.ScopeId, input.PlanId, input.Job.InstanceId);
                  }
                  timerCancel = (CancellationTokenSource) null;
                  timerTask = (Task<string>) null;
                }
                else
                  goto label_16;
              }
              else
                goto label_13;
            }
            else
              goto label_9;
          }
          else
            break;
        }
        context.TraceJobError(10015551, this.m_input.ScopeId, this.m_input.PlanId, this.m_input.Job.InstanceId, "Wait queue is empty and orchestration is not terminated");
        goto label_25;
label_9:
        if (!jobAssigned)
          context.TraceJobError(10015550, this.m_input.ScopeId, this.m_input.PlanId, this.m_input.Job.InstanceId, "Received job completed event with result {0} without receiving the assigned event", (object) jobResult.Result);
        timerCancel.Cancel();
        jobResult.Result = this.m_jobCompleted.Task.Result;
        goto label_25;
label_13:
        timerCancel.Cancel();
        CanceledEvent result = this.m_jobCanceled.Task.Result;
        jobResult.Result = TaskResult.Canceled;
        jobResult.Message = result.Reason;
        context.TraceServerJobCancellationSending(input.ScopeId, input.PlanId, input.Job.InstanceId);
        await this.ExecuteAsync(context, this.m_input, closure_5 ?? (closure_5 = (Func<Task>) (() => this.JobExecutionHandler.CancelJob(input.ScopeId, input.PlanId, input.Job.InstanceId))));
        context.TraceServerJobCancellationSent(input.ScopeId, input.PlanId, input.Job.InstanceId);
        goto label_25;
label_16:
        context.TraceTimedOutWaitingForEvent(input.ScopeId, input.PlanId, input.Job.InstanceId, eventName, timeSpan.ToString());
        jobResult.Result = TaskResult.Failed;
        context.TraceServerJobCancellationSending(input.ScopeId, input.PlanId, input.Job.InstanceId);
        await this.ExecuteAsync(context, this.m_input, closure_6 ?? (closure_6 = (Func<Task>) (() => this.JobExecutionHandler.CancelJob(input.ScopeId, input.PlanId, input.Job.InstanceId))));
        await this.ExecuteAsync(context, this.m_input, (Func<Task>) (() => this.Tracker.LogIssue(input.ScopeId, input.PlanId, input.Job.InstanceId, context.CurrentUtcDateTime, IssueType.Error, Resources.JobTimedOutWaitingForEvent((object) timeSpan.ToString(), (object) eventName))));
        context.TraceServerJobCancellationSent(input.ScopeId, input.PlanId, input.Job.InstanceId);
label_25:
        waitQueue = (Queue<string>) null;
      }
      catch (TaskFailedException ex)
      {
        context.TraceJobException(input.ScopeId, input.PlanId, input.Job.InstanceId, (Exception) ex);
        jobResult.Result = TaskResult.Failed;
        if (!jobAssigned && ex.InnerException is ServerJobFailureException)
          jobResult.IsRetryable = true;
        jobResult.Message = !(ex.InnerException is AggregateException) ? ex.Message : (ex.InnerException as AggregateException).Flatten().InnerExceptions.Aggregate<Exception, string>(string.Empty, (Func<string, Exception, string>) ((current, e) => current + e.Message + " "));
      }
      await this.ExecuteAsync(context, this.m_input, (Func<Task>) (() => this.Tracker.JobCompleted(input.ScopeId, input.PlanId, input.Job.InstanceId)));
      context.TraceServerJobCompleted(input.ScopeId, input.PlanId, input.Job.InstanceId, jobResult.Result);
      JobResult jobResult1 = jobResult;
      jobResult = (JobResult) null;
      return jobResult1;
    }

    private RunServerJob.WaitEventData GetWaitEventDetails(
      OrchestrationContext context,
      string eventName)
    {
      TimeSpan? nullable = new TimeSpan?();
      JobEventConfig jobEventConfig = (JobEventConfig) null;
      bool isConfigured = this.m_jobEventsConfiguration.TryGetValue(eventName, out jobEventConfig);
      if (isConfigured)
      {
        TimeSpan result;
        if (!TimeSpan.TryParse(jobEventConfig.Timeout, out result))
          context.TraceJobError(10015550, this.m_input.ScopeId, this.m_input.PlanId, this.m_input.Job.InstanceId, "Unable to parse timeout: {0} for event: {1}", (object) jobEventConfig.Timeout, (object) eventName);
        else if (result > TimeSpan.Zero && result < TimeSpan.FromMilliseconds((double) int.MaxValue))
          nullable = new TimeSpan?(result);
      }
      Task task;
      switch (eventName)
      {
        case "JobCanceled":
          task = (Task) this.m_jobCanceled.Task;
          break;
        case "JobAssigned":
          if (!isConfigured)
            this.m_jobAssigned.TrySetResult(true);
          task = (Task) this.m_jobAssigned.Task;
          break;
        case "JobStarted":
          if (!isConfigured)
            this.m_jobStarted.TrySetResult(true);
          task = (Task) this.m_jobStarted.Task;
          break;
        case "JobCompleted":
          if (!isConfigured)
            this.m_jobCompleted.TrySetResult(TaskResult.Succeeded);
          task = (Task) this.m_jobCompleted.Task;
          break;
        default:
          throw new InvalidOperationException("Not known job event type");
      }
      context.TraceEventConfig(this.m_input.ScopeId, this.m_input.PlanId, this.m_input.Job.InstanceId, eventName, nullable.HasValue ? nullable.ToString() : "Nil", isConfigured);
      return new RunServerJob.WaitEventData()
      {
        WaitTask = task,
        Timeout = nullable
      };
    }

    private Task ExecuteAsync(
      OrchestrationContext context,
      RunServerJobInput input,
      Func<Task> operation,
      int maxAttempts = 5)
    {
      return context.ExecuteAsync(operation, maxAttempts, canRetry: this.RetryException, traceException: (Action<Exception>) (ex => context.TraceJobException(input.ScopeId, input.PlanId, input.Job.InstanceId, ex)));
    }

    private Task<T> ExecuteAsync<T>(
      OrchestrationContext context,
      RunServerJobInput input,
      Func<Task<T>> operation,
      int maxAttempts = 5)
    {
      return context.ExecuteAsync<T>(operation, maxAttempts, canRetry: this.RetryException, traceException: (Action<Exception>) (ex => context.TraceJobException(input.ScopeId, input.PlanId, input.Job.InstanceId, ex)));
    }

    private void EnsureExtensions(OrchestrationContext context)
    {
      this.JobExecutionHandler = context.CreateClient<IServerJobHandlerExtension>(true);
      this.Tracker = context.CreateClient<IServerJobTrackingExtension>(true);
    }

    private struct WaitEventData
    {
      internal Task WaitTask { get; set; }

      internal TimeSpan? Timeout { get; set; }
    }
  }
}
