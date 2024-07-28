// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.Server.WebApi.NotificationSubscribersController
// Assembly: Microsoft.VisualStudio.Services.Notifications.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8ED1F52D-E567-4EFA-AAED-F2D9262BE9C2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.Notifications.WebApi;
using System;
using System.Net;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Notifications.Server.WebApi
{
  [ControllerApiVersion(4.0)]
  [VersionedApiControllerCustomName(Area = "notification", ResourceName = "Subscribers")]
  public class NotificationSubscribersController : NotificationControllerBase
  {
    [HttpGet]
    [ClientExample("GET__notification_subscriber.json", null, null, null)]
    public NotificationSubscriber GetSubscriber(Guid subscriberId) => this.TfsRequestContext.GetService<INotificationSubscriberService>().GetSubscriber(this.TfsRequestContext, subscriberId);

    [HttpPatch]
    [ClientExample("PATCH__notification_subscriber.json", null, null, null)]
    public NotificationSubscriber UpdateSubscriber(
      Guid subscriberId,
      NotificationSubscriberUpdateParameters updateParameters)
    {
      this.LoggableDiagnosticParameters[nameof (updateParameters)] = (object) updateParameters;
      return this.TfsRequestContext.GetService<INotificationSubscriberService>().UpdateSubscriber(this.TfsRequestContext, subscriberId, updateParameters);
    }

    protected override void InitializeExceptionMap(ApiExceptionMapping exceptionMap)
    {
      base.InitializeExceptionMap(exceptionMap);
      exceptionMap.AddStatusCode<IdentityNotFoundException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<UnsupportedDeliveryPreference>(HttpStatusCode.BadRequest);
    }
  }
}
