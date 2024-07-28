// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.Server.NotificationSubscriptionQueryController
// Assembly: Microsoft.VisualStudio.Services.Notifications.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8ED1F52D-E567-4EFA-AAED-F2D9262BE9C2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Notifications.WebApi;
using System.Collections.Generic;
using System.Net;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Notifications.Server
{
  [ControllerApiVersion(3.0)]
  [ClientGroupByResource("Subscriptions")]
  public class NotificationSubscriptionQueryController : NotificationControllerBase
  {
    [HttpPost]
    [ClientExample("POST__notification_subscriptionQuery.json", "By subscriber", null, null)]
    public List<NotificationSubscription> QuerySubscriptions(SubscriptionQuery subscriptionQuery)
    {
      this.LoggableDiagnosticParameters[nameof (subscriptionQuery)] = (object) subscriptionQuery;
      return this.TfsRequestContext.GetService<INotificationSubscriptionService>().QuerySubscriptions(this.TfsRequestContext, subscriptionQuery);
    }

    protected override void InitializeExceptionMap(ApiExceptionMapping exceptionMap)
    {
      base.InitializeExceptionMap(exceptionMap);
      exceptionMap.AddStatusCode<InvalidNotificationSubscriptionQueryException>(HttpStatusCode.BadRequest);
    }
  }
}
