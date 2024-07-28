// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.Server.EventNotificationComponent1260
// Assembly: Microsoft.VisualStudio.Services.Notifications.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8ED1F52D-E567-4EFA-AAED-F2D9262BE9C2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.Server.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Notifications.Common;
using System;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.VisualStudio.Services.Notifications.Server
{
  internal class EventNotificationComponent1260 : EventNotificationComponent1220
  {
    internal virtual NotificationQueryBinder GetNotificationQueryVersion() => new NotificationQueryBinder();

    public override List<TeamFoundationNotification> QueryNotifications(
      IEnumerable<NotificationLookup> notificationKeys)
    {
      this.PrepareStoredProcedure("prc_QueryEventNotifications");
      this.BindNotificationKeys("@notificationKeys", notificationKeys);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<TeamFoundationNotification>((ObjectBinder<TeamFoundationNotification>) this.GetNotificationQueryVersion());
        return resultCollection.GetCurrent<TeamFoundationNotification>().Items;
      }
    }

    protected override void BindNotificationStatus(
      string parameterName,
      IEnumerable<TeamFoundationNotification> notifications)
    {
      List<SqlDataRecord> rows = new List<SqlDataRecord>();
      foreach (TeamFoundationNotification notification in notifications)
        rows.Add(this.BindNotificationStatusRow(notification));
      this.BindTable(parameterName, this.NotificationStatusTypeName, (IEnumerable<SqlDataRecord>) rows);
    }

    protected virtual SqlDataRecord BindNotificationStatusRow(
      TeamFoundationNotification notification)
    {
      SqlDataRecord record = new SqlDataRecord(this.NotificationStatusType);
      record.SetInt32(0, notification.Id);
      record.SetString(1, notification.SendNotificationState.GetDbStateName());
      record.SetNullableString(2, notification.Result);
      record.SetNullableString(3, notification.ResultDetail);
      return record;
    }

    protected virtual string NotificationStatusTypeName => "typ_NotificationStatusTable2";

    protected virtual SqlMetaData[] NotificationStatusType => NotificationEventTypes.typ_NotificationStatusTable2;

    private void BindNotificationKeys(
      string parameterName,
      IEnumerable<NotificationLookup> notificationKeys)
    {
      List<SqlDataRecord> rows = new List<SqlDataRecord>();
      int num = 0;
      foreach (NotificationLookup notificationKey in notificationKeys)
      {
        SqlDataRecord record = new SqlDataRecord(NotificationEventTypes.typ_NotificationKey);
        record.SetInt32(0, num++);
        record.SetInt32(1, (int) notificationKey.QueryType);
        record.SetNullableInt32(2, notificationKey.NotificationId);
        record.SetNullableGuid(3, notificationKey.SubscriptionUniqueId);
        record.SetInt32(4, notificationKey.IncludeResultDetail ? 1 : 0);
        rows.Add(record);
      }
      this.BindTable(parameterName, "typ_NotificationKey", (IEnumerable<SqlDataRecord>) rows);
    }

    protected override string SubcriptionTypeName => "typ_Subscription3";

    protected override SqlMetaData[] SubscriptionType => NotificationEventTypes.typ_Subscription3;

    protected override SqlDataRecord BindSubscriptionRecord(Subscription subscription)
    {
      SqlDataRecord record = base.BindSubscriptionRecord(subscription);
      record.SetNullableGuid(16, subscription.UniqueId);
      return record;
    }

    protected override string SubscriptionKeyTypeName => "typ_SubscriptionKey4";

    protected override SqlMetaData[] SubscriptionKeyType => NotificationEventTypes.typ_SubscriptionKey4;

    protected override SqlDataRecord BindSubscriptionKeyRecord(SubscriptionLookup key)
    {
      SqlDataRecord record = base.BindSubscriptionKeyRecord(key);
      record.SetNullableGuid(12, key.UniqueId ?? Guid.Empty);
      return record;
    }
  }
}
