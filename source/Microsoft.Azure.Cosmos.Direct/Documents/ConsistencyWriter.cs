// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.ConsistencyWriter
// Assembly: Microsoft.Azure.Cosmos.Direct, Version=3.29.4.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: FFE3C00D-4333-4294-8947-B1C93A852E2F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Direct.dll

using Microsoft.Azure.Cosmos.Core.Trace;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Documents
{
  [SuppressMessage("", "AvoidMultiLineComments", Justification = "Multi line business logic")]
  internal sealed class ConsistencyWriter
  {
    private const int maxNumberOfWriteBarrierReadRetries = 30;
    private const int delayBetweenWriteBarrierCallsInMs = 30;
    private const int maxShortBarrierRetriesForMultiRegion = 4;
    private const int shortbarrierRetryIntervalInMsForMultiRegion = 10;
    private readonly StoreReader storeReader;
    private readonly TransportClient transportClient;
    private readonly AddressSelector addressSelector;
    private readonly ISessionContainer sessionContainer;
    private readonly IServiceConfigurationReader serviceConfigReader;
    private readonly IAuthorizationTokenProvider authorizationTokenProvider;
    private readonly bool useMultipleWriteLocations;

    public ConsistencyWriter(
      AddressSelector addressSelector,
      ISessionContainer sessionContainer,
      TransportClient transportClient,
      IServiceConfigurationReader serviceConfigReader,
      IAuthorizationTokenProvider authorizationTokenProvider,
      bool useMultipleWriteLocations)
    {
      this.transportClient = transportClient;
      this.addressSelector = addressSelector;
      this.sessionContainer = sessionContainer;
      this.serviceConfigReader = serviceConfigReader;
      this.authorizationTokenProvider = authorizationTokenProvider;
      this.useMultipleWriteLocations = useMultipleWriteLocations;
      this.storeReader = new StoreReader(transportClient, addressSelector, (IAddressEnumerator) new AddressEnumerator(), (ISessionContainer) null);
    }

    internal string LastWriteAddress { get; private set; }

    public async Task<StoreResponse> WriteAsync(
      DocumentServiceRequest entity,
      TimeoutHelper timeout,
      bool forceRefresh,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      timeout.ThrowTimeoutIfElapsed();
      string sessionToken = entity.Headers["x-ms-session-token"];
      StoreResponse storeResponse;
      try
      {
        storeResponse = await BackoffRetryUtility<StoreResponse>.ExecuteAsync((Func<Task<StoreResponse>>) (() => this.WritePrivateAsync(entity, timeout, forceRefresh)), (IRetryPolicy) new SessionTokenMismatchRetryPolicy(), cancellationToken);
      }
      finally
      {
        SessionTokenHelper.SetOriginalSessionToken(entity, sessionToken);
      }
      sessionToken = (string) null;
      return storeResponse;
    }

    private async Task<StoreResponse> WritePrivateAsync(
      DocumentServiceRequest request,
      TimeoutHelper timeout,
      bool forceRefresh)
    {
      timeout.ThrowTimeoutIfElapsed();
      request.RequestContext.TimeoutHelper = timeout;
      if (request.RequestContext.RequestChargeTracker == null)
        request.RequestContext.RequestChargeTracker = new RequestChargeTracker();
      if (request.RequestContext.ClientRequestStatistics == null)
        request.RequestContext.ClientRequestStatistics = (IClientSideRequestStatistics) new ClientSideRequestStatistics();
      request.RequestContext.ForceRefreshAddressCache = forceRefresh;
      DocumentServiceRequest barrierRequest;
      if (request.RequestContext.GlobalStrongWriteStoreResult == null)
      {
        string requestedCollectionRid = request.RequestContext.ResolvedCollectionRid;
        PerProtocolPartitionAddressInformation addressInformation = await this.addressSelector.ResolveAddressesAsync(request, forceRefresh);
        if (!string.IsNullOrEmpty(requestedCollectionRid) && !string.IsNullOrEmpty(request.RequestContext.ResolvedCollectionRid) && !requestedCollectionRid.Equals(request.RequestContext.ResolvedCollectionRid))
          this.sessionContainer.ClearTokenByResourceId(requestedCollectionRid);
        request.RequestContext.ClientRequestStatistics.ContactedReplicas = addressInformation.ReplicaTransportAddressUris.ToList<TransportAddressUri>();
        TransportAddressUri primaryUri = addressInformation.GetPrimaryAddressUri(request);
        this.LastWriteAddress = primaryUri.ToString();
        if ((this.useMultipleWriteLocations || request.OperationType == OperationType.Batch) && RequestHelper.GetConsistencyLevelToUse(this.serviceConfigReader, request) == ConsistencyLevel.Session)
          SessionTokenHelper.SetPartitionLocalSessionToken(request, this.sessionContainer);
        else
          SessionTokenHelper.ValidateAndRemoveSessionToken(request);
        DateTime startTimeUtc = DateTime.UtcNow;
        StoreResult storeResult;
        try
        {
          storeResult = StoreResult.CreateStoreResult(await this.transportClient.InvokeResourceOperationAsync(primaryUri, request), (Exception) null, true, false, primaryUri.Uri);
          request.RequestContext.ClientRequestStatistics.RecordResponse(request, storeResult, startTimeUtc, DateTime.UtcNow);
        }
        catch (Exception ex1)
        {
          storeResult = StoreResult.CreateStoreResult((StoreResponse) null, ex1, true, false, primaryUri.Uri);
          request.RequestContext.ClientRequestStatistics.RecordResponse(request, storeResult, startTimeUtc, DateTime.UtcNow);
          if (ex1 is DocumentClientException)
          {
            DocumentClientException ex2 = (DocumentClientException) ex1;
            StoreResult.VerifyCanContinueOnException(ex2);
            if (!string.IsNullOrWhiteSpace(ex2.Headers["x-ms-write-request-trigger-refresh"]))
            {
              int result;
              if (int.TryParse(ex2.Headers.GetValues("x-ms-write-request-trigger-refresh")[0], NumberStyles.Integer, (IFormatProvider) CultureInfo.InvariantCulture, out result))
              {
                if (result == 1)
                  this.addressSelector.StartBackgroundAddressRefresh(request);
              }
            }
          }
        }
        if (storeResult == null)
        {
          DefaultTrace.TraceCritical("ConsistencyWriter did not get storeResult!");
          throw new InternalServerErrorException();
        }
        if (!ReplicatedResourceClient.IsGlobalStrongEnabled() || !this.ShouldPerformWriteBarrierForGlobalStrong(storeResult))
          return storeResult.ToResponse();
        long lsn = storeResult.LSN;
        long globalCommittedLsn = storeResult.GlobalCommittedLSN;
        if (lsn == -1L || globalCommittedLsn == -1L)
        {
          DefaultTrace.TraceWarning("ConsistencyWriter: LSN {0} or GlobalCommittedLsn {1} is not set for global strong request", (object) lsn, (object) globalCommittedLsn);
          throw new GoneException(RMResources.Gone, SubStatusCodes.ServerGenerated410);
        }
        request.RequestContext.GlobalStrongWriteStoreResult = storeResult;
        request.RequestContext.GlobalCommittedSelectedLSN = lsn;
        request.RequestContext.ForceRefreshAddressCache = false;
        DefaultTrace.TraceInformation("ConsistencyWriter: globalCommittedLsn {0}, lsn {1}", (object) globalCommittedLsn, (object) lsn);
        if (globalCommittedLsn < lsn)
        {
          barrierRequest = await BarrierRequestHelper.CreateAsync(request, this.authorizationTokenProvider, new long?(), new long?(request.RequestContext.GlobalCommittedSelectedLSN));
          try
          {
            if (!await this.WaitForWriteBarrierAsync(barrierRequest, request.RequestContext.GlobalCommittedSelectedLSN))
            {
              DefaultTrace.TraceError("ConsistencyWriter: Write barrier has not been met for global strong request. SelectedGlobalCommittedLsn: {0}", (object) request.RequestContext.GlobalCommittedSelectedLSN);
              throw new GoneException(RMResources.GlobalStrongWriteBarrierNotMet, SubStatusCodes.Server_GlobalStrongWriteBarrierNotMet);
            }
          }
          finally
          {
            barrierRequest?.Dispose();
          }
          barrierRequest = (DocumentServiceRequest) null;
        }
        requestedCollectionRid = (string) null;
        primaryUri = (TransportAddressUri) null;
      }
      else
      {
        barrierRequest = await BarrierRequestHelper.CreateAsync(request, this.authorizationTokenProvider, new long?(), new long?(request.RequestContext.GlobalCommittedSelectedLSN));
        try
        {
          if (!await this.WaitForWriteBarrierAsync(barrierRequest, request.RequestContext.GlobalCommittedSelectedLSN))
          {
            DefaultTrace.TraceWarning("ConsistencyWriter: Write barrier has not been met for global strong request. SelectedGlobalCommittedLsn: {0}", (object) request.RequestContext.GlobalCommittedSelectedLSN);
            throw new GoneException(RMResources.GlobalStrongWriteBarrierNotMet, SubStatusCodes.Server_GlobalStrongWriteBarrierNotMet);
          }
        }
        finally
        {
          barrierRequest?.Dispose();
        }
        barrierRequest = (DocumentServiceRequest) null;
      }
      return request.RequestContext.GlobalStrongWriteStoreResult.ToResponse();
    }

    private bool ShouldPerformWriteBarrierForGlobalStrong(StoreResult storeResult) => (storeResult.StatusCode < StatusCodes.StartingErrorCode || storeResult.StatusCode == StatusCodes.Conflict || storeResult.StatusCode == StatusCodes.NotFound && storeResult.SubStatusCode != SubStatusCodes.PartitionKeyRangeGone || storeResult.StatusCode == StatusCodes.PreconditionFailed) && this.serviceConfigReader.DefaultConsistencyLevel == ConsistencyLevel.Strong && storeResult.NumberOfReadRegions > 0L;

    private async Task<bool> WaitForWriteBarrierAsync(
      DocumentServiceRequest barrierRequest,
      long selectedGlobalCommittedLsn)
    {
      int writeBarrierRetryCount = 30;
      long maxGlobalCommittedLsnReceived = 0;
      while (writeBarrierRetryCount-- > 0)
      {
        barrierRequest.RequestContext.TimeoutHelper.ThrowTimeoutIfElapsed();
        IList<StoreResult> storeResultList = await this.storeReader.ReadMultipleReplicaAsync(barrierRequest, true, 1, false, false, ReadMode.Strong);
        if (storeResultList != null && storeResultList.Any<StoreResult>((Func<StoreResult, bool>) (response => response.GlobalCommittedLSN >= selectedGlobalCommittedLsn)))
          return true;
        long num = storeResultList != null ? storeResultList.Select<StoreResult, long>((Func<StoreResult, long>) (s => s.GlobalCommittedLSN)).DefaultIfEmpty<long>(0L).Max() : 0L;
        maxGlobalCommittedLsnReceived = maxGlobalCommittedLsnReceived > num ? maxGlobalCommittedLsnReceived : num;
        barrierRequest.RequestContext.ForceRefreshAddressCache = false;
        if (writeBarrierRetryCount == 0)
          DefaultTrace.TraceInformation("ConsistencyWriter: WaitForWriteBarrierAsync - Last barrier multi-region strong. Responses: {0}", (object) string.Join<StoreResult>("; ", (IEnumerable<StoreResult>) storeResultList));
        else if (30 - writeBarrierRetryCount > 4)
          await Task.Delay(30);
        else
          await Task.Delay(10);
      }
      DefaultTrace.TraceInformation("ConsistencyWriter: Highest global committed lsn received for write barrier call is {0}", (object) maxGlobalCommittedLsnReceived);
      return false;
    }
  }
}
