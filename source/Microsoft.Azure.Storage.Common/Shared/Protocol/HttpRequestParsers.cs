// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Storage.Shared.Protocol.HttpRequestParsers
// Assembly: Microsoft.Azure.Storage.Common, Version=11.2.3.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 0978DA65-6954-4A99-9ACB-2EF3D979A5D5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Storage.Common.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;

namespace Microsoft.Azure.Storage.Shared.Protocol
{
  internal static class HttpRequestParsers
  {
    internal static string GetAuthorization(HttpRequestMessage request) => request.Headers.Authorization == null ? (string) null : request.Headers.Authorization.ToString();

    internal static string GetDate(HttpRequestMessage request) => !request.Headers.Date.HasValue ? (string) null : request.Headers.Date.ToString();

    internal static string GetContentType(HttpRequestMessage request) => request.Content.Headers.ContentType == null ? (string) null : request.Content.Headers.ContentType.ToString();

    internal static string GetContentRange(HttpRequestMessage request) => request.Content.Headers.ContentRange == null ? (string) null : request.Content.Headers.ContentRange.ToString();

    internal static string GetContentMD5(HttpRequestMessage request) => request.Content.Headers.ContentMD5 == null ? (string) null : Convert.ToBase64String(request.Content.Headers.ContentMD5);

    internal static string GetContentCRC64(HttpRequestMessage request) => HttpRequestParsers.GetHeader(request, "x-ms-content-crc64");

    internal static string GetContentLocation(HttpRequestMessage request) => !(request.Content.Headers.ContentLocation != (Uri) null) ? (string) null : request.Content.Headers.ContentLocation.ToString();

    internal static string GetContentLength(HttpRequestMessage request) => !request.Content.Headers.ContentLength.HasValue ? (string) null : request.Content.Headers.ContentLength.ToString();

    internal static string GetContentLanguage(HttpRequestMessage request) => request.Content.Headers.ContentLanguage == null ? (string) null : request.Content.Headers.ContentLanguage.ToString();

    internal static string GetContentEncoding(HttpRequestMessage request) => request.Content.Headers.ContentEncoding == null ? (string) null : request.Content.Headers.ContentEncoding.ToString();

    internal static string GetContentDisposition(HttpRequestMessage request) => request.Content.Headers.ContentDisposition == null ? (string) null : request.Content.Headers.ContentDisposition.ToString();

    internal static string GetIfMatch(HttpRequestMessage request) => request.Headers.IfMatch == null ? (string) null : request.Headers.IfMatch.ToString();

    internal static string GetIfNoneMatch(HttpRequestMessage request) => request.Headers.IfNoneMatch == null ? (string) null : request.Headers.IfNoneMatch.ToString();

    internal static string GetIfModifiedSince(HttpRequestMessage request) => !request.Headers.IfModifiedSince.HasValue ? (string) null : request.Headers.IfModifiedSince.ToString();

    internal static string GetIfUnModifiedSince(HttpRequestMessage request) => !request.Headers.IfUnmodifiedSince.HasValue ? (string) null : request.Headers.IfUnmodifiedSince.ToString();

    internal static string GetCacheControl(HttpRequestMessage request) => request.Headers.CacheControl == null ? (string) null : request.Headers.CacheControl.ToString();

    internal static List<string> GetAllHeaders(HttpRequestMessage request) => request.Content != null ? request.Headers.Concat<KeyValuePair<string, IEnumerable<string>>>((IEnumerable<KeyValuePair<string, IEnumerable<string>>>) request.Content.Headers).Select<KeyValuePair<string, IEnumerable<string>>, string>((Func<KeyValuePair<string, IEnumerable<string>>, string>) (r => r.Key)).ToList<string>() : request.Headers.Select<KeyValuePair<string, IEnumerable<string>>, string>((Func<KeyValuePair<string, IEnumerable<string>>, string>) (r => r.Key)).ToList<string>();

    internal static string GetHeader(HttpRequestMessage request, string headerName)
    {
      if (request.Content != null && request.Content.Headers.Contains(headerName))
        return request.Content.Headers.GetValues(headerName).First<string>();
      return request.Headers.Contains(headerName) ? request.Headers.GetValues(headerName).First<string>() : (string) null;
    }
  }
}
