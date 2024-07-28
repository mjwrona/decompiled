// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.Server.EventNotificationComponent108
// Assembly: Microsoft.VisualStudio.Services.Notifications.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8ED1F52D-E567-4EFA-AAED-F2D9262BE9C2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.Server.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Notifications.WebApi;
using System;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.VisualStudio.Services.Notifications.Server
{
  internal class EventNotificationComponent108 : EventNotificationComponent107
  {
    public override void SuspendUnprocessedNotifications(
      IEnumerable<NotificationQueryCondition> suspendNotificationKey,
      int batchSize)
    {
      this.PrepareStoredProcedure("prc_SuspendUnprocessedNotifications", 3600);
      this.BindSuspendNotificationKey("@notificationKeys", suspendNotificationKey);
      this.BindInt("@batchSize", batchSize);
      this.ExecuteNonQuery();
    }

    protected virtual void BindSuspendNotificationKey(
      string parameterName,
      IEnumerable<NotificationQueryCondition> keys)
    {
      List<SqlDataRecord> rows = new List<SqlDataRecord>();
      foreach (NotificationQueryCondition key in keys)
      {
        if (key != null)
        {
          SqlDataRecord record = new SqlDataRecord(NotificationEventTypes.typ_SuspendNotificationKey);
          record.SetGuid(0, key.Subscriber);
          record.SetGuid(1, key.EventInitiator);
          record.SetNullableString(2, key.SubscriptionId);
          record.SetNullableString(3, key.EventType);
          rows.Add(record);
        }
      }
      this.BindTable(parameterName, "typ_SuspendNotificationKey", (IEnumerable<SqlDataRecord>) rows);
    }

    public override int CleanupStatistics(
      int dailyStatisticsAgeDays,
      int hourlyStatisticsAgeDays,
      int keepTopN,
      int batchSize)
    {
      this.PrepareStoredProcedure("prc_CleanupStatistics", 14400);
      this.BindInt("@dailyStatisticsAgeDays", dailyStatisticsAgeDays);
      this.BindInt("@hourlyStatisticsAgeDays", hourlyStatisticsAgeDays);
      this.BindInt("@keepTopN", keepTopN);
      this.BindInt("@batchSize", batchSize);
      this.ExecuteNonQuery();
      return -1;
    }

    public override List<NotificationStatistic> QueryNotificationStatistics(
      IEnumerable<NotificationStatisticsQueryConditions> queries)
    {
      return new List<NotificationStatistic>();
    }

    public override NotificationEventBacklogStatus QueryNotificationBacklogStatus(
      int maxAllowedDelayDays,
      HashSet<Tuple<string, string>> processQueueChannels)
    {
      NotificationEventBacklogStatus eventBacklogStatus = new NotificationEventBacklogStatus();
      this.PrepareStoredProcedure("prc_QueryNotificationBacklogStatus");
      this.BindInt("@maxAllowedDelayDays", maxAllowedDelayDays);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<EventBacklogStatus>((ObjectBinder<EventBacklogStatus>) this.GetEventBacklogStatusVersion());
      resultCollection.AddBinder<NotificationBacklogStatus>((ObjectBinder<NotificationBacklogStatus>) this.GetNotificationBacklogStatusVersion());
      eventBacklogStatus.EventBacklogStatus = resultCollection.GetCurrent<EventBacklogStatus>().Items;
      resultCollection.NextResult();
      eventBacklogStatus.NotificationBacklogStatus = resultCollection.GetCurrent<NotificationBacklogStatus>().Items;
      return eventBacklogStatus;
    }

    protected override void BindEventEntryRow(
      SqlDataRecord record,
      SerializedNotificationEvent notifEvent)
    {
      base.BindEventEntryRow(record, notifEvent);
      record.SetGuid(5, notifEvent.GetInitiator());
    }

    protected override SqlDataRecord NewEventEntryRecord() => new SqlDataRecord(NotificationEventTypes.typ_EventEntryTable5);

    protected override string EventEntryTableName => "typ_EventEntryTable5";

    protected virtual SqlDataRecord SuspendNotificationTableRecord => new SqlDataRecord(NotificationEventTypes.typ_SuspendNotificationKey);
  }
}
