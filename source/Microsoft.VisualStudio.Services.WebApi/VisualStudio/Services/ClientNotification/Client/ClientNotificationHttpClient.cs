// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ClientNotification.Client.ClientNotificationHttpClient
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.ClientNotification.Client
{
  [ResourceArea("C2845FF0-342A-4059-A831-AA7A5BF00FF0")]
  [ClientCircuitBreakerSettings(10, 80, MaxConcurrentRequests = 40)]
  [ClientCancellationTimeout(30)]
  public class ClientNotificationHttpClient : ClientNotificationHttpClientBase
  {
    protected ClientNotificationHttpClient(Uri baseUrl, VssCredentials credentials)
      : base(baseUrl, credentials)
    {
    }

    public ClientNotificationHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings)
      : base(baseUrl, credentials, settings)
    {
    }

    public ClientNotificationHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, handlers)
    {
    }

    public ClientNotificationHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, settings, handlers)
    {
    }

    public ClientNotificationHttpClient(
      Uri baseUrl,
      HttpMessageHandler pipeline,
      bool disposeHandler)
      : base(baseUrl, pipeline, disposeHandler)
    {
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("Use GetSubscriptionAsync instead.")]
    public virtual Task<ClientNotificationSubscription> RegisterNotificationAsync(
      ClientNotificationHttpContext context,
      CancellationToken cancellationToken = default (CancellationToken),
      string id = "me",
      object userState = null)
    {
      return this.GetSubscriptionAsync(userState, cancellationToken);
    }

    public override Task<ClientNotificationSubscription> GetSubscriptionAsync(
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("GET");
      Guid guid = new Guid("e037c69c-5ad1-4b26-b340-51c18035516f");
      object obj1 = (object) new{ id = "me" };
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      ApiResourceVersion apiResourceVersion = new ApiResourceVersion("5.0-preview.2");
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      object routeValues = obj1;
      ApiResourceVersion version = apiResourceVersion;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<ClientNotificationSubscription>(method, locationId, routeValues, version, userState: userState1, cancellationToken: cancellationToken2);
    }

    protected override IDictionary<string, Type> TranslatedExceptions { get; } = (IDictionary<string, Type>) new Dictionary<string, Type>()
    {
      {
        "NotAuthorizedException",
        typeof (NotAuthorizedException)
      }
    };
  }
}
