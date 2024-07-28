// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.AlertPublishingComponent
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.Framework.Server;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Cloud
{
  internal class AlertPublishingComponent : TeamFoundationSqlResourceComponent
  {
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[4]
    {
      (IComponentCreator) new ComponentCreator<AlertPublishingComponent>(1),
      (IComponentCreator) new ComponentCreator<AlertPublishingComponent2>(2),
      (IComponentCreator) new ComponentCreator<AlertPublishingComponent3>(3),
      (IComponentCreator) new ComponentCreator<AlertPublishingComponent4>(4)
    }, "Alert");
    private static readonly SqlMetaData[] typ_AlertTable = new SqlMetaData[6]
    {
      new SqlMetaData("EventId", SqlDbType.Int),
      new SqlMetaData("DeleteEntry", SqlDbType.Bit),
      new SqlMetaData("Version", SqlDbType.Int),
      new SqlMetaData("Enabled", SqlDbType.Bit),
      new SqlMetaData("Area", SqlDbType.NVarChar, 128L),
      new SqlMetaData("Description", SqlDbType.NVarChar, SqlMetaData.Max)
    };
    private static readonly Dictionary<int, SqlExceptionFactory> s_sqlExceptionFactories = new Dictionary<int, SqlExceptionFactory>();

    public virtual List<AlertConfiguration> QueryAlerts(
      bool liveEntriesOnly,
      bool enabledAlertsOnly)
    {
      this.PrepareStoredProcedure("prc_QueryAlerts");
      this.BindBoolean("@liveEntriesOnly", liveEntriesOnly);
      this.BindBoolean("@enabledAlertsOnly", enabledAlertsOnly);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), "prc_QueryAlerts", this.RequestContext);
      resultCollection.AddBinder<AlertConfiguration>((ObjectBinder<AlertConfiguration>) new AlertConfigurationBinder());
      return resultCollection.GetCurrent<AlertConfiguration>().Items;
    }

    public virtual void UpdateAlerts(
      bool runtimeUpdates,
      string eventSource,
      List<AlertUpdate> updates)
    {
      this.PrepareStoredProcedure("prc_UpdateAlerts");
      this.BindBoolean("@runtimeUpdates", runtimeUpdates);
      this.BindString("@eventSource", eventSource, 128, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      this.BindAlertTable("@updates", (IEnumerable<AlertUpdate>) updates);
      this.ExecuteNonQuery();
    }

    private SqlParameter BindAlertTable(string parameterName, IEnumerable<AlertUpdate> rows)
    {
      rows = rows ?? Enumerable.Empty<AlertUpdate>();
      System.Func<AlertUpdate, SqlDataRecord> selector = (System.Func<AlertUpdate, SqlDataRecord>) (alertUpdate =>
      {
        SqlDataRecord record = new SqlDataRecord(AlertPublishingComponent.typ_AlertTable);
        record.SetInt32(0, alertUpdate.EventId);
        record.SetBoolean(1, alertUpdate.DeleteEntry);
        record.SetInt32(2, alertUpdate.Version);
        record.SetBoolean(3, alertUpdate.Enabled);
        record.SetString(4, alertUpdate.Area);
        record.SetNullableString(5, alertUpdate.Description);
        return record;
      });
      return this.BindTable(parameterName, "typ_AlertTable", rows.Select<AlertUpdate, SqlDataRecord>(selector));
    }
  }
}
