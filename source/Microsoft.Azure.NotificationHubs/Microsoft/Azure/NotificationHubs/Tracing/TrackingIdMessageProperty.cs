// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.Tracing.TrackingIdMessageProperty
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using System;
using System.Collections.Generic;

namespace Microsoft.Azure.NotificationHubs.Tracing
{
  internal class TrackingIdMessageProperty
  {
    public TrackingIdMessageProperty()
      : this(Guid.NewGuid().ToString())
    {
    }

    public TrackingIdMessageProperty(string trackingId) => this.Id = trackingId;

    public string Id { get; private set; }

    public static bool TryAdd(IDictionary<string, object> messageProperties, string trackingId)
    {
      if (messageProperties == null || TrackingIdMessageProperty.TryGet<TrackingIdMessageProperty>(messageProperties, out TrackingIdMessageProperty _))
        return false;
      messageProperties.Add("TrackingId", (object) new TrackingIdMessageProperty(trackingId));
      return true;
    }

    public static TrackingIdMessageProperty Read(IDictionary<string, object> messageProperties)
    {
      TrackingIdMessageProperty property;
      if (!TrackingIdMessageProperty.TryGet<TrackingIdMessageProperty>(messageProperties, out property))
        throw new ArgumentException(SRClient.TrackingIDPropertyMissing, nameof (messageProperties));
      return property;
    }

    public static bool Remove(IDictionary<string, object> messageProperties) => messageProperties != null && messageProperties.Remove("TrackingId");

    public static bool TryGet<T>(IDictionary<string, object> messageProperties, out T property) where T : class
    {
      property = default (T);
      if (messageProperties != null)
      {
        object obj;
        if (!messageProperties.TryGetValue("TrackingId", out obj))
          return false;
        property = obj as T;
      }
      return (object) property != null;
    }
  }
}
