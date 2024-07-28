// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.Server.WebApi.TokenNotificationEventsController
// Assembly: Microsoft.VisualStudio.Services.Notifications.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8ED1F52D-E567-4EFA-AAED-F2D9262BE9C2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Notifications.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System.Diagnostics;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Notifications.Server.WebApi
{
  [ControllerApiVersion(6.1)]
  [VersionedApiControllerCustomName(Area = "notification", ResourceName = "TokenNotificationEvent")]
  public class TokenNotificationEventsController : NotificationControllerBase
  {
    [HttpPost]
    [ClientInternalUseOnly(true)]
    public VssNotificationEvent PublishTokenEvent(VssNotificationEvent notificationEvent)
    {
      ArgumentUtility.CheckForNull<VssNotificationEvent>(notificationEvent, nameof (notificationEvent));
      ArgumentUtility.CheckForNull<object>(notificationEvent.Data, "notificationEvent.Data");
      ArgumentUtility.CheckForNull<string>(notificationEvent.EventType, "notificationEvent.EventType");
      this.CheckPermission(this.TfsRequestContext);
      this.LoggableDiagnosticParameters[nameof (notificationEvent)] = (object) notificationEvent;
      INotificationEventService service = this.TfsRequestContext.GetService<INotificationEventService>();
      this.TfsRequestContext.Trace(1003100, TraceLevel.Info, NotificationEventService.Area, NotificationEventService.Layer, "Publishing: EventType {0} Data {1}", (object) notificationEvent.EventType, (object) notificationEvent.Data.ToString());
      IVssRequestContext tfsRequestContext = this.TfsRequestContext;
      VssNotificationEvent theEvent = notificationEvent;
      service.PublishSystemEvent(tfsRequestContext, theEvent);
      return notificationEvent;
    }

    private void CheckPermission(IVssRequestContext requestContext) => requestContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(requestContext, EventPublishSecurity.NamespaceId).CheckPermission(requestContext, EventPublishSecurity.PublishEventsResource, 2);
  }
}
