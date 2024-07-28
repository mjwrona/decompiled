// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.SqlNotificationComponent
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class SqlNotificationComponent : TeamFoundationSqlResourceComponent
  {
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[2]
    {
      (IComponentCreator) new ComponentCreator<SqlNotificationComponent>(1, true),
      (IComponentCreator) new ComponentCreator<SqlNotificationComponent2>(2)
    }, "SqlNotification");
    private static readonly Dictionary<int, SqlExceptionFactory> s_sqlExceptionFactories;
    private static readonly string s_Area = "SqlNotifications";
    private static readonly string s_Layer = nameof (SqlNotificationComponent);

    static SqlNotificationComponent()
    {
      SqlNotificationComponent.s_sqlExceptionFactories = new Dictionary<int, SqlExceptionFactory>();
      SqlNotificationComponent.s_sqlExceptionFactories.Add(800039, new SqlExceptionFactory(typeof (HostProcessNotFoundException)));
    }

    protected override IDictionary<int, SqlExceptionFactory> TranslatedExceptions => (IDictionary<int, SqlExceptionFactory>) SqlNotificationComponent.s_sqlExceptionFactories;

    public List<SqlNotification> QueryNotifications(long lastEventId)
    {
      Stopwatch stopwatch = new Stopwatch();
      this.PrepareStoredProcedure("prc_QueryNotifications");
      this.BindLong("@lastEventId", lastEventId);
      this.MaxDeadlockRetries = 2;
      stopwatch.Start();
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), "prc_QueryNotifications", this.RequestContext);
      stopwatch.Stop();
      if (stopwatch.ElapsedMilliseconds > 30000L)
        TeamFoundationTracingService.TraceRaw(68699, TraceLevel.Error, SqlNotificationComponent.s_Area, SqlNotificationComponent.s_Layer, "prc_QueryNotifications took {0} seconds for ds:{1} db:{2}", (object) (stopwatch.ElapsedMilliseconds / 1000L), (object) this.DataSource, (object) this.Database);
      resultCollection.AddBinder<SqlNotification>((ObjectBinder<SqlNotification>) new SqlNotificationBinder(false));
      resultCollection.AddBinder<SqlNotification>((ObjectBinder<SqlNotification>) new SqlNotificationBinder(true));
      List<SqlNotification> items = resultCollection.GetCurrent<SqlNotification>().Items;
      if (resultCollection.TryNextResult())
        items.AddRange((IEnumerable<SqlNotification>) resultCollection.GetCurrent<SqlNotification>().Items);
      return items;
    }

    public long QueryNotificationWatermark(
      out List<KeyValuePair<Guid, long>> globalWatermarks)
    {
      this.PrepareStoredProcedure("prc_QueryNotificationWatermark");
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), "prc_QueryNotificationWatermark", this.RequestContext);
      resultCollection.AddBinder<long>((ObjectBinder<long>) new SqlNotificationWatermarkBinder());
      resultCollection.AddBinder<KeyValuePair<Guid, long>>((ObjectBinder<KeyValuePair<Guid, long>>) new GlobalSqlNotificationWatermarkBinder());
      long num = resultCollection.GetCurrent<long>().Items.FirstOrDefault<long>();
      if (resultCollection.TryNextResult())
      {
        globalWatermarks = resultCollection.GetCurrent<KeyValuePair<Guid, long>>().Items;
        return num;
      }
      globalWatermarks = new List<KeyValuePair<Guid, long>>();
      return num;
    }

    public virtual long SendNotification(
      Guid eventClass,
      string eventData,
      Guid eventAuthor,
      Guid hostId)
    {
      this.PrepareStoredProcedure("prc_SendNotification");
      this.BindGuid("@eventClass", eventClass);
      this.BindString("@eventData", eventData, int.MaxValue, true, SqlDbType.NVarChar);
      this.BindNullableGuid("@eventAuthor", eventAuthor);
      this.BindGuid("@hostId", hostId);
      this.ExecuteNonQuery();
      return 0;
    }

    public void SendGlobalNotification(Guid eventClass)
    {
      this.PrepareStoredProcedure("prc_SendGlobalNotification");
      this.BindGuid("@eventClass", eventClass);
      this.ExecuteNonQuery();
    }
  }
}
