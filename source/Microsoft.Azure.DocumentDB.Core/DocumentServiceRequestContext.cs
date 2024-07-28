// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.DocumentServiceRequestContext
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using Microsoft.Azure.Documents.Routing;
using System;
using System.Collections.Generic;

namespace Microsoft.Azure.Documents
{
  internal sealed class DocumentServiceRequestContext
  {
    public TimeoutHelper TimeoutHelper { get; set; }

    public RequestChargeTracker RequestChargeTracker { get; set; }

    public bool ForceRefreshAddressCache { get; set; }

    public StoreResult QuorumSelectedStoreResponse { get; set; }

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
      EnableConnectionStateListener = this.EnableConnectionStateListener
    };
  }
}
