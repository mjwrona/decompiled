// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.Server.EventNotificationComponent1360
// Assembly: Microsoft.VisualStudio.Services.Notifications.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8ED1F52D-E567-4EFA-AAED-F2D9262BE9C2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.Server.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Notifications.Server
{
  internal class EventNotificationComponent1360 : EventNotificationComponent1340
  {
    protected override int ChannelLength => 100;

    protected override void BindEventEntryRow(
      SqlDataRecord record,
      SerializedNotificationEvent notifEvent)
    {
      DateTime utcNow = DateTime.UtcNow;
      record.SetString(0, notifEvent.EventType);
      record.SetString(1, notifEvent.SerializeEvent());
      record.SetGuid(2, notifEvent.GetInitiator());
      record.SetString(3, notifEvent.ProcessQueue ?? string.Empty);
      record.SetNullableString(4, notifEvent.Status);
      record.SetDateTime(5, utcNow.AddTimeSpan(notifEvent.ProcessDelay, true));
      record.SetDateTime(6, utcNow.AddTimeSpan(notifEvent.ExpiresIn, true));
      DateTime? eventCreatedTime = notifEvent.SourceEventCreatedTime;
      if (eventCreatedTime.HasValue)
      {
        SqlDataRecord record1 = record;
        eventCreatedTime = notifEvent.SourceEventCreatedTime;
        DateTime sql = eventCreatedTime.Value.ConstrainToSql();
        record1.SetNullableDateTime(7, sql);
      }
      else
        record.SetDBNull(7);
    }

    protected override SqlDataRecord NewEventEntryRecord() => new SqlDataRecord(NotificationEventTypes.typ_EventEntryTable7);

    protected override string EventEntryTableName => "typ_EventEntryTable7";

    protected override SqlParameter BindEventTable(
      string parameterName,
      IEnumerable<TeamFoundationEvent> rows)
    {
      rows = rows ?? Enumerable.Empty<TeamFoundationEvent>();
      System.Func<TeamFoundationEvent, SqlDataRecord> selector = (System.Func<TeamFoundationEvent, SqlDataRecord>) (teamFoundationEvent =>
      {
        SqlDataRecord record = new SqlDataRecord(NotificationEventTypes.typ_EventTable3);
        record.SetInt32(0, teamFoundationEvent.Id);
        record.SetInt32(1, teamFoundationEvent.Matches);
        record.SetString(2, teamFoundationEvent.Status ?? "Processed");
        record.SetDateTime(3, teamFoundationEvent.NextProcessTime.ConstrainToSql());
        record.SetNullableDateTime(4, teamFoundationEvent.ExpirationTime != DateTimeUtils.SqlDateMinValue ? teamFoundationEvent.ExpirationTime.ConstrainToSql() : DateTime.MinValue);
        return record;
      });
      return this.BindTable(parameterName, "typ_EventTable3", rows.Select<TeamFoundationEvent, SqlDataRecord>(selector));
    }

    public override DateTime GetNextEventProcessTime(IEnumerable<string> processQueues)
    {
      this.PrepareStoredProcedure("prc_GetNextEventProcessTime");
      this.BindStringTable("@processQueues", processQueues, nvarchar: false);
      using (ResultCollection rc = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        return this.GetNextProcessedTime(rc);
    }

    public override DateTime GetNextNotificationProcessTime(
      IEnumerable<string> channels,
      IEnumerable<string> processQueues)
    {
      this.PrepareStoredProcedure("prc_GetNextNotificationProcessTime");
      this.BindProcessQueueChannels("@processQueueChannels", channels, processQueues);
      using (ResultCollection rc = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        return this.GetNextProcessedTime(rc);
    }

    protected virtual DateTime GetNextProcessedTime(ResultCollection rc)
    {
      SqlColumnBinder nextProcessTimeBinder = new SqlColumnBinder("NextProcessTime");
      rc.AddBinder<DateTime>((ObjectBinder<DateTime>) new SimpleObjectBinder<DateTime>((System.Func<IDataReader, DateTime>) (reader => nextProcessTimeBinder.GetDateTime(reader))));
      List<DateTime> items = rc.GetCurrent<DateTime>().Items;
      return !items.Any<DateTime>() ? DateTime.MaxValue : items.First<DateTime>();
    }

    protected virtual void BindProcessQueueChannels(
      string pqcFieldName,
      string cFieldName,
      HashSet<Tuple<string, string>> processQueueChannels)
    {
      bool flag1 = !string.IsNullOrEmpty(pqcFieldName);
      bool flag2 = !string.IsNullOrEmpty(cFieldName);
      List<SqlDataRecord> rows1 = flag1 ? new List<SqlDataRecord>() : (List<SqlDataRecord>) null;
      List<SqlDataRecord> rows2 = flag2 ? new List<SqlDataRecord>() : (List<SqlDataRecord>) null;
      foreach (Tuple<string, string> processQueueChannel in processQueueChannels)
      {
        if (flag1)
        {
          SqlDataRecord sqlDataRecord = new SqlDataRecord(NotificationEventTypes.typ_ProcessQueueChannelTable);
          sqlDataRecord.SetString(0, processQueueChannel.Item1);
          sqlDataRecord.SetString(1, processQueueChannel.Item2);
          rows1.Add(sqlDataRecord);
        }
        if (flag2)
        {
          SqlDataRecord sqlDataRecord = new SqlDataRecord(NotificationEventTypes.typ_StringVarcharTable);
          sqlDataRecord.SetString(0, processQueueChannel.Item1);
          rows2.Add(sqlDataRecord);
        }
      }
      if (flag1)
        this.BindTable(pqcFieldName, "typ_ProcessQueueChannelTable", (IEnumerable<SqlDataRecord>) rows1);
      if (!flag2)
        return;
      this.BindTable(cFieldName, "typ_StringVarcharTable", (IEnumerable<SqlDataRecord>) rows2);
    }

    protected virtual void BindProcessQueueChannels(
      string pqcfieldName,
      IEnumerable<string> channels,
      IEnumerable<string> processQueues)
    {
      HashSet<Tuple<string, string>> processQueueChannels = new HashSet<Tuple<string, string>>();
      foreach (string channel in channels)
      {
        foreach (string processQueue in processQueues)
          processQueueChannels.Add(new Tuple<string, string>(processQueue ?? string.Empty, channel));
      }
      this.BindProcessQueueChannels(pqcfieldName, (string) null, processQueueChannels);
    }

    public override List<TeamFoundationNotification> GetUnprocessedNotificationsWorker(
      int lastNotificationId,
      int notificationBatchSize,
      IEnumerable<string> channels,
      IEnumerable<string> processQueues,
      int failedRetryInterval)
    {
      this.PrepareStoredProcedure("prc_GetUnprocessedNotifications");
      this.BindInt("@batchSize", notificationBatchSize);
      this.BindProcessQueueChannels("@processQueueChannels", channels, processQueues);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<TeamFoundationNotification>((ObjectBinder<TeamFoundationNotification>) this.GetNotificationVersion());
      return resultCollection.GetCurrent<TeamFoundationNotification>().Items;
    }

    protected override string NotificationTableName => "typ_NotificationTable8";

    protected override SqlDataRecord NotificationTableRecord => new SqlDataRecord(NotificationEventTypes.typ_NotificationTable8);

    protected override SqlDataRecord NotificationTableRowBinder(
      TeamFoundationNotification notification)
    {
      SqlDataRecord notificationTableRecord = this.NotificationTableRecord;
      notificationTableRecord.SetInt32(0, notification.Event.Id);
      notificationTableRecord.SetGuid(1, notification.SingleSubscriberOverrideId);
      notificationTableRecord.SetString(2, notification.Channel);
      notificationTableRecord.SetNullableString(3, notification.Metadata);
      notificationTableRecord.SetNullableString(4, notification.DeliveryDetails?.NotificationSource);
      notificationTableRecord.SetString(5, notification.ProcessQueue ?? string.Empty);
      notificationTableRecord.SetDateTime(6, notification.NextProcessTime.ConstrainToSql());
      notificationTableRecord.SetDateTime(7, notification.ExpirationTime.ConstrainToSql());
      return notificationTableRecord;
    }

    internal override TeamFoundationNotificationBinder GetNotificationVersion() => (TeamFoundationNotificationBinder) new TeamFoundationNotificationBinder2((TeamFoundationSqlResourceComponent) this);

    internal override NotificationQueryBinder GetNotificationQueryVersion() => (NotificationQueryBinder) new NotificationQueryBinder2();

    protected override SqlDataRecord BindNotificationStatusRow(
      TeamFoundationNotification notification)
    {
      SqlDataRecord record = base.BindNotificationStatusRow(notification);
      record.SetDateTime(4, notification.NextProcessTime.ConstrainToSql());
      record.SetNullableDateTime(5, notification.ExpirationTime != DateTimeUtils.SqlDateMinValue ? notification.ExpirationTime.ConstrainToSql() : DateTime.MinValue);
      return record;
    }

    protected override string NotificationStatusTypeName => "typ_NotificationStatusTable3";

    protected override SqlMetaData[] NotificationStatusType => NotificationEventTypes.typ_NotificationStatusTable3;

    public override (int, int) CleanupNotificationsEvents(
      int eventAgeMins,
      int notificationAgeMins,
      int batchSize)
    {
      this.PrepareStoredProcedure("prc_CleanupEventsNotifications", 3600);
      this.BindInt("@batchSize", batchSize);
      this.ExecuteNonQuery();
      return (-1, -1);
    }

    protected override SubscriptionLookupType MaxSubscriptionLookupType => SubscriptionLookupType.SubscriptionId;
  }
}
