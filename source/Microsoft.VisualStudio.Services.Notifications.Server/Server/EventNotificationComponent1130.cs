// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.Server.EventNotificationComponent1130
// Assembly: Microsoft.VisualStudio.Services.Notifications.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8ED1F52D-E567-4EFA-AAED-F2D9262BE9C2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.Server.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.VisualStudio.Services.Notifications.Server
{
  internal class EventNotificationComponent1130 : EventNotificationComponent1120
  {
    public override List<TeamFoundationNotification> GetUnprocessedNotificationsWorker(
      int lastNotificationId,
      int notificationBatchSize,
      IEnumerable<string> channels,
      IEnumerable<string> processQueues,
      int failedRetryInterval)
    {
      this.PrepareStoredProcedure("prc_GetUnprocessedNotifications");
      this.BindInt("@lastNotificationId", lastNotificationId);
      this.BindInt("@batchSize", notificationBatchSize);
      this.BindInt("@failedRetryInterval", failedRetryInterval);
      this.BindChannels(channels);
      this.BindNotificationProcessQueues(processQueues);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<TeamFoundationNotification>((ObjectBinder<TeamFoundationNotification>) this.GetNotificationVersion());
      return resultCollection.GetCurrent<TeamFoundationNotification>().Items;
    }

    protected virtual void BindNotificationProcessQueues(IEnumerable<string> processQueues) => this.BindStringTable("@processQueues", processQueues, true, 100, false);

    public override void UpdateDefaultSubscriptionsAdminEnabled(
      string subscriptionName,
      bool disabled,
      bool blockUserDisable,
      Guid modifiedBy)
    {
      this.PrepareStoredProcedure("prc_UpdateDefaultSubscriptionsAdminEnabled");
      this.BindString("@subscriptionName", subscriptionName, 100, false, SqlDbType.VarChar);
      this.BindBoolean("@disabled", disabled);
      this.BindBoolean("@blockUserDisable", blockUserDisable);
      this.BindGuid("@modifiedBy", modifiedBy);
      this.ExecuteNonQuery();
    }

    protected override SqlDataRecord NotificationTableRowBinder(
      TeamFoundationNotification notification)
    {
      SqlDataRecord sqlDataRecord = base.NotificationTableRowBinder(notification);
      sqlDataRecord.SetString(8, notification.ProcessQueue ?? string.Empty);
      return sqlDataRecord;
    }

    protected override string NotificationTableName => "typ_NotificationTable6";

    protected override SqlDataRecord NotificationTableRecord => new SqlDataRecord(NotificationEventTypes.typ_NotificationTable6);

    public override void SaveProcessedNotifications(IList<TeamFoundationNotification> notifications)
    {
      this.PrepareStoredProcedure("prc_SaveProcessedNotifications");
      this.BindNotificationStatus("@notificationResults", (IEnumerable<TeamFoundationNotification>) notifications);
      this.ExecuteNonQuery();
    }

    protected virtual void BindNotificationStatus(
      string parameterName,
      IEnumerable<TeamFoundationNotification> notifications)
    {
      List<SqlDataRecord> rows = new List<SqlDataRecord>();
      foreach (TeamFoundationNotification notification in notifications)
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(NotificationEventTypes.typ_NotificationStatusTable);
        sqlDataRecord.SetInt32(0, notification.Id);
        sqlDataRecord.SetString(1, notification.SendNotificationState.GetDbStateName());
        rows.Add(sqlDataRecord);
      }
      this.BindTable(parameterName, "typ_NotificationStatusTable", (IEnumerable<SqlDataRecord>) rows);
    }
  }
}
