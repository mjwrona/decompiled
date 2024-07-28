// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.Server.NotificationEventTypeFieldValuesQueryController
// Assembly: Microsoft.VisualStudio.Services.Notifications.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8ED1F52D-E567-4EFA-AAED-F2D9262BE9C2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Notifications.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Notifications.Server
{
  [ControllerApiVersion(3.0)]
  [VersionedApiControllerCustomName(Area = "notification", ResourceName = "EventTypeFieldValuesQuery")]
  [ClientInternalUseOnly(false)]
  public class NotificationEventTypeFieldValuesQueryController : NotificationControllerBase
  {
    [HttpPost]
    [ClientLocationId("b5bbdd21-c178-4398-b6db-0166d910028a")]
    public IEnumerable<NotificationEventField> QueryEventTypes(
      string eventType,
      [FromBody] FieldValuesQuery inputValuesQuery)
    {
      ArgumentUtility.CheckForNull<FieldValuesQuery>(inputValuesQuery, nameof (inputValuesQuery));
      ArgumentUtility.CheckStringForNullOrEmpty(eventType, nameof (eventType));
      this.LoggableDiagnosticParameters[nameof (inputValuesQuery)] = (object) inputValuesQuery;
      return (IEnumerable<NotificationEventField>) this.TfsRequestContext.GetService<INotificationEventService>().GetInputValues(this.TfsRequestContext, eventType, inputValuesQuery).OrderBy<NotificationEventField, string>((Func<NotificationEventField, string>) (f => f.Name), (IComparer<string>) StringComparer.CurrentCultureIgnoreCase).ToList<NotificationEventField>();
    }

    protected override void InitializeExceptionMap(ApiExceptionMapping exceptionMap)
    {
      base.InitializeExceptionMap(exceptionMap);
      exceptionMap.AddStatusCode<ArgumentException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<EventTypeNotSupportedException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<NoSubscriptionAdaterFoundException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<InvalidScopeException>(HttpStatusCode.BadRequest);
    }
  }
}
