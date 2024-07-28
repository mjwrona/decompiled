// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.QuorumReader
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using Microsoft.Azure.Cosmos.Core.Trace;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;

namespace Microsoft.Azure.Documents
{
  internal sealed class QuorumReader
  {
    private const int maxNumberOfReadBarrierReadRetries = 6;
    private const int maxNumberOfPrimaryReadRetries = 6;
    private const int maxNumberOfReadQuorumRetries = 6;
    private const int delayBetweenReadBarrierCallsInMs = 5;
    private const int maxBarrierRetriesForMultiRegion = 30;
    private const int barrierRetryIntervalInMsForMultiRegion = 30;
    private const int maxShortBarrierRetriesForMultiRegion = 4;
    private const int shortbarrierRetryIntervalInMsForMultiRegion = 10;
    private readonly StoreReader storeReader;
    private readonly IServiceConfigurationReader serviceConfigReader;
    private readonly IAuthorizationTokenProvider authorizationTokenProvider;

    public QuorumReader(
      TransportClient transportClient,
      AddressSelector addressSelector,
      StoreReader storeReader,
      IServiceConfigurationReader serviceConfigReader,
      IAuthorizationTokenProvider authorizationTokenProvider)
    {
      this.storeReader = storeReader;
      this.serviceConfigReader = serviceConfigReader;
      this.authorizationTokenProvider = authorizationTokenProvider;
    }

    public async Task<StoreResponse> ReadStrongAsync(
      DocumentServiceRequest entity,
      int readQuorumValue,
      ReadMode readMode)
    {
      int readQuorumRetry = 6;
      bool shouldRetryOnSecondary = false;
      bool hasPerformedReadFromPrimary = false;
      do
      {
        entity.RequestContext.TimeoutHelper.ThrowGoneIfElapsed();
        shouldRetryOnSecondary = false;
        QuorumReader.ReadQuorumResult secondaryQuorumReadResult = await this.ReadQuorumAsync(entity, readQuorumValue, false, readMode);
        switch (secondaryQuorumReadResult.QuorumResult)
        {
          case QuorumReader.ReadQuorumResultKind.QuorumMet:
            return secondaryQuorumReadResult.GetResponse();
          case QuorumReader.ReadQuorumResultKind.QuorumSelected:
            if (await this.WaitForReadBarrierAsync(await BarrierRequestHelper.CreateAsync(entity, this.authorizationTokenProvider, new long?(secondaryQuorumReadResult.SelectedLsn), new long?(secondaryQuorumReadResult.GlobalCommittedSelectedLsn)), true, readQuorumValue, secondaryQuorumReadResult.SelectedLsn, secondaryQuorumReadResult.GlobalCommittedSelectedLsn, readMode))
              return secondaryQuorumReadResult.GetResponse();
            DefaultTrace.TraceWarning("QuorumSelected: Could not converge on the LSN {0} GlobalCommittedLSN {3} after primary read barrier with read quorum {1} for strong read, Responses: {2}", (object) secondaryQuorumReadResult.SelectedLsn, (object) readQuorumValue, (object) string.Join(";", (IEnumerable<string>) secondaryQuorumReadResult.StoreResponses), (object) secondaryQuorumReadResult.GlobalCommittedSelectedLsn);
            entity.RequestContext.QuorumSelectedStoreResponse = secondaryQuorumReadResult.SelectedResponse;
            entity.RequestContext.StoreResponses = secondaryQuorumReadResult.StoreResponses;
            entity.RequestContext.QuorumSelectedLSN = secondaryQuorumReadResult.SelectedLsn;
            entity.RequestContext.GlobalCommittedSelectedLSN = secondaryQuorumReadResult.GlobalCommittedSelectedLsn;
            break;
          case QuorumReader.ReadQuorumResultKind.QuorumNotSelected:
            if (hasPerformedReadFromPrimary)
            {
              DefaultTrace.TraceWarning("QuorumNotSelected: Primary read already attempted. Quorum could not be selected after retrying on secondaries.");
              throw new GoneException(RMResources.ReadQuorumNotMet);
            }
            DefaultTrace.TraceWarning("QuorumNotSelected: Quorum could not be selected with read quorum of {0}", (object) readQuorumValue);
            QuorumReader.ReadPrimaryResult readPrimaryResult = await this.ReadPrimaryAsync(entity, readQuorumValue, false);
            if (readPrimaryResult.IsSuccessful && readPrimaryResult.ShouldRetryOnSecondary)
            {
              DefaultTrace.TraceCritical("PrimaryResult has both Successful and ShouldRetryOnSecondary flags set");
              break;
            }
            if (readPrimaryResult.IsSuccessful)
            {
              DefaultTrace.TraceInformation("QuorumNotSelected: ReadPrimary successful");
              return readPrimaryResult.GetResponse();
            }
            if (readPrimaryResult.ShouldRetryOnSecondary)
            {
              shouldRetryOnSecondary = true;
              DefaultTrace.TraceWarning("QuorumNotSelected: ReadPrimary did not succeed. Will retry on secondary.");
              hasPerformedReadFromPrimary = true;
              break;
            }
            DefaultTrace.TraceWarning("QuorumNotSelected: Could not get successful response from ReadPrimary");
            throw new GoneException(RMResources.ReadQuorumNotMet);
          default:
            DefaultTrace.TraceCritical("Unknown ReadQuorum result {0}", (object) secondaryQuorumReadResult.QuorumResult.ToString());
            throw new InternalServerErrorException(RMResources.InternalServerError);
        }
        secondaryQuorumReadResult = (QuorumReader.ReadQuorumResult) null;
      }
      while (--readQuorumRetry > 0 & shouldRetryOnSecondary);
      DefaultTrace.TraceWarning("Could not complete read quorum with read quorum value of {0}", (object) readQuorumValue);
      throw new GoneException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, RMResources.ReadQuorumNotMet, (object) readQuorumValue));
    }

    public async Task<StoreResponse> ReadBoundedStalenessAsync(
      DocumentServiceRequest entity,
      int readQuorumValue)
    {
      int readQuorumRetry = 6;
      bool shouldRetryOnSecondary = false;
      bool hasPerformedReadFromPrimary = false;
      do
      {
        entity.RequestContext.TimeoutHelper.ThrowGoneIfElapsed();
        shouldRetryOnSecondary = false;
        QuorumReader.ReadQuorumResult readQuorumResult = await this.ReadQuorumAsync(entity, readQuorumValue, false, ReadMode.BoundedStaleness);
        switch (readQuorumResult.QuorumResult)
        {
          case QuorumReader.ReadQuorumResultKind.QuorumMet:
            return readQuorumResult.GetResponse();
          case QuorumReader.ReadQuorumResultKind.QuorumSelected:
            DefaultTrace.TraceWarning("QuorumSelected: Could not converge on LSN {0} after barrier with QuorumValue {1} Will not perform barrier call on Primary for BoundedStaleness, Responses: {2}", (object) readQuorumResult.SelectedLsn, (object) readQuorumValue, (object) string.Join(";", (IEnumerable<string>) readQuorumResult.StoreResponses));
            entity.RequestContext.QuorumSelectedStoreResponse = readQuorumResult.SelectedResponse;
            entity.RequestContext.StoreResponses = readQuorumResult.StoreResponses;
            entity.RequestContext.QuorumSelectedLSN = readQuorumResult.SelectedLsn;
            break;
          case QuorumReader.ReadQuorumResultKind.QuorumNotSelected:
            if (hasPerformedReadFromPrimary)
            {
              DefaultTrace.TraceWarning("QuorumNotSelected: Primary read already attempted. Quorum could not be selected after retrying on secondaries.");
              throw new GoneException(RMResources.ReadQuorumNotMet);
            }
            DefaultTrace.TraceWarning("QuorumNotSelected: Quorum could not be selected with read quorum of {0}", (object) readQuorumValue);
            QuorumReader.ReadPrimaryResult readPrimaryResult = await this.ReadPrimaryAsync(entity, readQuorumValue, false);
            if (readPrimaryResult.IsSuccessful && readPrimaryResult.ShouldRetryOnSecondary)
            {
              DefaultTrace.TraceCritical("QuorumNotSelected: PrimaryResult has both Successful and ShouldRetryOnSecondary flags set");
              break;
            }
            if (readPrimaryResult.IsSuccessful)
            {
              DefaultTrace.TraceInformation("QuorumNotSelected: ReadPrimary successful");
              return readPrimaryResult.GetResponse();
            }
            if (readPrimaryResult.ShouldRetryOnSecondary)
            {
              shouldRetryOnSecondary = true;
              DefaultTrace.TraceWarning("QuorumNotSelected: ReadPrimary did not succeed. Will retry on secondary.");
              hasPerformedReadFromPrimary = true;
              break;
            }
            DefaultTrace.TraceWarning("QuorumNotSelected: Could not get successful response from ReadPrimary");
            throw new GoneException(RMResources.ReadQuorumNotMet);
          default:
            DefaultTrace.TraceCritical("Unknown ReadQuorum result {0}", (object) readQuorumResult.QuorumResult.ToString());
            throw new InternalServerErrorException(RMResources.InternalServerError);
        }
      }
      while (--readQuorumRetry > 0 & shouldRetryOnSecondary);
      DefaultTrace.TraceError("Could not complete read quorum with read quorum value of {0}, RetryCount: {1}", (object) readQuorumValue, (object) 6);
      throw new GoneException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, RMResources.ReadQuorumNotMet, (object) readQuorumValue));
    }

    private async Task<QuorumReader.ReadQuorumResult> ReadQuorumAsync(
      DocumentServiceRequest entity,
      int readQuorum,
      bool includePrimary,
      ReadMode readMode)
    {
      entity.RequestContext.TimeoutHelper.ThrowGoneIfElapsed();
      long readLsn = -1;
      long globalCommittedLSN = -1;
      StoreResult storeResult = (StoreResult) null;
      List<string> storeResponses = (List<string>) null;
      if (entity.RequestContext.QuorumSelectedStoreResponse == null)
      {
        IList<StoreResult> storeResultList = await this.storeReader.ReadMultipleReplicaAsync(entity, includePrimary, readQuorum, true, false, readMode);
        storeResponses = storeResultList.Select<StoreResult, string>((Func<StoreResult, string>) (response => response.ToString())).ToList<string>();
        if (storeResultList.Count<StoreResult>((Func<StoreResult, bool>) (response => response.IsValid)) < readQuorum)
          return new QuorumReader.ReadQuorumResult(entity.RequestContext.RequestChargeTracker, QuorumReader.ReadQuorumResultKind.QuorumNotSelected, -1L, -1L, (StoreResult) null, storeResponses);
        int num;
        if (ReplicatedResourceClient.IsGlobalStrongEnabled() && this.serviceConfigReader.DefaultConsistencyLevel == ConsistencyLevel.Strong)
        {
          ConsistencyLevel? consistencyLevel1 = entity.RequestContext.OriginalRequestConsistencyLevel;
          if (consistencyLevel1.HasValue)
          {
            consistencyLevel1 = entity.RequestContext.OriginalRequestConsistencyLevel;
            ConsistencyLevel consistencyLevel2 = ConsistencyLevel.Strong;
            num = consistencyLevel1.GetValueOrDefault() == consistencyLevel2 & consistencyLevel1.HasValue ? 1 : 0;
          }
          else
            num = 1;
        }
        else
          num = 0;
        bool isGlobalStrongRead = num != 0;
        if (this.IsQuorumMet(storeResultList, readQuorum, false, isGlobalStrongRead, out readLsn, out globalCommittedLSN, out storeResult))
          return new QuorumReader.ReadQuorumResult(entity.RequestContext.RequestChargeTracker, QuorumReader.ReadQuorumResultKind.QuorumMet, readLsn, globalCommittedLSN, storeResult, storeResponses);
        entity.RequestContext.ForceRefreshAddressCache = false;
      }
      else
      {
        readLsn = entity.RequestContext.QuorumSelectedLSN;
        globalCommittedLSN = entity.RequestContext.GlobalCommittedSelectedLSN;
        storeResult = entity.RequestContext.QuorumSelectedStoreResponse;
        storeResponses = entity.RequestContext.StoreResponses;
      }
      return await this.WaitForReadBarrierAsync(await BarrierRequestHelper.CreateAsync(entity, this.authorizationTokenProvider, new long?(readLsn), new long?(globalCommittedLSN)), false, readQuorum, readLsn, globalCommittedLSN, readMode) ? new QuorumReader.ReadQuorumResult(entity.RequestContext.RequestChargeTracker, QuorumReader.ReadQuorumResultKind.QuorumMet, readLsn, globalCommittedLSN, storeResult, storeResponses) : new QuorumReader.ReadQuorumResult(entity.RequestContext.RequestChargeTracker, QuorumReader.ReadQuorumResultKind.QuorumSelected, readLsn, globalCommittedLSN, storeResult, storeResponses);
    }

    private async Task<QuorumReader.ReadPrimaryResult> ReadPrimaryAsync(
      DocumentServiceRequest entity,
      int readQuorum,
      bool useSessionToken)
    {
      entity.RequestContext.TimeoutHelper.ThrowGoneIfElapsed();
      entity.RequestContext.ForceRefreshAddressCache = false;
      StoreResult storeResult = await this.storeReader.ReadPrimaryAsync(entity, true, useSessionToken);
      if (!storeResult.IsValid)
        ExceptionDispatchInfo.Capture((Exception) storeResult.GetException()).Throw();
      if (storeResult.CurrentReplicaSetSize <= 0 || storeResult.LSN < 0L || storeResult.QuorumAckedLSN < 0L)
      {
        string message = string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Invalid value received from response header. CurrentReplicaSetSize {0}, StoreLSN {1}, QuorumAckedLSN {2}", (object) storeResult.CurrentReplicaSetSize, (object) storeResult.LSN, (object) storeResult.QuorumAckedLSN);
        if (storeResult.CurrentReplicaSetSize <= 0)
          DefaultTrace.TraceError(message);
        else
          DefaultTrace.TraceCritical(message);
        throw new GoneException(RMResources.ReadQuorumNotMet);
      }
      if (storeResult.CurrentReplicaSetSize > readQuorum)
      {
        DefaultTrace.TraceWarning("Unexpected response. Replica Set size is {0} which is greater than min value {1}", (object) storeResult.CurrentReplicaSetSize, (object) readQuorum);
        return new QuorumReader.ReadPrimaryResult(entity.RequestContext.RequestChargeTracker, false, true, (StoreResult) null);
      }
      if (storeResult.LSN == storeResult.QuorumAckedLSN)
        return new QuorumReader.ReadPrimaryResult(entity.RequestContext.RequestChargeTracker, true, false, storeResult);
      DefaultTrace.TraceWarning("Store LSN {0} and quorum acked LSN {1} don't match", (object) storeResult.LSN, (object) storeResult.QuorumAckedLSN);
      long higherLsn = storeResult.LSN > storeResult.QuorumAckedLSN ? storeResult.LSN : storeResult.QuorumAckedLSN;
      switch (await this.WaitForPrimaryLsnAsync(await BarrierRequestHelper.CreateAsync(entity, this.authorizationTokenProvider, new long?(higherLsn), new long?()), higherLsn, readQuorum))
      {
        case QuorumReader.PrimaryReadOutcome.QuorumNotMet:
          return new QuorumReader.ReadPrimaryResult(entity.RequestContext.RequestChargeTracker, false, false, (StoreResult) null);
        case QuorumReader.PrimaryReadOutcome.QuorumInconclusive:
          return new QuorumReader.ReadPrimaryResult(entity.RequestContext.RequestChargeTracker, false, true, (StoreResult) null);
        default:
          return new QuorumReader.ReadPrimaryResult(entity.RequestContext.RequestChargeTracker, true, false, storeResult);
      }
    }

    private async Task<QuorumReader.PrimaryReadOutcome> WaitForPrimaryLsnAsync(
      DocumentServiceRequest barrierRequest,
      long lsnToWaitFor,
      int readQuorum)
    {
      int primaryRetries = 6;
      do
      {
        barrierRequest.RequestContext.TimeoutHelper.ThrowGoneIfElapsed();
        barrierRequest.RequestContext.ForceRefreshAddressCache = false;
        StoreResult storeResult = await this.storeReader.ReadPrimaryAsync(barrierRequest, true, false);
        if (!storeResult.IsValid)
          ExceptionDispatchInfo.Capture((Exception) storeResult.GetException()).Throw();
        if (storeResult.CurrentReplicaSetSize > readQuorum)
        {
          DefaultTrace.TraceWarning("Unexpected response. Replica Set size is {0} which is greater than min value {1}", (object) storeResult.CurrentReplicaSetSize, (object) readQuorum);
          return QuorumReader.PrimaryReadOutcome.QuorumInconclusive;
        }
        if (storeResult.LSN < lsnToWaitFor || storeResult.QuorumAckedLSN < lsnToWaitFor)
        {
          DefaultTrace.TraceWarning("Store LSN {0} or quorum acked LSN {1} are lower than expected LSN {2}", (object) storeResult.LSN, (object) storeResult.QuorumAckedLSN, (object) lsnToWaitFor);
          await Task.Delay(5);
        }
        else
          goto label_9;
      }
      while (--primaryRetries > 0);
      goto label_10;
label_9:
      return QuorumReader.PrimaryReadOutcome.QuorumMet;
label_10:
      return QuorumReader.PrimaryReadOutcome.QuorumNotMet;
    }

    private async Task<bool> WaitForReadBarrierAsync(
      DocumentServiceRequest barrierRequest,
      bool allowPrimary,
      int readQuorum,
      long readBarrierLsn,
      long targetGlobalCommittedLSN,
      ReadMode readMode)
    {
      int readBarrierRetryCount = 6;
      int readBarrierRetryCountMultiRegion = 30;
      long maxGlobalCommittedLsn = 0;
      while (readBarrierRetryCount-- > 0)
      {
        barrierRequest.RequestContext.TimeoutHelper.ThrowGoneIfElapsed();
        IList<StoreResult> storeResultList = await this.storeReader.ReadMultipleReplicaAsync(barrierRequest, allowPrimary, readQuorum, true, false, readMode, forceReadAll: true);
        long num = storeResultList.Count > 0 ? storeResultList.Max<StoreResult>((Func<StoreResult, long>) (response => response.GlobalCommittedLSN)) : 0L;
        if (storeResultList.Count<StoreResult>((Func<StoreResult, bool>) (response => response.LSN >= readBarrierLsn)) >= readQuorum && (targetGlobalCommittedLSN <= 0L || num >= targetGlobalCommittedLSN))
          return true;
        maxGlobalCommittedLsn = maxGlobalCommittedLsn > num ? maxGlobalCommittedLsn : num;
        barrierRequest.RequestContext.ForceRefreshAddressCache = false;
        if (readBarrierRetryCount == 0)
          DefaultTrace.TraceInformation("QuorumReader: WaitForReadBarrierAsync - Last barrier for single-region requests. Responses: {0}", (object) string.Join<StoreResult>("; ", (IEnumerable<StoreResult>) storeResultList));
        else
          await Task.Delay(5);
      }
      if (targetGlobalCommittedLSN > 0L)
      {
        while (readBarrierRetryCountMultiRegion-- > 0)
        {
          barrierRequest.RequestContext.TimeoutHelper.ThrowGoneIfElapsed();
          IList<StoreResult> storeResultList = await this.storeReader.ReadMultipleReplicaAsync(barrierRequest, allowPrimary, readQuorum, true, false, readMode, forceReadAll: true);
          long num = storeResultList.Count > 0 ? storeResultList.Max<StoreResult>((Func<StoreResult, long>) (response => response.GlobalCommittedLSN)) : 0L;
          if (storeResultList.Count<StoreResult>((Func<StoreResult, bool>) (response => response.LSN >= readBarrierLsn)) >= readQuorum && num >= targetGlobalCommittedLSN)
            return true;
          maxGlobalCommittedLsn = maxGlobalCommittedLsn > num ? maxGlobalCommittedLsn : num;
          if (readBarrierRetryCountMultiRegion == 0)
            DefaultTrace.TraceInformation("QuorumReader: WaitForReadBarrierAsync - Last barrier for mult-region strong requests. Responses: {0}", (object) string.Join<StoreResult>("; ", (IEnumerable<StoreResult>) storeResultList));
          else if (30 - readBarrierRetryCountMultiRegion > 4)
            await Task.Delay(30);
          else
            await Task.Delay(10);
        }
      }
      DefaultTrace.TraceInformation("QuorumReader: WaitForReadBarrierAsync - TargetGlobalCommittedLsn: {0}, MaxGlobalCommittedLsn: {1}.", (object) targetGlobalCommittedLSN, (object) maxGlobalCommittedLsn);
      return false;
    }

    private bool IsQuorumMet(
      IList<StoreResult> readResponses,
      int readQuorum,
      bool isPrimaryIncluded,
      bool isGlobalStrongRead,
      out long readLsn,
      out long globalCommittedLSN,
      out StoreResult selectedResponse)
    {
      long maxLsn = 0;
      long num1 = long.MaxValue;
      int num2 = 0;
      IEnumerable<StoreResult> source = readResponses.Where<StoreResult>((Func<StoreResult, bool>) (response => response.IsValid));
      int num3 = source.Count<StoreResult>();
      if (num3 == 0)
      {
        readLsn = 0L;
        globalCommittedLSN = -1L;
        selectedResponse = (StoreResult) null;
        return false;
      }
      long num4 = source.Max<StoreResult>((Func<StoreResult, long>) (res => res.NumberOfReadRegions));
      bool flag1 = isGlobalStrongRead && num4 > 0L;
      foreach (StoreResult storeResult in source)
      {
        if (storeResult.LSN == maxLsn)
          ++num2;
        else if (storeResult.LSN > maxLsn)
        {
          num2 = 1;
          maxLsn = storeResult.LSN;
        }
        if (storeResult.LSN < num1)
          num1 = storeResult.LSN;
      }
      selectedResponse = source.Where<StoreResult>((Func<StoreResult, bool>) (s => s.LSN == maxLsn && s.StatusCode < StatusCodes.StartingErrorCode)).FirstOrDefault<StoreResult>();
      if (selectedResponse == null)
        selectedResponse = source.First<StoreResult>((Func<StoreResult, bool>) (s => s.LSN == maxLsn));
      readLsn = selectedResponse.ItemLSN == -1L ? maxLsn : Math.Min(selectedResponse.ItemLSN, maxLsn);
      globalCommittedLSN = flag1 ? readLsn : -1L;
      long num5 = source.Max<StoreResult>((Func<StoreResult, long>) (res => res.GlobalCommittedLSN));
      DefaultTrace.TraceInformation("QuorumReader: MaxLSN {0} ReplicaCountMaxLSN {1} bCheckGlobalStrong {2} MaxGlobalCommittedLSN {3} NumberOfReadRegions {4} SelectedResponseItemLSN {5}", (object) maxLsn, (object) num2, (object) flag1, (object) num5, (object) num4, (object) selectedResponse.ItemLSN);
      bool flag2 = false;
      if (readLsn > 0L && num2 >= readQuorum && (!flag1 || num5 >= maxLsn))
        flag2 = true;
      if (!flag2 && num3 >= readQuorum && selectedResponse.ItemLSN != -1L && num1 != long.MaxValue && selectedResponse.ItemLSN <= num1 && (!flag1 || selectedResponse.ItemLSN <= num5))
        flag2 = true;
      return flag2;
    }

    private enum ReadQuorumResultKind
    {
      QuorumMet,
      QuorumSelected,
      QuorumNotSelected,
    }

    private abstract class ReadResult
    {
      private readonly StoreResult response;
      private readonly RequestChargeTracker requestChargeTracker;

      protected ReadResult(RequestChargeTracker requestChargeTracker, StoreResult response)
      {
        this.requestChargeTracker = requestChargeTracker;
        this.response = response;
      }

      public StoreResponse GetResponse()
      {
        if (!this.IsValidResult())
        {
          DefaultTrace.TraceCritical("GetResponse called for invalid result");
          throw new InternalServerErrorException(RMResources.InternalServerError);
        }
        return this.response.ToResponse(this.requestChargeTracker);
      }

      protected abstract bool IsValidResult();
    }

    private sealed class ReadQuorumResult : QuorumReader.ReadResult
    {
      public ReadQuorumResult(
        RequestChargeTracker requestChargeTracker,
        QuorumReader.ReadQuorumResultKind QuorumResult,
        long selectedLsn,
        long globalCommittedSelectedLsn,
        StoreResult selectedResponse,
        List<string> storeResponses)
        : base(requestChargeTracker, selectedResponse)
      {
        this.QuorumResult = QuorumResult;
        this.SelectedLsn = selectedLsn;
        this.GlobalCommittedSelectedLsn = globalCommittedSelectedLsn;
        this.SelectedResponse = selectedResponse;
        this.StoreResponses = storeResponses;
      }

      public QuorumReader.ReadQuorumResultKind QuorumResult { get; private set; }

      public StoreResult SelectedResponse { get; private set; }

      public List<string> StoreResponses { get; private set; }

      public long SelectedLsn { get; private set; }

      public long GlobalCommittedSelectedLsn { get; private set; }

      protected override bool IsValidResult() => this.QuorumResult == QuorumReader.ReadQuorumResultKind.QuorumMet || this.QuorumResult == QuorumReader.ReadQuorumResultKind.QuorumSelected;
    }

    private sealed class ReadPrimaryResult : QuorumReader.ReadResult
    {
      public ReadPrimaryResult(
        RequestChargeTracker requestChargeTracker,
        bool isSuccessful,
        bool shouldRetryOnSecondary,
        StoreResult response)
        : base(requestChargeTracker, response)
      {
        this.IsSuccessful = isSuccessful;
        this.ShouldRetryOnSecondary = shouldRetryOnSecondary;
      }

      public bool ShouldRetryOnSecondary { get; private set; }

      public bool IsSuccessful { get; private set; }

      protected override bool IsValidResult() => this.IsSuccessful;
    }

    private enum PrimaryReadOutcome
    {
      QuorumNotMet,
      QuorumInconclusive,
      QuorumMet,
    }
  }
}
