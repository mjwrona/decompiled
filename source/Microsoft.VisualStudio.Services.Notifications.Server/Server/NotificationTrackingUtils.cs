// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.Server.NotificationTrackingUtils
// Assembly: Microsoft.VisualStudio.Services.Notifications.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8ED1F52D-E567-4EFA-AAED-F2D9262BE9C2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Newtonsoft.Json.Linq;
using System;

namespace Microsoft.VisualStudio.Services.Notifications.Server
{
  internal static class NotificationTrackingUtils
  {
    public static NotificationTrackingData CreateTrackingData(
      IVssRequestContext requestContext,
      TeamFoundationNotification notification)
    {
      int result1 = 0;
      Guid result2 = Guid.Empty;
      string str1 = (string.IsNullOrEmpty(notification.DeliveryDetails.NotificationSource) || int.TryParse(notification.DeliveryDetails.NotificationSource, out result1) ? 1 : (Guid.TryParse(notification.DeliveryDetails.NotificationSource, out result2) ? 1 : 0)) == 0 ? "CON" : (notification.DeliveryDetails.SourceIdentity.IsContainer ? "GRP" : "PER");
      JObject data = new JObject();
      data.Add("Source", (JToken) "Email");
      data.Add("Type", (JToken) "Notification");
      data.Add("SID", (JToken) notification.DeliveryDetails.NotificationSource);
      data.Add("SType", (JToken) str1);
      data.Add("Recip", (JToken) notification.DeliveryDetails.Recipients.Count);
      string str2 = "unset";
      requestContext.TryGetItem<string>("$NotifMsgeRecipientCounts$", out str2);
      data.Add("_xci", (JToken) new JObject()
      {
        {
          "NID",
          (JToken) notification.Id
        },
        {
          "MRecip",
          (JToken) str2
        },
        {
          "Act",
          (JToken) requestContext.ActivityId
        }
      });
      return new NotificationTrackingData(data);
    }
  }
}
