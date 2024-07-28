// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceHooks.WebApi.ServiceHooksPublisherHttpClient
// Assembly: Microsoft.VisualStudio.Services.ServiceHooks.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8FEBD486-B6EA-43F6-B878-5BE1581FAD28
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ServiceHooks.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.FormInput;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.ServiceHooks.WebApi
{
  public class ServiceHooksPublisherHttpClient : ServiceHooksHttpClientBase
  {
    public ServiceHooksPublisherHttpClient(Uri baseUrl, VssCredentials credentials)
      : base(baseUrl, credentials)
    {
    }

    public ServiceHooksPublisherHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings)
      : base(baseUrl, credentials, settings)
    {
    }

    public ServiceHooksPublisherHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, handlers)
    {
    }

    public ServiceHooksPublisherHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, settings, handlers)
    {
    }

    public ServiceHooksPublisherHttpClient(
      Uri baseUrl,
      HttpMessageHandler pipeline,
      bool disposeHandler)
      : base(baseUrl, pipeline, disposeHandler)
    {
    }

    public virtual Task<Publisher> GetPublisherAsync(string publisherId, object userState = null)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(publisherId, nameof (publisherId));
      return this.SendAsync<Publisher>(HttpMethod.Get, ServiceHooksPublisherApiConstants.PublishersLocationId, (object) new
      {
        publisherId = publisherId
      }, userState: userState);
    }

    public Task<IList<Publisher>> GetPublishersAsync(object userState = null) => this.SendAsync<IList<Publisher>>(HttpMethod.Get, ServiceHooksPublisherApiConstants.PublishersLocationId, userState: userState);

    public Task<PublishersQuery> QueryPublishersAsync(PublishersQuery query, object userState = null)
    {
      ArgumentUtility.CheckForNull<PublishersQuery>(query, nameof (query));
      return this.SendAsync<PublishersQuery>(HttpMethod.Post, ServiceHooksPublisherApiConstants.PublishersQueryLocationId, content: (HttpContent) new ObjectContent<PublishersQuery>(query, this.Formatter), userState: userState);
    }

    public Task<Consumer> GetConsumerAsync(string consumerId, object userState = null)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(consumerId, nameof (consumerId));
      return this.SendAsync<Consumer>(HttpMethod.Get, ServiceHooksPublisherApiConstants.ConsumersLocationId, (object) new
      {
        consumerId = consumerId
      }, userState: userState);
    }

    public Task<IList<Consumer>> GetConsumersAsync(object userState = null) => this.SendAsync<IList<Consumer>>(HttpMethod.Get, ServiceHooksPublisherApiConstants.ConsumersLocationId, userState: userState);

    public Task<IList<ConsumerAction>> GetConsumerActionsAsync(string consumerId, object userState = null)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(consumerId, nameof (consumerId));
      return this.SendAsync<IList<ConsumerAction>>(HttpMethod.Get, ServiceHooksPublisherApiConstants.ConsumerActionsLocationId, (object) new
      {
        consumerId = consumerId
      }, userState: userState);
    }

    public Task<ConsumerAction> GetConsumerActionAsync(
      string consumerId,
      string consumerActionId,
      object userState = null)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(consumerId, nameof (consumerId));
      ArgumentUtility.CheckStringForNullOrEmpty(consumerActionId, nameof (consumerActionId));
      return this.SendAsync<ConsumerAction>(HttpMethod.Get, ServiceHooksPublisherApiConstants.ConsumerActionsLocationId, (object) new
      {
        consumerId = consumerId,
        consumerActionId = consumerActionId
      }, userState: userState);
    }

    public Task<IList<Subscription>> QuerySubscriptionsAsync(
      string publisherId = null,
      IEnumerable<InputFilter> inputFilters = null,
      object userState = null)
    {
      if (inputFilters != null)
      {
        SubscriptionsQuery subscriptionsQuery = new SubscriptionsQuery()
        {
          PublisherInputFilters = inputFilters,
          PublisherId = publisherId
        };
        return this.SendAsync<SubscriptionsQuery>(HttpMethod.Post, ServiceHooksPublisherApiConstants.SubscriptionsQueryLocationId, content: (HttpContent) new ObjectContent<SubscriptionsQuery>(subscriptionsQuery, this.Formatter), userState: userState).ContinueWith<IList<Subscription>>((Func<Task<SubscriptionsQuery>, IList<Subscription>>) (queryTask => queryTask.Result.Results));
      }
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (!string.IsNullOrEmpty(publisherId))
        keyValuePairList.Add(nameof (publisherId), publisherId);
      return this.SendAsync<IList<Subscription>>(HttpMethod.Get, ServiceHooksPublisherApiConstants.SubscriptionsLocationId, queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState);
    }

    public virtual Task<SubscriptionsQuery> QuerySubscriptionsAsync(
      SubscriptionsQuery subscriptionsQuery,
      object userState = null)
    {
      ArgumentUtility.CheckForNull<SubscriptionsQuery>(subscriptionsQuery, nameof (subscriptionsQuery));
      return this.SendAsync<SubscriptionsQuery>(HttpMethod.Post, ServiceHooksPublisherApiConstants.SubscriptionsQueryLocationId, content: (HttpContent) new ObjectContent<SubscriptionsQuery>(subscriptionsQuery, this.Formatter), userState: userState);
    }

    public virtual Task<Subscription> GetSubscriptionAsync(Guid subscriptionId, object userState = null)
    {
      ArgumentUtility.CheckForEmptyGuid(subscriptionId, nameof (subscriptionId));
      return this.SendAsync<Subscription>(HttpMethod.Get, ServiceHooksPublisherApiConstants.SubscriptionsLocationId, (object) new
      {
        subscriptionId = subscriptionId
      }, userState: userState);
    }

    public virtual Task<Subscription> CreateSubscriptionAsync(
      Subscription subscription,
      object userState = null)
    {
      ArgumentUtility.CheckForNull<Subscription>(subscription, nameof (subscription));
      return this.SendAsync<Subscription>(HttpMethod.Post, ServiceHooksPublisherApiConstants.SubscriptionsLocationId, content: (HttpContent) new ObjectContent<Subscription>(subscription, this.Formatter), userState: userState);
    }

    public Task<Subscription> UpdateSubscriptionAsync(Subscription subscription, object userState = null)
    {
      ArgumentUtility.CheckForNull<Subscription>(subscription, nameof (subscription));
      return this.SendAsync<Subscription>(HttpMethod.Put, ServiceHooksPublisherApiConstants.SubscriptionsLocationId, content: (HttpContent) new ObjectContent<Subscription>(subscription, this.Formatter), userState: userState);
    }

    public virtual Task DeleteSubscriptionAsync(Guid subscriptionId, object userState = null)
    {
      ArgumentUtility.CheckForEmptyGuid(subscriptionId, nameof (subscriptionId));
      HttpMethod delete = HttpMethod.Delete;
      Guid subscriptionsLocationId = ServiceHooksPublisherApiConstants.SubscriptionsLocationId;
      List<KeyValuePair<string, string>> queryParameters = new List<KeyValuePair<string, string>>();
      queryParameters.Add(new KeyValuePair<string, string>(nameof (subscriptionId), subscriptionId.ToString()));
      object userState1 = userState;
      CancellationToken cancellationToken = new CancellationToken();
      return (Task) this.SendAsync(delete, subscriptionsLocationId, queryParameters: (IEnumerable<KeyValuePair<string, string>>) queryParameters, userState: userState1, cancellationToken: cancellationToken);
    }

    public Task<IList<Notification>> GetNotifications(
      Guid subscriptionId,
      int? maxResults,
      NotificationStatus? status,
      NotificationResult? result,
      object userState = null)
    {
      ArgumentUtility.CheckForEmptyGuid(subscriptionId, nameof (subscriptionId));
      List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
      if (maxResults.HasValue)
        collection.Add(nameof (maxResults), maxResults.Value.ToString());
      if (status.HasValue)
        collection.Add(nameof (status), status.Value.ToString());
      if (result.HasValue)
        collection.Add(nameof (result), result.Value.ToString());
      HttpMethod get = HttpMethod.Get;
      Guid notificationsLocationId = ServiceHooksPublisherApiConstants.NotificationsLocationId;
      object obj = userState;
      var routeValues = new
      {
        subscriptionId = subscriptionId
      };
      List<KeyValuePair<string, string>> queryParameters = collection;
      object userState1 = obj;
      CancellationToken cancellationToken = new CancellationToken();
      return this.SendAsync<IList<Notification>>(get, notificationsLocationId, (object) routeValues, queryParameters: (IEnumerable<KeyValuePair<string, string>>) queryParameters, userState: userState1, cancellationToken: cancellationToken);
    }

    public Task<Notification> GetNotification(
      Guid subscriptionId,
      int notificationId,
      object userState = null)
    {
      ArgumentUtility.CheckForEmptyGuid(subscriptionId, nameof (subscriptionId));
      ArgumentUtility.CheckForOutOfRange(notificationId, nameof (notificationId), 1);
      HttpMethod get = HttpMethod.Get;
      Guid notificationsLocationId = ServiceHooksPublisherApiConstants.NotificationsLocationId;
      object obj = userState;
      var routeValues = new
      {
        subscriptionId = subscriptionId,
        notificationId = notificationId
      };
      object userState1 = obj;
      CancellationToken cancellationToken = new CancellationToken();
      return this.SendAsync<Notification>(get, notificationsLocationId, (object) routeValues, userState: userState1, cancellationToken: cancellationToken);
    }

    public Task<NotificationsQuery> QueryNotificationsAsync(
      NotificationsQuery query,
      object userState = null)
    {
      ArgumentUtility.CheckForNull<NotificationsQuery>(query, nameof (query));
      return this.SendAsync<NotificationsQuery>(HttpMethod.Post, ServiceHooksPublisherApiConstants.NotificationsQueryLocationId, content: (HttpContent) new ObjectContent<NotificationsQuery>(query, this.Formatter), userState: userState);
    }

    public Task<Notification> PostTestNotification(Notification notification, object userState = null)
    {
      ArgumentUtility.CheckForNull<Notification>(notification, nameof (notification));
      HttpMethod post = HttpMethod.Post;
      Guid notificationsLocationId = ServiceHooksPublisherApiConstants.TestNotificationsLocationId;
      object obj = userState;
      ObjectContent<Notification> content = new ObjectContent<Notification>(notification, this.Formatter);
      object userState1 = obj;
      CancellationToken cancellationToken = new CancellationToken();
      return this.SendAsync<Notification>(post, notificationsLocationId, content: (HttpContent) content, userState: userState1, cancellationToken: cancellationToken);
    }

    public virtual Task<SubscriptionInputValuesQuery> QueryInputValuesAsync(
      SubscriptionInputValuesQuery query,
      object userState = null)
    {
      ArgumentUtility.CheckForNull<SubscriptionInputValuesQuery>(query, nameof (query));
      return this.SendAsync<SubscriptionInputValuesQuery>(HttpMethod.Post, ServiceHooksPublisherApiConstants.InputValuesQueryLocationId, content: (HttpContent) new ObjectContent<SubscriptionInputValuesQuery>(query, this.Formatter), userState: userState);
    }
  }
}
