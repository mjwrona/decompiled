// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Storage.Shared.Protocol.RequestMessageExtensions
// Assembly: Microsoft.Azure.Storage.Common, Version=11.2.3.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 0978DA65-6954-4A99-9ACB-2EF3D979A5D5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Storage.Common.dll

using Microsoft.Azure.Storage.Core;
using Microsoft.Azure.Storage.Core.Util;
using System;
using System.Net.Http.Headers;

namespace Microsoft.Azure.Storage.Shared.Protocol
{
  internal static class RequestMessageExtensions
  {
    internal static void AddLeaseId(this StorageRequestMessage request, string leaseId)
    {
      if (leaseId == null)
        return;
      request.AddOptionalHeader("x-ms-lease-id", leaseId);
    }

    internal static void AddOptionalHeader(
      this StorageRequestMessage request,
      string name,
      string value)
    {
      if (string.IsNullOrEmpty(value))
        return;
      request.Headers.Add(name, value);
    }

    internal static void AddOptionalHeader(
      this StorageRequestMessage request,
      string name,
      int? value)
    {
      if (!value.HasValue)
        return;
      request.Headers.Add(name, value.Value.ToString());
    }

    internal static void AddOptionalHeader(
      this StorageRequestMessage request,
      string name,
      long? value)
    {
      if (!value.HasValue)
        return;
      request.Headers.Add(name, value.Value.ToString());
    }

    internal static void ApplySourceContentChecksumHeaders(
      this StorageRequestMessage request,
      Checksum sourceContentChecksum)
    {
      request.AddOptionalHeader("x-ms-source-content-md5", sourceContentChecksum?.MD5);
      request.AddOptionalHeader("x-ms-source-content-crc64", sourceContentChecksum?.CRC64);
    }

    internal static void ApplyRangeContentChecksumRequested(
      this StorageRequestMessage request,
      long? offset,
      ChecksumRequested rangeChecksumRequested)
    {
      if (offset.HasValue && rangeChecksumRequested.MD5)
        request.Headers.Add("x-ms-range-get-content-md5", "true");
      if (!offset.HasValue || !rangeChecksumRequested.CRC64)
        return;
      request.Headers.Add("x-ms-range-get-content-crc64", "true");
    }

    internal static void ApplyLeaseId(
      this StorageRequestMessage request,
      AccessCondition accessCondition)
    {
      if (accessCondition == null || string.IsNullOrEmpty(accessCondition.LeaseId))
        return;
      request.Headers.Add("x-ms-lease-id", accessCondition.LeaseId);
    }

    internal static void ApplySequenceNumberCondition(
      this StorageRequestMessage request,
      AccessCondition accessCondition)
    {
      if (accessCondition == null)
        return;
      request.AddOptionalHeader("x-ms-if-sequence-number-le", accessCondition.IfSequenceNumberLessThanOrEqual);
      request.AddOptionalHeader("x-ms-if-sequence-number-lt", accessCondition.IfSequenceNumberLessThan);
      request.AddOptionalHeader("x-ms-if-sequence-number-eq", accessCondition.IfSequenceNumberEqual);
    }

    internal static void ApplyAccessCondition(
      this StorageRequestMessage request,
      AccessCondition accessCondition)
    {
      if (accessCondition == null)
        return;
      if (!string.IsNullOrEmpty(accessCondition.IfMatchETag))
      {
        if (accessCondition.IfMatchETag.Equals(EntityTagHeaderValue.Any.ToString(), StringComparison.OrdinalIgnoreCase))
          request.Headers.IfMatch.Add(EntityTagHeaderValue.Any);
        else
          request.Headers.IfMatch.Add(EntityTagHeaderValue.Parse(accessCondition.IfMatchETag));
      }
      if (!string.IsNullOrEmpty(accessCondition.IfNoneMatchETag))
      {
        if (accessCondition.IfNoneMatchETag.Equals(EntityTagHeaderValue.Any.ToString(), StringComparison.OrdinalIgnoreCase))
          request.Headers.IfNoneMatch.Add(EntityTagHeaderValue.Any);
        else
          request.Headers.IfNoneMatch.Add(EntityTagHeaderValue.Parse(accessCondition.IfNoneMatchETag));
      }
      request.Headers.IfModifiedSince = accessCondition.IfModifiedSinceTime;
      request.Headers.IfUnmodifiedSince = accessCondition.IfNotModifiedSinceTime;
      request.ApplyLeaseId(accessCondition);
    }

    internal static void ApplyAppendCondition(
      this StorageRequestMessage request,
      AccessCondition accessCondition)
    {
      if (accessCondition == null)
        return;
      request.AddOptionalHeader("x-ms-blob-condition-maxsize", accessCondition.IfMaxSizeLessThanOrEqual);
      request.AddOptionalHeader("x-ms-blob-condition-appendpos", accessCondition.IfAppendPositionEqual);
    }

    internal static void ApplyAccessConditionToSource(
      this StorageRequestMessage request,
      AccessCondition accessCondition)
    {
      if (accessCondition == null)
        return;
      if (!string.IsNullOrEmpty(accessCondition.IfMatchETag))
        request.Headers.Add("x-ms-source-if-match", accessCondition.IfMatchETag);
      if (!string.IsNullOrEmpty(accessCondition.IfNoneMatchETag))
        request.Headers.Add("x-ms-source-if-none-match", accessCondition.IfNoneMatchETag);
      if (!string.IsNullOrEmpty(accessCondition.IfMatchContentCrc))
        request.Headers.Add("x-ms-source-if-match-crc64", accessCondition.IfMatchContentCrc);
      if (!string.IsNullOrEmpty(accessCondition.IfNoneMatchContentCrc))
        request.Headers.Add("x-ms-source-if-none-match-crc64", accessCondition.IfNoneMatchContentCrc);
      DateTimeOffset? modifiedSinceTime;
      if (accessCondition.IfModifiedSinceTime.HasValue)
      {
        HttpRequestHeaders headers = request.Headers;
        modifiedSinceTime = accessCondition.IfModifiedSinceTime;
        string httpString = HttpWebUtility.ConvertDateTimeToHttpString(modifiedSinceTime.Value);
        headers.Add("x-ms-source-if-modified-since", httpString);
      }
      modifiedSinceTime = accessCondition.IfNotModifiedSinceTime;
      if (modifiedSinceTime.HasValue)
      {
        HttpRequestHeaders headers = request.Headers;
        modifiedSinceTime = accessCondition.IfNotModifiedSinceTime;
        string httpString = HttpWebUtility.ConvertDateTimeToHttpString(modifiedSinceTime.Value);
        headers.Add("x-ms-source-if-unmodified-since", httpString);
      }
      if (!string.IsNullOrEmpty(accessCondition.LeaseId))
        throw new InvalidOperationException("A lease condition cannot be specified on the source of a copy.");
    }
  }
}
