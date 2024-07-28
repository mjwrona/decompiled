// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.StoreReader
// Assembly: Microsoft.Azure.Cosmos.Direct, Version=3.29.4.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: FFE3C00D-4333-4294-8947-B1C93A852E2F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Direct.dll

using Microsoft.Azure.Cosmos.Core.Trace;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Threading.Tasks;

namespace Microsoft.Azure.Documents
{
  internal sealed class StoreReader
  {
    private readonly TransportClient transportClient;
    private readonly AddressSelector addressSelector;
    private readonly IAddressEnumerator addressEnumerator;
    private readonly ISessionContainer sessionContainer;
    private readonly bool canUseLocalLSNBasedHeaders;

    public StoreReader(
      TransportClient transportClient,
      AddressSelector addressSelector,
      IAddressEnumerator addressEnumerator,
      ISessionContainer sessionContainer)
    {
      this.transportClient = transportClient;
      this.addressSelector = addressSelector;
      this.addressEnumerator = addressEnumerator ?? throw new ArgumentNullException(nameof (addressEnumerator));
      this.sessionContainer = sessionContainer;
      this.canUseLocalLSNBasedHeaders = VersionUtility.IsLaterThan(HttpConstants.Versions.CurrentVersion, HttpConstants.Versions.v2018_06_18);
    }

    internal string LastReadAddress { get; set; }

    public async Task<IList<StoreResult>> ReadMultipleReplicaAsync(
      DocumentServiceRequest entity,
      bool includePrimary,
      int replicaCountToRead,
      bool requiresValidLsn,
      bool useSessionToken,
      ReadMode readMode,
      bool checkMinLSN = false,
      bool forceReadAll = false)
    {
      entity.RequestContext.TimeoutHelper.ThrowGoneIfElapsed();
      string originalSessionToken = entity.Headers["x-ms-session-token"];
      try
      {
        using (StoreReader.ReadReplicaResult readQuorumResult = await this.ReadMultipleReplicasInternalAsync(entity, includePrimary, replicaCountToRead, requiresValidLsn, useSessionToken, readMode, checkMinLSN, forceReadAll))
        {
          if (!entity.RequestContext.PerformLocalRefreshOnGoneException || !readQuorumResult.RetryWithForceRefresh || entity.RequestContext.ForceRefreshAddressCache)
            return readQuorumResult.StoreResultList.GetValueAndDereference();
          entity.RequestContext.TimeoutHelper.ThrowGoneIfElapsed();
          entity.RequestContext.ForceRefreshAddressCache = true;
          using (StoreReader.ReadReplicaResult readReplicaResult = await this.ReadMultipleReplicasInternalAsync(entity, includePrimary, replicaCountToRead, requiresValidLsn, useSessionToken, readMode, forceReadAll: forceReadAll))
            return readReplicaResult.StoreResultList.GetValueAndDereference();
        }
      }
      finally
      {
        SessionTokenHelper.SetOriginalSessionToken(entity, originalSessionToken);
      }
    }

    public async Task<StoreResult> ReadPrimaryAsync(
      DocumentServiceRequest entity,
      bool requiresValidLsn,
      bool useSessionToken)
    {
      entity.RequestContext.TimeoutHelper.ThrowGoneIfElapsed();
      string originalSessionToken = entity.Headers["x-ms-session-token"];
      try
      {
        using (StoreReader.ReadReplicaResult readQuorumResult = await this.ReadPrimaryInternalAsync(entity, requiresValidLsn, useSessionToken, false))
        {
          if (!entity.RequestContext.PerformLocalRefreshOnGoneException || !readQuorumResult.RetryWithForceRefresh || entity.RequestContext.ForceRefreshAddressCache)
            return StoreReader.GetStoreResultOrThrowGoneException(readQuorumResult);
          entity.RequestContext.TimeoutHelper.ThrowGoneIfElapsed();
          entity.RequestContext.ForceRefreshAddressCache = true;
          using (StoreReader.ReadReplicaResult readReplicaResult = await this.ReadPrimaryInternalAsync(entity, requiresValidLsn, useSessionToken, true))
            return StoreReader.GetStoreResultOrThrowGoneException(readReplicaResult);
        }
      }
      finally
      {
        SessionTokenHelper.SetOriginalSessionToken(entity, originalSessionToken);
      }
    }

    private static StoreResult GetStoreResultOrThrowGoneException(
      StoreReader.ReadReplicaResult readReplicaResult)
    {
      StoreReader.StoreResultList storeResultList = readReplicaResult.StoreResultList;
      return storeResultList.Count != 0 ? storeResultList.GetFirstStoreResultAndDereference() : throw new GoneException(RMResources.Gone, SubStatusCodes.Server_NoValidStoreResponse);
    }

    private async Task<StoreReader.ReadReplicaResult> ReadMultipleReplicasInternalAsync(
      DocumentServiceRequest entity,
      bool includePrimary,
      int replicaCountToRead,
      bool requiresValidLsn,
      bool useSessionToken,
      ReadMode readMode,
      bool checkMinLSN = false,
      bool forceReadAll = false)
    {
      entity.RequestContext.TimeoutHelper.ThrowGoneIfElapsed();
      using (StoreReader.StoreResultList storeResultList = new StoreReader.StoreResultList((IList<StoreResult>) new List<StoreResult>(replicaCountToRead)))
      {
        string requestedCollectionRid = entity.RequestContext.ResolvedCollectionRid;
        IReadOnlyList<TransportAddressUri> resolveApiResults = await this.addressSelector.ResolveAllTransportAddressUriAsync(entity, includePrimary, entity.RequestContext.ForceRefreshAddressCache);
        if (!string.IsNullOrEmpty(requestedCollectionRid) && !string.IsNullOrEmpty(entity.RequestContext.ResolvedCollectionRid) && !requestedCollectionRid.Equals(entity.RequestContext.ResolvedCollectionRid))
          this.sessionContainer.ClearTokenByResourceId(requestedCollectionRid);
        ISessionToken requestSessionToken = (ISessionToken) null;
        if (useSessionToken)
        {
          SessionTokenHelper.SetPartitionLocalSessionToken(entity, this.sessionContainer);
          if (checkMinLSN)
            requestSessionToken = entity.RequestContext.SessionToken;
        }
        else
          entity.Headers.Remove("x-ms-session-token");
        if (resolveApiResults.Count < replicaCountToRead)
          return entity.RequestContext.ForceRefreshAddressCache ? new StoreReader.ReadReplicaResult(false, storeResultList.GetValueAndDereference()) : new StoreReader.ReadReplicaResult(true, storeResultList.GetValueAndDereference());
        int num = replicaCountToRead;
        string header = entity.Headers["x-ms-version"];
        bool enforceSessionCheck = !string.IsNullOrEmpty(header) && VersionUtility.IsLaterThan(header, HttpConstants.VersionDates.v2016_05_30);
        this.UpdateContinuationTokenIfReadFeedOrQuery(entity);
        bool hasGoneException = false;
        bool hasCancellationException = false;
        Exception cancellationException = (Exception) null;
        Exception exceptionToThrow = (Exception) null;
        SubStatusCodes subStatusCodeForException = SubStatusCodes.Unknown;
        IEnumerator<TransportAddressUri> uriEnumerator = this.addressEnumerator.GetTransportAddresses(resolveApiResults, entity.RequestContext.FailedEndpoints).GetEnumerator();
        while (num > 0 && uriEnumerator.MoveNext())
        {
          entity.RequestContext.TimeoutHelper.ThrowGoneIfElapsed();
          Dictionary<Task<(StoreResponse, DateTime)>, (TransportAddressUri, DateTime)> readStoreTasks = new Dictionary<Task<(StoreResponse, DateTime)>, (TransportAddressUri, DateTime)>();
          do
          {
            readStoreTasks.Add(this.ReadFromStoreAsync(uriEnumerator.Current, entity), (uriEnumerator.Current, DateTime.UtcNow));
          }
          while ((forceReadAll || readStoreTasks.Count != num) && uriEnumerator.MoveNext());
          try
          {
            (StoreResponse, DateTime)[] valueTupleArray = await Task.WhenAll<(StoreResponse, DateTime)>((IEnumerable<Task<(StoreResponse, DateTime)>>) readStoreTasks.Keys);
          }
          catch (Exception ex)
          {
            exceptionToThrow = ex;
            if (ex is DocumentClientException documentClientException1)
              subStatusCodeForException = documentClientException1.GetSubStatus();
            if (ex is DocumentClientException documentClientException2)
            {
              HttpStatusCode? statusCode = documentClientException2.StatusCode;
              HttpStatusCode httpStatusCode1 = HttpStatusCode.NotFound;
              if (!(statusCode.GetValueOrDefault() == httpStatusCode1 & statusCode.HasValue))
              {
                statusCode = documentClientException2.StatusCode;
                HttpStatusCode httpStatusCode2 = HttpStatusCode.Conflict;
                if (!(statusCode.GetValueOrDefault() == httpStatusCode2 & statusCode.HasValue))
                {
                  statusCode = documentClientException2.StatusCode;
                  if (statusCode.Value != (HttpStatusCode) 429)
                    goto label_22;
                }
              }
              DefaultTrace.TraceInformation("StoreReader.ReadMultipleReplicasInternalAsync exception thrown: StatusCode: {0}; SubStatusCode:{1}; Exception.Message: {2}", (object) documentClientException2.StatusCode, (object) documentClientException2.Headers?.Get("x-ms-substatus"), (object) documentClientException2.Message);
              goto label_23;
            }
label_22:
            DefaultTrace.TraceInformation("StoreReader.ReadMultipleReplicasInternalAsync exception thrown: Exception: {0}", (object) ex);
          }
label_23:
          foreach (KeyValuePair<Task<(StoreResponse, DateTime)>, (TransportAddressUri, DateTime)> keyValuePair in readStoreTasks)
          {
            Task<(StoreResponse, DateTime)> key = keyValuePair.Key;
            StoreResponse storeResponse;
            DateTime endTimeUtc;
            if (key.Status != TaskStatus.RanToCompletion)
            {
              DateTime utcNow = DateTime.UtcNow;
              storeResponse = (StoreResponse) null;
              endTimeUtc = utcNow;
            }
            else
              (storeResponse, endTimeUtc) = key.Result;
            Exception innerException = key.Exception?.InnerException;
            TransportAddressUri targetUri = keyValuePair.Value.Item1;
            if (innerException != null)
              entity.RequestContext.AddToFailedEndpoints(innerException, targetUri);
            if (key.IsCanceled || innerException is OperationCanceledException)
            {
              hasCancellationException = true;
              if (cancellationException == null)
                cancellationException = innerException;
            }
            else
            {
              using (StoreReader.DisposableObject<StoreResult> disposableObject = new StoreReader.DisposableObject<StoreResult>(StoreResult.CreateStoreResult(storeResponse, innerException, requiresValidLsn, this.canUseLocalLSNBasedHeaders && readMode != ReadMode.Strong, targetUri.Uri)))
              {
                StoreResult storeResult = disposableObject.Value;
                entity.RequestContext.RequestChargeTracker.AddCharge(storeResult.RequestCharge);
                if (storeResponse != null)
                  entity.RequestContext.ClientRequestStatistics.ContactedReplicas.Add(targetUri);
                if (innerException != null && innerException.InnerException is TransportException)
                  entity.RequestContext.ClientRequestStatistics.FailedReplicas.Add(targetUri);
                entity.RequestContext.ClientRequestStatistics.RecordResponse(entity, storeResult, keyValuePair.Value.Item2, endTimeUtc);
                if (storeResult.Exception != null)
                  StoreResult.VerifyCanContinueOnException(storeResult.Exception);
                if (storeResult.IsValid && (requestSessionToken == null || storeResult.SessionToken != null && requestSessionToken.IsValid(storeResult.SessionToken) || !enforceSessionCheck && storeResult.StatusCode != StatusCodes.NotFound))
                  storeResultList.Add(disposableObject.GetValueAndDereference());
                hasGoneException = ((hasGoneException ? 1 : 0) | (storeResult.StatusCode != StatusCodes.Gone ? 0 : (storeResult.SubStatusCode != SubStatusCodes.NameCacheIsStale ? 1 : 0))) != 0;
              }
              if (hasGoneException && !entity.RequestContext.PerformedBackgroundAddressRefresh)
              {
                this.addressSelector.StartBackgroundAddressRefresh(entity);
                entity.RequestContext.PerformedBackgroundAddressRefresh = true;
              }
            }
          }
          if (storeResultList.Count >= replicaCountToRead)
            return new StoreReader.ReadReplicaResult(false, storeResultList.GetValueAndDereference());
          num = replicaCountToRead - storeResultList.Count;
          readStoreTasks = (Dictionary<Task<(StoreResponse, DateTime)>, (TransportAddressUri, DateTime)>) null;
        }
        if (storeResultList.Count < replicaCountToRead)
        {
          DefaultTrace.TraceInformation("Could not get quorum number of responses. ValidResponsesReceived: {0} ResponsesExpected: {1}, ResolvedAddressCount: {2}, ResponsesString: {3}", (object) storeResultList.Count, (object) replicaCountToRead, (object) resolveApiResults.Count, (object) string.Join<StoreResult>(";", (IEnumerable<StoreResult>) storeResultList.GetValue()));
          if (hasGoneException)
          {
            if (!entity.RequestContext.PerformLocalRefreshOnGoneException)
              throw new GoneException(exceptionToThrow, subStatusCodeForException);
            if (!entity.RequestContext.ForceRefreshAddressCache)
              return new StoreReader.ReadReplicaResult(true, storeResultList.GetValueAndDereference());
          }
          else if (hasCancellationException)
            throw cancellationException ?? (Exception) new OperationCanceledException();
        }
        return new StoreReader.ReadReplicaResult(false, storeResultList.GetValueAndDereference());
      }
    }

    private async Task<StoreReader.ReadReplicaResult> ReadPrimaryInternalAsync(
      DocumentServiceRequest entity,
      bool requiresValidLsn,
      bool useSessionToken,
      bool isRetryAfterRefresh)
    {
      entity.RequestContext.TimeoutHelper.ThrowGoneIfElapsed();
      TransportAddressUri primaryUri = await this.addressSelector.ResolvePrimaryTransportAddressUriAsync(entity, entity.RequestContext.ForceRefreshAddressCache);
      if (useSessionToken)
        SessionTokenHelper.SetPartitionLocalSessionToken(entity, this.sessionContainer);
      else
        entity.Headers.Remove("x-ms-session-token");
      DateTime startTimeUtc = DateTime.UtcNow;
      DateTime? endTimeUtc = new DateTime?();
      StoreResult storeResult;
      try
      {
        this.UpdateContinuationTokenIfReadFeedOrQuery(entity);
        (StoreResponse, DateTime) valueTuple = await this.ReadFromStoreAsync(primaryUri, entity);
        StoreResponse storeResponse = valueTuple.Item1;
        endTimeUtc = new DateTime?(valueTuple.Item2);
        storeResult = StoreResult.CreateStoreResult(storeResponse, (Exception) null, requiresValidLsn, this.canUseLocalLSNBasedHeaders, primaryUri.Uri);
      }
      catch (Exception ex)
      {
        DefaultTrace.TraceInformation("Exception {0} is thrown while doing Read Primary", (object) ex);
        storeResult = StoreResult.CreateStoreResult((StoreResponse) null, ex, requiresValidLsn, this.canUseLocalLSNBasedHeaders, primaryUri.Uri);
      }
      using (StoreReader.DisposableObject<StoreResult> disposableObject = new StoreReader.DisposableObject<StoreResult>(storeResult))
      {
        entity.RequestContext.ClientRequestStatistics.RecordResponse(entity, storeResult, startTimeUtc, endTimeUtc ?? DateTime.UtcNow);
        entity.RequestContext.RequestChargeTracker.AddCharge(storeResult.RequestCharge);
        if (storeResult.Exception != null)
          StoreResult.VerifyCanContinueOnException(storeResult.Exception);
        if (storeResult.StatusCode == StatusCodes.Gone && storeResult.SubStatusCode != SubStatusCodes.NameCacheIsStale)
        {
          if (isRetryAfterRefresh || !entity.RequestContext.PerformLocalRefreshOnGoneException || entity.RequestContext.ForceRefreshAddressCache)
            throw new GoneException(RMResources.Gone, storeResult.SubStatusCode);
          return new StoreReader.ReadReplicaResult(true, (IList<StoreResult>) new List<StoreResult>());
        }
        return new StoreReader.ReadReplicaResult(false, (IList<StoreResult>) new StoreResult[1]
        {
          disposableObject.GetValueAndDereference()
        });
      }
    }

    private async Task<(StoreResponse, DateTime endTime)> ReadFromStoreAsync(
      TransportAddressUri physicalAddress,
      DocumentServiceRequest request)
    {
      request.RequestContext.TimeoutHelper.ThrowGoneIfElapsed();
      this.LastReadAddress = physicalAddress.ToString();
      switch (request.OperationType)
      {
        case OperationType.ExecuteJavaScript:
        case OperationType.Read:
        case OperationType.SqlQuery:
        case OperationType.Head:
        case OperationType.HeadFeed:
          return (await this.transportClient.InvokeResourceOperationAsync(physicalAddress, request), DateTime.UtcNow);
        case OperationType.ReadFeed:
        case OperationType.Query:
          QueryRequestPerformanceActivity activity = CustomTypeExtensions.StartActivity(request);
          return (await StoreReader.CompleteActivity(this.transportClient.InvokeResourceOperationAsync(physicalAddress, request), activity), DateTime.UtcNow);
        default:
          throw new InvalidOperationException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Unexpected operation type {0}", (object) request.OperationType));
      }
    }

    private void UpdateContinuationTokenIfReadFeedOrQuery(DocumentServiceRequest request)
    {
      if (request.OperationType != OperationType.ReadFeed && request.OperationType != OperationType.Query)
        return;
      string continuation = request.Continuation;
      if (continuation == null)
        return;
      int length = continuation.IndexOf(';');
      if (length < 0)
        return;
      int num = 1;
      for (int index = length + 1; index < continuation.Length; ++index)
      {
        if (continuation[index] == ';')
        {
          ++num;
          if (num >= 3)
            break;
        }
      }
      if (num < 3)
        throw new BadRequestException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, RMResources.InvalidHeaderValue, (object) continuation, (object) "x-ms-continuation"));
      request.Continuation = continuation.Substring(0, length);
    }

    private static async Task<StoreResponse> CompleteActivity(
      Task<StoreResponse> task,
      QueryRequestPerformanceActivity activity)
    {
      if (activity == null)
        return await task;
      StoreResponse storeResponse;
      try
      {
        storeResponse = await task;
      }
      catch
      {
        activity.ActivityComplete(false);
        throw;
      }
      activity.ActivityComplete(true);
      return storeResponse;
    }

    private sealed class ReadReplicaResult : IDisposable
    {
      public ReadReplicaResult(bool retryWithForceRefresh, IList<StoreResult> responses)
      {
        this.RetryWithForceRefresh = retryWithForceRefresh;
        this.StoreResultList = new StoreReader.StoreResultList(responses);
      }

      public bool RetryWithForceRefresh { get; private set; }

      public StoreReader.StoreResultList StoreResultList { get; private set; }

      public void Dispose() => this.StoreResultList.Dispose();
    }

    private class StoreResultList : IDisposable
    {
      private IList<StoreResult> value;

      public StoreResultList(IList<StoreResult> collection) => this.value = collection ?? throw new ArgumentNullException();

      public void Add(StoreResult storeResult) => this.GetValueOrThrow().Add(storeResult);

      public int Count => this.GetValueOrThrow().Count;

      public StoreResult GetFirstStoreResultAndDereference()
      {
        if (this.GetValueOrThrow().Count <= 0)
          return (StoreResult) null;
        StoreResult resultAndDereference = this.value[0];
        this.value[0] = (StoreResult) null;
        return resultAndDereference;
      }

      public IList<StoreResult> GetValue() => this.GetValueOrThrow();

      public IList<StoreResult> GetValueAndDereference()
      {
        IList<StoreResult> valueOrThrow = this.GetValueOrThrow();
        this.value = (IList<StoreResult>) null;
        return valueOrThrow;
      }

      public void Dispose()
      {
        if (this.value == null)
          return;
        for (int index = 0; index < this.value.Count; ++index)
          this.value[index]?.Dispose();
      }

      private IList<StoreResult> GetValueOrThrow() => this.value != null && (this.value.Count <= 0 || this.value[0] != null) ? this.value : throw new InvalidOperationException("Call on the StoreResultList with deferenced collection");
    }

    private struct DisposableObject<T> : IDisposable where T : IDisposable
    {
      private T value;

      public DisposableObject(T value) => this.value = value ?? throw new ArgumentNullException();

      public T Value => this.value ?? throw new InvalidOperationException("Call on the DisposableObject with deferenced object");

      public T GetValueAndDereference()
      {
        T valueAndDereference = this.Value;
        this.value = default (T);
        return valueAndDereference;
      }

      public void Dispose()
      {
        ref T local1 = ref this.value;
        if ((object) default (T) == null)
        {
          T obj = local1;
          ref T local2 = ref obj;
          if ((object) obj == null)
            return;
          local1 = ref local2;
        }
        local1.Dispose();
      }
    }
  }
}
