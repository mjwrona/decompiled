// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Storage.Shared.Protocol.HttpRequestHandler
// Assembly: Microsoft.Azure.Storage.Common, Version=11.2.3.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 0978DA65-6954-4A99-9ACB-2EF3D979A5D5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Storage.Common.dll

using System;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Microsoft.Azure.Storage.Shared.Protocol
{
  internal static class HttpRequestHandler
  {
    internal static void SetExpect100Continue(HttpRequestMessage request, bool value) => request.Headers.ExpectContinue = new bool?(value);

    internal static void SetKeepAlive(HttpRequestMessage request, bool alive)
    {
      if (alive)
        request.Headers.Connection.Add("Keep-Alive");
      else
        request.Headers.Connection.Remove("Keep-Alive");
    }

    internal static void SetAccept(HttpRequestMessage request, string accept) => request.Headers.Accept.ParseAdd(accept);

    internal static void SetContentLength(HttpRequestMessage request, long contentLength) => HttpRequestHandler.SetHeader(request, "Content-Length", contentLength.ToString());

    internal static void SetContentType(HttpRequestMessage request, string contentType) => HttpRequestHandler.SetHeader(request, "Content-Type", contentType);

    internal static void SetCacheControl(HttpRequestMessage request, string cacheControl) => HttpRequestHandler.SetHeader(request, "Cache-Control", cacheControl);

    internal static void SetContentMd5(HttpRequestMessage request, string cacheControl) => HttpRequestHandler.SetHeader(request, "Content-MD5", cacheControl);

    internal static void SetContentCrc64(HttpRequestMessage request, string cacheControl) => HttpRequestHandler.SetHeader(request, "Content-CRC64", cacheControl);

    internal static void SetContentLanguage(HttpRequestMessage request, string cacheControl) => HttpRequestHandler.SetHeader(request, "Content-Language", cacheControl);

    internal static void SetContentEncoding(HttpRequestMessage request, string cacheControl) => HttpRequestHandler.SetHeader(request, "Content-Encoding", cacheControl);

    internal static void SetIfMatch(HttpRequestMessage request, string ifMatchETag) => request.Headers.IfMatch.Add(new EntityTagHeaderValue(ifMatchETag));

    internal static void SetIfNoneMatch(HttpRequestMessage request, string ifNoneMatch) => request.Headers.IfNoneMatch.Add(new EntityTagHeaderValue(ifNoneMatch));

    internal static void SetIfModifiedSince(
      HttpRequestMessage request,
      DateTimeOffset ifModifiedSince)
    {
      request.Headers.IfModifiedSince = new DateTimeOffset?(ifModifiedSince);
    }

    internal static void SetIfUnModifiedSince(
      HttpRequestMessage request,
      DateTimeOffset ifUnModifiedSince)
    {
      request.Headers.IfUnmodifiedSince = new DateTimeOffset?(ifUnModifiedSince);
    }

    internal static void SetUserAgent(HttpRequestMessage request, string userAgent) => request.Headers.UserAgent.ParseAdd(userAgent);

    internal static void SetHeader(
      HttpRequestMessage request,
      string headerKey,
      string headerValue)
    {
      request.Headers.TryAddWithoutValidation(headerKey, headerValue);
    }
  }
}
