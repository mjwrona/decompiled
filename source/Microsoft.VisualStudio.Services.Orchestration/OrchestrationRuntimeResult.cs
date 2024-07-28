// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Orchestration.OrchestrationRuntimeResult
// Assembly: Microsoft.VisualStudio.Services.Orchestration, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C0C603F4-BE31-455B-860A-9FD3B046611C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Orchestration.dll

using Microsoft.VisualStudio.Services.Orchestration.History;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Orchestration
{
  public sealed class OrchestrationRuntimeResult
  {
    private List<TaskMessage> m_activityMessages;
    private List<HistoryEvent> m_state;
    private List<TaskMessage> m_subOrchestrationMessages;
    private List<TaskMessage> m_timerMessages;
    private OrchestrationStatus m_status;

    internal OrchestrationRuntimeResult(string sessionId)
    {
      this.SessionId = sessionId;
      this.m_state = new List<HistoryEvent>();
    }

    public OrchestrationRuntimeResult(string sessionId, bool isCompleted)
    {
      this.SessionId = sessionId;
      this.IsCompleted = isCompleted;
      this.m_state = new List<HistoryEvent>();
    }

    public IList<TaskMessage> ActivityMessages
    {
      get
      {
        if (this.m_activityMessages == null)
          this.m_activityMessages = new List<TaskMessage>();
        return (IList<TaskMessage>) this.m_activityMessages;
      }
    }

    public bool ContinuedAsNew { get; internal set; }

    public bool IsCompleted { get; internal set; }

    public string SessionId { get; internal set; }

    public IList<HistoryEvent> State => (IList<HistoryEvent>) this.m_state.AsReadOnly();

    public IList<TaskMessage> SubOrchestrationMessages
    {
      get
      {
        if (this.m_subOrchestrationMessages == null)
          this.m_subOrchestrationMessages = new List<TaskMessage>();
        return (IList<TaskMessage>) this.m_subOrchestrationMessages;
      }
    }

    public IList<TaskMessage> TimerMessages
    {
      get
      {
        if (this.m_timerMessages == null)
          this.m_timerMessages = new List<TaskMessage>();
        return (IList<TaskMessage>) this.m_timerMessages;
      }
    }

    public Exception UnhandledException { get; internal set; }

    public OrchestrationStatus Status => this.m_status;

    public void AddStateEvent(HistoryEvent historyEvent)
    {
      if (historyEvent == null)
        return;
      this.m_state.Add(historyEvent);
      if (!(historyEvent is ExecutionCompletedEvent))
        return;
      this.m_status = ((ExecutionCompletedEvent) historyEvent).OrchestrationStatus;
    }

    public void AddStateEvents(IList<HistoryEvent> historyEvents)
    {
      if (historyEvents == null)
        return;
      foreach (HistoryEvent historyEvent in (IEnumerable<HistoryEvent>) historyEvents)
        this.AddStateEvent(historyEvent);
    }
  }
}
