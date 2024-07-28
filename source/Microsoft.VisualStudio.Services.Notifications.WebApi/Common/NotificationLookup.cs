// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.Common.NotificationLookup
// Assembly: Microsoft.VisualStudio.Services.Notifications.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FF217E0A-7730-437B-BE9F-877363CB7392
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.WebApi.dll

using System;

namespace Microsoft.VisualStudio.Services.Notifications.Common
{
  public class NotificationLookup
  {
    public NotificationLookup()
    {
    }

    public NotificationLookup(int notificationId, bool includeDetail = false)
    {
      this.QueryType = NotificationLookupQueryType.ByNotificationId;
      this.NotificationId = new int?(notificationId);
      this.IncludeResultDetail = includeDetail;
    }

    public NotificationLookup(Guid subscriptionUniqueId, bool includeDetail = false)
    {
      this.QueryType = NotificationLookupQueryType.BySubscriptionUniqueId;
      this.SubscriptionUniqueId = new Guid?(subscriptionUniqueId);
      this.IncludeResultDetail = includeDetail;
    }

    public NotificationLookupQueryType QueryType { get; set; }

    public int? NotificationId { get; set; }

    public Guid? SubscriptionUniqueId { get; set; }

    public bool IncludeResultDetail { get; set; }
  }
}
