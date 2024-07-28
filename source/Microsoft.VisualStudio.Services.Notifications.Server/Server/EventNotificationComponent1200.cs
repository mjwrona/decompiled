// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.Server.EventNotificationComponent1200
// Assembly: Microsoft.VisualStudio.Services.Notifications.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8ED1F52D-E567-4EFA-AAED-F2D9262BE9C2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.Server.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Notifications.WebApi;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Notifications.Server
{
  internal class EventNotificationComponent1200 : EventNotificationComponent1160
  {
    public override void UpdateNotificationStatistics(IEnumerable<NotificationStatisticEntry> stats)
    {
      this.PrepareStoredProcedure("prc_UpdateNotificationStatistics");
      this.BindNotificationStatisticsTable("@stats", stats);
      this.ExecuteNonQuery();
    }

    protected virtual SqlParameter BindNotificationStatisticsTable(
      string parameterName,
      IEnumerable<NotificationStatisticEntry> rows)
    {
      rows = rows ?? Enumerable.Empty<NotificationStatisticEntry>();
      return this.BindTable(parameterName, this.NotificationStatisticsTableName, rows.Select<NotificationStatisticEntry, SqlDataRecord>(new System.Func<NotificationStatisticEntry, SqlDataRecord>(this.NotificationStatisticRowBinder)));
    }

    private SqlDataRecord NotificationStatisticRowBinder(NotificationStatisticEntry stat)
    {
      SqlDataRecord record = new SqlDataRecord(NotificationEventTypes.typ_NotificationStatistics);
      record.SetDateTime(0, stat.Date);
      record.SetInt32(1, (int) stat.Type);
      record.SetNullableString(2, stat.Path);
      record.SetNullableGuid(3, stat.UserID);
      record.SetInt32(4, stat.HitCount);
      return record;
    }

    internal override EventBacklogStatusBinder GetEventBacklogStatusVersion() => (EventBacklogStatusBinder) new EventBacklogStatusBinder1200();

    internal override NotificationBacklogStatusBinder GetNotificationBacklogStatusVersion() => (NotificationBacklogStatusBinder) new NotificationBacklogStatusBinder1200();

    public override List<NotificationStatistic> QueryNotificationStatistics(
      IEnumerable<NotificationStatisticsQueryConditions> queries)
    {
      this.PrepareStoredProcedure("prc_QueryNotificationStatistics");
      this.BindNotificationStatisticsQuery("@query", queries);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<NotificationStatistic>((ObjectBinder<NotificationStatistic>) new NotificationStatisticBinder());
      return resultCollection.GetCurrent<NotificationStatistic>().Items;
    }

    protected virtual void BindNotificationStatisticsQuery(
      string parameterName,
      IEnumerable<NotificationStatisticsQueryConditions> queries)
    {
      List<SqlDataRecord> rows = new List<SqlDataRecord>();
      foreach (NotificationStatisticsQueryConditions query in queries)
      {
        SqlDataRecord record = new SqlDataRecord(NotificationEventTypes.typ_NotificationStatisticsQuery2);
        record.SetNullableDateTime(0, query.StartDate);
        NotificationStatisticType? type = query.Type;
        if (type.HasValue)
        {
          SqlDataRecord sqlDataRecord = record;
          type = query.Type;
          int num = (int) type.Value;
          sqlDataRecord.SetInt32(1, num);
        }
        else
          record.SetDBNull(1);
        record.SetNullableString(2, query.Path);
        if (!string.IsNullOrEmpty(query.User?.Id))
          record.SetNullableGuid(3, Guid.Parse(query.User.Id));
        else
          record.SetDBNull(3);
        record.SetNullableInt32(4, query.HitCountMinimum);
        record.SetNullableDateTime(5, query.EndDate);
        rows.Add(record);
      }
      this.BindTable(parameterName, "typ_NotificationStatisticsQuery2", (IEnumerable<SqlDataRecord>) rows);
    }
  }
}
