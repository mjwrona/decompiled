// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.PersistedNotification.Server.NotificationsController
// Assembly: Microsoft.TeamFoundation.PersistedNotification.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 93EF6375-7D4B-4818-984E-834B7F34DA0F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.PersistedNotification.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.TeamFoundation.PersistedNotification.Server
{
  [ControllerApiVersion(1.0)]
  [VersionedApiControllerCustomName(Area = "PersistedNotification", ResourceName = "Notifications", ResourceVersion = 1)]
  public class NotificationsController : NotificationApiController
  {
    [HttpGet]
    public HttpResponseMessage GetNotifications() => this.Request.CreateResponse<IEnumerable<Microsoft.VisualStudio.Services.Notification.Notification>>(HttpStatusCode.OK, (IEnumerable<Microsoft.VisualStudio.Services.Notification.Notification>) this.PersistedNotificationService.GetRecipientNotifications(this.TfsRequestContext));

    [HttpPost]
    public HttpResponseMessage SaveNotifications(
      VssJsonCollectionWrapper<IEnumerable<Microsoft.VisualStudio.Services.Notification.Notification>> notifications)
    {
      this.PersistedNotificationService.SaveNotifications(this.TfsRequestContext, notifications.Value);
      return new HttpResponseMessage(HttpStatusCode.Created);
    }
  }
}
