// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.WebApi.Clients.NotificationHttpClient
// Assembly: Microsoft.VisualStudio.Services.Notifications.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FF217E0A-7730-437B-BE9F-877363CB7392
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Notifications.WebApi.Clients
{
  public class NotificationHttpClient : VssHttpClientBase
  {
    public NotificationHttpClient(Uri baseUrl, VssCredentials credentials)
      : base(baseUrl, credentials)
    {
    }

    public NotificationHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings)
      : base(baseUrl, credentials, settings)
    {
    }

    public NotificationHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, handlers)
    {
    }

    public NotificationHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, settings, handlers)
    {
    }

    public NotificationHttpClient(Uri baseUrl, HttpMessageHandler pipeline, bool disposeHandler)
      : base(baseUrl, pipeline, disposeHandler)
    {
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual async Task PerformBatchNotificationOperationsAsync(
      BatchNotificationOperation operation,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      NotificationHttpClient notificationHttpClient1 = this;
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("8f3c6ab2-5bae-4537-b16e-f84e0955599e");
      HttpContent httpContent = (HttpContent) new ObjectContent<BatchNotificationOperation>(operation, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      NotificationHttpClient notificationHttpClient2 = notificationHttpClient1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await notificationHttpClient2.SendAsync(method, locationId, version: version, content: content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }

    public virtual Task<List<INotificationDiagnosticLog>> ListLogsAsync(
      Guid source,
      Guid? entryId = null,
      DateTime? startTime = null,
      DateTime? endTime = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("991842f3-eb16-4aea-ac81-81353ef2b75c");
      object routeValues = (object) new
      {
        source = source,
        entryId = entryId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (startTime.HasValue)
        this.AddDateTimeToQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (startTime), startTime.Value);
      if (endTime.HasValue)
        this.AddDateTimeToQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (endTime), endTime.Value);
      return this.SendAsync<List<INotificationDiagnosticLog>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<SubscriptionDiagnostics> GetSubscriptionDiagnosticsAsync(
      string subscriptionId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<SubscriptionDiagnostics>(new HttpMethod("GET"), new Guid("20f1929d-4be7-4c2e-a74e-d47640ff3418"), (object) new
      {
        subscriptionId = subscriptionId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<SubscriptionDiagnostics> UpdateSubscriptionDiagnosticsAsync(
      UpdateSubscripitonDiagnosticsParameters updateParameters,
      string subscriptionId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PUT");
      Guid guid = new Guid("20f1929d-4be7-4c2e-a74e-d47640ff3418");
      object obj1 = (object) new
      {
        subscriptionId = subscriptionId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<UpdateSubscripitonDiagnosticsParameters>(updateParameters, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<SubscriptionDiagnostics>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<VssNotificationEvent> PublishEventAsync(
      VssNotificationEvent notificationEvent,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("14c57b7a-c0e6-4555-9f51-e067188fdd8e");
      HttpContent httpContent = (HttpContent) new ObjectContent<VssNotificationEvent>(notificationEvent, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<VssNotificationEvent>(method, locationId, version: version, content: content, userState: userState1, cancellationToken: cancellationToken2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<EventTransformResult> TransformEventAsync(
      EventTransformRequest transformRequest,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("9463a800-1b44-450e-9083-f948ea174b45");
      HttpContent httpContent = (HttpContent) new ObjectContent<EventTransformRequest>(transformRequest, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<EventTransformResult>(method, locationId, version: version, content: content, userState: userState1, cancellationToken: cancellationToken2);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<NotificationEventField>> QueryEventTypesAsync(
      FieldValuesQuery inputValuesQuery,
      string eventType,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("b5bbdd21-c178-4398-b6db-0166d910028a");
      object obj1 = (object) new{ eventType = eventType };
      HttpContent httpContent = (HttpContent) new ObjectContent<FieldValuesQuery>(inputValuesQuery, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<List<NotificationEventField>>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<NotificationEventType> GetEventTypeAsync(
      string eventType,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<NotificationEventType>(new HttpMethod("GET"), new Guid("cc84fb5f-6247-4c7a-aeae-e5a3c3fddb21"), (object) new
      {
        eventType = eventType
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<NotificationEventType>> ListEventTypesAsync(
      string publisherId = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("cc84fb5f-6247-4c7a-aeae-e5a3c3fddb21");
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (publisherId != null)
        keyValuePairList.Add(nameof (publisherId), publisherId);
      return this.SendAsync<List<NotificationEventType>>(method, locationId, version: new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<NotificationReason> GetNotificationReasonsAsync(
      int notificationId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<NotificationReason>(new HttpMethod("GET"), new Guid("19824fa9-1c76-40e6-9cce-cf0b9ca1cb60"), (object) new
      {
        notificationId = notificationId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<List<NotificationReason>> ListNotificationReasonsAsync(
      int? notificationIds = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("19824fa9-1c76-40e6-9cce-cf0b9ca1cb60");
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (notificationIds.HasValue)
        keyValuePairList.Add(nameof (notificationIds), notificationIds.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      return this.SendAsync<List<NotificationReason>>(method, locationId, version: new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<NotificationAdminSettings> GetSettingsAsync(
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<NotificationAdminSettings>(new HttpMethod("GET"), new Guid("cbe076d8-2803-45ff-8d8d-44653686ea2a"), version: new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<NotificationAdminSettings> UpdateSettingsAsync(
      NotificationAdminSettingsUpdateParameters updateParameters,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("cbe076d8-2803-45ff-8d8d-44653686ea2a");
      HttpContent httpContent = (HttpContent) new ObjectContent<NotificationAdminSettingsUpdateParameters>(updateParameters, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<NotificationAdminSettings>(method, locationId, version: version, content: content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<List<NotificationStatistic>> QueryStatisticsAsync(
      NotificationStatisticsQuery statisticsQuery,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("77878ce9-6391-49af-aa9d-768ac784461f");
      HttpContent httpContent = (HttpContent) new ObjectContent<NotificationStatisticsQuery>(statisticsQuery, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<List<NotificationStatistic>>(method, locationId, version: version, content: content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<NotificationSubscriber> GetSubscriberAsync(
      Guid subscriberId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<NotificationSubscriber>(new HttpMethod("GET"), new Guid("4d5caff1-25ba-430b-b808-7a1f352cc197"), (object) new
      {
        subscriberId = subscriberId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<NotificationSubscriber> UpdateSubscriberAsync(
      NotificationSubscriberUpdateParameters updateParameters,
      Guid subscriberId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("4d5caff1-25ba-430b-b808-7a1f352cc197");
      object obj1 = (object) new
      {
        subscriberId = subscriberId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<NotificationSubscriberUpdateParameters>(updateParameters, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<NotificationSubscriber>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<List<NotificationSubscription>> QuerySubscriptionsAsync(
      SubscriptionQuery subscriptionQuery,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("6864db85-08c0-4006-8e8e-cc1bebe31675");
      HttpContent httpContent = (HttpContent) new ObjectContent<SubscriptionQuery>(subscriptionQuery, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<List<NotificationSubscription>>(method, locationId, version: version, content: content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<NotificationSubscription> CreateSubscriptionAsync(
      NotificationSubscriptionCreateParameters createParameters,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("70f911d6-abac-488c-85b3-a206bf57e165");
      HttpContent httpContent = (HttpContent) new ObjectContent<NotificationSubscriptionCreateParameters>(createParameters, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<NotificationSubscription>(method, locationId, version: version, content: content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual async Task DeleteSubscriptionAsync(
      string subscriptionId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("70f911d6-abac-488c-85b3-a206bf57e165"), (object) new
      {
        subscriptionId = subscriptionId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public virtual Task<NotificationSubscription> GetSubscriptionAsync(
      string subscriptionId,
      SubscriptionQueryFlags? queryFlags = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("70f911d6-abac-488c-85b3-a206bf57e165");
      object routeValues = (object) new
      {
        subscriptionId = subscriptionId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (queryFlags.HasValue)
        keyValuePairList.Add(nameof (queryFlags), queryFlags.Value.ToString());
      return this.SendAsync<NotificationSubscription>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<NotificationSubscription>> ListSubscriptionsAsync(
      Guid? targetId = null,
      IEnumerable<string> ids = null,
      SubscriptionQueryFlags? queryFlags = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("70f911d6-abac-488c-85b3-a206bf57e165");
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (targetId.HasValue)
        keyValuePairList.Add(nameof (targetId), targetId.Value.ToString());
      if (ids != null && ids.Any<string>())
        keyValuePairList.Add(nameof (ids), string.Join(",", ids));
      if (queryFlags.HasValue)
        keyValuePairList.Add(nameof (queryFlags), queryFlags.Value.ToString());
      return this.SendAsync<List<NotificationSubscription>>(method, locationId, version: new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<NotificationSubscription> UpdateSubscriptionAsync(
      NotificationSubscriptionUpdateParameters updateParameters,
      string subscriptionId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("70f911d6-abac-488c-85b3-a206bf57e165");
      object obj1 = (object) new
      {
        subscriptionId = subscriptionId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<NotificationSubscriptionUpdateParameters>(updateParameters, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<NotificationSubscription>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<List<NotificationSubscriptionTemplate>> GetSubscriptionTemplatesAsync(
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<NotificationSubscriptionTemplate>>(new HttpMethod("GET"), new Guid("fa5d24ba-7484-4f3d-888d-4ec6b1974082"), version: new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<VssNotificationEvent> PublishTokenEventAsync(
      VssNotificationEvent notificationEvent,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("31dc86a2-67e8-4452-99a4-2b301ba28291");
      HttpContent httpContent = (HttpContent) new ObjectContent<VssNotificationEvent>(notificationEvent, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<VssNotificationEvent>(method, locationId, version: version, content: content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<SubscriptionUserSettings> UpdateSubscriptionUserSettingsAsync(
      SubscriptionUserSettings userSettings,
      string subscriptionId,
      Guid userId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PUT");
      Guid guid = new Guid("ed5a3dff-aeb5-41b1-b4f7-89e66e58b62e");
      object obj1 = (object) new
      {
        subscriptionId = subscriptionId,
        userId = userId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<SubscriptionUserSettings>(userSettings, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<SubscriptionUserSettings>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }
  }
}
