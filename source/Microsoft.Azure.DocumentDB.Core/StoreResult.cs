// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.StoreResult
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using Microsoft.Azure.Cosmos.Core.Trace;
using Microsoft.Azure.Documents.Routing;
using System;
using System.Globalization;
using System.Net;
using System.Runtime.ExceptionServices;
using System.Text;

namespace Microsoft.Azure.Documents
{
  internal sealed class StoreResult
  {
    private readonly StoreResponse storeResponse;
    private readonly DocumentClientException exception;
    private static bool UseSessionTokenHeader = VersionUtility.IsLaterThan(HttpConstants.Versions.CurrentVersion, HttpConstants.Versions.v2018_06_18);

    public static StoreResult CreateStoreResult(
      StoreResponse storeResponse,
      Exception responseException,
      bool requiresValidLsn,
      bool useLocalLSNBasedHeaders,
      Uri storePhysicalAddress = null)
    {
      if (storeResponse == null && responseException == null)
        throw new ArgumentException("storeResponse or responseException must be populated.");
      if (responseException == null)
      {
        string str = (string) null;
        long quorumAckedLsn = -1;
        int currentReplicaSetSize = -1;
        int currentWriteQuorum = -1;
        long globalCommittedLSN = -1;
        int numberOfReadRegions = -1;
        long itemLSN = -1;
        if (storeResponse.TryGetHeaderValue(useLocalLSNBasedHeaders ? "x-ms-cosmos-quorum-acked-llsn" : "x-ms-quorum-acked-lsn", out str))
          quorumAckedLsn = long.Parse(str, (IFormatProvider) CultureInfo.InvariantCulture);
        if (storeResponse.TryGetHeaderValue("x-ms-current-replica-set-size", out str))
          currentReplicaSetSize = int.Parse(str, (IFormatProvider) CultureInfo.InvariantCulture);
        if (storeResponse.TryGetHeaderValue("x-ms-current-write-quorum", out str))
          currentWriteQuorum = int.Parse(str, (IFormatProvider) CultureInfo.InvariantCulture);
        double requestCharge = 0.0;
        if (storeResponse.TryGetHeaderValue("x-ms-request-charge", out str))
          requestCharge = double.Parse(str, (IFormatProvider) CultureInfo.InvariantCulture);
        if (storeResponse.TryGetHeaderValue("x-ms-number-of-read-regions", out str))
          numberOfReadRegions = int.Parse(str, (IFormatProvider) CultureInfo.InvariantCulture);
        if (storeResponse.TryGetHeaderValue("x-ms-global-Committed-lsn", out str))
          globalCommittedLSN = long.Parse(str, (IFormatProvider) CultureInfo.InvariantCulture);
        if (storeResponse.TryGetHeaderValue(useLocalLSNBasedHeaders ? "x-ms-cosmos-item-llsn" : "x-ms-item-lsn", out str))
          itemLSN = long.Parse(str, (IFormatProvider) CultureInfo.InvariantCulture);
        long lsn = -1;
        if (useLocalLSNBasedHeaders)
        {
          if (storeResponse.TryGetHeaderValue("x-ms-cosmos-llsn", out str))
            lsn = long.Parse(str, (IFormatProvider) CultureInfo.InvariantCulture);
        }
        else
          lsn = storeResponse.LSN;
        ISessionToken sessionToken = (ISessionToken) null;
        if (StoreResult.UseSessionTokenHeader)
        {
          if (storeResponse.TryGetHeaderValue("x-ms-session-token", out str))
            sessionToken = SessionTokenHelper.Parse(str);
        }
        else
          sessionToken = (ISessionToken) new SimpleSessionToken(storeResponse.LSN);
        return new StoreResult(storeResponse, (DocumentClientException) null, storeResponse.PartitionKeyRangeId, lsn, quorumAckedLsn, requestCharge, currentReplicaSetSize, currentWriteQuorum, true, storePhysicalAddress, globalCommittedLSN, numberOfReadRegions, itemLSN, sessionToken, useLocalLSNBasedHeaders);
      }
      if (responseException is DocumentClientException ex)
      {
        StoreResult.VerifyCanContinueOnException(ex);
        long num1 = -1;
        int num2 = -1;
        int num3 = -1;
        long num4 = -1;
        int num5 = -1;
        string header1 = ex.Headers[useLocalLSNBasedHeaders ? "x-ms-cosmos-quorum-acked-llsn" : "x-ms-quorum-acked-lsn"];
        if (!string.IsNullOrEmpty(header1))
          num1 = long.Parse(header1, (IFormatProvider) CultureInfo.InvariantCulture);
        string header2 = ex.Headers["x-ms-current-replica-set-size"];
        if (!string.IsNullOrEmpty(header2))
          num2 = int.Parse(header2, (IFormatProvider) CultureInfo.InvariantCulture);
        string header3 = ex.Headers["x-ms-current-write-quorum"];
        if (!string.IsNullOrEmpty(header3))
          num3 = int.Parse(header3, (IFormatProvider) CultureInfo.InvariantCulture);
        double num6 = 0.0;
        string header4 = ex.Headers["x-ms-request-charge"];
        if (!string.IsNullOrEmpty(header4))
          num6 = double.Parse(header4, (IFormatProvider) CultureInfo.InvariantCulture);
        string header5 = ex.Headers["x-ms-number-of-read-regions"];
        if (!string.IsNullOrEmpty(header5))
          num5 = int.Parse(header5, (IFormatProvider) CultureInfo.InvariantCulture);
        string header6 = ex.Headers["x-ms-global-Committed-lsn"];
        if (!string.IsNullOrEmpty(header6))
          num4 = long.Parse(header6, (IFormatProvider) CultureInfo.InvariantCulture);
        long num7 = -1;
        if (useLocalLSNBasedHeaders)
        {
          string header7 = ex.Headers["x-ms-cosmos-llsn"];
          if (!string.IsNullOrEmpty(header7))
            num7 = long.Parse(header7, (IFormatProvider) CultureInfo.InvariantCulture);
        }
        else
          num7 = ex.LSN;
        ISessionToken sessionToken1 = (ISessionToken) null;
        if (StoreResult.UseSessionTokenHeader)
        {
          string header8 = ex.Headers["x-ms-session-token"];
          if (!string.IsNullOrEmpty(header8))
            sessionToken1 = SessionTokenHelper.Parse(header8);
        }
        else
          sessionToken1 = (ISessionToken) new SimpleSessionToken(ex.LSN);
        DocumentClientException exception = ex;
        string partitionKeyRangeId = ex.PartitionKeyRangeId;
        long lsn = num7;
        long quorumAckedLsn = num1;
        double requestCharge = num6;
        int currentReplicaSetSize = num2;
        int currentWriteQuorum = num3;
        int num8;
        if (requiresValidLsn)
        {
          HttpStatusCode? statusCode = ex.StatusCode;
          HttpStatusCode httpStatusCode = HttpStatusCode.Gone;
          num8 = !(statusCode.GetValueOrDefault() == httpStatusCode & statusCode.HasValue) || ex.GetSubStatus() == SubStatusCodes.NameCacheIsStale ? (num7 >= 0L ? 1 : 0) : 0;
        }
        else
          num8 = 1;
        Uri storePhysicalAddress1 = storePhysicalAddress == (Uri) null ? ex.RequestUri : storePhysicalAddress;
        long globalCommittedLSN = num4;
        int numberOfReadRegions = num5;
        ISessionToken sessionToken2 = sessionToken1;
        int num9 = useLocalLSNBasedHeaders ? 1 : 0;
        return new StoreResult((StoreResponse) null, exception, partitionKeyRangeId, lsn, quorumAckedLsn, requestCharge, currentReplicaSetSize, currentWriteQuorum, num8 != 0, storePhysicalAddress1, globalCommittedLSN, numberOfReadRegions, -1L, sessionToken2, num9 != 0);
      }
      DefaultTrace.TraceCritical("Unexpected exception {0} received while reading from store.", (object) responseException);
      return new StoreResult((StoreResponse) null, (DocumentClientException) new InternalServerErrorException(RMResources.InternalServerError, responseException), (string) null, -1L, -1L, 0.0, 0, 0, false, storePhysicalAddress, -1L, 0, -1L, (ISessionToken) null, useLocalLSNBasedHeaders);
    }

    public StoreResult(
      StoreResponse storeResponse,
      DocumentClientException exception,
      string partitionKeyRangeId,
      long lsn,
      long quorumAckedLsn,
      double requestCharge,
      int currentReplicaSetSize,
      int currentWriteQuorum,
      bool isValid,
      Uri storePhysicalAddress,
      long globalCommittedLSN,
      int numberOfReadRegions,
      long itemLSN,
      ISessionToken sessionToken,
      bool usingLocalLSN)
    {
      this.storeResponse = storeResponse != null || exception != null ? storeResponse : throw new ArgumentException("storeResponse or responseException must be populated.");
      this.exception = exception;
      this.PartitionKeyRangeId = partitionKeyRangeId;
      this.LSN = lsn;
      this.QuorumAckedLSN = quorumAckedLsn;
      this.RequestCharge = requestCharge;
      this.CurrentReplicaSetSize = currentReplicaSetSize;
      this.CurrentWriteQuorum = currentWriteQuorum;
      this.IsValid = isValid;
      this.StorePhysicalAddress = storePhysicalAddress;
      this.GlobalCommittedLSN = globalCommittedLSN;
      this.NumberOfReadRegions = (long) numberOfReadRegions;
      this.ItemLSN = itemLSN;
      this.SessionToken = sessionToken;
      this.UsingLocalLSN = usingLocalLSN;
      this.StatusCode = (StatusCodes) (this.storeResponse != null ? new HttpStatusCode?(this.storeResponse.StatusCode) : (this.exception == null || !this.exception.StatusCode.HasValue ? new HttpStatusCode?((HttpStatusCode) 0) : this.exception.StatusCode)).Value;
      this.SubStatusCode = this.storeResponse != null ? this.storeResponse.SubStatusCode : (this.exception != null ? this.exception.GetSubStatus() : SubStatusCodes.Unknown);
    }

    public long LSN { get; private set; }

    public string PartitionKeyRangeId { get; private set; }

    public long QuorumAckedLSN { get; private set; }

    public long GlobalCommittedLSN { get; private set; }

    public long NumberOfReadRegions { get; private set; }

    public long ItemLSN { get; private set; }

    public ISessionToken SessionToken { get; private set; }

    public bool UsingLocalLSN { get; private set; }

    public double RequestCharge { get; private set; }

    public int CurrentReplicaSetSize { get; private set; }

    public int CurrentWriteQuorum { get; private set; }

    public bool IsValid { get; private set; }

    public Uri StorePhysicalAddress { get; private set; }

    public StatusCodes StatusCode { get; private set; }

    public SubStatusCodes SubStatusCode { get; private set; }

    public bool IsClientCpuOverloaded => this.exception?.InnerException is TransportException innerException && innerException.IsClientCpuOverloaded;

    public DocumentClientException GetException()
    {
      if (this.exception == null)
      {
        DefaultTrace.TraceCritical("Exception should be available but found none");
        throw new InternalServerErrorException(RMResources.InternalServerError);
      }
      return this.exception;
    }

    public StoreResponse ToResponse(RequestChargeTracker requestChargeTracker = null)
    {
      if (!this.IsValid)
      {
        if (this.exception == null)
        {
          DefaultTrace.TraceCritical("Exception not set for invalid response");
          throw new InternalServerErrorException(RMResources.InternalServerError);
        }
        throw this.exception;
      }
      if (requestChargeTracker != null)
        StoreResult.SetRequestCharge(this.storeResponse, this.exception, requestChargeTracker.TotalRequestCharge);
      if (this.exception != null)
        throw this.exception;
      return this.storeResponse;
    }

    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder();
      this.AppendToBuilder(stringBuilder);
      return stringBuilder.ToString();
    }

    public void AppendToBuilder(StringBuilder stringBuilder)
    {
      if (stringBuilder == null)
        throw new ArgumentNullException(nameof (stringBuilder));
      stringBuilder.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, "StorePhysicalAddress: {0}, LSN: {1}, GlobalCommittedLsn: {2}, PartitionKeyRangeId: {3}, IsValid: {4}, StatusCode: {5}, SubStatusCode: {6}, RequestCharge: {7}, ItemLSN: {8}, SessionToken: {9}, UsingLocalLSN: {10}, TransportException: {11}", (object) this.StorePhysicalAddress, (object) this.LSN, (object) this.GlobalCommittedLSN, (object) this.PartitionKeyRangeId, (object) this.IsValid, (object) (int) this.StatusCode, (object) (int) this.SubStatusCode, (object) this.RequestCharge, (object) this.ItemLSN, (object) this.SessionToken?.ConvertToString(), (object) this.UsingLocalLSN, this.exception?.InnerException is TransportException ? (object) this.exception.InnerException.Message : (object) "null");
    }

    private static void SetRequestCharge(
      StoreResponse response,
      DocumentClientException documentClientException,
      double totalRequestCharge)
    {
      if (documentClientException != null)
      {
        documentClientException.Headers["x-ms-request-charge"] = totalRequestCharge.ToString((IFormatProvider) CultureInfo.InvariantCulture);
      }
      else
      {
        if (response.ResponseHeaderNames == null)
          return;
        for (int index = 0; index < response.ResponseHeaderNames.Length; ++index)
        {
          if (string.Equals(response.ResponseHeaderNames[index], "x-ms-request-charge", StringComparison.OrdinalIgnoreCase))
          {
            response.ResponseHeaderValues[index] = totalRequestCharge.ToString((IFormatProvider) CultureInfo.InvariantCulture);
            break;
          }
        }
      }
    }

    private static void VerifyCanContinueOnException(DocumentClientException ex)
    {
      switch (ex)
      {
        case PartitionKeyRangeGoneException _:
        case PartitionKeyRangeIsSplittingException _:
        case PartitionIsMigratingException _:
          ExceptionDispatchInfo.Capture((Exception) ex).Throw();
          break;
      }
      int result;
      if (string.IsNullOrWhiteSpace(ex.Headers["x-ms-request-validation-failure"]) || !int.TryParse(ex.Headers.GetValues("x-ms-request-validation-failure")[0], NumberStyles.Integer, (IFormatProvider) CultureInfo.InvariantCulture, out result) || result != 1)
        return;
      ExceptionDispatchInfo.Capture((Exception) ex).Throw();
    }
  }
}
