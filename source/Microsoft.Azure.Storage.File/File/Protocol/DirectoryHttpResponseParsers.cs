// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Storage.File.Protocol.DirectoryHttpResponseParsers
// Assembly: Microsoft.Azure.Storage.File, Version=11.2.3.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: C68E95B0-8DFB-410C-8E70-706406D1A279
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Storage.File.dll

using Microsoft.Azure.Storage.Core.Util;
using Microsoft.Azure.Storage.Shared.Protocol;
using System;
using System.Collections.Generic;
using System.Net.Http;

namespace Microsoft.Azure.Storage.File.Protocol
{
  public static class DirectoryHttpResponseParsers
  {
    public static FileDirectoryProperties GetProperties(HttpResponseMessage response)
    {
      CommonUtility.AssertNotNull(nameof (response), (object) response);
      FileDirectoryProperties properties = new FileDirectoryProperties();
      properties.ETag = response.Headers.ETag == null ? (string) null : response.Headers.ETag.ToString();
      string singleValueOrDefault = response.Headers.GetHeaderSingleValueOrDefault("x-ms-server-encrypted");
      properties.IsServerEncrypted = string.Equals(singleValueOrDefault, "true", StringComparison.OrdinalIgnoreCase);
      properties.LastModified = response.Content == null ? new DateTimeOffset?() : response.Content.Headers.LastModified;
      return properties;
    }

    public static void UpdateSmbProperties(
      HttpResponseMessage response,
      FileDirectoryProperties properties)
    {
      properties.filePermissionKey = HttpResponseParsers.GetHeader(response, "x-ms-file-permission-key");
      properties.ntfsAttributes = CloudFileNtfsAttributesHelper.ToAttributes(HttpResponseParsers.GetHeader(response, "x-ms-file-attributes"));
      properties.creationTime = new DateTimeOffset?(DateTimeOffset.Parse(HttpResponseParsers.GetHeader(response, "x-ms-file-creation-time")));
      properties.lastWriteTime = new DateTimeOffset?(DateTimeOffset.Parse(HttpResponseParsers.GetHeader(response, "x-ms-file-last-write-time")));
      properties.ChangeTime = new DateTimeOffset?(DateTimeOffset.Parse(HttpResponseParsers.GetHeader(response, "x-ms-file-change-time")));
      properties.DirectoryId = HttpResponseParsers.GetHeader(response, "x-ms-file-id");
      properties.ParentId = HttpResponseParsers.GetHeader(response, "x-ms-file-parent-id");
      properties.filePermissionKeyToSet = (string) null;
      properties.ntfsAttributesToSet = new CloudFileNtfsAttributes?();
      properties.creationTimeToSet = new DateTimeOffset?();
      properties.lastWriteTimeToSet = new DateTimeOffset?();
    }

    public static IDictionary<string, string> GetMetadata(HttpResponseMessage response) => HttpResponseParsers.GetMetadata(response);
  }
}
