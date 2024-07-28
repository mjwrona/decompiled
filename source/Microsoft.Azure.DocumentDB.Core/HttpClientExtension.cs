// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.HttpClientExtension
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Documents
{
  internal static class HttpClientExtension
  {
    internal static void AddUserAgentHeader(
      this HttpClient httpClient,
      UserAgentContainer userAgent)
    {
      httpClient.DefaultRequestHeaders.Add("User-Agent", userAgent.UserAgent);
    }

    internal static void AddApiTypeHeader(this HttpClient httpClient, ApiType apitype)
    {
      if (apitype.Equals((object) ApiType.None))
        return;
      httpClient.DefaultRequestHeaders.Add("x-ms-cosmos-apitype", apitype.ToString());
    }

    internal static Task<HttpResponseMessage> SendHttpAsync(
      this HttpClient httpClient,
      HttpRequestMessage requestMessage,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      try
      {
        return httpClient.SendAsync(requestMessage, cancellationToken);
      }
      catch (HttpRequestException ex)
      {
        throw new ServiceUnavailableException((Exception) ex);
      }
    }

    internal static Task<HttpResponseMessage> SendHttpAsync(
      this HttpClient httpClient,
      HttpRequestMessage requestMessage,
      HttpCompletionOption options,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      try
      {
        return httpClient.SendAsync(requestMessage, options, cancellationToken);
      }
      catch (HttpRequestException ex)
      {
        throw new ServiceUnavailableException((Exception) ex);
      }
    }

    internal static Task<HttpResponseMessage> GetHttpAsync(
      this HttpClient httpClient,
      Uri serviceEndpoint,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      try
      {
        return httpClient.GetAsync(serviceEndpoint, cancellationToken);
      }
      catch (HttpRequestException ex)
      {
        throw new ServiceUnavailableException((Exception) ex);
      }
    }
  }
}
