// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.Common.CollectionCache
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using Microsoft.Azure.Cosmos.Core.Trace;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Documents.Common
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

    public virtual Task<DocumentCollection> ResolveCollectionAsync(
      DocumentServiceRequest request,
      TimeSpan refreshAfter,
      CancellationToken cancellationToken)
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
      return this.ResolveCollectionAsync(request, cancellationToken);
    }

    public virtual async Task<DocumentCollection> ResolveCollectionAsync(
      DocumentServiceRequest request,
      CancellationToken cancellationToken)
    {
      if (request.IsNameBased)
      {
        if (request.ForceNameCacheRefresh)
        {
          await this.RefreshAsync(request, cancellationToken);
          request.ForceNameCacheRefresh = false;
        }
        DocumentCollection documentCollection1 = await this.ResolveByPartitionKeyRangeIdentityAsync(request.Headers["x-ms-version"], request.PartitionKeyRangeIdentity, cancellationToken);
        if (documentCollection1 != null)
          return documentCollection1;
        if (request.RequestContext.ResolvedCollectionRid != null)
          return await this.ResolveByRidAsync(request.Headers["x-ms-version"], request.RequestContext.ResolvedCollectionRid, cancellationToken);
        DocumentCollection documentCollection2 = await this.ResolveByNameAsync(request.Headers["x-ms-version"], request.ResourceAddress, cancellationToken);
        if (documentCollection2 != null)
        {
          DefaultTrace.TraceVerbose("Mapped resourceName {0} to resourceId {1}. '{2}'", (object) request.ResourceAddress, (object) documentCollection2.ResourceId, (object) Microsoft.Azure.Documents.Trace.CorrelationManager.ActivityId);
          request.ResourceId = documentCollection2.ResourceId;
          request.RequestContext.ResolvedCollectionRid = documentCollection2.ResourceId;
        }
        else
          DefaultTrace.TraceVerbose("Collection with resourceName {0} not found. '{1}'", (object) request.ResourceAddress, (object) Microsoft.Azure.Documents.Trace.CorrelationManager.ActivityId);
        return documentCollection2;
      }
      DocumentCollection documentCollection = await this.ResolveByPartitionKeyRangeIdentityAsync(request.Headers["x-ms-version"], request.PartitionKeyRangeIdentity, cancellationToken);
      if (documentCollection == null)
        documentCollection = await this.ResolveByRidAsync(request.Headers["x-ms-version"], request.ResourceAddress, cancellationToken);
      return documentCollection;
    }

    public void Refresh(string resourceAddress, string apiVersion = null)
    {
      CollectionCache.InternalCache cache = this.GetCache(apiVersion);
      if (!PathsHelper.IsNameBased(resourceAddress))
        return;
      string collectionPath = PathsHelper.GetCollectionPath(resourceAddress);
      cache.collectionInfoByName.TryRemoveIfCompleted(collectionPath);
    }

    protected abstract Task<DocumentCollection> GetByRidAsync(
      string apiVersion,
      string collectionRid,
      CancellationToken cancellationToken);

    protected abstract Task<DocumentCollection> GetByNameAsync(
      string apiVersion,
      string resourceAddress,
      CancellationToken cancellationToken);

    private async Task<DocumentCollection> ResolveByPartitionKeyRangeIdentityAsync(
      string apiVersion,
      PartitionKeyRangeIdentity partitionKeyRangeIdentity,
      CancellationToken cancellationToken)
    {
      int num;
      if (num != 0 && (partitionKeyRangeIdentity == null || partitionKeyRangeIdentity.CollectionRid == null))
        return (DocumentCollection) null;
      try
      {
        return await this.ResolveByRidAsync(apiVersion, partitionKeyRangeIdentity.CollectionRid, cancellationToken);
      }
      catch (NotFoundException ex)
      {
        throw new InvalidPartitionException(RMResources.InvalidDocumentCollection);
      }
    }

    private Task<DocumentCollection> ResolveByRidAsync(
      string apiVersion,
      string resourceId,
      CancellationToken cancellationToken)
    {
      cancellationToken.ThrowIfCancellationRequested();
      string collectionResourceId = ResourceId.Parse(resourceId).DocumentCollectionId.ToString();
      CollectionCache.InternalCache cache = this.GetCache(apiVersion);
      return cache.collectionInfoById.GetAsync(collectionResourceId, (DocumentCollection) null, (Func<Task<DocumentCollection>>) (async () =>
      {
        DateTime currentTime = DateTime.UtcNow;
        DocumentCollection byRidAsync = await this.GetByRidAsync(apiVersion, collectionResourceId, cancellationToken);
        cache.collectionInfoByIdLastRefreshTime.AddOrUpdate(collectionResourceId, currentTime, (Func<string, DateTime, DateTime>) ((currentKey, currentValue) => currentTime));
        return byRidAsync;
      }), cancellationToken);
    }

    private async Task<DocumentCollection> ResolveByNameAsync(
      string apiVersion,
      string resourceAddress,
      CancellationToken cancellationToken)
    {
      cancellationToken.ThrowIfCancellationRequested();
      string resourceFullName = PathsHelper.GetCollectionPath(resourceAddress);
      CollectionCache.InternalCache cache = this.GetCache(apiVersion);
      return await cache.collectionInfoByName.GetAsync(resourceFullName, (DocumentCollection) null, (Func<Task<DocumentCollection>>) (async () =>
      {
        DateTime currentTime = DateTime.UtcNow;
        DocumentCollection byNameAsync = await this.GetByNameAsync(apiVersion, resourceFullName, cancellationToken);
        cache.collectionInfoById.Set(byNameAsync.ResourceId, byNameAsync);
        cache.collectionInfoByNameLastRefreshTime.AddOrUpdate(resourceFullName, currentTime, (Func<string, DateTime, DateTime>) ((currentKey, currentValue) => currentTime));
        cache.collectionInfoByIdLastRefreshTime.AddOrUpdate(byNameAsync.ResourceId, currentTime, (Func<string, DateTime, DateTime>) ((currentKey, currentValue) => currentTime));
        return byNameAsync;
      }), cancellationToken);
    }

    private async Task RefreshAsync(
      DocumentServiceRequest request,
      CancellationToken cancellationToken)
    {
      CollectionCache.InternalCache cache = this.GetCache(request.Headers["x-ms-version"]);
      string resourceFullName = PathsHelper.GetCollectionPath(request.ResourceAddress);
      if (request.RequestContext.ResolvedCollectionRid != null)
      {
        AsyncCache<string, DocumentCollection> collectionInfoByName = cache.collectionInfoByName;
        string key = resourceFullName;
        DocumentCollection obsoleteValue = new DocumentCollection();
        obsoleteValue.ResourceId = request.RequestContext.ResolvedCollectionRid;
        Func<Task<DocumentCollection>> singleValueInitFunc = (Func<Task<DocumentCollection>>) (async () =>
        {
          DateTime currentTime = DateTime.UtcNow;
          DocumentCollection byNameAsync = await this.GetByNameAsync(request.Headers["x-ms-version"], resourceFullName, cancellationToken);
          cache.collectionInfoById.Set(byNameAsync.ResourceId, byNameAsync);
          cache.collectionInfoByNameLastRefreshTime.AddOrUpdate(resourceFullName, currentTime, (Func<string, DateTime, DateTime>) ((currentKey, currentValue) => currentTime));
          cache.collectionInfoByIdLastRefreshTime.AddOrUpdate(byNameAsync.ResourceId, currentTime, (Func<string, DateTime, DateTime>) ((currentKey, currentValue) => currentTime));
          return byNameAsync;
        });
        CancellationToken cancellationToken1 = cancellationToken;
        DocumentCollection async = await collectionInfoByName.GetAsync(key, obsoleteValue, singleValueInitFunc, cancellationToken1);
      }
      else
        this.Refresh(request.ResourceAddress, request.Headers["x-ms-version"]);
      request.RequestContext.ResolvedCollectionRid = (string) null;
    }

    protected CollectionCache.InternalCache GetCache(string apiVersion) => !string.IsNullOrEmpty(apiVersion) && VersionUtility.IsLaterThan(apiVersion, HttpConstants.Versions.v2018_12_31) ? this.cacheByApiList[1] : this.cacheByApiList[0];

    protected class InternalCache
    {
      internal readonly AsyncCache<string, DocumentCollection> collectionInfoByName;
      internal readonly AsyncCache<string, DocumentCollection> collectionInfoById;
      internal readonly ConcurrentDictionary<string, DateTime> collectionInfoByNameLastRefreshTime;
      internal readonly ConcurrentDictionary<string, DateTime> collectionInfoByIdLastRefreshTime;

      internal InternalCache()
      {
        this.collectionInfoByName = new AsyncCache<string, DocumentCollection>((IEqualityComparer<DocumentCollection>) new CollectionCache.CollectionRidComparer());
        this.collectionInfoById = new AsyncCache<string, DocumentCollection>((IEqualityComparer<DocumentCollection>) new CollectionCache.CollectionRidComparer());
        this.collectionInfoByNameLastRefreshTime = new ConcurrentDictionary<string, DateTime>();
        this.collectionInfoByIdLastRefreshTime = new ConcurrentDictionary<string, DateTime>();
      }
    }

    private sealed class CollectionRidComparer : IEqualityComparer<DocumentCollection>
    {
      public bool Equals(DocumentCollection left, DocumentCollection right)
      {
        if (left == null && right == null)
          return true;
        return !(left == null ^ right == null) && StringComparer.Ordinal.Compare(left.ResourceId, right.ResourceId) == 0;
      }

      public int GetHashCode(DocumentCollection collection) => collection.ResourceId.GetHashCode();
    }
  }
}
