// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.Server.EventNotificationComponentExtensions
// Assembly: Microsoft.VisualStudio.Services.Notifications.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8ED1F52D-E567-4EFA-AAED-F2D9262BE9C2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.Server.dll

using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Notifications.Server
{
  public static class EventNotificationComponentExtensions
  {
    public static Subscription GetSubscription(
      this EventNotificationComponent component,
      int subscriptionId,
      bool includeDeleted = false)
    {
      List<SubscriptionLookup> subscriptionKeys = new List<SubscriptionLookup>()
      {
        SubscriptionLookup.CreateSubscriptionIdLookup(subscriptionId)
      };
      return component.QuerySubscriptions((IEnumerable<SubscriptionLookup>) subscriptionKeys, includeDeleted).FirstOrDefault<Subscription>();
    }
  }
}
