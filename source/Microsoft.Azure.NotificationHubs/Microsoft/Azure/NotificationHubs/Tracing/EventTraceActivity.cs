// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.Tracing.EventTraceActivity
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using System;
using System.Diagnostics;

namespace Microsoft.Azure.NotificationHubs.Tracing
{
  internal class EventTraceActivity
  {
    private static EventTraceActivity empty;
    public Guid ActivityId;

    public EventTraceActivity()
      : this(Guid.NewGuid())
    {
    }

    public EventTraceActivity(Guid activityId) => this.ActivityId = activityId;

    public static EventTraceActivity Empty
    {
      get
      {
        if (EventTraceActivity.empty == null)
          EventTraceActivity.empty = new EventTraceActivity(Guid.Empty);
        return EventTraceActivity.empty;
      }
    }

    public static string Name => "E2EActivity";

    public static EventTraceActivity CreateFromThread()
    {
      Guid activityId = Trace.CorrelationManager.ActivityId;
      return activityId == Guid.Empty ? EventTraceActivity.Empty : new EventTraceActivity(activityId);
    }
  }
}
