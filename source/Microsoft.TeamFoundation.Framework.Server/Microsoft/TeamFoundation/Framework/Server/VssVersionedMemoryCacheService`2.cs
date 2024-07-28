// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.VssVersionedMemoryCacheService`2
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public abstract class VssVersionedMemoryCacheService<TKey, TValue> : 
    VssMemoryCacheService<TKey, TValue>
  {
    private ILockName m_versionLock;
    private volatile int m_cacheVersion = -1;

    protected VssVersionedMemoryCacheService()
    {
    }

    protected VssVersionedMemoryCacheService(IEqualityComparer<TKey> comparer)
      : base(comparer)
    {
    }

    protected VssVersionedMemoryCacheService(
      IEqualityComparer<TKey> comparer,
      MemoryCacheConfiguration<TKey, TValue> configuration)
      : base(comparer, configuration)
    {
    }

    internal override void InternalInitialize(IVssRequestContext requestContext)
    {
      base.InternalInitialize(requestContext);
      this.m_versionLock = this.CreateLockName(requestContext, "VersionLock");
    }

    internal int Version => this.m_cacheVersion;

    protected internal IVssVersionedCacheContext<TKey, TValue> CreateVersionedContext(
      IVssRequestContext requestContext,
      IComparer<TValue> valueComparer)
    {
      ArgumentUtility.CheckForNull<IComparer<TValue>>(valueComparer, nameof (valueComparer));
      return (IVssVersionedCacheContext<TKey, TValue>) new VssVersionedMemoryCacheService<TKey, TValue>.OrderedCacheContext(this, valueComparer);
    }

    protected internal IVssVersionedCacheContext<TKey, TValue> CreateVersionedContext(
      IVssRequestContext requestContext,
      IEqualityComparer<TValue> valueComparer = null)
    {
      return (IVssVersionedCacheContext<TKey, TValue>) new VssVersionedMemoryCacheService<TKey, TValue>.UnorderedCacheContext(this, valueComparer ?? (IEqualityComparer<TValue>) EqualityComparer<TValue>.Default);
    }

    protected internal IVssVersionedCacheContext<TKey, TValue> CreateVersionedContext<TVersion>(
      IVssRequestContext requestContext,
      Func<TKey, TVersion> extractVersionDelegate,
      TKey key,
      IEqualityComparer<TValue> valueComparer = null)
    {
      return (IVssVersionedCacheContext<TKey, TValue>) new VssVersionedMemoryCacheService<TKey, TValue>.UnorderedCacheContextItemBased<TVersion>(this, key, extractVersionDelegate, valueComparer ?? (IEqualityComparer<TValue>) EqualityComparer<TValue>.Default);
    }

    private abstract class UnorderedCacheContextBase : 
      IVssVersionedCacheContext<TKey, TValue>,
      IDisposable
    {
      private const string c_area = "MemoryCache";
      private const string c_layer = "UnorderedCacheContextBase";
      protected readonly VssVersionedMemoryCacheService<TKey, TValue> m_cacheService;
      private readonly IEqualityComparer<TValue> m_valueComparer;

      public UnorderedCacheContextBase(
        VssVersionedMemoryCacheService<TKey, TValue> cacheService,
        IEqualityComparer<TValue> valueComparer)
      {
        this.m_cacheService = cacheService;
        this.m_valueComparer = valueComparer;
      }

      public CacheUpdateResult TryUpdate(IVssRequestContext requestContext, TKey key, TValue value)
      {
        CacheUpdateResult cacheUpdateResult;
        using (requestContext.AcquireWriterLock(this.m_cacheService.m_versionLock))
        {
          VssMemoryCacheList<TKey, TValue> memoryCache = this.m_cacheService.MemoryCache;
          bool flag1 = this.IsSameVersion();
          TValue y;
          bool flag2 = memoryCache.TryGetValue(key, out y);
          bool flag3 = flag2 && this.m_valueComparer.Equals(value, y);
          if (flag1 | flag3)
          {
            memoryCache.Add(key, value, true);
            cacheUpdateResult = CacheUpdateResult.Success;
          }
          else
          {
            if (!flag1 && !flag2)
            {
              cacheUpdateResult = CacheUpdateResult.FailCacheVersionChanged;
              requestContext.Trace(10007301, TraceLevel.Info, "MemoryCache", nameof (UnorderedCacheContextBase<>), "Key {0} not cached because cache version was different and also value not cached before", (object) key);
            }
            else if (!flag3)
            {
              cacheUpdateResult = CacheUpdateResult.FailCachedObjectNotEqual;
              requestContext.Trace(10007301, TraceLevel.Info, "MemoryCache", nameof (UnorderedCacheContextBase<>), "Key {0} not cached because cache version was different and also cached value not equal", (object) key);
            }
            else
            {
              cacheUpdateResult = CacheUpdateResult.FailUnknown;
              requestContext.Trace(10007301, TraceLevel.Info, "MemoryCache", nameof (UnorderedCacheContextBase<>), "Key {0} not cached for unknown reasons", (object) key);
            }
            memoryCache.Remove(key);
          }
          ++this.m_cacheService.m_cacheVersion;
        }
        return cacheUpdateResult;
      }

      protected abstract bool IsSameVersion();

      public void Invalidate(IVssRequestContext requestContext, TKey key)
      {
        using (requestContext.AcquireWriterLock(this.m_cacheService.m_versionLock))
        {
          this.m_cacheService.MemoryCache.Remove(key);
          ++this.m_cacheService.m_cacheVersion;
        }
      }

      public void Dispose()
      {
      }
    }

    private class UnorderedCacheContext : 
      VssVersionedMemoryCacheService<TKey, TValue>.UnorderedCacheContextBase
    {
      private readonly int m_cacheVersion;

      public UnorderedCacheContext(
        VssVersionedMemoryCacheService<TKey, TValue> cacheService,
        IEqualityComparer<TValue> valueComparer)
        : base(cacheService, valueComparer)
      {
        this.m_cacheVersion = cacheService.m_cacheVersion;
      }

      protected override bool IsSameVersion() => this.m_cacheVersion == this.m_cacheService.m_cacheVersion;
    }

    private class UnorderedCacheContextItemBased<TVersion> : 
      VssVersionedMemoryCacheService<TKey, TValue>.UnorderedCacheContextBase
    {
      private readonly TKey m_itemKey;
      private readonly Func<TKey, TVersion> m_extractVersionDelegate;
      private readonly TVersion m_startVersion;

      public UnorderedCacheContextItemBased(
        VssVersionedMemoryCacheService<TKey, TValue> cacheService,
        TKey key,
        Func<TKey, TVersion> extractVersionDelegate,
        IEqualityComparer<TValue> valueComparer)
        : base(cacheService, valueComparer)
      {
        this.m_itemKey = key;
        this.m_extractVersionDelegate = extractVersionDelegate;
        this.m_startVersion = extractVersionDelegate(key);
      }

      protected override bool IsSameVersion() => this.m_extractVersionDelegate(this.m_itemKey).Equals((object) this.m_startVersion);
    }

    private class OrderedCacheContext : IVssVersionedCacheContext<TKey, TValue>, IDisposable
    {
      private const string c_area = "MemoryCache";
      private const string c_layer = "OrderedCacheContext";
      private readonly VssVersionedMemoryCacheService<TKey, TValue> m_cacheService;
      private readonly IComparer<TValue> m_valueComparer;

      public OrderedCacheContext(
        VssVersionedMemoryCacheService<TKey, TValue> cacheService,
        IComparer<TValue> valueComparer)
      {
        this.m_cacheService = cacheService;
        this.m_valueComparer = valueComparer;
      }

      public CacheUpdateResult TryUpdate(IVssRequestContext requestContext, TKey key, TValue value)
      {
        CacheUpdateResult cacheUpdateResult;
        using (requestContext.AcquireWriterLock(this.m_cacheService.m_versionLock))
        {
          VssMemoryCacheList<TKey, TValue> memoryCache = this.m_cacheService.MemoryCache;
          TValue x;
          if (memoryCache.TryGetValue(key, out x) && this.m_valueComparer.Compare(x, value) >= 0)
          {
            requestContext.Trace(10007300, TraceLevel.Info, "MemoryCache", nameof (OrderedCacheContext<>), "Not updating the cache for key {0}", (object) key);
            cacheUpdateResult = CacheUpdateResult.FailCachedObjectNewer;
          }
          else
          {
            memoryCache.Add(key, value, true);
            cacheUpdateResult = CacheUpdateResult.Success;
          }
          ++this.m_cacheService.m_cacheVersion;
        }
        return cacheUpdateResult;
      }

      public void Invalidate(IVssRequestContext requestContext, TKey key) => throw new InvalidOperationException("Tombstone records should be used instead of deleting the keys");

      public void Dispose()
      {
      }
    }
  }
}
