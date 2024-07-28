// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.Server.SubscriptionDiagnosticsNotificationTracingController
// Assembly: Microsoft.VisualStudio.Services.Notifications.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8ED1F52D-E567-4EFA-AAED-F2D9262BE9C2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Notifications.WebApi;
using System;
using System.Net;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Notifications.Server
{
  [ControllerApiVersion(4.1)]
  [VersionedApiControllerCustomName(Area = "notification", ResourceName = "Diagnostics")]
  public class SubscriptionDiagnosticsNotificationTracingController : NotificationControllerBase
  {
    [HttpGet]
    [ClientExample("GET__notification_diagnostics.json", null, null, null)]
    public SubscriptionDiagnostics GetSubscriptionDiagnostics(string subscriptionId)
    {
      Subscription subscription = this.TfsRequestContext.GetService<INotificationSubscriptionService>().GetSubscription(this.TfsRequestContext, subscriptionId);
      SubscriptionDiagnostics diagnostics = subscription.Diagnostics;
      if (diagnostics != null && diagnostics.DeliveryResults == null && subscription.IsServiceHooksDelivery)
        diagnostics.DeliveryResults = new SubscriptionTracing()
        {
          Enabled = true,
          StartDate = DateTime.MinValue,
          EndDate = DateTime.MaxValue,
          MaxTracedEntries = int.MaxValue,
          TracedEntries = 0
        };
      return diagnostics;
    }

    [HttpPut]
    [ClientExample("PUT__notification_diagnostics.json", null, null, null)]
    public SubscriptionDiagnostics UpdateSubscriptionDiagnostics(
      string subscriptionId,
      UpdateSubscripitonDiagnosticsParameters updateParameters)
    {
      this.LoggableDiagnosticParameters[nameof (updateParameters)] = (object) updateParameters;
      return this.TfsRequestContext.GetService<INotificationSubscriptionService>().UpdateSubscriptionDiagnostics(this.TfsRequestContext, subscriptionId, updateParameters);
    }

    protected override void InitializeExceptionMap(ApiExceptionMapping exceptionMap)
    {
      base.InitializeExceptionMap(exceptionMap);
      exceptionMap.AddStatusCode<ArgumentException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<SubscriptionNotFoundException>(HttpStatusCode.NotFound);
    }
  }
}
