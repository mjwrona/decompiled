// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Storage.File.Protocol.FileHttpResponseParsers
// Assembly: Microsoft.Azure.Storage.File, Version=11.2.3.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: C68E95B0-8DFB-410C-8E70-706406D1A279
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Storage.File.dll

using Microsoft.Azure.Storage.Core.Util;
using Microsoft.Azure.Storage.Shared.Protocol;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace Microsoft.Azure.Storage.File.Protocol
{
  public static class FileHttpResponseParsers
  {
    public static FileProperties GetProperties(HttpResponseMessage response)
    {
      CommonUtility.AssertNotNull(nameof (response), (object) response);
      FileProperties properties = new FileProperties();
      if (response.Content != null)
      {
        properties.LastModified = response.Content.Headers.LastModified;
        HttpContentHeaders headers = response.Content.Headers;
        properties.ContentEncoding = HttpWebUtility.GetHeaderValues("Content-Encoding", (HttpHeaders) headers);
        properties.ContentLanguage = HttpWebUtility.GetHeaderValues("Content-Language", (HttpHeaders) headers);
        properties.ContentDisposition = HttpWebUtility.GetHeaderValues("Content-Disposition", (HttpHeaders) headers);
        properties.ContentType = HttpWebUtility.GetHeaderValues("Content-Type", (HttpHeaders) headers);
        if (response.Content.Headers.ContentMD5 != null && response.Content.Headers.ContentRange == null)
          properties.ContentChecksum.MD5 = Convert.ToBase64String(response.Content.Headers.ContentMD5);
        else if (!string.IsNullOrEmpty(response.Headers.GetHeaderSingleValueOrDefault("x-ms-content-md5")))
          properties.ContentChecksum.MD5 = response.Headers.GetHeaderSingleValueOrDefault("x-ms-content-md5");
        if (!string.IsNullOrEmpty(response.Headers.GetHeaderSingleValueOrDefault("x-ms-content-crc64")))
          properties.ContentChecksum.CRC64 = response.Headers.GetHeaderSingleValueOrDefault("x-ms-content-crc64");
        string singleValueOrDefault1 = response.Headers.GetHeaderSingleValueOrDefault("x-ms-server-encrypted");
        properties.IsServerEncrypted = string.Equals(singleValueOrDefault1, "true", StringComparison.OrdinalIgnoreCase);
        string singleValueOrDefault2 = response.Headers.GetHeaderSingleValueOrDefault("x-ms-content-length");
        if (response.Content.Headers.ContentRange != null && response.Content.Headers.ContentRange.HasLength)
          properties.Length = response.Content.Headers.ContentRange.Length.Value;
        else if (!string.IsNullOrEmpty(singleValueOrDefault2))
          properties.Length = long.Parse(singleValueOrDefault2);
        else if (response.Content.Headers.ContentLength.HasValue)
          properties.Length = response.Content.Headers.ContentLength.Value;
      }
      properties.CacheControl = HttpWebUtility.GetHeaderValues("Cache-Control", (HttpHeaders) response.Headers);
      if (response.Headers.ETag != null)
        properties.ETag = response.Headers.ETag.ToString();
      return properties;
    }

    public static void UpdateSmbProperties(HttpResponseMessage response, FileProperties properties)
    {
      properties.filePermissionKey = HttpResponseParsers.GetHeader(response, "x-ms-file-permission-key");
      properties.ntfsAttributes = CloudFileNtfsAttributesHelper.ToAttributes(HttpResponseParsers.GetHeader(response, "x-ms-file-attributes"));
      properties.creationTime = new DateTimeOffset?(DateTimeOffset.Parse(HttpResponseParsers.GetHeader(response, "x-ms-file-creation-time")));
      properties.lastWriteTime = new DateTimeOffset?(DateTimeOffset.Parse(HttpResponseParsers.GetHeader(response, "x-ms-file-last-write-time")));
      properties.ChangeTime = new DateTimeOffset?(DateTimeOffset.Parse(HttpResponseParsers.GetHeader(response, "x-ms-file-change-time")));
      properties.FileId = HttpResponseParsers.GetHeader(response, "x-ms-file-id");
      properties.ParentId = HttpResponseParsers.GetHeader(response, "x-ms-file-parent-id");
      properties.filePermissionKeyToSet = (string) null;
      properties.ntfsAttributesToSet = new CloudFileNtfsAttributes?();
      properties.creationTimeToSet = new DateTimeOffset?();
      properties.lastWriteTimeToSet = new DateTimeOffset?();
    }

    public static IDictionary<string, string> GetMetadata(HttpResponseMessage response) => HttpResponseParsers.GetMetadata(response);

    internal static CopyState GetCopyAttributes(
      string copyStatusString,
      string copyId,
      string copySourceString,
      string copyProgressString,
      string copyCompletionTimeString,
      string copyStatusDescription,
      string copyDestinationSnapshotTimeString)
    {
      CopyState copyAttributes = new CopyState()
      {
        CopyId = copyId,
        StatusDescription = copyStatusDescription
      };
      switch (copyStatusString)
      {
        case "success":
          copyAttributes.Status = CopyStatus.Success;
          break;
        case "pending":
          copyAttributes.Status = CopyStatus.Pending;
          break;
        case "aborted":
          copyAttributes.Status = CopyStatus.Aborted;
          break;
        case "failed":
          copyAttributes.Status = CopyStatus.Failed;
          break;
        default:
          copyAttributes.Status = CopyStatus.Invalid;
          break;
      }
      if (!string.IsNullOrEmpty(copyProgressString))
      {
        string[] strArray = copyProgressString.Split('/');
        copyAttributes.BytesCopied = new long?(long.Parse(strArray[0], (IFormatProvider) CultureInfo.InvariantCulture));
        copyAttributes.TotalBytes = new long?(long.Parse(strArray[1], (IFormatProvider) CultureInfo.InvariantCulture));
      }
      if (!string.IsNullOrEmpty(copySourceString))
        copyAttributes.Source = new Uri(copySourceString);
      if (!string.IsNullOrEmpty(copyCompletionTimeString))
        copyAttributes.CompletionTime = new DateTimeOffset?((DateTimeOffset) copyCompletionTimeString.ToUTCTime());
      if (!string.IsNullOrEmpty(copyDestinationSnapshotTimeString))
        copyAttributes.DestinationSnapshotTime = new DateTimeOffset?((DateTimeOffset) copyDestinationSnapshotTimeString.ToUTCTime());
      return copyAttributes;
    }

    public static CopyState GetCopyAttributes(HttpResponseMessage response)
    {
      string singleValueOrDefault = response.Headers.GetHeaderSingleValueOrDefault("x-ms-copy-status");
      return !string.IsNullOrEmpty(singleValueOrDefault) ? FileHttpResponseParsers.GetCopyAttributes(singleValueOrDefault, response.Headers.GetHeaderSingleValueOrDefault("x-ms-copy-id"), response.Headers.GetHeaderSingleValueOrDefault("x-ms-copy-source"), response.Headers.GetHeaderSingleValueOrDefault("x-ms-copy-progress"), response.Headers.GetHeaderSingleValueOrDefault("x-ms-copy-completion-time"), response.Headers.GetHeaderSingleValueOrDefault("x-ms-copy-status-description"), response.Headers.GetHeaderSingleValueOrDefault("x-ms-copy-destination-snapshot")) : (CopyState) null;
    }

    public static Task<FileServiceProperties> ReadServicePropertiesAsync(
      Stream inputStream,
      CancellationToken token)
    {
      return Task.Run<FileServiceProperties>((Func<FileServiceProperties>) (() =>
      {
        using (XmlReader reader = XmlReader.Create(inputStream))
          return FileServiceProperties.FromServiceXml(XDocument.Load(reader));
      }), token);
    }

    public static Task<ServiceStats> ReadServiceStatsAsync(
      Stream inputStream,
      CancellationToken token)
    {
      return HttpResponseParsers.ReadServiceStatsAsync(inputStream, token);
    }
  }
}
