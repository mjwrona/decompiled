// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.HttpClientExtension
// Assembly: Microsoft.Azure.Cosmos.Direct, Version=3.29.4.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: FFE3C00D-4333-4294-8947-B1C93A852E2F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Direct.dll

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

    internal static void AddSDKSupportedCapabilitiesHeader(
      this HttpClient httpClient,
      ulong capabilities)
    {
      httpClient.DefaultRequestHeaders.Add("x-ms-cosmos-sdk-supportedcapabilities", capabilities.ToString());
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
        throw ServiceUnavailableException.Create(new SubStatusCodes?(SubStatusCodes.Unknown), (Exception) ex);
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
        throw ServiceUnavailableException.Create(new SubStatusCodes?(SubStatusCodes.Unknown), (Exception) ex);
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
        throw ServiceUnavailableException.Create(new SubStatusCodes?(SubStatusCodes.Unknown), (Exception) ex);
      }
    }
  }
}
