// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceHooks.Sdk.Server.ServiceHooksClientService
// Assembly: Microsoft.VisualStudio.Services.ServiceHooks.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3575E571-FF3A-4E7B-A8CC-64FFB01E8C91
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ServiceHooks.Sdk.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.FormInput;
using Microsoft.VisualStudio.Services.ServiceHooks.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ServiceHooks.Sdk.Server
{
  public class ServiceHooksClientService
  {
    private ServiceHooksManagementHttpClient GetHttpClient(IVssRequestContext requestContext) => requestContext.GetClient<ServiceHooksManagementHttpClient>();

    public Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription GetSubscription(
      IVssRequestContext requestContext,
      Guid subscriptionId)
    {
      return this.GetHttpClient(requestContext.Elevate()).GetSubscriptionAsync(subscriptionId).SyncResult<Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription>();
    }

    public IEnumerable<Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription> QuerySubscriptions(
      IVssRequestContext requestContext,
      SubscriptionsQuery query,
      bool unmaskConfidentialInputs = false)
    {
      return (IEnumerable<Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription>) this.GetHttpClient(requestContext.Elevate()).QuerySubscriptionsAsync(query, unmaskConfidentialInputs).SyncResult<IList<Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription>>();
    }

    public virtual Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription CreateSubscription(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription subscription)
    {
      return this.GetHttpClient(requestContext.Elevate()).CreateSubscriptionAsync(subscription).SyncResult<Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription>();
    }

    public Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription UpdateSubscription(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription subscription)
    {
      return this.GetHttpClient(requestContext.Elevate()).UpdateSubscriptionAsync(subscription).SyncResult<Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription>();
    }

    public void DeleteSubscription(IVssRequestContext requestContext, Guid subscriptionId) => this.GetHttpClient(requestContext.Elevate()).DeleteSubscriptionAsync(subscriptionId).SyncResult();

    public Consumer GetConsumer(IVssRequestContext requestContext, string consumerId) => this.GetHttpClient(requestContext.Elevate()).GetConsumerAsync(consumerId).SyncResult<Consumer>();

    public IEnumerable<Consumer> GetConsumers(IVssRequestContext requestContext) => (IEnumerable<Consumer>) this.GetHttpClient(requestContext.Elevate()).GetConsumersAsync().SyncResult<IList<Consumer>>();

    public Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Notification GetNotification(
      IVssRequestContext requestContext,
      Guid subscriptionId,
      int notificationId)
    {
      return this.GetHttpClient(requestContext.Elevate()).GetNotification(subscriptionId, notificationId).SyncResult<Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Notification>();
    }

    public IList<Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Notification> GetNotifications(
      IVssRequestContext requestContext,
      Guid subscriptionId,
      NotificationStatus? status,
      NotificationResult? result,
      int? maxResults)
    {
      return this.GetHttpClient(requestContext.Elevate()).GetNotifications(subscriptionId, maxResults, status, result).SyncResult<IList<Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Notification>>();
    }

    public Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Notification SendTestNotification(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Notification testNotification)
    {
      return this.GetHttpClient(requestContext.Elevate()).PostTestNotification(testNotification).SyncResult<Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Notification>();
    }

    public IList<InputValues> GetConsumerInputValues(
      IVssRequestContext requestContext,
      string consumerId,
      string consumerActionId,
      IDictionary<string, string> currentConsumerInputValues,
      IEnumerable<string> inputIds,
      Guid? subscriptionId)
    {
      return this.GetHttpClient(requestContext.Elevate()).QueryInputValuesAsync(new SubscriptionInputValuesQuery()
      {
        Scope = SubscriptionInputScope.Consumer,
        InputValues = (IList<InputValues>) inputIds.Select<string, InputValues>((Func<string, InputValues>) (id => new InputValues()
        {
          InputId = id
        })).ToList<InputValues>(),
        Subscription = new Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Subscription()
        {
          Id = subscriptionId ?? Guid.Empty,
          ConsumerId = consumerId,
          ConsumerActionId = consumerActionId,
          ConsumerInputs = currentConsumerInputValues
        }
      }).SyncResult<SubscriptionInputValuesQuery>().InputValues;
    }

    public void PublishEvents(
      IVssRequestContext requestContext,
      IEnumerable<PublisherEvent> eventsToPublish)
    {
      this.GetHttpClient(requestContext.Elevate()).PublishEventsAsync(eventsToPublish).SyncResult();
    }

    public NotificationsQuery QueryNotifications(
      IVssRequestContext requestContext,
      NotificationsQuery query)
    {
      return this.GetHttpClient(requestContext.Elevate()).QueryNotificationsAsync(query).SyncResult<NotificationsQuery>();
    }
  }
}
