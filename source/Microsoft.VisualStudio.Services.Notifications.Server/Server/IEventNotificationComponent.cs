// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.Server.IEventNotificationComponent
// Assembly: Microsoft.VisualStudio.Services.Notifications.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8ED1F52D-E567-4EFA-AAED-F2D9262BE9C2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.Server.dll

using Microsoft.VisualStudio.Services.Notifications.Common;
using Microsoft.VisualStudio.Services.Notifications.WebApi;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Notifications.Server
{
  public interface IEventNotificationComponent : IDisposable
  {
    void DeleteSubscription(int subscriptionId);

    void DeleteSubscriptions(IEnumerable<int> subscriptionIds);

    void FireEvents(IEnumerable<SerializedNotificationEvent> theEvents);

    List<DefaultSubscriptionAdminCandidate> GetDefaultSubscriptionsAdminDisabled(
      HashSet<string> candidates);

    List<DefaultSubscriptionUserCandidate> GetDefaultSubscriptionsUserDisabled(
      IEnumerable<DefaultSubscriptionUserCandidate> candidates);

    List<Subscription> GetSubscriptions(IEnumerable<string> eventTypes);

    List<TeamFoundationEvent> GetUnprocessedEvents(
      int eventBatchSize,
      IEnumerable<string> processQueues);

    List<TeamFoundationNotification> GetUnprocessedNotifications(
      int lastNotificationId,
      int notificationBatchSize,
      IEnumerable<string> channels,
      IEnumerable<string> processQueues,
      int failedRetryInterval = 0);

    (int, int) CleanupNotificationsEvents(int eventAgeMins, int notificationAgeMins, int batchSize);

    void CleanupSubscriptions(int subscriptionAgeDays, int batchSize);

    List<Subscription> QuerySubscriptions(
      IEnumerable<SubscriptionLookup> subscriptionKeys,
      bool includeDeleted);

    void SaveProcessedEvents(
      IList<TeamFoundationEvent> events,
      IList<TeamFoundationNotification> notifications);

    void SaveTransformedEvents(IList<TeamFoundationEvent> events);

    void SaveProcessedNotifications(IList<TeamFoundationNotification> notifications);

    int SubscribeEvent(Subscription subscription, bool allowDuplicates);

    void UpdateDefaultSubscriptionsAdminEnabled(
      string subscriptionName,
      bool disabled,
      bool blockUserDisable,
      Guid modifiedBy);

    void UpdateDefaultSubscriptionsUserEnabled(
      Guid subscriberId,
      string subscriptionName,
      bool enabled);

    void UpdateEventSubscription(List<SubscriptionUpdate> subscriptionUpdates);

    void UpdateSubscriptionProject(Subscription subscription, Guid newProjectId);

    int CleanupStatistics(
      int dailyStatisticsAgeDays,
      int hourlyStatisticsAgeDays,
      int keepTopN,
      int batchSize);

    void SuspendUnprocessedNotifications(
      IEnumerable<NotificationQueryCondition> notificationKeys,
      int batchSize);

    List<NotificationStatistic> QueryNotificationStatistics(
      IEnumerable<NotificationStatisticsQueryConditions> queries);

    NotificationEventBacklogStatus QueryNotificationBacklogStatus(
      int maxAllowedDelayDays,
      HashSet<Tuple<string, string>> processQueueChannels);

    void UpdateNotificationStatistics(IEnumerable<NotificationStatisticEntry> stats);

    List<TeamFoundationNotification> QueryNotifications(IEnumerable<NotificationLookup> lookups);

    void SaveNotificationLog(INotificationDiagnosticLog log);

    List<INotificationDiagnosticLog> QueryNotificationLog(NotificationDiagnosticsQuery query);

    int CleanupNotificationLog(int logAgeMins, int batchSize);

    DateTime GetNextNotificationProcessTime(
      IEnumerable<string> channels,
      IEnumerable<string> processQueues);

    DateTime GetNextEventProcessTime(IEnumerable<string> processQueues);

    void UpdateEventExpirationTime(IEnumerable<int> eventIds, DateTime expirationTime);
  }
}
