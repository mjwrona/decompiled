// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.Server.EventNotificationComponent1290
// Assembly: Microsoft.VisualStudio.Services.Notifications.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8ED1F52D-E567-4EFA-AAED-F2D9262BE9C2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.Server.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.Framework.Server;

namespace Microsoft.VisualStudio.Services.Notifications.Server
{
  internal class EventNotificationComponent1290 : EventNotificationComponent1260
  {
    protected override string SubscriptionUpdateTypeName => "typ_SubscriptionUpdate5";

    protected override SqlMetaData[] SubscriptionUpdateType => NotificationEventTypes.typ_SubscriptionUpdate5;

    protected override SqlDataRecord BindSubscriptionUpdateRecord(SubscriptionUpdate update)
    {
      SqlDataRecord record = base.BindSubscriptionUpdateRecord(update);
      record.SetNullableString(15, DiagnosticUtils.SerializeDiagnostics(update.Diagnostics));
      return record;
    }

    protected override string SubcriptionTypeName => "typ_Subscription4";

    protected override SqlMetaData[] SubscriptionType => NotificationEventTypes.typ_Subscription4;

    protected override SqlDataRecord BindSubscriptionRecord(Subscription subscription)
    {
      SqlDataRecord record = base.BindSubscriptionRecord(subscription);
      record.SetNullableString(17, DiagnosticUtils.SerializeDiagnostics(subscription.Diagnostics));
      return record;
    }
  }
}
