// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceHooks.Server.IServiceHooksComponent
// Assembly: Microsoft.VisualStudio.Services.ServiceHooks.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 25B6D63E-3809-4A04-9AE1-1F77D8FFEE67
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ServiceHooks.Server.dll

using Microsoft.VisualStudio.Services.ServiceHooks.WebApi;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.ServiceHooks.Server
{
  public interface IServiceHooksComponent : IDisposable
  {
    void CleanupNotificationDetails(int deleteOlderThanDays);

    void CreateNotification(
      Notification notification,
      bool allowFullContent,
      out int payloadLength);

    Subscription CreateSubscription(Subscription subscription);

    void DeleteSubscription(Guid subscriptionId);

    ServiceHooksStats GetStats();

    IList<Notification> QueryNotifications(
      Guid? subscriptionId,
      NotificationStatus? status,
      NotificationResult? result,
      DateTime? minDate,
      DateTime? maxDate,
      int? maxResults);

    IList<Notification> QueryNotifications(
      IEnumerable<Guid> subscriptionIds,
      NotificationStatus? status,
      NotificationResult? result,
      DateTime? minDate,
      DateTime? maxDate,
      int? maxResults,
      int? maxResultsPerSubScription);

    IList<Notification> QueryNotificationsWithDetails(
      NotificationStatus? status,
      NotificationResult? result,
      DateTime? minDate,
      DateTime? maxDate,
      int? maxResults);

    IList<NotificationResultsSummaryDetail> QueryNotificationSummary(
      Guid subscriptionId,
      NotificationStatus? status,
      NotificationResult? result,
      DateTime? minDate,
      DateTime? maxDate);

    IEnumerable<Subscription> QuerySubscriptions(
      SubscriptionStatus? status,
      string publisherId,
      string eventType,
      string consumerId,
      string consumerActionId);

    Notification ReadNotification(Guid subscriptionId, int notificationId);

    Subscription ReadSubscription(Guid subscriptionId);

    IList<Subscription> ReadSubscriptions(IEnumerable<Guid> subscriptionIds);

    void UpdateNotification(
      Guid subscriptionId,
      int notificationId,
      NotificationStatus? status,
      NotificationResult? result,
      string request,
      string response,
      string errorMessage,
      string errorDetail,
      DateTime? queuedDate,
      DateTime? dequeuedDate,
      DateTime? processedDate,
      DateTime? completedDate,
      double? requestDuration,
      bool incrementRequestAttempts);

    Subscription UpdateSubscription(
      Subscription subscription,
      bool updateProbationRetries,
      bool requestedByUser);

    void UpdateSubscriptionStatus(
      Guid subscriptionId,
      SubscriptionStatus status,
      Guid modifiedByIdentity,
      bool resetProbationRetries,
      bool incrementProbationRetries,
      bool requestedByUser);
  }
}
