// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.Server.NotificationDiagnosticLogService
// Assembly: Microsoft.VisualStudio.Services.Notifications.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8ED1F52D-E567-4EFA-AAED-F2D9262BE9C2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Notifications.WebApi;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Notifications.Server
{
  public sealed class NotificationDiagnosticLogService : 
    INotificationDiagnosticLogService,
    IVssFrameworkService
  {
    void IVssFrameworkService.ServiceStart(IVssRequestContext requestContext)
    {
    }

    void IVssFrameworkService.ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public List<INotificationDiagnosticLog> QueryNotificationLog(
      IVssRequestContext requestContext,
      NotificationDiagnosticsQuery query)
    {
      switch (query.Type)
      {
        case NotificationDiagnosticsQueryType.LookupById:
          ArgumentUtility.CheckForNull<Guid>(query.Id, "id");
          break;
        case NotificationDiagnosticsQueryType.LookupByStartEndTime:
          ArgumentUtility.CheckForNull<DateTime>(query.StartTime, "timeMin");
          ArgumentUtility.CheckForNull<DateTime>(query.EndTime, "timeMax");
          break;
        default:
          throw new ArgumentException("type");
      }
      using (IEventNotificationComponent component = requestContext.CreateComponent<IEventNotificationComponent, EventNotificationComponent>())
        return component.QueryNotificationLog(query);
    }
  }
}
