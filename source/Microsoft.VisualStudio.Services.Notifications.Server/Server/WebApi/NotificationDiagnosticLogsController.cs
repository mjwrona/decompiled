// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.Server.WebApi.NotificationDiagnosticLogsController
// Assembly: Microsoft.VisualStudio.Services.Notifications.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8ED1F52D-E567-4EFA-AAED-F2D9262BE9C2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Notifications.WebApi;
using System;
using System.Collections.Generic;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Notifications.Server.WebApi
{
  [ControllerApiVersion(4.0)]
  [VersionedApiControllerCustomName(Area = "notification", ResourceName = "DiagnosticLogs")]
  public class NotificationDiagnosticLogsController : NotificationControllerBase
  {
    [HttpGet]
    [ClientExample("GET__notification_diagnosticLogs.json", null, null, null)]
    public IEnumerable<INotificationDiagnosticLog> ListLogs(
      Guid source,
      Guid? entryId = null,
      DateTime? startTime = null,
      DateTime? endTime = null)
    {
      INotificationDiagnosticLogService service = this.TfsRequestContext.GetService<INotificationDiagnosticLogService>();
      NotificationDiagnosticsQueryType diagnosticsQueryType = entryId.HasValue ? NotificationDiagnosticsQueryType.LookupById : NotificationDiagnosticsQueryType.LookupByStartEndTime;
      NotificationDiagnosticsQuery diagnosticsQuery = new NotificationDiagnosticsQuery()
      {
        Type = diagnosticsQueryType,
        Source = source,
        Id = entryId,
        StartTime = startTime,
        EndTime = endTime
      };
      IVssRequestContext tfsRequestContext = this.TfsRequestContext;
      NotificationDiagnosticsQuery query = diagnosticsQuery;
      return (IEnumerable<INotificationDiagnosticLog>) service.QueryNotificationLog(tfsRequestContext, query);
    }
  }
}
