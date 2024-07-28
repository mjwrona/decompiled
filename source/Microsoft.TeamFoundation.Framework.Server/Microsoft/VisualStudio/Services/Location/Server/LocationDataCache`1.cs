// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Location.Server.LocationDataCache`1
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Microsoft.VisualStudio.Services.Location.Server
{
  internal abstract class LocationDataCache<T> : ILocationDataCache<T>
  {
    private readonly VssMemoryCacheList<T, LocationData> m_locationDataMap;
    private readonly IVssMemoryCacheGrouping<T, LocationData, LocationDataKind> m_locationGrouping;
    private volatile int m_cacheVersion;
    private readonly ILocationServiceRedisHelper<T> m_redisHelper;
    private static readonly string s_area = "LocationService";
    private static readonly string s_layer = "LocationCache";

    public LocationDataCache(
      IVssRequestContext requestContext,
      ILocationServiceRedisHelper<T> redisHelper = null,
      IEqualityComparer<T> comparer = null)
    {
      this.m_locationDataMap = new VssMemoryCacheList<T, LocationData>((IVssCachePerformanceProvider) new VssCachePerformanceProvider(this.GetType().Name), comparer ?? (IEqualityComparer<T>) EqualityComparer<T>.Default);
      this.m_locationGrouping = (IVssMemoryCacheGrouping<T, LocationData, LocationDataKind>) new ConcurrentVssMemoryCacheGrouping<T, LocationData, LocationDataKind>(this.m_locationDataMap, (Func<T, LocationData, IEnumerable<LocationDataKind>>) ((k, v) => (IEnumerable<LocationDataKind>) new LocationDataKind[1]
      {
        this.GetKind(k)
      }));
      this.m_redisHelper = redisHelper ?? (ILocationServiceRedisHelper<T>) new LocationServiceRedisHelper<T>();
      this.m_cacheVersion = 0;
    }

    public LocationData GetLocationData(
      IVssRequestContext requestContext,
      T cacheKeyIdentifier,
      Func<IVssRequestContext, T, LocationData> loadData = null)
    {
      ArgumentUtility.CheckGenericForNull((object) cacheKeyIdentifier, nameof (cacheKeyIdentifier));
      LocationData data;
      if (this.m_locationDataMap.TryGetValue(cacheKeyIdentifier, out data) && data.CacheExpirationDate > DateTime.UtcNow)
      {
        requestContext.Trace(1176096515, TraceLevel.Verbose, LocationDataCache<T>.s_area, LocationDataCache<T>.s_layer, "Found key {0} in l1: {1}", (object) cacheKeyIdentifier, (object) data);
        return data;
      }
      int cacheVersion = this.m_cacheVersion;
      requestContext.Trace(1176096516, TraceLevel.Verbose, LocationDataCache<T>.s_area, LocationDataCache<T>.s_layer, "Could not find key {0} in l1, trying Redis", (object) cacheKeyIdentifier);
      bool flag1 = false;
      if (this.CanUseRedis(this.GetKind(cacheKeyIdentifier)))
      {
        data = this.m_redisHelper.GetLocationData(requestContext, cacheKeyIdentifier);
        if (data != null)
        {
          flag1 = true;
          requestContext.Trace(258120625, TraceLevel.Verbose, LocationDataCache<T>.s_area, LocationDataCache<T>.s_layer, "Found key {0} in Redis: {1}", (object) cacheKeyIdentifier, (object) data);
        }
      }
      if (data == null || data.CacheExpirationDate <= DateTime.UtcNow)
      {
        requestContext.Trace(258120626, TraceLevel.Verbose, LocationDataCache<T>.s_area, LocationDataCache<T>.s_layer, "Loading data for key {0} from source", (object) cacheKeyIdentifier);
        data = loadData != null ? loadData(requestContext, cacheKeyIdentifier) : (LocationData) null;
        flag1 = false;
      }
      if (data != null && this.m_cacheVersion == cacheVersion)
      {
        bool flag2 = false;
        lock (this.m_locationDataMap)
        {
          if (this.m_cacheVersion == cacheVersion)
          {
            this.m_locationDataMap[cacheKeyIdentifier] = data;
            flag2 = true;
          }
        }
        if (flag2 && !flag1 && this.CanUseRedis(this.GetKind(cacheKeyIdentifier)))
        {
          requestContext.Trace(258120627, TraceLevel.Verbose, LocationDataCache<T>.s_area, LocationDataCache<T>.s_layer, "Saving data for key {0} into Redis", (object) cacheKeyIdentifier);
          this.m_redisHelper.SetLocationData(requestContext, cacheKeyIdentifier, data);
        }
      }
      return data;
    }

    public void Invalidate(IVssRequestContext requestContext, T cacheKeyIdentifier, bool isAuthor)
    {
      ArgumentUtility.CheckGenericForNull((object) cacheKeyIdentifier, nameof (cacheKeyIdentifier));
      lock (this.m_locationDataMap)
      {
        this.m_locationDataMap.Remove(cacheKeyIdentifier);
        ++this.m_cacheVersion;
      }
      if (!isAuthor || !this.CanUseRedis(this.GetKind(cacheKeyIdentifier)))
        return;
      this.m_redisHelper.TryClearCache(requestContext, cacheKeyIdentifier);
    }

    public void Invalidate(IVssRequestContext requestContext, LocationDataKind kind, bool isAuthor)
    {
      IEnumerable<T> keys = this.GetKeys(requestContext, kind);
      lock (this.m_locationDataMap)
      {
        foreach (T key in keys)
          this.m_locationDataMap.Remove(key);
        ++this.m_cacheVersion;
      }
      if (!isAuthor || !this.CanUseRedis(kind))
        return;
      this.m_redisHelper.TryClearCache(requestContext);
    }

    public void Update(
      IVssRequestContext requestContext,
      T cacheKeyIdentifier,
      Action updateAction)
    {
      updateAction();
      this.Invalidate(requestContext, cacheKeyIdentifier, true);
    }

    private IEnumerable<T> GetKeys(IVssRequestContext requestContext, LocationDataKind kind)
    {
      List<T> keys1 = new List<T>();
      IEnumerable<T> keys2;
      if ((kind == LocationDataKind.Local || kind == LocationDataKind.All) && this.m_locationGrouping.TryGetKeys(LocationDataKind.Local, out keys2))
        keys1.AddRange(keys2);
      IEnumerable<T> keys3;
      if ((kind == LocationDataKind.Remote || kind == LocationDataKind.All) && this.m_locationGrouping.TryGetKeys(LocationDataKind.Remote, out keys3))
        keys1.AddRange(keys3);
      return (IEnumerable<T>) keys1;
    }

    public abstract bool CanUseRedis(LocationDataKind kind);

    public abstract LocationDataKind GetKind(T key);
  }
}
