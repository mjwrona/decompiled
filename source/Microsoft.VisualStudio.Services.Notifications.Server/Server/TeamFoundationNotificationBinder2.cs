// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.Server.TeamFoundationNotificationBinder2
// Assembly: Microsoft.VisualStudio.Services.Notifications.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8ED1F52D-E567-4EFA-AAED-F2D9262BE9C2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Data;

namespace Microsoft.VisualStudio.Services.Notifications.Server
{
  internal class TeamFoundationNotificationBinder2 : TeamFoundationNotificationBinder
  {
    protected SqlColumnBinder AttemptsColumn = new SqlColumnBinder("Attempts");
    protected SqlColumnBinder NextProcessTimeColumn = new SqlColumnBinder("NextProcessTime");
    protected SqlColumnBinder ExpirationTimeColumn = new SqlColumnBinder("ExpirationTime");

    internal TeamFoundationNotificationBinder2(TeamFoundationSqlResourceComponent component)
      : base(component)
    {
    }

    protected override TeamFoundationNotification BindNotification()
    {
      string eventType = this.EventTypeNameColumn.GetString((IDataReader) this.Reader, true);
      string eventData = this.EventColumn.GetString((IDataReader) this.Reader, true);
      TeamFoundationEvent teamFoundationEvent = (TeamFoundationEvent) null;
      if (!string.IsNullOrEmpty(eventType))
      {
        if (!string.IsNullOrEmpty(eventData))
        {
          try
          {
            teamFoundationEvent = TeamFoundationEventFactory.GetEvent(eventType, eventData);
            teamFoundationEvent.Id = this.EventIdColumn.GetInt32((IDataReader) this.Reader);
            teamFoundationEvent.CreatedTime = this.EventCreatedTimeColumn.GetDateTime((IDataReader) this.Reader);
          }
          catch (Exception ex)
          {
            teamFoundationEvent = (TeamFoundationEvent) null;
          }
        }
      }
      return new TeamFoundationNotification()
      {
        Id = this.IdColumn.GetInt32((IDataReader) this.Reader),
        Attempts = this.AttemptsColumn.GetInt32((IDataReader) this.Reader),
        Event = teamFoundationEvent,
        Channel = this.ChannelColumn.GetString((IDataReader) this.Reader, false),
        Metadata = this.MetadataColumnBinder.GetString((IDataReader) this.Reader, true) ?? string.Empty,
        CreatedTime = this.CreatedTimeColumn.GetDateTime((IDataReader) this.Reader),
        ProcessQueue = this.ProcessQueueColumn.GetString((IDataReader) this.Reader, string.Empty) ?? string.Empty,
        SingleSubscriberOverrideId = this.SubscriberOverrideIdColumn.GetGuid((IDataReader) this.Reader, true),
        BisEvent = this.BisEventColumn.GetInt32((IDataReader) this.Reader, -2),
        NextProcessTime = this.NextProcessTimeColumn.GetDateTime((IDataReader) this.Reader),
        ExpirationTime = this.ExpirationTimeColumn.GetDateTime((IDataReader) this.Reader)
      };
    }
  }
}
