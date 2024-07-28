// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.Server.EventNotificationComponent1340
// Assembly: Microsoft.VisualStudio.Services.Notifications.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8ED1F52D-E567-4EFA-AAED-F2D9262BE9C2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.Server.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Notifications.WebApi;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Notifications.Server
{
  internal class EventNotificationComponent1340 : EventNotificationComponent1330
  {
    protected override string SubscriptionKeyTypeName => "typ_SubscriptionKey5";

    protected override SqlMetaData[] SubscriptionKeyType => NotificationEventTypes.typ_SubscriptionKey5;

    protected virtual SubscriptionLookupType MaxSubscriptionLookupType => SubscriptionLookupType.ForTarget;

    protected override SqlDataRecord BindSubscriptionKeyRecord(SubscriptionLookup key)
    {
      SqlDataRecord record1 = new SqlDataRecord(this.SubscriptionKeyType);
      SubscriptionLookupType subscriptionLookupType = key.LookupType <= this.MaxSubscriptionLookupType ? key.LookupType : SubscriptionLookupType.Any;
      record1.SetInt32(0, (int) subscriptionLookupType);
      record1.SetNullableInt32(1, key.SubscriptionId);
      SqlDataRecord record2 = record1;
      Guid? nullable1 = key.SubscriberId;
      Guid guid1 = nullable1 ?? Guid.Empty;
      record2.SetNullableGuid(2, guid1);
      record1.SetNullableString(3, key.Matcher);
      record1.SetNullableString(4, key.EventType);
      record1.SetNullableString(5, key.IndexedExpression);
      SqlDataRecord record3 = record1;
      nullable1 = key.DataspaceId;
      int? nullable2;
      if (!nullable1.HasValue)
      {
        nullable2 = new int?();
      }
      else
      {
        nullable1 = key.DataspaceId;
        nullable2 = new int?(this.GetDataspaceId(nullable1.Value));
      }
      record3.SetNullableInt32(6, nullable2);
      record1.SetNullableString(7, string.IsNullOrEmpty(key.Classification) ? (string) null : key.Classification);
      record1.SetNullableString(8, key.Channel);
      record1.SetNullableString(9, key.Metadata);
      SubscriptionFlags? flags = key.Flags;
      if (flags.HasValue)
      {
        SqlDataRecord sqlDataRecord = record1;
        flags = key.Flags;
        int num = (int) flags.Value;
        sqlDataRecord.SetInt32(10, num);
      }
      else
        record1.SetDBNull(10);
      SqlDataRecord record4 = record1;
      nullable1 = key.ScopeId;
      Guid guid2 = nullable1 ?? Guid.Empty;
      record4.SetNullableGuid(11, guid2);
      SqlDataRecord record5 = record1;
      nullable1 = key.UniqueId;
      Guid guid3 = nullable1 ?? Guid.Empty;
      record5.SetNullableGuid(12, guid3);
      return record1;
    }

    public override void BindGetSubscriptionParameters(
      IEnumerable<string> eventTypes,
      IEnumerable<string> matchers)
    {
      this.BindGetSubscriptionsTable(eventTypes, matchers);
    }

    protected virtual void BindGetSubscriptionsTable(
      IEnumerable<string> eventTypes,
      IEnumerable<string> matchers)
    {
      List<SqlDataRecord> rows = new List<SqlDataRecord>();
      foreach (string eventType in eventTypes)
      {
        if (eventType != null)
        {
          foreach (string matcher in matchers)
          {
            SqlDataRecord sqlDataRecord = new SqlDataRecord(NotificationEventTypes.typ_GetSubscriptionTable);
            sqlDataRecord.SetString(0, eventType);
            sqlDataRecord.SetString(1, matcher);
            rows.Add(sqlDataRecord);
          }
        }
      }
      this.BindTable("@getSubscriptions", "typ_GetSubscriptionTable", (IEnumerable<SqlDataRecord>) rows);
    }
  }
}
