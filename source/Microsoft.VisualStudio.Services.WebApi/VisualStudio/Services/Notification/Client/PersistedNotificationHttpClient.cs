// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notification.Client.PersistedNotificationHttpClient
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Notification.Client
{
  [ResourceArea("BA8495F8-E9EE-4A9E-9CBE-142897543FE9")]
  public class PersistedNotificationHttpClient : VssHttpClientBase
  {
    protected static readonly Version previewApiVersion = new Version(1, 0);

    public PersistedNotificationHttpClient(Uri baseUrl, VssCredentials credentials)
      : base(baseUrl, credentials)
    {
    }

    public PersistedNotificationHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings)
      : base(baseUrl, credentials, settings)
    {
    }

    public PersistedNotificationHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, handlers)
    {
    }

    public PersistedNotificationHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, settings, handlers)
    {
    }

    public PersistedNotificationHttpClient(
      Uri baseUrl,
      HttpMessageHandler pipeline,
      bool disposeHandler)
      : base(baseUrl, pipeline, disposeHandler)
    {
    }

    public virtual async Task SaveNotificationsAsync(
      IEnumerable<Microsoft.VisualStudio.Services.Notification.Notification> notifications,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      PersistedNotificationHttpClient notificationHttpClient = this;
      using (new VssHttpClientBase.OperationScope("PersistedNotification", nameof (SaveNotificationsAsync)))
      {
        ArgumentUtility.CheckForNull<IEnumerable<Microsoft.VisualStudio.Services.Notification.Notification>>(notifications, nameof (notifications));
        ObjectContent<VssJsonCollectionWrapper<IEnumerable<Microsoft.VisualStudio.Services.Notification.Notification>>> content = new ObjectContent<VssJsonCollectionWrapper<IEnumerable<Microsoft.VisualStudio.Services.Notification.Notification>>>(new VssJsonCollectionWrapper<IEnumerable<Microsoft.VisualStudio.Services.Notification.Notification>>((IEnumerable) notifications), notificationHttpClient.Formatter);
        HttpResponseMessage httpResponseMessage = await notificationHttpClient.SendAsync(HttpMethod.Post, PersistedNotificationResourceIds.NotificationsId, version: new ApiResourceVersion(PersistedNotificationHttpClient.previewApiVersion, 1), content: (HttpContent) content, userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false);
      }
    }

    public virtual async Task<IList<Microsoft.VisualStudio.Services.Notification.Notification>> GetRecipientNotificationsAsync(
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      PersistedNotificationHttpClient notificationHttpClient = this;
      IList<Microsoft.VisualStudio.Services.Notification.Notification> notificationsAsync;
      using (new VssHttpClientBase.OperationScope("PersistedNotification", nameof (GetRecipientNotificationsAsync)))
        notificationsAsync = await notificationHttpClient.SendAsync<IList<Microsoft.VisualStudio.Services.Notification.Notification>>(HttpMethod.Get, PersistedNotificationResourceIds.NotificationsId, version: new ApiResourceVersion(PersistedNotificationHttpClient.previewApiVersion, 1), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false);
      return notificationsAsync;
    }

    public virtual async Task<RecipientMetadata> GetRecipientMetadataAsync(
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      PersistedNotificationHttpClient notificationHttpClient = this;
      RecipientMetadata recipientMetadataAsync;
      using (new VssHttpClientBase.OperationScope("PersistedNotification", nameof (GetRecipientMetadataAsync)))
        recipientMetadataAsync = await notificationHttpClient.SendAsync<RecipientMetadata>(HttpMethod.Get, PersistedNotificationResourceIds.RecipientMetadataId, version: new ApiResourceVersion(PersistedNotificationHttpClient.previewApiVersion, 1), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false);
      return recipientMetadataAsync;
    }

    public Task<RecipientMetadata> UpdateRecipientMetadataAsync(
      RecipientMetadata metadata,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (new VssHttpClientBase.OperationScope("PersistedNotification", nameof (UpdateRecipientMetadataAsync)))
      {
        HttpContent content = (HttpContent) new ObjectContent<RecipientMetadata>(metadata, this.Formatter);
        return this.SendAsync<RecipientMetadata>(new HttpMethod("PATCH"), PersistedNotificationResourceIds.RecipientMetadataId, version: new ApiResourceVersion(PersistedNotificationHttpClient.previewApiVersion, 1), content: content, userState: userState, cancellationToken: cancellationToken);
      }
    }
  }
}
