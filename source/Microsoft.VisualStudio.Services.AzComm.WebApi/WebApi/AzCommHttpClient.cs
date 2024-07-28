// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.AzComm.WebApi.AzCommHttpClient
// Assembly: Microsoft.VisualStudio.Services.AzComm.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B1B69FBB-72A0-4C7F-B8FC-E0B0311A8184
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.AzComm.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Net.Http;

namespace Microsoft.VisualStudio.Services.AzComm.WebApi
{
  [ClientCircuitBreakerSettings(30, 80, MaxConcurrentRequests = 40)]
  [ClientCancellationTimeout(60)]
  public class AzCommHttpClient : AzCommHttpClientBase
  {
    public AzCommHttpClient(Uri baseUrl, VssCredentials credentials)
      : base(baseUrl, credentials)
    {
    }

    public AzCommHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings)
      : base(baseUrl, credentials, settings)
    {
    }

    public AzCommHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, handlers)
    {
    }

    public AzCommHttpClient(Uri baseUrl, HttpMessageHandler pipeline, bool disposeHandler)
      : base(baseUrl, pipeline, disposeHandler)
    {
    }

    public AzCommHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, settings, handlers)
    {
    }
  }
}
