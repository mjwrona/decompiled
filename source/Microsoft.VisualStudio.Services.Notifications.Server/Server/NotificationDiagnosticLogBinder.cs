// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.Server.NotificationDiagnosticLogBinder
// Assembly: Microsoft.VisualStudio.Services.Notifications.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8ED1F52D-E567-4EFA-AAED-F2D9262BE9C2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Notifications.WebApi;
using Newtonsoft.Json;
using System;
using System.Data;

namespace Microsoft.VisualStudio.Services.Notifications.Server
{
  internal class NotificationDiagnosticLogBinder : ObjectBinder<INotificationDiagnosticLog>
  {
    private SqlColumnBinder ContentColumn = new SqlColumnBinder("Content");

    protected override INotificationDiagnosticLog Bind()
    {
      string str = this.ContentColumn.GetString((IDataReader) this.Reader, true) ?? string.Empty;
      INotificationDiagnosticLog notificationDiagnosticLog;
      if (!string.IsNullOrEmpty(str))
      {
        try
        {
          notificationDiagnosticLog = JsonConvert.DeserializeObject<INotificationDiagnosticLog>(str, NotificationsSerialization.NotificationDiagnosticLogJsonSerializerSettings);
        }
        catch (Exception ex)
        {
          InvalidDiagnosticLog invalidDiagnosticLog = new InvalidDiagnosticLog();
          invalidDiagnosticLog.Description = ex.ToString();
          notificationDiagnosticLog = (INotificationDiagnosticLog) invalidDiagnosticLog;
        }
      }
      else
      {
        InvalidDiagnosticLog invalidDiagnosticLog = new InvalidDiagnosticLog();
        invalidDiagnosticLog.Description = "Empty";
        notificationDiagnosticLog = (INotificationDiagnosticLog) invalidDiagnosticLog;
      }
      return notificationDiagnosticLog;
    }
  }
}
