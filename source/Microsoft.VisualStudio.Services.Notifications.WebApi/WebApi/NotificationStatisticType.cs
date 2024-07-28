// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.WebApi.NotificationStatisticType
// Assembly: Microsoft.VisualStudio.Services.Notifications.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FF217E0A-7730-437B-BE9F-877363CB7392
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.WebApi.dll

namespace Microsoft.VisualStudio.Services.Notifications.WebApi
{
  public enum NotificationStatisticType
  {
    NotificationBySubscription = 0,
    EventsByEventType = 1,
    NotificationByEventType = 2,
    EventsByEventTypePerUser = 3,
    NotificationByEventTypePerUser = 4,
    Events = 5,
    Notifications = 6,
    NotificationFailureBySubscription = 7,
    UnprocessedRangeStart = 100, // 0x00000064
    UnprocessedEventsByPublisher = 101, // 0x00000065
    UnprocessedEventDelayByPublisher = 102, // 0x00000066
    UnprocessedNotificationsByChannelByPublisher = 103, // 0x00000067
    UnprocessedNotificationDelayByChannelByPublisher = 104, // 0x00000068
    DelayRangeStart = 200, // 0x000000C8
    TotalPipelineTime = 201, // 0x000000C9
    NotificationPipelineTime = 202, // 0x000000CA
    EventPipelineTime = 203, // 0x000000CB
    HourlyRangeStart = 1000, // 0x000003E8
    HourlyNotificationBySubscription = 1001, // 0x000003E9
    HourlyEventsByEventTypePerUser = 1002, // 0x000003EA
    HourlyEvents = 1003, // 0x000003EB
    HourlyNotifications = 1004, // 0x000003EC
    HourlyUnprocessedEventsByPublisher = 1101, // 0x0000044D
    HourlyUnprocessedEventDelayByPublisher = 1102, // 0x0000044E
    HourlyUnprocessedNotificationsByChannelByPublisher = 1103, // 0x0000044F
    HourlyUnprocessedNotificationDelayByChannelByPublisher = 1104, // 0x00000450
    HourlyTotalPipelineTime = 1201, // 0x000004B1
    HourlyNotificationPipelineTime = 1202, // 0x000004B2
    HourlyEventPipelineTime = 1203, // 0x000004B3
  }
}
