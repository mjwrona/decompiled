// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceHooks.Server.WebApi.HooksSvcSubscriptionsController
// Assembly: Microsoft.VisualStudio.Services.ServiceHooks.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 25B6D63E-3809-4A04-9AE1-1F77D8FFEE67
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ServiceHooks.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.FormInput;
using Microsoft.VisualStudio.Services.ServiceHooks.WebApi;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.ServiceHooks.Server.WebApi
{
  [VersionedApiControllerCustomName(Area = "hookssvc", ResourceName = "Subscriptions")]
  public class HooksSvcSubscriptionsController : ServiceHooksSvcControllerBase
  {
    [HttpGet]
    public IEnumerable<Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription> ListSubscriptions(
      string publisherId = null,
      string eventType = null,
      string consumerId = null,
      string consumerActionId = null)
    {
      this.CheckPermission(this.TfsRequestContext, 1);
      IEnumerable<Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription> subscriptions = this.TfsRequestContext.GetService<ServiceHooksService>().QuerySubscriptions(this.TfsRequestContext, new SubscriptionStatus?(), publisherId, eventType, (IEnumerable<InputFilter>) null, (IEnumerable<InputFilter>) null, consumerId, consumerActionId);
      subscriptions.SetSubscriptionUrl(this.Url, this.TfsRequestContext);
      return subscriptions;
    }

    public Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription GetSubscription(
      Guid subscriptionId)
    {
      this.CheckPermission(this.TfsRequestContext, 1);
      Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription subscription = this.TfsRequestContext.GetService<ServiceHooksService>().GetSubscription(this.TfsRequestContext, subscriptionId);
      subscription.SetSubscriptionUrl(this.Url, this.TfsRequestContext);
      return subscription;
    }

    [HttpPost]
    public Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription CreateSubscription(
      Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription subscription)
    {
      this.CheckPermission(this.TfsRequestContext, 2);
      Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription subscription1 = this.TfsRequestContext.GetService<ServiceHooksService>().CreateSubscription(this.TfsRequestContext, subscription);
      subscription1.SetSubscriptionUrl(this.Url, this.TfsRequestContext);
      return subscription1;
    }

    [HttpPut]
    public Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription ReplaceSubscription(
      Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription subscription)
    {
      this.CheckPermission(this.TfsRequestContext, 2);
      Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription subscription1 = this.TfsRequestContext.GetService<ServiceHooksService>().UpdateSubscription(this.TfsRequestContext, subscription);
      subscription1.SetSubscriptionUrl(this.Url, this.TfsRequestContext);
      return subscription1;
    }

    [ClientResponseType(typeof (void), null, null)]
    public HttpResponseMessage DeleteSubscription(Guid subscriptionId)
    {
      if (!this.CheckPermission(this.TfsRequestContext, 2, false))
        this.CheckPermission(this.TfsRequestContext, 4);
      this.TfsRequestContext.GetService<ServiceHooksService>().DeleteSubscription(this.TfsRequestContext, subscriptionId);
      return this.Request.CreateResponse(HttpStatusCode.NoContent);
    }
  }
}
