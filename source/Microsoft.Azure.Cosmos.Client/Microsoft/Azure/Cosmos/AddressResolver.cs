// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.AddressResolver
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.Common;
using Microsoft.Azure.Cosmos.Core.Trace;
using Microsoft.Azure.Cosmos.Routing;
using Microsoft.Azure.Cosmos.Tracing;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Rntbd;
using Microsoft.Azure.Documents.Routing;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Cosmos
{
  internal sealed class AddressResolver : IAddressResolver
  {
    private readonly IMasterServiceIdentityProvider masterServiceIdentityProvider;
    private readonly IRequestSigner requestSigner;
    private readonly string location;
    private readonly PartitionKeyRangeIdentity masterPartitionKeyRangeIdentity = new PartitionKeyRangeIdentity("M");
    private CollectionCache collectionCache;
    private ICollectionRoutingMapCache collectionRoutingMapCache;
    private IAddressCache addressCache;

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
      request.RequestContext.LocalRegionRequest = result.Addresses.IsLocalRegion;
      await this.requestSigner.SignRequestAsync(request, cancellationToken);
      PartitionAddressInformation addresses = result.Addresses;
      result = (AddressResolver.ResolutionResult) null;
      return addresses;
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
        PartitionAddressInformation addressesAsync = await this.addressCache.TryGetAddressesAsync(request, (PartitionKeyRangeIdentity) null, identity, forceRefreshPartitionAddresses, cancellationToken);
        if (addressesAsync == null && identity.IsMasterService && this.masterServiceIdentityProvider != null)
        {
          DefaultTrace.TraceWarning("Could not get addresses for MasterServiceIdentity {0}. will refresh masterServiceIdentity and retry", (object) identity);
          await this.masterServiceIdentityProvider.RefreshAsync(identity, cancellationToken);
          identity = this.masterServiceIdentityProvider.MasterServiceIdentity;
          addressesAsync = await this.addressCache.TryGetAddressesAsync(request, (PartitionKeyRangeIdentity) null, identity, forceRefreshPartitionAddresses, cancellationToken);
        }
        if (addressesAsync == null)
        {
          DefaultTrace.TraceInformation("Could not get addresses for explicitly specified ServiceIdentity {0}", (object) identity);
          NotFoundException notFoundException = new NotFoundException();
          notFoundException.ResourceAddress = request.ResourceAddress;
          throw notFoundException;
        }
        return new AddressResolver.ResolutionResult(addressesAsync, identity);
      }
      if (ReplicatedResourceClient.IsReadingFromMaster(request.ResourceType, request.OperationType) && request.PartitionKeyRangeIdentity == null)
      {
        DefaultTrace.TraceInformation("Resolving Master service address, forceMasterRefresh: {0}, currentMaster: {1}", (object) request.ForceMasterRefresh, (object) this.masterServiceIdentityProvider?.MasterServiceIdentity);
        if (request.ForceMasterRefresh && this.masterServiceIdentityProvider != null)
          await this.masterServiceIdentityProvider.RefreshAsync(this.masterServiceIdentityProvider.MasterServiceIdentity, cancellationToken);
        identity = this.masterServiceIdentityProvider?.MasterServiceIdentity;
        PartitionKeyRangeIdentity keyRangeIdentity = this.masterPartitionKeyRangeIdentity;
        PartitionAddressInformation addressesAsync = await this.addressCache.TryGetAddressesAsync(request, keyRangeIdentity, identity, forceRefreshPartitionAddresses, cancellationToken);
        if (addressesAsync == null)
        {
          DefaultTrace.TraceCritical("Could not get addresses for master partition {0}", (object) identity);
          NotFoundException notFoundException = new NotFoundException();
          notFoundException.ResourceAddress = request.ResourceAddress;
          throw notFoundException;
        }
        PartitionKeyRange targetPartitionKeyRange = new PartitionKeyRange();
        targetPartitionKeyRange.Id = "M";
        return new AddressResolver.ResolutionResult(targetPartitionKeyRange, addressesAsync, identity);
      }
      bool collectionCacheIsUptoDate = !request.IsNameBased || request.PartitionKeyRangeIdentity != null && request.PartitionKeyRangeIdentity.CollectionRid != null;
      bool collectionRoutingMapCacheIsUptoDate = false;
      ContainerProperties collection = await this.collectionCache.ResolveCollectionAsync(request, cancellationToken, (ITrace) NoOpTrace.Singleton);
      CollectionRoutingMap routingMap = await this.collectionRoutingMapCache.TryLookupAsync(collection.ResourceId, (CollectionRoutingMap) null, request, (ITrace) NoOpTrace.Singleton);
      if (routingMap != null && request.ForceCollectionRoutingMapRefresh)
      {
        DefaultTrace.TraceInformation("AddressResolver.ResolveAddressesAndIdentityAsync ForceCollectionRoutingMapRefresh collection.ResourceId = {0}", (object) collection.ResourceId);
        routingMap = await this.collectionRoutingMapCache.TryLookupAsync(collection.ResourceId, routingMap, request, (ITrace) NoOpTrace.Singleton);
      }
      if (request.ForcePartitionKeyRangeRefresh)
      {
        collectionRoutingMapCacheIsUptoDate = true;
        request.ForcePartitionKeyRangeRefresh = false;
        if (routingMap != null)
          routingMap = await this.collectionRoutingMapCache.TryLookupAsync(collection.ResourceId, routingMap, request, (ITrace) NoOpTrace.Singleton);
      }
      if (routingMap == null && !collectionCacheIsUptoDate)
      {
        request.ForceNameCacheRefresh = true;
        collectionCacheIsUptoDate = true;
        collectionRoutingMapCacheIsUptoDate = false;
        collection = await this.collectionCache.ResolveCollectionAsync(request, cancellationToken, (ITrace) NoOpTrace.Singleton);
        routingMap = await this.collectionRoutingMapCache.TryLookupAsync(collection.ResourceId, (CollectionRoutingMap) null, request, (ITrace) NoOpTrace.Singleton);
      }
      AddressResolver.EnsureRoutingMapPresent(request, routingMap, collection);
      AddressResolver.ResolutionResult resolutionResult = await this.TryResolveServerPartitionAsync(request, collection, routingMap, collectionCacheIsUptoDate, collectionRoutingMapCacheIsUptoDate, forceRefreshPartitionAddresses, cancellationToken);
      if (resolutionResult == null)
      {
        if (!collectionCacheIsUptoDate)
        {
          request.ForceNameCacheRefresh = true;
          collection = await this.collectionCache.ResolveCollectionAsync(request, cancellationToken, (ITrace) NoOpTrace.Singleton);
          if (collection.ResourceId != routingMap.CollectionUniqueId)
          {
            collectionRoutingMapCacheIsUptoDate = false;
            routingMap = await this.collectionRoutingMapCache.TryLookupAsync(collection.ResourceId, (CollectionRoutingMap) null, request, (ITrace) NoOpTrace.Singleton);
          }
        }
        if (!collectionRoutingMapCacheIsUptoDate)
          routingMap = await this.collectionRoutingMapCache.TryLookupAsync(collection.ResourceId, routingMap, request, (ITrace) NoOpTrace.Singleton);
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
      ContainerProperties collection)
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
      ContainerProperties collection,
      CollectionRoutingMap routingMap,
      bool collectionCacheIsUptodate,
      bool collectionRoutingMapCacheIsUptodate,
      bool forceRefreshPartitionAddresses,
      CancellationToken cancellationToken)
    {
      if (request.PartitionKeyRangeIdentity != null)
        return await this.TryResolveServerPartitionByPartitionKeyRangeIdAsync(request, collection, routingMap, collectionCacheIsUptodate, collectionRoutingMapCacheIsUptodate, forceRefreshPartitionAddresses, cancellationToken);
      if (!request.ResourceType.IsPartitioned() && (request.ResourceType != ResourceType.StoredProcedure || request.OperationType != Microsoft.Azure.Documents.OperationType.ExecuteJavaScript) && (request.ResourceType != ResourceType.Collection || request.OperationType != Microsoft.Azure.Documents.OperationType.Head))
      {
        DefaultTrace.TraceCritical("Shouldn't come here for non partitioned resources. resourceType : {0}, operationtype:{1}, resourceaddress:{2}", (object) request.ResourceType, (object) request.OperationType, (object) request.ResourceAddress);
        InternalServerErrorException serverErrorException = new InternalServerErrorException(RMResources.InternalServerError);
        serverErrorException.ResourceAddress = request.ResourceAddress;
        throw serverErrorException;
      }
      string header = request.Headers["x-ms-documentdb-partitionkey"];
      PartitionKeyRange range;
      if (header != null)
      {
        range = AddressResolver.TryResolveServerPartitionByPartitionKey(request, header, collectionCacheIsUptodate, collection, routingMap);
      }
      else
      {
        object obj;
        if (request.Properties != null && request.Properties.TryGetValue("x-ms-effective-partition-key-string", out obj))
        {
          if (!collection.HasPartitionKey || collection.PartitionKey.IsSystemKey.GetValueOrDefault(false))
            throw new ArgumentOutOfRangeException(nameof (collection));
          string effectivePartitionKeyValue = obj as string;
          range = !string.IsNullOrEmpty(effectivePartitionKeyValue) ? routingMap.GetRangeByEffectivePartitionKey(effectivePartitionKeyValue) : throw new ArgumentOutOfRangeException("effectivePartitionKeyString");
        }
        else
          range = this.TryResolveSinglePartitionCollection(request, collection, routingMap, collectionCacheIsUptodate);
      }
      if (range == null)
        return (AddressResolver.ResolutionResult) null;
      ServiceIdentity serviceIdentity = routingMap.TryGetInfoByPartitionKeyRangeId(range.Id);
      PartitionAddressInformation addressesAsync = await this.addressCache.TryGetAddressesAsync(request, new PartitionKeyRangeIdentity(collection.ResourceId, range.Id), serviceIdentity, forceRefreshPartitionAddresses, cancellationToken);
      if (addressesAsync != null)
        return new AddressResolver.ResolutionResult(range, addressesAsync, serviceIdentity);
      DefaultTrace.TraceVerbose("Could not resolve addresses for identity {0}/{1}. Potentially collection cache or routing map cache is outdated. Return null - upper logic will refresh and retry. ", (object) new PartitionKeyRangeIdentity(collection.ResourceId, range.Id), (object) serviceIdentity);
      return (AddressResolver.ResolutionResult) null;
    }

    private PartitionKeyRange TryResolveSinglePartitionCollection(
      DocumentServiceRequest request,
      ContainerProperties collection,
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
        return AddressResolver.TryResolveServerPartitionByPartitionKey(request, "[]", collectionCacheIsUptoDate, collection, routingMap);
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
      ContainerProperties collection,
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
      PartitionAddressInformation addressesAsync = await this.addressCache.TryGetAddressesAsync(request, new PartitionKeyRangeIdentity(collection.ResourceId, request.PartitionKeyRangeIdentity.PartitionKeyRangeId), identity, forceRefreshPartitionAddresses, cancellationToken);
      if (addressesAsync != null)
        return new AddressResolver.ResolutionResult(partitionKeyRange, addressesAsync, identity);
      DefaultTrace.TraceInformation("Cannot resolve addresses for range '{0}'", (object) request.PartitionKeyRangeIdentity.ToHeader());
      return this.HandleRangeAddressResolutionFailure(request, collectionCacheIsUpToDate, routingMapCacheIsUpToDate, routingMap);
    }

    internal static PartitionKeyRange TryResolveServerPartitionByPartitionKey(
      DocumentServiceRequest request,
      string partitionKeyString,
      bool collectionCacheUptoDate,
      ContainerProperties collection,
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

    public Task UpdateAsync(ServerKey serverKey, CancellationToken cancellationToken = default (CancellationToken)) => throw new NotImplementedException();

    public Task UpdateAsync(
      IReadOnlyList<AddressCacheToken> addressCacheTokens,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      throw new NotImplementedException();
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
