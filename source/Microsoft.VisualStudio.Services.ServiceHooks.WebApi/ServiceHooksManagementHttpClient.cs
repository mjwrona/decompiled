// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceHooks.WebApi.ServiceHooksManagementHttpClient
// Assembly: Microsoft.VisualStudio.Services.ServiceHooks.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8FEBD486-B6EA-43F6-B878-5BE1581FAD28
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ServiceHooks.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.FormInput;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.ServiceHooks.WebApi
{
  [ResourceArea("6F0D0CB2-7079-41FA-AEEF-4772F7A835F7")]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class ServiceHooksManagementHttpClient : ServiceHooksHttpClientBase
  {
    public ServiceHooksManagementHttpClient(Uri baseUrl, VssCredentials credentials)
      : base(baseUrl, credentials)
    {
    }

    public ServiceHooksManagementHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings)
      : base(baseUrl, credentials, settings)
    {
    }

    public ServiceHooksManagementHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, handlers)
    {
    }

    public ServiceHooksManagementHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, settings, handlers)
    {
    }

    public ServiceHooksManagementHttpClient(
      Uri baseUrl,
      HttpMessageHandler pipeline,
      bool disposeHandler)
      : base(baseUrl, pipeline, disposeHandler)
    {
    }

    public Task<IList<Subscription>> QuerySubscriptionsAsync(
      string publisherId = null,
      IEnumerable<InputFilter> inputFilters = null,
      bool unmaskConfidentialInputs = false,
      object userState = null)
    {
      if (inputFilters != null)
        return this.QuerySubscriptionsAsync(new SubscriptionsQuery()
        {
          PublisherInputFilters = inputFilters,
          PublisherId = publisherId
        }, unmaskConfidentialInputs, userState);
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (!string.IsNullOrEmpty(publisherId))
        keyValuePairList.Add(nameof (publisherId), publisherId);
      return this.SendAsync<IList<Subscription>>(HttpMethod.Get, ServiceHooksApiConstants.SubscriptionsLocationId, queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState);
    }

    public Task<IList<Subscription>> QuerySubscriptionsAsync(
      SubscriptionsQuery query,
      bool unmaskConfidentialInputs = false,
      object userState = null)
    {
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (unmaskConfidentialInputs), unmaskConfidentialInputs.ToString());
      return this.SendAsync<SubscriptionsQuery>(HttpMethod.Post, ServiceHooksApiConstants.SubscriptionsQueryLocationId, content: (HttpContent) new ObjectContent<SubscriptionsQuery>(query, this.Formatter), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState).ContinueWith<IList<Subscription>>((Func<Task<SubscriptionsQuery>, IList<Subscription>>) (queryTask => queryTask.Result.Results));
    }

    public Task<Subscription> GetSubscriptionAsync(Guid subscriptionId, object userState = null) => this.SendAsync<Subscription>(HttpMethod.Get, ServiceHooksApiConstants.SubscriptionsLocationId, (object) new
    {
      subscriptionId = subscriptionId
    }, userState: userState);

    public Task<Subscription> CreateSubscriptionAsync(Subscription subscription, object userState = null) => this.SendAsync<Subscription>(HttpMethod.Post, ServiceHooksApiConstants.SubscriptionsLocationId, content: (HttpContent) new ObjectContent<Subscription>(subscription, this.Formatter), userState: userState);

    public Task<Subscription> UpdateSubscriptionAsync(Subscription subscription, object userState = null) => this.SendAsync<Subscription>(HttpMethod.Put, ServiceHooksApiConstants.SubscriptionsLocationId, content: (HttpContent) new ObjectContent<Subscription>(subscription, this.Formatter), userState: userState);

    public Task DeleteSubscriptionAsync(Guid subscriptionId, object userState = null)
    {
      HttpMethod delete = HttpMethod.Delete;
      Guid subscriptionsLocationId = ServiceHooksApiConstants.SubscriptionsLocationId;
      List<KeyValuePair<string, string>> queryParameters = new List<KeyValuePair<string, string>>();
      queryParameters.Add(new KeyValuePair<string, string>(nameof (subscriptionId), subscriptionId.ToString()));
      object userState1 = userState;
      CancellationToken cancellationToken = new CancellationToken();
      return (Task) this.SendAsync(delete, subscriptionsLocationId, queryParameters: (IEnumerable<KeyValuePair<string, string>>) queryParameters, userState: userState1, cancellationToken: cancellationToken);
    }

    public Task<Consumer> GetConsumerAsync(string consumerId, object userState = null) => this.SendAsync<Consumer>(HttpMethod.Get, ServiceHooksApiConstants.ConsumersLocationId, (object) new
    {
      consumerId = consumerId
    }, userState: userState);

    public Task<IList<Consumer>> GetConsumersAsync(object userState = null) => this.SendAsync<IList<Consumer>>(HttpMethod.Get, ServiceHooksApiConstants.ConsumersLocationId, userState: userState);

    public Task PublishEventsAsync(IEnumerable<PublisherEvent> events, object userState = null)
    {
      PublishEventsRequestData eventsRequestData = new PublishEventsRequestData()
      {
        Events = events
      };
      HttpMethod post = HttpMethod.Post;
      Guid eventsLocationId = ServiceHooksApiConstants.EventsLocationId;
      object obj = userState;
      ObjectContent<PublishEventsRequestData> content = new ObjectContent<PublishEventsRequestData>(eventsRequestData, this.Formatter);
      object userState1 = obj;
      CancellationToken cancellationToken = new CancellationToken();
      return (Task) this.SendAsync(post, eventsLocationId, content: (HttpContent) content, userState: userState1, cancellationToken: cancellationToken);
    }

    public Task<IList<Notification>> GetNotifications(
      Guid subscriptionId,
      int? maxResults,
      NotificationStatus? status,
      NotificationResult? result,
      object userState = null)
    {
      List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
      if (maxResults.HasValue)
        collection.Add(nameof (maxResults), maxResults.Value.ToString());
      if (status.HasValue)
        collection.Add(nameof (status), status.Value.ToString());
      if (result.HasValue)
        collection.Add(nameof (result), result.Value.ToString());
      HttpMethod get = HttpMethod.Get;
      Guid notificationsLocationId = ServiceHooksApiConstants.NotificationsLocationId;
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
      HttpMethod get = HttpMethod.Get;
      Guid notificationsLocationId = ServiceHooksApiConstants.NotificationsLocationId;
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

    public Task<Notification> PostTestNotification(Notification notification, object userState = null)
    {
      HttpMethod post = HttpMethod.Post;
      Guid notificationsLocationId = ServiceHooksApiConstants.TestNotificationsLocationId;
      object obj = userState;
      ObjectContent<Notification> content = new ObjectContent<Notification>(notification, this.Formatter);
      object userState1 = obj;
      CancellationToken cancellationToken = new CancellationToken();
      return this.SendAsync<Notification>(post, notificationsLocationId, content: (HttpContent) content, userState: userState1, cancellationToken: cancellationToken);
    }

    public Task<NotificationsQuery> QueryNotificationsAsync(
      NotificationsQuery query,
      object userState = null)
    {
      return this.SendAsync<NotificationsQuery>(HttpMethod.Post, ServiceHooksApiConstants.NotificationsQueryLocationId, content: (HttpContent) new ObjectContent<NotificationsQuery>(query, this.Formatter), userState: userState);
    }

    public Task<SubscriptionInputValuesQuery> QueryInputValuesAsync(
      SubscriptionInputValuesQuery query,
      object userState = null)
    {
      return this.SendAsync<SubscriptionInputValuesQuery>(HttpMethod.Post, ServiceHooksApiConstants.InputValuesQueryLocationId, content: (HttpContent) new ObjectContent<SubscriptionInputValuesQuery>(query, this.Formatter), userState: userState);
    }
  }
}
