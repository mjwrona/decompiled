// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Table.RestExecutor.TableCommand.HttpClientFactory
// Assembly: Microsoft.Azure.Cosmos.Table, Version=1.0.7.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 461D0B3A-0B96-4D42-B330-3A8E714FC39A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Table.dll

using Microsoft.Azure.Cosmos.Table.RestExecutor.Common.Auth;
using System;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Microsoft.Azure.Cosmos.Table.RestExecutor.TableCommand
{
  internal static class HttpClientFactory
  {
    private static Lazy<HttpClient> instance = new Lazy<HttpClient>((Func<HttpClient>) (() => HttpClientFactory.BuildHttpClient((HttpMessageHandler) StorageAuthenticationHttpHandler.Instance, new TimeSpan?())));

    private static HttpClient BuildHttpClient(
      HttpMessageHandler httpMessageHandler,
      TimeSpan? httpClientTimeout)
    {
      HttpClient httpClient = new HttpClient(httpMessageHandler, false);
      httpClient.DefaultRequestHeaders.ExpectContinue = new bool?(false);
      httpClient.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("Azure-Cosmos-Table", "1.0.7"));
      httpClient.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue(TableRestConstants.HeaderConstants.UserAgentComment));
      httpClient.DefaultRequestHeaders.TryAddWithoutValidation("x-ms-version", "2017-07-29");
      httpClient.Timeout = httpClientTimeout ?? TableRestConstants.DefaultHttpClientTimeout;
      return httpClient;
    }

    public static HttpClient Instance => HttpClientFactory.instance.Value;

    internal static HttpClient HttpClientFromConfiguration(RestExecutorConfiguration configuration)
    {
      if (configuration == null)
        return (HttpClient) null;
      if (configuration.DelegatingHandler == null)
        return HttpClientFactory.BuildHttpClient((HttpMessageHandler) StorageAuthenticationHttpHandler.Instance, configuration.HttpClientTimeout);
      DelegatingHandler delegatingHandler1 = configuration.DelegatingHandler;
      DelegatingHandler delegatingHandler2;
      for (delegatingHandler2 = delegatingHandler1; delegatingHandler2.InnerHandler != null; delegatingHandler2 = innerHandler)
      {
        if (!(delegatingHandler2.InnerHandler is DelegatingHandler innerHandler))
          throw new ArgumentException("Innermost DelegatingHandler must have a null InnerHandler.");
      }
      delegatingHandler2.InnerHandler = (HttpMessageHandler) new StorageAuthenticationHttpHandler();
      return HttpClientFactory.BuildHttpClient((HttpMessageHandler) delegatingHandler1, configuration.HttpClientTimeout);
    }
  }
}
