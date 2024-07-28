// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.Server.NotificationBacklogStatusBinder1380
// Assembly: Microsoft.VisualStudio.Services.Notifications.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8ED1F52D-E567-4EFA-AAED-F2D9262BE9C2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Notifications.WebApi;
using System;
using System.Data;

namespace Microsoft.VisualStudio.Services.Notifications.Server
{
  internal class NotificationBacklogStatusBinder1380 : NotificationBacklogStatusBinder1200
  {
    protected SqlColumnBinder OldestCreatedTimeColumn = new SqlColumnBinder("OldestCreatedTime");
    protected SqlColumnBinder StatusColumn = new SqlColumnBinder("Status");
    protected SqlColumnBinder CaptureTimeColumn = new SqlColumnBinder("CaptureTime");

    protected override NotificationBacklogStatus Bind()
    {
      NotificationBacklogStatus notificationBacklogStatus = new NotificationBacklogStatus()
      {
        UnprocessedNotifications = this.UnprocessedNotificationsColumn.GetInt32((IDataReader) this.Reader),
        OldestPendingNotificationTime = this.OldestCreatedTimeColumn.GetDateTime((IDataReader) this.Reader),
        Channel = this.ChannelColumn.GetString((IDataReader) this.Reader, true),
        Publisher = this.PublisherColumn.GetString((IDataReader) this.Reader, true),
        Status = this.StatusColumn.GetString((IDataReader) this.Reader, true),
        CaptureTime = this.CaptureTimeColumn.GetDateTime((IDataReader) this.Reader, DateTime.MinValue)
      };
      if (notificationBacklogStatus.CaptureTime <= DateTime.MinValue)
        notificationBacklogStatus.CaptureTime = DateTime.UtcNow;
      return notificationBacklogStatus;
    }
  }
}
