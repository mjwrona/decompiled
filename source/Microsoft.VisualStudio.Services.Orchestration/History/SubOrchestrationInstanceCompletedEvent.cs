// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Orchestration.History.SubOrchestrationInstanceCompletedEvent
// Assembly: Microsoft.VisualStudio.Services.Orchestration, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C0C603F4-BE31-455B-860A-9FD3B046611C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Orchestration.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Orchestration.History
{
  [DataContract]
  public class SubOrchestrationInstanceCompletedEvent : HistoryEvent
  {
    internal SubOrchestrationInstanceCompletedEvent()
    {
    }

    public SubOrchestrationInstanceCompletedEvent(
      int eventId,
      DateTime timestamp,
      int taskScheduledId,
      string result)
      : base(eventId, timestamp)
    {
      this.TaskScheduledId = taskScheduledId;
      this.Result = result;
    }

    public override EventType EventType => EventType.SubOrchestrationInstanceCompleted;

    [DataMember]
    public int TaskScheduledId { get; private set; }

    [DataMember]
    public string Result { get; private set; }
  }
}
