// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.Server.WebApi.NotificationBatchNotificationOperationsController
// Assembly: Microsoft.VisualStudio.Services.Notifications.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8ED1F52D-E567-4EFA-AAED-F2D9262BE9C2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Notifications.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Notifications.Server.WebApi
{
  [ControllerApiVersion(3.1)]
  [ClientInternalUseOnly(true)]
  public class NotificationBatchNotificationOperationsController : NotificationControllerBase
  {
    [HttpPost]
    [ClientResponseType(typeof (void), null, null)]
    [MethodInformation(IsLongRunning = true)]
    public HttpResponseMessage PerformBatchNotificationOperations(
      [FromBody] BatchNotificationOperation operation)
    {
      this.LoggableDiagnosticParameters[nameof (operation)] = (object) operation;
      IEventNotificationServiceInternal service = this.TfsRequestContext.GetService<IEventNotificationServiceInternal>();
      switch (operation.NotificationOperation)
      {
        case NotificationOperation.SuspendUnprocessed:
          service.SuspendUnprocessedNotifications(this.TfsRequestContext, operation.NotificationQueryConditions, true);
          break;
      }
      return this.Request.CreateResponse(HttpStatusCode.OK);
    }
  }
}
