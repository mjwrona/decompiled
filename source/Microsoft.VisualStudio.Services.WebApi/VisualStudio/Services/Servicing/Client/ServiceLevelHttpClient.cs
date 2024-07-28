// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Servicing.Client.ServiceLevelHttpClient
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.ComponentModel;
using System.Net.Http;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Servicing.Client
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class ServiceLevelHttpClient : VssHttpClientBase
  {
    public ServiceLevelHttpClient(Uri baseUrl, VssCredentials credentials)
      : base(baseUrl, credentials)
    {
    }

    public ServiceLevelHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings)
      : base(baseUrl, credentials, settings)
    {
    }

    public ServiceLevelHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, handlers)
    {
    }

    public ServiceLevelHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, settings, handlers)
    {
    }

    public ServiceLevelHttpClient(Uri baseUrl, HttpMessageHandler pipeline, bool disposeHandler)
      : base(baseUrl, pipeline, disposeHandler)
    {
    }

    public async Task<ServiceLevelData> GetServiceLevelAsync()
    {
      ServiceLevelHttpClient serviceLevelHttpClient = this;
      ServiceLevelData serviceLevelAsync;
      using (new VssHttpClientBase.OperationScope("Servicing", "GetServiceLevel"))
      {
        // ISSUE: explicit non-virtual call
        HttpRequestMessage message = new HttpRequestMessage(HttpMethod.Get, new Uri(PathUtility.Combine(__nonvirtual (serviceLevelHttpClient.BaseAddress).GetLeftPart(UriPartial.Path), "/_apis/servicelevel")).AbsoluteUri);
        serviceLevelAsync = await serviceLevelHttpClient.SendAsync<ServiceLevelData>(message);
      }
      return serviceLevelAsync;
    }
  }
}
