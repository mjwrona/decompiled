// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Orchestration.OrchestrationRuntime
// Assembly: Microsoft.VisualStudio.Services.Orchestration, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C0C603F4-BE31-455B-860A-9FD3B046611C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Orchestration.dll

using Microsoft.VisualStudio.Services.Orchestration.Command;
using Microsoft.VisualStudio.Services.Orchestration.History;
using Microsoft.VisualStudio.Services.Orchestration.TaskActivityDispatcherSharding;
using Microsoft.VisualStudio.Services.Orchestration.Tracing;
using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Tracing;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Orchestration
{
  public sealed class OrchestrationRuntime : IDisposable
  {
    private OrchestrationTraceSource m_traceSource;
    private NameVersionObjectManager<TaskActivity> m_activityManager = new NameVersionObjectManager<TaskActivity>();
    private NameVersionObjectManager<TaskOrchestration> m_orchestrationManager = new NameVersionObjectManager<TaskOrchestration>();

    public OrchestrationRuntime(OrchestrationSerializer serializer)
    {
      if (serializer == null)
        throw new ArgumentNullException(nameof (serializer));
      this.MaxMessageCount = 100;
      this.Serializer = serializer;
      this.StateSerializer = new OrchestrationSerializer(JsonUtility.CreateJsonSerializer());
      this.m_traceSource = new OrchestrationTraceSource(SourceLevels.Verbose);
      if (this.m_traceSource.Listeners.Count <= 0 || !(this.m_traceSource.Listeners[0] is DefaultTraceListener))
        return;
      this.m_traceSource.Listeners.RemoveAt(0);
    }

    public bool AlwaysSaveStateToHistory { get; set; }

    public bool IncludeDetails { get; set; }

    public bool IncludeParameters { get; set; }

    public int MaxMessageCount { get; set; }

    public OrchestrationSerializer Serializer { get; private set; }

    public OrchestrationSerializer StateSerializer { get; private set; }

    public TraceSource TraceSource => (TraceSource) this.m_traceSource;

    public IOrchestrationTracer Tracer { get; set; }

    public IActivityShardLocator ActivityShardLocator { get; set; }

    public void Dispose()
    {
      if (this.m_traceSource == null)
        return;
      this.m_traceSource.Close();
      this.m_traceSource = (OrchestrationTraceSource) null;
    }

    public OrchestrationRuntime AddTaskActivities(params TaskActivity[] taskActivityObjects)
    {
      foreach (TaskActivity taskActivityObject in taskActivityObjects)
        this.m_activityManager.Add((ObjectCreator<TaskActivity>) new DefaultObjectCreator<TaskActivity>(taskActivityObject));
      return this;
    }

    public OrchestrationRuntime AddTaskActivities(params Type[] taskActivityTypes)
    {
      foreach (Type taskActivityType in taskActivityTypes)
        this.m_activityManager.Add((ObjectCreator<TaskActivity>) new DefaultObjectCreator<TaskActivity>(taskActivityType));
      return this;
    }

    public OrchestrationRuntime AddTaskActivities(
      params ObjectCreator<TaskActivity>[] taskActivityCreators)
    {
      foreach (ObjectCreator<TaskActivity> taskActivityCreator in taskActivityCreators)
        this.m_activityManager.Add(taskActivityCreator);
      return this;
    }

    public OrchestrationRuntime AddTaskActivitiesFromInterface<T>(T activities) => this.AddTaskActivitiesFromInterface<T>(activities, false);

    public OrchestrationRuntime AddTaskActivitiesFromInterface<T>(
      T activities,
      bool useFullyQualifiedMethodNames)
    {
      return this.AddTaskActivitiesFromInterface<T>((ITaskActivityInvoker) new SingletonTaskActivityInvoker<T>(activities), useFullyQualifiedMethodNames);
    }

    public OrchestrationRuntime AddTaskActivitiesFromInterface<T>(
      ITaskActivityInvoker invoker,
      bool useFullyQualifiedMethodNames)
    {
      Type type = typeof (T);
      if (!type.IsInterface)
        throw new Exception("Contract can only be an interface.");
      foreach (MethodInfo method in type.GetMethods())
        this.m_activityManager.Add((ObjectCreator<TaskActivity>) new NameValueObjectCreator<TaskActivity>(NameVersionHelper.GetDefaultName((object) method, useFullyQualifiedMethodNames), NameVersionHelper.GetDefaultVersion((object) method), (TaskActivity) new ReflectionBasedTaskActivity(this.Serializer, invoker, method)));
      return this;
    }

    public OrchestrationRuntime AddTaskOrchestrations(params Type[] taskOrchestrationTypes)
    {
      foreach (Type orchestrationType in taskOrchestrationTypes)
        this.m_orchestrationManager.Add((ObjectCreator<TaskOrchestration>) new DefaultObjectCreator<TaskOrchestration>(orchestrationType));
      return this;
    }

    public OrchestrationRuntime AddTaskOrchestrations(
      params ObjectCreator<TaskOrchestration>[] taskOrchestrationCreators)
    {
      foreach (ObjectCreator<TaskOrchestration> orchestrationCreator in taskOrchestrationCreators)
        this.m_orchestrationManager.Add(orchestrationCreator);
      return this;
    }

    public async Task<TaskMessage> RunTaskAsync(TaskMessage message, OrchestrationClock clock)
    {
      OrchestrationInstance orchestrationInstance = message.OrchestrationInstance;
      if (orchestrationInstance == null || string.IsNullOrWhiteSpace(orchestrationInstance.InstanceId))
        throw this.m_traceSource.TraceException(EventLevel.Error, (Exception) new InvalidOperationException("Message does not contain any OrchestrationInstance information"));
      if (message.Event.EventType != EventType.TaskScheduled)
        throw this.m_traceSource.TraceException(EventLevel.Critical, (Exception) new NotSupportedException("Activity worker does not support event of type: " + message.Event.EventType.ToString()));
      TaskScheduledEvent scheduledEvent = (TaskScheduledEvent) message.Event;
      TaskActivity taskActivity = this.m_activityManager.GetObject(scheduledEvent.Name, scheduledEvent.Version);
      TaskContext context = new TaskContext(message);
      HistoryEvent historyEvent;
      try
      {
        if (taskActivity == null)
          throw new TypeMissingException("TaskActivity " + scheduledEvent.Name + " version " + scheduledEvent.Version + " was not found");
        historyEvent = (HistoryEvent) new TaskCompletedEvent(-1, clock.UtcNow, scheduledEvent.EventId, await taskActivity.RunAsync(context, scheduledEvent.Input));
      }
      catch (TaskFailureException ex)
      {
        this.m_traceSource.TraceExceptionInstance(EventLevel.Error, message.OrchestrationInstance, (Exception) ex);
        string details = this.IncludeDetails ? ex.Details : (string) null;
        historyEvent = (HistoryEvent) new TaskFailedEvent(-1, clock.UtcNow, scheduledEvent.EventId, ex.Message, details);
      }
      catch (Exception ex)
      {
        this.m_traceSource.TraceExceptionInstance(EventLevel.Error, message.OrchestrationInstance, ex);
        string details = this.IncludeDetails ? string.Format("Unhandled exception while executing task: {0}\n\t{1}", (object) ex, (object) ex.StackTrace) : (string) null;
        historyEvent = (HistoryEvent) new TaskFailedEvent(-1, clock.UtcNow, scheduledEvent.EventId, ex.Message, details);
      }
      TaskMessage taskMessage = new TaskMessage(orchestrationInstance, historyEvent);
      orchestrationInstance = (OrchestrationInstance) null;
      scheduledEvent = (TaskScheduledEvent) null;
      return taskMessage;
    }

    public OrchestrationRuntimeResult RunOrchestration(
      string sessionId,
      OrchestrationRuntimeState runtimeState,
      IEnumerable<TaskMessage> newMessages,
      OrchestrationClock clock)
    {
      OrchestrationRuntimeResult orchestrationRuntimeResult = new OrchestrationRuntimeResult(sessionId);
      TaskMessage taskMessage = (TaskMessage) null;
      ExecutionStartedEvent executionStartedEvent = (ExecutionStartedEvent) null;
      try
      {
        runtimeState.AddEvent((HistoryEvent) new OrchestratorStartedEvent(-1, clock.UtcNow));
        if (!this.ReconcileMessagesWithState(sessionId, runtimeState, newMessages))
        {
          this.m_traceSource.TraceSession(EventLevel.Warning, sessionId, "Received result for a deleted orchestration");
          orchestrationRuntimeResult.IsCompleted = true;
        }
        else
        {
          this.m_traceSource.TraceInstance(EventLevel.Verbose, runtimeState.OrchestrationInstance, (Func<string>) (() => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Executing user orchestration: {0}", (object) TraceExtensions.ToJsonString((object) runtimeState.GetOrchestrationRuntimeStateDump()))));
          IEnumerable<OrchestratorAction> decisions = this.ExecuteOrchestration(runtimeState);
          this.m_traceSource.TraceInstance(EventLevel.Informational, runtimeState.OrchestrationInstance, (Func<string>) (() => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Executed user orchestration. Received {0} orchestrator actions: {1}", (object) decisions.Count<OrchestratorAction>(), (object) string.Join(", ", decisions.Select<OrchestratorAction, string>((Func<OrchestratorAction, string>) (d => d.Id.ToString() + ":" + d.OrchestratorActionType.ToString()))))));
          foreach (OrchestratorAction action1 in decisions)
          {
            this.m_traceSource.TraceInstance(EventLevel.Informational, runtimeState.OrchestrationInstance, "Processing orchestrator action of type {0}", (object) action1.OrchestratorActionType);
            switch (action1.OrchestratorActionType)
            {
              case OrchestratorActionType.ScheduleOrchestrator:
                TaskMessage message1 = this.CreateMessage((ScheduleTaskOrchestratorAction) action1, runtimeState, clock);
                orchestrationRuntimeResult.ActivityMessages.Add(message1);
                break;
              case OrchestratorActionType.CreateSubOrchestration:
                TaskMessage message2 = this.CreateMessage((CreateSubOrchestrationAction) action1, runtimeState, clock);
                orchestrationRuntimeResult.SubOrchestrationMessages.Add(message2);
                break;
              case OrchestratorActionType.CreateTimer:
                TaskMessage message3 = this.CreateMessage((CreateTimerOrchestratorAction) action1, runtimeState, clock);
                orchestrationRuntimeResult.TimerMessages.Add(message3);
                break;
              case OrchestratorActionType.OrchestrationComplete:
                bool continuedAsNew;
                TaskMessage message4 = this.CreateMessage((OrchestrationCompleteOrchestratorAction) action1, runtimeState, clock, out continuedAsNew);
                if (message4 != null)
                {
                  if (continuedAsNew)
                  {
                    taskMessage = message4;
                    executionStartedEvent = message4.Event as ExecutionStartedEvent;
                  }
                  else
                    orchestrationRuntimeResult.SubOrchestrationMessages.Add(message4);
                }
                orchestrationRuntimeResult.ContinuedAsNew = continuedAsNew;
                orchestrationRuntimeResult.IsCompleted = !continuedAsNew;
                break;
              default:
                throw this.m_traceSource.TraceExceptionInstance(EventLevel.Error, runtimeState.OrchestrationInstance, (Exception) new NotSupportedException(string.Format("Decision type {0} not supported", (object) action1.OrchestratorActionType)));
            }
            if (orchestrationRuntimeResult.ActivityMessages.Count + orchestrationRuntimeResult.SubOrchestrationMessages.Count + orchestrationRuntimeResult.TimerMessages.Count > this.MaxMessageCount)
            {
              this.m_traceSource.TraceInstance(EventLevel.Informational, runtimeState.OrchestrationInstance, "MaxMessageCount reached.  Adding timer to process remaining events in next attempt.");
              if (orchestrationRuntimeResult.IsCompleted || orchestrationRuntimeResult.ContinuedAsNew)
              {
                this.m_traceSource.TraceInstance(EventLevel.Informational, runtimeState.OrchestrationInstance, "Orchestration already completed.  Skip adding timer for splitting messages.");
                break;
              }
              CreateTimerOrchestratorAction action2 = new CreateTimerOrchestratorAction();
              action2.Id = -100;
              action2.FireAt = DateTime.UtcNow;
              TaskMessage message5 = this.CreateMessage(action2, runtimeState, clock);
              orchestrationRuntimeResult.TimerMessages.Add(message5);
              break;
            }
          }
        }
      }
      catch (Exception ex)
      {
        this.m_traceSource.TraceExceptionInstance(EventLevel.Error, runtimeState.OrchestrationInstance, ex);
        runtimeState.AddEvent((HistoryEvent) new ExecutionCompletedEvent(-1, clock.UtcNow, ex.GetType().Name, OrchestrationStatus.Failed));
        orchestrationRuntimeResult.IsCompleted = true;
        orchestrationRuntimeResult.UnhandledException = ex;
      }
      if (!orchestrationRuntimeResult.ContinuedAsNew)
      {
        runtimeState.AddEvent((HistoryEvent) new OrchestratorCompletedEvent(-1, clock.UtcNow));
      }
      else
      {
        orchestrationRuntimeResult.ActivityMessages.Clear();
        orchestrationRuntimeResult.TimerMessages.Clear();
        if (taskMessage != null)
          orchestrationRuntimeResult.SubOrchestrationMessages.Add(taskMessage);
      }
      if (orchestrationRuntimeResult.ContinuedAsNew)
      {
        this.m_traceSource.TraceSession(EventLevel.Informational, sessionId, "Updating state for continuation");
        orchestrationRuntimeResult.AddStateEvent((HistoryEvent) new OrchestratorStartedEvent(-1, clock.UtcNow));
        orchestrationRuntimeResult.AddStateEvent((HistoryEvent) executionStartedEvent);
        orchestrationRuntimeResult.AddStateEvent((HistoryEvent) new OrchestratorCompletedEvent(-1, clock.UtcNow));
      }
      else
        orchestrationRuntimeResult.AddStateEvents(runtimeState.Events);
      return orchestrationRuntimeResult;
    }

    private IEnumerable<OrchestratorAction> ExecuteOrchestration(
      OrchestrationRuntimeState runtimeState)
    {
      TaskOrchestration taskOrchestration = this.m_orchestrationManager.GetObject(runtimeState.Name, runtimeState.Version);
      if (taskOrchestration == null)
      {
        if (runtimeState.ParentInstance == null)
          throw this.m_traceSource.TraceExceptionInstance(EventLevel.Error, runtimeState.OrchestrationInstance, (Exception) new TypeMissingException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Orchestration not found: ({0}, {1})", (object) runtimeState.Name, (object) runtimeState.Version)));
        OrchestrationCompleteOrchestratorAction orchestratorAction = new OrchestrationCompleteOrchestratorAction();
        orchestratorAction.OrchestrationStatus = OrchestrationStatus.Failed;
        if (this.IncludeDetails)
        {
          TypeMissingException originalException = new TypeMissingException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Orchestration not found: ({0}, {1})", (object) runtimeState.Name, (object) runtimeState.Version));
          orchestratorAction.Details = Utils.SerializeCause((Exception) originalException, this.Serializer);
        }
        return (IEnumerable<OrchestratorAction>) new List<OrchestrationCompleteOrchestratorAction>()
        {
          orchestratorAction
        };
      }
      taskOrchestration.Serializer = this.Serializer;
      taskOrchestration.StateSerializer = this.StateSerializer ?? this.Serializer;
      return new TaskOrchestrationExecutor(runtimeState, taskOrchestration, this.Serializer, this.ActivityShardLocator, this.Tracer).Execute();
    }

    private TaskMessage CreateMessage(
      OrchestrationCompleteOrchestratorAction action,
      OrchestrationRuntimeState runtimeState,
      OrchestrationClock clock,
      out bool continuedAsNew)
    {
      continuedAsNew = action.OrchestrationStatus == OrchestrationStatus.ContinuedAsNew;
      ExecutionCompletedEvent executionCompletedEvent = !continuedAsNew ? new ExecutionCompletedEvent(action.Id, clock.UtcNow, action.Result, action.OrchestrationStatus) : (ExecutionCompletedEvent) new ContinueAsNewEvent(action.Id, clock.UtcNow, action.Result);
      runtimeState.AddEvent((HistoryEvent) executionCompletedEvent);
      this.m_traceSource.TraceInstance(EventLevel.Informational, runtimeState.OrchestrationInstance, "Instance Id '{0}' completed in state {1} with result: {2}", (object) runtimeState.OrchestrationInstance, (object) runtimeState.OrchestrationStatus, (object) action.Result);
      string history = this.Serializer.Serialize((object) runtimeState.Events, Formatting.Indented);
      this.m_traceSource.TraceInstance(EventLevel.Verbose, runtimeState.OrchestrationInstance, (Func<string>) (() => Utils.EscapeJson(history)));
      if (action.OrchestrationStatus == OrchestrationStatus.ContinuedAsNew)
      {
        TaskMessage message = new TaskMessage();
        ExecutionStartedEvent executionStartedEvent = new ExecutionStartedEvent(-1, clock.UtcNow, action.Result);
        executionStartedEvent.OrchestrationInstance = new OrchestrationInstance()
        {
          InstanceId = runtimeState.OrchestrationInstance.InstanceId,
          ExecutionId = Guid.NewGuid().ToString("N")
        };
        executionStartedEvent.Tags = runtimeState.Tags;
        executionStartedEvent.ParentInstance = runtimeState.ParentInstance;
        executionStartedEvent.Name = runtimeState.Name;
        executionStartedEvent.Version = action.NewVersion ?? runtimeState.Version;
        message.OrchestrationInstance = executionStartedEvent.OrchestrationInstance;
        message.Event = (HistoryEvent) executionStartedEvent;
        return message;
      }
      if (runtimeState.ParentInstance != null)
      {
        TaskMessage message = new TaskMessage();
        if (action.OrchestrationStatus == OrchestrationStatus.Completed)
        {
          SubOrchestrationInstanceCompletedEvent instanceCompletedEvent = new SubOrchestrationInstanceCompletedEvent(-1, clock.UtcNow, runtimeState.ParentInstance.TaskScheduleId, action.Result);
          message.Event = (HistoryEvent) instanceCompletedEvent;
        }
        else if (action.OrchestrationStatus == OrchestrationStatus.Failed || action.OrchestrationStatus == OrchestrationStatus.Terminated)
        {
          SubOrchestrationInstanceFailedEvent instanceFailedEvent = new SubOrchestrationInstanceFailedEvent(-1, clock.UtcNow, runtimeState.ParentInstance.TaskScheduleId, action.Result, this.IncludeDetails ? action.Details : (string) null);
          message.Event = (HistoryEvent) instanceFailedEvent;
        }
        if (message.Event != null)
        {
          message.OrchestrationInstance = runtimeState.ParentInstance.OrchestrationInstance;
          return message;
        }
      }
      return (TaskMessage) null;
    }

    private TaskMessage CreateMessage(
      ScheduleTaskOrchestratorAction action,
      OrchestrationRuntimeState runtimeState,
      OrchestrationClock clock)
    {
      TaskScheduledEvent taskScheduledEvent = new TaskScheduledEvent(action.Id, clock.UtcNow);
      taskScheduledEvent.Name = action.Name;
      taskScheduledEvent.Version = action.Version;
      taskScheduledEvent.Input = action.Input;
      taskScheduledEvent.DispatcherType = action.DispatcherType;
      if (this.IncludeParameters)
        runtimeState.AddEvent((HistoryEvent) taskScheduledEvent);
      else
        runtimeState.AddEvent((HistoryEvent) new TaskScheduledEvent(action.Id, clock.UtcNow)
        {
          Name = action.Name,
          Version = action.Version
        });
      return new TaskMessage(runtimeState.OrchestrationInstance, (HistoryEvent) taskScheduledEvent, action.DispatcherType);
    }

    private TaskMessage CreateMessage(
      CreateTimerOrchestratorAction action,
      OrchestrationRuntimeState runtimeState,
      OrchestrationClock clock)
    {
      TimerCreatedEvent timerCreatedEvent = new TimerCreatedEvent(action.Id, clock.UtcNow);
      timerCreatedEvent.FireAt = action.FireAt;
      TimerFiredEvent timerFiredEvent = new TimerFiredEvent(-1, clock.UtcNow);
      timerFiredEvent.TimerId = action.Id;
      runtimeState.AddEvent((HistoryEvent) timerCreatedEvent);
      return new TaskMessage(runtimeState.OrchestrationInstance, (HistoryEvent) timerFiredEvent, new DateTime?(action.FireAt));
    }

    private TaskMessage CreateMessage(
      CreateSubOrchestrationAction action,
      OrchestrationRuntimeState runtimeState,
      OrchestrationClock clock)
    {
      SubOrchestrationInstanceCreatedEvent instanceCreatedEvent = new SubOrchestrationInstanceCreatedEvent(action.Id, clock.UtcNow);
      instanceCreatedEvent.Name = action.Name;
      instanceCreatedEvent.Version = action.Version;
      instanceCreatedEvent.InstanceId = action.InstanceId;
      if (this.IncludeParameters)
        instanceCreatedEvent.Input = action.Input;
      runtimeState.AddEvent((HistoryEvent) instanceCreatedEvent);
      ExecutionStartedEvent executionStartedEvent = new ExecutionStartedEvent(-1, clock.UtcNow, action.Input);
      executionStartedEvent.Tags = runtimeState.Tags;
      executionStartedEvent.OrchestrationInstance = new OrchestrationInstance()
      {
        InstanceId = action.InstanceId,
        ExecutionId = Guid.NewGuid().ToString("N")
      };
      executionStartedEvent.ParentInstance = new ParentInstance()
      {
        OrchestrationInstance = runtimeState.OrchestrationInstance,
        Name = runtimeState.Name,
        Version = runtimeState.Version,
        TaskScheduleId = action.Id
      };
      executionStartedEvent.Name = action.Name;
      executionStartedEvent.Version = action.Version;
      return new TaskMessage(executionStartedEvent.OrchestrationInstance, (HistoryEvent) executionStartedEvent);
    }

    private bool ReconcileMessagesWithState(
      string sessionId,
      OrchestrationRuntimeState runtimeState,
      IEnumerable<TaskMessage> messages)
    {
      foreach (TaskMessage message in messages)
      {
        OrchestrationInstance orchestrationInstance = message.OrchestrationInstance;
        if (orchestrationInstance == null || string.IsNullOrWhiteSpace(orchestrationInstance.InstanceId))
          throw this.m_traceSource.TraceException(EventLevel.Error, (Exception) new InvalidOperationException("Message does not contain any OrchestrationInstance information"));
        if (runtimeState.Events.Count == 1 && message.Event.EventType != EventType.ExecutionStarted)
          return false;
        this.m_traceSource.TraceInstance(EventLevel.Informational, orchestrationInstance, "Processing new event with Id {0} and type {1}", (object) message.Event.EventId, (object) message.Event.EventType);
        if (message.Event.EventType == EventType.ExecutionStarted)
        {
          if (runtimeState.Events.Count > 1)
          {
            this.m_traceSource.TraceInstance(EventLevel.Warning, orchestrationInstance, "Duplicate start event.  Ignoring event with Id {0} and type {1} ", (object) message.Event.EventId, (object) message.Event.EventType);
            continue;
          }
        }
        else if (!string.IsNullOrWhiteSpace(orchestrationInstance.ExecutionId) && !string.Equals(orchestrationInstance.ExecutionId, runtimeState.OrchestrationInstance.ExecutionId))
        {
          this.m_traceSource.TraceInstance(EventLevel.Warning, orchestrationInstance, "ExecutionId of event does not match current executionId.  Ignoring event with Id {0} and type {1} ", (object) message.Event.EventId, (object) message.Event.EventType);
          continue;
        }
        runtimeState.AddEvent(message.Event);
        if (message.Event.EventType == EventType.ExecutionTerminated)
          break;
      }
      return true;
    }
  }
}
