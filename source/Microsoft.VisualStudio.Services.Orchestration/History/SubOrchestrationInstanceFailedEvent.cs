// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Orchestration.History.SubOrchestrationInstanceFailedEvent
// Assembly: Microsoft.VisualStudio.Services.Orchestration, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C0C603F4-BE31-455B-860A-9FD3B046611C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Orchestration.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Orchestration.History
{
  [DataContract]
  public class SubOrchestrationInstanceFailedEvent : HistoryEvent
  {
    internal SubOrchestrationInstanceFailedEvent()
    {
    }

    public SubOrchestrationInstanceFailedEvent(
      int eventId,
      DateTime timestamp,
      int taskScheduledId,
      string reason,
      string details)
      : base(eventId, timestamp)
    {
      this.TaskScheduledId = taskScheduledId;
      this.Reason = reason;
      this.Details = details;
    }

    public override EventType EventType => EventType.SubOrchestrationInstanceFailed;

    [DataMember]
    public int TaskScheduledId { get; private set; }

    [DataMember]
    public string Reason { get; private set; }

    [DataMember]
    public string Details { get; private set; }
  }
}
