// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.Server.WebApi.NotificationEventsController
// Assembly: Microsoft.VisualStudio.Services.Notifications.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8ED1F52D-E567-4EFA-AAED-F2D9262BE9C2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Notifications.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Notifications.Server.WebApi
{
  public class NotificationEventsController : NotificationControllerBase
  {
    [HttpPost]
    [FeatureEnabled("Notifications.ThirdPartyEventPublishing")]
    [ClientInternalUseOnly(true)]
    public VssNotificationEvent PublishEvent(VssNotificationEvent notificationEvent)
    {
      ArgumentUtility.CheckForNull<VssNotificationEvent>(notificationEvent, nameof (notificationEvent));
      ArgumentUtility.CheckForNull<object>(notificationEvent.Data, "notificationEvent.Data");
      ArgumentUtility.CheckForNull<string>(notificationEvent.EventType, "notificationEvent.EventType");
      this.LoggableDiagnosticParameters[nameof (notificationEvent)] = (object) notificationEvent;
      INotificationEventService service = this.TfsRequestContext.GetService<INotificationEventService>();
      NotificationEventType eventType = service.GetEventType(this.TfsRequestContext, notificationEvent.EventType);
      if (eventType != null)
      {
        IReadOnlyList<INotificationEventPublishValidator> extensions = (IReadOnlyList<INotificationEventPublishValidator>) this.TfsRequestContext.GetExtensions<INotificationEventPublishValidator>(ExtensionLifetime.Service);
        if (extensions.Count > 0)
        {
          foreach (INotificationEventPublishValidator publishValidator in (IEnumerable<INotificationEventPublishValidator>) extensions)
          {
            if (publishValidator.ValidateEventPublish(this.TfsRequestContext, notificationEvent, eventType))
            {
              this.TfsRequestContext.Trace(1002213, TraceLevel.Info, NotificationEventService.Area, NotificationEventService.Layer, "Publishing: EventType {0} Data {1}", (object) notificationEvent.EventType, (object) this.GetSafeLoggablePublishData(notificationEvent.Data));
              service.PublishSystemEvent(this.TfsRequestContext, notificationEvent);
              return notificationEvent;
            }
          }
          this.TfsRequestContext.Trace(1002213, TraceLevel.Warning, NotificationEventService.Area, NotificationEventService.Layer, "Could not validate: EventType {0} Data {1}", (object) notificationEvent.EventType, (object) this.GetSafeLoggablePublishData(notificationEvent.Data));
          throw new UnauthorizedAccessException(CoreRes.UnauthorizedPublishEvent());
        }
      }
      else
        this.TfsRequestContext.Trace(1002213, TraceLevel.Warning, NotificationEventService.Area, NotificationEventService.Layer, "Could not find NotificationEventType for {0}", (object) notificationEvent.EventType);
      throw new NotImplementedException();
    }

    private string GetSafeLoggablePublishData(object o) => !(o is string str) ? "[object]" : str.Substring(0, 1024);
  }
}
