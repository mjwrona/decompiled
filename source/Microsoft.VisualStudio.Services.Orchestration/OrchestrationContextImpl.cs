// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Orchestration.OrchestrationContextImpl
// Assembly: Microsoft.VisualStudio.Services.Orchestration, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C0C603F4-BE31-455B-860A-9FD3B046611C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Orchestration.dll

using Microsoft.VisualStudio.Services.Orchestration.Command;
using Microsoft.VisualStudio.Services.Orchestration.History;
using Microsoft.VisualStudio.Services.Orchestration.TaskActivityDispatcherSharding;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Orchestration
{
  internal class OrchestrationContextImpl : OrchestrationContext
  {
    private IDictionary<int, OrchestrationContextImpl.OpenTaskInfo> openTasks;
    private IDictionary<int, OrchestratorAction> orchestratorActionsMap;
    private int idCounter;
    private TaskScheduler taskScheduler;
    private OrchestrationSerializer serializer;
    private OrchestrationCompleteOrchestratorAction continueAsNew;

    public OrchestrationContextImpl(
      OrchestrationRuntimeState orchestrationRuntimeState,
      TaskScheduler taskScheduler,
      OrchestrationSerializer serializer,
      IActivityShardLocator activityShardLocator,
      IOrchestrationTracer tracer)
    {
      this.taskScheduler = taskScheduler;
      this.openTasks = (IDictionary<int, OrchestrationContextImpl.OpenTaskInfo>) new Dictionary<int, OrchestrationContextImpl.OpenTaskInfo>();
      this.orchestratorActionsMap = (IDictionary<int, OrchestratorAction>) new Dictionary<int, OrchestratorAction>();
      this.idCounter = 0;
      this.ActivityShardLocator = activityShardLocator;
      this.Tracer = tracer;
      this.serializer = serializer;
      this.OrchestrationRuntimeState = orchestrationRuntimeState;
      this.IsReplaying = false;
    }

    public IEnumerable<OrchestratorAction> OrchestratorActions => (IEnumerable<OrchestratorAction>) this.orchestratorActionsMap.Values;

    public override TaskScheduler Scheduler => this.taskScheduler;

    public bool HasOpenTasks => this.openTasks.Count > 0;

    public override async Task<TResult> ScheduleTask<TResult>(
      string name,
      string version,
      params object[] parameters)
    {
      return await this.ScheduleTask<TResult>(name, version, (string) null, parameters);
    }

    public override async Task<TResult> ScheduleTask<TResult>(
      string name,
      string version,
      string dispatcherType,
      params object[] parameters)
    {
      return (TResult) await this.ScheduleTaskInternal(name, version, dispatcherType, typeof (TResult), parameters);
    }

    public async Task<object> ScheduleTaskInternal(
      string name,
      string version,
      string dispatcherType,
      Type resultType,
      params object[] parameters)
    {
      OrchestrationContextImpl orchestrationContextImpl1 = this;
      OrchestrationContextImpl orchestrationContextImpl2 = orchestrationContextImpl1;
      int idCounter = orchestrationContextImpl1.idCounter;
      int num = idCounter + 1;
      orchestrationContextImpl2.idCounter = num;
      int key = idCounter;
      string str = orchestrationContextImpl1.serializer.Serialize((object) parameters);
      ScheduleTaskOrchestratorAction orchestratorAction1 = new ScheduleTaskOrchestratorAction();
      orchestratorAction1.Id = key;
      orchestratorAction1.Name = name;
      orchestratorAction1.Version = version;
      orchestratorAction1.DispatcherType = dispatcherType;
      orchestratorAction1.Input = str;
      ScheduleTaskOrchestratorAction orchestratorAction2 = orchestratorAction1;
      orchestrationContextImpl1.orchestratorActionsMap.Add(key, (OrchestratorAction) orchestratorAction2);
      TaskCompletionSource<string> completionSource = new TaskCompletionSource<string>();
      orchestrationContextImpl1.openTasks.Add(key, new OrchestrationContextImpl.OpenTaskInfo()
      {
        Name = name,
        Version = version,
        Result = completionSource
      });
      string task = await completionSource.Task;
      return orchestrationContextImpl1.serializer.Deserialize(task, resultType);
    }

    public override Task<T> CreateSubOrchestrationInstance<T>(
      string name,
      string version,
      string instanceId,
      object input)
    {
      return this.CreateSubOrchestrationInstanceCore<T>(name, version, instanceId, input);
    }

    public override Task<T> CreateSubOrchestrationInstance<T>(
      string name,
      string version,
      object input)
    {
      return this.CreateSubOrchestrationInstanceCore<T>(name, version, (string) null, input);
    }

    private async Task<T> CreateSubOrchestrationInstanceCore<T>(
      string name,
      string version,
      string instanceId,
      object input)
    {
      OrchestrationContextImpl orchestrationContextImpl1 = this;
      OrchestrationContextImpl orchestrationContextImpl2 = orchestrationContextImpl1;
      int idCounter = orchestrationContextImpl1.idCounter;
      int num = idCounter + 1;
      orchestrationContextImpl2.idCounter = num;
      int key = idCounter;
      string str1 = orchestrationContextImpl1.serializer.Serialize(input);
      string str2 = instanceId;
      if (string.IsNullOrEmpty(str2))
        str2 = orchestrationContextImpl1.OrchestrationInstance.ExecutionId + ":" + key.ToString();
      CreateSubOrchestrationAction orchestrationAction1 = new CreateSubOrchestrationAction();
      orchestrationAction1.Id = key;
      orchestrationAction1.InstanceId = str2;
      orchestrationAction1.Name = name;
      orchestrationAction1.Version = version;
      orchestrationAction1.Input = str1;
      CreateSubOrchestrationAction orchestrationAction2 = orchestrationAction1;
      orchestrationContextImpl1.orchestratorActionsMap.Add(key, (OrchestratorAction) orchestrationAction2);
      TaskCompletionSource<string> completionSource = new TaskCompletionSource<string>();
      orchestrationContextImpl1.openTasks.Add(key, new OrchestrationContextImpl.OpenTaskInfo()
      {
        Name = name,
        Version = version,
        Result = completionSource
      });
      string task = await completionSource.Task;
      return orchestrationContextImpl1.serializer.Deserialize<T>(task);
    }

    public override void ContinueAsNew(object input) => this.ContinueAsNew((string) null, input);

    public override void ContinueAsNew(string newVersion, object input) => this.ContinueAsNewCore(newVersion, input);

    private void ContinueAsNewCore(string newVersion, object input)
    {
      string str = this.serializer.Serialize(input);
      this.continueAsNew = new OrchestrationCompleteOrchestratorAction();
      this.continueAsNew.Result = str;
      this.continueAsNew.OrchestrationStatus = OrchestrationStatus.ContinuedAsNew;
      this.continueAsNew.NewVersion = newVersion;
    }

    public override Task<T> CreateTimer<T>(DateTime fireAt, T state) => this.CreateTimer<T>(fireAt, state, CancellationToken.None);

    public override async Task<T> CreateTimer<T>(
      DateTime fireAt,
      T state,
      CancellationToken cancelToken)
    {
      OrchestrationContextImpl orchestrationContextImpl1 = this;
      OrchestrationContextImpl orchestrationContextImpl2 = orchestrationContextImpl1;
      int idCounter = orchestrationContextImpl1.idCounter;
      int num = idCounter + 1;
      orchestrationContextImpl2.idCounter = num;
      int id = idCounter;
      CreateTimerOrchestratorAction orchestratorAction1 = new CreateTimerOrchestratorAction();
      orchestratorAction1.Id = id;
      orchestratorAction1.FireAt = fireAt;
      CreateTimerOrchestratorAction orchestratorAction2 = orchestratorAction1;
      orchestrationContextImpl1.orchestratorActionsMap.Add(id, (OrchestratorAction) orchestratorAction2);
      TaskCompletionSource<string> tcs = new TaskCompletionSource<string>();
      orchestrationContextImpl1.openTasks.Add(id, new OrchestrationContextImpl.OpenTaskInfo()
      {
        Name = (string) null,
        Version = (string) null,
        Result = tcs
      });
      if (cancelToken != CancellationToken.None)
        cancelToken.Register((Action<object>) (s =>
        {
          if (!tcs.TrySetCanceled())
            return;
          this.openTasks.Remove(id);
        }), (object) tcs);
      string task = await tcs.Task;
      return (T) state;
    }

    public void HandleTaskScheduledEvent(TaskScheduledEvent scheduledEvent)
    {
      int eventId = scheduledEvent.EventId;
      if (this.orchestratorActionsMap.ContainsKey(eventId))
        this.orchestratorActionsMap.Remove(eventId);
      else
        throw new NonDeterministicOrchestrationException(scheduledEvent.EventId, string.Format("TaskScheduledEvent: {0} {1} {2} {3}", (object) scheduledEvent.EventId, (object) scheduledEvent.EventType, (object) scheduledEvent.Name, (object) scheduledEvent.Version));
    }

    public void HandleTimerCreatedEvent(TimerCreatedEvent timerCreatedEvent)
    {
      int eventId = timerCreatedEvent.EventId;
      if (eventId == -100)
        return;
      if (!this.orchestratorActionsMap.ContainsKey(eventId))
        throw new NonDeterministicOrchestrationException(timerCreatedEvent.EventId, string.Format("TimerCreatedEvent: {0} {1}", (object) timerCreatedEvent.EventId, (object) timerCreatedEvent.EventType));
      this.orchestratorActionsMap.Remove(eventId);
    }

    public void HandleSubOrchestrationCreatedEvent(
      SubOrchestrationInstanceCreatedEvent subOrchestrationCreateEvent)
    {
      int eventId = subOrchestrationCreateEvent.EventId;
      if (this.orchestratorActionsMap.ContainsKey(eventId))
        this.orchestratorActionsMap.Remove(eventId);
      else
        throw new NonDeterministicOrchestrationException(subOrchestrationCreateEvent.EventId, string.Format("SubOrchestrationInstanceCreatedEvent: {0} {1} {2} {3} {4}", (object) subOrchestrationCreateEvent.EventId, (object) subOrchestrationCreateEvent.EventType, (object) subOrchestrationCreateEvent.Name, (object) subOrchestrationCreateEvent.Version, (object) subOrchestrationCreateEvent.InstanceId));
    }

    public void HandleTaskCompletedEvent(TaskCompletedEvent completedEvent)
    {
      int taskScheduledId = completedEvent.TaskScheduledId;
      if (!this.openTasks.ContainsKey(taskScheduledId))
        return;
      this.openTasks[taskScheduledId].Result.SetResult(completedEvent.Result);
      this.openTasks.Remove(taskScheduledId);
    }

    public void HandleTaskFailedEvent(TaskFailedEvent failedEvent)
    {
      int taskScheduledId = failedEvent.TaskScheduledId;
      if (!this.openTasks.ContainsKey(taskScheduledId))
        return;
      OrchestrationContextImpl.OpenTaskInfo openTask = this.openTasks[taskScheduledId];
      Exception cause = Utils.RetrieveCause(failedEvent.Details, this.serializer);
      TaskFailedException taskFailedException = new TaskFailedException(failedEvent.EventId, taskScheduledId, openTask.Name, openTask.Version, failedEvent.Reason, cause);
      openTask.Result.SetException((Exception) taskFailedException);
      this.openTasks.Remove(taskScheduledId);
    }

    public void HandleSubOrchestrationInstanceCompletedEvent(
      SubOrchestrationInstanceCompletedEvent completedEvent)
    {
      int taskScheduledId = completedEvent.TaskScheduledId;
      if (!this.openTasks.ContainsKey(taskScheduledId))
        return;
      this.openTasks[taskScheduledId].Result.SetResult(completedEvent.Result);
      this.openTasks.Remove(taskScheduledId);
    }

    public void HandleSubOrchestrationInstanceFailedEvent(
      SubOrchestrationInstanceFailedEvent failedEvent)
    {
      int taskScheduledId = failedEvent.TaskScheduledId;
      if (!this.openTasks.ContainsKey(taskScheduledId))
        return;
      OrchestrationContextImpl.OpenTaskInfo openTask = this.openTasks[taskScheduledId];
      Exception cause = Utils.RetrieveCause(failedEvent.Details, this.serializer);
      SubOrchestrationFailedException orchestrationFailedException = new SubOrchestrationFailedException(failedEvent.EventId, taskScheduledId, openTask.Name, openTask.Version, failedEvent.Reason, cause);
      openTask.Result.SetException((Exception) orchestrationFailedException);
      this.openTasks.Remove(taskScheduledId);
    }

    public void HandleTimerFiredEvent(TimerFiredEvent timerFiredEvent)
    {
      int timerId = timerFiredEvent.TimerId;
      if (!this.openTasks.ContainsKey(timerId))
        return;
      this.openTasks[timerId].Result.SetResult(timerFiredEvent.TimerId.ToString());
      this.openTasks.Remove(timerId);
    }

    public void HandleExecutionTerminatedEvent(ExecutionTerminatedEvent terminatedEvent) => this.CompleteOrchestration(terminatedEvent.Input, (string) null, OrchestrationStatus.Terminated);

    public void CompleteOrchestration(string result) => this.CompleteOrchestration(result, (string) null, OrchestrationStatus.Completed);

    public void FailOrchestration(Exception failure)
    {
      if (failure == null)
        throw new ArgumentNullException(nameof (failure));
      this.CompleteOrchestration(failure.Message, !(failure is OrchestrationFailureException failureException) ? string.Format("Unhandled exception while executing orchestration: {0}\n\t{1}", (object) failure, (object) failure.StackTrace) : failureException.Details, OrchestrationStatus.Failed);
    }

    public void CompleteOrchestration(
      string result,
      string details,
      OrchestrationStatus orchestrationStatus)
    {
      int key = this.idCounter++;
      OrchestrationCompleteOrchestratorAction orchestratorAction;
      if (orchestrationStatus == OrchestrationStatus.Completed && this.continueAsNew != null)
      {
        orchestratorAction = this.continueAsNew;
      }
      else
      {
        orchestratorAction = new OrchestrationCompleteOrchestratorAction();
        orchestratorAction.Result = result;
        orchestratorAction.Details = details;
        orchestratorAction.OrchestrationStatus = orchestrationStatus;
      }
      orchestratorAction.Id = key;
      this.orchestratorActionsMap.Add(key, (OrchestratorAction) orchestratorAction);
    }

    private class OpenTaskInfo
    {
      public string Name { get; set; }

      public string Version { get; set; }

      public TaskCompletionSource<string> Result { get; set; }
    }
  }
}
