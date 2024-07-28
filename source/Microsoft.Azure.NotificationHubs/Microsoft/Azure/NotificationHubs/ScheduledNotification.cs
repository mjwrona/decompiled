// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.ScheduledNotification
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.Azure.NotificationHubs
{
  [DataContract(Name = "ScheduledNotification", Namespace = "http://schemas.microsoft.com/netservices/2010/10/servicebus/connect")]
  public class ScheduledNotification
  {
    [DataMember(Name = "Payload", IsRequired = true, Order = 1001, EmitDefaultValue = true)]
    public Notification Payload { get; internal set; }

    [DataMember(Name = "ScheduledNotificationId", IsRequired = true, Order = 1002, EmitDefaultValue = true)]
    public string ScheduledNotificationId { get; internal set; }

    [DataMember(Name = "ScheduledTime", IsRequired = true, Order = 1003, EmitDefaultValue = true)]
    public DateTimeOffset ScheduledTime { get; internal set; }

    [DataMember(Name = "Tags", IsRequired = true, Order = 1004, EmitDefaultValue = true)]
    public string Tags { get; internal set; }

    [DataMember(Name = "TrackingId", IsRequired = true, Order = 1005, EmitDefaultValue = true)]
    public string TrackingId { get; internal set; }
  }
}
