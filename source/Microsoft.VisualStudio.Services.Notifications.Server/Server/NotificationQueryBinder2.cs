// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.Server.NotificationQueryBinder2
// Assembly: Microsoft.VisualStudio.Services.Notifications.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8ED1F52D-E567-4EFA-AAED-F2D9262BE9C2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.VisualStudio.Services.Notifications.Server
{
  internal class NotificationQueryBinder2 : NotificationQueryBinder
  {
    protected SqlColumnBinder AttemptsColumn = new SqlColumnBinder("Attempts");
    protected SqlColumnBinder NextProcessTimeColumn = new SqlColumnBinder("NextProcessTime");
    protected SqlColumnBinder ExpirationTimeColumn = new SqlColumnBinder("ExpirationTime");

    protected override TeamFoundationNotification BindNotificationQuery() => new TeamFoundationNotification()
    {
      Id = this.IdColumn.GetInt32((IDataReader) this.Reader),
      Attempts = this.AttemptsColumn.GetInt32((IDataReader) this.Reader),
      Channel = this.ChannelColumn.GetString((IDataReader) this.Reader, true),
      CreatedTime = this.CreatedTimeColumn.GetDateTime((IDataReader) this.Reader),
      ProcessedTime = this.ProcessedTimeColumn.GetDateTime((IDataReader) this.Reader),
      DeliveryDetails = new NotificationDeliveryDetails()
      {
        ChannelMetadata = this.NotificationMetadataColumn.GetString((IDataReader) this.Reader, true),
        NotificationSource = this.NotificationSourceColumn.GetString((IDataReader) this.Reader, true)
      },
      Result = this.ResultColumn.GetString((IDataReader) this.Reader, true) ?? string.Empty,
      ResultDetail = this.ResultDetailColumn.ColumnExists((IDataReader) this.Reader) ? this.ResultDetailColumn.GetString((IDataReader) this.Reader, true) : (string) null,
      SingleSubscriberOverrideId = this.SubscriberOverrideIdColumn.GetGuid((IDataReader) this.Reader),
      ProcessQueue = this.ProcessQueueColumn.GetString((IDataReader) this.Reader, true),
      NextProcessTime = this.NextProcessTimeColumn.GetDateTime((IDataReader) this.Reader),
      ExpirationTime = this.ExpirationTimeColumn.GetDateTime((IDataReader) this.Reader)
    };
  }
}
