// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.Server.NotificationSubscriptionsController
// Assembly: Microsoft.VisualStudio.Services.Notifications.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8ED1F52D-E567-4EFA-AAED-F2D9262BE9C2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.Notifications.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Notifications.Server
{
  [ControllerApiVersion(3.0)]
  public class NotificationSubscriptionsController : NotificationControllerBase
  {
    [HttpGet]
    [ClientExample("GET__notification_subscriptions__subscriptionId_.json", null, null, null)]
    public NotificationSubscription GetSubscription(
      string subscriptionId,
      SubscriptionQueryFlags queryFlags = SubscriptionQueryFlags.IncludeInvalidSubscriptions | SubscriptionQueryFlags.IncludeDeletedSubscriptions | SubscriptionQueryFlags.IncludeFilterDetails)
    {
      return this.TfsRequestContext.GetService<INotificationSubscriptionService>().GetNotificationSubscription(this.TfsRequestContext, subscriptionId, queryFlags);
    }

    [HttpGet]
    [ClientExample("GET__notification_subscriptions.json", null, null, null)]
    public IEnumerable<NotificationSubscription> ListSubscriptions(
      Guid? targetId = null,
      [ClientParameterAsIEnumerable(typeof (string), ',')] string ids = null,
      SubscriptionQueryFlags queryFlags = SubscriptionQueryFlags.None)
    {
      INotificationSubscriptionService service = this.TfsRequestContext.GetService<INotificationSubscriptionService>();
      if (string.IsNullOrEmpty(ids))
        return service.GetNotificationSubscriptionsForTarget(this.TfsRequestContext, targetId.HasValue ? targetId.Value : Guid.Empty, queryFlags);
      return (IEnumerable<NotificationSubscription>) service.GetNotificationSubscriptions(this.TfsRequestContext, ((IEnumerable<string>) ids.Split(',')).ToList<string>(), queryFlags);
    }

    [HttpPost]
    [ClientExample("POST__notification_subscriptions.json", "Create a personal subscription", null, null)]
    [ClientExample("POST__notification_subscriptions2.json", "Create a team subscription", null, null)]
    [ClientExample("POST__notification_subscriptions__work_item.json", "Subscribe to work item changes", null, null)]
    [ClientExample("POST__notification_subscriptions__pull_request.json", "Subscribe to pull request changes", null, null)]
    public NotificationSubscription CreateSubscription(
      NotificationSubscriptionCreateParameters createParameters)
    {
      this.LoggableDiagnosticParameters[nameof (createParameters)] = (object) createParameters;
      return this.TfsRequestContext.GetService<INotificationSubscriptionService>().CreateSubscription(this.TfsRequestContext, createParameters);
    }

    [HttpDelete]
    [ClientResponseType(typeof (void), null, null)]
    [ClientExample("DELETE__notification_subscriptions__subscriptionId_.json", "Delete a subscription", null, null)]
    public void DeleteSubscription(string subscriptionId) => this.TfsRequestContext.GetService<INotificationSubscriptionService>().DeleteSubscription(this.TfsRequestContext, subscriptionId);

    [HttpPatch]
    [ClientExample("PATCH__notification_subscriptions__subscriptionId_.json", "Change description", null, null)]
    [ClientExample("PATCH__notification_subscriptions__subscriptionId_2.json", "Disable a subscription", null, null)]
    public NotificationSubscription UpdateSubscription(
      string subscriptionId,
      NotificationSubscriptionUpdateParameters updateParameters)
    {
      this.LoggableDiagnosticParameters[nameof (updateParameters)] = (object) updateParameters;
      return this.TfsRequestContext.GetService<INotificationSubscriptionService>().UpdateSubscription(this.TfsRequestContext, subscriptionId, updateParameters);
    }

    protected override void InitializeExceptionMap(ApiExceptionMapping exceptionMap)
    {
      base.InitializeExceptionMap(exceptionMap);
      exceptionMap.AddStatusCode<ArgumentException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<EventTypeNotSupportedException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<InvalidSubscriptionException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<SubscriptionNotFoundException>(HttpStatusCode.NotFound);
      exceptionMap.AddStatusCode<NotificationEventTypeNotAllowedException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<NotificationEventTypeNotFoundException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<NotificationSubscriptionChannelNotAllowedException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<InvalidIdentityException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<InvalidQueryFlagException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<IdentityNotFoundException>(HttpStatusCode.BadRequest);
    }
  }
}
