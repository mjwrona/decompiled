// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceHooks.Sdk.Server.HooksPublisherNotificationsController
// Assembly: Microsoft.VisualStudio.Services.ServiceHooks.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3575E571-FF3A-4E7B-A8CC-64FFB01E8C91
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ServiceHooks.Sdk.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Notifications.Common;
using Microsoft.VisualStudio.Services.Notifications.Server;
using Microsoft.VisualStudio.Services.ServiceHooks.WebApi;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ServiceHooks.Sdk.Server
{
  [VersionedApiControllerCustomName(Area = "hooks", ResourceName = "Notifications")]
  public class HooksPublisherNotificationsController : ServiceHooksPublisherControllerBase
  {
    private const int c_defaultMaxResults = 100;

    public IEnumerable<Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Notification> GetNotifications(
      Guid subscriptionId,
      int? maxResults = 100,
      Microsoft.VisualStudio.Services.ServiceHooks.WebApi.NotificationStatus? status = null,
      NotificationResult? result = null)
    {
      Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription subscription = this.GetHooksService().GetSubscription(this.TfsRequestContext, subscriptionId);
      ServiceHooksPublisher publisher = this.FindPublisher(subscription.PublisherId);
      publisher.CheckPermission(this.TfsRequestContext, subscription);
      this.CheckScope(subscription.EventType);
      IList<Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Notification> notifications = publisher.GetHooksService(this.TfsRequestContext).GetNotifications(this.TfsRequestContext, subscriptionId, status, result, maxResults);
      if (!this.CanUserViewDiagnostics(this.TfsRequestContext, subscription, publisher))
        this.ScrubNotificationHistory(this.TfsRequestContext, notifications);
      return (IEnumerable<Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Notification>) notifications;
    }

    public Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Notification GetNotification(
      Guid subscriptionId,
      int notificationId)
    {
      ServiceHooksClientService hooksService = this.GetHooksService();
      Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Notification notification;
      if (subscriptionId == Guid.Empty)
      {
        notification = hooksService.GetNotification(this.TfsRequestContext, Guid.Empty, notificationId);
        if (notification.Details == null)
          throw new NotificationNotFoundException(notificationId);
        this.CheckScope(notification.Details.EventType);
        this.FindPublisher(notification.Details.PublisherId).CheckPermission(this.TfsRequestContext, notification, false);
      }
      else
      {
        Microsoft.VisualStudio.Services.Notifications.Server.Subscription localSubscription = this.GetLocalSubscription(this.TfsRequestContext, subscriptionId);
        Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription subscription = localSubscription != null ? localSubscription.GetHooksSubscription() : hooksService.GetSubscription(this.TfsRequestContext, subscriptionId);
        ServiceHooksPublisher publisher = this.FindPublisher(subscription.PublisherId);
        publisher.CheckPermission(this.TfsRequestContext, subscription, false);
        this.CheckScope(subscription.EventType);
        notification = localSubscription != null ? this.GetLocalHooksNotification(this.TfsRequestContext, notificationId) : hooksService.GetNotification(this.TfsRequestContext, subscriptionId, notificationId);
        if (!this.CanUserViewDiagnostics(this.TfsRequestContext, subscription, publisher))
          this.ScrubNotificationHistory(this.TfsRequestContext, notification);
      }
      return notification;
    }

    private Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Notification GetLocalHooksNotification(
      IVssRequestContext requestContext,
      int notificationId)
    {
      IEventNotificationService service = requestContext.GetService<IEventNotificationService>();
      NotificationLookup notificationLookup = new NotificationLookup(notificationId, true);
      IVssRequestContext requestContext1 = requestContext;
      NotificationLookup[] notificationKeys = new NotificationLookup[1]
      {
        notificationLookup
      };
      TeamFoundationNotification foundationNotification = service.QueryNotifications(requestContext1, (IEnumerable<NotificationLookup>) notificationKeys).FirstOrDefault<TeamFoundationNotification>();
      if (foundationNotification == null || foundationNotification.Result == null)
        return (Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Notification) null;
      Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Notification hooksNotification = JsonConvert.DeserializeObject<Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Notification>(foundationNotification.Result, NotificationsSerialization.JsonSerializerSettings);
      if (foundationNotification.ResultDetail != null)
      {
        NotificationDetails notificationDetails = JsonConvert.DeserializeObject<NotificationDetails>(foundationNotification.ResultDetail, NotificationsSerialization.JsonSerializerSettings);
        hooksNotification.Details = notificationDetails;
      }
      return hooksNotification;
    }
  }
}
