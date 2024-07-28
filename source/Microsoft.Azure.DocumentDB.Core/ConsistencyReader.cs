// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.ConsistencyReader
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using Microsoft.Azure.Cosmos.Core.Trace;
using Microsoft.Azure.Documents.Collections;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace Microsoft.Azure.Documents
{
  [SuppressMessage("", "AvoidMultiLineComments", Justification = "Multi line business logic")]
  internal sealed class ConsistencyReader
  {
    private const int maxNumberOfSecondaryReadRetries = 3;
    private readonly AddressSelector addressSelector;
    private readonly IServiceConfigurationReader serviceConfigReader;
    private readonly IAuthorizationTokenProvider authorizationTokenProvider;
    private readonly StoreReader storeReader;
    private readonly QuorumReader quorumReader;

    public ConsistencyReader(
      AddressSelector addressSelector,
      ISessionContainer sessionContainer,
      TransportClient transportClient,
      IServiceConfigurationReader serviceConfigReader,
      IAuthorizationTokenProvider authorizationTokenProvider,
      ConnectionStateListener connectionStateListener)
    {
      this.addressSelector = addressSelector;
      this.serviceConfigReader = serviceConfigReader;
      this.authorizationTokenProvider = authorizationTokenProvider;
      this.storeReader = new StoreReader(transportClient, addressSelector, sessionContainer, connectionStateListener);
      this.quorumReader = new QuorumReader(transportClient, addressSelector, this.storeReader, serviceConfigReader, authorizationTokenProvider);
    }

    public string LastReadAddress
    {
      get => this.storeReader.LastReadAddress;
      set => this.storeReader.LastReadAddress = value;
    }

    public Task<StoreResponse> ReadAsync(
      DocumentServiceRequest entity,
      TimeoutHelper timeout,
      bool isInRetry,
      bool forceRefresh)
    {
      if (!isInRetry)
        timeout.ThrowTimeoutIfElapsed();
      else
        timeout.ThrowGoneIfElapsed();
      entity.RequestContext.TimeoutHelper = timeout;
      if (entity.RequestContext.RequestChargeTracker == null)
        entity.RequestContext.RequestChargeTracker = new RequestChargeTracker();
      if (entity.RequestContext.ClientRequestStatistics == null)
        entity.RequestContext.ClientRequestStatistics = (IClientSideRequestStatistics) new ClientSideRequestStatistics();
      entity.RequestContext.ForceRefreshAddressCache = forceRefresh;
      ConsistencyLevel targetConsistencyLevel;
      bool useSessionToken;
      ReadMode readMode = this.DeduceReadMode(entity, out targetConsistencyLevel, out useSessionToken);
      int maxReplicaSetSize = this.GetMaxReplicaSetSize(entity);
      int readQuorumValue = maxReplicaSetSize - maxReplicaSetSize / 2;
      switch (readMode)
      {
        case ReadMode.Primary:
          return this.ReadPrimaryAsync(entity, useSessionToken);
        case ReadMode.Strong:
          entity.RequestContext.PerformLocalRefreshOnGoneException = true;
          return this.quorumReader.ReadStrongAsync(entity, readQuorumValue, readMode);
        case ReadMode.BoundedStaleness:
          entity.RequestContext.PerformLocalRefreshOnGoneException = true;
          return this.quorumReader.ReadStrongAsync(entity, readQuorumValue, readMode);
        case ReadMode.Any:
          return targetConsistencyLevel == ConsistencyLevel.Session ? this.ReadSessionAsync(entity, readMode) : this.ReadAnyAsync(entity, readMode);
        default:
          throw new InvalidOperationException();
      }
    }

    private async Task<StoreResponse> ReadPrimaryAsync(
      DocumentServiceRequest entity,
      bool useSessionToken)
    {
      return (await this.storeReader.ReadPrimaryAsync(entity, false, useSessionToken)).ToResponse();
    }

    private async Task<StoreResponse> ReadAnyAsync(DocumentServiceRequest entity, ReadMode readMode)
    {
      IList<StoreResult> storeResultList = await this.storeReader.ReadMultipleReplicaAsync(entity, true, 1, false, false, readMode);
      return storeResultList.Count != 0 ? storeResultList[0].ToResponse() : throw new GoneException(RMResources.Gone);
    }

    private async Task<StoreResponse> ReadSessionAsync(
      DocumentServiceRequest entity,
      ReadMode readMode)
    {
      entity.RequestContext.TimeoutHelper.ThrowGoneIfElapsed();
      IList<StoreResult> storeResultList = await this.storeReader.ReadMultipleReplicaAsync(entity, true, 1, true, true, readMode, true);
      if (storeResultList.Count > 0)
      {
        StoreResponse storeResponse;
        try
        {
          StoreResponse response = storeResultList[0].ToResponse(entity.RequestContext.RequestChargeTracker);
          if (response.Status == 404 && entity.IsValidStatusCodeForExceptionlessRetry(response.Status) && entity.RequestContext.SessionToken != null && storeResultList[0].SessionToken != null && !entity.RequestContext.SessionToken.IsValid(storeResultList[0].SessionToken))
          {
            DefaultTrace.TraceInformation("Convert to session read exception, request {0} Session Lsn {1}, responseLSN {2}", (object) entity.ResourceAddress, (object) entity.RequestContext.SessionToken.ConvertToString(), (object) storeResultList[0].LSN);
            INameValueCollection headers = (INameValueCollection) new DictionaryNameValueCollection();
            headers.Set("x-ms-substatus", 1002.ToString());
            throw new NotFoundException(RMResources.ReadSessionNotAvailable, headers);
          }
          storeResponse = response;
        }
        catch (NotFoundException ex)
        {
          if (entity.RequestContext.SessionToken != null && storeResultList[0].SessionToken != null && !entity.RequestContext.SessionToken.IsValid(storeResultList[0].SessionToken))
          {
            DefaultTrace.TraceInformation("Convert to session read exception, request {0} Session Lsn {1}, responseLSN {2}", (object) entity.ResourceAddress, (object) entity.RequestContext.SessionToken.ConvertToString(), (object) storeResultList[0].LSN);
            ex.Headers.Set("x-ms-substatus", 1002.ToString());
          }
          throw ex;
        }
        return storeResponse;
      }
      INameValueCollection headers1 = (INameValueCollection) new DictionaryNameValueCollection();
      headers1.Set("x-ms-substatus", 1002.ToString());
      ISessionToken sessionToken = entity.RequestContext.SessionToken;
      DefaultTrace.TraceInformation("Fail the session read {0}, request session token {1}", (object) entity.ResourceAddress, sessionToken == null ? (object) "<empty>" : (object) sessionToken.ConvertToString());
      throw new NotFoundException(RMResources.ReadSessionNotAvailable, headers1);
    }

    private ReadMode DeduceReadMode(
      DocumentServiceRequest request,
      out ConsistencyLevel targetConsistencyLevel,
      out bool useSessionToken)
    {
      targetConsistencyLevel = RequestHelper.GetConsistencyLevelToUse(this.serviceConfigReader, request);
      useSessionToken = targetConsistencyLevel == ConsistencyLevel.Session;
      if (request.DefaultReplicaIndex.HasValue)
      {
        useSessionToken = false;
        return ReadMode.Primary;
      }
      switch (targetConsistencyLevel)
      {
        case ConsistencyLevel.Strong:
          return ReadMode.Strong;
        case ConsistencyLevel.BoundedStaleness:
          return ReadMode.BoundedStaleness;
        case ConsistencyLevel.Session:
          return ReadMode.Any;
        case ConsistencyLevel.Eventual:
          return ReadMode.Any;
        case ConsistencyLevel.ConsistentPrefix:
          return ReadMode.Any;
        default:
          throw new InvalidOperationException();
      }
    }

    public int GetMaxReplicaSetSize(DocumentServiceRequest entity) => ReplicatedResourceClient.IsReadingFromMaster(entity.ResourceType, entity.OperationType) ? this.serviceConfigReader.SystemReplicationPolicy.MaxReplicaSetSize : this.serviceConfigReader.UserReplicationPolicy.MaxReplicaSetSize;

    public int GetMinReplicaSetSize(DocumentServiceRequest entity) => ReplicatedResourceClient.IsReadingFromMaster(entity.ResourceType, entity.OperationType) ? this.serviceConfigReader.SystemReplicationPolicy.MinReplicaSetSize : this.serviceConfigReader.UserReplicationPolicy.MinReplicaSetSize;
  }
}
