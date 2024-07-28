// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Client.HttpClientFactoryWithUserAgent
// Assembly: Microsoft.VisualStudio.Services.Client.Interactive, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 00B1FD41-439C-4B93-A417-9D1E4874E657
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Client.Interactive.dll

using Microsoft.Identity.Client;
using System;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;

namespace Microsoft.VisualStudio.Services.Client
{
  internal class HttpClientFactoryWithUserAgent : IMsalHttpClientFactory
  {
    private const string UserAgentPrefix = "AzureDevops.InteractiveClient";
    private static readonly Lazy<HttpClient> httpClient = new Lazy<HttpClient>((Func<HttpClient>) (() => HttpClientFactoryWithUserAgent.CreateClientWithCustomUserAgent()));

    private static HttpClient CreateClientWithCustomUserAgent() => new HttpClient()
    {
      DefaultRequestHeaders = {
        UserAgent = {
          new ProductInfoHeaderValue("AzureDevops.InteractiveClient", FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).ProductVersion)
        }
      }
    };

    public HttpClient GetHttpClient() => HttpClientFactoryWithUserAgent.httpClient.Value;
  }
}
