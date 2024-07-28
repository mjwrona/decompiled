// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.StoreReader
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using Microsoft.Azure.Cosmos.Core.Trace;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.Azure.Documents
{
  internal sealed class StoreReader
  {
    private readonly TransportClient transportClient;
    private readonly AddressSelector addressSelector;
    private readonly ISessionContainer sessionContainer;
    private readonly ConnectionStateListener connectionStateListener;
    private readonly bool canUseLocalLSNBasedHeaders;
    [ThreadStatic]
    private static Random random;

    public StoreReader(
      TransportClient transportClient,
      AddressSelector addressSelector,
      ISessionContainer sessionContainer,
      ConnectionStateListener connectionStateListener)
    {
      this.transportClient = transportClient;
      this.addressSelector = addressSelector;
      this.sessionContainer = sessionContainer;
      this.connectionStateListener = connectionStateListener;
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
      IList<StoreResult> responses;
      try
      {
        StoreReader.ReadReplicaResult readReplicaResult = await this.ReadMultipleReplicasInternalAsync(entity, includePrimary, replicaCountToRead, requiresValidLsn, useSessionToken, readMode, checkMinLSN, forceReadAll);
        if (entity.RequestContext.PerformLocalRefreshOnGoneException && readReplicaResult.RetryWithForceRefresh && !entity.RequestContext.ForceRefreshAddressCache)
        {
          entity.RequestContext.TimeoutHelper.ThrowGoneIfElapsed();
          entity.RequestContext.ForceRefreshAddressCache = true;
          readReplicaResult = await this.ReadMultipleReplicasInternalAsync(entity, includePrimary, replicaCountToRead, requiresValidLsn, useSessionToken, readMode, forceReadAll: forceReadAll);
        }
        responses = readReplicaResult.Responses;
      }
      finally
      {
        SessionTokenHelper.SetOriginalSessionToken(entity, originalSessionToken);
      }
      return responses;
    }

    public async Task<StoreResult> ReadPrimaryAsync(
      DocumentServiceRequest entity,
      bool requiresValidLsn,
      bool useSessionToken)
    {
      entity.RequestContext.TimeoutHelper.ThrowGoneIfElapsed();
      string originalSessionToken = entity.Headers["x-ms-session-token"];
      StoreResult response;
      try
      {
        StoreReader.ReadReplicaResult readReplicaResult = await this.ReadPrimaryInternalAsync(entity, requiresValidLsn, useSessionToken);
        if (entity.RequestContext.PerformLocalRefreshOnGoneException && readReplicaResult.RetryWithForceRefresh && !entity.RequestContext.ForceRefreshAddressCache)
        {
          entity.RequestContext.TimeoutHelper.ThrowGoneIfElapsed();
          entity.RequestContext.ForceRefreshAddressCache = true;
          readReplicaResult = await this.ReadPrimaryInternalAsync(entity, requiresValidLsn, useSessionToken);
        }
        if (readReplicaResult.Responses.Count == 0)
          throw new GoneException(RMResources.Gone);
        response = readReplicaResult.Responses[0];
      }
      finally
      {
        SessionTokenHelper.SetOriginalSessionToken(entity, originalSessionToken);
      }
      return response;
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
      List<StoreResult> responseResult = new List<StoreResult>();
      string requestedCollectionRid = entity.RequestContext.ResolvedCollectionRid;
      Tuple<IReadOnlyList<Uri>, AddressCacheToken> tuple = await this.addressSelector.ResolveAllUriAsync(entity, includePrimary, entity.RequestContext.ForceRefreshAddressCache);
      List<Uri> resolveApiResults = tuple.Item1.ToList<Uri>();
      this.connectionStateListener?.UpdateConnectionState(tuple.Item1, tuple.Item2);
      if (!string.IsNullOrEmpty(requestedCollectionRid) && !string.IsNullOrEmpty(entity.RequestContext.ResolvedCollectionRid) && !requestedCollectionRid.Equals(entity.RequestContext.ResolvedCollectionRid))
        this.sessionContainer.ClearTokenByResourceId(requestedCollectionRid);
      int resolvedAddressCount = resolveApiResults.Count;
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
        return entity.RequestContext.ForceRefreshAddressCache ? new StoreReader.ReadReplicaResult(false, (IList<StoreResult>) responseResult) : new StoreReader.ReadReplicaResult(true, (IList<StoreResult>) responseResult);
      int num1 = replicaCountToRead;
      string header = entity.Headers["x-ms-version"];
      bool enforceSessionCheck = !string.IsNullOrEmpty(header) && VersionUtility.IsLaterThan(header, HttpConstants.Versions.v2016_05_30);
      bool hasGoneException = false;
      Exception exceptionToThrow = (Exception) null;
      while (num1 > 0 && resolveApiResults.Count > 0)
      {
        entity.RequestContext.TimeoutHelper.ThrowGoneIfElapsed();
        Dictionary<Task<StoreResponse>, Uri> readStoreTasks = new Dictionary<Task<StoreResponse>, Uri>();
        int nextRandom = StoreReader.GenerateNextRandom(resolveApiResults.Count);
        while (resolveApiResults.Count > 0)
        {
          nextRandom %= resolveApiResults.Count;
          readStoreTasks.Add(this.ReadFromStoreAsync(resolveApiResults[nextRandom], entity), resolveApiResults[nextRandom]);
          resolveApiResults.RemoveAt(nextRandom);
          if (!forceReadAll && readStoreTasks.Count == num1)
            break;
        }
        int num2 = readStoreTasks.Count >= num1 ? 0 : num1 - readStoreTasks.Count;
        try
        {
          StoreResponse[] storeResponseArray = await Task.WhenAll<StoreResponse>((IEnumerable<Task<StoreResponse>>) readStoreTasks.Keys);
        }
        catch (Exception ex)
        {
          DefaultTrace.TraceInformation("Exception {0} is thrown while doing readMany", (object) ex);
          exceptionToThrow = ex;
        }
        foreach (Task<StoreResponse> key in readStoreTasks.Keys)
        {
          StoreResponse result = key.Exception == null ? key.Result : (StoreResponse) null;
          Exception innerException = key.Exception != null ? key.Exception.InnerException : (Exception) null;
          Uri storePhysicalAddress = readStoreTasks[key];
          StoreResult storeResult = StoreResult.CreateStoreResult(result, innerException, requiresValidLsn, this.canUseLocalLSNBasedHeaders && readMode != ReadMode.Strong, storePhysicalAddress);
          entity.RequestContext.RequestChargeTracker.AddCharge(storeResult.RequestCharge);
          if (result != null)
            entity.RequestContext.ClientRequestStatistics.ContactedReplicas.Add(storePhysicalAddress);
          if (innerException != null && innerException.InnerException is TransportException)
            entity.RequestContext.ClientRequestStatistics.FailedReplicas.Add(storePhysicalAddress);
          entity.RequestContext.ClientRequestStatistics.RecordResponse(entity, storeResult);
          if (storeResult.IsValid && (requestSessionToken == null || storeResult.SessionToken != null && requestSessionToken.IsValid(storeResult.SessionToken) || !enforceSessionCheck && storeResult.StatusCode != StatusCodes.NotFound))
            responseResult.Add(storeResult);
          hasGoneException = ((hasGoneException ? 1 : 0) | (storeResult.StatusCode != StatusCodes.Gone ? 0 : (storeResult.SubStatusCode != SubStatusCodes.NameCacheIsStale ? 1 : 0))) != 0;
        }
        if (responseResult.Count >= replicaCountToRead)
        {
          if (hasGoneException && !entity.RequestContext.PerformedBackgroundAddressRefresh)
          {
            this.StartBackgroundAddressRefresh(entity);
            entity.RequestContext.PerformedBackgroundAddressRefresh = true;
          }
          return new StoreReader.ReadReplicaResult(false, (IList<StoreResult>) responseResult);
        }
        num1 = replicaCountToRead - responseResult.Count;
        readStoreTasks = (Dictionary<Task<StoreResponse>, Uri>) null;
      }
      if (responseResult.Count < replicaCountToRead)
      {
        DefaultTrace.TraceInformation("Could not get quorum number of responses. ValidResponsesReceived: {0} ResponsesExpected: {1}, ResolvedAddressCount: {2}, ResponsesString: {3}", (object) responseResult.Count, (object) replicaCountToRead, (object) resolvedAddressCount, (object) string.Join<StoreResult>(";", (IEnumerable<StoreResult>) responseResult));
        if (hasGoneException)
        {
          if (!entity.RequestContext.PerformLocalRefreshOnGoneException)
            throw new GoneException(exceptionToThrow);
          if (!entity.RequestContext.ForceRefreshAddressCache)
            return new StoreReader.ReadReplicaResult(true, (IList<StoreResult>) responseResult);
        }
      }
      return new StoreReader.ReadReplicaResult(false, (IList<StoreResult>) responseResult);
    }

    private async Task<StoreReader.ReadReplicaResult> ReadPrimaryInternalAsync(
      DocumentServiceRequest entity,
      bool requiresValidLsn,
      bool useSessionToken)
    {
      entity.RequestContext.TimeoutHelper.ThrowGoneIfElapsed();
      Tuple<Uri, AddressCacheToken> addressInfo = await this.addressSelector.ResolvePrimaryUriAsync(entity, entity.RequestContext.ForceRefreshAddressCache);
      this.connectionStateListener?.UpdateConnectionState(addressInfo.Item1, addressInfo.Item2);
      if (useSessionToken)
        SessionTokenHelper.SetPartitionLocalSessionToken(entity, this.sessionContainer);
      else
        entity.Headers.Remove("x-ms-session-token");
      Exception storeTaskException = (Exception) null;
      StoreResponse storeResponse = (StoreResponse) null;
      try
      {
        storeResponse = await this.ReadFromStoreAsync(addressInfo.Item1, entity);
      }
      catch (Exception ex)
      {
        storeTaskException = ex;
        DefaultTrace.TraceInformation("Exception {0} is thrown while doing Read Primary", (object) ex);
      }
      StoreResult storeResult = StoreResult.CreateStoreResult(storeResponse, storeTaskException, requiresValidLsn, this.canUseLocalLSNBasedHeaders, addressInfo.Item1);
      entity.RequestContext.ClientRequestStatistics.RecordResponse(entity, storeResult);
      entity.RequestContext.RequestChargeTracker.AddCharge(storeResult.RequestCharge);
      if (storeResult.StatusCode == StatusCodes.Gone && storeResult.SubStatusCode != SubStatusCodes.NameCacheIsStale)
        return new StoreReader.ReadReplicaResult(true, (IList<StoreResult>) new List<StoreResult>());
      return new StoreReader.ReadReplicaResult(false, (IList<StoreResult>) new StoreResult[1]
      {
        storeResult
      });
    }

    private async Task<StoreResponse> ReadFromStoreAsync(
      Uri physicalAddress,
      DocumentServiceRequest request)
    {
      request.RequestContext.TimeoutHelper.ThrowGoneIfElapsed();
      QueryRequestPerformanceActivity activity = (QueryRequestPerformanceActivity) null;
      string header1 = request.Headers["If-None-Match"];
      this.LastReadAddress = physicalAddress.ToString();
      if (request.OperationType == OperationType.ReadFeed || request.OperationType == OperationType.Query)
      {
        string source = request.Headers["x-ms-continuation"];
        string header2 = request.Headers["x-ms-max-item-count"];
        if (source != null && source.Contains<char>(';'))
        {
          string[] strArray = source.Split(';');
          source = strArray.Length >= 3 ? strArray[0] : throw new BadRequestException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, RMResources.InvalidHeaderValue, (object) source, (object) "x-ms-continuation"));
        }
        request.Continuation = source;
        activity = CustomTypeExtensions.StartActivity(request);
      }
      switch (request.OperationType)
      {
        case OperationType.ExecuteJavaScript:
        case OperationType.ReadFeed:
        case OperationType.SqlQuery:
        case OperationType.Query:
        case OperationType.HeadFeed:
          return await StoreReader.CompleteActivity(this.transportClient.InvokeResourceOperationAsync(physicalAddress, request), activity);
        case OperationType.Read:
        case OperationType.Head:
          return await this.transportClient.InvokeResourceOperationAsync(physicalAddress, request);
        default:
          throw new InvalidOperationException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Unexpected operation type {0}", (object) request.OperationType));
      }
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

    private void StartBackgroundAddressRefresh(DocumentServiceRequest request)
    {
      try
      {
        this.addressSelector.ResolveAllUriAsync(request, true, true).ContinueWith((Action<Task<Tuple<IReadOnlyList<Uri>, AddressCacheToken>>>) (task =>
        {
          if (!task.IsFaulted)
            return;
          DefaultTrace.TraceWarning("Background refresh of the addresses failed with {0}", (object) task.Exception.ToString());
        }));
      }
      catch (Exception ex)
      {
        DefaultTrace.TraceWarning("Background refresh of the addresses failed with {0}", (object) ex.ToString());
      }
    }

    private static int GenerateNextRandom(int maxValue)
    {
      if (StoreReader.random == null)
        StoreReader.random = CustomTypeExtensions.GetRandomNumber();
      return StoreReader.random.Next(maxValue);
    }

    private sealed class ReadReplicaResult
    {
      public ReadReplicaResult(bool retryWithForceRefresh, IList<StoreResult> responses)
      {
        this.RetryWithForceRefresh = retryWithForceRefresh;
        this.Responses = responses;
      }

      public bool RetryWithForceRefresh { get; private set; }

      public IList<StoreResult> Responses { get; private set; }
    }
  }
}
