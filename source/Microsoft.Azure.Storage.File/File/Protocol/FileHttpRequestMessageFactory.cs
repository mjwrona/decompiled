// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Storage.File.Protocol.FileHttpRequestMessageFactory
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
using System.Globalization;
using System.Net.Http;

namespace Microsoft.Azure.Storage.File.Protocol
{
  internal static class FileHttpRequestMessageFactory
  {
    public static StorageRequestMessage Create(
      Uri uri,
      int? timeout,
      FileProperties properties,
      string filePermissionToSet,
      long fileSize,
      AccessCondition accessCondition,
      HttpContent content,
      OperationContext operationContext,
      ICanonicalizer canonicalizer,
      StorageCredentials credentials)
    {
      CommonUtility.AssertNotNull(nameof (properties), (object) properties);
      StorageRequestMessage requestMessage = HttpRequestMessageFactory.CreateRequestMessage(HttpMethod.Put, uri, timeout, (UriQueryBuilder) null, content, operationContext, canonicalizer, credentials);
      if (properties.CacheControl != null)
        requestMessage.AddOptionalHeader("x-ms-cache-control", properties.CacheControl);
      if (properties.ContentType != null)
        requestMessage.AddOptionalHeader("x-ms-content-type", properties.ContentType);
      if (properties.ContentLanguage != null)
        requestMessage.AddOptionalHeader("x-ms-content-language", properties.ContentLanguage);
      if (properties.ContentEncoding != null)
        requestMessage.AddOptionalHeader("x-ms-content-encoding", properties.ContentEncoding);
      if (properties.ContentDisposition != null)
        requestMessage.AddOptionalHeader("x-ms-content-disposition", properties.ContentDisposition);
      FileHttpRequestMessageFactory.AddFilePermissionOrFilePermissionKey(requestMessage, filePermissionToSet, properties, "Inherit");
      FileHttpRequestMessageFactory.AddNtfsFileAttributes(requestMessage, properties, "None");
      FileHttpRequestMessageFactory.AddCreationTime(requestMessage, properties, "Now");
      FileHttpRequestMessageFactory.AddLastWriteTime(requestMessage, properties, "Now");
      requestMessage.ApplyFileContentChecksumHeaders(properties.ContentChecksum);
      requestMessage.Headers.Add("x-ms-type", "File");
      requestMessage.Headers.Add("x-ms-content-length", fileSize.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo));
      properties.Length = fileSize;
      requestMessage.ApplyAccessCondition(accessCondition);
      return requestMessage;
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
      UriQueryBuilder builder = new UriQueryBuilder();
      FileHttpRequestMessageFactory.AddShareSnapshot(builder, shareSnapshot);
      StorageRequestMessage properties = HttpRequestMessageFactory.GetProperties(uri, timeout, builder, content, operationContext, canonicalizer, credentials);
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
      UriQueryBuilder builder = new UriQueryBuilder();
      FileHttpRequestMessageFactory.AddShareSnapshot(builder, shareSnapshot);
      StorageRequestMessage metadata = HttpRequestMessageFactory.GetMetadata(uri, timeout, builder, content, operationContext, canonicalizer, credentials);
      metadata.ApplyAccessCondition(accessCondition);
      return metadata;
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
      StorageRequestMessage request = HttpRequestMessageFactory.SetMetadata(uri, timeout, (UriQueryBuilder) null, content, operationContext, canonicalizer, credentials);
      request.ApplyAccessCondition(accessCondition);
      return request;
    }

    public static void AddMetadata(
      StorageRequestMessage request,
      IDictionary<string, string> metadata)
    {
      HttpRequestMessageFactory.AddMetadata(request, metadata);
    }

    public static void AddMetadata(StorageRequestMessage request, string name, string value) => HttpRequestMessageFactory.AddMetadata(request, name, value);

    public static StorageRequestMessage Delete(
      Uri uri,
      int? timeout,
      AccessCondition accessCondition,
      HttpContent content,
      OperationContext operationContext,
      ICanonicalizer canonicalizer,
      StorageCredentials credentials)
    {
      StorageRequestMessage request = HttpRequestMessageFactory.Delete(uri, timeout, (UriQueryBuilder) null, content, operationContext, canonicalizer, credentials);
      request.ApplyAccessCondition(accessCondition);
      return request;
    }

    private static void AddRange(StorageRequestMessage request, long? offset, long? count)
    {
      if (count.HasValue)
      {
        CommonUtility.AssertNotNull(nameof (offset), (object) offset);
        CommonUtility.AssertInBounds<long>(nameof (count), count.Value, 1L, long.MaxValue);
      }
      if (!offset.HasValue)
        return;
      string str1 = offset.ToString();
      string empty = string.Empty;
      if (count.HasValue)
      {
        long? nullable1 = offset;
        long num1 = count.Value;
        long? nullable2 = nullable1.HasValue ? new long?(nullable1.GetValueOrDefault() + num1) : new long?();
        long num2 = 1;
        empty = (nullable2.HasValue ? new long?(nullable2.GetValueOrDefault() - num2) : new long?()).ToString();
      }
      string str2 = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "bytes={0}-{1}", (object) str1, (object) empty);
      request.Headers.Add("x-ms-range", str2);
    }

    public static StorageRequestMessage Get(
      Uri uri,
      int? timeout,
      DateTimeOffset? shareSnapshot,
      AccessCondition accessCondition,
      HttpContent content,
      OperationContext operationContext,
      ICanonicalizer canonicalizer,
      StorageCredentials credentials)
    {
      UriQueryBuilder builder = new UriQueryBuilder();
      FileHttpRequestMessageFactory.AddShareSnapshot(builder, shareSnapshot);
      StorageRequestMessage requestMessage = HttpRequestMessageFactory.CreateRequestMessage(HttpMethod.Get, uri, timeout, builder, content, operationContext, canonicalizer, credentials);
      requestMessage.ApplyAccessCondition(accessCondition);
      return requestMessage;
    }

    public static StorageRequestMessage ListRanges(
      Uri uri,
      int? timeout,
      long? offset,
      long? count,
      DateTimeOffset? shareSnapshot,
      AccessCondition accessCondition,
      HttpContent content,
      OperationContext operationContext,
      ICanonicalizer canonicalizer,
      StorageCredentials credentials)
    {
      if (offset.HasValue)
        CommonUtility.AssertNotNull(nameof (count), (object) count);
      UriQueryBuilder builder = new UriQueryBuilder();
      FileHttpRequestMessageFactory.AddShareSnapshot(builder, shareSnapshot);
      builder.Add("comp", "rangelist");
      StorageRequestMessage requestMessage = HttpRequestMessageFactory.CreateRequestMessage(HttpMethod.Get, uri, timeout, builder, content, operationContext, canonicalizer, credentials);
      FileHttpRequestMessageFactory.AddRange(requestMessage, offset, count);
      requestMessage.ApplyAccessCondition(accessCondition);
      return requestMessage;
    }

    public static StorageRequestMessage ListHandles(
      Uri uri,
      int? timeout,
      DateTimeOffset? shareSnapshot,
      int? maxResults,
      bool? recursive,
      FileContinuationToken nextMarker,
      AccessCondition accessCondition,
      HttpContent content,
      OperationContext operationContext,
      ICanonicalizer canonicalizer,
      StorageCredentials credentials)
    {
      UriQueryBuilder builder = new UriQueryBuilder();
      FileHttpRequestMessageFactory.AddShareSnapshot(builder, shareSnapshot);
      builder.Add("comp", "listhandles");
      if (maxResults.HasValue)
        builder.Add("MaxResults", maxResults.Value.ToString());
      if (nextMarker != null)
        builder.Add("marker", nextMarker.NextMarker);
      StorageRequestMessage requestMessage = HttpRequestMessageFactory.CreateRequestMessage(HttpMethod.Get, uri, timeout, builder, content, operationContext, canonicalizer, credentials);
      requestMessage.ApplyAccessCondition(accessCondition);
      if (recursive.HasValue)
        requestMessage.AddOptionalHeader("x-ms-recursive", recursive.Value.ToString());
      return requestMessage;
    }

    public static StorageRequestMessage CloseHandle(
      Uri uri,
      int? timeout,
      DateTimeOffset? shareSnapshot,
      string handleId,
      bool? recursive,
      FileContinuationToken token,
      AccessCondition accessCondition,
      HttpContent content,
      OperationContext operationContext,
      ICanonicalizer canonicalizer,
      StorageCredentials credentials)
    {
      UriQueryBuilder builder = new UriQueryBuilder();
      FileHttpRequestMessageFactory.AddShareSnapshot(builder, shareSnapshot);
      builder.Add("comp", "forceclosehandles");
      if (token != null && token.NextMarker != null)
        builder.Add("marker", token.NextMarker);
      StorageRequestMessage requestMessage = HttpRequestMessageFactory.CreateRequestMessage(HttpMethod.Put, uri, timeout, builder, content, operationContext, canonicalizer, credentials);
      if (handleId != null)
        requestMessage.AddOptionalHeader("x-ms-handle-id", handleId);
      if (recursive.HasValue)
        requestMessage.AddOptionalHeader("x-ms-recursive", recursive.Value.ToString());
      requestMessage.ApplyAccessCondition(accessCondition);
      return requestMessage;
    }

    public static StorageRequestMessage SetProperties(
      Uri uri,
      int? timeout,
      FileProperties properties,
      string filePermissionToSet,
      AccessCondition accessCondition,
      HttpContent content,
      OperationContext operationContext,
      ICanonicalizer canonicalizer,
      StorageCredentials credentials)
    {
      CommonUtility.AssertNotNull(nameof (properties), (object) properties);
      UriQueryBuilder builder = new UriQueryBuilder();
      builder.Add("comp", nameof (properties));
      StorageRequestMessage requestMessage = HttpRequestMessageFactory.CreateRequestMessage(HttpMethod.Put, uri, timeout, builder, content, operationContext, canonicalizer, credentials);
      if (properties != null)
      {
        requestMessage.AddOptionalHeader("x-ms-cache-control", properties.CacheControl);
        requestMessage.AddOptionalHeader("x-ms-content-disposition", properties.ContentDisposition);
        requestMessage.AddOptionalHeader("x-ms-content-encoding", properties.ContentEncoding);
        requestMessage.AddOptionalHeader("x-ms-content-language", properties.ContentLanguage);
        requestMessage.AddOptionalHeader("x-ms-content-type", properties.ContentType);
        requestMessage.ApplyFileContentChecksumHeaders(properties.ContentChecksum);
      }
      FileHttpRequestMessageFactory.AddFilePermissionOrFilePermissionKey(requestMessage, filePermissionToSet, properties, "Preserve");
      FileHttpRequestMessageFactory.AddNtfsFileAttributes(requestMessage, properties, "Preserve");
      FileHttpRequestMessageFactory.AddCreationTime(requestMessage, properties, "Preserve");
      FileHttpRequestMessageFactory.AddLastWriteTime(requestMessage, properties, "Preserve");
      requestMessage.ApplyAccessCondition(accessCondition);
      return requestMessage;
    }

    public static StorageRequestMessage Get(
      Uri uri,
      int? timeout,
      long? offset,
      long? count,
      ChecksumRequested rangeContentChecksumRequested,
      DateTimeOffset? shareSnapshot,
      AccessCondition accessCondition,
      HttpContent content,
      OperationContext operationContext,
      ICanonicalizer canonicalizer,
      StorageCredentials credentials)
    {
      if (offset.HasValue && offset.Value < 0L)
        CommonUtility.ArgumentOutOfRange(nameof (offset), (object) offset);
      rangeContentChecksumRequested.AssertInBounds(offset, count, 4194304, 4194304);
      StorageRequestMessage request = FileHttpRequestMessageFactory.Get(uri, timeout, shareSnapshot, accessCondition, content, operationContext, canonicalizer, credentials);
      FileHttpRequestMessageFactory.AddRange(request, offset, count);
      request.ApplyRangeContentChecksumRequested(offset, rangeContentChecksumRequested);
      return request;
    }

    public static StorageRequestMessage Resize(
      Uri uri,
      int? timeout,
      long newFileSize,
      AccessCondition accessCondition,
      HttpContent content,
      OperationContext operationContext,
      ICanonicalizer canonicalizer,
      StorageCredentials credentials)
    {
      UriQueryBuilder builder = new UriQueryBuilder();
      builder.Add("comp", "properties");
      StorageRequestMessage requestMessage = HttpRequestMessageFactory.CreateRequestMessage(HttpMethod.Put, uri, timeout, builder, content, operationContext, canonicalizer, credentials);
      requestMessage.Headers.Add("x-ms-content-length", newFileSize.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo));
      requestMessage.AddOptionalHeader("x-ms-file-permission", "Preserve");
      requestMessage.AddOptionalHeader("x-ms-file-attributes", "Preserve");
      requestMessage.AddOptionalHeader("x-ms-file-creation-time", "Preserve");
      requestMessage.AddOptionalHeader("x-ms-file-last-write-time", "Preserve");
      requestMessage.ApplyAccessCondition(accessCondition);
      return requestMessage;
    }

    public static StorageRequestMessage PutRange(
      Uri uri,
      int? timeout,
      FileRange fileRange,
      FileRangeWrite fileRangeWrite,
      AccessCondition accessCondition,
      HttpContent content,
      OperationContext operationContext,
      ICanonicalizer canonicalizer,
      StorageCredentials credentials)
    {
      UriQueryBuilder builder = new UriQueryBuilder();
      builder.Add("comp", "range");
      StorageRequestMessage requestMessage = HttpRequestMessageFactory.CreateRequestMessage(HttpMethod.Put, uri, timeout, builder, content, operationContext, canonicalizer, credentials);
      requestMessage.AddOptionalHeader("x-ms-range", fileRange.ToString());
      requestMessage.Headers.Add("x-ms-write", fileRangeWrite.ToString());
      requestMessage.ApplyAccessCondition(accessCondition);
      return requestMessage;
    }

    public static StorageRequestMessage PutRangeFromUrl(
      Uri uri,
      Uri sourceUri,
      FileRange sourceFileRange,
      FileRange destFileRange,
      int? timeout,
      Checksum sourceContentChecksum,
      AccessCondition sourceAccessCondition,
      HttpContent content,
      OperationContext operationContext,
      ICanonicalizer canonicalizer,
      StorageCredentials credentials)
    {
      UriQueryBuilder builder = new UriQueryBuilder();
      builder.Add("comp", "range");
      StorageRequestMessage requestMessage = HttpRequestMessageFactory.CreateRequestMessage(HttpMethod.Put, uri, timeout, builder, content, operationContext, canonicalizer, credentials);
      requestMessage.Headers.Add("x-ms-copy-source", sourceUri.AbsoluteUri);
      requestMessage.Headers.Add("x-ms-write", FileRangeWrite.Update.ToString());
      requestMessage.Headers.Add("x-ms-range", destFileRange.ToString());
      requestMessage.Headers.Add("x-ms-source-range", sourceFileRange.ToString());
      requestMessage.ApplySourceContentChecksumHeaders(sourceContentChecksum);
      requestMessage.ApplyAccessConditionToSource(sourceAccessCondition);
      return requestMessage;
    }

    public static StorageRequestMessage CopyFrom(
      Uri uri,
      int? timeout,
      Uri source,
      AccessCondition sourceAccessCondition,
      AccessCondition destAccessCondition,
      HttpContent content,
      OperationContext operationContext,
      ICanonicalizer canonicalizer,
      StorageCredentials credentials)
    {
      StorageRequestMessage requestMessage = HttpRequestMessageFactory.CreateRequestMessage(HttpMethod.Put, uri, timeout, (UriQueryBuilder) null, content, operationContext, canonicalizer, credentials);
      requestMessage.Headers.Add("x-ms-copy-source", source.AbsoluteUri);
      requestMessage.ApplyAccessCondition(destAccessCondition);
      requestMessage.ApplyAccessConditionToSource(sourceAccessCondition);
      return requestMessage;
    }

    public static StorageRequestMessage GetServiceProperties(
      Uri uri,
      int? timeout,
      OperationContext operationContext,
      ICanonicalizer canonicalizer,
      StorageCredentials credentials)
    {
      return HttpRequestMessageFactory.GetServiceProperties(uri, timeout, operationContext, canonicalizer, credentials);
    }

    internal static StorageRequestMessage SetServiceProperties(
      Uri uri,
      int? timeout,
      HttpContent content,
      OperationContext operationContext,
      ICanonicalizer canonicalizer,
      StorageCredentials credentials)
    {
      return HttpRequestMessageFactory.SetServiceProperties(uri, timeout, content, operationContext, canonicalizer, credentials);
    }

    public static StorageRequestMessage AbortCopy(
      Uri uri,
      int? timeout,
      string copyId,
      AccessCondition accessCondition,
      HttpContent content,
      OperationContext operationContext,
      ICanonicalizer canonicalizer,
      StorageCredentials credentials)
    {
      UriQueryBuilder builder = new UriQueryBuilder();
      builder.Add("comp", "copy");
      builder.Add("copyid", copyId);
      StorageRequestMessage requestMessage = HttpRequestMessageFactory.CreateRequestMessage(HttpMethod.Put, uri, timeout, builder, content, operationContext, canonicalizer, credentials);
      requestMessage.Headers.Add("x-ms-copy-action", "abort");
      requestMessage.ApplyAccessCondition(accessCondition);
      return requestMessage;
    }

    private static void AddShareSnapshot(UriQueryBuilder builder, DateTimeOffset? snapshot)
    {
      if (!snapshot.HasValue)
        return;
      builder.Add("sharesnapshot", Request.ConvertDateTimeToSnapshotString(snapshot.Value));
    }

    private static void AddFilePermissionOrFilePermissionKey(
      StorageRequestMessage request,
      string filePermissionToSet,
      FileProperties properties,
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
      FileProperties properties,
      string defaultValue)
    {
      if (properties != null && properties.ntfsAttributesToSet.HasValue)
        request.AddOptionalHeader("x-ms-file-attributes", CloudFileNtfsAttributesHelper.ToString(properties.ntfsAttributesToSet.Value));
      else
        request.AddOptionalHeader("x-ms-file-attributes", defaultValue);
    }

    private static void AddCreationTime(
      StorageRequestMessage request,
      FileProperties properties,
      string defaultValue)
    {
      if (properties != null && properties.creationTimeToSet.HasValue)
        request.AddOptionalHeader("x-ms-file-creation-time", Request.ConvertDateTimeToSnapshotString(properties.creationTimeToSet.Value));
      else
        request.AddOptionalHeader("x-ms-file-creation-time", defaultValue);
    }

    private static void AddLastWriteTime(
      StorageRequestMessage request,
      FileProperties properties,
      string defaultValue)
    {
      if (properties != null && properties.lastWriteTimeToSet.HasValue)
        request.AddOptionalHeader("x-ms-file-last-write-time", Request.ConvertDateTimeToSnapshotString(properties.lastWriteTimeToSet.Value));
      else
        request.AddOptionalHeader("x-ms-file-last-write-time", defaultValue);
    }
  }
}
