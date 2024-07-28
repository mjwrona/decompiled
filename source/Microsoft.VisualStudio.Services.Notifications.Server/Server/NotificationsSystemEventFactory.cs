// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.Server.NotificationsSystemEventFactory
// Assembly: Microsoft.VisualStudio.Services.Notifications.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8ED1F52D-E567-4EFA-AAED-F2D9262BE9C2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Notifications.WebApi;

namespace Microsoft.VisualStudio.Services.Notifications.Server
{
  public class NotificationsSystemEventFactory : INotificationsSystemEventFactory
  {
    public VssNotificationEvent CreateSubscriptionDisabledEvent(
      IVssRequestContext requestContext,
      Subscription subscription,
      SubscriptionUpdate subscriptionUpdate)
    {
      if (!subscription.IsServiceHooksDelivery)
      {
        SubscriptionStatus? status = subscriptionUpdate.Status;
        SubscriptionStatus subscriptionStatus1 = SubscriptionStatus.DisabledInvalidPathClause;
        if (!(status.GetValueOrDefault() == subscriptionStatus1 & status.HasValue))
        {
          status = subscriptionUpdate.Status;
          SubscriptionStatus subscriptionStatus2 = SubscriptionStatus.JailedByNotificationsVolume;
          if (!(status.GetValueOrDefault() == subscriptionStatus2 & status.HasValue))
          {
            status = subscriptionUpdate.Status;
            SubscriptionStatus subscriptionStatus3 = SubscriptionStatus.DisabledInvalidRoleExpression;
            if (!(status.GetValueOrDefault() == subscriptionStatus3 & status.HasValue))
            {
              status = subscriptionUpdate.Status;
              SubscriptionStatus subscriptionStatus4 = SubscriptionStatus.DisabledArgumentException;
              if (!(status.GetValueOrDefault() == subscriptionStatus4 & status.HasValue))
                goto label_6;
            }
          }
        }
        return new VssNotificationEvent()
        {
          ItemId = subscription.SubscriptionId,
          EventType = "ms.vss-notifications.subscription-disabled-event",
          Actors = {
            new EventActor()
            {
              Id = subscription.SubscriberId,
              Role = SubscriptionDisabledEvent.Roles.Subscriber
            },
            new EventActor()
            {
              Id = subscription.LastModifiedBy,
              Role = SubscriptionDisabledEvent.Roles.AuthorizedAs
            }
          },
          Data = (object) new SubscriptionDisabledEvent()
          {
            SubscriptionId = subscription.SubscriptionId,
            SubscriptionTitle = subscription.Description,
            SubscriptionUri = NotificationSubscriptionService.GetMyNotificationsUrl(requestContext, subscription),
            Message = (subscriptionUpdate.StatusMessage ?? subscription.StatusMessage)
          }
        };
      }
label_6:
      return (VssNotificationEvent) null;
    }
  }
}
