// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.AuditLog.NotificationAuditingConstants
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

namespace Microsoft.TeamFoundation.Framework.Server.AuditLog
{
  public static class NotificationAuditingConstants
  {
    public static readonly string SubscriptionManagement = nameof (SubscriptionManagement);
    public static readonly string SubscriberManagement = nameof (SubscriberManagement);
    public static readonly string AdminSettingsManagement = nameof (AdminSettingsManagement);
    public static readonly string CreateSubscriptionAction = "CreateSubscription";
    public static readonly string UpdateSubscriptionAction = "UpdateSubscription";
    public static readonly string UserOptOutAction = "UserOptOut";
    public static readonly string UserOptInAction = "UserOptIn";
    public static readonly string AdminSettingsAction = "AdminSettings";
    public static readonly string DeleteSubscriptionAction = "DeleteSubscription";
    public static readonly string UpdateSubscriberAction = "UpdateSubscriber";
    public static readonly string UpdateAdminSettingsAction = "UpdateAdminSettings";
  }
}
