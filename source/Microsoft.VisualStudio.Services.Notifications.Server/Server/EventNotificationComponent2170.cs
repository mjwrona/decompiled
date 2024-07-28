// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.Server.EventNotificationComponent2170
// Assembly: Microsoft.VisualStudio.Services.Notifications.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8ED1F52D-E567-4EFA-AAED-F2D9262BE9C2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Data;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Notifications.Server
{
  internal class EventNotificationComponent2170 : EventNotificationComponent1440
  {
    public override (int, int) CleanupNotificationsEvents(
      int eventAgeMins,
      int notificationAgeMins,
      int batchSize)
    {
      this.PrepareStoredProcedure("prc_CleanupEventsNotifications", 3600);
      this.BindInt("@batchSize", batchSize);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<(int, int)>((ObjectBinder<(int, int)>) new EventNotificationComponent2170.CleanupEventsNotificationsResultsBinder());
        (int, int) tuple = resultCollection.GetCurrent<(int, int)>().First<(int, int)>();
        return (tuple.Item1, tuple.Item2);
      }
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
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<int>((ObjectBinder<int>) new SimpleObjectBinder<int>((System.Func<IDataReader, int>) (reader => new SqlColumnBinder("CleanedStatisticsCount").GetInt32(reader))));
        return resultCollection.GetCurrent<int>().First<int>();
      }
    }

    public override int CleanupNotificationLog(int logAgeMins, int batchSize)
    {
      this.PrepareStoredProcedure("prc_CleanupNotificationLog", 14400);
      this.BindInt("@logAgeMins", logAgeMins);
      this.BindInt("@batchSize", batchSize);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<int>((ObjectBinder<int>) new SimpleObjectBinder<int>((System.Func<IDataReader, int>) (reader => new SqlColumnBinder("CleanedNotificationLogsCount").GetInt32(reader))));
        return resultCollection.GetCurrent<int>().First<int>();
      }
    }

    protected class CleanupEventsNotificationsResultsBinder : ObjectBinder<(int, int)>
    {
      private SqlColumnBinder NotificationsCleanedCountBinder = new SqlColumnBinder("CleanedNotificationsCount");
      private SqlColumnBinder EventsCleanedCountBinder = new SqlColumnBinder("CleanedEventsCount");

      protected override (int, int) Bind() => (this.NotificationsCleanedCountBinder.GetInt32((IDataReader) this.Reader), this.EventsCleanedCountBinder.GetInt32((IDataReader) this.Reader));
    }
  }
}
