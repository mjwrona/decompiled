// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Orchestration.OrchestrationRuntimeState
// Assembly: Microsoft.VisualStudio.Services.Orchestration, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C0C603F4-BE31-455B-860A-9FD3B046611C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Orchestration.dll

using Microsoft.VisualStudio.Services.Orchestration.History;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Orchestration
{
  public sealed class OrchestrationRuntimeState
  {
    public readonly IList<HistoryEvent> Events;
    public string Status;
    public readonly IList<HistoryEvent> NewEvents;
    private ExecutionStartedEvent ExecutionStartedEvent;
    private ExecutionCompletedEvent ExecutionCompletedEvent;
    public long Size;
    public long CompressedSize;

    public OrchestrationRuntimeState()
      : this((IList<HistoryEvent>) null)
    {
    }

    public OrchestrationRuntimeState(IList<HistoryEvent> events)
    {
      this.Events = (IList<HistoryEvent>) new List<HistoryEvent>();
      this.NewEvents = (IList<HistoryEvent>) new List<HistoryEvent>();
      if (events == null || events.Count <= 0)
        return;
      foreach (HistoryEvent historyEvent in (IEnumerable<HistoryEvent>) events)
        this.AddEvent(historyEvent, false, true);
    }

    public DateTime CreatedTime => this.ExecutionStartedEvent.Timestamp;

    public DateTime? CompletedTime => this.ExecutionCompletedEvent?.Timestamp;

    public string Input => this.ExecutionStartedEvent.Input;

    public string Output => this.ExecutionCompletedEvent?.Result;

    public string Name => this.ExecutionStartedEvent.Name;

    public string Version => this.ExecutionStartedEvent.Version;

    public string Tags => this.ExecutionStartedEvent.Tags;

    public OrchestrationStatus OrchestrationStatus => this.ExecutionCompletedEvent != null ? this.ExecutionCompletedEvent.OrchestrationStatus : OrchestrationStatus.Running;

    public OrchestrationInstance OrchestrationInstance => this.ExecutionStartedEvent != null ? this.ExecutionStartedEvent.OrchestrationInstance : (OrchestrationInstance) null;

    public ParentInstance ParentInstance => this.ExecutionStartedEvent != null ? this.ExecutionStartedEvent.ParentInstance : (ParentInstance) null;

    public void AddEvent(HistoryEvent historyEvent) => this.AddEvent(historyEvent, true, true);

    public bool TryAddEvent(HistoryEvent historyEvent) => this.AddEvent(historyEvent, true, false);

    private bool AddEvent(HistoryEvent historyEvent, bool isNewEvent, bool throwOnError)
    {
      this.Events.Add(historyEvent);
      if (isNewEvent)
        this.NewEvents.Add(historyEvent);
      return this.SetMarkerEvents(historyEvent, throwOnError);
    }

    private bool SetMarkerEvents(HistoryEvent historyEvent, bool throwOnError)
    {
      switch (historyEvent)
      {
        case ExecutionStartedEvent _:
          if (this.ExecutionStartedEvent != null)
          {
            if (throwOnError)
              throw new InvalidOperationException("Multiple ExecutionStartedEvent found, potential corruption in state storage");
            return false;
          }
          this.ExecutionStartedEvent = (ExecutionStartedEvent) historyEvent;
          break;
        case ExecutionCompletedEvent _:
          if (this.ExecutionCompletedEvent != null)
          {
            if (throwOnError)
              throw new InvalidOperationException("Multiple ExecutionCompletedEvents found, potential corruption in state storage");
            return false;
          }
          this.ExecutionCompletedEvent = (ExecutionCompletedEvent) historyEvent;
          break;
      }
      return true;
    }

    public OrchestrationRuntimeStateDump GetOrchestrationRuntimeStateDump()
    {
      OrchestrationRuntimeStateDump runtimeStateDump = new OrchestrationRuntimeStateDump()
      {
        Events = (IList<HistoryEvent>) new List<HistoryEvent>(),
        NewEvents = (IList<HistoryEvent>) new List<HistoryEvent>()
      };
      foreach (HistoryEvent evt in (IEnumerable<HistoryEvent>) this.Events)
      {
        HistoryEvent abridgedEvent = this.GenerateAbridgedEvent(evt);
        runtimeStateDump.Events.Add(abridgedEvent);
      }
      foreach (HistoryEvent newEvent in (IEnumerable<HistoryEvent>) this.NewEvents)
      {
        HistoryEvent abridgedEvent = this.GenerateAbridgedEvent(newEvent);
        runtimeStateDump.NewEvents.Add(abridgedEvent);
      }
      return runtimeStateDump;
    }

    private HistoryEvent GenerateAbridgedEvent(HistoryEvent evt)
    {
      HistoryEvent abridgedEvent = evt;
      switch (evt)
      {
        case TaskScheduledEvent _:
          TaskScheduledEvent taskScheduledEvent1 = (TaskScheduledEvent) evt;
          TaskScheduledEvent taskScheduledEvent2 = new TaskScheduledEvent(taskScheduledEvent1.EventId, taskScheduledEvent1.Timestamp);
          taskScheduledEvent2.IsPlayed = taskScheduledEvent1.IsPlayed;
          taskScheduledEvent2.Name = taskScheduledEvent1.Name;
          taskScheduledEvent2.Version = taskScheduledEvent1.Version;
          taskScheduledEvent2.Input = "[..snipped..]";
          abridgedEvent = (HistoryEvent) taskScheduledEvent2;
          break;
        case TaskCompletedEvent _:
          TaskCompletedEvent taskCompletedEvent1 = (TaskCompletedEvent) evt;
          TaskCompletedEvent taskCompletedEvent2 = new TaskCompletedEvent(taskCompletedEvent1.EventId, taskCompletedEvent1.Timestamp, taskCompletedEvent1.TaskScheduledId, "[..snipped..]");
          taskCompletedEvent2.IsPlayed = taskCompletedEvent1.IsPlayed;
          abridgedEvent = (HistoryEvent) taskCompletedEvent2;
          break;
        case SubOrchestrationInstanceCreatedEvent _:
          SubOrchestrationInstanceCreatedEvent instanceCreatedEvent1 = (SubOrchestrationInstanceCreatedEvent) evt;
          SubOrchestrationInstanceCreatedEvent instanceCreatedEvent2 = new SubOrchestrationInstanceCreatedEvent(instanceCreatedEvent1.EventId, instanceCreatedEvent1.Timestamp);
          instanceCreatedEvent2.IsPlayed = instanceCreatedEvent1.IsPlayed;
          instanceCreatedEvent2.Name = instanceCreatedEvent1.Name;
          instanceCreatedEvent2.Version = instanceCreatedEvent1.Version;
          instanceCreatedEvent2.Input = "[..snipped..]";
          abridgedEvent = (HistoryEvent) instanceCreatedEvent2;
          break;
        case SubOrchestrationInstanceCompletedEvent _:
          SubOrchestrationInstanceCompletedEvent instanceCompletedEvent1 = (SubOrchestrationInstanceCompletedEvent) evt;
          SubOrchestrationInstanceCompletedEvent instanceCompletedEvent2 = new SubOrchestrationInstanceCompletedEvent(instanceCompletedEvent1.EventId, instanceCompletedEvent1.Timestamp, instanceCompletedEvent1.TaskScheduledId, "[..snipped..]");
          instanceCompletedEvent2.IsPlayed = instanceCompletedEvent1.IsPlayed;
          abridgedEvent = (HistoryEvent) instanceCompletedEvent2;
          break;
        case TaskFailedEvent _:
          TaskFailedEvent taskFailedEvent1 = (TaskFailedEvent) evt;
          TaskFailedEvent taskFailedEvent2 = new TaskFailedEvent(taskFailedEvent1.EventId, taskFailedEvent1.Timestamp, taskFailedEvent1.TaskScheduledId, taskFailedEvent1.Reason, "[..snipped..]");
          taskFailedEvent2.IsPlayed = taskFailedEvent1.IsPlayed;
          abridgedEvent = (HistoryEvent) taskFailedEvent2;
          break;
        case SubOrchestrationInstanceFailedEvent _:
          SubOrchestrationInstanceFailedEvent instanceFailedEvent1 = (SubOrchestrationInstanceFailedEvent) evt;
          SubOrchestrationInstanceFailedEvent instanceFailedEvent2 = new SubOrchestrationInstanceFailedEvent(instanceFailedEvent1.EventId, instanceFailedEvent1.Timestamp, instanceFailedEvent1.TaskScheduledId, instanceFailedEvent1.Reason, "[..snipped..]");
          instanceFailedEvent2.IsPlayed = instanceFailedEvent1.IsPlayed;
          abridgedEvent = (HistoryEvent) instanceFailedEvent2;
          break;
        case ExecutionStartedEvent _:
          ExecutionStartedEvent executionStartedEvent1 = (ExecutionStartedEvent) evt;
          ExecutionStartedEvent executionStartedEvent2 = new ExecutionStartedEvent(executionStartedEvent1.EventId, executionStartedEvent1.Timestamp, "[..snipped..]");
          executionStartedEvent2.IsPlayed = executionStartedEvent1.IsPlayed;
          abridgedEvent = (HistoryEvent) executionStartedEvent2;
          break;
        case ContinueAsNewEvent _:
          ContinueAsNewEvent continueAsNewEvent1 = (ContinueAsNewEvent) evt;
          ContinueAsNewEvent continueAsNewEvent2 = new ContinueAsNewEvent(continueAsNewEvent1.EventId, continueAsNewEvent1.Timestamp, "[..snipped..]");
          continueAsNewEvent2.IsPlayed = continueAsNewEvent1.IsPlayed;
          abridgedEvent = (HistoryEvent) continueAsNewEvent2;
          break;
        case ExecutionCompletedEvent _:
          ExecutionCompletedEvent executionCompletedEvent1 = (ExecutionCompletedEvent) evt;
          ExecutionCompletedEvent executionCompletedEvent2 = new ExecutionCompletedEvent(executionCompletedEvent1.EventId, executionCompletedEvent1.Timestamp, "[..snipped..]", executionCompletedEvent1.OrchestrationStatus);
          executionCompletedEvent2.IsPlayed = executionCompletedEvent1.IsPlayed;
          abridgedEvent = (HistoryEvent) executionCompletedEvent2;
          break;
        case ExecutionTerminatedEvent _:
          ExecutionTerminatedEvent executionTerminatedEvent1 = (ExecutionTerminatedEvent) evt;
          ExecutionTerminatedEvent executionTerminatedEvent2 = new ExecutionTerminatedEvent(executionTerminatedEvent1.EventId, executionTerminatedEvent1.Timestamp, "[..snipped..]");
          executionTerminatedEvent2.IsPlayed = executionTerminatedEvent1.IsPlayed;
          abridgedEvent = (HistoryEvent) executionTerminatedEvent2;
          break;
      }
      return abridgedEvent;
    }
  }
}
