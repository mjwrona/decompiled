// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Orchestration.History.EventRaisedEvent
// Assembly: Microsoft.VisualStudio.Services.Orchestration, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C0C603F4-BE31-455B-860A-9FD3B046611C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Orchestration.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Orchestration.History
{
  [DataContract]
  public sealed class EventRaisedEvent : HistoryEvent
  {
    internal EventRaisedEvent()
    {
    }

    public EventRaisedEvent(int eventId, DateTime timestamp, string input)
      : base(eventId, timestamp)
    {
      this.Input = input;
    }

    public override EventType EventType => EventType.EventRaised;

    [DataMember]
    public string Name { get; set; }

    [DataMember]
    public string Input { get; set; }
  }
}
