// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Storage.Shared.Protocol.HttpResponseParsers
// Assembly: Microsoft.Azure.Storage.Common, Version=11.2.3.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 0978DA65-6954-4A99-9ACB-2EF3D979A5D5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Storage.Common.dll

using Microsoft.Azure.Storage.Core.Executor;
using Microsoft.Azure.Storage.Core.Util;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace Microsoft.Azure.Storage.Shared.Protocol
{
  internal static class HttpResponseParsers
  {
    internal static DateTime ToUTCTime(this string str) => DateTime.Parse(str, (IFormatProvider) DateTimeFormatInfo.InvariantInfo, DateTimeStyles.AdjustToUniversal);

    internal static T ProcessExpectedStatusCodeNoException<T>(
      HttpStatusCode expectedStatusCode,
      HttpStatusCode actualStatusCode,
      T retVal,
      StorageCommandBase<T> cmd,
      Exception ex)
    {
      if (ex != null)
        throw ex;
      if (actualStatusCode != expectedStatusCode)
        throw new StorageException(cmd.CurrentResult, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Unexpected response code, Expected:{0}, Received:{1}", (object) expectedStatusCode, (object) actualStatusCode), (Exception) null);
      return retVal;
    }

    internal static T ProcessExpectedStatusCodeNoException<T>(
      HttpStatusCode[] expectedStatusCodes,
      HttpStatusCode actualStatusCode,
      T retVal,
      StorageCommandBase<T> cmd,
      Exception ex)
    {
      if (ex != null)
        throw ex;
      if (!((IEnumerable<HttpStatusCode>) expectedStatusCodes).Contains<HttpStatusCode>(actualStatusCode))
      {
        string str = string.Join<HttpStatusCode>(",", (IEnumerable<HttpStatusCode>) expectedStatusCodes);
        throw new StorageException(cmd.CurrentResult, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Unexpected response code, Expected:{0}, Received:{1}", (object) str, (object) actualStatusCode.ToString()), (Exception) null);
      }
      return retVal;
    }

    internal static void ValidateResponseStreamChecksumAndLength<T>(
      long? length,
      string md5,
      string crc64,
      StorageCommandBase<T> cmd)
    {
      if (cmd.StreamCopyState == null)
        throw new StorageException(cmd.CurrentResult, "The operation requires a response body but no data was copied to the destination buffer.", (Exception) null)
        {
          IsRetryable = false
        };
      if (length.HasValue && cmd.StreamCopyState.Length != length.Value)
        throw new StorageException(cmd.CurrentResult, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Incorrect number of bytes received. Expected '{0}', received '{1}'", (object) length, (object) cmd.StreamCopyState.Length), (Exception) null)
        {
          IsRetryable = false
        };
      if (md5 != null && cmd.StreamCopyState.Md5 != null && cmd.StreamCopyState.Md5 != md5)
        throw new StorageException(cmd.CurrentResult, "Calculated MD5 does not match existing property", (Exception) null)
        {
          IsRetryable = false
        };
      if (crc64 != null && cmd.StreamCopyState.Crc64 != null && cmd.StreamCopyState.Crc64 != crc64)
        throw new StorageException(cmd.CurrentResult, "Calculated CRC64 does not match existing property", (Exception) null)
        {
          IsRetryable = false
        };
    }

    internal static AccountProperties ReadAccountProperties(HttpResponseMessage response) => AccountProperties.FromHttpResponseHeaders(response.Headers);

    internal static Task<ServiceProperties> ReadServicePropertiesAsync(
      Stream inputStream,
      CancellationToken token)
    {
      return Task.Run<ServiceProperties>((Func<ServiceProperties>) (() =>
      {
        using (XmlReader reader = XmlReader.Create(inputStream))
          return ServiceProperties.FromServiceXml(XDocument.Load(reader));
      }), token);
    }

    internal static Task<ServiceStats> ReadServiceStatsAsync(
      Stream inputStream,
      CancellationToken token)
    {
      return Task.Run<ServiceStats>((Func<ServiceStats>) (() =>
      {
        using (XmlReader reader = XmlReader.Create(inputStream))
          return ServiceStats.FromServiceXml(XDocument.Load(reader));
      }), token);
    }

    internal static T ProcessExpectedStatusCodeNoException<T>(
      HttpStatusCode expectedStatusCode,
      HttpResponseMessage resp,
      T retVal,
      StorageCommandBase<T> cmd,
      Exception ex)
    {
      return HttpResponseParsers.ProcessExpectedStatusCodeNoException<T>(expectedStatusCode, resp != null ? resp.StatusCode : HttpStatusCode.Unused, retVal, cmd, ex);
    }

    internal static T ProcessExpectedStatusCodeNoException<T>(
      HttpStatusCode[] expectedStatusCodes,
      HttpResponseMessage resp,
      T retVal,
      StorageCommandBase<T> cmd,
      Exception ex)
    {
      return HttpResponseParsers.ProcessExpectedStatusCodeNoException<T>(expectedStatusCodes, resp != null ? resp.StatusCode : HttpStatusCode.Unused, retVal, cmd, ex);
    }

    internal static string GetETag(HttpResponseMessage response) => response.Headers.ETag == null ? (string) null : response.Headers.ETag.ToString();

    internal static bool ParseServerRequestEncrypted(HttpResponseMessage response) => string.Equals(response.Headers.GetHeaderSingleValueOrDefault("x-ms-request-server-encrypted"), "true", StringComparison.OrdinalIgnoreCase);

    internal static string ParseEncryptionKeySHA256(HttpResponseMessage response) => response.Headers.GetHeaderSingleValueOrDefault("x-ms-encryption-key-sha256");

    internal static bool ParseServiceEncrypted(HttpResponseMessage response) => string.Equals(response.Headers.GetHeaderSingleValueOrDefault("x-ms-server-encrypted"), "true", StringComparison.OrdinalIgnoreCase);

    internal static string ParseEncryptionScope(HttpResponseMessage response) => response.Headers.GetHeaderSingleValueOrDefault("x-ms-encryption-scope");

    private static IDictionary<string, string> GetMetadataOrProperties(
      HttpResponseMessage response,
      string prefix)
    {
      IDictionary<string, string> metadataOrProperties = prefix == "x-ms-meta-" ? (IDictionary<string, string>) new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) : (IDictionary<string, string>) new Dictionary<string, string>();
      HttpResponseHeaders headers = response.Headers;
      int length = prefix.Length;
      foreach (KeyValuePair<string, IEnumerable<string>> keyValuePair in (HttpHeaders) headers)
      {
        if (keyValuePair.Key.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
          metadataOrProperties[keyValuePair.Key.Substring(length)] = string.Join(",", keyValuePair.Value);
      }
      return metadataOrProperties;
    }

    internal static string GetContentType(HttpResponseMessage response) => response.Content.Headers.ContentType == null ? (string) null : response.Content.Headers.ContentType.ToString();

    internal static string GetContentRange(HttpResponseMessage response) => response.Content.Headers.ContentRange == null ? (string) null : response.Content.Headers.ContentRange.ToString();

    internal static string GetContentMD5(HttpResponseMessage response) => response.Content.Headers.ContentMD5 == null ? (string) null : Convert.ToBase64String(response.Content.Headers.ContentMD5);

    internal static string GetContentCRC64(HttpResponseMessage response) => HttpResponseParsers.GetHeader(response, "x-ms-content-crc64");

    internal static string GetContentLocation(HttpResponseMessage response) => !(response.Content.Headers.ContentLocation != (Uri) null) ? (string) null : response.Content.Headers.ContentLocation.ToString();

    internal static string GetContentLength(HttpResponseMessage response) => !response.Content.Headers.ContentLength.HasValue ? (string) null : response.Content.Headers.ContentLength.ToString();

    internal static string GetContentLanguage(HttpResponseMessage response) => response.Content.Headers.ContentLanguage == null ? (string) null : response.Content.Headers.ContentLanguage.ToString();

    internal static string GetContentEncoding(HttpResponseMessage response) => response.Content.Headers.ContentEncoding == null ? (string) null : response.Content.Headers.ContentEncoding.ToString();

    internal static string GetContentDisposition(HttpResponseMessage response) => response.Content.Headers.ContentDisposition == null ? (string) null : response.Content.Headers.ContentDisposition.ToString();

    internal static string GetCacheControl(HttpResponseMessage response) => response.Headers.CacheControl == null ? (string) null : response.Headers.CacheControl.ToString();

    internal static string GetHeader(HttpResponseMessage response, string headerName)
    {
      if (response.Content != null && response.Content.Headers.Contains(headerName))
        return response.Content.Headers.GetValues(headerName).First<string>();
      return response.Headers.Contains(headerName) ? response.Headers.GetValues(headerName).First<string>() : (string) null;
    }

    internal static List<string> GetAllHeaders(HttpResponseMessage response) => response.Headers.Concat<KeyValuePair<string, IEnumerable<string>>>((IEnumerable<KeyValuePair<string, IEnumerable<string>>>) response.Content.Headers).Select<KeyValuePair<string, IEnumerable<string>>, string>((Func<KeyValuePair<string, IEnumerable<string>>, string>) (r => r.Key)).ToList<string>();

    internal static DateTimeOffset GetLastModifiedTime(HttpResponseMessage response) => response.Content.Headers.LastModified.GetValueOrDefault();

    internal static Stream GetResponseStream(HttpResponseMessage response) => response.Content.ReadAsStreamAsync().GetAwaiter().GetResult();

    internal static IDictionary<string, string> GetMetadata(HttpResponseMessage response) => HttpResponseParsers.GetMetadataOrProperties(response, "x-ms-meta-");
  }
}
