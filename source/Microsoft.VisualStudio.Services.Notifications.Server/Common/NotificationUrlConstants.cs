// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.Common.NotificationUrlConstants
// Assembly: Microsoft.VisualStudio.Services.Notifications.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8ED1F52D-E567-4EFA-AAED-F2D9262BE9C2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.Server.dll

namespace Microsoft.VisualStudio.Services.Notifications.Common
{
  public static class NotificationUrlConstants
  {
    public const string NotificationsPageRelativePath = "_notifications";
    public const string NotificationSubscriptionParam = "subscriptionId";
    public const string NotificationSubscriptionPublisherParam = "publisherId";
    public const string NotificationSubscriptionActionParam = "action";
    public const string NotificationSubscriptionUnsubscribeValue = "unsubscribe";
    public const string NotificationSubscriptionViewValue = "view";
    public const string AlertsPageRelativePath = "{0}#id={1}&showteams={2}";
    public const string AlertsPage = "_Alerts";
    public const string AlertsAdminPage = "_admin/_Alerts";
    public const string AdminPageRouteContributionId = "ms.vss-notifications-web.collection-admin-hub-route";
    public const string UserNotificationsRouteContributionId = "ms.vss-notifications-web.user-notifications-route";
    public const string TeamNotificationsRouteContributionId = "ms.vss-notifications-web.team-notifications-route";
  }
}
