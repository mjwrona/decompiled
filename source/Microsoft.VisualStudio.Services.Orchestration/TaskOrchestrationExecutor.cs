// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Orchestration.TaskOrchestrationExecutor
// Assembly: Microsoft.VisualStudio.Services.Orchestration, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C0C603F4-BE31-455B-860A-9FD3B046611C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Orchestration.dll

using Microsoft.VisualStudio.Services.Orchestration.Async;
using Microsoft.VisualStudio.Services.Orchestration.Command;
using Microsoft.VisualStudio.Services.Orchestration.History;
using Microsoft.VisualStudio.Services.Orchestration.TaskActivityDispatcherSharding;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Orchestration
{
  internal class TaskOrchestrationExecutor
  {
    private OrchestrationContextImpl context;
    private OrchestrationRuntimeState orchestrationRuntimeState;
    private TaskOrchestration taskOrchestration;
    private TaskScheduler decisionScheduler;
    private Task<string> result;

    public TaskOrchestrationExecutor(
      OrchestrationRuntimeState orchestrationRuntimeState,
      TaskOrchestration taskOrchestration,
      OrchestrationSerializer serializer,
      IActivityShardLocator activityShardLocator,
      IOrchestrationTracer tracer)
    {
      this.decisionScheduler = (TaskScheduler) new CurrentThreadTaskScheduler();
      this.context = new OrchestrationContextImpl(orchestrationRuntimeState, this.decisionScheduler, serializer, activityShardLocator, tracer);
      this.orchestrationRuntimeState = orchestrationRuntimeState;
      this.taskOrchestration = taskOrchestration;
    }

    public IEnumerable<OrchestratorAction> Execute()
    {
      SynchronizationContext current = SynchronizationContext.Current;
      try
      {
        SynchronizationContext.SetSynchronizationContext(this.decisionScheduler.ToSynchronizationContext());
        try
        {
          foreach (HistoryEvent historyEvent in (IEnumerable<HistoryEvent>) this.orchestrationRuntimeState.Events)
          {
            if (historyEvent.EventType == EventType.OrchestratorStarted)
            {
              this.context.CurrentUtcDateTime = historyEvent.Timestamp;
            }
            else
            {
              this.context.IsReplaying = historyEvent.IsPlayed;
              this.ProcessEvent(historyEvent);
              historyEvent.IsPlayed = true;
            }
          }
          if (!this.context.HasOpenTasks)
          {
            if (this.result.IsCompleted)
            {
              if (this.result.IsFaulted)
                this.context.FailOrchestration(this.result.Exception.InnerExceptions.FirstOrDefault<Exception>());
              else
                this.context.CompleteOrchestration(this.result.Result);
            }
          }
        }
        catch (NonDeterministicOrchestrationException ex)
        {
          if (this.result.IsFaulted)
            this.context.Trace(0, TraceLevel.Error, this.result.Exception.InnerExceptions.FirstOrDefault<Exception>().ToString());
          this.context.FailOrchestration((Exception) ex);
        }
        return this.context.OrchestratorActions;
      }
      finally
      {
        this.orchestrationRuntimeState.Status = this.taskOrchestration.GetStatus();
        SynchronizationContext.SetSynchronizationContext(current);
      }
    }

    private void ProcessEvent(HistoryEvent historyEvent)
    {
      switch (historyEvent.EventType)
      {
        case EventType.ExecutionStarted:
          this.result = this.taskOrchestration.Execute((OrchestrationContext) this.context, ((ExecutionStartedEvent) historyEvent).Input);
          break;
        case EventType.ExecutionTerminated:
          this.context.HandleExecutionTerminatedEvent((ExecutionTerminatedEvent) historyEvent);
          break;
        case EventType.TaskScheduled:
          this.context.HandleTaskScheduledEvent((TaskScheduledEvent) historyEvent);
          break;
        case EventType.TaskCompleted:
          this.context.HandleTaskCompletedEvent((TaskCompletedEvent) historyEvent);
          break;
        case EventType.TaskFailed:
          this.context.HandleTaskFailedEvent((TaskFailedEvent) historyEvent);
          break;
        case EventType.SubOrchestrationInstanceCreated:
          this.context.HandleSubOrchestrationCreatedEvent((SubOrchestrationInstanceCreatedEvent) historyEvent);
          break;
        case EventType.SubOrchestrationInstanceCompleted:
          this.context.HandleSubOrchestrationInstanceCompletedEvent((SubOrchestrationInstanceCompletedEvent) historyEvent);
          break;
        case EventType.SubOrchestrationInstanceFailed:
          this.context.HandleSubOrchestrationInstanceFailedEvent((SubOrchestrationInstanceFailedEvent) historyEvent);
          break;
        case EventType.TimerCreated:
          this.context.HandleTimerCreatedEvent((TimerCreatedEvent) historyEvent);
          break;
        case EventType.TimerFired:
          this.context.HandleTimerFiredEvent((TimerFiredEvent) historyEvent);
          break;
        case EventType.EventRaised:
          EventRaisedEvent eventRaisedEvent = (EventRaisedEvent) historyEvent;
          this.taskOrchestration.RaiseEvent((OrchestrationContext) this.context, eventRaisedEvent.Name, eventRaisedEvent.Input);
          break;
      }
    }
  }
}
