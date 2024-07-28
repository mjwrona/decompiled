// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceHooks.Sdk.Server.HooksPublisherSubscriptionsController
// Assembly: Microsoft.VisualStudio.Services.ServiceHooks.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3575E571-FF3A-4E7B-A8CC-64FFB01E8C91
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ServiceHooks.Sdk.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.ServiceHooks.WebApi;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.ServiceHooks.Sdk.Server
{
  [VersionedApiControllerCustomName(Area = "hooks", ResourceName = "Subscriptions")]
  public class HooksPublisherSubscriptionsController : ServiceHooksPublisherControllerBase
  {
    [HttpGet]
    [ClientExample("GET__hooks_subscriptions_.json", null, null, null)]
    public IEnumerable<Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription> ListSubscriptions(
      string publisherId = null,
      string eventType = null,
      string consumerId = null,
      string consumerActionId = null)
    {
      if (!string.IsNullOrEmpty(eventType))
        this.CheckScope(eventType);
      SubscriptionsQuery query = new SubscriptionsQuery()
      {
        PublisherId = publisherId,
        EventType = eventType,
        ConsumerId = consumerId,
        ConsumerActionId = consumerActionId
      };
      List<Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription> subscriptions = new List<Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription>();
      List<string> userEventTypes = this.GetUserEventTypes();
      foreach (ServiceHooksPublisher publisher in this.FindPublishers(publisherId))
      {
        foreach (Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription querySubscription in publisher.QuerySubscriptions(this.TfsRequestContext, query))
        {
          if (userEventTypes.Contains(querySubscription.EventType))
            subscriptions.Add(querySubscription);
        }
      }
      subscriptions.SetSubscriptionUrl(this.Url, this.TfsRequestContext);
      subscriptions.OverrideServiceIdentity(this.TfsRequestContext);
      return (IEnumerable<Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription>) subscriptions;
    }

    [ClientExample("GET__hooks_subscriptions__subscriptionId_.json", null, null, null)]
    public Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription GetSubscription(
      Guid subscriptionId)
    {
      Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription subscription = this.GetSubscription(this.TfsRequestContext, subscriptionId);
      this.CheckScope(subscription.EventType);
      ServiceHooksPublisher publisher = this.FindPublisher(subscription.PublisherId);
      publisher.CheckPermission(this.TfsRequestContext, subscription);
      publisher.UpdateSubscriptionStatus(this.TfsRequestContext, subscription);
      subscription.SetSubscriptionUrl(this.Url, this.TfsRequestContext);
      subscription.OverrideServiceIdentity(this.TfsRequestContext);
      subscription.SetSubscriptionSubscriber(this.TfsRequestContext);
      return subscription;
    }

    [HttpPost]
    [ClientExample("PUT__hooks_subscriptions__newSubscriptionId_.json", null, null, null)]
    public Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription CreateSubscription(
      Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription subscription)
    {
      ArgumentUtility.CheckForNull<Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription>(subscription, nameof (subscription));
      subscription.Validate(true);
      this.CheckScope(subscription.EventType, true);
      Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription subscription1 = this.FindPublisher(subscription.PublisherId).CreateSubscription(this.TfsRequestContext, subscription);
      subscription1.SetSubscriptionUrl(this.Url, this.TfsRequestContext);
      return subscription1;
    }

    [HttpPut]
    [ClientExample("POST__hooks_subscriptions.json", null, null, null)]
    public Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription ReplaceSubscription(
      Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription subscription,
      Guid? subscriptionId = null)
    {
      ArgumentUtility.CheckForNull<Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription>(subscription, nameof (subscription));
      if (subscriptionId.HasValue)
        subscription.Id = subscriptionId.Value;
      subscription.Validate(false);
      Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription subscription1 = this.GetSubscription(this.TfsRequestContext, subscription.Id);
      this.FindPublisher(subscription1.PublisherId).CheckPermission(this.TfsRequestContext, subscription, writePermission: true);
      this.CheckScope(subscription1.EventType);
      if (!subscription.EventType.Equals(subscription1.EventType, StringComparison.InvariantCultureIgnoreCase))
        this.CheckScope(subscription.EventType, true);
      Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription subscription2 = this.FindPublisher(subscription.PublisherId).UpdateSubscription(this.TfsRequestContext, subscription1, subscription);
      subscription2.SetSubscriptionUrl(this.Url, this.TfsRequestContext);
      return subscription2;
    }

    [ClientResponseType(typeof (void), null, null)]
    [ClientExample("DELETE__hooks_subscriptions__newSubscriptionId_.json", null, null, null)]
    public HttpResponseMessage DeleteSubscription(Guid subscriptionId)
    {
      Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription subscription = this.GetSubscription(this.TfsRequestContext, subscriptionId);
      this.CheckScope(subscription.EventType, true);
      this.FindPublisher(subscription.PublisherId).DeleteSubscription(this.TfsRequestContext, subscription);
      return this.Request.CreateResponse(HttpStatusCode.NoContent);
    }

    protected override void InitializeExceptionMap(ApiExceptionMapping exceptionMap)
    {
      base.InitializeExceptionMap(exceptionMap);
      exceptionMap.AddStatusCode<SubscriptionInputException>(HttpStatusCode.BadRequest);
    }
  }
}
