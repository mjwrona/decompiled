// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.Server.EventNotificationComponent1310
// Assembly: Microsoft.VisualStudio.Services.Notifications.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8ED1F52D-E567-4EFA-AAED-F2D9262BE9C2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Notifications.WebApi;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.VisualStudio.Services.Notifications.Server
{
  internal class EventNotificationComponent1310 : EventNotificationComponent1290
  {
    public override void SaveNotificationLog(INotificationDiagnosticLog log)
    {
      this.PrepareStoredProcedure("prc_SaveNotificationLog");
      string str = JsonConvert.SerializeObject((object) log, NotificationsSerialization.JsonSerializerSettings);
      this.BindGuid("@logSource", log.Source);
      this.BindGuid("@logId", log.Id);
      this.BindDateTime("@startTime", log.StartTime);
      this.BindDateTime("@endTime", log.EndTime);
      this.BindString("@content", str ?? string.Empty, -1, false, SqlDbType.NVarChar);
      this.ExecuteNonQuery();
    }

    public override List<INotificationDiagnosticLog> QueryNotificationLog(
      NotificationDiagnosticsQuery query)
    {
      this.PrepareStoredProcedure("prc_QueryNotificationLog");
      this.BindInt("@queryType", (int) query.Type);
      this.BindGuid("@logSource", query.Source);
      this.BindNullableGuid("@logId", query.Id);
      this.BindNullableDateTime("@timeMin", query.StartTime);
      this.BindNullableDateTime("@timeMax", query.EndTime);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<INotificationDiagnosticLog>((ObjectBinder<INotificationDiagnosticLog>) this.GetNotificationDiagnosticLogVersion());
        return resultCollection.GetCurrent<INotificationDiagnosticLog>().Items;
      }
    }

    public override int CleanupNotificationLog(int logAgeMins, int batchSize)
    {
      this.PrepareStoredProcedure("prc_CleanupNotificationLog", 14400);
      this.BindInt("@logAgeMins", logAgeMins);
      this.BindInt("@batchSize", batchSize);
      this.ExecuteNonQuery();
      return -1;
    }

    internal virtual NotificationDiagnosticLogBinder GetNotificationDiagnosticLogVersion() => new NotificationDiagnosticLogBinder();
  }
}
