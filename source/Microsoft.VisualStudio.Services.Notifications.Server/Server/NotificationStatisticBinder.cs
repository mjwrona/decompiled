// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.Server.NotificationStatisticBinder
// Assembly: Microsoft.VisualStudio.Services.Notifications.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8ED1F52D-E567-4EFA-AAED-F2D9262BE9C2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Notifications.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System.Data;

namespace Microsoft.VisualStudio.Services.Notifications.Server
{
  internal class NotificationStatisticBinder : ObjectBinder<NotificationStatistic>
  {
    private SqlColumnBinder TrackingDateColumn = new SqlColumnBinder("TrackingDate");
    private SqlColumnBinder StatisticTypeColumn = new SqlColumnBinder("StatisticType");
    private SqlColumnBinder StatisticPathColumn = new SqlColumnBinder("StatisticPath");
    private SqlColumnBinder UserIdColumn = new SqlColumnBinder("UserId");
    private SqlColumnBinder HitCountColumn = new SqlColumnBinder("HitCount");

    protected override NotificationStatistic Bind()
    {
      NotificationStatistic notificationStatistic = new NotificationStatistic()
      {
        Date = this.TrackingDateColumn.GetDateTime((IDataReader) this.Reader),
        Type = (NotificationStatisticType) this.StatisticTypeColumn.GetInt32((IDataReader) this.Reader),
        HitCount = this.HitCountColumn.GetInt32((IDataReader) this.Reader)
      };
      switch (notificationStatistic.Type)
      {
        case NotificationStatisticType.NotificationBySubscription:
        case NotificationStatisticType.EventsByEventType:
        case NotificationStatisticType.NotificationByEventType:
        case NotificationStatisticType.EventsByEventTypePerUser:
        case NotificationStatisticType.NotificationByEventTypePerUser:
        case NotificationStatisticType.HourlyNotificationBySubscription:
        case NotificationStatisticType.HourlyEventsByEventTypePerUser:
        case NotificationStatisticType.HourlyNotifications:
          notificationStatistic.Path = EventTypeMapper.ToContributed((IVssRequestContext) null, this.StatisticPathColumn.GetString((IDataReader) this.Reader, true));
          break;
        default:
          notificationStatistic.Path = this.StatisticPathColumn.GetString((IDataReader) this.Reader, true);
          break;
      }
      if (!this.UserIdColumn.IsNull((IDataReader) this.Reader))
        notificationStatistic.User = new IdentityRef()
        {
          Id = this.UserIdColumn.GetGuid((IDataReader) this.Reader, false).ToString()
        };
      return notificationStatistic;
    }
  }
}
