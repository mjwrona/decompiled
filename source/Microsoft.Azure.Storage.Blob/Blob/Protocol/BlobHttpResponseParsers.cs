// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Storage.Blob.Protocol.BlobHttpResponseParsers
// Assembly: Microsoft.Azure.Storage.Blob, Version=11.2.3.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: A04A3512-352A-442F-A95B-BC1B94EF8840
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Storage.Blob.dll

using Microsoft.Azure.Storage.Core.Executor;
using Microsoft.Azure.Storage.Core.Util;
using Microsoft.Azure.Storage.Shared.Protocol;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Storage.Blob.Protocol
{
  public static class BlobHttpResponseParsers
  {
    public static BlobProperties GetProperties(HttpResponseMessage response)
    {
      CommonUtility.AssertNotNull(nameof (response), (object) response);
      BlobProperties properties = new BlobProperties();
      if (response.Content != null)
      {
        properties.LastModified = response.Content.Headers.LastModified;
        HttpContentHeaders headers = response.Content.Headers;
        properties.ContentEncoding = HttpWebUtility.GetHeaderValues("Content-Encoding", (HttpHeaders) headers);
        properties.ContentLanguage = HttpWebUtility.GetHeaderValues("Content-Language", (HttpHeaders) headers);
        properties.ContentDisposition = HttpWebUtility.GetHeaderValues("Content-Disposition", (HttpHeaders) headers);
        properties.ContentType = HttpWebUtility.GetHeaderValues("Content-Type", (HttpHeaders) headers);
        if (response.Content.Headers.ContentMD5 != null && response.Content.Headers.ContentRange == null)
          properties.ContentChecksum.MD5 = HttpResponseParsers.GetContentMD5(response);
        else if (!string.IsNullOrEmpty(response.Headers.GetHeaderSingleValueOrDefault("x-ms-blob-content-md5")))
          properties.ContentChecksum.MD5 = response.Headers.GetHeaderSingleValueOrDefault("x-ms-blob-content-md5");
        if (!string.IsNullOrEmpty(response.Headers.GetHeaderSingleValueOrDefault("x-ms-blob-content-crc64")))
          properties.ContentChecksum.CRC64 = response.Headers.GetHeaderSingleValueOrDefault("x-ms-blob-content-crc64");
        string singleValueOrDefault1 = response.Headers.GetHeaderSingleValueOrDefault("x-ms-creation-time");
        properties.Created = string.IsNullOrEmpty(singleValueOrDefault1) ? new DateTimeOffset?() : new DateTimeOffset?(DateTimeOffset.Parse(singleValueOrDefault1, (IFormatProvider) CultureInfo.InvariantCulture).ToUniversalTime());
        string singleValueOrDefault2 = response.Headers.GetHeaderSingleValueOrDefault("x-ms-server-encrypted");
        properties.IsServerEncrypted = string.Equals(singleValueOrDefault2, "true", StringComparison.OrdinalIgnoreCase);
        string singleValueOrDefault3 = response.Headers.GetHeaderSingleValueOrDefault("x-ms-encryption-scope");
        if (!string.IsNullOrEmpty(singleValueOrDefault3))
          properties.EncryptionScope = singleValueOrDefault3;
        string singleValueOrDefault4 = response.Headers.GetHeaderSingleValueOrDefault("x-ms-incremental-copy");
        properties.IsIncrementalCopy = string.Equals(singleValueOrDefault4, "true", StringComparison.OrdinalIgnoreCase);
        string singleValueOrDefault5 = response.Headers.GetHeaderSingleValueOrDefault("x-ms-blob-content-length");
        if (response.Content.Headers.ContentRange != null && response.Content.Headers.ContentRange.HasLength)
          properties.Length = response.Content.Headers.ContentRange.Length.Value;
        else if (!string.IsNullOrEmpty(singleValueOrDefault5))
          properties.Length = long.Parse(singleValueOrDefault5);
        else if (response.Content.Headers.ContentLength.HasValue)
          properties.Length = response.Content.Headers.ContentLength.Value;
      }
      properties.CacheControl = HttpWebUtility.GetHeaderValues("Cache-Control", (HttpHeaders) response.Headers);
      if (response.Headers.ETag != null)
        properties.ETag = response.Headers.ETag.ToString();
      string singleValueOrDefault6 = response.Headers.GetHeaderSingleValueOrDefault("x-ms-blob-type");
      if (!string.IsNullOrEmpty(singleValueOrDefault6))
        properties.BlobType = (BlobType) Enum.Parse(typeof (BlobType), singleValueOrDefault6, true);
      properties.LeaseStatus = BlobHttpResponseParsers.GetLeaseStatus(response);
      properties.LeaseState = BlobHttpResponseParsers.GetLeaseState(response);
      properties.LeaseDuration = BlobHttpResponseParsers.GetLeaseDuration(response);
      string singleValueOrDefault7 = response.Headers.GetHeaderSingleValueOrDefault("x-ms-blob-sequence-number");
      if (!string.IsNullOrEmpty(singleValueOrDefault7))
        properties.PageBlobSequenceNumber = new long?(long.Parse(singleValueOrDefault7, (IFormatProvider) CultureInfo.InvariantCulture));
      string singleValueOrDefault8 = response.Headers.GetHeaderSingleValueOrDefault("x-ms-blob-committed-block-count");
      if (!string.IsNullOrEmpty(singleValueOrDefault8))
        properties.AppendBlobCommittedBlockCount = new int?(int.Parse(singleValueOrDefault8, (IFormatProvider) CultureInfo.InvariantCulture));
      string singleValueOrDefault9 = response.Headers.GetHeaderSingleValueOrDefault("x-ms-access-tier-inferred");
      if (!string.IsNullOrEmpty(singleValueOrDefault9))
        properties.BlobTierInferred = new bool?(Convert.ToBoolean(singleValueOrDefault9));
      string singleValueOrDefault10 = response.Headers.GetHeaderSingleValueOrDefault("x-ms-access-tier");
      StandardBlobTier? standardBlobTier;
      PremiumPageBlobTier? premiumPageBlobTier;
      BlobHttpResponseParsers.GetBlobTier(properties.BlobType, singleValueOrDefault10, out standardBlobTier, out premiumPageBlobTier);
      properties.StandardBlobTier = standardBlobTier;
      properties.PremiumPageBlobTier = premiumPageBlobTier;
      string singleValueOrDefault11 = response.Headers.GetHeaderSingleValueOrDefault("x-ms-archive-status");
      if (!string.IsNullOrEmpty(singleValueOrDefault11))
        properties.RehydrationStatus = !"rehydrate-pending-to-hot".Equals(singleValueOrDefault11) ? (!"rehydrate-pending-to-cool".Equals(singleValueOrDefault11) ? new RehydrationStatus?(RehydrationStatus.Unknown) : new RehydrationStatus?(RehydrationStatus.PendingToCool)) : new RehydrationStatus?(RehydrationStatus.PendingToHot);
      if ((properties.PremiumPageBlobTier.HasValue || properties.StandardBlobTier.HasValue) && !properties.BlobTierInferred.HasValue)
        properties.BlobTierInferred = new bool?(false);
      string singleValueOrDefault12 = response.Headers.GetHeaderSingleValueOrDefault("x-ms-access-tier-change-time");
      if (!string.IsNullOrEmpty(singleValueOrDefault12))
        properties.BlobTierLastModifiedTime = new DateTimeOffset?(DateTimeOffset.Parse(singleValueOrDefault12, (IFormatProvider) CultureInfo.InvariantCulture));
      return properties;
    }

    public static LeaseStatus GetLeaseStatus(HttpResponseMessage response)
    {
      CommonUtility.AssertNotNull(nameof (response), (object) response);
      return BlobHttpResponseParsers.GetLeaseStatus(response.Headers.GetHeaderSingleValueOrDefault("x-ms-lease-status"));
    }

    public static LeaseState GetLeaseState(HttpResponseMessage response)
    {
      CommonUtility.AssertNotNull(nameof (response), (object) response);
      return BlobHttpResponseParsers.GetLeaseState(response.Headers.GetHeaderSingleValueOrDefault("x-ms-lease-state"));
    }

    public static LeaseDuration GetLeaseDuration(HttpResponseMessage response)
    {
      CommonUtility.AssertNotNull(nameof (response), (object) response);
      return BlobHttpResponseParsers.GetLeaseDuration(response.Headers.GetHeaderSingleValueOrDefault("x-ms-lease-duration"));
    }

    public static string GetLeaseId(HttpResponseMessage response)
    {
      CommonUtility.AssertNotNull(nameof (response), (object) response);
      return response.Headers.GetHeaderSingleValueOrDefault("x-ms-lease-id");
    }

    public static int? GetRemainingLeaseTime(HttpResponseMessage response)
    {
      CommonUtility.AssertNotNull(nameof (response), (object) response);
      int result;
      return int.TryParse(response.Headers.GetHeaderSingleValueOrDefault("x-ms-lease-time"), out result) ? new int?(result) : new int?();
    }

    public static IDictionary<string, string> GetMetadata(HttpResponseMessage response) => HttpResponseParsers.GetMetadata(response);

    public static CopyState GetCopyAttributes(HttpResponseMessage response)
    {
      CommonUtility.AssertNotNull(nameof (response), (object) response);
      string singleValueOrDefault = response.Headers.GetHeaderSingleValueOrDefault("x-ms-copy-status");
      return !string.IsNullOrEmpty(singleValueOrDefault) ? BlobHttpResponseParsers.GetCopyAttributes(singleValueOrDefault, response.Headers.GetHeaderSingleValueOrDefault("x-ms-copy-id"), response.Headers.GetHeaderSingleValueOrDefault("x-ms-copy-source"), response.Headers.GetHeaderSingleValueOrDefault("x-ms-copy-progress"), response.Headers.GetHeaderSingleValueOrDefault("x-ms-copy-completion-time"), response.Headers.GetHeaderSingleValueOrDefault("x-ms-copy-status-description"), response.Headers.GetHeaderSingleValueOrDefault("x-ms-copy-destination-snapshot")) : (CopyState) null;
    }

    public static string GetSnapshotTime(HttpResponseMessage response)
    {
      CommonUtility.AssertNotNull(nameof (response), (object) response);
      return response.Headers.GetHeaderSingleValueOrDefault("x-ms-snapshot");
    }

    public static AccountProperties ReadAccountProperties(HttpResponseMessage response) => HttpResponseParsers.ReadAccountProperties(response);

    public static Task<ServiceProperties> ReadServicePropertiesAsync(
      Stream inputStream,
      CancellationToken token)
    {
      return HttpResponseParsers.ReadServicePropertiesAsync(inputStream, token);
    }

    public static Task<ServiceStats> ReadServiceStatsAsync(
      Stream inputStream,
      CancellationToken token)
    {
      return HttpResponseParsers.ReadServiceStatsAsync(inputStream, token);
    }

    internal static LeaseStatus GetLeaseStatus(string leaseStatus)
    {
      if (string.IsNullOrEmpty(leaseStatus))
        return LeaseStatus.Unspecified;
      switch (leaseStatus)
      {
        case "locked":
          return LeaseStatus.Locked;
        case "unlocked":
          return LeaseStatus.Unlocked;
        default:
          throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Invalid lease status in response: '{0}'", (object) leaseStatus), nameof (leaseStatus));
      }
    }

    internal static LeaseState GetLeaseState(string leaseState)
    {
      if (string.IsNullOrEmpty(leaseState))
        return LeaseState.Unspecified;
      switch (leaseState)
      {
        case "available":
          return LeaseState.Available;
        case "leased":
          return LeaseState.Leased;
        case "expired":
          return LeaseState.Expired;
        case "breaking":
          return LeaseState.Breaking;
        case "broken":
          return LeaseState.Broken;
        default:
          throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Invalid lease state in response: '{0}'", (object) leaseState), nameof (leaseState));
      }
    }

    internal static LeaseDuration GetLeaseDuration(string leaseDuration)
    {
      if (string.IsNullOrEmpty(leaseDuration))
        return LeaseDuration.Unspecified;
      switch (leaseDuration)
      {
        case "fixed":
          return LeaseDuration.Fixed;
        case "infinite":
          return LeaseDuration.Infinite;
        default:
          throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Invalid lease duration in response: '{0}'", (object) leaseDuration), nameof (leaseDuration));
      }
    }

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

    public static bool GetServerEncrypted(string encryptionHeader) => BlobHttpResponseParsers.CheckIfTrue(encryptionHeader);

    public static bool GetIncrementalCopyStatus(string incrementalCopyHeader) => BlobHttpResponseParsers.CheckIfTrue(incrementalCopyHeader);

    public static bool GetDeletionStatus(string deletedHeader) => BlobHttpResponseParsers.CheckIfTrue(deletedHeader);

    private static bool CheckIfTrue(string header) => string.Equals(header, "true", StringComparison.OrdinalIgnoreCase);

    internal static void GetBlobTier(
      BlobType blobType,
      string blobTierString,
      out StandardBlobTier? standardBlobTier,
      out PremiumPageBlobTier? premiumPageBlobTier)
    {
      standardBlobTier = new StandardBlobTier?();
      premiumPageBlobTier = new PremiumPageBlobTier?();
      if (blobType.Equals((object) BlobType.BlockBlob))
      {
        StandardBlobTier result;
        if (Enum.TryParse<StandardBlobTier>(blobTierString, true, out result))
          standardBlobTier = new StandardBlobTier?(result);
        else
          standardBlobTier = new StandardBlobTier?(StandardBlobTier.Unknown);
      }
      else if (blobType.Equals((object) BlobType.PageBlob))
      {
        PremiumPageBlobTier result;
        if (Enum.TryParse<PremiumPageBlobTier>(blobTierString, true, out result))
          premiumPageBlobTier = new PremiumPageBlobTier?(result);
        else
          premiumPageBlobTier = new PremiumPageBlobTier?(PremiumPageBlobTier.Unknown);
      }
      else
      {
        if (!blobType.Equals((object) BlobType.Unspecified))
          return;
        StandardBlobTier result1;
        if (Enum.TryParse<StandardBlobTier>(blobTierString, true, out result1))
        {
          standardBlobTier = new StandardBlobTier?(result1);
        }
        else
        {
          PremiumPageBlobTier result2;
          if (Enum.TryParse<PremiumPageBlobTier>(blobTierString, true, out result2))
          {
            premiumPageBlobTier = new PremiumPageBlobTier?(result2);
          }
          else
          {
            standardBlobTier = new StandardBlobTier?(StandardBlobTier.Unknown);
            premiumPageBlobTier = new PremiumPageBlobTier?(PremiumPageBlobTier.Unknown);
          }
        }
      }
    }

    internal static async Task<IList<BlobBatchSubOperationResponse>> BatchPostProcessAsync(
      IList<BlobBatchSubOperationResponse> result,
      RESTCommand<IList<BlobBatchSubOperationResponse>> cmd,
      IList<HttpStatusCode> successfulStatusCodes,
      HttpResponseMessage response,
      OperationContext ctx,
      CancellationToken cancellationToken)
    {
      StreamReader streamReader = new StreamReader(cmd.ResponseStream);
      List<BlobBatchSubOperationError> errors = new List<BlobBatchSubOperationError>();
      string str1 = await streamReader.ReadLineAsync().ConfigureAwait(false);
      for (string str2 = await streamReader.ReadLineAsync().ConfigureAwait(false); str2 != null && (!str2.StartsWith("--batchresponse") || !str2.EndsWith("--")); str2 = await streamReader.ReadLineAsync().ConfigureAwait(false))
      {
        while (str2 != null && !str2.StartsWith("Content-ID"))
          str2 = await streamReader.ReadLineAsync().ConfigureAwait(false);
        int operationIndex = int.Parse(str2.Split(':')[1]);
        while (str2 != null && !str2.StartsWith("HTTP"))
          str2 = await streamReader.ReadLineAsync().ConfigureAwait(false);
        HttpStatusCode statusCode = (HttpStatusCode) int.Parse(str2.Substring(9, 3));
        string currentLine = await streamReader.ReadLineAsync().ConfigureAwait(false);
        ConfiguredTaskAwaitable<Dictionary<string, string>> configuredTaskAwaitable;
        if (successfulStatusCodes.Contains(statusCode))
        {
          BlobBatchSubOperationResponse operationResponse1 = new BlobBatchSubOperationResponse();
          operationResponse1.StatusCode = statusCode;
          operationResponse1.OperationIndex = operationIndex;
          BlobBatchSubOperationResponse operationResponse2 = operationResponse1;
          configuredTaskAwaitable = BlobHttpResponseParsers.ParseSubRequestHeadersAsync(currentLine, streamReader).ConfigureAwait(false);
          operationResponse2.Headers = await configuredTaskAwaitable;
          BlobBatchSubOperationResponse operationResponse = operationResponse1;
          operationResponse2 = (BlobBatchSubOperationResponse) null;
          operationResponse1 = (BlobBatchSubOperationResponse) null;
          result.Add(operationResponse);
        }
        else
        {
          BlobBatchSubOperationError error = new BlobBatchSubOperationError()
          {
            OperationIndex = operationIndex,
            StatusCode = statusCode
          };
          configuredTaskAwaitable = BlobHttpResponseParsers.ParseSubRequestHeadersAsync(currentLine, streamReader).ConfigureAwait(false);
          Dictionary<string, string> dictionary = await configuredTaskAwaitable;
          error.ErrorCode = dictionary["x-ms-error-code"];
          int length = int.Parse(dictionary["Content-Length"]);
          char[] chars = new char[length];
          int num = 0;
          while (num < length)
            chars[num++] = (char) streamReader.Read();
          BlobBatchSubOperationError subOperationError = error;
          subOperationError.ExtendedErrorInformation = await StorageExtendedErrorInformation.ReadFromStreamAsync((Stream) new MemoryStream(Encoding.UTF8.GetBytes(chars))).ConfigureAwait(false);
          subOperationError = (BlobBatchSubOperationError) null;
          errors.Add(error);
          error = (BlobBatchSubOperationError) null;
        }
      }
      if (errors.Count > 0)
        throw new BlobBatchException()
        {
          ErrorResponses = (IList<BlobBatchSubOperationError>) errors,
          SuccessfulResponses = result
        };
      IList<BlobBatchSubOperationResponse> operationResponseList = result;
      streamReader = (StreamReader) null;
      errors = (List<BlobBatchSubOperationError>) null;
      return operationResponseList;
    }

    private static async Task<Dictionary<string, string>> ParseSubRequestHeadersAsync(
      string currentLine,
      StreamReader streamReader)
    {
      Dictionary<string, string> headers = new Dictionary<string, string>();
      for (; !string.IsNullOrWhiteSpace(currentLine); currentLine = await streamReader.ReadLineAsync().ConfigureAwait(false))
      {
        int length = currentLine.IndexOf(':');
        headers[currentLine.Substring(0, length)] = currentLine.Substring(length + 2);
      }
      Dictionary<string, string> requestHeadersAsync = headers;
      headers = (Dictionary<string, string>) null;
      return requestHeadersAsync;
    }

    internal static RehydrationStatus? GetRehydrationStatus(string rehydrationStatus)
    {
      if (string.IsNullOrEmpty(rehydrationStatus))
        return new RehydrationStatus?();
      if ("rehydrate-pending-to-hot".Equals(rehydrationStatus))
        return new RehydrationStatus?(RehydrationStatus.PendingToHot);
      return "rehydrate-pending-to-cool".Equals(rehydrationStatus) ? new RehydrationStatus?(RehydrationStatus.PendingToCool) : new RehydrationStatus?(RehydrationStatus.Unknown);
    }
  }
}
