// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Storage.File.Protocol.DirectoryHttpRequestMessageFactory
// Assembly: Microsoft.Azure.Storage.File, Version=11.2.3.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: C68E95B0-8DFB-410C-8E70-706406D1A279
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Storage.File.dll

using Microsoft.Azure.Storage.Auth;
using Microsoft.Azure.Storage.Core;
using Microsoft.Azure.Storage.Core.Auth;
using Microsoft.Azure.Storage.Core.Util;
using Microsoft.Azure.Storage.Shared.Protocol;
using System;
using System.Collections.Generic;
using System.Net.Http;

namespace Microsoft.Azure.Storage.File.Protocol
{
  internal static class DirectoryHttpRequestMessageFactory
  {
    public static StorageRequestMessage Create(
      Uri uri,
      int? timeout,
      FileDirectoryProperties properties,
      string filePermissionToSet,
      HttpContent content,
      OperationContext operationContext,
      ICanonicalizer canonicalizer,
      StorageCredentials credentials)
    {
      UriQueryBuilder directoryUriQueryBuilder = DirectoryHttpRequestMessageFactory.GetDirectoryUriQueryBuilder();
      StorageRequestMessage request = HttpRequestMessageFactory.Create(uri, timeout, directoryUriQueryBuilder, content, operationContext, canonicalizer, credentials);
      DirectoryHttpRequestMessageFactory.AddFilePermissionOrFilePermissionKey(request, filePermissionToSet, properties, "Inherit");
      DirectoryHttpRequestMessageFactory.AddNtfsFileAttributes(request, properties, "None");
      DirectoryHttpRequestMessageFactory.AddCreationTime(request, properties, "Now");
      DirectoryHttpRequestMessageFactory.AddLastWriteTime(request, properties, "Now");
      return request;
    }

    public static StorageRequestMessage Delete(
      Uri uri,
      int? timeout,
      AccessCondition accessCondition,
      HttpContent content,
      OperationContext operationContext,
      ICanonicalizer canonicalizer,
      StorageCredentials credentials)
    {
      UriQueryBuilder directoryUriQueryBuilder = DirectoryHttpRequestMessageFactory.GetDirectoryUriQueryBuilder();
      StorageRequestMessage request = HttpRequestMessageFactory.Delete(uri, timeout, directoryUriQueryBuilder, content, operationContext, canonicalizer, credentials);
      request.ApplyAccessCondition(accessCondition);
      return request;
    }

    public static StorageRequestMessage GetProperties(
      Uri uri,
      int? timeout,
      DateTimeOffset? shareSnapshot,
      AccessCondition accessCondition,
      HttpContent content,
      OperationContext operationContext,
      ICanonicalizer canonicalizer,
      StorageCredentials credentials)
    {
      UriQueryBuilder directoryUriQueryBuilder = DirectoryHttpRequestMessageFactory.GetDirectoryUriQueryBuilder();
      DirectoryHttpRequestMessageFactory.AddShareSnapshot(directoryUriQueryBuilder, shareSnapshot);
      StorageRequestMessage properties = HttpRequestMessageFactory.GetProperties(uri, timeout, directoryUriQueryBuilder, content, operationContext, canonicalizer, credentials);
      properties.ApplyAccessCondition(accessCondition);
      return properties;
    }

    public static StorageRequestMessage GetMetadata(
      Uri uri,
      int? timeout,
      DateTimeOffset? shareSnapshot,
      AccessCondition accessCondition,
      HttpContent content,
      OperationContext operationContext,
      ICanonicalizer canonicalizer,
      StorageCredentials credentials)
    {
      UriQueryBuilder directoryUriQueryBuilder = DirectoryHttpRequestMessageFactory.GetDirectoryUriQueryBuilder();
      DirectoryHttpRequestMessageFactory.AddShareSnapshot(directoryUriQueryBuilder, shareSnapshot);
      StorageRequestMessage metadata = HttpRequestMessageFactory.GetMetadata(uri, timeout, directoryUriQueryBuilder, content, operationContext, canonicalizer, credentials);
      metadata.ApplyAccessCondition(accessCondition);
      return metadata;
    }

    public static StorageRequestMessage List(
      Uri uri,
      int? timeout,
      DateTimeOffset? shareSnapshot,
      FileListingContext listingContext,
      HttpContent content,
      OperationContext operationContext,
      ICanonicalizer canonicalizer,
      StorageCredentials credentials)
    {
      UriQueryBuilder directoryUriQueryBuilder = DirectoryHttpRequestMessageFactory.GetDirectoryUriQueryBuilder();
      DirectoryHttpRequestMessageFactory.AddShareSnapshot(directoryUriQueryBuilder, shareSnapshot);
      directoryUriQueryBuilder.Add("comp", "list");
      if (listingContext != null)
      {
        if (listingContext.Marker != null)
          directoryUriQueryBuilder.Add("marker", listingContext.Marker);
        if (listingContext.MaxResults.HasValue)
          directoryUriQueryBuilder.Add("maxresults", listingContext.MaxResults.ToString());
        if (listingContext.Prefix != null)
          directoryUriQueryBuilder.Add("prefix", listingContext.Prefix);
      }
      return HttpRequestMessageFactory.CreateRequestMessage(HttpMethod.Get, uri, timeout, directoryUriQueryBuilder, content, operationContext, canonicalizer, credentials);
    }

    public static StorageRequestMessage SetMetadata(
      Uri uri,
      int? timeout,
      AccessCondition accessCondition,
      HttpContent content,
      OperationContext operationContext,
      ICanonicalizer canonicalizer,
      StorageCredentials credentials)
    {
      UriQueryBuilder directoryUriQueryBuilder = DirectoryHttpRequestMessageFactory.GetDirectoryUriQueryBuilder();
      StorageRequestMessage request = HttpRequestMessageFactory.SetMetadata(uri, timeout, directoryUriQueryBuilder, content, operationContext, canonicalizer, credentials);
      request.ApplyAccessCondition(accessCondition);
      return request;
    }

    public static StorageRequestMessage SetProperties(
      Uri uri,
      int? timeout,
      FileDirectoryProperties properties,
      string filePermissionToSet,
      HttpContent content,
      OperationContext operationContext,
      ICanonicalizer canonicalizer,
      StorageCredentials credentials)
    {
      CommonUtility.AssertNotNull(nameof (properties), (object) properties);
      UriQueryBuilder directoryUriQueryBuilder = DirectoryHttpRequestMessageFactory.GetDirectoryUriQueryBuilder();
      directoryUriQueryBuilder.Add("comp", nameof (properties));
      StorageRequestMessage requestMessage = HttpRequestMessageFactory.CreateRequestMessage(HttpMethod.Put, uri, timeout, directoryUriQueryBuilder, content, operationContext, canonicalizer, credentials);
      DirectoryHttpRequestMessageFactory.AddFilePermissionOrFilePermissionKey(requestMessage, filePermissionToSet, properties, "Preserve");
      DirectoryHttpRequestMessageFactory.AddNtfsFileAttributes(requestMessage, properties, "Preserve");
      DirectoryHttpRequestMessageFactory.AddCreationTime(requestMessage, properties, "Preserve");
      DirectoryHttpRequestMessageFactory.AddLastWriteTime(requestMessage, properties, "Preserve");
      return requestMessage;
    }

    public static void AddMetadata(
      StorageRequestMessage request,
      IDictionary<string, string> metadata)
    {
      HttpRequestMessageFactory.AddMetadata(request, metadata);
    }

    private static void AddShareSnapshot(UriQueryBuilder builder, DateTimeOffset? snapshot)
    {
      if (!snapshot.HasValue)
        return;
      builder.Add("sharesnapshot", Request.ConvertDateTimeToSnapshotString(snapshot.Value));
    }

    internal static UriQueryBuilder GetDirectoryUriQueryBuilder()
    {
      UriQueryBuilder directoryUriQueryBuilder = new UriQueryBuilder();
      directoryUriQueryBuilder.Add("restype", "directory");
      return directoryUriQueryBuilder;
    }

    private static void AddFilePermissionOrFilePermissionKey(
      StorageRequestMessage request,
      string filePermissionToSet,
      FileDirectoryProperties properties,
      string defaultValue)
    {
      if (filePermissionToSet == null && (properties == null || properties.filePermissionKeyToSet == null))
        request.AddOptionalHeader("x-ms-file-permission", defaultValue);
      else if (filePermissionToSet != null)
        request.AddOptionalHeader("x-ms-file-permission", filePermissionToSet);
      else
        request.AddOptionalHeader("x-ms-file-permission-key", properties.filePermissionKeyToSet);
    }

    private static void AddNtfsFileAttributes(
      StorageRequestMessage request,
      FileDirectoryProperties properties,
      string defaultValue)
    {
      if (properties != null && properties.ntfsAttributesToSet.HasValue)
        request.AddOptionalHeader("x-ms-file-attributes", CloudFileNtfsAttributesHelper.ToString(properties.ntfsAttributesToSet.Value));
      else
        request.AddOptionalHeader("x-ms-file-attributes", defaultValue);
    }

    private static void AddCreationTime(
      StorageRequestMessage request,
      FileDirectoryProperties properties,
      string defaultValue)
    {
      if (properties != null && properties.creationTimeToSet.HasValue)
        request.AddOptionalHeader("x-ms-file-creation-time", Request.ConvertDateTimeToSnapshotString(properties.creationTimeToSet.Value));
      else
        request.AddOptionalHeader("x-ms-file-creation-time", defaultValue);
    }

    private static void AddLastWriteTime(
      StorageRequestMessage request,
      FileDirectoryProperties properties,
      string defaultValue)
    {
      if (properties != null && properties.lastWriteTimeToSet.HasValue)
        request.AddOptionalHeader("x-ms-file-last-write-time", Request.ConvertDateTimeToSnapshotString(properties.lastWriteTimeToSet.Value));
      else
        request.AddOptionalHeader("x-ms-file-last-write-time", defaultValue);
    }
  }
}
