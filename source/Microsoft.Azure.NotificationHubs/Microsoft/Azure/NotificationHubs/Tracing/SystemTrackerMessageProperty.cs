// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.Tracing.SystemTrackerMessageProperty
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using System;
using System.Collections.Generic;

namespace Microsoft.Azure.NotificationHubs.Tracing
{
  internal class SystemTrackerMessageProperty
  {
    public SystemTrackerMessageProperty()
      : this(string.Empty)
    {
    }

    public SystemTrackerMessageProperty(string tracker) => this.Tracker = tracker;

    public string Tracker { get; private set; }

    public static bool TryAdd(IDictionary<string, object> messageProperties, string systemTracker)
    {
      if (messageProperties == null || SystemTrackerMessageProperty.TryGet<SystemTrackerMessageProperty>(messageProperties, out SystemTrackerMessageProperty _))
        return false;
      messageProperties.Add("SystemTracker", (object) new SystemTrackerMessageProperty(systemTracker));
      return true;
    }

    public static SystemTrackerMessageProperty Read(IDictionary<string, object> messageProperties)
    {
      SystemTrackerMessageProperty property;
      if (!SystemTrackerMessageProperty.TryGet<SystemTrackerMessageProperty>(messageProperties, out property))
        throw new ArgumentException(SRClient.SystemTrackerPropertyMissing, nameof (messageProperties));
      return property;
    }

    public static bool Remove(IDictionary<string, object> messageProperties) => messageProperties != null && messageProperties.Remove("SystemTracker");

    public static bool TryGet<T>(IDictionary<string, object> messageProperties, out T property) where T : class
    {
      property = default (T);
      if (messageProperties != null)
      {
        object obj;
        if (!messageProperties.TryGetValue("SystemTracker", out obj))
          return false;
        property = obj as T;
      }
      return (object) property != null;
    }
  }
}
