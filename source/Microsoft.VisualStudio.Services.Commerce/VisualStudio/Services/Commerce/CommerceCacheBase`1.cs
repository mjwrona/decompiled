// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.CommerceCacheBase`1
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Diagnostics;

namespace Microsoft.VisualStudio.Services.Commerce
{
  internal abstract class CommerceCacheBase<T> where T : class
  {
    private ICommerceDistributedCacheProvider distributedCache;
    private ICommerceMemoryCache<T> memoryCache;
    private string memoryCacheFeatureFlag;
    private Guid memoryCacheSqlNotificationEvent;
    protected string Area = "Commerce";

    protected void SetupCaches(
      Guid distributedNamespace,
      string distributedFeatureFlag,
      ICommerceMemoryCache<T> memoryCacheService,
      string memoryFeatureFlag,
      TimeSpan? distributedCacheExpiration)
    {
      this.distributedCache = (ICommerceDistributedCacheProvider) new CommerceRedisCacheProvider(distributedNamespace, distributedFeatureFlag, distributedCacheExpiration);
      this.memoryCache = memoryCacheService;
      this.memoryCacheFeatureFlag = memoryFeatureFlag;
    }

    protected bool TryGetCachedItem(
      IVssRequestContext requestContext,
      string key,
      out T storedValue)
    {
      storedValue = default (T);
      requestContext.Trace(5108474, TraceLevel.Info, this.Area, this.Layer, "Check L1 for the Key " + key);
      if (this.memoryCache.IsEnabled(requestContext, this.memoryCacheFeatureFlag) && this.memoryCache.TryGetValue(requestContext, key, out storedValue))
        return true;
      requestContext.Trace(5108475, TraceLevel.Info, this.Area, this.Layer, "Check L2 for the Key " + key);
      if (this.distributedCache.TryGet<T>(requestContext, key, out storedValue))
      {
        if (this.memoryCache.IsEnabled(requestContext, this.memoryCacheFeatureFlag))
          this.memoryCache.Set(requestContext, key, storedValue);
        return true;
      }
      requestContext.Trace(5108476, TraceLevel.Info, this.Area, this.Layer, key + " not found in cache");
      return false;
    }

    protected void SetCacheItem(IVssRequestContext requestContext, string key, T valueToCache)
    {
      if (this.memoryCache.IsEnabled(requestContext, this.memoryCacheFeatureFlag))
        this.memoryCache.Set(requestContext, key, valueToCache);
      requestContext.TraceProperties<T>(5108495, this.Area, this.Layer, valueToCache, "Object type: " + typeof (T).Name + " Key: " + key);
      this.distributedCache.TrySet<T>(requestContext, key, valueToCache);
    }

    protected void InvalidateCache(IVssRequestContext requestContext, string key)
    {
      this.distributedCache.Invalidate<T>(requestContext, key);
      this.InvalidateMemoryCache(requestContext, key);
    }

    protected void InvalidateMemoryCache(IVssRequestContext requestContext, string key) => this.memoryCache?.Remove(requestContext, key);

    protected void RegisterSqlNotification(
      IVssRequestContext requestContext,
      Guid notificationKey,
      SqlNotificationHandler handler)
    {
      this.memoryCacheSqlNotificationEvent = notificationKey;
      requestContext.GetService<ITeamFoundationSqlNotificationService>().RegisterNotification(requestContext, "Default", this.memoryCacheSqlNotificationEvent, handler, false);
    }

    protected void UnregisterSqlNotification(
      IVssRequestContext requestContext,
      SqlNotificationHandler handler)
    {
      requestContext.GetService<ITeamFoundationSqlNotificationService>().UnregisterNotification(requestContext, "Default", this.memoryCacheSqlNotificationEvent, handler, true);
    }

    protected void SendMemoryCacheInvalidationSqlNotification(
      IVssRequestContext requestContext,
      string eventData)
    {
      requestContext.GetService<TeamFoundationSqlNotificationService>().SendNotification(requestContext, this.memoryCacheSqlNotificationEvent, eventData);
      requestContext.Trace(5108477, TraceLevel.Info, this.Area, this.Layer, "Sent the " + eventData + " notification");
    }

    protected virtual string Layer { get; }
  }
}
