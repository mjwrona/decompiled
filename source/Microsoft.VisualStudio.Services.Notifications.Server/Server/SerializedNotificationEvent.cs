// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.Server.SerializedNotificationEvent
// Assembly: Microsoft.VisualStudio.Services.Notifications.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8ED1F52D-E567-4EFA-AAED-F2D9262BE9C2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.Server.dll

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Notifications.Server
{
  [NotificationsEmbedType]
  public class SerializedNotificationEvent : VssNotificationEvent
  {
    public SerializedNotificationEvent()
    {
    }

    public SerializedNotificationEvent(object data)
      : base(data)
    {
    }

    public SerializedNotificationEvent(string serializedEvent, string eventType)
      : base(serializedEvent, eventType)
    {
    }

    public SerializedNotificationEvent(VssNotificationEvent other) => this.CloneFrom(other);

    protected override void CloneFrom(VssNotificationEvent other)
    {
      base.CloneFrom(other);
      if (!(other is SerializedNotificationEvent notificationEvent))
        return;
      this.ProcessQueue = notificationEvent.ProcessQueue;
      HashSet<string> allowedChannels = notificationEvent.AllowedChannels;
      this.AllowedChannels = allowedChannels != null ? allowedChannels.ToHashSet() : (HashSet<string>) null;
    }

    [DataMember]
    public string ProcessQueue { get; set; }

    [DataMember]
    public HashSet<string> AllowedChannels { get; set; }

    [DataMember]
    public string Status { get; set; }

    [DataMember]
    public DateTime VssNotificationEventCreatedTime => this.m_createdTime;
  }
}
