// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.AlertPublishingComponent3
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
  internal class AlertPublishingComponent3 : AlertPublishingComponent2
  {
    private static readonly SqlMetaData[] typ_AlertTable3 = new SqlMetaData[8]
    {
      new SqlMetaData("EventId", SqlDbType.Int),
      new SqlMetaData("DeleteEntry", SqlDbType.Bit),
      new SqlMetaData("Version", SqlDbType.Int),
      new SqlMetaData("Enabled", SqlDbType.Bit),
      new SqlMetaData("Area", SqlDbType.NVarChar, 128L),
      new SqlMetaData("Description", SqlDbType.NVarChar, SqlMetaData.Max),
      new SqlMetaData("AreaPath", SqlDbType.NVarChar, SqlMetaData.Max),
      new SqlMetaData("EventName", SqlDbType.NVarChar, 128L)
    };

    public override List<AlertConfiguration> QueryAlerts(
      bool liveEntriesOnly,
      bool enabledAlertsOnly)
    {
      this.PrepareStoredProcedure("prc_QueryAlerts");
      this.BindBoolean("@liveEntriesOnly", liveEntriesOnly);
      this.BindBoolean("@enabledAlertsOnly", enabledAlertsOnly);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), "prc_QueryAlerts", this.RequestContext);
      resultCollection.AddBinder<AlertConfiguration>((ObjectBinder<AlertConfiguration>) new AlertConfigurationBinder2());
      return resultCollection.GetCurrent<AlertConfiguration>().Items;
    }

    public override void UpdateAlerts(
      bool runtimeUpdates,
      string eventSource,
      List<AlertUpdate> updates)
    {
      this.PrepareStoredProcedure("prc_UpdateAlerts2");
      this.BindBoolean("@runtimeUpdates", runtimeUpdates);
      this.BindString("@eventSource", eventSource, 128, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      this.BindAlertTable3("@updates", (IEnumerable<AlertUpdate>) updates);
      this.ExecuteNonQuery();
    }

    private SqlParameter BindAlertTable3(string parameterName, IEnumerable<AlertUpdate> rows)
    {
      rows = rows ?? Enumerable.Empty<AlertUpdate>();
      System.Func<AlertUpdate, SqlDataRecord> selector = (System.Func<AlertUpdate, SqlDataRecord>) (alertUpdate =>
      {
        SqlDataRecord record = new SqlDataRecord(AlertPublishingComponent3.typ_AlertTable3);
        record.SetInt32(0, alertUpdate.EventId);
        record.SetBoolean(1, alertUpdate.DeleteEntry);
        record.SetInt32(2, alertUpdate.Version);
        record.SetBoolean(3, alertUpdate.Enabled);
        record.SetString(4, alertUpdate.Area);
        record.SetNullableString(5, alertUpdate.Description);
        record.SetNullableString(6, alertUpdate.AreaPath);
        record.SetNullableString(7, alertUpdate.EventName);
        return record;
      });
      return this.BindTable(parameterName, "typ_AlertTable3", rows.Select<AlertUpdate, SqlDataRecord>(selector));
    }
  }
}
