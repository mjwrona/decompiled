// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Common.CollectionCache
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.Core.Trace;
using Microsoft.Azure.Cosmos.Tracing;
using Microsoft.Azure.Documents;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Cosmos.Common
{
  internal abstract class CollectionCache
  {
    protected readonly CollectionCache.InternalCache[] cacheByApiList;

    protected CollectionCache()
    {
      this.cacheByApiList = new CollectionCache.InternalCache[2];
      this.cacheByApiList[0] = new CollectionCache.InternalCache();
      this.cacheByApiList[1] = new CollectionCache.InternalCache();
    }

    public virtual Task<ContainerProperties> ResolveCollectionAsync(
      DocumentServiceRequest request,
      TimeSpan refreshAfter,
      CancellationToken cancellationToken,
      ITrace trace)
    {
      cancellationToken.ThrowIfCancellationRequested();
      CollectionCache.InternalCache cache = this.GetCache(request.Headers["x-ms-version"]);
      DateTime utcNow = DateTime.UtcNow;
      DateTime minValue = DateTime.MinValue;
      if (request.IsNameBased)
      {
        string collectionPath = PathsHelper.GetCollectionPath(request.ResourceAddress);
        if (cache.collectionInfoByNameLastRefreshTime.TryGetValue(collectionPath, out minValue) && utcNow - minValue > refreshAfter)
          cache.collectionInfoByName.TryRemoveIfCompleted(collectionPath);
      }
      else
      {
        string key = ResourceId.Parse(request.ResourceId).DocumentCollectionId.ToString();
        if (cache.collectionInfoByIdLastRefreshTime.TryGetValue(key, out minValue) && utcNow - minValue > refreshAfter)
          cache.collectionInfoById.TryRemoveIfCompleted(request.ResourceId);
      }
      return this.ResolveCollectionAsync(request, cancellationToken, trace);
    }

    public virtual async Task<ContainerProperties> ResolveCollectionAsync(
      DocumentServiceRequest request,
      CancellationToken cancellationToken,
      ITrace trace)
    {
      IClientSideRequestStatistics clientSideRequestStatistics = request.RequestContext?.ClientRequestStatistics;
      if (request.IsNameBased)
      {
        if (request.ForceNameCacheRefresh)
        {
          await this.RefreshAsync(request, trace, clientSideRequestStatistics, cancellationToken);
          request.ForceNameCacheRefresh = false;
        }
        ContainerProperties containerProperties1 = await this.ResolveByPartitionKeyRangeIdentityAsync(request.Headers["x-ms-version"], request.PartitionKeyRangeIdentity, trace, clientSideRequestStatistics, cancellationToken);
        if (containerProperties1 != null)
          return containerProperties1;
        if (request.RequestContext.ResolvedCollectionRid != null)
          return await this.ResolveByRidAsync(request.Headers["x-ms-version"], request.RequestContext.ResolvedCollectionRid, trace, clientSideRequestStatistics, cancellationToken);
        ContainerProperties containerProperties2 = await this.ResolveByNameAsync(request.Headers["x-ms-version"], request.ResourceAddress, false, trace, clientSideRequestStatistics, cancellationToken);
        if (containerProperties2 != null)
        {
          DefaultTrace.TraceVerbose("Mapped resourceName {0} to resourceId {1}. '{2}'", (object) request.ResourceAddress, (object) containerProperties2.ResourceId, (object) System.Diagnostics.Trace.CorrelationManager.ActivityId);
          request.ResourceId = containerProperties2.ResourceId;
          request.RequestContext.ResolvedCollectionRid = containerProperties2.ResourceId;
        }
        else
          DefaultTrace.TraceVerbose("Collection with resourceName {0} not found. '{1}'", (object) request.ResourceAddress, (object) System.Diagnostics.Trace.CorrelationManager.ActivityId);
        return containerProperties2;
      }
      ContainerProperties containerProperties = await this.ResolveByPartitionKeyRangeIdentityAsync(request.Headers["x-ms-version"], request.PartitionKeyRangeIdentity, trace, clientSideRequestStatistics, cancellationToken);
      if (containerProperties == null)
        containerProperties = await this.ResolveByRidAsync(request.Headers["x-ms-version"], request.ResourceAddress, trace, clientSideRequestStatistics, cancellationToken);
      return containerProperties;
    }

    public void Refresh(string resourceAddress, string apiVersion = null)
    {
      CollectionCache.InternalCache cache = this.GetCache(apiVersion);
      if (!PathsHelper.IsNameBased(resourceAddress))
        return;
      string collectionPath = PathsHelper.GetCollectionPath(resourceAddress);
      cache.collectionInfoByName.TryRemoveIfCompleted(collectionPath);
    }

    protected abstract Task<ContainerProperties> GetByRidAsync(
      string apiVersion,
      string collectionRid,
      ITrace trace,
      IClientSideRequestStatistics clientSideRequestStatistics,
      CancellationToken cancellationToken);

    protected abstract Task<ContainerProperties> GetByNameAsync(
      string apiVersion,
      string resourceAddress,
      ITrace trace,
      IClientSideRequestStatistics clientSideRequestStatistics,
      CancellationToken cancellationToken);

    private async Task<ContainerProperties> ResolveByPartitionKeyRangeIdentityAsync(
      string apiVersion,
      PartitionKeyRangeIdentity partitionKeyRangeIdentity,
      ITrace trace,
      IClientSideRequestStatistics clientSideRequestStatistics,
      CancellationToken cancellationToken)
    {
      int num;
      if (num != 0 && partitionKeyRangeIdentity?.CollectionRid == null)
        return (ContainerProperties) null;
      try
      {
        return await this.ResolveByRidAsync(apiVersion, partitionKeyRangeIdentity.CollectionRid, trace, clientSideRequestStatistics, cancellationToken);
      }
      catch (NotFoundException ex)
      {
        throw new InvalidPartitionException(RMResources.InvalidDocumentCollection);
      }
    }

    private Task<ContainerProperties> ResolveByRidAsync(
      string apiVersion,
      string resourceId,
      ITrace trace,
      IClientSideRequestStatistics clientSideRequestStatistics,
      CancellationToken cancellationToken)
    {
      cancellationToken.ThrowIfCancellationRequested();
      string collectionResourceId = ResourceId.Parse(resourceId).DocumentCollectionId.ToString();
      CollectionCache.InternalCache cache = this.GetCache(apiVersion);
      return cache.collectionInfoById.GetAsync(collectionResourceId, (ContainerProperties) null, (Func<Task<ContainerProperties>>) (async () =>
      {
        DateTime currentTime = DateTime.UtcNow;
        ContainerProperties byRidAsync = await this.GetByRidAsync(apiVersion, collectionResourceId, trace, clientSideRequestStatistics, cancellationToken);
        cache.collectionInfoByIdLastRefreshTime.AddOrUpdate(collectionResourceId, currentTime, (Func<string, DateTime, DateTime>) ((currentKey, currentValue) => currentTime));
        return byRidAsync;
      }), cancellationToken);
    }

    internal virtual async Task<ContainerProperties> ResolveByNameAsync(
      string apiVersion,
      string resourceAddress,
      bool forceRefesh,
      ITrace trace,
      IClientSideRequestStatistics clientSideRequestStatistics,
      CancellationToken cancellationToken)
    {
      cancellationToken.ThrowIfCancellationRequested();
      string resourceFullName = PathsHelper.GetCollectionPath(resourceAddress);
      CollectionCache.InternalCache cache = this.GetCache(apiVersion);
      if (forceRefesh)
        cache.collectionInfoByName.TryRemoveIfCompleted(resourceFullName);
      return await cache.collectionInfoByName.GetAsync(resourceFullName, (ContainerProperties) null, (Func<Task<ContainerProperties>>) (async () =>
      {
        DateTime currentTime = DateTime.UtcNow;
        ContainerProperties byNameAsync = await this.GetByNameAsync(apiVersion, resourceFullName, trace, clientSideRequestStatistics, cancellationToken);
        cache.collectionInfoById.Set(byNameAsync.ResourceId, byNameAsync);
        cache.collectionInfoByNameLastRefreshTime.AddOrUpdate(resourceFullName, currentTime, (Func<string, DateTime, DateTime>) ((currentKey, currentValue) => currentTime));
        cache.collectionInfoByIdLastRefreshTime.AddOrUpdate(byNameAsync.ResourceId, currentTime, (Func<string, DateTime, DateTime>) ((currentKey, currentValue) => currentTime));
        return byNameAsync;
      }), cancellationToken);
    }

    private async Task RefreshAsync(
      DocumentServiceRequest request,
      ITrace trace,
      IClientSideRequestStatistics clientSideRequestStatistics,
      CancellationToken cancellationToken)
    {
      CollectionCache.InternalCache cache = this.GetCache(request.Headers["x-ms-version"]);
      string resourceFullName = PathsHelper.GetCollectionPath(request.ResourceAddress);
      if (request.RequestContext.ResolvedCollectionRid != null)
      {
        ContainerProperties async = await cache.collectionInfoByName.GetAsync(resourceFullName, ContainerProperties.CreateWithResourceId(request.RequestContext.ResolvedCollectionRid), (Func<Task<ContainerProperties>>) (async () =>
        {
          DateTime currentTime = DateTime.UtcNow;
          ContainerProperties byNameAsync = await this.GetByNameAsync(request.Headers["x-ms-version"], resourceFullName, trace, clientSideRequestStatistics, cancellationToken);
          cache.collectionInfoById.Set(byNameAsync.ResourceId, byNameAsync);
          cache.collectionInfoByNameLastRefreshTime.AddOrUpdate(resourceFullName, currentTime, (Func<string, DateTime, DateTime>) ((currentKey, currentValue) => currentTime));
          cache.collectionInfoByIdLastRefreshTime.AddOrUpdate(byNameAsync.ResourceId, currentTime, (Func<string, DateTime, DateTime>) ((currentKey, currentValue) => currentTime));
          return byNameAsync;
        }), cancellationToken);
      }
      else
        this.Refresh(request.ResourceAddress, request.Headers["x-ms-version"]);
      request.RequestContext.ResolvedCollectionRid = (string) null;
    }

    protected CollectionCache.InternalCache GetCache(string apiVersion) => string.IsNullOrEmpty(apiVersion) || VersionUtility.IsLaterThan(apiVersion, HttpConstants.VersionDates.v2018_12_31) ? this.cacheByApiList[1] : this.cacheByApiList[0];

    protected class InternalCache
    {
      internal readonly AsyncCache<string, ContainerProperties> collectionInfoByName;
      internal readonly AsyncCache<string, ContainerProperties> collectionInfoById;
      internal readonly ConcurrentDictionary<string, DateTime> collectionInfoByNameLastRefreshTime;
      internal readonly ConcurrentDictionary<string, DateTime> collectionInfoByIdLastRefreshTime;

      internal InternalCache()
      {
        this.collectionInfoByName = new AsyncCache<string, ContainerProperties>((IEqualityComparer<ContainerProperties>) new CollectionCache.CollectionRidComparer());
        this.collectionInfoById = new AsyncCache<string, ContainerProperties>((IEqualityComparer<ContainerProperties>) new CollectionCache.CollectionRidComparer());
        this.collectionInfoByNameLastRefreshTime = new ConcurrentDictionary<string, DateTime>();
        this.collectionInfoByIdLastRefreshTime = new ConcurrentDictionary<string, DateTime>();
      }
    }

    private sealed class CollectionRidComparer : IEqualityComparer<ContainerProperties>
    {
      public bool Equals(ContainerProperties left, ContainerProperties right)
      {
        if (left == null && right == null)
          return true;
        return !(left == null ^ right == null) && StringComparer.Ordinal.Compare(left.ResourceId, right.ResourceId) == 0;
      }

      public int GetHashCode(ContainerProperties collection) => collection.ResourceId.GetHashCode();
    }
  }
}
