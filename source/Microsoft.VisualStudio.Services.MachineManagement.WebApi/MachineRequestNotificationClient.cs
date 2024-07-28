// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.MachineManagement.WebApi.MachineRequestNotificationClient
// Assembly: Microsoft.VisualStudio.Services.MachineManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0CB85E58-B74B-46EE-B86D-9E028F77476B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.MachineManagement.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.MachineManagement.WebApi
{
  public sealed class MachineRequestNotificationClient : VssHttpClientBase
  {
    private readonly ApiResourceVersion m_currentApiVersion = new ApiResourceVersion(1.0);

    public MachineRequestNotificationClient(Uri baseUrl, VssCredentials credentials)
      : base(baseUrl, credentials)
    {
    }

    public MachineRequestNotificationClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings)
      : base(baseUrl, credentials, settings)
    {
    }

    public MachineRequestNotificationClient(
      Uri baseUrl,
      VssCredentials credentials,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, handlers)
    {
    }

    public MachineRequestNotificationClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, settings, handlers)
    {
    }

    public MachineRequestNotificationClient(
      Uri baseUrl,
      HttpMessageHandler pipeline,
      bool disposeHandler)
      : base(baseUrl, pipeline, disposeHandler)
    {
    }

    public Task<MachineRequestNotification> NotifyRequestChangedAsync(
      string routeTypeValue,
      MachineRequestNotification requestNotification,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      var routeValues = new{ poolType = routeTypeValue };
      return this.PostAsync<MachineRequestNotification, MachineRequestNotification>(requestNotification, MachineManagementResourceIds.RequestNotificationsLocationId, (object) routeValues, this.m_currentApiVersion, userState: userState, cancellationToken: cancellationToken);
    }

    public async Task<Stream> GetMachineRequestResourceAsync(
      string poolType,
      string resourceVersion,
      string resourcePlatform,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      MachineRequestNotificationClient notificationClient1 = this;
      var data = new
      {
        poolType = poolType,
        resourceVersion = resourceVersion
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (!string.IsNullOrEmpty(resourcePlatform))
        keyValuePairList.Add(new KeyValuePair<string, string>(nameof (resourcePlatform), resourcePlatform));
      MachineRequestNotificationClient notificationClient2 = notificationClient1;
      Guid notificationsLocationId = MachineManagementResourceIds.RequestNotificationsLocationId;
      var routeValues = data;
      IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) keyValuePairList;
      ApiResourceVersion currentApiVersion = notificationClient1.m_currentApiVersion;
      IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
      object userState1 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      return await (await notificationClient2.GetAsync(notificationsLocationId, (object) routeValues, currentApiVersion, queryParameters, userState1, cancellationToken1)).Content.ReadAsStreamAsync();
    }
  }
}
