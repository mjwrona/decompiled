// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.Tracing.EventAttribute
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using System;

namespace Microsoft.Azure.NotificationHubs.Tracing
{
  [AttributeUsage(AttributeTargets.Method)]
  internal sealed class EventAttribute : Attribute
  {
    public EventAttribute(int eventId)
    {
      this.EventId = eventId;
      this.Level = EventLevel.Informational;
    }

    public int EventId { get; private set; }

    public EventLevel Level { get; set; }

    public EventKeywords Keywords { get; set; }

    public EventOpcode Opcode { get; set; }

    public EventTask Task { get; set; }

    public EventChannel Channel { get; set; }

    public byte Version { get; set; }

    public string Message { get; set; }
  }
}
