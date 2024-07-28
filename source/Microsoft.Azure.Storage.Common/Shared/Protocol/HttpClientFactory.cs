// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Storage.Shared.Protocol.HttpClientFactory
// Assembly: Microsoft.Azure.Storage.Common, Version=11.2.3.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 0978DA65-6954-4A99-9ACB-2EF3D979A5D5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Storage.Common.dll

using Microsoft.Azure.Storage.Auth.Protocol;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;

namespace Microsoft.Azure.Storage.Shared.Protocol
{
  internal static class HttpClientFactory
  {
    private static Lazy<HttpClient> instance = new Lazy<HttpClient>((Func<HttpClient>) (() => HttpClientFactory.BuildHttpClient((HttpMessageHandler) StorageAuthenticationHttpHandler.Instance)));

    public static HttpClient Instance => HttpClientFactory.instance.Value;

    internal static HttpClient HttpClientFromDelegatingHandler(DelegatingHandler delegatingHandler)
    {
      if (delegatingHandler == null)
        return (HttpClient) null;
      DelegatingHandler delegatingHandler1;
      HttpMessageHandler innerHandler;
      for (delegatingHandler1 = delegatingHandler; delegatingHandler1.InnerHandler != null; delegatingHandler1 = (DelegatingHandler) innerHandler)
      {
        innerHandler = delegatingHandler1.InnerHandler;
        if (!(innerHandler is DelegatingHandler))
          throw new ArgumentException("Innermost DelegatingHandler must have a null InnerHandler.");
      }
      delegatingHandler1.InnerHandler = (HttpMessageHandler) new StorageAuthenticationHttpHandler();
      return HttpClientFactory.BuildHttpClient((HttpMessageHandler) delegatingHandler);
    }

    private static HttpClient BuildHttpClient(HttpMessageHandler httpMessageHandler)
    {
      HttpClient httpClient = new HttpClient(httpMessageHandler, false);
      httpClient.DefaultRequestHeaders.ExpectContinue = new bool?(false);
      httpClient.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("Azure-Storage", "11.2.3"));
      httpClient.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue(Constants.HeaderConstants.UserAgentComment));
      httpClient.DefaultRequestHeaders.TryAddWithoutValidation("x-ms-version", "2019-07-07");
      httpClient.Timeout = Timeout.InfiniteTimeSpan;
      return httpClient;
    }
  }
}
