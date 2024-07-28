// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.Server.WebApi.NotificationEventTypesController
// Assembly: Microsoft.VisualStudio.Services.Notifications.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8ED1F52D-E567-4EFA-AAED-F2D9262BE9C2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Notifications.WebApi;
using System.Collections.Generic;
using System.Net;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Notifications.Server.WebApi
{
  public class NotificationEventTypesController : NotificationControllerBase
  {
    [HttpGet]
    [ClientExample("GET__notification_eventTypes.json", "All", null, null)]
    [ClientExample("GET__notification_eventTypes_publisherId-_publisherId_.json", "By publisher", null, null)]
    public IEnumerable<NotificationEventType> ListEventTypes(string publisherId = null)
    {
      INotificationEventService service = this.TfsRequestContext.GetService<INotificationEventService>();
      return !string.IsNullOrEmpty(publisherId) ? service.GetPublisherEventTypes(this.TfsRequestContext, publisherId) : (IEnumerable<NotificationEventType>) service.GetEventTypes(this.TfsRequestContext);
    }

    [HttpGet]
    [ClientExample("GET__notification_eventTypes__eventTypeId_.json", null, null, null)]
    public NotificationEventType GetEventType(string eventType) => this.TfsRequestContext.GetService<INotificationEventService>().GetEventType(this.TfsRequestContext, eventType) ?? throw new EventTypeDoesNotExistException(eventType);

    protected override void InitializeExceptionMap(ApiExceptionMapping exceptionMap)
    {
      base.InitializeExceptionMap(exceptionMap);
      exceptionMap.AddStatusCode<EventTypeDoesNotExistException>(HttpStatusCode.NotFound);
    }
  }
}
