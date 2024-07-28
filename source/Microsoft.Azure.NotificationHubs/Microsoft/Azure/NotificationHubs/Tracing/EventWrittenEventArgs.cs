// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.Tracing.EventWrittenEventArgs
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using System;
using System.Collections.Generic;

namespace Microsoft.Azure.NotificationHubs.Tracing
{
  internal class EventWrittenEventArgs : EventArgs
  {
    private EventSource m_eventSource;

    public int EventId { get; internal set; }

    public IEnumerable<object> Payload { get; internal set; }

    public EventSource EventSource => this.m_eventSource;

    public EventKeywords Keywords => (EventKeywords) this.m_eventSource.m_eventData[this.EventId].Descriptor.Keywords;

    public EventOpcode Opcode => (EventOpcode) this.m_eventSource.m_eventData[this.EventId].Descriptor.Opcode;

    public EventTask Task => (EventTask) this.m_eventSource.m_eventData[this.EventId].Descriptor.Task;

    public string Message => this.m_eventSource.m_eventData[this.EventId].Message;

    public EventChannel Channel => (EventChannel) this.m_eventSource.m_eventData[this.EventId].Descriptor.Channel;

    public byte Version => this.m_eventSource.m_eventData[this.EventId].Descriptor.Version;

    public EventLevel Level => (uint) this.EventId >= (uint) this.m_eventSource.m_eventData.Length ? EventLevel.LogAlways : (EventLevel) this.m_eventSource.m_eventData[this.EventId].Descriptor.Level;

    internal EventWrittenEventArgs(EventSource eventSource) => this.m_eventSource = eventSource;
  }
}
