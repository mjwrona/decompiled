// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.Server.EventNotificationComponent1330
// Assembly: Microsoft.VisualStudio.Services.Notifications.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8ED1F52D-E567-4EFA-AAED-F2D9262BE9C2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.Server.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.Framework.Server;

namespace Microsoft.VisualStudio.Services.Notifications.Server
{
  internal class EventNotificationComponent1330 : EventNotificationComponent1321
  {
    protected override string SubscriptionUpdateTypeName => "typ_SubscriptionUpdate6";

    protected override SqlMetaData[] SubscriptionUpdateType => NotificationEventTypes.typ_SubscriptionUpdate6;

    protected override SqlDataRecord BindSubscriptionUpdateRecord(SubscriptionUpdate update)
    {
      SqlDataRecord record = base.BindSubscriptionUpdateRecord(update);
      record.SetNullableGuid(16, update.SubscriberId);
      return record;
    }
  }
}
