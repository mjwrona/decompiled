// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.Server.NotificationBacklogStatusBinder
// Assembly: Microsoft.VisualStudio.Services.Notifications.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8ED1F52D-E567-4EFA-AAED-F2D9262BE9C2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Notifications.WebApi;
using System;
using System.Data;

namespace Microsoft.VisualStudio.Services.Notifications.Server
{
  internal class NotificationBacklogStatusBinder : ObjectBinder<NotificationBacklogStatus>
  {
    protected SqlColumnBinder UnprocessedNotificationsColumn = new SqlColumnBinder("UnprocessedNotifications");
    protected SqlColumnBinder MaxUnprocessedNotificationAgeMsColumn = new SqlColumnBinder("MaxUnprocessedNotificationAgeMs");
    protected SqlColumnBinder ChannelColumn = new SqlColumnBinder("Channel");

    protected override NotificationBacklogStatus Bind() => new NotificationBacklogStatus()
    {
      UnprocessedNotifications = this.UnprocessedNotificationsColumn.GetInt32((IDataReader) this.Reader),
      OldestPendingNotificationTime = DateTime.UtcNow.AddMilliseconds((double) this.MaxUnprocessedNotificationAgeMsColumn.GetInt32((IDataReader) this.Reader)),
      Channel = this.ChannelColumn.GetString((IDataReader) this.Reader, true)
    };
  }
}
