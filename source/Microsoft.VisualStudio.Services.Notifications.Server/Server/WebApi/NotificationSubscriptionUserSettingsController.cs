// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.Server.WebApi.NotificationSubscriptionUserSettingsController
// Assembly: Microsoft.VisualStudio.Services.Notifications.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8ED1F52D-E567-4EFA-AAED-F2D9262BE9C2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Notifications.WebApi;
using System;
using System.Net;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Notifications.Server.WebApi
{
  [ControllerApiVersion(3.2)]
  [ClientGroupByResource("Subscriptions")]
  [VersionedApiControllerCustomName(Area = "notification", ResourceName = "UserSettings")]
  public class NotificationSubscriptionUserSettingsController : NotificationControllerBase
  {
    [HttpPut]
    [ClientExample("PUT__notification_subscriptions__sharedSubscriptionId__userSettings_me.json", "Opt out", null, null)]
    public SubscriptionUserSettings UpdateSubscriptionUserSettings(
      string subscriptionId,
      [FromBody] SubscriptionUserSettings userSettings,
      Guid userId)
    {
      this.LoggableDiagnosticParameters[nameof (userSettings)] = (object) userSettings;
      return this.TfsRequestContext.GetService<INotificationSubscriptionService>().UpdateSubscriptionUserSettings(this.TfsRequestContext, subscriptionId, userId, userSettings);
    }

    protected override void InitializeExceptionMap(ApiExceptionMapping exceptionMap)
    {
      base.InitializeExceptionMap(exceptionMap);
      exceptionMap.AddStatusCode<ArgumentException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<SubscriptionNotFoundException>(HttpStatusCode.NotFound);
    }
  }
}
