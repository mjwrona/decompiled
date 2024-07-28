// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Orchestration.History.HistoryEvent
// Assembly: Microsoft.VisualStudio.Services.Orchestration, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C0C603F4-BE31-455B-860A-9FD3B046611C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Orchestration.dll

using Newtonsoft.Json;
using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Orchestration.History
{
  [DataContract]
  [KnownType(typeof (ExecutionStartedEvent))]
  [KnownType(typeof (ExecutionCompletedEvent))]
  [KnownType(typeof (ExecutionTerminatedEvent))]
  [KnownType(typeof (TaskCompletedEvent))]
  [KnownType(typeof (TaskFailedEvent))]
  [KnownType(typeof (TaskScheduledEvent))]
  [KnownType(typeof (SubOrchestrationInstanceCreatedEvent))]
  [KnownType(typeof (SubOrchestrationInstanceCompletedEvent))]
  [KnownType(typeof (SubOrchestrationInstanceFailedEvent))]
  [KnownType(typeof (TimerCreatedEvent))]
  [KnownType(typeof (TimerFiredEvent))]
  [KnownType(typeof (OrchestratorStartedEvent))]
  [KnownType(typeof (OrchestratorCompletedEvent))]
  [KnownType(typeof (EventRaisedEvent))]
  [KnownType(typeof (ContinueAsNewEvent))]
  [JsonConverter(typeof (HistoryEventJsonConverter))]
  public abstract class HistoryEvent
  {
    internal HistoryEvent()
    {
    }

    public HistoryEvent(int eventId, DateTime timestamp)
    {
      this.EventId = eventId;
      this.IsPlayed = false;
      this.Timestamp = timestamp;
    }

    [DataMember]
    public int EventId { get; private set; }

    [DataMember]
    public bool IsPlayed { get; internal set; }

    [DataMember]
    public DateTime Timestamp { get; private set; }

    [DataMember]
    public virtual EventType EventType { get; private set; }
  }
}
