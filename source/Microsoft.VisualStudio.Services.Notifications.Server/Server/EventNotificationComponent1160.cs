// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.Server.EventNotificationComponent1160
// Assembly: Microsoft.VisualStudio.Services.Notifications.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8ED1F52D-E567-4EFA-AAED-F2D9262BE9C2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.Server.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.Framework.Server;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Notifications.Server
{
  internal class EventNotificationComponent1160 : EventNotificationComponent1150
  {
    protected override void BindNotificationProcessQueues(IEnumerable<string> processQueues) => this.BindStringTable("@processQueues", processQueues, true, 105, false);

    protected override string NotificationTableName => "typ_NotificationTable7";

    protected override SqlDataRecord NotificationTableRecord => new SqlDataRecord(NotificationEventTypes.typ_NotificationTable7);
  }
}
