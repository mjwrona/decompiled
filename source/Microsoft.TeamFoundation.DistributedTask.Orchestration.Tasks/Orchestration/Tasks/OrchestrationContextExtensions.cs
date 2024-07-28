// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks.OrchestrationContextExtensions
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: ACF674F2-B05D-403A-A061-F4792BD3317C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.VisualStudio.Services.Orchestration;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.DistributedTask.Orchestration.Tasks
{
  public static class OrchestrationContextExtensions
  {
    public static Task ExecuteAsync(
      this OrchestrationContext context,
      Func<Task> action,
      int maxAttempts = 5,
      int backoffIntervalInSeconds = 10,
      Func<Exception, bool> canRetry = null,
      Action<Exception> traceException = null)
    {
      Func<Task<object>> action1 = (Func<Task<object>>) (async () =>
      {
        await action();
        object obj;
        return obj;
      });
      return (Task) context.ExecuteAsync<object>(action1, maxAttempts, backoffIntervalInSeconds, canRetry, traceException);
    }

    public static async Task<T> ExecuteAsync<T>(
      this OrchestrationContext context,
      Func<Task<T>> action,
      int maxAttempts = 5,
      int backoffIntervalInSeconds = 10,
      Func<Exception, bool> canRetry = null,
      Action<Exception> traceException = null)
    {
      while (maxAttempts-- > 0)
      {
        T obj;
        try
        {
          obj = await action();
          goto label_11;
        }
        catch (TaskFailedException ex)
        {
          if (traceException != null)
            traceException((Exception) ex);
          if (maxAttempts != 0 && canRetry != null)
          {
            if (canRetry(ex.InnerException))
              goto label_8;
          }
          throw;
        }
label_8:
        object timer = await context.CreateTimer<object>(context.CurrentUtcDateTime.Add(TimeSpan.FromSeconds((double) backoffIntervalInSeconds)), (object) null);
        backoffIntervalInSeconds *= 2;
        continue;
label_11:
        return obj;
      }
      throw new InvalidOperationException("Should never get here");
    }

    internal static void TraceInfo(this OrchestrationContext context, string message) => context.Trace(0, TraceLevel.Info, message);

    internal static void TraceError(this OrchestrationContext context, string message) => context.TraceError(0, message);

    internal static void TraceError(
      this OrchestrationContext context,
      int tracepoint,
      string message)
    {
      context.Trace(tracepoint, TraceLevel.Error, message);
    }

    internal static void TraceException(this OrchestrationContext context, Exception exception)
    {
      switch (exception)
      {
        case AggregateException _:
          using (IEnumerator<Exception> enumerator = ((AggregateException) exception).Flatten().InnerExceptions.GetEnumerator())
          {
            while (enumerator.MoveNext())
            {
              Exception current = enumerator.Current;
              if (current is TaskFailedException && current.InnerException != null)
                context.TraceError(current.InnerException.ToString());
              else
                context.TraceError(current.ToString());
            }
            return;
          }
        case TaskFailedException _:
          if (exception.InnerException != null)
          {
            context.TraceError(exception.InnerException.ToString());
            return;
          }
          break;
      }
      context.TraceError(exception.ToString());
    }

    internal static void TraceAgentRequesting(
      this OrchestrationContext context,
      Guid hostId,
      Guid planId,
      Guid jobId,
      int poolId)
    {
      context.Trace(0, TraceLevel.Info, "AgentJob {0} - Requesting an agent in pool {1}", (object) jobId, (object) poolId);
    }

    internal static void TraceAgentRequesting(
      this OrchestrationContext context,
      Guid hostId,
      Guid planId,
      Guid jobId,
      int poolId,
      IList<Demand> demands)
    {
      context.Trace(0, TraceLevel.Info, "AgentJob {0} - Requesting an agent in pool {1} with capabilities {2}", (object) jobId, (object) poolId, (object) string.Join<Demand>(", ", (IEnumerable<Demand>) demands));
    }

    internal static void TraceAgentWaiting(
      this OrchestrationContext context,
      Guid hostId,
      Guid planId,
      Guid jobId,
      int poolId,
      long reservationId)
    {
      context.Trace(0, TraceLevel.Info, "AgentJob {0} - Waiting for an agent in pool {1} with reservation {2}", (object) jobId, (object) poolId, (object) reservationId);
    }

    internal static void TraceAgentAssigned(
      this OrchestrationContext context,
      Guid hostId,
      Guid planId,
      Guid jobId,
      int poolId,
      long reservationId,
      TaskAgentReference reservedAgent)
    {
      context.Trace(0, TraceLevel.Info, "AgentJob {0} - Received agent {1} ({2}) in pool {3} for reservation {4}", (object) jobId, (object) reservedAgent.Id, (object) reservedAgent.Name, (object) poolId, (object) reservationId);
    }

    internal static void TraceAgentJobSending(
      this OrchestrationContext context,
      Guid hostId,
      Guid planId,
      Guid jobId,
      int poolId,
      long reservationId,
      TaskAgentReference reservedAgent)
    {
      context.Trace(0, TraceLevel.Info, "AgentJob {0} - Sending job request to agent {1} ({2}) in pool {3} for reservation {4}", (object) jobId, (object) reservedAgent.Id, (object) reservedAgent.Name, (object) poolId, (object) reservationId);
    }

    internal static void TraceAgentJobSent(
      this OrchestrationContext context,
      Guid hostId,
      Guid planId,
      Guid jobId,
      int poolId,
      long reservationId,
      TaskAgentReference reservedAgent)
    {
      context.Trace(0, TraceLevel.Info, "AgentJob {0} - Successfully sent the job request to agent {1} ({2}) in pool {3} for reservation {4}", (object) jobId, (object) reservedAgent.Id, (object) reservedAgent.Name, (object) poolId, (object) reservationId);
    }

    internal static void TraceAgentJobComplete(
      this OrchestrationContext context,
      Guid hostId,
      Guid planId,
      Guid jobId,
      int poolId,
      long reservationId,
      TaskAgentReference reservedAgent,
      TaskResult result)
    {
      context.Trace(0, TraceLevel.Info, "AgentJob {0} - Received a job complete notification from agent {1} ({2}) in pool {3} for reservation {4} with result {5}", (object) jobId, (object) reservedAgent.Id, (object) reservedAgent.Name, (object) poolId, (object) reservationId, (object) result);
    }

    internal static void TraceAgentJobCancellationSent(
      this OrchestrationContext context,
      Guid hostId,
      Guid planId,
      Guid jobId,
      int poolId,
      long reservationId,
      TaskAgentReference reservedAgent)
    {
      context.Trace(0, TraceLevel.Info, "AgentJob {0} - Sent cancellation to agent {1} ({2}) in pool {3} for reservation {4}", (object) jobId, (object) reservedAgent.Id, (object) reservedAgent.Name, (object) poolId, (object) reservationId);
    }

    internal static void TraceAgentReleased(
      this OrchestrationContext context,
      Guid hostId,
      Guid planId,
      Guid jobId,
      int poolId,
      long reservationId,
      TaskAgentReference reservedAgent)
    {
      context.Trace(0, TraceLevel.Info, "AgentJob {0} - Released agent {1} ({2}) back to pool {3} for reservation {4}", (object) jobId, (object) reservedAgent.Id, (object) reservedAgent.Name, (object) poolId, (object) reservationId);
    }

    internal static void TraceJobScheduled(
      this OrchestrationContext context,
      Guid hostId,
      Guid planId,
      Guid jobId)
    {
      context.Trace(0, TraceLevel.Info, "Scheduled orchestration for job {0}", (object) jobId);
    }

    internal static void TraceJobCompleted(
      this OrchestrationContext context,
      Guid hostId,
      Guid planId,
      Guid jobId,
      TaskResult result)
    {
      context.Trace(0, TraceLevel.Info, "Completed orchestration for job {0} with result {1}", (object) jobId, (object) result);
    }

    internal static void TraceJobCompleted(
      this OrchestrationContext context,
      Guid hostId,
      Guid planId,
      string jobName,
      TaskResult result)
    {
      context.Trace(0, TraceLevel.Info, "Completed orchestration for job {0} with result {1}", (object) jobName, (object) result);
    }

    internal static void TraceServerJobRequesting(
      this OrchestrationContext context,
      Guid hostId,
      Guid planId,
      Guid jobId)
    {
      context.TraceServerJobRequesting(hostId, planId, jobId.ToString("D"));
    }

    internal static void TraceServerJobRequesting(
      this OrchestrationContext context,
      Guid hostId,
      Guid planId,
      string jobId)
    {
      context.Trace(0, TraceLevel.Info, "ServerJob {0} - Sending job request", (object) jobId);
    }

    internal static void TraceServerJobRequestSent(
      this OrchestrationContext context,
      Guid hostId,
      Guid planId,
      Guid jobId)
    {
      context.TraceServerJobRequestSent(hostId, planId, jobId.ToString("D"));
    }

    internal static void TraceServerJobRequestSent(
      this OrchestrationContext context,
      Guid hostId,
      Guid planId,
      string jobId)
    {
      context.Trace(0, TraceLevel.Info, "ServerJob {0} - Successfully sent the job request", (object) jobId);
    }

    internal static void TraceServerTaskRequesting(
      this OrchestrationContext context,
      Guid hostId,
      Guid planId,
      Guid jobId,
      Guid taskId)
    {
      context.TraceServerTaskRequesting(hostId, planId, jobId.ToString("D"), taskId.ToString("D"));
    }

    internal static void TraceServerTaskRequesting(
      this OrchestrationContext context,
      Guid hostId,
      Guid planId,
      string jobId,
      string taskId)
    {
      context.Trace(0, TraceLevel.Info, "ServerJob {0} - Sending task request for task {1}", (object) jobId, (object) taskId);
    }

    internal static void TraceServerTaskRequestSent(
      this OrchestrationContext context,
      Guid hostId,
      Guid planId,
      Guid jobId,
      Guid taskId)
    {
      context.TraceServerTaskRequestSent(hostId, planId, jobId.ToString("D"), taskId.ToString("D"));
    }

    internal static void TraceServerTaskRequestSent(
      this OrchestrationContext context,
      Guid hostId,
      Guid planId,
      string jobId,
      string taskId)
    {
      context.Trace(0, TraceLevel.Info, "ServerJob {0} - Successfully sent the task request for task {1}", (object) jobId, (object) taskId);
    }

    internal static void TraceServerJobCancellationSending(
      this OrchestrationContext context,
      Guid hostId,
      Guid planId,
      Guid jobId)
    {
      context.TraceServerJobCancellationSending(hostId, planId, jobId.ToString("D"));
    }

    internal static void TraceServerJobCancellationSending(
      this OrchestrationContext context,
      Guid hostId,
      Guid planId,
      string jobId)
    {
      context.Trace(0, TraceLevel.Info, "ServerJob {0} - Sending job cancellation request", (object) jobId);
    }

    internal static void TraceServerTaskCancellationSending(
      this OrchestrationContext context,
      Guid hostId,
      Guid planId,
      string jobId,
      string taskId)
    {
      context.Trace(0, TraceLevel.Info, "ServerTask {0}/{1} - Sending job cancellation request", (object) jobId, (object) taskId);
    }

    internal static void TraceServerJobCancellationSent(
      this OrchestrationContext context,
      Guid hostId,
      Guid planId,
      Guid jobId)
    {
      context.TraceServerJobCancellationSent(hostId, planId, jobId.ToString("D"));
    }

    internal static void TraceServerJobCancellationSent(
      this OrchestrationContext context,
      Guid hostId,
      Guid planId,
      string jobId)
    {
      context.Trace(0, TraceLevel.Info, "ServerJob {0} - Successfully sent job cancellation request", (object) jobId);
    }

    internal static void TraceServerTaskCancellationSent(
      this OrchestrationContext context,
      Guid hostId,
      Guid planId,
      string jobId,
      string taskId)
    {
      context.Trace(0, TraceLevel.Info, "ServerTask {0}/{1} - Successfully sent job cancellation request", (object) jobId, (object) taskId);
    }

    internal static void TraceServerJobLocalExecutionCompletedEventReceived(
      this OrchestrationContext context,
      Guid hostId,
      Guid planId,
      Guid jobId)
    {
      context.TraceServerJobLocalExecutionCompletedEventReceived(hostId, planId, jobId.ToString("D"));
    }

    internal static void TraceServerJobLocalExecutionCompletedEventReceived(
      this OrchestrationContext context,
      Guid hostId,
      Guid planId,
      string jobId)
    {
      context.Trace(0, TraceLevel.Info, "ServerJob {0} - Received Task local execution completed event.", (object) jobId);
    }

    internal static void TraceServerTaskLocalExecutionCompletedEventReceived(
      this OrchestrationContext context,
      Guid hostId,
      Guid planId,
      Guid jobId,
      Guid taskId)
    {
      context.TraceServerTaskLocalExecutionCompletedEventReceived(hostId, planId, jobId.ToString("D"), taskId.ToString("D"));
    }

    internal static void TraceServerTaskLocalExecutionCompletedEventReceived(
      this OrchestrationContext context,
      Guid hostId,
      Guid planId,
      string jobId,
      string taskId)
    {
      context.Trace(0, TraceLevel.Info, "ServerJob {0} - Received Task local execution completed event for task {1}.", (object) jobId, (object) taskId);
    }

    internal static void TraceServerJobLocalCancellationCompletedEventReceived(
      this OrchestrationContext context,
      Guid hostId,
      Guid planId,
      Guid jobId)
    {
      context.TraceServerJobLocalCancellationCompletedEventReceived(hostId, planId, jobId.ToString("D"));
    }

    internal static void TraceServerJobLocalCancellationCompletedEventReceived(
      this OrchestrationContext context,
      Guid hostId,
      Guid planId,
      string jobId)
    {
      context.Trace(0, TraceLevel.Info, "ServerJob {0} - Received Task local cancellation completed event.", (object) jobId);
    }

    internal static void TraceServerTaskLocalCancellationCompletedEventReceived(
      this OrchestrationContext context,
      Guid hostId,
      Guid planId,
      string jobId,
      string taskId)
    {
      context.Trace(0, TraceLevel.Info, "ServerTask {0}/{1} - Received Task local cancellation completed event.", (object) jobId, (object) taskId);
    }

    internal static void TraceServerJobAssigned(
      this OrchestrationContext context,
      Guid hostId,
      Guid planId,
      Guid jobId)
    {
      context.TraceServerJobAssigned(hostId, planId, jobId.ToString("D"));
    }

    internal static void TraceServerJobAssigned(
      this OrchestrationContext context,
      Guid hostId,
      Guid planId,
      string jobId)
    {
      context.Trace(0, TraceLevel.Info, "ServerJob {0} - Received job assigned notification", (object) jobId);
    }

    internal static void TraceServerJobCancelTask(
      this OrchestrationContext context,
      Guid hostId,
      Guid planId,
      Guid jobId,
      Guid taskId)
    {
      context.TraceServerJobCancelTask(hostId, planId, jobId.ToString("D"), taskId.ToString("D"));
    }

    internal static void TraceServerJobCancelTask(
      this OrchestrationContext context,
      Guid hostId,
      Guid planId,
      string jobId,
      string taskId)
    {
      context.Trace(0, TraceLevel.Info, "ServerJob {0} - Sent CancelTask notification to task {1}", (object) jobId, (object) taskId);
    }

    internal static void TraceServerJobAssignTask(
      this OrchestrationContext context,
      Guid hostId,
      Guid planId,
      Guid jobId,
      Guid taskId)
    {
      context.TraceServerJobAssignTask(hostId, planId, jobId.ToString("D"), taskId.ToString("D"));
    }

    internal static void TraceServerJobAssignTask(
      this OrchestrationContext context,
      Guid hostId,
      Guid planId,
      string jobId,
      string taskId)
    {
      context.Trace(0, TraceLevel.Info, "ServerJob {0} - Sent AssignTask notification to task {1}", (object) jobId, (object) taskId);
    }

    internal static void TraceServerJobStartTask(
      this OrchestrationContext context,
      Guid hostId,
      Guid planId,
      Guid jobId,
      Guid taskId)
    {
      context.TraceServerJobStartTask(hostId, planId, jobId.ToString("D"), taskId.ToString("D"));
    }

    internal static void TraceServerJobStartTask(
      this OrchestrationContext context,
      Guid hostId,
      Guid planId,
      string jobId,
      string taskId)
    {
      context.Trace(0, TraceLevel.Info, "ServerJob {0} - Sent StartTask notification to task {1}", (object) jobId, (object) taskId);
    }

    internal static void TraceServerJobCompleteTask(
      this OrchestrationContext context,
      Guid hostId,
      Guid planId,
      Guid jobId,
      Guid taskId,
      TaskResult result)
    {
      context.TraceServerJobCompleteTask(hostId, planId, jobId.ToString("D"), taskId.ToString("D"), result);
    }

    internal static void TraceServerJobCompleteTask(
      this OrchestrationContext context,
      Guid hostId,
      Guid planId,
      string jobId,
      string taskId,
      TaskResult result)
    {
      context.Trace(0, TraceLevel.Info, "ServerJob {0} - Sent CompleteTask notification with result {1} to task {2}", (object) jobId, (object) result, (object) taskId);
    }

    internal static void TraceServerTaskAssigned(
      this OrchestrationContext context,
      Guid hostId,
      Guid planId,
      Guid jobId,
      Guid taskId)
    {
      context.TraceServerTaskAssigned(hostId, planId, jobId.ToString("D"), taskId.ToString("D"));
    }

    internal static void TraceServerTaskAssigned(
      this OrchestrationContext context,
      Guid hostId,
      Guid planId,
      string jobId,
      string taskId)
    {
      context.Trace(0, TraceLevel.Info, "ServerTask {0}/{1} - Received task assigned notification", (object) jobId, (object) taskId);
    }

    internal static void TraceServerJobStarted(
      this OrchestrationContext context,
      Guid hostId,
      Guid planId,
      Guid jobId)
    {
      context.TraceServerJobStarted(hostId, planId, jobId.ToString("D"));
    }

    internal static void TraceServerJobStarted(
      this OrchestrationContext context,
      Guid hostId,
      Guid planId,
      string jobId)
    {
      context.Trace(0, TraceLevel.Info, "ServerJob {0} - Received job started notification", (object) jobId);
    }

    internal static void TraceServerTaskStarted(
      this OrchestrationContext context,
      Guid hostId,
      Guid planId,
      Guid jobId,
      Guid taskId)
    {
      context.TraceServerTaskStarted(hostId, planId, jobId.ToString("D"), taskId.ToString("D"));
    }

    internal static void TraceServerTaskStarted(
      this OrchestrationContext context,
      Guid hostId,
      Guid planId,
      string jobId,
      string taskId)
    {
      context.Trace(0, TraceLevel.Info, "ServerTask {0}/{1} - Received task started notification", (object) jobId, (object) taskId);
    }

    internal static void TraceServerJobCompleted(
      this OrchestrationContext context,
      Guid hostId,
      Guid planId,
      Guid jobId,
      TaskResult result)
    {
      context.TraceServerJobCompleted(hostId, planId, jobId.ToString("D"), result);
    }

    internal static void TraceServerJobCompleted(
      this OrchestrationContext context,
      Guid hostId,
      Guid planId,
      string jobId,
      TaskResult result)
    {
      context.Trace(0, TraceLevel.Info, "ServerJob {0} - Received job completed notification with result {1}", (object) jobId, (object) result);
    }

    internal static void TraceServerTaskCompleted(
      this OrchestrationContext context,
      Guid hostId,
      Guid planId,
      Guid jobId,
      Guid taskId,
      TaskResult result)
    {
      context.TraceServerTaskCompleted(hostId, planId, jobId.ToString("D"), taskId.ToString("D"), result);
    }

    internal static void TraceServerTaskCompleted(
      this OrchestrationContext context,
      Guid hostId,
      Guid planId,
      string jobId,
      string taskId,
      TaskResult result)
    {
      context.Trace(0, TraceLevel.Info, "ServerTask {0}/{1} - Received task completed notification with result {2}", (object) jobId, (object) taskId, (object) result);
    }

    internal static void TraceServerTaskWaiting(
      this OrchestrationContext context,
      Guid hostId,
      Guid planId,
      Guid jobId,
      Guid taskId,
      string eventName,
      string timespan)
    {
      context.TraceServerTaskWaiting(hostId, planId, jobId.ToString("D"), taskId.ToString("D"), eventName, timespan);
    }

    internal static void TraceServerTaskWaiting(
      this OrchestrationContext context,
      Guid hostId,
      Guid planId,
      string jobId,
      string taskId,
      string eventName,
      string timespan)
    {
      context.Trace(0, TraceLevel.Info, "ServerTask {0}/{1} - Waiting for event {2} with timeout of {3}", (object) jobId, (object) taskId, (object) eventName, (object) timespan);
    }

    internal static void TraceServerJobWaiting(
      this OrchestrationContext context,
      Guid hostId,
      Guid planId,
      Guid jobId,
      string eventName,
      string timespan)
    {
      context.TraceServerJobWaiting(hostId, planId, jobId.ToString("D"), eventName, timespan);
    }

    internal static void TraceServerJobWaiting(
      this OrchestrationContext context,
      Guid hostId,
      Guid planId,
      string jobId,
      string eventName,
      string timespan)
    {
      context.Trace(0, TraceLevel.Info, "ServerJob {0} - Waiting for event {1} with timeout of {2}", (object) jobId, (object) eventName, (object) timespan);
    }

    internal static void TraceTimedOutWaitingForEvent(
      this OrchestrationContext context,
      Guid hostId,
      Guid planId,
      Guid jobId,
      string eventName,
      string timespan)
    {
      context.TraceTimedOutWaitingForEvent(hostId, planId, jobId.ToString("D"), eventName, timespan);
    }

    internal static void TraceTimedOutWaitingForEvent(
      this OrchestrationContext context,
      Guid hostId,
      Guid planId,
      string jobId,
      string eventName,
      string timespan)
    {
      context.Trace(0, TraceLevel.Info, "ServerJob {0} - Waiting for event {1} timed out after {2}", (object) jobId, (object) eventName, (object) timespan);
    }

    internal static void TraceTimedOutWaitingForEvent(
      this OrchestrationContext context,
      Guid hostId,
      Guid planId,
      string jobId,
      string taskId,
      string eventName,
      string timespan)
    {
      context.Trace(0, TraceLevel.Info, "ServerTask {0}/{1} - Waiting for event {2} timed out after {3}", (object) jobId, (object) taskId, (object) eventName, (object) timespan);
    }

    internal static void TraceEvent(this OrchestrationContext context, string eventName) => context.Trace(0, TraceLevel.Info, "Received event {0}", (object) eventName);

    internal static void TraceEventConfig(
      this OrchestrationContext context,
      Guid hostId,
      Guid planId,
      Guid jobId,
      string eventName,
      string timeout,
      bool isConfigured)
    {
      context.TraceEventConfig(hostId, planId, jobId.ToString("D"), eventName, timeout, isConfigured);
    }

    internal static void TraceEventConfig(
      this OrchestrationContext context,
      Guid hostId,
      Guid planId,
      string jobId,
      string eventName,
      string timeout,
      bool isConfigured)
    {
      context.Trace(0, TraceLevel.Info, "Job {0} - Event config : Name {1}, isConfigured {2}, Timeout {3}", (object) jobId, (object) eventName, (object) isConfigured, (object) timeout);
    }

    public static void TracePlanStarted(
      this OrchestrationContext context,
      Guid hostId,
      Guid planId)
    {
      context.Trace(0, TraceLevel.Info, "Started orchestration for plan");
    }

    public static void TracePlanCompleted(
      this OrchestrationContext context,
      Guid hostId,
      Guid planId,
      TaskResult result)
    {
      context.Trace(0, TraceLevel.Info, "Completed orchestration with result {0}", (object) result);
    }

    internal static void TraceJobError(
      this OrchestrationContext context,
      int tracepoint,
      Guid hostId,
      Guid planId,
      Guid jobId,
      string format,
      params object[] arguments)
    {
      context.TraceJobError(tracepoint, hostId, planId, jobId.ToString("D"), format, arguments);
    }

    internal static void TraceJobError(
      this OrchestrationContext context,
      int tracepoint,
      Guid hostId,
      Guid planId,
      string jobName,
      string format,
      params object[] arguments)
    {
      string str = format;
      if (arguments != null && arguments.Length != 0)
        str = string.Format((IFormatProvider) CultureInfo.InvariantCulture, format, arguments);
      context.Trace(tracepoint, TraceLevel.Error, "Job {0} - {1}", (object) jobName, (object) str);
    }

    internal static void TraceJobInfo(
      this OrchestrationContext context,
      int tracepoint,
      Guid hostId,
      Guid planId,
      Guid jobId,
      string format,
      params object[] arguments)
    {
      context.TraceJobInfo(tracepoint, hostId, planId, jobId.ToString("D"), format, arguments);
    }

    internal static void TraceJobInfo(
      this OrchestrationContext context,
      int tracepoint,
      Guid hostId,
      Guid planId,
      string jobName,
      string format,
      params object[] arguments)
    {
      string str = format;
      if (arguments != null && arguments.Length != 0)
        str = string.Format((IFormatProvider) CultureInfo.InvariantCulture, format, arguments);
      context.Trace(tracepoint, TraceLevel.Info, "Job {0} - {1}", (object) jobName, (object) str);
    }

    internal static void TraceServerTaskError(
      this OrchestrationContext context,
      int tracepoint,
      Guid hostId,
      Guid planId,
      Guid jobId,
      Guid taskId,
      string format,
      params object[] arguments)
    {
      context.TraceJobError(tracepoint, hostId, planId, jobId.ToString("D"), taskId.ToString("D"), (object) format, (object) arguments);
    }

    internal static void TraceServerTaskError(
      this OrchestrationContext context,
      int tracepoint,
      Guid hostId,
      Guid planId,
      string jobName,
      string taskName,
      string format,
      params object[] arguments)
    {
      string str = format;
      if (arguments != null && arguments.Length != 0)
        str = string.Format((IFormatProvider) CultureInfo.InvariantCulture, format, arguments);
      context.Trace(tracepoint, TraceLevel.Error, "ServerTask {0}:{1} - {2}", (object) jobName, (object) taskName, (object) str);
    }

    internal static void TraceJobException(
      this OrchestrationContext context,
      Guid hostId,
      Guid planId,
      Guid jobId,
      Exception exception)
    {
      context.TraceJobException(hostId, planId, jobId.ToString("D"), exception);
    }

    internal static void TraceJobException(
      this OrchestrationContext context,
      Guid hostId,
      Guid planId,
      string jobName,
      Exception exception)
    {
      if (exception is AggregateException)
        exception = ((AggregateException) exception).Flatten().InnerException;
      if (exception is TaskFailedException && exception.InnerException != null)
        exception = exception.InnerException;
      context.Trace(0, TraceLevel.Error, "Job {0} - {1}", (object) jobName, (object) exception.ToString());
    }

    internal static void TraceTaskException(
      this OrchestrationContext context,
      Guid hostId,
      Guid planId,
      Guid jobId,
      Guid taskId,
      Exception exception)
    {
      context.TraceTaskException(hostId, planId, jobId.ToString("D"), taskId.ToString("D"), exception);
    }

    internal static void TraceTaskException(
      this OrchestrationContext context,
      Guid hostId,
      Guid planId,
      string jobName,
      string taskName,
      Exception exception)
    {
      if (exception is AggregateException)
        exception = ((AggregateException) exception).Flatten().InnerException;
      if (exception is TaskFailedException && exception.InnerException != null)
        exception = exception.InnerException;
      context.Trace(0, TraceLevel.Error, "Job {0} - Task {1} - {2}", (object) jobName, (object) taskName, (object) exception.ToString());
    }

    public static void TracePlanInformation(
      this OrchestrationContext context,
      Guid hostId,
      Guid planId,
      string format,
      params object[] arguments)
    {
      context.Trace(0, TraceLevel.Info, format, arguments);
    }

    public static void TracePlanException(
      this OrchestrationContext context,
      Guid hostId,
      Guid planId,
      Exception exception)
    {
      if (exception is AggregateException)
        exception = ((AggregateException) exception).Flatten().InnerException;
      if (exception is TaskFailedException && exception.InnerException != null)
        exception = exception.InnerException;
      if (exception is SubOrchestrationFailedException && exception.InnerException != null)
        exception = exception.InnerException;
      context.Trace(0, TraceLevel.Error, exception.ToString());
    }

    public static void TracePlanError(
      this OrchestrationContext context,
      Guid hostId,
      Guid planId,
      string format,
      params object[] arguments)
    {
      context.Trace(0, TraceLevel.Error, format, arguments);
    }
  }
}
