// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Storage.Blob.Protocol.BlobHttpRequestMessageFactory
// Assembly: Microsoft.Azure.Storage.Blob, Version=11.2.3.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: A04A3512-352A-442F-A95B-BC1B94EF8840
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Storage.Blob.dll

using Microsoft.Azure.Storage.Auth;
using Microsoft.Azure.Storage.Auth.Protocol;
using Microsoft.Azure.Storage.Core;
using Microsoft.Azure.Storage.Core.Auth;
using Microsoft.Azure.Storage.Core.Executor;
using Microsoft.Azure.Storage.Core.Util;
using Microsoft.Azure.Storage.Shared.Protocol;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace Microsoft.Azure.Storage.Blob.Protocol
{
  internal static class BlobHttpRequestMessageFactory
  {
    public static StorageRequestMessage AppendBlock(
      Uri uri,
      int? timeout,
      AccessCondition accessCondition,
      HttpContent content,
      OperationContext operationContext,
      ICanonicalizer canonicalizer,
      StorageCredentials credentials,
      BlobRequestOptions options)
    {
      UriQueryBuilder builder = new UriQueryBuilder();
      builder.Add("comp", "appendblock");
      StorageRequestMessage requestMessage = HttpRequestMessageFactory.CreateRequestMessage(HttpMethod.Put, uri, timeout, builder, content, operationContext, canonicalizer, credentials);
      requestMessage.ApplyAccessCondition(accessCondition);
      requestMessage.ApplyAppendCondition(accessCondition);
      BlobRequest.ApplyCustomerProvidedKeyOrEncryptionScope(requestMessage, options, false);
      return requestMessage;
    }

    public static StorageRequestMessage AppendBlock(
      Uri uri,
      Uri sourceUri,
      long? offset,
      long? count,
      Checksum sourceContentChecksum,
      int? timeout,
      AccessCondition sourceAccessCondition,
      AccessCondition destAccessCondition,
      HttpContent content,
      OperationContext operationContext,
      ICanonicalizer canonicalizer,
      StorageCredentials credentials,
      BlobRequestOptions options)
    {
      UriQueryBuilder builder = new UriQueryBuilder();
      builder.Add("comp", "appendblock");
      StorageRequestMessage requestMessage = HttpRequestMessageFactory.CreateRequestMessage(HttpMethod.Put, uri, timeout, builder, content, operationContext, canonicalizer, credentials);
      requestMessage.ApplyAccessConditionToSource(sourceAccessCondition);
      requestMessage.ApplyAccessCondition(destAccessCondition);
      requestMessage.ApplyAppendCondition(destAccessCondition);
      BlobHttpRequestMessageFactory.AddCopySource(requestMessage, sourceUri);
      BlobHttpRequestMessageFactory.AddSourceRange(requestMessage, offset, count);
      requestMessage.ApplySourceContentChecksumHeaders(sourceContentChecksum);
      BlobRequest.ApplyCustomerProvidedKeyOrEncryptionScope(requestMessage, options, false);
      return requestMessage;
    }

    public static StorageRequestMessage Put(
      Uri uri,
      int? timeout,
      BlobProperties properties,
      BlobType blobType,
      long pageBlobSize,
      PremiumPageBlobTier? pageBlobTier,
      AccessCondition accessCondition,
      HttpContent content,
      OperationContext operationContext,
      ICanonicalizer canonicalizer,
      StorageCredentials credentials,
      BlobRequestOptions options)
    {
      CommonUtility.AssertNotNull(nameof (properties), (object) properties);
      if (blobType == BlobType.Unspecified)
        throw new InvalidOperationException("The blob type cannot be undefined.");
      StorageRequestMessage requestMessage = HttpRequestMessageFactory.CreateRequestMessage(HttpMethod.Put, uri, timeout, (UriQueryBuilder) null, content, operationContext, canonicalizer, credentials);
      if (properties.CacheControl != null)
        requestMessage.AddOptionalHeader("x-ms-blob-cache-control", properties.CacheControl);
      if (properties.ContentType != null)
        requestMessage.AddOptionalHeader("x-ms-blob-content-type", properties.ContentType);
      if (properties.ContentLanguage != null)
        requestMessage.AddOptionalHeader("x-ms-blob-content-language", properties.ContentLanguage);
      if (properties.ContentEncoding != null)
        requestMessage.AddOptionalHeader("x-ms-blob-content-encoding", properties.ContentEncoding);
      if (properties.ContentDisposition != null)
        requestMessage.AddOptionalHeader("x-ms-blob-content-disposition", properties.ContentDisposition);
      requestMessage.ApplyBlobContentChecksumHeaders(properties.ContentChecksum);
      switch (blobType)
      {
        case BlobType.PageBlob:
          requestMessage.Headers.Add("x-ms-blob-type", "PageBlob");
          requestMessage.Headers.Add("x-ms-blob-content-length", pageBlobSize.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo));
          properties.Length = pageBlobSize;
          if (pageBlobTier.HasValue)
          {
            requestMessage.Headers.Add("x-ms-access-tier", pageBlobTier.Value.ToString());
            break;
          }
          break;
        case BlobType.BlockBlob:
          requestMessage.Headers.Add("x-ms-blob-type", "BlockBlob");
          break;
        default:
          requestMessage.Headers.Add("x-ms-blob-type", "AppendBlob");
          break;
      }
      requestMessage.ApplyAccessCondition(accessCondition);
      BlobRequest.ApplyCustomerProvidedKeyOrEncryptionScope(requestMessage, options, false);
      return requestMessage;
    }

    private static void AddSnapshot(UriQueryBuilder builder, DateTimeOffset? snapshot)
    {
      if (!snapshot.HasValue)
        return;
      builder.Add(nameof (snapshot), Request.ConvertDateTimeToSnapshotString(snapshot.Value));
    }

    public static StorageRequestMessage GetPageRanges(
      Uri uri,
      int? timeout,
      DateTimeOffset? snapshot,
      long? offset,
      long? count,
      AccessCondition accessCondition,
      HttpContent content,
      OperationContext operationContext,
      ICanonicalizer canonicalizer,
      StorageCredentials credentials)
    {
      if (offset.HasValue)
        CommonUtility.AssertNotNull(nameof (count), (object) count);
      UriQueryBuilder builder = new UriQueryBuilder();
      builder.Add("comp", "pagelist");
      BlobHttpRequestMessageFactory.AddSnapshot(builder, snapshot);
      StorageRequestMessage requestMessage = HttpRequestMessageFactory.CreateRequestMessage(HttpMethod.Get, uri, timeout, builder, content, operationContext, canonicalizer, credentials);
      BlobHttpRequestMessageFactory.AddRange(requestMessage, offset, count);
      requestMessage.ApplyAccessCondition(accessCondition);
      return requestMessage;
    }

    public static StorageRequestMessage GetPageRangesDiff(
      Uri uri,
      int? timeout,
      DateTimeOffset? snapshot,
      DateTimeOffset previousSnapshotTime,
      long? offset,
      long? count,
      AccessCondition accessCondition,
      HttpContent content,
      OperationContext operationContext,
      ICanonicalizer canonicalizer,
      StorageCredentials credentials)
    {
      if (offset.HasValue)
        CommonUtility.AssertNotNull(nameof (count), (object) count);
      UriQueryBuilder builder = new UriQueryBuilder();
      builder.Add("comp", "pagelist");
      BlobHttpRequestMessageFactory.AddSnapshot(builder, snapshot);
      builder.Add("prevsnapshot", Request.ConvertDateTimeToSnapshotString(previousSnapshotTime));
      StorageRequestMessage requestMessage = HttpRequestMessageFactory.CreateRequestMessage(HttpMethod.Get, uri, timeout, builder, content, operationContext, canonicalizer, credentials);
      BlobHttpRequestMessageFactory.AddRange(requestMessage, offset, count);
      requestMessage.ApplyAccessCondition(accessCondition);
      return requestMessage;
    }

    private static void AddCopySource(StorageRequestMessage request, Uri sourceUri) => request.Headers.Add("x-ms-copy-source", sourceUri.AbsoluteUri);

    private static void AddRange(StorageRequestMessage request, long? offset, long? count) => BlobHttpRequestMessageFactory.AddRangeImpl("x-ms-range", request, offset, count);

    private static void AddSourceRange(StorageRequestMessage request, long? offset, long? count) => BlobHttpRequestMessageFactory.AddRangeImpl("x-ms-source-range", request, offset, count);

    private static void AddRangeImpl(
      string header,
      StorageRequestMessage request,
      long? offset,
      long? count)
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
      request.Headers.Add(header, str2);
    }

    public static StorageRequestMessage GetProperties(
      Uri uri,
      int? timeout,
      DateTimeOffset? snapshot,
      AccessCondition accessCondition,
      HttpContent content,
      OperationContext operationContext,
      ICanonicalizer canonicalizer,
      StorageCredentials credentials,
      BlobRequestOptions options)
    {
      UriQueryBuilder builder = new UriQueryBuilder();
      BlobHttpRequestMessageFactory.AddSnapshot(builder, snapshot);
      StorageRequestMessage properties = HttpRequestMessageFactory.GetProperties(uri, timeout, builder, content, operationContext, canonicalizer, credentials);
      properties.ApplyAccessCondition(accessCondition);
      BlobRequest.ApplyCustomerProvidedKey(properties, options?.CustomerProvidedKey, false);
      return properties;
    }

    public static StorageRequestMessage SetProperties(
      Uri uri,
      int? timeout,
      BlobProperties properties,
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
        requestMessage.AddOptionalHeader("x-ms-blob-cache-control", properties.CacheControl);
        requestMessage.AddOptionalHeader("x-ms-blob-content-disposition", properties.ContentDisposition);
        requestMessage.AddOptionalHeader("x-ms-blob-content-encoding", properties.ContentEncoding);
        requestMessage.AddOptionalHeader("x-ms-blob-content-language", properties.ContentLanguage);
        requestMessage.AddOptionalHeader("x-ms-blob-content-type", properties.ContentType);
        requestMessage.ApplyBlobContentChecksumHeaders(properties.ContentChecksum);
      }
      requestMessage.ApplyAccessCondition(accessCondition);
      return requestMessage;
    }

    public static StorageRequestMessage GetUserDelegationKey(
      Uri uri,
      int? timeout,
      AccessCondition accessCondition,
      HttpContent content,
      OperationContext operationContext,
      ICanonicalizer canonicalizer,
      StorageCredentials credentials)
    {
      UriQueryBuilder builder = new UriQueryBuilder();
      builder.Add("restype", "service");
      builder.Add("comp", "userdelegationkey");
      StorageRequestMessage requestMessage = HttpRequestMessageFactory.CreateRequestMessage(HttpMethod.Post, uri, timeout, builder, content, operationContext, canonicalizer, credentials);
      requestMessage.ApplyAccessCondition(accessCondition);
      return requestMessage;
    }

    public static StorageRequestMessage Resize(
      Uri uri,
      int? timeout,
      long newBlobSize,
      AccessCondition accessCondition,
      HttpContent content,
      OperationContext operationContext,
      ICanonicalizer canonicalizer,
      StorageCredentials credentials)
    {
      UriQueryBuilder builder = new UriQueryBuilder();
      builder.Add("comp", "properties");
      StorageRequestMessage requestMessage = HttpRequestMessageFactory.CreateRequestMessage(HttpMethod.Put, uri, timeout, builder, content, operationContext, canonicalizer, credentials);
      requestMessage.Headers.Add("x-ms-blob-content-length", newBlobSize.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo));
      requestMessage.ApplyAccessCondition(accessCondition);
      return requestMessage;
    }

    public static StorageRequestMessage SetSequenceNumber(
      Uri uri,
      int? timeout,
      SequenceNumberAction sequenceNumberAction,
      long? sequenceNumber,
      AccessCondition accessCondition,
      HttpContent content,
      OperationContext operationContext,
      ICanonicalizer canonicalizer,
      StorageCredentials credentials)
    {
      CommonUtility.AssertInBounds<SequenceNumberAction>(nameof (sequenceNumberAction), sequenceNumberAction, SequenceNumberAction.Max, SequenceNumberAction.Increment);
      if (sequenceNumberAction == SequenceNumberAction.Increment)
      {
        if (sequenceNumber.HasValue)
          throw new ArgumentException("The sequence number may not be specified for an increment operation.", nameof (sequenceNumber));
      }
      else
      {
        CommonUtility.AssertNotNull(nameof (sequenceNumber), (object) sequenceNumber);
        CommonUtility.AssertInBounds<long>(nameof (sequenceNumber), sequenceNumber.Value, 0L);
      }
      UriQueryBuilder builder = new UriQueryBuilder();
      builder.Add("comp", "properties");
      StorageRequestMessage requestMessage = HttpRequestMessageFactory.CreateRequestMessage(HttpMethod.Put, uri, timeout, builder, content, operationContext, canonicalizer, credentials);
      requestMessage.Headers.Add("x-ms-sequence-number-action", sequenceNumberAction.ToString());
      if (sequenceNumberAction != SequenceNumberAction.Increment)
        requestMessage.Headers.Add("x-ms-blob-sequence-number", sequenceNumber.Value.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      requestMessage.ApplyAccessCondition(accessCondition);
      return requestMessage;
    }

    public static StorageRequestMessage GetMetadata(
      Uri uri,
      int? timeout,
      DateTimeOffset? snapshot,
      AccessCondition accessCondition,
      HttpContent content,
      OperationContext operationContext,
      ICanonicalizer canonicalizer,
      StorageCredentials credentials,
      BlobRequestOptions options)
    {
      UriQueryBuilder builder = new UriQueryBuilder();
      BlobHttpRequestMessageFactory.AddSnapshot(builder, snapshot);
      StorageRequestMessage metadata = HttpRequestMessageFactory.GetMetadata(uri, timeout, builder, content, operationContext, canonicalizer, credentials);
      metadata.ApplyAccessCondition(accessCondition);
      BlobRequest.ApplyCustomerProvidedKey(metadata, options?.CustomerProvidedKey, false);
      return metadata;
    }

    public static StorageRequestMessage SetMetadata(
      Uri uri,
      int? timeout,
      AccessCondition accessCondition,
      HttpContent content,
      OperationContext operationContext,
      ICanonicalizer canonicalizer,
      StorageCredentials credentials,
      BlobRequestOptions options)
    {
      StorageRequestMessage request = HttpRequestMessageFactory.SetMetadata(uri, timeout, (UriQueryBuilder) null, content, operationContext, canonicalizer, credentials);
      request.ApplyAccessCondition(accessCondition);
      BlobRequest.ApplyCustomerProvidedKeyOrEncryptionScope(request, options, false);
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
      DateTimeOffset? snapshot,
      DeleteSnapshotsOption deleteSnapshotsOption,
      AccessCondition accessCondition,
      HttpContent content,
      OperationContext operationContext,
      ICanonicalizer canonicalizer,
      StorageCredentials credentials)
    {
      if (snapshot.HasValue && deleteSnapshotsOption != DeleteSnapshotsOption.None)
        throw new InvalidOperationException(string.Format("The option '{0}' must be 'None' to delete a specific snapshot specified by '{1}'", (object) nameof (deleteSnapshotsOption), (object) nameof (snapshot)));
      UriQueryBuilder builder = new UriQueryBuilder();
      BlobHttpRequestMessageFactory.AddSnapshot(builder, snapshot);
      StorageRequestMessage request = HttpRequestMessageFactory.Delete(uri, timeout, builder, content, operationContext, canonicalizer, credentials);
      switch (deleteSnapshotsOption)
      {
        case DeleteSnapshotsOption.IncludeSnapshots:
          request.Headers.Add("x-ms-delete-snapshots", "include");
          break;
        case DeleteSnapshotsOption.DeleteSnapshotsOnly:
          request.Headers.Add("x-ms-delete-snapshots", "only");
          break;
      }
      request.ApplyAccessCondition(accessCondition);
      return request;
    }

    public static StorageRequestMessage Undelete(
      Uri uri,
      int? timeout,
      AccessCondition accessCondition,
      HttpContent content,
      OperationContext operationContext,
      ICanonicalizer canonicalizer,
      StorageCredentials credentials)
    {
      UriQueryBuilder builder = new UriQueryBuilder();
      StorageRequestMessage request = HttpRequestMessageFactory.Undelete(uri, timeout, builder, content, operationContext, canonicalizer, credentials);
      request.ApplyAccessCondition(accessCondition);
      return request;
    }

    public static StorageRequestMessage Snapshot(
      Uri uri,
      int? timeout,
      AccessCondition accessCondition,
      HttpContent content,
      OperationContext operationContext,
      ICanonicalizer canonicalizer,
      StorageCredentials credentials,
      BlobRequestOptions options)
    {
      UriQueryBuilder builder = new UriQueryBuilder();
      builder.Add("comp", "snapshot");
      StorageRequestMessage requestMessage = HttpRequestMessageFactory.CreateRequestMessage(HttpMethod.Put, uri, timeout, builder, content, operationContext, canonicalizer, credentials);
      requestMessage.ApplyAccessCondition(accessCondition);
      BlobRequest.ApplyCustomerProvidedKeyOrEncryptionScope(requestMessage, options, false);
      return requestMessage;
    }

    public static StorageRequestMessage Lease(
      Uri uri,
      int? timeout,
      LeaseAction action,
      string proposedLeaseId,
      int? leaseDuration,
      int? leaseBreakPeriod,
      AccessCondition accessCondition,
      HttpContent content,
      OperationContext operationContext,
      ICanonicalizer canonicalizer,
      StorageCredentials credentials)
    {
      UriQueryBuilder builder = new UriQueryBuilder();
      builder.Add("comp", "lease");
      StorageRequestMessage requestMessage = HttpRequestMessageFactory.CreateRequestMessage(HttpMethod.Put, uri, timeout, builder, content, operationContext, canonicalizer, credentials);
      requestMessage.ApplyAccessCondition(accessCondition);
      BlobHttpRequestMessageFactory.AddLeaseAction(requestMessage, action);
      BlobHttpRequestMessageFactory.AddLeaseDuration(requestMessage, leaseDuration);
      BlobHttpRequestMessageFactory.AddProposedLeaseId(requestMessage, proposedLeaseId);
      BlobHttpRequestMessageFactory.AddLeaseBreakPeriod(requestMessage, leaseBreakPeriod);
      return requestMessage;
    }

    internal static void AddProposedLeaseId(StorageRequestMessage request, string proposedLeaseId) => request.AddOptionalHeader("x-ms-proposed-lease-id", proposedLeaseId);

    internal static void AddLeaseDuration(StorageRequestMessage request, int? leaseDuration) => request.AddOptionalHeader("x-ms-lease-duration", leaseDuration);

    internal static void AddLeaseBreakPeriod(StorageRequestMessage request, int? leaseBreakPeriod) => request.AddOptionalHeader("x-ms-lease-break-period", leaseBreakPeriod);

    internal static void AddLeaseAction(StorageRequestMessage request, LeaseAction leaseAction) => request.Headers.Add("x-ms-lease-action", leaseAction.ToString().ToLower());

    public static StorageRequestMessage PutBlock(
      Uri uri,
      int? timeout,
      string blockId,
      AccessCondition accessCondition,
      HttpContent content,
      OperationContext operationContext,
      ICanonicalizer canonicalizer,
      StorageCredentials credentials,
      BlobRequestOptions options)
    {
      UriQueryBuilder builder = new UriQueryBuilder();
      builder.Add("comp", "block");
      builder.Add("blockid", blockId);
      StorageRequestMessage requestMessage = HttpRequestMessageFactory.CreateRequestMessage(HttpMethod.Put, uri, timeout, builder, content, operationContext, canonicalizer, credentials);
      requestMessage.ApplyLeaseId(accessCondition);
      BlobRequest.ApplyCustomerProvidedKeyOrEncryptionScope(requestMessage, options, false);
      return requestMessage;
    }

    public static StorageRequestMessage PutBlock(
      Uri uri,
      Uri sourceUri,
      long? offset,
      long? count,
      Checksum sourceContentChecksum,
      int? timeout,
      string blockId,
      AccessCondition accessCondition,
      HttpContent content,
      OperationContext operationContext,
      ICanonicalizer canonicalizer,
      StorageCredentials credentials,
      BlobRequestOptions options)
    {
      if (offset.HasValue && offset.Value < 0L)
        CommonUtility.ArgumentOutOfRange(nameof (offset), (object) offset);
      UriQueryBuilder builder = new UriQueryBuilder();
      builder.Add("comp", "block");
      builder.Add("blockid", blockId);
      StorageRequestMessage requestMessage = HttpRequestMessageFactory.CreateRequestMessage(HttpMethod.Put, uri, timeout, builder, content, operationContext, canonicalizer, credentials);
      requestMessage.ApplyLeaseId(accessCondition);
      BlobHttpRequestMessageFactory.AddCopySource(requestMessage, sourceUri);
      BlobHttpRequestMessageFactory.AddSourceRange(requestMessage, offset, count);
      requestMessage.ApplySourceContentChecksumHeaders(sourceContentChecksum);
      BlobRequest.ApplyCustomerProvidedKeyOrEncryptionScope(requestMessage, options, false);
      return requestMessage;
    }

    public static StorageRequestMessage PutBlockList(
      Uri uri,
      int? timeout,
      BlobProperties properties,
      AccessCondition accessCondition,
      HttpContent content,
      OperationContext operationContext,
      ICanonicalizer canonicalizer,
      StorageCredentials credentials,
      BlobRequestOptions options)
    {
      CommonUtility.AssertNotNull(nameof (properties), (object) properties);
      UriQueryBuilder builder = new UriQueryBuilder();
      builder.Add("comp", "blocklist");
      StorageRequestMessage requestMessage = HttpRequestMessageFactory.CreateRequestMessage(HttpMethod.Put, uri, timeout, builder, content, operationContext, canonicalizer, credentials);
      if (properties != null)
      {
        requestMessage.AddOptionalHeader("x-ms-blob-cache-control", properties.CacheControl);
        requestMessage.AddOptionalHeader("x-ms-blob-content-type", properties.ContentType);
        requestMessage.AddOptionalHeader("x-ms-blob-content-language", properties.ContentLanguage);
        requestMessage.AddOptionalHeader("x-ms-blob-content-encoding", properties.ContentEncoding);
        requestMessage.AddOptionalHeader("x-ms-blob-content-disposition", properties.ContentDisposition);
        requestMessage.ApplyBlobContentChecksumHeaders(properties.ContentChecksum);
      }
      requestMessage.ApplyAccessCondition(accessCondition);
      BlobRequest.ApplyCustomerProvidedKeyOrEncryptionScope(requestMessage, options, false);
      return requestMessage;
    }

    public static StorageRequestMessage GetBlockList(
      Uri uri,
      int? timeout,
      DateTimeOffset? snapshot,
      BlockListingFilter typesOfBlocks,
      AccessCondition accessCondition,
      HttpContent content,
      OperationContext operationContext,
      ICanonicalizer canonicalizer,
      StorageCredentials credentials)
    {
      UriQueryBuilder builder = new UriQueryBuilder();
      builder.Add("comp", "blocklist");
      builder.Add("blocklisttype", typesOfBlocks.ToString());
      BlobHttpRequestMessageFactory.AddSnapshot(builder, snapshot);
      StorageRequestMessage requestMessage = HttpRequestMessageFactory.CreateRequestMessage(HttpMethod.Get, uri, timeout, builder, content, operationContext, canonicalizer, credentials);
      requestMessage.ApplyAccessCondition(accessCondition);
      return requestMessage;
    }

    public static StorageRequestMessage PutPage(
      Uri uri,
      int? timeout,
      PageRange pageRange,
      PageWrite pageWrite,
      AccessCondition accessCondition,
      HttpContent content,
      OperationContext operationContext,
      ICanonicalizer canonicalizer,
      StorageCredentials credentials,
      BlobRequestOptions options)
    {
      UriQueryBuilder builder = new UriQueryBuilder();
      builder.Add("comp", "page");
      StorageRequestMessage requestMessage = HttpRequestMessageFactory.CreateRequestMessage(HttpMethod.Put, uri, timeout, builder, content, operationContext, canonicalizer, credentials);
      requestMessage.Headers.Add("x-ms-range", pageRange.ToString());
      requestMessage.Headers.Add("x-ms-page-write", pageWrite.ToString());
      requestMessage.ApplyAccessCondition(accessCondition);
      requestMessage.ApplySequenceNumberCondition(accessCondition);
      BlobRequest.ApplyCustomerProvidedKeyOrEncryptionScope(requestMessage, options, false);
      return requestMessage;
    }

    public static StorageRequestMessage PutPage(
      Uri uri,
      Uri sourceUri,
      long? offset,
      long? count,
      Checksum sourceContentChecksum,
      int? timeout,
      PageRange pageRange,
      AccessCondition sourceAccessCondition,
      AccessCondition destAccessCondition,
      HttpContent content,
      OperationContext operationContext,
      ICanonicalizer canonicalizer,
      StorageCredentials credentials,
      BlobRequestOptions options)
    {
      UriQueryBuilder builder = new UriQueryBuilder();
      builder.Add("comp", "page");
      builder.Add("update", (string) null);
      StorageRequestMessage requestMessage = HttpRequestMessageFactory.CreateRequestMessage(HttpMethod.Put, uri, timeout, builder, content, operationContext, canonicalizer, credentials);
      requestMessage.Headers.Add("x-ms-range", pageRange.ToString());
      requestMessage.Headers.Add("x-ms-page-write", "update");
      requestMessage.ApplyAccessConditionToSource(sourceAccessCondition);
      requestMessage.ApplyAccessCondition(destAccessCondition);
      requestMessage.ApplySequenceNumberCondition(destAccessCondition);
      BlobHttpRequestMessageFactory.AddCopySource(requestMessage, sourceUri);
      BlobHttpRequestMessageFactory.AddSourceRange(requestMessage, offset, count);
      requestMessage.ApplySourceContentChecksumHeaders(sourceContentChecksum);
      BlobRequest.ApplyCustomerProvidedKeyOrEncryptionScope(requestMessage, options, false);
      return requestMessage;
    }

    public static StorageRequestMessage CopyFrom(
      Uri uri,
      int? timeout,
      Uri source,
      bool incrementalCopy,
      AccessCondition sourceAccessCondition,
      AccessCondition destAccessCondition,
      HttpContent content,
      OperationContext operationContext,
      ICanonicalizer canonicalizer,
      StorageCredentials credentials,
      BlobRequestOptions options)
    {
      return BlobHttpRequestMessageFactory.CopyFrom(uri, timeout, source, incrementalCopy, new PremiumPageBlobTier?(), new StandardBlobTier?(StandardBlobTier.Unknown), new RehydratePriority?(), sourceAccessCondition, destAccessCondition, content, operationContext, canonicalizer, credentials);
    }

    public static StorageRequestMessage CopyFrom(
      Uri uri,
      int? timeout,
      Uri source,
      bool incrementalCopy,
      PremiumPageBlobTier? premiumPageBlobTier,
      StandardBlobTier? standardBlockBlobTier,
      RehydratePriority? rehydratePriority,
      AccessCondition sourceAccessCondition,
      AccessCondition destAccessCondition,
      HttpContent content,
      OperationContext operationContext,
      ICanonicalizer canonicalizer,
      StorageCredentials credentials)
    {
      return BlobHttpRequestMessageFactory.CopyFrom(uri, timeout, source, new Checksum(), incrementalCopy, false, premiumPageBlobTier, standardBlockBlobTier, rehydratePriority, sourceAccessCondition, destAccessCondition, content, operationContext, canonicalizer, credentials);
    }

    public static StorageRequestMessage CopyFrom(
      Uri uri,
      int? timeout,
      Uri source,
      Checksum sourceContentChecksum,
      bool incrementalCopy,
      bool syncCopy,
      PremiumPageBlobTier? premiumPageBlobTier,
      StandardBlobTier? standardBlockBlobTier,
      RehydratePriority? rehydratePriority,
      AccessCondition sourceAccessCondition,
      AccessCondition destAccessCondition,
      HttpContent content,
      OperationContext operationContext,
      ICanonicalizer canonicalizer,
      StorageCredentials credentials)
    {
      if (!syncCopy && sourceContentChecksum.HasAny)
        throw new InvalidOperationException();
      UriQueryBuilder builder = (UriQueryBuilder) null;
      if (incrementalCopy)
      {
        builder = new UriQueryBuilder();
        builder.Add("comp", "incrementalcopy");
      }
      StorageRequestMessage requestMessage = HttpRequestMessageFactory.CreateRequestMessage(HttpMethod.Put, uri, timeout, builder, content, operationContext, canonicalizer, credentials);
      requestMessage.Headers.Add("x-ms-copy-source", source.AbsoluteUri);
      requestMessage.ApplyAccessCondition(destAccessCondition);
      requestMessage.ApplyAccessConditionToSource(sourceAccessCondition);
      if (premiumPageBlobTier.HasValue && standardBlockBlobTier.HasValue)
        throw new ArgumentOutOfRangeException(nameof (standardBlockBlobTier), "Cannot specify both page and block tiers at the same time.");
      if (rehydratePriority.HasValue)
        requestMessage.AddOptionalHeader("x-ms-rehydrate-priority", rehydratePriority.Value.ToString());
      if (premiumPageBlobTier.HasValue)
        requestMessage.Headers.Add("x-ms-access-tier", premiumPageBlobTier.Value.ToString());
      else if (standardBlockBlobTier.HasValue)
        requestMessage.Headers.Add("x-ms-access-tier", standardBlockBlobTier.Value.ToString());
      if (syncCopy)
        requestMessage.Headers.Add("x-ms-requires-sync", "true");
      requestMessage.ApplySourceContentChecksumHeaders(sourceContentChecksum);
      return requestMessage;
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

    public static StorageRequestMessage Get(
      Uri uri,
      int? timeout,
      DateTimeOffset? snapshot,
      AccessCondition accessCondition,
      HttpContent content,
      OperationContext operationContext,
      ICanonicalizer canonicalizer,
      StorageCredentials credentials)
    {
      UriQueryBuilder builder = new UriQueryBuilder();
      if (snapshot.HasValue)
        builder.Add(nameof (snapshot), Request.ConvertDateTimeToSnapshotString(snapshot.Value));
      StorageRequestMessage requestMessage = HttpRequestMessageFactory.CreateRequestMessage(HttpMethod.Get, uri, timeout, builder, content, operationContext, canonicalizer, credentials);
      requestMessage.ApplyAccessCondition(accessCondition);
      return requestMessage;
    }

    public static StorageRequestMessage Get(
      Uri uri,
      int? timeout,
      DateTimeOffset? snapshot,
      long? offset,
      long? count,
      ChecksumRequested rangeContentChecksumRequested,
      AccessCondition accessCondition,
      HttpContent content,
      OperationContext operationContext,
      ICanonicalizer canonicalizer,
      StorageCredentials credentials,
      BlobRequestOptions options)
    {
      if (offset.HasValue && offset.Value < 0L)
        CommonUtility.ArgumentOutOfRange(nameof (offset), (object) offset);
      rangeContentChecksumRequested.AssertInBounds(offset, count, 4194304, 4194304);
      StorageRequestMessage request = BlobHttpRequestMessageFactory.Get(uri, timeout, snapshot, accessCondition, content, operationContext, canonicalizer, credentials);
      BlobHttpRequestMessageFactory.AddRange(request, offset, count);
      request.ApplyRangeContentChecksumRequested(offset, rangeContentChecksumRequested);
      BlobRequest.ApplyCustomerProvidedKey(request, options?.CustomerProvidedKey, false);
      return request;
    }

    public static StorageRequestMessage GetAccountProperties(
      Uri uri,
      UriQueryBuilder builder,
      int? timeout,
      HttpContent content,
      OperationContext operationContext,
      ICanonicalizer canonicalizer,
      StorageCredentials credentials)
    {
      return HttpRequestMessageFactory.GetAccountProperties(uri, builder, timeout, content, operationContext, canonicalizer, credentials);
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

    public static StorageRequestMessage GetServiceStats(
      Uri uri,
      int? timeout,
      OperationContext operationContext,
      ICanonicalizer canonicalizer,
      StorageCredentials credentials)
    {
      return HttpRequestMessageFactory.GetServiceStats(uri, timeout, operationContext, canonicalizer, credentials);
    }

    public static StorageRequestMessage SetBlobTier(
      Uri uri,
      int? timeout,
      string blobTier,
      RehydratePriority? rehydratePriority,
      HttpContent content,
      OperationContext operationContext,
      ICanonicalizer canonicalizer,
      StorageCredentials credentials)
    {
      UriQueryBuilder builder = new UriQueryBuilder();
      builder.Add("comp", "tier");
      StorageRequestMessage requestMessage = HttpRequestMessageFactory.CreateRequestMessage(HttpMethod.Put, uri, timeout, builder, content, operationContext, canonicalizer, credentials);
      requestMessage.Headers.Add("x-ms-access-tier", blobTier);
      if (rehydratePriority.HasValue)
        requestMessage.AddOptionalHeader("x-ms-rehydrate-priority", rehydratePriority.Value.ToString());
      return requestMessage;
    }

    public static StorageRequestMessage PrepareBatchRequest(
      Uri uri,
      IBufferManager bufferManager,
      int? timeout,
      BatchOperation batchOperation,
      HttpContent content,
      OperationContext operationContext,
      ICanonicalizer canonicalizer,
      StorageCredentials credentials)
    {
      UriQueryBuilder builder = new UriQueryBuilder();
      builder.Add("comp", "batch");
      return HttpRequestMessageFactory.CreateRequestMessage(HttpMethod.Post, uri, timeout, builder, content, operationContext, canonicalizer, credentials);
    }

    public static HttpContent WriteBatchBody(
      CloudBlobClient client,
      RESTCommand<IList<BlobBatchSubOperationResponse>> cmd,
      BatchOperation batchOperation,
      OperationContext operationContext)
    {
      string boundary = "batch_" + batchOperation.BatchID;
      MultipartContent multipartContent = new MultipartContent("mixed", boundary);
      multipartContent.Headers.ContentType = MediaTypeHeaderValue.Parse("multipart/mixed; boundary=" + boundary);
      int num = 0;
      foreach (StorageRequestMessage request in batchOperation.Operations.Select<RESTCommand<NullType>, StorageRequestMessage>((Func<RESTCommand<NullType>, StorageRequestMessage>) (command => command.BuildRequest(command, command.StorageUri.PrimaryUri, new UriQueryBuilder(), command.BuildContent != null ? command.BuildContent(command, operationContext) : (HttpContent) null, command.ServerTimeoutInSeconds, operationContext))))
      {
        ExecutorBase.ApplyUserHeaders(operationContext, (HttpRequestMessage) request);
        StorageCredentials credentials1 = request.Credentials;
        if ((credentials1 != null ? (credentials1.IsSharedKey ? 1 : 0) : 0) != 0)
        {
          StorageAuthenticationHttpHandler.AddDateHeader(request);
          StorageAuthenticationHttpHandler.AddSharedKeyAuth(request);
        }
        else
        {
          StorageCredentials credentials2 = request.Credentials;
          if ((credentials2 != null ? (credentials2.IsSAS ? 1 : 0) : 0) != 0)
          {
            request.RequestUri = request.Credentials.TransformUri(request.RequestUri);
          }
          else
          {
            StorageCredentials credentials3 = request.Credentials;
            if ((credentials3 != null ? (credentials3.IsToken ? 1 : 0) : 0) != 0)
              StorageAuthenticationHttpHandler.AddTokenAuth(request);
          }
        }
        request.Headers.Remove("x-ms-x-ms-version");
        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.AppendLine(string.Format("{0} {1} HTTP/1.1", (object) request.Method, (object) request.RequestUri.PathAndQuery));
        stringBuilder.Append(request.Headers.ToString());
        if (request.Content != null)
        {
          stringBuilder.AppendLine();
          stringBuilder.Append(request.Content.ReadAsStringAsync().ConfigureAwait(false).GetAwaiter().GetResult());
        }
        StringContent content = new StringContent(stringBuilder.ToString());
        content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/http");
        content.Headers.TryAddWithoutValidation("Content-Transfer-Encoding", "binary");
        content.Headers.TryAddWithoutValidation("Content-ID", num++.ToString((IFormatProvider) CultureInfo.InvariantCulture));
        multipartContent.Add((HttpContent) content);
      }
      return (HttpContent) multipartContent;
    }
  }
}
