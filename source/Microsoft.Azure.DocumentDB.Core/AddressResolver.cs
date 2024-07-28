// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.AddressResolver
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using Microsoft.Azure.Cosmos.Core.Trace;
using Microsoft.Azure.Documents.Common;
using Microsoft.Azure.Documents.Routing;
using Newtonsoft.Json;
using System;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Documents
{
  internal sealed class AddressResolver : IAddressResolver
  {
    private CollectionCache collectionCache;
    private ICollectionRoutingMapCache collectionRoutingMapCache;
    private IAddressCache addressCache;
    private readonly IMasterServiceIdentityProvider masterServiceIdentityProvider;
    private readonly IRequestSigner requestSigner;
    private readonly string location;
    private readonly PartitionKeyRangeIdentity masterPartitionKeyRangeIdentity = new PartitionKeyRangeIdentity("M");

    public AddressResolver(
      IMasterServiceIdentityProvider masterServiceIdentityProvider,
      IRequestSigner requestSigner,
      string location)
    {
      this.masterServiceIdentityProvider = masterServiceIdentityProvider;
      this.requestSigner = requestSigner;
      this.location = location;
    }

    public void InitializeCaches(
      CollectionCache collectionCache,
      ICollectionRoutingMapCache collectionRoutingMapCache,
      IAddressCache addressCache)
    {
      this.collectionCache = collectionCache;
      this.addressCache = addressCache;
      this.collectionRoutingMapCache = collectionRoutingMapCache;
    }

    public async Task<PartitionAddressInformation> ResolveAsync(
      DocumentServiceRequest request,
      bool forceRefreshPartitionAddresses,
      CancellationToken cancellationToken)
    {
      cancellationToken.ThrowIfCancellationRequested();
      AddressResolver.ResolutionResult result = await this.ResolveAddressesAndIdentityAsync(request, forceRefreshPartitionAddresses, cancellationToken);
      this.ThrowIfTargetChanged(request, result.TargetPartitionKeyRange);
      request.RequestContext.TargetIdentity = result.TargetServiceIdentity;
      request.RequestContext.ResolvedPartitionKeyRange = result.TargetPartitionKeyRange;
      request.RequestContext.RegionName = this.location;
      await this.requestSigner.SignRequestAsync(request, cancellationToken);
      return result.Addresses;
    }

    private static bool IsSameCollection(
      PartitionKeyRange initiallyResolved,
      PartitionKeyRange newlyResolved)
    {
      if (initiallyResolved == null)
        throw new ArgumentException("parent");
      if (newlyResolved == null)
        return false;
      if (initiallyResolved.Id == "M" && newlyResolved.Id == "M")
        return true;
      if (initiallyResolved.Id == "M" || newlyResolved.Id == "M")
      {
        DefaultTrace.TraceCritical("Request was resolved to master partition and then to server partition.");
        return false;
      }
      if ((int) ResourceId.Parse(initiallyResolved.ResourceId).DocumentCollection != (int) ResourceId.Parse(newlyResolved.ResourceId).DocumentCollection)
        return false;
      if (!(initiallyResolved.Id != newlyResolved.Id) || newlyResolved.Parents != null && newlyResolved.Parents.Contains(initiallyResolved.Id))
        return true;
      DefaultTrace.TraceCritical("Request is targeted at a partition key range which is not child of previously targeted range.");
      return false;
    }

    private void ThrowIfTargetChanged(DocumentServiceRequest request, PartitionKeyRange targetRange)
    {
      if (request.RequestContext.ResolvedPartitionKeyRange != null && !AddressResolver.IsSameCollection(request.RequestContext.ResolvedPartitionKeyRange, targetRange))
      {
        if (!request.IsNameBased)
          DefaultTrace.TraceCritical(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Target should not change for non name based requests. Previous target {0}, Current {1}", (object) request.RequestContext.ResolvedPartitionKeyRange, (object) targetRange));
        request.RequestContext.TargetIdentity = (ServiceIdentity) null;
        request.RequestContext.ResolvedPartitionKeyRange = (PartitionKeyRange) null;
        InvalidPartitionException partitionException = new InvalidPartitionException(RMResources.InvalidTarget);
        partitionException.ResourceAddress = request.ResourceAddress;
        throw partitionException;
      }
    }

    private async Task<AddressResolver.ResolutionResult> ResolveAddressesAndIdentityAsync(
      DocumentServiceRequest request,
      bool forceRefreshPartitionAddresses,
      CancellationToken cancellationToken)
    {
      cancellationToken.ThrowIfCancellationRequested();
      ServiceIdentity identity;
      if (request.ServiceIdentity != null)
      {
        if (request.ServiceIdentity.IsMasterService && request.ForceMasterRefresh && this.masterServiceIdentityProvider != null)
        {
          await this.masterServiceIdentityProvider.RefreshAsync(request.ServiceIdentity, cancellationToken);
          ServiceIdentity masterServiceIdentity = this.masterServiceIdentityProvider.MasterServiceIdentity;
          bool flag = masterServiceIdentity != null && !masterServiceIdentity.Equals((object) request.ServiceIdentity);
          DefaultTrace.TraceInformation("Refreshed master service identity. masterServiceIdentityChanged = {0}, previousRequestServiceIdentity = {1}, newMasterServiceIdentity = {2}", (object) flag, (object) request.ServiceIdentity, (object) masterServiceIdentity);
          if (flag)
            request.RouteTo(masterServiceIdentity);
        }
        identity = request.ServiceIdentity;
        PartitionAddressInformation addresses = await this.addressCache.TryGetAddresses(request, (PartitionKeyRangeIdentity) null, identity, forceRefreshPartitionAddresses, cancellationToken);
        if (addresses == null && identity.IsMasterService && this.masterServiceIdentityProvider != null)
        {
          DefaultTrace.TraceWarning("Could not get addresses for MasterServiceIdentity {0}. will refresh masterServiceIdentity and retry", (object) identity);
          await this.masterServiceIdentityProvider.RefreshAsync(identity, cancellationToken);
          identity = this.masterServiceIdentityProvider.MasterServiceIdentity;
          addresses = await this.addressCache.TryGetAddresses(request, (PartitionKeyRangeIdentity) null, identity, forceRefreshPartitionAddresses, cancellationToken);
        }
        if (addresses == null)
        {
          DefaultTrace.TraceInformation("Could not get addresses for explicitly specified ServiceIdentity {0}", (object) identity);
          NotFoundException notFoundException = new NotFoundException();
          notFoundException.ResourceAddress = request.ResourceAddress;
          throw notFoundException;
        }
        return new AddressResolver.ResolutionResult(addresses, identity);
      }
      if (ReplicatedResourceClient.IsReadingFromMaster(request.ResourceType, request.OperationType) && request.PartitionKeyRangeIdentity == null)
      {
        DefaultTrace.TraceInformation("Resolving Master service address, forceMasterRefresh: {0}, currentMaster: {1}", (object) request.ForceMasterRefresh, (object) this.masterServiceIdentityProvider?.MasterServiceIdentity);
        if (request.ForceMasterRefresh && this.masterServiceIdentityProvider != null)
          await this.masterServiceIdentityProvider.RefreshAsync(this.masterServiceIdentityProvider.MasterServiceIdentity, cancellationToken);
        identity = this.masterServiceIdentityProvider?.MasterServiceIdentity;
        PartitionKeyRangeIdentity partitionKeyRangeIdentity = this.masterPartitionKeyRangeIdentity;
        PartitionAddressInformation addresses = await this.addressCache.TryGetAddresses(request, partitionKeyRangeIdentity, identity, forceRefreshPartitionAddresses, cancellationToken);
        if (addresses == null && this.masterServiceIdentityProvider != null)
        {
          DefaultTrace.TraceWarning("Could not get addresses for master partition {0} on first attempt, will refresh masterServiceIdentity and retry", (object) identity);
          await this.masterServiceIdentityProvider.RefreshAsync(this.masterServiceIdentityProvider.MasterServiceIdentity, cancellationToken);
          identity = this.masterServiceIdentityProvider.MasterServiceIdentity;
          addresses = await this.addressCache.TryGetAddresses(request, partitionKeyRangeIdentity, identity, forceRefreshPartitionAddresses, cancellationToken);
        }
        if (addresses == null)
        {
          DefaultTrace.TraceCritical("Could not get addresses for master partition {0}", (object) identity);
          NotFoundException notFoundException = new NotFoundException();
          notFoundException.ResourceAddress = request.ResourceAddress;
          throw notFoundException;
        }
        PartitionKeyRange targetPartitionKeyRange = new PartitionKeyRange();
        targetPartitionKeyRange.Id = "M";
        return new AddressResolver.ResolutionResult(targetPartitionKeyRange, addresses, identity);
      }
      bool collectionCacheIsUptoDate = !request.IsNameBased || request.PartitionKeyRangeIdentity != null && request.PartitionKeyRangeIdentity.CollectionRid != null;
      bool collectionRoutingMapCacheIsUptoDate = false;
      DocumentCollection collection = await this.collectionCache.ResolveCollectionAsync(request, cancellationToken);
      CollectionRoutingMap routingMap = await this.collectionRoutingMapCache.TryLookupAsync(collection.ResourceId, (CollectionRoutingMap) null, request, cancellationToken);
      if (routingMap != null && request.ForceCollectionRoutingMapRefresh)
      {
        DefaultTrace.TraceInformation("AddressResolver.ResolveAddressesAndIdentityAsync ForceCollectionRoutingMapRefresh collection.ResourceId = {0}", (object) collection.ResourceId);
        routingMap = await this.collectionRoutingMapCache.TryLookupAsync(collection.ResourceId, routingMap, request, cancellationToken);
      }
      if (request.ForcePartitionKeyRangeRefresh)
      {
        collectionRoutingMapCacheIsUptoDate = true;
        request.ForcePartitionKeyRangeRefresh = false;
        if (routingMap != null)
          routingMap = await this.collectionRoutingMapCache.TryLookupAsync(collection.ResourceId, routingMap, request, cancellationToken);
      }
      if (routingMap == null && !collectionCacheIsUptoDate)
      {
        request.ForceNameCacheRefresh = true;
        collectionCacheIsUptoDate = true;
        collectionRoutingMapCacheIsUptoDate = false;
        collection = await this.collectionCache.ResolveCollectionAsync(request, cancellationToken);
        routingMap = await this.collectionRoutingMapCache.TryLookupAsync(collection.ResourceId, (CollectionRoutingMap) null, request, cancellationToken);
      }
      AddressResolver.EnsureRoutingMapPresent(request, routingMap, collection);
      AddressResolver.ResolutionResult resolutionResult = await this.TryResolveServerPartitionAsync(request, collection, routingMap, collectionCacheIsUptoDate, collectionRoutingMapCacheIsUptoDate, forceRefreshPartitionAddresses, cancellationToken);
      if (resolutionResult == null)
      {
        if (!collectionCacheIsUptoDate)
        {
          request.ForceNameCacheRefresh = true;
          collectionCacheIsUptoDate = true;
          collection = await this.collectionCache.ResolveCollectionAsync(request, cancellationToken);
          if (collection.ResourceId != routingMap.CollectionUniqueId)
          {
            collectionRoutingMapCacheIsUptoDate = false;
            routingMap = await this.collectionRoutingMapCache.TryLookupAsync(collection.ResourceId, (CollectionRoutingMap) null, request, cancellationToken);
          }
        }
        if (!collectionRoutingMapCacheIsUptoDate)
        {
          collectionRoutingMapCacheIsUptoDate = true;
          routingMap = await this.collectionRoutingMapCache.TryLookupAsync(collection.ResourceId, routingMap, request, cancellationToken);
        }
        AddressResolver.EnsureRoutingMapPresent(request, routingMap, collection);
        resolutionResult = await this.TryResolveServerPartitionAsync(request, collection, routingMap, true, true, forceRefreshPartitionAddresses, cancellationToken);
      }
      if (resolutionResult == null)
      {
        DefaultTrace.TraceInformation("Couldn't route partitionkeyrange-oblivious request after retry/cache refresh. Collection doesn't exist.");
        NotFoundException notFoundException = new NotFoundException();
        notFoundException.ResourceAddress = request.ResourceAddress;
        throw notFoundException;
      }
      if (request.IsNameBased)
        request.Headers["x-ms-documentdb-collection-rid"] = collection.ResourceId;
      return resolutionResult;
    }

    private static void EnsureRoutingMapPresent(
      DocumentServiceRequest request,
      CollectionRoutingMap routingMap,
      DocumentCollection collection)
    {
      if (routingMap == null && request.IsNameBased && request.PartitionKeyRangeIdentity != null && request.PartitionKeyRangeIdentity.CollectionRid != null)
      {
        DefaultTrace.TraceInformation("Routing map for request with partitionkeyrageid {0} was not found", (object) request.PartitionKeyRangeIdentity.ToHeader());
        InvalidPartitionException partitionException = new InvalidPartitionException();
        partitionException.ResourceAddress = request.ResourceAddress;
        throw partitionException;
      }
      if (routingMap == null)
      {
        DefaultTrace.TraceInformation("Routing map was not found although collection cache is upto date for collection {0}", (object) collection.ResourceId);
        NotFoundException notFoundException = new NotFoundException();
        notFoundException.ResourceAddress = request.ResourceAddress;
        throw notFoundException;
      }
    }

    private async Task<AddressResolver.ResolutionResult> TryResolveServerPartitionAsync(
      DocumentServiceRequest request,
      DocumentCollection collection,
      CollectionRoutingMap routingMap,
      bool collectionCacheIsUptodate,
      bool collectionRoutingMapCacheIsUptodate,
      bool forceRefreshPartitionAddresses,
      CancellationToken cancellationToken)
    {
      if (request.PartitionKeyRangeIdentity != null)
        return await this.TryResolveServerPartitionByPartitionKeyRangeIdAsync(request, collection, routingMap, collectionCacheIsUptodate, collectionRoutingMapCacheIsUptodate, forceRefreshPartitionAddresses, cancellationToken);
      if (!request.ResourceType.IsPartitioned() && (request.ResourceType != ResourceType.StoredProcedure || request.OperationType != OperationType.ExecuteJavaScript) && (request.ResourceType != ResourceType.Collection || request.OperationType != OperationType.Head))
      {
        DefaultTrace.TraceCritical("Shouldn't come here for non partitioned resources. resourceType : {0}, operationtype:{1}, resourceaddress:{2}", (object) request.ResourceType, (object) request.OperationType, (object) request.ResourceAddress);
        InternalServerErrorException serverErrorException = new InternalServerErrorException(RMResources.InternalServerError);
        serverErrorException.ResourceAddress = request.ResourceAddress;
        throw serverErrorException;
      }
      string header = request.Headers["x-ms-documentdb-partitionkey"];
      object obj = (object) null;
      PartitionKeyRange range;
      if (header != null)
        range = this.TryResolveServerPartitionByPartitionKey(request, header, collectionCacheIsUptodate, collection, routingMap);
      else if (request.Properties != null && request.Properties.TryGetValue("x-ms-effective-partition-key-string", out obj))
      {
        if (!collection.HasPartitionKey || collection.PartitionKey.IsSystemKey.GetValueOrDefault(false))
          throw new ArgumentOutOfRangeException(nameof (collection));
        string effectivePartitionKeyValue = obj as string;
        range = !string.IsNullOrEmpty(effectivePartitionKeyValue) ? routingMap.GetRangeByEffectivePartitionKey(effectivePartitionKeyValue) : throw new ArgumentOutOfRangeException("effectivePartitionKeyString");
      }
      else
        range = this.TryResolveSinglePartitionCollection(request, collection, routingMap, collectionCacheIsUptodate);
      if (range == null)
        return (AddressResolver.ResolutionResult) null;
      ServiceIdentity serviceIdentity = routingMap.TryGetInfoByPartitionKeyRangeId(range.Id);
      PartitionAddressInformation addresses = await this.addressCache.TryGetAddresses(request, new PartitionKeyRangeIdentity(collection.ResourceId, range.Id), serviceIdentity, forceRefreshPartitionAddresses, cancellationToken);
      if (addresses != null)
        return new AddressResolver.ResolutionResult(range, addresses, serviceIdentity);
      DefaultTrace.TraceVerbose("Could not resolve addresses for identity {0}/{1}. Potentially collection cache or routing map cache is outdated. Return null - upper logic will refresh and retry. ", (object) new PartitionKeyRangeIdentity(collection.ResourceId, range.Id), (object) serviceIdentity);
      return (AddressResolver.ResolutionResult) null;
    }

    private PartitionKeyRange TryResolveSinglePartitionCollection(
      DocumentServiceRequest request,
      DocumentCollection collection,
      CollectionRoutingMap routingMap,
      bool collectionCacheIsUptoDate)
    {
      if (routingMap.OrderedPartitionKeyRanges.Count == 1)
        return routingMap.OrderedPartitionKeyRanges.Single<PartitionKeyRange>();
      if (!collectionCacheIsUptoDate)
        return (PartitionKeyRange) null;
      if (collection.PartitionKey.Paths.Count >= 1 && !collection.PartitionKey.IsSystemKey.GetValueOrDefault(false))
      {
        BadRequestException requestException = new BadRequestException(RMResources.MissingPartitionKeyValue);
        requestException.ResourceAddress = request.ResourceAddress;
        throw requestException;
      }
      if (routingMap.OrderedPartitionKeyRanges.Count > 1)
        return this.TryResolveServerPartitionByPartitionKey(request, "[]", collectionCacheIsUptoDate, collection, routingMap);
      DefaultTrace.TraceCritical("No Partition Key ranges present for the collection {0}", (object) collection.ResourceId);
      InternalServerErrorException serverErrorException = new InternalServerErrorException(RMResources.InternalServerError);
      serverErrorException.ResourceAddress = request.ResourceAddress;
      throw serverErrorException;
    }

    private AddressResolver.ResolutionResult HandleRangeAddressResolutionFailure(
      DocumentServiceRequest request,
      bool collectionCacheIsUpToDate,
      bool routingMapCacheIsUpToDate,
      CollectionRoutingMap routingMap)
    {
      if (collectionCacheIsUpToDate & routingMapCacheIsUpToDate || collectionCacheIsUpToDate && routingMap.IsGone(request.PartitionKeyRangeIdentity.PartitionKeyRangeId))
      {
        PartitionKeyRangeGoneException rangeGoneException = new PartitionKeyRangeGoneException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, RMResources.PartitionKeyRangeNotFound, (object) request.PartitionKeyRangeIdentity.PartitionKeyRangeId, (object) request.PartitionKeyRangeIdentity.CollectionRid));
        rangeGoneException.ResourceAddress = request.ResourceAddress;
        throw rangeGoneException;
      }
      return (AddressResolver.ResolutionResult) null;
    }

    private async Task<AddressResolver.ResolutionResult> TryResolveServerPartitionByPartitionKeyRangeIdAsync(
      DocumentServiceRequest request,
      DocumentCollection collection,
      CollectionRoutingMap routingMap,
      bool collectionCacheIsUpToDate,
      bool routingMapCacheIsUpToDate,
      bool forceRefreshPartitionAddresses,
      CancellationToken cancellationToken)
    {
      PartitionKeyRange partitionKeyRange = routingMap.TryGetRangeByPartitionKeyRangeId(request.PartitionKeyRangeIdentity.PartitionKeyRangeId);
      if (partitionKeyRange == null)
      {
        DefaultTrace.TraceInformation("Cannot resolve range '{0}'", (object) request.PartitionKeyRangeIdentity.ToHeader());
        return this.HandleRangeAddressResolutionFailure(request, collectionCacheIsUpToDate, routingMapCacheIsUpToDate, routingMap);
      }
      ServiceIdentity identity = routingMap.TryGetInfoByPartitionKeyRangeId(request.PartitionKeyRangeIdentity.PartitionKeyRangeId);
      PartitionAddressInformation addresses = await this.addressCache.TryGetAddresses(request, new PartitionKeyRangeIdentity(collection.ResourceId, request.PartitionKeyRangeIdentity.PartitionKeyRangeId), identity, forceRefreshPartitionAddresses, cancellationToken);
      if (addresses != null)
        return new AddressResolver.ResolutionResult(partitionKeyRange, addresses, identity);
      DefaultTrace.TraceInformation("Cannot resolve addresses for range '{0}'", (object) request.PartitionKeyRangeIdentity.ToHeader());
      return this.HandleRangeAddressResolutionFailure(request, collectionCacheIsUpToDate, routingMapCacheIsUpToDate, routingMap);
    }

    private PartitionKeyRange TryResolveServerPartitionByPartitionKey(
      DocumentServiceRequest request,
      string partitionKeyString,
      bool collectionCacheUptoDate,
      DocumentCollection collection,
      CollectionRoutingMap routingMap)
    {
      if (request == null)
        throw new ArgumentNullException(nameof (request));
      if (partitionKeyString == null)
        throw new ArgumentNullException(nameof (partitionKeyString));
      if (collection == null)
        throw new ArgumentNullException(nameof (collection));
      if (routingMap == null)
        throw new ArgumentNullException(nameof (routingMap));
      PartitionKeyInternal partitionKeyInternal;
      try
      {
        partitionKeyInternal = PartitionKeyInternal.FromJsonString(partitionKeyString);
      }
      catch (JsonException ex)
      {
        BadRequestException requestException = new BadRequestException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, RMResources.InvalidPartitionKey, (object) partitionKeyString), (Exception) ex);
        requestException.ResourceAddress = request.ResourceAddress;
        throw requestException;
      }
      if (partitionKeyInternal == null)
        throw new InternalServerErrorException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "partition key is null '{0}'", (object) partitionKeyString));
      if (partitionKeyInternal.Equals(PartitionKeyInternal.Empty) || partitionKeyInternal.Components.Count == collection.PartitionKey.Paths.Count)
      {
        string partitionKeyString1 = partitionKeyInternal.GetEffectivePartitionKeyString(collection.PartitionKey);
        return routingMap.GetRangeByEffectivePartitionKey(partitionKeyString1);
      }
      if (collectionCacheUptoDate)
      {
        BadRequestException requestException = new BadRequestException(RMResources.PartitionKeyMismatch);
        requestException.ResourceAddress = request.ResourceAddress;
        requestException.Headers["x-ms-substatus"] = 1001U.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        throw requestException;
      }
      DefaultTrace.TraceInformation("Cannot compute effective partition key. Definition has '{0}' paths, values supplied has '{1}' paths. Will refresh cache and retry.", (object) collection.PartitionKey.Paths.Count, (object) partitionKeyInternal.Components.Count);
      return (PartitionKeyRange) null;
    }

    private class ResolutionResult
    {
      public PartitionKeyRange TargetPartitionKeyRange { get; private set; }

      public PartitionAddressInformation Addresses { get; private set; }

      public ServiceIdentity TargetServiceIdentity { get; private set; }

      public ResolutionResult(
        PartitionAddressInformation addresses,
        ServiceIdentity serviceIdentity)
      {
        if (addresses == null)
          throw new ArgumentNullException(nameof (addresses));
        if (serviceIdentity == null)
          throw new ArgumentNullException(nameof (serviceIdentity));
        this.Addresses = addresses;
        this.TargetServiceIdentity = serviceIdentity;
      }

      public ResolutionResult(
        PartitionKeyRange targetPartitionKeyRange,
        PartitionAddressInformation addresses,
        ServiceIdentity serviceIdentity)
      {
        if (targetPartitionKeyRange == null)
          throw new ArgumentNullException(nameof (targetPartitionKeyRange));
        if (addresses == null)
          throw new ArgumentNullException(nameof (addresses));
        this.TargetPartitionKeyRange = targetPartitionKeyRange;
        this.Addresses = addresses;
        this.TargetServiceIdentity = serviceIdentity;
      }
    }
  }
}
