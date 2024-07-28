// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.DocumentServiceRequestContext
// Assembly: Microsoft.Azure.Cosmos.Direct, Version=3.29.4.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: FFE3C00D-4333-4294-8947-B1C93A852E2F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Direct.dll

using Microsoft.Azure.Documents.Routing;
using System;
using System.Collections.Generic;
using System.Net;

namespace Microsoft.Azure.Documents
{
  internal sealed class DocumentServiceRequestContext
  {
    private StoreResult quorumSelectedStoreResponse;

    public TimeoutHelper TimeoutHelper { get; set; }

    public RequestChargeTracker RequestChargeTracker { get; set; }

    public bool ForceRefreshAddressCache { get; set; }

    public int LastPartitionAddressInformationHashCode { get; set; }

    public StoreResult QuorumSelectedStoreResponse => this.quorumSelectedStoreResponse;

    public List<string> StoreResponses { get; set; }

    public ConsistencyLevel? OriginalRequestConsistencyLevel { get; set; }

    public long QuorumSelectedLSN { get; set; }

    public long GlobalCommittedSelectedLSN { get; set; }

    public StoreResult GlobalStrongWriteStoreResult { get; set; }

    public ServiceIdentity TargetIdentity { get; set; }

    public bool PerformLocalRefreshOnGoneException { get; set; }

    public PartitionKeyInternal EffectivePartitionKey { get; set; }

    public PartitionKeyRange ResolvedPartitionKeyRange { get; set; }

    public ISessionToken SessionToken { get; set; }

    public bool PerformedBackgroundAddressRefresh { get; set; }

    public IClientSideRequestStatistics ClientRequestStatistics { get; set; }

    public string ResolvedCollectionRid { get; set; }

    public string RegionName { get; set; }

    public bool LocalRegionRequest { get; set; }

    public bool IsRetry { get; set; }

    public Lazy<HashSet<TransportAddressUri>> FailedEndpoints { get; private set; }

    public DocumentServiceRequestContext() => this.FailedEndpoints = new Lazy<HashSet<TransportAddressUri>>();

    public void UpdateQuorumSelectedStoreResponse(StoreResult storeResult)
    {
      StoreResult selectedStoreResponse = this.quorumSelectedStoreResponse;
      if (selectedStoreResponse == storeResult)
        return;
      selectedStoreResponse?.Dispose();
      this.quorumSelectedStoreResponse = storeResult;
    }

    public void AddToFailedEndpoints(Exception storeException, TransportAddressUri targetUri)
    {
      if (!(storeException is DocumentClientException documentClientException))
        return;
      HttpStatusCode? statusCode = documentClientException.StatusCode;
      HttpStatusCode httpStatusCode1 = HttpStatusCode.Gone;
      if (!(statusCode.GetValueOrDefault() == httpStatusCode1 & statusCode.HasValue))
      {
        statusCode = documentClientException.StatusCode;
        HttpStatusCode httpStatusCode2 = HttpStatusCode.RequestTimeout;
        if (!(statusCode.GetValueOrDefault() == httpStatusCode2 & statusCode.HasValue))
        {
          statusCode = documentClientException.StatusCode;
          if (statusCode.Value < HttpStatusCode.InternalServerError)
            return;
        }
      }
      this.FailedEndpoints.Value.Add(targetUri);
    }

    public void RouteToLocation(int locationIndex, bool usePreferredLocations)
    {
      this.LocationIndexToRoute = new int?(locationIndex);
      this.UsePreferredLocations = new bool?(usePreferredLocations);
      this.LocationEndpointToRoute = (Uri) null;
    }

    public void RouteToLocation(Uri locationEndpoint)
    {
      this.LocationEndpointToRoute = locationEndpoint;
      this.LocationIndexToRoute = new int?();
      this.UsePreferredLocations = new bool?();
    }

    public void ClearRouteToLocation()
    {
      this.LocationIndexToRoute = new int?();
      this.LocationEndpointToRoute = (Uri) null;
      this.UsePreferredLocations = new bool?();
    }

    public bool? UsePreferredLocations { get; private set; }

    public int? LocationIndexToRoute { get; private set; }

    public Uri LocationEndpointToRoute { get; private set; }

    public bool EnsureCollectionExistsCheck { get; set; }

    public bool EnableConnectionStateListener { get; set; }

    public string SerializedSourceCollectionForMaterializedView { get; set; }

    public DocumentServiceRequestContext Clone() => new DocumentServiceRequestContext()
    {
      TimeoutHelper = this.TimeoutHelper,
      RequestChargeTracker = this.RequestChargeTracker,
      ForceRefreshAddressCache = this.ForceRefreshAddressCache,
      TargetIdentity = this.TargetIdentity,
      PerformLocalRefreshOnGoneException = this.PerformLocalRefreshOnGoneException,
      SessionToken = this.SessionToken,
      ResolvedPartitionKeyRange = this.ResolvedPartitionKeyRange,
      PerformedBackgroundAddressRefresh = this.PerformedBackgroundAddressRefresh,
      ResolvedCollectionRid = this.ResolvedCollectionRid,
      EffectivePartitionKey = this.EffectivePartitionKey,
      ClientRequestStatistics = this.ClientRequestStatistics,
      OriginalRequestConsistencyLevel = this.OriginalRequestConsistencyLevel,
      UsePreferredLocations = this.UsePreferredLocations,
      LocationIndexToRoute = this.LocationIndexToRoute,
      LocationEndpointToRoute = this.LocationEndpointToRoute,
      EnsureCollectionExistsCheck = this.EnsureCollectionExistsCheck,
      EnableConnectionStateListener = this.EnableConnectionStateListener,
      LocalRegionRequest = this.LocalRegionRequest,
      FailedEndpoints = this.FailedEndpoints,
      LastPartitionAddressInformationHashCode = this.LastPartitionAddressInformationHashCode
    };
  }
}
