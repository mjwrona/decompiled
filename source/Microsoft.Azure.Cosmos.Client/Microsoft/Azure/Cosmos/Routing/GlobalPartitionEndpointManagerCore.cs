// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Routing.GlobalPartitionEndpointManagerCore
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.Core.Trace;
using Microsoft.Azure.Documents;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;


#nullable enable
namespace Microsoft.Azure.Cosmos.Routing
{
  internal sealed class GlobalPartitionEndpointManagerCore : GlobalPartitionEndpointManager
  {
    private readonly IGlobalEndpointManager globalEndpointManager;
    private readonly Lazy<ConcurrentDictionary<PartitionKeyRange, GlobalPartitionEndpointManagerCore.PartitionKeyRangeFailoverInfo>> PartitionKeyRangeToLocation = new Lazy<ConcurrentDictionary<PartitionKeyRange, GlobalPartitionEndpointManagerCore.PartitionKeyRangeFailoverInfo>>((Func<ConcurrentDictionary<PartitionKeyRange, GlobalPartitionEndpointManagerCore.PartitionKeyRangeFailoverInfo>>) (() => new ConcurrentDictionary<PartitionKeyRange, GlobalPartitionEndpointManagerCore.PartitionKeyRangeFailoverInfo>()));

    public GlobalPartitionEndpointManagerCore(IGlobalEndpointManager globalEndpointManager) => this.globalEndpointManager = globalEndpointManager ?? throw new ArgumentNullException(nameof (globalEndpointManager));

    private bool CanUsePartitionLevelFailoverLocations(DocumentServiceRequest request) => this.globalEndpointManager.ReadEndpoints.Count > 1 && (request.ResourceType == ResourceType.Document || request.ResourceType == ResourceType.StoredProcedure && request.OperationType == Microsoft.Azure.Documents.OperationType.ExecuteJavaScript) && !this.globalEndpointManager.CanUseMultipleWriteLocations(request);

    public override bool TryAddPartitionLevelLocationOverride(DocumentServiceRequest request)
    {
      if (request == null)
        throw new ArgumentNullException(nameof (request));
      if (request.RequestContext == null || !this.CanUsePartitionLevelFailoverLocations(request))
        return false;
      PartitionKeyRange partitionKeyRange = request.RequestContext.ResolvedPartitionKeyRange;
      GlobalPartitionEndpointManagerCore.PartitionKeyRangeFailoverInfo rangeFailoverInfo;
      if (partitionKeyRange == null || !this.PartitionKeyRangeToLocation.IsValueCreated || !this.PartitionKeyRangeToLocation.Value.TryGetValue(partitionKeyRange, out rangeFailoverInfo))
        return false;
      DefaultTrace.TraceVerbose("Partition level override. URI: {0}, PartitionKeyRange: {1}", (object) rangeFailoverInfo.Current, (object) partitionKeyRange.Id);
      request.RequestContext.RouteToLocation(rangeFailoverInfo.Current);
      return true;
    }

    public override bool TryMarkEndpointUnavailableForPartitionKeyRange(
      DocumentServiceRequest request)
    {
      if (request == null)
        throw new ArgumentNullException(nameof (request));
      if (request.IsReadOnlyRequest || request.RequestContext == null || !this.CanUsePartitionLevelFailoverLocations(request))
        return false;
      PartitionKeyRange partitionKeyRange = request.RequestContext.ResolvedPartitionKeyRange;
      if (partitionKeyRange == null)
        return false;
      Uri failedLocation = request.RequestContext.LocationEndpointToRoute;
      if (failedLocation == (Uri) null)
        return false;
      GlobalPartitionEndpointManagerCore.PartitionKeyRangeFailoverInfo orAdd = this.PartitionKeyRangeToLocation.Value.GetOrAdd(partitionKeyRange, (Func<PartitionKeyRange, GlobalPartitionEndpointManagerCore.PartitionKeyRangeFailoverInfo>) (_ => new GlobalPartitionEndpointManagerCore.PartitionKeyRangeFailoverInfo(failedLocation)));
      if (orAdd.TryMoveNextLocation((IReadOnlyCollection<Uri>) this.globalEndpointManager.ReadEndpoints, failedLocation))
      {
        DefaultTrace.TraceInformation("Partition level override added to new location. PartitionKeyRange: {0}, failedLocation: {1}, new location: {2}", (object) partitionKeyRange, (object) failedLocation, (object) orAdd.Current);
        return true;
      }
      DefaultTrace.TraceInformation("Partition level override removed. PartitionKeyRange: {0}, failedLocation: {1}", (object) partitionKeyRange, (object) failedLocation);
      this.PartitionKeyRangeToLocation.Value.TryRemove(partitionKeyRange, out GlobalPartitionEndpointManagerCore.PartitionKeyRangeFailoverInfo _);
      return false;
    }

    private sealed class PartitionKeyRangeFailoverInfo
    {
      private readonly HashSet<Uri> FailedLocations;

      public PartitionKeyRangeFailoverInfo(Uri currentLocation)
      {
        this.Current = currentLocation;
        this.FailedLocations = new HashSet<Uri>();
      }

      public Uri Current { get; private set; }

      public bool TryMoveNextLocation(IReadOnlyCollection<Uri> locations, Uri failedLocation)
      {
        if (failedLocation != this.Current)
          return true;
        lock (this.FailedLocations)
        {
          if (failedLocation != this.Current)
            return true;
          foreach (Uri location in (IEnumerable<Uri>) locations)
          {
            if (!(this.Current == location) && !this.FailedLocations.Contains(location))
            {
              this.FailedLocations.Add(failedLocation);
              this.Current = location;
              return true;
            }
          }
        }
        return false;
      }
    }
  }
}
